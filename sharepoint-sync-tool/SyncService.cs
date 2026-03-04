using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Gov.Lclb.Cllb.Interfaces;
using Gov.Lclb.Cllb.Interfaces.Models;
using Microsoft.Extensions.Logging;
using Microsoft.Rest;

namespace SharePointSyncTool
{
  public class SyncService
  {
    private readonly ISharePointFileManager _sharePointManager;
    private readonly IDynamicsClient _dynamicsClient;
    private readonly ILogger _logger;

    // Regex pattern to extract GUID from folder name
    // Format: SomeName_GUIDWITHOUTDASHES where GUID is 32 hex characters
    private static readonly Regex GuidPattern = new Regex(@"_([A-F0-9]{32})$", RegexOptions.IgnoreCase);

    public SyncService(ISharePointFileManager sharePointManager, IDynamicsClient dynamicsClient, ILoggerFactory loggerFactory)
    {
      _sharePointManager = sharePointManager;
      _dynamicsClient = dynamicsClient;
      _logger = loggerFactory.CreateLogger<SyncService>();
    }

    public async Task SyncSharePointFoldersAsync(SyncConfiguration config)
    {
      // Normalize entity name to friendly name (application, account, worker, etc.)
      string normalizedEntityName = NormalizeEntityName(config.EntityName);

      // Get SharePoint internal document library name (adoxio_application, account, etc.)
      string documentLibraryInternalName = SharePointConstants.GetDocumentTemplateUrlPart(normalizedEntityName);

      _logger.LogInformation(
        "Starting SharePoint folder sync - Entity: {EntityName} (normalized: {NormalizedName}), Document Library: {DocumentLibrary}",
        config.EntityName,
        normalizedEntityName,
        documentLibraryInternalName
      );

      // Initialize CSV export files
      string csvFilePath = InitializeCsvExport(normalizedEntityName);
      string errorCsvFilePath = InitializeErrorCsvExport(normalizedEntityName);
      _logger.LogInformation("CSV export file: {CsvFilePath}", csvFilePath);
      _logger.LogInformation("Error CSV file: {ErrorCsvFilePath}", errorCsvFilePath);

      // Check if this is a nested entity type (Contravention or Enforcement Action)
      bool isNestedEntity = IsNestedEntityType(normalizedEntityName);
      List<FolderItem> foldersToSync;

      // For nested entities, the parent document library is always "account"
      // For regular entities, use the entity's own document library
      string parentDocumentLibrary = isNestedEntity ? SharePointConstants.AccountFolderInternalName : documentLibraryInternalName;

      _logger.LogInformation("Parent document library for document locations: {ParentDocumentLibrary}", parentDocumentLibrary);

      if (isNestedEntity)
      {
        // For nested entities, fetch folders from Account folders
        foldersToSync = await GetNestedFoldersAsync(normalizedEntityName, config.ModifiedAfterDateParsed);
      }
      else
      {
        // For regular entities, fetch folders directly from document library
        foldersToSync = await GetFoldersAsync(documentLibraryInternalName, config.ModifiedAfterDateParsed);
      }

      _logger.LogInformation("Found {FolderCount} folders to sync in SharePoint", foldersToSync.Count);

      if (foldersToSync.Count == 0)
      {
        _logger.LogInformation("No folders to sync");
        return;
      }

      // Apply index range if specified
      if (config.StartIndex > 0 || config.EndIndex > 0)
      {
        int startIdx = config.StartIndex;
        int endIdx = config.EndIndex;

        // If both are 0, process all records (no filtering)
        if (startIdx == 0 && endIdx == 0)
        {
          // Process all - do nothing
        }
        else
        {
          // Validate indices
          if (startIdx < 0)
          {
            startIdx = 0;
          }
          if (endIdx > foldersToSync.Count)
          {
            endIdx = foldersToSync.Count;
          }
          if (endIdx <= startIdx && endIdx > 0)
          {
            _logger.LogWarning(
              "EndIndex ({EndIndex}) must be greater than StartIndex ({StartIndex}). Processing all records.",
              endIdx,
              startIdx
            );
          }
          else if (endIdx > 0)
          {
            int count = endIdx - startIdx;
            _logger.LogInformation(
              "Processing records {StartIndex} to {EndIndex} ({Count} folders out of {TotalFolders})",
              startIdx,
              endIdx - 1,
              count,
              foldersToSync.Count
            );
            foldersToSync = foldersToSync.Skip(startIdx).Take(count).ToList();
          }
        }
      }

      // Analyze folder GUID distribution
      var foldersByGuid = AnalyzeFolderDistribution(foldersToSync);
      _logger.LogInformation(
        "Found {UniqueEntities} unique entities with {TotalFolders} total folders",
        foldersByGuid.Count,
        foldersToSync.Count
      );

      // Log entities with multiple folders
      var entitiesWithMultipleFolders = foldersByGuid.Where(kvp => kvp.Value.Count > 1).ToList();
      if (entitiesWithMultipleFolders.Any())
      {
        _logger.LogInformation("{Count} entities have multiple folders that will all be mapped:", entitiesWithMultipleFolders.Count);
        foreach (var entity in entitiesWithMultipleFolders.Take(5))
        {
          _logger.LogInformation(
            "  - GUID {Guid} has {FolderCount} folders: {FolderNames}",
            entity.Key,
            entity.Value.Count,
            string.Join(", ", entity.Value.Take(3).Select(f => f.Name))
          );
        }
        if (entitiesWithMultipleFolders.Count > 5)
        {
          _logger.LogInformation("  ... and {More} more", entitiesWithMultipleFolders.Count - 5);
        }
      }

      // Process folders in batches
      var batches = foldersToSync
        .Select((folder, index) => new { folder, index })
        .GroupBy(x => x.index / config.BatchSize)
        .Select(g => g.Select(x => x.folder).ToList())
        .ToList();

      _logger.LogInformation("Processing {BatchCount} batches of size {BatchSize}", batches.Count, config.BatchSize);

      int totalProcessed = 0;
      int totalCreated = 0;
      int totalSkipped = 0;
      int totalErrors = 0;

      foreach (var batch in batches)
      {
        _logger.LogInformation(
          "Processing batch {BatchNum}/{TotalBatches} ({FolderCount} folders)",
          batches.IndexOf(batch) + 1,
          batches.Count,
          batch.Count
        );

        foreach (var folder in batch)
        {
          totalProcessed++;

          try
          {
            var result = await ProcessFolderAsync(
              folder,
              normalizedEntityName,
              parentDocumentLibrary,
              config.DryRun,
              csvFilePath,
              errorCsvFilePath
            );

            switch (result)
            {
              case SyncResult.Created:
                totalCreated++;
                break;
              case SyncResult.AlreadyExists:
                totalSkipped++;
                break;
              case SyncResult.Error:
                totalErrors++;
                break;
            }
          }
          catch (Exception ex)
          {
            _logger.LogError(ex, "Error processing folder: {FolderName}", folder.Name);
            await WriteErrorToCsvAsync(errorCsvFilePath, folder.Name, ExtractGuidFromFolderName(folder.Name) ?? "N/A", ex.Message);
            totalErrors++;
          }
        }

        // Small delay between batches to avoid overwhelming the service
        if (batches.IndexOf(batch) < batches.Count - 1)
        {
          await Task.Delay(1000);
        }
      }

      _logger.LogInformation("Sync Summary:");
      _logger.LogInformation("  Total Folders Processed: {TotalProcessed}", totalProcessed);
      _logger.LogInformation("  Unique Entities: {UniqueEntities}", foldersByGuid.Count);
      _logger.LogInformation("  Document Locations Created: {TotalCreated}", totalCreated);
      _logger.LogInformation("  Already Exists (Skipped): {TotalSkipped}", totalSkipped);
      _logger.LogInformation("  Errors: {TotalErrors}", totalErrors);

      if (totalCreated > 0)
      {
        _logger.LogInformation(
          "Successfully created {TotalCreated} document location(s) for {UniqueEntities} entity/entities",
          totalCreated,
          foldersByGuid.Count
        );
      }

      _logger.LogInformation("Document location data exported to: {CsvFilePath}", csvFilePath);
      if (totalErrors > 0)
      {
        _logger.LogInformation("Errors logged to: {ErrorCsvFilePath}", errorCsvFilePath);
      }
    }

    private string NormalizeEntityName(string entityName)
    {
      // Convert internal names to friendly names
      return entityName.ToLower() switch
      {
        "account" => "account",
        "contact" => "contact",
        "adoxio_application" => "application",
        "adoxio_worker" => "worker",
        "adoxio_event" => "event",
        "adoxio_licences" => "licence",
        "adoxio_contravention" => "contravention",
        "adoxio_enforcementaction" => "enforcement action",
        "adoxio_specialevent" => "special event",
        "adoxio_complaint" => "complaint",
        _ => entityName.ToLower(),
      };
    }

    private bool IsNestedEntityType(string entityName)
    {
      return entityName.ToLower() == "contravention" || entityName.ToLower() == "enforcement action" || entityName.ToLower() == "incident";
    }

    private string GetNestedFolderInternalName(string entityName)
    {
      // Use SharePointConstants to get the internal SharePoint folder name for this entity
      // This returns the folder name used in SharePoint URLs (e.g., "adoxio_contravention", "adoxio_enforcementaction")
      return SharePointConstants.GetDocumentTemplateUrlPart(entityName);
    }

    private async Task<List<FolderItem>> GetNestedFoldersAsync(string entityName, DateTime? modifiedAfter)
    {
      try
      {
        var onPremManager = (OnPremSharePointFileManager)_sharePointManager;
        var nestedFolders = new List<FolderItem>();

        // Determine the nested folder name based on entity type
        string nestedFolderName = GetNestedFolderInternalName(entityName);

        _logger.LogInformation(
          "Fetching {EntityName} folders nested under Account folders (looking for '{NestedFolderName}' subfolders)",
          entityName,
          nestedFolderName
        );

        // Get all Account folders
        var accountFolders = await onPremManager.GetFoldersInDocumentLibraryAfterDate(
          SharePointConstants.AccountFolderInternalName,
          modifiedAfter.Value
        );

        _logger.LogInformation(
          "Found {AccountFolderCount} Account folders to check for nested {EntityName} folders",
          accountFolders.Count,
          entityName
        );

        int accountsWithNestedFolders = 0;

        // For each Account folder, check for nested entity folders
        foreach (var accountFolder in accountFolders)
        {
          try
          {
            // Get child folders of this account folder
            var childFolders = await onPremManager.GetChildFolders(accountFolder.ServerRelativeUrl);

            // Find the specific nested folder (adoxio_contravention or adoxio_enforcementaction)
            var nestedFolder = childFolders?.FirstOrDefault(f => f.Name.Equals(nestedFolderName, StringComparison.OrdinalIgnoreCase));

            if (nestedFolder != null)
            {
              accountsWithNestedFolders++;

              // Get the entity folders within the nested folder
              var entityFolders = await onPremManager.GetChildFolders(nestedFolder.ServerRelativeUrl);

              if (entityFolders != null && entityFolders.Any())
              {
                _logger.LogDebug(
                  "Found {FolderCount} {EntityName} folders in Account folder '{AccountFolderName}'",
                  entityFolders.Count,
                  entityName,
                  accountFolder.Name
                );
                nestedFolders.AddRange(entityFolders);
              }
            }
          }
          catch (Exception ex)
          {
            _logger.LogWarning(
              ex,
              "Error checking Account folder '{AccountFolderName}' for nested {EntityName} folders",
              accountFolder.Name,
              entityName
            );
          }
        }

        _logger.LogInformation(
          "Found {AccountsWithNestedFolders}/{TotalAccounts} Account folders containing {EntityName} subfolders, total {EntityName} folders: {TotalNestedFolders}",
          accountsWithNestedFolders,
          accountFolders.Count,
          entityName,
          entityName,
          nestedFolders.Count
        );

        return nestedFolders;
      }
      catch (Exception ex)
      {
        _logger.LogError(ex, "Error fetching nested folders from SharePoint for entity: {EntityName}", entityName);
        throw;
      }
    }

    private async Task<List<FolderItem>> GetFoldersAsync(string documentLibrary, DateTime? modifiedAfter)
    {
      try
      {
        // Use OnPrem SharePoint implementation
        var onPremManager = (OnPremSharePointFileManager)_sharePointManager;
        return await onPremManager.GetFoldersInDocumentLibraryAfterDate(documentLibrary, modifiedAfter.Value);
      }
      catch (Exception ex)
      {
        _logger.LogError(ex, "Error fetching folders from SharePoint");
        throw;
      }
    }

    private List<FolderItem> FilterFoldersByDate(List<FolderItem> folders, DateTime? modifiedAfter)
    {
      if (modifiedAfter == null)
      {
        return folders;
      }

      // Note: FolderItem doesn't have a TimeCreated property in the current model
      // You might need to fetch additional metadata for filtering by date
      // For now, we'll return all folders and log a warning
      _logger.LogWarning("Filtering by creation date is not implemented. Processing all folders.");
      _logger.LogWarning("To implement date filtering, the FolderItem class needs to include TimeCreated property.");

      return folders;
    }

    private Dictionary<string, List<FolderItem>> AnalyzeFolderDistribution(List<FolderItem> folders)
    {
      var foldersByGuid = new Dictionary<string, List<FolderItem>>();

      foreach (var folder in folders)
      {
        var guid = ExtractGuidFromFolderName(folder.Name);
        if (guid != null)
        {
          if (!foldersByGuid.ContainsKey(guid))
          {
            foldersByGuid[guid] = new List<FolderItem>();
          }
          foldersByGuid[guid].Add(folder);
        }
      }

      return foldersByGuid;
    }

    private async Task<SyncResult> ProcessFolderAsync(
      FolderItem folder,
      string entityName,
      string documentLibrary,
      bool dryRun,
      string csvFilePath,
      string errorCsvFilePath
    )
    {
      // Extract GUID from folder name
      var guid = ExtractGuidFromFolderName(folder.Name);

      if (guid == null)
      {
        _logger.LogWarning("Could not extract GUID from folder name: {FolderName}. Skipping.", folder.Name);
        return SyncResult.Error;
      }

      // Calculate the relative URL for the document location
      // For all entities, this is just the folder name
      string relativeUrl = CalculateRelativeUrl(folder, entityName, documentLibrary);

      _logger.LogDebug(
        "Processing folder: {FolderName}, GUID: {Guid}, ServerRelativeUrl: {ServerUrl}, CalculatedRelativeUrl: {RelativeUrl}",
        folder.Name,
        guid,
        folder.ServerRelativeUrl,
        relativeUrl
      );

      // Note: Multiple folders can have the same GUID.
      // Each folder will create its own document location record,
      // all linked to the same entity via the GUID.

      // Check if document location already exists and validate it
      var existingLocationId = await GetExistingDocumentLocationAsync(relativeUrl, guid);

      if (existingLocationId != null)
      {
        _logger.LogDebug("Document location already exists for: {FolderName} (ID: {LocationId})", folder.Name, existingLocationId);
        return SyncResult.AlreadyExists;
      }

      // Write to CSV export file (regardless of dry run mode)
      await WriteToCsvAsync(csvFilePath, entityName, documentLibrary, folder.Name, relativeUrl, guid);

      // Create document location
      if (dryRun)
      {
        // In dry run mode, we just log what would be created, and return, without making any changes
        _logger.LogInformation(
          "[DRY RUN] Would create document location - FolderName: {FolderName}, RelativeUrl: {RelativeUrl}, RegardingEntity: {EntityName}, GUID: {Guid}",
          folder.Name,
          relativeUrl,
          entityName,
          guid
        );
        return SyncResult.Created;
      }

      // For nested entities, create hierarchical document locations
      bool created;
      if (IsNestedEntityType(entityName))
      {
        created = await CreateNestedDocumentLocationHierarchyAsync(entityName, guid, relativeUrl, folder, csvFilePath);
      }
      else
      {
        created = await CreateDocumentLocationAsync(entityName, documentLibrary, guid, relativeUrl);
      }

      if (created)
      {
        _logger.LogInformation(
          "Created document location - FolderName: {FolderName}, RelativeUrl: {RelativeUrl}, RegardingEntity: {EntityName}, GUID: {Guid}",
          folder.Name,
          relativeUrl,
          entityName,
          guid
        );
        return SyncResult.Created;
      }
      else
      {
        _logger.LogError("Failed to create document location for: {FolderName}", folder.Name);
        await WriteErrorToCsvAsync(errorCsvFilePath, folder.Name, guid, "Failed to create document location");
        return SyncResult.Error;
      }
    }

    private string? ExtractGuidFromFolderName(string folderName)
    {
      // Folder names are in format: Name_GUIDWITHOUTDASHES
      // Example: "John Doe_550E8400E29B41D4A716446655440000"
      // We need to extract the GUID and format it with dashes

      var match = GuidPattern.Match(folderName);
      if (!match.Success)
      {
        return null;
      }

      var guidWithoutDashes = match.Groups[1].Value;

      // Convert to standard GUID format with dashes
      // Format: xxxxxxxx-xxxx-xxxx-xxxx-xxxxxxxxxxxx
      if (guidWithoutDashes.Length != 32)
      {
        return null;
      }

      var formattedGuid = string.Format(
        "{0}-{1}-{2}-{3}-{4}",
        guidWithoutDashes.Substring(0, 8),
        guidWithoutDashes.Substring(8, 4),
        guidWithoutDashes.Substring(12, 4),
        guidWithoutDashes.Substring(16, 4),
        guidWithoutDashes.Substring(20, 12)
      );

      // Validate it's a proper GUID
      if (Guid.TryParse(formattedGuid, out _))
      {
        return formattedGuid.ToLower();
      }

      return null;
    }

    private string CalculateRelativeUrl(FolderItem folder, string entityName, string documentLibrary)
    {
      // For all entities (including nested ones), the relative URL is just the folder name
      // Nested entities will create multiple document location records, one for each level
      return folder.Name;
    }

    private async Task<string?> GetExistingDocumentLocationAsync(string relativeUrl, string entityGuid)
    {
      try
      {
        var sanitizedUrl = relativeUrl.Replace("'", "''");
        var filter = $"relativeurl eq '{sanitizedUrl}'";

        var locations = await Task.Run(() => _dynamicsClient.Sharepointdocumentlocations.Get(filter: filter));

        if (locations?.Value == null || !locations.Value.Any())
        {
          return null;
        }

        // Process each found location
        string validLocationId = null;

        foreach (var location in locations.Value)
        {
          // Check if this location has a regarding object (entity link)
          if (string.IsNullOrEmpty(location._regardingobjectidValue))
          {
            _logger.LogWarning(
              "Orphan document location found (no entity link): {LocationId} for relativeUrl {RelativeUrl}. Skipping to avoid duplicates.",
              location.Sharepointdocumentlocationid,
              relativeUrl
            );
            // Don't delete orphans in this tool - just skip them
            // The existing document location can be cleaned up separately
            continue;
          }

          // Validate the regarding object matches our expected GUID
          var locationEntityGuid = location._regardingobjectidValue?.ToLower();
          var expectedGuid = entityGuid.ToLower();

          if (locationEntityGuid == expectedGuid)
          {
            // Perfect match - document location exists and points to correct entity
            _logger.LogDebug(
              "Valid document location found: {LocationId} for relativeUrl {RelativeUrl}, linked to entity {Guid}",
              location.Sharepointdocumentlocationid,
              relativeUrl,
              entityGuid
            );
            validLocationId = location.Sharepointdocumentlocationid;
          }
          else
          {
            // Document location exists but points to different entity
            _logger.LogWarning(
              "Document location {LocationId} for relativeUrl {RelativeUrl} exists but is linked to different entity. Expected: {ExpectedGuid}, Found: {FoundGuid}. Skipping to avoid duplicates.",
              location.Sharepointdocumentlocationid,
              relativeUrl,
              expectedGuid,
              locationEntityGuid
            );
          }
        }

        if (validLocationId != null && locations.Value.Count > 1)
        {
          _logger.LogWarning(
            "Multiple document locations found for relativeUrl {RelativeUrl}. This may indicate data inconsistency.",
            relativeUrl
          );
        }

        return validLocationId;
      }
      catch (HttpOperationException ex)
      {
        _logger.LogError(ex, "Error checking if document location exists for relativeUrl: {RelativeUrl}", relativeUrl);
        return null;
      }
    }

    private async Task<bool> CreateDocumentLocationAsync(string entityName, string documentLibrary, string entityGuid, string relativeUrl)
    {
      try
      {
        // Get parent document library location
        var parentDocumentLibraryReference = GetDocumentLocationReferenceByRelativeURL(documentLibrary);

        if (parentDocumentLibraryReference == null)
        {
          _logger.LogError("Parent document library not found: {DocumentLibrary}", documentLibrary);
          return false;
        }

        // Create the SharePointDocumentLocation entity
        var documentLocation = new MicrosoftDynamicsCRMsharepointdocumentlocation
        {
          Relativeurl = relativeUrl,
          Description = GetDescriptionForEntity(entityName),
          Name = relativeUrl, // Use relative URL as name for nested entities
        };

        // Set the parent document library reference
        documentLocation.ParentsiteorlocationSharepointdocumentlocationODataBind = _dynamicsClient.GetEntityURI(
          "sharepointdocumentlocations",
          parentDocumentLibraryReference
        );

        // Set the regarding object based on entity type
        SetRegardingObject(documentLocation, entityName, entityGuid);

        // Log what we're about to create for debugging
        _logger.LogDebug(
          "Creating document location - Name: {Name}, RelativeUrl: {RelativeUrl}, Description: {Description}, Parent: {Parent}, RegardingEntity: {EntityName}, EntityGuid: {Guid}",
          documentLocation.Name,
          documentLocation.Relativeurl,
          documentLocation.Description,
          documentLibrary,
          entityName,
          entityGuid
        );

        // Double-check one more time before creating to avoid race conditions
        var finalCheck = await GetExistingDocumentLocationAsync(relativeUrl, entityGuid);
        if (finalCheck != null)
        {
          _logger.LogInformation("Document location was created by another process for {RelativeUrl}. Skipping creation.", relativeUrl);
          return true; // Consider this a success since the record exists
        }

        // Create the document location
        var result = await Task.Run(() => _dynamicsClient.Sharepointdocumentlocations.Create(documentLocation));

        if (result != null && !string.IsNullOrEmpty(result.Sharepointdocumentlocationid))
        {
          _logger.LogDebug(
            "Created document location {LocationId} - RelativeUrl: {RelativeUrl}, RegardingEntity: {EntityName}, GUID: {Guid}",
            result.Sharepointdocumentlocationid,
            relativeUrl,
            entityName,
            entityGuid
          );

          // Add reference to the entity
          // Note: If multiple folders share the same GUID, each will create its own
          // document location and all will be linked to the same entity
          await AddReferenceToEntity(entityName, entityGuid, result.Sharepointdocumentlocationid);
          return true;
        }

        return false;
      }
      catch (HttpOperationException ex)
      {
        // Log the response content to see what Dynamics is complaining about
        var errorContent = ex.Response?.Content;
        var statusCode = ex.Response?.StatusCode;

        _logger.LogError(
          "HTTP error creating document location - StatusCode: {StatusCode}, EntityName: {EntityName}, GUID: {Guid}, RelativeUrl: {RelativeUrl}",
          statusCode,
          entityName,
          entityGuid,
          relativeUrl
        );

        if (!string.IsNullOrEmpty(errorContent))
        {
          _logger.LogError("Error response from Dynamics: {ErrorContent}", errorContent);
        }

        // Check if this is a duplicate key error
        if (
          errorContent != null
          && (errorContent.Contains("duplicate") || errorContent.Contains("already exists") || errorContent.Contains("duplicate key"))
        )
        {
          _logger.LogWarning(
            "Document location for {RelativeUrl} already exists (detected during creation). This may be a race condition.",
            relativeUrl
          );

          // Verify it exists now
          var verification = await GetExistingDocumentLocationAsync(relativeUrl, entityGuid);
          if (verification != null)
          {
            _logger.LogInformation("Verified existing document location: {LocationId}", verification);
            return true; // Record exists, so this is effectively successful
          }
        }

        _logger.LogError(ex, "Error creating document location for entity {EntityName}, GUID: {Guid}", entityName, entityGuid);
        return false;
      }
      catch (Exception ex)
      {
        _logger.LogError(ex, "Unexpected error creating document location for entity {EntityName}, GUID: {Guid}", entityName, entityGuid);
        return false;
      }
    }

    private string? GetDocumentLocationReferenceByRelativeURL(string relativeUrl)
    {
      try
      {
        var sanitized = relativeUrl.Replace("'", "''");
        var filter = $"relativeurl eq '{sanitized}'";

        _logger.LogDebug("Looking up parent document library with relativeurl: {RelativeUrl}", relativeUrl);

        var locations = _dynamicsClient.Sharepointdocumentlocations.Get(filter: filter);

        var location = locations?.Value?.FirstOrDefault();

        if (location == null)
        {
          _logger.LogError(
            "Parent document library location not found with relativeurl '{RelativeUrl}'. "
              + "The parent document library must exist in Dynamics before syncing folders. "
              + "Please ensure SharePoint integration is properly configured and the document library location exists.",
            relativeUrl
          );

          // Try to list available document libraries to help with troubleshooting
          try
          {
            var allLocations = _dynamicsClient.Sharepointdocumentlocations.Get(filter: "regardingobjectid eq null", top: 10);

            if (allLocations?.Value?.Any() == true)
            {
              _logger.LogInformation("Available document library locations:");
              foreach (var loc in allLocations.Value.Take(10))
              {
                _logger.LogInformation("  - RelativeUrl: '{RelativeUrl}', Name: '{Name}'", loc.Relativeurl, loc.Name);
              }
            }
          }
          catch
          {
            // Ignore errors when trying to list available locations
          }

          return null;
        }

        _logger.LogDebug(
          "Found parent document library location: ID={LocationId}, Name={Name}",
          location.Sharepointdocumentlocationid,
          location.Name
        );

        return location.Sharepointdocumentlocationid;
      }
      catch (HttpOperationException ex)
      {
        _logger.LogError(ex, "Error getting document location reference for: {RelativeUrl}", relativeUrl);
        return null;
      }
    }

    /// <summary>
    /// Creates hierarchical document locations for nested entities (contraventions, enforcement actions).
    /// Creates 3 document location records: Account level, Entity type level, and Entity specific level.
    /// </summary>
    private async Task<bool> CreateNestedDocumentLocationHierarchyAsync(
      string entityName,
      string entityGuid,
      string entityFolderName,
      FolderItem folder,
      string csvFilePath
    )
    {
      try
      {
        // Parse the ServerRelativeUrl to extract the three components
        // Format: /account/ACCOUNT_NAME_GUID/adoxio_contravention/CONTRAVENTION_NAME_GUID
        var serverRelativeUrl = folder.ServerRelativeUrl;
        var accountLibraryPath = "/" + SharePointConstants.AccountFolderInternalName + "/";
        var startIndex = serverRelativeUrl.IndexOf(accountLibraryPath);

        if (startIndex < 0)
        {
          _logger.LogError("Failed to parse ServerRelativeUrl for nested entity: {ServerRelativeUrl}", serverRelativeUrl);
          return false;
        }

        // Extract path after "/account/"
        var pathAfterAccount = serverRelativeUrl.Substring(startIndex + accountLibraryPath.Length);
        var pathParts = pathAfterAccount.Split('/');

        if (pathParts.Length < 3)
        {
          _logger.LogError("Invalid path structure for nested entity. Expected at least 3 parts: {ServerRelativeUrl}", serverRelativeUrl);
          return false;
        }

        var accountFolderName = pathParts[0]; // e.g., "myaccount_123"
        var entityTypeFolderName = pathParts[1]; // e.g., "adoxio_contravention"
        var entitySpecificFolderName = pathParts[2]; // e.g., "mycontravention_456"

        // Extract the account GUID from the account folder name
        var accountGuid = ExtractGuidFromFolderName(accountFolderName);
        if (accountGuid == null)
        {
          _logger.LogError("Failed to extract account GUID from folder name: {FolderName}", accountFolderName);
          return false;
        }

        _logger.LogDebug(
          "Creating hierarchical document locations - Account: {AccountFolder}, EntityType: {EntityTypeFolder}, Entity: {EntityFolder}",
          accountFolderName,
          entityTypeFolderName,
          entitySpecificFolderName
        );

        // Level 1: Create or get Account document location
        var (accountDocLocationId, accountWasCreated) = await GetOrCreateAccountDocumentLocationAsync(accountGuid, accountFolderName);

        if (accountDocLocationId == null)
        {
          _logger.LogError("Failed to create/get account document location for account: {AccountGuid}", accountGuid);
          return false;
        }

        // Write Level 1 to CSV only if it was newly created
        if (accountWasCreated)
        {
          await WriteToCsvAsync(
            csvFilePath,
            "account",
            SharePointConstants.AccountFolderInternalName,
            accountFolderName,
            accountFolderName,
            accountGuid
          );
        }

        // Level 2: Create or get Entity Type folder document location
        var (entityTypeDocLocationId, entityTypeFolderWasCreated) = await GetOrCreateEntityTypeFolderDocumentLocationAsync(
          entityTypeFolderName,
          accountDocLocationId
        );

        if (entityTypeDocLocationId == null)
        {
          _logger.LogError("Failed to create/get entity type folder document location for: {EntityTypeFolder}", entityTypeFolderName);
          return false;
        }

        // Write Level 2 to CSV only if it was newly created
        if (entityTypeFolderWasCreated)
        {
          await WriteCsvForEntityTypeFolder(csvFilePath, entityTypeFolderName, accountFolderName);
        }

        // Level 3: Create the Entity-specific document location
        var entityDocLocationCreated = await CreateEntityDocumentLocationAsync(
          entityName,
          entityGuid,
          entitySpecificFolderName,
          entityTypeDocLocationId
        );

        return entityDocLocationCreated;
      }
      catch (Exception ex)
      {
        _logger.LogError(
          ex,
          "Error creating nested document location hierarchy for entity {EntityName}, GUID: {Guid}",
          entityName,
          entityGuid
        );
        return false;
      }
    }

    /// <summary>
    /// Gets or creates the Account-level document location (Level 1)
    /// Returns a tuple of (locationId, wasCreated)
    /// </summary>
    private async Task<(string? locationId, bool wasCreated)> GetOrCreateAccountDocumentLocationAsync(
      string accountGuid,
      string accountFolderName
    )
    {
      // Check if account document location already exists
      var existingLocationId = await GetExistingDocumentLocationAsync(accountFolderName, accountGuid);

      if (existingLocationId != null)
      {
        _logger.LogDebug(
          "Account document location already exists: {LocationId} for account {AccountGuid}",
          existingLocationId,
          accountGuid
        );
        return (existingLocationId, false);
      }

      // Get the parent Account document library reference
      var accountLibraryReference = GetDocumentLocationReferenceByRelativeURL(SharePointConstants.AccountFolderInternalName);

      if (accountLibraryReference == null)
      {
        _logger.LogError("Account document library not found. Cannot create account document location.");
        return (null, false);
      }

      // Create the Account document location
      var accountDocLocation = new MicrosoftDynamicsCRMsharepointdocumentlocation
      {
        Relativeurl = accountFolderName,
        Description = "Account Files",
        Name = accountFolderName,
      };

      accountDocLocation.ParentsiteorlocationSharepointdocumentlocationODataBind = _dynamicsClient.GetEntityURI(
        "sharepointdocumentlocations",
        accountLibraryReference
      );

      // Set regarding to the account entity
      var accountEntityUri = _dynamicsClient.GetEntityURI("accounts", accountGuid);
      accountDocLocation.RegardingobjectIdAccountODataBind = accountEntityUri;

      _logger.LogDebug(
        "Creating account document location - Name: {Name}, RelativeUrl: {RelativeUrl}, AccountGuid: {Guid}",
        accountDocLocation.Name,
        accountDocLocation.Relativeurl,
        accountGuid
      );

      // Double-check before creating
      var finalCheck = await GetExistingDocumentLocationAsync(accountFolderName, accountGuid);
      if (finalCheck != null)
      {
        _logger.LogInformation("Account document location was created by another process. Using existing: {LocationId}", finalCheck);
        return (finalCheck, false);
      }

      try
      {
        var result = await Task.Run(() => _dynamicsClient.Sharepointdocumentlocations.Create(accountDocLocation));

        if (result != null && !string.IsNullOrEmpty(result.Sharepointdocumentlocationid))
        {
          _logger.LogDebug(
            "Created account document location: {LocationId} for account {AccountGuid}",
            result.Sharepointdocumentlocationid,
            accountGuid
          );
          return (result.Sharepointdocumentlocationid, true);
        }
      }
      catch (HttpOperationException ex)
      {
        var errorContent = ex.Response?.Content;

        // Check if this is a duplicate key error
        if (
          errorContent != null
          && (errorContent.Contains("duplicate") || errorContent.Contains("already exists") || errorContent.Contains("duplicate key"))
        )
        {
          _logger.LogWarning("Account document location already exists (detected during creation). Retrieving existing record.");

          var verification = await GetExistingDocumentLocationAsync(accountFolderName, accountGuid);
          if (verification != null)
          {
            return (verification, false);
          }
        }

        _logger.LogError(ex, "Error creating account document location for account {AccountGuid}", accountGuid);
      }

      return (null, false);
    }

    /// <summary>
    /// Gets or creates the Entity Type folder document location (Level 2)
    /// Returns a tuple of (locationId, wasCreated)
    /// </summary>
    private async Task<(string? locationId, bool wasCreated)> GetOrCreateEntityTypeFolderDocumentLocationAsync(
      string entityTypeFolderName,
      string parentAccountDocLocationId
    )
    {
      // For entity type folders, we don't have a regarding entity, so we search by relativeUrl and parent
      // We need to find a document location with this relativeUrl AND this specific parent
      try
      {
        var sanitizedUrl = entityTypeFolderName.Replace("'", "''");
        var filter = $"relativeurl eq '{sanitizedUrl}' and _parentsiteorlocation_value eq {parentAccountDocLocationId}";

        var locations = await Task.Run(() => _dynamicsClient.Sharepointdocumentlocations.Get(filter: filter));

        if (locations?.Value != null && locations.Value.Any())
        {
          var location = locations.Value.First();
          _logger.LogDebug(
            "Entity type folder document location already exists: {LocationId} for {EntityTypeFolder}",
            location.Sharepointdocumentlocationid,
            entityTypeFolderName
          );
          return (location.Sharepointdocumentlocationid, false);
        }
      }
      catch (HttpOperationException ex)
      {
        _logger.LogWarning(
          ex,
          "Error checking for existing entity type folder document location: {EntityTypeFolder}",
          entityTypeFolderName
        );
      }

      // Create the Entity Type folder document location
      var entityTypeDocLocation = new MicrosoftDynamicsCRMsharepointdocumentlocation
      {
        Relativeurl = entityTypeFolderName,
        Description = string.Empty, // Blank description as specified
        Name = "Documents on Default Site 1",
      };

      entityTypeDocLocation.ParentsiteorlocationSharepointdocumentlocationODataBind = _dynamicsClient.GetEntityURI(
        "sharepointdocumentlocations",
        parentAccountDocLocationId
      );

      // No regarding object for entity type folders

      _logger.LogDebug(
        "Creating entity type folder document location - Name: {Name}, RelativeUrl: {RelativeUrl}, ParentId: {ParentId}",
        entityTypeDocLocation.Name,
        entityTypeDocLocation.Relativeurl,
        parentAccountDocLocationId
      );

      try
      {
        var result = await Task.Run(() => _dynamicsClient.Sharepointdocumentlocations.Create(entityTypeDocLocation));

        if (result != null && !string.IsNullOrEmpty(result.Sharepointdocumentlocationid))
        {
          _logger.LogDebug(
            "Created entity type folder document location: {LocationId} for {EntityTypeFolder}",
            result.Sharepointdocumentlocationid,
            entityTypeFolderName
          );
          return (result.Sharepointdocumentlocationid, true);
        }
      }
      catch (HttpOperationException ex)
      {
        var errorContent = ex.Response?.Content;

        // Check if this is a duplicate key error
        if (
          errorContent != null
          && (errorContent.Contains("duplicate") || errorContent.Contains("already exists") || errorContent.Contains("duplicate key"))
        )
        {
          _logger.LogWarning("Entity type folder document location already exists (detected during creation). Retrieving existing record.");

          // Try to retrieve it again
          var sanitizedUrl = entityTypeFolderName.Replace("'", "''");
          var filter = $"relativeurl eq '{sanitizedUrl}' and _parentsiteorlocation_value eq {parentAccountDocLocationId}";
          var locations = await Task.Run(() => _dynamicsClient.Sharepointdocumentlocations.Get(filter: filter));

          if (locations?.Value != null && locations.Value.Any())
          {
            return (locations.Value.First().Sharepointdocumentlocationid, false);
          }
        }

        _logger.LogError(ex, "Error creating entity type folder document location for {EntityTypeFolder}", entityTypeFolderName);
      }

      return (null, false);
    }

    /// <summary>
    /// Creates the Entity-specific document location (Level 3)
    /// </summary>
    private async Task<bool> CreateEntityDocumentLocationAsync(
      string entityName,
      string entityGuid,
      string entityFolderName,
      string parentEntityTypeDocLocationId
    )
    {
      // Check if entity document location already exists
      var existingLocationId = await GetExistingDocumentLocationAsync(entityFolderName, entityGuid);

      if (existingLocationId != null)
      {
        _logger.LogDebug("Entity document location already exists: {LocationId} for entity {EntityGuid}", existingLocationId, entityGuid);
        return true;
      }

      // Create the Entity document location
      var entityDocLocation = new MicrosoftDynamicsCRMsharepointdocumentlocation
      {
        Relativeurl = entityFolderName,
        Description = GetDescriptionForEntity(entityName),
        Name = entityFolderName,
      };

      entityDocLocation.ParentsiteorlocationSharepointdocumentlocationODataBind = _dynamicsClient.GetEntityURI(
        "sharepointdocumentlocations",
        parentEntityTypeDocLocationId
      );

      // Set the regarding object based on entity type
      SetRegardingObject(entityDocLocation, entityName, entityGuid);

      _logger.LogDebug(
        "Creating entity document location - Name: {Name}, RelativeUrl: {RelativeUrl}, EntityName: {EntityName}, EntityGuid: {Guid}, ParentId: {ParentId}",
        entityDocLocation.Name,
        entityDocLocation.Relativeurl,
        entityName,
        entityGuid,
        parentEntityTypeDocLocationId
      );

      // Double-check before creating
      var finalCheck = await GetExistingDocumentLocationAsync(entityFolderName, entityGuid);
      if (finalCheck != null)
      {
        _logger.LogInformation("Entity document location was created by another process. Skipping: {LocationId}", finalCheck);
        return true;
      }

      try
      {
        var result = await Task.Run(() => _dynamicsClient.Sharepointdocumentlocations.Create(entityDocLocation));

        if (result != null && !string.IsNullOrEmpty(result.Sharepointdocumentlocationid))
        {
          _logger.LogDebug(
            "Created entity document location: {LocationId} for entity {EntityGuid}",
            result.Sharepointdocumentlocationid,
            entityGuid
          );

          // Add reference to the entity
          await AddReferenceToEntity(entityName, entityGuid, result.Sharepointdocumentlocationid);
          return true;
        }

        return false;
      }
      catch (HttpOperationException ex)
      {
        var errorContent = ex.Response?.Content;

        // Check if this is a duplicate key error
        if (
          errorContent != null
          && (errorContent.Contains("duplicate") || errorContent.Contains("already exists") || errorContent.Contains("duplicate key"))
        )
        {
          _logger.LogWarning("Entity document location already exists (detected during creation).");

          var verification = await GetExistingDocumentLocationAsync(entityFolderName, entityGuid);
          if (verification != null)
          {
            return true;
          }
        }

        _logger.LogError(ex, "Error creating entity document location for entity {EntityName}, GUID: {Guid}", entityName, entityGuid);
        return false;
      }
    }

    private void SetRegardingObject(MicrosoftDynamicsCRMsharepointdocumentlocation documentLocation, string entityName, string entityGuid)
    {
      var entityUri = _dynamicsClient.GetEntityURI(GetEntityPluralName(entityName), entityGuid);

      switch (entityName.ToLower())
      {
        case "account":
          documentLocation.RegardingobjectIdAccountODataBind = entityUri;
          break;
        case "application":
          documentLocation.RegardingobjectidAdoxioApplicationODataBind = entityUri;
          break;
        case "contact":
          documentLocation.RegardingobjectIdContactODataBind = entityUri;
          break;
        case "worker":
          documentLocation.RegardingobjectidWorkerApplicationODataBind = entityUri;
          break;
        case "event":
          documentLocation.RegardingobjectIdEventODataBind = entityUri;
          break;
        case "licence":
          documentLocation.RegardingobjectIdLicenceODataBind = entityUri;
          break;
        case "contravention":
          documentLocation.RegardingobjectidAdoxioContraventionODataBind = entityUri;
          break;
        case "enforcement action":
          documentLocation.RegardingobjectidAdoxioEnforcementactionODataBind = entityUri;
          break;
        case "special event":
          documentLocation.RegardingobjectidAdoxioSpecialeventODataBind = entityUri;
          break;
        case "incident":
          documentLocation.RegardingobjectIdIncidentODataBind = entityUri;
          break;
        case "complaint":
          documentLocation.RegardingobjectidAdoxioComplaintODataBind = entityUri;
          break;
        default:
          _logger.LogWarning("Unknown entity type: {EntityName}. Document location may not be properly linked.", entityName);
          break;
      }
    }

    private async Task AddReferenceToEntity(string entityName, string entityGuid, string documentLocationId)
    {
      // The relationship is already established via RegardingObject set in CreateDocumentLocationAsync
      // Setting the lookup field creates the bidirectional relationship automatically in Dynamics
      // AddReference is redundant and not needed for any entity type
      _logger.LogDebug(
        "Relationship between {EntityName} entity {Guid} and document location {LocationId} established via RegardingObject",
        entityName,
        entityGuid,
        documentLocationId
      );
      await Task.CompletedTask; // Maintain async signature for compatibility
    }

    private string GetDescriptionForEntity(string entityName)
    {
      return entityName.ToLower() switch
      {
        "account" => "Account Files",
        "application" => "Application Files",
        "contact" => "Contact Files",
        "worker" => "Worker Files",
        "event" => "Event Files",
        "licence" => "Licence Files",
        "contravention" => "Contravention Files",
        "enforcement action" => "Enforcement Action Files",
        "special event" => "Special Event Files",
        "incident" => "Incident Files",
        "complaint" => "Complaint Files",
        _ => $"{entityName} Files",
      };
    }

    private string GetEntityPluralName(string entityName)
    {
      return entityName.ToLower() switch
      {
        "account" => "accounts",
        "application" => "adoxio_applications",
        "contact" => "contacts",
        "worker" => "adoxio_workers",
        "event" => "adoxio_events",
        "licence" => "adoxio_licences",
        "contravention" => "adoxio_contraventions",
        "enforcement action" => "adoxio_enforcementactions",
        "special event" => "adoxio_specialevents",
        "incident" => "incidents",
        "complaint" => "adoxio_complaints",
        _ => entityName + "s",
      };
    }

    /// <summary>
    /// Initialize CSV export file with header row
    /// </summary>
    private string InitializeCsvExport(string entityName)
    {
      string timestamp = DateTime.Now.ToString("yyyyMMdd-HHmmss");
      string fileName = $"sharepoint-sync-{entityName.Replace(" ", "-")}-{timestamp}.csv";
      string filePath = Path.Combine(Directory.GetCurrentDirectory(), fileName);

      // Write CSV header
      var header =
        "FolderName,RelativeUrl,Name,Description,RegardingEntityType,RegardingEntityGuid,RegardingObjectODataBind,ParentDocumentLibrary";
      File.WriteAllText(filePath, header + Environment.NewLine, Encoding.UTF8);

      return filePath;
    }

    /// <summary>
    /// Initialize error CSV export file with header row
    /// </summary>
    private string InitializeErrorCsvExport(string entityName)
    {
      string timestamp = DateTime.Now.ToString("yyyyMMdd-HHmmss");
      string fileName = $"sharepoint-sync-{entityName.Replace(" ", "-")}-errors-{timestamp}.csv";
      string filePath = Path.Combine(Directory.GetCurrentDirectory(), fileName);

      // Write CSV header
      var header = "FolderName,EntityGuid,ErrorMessage,Timestamp";
      File.WriteAllText(filePath, header + Environment.NewLine, Encoding.UTF8);

      return filePath;
    }

    /// <summary>
    /// Write an error record to error CSV file
    /// </summary>
    private async Task WriteErrorToCsvAsync(string errorCsvFilePath, string folderName, string entityGuid, string errorMessage)
    {
      try
      {
        var timestamp = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");

        // Escape fields that might contain commas or quotes
        var csvLine = string.Join(
          ",",
          EscapeCsvField(folderName),
          EscapeCsvField(entityGuid),
          EscapeCsvField(errorMessage),
          EscapeCsvField(timestamp)
        );

        await File.AppendAllTextAsync(errorCsvFilePath, csvLine + Environment.NewLine, Encoding.UTF8);
      }
      catch (Exception ex)
      {
        _logger.LogError(ex, "Error writing to error CSV file: {ErrorCsvFilePath}", errorCsvFilePath);
      }
    }

    /// <summary>
    /// Write a document location record to CSV file
    /// </summary>
    private async Task WriteToCsvAsync(
      string csvFilePath,
      string entityName,
      string documentLibrary,
      string folderName,
      string relativeUrl,
      string entityGuid
    )
    {
      try
      {
        var description = GetDescriptionForEntity(entityName);
        var regardingEntityPluralName = GetEntityPluralName(entityName);
        var regardingODataBind = $"/{regardingEntityPluralName}({entityGuid})";

        // Escape fields that might contain commas or quotes
        var csvLine = string.Join(
          ",",
          EscapeCsvField(folderName),
          EscapeCsvField(relativeUrl),
          EscapeCsvField(relativeUrl), // Name field (same as RelativeUrl)
          EscapeCsvField(description),
          EscapeCsvField(entityName),
          EscapeCsvField(entityGuid),
          EscapeCsvField(regardingODataBind),
          EscapeCsvField(documentLibrary)
        );

        await File.AppendAllTextAsync(csvFilePath, csvLine + Environment.NewLine, Encoding.UTF8);
      }
      catch (Exception ex)
      {
        _logger.LogError(ex, "Error writing to CSV file: {CsvFilePath}", csvFilePath);
      }
    }

    /// <summary>
    /// Write CSV entry for entity type folder (Level 2 in nested hierarchy).
    /// This level has no regarding entity.
    /// </summary>
    private async Task WriteCsvForEntityTypeFolder(string csvFilePath, string entityTypeFolderName, string parentAccountFolderName)
    {
      try
      {
        // Escape fields that might contain commas or quotes
        var csvLine = string.Join(
          ",",
          EscapeCsvField(entityTypeFolderName),
          EscapeCsvField(entityTypeFolderName),
          EscapeCsvField("Documents on Default Site 1"), // Name field
          EscapeCsvField(""), // Description is blank
          EscapeCsvField(""), // No RegardingEntityType
          EscapeCsvField(""), // No RegardingEntityGuid
          EscapeCsvField(""), // No RegardingObjectODataBind
          EscapeCsvField(parentAccountFolderName) // Parent is the account folder
        );

        await File.AppendAllTextAsync(csvFilePath, csvLine + Environment.NewLine, Encoding.UTF8);
      }
      catch (Exception ex)
      {
        _logger.LogError(ex, "Error writing entity type folder to CSV file: {CsvFilePath}", csvFilePath);
      }
    }

    /// <summary>
    /// Escape CSV field by wrapping in quotes if it contains comma, quote, or newline
    /// </summary>
    private string EscapeCsvField(string field)
    {
      if (string.IsNullOrEmpty(field))
      {
        return "";
      }

      // If field contains comma, quote, or newline, wrap in quotes and escape internal quotes
      if (field.Contains(",") || field.Contains("\"") || field.Contains("\n") || field.Contains("\r"))
      {
        return "\"" + field.Replace("\"", "\"\"") + "\"";
      }

      return field;
    }
  }

  public enum SyncResult
  {
    Created,
    AlreadyExists,
    Error,
  }
}

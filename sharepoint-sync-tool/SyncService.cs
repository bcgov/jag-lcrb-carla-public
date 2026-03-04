using System;
using System.Collections.Generic;
using System.Linq;
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
      _logger.LogInformation("Starting SharePoint folder sync for entity: {EntityName}", config.EntityName);

      // Check if this is a nested entity type (Contravention or Enforcement Action)
      bool isNestedEntity = IsNestedEntityType(config.EntityName);
      List<FolderItem> foldersToSync;

      if (isNestedEntity)
      {
        // For nested entities, fetch folders from Account folders
        foldersToSync = await GetNestedFoldersAsync(config.EntityName, config.ModifiedAfterDateParsed);
      }
      else
      {
        // For regular entities, fetch folders directly from document library
        foldersToSync = await GetFoldersAsync(config.DocumentLibrary, config.ModifiedAfterDateParsed);
      }

      _logger.LogInformation("Found {FolderCount} folders to sync in SharePoint", foldersToSync.Count);

      if (foldersToSync.Count == 0)
      {
        _logger.LogInformation("No folders to sync");
        return;
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
            var result = await ProcessFolderAsync(folder, config);

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
    }

    private bool IsNestedEntityType(string entityName)
    {
      return entityName.ToLower() == "contravention" || entityName.ToLower() == "enforcement action";
    }

    private async Task<List<FolderItem>> GetNestedFoldersAsync(string entityName, DateTime? modifiedAfter)
    {
      try
      {
        var onPremManager = (OnPremSharePointFileManager)_sharePointManager;
        var nestedFolders = new List<FolderItem>();

        // Determine the nested folder name based on entity type
        string nestedFolderName =
          entityName.ToLower() == "contravention"
            ? SharePointConstants.ContraventionFolderInternalName
            : SharePointConstants.EnforcementActionFolderInternalName;

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

    private async Task<SyncResult> ProcessFolderAsync(FolderItem folder, SyncConfiguration config)
    {
      // Extract GUID from folder name
      var guid = ExtractGuidFromFolderName(folder.Name);

      if (guid == null)
      {
        _logger.LogWarning("Could not extract GUID from folder name: {FolderName}. Skipping.", folder.Name);
        return SyncResult.Error;
      }

      // Calculate the relative URL for the document location
      // For nested entities, this includes the parent folder path
      string relativeUrl = CalculateRelativeUrl(folder, config.EntityName, config.DocumentLibrary);

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

      // Create document location
      if (config.DryRun)
      {
        // In dry run mode, we just log what would be created, and return, without making any changes
        _logger.LogInformation(
          "[DRY RUN] Would create document location - FolderName: {FolderName}, RelativeUrl: {RelativeUrl}, RegardingEntity: {EntityName}, GUID: {Guid}",
          folder.Name,
          relativeUrl,
          config.EntityName,
          guid
        );
        return SyncResult.Created;
      }

      var created = await CreateDocumentLocationAsync(config.EntityName, config.DocumentLibrary, guid, relativeUrl);

      if (created)
      {
        _logger.LogInformation(
          "Created document location - FolderName: {FolderName}, RelativeUrl: {RelativeUrl}, RegardingEntity: {EntityName}, GUID: {Guid}",
          folder.Name,
          relativeUrl,
          config.EntityName,
          guid
        );
        return SyncResult.Created;
      }
      else
      {
        _logger.LogError("Failed to create document location for: {FolderName}", folder.Name);
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
      // For nested entities (contravention, enforcement action), the relative URL must include
      // the parent folder structure relative to the Account document library
      // Example: ACCOUNT_NAME_GUID/adoxio_contravention/CONTRAVENTION_NAME_GUID

      if (IsNestedEntityType(entityName))
      {
        // For nested entities, extract the path after the account document library
        // ServerRelativeUrl format: /account/ACCOUNT_NAME_GUID/adoxio_contravention/CONTRAVENTION_NAME_GUID
        var serverRelativeUrl = folder.ServerRelativeUrl;

        // Find the position after "/account/"
        var accountLibraryPath = "/" + SharePointConstants.AccountFolderInternalName + "/";
        var startIndex = serverRelativeUrl.IndexOf(accountLibraryPath);

        if (startIndex >= 0)
        {
          // Extract everything after "/account/" (e.g., "ACCOUNT_NAME_GUID/adoxio_contravention/CONTRAVENTION_NAME_GUID")
          var relativeUrl = serverRelativeUrl.Substring(startIndex + accountLibraryPath.Length);
          return relativeUrl;
        }

        // Fallback to folder name if we can't parse the URL
        _logger.LogWarning(
          "Could not parse ServerRelativeUrl for nested entity: {ServerRelativeUrl}. Using folder name as fallback.",
          serverRelativeUrl
        );
        return folder.Name;
      }

      // For regular entities, just use the folder name
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
        // Check if this is a duplicate key error
        var errorContent = ex.Response?.Content;
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
        var locations = _dynamicsClient.Sharepointdocumentlocations.Get(filter: $"relativeurl eq '{sanitized}'");

        var location = locations?.Value?.FirstOrDefault();

        if (location == null)
        {
          var newRecord = new MicrosoftDynamicsCRMsharepointdocumentlocation { Relativeurl = relativeUrl };

          location = _dynamicsClient.Sharepointdocumentlocations.Create(newRecord);
        }

        return location?.Sharepointdocumentlocationid;
      }
      catch (HttpOperationException ex)
      {
        _logger.LogError(ex, "Error getting document location reference for: {RelativeUrl}", relativeUrl);
        return null;
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
        "licence" => "adoxio_licenceses",
        "contravention" => "adoxio_contraventions",
        "enforcement action" => "adoxio_enforcementactions",
        _ => entityName + "s",
      };
    }
  }

  public enum SyncResult
  {
    Created,
    AlreadyExists,
    Error,
  }
}

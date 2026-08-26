extern alias DV;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Gov.Lclb.Cllb.Interfaces;
using Microsoft.Extensions.Logging;
using IDataverseClient = DV::Gov.Lclb.Cllb.Interfaces.IDataverseClient;
using SharePointDocumentLocation = DV::Gov.Lclb.Cllb.Interfaces.SharePointDocumentLocation;
using EntityReference = Microsoft.Xrm.Sdk.EntityReference;

namespace SharePointSyncTool
{
  public class SyncService
  {
    private readonly ISharePointFileManager _sharePointManager;
    private readonly IDataverseClient _dataverse;
    private readonly ILogger _logger;

    // Regex pattern to extract GUID from folder name
    // Format: SomeName_GUIDWITHOUTDASHES where GUID is 32 hex characters
    private static readonly Regex GuidPattern = new Regex(@"_([A-F0-9]{32})$", RegexOptions.IgnoreCase);

    public SyncService(ISharePointFileManager sharePointManager, IDataverseClient dataverse, ILoggerFactory loggerFactory)
    {
      _sharePointManager = sharePointManager;
      _dataverse = dataverse;
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

      if (config.DryRun)
      {
        _logger.LogWarning("DRY RUN MODE: No changes will be made to Dynamics");
      }

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
      _logger.LogInformation("  Document Locations {Action}: {TotalCreated}", config.DryRun ? "Simulated" : "Created", totalCreated);
      _logger.LogInformation("  Already Exists (Skipped): {TotalSkipped}", totalSkipped);
      _logger.LogInformation("  Errors: {TotalErrors}", totalErrors);

      if (totalCreated > 0)
      {
        _logger.LogInformation(
          "{DryRunPrefix}Successfully {Action} {TotalCreated} document location(s) for {UniqueEntities} entity/entities",
          config.DryRun ? "[DRY RUN] " : "",
          config.DryRun ? "simulated" : "created",
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
        var nestedFolders = new List<FolderItem>();

        // Determine the nested folder name based on entity type
        string nestedFolderName = GetNestedFolderInternalName(entityName);

        _logger.LogInformation(
          "Fetching {EntityName} folders nested under Account folders (looking for '{NestedFolderName}' subfolders)",
          entityName,
          nestedFolderName
        );

        // Get all Account folders
        var accountFolders = await _sharePointManager.GetFoldersInDocumentLibraryAfterDate(
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
            var childFolders = await _sharePointManager.GetChildFolders(accountFolder.ServerRelativeUrl);

            // Find the specific nested folder (adoxio_contravention or adoxio_enforcementaction)
            var nestedFolder = childFolders?.FirstOrDefault(f => f.Name.Equals(nestedFolderName, StringComparison.OrdinalIgnoreCase));

            if (nestedFolder != null)
            {
              accountsWithNestedFolders++;

              // Get the entity folders within the nested folder
              var entityFolders = await _sharePointManager.GetChildFolders(nestedFolder.ServerRelativeUrl);

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
        return await _sharePointManager.GetFoldersInDocumentLibraryAfterDate(documentLibrary, modifiedAfter.Value);
      }
      catch (Exception ex)
      {
        _logger.LogError(ex, "Error fetching folders from SharePoint");
        throw;
      }
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
        var locations = await _dataverse.GetSharePointDocLocsByRelativeUrlAsync(relativeUrl);

        if (locations == null || !locations.Any())
        {
          return null;
        }

        string? validLocationId = null;

        foreach (var location in locations)
        {
          // Check if this location has a regarding object (entity link)
          if (location.RegardingObjectId == null)
          {
            _logger.LogWarning(
              "Orphan document location found (no entity link): {LocationId} for relativeUrl {RelativeUrl}. Skipping to avoid duplicates.",
              location.Id.ToString(),
              relativeUrl
            );
            continue;
          }

          var locationEntityGuid = location.RegardingObjectId.Id.ToString().ToLower();
          var expectedGuid = entityGuid.ToLower();

          if (locationEntityGuid == expectedGuid)
          {
            _logger.LogDebug(
              "Valid document location found: {LocationId} for relativeUrl {RelativeUrl}, linked to entity {Guid}",
              location.Id.ToString(),
              relativeUrl,
              entityGuid
            );
            validLocationId = location.Id.ToString();
          }
          else
          {
            _logger.LogWarning(
              "Document location {LocationId} for relativeUrl {RelativeUrl} exists but is linked to different entity. Expected: {ExpectedGuid}, Found: {FoundGuid}. Skipping to avoid duplicates.",
              location.Id.ToString(),
              relativeUrl,
              expectedGuid,
              locationEntityGuid
            );
          }
        }

        if (validLocationId != null && locations.Count > 1)
        {
          _logger.LogWarning(
            "Multiple document locations found for relativeUrl {RelativeUrl}. This may indicate data inconsistency.",
            relativeUrl
          );
        }

        return validLocationId;
      }
      catch (Exception ex)
      {
        _logger.LogError(ex, "Error checking if document location exists for relativeUrl: {RelativeUrl}", relativeUrl);
        return null;
      }
    }

    private async Task<bool> CreateDocumentLocationAsync(string entityName, string documentLibrary, string entityGuid, string relativeUrl)
    {
      try
      {
        var parentDocumentLibraryId = await GetDocumentLocationReferenceByRelativeUrlAsync(documentLibrary);

        if (parentDocumentLibraryId == null)
        {
          _logger.LogError("Parent document library not found: {DocumentLibrary}", documentLibrary);
          return false;
        }

        var regardingRef = GetRegardingObjectReference(entityName, entityGuid);
        var documentLocation = new SharePointDocumentLocation
        {
          RelativeUrl = relativeUrl,
          Description = GetDescriptionForEntity(entityName),
          Name = relativeUrl,
          ParentSiteOrLocation = new EntityReference(SharePointDocumentLocation.EntityLogicalName, Guid.Parse(parentDocumentLibraryId)),
          RegardingObjectId = regardingRef
        };

        _logger.LogDebug(
          "Creating document location - Name: {Name}, RelativeUrl: {RelativeUrl}, Description: {Description}, Parent: {Parent}, RegardingEntity: {EntityName}, EntityGuid: {Guid}",
          documentLocation.Name,
          documentLocation.RelativeUrl,
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
          return true;
        }

        var createdId = await _dataverse.CreateSharePointDocLocAsync(documentLocation);

        if (createdId != Guid.Empty)
        {
          _logger.LogDebug(
            "Created document location {LocationId} - RelativeUrl: {RelativeUrl}, RegardingEntity: {EntityName}, GUID: {Guid}",
            createdId.ToString(),
            relativeUrl,
            entityName,
            entityGuid
          );
          return true;
        }

        return false;
      }
      catch (Exception ex)
      {
        if (ex.Message.Contains("duplicate") || ex.Message.Contains("already exists") || ex.Message.Contains("duplicate key"))
        {
          _logger.LogWarning(
            "Document location for {RelativeUrl} already exists (detected during creation). This may be a race condition.",
            relativeUrl
          );

          var verification = await GetExistingDocumentLocationAsync(relativeUrl, entityGuid);
          if (verification != null)
          {
            _logger.LogInformation("Verified existing document location: {LocationId}", verification);
            return true;
          }
        }

        _logger.LogError(ex, "Error creating document location for entity {EntityName}, GUID: {Guid}", entityName, entityGuid);
        return false;
      }
    }

    private async Task<string?> GetDocumentLocationReferenceByRelativeUrlAsync(string relativeUrl)
    {
      try
      {
        _logger.LogDebug("Looking up parent document library with relativeurl: {RelativeUrl}", relativeUrl);

        var locations = await _dataverse.GetSharePointDocLocsByRelativeUrlAsync(relativeUrl);
        var location = locations?.FirstOrDefault();

        if (location == null)
        {
          _logger.LogError(
            "Parent document library location not found with relativeurl '{RelativeUrl}'. "
              + "The parent document library must exist in Dataverse before syncing folders. "
              + "Please ensure SharePoint integration is properly configured and the document library location exists.",
            relativeUrl
          );
          return null;
        }

        _logger.LogDebug(
          "Found parent document library location: ID={LocationId}, Name={Name}",
          location.Id.ToString(),
          location.Name
        );

        return location.Id.ToString();
      }
      catch (Exception ex)
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

      var accountLibraryId = await GetDocumentLocationReferenceByRelativeUrlAsync(SharePointConstants.AccountFolderInternalName);
      if (accountLibraryId == null)
      {
        _logger.LogError("Account document library not found. Cannot create account document location.");
        return (null, false);
      }

      if (!Guid.TryParse(accountGuid, out var accountGuidParsed)) return (null, false);

      // Double-check before creating
      var finalCheck = await GetExistingDocumentLocationAsync(accountFolderName, accountGuid);
      if (finalCheck != null)
      {
        _logger.LogInformation("Account document location was created by another process. Using existing: {LocationId}", finalCheck);
        return (finalCheck, false);
      }

      try
      {
        var accountDocLocation = new SharePointDocumentLocation
        {
          RelativeUrl = accountFolderName,
          Description = "Account Files",
          Name = accountFolderName,
          ParentSiteOrLocation = new EntityReference(SharePointDocumentLocation.EntityLogicalName, Guid.Parse(accountLibraryId)),
          RegardingObjectId = new EntityReference("account", accountGuidParsed)
        };

        _logger.LogDebug(
          "Creating account document location - Name: {Name}, RelativeUrl: {RelativeUrl}, AccountGuid: {Guid}",
          accountDocLocation.Name,
          accountDocLocation.RelativeUrl,
          accountGuid
        );

        var createdId = await _dataverse.CreateSharePointDocLocAsync(accountDocLocation);
        if (createdId != Guid.Empty)
        {
          _logger.LogDebug(
            "Created account document location: {LocationId} for account {AccountGuid}",
            createdId.ToString(),
            accountGuid
          );
          return (createdId.ToString(), true);
        }
      }
      catch (Exception ex)
      {
        if (ex.Message.Contains("duplicate") || ex.Message.Contains("already exists") || ex.Message.Contains("duplicate key"))
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
      try
      {
        var locations = await _dataverse.GetSharePointDocLocsByRelativeUrlAsync(entityTypeFolderName);
        var match = locations?.FirstOrDefault(l =>
          l.ParentSiteOrLocation?.Id.ToString().ToLower() == parentAccountDocLocationId.ToLower()
        );

        if (match != null)
        {
          _logger.LogDebug(
            "Entity type folder document location already exists: {LocationId} for {EntityTypeFolder}",
            match.Id.ToString(),
            entityTypeFolderName
          );
          return (match.Id.ToString(), false);
        }
      }
      catch (Exception ex)
      {
        _logger.LogWarning(
          ex,
          "Error checking for existing entity type folder document location: {EntityTypeFolder}",
          entityTypeFolderName
        );
      }

      try
      {
        var entityTypeDocLocation = new SharePointDocumentLocation
        {
          RelativeUrl = entityTypeFolderName,
          Description = string.Empty,
          Name = "Documents on Default Site 1",
          ParentSiteOrLocation = new EntityReference(SharePointDocumentLocation.EntityLogicalName, Guid.Parse(parentAccountDocLocationId))
        };

        _logger.LogDebug(
          "Creating entity type folder document location - Name: {Name}, RelativeUrl: {RelativeUrl}, ParentId: {ParentId}",
          entityTypeDocLocation.Name,
          entityTypeDocLocation.RelativeUrl,
          parentAccountDocLocationId
        );

        var createdId = await _dataverse.CreateSharePointDocLocAsync(entityTypeDocLocation);
        if (createdId != Guid.Empty)
        {
          _logger.LogDebug(
            "Created entity type folder document location: {LocationId} for {EntityTypeFolder}",
            createdId.ToString(),
            entityTypeFolderName
          );
          return (createdId.ToString(), true);
        }
      }
      catch (Exception ex)
      {
        if (ex.Message.Contains("duplicate") || ex.Message.Contains("already exists") || ex.Message.Contains("duplicate key"))
        {
          _logger.LogWarning("Entity type folder document location already exists (detected during creation). Retrieving existing record.");
          var retryLocations = await _dataverse.GetSharePointDocLocsByRelativeUrlAsync(entityTypeFolderName);
          var retryMatch = retryLocations?.FirstOrDefault(l =>
            l.ParentSiteOrLocation?.Id.ToString().ToLower() == parentAccountDocLocationId.ToLower()
          );
          if (retryMatch != null)
          {
            return (retryMatch.Id.ToString(), false);
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

      // Double-check before creating
      var finalCheck = await GetExistingDocumentLocationAsync(entityFolderName, entityGuid);
      if (finalCheck != null)
      {
        _logger.LogInformation("Entity document location was created by another process. Skipping: {LocationId}", finalCheck);
        return true;
      }

      try
      {
        var entityDocLocation = new SharePointDocumentLocation
        {
          RelativeUrl = entityFolderName,
          Description = GetDescriptionForEntity(entityName),
          Name = entityFolderName,
          ParentSiteOrLocation = new EntityReference(SharePointDocumentLocation.EntityLogicalName, Guid.Parse(parentEntityTypeDocLocationId)),
          RegardingObjectId = GetRegardingObjectReference(entityName, entityGuid)
        };

        _logger.LogDebug(
          "Creating entity document location - Name: {Name}, RelativeUrl: {RelativeUrl}, EntityName: {EntityName}, EntityGuid: {Guid}, ParentId: {ParentId}",
          entityDocLocation.Name,
          entityDocLocation.RelativeUrl,
          entityName,
          entityGuid,
          parentEntityTypeDocLocationId
        );

        var createdId = await _dataverse.CreateSharePointDocLocAsync(entityDocLocation);
        if (createdId != Guid.Empty)
        {
          _logger.LogDebug(
            "Created entity document location: {LocationId} for entity {EntityGuid}",
            createdId.ToString(),
            entityGuid
          );
          return true;
        }

        return false;
      }
      catch (Exception ex)
      {
        if (ex.Message.Contains("duplicate") || ex.Message.Contains("already exists") || ex.Message.Contains("duplicate key"))
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

    private EntityReference? GetRegardingObjectReference(string entityName, string entityGuid)
    {
      if (!Guid.TryParse(entityGuid, out var guid)) return null;

      var logicalName = entityName.ToLower() switch
      {
        "account" => "account",
        "application" => "adoxio_application",
        "contact" => "contact",
        "worker" => "adoxio_worker",
        "event" => "adoxio_event",
        "licence" => "adoxio_licences",
        "contravention" => "adoxio_contravention",
        "enforcement action" => "adoxio_enforcementaction",
        "special event" => "adoxio_specialevent",
        "incident" => "incident",
        "complaint" => "adoxio_complaint",
        _ => null
      };

      if (logicalName == null)
      {
        _logger.LogWarning("Unknown entity type: {EntityName}. Document location may not be properly linked.", entityName);
        return null;
      }

      return new EntityReference(logicalName, guid);
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

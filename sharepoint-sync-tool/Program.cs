using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Gov.Lclb.Cllb.Interfaces;
using Gov.Lclb.Cllb.Interfaces.Models;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Rest;

namespace SharePointSyncTool
{
  class Program
  {
    static async Task<int> Main(string[] args)
    {
      // Setup configuration
      var configuration = new ConfigurationBuilder().AddEnvironmentVariables().Build();

      // Parse log level from configuration (default to Information)
      var logLevelString = configuration["LOG_LEVEL"] ?? "Information";
      var logLevel = Enum.TryParse<LogLevel>(logLevelString, ignoreCase: true, out var parsedLevel) ? parsedLevel : LogLevel.Information;

      // Setup logging
      var serviceProvider = new ServiceCollection()
        .AddLogging(builder =>
        {
          builder.AddSimpleConsole(options =>
          {
            options.SingleLine = true;
            options.TimestampFormat = "yyyy-MM-dd HH:mm:ss ";
          });
          builder.SetMinimumLevel(logLevel);
        })
        .BuildServiceProvider();

      var loggerFactory = serviceProvider.GetRequiredService<ILoggerFactory>();
      var logger = loggerFactory.CreateLogger<Program>();

      try
      {
        logger.LogInformation("Starting SharePoint to Dynamics Sync Tool");

        // Validate required environment variables
        var config = ValidateConfiguration(configuration, logger);
        if (config == null)
        {
          return 1;
        }

        // Initialize SharePoint and Dynamics clients
        var sharePointManager = SharePointFileManager.Create(configuration, loggerFactory);
        var dynamicsClient = DynamicsSetupUtil.SetupDynamics(configuration);

        // Run the sync
        var syncService = new SyncService(sharePointManager, dynamicsClient, loggerFactory);
        await syncService.SyncSharePointFoldersAsync(config);

        logger.LogInformation("Sync completed successfully");
        return 0;
      }
      catch (Exception ex)
      {
        logger.LogError(ex, "Fatal error during sync");
        return 1;
      }
    }

    private static SyncConfiguration? ValidateConfiguration(IConfiguration configuration, ILogger logger)
    {
      var config = new SyncConfiguration
      {
        EntityName = configuration["SYNC_ENTITY_NAME"] ?? "",
        ModifiedAfterDate = configuration["SYNC_MODIFIED_AFTER_DATE"],
        BatchSize = int.TryParse(configuration["SYNC_BATCH_SIZE"], out var batchSize) ? batchSize : 100,
        DryRun = bool.TryParse(configuration["SYNC_DRY_RUN"], out var dryRun) && dryRun,
        StartIndex = int.TryParse(configuration["SYNC_START_INDEX"], out var startIndex) ? startIndex : 0,
        EndIndex = int.TryParse(configuration["SYNC_END_INDEX"], out var endIndex) ? endIndex : 0,
      };

      var errors = new List<string>();

      if (string.IsNullOrWhiteSpace(config.EntityName))
      {
        errors.Add(
          "SYNC_ENTITY_NAME is required (e.g., 'account', 'contact', 'application', 'worker', 'event', 'licence', 'contravention', 'enforcement action', 'special event', 'incident', 'complaint')"
        );
      }

      if (!string.IsNullOrWhiteSpace(config.ModifiedAfterDate))
      {
        if (!DateTime.TryParse(config.ModifiedAfterDate, out var parsedDate))
        {
          errors.Add("SYNC_MODIFIED_AFTER_DATE must be a valid date (e.g., '2024-01-01')");
        }
        else
        {
          config.ModifiedAfterDateParsed = parsedDate;
        }
      }

      // Validate SharePoint configuration (On-Premise)
      if (string.IsNullOrEmpty(configuration["SHAREPOINT_ODATA_URI"]))
      {
        errors.Add("SHAREPOINT_ODATA_URI is required");
      }
      if (string.IsNullOrEmpty(configuration["SHAREPOINT_NATIVE_BASE_URI"]))
      {
        errors.Add("SHAREPOINT_NATIVE_BASE_URI is required");
      }
      if (string.IsNullOrEmpty(configuration["SHAREPOINT_USERNAME"]))
      {
        errors.Add("SHAREPOINT_USERNAME is required");
      }
      if (string.IsNullOrEmpty(configuration["SHAREPOINT_PASSWORD"]))
      {
        errors.Add("SHAREPOINT_PASSWORD is required");
      }
      if (string.IsNullOrEmpty(configuration["SHAREPOINT_STS_TOKEN_URI"]))
      {
        errors.Add("SHAREPOINT_STS_TOKEN_URI is required");
      }
      if (string.IsNullOrEmpty(configuration["SHAREPOINT_RELYING_PARTY_IDENTIFIER"]))
      {
        errors.Add("SHAREPOINT_RELYING_PARTY_IDENTIFIER is required");
      }

      // Validate Dynamics configuration (Cloud)
      if (string.IsNullOrEmpty(configuration["DYNAMICS_ODATA_URI"]))
      {
        errors.Add("DYNAMICS_ODATA_URI is required");
      }
      if (string.IsNullOrEmpty(configuration["DYNAMICS_AAD_TENANT_ID"]))
      {
        errors.Add("DYNAMICS_AAD_TENANT_ID is required");
      }
      if (string.IsNullOrEmpty(configuration["DYNAMICS_SERVER_APP_ID_URI"]))
      {
        errors.Add("DYNAMICS_SERVER_APP_ID_URI is required");
      }
      if (string.IsNullOrEmpty(configuration["DYNAMICS_APP_REG_CLIENT_ID"]))
      {
        errors.Add("DYNAMICS_APP_REG_CLIENT_ID is required");
      }
      if (string.IsNullOrEmpty(configuration["DYNAMICS_APP_REG_CLIENT_KEY"]))
      {
        errors.Add("DYNAMICS_APP_REG_CLIENT_KEY is required");
      }

      if (errors.Any())
      {
        logger.LogError("Configuration validation failed:");
        foreach (var error in errors)
        {
          logger.LogError("  - {Error}", error);
        }
        return null;
      }

      logger.LogInformation("Configuration validated successfully");
      logger.LogInformation("Entity Name: {EntityName}", config.EntityName);
      logger.LogInformation("Modified After: {ModifiedAfter}", config.ModifiedAfterDate ?? "Not specified");
      logger.LogInformation("Batch Size: {BatchSize}", config.BatchSize);
      logger.LogInformation("Dry Run: {DryRun}", config.DryRun);
      logger.LogInformation("Start Index: {StartIndex}", config.StartIndex);
      logger.LogInformation("End Index: {EndIndex}", config.EndIndex);

      return config;
    }
  }

  public class SyncConfiguration
  {
    public string EntityName { get; set; } = "";
    public string? ModifiedAfterDate { get; set; }
    public DateTime? ModifiedAfterDateParsed { get; set; }
    public int BatchSize { get; set; } = 100;
    public bool DryRun { get; set; } = false;
    public int StartIndex { get; set; } = 0;
    public int EndIndex { get; set; } = 0;
  }
}

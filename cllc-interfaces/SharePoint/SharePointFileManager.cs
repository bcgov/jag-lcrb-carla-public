using System;
using Gov.Lclb.Cllb.Interfaces;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace Gov.Lclb.Cllb.Interfaces;

/// <summary>
/// Factory for creating SharePoint file manager instances.
/// </summary>
/// <remarks>
/// Toggle between cloud and on-prem using the "SHAREPOINT_TYPE" configuration setting ("Cloud" or "OnPrem").
/// This allows the application to use the same ISharePointFileManager interface for both implementations without
/// modification. The factory will log which implementation is being used for easier debugging and verification.
/// </remarks>
public static class SharePointFileManager
{
    /// <summary>
    /// Creates a SharePoint file manager instance based on configuration.
    /// </summary>
    /// <param name="configuration">Application configuration</param>
    /// <param name="loggerFactory">Logger factory for logging</param>
    /// <returns>ISharePointFileManager implementation</returns>
    public static ISharePointFileManager Create(
        IConfiguration configuration,
        ILoggerFactory loggerFactory
    )
    {
        string sharePointType = configuration["SHAREPOINT_TYPE"]; // "Cloud" or "OnPrem"

        bool useCloud = sharePointType == "Cloud";

        var logger = loggerFactory.CreateLogger("SharePointFileManager");

        if (useCloud)
        {
            logger.LogInformation(
                "[SharePointFileManager] Create - initialized with Cloud (Graph API) implementation"
            );

            return new CloudSharePointFileManager(configuration, loggerFactory);
        }
        else
        {
            logger.LogInformation(
                "[SharePointFileManager] Create - initialized with OnPrem (REST API) implementation"
            );

            return new OnPremSharePointFileManager(configuration, loggerFactory);
        }
    }
}

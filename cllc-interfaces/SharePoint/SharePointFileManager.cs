using System;
using Gov.Lclb.Cllb.Interfaces;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace Gov.Lclb.Cllb.Interfaces;

/// <summary>
/// Factory for creating SharePoint file manager instances.
/// </summary>
/// <remarks>
/// Automatically selects Cloud (Graph API) implementation if all cloud configuration variables are present:
/// <list type="bullet">
/// <item>SHAREPOINT_ODATA_URI</item>
/// <item>SHAREPOINT_AAD_TENANTID</item>
/// <item>SHAREPOINT_CLIENT_ID</item>
/// <item>SHAREPOINT_CLIENT_SECRET</item>
/// </list>
/// Otherwise, uses on-premises (REST API) implementation.
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
        // Check if all cloud configuration variables are present
        string sharePointOdataUri = configuration["SHAREPOINT_ODATA_URI"];
        string sharePointAadTenantId = configuration["SHAREPOINT_AAD_TENANTID"];
        string sharePointClientId = configuration["SHAREPOINT_CLIENT_ID"];
        string sharePointClientSecret = configuration["SHAREPOINT_CLIENT_SECRET"];

        bool useCloud =
            !string.IsNullOrEmpty(sharePointOdataUri)
            && !string.IsNullOrEmpty(sharePointAadTenantId)
            && !string.IsNullOrEmpty(sharePointClientId)
            && !string.IsNullOrEmpty(sharePointClientSecret);

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

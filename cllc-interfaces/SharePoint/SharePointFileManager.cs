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
    /// <param name="loggerFactory">Logger factory (required for cloud implementation)</param>
    /// <returns>ISharePointFileManager implementation</returns>
    public static ISharePointFileManager Create(
        IConfiguration configuration,
        ILoggerFactory loggerFactory = null
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

        if (useCloud)
        {
            Console.WriteLine("SharePointFileManager - Cloud - 1");
            if (loggerFactory == null)
            {
                Console.WriteLine("SharePointFileManager - Cloud - 2");
                throw new ArgumentNullException(
                    nameof(loggerFactory),
                    "ILoggerFactory is required when using cloud SharePoint implementation."
                );
            }

            Console.WriteLine("SharePointFileManager - Cloud - 3");
            var logger = loggerFactory.CreateLogger("SharePointFileManager");
            logger.LogInformation(
                "SharePointFileManager - initialized with Cloud (Graph API) implementation"
            );

            return new CloudSharePointFileManager(configuration, loggerFactory);
        }
        else
        {
            Console.WriteLine("SharePointFileManager - OnPrem - 1");
            if (loggerFactory != null)
            {
                Console.WriteLine("SharePointFileManager - OnPrem - 2");
                var logger = loggerFactory.CreateLogger("SharePointFileManager");
                logger.LogInformation(
                    "SharePointFileManager - initialized with OnPrem (REST API) implementation"
                );
            }
            Console.WriteLine("SharePointFileManager - OnPrem - 3");

            return new OnPremSharePointFileManager(configuration);
        }
    }
}

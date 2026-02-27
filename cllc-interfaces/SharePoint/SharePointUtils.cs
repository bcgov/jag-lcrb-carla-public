using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Xml;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.Clients.ActiveDirectory;
using Microsoft.Rest;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace Gov.Lclb.Cllb.Interfaces;

public static class SharePointUtils
{
    /// <summary>
    /// Escape the apostrophe character.  Since we use it to enclose the filename it must be escaped.
    /// </summary>
    /// <param name="filename"></param>
    /// <returns>Filename, with apropstophes escaped.</returns>
    public static string EscapeApostrophe(string filename)
    {
        string result = null;
        if (!string.IsNullOrEmpty(filename))
        {
            result = filename.Replace("'", "''");
        }
        return result;
    }

    /// <summary>
    /// Fixes a folder name or folder path by removing invalid characters.
    /// If the input contains "/" path separators, each segment is fixed individually
    /// to preserve the path structure.
    /// </summary>
    /// <param name="folderNameOrPath">A single folder name or a path like "folder1/folder2"</param>
    /// <param name="entityName">Optional entity name (listTitle or urlTitle) to customize invalid character handling</param>
    /// <returns>The sanitized folder name or path</returns>
    public static string FixFoldername(string folderNameOrPath, string entityName = null)
    {
        if (string.IsNullOrEmpty(folderNameOrPath))
            return folderNameOrPath;

        // If it contains path separators, fix each segment individually
        if (folderNameOrPath.Contains("/"))
        {
            var segments = folderNameOrPath.Split('/');
            var fixedSegments = segments
                .Select(s => RemoveInvalidCharacters(s, entityName))
                .ToArray();
            return string.Join("/", fixedSegments);
        }

        // Single folder name
        return RemoveInvalidCharacters(folderNameOrPath, entityName);
    }

    /// <summary>
    /// Fix a filename by removing invalid characters and truncating if necessary
    /// </summary>
    /// <param name="filename">The filename to fix</param>
    /// <param name="maxLength">Maximum length for the filename</param>
    /// <param name="entityName">Optional entity name (listTitle or urlTitle) to customize invalid character handling</param>
    /// <returns>Fixed filename</returns>
    public static string FixFilename(string filename, int maxLength, string entityName = null)
    {
        string result = RemoveInvalidCharacters(filename, entityName);

        // SharePoint requires that the filename is less than 128 characters.

        if (result.Length >= maxLength)
        {
            string extension = Path.GetExtension(result);
            int extensionLength = extension.Length;

            // Calculate the length available for the filename without extension
            int nameLength = maxLength - extensionLength;

            // Ensure we don't try to create a substring with negative length
            if (nameLength <= 0)
            {
                // If there's no room for the filename, truncate to just the maxLength
                // This handles edge cases where the path is extremely long
                result = result.Substring(0, Math.Max(1, maxLength));
            }
            else
            {
                result = Path.GetFileNameWithoutExtension(result).Substring(0, nameLength);
                result += extension;
            }
        }

        return result;
    }

    /// <summary>
    /// Remove invalid characters from a filename, with optional entity-specific behavior
    /// </summary>
    /// <param name="filename">The filename to sanitize</param>
    /// <param name="entityName">Optional entity name to customize invalid character handling</param>
    /// <returns>Sanitized filename</returns>
    public static string RemoveInvalidCharacters(string filename, string entityName = null)
    {
        if (string.IsNullOrEmpty(filename))
        {
            return filename;
        }

        var original = filename;

        // Get OS invalid chars and add SharePoint-specific invalid characters
        var osInvalidChars = System.IO.Path.GetInvalidFileNameChars();

        // Get entity-specific invalid characters
        var additionalInvalidChars = GetInvalidCharactersForEntity(entityName);

        // Combine all invalid characters
        var allInvalidChars = new HashSet<char>(osInvalidChars.Concat(additionalInvalidChars));

        // Replace each invalid character based on entity-specific rules
        var result = new StringBuilder(filename.Length);
        foreach (char c in filename)
        {
            if (allInvalidChars.Contains(c))
            {
                result.Append('-'); // dash
            }
            else
            {
                result.Append(c);
            }
        }

        // Handle trailing dots (not allowed in SharePoint)
        var resultString = result.ToString().TrimEnd('.');

        // Log if any changes were made
        if (original != resultString)
        {
            Console.WriteLine(
                $"RemoveInvalidCharacters (entity: '{entityName ?? "default"}'): '{original}' -> '{resultString}'"
            );
        }

        return resultString;
    }

    public static char[] GetInvalidCharactersForEntity(string entityName)
    {
        var defaultInvalidChars = new char[]
        {
            '~',
            '#',
            '%',
            '*',
            '[',
            ']',
            '{',
            '}',
            ':',
            '<',
            '>',
            '?',
            '/',
            '\\',
            '|',
            '"',
        };

        if (string.IsNullOrEmpty(entityName))
        {
            return defaultInvalidChars;
        }

        // Entity-specific invalid character rules
        switch (entityName)
        {
            case SharePointConstants.EnforcementActionFolderDisplayName:
            case SharePointConstants.EnforcementActionFolderInternalName:
            case SharePointConstants.ContraventionFolderDisplayName:
            case SharePointConstants.ContraventionFolderInternalName:
                // Enforcement and contravention entities - include period to match legacy conventions
                return defaultInvalidChars.Append('.').ToArray();
            default:
                return defaultInvalidChars;
        }
    }
}

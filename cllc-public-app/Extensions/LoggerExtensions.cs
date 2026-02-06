using Microsoft.Extensions.Logging;
using Microsoft.Rest;
using Newtonsoft.Json;
using System;

namespace Gov.Lclb.Cllb.Public.Extensions
{
    /// <summary>
    /// Extension methods for ILogger that provide enhanced logging for HttpOperationException
    /// </summary>
    public static class LoggerExtensions
    {
        /// <summary>
        /// Logs an error with enhanced details for HttpOperationException
        /// </summary>
        /// <param name="logger">The logger instance</param>
        /// <param name="exception">The HttpOperationException to log</param>
        /// <param name="message">The error message</param>
        public static void LogError(this ILogger logger, HttpOperationException exception, string message)
        {
            logger.LogError((Exception)exception, message);
            logger.LogError($"Status Code: {exception.Response?.StatusCode}");
            logger.LogError($"Request: {JsonConvert.SerializeObject(exception.Request)}");
            logger.LogError($"Response: {JsonConvert.SerializeObject(exception.Response)}");

            if (!string.IsNullOrEmpty(exception.Response?.Content))
            {
                logger.LogError($"Response Content: {exception.Response.Content}");
            }
        }

        /// <summary>
        /// Logs debug information with enhanced details for HttpOperationException
        /// </summary>
        /// <param name="logger">The logger instance</param>
        /// <param name="exception">The HttpOperationException to log</param>
        /// <param name="message">The debug message</param>
        public static void LogDebug(this ILogger logger, HttpOperationException exception, string message)
        {
            logger.LogDebug((Exception)exception, message);
            logger.LogDebug($"Status Code: {exception.Response?.StatusCode}");
            logger.LogDebug($"Request: {JsonConvert.SerializeObject(exception.Request)}");
            logger.LogDebug($"Response: {JsonConvert.SerializeObject(exception.Response)}");

            if (!string.IsNullOrEmpty(exception.Response?.Content))
            {
                logger.LogDebug($"Response Content: {exception.Response.Content}");
            }
        }
    }
}

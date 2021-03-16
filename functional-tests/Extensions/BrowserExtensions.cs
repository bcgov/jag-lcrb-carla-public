using System.IO;
using System.Reflection;
using log4net;
using Microsoft.Dynamics365.UIAutomation.Api;
using OpenQA.Selenium;

namespace EasyReproTest.Extensions
{
    public static class BrowserExtensions
    {
        private static readonly ILog log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

        private static int screenshotsCount;

        public static void TakeScreenShot(this Browser browser)
        {
            var screenshotsDir = "screenshots";
            Directory.CreateDirectory(screenshotsDir);
            var path = $"{screenshotsDir}/screen{++screenshotsCount}.png";
            log.Info($"Screenshot: {path}");

            browser.TakeWindowScreenShot(path, ScreenshotImageFormat.Png);
        }

        public static void LogAndScreenShot(this Browser browser, string logMessage)
        {
            log.Info(logMessage);
            browser.TakeScreenShot();
        }
    }
}
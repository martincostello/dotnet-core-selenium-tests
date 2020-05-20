using System;
using System.Runtime.InteropServices;
using System.Security.Principal;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using OpenQA.Selenium.Edge;
using OpenQA.Selenium.Firefox;
using OpenQA.Selenium.IE;
using Xunit;

namespace seleniumtests
{
    public static class BingTests
    {
        [SkippableTheory]
        [InlineData("Chrome")]
        [InlineData("Edge")]
        [InlineData("Firefox")]
        [InlineData("InternetExplorer")] // Follow https://stackoverflow.com/a/36836137/1064169 for one-time setup
        public static void Search_For_DotNet_Core(string browserName)
        {
            // Arrange
            Skip.If(
                !RuntimeInformation.IsOSPlatform(OSPlatform.Windows) && (browserName == "Edge" || browserName == "InternetExplorer"),
                $"{browserName} is only supported on Windows.");

            Skip.If(
                RuntimeInformation.IsOSPlatform(OSPlatform.Windows) && browserName == "Edge" && Environment.GetEnvironmentVariable("GITHUB_ACTIONS") != null,
                "Edge cannot used in GitHub Actions");

            // Act
            using var driver = CreateWebDriver(browserName);
            driver.Navigate().GoToUrl("https://bing.com");

            // Assert
            driver.FindElement(By.Name("q")).SendKeys("dotnet core" + Keys.Enter);
        }

        private static IWebDriver CreateWebDriver(string browserName)
        {
            string driverDirectory = System.IO.Path.GetDirectoryName(typeof(BingTests).Assembly.Location) ?? ".";

            return browserName.ToLowerInvariant() switch
            {
                "chrome" => CreateChromeDriver(driverDirectory),
                "edge" => new EdgeDriver(driverDirectory),
                "firefox" => new FirefoxDriver(driverDirectory),
                "internetexplorer" => new InternetExplorerDriver(driverDirectory, new InternetExplorerOptions() { IgnoreZoomLevel = true }),
                _ => throw new NotSupportedException($"The browser '{browserName}' is not supported."),
            };
        }

        private static IWebDriver CreateChromeDriver(string driverDirectory)
        {
            var options = new ChromeOptions();

            if (!System.Diagnostics.Debugger.IsAttached)
            {
                options.AddArgument("--headless");
            }

            // HACK Workaround for "(unknown error: DevToolsActivePort file doesn't exist)"
            if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux))
            {
                options.AddArgument("--no-sandbox");
            }

            return new ChromeDriver(driverDirectory, options);
        }
    }
}

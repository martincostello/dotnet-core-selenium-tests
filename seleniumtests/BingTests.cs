using System;
using System.Runtime.InteropServices;
using Microsoft.Edge.SeleniumTools;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using OpenQA.Selenium.Firefox;
using Xunit;

namespace seleniumtests
{
    public static class BingTests
    {
        [SkippableTheory]
        [InlineData("Chrome")]
        [InlineData("Edge")]
        [InlineData("Firefox")]
        public static void Search_For_DotNet_Core(string browserName)
        {
            // Arrange
            Skip.If(
                RuntimeInformation.IsOSPlatform(OSPlatform.Linux) && browserName == "Edge",
                $"{browserName} is not supported on Linux.");

            Skip.If(
                RuntimeInformation.IsOSPlatform(OSPlatform.OSX) && browserName == "Edge" && Environment.GetEnvironmentVariable("GITHUB_ACTIONS") != null,
                $"{browserName} is not supported on macOS in GitHub Actions.");

            // Act
            using var driver = CreateWebDriver(browserName);
            driver.Navigate().GoToUrl("https://bing.com");

            // Assert
            driver.FindElement(By.Name("q")).SendKeys("dotnet core" + Keys.Enter);
        }

        private static IWebDriver CreateWebDriver(string browserName)
        {
            string driverDirectory = System.IO.Path.GetDirectoryName(typeof(BingTests).Assembly.Location) ?? ".";
            bool isDebuggerAttached = System.Diagnostics.Debugger.IsAttached;

            return browserName.ToLowerInvariant() switch
            {
                "chrome" => CreateChromeDriver(driverDirectory, isDebuggerAttached),
                "edge" => CreateEdgeDriver(driverDirectory, isDebuggerAttached),
                "firefox" => CreateFirefoxDriver(driverDirectory, isDebuggerAttached),
                _ => throw new NotSupportedException($"The browser '{browserName}' is not supported."),
            };
        }

        private static IWebDriver CreateChromeDriver(
            string driverDirectory,
            bool isDebuggerAttached)
        {
            var options = new ChromeOptions();

            if (!isDebuggerAttached)
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

        private static IWebDriver CreateEdgeDriver(
            string driverDirectory,
            bool isDebuggerAttached)
        {
            var options = new EdgeOptions()
            {
                UseChromium = true,
            };

            if (!isDebuggerAttached)
            {
                options.AddArgument("--headless");
            }

            return new EdgeDriver(driverDirectory, options);
        }

        private static IWebDriver CreateFirefoxDriver(
            string driverDirectory,
            bool isDebuggerAttached)
        {
            var options = new FirefoxOptions();

            if (!isDebuggerAttached)
            {
                options.AddArgument("--headless");
            }

            return new FirefoxDriver(driverDirectory, options);
        }
    }
}

using System;
using System.Runtime.InteropServices;
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
            Skip.IfNot(RuntimeInformation.IsOSPlatform(OSPlatform.Windows), $"{browserName} is only supported on Windows.");

            // Act
            using var driver = CreateWebDriver(browserName);
            driver.Navigate().GoToUrl("https://bing.com");

            // Assert
            driver.FindElement(By.Name("q")).SendKeys("dotnet core" + Keys.Enter);
        }

        private static IWebDriver CreateWebDriver(string browserName)
        {
            return browserName.ToLowerInvariant() switch
            {
                "chrome" => new ChromeDriver(),
                "edge" => new EdgeDriver(),
                "firefox" => new FirefoxDriver(),
                "internetexplorer" => new InternetExplorerDriver(new InternetExplorerOptions() { IgnoreZoomLevel = true }),
                _ => throw new NotSupportedException($"The browser '{browserName}' is not supported."),
            };
        }
    }
}

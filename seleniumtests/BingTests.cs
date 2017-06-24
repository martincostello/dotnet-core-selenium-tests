using System;
using OpenQA.Selenium;
using Xunit;

namespace seleniumtests
{
    public static class BingTests
    {
        [Theory]
        [InlineData("Chrome")]
        [InlineData("Edge")]
        [InlineData("Firefox")]
        [InlineData("InternetExplorer", Skip = "Leaves it open when finished. *bin emoji*")] // Follow https://stackoverflow.com/a/36836137/1064169 for one-time setup
        public static void Search_For_DotNet_Core(string browserName)
        {
            string url = "https://bing.com"; // Google doesn't seem to work properly in IE at the moment...

            using (var driver = CreateWebDriver(browserName))
            {
                driver.Navigate().GoToUrl(url);
                driver.FindElement(By.Name("q")).SendKeys("dotnet core" + Keys.Enter);
            }
        }

        private static IWebDriver CreateWebDriver(string browserName)
        {
            switch (browserName.ToLowerInvariant())
            {
                case "chrome":
                    return new OpenQA.Selenium.Chrome.ChromeDriver();

                case "edge":
                    return new OpenQA.Selenium.Edge.EdgeDriver();

                case "firefox":
                    return new OpenQA.Selenium.Firefox.FirefoxDriver();

                case "internetexplorer":
                    var options = new OpenQA.Selenium.IE.InternetExplorerOptions() { IgnoreZoomLevel = true };
                    return new OpenQA.Selenium.IE.InternetExplorerDriver(options);

                default:
                    throw new NotSupportedException($"The browser '{browserName}' is not supported.");
            }
        }
    }
}

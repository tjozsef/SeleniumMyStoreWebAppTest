
using Microsoft.VisualBasic;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using OpenQA.Selenium.Edge;
using OpenQA.Selenium.Firefox;
using SeleniumMyStoreWebAppFramework.Configuration;
using System.Configuration;
namespace SeleniumMyStoreWebAppFramework.Util;



public static class DriverFactory
{

    private static ThreadLocal<IWebDriver?> _driver = new();
    public static IWebDriver GetWebDriver()
    {
        if (!_driver.IsValueCreated || _driver.Value == null)
        {
            _driver.Value = CreateWebDriver();
        }
        return _driver.Value;
    }

    private static IWebDriver CreateWebDriver()
    {
        var browserType = GetBrowserType();
        IWebDriver? driver = null;
        switch (browserType)
        {
            case BrowserType.CHROME:
                var chromeOptions = new ChromeOptions
                {
                    PageLoadStrategy = PageLoadStrategy.Normal
                };
                driver = new ChromeDriver(chromeOptions);
                break;
            case BrowserType.FIREFOX:
                var firefoxOptions = new FirefoxOptions
                {
                    PageLoadStrategy = PageLoadStrategy.Normal
                };
                driver = new FirefoxDriver(firefoxOptions);
                break;
            case BrowserType.EDGE:
                var edgeOptions = new EdgeOptions
                {
                    PageLoadStrategy = PageLoadStrategy.Normal
                };
                driver = new EdgeDriver(edgeOptions);
                break;
        }
        if (driver == null)
        {
            throw new Exception($"Could not create webdriver for browsertype: {browserType.ToString()}");
        }
        driver.Manage().Window.Maximize();
        driver.Manage().Timeouts().ImplicitWait = TimeSpan.FromSeconds(DefaultConstans.ImplicitWaitSeconds);
        driver.Manage().Timeouts().PageLoad = TimeSpan.FromSeconds(10);
        return driver;
    }

    private static BrowserType GetBrowserType()
    {
        var browserName = TestContext.Parameters[DefaultConstans.BrowserKey]?.ToUpper() ?? ConfigurationManager.AppSettings[DefaultConstans.BrowserKey]?.ToUpper() ?? DefaultConstans.FallBackBrowser;
        var browserType = Enum.Parse<BrowserType>(browserName);
        return browserType;
    }

    public static void TearDownWebDriver()
    {
        if (_driver.IsValueCreated && _driver.Value != null)
        {
            _driver.Value.Quit();
            _driver.Value = null;
        }
    }
}


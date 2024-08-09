using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using OpenQA.Selenium.Interactions;

namespace OpenAIinNET.Selenium.Services
{
    public class SeleniumService : ISeleniumService
    {
        private IWebDriver _driver = null;

        public void Start() => _driver = new ChromeDriver();

        public void SetWindowsSize(int x, int y) => _driver.Manage().Window.Size = new System.Drawing.Size(x, y);

        public void GoToWebsite(string url) => _driver.Navigate().GoToUrl(url);

        public void ScrollY(int y) => new Actions(_driver).ScrollByAmount(0, y).Build().Perform();

        public byte[] DoScreenshot() => ((ITakesScreenshot)_driver).GetScreenshot().AsByteArray;

        public int GetElementHeightByXPath(string path) => _driver.FindElement(By.XPath(path)).Size.Height;

        public void ClickElementByXPath(string path) => _driver.FindElement(By.XPath(path)).Click();
    }
}

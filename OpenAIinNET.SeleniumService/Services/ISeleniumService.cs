namespace OpenAIinNET.Selenium.Services
{
    public interface ISeleniumService
    {
        void ClickElementByXPath(string path);
        byte[] DoScreenshot();
        int GetElementHeightByXPath(string path);
        void GoToWebsite(string url);
        void ScrollY(int y);
        void SetWindowsSize(int x, int y);
        void Start();
    }
}
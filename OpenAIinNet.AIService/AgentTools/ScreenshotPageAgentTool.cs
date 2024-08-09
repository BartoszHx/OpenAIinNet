using LangChain.Chains.StackableChains.Agents.Tools;
using OpenAIinNET.Selenium.Services;

namespace OpenAIinNET.AI.AgentTools
{
    public class ScreenshotPageAgentTool : AgentTool
    {
        private const string _description = @"
            This tool can do screenshots of all list of product on website and go to next pages and repeat this process. 
            Input is an integer value that specifies how many pages will be screenshotted.
            If process is done, Output will return 'Ok'
            ";

        private List<byte[]> _imageList;
        private ISeleniumService _service;

        public ScreenshotPageAgentTool(ISeleniumService service, List<byte[]> imageList) : base("screenshotpage", _description)
        {
            _service = service;
            _imageList = imageList;
        }

        public override async Task<string> ToolTask(string input, CancellationToken token = default)
        {
            var maxPage = int.Parse(input);

            var elementTopHeight = _service.GetElementHeightByXPath("//*[@id=\"__nuxt\"]/div/div[1]/header/div/div");

            var scrollDown = 1000 - elementTopHeight - 200;

            for (int i = 0; i < maxPage; i++)
            {
                await ScreenshotFullPage(scrollDown);
                await NextPage(i, maxPage);
            }

            return "Ok";
        }

        private async Task ScreenshotFullPage(int scrollDown)
        {
            var elementHeight = _service.GetElementHeightByXPath("//*[@id=\"__nuxt\"]/div/div[1]/div[1]");
            _service.ScrollY(elementHeight);
            await Task.Delay(100);

            var listElementHeight = _service.GetElementHeightByXPath("//*[@id=\"__nuxt\"]/div/div[1]/div[2]");
            var max = listElementHeight / scrollDown;

            for (int i = 0; i < max; i++)
            {
                var image = _service.DoScreenshot();
                _imageList.Add(image);

                _service.ScrollY(scrollDown);
                await Task.Delay(100);
            }
        }

        private async Task NextPage(int i, int maxPage)
        {
            if (i < maxPage - 1)
            {
                _service.ScrollY(-10000);
                await Task.Delay(100);
                _service.ClickElementByXPath("//a[text() = 'Następna']");
                await Task.Delay(5000);
            }
        }
    }
}

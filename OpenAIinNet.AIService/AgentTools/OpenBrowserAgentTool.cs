using LangChain.Chains.StackableChains.Agents.Tools;
using OpenAIinNET.Selenium.Services;

namespace OpenAIinNET.AI.AgentTools
{
    public class OpenBrowserAgentTool : AgentTool
    {
        private const string _description = @"Can open browser and go to the website from input. If browser is open it will return 'Ok' output.";

        private ISeleniumService _service;

        public OpenBrowserAgentTool(ISeleniumService service) : base("openbrowser", _description)
        {
            _service = service;
        }

        public override async Task<string> ToolTask(string input, CancellationToken token = default)
        {
            _service.Start();
            _service.SetWindowsSize(1335, 1000);
            _service.GoToWebsite(input);
            await Task.Delay(4000);

            return "Ok";
        }
    }
}

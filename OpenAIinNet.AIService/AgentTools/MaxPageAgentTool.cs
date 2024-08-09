using LangChain.Chains.StackableChains.Agents.Tools;
using OpenAI_API;
using OpenAI_API.Chat;
using OpenAIinNET.Selenium.Services;

namespace OpenAIinNET.AI.AgentTools
{
    public class MaxPageAgentTool : AgentTool
    {
        private const string _description = @"
            Can take max list of pages from website.
            Output is number of pages what website has.
            ";

        private string _openAIKey;
        private ISeleniumService _service;

        public MaxPageAgentTool(string openAIKey, ISeleniumService service) : base("maxpage", _description)
        {
            _openAIKey = openAIKey;
            _service = service;
        }

        public override async Task<string> ToolTask(string input, CancellationToken token = default)
        {
            var image = _service.DoScreenshot();

            var chatMessageImageList = new List<ChatMessage.ImageInput>() { new ChatMessage.ImageInput(image) };

            var questionSystem = "You only return answer as integer. Answer must be that short how is only possible.";

            var questionUser = "I need to know what amount of pages is on this website";

            var messageSystem = new ChatMessage
            {
                Role = ChatMessageRole.System,
                TextContent = questionSystem,
            };

            var messageUser = new ChatMessage
            {
                Role = ChatMessageRole.User,
                Images = chatMessageImageList,
                TextContent = questionUser,
            };

            var request = new ChatRequest()
            {
                Model = "gpt-4o",
                Temperature = 0.0,
                MaxTokens = 16,
                Messages = new List<ChatMessage>() { messageSystem, messageUser }
            };

            var api = new OpenAIAPI(_openAIKey);
            var result = await api.Chat.CreateChatCompletionAsync(request);

            return result.ToString();
        }
    }
}

using LangChain.Chains.StackableChains.Agents.Tools;
using OpenAI_API;
using OpenAI_API.Chat;

namespace OpenAIinNET.AI.AgentTools
{
    public class ExtractImageAgentTool : AgentTool
    {
        private const string _description = @"Take information about cards from screenshots.
            Tool will have information about screenshots doing before.
            Output is list of cards name with price.";

        private string _openAIKey;
        private List<byte[]> _imageList;

        public ExtractImageAgentTool(string openAIKey, List<byte[]> imageList) : base("extractimage", _description)
        {
            _openAIKey = openAIKey;
            _imageList = imageList;
        }

        public override async Task<string> ToolTask(string input, CancellationToken token = default)
        {
            var chatMessageImageList = _imageList.Select(s => new ChatMessage.ImageInput(s)).ToList();

            var questionSystem = "You are a helpful assistant.";


            var questionUser = @"Take cards name, amount, and price from the images. 
                Don't take cards with violet color sign with text 'FOIL'.
                Don't take cards where amount is 0. This information is located to the left of the price.
                Return list of cards. Card have name and price as string format";

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
                MaxTokens = 4000,
                Messages = new List<ChatMessage>() { messageSystem, messageUser }
            };

            var api = new OpenAIAPI(_openAIKey);
            var result = await api.Chat.CreateChatCompletionAsync(request);

            return result.ToString();
        }
    }
}

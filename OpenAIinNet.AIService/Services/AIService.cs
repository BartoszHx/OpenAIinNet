using LangChain.Providers;
using LangChain.Providers.OpenAI;
using Newtonsoft.Json;
using NJsonSchema;
using OpenAIinNET.AI.AgentTools;
using OpenAIinNET.AI.Models;
using OpenAIinNET.Selenium.Services;
using static LangChain.Chains.Chain;

namespace OpenAIinNET.AI.Services
{
    public class AIService
    {
        private string _openAIKey;
        private ISeleniumService _seleniumService;

        public AIService(string openAIKey)
        {
            _openAIKey = openAIKey;
            _seleniumService = new SeleniumService();
        }

        public async Task<List<CardModel>> CardSearch(List<string> cards)
        {
            var model = PrepareOpenAIModel();
            model.UseConsoleForDebug();

            string searchCards = string.Join(", ", cards);

            var jsonSchema = JsonSchema.FromType<List<CardModel>>().ToJson();
            var exampleList = new List<CardModel>() { new CardModel { Price = 10.02, Currency = "zl", Name = "Sol Ring" } };
            var jsonSchemaExample = JsonConvert.SerializeObject(exampleList);

            var prompt = @"
                Go to website https://mtgspot.pl/single/universes-beyond-assassins-creed/5655
                Do screenshots only for two page in this website.
                After that collect card information from screenshots and check if cards named: " + searchCards + @" are in this list.
                Return list of searching cards. If card is not found, don't return that.
                The output should be formatted as a JSON instance that conforms to the JSON schema below.

                As an example, for the schema " + jsonSchemaExample + @"

                Here is the output schema:
                ```
                " + jsonSchema + @"
                ```
            ";


            var imageList = new List<byte[]>();

            var websiteTool = new OpenBrowserAgentTool(_seleniumService);
            //var maxPageTool = new MaxPageAgentTool(_openAIKey, _seleniumService);
            var screenshotPageTool = new ScreenshotPageAgentTool(_seleniumService, imageList);
            var extractImageImageTool = new ExtractImageAgentTool(_openAIKey, imageList);

            var chain =
                Set(prompt)
                | ReActAgentExecutor(model, inputKey: "text", outputKey: "result")
                .UseTool(websiteTool)
                //.UseTool(maxPageTool)
                .UseTool(screenshotPageTool)
                .UseTool(extractImageImageTool);


            var result = await chain.RunAsync();

            var resultJson = result.Value["result"].ToString().Replace("```json", "").Replace("```", "");
            var cardsResult = JsonConvert.DeserializeObject<List<CardModel>>(resultJson);

            return cardsResult;
        }

        private OpenAiChatModel PrepareOpenAIModel()
        {
            return new OpenAiChatModel(new OpenAiProvider(_openAIKey), "gpt-4o")
            {
                Settings = new ChatSettings
                {
                    StopSequences = new[] { "Observation", "[END]" },
                }
            };
        }
    }
}

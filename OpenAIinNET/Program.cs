using OpenAIinNET.AI.Services;

//Setup
string openAIkey = "<API KEY>";
var aiService = new AIService(openAIkey);

//Run
var search = new List<string> { "Aya of Alexandria", "Arno Dorian", "Hookblade" };
var result = await aiService.CardSearch(search);
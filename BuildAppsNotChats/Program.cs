using BuildAppsNotChats;
using Microsoft.ML.OnnxRuntimeGenAI;
using OpenAI;
using OpenAI.Chat;
using PhiMiniLib;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Text.RegularExpressions;

Console.WriteLine("Would you like to test the Web API?");
Console.WriteLine("Type 'yes' to test the Web API or 'no' to test the Phi model.");
var testAPI = Console.ReadLine()!;

// since we are calling the Service and on the same server
// we might not have enough resources to run both the service and the model
if (testAPI == "yes")
{
    TestAPIServiceWithOpenAIstandard();
    return;
}

// Get the bin directory of the test project
string binDirectory = AppDomain.CurrentDomain.BaseDirectory;

// Traverse up to the solution directory (assuming the test project is one level deep in the solution)
string solutionDirectory = Directory.GetParent(binDirectory)!.Parent!.Parent!.Parent!.Parent!.FullName;

// Combine the solution directory with the "Models" folder
string modelPath = Path.Combine(solutionDirectory, "Models", "Phi-3.5-mini-Instruct-onnx-cuda-fp32");

Console.WriteLine("loading Phi model...");

Model model = new Model(modelPath);
Tokenizer tokenizer = new Tokenizer(model);
PhiProcessor phiProcessor = new PhiProcessor(model, tokenizer);
PhiHelper phiHelper = new PhiHelper(phiProcessor);

Console.WriteLine("Phi model loaded");

Start(EntityExtractionHud, GetJsonEntity, @"Prompt\EntityExtraction.txt");
Start(ContentFilterHud, GetJsonEntity, @"Prompt\ContentFilter.txt");
Start(GrammerSpellCheckHud, GetJsonEntity, @"Prompt\GrammerSpellCheck.txt");
Start(SingleWordSpellCheckHud, GetJsonEntity, @"Prompt\SingleWordSpelling.txt");
Readability(phiProcessor, phiHelper);

void Start(Action hud, Func<string, string> getJsonEntity, string promptPath)
{
    hud();

    while (true)
    {
        Console.Write("User: ");
        var userQuery = Console.ReadLine()!;
        if (userQuery == "/exit" || string.IsNullOrWhiteSpace(userQuery))
        {
            break;
        }

        // read the prompt file
        var entityExtractionPrompt = File.ReadAllText(promptPath);
        var prompt = entityExtractionPrompt.Replace("{USER_QUERY}", userQuery);
        var phiPrompt = phiHelper.CreateUserPromptEnding(prompt);
        var response = phiProcessor.GetResponse(phiPrompt);
        var jsonString = getJsonEntity(response);

        Console.WriteLine("Entities extracted:");
        Console.WriteLine(string.Concat("Phi: ", jsonString));
    }
}

string GetJsonEntity(string phiResponse)
{
    var pattern = @"\{(?:[^{}]|(?<open>\{)|(?<-open>\}))*\}(?(open)(?!))";
    var match = Regex.Match(phiResponse, pattern);

    if (match.Success)
    {
        return match.Value;
    }

    return "{}";
}

void EntityExtractionHud()
{
    Console.WriteLine("Entity Extraction Started");
    Console.WriteLine("Type '/exit' to quit or enter!");
    Console.WriteLine("Please enter a query to extract entities from.");
    Console.WriteLine("Example 1: Hello my name is Max and I would like to know what is the weather in Seattle?");
    Console.WriteLine("Example 2: Do you have a name?");
}

void ContentFilterHud()
{
    Console.WriteLine("Content Filter Started");
    Console.WriteLine("Type '/exit' to quit or enter!");
    Console.WriteLine("Please enter a query to filter content.");
    Console.WriteLine("Example 1: That movie was a shitty movie, can't beleive any fuckers would pay to see it!");
    Console.WriteLine("Example 2: Greate movie?");
}

void GrammerSpellCheckHud()
{
    Console.WriteLine("Grammer/Spellcheck Started");
    Console.WriteLine("Type '/exit' to quit or enter!");
    Console.WriteLine("Please enter a query for Grammer/Spellcehck.");
    Console.WriteLine("Example 1: The childrens were playing in the park when it started to rain cats and dogs.");
    Console.WriteLine("Example 2: I should of known better then to leave my homework at home.");
    Console.WriteLine("Example 3: Despite the whether, we decided to go ahead with are picnic plans.");
    Console.WriteLine("Example 4: The company's new policy effects all employees and there families.");
    Console.WriteLine("Example 5: She preformed admirably in the play, recieving a standing ovation.");
}

void SingleWordSpellCheckHud()
{
    Console.WriteLine("Single word spellcheck Started");
    Console.WriteLine("Type '/exit' to quit or enter!");
    Console.WriteLine("Please enter a query for Grammer/Spellcehck.");
    Console.WriteLine("occurnce");
    Console.WriteLine("defintely");
    Console.WriteLine("liason");
    Console.WriteLine("conscince");
    Console.WriteLine("pharoah");
    Console.WriteLine("rhythem");
    Console.WriteLine("mischievous");
    Console.WriteLine("cemetery");
}

void ReadabilityHud()
{
    Console.WriteLine("Readability Started");
    Console.WriteLine("Type '/exit' to quit or enter!");
    Console.WriteLine("Please enter a query for Readability.");
    Console.WriteLine("Example 1: The quick brown fox jumps over the lazy dog.");
    Console.WriteLine("Example 2: In the beginning God created the heavens and the earth.");
    Console.WriteLine("Example 3: It was the best of times, it was the worst of times, it was the age of wisdom, it was the age of foolishness.");
    Console.WriteLine("Example 4: To be, or not to be, that is the question.");
    Console.WriteLine("Example 5: It was a dark and stormy night; the rain fell in torrents, except at occasional intervals, when it was checked by a violent gust of wind which swept up the streets.");
}

void Readability(PhiProcessor phiProcessor, PhiHelper phiHelper)
{
    ReadabilityHud();
    Console.Write("User: ");
    string text = Console.ReadLine()!;
    var scorer = new LLMEnhancedReadabilityScorer(phiProcessor, phiHelper);
    var textStatistics = scorer.AnalyzeText(text);

    Console.WriteLine($"Basic word count: {textStatistics.WordCount}");
    Console.WriteLine($"LLM syllable count: {textStatistics.SyllableCount}");
    Console.WriteLine($"Content analysis: {textStatistics.ContentAnalysis}");
    Console.WriteLine("Improvement suggestions:");

    foreach (var suggestion in textStatistics.ImprovementSuggestions)
    {
        Console.WriteLine($"- {suggestion}");
    }

    Console.WriteLine($"Audience-specific analysis: {textStatistics.AudienceSpecificAnalysis}");
}

static void TestAPIServiceWithOpenAIstandard()
{
    var apiKey = "API_KEY";
    var endpoint = "http://localhost:5000/";

    OpenAIClient openAIClient = new(apiKey: apiKey, options: new OpenAIClientOptions() { Endpoint = new Uri(endpoint) });

    ChatClient chatClient = openAIClient.GetChatClient("Phi-3.5-mini-instruct");

    ChatCompletion completion = chatClient.CompleteChat(
        [
            // System messages represent instructions or other guidance about how the assistant should behave
            new SystemChatMessage("You are a helpful assistant that talks like a pirate."),
            // User messages represent user input, whether historical or the most recen tinput
            new UserChatMessage("Hi, can you help me?"),
            // Assistant messages in a request represent conversation history for responses
            new AssistantChatMessage("Arrr! Of course, me hearty! What can I do for ye?"),
            new UserChatMessage("What's the best way to train a parrot?"),
    ]);

    Console.WriteLine($"{completion.Role}: {completion.Content[0].Text}");
}
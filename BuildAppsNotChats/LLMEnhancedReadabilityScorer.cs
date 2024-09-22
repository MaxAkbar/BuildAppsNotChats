using PhiMiniLib;

namespace BuildAppsNotChats;

public class LLMEnhancedReadabilityScorer : ReadabilityScorer
{
    private readonly PhiProcessor _phiProcessor;
    private readonly PhiHelper _phiHelper;

    public LLMEnhancedReadabilityScorer(PhiProcessor phiProcessor, PhiHelper phiHelper)
    {
        _phiProcessor = phiProcessor;
        _phiHelper = phiHelper;
    }

    public override TextStatistics AnalyzeText(string text)
    {
        var basicStats = base.AnalyzeText(text);
        var prompt = LLMPrompts.GetSyllableCountPrompt(text);

        // Enhance with LLM capabilities
        (int syllableCount, bool result) = CountSyllablesWithLLM(text);

        basicStats.SyllableCount = 0;
        
        if (result)
        {
            basicStats.SyllableCount = syllableCount;
        }
        
        basicStats.ContentAnalysis = AnalyzeContentWithLLM(text);
        basicStats.ImprovementSuggestions = GenerateImprovementSuggestionsWithLLM(text, basicStats);

        return basicStats;
    }

    private (int, bool) CountSyllablesWithLLM(string text)
    {
        var syllablesPrompt = $@"Count the number of syllables in the following word: '{text}'
                                Please respond with only the number of syllables, without any explanation or additional text.";

        var syllableCount = _phiProcessor.GetResponse(_phiHelper.CreateUserPromptEnding(syllablesPrompt));
        var syllableIntCount = 0;
        var syllableResult = int.TryParse(syllableCount, out syllableIntCount);

        return (syllableIntCount, syllableResult);
    }

    private string AnalyzeContentWithLLM(string text)
    {
        return _phiProcessor.GetResponse(_phiHelper.CreateUserPromptEnding(text));
    }

    private string[] GenerateImprovementSuggestionsWithLLM(string text, TextStatistics stats)
    {

        string prompt = LLMPrompts.GenerateImprovementSuggestionsPrompt(text, stats);
        var improvementSuggestions = _phiProcessor.GetResponse(_phiHelper.CreateUserPromptEnding(prompt));

        return improvementSuggestions.Split('\n');  //
    }

    private string GetAudienceSpecificAnalysis(string text, string audience)
    {
        string audienceSpecificAnalysisPrompt = LLMPrompts.GetAudienceSpecificAnalysisPrompt(text, audience);

        return _phiProcessor.GetResponse(_phiHelper.CreateUserPromptEnding(audienceSpecificAnalysisPrompt));
    }
}

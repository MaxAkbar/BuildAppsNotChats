namespace PhiMiniLib
{
    public interface IPhiHelper
    {
        string CreateUserPromptEnding(string userPrompt);
        string CreateSystemPrompt(string systemPrompt);
        string CreateUserPrompt(string userPrompt);
        string CreateAssistantPrompt(string assistantPrompt);

        ModelResponse ConvertToOpenAIResponse(string phiResponse, int phiPromptTokenCount);
        (string, int) ConvertToPhiPrompt(ChatRequest request);
    }
}
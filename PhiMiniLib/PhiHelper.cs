using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Metadata;
using System.Text;
using System.Threading.Tasks;

namespace PhiMiniLib;

public class PhiHelper : IPhiHelper
{
    private readonly IPhiProcessor _phiProcessor;

    public PhiHelper(IPhiProcessor phiProcessor)
    {
        _phiProcessor = phiProcessor;
    }

    public string CreateSystemPrompt(string systemPrompt)
    {
        return $"<|system|>{Environment.NewLine}{systemPrompt}<|end|>";
    }
    public string CreateUserPrompt(string userPrompt)
    {
        return $"<|user|>{Environment.NewLine}{userPrompt}<|end|>";
    }

    public string CreateUserPromptEnding(string userPrompt)
    {
        return $"<|user|>{Environment.NewLine}{userPrompt}<|end|>{Environment.NewLine}<|assistant|>";
    }

    public string CreateAssistantPrompt(string assistantPrompt)
    {
        return $"<|assistant|>{Environment.NewLine}{assistantPrompt}<|end|>";
    }

    public (string, int) ConvertToPhiPrompt(ChatRequest request)
    {
        var tokenizer = _phiProcessor.GetTokenizer();
        var sb = new StringBuilder();
        var systemTemplate = "<|system|>{systemPrompt}<|end|>";
        var userTemplate = "<|user|>{userPrompt}<|end|>";
        var assistantTemplate = "<|assistant|>{assistantPrompt}<|end|>";

        foreach (var message in request.Messages)
        {
            if (message.Role == "system")
            {
                sb.Append(systemTemplate.Replace("{systemPrompt}", message.Content));
            }
            else if (message.Role == "user")
            {
                sb.Append(userTemplate.Replace("{userPrompt}", message.Content));
            }
            else if (message.Role == "assistant")
            {
                sb.Append(assistantTemplate.Replace("{assistantPrompt}", message.Content));
            }
            sb.Append("\n");
        }

        sb.Append("<assistant>");

        var sequences = tokenizer.Encode(sb.ToString());

        return (sb.ToString(), sequences[0].Length);
    }

    public ModelResponse ConvertToOpenAIResponse(string phiResponse, int phiPromptTokenCount)
    {
        var phiResponseCleaned = phiResponse.Trim();
        var sequences = _phiProcessor.GetTokenizer().Encode(phiResponseCleaned);
        var response = new ModelResponse
        {
            Id = $"chatcmpl-{Guid.NewGuid()}",
            Object = "chat.completion",
            Created = DateTimeOffset.UtcNow.ToUnixTimeSeconds(),
            Model = "Phi-3.5-mini-instruct",
            SystemFingerprint = string.Empty,
            Choices = new List<Choice>
                {
                    new Choice
                    {
                        Index = 0,
                        Message = new Message
                        {
                            Role = "assistant",
                            Content = phiResponseCleaned
                        },
                        Logprobs = null,
                        FinishReason = "stop"
                    }
                },
            Usage = new Usage
            {
                PromptTokens = phiPromptTokenCount,
                CompletionTokens = sequences[0].Length,
                TotalTokens = sequences[0].Length + phiPromptTokenCount
            }
        };

        return response;
    }

}

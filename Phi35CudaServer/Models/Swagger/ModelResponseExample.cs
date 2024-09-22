using PhiMiniLib;
using Swashbuckle.AspNetCore.Filters;

namespace Phi35CudaServer.Models.Swagger;

public class ModelResponseExample : IExamplesProvider<ModelResponse>
{
    public ModelResponse GetExamples()
    {
        return new ModelResponse
        {
            Id = "chatcmpl-123",
            Object = "chat.completion",
            Created = 1677652288,
            Model = "Phi-3.5-mini-instruct",
            SystemFingerprint = "fp_44709d6fcb",
            Choices = new List<Choice>
            {
                new Choice
                {
                    Index = 0,
                    Message = new Message
                    {
                        Role = "assistant",
                        Content = "I can do many things."
                    },
                    Logprobs = null,
                    FinishReason = "stop"
                }
            },
            Usage = new Usage
            {
                PromptTokens = 10,
                CompletionTokens = 10,
                TotalTokens = 20
            }
        };
    }
}


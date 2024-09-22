using PhiMiniLib;
using Swashbuckle.AspNetCore.Filters;
using System.Collections.Generic;

namespace Phi35CudaServer.Models.Swagger;

public class ChatRequestExample : IExamplesProvider<ChatRequest>
{
    public ChatRequest GetExamples()
    {
        return new ChatRequest
        {
            Model = "Phi-3.5-mini-instruct",
            Messages = new List<Message>
           {
               new Message
               {
                   Role = "user",
                   Content = "Hello, how can you assist me today? Be short and concise."
               }
           },
            MaxTokens = 100,
            Temperature = 0.7
        };
    }
}

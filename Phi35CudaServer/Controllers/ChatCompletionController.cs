using Microsoft.AspNetCore.Mvc;
using Phi35CudaServer.Models.Swagger;
using PhiMiniLib;
using Swashbuckle.AspNetCore.Annotations;
using Swashbuckle.AspNetCore.Filters;
using System.Text;

namespace Phi35CudaServer.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class ChatCompletionController : ControllerBase
    {
        private readonly IPhiProcessor _phiProcessor;
        private readonly IPhiHelper _phiHelper;

        public ChatCompletionController(IPhiProcessor phiProcessor, IPhiHelper phiHelper)
        {
            _phiProcessor = phiProcessor;
            _phiHelper = phiHelper;
        }


        [Route("/chat/completions")]
        [Route("/openai/deployments/chat/completions")]
        [HttpPost]
        [SwaggerOperation(Summary = "Generates text based on the provided chat request")]
        [SwaggerRequestExample(typeof(ChatRequest), typeof(ChatRequestExample))]
        [SwaggerResponse(200, "The model's response", typeof(ModelResponse))]
        public IActionResult GenerateText([FromBody] ChatRequest request)
        {
            var (phiPrompt, phiPromptTokenCount) = _phiHelper.ConvertToPhiPrompt(request);
            var phiResponse = _phiProcessor.GetResponse(phiPrompt);

            return Ok(_phiHelper.ConvertToOpenAIResponse(phiResponse, phiPromptTokenCount));
        }
    }
}

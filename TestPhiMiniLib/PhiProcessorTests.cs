using Microsoft.ML.OnnxRuntimeGenAI;

namespace PhiMiniLib.Tests
{
    [TestClass]
    public class PhiProcessorTests
    {
        private static PhiProcessor? _phiProcessor;
        private static Model? _model;
        private static Tokenizer? _tokenizer;

        [ClassInitialize]
        public static void ClassSetup(TestContext context)
        {
            // Get the bin directory of the test project
            string binDirectory = AppDomain.CurrentDomain.BaseDirectory;

            // Traverse up to the solution directory (assuming the test project is one level deep in the solution)
            string solutionDirectory = Directory.GetParent(binDirectory)!.Parent!.Parent!.Parent!.Parent!.FullName;

            // Combine the solution directory with the "Models" folder
            string modelPath = Path.Combine(solutionDirectory, "Models", "Phi-3.5-mini-Instruct-onnx-cuda-fp32");

            // Initialize the model, tokenizer, and phiProcessor
            _model = new Model(modelPath);
            _tokenizer = new Tokenizer(_model);
            _phiProcessor = new PhiProcessor(_model, _tokenizer);
        }

        [TestMethod]
        public void GetResponse_ShouldReturnResponse()
        {
            // Arrange
            string prompt = "<|system|>\r\nYou are a helpful assistant.<|end|>\r\n<|user|>\r\nJust say hello. Do not add anythig else<|end|>\r\n<|assistant|>";
            int maxLength = 4096;
            string expectedResponse = "Hello!";

            // Act
            string response = _phiProcessor!.GetResponse(prompt, maxLength);

            // Assert
            Assert.AreEqual(expectedResponse, response.Trim());
        }

        [TestMethod]
        public void GetResponseStreamed_ShouldReturnResponseStream()
        {
            // Arrange
            string prompt = "<|system|>\r\nYou are a helpful assistant.<|end|>\r\n<|user|>\r\nJust say hello. Do not add anythig else<|end|>\r\n<|assistant|>";
            int maxLength = 4096;
            string expectedResponse = "Hello!";

            // Act
            IEnumerable<string> responseStream = _phiProcessor!.GetResponseStreamed(prompt, maxLength);

            // make the enumerable into a string
            string response = string.Join("", responseStream);

            // Assert
            Assert.AreEqual(expectedResponse, response.Trim());
        }
    }
}

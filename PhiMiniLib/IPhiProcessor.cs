
using Microsoft.ML.OnnxRuntimeGenAI;

namespace PhiMiniLib
{
    public interface IPhiProcessor
    {
        string GetResponse(string prompt, int maxLength = 4096);
        IEnumerable<string> GetResponseStreamed(string prompt, int maxLength = 4096);
        Tokenizer GetTokenizer();
    }
}
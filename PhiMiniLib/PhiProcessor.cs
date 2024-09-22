using System.Reflection.Emit;
using System.Reflection;
using System.Text;
using Microsoft.ML.OnnxRuntimeGenAI;

namespace PhiMiniLib;

public class PhiProcessor : IPhiProcessor
{
    private readonly Model _model;
    private readonly Tokenizer _tokenizer;
    private readonly GeneratorParams _generatorParams;

    public PhiProcessor(Model model, Tokenizer tokenizer)
    {
        _model = model;
        _tokenizer = tokenizer;
        _generatorParams = new(model);
    }

    /// <summary>
    /// Get the response from the model based on the input prompt.
    /// </summary>
    /// <param name="prompt"></param>
    /// <returns></returns>
    public string GetResponse(string prompt, int maxLength = 4096)
    {
        _generatorParams.SetSearchOption("max_length", maxLength);
        _generatorParams.SetInputSequences(_tokenizer.Encode(prompt));

        var responseStreamed = new StringBuilder();

        foreach (var response in GetResponseStreamed(prompt))
        {
            responseStreamed.Append(response);
        }

        return responseStreamed.ToString();
    }

    /// <summary>
    /// Get the response from the model based on the input prompt streamed.
    /// </summary>
    /// <param name="prompt"></param>
    /// <returns></returns>
    public IEnumerable<string> GetResponseStreamed(string prompt, int maxLength = 4096)
    {

        _generatorParams.SetSearchOption("max_length", maxLength);
        _generatorParams.SetInputSequences(_tokenizer.Encode(prompt));

        using var tokenizerStream = _tokenizer.CreateStream();
        using var generator = new Generator(_model, _generatorParams);

        while (!generator.IsDone())
        {
            generator.ComputeLogits();
            generator.GenerateNextToken();

            yield return tokenizerStream.Decode(generator.GetSequence(0)[^1]);
        }
    }

    /// <summary>
    /// Get the Tokenizer.
    /// </summary>
    /// <returns></returns>
    public Tokenizer GetTokenizer()
    {
        return _tokenizer;
    }
}

using System.Collections;
using System.Collections.Generic;
using System.Text;
using Newtonsoft.Json;
using Unity.InferenceEngine;
using UnityEngine;

public class FCKGPT : MonoBehaviour
{
    // Referenzed Model Files
    public ModelAsset modelAsset;
    public TextAsset vocabAsset;
    public TextAsset mergesAsset;
    private string outputString = "One day an alien came down from Mars. It saw a chicken";

    // Used on Model Runtime
    Tensor<float>[] pastKeys = new Tensor<float>[numLayers];
    Tensor<float>[] pastValues = new Tensor<float>[numLayers];
    bool runInference;
    Worker engine;
    const int maxTokens = 100;
    int currentToken;
    int[] outputTokens = new int[maxTokens];

    // For Tokkenizing and Decoding
    const int END_OF_TEXT = 50256;
    string[] tokens;
    int[] whiteSpaceCharacters = new int[256];
    int[] encodedCharacters = new int[256];
    string[] merges;
    Dictionary<string, int> vocab;
    int totalTokens;

    // Model Constants
    const int numLayers = 6;
    const int batchSize = 1;
    const int numHeads = 12;
    const int headDim = 64;

    //
    // Runtime Block
    //

    void Start()
    {
        // 
        // Prep Work
        //
        SetupWhiteSpaceShifts();
        LoadVocabulary();
        DecodePrompt(outputString);

        for (int i = 0; i < numLayers; i++)
        {
            pastKeys[i] = new Tensor<float>(new TensorShape(batchSize, numHeads, 1, headDim));
            pastValues[i] = new Tensor<float>(new TensorShape(batchSize, numHeads, 1, headDim));
        }

        //
        // Create and run the Runtime Model 
        //

        Model model1 = ModelLoader.Load(modelAsset);
        FunctionalGraph graph = new FunctionalGraph();

        // Input Block
        List<FunctionalTensor> inputs = new List<FunctionalTensor>
        {
            graph.AddInput<int>(new TensorShape(1, 1), "input_ids")
        };
        for (int i = 0; i < 6; i++)
        {
            inputs.Add(graph.AddInput<float>(new DynamicTensorShape(1, 12, -1, 64), $"past_key_values.{i}.key"));
            inputs.Add(graph.AddInput<float>(new DynamicTensorShape(1, 12, -1, 64), $"past_key_values.{i}.value"));
        }
        inputs.Add(graph.AddInput<int>(new TensorShape(1, 1), "position_ids"));
        inputs.Add(graph.AddInput<int>(new DynamicTensorShape(1, -1), "attention_mask"));

        // Outputs Block
        FunctionalTensor[] outputs = Functional.Forward(model1, inputs.ToArray());
        foreach (var output in outputs)
        {
            graph.AddOutput(output);
        }

        // Compile and run model
        var runTimeModel = graph.Compile();
        engine = new Worker(runTimeModel, BackendType.GPUCompute);
        for (int i = 0; i < currentToken; i++)
        {
            RunSingleTokenInference(outputTokens[i]);
        }
        StartCoroutine(InferenceLoop());
    }

    IEnumerator InferenceLoop()
    {
        runInference = true;
        while (runInference)
        {
            RunInference();
            yield return null;
        }
    }

    void OnDestroy()
    {
        engine?.Dispose();
    }

    void RunSingleTokenInference(int token)
    {
        var inputIds = new Tensor<int>(new TensorShape(1, 1), new int[] { outputTokens[currentToken] });
        var attentionMask = new Tensor<int>(new TensorShape(1, 1), new int[] { 1 });
        var positionIds = new Tensor<int>(new TensorShape(1, 1), new int[] { totalTokens });

        engine.SetInput("input_ids", inputIds);
        for (int i = 0; i < numLayers; i++)
        {
            engine.SetInput("past_key_values." + i + ".key", pastKeys[i]);
            engine.SetInput("past_key_values." + i + ".value", pastValues[i]);
        }
        engine.SetInput("attention_mask", attentionMask);
        engine.SetInput("position_ids", positionIds);

        engine.Schedule();

        var logits = (engine.PeekOutput() as Tensor<float>).ReadbackAndClone();
        //int nextToken = SampleFromLogits(logits);
        totalTokens++;

        for (int i = 0; i < numLayers; i++)
        {
            int keyIndex = 1 + i * 2;
            int valueIndex = 1 + i * 2 + 1;
            pastKeys[i] = (engine.PeekOutput(keyIndex) as Tensor<float>).ReadbackAndClone();
            pastValues[i] = (engine.PeekOutput(valueIndex) as Tensor<float>).ReadbackAndClone();
        }

        

    }

    void RunInference()
    {
        var inputIds = new Tensor<int>(new TensorShape(1, 1),  new int[] { outputTokens[currentToken] });
        var attentionMask = new Tensor<int>(new TensorShape(1, 1), new int[] { 1 });
        var positionIds = new Tensor<int>(new TensorShape(1, 1), new int[] { totalTokens });

        // Past key/values
        engine.SetInput("input_ids", inputIds);
        for (int i = 0; i < numLayers; i++)
        {
            engine.SetInput("past_key_values."+i+".key", pastKeys[i]);
            engine.SetInput("past_key_values."+i+".value", pastValues[i]);
        }
        engine.SetInput("attention_mask", attentionMask);
        engine.SetInput("position_ids", positionIds);

        engine.Schedule();

        // Get logits → sample next token
        var logits = (engine.PeekOutput() as Tensor<float>).ReadbackAndClone();
        int nextToken = SampleFromLogits(logits);
        totalTokens++;

        // Update past keys/values (peek from outputs, store for next step)
        for (int i = 0; i < numLayers; i++)
        {
            int keyIndex = 1 + i * 2;
            int valueIndex = 1 + i * 2 + 1;
            pastKeys[i] = (engine.PeekOutput(keyIndex) as Tensor<float>).ReadbackAndClone();
            pastValues[i] = (engine.PeekOutput(valueIndex) as Tensor<float>).ReadbackAndClone();
        }

        // Store new token TODO TEST & CLEAN
        if (currentToken >= maxTokens - 1)
        {
            for (int i = 0; i < maxTokens - 1; i++) outputTokens[i] = outputTokens[i + 1];
            currentToken--;
        }

        outputTokens[++currentToken] = nextToken;
        



        // Stop condition
        if (nextToken == END_OF_TEXT || totalTokens >= maxTokens)
        {
            runInference = false;
        }
        else if (nextToken < 0 || nextToken >= tokens.Length)
        {
            outputString += " ";
        }
        else
        {
            outputString += GetUnicodeText(tokens[nextToken]);
        }

        Debug.Log(outputString);
    }

    int SampleFromLogits(Tensor<float> logits)
    {
        // Basic greedy sampling (can be improved with temperature/top-k)
        float maxLogit = float.MinValue;
        int maxIndex = -1;
        for (int i = 0; i < logits.shape.length; i++)
        {
            if (logits[i] > maxLogit)
            {
                maxLogit = logits[i];
                maxIndex = i;
            }
        }
        return maxIndex;
    }

    //
    // Decoding and Token Block
    //

    void DecodePrompt(string text)
    {
        var inputTokens = GetTokens(text);

        for (int i = 0; i < inputTokens.Count && i < maxTokens; i++)
        {
            outputTokens[i] = inputTokens[i];
        }
        currentToken = inputTokens.Count - 1;
    }

    void LoadVocabulary()
    {
        var jsonText = vocabAsset.text;
        vocab = JsonConvert.DeserializeObject<Dictionary<string, int>>(jsonText);
        tokens = new string[vocab.Count];
        foreach (var item in vocab)
        {
            tokens[item.Value] = item.Key;
        }

        merges = mergesAsset.text.Split("\r\n");
    }

    string GetUnicodeText(string text)
    {
        var bytes = Encoding.GetEncoding("ISO-8859-1").GetBytes(ShiftCharacterDown(text));
        return Encoding.UTF8.GetString(bytes);
    }

    string GetASCIIText(string newText)
    {
        var bytes = Encoding.UTF8.GetBytes(newText);
        return ShiftCharacterUp(Encoding.GetEncoding("ISO-8859-1").GetString(bytes));
    }

    string ShiftCharacterDown(string text)
    {
        string outText = "";
        foreach (char letter in text)
        {
            outText += (letter <= 256) ? letter : (char)whiteSpaceCharacters[letter - 256];
        }
        return outText;
    }

    string ShiftCharacterUp(string text)
    {
        string outText = "";
        foreach (char letter in text)
        {
            outText += (char)encodedCharacters[letter];
        }
        return outText;
    }

    void SetupWhiteSpaceShifts()
    {
        for (int i = 0, n = 0; i < 256; i++)
        {
            encodedCharacters[i] = i;
            if (IsWhiteSpace(i))
            {
                encodedCharacters[i] = n + 256;
                whiteSpaceCharacters[n++] = i;
            }
        }
    }

    bool IsWhiteSpace(int i)
    {
        //returns true if it is a whitespace character
        return i <= 32 || (i >= 127 && i <= 160) || i == 173;
    }

    List<int> GetTokens(string text)
    {
        text = GetASCIIText(text);

        // Start with a list of single characters
        var inputTokens = new List<string>();
        foreach (var letter in text)
        {
            inputTokens.Add(letter.ToString());
        }

        ApplyMerges(inputTokens);

        //Find the ids of the words in the vocab
        var ids = new List<int>();
        foreach (var token in inputTokens)
        {
            if (vocab.TryGetValue(token, out int id))
            {
                ids.Add(id);
            }
        }

        return ids;
    }

    void ApplyMerges(List<string> inputTokens)
    {
        foreach (var merge in merges)
        {
            string[] pair = merge.Split(' ');
            int n = 0;
            while (n >= 0)
            {
                n = inputTokens.IndexOf(pair[0], n);
                if (n != -1 && n < inputTokens.Count - 1 && inputTokens[n + 1] == pair[1])
                {
                    inputTokens[n] += inputTokens[n + 1];
                    inputTokens.RemoveAt(n + 1);
                }
                if (n != -1) n++;
            }
        }
    }

}
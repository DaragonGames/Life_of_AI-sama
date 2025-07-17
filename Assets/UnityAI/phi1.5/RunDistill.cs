using System;
using System.Collections.Generic;
using System.Text;
using Newtonsoft.Json;
using Unity.InferenceEngine;
using UnityEngine;

public class RunDistill : MonoBehaviour
{
    public ModelAsset modelAsset;
    public TextAsset vocabAsset;
    public TextAsset mergesAsset;

    const BackendType backend = BackendType.GPUCompute;

    //string outputString = "Once upon a time, there were three bears";
    string outputString = "One day an alien came down from Mars. It saw a chicken";

    // This is how many tokens you want. It can be adjusted.
    const int maxTokens = 100;

    //Make this smaller for more randomness
    const float predictability = 5f;

    //Special tokens
    const int END_OF_TEXT = 50256;

    //Store the vocabulary
    string[] tokens;

    Worker engine;

    int currentToken;
    int[] outputTokens = new int[maxTokens];

    // Used for special character decoding
    int[] whiteSpaceCharacters = new int[256];
    int[] encodedCharacters = new int[256];

    bool runInference;

    //stop after this many tokens
    const int stopAfter = 100;

    int totalTokens;

    string[] merges;
    Dictionary<string, int> vocab;

    const int numLayers = 6;
    const int batchSize = 1;
    const int numHeads = 12;
    const int headDim = 64;
    const int seqLen = maxTokens; // Adjust as needed

    Tensor<float>[] pastKeys = new Tensor<float>[numLayers];
    Tensor<float>[] pastValues = new Tensor<float>[numLayers];

    void Start()
    {
        SetupWhiteSpaceShifts();

        LoadVocabulary();

        var model1 = ModelLoader.Load(modelAsset);
        //Create a new model to select the random token:

        var graph = new FunctionalGraph();
        List<FunctionalTensor> inputs = new List<FunctionalTensor>
        {
            graph.AddInput<int>(new TensorShape(1, 1), "input_ids")
        };
        for (int i = 0; i < 6; i++)
        {
            inputs.Add(graph.AddInput<float>(new TensorShape(1, 12, 0, 64), $"past_key_values.{i}.key"));
            inputs.Add(graph.AddInput<float>(new TensorShape(1, 12, 0, 64), $"past_key_values.{i}.value"));
        }
        inputs.Add(graph.AddInput<int>(new TensorShape(1, 1), "position_ids"));
        inputs.Add(graph.AddInput<int>(new TensorShape(1, 1), "attention_mask"));

        // Forward pass with all inputs
        var outputs = Functional.Forward(model1, inputs.ToArray());

        // Extract logits from the last output (adjust if different)
        graph.AddOutput(outputs[^1]); 

int pastKeyValuesStartIndex = outputs.Length - 1 - numLayers * 2; // index of first past_key_value tensor

for (int i = 0; i < numLayers; i++)
{
    graph.AddOutput(outputs[pastKeyValuesStartIndex + i * 2]);     // past_key_values[i].key
    graph.AddOutput(outputs[pastKeyValuesStartIndex + i * 2 + 1]); // past_key_values[i].value
}

        // Compile the full model
        var model2 = graph.Compile();

        for (int i = 0; i < numLayers; i++)
        {
            // Shape: [batch, num_heads, past_seq_len, head_dim]
            pastKeys[i] = new Tensor<float>(new TensorShape(batchSize, numHeads, 0, headDim));
            pastValues[i] = new Tensor<float>(new TensorShape(batchSize, numHeads, 0, headDim));
        }

        engine = new Worker(model2, backend);

        DecodePrompt(outputString);

        runInference = true;
    }

    // Update is called once per frame
    void Update()
    {
        if (runInference)
        {
            RunInference();
        }
    }

    void RunInference()
    {
        // 1. Current token input
        var inputIds = new Tensor<int>(new TensorShape(1, 1), new int[] { outputTokens[currentToken] });

        // 2. Attention mask: all 1s so far
        var attentionMask = new Tensor<int>(new TensorShape(1, 1), new int[] { 1 });
        for (int i = 0; i <= currentToken; i++) attentionMask[0, i] = 1;

        // 3. Position ids (0-based)
        var positionIds = new Tensor<int>(new TensorShape(1, 1), new int[] { currentToken });

        // 4. Past key/values
        engine.SetInput("input_ids", inputIds);
        for (int i = 0; i < numLayers; i++)
        {
            engine.SetInput("past_key_values."+i+".key", pastKeys[i]);
            engine.SetInput("past_key_values."+i+".value", pastValues[i]);
        }
        engine.SetInput("attention_mask", attentionMask);
        engine.SetInput("position_ids", positionIds);

        engine.Schedule();

        // 5. Get logits → sample next token
        using var logits = (engine.PeekOutput() as Tensor<float>).ReadbackAndClone();
        int nextToken = SampleFromLogits(logits);

        // Store new token
        if (currentToken >= maxTokens - 1)
        {
            for (int i = 0; i < maxTokens - 1; i++) outputTokens[i] = outputTokens[i + 1];
            currentToken--;
        }

        outputTokens[++currentToken] = nextToken;
        totalTokens++;

        // 6. Update past keys/values (peek from outputs, store for next step)
        for (int i = 0; i < numLayers; i++)
        {
            pastKeys[i] = engine.PeekOutput($"past_key_values.{i}.key") as Tensor<float>;
            pastValues[i] = engine.PeekOutput($"past_key_values.{i}.value") as Tensor<float>;
        }

        // 7. Stop condition
        if (nextToken == END_OF_TEXT || totalTokens >= stopAfter)
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

    void DecodePrompt(string text)
    {
        var inputTokens = GetTokens(text);

        for (int i = 0; i < inputTokens.Count; i++)
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

    // Translates encoded special characters to Unicode
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

    void OnDestroy()
    {
        engine?.Dispose();
    }
}

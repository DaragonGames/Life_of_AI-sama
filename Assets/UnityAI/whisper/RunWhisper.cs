using System.Collections.Generic;
using UnityEngine;
using Unity.InferenceEngine;
using System.Text;
using Unity.Collections;
using Newtonsoft.Json;

public class RunWhisper : MonoBehaviour
{
    private bool outputWords = true;
    string outputString = "";

    Worker decoder1, decoder2, encoder, spectrogram;
    Worker argmax;

    // This is how many tokens you want. It can be adjusted.
    const int maxTokens = 100;

    // Special tokens see added tokens file for details
    const int END_OF_TEXT = 50257;
    const int START_OF_TRANSCRIPT = 50258;
    const int ENGLISH = 50259;
    const int TRANSCRIBE = 50359; //for speech-to-text in specified language
    const int NO_TIME_STAMPS = 50363;

    int numSamples;
    string[] tokens;

    int tokenCount = 0;
    NativeArray<int> outputTokens;

    // Used for special character decoding
    int[] whiteSpaceCharacters = new int[256];

    Tensor<float> encodedAudio;

    bool transcribe = false;

    // Maximum size of audioClip (30s at 16kHz)
    const int maxSamples = 30 * 16000;

    public ModelAsset audioDecoder1, audioDecoder2;
    public ModelAsset audioEncoder;
    public ModelAsset logMelSpectro;

    public void Start()
    {
        SetupWhiteSpaceShifts();
        GetTokens();

        decoder1 = new Worker(ModelLoader.Load(audioDecoder1), BackendType.GPUCompute);
        decoder2 = new Worker(ModelLoader.Load(audioDecoder2), BackendType.GPUCompute);

        FunctionalGraph graph = new FunctionalGraph();
        var input = graph.AddInput(DataType.Float, new DynamicTensorShape(1, 1, 51865));
        var amax = Functional.ArgMax(input, -1, false);
        var selectTokenModel = graph.Compile(amax);
        argmax = new Worker(selectTokenModel, BackendType.GPUCompute);

        encoder = new Worker(ModelLoader.Load(audioEncoder), BackendType.GPUCompute);
        spectrogram = new Worker(ModelLoader.Load(logMelSpectro), BackendType.GPUCompute);
    }

    public async void Transcribe(AudioClip source)
    {
        outputTokens = new NativeArray<int>(maxTokens, Allocator.Persistent);

        outputTokens[0] = START_OF_TRANSCRIPT;
        outputTokens[1] = ENGLISH;
        outputTokens[2] = TRANSCRIBE;
        tokenCount = 3;

        LoadAudio(source);
        EncodeAudio();
        transcribe = true;

        tokensTensor = new Tensor<int>(new TensorShape(1, maxTokens));
        ComputeTensorData.Pin(tokensTensor);
        tokensTensor.Reshape(new TensorShape(1, tokenCount));
        tokensTensor.dataOnBackend.Upload<int>(outputTokens, tokenCount);

        lastToken = new NativeArray<int>(1, Allocator.Persistent); lastToken[0] = NO_TIME_STAMPS;
        lastTokenTensor = new Tensor<int>(new TensorShape(1, 1), new[] { NO_TIME_STAMPS });

        while (true)
        {
            if (!transcribe || tokenCount >= (outputTokens.Length - 1))
                break;
            m_Awaitable = InferenceStep();
            await m_Awaitable;
        }
        if (!outputWords)
        {
            AIManager.TranscriptionResult(outputString);
        }
        outputString = "";

        // Prevent Memory Lackage
        audioInput.Dispose();
        lastTokenTensor.Dispose();
        tokensTensor.Dispose();
    }
    Awaitable m_Awaitable;

    NativeArray<int> lastToken;
    Tensor<int> lastTokenTensor;
    Tensor<int> tokensTensor;
    Tensor<float> audioInput;

    void LoadAudio(AudioClip audioClip)
    {
        numSamples = audioClip.samples;
        var data = new float[maxSamples];
        numSamples = maxSamples;
        audioClip.GetData(data, 0);
        audioInput = new Tensor<float>(new TensorShape(1, numSamples), data);
    }

    void EncodeAudio()
    {
        spectrogram.Schedule(audioInput);
        var logmel = spectrogram.PeekOutput() as Tensor<float>;
        encoder.Schedule(logmel);
        encodedAudio = encoder.PeekOutput() as Tensor<float>;
    }
    async Awaitable InferenceStep()
    {
        decoder1.SetInput("input_ids", tokensTensor);
        decoder1.SetInput("encoder_hidden_states", encodedAudio);
        decoder1.Schedule();


        decoder2.SetInput("input_ids", lastTokenTensor);
        for (int i = 0; i < 12; i++)
        {
            var past_key_values_decoder_key = decoder1.PeekOutput("present." + i + ".decoder.key") as Tensor<float>;
            var past_key_values_decoder_value = decoder1.PeekOutput("present." + i + ".decoder.value") as Tensor<float>;
            var past_key_values_encoder_key = decoder1.PeekOutput("present." + i + ".encoder.key") as Tensor<float>;
            var past_key_values_encoder_value = decoder1.PeekOutput("present." + i + ".encoder.value") as Tensor<float>;

            decoder2.SetInput("past_key_values." + i + ".decoder.key", past_key_values_decoder_key);
            decoder2.SetInput("past_key_values." + i + ".decoder.value", past_key_values_decoder_value);
            decoder2.SetInput("past_key_values." + i + ".encoder.key", past_key_values_encoder_key);
            decoder2.SetInput("past_key_values." + i + ".encoder.value", past_key_values_encoder_value);
        }

        decoder2.Schedule();

        var logits = decoder2.PeekOutput("logits") as Tensor<float>;
        argmax.Schedule(logits);
        using var t_Token = await argmax.PeekOutput().ReadbackAndCloneAsync() as Tensor<int>;
        int index = t_Token[0];

        outputTokens[tokenCount] = lastToken[0];
        lastToken[0] = index;
        tokenCount++;
        tokensTensor.Reshape(new TensorShape(1, tokenCount));
        tokensTensor.dataOnBackend.Upload<int>(outputTokens, tokenCount);
        lastTokenTensor.dataOnBackend.Upload<int>(lastToken, 1);

        if (index == END_OF_TEXT)
        {
            transcribe = false;
        }
        else if (index < tokens.Length)
        {
            string word = GetUnicodeText(tokens[index]);
            if (outputWords)
            {
                AIManager.TranscriptionResult(word);
            }
            else
            {                
                outputString += word;
            }
        }
    }

    // Tokenizer
    public TextAsset vocabAsset;
    void GetTokens()
    {
        var vocab = JsonConvert.DeserializeObject<Dictionary<string, int>>(vocabAsset.text);
        tokens = new string[vocab.Count];
        foreach (var item in vocab)
        {
            tokens[item.Value] = item.Key;
        }
    }

    string GetUnicodeText(string text)
    {
        var bytes = Encoding.GetEncoding("ISO-8859-1").GetBytes(ShiftCharacterDown(text));
        return Encoding.UTF8.GetString(bytes);
    }

    string ShiftCharacterDown(string text)
    {
        string outText = "";
        foreach (char letter in text)
        {
            outText += ((int)letter <= 256) ? letter : (char)whiteSpaceCharacters[(int)(letter - 256)];
        }
        return outText;
    }

    void SetupWhiteSpaceShifts()
    {
        for (int i = 0, n = 0; i < 256; i++)
        {
            if (IsWhiteSpace((char)i)) whiteSpaceCharacters[n++] = i;
        }
    }

    bool IsWhiteSpace(char c)
    {
        return !(('!' <= c && c <= '~') || ('�' <= c && c <= '�') || ('�' <= c && c <= '�'));
    }

    private void OnDestroy()
    {
        decoder1.Dispose();
        decoder2.Dispose();
        encoder.Dispose();
        spectrogram.Dispose();
        argmax.Dispose();
    }
}

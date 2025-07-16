using UnityEngine;

public class AudioRecorder : MonoBehaviour
{
    private AudioClip recordedClip;
    private bool isRecording;
    private float recordingTime;
    private string selectedDevice;
    private const int sampleRate = 16000;
    private const int maxLength = 29;
    private RunWhisper whisperAPI;

    void Start()
    {
        selectedDevice = Microphone.devices[0];
        whisperAPI = GetComponent<RunWhisper>();
    }

    void Update()
    {
        if (isRecording)
        {
            recordingTime += Time.deltaTime;
            if (recordingTime > maxLength)
            {
                ToggleRecording();
            }
        }
        else
        {
            recordingTime = 0;
        }
    }

    public void ToggleRecording()
    {
        if (isRecording)
        {
            Microphone.End(null);
            whisperAPI.audioClip = ConvertToMono(recordedClip);
            //whisperAPI.DoIt();
        }
        else
        {
            recordedClip = Microphone.Start(selectedDevice, false, maxLength, sampleRate);
        }
        isRecording = !isRecording;
    }

    public static AudioClip ConvertToMono(AudioClip clip)
    {
        int samples = clip.samples;
        int channels = clip.channels;
        int frequency = clip.frequency;

        float[] stereoData = new float[samples * channels];
        clip.GetData(stereoData, 0);

        float[] monoData = new float[samples];

        for (int i = 0; i < samples; i++)
        {
            float sum = 0f;
            for (int ch = 0; ch < channels; ch++)
            {
                sum += stereoData[i * channels + ch];
            }
            monoData[i] = sum / channels;
        }

        AudioClip monoClip = AudioClip.Create(clip.name + "_mono", samples, 1, frequency, false);
        monoClip.SetData(monoData, 0);
        return monoClip;
    }

}

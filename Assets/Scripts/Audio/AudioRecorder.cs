using UnityEngine;
//using UnityEngine.Networking;
using System.Collections;
using UnityEngine.XR;
//using System.Linq;
using UnityEngine.InputSystem.XR;
using UnityEngine.XR.Interaction.Toolkit;

public class AudioRecorder : MonoBehaviour
{
    public XRController leftController;
    public AudioSource audioFeedback;
    public WhisperAPI whisperAPI;  // Reference to your WhisperAPI component
    
    private AudioClip recording;
    private bool isRecording = false;

    private float lastCommandTime;
    public float commandCooldown = 1.5f;
    
    void Update()
    {
        if (leftController.inputDevice.TryGetFeatureValue(CommonUsages.primaryButton, out bool pressed) && pressed)
        {
            if (!isRecording) StartRecording();
        }
        else if (isRecording)
        {
            StopRecording();
        }
    }

    void StartRecording()
    {
        recording = Microphone.Start(null, false, 10, 44100);
        isRecording = true;
        audioFeedback.Play();  // Audio feedback
    }

    void StopRecording()
    {
        Microphone.End(null);
        isRecording = false;
        StartCoroutine(ProcessVoiceCommand());
    }

    IEnumerator ProcessVoiceCommand()
    {
        if (Time.time - lastCommandTime < commandCooldown) yield break;

        byte[] audioData = WavConverter.ConvertToWav(recording);
        string transcript = null;

        // Use the WhisperAPI component directly
        yield return StartCoroutine(whisperAPI.TranscribeAudio(audioData, (result) =>
        {
            transcript = result;
        }));

        if (!string.IsNullOrEmpty(transcript))
        {
            Debug.Log($"Transcribed: {transcript}");
            CommandParser.ExecuteCommand(transcript);
        }

        lastCommandTime = Time.time;
    }
}
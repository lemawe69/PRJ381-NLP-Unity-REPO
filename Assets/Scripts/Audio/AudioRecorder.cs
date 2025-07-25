using UnityEngine;
using System.Collections;
using UnityEngine.XR;
using System.Collections.Generic;

public class AudioRecorder : MonoBehaviour
{
    public AudioSource audioFeedback;
    public WhisperAPI whisperAPI;
    
    private AudioClip recording;
    private bool isRecording = false;
    private InputDevice leftController;
    private bool buttonPressedLastFrame = false;

    void Start()
    {
        InitializeController();
    }

    void Update()
    {
        // Try to initialize if not valid
        if (!leftController.isValid)
        {
            InitializeController();
            if (!leftController.isValid) return;
        }

        // Check for button press
        if (leftController.TryGetFeatureValue(CommonUsages.primaryButton, out bool pressed) && pressed)
        {
            if (!buttonPressedLastFrame && !isRecording)
            {
                StartRecording();
            }
            buttonPressedLastFrame = true;
        }
        else if (buttonPressedLastFrame)
        {
            buttonPressedLastFrame = false;
            if (isRecording)
            {
                StopRecording();
            }
        }
    }

    void InitializeController()
    {
        var devices = new List<InputDevice>();
        InputDevices.GetDevicesAtXRNode(XRNode.LeftHand, devices);
        
        if (devices.Count > 0)
        {
            leftController = devices[0];
            Debug.Log($"Found controller: {leftController.name}");
        }
        else
        {
            Debug.LogWarning("Left controller not found!");
        }
    }

    void StartRecording()
    {
        Debug.Log("Starting recording...");
        recording = Microphone.Start(null, false, 10, 44100);
        isRecording = true;
        if (audioFeedback != null) audioFeedback.Play();
    }

    void StopRecording()
    {
        Debug.Log("Stopping recording...");
        Microphone.End(null);
        isRecording = false;
        StartCoroutine(ProcessVoiceCommand());
    }

    IEnumerator ProcessVoiceCommand()
    {
        if (recording == null)
        {
            Debug.LogWarning("No recording to process");
            yield break;
        }

        byte[] audioData = WavConverter.ConvertToWav(recording);
        
        if (audioData == null || audioData.Length == 0)
        {
            Debug.LogWarning("No audio data recorded");
            yield break;
        }

        string transcript = null;
        yield return StartCoroutine(whisperAPI.TranscribeAudio(audioData, (result) => {
            transcript = result;
        }));

        if (!string.IsNullOrEmpty(transcript))
        {
            Debug.Log($"Transcribed: {transcript}");
            CommandParser.ExecuteCommand(transcript);
        }
    }
}
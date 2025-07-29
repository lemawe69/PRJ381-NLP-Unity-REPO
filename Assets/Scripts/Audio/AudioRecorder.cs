using UnityEngine;
using System.Collections;
using UnityEngine.XR;
using System.Collections.Generic;
using UnityEngine.InputSystem;

public class AudioRecorder : MonoBehaviour
{
    public AudioSource audioFeedback;
    public WhisperAPI whisperAPI;

    private AudioClip recording;
    private bool isRecording = false;
    private UnityEngine.XR.InputDevice leftController;
    private bool buttonPressedLastFrame = false;

    private Keyboard keyboard;

    private float lastCommandTime;
    private const float MIN_COMMAND_INTERVAL = 3f;

    //byte[] audioData = WavConverter.ConvertToWav(recording);

    void Start()
    {
        InitializeController();
        keyboard = Keyboard.current;
    }

    void Update()
    {
        // Try to initialize if not valid
        // if (!leftController.isValid)
        // {
        //     InitializeController();
        //     if (!leftController.isValid) return;
        // }

        // Check for button press
        if (leftController.TryGetFeatureValue(UnityEngine.XR.CommonUsages.primaryButton, out bool pressed) && pressed)
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

        // test without vr headset code
        #if UNITY_EDITOR
        if (keyboard != null)
        {
            if (keyboard.spaceKey.wasPressedThisFrame && !isRecording)
            {
                StartRecording();
            }

            if (keyboard.spaceKey.wasReleasedThisFrame && isRecording)
            {
                StopRecording();
            }
        }
    #endif
    }

    void InitializeController()
    {
        var devices = new List<UnityEngine.XR.InputDevice>();
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

        // Get all available microphones
        string[] devices = Microphone.devices;
        Debug.Log("Available microphones:");
        foreach (string device in devices)
        {
            Debug.Log(device);
        }
    
        // Use the first headset microphone (if available)
        string selectedDevice = null;
        foreach (string device in devices)
        {
            if (device.ToLower().Contains("headset") || 
                device.ToLower().Contains("earbud") ||
                device.ToLower().Contains("airpod"))
            {
                selectedDevice = device;
                break;
            }
        }
    
        // Start recording
        Debug.Log("Starting recording...");
        //recording = Microphone.Start(null, false, 5, 16000);
        recording = Microphone.Start(selectedDevice, false, 5, 16000);
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
        // Prevent too frequent commands
        if (Time.time - lastCommandTime < MIN_COMMAND_INTERVAL)
        {
            Debug.Log("Command throttled");
            yield break;
        }

        if (recording == null)
        {
            Debug.LogWarning("No recording to process");
            yield break;
        }

        byte[] audioData = WavConverter.ConvertToWav(recording);
        audioData = WavConverter.TrimSilence(audioData);

        if (audioData == null || audioData.Length == 0)
        {
            Debug.LogWarning("No audio data recorded");
            yield break;
        }

        string quickCommand = DetectSimpleCommand(audioData);
        if (!string.IsNullOrEmpty(quickCommand))
        {
            Debug.Log($"Offline command: {quickCommand}");
            CommandParser.ExecuteCommand(quickCommand);
            yield break;
        }

        string transcript = null;
        yield return StartCoroutine(whisperAPI.TranscribeAudio(audioData, (result) =>
        {
            transcript = result;
        }));

        if (!string.IsNullOrEmpty(transcript))
        {
            Debug.Log($"Transcribed: {transcript}");
            CommandParser.ExecuteCommand(transcript);
        }

        if (!string.IsNullOrEmpty(transcript))
        {
            Debug.Log($"Transcribed: {transcript}");
            CommandParser.ExecuteCommand(transcript);
        }
        else
        {
            Debug.LogWarning("Transcription failed or returned empty");
        }

        if (!string.IsNullOrEmpty(transcript))
        {
            lastCommandTime = Time.time;
            CommandParser.ExecuteCommand(transcript);
        }
    }

    private string DetectSimpleCommand(byte[] audioData)
    {
        try
        {
            // Simple energy-based detection
            float energy = CalculateAudioEnergy(audioData);
            if (energy < 0.1f) return null; // Too quiet

            // Try recognizing short commands
            string[] commands = { "take off", "land", "stop", "up", "down", "left", "right" };
            foreach (string cmd in commands)
            {
                if (audioData.Length < 5000 &&
                    System.Text.Encoding.UTF8.GetString(audioData).Contains(cmd.Substring(0, 3)))
                {
                    return cmd;
                }
            }
        }
        catch { }
        return null;
    }
    
    private float CalculateAudioEnergy(byte[] audio)
    {
        float sum = 0;
        for (int i = 0; i < audio.Length; i += 2)
        {
            short sample = (short)((audio[i+1] << 8) | audio[i]);
            sum += Mathf.Abs(sample / 32768f);
        }
        return sum / (audio.Length / 2);
    }
}
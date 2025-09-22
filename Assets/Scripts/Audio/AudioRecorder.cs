using UnityEngine;
using System.Collections;
using UnityEngine.XR;
using System.Collections.Generic;
using UnityEngine.InputSystem;
using UnityEngine.UI;
using TMPro; // Added for TextMeshPro support

public class AudioRecorder : MonoBehaviour
{
    public AudioSource audioFeedback;
    public WhisperAPI whisperAPI;
    public Button recordButton; // Assign in the inspector
    public Image recordingPanel; // Assign a panel in the inspector
    public TMP_Text commandTextUI; // Assign in the inspector

    private AudioClip recording;
    private bool isRecording = false;
    private UnityEngine.XR.InputDevice leftController;
    public bool buttonPressedLastFrame = false;

    private Keyboard keyboard;
    private string currentDeviceName; // Track the microphone device

    private float lastCommandTime;
    private const float MIN_COMMAND_INTERVAL = 3f;

    void Start()
    {
        InitializeController();
        keyboard = Keyboard.current;

        if (recordButton != null)
        {
            recordButton.onClick.AddListener(ToggleRecording);
        }

        if (recordingPanel != null)
        {
            recordingPanel.color = Color.white;
        }
    }

    void Update()
    {
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

    public void ToggleRecording()
    {
        if (isRecording)
        {
            StopRecording();
        }
        else
        {
            StartRecording();
        }
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
        string[] devices = Microphone.devices;
        if (devices.Length == 0)
        {
            Debug.LogError("No microphone devices found!");
            return;
        }

        Debug.Log("Available microphones:");
        foreach (string device in devices)
        {
            Debug.Log(device);
        }

        currentDeviceName = null;
        foreach (string device in devices)
        {
            if (device.ToLower().Contains("oculus") ||
                device.ToLower().Contains("quest") ||
                device.ToLower().Contains("headset"))
            {
                currentDeviceName = device;
                break;
            }
        }

        if (string.IsNullOrEmpty(currentDeviceName))
        {
            currentDeviceName = devices[0];
        }

        Debug.Log($"Using microphone: {currentDeviceName}");

        recording = Microphone.Start(currentDeviceName, false, 10, 44100);
        isRecording = true;
        if (audioFeedback != null) audioFeedback.Play();

        UpdatePanelColor();
    }

    void StopRecording()
    {
        if (!isRecording) return;

        Microphone.End(currentDeviceName);
        isRecording = false;
        StartCoroutine(ProcessVoiceCommand());

        UpdatePanelColor();
    }

    void UpdatePanelColor()
    {
        if (recordingPanel != null)
        {
            recordingPanel.color = isRecording ? Color.red : Color.white;
        }
    }

    IEnumerator ProcessVoiceCommand()
    {
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
            ShowCommandText(quickCommand); // Display offline command
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
            ShowCommandText(transcript); // Display transcribed command
            lastCommandTime = Time.time;
            CommandParser.ExecuteCommand(transcript);
        }
        else
        {
            Debug.LogWarning("Transcription failed or returned empty");
        }
    }

    private string DetectSimpleCommand(byte[] audioData)
    {
        try
        {
            float energy = CalculateAudioEnergy(audioData);
            if (energy < 0.1f) return null;

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
            short sample = (short)((audio[i + 1] << 8) | audio[i]);
            sum += Mathf.Abs(sample / 32768f);
        }
        return sum / (audio.Length / 2);
    }

    private void ShowCommandText(string command)
    {
        if (commandTextUI != null)
        {
            commandTextUI.text = $"Command: {command}";
            StartCoroutine(ClearCommandText(4f));
        }
    }

    private IEnumerator ClearCommandText(float delay)
    {
        yield return new WaitForSeconds(delay);
        if (commandTextUI != null)
            commandTextUI.text = "";
    }
}
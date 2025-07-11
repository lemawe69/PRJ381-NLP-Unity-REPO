using UnityEngine;

public class AudioRecorder : MonoBehaviour
{
    // This script allows for recording audio using the microphone.
    // It starts recording when StartRecording is called and stops when StopRecording is called.
    
    private AudioClip recording;
    private bool isRecording = false;
    private const int RECORDING_LENGTH = 10; // Max 10 seconds

    // Starts recording audio from the microphone.
    public void StartRecording()
    {
        // If already recording, do nothing.
        if (isRecording) return;
        // Start recording audio from the default microphone.
        recording = Microphone.Start(null, false, RECORDING_LENGTH, 44100);
        isRecording = true;
    }

    // Stops the recording and returns the recorded AudioClip.
    public AudioClip StopRecording()
    {
        // If not recording, return null.
        if (!isRecording) return null;
        // Stop the microphone and get the recorded audio clip.
        Microphone.End(null);
        isRecording = false;
        return recording;
    }

    // Checks if the recorder is currently recording.
    public bool IsRecording() => isRecording;
}

using UnityEngine;
using UnityEngine.UI;
using System.IO;
using System.Diagnostics;
using System.Collections;
using System.Collections.Generic;

public class FrontCamFrameRecorder : MonoBehaviour
{
    [Header("Recording Settings")]
    public RenderTexture feedTexture;
    public int captureFrameRate = 30;
    public string outputDirectory = "C:/DroneFootage/FrontCamFrames";
    public string outputVideoPrefix = "droneFeed_"; // Each file gets timestamp

    [Header("UI Elements")]
    public Text recIndicator;       // UI text: "REC"
    public Text timerText;          // UI text: "00:00:00"
    public Color recOnColor = Color.red;
    public Color recOffColor = new Color(1, 0, 0, 0.2f);

    private Texture2D frameTexture;
    private int frameCount = 0;
    private List<string> savedFrames = new List<string>();
    private string ffmpegPath;
    private bool isRecording = false;
    private float recordingStartTime;

    void Start()
    {
        if (!Directory.Exists(outputDirectory))
            Directory.CreateDirectory(outputDirectory);

        frameTexture = new Texture2D(feedTexture.width, feedTexture.height, TextureFormat.RGB24, false);

        // ffmpeg path
#if UNITY_STANDALONE_WIN
        ffmpegPath = Path.Combine(Application.streamingAssetsPath, "FFmpeg/Windows/ffmpeg.exe");
#elif UNITY_STANDALONE_OSX
        ffmpegPath = Path.Combine(Application.streamingAssetsPath, "FFmpeg/Mac/ffmpeg");
#elif UNITY_STANDALONE_LINUX
        ffmpegPath = Path.Combine(Application.streamingAssetsPath, "FFmpeg/Linux/ffmpeg");
#endif

        if (recIndicator != null) recIndicator.gameObject.SetActive(false);
        if (timerText != null) timerText.gameObject.SetActive(false);

        UnityEngine.Debug.Log($"Recorder ready. FFmpeg at: {ffmpegPath}");
    }

    void LateUpdate()
    {
        if (!isRecording) return;

        // Capture frame
        RenderTexture.active = feedTexture;
        frameTexture.ReadPixels(new Rect(0, 0, feedTexture.width, feedTexture.height), 0, 0);
        frameTexture.Apply();
        RenderTexture.active = null;

        // Save PNG
        string framePath = Path.Combine(outputDirectory, $"frame_{frameCount:D04}.png");
        File.WriteAllBytes(framePath, frameTexture.EncodeToPNG());
        savedFrames.Add(framePath);

        frameCount++;

        // Update timer
        if (timerText != null)
        {
            float elapsed = Time.time - recordingStartTime;
            System.TimeSpan t = System.TimeSpan.FromSeconds(elapsed);
            timerText.text = string.Format("{0:00}:{1:00}:{2:00}", t.Hours, t.Minutes, t.Seconds);
        }
    }

    // --- UI Button ---
    public void ToggleRecording()
    {
        if (isRecording)
        {
            isRecording = false;
            if (recIndicator != null) recIndicator.gameObject.SetActive(false);
            if (timerText != null) timerText.gameObject.SetActive(false);
            StartCoroutine(StopAndCompile());
        }
        else
        {
            frameCount = 0;
            savedFrames.Clear();
            isRecording = true;
            recordingStartTime = Time.time;

            if (recIndicator != null)
            {
                recIndicator.gameObject.SetActive(true);
                StartCoroutine(BlinkREC());
            }
            if (timerText != null)
            {
                timerText.gameObject.SetActive(true);
                timerText.text = "00:00:00";
            }

            UnityEngine.Debug.Log("Recording started...");
        }
    }

    private IEnumerator StopAndCompile()
    {
        UnityEngine.Debug.Log("Recording stopped. Compiling video...");

        yield return new WaitForEndOfFrame();

        // Unique video filename with timestamp
        string timestamp = System.DateTime.Now.ToString("yyyyMMdd_HHmmss");
        string videoPath = Path.Combine(outputDirectory, outputVideoPrefix + timestamp + ".mp4");

        string args = $"-framerate {captureFrameRate} -i \"{outputDirectory}/frame_%04d.png\" -c:v libx264 -crf 18 -pix_fmt yuv420p \"{videoPath}\"";

        ProcessStartInfo startInfo = new ProcessStartInfo
        {
            FileName = ffmpegPath,
            Arguments = args,
            UseShellExecute = false,
            CreateNoWindow = true,
        };

        try
        {
            Process ffmpeg = Process.Start(startInfo);
            ffmpeg.WaitForExit();

            UnityEngine.Debug.Log("Video saved at: " + videoPath);

            // Cleanup
            foreach (string frame in savedFrames)
            {
                if (File.Exists(frame)) File.Delete(frame);
            }
            UnityEngine.Debug.Log("Cleaned up PNG frames.");
        }
        catch (System.Exception e)
        {
            UnityEngine.Debug.LogError("FFmpeg failed: " + e.Message);
        }
    }

    private IEnumerator BlinkREC()
    {
        while (isRecording && recIndicator != null)
        {
            recIndicator.color = recOnColor;
            yield return new WaitForSeconds(0.5f);
            recIndicator.color = recOffColor;
            yield return new WaitForSeconds(0.5f);
        }
    }
}

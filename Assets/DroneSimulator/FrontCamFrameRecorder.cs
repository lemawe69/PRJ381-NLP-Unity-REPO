using UnityEngine;
using System.IO;
using System.Diagnostics;
using System.Collections.Generic;

public class FrontCamFrameRecorder : MonoBehaviour
{
    [Header("Recording Settings")]
    public RenderTexture feedTexture;            // Assign your FrontCam RenderTexture
    public int captureFrameRate = 30;            // FPS
    public string outputDirectory = "C:/DroneFootage/FrontCamFrames";
    public string outputVideoName = "droneFeed.mp4";

    private Texture2D frameTexture;
    private int frameCount = 0;
    private List<string> savedFrames = new List<string>(); // Track saved frames
    private string ffmpegPath;

    void Start()
    {
        // Lock framerate for consistent capture
        Time.captureFramerate = captureFrameRate;

        // Ensure output directory exists
        if (!Directory.Exists(outputDirectory))
            Directory.CreateDirectory(outputDirectory);

        // Prepare frame buffer
        frameTexture = new Texture2D(feedTexture.width, feedTexture.height, TextureFormat.RGB24, false);

        // Build ffmpeg path (use StreamingAssets)
#if UNITY_STANDALONE_WIN
            ffmpegPath = Path.Combine(Application.streamingAssetsPath, "FFmpeg/Windows/ffmpeg.exe");
#elif UNITY_STANDALONE_OSX
            ffmpegPath = Path.Combine(Application.streamingAssetsPath, "FFmpeg/Mac/ffmpeg");
#elif UNITY_STANDALONE_LINUX
            ffmpegPath = Path.Combine(Application.streamingAssetsPath, "FFmpeg/Linux/ffmpeg");
#endif

        UnityEngine.Debug.Log($"Recording started. Saving frames to: {outputDirectory}");
        UnityEngine.Debug.Log($"Expecting FFmpeg at: {ffmpegPath}");
    }

    void LateUpdate()
    {
        // Capture the frame from RenderTexture
        RenderTexture.active = feedTexture;
        frameTexture.ReadPixels(new Rect(0, 0, feedTexture.width, feedTexture.height), 0, 0);
        frameTexture.Apply();
        RenderTexture.active = null;

        // Save PNG
        string framePath = Path.Combine(outputDirectory, $"frame_{frameCount:D04}.png");
        File.WriteAllBytes(framePath, frameTexture.EncodeToPNG());
        savedFrames.Add(framePath);

        frameCount++;
    }

    void OnApplicationQuit()
    {
        // Where the final video goes
        string videoPath = Path.Combine(outputDirectory, outputVideoName);

        // Build ffmpeg arguments
        string args = $"-framerate {captureFrameRate} -i \"{outputDirectory}/frame_%04d.png\" -c:v libx264 -pix_fmt yuv420p \"{videoPath}\"";

        ProcessStartInfo startInfo = new ProcessStartInfo
        {
            FileName = ffmpegPath,
            Arguments = args,
            UseShellExecute = false,
            CreateNoWindow = true,
        };

        try
        {
            UnityEngine.Debug.Log("Running FFmpeg to create MP4...");
            Process ffmpeg = Process.Start(startInfo);
            ffmpeg.WaitForExit();

            UnityEngine.Debug.Log("Video saved at: " + videoPath);

            // Cleanup frames after successful video creation
            foreach (string frame in savedFrames)
            {
                if (File.Exists(frame))
                    File.Delete(frame);
            }
            UnityEngine.Debug.Log("Cleaned up PNG frames.");
        }
        catch (System.Exception e)
        {
            UnityEngine.Debug.LogError("Failed to run FFmpeg: " + e.Message);
        }
    }
}

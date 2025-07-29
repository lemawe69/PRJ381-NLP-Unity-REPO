using UnityEngine;
using System.IO;

public class FrontCamFrameRecorder : MonoBehaviour
{
    public RenderTexture feedTexture;        // Assign FrontCamFeedTexture
    public int captureFrameRate = 30;        // FPS
    public string outputDirectory = "C:/DroneFootage/FrontCamFrames"; // External save location

    private Texture2D frameTexture;
    private int frameCount = 0;

    void Start()
    {
        // Lock frame rate for consistent capture
        Time.captureFramerate = captureFrameRate;

        // Create output directory if it doesn't exist
        if (!Directory.Exists(outputDirectory)) Directory.CreateDirectory(outputDirectory);

        // Prepare a buffer to read frames
        frameTexture = new Texture2D(feedTexture.width, feedTexture.height, TextureFormat.RGB24, false);

        Debug.Log("Saving drone feed frames to: " + outputDirectory);
    }

    void LateUpdate()
    {
        // Capture the frame from RenderTexture
        RenderTexture.active = feedTexture;
        frameTexture.ReadPixels(new Rect(0, 0, feedTexture.width, feedTexture.height), 0, 0);
        frameTexture.Apply();
        RenderTexture.active = null;

        // Save PNG to external folder
        string framePath = Path.Combine(outputDirectory, $"frame_{frameCount:D04}.png");
        File.WriteAllBytes(framePath, frameTexture.EncodeToPNG());

        frameCount++;
    }
}

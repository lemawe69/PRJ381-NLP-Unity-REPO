using UnityEngine;
using UnityEngine.Networking;
using System.Collections;
using Newtonsoft.Json.Linq;

public class WhisperAPI : MonoBehaviour
{
    [SerializeField] private string apiKey = "";
    private string apiUrl = "https://api.openai.com/v1/audio/transcriptions";

    // Common drone vocabulary prompt (helps Whisper bias towards these commands)
    private string commandPrompt = "Drone control commands include: take off, land, emergency stop, go forward, go back, go left, go right, go up, go down, turn left, turn right, set speed, flip forward, flip back, flip left, flip right";

    public IEnumerator TranscribeAudio(byte[] audioData, System.Action<string> callback)
    {
        if (string.IsNullOrEmpty(apiKey))
        {
            Debug.LogError("API Key is not set.");
            callback?.Invoke(null);
            yield break;
        }
        if (audioData == null || audioData.Length == 0)
        {
            Debug.LogError("Audio data is null or empty.");
            callback?.Invoke(null);
            yield break;
        }

        WWWForm form = new WWWForm();
        form.AddBinaryData("file", audioData, "audio.wav", "audio/wav");
        form.AddField("model", "whisper-1");
        form.AddField("temperature", "0"); // deterministic transcription
        form.AddField("prompt", commandPrompt); // bias Whisper towards drone commands

        int retryCount = 0;
        float initialDelay = 0.5f;
        float maxDelay = 60f;

        while (retryCount < 5)
        {
            using (UnityWebRequest www = UnityWebRequest.Post(apiUrl, form))
            {
                www.timeout = 20;
                www.SetRequestHeader("Authorization", "Bearer " + apiKey);
                yield return www.SendWebRequest();

                if (www.result == UnityWebRequest.Result.Success)
                {
                    JObject json = JObject.Parse(www.downloadHandler.text);
                    string transcript = json["text"]?.ToString()?.Trim();

                    // ?? Normalize transcription before passing to CommandParser
                    transcript = NormalizeCommand(transcript);

                    Debug.Log($"Whisper Transcription: {transcript}");
                    callback?.Invoke(transcript);
                    yield break;
                }
                else if (www.responseCode == 429) // Rate limited
                {
                    float delay = Mathf.Min(initialDelay * Mathf.Pow(2, retryCount), maxDelay);
                    delay += Random.Range(0f, 1f);
                    Debug.LogWarning($"Rate limit exceeded. Retrying in {delay} seconds...");
                    yield return new WaitForSeconds(delay);
                    retryCount++;
                }
                else
                {
                    Debug.LogError($"API Error: {www.error}");
                    callback?.Invoke(null);
                    yield break;
                }
            }
        }

        Debug.LogError("Max retries reached. Transcription failed.");
        callback?.Invoke(null);
    }


    private string NormalizeCommand(string input)
    {
        if (string.IsNullOrEmpty(input)) return "";

        string cmd = input.ToLower().Trim();

        // Common variations ? normalize
        cmd = cmd.Replace("takeoff", "take off");
        cmd = cmd.Replace("lift off", "take off");
        cmd = cmd.Replace("fly up", "go up");
        cmd = cmd.Replace("fly down", "go down");
        cmd = cmd.Replace("fly forward", "go forward");
        cmd = cmd.Replace("fly back", "go back");
        cmd = cmd.Replace("fly left", "go left");
        cmd = cmd.Replace("fly right", "go right");
        cmd = cmd.Replace("stop now", "emergency");
        cmd = cmd.Replace("halt", "emergency");

        return cmd;
    }
}

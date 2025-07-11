using UnityEngine;
using UnityEngine.Networking;
using System.Collections;
using Newtonsoft.Json.Linq;

public class WhisperAPI : MonoBehaviour
{
    // This class provides functionality to interact with the Whisper API for audio transcription.
    // It sends audio data to the API and retrieves the transcribed text. 
    // The API key and URL should be set before calling the TranscribeAudio method.
    // apiKey is a secrete key from OpenAI so add it here from OpenAI.
    private string apiKey = "";
    private string apiUrl = "https://api.openai.com/v1/audio/transcriptions";

    public IEnumerator TranscribeAudio(byte[] audioData, System.Action<string> callback)
    {
        // Check if the API key is set.
        if (string.IsNullOrEmpty(apiKey))
        {
            Debug.LogError("API Key is not set.");
            callback?.Invoke(null);
            yield break;
        }
        // Validate the audio data before sending it to the API.
        if (audioData == null || audioData.Length == 0)
        {
            Debug.LogError("Audio data is null or empty.");
            callback?.Invoke(null);
            yield break;
        }

        // Prepare the form data for the API request.
        // The audio data is sent as a binary file with the name "file".
        WWWForm form = new WWWForm();
        form.AddBinaryData("file", audioData, "audio.wav", "audio/wav");
        form.AddField("model", "whisper-1");

        // Create a UnityWebRequest to send the audio data to the Whisper API.
        // The request is sent as a POST request with the form data.
        using (UnityWebRequest www = UnityWebRequest.Post(apiUrl, form))
        {
            www.SetRequestHeader("Authorization", "Bearer " + apiKey);
            yield return www.SendWebRequest();

            if (www.result == UnityWebRequest.Result.Success)
            {
                JObject json = JObject.Parse(www.downloadHandler.text);
                string transcript = json["text"]?.ToString()?.Trim();
                callback?.Invoke(transcript);
            }
            else
            {
                Debug.LogError($"API Error: {www.error}");
                callback?.Invoke(null);
            }
        }
    }
}

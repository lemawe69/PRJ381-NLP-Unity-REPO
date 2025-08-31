using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Networking;
using System.Collections;
using Newtonsoft.Json.Linq;

public class WeatherManager : MonoBehaviour
{
    public Text temperatureText;
    public Text windText;
    public Text humidityText;
    public Text descriptionText;

    private string apiKey = "da80afbb46faad148e4e8af2a7f7f039";

    void Start()
    {
        // Hardcoded coordinates for Tshwane, South Africa
        float latitude = -25.7479f;
        float longitude = 28.2293f;
        StartCoroutine(GetWeather(latitude, longitude));
    }

    IEnumerator GetWeather(float lat, float lon)
    {
        string url = $"https://api.openweathermap.org/data/2.5/weather?lat={lat}&lon={lon}&appid={apiKey}&units=metric";

        UnityWebRequest request = UnityWebRequest.Get(url);
        yield return request.SendWebRequest();

        if (request.result != UnityWebRequest.Result.Success)
        {
            Debug.LogError("Weather API error: " + request.error);
            temperatureText.text = "Weather unavailable";
            windText.text = "";
            humidityText.text = "";
            descriptionText.text = "";
            yield break;
        }

        JObject data = JObject.Parse(request.downloadHandler.text);

        float temp = (float)data["main"]["temp"];
        float wind = (float)data["wind"]["speed"];
        int humidity = (int)data["main"]["humidity"];
        string description = (string)data["weather"][0]["description"];

        temperatureText.text = $"Temp: {temp}°C";
        windText.text = $"Wind: {wind} m/s";
        humidityText.text = $"Humidity: {humidity}%";
        descriptionText.text = $"Condition: {description}";
    }
}
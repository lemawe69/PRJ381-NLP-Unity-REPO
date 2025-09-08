using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Networking;
using System;

public class Map : MonoBehaviour
{
    public string apiKey;
    public float lat = -25.68308f;
    public float lon = 28.13153f;
    public int zoom = 12;

    public enum resolution { low = 1, high = 2 };
    public resolution mapResolution = resolution.high; //Use high resolution for better quality

    public enum type { roadmap, satellite, hybrid, terrain }; //Fixed typo: "gybrid" → "hybrid"
    public type mapType = type.hybrid;

    private string url = "";
    private int mapWidth = 640;
    private int mapHeight = 640;
    private bool mapIsLoading = false;
    private Rect rect;

    private string apiKeyLast;
    private float latLast = -33.85660618894087f;
    private float lonLast = 151.21500701957325f;
    private int zoomLast = 12;
    private resolution mapResolutionLast = resolution.low;
    private type mapTypeLast = type.roadmap;
    private bool updateMap = true;

    void Start()
    {
        rect = gameObject.GetComponent<RawImage>().rectTransform.rect;
        mapWidth = (int)Math.Round(rect.width);
        mapHeight = (int)Math.Round(rect.height);
        StartCoroutine(GetGoogleMap());
    }

    void Update()
    {
        if (updateMap && (
            apiKeyLast != apiKey ||
            !Mathf.Approximately(latLast, lat) ||
            !Mathf.Approximately(lonLast, lon) ||
            zoomLast != zoom ||
            mapResolutionLast != mapResolution ||
            mapTypeLast != mapType))
        {
            rect = gameObject.GetComponent<RawImage>().rectTransform.rect;
            mapWidth = (int)Math.Round(rect.width);
            mapHeight = (int)Math.Round(rect.height);
            StartCoroutine(GetGoogleMap());
            updateMap = false;
        }
    }

    IEnumerator GetGoogleMap()
    {
        url = $"https://maps.googleapis.com/maps/api/staticmap?center={lat},{lon}&zoom={zoom}&size={mapWidth}x{mapHeight}&scale={(int)mapResolution}&maptype={mapType}&key={apiKey}";
        Debug.Log("Map URL: " + url); //Optional: helps debug malformed requests

        mapIsLoading = true;
        UnityWebRequest www = UnityWebRequestTexture.GetTexture(url);
        yield return www.SendWebRequest();

        if (www.result != UnityWebRequest.Result.Success)
        {
            mapIsLoading = false;
            Debug.LogError("Map request failed: " + www.error);
        }
        else
        {
            Texture2D mapTexture = DownloadHandlerTexture.GetContent(www);
            RawImage rawImage = gameObject.GetComponent<RawImage>();

            if (rawImage != null && mapTexture != null)
            {
                rawImage.texture = mapTexture;
            }
            else
            {
                Debug.LogWarning("RawImage component or downloaded texture is missing.");
            }

            mapIsLoading = false;

            apiKeyLast = apiKey;
            latLast = lat;
            lonLast = lon;
            zoomLast = zoom;
            mapResolutionLast = mapResolution;
            mapTypeLast = mapType;
            updateMap = true;
        }
    }
}
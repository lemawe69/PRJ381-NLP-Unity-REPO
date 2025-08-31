using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

public class UpdateGPSText : MonoBehaviour
{
    public Text coordinates;

    private void Update()
    {  
       coordinates.text = "Lat: " + GPS.Instance.latitude.ToString() + ", Long: " + GPS.Instance.longitude.ToString(); 
        
    }
}

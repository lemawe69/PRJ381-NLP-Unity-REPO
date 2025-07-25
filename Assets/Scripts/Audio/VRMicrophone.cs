using System.Linq;
using UnityEngine;

public class VRMicrophone : MonoBehaviour
{
    public AudioClip StartRecording() {
        string mic = Microphone.devices.FirstOrDefault(d => d.Contains("Oculus"));
        return Microphone.Start(mic, false, 5, 44100);
    }
}

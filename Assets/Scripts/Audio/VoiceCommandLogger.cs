using UnityEngine;
using TMPro;
using System.Collections.Generic;

public class VoiceCommandLogger : MonoBehaviour
{
    public TMP_Text logText;
    private List<string> commands = new List<string>();

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.V)) AddCommand("Scan Area");
        if (Input.GetKeyDown(KeyCode.T)) AddCommand("Takeoff");
    }

    void AddCommand(string command)
    {
        commands.Add(command);
        logText.text = string.Join("\n", commands);
    }
}
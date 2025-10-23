using UnityEngine;
using TMPro;

public class ShowingCommands : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI commandsText;

    public void displayCommand(string command)
    {
        commandsText.text = command;
    }
}

using System;
using UnityEngine;
using Tello;
using System.Text.RegularExpressions;
using System.Collections.Generic;

public class droneController
{
	private static TelloCmd tello;

    public droneController()
	{
		tello = new TelloCmd();

		tello.Connect();
	}

    private static Dictionary<string, Action> commandCache = new Dictionary<string, Action>()
    {
        {"take off", () => tello.TakeOff()},
        {"land", () => tello.Land()},
        {"emergency stop", () => tello.Emergency()},
    };

    public void execute_command(string cmd)
	{
        if (tello == null) return;

        command = command.ToLower().Trim();
        Debug.Log($"Executing: {command}");

        if (commandCache.ContainsKey(command))
        {
            commandCache[command].Invoke();
            return;
        }

        //Handle natural language variations
        if (Regex.IsMatch(command, @"take off|launch|start|fly"))
            tello.TakeOff();

        else if (Regex.IsMatch(command, @"land|stop|end|come down"))
            tello.Land();

        else if (Regex.IsMatch(command, @"emergency|halt|stop now"))
            tello.EmergencyStop();

        else if (TryParseMovement(command, @"go forward (\d+)", "forward")) return;
        else if (TryParseMovement(command, @"go back(?:ward)? (\d+)", "back")) return;
        else if (TryParseMovement(command, @"go left (\d+)", "left")) return;
        else if (TryParseMovement(command, @"go right (\d+)", "right")) return;
        else if (TryParseMovement(command, @"go up (\d+)", "up")) return;
        else if (TryParseMovement(command, @"go down (\d+)", "down")) return;

        // Rotation commands
        else if (TryParseRotation(command, @"turn left (\d+)", "ccw")) return;
        else if (TryParseRotation(command, @"turn right (\d+)", "cw")) return;

        // Flip commands
        else if (Regex.IsMatch(command, @"do a front flip|flip forward"))
            tello.SendCommand("flip f");
        else if (Regex.IsMatch(command, @"do a back flip|flip backward"))
            tello.Flip("flip b");
        else if (Regex.IsMatch(command, @"do a left flip|flip left"))
            tello.Flip("flip l");
        else if (Regex.IsMatch(command, @"do a right flip|flip right"))
            tello.Flip("flip r");

        // Speed control
        else if (TryParseSpeed(command, @"set speed to (\d+)")) return;

        else Debug.LogWarning($"Unrecognized command: {command}");


    }


    private static bool TryParseMovement(string command, string pattern, string action)
    {
        Match match = Regex.Match(command, pattern);
        if (match.Success && int.TryParse(match.Groups[1].Value, out int distance))
        {
            var clamped_distance = Mathf.Clamp(distance, 20, 500);

            tello.SendCommand($"{command} {clamped_distance}");

            return true;
        }
        return false;
    }


    private static bool TryParseRotation(string command, string pattern, string direction)
    {
        Match match = Regex.Match(command, pattern);
        if (match.Success && int.TryParse(match.Groups[1].Value, out int degrees))
        {
            var clamped_angle = Mathf.Clamp(degrees, 1, 360)

            tello.SendCommand($"{command} {clamped_angle}");
            
            return true;
        }
        return false;
    }

    private static bool TryParseSpeed(string command, string pattern)
    {
        Match match = Regex.Match(command, pattern);
        if (match.Success && int.TryParse(match.Groups[1].Value, out int speed))
        {
            var clamped_speed = Mathf.Clamp(speed, 10, 100)

            tello.SendCommand($"speed {clamped_speed}")
            return true;
        }
        return false;
    }
}

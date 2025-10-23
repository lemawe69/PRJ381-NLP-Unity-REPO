using UnityEngine;
using System.Text.RegularExpressions;
using System.Collections.Generic;
using System;

public static class CommandParser 
{
    // Class for Tello drone controller
    private static TelloController tello;
    private const int DefaultDistance = 50; // Default distance in cm
    private const int DefaultRotation = 90; // Default rotation in degrees

    [SerializeField] static ShowingCommands showingCommands;

    static CommandParser()
    {
        tello = GameObject.FindObjectOfType<TelloController>();
        showingCommands = GameObject.FindObjectOfType<ShowingCommands>();
    }

    private static Dictionary<string, Action> commandCache = new Dictionary<string, Action>()
    {
        {"take off", () => tello.TakeOff()},
        {"land", () => tello.Land()},
        {"emergency stop", () => tello.EmergencyStop()},
        {"forward", () => tello.Move("forward", DefaultDistance)},
        {"back", () => tello.Move("back", DefaultDistance)},
        {"left", () => tello.Move("left", DefaultDistance)},
        {"right", () => tello.Move("right", DefaultDistance)},
        {"up", () => tello.Move("up", DefaultDistance)},
        {"down", () => tello.Move("down", DefaultDistance)},
        {"turn left", () => tello.Rotate("ccw", DefaultRotation)},
        {"turn right", () => tello.Rotate("cw", DefaultRotation)},
    };

    public static void ExecuteCommand(string command)
    {

        if (tello == null) return;

        command = command.ToLower().Trim();
        Debug.Log($"Executing: {command}");

        showingCommands.displayCommand($"Executing: {command}");

        if (commandCache.ContainsKey(command))
        {
            commandCache[command].Invoke();
            return;
        }

        // Handle natural language variations
        if (Regex.IsMatch(command, @"take off|launch|start|fly|lift off"))
            tello.TakeOff();

        else if (Regex.IsMatch(command, @"land|stop|end|come down|touch down"))
            tello.Land();

        else if (Regex.IsMatch(command, @"emergency|halt|stop now|abort"))
            tello.EmergencyStop();

        // Rotation commands should be checked before movement commands
        else if (TryParseRotation(command, @"turn left (\d+)", "ccw")) return;
        else if (TryParseRotation(command, @"turn right (\d+)", "cw")) return;
        else if (TryParseRotation(command, @"rotate left (\d+)", "ccw")) return;
        else if (TryParseRotation(command, @"rotate right (\d+)", "cw")) return;
        else if (TryParseRotation(command, @"spin left (\d+)", "ccw")) return;
        else if (TryParseRotation(command, @"spin right (\d+)", "cw")) return;
        
        else if (Regex.IsMatch(command, @"turn left|rotate left|spin left"))
            tello.Rotate("ccw", DefaultRotation);
        else if (Regex.IsMatch(command, @"turn right|rotate right|spin right"))
            tello.Rotate("cw", DefaultRotation);

        // Movement commands with distance
        else if (TryParseMovement(command, @"go forward (\d+)", "forward")) return;
        else if (TryParseMovement(command, @"go back(?:ward)? (\d+)", "back")) return;
        else if (TryParseMovement(command, @"go left (\d+)", "left")) return;
        else if (TryParseMovement(command, @"go right (\d+)", "right")) return;
        else if (TryParseMovement(command, @"go up (\d+)", "up")) return;
        else if (TryParseMovement(command, @"go down (\d+)", "down")) return;

        else if (TryParseMovement(command, @"move forward (\d+)", "forward")) return;
        else if (TryParseMovement(command, @"move back(?:ward)? (\d+)", "back")) return;
        else if (TryParseMovement(command, @"move left (\d+)", "left")) return;
        else if (TryParseMovement(command, @"move right (\d+)", "right")) return;
        else if (TryParseMovement(command, @"move up (\d+)", "up")) return;
        else if (TryParseMovement(command, @"move down (\d+)", "down")) return;

        else if (TryParseMovement(command, @"fly forward (\d+)", "forward")) return;
        else if (TryParseMovement(command, @"fly back(?:ward)? (\d+)", "back")) return;
        else if (TryParseMovement(command, @"fly left (\d+)", "left")) return;
        else if (TryParseMovement(command, @"fly right (\d+)", "right")) return;
        else if (TryParseMovement(command, @"fly up (\d+)", "up")) return;
        else if (TryParseMovement(command, @"fly down (\d+)", "down")) return;

        // Movement commands without distance (use default)
        else if (Regex.IsMatch(command, @"go forward|move forward|fly forward|forward"))
            tello.Move("forward", DefaultDistance);
        else if (Regex.IsMatch(command, @"go back|move back|fly back|back(?:ward)?"))
            tello.Move("back", DefaultDistance);
        else if (Regex.IsMatch(command, @"go left|move left|fly left|left"))
            tello.Move("left", DefaultDistance);
        else if (Regex.IsMatch(command, @"go right|move right|fly right|right"))
            tello.Move("right", DefaultDistance);
        else if (Regex.IsMatch(command, @"go up|move up|fly up|up|rise"))
            tello.Move("up", DefaultDistance);
        else if (Regex.IsMatch(command, @"go down|move down|fly down|down|descend"))
            tello.Move("down", DefaultDistance);

        // Flip commands
        else if (Regex.IsMatch(command, @"do a front flip|flip forward|front flip"))
            tello.Flip("f");
        else if (Regex.IsMatch(command, @"do a back flip|flip backward|back flip"))
            tello.Flip("b");
        else if (Regex.IsMatch(command, @"do a left flip|flip left|left flip"))
            tello.Flip("l");
        else if (Regex.IsMatch(command, @"do a right flip|flip right|right flip"))
            tello.Flip("r");

        // Speed control
        else if (TryParseSpeed(command, @"set speed to (\d+)")) return;
        else if (TryParseSpeed(command, @"speed (\d+)")) return;

        // Hover command
        // else if (Regex.IsMatch(command, @"hover|stay|hold position"))
        //     tello.Hover();

        else Debug.LogWarning($"Unrecognized command: {command}");
    }

    private static bool TryParseMovement(string command, string pattern, string action)
    {
        Match match = Regex.Match(command, pattern);
        if (match.Success && int.TryParse(match.Groups[1].Value, out int distance))
        {
            tello.Move(action, Mathf.Clamp(distance, 20, 500));
            return true;
        }
        return false;
    }

    private static bool TryParseRotation(string command, string pattern, string direction)
    {
        Match match = Regex.Match(command, pattern);
        if (match.Success && int.TryParse(match.Groups[1].Value, out int degrees))
        {
            tello.Rotate(direction, Mathf.Clamp(degrees, 1, 360));
            return true;
        }
        return false;
    }

    private static bool TryParseSpeed(string command, string pattern)
    {
        Match match = Regex.Match(command, pattern);
        if (match.Success && int.TryParse(match.Groups[1].Value, out int speed))
        {
            tello.SetSpeed(Mathf.Clamp(speed, 10, 100));
            return true;
        }
        return false;
    }
}

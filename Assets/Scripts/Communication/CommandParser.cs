using UnityEngine;
using System.Text.RegularExpressions;

public static class CommandParser
{
    private static TelloController tello;

    static CommandParser()
    {
        tello = GameObject.FindObjectOfType<TelloController>();
    }

    public static void ExecuteCommand(string command)
    {
        if (tello == null) return;

        command = command.ToLower().Trim();
        Debug.Log($"Executing: {command}");

        // --- Basic Commands ---
        if (Regex.IsMatch(command, @"take off|launch|start|fly"))
            tello.TakeOff();

        else if (Regex.IsMatch(command, @"land|stop flight|end flight|come down"))
            tello.Land();

        else if (Regex.IsMatch(command, @"emergency|halt|stop now"))
            tello.EmergencyStop();

        // --- Directional with Distance ---
        else if (TryParseMovement(command, @"(?:go|move|fly) forward ([\w\d ]+)", "forward")) return;
        else if (TryParseMovement(command, @"(?:go|move|fly) back(?:ward)? ([\w\d ]+)", "back")) return;
        else if (TryParseMovement(command, @"(?:go|move|fly) left ([\w\d ]+)", "left")) return;
        else if (TryParseMovement(command, @"(?:go|move|fly) right ([\w\d ]+)", "right")) return;
        else if (TryParseMovement(command, @"(?:go|move|fly) up ([\w\d ]+)", "up")) return;
        else if (TryParseMovement(command, @"(?:go|move|fly) down ([\w\d ]+)", "down")) return;

        // --- Directional WITHOUT Distance (default 100 cm) ---
        else if (Regex.IsMatch(command, @"(?:go|move|fly) forward$"))
            tello.Move("forward", tello.defaultMoveDistance);
        else if (Regex.IsMatch(command, @"(?:go|move|fly) back(?:ward)?$"))
            tello.Move("back", tello.defaultMoveDistance);
        else if (Regex.IsMatch(command, @"(?:go|move|fly) left$"))
            tello.Move("left", tello.defaultMoveDistance);
        else if (Regex.IsMatch(command, @"(?:go|move|fly) right$"))
            tello.Move("right", tello.defaultMoveDistance);
        else if (Regex.IsMatch(command, @"(?:go|move|fly) up$"))
            tello.Move("up", tello.defaultMoveDistance);
        else if (Regex.IsMatch(command, @"(?:go|move|fly) down$"))
            tello.Move("down", tello.defaultMoveDistance);

        // --- Rotation Commands ---
        else if (TryParseRotation(command, @"turn left ([\w\d ]+)", "ccw")) return;
        else if (TryParseRotation(command, @"turn right ([\w\d ]+)", "cw")) return;

        // --- Flip Commands ---
        else if (Regex.IsMatch(command, @"do a front flip|flip forward"))
            tello.Flip("f");
        else if (Regex.IsMatch(command, @"do a back flip|flip back"))
            tello.Flip("b");
        else if (Regex.IsMatch(command, @"flip left"))
            tello.Flip("l");
        else if (Regex.IsMatch(command, @"flip right"))
            tello.Flip("r");

        // --- Speed Control ---
        else if (TryParseSpeed(command, @"set speed to ([\w\d ]+)")) return;

        else Debug.LogWarning($"Unrecognized command: {command}");
    }

    // --- Helpers ---
    private static bool TryParseMovement(string command, string pattern, string action)
    {
        Match match = Regex.Match(command, pattern);
        if (match.Success)
        {
            int distance = ParseDistance(match.Groups[1].Value);
            if (distance > 0)
            {
                tello.Move(action, Mathf.Clamp(distance, 20, 500)); // clamp between 20–500cm
                return true;
            }
        }
        return false;
    }

    private static bool TryParseRotation(string command, string pattern, string direction)
    {
        Match match = Regex.Match(command, pattern);
        if (match.Success)
        {
            int degrees = WordToNumber(match.Groups[1].Value);
            tello.Rotate(direction, Mathf.Clamp(degrees, 1, 360));
            return true;
        }
        return false;
    }

    private static bool TryParseSpeed(string command, string pattern)
    {
        Match match = Regex.Match(command, pattern);
        if (match.Success)
        {
            int speed = WordToNumber(match.Groups[1].Value);
            tello.SetSpeed(Mathf.Clamp(speed, 10, 100));
            return true;
        }
        return false;
    }

    // --- Distance Parser (supports meters, cm, feet) ---
    private static int ParseDistance(string input)
    {
        input = input.ToLower().Trim();

        if (input.EndsWith("meter") || input.EndsWith("meters"))
            return WordToNumber(input.Replace("meters", "").Replace("meter", "")) * 100;
        if (input.EndsWith("centimeter") || input.EndsWith("centimeters") || input.EndsWith("cm"))
            return WordToNumber(input.Replace("centimeters", "").Replace("centimeter", "").Replace("cm", ""));
        if (input.EndsWith("feet") || input.EndsWith("foot"))
            return WordToNumber(input.Replace("feet", "").Replace("foot", "")) * 30;

        return WordToNumber(input); // fallback
    }

    // --- Word-to-Number Parser ---
    private static int WordToNumber(string input)
    {
        input = input.Trim();
        if (int.TryParse(input, out int number)) return number;

        // Simple word-to-number dictionary
        switch (input)
        {
            case "one": return 1;
            case "two": return 2;
            case "three": return 3;
            case "four": return 4;
            case "five": return 5;
            case "six": return 6;
            case "seven": return 7;
            case "eight": return 8;
            case "nine": return 9;
            case "ten": return 10;
            case "twenty": return 20;
            case "thirty": return 30;
            case "forty": return 40;
            case "fifty": return 50;
            case "sixty": return 60;
            case "seventy": return 70;
            case "eighty": return 80;
            case "ninety": return 90;
            case "hundred": return 100;
        }

        // Try splitting compound numbers (e.g., "one hundred twenty")
        string[] parts = input.Split(' ');
        int total = 0;
        foreach (string part in parts)
        {
            total += WordToNumber(part);
        }

        return total > 0 ? total : 0;
    }
}

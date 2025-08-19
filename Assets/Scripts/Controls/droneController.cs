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


    }


    private static bool TryParseMovement(string command, string pattern, string action)
    {
        Match match = Regex.Match(command, pattern);
        if (match.Success && int.TryParse(match.Groups[1].Value, out int distance))
        {
            tello.


            tello.Move(action, Mathf.Clamp(distance, 20, 500));
            return true;
        }
        return false;
    }

    public void move(string sub_cmd)
	{
		string command = sub_cmd[0] + " " + sub_cmd[1];

		try
		{
			tello.SendCommand(command)

		} catch (Exception ex)
		{
            //invalid subcommand
        }
    }



    public void turn(string sub_cmd)
    {
		switch(sub_cmd[0])
		{
			case "left" {
					tello.SendCommand("ccw " + sub_cmd[1]);
                    break;
				}
			case "right" {
                    tello.SendCommand("cw " + sub_cmd[1]);
                    break;
				}
			default {
					break;
				}
		}
    }

    public void change(string sub_cmd)
	{
		try
		{
			var command = sub_cmd[0] + " " + sub_cmd[1];

			tello.SendCommand(command);
        } catch(Exception ex)
		{
			//
		}
	}
}

using System;
using tellocs;

public class droneController
{
	int moveSpeed = 20;
	int liftSpeed = 2;
	int rotationSpeed = 60;
	int takeOffHeight = 2;
	Tello tello;

    public droneController()
	{
		tello = new tello();

		tello.Connect();
	}

	public void execute_command(string cmd)
	{
		var keywords =

		switch (keywords[0])
		{
			case "move"{
                    move(keywords[1:])
                    break;
			}
			case "change"{
                    change(keywords[1:])
                    break;
				}
			case "turn"{
                    turn(keywords[1:])
                    break;
				}
			default {
                    command = ' '.join(keywords)

					switch (command)
					{
						case "take off"{
                                tello.TakeOff();
                                break;
							}
						case "land" {
                                tello.Land();
                                break;
							}
						case "stop"
                        {
							break;
						}
                        default {
                                break;
                            }
                    }
        }
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

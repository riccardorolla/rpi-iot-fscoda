using System;


namespace Rover {
	class MainClass
	{
		public static void Main(string[] args)
		{

			Configuration rover = Configuration.Read();
			if (args.Length <= 0) return;
			try
			{
				switch (args[0])
				{
					case "led":
						if (args.Length > 2)
						{
							int numled = 999;
							bool result = int.TryParse(args[1], out numled);
							if (result)
							{
								if (rover.led.Length > numled)
								{
									Led led = new Led(numled, rover.led[numled]);
									if ((args[2]) == "on") led.on();
									else led.off();
									Console.Write("OK");
									led.dispose();
								}
							}
						}
						break;
					case "button":
						if (args.Length > 1)
						{
							int numbtn = 999;
							bool result = int.TryParse(args[1], out numbtn);
							if (result)
							{
								if (rover.button.Length > numbtn)
								{
									Button btn = new Button(rover.button[numbtn]);
									Console.Write(btn.read());
									btn.dispose();
								}
							}
						}
						break;
					case "motor":
						TwoMotorsDriver motors = new TwoMotorsDriver(rover.motor);
						if (args.Length > 1)
						{
							switch (args[1])
							{
								case "left":
									motors.TurnLeft();
									break;
								case "right":
									motors.TurnRight();
									break;
								case "forward":
									motors.MoveForward();
									break;
								case "backward":
									motors.MoveBackward();
									break;
								case "stop":
									motors.Stop();
									break;
								default: { } break;

							}
							System.Threading.Thread.Sleep(
								TimeSpan.FromMilliseconds(1000 * rover.timestep));
							motors.Stop();
							Console.Write("OK");
							motors.dispose();
						}
						break;
					case "uds":
						UltrasonicDistanceSensor uds = new UltrasonicDistanceSensor(rover.uds);
						uds.getDistance();
						Console.Write(uds.getDistance());
						uds.Close();
						break;



				}
			}
				catch (Exception) {
				Console.Write("exception occur: the application only works on raspian with 'sudo' command");
				}
		}
		}
}
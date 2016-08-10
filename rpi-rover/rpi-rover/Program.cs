using System;
using Raspberry.IO.GeneralPurpose;
using Raspberry.IO.GeneralPurpose.Behaviors;

namespace Rover

{
	class MainClass
	{

		public static void Main(string[] args)
		{

			//driver motors, uds, led
			//time 
			//pins 
			Configuration rover = Configuration.Read();

			switch (args[0])
			{
				case "motor":
					{
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
								default:
									{ }
									break;
							}
						}
						Console.Write("OK");
					}
					break;
				case "uds":
					UltrasonicDistanceSensor uds = new UltrasonicDistanceSensor(rover.uds);
					Console.Write(uds.getCM());
					break;
			}

		}
	}
}
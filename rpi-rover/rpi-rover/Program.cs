using System;
using Raspberry.IO.GeneralPurpose;
using Raspberry.IO.GeneralPurpose.Behaviors;
using System.Threading;
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
						if (args.Length > 2)
						{
							int time = 1;
							bool result = int.TryParse(args[2], out time);
							switch (args[1])
							{
								
								case "left":
									motors.TurnLeft();
									Thread.Sleep(time * 1000);
									motors.Stop();
									break;
								case "right":
									motors.TurnRight();
									Thread.Sleep(time * 1000);
									motors.Stop();
									break;
								case "forward":
									motors.MoveForward();
									Thread.Sleep(time * 1000);
									motors.Stop();
									break;
								case "backward":
									motors.MoveBackward();
									Thread.Sleep(time * 1000);
									motors.Stop();
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
				case "led":
					if (args.Length > 2)
					{
						int numled = 999;
						bool result = int.TryParse(args[1],out numled);
						if (result) {
							if (rover.led.Length > numled)
							{
								Led led = new Led(numled,rover.led[numled]);
								if ((args[2]) == "on") led.on();
								else led.off();
							}
						}
					}
					break;
			}

		}
	}
}
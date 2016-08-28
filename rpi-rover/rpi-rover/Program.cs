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
			UltrasonicDistanceSensor uds = new UltrasonicDistanceSensor(rover.uds);
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
						//	System.Threading.Thread.Sleep(TimeSpan.FromMilliseconds(1000*rover.timestep));
					//		motors.Stop();
				//			Console.Write("OK");
						}

					}
					break;
				case "uds":
		 
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
								Console.Write("OK");
							}

						}
					}
					break;
			}

		}
	}
}
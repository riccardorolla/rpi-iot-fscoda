using System;
using Raspberry.IO.GeneralPurpose;
using Raspberry.IO.GeneralPurpose.Behaviors;
namespace Rover
 
{
	class MainClass
	{
		public static void Main (string[] args)
		{
			Console.WriteLine ("UltrasonicDistanceSensor");
			TwoMotorsDriver driver = new TwoMotorsDriver(new Motor(ConnectorPin.P1Pin13.ToProcessor(), ConnectorPin.P1Pin15.ToProcessor()), new Motor(ConnectorPin.P1Pin29.ToProcessor(), ConnectorPin.P1Pin31.ToProcessor()));
			UltrasonicDistanceSensor uds= new UltrasonicDistanceSensor (ConnectorPin.P1Pin16.ToProcessor(),ConnectorPin.P1Pin18.ToProcessor());
			double distance;
			while (true) {

				distance = uds.getCM ();
				if (distance < 50)
			            driver.TurnLeft  ();
				else 	driver.MoveForward ();
				//while(true)driver.MoveForward ();

			}
		}
	}
}

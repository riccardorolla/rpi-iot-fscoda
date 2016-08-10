using System;
using Raspberry.IO.GeneralPurpose;
using Raspberry.IO.GeneralPurpose.Behaviors;
namespace Rover

{
	class MainClass
	{
		
		public static void Main(string[] args)
		{

			TwoMotorsDriver motors = new TwoMotorsDriver(new Motor("13","15"), new Motor("29", "31"));
			UltrasonicDistanceSensor uds = new UltrasonicDistanceSensor("16", "18");


			System.Xml.Serialization.XmlSerializer writer =
				new System.Xml.Serialization.XmlSerializer(typeof(Motor));

			var path =   Environment.CurrentDirectory + "//MotorA.xml";
			System.IO.FileStream file = System.IO.File.Create(path);

			writer.Serialize(file, new Motor("13", "15"));
			file.Close();
			System.Xml.Serialization.XmlSerializer writer2 =
			new System.Xml.Serialization.XmlSerializer(typeof(UltrasonicDistanceSensor));

			var path2 = Environment.CurrentDirectory + "//UltrasonicDistanceSensor.xml";
			System.IO.FileStream file2 = System.IO.File.Create(path);

			writer.Serialize(file2, uds);
			file2.Close();
			/*	double distance;
				while (true) {

					distance = uds.getCM ();
					if (distance < 50)
							motors.TurnLeft  ();
					else 	motors.MoveForward ();
					 while(true)driver.MoveForward ();

				}
			}*/
		}
	}
}

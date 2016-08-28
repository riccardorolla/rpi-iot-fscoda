using System;
using System.Diagnostics;
 
 using System.Globalization;
using Raspberry.IO.GeneralPurpose;
using Raspberry.Timers;
using Raspberry.IO.Components.Sensors.Distance.HcSr04;
using UnitsNet;
 
namespace Rover
{
	public class UltrasonicDistanceSensor {
     
		ProcessorPin _gpioPinTrig;
        // ReSharper disable once PrivateFieldCanBeConvertedToLocalVariable
		ProcessorPin _gpioPinEcho;

        private readonly Stopwatch _stopwatch;
 
		public UltrasonicDistanceSensor(int[] pin)
		{
			_stopwatch = new Stopwatch();

			_gpioPinTrig = Utilities.getPin(pin[0]);
			_gpioPinEcho = Utilities.getPin(pin[1]);
			 
		}
		public UltrasonicDistanceSensor(int trigGpioPin, int echoGpioPin)
        {
 
            _stopwatch  = new Stopwatch();
	
			_gpioPinTrig = Utilities.getPin(trigGpioPin);
			_gpioPinEcho = Utilities.getPin(echoGpioPin);
			 
        }

		public double getCM() {
			var driver = GpioConnectionSettings.DefaultDriver;
			double dist = -1;
			int i = 50;
			using (var connection = new HcSr04Connection(
			  driver.Out(_gpioPinTrig),
			  driver.In(_gpioPinEcho)))


				while (dist < 0)
				{

					try
					{
						var distance = connection.GetDistance().Centimeters;

						//		Console.WriteLine(string.Format(CultureInfo.InvariantCulture, "{0:0.0}cm", distance).PadRight(16));
						//		Console.CursorTop--;
						dist = distance;
					}
					catch (TimeoutException )
					{
						i = i + 50;
						connection.Timeout = TimeSpan.FromMilliseconds(i);
						dist = 0;
						//		Console.WriteLine("(Timeout): " + e.Message);
					}

				}
			return dist;
			}

	 
		}


	 
    }
 
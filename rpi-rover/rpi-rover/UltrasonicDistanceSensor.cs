using System;
using System.Diagnostics;
using System.Threading.Tasks;
using System.Threading;
using Raspberry.IO.GeneralPurpose;
using Raspberry.IO.GeneralPurpose.Behaviors;
 
namespace Rover
{
	public class UltrasonicDistanceSensor {
     
		ProcessorPin _gpioPinTrig;
        // ReSharper disable once PrivateFieldCanBeConvertedToLocalVariable
		ProcessorPin _gpioPinEcho;
		IGpioConnectionDriver gpio = GpioConnectionSettings.DefaultDriver;
        private readonly Stopwatch _stopwatch;
 
		public UltrasonicDistanceSensor(int[] pin)
		{
			_stopwatch = new Stopwatch();

			_gpioPinTrig = Utilities.getPin(pin[0]);
			_gpioPinEcho = Utilities.getPin(pin[1]);
			gpio.Allocate(_gpioPinTrig, PinDirection.Output);
			gpio.Allocate(_gpioPinEcho, PinDirection.Input);
			gpio.Write(_gpioPinTrig, false);
			Thread.Sleep(30);
		}
		public UltrasonicDistanceSensor(int trigGpioPin, int echoGpioPin)
        {
 
            _stopwatch  = new Stopwatch();
	
			_gpioPinTrig = Utilities.getPin(trigGpioPin);
			_gpioPinEcho = Utilities.getPin(echoGpioPin);
			gpio.Allocate (_gpioPinTrig, PinDirection.Output);
			gpio.Allocate (_gpioPinEcho, PinDirection.Input);
			gpio.Write (_gpioPinTrig, false);
			Thread.Sleep (30);
        }

		public double getCM() {
			gpio.Write (_gpioPinTrig, true);
			Thread.Sleep(TimeSpan.FromMilliseconds(10));
			gpio.Write (_gpioPinTrig, false);
			//Console.WriteLine ("gpio.Read (_gpioPinEcho) == false)" );
			while (gpio.Read (_gpioPinEcho) == false);


			double starttime = (double) DateTime.Now.Ticks / (double)  TimeSpan.TicksPerMillisecond;
			//Console.WriteLine ("gpio.Read (_gpioPinEcho) == true)" );
			while (gpio.Read (_gpioPinEcho) == true);
			double traveltime = ((double) DateTime.Now.Ticks / (double) TimeSpan.TicksPerMillisecond) - starttime;
			double distance = traveltime* 34.3 / 2.0 ;
			return distance;
    }
}

}
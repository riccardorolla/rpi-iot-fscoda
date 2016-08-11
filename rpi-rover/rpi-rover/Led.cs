using Raspberry.IO.GeneralPurpose;
using System;
namespace Rover
{

	class Led
	{
		ProcessorPin ledGpioPin;
		IGpioConnectionDriver gpio;
		int name;
		public Led(int pin){
			name = pin;
			gpio = GpioConnectionSettings.DefaultDriver;
			ledGpioPin = Utilities.getPin(pin);
			gpio.Allocate(ledGpioPin, PinDirection.Output);
		}

		public void on(){
			gpio.Write(ledGpioPin, true);

			Console.Write("led " + name + " is on");
		}
		public void off()
		{
			gpio.Write(ledGpioPin, false);
			Console.Write("led " + name + " is off");
		 
		}
	}
}
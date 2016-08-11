using Raspberry.IO.GeneralPurpose;
using System;
namespace Rover
{

	class Led
	{
		ProcessorPin ledGpioPin;
		IGpioConnectionDriver gpio;
		int name;
		public Led(int name,int pin){
			this.name=name;
			gpio = GpioConnectionSettings.DefaultDriver;
			ledGpioPin = Utilities.getPin(pin);
			gpio.Allocate(ledGpioPin, PinDirection.Output);
		}

		public void on(){
			gpio.Write(ledGpioPin, false);
			gpio.Release(ledGpioPin);
			Console.Write("led " + name + " is on");
		}
		public void off()
		{
			gpio.Write(ledGpioPin, true);
			gpio.Release(ledGpioPin);
			Console.Write("led " + name + " is off");
		 
		}
	}
}
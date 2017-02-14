using System;
using Raspberry.IO.GeneralPurpose;
namespace Rover
{
	public class Button
	{   
		ProcessorPin buttonGpioPin;
		IGpioConnectionDriver gpio;
		//int pin;
		public Button(int pin)
		{
	 
			gpio = GpioConnectionSettings.DefaultDriver;
			buttonGpioPin = Utilities.getPin(pin);
			gpio.Allocate(buttonGpioPin, PinDirection.Input);
			gpio.SetPinResistor(buttonGpioPin, PinResistor.PullUp);
		}

		public int read()
		{
			return  (gpio.Read(buttonGpioPin)?0:1);
 
		}
		public void dispose()
		{
			gpio.Release(buttonGpioPin);
		}
	 
	}
}

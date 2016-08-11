using Raspberry.IO.GeneralPurpose;
namespace Rover
{

	class Led
	{
		ProcessorPin ledGpioPin;
		IGpioConnectionDriver gpio;
		public Led(int pin){
			gpio = GpioConnectionSettings.DefaultDriver;
			ledGpioPin = Utilities.getPin(pin);
			gpio.Allocate(ledGpioPin, PinDirection.Output);
		}

		public void on(){
			gpio.Write(ledGpioPin, true);
		}
		public void off()
		{
			gpio.Write(ledGpioPin, false);
		 
		}
	}
}
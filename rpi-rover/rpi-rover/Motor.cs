﻿using Raspberry.IO.GeneralPurpose;
using Raspberry.IO.GeneralPurpose.Behaviors;
using System.Xml.Serialization;
namespace Rover
{
    public class Motor
    {
		ProcessorPin _motorGpioPinA  ;
		ProcessorPin _motorGpioPinB  ;
		IGpioConnectionDriver gpio = GpioConnectionSettings.DefaultDriver;
		[XmlElement(DataType = "string")]
		public string pinA;
		[XmlElement(DataType = "string")]
		public string pinB;

		public Motor()
		{
		
		}
		public Motor(string pin1, string pin2)
        {
			pinA = pin1;
			pinB = pin2;
			_motorGpioPinA = Utilities.getPin(pin1);
			_motorGpioPinB = Utilities.getPin(pin2);	
			gpio.Allocate (_motorGpioPinA, PinDirection.Output);
			gpio.Allocate (_motorGpioPinB, PinDirection.Output);
			gpio.Write (_motorGpioPinA, false);
			gpio.Write (_motorGpioPinB, false);
         /*   _motorGpioPinA = gpio.OpenPin(gpioPinIn1);
            _motorGpioPinB = gpio.OpenPin(gpioPinIn2);
            _motorGpioPinA.Write(GpioPinValue.Low);
            _motorGpioPinB.Write(GpioPinValue.Low);
            _motorGpioPinA.SetDriveMode(GpioPinDriveMode.Output);
            _motorGpioPinB.SetDriveMode(GpioPinDriveMode.Output);*/
        }

        public void MoveForward()
        {
			gpio.Write (_motorGpioPinA, false);
			gpio.Write (_motorGpioPinB, true);
        /*    _motorGpioPinA.Write(GpioPinValue.Low);
            _motorGpioPinB.Write(GpioPinValue.High);*/
        }

        public void MoveBackward()
        {
			gpio.Write (_motorGpioPinA, true);
			gpio.Write (_motorGpioPinB, false);
          /*  _motorGpioPinA.Write(GpioPinValue.High);
            _motorGpioPinB.Write(GpioPinValue.Low);*/
        }

        public void Stop()
        {
			gpio.Write (_motorGpioPinA, false);
			gpio.Write (_motorGpioPinB, false);
		 
        }
    }
}

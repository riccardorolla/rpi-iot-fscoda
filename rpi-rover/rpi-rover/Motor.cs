using Raspberry.IO.GeneralPurpose;
namespace Rover {

    public class Motor {
        ProcessorPin _motorGpioPinA;
        ProcessorPin _motorGpioPinB;
        IGpioConnectionDriver gpio = GpioConnectionSettings.DefaultDriver;

        public Motor(int pin1, int pin2) {

            _motorGpioPinA = Utilities.getPin(pin1);
            _motorGpioPinB = Utilities.getPin(pin2);
            gpio.Allocate(_motorGpioPinA, PinDirection.Output);
            gpio.Allocate(_motorGpioPinB, PinDirection.Output);
            gpio.Write(_motorGpioPinA, false);
            gpio.Write(_motorGpioPinB, false);

        }

        public void MoveForward() {
            gpio.Write(_motorGpioPinA, false);
            gpio.Write(_motorGpioPinB, true);

        }

        public void MoveBackward() {
            gpio.Write(_motorGpioPinA, true);
            gpio.Write(_motorGpioPinB, false);
        }

        public void Stop() {
            gpio.Write(_motorGpioPinA, false);
            gpio.Write(_motorGpioPinB, false);

        }
        public void dispose() {
            gpio.Release(_motorGpioPinA);
            gpio.Release(_motorGpioPinB);
        }
    }
}

using System;
using Raspberry.Timers;
using Raspberry.IO;
using Raspberry.IO.GeneralPurpose;

namespace Rover {
    public class UltrasonicDistanceSensor {
        private IOutputBinaryPin triggerPin;
        private IInputBinaryPin echoPin;
        private TimeSpan triggerTime = TimeSpanUtility.FromMicroseconds(10);
        private TimeSpan echoUpTimeout = TimeSpan.FromMilliseconds(500);
        public static TimeSpan Timeout = TimeSpan.FromMilliseconds(50);
        private IGpioConnectionDriver driver = GpioConnectionSettings.DefaultDriver;

        public UltrasonicDistanceSensor(int[] pin) {
            triggerPin = driver.Out(Utilities.getPin(pin[0]));
            echoPin = driver.In(Utilities.getPin(pin[1]));
        }
        public void Close() {
            triggerPin.Dispose();
            echoPin.Dispose();
        }
        public double getDistance() {
            double dist = -1;
            do {
                try {
                    triggerPin.Write(true);
                    Timer.Sleep(triggerTime);
                    triggerPin.Write(false);
                    var upTime = echoPin.Time(true, echoUpTimeout, Timeout);
                    dist = ((upTime < TimeSpan.Zero) ? (double.MinValue) : ((upTime.TotalMilliseconds) / 1000 * 343.8 * 100) / 2.0);
                }
                catch (TimeoutException) { }
            } while (dist < 0);
            return dist;
        }
    }
}

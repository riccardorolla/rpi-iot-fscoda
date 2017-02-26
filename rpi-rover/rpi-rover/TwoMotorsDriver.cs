using System;
using System.Threading.Tasks;
namespace Rover {
    public class TwoMotorsDriver {
        private readonly Motor _leftMotor;
        private readonly Motor _rightMotor;
        public TwoMotorsDriver(int[] pin) {
            _leftMotor = new Motor(pin[0], pin[1]);
            _rightMotor = new Motor(pin[2], pin[3]);
        }
        public TwoMotorsDriver(Motor leftMotor, Motor rightMotor) {
            _leftMotor = leftMotor;
            _rightMotor = rightMotor;
        }
        public void Stop() {
            _leftMotor.Stop();
            _rightMotor.Stop();
        }
        public void MoveForward() {
            _leftMotor.MoveForward();
            _rightMotor.MoveForward();
        }
        public void MoveBackward() {
            _leftMotor.MoveBackward();
            _rightMotor.MoveBackward();
        }
        public void TurnRight() {
            _leftMotor.MoveForward();
            _rightMotor.MoveBackward();
        }
        public void TurnLeft() {
            _leftMotor.MoveBackward();
            _rightMotor.MoveForward();
        }
        public void dispose() {
            _leftMotor.dispose();
            _rightMotor.dispose();
        }
    }
}
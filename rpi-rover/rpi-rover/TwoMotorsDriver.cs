using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
 
namespace Rover
{
    public class TwoMotorsDriver
    {
 
        private readonly Motor _leftMotor;
		private int time;
        private readonly Motor _rightMotor;
		public TwoMotorsDriver (int[] pin,int time){
			_leftMotor = new Motor(pin[0], pin[1]);
			_rightMotor = new Motor(pin[2], pin[3]);
			this.time = time;
			  }
        public TwoMotorsDriver(Motor leftMotor, Motor rightMotor)
        {
            _leftMotor = leftMotor;
            _rightMotor = rightMotor;
        }

        public void Stop()
        {
            _leftMotor.Stop();
            _rightMotor.Stop();
        }

        public void MoveForward()
        {
            _leftMotor.MoveForward();
            _rightMotor.MoveForward();
        }

        public void MoveBackward()
        {
            _leftMotor.MoveBackward();
            _rightMotor.MoveBackward();

        }
		public void TurnRight()
		{
			_leftMotor.MoveForward();
			_rightMotor.MoveBackward();

			System.Threading.Thread.Sleep(TimeSpan.FromMilliseconds(time));


			_leftMotor.Stop();
			_rightMotor.Stop();
		}
        public async Task TurnRightAsync()
        {
            _leftMotor.MoveForward();
            _rightMotor.MoveBackward();

            await Task.Delay(TimeSpan.FromMilliseconds(time));

            _leftMotor.Stop();
            _rightMotor.Stop();
        }
		public void TurnLeft()
		{
			_leftMotor.MoveBackward();
			_rightMotor.MoveForward();

			System.Threading.Thread.Sleep(TimeSpan.FromMilliseconds(time));

			_leftMotor.Stop();
			_rightMotor.Stop();
		}
        public async Task TurnLeftAsync()
        {
            _leftMotor.MoveBackward();
            _rightMotor.MoveForward();

			await Task.Delay(TimeSpan.FromMilliseconds(time));

            _leftMotor.Stop();
            _rightMotor.Stop();
        }
    }
}
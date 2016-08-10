using System;
using Raspberry.IO.GeneralPurpose;
using Raspberry.IO.GeneralPurpose.Behaviors;
namespace Rover
{
	public class Utilities
	{
		public static ProcessorPin getPin(int ipin)
		{
			ProcessorPin pin;
			switch (ipin)
			{
				case 3:
					pin = ConnectorPin.P1Pin3.ToProcessor();
					break;
				case 5:
					pin = ConnectorPin.P1Pin5.ToProcessor();
					break;
				case 7:
					pin = ConnectorPin.P1Pin7.ToProcessor();
					break;
				case 8:
					pin = ConnectorPin.P1Pin8.ToProcessor();
					break;
				case 10:
					pin = ConnectorPin.P1Pin10.ToProcessor();
					break;
				case 11:
					pin = ConnectorPin.P1Pin11.ToProcessor();
					break;
				case 12:
					pin = ConnectorPin.P1Pin12.ToProcessor();
					break;
				case 13:
					pin = ConnectorPin.P1Pin13.ToProcessor();
					break;
				case 15:
					pin = ConnectorPin.P1Pin15.ToProcessor();
					break;
				case 16:
					pin = ConnectorPin.P1Pin16.ToProcessor();
					break;
				case 18:
					pin = ConnectorPin.P1Pin18.ToProcessor();
					break;
				case 19:
					pin = ConnectorPin.P1Pin19.ToProcessor();
					break;
				case 21:
					pin = ConnectorPin.P1Pin21.ToProcessor();
					break;
				case 22:
					pin = ConnectorPin.P1Pin22.ToProcessor();
					break;
				case 23:
					pin = ConnectorPin.P1Pin23.ToProcessor();
					break;

				case 24:
					pin = ConnectorPin.P1Pin24.ToProcessor();
					break;
				case 26:
					pin = ConnectorPin.P1Pin26.ToProcessor();
					break;
				case 27:
					pin = ConnectorPin.P1Pin27.ToProcessor();
					break;
				case 28:
					pin = ConnectorPin.P1Pin28.ToProcessor();
					break;
				case 29:
					pin = ConnectorPin.P1Pin29.ToProcessor();
					break;
				case 31:
					pin = ConnectorPin.P1Pin31.ToProcessor();
					break;
				case 32:
					pin = ConnectorPin.P1Pin32.ToProcessor();
					break;
				case 33:
					pin = ConnectorPin.P1Pin33.ToProcessor();
					break;
				case 35:
					pin = ConnectorPin.P1Pin35.ToProcessor();
					break;
				case 36:
					pin = ConnectorPin.P1Pin36.ToProcessor();
					break;
				case 37:
					pin = ConnectorPin.P1Pin37.ToProcessor();
					break;
				case 38:
					pin = ConnectorPin.P1Pin38.ToProcessor();
					break;
				case 40:
					pin = ConnectorPin.P1Pin40.ToProcessor();
					break;
				default:
					pin = ConnectorPin.P1Pin3.ToProcessor();
					break;
			}

			return pin;

		}
	}
}


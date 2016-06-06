using System;
using RaspberryCam;
using RaspberryCam.Servers;
namespace cameratest
{
	class MainClass
	{
		public static void Main (string[] args)
		{
			var cameras = Cameras.DeclareDevice()
				.Named("Camera 1").WithDevicePath("/dev/video0")
				.Memorize();

			cameras.Get("Camera 1").SavePicture(new PictureSize(640, 480), "Test1.jpg", 20);

			//Or

			cameras.Default.SavePicture(new PictureSize(640, 480), "Test2.jpg", 20);
			var videoServer = new TcpVideoServer(8080, cameras);  
			Console.WriteLine("Server strating ...");  
			videoServer.Start();  
			Console.WriteLine("Server strated."); 
		}
	}
}

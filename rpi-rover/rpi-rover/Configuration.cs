using System;
using System.IO;
using Newtonsoft.Json;
namespace Rover
{
	public class Configuration
	{
		public int[] motor;
		public int[] led;
		public int[] uds;
		public Configuration()
		{

		
		}

		public static Configuration Read()
		{
			
			string line;
			string json = "";
			// Read the file and display it line by line.
			try
			{
				StreamReader file =
				   new StreamReader("rover.json");
				while ((line = file.ReadLine()) != null)
				{
					json = json + line;

				}

				file.Close();
			}
			catch (FileNotFoundException)
			{
				json = "{\"motor\":[13,15,29,31],\"led\":[11],\"uds\":[16,18]}";
				Configuration.Write(json);
			}

			Configuration conf = JsonConvert.DeserializeObject<Configuration>(json);
			return conf;
		}
		public static void Write(string json)
		{
			Console.Write(json);
			System.IO.StreamWriter file = new System.IO.StreamWriter("rover.json");
			file.WriteLine(json);

			file.Close();

		}
	
	} 

}


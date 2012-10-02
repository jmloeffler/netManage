using System;

namespace netManageLoadTester
{
	class MainClass
	{
		public static void Main (string[] args)
		{
			Console.WriteLine ("Waiting 5 seconds before starting...");
			System.Threading.Thread.Sleep(5000);
			
			Console.WriteLine ("Beginning load test...");
			var client = new System.Net.WebClient();
			
			var sleepPeriod = new Random(DateTime.Now.Second);
			
			while(true)
			{
				client.DownloadString("http://127.0.0.1:13000");
				System.Threading.Thread.Sleep (sleepPeriod.Next (1, 300));
			}
		}
	}
}

using System;

namespace netManage
{
	class MainClass
	{
		public static void Main (string[] args)
		{
			string serviceUri = "http://127.0.0.1:12000/";
			string managerUri = "http://1ser0.0.1:11000/";
			var svc = new ManagedService(serviceUri, managerUri);
			
			svc.Start();
			
			Console.WriteLine ("Press enter to stop the service");
			Console.ReadLine ();
		}
	}
}

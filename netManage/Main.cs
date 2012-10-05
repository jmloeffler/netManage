using System;
using Castle.Windsor;
using Castle.MicroKernel.Registration;
using Castle.DynamicProxy;
using Castle.MicroKernel.Lifestyle.Pool;

namespace netManage
{
	class MainClass
	{
		public static void Main (string[] args)
		{
			var container = new WindsorContainer();
			container.Register (
				Component.For<ManagerService>()
					.LifeStyle.Singleton);
			
			container.Register (
				Component.For<UnmanagedService>()
					.Interceptors<ManagerService>());
			
			var managerService = container.Resolve<ManagerService>();
			managerService.Start ();
			
			var unmanagedSvc = container.Resolve<UnmanagedService>();
			unmanagedSvc.Start ();
			
			Console.WriteLine ("Press enter to stop the service");
			Console.ReadLine ();
			
			managerService.Stop ();
			unmanagedSvc.Stop ();
			
			container.Dispose ();
		}
	}
}

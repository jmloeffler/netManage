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
				Component.For<ServiceManagerInterceptor>().LifeStyle.Singleton);
			
			container.Register (
				Component.For<IHttpRequestHandler>()
					.ImplementedBy<HttpRequestHandler>()
					.Interceptors<ServiceManagerInterceptor>()
				);
			
			var managerService = new ManagerService(container);
			managerService.Start ();
			
			var unmanagedSvc = new UnmanagedService(container.Resolve<IHttpRequestHandler>());
			unmanagedSvc.Start ();
			
			Console.WriteLine ("Press enter to stop the service");
			Console.ReadLine ();
			
			managerService.Stop ();
			unmanagedSvc.Stop ();
			
			container.Dispose ();
		}
	}
}

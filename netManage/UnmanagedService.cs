using System;
using System.Net;

namespace netManage
{
	public class UnmanagedService
	{
		private readonly HttpListener workerWebListener;
		private readonly IHttpRequestHandler _handler;
		
		public UnmanagedService (IHttpRequestHandler handler)
		{
			_handler = handler;
			
			workerWebListener = new HttpListener();
			workerWebListener.Prefixes.Add("http://127.0.0.1:13000/");
		}
		
		public void Start()
		{
			workerWebListener.Start();
			workerWebListener.BeginGetContext (new AsyncCallback(_handler.HandleRequest), 
			                                  new ServiceState { 
												Handler = _handler, 
												Listener = workerWebListener});
		}
		
		
		public void Stop()
		{
			workerWebListener.Stop ();
		}
		
		
	}
}


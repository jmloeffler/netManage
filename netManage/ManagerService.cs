using System;
using System.Net;
using Castle.Windsor;

namespace netManage
{
	public class ManagerService
	{
		private readonly WindsorContainer _container;
		
		public ManagerService (WindsorContainer container)
		{
			_container = container;
			
			managementWebListener = new HttpListener();
			managementWebListener.Prefixes.Add("http://127.0.0.1:14000/");
		}
		
		private readonly HttpListener managementWebListener;
		public void Start()
		{
			managementWebListener.Start ();
			managementWebListener.BeginGetContext (new AsyncCallback(HandleRequest), managementWebListener);
		}
		
		
		public void Stop()
		{
			managementWebListener.Stop ();
		}
		
		public virtual void HandleRequest(IAsyncResult result)
		{
			HttpListener listener = (HttpListener) result.AsyncState;
		    // Call EndGetContext to complete the asynchronous operation.
		    HttpListenerContext context = listener.EndGetContext(result);
			listener.BeginGetContext (new AsyncCallback(HandleRequest), listener);
		    //HttpListenerRequest request = context.Request;
		    // Obtain a response object.
		    HttpListenerResponse response = context.Response;
		    // Construct a response. 
			var interceptor = _container.Resolve<ServiceManagerInterceptor>();
			var requests = interceptor.RequestsPerSecond;
		    string responseString = string.Format ("<HTML><BODY> Requests per second: {0}</BODY></HTML>", requests);
		    byte[] buffer = System.Text.Encoding.UTF8.GetBytes(responseString);
		    // Get a response stream and write the response to it.
		    response.ContentLength64 = buffer.Length;
		    System.IO.Stream output = response.OutputStream;
		    output.Write(buffer,0,buffer.Length);
		    // You must close the output stream.
		    output.Close();
		}
		
		
	}
}

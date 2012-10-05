using System;
using System.Net;
using System.Threading;
using Castle.DynamicProxy;
using Castle.Windsor;

namespace netManage
{
	public class ManagerService : IInterceptor
	{
		private readonly ServiceStatistics _statistics;
		
		public ManagerService ()
		{
			_statistics = new ServiceStatistics();
			
			managementWebListener = new HttpListener();
			managementWebListener.Prefixes.Add("http://127.0.0.1:14000/");
			statisticsRecalculationTimer = new Timer(RecalculateStatistics, null, 0, 1000);
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
		    // Call EndGetContext to complete the asynchronous operation.
			HttpListener listener = (HttpListener) result.AsyncState;
		    HttpListenerContext context = listener.EndGetContext(result);
			listener.BeginGetContext (new AsyncCallback(HandleRequest), listener);
		    
		    // Construct a response. 
			HttpListenerResponse response = context.Response;
			var requests = _statistics.RequestsPerSecond;
		    string responseString = string.Format ("<HTML><BODY> Requests per second: {0}</BODY></HTML>", requests);
		    byte[] buffer = System.Text.Encoding.UTF8.GetBytes(responseString);
		    
			// Get a response stream and write the response to it.
		    response.ContentLength64 = buffer.Length;
		    System.IO.Stream output = response.OutputStream;
		    output.Write(buffer,0,buffer.Length);
		    output.Close();
		}
		
		#region IInterceptor implementation
		public void Intercept (IInvocation invocation)
		{
			invocation.Proceed ();
			
			if(invocation.Method.Name == "HandleRequest")
			{
				Interlocked.Increment (ref currentNumberOfRequests);
			}
		}
		#endregion
		
		private readonly Timer statisticsRecalculationTimer;
		private long currentNumberOfRequests;
		
		private void RecalculateStatistics(object state)
		{
			_statistics.RequestsPerSecond = Interlocked.Read (ref currentNumberOfRequests);
			Interlocked.Exchange (ref currentNumberOfRequests, 0);
		}
	}
}

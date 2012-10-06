using System;
using System.Collections.Generic;
using System.Net;
using System.Threading;
using Castle.DynamicProxy;
using Castle.Windsor;

namespace netManage
{
	public class ManagerService : IInterceptor
	{
		const int statisticsTimerInterval = 10000;
		
		private readonly Dictionary<string, ServiceStatistics> _statistics;
		
		public ManagerService ()
		{
			_statistics = new Dictionary<string, ServiceStatistics>();
			
			managementWebListener = new HttpListener();
			managementWebListener.Prefixes.Add("http://127.0.0.1:14000/");
			statisticsRecalculationTimer = new Timer(RecalculateStatistics, null, 0, statisticsTimerInterval);
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

		string FormatResponse ()
		{
			var response = new System.Text.StringBuilder();
			response.Append ("<HTML><BODY>");
			foreach(var stat in _statistics)
			{
				response.AppendFormat ("{0}: {1} rps", stat.Key, stat.Value.RequestsPerSecond);
				response.AppendLine ();
			}
			response.Append ("</BODY></HTML>");
			return response.ToString ();
		}
		
		public virtual void HandleRequest(IAsyncResult result)
		{
		    // Call EndGetContext to complete the asynchronous operation.
			HttpListener listener = (HttpListener) result.AsyncState;
		    HttpListenerContext context = listener.EndGetContext(result);
			listener.BeginGetContext (new AsyncCallback(HandleRequest), listener);
		    
		    // Construct a response. 
			HttpListenerResponse response = context.Response;
			string responseString = FormatResponse ();
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
				lock (_statistics) {
					if(!_statistics.ContainsKey (invocation.TargetType.FullName))
					   _statistics.Add (invocation.TargetType.FullName, new ServiceStatistics());
					
					_statistics[invocation.TargetType.FullName].Increment();
				}
			}
		}
		#endregion
		
		private readonly Timer statisticsRecalculationTimer;
		
		private void RecalculateStatistics(object state)
		{
			lock (_statistics) {
				foreach(var stat in _statistics)
				{
					stat.Value.Latch (statisticsTimerInterval / 1000);
				}
			}
		}
	}
}

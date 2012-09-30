using System;
using System.Threading;
using System.IO.Ports;
using System.Net;

namespace netManage
{
	public class ManagedService
	{
		private readonly HttpListener managementWebListener;
		private readonly HttpListener workerWebListener;
		
		private Timer statisticsRecalculationTimer;
		private long currentNumberOfRequests;
		private long requestsPerSecond;
		
		public ManagedService (string serviceUri, string managerUri)
		{
			managementWebListener = new HttpListener();
			workerWebListener = new HttpListener();
			managementWebListener.Prefixes.Add(managerUri);
			workerWebListener.Prefixes.Add(serviceUri);
			statisticsRecalculationTimer = new Timer(RecalculateStatistics, null, 0, 1000);
		}
		
		public void Start()
		{
			workerWebListener.Start();
			managementWebListener.Start ();
			workerWebListener.BeginGetContext (new AsyncCallback(WorkerCallback), workerWebListener);
			managementWebListener.BeginGetContext (new AsyncCallback(ManagerCallback), managementWebListener);
		}
		
		
		public void Stop()
		{
			managementWebListener.Stop ();
			workerWebListener.Stop ();
		}
		
		public virtual void RecalculateStatistics(object state)
		{
			Interlocked.Exchange (ref requestsPerSecond, currentNumberOfRequests);
			Interlocked.Exchange (ref currentNumberOfRequests, 0);
		}
		
		public virtual void ManagerCallback(IAsyncResult result)
		{
			HttpListener listener = (HttpListener) result.AsyncState;
		    // Call EndGetContext to complete the asynchronous operation.
		    HttpListenerContext context = listener.EndGetContext(result);
			listener.BeginGetContext (new AsyncCallback(ManagerCallback), listener);
		    //HttpListenerRequest request = context.Request;
		    // Obtain a response object.
		    HttpListenerResponse response = context.Response;
		    // Construct a response. 
			var requests = Interlocked.Read (ref requestsPerSecond);
		    string responseString = string.Format ("<HTML><BODY> Requests per second: {0}</BODY></HTML>", requests);
		    byte[] buffer = System.Text.Encoding.UTF8.GetBytes(responseString);
		    // Get a response stream and write the response to it.
		    response.ContentLength64 = buffer.Length;
		    System.IO.Stream output = response.OutputStream;
		    output.Write(buffer,0,buffer.Length);
		    // You must close the output stream.
		    output.Close();
		}
		
		public virtual void WorkerCallback(IAsyncResult result)
		{
			Interlocked.Increment (ref currentNumberOfRequests);
			
		    HttpListener listener = (HttpListener) result.AsyncState;
		    // Call EndGetContext to complete the asynchronous operation.
		    HttpListenerContext context = listener.EndGetContext(result);
			listener.BeginGetContext (new AsyncCallback(WorkerCallback), listener);
		    //HttpListenerRequest request = context.Request;
		    // Obtain a response object.
		    HttpListenerResponse response = context.Response;
		    // Construct a response. 
		    string responseString = "<HTML><BODY> Hello world!</BODY></HTML>";
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


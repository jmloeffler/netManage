using System;
using System.Net;

namespace netManage
{
	public class UnmanagedService
	{
		private readonly HttpListener workerWebListener;
		
		public UnmanagedService ()
		{
			workerWebListener = new HttpListener();
			workerWebListener.Prefixes.Add("http://127.0.0.1:13000/");
		}
		
		public void Start()
		{
			workerWebListener.Start();
			workerWebListener.BeginGetContext (new AsyncCallback(HandleRequest), 
			                                  workerWebListener);
		}
		
		
		public void Stop()
		{
			workerWebListener.Stop ();
		}
		
		public virtual void HandleRequest(IAsyncResult result)
		{
			// Call EndGetContext to complete the asynchronous operation.
			HttpListener listener = (HttpListener) result.AsyncState;
		    HttpListenerContext context = listener.EndGetContext(result);
			listener.BeginGetContext (new AsyncCallback(HandleRequest), listener);
		    
		    // Construct a response. 
			HttpListenerResponse response = context.Response;
		    string responseString = "<HTML><BODY> Hello world!</BODY></HTML>";
		    byte[] buffer = System.Text.Encoding.UTF8.GetBytes(responseString);
		    
			// Get a response stream and write the response to it.
		    response.ContentLength64 = buffer.Length;
		    System.IO.Stream output = response.OutputStream;
		    output.Write(buffer,0,buffer.Length);
		    output.Close();
		}
	}
}


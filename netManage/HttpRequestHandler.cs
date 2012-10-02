using System;
using System.Net;

namespace netManage
{
	public interface IHttpRequestHandler
	{
		void HandleRequest(IAsyncResult result);
	}
	
	public class HttpRequestHandler : IHttpRequestHandler
	{
		public virtual void HandleRequest(IAsyncResult result)
		{
		    ServiceState state = (ServiceState) result.AsyncState;
			
		    // Call EndGetContext to complete the asynchronous operation.
		    HttpListenerContext context = state.Listener.EndGetContext(result);
			state.Listener.BeginGetContext (new AsyncCallback(state.Handler.HandleRequest), 
			                               new ServiceState { Handler = state.Handler, Listener = state.Listener});
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


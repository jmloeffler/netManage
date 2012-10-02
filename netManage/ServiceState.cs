using System.Net;

namespace netManage
{
	public class ServiceState
	{
		public HttpListener Listener { get; set; }
		public IHttpRequestHandler Handler { get; set; }
	}
}
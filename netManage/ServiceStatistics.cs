using System;
using System.Threading;

namespace netManage
{
	public class ServiceStatistics
	{
		private long _requestsPerSecond;
		public long RequestsPerSecond
		{
			get { return Interlocked.Read(ref _requestsPerSecond); }
			set { Interlocked.Exchange(ref _requestsPerSecond, value); }
		}
	}
}


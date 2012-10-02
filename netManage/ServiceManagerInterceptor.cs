using System;
using System.Net;
using System.Threading;
using Castle.DynamicProxy;
using Castle.Windsor;

namespace netManage
{
	public class ServiceManagerInterceptor : IInterceptor
	{
		public ServiceManagerInterceptor ()
		{
			statisticsRecalculationTimer = new Timer(RecalculateStatistics, null, 0, 1000);
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
		private long requestsPerSecond;
		
		public long RequestsPerSecond
		{
			get { return Interlocked.Read (ref requestsPerSecond); }
		}
		
		private void RecalculateStatistics(object state)
		{
			Interlocked.Exchange (ref requestsPerSecond, currentNumberOfRequests);
			Interlocked.Exchange (ref currentNumberOfRequests, 0);
		}
		
	}
}


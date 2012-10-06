using System;
using System.Threading;

namespace netManage
{
	public class ServiceStatistics
	{
		private long counter;
		public long RequestsPerSecond { get; private set; }
		
		public void Increment()
		{
			counter++;
		}
		
		public void Latch(long elapsedSeconds)
		{
			RequestsPerSecond = counter / elapsedSeconds;
		}
	}
}


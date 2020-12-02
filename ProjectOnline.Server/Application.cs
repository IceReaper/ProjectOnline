namespace ProjectOnline.Server
{
	using System;
	using System.Threading;

	public class Application : IDisposable
	{
		private bool isDisposed;

		public void Run()
		{
			while (!this.isDisposed)
			{
				Thread.Sleep(1000);
			}
		}

		public void Dispose()
		{
			this.isDisposed = true;
			GC.SuppressFinalize(this);
		}
	}
}

namespace ProjectOnline.Shared.Utils
{
	using System;
	using System.Threading;

	public abstract class ThreadedProcess
	{
		private Thread? thread;
		private DateTime lastUpdate;

		protected abstract int UpdateRate { get; }

		public void Start()
		{
			if (this.thread != null)
				throw new Exception("Process already started!");

			this.FirstUpdate();
			this.lastUpdate = DateTime.Now;

			this.thread = new Thread(
				() =>
				{
					while (this.thread != null && this.thread.IsAlive)
					{
						while (this.lastUpdate + TimeSpan.FromSeconds(1.0 / this.UpdateRate) <= DateTime.Now)
							this.Update();

						Thread.Sleep(DateTime.Now - this.lastUpdate);
					}

					this.LastUpdate();
				}
			);

			this.thread.Start();
		}

		protected virtual void FirstUpdate()
		{
		}

		protected abstract void Update();

		protected virtual void LastUpdate()
		{
		}

		public void Stop()
		{
			this.thread?.Join();
			this.thread = null;
		}
	}
}

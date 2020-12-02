namespace ProjectOnline.Server
{
	using Networking;
	using Shared.Networking.Packets;
	using System;
	using System.Threading;

	public class Application : IDisposable
	{
		private bool isDisposed;

		public void Run()
		{
			while (!this.isDisposed)
			{
				var server = new NetworkServer();
				server.Start();

				while (!Console.KeyAvailable)
				{
					foreach (var sender in server.Connections)
					{
						foreach (var packet in sender.Receive())
						{
							if (packet is not ChatPacket chatPacket)
								continue;

							foreach (var receiver in server.Connections)
								receiver.Send(chatPacket);
						}

						if (!sender.IsConnected)
							sender.Dispose();
					}

					Thread.Sleep(15);
				}

				server.Stop();
			}
		}

		public void Dispose()
		{
			this.isDisposed = true;
			GC.SuppressFinalize(this);
		}
	}
}

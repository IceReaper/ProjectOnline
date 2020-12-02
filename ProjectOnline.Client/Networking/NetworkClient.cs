namespace ProjectOnline.Client.Networking
{
	using LiteNetLib;
	using Shared.Networking;
	using System;
	using System.Threading;

	public class NetworkClient
	{
		private readonly NetManager client;

		private Thread? thread;
		private Connection? connection;

		public NetworkClient()
		{
			var listener = new EventBasedNetListener();

			listener.NetworkReceiveEvent += (_, netReader, _) => this.connection?.Receive(netReader.GetRemainingBytes());

			this.client = new NetManager(listener);
		}

		public Connection Connect(string hostname)
		{
			if (this.client.IsRunning)
				throw new Exception("NetworkClient already started!");

			this.client.Start();

			var peer = this.client.Connect(hostname, NetworkSettings.Port, NetworkSettings.HandshakeKey);
			this.connection = new Connection(peer);

			this.thread = new Thread(
				() =>
				{
					while (peer.ConnectionState != ConnectionState.Disconnected)
					{
						this.client.PollEvents();
						Thread.Sleep(NetworkSettings.PollTimeout);
					}

					this.client.Stop();
				}
			);

			this.thread.Start();

			return this.connection;
		}
	}
}

namespace ProjectOnline.Server.Networking
{
	using LiteNetLib;
	using Shared.Networking;
	using System;
	using System.Collections.Generic;
	using System.Threading;

	public class NetworkServer
	{
		private readonly NetManager server;
		private readonly Dictionary<NetPeer, Connection> connections = new();

		private Thread? thread;

		public IEnumerable<Connection> Connections => this.connections.Values;

		public NetworkServer()
		{
			var listener = new EventBasedNetListener();

			listener.ConnectionRequestEvent += request => request.AcceptIfKey(NetworkSettings.HandshakeKey);
			listener.PeerConnectedEvent += peer => this.connections.Add(peer, new Connection(peer, () => this.connections.Remove(peer)));
			listener.NetworkReceiveEvent += (peer, netReader, _) => this.connections[peer].Receive(netReader.GetRemainingBytes());

			this.server = new NetManager(listener);
		}

		public void Start()
		{
			if (this.server.IsRunning)
				throw new Exception("NetworkServer already started!");

			this.server.Start(NetworkSettings.Port);

			this.thread = new Thread(
				() =>
				{
					while (this.thread != null && this.thread.IsAlive)
					{
						this.server.PollEvents();
						Thread.Sleep(NetworkSettings.PollTimeout);
					}
				}
			);

			this.thread.Start();
		}

		public void Stop()
		{
			this.thread?.Join();
			this.server.Stop();
		}
	}
}

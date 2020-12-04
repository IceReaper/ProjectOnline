namespace ProjectOnline.Shared.Networking
{
	using LiteNetLib;
	using System.Collections.Generic;
	using Utils;

	public class NetworkServer : ThreadedProcess
	{
		private readonly NetManager server;
		private readonly Dictionary<NetPeer, Connection> connections = new();

		protected override int UpdateRate => 10;

		public NetworkServer(IConnectionHandler connectionHandler)
		{
			var listener = new EventBasedNetListener();

			listener.ConnectionRequestEvent += request => request.AcceptIfKey(NetworkSettings.HandshakeKey);

			listener.PeerConnectedEvent += peer =>
			{
				var connection = new Connection(peer);
				this.connections.Add(peer, connection);
				connectionHandler.Add(connection);
			};

			listener.NetworkReceiveEvent += (peer, netReader, _) => this.connections[peer].Receive(netReader.GetRemainingBytes());

			listener.PeerDisconnectedEvent += (peer, _) => this.connections.Remove(peer);

			this.server = new NetManager(listener);
		}

		protected override void FirstUpdate()
		{
			this.server.Start(NetworkSettings.Port);
		}

		protected override void Update()
		{
			this.server.PollEvents();
		}

		protected override void LastUpdate()
		{
			this.server.Stop();
			this.connections.Clear();
		}
	}
}

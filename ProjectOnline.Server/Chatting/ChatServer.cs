namespace ProjectOnline.Server.Chatting
{
	using Authentication;
	using Shared.Networking;
	using Shared.Networking.Packets;
	using Shared.Utils;
	using System.Collections.Generic;
	using System.Linq;

	public class ChatServer : ThreadedProcess, IConnectionHandler
	{
		private readonly List<Connection> connections = new();

		protected override int UpdateRate => 10;

		public void Add(Connection newConnection)
		{
			this.connections.Add(newConnection);

			foreach (var connection in this.connections)
				connection.Send(new ChatPacket {Message = $"Client {newConnection.Properties.OfType<Account>().First().Username} connected."});
		}

		protected override void Update()
		{
			foreach (var sender in this.connections.ToArray())
			{
				IPacket packet;

				while ((packet = sender.Receive()) != null)
				{
					if (packet is ChatPacket chatPacket)
					{
						var response = new ChatPacket {Message = $"{sender.Properties.OfType<Account>().First().Username}: {chatPacket.Message}"};

						foreach (var receiver in this.connections)
							receiver.Send(response);
					}
				}

				if (sender.IsConnected)
					continue;

				sender.Dispose();
				this.connections.Remove(sender);

				foreach (var receiver in this.connections)
					receiver.Send(new ChatPacket {Message = $"Client {sender.Properties.OfType<Account>().First().Username} disconnected."});
			}
		}
	}
}

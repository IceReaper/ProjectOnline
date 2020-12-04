namespace ProjectOnline.Server.Authentication
{
	using Shared.Networking;
	using Shared.Utils;
	using System.Collections.Generic;

	public class AuthenticationServer : ThreadedProcess, IConnectionHandler
	{
		private readonly List<Connection> connections = new();
		private readonly IConnectionHandler authenticatedConnectionHandler;

		public AuthenticationServer(IConnectionHandler authenticatedConnectionHandler)
		{
			this.authenticatedConnectionHandler = authenticatedConnectionHandler;
		}

		protected override int UpdateRate => 10;

		public void Add(Connection connection)
		{
			this.connections.Add(connection);
		}

		protected override void Update()
		{
			foreach (var connection in this.connections.ToArray())
			{
				if (!connection.IsConnected)
				{
					connection.Dispose();
					this.connections.Remove(connection);

					continue;
				}

				// TODO implement login. For now we assume the authentication was successful.

				connection.Properties.Add(new Account("DUDE"));
				this.connections.Remove(connection);
				this.authenticatedConnectionHandler.Add(connection);
			}
		}
	}
}

namespace ProjectOnline.Server.Authentication
{
	using SecureRemotePassword;
	using Shared.Networking;
	using Shared.Networking.Packets.Authentication;
	using Shared.Utils;
	using System.Collections.Generic;
	using System.Linq;
	using System.Text;

	public class AuthenticationServer : ThreadedProcess, IConnectionHandler
	{
		private readonly List<Account> accounts = new();
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

				IPacket? packet;

				while ((packet = connection.Receive()) != null)
				{
					switch (packet)
					{
						case RegisterRequestPacket registerRequestPacket:
						{
							if (this.accounts.All(account => account.Username != registerRequestPacket.Username))
							{
								this.accounts.Add(new Account(registerRequestPacket.Username, registerRequestPacket.Salt, registerRequestPacket.Verifier));
								connection.Send(new RegisterResponsePacket {Success = true});
							}
							else
								connection.Send(new RegisterResponsePacket {Success = false});

							break;
						}

						case PrepareLoginRequestPacket prepareLoginRequestPacket:
						{
							var account = this.accounts.FirstOrDefault(a => a.Username == prepareLoginRequestPacket.Username);

							if (account == null)
							{
								connection.Send(new PrepareLoginResponsePacket {Salt = SrpInteger.RandomInteger(32), Ephemeral = SrpInteger.RandomInteger(32)});

								break;
							}

							var session = new Session(account);
							var ephemeral = session.PrepareLogin();

							connection.Properties.RemoveAll(property => property is Session);
							connection.Properties.Add(session);
							connection.Send(new PrepareLoginResponsePacket {Salt = account.Salt, Ephemeral = ephemeral});

							break;
						}

						case LoginRequestPacket loginRequestPacket:
						{
							var session = connection.Properties.OfType<Session>().FirstOrDefault();
							var proof = session?.Login(loginRequestPacket.Ephemeral, loginRequestPacket.Proof);

							if (proof == null)
							{
								connection.Send(new LoginResponsePacket {Proof = SrpInteger.RandomInteger(32)});

								break;
							}

							connection.Send(new LoginResponsePacket {Proof = proof});

							break;
						}

						case KeyChangedPacket:
						{
							var session = connection.Properties.OfType<Session>().FirstOrDefault();

							if (session?.EncryptionKey == null)
								break;

							connection.EncryptionKey = Encoding.ASCII.GetBytes(session.EncryptionKey);

							this.connections.Remove(connection);
							this.authenticatedConnectionHandler.Add(connection);

							break;
						}
					}
				}
			}
		}
	}
}

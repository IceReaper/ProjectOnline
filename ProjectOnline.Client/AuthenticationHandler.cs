namespace ProjectOnline.Client
{
	using SecureRemotePassword;
	using Shared.Networking;
	using Shared.Networking.Packets.Authentication;
	using System;
	using System.Text;
	using System.Threading;

	public class AuthenticationHandler : IHandler
	{
		public IHandler Handle(Connection connection)
		{
			Console.WriteLine("To login type 'login <username> <password>'.");
			Console.WriteLine("To register type 'register <username> <password>'.");
			Console.WriteLine("To quit type 'quit'.");

			var line = Console.ReadLine();

			if (line == null)
				return this;

			var chunks = line.Split(' ');

			switch (chunks[0])
			{
				case "register" when chunks.Length >= 3:
				{
					var srpClient = new SrpClient();

					var salt = srpClient.GenerateSalt();
					var privateKey = srpClient.DerivePrivateKey(salt, chunks[1], chunks[2]);
					var verifier = srpClient.DeriveVerifier(privateKey);

					connection.Send(new RegisterRequestPacket {Username = chunks[1], Salt = salt, Verifier = verifier});

					while (true)
					{
						if (connection.Receive() is RegisterResponsePacket registerResponsePacket)
						{
							Console.WriteLine(
								registerResponsePacket.Success ? $"Registered account '{chunks[1]}'." : $"Unable to registered account '{chunks[1]}'."
							);

							break;
						}

						Thread.Sleep(15);
					}

					break;
				}

				case "login" when chunks.Length >= 3:
				{
					connection.Send(new PrepareLoginRequestPacket {Username = chunks[1]});

					PrepareLoginResponsePacket prepareLoginResponsePacket;

					while (true)
					{
						if (connection.Receive() is PrepareLoginResponsePacket responsePacket)
						{
							prepareLoginResponsePacket = responsePacket;

							break;
						}

						Thread.Sleep(15);
					}

					var srpClient = new SrpClient();
					var privateKey = srpClient.DerivePrivateKey(prepareLoginResponsePacket.Salt, chunks[1], chunks[2]);
					var ephemeral = srpClient.GenerateEphemeral();

					var session = srpClient.DeriveSession(
						ephemeral.Secret,
						prepareLoginResponsePacket.Ephemeral,
						prepareLoginResponsePacket.Salt,
						chunks[1],
						privateKey
					);

					connection.Send(new LoginRequestPacket {Ephemeral = ephemeral.Public, Proof = session.Proof});

					LoginResponsePacket loginResponsePacket;

					while (true)
					{
						if (connection.Receive() is LoginResponsePacket responsePacket)
						{
							loginResponsePacket = responsePacket;

							break;
						}

						Thread.Sleep(15);
					}

					try
					{
						srpClient.VerifySession(ephemeral.Public, session, loginResponsePacket.Proof);
						connection.Send(new KeyChangedPacket());
						connection.EncryptionKey = Encoding.ASCII.GetBytes(session.Key);
						Console.WriteLine("Login successful!");

						return new ChatHandler();
					}
					catch (Exception)
					{
						Console.WriteLine("Login failed!");
					}

					break;
				}

				case "quit":
					connection.Dispose();

					break;
			}

			return this;
		}
	}
}

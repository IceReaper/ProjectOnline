namespace ProjectOnline.Client
{
	using Microsoft.Xna.Framework;
	using Shared.Networking;
	using Shared.Networking.Packets;
	using System;
	using System.Threading;

	public class Application : Game
	{
		private readonly GraphicsDeviceManager graphicsDeviceManager;

		public Application()
		{
			this.graphicsDeviceManager = new GraphicsDeviceManager(this);
		}

		protected override void Initialize()
		{
			var client = new NetworkClient();
			var connection = client.Connect("localhost");

			while (connection.IsConnecting)
				Thread.Sleep(15);

			var line = "";
			
			
			if (connection.IsConnected)
			{
				var username = Console.ReadLine()!.Trim();
				var password = username;
			}

			while (connection.IsConnected)
			{
				Thread.Sleep(15);

				IPacket packet;

				while ((packet = connection.Receive()) != null)
				{
					if (packet is ChatPacket chatPacket)
						Console.WriteLine(chatPacket.Message);
				}

				if (!Console.KeyAvailable)
					continue;

				var key = Console.ReadKey(true);

				if (key.Key != ConsoleKey.Enter)
				{
					line += key.KeyChar;

					continue;
				}

				if (line == "quit")
				{
					connection.Dispose();

					break;
				}

				connection.Send(new ChatPacket {Message = line});

				line = "";
			}
		}

		protected override void Draw(GameTime gameTime)
		{
		}

		protected override void Update(GameTime gameTime)
		{
		}
	}
}

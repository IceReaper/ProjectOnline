namespace ProjectOnline.Client
{
	using Microsoft.Xna.Framework;
	using Networking;
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

			while (connection.IsConnected)
			{
				Thread.Sleep(15);

				foreach (var packet in connection.Receive())
				{
					if (packet is not ChatPacket chatPacket)
						continue;

					Console.WriteLine($"ChatPacket: {chatPacket.Message}");
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

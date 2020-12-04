namespace ProjectOnline.Client
{
	using Shared.Networking;
	using Shared.Networking.Packets;
	using System;
	using System.Threading;

	public class ChatHandler : IHandler
	{
		private string line = "";

		public IHandler Handle(Connection connection)
		{
			IPacket? packet;

			while ((packet = connection.Receive()) != null)
			{
				if (packet is ChatPacket chatPacket)
					Console.WriteLine(chatPacket.Message);
			}

			if (Console.KeyAvailable)
			{
				var key = Console.ReadKey(true);

				if (key.Key != ConsoleKey.Enter)
				{
					this.line += key.KeyChar;

					return this;
				}

				if (this.line == "quit")
				{
					connection.Dispose();

					return this;
				}

				connection.Send(new ChatPacket {Message = this.line!});

				this.line = "";
			}

			Thread.Sleep(15);

			return this;
		}
	}
}

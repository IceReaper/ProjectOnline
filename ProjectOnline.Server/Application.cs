namespace ProjectOnline.Server
{
	using Authentication;
	using Chatting;
	using Shared.Networking;
	using System;

	public class Application : IDisposable
	{
		private bool isDisposed;

		public void Run()
		{
			var chatServer = new ChatServer();
			chatServer.Start();

			var authenticationServer = new AuthenticationServer(chatServer);
			authenticationServer.Start();

			var server = new NetworkServer(authenticationServer);
			server.Start();

			while (!this.isDisposed)
			{
				if (Console.ReadLine() == "quit")
					this.Dispose();
			}

			server.Stop();
			authenticationServer.Stop();
		}

		public void Dispose()
		{
			this.isDisposed = true;
			GC.SuppressFinalize(this);
		}
	}
}

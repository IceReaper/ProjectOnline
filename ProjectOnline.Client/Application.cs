namespace ProjectOnline.Client
{
	using Microsoft.Xna.Framework;
	using Shared.Networking;
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

			IHandler handler = new AuthenticationHandler();

			while (connection.IsConnected)
				handler = handler.Handle(connection);
		}

		protected override void Draw(GameTime gameTime)
		{
		}

		protected override void Update(GameTime gameTime)
		{
		}
	}
}

namespace ProjectOnline.Client
{
	using Shared.Networking;

	public interface IHandler
	{
		public IHandler Handle(Connection connection);
	}
}

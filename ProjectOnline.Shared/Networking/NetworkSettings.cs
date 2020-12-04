namespace ProjectOnline.Shared.Networking
{
	using System;
	using System.Linq;

	public static class NetworkSettings
	{
		public const string HandshakeKey = "ProjectOnline";
		public const short Port = 9001;

		public static readonly Type[] Packets;

		static NetworkSettings()
		{
			NetworkSettings.Packets = typeof(IPacket).Assembly.GetTypes()
				.Where(type => typeof(IPacket).IsAssignableFrom(type) && type.IsClass)
				.OrderBy(type => type.FullName)
				.ToArray();
		}
	}
}

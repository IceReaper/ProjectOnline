namespace ProjectOnline.Shared.Networking
{
	using Packets;
	using System;
	using System.Text;

	public static class NetworkSettings
	{
		public const string HandshakeKey = "ProjectOnline";
		public const short Port = 9001;
		public const short PollTimeout = 100;

		public static readonly byte[] EncryptionKey = Encoding.UTF8.GetBytes("ProjectOnline");
		public static readonly Type[] Packets = {typeof(ChatPacket)};
	}
}

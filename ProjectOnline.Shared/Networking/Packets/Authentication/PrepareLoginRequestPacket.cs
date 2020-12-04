namespace ProjectOnline.Shared.Networking.Packets.Authentication
{
	using System.IO;

	public class PrepareLoginRequestPacket : IPacket
	{
		public string Username = null!;

		public void Read(BinaryReader reader)
		{
			this.Username = reader.ReadString();
		}

		public void Write(BinaryWriter writer)
		{
			writer.Write(this.Username);
		}
	}
}

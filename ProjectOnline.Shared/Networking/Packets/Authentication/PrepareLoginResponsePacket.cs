namespace ProjectOnline.Shared.Networking.Packets.Authentication
{
	using System.IO;

	public class PrepareLoginResponsePacket : IPacket
	{
		public string Salt = null!;
		public string Ephemeral = null!;

		public void Read(BinaryReader reader)
		{
			this.Salt = reader.ReadString();
			this.Ephemeral = reader.ReadString();
		}

		public void Write(BinaryWriter writer)
		{
			writer.Write(this.Salt);
			writer.Write(this.Ephemeral);
		}
	}
}

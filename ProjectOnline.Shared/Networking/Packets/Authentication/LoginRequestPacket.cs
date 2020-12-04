namespace ProjectOnline.Shared.Networking.Packets.Authentication
{
	using System.IO;

	public class LoginRequestPacket : IPacket
	{
		public string Ephemeral = null!;
		public string Proof = null!;

		public void Read(BinaryReader reader)
		{
			this.Ephemeral = reader.ReadString();
			this.Proof = reader.ReadString();
		}

		public void Write(BinaryWriter writer)
		{
			writer.Write(this.Ephemeral);
			writer.Write(this.Proof);
		}
	}
}

namespace ProjectOnline.Shared.Networking.Packets.Authentication
{
	using System.IO;

	public class RegisterRequestPacket : IPacket
	{
		public string Username = null!;
		public string Salt = null!;
		public string Verifier = null!;

		public void Read(BinaryReader reader)
		{
			this.Username = reader.ReadString();
			this.Salt = reader.ReadString();
			this.Verifier = reader.ReadString();
		}

		public void Write(BinaryWriter writer)
		{
			writer.Write(this.Username);
			writer.Write(this.Salt);
			writer.Write(this.Verifier);
		}
	}
}

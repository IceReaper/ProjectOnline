namespace ProjectOnline.Shared.Networking.Packets.Authentication
{
	using System.IO;

	public class LoginResponsePacket : IPacket
	{
		public string Proof = null!;

		public void Read(BinaryReader reader)
		{
			this.Proof = reader.ReadString();
		}

		public void Write(BinaryWriter writer)
		{
			writer.Write(this.Proof);
		}
	}
}

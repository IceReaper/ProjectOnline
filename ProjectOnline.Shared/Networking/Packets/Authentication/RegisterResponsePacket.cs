namespace ProjectOnline.Shared.Networking.Packets.Authentication
{
	using System.IO;

	public class RegisterResponsePacket : IPacket
	{
		public bool Success;

		public void Read(BinaryReader reader)
		{
			this.Success = reader.ReadBoolean();
		}

		public void Write(BinaryWriter writer)
		{
			writer.Write(this.Success);
		}
	}
}

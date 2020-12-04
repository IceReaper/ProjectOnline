namespace ProjectOnline.Shared.Networking.Packets
{
	using System.IO;

	public class ChatPacket : IPacket
	{
		public string Message = null!;

		public void Read(BinaryReader reader)
		{
			this.Message = reader.ReadString();
		}

		public void Write(BinaryWriter writer)
		{
			writer.Write(this.Message);
		}
	}
}

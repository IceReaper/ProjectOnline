namespace ProjectOnline.Shared.Networking
{
	using System.IO;

	public interface IPacket
	{
		public void Read(BinaryReader reader);
		public void Write(BinaryWriter writer);
	}
}

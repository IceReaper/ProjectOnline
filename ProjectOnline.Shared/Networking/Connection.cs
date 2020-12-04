namespace ProjectOnline.Shared.Networking
{
	using LiteNetLib;
	using System;
	using System.Collections.Generic;
	using System.IO;
	using System.IO.Compression;
	using System.Text;
	using Utils;

	public class Connection : IDisposable
	{
		private readonly NetPeer peer;
		private readonly List<IPacket> packets = new();

		public readonly List<object> Properties = new();

		public byte[] EncryptionKey = Encoding.UTF8.GetBytes(NetworkSettings.HandshakeKey);

		public bool IsConnecting => this.peer.ConnectionState == ConnectionState.Outgoing;
		public bool IsConnected => this.peer.ConnectionState != ConnectionState.Disconnected;

		public Connection(NetPeer peer)
		{
			this.peer = peer;
		}

		public void Receive(byte[] data)
		{
			try
			{
				var reader = new BinaryReader(new GZipStream(new EncryptedStream(new MemoryStream(data), this.EncryptionKey), CompressionMode.Decompress));
				var packet = (IPacket?) Activator.CreateInstance(NetworkSettings.Packets[reader.ReadUInt16()]);

				if (packet == null)
					return;

				packet.Read(reader);
				this.packets.Add(packet);
			}
			catch (Exception)
			{
				// ignored
			}
		}

		public IPacket? Receive()
		{
			if (this.packets.Count == 0)
				return null;

			var packet = this.packets[0];
			this.packets.Remove(packet);

			return packet;
		}

		public void Send(IPacket packet)
		{
			var stream = new MemoryStream();
			var compression = new GZipStream(stream, CompressionMode.Compress);
			var writer = new BinaryWriter(compression);

			writer.Write((ushort) Array.IndexOf(NetworkSettings.Packets, packet.GetType()));
			packet.Write(writer);

			compression.Flush();

			var encrypted = new MemoryStream();
			new EncryptedStream(encrypted, this.EncryptionKey).Write(stream.ToArray());
			this.peer.Send(encrypted.ToArray(), DeliveryMethod.ReliableOrdered);
		}

		public void Dispose()
		{
			this.peer.Disconnect();
			GC.SuppressFinalize(this);
		}
	}
}

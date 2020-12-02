namespace ProjectOnline.Shared.Networking
{
	using LiteNetLib;
	using System;
	using System.Collections.Generic;
	using System.IO;
	using System.IO.Compression;

	public class Connection : IDisposable
	{
		private readonly NetPeer peer;
		private readonly Action? onDispose;
		private readonly List<IPacket> packets = new();

		public bool IsConnecting => this.peer.ConnectionState == ConnectionState.Outgoing;
		public bool IsConnected => this.peer.ConnectionState != ConnectionState.Disconnected;

		public Connection(NetPeer peer, Action? onDispose = null)
		{
			this.peer = peer;
			this.onDispose = onDispose;
		}

		public void Receive(byte[] data)
		{
			var reader = new BinaryReader(
				new GZipStream(new EncryptedStream(new MemoryStream(data), NetworkSettings.EncryptionKey), CompressionMode.Decompress)
			);

			var packet = (IPacket?) Activator.CreateInstance(NetworkSettings.Packets[reader.ReadUInt16()]);

			if (packet == null)
				return;

			packet.Read(reader);
			this.packets.Add(packet);
		}

		public IEnumerable<IPacket> Receive()
		{
			var packets = this.packets.ToArray();

			foreach (var packet in packets)
				this.packets.Remove(packet);

			return packets;
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
			new EncryptedStream(encrypted, NetworkSettings.EncryptionKey).Write(stream.ToArray());
			this.peer.Send(encrypted.ToArray(), DeliveryMethod.ReliableOrdered);
		}

		public void Dispose()
		{
			this.peer.Disconnect();
			this.onDispose?.Invoke();
			GC.SuppressFinalize(this);
		}
	}
}

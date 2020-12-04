namespace ProjectOnline.Shared.Utils
{
	using System.IO;

	public class EncryptedStream : Stream
	{
		private readonly Stream BaseStream;

		private readonly byte[] key;
		private readonly byte[] keyNum0;

		public override bool CanRead => this.BaseStream.CanRead;
		public override bool CanSeek => this.BaseStream.CanSeek;
		public override bool CanWrite => this.BaseStream.CanWrite;
		public override long Length => this.BaseStream.Length;

		public override long Position
		{
			get => this.BaseStream.Position;
			set => this.BaseStream.Position = value;
		}

		public EncryptedStream(Stream baseStream, byte[] key)
		{
			this.key = key;
			this.BaseStream = baseStream;

			this.keyNum0 = new byte[this.key.Length];

			for (var i = 0; i < this.key.Length; i++)
			for (var j = 0; j < 8; j++)
				if (((this.key[i] >> j) & 1) == 0)
					this.keyNum0[i]++;
		}

		public override long Seek(long offset, SeekOrigin origin)
		{
			return this.BaseStream.Seek(offset, origin);
		}

		public override void SetLength(long value)
		{
			this.BaseStream.SetLength(value);
		}

		public override void Flush()
		{
			this.BaseStream.Flush();
		}

		public override int Read(byte[] buffer, int offset, int count)
		{
			var startOffset = this.Position;
			var result = this.BaseStream.Read(buffer, offset, count);

			for (var i = 0; i < result; i++)
				buffer[offset + i] = this.Decrypt(buffer[offset + i], startOffset + i);

			return result;
		}

		public override void Write(byte[] buffer, int offset, int count)
		{
			var startOffset = this.Position;
			var encrypted = new byte[count];

			for (var i = 0; i < count; i++)
				encrypted[i] = this.Encrypt(buffer[offset + i], startOffset + i);

			this.BaseStream.Write(encrypted);
		}

		private byte Encrypt(byte decrypted, long keyOffset)
		{
			var byteKey = this.key[keyOffset % this.key.Length];
			var byteKeyNum0 = this.keyNum0[keyOffset % this.keyNum0.Length];

			var encrypted = 0;
			var num0 = 0;
			var num1 = 0;

			for (var i = 0; i < 8; i++)
			{
				var keyBit = (byteKey >> i) & 1;
				encrypted |= ((((decrypted >> i) & 1) + keyBit) % 2) << (keyBit == 0 ? num0++ : byteKeyNum0 + num1++);
			}

			return (byte) encrypted;
		}

		private byte Decrypt(byte encrypted, long keyOffset)
		{
			var byteKey = this.key[keyOffset % this.key.Length];
			var byteKeyNum0 = this.keyNum0[keyOffset % this.keyNum0.Length];

			var decrypted = 0;
			var num0 = 0;
			var num1 = 0;

			for (var i = 0; i < 8; i++)
			{
				var keyBit = (byteKey >> i) & 1;
				decrypted |= ((((encrypted >> (keyBit == 0 ? num0++ : byteKeyNum0 + num1++)) + keyBit) % 2) & 1) << i;
			}

			return (byte) decrypted;
		}
	}
}

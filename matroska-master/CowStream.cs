using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;

namespace Matroska
{
    /// <summary>
    /// Implements a <see cref="Memory{T}"/>-based stream that copies its memory on first write (Copy On Write).
    /// </summary>
    internal sealed class CowStream : Stream
	{
		#region Fields

		private Memory<byte> data;
		private bool ownsData;
		private int position;

		#endregion

		#region Properties

		public override bool CanRead => true;
		public override bool CanSeek => true;
		public override bool CanWrite => true;
		public override long Length => data.Length;

		public override long Position
		{
			get => position;
			set
			{
				if (value < 0)
					throw new ArgumentOutOfRangeException(nameof(value));

				if (value > Length)
					throw new ArgumentOutOfRangeException(nameof(value));

				position = (int)value;
			}
		}
		
		#endregion

		#region Constructors

		public CowStream(Memory<byte> data, bool ownsData = true)
		{
			this.data = data;
			this.ownsData = ownsData;
		}

		public CowStream(CowStream baseStream)
		{
			data = baseStream.data;
			ownsData = false;
		}

		#endregion

		#region Methods

		#endregion

		public override void Flush()
		{
			
		}

		public override Task FlushAsync(CancellationToken cancellationToken)
		{
			return Task.CompletedTask;
		}
		
		public override int Read(Span<byte> buffer)
		{
			if (position < 0)
				throw new ObjectDisposedException(nameof(CowStream));

			int n = Math.Min(data.Length - position, buffer.Length);
			data.Span.Slice(position, n).CopyTo(buffer.Slice(0, n));
			position += n;
			return n;
		}
		
		public override int Read(byte[] buffer, int offset, int count) => Read(new Span<byte>(buffer, offset, count));

		public override Task<int> ReadAsync(byte[] buffer, int offset, int count, CancellationToken cancellationToken) => ReadAsync(new Memory<byte>(buffer, offset, count), cancellationToken).AsTask();

		public override ValueTask<int> ReadAsync(Memory<byte> buffer, CancellationToken cancellationToken = new CancellationToken()) => new ValueTask<int>(Read(buffer.Span));

		public override int ReadByte()
		{
			Span<byte> temp = stackalloc byte[1];
			int n = Read(temp);
			if (n == 0)
				return -1;
			return temp[0];
		}

		public override void Write(ReadOnlySpan<byte> buffer)
		{
			if (position < 0)
				throw new ObjectDisposedException(nameof(CowStream));

			if (position + buffer.Length > Length)
				throw new NotSupportedException("Can't write past the end of the stream.");

			if (!ownsData)
			{
				byte[] dataBuf = new byte[data.Length];
				data.CopyTo(dataBuf);
				data = dataBuf;
				ownsData = true;
			}

			buffer.CopyTo(data.Span.Slice(position, buffer.Length));
			position += buffer.Length;
		}

		public override void Write(byte[] buffer, int offset, int count) => Write(new ReadOnlySpan<byte>(buffer, offset, count));

		public override ValueTask WriteAsync(ReadOnlyMemory<byte> buffer, CancellationToken cancellationToken = new CancellationToken())
		{
			Write(buffer.Span);
			return default;
		}

		public override Task WriteAsync(byte[] buffer, int offset, int count, CancellationToken cancellationToken)
		{
			Write(new ReadOnlySpan<byte>(buffer, offset, count));
			return Task.CompletedTask;
		}

		public override void WriteByte(byte value)
		{
			Span<byte> temp = stackalloc byte[1];
			temp[0] = value;
			Write(temp);
		}

		public override long Seek(long offset, SeekOrigin origin)
		{
			if (position < 0)
				throw new ObjectDisposedException(nameof(CowStream));
			
			switch (origin)
			{
				case SeekOrigin.Begin:
					Position = offset;
					break;
				case SeekOrigin.Current:
					Position += offset;
					break;
				case SeekOrigin.End:
					Position = Length - offset;
					break;
				default:
					throw new ArgumentOutOfRangeException(nameof(origin), origin, null);
			}

			return Position;
		}

		public override void SetLength(long value)
		{
			throw new NotSupportedException();
		}

		protected override void Dispose(bool disposing)
		{
			data = default;
			position = -1;
		}

		public override ValueTask DisposeAsync()
		{
			data = default;
			position = -1;
			return default;
		}
	}
}

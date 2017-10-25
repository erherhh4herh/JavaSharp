/*
 * Copyright (c) 1996, 2006, Oracle and/or its affiliates. All rights reserved.
 * ORACLE PROPRIETARY/CONFIDENTIAL. Use is subject to license terms.
 *
 *
 *
 *
 *
 *
 *
 *
 *
 *
 *
 *
 *
 *
 *
 *
 *
 *
 *
 *
 */

namespace java.util.zip
{


	/// <summary>
	/// An input stream that also maintains a checksum of the data being read.
	/// The checksum can then be used to verify the integrity of the input data.
	/// </summary>
	/// <seealso cref=         Checksum
	/// @author      David Connelly </seealso>
	public class CheckedInputStream : FilterInputStream
	{
		private Checksum Cksum;

		/// <summary>
		/// Creates an input stream using the specified Checksum. </summary>
		/// <param name="in"> the input stream </param>
		/// <param name="cksum"> the Checksum </param>
		public CheckedInputStream(InputStream @in, Checksum cksum) : base(@in)
		{
			this.Cksum = cksum;
		}

		/// <summary>
		/// Reads a byte. Will block if no input is available. </summary>
		/// <returns> the byte read, or -1 if the end of the stream is reached. </returns>
		/// <exception cref="IOException"> if an I/O error has occurred </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public int read() throws java.io.IOException
		public override int Read()
		{
			int b = @in.Read();
			if (b != -1)
			{
				Cksum.Update(b);
			}
			return b;
		}

		/// <summary>
		/// Reads into an array of bytes. If <code>len</code> is not zero, the method
		/// blocks until some input is available; otherwise, no
		/// bytes are read and <code>0</code> is returned. </summary>
		/// <param name="buf"> the buffer into which the data is read </param>
		/// <param name="off"> the start offset in the destination array <code>b</code> </param>
		/// <param name="len"> the maximum number of bytes read </param>
		/// <returns>    the actual number of bytes read, or -1 if the end
		///            of the stream is reached. </returns>
		/// <exception cref="NullPointerException"> If <code>buf</code> is <code>null</code>. </exception>
		/// <exception cref="IndexOutOfBoundsException"> If <code>off</code> is negative,
		/// <code>len</code> is negative, or <code>len</code> is greater than
		/// <code>buf.length - off</code> </exception>
		/// <exception cref="IOException"> if an I/O error has occurred </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public int read(byte[] buf, int off, int len) throws java.io.IOException
		public override int Read(sbyte[] buf, int off, int len)
		{
			len = @in.Read(buf, off, len);
			if (len != -1)
			{
				Cksum.Update(buf, off, len);
			}
			return len;
		}

		/// <summary>
		/// Skips specified number of bytes of input. </summary>
		/// <param name="n"> the number of bytes to skip </param>
		/// <returns> the actual number of bytes skipped </returns>
		/// <exception cref="IOException"> if an I/O error has occurred </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public long skip(long n) throws java.io.IOException
		public override long Skip(long n)
		{
			sbyte[] buf = new sbyte[512];
			long total = 0;
			while (total < n)
			{
				long len = n - total;
				len = Read(buf, 0, len < buf.Length ? (int)len : buf.Length);
				if (len == -1)
				{
					return total;
				}
				total += len;
			}
			return total;
		}

		/// <summary>
		/// Returns the Checksum for this input stream. </summary>
		/// <returns> the Checksum value </returns>
		public virtual Checksum Checksum
		{
			get
			{
				return Cksum;
			}
		}
	}

}
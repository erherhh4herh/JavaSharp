/*
 * Copyright (c) 1996, 1999, Oracle and/or its affiliates. All rights reserved.
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
	/// An output stream that also maintains a checksum of the data being
	/// written. The checksum can then be used to verify the integrity of
	/// the output data.
	/// </summary>
	/// <seealso cref=         Checksum
	/// @author      David Connelly </seealso>
	public class CheckedOutputStream : FilterOutputStream
	{
		private Checksum Cksum;

		/// <summary>
		/// Creates an output stream with the specified Checksum. </summary>
		/// <param name="out"> the output stream </param>
		/// <param name="cksum"> the checksum </param>
		public CheckedOutputStream(OutputStream @out, Checksum cksum) : base(@out)
		{
			this.Cksum = cksum;
		}

		/// <summary>
		/// Writes a byte. Will block until the byte is actually written. </summary>
		/// <param name="b"> the byte to be written </param>
		/// <exception cref="IOException"> if an I/O error has occurred </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public void write(int b) throws java.io.IOException
		public override void Write(int b)
		{
			@out.Write(b);
			Cksum.Update(b);
		}

		/// <summary>
		/// Writes an array of bytes. Will block until the bytes are
		/// actually written. </summary>
		/// <param name="b"> the data to be written </param>
		/// <param name="off"> the start offset of the data </param>
		/// <param name="len"> the number of bytes to be written </param>
		/// <exception cref="IOException"> if an I/O error has occurred </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public void write(byte[] b, int off, int len) throws java.io.IOException
		public override void Write(sbyte[] b, int off, int len)
		{
			@out.Write(b, off, len);
			Cksum.Update(b, off, len);
		}

		/// <summary>
		/// Returns the Checksum for this output stream. </summary>
		/// <returns> the Checksum </returns>
		public virtual Checksum Checksum
		{
			get
			{
				return Cksum;
			}
		}
	}

}
/*
 * Copyright (c) 1996, 2013, Oracle and/or its affiliates. All rights reserved.
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

namespace java.security
{


	/// <summary>
	/// A transparent stream that updates the associated message digest using
	/// the bits going through the stream.
	/// 
	/// <para>To complete the message digest computation, call one of the
	/// {@code digest} methods on the associated message
	/// digest after your calls to one of this digest input stream's
	/// <seealso cref="#read() read"/> methods.
	/// 
	/// </para>
	/// <para>It is possible to turn this stream on or off (see
	/// <seealso cref="#on(boolean) on"/>). When it is on, a call to one of the
	/// {@code read} methods
	/// results in an update on the message digest.  But when it is off,
	/// the message digest is not updated. The default is for the stream
	/// to be on.
	/// 
	/// </para>
	/// <para>Note that digest objects can compute only one digest (see
	/// <seealso cref="MessageDigest"/>),
	/// so that in order to compute intermediate digests, a caller should
	/// retain a handle onto the digest object, and clone it for each
	/// digest to be computed, leaving the orginal digest untouched.
	/// 
	/// </para>
	/// </summary>
	/// <seealso cref= MessageDigest
	/// </seealso>
	/// <seealso cref= DigestOutputStream
	/// 
	/// @author Benjamin Renaud </seealso>

	public class DigestInputStream : FilterInputStream
	{

		/* NOTE: This should be made a generic UpdaterInputStream */

		/* Are we on or off? */
		private bool On_Renamed = true;

		/// <summary>
		/// The message digest associated with this stream.
		/// </summary>
		protected internal MessageDigest Digest;

		/// <summary>
		/// Creates a digest input stream, using the specified input stream
		/// and message digest.
		/// </summary>
		/// <param name="stream"> the input stream.
		/// </param>
		/// <param name="digest"> the message digest to associate with this stream. </param>
		public DigestInputStream(InputStream stream, MessageDigest digest) : base(stream)
		{
			MessageDigest = digest;
		}

		/// <summary>
		/// Returns the message digest associated with this stream.
		/// </summary>
		/// <returns> the message digest associated with this stream. </returns>
		/// <seealso cref= #setMessageDigest(java.security.MessageDigest) </seealso>
		public virtual MessageDigest MessageDigest
		{
			get
			{
				return Digest;
			}
			set
			{
				this.Digest = value;
			}
		}


		/// <summary>
		/// Reads a byte, and updates the message digest (if the digest
		/// function is on).  That is, this method reads a byte from the
		/// input stream, blocking until the byte is actually read. If the
		/// digest function is on (see <seealso cref="#on(boolean) on"/>), this method
		/// will then call {@code update} on the message digest associated
		/// with this stream, passing it the byte read.
		/// </summary>
		/// <returns> the byte read.
		/// </returns>
		/// <exception cref="IOException"> if an I/O error occurs.
		/// </exception>
		/// <seealso cref= MessageDigest#update(byte) </seealso>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public int read() throws java.io.IOException
		public override int Read()
		{
			int ch = @in.Read();
			if (On_Renamed && ch != -1)
			{
				Digest.Update((sbyte)ch);
			}
			return ch;
		}

		/// <summary>
		/// Reads into a byte array, and updates the message digest (if the
		/// digest function is on).  That is, this method reads up to
		/// {@code len} bytes from the input stream into the array
		/// {@code b}, starting at offset {@code off}. This method
		/// blocks until the data is actually
		/// read. If the digest function is on (see
		/// <seealso cref="#on(boolean) on"/>), this method will then call {@code update}
		/// on the message digest associated with this stream, passing it
		/// the data.
		/// </summary>
		/// <param name="b"> the array into which the data is read.
		/// </param>
		/// <param name="off"> the starting offset into {@code b} of where the
		/// data should be placed.
		/// </param>
		/// <param name="len"> the maximum number of bytes to be read from the input
		/// stream into b, starting at offset {@code off}.
		/// </param>
		/// <returns>  the actual number of bytes read. This is less than
		/// {@code len} if the end of the stream is reached prior to
		/// reading {@code len} bytes. -1 is returned if no bytes were
		/// read because the end of the stream had already been reached when
		/// the call was made.
		/// </returns>
		/// <exception cref="IOException"> if an I/O error occurs.
		/// </exception>
		/// <seealso cref= MessageDigest#update(byte[], int, int) </seealso>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public int read(byte[] b, int off, int len) throws java.io.IOException
		public override int Read(sbyte[] b, int off, int len)
		{
			int result = @in.Read(b, off, len);
			if (On_Renamed && result != -1)
			{
				Digest.Update(b, off, result);
			}
			return result;
		}

		/// <summary>
		/// Turns the digest function on or off. The default is on.  When
		/// it is on, a call to one of the {@code read} methods results in an
		/// update on the message digest.  But when it is off, the message
		/// digest is not updated.
		/// </summary>
		/// <param name="on"> true to turn the digest function on, false to turn
		/// it off. </param>
		public virtual void On(bool on)
		{
			this.On_Renamed = on;
		}

		/// <summary>
		/// Prints a string representation of this digest input stream and
		/// its associated message digest object.
		/// </summary>
		 public override String ToString()
		 {
			 return "[Digest Input Stream] " + Digest.ToString();
		 }
	}

}
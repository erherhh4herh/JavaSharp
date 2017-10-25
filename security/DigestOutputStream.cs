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
	/// digest after your calls to one of this digest output stream's
	/// <seealso cref="#write(int) write"/> methods.
	/// 
	/// </para>
	/// <para>It is possible to turn this stream on or off (see
	/// <seealso cref="#on(boolean) on"/>). When it is on, a call to one of the
	/// {@code write} methods results in
	/// an update on the message digest.  But when it is off, the message
	/// digest is not updated. The default is for the stream to be on.
	/// 
	/// </para>
	/// </summary>
	/// <seealso cref= MessageDigest </seealso>
	/// <seealso cref= DigestInputStream
	/// 
	/// @author Benjamin Renaud </seealso>
	public class DigestOutputStream : FilterOutputStream
	{

		private bool On_Renamed = true;

		/// <summary>
		/// The message digest associated with this stream.
		/// </summary>
		protected internal MessageDigest Digest;

		/// <summary>
		/// Creates a digest output stream, using the specified output stream
		/// and message digest.
		/// </summary>
		/// <param name="stream"> the output stream.
		/// </param>
		/// <param name="digest"> the message digest to associate with this stream. </param>
		public DigestOutputStream(OutputStream stream, MessageDigest digest) : base(stream)
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
		/// Updates the message digest (if the digest function is on) using
		/// the specified byte, and in any case writes the byte
		/// to the output stream. That is, if the digest function is on
		/// (see <seealso cref="#on(boolean) on"/>), this method calls
		/// {@code update} on the message digest associated with this
		/// stream, passing it the byte {@code b}. This method then
		/// writes the byte to the output stream, blocking until the byte
		/// is actually written.
		/// </summary>
		/// <param name="b"> the byte to be used for updating and writing to the
		/// output stream.
		/// </param>
		/// <exception cref="IOException"> if an I/O error occurs.
		/// </exception>
		/// <seealso cref= MessageDigest#update(byte) </seealso>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public void write(int b) throws java.io.IOException
		public override void Write(int b)
		{
			@out.Write(b);
			if (On_Renamed)
			{
				Digest.Update((sbyte)b);
			}
		}

		/// <summary>
		/// Updates the message digest (if the digest function is on) using
		/// the specified subarray, and in any case writes the subarray to
		/// the output stream. That is, if the digest function is on (see
		/// <seealso cref="#on(boolean) on"/>), this method calls {@code update}
		/// on the message digest associated with this stream, passing it
		/// the subarray specifications. This method then writes the subarray
		/// bytes to the output stream, blocking until the bytes are actually
		/// written.
		/// </summary>
		/// <param name="b"> the array containing the subarray to be used for updating
		/// and writing to the output stream.
		/// </param>
		/// <param name="off"> the offset into {@code b} of the first byte to
		/// be updated and written.
		/// </param>
		/// <param name="len"> the number of bytes of data to be updated and written
		/// from {@code b}, starting at offset {@code off}.
		/// </param>
		/// <exception cref="IOException"> if an I/O error occurs.
		/// </exception>
		/// <seealso cref= MessageDigest#update(byte[], int, int) </seealso>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public void write(byte[] b, int off, int len) throws java.io.IOException
		public override void Write(sbyte[] b, int off, int len)
		{
			@out.Write(b, off, len);
			if (On_Renamed)
			{
				Digest.Update(b, off, len);
			}
		}

		/// <summary>
		/// Turns the digest function on or off. The default is on.  When
		/// it is on, a call to one of the {@code write} methods results in an
		/// update on the message digest.  But when it is off, the message
		/// digest is not updated.
		/// </summary>
		/// <param name="on"> true to turn the digest function on, false to turn it
		/// off. </param>
		public virtual void On(bool on)
		{
			this.On_Renamed = on;
		}

		/// <summary>
		/// Prints a string representation of this digest output stream and
		/// its associated message digest object.
		/// </summary>
		 public override String ToString()
		 {
			 return "[Digest Output Stream] " + Digest.ToString();
		 }
	}

}
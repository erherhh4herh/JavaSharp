using System;

/*
 * Copyright (c) 1997, 2013, Oracle and/or its affiliates. All rights reserved.
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

	using JCAUtil = sun.security.jca.JCAUtil;

	/// <summary>
	/// This class defines the <i>Service Provider Interface</i> (<b>SPI</b>)
	/// for the {@code MessageDigest} class, which provides the functionality
	/// of a message digest algorithm, such as MD5 or SHA. Message digests are
	/// secure one-way hash functions that take arbitrary-sized data and output a
	/// fixed-length hash value.
	/// 
	/// <para> All the abstract methods in this class must be implemented by a
	/// cryptographic service provider who wishes to supply the implementation
	/// of a particular message digest algorithm.
	/// 
	/// </para>
	/// <para> Implementations are free to implement the Cloneable interface.
	/// 
	/// @author Benjamin Renaud
	/// 
	/// 
	/// </para>
	/// </summary>
	/// <seealso cref= MessageDigest </seealso>

	public abstract class MessageDigestSpi
	{

		// for re-use in engineUpdate(ByteBuffer input)
		private sbyte[] TempArray;

		/// <summary>
		/// Returns the digest length in bytes.
		/// 
		/// <para>This concrete method has been added to this previously-defined
		/// abstract class. (For backwards compatibility, it cannot be abstract.)
		/// 
		/// </para>
		/// <para>The default behavior is to return 0.
		/// 
		/// </para>
		/// <para>This method may be overridden by a provider to return the digest
		/// length.
		/// 
		/// </para>
		/// </summary>
		/// <returns> the digest length in bytes.
		/// 
		/// @since 1.2 </returns>
		protected internal virtual int EngineGetDigestLength()
		{
			return 0;
		}

		/// <summary>
		/// Updates the digest using the specified byte.
		/// </summary>
		/// <param name="input"> the byte to use for the update. </param>
		protected internal abstract void EngineUpdate(sbyte input);

		/// <summary>
		/// Updates the digest using the specified array of bytes,
		/// starting at the specified offset.
		/// </summary>
		/// <param name="input"> the array of bytes to use for the update.
		/// </param>
		/// <param name="offset"> the offset to start from in the array of bytes.
		/// </param>
		/// <param name="len"> the number of bytes to use, starting at
		/// {@code offset}. </param>
		protected internal abstract void EngineUpdate(sbyte[] input, int offset, int len);

		/// <summary>
		/// Update the digest using the specified ByteBuffer. The digest is
		/// updated using the {@code input.remaining()} bytes starting
		/// at {@code input.position()}.
		/// Upon return, the buffer's position will be equal to its limit;
		/// its limit will not have changed.
		/// </summary>
		/// <param name="input"> the ByteBuffer
		/// @since 1.5 </param>
		protected internal virtual void EngineUpdate(ByteBuffer input)
		{
			if (input.HasRemaining() == false)
			{
				return;
			}
			if (input.HasArray())
			{
				sbyte[] b = input.Array();
				int ofs = input.ArrayOffset();
				int pos = input.Position();
				int lim = input.Limit();
				EngineUpdate(b, ofs + pos, lim - pos);
				input.Position(lim);
			}
			else
			{
				int len = input.Remaining();
				int n = JCAUtil.getTempArraySize(len);
				if ((TempArray == null) || (n > TempArray.Length))
				{
					TempArray = new sbyte[n];
				}
				while (len > 0)
				{
					int chunk = System.Math.Min(len, TempArray.Length);
					input.Get(TempArray, 0, chunk);
					EngineUpdate(TempArray, 0, chunk);
					len -= chunk;
				}
			}
		}

		/// <summary>
		/// Completes the hash computation by performing final
		/// operations such as padding. Once {@code engineDigest} has
		/// been called, the engine should be reset (see
		/// <seealso cref="#engineReset() engineReset"/>).
		/// Resetting is the responsibility of the
		/// engine implementor.
		/// </summary>
		/// <returns> the array of bytes for the resulting hash value. </returns>
		protected internal abstract sbyte[] EngineDigest();

		/// <summary>
		/// Completes the hash computation by performing final
		/// operations such as padding. Once {@code engineDigest} has
		/// been called, the engine should be reset (see
		/// <seealso cref="#engineReset() engineReset"/>).
		/// Resetting is the responsibility of the
		/// engine implementor.
		/// 
		/// This method should be abstract, but we leave it concrete for
		/// binary compatibility.  Knowledgeable providers should override this
		/// method.
		/// </summary>
		/// <param name="buf"> the output buffer in which to store the digest
		/// </param>
		/// <param name="offset"> offset to start from in the output buffer
		/// </param>
		/// <param name="len"> number of bytes within buf allotted for the digest.
		/// Both this default implementation and the SUN provider do not
		/// return partial digests.  The presence of this parameter is solely
		/// for consistency in our API's.  If the value of this parameter is less
		/// than the actual digest length, the method will throw a DigestException.
		/// This parameter is ignored if its value is greater than or equal to
		/// the actual digest length.
		/// </param>
		/// <returns> the length of the digest stored in the output buffer.
		/// </returns>
		/// <exception cref="DigestException"> if an error occurs.
		/// 
		/// @since 1.2 </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: protected int engineDigest(byte[] buf, int offset, int len) throws DigestException
		protected internal virtual int EngineDigest(sbyte[] buf, int offset, int len)
		{

			sbyte[] digest = EngineDigest();
			if (len < digest.Length)
			{
					throw new DigestException("partial digests not returned");
			}
			if (buf.Length - offset < digest.Length)
			{
					throw new DigestException("insufficient space in the output " + "buffer to store the digest");
			}
			System.Array.Copy(digest, 0, buf, offset, digest.Length);
			return digest.Length;
		}

		/// <summary>
		/// Resets the digest for further use.
		/// </summary>
		protected internal abstract void EngineReset();

		/// <summary>
		/// Returns a clone if the implementation is cloneable.
		/// </summary>
		/// <returns> a clone if the implementation is cloneable.
		/// </returns>
		/// <exception cref="CloneNotSupportedException"> if this is called on an
		/// implementation that does not support {@code Cloneable}. </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public Object clone() throws CloneNotSupportedException
		public virtual Object Clone()
		{
			if (this is Cloneable)
			{
				return base.Clone();
			}
			else
			{
				throw new CloneNotSupportedException();
			}
		}
	}

}
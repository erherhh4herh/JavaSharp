/*
 * Copyright (c) 1996, 2015, Oracle and/or its affiliates. All rights reserved.
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


	using Debug = sun.security.util.Debug;

	/// <summary>
	/// This MessageDigest class provides applications the functionality of a
	/// message digest algorithm, such as SHA-1 or SHA-256.
	/// Message digests are secure one-way hash functions that take arbitrary-sized
	/// data and output a fixed-length hash value.
	/// 
	/// <para>A MessageDigest object starts out initialized. The data is
	/// processed through it using the <seealso cref="#update(byte) update"/>
	/// methods. At any point <seealso cref="#reset() reset"/> can be called
	/// to reset the digest. Once all the data to be updated has been
	/// updated, one of the <seealso cref="#digest() digest"/> methods should
	/// be called to complete the hash computation.
	/// 
	/// </para>
	/// <para>The {@code digest} method can be called once for a given number
	/// of updates. After {@code digest} has been called, the MessageDigest
	/// object is reset to its initialized state.
	/// 
	/// </para>
	/// <para>Implementations are free to implement the Cloneable interface.
	/// Client applications can test cloneability by attempting cloning
	/// and catching the CloneNotSupportedException:
	/// 
	/// <pre>{@code
	/// MessageDigest md = MessageDigest.getInstance("SHA");
	/// 
	/// try {
	///     md.update(toChapter1);
	///     MessageDigest tc1 = md.clone();
	///     byte[] toChapter1Digest = tc1.digest();
	///     md.update(toChapter2);
	///     ...etc.
	/// } catch (CloneNotSupportedException cnse) {
	///     throw new DigestException("couldn't make digest of partial content");
	/// }
	/// }</pre>
	/// 
	/// </para>
	/// <para>Note that if a given implementation is not cloneable, it is
	/// still possible to compute intermediate digests by instantiating
	/// several instances, if the number of digests is known in advance.
	/// 
	/// </para>
	/// <para>Note that this class is abstract and extends from
	/// {@code MessageDigestSpi} for historical reasons.
	/// Application developers should only take notice of the methods defined in
	/// this {@code MessageDigest} class; all the methods in
	/// the superclass are intended for cryptographic service providers who wish to
	/// supply their own implementations of message digest algorithms.
	/// 
	/// </para>
	/// <para> Every implementation of the Java platform is required to support
	/// the following standard {@code MessageDigest} algorithms:
	/// <ul>
	/// <li>{@code MD5}</li>
	/// <li>{@code SHA-1}</li>
	/// <li>{@code SHA-256}</li>
	/// </ul>
	/// These algorithms are described in the <a href=
	/// "{@docRoot}/../technotes/guides/security/StandardNames.html#MessageDigest">
	/// MessageDigest section</a> of the
	/// Java Cryptography Architecture Standard Algorithm Name Documentation.
	/// Consult the release documentation for your implementation to see if any
	/// other algorithms are supported.
	/// 
	/// @author Benjamin Renaud
	/// 
	/// </para>
	/// </summary>
	/// <seealso cref= DigestInputStream </seealso>
	/// <seealso cref= DigestOutputStream </seealso>

	public abstract class MessageDigest : MessageDigestSpi
	{

		private static readonly Debug Pdebug = Debug.getInstance("provider", "Provider");
		private static readonly bool SkipDebug = Debug.isOn("engine=") && !Debug.isOn("messagedigest");

		private String Algorithm_Renamed;

		// The state of this digest
		private const int INITIAL = 0;
		private const int IN_PROGRESS = 1;
		private int State = INITIAL;

		// The provider
		private Provider Provider_Renamed;

		/// <summary>
		/// Creates a message digest with the specified algorithm name.
		/// </summary>
		/// <param name="algorithm"> the standard name of the digest algorithm.
		/// See the MessageDigest section in the <a href=
		/// "{@docRoot}/../technotes/guides/security/StandardNames.html#MessageDigest">
		/// Java Cryptography Architecture Standard Algorithm Name Documentation</a>
		/// for information about standard algorithm names. </param>
		protected internal MessageDigest(String algorithm)
		{
			this.Algorithm_Renamed = algorithm;
		}

		/// <summary>
		/// Returns a MessageDigest object that implements the specified digest
		/// algorithm.
		/// 
		/// <para> This method traverses the list of registered security Providers,
		/// starting with the most preferred Provider.
		/// A new MessageDigest object encapsulating the
		/// MessageDigestSpi implementation from the first
		/// Provider that supports the specified algorithm is returned.
		/// 
		/// </para>
		/// <para> Note that the list of registered providers may be retrieved via
		/// the <seealso cref="Security#getProviders() Security.getProviders()"/> method.
		/// 
		/// </para>
		/// </summary>
		/// <param name="algorithm"> the name of the algorithm requested.
		/// See the MessageDigest section in the <a href=
		/// "{@docRoot}/../technotes/guides/security/StandardNames.html#MessageDigest">
		/// Java Cryptography Architecture Standard Algorithm Name Documentation</a>
		/// for information about standard algorithm names.
		/// </param>
		/// <returns> a Message Digest object that implements the specified algorithm.
		/// </returns>
		/// <exception cref="NoSuchAlgorithmException"> if no Provider supports a
		///          MessageDigestSpi implementation for the
		///          specified algorithm.
		/// </exception>
		/// <seealso cref= Provider </seealso>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public static MessageDigest getInstance(String algorithm) throws NoSuchAlgorithmException
		public static MessageDigest GetInstance(String algorithm)
		{
			try
			{
				MessageDigest md;
				Object[] objs = Security.GetImpl(algorithm, "MessageDigest", (String)null);
				if (objs[0] is MessageDigest)
				{
					md = (MessageDigest)objs[0];
				}
				else
				{
					md = new Delegate((MessageDigestSpi)objs[0], algorithm);
				}
				md.Provider_Renamed = (Provider)objs[1];

				if (!SkipDebug && Pdebug != null)
				{
					Pdebug.println("MessageDigest." + algorithm + " algorithm from: " + md.Provider_Renamed.Name);
				}

				return md;

			}
			catch (NoSuchProviderException)
			{
				throw new NoSuchAlgorithmException(algorithm + " not found");
			}
		}

		/// <summary>
		/// Returns a MessageDigest object that implements the specified digest
		/// algorithm.
		/// 
		/// <para> A new MessageDigest object encapsulating the
		/// MessageDigestSpi implementation from the specified provider
		/// is returned.  The specified provider must be registered
		/// in the security provider list.
		/// 
		/// </para>
		/// <para> Note that the list of registered providers may be retrieved via
		/// the <seealso cref="Security#getProviders() Security.getProviders()"/> method.
		/// 
		/// </para>
		/// </summary>
		/// <param name="algorithm"> the name of the algorithm requested.
		/// See the MessageDigest section in the <a href=
		/// "{@docRoot}/../technotes/guides/security/StandardNames.html#MessageDigest">
		/// Java Cryptography Architecture Standard Algorithm Name Documentation</a>
		/// for information about standard algorithm names.
		/// </param>
		/// <param name="provider"> the name of the provider.
		/// </param>
		/// <returns> a MessageDigest object that implements the specified algorithm.
		/// </returns>
		/// <exception cref="NoSuchAlgorithmException"> if a MessageDigestSpi
		///          implementation for the specified algorithm is not
		///          available from the specified provider.
		/// </exception>
		/// <exception cref="NoSuchProviderException"> if the specified provider is not
		///          registered in the security provider list.
		/// </exception>
		/// <exception cref="IllegalArgumentException"> if the provider name is null
		///          or empty.
		/// </exception>
		/// <seealso cref= Provider </seealso>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public static MessageDigest getInstance(String algorithm, String provider) throws NoSuchAlgorithmException, NoSuchProviderException
		public static MessageDigest GetInstance(String algorithm, String provider)
		{
			if (provider == null || provider.Length() == 0)
			{
				throw new IllegalArgumentException("missing provider");
			}
			Object[] objs = Security.GetImpl(algorithm, "MessageDigest", provider);
			if (objs[0] is MessageDigest)
			{
				MessageDigest md = (MessageDigest)objs[0];
				md.Provider_Renamed = (Provider)objs[1];
				return md;
			}
			else
			{
				MessageDigest @delegate = new Delegate((MessageDigestSpi)objs[0], algorithm);
				@delegate.Provider_Renamed = (Provider)objs[1];
				return @delegate;
			}
		}

		/// <summary>
		/// Returns a MessageDigest object that implements the specified digest
		/// algorithm.
		/// 
		/// <para> A new MessageDigest object encapsulating the
		/// MessageDigestSpi implementation from the specified Provider
		/// object is returned.  Note that the specified Provider object
		/// does not have to be registered in the provider list.
		/// 
		/// </para>
		/// </summary>
		/// <param name="algorithm"> the name of the algorithm requested.
		/// See the MessageDigest section in the <a href=
		/// "{@docRoot}/../technotes/guides/security/StandardNames.html#MessageDigest">
		/// Java Cryptography Architecture Standard Algorithm Name Documentation</a>
		/// for information about standard algorithm names.
		/// </param>
		/// <param name="provider"> the provider.
		/// </param>
		/// <returns> a MessageDigest object that implements the specified algorithm.
		/// </returns>
		/// <exception cref="NoSuchAlgorithmException"> if a MessageDigestSpi
		///          implementation for the specified algorithm is not available
		///          from the specified Provider object.
		/// </exception>
		/// <exception cref="IllegalArgumentException"> if the specified provider is null.
		/// </exception>
		/// <seealso cref= Provider
		/// 
		/// @since 1.4 </seealso>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public static MessageDigest getInstance(String algorithm, Provider provider) throws NoSuchAlgorithmException
		public static MessageDigest GetInstance(String algorithm, Provider provider)
		{
			if (provider == null)
			{
				throw new IllegalArgumentException("missing provider");
			}
			Object[] objs = Security.GetImpl(algorithm, "MessageDigest", provider);
			if (objs[0] is MessageDigest)
			{
				MessageDigest md = (MessageDigest)objs[0];
				md.Provider_Renamed = (Provider)objs[1];
				return md;
			}
			else
			{
				MessageDigest @delegate = new Delegate((MessageDigestSpi)objs[0], algorithm);
				@delegate.Provider_Renamed = (Provider)objs[1];
				return @delegate;
			}
		}

		/// <summary>
		/// Returns the provider of this message digest object.
		/// </summary>
		/// <returns> the provider of this message digest object </returns>
		public Provider Provider
		{
			get
			{
				return this.Provider_Renamed;
			}
		}

		/// <summary>
		/// Updates the digest using the specified byte.
		/// </summary>
		/// <param name="input"> the byte with which to update the digest. </param>
		public virtual void Update(sbyte input)
		{
			EngineUpdate(input);
			State = IN_PROGRESS;
		}

		/// <summary>
		/// Updates the digest using the specified array of bytes, starting
		/// at the specified offset.
		/// </summary>
		/// <param name="input"> the array of bytes.
		/// </param>
		/// <param name="offset"> the offset to start from in the array of bytes.
		/// </param>
		/// <param name="len"> the number of bytes to use, starting at
		/// {@code offset}. </param>
		public virtual void Update(sbyte[] input, int offset, int len)
		{
			if (input == null)
			{
				throw new IllegalArgumentException("No input buffer given");
			}
			if (input.Length - offset < len)
			{
				throw new IllegalArgumentException("Input buffer too short");
			}
			EngineUpdate(input, offset, len);
			State = IN_PROGRESS;
		}

		/// <summary>
		/// Updates the digest using the specified array of bytes.
		/// </summary>
		/// <param name="input"> the array of bytes. </param>
		public virtual void Update(sbyte[] input)
		{
			EngineUpdate(input, 0, input.Length);
			State = IN_PROGRESS;
		}

		/// <summary>
		/// Update the digest using the specified ByteBuffer. The digest is
		/// updated using the {@code input.remaining()} bytes starting
		/// at {@code input.position()}.
		/// Upon return, the buffer's position will be equal to its limit;
		/// its limit will not have changed.
		/// </summary>
		/// <param name="input"> the ByteBuffer
		/// @since 1.5 </param>
		public void Update(ByteBuffer input)
		{
			if (input == null)
			{
				throw new NullPointerException();
			}
			EngineUpdate(input);
			State = IN_PROGRESS;
		}

		/// <summary>
		/// Completes the hash computation by performing final operations
		/// such as padding. The digest is reset after this call is made.
		/// </summary>
		/// <returns> the array of bytes for the resulting hash value. </returns>
		public virtual sbyte[] Digest()
		{
			/* Resetting is the responsibility of implementors. */
			sbyte[] result = EngineDigest();
			State = INITIAL;
			return result;
		}

		/// <summary>
		/// Completes the hash computation by performing final operations
		/// such as padding. The digest is reset after this call is made.
		/// </summary>
		/// <param name="buf"> output buffer for the computed digest
		/// </param>
		/// <param name="offset"> offset into the output buffer to begin storing the digest
		/// </param>
		/// <param name="len"> number of bytes within buf allotted for the digest
		/// </param>
		/// <returns> the number of bytes placed into {@code buf}
		/// </returns>
		/// <exception cref="DigestException"> if an error occurs. </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public int digest(byte[] buf, int offset, int len) throws DigestException
		public virtual int Digest(sbyte[] buf, int offset, int len)
		{
			if (buf == null)
			{
				throw new IllegalArgumentException("No output buffer given");
			}
			if (buf.Length - offset < len)
			{
				throw new IllegalArgumentException("Output buffer too small for specified offset and length");
			}
			int numBytes = EngineDigest(buf, offset, len);
			State = INITIAL;
			return numBytes;
		}

		/// <summary>
		/// Performs a final update on the digest using the specified array
		/// of bytes, then completes the digest computation. That is, this
		/// method first calls <seealso cref="#update(byte[]) update(input)"/>,
		/// passing the <i>input</i> array to the {@code update} method,
		/// then calls <seealso cref="#digest() digest()"/>.
		/// </summary>
		/// <param name="input"> the input to be updated before the digest is
		/// completed.
		/// </param>
		/// <returns> the array of bytes for the resulting hash value. </returns>
		public virtual sbyte[] Digest(sbyte[] input)
		{
			Update(input);
			return Digest();
		}

		/// <summary>
		/// Returns a string representation of this message digest object.
		/// </summary>
		public override String ToString()
		{
			ByteArrayOutputStream baos = new ByteArrayOutputStream();
			PrintStream p = new PrintStream(baos);
			p.Print(Algorithm_Renamed + " Message Digest from " + Provider_Renamed.Name + ", ");
			switch (State)
			{
			case INITIAL:
				p.Print("<initialized>");
				break;
			case IN_PROGRESS:
				p.Print("<in progress>");
				break;
			}
			p.Println();
			return (baos.ToString());
		}

		/// <summary>
		/// Compares two digests for equality. Does a simple byte compare.
		/// </summary>
		/// <param name="digesta"> one of the digests to compare.
		/// </param>
		/// <param name="digestb"> the other digest to compare.
		/// </param>
		/// <returns> true if the digests are equal, false otherwise. </returns>
		public static bool IsEqual(sbyte[] digesta, sbyte[] digestb)
		{
			if (digesta == digestb)
			{
				return true;
			}
			if (digesta == null || digestb == null)
			{
				return false;
			}
			if (digesta.Length != digestb.Length)
			{
				return false;
			}

			int result = 0;
			// time-constant comparison
			for (int i = 0; i < digesta.Length; i++)
			{
				result |= digesta[i] ^ digestb[i];
			}
			return result == 0;
		}

		/// <summary>
		/// Resets the digest for further use.
		/// </summary>
		public virtual void Reset()
		{
			EngineReset();
			State = INITIAL;
		}

		/// <summary>
		/// Returns a string that identifies the algorithm, independent of
		/// implementation details. The name should be a standard
		/// Java Security name (such as "SHA", "MD5", and so on).
		/// See the MessageDigest section in the <a href=
		/// "{@docRoot}/../technotes/guides/security/StandardNames.html#MessageDigest">
		/// Java Cryptography Architecture Standard Algorithm Name Documentation</a>
		/// for information about standard algorithm names.
		/// </summary>
		/// <returns> the name of the algorithm </returns>
		public String Algorithm
		{
			get
			{
				return this.Algorithm_Renamed;
			}
		}

		/// <summary>
		/// Returns the length of the digest in bytes, or 0 if this operation is
		/// not supported by the provider and the implementation is not cloneable.
		/// </summary>
		/// <returns> the digest length in bytes, or 0 if this operation is not
		/// supported by the provider and the implementation is not cloneable.
		/// 
		/// @since 1.2 </returns>
		public int DigestLength
		{
			get
			{
				int digestLen = EngineGetDigestLength();
				if (digestLen == 0)
				{
					try
					{
						MessageDigest md = (MessageDigest)Clone();
						sbyte[] digest = md.Digest();
						return digest.Length;
					}
					catch (CloneNotSupportedException)
					{
						return digestLen;
					}
				}
				return digestLen;
			}
		}

		/// <summary>
		/// Returns a clone if the implementation is cloneable.
		/// </summary>
		/// <returns> a clone if the implementation is cloneable.
		/// </returns>
		/// <exception cref="CloneNotSupportedException"> if this is called on an
		/// implementation that does not support {@code Cloneable}. </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public Object clone() throws CloneNotSupportedException
		public override Object Clone()
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




		/*
		 * The following class allows providers to extend from MessageDigestSpi
		 * rather than from MessageDigest. It represents a MessageDigest with an
		 * encapsulated, provider-supplied SPI object (of type MessageDigestSpi).
		 * If the provider implementation is an instance of MessageDigestSpi,
		 * the getInstance() methods above return an instance of this class, with
		 * the SPI object encapsulated.
		 *
		 * Note: All SPI methods from the original MessageDigest class have been
		 * moved up the hierarchy into a new class (MessageDigestSpi), which has
		 * been interposed in the hierarchy between the API (MessageDigest)
		 * and its original parent (Object).
		 */

		internal class Delegate : MessageDigest
		{

			// The provider implementation (delegate)
			internal MessageDigestSpi DigestSpi;

			// constructor
			public Delegate(MessageDigestSpi digestSpi, String algorithm) : base(algorithm)
			{
				this.DigestSpi = digestSpi;
			}

			/// <summary>
			/// Returns a clone if the delegate is cloneable.
			/// </summary>
			/// <returns> a clone if the delegate is cloneable.
			/// </returns>
			/// <exception cref="CloneNotSupportedException"> if this is called on a
			/// delegate that does not support {@code Cloneable}. </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public Object clone() throws CloneNotSupportedException
			public override Object Clone()
			{
				if (DigestSpi is Cloneable)
				{
					MessageDigestSpi digestSpiClone = (MessageDigestSpi)DigestSpi.Clone();
					// Because 'algorithm', 'provider', and 'state' are private
					// members of our supertype, we must perform a cast to
					// access them.
					MessageDigest that = new Delegate(digestSpiClone, ((MessageDigest)this).Algorithm_Renamed);
					that.Provider_Renamed = ((MessageDigest)this).Provider_Renamed;
					that.State = ((MessageDigest)this).State;
					return that;
				}
				else
				{
					throw new CloneNotSupportedException();
				}
			}

			protected internal override int EngineGetDigestLength()
			{
				return DigestSpi.EngineGetDigestLength();
			}

			protected internal override void EngineUpdate(sbyte input)
			{
				DigestSpi.EngineUpdate(input);
			}

			protected internal override void EngineUpdate(sbyte[] input, int offset, int len)
			{
				DigestSpi.EngineUpdate(input, offset, len);
			}

			protected internal override void EngineUpdate(ByteBuffer input)
			{
				DigestSpi.EngineUpdate(input);
			}

			protected internal override sbyte[] EngineDigest()
			{
				return DigestSpi.EngineDigest();
			}

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: protected int engineDigest(byte[] buf, int offset, int len) throws DigestException
			protected internal override int EngineDigest(sbyte[] buf, int offset, int len)
			{
					return DigestSpi.EngineDigest(buf, offset, len);
			}

			protected internal override void EngineReset()
			{
				DigestSpi.EngineReset();
			}
		}
	}

}
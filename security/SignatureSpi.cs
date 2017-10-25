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
	/// for the {@code Signature} class, which is used to provide the
	/// functionality of a digital signature algorithm. Digital signatures are used
	/// for authentication and integrity assurance of digital data.
	/// .
	/// <para> All the abstract methods in this class must be implemented by each
	/// cryptographic service provider who wishes to supply the implementation
	/// of a particular signature algorithm.
	/// 
	/// @author Benjamin Renaud
	/// 
	/// 
	/// </para>
	/// </summary>
	/// <seealso cref= Signature </seealso>

	public abstract class SignatureSpi
	{

		/// <summary>
		/// Application-specified source of randomness.
		/// </summary>
		protected internal SecureRandom AppRandom = null;

		/// <summary>
		/// Initializes this signature object with the specified
		/// public key for verification operations.
		/// </summary>
		/// <param name="publicKey"> the public key of the identity whose signature is
		/// going to be verified.
		/// </param>
		/// <exception cref="InvalidKeyException"> if the key is improperly
		/// encoded, parameters are missing, and so on. </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: protected abstract void engineInitVerify(PublicKey publicKey) throws InvalidKeyException;
		protected internal abstract void EngineInitVerify(PublicKey publicKey);

		/// <summary>
		/// Initializes this signature object with the specified
		/// private key for signing operations.
		/// </summary>
		/// <param name="privateKey"> the private key of the identity whose signature
		/// will be generated.
		/// </param>
		/// <exception cref="InvalidKeyException"> if the key is improperly
		/// encoded, parameters are missing, and so on. </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: protected abstract void engineInitSign(PrivateKey privateKey) throws InvalidKeyException;
		protected internal abstract void EngineInitSign(PrivateKey privateKey);

		/// <summary>
		/// Initializes this signature object with the specified
		/// private key and source of randomness for signing operations.
		/// 
		/// <para>This concrete method has been added to this previously-defined
		/// abstract class. (For backwards compatibility, it cannot be abstract.)
		/// 
		/// </para>
		/// </summary>
		/// <param name="privateKey"> the private key of the identity whose signature
		/// will be generated. </param>
		/// <param name="random"> the source of randomness
		/// </param>
		/// <exception cref="InvalidKeyException"> if the key is improperly
		/// encoded, parameters are missing, and so on. </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: protected void engineInitSign(PrivateKey privateKey, SecureRandom random) throws InvalidKeyException
		protected internal virtual void EngineInitSign(PrivateKey privateKey, SecureRandom random)
		{
				this.AppRandom = random;
				EngineInitSign(privateKey);
		}

		/// <summary>
		/// Updates the data to be signed or verified
		/// using the specified byte.
		/// </summary>
		/// <param name="b"> the byte to use for the update.
		/// </param>
		/// <exception cref="SignatureException"> if the engine is not initialized
		/// properly. </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: protected abstract void engineUpdate(byte b) throws SignatureException;
		protected internal abstract void EngineUpdate(sbyte b);

		/// <summary>
		/// Updates the data to be signed or verified, using the
		/// specified array of bytes, starting at the specified offset.
		/// </summary>
		/// <param name="b"> the array of bytes </param>
		/// <param name="off"> the offset to start from in the array of bytes </param>
		/// <param name="len"> the number of bytes to use, starting at offset
		/// </param>
		/// <exception cref="SignatureException"> if the engine is not initialized
		/// properly </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: protected abstract void engineUpdate(byte[] b, int off, int len) throws SignatureException;
		protected internal abstract void EngineUpdate(sbyte[] b, int off, int len);

		/// <summary>
		/// Updates the data to be signed or verified using the specified
		/// ByteBuffer. Processes the {@code data.remaining()} bytes
		/// starting at at {@code data.position()}.
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
			try
			{
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
					sbyte[] b = new sbyte[JCAUtil.getTempArraySize(len)];
					while (len > 0)
					{
						int chunk = System.Math.Min(len, b.Length);
						input.Get(b, 0, chunk);
						EngineUpdate(b, 0, chunk);
						len -= chunk;
					}
				}
			}
			catch (SignatureException e)
			{
				// is specified to only occur when the engine is not initialized
				// this case should never occur as it is caught in Signature.java
				throw new ProviderException("update() failed", e);
			}
		}

		/// <summary>
		/// Returns the signature bytes of all the data
		/// updated so far.
		/// The format of the signature depends on the underlying
		/// signature scheme.
		/// </summary>
		/// <returns> the signature bytes of the signing operation's result.
		/// </returns>
		/// <exception cref="SignatureException"> if the engine is not
		/// initialized properly or if this signature algorithm is unable to
		/// process the input data provided. </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: protected abstract byte[] engineSign() throws SignatureException;
		protected internal abstract sbyte[] EngineSign();

		/// <summary>
		/// Finishes this signature operation and stores the resulting signature
		/// bytes in the provided buffer {@code outbuf}, starting at
		/// {@code offset}.
		/// The format of the signature depends on the underlying
		/// signature scheme.
		/// 
		/// <para>The signature implementation is reset to its initial state
		/// (the state it was in after a call to one of the
		/// {@code engineInitSign} methods)
		/// and can be reused to generate further signatures with the same private
		/// key.
		/// 
		/// This method should be abstract, but we leave it concrete for
		/// binary compatibility.  Knowledgeable providers should override this
		/// method.
		/// 
		/// </para>
		/// </summary>
		/// <param name="outbuf"> buffer for the signature result.
		/// </param>
		/// <param name="offset"> offset into {@code outbuf} where the signature is
		/// stored.
		/// </param>
		/// <param name="len"> number of bytes within {@code outbuf} allotted for the
		/// signature.
		/// Both this default implementation and the SUN provider do not
		/// return partial digests. If the value of this parameter is less
		/// than the actual signature length, this method will throw a
		/// SignatureException.
		/// This parameter is ignored if its value is greater than or equal to
		/// the actual signature length.
		/// </param>
		/// <returns> the number of bytes placed into {@code outbuf}
		/// </returns>
		/// <exception cref="SignatureException"> if the engine is not
		/// initialized properly, if this signature algorithm is unable to
		/// process the input data provided, or if {@code len} is less
		/// than the actual signature length.
		/// 
		/// @since 1.2 </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: protected int engineSign(byte[] outbuf, int offset, int len) throws SignatureException
		protected internal virtual int EngineSign(sbyte[] outbuf, int offset, int len)
		{
			sbyte[] sig = EngineSign();
			if (len < sig.Length)
			{
					throw new SignatureException("partial signatures not returned");
			}
			if (outbuf.Length - offset < sig.Length)
			{
					throw new SignatureException("insufficient space in the output buffer to store the " + "signature");
			}
			System.Array.Copy(sig, 0, outbuf, offset, sig.Length);
			return sig.Length;
		}

		/// <summary>
		/// Verifies the passed-in signature.
		/// </summary>
		/// <param name="sigBytes"> the signature bytes to be verified.
		/// </param>
		/// <returns> true if the signature was verified, false if not.
		/// </returns>
		/// <exception cref="SignatureException"> if the engine is not
		/// initialized properly, the passed-in signature is improperly
		/// encoded or of the wrong type, if this signature algorithm is unable to
		/// process the input data provided, etc. </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: protected abstract boolean engineVerify(byte[] sigBytes) throws SignatureException;
		protected internal abstract bool EngineVerify(sbyte[] sigBytes);

		/// <summary>
		/// Verifies the passed-in signature in the specified array
		/// of bytes, starting at the specified offset.
		/// 
		/// <para> Note: Subclasses should overwrite the default implementation.
		/// 
		/// 
		/// </para>
		/// </summary>
		/// <param name="sigBytes"> the signature bytes to be verified. </param>
		/// <param name="offset"> the offset to start from in the array of bytes. </param>
		/// <param name="length"> the number of bytes to use, starting at offset.
		/// </param>
		/// <returns> true if the signature was verified, false if not.
		/// </returns>
		/// <exception cref="SignatureException"> if the engine is not
		/// initialized properly, the passed-in signature is improperly
		/// encoded or of the wrong type, if this signature algorithm is unable to
		/// process the input data provided, etc.
		/// @since 1.4 </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: protected boolean engineVerify(byte[] sigBytes, int offset, int length) throws SignatureException
		protected internal virtual bool EngineVerify(sbyte[] sigBytes, int offset, int length)
		{
			sbyte[] sigBytesCopy = new sbyte[length];
			System.Array.Copy(sigBytes, offset, sigBytesCopy, 0, length);
			return EngineVerify(sigBytesCopy);
		}

		/// <summary>
		/// Sets the specified algorithm parameter to the specified
		/// value. This method supplies a general-purpose mechanism through
		/// which it is possible to set the various parameters of this object.
		/// A parameter may be any settable parameter for the algorithm, such as
		/// a parameter size, or a source of random bits for signature generation
		/// (if appropriate), or an indication of whether or not to perform
		/// a specific but optional computation. A uniform algorithm-specific
		/// naming scheme for each parameter is desirable but left unspecified
		/// at this time.
		/// </summary>
		/// <param name="param"> the string identifier of the parameter.
		/// </param>
		/// <param name="value"> the parameter value.
		/// </param>
		/// <exception cref="InvalidParameterException"> if {@code param} is an
		/// invalid parameter for this signature algorithm engine,
		/// the parameter is already set
		/// and cannot be set again, a security exception occurs, and so on.
		/// </exception>
		/// @deprecated Replaced by {@link
		/// #engineSetParameter(java.security.spec.AlgorithmParameterSpec)
		/// engineSetParameter}. 
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: @Deprecated("Replaced by {@link") protected abstract void engineSetParameter(String param, Object value) throws InvalidParameterException;
		[Obsolete("Replaced by {@link")]
		protected internal abstract void EngineSetParameter(String param, Object value);

		/// <summary>
		/// <para>This method is overridden by providers to initialize
		/// this signature engine with the specified parameter set.
		/// 
		/// </para>
		/// </summary>
		/// <param name="params"> the parameters
		/// </param>
		/// <exception cref="UnsupportedOperationException"> if this method is not
		/// overridden by a provider
		/// </exception>
		/// <exception cref="InvalidAlgorithmParameterException"> if this method is
		/// overridden by a provider and the given parameters
		/// are inappropriate for this signature engine </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: protected void engineSetParameter(java.security.spec.AlgorithmParameterSpec params) throws InvalidAlgorithmParameterException
		protected internal virtual void EngineSetParameter(AlgorithmParameterSpec @params)
		{
				throw new UnsupportedOperationException();
		}

		/// <summary>
		/// <para>This method is overridden by providers to return the
		/// parameters used with this signature engine, or null
		/// if this signature engine does not use any parameters.
		/// 
		/// </para>
		/// <para>The returned parameters may be the same that were used to initialize
		/// this signature engine, or may contain a combination of default and
		/// randomly generated parameter values used by the underlying signature
		/// implementation if this signature engine requires algorithm parameters
		/// but was not initialized with any.
		/// 
		/// </para>
		/// </summary>
		/// <returns> the parameters used with this signature engine, or null if this
		/// signature engine does not use any parameters
		/// </returns>
		/// <exception cref="UnsupportedOperationException"> if this method is
		/// not overridden by a provider
		/// @since 1.4 </exception>
		protected internal virtual AlgorithmParameters EngineGetParameters()
		{
			throw new UnsupportedOperationException();
		}

		/// <summary>
		/// Gets the value of the specified algorithm parameter.
		/// This method supplies a general-purpose mechanism through which it
		/// is possible to get the various parameters of this object. A parameter
		/// may be any settable parameter for the algorithm, such as a parameter
		/// size, or  a source of random bits for signature generation (if
		/// appropriate), or an indication of whether or not to perform a
		/// specific but optional computation. A uniform algorithm-specific
		/// naming scheme for each parameter is desirable but left unspecified
		/// at this time.
		/// </summary>
		/// <param name="param"> the string name of the parameter.
		/// </param>
		/// <returns> the object that represents the parameter value, or null if
		/// there is none.
		/// </returns>
		/// <exception cref="InvalidParameterException"> if {@code param} is an
		/// invalid parameter for this engine, or another exception occurs while
		/// trying to get this parameter.
		/// 
		/// @deprecated </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: @Deprecated protected abstract Object engineGetParameter(String param) throws InvalidParameterException;
		[Obsolete]
		protected internal abstract Object EngineGetParameter(String param);

		/// <summary>
		/// Returns a clone if the implementation is cloneable.
		/// </summary>
		/// <returns> a clone if the implementation is cloneable.
		/// </returns>
		/// <exception cref="CloneNotSupportedException"> if this is called
		/// on an implementation that does not support {@code Cloneable}. </exception>
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
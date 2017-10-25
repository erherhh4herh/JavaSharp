using System;

/*
 * Copyright (c) 2003, 2013, Oracle and/or its affiliates. All rights reserved.
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
	/// Standardized representation for serialized Key objects.
	/// 
	/// <para>
	/// 
	/// Note that a serialized Key may contain sensitive information
	/// which should not be exposed in untrusted environments.  See the
	/// <a href="../../../platform/serialization/spec/security.html">
	/// Security Appendix</a>
	/// of the Serialization Specification for more information.
	/// 
	/// </para>
	/// </summary>
	/// <seealso cref= Key </seealso>
	/// <seealso cref= KeyFactory </seealso>
	/// <seealso cref= javax.crypto.spec.SecretKeySpec </seealso>
	/// <seealso cref= java.security.spec.X509EncodedKeySpec </seealso>
	/// <seealso cref= java.security.spec.PKCS8EncodedKeySpec
	/// 
	/// @since 1.5 </seealso>

	[Serializable]
	public class KeyRep
	{

		private const long SerialVersionUID = -4757683898830641853L;

		/// <summary>
		/// Key type.
		/// 
		/// @since 1.5
		/// </summary>
		public enum Type
		{

			/// <summary>
			/// Type for secret keys. </summary>
			SECRET,

			/// <summary>
			/// Type for public keys. </summary>
			PUBLIC,

			/// <summary>
			/// Type for private keys. </summary>
			PRIVATE,

		}

		private const String PKCS8 = "PKCS#8";
		private const String X509 = "X.509";
		private const String RAW = "RAW";

		/// <summary>
		/// Either one of Type.SECRET, Type.PUBLIC, or Type.PRIVATE
		/// 
		/// @serial
		/// </summary>
		private Type Type;

		/// <summary>
		/// The Key algorithm
		/// 
		/// @serial
		/// </summary>
		private String Algorithm;

		/// <summary>
		/// The Key encoding format
		/// 
		/// @serial
		/// </summary>
		private String Format;

		/// <summary>
		/// The encoded Key bytes
		/// 
		/// @serial
		/// </summary>
		private sbyte[] Encoded;

		/// <summary>
		/// Construct the alternate Key class.
		/// 
		/// <para>
		/// 
		/// </para>
		/// </summary>
		/// <param name="type"> either one of Type.SECRET, Type.PUBLIC, or Type.PRIVATE </param>
		/// <param name="algorithm"> the algorithm returned from
		///          {@code Key.getAlgorithm()} </param>
		/// <param name="format"> the encoding format returned from
		///          {@code Key.getFormat()} </param>
		/// <param name="encoded"> the encoded bytes returned from
		///          {@code Key.getEncoded()}
		/// </param>
		/// <exception cref="NullPointerException">
		///          if type is {@code null},
		///          if algorithm is {@code null},
		///          if format is {@code null},
		///          or if encoded is {@code null} </exception>
		public KeyRep(Type type, String algorithm, String format, sbyte[] encoded)
		{

			if (type == null || algorithm == null || format == null || encoded == null)
			{
				throw new NullPointerException("invalid null input(s)");
			}

			this.Type = type;
			this.Algorithm = algorithm;
			this.Format = format.ToUpperCase(Locale.ENGLISH);
			this.Encoded = encoded.clone();
		}

		/// <summary>
		/// Resolve the Key object.
		/// 
		/// <para> This method supports three Type/format combinations:
		/// <ul>
		/// <li> Type.SECRET/"RAW" - returns a SecretKeySpec object
		/// constructed using encoded key bytes and algorithm
		/// <li> Type.PUBLIC/"X.509" - gets a KeyFactory instance for
		/// the key algorithm, constructs an X509EncodedKeySpec with the
		/// encoded key bytes, and generates a public key from the spec
		/// <li> Type.PRIVATE/"PKCS#8" - gets a KeyFactory instance for
		/// the key algorithm, constructs a PKCS8EncodedKeySpec with the
		/// encoded key bytes, and generates a private key from the spec
		/// </ul>
		/// 
		/// </para>
		/// <para>
		/// 
		/// </para>
		/// </summary>
		/// <returns> the resolved Key object
		/// </returns>
		/// <exception cref="ObjectStreamException"> if the Type/format
		///  combination is unrecognized, if the algorithm, key format, or
		///  encoded key bytes are unrecognized/invalid, of if the
		///  resolution of the key fails for any reason </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: protected Object readResolve() throws ObjectStreamException
		protected internal virtual Object ReadResolve()
		{
			try
			{
				if (Type == Type.SECRET && RAW.Equals(Format))
				{
					return new SecretKeySpec(Encoded, Algorithm);
				}
				else if (Type == Type.PUBLIC && X509.Equals(Format))
				{
					KeyFactory f = KeyFactory.GetInstance(Algorithm);
					return f.GeneratePublic(new X509EncodedKeySpec(Encoded));
				}
				else if (Type == Type.PRIVATE && PKCS8.Equals(Format))
				{
					KeyFactory f = KeyFactory.GetInstance(Algorithm);
					return f.GeneratePrivate(new PKCS8EncodedKeySpec(Encoded));
				}
				else
				{
					throw new NotSerializableException("unrecognized type/format combination: " + Type + "/" + Format);
				}
			}
			catch (NotSerializableException nse)
			{
				throw nse;
			}
			catch (Exception e)
			{
				NotSerializableException nse = new NotSerializableException("java.security.Key: " + "[" + Type + "] " + "[" + Algorithm + "] " + "[" + Format + "]");
				nse.InitCause(e);
				throw nse;
			}
		}
	}

}
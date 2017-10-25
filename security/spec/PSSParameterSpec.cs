/*
 * Copyright (c) 2001, 2013, Oracle and/or its affiliates. All rights reserved.
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

namespace java.security.spec
{


	/// <summary>
	/// This class specifies a parameter spec for RSA-PSS signature scheme,
	/// as defined in the
	/// <a href="http://www.ietf.org/rfc/rfc3447.txt">PKCS#1 v2.1</a>
	/// standard.
	/// 
	/// <para>Its ASN.1 definition in PKCS#1 standard is described below:
	/// <pre>
	/// RSASSA-PSS-params ::= SEQUENCE {
	///   hashAlgorithm      [0] OAEP-PSSDigestAlgorithms  DEFAULT sha1,
	///   maskGenAlgorithm   [1] PKCS1MGFAlgorithms  DEFAULT mgf1SHA1,
	///   saltLength         [2] INTEGER  DEFAULT 20,
	///   trailerField       [3] INTEGER  DEFAULT 1
	/// }
	/// </pre>
	/// where
	/// <pre>
	/// OAEP-PSSDigestAlgorithms    ALGORITHM-IDENTIFIER ::= {
	///   { OID id-sha1 PARAMETERS NULL   }|
	///   { OID id-sha224 PARAMETERS NULL   }|
	///   { OID id-sha256 PARAMETERS NULL }|
	///   { OID id-sha384 PARAMETERS NULL }|
	///   { OID id-sha512 PARAMETERS NULL },
	///   ...  -- Allows for future expansion --
	/// }
	/// 
	/// PKCS1MGFAlgorithms    ALGORITHM-IDENTIFIER ::= {
	///   { OID id-mgf1 PARAMETERS OAEP-PSSDigestAlgorithms },
	///   ...  -- Allows for future expansion --
	/// }
	/// </pre>
	/// </para>
	/// <para>Note: the PSSParameterSpec.DEFAULT uses the following:
	///     message digest  -- "SHA-1"
	///     mask generation function (mgf) -- "MGF1"
	///     parameters for mgf -- MGF1ParameterSpec.SHA1
	///     SaltLength   -- 20
	///     TrailerField -- 1
	/// 
	/// </para>
	/// </summary>
	/// <seealso cref= MGF1ParameterSpec </seealso>
	/// <seealso cref= AlgorithmParameterSpec </seealso>
	/// <seealso cref= java.security.Signature
	/// 
	/// @author Valerie Peng
	/// 
	/// 
	/// @since 1.4 </seealso>

	public class PSSParameterSpec : AlgorithmParameterSpec
	{

		private String MdName = "SHA-1";
		private String MgfName = "MGF1";
		private AlgorithmParameterSpec MgfSpec = MGF1ParameterSpec.SHA1;
		private int SaltLen = 20;
		private int TrailerField_Renamed = 1;

		/// <summary>
		/// The PSS parameter set with all default values.
		/// @since 1.5
		/// </summary>
		public static readonly PSSParameterSpec DEFAULT = new PSSParameterSpec();

		/// <summary>
		/// Constructs a new {@code PSSParameterSpec} as defined in
		/// the PKCS #1 standard using the default values.
		/// </summary>
		private PSSParameterSpec()
		{
		}

		/// <summary>
		/// Creates a new {@code PSSParameterSpec} as defined in
		/// the PKCS #1 standard using the specified message digest,
		/// mask generation function, parameters for mask generation
		/// function, salt length, and trailer field values.
		/// </summary>
		/// <param name="mdName"> the algorithm name of the hash function. </param>
		/// <param name="mgfName"> the algorithm name of the mask generation
		/// function. </param>
		/// <param name="mgfSpec"> the parameters for the mask generation
		/// function. If null is specified, null will be returned by
		/// getMGFParameters(). </param>
		/// <param name="saltLen"> the length of salt. </param>
		/// <param name="trailerField"> the value of the trailer field. </param>
		/// <exception cref="NullPointerException"> if {@code mdName},
		/// or {@code mgfName} is null. </exception>
		/// <exception cref="IllegalArgumentException"> if {@code saltLen}
		/// or {@code trailerField} is less than 0.
		/// @since 1.5 </exception>
		public PSSParameterSpec(String mdName, String mgfName, AlgorithmParameterSpec mgfSpec, int saltLen, int trailerField)
		{
			if (mdName == null)
			{
				throw new NullPointerException("digest algorithm is null");
			}
			if (mgfName == null)
			{
				throw new NullPointerException("mask generation function " + "algorithm is null");
			}
			if (saltLen < 0)
			{
				throw new IllegalArgumentException("negative saltLen value: " + saltLen);
			}
			if (trailerField < 0)
			{
				throw new IllegalArgumentException("negative trailerField: " + trailerField);
			}
			this.MdName = mdName;
			this.MgfName = mgfName;
			this.MgfSpec = mgfSpec;
			this.SaltLen = saltLen;
			this.TrailerField_Renamed = trailerField;
		}

		/// <summary>
		/// Creates a new {@code PSSParameterSpec}
		/// using the specified salt length and other default values as
		/// defined in PKCS#1.
		/// </summary>
		/// <param name="saltLen"> the length of salt in bits to be used in PKCS#1
		/// PSS encoding. </param>
		/// <exception cref="IllegalArgumentException"> if {@code saltLen} is
		/// less than 0. </exception>
		public PSSParameterSpec(int saltLen)
		{
			if (saltLen < 0)
			{
				throw new IllegalArgumentException("negative saltLen value: " + saltLen);
			}
			this.SaltLen = saltLen;
		}

		/// <summary>
		/// Returns the message digest algorithm name.
		/// </summary>
		/// <returns> the message digest algorithm name.
		/// @since 1.5 </returns>
		public virtual String DigestAlgorithm
		{
			get
			{
				return MdName;
			}
		}

		/// <summary>
		/// Returns the mask generation function algorithm name.
		/// </summary>
		/// <returns> the mask generation function algorithm name.
		/// 
		/// @since 1.5 </returns>
		public virtual String MGFAlgorithm
		{
			get
			{
				return MgfName;
			}
		}

		/// <summary>
		/// Returns the parameters for the mask generation function.
		/// </summary>
		/// <returns> the parameters for the mask generation function.
		/// @since 1.5 </returns>
		public virtual AlgorithmParameterSpec MGFParameters
		{
			get
			{
				return MgfSpec;
			}
		}

		/// <summary>
		/// Returns the salt length in bits.
		/// </summary>
		/// <returns> the salt length. </returns>
		public virtual int SaltLength
		{
			get
			{
				return SaltLen;
			}
		}

		/// <summary>
		/// Returns the value for the trailer field, i.e. bc in PKCS#1 v2.1.
		/// </summary>
		/// <returns> the value for the trailer field, i.e. bc in PKCS#1 v2.1.
		/// @since 1.5 </returns>
		public virtual int TrailerField
		{
			get
			{
				return TrailerField_Renamed;
			}
		}
	}

}
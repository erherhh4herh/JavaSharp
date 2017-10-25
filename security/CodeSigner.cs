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
	/// This class encapsulates information about a code signer.
	/// It is immutable.
	/// 
	/// @since 1.5
	/// @author Vincent Ryan
	/// </summary>

	[Serializable]
	public sealed class CodeSigner
	{

		private const long SerialVersionUID = 6819288105193937581L;

		/// <summary>
		/// The signer's certificate path.
		/// 
		/// @serial
		/// </summary>
		private CertPath SignerCertPath_Renamed;

		/*
		 * The signature timestamp.
		 *
		 * @serial
		 */
		private Timestamp Timestamp_Renamed;

		/*
		 * Hash code for this code signer.
		 */
		[NonSerialized]
		private int Myhash = -1;

		/// <summary>
		/// Constructs a CodeSigner object.
		/// </summary>
		/// <param name="signerCertPath"> The signer's certificate path.
		///                       It must not be {@code null}. </param>
		/// <param name="timestamp"> A signature timestamp.
		///                  If {@code null} then no timestamp was generated
		///                  for the signature. </param>
		/// <exception cref="NullPointerException"> if {@code signerCertPath} is
		///                              {@code null}. </exception>
		public CodeSigner(CertPath signerCertPath, Timestamp timestamp)
		{
			if (signerCertPath == null)
			{
				throw new NullPointerException();
			}
			this.SignerCertPath_Renamed = signerCertPath;
			this.Timestamp_Renamed = timestamp;
		}

		/// <summary>
		/// Returns the signer's certificate path.
		/// </summary>
		/// <returns> A certificate path. </returns>
		public CertPath SignerCertPath
		{
			get
			{
				return SignerCertPath_Renamed;
			}
		}

		/// <summary>
		/// Returns the signature timestamp.
		/// </summary>
		/// <returns> The timestamp or {@code null} if none is present. </returns>
		public Timestamp Timestamp
		{
			get
			{
				return Timestamp_Renamed;
			}
		}

		/// <summary>
		/// Returns the hash code value for this code signer.
		/// The hash code is generated using the signer's certificate path and the
		/// timestamp, if present.
		/// </summary>
		/// <returns> a hash code value for this code signer. </returns>
		public override int HashCode()
		{
			if (Myhash == -1)
			{
				if (Timestamp_Renamed == null)
				{
					Myhash = SignerCertPath_Renamed.HashCode();
				}
				else
				{
					Myhash = SignerCertPath_Renamed.HashCode() + Timestamp_Renamed.HashCode();
				}
			}
			return Myhash;
		}

		/// <summary>
		/// Tests for equality between the specified object and this
		/// code signer. Two code signers are considered equal if their
		/// signer certificate paths are equal and if their timestamps are equal,
		/// if present in both.
		/// </summary>
		/// <param name="obj"> the object to test for equality with this object.
		/// </param>
		/// <returns> true if the objects are considered equal, false otherwise. </returns>
		public override bool Equals(Object obj)
		{
			if (obj == null || (!(obj is CodeSigner)))
			{
				return false;
			}
			CodeSigner that = (CodeSigner)obj;

			if (this == that)
			{
				return true;
			}
			Timestamp thatTimestamp = that.Timestamp;
			if (Timestamp_Renamed == null)
			{
				if (thatTimestamp != null)
				{
					return false;
				}
			}
			else
			{
				if (thatTimestamp == null || (!Timestamp_Renamed.Equals(thatTimestamp)))
				{
					return false;
				}
			}
			return SignerCertPath_Renamed.Equals(that.SignerCertPath);
		}

		/// <summary>
		/// Returns a string describing this code signer.
		/// </summary>
		/// <returns> A string comprising the signer's certificate and a timestamp,
		///         if present. </returns>
		public override String ToString()
		{
			StringBuffer sb = new StringBuffer();
			sb.Append("(");
			sb.Append("Signer: " + SignerCertPath_Renamed.Certificates[0]);
			if (Timestamp_Renamed != null)
			{
				sb.Append("timestamp: " + Timestamp_Renamed);
			}
			sb.Append(")");
			return sb.ToString();
		}

		// Explicitly reset hash code value to -1
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: private void readObject(ObjectInputStream ois) throws IOException, ClassNotFoundException
		private void ReadObject(ObjectInputStream ois)
		{
		 ois.DefaultReadObject();
		 Myhash = -1;
		}
	}

}
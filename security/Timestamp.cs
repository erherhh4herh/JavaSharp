using System;
using System.Collections.Generic;

/*
 * Copyright (c) 2003, 2011, Oracle and/or its affiliates. All rights reserved.
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
	/// This class encapsulates information about a signed timestamp.
	/// It is immutable.
	/// It includes the timestamp's date and time as well as information about the
	/// Timestamping Authority (TSA) which generated and signed the timestamp.
	/// 
	/// @since 1.5
	/// @author Vincent Ryan
	/// </summary>

	[Serializable]
	public sealed class Timestamp
	{

		private const long SerialVersionUID = -5502683707821851294L;

		/// <summary>
		/// The timestamp's date and time
		/// 
		/// @serial
		/// </summary>
		private DateTime Timestamp_Renamed;

		/// <summary>
		/// The TSA's certificate path.
		/// 
		/// @serial
		/// </summary>
		private CertPath SignerCertPath_Renamed;

		/*
		 * Hash code for this timestamp.
		 */
		[NonSerialized]
		private int Myhash = -1;

		/// <summary>
		/// Constructs a Timestamp.
		/// </summary>
		/// <param name="timestamp"> is the timestamp's date and time. It must not be null. </param>
		/// <param name="signerCertPath"> is the TSA's certificate path. It must not be null. </param>
		/// <exception cref="NullPointerException"> if timestamp or signerCertPath is null. </exception>
		public Timestamp(DateTime timestamp, CertPath signerCertPath)
		{
			if (timestamp == null || signerCertPath == null)
			{
				throw new NullPointerException();
			}
			this.Timestamp_Renamed = new DateTime(timestamp.Ticks); // clone
			this.SignerCertPath_Renamed = signerCertPath;
		}

		/// <summary>
		/// Returns the date and time when the timestamp was generated.
		/// </summary>
		/// <returns> The timestamp's date and time. </returns>
		public DateTime Timestamp
		{
			get
			{
				return new DateTime(Timestamp_Renamed.Ticks); // clone
			}
		}

		/// <summary>
		/// Returns the certificate path for the Timestamping Authority.
		/// </summary>
		/// <returns> The TSA's certificate path. </returns>
		public CertPath SignerCertPath
		{
			get
			{
				return SignerCertPath_Renamed;
			}
		}

		/// <summary>
		/// Returns the hash code value for this timestamp.
		/// The hash code is generated using the date and time of the timestamp
		/// and the TSA's certificate path.
		/// </summary>
		/// <returns> a hash code value for this timestamp. </returns>
		public override int HashCode()
		{
			if (Myhash == -1)
			{
				Myhash = Timestamp_Renamed.HashCode() + SignerCertPath_Renamed.HashCode();
			}
			return Myhash;
		}

		/// <summary>
		/// Tests for equality between the specified object and this
		/// timestamp. Two timestamps are considered equal if the date and time of
		/// their timestamp's and their signer's certificate paths are equal.
		/// </summary>
		/// <param name="obj"> the object to test for equality with this timestamp.
		/// </param>
		/// <returns> true if the timestamp are considered equal, false otherwise. </returns>
		public override bool Equals(Object obj)
		{
			if (obj == null || (!(obj is Timestamp)))
			{
				return false;
			}
			Timestamp that = (Timestamp)obj;

			if (this == that)
			{
				return true;
			}
			return (Timestamp_Renamed.Equals(that.Timestamp) && SignerCertPath_Renamed.Equals(that.SignerCertPath));
		}

		/// <summary>
		/// Returns a string describing this timestamp.
		/// </summary>
		/// <returns> A string comprising the date and time of the timestamp and
		///         its signer's certificate. </returns>
		public override String ToString()
		{
			StringBuffer sb = new StringBuffer();
			sb.Append("(");
			sb.Append("timestamp: " + Timestamp_Renamed);
//JAVA TO C# CONVERTER TODO TASK: Java wildcard generics are not converted to .NET:
//ORIGINAL LINE: java.util.List<? extends java.security.cert.Certificate> certs = signerCertPath.getCertificates();
			IList<?> certs = SignerCertPath_Renamed.Certificates;
			if (certs.Count > 0)
			{
				sb.Append("TSA: " + certs[0]);
			}
			else
			{
				sb.Append("TSA: <empty>");
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
			Timestamp_Renamed = new DateTime(Timestamp_Renamed.Ticks);
		}
	}

}
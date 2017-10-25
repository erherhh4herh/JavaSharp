using System;
using System.Collections.Generic;

/*
 * Copyright (c) 2000, 2013, Oracle and/or its affiliates. All rights reserved.
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

namespace java.security.cert
{


	/// <summary>
	/// An immutable sequence of certificates (a certification path).
	/// <para>
	/// This is an abstract class that defines the methods common to all
	/// {@code CertPath}s. Subclasses can handle different kinds of
	/// certificates (X.509, PGP, etc.).
	/// </para>
	/// <para>
	/// All {@code CertPath} objects have a type, a list of
	/// {@code Certificate}s, and one or more supported encodings. Because the
	/// {@code CertPath} class is immutable, a {@code CertPath} cannot
	/// change in any externally visible way after being constructed. This
	/// stipulation applies to all public fields and methods of this class and any
	/// added or overridden by subclasses.
	/// </para>
	/// <para>
	/// The type is a {@code String} that identifies the type of
	/// {@code Certificate}s in the certification path. For each
	/// certificate {@code cert} in a certification path {@code certPath},
	/// {@code cert.getType().equals(certPath.getType())} must be
	/// {@code true}.
	/// </para>
	/// <para>
	/// The list of {@code Certificate}s is an ordered {@code List} of
	/// zero or more {@code Certificate}s. This {@code List} and all
	/// of the {@code Certificate}s contained in it must be immutable.
	/// </para>
	/// <para>
	/// Each {@code CertPath} object must support one or more encodings
	/// so that the object can be translated into a byte array for storage or
	/// transmission to other parties. Preferably, these encodings should be
	/// well-documented standards (such as PKCS#7). One of the encodings supported
	/// by a {@code CertPath} is considered the default encoding. This
	/// encoding is used if no encoding is explicitly requested (for the
	/// <seealso cref="#getEncoded() getEncoded()"/> method, for instance).
	/// </para>
	/// <para>
	/// All {@code CertPath} objects are also {@code Serializable}.
	/// {@code CertPath} objects are resolved into an alternate
	/// <seealso cref="CertPathRep CertPathRep"/> object during serialization. This allows
	/// a {@code CertPath} object to be serialized into an equivalent
	/// representation regardless of its underlying implementation.
	/// </para>
	/// <para>
	/// {@code CertPath} objects can be created with a
	/// {@code CertificateFactory} or they can be returned by other classes,
	/// such as a {@code CertPathBuilder}.
	/// </para>
	/// <para>
	/// By convention, X.509 {@code CertPath}s (consisting of
	/// {@code X509Certificate}s), are ordered starting with the target
	/// certificate and ending with a certificate issued by the trust anchor. That
	/// is, the issuer of one certificate is the subject of the following one. The
	/// certificate representing the <seealso cref="TrustAnchor TrustAnchor"/> should not be
	/// included in the certification path. Unvalidated X.509 {@code CertPath}s
	/// may not follow these conventions. PKIX {@code CertPathValidator}s will
	/// detect any departure from these conventions that cause the certification
	/// path to be invalid and throw a {@code CertPathValidatorException}.
	/// 
	/// </para>
	/// <para> Every implementation of the Java platform is required to support the
	/// following standard {@code CertPath} encodings:
	/// <ul>
	/// <li>{@code PKCS7}</li>
	/// <li>{@code PkiPath}</li>
	/// </ul>
	/// These encodings are described in the <a href=
	/// "{@docRoot}/../technotes/guides/security/StandardNames.html#CertPathEncodings">
	/// CertPath Encodings section</a> of the
	/// Java Cryptography Architecture Standard Algorithm Name Documentation.
	/// Consult the release documentation for your implementation to see if any
	/// other encodings are supported.
	/// </para>
	/// <para>
	/// <b>Concurrent Access</b>
	/// </para>
	/// <para>
	/// All {@code CertPath} objects must be thread-safe. That is, multiple
	/// threads may concurrently invoke the methods defined in this class on a
	/// single {@code CertPath} object (or more than one) with no
	/// ill effects. This is also true for the {@code List} returned by
	/// {@code CertPath.getCertificates}.
	/// </para>
	/// <para>
	/// Requiring {@code CertPath} objects to be immutable and thread-safe
	/// allows them to be passed around to various pieces of code without worrying
	/// about coordinating access.  Providing this thread-safety is
	/// generally not difficult, since the {@code CertPath} and
	/// {@code List} objects in question are immutable.
	/// 
	/// </para>
	/// </summary>
	/// <seealso cref= CertificateFactory </seealso>
	/// <seealso cref= CertPathBuilder
	/// 
	/// @author      Yassir Elley
	/// @since       1.4 </seealso>
	[Serializable]
	public abstract class CertPath
	{

		private const long SerialVersionUID = 6068470306649138683L;

		private String Type_Renamed; // the type of certificates in this chain

		/// <summary>
		/// Creates a {@code CertPath} of the specified type.
		/// <para>
		/// This constructor is protected because most users should use a
		/// {@code CertificateFactory} to create {@code CertPath}s.
		/// 
		/// </para>
		/// </summary>
		/// <param name="type"> the standard name of the type of
		/// {@code Certificate}s in this path </param>
		protected internal CertPath(String type)
		{
			this.Type_Renamed = type;
		}

		/// <summary>
		/// Returns the type of {@code Certificate}s in this certification
		/// path. This is the same string that would be returned by
		/// <seealso cref="java.security.cert.Certificate#getType() cert.getType()"/>
		/// for all {@code Certificate}s in the certification path.
		/// </summary>
		/// <returns> the type of {@code Certificate}s in this certification
		/// path (never null) </returns>
		public virtual String Type
		{
			get
			{
				return Type_Renamed;
			}
		}

		/// <summary>
		/// Returns an iteration of the encodings supported by this certification
		/// path, with the default encoding first. Attempts to modify the returned
		/// {@code Iterator} via its {@code remove} method result in an
		/// {@code UnsupportedOperationException}.
		/// </summary>
		/// <returns> an {@code Iterator} over the names of the supported
		///         encodings (as Strings) </returns>
		public abstract IEnumerator<String> Encodings {get;}

		/// <summary>
		/// Compares this certification path for equality with the specified
		/// object. Two {@code CertPath}s are equal if and only if their
		/// types are equal and their certificate {@code List}s (and by
		/// implication the {@code Certificate}s in those {@code List}s)
		/// are equal. A {@code CertPath} is never equal to an object that is
		/// not a {@code CertPath}.
		/// <para>
		/// This algorithm is implemented by this method. If it is overridden,
		/// the behavior specified here must be maintained.
		/// 
		/// </para>
		/// </summary>
		/// <param name="other"> the object to test for equality with this certification path </param>
		/// <returns> true if the specified object is equal to this certification path,
		/// false otherwise </returns>
		public override bool Equals(Object other)
		{
			if (this == other)
			{
				return true;
			}

			if (!(other is CertPath))
			{
				return false;
			}

			CertPath otherCP = (CertPath) other;
			if (!otherCP.Type.Equals(Type_Renamed))
			{
				return false;
			}

//JAVA TO C# CONVERTER TODO TASK: Java wildcard generics are not converted to .NET:
//ORIGINAL LINE: java.util.List<? extends Certificate> thisCertList = this.getCertificates();
			IList<?> thisCertList = this.Certificates;
//JAVA TO C# CONVERTER TODO TASK: Java wildcard generics are not converted to .NET:
//ORIGINAL LINE: java.util.List<? extends Certificate> otherCertList = otherCP.getCertificates();
			IList<?> otherCertList = otherCP.Certificates;
			return (thisCertList.Equals(otherCertList));
		}

		/// <summary>
		/// Returns the hashcode for this certification path. The hash code of
		/// a certification path is defined to be the result of the following
		/// calculation:
		/// <pre>{@code
		///  hashCode = path.getType().hashCode();
		///  hashCode = 31*hashCode + path.getCertificates().hashCode();
		/// }</pre>
		/// This ensures that {@code path1.equals(path2)} implies that
		/// {@code path1.hashCode()==path2.hashCode()} for any two certification
		/// paths, {@code path1} and {@code path2}, as required by the
		/// general contract of {@code Object.hashCode}.
		/// </summary>
		/// <returns> the hashcode value for this certification path </returns>
		public override int HashCode()
		{
			int hashCode = Type_Renamed.HashCode();
			hashCode = 31 * hashCode + Certificates.HashCode();
			return hashCode;
		}

		/// <summary>
		/// Returns a string representation of this certification path.
		/// This calls the {@code toString} method on each of the
		/// {@code Certificate}s in the path.
		/// </summary>
		/// <returns> a string representation of this certification path </returns>
		public override String ToString()
		{
			StringBuffer sb = new StringBuffer();
//JAVA TO C# CONVERTER TODO TASK: Java wildcard generics are not converted to .NET:
//ORIGINAL LINE: java.util.Iterator<? extends Certificate> stringIterator = getCertificates().iterator();
			IEnumerator<?> stringIterator = Certificates.GetEnumerator();

			sb.Append("\n" + Type_Renamed + " Cert Path: length = " + Certificates.Count + ".\n");
			sb.Append("[\n");
			int i = 1;
			while (stringIterator.MoveNext())
			{
				sb.Append("==========================================" + "===============Certificate " + i + " start.\n");
				Certificate stringCert = stringIterator.Current;
				sb.Append(stringCert.ToString());
				sb.Append("\n========================================" + "=================Certificate " + i + " end.\n\n\n");
				i++;
			}

			sb.Append("\n]");
			return sb.ToString();
		}

		/// <summary>
		/// Returns the encoded form of this certification path, using the default
		/// encoding.
		/// </summary>
		/// <returns> the encoded bytes </returns>
		/// <exception cref="CertificateEncodingException"> if an encoding error occurs </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public abstract byte[] getEncoded() throws CertificateEncodingException;
		public abstract sbyte[] Encoded {get;}

		/// <summary>
		/// Returns the encoded form of this certification path, using the
		/// specified encoding.
		/// </summary>
		/// <param name="encoding"> the name of the encoding to use </param>
		/// <returns> the encoded bytes </returns>
		/// <exception cref="CertificateEncodingException"> if an encoding error occurs or
		///   the encoding requested is not supported </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public abstract byte[] getEncoded(String encoding) throws CertificateEncodingException;
		public abstract sbyte[] GetEncoded(String encoding);

		/// <summary>
		/// Returns the list of certificates in this certification path.
		/// The {@code List} returned must be immutable and thread-safe.
		/// </summary>
		/// <returns> an immutable {@code List} of {@code Certificate}s
		///         (may be empty, but not null) </returns>
//JAVA TO C# CONVERTER TODO TASK: Java wildcard generics are not converted to .NET:
//ORIGINAL LINE: public abstract java.util.List<? extends Certificate> getCertificates();
//JAVA TO C# CONVERTER TODO TASK: Java wildcard generics are not converted to .NET:
//ORIGINAL LINE: public abstract java.util.List<? extends Certificate> getCertificates();
		public abstract IList<?> Certificates where ? : Certificate {get;}

		/// <summary>
		/// Replaces the {@code CertPath} to be serialized with a
		/// {@code CertPathRep} object.
		/// </summary>
		/// <returns> the {@code CertPathRep} to be serialized
		/// </returns>
		/// <exception cref="ObjectStreamException"> if a {@code CertPathRep} object
		/// representing this certification path could not be created </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: protected Object writeReplace() throws java.io.ObjectStreamException
		protected internal virtual Object WriteReplace()
		{
			try
			{
				return new CertPathRep(Type_Renamed, Encoded);
			}
			catch (CertificateException ce)
			{
				NotSerializableException nse = new NotSerializableException("java.security.cert.CertPath: " + Type_Renamed);
				nse.InitCause(ce);
				throw nse;
			}
		}

		/// <summary>
		/// Alternate {@code CertPath} class for serialization.
		/// @since 1.4
		/// </summary>
		[Serializable]
		protected internal class CertPathRep
		{

			internal const long SerialVersionUID = 3015633072427920915L;

			/// <summary>
			/// The Certificate type </summary>
			internal String Type;
			/// <summary>
			/// The encoded form of the cert path </summary>
			internal sbyte[] Data;

			/// <summary>
			/// Creates a {@code CertPathRep} with the specified
			/// type and encoded form of a certification path.
			/// </summary>
			/// <param name="type"> the standard name of a {@code CertPath} type </param>
			/// <param name="data"> the encoded form of the certification path </param>
			protected internal CertPathRep(String type, sbyte[] data)
			{
				this.Type = type;
				this.Data = data;
			}

			/// <summary>
			/// Returns a {@code CertPath} constructed from the type and data.
			/// </summary>
			/// <returns> the resolved {@code CertPath} object
			/// </returns>
			/// <exception cref="ObjectStreamException"> if a {@code CertPath} could not
			/// be constructed </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: protected Object readResolve() throws java.io.ObjectStreamException
			protected internal virtual Object ReadResolve()
			{
				try
				{
					CertificateFactory cf = CertificateFactory.GetInstance(Type);
					return cf.GenerateCertPath(new ByteArrayInputStream(Data));
				}
				catch (CertificateException ce)
				{
					NotSerializableException nse = new NotSerializableException("java.security.cert.CertPath: " + Type);
					nse.InitCause(ce);
					throw nse;
				}
			}
		}
	}

}
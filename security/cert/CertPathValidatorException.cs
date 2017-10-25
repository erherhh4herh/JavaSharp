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
	/// An exception indicating one of a variety of problems encountered when
	/// validating a certification path.
	/// <para>
	/// A {@code CertPathValidatorException} provides support for wrapping
	/// exceptions. The <seealso cref="#getCause getCause"/> method returns the throwable,
	/// if any, that caused this exception to be thrown.
	/// </para>
	/// <para>
	/// A {@code CertPathValidatorException} may also include the
	/// certification path that was being validated when the exception was thrown,
	/// the index of the certificate in the certification path that caused the
	/// exception to be thrown, and the reason that caused the failure. Use the
	/// <seealso cref="#getCertPath getCertPath"/>, <seealso cref="#getIndex getIndex"/>, and
	/// <seealso cref="#getReason getReason"/> methods to retrieve this information.
	/// 
	/// </para>
	/// <para>
	/// <b>Concurrent Access</b>
	/// </para>
	/// <para>
	/// Unless otherwise specified, the methods defined in this class are not
	/// thread-safe. Multiple threads that need to access a single
	/// object concurrently should synchronize amongst themselves and
	/// provide the necessary locking. Multiple threads each manipulating
	/// separate objects need not synchronize.
	/// 
	/// </para>
	/// </summary>
	/// <seealso cref= CertPathValidator
	/// 
	/// @since       1.4
	/// @author      Yassir Elley </seealso>
	public class CertPathValidatorException : GeneralSecurityException
	{

		private new const long SerialVersionUID = -3083180014971893139L;

		/// <summary>
		/// @serial the index of the certificate in the certification path
		/// that caused the exception to be thrown
		/// </summary>
		private int Index_Renamed = -1;

		/// <summary>
		/// @serial the {@code CertPath} that was being validated when
		/// the exception was thrown
		/// </summary>
		private CertPath CertPath_Renamed;

		/// <summary>
		/// @serial the reason the validation failed
		/// </summary>
		private Reason Reason_Renamed = BasicReason.UNSPECIFIED;

		/// <summary>
		/// Creates a {@code CertPathValidatorException} with
		/// no detail message.
		/// </summary>
		public CertPathValidatorException() : this(null, null)
		{
		}

		/// <summary>
		/// Creates a {@code CertPathValidatorException} with the given
		/// detail message. A detail message is a {@code String} that
		/// describes this particular exception.
		/// </summary>
		/// <param name="msg"> the detail message </param>
		public CertPathValidatorException(String msg) : this(msg, null)
		{
		}

		/// <summary>
		/// Creates a {@code CertPathValidatorException} that wraps the
		/// specified throwable. This allows any exception to be converted into a
		/// {@code CertPathValidatorException}, while retaining information
		/// about the wrapped exception, which may be useful for debugging. The
		/// detail message is set to ({@code cause==null ? null : cause.toString()})
		/// (which typically contains the class and detail message of
		/// cause).
		/// </summary>
		/// <param name="cause"> the cause (which is saved for later retrieval by the
		/// <seealso cref="#getCause getCause()"/> method). (A {@code null} value is
		/// permitted, and indicates that the cause is nonexistent or unknown.) </param>
		public CertPathValidatorException(Throwable cause) : this((cause == null ? null : cause.ToString()), cause)
		{
		}

		/// <summary>
		/// Creates a {@code CertPathValidatorException} with the specified
		/// detail message and cause.
		/// </summary>
		/// <param name="msg"> the detail message </param>
		/// <param name="cause"> the cause (which is saved for later retrieval by the
		/// <seealso cref="#getCause getCause()"/> method). (A {@code null} value is
		/// permitted, and indicates that the cause is nonexistent or unknown.) </param>
		public CertPathValidatorException(String msg, Throwable cause) : this(msg, cause, null, -1)
		{
		}

		/// <summary>
		/// Creates a {@code CertPathValidatorException} with the specified
		/// detail message, cause, certification path, and index.
		/// </summary>
		/// <param name="msg"> the detail message (or {@code null} if none) </param>
		/// <param name="cause"> the cause (or {@code null} if none) </param>
		/// <param name="certPath"> the certification path that was in the process of
		/// being validated when the error was encountered </param>
		/// <param name="index"> the index of the certificate in the certification path
		/// that caused the error (or -1 if not applicable). Note that
		/// the list of certificates in a {@code CertPath} is zero based. </param>
		/// <exception cref="IndexOutOfBoundsException"> if the index is out of range
		/// {@code (index < -1 || (certPath != null && index >=
		/// certPath.getCertificates().size()) } </exception>
		/// <exception cref="IllegalArgumentException"> if {@code certPath} is
		/// {@code null} and {@code index} is not -1 </exception>
		public CertPathValidatorException(String msg, Throwable cause, CertPath certPath, int index) : this(msg, cause, certPath, index, BasicReason.UNSPECIFIED)
		{
		}

		/// <summary>
		/// Creates a {@code CertPathValidatorException} with the specified
		/// detail message, cause, certification path, index, and reason.
		/// </summary>
		/// <param name="msg"> the detail message (or {@code null} if none) </param>
		/// <param name="cause"> the cause (or {@code null} if none) </param>
		/// <param name="certPath"> the certification path that was in the process of
		/// being validated when the error was encountered </param>
		/// <param name="index"> the index of the certificate in the certification path
		/// that caused the error (or -1 if not applicable). Note that
		/// the list of certificates in a {@code CertPath} is zero based. </param>
		/// <param name="reason"> the reason the validation failed </param>
		/// <exception cref="IndexOutOfBoundsException"> if the index is out of range
		/// {@code (index < -1 || (certPath != null && index >=
		/// certPath.getCertificates().size()) } </exception>
		/// <exception cref="IllegalArgumentException"> if {@code certPath} is
		/// {@code null} and {@code index} is not -1 </exception>
		/// <exception cref="NullPointerException"> if {@code reason} is {@code null}
		/// 
		/// @since 1.7 </exception>
		public CertPathValidatorException(String msg, Throwable cause, CertPath certPath, int index, Reason reason) : base(msg, cause)
		{
			if (certPath == null && index != -1)
			{
				throw new IllegalArgumentException();
			}
			if (index < -1 || (certPath != null && index >= certPath.Certificates.Count))
			{
				throw new IndexOutOfBoundsException();
			}
			if (reason == null)
			{
				throw new NullPointerException("reason can't be null");
			}
			this.CertPath_Renamed = certPath;
			this.Index_Renamed = index;
			this.Reason_Renamed = reason;
		}

		/// <summary>
		/// Returns the certification path that was being validated when
		/// the exception was thrown.
		/// </summary>
		/// <returns> the {@code CertPath} that was being validated when
		/// the exception was thrown (or {@code null} if not specified) </returns>
		public virtual CertPath CertPath
		{
			get
			{
				return this.CertPath_Renamed;
			}
		}

		/// <summary>
		/// Returns the index of the certificate in the certification path
		/// that caused the exception to be thrown. Note that the list of
		/// certificates in a {@code CertPath} is zero based. If no
		/// index has been set, -1 is returned.
		/// </summary>
		/// <returns> the index that has been set, or -1 if none has been set </returns>
		public virtual int Index
		{
			get
			{
				return this.Index_Renamed;
			}
		}

		/// <summary>
		/// Returns the reason that the validation failed. The reason is
		/// associated with the index of the certificate returned by
		/// <seealso cref="#getIndex"/>.
		/// </summary>
		/// <returns> the reason that the validation failed, or
		///    {@code BasicReason.UNSPECIFIED} if a reason has not been
		///    specified
		/// 
		/// @since 1.7 </returns>
		public virtual Reason Reason
		{
			get
			{
				return this.Reason_Renamed;
			}
		}

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: private void readObject(java.io.ObjectInputStream stream) throws ClassNotFoundException, java.io.IOException
		private void ReadObject(ObjectInputStream stream)
		{
			stream.DefaultReadObject();
			if (Reason_Renamed == null)
			{
				Reason_Renamed = BasicReason.UNSPECIFIED;
			}
			if (CertPath_Renamed == null && Index_Renamed != -1)
			{
				throw new InvalidObjectException("certpath is null and index != -1");
			}
			if (Index_Renamed < -1 || (CertPath_Renamed != null && Index_Renamed >= CertPath_Renamed.Certificates.Count))
			{
				throw new InvalidObjectException("index out of range");
			}
		}

		/// <summary>
		/// The reason the validation algorithm failed.
		/// 
		/// @since 1.7
		/// </summary>
		public interface Reason
		{
		}


		/// <summary>
		/// The BasicReason enumerates the potential reasons that a certification
		/// path of any type may be invalid.
		/// 
		/// @since 1.7
		/// </summary>
//JAVA TO C# CONVERTER TODO TASK: Enums cannot implement interfaces in .NET:
//ORIGINAL LINE: public static enum BasicReason implements Reason
		public enum BasicReason
		{
			/// <summary>
			/// Unspecified reason.
			/// </summary>
			UNSPECIFIED,

			/// <summary>
			/// The certificate is expired.
			/// </summary>
			EXPIRED,

			/// <summary>
			/// The certificate is not yet valid.
			/// </summary>
			NOT_YET_VALID,

			/// <summary>
			/// The certificate is revoked.
			/// </summary>
			REVOKED,

			/// <summary>
			/// The revocation status of the certificate could not be determined.
			/// </summary>
			UNDETERMINED_REVOCATION_STATUS,

			/// <summary>
			/// The signature is invalid.
			/// </summary>
			INVALID_SIGNATURE,

			/// <summary>
			/// The public key or the signature algorithm has been constrained.
			/// </summary>
			ALGORITHM_CONSTRAINED
		}
	}

}
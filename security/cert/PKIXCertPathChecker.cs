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
	/// An abstract class that performs one or more checks on an
	/// {@code X509Certificate}.
	/// 
	/// <para>A concrete implementation of the {@code PKIXCertPathChecker} class
	/// can be created to extend the PKIX certification path validation algorithm.
	/// For example, an implementation may check for and process a critical private
	/// extension of each certificate in a certification path.
	/// 
	/// </para>
	/// <para>Instances of {@code PKIXCertPathChecker} are passed as parameters
	/// using the <seealso cref="PKIXParameters#setCertPathCheckers setCertPathCheckers"/>
	/// or <seealso cref="PKIXParameters#addCertPathChecker addCertPathChecker"/> methods
	/// of the {@code PKIXParameters} and {@code PKIXBuilderParameters}
	/// class. Each of the {@code PKIXCertPathChecker}s <seealso cref="#check check"/>
	/// methods will be called, in turn, for each certificate processed by a PKIX
	/// {@code CertPathValidator} or {@code CertPathBuilder}
	/// implementation.
	/// 
	/// </para>
	/// <para>A {@code PKIXCertPathChecker} may be called multiple times on
	/// successive certificates in a certification path. Concrete subclasses
	/// are expected to maintain any internal state that may be necessary to
	/// check successive certificates. The <seealso cref="#init init"/> method is used
	/// to initialize the internal state of the checker so that the certificates
	/// of a new certification path may be checked. A stateful implementation
	/// <b>must</b> override the <seealso cref="#clone clone"/> method if necessary in
	/// order to allow a PKIX {@code CertPathBuilder} to efficiently
	/// backtrack and try other paths. In these situations, the
	/// {@code CertPathBuilder} is able to restore prior path validation
	/// states by restoring the cloned {@code PKIXCertPathChecker}s.
	/// 
	/// </para>
	/// <para>The order in which the certificates are presented to the
	/// {@code PKIXCertPathChecker} may be either in the forward direction
	/// (from target to most-trusted CA) or in the reverse direction (from
	/// most-trusted CA to target). A {@code PKIXCertPathChecker} implementation
	/// <b>must</b> support reverse checking (the ability to perform its checks when
	/// it is presented with certificates in the reverse direction) and <b>may</b>
	/// support forward checking (the ability to perform its checks when it is
	/// presented with certificates in the forward direction). The
	/// <seealso cref="#isForwardCheckingSupported isForwardCheckingSupported"/> method
	/// indicates whether forward checking is supported.
	/// </para>
	/// <para>
	/// Additional input parameters required for executing the check may be
	/// specified through constructors of concrete implementations of this class.
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
	/// <seealso cref= PKIXParameters </seealso>
	/// <seealso cref= PKIXBuilderParameters
	/// 
	/// @since       1.4
	/// @author      Yassir Elley
	/// @author      Sean Mullan </seealso>
	public abstract class PKIXCertPathChecker : CertPathChecker, Cloneable
	{

		/// <summary>
		/// Default constructor.
		/// </summary>
		protected internal PKIXCertPathChecker()
		{
		}

		/// <summary>
		/// Initializes the internal state of this {@code PKIXCertPathChecker}.
		/// <para>
		/// The {@code forward} flag specifies the order that
		/// certificates will be passed to the <seealso cref="#check check"/> method
		/// (forward or reverse). A {@code PKIXCertPathChecker} <b>must</b>
		/// support reverse checking and <b>may</b> support forward checking.
		/// 
		/// </para>
		/// </summary>
		/// <param name="forward"> the order that certificates are presented to
		/// the {@code check} method. If {@code true}, certificates
		/// are presented from target to most-trusted CA (forward); if
		/// {@code false}, from most-trusted CA to target (reverse). </param>
		/// <exception cref="CertPathValidatorException"> if this
		/// {@code PKIXCertPathChecker} is unable to check certificates in
		/// the specified order; it should never be thrown if the forward flag
		/// is false since reverse checking must be supported </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: @Override public abstract void init(boolean forward) throws CertPathValidatorException;
		public override abstract void Init(bool forward);

		/// <summary>
		/// Indicates if forward checking is supported. Forward checking refers
		/// to the ability of the {@code PKIXCertPathChecker} to perform
		/// its checks when certificates are presented to the {@code check}
		/// method in the forward direction (from target to most-trusted CA).
		/// </summary>
		/// <returns> {@code true} if forward checking is supported,
		/// {@code false} otherwise </returns>
		public override abstract bool ForwardCheckingSupported {get;}

		/// <summary>
		/// Returns an immutable {@code Set} of X.509 certificate extensions
		/// that this {@code PKIXCertPathChecker} supports (i.e. recognizes, is
		/// able to process), or {@code null} if no extensions are supported.
		/// <para>
		/// Each element of the set is a {@code String} representing the
		/// Object Identifier (OID) of the X.509 extension that is supported.
		/// The OID is represented by a set of nonnegative integers separated by
		/// periods.
		/// </para>
		/// <para>
		/// All X.509 certificate extensions that a {@code PKIXCertPathChecker}
		/// might possibly be able to process should be included in the set.
		/// 
		/// </para>
		/// </summary>
		/// <returns> an immutable {@code Set} of X.509 extension OIDs (in
		/// {@code String} format) supported by this
		/// {@code PKIXCertPathChecker}, or {@code null} if no
		/// extensions are supported </returns>
		public abstract Set<String> SupportedExtensions {get;}

		/// <summary>
		/// Performs the check(s) on the specified certificate using its internal
		/// state and removes any critical extensions that it processes from the
		/// specified collection of OID strings that represent the unresolved
		/// critical extensions. The certificates are presented in the order
		/// specified by the {@code init} method.
		/// </summary>
		/// <param name="cert"> the {@code Certificate} to be checked </param>
		/// <param name="unresolvedCritExts"> a {@code Collection} of OID strings
		/// representing the current set of unresolved critical extensions </param>
		/// <exception cref="CertPathValidatorException"> if the specified certificate does
		/// not pass the check </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public abstract void check(Certificate cert, java.util.Collection<String> unresolvedCritExts) throws CertPathValidatorException;
		public abstract void Check(Certificate cert, ICollection<String> unresolvedCritExts);

		/// <summary>
		/// {@inheritDoc}
		/// 
		/// <para>This implementation calls
		/// {@code check(cert, java.util.Collections.<String>emptySet())}.
		/// </para>
		/// </summary>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: @Override public void check(Certificate cert) throws CertPathValidatorException
		public virtual void Check(Certificate cert)
		{
			Check(cert, System.Linq.Enumerable.Empty<String>());
		}

		/// <summary>
		/// Returns a clone of this object. Calls the {@code Object.clone()}
		/// method.
		/// All subclasses which maintain state must support and
		/// override this method, if necessary.
		/// </summary>
		/// <returns> a copy of this {@code PKIXCertPathChecker} </returns>
		public override Object Clone()
		{
			try
			{
				return base.Clone();
			}
			catch (CloneNotSupportedException e)
			{
				/* Cannot happen */
				throw new InternalError(e.ToString(), e);
			}
		}
	}

}
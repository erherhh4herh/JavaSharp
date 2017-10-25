using System.Collections.Generic;

/*
 * Copyright (c) 2000, 2015, Oracle and/or its affiliates. All rights reserved.
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


	using Debug = sun.security.util.Debug;
	using DerInputStream = sun.security.util.DerInputStream;
	using CRLNumberExtension = sun.security.x509.CRLNumberExtension;
	using X500Name = sun.security.x509.X500Name;

	/// <summary>
	/// A {@code CRLSelector} that selects {@code X509CRLs} that
	/// match all specified criteria. This class is particularly useful when
	/// selecting CRLs from a {@code CertStore} to check revocation status
	/// of a particular certificate.
	/// <para>
	/// When first constructed, an {@code X509CRLSelector} has no criteria
	/// enabled and each of the {@code get} methods return a default
	/// value ({@code null}). Therefore, the <seealso cref="#match match"/> method
	/// would return {@code true} for any {@code X509CRL}. Typically,
	/// several criteria are enabled (by calling <seealso cref="#setIssuers setIssuers"/>
	/// or <seealso cref="#setDateAndTime setDateAndTime"/>, for instance) and then the
	/// {@code X509CRLSelector} is passed to
	/// <seealso cref="CertStore#getCRLs CertStore.getCRLs"/> or some similar
	/// method.
	/// </para>
	/// <para>
	/// Please refer to <a href="http://www.ietf.org/rfc/rfc3280.txt">RFC 3280:
	/// Internet X.509 Public Key Infrastructure Certificate and CRL Profile</a>
	/// for definitions of the X.509 CRL fields and extensions mentioned below.
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
	/// <seealso cref= CRLSelector </seealso>
	/// <seealso cref= X509CRL
	/// 
	/// @since       1.4
	/// @author      Steve Hanna </seealso>
	public class X509CRLSelector : CRLSelector
	{

		static X509CRLSelector()
		{
			CertPathHelperImpl.Initialize();
		}

		private static readonly Debug Debug = Debug.getInstance("certpath");
		private HashSet<Object> IssuerNames_Renamed;
		private HashSet<X500Principal> IssuerX500Principals;
		private System.Numerics.BigInteger MinCRL_Renamed;
		private System.Numerics.BigInteger MaxCRL_Renamed;
		private Date DateAndTime_Renamed;
		private X509Certificate CertChecking;
		private long Skew = 0;

		/// <summary>
		/// Creates an {@code X509CRLSelector}. Initially, no criteria are set
		/// so any {@code X509CRL} will match.
		/// </summary>
		public X509CRLSelector()
		{
		}

		/// <summary>
		/// Sets the issuerNames criterion. The issuer distinguished name in the
		/// {@code X509CRL} must match at least one of the specified
		/// distinguished names. If {@code null}, any issuer distinguished name
		/// will do.
		/// <para>
		/// This method allows the caller to specify, with a single method call,
		/// the complete set of issuer names which {@code X509CRLs} may contain.
		/// The specified value replaces the previous value for the issuerNames
		/// criterion.
		/// </para>
		/// <para>
		/// The {@code names} parameter (if not {@code null}) is a
		/// {@code Collection} of {@code X500Principal}s.
		/// </para>
		/// <para>
		/// Note that the {@code names} parameter can contain duplicate
		/// distinguished names, but they may be removed from the
		/// {@code Collection} of names returned by the
		/// <seealso cref="#getIssuers getIssuers"/> method.
		/// </para>
		/// <para>
		/// Note that a copy is performed on the {@code Collection} to
		/// protect against subsequent modifications.
		/// 
		/// </para>
		/// </summary>
		/// <param name="issuers"> a {@code Collection} of X500Principals
		///   (or {@code null}) </param>
		/// <seealso cref= #getIssuers
		/// @since 1.5 </seealso>
		public virtual Collection<X500Principal> Issuers
		{
			set
			{
				if ((value == null) || value.Empty)
				{
					IssuerNames_Renamed = null;
					IssuerX500Principals = null;
				}
				else
				{
					// clone
					IssuerX500Principals = new HashSet<X500Principal>(value);
					IssuerNames_Renamed = new HashSet<Object>();
					foreach (X500Principal p in IssuerX500Principals)
					{
						IssuerNames_Renamed.Add(p.Encoded);
					}
				}
			}
			get
			{
				if (IssuerX500Principals == null)
				{
					return null;
				}
				return Collections.UnmodifiableCollection(IssuerX500Principals);
			}
		}

		/// <summary>
		/// <strong>Note:</strong> use <seealso cref="#setIssuers(Collection)"/> instead
		/// or only specify the byte array form of distinguished names when using
		/// this method. See <seealso cref="#addIssuerName(String)"/> for more information.
		/// <para>
		/// Sets the issuerNames criterion. The issuer distinguished name in the
		/// {@code X509CRL} must match at least one of the specified
		/// distinguished names. If {@code null}, any issuer distinguished name
		/// will do.
		/// </para>
		/// <para>
		/// This method allows the caller to specify, with a single method call,
		/// the complete set of issuer names which {@code X509CRLs} may contain.
		/// The specified value replaces the previous value for the issuerNames
		/// criterion.
		/// </para>
		/// <para>
		/// The {@code names} parameter (if not {@code null}) is a
		/// {@code Collection} of names. Each name is a {@code String}
		/// or a byte array representing a distinguished name (in
		/// <a href="http://www.ietf.org/rfc/rfc2253.txt">RFC 2253</a> or
		/// ASN.1 DER encoded form, respectively). If {@code null} is supplied
		/// as the value for this argument, no issuerNames check will be performed.
		/// </para>
		/// <para>
		/// Note that the {@code names} parameter can contain duplicate
		/// distinguished names, but they may be removed from the
		/// {@code Collection} of names returned by the
		/// <seealso cref="#getIssuerNames getIssuerNames"/> method.
		/// </para>
		/// <para>
		/// If a name is specified as a byte array, it should contain a single DER
		/// encoded distinguished name, as defined in X.501. The ASN.1 notation for
		/// this structure is as follows.
		/// <pre>{@code
		/// Name ::= CHOICE {
		///   RDNSequence }
		/// 
		/// RDNSequence ::= SEQUENCE OF RelativeDistinguishedName
		/// 
		/// RelativeDistinguishedName ::=
		///   SET SIZE (1 .. MAX) OF AttributeTypeAndValue
		/// 
		/// AttributeTypeAndValue ::= SEQUENCE {
		///   type     AttributeType,
		///   value    AttributeValue }
		/// 
		/// AttributeType ::= OBJECT IDENTIFIER
		/// 
		/// AttributeValue ::= ANY DEFINED BY AttributeType
		/// ....
		/// DirectoryString ::= CHOICE {
		///       teletexString           TeletexString (SIZE (1..MAX)),
		///       printableString         PrintableString (SIZE (1..MAX)),
		///       universalString         UniversalString (SIZE (1..MAX)),
		///       utf8String              UTF8String (SIZE (1.. MAX)),
		///       bmpString               BMPString (SIZE (1..MAX)) }
		/// }</pre>
		/// </para>
		/// <para>
		/// Note that a deep copy is performed on the {@code Collection} to
		/// protect against subsequent modifications.
		/// 
		/// </para>
		/// </summary>
		/// <param name="names"> a {@code Collection} of names (or {@code null}) </param>
		/// <exception cref="IOException"> if a parsing error occurs </exception>
		/// <seealso cref= #getIssuerNames </seealso>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public void setIssuerNames(Collection<?> names) throws java.io.IOException
		public virtual Collection<T1> IssuerNames<T1>
		{
			set
			{
				if (value == null || value.Size() == 0)
				{
					IssuerNames_Renamed = null;
					IssuerX500Principals = null;
				}
				else
				{
					HashSet<Object> tempNames = CloneAndCheckIssuerNames(value);
					// Ensure that we either set both of these or neither
					IssuerX500Principals = ParseIssuerNames(tempNames);
					IssuerNames_Renamed = tempNames;
				}
			}
			get
			{
				if (IssuerNames_Renamed == null)
				{
					return null;
				}
				return CloneIssuerNames(IssuerNames_Renamed);
			}
		}

		/// <summary>
		/// Adds a name to the issuerNames criterion. The issuer distinguished
		/// name in the {@code X509CRL} must match at least one of the specified
		/// distinguished names.
		/// <para>
		/// This method allows the caller to add a name to the set of issuer names
		/// which {@code X509CRLs} may contain. The specified name is added to
		/// any previous value for the issuerNames criterion.
		/// If the specified name is a duplicate, it may be ignored.
		/// 
		/// </para>
		/// </summary>
		/// <param name="issuer"> the issuer as X500Principal
		/// @since 1.5 </param>
		public virtual void AddIssuer(X500Principal issuer)
		{
			AddIssuerNameInternal(issuer.Encoded, issuer);
		}

		/// <summary>
		/// <strong>Denigrated</strong>, use
		/// <seealso cref="#addIssuer(X500Principal)"/> or
		/// <seealso cref="#addIssuerName(byte[])"/> instead. This method should not be
		/// relied on as it can fail to match some CRLs because of a loss of
		/// encoding information in the RFC 2253 String form of some distinguished
		/// names.
		/// <para>
		/// Adds a name to the issuerNames criterion. The issuer distinguished
		/// name in the {@code X509CRL} must match at least one of the specified
		/// distinguished names.
		/// </para>
		/// <para>
		/// This method allows the caller to add a name to the set of issuer names
		/// which {@code X509CRLs} may contain. The specified name is added to
		/// any previous value for the issuerNames criterion.
		/// If the specified name is a duplicate, it may be ignored.
		/// 
		/// </para>
		/// </summary>
		/// <param name="name"> the name in RFC 2253 form </param>
		/// <exception cref="IOException"> if a parsing error occurs </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public void addIssuerName(String name) throws java.io.IOException
		public virtual void AddIssuerName(String name)
		{
			AddIssuerNameInternal(name, (new X500Name(name)).asX500Principal());
		}

		/// <summary>
		/// Adds a name to the issuerNames criterion. The issuer distinguished
		/// name in the {@code X509CRL} must match at least one of the specified
		/// distinguished names.
		/// <para>
		/// This method allows the caller to add a name to the set of issuer names
		/// which {@code X509CRLs} may contain. The specified name is added to
		/// any previous value for the issuerNames criterion. If the specified name
		/// is a duplicate, it may be ignored.
		/// If a name is specified as a byte array, it should contain a single DER
		/// encoded distinguished name, as defined in X.501. The ASN.1 notation for
		/// this structure is as follows.
		/// </para>
		/// <para>
		/// The name is provided as a byte array. This byte array should contain
		/// a single DER encoded distinguished name, as defined in X.501. The ASN.1
		/// notation for this structure appears in the documentation for
		/// <seealso cref="#setIssuerNames setIssuerNames(Collection names)"/>.
		/// </para>
		/// <para>
		/// Note that the byte array supplied here is cloned to protect against
		/// subsequent modifications.
		/// 
		/// </para>
		/// </summary>
		/// <param name="name"> a byte array containing the name in ASN.1 DER encoded form </param>
		/// <exception cref="IOException"> if a parsing error occurs </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public void addIssuerName(byte[] name) throws java.io.IOException
		public virtual void AddIssuerName(sbyte[] name)
		{
			// clone because byte arrays are modifiable
			AddIssuerNameInternal(name.clone(), (new X500Name(name)).asX500Principal());
		}

		/// <summary>
		/// A private method that adds a name (String or byte array) to the
		/// issuerNames criterion. The issuer distinguished
		/// name in the {@code X509CRL} must match at least one of the specified
		/// distinguished names.
		/// </summary>
		/// <param name="name"> the name in string or byte array form </param>
		/// <param name="principal"> the name in X500Principal form </param>
		/// <exception cref="IOException"> if a parsing error occurs </exception>
		private void AddIssuerNameInternal(Object name, X500Principal principal)
		{
			if (IssuerNames_Renamed == null)
			{
				IssuerNames_Renamed = new HashSet<Object>();
			}
			if (IssuerX500Principals == null)
			{
				IssuerX500Principals = new HashSet<X500Principal>();
			}
			IssuerNames_Renamed.Add(name);
			IssuerX500Principals.Add(principal);
		}

		/// <summary>
		/// Clone and check an argument of the form passed to
		/// setIssuerNames. Throw an IOException if the argument is malformed.
		/// </summary>
		/// <param name="names"> a {@code Collection} of names. Each entry is a
		///              String or a byte array (the name, in string or ASN.1
		///              DER encoded form, respectively). {@code null} is
		///              not an acceptable value. </param>
		/// <returns> a deep copy of the specified {@code Collection} </returns>
		/// <exception cref="IOException"> if a parsing error occurs </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: private static HashSet<Object> cloneAndCheckIssuerNames(Collection<?> names) throws java.io.IOException
		private static HashSet<Object> CloneAndCheckIssuerNames(Collection<T1> names)
		{
			HashSet<Object> namesCopy = new HashSet<Object>();
//JAVA TO C# CONVERTER TODO TASK: Java wildcard generics are not converted to .NET:
//ORIGINAL LINE: Iterator<?> i = names.iterator();
			Iterator<?> i = names.Iterator();
			while (i.HasNext())
			{
				Object nameObject = i.Next();
				if (!(nameObject is sbyte []) && !(nameObject is String))
				{
					throw new IOException("name not byte array or String");
				}
				if (nameObject is sbyte [])
				{
					namesCopy.Add(((sbyte []) nameObject).clone());
				}
				else
				{
					namesCopy.Add(nameObject);
				}
			}
			return (namesCopy);
		}

		/// <summary>
		/// Clone an argument of the form passed to setIssuerNames.
		/// Throw a RuntimeException if the argument is malformed.
		/// <para>
		/// This method wraps cloneAndCheckIssuerNames, changing any IOException
		/// into a RuntimeException. This method should be used when the object being
		/// cloned has already been checked, so there should never be any exceptions.
		/// 
		/// </para>
		/// </summary>
		/// <param name="names"> a {@code Collection} of names. Each entry is a
		///              String or a byte array (the name, in string or ASN.1
		///              DER encoded form, respectively). {@code null} is
		///              not an acceptable value. </param>
		/// <returns> a deep copy of the specified {@code Collection} </returns>
		/// <exception cref="RuntimeException"> if a parsing error occurs </exception>
		private static HashSet<Object> CloneIssuerNames(Collection<Object> names)
		{
			try
			{
				return CloneAndCheckIssuerNames(names);
			}
			catch (IOException ioe)
			{
				throw new RuntimeException(ioe);
			}
		}

		/// <summary>
		/// Parse an argument of the form passed to setIssuerNames,
		/// returning a Collection of issuerX500Principals.
		/// Throw an IOException if the argument is malformed.
		/// </summary>
		/// <param name="names"> a {@code Collection} of names. Each entry is a
		///              String or a byte array (the name, in string or ASN.1
		///              DER encoded form, respectively). <Code>Null</Code> is
		///              not an acceptable value. </param>
		/// <returns> a HashSet of issuerX500Principals </returns>
		/// <exception cref="IOException"> if a parsing error occurs </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: private static HashSet<javax.security.auth.x500.X500Principal> parseIssuerNames(Collection<Object> names) throws java.io.IOException
		private static HashSet<X500Principal> ParseIssuerNames(Collection<Object> names)
		{
			HashSet<X500Principal> x500Principals = new HashSet<X500Principal>();
			for (Iterator<Object> t = names.Iterator(); t.HasNext();)
			{
				Object nameObject = t.Next();
				if (nameObject is String)
				{
					x500Principals.Add((new X500Name((String)nameObject)).asX500Principal());
				}
				else
				{
					try
					{
						x500Principals.Add(new X500Principal((sbyte[])nameObject));
					}
					catch (IllegalArgumentException e)
					{
						throw (IOException)(new IOException("Invalid name")).InitCause(e);
					}
				}
			}
			return x500Principals;
		}

		/// <summary>
		/// Sets the minCRLNumber criterion. The {@code X509CRL} must have a
		/// CRL number extension whose value is greater than or equal to the
		/// specified value. If {@code null}, no minCRLNumber check will be
		/// done.
		/// </summary>
		/// <param name="minCRL"> the minimum CRL number accepted (or {@code null}) </param>
		public virtual System.Numerics.BigInteger MinCRLNumber
		{
			set
			{
				this.MinCRL_Renamed = value;
			}
		}

		/// <summary>
		/// Sets the maxCRLNumber criterion. The {@code X509CRL} must have a
		/// CRL number extension whose value is less than or equal to the
		/// specified value. If {@code null}, no maxCRLNumber check will be
		/// done.
		/// </summary>
		/// <param name="maxCRL"> the maximum CRL number accepted (or {@code null}) </param>
		public virtual System.Numerics.BigInteger MaxCRLNumber
		{
			set
			{
				this.MaxCRL_Renamed = value;
			}
		}

		/// <summary>
		/// Sets the dateAndTime criterion. The specified date must be
		/// equal to or later than the value of the thisUpdate component
		/// of the {@code X509CRL} and earlier than the value of the
		/// nextUpdate component. There is no match if the {@code X509CRL}
		/// does not contain a nextUpdate component.
		/// If {@code null}, no dateAndTime check will be done.
		/// <para>
		/// Note that the {@code Date} supplied here is cloned to protect
		/// against subsequent modifications.
		/// 
		/// </para>
		/// </summary>
		/// <param name="dateAndTime"> the {@code Date} to match against
		///                    (or {@code null}) </param>
		/// <seealso cref= #getDateAndTime </seealso>
		public virtual Date DateAndTime
		{
			set
			{
				if (value == null)
				{
					this.DateAndTime_Renamed = null;
				}
				else
				{
					this.DateAndTime_Renamed = new Date(value.Time);
				}
				this.Skew = 0;
			}
			get
			{
				if (DateAndTime_Renamed == null)
				{
					return null;
				}
				return (Date) DateAndTime_Renamed.Clone();
			}
		}

		/// <summary>
		/// Sets the dateAndTime criterion and allows for the specified clock skew
		/// (in milliseconds) when checking against the validity period of the CRL.
		/// </summary>
		internal virtual void SetDateAndTime(Date dateAndTime, long skew)
		{
			this.DateAndTime_Renamed = (dateAndTime == null ? null : new Date(dateAndTime.Time));
			this.Skew = skew;
		}

		/// <summary>
		/// Sets the certificate being checked. This is not a criterion. Rather,
		/// it is optional information that may help a {@code CertStore}
		/// find CRLs that would be relevant when checking revocation for the
		/// specified certificate. If {@code null} is specified, then no
		/// such optional information is provided.
		/// </summary>
		/// <param name="cert"> the {@code X509Certificate} being checked
		///             (or {@code null}) </param>
		/// <seealso cref= #getCertificateChecking </seealso>
		public virtual X509Certificate CertificateChecking
		{
			set
			{
				CertChecking = value;
			}
			get
			{
				return CertChecking;
			}
		}



		/// <summary>
		/// Returns the minCRLNumber criterion. The {@code X509CRL} must have a
		/// CRL number extension whose value is greater than or equal to the
		/// specified value. If {@code null}, no minCRLNumber check will be done.
		/// </summary>
		/// <returns> the minimum CRL number accepted (or {@code null}) </returns>
		public virtual System.Numerics.BigInteger MinCRL
		{
			get
			{
				return MinCRL_Renamed;
			}
		}

		/// <summary>
		/// Returns the maxCRLNumber criterion. The {@code X509CRL} must have a
		/// CRL number extension whose value is less than or equal to the
		/// specified value. If {@code null}, no maxCRLNumber check will be
		/// done.
		/// </summary>
		/// <returns> the maximum CRL number accepted (or {@code null}) </returns>
		public virtual System.Numerics.BigInteger MaxCRL
		{
			get
			{
				return MaxCRL_Renamed;
			}
		}



		/// <summary>
		/// Returns a printable representation of the {@code X509CRLSelector}.
		/// </summary>
		/// <returns> a {@code String} describing the contents of the
		///         {@code X509CRLSelector}. </returns>
		public override String ToString()
		{
			StringBuffer sb = new StringBuffer();
			sb.Append("X509CRLSelector: [\n");
			if (IssuerNames_Renamed != null)
			{
				sb.Append("  IssuerNames:\n");
				Iterator<Object> i = IssuerNames_Renamed.Iterator();
				while (i.HasNext())
				{
					sb.Append("    " + i.Next() + "\n");
				}
			}
			if (MinCRL_Renamed != null)
			{
				sb.Append("  minCRLNumber: " + MinCRL_Renamed + "\n");
			}
			if (MaxCRL_Renamed != null)
			{
				sb.Append("  maxCRLNumber: " + MaxCRL_Renamed + "\n");
			}
			if (DateAndTime_Renamed != null)
			{
				sb.Append("  dateAndTime: " + DateAndTime_Renamed + "\n");
			}
			if (CertChecking != null)
			{
				sb.Append("  Certificate being checked: " + CertChecking + "\n");
			}
			sb.Append("]");
			return sb.ToString();
		}

		/// <summary>
		/// Decides whether a {@code CRL} should be selected.
		/// </summary>
		/// <param name="crl"> the {@code CRL} to be checked </param>
		/// <returns> {@code true} if the {@code CRL} should be selected,
		///         {@code false} otherwise </returns>
		public virtual bool Match(CRL crl)
		{
			if (!(crl is X509CRL))
			{
				return false;
			}
			X509CRL xcrl = (X509CRL)crl;

			/* match on issuer name */
			if (IssuerNames_Renamed != null)
			{
				X500Principal issuer = xcrl.IssuerX500Principal;
				Iterator<X500Principal> i = IssuerX500Principals.Iterator();
				bool found = false;
				while (!found && i.HasNext())
				{
					if (i.Next().Equals(issuer))
					{
						found = true;
					}
				}
				if (!found)
				{
					if (Debug != null)
					{
						Debug.println("X509CRLSelector.match: issuer DNs " + "don't match");
					}
					return false;
				}
			}

			if ((MinCRL_Renamed != null) || (MaxCRL_Renamed != null))
			{
				/* Get CRL number extension from CRL */
				sbyte[] crlNumExtVal = xcrl.GetExtensionValue("2.5.29.20");
				if (crlNumExtVal == null)
				{
					if (Debug != null)
					{
						Debug.println("X509CRLSelector.match: no CRLNumber");
					}
				}
				System.Numerics.BigInteger crlNum;
				try
				{
					DerInputStream @in = new DerInputStream(crlNumExtVal);
					sbyte[] encoded = @in.OctetString;
					CRLNumberExtension crlNumExt = new CRLNumberExtension(false, encoded);
					crlNum = crlNumExt.get(CRLNumberExtension.NUMBER);
				}
				catch (IOException)
				{
					if (Debug != null)
					{
						Debug.println("X509CRLSelector.match: exception in " + "decoding CRL number");
					}
					return false;
				}

				/* match on minCRLNumber */
				if (MinCRL_Renamed != null)
				{
					if (crlNum.CompareTo(MinCRL_Renamed) < 0)
					{
						if (Debug != null)
						{
							Debug.println("X509CRLSelector.match: CRLNumber too small");
						}
						return false;
					}
				}

				/* match on maxCRLNumber */
				if (MaxCRL_Renamed != null)
				{
					if (crlNum.CompareTo(MaxCRL_Renamed) > 0)
					{
						if (Debug != null)
						{
							Debug.println("X509CRLSelector.match: CRLNumber too large");
						}
						return false;
					}
				}
			}


			/* match on dateAndTime */
			if (DateAndTime_Renamed != null)
			{
				Date crlThisUpdate = xcrl.ThisUpdate;
				Date nextUpdate = xcrl.NextUpdate;
				if (nextUpdate == null)
				{
					if (Debug != null)
					{
						Debug.println("X509CRLSelector.match: nextUpdate null");
					}
					return false;
				}
				Date nowPlusSkew = DateAndTime_Renamed;
				Date nowMinusSkew = DateAndTime_Renamed;
				if (Skew > 0)
				{
					nowPlusSkew = new Date(DateAndTime_Renamed.Time + Skew);
					nowMinusSkew = new Date(DateAndTime_Renamed.Time - Skew);
				}

				// Check that the test date is within the validity interval:
				//   [ thisUpdate - MAX_CLOCK_SKEW,
				//     nextUpdate + MAX_CLOCK_SKEW ]
				if (nowMinusSkew.After(nextUpdate) || nowPlusSkew.Before(crlThisUpdate))
				{
					if (Debug != null)
					{
						Debug.println("X509CRLSelector.match: update out-of-range");
					}
					return false;
				}
			}

			return true;
		}

		/// <summary>
		/// Returns a copy of this object.
		/// </summary>
		/// <returns> the copy </returns>
		public virtual Object Clone()
		{
			try
			{
				X509CRLSelector copy = (X509CRLSelector)base.Clone();
				if (IssuerNames_Renamed != null)
				{
					copy.IssuerNames_Renamed = new HashSet<Object>(IssuerNames_Renamed);
					copy.IssuerX500Principals = new HashSet<X500Principal>(IssuerX500Principals);
				}
				return copy;
			}
			catch (CloneNotSupportedException e)
			{
				/* Cannot happen */
				throw new InternalError(e.ToString(), e);
			}
		}
	}

}
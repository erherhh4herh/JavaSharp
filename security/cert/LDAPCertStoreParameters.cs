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
	/// Parameters used as input for the LDAP {@code CertStore} algorithm.
	/// <para>
	/// This class is used to provide necessary configuration parameters (server
	/// name and port number) to implementations of the LDAP {@code CertStore}
	/// algorithm.
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
	/// @since       1.4
	/// @author      Steve Hanna
	/// </para>
	/// </summary>
	/// <seealso cref=         CertStore </seealso>
	public class LDAPCertStoreParameters : CertStoreParameters
	{

		private const int LDAP_DEFAULT_PORT = 389;

		/// <summary>
		/// the port number of the LDAP server
		/// </summary>
		private int Port_Renamed;

		/// <summary>
		/// the DNS name of the LDAP server
		/// </summary>
		private String ServerName_Renamed;

		/// <summary>
		/// Creates an instance of {@code LDAPCertStoreParameters} with the
		/// specified parameter values.
		/// </summary>
		/// <param name="serverName"> the DNS name of the LDAP server </param>
		/// <param name="port"> the port number of the LDAP server </param>
		/// <exception cref="NullPointerException"> if {@code serverName} is
		/// {@code null} </exception>
		public LDAPCertStoreParameters(String serverName, int port)
		{
			if (serverName == null)
			{
				throw new NullPointerException();
			}
			this.ServerName_Renamed = serverName;
			this.Port_Renamed = port;
		}

		/// <summary>
		/// Creates an instance of {@code LDAPCertStoreParameters} with the
		/// specified server name and a default port of 389.
		/// </summary>
		/// <param name="serverName"> the DNS name of the LDAP server </param>
		/// <exception cref="NullPointerException"> if {@code serverName} is
		/// {@code null} </exception>
		public LDAPCertStoreParameters(String serverName) : this(serverName, LDAP_DEFAULT_PORT)
		{
		}

		/// <summary>
		/// Creates an instance of {@code LDAPCertStoreParameters} with the
		/// default parameter values (server name "localhost", port 389).
		/// </summary>
		public LDAPCertStoreParameters() : this("localhost", LDAP_DEFAULT_PORT)
		{
		}

		/// <summary>
		/// Returns the DNS name of the LDAP server.
		/// </summary>
		/// <returns> the name (not {@code null}) </returns>
		public virtual String ServerName
		{
			get
			{
				return ServerName_Renamed;
			}
		}

		/// <summary>
		/// Returns the port number of the LDAP server.
		/// </summary>
		/// <returns> the port number </returns>
		public virtual int Port
		{
			get
			{
				return Port_Renamed;
			}
		}

		/// <summary>
		/// Returns a copy of this object. Changes to the copy will not affect
		/// the original and vice versa.
		/// <para>
		/// Note: this method currently performs a shallow copy of the object
		/// (simply calls {@code Object.clone()}). This may be changed in a
		/// future revision to perform a deep copy if new parameters are added
		/// that should not be shared.
		/// 
		/// </para>
		/// </summary>
		/// <returns> the copy </returns>
		public virtual Object Clone()
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

		/// <summary>
		/// Returns a formatted string describing the parameters.
		/// </summary>
		/// <returns> a formatted string describing the parameters </returns>
		public override String ToString()
		{
			StringBuffer sb = new StringBuffer();
			sb.Append("LDAPCertStoreParameters: [\n");

			sb.Append("  serverName: " + ServerName_Renamed + "\n");
			sb.Append("  port: " + Port_Renamed + "\n");
			sb.Append("]");
			return sb.ToString();
		}
	}

}
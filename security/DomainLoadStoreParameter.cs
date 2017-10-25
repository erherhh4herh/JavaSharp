/*
 * Copyright (c) 2013, Oracle and/or its affiliates. All rights reserved.
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
	/// Configuration data that specifies the keystores in a keystore domain.
	/// A keystore domain is a collection of keystores that are presented as a
	/// single logical keystore. The configuration data is used during
	/// {@code KeyStore}
	/// <seealso cref="KeyStore#load(KeyStore.LoadStoreParameter) load"/> and
	/// <seealso cref="KeyStore#store(KeyStore.LoadStoreParameter) store"/> operations.
	/// <para>
	/// The following syntax is supported for configuration data:
	/// <pre>{@code
	///     domain <domainName> [<property> ...] {
	///         keystore <keystoreName> [<property> ...] ;
	///         ...
	///     };
	///     ...
	/// }</pre>
	/// where {@code domainName} and {@code keystoreName} are identifiers
	/// and {@code property} is a key/value pairing. The key and value are
	/// separated by an 'equals' symbol and the value is enclosed in double
	/// quotes. A property value may be either a printable string or a binary
	/// string of colon-separated pairs of hexadecimal digits. Multi-valued
	/// properties are represented as a comma-separated list of values,
	/// enclosed in square brackets.
	/// See <seealso cref="Arrays#toString(java.lang.Object[])"/>.
	/// </para>
	/// <para>
	/// To ensure that keystore entries are uniquely identified, each
	/// entry's alias is prefixed by its {@code keystoreName} followed
	/// by the entry name separator and each {@code keystoreName} must be
	/// unique within its domain. Entry name prefixes are omitted when
	/// storing a keystore.
	/// </para>
	/// <para>
	/// Properties are context-sensitive: properties that apply to
	/// all the keystores in a domain are located in the domain clause,
	/// and properties that apply only to a specific keystore are located
	/// in that keystore's clause.
	/// Unless otherwise specified, a property in a keystore clause overrides
	/// a property of the same name in the domain clause. All property names
	/// are case-insensitive. The following properties are supported:
	/// <dl>
	/// <dt> {@code keystoreType="<type>"} </dt>
	///     <dd> The keystore type. </dd>
	/// <dt> {@code keystoreURI="<url>"} </dt>
	///     <dd> The keystore location. </dd>
	/// <dt> {@code keystoreProviderName="<name>"} </dt>
	///     <dd> The name of the keystore's JCE provider. </dd>
	/// <dt> {@code keystorePasswordEnv="<environment-variable>"} </dt>
	///     <dd> The environment variable that stores a keystore password.
	///          Alternatively, passwords may be supplied to the constructor
	///          method in a {@code Map<String, ProtectionParameter>}. </dd>
	/// <dt> {@code entryNameSeparator="<separator>"} </dt>
	///     <dd> The separator between a keystore name prefix and an entry name.
	///          When specified, it applies to all the entries in a domain.
	///          Its default value is a space. </dd>
	/// </dl>
	/// </para>
	/// <para>
	/// For example, configuration data for a simple keystore domain
	/// comprising three keystores is shown below:
	/// <pre>
	/// 
	/// domain app1 {
	///     keystore app1-truststore
	///         keystoreURI="file:///app1/etc/truststore.jks";
	/// 
	///     keystore system-truststore
	///         keystoreURI="${java.home}/lib/security/cacerts";
	/// 
	///     keystore app1-keystore
	///         keystoreType="PKCS12"
	///         keystoreURI="file:///app1/etc/keystore.p12";
	/// };
	/// 
	/// </pre>
	/// @since 1.8
	/// </para>
	/// </summary>
	public sealed class DomainLoadStoreParameter : LoadStoreParameter
	{

		private readonly URI Configuration_Renamed;
		private readonly Map<String, ProtectionParameter> ProtectionParams_Renamed;

		/// <summary>
		/// Constructs a DomainLoadStoreParameter for a keystore domain with
		/// the parameters used to protect keystore data.
		/// </summary>
		/// <param name="configuration"> identifier for the domain configuration data.
		///     The name of the target domain should be specified in the
		///     {@code java.net.URI} fragment component when it is necessary
		///     to distinguish between several domain configurations at the
		///     same location.
		/// </param>
		/// <param name="protectionParams"> the map from keystore name to the parameter
		///     used to protect keystore data.
		///     A {@code java.util.Collections.EMPTY_MAP} should be used
		///     when protection parameters are not required or when they have
		///     been specified by properties in the domain configuration data.
		///     It is cloned to prevent subsequent modification.
		/// </param>
		/// <exception cref="NullPointerException"> if {@code configuration} or
		///     {@code protectionParams} is {@code null} </exception>
		public DomainLoadStoreParameter(URI configuration, Map<String, ProtectionParameter> protectionParams)
		{
			if (configuration == null || protectionParams == null)
			{
				throw new NullPointerException("invalid null input");
			}
			this.Configuration_Renamed = configuration;
			this.ProtectionParams_Renamed = Collections.UnmodifiableMap(new HashMap<>(protectionParams));
		}

		/// <summary>
		/// Gets the identifier for the domain configuration data.
		/// </summary>
		/// <returns> the identifier for the configuration data </returns>
		public URI Configuration
		{
			get
			{
				return Configuration_Renamed;
			}
		}

		/// <summary>
		/// Gets the keystore protection parameters for keystores in this
		/// domain.
		/// </summary>
		/// <returns> an unmodifiable map of keystore names to protection
		///     parameters </returns>
		public Map<String, ProtectionParameter> ProtectionParams
		{
			get
			{
				return ProtectionParams_Renamed;
			}
		}

		/// <summary>
		/// Gets the keystore protection parameters for this domain.
		/// Keystore domains do not support a protection parameter.
		/// </summary>
		/// <returns> always returns {@code null} </returns>
		public override KeyStore.ProtectionParameter ProtectionParameter
		{
			get
			{
				return null;
			}
		}
	}

}
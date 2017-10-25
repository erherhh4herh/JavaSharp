/*
 * Copyright (c) 1997, 2013, Oracle and/or its affiliates. All rights reserved.
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

namespace java.net
{

	/// <summary>
	/// The class Authenticator represents an object that knows how to obtain
	/// authentication for a network connection.  Usually, it will do this
	/// by prompting the user for information.
	/// <para>
	/// Applications use this class by overriding {@link
	/// #getPasswordAuthentication()} in a sub-class. This method will
	/// typically use the various getXXX() accessor methods to get information
	/// about the entity requesting authentication. It must then acquire a
	/// username and password either by interacting with the user or through
	/// some other non-interactive means. The credentials are then returned
	/// as a <seealso cref="PasswordAuthentication"/> return value.
	/// </para>
	/// <para>
	/// An instance of this concrete sub-class is then registered
	/// with the system by calling <seealso cref="#setDefault(Authenticator)"/>.
	/// When authentication is required, the system will invoke one of the
	/// requestPasswordAuthentication() methods which in turn will call the
	/// getPasswordAuthentication() method of the registered object.
	/// </para>
	/// <para>
	/// All methods that request authentication have a default implementation
	/// that fails.
	/// 
	/// </para>
	/// </summary>
	/// <seealso cref= java.net.Authenticator#setDefault(java.net.Authenticator) </seealso>
	/// <seealso cref= java.net.Authenticator#getPasswordAuthentication()
	/// 
	/// @author  Bill Foote
	/// @since   1.2 </seealso>

	// There are no abstract methods, but to be useful the user must
	// subclass.
	public abstract class Authenticator
	{

		// The system-wide authenticator object.  See setDefault().
		private static Authenticator TheAuthenticator;

		private String RequestingHost_Renamed;
		private InetAddress RequestingSite_Renamed;
		private int RequestingPort_Renamed;
		private String RequestingProtocol_Renamed;
		private String RequestingPrompt_Renamed;
		private String RequestingScheme_Renamed;
		private URL RequestingURL_Renamed;
		private RequestorType RequestingAuthType;

		/// <summary>
		/// The type of the entity requesting authentication.
		/// 
		/// @since 1.5
		/// </summary>
		public enum RequestorType
		{
			/// <summary>
			/// Entity requesting authentication is a HTTP proxy server.
			/// </summary>
			PROXY,
			/// <summary>
			/// Entity requesting authentication is a HTTP origin server.
			/// </summary>
			SERVER
		}

		private void Reset()
		{
			RequestingHost_Renamed = null;
			RequestingSite_Renamed = null;
			RequestingPort_Renamed = -1;
			RequestingProtocol_Renamed = null;
			RequestingPrompt_Renamed = null;
			RequestingScheme_Renamed = null;
			RequestingURL_Renamed = null;
			RequestingAuthType = RequestorType.SERVER;
		}


		/// <summary>
		/// Sets the authenticator that will be used by the networking code
		/// when a proxy or an HTTP server asks for authentication.
		/// <para>
		/// First, if there is a security manager, its {@code checkPermission}
		/// method is called with a
		/// {@code NetPermission("setDefaultAuthenticator")} permission.
		/// This may result in a java.lang.SecurityException.
		/// 
		/// </para>
		/// </summary>
		/// <param name="a">       The authenticator to be set. If a is {@code null} then
		///                  any previously set authenticator is removed.
		/// </param>
		/// <exception cref="SecurityException">
		///        if a security manager exists and its
		///        {@code checkPermission} method doesn't allow
		///        setting the default authenticator.
		/// </exception>
		/// <seealso cref= SecurityManager#checkPermission </seealso>
		/// <seealso cref= java.net.NetPermission </seealso>
		public static Authenticator Default
		{
			set
			{
				lock (typeof(Authenticator))
				{
					SecurityManager sm = System.SecurityManager;
					if (sm != null)
					{
						NetPermission setDefaultPermission = new NetPermission("setDefaultAuthenticator");
						sm.CheckPermission(setDefaultPermission);
					}
            
					TheAuthenticator = value;
				}
			}
		}

		/// <summary>
		/// Ask the authenticator that has been registered with the system
		/// for a password.
		/// <para>
		/// First, if there is a security manager, its {@code checkPermission}
		/// method is called with a
		/// {@code NetPermission("requestPasswordAuthentication")} permission.
		/// This may result in a java.lang.SecurityException.
		/// 
		/// </para>
		/// </summary>
		/// <param name="addr"> The InetAddress of the site requesting authorization,
		///             or null if not known. </param>
		/// <param name="port"> the port for the requested connection </param>
		/// <param name="protocol"> The protocol that's requesting the connection
		///          (<seealso cref="java.net.Authenticator#getRequestingProtocol()"/>) </param>
		/// <param name="prompt"> A prompt string for the user </param>
		/// <param name="scheme"> The authentication scheme
		/// </param>
		/// <returns> The username/password, or null if one can't be gotten.
		/// </returns>
		/// <exception cref="SecurityException">
		///        if a security manager exists and its
		///        {@code checkPermission} method doesn't allow
		///        the password authentication request.
		/// </exception>
		/// <seealso cref= SecurityManager#checkPermission </seealso>
		/// <seealso cref= java.net.NetPermission </seealso>
		public static PasswordAuthentication RequestPasswordAuthentication(InetAddress addr, int port, String protocol, String prompt, String scheme)
		{

			SecurityManager sm = System.SecurityManager;
			if (sm != null)
			{
				NetPermission requestPermission = new NetPermission("requestPasswordAuthentication");
				sm.CheckPermission(requestPermission);
			}

			Authenticator a = TheAuthenticator;
			if (a == null)
			{
				return null;
			}
			else
			{
				lock (a)
				{
					a.Reset();
					a.RequestingSite_Renamed = addr;
					a.RequestingPort_Renamed = port;
					a.RequestingProtocol_Renamed = protocol;
					a.RequestingPrompt_Renamed = prompt;
					a.RequestingScheme_Renamed = scheme;
					return a.PasswordAuthentication;
				}
			}
		}

		/// <summary>
		/// Ask the authenticator that has been registered with the system
		/// for a password. This is the preferred method for requesting a password
		/// because the hostname can be provided in cases where the InetAddress
		/// is not available.
		/// <para>
		/// First, if there is a security manager, its {@code checkPermission}
		/// method is called with a
		/// {@code NetPermission("requestPasswordAuthentication")} permission.
		/// This may result in a java.lang.SecurityException.
		/// 
		/// </para>
		/// </summary>
		/// <param name="host"> The hostname of the site requesting authentication. </param>
		/// <param name="addr"> The InetAddress of the site requesting authentication,
		///             or null if not known. </param>
		/// <param name="port"> the port for the requested connection. </param>
		/// <param name="protocol"> The protocol that's requesting the connection
		///          (<seealso cref="java.net.Authenticator#getRequestingProtocol()"/>) </param>
		/// <param name="prompt"> A prompt string for the user which identifies the authentication realm. </param>
		/// <param name="scheme"> The authentication scheme
		/// </param>
		/// <returns> The username/password, or null if one can't be gotten.
		/// </returns>
		/// <exception cref="SecurityException">
		///        if a security manager exists and its
		///        {@code checkPermission} method doesn't allow
		///        the password authentication request.
		/// </exception>
		/// <seealso cref= SecurityManager#checkPermission </seealso>
		/// <seealso cref= java.net.NetPermission
		/// @since 1.4 </seealso>
		public static PasswordAuthentication RequestPasswordAuthentication(String host, InetAddress addr, int port, String protocol, String prompt, String scheme)
		{

			SecurityManager sm = System.SecurityManager;
			if (sm != null)
			{
				NetPermission requestPermission = new NetPermission("requestPasswordAuthentication");
				sm.CheckPermission(requestPermission);
			}

			Authenticator a = TheAuthenticator;
			if (a == null)
			{
				return null;
			}
			else
			{
				lock (a)
				{
					a.Reset();
					a.RequestingHost_Renamed = host;
					a.RequestingSite_Renamed = addr;
					a.RequestingPort_Renamed = port;
					a.RequestingProtocol_Renamed = protocol;
					a.RequestingPrompt_Renamed = prompt;
					a.RequestingScheme_Renamed = scheme;
					return a.PasswordAuthentication;
				}
			}
		}

		/// <summary>
		/// Ask the authenticator that has been registered with the system
		/// for a password.
		/// <para>
		/// First, if there is a security manager, its {@code checkPermission}
		/// method is called with a
		/// {@code NetPermission("requestPasswordAuthentication")} permission.
		/// This may result in a java.lang.SecurityException.
		/// 
		/// </para>
		/// </summary>
		/// <param name="host"> The hostname of the site requesting authentication. </param>
		/// <param name="addr"> The InetAddress of the site requesting authorization,
		///             or null if not known. </param>
		/// <param name="port"> the port for the requested connection </param>
		/// <param name="protocol"> The protocol that's requesting the connection
		///          (<seealso cref="java.net.Authenticator#getRequestingProtocol()"/>) </param>
		/// <param name="prompt"> A prompt string for the user </param>
		/// <param name="scheme"> The authentication scheme </param>
		/// <param name="url"> The requesting URL that caused the authentication </param>
		/// <param name="reqType"> The type (server or proxy) of the entity requesting
		///              authentication.
		/// </param>
		/// <returns> The username/password, or null if one can't be gotten.
		/// </returns>
		/// <exception cref="SecurityException">
		///        if a security manager exists and its
		///        {@code checkPermission} method doesn't allow
		///        the password authentication request.
		/// </exception>
		/// <seealso cref= SecurityManager#checkPermission </seealso>
		/// <seealso cref= java.net.NetPermission
		/// 
		/// @since 1.5 </seealso>
		public static PasswordAuthentication RequestPasswordAuthentication(String host, InetAddress addr, int port, String protocol, String prompt, String scheme, URL url, RequestorType reqType)
		{

			SecurityManager sm = System.SecurityManager;
			if (sm != null)
			{
				NetPermission requestPermission = new NetPermission("requestPasswordAuthentication");
				sm.CheckPermission(requestPermission);
			}

			Authenticator a = TheAuthenticator;
			if (a == null)
			{
				return null;
			}
			else
			{
				lock (a)
				{
					a.Reset();
					a.RequestingHost_Renamed = host;
					a.RequestingSite_Renamed = addr;
					a.RequestingPort_Renamed = port;
					a.RequestingProtocol_Renamed = protocol;
					a.RequestingPrompt_Renamed = prompt;
					a.RequestingScheme_Renamed = scheme;
					a.RequestingURL_Renamed = url;
					a.RequestingAuthType = reqType;
					return a.PasswordAuthentication;
				}
			}
		}

		/// <summary>
		/// Gets the {@code hostname} of the
		/// site or proxy requesting authentication, or {@code null}
		/// if not available.
		/// </summary>
		/// <returns> the hostname of the connection requiring authentication, or null
		///          if it's not available.
		/// @since 1.4 </returns>
		protected internal String RequestingHost
		{
			get
			{
				return RequestingHost_Renamed;
			}
		}

		/// <summary>
		/// Gets the {@code InetAddress} of the
		/// site requesting authorization, or {@code null}
		/// if not available.
		/// </summary>
		/// <returns> the InetAddress of the site requesting authorization, or null
		///          if it's not available. </returns>
		protected internal InetAddress RequestingSite
		{
			get
			{
				return RequestingSite_Renamed;
			}
		}

		/// <summary>
		/// Gets the port number for the requested connection. </summary>
		/// <returns> an {@code int} indicating the
		/// port for the requested connection. </returns>
		protected internal int RequestingPort
		{
			get
			{
				return RequestingPort_Renamed;
			}
		}

		/// <summary>
		/// Give the protocol that's requesting the connection.  Often this
		/// will be based on a URL, but in a future JDK it could be, for
		/// example, "SOCKS" for a password-protected SOCKS5 firewall.
		/// </summary>
		/// <returns> the protocol, optionally followed by "/version", where
		///          version is a version number.
		/// </returns>
		/// <seealso cref= java.net.URL#getProtocol() </seealso>
		protected internal String RequestingProtocol
		{
			get
			{
				return RequestingProtocol_Renamed;
			}
		}

		/// <summary>
		/// Gets the prompt string given by the requestor.
		/// </summary>
		/// <returns> the prompt string given by the requestor (realm for
		///          http requests) </returns>
		protected internal String RequestingPrompt
		{
			get
			{
				return RequestingPrompt_Renamed;
			}
		}

		/// <summary>
		/// Gets the scheme of the requestor (the HTTP scheme
		/// for an HTTP firewall, for example).
		/// </summary>
		/// <returns> the scheme of the requestor
		///  </returns>
		protected internal String RequestingScheme
		{
			get
			{
				return RequestingScheme_Renamed;
			}
		}

		/// <summary>
		/// Called when password authorization is needed.  Subclasses should
		/// override the default implementation, which returns null. </summary>
		/// <returns> The PasswordAuthentication collected from the
		///          user, or null if none is provided. </returns>
		protected internal virtual PasswordAuthentication PasswordAuthentication
		{
			get
			{
				return null;
			}
		}

		/// <summary>
		/// Returns the URL that resulted in this
		/// request for authentication.
		/// 
		/// @since 1.5
		/// </summary>
		/// <returns> the requesting URL
		///  </returns>
		protected internal virtual URL RequestingURL
		{
			get
			{
				return RequestingURL_Renamed;
			}
		}

		/// <summary>
		/// Returns whether the requestor is a Proxy or a Server.
		/// 
		/// @since 1.5
		/// </summary>
		/// <returns> the authentication type of the requestor
		///  </returns>
		protected internal virtual RequestorType RequestorType
		{
			get
			{
				return RequestingAuthType;
			}
		}
	}

}
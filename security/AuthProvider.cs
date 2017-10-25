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
	/// This class defines login and logout methods for a provider.
	/// 
	/// <para> While callers may invoke {@code login} directly,
	/// the provider may also invoke {@code login} on behalf of callers
	/// if it determines that a login must be performed
	/// prior to certain operations.
	/// 
	/// @since 1.5
	/// </para>
	/// </summary>
	public abstract class AuthProvider : Provider
	{

		private new const long SerialVersionUID = 4197859053084546461L;

		/// <summary>
		/// Constructs a provider with the specified name, version number,
		/// and information.
		/// </summary>
		/// <param name="name"> the provider name. </param>
		/// <param name="version"> the provider version number. </param>
		/// <param name="info"> a description of the provider and its services. </param>
		protected internal AuthProvider(String name, double version, String info) : base(name, version, info)
		{
		}

		/// <summary>
		/// Log in to this provider.
		/// 
		/// <para> The provider relies on a {@code CallbackHandler}
		/// to obtain authentication information from the caller
		/// (a PIN, for example).  If the caller passes a {@code null}
		/// handler to this method, the provider uses the handler set in the
		/// {@code setCallbackHandler} method.
		/// If no handler was set in that method, the provider queries the
		/// <i>auth.login.defaultCallbackHandler</i> security property
		/// for the fully qualified class name of a default handler implementation.
		/// If the security property is not set,
		/// the provider is assumed to have alternative means
		/// for obtaining authentication information.
		/// 
		/// </para>
		/// </summary>
		/// <param name="subject"> the {@code Subject} which may contain
		///          principals/credentials used for authentication,
		///          or may be populated with additional principals/credentials
		///          after successful authentication has completed.
		///          This parameter may be {@code null}. </param>
		/// <param name="handler"> the {@code CallbackHandler} used by
		///          this provider to obtain authentication information
		///          from the caller, which may be {@code null}
		/// </param>
		/// <exception cref="LoginException"> if the login operation fails </exception>
		/// <exception cref="SecurityException"> if the caller does not pass a
		///  security check for
		///  {@code SecurityPermission("authProvider.name")},
		///  where {@code name} is the value returned by
		///  this provider's {@code getName} method </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public abstract void login(javax.security.auth.Subject subject, javax.security.auth.callback.CallbackHandler handler) throws javax.security.auth.login.LoginException;
		public abstract void Login(Subject subject, CallbackHandler handler);

		/// <summary>
		/// Log out from this provider.
		/// </summary>
		/// <exception cref="LoginException"> if the logout operation fails </exception>
		/// <exception cref="SecurityException"> if the caller does not pass a
		///  security check for
		///  {@code SecurityPermission("authProvider.name")},
		///  where {@code name} is the value returned by
		///  this provider's {@code getName} method </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public abstract void logout() throws javax.security.auth.login.LoginException;
		public abstract void Logout();

		/// <summary>
		/// Set a {@code CallbackHandler}.
		/// 
		/// <para> The provider uses this handler if one is not passed to the
		/// {@code login} method.  The provider also uses this handler
		/// if it invokes {@code login} on behalf of callers.
		/// In either case if a handler is not set via this method,
		/// the provider queries the
		/// <i>auth.login.defaultCallbackHandler</i> security property
		/// for the fully qualified class name of a default handler implementation.
		/// If the security property is not set,
		/// the provider is assumed to have alternative means
		/// for obtaining authentication information.
		/// 
		/// </para>
		/// </summary>
		/// <param name="handler"> a {@code CallbackHandler} for obtaining
		///          authentication information, which may be {@code null}
		/// </param>
		/// <exception cref="SecurityException"> if the caller does not pass a
		///  security check for
		///  {@code SecurityPermission("authProvider.name")},
		///  where {@code name} is the value returned by
		///  this provider's {@code getName} method </exception>
		public abstract CallbackHandler CallbackHandler {set;}
	}

}
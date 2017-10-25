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
	/// The class PasswordAuthentication is a data holder that is used by
	/// Authenticator.  It is simply a repository for a user name and a password.
	/// </summary>
	/// <seealso cref= java.net.Authenticator </seealso>
	/// <seealso cref= java.net.Authenticator#getPasswordAuthentication()
	/// 
	/// @author  Bill Foote
	/// @since   1.2 </seealso>

	public sealed class PasswordAuthentication
	{

		private String UserName_Renamed;
		private char[] Password_Renamed;

		/// <summary>
		/// Creates a new {@code PasswordAuthentication} object from the given
		/// user name and password.
		/// 
		/// <para> Note that the given user password is cloned before it is stored in
		/// the new {@code PasswordAuthentication} object.
		/// 
		/// </para>
		/// </summary>
		/// <param name="userName"> the user name </param>
		/// <param name="password"> the user's password </param>
		public PasswordAuthentication(String userName, char[] password)
		{
			this.UserName_Renamed = userName;
			this.Password_Renamed = password.clone();
		}

		/// <summary>
		/// Returns the user name.
		/// </summary>
		/// <returns> the user name </returns>
		public String UserName
		{
			get
			{
				return UserName_Renamed;
			}
		}

		/// <summary>
		/// Returns the user password.
		/// 
		/// <para> Note that this method returns a reference to the password. It is
		/// the caller's responsibility to zero out the password information after
		/// it is no longer needed.
		/// 
		/// </para>
		/// </summary>
		/// <returns> the password </returns>
		public char[] Password
		{
			get
			{
				return Password_Renamed;
			}
		}
	}

}
/*
 * Copyright (c) 2007, 2009, Oracle and/or its affiliates. All rights reserved.
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

namespace java.nio.file.attribute
{

	/// <summary>
	/// Checked exception thrown when a lookup of <seealso cref="UserPrincipal"/> fails because
	/// the principal does not exist.
	/// 
	/// @since 1.7
	/// </summary>

	public class UserPrincipalNotFoundException : IOException
	{
		internal new const long SerialVersionUID = -5369283889045833024L;

		private readonly String Name_Renamed;

		/// <summary>
		/// Constructs an instance of this class.
		/// </summary>
		/// <param name="name">
		///          the principal name; may be {@code null} </param>
		public UserPrincipalNotFoundException(String name) : base()
		{
			this.Name_Renamed = name;
		}

		/// <summary>
		/// Returns the user principal name if this exception was created with the
		/// user principal name that was not found, otherwise <tt>null</tt>.
		/// </summary>
		/// <returns>  the user principal name or {@code null} </returns>
		public virtual String Name
		{
			get
			{
				return Name_Renamed;
			}
		}
	}

}
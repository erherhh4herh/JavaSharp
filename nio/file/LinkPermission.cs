/*
 * Copyright (c) 2007, 2011, Oracle and/or its affiliates. All rights reserved.
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

namespace java.nio.file
{

	/// <summary>
	/// The {@code Permission} class for link creation operations.
	/// 
	/// <para> The following table provides a summary description of what the permission
	/// allows, and discusses the risks of granting code the permission.
	/// 
	/// <table border=1 cellpadding=5
	///        summary="Table shows permission target name, what the permission allows, and associated risks">
	/// <tr>
	/// <th>Permission Target Name</th>
	/// <th>What the Permission Allows</th>
	/// <th>Risks of Allowing this Permission</th>
	/// </tr>
	/// <tr>
	///   <td>hard</td>
	///   <td> Ability to add an existing file to a directory. This is sometimes
	///   known as creating a link, or hard link. </td>
	///   <td> Extreme care should be taken when granting this permission. It allows
	///   linking to any file or directory in the file system thus allowing the
	///   attacker access to all files. </td>
	/// </tr>
	/// <tr>
	///   <td>symbolic</td>
	///   <td> Ability to create symbolic links. </td>
	///   <td> Extreme care should be taken when granting this permission. It allows
	///   linking to any file or directory in the file system thus allowing the
	///   attacker to access to all files. </td>
	/// </tr>
	/// </table>
	/// 
	/// @since 1.7
	/// 
	/// </para>
	/// </summary>
	/// <seealso cref= Files#createLink </seealso>
	/// <seealso cref= Files#createSymbolicLink </seealso>
	public sealed class LinkPermission : BasicPermission
	{
		internal const long SerialVersionUID = -1441492453772213220L;

		private void CheckName(String name)
		{
			if (!name.Equals("hard") && !name.Equals("symbolic"))
			{
				throw new IllegalArgumentException("name: " + name);
			}
		}

		/// <summary>
		/// Constructs a {@code LinkPermission} with the specified name.
		/// </summary>
		/// <param name="name">
		///          the name of the permission. It must be "hard" or "symbolic".
		/// </param>
		/// <exception cref="IllegalArgumentException">
		///          if name is empty or invalid </exception>
		public LinkPermission(String name) : base(name)
		{
			CheckName(name);
		}

		/// <summary>
		/// Constructs a {@code LinkPermission} with the specified name.
		/// </summary>
		/// <param name="name">
		///          the name of the permission; must be "hard" or "symbolic". </param>
		/// <param name="actions">
		///          the actions for the permission; must be the empty string or
		///          {@code null}
		/// </param>
		/// <exception cref="IllegalArgumentException">
		///          if name is empty or invalid, or actions is a non-empty string </exception>
		public LinkPermission(String name, String actions) : base(name)
		{
			CheckName(name);
			if (actions != null && actions.Length() > 0)
			{
				throw new IllegalArgumentException("actions: " + actions);
			}
		}
	}

}
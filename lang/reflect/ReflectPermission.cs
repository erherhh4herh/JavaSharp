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

namespace java.lang.reflect
{

	/// <summary>
	/// The Permission class for reflective operations.
	/// <P>
	/// The following table
	/// provides a summary description of what the permission allows,
	/// and discusses the risks of granting code the permission.
	/// 
	/// <table border=1 cellpadding=5 summary="Table shows permission target name, what the permission allows, and associated risks">
	/// <tr>
	/// <th>Permission Target Name</th>
	/// <th>What the Permission Allows</th>
	/// <th>Risks of Allowing this Permission</th>
	/// </tr>
	/// 
	/// <tr>
	///   <td>suppressAccessChecks</td>
	///   <td>ability to suppress the standard Java language access checks
	///       on fields and methods in a class; allow access not only public members
	///       but also allow access to default (package) access, protected,
	///       and private members.</td>
	///   <td>This is dangerous in that information (possibly confidential) and
	///       methods normally unavailable would be accessible to malicious code.</td>
	/// </tr>
	/// <tr>
	///   <td>newProxyInPackage.{package name}</td>
	///   <td>ability to create a proxy instance in the specified package of which
	///       the non-public interface that the proxy class implements.</td>
	///   <td>This gives code access to classes in packages to which it normally
	///       does not have access and the dynamic proxy class is in the system
	///       protection domain. Malicious code may use these classes to
	///       help in its attempt to compromise security in the system.</td>
	/// </tr>
	/// 
	/// </table>
	/// </summary>
	/// <seealso cref= java.security.Permission </seealso>
	/// <seealso cref= java.security.BasicPermission </seealso>
	/// <seealso cref= AccessibleObject </seealso>
	/// <seealso cref= Field#get </seealso>
	/// <seealso cref= Field#set </seealso>
	/// <seealso cref= Method#invoke </seealso>
	/// <seealso cref= Constructor#newInstance </seealso>
	/// <seealso cref= Proxy#newProxyInstance
	/// 
	/// @since 1.2 </seealso>
	public sealed class ReflectPermission : java.security.BasicPermission
	{

		private const long SerialVersionUID = 7412737110241507485L;

		/// <summary>
		/// Constructs a ReflectPermission with the specified name.
		/// </summary>
		/// <param name="name"> the name of the ReflectPermission
		/// </param>
		/// <exception cref="NullPointerException"> if {@code name} is {@code null}. </exception>
		/// <exception cref="IllegalArgumentException"> if {@code name} is empty. </exception>
		public ReflectPermission(String name) : base(name)
		{
		}

		/// <summary>
		/// Constructs a ReflectPermission with the specified name and actions.
		/// The actions should be null; they are ignored.
		/// </summary>
		/// <param name="name"> the name of the ReflectPermission
		/// </param>
		/// <param name="actions"> should be null
		/// </param>
		/// <exception cref="NullPointerException"> if {@code name} is {@code null}. </exception>
		/// <exception cref="IllegalArgumentException"> if {@code name} is empty. </exception>
		public ReflectPermission(String name, String actions) : base(name, actions)
		{
		}

	}

}
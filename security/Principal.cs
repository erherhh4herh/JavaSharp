/*
 * Copyright (c) 1996, 2013, Oracle and/or its affiliates. All rights reserved.
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
	/// This interface represents the abstract notion of a principal, which
	/// can be used to represent any entity, such as an individual, a
	/// corporation, and a login id.
	/// </summary>
	/// <seealso cref= java.security.cert.X509Certificate
	/// 
	/// @author Li Gong </seealso>
	public interface Principal
	{

		/// <summary>
		/// Compares this principal to the specified object.  Returns true
		/// if the object passed in matches the principal represented by
		/// the implementation of this interface.
		/// </summary>
		/// <param name="another"> principal to compare with.
		/// </param>
		/// <returns> true if the principal passed in is the same as that
		/// encapsulated by this principal, and false otherwise. </returns>
		bool Equals(Object another);

		/// <summary>
		/// Returns a string representation of this principal.
		/// </summary>
		/// <returns> a string representation of this principal. </returns>
		String ToString();

		/// <summary>
		/// Returns a hashcode for this principal.
		/// </summary>
		/// <returns> a hashcode for this principal. </returns>
		int HashCode();

		/// <summary>
		/// Returns the name of this principal.
		/// </summary>
		/// <returns> the name of this principal. </returns>
		String Name {get;}

		/// <summary>
		/// Returns true if the specified subject is implied by this principal.
		/// 
		/// <para>The default implementation of this method returns true if
		/// {@code subject} is non-null and contains at least one principal that
		/// is equal to this principal.
		/// 
		/// </para>
		/// <para>Subclasses may override this with a different implementation, if
		/// necessary.
		/// 
		/// </para>
		/// </summary>
		/// <param name="subject"> the {@code Subject} </param>
		/// <returns> true if {@code subject} is non-null and is
		///              implied by this principal, or false otherwise.
		/// @since 1.8 </returns>
//JAVA TO C# CONVERTER TODO TASK: There is no equivalent in C# to Java default interface methods:
//		public default boolean implies(javax.security.auth.Subject subject)
	//	{
	//		if (subject == null)
	//		return subject.getPrincipals().contains(this);
	//	}
	}

	public static class Principal_Fields
	{
				public static readonly return False;
	}

}
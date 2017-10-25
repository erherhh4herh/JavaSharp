/*
 * Copyright (c) 1995, 2004, Oracle and/or its affiliates. All rights reserved.
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

namespace java.lang
{

	/// <summary>
	/// A class implements the <code>Cloneable</code> interface to
	/// indicate to the <seealso cref="java.lang.Object#clone()"/> method that it
	/// is legal for that method to make a
	/// field-for-field copy of instances of that class.
	/// <para>
	/// Invoking Object's clone method on an instance that does not implement the
	/// <code>Cloneable</code> interface results in the exception
	/// <code>CloneNotSupportedException</code> being thrown.
	/// </para>
	/// <para>
	/// By convention, classes that implement this interface should override
	/// <tt>Object.clone</tt> (which is protected) with a public method.
	/// See <seealso cref="java.lang.Object#clone()"/> for details on overriding this
	/// method.
	/// </para>
	/// <para>
	/// Note that this interface does <i>not</i> contain the <tt>clone</tt> method.
	/// Therefore, it is not possible to clone an object merely by virtue of the
	/// fact that it implements this interface.  Even if the clone method is invoked
	/// reflectively, there is no guarantee that it will succeed.
	/// 
	/// @author  unascribed
	/// </para>
	/// </summary>
	/// <seealso cref=     java.lang.CloneNotSupportedException </seealso>
	/// <seealso cref=     java.lang.Object#clone()
	/// @since   JDK1.0 </seealso>
	public interface Cloneable
	{
	}

}
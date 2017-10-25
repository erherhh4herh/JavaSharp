/*
 * Copyright (c) 1994, 2005, Oracle and/or its affiliates. All rights reserved.
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

namespace java.util
{

	/// <summary>
	/// An object that implements the Enumeration interface generates a
	/// series of elements, one at a time. Successive calls to the
	/// <code>nextElement</code> method return successive elements of the
	/// series.
	/// <para>
	/// For example, to print all elements of a <tt>Vector&lt;E&gt;</tt> <i>v</i>:
	/// <pre>
	///   for (Enumeration&lt;E&gt; e = v.elements(); e.hasMoreElements();)
	///       System.out.println(e.nextElement());</pre>
	/// </para>
	/// <para>
	/// Methods are provided to enumerate through the elements of a
	/// vector, the keys of a hashtable, and the values in a hashtable.
	/// Enumerations are also used to specify the input streams to a
	/// <code>SequenceInputStream</code>.
	/// </para>
	/// <para>
	/// NOTE: The functionality of this interface is duplicated by the Iterator
	/// interface.  In addition, Iterator adds an optional remove operation, and
	/// has shorter method names.  New implementations should consider using
	/// Iterator in preference to Enumeration.
	/// 
	/// </para>
	/// </summary>
	/// <seealso cref=     java.util.Iterator </seealso>
	/// <seealso cref=     java.io.SequenceInputStream </seealso>
	/// <seealso cref=     java.util.Enumeration#nextElement() </seealso>
	/// <seealso cref=     java.util.Hashtable </seealso>
	/// <seealso cref=     java.util.Hashtable#elements() </seealso>
	/// <seealso cref=     java.util.Hashtable#keys() </seealso>
	/// <seealso cref=     java.util.Vector </seealso>
	/// <seealso cref=     java.util.Vector#elements()
	/// 
	/// @author  Lee Boynton
	/// @since   JDK1.0 </seealso>
	public interface java.util.Iterator<E>
	{
		/// <summary>
		/// Tests if this enumeration contains more elements.
		/// </summary>
		/// <returns>  <code>true</code> if and only if this enumeration object
		///           contains at least one more element to provide;
		///          <code>false</code> otherwise. </returns>
		bool HasMoreElements();

		/// <summary>
		/// Returns the next element of this enumeration if this enumeration
		/// object has at least one more element to provide.
		/// </summary>
		/// <returns>     the next element of this enumeration. </returns>
		/// <exception cref="NoSuchElementException">  if no more elements exist. </exception>
		E NextElement();
	}

}
using System.Collections.Generic;

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

namespace java.util
{

	/// <summary>
	/// The <code>Dictionary</code> class is the abstract parent of any
	/// class, such as <code>Hashtable</code>, which maps keys to values.
	/// Every key and every value is an object. In any one <tt>Dictionary</tt>
	/// object, every key is associated with at most one value. Given a
	/// <tt>Dictionary</tt> and a key, the associated element can be looked up.
	/// Any non-<code>null</code> object can be used as a key and as a value.
	/// <para>
	/// As a rule, the <code>equals</code> method should be used by
	/// implementations of this class to decide if two keys are the same.
	/// </para>
	/// <para>
	/// <strong>NOTE: This class is obsolete.  New implementations should
	/// implement the Map interface, rather than extending this class.</strong>
	/// 
	/// @author  unascribed
	/// </para>
	/// </summary>
	/// <seealso cref=     java.util.Map </seealso>
	/// <seealso cref=     java.lang.Object#equals(java.lang.Object) </seealso>
	/// <seealso cref=     java.lang.Object#hashCode() </seealso>
	/// <seealso cref=     java.util.Hashtable
	/// @since   JDK1.0 </seealso>
	public abstract class Dictionary<K, V>
	{
		/// <summary>
		/// Sole constructor.  (For invocation by subclass constructors, typically
		/// implicit.)
		/// </summary>
		public Dictionary()
		{
		}

		/// <summary>
		/// Returns the number of entries (distinct keys) in this dictionary.
		/// </summary>
		/// <returns>  the number of keys in this dictionary. </returns>
		public abstract int Size();

		/// <summary>
		/// Tests if this dictionary maps no keys to value. The general contract
		/// for the <tt>isEmpty</tt> method is that the result is true if and only
		/// if this dictionary contains no entries.
		/// </summary>
		/// <returns>  <code>true</code> if this dictionary maps no keys to values;
		///          <code>false</code> otherwise. </returns>
		public abstract bool Empty {get;}

		/// <summary>
		/// Returns an enumeration of the keys in this dictionary. The general
		/// contract for the keys method is that an <tt>Enumeration</tt> object
		/// is returned that will generate all the keys for which this dictionary
		/// contains entries.
		/// </summary>
		/// <returns>  an enumeration of the keys in this dictionary. </returns>
		/// <seealso cref=     java.util.Dictionary#elements() </seealso>
		/// <seealso cref=     java.util.Enumeration </seealso>
		public abstract IEnumerator<K> Keys();

		/// <summary>
		/// Returns an enumeration of the values in this dictionary. The general
		/// contract for the <tt>elements</tt> method is that an
		/// <tt>Enumeration</tt> is returned that will generate all the elements
		/// contained in entries in this dictionary.
		/// </summary>
		/// <returns>  an enumeration of the values in this dictionary. </returns>
		/// <seealso cref=     java.util.Dictionary#keys() </seealso>
		/// <seealso cref=     java.util.Enumeration </seealso>
		public abstract IEnumerator<V> Elements();

		/// <summary>
		/// Returns the value to which the key is mapped in this dictionary.
		/// The general contract for the <tt>isEmpty</tt> method is that if this
		/// dictionary contains an entry for the specified key, the associated
		/// value is returned; otherwise, <tt>null</tt> is returned.
		/// </summary>
		/// <returns>  the value to which the key is mapped in this dictionary; </returns>
		/// <param name="key">   a key in this dictionary.
		///          <code>null</code> if the key is not mapped to any value in
		///          this dictionary. </param>
		/// <exception cref="NullPointerException"> if the <tt>key</tt> is <tt>null</tt>. </exception>
		/// <seealso cref=     java.util.Dictionary#put(java.lang.Object, java.lang.Object) </seealso>
		public abstract V Get(Object key);

		/// <summary>
		/// Maps the specified <code>key</code> to the specified
		/// <code>value</code> in this dictionary. Neither the key nor the
		/// value can be <code>null</code>.
		/// <para>
		/// If this dictionary already contains an entry for the specified
		/// <tt>key</tt>, the value already in this dictionary for that
		/// <tt>key</tt> is returned, after modifying the entry to contain the
		/// </para>
		///  new element. <para>If this dictionary does not already have an entry
		///  for the specified <tt>key</tt>, an entry is created for the
		///  specified <tt>key</tt> and <tt>value</tt>, and <tt>null</tt> is
		///  returned.
		/// </para>
		/// <para>
		/// The <code>value</code> can be retrieved by calling the
		/// <code>get</code> method with a <code>key</code> that is equal to
		/// the original <code>key</code>.
		/// 
		/// </para>
		/// </summary>
		/// <param name="key">     the hashtable key. </param>
		/// <param name="value">   the value. </param>
		/// <returns>     the previous value to which the <code>key</code> was mapped
		///             in this dictionary, or <code>null</code> if the key did not
		///             have a previous mapping. </returns>
		/// <exception cref="NullPointerException">  if the <code>key</code> or
		///               <code>value</code> is <code>null</code>. </exception>
		/// <seealso cref=        java.lang.Object#equals(java.lang.Object) </seealso>
		/// <seealso cref=        java.util.Dictionary#get(java.lang.Object) </seealso>
		public abstract V Put(K key, V value);

		/// <summary>
		/// Removes the <code>key</code> (and its corresponding
		/// <code>value</code>) from this dictionary. This method does nothing
		/// if the <code>key</code> is not in this dictionary.
		/// </summary>
		/// <param name="key">   the key that needs to be removed. </param>
		/// <returns>  the value to which the <code>key</code> had been mapped in this
		///          dictionary, or <code>null</code> if the key did not have a
		///          mapping. </returns>
		/// <exception cref="NullPointerException"> if <tt>key</tt> is <tt>null</tt>. </exception>
		public abstract V Remove(Object key);
	}

}
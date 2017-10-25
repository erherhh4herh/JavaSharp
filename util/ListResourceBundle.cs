using System.Collections.Generic;

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

/*
 * (C) Copyright Taligent, Inc. 1996, 1997 - All Rights Reserved
 * (C) Copyright IBM Corp. 1996 - 1998 - All Rights Reserved
 *
 * The original version of this source code and documentation
 * is copyrighted and owned by Taligent, Inc., a wholly-owned
 * subsidiary of IBM. These materials are provided under terms
 * of a License Agreement between Taligent and Sun. This technology
 * is protected by multiple US and International patents.
 *
 * This notice and attribution to Taligent may not be removed.
 * Taligent is a registered trademark of Taligent, Inc.
 *
 */

namespace java.util
{

	using ResourceBundleEnumeration = sun.util.ResourceBundleEnumeration;

	/// <summary>
	/// <code>ListResourceBundle</code> is an abstract subclass of
	/// <code>ResourceBundle</code> that manages resources for a locale
	/// in a convenient and easy to use list. See <code>ResourceBundle</code> for
	/// more information about resource bundles in general.
	/// 
	/// <P>
	/// Subclasses must override <code>getContents</code> and provide an array,
	/// where each item in the array is a pair of objects.
	/// The first element of each pair is the key, which must be a
	/// <code>String</code>, and the second element is the value associated with
	/// that key.
	/// 
	/// <para>
	/// The following <a name="sample">example</a> shows two members of a resource
	/// bundle family with the base name "MyResources".
	/// "MyResources" is the default member of the bundle family, and
	/// "MyResources_fr" is the French member.
	/// These members are based on <code>ListResourceBundle</code>
	/// (a related <a href="PropertyResourceBundle.html#sample">example</a> shows
	/// how you can add a bundle to this family that's based on a properties file).
	/// The keys in this example are of the form "s1" etc. The actual
	/// keys are entirely up to your choice, so long as they are the same as
	/// the keys you use in your program to retrieve the objects from the bundle.
	/// Keys are case-sensitive.
	/// <blockquote>
	/// <pre>
	/// 
	/// public class MyResources extends ListResourceBundle {
	///     protected Object[][] getContents() {
	///         return new Object[][] {
	///         // LOCALIZE THIS
	///             {"s1", "The disk \"{1}\" contains {0}."},  // MessageFormat pattern
	///             {"s2", "1"},                               // location of {0} in pattern
	///             {"s3", "My Disk"},                         // sample disk name
	///             {"s4", "no files"},                        // first ChoiceFormat choice
	///             {"s5", "one file"},                        // second ChoiceFormat choice
	///             {"s6", "{0,number} files"},                // third ChoiceFormat choice
	///             {"s7", "3 Mar 96"},                        // sample date
	///             {"s8", new Dimension(1,5)}                 // real object, not just string
	///         // END OF MATERIAL TO LOCALIZE
	///         };
	///     }
	/// }
	/// 
	/// public class MyResources_fr extends ListResourceBundle {
	///     protected Object[][] getContents() {
	///         return new Object[][] {
	///         // LOCALIZE THIS
	///             {"s1", "Le disque \"{1}\" {0}."},          // MessageFormat pattern
	///             {"s2", "1"},                               // location of {0} in pattern
	///             {"s3", "Mon disque"},                      // sample disk name
	///             {"s4", "ne contient pas de fichiers"},     // first ChoiceFormat choice
	///             {"s5", "contient un fichier"},             // second ChoiceFormat choice
	///             {"s6", "contient {0,number} fichiers"},    // third ChoiceFormat choice
	///             {"s7", "3 mars 1996"},                     // sample date
	///             {"s8", new Dimension(1,3)}                 // real object, not just string
	///         // END OF MATERIAL TO LOCALIZE
	///         };
	///     }
	/// }
	/// </pre>
	/// </blockquote>
	/// 
	/// </para>
	/// <para>
	/// The implementation of a {@code ListResourceBundle} subclass must be thread-safe
	/// if it's simultaneously used by multiple threads. The default implementations
	/// of the methods in this class are thread-safe.
	/// 
	/// </para>
	/// </summary>
	/// <seealso cref= ResourceBundle </seealso>
	/// <seealso cref= PropertyResourceBundle
	/// @since JDK1.1 </seealso>
	public abstract class ListResourceBundle : ResourceBundle
	{
		/// <summary>
		/// Sole constructor.  (For invocation by subclass constructors, typically
		/// implicit.)
		/// </summary>
		public ListResourceBundle()
		{
		}

		// Implements java.util.ResourceBundle.handleGetObject; inherits javadoc specification.
		public sealed override Object HandleGetObject(String key)
		{
			// lazily load the lookup hashtable.
			if (Lookup == null)
			{
				LoadLookup();
			}
			if (key == null)
			{
				throw new NullPointerException();
			}
			return Lookup.Get(key); // this class ignores locales
		}

		/// <summary>
		/// Returns an <code>Enumeration</code> of the keys contained in
		/// this <code>ResourceBundle</code> and its parent bundles.
		/// </summary>
		/// <returns> an <code>Enumeration</code> of the keys contained in
		///         this <code>ResourceBundle</code> and its parent bundles. </returns>
		/// <seealso cref= #keySet() </seealso>
		public override IEnumerator<String> Keys
		{
			get
			{
				// lazily load the lookup hashtable.
				if (Lookup == null)
				{
					LoadLookup();
				}
    
				ResourceBundle parent = this.Parent_Renamed;
				return new ResourceBundleEnumeration(Lookup.KeySet(), (parent != null) ? parent.Keys : null);
			}
		}

		/// <summary>
		/// Returns a <code>Set</code> of the keys contained
		/// <em>only</em> in this <code>ResourceBundle</code>.
		/// </summary>
		/// <returns> a <code>Set</code> of the keys contained only in this
		///         <code>ResourceBundle</code>
		/// @since 1.6 </returns>
		/// <seealso cref= #keySet() </seealso>
		protected internal override Set<String> HandleKeySet()
		{
			if (Lookup == null)
			{
				LoadLookup();
			}
			return Lookup.KeySet();
		}

		/// <summary>
		/// Returns an array in which each item is a pair of objects in an
		/// <code>Object</code> array. The first element of each pair is
		/// the key, which must be a <code>String</code>, and the second
		/// element is the value associated with that key.  See the class
		/// description for details.
		/// </summary>
		/// <returns> an array of an <code>Object</code> array representing a
		/// key-value pair. </returns>
		protected internal abstract Object[][] Contents {get;}

		// ==================privates====================

		/// <summary>
		/// We lazily load the lookup hashtable.  This function does the
		/// loading.
		/// </summary>
		private void LoadLookup()
		{
			lock (this)
			{
				if (Lookup != null)
				{
					return;
				}
        
				Object[][] contents = Contents;
				HashMap<String, Object> temp = new HashMap<String, Object>(contents.Length);
				for (int i = 0; i < contents.Length; ++i)
				{
					// key must be non-null String, value must be non-null
					String key = (String) contents[i][0];
					Object value = contents[i][1];
					if (key == null || value == null)
					{
						throw new NullPointerException();
					}
					temp.Put(key, value);
				}
				Lookup = temp;
			}
		}

		private Map<String, Object> Lookup = null;
	}

}
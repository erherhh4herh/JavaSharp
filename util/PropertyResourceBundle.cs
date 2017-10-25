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
 */

namespace java.util
{

	using ResourceBundleEnumeration = sun.util.ResourceBundleEnumeration;

	/// <summary>
	/// <code>PropertyResourceBundle</code> is a concrete subclass of
	/// <code>ResourceBundle</code> that manages resources for a locale
	/// using a set of static strings from a property file. See
	/// <seealso cref="ResourceBundle ResourceBundle"/> for more information about resource
	/// bundles.
	/// 
	/// <para>
	/// Unlike other types of resource bundle, you don't subclass
	/// <code>PropertyResourceBundle</code>.  Instead, you supply properties
	/// files containing the resource data.  <code>ResourceBundle.getBundle</code>
	/// will automatically look for the appropriate properties file and create a
	/// <code>PropertyResourceBundle</code> that refers to it. See
	/// <seealso cref="ResourceBundle#getBundle(java.lang.String, java.util.Locale, java.lang.ClassLoader) ResourceBundle.getBundle"/>
	/// for a complete description of the search and instantiation strategy.
	/// 
	/// </para>
	/// <para>
	/// The following <a name="sample">example</a> shows a member of a resource
	/// bundle family with the base name "MyResources".
	/// The text defines the bundle "MyResources_de",
	/// the German member of the bundle family.
	/// This member is based on <code>PropertyResourceBundle</code>, and the text
	/// therefore is the content of the file "MyResources_de.properties"
	/// (a related <a href="ListResourceBundle.html#sample">example</a> shows
	/// how you can add bundles to this family that are implemented as subclasses
	/// of <code>ListResourceBundle</code>).
	/// The keys in this example are of the form "s1" etc. The actual
	/// keys are entirely up to your choice, so long as they are the same as
	/// the keys you use in your program to retrieve the objects from the bundle.
	/// Keys are case-sensitive.
	/// <blockquote>
	/// <pre>
	/// # MessageFormat pattern
	/// s1=Die Platte \"{1}\" enth&auml;lt {0}.
	/// 
	/// # location of {0} in pattern
	/// s2=1
	/// 
	/// # sample disk name
	/// s3=Meine Platte
	/// 
	/// # first ChoiceFormat choice
	/// s4=keine Dateien
	/// 
	/// # second ChoiceFormat choice
	/// s5=eine Datei
	/// 
	/// # third ChoiceFormat choice
	/// s6={0,number} Dateien
	/// 
	/// # sample date
	/// s7=3. M&auml;rz 1996
	/// </pre>
	/// </blockquote>
	/// 
	/// </para>
	/// <para>
	/// The implementation of a {@code PropertyResourceBundle} subclass must be
	/// thread-safe if it's simultaneously used by multiple threads. The default
	/// implementations of the non-abstract methods in this class are thread-safe.
	/// 
	/// </para>
	/// <para>
	/// <strong>Note:</strong> PropertyResourceBundle can be constructed either
	/// from an InputStream or a Reader, which represents a property file.
	/// Constructing a PropertyResourceBundle instance from an InputStream requires
	/// that the input stream be encoded in ISO-8859-1.  In that case, characters
	/// that cannot be represented in ISO-8859-1 encoding must be represented by Unicode Escapes
	/// as defined in section 3.3 of
	/// <cite>The Java&trade; Language Specification</cite>
	/// whereas the other constructor which takes a Reader does not have that limitation.
	/// 
	/// </para>
	/// </summary>
	/// <seealso cref= ResourceBundle </seealso>
	/// <seealso cref= ListResourceBundle </seealso>
	/// <seealso cref= Properties
	/// @since JDK1.1 </seealso>
	public class PropertyResourceBundle : ResourceBundle
	{
		/// <summary>
		/// Creates a property resource bundle from an {@link java.io.InputStream
		/// InputStream}.  The property file read with this constructor
		/// must be encoded in ISO-8859-1.
		/// </summary>
		/// <param name="stream"> an InputStream that represents a property file
		///        to read from. </param>
		/// <exception cref="IOException"> if an I/O error occurs </exception>
		/// <exception cref="NullPointerException"> if <code>stream</code> is null </exception>
		/// <exception cref="IllegalArgumentException"> if {@code stream} contains a
		///     malformed Unicode escape sequence. </exception>
//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @SuppressWarnings({"unchecked", "rawtypes"}) public PropertyResourceBundle(java.io.InputStream stream) throws java.io.IOException
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
		public PropertyResourceBundle(InputStream stream)
		{
			Properties properties = new Properties();
			properties.Load(stream);
			Lookup = new HashMap(properties);
		}

		/// <summary>
		/// Creates a property resource bundle from a {@link java.io.Reader
		/// Reader}.  Unlike the constructor
		/// <seealso cref="#PropertyResourceBundle(java.io.InputStream) PropertyResourceBundle(InputStream)"/>,
		/// there is no limitation as to the encoding of the input property file.
		/// </summary>
		/// <param name="reader"> a Reader that represents a property file to
		///        read from. </param>
		/// <exception cref="IOException"> if an I/O error occurs </exception>
		/// <exception cref="NullPointerException"> if <code>reader</code> is null </exception>
		/// <exception cref="IllegalArgumentException"> if a malformed Unicode escape sequence appears
		///     from {@code reader}.
		/// @since 1.6 </exception>
//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @SuppressWarnings({"unchecked", "rawtypes"}) public PropertyResourceBundle(java.io.Reader reader) throws java.io.IOException
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
		public PropertyResourceBundle(Reader reader)
		{
			Properties properties = new Properties();
			properties.Load(reader);
			Lookup = new HashMap(properties);
		}

		// Implements java.util.ResourceBundle.handleGetObject; inherits javadoc specification.
		public override Object HandleGetObject(String key)
		{
			if (key == null)
			{
				throw new NullPointerException();
			}
			return Lookup.Get(key);
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
			return Lookup.KeySet();
		}

		// ==================privates====================

		private Map<String, Object> Lookup;
	}

}
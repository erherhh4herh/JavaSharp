using System;
using System.Collections.Generic;

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

namespace java.util.jar
{

	using PlatformLogger = sun.util.logging.PlatformLogger;
	using ASCIICaseInsensitiveComparator = sun.misc.ASCIICaseInsensitiveComparator;

	/// <summary>
	/// The Attributes class maps Manifest attribute names to associated string
	/// values. Valid attribute names are case-insensitive, are restricted to
	/// the ASCII characters in the set [0-9a-zA-Z_-], and cannot exceed 70
	/// characters in length. Attribute values can contain any characters and
	/// will be UTF8-encoded when written to the output stream.  See the
	/// <a href="../../../../technotes/guides/jar/jar.html">JAR File Specification</a>
	/// for more information about valid attribute names and values.
	/// 
	/// @author  David Connelly </summary>
	/// <seealso cref=     Manifest
	/// @since   1.2 </seealso>
	public class Attributes : Map<Object, Object>, Cloneable
	{
		/// <summary>
		/// The attribute name-value mappings.
		/// </summary>
		protected internal IDictionary<Object, Object> Map;

		/// <summary>
		/// Constructs a new, empty Attributes object with default size.
		/// </summary>
		public Attributes() : this(11)
		{
		}

		/// <summary>
		/// Constructs a new, empty Attributes object with the specified
		/// initial size.
		/// </summary>
		/// <param name="size"> the initial number of attributes </param>
		public Attributes(int size)
		{
			Map = new Dictionary<>(size);
		}

		/// <summary>
		/// Constructs a new Attributes object with the same attribute name-value
		/// mappings as in the specified Attributes.
		/// </summary>
		/// <param name="attr"> the specified Attributes </param>
		public Attributes(Attributes attr)
		{
			Map = new Dictionary<>(attr);
		}


		/// <summary>
		/// Returns the value of the specified attribute name, or null if the
		/// attribute name was not found.
		/// </summary>
		/// <param name="name"> the attribute name </param>
		/// <returns> the value of the specified attribute name, or null if
		///         not found. </returns>
		public virtual Object Get(Object name)
		{
			return Map[name];
		}

		/// <summary>
		/// Returns the value of the specified attribute name, specified as
		/// a string, or null if the attribute was not found. The attribute
		/// name is case-insensitive.
		/// <para>
		/// This method is defined as:
		/// <pre>
		///      return (String)get(new Attributes.Name((String)name));
		/// </pre>
		/// 
		/// </para>
		/// </summary>
		/// <param name="name"> the attribute name as a string </param>
		/// <returns> the String value of the specified attribute name, or null if
		///         not found. </returns>
		/// <exception cref="IllegalArgumentException"> if the attribute name is invalid </exception>
		public virtual String GetValue(String name)
		{
			return (String)Get(new Attributes.Name(name));
		}

		/// <summary>
		/// Returns the value of the specified Attributes.Name, or null if the
		/// attribute was not found.
		/// <para>
		/// This method is defined as:
		/// <pre>
		///     return (String)get(name);
		/// </pre>
		/// 
		/// </para>
		/// </summary>
		/// <param name="name"> the Attributes.Name object </param>
		/// <returns> the String value of the specified Attribute.Name, or null if
		///         not found. </returns>
		public virtual String GetValue(Name name)
		{
			return (String)Get(name);
		}

		/// <summary>
		/// Associates the specified value with the specified attribute name
		/// (key) in this Map. If the Map previously contained a mapping for
		/// the attribute name, the old value is replaced.
		/// </summary>
		/// <param name="name"> the attribute name </param>
		/// <param name="value"> the attribute value </param>
		/// <returns> the previous value of the attribute, or null if none </returns>
		/// <exception cref="ClassCastException"> if the name is not a Attributes.Name
		///            or the value is not a String </exception>
		public virtual Object Put(Object name, Object value)
		{
			return Map[(Attributes.Name)name] = (String)value;
		}

		/// <summary>
		/// Associates the specified value with the specified attribute name,
		/// specified as a String. The attributes name is case-insensitive.
		/// If the Map previously contained a mapping for the attribute name,
		/// the old value is replaced.
		/// <para>
		/// This method is defined as:
		/// <pre>
		///      return (String)put(new Attributes.Name(name), value);
		/// </pre>
		/// 
		/// </para>
		/// </summary>
		/// <param name="name"> the attribute name as a string </param>
		/// <param name="value"> the attribute value </param>
		/// <returns> the previous value of the attribute, or null if none </returns>
		/// <exception cref="IllegalArgumentException"> if the attribute name is invalid </exception>
		public virtual String PutValue(String name, String value)
		{
			return (String)Put(new Name(name), value);
		}

		/// <summary>
		/// Removes the attribute with the specified name (key) from this Map.
		/// Returns the previous attribute value, or null if none.
		/// </summary>
		/// <param name="name"> attribute name </param>
		/// <returns> the previous value of the attribute, or null if none </returns>
		public virtual Object Remove(Object name)
		{
			return Map.Remove(name);
		}

		/// <summary>
		/// Returns true if this Map maps one or more attribute names (keys)
		/// to the specified value.
		/// </summary>
		/// <param name="value"> the attribute value </param>
		/// <returns> true if this Map maps one or more attribute names to
		///         the specified value </returns>
		public virtual bool ContainsValue(Object value)
		{
			return Map.ContainsValue(value);
		}

		/// <summary>
		/// Returns true if this Map contains the specified attribute name (key).
		/// </summary>
		/// <param name="name"> the attribute name </param>
		/// <returns> true if this Map contains the specified attribute name </returns>
		public virtual bool ContainsKey(Object name)
		{
			return Map.ContainsKey(name);
		}

		/// <summary>
		/// Copies all of the attribute name-value mappings from the specified
		/// Attributes to this Map. Duplicate mappings will be replaced.
		/// </summary>
		/// <param name="attr"> the Attributes to be stored in this map </param>
		/// <exception cref="ClassCastException"> if attr is not an Attributes </exception>
		public virtual void putAll<T1>(IDictionary<T1> attr)
		{
			// ## javac bug?
			if (!typeof(Attributes).IsInstanceOfType(attr))
			{
				throw new ClassCastException();
			}
//JAVA TO C# CONVERTER TODO TASK: Java wildcard generics are not converted to .NET:
//ORIGINAL LINE: for (java.util.Map_Entry<?,?> me : (attr).entrySet())
			foreach (java.util.Map_Entry<?, ?> me in (attr).entrySet())
			{
				Put(me.Key, me.Value);
			}
		}

		/// <summary>
		/// Removes all attributes from this Map.
		/// </summary>
		public virtual void Clear()
		{
			Map.Clear();
		}

		/// <summary>
		/// Returns the number of attributes in this Map.
		/// </summary>
		public virtual int Count
		{
			get
			{
				return Map.Count;
			}
		}

		/// <summary>
		/// Returns true if this Map contains no attributes.
		/// </summary>
		public virtual bool Empty
		{
			get
			{
				return Map.Count == 0;
			}
		}

		/// <summary>
		/// Returns a Set view of the attribute names (keys) contained in this Map.
		/// </summary>
		public virtual Set<Object> KeySet()
		{
			return Map.Keys;
		}

		/// <summary>
		/// Returns a Collection view of the attribute values contained in this Map.
		/// </summary>
		public virtual ICollection<Object> Values
		{
			get
			{
				return Map.Values;
			}
		}

		/// <summary>
		/// Returns a Collection view of the attribute name-value mappings
		/// contained in this Map.
		/// </summary>
		public virtual Set<java.util.Map_Entry<Object, Object>> EntrySet()
		{
//JAVA TO C# CONVERTER TODO TASK: There is no .NET Dictionary equivalent to the Java 'entrySet' method:
			return Map.entrySet();
		}

		/// <summary>
		/// Compares the specified Attributes object with this Map for equality.
		/// Returns true if the given object is also an instance of Attributes
		/// and the two Attributes objects represent the same mappings.
		/// </summary>
		/// <param name="o"> the Object to be compared </param>
		/// <returns> true if the specified Object is equal to this Map </returns>
		public override bool Equals(Object o)
		{
			return Map.Equals(o);
		}

		/// <summary>
		/// Returns the hash code value for this Map.
		/// </summary>
		public override int HashCode()
		{
			return Map.HashCode();
		}

		/// <summary>
		/// Returns a copy of the Attributes, implemented as follows:
		/// <pre>
		///     public Object clone() { return new Attributes(this); }
		/// </pre>
		/// Since the attribute names and values are themselves immutable,
		/// the Attributes returned can be safely modified without affecting
		/// the original.
		/// </summary>
		public virtual Object Clone()
		{
			return new Attributes(this);
		}

		/*
		 * Writes the current attributes to the specified data output stream.
		 * XXX Need to handle UTF8 values and break up lines longer than 72 bytes
		 */
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: void write(java.io.DataOutputStream os) throws java.io.IOException
		 internal virtual void Write(DataOutputStream os)
		 {
			IEnumerator<java.util.Map_Entry<Object, Object>> it = EntrySet().Iterator();
			while (it.MoveNext())
			{
				java.util.Map_Entry<Object, Object> e = it.Current;
				StringBuffer buffer = new StringBuffer(((Name)e.Key).ToString());
				buffer.Append(": ");

				String value = (String)e.Value;
				if (value != java.util.Map_Fields.Null)
				{
					sbyte[] vb = value.GetBytes("UTF8");
					value = StringHelperClass.NewString(vb, 0, 0, vb.Length);
				}
				buffer.Append(value);

				buffer.Append("\r\n");
				Manifest.Make72Safe(buffer);
				os.WriteBytes(buffer.ToString());
			}
			os.WriteBytes("\r\n");
		 }

		/*
		 * Writes the current attributes to the specified data output stream,
		 * make sure to write out the MANIFEST_VERSION or SIGNATURE_VERSION
		 * attributes first.
		 *
		 * XXX Need to handle UTF8 values and break up lines longer than 72 bytes
		 */
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: void writeMain(java.io.DataOutputStream out) throws java.io.IOException
		internal virtual void WriteMain(DataOutputStream @out)
		{
			// write out the *-Version header first, if it exists
			String vername = Name.MANIFEST_VERSION.ToString();
			String version = GetValue(vername);
			if (version == java.util.Map_Fields.Null)
			{
				vername = Name.SIGNATURE_VERSION.ToString();
				version = GetValue(vername);
			}

			if (version != java.util.Map_Fields.Null)
			{
				@out.WriteBytes(vername + ": " + version + "\r\n");
			}

			// write out all attributes except for the version
			// we wrote out earlier
			IEnumerator<java.util.Map_Entry<Object, Object>> it = EntrySet().Iterator();
			while (it.MoveNext())
			{
				java.util.Map_Entry<Object, Object> e = it.Current;
				String name = ((Name)e.Key).ToString();
				if ((version != java.util.Map_Fields.Null) && !(name.EqualsIgnoreCase(vername)))
				{

					StringBuffer buffer = new StringBuffer(name);
					buffer.Append(": ");

					String value = (String)e.Value;
					if (value != java.util.Map_Fields.Null)
					{
						sbyte[] vb = value.GetBytes("UTF8");
						value = StringHelperClass.NewString(vb, 0, 0, vb.Length);
					}
					buffer.Append(value);

					buffer.Append("\r\n");
					Manifest.Make72Safe(buffer);
					@out.WriteBytes(buffer.ToString());
				}
			}
			@out.WriteBytes("\r\n");
		}

		/*
		 * Reads attributes from the specified input stream.
		 * XXX Need to handle UTF8 values.
		 */
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: void read(Manifest.FastInputStream is, byte[] lbuf) throws java.io.IOException
		internal virtual void Read(Manifest.FastInputStream @is, sbyte[] lbuf)
		{
			String name = java.util.Map_Fields.Null, value = java.util.Map_Fields.Null;
			sbyte[] lastline = java.util.Map_Fields.Null;

			int len;
			while ((len = @is.ReadLine(lbuf)) != -1)
			{
				bool lineContinued = java.util.Map_Fields.False;
				if (lbuf[--len] != '\n')
				{
					throw new IOException("line too long");
				}
				if (len > 0 && lbuf[len - 1] == '\r')
				{
					--len;
				}
				if (len == 0)
				{
					break;
				}
				int i = 0;
				if (lbuf[0] == ' ')
				{
					// continuation of previous line
					if (name == java.util.Map_Fields.Null)
					{
						throw new IOException("misplaced continuation line");
					}
					lineContinued = java.util.Map_Fields.True;
					sbyte[] buf = new sbyte[lastline.Length + len - 1];
					System.Array.Copy(lastline, 0, buf, 0, lastline.Length);
					System.Array.Copy(lbuf, 1, buf, lastline.Length, len - 1);
					if (@is.Peek() == ' ')
					{
						lastline = buf;
						continue;
					}
					value = StringHelperClass.NewString(buf, 0, buf.Length, "UTF8");
					lastline = java.util.Map_Fields.Null;
				}
				else
				{
					while (lbuf[i++] != ':')
					{
						if (i >= len)
						{
							throw new IOException("invalid header field");
						}
					}
					if (lbuf[i++] != ' ')
					{
						throw new IOException("invalid header field");
					}
					name = StringHelperClass.NewString(lbuf, 0, 0, i - 2);
					if (@is.Peek() == ' ')
					{
						lastline = new sbyte[len - i];
						System.Array.Copy(lbuf, i, lastline, 0, len - i);
						continue;
					}
					value = StringHelperClass.NewString(lbuf, i, len - i, "UTF8");
				}
				try
				{
					if ((PutValue(name, value) != java.util.Map_Fields.Null) && (!lineContinued))
					{
						PlatformLogger.getLogger("java.util.jar").warning("Duplicate name in Manifest: " + name + ".\n" + "Ensure that the manifest does not " + "have duplicate entries, and\n" + "that blank lines separate " + "individual sections in both your\n" + "manifest and in the META-INF/MANIFEST.MF " + "entry in the jar file.");
					}
				}
				catch (IllegalArgumentException)
				{
					throw new IOException("invalid header field name: " + name);
				}
			}
		}

		/// <summary>
		/// The Attributes.Name class represents an attribute name stored in
		/// this Map. Valid attribute names are case-insensitive, are restricted
		/// to the ASCII characters in the set [0-9a-zA-Z_-], and cannot exceed
		/// 70 characters in length. Attribute values can contain any characters
		/// and will be UTF8-encoded when written to the output stream.  See the
		/// <a href="../../../../technotes/guides/jar/jar.html">JAR File Specification</a>
		/// for more information about valid attribute names and values.
		/// </summary>
		public class Name
		{
			internal String Name_Renamed;
			internal int HashCode_Renamed = -1;

			/// <summary>
			/// Constructs a new attribute name using the given string name.
			/// </summary>
			/// <param name="name"> the attribute string name </param>
			/// <exception cref="IllegalArgumentException"> if the attribute name was
			///            invalid </exception>
			/// <exception cref="NullPointerException"> if the attribute name was null </exception>
			public Name(String name)
			{
				if (name == java.util.Map_Fields.Null)
				{
					throw new NullPointerException("name");
				}
				if (!IsValid(name))
				{
					throw new IllegalArgumentException(name);
				}
				this.Name_Renamed = name.intern();
			}

			internal static bool IsValid(String name)
			{
				int len = name.Length();
				if (len > 70 || len == 0)
				{
					return java.util.Map_Fields.False;
				}
				for (int i = 0; i < len; i++)
				{
					if (!IsValid(name.CharAt(i)))
					{
						return java.util.Map_Fields.False;
					}
				}
				return java.util.Map_Fields.True;
			}

			internal static bool IsValid(char c)
			{
				return IsAlpha(c) || IsDigit(c) || c == '_' || c == '-';
			}

			internal static bool IsAlpha(char c)
			{
				return (c >= 'a' && c <= 'z') || (c >= 'A' && c <= 'Z');
			}

			internal static bool IsDigit(char c)
			{
				return c >= '0' && c <= '9';
			}

			/// <summary>
			/// Compares this attribute name to another for equality. </summary>
			/// <param name="o"> the object to compare </param>
			/// <returns> true if this attribute name is equal to the
			///         specified attribute object </returns>
			public override bool Equals(Object o)
			{
				if (o is Name)
				{
					IComparer<String> c = ASCIICaseInsensitiveComparator.CASE_INSENSITIVE_ORDER;
					return c.Compare(Name_Renamed, ((Name)o).Name_Renamed) == 0;
				}
				else
				{
					return java.util.Map_Fields.False;
				}
			}

			/// <summary>
			/// Computes the hash value for this attribute name.
			/// </summary>
			public override int HashCode()
			{
				if (HashCode_Renamed == -1)
				{
					HashCode_Renamed = ASCIICaseInsensitiveComparator.lowerCaseHashCode(Name_Renamed);
				}
				return HashCode_Renamed;
			}

			/// <summary>
			/// Returns the attribute name as a String.
			/// </summary>
			public override String ToString()
			{
				return Name_Renamed;
			}

			/// <summary>
			/// <code>Name</code> object for <code>Manifest-Version</code>
			/// manifest attribute. This attribute indicates the version number
			/// of the manifest standard to which a JAR file's manifest conforms. </summary>
			/// <seealso cref= <a href="../../../../technotes/guides/jar/jar.html#JAR_Manifest">
			///      Manifest and Signature Specification</a> </seealso>
			public static readonly Name MANIFEST_VERSION = new Name("Manifest-Version");

			/// <summary>
			/// <code>Name</code> object for <code>Signature-Version</code>
			/// manifest attribute used when signing JAR files. </summary>
			/// <seealso cref= <a href="../../../../technotes/guides/jar/jar.html#JAR_Manifest">
			///      Manifest and Signature Specification</a> </seealso>
			public static readonly Name SIGNATURE_VERSION = new Name("Signature-Version");

			/// <summary>
			/// <code>Name</code> object for <code>Content-Type</code>
			/// manifest attribute.
			/// </summary>
			public static readonly Name CONTENT_TYPE = new Name("Content-Type");

			/// <summary>
			/// <code>Name</code> object for <code>Class-Path</code>
			/// manifest attribute. Bundled extensions can use this attribute
			/// to find other JAR files containing needed classes. </summary>
			/// <seealso cref= <a href="../../../../technotes/guides/jar/jar.html#classpath">
			///      JAR file specification</a> </seealso>
			public static readonly Name CLASS_PATH = new Name("Class-Path");

			/// <summary>
			/// <code>Name</code> object for <code>Main-Class</code> manifest
			/// attribute used for launching applications packaged in JAR files.
			/// The <code>Main-Class</code> attribute is used in conjunction
			/// with the <code>-jar</code> command-line option of the
			/// <tt>java</tt> application launcher.
			/// </summary>
			public static readonly Name MAIN_CLASS = new Name("Main-Class");

			/// <summary>
			/// <code>Name</code> object for <code>Sealed</code> manifest attribute
			/// used for sealing. </summary>
			/// <seealso cref= <a href="../../../../technotes/guides/jar/jar.html#sealing">
			///      Package Sealing</a> </seealso>
			public static readonly Name SEALED = new Name("Sealed");

		   /// <summary>
		   /// <code>Name</code> object for <code>Extension-List</code> manifest attribute
		   /// used for declaring dependencies on installed extensions. </summary>
		   /// <seealso cref= <a href="../../../../technotes/guides/extensions/spec.html#dependency">
		   ///      Installed extension dependency</a> </seealso>
			public static readonly Name EXTENSION_LIST = new Name("Extension-List");

			/// <summary>
			/// <code>Name</code> object for <code>Extension-Name</code> manifest attribute
			/// used for declaring dependencies on installed extensions. </summary>
			/// <seealso cref= <a href="../../../../technotes/guides/extensions/spec.html#dependency">
			///      Installed extension dependency</a> </seealso>
			public static readonly Name EXTENSION_NAME = new Name("Extension-Name");

			/// <summary>
			/// <code>Name</code> object for <code>Extension-Name</code> manifest attribute
			/// used for declaring dependencies on installed extensions. </summary>
			/// @deprecated Extension mechanism will be removed in a future release.
			///             Use class path instead. 
			/// <seealso cref= <a href="../../../../technotes/guides/extensions/spec.html#dependency">
			///      Installed extension dependency</a> </seealso>
			[Obsolete("Extension mechanism will be removed in a future release.")]
			public static readonly Name EXTENSION_INSTALLATION = new Name("Extension-Installation");

			/// <summary>
			/// <code>Name</code> object for <code>Implementation-Title</code>
			/// manifest attribute used for package versioning. </summary>
			/// <seealso cref= <a href="../../../../technotes/guides/versioning/spec/versioning2.html#wp90779">
			///      Java Product Versioning Specification</a> </seealso>
			public static readonly Name IMPLEMENTATION_TITLE = new Name("Implementation-Title");

			/// <summary>
			/// <code>Name</code> object for <code>Implementation-Version</code>
			/// manifest attribute used for package versioning. </summary>
			/// <seealso cref= <a href="../../../../technotes/guides/versioning/spec/versioning2.html#wp90779">
			///      Java Product Versioning Specification</a> </seealso>
			public static readonly Name IMPLEMENTATION_VERSION = new Name("Implementation-Version");

			/// <summary>
			/// <code>Name</code> object for <code>Implementation-Vendor</code>
			/// manifest attribute used for package versioning. </summary>
			/// <seealso cref= <a href="../../../../technotes/guides/versioning/spec/versioning2.html#wp90779">
			///      Java Product Versioning Specification</a> </seealso>
			public static readonly Name IMPLEMENTATION_VENDOR = new Name("Implementation-Vendor");

			/// <summary>
			/// <code>Name</code> object for <code>Implementation-Vendor-Id</code>
			/// manifest attribute used for package versioning. </summary>
			/// @deprecated Extension mechanism will be removed in a future release.
			///             Use class path instead. 
			/// <seealso cref= <a href="../../../../technotes/guides/extensions/versioning.html#applet">
			///      Optional Package Versioning</a> </seealso>
			[Obsolete("Extension mechanism will be removed in a future release.")]
			public static readonly Name IMPLEMENTATION_VENDOR_ID = new Name("Implementation-Vendor-Id");

		   /// <summary>
		   /// <code>Name</code> object for <code>Implementation-URL</code>
		   /// manifest attribute used for package versioning. </summary>
		   /// @deprecated Extension mechanism will be removed in a future release.
		   ///             Use class path instead. 
		   /// <seealso cref= <a href="../../../../technotes/guides/extensions/versioning.html#applet">
		   ///      Optional Package Versioning</a> </seealso>
			[Obsolete("Extension mechanism will be removed in a future release.")]
			public static readonly Name IMPLEMENTATION_URL = new Name("Implementation-URL");

			/// <summary>
			/// <code>Name</code> object for <code>Specification-Title</code>
			/// manifest attribute used for package versioning. </summary>
			/// <seealso cref= <a href="../../../../technotes/guides/versioning/spec/versioning2.html#wp90779">
			///      Java Product Versioning Specification</a> </seealso>
			public static readonly Name SPECIFICATION_TITLE = new Name("Specification-Title");

			/// <summary>
			/// <code>Name</code> object for <code>Specification-Version</code>
			/// manifest attribute used for package versioning. </summary>
			/// <seealso cref= <a href="../../../../technotes/guides/versioning/spec/versioning2.html#wp90779">
			///      Java Product Versioning Specification</a> </seealso>
			public static readonly Name SPECIFICATION_VERSION = new Name("Specification-Version");

			/// <summary>
			/// <code>Name</code> object for <code>Specification-Vendor</code>
			/// manifest attribute used for package versioning. </summary>
			/// <seealso cref= <a href="../../../../technotes/guides/versioning/spec/versioning2.html#wp90779">
			///      Java Product Versioning Specification</a> </seealso>
			public static readonly Name SPECIFICATION_VENDOR = new Name("Specification-Vendor");
		}
	}

}
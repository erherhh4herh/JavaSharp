using System.Collections.Generic;

/*
 * Copyright (c) 2000, 2013, Oracle and/or its affiliates. All rights reserved.
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

namespace java.nio.charset
{

	using ASCIICaseInsensitiveComparator = sun.misc.ASCIICaseInsensitiveComparator;
	using StandardCharsets = sun.nio.cs.StandardCharsets;
	using ThreadLocalCoders = sun.nio.cs.ThreadLocalCoders;
	using GetPropertyAction = sun.security.action.GetPropertyAction;


	/// <summary>
	/// A named mapping between sequences of sixteen-bit Unicode <a
	/// href="../../lang/Character.html#unicode">code units</a> and sequences of
	/// bytes.  This class defines methods for creating decoders and encoders and
	/// for retrieving the various names associated with a charset.  Instances of
	/// this class are immutable.
	/// 
	/// <para> This class also defines static methods for testing whether a particular
	/// charset is supported, for locating charset instances by name, and for
	/// constructing a map that contains every charset for which support is
	/// available in the current Java virtual machine.  Support for new charsets can
	/// be added via the service-provider interface defined in the {@link
	/// java.nio.charset.spi.CharsetProvider} class.
	/// 
	/// </para>
	/// <para> All of the methods defined in this class are safe for use by multiple
	/// concurrent threads.
	/// 
	/// 
	/// <a name="names"></a><a name="charenc"></a>
	/// <h2>Charset names</h2>
	/// 
	/// </para>
	/// <para> Charsets are named by strings composed of the following characters:
	/// 
	/// <ul>
	/// 
	///   <li> The uppercase letters <tt>'A'</tt> through <tt>'Z'</tt>
	///        (<tt>'&#92;u0041'</tt>&nbsp;through&nbsp;<tt>'&#92;u005a'</tt>),
	/// 
	///   <li> The lowercase letters <tt>'a'</tt> through <tt>'z'</tt>
	///        (<tt>'&#92;u0061'</tt>&nbsp;through&nbsp;<tt>'&#92;u007a'</tt>),
	/// 
	///   <li> The digits <tt>'0'</tt> through <tt>'9'</tt>
	///        (<tt>'&#92;u0030'</tt>&nbsp;through&nbsp;<tt>'&#92;u0039'</tt>),
	/// 
	///   <li> The dash character <tt>'-'</tt>
	///        (<tt>'&#92;u002d'</tt>,&nbsp;<small>HYPHEN-MINUS</small>),
	/// 
	///   <li> The plus character <tt>'+'</tt>
	///        (<tt>'&#92;u002b'</tt>,&nbsp;<small>PLUS SIGN</small>),
	/// 
	///   <li> The period character <tt>'.'</tt>
	///        (<tt>'&#92;u002e'</tt>,&nbsp;<small>FULL STOP</small>),
	/// 
	///   <li> The colon character <tt>':'</tt>
	///        (<tt>'&#92;u003a'</tt>,&nbsp;<small>COLON</small>), and
	/// 
	///   <li> The underscore character <tt>'_'</tt>
	///        (<tt>'&#92;u005f'</tt>,&nbsp;<small>LOW&nbsp;LINE</small>).
	/// 
	/// </ul>
	/// 
	/// A charset name must begin with either a letter or a digit.  The empty string
	/// is not a legal charset name.  Charset names are not case-sensitive; that is,
	/// case is always ignored when comparing charset names.  Charset names
	/// generally follow the conventions documented in <a
	/// href="http://www.ietf.org/rfc/rfc2278.txt"><i>RFC&nbsp;2278:&nbsp;IANA Charset
	/// Registration Procedures</i></a>.
	/// 
	/// </para>
	/// <para> Every charset has a <i>canonical name</i> and may also have one or more
	/// <i>aliases</i>.  The canonical name is returned by the <seealso cref="#name() name"/> method
	/// of this class.  Canonical names are, by convention, usually in upper case.
	/// The aliases of a charset are returned by the <seealso cref="#aliases() aliases"/>
	/// method.
	/// 
	/// </para>
	/// <para><a name="hn">Some charsets have an <i>historical name</i> that is defined for
	/// compatibility with previous versions of the Java platform.</a>  A charset's
	/// historical name is either its canonical name or one of its aliases.  The
	/// historical name is returned by the <tt>getEncoding()</tt> methods of the
	/// <seealso cref="java.io.InputStreamReader#getEncoding InputStreamReader"/> and {@link
	/// java.io.OutputStreamWriter#getEncoding OutputStreamWriter} classes.
	/// 
	/// </para>
	/// <para><a name="iana"> </a>If a charset listed in the <a
	/// href="http://www.iana.org/assignments/character-sets"><i>IANA Charset
	/// Registry</i></a> is supported by an implementation of the Java platform then
	/// its canonical name must be the name listed in the registry. Many charsets
	/// are given more than one name in the registry, in which case the registry
	/// identifies one of the names as <i>MIME-preferred</i>.  If a charset has more
	/// than one registry name then its canonical name must be the MIME-preferred
	/// name and the other names in the registry must be valid aliases.  If a
	/// supported charset is not listed in the IANA registry then its canonical name
	/// must begin with one of the strings <tt>"X-"</tt> or <tt>"x-"</tt>.
	/// 
	/// </para>
	/// <para> The IANA charset registry does change over time, and so the canonical
	/// name and the aliases of a particular charset may also change over time.  To
	/// ensure compatibility it is recommended that no alias ever be removed from a
	/// charset, and that if the canonical name of a charset is changed then its
	/// previous canonical name be made into an alias.
	/// 
	/// 
	/// <h2>Standard charsets</h2>
	/// 
	/// 
	/// 
	/// </para>
	/// <para><a name="standard">Every implementation of the Java platform is required to support the
	/// following standard charsets.</a>  Consult the release documentation for your
	/// implementation to see if any other charsets are supported.  The behavior
	/// of such optional charsets may differ between implementations.
	/// 
	/// <blockquote><table width="80%" summary="Description of standard charsets">
	/// <tr><th align="left">Charset</th><th align="left">Description</th></tr>
	/// <tr><td valign=top><tt>US-ASCII</tt></td>
	///     <td>Seven-bit ASCII, a.k.a. <tt>ISO646-US</tt>,
	///         a.k.a. the Basic Latin block of the Unicode character set</td></tr>
	/// <tr><td valign=top><tt>ISO-8859-1&nbsp;&nbsp;</tt></td>
	///     <td>ISO Latin Alphabet No. 1, a.k.a. <tt>ISO-LATIN-1</tt></td></tr>
	/// <tr><td valign=top><tt>UTF-8</tt></td>
	///     <td>Eight-bit UCS Transformation Format</td></tr>
	/// <tr><td valign=top><tt>UTF-16BE</tt></td>
	///     <td>Sixteen-bit UCS Transformation Format,
	///         big-endian byte&nbsp;order</td></tr>
	/// <tr><td valign=top><tt>UTF-16LE</tt></td>
	///     <td>Sixteen-bit UCS Transformation Format,
	///         little-endian byte&nbsp;order</td></tr>
	/// <tr><td valign=top><tt>UTF-16</tt></td>
	///     <td>Sixteen-bit UCS Transformation Format,
	///         byte&nbsp;order identified by an optional byte-order mark</td></tr>
	/// </table></blockquote>
	/// 
	/// </para>
	/// <para> The <tt>UTF-8</tt> charset is specified by <a
	/// href="http://www.ietf.org/rfc/rfc2279.txt"><i>RFC&nbsp;2279</i></a>; the
	/// transformation format upon which it is based is specified in
	/// Amendment&nbsp;2 of ISO&nbsp;10646-1 and is also described in the <a
	/// href="http://www.unicode.org/unicode/standard/standard.html"><i>Unicode
	/// Standard</i></a>.
	/// 
	/// </para>
	/// <para> The <tt>UTF-16</tt> charsets are specified by <a
	/// href="http://www.ietf.org/rfc/rfc2781.txt"><i>RFC&nbsp;2781</i></a>; the
	/// transformation formats upon which they are based are specified in
	/// Amendment&nbsp;1 of ISO&nbsp;10646-1 and are also described in the <a
	/// href="http://www.unicode.org/unicode/standard/standard.html"><i>Unicode
	/// Standard</i></a>.
	/// 
	/// </para>
	/// <para> The <tt>UTF-16</tt> charsets use sixteen-bit quantities and are
	/// therefore sensitive to byte order.  In these encodings the byte order of a
	/// stream may be indicated by an initial <i>byte-order mark</i> represented by
	/// the Unicode character <tt>'&#92;uFEFF'</tt>.  Byte-order marks are handled
	/// as follows:
	/// 
	/// <ul>
	/// 
	/// </para>
	///   <li><para> When decoding, the <tt>UTF-16BE</tt> and <tt>UTF-16LE</tt>
	///   charsets interpret the initial byte-order marks as a <small>ZERO-WIDTH
	///   NON-BREAKING SPACE</small>; when encoding, they do not write
	///   byte-order marks. </para></li>
	/// 
	/// 
	///   <li><para> When decoding, the <tt>UTF-16</tt> charset interprets the
	///   byte-order mark at the beginning of the input stream to indicate the
	///   byte-order of the stream but defaults to big-endian if there is no
	///   byte-order mark; when encoding, it uses big-endian byte order and writes
	///   a big-endian byte-order mark. </para></li>
	/// 
	/// </ul>
	/// 
	/// In any case, byte order marks occurring after the first element of an
	/// input sequence are not omitted since the same code is used to represent
	/// <small>ZERO-WIDTH NON-BREAKING SPACE</small>.
	/// 
	/// <para> Every instance of the Java virtual machine has a default charset, which
	/// may or may not be one of the standard charsets.  The default charset is
	/// determined during virtual-machine startup and typically depends upon the
	/// locale and charset being used by the underlying operating system. </para>
	/// 
	/// <para>The <seealso cref="StandardCharsets"/> class defines constants for each of the
	/// standard charsets.
	/// 
	/// <h2>Terminology</h2>
	/// 
	/// </para>
	/// <para> The name of this class is taken from the terms used in
	/// <a href="http://www.ietf.org/rfc/rfc2278.txt"><i>RFC&nbsp;2278</i></a>.
	/// In that document a <i>charset</i> is defined as the combination of
	/// one or more coded character sets and a character-encoding scheme.
	/// (This definition is confusing; some other software systems define
	/// <i>charset</i> as a synonym for <i>coded character set</i>.)
	/// 
	/// </para>
	/// <para> A <i>coded character set</i> is a mapping between a set of abstract
	/// characters and a set of integers.  US-ASCII, ISO&nbsp;8859-1,
	/// JIS&nbsp;X&nbsp;0201, and Unicode are examples of coded character sets.
	/// 
	/// </para>
	/// <para> Some standards have defined a <i>character set</i> to be simply a
	/// set of abstract characters without an associated assigned numbering.
	/// An alphabet is an example of such a character set.  However, the subtle
	/// distinction between <i>character set</i> and <i>coded character set</i>
	/// is rarely used in practice; the former has become a short form for the
	/// latter, including in the Java API specification.
	/// 
	/// </para>
	/// <para> A <i>character-encoding scheme</i> is a mapping between one or more
	/// coded character sets and a set of octet (eight-bit byte) sequences.
	/// UTF-8, UTF-16, ISO&nbsp;2022, and EUC are examples of
	/// character-encoding schemes.  Encoding schemes are often associated with
	/// a particular coded character set; UTF-8, for example, is used only to
	/// encode Unicode.  Some schemes, however, are associated with multiple
	/// coded character sets; EUC, for example, can be used to encode
	/// characters in a variety of Asian coded character sets.
	/// 
	/// </para>
	/// <para> When a coded character set is used exclusively with a single
	/// character-encoding scheme then the corresponding charset is usually
	/// named for the coded character set; otherwise a charset is usually named
	/// for the encoding scheme and, possibly, the locale of the coded
	/// character sets that it supports.  Hence <tt>US-ASCII</tt> is both the
	/// name of a coded character set and of the charset that encodes it, while
	/// <tt>EUC-JP</tt> is the name of the charset that encodes the
	/// JIS&nbsp;X&nbsp;0201, JIS&nbsp;X&nbsp;0208, and JIS&nbsp;X&nbsp;0212
	/// coded character sets for the Japanese language.
	/// 
	/// </para>
	/// <para> The native character encoding of the Java programming language is
	/// UTF-16.  A charset in the Java platform therefore defines a mapping
	/// between sequences of sixteen-bit UTF-16 code units (that is, sequences
	/// of chars) and sequences of bytes. </para>
	/// 
	/// 
	/// @author Mark Reinhold
	/// @author JSR-51 Expert Group
	/// @since 1.4
	/// </summary>
	/// <seealso cref= CharsetDecoder </seealso>
	/// <seealso cref= CharsetEncoder </seealso>
	/// <seealso cref= java.nio.charset.spi.CharsetProvider </seealso>
	/// <seealso cref= java.lang.Character </seealso>

	public abstract class Charset : Comparable<Charset>
	{

		/* -- Static methods -- */

		private static volatile String BugLevel = null;

		internal static bool AtBugLevel(String bl) // package-private
		{
			String level = BugLevel;
			if (level == null)
			{
				if (!sun.misc.VM.Booted)
				{
					return false;
				}
				BugLevel = level = AccessController.doPrivileged(new GetPropertyAction("sun.nio.cs.bugLevel", ""));
			}
			return level.Equals(bl);
		}

		/// <summary>
		/// Checks that the given string is a legal charset name. </p>
		/// </summary>
		/// <param name="s">
		///         A purported charset name
		/// </param>
		/// <exception cref="IllegalCharsetNameException">
		///          If the given name is not a legal charset name </exception>
		private static void CheckName(String s)
		{
			int n = s.Length();
			if (!AtBugLevel("1.4"))
			{
				if (n == 0)
				{
					throw new IllegalCharsetNameException(s);
				}
			}
			for (int i = 0; i < n; i++)
			{
				char c = s.CharAt(i);
				if (c >= 'A' && c <= 'Z')
				{
					continue;
				}
				if (c >= 'a' && c <= 'z')
				{
					continue;
				}
				if (c >= '0' && c <= '9')
				{
					continue;
				}
				if (c == '-' && i != 0)
				{
					continue;
				}
				if (c == '+' && i != 0)
				{
					continue;
				}
				if (c == ':' && i != 0)
				{
					continue;
				}
				if (c == '_' && i != 0)
				{
					continue;
				}
				if (c == '.' && i != 0)
				{
					continue;
				}
				throw new IllegalCharsetNameException(s);
			}
		}

		/* The standard set of charsets */
		private static CharsetProvider StandardProvider = new StandardCharsets();

		// Cache of the most-recently-returned charsets,
		// along with the names that were used to find them
		//
		private static volatile Object[] Cache1 = null; // "Level 1" cache
		private static volatile Object[] Cache2 = null; // "Level 2" cache

		private static void Cache(String charsetName, Charset cs)
		{
			Cache2 = Cache1;
			Cache1 = new Object[] {charsetName, cs};
		}

		// Creates an iterator that walks over the available providers, ignoring
		// those whose lookup or instantiation causes a security exception to be
		// thrown.  Should be invoked with full privileges.
		//
		private static IEnumerator<CharsetProvider> Providers()
		{
			return new IteratorAnonymousInnerClassHelper();
		}

		private class IteratorAnonymousInnerClassHelper : Iterator<CharsetProvider>
		{
			public IteratorAnonymousInnerClassHelper()
			{
			}


			internal ClassLoader cl = ClassLoader.SystemClassLoader;
			internal ServiceLoader<CharsetProvider> sl = ServiceLoader.Load(typeof(CharsetProvider), cl);
			internal IEnumerator<CharsetProvider> i = sl.GetEnumerator();

			internal CharsetProvider next = null;

			private bool Next
			{
				get
				{
					while (next == null)
					{
						try
						{
							if (!i.hasNext())
							{
								return false;
							}
							next = i.next();
						}
						catch (ServiceConfigurationError sce)
						{
							if (sce.Cause is SecurityException)
							{
								// Ignore security exceptions
								continue;
							}
							throw sce;
						}
					}
					return true;
				}
			}

			public virtual bool HasNext()
			{
				return Next;
			}

			public virtual CharsetProvider Next()
			{
				if (!Next)
				{
					throw new NoSuchElementException();
				}
				CharsetProvider n = next;
				next = null;
				return n;
			}

			public virtual void Remove()
			{
				throw new UnsupportedOperationException();
			}

		}

		// Thread-local gate to prevent recursive provider lookups
//JAVA TO C# CONVERTER TODO TASK: Java wildcard generics are not converted to .NET:
//ORIGINAL LINE: private static ThreadLocal<ThreadLocal<?>> gate = new ThreadLocal<ThreadLocal<?>>();
		private static ThreadLocal<ThreadLocal<?>> Gate = new ThreadLocal<ThreadLocal<?>>();

//JAVA TO C# CONVERTER WARNING: 'final' parameters are not allowed in .NET:
//ORIGINAL LINE: private static Charset lookupViaProviders(final String charsetName)
		private static Charset LookupViaProviders(String charsetName)
		{

			// The runtime startup sequence looks up standard charsets as a
			// consequence of the VM's invocation of System.initializeSystemClass
			// in order to, e.g., set system properties and encode filenames.  At
			// that point the application class loader has not been initialized,
			// however, so we can't look for providers because doing so will cause
			// that loader to be prematurely initialized with incomplete
			// information.
			//
			if (!sun.misc.VM.Booted)
			{
				return null;
			}

			if (Gate.Get() != null)
			{
				// Avoid recursive provider lookups
				return null;
			}
			try
			{
				Gate.Set(Gate);

				return AccessController.doPrivileged(new PrivilegedActionAnonymousInnerClassHelper(charsetName));

			}
			finally
			{
				Gate.Set(null);
			}
		}

		private class PrivilegedActionAnonymousInnerClassHelper : PrivilegedAction<Charset>
		{
			private string CharsetName;

			public PrivilegedActionAnonymousInnerClassHelper(string charsetName)
			{
				this.CharsetName = charsetName;
			}

			public virtual Charset Run()
			{
				for (IEnumerator<CharsetProvider> i = Providers(); i.MoveNext();)
				{
					CharsetProvider cp = i.Current;
					Charset cs = cp.CharsetForName(CharsetName);
					if (cs != null)
					{
						return cs;
					}
				}
				return null;
			}
		}

		/* The extended set of charsets */
		private class ExtendedProviderHolder
		{
			internal static readonly CharsetProvider ExtendedProvider_Renamed = ExtendedProvider();
			// returns ExtendedProvider, if installed
			internal static CharsetProvider ExtendedProvider()
			{
				return AccessController.doPrivileged(new PrivilegedActionAnonymousInnerClassHelper2());
			}

			private class PrivilegedActionAnonymousInnerClassHelper2 : PrivilegedAction<CharsetProvider>
			{
				public PrivilegedActionAnonymousInnerClassHelper2()
				{
				}

				public virtual CharsetProvider Run()
				{
					 try
					 {
						 Class epc = Class.ForName("sun.nio.cs.ext.ExtendedCharsets");
						 return (CharsetProvider)epc.NewInstance();
					 }
					 catch (ClassNotFoundException)
					 {
						 // Extended charsets not available
						 // (charsets.jar not present)
					 }
//JAVA TO C# CONVERTER TODO TASK: There is no equivalent in C# to Java 'multi-catch' syntax:
					 catch (InstantiationException | IllegalAccessException x)
					 {
					   throw new Error(x);
					 }
					 return null;
				}
			}
		}

		private static Charset LookupExtendedCharset(String charsetName)
		{
			CharsetProvider ecp = ExtendedProviderHolder.ExtendedProvider_Renamed;
			return (ecp != null) ? ecp.CharsetForName(charsetName) : null;
		}

		private static Charset Lookup(String charsetName)
		{
			if (charsetName == null)
			{
				throw new IllegalArgumentException("Null charset name");
			}
			Object[] a;
			if ((a = Cache1) != null && charsetName.Equals(a[0]))
			{
				return (Charset)a[1];
			}
			// We expect most programs to use one Charset repeatedly.
			// We convey a hint to this effect to the VM by putting the
			// level 1 cache miss code in a separate method.
			return Lookup2(charsetName);
		}

		private static Charset Lookup2(String charsetName)
		{
			Object[] a;
			if ((a = Cache2) != null && charsetName.Equals(a[0]))
			{
				Cache2 = Cache1;
				Cache1 = a;
				return (Charset)a[1];
			}
			Charset cs;
			if ((cs = StandardProvider.CharsetForName(charsetName)) != null || (cs = LookupExtendedCharset(charsetName)) != null || (cs = LookupViaProviders(charsetName)) != null)
			{
				Cache(charsetName, cs);
				return cs;
			}

			/* Only need to check the name if we didn't find a charset for it */
			CheckName(charsetName);
			return null;
		}

		/// <summary>
		/// Tells whether the named charset is supported.
		/// </summary>
		/// <param name="charsetName">
		///         The name of the requested charset; may be either
		///         a canonical name or an alias
		/// </param>
		/// <returns>  <tt>true</tt> if, and only if, support for the named charset
		///          is available in the current Java virtual machine
		/// </returns>
		/// <exception cref="IllegalCharsetNameException">
		///         If the given charset name is illegal
		/// </exception>
		/// <exception cref="IllegalArgumentException">
		///          If the given <tt>charsetName</tt> is null </exception>
		public static bool IsSupported(String charsetName)
		{
			return (Lookup(charsetName) != null);
		}

		/// <summary>
		/// Returns a charset object for the named charset.
		/// </summary>
		/// <param name="charsetName">
		///         The name of the requested charset; may be either
		///         a canonical name or an alias
		/// </param>
		/// <returns>  A charset object for the named charset
		/// </returns>
		/// <exception cref="IllegalCharsetNameException">
		///          If the given charset name is illegal
		/// </exception>
		/// <exception cref="IllegalArgumentException">
		///          If the given <tt>charsetName</tt> is null
		/// </exception>
		/// <exception cref="UnsupportedCharsetException">
		///          If no support for the named charset is available
		///          in this instance of the Java virtual machine </exception>
		public static Charset ForName(String charsetName)
		{
			Charset cs = Lookup(charsetName);
			if (cs != null)
			{
				return cs;
			}
			throw new UnsupportedCharsetException(charsetName);
		}

		// Fold charsets from the given iterator into the given map, ignoring
		// charsets whose names already have entries in the map.
		//
		private static void Put(IEnumerator<Charset> i, IDictionary<String, Charset> m)
		{
			while (i.MoveNext())
			{
				Charset cs = i.Current;
				if (!m.ContainsKey(cs.Name()))
				{
					m[cs.Name()] = cs;
				}
			}
		}

		/// <summary>
		/// Constructs a sorted map from canonical charset names to charset objects.
		/// 
		/// <para> The map returned by this method will have one entry for each charset
		/// for which support is available in the current Java virtual machine.  If
		/// two or more supported charsets have the same canonical name then the
		/// resulting map will contain just one of them; which one it will contain
		/// is not specified. </para>
		/// 
		/// <para> The invocation of this method, and the subsequent use of the
		/// resulting map, may cause time-consuming disk or network I/O operations
		/// to occur.  This method is provided for applications that need to
		/// enumerate all of the available charsets, for example to allow user
		/// charset selection.  This method is not used by the {@link #forName
		/// forName} method, which instead employs an efficient incremental lookup
		/// algorithm.
		/// 
		/// </para>
		/// <para> This method may return different results at different times if new
		/// charset providers are dynamically made available to the current Java
		/// virtual machine.  In the absence of such changes, the charsets returned
		/// by this method are exactly those that can be retrieved via the {@link
		/// #forName forName} method.  </para>
		/// </summary>
		/// <returns> An immutable, case-insensitive map from canonical charset names
		///         to charset objects </returns>
		public static SortedMap<String, Charset> AvailableCharsets()
		{
			return AccessController.doPrivileged(new PrivilegedActionAnonymousInnerClassHelper());
		}

		private class PrivilegedActionAnonymousInnerClassHelper : PrivilegedAction<SortedMap<String, Charset>>
		{
			public PrivilegedActionAnonymousInnerClassHelper()
			{
			}

			public virtual SortedMap<String, Charset> Run()
			{
				SortedDictionary<String, Charset> m = new SortedDictionary<String, Charset>(ASCIICaseInsensitiveComparator.CASE_INSENSITIVE_ORDER);
				Put(StandardProvider.Charsets(), m);
				CharsetProvider ecp = ExtendedProviderHolder.ExtendedProvider_Renamed;
				if (ecp != null)
				{
					Put(ecp.Charsets(), m);
				}
				for (IEnumerator<CharsetProvider> i = Providers(); i.MoveNext();)
				{
					CharsetProvider cp = i.Current;
					Put(cp.Charsets(), m);
				}
				return Collections.UnmodifiableSortedMap(m);
			}
		}

		private static volatile Charset DefaultCharset_Renamed;

		/// <summary>
		/// Returns the default charset of this Java virtual machine.
		/// 
		/// <para> The default charset is determined during virtual-machine startup and
		/// typically depends upon the locale and charset of the underlying
		/// operating system.
		/// 
		/// </para>
		/// </summary>
		/// <returns>  A charset object for the default charset
		/// 
		/// @since 1.5 </returns>
		public static Charset DefaultCharset()
		{
			if (DefaultCharset_Renamed == null)
			{
				lock (typeof(Charset))
				{
					String csn = AccessController.doPrivileged(new GetPropertyAction("file.encoding"));
					Charset cs = Lookup(csn);
					if (cs != null)
					{
						DefaultCharset_Renamed = cs;
					}
					else
					{
						DefaultCharset_Renamed = ForName("UTF-8");
					}
				}
			}
			return DefaultCharset_Renamed;
		}


		/* -- Instance fields and methods -- */

		private readonly String Name_Renamed; // tickles a bug in oldjavac
		private readonly String[] Aliases_Renamed; // tickles a bug in oldjavac
		private Set<String> AliasSet = null;

		/// <summary>
		/// Initializes a new charset with the given canonical name and alias
		/// set.
		/// </summary>
		/// <param name="canonicalName">
		///         The canonical name of this charset
		/// </param>
		/// <param name="aliases">
		///         An array of this charset's aliases, or null if it has no aliases
		/// </param>
		/// <exception cref="IllegalCharsetNameException">
		///         If the canonical name or any of the aliases are illegal </exception>
		protected internal Charset(String canonicalName, String[] aliases)
		{
			CheckName(canonicalName);
			String[] @as = (aliases == null) ? new String[0] : aliases;
			for (int i = 0; i < @as.Length; i++)
			{
				CheckName(@as[i]);
			}
			this.Name_Renamed = canonicalName;
			this.Aliases_Renamed = @as;
		}

		/// <summary>
		/// Returns this charset's canonical name.
		/// </summary>
		/// <returns>  The canonical name of this charset </returns>
		public String Name()
		{
			return Name_Renamed;
		}

		/// <summary>
		/// Returns a set containing this charset's aliases.
		/// </summary>
		/// <returns>  An immutable set of this charset's aliases </returns>
		public Set<String> Aliases()
		{
			if (AliasSet != null)
			{
				return AliasSet;
			}
			int n = Aliases_Renamed.Length;
			HashSet<String> hs = new HashSet<String>(n);
			for (int i = 0; i < n; i++)
			{
				hs.Add(Aliases_Renamed[i]);
			}
			AliasSet = Collections.UnmodifiableSet(hs);
			return AliasSet;
		}

		/// <summary>
		/// Returns this charset's human-readable name for the default locale.
		/// 
		/// <para> The default implementation of this method simply returns this
		/// charset's canonical name.  Concrete subclasses of this class may
		/// override this method in order to provide a localized display name. </para>
		/// </summary>
		/// <returns>  The display name of this charset in the default locale </returns>
		public virtual String DisplayName()
		{
			return Name_Renamed;
		}

		/// <summary>
		/// Tells whether or not this charset is registered in the <a
		/// href="http://www.iana.org/assignments/character-sets">IANA Charset
		/// Registry</a>.
		/// </summary>
		/// <returns>  <tt>true</tt> if, and only if, this charset is known by its
		///          implementor to be registered with the IANA </returns>
		public bool Registered
		{
			get
			{
				return !Name_Renamed.StartsWith("X-") && !Name_Renamed.StartsWith("x-");
			}
		}

		/// <summary>
		/// Returns this charset's human-readable name for the given locale.
		/// 
		/// <para> The default implementation of this method simply returns this
		/// charset's canonical name.  Concrete subclasses of this class may
		/// override this method in order to provide a localized display name. </para>
		/// </summary>
		/// <param name="locale">
		///         The locale for which the display name is to be retrieved
		/// </param>
		/// <returns>  The display name of this charset in the given locale </returns>
		public virtual String DisplayName(Locale locale)
		{
			return Name_Renamed;
		}

		/// <summary>
		/// Tells whether or not this charset contains the given charset.
		/// 
		/// <para> A charset <i>C</i> is said to <i>contain</i> a charset <i>D</i> if,
		/// and only if, every character representable in <i>D</i> is also
		/// representable in <i>C</i>.  If this relationship holds then it is
		/// guaranteed that every string that can be encoded in <i>D</i> can also be
		/// encoded in <i>C</i> without performing any replacements.
		/// 
		/// </para>
		/// <para> That <i>C</i> contains <i>D</i> does not imply that each character
		/// representable in <i>C</i> by a particular byte sequence is represented
		/// in <i>D</i> by the same byte sequence, although sometimes this is the
		/// case.
		/// 
		/// </para>
		/// <para> Every charset contains itself.
		/// 
		/// </para>
		/// <para> This method computes an approximation of the containment relation:
		/// If it returns <tt>true</tt> then the given charset is known to be
		/// contained by this charset; if it returns <tt>false</tt>, however, then
		/// it is not necessarily the case that the given charset is not contained
		/// in this charset.
		/// 
		/// </para>
		/// </summary>
		/// <param name="cs">
		///          The given charset
		/// </param>
		/// <returns>  <tt>true</tt> if the given charset is contained in this charset </returns>
		public abstract bool Contains(Charset cs);

		/// <summary>
		/// Constructs a new decoder for this charset.
		/// </summary>
		/// <returns>  A new decoder for this charset </returns>
		public abstract CharsetDecoder NewDecoder();

		/// <summary>
		/// Constructs a new encoder for this charset.
		/// </summary>
		/// <returns>  A new encoder for this charset
		/// </returns>
		/// <exception cref="UnsupportedOperationException">
		///          If this charset does not support encoding </exception>
		public abstract CharsetEncoder NewEncoder();

		/// <summary>
		/// Tells whether or not this charset supports encoding.
		/// 
		/// <para> Nearly all charsets support encoding.  The primary exceptions are
		/// special-purpose <i>auto-detect</i> charsets whose decoders can determine
		/// which of several possible encoding schemes is in use by examining the
		/// input byte sequence.  Such charsets do not support encoding because
		/// there is no way to determine which encoding should be used on output.
		/// Implementations of such charsets should override this method to return
		/// <tt>false</tt>. </para>
		/// </summary>
		/// <returns>  <tt>true</tt> if, and only if, this charset supports encoding </returns>
		public virtual bool CanEncode()
		{
			return true;
		}

		/// <summary>
		/// Convenience method that decodes bytes in this charset into Unicode
		/// characters.
		/// 
		/// <para> An invocation of this method upon a charset <tt>cs</tt> returns the
		/// same result as the expression
		/// 
		/// <pre>
		///     cs.newDecoder()
		///       .onMalformedInput(CodingErrorAction.REPLACE)
		///       .onUnmappableCharacter(CodingErrorAction.REPLACE)
		///       .decode(bb); </pre>
		/// 
		/// except that it is potentially more efficient because it can cache
		/// decoders between successive invocations.
		/// 
		/// </para>
		/// <para> This method always replaces malformed-input and unmappable-character
		/// sequences with this charset's default replacement byte array.  In order
		/// to detect such sequences, use the {@link
		/// CharsetDecoder#decode(java.nio.ByteBuffer)} method directly.  </para>
		/// </summary>
		/// <param name="bb">  The byte buffer to be decoded
		/// </param>
		/// <returns>  A char buffer containing the decoded characters </returns>
		public CharBuffer Decode(ByteBuffer bb)
		{
			try
			{
				return ThreadLocalCoders.decoderFor(this).onMalformedInput(CodingErrorAction.REPLACE).onUnmappableCharacter(CodingErrorAction.REPLACE).decode(bb);
			}
			catch (CharacterCodingException x)
			{
				throw new Error(x); // Can't happen
			}
		}

		/// <summary>
		/// Convenience method that encodes Unicode characters into bytes in this
		/// charset.
		/// 
		/// <para> An invocation of this method upon a charset <tt>cs</tt> returns the
		/// same result as the expression
		/// 
		/// <pre>
		///     cs.newEncoder()
		///       .onMalformedInput(CodingErrorAction.REPLACE)
		///       .onUnmappableCharacter(CodingErrorAction.REPLACE)
		///       .encode(bb); </pre>
		/// 
		/// except that it is potentially more efficient because it can cache
		/// encoders between successive invocations.
		/// 
		/// </para>
		/// <para> This method always replaces malformed-input and unmappable-character
		/// sequences with this charset's default replacement string.  In order to
		/// detect such sequences, use the {@link
		/// CharsetEncoder#encode(java.nio.CharBuffer)} method directly.  </para>
		/// </summary>
		/// <param name="cb">  The char buffer to be encoded
		/// </param>
		/// <returns>  A byte buffer containing the encoded characters </returns>
		public ByteBuffer Encode(CharBuffer cb)
		{
			try
			{
				return ThreadLocalCoders.encoderFor(this).onMalformedInput(CodingErrorAction.REPLACE).onUnmappableCharacter(CodingErrorAction.REPLACE).encode(cb);
			}
			catch (CharacterCodingException x)
			{
				throw new Error(x); // Can't happen
			}
		}

		/// <summary>
		/// Convenience method that encodes a string into bytes in this charset.
		/// 
		/// <para> An invocation of this method upon a charset <tt>cs</tt> returns the
		/// same result as the expression
		/// 
		/// <pre>
		///     cs.encode(CharBuffer.wrap(s)); </pre>
		/// 
		/// </para>
		/// </summary>
		/// <param name="str">  The string to be encoded
		/// </param>
		/// <returns>  A byte buffer containing the encoded characters </returns>
		public ByteBuffer Encode(String str)
		{
			return Encode(CharBuffer.Wrap(str));
		}

		/// <summary>
		/// Compares this charset to another.
		/// 
		/// <para> Charsets are ordered by their canonical names, without regard to
		/// case. </para>
		/// </summary>
		/// <param name="that">
		///         The charset to which this charset is to be compared
		/// </param>
		/// <returns> A negative integer, zero, or a positive integer as this charset
		///         is less than, equal to, or greater than the specified charset </returns>
		public int CompareTo(Charset that)
		{
			return (Name().CompareToIgnoreCase(that.Name()));
		}

		/// <summary>
		/// Computes a hashcode for this charset.
		/// </summary>
		/// <returns>  An integer hashcode </returns>
		public sealed override int HashCode()
		{
			return Name().HashCode();
		}

		/// <summary>
		/// Tells whether or not this object is equal to another.
		/// 
		/// <para> Two charsets are equal if, and only if, they have the same canonical
		/// names.  A charset is never equal to any other type of object.  </para>
		/// </summary>
		/// <returns>  <tt>true</tt> if, and only if, this charset is equal to the
		///          given object </returns>
		public sealed override bool Equals(Object ob)
		{
			if (!(ob is Charset))
			{
				return false;
			}
			if (this == ob)
			{
				return true;
			}
			return Name_Renamed.Equals(((Charset)ob).Name());
		}

		/// <summary>
		/// Returns a string describing this charset.
		/// </summary>
		/// <returns>  A string describing this charset </returns>
		public sealed override String ToString()
		{
			return Name();
		}

	}

}
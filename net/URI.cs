using System;
using System.Diagnostics;

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

namespace java.net
{

	using ThreadLocalCoders = sun.nio.cs.ThreadLocalCoders;



	/// <summary>
	/// Represents a Uniform Resource Identifier (URI) reference.
	/// 
	/// <para> Aside from some minor deviations noted below, an instance of this
	/// class represents a URI reference as defined by
	/// <a href="http://www.ietf.org/rfc/rfc2396.txt"><i>RFC&nbsp;2396: Uniform
	/// Resource Identifiers (URI): Generic Syntax</i></a>, amended by <a
	/// href="http://www.ietf.org/rfc/rfc2732.txt"><i>RFC&nbsp;2732: Format for
	/// Literal IPv6 Addresses in URLs</i></a>. The Literal IPv6 address format
	/// also supports scope_ids. The syntax and usage of scope_ids is described
	/// <a href="Inet6Address.html#scoped">here</a>.
	/// This class provides constructors for creating URI instances from
	/// their components or by parsing their string forms, methods for accessing the
	/// various components of an instance, and methods for normalizing, resolving,
	/// and relativizing URI instances.  Instances of this class are immutable.
	/// 
	/// 
	/// <h3> URI syntax and components </h3>
	/// 
	/// At the highest level a URI reference (hereinafter simply "URI") in string
	/// form has the syntax
	/// 
	/// <blockquote>
	/// [<i>scheme</i><b>{@code :}</b>]<i>scheme-specific-part</i>[<b>{@code #}</b><i>fragment</i>]
	/// </blockquote>
	/// 
	/// where square brackets [...] delineate optional components and the characters
	/// <b>{@code :}</b> and <b>{@code #}</b> stand for themselves.
	/// 
	/// </para>
	/// <para> An <i>absolute</i> URI specifies a scheme; a URI that is not absolute is
	/// said to be <i>relative</i>.  URIs are also classified according to whether
	/// they are <i>opaque</i> or <i>hierarchical</i>.
	/// 
	/// </para>
	/// <para> An <i>opaque</i> URI is an absolute URI whose scheme-specific part does
	/// not begin with a slash character ({@code '/'}).  Opaque URIs are not
	/// subject to further parsing.  Some examples of opaque URIs are:
	/// 
	/// <blockquote><table cellpadding=0 cellspacing=0 summary="layout">
	/// <tr><td>{@code mailto:java-net@java.sun.com}<td></tr>
	/// <tr><td>{@code news:comp.lang.java}<td></tr>
	/// <tr><td>{@code urn:isbn:096139210x}</td></tr>
	/// </table></blockquote>
	/// 
	/// </para>
	/// <para> A <i>hierarchical</i> URI is either an absolute URI whose
	/// scheme-specific part begins with a slash character, or a relative URI, that
	/// is, a URI that does not specify a scheme.  Some examples of hierarchical
	/// URIs are:
	/// 
	/// <blockquote>
	/// {@code http://java.sun.com/j2se/1.3/}<br>
	/// {@code docs/guide/collections/designfaq.html#28}<br>
	/// {@code ../../../demo/jfc/SwingSet2/src/SwingSet2.java}<br>
	/// {@code file:///~/calendar}
	/// </blockquote>
	/// 
	/// </para>
	/// <para> A hierarchical URI is subject to further parsing according to the syntax
	/// 
	/// <blockquote>
	/// [<i>scheme</i><b>{@code :}</b>][<b>{@code //}</b><i>authority</i>][<i>path</i>][<b>{@code ?}</b><i>query</i>][<b>{@code #}</b><i>fragment</i>]
	/// </blockquote>
	/// 
	/// where the characters <b>{@code :}</b>, <b>{@code /}</b>,
	/// <b>{@code ?}</b>, and <b>{@code #}</b> stand for themselves.  The
	/// scheme-specific part of a hierarchical URI consists of the characters
	/// between the scheme and fragment components.
	/// 
	/// </para>
	/// <para> The authority component of a hierarchical URI is, if specified, either
	/// <i>server-based</i> or <i>registry-based</i>.  A server-based authority
	/// parses according to the familiar syntax
	/// 
	/// <blockquote>
	/// [<i>user-info</i><b>{@code @}</b>]<i>host</i>[<b>{@code :}</b><i>port</i>]
	/// </blockquote>
	/// 
	/// where the characters <b>{@code @}</b> and <b>{@code :}</b> stand for
	/// themselves.  Nearly all URI schemes currently in use are server-based.  An
	/// authority component that does not parse in this way is considered to be
	/// registry-based.
	/// 
	/// </para>
	/// <para> The path component of a hierarchical URI is itself said to be absolute
	/// if it begins with a slash character ({@code '/'}); otherwise it is
	/// relative.  The path of a hierarchical URI that is either absolute or
	/// specifies an authority is always absolute.
	/// 
	/// </para>
	/// <para> All told, then, a URI instance has the following nine components:
	/// 
	/// <blockquote><table summary="Describes the components of a URI:scheme,scheme-specific-part,authority,user-info,host,port,path,query,fragment">
	/// <tr><th><i>Component</i></th><th><i>Type</i></th></tr>
	/// <tr><td>scheme</td><td>{@code String}</td></tr>
	/// <tr><td>scheme-specific-part&nbsp;&nbsp;&nbsp;&nbsp;</td><td>{@code String}</td></tr>
	/// <tr><td>authority</td><td>{@code String}</td></tr>
	/// <tr><td>user-info</td><td>{@code String}</td></tr>
	/// <tr><td>host</td><td>{@code String}</td></tr>
	/// <tr><td>port</td><td>{@code int}</td></tr>
	/// <tr><td>path</td><td>{@code String}</td></tr>
	/// <tr><td>query</td><td>{@code String}</td></tr>
	/// <tr><td>fragment</td><td>{@code String}</td></tr>
	/// </table></blockquote>
	/// 
	/// In a given instance any particular component is either <i>undefined</i> or
	/// <i>defined</i> with a distinct value.  Undefined string components are
	/// represented by {@code null}, while undefined integer components are
	/// represented by {@code -1}.  A string component may be defined to have the
	/// empty string as its value; this is not equivalent to that component being
	/// undefined.
	/// 
	/// </para>
	/// <para> Whether a particular component is or is not defined in an instance
	/// depends upon the type of the URI being represented.  An absolute URI has a
	/// scheme component.  An opaque URI has a scheme, a scheme-specific part, and
	/// possibly a fragment, but has no other components.  A hierarchical URI always
	/// has a path (though it may be empty) and a scheme-specific-part (which at
	/// least contains the path), and may have any of the other components.  If the
	/// authority component is present and is server-based then the host component
	/// will be defined and the user-information and port components may be defined.
	/// 
	/// 
	/// <h4> Operations on URI instances </h4>
	/// 
	/// The key operations supported by this class are those of
	/// <i>normalization</i>, <i>resolution</i>, and <i>relativization</i>.
	/// 
	/// </para>
	/// <para> <i>Normalization</i> is the process of removing unnecessary {@code "."}
	/// and {@code ".."} segments from the path component of a hierarchical URI.
	/// Each {@code "."} segment is simply removed.  A {@code ".."} segment is
	/// removed only if it is preceded by a non-{@code ".."} segment.
	/// Normalization has no effect upon opaque URIs.
	/// 
	/// </para>
	/// <para> <i>Resolution</i> is the process of resolving one URI against another,
	/// <i>base</i> URI.  The resulting URI is constructed from components of both
	/// URIs in the manner specified by RFC&nbsp;2396, taking components from the
	/// base URI for those not specified in the original.  For hierarchical URIs,
	/// the path of the original is resolved against the path of the base and then
	/// normalized.  The result, for example, of resolving
	/// 
	/// <blockquote>
	/// {@code docs/guide/collections/designfaq.html#28}
	/// &nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;
	/// &nbsp;&nbsp;&nbsp;&nbsp;(1)
	/// </blockquote>
	/// 
	/// against the base URI {@code http://java.sun.com/j2se/1.3/} is the result
	/// URI
	/// 
	/// <blockquote>
	/// {@code https://docs.oracle.com/javase/1.3/docs/guide/collections/designfaq.html#28}
	/// </blockquote>
	/// 
	/// Resolving the relative URI
	/// 
	/// <blockquote>
	/// {@code ../../../demo/jfc/SwingSet2/src/SwingSet2.java}&nbsp;&nbsp;&nbsp;&nbsp;(2)
	/// </blockquote>
	/// 
	/// against this result yields, in turn,
	/// 
	/// <blockquote>
	/// {@code http://java.sun.com/j2se/1.3/demo/jfc/SwingSet2/src/SwingSet2.java}
	/// </blockquote>
	/// 
	/// Resolution of both absolute and relative URIs, and of both absolute and
	/// relative paths in the case of hierarchical URIs, is supported.  Resolving
	/// the URI {@code file:///~calendar} against any other URI simply yields the
	/// original URI, since it is absolute.  Resolving the relative URI (2) above
	/// against the relative base URI (1) yields the normalized, but still relative,
	/// URI
	/// 
	/// <blockquote>
	/// {@code demo/jfc/SwingSet2/src/SwingSet2.java}
	/// </blockquote>
	/// 
	/// </para>
	/// <para> <i>Relativization</i>, finally, is the inverse of resolution: For any
	/// two normalized URIs <i>u</i> and&nbsp;<i>v</i>,
	/// 
	/// <blockquote>
	///   <i>u</i>{@code .relativize(}<i>u</i>{@code .resolve(}<i>v</i>{@code )).equals(}<i>v</i>{@code )}&nbsp;&nbsp;and<br>
	///   <i>u</i>{@code .resolve(}<i>u</i>{@code .relativize(}<i>v</i>{@code )).equals(}<i>v</i>{@code )}&nbsp;&nbsp;.<br>
	/// </blockquote>
	/// 
	/// This operation is often useful when constructing a document containing URIs
	/// that must be made relative to the base URI of the document wherever
	/// possible.  For example, relativizing the URI
	/// 
	/// <blockquote>
	/// {@code https://docs.oracle.com/javase/1.3/docs/guide/index.html}
	/// </blockquote>
	/// 
	/// against the base URI
	/// 
	/// <blockquote>
	/// {@code http://java.sun.com/j2se/1.3}
	/// </blockquote>
	/// 
	/// yields the relative URI {@code docs/guide/index.html}.
	/// 
	/// 
	/// <h4> Character categories </h4>
	/// 
	/// RFC&nbsp;2396 specifies precisely which characters are permitted in the
	/// various components of a URI reference.  The following categories, most of
	/// which are taken from that specification, are used below to describe these
	/// constraints:
	/// 
	/// <blockquote><table cellspacing=2 summary="Describes categories alpha,digit,alphanum,unreserved,punct,reserved,escaped,and other">
	///   <tr><th valign=top><i>alpha</i></th>
	///       <td>The US-ASCII alphabetic characters,
	///        {@code 'A'}&nbsp;through&nbsp;{@code 'Z'}
	///        and {@code 'a'}&nbsp;through&nbsp;{@code 'z'}</td></tr>
	///   <tr><th valign=top><i>digit</i></th>
	///       <td>The US-ASCII decimal digit characters,
	///       {@code '0'}&nbsp;through&nbsp;{@code '9'}</td></tr>
	///   <tr><th valign=top><i>alphanum</i></th>
	///       <td>All <i>alpha</i> and <i>digit</i> characters</td></tr>
	///   <tr><th valign=top><i>unreserved</i>&nbsp;&nbsp;&nbsp;&nbsp;</th>
	///       <td>All <i>alphanum</i> characters together with those in the string
	///        {@code "_-!.~'()*"}</td></tr>
	///   <tr><th valign=top><i>punct</i></th>
	///       <td>The characters in the string {@code ",;:$&+="}</td></tr>
	///   <tr><th valign=top><i>reserved</i></th>
	///       <td>All <i>punct</i> characters together with those in the string
	///        {@code "?/[]@"}</td></tr>
	///   <tr><th valign=top><i>escaped</i></th>
	///       <td>Escaped octets, that is, triplets consisting of the percent
	///           character ({@code '%'}) followed by two hexadecimal digits
	///           ({@code '0'}-{@code '9'}, {@code 'A'}-{@code 'F'}, and
	///           {@code 'a'}-{@code 'f'})</td></tr>
	///   <tr><th valign=top><i>other</i></th>
	///       <td>The Unicode characters that are not in the US-ASCII character set,
	///           are not control characters (according to the {@link
	///           java.lang.Character#isISOControl(char) Character.isISOControl}
	///           method), and are not space characters (according to the {@link
	///           java.lang.Character#isSpaceChar(char) Character.isSpaceChar}
	///           method)&nbsp;&nbsp;<i>(<b>Deviation from RFC 2396</b>, which is
	///           limited to US-ASCII)</i></td></tr>
	/// </table></blockquote>
	/// 
	/// </para>
	/// <para><a name="legal-chars"></a> The set of all legal URI characters consists of
	/// the <i>unreserved</i>, <i>reserved</i>, <i>escaped</i>, and <i>other</i>
	/// characters.
	/// 
	/// 
	/// <h4> Escaped octets, quotation, encoding, and decoding </h4>
	/// 
	/// RFC 2396 allows escaped octets to appear in the user-info, path, query, and
	/// fragment components.  Escaping serves two purposes in URIs:
	/// 
	/// <ul>
	/// 
	/// </para>
	///   <li><para> To <i>encode</i> non-US-ASCII characters when a URI is required to
	///   conform strictly to RFC&nbsp;2396 by not containing any <i>other</i>
	///   characters.  </para></li>
	/// 
	///   <li><para> To <i>quote</i> characters that are otherwise illegal in a
	///   component.  The user-info, path, query, and fragment components differ
	///   slightly in terms of which characters are considered legal and illegal.
	///   </para></li>
	/// 
	/// </ul>
	/// 
	/// These purposes are served in this class by three related operations:
	/// 
	/// <ul>
	/// 
	///   <li><para><a name="encode"></a> A character is <i>encoded</i> by replacing it
	///   with the sequence of escaped octets that represent that character in the
	///   UTF-8 character set.  The Euro currency symbol ({@code '\u005Cu20AC'}),
	///   for example, is encoded as {@code "%E2%82%AC"}.  <i>(<b>Deviation from
	///   RFC&nbsp;2396</b>, which does not specify any particular character
	///   set.)</i> </para></li>
	/// 
	///   <li><para><a name="quote"></a> An illegal character is <i>quoted</i> simply by
	///   encoding it.  The space character, for example, is quoted by replacing it
	///   with {@code "%20"}.  UTF-8 contains US-ASCII, hence for US-ASCII
	///   characters this transformation has exactly the effect required by
	///   RFC&nbsp;2396. </para></li>
	/// 
	///   <li><para><a name="decode"></a>
	///   A sequence of escaped octets is <i>decoded</i> by
	///   replacing it with the sequence of characters that it represents in the
	///   UTF-8 character set.  UTF-8 contains US-ASCII, hence decoding has the
	///   effect of de-quoting any quoted US-ASCII characters as well as that of
	///   decoding any encoded non-US-ASCII characters.  If a <a
	///   href="../nio/charset/CharsetDecoder.html#ce">decoding error</a> occurs
	///   when decoding the escaped octets then the erroneous octets are replaced by
	///   {@code '\u005CuFFFD'}, the Unicode replacement character.  </para></li>
	/// 
	/// </ul>
	/// 
	/// These operations are exposed in the constructors and methods of this class
	/// as follows:
	/// 
	/// <ul>
	/// 
	///   <li><para> The {@link #URI(java.lang.String) single-argument
	///   constructor} requires any illegal characters in its argument to be
	///   quoted and preserves any escaped octets and <i>other</i> characters that
	///   are present.  </para></li>
	/// 
	///   <li><para> The {@linkplain
	///   #URI(java.lang.String,java.lang.String,java.lang.String,int,java.lang.String,java.lang.String,java.lang.String)
	///   multi-argument constructors} quote illegal characters as
	///   required by the components in which they appear.  The percent character
	///   ({@code '%'}) is always quoted by these constructors.  Any <i>other</i>
	///   characters are preserved.  </para></li>
	/// 
	///   <li><para> The <seealso cref="#getRawUserInfo() getRawUserInfo"/>, {@link #getRawPath()
	///   getRawPath}, <seealso cref="#getRawQuery() getRawQuery"/>, {@link #getRawFragment()
	///   getRawFragment}, <seealso cref="#getRawAuthority() getRawAuthority"/>, and {@link
	///   #getRawSchemeSpecificPart() getRawSchemeSpecificPart} methods return the
	///   values of their corresponding components in raw form, without interpreting
	///   any escaped octets.  The strings returned by these methods may contain
	///   both escaped octets and <i>other</i> characters, and will not contain any
	///   illegal characters.  </para></li>
	/// 
	///   <li><para> The <seealso cref="#getUserInfo() getUserInfo"/>, {@link #getPath()
	///   getPath}, <seealso cref="#getQuery() getQuery"/>, {@link #getFragment()
	///   getFragment}, <seealso cref="#getAuthority() getAuthority"/>, and {@link
	///   #getSchemeSpecificPart() getSchemeSpecificPart} methods decode any escaped
	///   octets in their corresponding components.  The strings returned by these
	///   methods may contain both <i>other</i> characters and illegal characters,
	///   and will not contain any escaped octets.  </para></li>
	/// 
	///   <li><para> The <seealso cref="#toString() toString"/> method returns a URI string with
	///   all necessary quotation but which may contain <i>other</i> characters.
	///   </para></li>
	/// 
	///   <li><para> The <seealso cref="#toASCIIString() toASCIIString"/> method returns a fully
	///   quoted and encoded URI string that does not contain any <i>other</i>
	///   characters.  </para></li>
	/// 
	/// </ul>
	/// 
	/// 
	/// <h4> Identities </h4>
	/// 
	/// For any URI <i>u</i>, it is always the case that
	/// 
	/// <blockquote>
	/// {@code new URI(}<i>u</i>{@code .toString()).equals(}<i>u</i>{@code )}&nbsp;.
	/// </blockquote>
	/// 
	/// For any URI <i>u</i> that does not contain redundant syntax such as two
	/// slashes before an empty authority (as in {@code file:///tmp/}&nbsp;) or a
	/// colon following a host name but no port (as in
	/// {@code http://java.sun.com:}&nbsp;), and that does not encode characters
	/// except those that must be quoted, the following identities also hold:
	/// <pre>
	///     new URI(<i>u</i>.getScheme(),
	///             <i>u</i>.getSchemeSpecificPart(),
	///             <i>u</i>.getFragment())
	///     .equals(<i>u</i>)</pre>
	/// in all cases,
	/// <pre>
	///     new URI(<i>u</i>.getScheme(),
	///             <i>u</i>.getUserInfo(), <i>u</i>.getAuthority(),
	///             <i>u</i>.getPath(), <i>u</i>.getQuery(),
	///             <i>u</i>.getFragment())
	///     .equals(<i>u</i>)</pre>
	/// if <i>u</i> is hierarchical, and
	/// <pre>
	///     new URI(<i>u</i>.getScheme(),
	///             <i>u</i>.getUserInfo(), <i>u</i>.getHost(), <i>u</i>.getPort(),
	///             <i>u</i>.getPath(), <i>u</i>.getQuery(),
	///             <i>u</i>.getFragment())
	///     .equals(<i>u</i>)</pre>
	/// if <i>u</i> is hierarchical and has either no authority or a server-based
	/// authority.
	/// 
	/// 
	/// <h4> URIs, URLs, and URNs </h4>
	/// 
	/// A URI is a uniform resource <i>identifier</i> while a URL is a uniform
	/// resource <i>locator</i>.  Hence every URL is a URI, abstractly speaking, but
	/// not every URI is a URL.  This is because there is another subcategory of
	/// URIs, uniform resource <i>names</i> (URNs), which name resources but do not
	/// specify how to locate them.  The {@code mailto}, {@code news}, and
	/// {@code isbn} URIs shown above are examples of URNs.
	/// 
	/// <para> The conceptual distinction between URIs and URLs is reflected in the
	/// differences between this class and the <seealso cref="URL"/> class.
	/// 
	/// </para>
	/// <para> An instance of this class represents a URI reference in the syntactic
	/// sense defined by RFC&nbsp;2396.  A URI may be either absolute or relative.
	/// A URI string is parsed according to the generic syntax without regard to the
	/// scheme, if any, that it specifies.  No lookup of the host, if any, is
	/// performed, and no scheme-dependent stream handler is constructed.  Equality,
	/// hashing, and comparison are defined strictly in terms of the character
	/// content of the instance.  In other words, a URI instance is little more than
	/// a structured string that supports the syntactic, scheme-independent
	/// operations of comparison, normalization, resolution, and relativization.
	/// 
	/// </para>
	/// <para> An instance of the <seealso cref="URL"/> class, by contrast, represents the
	/// syntactic components of a URL together with some of the information required
	/// to access the resource that it describes.  A URL must be absolute, that is,
	/// it must always specify a scheme.  A URL string is parsed according to its
	/// scheme.  A stream handler is always established for a URL, and in fact it is
	/// impossible to create a URL instance for a scheme for which no handler is
	/// available.  Equality and hashing depend upon both the scheme and the
	/// Internet address of the host, if any; comparison is not defined.  In other
	/// words, a URL is a structured string that supports the syntactic operation of
	/// resolution as well as the network I/O operations of looking up the host and
	/// opening a connection to the specified resource.
	/// 
	/// 
	/// @author Mark Reinhold
	/// @since 1.4
	/// 
	/// </para>
	/// </summary>
	/// <seealso cref= <a href="http://www.ietf.org/rfc/rfc2279.txt"><i>RFC&nbsp;2279: UTF-8, a
	/// transformation format of ISO 10646</i></a>, <br><a
	/// href="http://www.ietf.org/rfc/rfc2373.txt"><i>RFC&nbsp;2373: IPv6 Addressing
	/// Architecture</i></a>, <br><a
	/// href="http://www.ietf.org/rfc/rfc2396.txt"><i>RFC&nbsp;2396: Uniform
	/// Resource Identifiers (URI): Generic Syntax</i></a>, <br><a
	/// href="http://www.ietf.org/rfc/rfc2732.txt"><i>RFC&nbsp;2732: Format for
	/// Literal IPv6 Addresses in URLs</i></a>, <br><a
	/// href="URISyntaxException.html">URISyntaxException</a> </seealso>

	[Serializable]
	public sealed class URI : Comparable<URI>
	{

		// Note: Comments containing the word "ASSERT" indicate places where a
		// throw of an InternalError should be replaced by an appropriate assertion
		// statement once asserts are enabled in the build.

		internal const long SerialVersionUID = -6052424284110960213L;


		// -- Properties and components of this instance --

		// Components of all URIs: [<scheme>:]<scheme-specific-part>[#<fragment>]
		[NonSerialized]
		private String Scheme_Renamed; // null ==> relative URI
		[NonSerialized]
		private String Fragment_Renamed;

		// Hierarchical URI components: [//<authority>]<path>[?<query>]
		[NonSerialized]
		private String Authority_Renamed; // Registry or server

		// Server-based authority: [<userInfo>@]<host>[:<port>]
		[NonSerialized]
		private String UserInfo_Renamed;
		[NonSerialized]
		private String Host_Renamed; // null ==> registry-based
		[NonSerialized]
		private int Port_Renamed = -1; // -1 ==> undefined

		// Remaining components of hierarchical URIs
		[NonSerialized]
		private String Path_Renamed; // null ==> opaque
		[NonSerialized]
		private String Query_Renamed;

		// The remaining fields may be computed on demand

		[NonSerialized]
		private volatile String SchemeSpecificPart_Renamed;
		[NonSerialized]
		private volatile int Hash_Renamed; // Zero ==> undefined

		[NonSerialized]
		private volatile String DecodedUserInfo = null;
		[NonSerialized]
		private volatile String DecodedAuthority = null;
		[NonSerialized]
		private volatile String DecodedPath = null;
		[NonSerialized]
		private volatile String DecodedQuery = null;
		[NonSerialized]
		private volatile String DecodedFragment = null;
		[NonSerialized]
		private volatile String DecodedSchemeSpecificPart = null;

		/// <summary>
		/// The string form of this URI.
		/// 
		/// @serial
		/// </summary>
		private volatile String @string; // The only serializable field



		// -- Constructors and factories --

		private URI() // Used internally
		{
		}

		/// <summary>
		/// Constructs a URI by parsing the given string.
		/// 
		/// <para> This constructor parses the given string exactly as specified by the
		/// grammar in <a
		/// href="http://www.ietf.org/rfc/rfc2396.txt">RFC&nbsp;2396</a>,
		/// Appendix&nbsp;A, <b><i>except for the following deviations:</i></b> </para>
		/// 
		/// <ul>
		/// 
		///   <li><para> An empty authority component is permitted as long as it is
		///   followed by a non-empty path, a query component, or a fragment
		///   component.  This allows the parsing of URIs such as
		///   {@code "file:///foo/bar"}, which seems to be the intent of
		///   RFC&nbsp;2396 although the grammar does not permit it.  If the
		///   authority component is empty then the user-information, host, and port
		///   components are undefined. </para></li>
		/// 
		///   <li><para> Empty relative paths are permitted; this seems to be the
		///   intent of RFC&nbsp;2396 although the grammar does not permit it.  The
		///   primary consequence of this deviation is that a standalone fragment
		///   such as {@code "#foo"} parses as a relative URI with an empty path
		///   and the given fragment, and can be usefully <a
		///   href="#resolve-frag">resolved</a> against a base URI.
		/// 
		/// </para>
		///   <li><para> IPv4 addresses in host components are parsed rigorously, as
		///   specified by <a
		///   href="http://www.ietf.org/rfc/rfc2732.txt">RFC&nbsp;2732</a>: Each
		///   element of a dotted-quad address must contain no more than three
		///   decimal digits.  Each element is further constrained to have a value
		///   no greater than 255. </para></li>
		/// 
		///   <li> <para> Hostnames in host components that comprise only a single
		///   domain label are permitted to start with an <i>alphanum</i>
		///   character. This seems to be the intent of <a
		///   href="http://www.ietf.org/rfc/rfc2396.txt">RFC&nbsp;2396</a>
		///   section&nbsp;3.2.2 although the grammar does not permit it. The
		///   consequence of this deviation is that the authority component of a
		///   hierarchical URI such as {@code s://123}, will parse as a server-based
		///   authority. </para></li>
		/// 
		///   <li><para> IPv6 addresses are permitted for the host component.  An IPv6
		///   address must be enclosed in square brackets ({@code '['} and
		///   {@code ']'}) as specified by <a
		///   href="http://www.ietf.org/rfc/rfc2732.txt">RFC&nbsp;2732</a>.  The
		///   IPv6 address itself must parse according to <a
		///   href="http://www.ietf.org/rfc/rfc2373.txt">RFC&nbsp;2373</a>.  IPv6
		///   addresses are further constrained to describe no more than sixteen
		///   bytes of address information, a constraint implicit in RFC&nbsp;2373
		///   but not expressible in the grammar. </para></li>
		/// 
		///   <li><para> Characters in the <i>other</i> category are permitted wherever
		///   RFC&nbsp;2396 permits <i>escaped</i> octets, that is, in the
		///   user-information, path, query, and fragment components, as well as in
		///   the authority component if the authority is registry-based.  This
		///   allows URIs to contain Unicode characters beyond those in the US-ASCII
		///   character set. </para></li>
		/// 
		/// </ul>
		/// </summary>
		/// <param name="str">   The string to be parsed into a URI
		/// </param>
		/// <exception cref="NullPointerException">
		///          If {@code str} is {@code null}
		/// </exception>
		/// <exception cref="URISyntaxException">
		///          If the given string violates RFC&nbsp;2396, as augmented
		///          by the above deviations </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public URI(String str) throws URISyntaxException
		public URI(String str)
		{
			(new Parser(this, str)).Parse(false);
		}

		/// <summary>
		/// Constructs a hierarchical URI from the given components.
		/// 
		/// <para> If a scheme is given then the path, if also given, must either be
		/// empty or begin with a slash character ({@code '/'}).  Otherwise a
		/// component of the new URI may be left undefined by passing {@code null}
		/// for the corresponding parameter or, in the case of the {@code port}
		/// parameter, by passing {@code -1}.
		/// 
		/// </para>
		/// <para> This constructor first builds a URI string from the given components
		/// according to the rules specified in <a
		/// href="http://www.ietf.org/rfc/rfc2396.txt">RFC&nbsp;2396</a>,
		/// section&nbsp;5.2, step&nbsp;7: </para>
		/// 
		/// <ol>
		/// 
		///   <li><para> Initially, the result string is empty. </para></li>
		/// 
		///   <li><para> If a scheme is given then it is appended to the result,
		///   followed by a colon character ({@code ':'}).  </para></li>
		/// 
		///   <li><para> If user information, a host, or a port are given then the
		///   string {@code "//"} is appended.  </para></li>
		/// 
		///   <li><para> If user information is given then it is appended, followed by
		///   a commercial-at character ({@code '@'}).  Any character not in the
		///   <i>unreserved</i>, <i>punct</i>, <i>escaped</i>, or <i>other</i>
		///   categories is <a href="#quote">quoted</a>.  </para></li>
		/// 
		///   <li><para> If a host is given then it is appended.  If the host is a
		///   literal IPv6 address but is not enclosed in square brackets
		///   ({@code '['} and {@code ']'}) then the square brackets are added.
		///   </para></li>
		/// 
		///   <li><para> If a port number is given then a colon character
		///   ({@code ':'}) is appended, followed by the port number in decimal.
		///   </para></li>
		/// 
		///   <li><para> If a path is given then it is appended.  Any character not in
		///   the <i>unreserved</i>, <i>punct</i>, <i>escaped</i>, or <i>other</i>
		///   categories, and not equal to the slash character ({@code '/'}) or the
		///   commercial-at character ({@code '@'}), is quoted.  </para></li>
		/// 
		///   <li><para> If a query is given then a question-mark character
		///   ({@code '?'}) is appended, followed by the query.  Any character that
		///   is not a <a href="#legal-chars">legal URI character</a> is quoted.
		///   </para></li>
		/// 
		///   <li><para> Finally, if a fragment is given then a hash character
		///   ({@code '#'}) is appended, followed by the fragment.  Any character
		///   that is not a legal URI character is quoted.  </para></li>
		/// 
		/// </ol>
		/// 
		/// <para> The resulting URI string is then parsed as if by invoking the {@link
		/// #URI(String)} constructor and then invoking the {@link
		/// #parseServerAuthority()} method upon the result; this may cause a {@link
		/// URISyntaxException} to be thrown.  </para>
		/// </summary>
		/// <param name="scheme">    Scheme name </param>
		/// <param name="userInfo">  User name and authorization information </param>
		/// <param name="host">      Host name </param>
		/// <param name="port">      Port number </param>
		/// <param name="path">      Path </param>
		/// <param name="query">     Query </param>
		/// <param name="fragment">  Fragment
		/// </param>
		/// <exception cref="URISyntaxException">
		///         If both a scheme and a path are given but the path is relative,
		///         if the URI string constructed from the given components violates
		///         RFC&nbsp;2396, or if the authority component of the string is
		///         present but cannot be parsed as a server-based authority </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public URI(String scheme, String userInfo, String host, int port, String path, String query, String fragment) throws URISyntaxException
		public URI(String scheme, String userInfo, String host, int port, String path, String query, String fragment)
		{
			String s = ToString(scheme, null, null, userInfo, host, port, path, query, fragment);
			CheckPath(s, scheme, path);
			(new Parser(this, s)).Parse(true);
		}

		/// <summary>
		/// Constructs a hierarchical URI from the given components.
		/// 
		/// <para> If a scheme is given then the path, if also given, must either be
		/// empty or begin with a slash character ({@code '/'}).  Otherwise a
		/// component of the new URI may be left undefined by passing {@code null}
		/// for the corresponding parameter.
		/// 
		/// </para>
		/// <para> This constructor first builds a URI string from the given components
		/// according to the rules specified in <a
		/// href="http://www.ietf.org/rfc/rfc2396.txt">RFC&nbsp;2396</a>,
		/// section&nbsp;5.2, step&nbsp;7: </para>
		/// 
		/// <ol>
		/// 
		///   <li><para> Initially, the result string is empty.  </para></li>
		/// 
		///   <li><para> If a scheme is given then it is appended to the result,
		///   followed by a colon character ({@code ':'}).  </para></li>
		/// 
		///   <li><para> If an authority is given then the string {@code "//"} is
		///   appended, followed by the authority.  If the authority contains a
		///   literal IPv6 address then the address must be enclosed in square
		///   brackets ({@code '['} and {@code ']'}).  Any character not in the
		///   <i>unreserved</i>, <i>punct</i>, <i>escaped</i>, or <i>other</i>
		///   categories, and not equal to the commercial-at character
		///   ({@code '@'}), is <a href="#quote">quoted</a>.  </para></li>
		/// 
		///   <li><para> If a path is given then it is appended.  Any character not in
		///   the <i>unreserved</i>, <i>punct</i>, <i>escaped</i>, or <i>other</i>
		///   categories, and not equal to the slash character ({@code '/'}) or the
		///   commercial-at character ({@code '@'}), is quoted.  </para></li>
		/// 
		///   <li><para> If a query is given then a question-mark character
		///   ({@code '?'}) is appended, followed by the query.  Any character that
		///   is not a <a href="#legal-chars">legal URI character</a> is quoted.
		///   </para></li>
		/// 
		///   <li><para> Finally, if a fragment is given then a hash character
		///   ({@code '#'}) is appended, followed by the fragment.  Any character
		///   that is not a legal URI character is quoted.  </para></li>
		/// 
		/// </ol>
		/// 
		/// <para> The resulting URI string is then parsed as if by invoking the {@link
		/// #URI(String)} constructor and then invoking the {@link
		/// #parseServerAuthority()} method upon the result; this may cause a {@link
		/// URISyntaxException} to be thrown.  </para>
		/// </summary>
		/// <param name="scheme">     Scheme name </param>
		/// <param name="authority">  Authority </param>
		/// <param name="path">       Path </param>
		/// <param name="query">      Query </param>
		/// <param name="fragment">   Fragment
		/// </param>
		/// <exception cref="URISyntaxException">
		///         If both a scheme and a path are given but the path is relative,
		///         if the URI string constructed from the given components violates
		///         RFC&nbsp;2396, or if the authority component of the string is
		///         present but cannot be parsed as a server-based authority </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public URI(String scheme, String authority, String path, String query, String fragment) throws URISyntaxException
		public URI(String scheme, String authority, String path, String query, String fragment)
		{
			String s = ToString(scheme, null, authority, null, null, -1, path, query, fragment);
			CheckPath(s, scheme, path);
			(new Parser(this, s)).Parse(false);
		}

		/// <summary>
		/// Constructs a hierarchical URI from the given components.
		/// 
		/// <para> A component may be left undefined by passing {@code null}.
		/// 
		/// </para>
		/// <para> This convenience constructor works as if by invoking the
		/// seven-argument constructor as follows:
		/// 
		/// <blockquote>
		/// {@code new} {@link #URI(String, String, String, int, String, String, String)
		/// URI}{@code (scheme, null, host, -1, path, null, fragment);}
		/// </blockquote>
		/// 
		/// </para>
		/// </summary>
		/// <param name="scheme">    Scheme name </param>
		/// <param name="host">      Host name </param>
		/// <param name="path">      Path </param>
		/// <param name="fragment">  Fragment
		/// </param>
		/// <exception cref="URISyntaxException">
		///          If the URI string constructed from the given components
		///          violates RFC&nbsp;2396 </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public URI(String scheme, String host, String path, String fragment) throws URISyntaxException
		public URI(String scheme, String host, String path, String fragment) : this(scheme, null, host, -1, path, null, fragment)
		{
		}

		/// <summary>
		/// Constructs a URI from the given components.
		/// 
		/// <para> A component may be left undefined by passing {@code null}.
		/// 
		/// </para>
		/// <para> This constructor first builds a URI in string form using the given
		/// components as follows:  </para>
		/// 
		/// <ol>
		/// 
		///   <li><para> Initially, the result string is empty.  </para></li>
		/// 
		///   <li><para> If a scheme is given then it is appended to the result,
		///   followed by a colon character ({@code ':'}).  </para></li>
		/// 
		///   <li><para> If a scheme-specific part is given then it is appended.  Any
		///   character that is not a <a href="#legal-chars">legal URI character</a>
		///   is <a href="#quote">quoted</a>.  </para></li>
		/// 
		///   <li><para> Finally, if a fragment is given then a hash character
		///   ({@code '#'}) is appended to the string, followed by the fragment.
		///   Any character that is not a legal URI character is quoted.  </para></li>
		/// 
		/// </ol>
		/// 
		/// <para> The resulting URI string is then parsed in order to create the new
		/// URI instance as if by invoking the <seealso cref="#URI(String)"/> constructor;
		/// this may cause a <seealso cref="URISyntaxException"/> to be thrown.  </para>
		/// </summary>
		/// <param name="scheme">    Scheme name </param>
		/// <param name="ssp">       Scheme-specific part </param>
		/// <param name="fragment">  Fragment
		/// </param>
		/// <exception cref="URISyntaxException">
		///          If the URI string constructed from the given components
		///          violates RFC&nbsp;2396 </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public URI(String scheme, String ssp, String fragment) throws URISyntaxException
		public URI(String scheme, String ssp, String fragment)
		{
			(new Parser(this, ToString(scheme, ssp, null, null, null, -1, null, null, fragment))).Parse(false);
		}

		/// <summary>
		/// Creates a URI by parsing the given string.
		/// 
		/// <para> This convenience factory method works as if by invoking the {@link
		/// #URI(String)} constructor; any <seealso cref="URISyntaxException"/> thrown by the
		/// constructor is caught and wrapped in a new {@link
		/// IllegalArgumentException} object, which is then thrown.
		/// 
		/// </para>
		/// <para> This method is provided for use in situations where it is known that
		/// the given string is a legal URI, for example for URI constants declared
		/// within in a program, and so it would be considered a programming error
		/// for the string not to parse as such.  The constructors, which throw
		/// <seealso cref="URISyntaxException"/> directly, should be used situations where a
		/// URI is being constructed from user input or from some other source that
		/// may be prone to errors.  </para>
		/// </summary>
		/// <param name="str">   The string to be parsed into a URI </param>
		/// <returns> The new URI
		/// </returns>
		/// <exception cref="NullPointerException">
		///          If {@code str} is {@code null}
		/// </exception>
		/// <exception cref="IllegalArgumentException">
		///          If the given string violates RFC&nbsp;2396 </exception>
		public static URI Create(String str)
		{
			try
			{
				return new URI(str);
			}
			catch (URISyntaxException x)
			{
				throw new IllegalArgumentException(x.Message, x);
			}
		}


		// -- Operations --

		/// <summary>
		/// Attempts to parse this URI's authority component, if defined, into
		/// user-information, host, and port components.
		/// 
		/// <para> If this URI's authority component has already been recognized as
		/// being server-based then it will already have been parsed into
		/// user-information, host, and port components.  In this case, or if this
		/// URI has no authority component, this method simply returns this URI.
		/// 
		/// </para>
		/// <para> Otherwise this method attempts once more to parse the authority
		/// component into user-information, host, and port components, and throws
		/// an exception describing why the authority component could not be parsed
		/// in that way.
		/// 
		/// </para>
		/// <para> This method is provided because the generic URI syntax specified in
		/// <a href="http://www.ietf.org/rfc/rfc2396.txt">RFC&nbsp;2396</a>
		/// cannot always distinguish a malformed server-based authority from a
		/// legitimate registry-based authority.  It must therefore treat some
		/// instances of the former as instances of the latter.  The authority
		/// component in the URI string {@code "//foo:bar"}, for example, is not a
		/// legal server-based authority but it is legal as a registry-based
		/// authority.
		/// 
		/// </para>
		/// <para> In many common situations, for example when working URIs that are
		/// known to be either URNs or URLs, the hierarchical URIs being used will
		/// always be server-based.  They therefore must either be parsed as such or
		/// treated as an error.  In these cases a statement such as
		/// 
		/// <blockquote>
		/// {@code URI }<i>u</i>{@code  = new URI(str).parseServerAuthority();}
		/// </blockquote>
		/// 
		/// </para>
		/// <para> can be used to ensure that <i>u</i> always refers to a URI that, if
		/// it has an authority component, has a server-based authority with proper
		/// user-information, host, and port components.  Invoking this method also
		/// ensures that if the authority could not be parsed in that way then an
		/// appropriate diagnostic message can be issued based upon the exception
		/// that is thrown. </para>
		/// </summary>
		/// <returns>  A URI whose authority field has been parsed
		///          as a server-based authority
		/// </returns>
		/// <exception cref="URISyntaxException">
		///          If the authority component of this URI is defined
		///          but cannot be parsed as a server-based authority
		///          according to RFC&nbsp;2396 </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public URI parseServerAuthority() throws URISyntaxException
		public URI ParseServerAuthority()
		{
			// We could be clever and cache the error message and index from the
			// exception thrown during the original parse, but that would require
			// either more fields or a more-obscure representation.
			if ((Host_Renamed != null) || (Authority_Renamed == null))
			{
				return this;
			}
			DefineString();
			(new Parser(this, @string)).Parse(true);
			return this;
		}

		/// <summary>
		/// Normalizes this URI's path.
		/// 
		/// <para> If this URI is opaque, or if its path is already in normal form,
		/// then this URI is returned.  Otherwise a new URI is constructed that is
		/// identical to this URI except that its path is computed by normalizing
		/// this URI's path in a manner consistent with <a
		/// href="http://www.ietf.org/rfc/rfc2396.txt">RFC&nbsp;2396</a>,
		/// section&nbsp;5.2, step&nbsp;6, sub-steps&nbsp;c through&nbsp;f; that is:
		/// </para>
		/// 
		/// <ol>
		/// 
		///   <li><para> All {@code "."} segments are removed. </para></li>
		/// 
		///   <li><para> If a {@code ".."} segment is preceded by a non-{@code ".."}
		///   segment then both of these segments are removed.  This step is
		///   repeated until it is no longer applicable. </para></li>
		/// 
		///   <li><para> If the path is relative, and if its first segment contains a
		///   colon character ({@code ':'}), then a {@code "."} segment is
		///   prepended.  This prevents a relative URI with a path such as
		///   {@code "a:b/c/d"} from later being re-parsed as an opaque URI with a
		///   scheme of {@code "a"} and a scheme-specific part of {@code "b/c/d"}.
		///   <b><i>(Deviation from RFC&nbsp;2396)</i></b> </para></li>
		/// 
		/// </ol>
		/// 
		/// <para> A normalized path will begin with one or more {@code ".."} segments
		/// if there were insufficient non-{@code ".."} segments preceding them to
		/// allow their removal.  A normalized path will begin with a {@code "."}
		/// segment if one was inserted by step 3 above.  Otherwise, a normalized
		/// path will not contain any {@code "."} or {@code ".."} segments. </para>
		/// </summary>
		/// <returns>  A URI equivalent to this URI,
		///          but whose path is in normal form </returns>
		public URI Normalize()
		{
			return Normalize(this);
		}

		/// <summary>
		/// Resolves the given URI against this URI.
		/// 
		/// <para> If the given URI is already absolute, or if this URI is opaque, then
		/// the given URI is returned.
		/// 
		/// </para>
		/// <para><a name="resolve-frag"></a> If the given URI's fragment component is
		/// defined, its path component is empty, and its scheme, authority, and
		/// query components are undefined, then a URI with the given fragment but
		/// with all other components equal to those of this URI is returned.  This
		/// allows a URI representing a standalone fragment reference, such as
		/// {@code "#foo"}, to be usefully resolved against a base URI.
		/// 
		/// </para>
		/// <para> Otherwise this method constructs a new hierarchical URI in a manner
		/// consistent with <a
		/// href="http://www.ietf.org/rfc/rfc2396.txt">RFC&nbsp;2396</a>,
		/// section&nbsp;5.2; that is: </para>
		/// 
		/// <ol>
		/// 
		///   <li><para> A new URI is constructed with this URI's scheme and the given
		///   URI's query and fragment components. </para></li>
		/// 
		///   <li><para> If the given URI has an authority component then the new URI's
		///   authority and path are taken from the given URI. </para></li>
		/// 
		///   <li><para> Otherwise the new URI's authority component is copied from
		///   this URI, and its path is computed as follows: </para>
		/// 
		///   <ol>
		/// 
		///     <li><para> If the given URI's path is absolute then the new URI's path
		///     is taken from the given URI. </para></li>
		/// 
		///     <li><para> Otherwise the given URI's path is relative, and so the new
		///     URI's path is computed by resolving the path of the given URI
		///     against the path of this URI.  This is done by concatenating all but
		///     the last segment of this URI's path, if any, with the given URI's
		///     path and then normalizing the result as if by invoking the {@link
		///     #normalize() normalize} method. </para></li>
		/// 
		///   </ol></li>
		/// 
		/// </ol>
		/// 
		/// <para> The result of this method is absolute if, and only if, either this
		/// URI is absolute or the given URI is absolute.  </para>
		/// </summary>
		/// <param name="uri">  The URI to be resolved against this URI </param>
		/// <returns> The resulting URI
		/// </returns>
		/// <exception cref="NullPointerException">
		///          If {@code uri} is {@code null} </exception>
		public URI Resolve(URI uri)
		{
			return Resolve(this, uri);
		}

		/// <summary>
		/// Constructs a new URI by parsing the given string and then resolving it
		/// against this URI.
		/// 
		/// <para> This convenience method works as if invoking it were equivalent to
		/// evaluating the expression {@link #resolve(java.net.URI)
		/// resolve}{@code (URI.}<seealso cref="#create(String) create"/>{@code (str))}. </para>
		/// </summary>
		/// <param name="str">   The string to be parsed into a URI </param>
		/// <returns> The resulting URI
		/// </returns>
		/// <exception cref="NullPointerException">
		///          If {@code str} is {@code null}
		/// </exception>
		/// <exception cref="IllegalArgumentException">
		///          If the given string violates RFC&nbsp;2396 </exception>
		public URI Resolve(String str)
		{
			return Resolve(URI.Create(str));
		}

		/// <summary>
		/// Relativizes the given URI against this URI.
		/// 
		/// <para> The relativization of the given URI against this URI is computed as
		/// follows: </para>
		/// 
		/// <ol>
		/// 
		///   <li><para> If either this URI or the given URI are opaque, or if the
		///   scheme and authority components of the two URIs are not identical, or
		///   if the path of this URI is not a prefix of the path of the given URI,
		///   then the given URI is returned. </para></li>
		/// 
		///   <li><para> Otherwise a new relative hierarchical URI is constructed with
		///   query and fragment components taken from the given URI and with a path
		///   component computed by removing this URI's path from the beginning of
		///   the given URI's path. </para></li>
		/// 
		/// </ol>
		/// </summary>
		/// <param name="uri">  The URI to be relativized against this URI </param>
		/// <returns> The resulting URI
		/// </returns>
		/// <exception cref="NullPointerException">
		///          If {@code uri} is {@code null} </exception>
		public URI Relativize(URI uri)
		{
			return Relativize(this, uri);
		}

		/// <summary>
		/// Constructs a URL from this URI.
		/// 
		/// <para> This convenience method works as if invoking it were equivalent to
		/// evaluating the expression {@code new URL(this.toString())} after
		/// first checking that this URI is absolute. </para>
		/// </summary>
		/// <returns>  A URL constructed from this URI
		/// </returns>
		/// <exception cref="IllegalArgumentException">
		///          If this URL is not absolute
		/// </exception>
		/// <exception cref="MalformedURLException">
		///          If a protocol handler for the URL could not be found,
		///          or if some other error occurred while constructing the URL </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public URL toURL() throws MalformedURLException
		public URL ToURL()
		{
			if (!Absolute)
			{
				throw new IllegalArgumentException("URI is not absolute");
			}
			return new URL(ToString());
		}

		// -- Component access methods --

		/// <summary>
		/// Returns the scheme component of this URI.
		/// 
		/// <para> The scheme component of a URI, if defined, only contains characters
		/// in the <i>alphanum</i> category and in the string {@code "-.+"}.  A
		/// </para>
		/// scheme always starts with an <i>alpha</i> character. <para>
		/// 
		/// The scheme component of a URI cannot contain escaped octets, hence this
		/// method does not perform any decoding.
		/// 
		/// </para>
		/// </summary>
		/// <returns>  The scheme component of this URI,
		///          or {@code null} if the scheme is undefined </returns>
		public String Scheme
		{
			get
			{
				return Scheme_Renamed;
			}
		}

		/// <summary>
		/// Tells whether or not this URI is absolute.
		/// 
		/// <para> A URI is absolute if, and only if, it has a scheme component. </para>
		/// </summary>
		/// <returns>  {@code true} if, and only if, this URI is absolute </returns>
		public bool Absolute
		{
			get
			{
				return Scheme_Renamed != null;
			}
		}

		/// <summary>
		/// Tells whether or not this URI is opaque.
		/// 
		/// <para> A URI is opaque if, and only if, it is absolute and its
		/// scheme-specific part does not begin with a slash character ('/').
		/// An opaque URI has a scheme, a scheme-specific part, and possibly
		/// a fragment; all other components are undefined. </para>
		/// </summary>
		/// <returns>  {@code true} if, and only if, this URI is opaque </returns>
		public bool Opaque
		{
			get
			{
				return Path_Renamed == null;
			}
		}

		/// <summary>
		/// Returns the raw scheme-specific part of this URI.  The scheme-specific
		/// part is never undefined, though it may be empty.
		/// 
		/// <para> The scheme-specific part of a URI only contains legal URI
		/// characters. </para>
		/// </summary>
		/// <returns>  The raw scheme-specific part of this URI
		///          (never {@code null}) </returns>
		public String RawSchemeSpecificPart
		{
			get
			{
				DefineSchemeSpecificPart();
				return SchemeSpecificPart_Renamed;
			}
		}

		/// <summary>
		/// Returns the decoded scheme-specific part of this URI.
		/// 
		/// <para> The string returned by this method is equal to that returned by the
		/// <seealso cref="#getRawSchemeSpecificPart() getRawSchemeSpecificPart"/> method
		/// except that all sequences of escaped octets are <a
		/// href="#decode">decoded</a>.  </para>
		/// </summary>
		/// <returns>  The decoded scheme-specific part of this URI
		///          (never {@code null}) </returns>
		public String SchemeSpecificPart
		{
			get
			{
				if (DecodedSchemeSpecificPart == null)
				{
					DecodedSchemeSpecificPart = Decode(RawSchemeSpecificPart);
				}
				return DecodedSchemeSpecificPart;
			}
		}

		/// <summary>
		/// Returns the raw authority component of this URI.
		/// 
		/// <para> The authority component of a URI, if defined, only contains the
		/// commercial-at character ({@code '@'}) and characters in the
		/// <i>unreserved</i>, <i>punct</i>, <i>escaped</i>, and <i>other</i>
		/// categories.  If the authority is server-based then it is further
		/// constrained to have valid user-information, host, and port
		/// components. </para>
		/// </summary>
		/// <returns>  The raw authority component of this URI,
		///          or {@code null} if the authority is undefined </returns>
		public String RawAuthority
		{
			get
			{
				return Authority_Renamed;
			}
		}

		/// <summary>
		/// Returns the decoded authority component of this URI.
		/// 
		/// <para> The string returned by this method is equal to that returned by the
		/// <seealso cref="#getRawAuthority() getRawAuthority"/> method except that all
		/// sequences of escaped octets are <a href="#decode">decoded</a>.  </para>
		/// </summary>
		/// <returns>  The decoded authority component of this URI,
		///          or {@code null} if the authority is undefined </returns>
		public String Authority
		{
			get
			{
				if (DecodedAuthority == null)
				{
					DecodedAuthority = Decode(Authority_Renamed);
				}
				return DecodedAuthority;
			}
		}

		/// <summary>
		/// Returns the raw user-information component of this URI.
		/// 
		/// <para> The user-information component of a URI, if defined, only contains
		/// characters in the <i>unreserved</i>, <i>punct</i>, <i>escaped</i>, and
		/// <i>other</i> categories. </para>
		/// </summary>
		/// <returns>  The raw user-information component of this URI,
		///          or {@code null} if the user information is undefined </returns>
		public String RawUserInfo
		{
			get
			{
				return UserInfo_Renamed;
			}
		}

		/// <summary>
		/// Returns the decoded user-information component of this URI.
		/// 
		/// <para> The string returned by this method is equal to that returned by the
		/// <seealso cref="#getRawUserInfo() getRawUserInfo"/> method except that all
		/// sequences of escaped octets are <a href="#decode">decoded</a>.  </para>
		/// </summary>
		/// <returns>  The decoded user-information component of this URI,
		///          or {@code null} if the user information is undefined </returns>
		public String UserInfo
		{
			get
			{
				if ((DecodedUserInfo == null) && (UserInfo_Renamed != null))
				{
					DecodedUserInfo = Decode(UserInfo_Renamed);
				}
				return DecodedUserInfo;
			}
		}

		/// <summary>
		/// Returns the host component of this URI.
		/// 
		/// <para> The host component of a URI, if defined, will have one of the
		/// following forms: </para>
		/// 
		/// <ul>
		/// 
		///   <li><para> A domain name consisting of one or more <i>labels</i>
		///   separated by period characters ({@code '.'}), optionally followed by
		///   a period character.  Each label consists of <i>alphanum</i> characters
		///   as well as hyphen characters ({@code '-'}), though hyphens never
		///   occur as the first or last characters in a label. The rightmost
		///   label of a domain name consisting of two or more labels, begins
		///   with an <i>alpha</i> character. </li>
		/// 
		/// </para>
		///   <li><para> A dotted-quad IPv4 address of the form
		///   <i>digit</i>{@code +.}<i>digit</i>{@code +.}<i>digit</i>{@code +.}<i>digit</i>{@code +},
		///   where no <i>digit</i> sequence is longer than three characters and no
		///   sequence has a value larger than 255. </para></li>
		/// 
		///   <li><para> An IPv6 address enclosed in square brackets ({@code '['} and
		///   {@code ']'}) and consisting of hexadecimal digits, colon characters
		///   ({@code ':'}), and possibly an embedded IPv4 address.  The full
		///   syntax of IPv6 addresses is specified in <a
		///   href="http://www.ietf.org/rfc/rfc2373.txt"><i>RFC&nbsp;2373: IPv6
		///   Addressing Architecture</i></a>.  </para></li>
		/// 
		/// </ul>
		/// 
		/// The host component of a URI cannot contain escaped octets, hence this
		/// method does not perform any decoding.
		/// </summary>
		/// <returns>  The host component of this URI,
		///          or {@code null} if the host is undefined </returns>
		public String Host
		{
			get
			{
				return Host_Renamed;
			}
		}

		/// <summary>
		/// Returns the port number of this URI.
		/// 
		/// <para> The port component of a URI, if defined, is a non-negative
		/// integer. </para>
		/// </summary>
		/// <returns>  The port component of this URI,
		///          or {@code -1} if the port is undefined </returns>
		public int Port
		{
			get
			{
				return Port_Renamed;
			}
		}

		/// <summary>
		/// Returns the raw path component of this URI.
		/// 
		/// <para> The path component of a URI, if defined, only contains the slash
		/// character ({@code '/'}), the commercial-at character ({@code '@'}),
		/// and characters in the <i>unreserved</i>, <i>punct</i>, <i>escaped</i>,
		/// and <i>other</i> categories. </para>
		/// </summary>
		/// <returns>  The path component of this URI,
		///          or {@code null} if the path is undefined </returns>
		public String RawPath
		{
			get
			{
				return Path_Renamed;
			}
		}

		/// <summary>
		/// Returns the decoded path component of this URI.
		/// 
		/// <para> The string returned by this method is equal to that returned by the
		/// <seealso cref="#getRawPath() getRawPath"/> method except that all sequences of
		/// escaped octets are <a href="#decode">decoded</a>.  </para>
		/// </summary>
		/// <returns>  The decoded path component of this URI,
		///          or {@code null} if the path is undefined </returns>
		public String Path
		{
			get
			{
				if ((DecodedPath == null) && (Path_Renamed != null))
				{
					DecodedPath = Decode(Path_Renamed);
				}
				return DecodedPath;
			}
		}

		/// <summary>
		/// Returns the raw query component of this URI.
		/// 
		/// <para> The query component of a URI, if defined, only contains legal URI
		/// characters. </para>
		/// </summary>
		/// <returns>  The raw query component of this URI,
		///          or {@code null} if the query is undefined </returns>
		public String RawQuery
		{
			get
			{
				return Query_Renamed;
			}
		}

		/// <summary>
		/// Returns the decoded query component of this URI.
		/// 
		/// <para> The string returned by this method is equal to that returned by the
		/// <seealso cref="#getRawQuery() getRawQuery"/> method except that all sequences of
		/// escaped octets are <a href="#decode">decoded</a>.  </para>
		/// </summary>
		/// <returns>  The decoded query component of this URI,
		///          or {@code null} if the query is undefined </returns>
		public String Query
		{
			get
			{
				if ((DecodedQuery == null) && (Query_Renamed != null))
				{
					DecodedQuery = Decode(Query_Renamed);
				}
				return DecodedQuery;
			}
		}

		/// <summary>
		/// Returns the raw fragment component of this URI.
		/// 
		/// <para> The fragment component of a URI, if defined, only contains legal URI
		/// characters. </para>
		/// </summary>
		/// <returns>  The raw fragment component of this URI,
		///          or {@code null} if the fragment is undefined </returns>
		public String RawFragment
		{
			get
			{
				return Fragment_Renamed;
			}
		}

		/// <summary>
		/// Returns the decoded fragment component of this URI.
		/// 
		/// <para> The string returned by this method is equal to that returned by the
		/// <seealso cref="#getRawFragment() getRawFragment"/> method except that all
		/// sequences of escaped octets are <a href="#decode">decoded</a>.  </para>
		/// </summary>
		/// <returns>  The decoded fragment component of this URI,
		///          or {@code null} if the fragment is undefined </returns>
		public String Fragment
		{
			get
			{
				if ((DecodedFragment == null) && (Fragment_Renamed != null))
				{
					DecodedFragment = Decode(Fragment_Renamed);
				}
				return DecodedFragment;
			}
		}


		// -- Equality, comparison, hash code, toString, and serialization --

		/// <summary>
		/// Tests this URI for equality with another object.
		/// 
		/// <para> If the given object is not a URI then this method immediately
		/// returns {@code false}.
		/// 
		/// </para>
		/// <para> For two URIs to be considered equal requires that either both are
		/// opaque or both are hierarchical.  Their schemes must either both be
		/// undefined or else be equal without regard to case. Their fragments
		/// must either both be undefined or else be equal.
		/// 
		/// </para>
		/// <para> For two opaque URIs to be considered equal, their scheme-specific
		/// parts must be equal.
		/// 
		/// </para>
		/// <para> For two hierarchical URIs to be considered equal, their paths must
		/// be equal and their queries must either both be undefined or else be
		/// equal.  Their authorities must either both be undefined, or both be
		/// registry-based, or both be server-based.  If their authorities are
		/// defined and are registry-based, then they must be equal.  If their
		/// authorities are defined and are server-based, then their hosts must be
		/// equal without regard to case, their port numbers must be equal, and
		/// their user-information components must be equal.
		/// 
		/// </para>
		/// <para> When testing the user-information, path, query, fragment, authority,
		/// or scheme-specific parts of two URIs for equality, the raw forms rather
		/// than the encoded forms of these components are compared and the
		/// hexadecimal digits of escaped octets are compared without regard to
		/// case.
		/// 
		/// </para>
		/// <para> This method satisfies the general contract of the {@link
		/// java.lang.Object#equals(Object) Object.equals} method. </para>
		/// </summary>
		/// <param name="ob">   The object to which this object is to be compared
		/// </param>
		/// <returns>  {@code true} if, and only if, the given object is a URI that
		///          is identical to this URI </returns>
		public override bool Equals(Object ob)
		{
			if (ob == this)
			{
				return true;
			}
			if (!(ob is URI))
			{
				return false;
			}
			URI that = (URI)ob;
			if (this.Opaque != that.Opaque)
			{
				return false;
			}
			if (!EqualIgnoringCase(this.Scheme_Renamed, that.Scheme_Renamed))
			{
				return false;
			}
			if (!Equal(this.Fragment_Renamed, that.Fragment_Renamed))
			{
				return false;
			}

			// Opaque
			if (this.Opaque)
			{
				return Equal(this.SchemeSpecificPart_Renamed, that.SchemeSpecificPart_Renamed);
			}

			// Hierarchical
			if (!Equal(this.Path_Renamed, that.Path_Renamed))
			{
				return false;
			}
			if (!Equal(this.Query_Renamed, that.Query_Renamed))
			{
				return false;
			}

			// Authorities
			if (this.Authority_Renamed == that.Authority_Renamed)
			{
				return true;
			}
			if (this.Host_Renamed != null)
			{
				// Server-based
				if (!Equal(this.UserInfo_Renamed, that.UserInfo_Renamed))
				{
					return false;
				}
				if (!EqualIgnoringCase(this.Host_Renamed, that.Host_Renamed))
				{
					return false;
				}
				if (this.Port_Renamed != that.Port_Renamed)
				{
					return false;
				}
			}
			else if (this.Authority_Renamed != null)
			{
				// Registry-based
				if (!Equal(this.Authority_Renamed, that.Authority_Renamed))
				{
					return false;
				}
			}
			else if (this.Authority_Renamed != that.Authority_Renamed)
			{
				return false;
			}

			return true;
		}

		/// <summary>
		/// Returns a hash-code value for this URI.  The hash code is based upon all
		/// of the URI's components, and satisfies the general contract of the
		/// <seealso cref="java.lang.Object#hashCode() Object.hashCode"/> method.
		/// </summary>
		/// <returns>  A hash-code value for this URI </returns>
		public override int HashCode()
		{
			if (Hash_Renamed != 0)
			{
				return Hash_Renamed;
			}
			int h = HashIgnoringCase(0, Scheme_Renamed);
			h = Hash(h, Fragment_Renamed);
			if (Opaque)
			{
				h = Hash(h, SchemeSpecificPart_Renamed);
			}
			else
			{
				h = Hash(h, Path_Renamed);
				h = Hash(h, Query_Renamed);
				if (Host_Renamed != null)
				{
					h = Hash(h, UserInfo_Renamed);
					h = HashIgnoringCase(h, Host_Renamed);
					h += 1949 * Port_Renamed;
				}
				else
				{
					h = Hash(h, Authority_Renamed);
				}
			}
			Hash_Renamed = h;
			return h;
		}

		/// <summary>
		/// Compares this URI to another object, which must be a URI.
		/// 
		/// <para> When comparing corresponding components of two URIs, if one
		/// component is undefined but the other is defined then the first is
		/// considered to be less than the second.  Unless otherwise noted, string
		/// components are ordered according to their natural, case-sensitive
		/// ordering as defined by the {@link java.lang.String#compareTo(Object)
		/// String.compareTo} method.  String components that are subject to
		/// encoding are compared by comparing their raw forms rather than their
		/// encoded forms.
		/// 
		/// </para>
		/// <para> The ordering of URIs is defined as follows: </para>
		/// 
		/// <ul>
		/// 
		///   <li><para> Two URIs with different schemes are ordered according the
		///   ordering of their schemes, without regard to case. </para></li>
		/// 
		///   <li><para> A hierarchical URI is considered to be less than an opaque URI
		///   with an identical scheme. </para></li>
		/// 
		///   <li><para> Two opaque URIs with identical schemes are ordered according
		///   to the ordering of their scheme-specific parts. </para></li>
		/// 
		///   <li><para> Two opaque URIs with identical schemes and scheme-specific
		///   parts are ordered according to the ordering of their
		///   fragments. </para></li>
		/// 
		///   <li><para> Two hierarchical URIs with identical schemes are ordered
		///   according to the ordering of their authority components: </para>
		/// 
		///   <ul>
		/// 
		///     <li><para> If both authority components are server-based then the URIs
		///     are ordered according to their user-information components; if these
		///     components are identical then the URIs are ordered according to the
		///     ordering of their hosts, without regard to case; if the hosts are
		///     identical then the URIs are ordered according to the ordering of
		///     their ports. </para></li>
		/// 
		///     <li><para> If one or both authority components are registry-based then
		///     the URIs are ordered according to the ordering of their authority
		///     components. </para></li>
		/// 
		///   </ul></li>
		/// 
		///   <li><para> Finally, two hierarchical URIs with identical schemes and
		///   authority components are ordered according to the ordering of their
		///   paths; if their paths are identical then they are ordered according to
		///   the ordering of their queries; if the queries are identical then they
		///   are ordered according to the order of their fragments. </para></li>
		/// 
		/// </ul>
		/// 
		/// <para> This method satisfies the general contract of the {@link
		/// java.lang.Comparable#compareTo(Object) Comparable.compareTo}
		/// method. </para>
		/// </summary>
		/// <param name="that">
		///          The object to which this URI is to be compared
		/// </param>
		/// <returns>  A negative integer, zero, or a positive integer as this URI is
		///          less than, equal to, or greater than the given URI
		/// </returns>
		/// <exception cref="ClassCastException">
		///          If the given object is not a URI </exception>
		public int CompareTo(URI that)
		{
			int c;

			if ((c = CompareIgnoringCase(this.Scheme_Renamed, that.Scheme_Renamed)) != 0)
			{
				return c;
			}

			if (this.Opaque)
			{
				if (that.Opaque)
				{
					// Both opaque
					if ((c = Compare(this.SchemeSpecificPart_Renamed, that.SchemeSpecificPart_Renamed)) != 0)
					{
						return c;
					}
					return Compare(this.Fragment_Renamed, that.Fragment_Renamed);
				}
				return +1; // Opaque > hierarchical
			}
			else if (that.Opaque)
			{
				return -1; // Hierarchical < opaque
			}

			// Hierarchical
			if ((this.Host_Renamed != null) && (that.Host_Renamed != null))
			{
				// Both server-based
				if ((c = Compare(this.UserInfo_Renamed, that.UserInfo_Renamed)) != 0)
				{
					return c;
				}
				if ((c = CompareIgnoringCase(this.Host_Renamed, that.Host_Renamed)) != 0)
				{
					return c;
				}
				if ((c = this.Port_Renamed - that.Port_Renamed) != 0)
				{
					return c;
				}
			}
			else
			{
				// If one or both authorities are registry-based then we simply
				// compare them in the usual, case-sensitive way.  If one is
				// registry-based and one is server-based then the strings are
				// guaranteed to be unequal, hence the comparison will never return
				// zero and the compareTo and equals methods will remain
				// consistent.
				if ((c = Compare(this.Authority_Renamed, that.Authority_Renamed)) != 0)
				{
					return c;
				}
			}

			if ((c = Compare(this.Path_Renamed, that.Path_Renamed)) != 0)
			{
				return c;
			}
			if ((c = Compare(this.Query_Renamed, that.Query_Renamed)) != 0)
			{
				return c;
			}
			return Compare(this.Fragment_Renamed, that.Fragment_Renamed);
		}

		/// <summary>
		/// Returns the content of this URI as a string.
		/// 
		/// <para> If this URI was created by invoking one of the constructors in this
		/// class then a string equivalent to the original input string, or to the
		/// string computed from the originally-given components, as appropriate, is
		/// returned.  Otherwise this URI was created by normalization, resolution,
		/// or relativization, and so a string is constructed from this URI's
		/// components according to the rules specified in <a
		/// href="http://www.ietf.org/rfc/rfc2396.txt">RFC&nbsp;2396</a>,
		/// section&nbsp;5.2, step&nbsp;7. </para>
		/// </summary>
		/// <returns>  The string form of this URI </returns>
		public override String ToString()
		{
			DefineString();
			return @string;
		}

		/// <summary>
		/// Returns the content of this URI as a US-ASCII string.
		/// 
		/// <para> If this URI does not contain any characters in the <i>other</i>
		/// category then an invocation of this method will return the same value as
		/// an invocation of the <seealso cref="#toString() toString"/> method.  Otherwise
		/// this method works as if by invoking that method and then <a
		/// href="#encode">encoding</a> the result.  </para>
		/// </summary>
		/// <returns>  The string form of this URI, encoded as needed
		///          so that it only contains characters in the US-ASCII
		///          charset </returns>
		public String ToASCIIString()
		{
			DefineString();
			return Encode(@string);
		}


		// -- Serialization support --

		/// <summary>
		/// Saves the content of this URI to the given serial stream.
		/// 
		/// <para> The only serializable field of a URI instance is its {@code string}
		/// field.  That field is given a value, if it does not have one already,
		/// and then the <seealso cref="java.io.ObjectOutputStream#defaultWriteObject()"/>
		/// method of the given object-output stream is invoked. </para>
		/// </summary>
		/// <param name="os">  The object-output stream to which this object
		///             is to be written </param>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: private void writeObject(java.io.ObjectOutputStream os) throws java.io.IOException
		private void WriteObject(ObjectOutputStream os)
		{
			DefineString();
			os.DefaultWriteObject(); // Writes the string field only
		}

		/// <summary>
		/// Reconstitutes a URI from the given serial stream.
		/// 
		/// <para> The <seealso cref="java.io.ObjectInputStream#defaultReadObject()"/> method is
		/// invoked to read the value of the {@code string} field.  The result is
		/// then parsed in the usual way.
		/// 
		/// </para>
		/// </summary>
		/// <param name="is">  The object-input stream from which this object
		///             is being read </param>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: private void readObject(java.io.ObjectInputStream is) throws ClassNotFoundException, java.io.IOException
		private void ReadObject(ObjectInputStream @is)
		{
			Port_Renamed = -1; // Argh
			@is.DefaultReadObject();
			try
			{
				(new Parser(this, @string)).Parse(false);
			}
			catch (URISyntaxException x)
			{
				IOException y = new InvalidObjectException("Invalid URI");
				y.InitCause(x);
				throw y;
			}
		}


		// -- End of public methods --


		// -- Utility methods for string-field comparison and hashing --

		// These methods return appropriate values for null string arguments,
		// thereby simplifying the equals, hashCode, and compareTo methods.
		//
		// The case-ignoring methods should only be applied to strings whose
		// characters are all known to be US-ASCII.  Because of this restriction,
		// these methods are faster than the similar methods in the String class.

		// US-ASCII only
		private static int ToLower(char c)
		{
			if ((c >= 'A') && (c <= 'Z'))
			{
				return c + ('a' - 'A');
			}
			return c;
		}

		// US-ASCII only
		private static int ToUpper(char c)
		{
			if ((c >= 'a') && (c <= 'z'))
			{
				return c - ('a' - 'A');
			}
			return c;
		}

		private static bool Equal(String s, String t)
		{
			if (s == t)
			{
				return true;
			}
			if ((s != null) && (t != null))
			{
				if (s.Length() != t.Length())
				{
					return false;
				}
				if (s.IndexOf('%') < 0)
				{
					return s.Equals(t);
				}
				int n = s.Length();
				for (int i = 0; i < n;)
				{
					char c = s.CharAt(i);
					char d = t.CharAt(i);
					if (c != '%')
					{
						if (c != d)
						{
							return false;
						}
						i++;
						continue;
					}
					if (d != '%')
					{
						return false;
					}
					i++;
					if (ToLower(s.CharAt(i)) != ToLower(t.CharAt(i)))
					{
						return false;
					}
					i++;
					if (ToLower(s.CharAt(i)) != ToLower(t.CharAt(i)))
					{
						return false;
					}
					i++;
				}
				return true;
			}
			return false;
		}

		// US-ASCII only
		private static bool EqualIgnoringCase(String s, String t)
		{
			if (s == t)
			{
				return true;
			}
			if ((s != null) && (t != null))
			{
				int n = s.Length();
				if (t.Length() != n)
				{
					return false;
				}
				for (int i = 0; i < n; i++)
				{
					if (ToLower(s.CharAt(i)) != ToLower(t.CharAt(i)))
					{
						return false;
					}
				}
				return true;
			}
			return false;
		}

		private static int Hash(int hash, String s)
		{
			if (s == null)
			{
				return hash;
			}
			return s.IndexOf('%') < 0 ? hash * 127 + s.HashCode() : NormalizedHash(hash, s);
		}


		private static int NormalizedHash(int hash, String s)
		{
			int h = 0;
			for (int index = 0; index < s.Length(); index++)
			{
				char ch = s.CharAt(index);
				h = 31 * h + ch;
				if (ch == '%')
				{
					/*
					 * Process the next two encoded characters
					 */
					for (int i = index + 1; i < index + 3; i++)
					{
						h = 31 * h + ToUpper(s.CharAt(i));
					}
					index += 2;
				}
			}
			return hash * 127 + h;
		}

		// US-ASCII only
		private static int HashIgnoringCase(int hash, String s)
		{
			if (s == null)
			{
				return hash;
			}
			int h = hash;
			int n = s.Length();
			for (int i = 0; i < n; i++)
			{
				h = 31 * h + ToLower(s.CharAt(i));
			}
			return h;
		}

		private static int Compare(String s, String t)
		{
			if (s == t)
			{
				return 0;
			}
			if (s != null)
			{
				if (t != null)
				{
					return s.CompareTo(t);
				}
				else
				{
					return +1;
				}
			}
			else
			{
				return -1;
			}
		}

		// US-ASCII only
		private static int CompareIgnoringCase(String s, String t)
		{
			if (s == t)
			{
				return 0;
			}
			if (s != null)
			{
				if (t != null)
				{
					int sn = s.Length();
					int tn = t.Length();
					int n = sn < tn ? sn : tn;
					for (int i = 0; i < n; i++)
					{
						int c = ToLower(s.CharAt(i)) - ToLower(t.CharAt(i));
						if (c != 0)
						{
							return c;
						}
					}
					return sn - tn;
				}
				return +1;
			}
			else
			{
				return -1;
			}
		}


		// -- String construction --

		// If a scheme is given then the path, if given, must be absolute
		//
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: private static void checkPath(String s, String scheme, String path) throws URISyntaxException
		private static void CheckPath(String s, String scheme, String path)
		{
			if (scheme != null)
			{
				if ((path != null) && ((path.Length() > 0) && (path.CharAt(0) != '/')))
				{
					throw new URISyntaxException(s, "Relative path in absolute URI");
				}
			}
		}

		private void AppendAuthority(StringBuffer sb, String authority, String userInfo, String host, int port)
		{
			if (host != null)
			{
				sb.Append("//");
				if (userInfo != null)
				{
					sb.Append(Quote(userInfo, L_USERINFO, H_USERINFO));
					sb.Append('@');
				}
				bool needBrackets = ((host.IndexOf(':') >= 0) && !host.StartsWith("[") && !host.EndsWith("]"));
				if (needBrackets)
				{
					sb.Append('[');
				}
				sb.Append(host);
				if (needBrackets)
				{
					sb.Append(']');
				}
				if (port != -1)
				{
					sb.Append(':');
					sb.Append(port);
				}
			}
			else if (authority != null)
			{
				sb.Append("//");
				if (authority.StartsWith("["))
				{
					// authority should (but may not) contain an embedded IPv6 address
					int end = authority.IndexOf("]");
					String doquote = authority, dontquote = "";
					if (end != -1 && authority.IndexOf(":") != -1)
					{
						// the authority contains an IPv6 address
						if (end == authority.Length())
						{
							dontquote = authority;
							doquote = "";
						}
						else
						{
							dontquote = authority.Substring(0, end + 1);
							doquote = authority.Substring(end + 1);
						}
					}
					sb.Append(dontquote);
					sb.Append(Quote(doquote, L_REG_NAME | L_SERVER, H_REG_NAME | H_SERVER));
				}
				else
				{
					sb.Append(Quote(authority, L_REG_NAME | L_SERVER, H_REG_NAME | H_SERVER));
				}
			}
		}

		private void AppendSchemeSpecificPart(StringBuffer sb, String opaquePart, String authority, String userInfo, String host, int port, String path, String query)
		{
			if (opaquePart != null)
			{
				/* check if SSP begins with an IPv6 address
				 * because we must not quote a literal IPv6 address
				 */
				if (opaquePart.StartsWith("//["))
				{
					int end = opaquePart.IndexOf("]");
					if (end != -1 && opaquePart.IndexOf(":") != -1)
					{
						String doquote, dontquote;
						if (end == opaquePart.Length())
						{
							dontquote = opaquePart;
							doquote = "";
						}
						else
						{
							dontquote = opaquePart.Substring(0,end + 1);
							doquote = opaquePart.Substring(end + 1);
						}
						sb.Append(dontquote);
						sb.Append(Quote(doquote, L_URIC, H_URIC));
					}
				}
				else
				{
					sb.Append(Quote(opaquePart, L_URIC, H_URIC));
				}
			}
			else
			{
				AppendAuthority(sb, authority, userInfo, host, port);
				if (path != null)
				{
					sb.Append(Quote(path, L_PATH, H_PATH));
				}
				if (query != null)
				{
					sb.Append('?');
					sb.Append(Quote(query, L_URIC, H_URIC));
				}
			}
		}

		private void AppendFragment(StringBuffer sb, String fragment)
		{
			if (fragment != null)
			{
				sb.Append('#');
				sb.Append(Quote(fragment, L_URIC, H_URIC));
			}
		}

		private String ToString(String scheme, String opaquePart, String authority, String userInfo, String host, int port, String path, String query, String fragment)
		{
			StringBuffer sb = new StringBuffer();
			if (scheme != null)
			{
				sb.Append(scheme);
				sb.Append(':');
			}
			AppendSchemeSpecificPart(sb, opaquePart, authority, userInfo, host, port, path, query);
			AppendFragment(sb, fragment);
			return sb.ToString();
		}

		private void DefineSchemeSpecificPart()
		{
			if (SchemeSpecificPart_Renamed != null)
			{
				return;
			}
			StringBuffer sb = new StringBuffer();
			AppendSchemeSpecificPart(sb, null, Authority, UserInfo, Host_Renamed, Port_Renamed, Path, Query);
			if (sb.Length() == 0)
			{
				return;
			}
			SchemeSpecificPart_Renamed = sb.ToString();
		}

		private void DefineString()
		{
			if (@string != null)
			{
				return;
			}

			StringBuffer sb = new StringBuffer();
			if (Scheme_Renamed != null)
			{
				sb.Append(Scheme_Renamed);
				sb.Append(':');
			}
			if (Opaque)
			{
				sb.Append(SchemeSpecificPart_Renamed);
			}
			else
			{
				if (Host_Renamed != null)
				{
					sb.Append("//");
					if (UserInfo_Renamed != null)
					{
						sb.Append(UserInfo_Renamed);
						sb.Append('@');
					}
					bool needBrackets = ((Host_Renamed.IndexOf(':') >= 0) && !Host_Renamed.StartsWith("[") && !Host_Renamed.EndsWith("]"));
					if (needBrackets)
					{
						sb.Append('[');
					}
					sb.Append(Host_Renamed);
					if (needBrackets)
					{
						sb.Append(']');
					}
					if (Port_Renamed != -1)
					{
						sb.Append(':');
						sb.Append(Port_Renamed);
					}
				}
				else if (Authority_Renamed != null)
				{
					sb.Append("//");
					sb.Append(Authority_Renamed);
				}
				if (Path_Renamed != null)
				{
					sb.Append(Path_Renamed);
				}
				if (Query_Renamed != null)
				{
					sb.Append('?');
					sb.Append(Query_Renamed);
				}
			}
			if (Fragment_Renamed != null)
			{
				sb.Append('#');
				sb.Append(Fragment_Renamed);
			}
			@string = sb.ToString();
		}


		// -- Normalization, resolution, and relativization --

		// RFC2396 5.2 (6)
		private static String ResolvePath(String @base, String child, bool absolute)
		{
			int i = @base.LastIndexOf('/');
			int cn = child.Length();
			String path = "";

			if (cn == 0)
			{
				// 5.2 (6a)
				if (i >= 0)
				{
					path = @base.Substring(0, i + 1);
				}
			}
			else
			{
				StringBuffer sb = new StringBuffer(@base.Length() + cn);
				// 5.2 (6a)
				if (i >= 0)
				{
					sb.Append(@base.Substring(0, i + 1));
				}
				// 5.2 (6b)
				sb.Append(child);
				path = sb.ToString();
			}

			// 5.2 (6c-f)
			String np = Normalize(path);

			// 5.2 (6g): If the result is absolute but the path begins with "../",
			// then we simply leave the path as-is

			return np;
		}

		// RFC2396 5.2
		private static URI Resolve(URI @base, URI child)
		{
			// check if child if opaque first so that NPE is thrown
			// if child is null.
			if (child.Opaque || @base.Opaque)
			{
				return child;
			}

			// 5.2 (2): Reference to current document (lone fragment)
			if ((child.Scheme_Renamed == null) && (child.Authority_Renamed == null) && child.Path_Renamed.Equals("") && (child.Fragment_Renamed != null) && (child.Query_Renamed == null))
			{
				if ((@base.Fragment_Renamed != null) && child.Fragment_Renamed.Equals(@base.Fragment_Renamed))
				{
					return @base;
				}
				URI ru = new URI();
				ru.Scheme_Renamed = @base.Scheme_Renamed;
				ru.Authority_Renamed = @base.Authority_Renamed;
				ru.UserInfo_Renamed = @base.UserInfo_Renamed;
				ru.Host_Renamed = @base.Host_Renamed;
				ru.Port_Renamed = @base.Port_Renamed;
				ru.Path_Renamed = @base.Path_Renamed;
				ru.Fragment_Renamed = child.Fragment_Renamed;
				ru.Query_Renamed = @base.Query_Renamed;
				return ru;
			}

			// 5.2 (3): Child is absolute
			if (child.Scheme_Renamed != null)
			{
				return child;
			}

			URI ru = new URI(); // Resolved URI
			ru.Scheme_Renamed = @base.Scheme_Renamed;
			ru.Query_Renamed = child.Query_Renamed;
			ru.Fragment_Renamed = child.Fragment_Renamed;

			// 5.2 (4): Authority
			if (child.Authority_Renamed == null)
			{
				ru.Authority_Renamed = @base.Authority_Renamed;
				ru.Host_Renamed = @base.Host_Renamed;
				ru.UserInfo_Renamed = @base.UserInfo_Renamed;
				ru.Port_Renamed = @base.Port_Renamed;

				String cp = (child.Path_Renamed == null) ? "" : child.Path_Renamed;
				if ((cp.Length() > 0) && (cp.CharAt(0) == '/'))
				{
					// 5.2 (5): Child path is absolute
					ru.Path_Renamed = child.Path_Renamed;
				}
				else
				{
					// 5.2 (6): Resolve relative path
					ru.Path_Renamed = ResolvePath(@base.Path_Renamed, cp, @base.Absolute);
				}
			}
			else
			{
				ru.Authority_Renamed = child.Authority_Renamed;
				ru.Host_Renamed = child.Host_Renamed;
				ru.UserInfo_Renamed = child.UserInfo_Renamed;
				ru.Host_Renamed = child.Host_Renamed;
				ru.Port_Renamed = child.Port_Renamed;
				ru.Path_Renamed = child.Path_Renamed;
			}

			// 5.2 (7): Recombine (nothing to do here)
			return ru;
		}

		// If the given URI's path is normal then return the URI;
		// o.w., return a new URI containing the normalized path.
		//
		private static URI Normalize(URI u)
		{
			if (u.Opaque || (u.Path_Renamed == null) || (u.Path_Renamed.Length() == 0))
			{
				return u;
			}

			String np = Normalize(u.Path_Renamed);
			if (np == u.Path_Renamed)
			{
				return u;
			}

			URI v = new URI();
			v.Scheme_Renamed = u.Scheme_Renamed;
			v.Fragment_Renamed = u.Fragment_Renamed;
			v.Authority_Renamed = u.Authority_Renamed;
			v.UserInfo_Renamed = u.UserInfo_Renamed;
			v.Host_Renamed = u.Host_Renamed;
			v.Port_Renamed = u.Port_Renamed;
			v.Path_Renamed = np;
			v.Query_Renamed = u.Query_Renamed;
			return v;
		}

		// If both URIs are hierarchical, their scheme and authority components are
		// identical, and the base path is a prefix of the child's path, then
		// return a relative URI that, when resolved against the base, yields the
		// child; otherwise, return the child.
		//
		private static URI Relativize(URI @base, URI child)
		{
			// check if child if opaque first so that NPE is thrown
			// if child is null.
			if (child.Opaque || @base.Opaque)
			{
				return child;
			}
			if (!EqualIgnoringCase(@base.Scheme_Renamed, child.Scheme_Renamed) || !Equal(@base.Authority_Renamed, child.Authority_Renamed))
			{
				return child;
			}

			String bp = Normalize(@base.Path_Renamed);
			String cp = Normalize(child.Path_Renamed);
			if (!bp.Equals(cp))
			{
				if (!bp.EndsWith("/"))
				{
					bp = bp + "/";
				}
				if (!cp.StartsWith(bp))
				{
					return child;
				}
			}

			URI v = new URI();
			v.Path_Renamed = cp.Substring(bp.Length());
			v.Query_Renamed = child.Query_Renamed;
			v.Fragment_Renamed = child.Fragment_Renamed;
			return v;
		}



		// -- Path normalization --

		// The following algorithm for path normalization avoids the creation of a
		// string object for each segment, as well as the use of a string buffer to
		// compute the final result, by using a single char array and editing it in
		// place.  The array is first split into segments, replacing each slash
		// with '\0' and creating a segment-index array, each element of which is
		// the index of the first char in the corresponding segment.  We then walk
		// through both arrays, removing ".", "..", and other segments as necessary
		// by setting their entries in the index array to -1.  Finally, the two
		// arrays are used to rejoin the segments and compute the final result.
		//
		// This code is based upon src/solaris/native/java/io/canonicalize_md.c


		// Check the given path to see if it might need normalization.  A path
		// might need normalization if it contains duplicate slashes, a "."
		// segment, or a ".." segment.  Return -1 if no further normalization is
		// possible, otherwise return the number of segments found.
		//
		// This method takes a string argument rather than a char array so that
		// this test can be performed without invoking path.toCharArray().
		//
		private static int NeedsNormalization(String path)
		{
			bool normal = true;
			int ns = 0; // Number of segments
			int end = path.Length() - 1; // Index of last char in path
			int p = 0; // Index of next char in path

			// Skip initial slashes
			while (p <= end)
			{
				if (path.CharAt(p) != '/')
				{
					break;
				}
				p++;
			}
			if (p > 1)
			{
				normal = false;
			}

			// Scan segments
			while (p <= end)
			{

				// Looking at "." or ".." ?
				if ((path.CharAt(p) == '.') && ((p == end) || ((path.CharAt(p + 1) == '/') || ((path.CharAt(p + 1) == '.') && ((p + 1 == end) || (path.CharAt(p + 2) == '/'))))))
				{
					normal = false;
				}
				ns++;

				// Find beginning of next segment
				while (p <= end)
				{
					if (path.CharAt(p++) != '/')
					{
						continue;
					}

					// Skip redundant slashes
					while (p <= end)
					{
						if (path.CharAt(p) != '/')
						{
							break;
						}
						normal = false;
						p++;
					}

					break;
				}
			}

			return normal ? - 1 : ns;
		}


		// Split the given path into segments, replacing slashes with nulls and
		// filling in the given segment-index array.
		//
		// Preconditions:
		//   segs.length == Number of segments in path
		//
		// Postconditions:
		//   All slashes in path replaced by '\0'
		//   segs[i] == Index of first char in segment i (0 <= i < segs.length)
		//
		private static void Split(char[] path, int[] segs)
		{
			int end = path.Length - 1; // Index of last char in path
			int p = 0; // Index of next char in path
			int i = 0; // Index of current segment

			// Skip initial slashes
			while (p <= end)
			{
				if (path[p] != '/')
				{
					break;
				}
				path[p] = '\0';
				p++;
			}

			while (p <= end)
			{

				// Note start of segment
				segs[i++] = p++;

				// Find beginning of next segment
				while (p <= end)
				{
					if (path[p++] != '/')
					{
						continue;
					}
					path[p - 1] = '\0';

					// Skip redundant slashes
					while (p <= end)
					{
						if (path[p] != '/')
						{
							break;
						}
						path[p++] = '\0';
					}
					break;
				}
			}

			if (i != segs.Length)
			{
				throw new InternalError(); // ASSERT
			}
		}


		// Join the segments in the given path according to the given segment-index
		// array, ignoring those segments whose index entries have been set to -1,
		// and inserting slashes as needed.  Return the length of the resulting
		// path.
		//
		// Preconditions:
		//   segs[i] == -1 implies segment i is to be ignored
		//   path computed by split, as above, with '\0' having replaced '/'
		//
		// Postconditions:
		//   path[0] .. path[return value] == Resulting path
		//
		private static int Join(char[] path, int[] segs)
		{
			int ns = segs.Length; // Number of segments
			int end = path.Length - 1; // Index of last char in path
			int p = 0; // Index of next path char to write

			if (path[p] == '\0')
			{
				// Restore initial slash for absolute paths
				path[p++] = '/';
			}

			for (int i = 0; i < ns; i++)
			{
				int q = segs[i]; // Current segment
				if (q == -1)
				{
					// Ignore this segment
					continue;
				}

				if (p == q)
				{
					// We're already at this segment, so just skip to its end
					while ((p <= end) && (path[p] != '\0'))
					{
						p++;
					}
					if (p <= end)
					{
						// Preserve trailing slash
						path[p++] = '/';
					}
				}
				else if (p < q)
				{
					// Copy q down to p
					while ((q <= end) && (path[q] != '\0'))
					{
						path[p++] = path[q++];
					}
					if (q <= end)
					{
						// Preserve trailing slash
						path[p++] = '/';
					}
				}
				else
				{
					throw new InternalError(); // ASSERT false
				}
			}

			return p;
		}


		// Remove "." segments from the given path, and remove segment pairs
		// consisting of a non-".." segment followed by a ".." segment.
		//
		private static void RemoveDots(char[] path, int[] segs)
		{
			int ns = segs.Length;
			int end = path.Length - 1;

			for (int i = 0; i < ns; i++)
			{
				int dots = 0; // Number of dots found (0, 1, or 2)

				// Find next occurrence of "." or ".."
				do
				{
					int p = segs[i];
					if (path[p] == '.')
					{
						if (p == end)
						{
							dots = 1;
							break;
						}
						else if (path[p + 1] == '\0')
						{
							dots = 1;
							break;
						}
						else if ((path[p + 1] == '.') && ((p + 1 == end) || (path[p + 2] == '\0')))
						{
							dots = 2;
							break;
						}
					}
					i++;
				} while (i < ns);
				if ((i > ns) || (dots == 0))
				{
					break;
				}

				if (dots == 1)
				{
					// Remove this occurrence of "."
					segs[i] = -1;
				}
				else
				{
					// If there is a preceding non-".." segment, remove both that
					// segment and this occurrence of ".."; otherwise, leave this
					// ".." segment as-is.
					int j;
					for (j = i - 1; j >= 0; j--)
					{
						if (segs[j] != -1)
						{
							break;
						}
					}
					if (j >= 0)
					{
						int q = segs[j];
						if (!((path[q] == '.') && (path[q + 1] == '.') && (path[q + 2] == '\0')))
						{
							segs[i] = -1;
							segs[j] = -1;
						}
					}
				}
			}
		}


		// DEVIATION: If the normalized path is relative, and if the first
		// segment could be parsed as a scheme name, then prepend a "." segment
		//
		private static void MaybeAddLeadingDot(char[] path, int[] segs)
		{

			if (path[0] == '\0')
			{
				// The path is absolute
				return;
			}

			int ns = segs.Length;
			int f = 0; // Index of first segment
			while (f < ns)
			{
				if (segs[f] >= 0)
				{
					break;
				}
				f++;
			}
			if ((f >= ns) || (f == 0))
				// The path is empty, or else the original first segment survived,
				// in which case we already know that no leading "." is needed
			{
				return;
			}

			int p = segs[f];
			while ((p < path.Length) && (path[p] != ':') && (path[p] != '\0'))
			{
				p++;
			}
			if (p >= path.Length || path[p] == '\0')
			{
				// No colon in first segment, so no "." needed
				return;
			}

			// At this point we know that the first segment is unused,
			// hence we can insert a "." segment at that position
			path[0] = '.';
			path[1] = '\0';
			segs[0] = 0;
		}


		// Normalize the given path string.  A normal path string has no empty
		// segments (i.e., occurrences of "//"), no segments equal to ".", and no
		// segments equal to ".." that are preceded by a segment not equal to "..".
		// In contrast to Unix-style pathname normalization, for URI paths we
		// always retain trailing slashes.
		//
		private static String Normalize(String ps)
		{

			// Does this path need normalization?
			int ns = NeedsNormalization(ps); // Number of segments
			if (ns < 0)
			{
				// Nope -- just return it
				return ps;
			}

			char[] path = ps.ToCharArray(); // Path in char-array form

			// Split path into segments
			int[] segs = new int[ns]; // Segment-index array
			Split(path, segs);

			// Remove dots
			RemoveDots(path, segs);

			// Prevent scheme-name confusion
			MaybeAddLeadingDot(path, segs);

			// Join the remaining segments and return the result
			String s = new String(path, 0, Join(path, segs));
			if (s.Equals(ps))
			{
				// string was already normalized
				return ps;
			}
			return s;
		}



		// -- Character classes for parsing --

		// RFC2396 precisely specifies which characters in the US-ASCII charset are
		// permissible in the various components of a URI reference.  We here
		// define a set of mask pairs to aid in enforcing these restrictions.  Each
		// mask pair consists of two longs, a low mask and a high mask.  Taken
		// together they represent a 128-bit mask, where bit i is set iff the
		// character with value i is permitted.
		//
		// This approach is more efficient than sequentially searching arrays of
		// permitted characters.  It could be made still more efficient by
		// precompiling the mask information so that a character's presence in a
		// given mask could be determined by a single table lookup.

		// Compute the low-order mask for the characters in the given string
		private static long LowMask(String chars)
		{
			int n = chars.Length();
			long m = 0;
			for (int i = 0; i < n; i++)
			{
				char c = chars.CharAt(i);
				if (c < 64)
				{
					m |= (1L << c);
				}
			}
			return m;
		}

		// Compute the high-order mask for the characters in the given string
		private static long HighMask(String chars)
		{
			int n = chars.Length();
			long m = 0;
			for (int i = 0; i < n; i++)
			{
				char c = chars.CharAt(i);
				if ((c >= 64) && (c < 128))
				{
					m |= (1L << (c - 64));
				}
			}
			return m;
		}

		// Compute a low-order mask for the characters
		// between first and last, inclusive
		private static long LowMask(char first, char last)
		{
			long m = 0;
			int f = System.Math.Max(System.Math.Min(first, 63), 0);
			int l = System.Math.Max(System.Math.Min(last, 63), 0);
			for (int i = f; i <= l; i++)
			{
				m |= 1L << i;
			}
			return m;
		}

		// Compute a high-order mask for the characters
		// between first and last, inclusive
		private static long HighMask(char first, char last)
		{
			long m = 0;
			int f = System.Math.Max(System.Math.Min(first, 127), 64) - 64;
			int l = System.Math.Max(System.Math.Min(last, 127), 64) - 64;
			for (int i = f; i <= l; i++)
			{
				m |= 1L << i;
			}
			return m;
		}

		// Tell whether the given character is permitted by the given mask pair
		private static bool Match(char c, long lowMask, long highMask)
		{
			if (c == 0) // 0 doesn't have a slot in the mask. So, it never matches.
			{
				return false;
			}
			if (c < 64)
			{
				return ((1L << c) & lowMask) != 0;
			}
			if (c < 128)
			{
				return ((1L << (c - 64)) & highMask) != 0;
			}
			return false;
		}

		// Character-class masks, in reverse order from RFC2396 because
		// initializers for static fields cannot make forward references.

		// digit    = "0" | "1" | "2" | "3" | "4" | "5" | "6" | "7" |
		//            "8" | "9"
		private static readonly long L_DIGIT = LowMask('0', '9');
		private const long H_DIGIT = 0L;

		// upalpha  = "A" | "B" | "C" | "D" | "E" | "F" | "G" | "H" | "I" |
		//            "J" | "K" | "L" | "M" | "N" | "O" | "P" | "Q" | "R" |
		//            "S" | "T" | "U" | "V" | "W" | "X" | "Y" | "Z"
		private const long L_UPALPHA = 0L;
		private static readonly long H_UPALPHA = HighMask('A', 'Z');

		// lowalpha = "a" | "b" | "c" | "d" | "e" | "f" | "g" | "h" | "i" |
		//            "j" | "k" | "l" | "m" | "n" | "o" | "p" | "q" | "r" |
		//            "s" | "t" | "u" | "v" | "w" | "x" | "y" | "z"
		private const long L_LOWALPHA = 0L;
		private static readonly long H_LOWALPHA = HighMask('a', 'z');

		// alpha         = lowalpha | upalpha
		private static readonly long L_ALPHA = L_LOWALPHA | L_UPALPHA;
		private static readonly long H_ALPHA = H_LOWALPHA | H_UPALPHA;

		// alphanum      = alpha | digit
		private static readonly long L_ALPHANUM = L_DIGIT | L_ALPHA;
		private static readonly long H_ALPHANUM = H_DIGIT | H_ALPHA;

		// hex           = digit | "A" | "B" | "C" | "D" | "E" | "F" |
		//                         "a" | "b" | "c" | "d" | "e" | "f"
		private static readonly long L_HEX = L_DIGIT;
		private static readonly long H_HEX = HighMask('A', 'F') | HighMask('a', 'f');

		// mark          = "-" | "_" | "." | "!" | "~" | "*" | "'" |
		//                 "(" | ")"
		private static readonly long L_MARK = LowMask("-_.!~*'()");
		private static readonly long H_MARK = HighMask("-_.!~*'()");

		// unreserved    = alphanum | mark
		private static readonly long L_UNRESERVED = L_ALPHANUM | L_MARK;
		private static readonly long H_UNRESERVED = H_ALPHANUM | H_MARK;

		// reserved      = ";" | "/" | "?" | ":" | "@" | "&" | "=" | "+" |
		//                 "$" | "," | "[" | "]"
		// Added per RFC2732: "[", "]"
		private static readonly long L_RESERVED = LowMask(";/?:@&=+$,[]");
		private static readonly long H_RESERVED = HighMask(";/?:@&=+$,[]");

		// The zero'th bit is used to indicate that escape pairs and non-US-ASCII
		// characters are allowed; this is handled by the scanEscape method below.
		private const long L_ESCAPED = 1L;
		private const long H_ESCAPED = 0L;

		// uric          = reserved | unreserved | escaped
		private static readonly long L_URIC = L_RESERVED | L_UNRESERVED | L_ESCAPED;
		private static readonly long H_URIC = H_RESERVED | H_UNRESERVED | H_ESCAPED;

		// pchar         = unreserved | escaped |
		//                 ":" | "@" | "&" | "=" | "+" | "$" | ","
		private static readonly long L_PCHAR = L_UNRESERVED | L_ESCAPED | LowMask(":@&=+$,");
		private static readonly long H_PCHAR = H_UNRESERVED | H_ESCAPED | HighMask(":@&=+$,");

		// All valid path characters
		private static readonly long L_PATH = L_PCHAR | LowMask(";/");
		private static readonly long H_PATH = H_PCHAR | HighMask(";/");

		// Dash, for use in domainlabel and toplabel
		private static readonly long L_DASH = LowMask("-");
		private static readonly long H_DASH = HighMask("-");

		// Dot, for use in hostnames
		private static readonly long L_DOT = LowMask(".");
		private static readonly long H_DOT = HighMask(".");

		// userinfo      = *( unreserved | escaped |
		//                    ";" | ":" | "&" | "=" | "+" | "$" | "," )
		private static readonly long L_USERINFO = L_UNRESERVED | L_ESCAPED | LowMask(";:&=+$,");
		private static readonly long H_USERINFO = H_UNRESERVED | H_ESCAPED | HighMask(";:&=+$,");

		// reg_name      = 1*( unreserved | escaped | "$" | "," |
		//                     ";" | ":" | "@" | "&" | "=" | "+" )
		private static readonly long L_REG_NAME = L_UNRESERVED | L_ESCAPED | LowMask("$,;:@&=+");
		private static readonly long H_REG_NAME = H_UNRESERVED | H_ESCAPED | HighMask("$,;:@&=+");

		// All valid characters for server-based authorities
		private static readonly long L_SERVER = L_USERINFO | L_ALPHANUM | L_DASH | LowMask(".:@[]");
		private static readonly long H_SERVER = H_USERINFO | H_ALPHANUM | H_DASH | HighMask(".:@[]");

		// Special case of server authority that represents an IPv6 address
		// In this case, a % does not signify an escape sequence
		private static readonly long L_SERVER_PERCENT = L_SERVER | LowMask("%");
		private static readonly long H_SERVER_PERCENT = H_SERVER | HighMask("%");
		private static readonly long L_LEFT_BRACKET = LowMask("[");
		private static readonly long H_LEFT_BRACKET = HighMask("[");

		// scheme        = alpha *( alpha | digit | "+" | "-" | "." )
		private static readonly long L_SCHEME = L_ALPHA | L_DIGIT | LowMask("+-.");
		private static readonly long H_SCHEME = H_ALPHA | H_DIGIT | HighMask("+-.");

		// uric_no_slash = unreserved | escaped | ";" | "?" | ":" | "@" |
		//                 "&" | "=" | "+" | "$" | ","
		private static readonly long L_URIC_NO_SLASH = L_UNRESERVED | L_ESCAPED | LowMask(";?:@&=+$,");
		private static readonly long H_URIC_NO_SLASH = H_UNRESERVED | H_ESCAPED | HighMask(";?:@&=+$,");


		// -- Escaping and encoding --

		private static readonly char[] HexDigits = new char[] {'0', '1', '2', '3', '4', '5', '6', '7', '8', '9', 'A', 'B', 'C', 'D', 'E', 'F'};

		private static void AppendEscape(StringBuffer sb, sbyte b)
		{
			sb.Append('%');
			sb.Append(HexDigits[(b >> 4) & 0x0f]);
			sb.Append(HexDigits[(b >> 0) & 0x0f]);
		}

		private static void AppendEncoded(StringBuffer sb, char c)
		{
			ByteBuffer bb = null;
			try
			{
				bb = ThreadLocalCoders.encoderFor("UTF-8").encode(CharBuffer.Wrap("" + c));
			}
			catch (CharacterCodingException)
			{
				Debug.Assert(false);
			}
			while (bb.HasRemaining())
			{
				int b = bb.Get() & 0xff;
				if (b >= 0x80)
				{
					AppendEscape(sb, (sbyte)b);
				}
				else
				{
					sb.Append((char)b);
				}
			}
		}

		// Quote any characters in s that are not permitted
		// by the given mask pair
		//
		private static String Quote(String s, long lowMask, long highMask)
		{
			int n = s.Length();
			StringBuffer sb = null;
			bool allowNonASCII = ((lowMask & L_ESCAPED) != 0);
			for (int i = 0; i < s.Length(); i++)
			{
				char c = s.CharAt(i);
				if (c < '\u0080')
				{
					if (!Match(c, lowMask, highMask))
					{
						if (sb == null)
						{
							sb = new StringBuffer();
							sb.Append(s.Substring(0, i));
						}
						AppendEscape(sb, (sbyte)c);
					}
					else
					{
						if (sb != null)
						{
							sb.Append(c);
						}
					}
				}
				else if (allowNonASCII && (Character.IsSpaceChar(c) || char.IsControl(c)))
				{
					if (sb == null)
					{
						sb = new StringBuffer();
						sb.Append(s.Substring(0, i));
					}
					AppendEncoded(sb, c);
				}
				else
				{
					if (sb != null)
					{
						sb.Append(c);
					}
				}
			}
			return (sb == null) ? s : sb.ToString();
		}

		// Encodes all characters >= \u0080 into escaped, normalized UTF-8 octets,
		// assuming that s is otherwise legal
		//
		private static String Encode(String s)
		{
			int n = s.Length();
			if (n == 0)
			{
				return s;
			}

			// First check whether we actually need to encode
			for (int i = 0;;)
			{
				if (s.CharAt(i) >= '\u0080')
				{
					break;
				}
				if (++i >= n)
				{
					return s;
				}
			}

			String ns = Normalizer.Normalize(s, Normalizer.Form.NFC);
			ByteBuffer bb = null;
			try
			{
				bb = ThreadLocalCoders.encoderFor("UTF-8").encode(CharBuffer.Wrap(ns));
			}
			catch (CharacterCodingException)
			{
				Debug.Assert(false);
			}

			StringBuffer sb = new StringBuffer();
			while (bb.HasRemaining())
			{
				int b = bb.Get() & 0xff;
				if (b >= 0x80)
				{
					AppendEscape(sb, (sbyte)b);
				}
				else
				{
					sb.Append((char)b);
				}
			}
			return sb.ToString();
		}

		private static int Decode(char c)
		{
			if ((c >= '0') && (c <= '9'))
			{
				return c - '0';
			}
			if ((c >= 'a') && (c <= 'f'))
			{
				return c - 'a' + 10;
			}
			if ((c >= 'A') && (c <= 'F'))
			{
				return c - 'A' + 10;
			}
			Debug.Assert(false);
			return -1;
		}

		private static sbyte Decode(char c1, char c2)
		{
			return (sbyte)(((Decode(c1) & 0xf) << 4) | ((Decode(c2) & 0xf) << 0));
		}

		// Evaluates all escapes in s, applying UTF-8 decoding if needed.  Assumes
		// that escapes are well-formed syntactically, i.e., of the form %XX.  If a
		// sequence of escaped octets is not valid UTF-8 then the erroneous octets
		// are replaced with '\uFFFD'.
		// Exception: any "%" found between "[]" is left alone. It is an IPv6 literal
		//            with a scope_id
		//
		private static String Decode(String s)
		{
			if (s == null)
			{
				return s;
			}
			int n = s.Length();
			if (n == 0)
			{
				return s;
			}
			if (s.IndexOf('%') < 0)
			{
				return s;
			}

			StringBuffer sb = new StringBuffer(n);
			ByteBuffer bb = ByteBuffer.Allocate(n);
			CharBuffer cb = CharBuffer.Allocate(n);
			CharsetDecoder dec = ThreadLocalCoders.decoderFor("UTF-8").onMalformedInput(CodingErrorAction.REPLACE).onUnmappableCharacter(CodingErrorAction.REPLACE);

			// This is not horribly efficient, but it will do for now
			char c = s.CharAt(0);
			bool betweenBrackets = false;

			for (int i = 0; i < n;)
			{
				Debug.Assert(c == s.CharAt(i)); // Loop invariant
				if (c == '[')
				{
					betweenBrackets = true;
				}
				else if (betweenBrackets && c == ']')
				{
					betweenBrackets = false;
				}
				if (c != '%' || betweenBrackets)
				{
					sb.Append(c);
					if (++i >= n)
					{
						break;
					}
					c = s.CharAt(i);
					continue;
				}
				bb.Clear();
				int ui = i;
				for (;;)
				{
					assert(n - i >= 2);
					bb.Put(Decode(s.CharAt(++i), s.CharAt(++i)));
					if (++i >= n)
					{
						break;
					}
					c = s.CharAt(i);
					if (c != '%')
					{
						break;
					}
				}
				bb.Flip();
				cb.Clear();
				dec.Reset();
				CoderResult cr = dec.Decode(bb, cb, true);
				Debug.Assert(cr.Underflow);
				cr = dec.Flush(cb);
				Debug.Assert(cr.Underflow);
				sb.Append(cb.Flip().ToString());
			}

			return sb.ToString();
		}


		// -- Parsing --

		// For convenience we wrap the input URI string in a new instance of the
		// following internal class.  This saves always having to pass the input
		// string as an argument to each internal scan/parse method.

		private class Parser
		{
			private readonly URI OuterInstance;


			internal String Input; // URI input string
			internal bool RequireServerAuthority = false;

			internal Parser(URI outerInstance, String s)
			{
				this.OuterInstance = outerInstance;
				Input = s;
				outerInstance.@string = s;
			}

			// -- Methods for throwing URISyntaxException in various ways --

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: private void fail(String reason) throws URISyntaxException
			internal virtual void Fail(String reason)
			{
				throw new URISyntaxException(Input, reason);
			}

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: private void fail(String reason, int p) throws URISyntaxException
			internal virtual void Fail(String reason, int p)
			{
				throw new URISyntaxException(Input, reason, p);
			}

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: private void failExpecting(String expected, int p) throws URISyntaxException
			internal virtual void FailExpecting(String expected, int p)
			{
				Fail("Expected " + expected, p);
			}

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: private void failExpecting(String expected, String prior, int p) throws URISyntaxException
			internal virtual void FailExpecting(String expected, String prior, int p)
			{
				Fail("Expected " + expected + " following " + prior, p);
			}


			// -- Simple access to the input string --

			// Return a substring of the input string
			//
			internal virtual String Substring(int start, int end)
			{
				return Input.Substring(start, end - start);
			}

			// Return the char at position p,
			// assuming that p < input.length()
			//
			internal virtual char CharAt(int p)
			{
				return Input.CharAt(p);
			}

			// Tells whether start < end and, if so, whether charAt(start) == c
			//
			internal virtual bool At(int start, int end, char c)
			{
				return (start < end) && (CharAt(start) == c);
			}

			// Tells whether start + s.length() < end and, if so,
			// whether the chars at the start position match s exactly
			//
			internal virtual bool At(int start, int end, String s)
			{
				int p = start;
				int sn = s.Length();
				if (sn > end - p)
				{
					return false;
				}
				int i = 0;
				while (i < sn)
				{
					if (CharAt(p++) != s.CharAt(i))
					{
						break;
					}
					i++;
				}
				return (i == sn);
			}


			// -- Scanning --

			// The various scan and parse methods that follow use a uniform
			// convention of taking the current start position and end index as
			// their first two arguments.  The start is inclusive while the end is
			// exclusive, just as in the String class, i.e., a start/end pair
			// denotes the left-open interval [start, end) of the input string.
			//
			// These methods never proceed past the end position.  They may return
			// -1 to indicate outright failure, but more often they simply return
			// the position of the first char after the last char scanned.  Thus
			// a typical idiom is
			//
			//     int p = start;
			//     int q = scan(p, end, ...);
			//     if (q > p)
			//         // We scanned something
			//         ...;
			//     else if (q == p)
			//         // We scanned nothing
			//         ...;
			//     else if (q == -1)
			//         // Something went wrong
			//         ...;


			// Scan a specific char: If the char at the given start position is
			// equal to c, return the index of the next char; otherwise, return the
			// start position.
			//
			internal virtual int Scan(int start, int end, char c)
			{
				if ((start < end) && (CharAt(start) == c))
				{
					return start + 1;
				}
				return start;
			}

			// Scan forward from the given start position.  Stop at the first char
			// in the err string (in which case -1 is returned), or the first char
			// in the stop string (in which case the index of the preceding char is
			// returned), or the end of the input string (in which case the length
			// of the input string is returned).  May return the start position if
			// nothing matches.
			//
			internal virtual int Scan(int start, int end, String err, String stop)
			{
				int p = start;
				while (p < end)
				{
					char c = CharAt(p);
					if (err.IndexOf(c) >= 0)
					{
						return -1;
					}
					if (stop.IndexOf(c) >= 0)
					{
						break;
					}
					p++;
				}
				return p;
			}

			// Scan a potential escape sequence, starting at the given position,
			// with the given first char (i.e., charAt(start) == c).
			//
			// This method assumes that if escapes are allowed then visible
			// non-US-ASCII chars are also allowed.
			//
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: private int scanEscape(int start, int n, char first) throws URISyntaxException
			internal virtual int ScanEscape(int start, int n, char first)
			{
				int p = start;
				char c = first;
				if (c == '%')
				{
					// Process escape pair
					if ((p + 3 <= n) && Match(CharAt(p + 1), L_HEX, H_HEX) && Match(CharAt(p + 2), L_HEX, H_HEX))
					{
						return p + 3;
					}
					Fail("Malformed escape pair", p);
				}
				else if ((c > 128) && !Character.IsSpaceChar(c) && !char.IsControl(c))
				{
					// Allow unescaped but visible non-US-ASCII chars
					return p + 1;
				}
				return p;
			}

			// Scan chars that match the given mask pair
			//
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: private int scan(int start, int n, long lowMask, long highMask) throws URISyntaxException
			internal virtual int Scan(int start, int n, long lowMask, long highMask)
			{
				int p = start;
				while (p < n)
				{
					char c = CharAt(p);
					if (Match(c, lowMask, highMask))
					{
						p++;
						continue;
					}
					if ((lowMask & L_ESCAPED) != 0)
					{
						int q = ScanEscape(p, n, c);
						if (q > p)
						{
							p = q;
							continue;
						}
					}
					break;
				}
				return p;
			}

			// Check that each of the chars in [start, end) matches the given mask
			//
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: private void checkChars(int start, int end, long lowMask, long highMask, String what) throws URISyntaxException
			internal virtual void CheckChars(int start, int end, long lowMask, long highMask, String what)
			{
				int p = Scan(start, end, lowMask, highMask);
				if (p < end)
				{
					Fail("Illegal character in " + what, p);
				}
			}

			// Check that the char at position p matches the given mask
			//
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: private void checkChar(int p, long lowMask, long highMask, String what) throws URISyntaxException
			internal virtual void CheckChar(int p, long lowMask, long highMask, String what)
			{
				CheckChars(p, p + 1, lowMask, highMask, what);
			}


			// -- Parsing --

			// [<scheme>:]<scheme-specific-part>[#<fragment>]
			//
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: void parse(boolean rsa) throws URISyntaxException
			internal virtual void Parse(bool rsa)
			{
				RequireServerAuthority = rsa;
				int ssp; // Start of scheme-specific part
				int n = Input.Length();
				int p = Scan(0, n, "/?#", ":");
				if ((p >= 0) && At(p, n, ':'))
				{
					if (p == 0)
					{
						FailExpecting("scheme name", 0);
					}
					CheckChar(0, L_ALPHA, H_ALPHA, "scheme name");
					CheckChars(1, p, L_SCHEME, H_SCHEME, "scheme name");
					outerInstance.Scheme_Renamed = Substring(0, p);
					p++; // Skip ':'
					ssp = p;
					if (At(p, n, '/'))
					{
						p = ParseHierarchical(p, n);
					}
					else
					{
						int q = Scan(p, n, "", "#");
						if (q <= p)
						{
							FailExpecting("scheme-specific part", p);
						}
						CheckChars(p, q, L_URIC, H_URIC, "opaque part");
						p = q;
					}
				}
				else
				{
					ssp = 0;
					p = ParseHierarchical(0, n);
				}
				outerInstance.SchemeSpecificPart_Renamed = Substring(ssp, p);
				if (At(p, n, '#'))
				{
					CheckChars(p + 1, n, L_URIC, H_URIC, "fragment");
					outerInstance.Fragment_Renamed = Substring(p + 1, n);
					p = n;
				}
				if (p < n)
				{
					Fail("end of URI", p);
				}
			}

			// [//authority]<path>[?<query>]
			//
			// DEVIATION from RFC2396: We allow an empty authority component as
			// long as it's followed by a non-empty path, query component, or
			// fragment component.  This is so that URIs such as "file:///foo/bar"
			// will parse.  This seems to be the intent of RFC2396, though the
			// grammar does not permit it.  If the authority is empty then the
			// userInfo, host, and port components are undefined.
			//
			// DEVIATION from RFC2396: We allow empty relative paths.  This seems
			// to be the intent of RFC2396, but the grammar does not permit it.
			// The primary consequence of this deviation is that "#f" parses as a
			// relative URI with an empty path.
			//
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: private int parseHierarchical(int start, int n) throws URISyntaxException
			internal virtual int ParseHierarchical(int start, int n)
			{
				int p = start;
				if (At(p, n, '/') && At(p + 1, n, '/'))
				{
					p += 2;
					int q = Scan(p, n, "", "/?#");
					if (q > p)
					{
						p = ParseAuthority(p, q);
					}
					else if (q < n)
					{
						// DEVIATION: Allow empty authority prior to non-empty
						// path, query component or fragment identifier
					}
					else
					{
						FailExpecting("authority", p);
					}
				}
				int q = Scan(p, n, "", "?#"); // DEVIATION: May be empty
				CheckChars(p, q, L_PATH, H_PATH, "path");
				outerInstance.Path_Renamed = Substring(p, q);
				p = q;
				if (At(p, n, '?'))
				{
					p++;
					q = Scan(p, n, "", "#");
					CheckChars(p, q, L_URIC, H_URIC, "query");
					outerInstance.Query_Renamed = Substring(p, q);
					p = q;
				}
				return p;
			}

			// authority     = server | reg_name
			//
			// Ambiguity: An authority that is a registry name rather than a server
			// might have a prefix that parses as a server.  We use the fact that
			// the authority component is always followed by '/' or the end of the
			// input string to resolve this: If the complete authority did not
			// parse as a server then we try to parse it as a registry name.
			//
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: private int parseAuthority(int start, int n) throws URISyntaxException
			internal virtual int ParseAuthority(int start, int n)
			{
				int p = start;
				int q = p;
				URISyntaxException ex = null;

				bool serverChars;
				bool regChars;

				if (Scan(p, n, "", "]") > p)
				{
					// contains a literal IPv6 address, therefore % is allowed
					serverChars = (Scan(p, n, L_SERVER_PERCENT, H_SERVER_PERCENT) == n);
				}
				else
				{
					serverChars = (Scan(p, n, L_SERVER, H_SERVER) == n);
				}
				regChars = (Scan(p, n, L_REG_NAME, H_REG_NAME) == n);

				if (regChars && !serverChars)
				{
					// Must be a registry-based authority
					outerInstance.Authority_Renamed = Substring(p, n);
					return n;
				}

				if (serverChars)
				{
					// Might be (probably is) a server-based authority, so attempt
					// to parse it as such.  If the attempt fails, try to treat it
					// as a registry-based authority.
					try
					{
						q = ParseServer(p, n);
						if (q < n)
						{
							FailExpecting("end of authority", q);
						}
						outerInstance.Authority_Renamed = Substring(p, n);
					}
					catch (URISyntaxException x)
					{
						// Undo results of failed parse
						outerInstance.UserInfo_Renamed = null;
						outerInstance.Host_Renamed = null;
						outerInstance.Port_Renamed = -1;
						if (RequireServerAuthority)
						{
							// If we're insisting upon a server-based authority,
							// then just re-throw the exception
							throw x;
						}
						else
						{
							// Save the exception in case it doesn't parse as a
							// registry either
							ex = x;
							q = p;
						}
					}
				}

				if (q < n)
				{
					if (regChars)
					{
						// Registry-based authority
						outerInstance.Authority_Renamed = Substring(p, n);
					}
					else if (ex != null)
					{
						// Re-throw exception; it was probably due to
						// a malformed IPv6 address
						throw ex;
					}
					else
					{
						Fail("Illegal character in authority", q);
					}
				}

				return n;
			}


			// [<userinfo>@]<host>[:<port>]
			//
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: private int parseServer(int start, int n) throws URISyntaxException
			internal virtual int ParseServer(int start, int n)
			{
				int p = start;
				int q;

				// userinfo
				q = Scan(p, n, "/?#", "@");
				if ((q >= p) && At(q, n, '@'))
				{
					CheckChars(p, q, L_USERINFO, H_USERINFO, "user info");
					outerInstance.UserInfo_Renamed = Substring(p, q);
					p = q + 1; // Skip '@'
				}

				// hostname, IPv4 address, or IPv6 address
				if (At(p, n, '['))
				{
					// DEVIATION from RFC2396: Support IPv6 addresses, per RFC2732
					p++;
					q = Scan(p, n, "/?#", "]");
					if ((q > p) && At(q, n, ']'))
					{
						// look for a "%" scope id
						int r = Scan(p, q, "", "%");
						if (r > p)
						{
							ParseIPv6Reference(p, r);
							if (r + 1 == q)
							{
								Fail("scope id expected");
							}
							CheckChars(r + 1, q, L_ALPHANUM, H_ALPHANUM, "scope id");
						}
						else
						{
							ParseIPv6Reference(p, q);
						}
						outerInstance.Host_Renamed = Substring(p - 1, q + 1);
						p = q + 1;
					}
					else
					{
						FailExpecting("closing bracket for IPv6 address", q);
					}
				}
				else
				{
					q = ParseIPv4Address(p, n);
					if (q <= p)
					{
						q = ParseHostname(p, n);
					}
					p = q;
				}

				// port
				if (At(p, n, ':'))
				{
					p++;
					q = Scan(p, n, "", "/");
					if (q > p)
					{
						CheckChars(p, q, L_DIGIT, H_DIGIT, "port number");
						try
						{
							outerInstance.Port_Renamed = Convert.ToInt32(Substring(p, q));
						}
						catch (NumberFormatException)
						{
							Fail("Malformed port number", p);
						}
						p = q;
					}
				}
				if (p < n)
				{
					FailExpecting("port number", p);
				}

				return p;
			}

			// Scan a string of decimal digits whose value fits in a byte
			//
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: private int scanByte(int start, int n) throws URISyntaxException
			internal virtual int ScanByte(int start, int n)
			{
				int p = start;
				int q = Scan(p, n, L_DIGIT, H_DIGIT);
				if (q <= p)
				{
					return q;
				}
				if (Convert.ToInt32(Substring(p, q)) > 255)
				{
					return p;
				}
				return q;
			}

			// Scan an IPv4 address.
			//
			// If the strict argument is true then we require that the given
			// interval contain nothing besides an IPv4 address; if it is false
			// then we only require that it start with an IPv4 address.
			//
			// If the interval does not contain or start with (depending upon the
			// strict argument) a legal IPv4 address characters then we return -1
			// immediately; otherwise we insist that these characters parse as a
			// legal IPv4 address and throw an exception on failure.
			//
			// We assume that any string of decimal digits and dots must be an IPv4
			// address.  It won't parse as a hostname anyway, so making that
			// assumption here allows more meaningful exceptions to be thrown.
			//
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: private int scanIPv4Address(int start, int n, boolean strict) throws URISyntaxException
			internal virtual int ScanIPv4Address(int start, int n, bool strict)
			{
				int p = start;
				int q;
				int m = Scan(p, n, L_DIGIT | L_DOT, H_DIGIT | H_DOT);
				if ((m <= p) || (strict && (m != n)))
				{
					return -1;
				}
				for (;;)
				{
					// Per RFC2732: At most three digits per byte
					// Further constraint: Each element fits in a byte
					if ((q = ScanByte(p, m)) <= p)
					{
						break;
					}
					p = q;
					if ((q = Scan(p, m, '.')) <= p)
					{
						break;
					}
					p = q;
					if ((q = ScanByte(p, m)) <= p)
					{
						break;
					}
					p = q;
					if ((q = Scan(p, m, '.')) <= p)
					{
						break;
					}
					p = q;
					if ((q = ScanByte(p, m)) <= p)
					{
						break;
					}
					p = q;
					if ((q = Scan(p, m, '.')) <= p)
					{
						break;
					}
					p = q;
					if ((q = ScanByte(p, m)) <= p)
					{
						break;
					}
					p = q;
					if (q < m)
					{
						break;
					}
					return q;
				}
				Fail("Malformed IPv4 address", q);
				return -1;
			}

			// Take an IPv4 address: Throw an exception if the given interval
			// contains anything except an IPv4 address
			//
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: private int takeIPv4Address(int start, int n, String expected) throws URISyntaxException
			internal virtual int TakeIPv4Address(int start, int n, String expected)
			{
				int p = ScanIPv4Address(start, n, true);
				if (p <= start)
				{
					FailExpecting(expected, start);
				}
				return p;
			}

			// Attempt to parse an IPv4 address, returning -1 on failure but
			// allowing the given interval to contain [:<characters>] after
			// the IPv4 address.
			//
			internal virtual int ParseIPv4Address(int start, int n)
			{
				int p;

				try
				{
					p = ScanIPv4Address(start, n, false);
				}
				catch (URISyntaxException)
				{
					return -1;
				}
				catch (NumberFormatException)
				{
					return -1;
				}

				if (p > start && p < n)
				{
					// IPv4 address is followed by something - check that
					// it's a ":" as this is the only valid character to
					// follow an address.
					if (CharAt(p) != ':')
					{
						p = -1;
					}
				}

				if (p > start)
				{
					outerInstance.Host_Renamed = Substring(start, p);
				}

				return p;
			}

			// hostname      = domainlabel [ "." ] | 1*( domainlabel "." ) toplabel [ "." ]
			// domainlabel   = alphanum | alphanum *( alphanum | "-" ) alphanum
			// toplabel      = alpha | alpha *( alphanum | "-" ) alphanum
			//
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: private int parseHostname(int start, int n) throws URISyntaxException
			internal virtual int ParseHostname(int start, int n)
			{
				int p = start;
				int q;
				int l = -1; // Start of last parsed label

				do
				{
					// domainlabel = alphanum [ *( alphanum | "-" ) alphanum ]
					q = Scan(p, n, L_ALPHANUM, H_ALPHANUM);
					if (q <= p)
					{
						break;
					}
					l = p;
					if (q > p)
					{
						p = q;
						q = Scan(p, n, L_ALPHANUM | L_DASH, H_ALPHANUM | H_DASH);
						if (q > p)
						{
							if (CharAt(q - 1) == '-')
							{
								Fail("Illegal character in hostname", q - 1);
							}
							p = q;
						}
					}
					q = Scan(p, n, '.');
					if (q <= p)
					{
						break;
					}
					p = q;
				} while (p < n);

				if ((p < n) && !At(p, n, ':'))
				{
					Fail("Illegal character in hostname", p);
				}

				if (l < 0)
				{
					FailExpecting("hostname", start);
				}

				// for a fully qualified hostname check that the rightmost
				// label starts with an alpha character.
				if (l > start && !Match(CharAt(l), L_ALPHA, H_ALPHA))
				{
					Fail("Illegal character in hostname", l);
				}

				outerInstance.Host_Renamed = Substring(start, p);
				return p;
			}


			// IPv6 address parsing, from RFC2373: IPv6 Addressing Architecture
			//
			// Bug: The grammar in RFC2373 Appendix B does not allow addresses of
			// the form ::12.34.56.78, which are clearly shown in the examples
			// earlier in the document.  Here is the original grammar:
			//
			//   IPv6address = hexpart [ ":" IPv4address ]
			//   hexpart     = hexseq | hexseq "::" [ hexseq ] | "::" [ hexseq ]
			//   hexseq      = hex4 *( ":" hex4)
			//   hex4        = 1*4HEXDIG
			//
			// We therefore use the following revised grammar:
			//
			//   IPv6address = hexseq [ ":" IPv4address ]
			//                 | hexseq [ "::" [ hexpost ] ]
			//                 | "::" [ hexpost ]
			//   hexpost     = hexseq | hexseq ":" IPv4address | IPv4address
			//   hexseq      = hex4 *( ":" hex4)
			//   hex4        = 1*4HEXDIG
			//
			// This covers all and only the following cases:
			//
			//   hexseq
			//   hexseq : IPv4address
			//   hexseq ::
			//   hexseq :: hexseq
			//   hexseq :: hexseq : IPv4address
			//   hexseq :: IPv4address
			//   :: hexseq
			//   :: hexseq : IPv4address
			//   :: IPv4address
			//   ::
			//
			// Additionally we constrain the IPv6 address as follows :-
			//
			//  i.  IPv6 addresses without compressed zeros should contain
			//      exactly 16 bytes.
			//
			//  ii. IPv6 addresses with compressed zeros should contain
			//      less than 16 bytes.

			internal int Ipv6byteCount = 0;

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: private int parseIPv6Reference(int start, int n) throws URISyntaxException
			internal virtual int ParseIPv6Reference(int start, int n)
			{
				int p = start;
				int q;
				bool compressedZeros = false;

				q = ScanHexSeq(p, n);

				if (q > p)
				{
					p = q;
					if (At(p, n, "::"))
					{
						compressedZeros = true;
						p = ScanHexPost(p + 2, n);
					}
					else if (At(p, n, ':'))
					{
						p = TakeIPv4Address(p + 1, n, "IPv4 address");
						Ipv6byteCount += 4;
					}
				}
				else if (At(p, n, "::"))
				{
					compressedZeros = true;
					p = ScanHexPost(p + 2, n);
				}
				if (p < n)
				{
					Fail("Malformed IPv6 address", start);
				}
				if (Ipv6byteCount > 16)
				{
					Fail("IPv6 address too long", start);
				}
				if (!compressedZeros && Ipv6byteCount < 16)
				{
					Fail("IPv6 address too short", start);
				}
				if (compressedZeros && Ipv6byteCount == 16)
				{
					Fail("Malformed IPv6 address", start);
				}

				return p;
			}

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: private int scanHexPost(int start, int n) throws URISyntaxException
			internal virtual int ScanHexPost(int start, int n)
			{
				int p = start;
				int q;

				if (p == n)
				{
					return p;
				}

				q = ScanHexSeq(p, n);
				if (q > p)
				{
					p = q;
					if (At(p, n, ':'))
					{
						p++;
						p = TakeIPv4Address(p, n, "hex digits or IPv4 address");
						Ipv6byteCount += 4;
					}
				}
				else
				{
					p = TakeIPv4Address(p, n, "hex digits or IPv4 address");
					Ipv6byteCount += 4;
				}
				return p;
			}

			// Scan a hex sequence; return -1 if one could not be scanned
			//
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: private int scanHexSeq(int start, int n) throws URISyntaxException
			internal virtual int ScanHexSeq(int start, int n)
			{
				int p = start;
				int q;

				q = Scan(p, n, L_HEX, H_HEX);
				if (q <= p)
				{
					return -1;
				}
				if (At(q, n, '.')) // Beginning of IPv4 address
				{
					return -1;
				}
				if (q > p + 4)
				{
					Fail("IPv6 hexadecimal digit sequence too long", p);
				}
				Ipv6byteCount += 2;
				p = q;
				while (p < n)
				{
					if (!At(p, n, ':'))
					{
						break;
					}
					if (At(p + 1, n, ':'))
					{
						break; // "::"
					}
					p++;
					q = Scan(p, n, L_HEX, H_HEX);
					if (q <= p)
					{
						FailExpecting("digits for an IPv6 address", p);
					}
					if (At(q, n, '.')) // Beginning of IPv4 address
					{
						p--;
						break;
					}
					if (q > p + 4)
					{
						Fail("IPv6 hexadecimal digit sequence too long", p);
					}
					Ipv6byteCount += 2;
					p = q;
				}

				return p;
			}

		}

	}

}
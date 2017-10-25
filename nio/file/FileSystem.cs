/*
 * Copyright (c) 2007, 2013, Oracle and/or its affiliates. All rights reserved.
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

namespace java.nio.file
{


	/// <summary>
	/// Provides an interface to a file system and is the factory for objects to
	/// access files and other objects in the file system.
	/// 
	/// <para> The default file system, obtained by invoking the {@link FileSystems#getDefault
	/// FileSystems.getDefault} method, provides access to the file system that is
	/// accessible to the Java virtual machine. The <seealso cref="FileSystems"/> class defines
	/// methods to create file systems that provide access to other types of (custom)
	/// file systems.
	/// 
	/// </para>
	/// <para> A file system is the factory for several types of objects:
	/// 
	/// <ul>
	/// </para>
	///   <li><para> The <seealso cref="#getPath getPath"/> method converts a system dependent
	///     <em>path string</em>, returning a <seealso cref="Path"/> object that may be used
	///     to locate and access a file. </para></li>
	///   <li><para> The <seealso cref="#getPathMatcher  getPathMatcher"/> method is used
	///     to create a <seealso cref="PathMatcher"/> that performs match operations on
	///     paths. </para></li>
	///   <li><para> The <seealso cref="#getFileStores getFileStores"/> method returns an iterator
	///     over the underlying <seealso cref="FileStore file-stores"/>. </para></li>
	///   <li><para> The <seealso cref="#getUserPrincipalLookupService getUserPrincipalLookupService"/>
	///     method returns the <seealso cref="UserPrincipalLookupService"/> to lookup users or
	///     groups by name. </para></li>
	///   <li><para> The <seealso cref="#newWatchService newWatchService"/> method creates a
	///     <seealso cref="WatchService"/> that may be used to watch objects for changes and
	///     events. </para></li>
	/// </ul>
	/// 
	/// <para> File systems vary greatly. In some cases the file system is a single
	/// hierarchy of files with one top-level root directory. In other cases it may
	/// have several distinct file hierarchies, each with its own top-level root
	/// directory. The <seealso cref="#getRootDirectories getRootDirectories"/> method may be
	/// used to iterate over the root directories in the file system. A file system
	/// is typically composed of one or more underlying <seealso cref="FileStore file-stores"/>
	/// that provide the storage for the files. Theses file stores can also vary in
	/// the features they support, and the file attributes or <em>meta-data</em> that
	/// they associate with files.
	/// 
	/// </para>
	/// <para> A file system is open upon creation and can be closed by invoking its
	/// <seealso cref="#close() close"/> method. Once closed, any further attempt to access
	/// objects in the file system cause <seealso cref="ClosedFileSystemException"/> to be
	/// thrown. File systems created by the default <seealso cref="FileSystemProvider provider"/>
	/// cannot be closed.
	/// 
	/// </para>
	/// <para> A {@code FileSystem} can provide read-only or read-write access to the
	/// file system. Whether or not a file system provides read-only access is
	/// established when the {@code FileSystem} is created and can be tested by invoking
	/// its <seealso cref="#isReadOnly() isReadOnly"/> method. Attempts to write to file stores
	/// by means of an object associated with a read-only file system throws {@link
	/// ReadOnlyFileSystemException}.
	/// 
	/// </para>
	/// <para> File systems are safe for use by multiple concurrent threads. The {@link
	/// #close close} method may be invoked at any time to close a file system but
	/// whether a file system is <i>asynchronously closeable</i> is provider specific
	/// and therefore unspecified. In other words, if a thread is accessing an
	/// object in a file system, and another thread invokes the {@code close} method
	/// then it may require to block until the first operation is complete. Closing
	/// a file system causes all open channels, watch services, and other {@link
	/// Closeable closeable} objects associated with the file system to be closed.
	/// 
	/// @since 1.7
	/// </para>
	/// </summary>

	public abstract class FileSystem : Closeable
	{
		/// <summary>
		/// Initializes a new instance of this class.
		/// </summary>
		protected internal FileSystem()
		{
		}

		/// <summary>
		/// Returns the provider that created this file system.
		/// </summary>
		/// <returns>  The provider that created this file system. </returns>
		public abstract FileSystemProvider Provider();

		/// <summary>
		/// Closes this file system.
		/// 
		/// <para> After a file system is closed then all subsequent access to the file
		/// system, either by methods defined by this class or on objects associated
		/// with this file system, throw <seealso cref="ClosedFileSystemException"/>. If the
		/// file system is already closed then invoking this method has no effect.
		/// 
		/// </para>
		/// <para> Closing a file system will close all open {@link
		/// java.nio.channels.Channel channels}, <seealso cref="DirectoryStream directory-streams"/>,
		/// <seealso cref="WatchService watch-service"/>, and other closeable objects associated
		/// with this file system. The <seealso cref="FileSystems#getDefault default"/> file
		/// system cannot be closed.
		/// 
		/// </para>
		/// </summary>
		/// <exception cref="IOException">
		///          If an I/O error occurs </exception>
		/// <exception cref="UnsupportedOperationException">
		///          Thrown in the case of the default file system </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: @Override public abstract void close() throws java.io.IOException;
		public override abstract void Close();

		/// <summary>
		/// Tells whether or not this file system is open.
		/// 
		/// <para> File systems created by the default provider are always open.
		/// 
		/// </para>
		/// </summary>
		/// <returns>  {@code true} if, and only if, this file system is open </returns>
		public abstract bool Open {get;}

		/// <summary>
		/// Tells whether or not this file system allows only read-only access to
		/// its file stores.
		/// </summary>
		/// <returns>  {@code true} if, and only if, this file system provides
		///          read-only access </returns>
		public abstract bool ReadOnly {get;}

		/// <summary>
		/// Returns the name separator, represented as a string.
		/// 
		/// <para> The name separator is used to separate names in a path string. An
		/// implementation may support multiple name separators in which case this
		/// method returns an implementation specific <em>default</em> name separator.
		/// This separator is used when creating path strings by invoking the {@link
		/// Path#toString() toString()} method.
		/// 
		/// </para>
		/// <para> In the case of the default provider, this method returns the same
		/// separator as <seealso cref="java.io.File#separator"/>.
		/// 
		/// </para>
		/// </summary>
		/// <returns>  The name separator </returns>
		public abstract String Separator {get;}

		/// <summary>
		/// Returns an object to iterate over the paths of the root directories.
		/// 
		/// <para> A file system provides access to a file store that may be composed
		/// of a number of distinct file hierarchies, each with its own top-level
		/// root directory. Unless denied by the security manager, each element in
		/// the returned iterator corresponds to the root directory of a distinct
		/// file hierarchy. The order of the elements is not defined. The file
		/// hierarchies may change during the lifetime of the Java virtual machine.
		/// For example, in some implementations, the insertion of removable media
		/// may result in the creation of a new file hierarchy with its own
		/// top-level directory.
		/// 
		/// </para>
		/// <para> When a security manager is installed, it is invoked to check access
		/// to the each root directory. If denied, the root directory is not returned
		/// by the iterator. In the case of the default provider, the {@link
		/// SecurityManager#checkRead(String)} method is invoked to check read access
		/// to each root directory. It is system dependent if the permission checks
		/// are done when the iterator is obtained or during iteration.
		/// 
		/// </para>
		/// </summary>
		/// <returns>  An object to iterate over the root directories </returns>
		public abstract Iterable<Path> RootDirectories {get;}

		/// <summary>
		/// Returns an object to iterate over the underlying file stores.
		/// 
		/// <para> The elements of the returned iterator are the {@link
		/// FileStore FileStores} for this file system. The order of the elements is
		/// not defined and the file stores may change during the lifetime of the
		/// Java virtual machine. When an I/O error occurs, perhaps because a file
		/// store is not accessible, then it is not returned by the iterator.
		/// 
		/// </para>
		/// <para> In the case of the default provider, and a security manager is
		/// installed, the security manager is invoked to check {@link
		/// RuntimePermission}<tt>("getFileStoreAttributes")</tt>. If denied, then
		/// no file stores are returned by the iterator. In addition, the security
		/// manager's <seealso cref="SecurityManager#checkRead(String)"/> method is invoked to
		/// check read access to the file store's <em>top-most</em> directory. If
		/// denied, the file store is not returned by the iterator. It is system
		/// dependent if the permission checks are done when the iterator is obtained
		/// or during iteration.
		/// 
		/// </para>
		/// <para> <b>Usage Example:</b>
		/// Suppose we want to print the space usage for all file stores:
		/// <pre>
		///     for (FileStore store: FileSystems.getDefault().getFileStores()) {
		///         long total = store.getTotalSpace() / 1024;
		///         long used = (store.getTotalSpace() - store.getUnallocatedSpace()) / 1024;
		///         long avail = store.getUsableSpace() / 1024;
		///         System.out.format("%-20s %12d %12d %12d%n", store, total, used, avail);
		///     }
		/// </pre>
		/// 
		/// </para>
		/// </summary>
		/// <returns>  An object to iterate over the backing file stores </returns>
		public abstract Iterable<FileStore> FileStores {get;}

		/// <summary>
		/// Returns the set of the <seealso cref="FileAttributeView#name names"/> of the file
		/// attribute views supported by this {@code FileSystem}.
		/// 
		/// <para> The <seealso cref="BasicFileAttributeView"/> is required to be supported and
		/// therefore the set contains at least one element, "basic".
		/// 
		/// </para>
		/// <para> The {@link FileStore#supportsFileAttributeView(String)
		/// supportsFileAttributeView(String)} method may be used to test if an
		/// underlying <seealso cref="FileStore"/> supports the file attributes identified by a
		/// file attribute view.
		/// 
		/// </para>
		/// </summary>
		/// <returns>  An unmodifiable set of the names of the supported file attribute
		///          views </returns>
		public abstract Set<String> SupportedFileAttributeViews();

		/// <summary>
		/// Converts a path string, or a sequence of strings that when joined form
		/// a path string, to a {@code Path}. If {@code more} does not specify any
		/// elements then the value of the {@code first} parameter is the path string
		/// to convert. If {@code more} specifies one or more elements then each
		/// non-empty string, including {@code first}, is considered to be a sequence
		/// of name elements (see <seealso cref="Path"/>) and is joined to form a path string.
		/// The details as to how the Strings are joined is provider specific but
		/// typically they will be joined using the {@link #getSeparator
		/// name-separator} as the separator. For example, if the name separator is
		/// "{@code /}" and {@code getPath("/foo","bar","gus")} is invoked, then the
		/// path string {@code "/foo/bar/gus"} is converted to a {@code Path}.
		/// A {@code Path} representing an empty path is returned if {@code first}
		/// is the empty string and {@code more} does not contain any non-empty
		/// strings.
		/// 
		/// <para> The parsing and conversion to a path object is inherently
		/// implementation dependent. In the simplest case, the path string is rejected,
		/// and <seealso cref="InvalidPathException"/> thrown, if the path string contains
		/// characters that cannot be converted to characters that are <em>legal</em>
		/// to the file store. For example, on UNIX systems, the NUL (&#92;u0000)
		/// character is not allowed to be present in a path. An implementation may
		/// choose to reject path strings that contain names that are longer than those
		/// allowed by any file store, and where an implementation supports a complex
		/// path syntax, it may choose to reject path strings that are <em>badly
		/// formed</em>.
		/// 
		/// </para>
		/// <para> In the case of the default provider, path strings are parsed based
		/// on the definition of paths at the platform or virtual file system level.
		/// For example, an operating system may not allow specific characters to be
		/// present in a file name, but a specific underlying file store may impose
		/// different or additional restrictions on the set of legal
		/// characters.
		/// 
		/// </para>
		/// <para> This method throws <seealso cref="InvalidPathException"/> when the path string
		/// cannot be converted to a path. Where possible, and where applicable,
		/// the exception is created with an {@link InvalidPathException#getIndex
		/// index} value indicating the first position in the {@code path} parameter
		/// that caused the path string to be rejected.
		/// 
		/// </para>
		/// </summary>
		/// <param name="first">
		///          the path string or initial part of the path string </param>
		/// <param name="more">
		///          additional strings to be joined to form the path string
		/// </param>
		/// <returns>  the resulting {@code Path}
		/// </returns>
		/// <exception cref="InvalidPathException">
		///          If the path string cannot be converted </exception>
		public abstract Path GetPath(String first, params String[] more);

		/// <summary>
		/// Returns a {@code PathMatcher} that performs match operations on the
		/// {@code String} representation of <seealso cref="Path"/> objects by interpreting a
		/// given pattern.
		/// 
		/// The {@code syntaxAndPattern} parameter identifies the syntax and the
		/// pattern and takes the form:
		/// <blockquote><pre>
		/// <i>syntax</i><b>:</b><i>pattern</i>
		/// </pre></blockquote>
		/// where {@code ':'} stands for itself.
		/// 
		/// <para> A {@code FileSystem} implementation supports the "{@code glob}" and
		/// "{@code regex}" syntaxes, and may support others. The value of the syntax
		/// component is compared without regard to case.
		/// 
		/// </para>
		/// <para> When the syntax is "{@code glob}" then the {@code String}
		/// representation of the path is matched using a limited pattern language
		/// that resembles regular expressions but with a simpler syntax. For example:
		/// 
		/// <blockquote>
		/// <table border="0" summary="Pattern Language">
		/// <tr>
		///   <td>{@code *.java}</td>
		///   <td>Matches a path that represents a file name ending in {@code .java}</td>
		/// </tr>
		/// <tr>
		///   <td>{@code *.*}</td>
		///   <td>Matches file names containing a dot</td>
		/// </tr>
		/// <tr>
		///   <td>{@code *.{java,class}}</td>
		///   <td>Matches file names ending with {@code .java} or {@code .class}</td>
		/// </tr>
		/// <tr>
		///   <td>{@code foo.?}</td>
		///   <td>Matches file names starting with {@code foo.} and a single
		///   character extension</td>
		/// </tr>
		/// <tr>
		///   <td><tt>&#47;home&#47;*&#47;*</tt>
		///   <td>Matches <tt>&#47;home&#47;gus&#47;data</tt> on UNIX platforms</td>
		/// </tr>
		/// <tr>
		///   <td><tt>&#47;home&#47;**</tt>
		///   <td>Matches <tt>&#47;home&#47;gus</tt> and
		///   <tt>&#47;home&#47;gus&#47;data</tt> on UNIX platforms</td>
		/// </tr>
		/// <tr>
		///   <td><tt>C:&#92;&#92;*</tt>
		///   <td>Matches <tt>C:&#92;foo</tt> and <tt>C:&#92;bar</tt> on the Windows
		///   platform (note that the backslash is escaped; as a string literal in the
		///   Java Language the pattern would be <tt>"C:&#92;&#92;&#92;&#92;*"</tt>) </td>
		/// </tr>
		/// 
		/// </table>
		/// </blockquote>
		/// 
		/// </para>
		/// <para> The following rules are used to interpret glob patterns:
		/// 
		/// <ul>
		/// </para>
		///   <li><para> The {@code *} character matches zero or more {@link Character
		///   characters} of a <seealso cref="Path#getName(int) name"/> component without
		///   crossing directory boundaries. </para></li>
		/// 
		///   <li><para> The {@code **} characters matches zero or more {@link Character
		///   characters} crossing directory boundaries. </para></li>
		/// 
		///   <li><para> The {@code ?} character matches exactly one character of a
		///   name component.</para></li>
		/// 
		///   <li><para> The backslash character ({@code \}) is used to escape characters
		///   that would otherwise be interpreted as special characters. The expression
		///   {@code \\} matches a single backslash and "\{" matches a left brace
		///   for example.  </para></li>
		/// 
		///   <li><para> The {@code [ ]} characters are a <i>bracket expression</i> that
		///   match a single character of a name component out of a set of characters.
		///   For example, {@code [abc]} matches {@code "a"}, {@code "b"}, or {@code "c"}.
		///   The hyphen ({@code -}) may be used to specify a range so {@code [a-z]}
		///   specifies a range that matches from {@code "a"} to {@code "z"} (inclusive).
		///   These forms can be mixed so [abce-g] matches {@code "a"}, {@code "b"},
		///   {@code "c"}, {@code "e"}, {@code "f"} or {@code "g"}. If the character
		///   after the {@code [} is a {@code !} then it is used for negation so {@code
		///   [!a-c]} matches any character except {@code "a"}, {@code "b"}, or {@code
		///   "c"}.
		/// </para>
		///   <para> Within a bracket expression the {@code *}, {@code ?} and {@code \}
		///   characters match themselves. The ({@code -}) character matches itself if
		///   it is the first character within the brackets, or the first character
		///   after the {@code !} if negating.</para></li>
		/// 
		///   <li><para> The {@code { }} characters are a group of subpatterns, where
		///   the group matches if any subpattern in the group matches. The {@code ","}
		///   character is used to separate the subpatterns. Groups cannot be nested.
		///   </para></li>
		/// 
		///   <li><para> Leading period<tt>&#47;</tt>dot characters in file name are
		///   treated as regular characters in match operations. For example,
		///   the {@code "*"} glob pattern matches file name {@code ".login"}.
		///   The <seealso cref="Files#isHidden"/> method may be used to test whether a file
		///   is considered hidden.
		///   </para></li>
		/// 
		///   <li><para> All other characters match themselves in an implementation
		///   dependent manner. This includes characters representing any {@link
		///   FileSystem#getSeparator name-separators}. </para></li>
		/// 
		///   <li><para> The matching of <seealso cref="Path#getRoot root"/> components is highly
		///   implementation-dependent and is not specified. </para></li>
		/// 
		/// </ul>
		/// 
		/// <para> When the syntax is "{@code regex}" then the pattern component is a
		/// regular expression as defined by the <seealso cref="java.util.regex.Pattern"/>
		/// class.
		/// 
		/// </para>
		/// <para>  For both the glob and regex syntaxes, the matching details, such as
		/// whether the matching is case sensitive, are implementation-dependent
		/// and therefore not specified.
		/// 
		/// </para>
		/// </summary>
		/// <param name="syntaxAndPattern">
		///          The syntax and pattern
		/// </param>
		/// <returns>  A path matcher that may be used to match paths against the pattern
		/// </returns>
		/// <exception cref="IllegalArgumentException">
		///          If the parameter does not take the form: {@code syntax:pattern} </exception>
		/// <exception cref="java.util.regex.PatternSyntaxException">
		///          If the pattern is invalid </exception>
		/// <exception cref="UnsupportedOperationException">
		///          If the pattern syntax is not known to the implementation
		/// </exception>
		/// <seealso cref= Files#newDirectoryStream(Path,String) </seealso>
		public abstract PathMatcher GetPathMatcher(String syntaxAndPattern);

		/// <summary>
		/// Returns the {@code UserPrincipalLookupService} for this file system
		/// <i>(optional operation)</i>. The resulting lookup service may be used to
		/// lookup user or group names.
		/// 
		/// <para> <b>Usage Example:</b>
		/// Suppose we want to make "joe" the owner of a file:
		/// <pre>
		///     UserPrincipalLookupService lookupService = FileSystems.getDefault().getUserPrincipalLookupService();
		///     Files.setOwner(path, lookupService.lookupPrincipalByName("joe"));
		/// </pre>
		/// 
		/// </para>
		/// </summary>
		/// <exception cref="UnsupportedOperationException">
		///          If this {@code FileSystem} does not does have a lookup service
		/// </exception>
		/// <returns>  The {@code UserPrincipalLookupService} for this file system </returns>
		public abstract UserPrincipalLookupService UserPrincipalLookupService {get;}

		/// <summary>
		/// Constructs a new <seealso cref="WatchService"/> <i>(optional operation)</i>.
		/// 
		/// <para> This method constructs a new watch service that may be used to watch
		/// registered objects for changes and events.
		/// 
		/// </para>
		/// </summary>
		/// <returns>  a new watch service
		/// </returns>
		/// <exception cref="UnsupportedOperationException">
		///          If this {@code FileSystem} does not support watching file system
		///          objects for changes and events. This exception is not thrown
		///          by {@code FileSystems} created by the default provider. </exception>
		/// <exception cref="IOException">
		///          If an I/O error occurs </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public abstract WatchService newWatchService() throws java.io.IOException;
		public abstract WatchService NewWatchService();
	}

}
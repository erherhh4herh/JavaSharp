using System;
using System.Diagnostics;
using System.Collections.Generic;

/*
 * Copyright (c) 1994, 2013, Oracle and/or its affiliates. All rights reserved.
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

namespace java.io
{

	using GetPropertyAction = sun.security.action.GetPropertyAction;

	/// <summary>
	/// An abstract representation of file and directory pathnames.
	/// 
	/// <para> User interfaces and operating systems use system-dependent <em>pathname
	/// strings</em> to name files and directories.  This class presents an
	/// abstract, system-independent view of hierarchical pathnames.  An
	/// <em>abstract pathname</em> has two components:
	/// 
	/// <ol>
	/// <li> An optional system-dependent <em>prefix</em> string,
	///      such as a disk-drive specifier, <code>"/"</code>&nbsp;for the UNIX root
	///      directory, or <code>"\\\\"</code>&nbsp;for a Microsoft Windows UNC pathname, and
	/// <li> A sequence of zero or more string <em>names</em>.
	/// </ol>
	/// 
	/// The first name in an abstract pathname may be a directory name or, in the
	/// case of Microsoft Windows UNC pathnames, a hostname.  Each subsequent name
	/// in an abstract pathname denotes a directory; the last name may denote
	/// either a directory or a file.  The <em>empty</em> abstract pathname has no
	/// prefix and an empty name sequence.
	/// 
	/// </para>
	/// <para> The conversion of a pathname string to or from an abstract pathname is
	/// inherently system-dependent.  When an abstract pathname is converted into a
	/// pathname string, each name is separated from the next by a single copy of
	/// the default <em>separator character</em>.  The default name-separator
	/// character is defined by the system property <code>file.separator</code>, and
	/// is made available in the public static fields <code>{@link
	/// #separator}</code> and <code><seealso cref="#separatorChar"/></code> of this class.
	/// When a pathname string is converted into an abstract pathname, the names
	/// within it may be separated by the default name-separator character or by any
	/// other name-separator character that is supported by the underlying system.
	/// 
	/// </para>
	/// <para> A pathname, whether abstract or in string form, may be either
	/// <em>absolute</em> or <em>relative</em>.  An absolute pathname is complete in
	/// that no other information is required in order to locate the file that it
	/// denotes.  A relative pathname, in contrast, must be interpreted in terms of
	/// information taken from some other pathname.  By default the classes in the
	/// <code>java.io</code> package always resolve relative pathnames against the
	/// current user directory.  This directory is named by the system property
	/// <code>user.dir</code>, and is typically the directory in which the Java
	/// virtual machine was invoked.
	/// 
	/// </para>
	/// <para> The <em>parent</em> of an abstract pathname may be obtained by invoking
	/// the <seealso cref="#getParent"/> method of this class and consists of the pathname's
	/// prefix and each name in the pathname's name sequence except for the last.
	/// Each directory's absolute pathname is an ancestor of any <tt>File</tt>
	/// object with an absolute abstract pathname which begins with the directory's
	/// absolute pathname.  For example, the directory denoted by the abstract
	/// pathname <tt>"/usr"</tt> is an ancestor of the directory denoted by the
	/// pathname <tt>"/usr/local/bin"</tt>.
	/// 
	/// </para>
	/// <para> The prefix concept is used to handle root directories on UNIX platforms,
	/// and drive specifiers, root directories and UNC pathnames on Microsoft Windows platforms,
	/// as follows:
	/// 
	/// <ul>
	/// 
	/// <li> For UNIX platforms, the prefix of an absolute pathname is always
	/// <code>"/"</code>.  Relative pathnames have no prefix.  The abstract pathname
	/// denoting the root directory has the prefix <code>"/"</code> and an empty
	/// name sequence.
	/// 
	/// <li> For Microsoft Windows platforms, the prefix of a pathname that contains a drive
	/// specifier consists of the drive letter followed by <code>":"</code> and
	/// possibly followed by <code>"\\"</code> if the pathname is absolute.  The
	/// prefix of a UNC pathname is <code>"\\\\"</code>; the hostname and the share
	/// name are the first two names in the name sequence.  A relative pathname that
	/// does not specify a drive has no prefix.
	/// 
	/// </ul>
	/// 
	/// </para>
	/// <para> Instances of this class may or may not denote an actual file-system
	/// object such as a file or a directory.  If it does denote such an object
	/// then that object resides in a <i>partition</i>.  A partition is an
	/// operating system-specific portion of storage for a file system.  A single
	/// storage device (e.g. a physical disk-drive, flash memory, CD-ROM) may
	/// contain multiple partitions.  The object, if any, will reside on the
	/// partition <a name="partName">named</a> by some ancestor of the absolute
	/// form of this pathname.
	/// 
	/// </para>
	/// <para> A file system may implement restrictions to certain operations on the
	/// actual file-system object, such as reading, writing, and executing.  These
	/// restrictions are collectively known as <i>access permissions</i>.  The file
	/// system may have multiple sets of access permissions on a single object.
	/// For example, one set may apply to the object's <i>owner</i>, and another
	/// may apply to all other users.  The access permissions on an object may
	/// cause some methods in this class to fail.
	/// 
	/// </para>
	/// <para> Instances of the <code>File</code> class are immutable; that is, once
	/// created, the abstract pathname represented by a <code>File</code> object
	/// will never change.
	/// 
	/// <h3>Interoperability with {@code java.nio.file} package</h3>
	/// 
	/// </para>
	/// <para> The <a href="../../java/nio/file/package-summary.html">{@code java.nio.file}</a>
	/// package defines interfaces and classes for the Java virtual machine to access
	/// files, file attributes, and file systems. This API may be used to overcome
	/// many of the limitations of the {@code java.io.File} class.
	/// The <seealso cref="#toPath toPath"/> method may be used to obtain a {@link
	/// Path} that uses the abstract path represented by a {@code File} object to
	/// locate a file. The resulting {@code Path} may be used with the {@link
	/// java.nio.file.Files} class to provide more efficient and extensive access to
	/// additional file operations, file attributes, and I/O exceptions to help
	/// diagnose errors when an operation on a file fails.
	/// 
	/// @author  unascribed
	/// @since   JDK1.0
	/// </para>
	/// </summary>

	[Serializable]
	public class File : Comparable<File>
	{

		/// <summary>
		/// The FileSystem object representing the platform's local file system.
		/// </summary>
		private static readonly FileSystem Fs = DefaultFileSystem.FileSystem;

		/// <summary>
		/// This abstract pathname's normalized pathname string. A normalized
		/// pathname string uses the default name-separator character and does not
		/// contain any duplicate or redundant separators.
		/// 
		/// @serial
		/// </summary>
		private readonly String Path_Renamed;

		/// <summary>
		/// Enum type that indicates the status of a file path.
		/// </summary>
		private enum PathStatus
		{
			INVALID,
			CHECKED
		}

		/// <summary>
		/// The flag indicating whether the file path is invalid.
		/// </summary>
		[NonSerialized]
		private PathStatus Status = null;

		/// <summary>
		/// Check if the file has an invalid path. Currently, the inspection of
		/// a file path is very limited, and it only covers Nul character check.
		/// Returning true means the path is definitely invalid/garbage. But
		/// returning false does not guarantee that the path is valid.
		/// </summary>
		/// <returns> true if the file path is invalid. </returns>
		internal bool Invalid
		{
			get
			{
				if (Status == null)
				{
					Status = (this.Path_Renamed.IndexOf('\u0000') < 0) ? PathStatus.CHECKED : PathStatus.INVALID;
				}
				return Status == PathStatus.INVALID;
			}
		}

		/// <summary>
		/// The length of this abstract pathname's prefix, or zero if it has no
		/// prefix.
		/// </summary>
		[NonSerialized]
		private readonly int PrefixLength_Renamed;

		/// <summary>
		/// Returns the length of this abstract pathname's prefix.
		/// For use by FileSystem classes.
		/// </summary>
		internal virtual int PrefixLength
		{
			get
			{
				return PrefixLength_Renamed;
			}
		}

		/// <summary>
		/// The system-dependent default name-separator character.  This field is
		/// initialized to contain the first character of the value of the system
		/// property <code>file.separator</code>.  On UNIX systems the value of this
		/// field is <code>'/'</code>; on Microsoft Windows systems it is <code>'\\'</code>.
		/// </summary>
		/// <seealso cref=     java.lang.System#getProperty(java.lang.String) </seealso>
		public static readonly char SeparatorChar = Fs.Separator;

		/// <summary>
		/// The system-dependent default name-separator character, represented as a
		/// string for convenience.  This string contains a single character, namely
		/// <code><seealso cref="#separatorChar"/></code>.
		/// </summary>
		public static readonly String Separator = "" + SeparatorChar;

		/// <summary>
		/// The system-dependent path-separator character.  This field is
		/// initialized to contain the first character of the value of the system
		/// property <code>path.separator</code>.  This character is used to
		/// separate filenames in a sequence of files given as a <em>path list</em>.
		/// On UNIX systems, this character is <code>':'</code>; on Microsoft Windows systems it
		/// is <code>';'</code>.
		/// </summary>
		/// <seealso cref=     java.lang.System#getProperty(java.lang.String) </seealso>
		public static readonly char PathSeparatorChar = Fs.PathSeparator;

		/// <summary>
		/// The system-dependent path-separator character, represented as a string
		/// for convenience.  This string contains a single character, namely
		/// <code><seealso cref="#pathSeparatorChar"/></code>.
		/// </summary>
		public static readonly String PathSeparator = "" + PathSeparatorChar;


		/* -- Constructors -- */

		/// <summary>
		/// Internal constructor for already-normalized pathname strings.
		/// </summary>
		private File(String pathname, int prefixLength)
		{
			this.Path_Renamed = pathname;
			this.PrefixLength_Renamed = prefixLength;
		}

		/// <summary>
		/// Internal constructor for already-normalized pathname strings.
		/// The parameter order is used to disambiguate this method from the
		/// public(File, String) constructor.
		/// </summary>
		private File(String child, File parent)
		{
			Debug.Assert(parent.Path_Renamed != null);
			assert(!parent.Path_Renamed.Equals(""));
			this.Path_Renamed = Fs.Resolve(parent.Path_Renamed, child);
			this.PrefixLength_Renamed = parent.PrefixLength_Renamed;
		}

		/// <summary>
		/// Creates a new <code>File</code> instance by converting the given
		/// pathname string into an abstract pathname.  If the given string is
		/// the empty string, then the result is the empty abstract pathname.
		/// </summary>
		/// <param name="pathname">  A pathname string </param>
		/// <exception cref="NullPointerException">
		///          If the <code>pathname</code> argument is <code>null</code> </exception>
		public File(String pathname)
		{
			if (pathname == null)
			{
				throw new NullPointerException();
			}
			this.Path_Renamed = Fs.Normalize(pathname);
			this.PrefixLength_Renamed = Fs.PrefixLength(this.Path_Renamed);
		}

		/* Note: The two-argument File constructors do not interpret an empty
		   parent abstract pathname as the current user directory.  An empty parent
		   instead causes the child to be resolved against the system-dependent
		   directory defined by the FileSystem.getDefaultParent method.  On Unix
		   this default is "/", while on Microsoft Windows it is "\\".  This is required for
		   compatibility with the original behavior of this class. */

		/// <summary>
		/// Creates a new <code>File</code> instance from a parent pathname string
		/// and a child pathname string.
		/// 
		/// <para> If <code>parent</code> is <code>null</code> then the new
		/// <code>File</code> instance is created as if by invoking the
		/// single-argument <code>File</code> constructor on the given
		/// <code>child</code> pathname string.
		/// 
		/// </para>
		/// <para> Otherwise the <code>parent</code> pathname string is taken to denote
		/// a directory, and the <code>child</code> pathname string is taken to
		/// denote either a directory or a file.  If the <code>child</code> pathname
		/// string is absolute then it is converted into a relative pathname in a
		/// system-dependent way.  If <code>parent</code> is the empty string then
		/// the new <code>File</code> instance is created by converting
		/// <code>child</code> into an abstract pathname and resolving the result
		/// against a system-dependent default directory.  Otherwise each pathname
		/// string is converted into an abstract pathname and the child abstract
		/// pathname is resolved against the parent.
		/// 
		/// </para>
		/// </summary>
		/// <param name="parent">  The parent pathname string </param>
		/// <param name="child">   The child pathname string </param>
		/// <exception cref="NullPointerException">
		///          If <code>child</code> is <code>null</code> </exception>
		public File(String parent, String child)
		{
			if (child == null)
			{
				throw new NullPointerException();
			}
			if (parent != null)
			{
				if (parent.Equals(""))
				{
					this.Path_Renamed = Fs.Resolve(Fs.DefaultParent, Fs.Normalize(child));
				}
				else
				{
					this.Path_Renamed = Fs.Resolve(Fs.Normalize(parent), Fs.Normalize(child));
				}
			}
			else
			{
				this.Path_Renamed = Fs.Normalize(child);
			}
			this.PrefixLength_Renamed = Fs.PrefixLength(this.Path_Renamed);
		}

		/// <summary>
		/// Creates a new <code>File</code> instance from a parent abstract
		/// pathname and a child pathname string.
		/// 
		/// <para> If <code>parent</code> is <code>null</code> then the new
		/// <code>File</code> instance is created as if by invoking the
		/// single-argument <code>File</code> constructor on the given
		/// <code>child</code> pathname string.
		/// 
		/// </para>
		/// <para> Otherwise the <code>parent</code> abstract pathname is taken to
		/// denote a directory, and the <code>child</code> pathname string is taken
		/// to denote either a directory or a file.  If the <code>child</code>
		/// pathname string is absolute then it is converted into a relative
		/// pathname in a system-dependent way.  If <code>parent</code> is the empty
		/// abstract pathname then the new <code>File</code> instance is created by
		/// converting <code>child</code> into an abstract pathname and resolving
		/// the result against a system-dependent default directory.  Otherwise each
		/// pathname string is converted into an abstract pathname and the child
		/// abstract pathname is resolved against the parent.
		/// 
		/// </para>
		/// </summary>
		/// <param name="parent">  The parent abstract pathname </param>
		/// <param name="child">   The child pathname string </param>
		/// <exception cref="NullPointerException">
		///          If <code>child</code> is <code>null</code> </exception>
		public File(File parent, String child)
		{
			if (child == null)
			{
				throw new NullPointerException();
			}
			if (parent != null)
			{
				if (parent.Path_Renamed.Equals(""))
				{
					this.Path_Renamed = Fs.Resolve(Fs.DefaultParent, Fs.Normalize(child));
				}
				else
				{
					this.Path_Renamed = Fs.Resolve(parent.Path_Renamed, Fs.Normalize(child));
				}
			}
			else
			{
				this.Path_Renamed = Fs.Normalize(child);
			}
			this.PrefixLength_Renamed = Fs.PrefixLength(this.Path_Renamed);
		}

		/// <summary>
		/// Creates a new <tt>File</tt> instance by converting the given
		/// <tt>file:</tt> URI into an abstract pathname.
		/// 
		/// <para> The exact form of a <tt>file:</tt> URI is system-dependent, hence
		/// the transformation performed by this constructor is also
		/// system-dependent.
		/// 
		/// </para>
		/// <para> For a given abstract pathname <i>f</i> it is guaranteed that
		/// 
		/// <blockquote><tt>
		/// new File(</tt><i>&nbsp;f</i><tt>.<seealso cref="#toURI() toURI"/>()).equals(</tt><i>&nbsp;f</i><tt>.<seealso cref="#getAbsoluteFile() getAbsoluteFile"/>())
		/// </tt></blockquote>
		/// 
		/// so long as the original abstract pathname, the URI, and the new abstract
		/// pathname are all created in (possibly different invocations of) the same
		/// Java virtual machine.  This relationship typically does not hold,
		/// however, when a <tt>file:</tt> URI that is created in a virtual machine
		/// on one operating system is converted into an abstract pathname in a
		/// virtual machine on a different operating system.
		/// 
		/// </para>
		/// </summary>
		/// <param name="uri">
		///         An absolute, hierarchical URI with a scheme equal to
		///         <tt>"file"</tt>, a non-empty path component, and undefined
		///         authority, query, and fragment components
		/// </param>
		/// <exception cref="NullPointerException">
		///          If <tt>uri</tt> is <tt>null</tt>
		/// </exception>
		/// <exception cref="IllegalArgumentException">
		///          If the preconditions on the parameter do not hold
		/// </exception>
		/// <seealso cref= #toURI() </seealso>
		/// <seealso cref= java.net.URI
		/// @since 1.4 </seealso>
		public File(URI uri)
		{

			// Check our many preconditions
			if (!uri.Absolute)
			{
				throw new IllegalArgumentException("URI is not absolute");
			}
			if (uri.Opaque)
			{
				throw new IllegalArgumentException("URI is not hierarchical");
			}
			String scheme = uri.Scheme;
			if ((scheme == null) || !scheme.EqualsIgnoreCase("file"))
			{
				throw new IllegalArgumentException("URI scheme is not \"file\"");
			}
			if (uri.Authority != null)
			{
				throw new IllegalArgumentException("URI has an authority component");
			}
			if (uri.Fragment != null)
			{
				throw new IllegalArgumentException("URI has a fragment component");
			}
			if (uri.Query != null)
			{
				throw new IllegalArgumentException("URI has a query component");
			}
			String p = uri.Path;
			if (p.Equals(""))
			{
				throw new IllegalArgumentException("URI path component is empty");
			}

			// Okay, now initialize
			p = Fs.FromURIPath(p);
			if (System.IO.Path.DirectorySeparatorChar != '/')
			{
				p = p.Replace('/', System.IO.Path.DirectorySeparatorChar);
			}
			this.Path_Renamed = Fs.Normalize(p);
			this.PrefixLength_Renamed = Fs.PrefixLength(this.Path_Renamed);
		}


		/* -- Path-component accessors -- */

		/// <summary>
		/// Returns the name of the file or directory denoted by this abstract
		/// pathname.  This is just the last name in the pathname's name
		/// sequence.  If the pathname's name sequence is empty, then the empty
		/// string is returned.
		/// </summary>
		/// <returns>  The name of the file or directory denoted by this abstract
		///          pathname, or the empty string if this pathname's name sequence
		///          is empty </returns>
		public virtual String Name
		{
			get
			{
				int index = Path_Renamed.LastIndexOf(SeparatorChar);
				if (index < PrefixLength_Renamed)
				{
					return Path_Renamed.Substring(PrefixLength_Renamed);
				}
				return Path_Renamed.Substring(index + 1);
			}
		}

		/// <summary>
		/// Returns the pathname string of this abstract pathname's parent, or
		/// <code>null</code> if this pathname does not name a parent directory.
		/// 
		/// <para> The <em>parent</em> of an abstract pathname consists of the
		/// pathname's prefix, if any, and each name in the pathname's name
		/// sequence except for the last.  If the name sequence is empty then
		/// the pathname does not name a parent directory.
		/// 
		/// </para>
		/// </summary>
		/// <returns>  The pathname string of the parent directory named by this
		///          abstract pathname, or <code>null</code> if this pathname
		///          does not name a parent </returns>
		public virtual String Parent
		{
			get
			{
				int index = Path_Renamed.LastIndexOf(SeparatorChar);
				if (index < PrefixLength_Renamed)
				{
					if ((PrefixLength_Renamed > 0) && (Path_Renamed.Length() > PrefixLength_Renamed))
					{
						return Path_Renamed.Substring(0, PrefixLength_Renamed);
					}
					return null;
				}
				return Path_Renamed.Substring(0, index);
			}
		}

		/// <summary>
		/// Returns the abstract pathname of this abstract pathname's parent,
		/// or <code>null</code> if this pathname does not name a parent
		/// directory.
		/// 
		/// <para> The <em>parent</em> of an abstract pathname consists of the
		/// pathname's prefix, if any, and each name in the pathname's name
		/// sequence except for the last.  If the name sequence is empty then
		/// the pathname does not name a parent directory.
		/// 
		/// </para>
		/// </summary>
		/// <returns>  The abstract pathname of the parent directory named by this
		///          abstract pathname, or <code>null</code> if this pathname
		///          does not name a parent
		/// 
		/// @since 1.2 </returns>
		public virtual File ParentFile
		{
			get
			{
				String p = this.Parent;
				if (p == null)
				{
					return null;
				}
				return new File(p, this.PrefixLength_Renamed);
			}
		}

		/// <summary>
		/// Converts this abstract pathname into a pathname string.  The resulting
		/// string uses the <seealso cref="#separator default name-separator character"/> to
		/// separate the names in the name sequence.
		/// </summary>
		/// <returns>  The string form of this abstract pathname </returns>
		public virtual String Path
		{
			get
			{
				return Path_Renamed;
			}
		}


		/* -- Path operations -- */

		/// <summary>
		/// Tests whether this abstract pathname is absolute.  The definition of
		/// absolute pathname is system dependent.  On UNIX systems, a pathname is
		/// absolute if its prefix is <code>"/"</code>.  On Microsoft Windows systems, a
		/// pathname is absolute if its prefix is a drive specifier followed by
		/// <code>"\\"</code>, or if its prefix is <code>"\\\\"</code>.
		/// </summary>
		/// <returns>  <code>true</code> if this abstract pathname is absolute,
		///          <code>false</code> otherwise </returns>
		public virtual bool Absolute
		{
			get
			{
				return Fs.IsAbsolute(this);
			}
		}

		/// <summary>
		/// Returns the absolute pathname string of this abstract pathname.
		/// 
		/// <para> If this abstract pathname is already absolute, then the pathname
		/// string is simply returned as if by the <code><seealso cref="#getPath"/></code>
		/// method.  If this abstract pathname is the empty abstract pathname then
		/// the pathname string of the current user directory, which is named by the
		/// system property <code>user.dir</code>, is returned.  Otherwise this
		/// pathname is resolved in a system-dependent way.  On UNIX systems, a
		/// relative pathname is made absolute by resolving it against the current
		/// user directory.  On Microsoft Windows systems, a relative pathname is made absolute
		/// by resolving it against the current directory of the drive named by the
		/// pathname, if any; if not, it is resolved against the current user
		/// directory.
		/// 
		/// </para>
		/// </summary>
		/// <returns>  The absolute pathname string denoting the same file or
		///          directory as this abstract pathname
		/// </returns>
		/// <exception cref="SecurityException">
		///          If a required system property value cannot be accessed.
		/// </exception>
		/// <seealso cref=     java.io.File#isAbsolute() </seealso>
		public virtual String AbsolutePath
		{
			get
			{
				return Fs.Resolve(this);
			}
		}

		/// <summary>
		/// Returns the absolute form of this abstract pathname.  Equivalent to
		/// <code>new&nbsp;File(this.<seealso cref="#getAbsolutePath"/>)</code>.
		/// </summary>
		/// <returns>  The absolute abstract pathname denoting the same file or
		///          directory as this abstract pathname
		/// </returns>
		/// <exception cref="SecurityException">
		///          If a required system property value cannot be accessed.
		/// 
		/// @since 1.2 </exception>
		public virtual File AbsoluteFile
		{
			get
			{
				String absPath = AbsolutePath;
				return new File(absPath, Fs.PrefixLength(absPath));
			}
		}

		/// <summary>
		/// Returns the canonical pathname string of this abstract pathname.
		/// 
		/// <para> A canonical pathname is both absolute and unique.  The precise
		/// definition of canonical form is system-dependent.  This method first
		/// converts this pathname to absolute form if necessary, as if by invoking the
		/// <seealso cref="#getAbsolutePath"/> method, and then maps it to its unique form in a
		/// system-dependent way.  This typically involves removing redundant names
		/// such as <tt>"."</tt> and <tt>".."</tt> from the pathname, resolving
		/// symbolic links (on UNIX platforms), and converting drive letters to a
		/// standard case (on Microsoft Windows platforms).
		/// 
		/// </para>
		/// <para> Every pathname that denotes an existing file or directory has a
		/// unique canonical form.  Every pathname that denotes a nonexistent file
		/// or directory also has a unique canonical form.  The canonical form of
		/// the pathname of a nonexistent file or directory may be different from
		/// the canonical form of the same pathname after the file or directory is
		/// created.  Similarly, the canonical form of the pathname of an existing
		/// file or directory may be different from the canonical form of the same
		/// pathname after the file or directory is deleted.
		/// 
		/// </para>
		/// </summary>
		/// <returns>  The canonical pathname string denoting the same file or
		///          directory as this abstract pathname
		/// </returns>
		/// <exception cref="IOException">
		///          If an I/O error occurs, which is possible because the
		///          construction of the canonical pathname may require
		///          filesystem queries
		/// </exception>
		/// <exception cref="SecurityException">
		///          If a required system property value cannot be accessed, or
		///          if a security manager exists and its <code>{@link
		///          java.lang.SecurityManager#checkRead}</code> method denies
		///          read access to the file
		/// 
		/// @since   JDK1.1 </exception>
		/// <seealso cref=     Path#toRealPath </seealso>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public String getCanonicalPath() throws IOException
		public virtual String CanonicalPath
		{
			get
			{
				if (Invalid)
				{
					throw new IOException("Invalid file path");
				}
				return Fs.Canonicalize(Fs.Resolve(this));
			}
		}

		/// <summary>
		/// Returns the canonical form of this abstract pathname.  Equivalent to
		/// <code>new&nbsp;File(this.<seealso cref="#getCanonicalPath"/>)</code>.
		/// </summary>
		/// <returns>  The canonical pathname string denoting the same file or
		///          directory as this abstract pathname
		/// </returns>
		/// <exception cref="IOException">
		///          If an I/O error occurs, which is possible because the
		///          construction of the canonical pathname may require
		///          filesystem queries
		/// </exception>
		/// <exception cref="SecurityException">
		///          If a required system property value cannot be accessed, or
		///          if a security manager exists and its <code>{@link
		///          java.lang.SecurityManager#checkRead}</code> method denies
		///          read access to the file
		/// 
		/// @since 1.2 </exception>
		/// <seealso cref=     Path#toRealPath </seealso>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public File getCanonicalFile() throws IOException
		public virtual File CanonicalFile
		{
			get
			{
				String canonPath = CanonicalPath;
				return new File(canonPath, Fs.PrefixLength(canonPath));
			}
		}

		private static String Slashify(String path, bool isDirectory)
		{
			String p = path;
			if (System.IO.Path.DirectorySeparatorChar != '/')
			{
				p = p.Replace(System.IO.Path.DirectorySeparatorChar, '/');
			}
			if (!p.StartsWith("/"))
			{
				p = "/" + p;
			}
			if (!p.EndsWith("/") && isDirectory)
			{
				p = p + "/";
			}
			return p;
		}

		/// <summary>
		/// Converts this abstract pathname into a <code>file:</code> URL.  The
		/// exact form of the URL is system-dependent.  If it can be determined that
		/// the file denoted by this abstract pathname is a directory, then the
		/// resulting URL will end with a slash.
		/// </summary>
		/// <returns>  A URL object representing the equivalent file URL
		/// </returns>
		/// <exception cref="MalformedURLException">
		///          If the path cannot be parsed as a URL
		/// </exception>
		/// <seealso cref=     #toURI() </seealso>
		/// <seealso cref=     java.net.URI </seealso>
		/// <seealso cref=     java.net.URI#toURL() </seealso>
		/// <seealso cref=     java.net.URL
		/// @since   1.2
		/// </seealso>
		/// @deprecated This method does not automatically escape characters that
		/// are illegal in URLs.  It is recommended that new code convert an
		/// abstract pathname into a URL by first converting it into a URI, via the
		/// <seealso cref="#toURI() toURI"/> method, and then converting the URI into a URL
		/// via the <seealso cref="java.net.URI#toURL() URI.toURL"/> method. 
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: @Deprecated("This method does not automatically escape characters that") public java.net.URL toURL() throws java.net.MalformedURLException
		[Obsolete("This method does not automatically escape characters that")]
		public virtual URL ToURL()
		{
			if (Invalid)
			{
				throw new MalformedURLException("Invalid file path");
			}
			return new URL("file", "", Slashify(AbsolutePath, Directory));
		}

		/// <summary>
		/// Constructs a <tt>file:</tt> URI that represents this abstract pathname.
		/// 
		/// <para> The exact form of the URI is system-dependent.  If it can be
		/// determined that the file denoted by this abstract pathname is a
		/// directory, then the resulting URI will end with a slash.
		/// 
		/// </para>
		/// <para> For a given abstract pathname <i>f</i>, it is guaranteed that
		/// 
		/// <blockquote><tt>
		/// new <seealso cref="#File(java.net.URI) File"/>(</tt><i>&nbsp;f</i><tt>.toURI()).equals(</tt><i>&nbsp;f</i><tt>.<seealso cref="#getAbsoluteFile() getAbsoluteFile"/>())
		/// </tt></blockquote>
		/// 
		/// so long as the original abstract pathname, the URI, and the new abstract
		/// pathname are all created in (possibly different invocations of) the same
		/// Java virtual machine.  Due to the system-dependent nature of abstract
		/// pathnames, however, this relationship typically does not hold when a
		/// <tt>file:</tt> URI that is created in a virtual machine on one operating
		/// system is converted into an abstract pathname in a virtual machine on a
		/// different operating system.
		/// 
		/// </para>
		/// <para> Note that when this abstract pathname represents a UNC pathname then
		/// all components of the UNC (including the server name component) are encoded
		/// in the {@code URI} path. The authority component is undefined, meaning
		/// that it is represented as {@code null}. The <seealso cref="Path"/> class defines the
		/// <seealso cref="Path#toUri toUri"/> method to encode the server name in the authority
		/// component of the resulting {@code URI}. The <seealso cref="#toPath toPath"/> method
		/// may be used to obtain a {@code Path} representing this abstract pathname.
		/// 
		/// </para>
		/// </summary>
		/// <returns>  An absolute, hierarchical URI with a scheme equal to
		///          <tt>"file"</tt>, a path representing this abstract pathname,
		///          and undefined authority, query, and fragment components </returns>
		/// <exception cref="SecurityException"> If a required system property value cannot
		/// be accessed.
		/// </exception>
		/// <seealso cref= #File(java.net.URI) </seealso>
		/// <seealso cref= java.net.URI </seealso>
		/// <seealso cref= java.net.URI#toURL()
		/// @since 1.4 </seealso>
		public virtual URI ToURI()
		{
			try
			{
				File f = AbsoluteFile;
				String sp = Slashify(f.Path, f.Directory);
				if (sp.StartsWith("//"))
				{
					sp = "//" + sp;
				}
				return new URI("file", null, sp, null);
			}
			catch (URISyntaxException x)
			{
				throw new Error(x); // Can't happen
			}
		}


		/* -- Attribute accessors -- */

		/// <summary>
		/// Tests whether the application can read the file denoted by this
		/// abstract pathname. On some platforms it may be possible to start the
		/// Java virtual machine with special privileges that allow it to read
		/// files that are marked as unreadable. Consequently this method may return
		/// {@code true} even though the file does not have read permissions.
		/// </summary>
		/// <returns>  <code>true</code> if and only if the file specified by this
		///          abstract pathname exists <em>and</em> can be read by the
		///          application; <code>false</code> otherwise
		/// </returns>
		/// <exception cref="SecurityException">
		///          If a security manager exists and its <code>{@link
		///          java.lang.SecurityManager#checkRead(java.lang.String)}</code>
		///          method denies read access to the file </exception>
		public virtual bool CanRead()
		{
			SecurityManager security = System.SecurityManager;
			if (security != null)
			{
				security.CheckRead(Path_Renamed);
			}
			if (Invalid)
			{
				return false;
			}
			return Fs.CheckAccess(this, FileSystem.ACCESS_READ);
		}

		/// <summary>
		/// Tests whether the application can modify the file denoted by this
		/// abstract pathname. On some platforms it may be possible to start the
		/// Java virtual machine with special privileges that allow it to modify
		/// files that are marked read-only. Consequently this method may return
		/// {@code true} even though the file is marked read-only.
		/// </summary>
		/// <returns>  <code>true</code> if and only if the file system actually
		///          contains a file denoted by this abstract pathname <em>and</em>
		///          the application is allowed to write to the file;
		///          <code>false</code> otherwise.
		/// </returns>
		/// <exception cref="SecurityException">
		///          If a security manager exists and its <code>{@link
		///          java.lang.SecurityManager#checkWrite(java.lang.String)}</code>
		///          method denies write access to the file </exception>
		public virtual bool CanWrite()
		{
			SecurityManager security = System.SecurityManager;
			if (security != null)
			{
				security.CheckWrite(Path_Renamed);
			}
			if (Invalid)
			{
				return false;
			}
			return Fs.CheckAccess(this, FileSystem.ACCESS_WRITE);
		}

		/// <summary>
		/// Tests whether the file or directory denoted by this abstract pathname
		/// exists.
		/// </summary>
		/// <returns>  <code>true</code> if and only if the file or directory denoted
		///          by this abstract pathname exists; <code>false</code> otherwise
		/// </returns>
		/// <exception cref="SecurityException">
		///          If a security manager exists and its <code>{@link
		///          java.lang.SecurityManager#checkRead(java.lang.String)}</code>
		///          method denies read access to the file or directory </exception>
		public virtual bool Exists()
		{
			SecurityManager security = System.SecurityManager;
			if (security != null)
			{
				security.CheckRead(Path_Renamed);
			}
			if (Invalid)
			{
				return false;
			}
			return ((Fs.GetBooleanAttributes(this) & FileSystem.BA_EXISTS) != 0);
		}

		/// <summary>
		/// Tests whether the file denoted by this abstract pathname is a
		/// directory.
		/// 
		/// <para> Where it is required to distinguish an I/O exception from the case
		/// that the file is not a directory, or where several attributes of the
		/// same file are required at the same time, then the {@link
		/// java.nio.file.Files#readAttributes(Path,Class,LinkOption[])
		/// Files.readAttributes} method may be used.
		/// 
		/// </para>
		/// </summary>
		/// <returns> <code>true</code> if and only if the file denoted by this
		///          abstract pathname exists <em>and</em> is a directory;
		///          <code>false</code> otherwise
		/// </returns>
		/// <exception cref="SecurityException">
		///          If a security manager exists and its <code>{@link
		///          java.lang.SecurityManager#checkRead(java.lang.String)}</code>
		///          method denies read access to the file </exception>
		public virtual bool Directory
		{
			get
			{
				SecurityManager security = System.SecurityManager;
				if (security != null)
				{
					security.CheckRead(Path_Renamed);
				}
				if (Invalid)
				{
					return false;
				}
				return ((Fs.GetBooleanAttributes(this) & FileSystem.BA_DIRECTORY) != 0);
			}
		}

		/// <summary>
		/// Tests whether the file denoted by this abstract pathname is a normal
		/// file.  A file is <em>normal</em> if it is not a directory and, in
		/// addition, satisfies other system-dependent criteria.  Any non-directory
		/// file created by a Java application is guaranteed to be a normal file.
		/// 
		/// <para> Where it is required to distinguish an I/O exception from the case
		/// that the file is not a normal file, or where several attributes of the
		/// same file are required at the same time, then the {@link
		/// java.nio.file.Files#readAttributes(Path,Class,LinkOption[])
		/// Files.readAttributes} method may be used.
		/// 
		/// </para>
		/// </summary>
		/// <returns>  <code>true</code> if and only if the file denoted by this
		///          abstract pathname exists <em>and</em> is a normal file;
		///          <code>false</code> otherwise
		/// </returns>
		/// <exception cref="SecurityException">
		///          If a security manager exists and its <code>{@link
		///          java.lang.SecurityManager#checkRead(java.lang.String)}</code>
		///          method denies read access to the file </exception>
		public virtual bool File
		{
			get
			{
				SecurityManager security = System.SecurityManager;
				if (security != null)
				{
					security.CheckRead(Path_Renamed);
				}
				if (Invalid)
				{
					return false;
				}
				return ((Fs.GetBooleanAttributes(this) & FileSystem.BA_REGULAR) != 0);
			}
		}

		/// <summary>
		/// Tests whether the file named by this abstract pathname is a hidden
		/// file.  The exact definition of <em>hidden</em> is system-dependent.  On
		/// UNIX systems, a file is considered to be hidden if its name begins with
		/// a period character (<code>'.'</code>).  On Microsoft Windows systems, a file is
		/// considered to be hidden if it has been marked as such in the filesystem.
		/// </summary>
		/// <returns>  <code>true</code> if and only if the file denoted by this
		///          abstract pathname is hidden according to the conventions of the
		///          underlying platform
		/// </returns>
		/// <exception cref="SecurityException">
		///          If a security manager exists and its <code>{@link
		///          java.lang.SecurityManager#checkRead(java.lang.String)}</code>
		///          method denies read access to the file
		/// 
		/// @since 1.2 </exception>
		public virtual bool Hidden
		{
			get
			{
				SecurityManager security = System.SecurityManager;
				if (security != null)
				{
					security.CheckRead(Path_Renamed);
				}
				if (Invalid)
				{
					return false;
				}
				return ((Fs.GetBooleanAttributes(this) & FileSystem.BA_HIDDEN) != 0);
			}
		}

		/// <summary>
		/// Returns the time that the file denoted by this abstract pathname was
		/// last modified.
		/// 
		/// <para> Where it is required to distinguish an I/O exception from the case
		/// where {@code 0L} is returned, or where several attributes of the
		/// same file are required at the same time, or where the time of last
		/// access or the creation time are required, then the {@link
		/// java.nio.file.Files#readAttributes(Path,Class,LinkOption[])
		/// Files.readAttributes} method may be used.
		/// 
		/// </para>
		/// </summary>
		/// <returns>  A <code>long</code> value representing the time the file was
		///          last modified, measured in milliseconds since the epoch
		///          (00:00:00 GMT, January 1, 1970), or <code>0L</code> if the
		///          file does not exist or if an I/O error occurs
		/// </returns>
		/// <exception cref="SecurityException">
		///          If a security manager exists and its <code>{@link
		///          java.lang.SecurityManager#checkRead(java.lang.String)}</code>
		///          method denies read access to the file </exception>
		public virtual long LastModified()
		{
			SecurityManager security = System.SecurityManager;
			if (security != null)
			{
				security.CheckRead(Path_Renamed);
			}
			if (Invalid)
			{
				return 0L;
			}
			return Fs.GetLastModifiedTime(this);
		}

		/// <summary>
		/// Returns the length of the file denoted by this abstract pathname.
		/// The return value is unspecified if this pathname denotes a directory.
		/// 
		/// <para> Where it is required to distinguish an I/O exception from the case
		/// that {@code 0L} is returned, or where several attributes of the same file
		/// are required at the same time, then the {@link
		/// java.nio.file.Files#readAttributes(Path,Class,LinkOption[])
		/// Files.readAttributes} method may be used.
		/// 
		/// </para>
		/// </summary>
		/// <returns>  The length, in bytes, of the file denoted by this abstract
		///          pathname, or <code>0L</code> if the file does not exist.  Some
		///          operating systems may return <code>0L</code> for pathnames
		///          denoting system-dependent entities such as devices or pipes.
		/// </returns>
		/// <exception cref="SecurityException">
		///          If a security manager exists and its <code>{@link
		///          java.lang.SecurityManager#checkRead(java.lang.String)}</code>
		///          method denies read access to the file </exception>
		public virtual long Length()
		{
			SecurityManager security = System.SecurityManager;
			if (security != null)
			{
				security.CheckRead(Path_Renamed);
			}
			if (Invalid)
			{
				return 0L;
			}
			return Fs.GetLength(this);
		}


		/* -- File operations -- */

		/// <summary>
		/// Atomically creates a new, empty file named by this abstract pathname if
		/// and only if a file with this name does not yet exist.  The check for the
		/// existence of the file and the creation of the file if it does not exist
		/// are a single operation that is atomic with respect to all other
		/// filesystem activities that might affect the file.
		/// <P>
		/// Note: this method should <i>not</i> be used for file-locking, as
		/// the resulting protocol cannot be made to work reliably. The
		/// <seealso cref="java.nio.channels.FileLock FileLock"/>
		/// facility should be used instead.
		/// </summary>
		/// <returns>  <code>true</code> if the named file does not exist and was
		///          successfully created; <code>false</code> if the named file
		///          already exists
		/// </returns>
		/// <exception cref="IOException">
		///          If an I/O error occurred
		/// </exception>
		/// <exception cref="SecurityException">
		///          If a security manager exists and its <code>{@link
		///          java.lang.SecurityManager#checkWrite(java.lang.String)}</code>
		///          method denies write access to the file
		/// 
		/// @since 1.2 </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public boolean createNewFile() throws IOException
		public virtual bool CreateNewFile()
		{
			SecurityManager security = System.SecurityManager;
			if (security != null)
			{
				security.CheckWrite(Path_Renamed);
			}
			if (Invalid)
			{
				throw new IOException("Invalid file path");
			}
			return Fs.CreateFileExclusively(Path_Renamed);
		}

		/// <summary>
		/// Deletes the file or directory denoted by this abstract pathname.  If
		/// this pathname denotes a directory, then the directory must be empty in
		/// order to be deleted.
		/// 
		/// <para> Note that the <seealso cref="java.nio.file.Files"/> class defines the {@link
		/// java.nio.file.Files#delete(Path) delete} method to throw an <seealso cref="IOException"/>
		/// when a file cannot be deleted. This is useful for error reporting and to
		/// diagnose why a file cannot be deleted.
		/// 
		/// </para>
		/// </summary>
		/// <returns>  <code>true</code> if and only if the file or directory is
		///          successfully deleted; <code>false</code> otherwise
		/// </returns>
		/// <exception cref="SecurityException">
		///          If a security manager exists and its <code>{@link
		///          java.lang.SecurityManager#checkDelete}</code> method denies
		///          delete access to the file </exception>
		public virtual bool Delete()
		{
			SecurityManager security = System.SecurityManager;
			if (security != null)
			{
				security.CheckDelete(Path_Renamed);
			}
			if (Invalid)
			{
				return false;
			}
			return Fs.Delete(this);
		}

		/// <summary>
		/// Requests that the file or directory denoted by this abstract
		/// pathname be deleted when the virtual machine terminates.
		/// Files (or directories) are deleted in the reverse order that
		/// they are registered. Invoking this method to delete a file or
		/// directory that is already registered for deletion has no effect.
		/// Deletion will be attempted only for normal termination of the
		/// virtual machine, as defined by the Java Language Specification.
		/// 
		/// <para> Once deletion has been requested, it is not possible to cancel the
		/// request.  This method should therefore be used with care.
		/// 
		/// <P>
		/// Note: this method should <i>not</i> be used for file-locking, as
		/// the resulting protocol cannot be made to work reliably. The
		/// <seealso cref="java.nio.channels.FileLock FileLock"/>
		/// facility should be used instead.
		/// 
		/// </para>
		/// </summary>
		/// <exception cref="SecurityException">
		///          If a security manager exists and its <code>{@link
		///          java.lang.SecurityManager#checkDelete}</code> method denies
		///          delete access to the file
		/// </exception>
		/// <seealso cref= #delete
		/// 
		/// @since 1.2 </seealso>
		public virtual void DeleteOnExit()
		{
			SecurityManager security = System.SecurityManager;
			if (security != null)
			{
				security.CheckDelete(Path_Renamed);
			}
			if (Invalid)
			{
				return;
			}
			DeleteOnExitHook.Add(Path_Renamed);
		}

		/// <summary>
		/// Returns an array of strings naming the files and directories in the
		/// directory denoted by this abstract pathname.
		/// 
		/// <para> If this abstract pathname does not denote a directory, then this
		/// method returns {@code null}.  Otherwise an array of strings is
		/// returned, one for each file or directory in the directory.  Names
		/// denoting the directory itself and the directory's parent directory are
		/// not included in the result.  Each string is a file name rather than a
		/// complete path.
		/// 
		/// </para>
		/// <para> There is no guarantee that the name strings in the resulting array
		/// will appear in any specific order; they are not, in particular,
		/// guaranteed to appear in alphabetical order.
		/// 
		/// </para>
		/// <para> Note that the <seealso cref="java.nio.file.Files"/> class defines the {@link
		/// java.nio.file.Files#newDirectoryStream(Path) newDirectoryStream} method to
		/// open a directory and iterate over the names of the files in the directory.
		/// This may use less resources when working with very large directories, and
		/// may be more responsive when working with remote directories.
		/// 
		/// </para>
		/// </summary>
		/// <returns>  An array of strings naming the files and directories in the
		///          directory denoted by this abstract pathname.  The array will be
		///          empty if the directory is empty.  Returns {@code null} if
		///          this abstract pathname does not denote a directory, or if an
		///          I/O error occurs.
		/// </returns>
		/// <exception cref="SecurityException">
		///          If a security manager exists and its {@link
		///          SecurityManager#checkRead(String)} method denies read access to
		///          the directory </exception>
		public virtual String[] List()
		{
			SecurityManager security = System.SecurityManager;
			if (security != null)
			{
				security.CheckRead(Path_Renamed);
			}
			if (Invalid)
			{
				return null;
			}
			return Fs.List(this);
		}

		/// <summary>
		/// Returns an array of strings naming the files and directories in the
		/// directory denoted by this abstract pathname that satisfy the specified
		/// filter.  The behavior of this method is the same as that of the
		/// <seealso cref="#list()"/> method, except that the strings in the returned array
		/// must satisfy the filter.  If the given {@code filter} is {@code null}
		/// then all names are accepted.  Otherwise, a name satisfies the filter if
		/// and only if the value {@code true} results when the {@link
		/// FilenameFilter#accept FilenameFilter.accept(File,&nbsp;String)} method
		/// of the filter is invoked on this abstract pathname and the name of a
		/// file or directory in the directory that it denotes.
		/// </summary>
		/// <param name="filter">
		///         A filename filter
		/// </param>
		/// <returns>  An array of strings naming the files and directories in the
		///          directory denoted by this abstract pathname that were accepted
		///          by the given {@code filter}.  The array will be empty if the
		///          directory is empty or if no names were accepted by the filter.
		///          Returns {@code null} if this abstract pathname does not denote
		///          a directory, or if an I/O error occurs.
		/// </returns>
		/// <exception cref="SecurityException">
		///          If a security manager exists and its {@link
		///          SecurityManager#checkRead(String)} method denies read access to
		///          the directory
		/// </exception>
		/// <seealso cref= java.nio.file.Files#newDirectoryStream(Path,String) </seealso>
		public virtual String[] List(FilenameFilter filter)
		{
			String[] names = List();
			if ((names == null) || (filter == null))
			{
				return names;
			}
			IList<String> v = new List<String>();
			for (int i = 0 ; i < names.Length ; i++)
			{
				if (filter.Accept(this, names[i]))
				{
					v.Add(names[i]);
				}
			}
			return v.ToArray();
		}

		/// <summary>
		/// Returns an array of abstract pathnames denoting the files in the
		/// directory denoted by this abstract pathname.
		/// 
		/// <para> If this abstract pathname does not denote a directory, then this
		/// method returns {@code null}.  Otherwise an array of {@code File} objects
		/// is returned, one for each file or directory in the directory.  Pathnames
		/// denoting the directory itself and the directory's parent directory are
		/// not included in the result.  Each resulting abstract pathname is
		/// constructed from this abstract pathname using the {@link #File(File,
		/// String) File(File,&nbsp;String)} constructor.  Therefore if this
		/// pathname is absolute then each resulting pathname is absolute; if this
		/// pathname is relative then each resulting pathname will be relative to
		/// the same directory.
		/// 
		/// </para>
		/// <para> There is no guarantee that the name strings in the resulting array
		/// will appear in any specific order; they are not, in particular,
		/// guaranteed to appear in alphabetical order.
		/// 
		/// </para>
		/// <para> Note that the <seealso cref="java.nio.file.Files"/> class defines the {@link
		/// java.nio.file.Files#newDirectoryStream(Path) newDirectoryStream} method
		/// to open a directory and iterate over the names of the files in the
		/// directory. This may use less resources when working with very large
		/// directories.
		/// 
		/// </para>
		/// </summary>
		/// <returns>  An array of abstract pathnames denoting the files and
		///          directories in the directory denoted by this abstract pathname.
		///          The array will be empty if the directory is empty.  Returns
		///          {@code null} if this abstract pathname does not denote a
		///          directory, or if an I/O error occurs.
		/// </returns>
		/// <exception cref="SecurityException">
		///          If a security manager exists and its {@link
		///          SecurityManager#checkRead(String)} method denies read access to
		///          the directory
		/// 
		/// @since  1.2 </exception>
		public virtual File[] ListFiles()
		{
			String[] ss = List();
			if (ss == null)
			{
				return null;
			}
			int n = ss.Length;
			File[] fs = new File[n];
			for (int i = 0; i < n; i++)
			{
				fs[i] = new File(ss[i], this);
			}
			return fs;
		}

		/// <summary>
		/// Returns an array of abstract pathnames denoting the files and
		/// directories in the directory denoted by this abstract pathname that
		/// satisfy the specified filter.  The behavior of this method is the same
		/// as that of the <seealso cref="#listFiles()"/> method, except that the pathnames in
		/// the returned array must satisfy the filter.  If the given {@code filter}
		/// is {@code null} then all pathnames are accepted.  Otherwise, a pathname
		/// satisfies the filter if and only if the value {@code true} results when
		/// the {@link FilenameFilter#accept
		/// FilenameFilter.accept(File,&nbsp;String)} method of the filter is
		/// invoked on this abstract pathname and the name of a file or directory in
		/// the directory that it denotes.
		/// </summary>
		/// <param name="filter">
		///         A filename filter
		/// </param>
		/// <returns>  An array of abstract pathnames denoting the files and
		///          directories in the directory denoted by this abstract pathname.
		///          The array will be empty if the directory is empty.  Returns
		///          {@code null} if this abstract pathname does not denote a
		///          directory, or if an I/O error occurs.
		/// </returns>
		/// <exception cref="SecurityException">
		///          If a security manager exists and its {@link
		///          SecurityManager#checkRead(String)} method denies read access to
		///          the directory
		/// 
		/// @since  1.2 </exception>
		/// <seealso cref= java.nio.file.Files#newDirectoryStream(Path,String) </seealso>
		public virtual File[] ListFiles(FilenameFilter filter)
		{
			String[] ss = List();
			if (ss == null)
			{
				return null;
			}
			List<File> files = new List<File>();
			foreach (String s in ss)
			{
				if ((filter == null) || filter.Accept(this, s))
				{
					files.Add(new File(s, this));
				}
			}
			return files.ToArray();
		}

		/// <summary>
		/// Returns an array of abstract pathnames denoting the files and
		/// directories in the directory denoted by this abstract pathname that
		/// satisfy the specified filter.  The behavior of this method is the same
		/// as that of the <seealso cref="#listFiles()"/> method, except that the pathnames in
		/// the returned array must satisfy the filter.  If the given {@code filter}
		/// is {@code null} then all pathnames are accepted.  Otherwise, a pathname
		/// satisfies the filter if and only if the value {@code true} results when
		/// the <seealso cref="FileFilter#accept FileFilter.accept(File)"/> method of the
		/// filter is invoked on the pathname.
		/// </summary>
		/// <param name="filter">
		///         A file filter
		/// </param>
		/// <returns>  An array of abstract pathnames denoting the files and
		///          directories in the directory denoted by this abstract pathname.
		///          The array will be empty if the directory is empty.  Returns
		///          {@code null} if this abstract pathname does not denote a
		///          directory, or if an I/O error occurs.
		/// </returns>
		/// <exception cref="SecurityException">
		///          If a security manager exists and its {@link
		///          SecurityManager#checkRead(String)} method denies read access to
		///          the directory
		/// 
		/// @since  1.2 </exception>
		/// <seealso cref= java.nio.file.Files#newDirectoryStream(Path,java.nio.file.DirectoryStream.Filter) </seealso>
		public virtual File[] ListFiles(FileFilter filter)
		{
			String[] ss = List();
			if (ss == null)
			{
				return null;
			}
			List<File> files = new List<File>();
			foreach (String s in ss)
			{
				File f = new File(s, this);
				if ((filter == null) || filter.Accept(f))
				{
					files.Add(f);
				}
			}
			return files.ToArray();
		}

		/// <summary>
		/// Creates the directory named by this abstract pathname.
		/// </summary>
		/// <returns>  <code>true</code> if and only if the directory was
		///          created; <code>false</code> otherwise
		/// </returns>
		/// <exception cref="SecurityException">
		///          If a security manager exists and its <code>{@link
		///          java.lang.SecurityManager#checkWrite(java.lang.String)}</code>
		///          method does not permit the named directory to be created </exception>
		public virtual bool Mkdir()
		{
			SecurityManager security = System.SecurityManager;
			if (security != null)
			{
				security.CheckWrite(Path_Renamed);
			}
			if (Invalid)
			{
				return false;
			}
			return Fs.CreateDirectory(this);
		}

		/// <summary>
		/// Creates the directory named by this abstract pathname, including any
		/// necessary but nonexistent parent directories.  Note that if this
		/// operation fails it may have succeeded in creating some of the necessary
		/// parent directories.
		/// </summary>
		/// <returns>  <code>true</code> if and only if the directory was created,
		///          along with all necessary parent directories; <code>false</code>
		///          otherwise
		/// </returns>
		/// <exception cref="SecurityException">
		///          If a security manager exists and its <code>{@link
		///          java.lang.SecurityManager#checkRead(java.lang.String)}</code>
		///          method does not permit verification of the existence of the
		///          named directory and all necessary parent directories; or if
		///          the <code>{@link
		///          java.lang.SecurityManager#checkWrite(java.lang.String)}</code>
		///          method does not permit the named directory and all necessary
		///          parent directories to be created </exception>
		public virtual bool Mkdirs()
		{
			if (Exists())
			{
				return false;
			}
			if (Mkdir())
			{
				return true;
			}
			File canonFile = null;
			try
			{
				canonFile = CanonicalFile;
			}
			catch (IOException)
			{
				return false;
			}

			File parent = canonFile.ParentFile;
			return (parent != null && (parent.Mkdirs() || parent.Exists()) && canonFile.Mkdir());
		}

		/// <summary>
		/// Renames the file denoted by this abstract pathname.
		/// 
		/// <para> Many aspects of the behavior of this method are inherently
		/// platform-dependent: The rename operation might not be able to move a
		/// file from one filesystem to another, it might not be atomic, and it
		/// might not succeed if a file with the destination abstract pathname
		/// already exists.  The return value should always be checked to make sure
		/// that the rename operation was successful.
		/// 
		/// </para>
		/// <para> Note that the <seealso cref="java.nio.file.Files"/> class defines the {@link
		/// java.nio.file.Files#move move} method to move or rename a file in a
		/// platform independent manner.
		/// 
		/// </para>
		/// </summary>
		/// <param name="dest">  The new abstract pathname for the named file
		/// </param>
		/// <returns>  <code>true</code> if and only if the renaming succeeded;
		///          <code>false</code> otherwise
		/// </returns>
		/// <exception cref="SecurityException">
		///          If a security manager exists and its <code>{@link
		///          java.lang.SecurityManager#checkWrite(java.lang.String)}</code>
		///          method denies write access to either the old or new pathnames
		/// </exception>
		/// <exception cref="NullPointerException">
		///          If parameter <code>dest</code> is <code>null</code> </exception>
		public virtual bool RenameTo(File dest)
		{
			SecurityManager security = System.SecurityManager;
			if (security != null)
			{
				security.CheckWrite(Path_Renamed);
				security.CheckWrite(dest.Path_Renamed);
			}
			if (dest == null)
			{
				throw new NullPointerException();
			}
			if (this.Invalid || dest.Invalid)
			{
				return false;
			}
			return Fs.Rename(this, dest);
		}

		/// <summary>
		/// Sets the last-modified time of the file or directory named by this
		/// abstract pathname.
		/// 
		/// <para> All platforms support file-modification times to the nearest second,
		/// but some provide more precision.  The argument will be truncated to fit
		/// the supported precision.  If the operation succeeds and no intervening
		/// operations on the file take place, then the next invocation of the
		/// <code><seealso cref="#lastModified"/></code> method will return the (possibly
		/// truncated) <code>time</code> argument that was passed to this method.
		/// 
		/// </para>
		/// </summary>
		/// <param name="time">  The new last-modified time, measured in milliseconds since
		///               the epoch (00:00:00 GMT, January 1, 1970)
		/// </param>
		/// <returns> <code>true</code> if and only if the operation succeeded;
		///          <code>false</code> otherwise
		/// </returns>
		/// <exception cref="IllegalArgumentException">  If the argument is negative
		/// </exception>
		/// <exception cref="SecurityException">
		///          If a security manager exists and its <code>{@link
		///          java.lang.SecurityManager#checkWrite(java.lang.String)}</code>
		///          method denies write access to the named file
		/// 
		/// @since 1.2 </exception>
		public virtual bool SetLastModified(long time)
		{
			if (time < 0)
			{
				throw new IllegalArgumentException("Negative time");
			}
			SecurityManager security = System.SecurityManager;
			if (security != null)
			{
				security.CheckWrite(Path_Renamed);
			}
			if (Invalid)
			{
				return false;
			}
			return Fs.SetLastModifiedTime(this, time);
		}

		/// <summary>
		/// Marks the file or directory named by this abstract pathname so that
		/// only read operations are allowed. After invoking this method the file
		/// or directory will not change until it is either deleted or marked
		/// to allow write access. On some platforms it may be possible to start the
		/// Java virtual machine with special privileges that allow it to modify
		/// files that are marked read-only. Whether or not a read-only file or
		/// directory may be deleted depends upon the underlying system.
		/// </summary>
		/// <returns> <code>true</code> if and only if the operation succeeded;
		///          <code>false</code> otherwise
		/// </returns>
		/// <exception cref="SecurityException">
		///          If a security manager exists and its <code>{@link
		///          java.lang.SecurityManager#checkWrite(java.lang.String)}</code>
		///          method denies write access to the named file
		/// 
		/// @since 1.2 </exception>
		public virtual bool SetReadOnly()
		{
			SecurityManager security = System.SecurityManager;
			if (security != null)
			{
				security.CheckWrite(Path_Renamed);
			}
			if (Invalid)
			{
				return false;
			}
			return Fs.setReadOnly(this);
		}

		/// <summary>
		/// Sets the owner's or everybody's write permission for this abstract
		/// pathname. On some platforms it may be possible to start the Java virtual
		/// machine with special privileges that allow it to modify files that
		/// disallow write operations.
		/// 
		/// <para> The <seealso cref="java.nio.file.Files"/> class defines methods that operate on
		/// file attributes including file permissions. This may be used when finer
		/// manipulation of file permissions is required.
		/// 
		/// </para>
		/// </summary>
		/// <param name="writable">
		///          If <code>true</code>, sets the access permission to allow write
		///          operations; if <code>false</code> to disallow write operations
		/// </param>
		/// <param name="ownerOnly">
		///          If <code>true</code>, the write permission applies only to the
		///          owner's write permission; otherwise, it applies to everybody.  If
		///          the underlying file system can not distinguish the owner's write
		///          permission from that of others, then the permission will apply to
		///          everybody, regardless of this value.
		/// </param>
		/// <returns>  <code>true</code> if and only if the operation succeeded. The
		///          operation will fail if the user does not have permission to change
		///          the access permissions of this abstract pathname.
		/// </returns>
		/// <exception cref="SecurityException">
		///          If a security manager exists and its <code>{@link
		///          java.lang.SecurityManager#checkWrite(java.lang.String)}</code>
		///          method denies write access to the named file
		/// 
		/// @since 1.6 </exception>
		public virtual bool SetWritable(bool writable, bool ownerOnly)
		{
			SecurityManager security = System.SecurityManager;
			if (security != null)
			{
				security.CheckWrite(Path_Renamed);
			}
			if (Invalid)
			{
				return false;
			}
			return Fs.SetPermission(this, FileSystem.ACCESS_WRITE, writable, ownerOnly);
		}

		/// <summary>
		/// A convenience method to set the owner's write permission for this abstract
		/// pathname. On some platforms it may be possible to start the Java virtual
		/// machine with special privileges that allow it to modify files that
		/// disallow write operations.
		/// 
		/// <para> An invocation of this method of the form <tt>file.setWritable(arg)</tt>
		/// behaves in exactly the same way as the invocation
		/// 
		/// <pre>
		///     file.setWritable(arg, true) </pre>
		/// 
		/// </para>
		/// </summary>
		/// <param name="writable">
		///          If <code>true</code>, sets the access permission to allow write
		///          operations; if <code>false</code> to disallow write operations
		/// </param>
		/// <returns>  <code>true</code> if and only if the operation succeeded.  The
		///          operation will fail if the user does not have permission to
		///          change the access permissions of this abstract pathname.
		/// </returns>
		/// <exception cref="SecurityException">
		///          If a security manager exists and its <code>{@link
		///          java.lang.SecurityManager#checkWrite(java.lang.String)}</code>
		///          method denies write access to the file
		/// 
		/// @since 1.6 </exception>
		public virtual bool SetWritable(bool writable)
		{
			return SetWritable(writable, true);
		}

		/// <summary>
		/// Sets the owner's or everybody's read permission for this abstract
		/// pathname. On some platforms it may be possible to start the Java virtual
		/// machine with special privileges that allow it to read files that are
		/// marked as unreadable.
		/// 
		/// <para> The <seealso cref="java.nio.file.Files"/> class defines methods that operate on
		/// file attributes including file permissions. This may be used when finer
		/// manipulation of file permissions is required.
		/// 
		/// </para>
		/// </summary>
		/// <param name="readable">
		///          If <code>true</code>, sets the access permission to allow read
		///          operations; if <code>false</code> to disallow read operations
		/// </param>
		/// <param name="ownerOnly">
		///          If <code>true</code>, the read permission applies only to the
		///          owner's read permission; otherwise, it applies to everybody.  If
		///          the underlying file system can not distinguish the owner's read
		///          permission from that of others, then the permission will apply to
		///          everybody, regardless of this value.
		/// </param>
		/// <returns>  <code>true</code> if and only if the operation succeeded.  The
		///          operation will fail if the user does not have permission to
		///          change the access permissions of this abstract pathname.  If
		///          <code>readable</code> is <code>false</code> and the underlying
		///          file system does not implement a read permission, then the
		///          operation will fail.
		/// </returns>
		/// <exception cref="SecurityException">
		///          If a security manager exists and its <code>{@link
		///          java.lang.SecurityManager#checkWrite(java.lang.String)}</code>
		///          method denies write access to the file
		/// 
		/// @since 1.6 </exception>
		public virtual bool SetReadable(bool readable, bool ownerOnly)
		{
			SecurityManager security = System.SecurityManager;
			if (security != null)
			{
				security.CheckWrite(Path_Renamed);
			}
			if (Invalid)
			{
				return false;
			}
			return Fs.SetPermission(this, FileSystem.ACCESS_READ, readable, ownerOnly);
		}

		/// <summary>
		/// A convenience method to set the owner's read permission for this abstract
		/// pathname. On some platforms it may be possible to start the Java virtual
		/// machine with special privileges that allow it to read files that that are
		/// marked as unreadable.
		/// 
		/// <para>An invocation of this method of the form <tt>file.setReadable(arg)</tt>
		/// behaves in exactly the same way as the invocation
		/// 
		/// <pre>
		///     file.setReadable(arg, true) </pre>
		/// 
		/// </para>
		/// </summary>
		/// <param name="readable">
		///          If <code>true</code>, sets the access permission to allow read
		///          operations; if <code>false</code> to disallow read operations
		/// </param>
		/// <returns>  <code>true</code> if and only if the operation succeeded.  The
		///          operation will fail if the user does not have permission to
		///          change the access permissions of this abstract pathname.  If
		///          <code>readable</code> is <code>false</code> and the underlying
		///          file system does not implement a read permission, then the
		///          operation will fail.
		/// </returns>
		/// <exception cref="SecurityException">
		///          If a security manager exists and its <code>{@link
		///          java.lang.SecurityManager#checkWrite(java.lang.String)}</code>
		///          method denies write access to the file
		/// 
		/// @since 1.6 </exception>
		public virtual bool SetReadable(bool readable)
		{
			return SetReadable(readable, true);
		}

		/// <summary>
		/// Sets the owner's or everybody's execute permission for this abstract
		/// pathname. On some platforms it may be possible to start the Java virtual
		/// machine with special privileges that allow it to execute files that are
		/// not marked executable.
		/// 
		/// <para> The <seealso cref="java.nio.file.Files"/> class defines methods that operate on
		/// file attributes including file permissions. This may be used when finer
		/// manipulation of file permissions is required.
		/// 
		/// </para>
		/// </summary>
		/// <param name="executable">
		///          If <code>true</code>, sets the access permission to allow execute
		///          operations; if <code>false</code> to disallow execute operations
		/// </param>
		/// <param name="ownerOnly">
		///          If <code>true</code>, the execute permission applies only to the
		///          owner's execute permission; otherwise, it applies to everybody.
		///          If the underlying file system can not distinguish the owner's
		///          execute permission from that of others, then the permission will
		///          apply to everybody, regardless of this value.
		/// </param>
		/// <returns>  <code>true</code> if and only if the operation succeeded.  The
		///          operation will fail if the user does not have permission to
		///          change the access permissions of this abstract pathname.  If
		///          <code>executable</code> is <code>false</code> and the underlying
		///          file system does not implement an execute permission, then the
		///          operation will fail.
		/// </returns>
		/// <exception cref="SecurityException">
		///          If a security manager exists and its <code>{@link
		///          java.lang.SecurityManager#checkWrite(java.lang.String)}</code>
		///          method denies write access to the file
		/// 
		/// @since 1.6 </exception>
		public virtual bool SetExecutable(bool executable, bool ownerOnly)
		{
			SecurityManager security = System.SecurityManager;
			if (security != null)
			{
				security.CheckWrite(Path_Renamed);
			}
			if (Invalid)
			{
				return false;
			}
			return Fs.SetPermission(this, FileSystem.ACCESS_EXECUTE, executable, ownerOnly);
		}

		/// <summary>
		/// A convenience method to set the owner's execute permission for this
		/// abstract pathname. On some platforms it may be possible to start the Java
		/// virtual machine with special privileges that allow it to execute files
		/// that are not marked executable.
		/// 
		/// <para>An invocation of this method of the form <tt>file.setExcutable(arg)</tt>
		/// behaves in exactly the same way as the invocation
		/// 
		/// <pre>
		///     file.setExecutable(arg, true) </pre>
		/// 
		/// </para>
		/// </summary>
		/// <param name="executable">
		///          If <code>true</code>, sets the access permission to allow execute
		///          operations; if <code>false</code> to disallow execute operations
		/// </param>
		/// <returns>   <code>true</code> if and only if the operation succeeded.  The
		///           operation will fail if the user does not have permission to
		///           change the access permissions of this abstract pathname.  If
		///           <code>executable</code> is <code>false</code> and the underlying
		///           file system does not implement an execute permission, then the
		///           operation will fail.
		/// </returns>
		/// <exception cref="SecurityException">
		///          If a security manager exists and its <code>{@link
		///          java.lang.SecurityManager#checkWrite(java.lang.String)}</code>
		///          method denies write access to the file
		/// 
		/// @since 1.6 </exception>
		public virtual bool SetExecutable(bool executable)
		{
			return SetExecutable(executable, true);
		}

		/// <summary>
		/// Tests whether the application can execute the file denoted by this
		/// abstract pathname. On some platforms it may be possible to start the
		/// Java virtual machine with special privileges that allow it to execute
		/// files that are not marked executable. Consequently this method may return
		/// {@code true} even though the file does not have execute permissions.
		/// </summary>
		/// <returns>  <code>true</code> if and only if the abstract pathname exists
		///          <em>and</em> the application is allowed to execute the file
		/// </returns>
		/// <exception cref="SecurityException">
		///          If a security manager exists and its <code>{@link
		///          java.lang.SecurityManager#checkExec(java.lang.String)}</code>
		///          method denies execute access to the file
		/// 
		/// @since 1.6 </exception>
		public virtual bool CanExecute()
		{
			SecurityManager security = System.SecurityManager;
			if (security != null)
			{
				security.CheckExec(Path_Renamed);
			}
			if (Invalid)
			{
				return false;
			}
			return Fs.CheckAccess(this, FileSystem.ACCESS_EXECUTE);
		}


		/* -- Filesystem interface -- */

		/// <summary>
		/// List the available filesystem roots.
		/// 
		/// <para> A particular Java platform may support zero or more
		/// hierarchically-organized file systems.  Each file system has a
		/// {@code root} directory from which all other files in that file system
		/// can be reached.  Windows platforms, for example, have a root directory
		/// for each active drive; UNIX platforms have a single root directory,
		/// namely {@code "/"}.  The set of available filesystem roots is affected
		/// by various system-level operations such as the insertion or ejection of
		/// removable media and the disconnecting or unmounting of physical or
		/// virtual disk drives.
		/// 
		/// </para>
		/// <para> This method returns an array of {@code File} objects that denote the
		/// root directories of the available filesystem roots.  It is guaranteed
		/// that the canonical pathname of any file physically present on the local
		/// machine will begin with one of the roots returned by this method.
		/// 
		/// </para>
		/// <para> The canonical pathname of a file that resides on some other machine
		/// and is accessed via a remote-filesystem protocol such as SMB or NFS may
		/// or may not begin with one of the roots returned by this method.  If the
		/// pathname of a remote file is syntactically indistinguishable from the
		/// pathname of a local file then it will begin with one of the roots
		/// returned by this method.  Thus, for example, {@code File} objects
		/// denoting the root directories of the mapped network drives of a Windows
		/// platform will be returned by this method, while {@code File} objects
		/// containing UNC pathnames will not be returned by this method.
		/// 
		/// </para>
		/// <para> Unlike most methods in this class, this method does not throw
		/// security exceptions.  If a security manager exists and its {@link
		/// SecurityManager#checkRead(String)} method denies read access to a
		/// particular root directory, then that directory will not appear in the
		/// result.
		/// 
		/// </para>
		/// </summary>
		/// <returns>  An array of {@code File} objects denoting the available
		///          filesystem roots, or {@code null} if the set of roots could not
		///          be determined.  The array will be empty if there are no
		///          filesystem roots.
		/// 
		/// @since  1.2 </returns>
		/// <seealso cref= java.nio.file.FileStore </seealso>
		public static File[] ListRoots()
		{
			return Fs.ListRoots();
		}


		/* -- Disk usage -- */

		/// <summary>
		/// Returns the size of the partition <a href="#partName">named</a> by this
		/// abstract pathname.
		/// </summary>
		/// <returns>  The size, in bytes, of the partition or <tt>0L</tt> if this
		///          abstract pathname does not name a partition
		/// </returns>
		/// <exception cref="SecurityException">
		///          If a security manager has been installed and it denies
		///          <seealso cref="RuntimePermission"/><tt>("getFileSystemAttributes")</tt>
		///          or its <seealso cref="SecurityManager#checkRead(String)"/> method denies
		///          read access to the file named by this abstract pathname
		/// 
		/// @since  1.6 </exception>
		public virtual long TotalSpace
		{
			get
			{
				SecurityManager sm = System.SecurityManager;
				if (sm != null)
				{
					sm.CheckPermission(new RuntimePermission("getFileSystemAttributes"));
					sm.CheckRead(Path_Renamed);
				}
				if (Invalid)
				{
					return 0L;
				}
				return Fs.GetSpace(this, FileSystem.SPACE_TOTAL);
			}
		}

		/// <summary>
		/// Returns the number of unallocated bytes in the partition <a
		/// href="#partName">named</a> by this abstract path name.
		/// 
		/// <para> The returned number of unallocated bytes is a hint, but not
		/// a guarantee, that it is possible to use most or any of these
		/// bytes.  The number of unallocated bytes is most likely to be
		/// accurate immediately after this call.  It is likely to be made
		/// inaccurate by any external I/O operations including those made
		/// on the system outside of this virtual machine.  This method
		/// makes no guarantee that write operations to this file system
		/// will succeed.
		/// 
		/// </para>
		/// </summary>
		/// <returns>  The number of unallocated bytes on the partition or <tt>0L</tt>
		///          if the abstract pathname does not name a partition.  This
		///          value will be less than or equal to the total file system size
		///          returned by <seealso cref="#getTotalSpace"/>.
		/// </returns>
		/// <exception cref="SecurityException">
		///          If a security manager has been installed and it denies
		///          <seealso cref="RuntimePermission"/><tt>("getFileSystemAttributes")</tt>
		///          or its <seealso cref="SecurityManager#checkRead(String)"/> method denies
		///          read access to the file named by this abstract pathname
		/// 
		/// @since  1.6 </exception>
		public virtual long FreeSpace
		{
			get
			{
				SecurityManager sm = System.SecurityManager;
				if (sm != null)
				{
					sm.CheckPermission(new RuntimePermission("getFileSystemAttributes"));
					sm.CheckRead(Path_Renamed);
				}
				if (Invalid)
				{
					return 0L;
				}
				return Fs.GetSpace(this, FileSystem.SPACE_FREE);
			}
		}

		/// <summary>
		/// Returns the number of bytes available to this virtual machine on the
		/// partition <a href="#partName">named</a> by this abstract pathname.  When
		/// possible, this method checks for write permissions and other operating
		/// system restrictions and will therefore usually provide a more accurate
		/// estimate of how much new data can actually be written than {@link
		/// #getFreeSpace}.
		/// 
		/// <para> The returned number of available bytes is a hint, but not a
		/// guarantee, that it is possible to use most or any of these bytes.  The
		/// number of unallocated bytes is most likely to be accurate immediately
		/// after this call.  It is likely to be made inaccurate by any external
		/// I/O operations including those made on the system outside of this
		/// virtual machine.  This method makes no guarantee that write operations
		/// to this file system will succeed.
		/// 
		/// </para>
		/// </summary>
		/// <returns>  The number of available bytes on the partition or <tt>0L</tt>
		///          if the abstract pathname does not name a partition.  On
		///          systems where this information is not available, this method
		///          will be equivalent to a call to <seealso cref="#getFreeSpace"/>.
		/// </returns>
		/// <exception cref="SecurityException">
		///          If a security manager has been installed and it denies
		///          <seealso cref="RuntimePermission"/><tt>("getFileSystemAttributes")</tt>
		///          or its <seealso cref="SecurityManager#checkRead(String)"/> method denies
		///          read access to the file named by this abstract pathname
		/// 
		/// @since  1.6 </exception>
		public virtual long UsableSpace
		{
			get
			{
				SecurityManager sm = System.SecurityManager;
				if (sm != null)
				{
					sm.CheckPermission(new RuntimePermission("getFileSystemAttributes"));
					sm.CheckRead(Path_Renamed);
				}
				if (Invalid)
				{
					return 0L;
				}
				return Fs.GetSpace(this, FileSystem.SPACE_USABLE);
			}
		}

		/* -- Temporary files -- */

		private class TempDirectory
		{
			internal TempDirectory()
			{
			}

			// temporary directory location
			internal static readonly File Tmpdir = new File(AccessController.doPrivileged(new GetPropertyAction("java.io.tmpdir")));
			internal static File Location()
			{
				return Tmpdir;
			}

			// file name generation
			internal static readonly SecureRandom Random = new SecureRandom();
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: static File generateFile(String prefix, String suffix, File dir) throws IOException
			internal static File GenerateFile(String prefix, String suffix, File dir)
			{
				long n = Random.NextLong();
				if (n == Long.MinValue)
				{
					n = 0; // corner case
				}
				else
				{
					n = System.Math.Abs(n);
				}

				// Use only the file name from the supplied prefix
				prefix = (new System.IO.FileInfo(prefix)).Name;

				String name = prefix + Convert.ToString(n) + suffix;
				File f = new File(dir, name);
				if (!name.Equals(f.Name) || f.Invalid)
				{
					if (System.SecurityManager != null)
					{
						throw new IOException("Unable to create temporary file");
					}
					else
					{
						throw new IOException("Unable to create temporary file, " + f);
					}
				}
				return f;
			}
		}

		/// <summary>
		/// <para> Creates a new empty file in the specified directory, using the
		/// given prefix and suffix strings to generate its name.  If this method
		/// returns successfully then it is guaranteed that:
		/// 
		/// <ol>
		/// <li> The file denoted by the returned abstract pathname did not exist
		///      before this method was invoked, and
		/// <li> Neither this method nor any of its variants will return the same
		///      abstract pathname again in the current invocation of the virtual
		///      machine.
		/// </ol>
		/// 
		/// This method provides only part of a temporary-file facility.  To arrange
		/// for a file created by this method to be deleted automatically, use the
		/// <code><seealso cref="#deleteOnExit"/></code> method.
		/// 
		/// </para>
		/// <para> The <code>prefix</code> argument must be at least three characters
		/// long.  It is recommended that the prefix be a short, meaningful string
		/// such as <code>"hjb"</code> or <code>"mail"</code>.  The
		/// <code>suffix</code> argument may be <code>null</code>, in which case the
		/// suffix <code>".tmp"</code> will be used.
		/// 
		/// </para>
		/// <para> To create the new file, the prefix and the suffix may first be
		/// adjusted to fit the limitations of the underlying platform.  If the
		/// prefix is too long then it will be truncated, but its first three
		/// characters will always be preserved.  If the suffix is too long then it
		/// too will be truncated, but if it begins with a period character
		/// (<code>'.'</code>) then the period and the first three characters
		/// following it will always be preserved.  Once these adjustments have been
		/// made the name of the new file will be generated by concatenating the
		/// prefix, five or more internally-generated characters, and the suffix.
		/// 
		/// </para>
		/// <para> If the <code>directory</code> argument is <code>null</code> then the
		/// system-dependent default temporary-file directory will be used.  The
		/// default temporary-file directory is specified by the system property
		/// <code>java.io.tmpdir</code>.  On UNIX systems the default value of this
		/// property is typically <code>"/tmp"</code> or <code>"/var/tmp"</code>; on
		/// Microsoft Windows systems it is typically <code>"C:\\WINNT\\TEMP"</code>.  A different
		/// value may be given to this system property when the Java virtual machine
		/// is invoked, but programmatic changes to this property are not guaranteed
		/// to have any effect upon the temporary directory used by this method.
		/// 
		/// </para>
		/// </summary>
		/// <param name="prefix">     The prefix string to be used in generating the file's
		///                    name; must be at least three characters long
		/// </param>
		/// <param name="suffix">     The suffix string to be used in generating the file's
		///                    name; may be <code>null</code>, in which case the
		///                    suffix <code>".tmp"</code> will be used
		/// </param>
		/// <param name="directory">  The directory in which the file is to be created, or
		///                    <code>null</code> if the default temporary-file
		///                    directory is to be used
		/// </param>
		/// <returns>  An abstract pathname denoting a newly-created empty file
		/// </returns>
		/// <exception cref="IllegalArgumentException">
		///          If the <code>prefix</code> argument contains fewer than three
		///          characters
		/// </exception>
		/// <exception cref="IOException">  If a file could not be created
		/// </exception>
		/// <exception cref="SecurityException">
		///          If a security manager exists and its <code>{@link
		///          java.lang.SecurityManager#checkWrite(java.lang.String)}</code>
		///          method does not allow a file to be created
		/// 
		/// @since 1.2 </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public static File createTempFile(String prefix, String suffix, File directory) throws IOException
		public static File CreateTempFile(String prefix, String suffix, File directory)
		{
			if (prefix.Length() < 3)
			{
				throw new IllegalArgumentException("Prefix string too short");
			}
			if (suffix == null)
			{
				suffix = ".tmp";
			}

			File tmpdir = (directory != null) ? directory : TempDirectory.Location();
			SecurityManager sm = System.SecurityManager;
			File f;
			do
			{
				f = TempDirectory.GenerateFile(prefix, suffix, tmpdir);

				if (sm != null)
				{
					try
					{
						sm.CheckWrite(f.Path);
					}
					catch (SecurityException se)
					{
						// don't reveal temporary directory location
						if (directory == null)
						{
							throw new SecurityException("Unable to create temporary file");
						}
						throw se;
					}
				}
			} while ((Fs.GetBooleanAttributes(f) & FileSystem.BA_EXISTS) != 0);

			if (!Fs.CreateFileExclusively(f.Path))
			{
				throw new IOException("Unable to create temporary file");
			}

			return f;
		}

		/// <summary>
		/// Creates an empty file in the default temporary-file directory, using
		/// the given prefix and suffix to generate its name. Invoking this method
		/// is equivalent to invoking <code>{@link #createTempFile(java.lang.String,
		/// java.lang.String, java.io.File)
		/// createTempFile(prefix,&nbsp;suffix,&nbsp;null)}</code>.
		/// 
		/// <para> The {@link
		/// java.nio.file.Files#createTempFile(String,String,java.nio.file.attribute.FileAttribute[])
		/// Files.createTempFile} method provides an alternative method to create an
		/// empty file in the temporary-file directory. Files created by that method
		/// may have more restrictive access permissions to files created by this
		/// method and so may be more suited to security-sensitive applications.
		/// 
		/// </para>
		/// </summary>
		/// <param name="prefix">     The prefix string to be used in generating the file's
		///                    name; must be at least three characters long
		/// </param>
		/// <param name="suffix">     The suffix string to be used in generating the file's
		///                    name; may be <code>null</code>, in which case the
		///                    suffix <code>".tmp"</code> will be used
		/// </param>
		/// <returns>  An abstract pathname denoting a newly-created empty file
		/// </returns>
		/// <exception cref="IllegalArgumentException">
		///          If the <code>prefix</code> argument contains fewer than three
		///          characters
		/// </exception>
		/// <exception cref="IOException">  If a file could not be created
		/// </exception>
		/// <exception cref="SecurityException">
		///          If a security manager exists and its <code>{@link
		///          java.lang.SecurityManager#checkWrite(java.lang.String)}</code>
		///          method does not allow a file to be created
		/// 
		/// @since 1.2 </exception>
		/// <seealso cref= java.nio.file.Files#createTempDirectory(String,FileAttribute[]) </seealso>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public static File createTempFile(String prefix, String suffix) throws IOException
		public static File CreateTempFile(String prefix, String suffix)
		{
			return CreateTempFile(prefix, suffix, null);
		}

		/* -- Basic infrastructure -- */

		/// <summary>
		/// Compares two abstract pathnames lexicographically.  The ordering
		/// defined by this method depends upon the underlying system.  On UNIX
		/// systems, alphabetic case is significant in comparing pathnames; on Microsoft Windows
		/// systems it is not.
		/// </summary>
		/// <param name="pathname">  The abstract pathname to be compared to this abstract
		///                    pathname
		/// </param>
		/// <returns>  Zero if the argument is equal to this abstract pathname, a
		///          value less than zero if this abstract pathname is
		///          lexicographically less than the argument, or a value greater
		///          than zero if this abstract pathname is lexicographically
		///          greater than the argument
		/// 
		/// @since   1.2 </returns>
		public virtual int CompareTo(File pathname)
		{
			return Fs.Compare(this, pathname);
		}

		/// <summary>
		/// Tests this abstract pathname for equality with the given object.
		/// Returns <code>true</code> if and only if the argument is not
		/// <code>null</code> and is an abstract pathname that denotes the same file
		/// or directory as this abstract pathname.  Whether or not two abstract
		/// pathnames are equal depends upon the underlying system.  On UNIX
		/// systems, alphabetic case is significant in comparing pathnames; on Microsoft Windows
		/// systems it is not.
		/// </summary>
		/// <param name="obj">   The object to be compared with this abstract pathname
		/// </param>
		/// <returns>  <code>true</code> if and only if the objects are the same;
		///          <code>false</code> otherwise </returns>
		public override bool Equals(Object obj)
		{
			if ((obj != null) && (obj is File))
			{
				return CompareTo((File)obj) == 0;
			}
			return false;
		}

		/// <summary>
		/// Computes a hash code for this abstract pathname.  Because equality of
		/// abstract pathnames is inherently system-dependent, so is the computation
		/// of their hash codes.  On UNIX systems, the hash code of an abstract
		/// pathname is equal to the exclusive <em>or</em> of the hash code
		/// of its pathname string and the decimal value
		/// <code>1234321</code>.  On Microsoft Windows systems, the hash
		/// code is equal to the exclusive <em>or</em> of the hash code of
		/// its pathname string converted to lower case and the decimal
		/// value <code>1234321</code>.  Locale is not taken into account on
		/// lowercasing the pathname string.
		/// </summary>
		/// <returns>  A hash code for this abstract pathname </returns>
		public override int HashCode()
		{
			return Fs.HashCode(this);
		}

		/// <summary>
		/// Returns the pathname string of this abstract pathname.  This is just the
		/// string returned by the <code><seealso cref="#getPath"/></code> method.
		/// </summary>
		/// <returns>  The string form of this abstract pathname </returns>
		public override String ToString()
		{
			return Path;
		}

		/// <summary>
		/// WriteObject is called to save this filename.
		/// The separator character is saved also so it can be replaced
		/// in case the path is reconstituted on a different host type.
		/// <para>
		/// @serialData  Default fields followed by separator character.
		/// </para>
		/// </summary>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: private synchronized void writeObject(java.io.ObjectOutputStream s) throws IOException
		private void WriteObject(java.io.ObjectOutputStream s)
		{
			lock (this)
			{
				s.DefaultWriteObject();
				s.WriteChar(SeparatorChar); // Add the separator character
			}
		}

		/// <summary>
		/// readObject is called to restore this filename.
		/// The original separator character is read.  If it is different
		/// than the separator character on this system, then the old separator
		/// is replaced by the local separator.
		/// </summary>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: private synchronized void readObject(java.io.ObjectInputStream s) throws IOException, ClassNotFoundException
		private void ReadObject(java.io.ObjectInputStream s)
		{
			lock (this)
			{
				ObjectInputStream.GetField fields = s.ReadFields();
				String pathField = (String)fields.Get("path", null);
				char sep = s.ReadChar(); // read the previous separator char
				if (sep != SeparatorChar)
				{
					pathField = pathField.Replace(sep, SeparatorChar);
				}
				String path = Fs.Normalize(pathField);
				UNSAFE.putObject(this, PATH_OFFSET, path);
				UNSAFE.putIntVolatile(this, PREFIX_LENGTH_OFFSET, Fs.PrefixLength(path));
			}
		}

		private static readonly long PATH_OFFSET;
		private static readonly long PREFIX_LENGTH_OFFSET;
		private static readonly sun.misc.Unsafe UNSAFE;
		static File()
		{
			try
			{
				sun.misc.Unsafe @unsafe = sun.misc.Unsafe.Unsafe;
				PATH_OFFSET = @unsafe.objectFieldOffset(typeof(File).getDeclaredField("path"));
				PREFIX_LENGTH_OFFSET = @unsafe.objectFieldOffset(typeof(File).getDeclaredField("prefixLength"));
				UNSAFE = @unsafe;
			}
			catch (ReflectiveOperationException e)
			{
				throw new Error(e);
			}
		}


		/// <summary>
		/// use serialVersionUID from JDK 1.0.2 for interoperability </summary>
		private const long SerialVersionUID = 301077366599181567L;

		// -- Integration with java.nio.file --

		[NonSerialized]
		private volatile Path FilePath;

		/// <summary>
		/// Returns a <seealso cref="Path java.nio.file.Path"/> object constructed from the
		/// this abstract path. The resulting {@code Path} is associated with the
		/// <seealso cref="java.nio.file.FileSystems#getDefault default-filesystem"/>.
		/// 
		/// <para> The first invocation of this method works as if invoking it were
		/// equivalent to evaluating the expression:
		/// <blockquote><pre>
		/// <seealso cref="java.nio.file.FileSystems#getDefault FileSystems.getDefault"/>().{@link
		/// java.nio.file.FileSystem#getPath getPath}(this.<seealso cref="#getPath getPath"/>());
		/// </pre></blockquote>
		/// Subsequent invocations of this method return the same {@code Path}.
		/// 
		/// </para>
		/// <para> If this abstract pathname is the empty abstract pathname then this
		/// method returns a {@code Path} that may be used to access the current
		/// user directory.
		/// 
		/// </para>
		/// </summary>
		/// <returns>  a {@code Path} constructed from this abstract path
		/// </returns>
		/// <exception cref="java.nio.file.InvalidPathException">
		///          if a {@code Path} object cannot be constructed from the abstract
		///          path (see <seealso cref="java.nio.file.FileSystem#getPath FileSystem.getPath"/>)
		/// 
		/// @since   1.7 </exception>
		/// <seealso cref= Path#toFile </seealso>
		public virtual Path ToPath()
		{
			Path result = FilePath;
			if (result == null)
			{
				lock (this)
				{
					result = FilePath;
					if (result == null)
					{
						result = FileSystems.Default.getPath(Path_Renamed);
						FilePath = result;
					}
				}
			}
			return result;
		}
	}

}
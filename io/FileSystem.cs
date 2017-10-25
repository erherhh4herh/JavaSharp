/*
 * Copyright (c) 1998, 2013, Oracle and/or its affiliates. All rights reserved.
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

	/// <summary>
	/// Package-private abstract class for the local filesystem abstraction.
	/// </summary>

	internal abstract class FileSystem
	{

		/* -- Normalization and construction -- */

		/// <summary>
		/// Return the local filesystem's name-separator character.
		/// </summary>
		public abstract char Separator {get;}

		/// <summary>
		/// Return the local filesystem's path-separator character.
		/// </summary>
		public abstract char PathSeparator {get;}

		/// <summary>
		/// Convert the given pathname string to normal form.  If the string is
		/// already in normal form then it is simply returned.
		/// </summary>
		public abstract String Normalize(String path);

		/// <summary>
		/// Compute the length of this pathname string's prefix.  The pathname
		/// string must be in normal form.
		/// </summary>
		public abstract int PrefixLength(String path);

		/// <summary>
		/// Resolve the child pathname string against the parent.
		/// Both strings must be in normal form, and the result
		/// will be in normal form.
		/// </summary>
		public abstract String Resolve(String parent, String child);

		/// <summary>
		/// Return the parent pathname string to be used when the parent-directory
		/// argument in one of the two-argument File constructors is the empty
		/// pathname.
		/// </summary>
		public abstract String DefaultParent {get;}

		/// <summary>
		/// Post-process the given URI path string if necessary.  This is used on
		/// win32, e.g., to transform "/c:/foo" into "c:/foo".  The path string
		/// still has slash separators; code in the File class will translate them
		/// after this method returns.
		/// </summary>
		public abstract String FromURIPath(String path);


		/* -- Path operations -- */

		/// <summary>
		/// Tell whether or not the given abstract pathname is absolute.
		/// </summary>
		public abstract bool IsAbsolute(File f);

		/// <summary>
		/// Resolve the given abstract pathname into absolute form.  Invoked by the
		/// getAbsolutePath and getCanonicalPath methods in the File class.
		/// </summary>
		public abstract String Resolve(File f);

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public abstract String canonicalize(String path) throws IOException;
		public abstract String Canonicalize(String path);


		/* -- Attribute accessors -- */

		/* Constants for simple boolean attributes */
//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Native public static final int BA_EXISTS = 0x01;
		public const int BA_EXISTS = 0x01;
//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Native public static final int BA_REGULAR = 0x02;
		public const int BA_REGULAR = 0x02;
//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Native public static final int BA_DIRECTORY = 0x04;
		public const int BA_DIRECTORY = 0x04;
//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Native public static final int BA_HIDDEN = 0x08;
		public const int BA_HIDDEN = 0x08;

		/// <summary>
		/// Return the simple boolean attributes for the file or directory denoted
		/// by the given abstract pathname, or zero if it does not exist or some
		/// other I/O error occurs.
		/// </summary>
		public abstract int GetBooleanAttributes(File f);

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Native public static final int ACCESS_READ = 0x04;
		public const int ACCESS_READ = 0x04;
//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Native public static final int ACCESS_WRITE = 0x02;
		public const int ACCESS_WRITE = 0x02;
//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Native public static final int ACCESS_EXECUTE = 0x01;
		public const int ACCESS_EXECUTE = 0x01;

		/// <summary>
		/// Check whether the file or directory denoted by the given abstract
		/// pathname may be accessed by this process.  The second argument specifies
		/// which access, ACCESS_READ, ACCESS_WRITE or ACCESS_EXECUTE, to check.
		/// Return false if access is denied or an I/O error occurs
		/// </summary>
		public abstract bool CheckAccess(File f, int access);
		/// <summary>
		/// Set on or off the access permission (to owner only or to all) to the file
		/// or directory denoted by the given abstract pathname, based on the parameters
		/// enable, access and oweronly.
		/// </summary>
		public abstract bool SetPermission(File f, int access, bool enable, bool owneronly);

		/// <summary>
		/// Return the time at which the file or directory denoted by the given
		/// abstract pathname was last modified, or zero if it does not exist or
		/// some other I/O error occurs.
		/// </summary>
		public abstract long GetLastModifiedTime(File f);

		/// <summary>
		/// Return the length in bytes of the file denoted by the given abstract
		/// pathname, or zero if it does not exist, is a directory, or some other
		/// I/O error occurs.
		/// </summary>
		public abstract long GetLength(File f);


		/* -- File operations -- */

		/// <summary>
		/// Create a new empty file with the given pathname.  Return
		/// <code>true</code> if the file was created and <code>false</code> if a
		/// file or directory with the given pathname already exists.  Throw an
		/// IOException if an I/O error occurs.
		/// </summary>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public abstract boolean createFileExclusively(String pathname) throws IOException;
		public abstract bool CreateFileExclusively(String pathname);

		/// <summary>
		/// Delete the file or directory denoted by the given abstract pathname,
		/// returning <code>true</code> if and only if the operation succeeds.
		/// </summary>
		public abstract bool Delete(File f);

		/// <summary>
		/// List the elements of the directory denoted by the given abstract
		/// pathname.  Return an array of strings naming the elements of the
		/// directory if successful; otherwise, return <code>null</code>.
		/// </summary>
		public abstract String[] List(File f);

		/// <summary>
		/// Create a new directory denoted by the given abstract pathname,
		/// returning <code>true</code> if and only if the operation succeeds.
		/// </summary>
		public abstract bool CreateDirectory(File f);

		/// <summary>
		/// Rename the file or directory denoted by the first abstract pathname to
		/// the second abstract pathname, returning <code>true</code> if and only if
		/// the operation succeeds.
		/// </summary>
		public abstract bool Rename(File f1, File f2);

		/// <summary>
		/// Set the last-modified time of the file or directory denoted by the
		/// given abstract pathname, returning <code>true</code> if and only if the
		/// operation succeeds.
		/// </summary>
		public abstract bool SetLastModifiedTime(File f, long time);

		/// <summary>
		/// Mark the file or directory denoted by the given abstract pathname as
		/// read-only, returning <code>true</code> if and only if the operation
		/// succeeds.
		/// </summary>
		public abstract bool SetReadOnly(File f);


		/* -- Filesystem interface -- */

		/// <summary>
		/// List the available filesystem roots.
		/// </summary>
		public abstract File[] ListRoots();

		/* -- Disk usage -- */
//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Native public static final int SPACE_TOTAL = 0;
		public const int SPACE_TOTAL = 0;
//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Native public static final int SPACE_FREE = 1;
		public const int SPACE_FREE = 1;
//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Native public static final int SPACE_USABLE = 2;
		public const int SPACE_USABLE = 2;

		public abstract long GetSpace(File f, int t);

		/* -- Basic infrastructure -- */

		/// <summary>
		/// Compare two abstract pathnames lexicographically.
		/// </summary>
		public abstract int Compare(File f1, File f2);

		/// <summary>
		/// Compute the hash code of an abstract pathname.
		/// </summary>
		public abstract int HashCode(File f);

		// Flags for enabling/disabling performance optimizations for file
		// name canonicalization
		internal static bool UseCanonCaches = true;
		internal static bool UseCanonPrefixCache = true;

		private static bool GetBooleanProperty(String prop, bool defaultVal)
		{
			String val = System.getProperty(prop);
			if (val == null)
			{
				return defaultVal;
			}
			if (val.EqualsIgnoreCase("true"))
			{
				return true;
			}
			else
			{
				return false;
			}
		}

		static FileSystem()
		{
			UseCanonCaches = GetBooleanProperty("sun.io.useCanonCaches", UseCanonCaches);
			UseCanonPrefixCache = GetBooleanProperty("sun.io.useCanonPrefixCache", UseCanonPrefixCache);
		}
	}

}
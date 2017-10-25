using System.Runtime.InteropServices;

/*
 * Copyright (c) 2001, 2012, Oracle and/or its affiliates. All rights reserved.
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
	/// Unicode-aware FileSystem for Windows NT/2000.
	/// 
	/// @author Konstantin Kladko
	/// @since 1.4
	/// </summary>
	internal class WinNTFileSystem : FileSystem
	{

		private readonly char Slash;
		private readonly char AltSlash;
		private readonly char Semicolon;

		public WinNTFileSystem()
		{
			Slash = AccessController.doPrivileged(new GetPropertyAction("file.separator")).charAt(0);
			Semicolon = AccessController.doPrivileged(new GetPropertyAction("path.separator")).charAt(0);
			AltSlash = (this.Slash == '\\') ? '/' : '\\';
		}

		private bool IsSlash(char c)
		{
			return (c == '\\') || (c == '/');
		}

		private bool IsLetter(char c)
		{
			return ((c >= 'a') && (c <= 'z')) || ((c >= 'A') && (c <= 'Z'));
		}

		private String Slashify(String p)
		{
			if ((p.Length() > 0) && (p.CharAt(0) != Slash))
			{
				return Slash + p;
			}
			else
			{
				return p;
			}
		}

		/* -- Normalization and construction -- */

		public override char Separator
		{
			get
			{
				return Slash;
			}
		}

		public override char PathSeparator
		{
			get
			{
				return Semicolon;
			}
		}

		/* Check that the given pathname is normal.  If not, invoke the real
		   normalizer on the part of the pathname that requires normalization.
		   This way we iterate through the whole pathname string only once. */
		public override String Normalize(String path)
		{
			int n = path.Length();
			char slash = this.Slash;
			char altSlash = this.AltSlash;
			char prev = (char)0;
			for (int i = 0; i < n; i++)
			{
				char c = path.CharAt(i);
				if (c == altSlash)
				{
					return Normalize(path, n, (prev == slash) ? i - 1 : i);
				}
				if ((c == slash) && (prev == slash) && (i > 1))
				{
					return Normalize(path, n, i - 1);
				}
				if ((c == ':') && (i > 1))
				{
					return Normalize(path, n, 0);
				}
				prev = c;
			}
			if (prev == slash)
			{
				return Normalize(path, n, n - 1);
			}
			return path;
		}

		/* Normalize the given pathname, whose length is len, starting at the given
		   offset; everything before this offset is already normal. */
		private String Normalize(String path, int len, int off)
		{
			if (len == 0)
			{
				return path;
			}
			if (off < 3) // Avoid fencepost cases with UNC pathnames
			{
				off = 0;
			}
			int src;
			char slash = this.Slash;
			StringBuffer sb = new StringBuffer(len);

			if (off == 0)
			{
				/* Complete normalization, including prefix */
				src = NormalizePrefix(path, len, sb);
			}
			else
			{
				/* Partial normalization */
				src = off;
				sb.Append(path.Substring(0, off));
			}

			/* Remove redundant slashes from the remainder of the path, forcing all
			   slashes into the preferred slash */
			while (src < len)
			{
				char c = path.CharAt(src++);
				if (IsSlash(c))
				{
					while ((src < len) && IsSlash(path.CharAt(src)))
					{
						src++;
					}
					if (src == len)
					{
						/* Check for trailing separator */
						int sn = sb.Length();
						if ((sn == 2) && (sb.CharAt(1) == ':'))
						{
							/* "z:\\" */
							sb.Append(slash);
							break;
						}
						if (sn == 0)
						{
							/* "\\" */
							sb.Append(slash);
							break;
						}
						if ((sn == 1) && (IsSlash(sb.CharAt(0))))
						{
							/* "\\\\" is not collapsed to "\\" because "\\\\" marks
							   the beginning of a UNC pathname.  Even though it is
							   not, by itself, a valid UNC pathname, we leave it as
							   is in order to be consistent with the win32 APIs,
							   which treat this case as an invalid UNC pathname
							   rather than as an alias for the root directory of
							   the current drive. */
							sb.Append(slash);
							break;
						}
						/* Path does not denote a root directory, so do not append
						   trailing slash */
						break;
					}
					else
					{
						sb.Append(slash);
					}
				}
				else
				{
					sb.Append(c);
				}
			}

			String rv = sb.ToString();
			return rv;
		}

		/* A normal Win32 pathname contains no duplicate slashes, except possibly
		   for a UNC prefix, and does not end with a slash.  It may be the empty
		   string.  Normalized Win32 pathnames have the convenient property that
		   the length of the prefix almost uniquely identifies the type of the path
		   and whether it is absolute or relative:
	
		       0  relative to both drive and directory
		       1  drive-relative (begins with '\\')
		       2  absolute UNC (if first char is '\\'),
		            else directory-relative (has form "z:foo")
		       3  absolute local pathname (begins with "z:\\")
		 */
		private int NormalizePrefix(String path, int len, StringBuffer sb)
		{
			int src = 0;
			while ((src < len) && IsSlash(path.CharAt(src)))
			{
				src++;
			}
			char c;
			if ((len - src >= 2) && IsLetter(c = path.CharAt(src)) && path.CharAt(src + 1) == ':')
			{
				/* Remove leading slashes if followed by drive specifier.
				   This hack is necessary to support file URLs containing drive
				   specifiers (e.g., "file://c:/path").  As a side effect,
				   "/c:/path" can be used as an alternative to "c:/path". */
				sb.Append(c);
				sb.Append(':');
				src += 2;
			}
			else
			{
				src = 0;
				if ((len >= 2) && IsSlash(path.CharAt(0)) && IsSlash(path.CharAt(1)))
				{
					/* UNC pathname: Retain first slash; leave src pointed at
					   second slash so that further slashes will be collapsed
					   into the second slash.  The result will be a pathname
					   beginning with "\\\\" followed (most likely) by a host
					   name. */
					src = 1;
					sb.Append(Slash);
				}
			}
			return src;
		}

		public override int PrefixLength(String path)
		{
			char slash = this.Slash;
			int n = path.Length();
			if (n == 0)
			{
				return 0;
			}
			char c0 = path.CharAt(0);
			char c1 = (n > 1) ? path.CharAt(1) : 0;
			if (c0 == slash)
			{
				if (c1 == slash) // Absolute UNC pathname "\\\\foo"
				{
					return 2;
				}
				return 1; // Drive-relative "\\foo"
			}
			if (IsLetter(c0) && (c1 == ':'))
			{
				if ((n > 2) && (path.CharAt(2) == slash))
				{
					return 3; // Absolute local pathname "z:\\foo"
				}
				return 2; // Directory-relative "z:foo"
			}
			return 0; // Completely relative
		}

		public override String Resolve(String parent, String child)
		{
			int pn = parent.Length();
			if (pn == 0)
			{
				return child;
			}
			int cn = child.Length();
			if (cn == 0)
			{
				return parent;
			}

			String c = child;
			int childStart = 0;
			int parentEnd = pn;

			if ((cn > 1) && (c.CharAt(0) == Slash))
			{
				if (c.CharAt(1) == Slash)
				{
					/* Drop prefix when child is a UNC pathname */
					childStart = 2;
				}
				else
				{
					/* Drop prefix when child is drive-relative */
					childStart = 1;

				}
				if (cn == childStart) // Child is double slash
				{
					if (parent.CharAt(pn - 1) == Slash)
					{
						return parent.Substring(0, pn - 1);
					}
					return parent;
				}
			}

			if (parent.CharAt(pn - 1) == Slash)
			{
				parentEnd--;
			}

			int strlen = parentEnd + cn - childStart;
			char[] theChars = null;
			if (child.CharAt(childStart) == Slash)
			{
				theChars = new char[strlen];
				parent.GetChars(0, parentEnd, theChars, 0);
				child.GetChars(childStart, cn, theChars, parentEnd);
			}
			else
			{
				theChars = new char[strlen + 1];
				parent.GetChars(0, parentEnd, theChars, 0);
				theChars[parentEnd] = Slash;
				child.GetChars(childStart, cn, theChars, parentEnd + 1);
			}
			return new String(theChars);
		}

		public override String DefaultParent
		{
			get
			{
				return ("" + Slash);
			}
		}

		public override String FromURIPath(String path)
		{
			String p = path;
			if ((p.Length() > 2) && (p.CharAt(2) == ':'))
			{
				// "/c:/foo" --> "c:/foo"
				p = p.Substring(1);
				// "c:/foo/" --> "c:/foo", but "c:/" --> "c:/"
				if ((p.Length() > 3) && p.EndsWith("/"))
				{
					p = p.Substring(0, p.Length() - 1);
				}
			}
			else if ((p.Length() > 1) && p.EndsWith("/"))
			{
				// "/foo/" --> "/foo"
				p = p.Substring(0, p.Length() - 1);
			}
			return p;
		}

		/* -- Path operations -- */

		public override bool IsAbsolute(File f)
		{
			int pl = f.PrefixLength;
			return (((pl == 2) && (f.Path.CharAt(0) == Slash)) || (pl == 3));
		}

		public override String Resolve(File f)
		{
			String path = f.Path;
			int pl = f.PrefixLength;
			if ((pl == 2) && (path.CharAt(0) == Slash))
			{
				return path; // UNC
			}
			if (pl == 3)
			{
				return path; // Absolute local
			}
			if (pl == 0)
			{
				return UserPath + Slashify(path); // Completely relative
			}
			if (pl == 1) // Drive-relative
			{
				String up = UserPath;
				String ud = GetDrive(up);
				if (ud != null)
				{
					return ud + path;
				}
				return up + path; // User dir is a UNC path
			}
			if (pl == 2) // Directory-relative
			{
				String up = UserPath;
				String ud = GetDrive(up);
				if ((ud != null) && path.StartsWith(ud))
				{
					return up + Slashify(path.Substring(2));
				}
				char drive = path.CharAt(0);
				String dir = GetDriveDirectory(drive);
				String np;
				if (dir != null)
				{
					/* When resolving a directory-relative path that refers to a
					   drive other than the current drive, insist that the caller
					   have read permission on the result */
					String p = drive + (':' + dir + Slashify(path.Substring(2)));
					SecurityManager security = System.SecurityManager;
					try
					{
						if (security != null)
						{
							security.CheckRead(p);
						}
					}
					catch (SecurityException)
					{
						/* Don't disclose the drive's directory in the exception */
						throw new SecurityException("Cannot resolve path " + path);
					}
					return p;
				}
				return drive + ":" + Slashify(path.Substring(2)); // fake it
			}
			throw new InternalError("Unresolvable path: " + path);
		}

		private String UserPath
		{
			get
			{
				/* For both compatibility and security,
				   we must look this up every time */
				return Normalize(System.getProperty("user.dir"));
			}
		}

		private String GetDrive(String path)
		{
			int pl = PrefixLength(path);
			return (pl == 3) ? path.Substring(0, 2) : null;
		}

		private static String[] DriveDirCache = new String[26];

		private static int DriveIndex(char d)
		{
			if ((d >= 'a') && (d <= 'z'))
			{
				return d - 'a';
			}
			if ((d >= 'A') && (d <= 'Z'))
			{
				return d - 'A';
			}
			return -1;
		}

//JAVA TO C# CONVERTER TODO TASK: Replace 'unknown' with the appropriate dll name:
		[DllImport("unknown")]
		private extern String getDriveDirectory(int drive);

		private String GetDriveDirectory(char drive)
		{
			int i = DriveIndex(drive);
			if (i < 0)
			{
				return null;
			}
			String s = DriveDirCache[i];
			if (s != null)
			{
				return s;
			}
			s = GetDriveDirectory(i + 1);
			DriveDirCache[i] = s;
			return s;
		}

		// Caches for canonicalization results to improve startup performance.
		// The first cache handles repeated canonicalizations of the same path
		// name. The prefix cache handles repeated canonicalizations within the
		// same directory, and must not create results differing from the true
		// canonicalization algorithm in canonicalize_md.c. For this reason the
		// prefix cache is conservative and is not used for complex path names.
		private ExpiringCache Cache = new ExpiringCache();
		private ExpiringCache PrefixCache = new ExpiringCache();

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: @Override public String canonicalize(String path) throws IOException
		public override String Canonicalize(String path)
		{
			// If path is a drive letter only then skip canonicalization
			int len = path.Length();
			if ((len == 2) && (IsLetter(path.CharAt(0))) && (path.CharAt(1) == ':'))
			{
				char c = path.CharAt(0);
				if ((c >= 'A') && (c <= 'Z'))
				{
					return path;
				}
				return "" + ((char)(c - 32)) + ':';
			}
			else if ((len == 3) && (IsLetter(path.CharAt(0))) && (path.CharAt(1) == ':') && (path.CharAt(2) == '\\'))
			{
				char c = path.CharAt(0);
				if ((c >= 'A') && (c <= 'Z'))
				{
					return path;
				}
				return "" + ((char)(c - 32)) + ':' + '\\';
			}
			if (!UseCanonCaches)
			{
				return canonicalize0(path);
			}
			else
			{
				String res = Cache.Get(path);
				if (res == null)
				{
					String dir = null;
					String resDir = null;
					if (UseCanonPrefixCache)
					{
						dir = ParentOrNull(path);
						if (dir != null)
						{
							resDir = PrefixCache.Get(dir);
							if (resDir != null)
							{
								/*
								 * Hit only in prefix cache; full path is canonical,
								 * but we need to get the canonical name of the file
								 * in this directory to get the appropriate
								 * capitalization
								 */
								String filename = path.Substring(1 + dir.Length());
								res = CanonicalizeWithPrefix(resDir, filename);
								Cache.Put(dir + System.IO.Path.DirectorySeparatorChar + filename, res);
							}
						}
					}
					if (res == null)
					{
						res = canonicalize0(path);
						Cache.Put(path, res);
						if (UseCanonPrefixCache && dir != null)
						{
							resDir = ParentOrNull(res);
							if (resDir != null)
							{
								File f = new File(res);
								if (f.Exists() && !f.Directory)
								{
									PrefixCache.Put(dir, resDir);
								}
							}
						}
					}
				}
				return res;
			}
		}

//JAVA TO C# CONVERTER TODO TASK: Replace 'unknown' with the appropriate dll name:
		[DllImport("unknown")]
		private extern String canonicalize0(String path);

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: private String canonicalizeWithPrefix(String canonicalPrefix, String filename) throws IOException
		private String CanonicalizeWithPrefix(String canonicalPrefix, String filename)
		{
			return canonicalizeWithPrefix0(canonicalPrefix, canonicalPrefix + System.IO.Path.DirectorySeparatorChar + filename);
		}

		// Run the canonicalization operation assuming that the prefix
		// (everything up to the last filename) is canonical; just gets
		// the canonical name of the last element of the path
//JAVA TO C# CONVERTER TODO TASK: Replace 'unknown' with the appropriate dll name:
		[DllImport("unknown")]
		private extern String canonicalizeWithPrefix0(String canonicalPrefix, String pathWithCanonicalPrefix);

		// Best-effort attempt to get parent of this path; used for
		// optimization of filename canonicalization. This must return null for
		// any cases where the code in canonicalize_md.c would throw an
		// exception or otherwise deal with non-simple pathnames like handling
		// of "." and "..". It may conservatively return null in other
		// situations as well. Returning null will cause the underlying
		// (expensive) canonicalization routine to be called.
		private static String ParentOrNull(String path)
		{
			if (path == null)
			{
				return null;
			}
			char sep = System.IO.Path.DirectorySeparatorChar;
			char altSep = '/';
			int last = path.Length() - 1;
			int idx = last;
			int adjacentDots = 0;
			int nonDotCount = 0;
			while (idx > 0)
			{
				char c = path.CharAt(idx);
				if (c == '.')
				{
					if (++adjacentDots >= 2)
					{
						// Punt on pathnames containing . and ..
						return null;
					}
					if (nonDotCount == 0)
					{
						// Punt on pathnames ending in a .
						return null;
					}
				}
				else if (c == sep)
				{
					if (adjacentDots == 1 && nonDotCount == 0)
					{
						// Punt on pathnames containing . and ..
						return null;
					}
					if (idx == 0 || idx >= last - 1 || path.CharAt(idx - 1) == sep || path.CharAt(idx - 1) == altSep)
					{
						// Punt on pathnames containing adjacent slashes
						// toward the end
						return null;
					}
					return path.Substring(0, idx);
				}
				else if (c == altSep)
				{
					// Punt on pathnames containing both backward and
					// forward slashes
					return null;
				}
				else if (c == '*' || c == '?')
				{
					// Punt on pathnames containing wildcards
					return null;
				}
				else
				{
					++nonDotCount;
					adjacentDots = 0;
				}
				--idx;
			}
			return null;
		}

		/* -- Attribute accessors -- */

//JAVA TO C# CONVERTER TODO TASK: Replace 'unknown' with the appropriate dll name:
		[DllImport("unknown")]
		public override extern int getBooleanAttributes(File f);

//JAVA TO C# CONVERTER TODO TASK: Replace 'unknown' with the appropriate dll name:
		[DllImport("unknown")]
		public override extern boolean checkAccess(File f, int access);

//JAVA TO C# CONVERTER TODO TASK: Replace 'unknown' with the appropriate dll name:
		[DllImport("unknown")]
		public override extern long getLastModifiedTime(File f);

//JAVA TO C# CONVERTER TODO TASK: Replace 'unknown' with the appropriate dll name:
		[DllImport("unknown")]
		public override extern long getLength(File f);

//JAVA TO C# CONVERTER TODO TASK: Replace 'unknown' with the appropriate dll name:
		[DllImport("unknown")]
		public override extern boolean setPermission(File f, int access, bool enable, bool owneronly);

		/* -- File operations -- */

//JAVA TO C# CONVERTER TODO TASK: Replace 'unknown' with the appropriate dll name:
		[DllImport("unknown")]
		public override extern boolean createFileExclusively(String path);

//JAVA TO C# CONVERTER TODO TASK: Replace 'unknown' with the appropriate dll name:
		[DllImport("unknown")]
		public override extern String[] list(File f);

//JAVA TO C# CONVERTER TODO TASK: Replace 'unknown' with the appropriate dll name:
		[DllImport("unknown")]
		public override extern boolean createDirectory(File f);

//JAVA TO C# CONVERTER TODO TASK: Replace 'unknown' with the appropriate dll name:
		[DllImport("unknown")]
		public override extern boolean setLastModifiedTime(File f, long time);

//JAVA TO C# CONVERTER TODO TASK: Replace 'unknown' with the appropriate dll name:
		[DllImport("unknown")]
		public override extern boolean setReadOnly(File f);

		public override bool Delete(File f)
		{
			// Keep canonicalization caches in sync after file deletion
			// and renaming operations. Could be more clever than this
			// (i.e., only remove/update affected entries) but probably
			// not worth it since these entries expire after 30 seconds
			// anyway.
			Cache.Clear();
			PrefixCache.Clear();
			return delete0(f);
		}

//JAVA TO C# CONVERTER TODO TASK: Replace 'unknown' with the appropriate dll name:
		[DllImport("unknown")]
		private extern boolean delete0(File f);

		public override bool Rename(File f1, File f2)
		{
			// Keep canonicalization caches in sync after file deletion
			// and renaming operations. Could be more clever than this
			// (i.e., only remove/update affected entries) but probably
			// not worth it since these entries expire after 30 seconds
			// anyway.
			Cache.Clear();
			PrefixCache.Clear();
			return rename0(f1, f2);
		}

//JAVA TO C# CONVERTER TODO TASK: Replace 'unknown' with the appropriate dll name:
		[DllImport("unknown")]
		private extern boolean rename0(File f1, File f2);

		/* -- Filesystem interface -- */

		public override File[] ListRoots()
		{
			int ds = listRoots0();
			int n = 0;
			for (int i = 0; i < 26; i++)
			{
				if (((ds >> i) & 1) != 0)
				{
					if (!Access((char)('A' + i) + ":" + Slash))
					{
						ds &= ~(1 << i);
					}
					else
					{
						n++;
					}
				}
			}
			File[] fs = new File[n];
			int j = 0;
			char slash = this.Slash;
			for (int i = 0; i < 26; i++)
			{
				if (((ds >> i) & 1) != 0)
				{
					fs[j++] = new File((char)('A' + i) + ":" + slash);
				}
			}
			return fs;
		}

//JAVA TO C# CONVERTER TODO TASK: Replace 'unknown' with the appropriate dll name:
		[DllImport("unknown")]
		private static extern int listRoots0();

		private bool Access(String path)
		{
			try
			{
				SecurityManager security = System.SecurityManager;
				if (security != null)
				{
					security.CheckRead(path);
				}
				return true;
			}
			catch (SecurityException)
			{
				return false;
			}
		}

		/* -- Disk usage -- */

		public override long GetSpace(File f, int t)
		{
			if (f.Exists())
			{
				return getSpace0(f, t);
			}
			return 0;
		}

//JAVA TO C# CONVERTER TODO TASK: Replace 'unknown' with the appropriate dll name:
		[DllImport("unknown")]
		private extern long getSpace0(File f, int t);

		/* -- Basic infrastructure -- */

		public override int Compare(File f1, File f2)
		{
			return f1.Path.CompareToIgnoreCase(f2.Path);
		}

		public override int HashCode(File f)
		{
			/* Could make this more efficient: String.hashCodeIgnoreCase */
			return f.Path.ToLowerCase(Locale.ENGLISH).HashCode() ^ 1234321;
		}

//JAVA TO C# CONVERTER TODO TASK: Replace 'unknown' with the appropriate dll name:
		[DllImport("unknown")]
		private static extern void initIDs();

		static WinNTFileSystem()
		{
			initIDs();
		}
	}

}
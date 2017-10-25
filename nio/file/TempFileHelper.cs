using System;

/*
 * Copyright (c) 2009, 2013, Oracle and/or its affiliates. All rights reserved.
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

	using GetPropertyAction = sun.security.action.GetPropertyAction;


	/// <summary>
	/// Helper class to support creation of temporary files and directories with
	/// initial attributes.
	/// </summary>

	internal class TempFileHelper
	{
		private TempFileHelper()
		{
		}

		// temporary directory location
		private static readonly Path Tmpdir = Paths.Get(doPrivileged(new GetPropertyAction("java.io.tmpdir")));

		private static readonly bool IsPosix = FileSystems.Default.SupportedFileAttributeViews().Contains("posix");

		// file name generation, same as java.io.File for now
		private static readonly SecureRandom Random = new SecureRandom();
		private static Path GeneratePath(String prefix, String suffix, Path dir)
		{
			long n = Random.NextLong();
			n = (n == Long.MinValue) ? 0 : System.Math.Abs(n);
			Path name = dir.FileSystem.getPath(prefix + Convert.ToString(n) + suffix);
			// the generated name should be a simple file name
			if (name.Parent != null)
			{
				throw new IllegalArgumentException("Invalid prefix or suffix");
			}
			return dir.Resolve(name);
		}

		// default file and directory permissions (lazily initialized)
		private class PosixPermissions
		{
			internal static readonly FileAttribute<Set<PosixFilePermission>> FilePermissions = PosixFilePermissions.AsFileAttribute(EnumSet.Of(OWNER_READ, OWNER_WRITE));
			internal static readonly FileAttribute<Set<PosixFilePermission>> DirPermissions = PosixFilePermissions.AsFileAttribute(EnumSet.Of(OWNER_READ, OWNER_WRITE, OWNER_EXECUTE));
		}

		/// <summary>
		/// Creates a file or directory in in the given given directory (or in the
		/// temporary directory if dir is {@code null}).
		/// </summary>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: private static Path create(Path dir, String prefix, String suffix, boolean createDirectory, java.nio.file.attribute.FileAttribute<?>[] attrs) throws java.io.IOException
		private static Path create<T1>(Path dir, String prefix, String suffix, bool createDirectory, FileAttribute<T1>[] attrs)
		{
			if (prefix == null)
			{
				prefix = "";
			}
			if (suffix == null)
			{
				suffix = (createDirectory) ? "" : ".tmp";
			}
			if (dir == null)
			{
				dir = Tmpdir;
			}

			// in POSIX environments use default file and directory permissions
			// if initial permissions not given by caller.
			if (IsPosix && (dir.FileSystem == FileSystems.Default))
			{
				if (attrs.Length == 0)
				{
					// no attributes so use default permissions
//JAVA TO C# CONVERTER TODO TASK: Java wildcard generics are not converted to .NET:
//ORIGINAL LINE: attrs = new java.nio.file.attribute.FileAttribute<?>[1];
					attrs = new FileAttribute<?>[1];
					attrs[0] = (createDirectory) ? PosixPermissions.DirPermissions : PosixPermissions.FilePermissions;
				}
				else
				{
					// check if posix permissions given; if not use default
					bool hasPermissions = false;
					for (int i = 0; i < attrs.Length; i++)
					{
						if (attrs[i].Name().Equals("posix:permissions"))
						{
							hasPermissions = true;
							break;
						}
					}
					if (!hasPermissions)
					{
//JAVA TO C# CONVERTER TODO TASK: Java wildcard generics are not converted to .NET:
//ORIGINAL LINE: java.nio.file.attribute.FileAttribute<?>[] copy = new java.nio.file.attribute.FileAttribute<?>[attrs.length+1];
						FileAttribute<?>[] copy = new FileAttribute<?>[attrs.Length + 1];
						System.Array.Copy(attrs, 0, copy, 0, attrs.Length);
						attrs = copy;
						attrs[attrs.Length - 1] = (createDirectory) ? PosixPermissions.DirPermissions : PosixPermissions.FilePermissions;
					}
				}
			}

			// loop generating random names until file or directory can be created
			SecurityManager sm = System.SecurityManager;
			for (;;)
			{
				Path f;
				try
				{
					f = GeneratePath(prefix, suffix, dir);
				}
				catch (InvalidPathException e)
				{
					// don't reveal temporary directory location
					if (sm != null)
					{
						throw new IllegalArgumentException("Invalid prefix or suffix");
					}
					throw e;
				}
				try
				{
					if (createDirectory)
					{
						return Files.CreateDirectory(f, attrs);
					}
					else
					{
						return Files.CreateFile(f, attrs);
					}
				}
				catch (SecurityException e)
				{
					// don't reveal temporary directory location
					if (dir == Tmpdir && sm != null)
					{
						throw new SecurityException("Unable to create temporary file or directory");
					}
					throw e;
				}
				catch (FileAlreadyExistsException)
				{
					// ignore
				}
			}
		}

		/// <summary>
		/// Creates a temporary file in the given directory, or in in the
		/// temporary directory if dir is {@code null}.
		/// </summary>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: static Path createTempFile(Path dir, String prefix, String suffix, java.nio.file.attribute.FileAttribute<?>[] attrs) throws java.io.IOException
		internal static Path createTempFile<T1>(Path dir, String prefix, String suffix, FileAttribute<T1>[] attrs)
		{
			return Create(dir, prefix, suffix, false, attrs);
		}

		/// <summary>
		/// Creates a temporary directory in the given directory, or in in the
		/// temporary directory if dir is {@code null}.
		/// </summary>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: static Path createTempDirectory(Path dir, String prefix, java.nio.file.attribute.FileAttribute<?>[] attrs) throws java.io.IOException
		internal static Path createTempDirectory<T1>(Path dir, String prefix, FileAttribute<T1>[] attrs)
		{
			return Create(dir, prefix, null, true, attrs);
		}
	}

}
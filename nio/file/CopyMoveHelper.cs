/*
 * Copyright (c) 2011, Oracle and/or its affiliates. All rights reserved.
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
	/// Helper class to support copying or moving files when the source and target
	/// are associated with different providers.
	/// </summary>

	internal class CopyMoveHelper
	{
		private CopyMoveHelper()
		{
		}

		/// <summary>
		/// Parses the arguments for a file copy operation.
		/// </summary>
		private class CopyOptions
		{
			internal bool ReplaceExisting = false;
			internal bool CopyAttributes = false;
			internal bool FollowLinks = true;

			internal CopyOptions()
			{
			}

			internal static CopyOptions Parse(params CopyOption[] options)
			{
				CopyOptions result = new CopyOptions();
				foreach (CopyOption option in options)
				{
					if (option == StandardCopyOption.REPLACE_EXISTING)
					{
						result.ReplaceExisting = true;
						continue;
					}
					if (option == LinkOption.NOFOLLOW_LINKS)
					{
						result.FollowLinks = false;
						continue;
					}
					if (option == StandardCopyOption.COPY_ATTRIBUTES)
					{
						result.CopyAttributes = true;
						continue;
					}
					if (option == null)
					{
						throw new NullPointerException();
					}
					throw new UnsupportedOperationException("'" + option + "' is not a recognized copy option");
				}
				return result;
			}
		}

		/// <summary>
		/// Converts the given array of options for moving a file to options suitable
		/// for copying the file when a move is implemented as copy + delete.
		/// </summary>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: private static CopyOption[] convertMoveToCopyOptions(CopyOption... options) throws AtomicMoveNotSupportedException
		private static CopyOption[] ConvertMoveToCopyOptions(params CopyOption[] options)
		{
			int len = options.Length;
			CopyOption[] newOptions = new CopyOption[len + 2];
			for (int i = 0; i < len; i++)
			{
				CopyOption option = options[i];
				if (option == StandardCopyOption.ATOMIC_MOVE)
				{
					throw new AtomicMoveNotSupportedException(null, null, "Atomic move between providers is not supported");
				}
				newOptions[i] = option;
			}
			newOptions[len] = LinkOption.NOFOLLOW_LINKS;
			newOptions[len + 1] = StandardCopyOption.COPY_ATTRIBUTES;
			return newOptions;
		}

		/// <summary>
		/// Simple copy for use when source and target are associated with different
		/// providers
		/// </summary>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: static void copyToForeignTarget(Path source, Path target, CopyOption... options) throws java.io.IOException
		internal static void CopyToForeignTarget(Path source, Path target, params CopyOption[] options)
		{
			CopyOptions opts = CopyOptions.Parse(options);
			LinkOption[] linkOptions = (opts.FollowLinks) ? new LinkOption[0] : new LinkOption[] {LinkOption.NOFOLLOW_LINKS};

			// attributes of source file
			BasicFileAttributes attrs = Files.ReadAttributes(source, typeof(BasicFileAttributes), linkOptions);
			if (attrs.SymbolicLink)
			{
				throw new IOException("Copying of symbolic links not supported");
			}

			// delete target if it exists and REPLACE_EXISTING is specified
			if (opts.ReplaceExisting)
			{
				Files.DeleteIfExists(target);
			}
			else if (Files.exists(target))
			{
				throw new FileAlreadyExistsException(target.ToString());
			}

			// create directory or copy file
			if (attrs.Directory)
			{
				Files.createDirectory(target);
			}
			else
			{
				using (InputStream @in = Files.newInputStream(source))
				{
					Files.Copy(@in, target);
				}
			}

			// copy basic attributes to target
			if (opts.CopyAttributes)
			{
				BasicFileAttributeView view = Files.getFileAttributeView(target, typeof(BasicFileAttributeView));
				try
				{
					view.SetTimes(attrs.LastModifiedTime(), attrs.LastAccessTime(), attrs.CreationTime());
				}
				catch (Throwable x)
				{
					// rollback
					try
					{
						Files.Delete(target);
					}
					catch (Throwable suppressed)
					{
						x.AddSuppressed(suppressed);
					}
					throw x;
				}
			}
		}

		/// <summary>
		/// Simple move implements as copy+delete for use when source and target are
		/// associated with different providers
		/// </summary>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: static void moveToForeignTarget(Path source, Path target, CopyOption... options) throws java.io.IOException
		internal static void MoveToForeignTarget(Path source, Path target, params CopyOption[] options)
		{
			CopyToForeignTarget(source, target, ConvertMoveToCopyOptions(options));
			Files.Delete(source);
		}
	}

}
/*
 * Copyright (c) 2007, 2011, Oracle and/or its affiliates. All rights reserved.
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
	/// A simple visitor of files with default behavior to visit all files and to
	/// re-throw I/O errors.
	/// 
	/// <para> Methods in this class may be overridden subject to their general contract.
	/// 
	/// </para>
	/// </summary>
	/// @param   <T>     The type of reference to the files
	/// 
	/// @since 1.7 </param>

	public class SimpleFileVisitor<T> : FileVisitor<T>
	{
		/// <summary>
		/// Initializes a new instance of this class.
		/// </summary>
		protected internal SimpleFileVisitor()
		{
		}

		/// <summary>
		/// Invoked for a directory before entries in the directory are visited.
		/// 
		/// <para> Unless overridden, this method returns {@link FileVisitResult#CONTINUE
		/// CONTINUE}.
		/// </para>
		/// </summary>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: @Override public FileVisitResult preVisitDirectory(T dir, java.nio.file.attribute.BasicFileAttributes attrs) throws java.io.IOException
		public override FileVisitResult PreVisitDirectory(T dir, BasicFileAttributes attrs)
		{
			Objects.RequireNonNull(dir);
			Objects.RequireNonNull(attrs);
			return FileVisitResult.CONTINUE;
		}

		/// <summary>
		/// Invoked for a file in a directory.
		/// 
		/// <para> Unless overridden, this method returns {@link FileVisitResult#CONTINUE
		/// CONTINUE}.
		/// </para>
		/// </summary>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: @Override public FileVisitResult visitFile(T file, java.nio.file.attribute.BasicFileAttributes attrs) throws java.io.IOException
		public override FileVisitResult VisitFile(T file, BasicFileAttributes attrs)
		{
			Objects.RequireNonNull(file);
			Objects.RequireNonNull(attrs);
			return FileVisitResult.CONTINUE;
		}

		/// <summary>
		/// Invoked for a file that could not be visited.
		/// 
		/// <para> Unless overridden, this method re-throws the I/O exception that prevented
		/// the file from being visited.
		/// </para>
		/// </summary>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: @Override public FileVisitResult visitFileFailed(T file, java.io.IOException exc) throws java.io.IOException
		public override FileVisitResult VisitFileFailed(T file, IOException exc)
		{
			Objects.RequireNonNull(file);
			throw exc;
		}

		/// <summary>
		/// Invoked for a directory after entries in the directory, and all of their
		/// descendants, have been visited.
		/// 
		/// <para> Unless overridden, this method returns {@link FileVisitResult#CONTINUE
		/// CONTINUE} if the directory iteration completes without an I/O exception;
		/// otherwise this method re-throws the I/O exception that caused the iteration
		/// of the directory to terminate prematurely.
		/// </para>
		/// </summary>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: @Override public FileVisitResult postVisitDirectory(T dir, java.io.IOException exc) throws java.io.IOException
		public override FileVisitResult PostVisitDirectory(T dir, IOException exc)
		{
			Objects.RequireNonNull(dir);
			if (exc != null)
			{
				throw exc;
			}
			return FileVisitResult.CONTINUE;
		}
	}

}
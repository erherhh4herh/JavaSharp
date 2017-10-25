/*
 * Copyright (c) 1996, 2001, Oracle and/or its affiliates. All rights reserved.
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
	/// Convenience class for writing character files.  The constructors of this
	/// class assume that the default character encoding and the default byte-buffer
	/// size are acceptable.  To specify these values yourself, construct an
	/// OutputStreamWriter on a FileOutputStream.
	/// 
	/// <para>Whether or not a file is available or may be created depends upon the
	/// underlying platform.  Some platforms, in particular, allow a file to be
	/// opened for writing by only one <tt>FileWriter</tt> (or other file-writing
	/// object) at a time.  In such situations the constructors in this class
	/// will fail if the file involved is already open.
	/// 
	/// </para>
	/// <para><code>FileWriter</code> is meant for writing streams of characters.
	/// For writing streams of raw bytes, consider using a
	/// <code>FileOutputStream</code>.
	/// 
	/// </para>
	/// </summary>
	/// <seealso cref= OutputStreamWriter </seealso>
	/// <seealso cref= FileOutputStream
	/// 
	/// @author      Mark Reinhold
	/// @since       JDK1.1 </seealso>

	public class FileWriter : OutputStreamWriter
	{

		/// <summary>
		/// Constructs a FileWriter object given a file name.
		/// </summary>
		/// <param name="fileName">  String The system-dependent filename. </param>
		/// <exception cref="IOException">  if the named file exists but is a directory rather
		///                  than a regular file, does not exist but cannot be
		///                  created, or cannot be opened for any other reason </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public FileWriter(String fileName) throws IOException
		public FileWriter(String fileName) : base(new FileOutputStream(fileName))
		{
		}

		/// <summary>
		/// Constructs a FileWriter object given a file name with a boolean
		/// indicating whether or not to append the data written.
		/// </summary>
		/// <param name="fileName">  String The system-dependent filename. </param>
		/// <param name="append">    boolean if <code>true</code>, then data will be written
		///                  to the end of the file rather than the beginning. </param>
		/// <exception cref="IOException">  if the named file exists but is a directory rather
		///                  than a regular file, does not exist but cannot be
		///                  created, or cannot be opened for any other reason </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public FileWriter(String fileName, boolean append) throws IOException
		public FileWriter(String fileName, bool append) : base(new FileOutputStream(fileName, append))
		{
		}

		/// <summary>
		/// Constructs a FileWriter object given a File object.
		/// </summary>
		/// <param name="file">  a File object to write to. </param>
		/// <exception cref="IOException">  if the file exists but is a directory rather than
		///                  a regular file, does not exist but cannot be created,
		///                  or cannot be opened for any other reason </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public FileWriter(File file) throws IOException
		public FileWriter(File file) : base(new FileOutputStream(file))
		{
		}

		/// <summary>
		/// Constructs a FileWriter object given a File object. If the second
		/// argument is <code>true</code>, then bytes will be written to the end
		/// of the file rather than the beginning.
		/// </summary>
		/// <param name="file">  a File object to write to </param>
		/// <param name="append">    if <code>true</code>, then bytes will be written
		///                      to the end of the file rather than the beginning </param>
		/// <exception cref="IOException">  if the file exists but is a directory rather than
		///                  a regular file, does not exist but cannot be created,
		///                  or cannot be opened for any other reason
		/// @since 1.4 </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public FileWriter(File file, boolean append) throws IOException
		public FileWriter(File file, bool append) : base(new FileOutputStream(file, append))
		{
		}

		/// <summary>
		/// Constructs a FileWriter object associated with a file descriptor.
		/// </summary>
		/// <param name="fd">  FileDescriptor object to write to. </param>
		public FileWriter(FileDescriptor fd) : base(new FileOutputStream(fd))
		{
		}

	}

}
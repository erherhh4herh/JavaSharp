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
	/// Convenience class for reading character files.  The constructors of this
	/// class assume that the default character encoding and the default byte-buffer
	/// size are appropriate.  To specify these values yourself, construct an
	/// InputStreamReader on a FileInputStream.
	/// 
	/// <para><code>FileReader</code> is meant for reading streams of characters.
	/// For reading streams of raw bytes, consider using a
	/// <code>FileInputStream</code>.
	/// 
	/// </para>
	/// </summary>
	/// <seealso cref= InputStreamReader </seealso>
	/// <seealso cref= FileInputStream
	/// 
	/// @author      Mark Reinhold
	/// @since       JDK1.1 </seealso>
	public class FileReader : InputStreamReader
	{

	   /// <summary>
	   /// Creates a new <tt>FileReader</tt>, given the name of the
	   /// file to read from.
	   /// </summary>
	   /// <param name="fileName"> the name of the file to read from </param>
	   /// <exception cref="FileNotFoundException">  if the named file does not exist,
	   ///                   is a directory rather than a regular file,
	   ///                   or for some other reason cannot be opened for
	   ///                   reading. </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public FileReader(String fileName) throws FileNotFoundException
		public FileReader(String fileName) : base(new FileInputStream(fileName))
		{
		}

	   /// <summary>
	   /// Creates a new <tt>FileReader</tt>, given the <tt>File</tt>
	   /// to read from.
	   /// </summary>
	   /// <param name="file"> the <tt>File</tt> to read from </param>
	   /// <exception cref="FileNotFoundException">  if the file does not exist,
	   ///                   is a directory rather than a regular file,
	   ///                   or for some other reason cannot be opened for
	   ///                   reading. </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public FileReader(File file) throws FileNotFoundException
		public FileReader(File file) : base(new FileInputStream(file))
		{
		}

	   /// <summary>
	   /// Creates a new <tt>FileReader</tt>, given the
	   /// <tt>FileDescriptor</tt> to read from.
	   /// </summary>
	   /// <param name="fd"> the FileDescriptor to read from </param>
		public FileReader(FileDescriptor fd) : base(new FileInputStream(fd))
		{
		}

	}

}
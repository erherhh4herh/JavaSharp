using System.Runtime.InteropServices;

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

	using FileChannelImpl = sun.nio.ch.FileChannelImpl;


	/// <summary>
	/// A file output stream is an output stream for writing data to a
	/// <code>File</code> or to a <code>FileDescriptor</code>. Whether or not
	/// a file is available or may be created depends upon the underlying
	/// platform.  Some platforms, in particular, allow a file to be opened
	/// for writing by only one <tt>FileOutputStream</tt> (or other
	/// file-writing object) at a time.  In such situations the constructors in
	/// this class will fail if the file involved is already open.
	/// 
	/// <para><code>FileOutputStream</code> is meant for writing streams of raw bytes
	/// such as image data. For writing streams of characters, consider using
	/// <code>FileWriter</code>.
	/// 
	/// @author  Arthur van Hoff
	/// </para>
	/// </summary>
	/// <seealso cref=     java.io.File </seealso>
	/// <seealso cref=     java.io.FileDescriptor </seealso>
	/// <seealso cref=     java.io.FileInputStream </seealso>
	/// <seealso cref=     java.nio.file.Files#newOutputStream
	/// @since   JDK1.0 </seealso>
	public class FileOutputStream : OutputStream
	{
		/// <summary>
		/// The system dependent file descriptor.
		/// </summary>
		private readonly FileDescriptor Fd;

		/// <summary>
		/// True if the file is opened for append.
		/// </summary>
		private readonly bool Append;

		/// <summary>
		/// The associated channel, initialized lazily.
		/// </summary>
		private FileChannel Channel_Renamed;

		/// <summary>
		/// The path of the referenced file
		/// (null if the stream is created with a file descriptor)
		/// </summary>
		private readonly String Path;

		private readonly Object CloseLock = new Object();
		private volatile bool Closed = false;

		/// <summary>
		/// Creates a file output stream to write to the file with the
		/// specified name. A new <code>FileDescriptor</code> object is
		/// created to represent this file connection.
		/// <para>
		/// First, if there is a security manager, its <code>checkWrite</code>
		/// method is called with <code>name</code> as its argument.
		/// </para>
		/// <para>
		/// If the file exists but is a directory rather than a regular file, does
		/// not exist but cannot be created, or cannot be opened for any other
		/// reason then a <code>FileNotFoundException</code> is thrown.
		/// 
		/// </para>
		/// </summary>
		/// <param name="name">   the system-dependent filename </param>
		/// <exception cref="FileNotFoundException">  if the file exists but is a directory
		///                   rather than a regular file, does not exist but cannot
		///                   be created, or cannot be opened for any other reason </exception>
		/// <exception cref="SecurityException">  if a security manager exists and its
		///               <code>checkWrite</code> method denies write access
		///               to the file. </exception>
		/// <seealso cref=        java.lang.SecurityManager#checkWrite(java.lang.String) </seealso>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public FileOutputStream(String name) throws FileNotFoundException
		public FileOutputStream(String name) : this(name != null ? new File(name) : null, false)
		{
		}

		/// <summary>
		/// Creates a file output stream to write to the file with the specified
		/// name.  If the second argument is <code>true</code>, then
		/// bytes will be written to the end of the file rather than the beginning.
		/// A new <code>FileDescriptor</code> object is created to represent this
		/// file connection.
		/// <para>
		/// First, if there is a security manager, its <code>checkWrite</code>
		/// method is called with <code>name</code> as its argument.
		/// </para>
		/// <para>
		/// If the file exists but is a directory rather than a regular file, does
		/// not exist but cannot be created, or cannot be opened for any other
		/// reason then a <code>FileNotFoundException</code> is thrown.
		/// 
		/// </para>
		/// </summary>
		/// <param name="name">        the system-dependent file name </param>
		/// <param name="append">      if <code>true</code>, then bytes will be written
		///                   to the end of the file rather than the beginning </param>
		/// <exception cref="FileNotFoundException">  if the file exists but is a directory
		///                   rather than a regular file, does not exist but cannot
		///                   be created, or cannot be opened for any other reason. </exception>
		/// <exception cref="SecurityException">  if a security manager exists and its
		///               <code>checkWrite</code> method denies write access
		///               to the file. </exception>
		/// <seealso cref=        java.lang.SecurityManager#checkWrite(java.lang.String)
		/// @since     JDK1.1 </seealso>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public FileOutputStream(String name, boolean append) throws FileNotFoundException
		public FileOutputStream(String name, bool append) : this(name != null ? new File(name) : null, append)
		{
		}

		/// <summary>
		/// Creates a file output stream to write to the file represented by
		/// the specified <code>File</code> object. A new
		/// <code>FileDescriptor</code> object is created to represent this
		/// file connection.
		/// <para>
		/// First, if there is a security manager, its <code>checkWrite</code>
		/// method is called with the path represented by the <code>file</code>
		/// argument as its argument.
		/// </para>
		/// <para>
		/// If the file exists but is a directory rather than a regular file, does
		/// not exist but cannot be created, or cannot be opened for any other
		/// reason then a <code>FileNotFoundException</code> is thrown.
		/// 
		/// </para>
		/// </summary>
		/// <param name="file">               the file to be opened for writing. </param>
		/// <exception cref="FileNotFoundException">  if the file exists but is a directory
		///                   rather than a regular file, does not exist but cannot
		///                   be created, or cannot be opened for any other reason </exception>
		/// <exception cref="SecurityException">  if a security manager exists and its
		///               <code>checkWrite</code> method denies write access
		///               to the file. </exception>
		/// <seealso cref=        java.io.File#getPath() </seealso>
		/// <seealso cref=        java.lang.SecurityException </seealso>
		/// <seealso cref=        java.lang.SecurityManager#checkWrite(java.lang.String) </seealso>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public FileOutputStream(File file) throws FileNotFoundException
		public FileOutputStream(File file) : this(file, false)
		{
		}

		/// <summary>
		/// Creates a file output stream to write to the file represented by
		/// the specified <code>File</code> object. If the second argument is
		/// <code>true</code>, then bytes will be written to the end of the file
		/// rather than the beginning. A new <code>FileDescriptor</code> object is
		/// created to represent this file connection.
		/// <para>
		/// First, if there is a security manager, its <code>checkWrite</code>
		/// method is called with the path represented by the <code>file</code>
		/// argument as its argument.
		/// </para>
		/// <para>
		/// If the file exists but is a directory rather than a regular file, does
		/// not exist but cannot be created, or cannot be opened for any other
		/// reason then a <code>FileNotFoundException</code> is thrown.
		/// 
		/// </para>
		/// </summary>
		/// <param name="file">               the file to be opened for writing. </param>
		/// <param name="append">      if <code>true</code>, then bytes will be written
		///                   to the end of the file rather than the beginning </param>
		/// <exception cref="FileNotFoundException">  if the file exists but is a directory
		///                   rather than a regular file, does not exist but cannot
		///                   be created, or cannot be opened for any other reason </exception>
		/// <exception cref="SecurityException">  if a security manager exists and its
		///               <code>checkWrite</code> method denies write access
		///               to the file. </exception>
		/// <seealso cref=        java.io.File#getPath() </seealso>
		/// <seealso cref=        java.lang.SecurityException </seealso>
		/// <seealso cref=        java.lang.SecurityManager#checkWrite(java.lang.String)
		/// @since 1.4 </seealso>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public FileOutputStream(File file, boolean append) throws FileNotFoundException
		public FileOutputStream(File file, bool append)
		{
			String name = (file != null ? file.Path : null);
			SecurityManager security = System.SecurityManager;
			if (security != null)
			{
				security.CheckWrite(name);
			}
			if (name == null)
			{
				throw new NullPointerException();
			}
			if (file.Invalid)
			{
				throw new FileNotFoundException("Invalid file path");
			}
			this.Fd = new FileDescriptor();
			Fd.Attach(this);
			this.Append = append;
			this.Path = name;

			Open(name, append);
		}

		/// <summary>
		/// Creates a file output stream to write to the specified file
		/// descriptor, which represents an existing connection to an actual
		/// file in the file system.
		/// <para>
		/// First, if there is a security manager, its <code>checkWrite</code>
		/// method is called with the file descriptor <code>fdObj</code>
		/// argument as its argument.
		/// </para>
		/// <para>
		/// If <code>fdObj</code> is null then a <code>NullPointerException</code>
		/// is thrown.
		/// </para>
		/// <para>
		/// This constructor does not throw an exception if <code>fdObj</code>
		/// is <seealso cref="java.io.FileDescriptor#valid() invalid"/>.
		/// However, if the methods are invoked on the resulting stream to attempt
		/// I/O on the stream, an <code>IOException</code> is thrown.
		/// 
		/// </para>
		/// </summary>
		/// <param name="fdObj">   the file descriptor to be opened for writing </param>
		/// <exception cref="SecurityException">  if a security manager exists and its
		///               <code>checkWrite</code> method denies
		///               write access to the file descriptor </exception>
		/// <seealso cref=        java.lang.SecurityManager#checkWrite(java.io.FileDescriptor) </seealso>
		public FileOutputStream(FileDescriptor fdObj)
		{
			SecurityManager security = System.SecurityManager;
			if (fdObj == null)
			{
				throw new NullPointerException();
			}
			if (security != null)
			{
				security.CheckWrite(fdObj);
			}
			this.Fd = fdObj;
			this.Append = false;
			this.Path = null;

			Fd.Attach(this);
		}

		/// <summary>
		/// Opens a file, with the specified name, for overwriting or appending. </summary>
		/// <param name="name"> name of file to be opened </param>
		/// <param name="append"> whether the file is to be opened in append mode </param>
//JAVA TO C# CONVERTER TODO TASK: Replace 'unknown' with the appropriate dll name:
		[DllImport("unknown")]
		private extern void open0(String name, bool append);

		// wrap native call to allow instrumentation
		/// <summary>
		/// Opens a file, with the specified name, for overwriting or appending. </summary>
		/// <param name="name"> name of file to be opened </param>
		/// <param name="append"> whether the file is to be opened in append mode </param>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: private void open(String name, boolean append) throws FileNotFoundException
		private void Open(String name, bool append)
		{
			open0(name, append);
		}

		/// <summary>
		/// Writes the specified byte to this file output stream.
		/// </summary>
		/// <param name="b">   the byte to be written. </param>
		/// <param name="append">   {@code true} if the write operation first
		///     advances the position to the end of file </param>
//JAVA TO C# CONVERTER TODO TASK: Replace 'unknown' with the appropriate dll name:
		[DllImport("unknown")]
		private extern void write(int b, bool append);

		/// <summary>
		/// Writes the specified byte to this file output stream. Implements
		/// the <code>write</code> method of <code>OutputStream</code>.
		/// </summary>
		/// <param name="b">   the byte to be written. </param>
		/// <exception cref="IOException">  if an I/O error occurs. </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public void write(int b) throws IOException
		public override void Write(int b)
		{
			write(b, Append);
		}

		/// <summary>
		/// Writes a sub array as a sequence of bytes. </summary>
		/// <param name="b"> the data to be written </param>
		/// <param name="off"> the start offset in the data </param>
		/// <param name="len"> the number of bytes that are written </param>
		/// <param name="append"> {@code true} to first advance the position to the
		///     end of file </param>
		/// <exception cref="IOException"> If an I/O error has occurred. </exception>
//JAVA TO C# CONVERTER TODO TASK: Replace 'unknown' with the appropriate dll name:
		[DllImport("unknown")]
		private extern void writeBytes(sbyte[] b, int off, int len, bool append);

		/// <summary>
		/// Writes <code>b.length</code> bytes from the specified byte array
		/// to this file output stream.
		/// </summary>
		/// <param name="b">   the data. </param>
		/// <exception cref="IOException">  if an I/O error occurs. </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public void write(byte b[]) throws IOException
		public override void Write(sbyte[] b)
		{
			writeBytes(b, 0, b.Length, Append);
		}

		/// <summary>
		/// Writes <code>len</code> bytes from the specified byte array
		/// starting at offset <code>off</code> to this file output stream.
		/// </summary>
		/// <param name="b">     the data. </param>
		/// <param name="off">   the start offset in the data. </param>
		/// <param name="len">   the number of bytes to write. </param>
		/// <exception cref="IOException">  if an I/O error occurs. </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public void write(byte b[] , int off, int len) throws IOException
		public override void Write(sbyte[] b, int off, int len)
		{
			writeBytes(b, off, len, Append);
		}

		/// <summary>
		/// Closes this file output stream and releases any system resources
		/// associated with this stream. This file output stream may no longer
		/// be used for writing bytes.
		/// 
		/// <para> If this stream has an associated channel then the channel is closed
		/// as well.
		/// 
		/// </para>
		/// </summary>
		/// <exception cref="IOException">  if an I/O error occurs.
		/// 
		/// @revised 1.4
		/// @spec JSR-51 </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public void close() throws IOException
		public override void Close()
		{
			lock (CloseLock)
			{
				if (Closed)
				{
					return;
				}
				Closed = true;
			}

			if (Channel_Renamed != null)
			{
				Channel_Renamed.Close();
			}

			Fd.CloseAll(new CloseableAnonymousInnerClassHelper(this));
		}

		private class CloseableAnonymousInnerClassHelper : Closeable
		{
			private readonly FileOutputStream OuterInstance;

			public CloseableAnonymousInnerClassHelper(FileOutputStream outerInstance)
			{
				this.OuterInstance = outerInstance;
			}

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public void close() throws IOException
			public virtual void Close()
			{
			   close0();
			}
		}

		/// <summary>
		/// Returns the file descriptor associated with this stream.
		/// </summary>
		/// <returns>  the <code>FileDescriptor</code> object that represents
		///          the connection to the file in the file system being used
		///          by this <code>FileOutputStream</code> object.
		/// </returns>
		/// <exception cref="IOException">  if an I/O error occurs. </exception>
		/// <seealso cref=        java.io.FileDescriptor </seealso>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public final FileDescriptor getFD() throws IOException
		 public FileDescriptor FD
		 {
			 get
			 {
				if (Fd != null)
				{
					return Fd;
				}
				throw new IOException();
			 }
		 }

		/// <summary>
		/// Returns the unique <seealso cref="java.nio.channels.FileChannel FileChannel"/>
		/// object associated with this file output stream.
		/// 
		/// <para> The initial {@link java.nio.channels.FileChannel#position()
		/// position} of the returned channel will be equal to the
		/// number of bytes written to the file so far unless this stream is in
		/// append mode, in which case it will be equal to the size of the file.
		/// Writing bytes to this stream will increment the channel's position
		/// accordingly.  Changing the channel's position, either explicitly or by
		/// writing, will change this stream's file position.
		/// 
		/// </para>
		/// </summary>
		/// <returns>  the file channel associated with this file output stream
		/// 
		/// @since 1.4
		/// @spec JSR-51 </returns>
		public virtual FileChannel Channel
		{
			get
			{
				lock (this)
				{
					if (Channel_Renamed == null)
					{
						Channel_Renamed = FileChannelImpl.open(Fd, Path, false, true, Append, this);
					}
					return Channel_Renamed;
				}
			}
		}

		/// <summary>
		/// Cleans up the connection to the file, and ensures that the
		/// <code>close</code> method of this file output stream is
		/// called when there are no more references to this stream.
		/// </summary>
		/// <exception cref="IOException">  if an I/O error occurs. </exception>
		/// <seealso cref=        java.io.FileInputStream#close() </seealso>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: protected void finalize() throws IOException
		~FileOutputStream()
		{
			if (Fd != null)
			{
				if (Fd == FileDescriptor.@out || Fd == FileDescriptor.Err)
				{
					Flush();
				}
				else
				{
					/* if fd is shared, the references in FileDescriptor
					 * will ensure that finalizer is only called when
					 * safe to do so. All references using the fd have
					 * become unreachable. We can call close()
					 */
					Close();
				}
			}
		}

//JAVA TO C# CONVERTER TODO TASK: Replace 'unknown' with the appropriate dll name:
		[DllImport("unknown")]
		private extern void close0();

//JAVA TO C# CONVERTER TODO TASK: Replace 'unknown' with the appropriate dll name:
		[DllImport("unknown")]
		private static extern void initIDs();

		static FileOutputStream()
		{
			initIDs();
		}

	}

}
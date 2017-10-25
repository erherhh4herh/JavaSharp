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
	/// A <code>FileInputStream</code> obtains input bytes
	/// from a file in a file system. What files
	/// are  available depends on the host environment.
	/// 
	/// <para><code>FileInputStream</code> is meant for reading streams of raw bytes
	/// such as image data. For reading streams of characters, consider using
	/// <code>FileReader</code>.
	/// 
	/// @author  Arthur van Hoff
	/// </para>
	/// </summary>
	/// <seealso cref=     java.io.File </seealso>
	/// <seealso cref=     java.io.FileDescriptor </seealso>
	/// <seealso cref=     java.io.FileOutputStream </seealso>
	/// <seealso cref=     java.nio.file.Files#newInputStream
	/// @since   JDK1.0 </seealso>
	public class FileInputStream : InputStream
	{
		/* File Descriptor - handle to the open file */
		private readonly FileDescriptor Fd;

		/// <summary>
		/// The path of the referenced file
		/// (null if the stream is created with a file descriptor)
		/// </summary>
		private readonly String Path;

		private FileChannel Channel_Renamed = null;

		private readonly Object CloseLock = new Object();
		private volatile bool Closed = false;

		/// <summary>
		/// Creates a <code>FileInputStream</code> by
		/// opening a connection to an actual file,
		/// the file named by the path name <code>name</code>
		/// in the file system.  A new <code>FileDescriptor</code>
		/// object is created to represent this file
		/// connection.
		/// <para>
		/// First, if there is a security
		/// manager, its <code>checkRead</code> method
		/// is called with the <code>name</code> argument
		/// as its argument.
		/// </para>
		/// <para>
		/// If the named file does not exist, is a directory rather than a regular
		/// file, or for some other reason cannot be opened for reading then a
		/// <code>FileNotFoundException</code> is thrown.
		/// 
		/// </para>
		/// </summary>
		/// <param name="name">   the system-dependent file name. </param>
		/// <exception cref="FileNotFoundException">  if the file does not exist,
		///                   is a directory rather than a regular file,
		///                   or for some other reason cannot be opened for
		///                   reading. </exception>
		/// <exception cref="SecurityException">      if a security manager exists and its
		///               <code>checkRead</code> method denies read access
		///               to the file. </exception>
		/// <seealso cref=        java.lang.SecurityManager#checkRead(java.lang.String) </seealso>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public FileInputStream(String name) throws FileNotFoundException
		public FileInputStream(String name) : this(name != null ? new File(name) : null)
		{
		}

		/// <summary>
		/// Creates a <code>FileInputStream</code> by
		/// opening a connection to an actual file,
		/// the file named by the <code>File</code>
		/// object <code>file</code> in the file system.
		/// A new <code>FileDescriptor</code> object
		/// is created to represent this file connection.
		/// <para>
		/// First, if there is a security manager,
		/// its <code>checkRead</code> method  is called
		/// with the path represented by the <code>file</code>
		/// argument as its argument.
		/// </para>
		/// <para>
		/// If the named file does not exist, is a directory rather than a regular
		/// file, or for some other reason cannot be opened for reading then a
		/// <code>FileNotFoundException</code> is thrown.
		/// 
		/// </para>
		/// </summary>
		/// <param name="file">   the file to be opened for reading. </param>
		/// <exception cref="FileNotFoundException">  if the file does not exist,
		///                   is a directory rather than a regular file,
		///                   or for some other reason cannot be opened for
		///                   reading. </exception>
		/// <exception cref="SecurityException">      if a security manager exists and its
		///               <code>checkRead</code> method denies read access to the file. </exception>
		/// <seealso cref=        java.io.File#getPath() </seealso>
		/// <seealso cref=        java.lang.SecurityManager#checkRead(java.lang.String) </seealso>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public FileInputStream(File file) throws FileNotFoundException
		public FileInputStream(File file)
		{
			String name = (file != null ? file.Path : null);
			SecurityManager security = System.SecurityManager;
			if (security != null)
			{
				security.CheckRead(name);
			}
			if (name == null)
			{
				throw new NullPointerException();
			}
			if (file.Invalid)
			{
				throw new FileNotFoundException("Invalid file path");
			}
			Fd = new FileDescriptor();
			Fd.Attach(this);
			Path = name;
			Open(name);
		}

		/// <summary>
		/// Creates a <code>FileInputStream</code> by using the file descriptor
		/// <code>fdObj</code>, which represents an existing connection to an
		/// actual file in the file system.
		/// <para>
		/// If there is a security manager, its <code>checkRead</code> method is
		/// called with the file descriptor <code>fdObj</code> as its argument to
		/// see if it's ok to read the file descriptor. If read access is denied
		/// to the file descriptor a <code>SecurityException</code> is thrown.
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
		/// <param name="fdObj">   the file descriptor to be opened for reading. </param>
		/// <exception cref="SecurityException">      if a security manager exists and its
		///                 <code>checkRead</code> method denies read access to the
		///                 file descriptor. </exception>
		/// <seealso cref=        SecurityManager#checkRead(java.io.FileDescriptor) </seealso>
		public FileInputStream(FileDescriptor fdObj)
		{
			SecurityManager security = System.SecurityManager;
			if (fdObj == null)
			{
				throw new NullPointerException();
			}
			if (security != null)
			{
				security.CheckRead(fdObj);
			}
			Fd = fdObj;
			Path = null;

			/*
			 * FileDescriptor is being shared by streams.
			 * Register this stream with FileDescriptor tracker.
			 */
			Fd.Attach(this);
		}

		/// <summary>
		/// Opens the specified file for reading. </summary>
		/// <param name="name"> the name of the file </param>
//JAVA TO C# CONVERTER TODO TASK: Replace 'unknown' with the appropriate dll name:
		[DllImport("unknown")]
		private extern void open0(String name);

		// wrap native call to allow instrumentation
		/// <summary>
		/// Opens the specified file for reading. </summary>
		/// <param name="name"> the name of the file </param>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: private void open(String name) throws FileNotFoundException
		private void Open(String name)
		{
			open0(name);
		}

		/// <summary>
		/// Reads a byte of data from this input stream. This method blocks
		/// if no input is yet available.
		/// </summary>
		/// <returns>     the next byte of data, or <code>-1</code> if the end of the
		///             file is reached. </returns>
		/// <exception cref="IOException">  if an I/O error occurs. </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public int read() throws IOException
		public override int Read()
		{
			return read0();
		}

//JAVA TO C# CONVERTER TODO TASK: Replace 'unknown' with the appropriate dll name:
		[DllImport("unknown")]
		private extern int read0();

		/// <summary>
		/// Reads a subarray as a sequence of bytes. </summary>
		/// <param name="b"> the data to be written </param>
		/// <param name="off"> the start offset in the data </param>
		/// <param name="len"> the number of bytes that are written </param>
		/// <exception cref="IOException"> If an I/O error has occurred. </exception>
//JAVA TO C# CONVERTER TODO TASK: Replace 'unknown' with the appropriate dll name:
		[DllImport("unknown")]
		private extern int readBytes(sbyte[] b, int off, int len);

		/// <summary>
		/// Reads up to <code>b.length</code> bytes of data from this input
		/// stream into an array of bytes. This method blocks until some input
		/// is available.
		/// </summary>
		/// <param name="b">   the buffer into which the data is read. </param>
		/// <returns>     the total number of bytes read into the buffer, or
		///             <code>-1</code> if there is no more data because the end of
		///             the file has been reached. </returns>
		/// <exception cref="IOException">  if an I/O error occurs. </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public int read(byte b[]) throws IOException
		public override int Read(sbyte[] b)
		{
			return readBytes(b, 0, b.Length);
		}

		/// <summary>
		/// Reads up to <code>len</code> bytes of data from this input stream
		/// into an array of bytes. If <code>len</code> is not zero, the method
		/// blocks until some input is available; otherwise, no
		/// bytes are read and <code>0</code> is returned.
		/// </summary>
		/// <param name="b">     the buffer into which the data is read. </param>
		/// <param name="off">   the start offset in the destination array <code>b</code> </param>
		/// <param name="len">   the maximum number of bytes read. </param>
		/// <returns>     the total number of bytes read into the buffer, or
		///             <code>-1</code> if there is no more data because the end of
		///             the file has been reached. </returns>
		/// <exception cref="NullPointerException"> If <code>b</code> is <code>null</code>. </exception>
		/// <exception cref="IndexOutOfBoundsException"> If <code>off</code> is negative,
		/// <code>len</code> is negative, or <code>len</code> is greater than
		/// <code>b.length - off</code> </exception>
		/// <exception cref="IOException">  if an I/O error occurs. </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public int read(byte b[] , int off, int len) throws IOException
		public override int Read(sbyte[] b, int off, int len)
		{
			return readBytes(b, off, len);
		}

		/// <summary>
		/// Skips over and discards <code>n</code> bytes of data from the
		/// input stream.
		/// 
		/// <para>The <code>skip</code> method may, for a variety of
		/// reasons, end up skipping over some smaller number of bytes,
		/// possibly <code>0</code>. If <code>n</code> is negative, the method
		/// will try to skip backwards. In case the backing file does not support
		/// backward skip at its current position, an <code>IOException</code> is
		/// thrown. The actual number of bytes skipped is returned. If it skips
		/// forwards, it returns a positive value. If it skips backwards, it
		/// returns a negative value.
		/// 
		/// </para>
		/// <para>This method may skip more bytes than what are remaining in the
		/// backing file. This produces no exception and the number of bytes skipped
		/// may include some number of bytes that were beyond the EOF of the
		/// backing file. Attempting to read from the stream after skipping past
		/// the end will result in -1 indicating the end of the file.
		/// 
		/// </para>
		/// </summary>
		/// <param name="n">   the number of bytes to be skipped. </param>
		/// <returns>     the actual number of bytes skipped. </returns>
		/// <exception cref="IOException">  if n is negative, if the stream does not
		///             support seek, or if an I/O error occurs. </exception>
//JAVA TO C# CONVERTER TODO TASK: Replace 'unknown' with the appropriate dll name:
		[DllImport("unknown")]
		public extern long skip(long n);

		/// <summary>
		/// Returns an estimate of the number of remaining bytes that can be read (or
		/// skipped over) from this input stream without blocking by the next
		/// invocation of a method for this input stream. Returns 0 when the file
		/// position is beyond EOF. The next invocation might be the same thread
		/// or another thread. A single read or skip of this many bytes will not
		/// block, but may read or skip fewer bytes.
		/// 
		/// <para> In some cases, a non-blocking read (or skip) may appear to be
		/// blocked when it is merely slow, for example when reading large
		/// files over slow networks.
		/// 
		/// </para>
		/// </summary>
		/// <returns>     an estimate of the number of remaining bytes that can be read
		///             (or skipped over) from this input stream without blocking. </returns>
		/// <exception cref="IOException">  if this file input stream has been closed by calling
		///             {@code close} or an I/O error occurs. </exception>
//JAVA TO C# CONVERTER TODO TASK: Replace 'unknown' with the appropriate dll name:
		[DllImport("unknown")]
		public extern int available();

		/// <summary>
		/// Closes this file input stream and releases any system resources
		/// associated with the stream.
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
			private readonly FileInputStream OuterInstance;

			public CloseableAnonymousInnerClassHelper(FileInputStream outerInstance)
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
		/// Returns the <code>FileDescriptor</code>
		/// object  that represents the connection to
		/// the actual file in the file system being
		/// used by this <code>FileInputStream</code>.
		/// </summary>
		/// <returns>     the file descriptor object associated with this stream. </returns>
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
		/// object associated with this file input stream.
		/// 
		/// <para> The initial {@link java.nio.channels.FileChannel#position()
		/// position} of the returned channel will be equal to the
		/// number of bytes read from the file so far.  Reading bytes from this
		/// stream will increment the channel's position.  Changing the channel's
		/// position, either explicitly or by reading, will change this stream's
		/// file position.
		/// 
		/// </para>
		/// </summary>
		/// <returns>  the file channel associated with this file input stream
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
						Channel_Renamed = FileChannelImpl.open(Fd, Path, true, false, this);
					}
					return Channel_Renamed;
				}
			}
		}

//JAVA TO C# CONVERTER TODO TASK: Replace 'unknown' with the appropriate dll name:
		[DllImport("unknown")]
		private static extern void initIDs();

//JAVA TO C# CONVERTER TODO TASK: Replace 'unknown' with the appropriate dll name:
		[DllImport("unknown")]
		private extern void close0();

		static FileInputStream()
		{
			initIDs();
		}

		/// <summary>
		/// Ensures that the <code>close</code> method of this file input stream is
		/// called when there are no more references to it.
		/// </summary>
		/// <exception cref="IOException">  if an I/O error occurs. </exception>
		/// <seealso cref=        java.io.FileInputStream#close() </seealso>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: protected void finalize() throws IOException
		~FileInputStream()
		{
			if ((Fd != null) && (Fd != FileDescriptor.@in))
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

}
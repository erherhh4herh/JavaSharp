using System;
using System.Runtime.InteropServices;

/*
 * Copyright (c) 1995, 2013, Oracle and/or its affiliates. All rights reserved.
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

namespace java.net
{


	using ConnectionResetException = sun.net.ConnectionResetException;

	/// <summary>
	/// This stream extends FileInputStream to implement a
	/// SocketInputStream. Note that this class should <b>NOT</b> be
	/// public.
	/// 
	/// @author      Jonathan Payne
	/// @author      Arthur van Hoff
	/// </summary>
	internal class SocketInputStream : FileInputStream
	{
		static SocketInputStream()
		{
			init();
		}

		private bool Eof;
		private AbstractPlainSocketImpl Impl = null;
		private sbyte[] Temp;
		private Socket Socket = null;

		/// <summary>
		/// Creates a new SocketInputStream. Can only be called
		/// by a Socket. This method needs to hang on to the owner Socket so
		/// that the fd will not be closed. </summary>
		/// <param name="impl"> the implemented socket input stream </param>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: SocketInputStream(AbstractPlainSocketImpl impl) throws java.io.IOException
		internal SocketInputStream(AbstractPlainSocketImpl impl) : base(impl.FileDescriptor)
		{
			this.Impl = impl;
			Socket = impl.Socket;
		}

		/// <summary>
		/// Returns the unique <seealso cref="java.nio.channels.FileChannel FileChannel"/>
		/// object associated with this file input stream.</p>
		/// 
		/// The {@code getChannel} method of {@code SocketInputStream}
		/// returns {@code null} since it is a socket based stream.</p>
		/// </summary>
		/// <returns>  the file channel associated with this file input stream
		/// 
		/// @since 1.4
		/// @spec JSR-51 </returns>
		public sealed override FileChannel Channel
		{
			get
			{
				return null;
			}
		}

		/// <summary>
		/// Reads into an array of bytes at the specified offset using
		/// the received socket primitive. </summary>
		/// <param name="fd"> the FileDescriptor </param>
		/// <param name="b"> the buffer into which the data is read </param>
		/// <param name="off"> the start offset of the data </param>
		/// <param name="len"> the maximum number of bytes read </param>
		/// <param name="timeout"> the read timeout in ms </param>
		/// <returns> the actual number of bytes read, -1 is
		///          returned when the end of the stream is reached. </returns>
		/// <exception cref="IOException"> If an I/O error has occurred. </exception>
//JAVA TO C# CONVERTER TODO TASK: Replace 'unknown' with the appropriate dll name:
		[DllImport("unknown")]
		private extern int socketRead0(java.io.FileDescriptor fd, sbyte[] b, int off, int len, int timeout);

		// wrap native call to allow instrumentation
		/// <summary>
		/// Reads into an array of bytes at the specified offset using
		/// the received socket primitive. </summary>
		/// <param name="fd"> the FileDescriptor </param>
		/// <param name="b"> the buffer into which the data is read </param>
		/// <param name="off"> the start offset of the data </param>
		/// <param name="len"> the maximum number of bytes read </param>
		/// <param name="timeout"> the read timeout in ms </param>
		/// <returns> the actual number of bytes read, -1 is
		///          returned when the end of the stream is reached. </returns>
		/// <exception cref="IOException"> If an I/O error has occurred. </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: private int socketRead(java.io.FileDescriptor fd, byte b[] , int off, int len, int timeout) throws java.io.IOException
		private int SocketRead(FileDescriptor fd, sbyte[] b, int off, int len, int timeout)
		{
			return socketRead0(fd, b, off, len, timeout);
		}

		/// <summary>
		/// Reads into a byte array data from the socket. </summary>
		/// <param name="b"> the buffer into which the data is read </param>
		/// <returns> the actual number of bytes read, -1 is
		///          returned when the end of the stream is reached. </returns>
		/// <exception cref="IOException"> If an I/O error has occurred. </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public int read(byte b[]) throws java.io.IOException
		public override int Read(sbyte[] b)
		{
			return Read(b, 0, b.Length);
		}

		/// <summary>
		/// Reads into a byte array <i>b</i> at offset <i>off</i>,
		/// <i>length</i> bytes of data. </summary>
		/// <param name="b"> the buffer into which the data is read </param>
		/// <param name="off"> the start offset of the data </param>
		/// <param name="length"> the maximum number of bytes read </param>
		/// <returns> the actual number of bytes read, -1 is
		///          returned when the end of the stream is reached. </returns>
		/// <exception cref="IOException"> If an I/O error has occurred. </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public int read(byte b[] , int off, int length) throws java.io.IOException
		public override int Read(sbyte[] b, int off, int length)
		{
			return Read(b, off, length, Impl.Timeout);
		}

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: int read(byte b[] , int off, int length, int timeout) throws java.io.IOException
		internal virtual int Read(sbyte[] b, int off, int length, int timeout)
		{
			int n;

			// EOF already encountered
			if (Eof)
			{
				return -1;
			}

			// connection reset
			if (Impl.ConnectionReset)
			{
				throw new SocketException("Connection reset");
			}

			// bounds check
			if (length <= 0 || off < 0 || off + length > b.Length)
			{
				if (length == 0)
				{
					return 0;
				}
				throw new ArrayIndexOutOfBoundsException();
			}

			bool gotReset = false;

			// acquire file descriptor and do the read
			FileDescriptor fd = Impl.AcquireFD();
			try
			{
				n = SocketRead(fd, b, off, length, timeout);
				if (n > 0)
				{
					return n;
				}
			}
			catch (ConnectionResetException)
			{
				gotReset = true;
			}
			finally
			{
				Impl.ReleaseFD();
			}

			/*
			 * We receive a "connection reset" but there may be bytes still
			 * buffered on the socket
			 */
			if (gotReset)
			{
				Impl.SetConnectionResetPending();
				Impl.AcquireFD();
				try
				{
					n = SocketRead(fd, b, off, length, timeout);
					if (n > 0)
					{
						return n;
					}
				}
				catch (ConnectionResetException)
				{
				}
				finally
				{
					Impl.ReleaseFD();
				}
			}

			/*
			 * If we get here we are at EOF, the socket has been closed,
			 * or the connection has been reset.
			 */
			if (Impl.ClosedOrPending)
			{
				throw new SocketException("Socket closed");
			}
			if (Impl.ConnectionResetPending)
			{
				Impl.SetConnectionReset();
			}
			if (Impl.ConnectionReset)
			{
				throw new SocketException("Connection reset");
			}
			Eof = true;
			return -1;
		}

		/// <summary>
		/// Reads a single byte from the socket.
		/// </summary>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public int read() throws java.io.IOException
		public override int Read()
		{
			if (Eof)
			{
				return -1;
			}
			Temp = new sbyte[1];
			int n = Read(Temp, 0, 1);
			if (n <= 0)
			{
				return -1;
			}
			return Temp[0] & 0xff;
		}

		/// <summary>
		/// Skips n bytes of input. </summary>
		/// <param name="numbytes"> the number of bytes to skip </param>
		/// <returns>  the actual number of bytes skipped. </returns>
		/// <exception cref="IOException"> If an I/O error has occurred. </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public long skip(long numbytes) throws java.io.IOException
		public override long Skip(long numbytes)
		{
			if (numbytes <= 0)
			{
				return 0;
			}
			long n = numbytes;
			int buflen = (int) System.Math.Min(1024, n);
			sbyte[] data = new sbyte[buflen];
			while (n > 0)
			{
				int r = Read(data, 0, (int) System.Math.Min((long) buflen, n));
				if (r < 0)
				{
					break;
				}
				n -= r;
			}
			return numbytes - n;
		}

		/// <summary>
		/// Returns the number of bytes that can be read without blocking. </summary>
		/// <returns> the number of immediately available bytes </returns>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public int available() throws java.io.IOException
		public override int Available()
		{
			return Impl.Available();
		}

		/// <summary>
		/// Closes the stream.
		/// </summary>
		private bool Closing = false;
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public void close() throws java.io.IOException
		public override void Close()
		{
			// Prevent recursion. See BugId 4484411
			if (Closing)
			{
				return;
			}
			Closing = true;
			if (Socket != null)
			{
				if (!Socket.Closed)
				{
					Socket.Close();
				}
			}
			else
			{
				Impl.Close();
			}
			Closing = false;
		}

		internal virtual bool EOF
		{
			set
			{
				this.Eof = value;
			}
		}

		/// <summary>
		/// Overrides finalize, the fd is closed by the Socket.
		/// </summary>
		~SocketInputStream()
		{
		}

		/// <summary>
		/// Perform class load-time initializations.
		/// </summary>
//JAVA TO C# CONVERTER TODO TASK: Replace 'unknown' with the appropriate dll name:
		[DllImport("unknown")]
		private extern static void init();
	}

}
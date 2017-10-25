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


	/// <summary>
	/// This stream extends FileOutputStream to implement a
	/// SocketOutputStream. Note that this class should <b>NOT</b> be
	/// public.
	/// 
	/// @author      Jonathan Payne
	/// @author      Arthur van Hoff
	/// </summary>
	internal class SocketOutputStream : FileOutputStream
	{
		static SocketOutputStream()
		{
			init();
		}

		private AbstractPlainSocketImpl Impl = null;
		private sbyte[] Temp = new sbyte[1];
		private Socket Socket = null;

		/// <summary>
		/// Creates a new SocketOutputStream. Can only be called
		/// by a Socket. This method needs to hang on to the owner Socket so
		/// that the fd will not be closed. </summary>
		/// <param name="impl"> the socket output stream inplemented </param>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: SocketOutputStream(AbstractPlainSocketImpl impl) throws java.io.IOException
		internal SocketOutputStream(AbstractPlainSocketImpl impl) : base(impl.FileDescriptor)
		{
			this.Impl = impl;
			Socket = impl.Socket;
		}

		/// <summary>
		/// Returns the unique <seealso cref="java.nio.channels.FileChannel FileChannel"/>
		/// object associated with this file output stream. </p>
		/// 
		/// The {@code getChannel} method of {@code SocketOutputStream}
		/// returns {@code null} since it is a socket based stream.</p>
		/// </summary>
		/// <returns>  the file channel associated with this file output stream
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
		/// Writes to the socket. </summary>
		/// <param name="fd"> the FileDescriptor </param>
		/// <param name="b"> the data to be written </param>
		/// <param name="off"> the start offset in the data </param>
		/// <param name="len"> the number of bytes that are written </param>
		/// <exception cref="IOException"> If an I/O error has occurred. </exception>
//JAVA TO C# CONVERTER TODO TASK: Replace 'unknown' with the appropriate dll name:
		[DllImport("unknown")]
		private extern void socketWrite0(java.io.FileDescriptor fd, sbyte[] b, int off, int len);

		/// <summary>
		/// Writes to the socket with appropriate locking of the
		/// FileDescriptor. </summary>
		/// <param name="b"> the data to be written </param>
		/// <param name="off"> the start offset in the data </param>
		/// <param name="len"> the number of bytes that are written </param>
		/// <exception cref="IOException"> If an I/O error has occurred. </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: private void socketWrite(byte b[] , int off, int len) throws java.io.IOException
		private void SocketWrite(sbyte[] b, int off, int len)
		{

			if (len <= 0 || off < 0 || off + len > b.Length)
			{
				if (len == 0)
				{
					return;
				}
				throw new ArrayIndexOutOfBoundsException();
			}

			FileDescriptor fd = Impl.AcquireFD();
			try
			{
				socketWrite0(fd, b, off, len);
			}
			catch (SocketException se)
			{
				if (se is sun.net.ConnectionResetException)
				{
					Impl.SetConnectionResetPending();
					se = new SocketException("Connection reset");
				}
				if (Impl.ClosedOrPending)
				{
					throw new SocketException("Socket closed");
				}
				else
				{
					throw se;
				}
			}
			finally
			{
				Impl.ReleaseFD();
			}
		}

		/// <summary>
		/// Writes a byte to the socket. </summary>
		/// <param name="b"> the data to be written </param>
		/// <exception cref="IOException"> If an I/O error has occurred. </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public void write(int b) throws java.io.IOException
		public override void Write(int b)
		{
			Temp[0] = (sbyte)b;
			SocketWrite(Temp, 0, 1);
		}

		/// <summary>
		/// Writes the contents of the buffer <i>b</i> to the socket. </summary>
		/// <param name="b"> the data to be written </param>
		/// <exception cref="SocketException"> If an I/O error has occurred. </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public void write(byte b[]) throws java.io.IOException
		public override void Write(sbyte[] b)
		{
			SocketWrite(b, 0, b.Length);
		}

		/// <summary>
		/// Writes <i>length</i> bytes from buffer <i>b</i> starting at
		/// offset <i>len</i>. </summary>
		/// <param name="b"> the data to be written </param>
		/// <param name="off"> the start offset in the data </param>
		/// <param name="len"> the number of bytes that are written </param>
		/// <exception cref="SocketException"> If an I/O error has occurred. </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public void write(byte b[] , int off, int len) throws java.io.IOException
		public override void Write(sbyte[] b, int off, int len)
		{
			SocketWrite(b, off, len);
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

		/// <summary>
		/// Overrides finalize, the fd is closed by the Socket.
		/// </summary>
		~SocketOutputStream()
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
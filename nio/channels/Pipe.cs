/*
 * Copyright (c) 2000, 2013, Oracle and/or its affiliates. All rights reserved.
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

namespace java.nio.channels
{



	/// <summary>
	/// A pair of channels that implements a unidirectional pipe.
	/// 
	/// <para> A pipe consists of a pair of channels: A writable {@link
	/// Pipe.SinkChannel sink} channel and a readable <seealso cref="Pipe.SourceChannel source"/>
	/// channel.  Once some bytes are written to the sink channel they can be read
	/// from source channel in exactlyAthe order in which they were written.
	/// 
	/// </para>
	/// <para> Whether or not a thread writing bytes to a pipe will block until another
	/// thread reads those bytes, or some previously-written bytes, from the pipe is
	/// system-dependent and therefore unspecified.  Many pipe implementations will
	/// buffer up to a certain number of bytes between the sink and source channels,
	/// but such buffering should not be assumed.  </para>
	/// 
	/// 
	/// @author Mark Reinhold
	/// @author JSR-51 Expert Group
	/// @since 1.4
	/// </summary>

	public abstract class Pipe
	{

		/// <summary>
		/// A channel representing the readable end of a <seealso cref="Pipe"/>.
		/// 
		/// @since 1.4
		/// </summary>
		public abstract class SourceChannel : AbstractSelectableChannel, ReadableByteChannel, ScatteringByteChannel
		{
			public abstract long Read(java.nio.ByteBuffer[] dsts, int offset, int length);
			public abstract int Read(java.nio.ByteBuffer dst);
			/// <summary>
			/// Constructs a new instance of this class.
			/// </summary>
			/// <param name="provider">
			///         The selector provider </param>
			protected internal SourceChannel(SelectorProvider provider) : base(provider)
			{
			}

			/// <summary>
			/// Returns an operation set identifying this channel's supported
			/// operations.
			/// 
			/// <para> Pipe-source channels only support reading, so this method
			/// returns <seealso cref="SelectionKey#OP_READ"/>.  </para>
			/// </summary>
			/// <returns>  The valid-operation set </returns>
			public sealed override int ValidOps()
			{
				return SelectionKey.OP_READ;
			}

		}

		/// <summary>
		/// A channel representing the writable end of a <seealso cref="Pipe"/>.
		/// 
		/// @since 1.4
		/// </summary>
		public abstract class SinkChannel : AbstractSelectableChannel, WritableByteChannel, GatheringByteChannel
		{
			public abstract long Write(java.nio.ByteBuffer[] srcs, int offset, int length);
			public abstract int Write(java.nio.ByteBuffer src);
			/// <summary>
			/// Initializes a new instance of this class.
			/// </summary>
			/// <param name="provider">
			///         The selector provider </param>
			protected internal SinkChannel(SelectorProvider provider) : base(provider)
			{
			}

			/// <summary>
			/// Returns an operation set identifying this channel's supported
			/// operations.
			/// 
			/// <para> Pipe-sink channels only support writing, so this method returns
			/// <seealso cref="SelectionKey#OP_WRITE"/>.  </para>
			/// </summary>
			/// <returns>  The valid-operation set </returns>
			public sealed override int ValidOps()
			{
				return SelectionKey.OP_WRITE;
			}

		}

		/// <summary>
		/// Initializes a new instance of this class.
		/// </summary>
		protected internal Pipe()
		{
		}

		/// <summary>
		/// Returns this pipe's source channel.
		/// </summary>
		/// <returns>  This pipe's source channel </returns>
		public abstract SourceChannel Source();

		/// <summary>
		/// Returns this pipe's sink channel.
		/// </summary>
		/// <returns>  This pipe's sink channel </returns>
		public abstract SinkChannel Sink();

		/// <summary>
		/// Opens a pipe.
		/// 
		/// <para> The new pipe is created by invoking the {@link
		/// java.nio.channels.spi.SelectorProvider#openPipe openPipe} method of the
		/// system-wide default <seealso cref="java.nio.channels.spi.SelectorProvider"/>
		/// object.  </para>
		/// </summary>
		/// <returns>  A new pipe
		/// </returns>
		/// <exception cref="IOException">
		///          If an I/O error occurs </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public static Pipe open() throws java.io.IOException
		public static Pipe Open()
		{
			return SelectorProvider.Provider().OpenPipe();
		}

	}

}
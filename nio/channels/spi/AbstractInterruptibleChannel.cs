using System.Threading;

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

/*
 */

namespace java.nio.channels.spi
{

	using Interruptible = sun.nio.ch.Interruptible;


	/// <summary>
	/// Base implementation class for interruptible channels.
	/// 
	/// <para> This class encapsulates the low-level machinery required to implement
	/// the asynchronous closing and interruption of channels.  A concrete channel
	/// class must invoke the <seealso cref="#begin begin"/> and <seealso cref="#end end"/> methods
	/// before and after, respectively, invoking an I/O operation that might block
	/// indefinitely.  In order to ensure that the <seealso cref="#end end"/> method is always
	/// invoked, these methods should be used within a
	/// <tt>try</tt>&nbsp;...&nbsp;<tt>finally</tt> block:
	/// 
	/// <blockquote><pre>
	/// boolean completed = false;
	/// try {
	///     begin();
	///     completed = ...;    // Perform blocking I/O operation
	///     return ...;         // Return result
	/// } finally {
	///     end(completed);
	/// }</pre></blockquote>
	/// 
	/// </para>
	/// <para> The <tt>completed</tt> argument to the <seealso cref="#end end"/> method tells
	/// whether or not the I/O operation actually completed, that is, whether it had
	/// any effect that would be visible to the invoker.  In the case of an
	/// operation that reads bytes, for example, this argument should be
	/// <tt>true</tt> if, and only if, some bytes were actually transferred into the
	/// invoker's target buffer.
	/// 
	/// </para>
	/// <para> A concrete channel class must also implement the {@link
	/// #implCloseChannel implCloseChannel} method in such a way that if it is
	/// invoked while another thread is blocked in a native I/O operation upon the
	/// channel then that operation will immediately return, either by throwing an
	/// exception or by returning normally.  If a thread is interrupted or the
	/// channel upon which it is blocked is asynchronously closed then the channel's
	/// <seealso cref="#end end"/> method will throw the appropriate exception.
	/// 
	/// </para>
	/// <para> This class performs the synchronization required to implement the {@link
	/// java.nio.channels.Channel} specification.  Implementations of the {@link
	/// #implCloseChannel implCloseChannel} method need not synchronize against
	/// other threads that might be attempting to close the channel.  </para>
	/// 
	/// 
	/// @author Mark Reinhold
	/// @author JSR-51 Expert Group
	/// @since 1.4
	/// </summary>

	public abstract class AbstractInterruptibleChannel : Channel, InterruptibleChannel
	{

		private readonly Object CloseLock = new Object();
		private volatile bool Open_Renamed = true;

		/// <summary>
		/// Initializes a new instance of this class.
		/// </summary>
		protected internal AbstractInterruptibleChannel()
		{
		}

		/// <summary>
		/// Closes this channel.
		/// 
		/// <para> If the channel has already been closed then this method returns
		/// immediately.  Otherwise it marks the channel as closed and then invokes
		/// the <seealso cref="#implCloseChannel implCloseChannel"/> method in order to
		/// complete the close operation.  </para>
		/// </summary>
		/// <exception cref="IOException">
		///          If an I/O error occurs </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public final void close() throws java.io.IOException
		public void Close()
		{
			lock (CloseLock)
			{
				if (!Open_Renamed)
				{
					return;
				}
				Open_Renamed = false;
				ImplCloseChannel();
			}
		}

		/// <summary>
		/// Closes this channel.
		/// 
		/// <para> This method is invoked by the <seealso cref="#close close"/> method in order
		/// to perform the actual work of closing the channel.  This method is only
		/// invoked if the channel has not yet been closed, and it is never invoked
		/// more than once.
		/// 
		/// </para>
		/// <para> An implementation of this method must arrange for any other thread
		/// that is blocked in an I/O operation upon this channel to return
		/// immediately, either by throwing an exception or by returning normally.
		/// </para>
		/// </summary>
		/// <exception cref="IOException">
		///          If an I/O error occurs while closing the channel </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: protected abstract void implCloseChannel() throws java.io.IOException;
		protected internal abstract void ImplCloseChannel();

		public bool Open
		{
			get
			{
				return Open_Renamed;
			}
		}


		// -- Interruption machinery --

		private Interruptible Interruptor;
		private volatile Thread Interrupted;

		/// <summary>
		/// Marks the beginning of an I/O operation that might block indefinitely.
		/// 
		/// <para> This method should be invoked in tandem with the <seealso cref="#end end"/>
		/// method, using a <tt>try</tt>&nbsp;...&nbsp;<tt>finally</tt> block as
		/// shown <a href="#be">above</a>, in order to implement asynchronous
		/// closing and interruption for this channel.  </para>
		/// </summary>
		protected internal void Begin()
		{
			if (Interruptor == null)
			{
				Interruptor = new InterruptibleAnonymousInnerClassHelper(this);
			}
			BlockedOn(Interruptor);
			Thread me = Thread.CurrentThread;
			if (me.Interrupted)
			{
				Interruptor.interrupt(me);
			}
		}

		private class InterruptibleAnonymousInnerClassHelper : Interruptible
		{
			private readonly AbstractInterruptibleChannel OuterInstance;

			public InterruptibleAnonymousInnerClassHelper(AbstractInterruptibleChannel outerInstance)
			{
				this.OuterInstance = outerInstance;
			}

			public virtual void Interrupt(Thread target)
			{
				lock (OuterInstance.CloseLock)
				{
					if (!OuterInstance.Open_Renamed)
					{
						return;
					}
					OuterInstance.Open_Renamed = false;
					OuterInstance.Interrupted = target;
					try
					{
						OuterInstance.ImplCloseChannel();
					}
					catch (IOException)
					{
					}
				}
			}
		}

		/// <summary>
		/// Marks the end of an I/O operation that might block indefinitely.
		/// 
		/// <para> This method should be invoked in tandem with the {@link #begin
		/// begin} method, using a <tt>try</tt>&nbsp;...&nbsp;<tt>finally</tt> block
		/// as shown <a href="#be">above</a>, in order to implement asynchronous
		/// closing and interruption for this channel.  </para>
		/// </summary>
		/// <param name="completed">
		///         <tt>true</tt> if, and only if, the I/O operation completed
		///         successfully, that is, had some effect that would be visible to
		///         the operation's invoker
		/// </param>
		/// <exception cref="AsynchronousCloseException">
		///          If the channel was asynchronously closed
		/// </exception>
		/// <exception cref="ClosedByInterruptException">
		///          If the thread blocked in the I/O operation was interrupted </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: protected final void end(boolean completed) throws AsynchronousCloseException
		protected internal void End(bool completed)
		{
			BlockedOn(null);
			Thread interrupted = this.Interrupted;
			if (interrupted != null && interrupted == Thread.CurrentThread)
			{
				interrupted = null;
				throw new ClosedByInterruptException();
			}
			if (!completed && !Open_Renamed)
			{
				throw new AsynchronousCloseException();
			}
		}


		// -- sun.misc.SharedSecrets --
		internal static void BlockedOn(Interruptible intr) // package-private
		{
			sun.misc.SharedSecrets.JavaLangAccess.blockedOn(Thread.CurrentThread, intr);
		}
	}

}
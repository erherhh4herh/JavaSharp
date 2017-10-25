using System.Collections.Generic;
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

namespace java.nio.channels.spi
{

	using Interruptible = sun.nio.ch.Interruptible;


	/// <summary>
	/// Base implementation class for selectors.
	/// 
	/// <para> This class encapsulates the low-level machinery required to implement
	/// the interruption of selection operations.  A concrete selector class must
	/// invoke the <seealso cref="#begin begin"/> and <seealso cref="#end end"/> methods before and
	/// after, respectively, invoking an I/O operation that might block
	/// indefinitely.  In order to ensure that the <seealso cref="#end end"/> method is always
	/// invoked, these methods should be used within a
	/// <tt>try</tt>&nbsp;...&nbsp;<tt>finally</tt> block:
	/// 
	/// <blockquote><pre>
	/// try {
	///     begin();
	///     // Perform blocking I/O operation here
	///     ...
	/// } finally {
	///     end();
	/// }</pre></blockquote>
	/// 
	/// </para>
	/// <para> This class also defines methods for maintaining a selector's
	/// cancelled-key set and for removing a key from its channel's key set, and
	/// declares the abstract <seealso cref="#register register"/> method that is invoked by a
	/// selectable channel's <seealso cref="AbstractSelectableChannel#register register"/>
	/// method in order to perform the actual work of registering a channel.  </para>
	/// 
	/// 
	/// @author Mark Reinhold
	/// @author JSR-51 Expert Group
	/// @since 1.4
	/// </summary>

	public abstract class AbstractSelector : Selector
	{

		private AtomicBoolean SelectorOpen = new AtomicBoolean(true);

		// The provider that created this selector
		private readonly SelectorProvider Provider_Renamed;

		/// <summary>
		/// Initializes a new instance of this class.
		/// </summary>
		/// <param name="provider">
		///         The provider that created this selector </param>
		protected internal AbstractSelector(SelectorProvider provider)
		{
			this.Provider_Renamed = provider;
		}

		private readonly Set<SelectionKey> CancelledKeys_Renamed = new HashSet<SelectionKey>();

		internal virtual void Cancel(SelectionKey k) // package-private
		{
			lock (CancelledKeys_Renamed)
			{
				CancelledKeys_Renamed.Add(k);
			}
		}

		/// <summary>
		/// Closes this selector.
		/// 
		/// <para> If the selector has already been closed then this method returns
		/// immediately.  Otherwise it marks the selector as closed and then invokes
		/// the <seealso cref="#implCloseSelector implCloseSelector"/> method in order to
		/// complete the close operation.  </para>
		/// </summary>
		/// <exception cref="IOException">
		///          If an I/O error occurs </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public final void close() throws java.io.IOException
		public sealed override void Close()
		{
			bool open = SelectorOpen.GetAndSet(false);
			if (!open)
			{
				return;
			}
			ImplCloseSelector();
		}

		/// <summary>
		/// Closes this selector.
		/// 
		/// <para> This method is invoked by the <seealso cref="#close close"/> method in order
		/// to perform the actual work of closing the selector.  This method is only
		/// invoked if the selector has not yet been closed, and it is never invoked
		/// more than once.
		/// 
		/// </para>
		/// <para> An implementation of this method must arrange for any other thread
		/// that is blocked in a selection operation upon this selector to return
		/// immediately as if by invoking the {@link
		/// java.nio.channels.Selector#wakeup wakeup} method. </para>
		/// </summary>
		/// <exception cref="IOException">
		///          If an I/O error occurs while closing the selector </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: protected abstract void implCloseSelector() throws java.io.IOException;
		protected internal abstract void ImplCloseSelector();

		public sealed override bool Open
		{
			get
			{
				return SelectorOpen.Get();
			}
		}

		/// <summary>
		/// Returns the provider that created this channel.
		/// </summary>
		/// <returns>  The provider that created this channel </returns>
		public sealed override SelectorProvider Provider()
		{
			return Provider_Renamed;
		}

		/// <summary>
		/// Retrieves this selector's cancelled-key set.
		/// 
		/// <para> This set should only be used while synchronized upon it.  </para>
		/// </summary>
		/// <returns>  The cancelled-key set </returns>
		protected internal Set<SelectionKey> CancelledKeys()
		{
			return CancelledKeys_Renamed;
		}

		/// <summary>
		/// Registers the given channel with this selector.
		/// 
		/// <para> This method is invoked by a channel's {@link
		/// AbstractSelectableChannel#register register} method in order to perform
		/// the actual work of registering the channel with this selector.  </para>
		/// </summary>
		/// <param name="ch">
		///         The channel to be registered
		/// </param>
		/// <param name="ops">
		///         The initial interest set, which must be valid
		/// </param>
		/// <param name="att">
		///         The initial attachment for the resulting key
		/// </param>
		/// <returns>  A new key representing the registration of the given channel
		///          with this selector </returns>
		protected internal abstract SelectionKey Register(AbstractSelectableChannel ch, int ops, Object att);

		/// <summary>
		/// Removes the given key from its channel's key set.
		/// 
		/// <para> This method must be invoked by the selector for each channel that it
		/// deregisters.  </para>
		/// </summary>
		/// <param name="key">
		///         The selection key to be removed </param>
		protected internal void Deregister(AbstractSelectionKey key)
		{
			((AbstractSelectableChannel)key.Channel()).RemoveKey(key);
		}


		// -- Interruption machinery --

		private Interruptible Interruptor = null;

		/// <summary>
		/// Marks the beginning of an I/O operation that might block indefinitely.
		/// 
		/// <para> This method should be invoked in tandem with the <seealso cref="#end end"/>
		/// method, using a <tt>try</tt>&nbsp;...&nbsp;<tt>finally</tt> block as
		/// shown <a href="#be">above</a>, in order to implement interruption for
		/// this selector.
		/// 
		/// </para>
		/// <para> Invoking this method arranges for the selector's {@link
		/// Selector#wakeup wakeup} method to be invoked if a thread's {@link
		/// Thread#interrupt interrupt} method is invoked while the thread is
		/// blocked in an I/O operation upon the selector.  </para>
		/// </summary>
		protected internal void Begin()
		{
			if (Interruptor == null)
			{
				Interruptor = new InterruptibleAnonymousInnerClassHelper(this);
			}
			AbstractInterruptibleChannel.BlockedOn(Interruptor);
			Thread me = Thread.CurrentThread;
			if (me.Interrupted)
			{
				Interruptor.interrupt(me);
			}
		}

		private class InterruptibleAnonymousInnerClassHelper : Interruptible
		{
			private readonly AbstractSelector OuterInstance;

			public InterruptibleAnonymousInnerClassHelper(AbstractSelector outerInstance)
			{
				this.OuterInstance = outerInstance;
			}

			public virtual void Interrupt(Thread ignore)
			{
				OuterInstance.Wakeup();
			}
		}

		/// <summary>
		/// Marks the end of an I/O operation that might block indefinitely.
		/// 
		/// <para> This method should be invoked in tandem with the <seealso cref="#begin begin"/>
		/// method, using a <tt>try</tt>&nbsp;...&nbsp;<tt>finally</tt> block as
		/// shown <a href="#be">above</a>, in order to implement interruption for
		/// this selector.  </para>
		/// </summary>
		protected internal void End()
		{
			AbstractInterruptibleChannel.BlockedOn(null);
		}

	}

}
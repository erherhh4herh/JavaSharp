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
	/// A channel that can be multiplexed via a <seealso cref="Selector"/>.
	/// 
	/// <para> In order to be used with a selector, an instance of this class must
	/// first be <i>registered</i> via the {@link #register(Selector,int,Object)
	/// register} method.  This method returns a new <seealso cref="SelectionKey"/> object
	/// that represents the channel's registration with the selector.
	/// 
	/// </para>
	/// <para> Once registered with a selector, a channel remains registered until it
	/// is <i>deregistered</i>.  This involves deallocating whatever resources were
	/// allocated to the channel by the selector.
	/// 
	/// </para>
	/// <para> A channel cannot be deregistered directly; instead, the key representing
	/// its registration must be <i>cancelled</i>.  Cancelling a key requests that
	/// the channel be deregistered during the selector's next selection operation.
	/// A key may be cancelled explicitly by invoking its {@link
	/// SelectionKey#cancel() cancel} method.  All of a channel's keys are cancelled
	/// implicitly when the channel is closed, whether by invoking its {@link
	/// Channel#close close} method or by interrupting a thread blocked in an I/O
	/// operation upon the channel.
	/// 
	/// </para>
	/// <para> If the selector itself is closed then the channel will be deregistered,
	/// and the key representing its registration will be invalidated, without
	/// further delay.
	/// 
	/// </para>
	/// <para> A channel may be registered at most once with any particular selector.
	/// 
	/// </para>
	/// <para> Whether or not a channel is registered with one or more selectors may be
	/// determined by invoking the <seealso cref="#isRegistered isRegistered"/> method.
	/// 
	/// </para>
	/// <para> Selectable channels are safe for use by multiple concurrent
	/// threads. </para>
	/// 
	/// 
	/// <a name="bm"></a>
	/// <h2>Blocking mode</h2>
	/// 
	/// A selectable channel is either in <i>blocking</i> mode or in
	/// <i>non-blocking</i> mode.  In blocking mode, every I/O operation invoked
	/// upon the channel will block until it completes.  In non-blocking mode an I/O
	/// operation will never block and may transfer fewer bytes than were requested
	/// or possibly no bytes at all.  The blocking mode of a selectable channel may
	/// be determined by invoking its <seealso cref="#isBlocking isBlocking"/> method.
	/// 
	/// <para> Newly-created selectable channels are always in blocking mode.
	/// Non-blocking mode is most useful in conjunction with selector-based
	/// multiplexing.  A channel must be placed into non-blocking mode before being
	/// registered with a selector, and may not be returned to blocking mode until
	/// it has been deregistered.
	/// 
	/// 
	/// @author Mark Reinhold
	/// @author JSR-51 Expert Group
	/// @since 1.4
	/// 
	/// </para>
	/// </summary>
	/// <seealso cref= SelectionKey </seealso>
	/// <seealso cref= Selector </seealso>

	public abstract class SelectableChannel : AbstractInterruptibleChannel, Channel
	{

		/// <summary>
		/// Initializes a new instance of this class.
		/// </summary>
		protected internal SelectableChannel()
		{
		}

		/// <summary>
		/// Returns the provider that created this channel.
		/// </summary>
		/// <returns>  The provider that created this channel </returns>
		public abstract SelectorProvider Provider();

		/// <summary>
		/// Returns an <a href="SelectionKey.html#opsets">operation set</a>
		/// identifying this channel's supported operations.  The bits that are set
		/// in this integer value denote exactly the operations that are valid for
		/// this channel.  This method always returns the same value for a given
		/// concrete channel class.
		/// </summary>
		/// <returns>  The valid-operation set </returns>
		public abstract int ValidOps();

		// Internal state:
		//   keySet, may be empty but is never null, typ. a tiny array
		//   boolean isRegistered, protected by key set
		//   regLock, lock object to prevent duplicate registrations
		//   boolean isBlocking, protected by regLock

		/// <summary>
		/// Tells whether or not this channel is currently registered with any
		/// selectors.  A newly-created channel is not registered.
		/// 
		/// <para> Due to the inherent delay between key cancellation and channel
		/// deregistration, a channel may remain registered for some time after all
		/// of its keys have been cancelled.  A channel may also remain registered
		/// for some time after it is closed.  </para>
		/// </summary>
		/// <returns> <tt>true</tt> if, and only if, this channel is registered </returns>
		public abstract bool Registered {get;}
		//
		// sync(keySet) { return isRegistered; }

		/// <summary>
		/// Retrieves the key representing the channel's registration with the given
		/// selector.
		/// </summary>
		/// <param name="sel">
		///          The selector
		/// </param>
		/// <returns>  The key returned when this channel was last registered with the
		///          given selector, or <tt>null</tt> if this channel is not
		///          currently registered with that selector </returns>
		public abstract SelectionKey KeyFor(Selector sel);
		//
		// sync(keySet) { return findKey(sel); }

		/// <summary>
		/// Registers this channel with the given selector, returning a selection
		/// key.
		/// 
		/// <para> If this channel is currently registered with the given selector then
		/// the selection key representing that registration is returned.  The key's
		/// interest set will have been changed to <tt>ops</tt>, as if by invoking
		/// the <seealso cref="SelectionKey#interestOps(int) interestOps(int)"/> method.  If
		/// the <tt>att</tt> argument is not <tt>null</tt> then the key's attachment
		/// will have been set to that value.  A <seealso cref="CancelledKeyException"/> will
		/// be thrown if the key has already been cancelled.
		/// 
		/// </para>
		/// <para> Otherwise this channel has not yet been registered with the given
		/// selector, so it is registered and the resulting new key is returned.
		/// The key's initial interest set will be <tt>ops</tt> and its attachment
		/// will be <tt>att</tt>.
		/// 
		/// </para>
		/// <para> This method may be invoked at any time.  If this method is invoked
		/// while another invocation of this method or of the {@link
		/// #configureBlocking(boolean) configureBlocking} method is in progress
		/// then it will first block until the other operation is complete.  This
		/// method will then synchronize on the selector's key set and therefore may
		/// block if invoked concurrently with another registration or selection
		/// operation involving the same selector. </para>
		/// 
		/// <para> If this channel is closed while this operation is in progress then
		/// the key returned by this method will have been cancelled and will
		/// therefore be invalid. </para>
		/// </summary>
		/// <param name="sel">
		///         The selector with which this channel is to be registered
		/// </param>
		/// <param name="ops">
		///         The interest set for the resulting key
		/// </param>
		/// <param name="att">
		///         The attachment for the resulting key; may be <tt>null</tt>
		/// </param>
		/// <exception cref="ClosedChannelException">
		///          If this channel is closed
		/// </exception>
		/// <exception cref="ClosedSelectorException">
		///          If the selector is closed
		/// </exception>
		/// <exception cref="IllegalBlockingModeException">
		///          If this channel is in blocking mode
		/// </exception>
		/// <exception cref="IllegalSelectorException">
		///          If this channel was not created by the same provider
		///          as the given selector
		/// </exception>
		/// <exception cref="CancelledKeyException">
		///          If this channel is currently registered with the given selector
		///          but the corresponding key has already been cancelled
		/// </exception>
		/// <exception cref="IllegalArgumentException">
		///          If a bit in the <tt>ops</tt> set does not correspond to an
		///          operation that is supported by this channel, that is, if
		///          {@code set & ~validOps() != 0}
		/// </exception>
		/// <returns>  A key representing the registration of this channel with
		///          the given selector </returns>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public abstract SelectionKey register(Selector sel, int ops, Object att) throws ClosedChannelException;
		public abstract SelectionKey Register(Selector sel, int ops, Object att);
		//
		// sync(regLock) {
		//   sync(keySet) { look for selector }
		//   if (channel found) { set interest ops -- may block in selector;
		//                        return key; }
		//   create new key -- may block somewhere in selector;
		//   sync(keySet) { add key; }
		//   attach(attachment);
		//   return key;
		// }

		/// <summary>
		/// Registers this channel with the given selector, returning a selection
		/// key.
		/// 
		/// <para> An invocation of this convenience method of the form
		/// 
		/// <blockquote><tt>sc.register(sel, ops)</tt></blockquote>
		/// 
		/// behaves in exactly the same way as the invocation
		/// 
		/// <blockquote><tt>sc.{@link
		/// #register(java.nio.channels.Selector,int,java.lang.Object)
		/// register}(sel, ops, null)</tt></blockquote>
		/// 
		/// </para>
		/// </summary>
		/// <param name="sel">
		///         The selector with which this channel is to be registered
		/// </param>
		/// <param name="ops">
		///         The interest set for the resulting key
		/// </param>
		/// <exception cref="ClosedChannelException">
		///          If this channel is closed
		/// </exception>
		/// <exception cref="ClosedSelectorException">
		///          If the selector is closed
		/// </exception>
		/// <exception cref="IllegalBlockingModeException">
		///          If this channel is in blocking mode
		/// </exception>
		/// <exception cref="IllegalSelectorException">
		///          If this channel was not created by the same provider
		///          as the given selector
		/// </exception>
		/// <exception cref="CancelledKeyException">
		///          If this channel is currently registered with the given selector
		///          but the corresponding key has already been cancelled
		/// </exception>
		/// <exception cref="IllegalArgumentException">
		///          If a bit in <tt>ops</tt> does not correspond to an operation
		///          that is supported by this channel, that is, if {@code set &
		///          ~validOps() != 0}
		/// </exception>
		/// <returns>  A key representing the registration of this channel with
		///          the given selector </returns>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public final SelectionKey register(Selector sel, int ops) throws ClosedChannelException
		public SelectionKey Register(Selector sel, int ops)
		{
			return Register(sel, ops, null);
		}

		/// <summary>
		/// Adjusts this channel's blocking mode.
		/// 
		/// <para> If this channel is registered with one or more selectors then an
		/// attempt to place it into blocking mode will cause an {@link
		/// IllegalBlockingModeException} to be thrown.
		/// 
		/// </para>
		/// <para> This method may be invoked at any time.  The new blocking mode will
		/// only affect I/O operations that are initiated after this method returns.
		/// For some implementations this may require blocking until all pending I/O
		/// operations are complete.
		/// 
		/// </para>
		/// <para> If this method is invoked while another invocation of this method or
		/// of the <seealso cref="#register(Selector, int) register"/> method is in progress
		/// then it will first block until the other operation is complete. </para>
		/// </summary>
		/// <param name="block">  If <tt>true</tt> then this channel will be placed in
		///                blocking mode; if <tt>false</tt> then it will be placed
		///                non-blocking mode
		/// </param>
		/// <returns>  This selectable channel
		/// </returns>
		/// <exception cref="ClosedChannelException">
		///          If this channel is closed
		/// </exception>
		/// <exception cref="IllegalBlockingModeException">
		///          If <tt>block</tt> is <tt>true</tt> and this channel is
		///          registered with one or more selectors
		/// </exception>
		/// <exception cref="IOException">
		///         If an I/O error occurs </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public abstract SelectableChannel configureBlocking(boolean block) throws java.io.IOException;
		public abstract SelectableChannel ConfigureBlocking(bool block);
		//
		// sync(regLock) {
		//   sync(keySet) { throw IBME if block && isRegistered; }
		//   change mode;
		// }

		/// <summary>
		/// Tells whether or not every I/O operation on this channel will block
		/// until it completes.  A newly-created channel is always in blocking mode.
		/// 
		/// <para> If this channel is closed then the value returned by this method is
		/// not specified. </para>
		/// </summary>
		/// <returns> <tt>true</tt> if, and only if, this channel is in blocking mode </returns>
		public abstract bool Blocking {get;}

		/// <summary>
		/// Retrieves the object upon which the {@link #configureBlocking
		/// configureBlocking} and <seealso cref="#register register"/> methods synchronize.
		/// This is often useful in the implementation of adaptors that require a
		/// specific blocking mode to be maintained for a short period of time.
		/// </summary>
		/// <returns>  The blocking-mode lock object </returns>
		public abstract Object BlockingLock();

	}

}
using System.Diagnostics;

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



	/// <summary>
	/// Base implementation class for selectable channels.
	/// 
	/// <para> This class defines methods that handle the mechanics of channel
	/// registration, deregistration, and closing.  It maintains the current
	/// blocking mode of this channel as well as its current set of selection keys.
	/// It performs all of the synchronization required to implement the {@link
	/// java.nio.channels.SelectableChannel} specification.  Implementations of the
	/// abstract protected methods defined in this class need not synchronize
	/// against other threads that might be engaged in the same operations.  </para>
	/// 
	/// 
	/// @author Mark Reinhold
	/// @author Mike McCloskey
	/// @author JSR-51 Expert Group
	/// @since 1.4
	/// </summary>

	public abstract class AbstractSelectableChannel : SelectableChannel
	{

		// The provider that created this channel
		private readonly SelectorProvider Provider_Renamed;

		// Keys that have been created by registering this channel with selectors.
		// They are saved because if this channel is closed the keys must be
		// deregistered.  Protected by keyLock.
		//
		private SelectionKey[] Keys = null;
		private int KeyCount = 0;

		// Lock for key set and count
		private readonly Object KeyLock = new Object();

		// Lock for registration and configureBlocking operations
		private readonly Object RegLock = new Object();

		// Blocking mode, protected by regLock
		internal bool Blocking_Renamed = true;

		/// <summary>
		/// Initializes a new instance of this class.
		/// </summary>
		/// <param name="provider">
		///         The provider that created this channel </param>
		protected internal AbstractSelectableChannel(SelectorProvider provider)
		{
			this.Provider_Renamed = provider;
		}

		/// <summary>
		/// Returns the provider that created this channel.
		/// </summary>
		/// <returns>  The provider that created this channel </returns>
		public sealed override SelectorProvider Provider()
		{
			return Provider_Renamed;
		}


		// -- Utility methods for the key set --

		private void AddKey(SelectionKey k)
		{
			Debug.Assert(Thread.holdsLock(KeyLock));
			int i = 0;
			if ((Keys != null) && (KeyCount < Keys.Length))
			{
				// Find empty element of key array
				for (i = 0; i < Keys.Length; i++)
				{
					if (Keys[i] == null)
					{
						break;
					}
				}
			}
			else if (Keys == null)
			{
				Keys = new SelectionKey[3];
			}
			else
			{
				// Grow key array
				int n = Keys.Length * 2;
				SelectionKey[] ks = new SelectionKey[n];
				for (i = 0; i < Keys.Length; i++)
				{
					ks[i] = Keys[i];
				}
				Keys = ks;
				i = KeyCount;
			}
			Keys[i] = k;
			KeyCount++;
		}

		private SelectionKey FindKey(Selector sel)
		{
			lock (KeyLock)
			{
				if (Keys == null)
				{
					return null;
				}
				for (int i = 0; i < Keys.Length; i++)
				{
					if ((Keys[i] != null) && (Keys[i].Selector() == sel))
					{
						return Keys[i];
					}
				}
				return null;
			}
		}

		internal virtual void RemoveKey(SelectionKey k) // package-private
		{
			lock (KeyLock)
			{
				for (int i = 0; i < Keys.Length; i++)
				{
					if (Keys[i] == k)
					{
						Keys[i] = null;
						KeyCount--;
					}((AbstractSelectionKey)k).invalidate();
				}
			}
		}

		private bool HaveValidKeys()
		{
			lock (KeyLock)
			{
				if (KeyCount == 0)
				{
					return false;
				}
				for (int i = 0; i < Keys.Length; i++)
				{
					if ((Keys[i] != null) && Keys[i].Valid)
					{
						return true;
					}
				}
				return false;
			}
		}


		// -- Registration --

		public sealed override bool Registered
		{
			get
			{
				lock (KeyLock)
				{
					return KeyCount != 0;
				}
			}
		}

		public sealed override SelectionKey KeyFor(Selector sel)
		{
			return FindKey(sel);
		}

		/// <summary>
		/// Registers this channel with the given selector, returning a selection key.
		/// 
		/// <para>  This method first verifies that this channel is open and that the
		/// given initial interest set is valid.
		/// 
		/// </para>
		/// <para> If this channel is already registered with the given selector then
		/// the selection key representing that registration is returned after
		/// setting its interest set to the given value.
		/// 
		/// </para>
		/// <para> Otherwise this channel has not yet been registered with the given
		/// selector, so the <seealso cref="AbstractSelector#register register"/> method of
		/// the selector is invoked while holding the appropriate locks.  The
		/// resulting key is added to this channel's key set before being returned.
		/// </para>
		/// </summary>
		/// <exception cref="ClosedSelectorException"> {@inheritDoc}
		/// </exception>
		/// <exception cref="IllegalBlockingModeException"> {@inheritDoc}
		/// </exception>
		/// <exception cref="IllegalSelectorException"> {@inheritDoc}
		/// </exception>
		/// <exception cref="CancelledKeyException"> {@inheritDoc}
		/// </exception>
		/// <exception cref="IllegalArgumentException"> {@inheritDoc} </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public final SelectionKey register(Selector sel, int ops, Object att) throws ClosedChannelException
		public sealed override SelectionKey Register(Selector sel, int ops, Object att)
		{
			lock (RegLock)
			{
				if (!Open)
				{
					throw new ClosedChannelException();
				}
				if ((ops & ~ValidOps()) != 0)
				{
					throw new IllegalArgumentException();
				}
				if (Blocking_Renamed)
				{
					throw new IllegalBlockingModeException();
				}
				SelectionKey k = FindKey(sel);
				if (k != null)
				{
					k.InterestOps(ops);
					k.Attach(att);
				}
				if (k == null)
				{
					// New registration
					lock (KeyLock)
					{
						if (!Open)
						{
							throw new ClosedChannelException();
						}
						k = ((AbstractSelector)sel).Register(this, ops, att);
						AddKey(k);
					}
				}
				return k;
			}
		}


		// -- Closing --

		/// <summary>
		/// Closes this channel.
		/// 
		/// <para> This method, which is specified in the {@link
		/// AbstractInterruptibleChannel} class and is invoked by the {@link
		/// java.nio.channels.Channel#close close} method, in turn invokes the
		/// <seealso cref="#implCloseSelectableChannel implCloseSelectableChannel"/> method in
		/// order to perform the actual work of closing this channel.  It then
		/// cancels all of this channel's keys.  </para>
		/// </summary>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: protected final void implCloseChannel() throws java.io.IOException
		protected internal sealed override void ImplCloseChannel()
		{
			ImplCloseSelectableChannel();
			lock (KeyLock)
			{
				int count = (Keys == null) ? 0 : Keys.Length;
				for (int i = 0; i < count; i++)
				{
					SelectionKey k = Keys[i];
					if (k != null)
					{
						k.Cancel();
					}
				}
			}
		}

		/// <summary>
		/// Closes this selectable channel.
		/// 
		/// <para> This method is invoked by the {@link java.nio.channels.Channel#close
		/// close} method in order to perform the actual work of closing the
		/// channel.  This method is only invoked if the channel has not yet been
		/// closed, and it is never invoked more than once.
		/// 
		/// </para>
		/// <para> An implementation of this method must arrange for any other thread
		/// that is blocked in an I/O operation upon this channel to return
		/// immediately, either by throwing an exception or by returning normally.
		/// </para>
		/// </summary>
		/// <exception cref="IOException">
		///          If an I/O error occurs </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: protected abstract void implCloseSelectableChannel() throws java.io.IOException;
		protected internal abstract void ImplCloseSelectableChannel();


		// -- Blocking --

		public sealed override bool Blocking
		{
			get
			{
				lock (RegLock)
				{
					return Blocking_Renamed;
				}
			}
		}

		public sealed override Object BlockingLock()
		{
			return RegLock;
		}

		/// <summary>
		/// Adjusts this channel's blocking mode.
		/// 
		/// <para> If the given blocking mode is different from the current blocking
		/// mode then this method invokes the {@link #implConfigureBlocking
		/// implConfigureBlocking} method, while holding the appropriate locks, in
		/// order to change the mode.  </para>
		/// </summary>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public final SelectableChannel configureBlocking(boolean block) throws java.io.IOException
		public sealed override SelectableChannel ConfigureBlocking(bool block)
		{
			lock (RegLock)
			{
				if (!Open)
				{
					throw new ClosedChannelException();
				}
				if (Blocking_Renamed == block)
				{
					return this;
				}
				if (block && HaveValidKeys())
				{
					throw new IllegalBlockingModeException();
				}
				ImplConfigureBlocking(block);
				Blocking_Renamed = block;
			}
			return this;
		}

		/// <summary>
		/// Adjusts this channel's blocking mode.
		/// 
		/// <para> This method is invoked by the {@link #configureBlocking
		/// configureBlocking} method in order to perform the actual work of
		/// changing the blocking mode.  This method is only invoked if the new mode
		/// is different from the current mode.  </para>
		/// </summary>
		/// <param name="block">  If <tt>true</tt> then this channel will be placed in
		///                blocking mode; if <tt>false</tt> then it will be placed
		///                non-blocking mode
		/// </param>
		/// <exception cref="IOException">
		///         If an I/O error occurs </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: protected abstract void implConfigureBlocking(boolean block) throws java.io.IOException;
		protected internal abstract void ImplConfigureBlocking(bool block);

	}

}
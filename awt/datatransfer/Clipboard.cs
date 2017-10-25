using System.Collections.Generic;

/*
 * Copyright (c) 1996, 2013, Oracle and/or its affiliates. All rights reserved.
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

namespace java.awt.datatransfer
{


	using EventListenerAggregate = sun.awt.EventListenerAggregate;

	/// <summary>
	/// A class that implements a mechanism to transfer data using
	/// cut/copy/paste operations.
	/// <para>
	/// <seealso cref="FlavorListener"/>s may be registered on an instance of the
	/// Clipboard class to be notified about changes to the set of
	/// <seealso cref="DataFlavor"/>s available on this clipboard (see
	/// <seealso cref="#addFlavorListener"/>).
	/// 
	/// </para>
	/// </summary>
	/// <seealso cref= java.awt.Toolkit#getSystemClipboard </seealso>
	/// <seealso cref= java.awt.Toolkit#getSystemSelection
	/// 
	/// @author      Amy Fowler
	/// @author      Alexander Gerasimov </seealso>
	public class Clipboard
	{

		internal String Name_Renamed;

		protected internal ClipboardOwner Owner;
		protected internal Transferable Contents;

		/// <summary>
		/// An aggregate of flavor listeners registered on this local clipboard.
		/// 
		/// @since 1.5
		/// </summary>
		private EventListenerAggregate FlavorListeners_Renamed;

		/// <summary>
		/// A set of <code>DataFlavor</code>s that is available on
		/// this local clipboard. It is used for tracking changes
		/// of <code>DataFlavor</code>s available on this clipboard.
		/// 
		/// @since 1.5
		/// </summary>
		private Set<DataFlavor> CurrentDataFlavors;

		/// <summary>
		/// Creates a clipboard object.
		/// </summary>
		/// <seealso cref= java.awt.Toolkit#getSystemClipboard </seealso>
		public Clipboard(String name)
		{
			this.Name_Renamed = name;
		}

		/// <summary>
		/// Returns the name of this clipboard object.
		/// </summary>
		/// <seealso cref= java.awt.Toolkit#getSystemClipboard </seealso>
		public virtual String Name
		{
			get
			{
				return Name_Renamed;
			}
		}

		/// <summary>
		/// Sets the current contents of the clipboard to the specified
		/// transferable object and registers the specified clipboard owner
		/// as the owner of the new contents.
		/// <para>
		/// If there is an existing owner different from the argument
		/// <code>owner</code>, that owner is notified that it no longer
		/// holds ownership of the clipboard contents via an invocation
		/// of <code>ClipboardOwner.lostOwnership()</code> on that owner.
		/// An implementation of <code>setContents()</code> is free not
		/// to invoke <code>lostOwnership()</code> directly from this method.
		/// For example, <code>lostOwnership()</code> may be invoked later on
		/// a different thread. The same applies to <code>FlavorListener</code>s
		/// registered on this clipboard.
		/// </para>
		/// <para>
		/// The method throws <code>IllegalStateException</code> if the clipboard
		/// is currently unavailable. For example, on some platforms, the system
		/// clipboard is unavailable while it is accessed by another application.
		/// 
		/// </para>
		/// </summary>
		/// <param name="contents"> the transferable object representing the
		///                 clipboard content </param>
		/// <param name="owner"> the object which owns the clipboard content </param>
		/// <exception cref="IllegalStateException"> if the clipboard is currently unavailable </exception>
		/// <seealso cref= java.awt.Toolkit#getSystemClipboard </seealso>
		public virtual void SetContents(Transferable contents, ClipboardOwner owner)
		{
			lock (this)
			{
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final ClipboardOwner oldOwner = this.owner;
				ClipboardOwner oldOwner = this.Owner;
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final Transferable oldContents = this.contents;
				Transferable oldContents = this.Contents;
        
				this.Owner = owner;
				this.Contents = contents;
        
				if (oldOwner != null && oldOwner != owner)
				{
					EventQueue.InvokeLater(new RunnableAnonymousInnerClassHelper(this, oldOwner, oldContents));
				}
				FireFlavorsChanged();
			}
		}

		private class RunnableAnonymousInnerClassHelper : Runnable
		{
			private readonly Clipboard OuterInstance;

			private java.awt.datatransfer.ClipboardOwner OldOwner;
			private java.awt.datatransfer.Transferable OldContents;

			public RunnableAnonymousInnerClassHelper(Clipboard outerInstance, java.awt.datatransfer.ClipboardOwner oldOwner, java.awt.datatransfer.Transferable oldContents)
			{
				this.OuterInstance = outerInstance;
				this.OldOwner = oldOwner;
				this.OldContents = oldContents;
			}

			public virtual void Run()
			{
				OldOwner.LostOwnership(OuterInstance, OldContents);
			}
		}

		/// <summary>
		/// Returns a transferable object representing the current contents
		/// of the clipboard.  If the clipboard currently has no contents,
		/// it returns <code>null</code>. The parameter Object requestor is
		/// not currently used.  The method throws
		/// <code>IllegalStateException</code> if the clipboard is currently
		/// unavailable.  For example, on some platforms, the system clipboard is
		/// unavailable while it is accessed by another application.
		/// </summary>
		/// <param name="requestor"> the object requesting the clip data  (not used) </param>
		/// <returns> the current transferable object on the clipboard </returns>
		/// <exception cref="IllegalStateException"> if the clipboard is currently unavailable </exception>
		/// <seealso cref= java.awt.Toolkit#getSystemClipboard </seealso>
		public virtual Transferable GetContents(Object requestor)
		{
			lock (this)
			{
				return Contents;
			}
		}


		/// <summary>
		/// Returns an array of <code>DataFlavor</code>s in which the current
		/// contents of this clipboard can be provided. If there are no
		/// <code>DataFlavor</code>s available, this method returns a zero-length
		/// array.
		/// </summary>
		/// <returns> an array of <code>DataFlavor</code>s in which the current
		///         contents of this clipboard can be provided
		/// </returns>
		/// <exception cref="IllegalStateException"> if this clipboard is currently unavailable
		/// 
		/// @since 1.5 </exception>
		public virtual DataFlavor[] AvailableDataFlavors
		{
			get
			{
				Transferable cntnts = GetContents(null);
				if (cntnts == null)
				{
					return new DataFlavor[0];
				}
				return cntnts.TransferDataFlavors;
			}
		}

		/// <summary>
		/// Returns whether or not the current contents of this clipboard can be
		/// provided in the specified <code>DataFlavor</code>.
		/// </summary>
		/// <param name="flavor"> the requested <code>DataFlavor</code> for the contents
		/// </param>
		/// <returns> <code>true</code> if the current contents of this clipboard
		///         can be provided in the specified <code>DataFlavor</code>;
		///         <code>false</code> otherwise
		/// </returns>
		/// <exception cref="NullPointerException"> if <code>flavor</code> is <code>null</code> </exception>
		/// <exception cref="IllegalStateException"> if this clipboard is currently unavailable
		/// 
		/// @since 1.5 </exception>
		public virtual bool IsDataFlavorAvailable(DataFlavor flavor)
		{
			if (flavor == null)
			{
				throw new NullPointerException("flavor");
			}

			Transferable cntnts = GetContents(null);
			if (cntnts == null)
			{
				return false;
			}
			return cntnts.IsDataFlavorSupported(flavor);
		}

		/// <summary>
		/// Returns an object representing the current contents of this clipboard
		/// in the specified <code>DataFlavor</code>.
		/// The class of the object returned is defined by the representation
		/// class of <code>flavor</code>.
		/// </summary>
		/// <param name="flavor"> the requested <code>DataFlavor</code> for the contents
		/// </param>
		/// <returns> an object representing the current contents of this clipboard
		///         in the specified <code>DataFlavor</code>
		/// </returns>
		/// <exception cref="NullPointerException"> if <code>flavor</code> is <code>null</code> </exception>
		/// <exception cref="IllegalStateException"> if this clipboard is currently unavailable </exception>
		/// <exception cref="UnsupportedFlavorException"> if the requested <code>DataFlavor</code>
		///         is not available </exception>
		/// <exception cref="IOException"> if the data in the requested <code>DataFlavor</code>
		///         can not be retrieved
		/// </exception>
		/// <seealso cref= DataFlavor#getRepresentationClass
		/// 
		/// @since 1.5 </seealso>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public Object getData(DataFlavor flavor) throws UnsupportedFlavorException, java.io.IOException
		public virtual Object GetData(DataFlavor flavor)
		{
			if (flavor == null)
			{
				throw new NullPointerException("flavor");
			}

			Transferable cntnts = GetContents(null);
			if (cntnts == null)
			{
				throw new UnsupportedFlavorException(flavor);
			}
			return cntnts.GetTransferData(flavor);
		}


		/// <summary>
		/// Registers the specified <code>FlavorListener</code> to receive
		/// <code>FlavorEvent</code>s from this clipboard.
		/// If <code>listener</code> is <code>null</code>, no exception
		/// is thrown and no action is performed.
		/// </summary>
		/// <param name="listener"> the listener to be added
		/// </param>
		/// <seealso cref= #removeFlavorListener </seealso>
		/// <seealso cref= #getFlavorListeners </seealso>
		/// <seealso cref= FlavorListener </seealso>
		/// <seealso cref= FlavorEvent
		/// @since 1.5 </seealso>
		public virtual void AddFlavorListener(FlavorListener listener)
		{
			lock (this)
			{
				if (listener == null)
				{
					return;
				}
				if (FlavorListeners_Renamed == null)
				{
					CurrentDataFlavors = AvailableDataFlavorSet;
					FlavorListeners_Renamed = new EventListenerAggregate(typeof(FlavorListener));
				}
				FlavorListeners_Renamed.add(listener);
			}
		}

		/// <summary>
		/// Removes the specified <code>FlavorListener</code> so that it no longer
		/// receives <code>FlavorEvent</code>s from this <code>Clipboard</code>.
		/// This method performs no function, nor does it throw an exception, if
		/// the listener specified by the argument was not previously added to this
		/// <code>Clipboard</code>.
		/// If <code>listener</code> is <code>null</code>, no exception
		/// is thrown and no action is performed.
		/// </summary>
		/// <param name="listener"> the listener to be removed
		/// </param>
		/// <seealso cref= #addFlavorListener </seealso>
		/// <seealso cref= #getFlavorListeners </seealso>
		/// <seealso cref= FlavorListener </seealso>
		/// <seealso cref= FlavorEvent
		/// @since 1.5 </seealso>
		public virtual void RemoveFlavorListener(FlavorListener listener)
		{
			lock (this)
			{
				if (listener == null || FlavorListeners_Renamed == null)
				{
					return;
				}
				FlavorListeners_Renamed.remove(listener);
			}
		}

		/// <summary>
		/// Returns an array of all the <code>FlavorListener</code>s currently
		/// registered on this <code>Clipboard</code>.
		/// </summary>
		/// <returns> all of this clipboard's <code>FlavorListener</code>s or an empty
		///         array if no listeners are currently registered </returns>
		/// <seealso cref= #addFlavorListener </seealso>
		/// <seealso cref= #removeFlavorListener </seealso>
		/// <seealso cref= FlavorListener </seealso>
		/// <seealso cref= FlavorEvent
		/// @since 1.5 </seealso>
		public virtual FlavorListener[] FlavorListeners
		{
			get
			{
				lock (this)
				{
					return FlavorListeners_Renamed == null ? new FlavorListener[0] : (FlavorListener[])FlavorListeners_Renamed.ListenersCopy;
				}
			}
		}

		/// <summary>
		/// Checks change of the <code>DataFlavor</code>s and, if necessary,
		/// notifies all listeners that have registered interest for notification
		/// on <code>FlavorEvent</code>s.
		/// 
		/// @since 1.5
		/// </summary>
		private void FireFlavorsChanged()
		{
			if (FlavorListeners_Renamed == null)
			{
				return;
			}
			Set<DataFlavor> prevDataFlavors = CurrentDataFlavors;
			CurrentDataFlavors = AvailableDataFlavorSet;
			if (prevDataFlavors.Equals(CurrentDataFlavors))
			{
				return;
			}
			FlavorListener[] flavorListenerArray = (FlavorListener[])FlavorListeners_Renamed.ListenersInternal;
			for (int i = 0; i < flavorListenerArray.Length; i++)
			{
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final FlavorListener listener = flavorListenerArray[i];
				FlavorListener listener = flavorListenerArray[i];
				EventQueue.InvokeLater(new RunnableAnonymousInnerClassHelper2(this, listener));
			}
		}

		private class RunnableAnonymousInnerClassHelper2 : Runnable
		{
			private readonly Clipboard OuterInstance;

			private java.awt.datatransfer.FlavorListener Listener;

			public RunnableAnonymousInnerClassHelper2(Clipboard outerInstance, java.awt.datatransfer.FlavorListener listener)
			{
				this.OuterInstance = outerInstance;
				this.Listener = listener;
			}

			public virtual void Run()
			{
				Listener.FlavorsChanged(new FlavorEvent(OuterInstance));
			}
		}

		/// <summary>
		/// Returns a set of <code>DataFlavor</code>s currently available
		/// on this clipboard.
		/// </summary>
		/// <returns> a set of <code>DataFlavor</code>s currently available
		///         on this clipboard
		/// 
		/// @since 1.5 </returns>
		private Set<DataFlavor> AvailableDataFlavorSet
		{
			get
			{
				Set<DataFlavor> set = new HashSet<DataFlavor>();
				Transferable contents = GetContents(null);
				if (contents != null)
				{
					DataFlavor[] flavors = contents.TransferDataFlavors;
					if (flavors != null)
					{
						set.AddAll(Arrays.AsList(flavors));
					}
				}
				return set;
			}
		}
	}

}
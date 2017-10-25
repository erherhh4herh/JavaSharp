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
namespace java.awt
{



	/// <summary>
	/// The <code>Choice</code> class presents a pop-up menu of choices.
	/// The current choice is displayed as the title of the menu.
	/// <para>
	/// The following code example produces a pop-up menu:
	/// 
	/// <hr><blockquote><pre>
	/// Choice ColorChooser = new Choice();
	/// ColorChooser.add("Green");
	/// ColorChooser.add("Red");
	/// ColorChooser.add("Blue");
	/// </pre></blockquote><hr>
	/// </para>
	/// <para>
	/// After this choice menu has been added to a panel,
	/// it appears as follows in its normal state:
	/// </para>
	/// <para>
	/// <img src="doc-files/Choice-1.gif" alt="The following text describes the graphic"
	/// style="float:center; margin: 7px 10px;">
	/// </para>
	/// <para>
	/// In the picture, <code>"Green"</code> is the current choice.
	/// Pushing the mouse button down on the object causes a menu to
	/// appear with the current choice highlighted.
	/// </para>
	/// <para>
	/// Some native platforms do not support arbitrary resizing of
	/// <code>Choice</code> components and the behavior of
	/// <code>setSize()/getSize()</code> is bound by
	/// such limitations.
	/// Native GUI <code>Choice</code> components' size are often bound by such
	/// attributes as font size and length of items contained within
	/// the <code>Choice</code>.
	/// </para>
	/// <para>
	/// @author      Sami Shaio
	/// @author      Arthur van Hoff
	/// @since       JDK1.0
	/// </para>
	/// </summary>
	public class Choice : Component, ItemSelectable, Accessible
	{
		/// <summary>
		/// The items for the <code>Choice</code>.
		/// This can be a <code>null</code> value.
		/// @serial </summary>
		/// <seealso cref= #add(String) </seealso>
		/// <seealso cref= #addItem(String) </seealso>
		/// <seealso cref= #getItem(int) </seealso>
		/// <seealso cref= #getItemCount() </seealso>
		/// <seealso cref= #insert(String, int) </seealso>
		/// <seealso cref= #remove(String) </seealso>
		internal Vector<String> PItems;

		/// <summary>
		/// The index of the current choice for this <code>Choice</code>
		/// or -1 if nothing is selected.
		/// @serial </summary>
		/// <seealso cref= #getSelectedItem() </seealso>
		/// <seealso cref= #select(int) </seealso>
		internal int SelectedIndex_Renamed = -1;

		[NonSerialized]
		internal ItemListener ItemListener;

		private const String @base = "choice";
		private static int NameCounter = 0;

		/*
		 * JDK 1.1 serialVersionUID
		 */
		private const long SerialVersionUID = -4075310674757313071L;

		static Choice()
		{
			/* ensure that the necessary native libraries are loaded */
			Toolkit.LoadLibraries();
			/* initialize JNI field and method ids */
			if (!GraphicsEnvironment.Headless)
			{
				initIDs();
			}
		}

		/// <summary>
		/// Creates a new choice menu. The menu initially has no items in it.
		/// <para>
		/// By default, the first item added to the choice menu becomes the
		/// selected item, until a different selection is made by the user
		/// by calling one of the <code>select</code> methods.
		/// </para>
		/// </summary>
		/// <exception cref="HeadlessException"> if GraphicsEnvironment.isHeadless()
		/// returns true </exception>
		/// <seealso cref=       java.awt.GraphicsEnvironment#isHeadless </seealso>
		/// <seealso cref=       #select(int) </seealso>
		/// <seealso cref=       #select(java.lang.String) </seealso>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public Choice() throws HeadlessException
		public Choice()
		{
			GraphicsEnvironment.CheckHeadless();
			PItems = new Vector<>();
		}

		/// <summary>
		/// Constructs a name for this component.  Called by
		/// <code>getName</code> when the name is <code>null</code>.
		/// </summary>
		internal override String ConstructComponentName()
		{
			lock (typeof(Choice))
			{
				return @base + NameCounter++;
			}
		}

		/// <summary>
		/// Creates the <code>Choice</code>'s peer.  This peer allows us
		/// to change the look
		/// of the <code>Choice</code> without changing its functionality. </summary>
		/// <seealso cref=     java.awt.Toolkit#createChoice(java.awt.Choice) </seealso>
		/// <seealso cref=     java.awt.Component#getToolkit() </seealso>
		public override void AddNotify()
		{
			lock (TreeLock)
			{
				if (Peer_Renamed == null)
				{
					Peer_Renamed = Toolkit.CreateChoice(this);
				}
				base.AddNotify();
			}
		}

		/// <summary>
		/// Returns the number of items in this <code>Choice</code> menu. </summary>
		/// <returns> the number of items in this <code>Choice</code> menu </returns>
		/// <seealso cref=     #getItem
		/// @since   JDK1.1 </seealso>
		public virtual int ItemCount
		{
			get
			{
				return CountItems();
			}
		}

		/// @deprecated As of JDK version 1.1,
		/// replaced by <code>getItemCount()</code>. 
		[Obsolete("As of JDK version 1.1,")]
		public virtual int CountItems()
		{
			return PItems.Size();
		}

		/// <summary>
		/// Gets the string at the specified index in this
		/// <code>Choice</code> menu. </summary>
		/// <param name="index"> the index at which to begin </param>
		/// <seealso cref=        #getItemCount </seealso>
		public virtual String GetItem(int index)
		{
			return GetItemImpl(index);
		}

		/*
		 * This is called by the native code, so client code can't
		 * be called on the toolkit thread.
		 */
		internal String GetItemImpl(int index)
		{
			return PItems.ElementAt(index);
		}

		/// <summary>
		/// Adds an item to this <code>Choice</code> menu. </summary>
		/// <param name="item">    the item to be added </param>
		/// <exception cref="NullPointerException">   if the item's value is
		///                  <code>null</code>
		/// @since      JDK1.1 </exception>
		public virtual void Add(String item)
		{
			AddItem(item);
		}

		/// <summary>
		/// Obsolete as of Java 2 platform v1.1.  Please use the
		/// <code>add</code> method instead.
		/// <para>
		/// Adds an item to this <code>Choice</code> menu.
		/// </para>
		/// </summary>
		/// <param name="item"> the item to be added </param>
		/// <exception cref="NullPointerException"> if the item's value is equal to
		///          <code>null</code> </exception>
		public virtual void AddItem(String item)
		{
			lock (this)
			{
				InsertNoInvalidate(item, PItems.Size());
			}

			// This could change the preferred size of the Component.
			InvalidateIfValid();
		}

		/// <summary>
		/// Inserts an item to this <code>Choice</code>,
		/// but does not invalidate the <code>Choice</code>.
		/// Client methods must provide their own synchronization before
		/// invoking this method. </summary>
		/// <param name="item"> the item to be added </param>
		/// <param name="index"> the new item position </param>
		/// <exception cref="NullPointerException"> if the item's value is equal to
		///          <code>null</code> </exception>
		private void InsertNoInvalidate(String item, int index)
		{
			if (item == null)
			{
				throw new NullPointerException("cannot add null item to Choice");
			}
			PItems.InsertElementAt(item, index);
			ChoicePeer peer = (ChoicePeer)this.Peer_Renamed;
			if (peer != null)
			{
				peer.Add(item, index);
			}
			// no selection or selection shifted up
			if (SelectedIndex_Renamed < 0 || SelectedIndex_Renamed >= index)
			{
				Select(0);
			}
		}


		/// <summary>
		/// Inserts the item into this choice at the specified position.
		/// Existing items at an index greater than or equal to
		/// <code>index</code> are shifted up by one to accommodate
		/// the new item.  If <code>index</code> is greater than or
		/// equal to the number of items in this choice,
		/// <code>item</code> is added to the end of this choice.
		/// <para>
		/// If the item is the first one being added to the choice,
		/// then the item becomes selected.  Otherwise, if the
		/// selected item was one of the items shifted, the first
		/// item in the choice becomes the selected item.  If the
		/// selected item was no among those shifted, it remains
		/// the selected item.
		/// </para>
		/// </summary>
		/// <param name="item"> the non-<code>null</code> item to be inserted </param>
		/// <param name="index"> the position at which the item should be inserted </param>
		/// <exception cref="IllegalArgumentException"> if index is less than 0 </exception>
		public virtual void Insert(String item, int index)
		{
			lock (this)
			{
				if (index < 0)
				{
					throw new IllegalArgumentException("index less than zero.");
				}
				/* if the index greater than item count, add item to the end */
				index = System.Math.Min(index, PItems.Size());

				InsertNoInvalidate(item, index);
			}

			// This could change the preferred size of the Component.
			InvalidateIfValid();
		}

		/// <summary>
		/// Removes the first occurrence of <code>item</code>
		/// from the <code>Choice</code> menu.  If the item
		/// being removed is the currently selected item,
		/// then the first item in the choice becomes the
		/// selected item.  Otherwise, the currently selected
		/// item remains selected (and the selected index is
		/// updated accordingly). </summary>
		/// <param name="item">  the item to remove from this <code>Choice</code> menu </param>
		/// <exception cref="IllegalArgumentException">  if the item doesn't
		///                     exist in the choice menu
		/// @since      JDK1.1 </exception>
		public virtual void Remove(String item)
		{
			lock (this)
			{
				int index = PItems.IndexOf(item);
				if (index < 0)
				{
					throw new IllegalArgumentException("item " + item + " not found in choice");
				}
				else
				{
					RemoveNoInvalidate(index);
				}
			}

			// This could change the preferred size of the Component.
			InvalidateIfValid();
		}

		/// <summary>
		/// Removes an item from the choice menu
		/// at the specified position.  If the item
		/// being removed is the currently selected item,
		/// then the first item in the choice becomes the
		/// selected item.  Otherwise, the currently selected
		/// item remains selected (and the selected index is
		/// updated accordingly). </summary>
		/// <param name="position"> the position of the item </param>
		/// <exception cref="IndexOutOfBoundsException"> if the specified
		///          position is out of bounds
		/// @since      JDK1.1 </exception>
		public virtual void Remove(int position)
		{
			lock (this)
			{
				RemoveNoInvalidate(position);
			}

			// This could change the preferred size of the Component.
			InvalidateIfValid();
		}

		/// <summary>
		/// Removes an item from the <code>Choice</code> at the
		/// specified position, but does not invalidate the <code>Choice</code>.
		/// Client methods must provide their
		/// own synchronization before invoking this method. </summary>
		/// <param name="position">   the position of the item </param>
		private void RemoveNoInvalidate(int position)
		{
			PItems.RemoveElementAt(position);
			ChoicePeer peer = (ChoicePeer)this.Peer_Renamed;
			if (peer != null)
			{
				peer.Remove(position);
			}
			/* Adjust selectedIndex if selected item was removed. */
			if (PItems.Size() == 0)
			{
				SelectedIndex_Renamed = -1;
			}
			else if (SelectedIndex_Renamed == position)
			{
				Select(0);
			}
			else if (SelectedIndex_Renamed > position)
			{
				Select(SelectedIndex_Renamed - 1);
			}
		}


		/// <summary>
		/// Removes all items from the choice menu. </summary>
		/// <seealso cref=       #remove
		/// @since     JDK1.1 </seealso>
		public virtual void RemoveAll()
		{
			lock (this)
			{
				if (Peer_Renamed != null)
				{
					((ChoicePeer)Peer_Renamed).RemoveAll();
				}
				PItems.RemoveAllElements();
				SelectedIndex_Renamed = -1;
			}

			// This could change the preferred size of the Component.
			InvalidateIfValid();
		}

		/// <summary>
		/// Gets a representation of the current choice as a string. </summary>
		/// <returns>    a string representation of the currently
		///                     selected item in this choice menu </returns>
		/// <seealso cref=       #getSelectedIndex </seealso>
		public virtual String SelectedItem
		{
			get
			{
				lock (this)
				{
					return (SelectedIndex_Renamed >= 0) ? GetItem(SelectedIndex_Renamed) : null;
				}
			}
		}

		/// <summary>
		/// Returns an array (length 1) containing the currently selected
		/// item.  If this choice has no items, returns <code>null</code>. </summary>
		/// <seealso cref= ItemSelectable </seealso>
		public virtual Object[] SelectedObjects
		{
			get
			{
				lock (this)
				{
					if (SelectedIndex_Renamed >= 0)
					{
						Object[] items = new Object[1];
						items[0] = GetItem(SelectedIndex_Renamed);
						return items;
					}
					return null;
				}
			}
		}

		/// <summary>
		/// Returns the index of the currently selected item.
		/// If nothing is selected, returns -1.
		/// </summary>
		/// <returns> the index of the currently selected item, or -1 if nothing
		///  is currently selected </returns>
		/// <seealso cref= #getSelectedItem </seealso>
		public virtual int SelectedIndex
		{
			get
			{
				return SelectedIndex_Renamed;
			}
		}

		/// <summary>
		/// Sets the selected item in this <code>Choice</code> menu to be the
		/// item at the specified position.
		/// 
		/// <para>Note that this method should be primarily used to
		/// initially select an item in this component.
		/// Programmatically calling this method will <i>not</i> trigger
		/// an <code>ItemEvent</code>.  The only way to trigger an
		/// <code>ItemEvent</code> is by user interaction.
		/// 
		/// </para>
		/// </summary>
		/// <param name="pos">      the position of the selected item </param>
		/// <exception cref="IllegalArgumentException"> if the specified
		///                            position is greater than the
		///                            number of items or less than zero </exception>
		/// <seealso cref=        #getSelectedItem </seealso>
		/// <seealso cref=        #getSelectedIndex </seealso>
		public virtual void Select(int pos)
		{
			lock (this)
			{
				if ((pos >= PItems.Size()) || (pos < 0))
				{
					throw new IllegalArgumentException("illegal Choice item position: " + pos);
				}
				if (PItems.Size() > 0)
				{
					SelectedIndex_Renamed = pos;
					ChoicePeer peer = (ChoicePeer)this.Peer_Renamed;
					if (peer != null)
					{
						peer.Select(pos);
					}
				}
			}
		}

		/// <summary>
		/// Sets the selected item in this <code>Choice</code> menu
		/// to be the item whose name is equal to the specified string.
		/// If more than one item matches (is equal to) the specified string,
		/// the one with the smallest index is selected.
		/// 
		/// <para>Note that this method should be primarily used to
		/// initially select an item in this component.
		/// Programmatically calling this method will <i>not</i> trigger
		/// an <code>ItemEvent</code>.  The only way to trigger an
		/// <code>ItemEvent</code> is by user interaction.
		/// 
		/// </para>
		/// </summary>
		/// <param name="str">     the specified string </param>
		/// <seealso cref=         #getSelectedItem </seealso>
		/// <seealso cref=         #getSelectedIndex </seealso>
		public virtual void Select(String str)
		{
			lock (this)
			{
				int index = PItems.IndexOf(str);
				if (index >= 0)
				{
					Select(index);
				}
			}
		}

		/// <summary>
		/// Adds the specified item listener to receive item events from
		/// this <code>Choice</code> menu.  Item events are sent in response
		/// to user input, but not in response to calls to <code>select</code>.
		/// If l is <code>null</code>, no exception is thrown and no action
		/// is performed.
		/// <para>Refer to <a href="doc-files/AWTThreadIssues.html#ListenersThreads"
		/// >AWT Threading Issues</a> for details on AWT's threading model.
		/// </para>
		/// </summary>
		/// <param name="l">    the item listener </param>
		/// <seealso cref=           #removeItemListener </seealso>
		/// <seealso cref=           #getItemListeners </seealso>
		/// <seealso cref=           #select </seealso>
		/// <seealso cref=           java.awt.event.ItemEvent </seealso>
		/// <seealso cref=           java.awt.event.ItemListener
		/// @since         JDK1.1 </seealso>
		public virtual void AddItemListener(ItemListener l)
		{
			lock (this)
			{
				if (l == null)
				{
				   return;
				}
				ItemListener = AWTEventMulticaster.Add(ItemListener, l);
				NewEventsOnly = true;
			}
		}

		/// <summary>
		/// Removes the specified item listener so that it no longer receives
		/// item events from this <code>Choice</code> menu.
		/// If l is <code>null</code>, no exception is thrown and no
		/// action is performed.
		/// <para>Refer to <a href="doc-files/AWTThreadIssues.html#ListenersThreads"
		/// >AWT Threading Issues</a> for details on AWT's threading model.
		/// </para>
		/// </summary>
		/// <param name="l">    the item listener </param>
		/// <seealso cref=           #addItemListener </seealso>
		/// <seealso cref=           #getItemListeners </seealso>
		/// <seealso cref=           java.awt.event.ItemEvent </seealso>
		/// <seealso cref=           java.awt.event.ItemListener
		/// @since         JDK1.1 </seealso>
		public virtual void RemoveItemListener(ItemListener l)
		{
			lock (this)
			{
				if (l == null)
				{
					return;
				}
				ItemListener = AWTEventMulticaster.Remove(ItemListener, l);
			}
		}

		/// <summary>
		/// Returns an array of all the item listeners
		/// registered on this choice.
		/// </summary>
		/// <returns> all of this choice's <code>ItemListener</code>s
		///         or an empty array if no item
		///         listeners are currently registered
		/// </returns>
		/// <seealso cref=           #addItemListener </seealso>
		/// <seealso cref=           #removeItemListener </seealso>
		/// <seealso cref=           java.awt.event.ItemEvent </seealso>
		/// <seealso cref=           java.awt.event.ItemListener
		/// @since 1.4 </seealso>
		public virtual ItemListener[] ItemListeners
		{
			get
			{
				lock (this)
				{
					return GetListeners(typeof(ItemListener));
				}
			}
		}

		/// <summary>
		/// Returns an array of all the objects currently registered
		/// as <code><em>Foo</em>Listener</code>s
		/// upon this <code>Choice</code>.
		/// <code><em>Foo</em>Listener</code>s are registered using the
		/// <code>add<em>Foo</em>Listener</code> method.
		/// 
		/// <para>
		/// You can specify the <code>listenerType</code> argument
		/// with a class literal, such as
		/// <code><em>Foo</em>Listener.class</code>.
		/// For example, you can query a
		/// <code>Choice</code> <code>c</code>
		/// for its item listeners with the following code:
		/// 
		/// <pre>ItemListener[] ils = (ItemListener[])(c.getListeners(ItemListener.class));</pre>
		/// 
		/// If no such listeners exist, this method returns an empty array.
		/// 
		/// </para>
		/// </summary>
		/// <param name="listenerType"> the type of listeners requested; this parameter
		///          should specify an interface that descends from
		///          <code>java.util.EventListener</code> </param>
		/// <returns> an array of all objects registered as
		///          <code><em>Foo</em>Listener</code>s on this choice,
		///          or an empty array if no such
		///          listeners have been added </returns>
		/// <exception cref="ClassCastException"> if <code>listenerType</code>
		///          doesn't specify a class or interface that implements
		///          <code>java.util.EventListener</code>
		/// </exception>
		/// <seealso cref= #getItemListeners
		/// @since 1.3 </seealso>
		public override T[] getListeners<T>(Class listenerType) where T : java.util.EventListener
		{
			EventListener l = null;
			if (listenerType == typeof(ItemListener))
			{
				l = ItemListener;
			}
			else
			{
				return base.GetListeners(listenerType);
			}
			return AWTEventMulticaster.GetListeners(l, listenerType);
		}

		// REMIND: remove when filtering is done at lower level
		internal override bool EventEnabled(AWTEvent e)
		{
			if (e.Id == ItemEvent.ITEM_STATE_CHANGED)
			{
				if ((EventMask & AWTEvent.ITEM_EVENT_MASK) != 0 || ItemListener != null)
				{
					return true;
				}
				return false;
			}
			return base.EventEnabled(e);
		}

		/// <summary>
		/// Processes events on this choice. If the event is an
		/// instance of <code>ItemEvent</code>, it invokes the
		/// <code>processItemEvent</code> method. Otherwise, it calls its
		/// superclass's <code>processEvent</code> method.
		/// <para>Note that if the event parameter is <code>null</code>
		/// the behavior is unspecified and may result in an
		/// exception.
		/// 
		/// </para>
		/// </summary>
		/// <param name="e"> the event </param>
		/// <seealso cref=        java.awt.event.ItemEvent </seealso>
		/// <seealso cref=        #processItemEvent
		/// @since      JDK1.1 </seealso>
		protected internal override void ProcessEvent(AWTEvent e)
		{
			if (e is ItemEvent)
			{
				ProcessItemEvent((ItemEvent)e);
				return;
			}
			base.ProcessEvent(e);
		}

		/// <summary>
		/// Processes item events occurring on this <code>Choice</code>
		/// menu by dispatching them to any registered
		/// <code>ItemListener</code> objects.
		/// <para>
		/// This method is not called unless item events are
		/// enabled for this component. Item events are enabled
		/// when one of the following occurs:
		/// <ul>
		/// <li>An <code>ItemListener</code> object is registered
		/// via <code>addItemListener</code>.
		/// <li>Item events are enabled via <code>enableEvents</code>.
		/// </ul>
		/// </para>
		/// <para>Note that if the event parameter is <code>null</code>
		/// the behavior is unspecified and may result in an
		/// exception.
		/// 
		/// </para>
		/// </summary>
		/// <param name="e"> the item event </param>
		/// <seealso cref=         java.awt.event.ItemEvent </seealso>
		/// <seealso cref=         java.awt.event.ItemListener </seealso>
		/// <seealso cref=         #addItemListener(ItemListener) </seealso>
		/// <seealso cref=         java.awt.Component#enableEvents
		/// @since       JDK1.1 </seealso>
		protected internal virtual void ProcessItemEvent(ItemEvent e)
		{
			ItemListener listener = ItemListener;
			if (listener != null)
			{
				listener.ItemStateChanged(e);
			}
		}

		/// <summary>
		/// Returns a string representing the state of this <code>Choice</code>
		/// menu. This method is intended to be used only for debugging purposes,
		/// and the content and format of the returned string may vary between
		/// implementations. The returned string may be empty but may not be
		/// <code>null</code>.
		/// </summary>
		/// <returns>    the parameter string of this <code>Choice</code> menu </returns>
		protected internal override String ParamString()
		{
			return base.ParamString() + ",current=" + SelectedItem;
		}


		/* Serialization support.
		 */

		/*
		 * Choice Serial Data Version.
		 * @serial
		 */
		private int ChoiceSerializedDataVersion = 1;

		/// <summary>
		/// Writes default serializable fields to stream.  Writes
		/// a list of serializable <code>ItemListeners</code>
		/// as optional data. The non-serializable
		/// <code>ItemListeners</code> are detected and
		/// no attempt is made to serialize them.
		/// </summary>
		/// <param name="s"> the <code>ObjectOutputStream</code> to write
		/// @serialData <code>null</code> terminated sequence of 0
		///   or more pairs; the pair consists of a <code>String</code>
		///   and an <code>Object</code>; the <code>String</code> indicates
		///   the type of object and is one of the following:
		///   <code>itemListenerK</code> indicating an
		///     <code>ItemListener</code> object
		/// </param>
		/// <seealso cref= AWTEventMulticaster#save(ObjectOutputStream, String, EventListener) </seealso>
		/// <seealso cref= java.awt.Component#itemListenerK </seealso>
		/// <seealso cref= #readObject(ObjectInputStream) </seealso>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: private void writeObject(java.io.ObjectOutputStream s) throws java.io.IOException
		private void WriteObject(ObjectOutputStream s)
		{
		  s.DefaultWriteObject();

		  AWTEventMulticaster.Save(s, ItemListenerK, ItemListener);
		  s.WriteObject(null);
		}

		/// <summary>
		/// Reads the <code>ObjectInputStream</code> and if it
		/// isn't <code>null</code> adds a listener to receive
		/// item events fired by the <code>Choice</code> item.
		/// Unrecognized keys or values will be ignored.
		/// </summary>
		/// <param name="s"> the <code>ObjectInputStream</code> to read </param>
		/// <exception cref="HeadlessException"> if
		///   <code>GraphicsEnvironment.isHeadless</code> returns
		///   <code>true</code>
		/// @serial </exception>
		/// <seealso cref= #removeItemListener(ItemListener) </seealso>
		/// <seealso cref= #addItemListener(ItemListener) </seealso>
		/// <seealso cref= java.awt.GraphicsEnvironment#isHeadless </seealso>
		/// <seealso cref= #writeObject(ObjectOutputStream) </seealso>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: private void readObject(java.io.ObjectInputStream s) throws ClassNotFoundException, java.io.IOException, HeadlessException
		private void ReadObject(ObjectInputStream s)
		{
		  GraphicsEnvironment.CheckHeadless();
		  s.DefaultReadObject();

		  Object keyOrNull;
		  while (null != (keyOrNull = s.ReadObject()))
		  {
			String key = ((String)keyOrNull).intern();

			if (ItemListenerK == key)
			{
			  AddItemListener((ItemListener)(s.ReadObject()));
			}

			else // skip value for unrecognized key
			{
			  s.ReadObject();
			}
		  }
		}

		/// <summary>
		/// Initialize JNI field and method IDs
		/// </summary>
//JAVA TO C# CONVERTER TODO TASK: Replace 'unknown' with the appropriate dll name:
		[DllImport("unknown")]
		private static extern void initIDs();

	/////////////////
	// Accessibility support
	////////////////


		/// <summary>
		/// Gets the <code>AccessibleContext</code> associated with this
		/// <code>Choice</code>. For <code>Choice</code> components,
		/// the <code>AccessibleContext</code> takes the form of an
		/// <code>AccessibleAWTChoice</code>. A new <code>AccessibleAWTChoice</code>
		/// instance is created if necessary.
		/// </summary>
		/// <returns> an <code>AccessibleAWTChoice</code> that serves as the
		///         <code>AccessibleContext</code> of this <code>Choice</code>
		/// @since 1.3 </returns>
		public override AccessibleContext AccessibleContext
		{
			get
			{
				if (AccessibleContext_Renamed == null)
				{
					AccessibleContext_Renamed = new AccessibleAWTChoice(this);
				}
				return AccessibleContext_Renamed;
			}
		}

		/// <summary>
		/// This class implements accessibility support for the
		/// <code>Choice</code> class.  It provides an implementation of the
		/// Java Accessibility API appropriate to choice user-interface elements.
		/// @since 1.3
		/// </summary>
		protected internal class AccessibleAWTChoice : AccessibleAWTComponent, AccessibleAction
		{
			private readonly Choice OuterInstance;

			/*
			 * JDK 1.3 serialVersionUID
			 */
			internal const long SerialVersionUID = 7175603582428509322L;

			public AccessibleAWTChoice(Choice outerInstance) : base(outerInstance)
			{
				this.OuterInstance = outerInstance;
			}

			/// <summary>
			/// Get the AccessibleAction associated with this object.  In the
			/// implementation of the Java Accessibility API for this class,
			/// return this object, which is responsible for implementing the
			/// AccessibleAction interface on behalf of itself.
			/// </summary>
			/// <returns> this object </returns>
			/// <seealso cref= AccessibleAction </seealso>
			public virtual AccessibleAction AccessibleAction
			{
				get
				{
					return this;
				}
			}

			/// <summary>
			/// Get the role of this object.
			/// </summary>
			/// <returns> an instance of AccessibleRole describing the role of the
			/// object </returns>
			/// <seealso cref= AccessibleRole </seealso>
			public override AccessibleRole AccessibleRole
			{
				get
				{
					return AccessibleRole.COMBO_BOX;
				}
			}

			/// <summary>
			/// Returns the number of accessible actions available in this object
			/// If there are more than one, the first one is considered the "default"
			/// action of the object.
			/// </summary>
			/// <returns> the zero-based number of Actions in this object </returns>
			public virtual int AccessibleActionCount
			{
				get
				{
					return 0; //  To be fully implemented in a future release
				}
			}

			/// <summary>
			/// Returns a description of the specified action of the object.
			/// </summary>
			/// <param name="i"> zero-based index of the actions </param>
			/// <returns> a String description of the action </returns>
			/// <seealso cref= #getAccessibleActionCount </seealso>
			public virtual String GetAccessibleActionDescription(int i)
			{
				return null; //  To be fully implemented in a future release
			}

			/// <summary>
			/// Perform the specified Action on the object
			/// </summary>
			/// <param name="i"> zero-based index of actions </param>
			/// <returns> true if the action was performed; otherwise false. </returns>
			/// <seealso cref= #getAccessibleActionCount </seealso>
			public virtual bool DoAccessibleAction(int i)
			{
				return false; //  To be fully implemented in a future release
			}

		} // inner class AccessibleAWTChoice

	}

}
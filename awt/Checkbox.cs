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
	/// A check box is a graphical component that can be in either an
	/// "on" (<code>true</code>) or "off" (<code>false</code>) state.
	/// Clicking on a check box changes its state from
	/// "on" to "off," or from "off" to "on."
	/// <para>
	/// The following code example creates a set of check boxes in
	/// a grid layout:
	/// 
	/// <hr><blockquote><pre>
	/// setLayout(new GridLayout(3, 1));
	/// add(new Checkbox("one", null, true));
	/// add(new Checkbox("two"));
	/// add(new Checkbox("three"));
	/// </pre></blockquote><hr>
	/// </para>
	/// <para>
	/// This image depicts the check boxes and grid layout
	/// created by this code example:
	/// </para>
	/// <para>
	/// <img src="doc-files/Checkbox-1.gif" alt="The following context describes the graphic."
	/// style="float:center; margin: 7px 10px;">
	/// </para>
	/// <para>
	/// The button labeled <code>one</code> is in the "on" state, and the
	/// other two are in the "off" state. In this example, which uses the
	/// <code>GridLayout</code> class, the states of the three check
	/// boxes are set independently.
	/// </para>
	/// <para>
	/// Alternatively, several check boxes can be grouped together under
	/// the control of a single object, using the
	/// <code>CheckboxGroup</code> class.
	/// In a check box group, at most one button can be in the "on"
	/// state at any given time. Clicking on a check box to turn it on
	/// forces any other check box in the same group that is on
	/// into the "off" state.
	/// 
	/// @author      Sami Shaio
	/// </para>
	/// </summary>
	/// <seealso cref=         java.awt.GridLayout </seealso>
	/// <seealso cref=         java.awt.CheckboxGroup
	/// @since       JDK1.0 </seealso>
	public class Checkbox : Component, ItemSelectable, Accessible
	{

		static Checkbox()
		{
			/* ensure that the necessary native libraries are loaded */
			Toolkit.LoadLibraries();
			if (!GraphicsEnvironment.Headless)
			{
				initIDs();
			}
		}

		/// <summary>
		/// The label of the Checkbox.
		/// This field can be null.
		/// @serial </summary>
		/// <seealso cref= #getLabel() </seealso>
		/// <seealso cref= #setLabel(String) </seealso>
		internal String Label_Renamed;

		/// <summary>
		/// The state of the <code>Checkbox</code>.
		/// @serial </summary>
		/// <seealso cref= #getState() </seealso>
		/// <seealso cref= #setState(boolean) </seealso>
		internal bool State_Renamed;

		/// <summary>
		/// The check box group.
		/// This field can be null indicating that the checkbox
		/// is not a group checkbox.
		/// @serial </summary>
		/// <seealso cref= #getCheckboxGroup() </seealso>
		/// <seealso cref= #setCheckboxGroup(CheckboxGroup) </seealso>
		internal CheckboxGroup Group;

		[NonSerialized]
		internal ItemListener ItemListener;

		private const String @base = "checkbox";
		private static int NameCounter = 0;

		/*
		 * JDK 1.1 serialVersionUID
		 */
		private const long SerialVersionUID = 7270714317450821763L;

		/// <summary>
		/// Helper function for setState and CheckboxGroup.setSelectedCheckbox
		/// Should remain package-private.
		/// </summary>
		internal virtual bool StateInternal
		{
			set
			{
				this.State_Renamed = value;
				CheckboxPeer peer = (CheckboxPeer)this.Peer_Renamed;
				if (peer != null)
				{
					peer.State = value;
				}
			}
		}

		/// <summary>
		/// Creates a check box with an empty string for its label.
		/// The state of this check box is set to "off," and it is not
		/// part of any check box group. </summary>
		/// <exception cref="HeadlessException"> if GraphicsEnvironment.isHeadless()
		/// returns true </exception>
		/// <seealso cref= java.awt.GraphicsEnvironment#isHeadless </seealso>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public Checkbox() throws HeadlessException
		public Checkbox() : this("", false, null)
		{
		}

		/// <summary>
		/// Creates a check box with the specified label.  The state
		/// of this check box is set to "off," and it is not part of
		/// any check box group.
		/// </summary>
		/// <param name="label">   a string label for this check box,
		///                        or <code>null</code> for no label. </param>
		/// <exception cref="HeadlessException"> if
		///      <code>GraphicsEnvironment.isHeadless</code>
		///      returns <code>true</code> </exception>
		/// <seealso cref= java.awt.GraphicsEnvironment#isHeadless </seealso>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public Checkbox(String label) throws HeadlessException
		public Checkbox(String label) : this(label, false, null)
		{
		}

		/// <summary>
		/// Creates a check box with the specified label
		/// and sets the specified state.
		/// This check box is not part of any check box group.
		/// </summary>
		/// <param name="label">   a string label for this check box,
		///                        or <code>null</code> for no label </param>
		/// <param name="state">    the initial state of this check box </param>
		/// <exception cref="HeadlessException"> if
		///     <code>GraphicsEnvironment.isHeadless</code>
		///     returns <code>true</code> </exception>
		/// <seealso cref= java.awt.GraphicsEnvironment#isHeadless </seealso>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public Checkbox(String label, boolean state) throws HeadlessException
		public Checkbox(String label, bool state) : this(label, state, null)
		{
		}

		/// <summary>
		/// Constructs a Checkbox with the specified label, set to the
		/// specified state, and in the specified check box group.
		/// </summary>
		/// <param name="label">   a string label for this check box,
		///                        or <code>null</code> for no label. </param>
		/// <param name="state">   the initial state of this check box. </param>
		/// <param name="group">   a check box group for this check box,
		///                           or <code>null</code> for no group. </param>
		/// <exception cref="HeadlessException"> if
		///     <code>GraphicsEnvironment.isHeadless</code>
		///     returns <code>true</code> </exception>
		/// <seealso cref= java.awt.GraphicsEnvironment#isHeadless
		/// @since     JDK1.1 </seealso>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public Checkbox(String label, boolean state, CheckboxGroup group) throws HeadlessException
		public Checkbox(String label, bool state, CheckboxGroup group)
		{
			GraphicsEnvironment.CheckHeadless();
			this.Label_Renamed = label;
			this.State_Renamed = state;
			this.Group = group;
			if (state && (group != null))
			{
				group.SelectedCheckbox = this;
			}
		}

		/// <summary>
		/// Creates a check box with the specified label, in the specified
		/// check box group, and set to the specified state.
		/// </summary>
		/// <param name="label">   a string label for this check box,
		///                        or <code>null</code> for no label. </param>
		/// <param name="group">   a check box group for this check box,
		///                           or <code>null</code> for no group. </param>
		/// <param name="state">   the initial state of this check box. </param>
		/// <exception cref="HeadlessException"> if
		///    <code>GraphicsEnvironment.isHeadless</code>
		///    returns <code>true</code> </exception>
		/// <seealso cref= java.awt.GraphicsEnvironment#isHeadless
		/// @since     JDK1.1 </seealso>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public Checkbox(String label, CheckboxGroup group, boolean state) throws HeadlessException
		public Checkbox(String label, CheckboxGroup group, bool state) : this(label, state, group)
		{
		}

		/// <summary>
		/// Constructs a name for this component.  Called by
		/// <code>getName</code> when the name is <code>null</code>.
		/// </summary>
		/// <returns> a name for this component </returns>
		internal override String ConstructComponentName()
		{
			lock (typeof(Checkbox))
			{
				return @base + NameCounter++;
			}
		}

		/// <summary>
		/// Creates the peer of the Checkbox. The peer allows you to change the
		/// look of the Checkbox without changing its functionality.
		/// </summary>
		/// <seealso cref=     java.awt.Toolkit#createCheckbox(java.awt.Checkbox) </seealso>
		/// <seealso cref=     java.awt.Component#getToolkit() </seealso>
		public override void AddNotify()
		{
			lock (TreeLock)
			{
				if (Peer_Renamed == null)
				{
					Peer_Renamed = Toolkit.CreateCheckbox(this);
				}
				base.AddNotify();
			}
		}

		/// <summary>
		/// Gets the label of this check box.
		/// </summary>
		/// <returns>   the label of this check box, or <code>null</code>
		///                  if this check box has no label. </returns>
		/// <seealso cref=      #setLabel(String) </seealso>
		public virtual String Label
		{
			get
			{
				return Label_Renamed;
			}
			set
			{
				bool testvalid = false;
    
				lock (this)
				{
					if (value != this.Label_Renamed && (this.Label_Renamed == null || !this.Label_Renamed.Equals(value)))
					{
						this.Label_Renamed = value;
						CheckboxPeer peer = (CheckboxPeer)this.Peer_Renamed;
						if (peer != null)
						{
							peer.Label = value;
						}
						testvalid = true;
					}
				}
    
				// This could change the preferred size of the Component.
				if (testvalid)
				{
					InvalidateIfValid();
				}
			}
		}


		/// <summary>
		/// Determines whether this check box is in the "on" or "off" state.
		/// The boolean value <code>true</code> indicates the "on" state,
		/// and <code>false</code> indicates the "off" state.
		/// </summary>
		/// <returns>    the state of this check box, as a boolean value </returns>
		/// <seealso cref=       #setState </seealso>
		public virtual bool State
		{
			get
			{
				return State_Renamed;
			}
			set
			{
				/* Cannot hold check box lock when calling group.setSelectedCheckbox. */
				CheckboxGroup group = this.Group;
				if (group != null)
				{
					if (value)
					{
						group.SelectedCheckbox = this;
					}
					else if (group.SelectedCheckbox == this)
					{
						value = true;
					}
				}
				StateInternal = value;
			}
		}


		/// <summary>
		/// Returns an array (length 1) containing the checkbox
		/// label or null if the checkbox is not selected. </summary>
		/// <seealso cref= ItemSelectable </seealso>
		public virtual Object[] SelectedObjects
		{
			get
			{
				if (State_Renamed)
				{
					Object[] items = new Object[1];
					items[0] = Label_Renamed;
					return items;
				}
				return null;
			}
		}

		/// <summary>
		/// Determines this check box's group. </summary>
		/// <returns>     this check box's group, or <code>null</code>
		///               if the check box is not part of a check box group. </returns>
		/// <seealso cref=        #setCheckboxGroup(CheckboxGroup) </seealso>
		public virtual CheckboxGroup CheckboxGroup
		{
			get
			{
				return Group;
			}
			set
			{
				CheckboxGroup oldGroup;
				bool oldState;
    
				/* Do nothing if this check box has already belonged
				 * to the check box group value.
				 */
				if (this.Group == value)
				{
					return;
				}
    
				lock (this)
				{
					oldGroup = this.Group;
					oldState = State;
    
					this.Group = value;
					CheckboxPeer peer = (CheckboxPeer)this.Peer_Renamed;
					if (peer != null)
					{
						peer.CheckboxGroup = value;
					}
					if (this.Group != null && State)
					{
						if (this.Group.SelectedCheckbox != null)
						{
							State = false;
						}
						else
						{
							this.Group.SelectedCheckbox = this;
						}
					}
				}
    
				/* Locking check box below could cause deadlock with
				 * CheckboxGroup's setSelectedCheckbox method.
				 *
				 * Fix for 4726853 by kdm@sparc.spb.su
				 * Here we should check if this check box was selected
				 * in the previous group and set selected check box to
				 * null for that group if so.
				 */
				if (oldGroup != null && oldState)
				{
					oldGroup.SelectedCheckbox = null;
				}
			}
		}


		/// <summary>
		/// Adds the specified item listener to receive item events from
		/// this check box.  Item events are sent to listeners in response
		/// to user input, but not in response to calls to setState().
		/// If l is null, no exception is thrown and no action is performed.
		/// <para>Refer to <a href="doc-files/AWTThreadIssues.html#ListenersThreads"
		/// >AWT Threading Issues</a> for details on AWT's threading model.
		/// 
		/// </para>
		/// </summary>
		/// <param name="l">    the item listener </param>
		/// <seealso cref=           #removeItemListener </seealso>
		/// <seealso cref=           #getItemListeners </seealso>
		/// <seealso cref=           #setState </seealso>
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
		/// Removes the specified item listener so that the item listener
		/// no longer receives item events from this check box.
		/// If l is null, no exception is thrown and no action is performed.
		/// <para>Refer to <a href="doc-files/AWTThreadIssues.html#ListenersThreads"
		/// >AWT Threading Issues</a> for details on AWT's threading model.
		/// 
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
		/// registered on this checkbox.
		/// </summary>
		/// <returns> all of this checkbox's <code>ItemListener</code>s
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
		/// upon this <code>Checkbox</code>.
		/// <code><em>Foo</em>Listener</code>s are registered using the
		/// <code>add<em>Foo</em>Listener</code> method.
		/// 
		/// <para>
		/// You can specify the <code>listenerType</code> argument
		/// with a class literal, such as
		/// <code><em>Foo</em>Listener.class</code>.
		/// For example, you can query a
		/// <code>Checkbox</code> <code>c</code>
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
		///          <code><em>Foo</em>Listener</code>s on this checkbox,
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
		/// Processes events on this check box.
		/// If the event is an instance of <code>ItemEvent</code>,
		/// this method invokes the <code>processItemEvent</code> method.
		/// Otherwise, it calls its superclass's <code>processEvent</code> method.
		/// <para>Note that if the event parameter is <code>null</code>
		/// the behavior is unspecified and may result in an
		/// exception.
		/// 
		/// </para>
		/// </summary>
		/// <param name="e"> the event </param>
		/// <seealso cref=           java.awt.event.ItemEvent </seealso>
		/// <seealso cref=           #processItemEvent
		/// @since         JDK1.1 </seealso>
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
		/// Processes item events occurring on this check box by
		/// dispatching them to any registered
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
		/// <seealso cref=         #addItemListener </seealso>
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
		/// Returns a string representing the state of this <code>Checkbox</code>.
		/// This method is intended to be used only for debugging purposes, and the
		/// content and format of the returned string may vary between
		/// implementations. The returned string may be empty but may not be
		/// <code>null</code>.
		/// </summary>
		/// <returns>    the parameter string of this check box </returns>
		protected internal override String ParamString()
		{
			String str = base.ParamString();
			String label = this.Label_Renamed;
			if (label != null)
			{
				str += ",label=" + label;
			}
			return str + ",state=" + State_Renamed;
		}


		/* Serialization support.
		 */

		/*
		 * Serialized data version
		 * @serial
		 */
		private int CheckboxSerializedDataVersion = 1;

		/// <summary>
		/// Writes default serializable fields to stream.  Writes
		/// a list of serializable <code>ItemListeners</code>
		/// as optional data.  The non-serializable
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
		/// item events fired by the <code>Checkbox</code>.
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
		/// Initialize JNI field and method ids
		/// </summary>
//JAVA TO C# CONVERTER TODO TASK: Replace 'unknown' with the appropriate dll name:
		[DllImport("unknown")]
		private static extern void initIDs();


	/////////////////
	// Accessibility support
	////////////////


		/// <summary>
		/// Gets the AccessibleContext associated with this Checkbox.
		/// For checkboxes, the AccessibleContext takes the form of an
		/// AccessibleAWTCheckbox.
		/// A new AccessibleAWTCheckbox is created if necessary.
		/// </summary>
		/// <returns> an AccessibleAWTCheckbox that serves as the
		///         AccessibleContext of this Checkbox
		/// @since 1.3 </returns>
		public override AccessibleContext AccessibleContext
		{
			get
			{
				if (AccessibleContext_Renamed == null)
				{
					AccessibleContext_Renamed = new AccessibleAWTCheckbox(this);
				}
				return AccessibleContext_Renamed;
			}
		}

		/// <summary>
		/// This class implements accessibility support for the
		/// <code>Checkbox</code> class.  It provides an implementation of the
		/// Java Accessibility API appropriate to checkbox user-interface elements.
		/// @since 1.3
		/// </summary>
		protected internal class AccessibleAWTCheckbox : AccessibleAWTComponent, ItemListener, AccessibleAction, AccessibleValue
		{
			private readonly Checkbox OuterInstance;

			/*
			 * JDK 1.3 serialVersionUID
			 */
			internal const long SerialVersionUID = 7881579233144754107L;

			public AccessibleAWTCheckbox(Checkbox outerInstance) : base(outerInstance)
			{
				this.OuterInstance = outerInstance;
				outerInstance.AddItemListener(this);
			}

			/// <summary>
			/// Fire accessible property change events when the state of the
			/// toggle button changes.
			/// </summary>
			public virtual void ItemStateChanged(ItemEvent e)
			{
				Checkbox cb = (Checkbox) e.Source;
				if (OuterInstance.AccessibleContext_Renamed != null)
				{
					if (cb.State)
					{
						OuterInstance.AccessibleContext_Renamed.firePropertyChange(AccessibleContext.ACCESSIBLE_STATE_PROPERTY, null, AccessibleState.CHECKED);
					}
					else
					{
						OuterInstance.AccessibleContext_Renamed.firePropertyChange(AccessibleContext.ACCESSIBLE_STATE_PROPERTY, AccessibleState.CHECKED, null);
					}
				}
			}

			/// <summary>
			/// Get the AccessibleAction associated with this object.  In the
			/// implementation of the Java Accessibility API for this class,
			/// return this object, which is responsible for implementing the
			/// AccessibleAction interface on behalf of itself.
			/// </summary>
			/// <returns> this object </returns>
			public virtual AccessibleAction AccessibleAction
			{
				get
				{
					return this;
				}
			}

			/// <summary>
			/// Get the AccessibleValue associated with this object.  In the
			/// implementation of the Java Accessibility API for this class,
			/// return this object, which is responsible for implementing the
			/// AccessibleValue interface on behalf of itself.
			/// </summary>
			/// <returns> this object </returns>
			public virtual AccessibleValue AccessibleValue
			{
				get
				{
					return this;
				}
			}

			/// <summary>
			/// Returns the number of Actions available in this object.
			/// If there is more than one, the first one is the "default"
			/// action.
			/// </summary>
			/// <returns> the number of Actions in this object </returns>
			public virtual int AccessibleActionCount
			{
				get
				{
					return 0; //  To be fully implemented in a future release
				}
			}

			/// <summary>
			/// Return a description of the specified action of the object.
			/// </summary>
			/// <param name="i"> zero-based index of the actions </param>
			public virtual String GetAccessibleActionDescription(int i)
			{
				return null; //  To be fully implemented in a future release
			}

			/// <summary>
			/// Perform the specified Action on the object
			/// </summary>
			/// <param name="i"> zero-based index of actions </param>
			/// <returns> true if the the action was performed; else false. </returns>
			public virtual bool DoAccessibleAction(int i)
			{
				return false; //  To be fully implemented in a future release
			}

			/// <summary>
			/// Get the value of this object as a Number.  If the value has not been
			/// set, the return value will be null.
			/// </summary>
			/// <returns> value of the object </returns>
			/// <seealso cref= #setCurrentAccessibleValue </seealso>
			public virtual Number CurrentAccessibleValue
			{
				get
				{
					return null; //  To be fully implemented in a future release
				}
			}

			/// <summary>
			/// Set the value of this object as a Number.
			/// </summary>
			/// <returns> True if the value was set; else False </returns>
			/// <seealso cref= #getCurrentAccessibleValue </seealso>
			public virtual bool SetCurrentAccessibleValue(Number n)
			{
				return false; //  To be fully implemented in a future release
			}

			/// <summary>
			/// Get the minimum value of this object as a Number.
			/// </summary>
			/// <returns> Minimum value of the object; null if this object does not
			/// have a minimum value </returns>
			/// <seealso cref= #getMaximumAccessibleValue </seealso>
			public virtual Number MinimumAccessibleValue
			{
				get
				{
					return null; //  To be fully implemented in a future release
				}
			}

			/// <summary>
			/// Get the maximum value of this object as a Number.
			/// </summary>
			/// <returns> Maximum value of the object; null if this object does not
			/// have a maximum value </returns>
			/// <seealso cref= #getMinimumAccessibleValue </seealso>
			public virtual Number MaximumAccessibleValue
			{
				get
				{
					return null; //  To be fully implemented in a future release
				}
			}

			/// <summary>
			/// Get the role of this object.
			/// </summary>
			/// <returns> an instance of AccessibleRole describing the role of
			/// the object </returns>
			/// <seealso cref= AccessibleRole </seealso>
			public override AccessibleRole AccessibleRole
			{
				get
				{
					return AccessibleRole.CHECK_BOX;
				}
			}

			/// <summary>
			/// Get the state set of this object.
			/// </summary>
			/// <returns> an instance of AccessibleState containing the current state
			/// of the object </returns>
			/// <seealso cref= AccessibleState </seealso>
			public override AccessibleStateSet AccessibleStateSet
			{
				get
				{
					AccessibleStateSet states = base.AccessibleStateSet;
					if (outerInstance.State)
					{
						states.add(AccessibleState.CHECKED);
					}
					return states;
				}
			}


		} // inner class AccessibleAWTCheckbox

	}

}
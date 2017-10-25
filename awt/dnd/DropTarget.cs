using System;

/*
 * Copyright (c) 1997, 2013, Oracle and/or its affiliates. All rights reserved.
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

namespace java.awt.dnd
{




	/// <summary>
	/// The <code>DropTarget</code> is associated
	/// with a <code>Component</code> when that <code>Component</code>
	/// wishes
	/// to accept drops during Drag and Drop operations.
	/// <P>
	///  Each
	/// <code>DropTarget</code> is associated with a <code>FlavorMap</code>.
	/// The default <code>FlavorMap</code> hereafter designates the
	/// <code>FlavorMap</code> returned by <code>SystemFlavorMap.getDefaultFlavorMap()</code>.
	/// 
	/// @since 1.2
	/// </summary>

	[Serializable]
	public class DropTarget : DropTargetListener
	{
		private bool InstanceFieldsInitialized = false;

		private void InitializeInstanceFields()
		{
			DropTargetContext_Renamed = CreateDropTargetContext();
		}


		private const long SerialVersionUID = -6283860791671019047L;

		/// <summary>
		/// Creates a new DropTarget given the <code>Component</code>
		/// to associate itself with, an <code>int</code> representing
		/// the default acceptable action(s) to
		/// support, a <code>DropTargetListener</code>
		/// to handle event processing, a <code>boolean</code> indicating
		/// if the <code>DropTarget</code> is currently accepting drops, and
		/// a <code>FlavorMap</code> to use (or null for the default <CODE>FlavorMap</CODE>).
		/// <P>
		/// The Component will receive drops only if it is enabled. </summary>
		/// <param name="c">         The <code>Component</code> with which this <code>DropTarget</code> is associated </param>
		/// <param name="ops">       The default acceptable actions for this <code>DropTarget</code> </param>
		/// <param name="dtl">       The <code>DropTargetListener</code> for this <code>DropTarget</code> </param>
		/// <param name="act">       Is the <code>DropTarget</code> accepting drops. </param>
		/// <param name="fm">        The <code>FlavorMap</code> to use, or null for the default <CODE>FlavorMap</CODE> </param>
		/// <exception cref="HeadlessException"> if GraphicsEnvironment.isHeadless()
		///            returns true </exception>
		/// <seealso cref= java.awt.GraphicsEnvironment#isHeadless </seealso>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public DropTarget(java.awt.Component c, int ops, DropTargetListener dtl, boolean act, java.awt.datatransfer.FlavorMap fm) throws java.awt.HeadlessException
		public DropTarget(Component c, int ops, DropTargetListener dtl, bool act, FlavorMap fm)
		{
			if (!InstanceFieldsInitialized)
			{
				InitializeInstanceFields();
				InstanceFieldsInitialized = true;
			}
			if (GraphicsEnvironment.Headless)
			{
				throw new HeadlessException();
			}

			Component_Renamed = c;

			DefaultActions = ops;

			if (dtl != null)
			{
				try
				{
				AddDropTargetListener(dtl);
				}
			catch (TooManyListenersException)
			{
				// do nothing!
			}
			}

			if (c != null)
			{
				c.DropTarget = this;
				Active = act;
			}

			if (fm != null)
			{
				FlavorMap_Renamed = fm;
			}
			else
			{
				FlavorMap_Renamed = SystemFlavorMap.DefaultFlavorMap;
			}
		}

		/// <summary>
		/// Creates a <code>DropTarget</code> given the <code>Component</code>
		/// to associate itself with, an <code>int</code> representing
		/// the default acceptable action(s)
		/// to support, a <code>DropTargetListener</code>
		/// to handle event processing, and a <code>boolean</code> indicating
		/// if the <code>DropTarget</code> is currently accepting drops.
		/// <P>
		/// The Component will receive drops only if it is enabled. </summary>
		/// <param name="c">         The <code>Component</code> with which this <code>DropTarget</code> is associated </param>
		/// <param name="ops">       The default acceptable actions for this <code>DropTarget</code> </param>
		/// <param name="dtl">       The <code>DropTargetListener</code> for this <code>DropTarget</code> </param>
		/// <param name="act">       Is the <code>DropTarget</code> accepting drops. </param>
		/// <exception cref="HeadlessException"> if GraphicsEnvironment.isHeadless()
		///            returns true </exception>
		/// <seealso cref= java.awt.GraphicsEnvironment#isHeadless </seealso>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public DropTarget(java.awt.Component c, int ops, DropTargetListener dtl, boolean act) throws java.awt.HeadlessException
		public DropTarget(Component c, int ops, DropTargetListener dtl, bool act) : this(c, ops, dtl, act, null)
		{
			if (!InstanceFieldsInitialized)
			{
				InitializeInstanceFields();
				InstanceFieldsInitialized = true;
			}
		}

		/// <summary>
		/// Creates a <code>DropTarget</code>. </summary>
		/// <exception cref="HeadlessException"> if GraphicsEnvironment.isHeadless()
		///            returns true </exception>
		/// <seealso cref= java.awt.GraphicsEnvironment#isHeadless </seealso>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public DropTarget() throws java.awt.HeadlessException
		public DropTarget() : this(null, DnDConstants.ACTION_COPY_OR_MOVE, null, true, null)
		{
			if (!InstanceFieldsInitialized)
			{
				InitializeInstanceFields();
				InstanceFieldsInitialized = true;
			}
		}

		/// <summary>
		/// Creates a <code>DropTarget</code> given the <code>Component</code>
		/// to associate itself with, and the <code>DropTargetListener</code>
		/// to handle event processing.
		/// <P>
		/// The Component will receive drops only if it is enabled. </summary>
		/// <param name="c">         The <code>Component</code> with which this <code>DropTarget</code> is associated </param>
		/// <param name="dtl">       The <code>DropTargetListener</code> for this <code>DropTarget</code> </param>
		/// <exception cref="HeadlessException"> if GraphicsEnvironment.isHeadless()
		///            returns true </exception>
		/// <seealso cref= java.awt.GraphicsEnvironment#isHeadless </seealso>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public DropTarget(java.awt.Component c, DropTargetListener dtl) throws java.awt.HeadlessException
		public DropTarget(Component c, DropTargetListener dtl) : this(c, DnDConstants.ACTION_COPY_OR_MOVE, dtl, true, null)
		{
			if (!InstanceFieldsInitialized)
			{
				InitializeInstanceFields();
				InstanceFieldsInitialized = true;
			}
		}

		/// <summary>
		/// Creates a <code>DropTarget</code> given the <code>Component</code>
		/// to associate itself with, an <code>int</code> representing
		/// the default acceptable action(s) to support, and a
		/// <code>DropTargetListener</code> to handle event processing.
		/// <P>
		/// The Component will receive drops only if it is enabled. </summary>
		/// <param name="c">         The <code>Component</code> with which this <code>DropTarget</code> is associated </param>
		/// <param name="ops">       The default acceptable actions for this <code>DropTarget</code> </param>
		/// <param name="dtl">       The <code>DropTargetListener</code> for this <code>DropTarget</code> </param>
		/// <exception cref="HeadlessException"> if GraphicsEnvironment.isHeadless()
		///            returns true </exception>
		/// <seealso cref= java.awt.GraphicsEnvironment#isHeadless </seealso>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public DropTarget(java.awt.Component c, int ops, DropTargetListener dtl) throws java.awt.HeadlessException
		public DropTarget(Component c, int ops, DropTargetListener dtl) : this(c, ops, dtl, true)
		{
			if (!InstanceFieldsInitialized)
			{
				InitializeInstanceFields();
				InstanceFieldsInitialized = true;
			}
		}

		/// <summary>
		/// Note: this interface is required to permit the safe association
		/// of a DropTarget with a Component in one of two ways, either:
		/// <code> component.setDropTarget(droptarget); </code>
		/// or <code> droptarget.setComponent(component); </code>
		/// <P>
		/// The Component will receive drops only if it is enabled. </summary>
		/// <param name="c"> The new <code>Component</code> this <code>DropTarget</code>
		/// is to be associated with. </param>

		public virtual Component Component
		{
			set
			{
				lock (this)
				{
					if (Component_Renamed == value || Component_Renamed != null && Component_Renamed.Equals(value))
					{
						return;
					}
            
					Component old;
					ComponentPeer oldPeer = null;
            
					if ((old = Component_Renamed) != null)
					{
						ClearAutoscroll();
            
						Component_Renamed = null;
            
						if (ComponentPeer != null)
						{
							oldPeer = ComponentPeer;
							RemoveNotify(ComponentPeer);
						}
            
						old.DropTarget = null;
            
					}
            
					if ((Component_Renamed = value) != null)
					{
						try
						{
						value.DropTarget = this;
						} // undo the change
					catch (Exception)
					{
						if (old != null)
						{
							old.DropTarget = this;
							AddNotify(oldPeer);
						}
					}
					}
				}
			}
			get
			{
				lock (this)
				{
					return Component_Renamed;
				}
			}
		}



		/// <summary>
		/// Sets the default acceptable actions for this <code>DropTarget</code>
		/// <P> </summary>
		/// <param name="ops"> the default actions </param>
		/// <seealso cref= java.awt.dnd.DnDConstants </seealso>

		public virtual int DefaultActions
		{
			set
			{
				DropTargetContext.TargetActions = value & (DnDConstants.ACTION_COPY_OR_MOVE | DnDConstants.ACTION_REFERENCE);
			}
			get
			{
				return Actions;
			}
		}

		/*
		 * Called by DropTargetContext.setTargetActions()
		 * with appropriate synchronization.
		 */
		internal virtual void DoSetDefaultActions(int ops)
		{
			Actions = ops;
		}



		/// <summary>
		/// Sets the DropTarget active if <code>true</code>,
		/// inactive if <code>false</code>.
		/// <P> </summary>
		/// <param name="isActive"> sets the <code>DropTarget</code> (in)active. </param>

		public virtual bool Active
		{
			set
			{
				lock (this)
				{
					if (value != Active_Renamed)
					{
						Active_Renamed = value;
					}
            
					if (!Active_Renamed)
					{
						ClearAutoscroll();
					}
				}
			}
			get
			{
				return Active_Renamed;
			}
		}



		/// <summary>
		/// Adds a new <code>DropTargetListener</code> (UNICAST SOURCE).
		/// <P> </summary>
		/// <param name="dtl"> The new <code>DropTargetListener</code>
		/// <P> </param>
		/// <exception cref="TooManyListenersException"> if a
		/// <code>DropTargetListener</code> is already added to this
		/// <code>DropTarget</code>. </exception>

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public synchronized void addDropTargetListener(DropTargetListener dtl) throws java.util.TooManyListenersException
		public virtual void AddDropTargetListener(DropTargetListener dtl)
		{
			lock (this)
			{
				if (dtl == null)
				{
					return;
				}
        
				if (Equals(dtl))
				{
					throw new IllegalArgumentException("DropTarget may not be its own Listener");
				}
        
				if (DtListener == null)
				{
					DtListener = dtl;
				}
				else
				{
					throw new TooManyListenersException();
				}
			}
		}

		/// <summary>
		/// Removes the current <code>DropTargetListener</code> (UNICAST SOURCE).
		/// <P> </summary>
		/// <param name="dtl"> the DropTargetListener to deregister. </param>

		public virtual void RemoveDropTargetListener(DropTargetListener dtl)
		{
			lock (this)
			{
				if (dtl != null && DtListener != null)
				{
					if (DtListener.Equals(dtl))
					{
						DtListener = null;
					}
					else
					{
						throw new IllegalArgumentException("listener mismatch");
					}
				}
			}
		}

		/// <summary>
		/// Calls <code>dragEnter</code> on the registered
		/// <code>DropTargetListener</code> and passes it
		/// the specified <code>DropTargetDragEvent</code>.
		/// Has no effect if this <code>DropTarget</code>
		/// is not active.
		/// </summary>
		/// <param name="dtde"> the <code>DropTargetDragEvent</code>
		/// </param>
		/// <exception cref="NullPointerException"> if this <code>DropTarget</code>
		///         is active and <code>dtde</code> is <code>null</code>
		/// </exception>
		/// <seealso cref= #isActive </seealso>
		public virtual void DragEnter(DropTargetDragEvent dtde)
		{
			lock (this)
			{
				IsDraggingInside = true;
        
				if (!Active_Renamed)
				{
					return;
				}
        
				if (DtListener != null)
				{
					DtListener.DragEnter(dtde);
				}
				else
				{
					dtde.DropTargetContext.TargetActions = DnDConstants.ACTION_NONE;
				}
        
				InitializeAutoscrolling(dtde.Location);
			}
		}

		/// <summary>
		/// Calls <code>dragOver</code> on the registered
		/// <code>DropTargetListener</code> and passes it
		/// the specified <code>DropTargetDragEvent</code>.
		/// Has no effect if this <code>DropTarget</code>
		/// is not active.
		/// </summary>
		/// <param name="dtde"> the <code>DropTargetDragEvent</code>
		/// </param>
		/// <exception cref="NullPointerException"> if this <code>DropTarget</code>
		///         is active and <code>dtde</code> is <code>null</code>
		/// </exception>
		/// <seealso cref= #isActive </seealso>
		public virtual void DragOver(DropTargetDragEvent dtde)
		{
			lock (this)
			{
				if (!Active_Renamed)
				{
					return;
				}
        
				if (DtListener != null && Active_Renamed)
				{
					DtListener.DragOver(dtde);
				}
        
				UpdateAutoscroll(dtde.Location);
			}
		}

		/// <summary>
		/// Calls <code>dropActionChanged</code> on the registered
		/// <code>DropTargetListener</code> and passes it
		/// the specified <code>DropTargetDragEvent</code>.
		/// Has no effect if this <code>DropTarget</code>
		/// is not active.
		/// </summary>
		/// <param name="dtde"> the <code>DropTargetDragEvent</code>
		/// </param>
		/// <exception cref="NullPointerException"> if this <code>DropTarget</code>
		///         is active and <code>dtde</code> is <code>null</code>
		/// </exception>
		/// <seealso cref= #isActive </seealso>
		public virtual void DropActionChanged(DropTargetDragEvent dtde)
		{
			lock (this)
			{
				if (!Active_Renamed)
				{
					return;
				}
        
				if (DtListener != null)
				{
					DtListener.DropActionChanged(dtde);
				}
        
				UpdateAutoscroll(dtde.Location);
			}
		}

		/// <summary>
		/// Calls <code>dragExit</code> on the registered
		/// <code>DropTargetListener</code> and passes it
		/// the specified <code>DropTargetEvent</code>.
		/// Has no effect if this <code>DropTarget</code>
		/// is not active.
		/// <para>
		/// This method itself does not throw any exception
		/// for null parameter but for exceptions thrown by
		/// the respective method of the listener.
		/// 
		/// </para>
		/// </summary>
		/// <param name="dte"> the <code>DropTargetEvent</code>
		/// </param>
		/// <seealso cref= #isActive </seealso>
		public virtual void DragExit(DropTargetEvent dte)
		{
			lock (this)
			{
				IsDraggingInside = false;
        
				if (!Active_Renamed)
				{
					return;
				}
        
				if (DtListener != null && Active_Renamed)
				{
					DtListener.DragExit(dte);
				}
        
				ClearAutoscroll();
			}
		}

		/// <summary>
		/// Calls <code>drop</code> on the registered
		/// <code>DropTargetListener</code> and passes it
		/// the specified <code>DropTargetDropEvent</code>
		/// if this <code>DropTarget</code> is active.
		/// </summary>
		/// <param name="dtde"> the <code>DropTargetDropEvent</code>
		/// </param>
		/// <exception cref="NullPointerException"> if <code>dtde</code> is null
		///         and at least one of the following is true: this
		///         <code>DropTarget</code> is not active, or there is
		///         no a <code>DropTargetListener</code> registered.
		/// </exception>
		/// <seealso cref= #isActive </seealso>
		public virtual void Drop(DropTargetDropEvent dtde)
		{
			lock (this)
			{
				IsDraggingInside = false;
        
				ClearAutoscroll();
        
				if (DtListener != null && Active_Renamed)
				{
					DtListener.Drop(dtde);
				}
				else // we should'nt get here ...
				{
					dtde.RejectDrop();
				}
			}
		}

		/// <summary>
		/// Gets the <code>FlavorMap</code>
		/// associated with this <code>DropTarget</code>.
		/// If no <code>FlavorMap</code> has been set for this
		/// <code>DropTarget</code>, it is associated with the default
		/// <code>FlavorMap</code>.
		/// <P> </summary>
		/// <returns> the FlavorMap for this DropTarget </returns>

		public virtual FlavorMap FlavorMap
		{
			get
			{
				return FlavorMap_Renamed;
			}
			set
			{
				FlavorMap_Renamed = value == null ? SystemFlavorMap.DefaultFlavorMap : value;
			}
		}



		/// <summary>
		/// Notify the DropTarget that it has been associated with a Component
		/// 
		/// **********************************************************************
		/// This method is usually called from java.awt.Component.addNotify() of
		/// the Component associated with this DropTarget to notify the DropTarget
		/// that a ComponentPeer has been associated with that Component.
		/// 
		/// Calling this method, other than to notify this DropTarget of the
		/// association of the ComponentPeer with the Component may result in
		/// a malfunction of the DnD system.
		/// **********************************************************************
		/// <P> </summary>
		/// <param name="peer"> The Peer of the Component we are associated with!
		///  </param>

		public virtual void AddNotify(ComponentPeer peer)
		{
			if (peer == ComponentPeer)
			{
				return;
			}

			ComponentPeer = peer;

			for (Component c = Component_Renamed; c != null && peer is LightweightPeer; c = c.Parent)
			{
				peer = c.Peer;
			}

			if (peer is DropTargetPeer)
			{
				NativePeer = peer;
				((DropTargetPeer)peer).AddDropTarget(this);
			}
			else
			{
				NativePeer = null;
			}
		}

		/// <summary>
		/// Notify the DropTarget that it has been disassociated from a Component
		/// 
		/// **********************************************************************
		/// This method is usually called from java.awt.Component.removeNotify() of
		/// the Component associated with this DropTarget to notify the DropTarget
		/// that a ComponentPeer has been disassociated with that Component.
		/// 
		/// Calling this method, other than to notify this DropTarget of the
		/// disassociation of the ComponentPeer from the Component may result in
		/// a malfunction of the DnD system.
		/// **********************************************************************
		/// <P> </summary>
		/// <param name="peer"> The Peer of the Component we are being disassociated from! </param>

		public virtual void RemoveNotify(ComponentPeer peer)
		{
			if (NativePeer != null)
			{
				((DropTargetPeer)NativePeer).RemoveDropTarget(this);
			}

			ComponentPeer = NativePeer = null;

			lock (this)
			{
				if (IsDraggingInside)
				{
					DragExit(new DropTargetEvent(DropTargetContext));
				}
			}
		}

		/// <summary>
		/// Gets the <code>DropTargetContext</code> associated
		/// with this <code>DropTarget</code>.
		/// <P> </summary>
		/// <returns> the <code>DropTargetContext</code> associated with this <code>DropTarget</code>. </returns>

		public virtual DropTargetContext DropTargetContext
		{
			get
			{
				return DropTargetContext_Renamed;
			}
		}

		/// <summary>
		/// Creates the DropTargetContext associated with this DropTarget.
		/// Subclasses may override this method to instantiate their own
		/// DropTargetContext subclass.
		/// 
		/// This call is typically *only* called by the platform's
		/// DropTargetContextPeer as a drag operation encounters this
		/// DropTarget. Accessing the Context while no Drag is current
		/// has undefined results.
		/// </summary>

		protected internal virtual DropTargetContext CreateDropTargetContext()
		{
			return new DropTargetContext(this);
		}

		/// <summary>
		/// Serializes this <code>DropTarget</code>. Performs default serialization,
		/// and then writes out this object's <code>DropTargetListener</code> if and
		/// only if it can be serialized. If not, <code>null</code> is written
		/// instead.
		/// 
		/// @serialData The default serializable fields, in alphabetical order,
		///             followed by either a <code>DropTargetListener</code>
		///             instance, or <code>null</code>.
		/// @since 1.4
		/// </summary>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: private void writeObject(java.io.ObjectOutputStream s) throws java.io.IOException
		private void WriteObject(ObjectOutputStream s)
		{
			s.DefaultWriteObject();

			s.WriteObject(SerializationTester.Test(DtListener) ? DtListener : null);
		}

		/// <summary>
		/// Deserializes this <code>DropTarget</code>. This method first performs
		/// default deserialization for all non-<code>transient</code> fields. An
		/// attempt is then made to deserialize this object's
		/// <code>DropTargetListener</code> as well. This is first attempted by
		/// deserializing the field <code>dtListener</code>, because, in releases
		/// prior to 1.4, a non-<code>transient</code> field of this name stored the
		/// <code>DropTargetListener</code>. If this fails, the next object in the
		/// stream is used instead.
		/// 
		/// @since 1.4
		/// </summary>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: private void readObject(java.io.ObjectInputStream s) throws ClassNotFoundException, java.io.IOException
		private void ReadObject(ObjectInputStream s)
		{
			ObjectInputStream.GetField f = s.ReadFields();

			try
			{
				DropTargetContext_Renamed = (DropTargetContext)f.Get("dropTargetContext", null);
			}
			catch (IllegalArgumentException)
			{
				// Pre-1.4 support. 'dropTargetContext' was previously transient
			}
			if (DropTargetContext_Renamed == null)
			{
				DropTargetContext_Renamed = CreateDropTargetContext();
			}

			Component_Renamed = (Component)f.Get("component", null);
			Actions = f.Get("actions", DnDConstants.ACTION_COPY_OR_MOVE);
			Active_Renamed = f.Get("active", true);

			// Pre-1.4 support. 'dtListener' was previously non-transient
			try
			{
				DtListener = (DropTargetListener)f.Get("dtListener", null);
			}
			catch (IllegalArgumentException)
			{
				// 1.4-compatible byte stream. 'dtListener' was written explicitly
				DtListener = (DropTargetListener)s.ReadObject();
			}
		}

		/// <summary>
		///****************************************************************** </summary>

		/// <summary>
		/// this protected nested class implements autoscrolling
		/// </summary>

		protected internal class DropTargetAutoScroller : ActionListener
		{

			/// <summary>
			/// construct a DropTargetAutoScroller
			/// <P> </summary>
			/// <param name="c"> the <code>Component</code> </param>
			/// <param name="p"> the <code>Point</code> </param>

			protected internal DropTargetAutoScroller(Component c, Point p) : base()
			{

				Component = c;
				AutoScroll = (Autoscroll)Component;

				Toolkit t = Toolkit.DefaultToolkit;

				Integer initial = Convert.ToInt32(100);
				Integer interval = Convert.ToInt32(100);

				try
				{
					initial = (Integer)t.GetDesktopProperty("DnD.Autoscroll.initialDelay");
				}
				catch (Exception)
				{
					// ignore
				}

				try
				{
					interval = (Integer)t.GetDesktopProperty("DnD.Autoscroll.interval");
				}
				catch (Exception)
				{
					// ignore
				}

				Timer = new Timer(interval.IntValue(), this);

				Timer.Coalesce = true;
				Timer.InitialDelay = initial.IntValue();

				Locn = p;
				Prev = p;

				try
				{
					Hysteresis = ((Integer)t.GetDesktopProperty("DnD.Autoscroll.cursorHysteresis")).IntValue();
				}
				catch (Exception)
				{
					// ignore
				}

				Timer.start();
			}

			/// <summary>
			/// update the geometry of the autoscroll region
			/// </summary>

			internal virtual void UpdateRegion()
			{
			   Insets i = AutoScroll.AutoscrollInsets;
			   Dimension size = Component.Size;

			   if (size.Width_Renamed != Outer.Width_Renamed || size.Height_Renamed != Outer.Height_Renamed)
			   {
					Outer.Reshape(0, 0, size.Width_Renamed, size.Height_Renamed);
			   }

			   if (Inner.x != i.Left || Inner.y != i.Top)
			   {
					Inner.SetLocation(i.Left, i.Top);
			   }

			   int newWidth = size.Width_Renamed - (i.Left + i.Right);
			   int newHeight = size.Height_Renamed - (i.Top + i.Bottom);

			   if (newWidth != Inner.Width_Renamed || newHeight != Inner.Height_Renamed)
			   {
					Inner.SetSize(newWidth, newHeight);
			   }

			}

			/// <summary>
			/// cause autoscroll to occur
			/// <P> </summary>
			/// <param name="newLocn"> the <code>Point</code> </param>

			protected internal virtual void UpdateLocation(Point newLocn)
			{
				lock (this)
				{
					Prev = Locn;
					Locn = newLocn;
        
					if (System.Math.Abs(Locn.x - Prev.x) > Hysteresis || System.Math.Abs(Locn.y - Prev.y) > Hysteresis)
					{
						if (Timer.Running)
						{
							Timer.stop();
						}
					}
					else
					{
						if (!Timer.Running)
						{
							Timer.start();
						}
					}
				}
			}

			/// <summary>
			/// cause autoscrolling to stop
			/// </summary>

			protected internal virtual void Stop()
			{
				Timer.stop();
			}

			/// <summary>
			/// cause autoscroll to occur
			/// <P> </summary>
			/// <param name="e"> the <code>ActionEvent</code> </param>

			public virtual void ActionPerformed(ActionEvent e)
			{
				lock (this)
				{
					UpdateRegion();
        
					if (Outer.Contains(Locn) && !Inner.Contains(Locn))
					{
						AutoScroll.Autoscroll(Locn);
					}
				}
			}

			/*
			 * fields
			 */

			internal Component Component;
			internal Autoscroll AutoScroll;

			internal Timer Timer;

			internal Point Locn;
			internal Point Prev;

			internal Rectangle Outer = new Rectangle();
			internal Rectangle Inner = new Rectangle();

			internal int Hysteresis = 10;
		}

		/// <summary>
		///****************************************************************** </summary>

		/// <summary>
		/// create an embedded autoscroller
		/// <P> </summary>
		/// <param name="c"> the <code>Component</code> </param>
		/// <param name="p"> the <code>Point</code> </param>

		protected internal virtual DropTargetAutoScroller CreateDropTargetAutoScroller(Component c, Point p)
		{
			return new DropTargetAutoScroller(c, p);
		}

		/// <summary>
		/// initialize autoscrolling
		/// <P> </summary>
		/// <param name="p"> the <code>Point</code> </param>

		protected internal virtual void InitializeAutoscrolling(Point p)
		{
			if (Component_Renamed == null || !(Component_Renamed is Autoscroll))
			{
				return;
			}

			AutoScroller = CreateDropTargetAutoScroller(Component_Renamed, p);
		}

		/// <summary>
		/// update autoscrolling with current cursor location
		/// <P> </summary>
		/// <param name="dragCursorLocn"> the <code>Point</code> </param>

		protected internal virtual void UpdateAutoscroll(Point dragCursorLocn)
		{
			if (AutoScroller != null)
			{
				AutoScroller.UpdateLocation(dragCursorLocn);
			}
		}

		/// <summary>
		/// clear autoscrolling
		/// </summary>

		protected internal virtual void ClearAutoscroll()
		{
			if (AutoScroller != null)
			{
				AutoScroller.Stop();
				AutoScroller = null;
			}
		}

		/// <summary>
		/// The DropTargetContext associated with this DropTarget.
		/// 
		/// @serial
		/// </summary>
		private DropTargetContext DropTargetContext_Renamed;

		/// <summary>
		/// The Component associated with this DropTarget.
		/// 
		/// @serial
		/// </summary>
		private Component Component_Renamed;

		/*
		 * That Component's  Peer
		 */
		[NonSerialized]
		private ComponentPeer ComponentPeer;

		/*
		 * That Component's "native" Peer
		 */
		[NonSerialized]
		private ComponentPeer NativePeer;


		/// <summary>
		/// Default permissible actions supported by this DropTarget.
		/// </summary>
		/// <seealso cref= #setDefaultActions </seealso>
		/// <seealso cref= #getDefaultActions
		/// @serial </seealso>
		internal int Actions = DnDConstants.ACTION_COPY_OR_MOVE;

		/// <summary>
		/// <code>true</code> if the DropTarget is accepting Drag &amp; Drop operations.
		/// 
		/// @serial
		/// </summary>
		internal bool Active_Renamed = true;

		/*
		 * the auto scrolling object
		 */

		[NonSerialized]
		private DropTargetAutoScroller AutoScroller;

		/*
		 * The delegate
		 */

		[NonSerialized]
		private DropTargetListener DtListener;

		/*
		 * The FlavorMap
		 */

		[NonSerialized]
		private FlavorMap FlavorMap_Renamed;

		/*
		 * If the dragging is currently inside this drop target
		 */
		[NonSerialized]
		private bool IsDraggingInside;
	}

}
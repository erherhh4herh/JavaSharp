using System;

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
namespace java.awt
{



	/// <summary>
	/// {@code AWTEventMulticaster} implements efficient and thread-safe multi-cast
	/// event dispatching for the AWT events defined in the {@code java.awt.event}
	/// package.
	/// <para>
	/// The following example illustrates how to use this class:
	/// 
	/// <pre><code>
	/// public myComponent extends Component {
	///     ActionListener actionListener = null;
	/// 
	///     public synchronized void addActionListener(ActionListener l) {
	///         actionListener = AWTEventMulticaster.add(actionListener, l);
	///     }
	///     public synchronized void removeActionListener(ActionListener l) {
	///         actionListener = AWTEventMulticaster.remove(actionListener, l);
	///     }
	///     public void processEvent(AWTEvent e) {
	///         // when event occurs which causes "action" semantic
	///         ActionListener listener = actionListener;
	///         if (listener != null) {
	///             listener.actionPerformed(new ActionEvent());
	///         }
	///     }
	/// }
	/// </code></pre>
	/// The important point to note is the first argument to the {@code
	/// add} and {@code remove} methods is the field maintaining the
	/// listeners. In addition you must assign the result of the {@code add}
	/// and {@code remove} methods to the field maintaining the listeners.
	/// </para>
	/// <para>
	/// {@code AWTEventMulticaster} is implemented as a pair of {@code
	/// EventListeners} that are set at construction time. {@code
	/// AWTEventMulticaster} is immutable. The {@code add} and {@code
	/// remove} methods do not alter {@code AWTEventMulticaster} in
	/// anyway. If necessary, a new {@code AWTEventMulticaster} is
	/// created. In this way it is safe to add and remove listeners during
	/// the process of an event dispatching.  However, event listeners
	/// added during the process of an event dispatch operation are not
	/// notified of the event currently being dispatched.
	/// </para>
	/// <para>
	/// All of the {@code add} methods allow {@code null} arguments. If the
	/// first argument is {@code null}, the second argument is returned. If
	/// the first argument is not {@code null} and the second argument is
	/// {@code null}, the first argument is returned. If both arguments are
	/// {@code non-null}, a new {@code AWTEventMulticaster} is created using
	/// the two arguments and returned.
	/// </para>
	/// <para>
	/// For the {@code remove} methods that take two arguments, the following is
	/// returned:
	/// <ul>
	///   <li>{@code null}, if the first argument is {@code null}, or
	///       the arguments are equal, by way of {@code ==}.
	///   <li>the first argument, if the first argument is not an instance of
	///       {@code AWTEventMulticaster}.
	///   <li>result of invoking {@code remove(EventListener)} on the
	///       first argument, supplying the second argument to the
	///       {@code remove(EventListener)} method.
	/// </ul>
	/// </para>
	/// <para>Swing makes use of
	/// <seealso cref="javax.swing.event.EventListenerList EventListenerList"/> for
	/// similar logic. Refer to it for details.
	/// 
	/// </para>
	/// </summary>
	/// <seealso cref= javax.swing.event.EventListenerList
	/// 
	/// @author      John Rose
	/// @author      Amy Fowler
	/// @since       1.1 </seealso>

	public class AWTEventMulticaster : ComponentListener, ContainerListener, FocusListener, KeyListener, MouseListener, MouseMotionListener, WindowListener, WindowFocusListener, WindowStateListener, ActionListener, ItemListener, AdjustmentListener, TextListener, InputMethodListener, HierarchyListener, HierarchyBoundsListener, MouseWheelListener
	{

		protected internal readonly EventListener a, b;

		/// <summary>
		/// Creates an event multicaster instance which chains listener-a
		/// with listener-b. Input parameters <code>a</code> and <code>b</code>
		/// should not be <code>null</code>, though implementations may vary in
		/// choosing whether or not to throw <code>NullPointerException</code>
		/// in that case. </summary>
		/// <param name="a"> listener-a </param>
		/// <param name="b"> listener-b </param>
		protected internal AWTEventMulticaster(EventListener a, EventListener b)
		{
			this.a = a;
			this.b = b;
		}

		/// <summary>
		/// Removes a listener from this multicaster.
		/// <para>
		/// The returned multicaster contains all the listeners in this
		/// multicaster with the exception of all occurrences of {@code oldl}.
		/// If the resulting multicaster contains only one regular listener
		/// the regular listener may be returned.  If the resulting multicaster
		/// is empty, then {@code null} may be returned instead.
		/// </para>
		/// <para>
		/// No exception is thrown if {@code oldl} is {@code null}.
		/// 
		/// </para>
		/// </summary>
		/// <param name="oldl"> the listener to be removed </param>
		/// <returns> resulting listener </returns>
		protected internal virtual EventListener Remove(EventListener oldl)
		{
			if (oldl == a)
			{
				return b;
			}
			if (oldl == b)
			{
				return a;
			}
			EventListener a2 = RemoveInternal(a, oldl);
			EventListener b2 = RemoveInternal(b, oldl);
			if (a2 == a && b2 == b)
			{
				return this; // it's not here
			}
			return AddInternal(a2, b2);
		}

		/// <summary>
		/// Handles the componentResized event by invoking the
		/// componentResized methods on listener-a and listener-b. </summary>
		/// <param name="e"> the component event </param>
		public virtual void ComponentResized(ComponentEvent e)
		{
			((ComponentListener)a).ComponentResized(e);
			((ComponentListener)b).ComponentResized(e);
		}

		/// <summary>
		/// Handles the componentMoved event by invoking the
		/// componentMoved methods on listener-a and listener-b. </summary>
		/// <param name="e"> the component event </param>
		public virtual void ComponentMoved(ComponentEvent e)
		{
			((ComponentListener)a).ComponentMoved(e);
			((ComponentListener)b).ComponentMoved(e);
		}

		/// <summary>
		/// Handles the componentShown event by invoking the
		/// componentShown methods on listener-a and listener-b. </summary>
		/// <param name="e"> the component event </param>
		public virtual void ComponentShown(ComponentEvent e)
		{
			((ComponentListener)a).ComponentShown(e);
			((ComponentListener)b).ComponentShown(e);
		}

		/// <summary>
		/// Handles the componentHidden event by invoking the
		/// componentHidden methods on listener-a and listener-b. </summary>
		/// <param name="e"> the component event </param>
		public virtual void ComponentHidden(ComponentEvent e)
		{
			((ComponentListener)a).ComponentHidden(e);
			((ComponentListener)b).ComponentHidden(e);
		}

		/// <summary>
		/// Handles the componentAdded container event by invoking the
		/// componentAdded methods on listener-a and listener-b. </summary>
		/// <param name="e"> the component event </param>
		public virtual void ComponentAdded(ContainerEvent e)
		{
			((ContainerListener)a).ComponentAdded(e);
			((ContainerListener)b).ComponentAdded(e);
		}

		/// <summary>
		/// Handles the componentRemoved container event by invoking the
		/// componentRemoved methods on listener-a and listener-b. </summary>
		/// <param name="e"> the component event </param>
		public virtual void ComponentRemoved(ContainerEvent e)
		{
			((ContainerListener)a).ComponentRemoved(e);
			((ContainerListener)b).ComponentRemoved(e);
		}

		/// <summary>
		/// Handles the focusGained event by invoking the
		/// focusGained methods on listener-a and listener-b. </summary>
		/// <param name="e"> the focus event </param>
		public virtual void FocusGained(FocusEvent e)
		{
			((FocusListener)a).FocusGained(e);
			((FocusListener)b).FocusGained(e);
		}

		/// <summary>
		/// Handles the focusLost event by invoking the
		/// focusLost methods on listener-a and listener-b. </summary>
		/// <param name="e"> the focus event </param>
		public virtual void FocusLost(FocusEvent e)
		{
			((FocusListener)a).FocusLost(e);
			((FocusListener)b).FocusLost(e);
		}

		/// <summary>
		/// Handles the keyTyped event by invoking the
		/// keyTyped methods on listener-a and listener-b. </summary>
		/// <param name="e"> the key event </param>
		public virtual void KeyTyped(KeyEvent e)
		{
			((KeyListener)a).KeyTyped(e);
			((KeyListener)b).KeyTyped(e);
		}

		/// <summary>
		/// Handles the keyPressed event by invoking the
		/// keyPressed methods on listener-a and listener-b. </summary>
		/// <param name="e"> the key event </param>
		public virtual void KeyPressed(KeyEvent e)
		{
			((KeyListener)a).KeyPressed(e);
			((KeyListener)b).KeyPressed(e);
		}

		/// <summary>
		/// Handles the keyReleased event by invoking the
		/// keyReleased methods on listener-a and listener-b. </summary>
		/// <param name="e"> the key event </param>
		public virtual void KeyReleased(KeyEvent e)
		{
			((KeyListener)a).KeyReleased(e);
			((KeyListener)b).KeyReleased(e);
		}

		/// <summary>
		/// Handles the mouseClicked event by invoking the
		/// mouseClicked methods on listener-a and listener-b. </summary>
		/// <param name="e"> the mouse event </param>
		public virtual void MouseClicked(MouseEvent e)
		{
			((MouseListener)a).MouseClicked(e);
			((MouseListener)b).MouseClicked(e);
		}

		/// <summary>
		/// Handles the mousePressed event by invoking the
		/// mousePressed methods on listener-a and listener-b. </summary>
		/// <param name="e"> the mouse event </param>
		public virtual void MousePressed(MouseEvent e)
		{
			((MouseListener)a).MousePressed(e);
			((MouseListener)b).MousePressed(e);
		}

		/// <summary>
		/// Handles the mouseReleased event by invoking the
		/// mouseReleased methods on listener-a and listener-b. </summary>
		/// <param name="e"> the mouse event </param>
		public virtual void MouseReleased(MouseEvent e)
		{
			((MouseListener)a).MouseReleased(e);
			((MouseListener)b).MouseReleased(e);
		}

		/// <summary>
		/// Handles the mouseEntered event by invoking the
		/// mouseEntered methods on listener-a and listener-b. </summary>
		/// <param name="e"> the mouse event </param>
		public virtual void MouseEntered(MouseEvent e)
		{
			((MouseListener)a).MouseEntered(e);
			((MouseListener)b).MouseEntered(e);
		}

		/// <summary>
		/// Handles the mouseExited event by invoking the
		/// mouseExited methods on listener-a and listener-b. </summary>
		/// <param name="e"> the mouse event </param>
		public virtual void MouseExited(MouseEvent e)
		{
			((MouseListener)a).MouseExited(e);
			((MouseListener)b).MouseExited(e);
		}

		/// <summary>
		/// Handles the mouseDragged event by invoking the
		/// mouseDragged methods on listener-a and listener-b. </summary>
		/// <param name="e"> the mouse event </param>
		public virtual void MouseDragged(MouseEvent e)
		{
			((MouseMotionListener)a).MouseDragged(e);
			((MouseMotionListener)b).MouseDragged(e);
		}

		/// <summary>
		/// Handles the mouseMoved event by invoking the
		/// mouseMoved methods on listener-a and listener-b. </summary>
		/// <param name="e"> the mouse event </param>
		public virtual void MouseMoved(MouseEvent e)
		{
			((MouseMotionListener)a).MouseMoved(e);
			((MouseMotionListener)b).MouseMoved(e);
		}

		/// <summary>
		/// Handles the windowOpened event by invoking the
		/// windowOpened methods on listener-a and listener-b. </summary>
		/// <param name="e"> the window event </param>
		public virtual void WindowOpened(WindowEvent e)
		{
			((WindowListener)a).WindowOpened(e);
			((WindowListener)b).WindowOpened(e);
		}

		/// <summary>
		/// Handles the windowClosing event by invoking the
		/// windowClosing methods on listener-a and listener-b. </summary>
		/// <param name="e"> the window event </param>
		public virtual void WindowClosing(WindowEvent e)
		{
			((WindowListener)a).WindowClosing(e);
			((WindowListener)b).WindowClosing(e);
		}

		/// <summary>
		/// Handles the windowClosed event by invoking the
		/// windowClosed methods on listener-a and listener-b. </summary>
		/// <param name="e"> the window event </param>
		public virtual void WindowClosed(WindowEvent e)
		{
			((WindowListener)a).WindowClosed(e);
			((WindowListener)b).WindowClosed(e);
		}

		/// <summary>
		/// Handles the windowIconified event by invoking the
		/// windowIconified methods on listener-a and listener-b. </summary>
		/// <param name="e"> the window event </param>
		public virtual void WindowIconified(WindowEvent e)
		{
			((WindowListener)a).WindowIconified(e);
			((WindowListener)b).WindowIconified(e);
		}

		/// <summary>
		/// Handles the windowDeiconfied event by invoking the
		/// windowDeiconified methods on listener-a and listener-b. </summary>
		/// <param name="e"> the window event </param>
		public virtual void WindowDeiconified(WindowEvent e)
		{
			((WindowListener)a).WindowDeiconified(e);
			((WindowListener)b).WindowDeiconified(e);
		}

		/// <summary>
		/// Handles the windowActivated event by invoking the
		/// windowActivated methods on listener-a and listener-b. </summary>
		/// <param name="e"> the window event </param>
		public virtual void WindowActivated(WindowEvent e)
		{
			((WindowListener)a).WindowActivated(e);
			((WindowListener)b).WindowActivated(e);
		}

		/// <summary>
		/// Handles the windowDeactivated event by invoking the
		/// windowDeactivated methods on listener-a and listener-b. </summary>
		/// <param name="e"> the window event </param>
		public virtual void WindowDeactivated(WindowEvent e)
		{
			((WindowListener)a).WindowDeactivated(e);
			((WindowListener)b).WindowDeactivated(e);
		}

		/// <summary>
		/// Handles the windowStateChanged event by invoking the
		/// windowStateChanged methods on listener-a and listener-b. </summary>
		/// <param name="e"> the window event
		/// @since 1.4 </param>
		public virtual void WindowStateChanged(WindowEvent e)
		{
			((WindowStateListener)a).WindowStateChanged(e);
			((WindowStateListener)b).WindowStateChanged(e);
		}


		/// <summary>
		/// Handles the windowGainedFocus event by invoking the windowGainedFocus
		/// methods on listener-a and listener-b. </summary>
		/// <param name="e"> the window event
		/// @since 1.4 </param>
		public virtual void WindowGainedFocus(WindowEvent e)
		{
			((WindowFocusListener)a).WindowGainedFocus(e);
			((WindowFocusListener)b).WindowGainedFocus(e);
		}

		/// <summary>
		/// Handles the windowLostFocus event by invoking the windowLostFocus
		/// methods on listener-a and listener-b. </summary>
		/// <param name="e"> the window event
		/// @since 1.4 </param>
		public virtual void WindowLostFocus(WindowEvent e)
		{
			((WindowFocusListener)a).WindowLostFocus(e);
			((WindowFocusListener)b).WindowLostFocus(e);
		}

		/// <summary>
		/// Handles the actionPerformed event by invoking the
		/// actionPerformed methods on listener-a and listener-b. </summary>
		/// <param name="e"> the action event </param>
		public virtual void ActionPerformed(ActionEvent e)
		{
			((ActionListener)a).ActionPerformed(e);
			((ActionListener)b).ActionPerformed(e);
		}

		/// <summary>
		/// Handles the itemStateChanged event by invoking the
		/// itemStateChanged methods on listener-a and listener-b. </summary>
		/// <param name="e"> the item event </param>
		public virtual void ItemStateChanged(ItemEvent e)
		{
			((ItemListener)a).ItemStateChanged(e);
			((ItemListener)b).ItemStateChanged(e);
		}

		/// <summary>
		/// Handles the adjustmentValueChanged event by invoking the
		/// adjustmentValueChanged methods on listener-a and listener-b. </summary>
		/// <param name="e"> the adjustment event </param>
		public virtual void AdjustmentValueChanged(AdjustmentEvent e)
		{
			((AdjustmentListener)a).AdjustmentValueChanged(e);
			((AdjustmentListener)b).AdjustmentValueChanged(e);
		}
		public virtual void TextValueChanged(TextEvent e)
		{
			((TextListener)a).TextValueChanged(e);
			((TextListener)b).TextValueChanged(e);
		}

		/// <summary>
		/// Handles the inputMethodTextChanged event by invoking the
		/// inputMethodTextChanged methods on listener-a and listener-b. </summary>
		/// <param name="e"> the item event </param>
		public virtual void InputMethodTextChanged(InputMethodEvent e)
		{
		   ((InputMethodListener)a).InputMethodTextChanged(e);
		   ((InputMethodListener)b).InputMethodTextChanged(e);
		}

		/// <summary>
		/// Handles the caretPositionChanged event by invoking the
		/// caretPositionChanged methods on listener-a and listener-b. </summary>
		/// <param name="e"> the item event </param>
		public virtual void CaretPositionChanged(InputMethodEvent e)
		{
		   ((InputMethodListener)a).CaretPositionChanged(e);
		   ((InputMethodListener)b).CaretPositionChanged(e);
		}

		/// <summary>
		/// Handles the hierarchyChanged event by invoking the
		/// hierarchyChanged methods on listener-a and listener-b. </summary>
		/// <param name="e"> the item event
		/// @since 1.3 </param>
		public virtual void HierarchyChanged(HierarchyEvent e)
		{
			((HierarchyListener)a).HierarchyChanged(e);
			((HierarchyListener)b).HierarchyChanged(e);
		}

		/// <summary>
		/// Handles the ancestorMoved event by invoking the
		/// ancestorMoved methods on listener-a and listener-b. </summary>
		/// <param name="e"> the item event
		/// @since 1.3 </param>
		public virtual void AncestorMoved(HierarchyEvent e)
		{
			((HierarchyBoundsListener)a).AncestorMoved(e);
			((HierarchyBoundsListener)b).AncestorMoved(e);
		}

		/// <summary>
		/// Handles the ancestorResized event by invoking the
		/// ancestorResized methods on listener-a and listener-b. </summary>
		/// <param name="e"> the item event
		/// @since 1.3 </param>
		public virtual void AncestorResized(HierarchyEvent e)
		{
			((HierarchyBoundsListener)a).AncestorResized(e);
			((HierarchyBoundsListener)b).AncestorResized(e);
		}

		/// <summary>
		/// Handles the mouseWheelMoved event by invoking the
		/// mouseWheelMoved methods on listener-a and listener-b. </summary>
		/// <param name="e"> the mouse event
		/// @since 1.4 </param>
		public virtual void MouseWheelMoved(MouseWheelEvent e)
		{
			((MouseWheelListener)a).MouseWheelMoved(e);
			((MouseWheelListener)b).MouseWheelMoved(e);
		}

		/// <summary>
		/// Adds component-listener-a with component-listener-b and
		/// returns the resulting multicast listener. </summary>
		/// <param name="a"> component-listener-a </param>
		/// <param name="b"> component-listener-b </param>
		public static ComponentListener Add(ComponentListener a, ComponentListener b)
		{
			return (ComponentListener)AddInternal(a, b);
		}

		/// <summary>
		/// Adds container-listener-a with container-listener-b and
		/// returns the resulting multicast listener. </summary>
		/// <param name="a"> container-listener-a </param>
		/// <param name="b"> container-listener-b </param>
		public static ContainerListener Add(ContainerListener a, ContainerListener b)
		{
			return (ContainerListener)AddInternal(a, b);
		}

		/// <summary>
		/// Adds focus-listener-a with focus-listener-b and
		/// returns the resulting multicast listener. </summary>
		/// <param name="a"> focus-listener-a </param>
		/// <param name="b"> focus-listener-b </param>
		public static FocusListener Add(FocusListener a, FocusListener b)
		{
			return (FocusListener)AddInternal(a, b);
		}

		/// <summary>
		/// Adds key-listener-a with key-listener-b and
		/// returns the resulting multicast listener. </summary>
		/// <param name="a"> key-listener-a </param>
		/// <param name="b"> key-listener-b </param>
		public static KeyListener Add(KeyListener a, KeyListener b)
		{
			return (KeyListener)AddInternal(a, b);
		}

		/// <summary>
		/// Adds mouse-listener-a with mouse-listener-b and
		/// returns the resulting multicast listener. </summary>
		/// <param name="a"> mouse-listener-a </param>
		/// <param name="b"> mouse-listener-b </param>
		public static MouseListener Add(MouseListener a, MouseListener b)
		{
			return (MouseListener)AddInternal(a, b);
		}

		/// <summary>
		/// Adds mouse-motion-listener-a with mouse-motion-listener-b and
		/// returns the resulting multicast listener. </summary>
		/// <param name="a"> mouse-motion-listener-a </param>
		/// <param name="b"> mouse-motion-listener-b </param>
		public static MouseMotionListener Add(MouseMotionListener a, MouseMotionListener b)
		{
			return (MouseMotionListener)AddInternal(a, b);
		}

		/// <summary>
		/// Adds window-listener-a with window-listener-b and
		/// returns the resulting multicast listener. </summary>
		/// <param name="a"> window-listener-a </param>
		/// <param name="b"> window-listener-b </param>
		public static WindowListener Add(WindowListener a, WindowListener b)
		{
			return (WindowListener)AddInternal(a, b);
		}

		/// <summary>
		/// Adds window-state-listener-a with window-state-listener-b
		/// and returns the resulting multicast listener. </summary>
		/// <param name="a"> window-state-listener-a </param>
		/// <param name="b"> window-state-listener-b
		/// @since 1.4 </param>
		public static WindowStateListener Add(WindowStateListener a, WindowStateListener b)
		{
			return (WindowStateListener)AddInternal(a, b);
		}

		/// <summary>
		/// Adds window-focus-listener-a with window-focus-listener-b
		/// and returns the resulting multicast listener. </summary>
		/// <param name="a"> window-focus-listener-a </param>
		/// <param name="b"> window-focus-listener-b
		/// @since 1.4 </param>
		public static WindowFocusListener Add(WindowFocusListener a, WindowFocusListener b)
		{
			return (WindowFocusListener)AddInternal(a, b);
		}

		/// <summary>
		/// Adds action-listener-a with action-listener-b and
		/// returns the resulting multicast listener. </summary>
		/// <param name="a"> action-listener-a </param>
		/// <param name="b"> action-listener-b </param>
		public static ActionListener Add(ActionListener a, ActionListener b)
		{
			return (ActionListener)AddInternal(a, b);
		}

		/// <summary>
		/// Adds item-listener-a with item-listener-b and
		/// returns the resulting multicast listener. </summary>
		/// <param name="a"> item-listener-a </param>
		/// <param name="b"> item-listener-b </param>
		public static ItemListener Add(ItemListener a, ItemListener b)
		{
			return (ItemListener)AddInternal(a, b);
		}

		/// <summary>
		/// Adds adjustment-listener-a with adjustment-listener-b and
		/// returns the resulting multicast listener. </summary>
		/// <param name="a"> adjustment-listener-a </param>
		/// <param name="b"> adjustment-listener-b </param>
		public static AdjustmentListener Add(AdjustmentListener a, AdjustmentListener b)
		{
			return (AdjustmentListener)AddInternal(a, b);
		}
		public static TextListener Add(TextListener a, TextListener b)
		{
			return (TextListener)AddInternal(a, b);
		}

		/// <summary>
		/// Adds input-method-listener-a with input-method-listener-b and
		/// returns the resulting multicast listener. </summary>
		/// <param name="a"> input-method-listener-a </param>
		/// <param name="b"> input-method-listener-b </param>
		 public static InputMethodListener Add(InputMethodListener a, InputMethodListener b)
		 {
			return (InputMethodListener)AddInternal(a, b);
		 }

		/// <summary>
		/// Adds hierarchy-listener-a with hierarchy-listener-b and
		/// returns the resulting multicast listener. </summary>
		/// <param name="a"> hierarchy-listener-a </param>
		/// <param name="b"> hierarchy-listener-b
		/// @since 1.3 </param>
		 public static HierarchyListener Add(HierarchyListener a, HierarchyListener b)
		 {
			return (HierarchyListener)AddInternal(a, b);
		 }

		/// <summary>
		/// Adds hierarchy-bounds-listener-a with hierarchy-bounds-listener-b and
		/// returns the resulting multicast listener. </summary>
		/// <param name="a"> hierarchy-bounds-listener-a </param>
		/// <param name="b"> hierarchy-bounds-listener-b
		/// @since 1.3 </param>
		 public static HierarchyBoundsListener Add(HierarchyBoundsListener a, HierarchyBoundsListener b)
		 {
			return (HierarchyBoundsListener)AddInternal(a, b);
		 }

		/// <summary>
		/// Adds mouse-wheel-listener-a with mouse-wheel-listener-b and
		/// returns the resulting multicast listener. </summary>
		/// <param name="a"> mouse-wheel-listener-a </param>
		/// <param name="b"> mouse-wheel-listener-b
		/// @since 1.4 </param>
		public static MouseWheelListener Add(MouseWheelListener a, MouseWheelListener b)
		{
			return (MouseWheelListener)AddInternal(a, b);
		}

		/// <summary>
		/// Removes the old component-listener from component-listener-l and
		/// returns the resulting multicast listener. </summary>
		/// <param name="l"> component-listener-l </param>
		/// <param name="oldl"> the component-listener being removed </param>
		public static ComponentListener Remove(ComponentListener l, ComponentListener oldl)
		{
			return (ComponentListener) RemoveInternal(l, oldl);
		}

		/// <summary>
		/// Removes the old container-listener from container-listener-l and
		/// returns the resulting multicast listener. </summary>
		/// <param name="l"> container-listener-l </param>
		/// <param name="oldl"> the container-listener being removed </param>
		public static ContainerListener Remove(ContainerListener l, ContainerListener oldl)
		{
			return (ContainerListener) RemoveInternal(l, oldl);
		}

		/// <summary>
		/// Removes the old focus-listener from focus-listener-l and
		/// returns the resulting multicast listener. </summary>
		/// <param name="l"> focus-listener-l </param>
		/// <param name="oldl"> the focus-listener being removed </param>
		public static FocusListener Remove(FocusListener l, FocusListener oldl)
		{
			return (FocusListener) RemoveInternal(l, oldl);
		}

		/// <summary>
		/// Removes the old key-listener from key-listener-l and
		/// returns the resulting multicast listener. </summary>
		/// <param name="l"> key-listener-l </param>
		/// <param name="oldl"> the key-listener being removed </param>
		public static KeyListener Remove(KeyListener l, KeyListener oldl)
		{
			return (KeyListener) RemoveInternal(l, oldl);
		}

		/// <summary>
		/// Removes the old mouse-listener from mouse-listener-l and
		/// returns the resulting multicast listener. </summary>
		/// <param name="l"> mouse-listener-l </param>
		/// <param name="oldl"> the mouse-listener being removed </param>
		public static MouseListener Remove(MouseListener l, MouseListener oldl)
		{
			return (MouseListener) RemoveInternal(l, oldl);
		}

		/// <summary>
		/// Removes the old mouse-motion-listener from mouse-motion-listener-l
		/// and returns the resulting multicast listener. </summary>
		/// <param name="l"> mouse-motion-listener-l </param>
		/// <param name="oldl"> the mouse-motion-listener being removed </param>
		public static MouseMotionListener Remove(MouseMotionListener l, MouseMotionListener oldl)
		{
			return (MouseMotionListener) RemoveInternal(l, oldl);
		}

		/// <summary>
		/// Removes the old window-listener from window-listener-l and
		/// returns the resulting multicast listener. </summary>
		/// <param name="l"> window-listener-l </param>
		/// <param name="oldl"> the window-listener being removed </param>
		public static WindowListener Remove(WindowListener l, WindowListener oldl)
		{
			return (WindowListener) RemoveInternal(l, oldl);
		}

		/// <summary>
		/// Removes the old window-state-listener from window-state-listener-l
		/// and returns the resulting multicast listener. </summary>
		/// <param name="l"> window-state-listener-l </param>
		/// <param name="oldl"> the window-state-listener being removed
		/// @since 1.4 </param>
		public static WindowStateListener Remove(WindowStateListener l, WindowStateListener oldl)
		{
			return (WindowStateListener) RemoveInternal(l, oldl);
		}

		/// <summary>
		/// Removes the old window-focus-listener from window-focus-listener-l
		/// and returns the resulting multicast listener. </summary>
		/// <param name="l"> window-focus-listener-l </param>
		/// <param name="oldl"> the window-focus-listener being removed
		/// @since 1.4 </param>
		public static WindowFocusListener Remove(WindowFocusListener l, WindowFocusListener oldl)
		{
			return (WindowFocusListener) RemoveInternal(l, oldl);
		}

		/// <summary>
		/// Removes the old action-listener from action-listener-l and
		/// returns the resulting multicast listener. </summary>
		/// <param name="l"> action-listener-l </param>
		/// <param name="oldl"> the action-listener being removed </param>
		public static ActionListener Remove(ActionListener l, ActionListener oldl)
		{
			return (ActionListener) RemoveInternal(l, oldl);
		}

		/// <summary>
		/// Removes the old item-listener from item-listener-l and
		/// returns the resulting multicast listener. </summary>
		/// <param name="l"> item-listener-l </param>
		/// <param name="oldl"> the item-listener being removed </param>
		public static ItemListener Remove(ItemListener l, ItemListener oldl)
		{
			return (ItemListener) RemoveInternal(l, oldl);
		}

		/// <summary>
		/// Removes the old adjustment-listener from adjustment-listener-l and
		/// returns the resulting multicast listener. </summary>
		/// <param name="l"> adjustment-listener-l </param>
		/// <param name="oldl"> the adjustment-listener being removed </param>
		public static AdjustmentListener Remove(AdjustmentListener l, AdjustmentListener oldl)
		{
			return (AdjustmentListener) RemoveInternal(l, oldl);
		}
		public static TextListener Remove(TextListener l, TextListener oldl)
		{
			return (TextListener) RemoveInternal(l, oldl);
		}

		/// <summary>
		/// Removes the old input-method-listener from input-method-listener-l and
		/// returns the resulting multicast listener. </summary>
		/// <param name="l"> input-method-listener-l </param>
		/// <param name="oldl"> the input-method-listener being removed </param>
		public static InputMethodListener Remove(InputMethodListener l, InputMethodListener oldl)
		{
			return (InputMethodListener) RemoveInternal(l, oldl);
		}

		/// <summary>
		/// Removes the old hierarchy-listener from hierarchy-listener-l and
		/// returns the resulting multicast listener. </summary>
		/// <param name="l"> hierarchy-listener-l </param>
		/// <param name="oldl"> the hierarchy-listener being removed
		/// @since 1.3 </param>
		public static HierarchyListener Remove(HierarchyListener l, HierarchyListener oldl)
		{
			return (HierarchyListener) RemoveInternal(l, oldl);
		}

		/// <summary>
		/// Removes the old hierarchy-bounds-listener from
		/// hierarchy-bounds-listener-l and returns the resulting multicast
		/// listener. </summary>
		/// <param name="l"> hierarchy-bounds-listener-l </param>
		/// <param name="oldl"> the hierarchy-bounds-listener being removed
		/// @since 1.3 </param>
		public static HierarchyBoundsListener Remove(HierarchyBoundsListener l, HierarchyBoundsListener oldl)
		{
			return (HierarchyBoundsListener) RemoveInternal(l, oldl);
		}

		/// <summary>
		/// Removes the old mouse-wheel-listener from mouse-wheel-listener-l
		/// and returns the resulting multicast listener. </summary>
		/// <param name="l"> mouse-wheel-listener-l </param>
		/// <param name="oldl"> the mouse-wheel-listener being removed
		/// @since 1.4 </param>
		public static MouseWheelListener Remove(MouseWheelListener l, MouseWheelListener oldl)
		{
		  return (MouseWheelListener) RemoveInternal(l, oldl);
		}

		/// <summary>
		/// Returns the resulting multicast listener from adding listener-a
		/// and listener-b together.
		/// If listener-a is null, it returns listener-b;
		/// If listener-b is null, it returns listener-a
		/// If neither are null, then it creates and returns
		/// a new AWTEventMulticaster instance which chains a with b. </summary>
		/// <param name="a"> event listener-a </param>
		/// <param name="b"> event listener-b </param>
		protected internal static EventListener AddInternal(EventListener a, EventListener b)
		{
			if (a == null)
			{
				return b;
			}
			if (b == null)
			{
				return a;
			}
			return new AWTEventMulticaster(a, b);
		}

		/// <summary>
		/// Returns the resulting multicast listener after removing the
		/// old listener from listener-l.
		/// If listener-l equals the old listener OR listener-l is null,
		/// returns null.
		/// Else if listener-l is an instance of AWTEventMulticaster,
		/// then it removes the old listener from it.
		/// Else, returns listener l. </summary>
		/// <param name="l"> the listener being removed from </param>
		/// <param name="oldl"> the listener being removed </param>
		protected internal static EventListener RemoveInternal(EventListener l, EventListener oldl)
		{
			if (l == oldl || l == null)
			{
				return null;
			}
			else if (l is AWTEventMulticaster)
			{
				return ((AWTEventMulticaster)l).Remove(oldl);
			}
			else
			{
				return l; // it's not here
			}
		}


		/* Serialization support.
		 */

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: protected void saveInternal(java.io.ObjectOutputStream s, String k) throws java.io.IOException
		protected internal virtual void SaveInternal(ObjectOutputStream s, String k)
		{
			if (a is AWTEventMulticaster)
			{
				((AWTEventMulticaster)a).SaveInternal(s, k);
			}
			else if (a is Serializable)
			{
				s.WriteObject(k);
				s.WriteObject(a);
			}

			if (b is AWTEventMulticaster)
			{
				((AWTEventMulticaster)b).SaveInternal(s, k);
			}
			else if (b is Serializable)
			{
				s.WriteObject(k);
				s.WriteObject(b);
			}
		}

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: protected static void save(java.io.ObjectOutputStream s, String k, java.util.EventListener l) throws java.io.IOException
		protected internal static void Save(ObjectOutputStream s, String k, EventListener l)
		{
		  if (l == null)
		  {
			  return;
		  }
		  else if (l is AWTEventMulticaster)
		  {
			  ((AWTEventMulticaster)l).SaveInternal(s, k);
		  }
		  else if (l is Serializable)
		  {
			   s.WriteObject(k);
			   s.WriteObject(l);
		  }
		}

		/*
		 * Recursive method which returns a count of the number of listeners in
		 * EventListener, handling the (common) case of l actually being an
		 * AWTEventMulticaster.  Additionally, only listeners of type listenerType
		 * are counted.  Method modified to fix bug 4513402.  -bchristi
		 */
		private static int GetListenerCount(EventListener l, Class listenerType)
		{
			if (l is AWTEventMulticaster)
			{
				AWTEventMulticaster mc = (AWTEventMulticaster)l;
				return GetListenerCount(mc.a, listenerType) + GetListenerCount(mc.b, listenerType);
			}
			else
			{
				// Only count listeners of correct type
				return listenerType.isInstance(l) ? 1 : 0;
			}
		}

		/*
		 * Recusive method which populates EventListener array a with EventListeners
		 * from l.  l is usually an AWTEventMulticaster.  Bug 4513402 revealed that
		 * if l differed in type from the element type of a, an ArrayStoreException
		 * would occur.  Now l is only inserted into a if it's of the appropriate
		 * type.  -bchristi
		 */
		private static int PopulateListenerArray(EventListener[] a, EventListener l, int index)
		{
			if (l is AWTEventMulticaster)
			{
				AWTEventMulticaster mc = (AWTEventMulticaster)l;
				int lhs = PopulateListenerArray(a, mc.a, index);
				return PopulateListenerArray(a, mc.b, lhs);
			}
			else if (a.GetType().GetElementType().isInstance(l))
			{
				a[index] = l;
				return index + 1;
			}
			// Skip nulls, instances of wrong class
			else
			{
				return index;
			}
		}

		/// <summary>
		/// Returns an array of all the objects chained as
		/// <code><em>Foo</em>Listener</code>s by the specified
		/// <code>java.util.EventListener</code>.
		/// <code><em>Foo</em>Listener</code>s are chained by the
		/// <code>AWTEventMulticaster</code> using the
		/// <code>add<em>Foo</em>Listener</code> method.
		/// If a <code>null</code> listener is specified, this method returns an
		/// empty array. If the specified listener is not an instance of
		/// <code>AWTEventMulticaster</code>, this method returns an array which
		/// contains only the specified listener. If no such listeners are chained,
		/// this method returns an empty array.
		/// </summary>
		/// <param name="l"> the specified <code>java.util.EventListener</code> </param>
		/// <param name="listenerType"> the type of listeners requested; this parameter
		///          should specify an interface that descends from
		///          <code>java.util.EventListener</code> </param>
		/// <returns> an array of all objects chained as
		///          <code><em>Foo</em>Listener</code>s by the specified multicast
		///          listener, or an empty array if no such listeners have been
		///          chained by the specified multicast listener </returns>
		/// <exception cref="NullPointerException"> if the specified
		///             {@code listenertype} parameter is {@code null} </exception>
		/// <exception cref="ClassCastException"> if <code>listenerType</code>
		///          doesn't specify a class or interface that implements
		///          <code>java.util.EventListener</code>
		/// 
		/// @since 1.4 </exception>
//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @SuppressWarnings("unchecked") public static <T extends java.util.EventListener> T[] getListeners(java.util.EventListener l, Class listenerType)
		public static T[] getListeners<T>(EventListener l, Class listenerType) where T : java.util.EventListener
		{
			if (listenerType == null)
			{
				throw new NullPointerException("Listener type should not be null");
			}

			int n = GetListenerCount(l, listenerType);
			T[] result = (T[])Array.newInstance(listenerType, n);
			PopulateListenerArray(result, l, 0);
			return result;
		}
	}

}
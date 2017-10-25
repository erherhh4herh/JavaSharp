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
	/// <b>NOTE:</b> The <code>Event</code> class is obsolete and is
	/// available only for backwards compatibility.  It has been replaced
	/// by the <code>AWTEvent</code> class and its subclasses.
	/// <para>
	/// <code>Event</code> is a platform-independent class that
	/// encapsulates events from the platform's Graphical User
	/// Interface in the Java&nbsp;1.0 event model. In Java&nbsp;1.1
	/// and later versions, the <code>Event</code> class is maintained
	/// only for backwards compatibility. The information in this
	/// class description is provided to assist programmers in
	/// converting Java&nbsp;1.0 programs to the new event model.
	/// </para>
	/// <para>
	/// In the Java&nbsp;1.0 event model, an event contains an
	/// <seealso cref="Event#id"/> field
	/// that indicates what type of event it is and which other
	/// <code>Event</code> variables are relevant for the event.
	/// </para>
	/// <para>
	/// For keyboard events, <seealso cref="Event#key"/>
	/// contains a value indicating which key was activated, and
	/// <seealso cref="Event#modifiers"/> contains the
	/// modifiers for that event.  For the KEY_PRESS and KEY_RELEASE
	/// event ids, the value of <code>key</code> is the unicode
	/// character code for the key. For KEY_ACTION and
	/// KEY_ACTION_RELEASE, the value of <code>key</code> is
	/// one of the defined action-key identifiers in the
	/// <code>Event</code> class (<code>PGUP</code>,
	/// <code>PGDN</code>, <code>F1</code>, <code>F2</code>, etc).
	/// 
	/// @author     Sami Shaio
	/// @since      JDK1.0
	/// </para>
	/// </summary>
	[Serializable]
	public class Event
	{
		[NonSerialized]
		private long Data;

		/* Modifier constants */

		/// <summary>
		/// This flag indicates that the Shift key was down when the event
		/// occurred.
		/// </summary>
		public static readonly int SHIFT_MASK = 1 << 0;

		/// <summary>
		/// This flag indicates that the Control key was down when the event
		/// occurred.
		/// </summary>
		public static readonly int CTRL_MASK = 1 << 1;

		/// <summary>
		/// This flag indicates that the Meta key was down when the event
		/// occurred. For mouse events, this flag indicates that the right
		/// button was pressed or released.
		/// </summary>
		public static readonly int META_MASK = 1 << 2;

		/// <summary>
		/// This flag indicates that the Alt key was down when
		/// the event occurred. For mouse events, this flag indicates that the
		/// middle mouse button was pressed or released.
		/// </summary>
		public static readonly int ALT_MASK = 1 << 3;

		/* Action keys */

		/// <summary>
		/// The Home key, a non-ASCII action key.
		/// </summary>
		public const int HOME = 1000;

		/// <summary>
		/// The End key, a non-ASCII action key.
		/// </summary>
		public const int END = 1001;

		/// <summary>
		/// The Page Up key, a non-ASCII action key.
		/// </summary>
		public const int PGUP = 1002;

		/// <summary>
		/// The Page Down key, a non-ASCII action key.
		/// </summary>
		public const int PGDN = 1003;

		/// <summary>
		/// The Up Arrow key, a non-ASCII action key.
		/// </summary>
		public const int UP = 1004;

		/// <summary>
		/// The Down Arrow key, a non-ASCII action key.
		/// </summary>
		public const int DOWN = 1005;

		/// <summary>
		/// The Left Arrow key, a non-ASCII action key.
		/// </summary>
		public const int LEFT = 1006;

		/// <summary>
		/// The Right Arrow key, a non-ASCII action key.
		/// </summary>
		public const int RIGHT = 1007;

		/// <summary>
		/// The F1 function key, a non-ASCII action key.
		/// </summary>
		public const int F1 = 1008;

		/// <summary>
		/// The F2 function key, a non-ASCII action key.
		/// </summary>
		public const int F2 = 1009;

		/// <summary>
		/// The F3 function key, a non-ASCII action key.
		/// </summary>
		public const int F3 = 1010;

		/// <summary>
		/// The F4 function key, a non-ASCII action key.
		/// </summary>
		public const int F4 = 1011;

		/// <summary>
		/// The F5 function key, a non-ASCII action key.
		/// </summary>
		public const int F5 = 1012;

		/// <summary>
		/// The F6 function key, a non-ASCII action key.
		/// </summary>
		public const int F6 = 1013;

		/// <summary>
		/// The F7 function key, a non-ASCII action key.
		/// </summary>
		public const int F7 = 1014;

		/// <summary>
		/// The F8 function key, a non-ASCII action key.
		/// </summary>
		public const int F8 = 1015;

		/// <summary>
		/// The F9 function key, a non-ASCII action key.
		/// </summary>
		public const int F9 = 1016;

		/// <summary>
		/// The F10 function key, a non-ASCII action key.
		/// </summary>
		public const int F10 = 1017;

		/// <summary>
		/// The F11 function key, a non-ASCII action key.
		/// </summary>
		public const int F11 = 1018;

		/// <summary>
		/// The F12 function key, a non-ASCII action key.
		/// </summary>
		public const int F12 = 1019;

		/// <summary>
		/// The Print Screen key, a non-ASCII action key.
		/// </summary>
		public const int PRINT_SCREEN = 1020;

		/// <summary>
		/// The Scroll Lock key, a non-ASCII action key.
		/// </summary>
		public const int SCROLL_LOCK = 1021;

		/// <summary>
		/// The Caps Lock key, a non-ASCII action key.
		/// </summary>
		public const int CAPS_LOCK = 1022;

		/// <summary>
		/// The Num Lock key, a non-ASCII action key.
		/// </summary>
		public const int NUM_LOCK = 1023;

		/// <summary>
		/// The Pause key, a non-ASCII action key.
		/// </summary>
		public const int PAUSE = 1024;

		/// <summary>
		/// The Insert key, a non-ASCII action key.
		/// </summary>
		public const int INSERT = 1025;

		/* Non-action keys */

		/// <summary>
		/// The Enter key.
		/// </summary>
		public const int ENTER = '\n';

		/// <summary>
		/// The BackSpace key.
		/// </summary>
		public const int BACK_SPACE = '\b';

		/// <summary>
		/// The Tab key.
		/// </summary>
		public const int TAB = '\t';

		/// <summary>
		/// The Escape key.
		/// </summary>
		public const int ESCAPE = 27;

		/// <summary>
		/// The Delete key.
		/// </summary>
		public const int DELETE = 127;


		/* Base for all window events. */
		private const int WINDOW_EVENT = 200;

		/// <summary>
		/// The user has asked the window manager to kill the window.
		/// </summary>
		public static readonly int WINDOW_DESTROY = 1 + WINDOW_EVENT;

		/// <summary>
		/// The user has asked the window manager to expose the window.
		/// </summary>
		public static readonly int WINDOW_EXPOSE = 2 + WINDOW_EVENT;

		/// <summary>
		/// The user has asked the window manager to iconify the window.
		/// </summary>
		public static readonly int WINDOW_ICONIFY = 3 + WINDOW_EVENT;

		/// <summary>
		/// The user has asked the window manager to de-iconify the window.
		/// </summary>
		public static readonly int WINDOW_DEICONIFY = 4 + WINDOW_EVENT;

		/// <summary>
		/// The user has asked the window manager to move the window.
		/// </summary>
		public static readonly int WINDOW_MOVED = 5 + WINDOW_EVENT;

		/* Base for all keyboard events. */
		private const int KEY_EVENT = 400;

		/// <summary>
		/// The user has pressed a normal key.
		/// </summary>
		public static readonly int KEY_PRESS = 1 + KEY_EVENT;

		/// <summary>
		/// The user has released a normal key.
		/// </summary>
		public static readonly int KEY_RELEASE = 2 + KEY_EVENT;

		/// <summary>
		/// The user has pressed a non-ASCII <em>action</em> key.
		/// The <code>key</code> field contains a value that indicates
		/// that the event occurred on one of the action keys, which
		/// comprise the 12 function keys, the arrow (cursor) keys,
		/// Page Up, Page Down, Home, End, Print Screen, Scroll Lock,
		/// Caps Lock, Num Lock, Pause, and Insert.
		/// </summary>
		public static readonly int KEY_ACTION = 3 + KEY_EVENT;

		/// <summary>
		/// The user has released a non-ASCII <em>action</em> key.
		/// The <code>key</code> field contains a value that indicates
		/// that the event occurred on one of the action keys, which
		/// comprise the 12 function keys, the arrow (cursor) keys,
		/// Page Up, Page Down, Home, End, Print Screen, Scroll Lock,
		/// Caps Lock, Num Lock, Pause, and Insert.
		/// </summary>
		public static readonly int KEY_ACTION_RELEASE = 4 + KEY_EVENT;

		/* Base for all mouse events. */
		private const int MOUSE_EVENT = 500;

		/// <summary>
		/// The user has pressed the mouse button. The <code>ALT_MASK</code>
		/// flag indicates that the middle button has been pressed.
		/// The <code>META_MASK</code>flag indicates that the
		/// right button has been pressed. </summary>
		/// <seealso cref=     java.awt.Event#ALT_MASK </seealso>
		/// <seealso cref=     java.awt.Event#META_MASK </seealso>
		public static readonly int MOUSE_DOWN = 1 + MOUSE_EVENT;

		/// <summary>
		/// The user has released the mouse button. The <code>ALT_MASK</code>
		/// flag indicates that the middle button has been released.
		/// The <code>META_MASK</code>flag indicates that the
		/// right button has been released. </summary>
		/// <seealso cref=     java.awt.Event#ALT_MASK </seealso>
		/// <seealso cref=     java.awt.Event#META_MASK </seealso>
		public static readonly int MOUSE_UP = 2 + MOUSE_EVENT;

		/// <summary>
		/// The mouse has moved with no button pressed.
		/// </summary>
		public static readonly int MOUSE_MOVE = 3 + MOUSE_EVENT;

		/// <summary>
		/// The mouse has entered a component.
		/// </summary>
		public static readonly int MOUSE_ENTER = 4 + MOUSE_EVENT;

		/// <summary>
		/// The mouse has exited a component.
		/// </summary>
		public static readonly int MOUSE_EXIT = 5 + MOUSE_EVENT;

		/// <summary>
		/// The user has moved the mouse with a button pressed. The
		/// <code>ALT_MASK</code> flag indicates that the middle
		/// button is being pressed. The <code>META_MASK</code> flag indicates
		/// that the right button is being pressed. </summary>
		/// <seealso cref=     java.awt.Event#ALT_MASK </seealso>
		/// <seealso cref=     java.awt.Event#META_MASK </seealso>
		public static readonly int MOUSE_DRAG = 6 + MOUSE_EVENT;


		/* Scrolling events */
		private const int SCROLL_EVENT = 600;

		/// <summary>
		/// The user has activated the <em>line up</em>
		/// area of a scroll bar.
		/// </summary>
		public static readonly int SCROLL_LINE_UP = 1 + SCROLL_EVENT;

		/// <summary>
		/// The user has activated the <em>line down</em>
		/// area of a scroll bar.
		/// </summary>
		public static readonly int SCROLL_LINE_DOWN = 2 + SCROLL_EVENT;

		/// <summary>
		/// The user has activated the <em>page up</em>
		/// area of a scroll bar.
		/// </summary>
		public static readonly int SCROLL_PAGE_UP = 3 + SCROLL_EVENT;

		/// <summary>
		/// The user has activated the <em>page down</em>
		/// area of a scroll bar.
		/// </summary>
		public static readonly int SCROLL_PAGE_DOWN = 4 + SCROLL_EVENT;

		/// <summary>
		/// The user has moved the bubble (thumb) in a scroll bar,
		/// moving to an "absolute" position, rather than to
		/// an offset from the last position.
		/// </summary>
		public static readonly int SCROLL_ABSOLUTE = 5 + SCROLL_EVENT;

		/// <summary>
		/// The scroll begin event.
		/// </summary>
		public static readonly int SCROLL_BEGIN = 6 + SCROLL_EVENT;

		/// <summary>
		/// The scroll end event.
		/// </summary>
		public static readonly int SCROLL_END = 7 + SCROLL_EVENT;

		/* List Events */
		private const int LIST_EVENT = 700;

		/// <summary>
		/// An item in a list has been selected.
		/// </summary>
		public static readonly int LIST_SELECT = 1 + LIST_EVENT;

		/// <summary>
		/// An item in a list has been deselected.
		/// </summary>
		public static readonly int LIST_DESELECT = 2 + LIST_EVENT;

		/* Misc Event */
		private const int MISC_EVENT = 1000;

		/// <summary>
		/// This event indicates that the user wants some action to occur.
		/// </summary>
		public static readonly int ACTION_EVENT = 1 + MISC_EVENT;

		/// <summary>
		/// A file loading event.
		/// </summary>
		public static readonly int LOAD_FILE = 2 + MISC_EVENT;

		/// <summary>
		/// A file saving event.
		/// </summary>
		public static readonly int SAVE_FILE = 3 + MISC_EVENT;

		/// <summary>
		/// A component gained the focus.
		/// </summary>
		public static readonly int GOT_FOCUS = 4 + MISC_EVENT;

		/// <summary>
		/// A component lost the focus.
		/// </summary>
		public static readonly int LOST_FOCUS = 5 + MISC_EVENT;

		/// <summary>
		/// The target component. This indicates the component over which the
		/// event occurred or with which the event is associated.
		/// This object has been replaced by AWTEvent.getSource()
		/// 
		/// @serial </summary>
		/// <seealso cref= java.awt.AWTEvent#getSource() </seealso>
		public Object Target;

		/// <summary>
		/// The time stamp.
		/// Replaced by InputEvent.getWhen().
		/// 
		/// @serial </summary>
		/// <seealso cref= java.awt.event.InputEvent#getWhen() </seealso>
		public long When;

		/// <summary>
		/// Indicates which type of event the event is, and which
		/// other <code>Event</code> variables are relevant for the event.
		/// This has been replaced by AWTEvent.getID()
		/// 
		/// @serial </summary>
		/// <seealso cref= java.awt.AWTEvent#getID() </seealso>
		public int Id;

		/// <summary>
		/// The <i>x</i> coordinate of the event.
		/// Replaced by MouseEvent.getX()
		/// 
		/// @serial </summary>
		/// <seealso cref= java.awt.event.MouseEvent#getX() </seealso>
		public int x;

		/// <summary>
		/// The <i>y</i> coordinate of the event.
		/// Replaced by MouseEvent.getY()
		/// 
		/// @serial </summary>
		/// <seealso cref= java.awt.event.MouseEvent#getY() </seealso>
		public int y;

		/// <summary>
		/// The key code of the key that was pressed in a keyboard event.
		/// This has been replaced by KeyEvent.getKeyCode()
		/// 
		/// @serial </summary>
		/// <seealso cref= java.awt.event.KeyEvent#getKeyCode() </seealso>
		public int Key;

		/// <summary>
		/// The key character that was pressed in a keyboard event.
		/// </summary>
	//    public char keyChar;

		/// <summary>
		/// The state of the modifier keys.
		/// This is replaced with InputEvent.getModifiers()
		/// In java 1.1 MouseEvent and KeyEvent are subclasses
		/// of InputEvent.
		/// 
		/// @serial </summary>
		/// <seealso cref= java.awt.event.InputEvent#getModifiers() </seealso>
		public int Modifiers;

		/// <summary>
		/// For <code>MOUSE_DOWN</code> events, this field indicates the
		/// number of consecutive clicks. For other events, its value is
		/// <code>0</code>.
		/// This field has been replaced by MouseEvent.getClickCount().
		/// 
		/// @serial </summary>
		/// <seealso cref= java.awt.event.MouseEvent#getClickCount() </seealso>
		public int ClickCount;

		/// <summary>
		/// An arbitrary argument of the event. The value of this field
		/// depends on the type of event.
		/// <code>arg</code> has been replaced by event specific property.
		/// 
		/// @serial
		/// </summary>
		public Object Arg;

		/// <summary>
		/// The next event. This field is set when putting events into a
		/// linked list.
		/// This has been replaced by EventQueue.
		/// 
		/// @serial </summary>
		/// <seealso cref= java.awt.EventQueue </seealso>
		public Event Evt;

		/* table for mapping old Event action keys to KeyEvent virtual keys. */
		private static readonly int[][] ActionKeyCodes = new int[][] {new int[] {KeyEvent.VK_HOME, Event.HOME}, new int[] {KeyEvent.VK_END, Event.END}, new int[] {KeyEvent.VK_PAGE_UP, Event.PGUP}, new int[] {KeyEvent.VK_PAGE_DOWN, Event.PGDN}, new int[] {KeyEvent.VK_UP, Event.UP}, new int[] {KeyEvent.VK_DOWN, Event.DOWN}, new int[] {KeyEvent.VK_LEFT, Event.LEFT}, new int[] {KeyEvent.VK_RIGHT, Event.RIGHT}, new int[] {KeyEvent.VK_F1, Event.F1}, new int[] {KeyEvent.VK_F2, Event.F2}, new int[] {KeyEvent.VK_F3, Event.F3}, new int[] {KeyEvent.VK_F4, Event.F4}, new int[] {KeyEvent.VK_F5, Event.F5}, new int[] {KeyEvent.VK_F6, Event.F6}, new int[] {KeyEvent.VK_F7, Event.F7}, new int[] {KeyEvent.VK_F8, Event.F8}, new int[] {KeyEvent.VK_F9, Event.F9}, new int[] {KeyEvent.VK_F10, Event.F10}, new int[] {KeyEvent.VK_F11, Event.F11}, new int[] {KeyEvent.VK_F12, Event.F12}, new int[] {KeyEvent.VK_PRINTSCREEN, Event.PRINT_SCREEN}, new int[] {KeyEvent.VK_SCROLL_LOCK, Event.SCROLL_LOCK}, new int[] {KeyEvent.VK_CAPS_LOCK, Event.CAPS_LOCK}, new int[] {KeyEvent.VK_NUM_LOCK, Event.NUM_LOCK}, new int[] {KeyEvent.VK_PAUSE, Event.PAUSE}, new int[] {KeyEvent.VK_INSERT, Event.INSERT}};

		/// <summary>
		/// This field controls whether or not the event is sent back
		/// down to the peer once the target has processed it -
		/// false means it's sent to the peer, true means it's not.
		/// 
		/// @serial </summary>
		/// <seealso cref= #isConsumed() </seealso>
		private bool Consumed_Renamed = false;

		/*
		 * JDK 1.1 serialVersionUID
		 */
		private const long SerialVersionUID = 5488922509400504703L;

		static Event()
		{
			/* ensure that the necessary native libraries are loaded */
			Toolkit.LoadLibraries();
			if (!GraphicsEnvironment.Headless)
			{
				initIDs();
			}
		}

		/// <summary>
		/// Initialize JNI field and method IDs for fields that may be
		///   accessed from C.
		/// </summary>
//JAVA TO C# CONVERTER TODO TASK: Replace 'unknown' with the appropriate dll name:
		[DllImport("unknown")]
		private static extern void initIDs();

		/// <summary>
		/// <b>NOTE:</b> The <code>Event</code> class is obsolete and is
		/// available only for backwards compatibility.  It has been replaced
		/// by the <code>AWTEvent</code> class and its subclasses.
		/// <para>
		/// Creates an instance of <code>Event</code> with the specified target
		/// component, time stamp, event type, <i>x</i> and <i>y</i>
		/// coordinates, keyboard key, state of the modifier keys, and
		/// argument.
		/// </para>
		/// </summary>
		/// <param name="target">     the target component. </param>
		/// <param name="when">       the time stamp. </param>
		/// <param name="id">         the event type. </param>
		/// <param name="x">          the <i>x</i> coordinate. </param>
		/// <param name="y">          the <i>y</i> coordinate. </param>
		/// <param name="key">        the key pressed in a keyboard event. </param>
		/// <param name="modifiers">  the state of the modifier keys. </param>
		/// <param name="arg">        the specified argument. </param>
		public Event(Object target, long when, int id, int x, int y, int key, int modifiers, Object arg)
		{
			this.Target = target;
			this.When = when;
			this.Id = id;
			this.x = x;
			this.y = y;
			this.Key = key;
			this.Modifiers = modifiers;
			this.Arg = arg;
			this.Data = 0;
			this.ClickCount = 0;
			switch (id)
			{
			  case ACTION_EVENT:
			  case WINDOW_DESTROY:
			  case WINDOW_ICONIFY:
			  case WINDOW_DEICONIFY:
			  case WINDOW_MOVED:
			  case SCROLL_LINE_UP:
			  case SCROLL_LINE_DOWN:
			  case SCROLL_PAGE_UP:
			  case SCROLL_PAGE_DOWN:
			  case SCROLL_ABSOLUTE:
			  case SCROLL_BEGIN:
			  case SCROLL_END:
			  case LIST_SELECT:
			  case LIST_DESELECT:
				Consumed_Renamed = true; // these types are not passed back to peer
				break;
			  default:
		  break;
			}
		}

		/// <summary>
		/// <b>NOTE:</b> The <code>Event</code> class is obsolete and is
		/// available only for backwards compatibility.  It has been replaced
		/// by the <code>AWTEvent</code> class and its subclasses.
		/// <para>
		/// Creates an instance of <code>Event</code>, with the specified target
		/// component, time stamp, event type, <i>x</i> and <i>y</i>
		/// coordinates, keyboard key, state of the modifier keys, and an
		/// argument set to <code>null</code>.
		/// </para>
		/// </summary>
		/// <param name="target">     the target component. </param>
		/// <param name="when">       the time stamp. </param>
		/// <param name="id">         the event type. </param>
		/// <param name="x">          the <i>x</i> coordinate. </param>
		/// <param name="y">          the <i>y</i> coordinate. </param>
		/// <param name="key">        the key pressed in a keyboard event. </param>
		/// <param name="modifiers">  the state of the modifier keys. </param>
		public Event(Object target, long when, int id, int x, int y, int key, int modifiers) : this(target, when, id, x, y, key, modifiers, null)
		{
		}

		/// <summary>
		/// <b>NOTE:</b> The <code>Event</code> class is obsolete and is
		/// available only for backwards compatibility.  It has been replaced
		/// by the <code>AWTEvent</code> class and its subclasses.
		/// <para>
		/// Creates an instance of <code>Event</code> with the specified
		/// target component, event type, and argument.
		/// </para>
		/// </summary>
		/// <param name="target">     the target component. </param>
		/// <param name="id">         the event type. </param>
		/// <param name="arg">        the specified argument. </param>
		public Event(Object target, int id, Object arg) : this(target, 0, id, 0, 0, 0, 0, arg)
		{
		}

		/// <summary>
		/// <b>NOTE:</b> The <code>Event</code> class is obsolete and is
		/// available only for backwards compatibility.  It has been replaced
		/// by the <code>AWTEvent</code> class and its subclasses.
		/// <para>
		/// Translates this event so that its <i>x</i> and <i>y</i>
		/// coordinates are increased by <i>dx</i> and <i>dy</i>,
		/// respectively.
		/// </para>
		/// <para>
		/// This method translates an event relative to the given component.
		/// This involves, at a minimum, translating the coordinates into the
		/// local coordinate system of the given component. It may also involve
		/// translating a region in the case of an expose event.
		/// </para>
		/// </summary>
		/// <param name="dx">     the distance to translate the <i>x</i> coordinate. </param>
		/// <param name="dy">     the distance to translate the <i>y</i> coordinate. </param>
		public virtual void Translate(int dx, int dy)
		{
			this.x += dx;
			this.y += dy;
		}

		/// <summary>
		/// <b>NOTE:</b> The <code>Event</code> class is obsolete and is
		/// available only for backwards compatibility.  It has been replaced
		/// by the <code>AWTEvent</code> class and its subclasses.
		/// <para>
		/// Checks if the Shift key is down.
		/// </para>
		/// </summary>
		/// <returns>    <code>true</code> if the key is down;
		///            <code>false</code> otherwise. </returns>
		/// <seealso cref=       java.awt.Event#modifiers </seealso>
		/// <seealso cref=       java.awt.Event#controlDown </seealso>
		/// <seealso cref=       java.awt.Event#metaDown </seealso>
		public virtual bool ShiftDown()
		{
			return (Modifiers & SHIFT_MASK) != 0;
		}

		/// <summary>
		/// <b>NOTE:</b> The <code>Event</code> class is obsolete and is
		/// available only for backwards compatibility.  It has been replaced
		/// by the <code>AWTEvent</code> class and its subclasses.
		/// <para>
		/// Checks if the Control key is down.
		/// </para>
		/// </summary>
		/// <returns>    <code>true</code> if the key is down;
		///            <code>false</code> otherwise. </returns>
		/// <seealso cref=       java.awt.Event#modifiers </seealso>
		/// <seealso cref=       java.awt.Event#shiftDown </seealso>
		/// <seealso cref=       java.awt.Event#metaDown </seealso>
		public virtual bool ControlDown()
		{
			return (Modifiers & CTRL_MASK) != 0;
		}

		/// <summary>
		/// <b>NOTE:</b> The <code>Event</code> class is obsolete and is
		/// available only for backwards compatibility.  It has been replaced
		/// by the <code>AWTEvent</code> class and its subclasses.
		/// <para>
		/// Checks if the Meta key is down.
		/// 
		/// </para>
		/// </summary>
		/// <returns>    <code>true</code> if the key is down;
		///            <code>false</code> otherwise. </returns>
		/// <seealso cref=       java.awt.Event#modifiers </seealso>
		/// <seealso cref=       java.awt.Event#shiftDown </seealso>
		/// <seealso cref=       java.awt.Event#controlDown </seealso>
		public virtual bool MetaDown()
		{
			return (Modifiers & META_MASK) != 0;
		}

		/// <summary>
		/// <b>NOTE:</b> The <code>Event</code> class is obsolete and is
		/// available only for backwards compatibility.  It has been replaced
		/// by the <code>AWTEvent</code> class and its subclasses.
		/// </summary>
		internal virtual void Consume()
		{
			switch (Id)
			{
			  case KEY_PRESS:
			  case KEY_RELEASE:
			  case KEY_ACTION:
			  case KEY_ACTION_RELEASE:
				  Consumed_Renamed = true;
				  break;
			  default:
				  // event type cannot be consumed
		  break;
			}
		}

		/// <summary>
		/// <b>NOTE:</b> The <code>Event</code> class is obsolete and is
		/// available only for backwards compatibility.  It has been replaced
		/// by the <code>AWTEvent</code> class and its subclasses.
		/// </summary>
		internal virtual bool Consumed
		{
			get
			{
				return Consumed_Renamed;
			}
		}

		/*
		 * <b>NOTE:</b> The <code>Event</code> class is obsolete and is
		 * available only for backwards compatibility.  It has been replaced
		 * by the <code>AWTEvent</code> class and its subclasses.
		 * <p>
		 * Returns the integer key-code associated with the key in this event,
		 * as described in java.awt.Event.
		 */
		internal static int GetOldEventKey(KeyEvent e)
		{
			int keyCode = e.KeyCode;
			for (int i = 0; i < ActionKeyCodes.Length; i++)
			{
				if (ActionKeyCodes[i][0] == keyCode)
				{
					return ActionKeyCodes[i][1];
				}
			}
			return (int)e.KeyChar;
		}

		/*
		 * <b>NOTE:</b> The <code>Event</code> class is obsolete and is
		 * available only for backwards compatibility.  It has been replaced
		 * by the <code>AWTEvent</code> class and its subclasses.
		 * <p>
		 * Returns a new KeyEvent char which corresponds to the int key
		 * of this old event.
		 */
		internal virtual char KeyEventChar
		{
			get
			{
			   for (int i = 0; i < ActionKeyCodes.Length; i++)
			   {
					if (ActionKeyCodes[i][1] == Key)
					{
						return KeyEvent.CHAR_UNDEFINED;
					}
			   }
			   return (char)Key;
			}
		}

		/// <summary>
		/// <b>NOTE:</b> The <code>Event</code> class is obsolete and is
		/// available only for backwards compatibility.  It has been replaced
		/// by the <code>AWTEvent</code> class and its subclasses.
		/// <para>
		/// Returns a string representing the state of this <code>Event</code>.
		/// This method is intended to be used only for debugging purposes, and the
		/// content and format of the returned string may vary between
		/// implementations. The returned string may be empty but may not be
		/// <code>null</code>.
		/// 
		/// </para>
		/// </summary>
		/// <returns>    the parameter string of this event </returns>
		protected internal virtual String ParamString()
		{
			String str = "id=" + Id + ",x=" + x + ",y=" + y;
			if (Key != 0)
			{
				str += ",key=" + Key;
			}
			if (ShiftDown())
			{
				str += ",shift";
			}
			if (ControlDown())
			{
				str += ",control";
			}
			if (MetaDown())
			{
				str += ",meta";
			}
			if (Target != null)
			{
				str += ",target=" + Target;
			}
			if (Arg != null)
			{
				str += ",arg=" + Arg;
			}
			return str;
		}

		/// <summary>
		/// <b>NOTE:</b> The <code>Event</code> class is obsolete and is
		/// available only for backwards compatibility.  It has been replaced
		/// by the <code>AWTEvent</code> class and its subclasses.
		/// <para>
		/// Returns a representation of this event's values as a string.
		/// </para>
		/// </summary>
		/// <returns>    a string that represents the event and the values
		///                 of its member fields. </returns>
		/// <seealso cref=       java.awt.Event#paramString
		/// @since     JDK1.1 </seealso>
		public override String ToString()
		{
//JAVA TO C# CONVERTER WARNING: The .NET Type.FullName property will not always yield results identical to the Java Class.getName method:
			return this.GetType().FullName + "[" + ParamString() + "]";
		}
	}

}
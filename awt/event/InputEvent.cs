using System;
using System.Runtime.InteropServices;

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

namespace java.awt.@event
{


	using AWTAccessor = sun.awt.AWTAccessor;
	using PlatformLogger = sun.util.logging.PlatformLogger;
	using SecurityConstants = sun.security.util.SecurityConstants;

	/// <summary>
	/// The root event class for all component-level input events.
	/// 
	/// Input events are delivered to listeners before they are
	/// processed normally by the source where they originated.
	/// This allows listeners and component subclasses to "consume"
	/// the event so that the source will not process them in their
	/// default manner.  For example, consuming mousePressed events
	/// on a Button component will prevent the Button from being
	/// activated.
	/// 
	/// @author Carl Quinn
	/// </summary>
	/// <seealso cref= KeyEvent </seealso>
	/// <seealso cref= KeyAdapter </seealso>
	/// <seealso cref= MouseEvent </seealso>
	/// <seealso cref= MouseAdapter </seealso>
	/// <seealso cref= MouseMotionAdapter
	/// 
	/// @since 1.1 </seealso>
	public abstract class InputEvent : ComponentEvent
	{

		private static readonly PlatformLogger Logger = PlatformLogger.getLogger("java.awt.event.InputEvent");

		/// <summary>
		/// The Shift key modifier constant.
		/// It is recommended that SHIFT_DOWN_MASK be used instead.
		/// </summary>
		public static readonly int SHIFT_MASK = Event.SHIFT_MASK;

		/// <summary>
		/// The Control key modifier constant.
		/// It is recommended that CTRL_DOWN_MASK be used instead.
		/// </summary>
		public static readonly int CTRL_MASK = Event.CTRL_MASK;

		/// <summary>
		/// The Meta key modifier constant.
		/// It is recommended that META_DOWN_MASK be used instead.
		/// </summary>
		public static readonly int META_MASK = Event.META_MASK;

		/// <summary>
		/// The Alt key modifier constant.
		/// It is recommended that ALT_DOWN_MASK be used instead.
		/// </summary>
		public static readonly int ALT_MASK = Event.ALT_MASK;

		/// <summary>
		/// The AltGraph key modifier constant.
		/// </summary>
		public static readonly int ALT_GRAPH_MASK = 1 << 5;

		/// <summary>
		/// The Mouse Button1 modifier constant.
		/// It is recommended that BUTTON1_DOWN_MASK be used instead.
		/// </summary>
		public static readonly int BUTTON1_MASK = 1 << 4;

		/// <summary>
		/// The Mouse Button2 modifier constant.
		/// It is recommended that BUTTON2_DOWN_MASK be used instead.
		/// Note that BUTTON2_MASK has the same value as ALT_MASK.
		/// </summary>
		public static readonly int BUTTON2_MASK = Event.ALT_MASK;

		/// <summary>
		/// The Mouse Button3 modifier constant.
		/// It is recommended that BUTTON3_DOWN_MASK be used instead.
		/// Note that BUTTON3_MASK has the same value as META_MASK.
		/// </summary>
		public static readonly int BUTTON3_MASK = Event.META_MASK;

		/// <summary>
		/// The Shift key extended modifier constant.
		/// @since 1.4
		/// </summary>
		public static readonly int SHIFT_DOWN_MASK = 1 << 6;

		/// <summary>
		/// The Control key extended modifier constant.
		/// @since 1.4
		/// </summary>
		public static readonly int CTRL_DOWN_MASK = 1 << 7;

		/// <summary>
		/// The Meta key extended modifier constant.
		/// @since 1.4
		/// </summary>
		public static readonly int META_DOWN_MASK = 1 << 8;

		/// <summary>
		/// The Alt key extended modifier constant.
		/// @since 1.4
		/// </summary>
		public static readonly int ALT_DOWN_MASK = 1 << 9;

		/// <summary>
		/// The Mouse Button1 extended modifier constant.
		/// @since 1.4
		/// </summary>
		public static readonly int BUTTON1_DOWN_MASK = 1 << 10;

		/// <summary>
		/// The Mouse Button2 extended modifier constant.
		/// @since 1.4
		/// </summary>
		public static readonly int BUTTON2_DOWN_MASK = 1 << 11;

		/// <summary>
		/// The Mouse Button3 extended modifier constant.
		/// @since 1.4
		/// </summary>
		public static readonly int BUTTON3_DOWN_MASK = 1 << 12;

		/// <summary>
		/// The AltGraph key extended modifier constant.
		/// @since 1.4
		/// </summary>
		public static readonly int ALT_GRAPH_DOWN_MASK = 1 << 13;

		/// <summary>
		/// An array of extended modifiers for additional buttons. </summary>
		/// <seealso cref= getButtonDownMasks
		/// There are twenty buttons fit into 4byte space.
		/// one more bit is reserved for FIRST_HIGH_BIT.
		/// @since 7.0 </seealso>
		private static readonly int[] BUTTON_DOWN_MASK = new int [] {BUTTON1_DOWN_MASK, BUTTON2_DOWN_MASK, BUTTON3_DOWN_MASK, 1 << 14, 1 << 15, 1 << 16, 1 << 17, 1 << 18, 1 << 19, 1 << 20, 1 << 21, 1 << 22, 1 << 23, 1 << 24, 1 << 25, 1 << 26, 1 << 27, 1 << 28, 1 << 29, 1 << 30};

		/// <summary>
		/// A method to access an array of extended modifiers for additional buttons.
		/// @since 7.0
		/// </summary>
		private static int [] ButtonDownMasks
		{
			get
			{
				return Arrays.CopyOf(BUTTON_DOWN_MASK, BUTTON_DOWN_MASK.Length);
			}
		}


		/// <summary>
		/// A method to obtain a mask for any existing mouse button.
		/// The returned mask may be used for different purposes. Following are some of them:
		/// <ul>
		/// <li> <seealso cref="java.awt.Robot#mousePress(int) mousePress(buttons)"/> and
		///      <seealso cref="java.awt.Robot#mouseRelease(int) mouseRelease(buttons)"/>
		/// <li> as a {@code modifiers} parameter when creating a new <seealso cref="MouseEvent"/> instance
		/// <li> to check <seealso cref="MouseEvent#getModifiersEx() modifiersEx"/> of existing {@code MouseEvent}
		/// </ul> </summary>
		/// <param name="button"> is a number to represent a button starting from 1.
		/// For example,
		/// <pre>
		/// int button = InputEvent.getMaskForButton(1);
		/// </pre>
		/// will have the same meaning as
		/// <pre>
		/// int button = InputEvent.getMaskForButton(MouseEvent.BUTTON1);
		/// </pre>
		/// because <seealso cref="MouseEvent#BUTTON1 MouseEvent.BUTTON1"/> equals to 1.
		/// If a mouse has three enabled buttons(see <seealso cref="java.awt.MouseInfo#getNumberOfButtons() MouseInfo.getNumberOfButtons()"/>)
		/// then the values from the left column passed into the method will return
		/// corresponding values from the right column:
		/// <PRE>
		///    <b>button </b>   <b>returned mask</b>
		///    <seealso cref="MouseEvent#BUTTON1 BUTTON1"/>  <seealso cref="MouseEvent#BUTTON1_DOWN_MASK BUTTON1_DOWN_MASK"/>
		///    <seealso cref="MouseEvent#BUTTON2 BUTTON2"/>  <seealso cref="MouseEvent#BUTTON2_DOWN_MASK BUTTON2_DOWN_MASK"/>
		///    <seealso cref="MouseEvent#BUTTON3 BUTTON3"/>  <seealso cref="MouseEvent#BUTTON3_DOWN_MASK BUTTON3_DOWN_MASK"/>
		/// </PRE>
		/// If a mouse has more than three enabled buttons then more values
		/// are admissible (4, 5, etc.). There is no assigned constants for these extended buttons.
		/// The button masks for the extra buttons returned by this method have no assigned names like the
		/// first three button masks.
		/// <para>
		/// This method has the following implementation restriction.
		/// It returns masks for a limited number of buttons only. The maximum number is
		/// implementation dependent and may vary.
		/// This limit is defined by the relevant number
		/// of buttons that may hypothetically exist on the mouse but it is greater than the
		/// <seealso cref="java.awt.MouseInfo#getNumberOfButtons() MouseInfo.getNumberOfButtons()"/>.
		/// </para>
		/// <para>
		/// </para>
		/// </param>
		/// <exception cref="IllegalArgumentException"> if {@code button} is less than zero or greater than the number
		///         of button masks reserved for buttons
		/// @since 7.0 </exception>
		/// <seealso cref= java.awt.MouseInfo#getNumberOfButtons() </seealso>
		/// <seealso cref= Toolkit#areExtraMouseButtonsEnabled() </seealso>
		/// <seealso cref= MouseEvent#getModifiers() </seealso>
		/// <seealso cref= MouseEvent#getModifiersEx() </seealso>
		public static int GetMaskForButton(int button)
		{
			if (button <= 0 || button > BUTTON_DOWN_MASK.Length)
			{
				throw new IllegalArgumentException("button doesn\'t exist " + button);
			}
			return BUTTON_DOWN_MASK[button - 1];
		}

		// the constant below MUST be updated if any extra modifier
		// bits are to be added!
		// in fact, it is undesirable to add modifier bits
		// to the same field as this may break applications
		// see bug# 5066958
		internal static readonly int FIRST_HIGH_BIT = 1 << 31;

		internal static readonly int JDK_1_3_MODIFIERS = SHIFT_DOWN_MASK - 1;
		internal static readonly int HIGH_MODIFIERS = ~(FIRST_HIGH_BIT - 1);

		/// <summary>
		/// The input event's Time stamp in UTC format.  The time stamp
		/// indicates when the input event was created.
		/// 
		/// @serial </summary>
		/// <seealso cref= #getWhen() </seealso>
		internal long When_Renamed;

		/// <summary>
		/// The state of the modifier mask at the time the input
		/// event was fired.
		/// 
		/// @serial </summary>
		/// <seealso cref= #getModifiers() </seealso>
		/// <seealso cref= #getModifiersEx() </seealso>
		/// <seealso cref= java.awt.event.KeyEvent </seealso>
		/// <seealso cref= java.awt.event.MouseEvent </seealso>
		internal int Modifiers_Renamed;

		/*
		 * A flag that indicates that this instance can be used to access
		 * the system clipboard.
		 */
		[NonSerialized]
		private bool CanAccessSystemClipboard_Renamed;

		static InputEvent()
		{
			/* ensure that the necessary native libraries are loaded */
			NativeLibLoader.LoadLibraries();
			if (!GraphicsEnvironment.Headless)
			{
				initIDs();
			}
			AWTAccessor.InputEventAccessor = new InputEventAccessorAnonymousInnerClassHelper();
		}

		private class InputEventAccessorAnonymousInnerClassHelper : AWTAccessor.InputEventAccessor
		{
			public InputEventAccessorAnonymousInnerClassHelper()
			{
			}

			public virtual int[] ButtonDownMasks
			{
				get
				{
					return InputEvent.ButtonDownMasks;
				}
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
		/// Constructs an InputEvent object with the specified source component,
		/// modifiers, and type.
		/// <para> This method throws an
		/// <code>IllegalArgumentException</code> if <code>source</code>
		/// is <code>null</code>.
		/// 
		/// </para>
		/// </summary>
		/// <param name="source"> the object where the event originated </param>
		/// <param name="id">           the integer that identifies the event type.
		///                     It is allowed to pass as parameter any value that
		///                     allowed for some subclass of {@code InputEvent} class.
		///                     Passing in the value different from those values result
		///                     in unspecified behavior </param>
		/// <param name="when">         a long int that gives the time the event occurred.
		///                     Passing negative or zero value
		///                     is not recommended </param>
		/// <param name="modifiers">    a modifier mask describing the modifier keys and mouse
		///                     buttons (for example, shift, ctrl, alt, and meta) that
		///                     are down during the event.
		///                     Only extended modifiers are allowed to be used as a
		///                     value for this parameter (see the <seealso cref="InputEvent#getModifiersEx"/>
		///                     class for the description of extended modifiers).
		///                     Passing negative parameter
		///                     is not recommended.
		///                     Zero value means that no modifiers were passed </param>
		/// <exception cref="IllegalArgumentException"> if <code>source</code> is null </exception>
		/// <seealso cref= #getSource() </seealso>
		/// <seealso cref= #getID() </seealso>
		/// <seealso cref= #getWhen() </seealso>
		/// <seealso cref= #getModifiers() </seealso>
		internal InputEvent(Component source, int id, long when, int modifiers) : base(source, id)
		{
			this.When_Renamed = when;
			this.Modifiers_Renamed = modifiers;
			CanAccessSystemClipboard_Renamed = CanAccessSystemClipboard();
		}

		private bool CanAccessSystemClipboard()
		{
			bool b = false;

			if (!GraphicsEnvironment.Headless)
			{
				SecurityManager sm = System.SecurityManager;
				if (sm != null)
				{
					try
					{
						sm.CheckPermission(SecurityConstants.AWT.ACCESS_CLIPBOARD_PERMISSION);
						b = true;
					}
					catch (SecurityException se)
					{
						if (Logger.isLoggable(PlatformLogger.Level.FINE))
						{
							Logger.fine("InputEvent.canAccessSystemClipboard() got SecurityException ", se);
						}
					}
				}
				else
				{
					b = true;
				}
			}

			return b;
		}

		/// <summary>
		/// Returns whether or not the Shift modifier is down on this event.
		/// </summary>
		public virtual bool ShiftDown
		{
			get
			{
				return (Modifiers_Renamed & SHIFT_MASK) != 0;
			}
		}

		/// <summary>
		/// Returns whether or not the Control modifier is down on this event.
		/// </summary>
		public virtual bool ControlDown
		{
			get
			{
				return (Modifiers_Renamed & CTRL_MASK) != 0;
			}
		}

		/// <summary>
		/// Returns whether or not the Meta modifier is down on this event.
		/// </summary>
		public virtual bool MetaDown
		{
			get
			{
				return (Modifiers_Renamed & META_MASK) != 0;
			}
		}

		/// <summary>
		/// Returns whether or not the Alt modifier is down on this event.
		/// </summary>
		public virtual bool AltDown
		{
			get
			{
				return (Modifiers_Renamed & ALT_MASK) != 0;
			}
		}

		/// <summary>
		/// Returns whether or not the AltGraph modifier is down on this event.
		/// </summary>
		public virtual bool AltGraphDown
		{
			get
			{
				return (Modifiers_Renamed & ALT_GRAPH_MASK) != 0;
			}
		}

		/// <summary>
		/// Returns the difference in milliseconds between the timestamp of when this event occurred and
		/// midnight, January 1, 1970 UTC.
		/// </summary>
		public virtual long When
		{
			get
			{
				return When_Renamed;
			}
		}

		/// <summary>
		/// Returns the modifier mask for this event.
		/// </summary>
		public virtual int Modifiers
		{
			get
			{
				return Modifiers_Renamed & (JDK_1_3_MODIFIERS | HIGH_MODIFIERS);
			}
		}

		/// <summary>
		/// Returns the extended modifier mask for this event.
		/// <P>
		/// Extended modifiers are the modifiers that ends with the _DOWN_MASK suffix,
		/// such as ALT_DOWN_MASK, BUTTON1_DOWN_MASK, and others.
		/// <P>
		/// Extended modifiers represent the state of all modal keys,
		/// such as ALT, CTRL, META, and the mouse buttons just after
		/// the event occurred.
		/// <P>
		/// For example, if the user presses <b>button 1</b> followed by
		/// <b>button 2</b>, and then releases them in the same order,
		/// the following sequence of events is generated:
		/// <PRE>
		///    <code>MOUSE_PRESSED</code>:  <code>BUTTON1_DOWN_MASK</code>
		///    <code>MOUSE_PRESSED</code>:  <code>BUTTON1_DOWN_MASK | BUTTON2_DOWN_MASK</code>
		///    <code>MOUSE_RELEASED</code>: <code>BUTTON2_DOWN_MASK</code>
		///    <code>MOUSE_CLICKED</code>:  <code>BUTTON2_DOWN_MASK</code>
		///    <code>MOUSE_RELEASED</code>:
		///    <code>MOUSE_CLICKED</code>:
		/// </PRE>
		/// <P>
		/// It is not recommended to compare the return value of this method
		/// using <code>==</code> because new modifiers can be added in the future.
		/// For example, the appropriate way to check that SHIFT and BUTTON1 are
		/// down, but CTRL is up is demonstrated by the following code:
		/// <PRE>
		///    int onmask = SHIFT_DOWN_MASK | BUTTON1_DOWN_MASK;
		///    int offmask = CTRL_DOWN_MASK;
		///    if ((event.getModifiersEx() &amp; (onmask | offmask)) == onmask) {
		///        ...
		///    }
		/// </PRE>
		/// The above code will work even if new modifiers are added.
		/// 
		/// @since 1.4
		/// </summary>
		public virtual int ModifiersEx
		{
			get
			{
				return Modifiers_Renamed & ~JDK_1_3_MODIFIERS;
			}
		}

		/// <summary>
		/// Consumes this event so that it will not be processed
		/// in the default manner by the source which originated it.
		/// </summary>
		public override void Consume()
		{
			Consumed_Renamed = true;
		}

		/// <summary>
		/// Returns whether or not this event has been consumed. </summary>
		/// <seealso cref= #consume </seealso>
		public override bool Consumed
		{
			get
			{
				return Consumed_Renamed;
			}
		}

		// state serialization compatibility with JDK 1.1
		internal const long SerialVersionUID = -2482525981698309786L;

		/// <summary>
		/// Returns a String describing the extended modifier keys and
		/// mouse buttons, such as "Shift", "Button1", or "Ctrl+Shift".
		/// These strings can be localized by changing the
		/// <code>awt.properties</code> file.
		/// <para>
		/// Note that passing negative parameter is incorrect,
		/// and will cause the returning an unspecified string.
		/// Zero parameter means that no modifiers were passed and will
		/// cause the returning an empty string.
		/// 
		/// </para>
		/// </summary>
		/// <param name="modifiers"> a modifier mask describing the extended
		///                modifier keys and mouse buttons for the event </param>
		/// <returns> a text description of the combination of extended
		///         modifier keys and mouse buttons that were held down
		///         during the event.
		/// @since 1.4 </returns>
		public static String GetModifiersExText(int modifiers)
		{
			StringBuilder buf = new StringBuilder();
			if ((modifiers & InputEvent.META_DOWN_MASK) != 0)
			{
				buf.Append(Toolkit.GetProperty("AWT.meta", "Meta"));
				buf.Append("+");
			}
			if ((modifiers & InputEvent.CTRL_DOWN_MASK) != 0)
			{
				buf.Append(Toolkit.GetProperty("AWT.control", "Ctrl"));
				buf.Append("+");
			}
			if ((modifiers & InputEvent.ALT_DOWN_MASK) != 0)
			{
				buf.Append(Toolkit.GetProperty("AWT.alt", "Alt"));
				buf.Append("+");
			}
			if ((modifiers & InputEvent.SHIFT_DOWN_MASK) != 0)
			{
				buf.Append(Toolkit.GetProperty("AWT.shift", "Shift"));
				buf.Append("+");
			}
			if ((modifiers & InputEvent.ALT_GRAPH_DOWN_MASK) != 0)
			{
				buf.Append(Toolkit.GetProperty("AWT.altGraph", "Alt Graph"));
				buf.Append("+");
			}

			int buttonNumber = 1;
			foreach (int mask in InputEvent.BUTTON_DOWN_MASK)
			{
				if ((modifiers & mask) != 0)
				{
					buf.Append(Toolkit.GetProperty("AWT.button" + buttonNumber, "Button" + buttonNumber));
					buf.Append("+");
				}
				buttonNumber++;
			}
			if (buf.Length() > 0)
			{
				buf.Length = buf.Length() - 1; // remove trailing '+'
			}
			return buf.ToString();
		}
	}

}
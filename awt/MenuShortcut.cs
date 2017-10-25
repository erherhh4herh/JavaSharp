using System;

/*
 * Copyright (c) 1996, 2009, Oracle and/or its affiliates. All rights reserved.
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
	/// The <code>MenuShortcut</code>class represents a keyboard accelerator
	/// for a MenuItem.
	/// <para>
	/// Menu shortcuts are created using virtual keycodes, not characters.
	/// For example, a menu shortcut for Ctrl-a (assuming that Control is
	/// the accelerator key) would be created with code like the following:
	/// </para>
	/// <para>
	/// <code>MenuShortcut ms = new MenuShortcut(KeyEvent.VK_A, false);</code>
	/// </para>
	/// <para> or alternatively
	/// </para>
	/// <para>
	/// <code>MenuShortcut ms = new MenuShortcut(KeyEvent.getExtendedKeyCodeForChar('A'), false);</code>
	/// </para>
	/// <para>
	/// Menu shortcuts may also be constructed for a wider set of keycodes
	/// using the <code>java.awt.event.KeyEvent.getExtendedKeyCodeForChar</code> call.
	/// For example, a menu shortcut for "Ctrl+cyrillic ef" is created by
	/// </para>
	/// <para>
	/// <code>MenuShortcut ms = new MenuShortcut(KeyEvent.getExtendedKeyCodeForChar('\u0444'), false);</code>
	/// </para>
	/// <para>
	/// Note that shortcuts created with a keycode or an extended keycode defined as a constant in <code>KeyEvent</code>
	/// work regardless of the current keyboard layout. However, a shortcut made of
	/// an extended keycode not listed in <code>KeyEvent</code>
	/// only work if the current keyboard layout produces a corresponding letter.
	/// </para>
	/// <para>
	/// The accelerator key is platform-dependent and may be obtained
	/// via <seealso cref="Toolkit#getMenuShortcutKeyMask"/>.
	/// 
	/// @author Thomas Ball
	/// @since JDK1.1
	/// </para>
	/// </summary>
	[Serializable]
	public class MenuShortcut
	{
		/// <summary>
		/// The virtual keycode for the menu shortcut.
		/// This is the keycode with which the menu shortcut will be created.
		/// Note that it is a virtual keycode, not a character,
		/// e.g. KeyEvent.VK_A, not 'a'.
		/// Note: in 1.1.x you must use setActionCommand() on a menu item
		/// in order for its shortcut to work, otherwise it will fire a null
		/// action command.
		/// 
		/// @serial </summary>
		/// <seealso cref= #getKey() </seealso>
		/// <seealso cref= #usesShiftModifier() </seealso>
		/// <seealso cref= java.awt.event.KeyEvent
		/// @since JDK1.1 </seealso>
		internal int Key_Renamed;

		/// <summary>
		/// Indicates whether the shft key was pressed.
		/// If true, the shift key was pressed.
		/// If false, the shift key was not pressed
		/// 
		/// @serial </summary>
		/// <seealso cref= #usesShiftModifier()
		/// @since JDK1.1 </seealso>
		internal bool UsesShift;

		/*
		 * JDK 1.1 serialVersionUID
		 */
		 private const long SerialVersionUID = 143448358473180225L;

		/// <summary>
		/// Constructs a new MenuShortcut for the specified virtual keycode. </summary>
		/// <param name="key"> the raw keycode for this MenuShortcut, as would be returned
		/// in the keyCode field of a <seealso cref="java.awt.event.KeyEvent KeyEvent"/> if
		/// this key were pressed. </param>
		/// <seealso cref= java.awt.event.KeyEvent
		///  </seealso>
		public MenuShortcut(int key) : this(key, false)
		{
		}

		/// <summary>
		/// Constructs a new MenuShortcut for the specified virtual keycode. </summary>
		/// <param name="key"> the raw keycode for this MenuShortcut, as would be returned
		/// in the keyCode field of a <seealso cref="java.awt.event.KeyEvent KeyEvent"/> if
		/// this key were pressed. </param>
		/// <param name="useShiftModifier"> indicates whether this MenuShortcut is invoked
		/// with the SHIFT key down. </param>
		/// <seealso cref= java.awt.event.KeyEvent
		///  </seealso>
		public MenuShortcut(int key, bool useShiftModifier)
		{
			this.Key_Renamed = key;
			this.UsesShift = useShiftModifier;
		}

		/// <summary>
		/// Returns the raw keycode of this MenuShortcut. </summary>
		/// <returns> the raw keycode of this MenuShortcut. </returns>
		/// <seealso cref= java.awt.event.KeyEvent
		/// @since JDK1.1 </seealso>
		public virtual int Key
		{
			get
			{
				return Key_Renamed;
			}
		}

		/// <summary>
		/// Returns whether this MenuShortcut must be invoked using the SHIFT key. </summary>
		/// <returns> <code>true</code> if this MenuShortcut must be invoked using the
		/// SHIFT key, <code>false</code> otherwise.
		/// @since JDK1.1 </returns>
		public virtual bool UsesShiftModifier()
		{
			return UsesShift;
		}

		/// <summary>
		/// Returns whether this MenuShortcut is the same as another:
		/// equality is defined to mean that both MenuShortcuts use the same key
		/// and both either use or don't use the SHIFT key. </summary>
		/// <param name="s"> the MenuShortcut to compare with this. </param>
		/// <returns> <code>true</code> if this MenuShortcut is the same as another,
		/// <code>false</code> otherwise.
		/// @since JDK1.1 </returns>
		public virtual bool Equals(MenuShortcut s)
		{
			return (s != null && (s.Key == Key_Renamed) && (s.UsesShiftModifier() == UsesShift));
		}

		/// <summary>
		/// Returns whether this MenuShortcut is the same as another:
		/// equality is defined to mean that both MenuShortcuts use the same key
		/// and both either use or don't use the SHIFT key. </summary>
		/// <param name="obj"> the Object to compare with this. </param>
		/// <returns> <code>true</code> if this MenuShortcut is the same as another,
		/// <code>false</code> otherwise.
		/// @since 1.2 </returns>
		public override bool Equals(Object obj)
		{
			if (obj is MenuShortcut)
			{
				return Equals((MenuShortcut) obj);
			}
			return false;
		}

		/// <summary>
		/// Returns the hashcode for this MenuShortcut. </summary>
		/// <returns> the hashcode for this MenuShortcut.
		/// @since 1.2 </returns>
		public override int HashCode()
		{
			return (UsesShift) ? (~Key_Renamed) : Key_Renamed;
		}

		/// <summary>
		/// Returns an internationalized description of the MenuShortcut. </summary>
		/// <returns> a string representation of this MenuShortcut.
		/// @since JDK1.1 </returns>
		public override String ToString()
		{
			int modifiers = 0;
			if (!GraphicsEnvironment.Headless)
			{
				modifiers = Toolkit.DefaultToolkit.MenuShortcutKeyMask;
			}
			if (UsesShiftModifier())
			{
				modifiers |= Event.SHIFT_MASK;
			}
			return KeyEvent.GetKeyModifiersText(modifiers) + "+" + KeyEvent.GetKeyText(Key_Renamed);
		}

		/// <summary>
		/// Returns the parameter string representing the state of this
		/// MenuShortcut. This string is useful for debugging. </summary>
		/// <returns>    the parameter string of this MenuShortcut.
		/// @since JDK1.1 </returns>
		protected internal virtual String ParamString()
		{
			String str = "key=" + Key_Renamed;
			if (UsesShiftModifier())
			{
				str += ",usesShiftModifier";
			}
			return str;
		}
	}

}
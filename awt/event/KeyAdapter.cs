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

	/// <summary>
	/// An abstract adapter class for receiving keyboard events.
	/// The methods in this class are empty. This class exists as
	/// convenience for creating listener objects.
	/// <P>
	/// Extend this class to create a <code>KeyEvent</code> listener
	/// and override the methods for the events of interest. (If you implement the
	/// <code>KeyListener</code> interface, you have to define all of
	/// the methods in it. This abstract class defines null methods for them
	/// all, so you can only have to define methods for events you care about.)
	/// <P>
	/// Create a listener object using the extended class and then register it with
	/// a component using the component's <code>addKeyListener</code>
	/// method. When a key is pressed, released, or typed,
	/// the relevant method in the listener object is invoked,
	/// and the <code>KeyEvent</code> is passed to it.
	/// 
	/// @author Carl Quinn
	/// </summary>
	/// <seealso cref= KeyEvent </seealso>
	/// <seealso cref= KeyListener </seealso>
	/// <seealso cref= <a href="https://docs.oracle.com/javase/tutorial/uiswing/events/keylistener.html">Tutorial: Writing a Key Listener</a>
	/// 
	/// @since 1.1 </seealso>
	public abstract class KeyAdapter : KeyListener
	{
		/// <summary>
		/// Invoked when a key has been typed.
		/// This event occurs when a key press is followed by a key release.
		/// </summary>
		public virtual void KeyTyped(KeyEvent e)
		{
		}

		/// <summary>
		/// Invoked when a key has been pressed.
		/// </summary>
		public virtual void KeyPressed(KeyEvent e)
		{
		}

		/// <summary>
		/// Invoked when a key has been released.
		/// </summary>
		public virtual void KeyReleased(KeyEvent e)
		{
		}
	}

}
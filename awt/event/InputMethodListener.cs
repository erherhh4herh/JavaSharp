/*
 * Copyright (c) 1997, 1999, Oracle and/or its affiliates. All rights reserved.
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
	/// The listener interface for receiving input method events. A text editing
	/// component has to install an input method event listener in order to work
	/// with input methods.
	/// 
	/// <para>
	/// The text editing component also has to provide an instance of InputMethodRequests.
	/// 
	/// @author JavaSoft Asia/Pacific
	/// </para>
	/// </summary>
	/// <seealso cref= InputMethodEvent </seealso>
	/// <seealso cref= java.awt.im.InputMethodRequests
	/// @since 1.2 </seealso>

	public interface InputMethodListener : EventListener
	{

		/// <summary>
		/// Invoked when the text entered through an input method has changed.
		/// </summary>
		void InputMethodTextChanged(InputMethodEvent @event);

		/// <summary>
		/// Invoked when the caret within composed text has changed.
		/// </summary>
		void CaretPositionChanged(InputMethodEvent @event);

	}

}
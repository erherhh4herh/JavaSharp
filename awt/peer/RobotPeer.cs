/*
 * Copyright (c) 1999, 2008, Oracle and/or its affiliates. All rights reserved.
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

namespace java.awt.peer
{

	/// <summary>
	/// RobotPeer defines an interface whereby toolkits support automated testing
	/// by allowing native input events to be generated from Java code.
	/// 
	/// This interface should not be directly imported by code outside the
	/// java.awt.* hierarchy; it is not to be considered public and is subject
	/// to change.
	/// 
	/// @author      Robi Khan
	/// </summary>
	public interface RobotPeer
	{
		/// <summary>
		/// Moves the mouse pointer to the specified screen location.
		/// </summary>
		/// <param name="x"> the X location on screen </param>
		/// <param name="y"> the Y location on screen
		/// </param>
		/// <seealso cref= Robot#mouseMove(int, int) </seealso>
		void MouseMove(int x, int y);

		/// <summary>
		/// Simulates a mouse press with the specified button(s).
		/// </summary>
		/// <param name="buttons"> the button mask
		/// </param>
		/// <seealso cref= Robot#mousePress(int) </seealso>
		void MousePress(int buttons);

		/// <summary>
		/// Simulates a mouse release with the specified button(s).
		/// </summary>
		/// <param name="buttons"> the button mask
		/// </param>
		/// <seealso cref= Robot#mouseRelease(int) </seealso>
		void MouseRelease(int buttons);

		/// <summary>
		/// Simulates mouse wheel action.
		/// </summary>
		/// <param name="wheelAmt"> number of notches to move the mouse wheel
		/// </param>
		/// <seealso cref= Robot#mouseWheel(int) </seealso>
		void MouseWheel(int wheelAmt);

		/// <summary>
		/// Simulates a key press of the specified key.
		/// </summary>
		/// <param name="keycode"> the key code to press
		/// </param>
		/// <seealso cref= Robot#keyPress(int) </seealso>
		void KeyPress(int keycode);

		/// <summary>
		/// Simulates a key release of the specified key.
		/// </summary>
		/// <param name="keycode"> the key code to release
		/// </param>
		/// <seealso cref= Robot#keyRelease(int) </seealso>
		void KeyRelease(int keycode);

		/// <summary>
		/// Gets the RGB value of the specified pixel on screen.
		/// </summary>
		/// <param name="x"> the X screen coordinate </param>
		/// <param name="y"> the Y screen coordinate
		/// </param>
		/// <returns> the RGB value of the specified pixel on screen
		/// </returns>
		/// <seealso cref= Robot#getPixelColor(int, int) </seealso>
		int GetRGBPixel(int x, int y);

		/// <summary>
		/// Gets the RGB values of the specified screen area as an array.
		/// </summary>
		/// <param name="bounds"> the screen area to capture the RGB values from
		/// </param>
		/// <returns> the RGB values of the specified screen area
		/// </returns>
		/// <seealso cref= Robot#createScreenCapture(Rectangle) </seealso>
		int[] GetRGBPixels(Rectangle bounds);

		/// <summary>
		/// Disposes the robot peer when it is not needed anymore.
		/// </summary>
		void Dispose();
	}

}
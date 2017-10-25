using System;
using System.Threading;

/*
 * Copyright (c) 1999, 2014, Oracle and/or its affiliates. All rights reserved.
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

	using ComponentFactory = sun.awt.ComponentFactory;
	using SunToolkit = sun.awt.SunToolkit;
	using SunWritableRaster = sun.awt.image.SunWritableRaster;
	using SecurityConstants = sun.security.util.SecurityConstants;

	/// <summary>
	/// This class is used to generate native system input events
	/// for the purposes of test automation, self-running demos, and
	/// other applications where control of the mouse and keyboard
	/// is needed. The primary purpose of Robot is to facilitate
	/// automated testing of Java platform implementations.
	/// <para>
	/// Using the class to generate input events differs from posting
	/// events to the AWT event queue or AWT components in that the
	/// events are generated in the platform's native input
	/// queue. For example, <code>Robot.mouseMove</code> will actually move
	/// the mouse cursor instead of just generating mouse move events.
	/// </para>
	/// <para>
	/// Note that some platforms require special privileges or extensions
	/// to access low-level input control. If the current platform configuration
	/// does not allow input control, an <code>AWTException</code> will be thrown
	/// when trying to construct Robot objects. For example, X-Window systems
	/// will throw the exception if the XTEST 2.2 standard extension is not supported
	/// (or not enabled) by the X server.
	/// </para>
	/// <para>
	/// Applications that use Robot for purposes other than self-testing should
	/// handle these error conditions gracefully.
	/// 
	/// @author      Robi Khan
	/// @since       1.3
	/// </para>
	/// </summary>
	public class Robot
	{
		private const int MAX_DELAY = 60000;
		private RobotPeer Peer;
		private bool IsAutoWaitForIdle = false;
		private int AutoDelay_Renamed = 0;
		private static int LEGAL_BUTTON_MASK = 0;

		private DirectColorModel ScreenCapCM = null;

		/// <summary>
		/// Constructs a Robot object in the coordinate system of the primary screen.
		/// <para>
		/// 
		/// </para>
		/// </summary>
		/// <exception cref="AWTException"> if the platform configuration does not allow
		/// low-level input control.  This exception is always thrown when
		/// GraphicsEnvironment.isHeadless() returns true </exception>
		/// <exception cref="SecurityException"> if <code>createRobot</code> permission is not granted </exception>
		/// <seealso cref=     java.awt.GraphicsEnvironment#isHeadless </seealso>
		/// <seealso cref=     SecurityManager#checkPermission </seealso>
		/// <seealso cref=     AWTPermission </seealso>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public Robot() throws AWTException
		public Robot()
		{
			if (GraphicsEnvironment.Headless)
			{
				throw new AWTException("headless environment");
			}
			Init(GraphicsEnvironment.LocalGraphicsEnvironment.DefaultScreenDevice);
		}

		/// <summary>
		/// Creates a Robot for the given screen device. Coordinates passed
		/// to Robot method calls like mouseMove and createScreenCapture will
		/// be interpreted as being in the same coordinate system as the
		/// specified screen. Note that depending on the platform configuration,
		/// multiple screens may either:
		/// <ul>
		/// <li>share the same coordinate system to form a combined virtual screen</li>
		/// <li>use different coordinate systems to act as independent screens</li>
		/// </ul>
		/// This constructor is meant for the latter case.
		/// <para>
		/// If screen devices are reconfigured such that the coordinate system is
		/// affected, the behavior of existing Robot objects is undefined.
		/// 
		/// </para>
		/// </summary>
		/// <param name="screen">    A screen GraphicsDevice indicating the coordinate
		///                  system the Robot will operate in. </param>
		/// <exception cref="AWTException"> if the platform configuration does not allow
		/// low-level input control.  This exception is always thrown when
		/// GraphicsEnvironment.isHeadless() returns true. </exception>
		/// <exception cref="IllegalArgumentException"> if <code>screen</code> is not a screen
		///          GraphicsDevice. </exception>
		/// <exception cref="SecurityException"> if <code>createRobot</code> permission is not granted </exception>
		/// <seealso cref=     java.awt.GraphicsEnvironment#isHeadless </seealso>
		/// <seealso cref=     GraphicsDevice </seealso>
		/// <seealso cref=     SecurityManager#checkPermission </seealso>
		/// <seealso cref=     AWTPermission </seealso>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public Robot(GraphicsDevice screen) throws AWTException
		public Robot(GraphicsDevice screen)
		{
			CheckIsScreenDevice(screen);
			Init(screen);
		}

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: private void init(GraphicsDevice screen) throws AWTException
		private void Init(GraphicsDevice screen)
		{
			CheckRobotAllowed();
			Toolkit toolkit = Toolkit.DefaultToolkit;
			if (toolkit is ComponentFactory)
			{
				Peer = ((ComponentFactory)toolkit).createRobot(this, screen);
				Disposer = new RobotDisposer(Peer);
				sun.java2d.Disposer.addRecord(Anchor, Disposer);
			}
			InitLegalButtonMask();
		}

		private static void InitLegalButtonMask()
		{
			lock (typeof(Robot))
			{
				if (LEGAL_BUTTON_MASK != 0)
				{
					return;
				}
        
				int tmpMask = 0;
				if (Toolkit.DefaultToolkit.AreExtraMouseButtonsEnabled())
				{
					if (Toolkit.DefaultToolkit is SunToolkit)
					{
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final int buttonsNumber = ((sun.awt.SunToolkit)(Toolkit.getDefaultToolkit())).getNumberOfButtons();
						int buttonsNumber = ((SunToolkit)(Toolkit.DefaultToolkit)).NumberOfButtons;
						for (int i = 0; i < buttonsNumber; i++)
						{
							tmpMask |= InputEvent.GetMaskForButton(i + 1);
						}
					}
				}
				tmpMask |= InputEvent.BUTTON1_MASK | InputEvent.BUTTON2_MASK | InputEvent.BUTTON3_MASK | InputEvent.BUTTON1_DOWN_MASK | InputEvent.BUTTON2_DOWN_MASK | InputEvent.BUTTON3_DOWN_MASK;
				LEGAL_BUTTON_MASK = tmpMask;
			}
		}

		/* determine if the security policy allows Robot's to be created */
		private void CheckRobotAllowed()
		{
			SecurityManager security = System.SecurityManager;
			if (security != null)
			{
				security.CheckPermission(SecurityConstants.AWT.CREATE_ROBOT_PERMISSION);
			}
		}

		/* check if the given device is a screen device */
		private void CheckIsScreenDevice(GraphicsDevice device)
		{
			if (device == null || device.Type != GraphicsDevice.TYPE_RASTER_SCREEN)
			{
				throw new IllegalArgumentException("not a valid screen device");
			}
		}

		[NonSerialized]
		private Object Anchor = new Object();

		internal class RobotDisposer : sun.java2d.DisposerRecord
		{
			internal readonly RobotPeer Peer;
			public RobotDisposer(RobotPeer peer)
			{
				this.Peer = peer;
			}
			public virtual void Dispose()
			{
				if (Peer != null)
				{
					Peer.Dispose();
				}
			}
		}

		[NonSerialized]
		private RobotDisposer Disposer;

		/// <summary>
		/// Moves mouse pointer to given screen coordinates. </summary>
		/// <param name="x">         X position </param>
		/// <param name="y">         Y position </param>
		public virtual void MouseMove(int x, int y)
		{
			lock (this)
			{
				Peer.MouseMove(x, y);
				AfterEvent();
			}
		}

		/// <summary>
		/// Presses one or more mouse buttons.  The mouse buttons should
		/// be released using the <seealso cref="#mouseRelease(int)"/> method.
		/// </summary>
		/// <param name="buttons"> the Button mask; a combination of one or more
		/// mouse button masks.
		/// <para>
		/// It is allowed to use only a combination of valid values as a {@code buttons} parameter.
		/// A valid combination consists of {@code InputEvent.BUTTON1_DOWN_MASK},
		/// {@code InputEvent.BUTTON2_DOWN_MASK}, {@code InputEvent.BUTTON3_DOWN_MASK}
		/// and values returned by the
		/// <seealso cref="InputEvent#getMaskForButton(int) InputEvent.getMaskForButton(button)"/> method.
		/// 
		/// The valid combination also depends on a
		/// <seealso cref="Toolkit#areExtraMouseButtonsEnabled() Toolkit.areExtraMouseButtonsEnabled()"/> value as follows:
		/// <ul>
		/// <li> If support for extended mouse buttons is
		/// <seealso cref="Toolkit#areExtraMouseButtonsEnabled() disabled"/> by Java
		/// then it is allowed to use only the following standard button masks:
		/// {@code InputEvent.BUTTON1_DOWN_MASK}, {@code InputEvent.BUTTON2_DOWN_MASK},
		/// {@code InputEvent.BUTTON3_DOWN_MASK}.
		/// <li> If support for extended mouse buttons is
		/// <seealso cref="Toolkit#areExtraMouseButtonsEnabled() enabled"/> by Java
		/// then it is allowed to use the standard button masks
		/// and masks for existing extended mouse buttons, if the mouse has more then three buttons.
		/// In that way, it is allowed to use the button masks corresponding to the buttons
		/// in the range from 1 to <seealso cref="java.awt.MouseInfo#getNumberOfButtons() MouseInfo.getNumberOfButtons()"/>.
		/// <br>
		/// It is recommended to use the <seealso cref="InputEvent#getMaskForButton(int) InputEvent.getMaskForButton(button)"/>
		/// method to obtain the mask for any mouse button by its number.
		/// </ul>
		/// </para>
		/// <para>
		/// The following standard button masks are also accepted:
		/// <ul>
		/// <li>{@code InputEvent.BUTTON1_MASK}
		/// <li>{@code InputEvent.BUTTON2_MASK}
		/// <li>{@code InputEvent.BUTTON3_MASK}
		/// </ul>
		/// However, it is recommended to use {@code InputEvent.BUTTON1_DOWN_MASK},
		/// {@code InputEvent.BUTTON2_DOWN_MASK},  {@code InputEvent.BUTTON3_DOWN_MASK} instead.
		/// Either extended {@code _DOWN_MASK} or old {@code _MASK} values
		/// should be used, but both those models should not be mixed.
		/// </para>
		/// </param>
		/// <exception cref="IllegalArgumentException"> if the {@code buttons} mask contains the mask for extra mouse button
		///         and support for extended mouse buttons is <seealso cref="Toolkit#areExtraMouseButtonsEnabled() disabled"/> by Java </exception>
		/// <exception cref="IllegalArgumentException"> if the {@code buttons} mask contains the mask for extra mouse button
		///         that does not exist on the mouse and support for extended mouse buttons is <seealso cref="Toolkit#areExtraMouseButtonsEnabled() enabled"/> by Java </exception>
		/// <seealso cref= #mouseRelease(int) </seealso>
		/// <seealso cref= InputEvent#getMaskForButton(int) </seealso>
		/// <seealso cref= Toolkit#areExtraMouseButtonsEnabled() </seealso>
		/// <seealso cref= java.awt.MouseInfo#getNumberOfButtons() </seealso>
		/// <seealso cref= java.awt.event.MouseEvent </seealso>
		public virtual void MousePress(int buttons)
		{
			lock (this)
			{
				CheckButtonsArgument(buttons);
				Peer.MousePress(buttons);
				AfterEvent();
			}
		}

		/// <summary>
		/// Releases one or more mouse buttons.
		/// </summary>
		/// <param name="buttons"> the Button mask; a combination of one or more
		/// mouse button masks.
		/// <para>
		/// It is allowed to use only a combination of valid values as a {@code buttons} parameter.
		/// A valid combination consists of {@code InputEvent.BUTTON1_DOWN_MASK},
		/// {@code InputEvent.BUTTON2_DOWN_MASK}, {@code InputEvent.BUTTON3_DOWN_MASK}
		/// and values returned by the
		/// <seealso cref="InputEvent#getMaskForButton(int) InputEvent.getMaskForButton(button)"/> method.
		/// 
		/// The valid combination also depends on a
		/// <seealso cref="Toolkit#areExtraMouseButtonsEnabled() Toolkit.areExtraMouseButtonsEnabled()"/> value as follows:
		/// <ul>
		/// <li> If the support for extended mouse buttons is
		/// <seealso cref="Toolkit#areExtraMouseButtonsEnabled() disabled"/> by Java
		/// then it is allowed to use only the following standard button masks:
		/// {@code InputEvent.BUTTON1_DOWN_MASK}, {@code InputEvent.BUTTON2_DOWN_MASK},
		/// {@code InputEvent.BUTTON3_DOWN_MASK}.
		/// <li> If the support for extended mouse buttons is
		/// <seealso cref="Toolkit#areExtraMouseButtonsEnabled() enabled"/> by Java
		/// then it is allowed to use the standard button masks
		/// and masks for existing extended mouse buttons, if the mouse has more then three buttons.
		/// In that way, it is allowed to use the button masks corresponding to the buttons
		/// in the range from 1 to <seealso cref="java.awt.MouseInfo#getNumberOfButtons() MouseInfo.getNumberOfButtons()"/>.
		/// <br>
		/// It is recommended to use the <seealso cref="InputEvent#getMaskForButton(int) InputEvent.getMaskForButton(button)"/>
		/// method to obtain the mask for any mouse button by its number.
		/// </ul>
		/// </para>
		/// <para>
		/// The following standard button masks are also accepted:
		/// <ul>
		/// <li>{@code InputEvent.BUTTON1_MASK}
		/// <li>{@code InputEvent.BUTTON2_MASK}
		/// <li>{@code InputEvent.BUTTON3_MASK}
		/// </ul>
		/// However, it is recommended to use {@code InputEvent.BUTTON1_DOWN_MASK},
		/// {@code InputEvent.BUTTON2_DOWN_MASK},  {@code InputEvent.BUTTON3_DOWN_MASK} instead.
		/// Either extended {@code _DOWN_MASK} or old {@code _MASK} values
		/// should be used, but both those models should not be mixed.
		/// </para>
		/// </param>
		/// <exception cref="IllegalArgumentException"> if the {@code buttons} mask contains the mask for extra mouse button
		///         and support for extended mouse buttons is <seealso cref="Toolkit#areExtraMouseButtonsEnabled() disabled"/> by Java </exception>
		/// <exception cref="IllegalArgumentException"> if the {@code buttons} mask contains the mask for extra mouse button
		///         that does not exist on the mouse and support for extended mouse buttons is <seealso cref="Toolkit#areExtraMouseButtonsEnabled() enabled"/> by Java </exception>
		/// <seealso cref= #mousePress(int) </seealso>
		/// <seealso cref= InputEvent#getMaskForButton(int) </seealso>
		/// <seealso cref= Toolkit#areExtraMouseButtonsEnabled() </seealso>
		/// <seealso cref= java.awt.MouseInfo#getNumberOfButtons() </seealso>
		/// <seealso cref= java.awt.event.MouseEvent </seealso>
		public virtual void MouseRelease(int buttons)
		{
			lock (this)
			{
				CheckButtonsArgument(buttons);
				Peer.MouseRelease(buttons);
				AfterEvent();
			}
		}

		private void CheckButtonsArgument(int buttons)
		{
			if ((buttons | LEGAL_BUTTON_MASK) != LEGAL_BUTTON_MASK)
			{
				throw new IllegalArgumentException("Invalid combination of button flags");
			}
		}

		/// <summary>
		/// Rotates the scroll wheel on wheel-equipped mice.
		/// </summary>
		/// <param name="wheelAmt">  number of "notches" to move the mouse wheel
		///                  Negative values indicate movement up/away from the user,
		///                  positive values indicate movement down/towards the user.
		/// 
		/// @since 1.4 </param>
		public virtual void MouseWheel(int wheelAmt)
		{
			lock (this)
			{
				Peer.MouseWheel(wheelAmt);
				AfterEvent();
			}
		}

		/// <summary>
		/// Presses a given key.  The key should be released using the
		/// <code>keyRelease</code> method.
		/// <para>
		/// Key codes that have more than one physical key associated with them
		/// (e.g. <code>KeyEvent.VK_SHIFT</code> could mean either the
		/// left or right shift key) will map to the left key.
		/// 
		/// </para>
		/// </summary>
		/// <param name="keycode"> Key to press (e.g. <code>KeyEvent.VK_A</code>) </param>
		/// <exception cref="IllegalArgumentException"> if <code>keycode</code> is not
		///          a valid key </exception>
		/// <seealso cref=     #keyRelease(int) </seealso>
		/// <seealso cref=     java.awt.event.KeyEvent </seealso>
		public virtual void KeyPress(int keycode)
		{
			lock (this)
			{
				CheckKeycodeArgument(keycode);
				Peer.KeyPress(keycode);
				AfterEvent();
			}
		}

		/// <summary>
		/// Releases a given key.
		/// <para>
		/// Key codes that have more than one physical key associated with them
		/// (e.g. <code>KeyEvent.VK_SHIFT</code> could mean either the
		/// left or right shift key) will map to the left key.
		/// 
		/// </para>
		/// </summary>
		/// <param name="keycode"> Key to release (e.g. <code>KeyEvent.VK_A</code>) </param>
		/// <exception cref="IllegalArgumentException"> if <code>keycode</code> is not a
		///          valid key </exception>
		/// <seealso cref=  #keyPress(int) </seealso>
		/// <seealso cref=     java.awt.event.KeyEvent </seealso>
		public virtual void KeyRelease(int keycode)
		{
			lock (this)
			{
				CheckKeycodeArgument(keycode);
				Peer.KeyRelease(keycode);
				AfterEvent();
			}
		}

		private void CheckKeycodeArgument(int keycode)
		{
			// rather than build a big table or switch statement here, we'll
			// just check that the key isn't VK_UNDEFINED and assume that the
			// peer implementations will throw an exception for other bogus
			// values e.g. -1, 999999
			if (keycode == KeyEvent.VK_UNDEFINED)
			{
				throw new IllegalArgumentException("Invalid key code");
			}
		}

		/// <summary>
		/// Returns the color of a pixel at the given screen coordinates. </summary>
		/// <param name="x">       X position of pixel </param>
		/// <param name="y">       Y position of pixel </param>
		/// <returns>  Color of the pixel </returns>
		public virtual Color GetPixelColor(int x, int y)
		{
			lock (this)
			{
				Color color = new Color(Peer.GetRGBPixel(x, y));
				return color;
			}
		}

		/// <summary>
		/// Creates an image containing pixels read from the screen.  This image does
		/// not include the mouse cursor. </summary>
		/// <param name="screenRect">      Rect to capture in screen coordinates </param>
		/// <returns>  The captured image </returns>
		/// <exception cref="IllegalArgumentException"> if <code>screenRect</code> width and height are not greater than zero </exception>
		/// <exception cref="SecurityException"> if <code>readDisplayPixels</code> permission is not granted </exception>
		/// <seealso cref=     SecurityManager#checkPermission </seealso>
		/// <seealso cref=     AWTPermission </seealso>
		public virtual BufferedImage CreateScreenCapture(Rectangle screenRect)
		{
			lock (this)
			{
				CheckScreenCaptureAllowed();
        
				CheckValidRect(screenRect);
        
				BufferedImage image;
				DataBufferInt buffer;
				WritableRaster raster;
        
				if (ScreenCapCM == null)
				{
					/*
					 * Fix for 4285201
					 * Create a DirectColorModel equivalent to the default RGB ColorModel,
					 * except with no Alpha component.
					 */
        
					ScreenCapCM = new DirectColorModel(24, 0x00FF0000, 0x0000FF00, 0x000000FF);
													   /* red mask */
													   /* green mask */
													   /* blue mask */
				}
        
				// need to sync the toolkit prior to grabbing the pixels since in some
				// cases rendering to the screen may be delayed
				Toolkit.DefaultToolkit.Sync();
        
				int[] pixels;
				int[] bandmasks = new int[3];
        
				pixels = Peer.GetRGBPixels(screenRect);
				buffer = new DataBufferInt(pixels, pixels.Length);
        
				bandmasks[0] = ScreenCapCM.RedMask;
				bandmasks[1] = ScreenCapCM.GreenMask;
				bandmasks[2] = ScreenCapCM.BlueMask;
        
				raster = Raster.CreatePackedRaster(buffer, screenRect.Width_Renamed, screenRect.Height_Renamed, screenRect.Width_Renamed, bandmasks, null);
				SunWritableRaster.makeTrackable(buffer);
        
				image = new BufferedImage(ScreenCapCM, raster, false, null);
        
				return image;
			}
		}

		private static void CheckValidRect(Rectangle rect)
		{
			if (rect.Width_Renamed <= 0 || rect.Height_Renamed <= 0)
			{
				throw new IllegalArgumentException("Rectangle width and height must be > 0");
			}
		}

		private static void CheckScreenCaptureAllowed()
		{
			SecurityManager security = System.SecurityManager;
			if (security != null)
			{
				security.CheckPermission(SecurityConstants.AWT.READ_DISPLAY_PIXELS_PERMISSION);
			}
		}

		/*
		 * Called after an event is generated
		 */
		private void AfterEvent()
		{
			AutoWaitForIdle();
			AutoDelay();
		}

		/// <summary>
		/// Returns whether this Robot automatically invokes <code>waitForIdle</code>
		/// after generating an event. </summary>
		/// <returns> Whether <code>waitForIdle</code> is automatically called </returns>
		public virtual bool AutoWaitForIdle
		{
			get
			{
				lock (this)
				{
					return IsAutoWaitForIdle;
				}
			}
			set
			{
				lock (this)
				{
					IsAutoWaitForIdle = value;
				}
			}
		}


		/*
		 * Calls waitForIdle after every event if so desired.
		 */
		private void AutoWaitForIdle()
		{
			if (IsAutoWaitForIdle)
			{
				WaitForIdle();
			}
		}

		/// <summary>
		/// Returns the number of milliseconds this Robot sleeps after generating an event.
		/// </summary>
		public virtual int AutoDelay
		{
			get
			{
				lock (this)
				{
					return AutoDelay_Renamed;
				}
			}
			set
			{
				lock (this)
				{
					CheckDelayArgument(value);
					AutoDelay_Renamed = value;
				}
			}
		}


		/*
		 * Automatically sleeps for the specified interval after event generated.
		 */
		private void AutoDelay()
		{
			Delay(AutoDelay_Renamed);
		}

		/// <summary>
		/// Sleeps for the specified time.
		/// To catch any <code>InterruptedException</code>s that occur,
		/// <code>Thread.sleep()</code> may be used instead. </summary>
		/// <param name="ms">      time to sleep in milliseconds </param>
		/// <exception cref="IllegalArgumentException"> if <code>ms</code> is not between 0 and 60,000 milliseconds inclusive </exception>
		/// <seealso cref=     java.lang.Thread#sleep </seealso>
		public virtual void Delay(int ms)
		{
			lock (this)
			{
				CheckDelayArgument(ms);
				try
				{
					Thread.Sleep(ms);
				}
				catch (InterruptedException ite)
				{
					Console.WriteLine(ite.ToString());
					Console.Write(ite.StackTrace);
				}
			}
		}

		private void CheckDelayArgument(int ms)
		{
			if (ms < 0 || ms > MAX_DELAY)
			{
				throw new IllegalArgumentException("Delay must be to 0 to 60,000ms");
			}
		}

		/// <summary>
		/// Waits until all events currently on the event queue have been processed. </summary>
		/// <exception cref="IllegalThreadStateException"> if called on the AWT event dispatching thread </exception>
		public virtual void WaitForIdle()
		{
			lock (this)
			{
				CheckNotDispatchThread();
				// post a dummy event to the queue so we know when
				// all the events before it have been processed
				try
				{
					SunToolkit.flushPendingEvents();
					EventQueue.InvokeAndWait(new RunnableAnonymousInnerClassHelper(this));
				}
				catch (InterruptedException ite)
				{
					System.Console.Error.WriteLine("Robot.waitForIdle, non-fatal exception caught:");
					Console.WriteLine(ite.ToString());
					Console.Write(ite.StackTrace);
				}
				catch (InvocationTargetException ine)
				{
					System.Console.Error.WriteLine("Robot.waitForIdle, non-fatal exception caught:");
					Console.WriteLine(ine.ToString());
					Console.Write(ine.StackTrace);
				}
			}
		}

		private class RunnableAnonymousInnerClassHelper : Runnable
		{
			private readonly Robot OuterInstance;

			public RunnableAnonymousInnerClassHelper(Robot outerInstance)
			{
				this.OuterInstance = outerInstance;
			}

			public virtual void Run()
			{
				// dummy implementation
			}
		}

		private void CheckNotDispatchThread()
		{
			if (EventQueue.DispatchThread)
			{
				throw new IllegalThreadStateException("Cannot call method from the event dispatcher thread");
			}
		}

		/// <summary>
		/// Returns a string representation of this Robot.
		/// </summary>
		/// <returns>  the string representation. </returns>
		public override String ToString()
		{
			lock (this)
			{
				String @params = "autoDelay = " + AutoDelay + ", " + "autoWaitForIdle = " + AutoWaitForIdle;
//JAVA TO C# CONVERTER WARNING: The .NET Type.FullName property will not always yield results identical to the Java Class.getName method:
				return this.GetType().FullName + "[ " + @params + " ]";
			}
		}
	}

}
using System.Diagnostics;

/*
 * Copyright (c) 2003, Oracle and/or its affiliates. All rights reserved.
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

	using SecurityConstants = sun.security.util.SecurityConstants;
	/// <summary>
	/// <code>MouseInfo</code>  provides methods for getting information about the mouse,
	/// such as mouse pointer location and the number of mouse buttons.
	/// 
	/// @author     Roman Poborchiy
	/// @since 1.5
	/// </summary>

	public class MouseInfo
	{

		/// <summary>
		/// Private constructor to prevent instantiation.
		/// </summary>
		private MouseInfo()
		{
		}

		/// <summary>
		/// Returns a <code>PointerInfo</code> instance that represents the current
		/// location of the mouse pointer.
		/// The <code>GraphicsDevice</code> stored in this <code>PointerInfo</code>
		/// contains the mouse pointer. The coordinate system used for the mouse position
		/// depends on whether or not the <code>GraphicsDevice</code> is part of a virtual
		/// screen device.
		/// For virtual screen devices, the coordinates are given in the virtual
		/// coordinate system, otherwise they are returned in the coordinate system
		/// of the <code>GraphicsDevice</code>. See <seealso cref="GraphicsConfiguration"/>
		/// for more information about the virtual screen devices.
		/// On systems without a mouse, returns <code>null</code>.
		/// <para>
		/// If there is a security manager, its <code>checkPermission</code> method
		/// is called with an <code>AWTPermission("watchMousePointer")</code>
		/// permission before creating and returning a <code>PointerInfo</code>
		/// object. This may result in a <code>SecurityException</code>.
		/// 
		/// </para>
		/// </summary>
		/// <exception cref="HeadlessException"> if GraphicsEnvironment.isHeadless() returns true </exception>
		/// <exception cref="SecurityException"> if a security manager exists and its
		///            <code>checkPermission</code> method doesn't allow the operation </exception>
		/// <seealso cref=       GraphicsConfiguration </seealso>
		/// <seealso cref=       SecurityManager#checkPermission </seealso>
		/// <seealso cref=       java.awt.AWTPermission </seealso>
		/// <returns>    location of the mouse pointer
		/// @since     1.5 </returns>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public static PointerInfo getPointerInfo() throws HeadlessException
		public static PointerInfo PointerInfo
		{
			get
			{
				if (GraphicsEnvironment.Headless)
				{
					throw new HeadlessException();
				}
    
				SecurityManager security = System.SecurityManager;
				if (security != null)
				{
					security.CheckPermission(SecurityConstants.AWT.WATCH_MOUSE_PERMISSION);
				}
    
				Point point = new Point(0, 0);
				int deviceNum = Toolkit.DefaultToolkit.MouseInfoPeer.FillPointWithCoords(point);
				GraphicsDevice[] gds = GraphicsEnvironment.LocalGraphicsEnvironment.ScreenDevices;
				PointerInfo retval = null;
				if (AreScreenDevicesIndependent(gds))
				{
					retval = new PointerInfo(gds[deviceNum], point);
				}
				else
				{
					for (int i = 0; i < gds.Length; i++)
					{
						GraphicsConfiguration gc = gds[i].DefaultConfiguration;
						Rectangle bounds = gc.Bounds;
						if (bounds.Contains(point))
						{
							retval = new PointerInfo(gds[i], point);
						}
					}
				}
    
				return retval;
			}
		}

		private static bool AreScreenDevicesIndependent(GraphicsDevice[] gds)
		{
			for (int i = 0; i < gds.Length; i++)
			{
				Rectangle bounds = gds[i].DefaultConfiguration.Bounds;
				if (bounds.x != 0 || bounds.y != 0)
				{
					return false;
				}
			}
			return true;
		}

		/// <summary>
		/// Returns the number of buttons on the mouse.
		/// On systems without a mouse, returns <code>-1</code>.
		/// </summary>
		/// <exception cref="HeadlessException"> if GraphicsEnvironment.isHeadless() returns true </exception>
		/// <returns> number of buttons on the mouse
		/// @since 1.5 </returns>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public static int getNumberOfButtons() throws HeadlessException
		public static int NumberOfButtons
		{
			get
			{
				if (GraphicsEnvironment.Headless)
				{
					throw new HeadlessException();
				}
				Object prop = Toolkit.DefaultToolkit.GetDesktopProperty("awt.mouse.numButtons");
				if (prop is Integer)
				{
					return ((Integer)prop).IntValue();
				}
    
				// This should never happen.
				Debug.Assert(false, "awt.mouse.numButtons is not an integer property");
				return 0;
			}
		}

	}

}
using System.Collections.Generic;

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


namespace java.awt
{

	using AWTAccessor = sun.awt.AWTAccessor;
	using AppContext = sun.awt.AppContext;
	using SunToolkit = sun.awt.SunToolkit;

	/// <summary>
	/// The <code>GraphicsDevice</code> class describes the graphics devices
	/// that might be available in a particular graphics environment.  These
	/// include screen and printer devices. Note that there can be many screens
	/// and many printers in an instance of <seealso cref="GraphicsEnvironment"/>. Each
	/// graphics device has one or more <seealso cref="GraphicsConfiguration"/> objects
	/// associated with it.  These objects specify the different configurations
	/// in which the <code>GraphicsDevice</code> can be used.
	/// <para>
	/// In a multi-screen environment, the <code>GraphicsConfiguration</code>
	/// objects can be used to render components on multiple screens.  The
	/// following code sample demonstrates how to create a <code>JFrame</code>
	/// object for each <code>GraphicsConfiguration</code> on each screen
	/// device in the <code>GraphicsEnvironment</code>:
	/// <pre>{@code
	///   GraphicsEnvironment ge = GraphicsEnvironment.
	///   getLocalGraphicsEnvironment();
	///   GraphicsDevice[] gs = ge.getScreenDevices();
	///   for (int j = 0; j < gs.length; j++) {
	///      GraphicsDevice gd = gs[j];
	///      GraphicsConfiguration[] gc =
	///      gd.getConfigurations();
	///      for (int i=0; i < gc.length; i++) {
	///         JFrame f = new
	///         JFrame(gs[j].getDefaultConfiguration());
	///         Canvas c = new Canvas(gc[i]);
	///         Rectangle gcBounds = gc[i].getBounds();
	///         int xoffs = gcBounds.x;
	///         int yoffs = gcBounds.y;
	///         f.getContentPane().add(c);
	///         f.setLocation((i*50)+xoffs, (i*60)+yoffs);
	///         f.show();
	///      }
	///   }
	/// }</pre>
	/// </para>
	/// <para>
	/// For more information on full-screen exclusive mode API, see the
	/// <a href="https://docs.oracle.com/javase/tutorial/extra/fullscreen/index.html">
	/// Full-Screen Exclusive Mode API Tutorial</a>.
	/// 
	/// </para>
	/// </summary>
	/// <seealso cref= GraphicsEnvironment </seealso>
	/// <seealso cref= GraphicsConfiguration </seealso>
	public abstract class GraphicsDevice
	{

		private Window FullScreenWindow_Renamed;
		private AppContext FullScreenAppContext; // tracks which AppContext
												 // created the FS window
		// this lock is used for making synchronous changes to the AppContext's
		// current full screen window
		private readonly Object FsAppContextLock = new Object();

		private Rectangle WindowedModeBounds;

		/// <summary>
		/// This is an abstract class that cannot be instantiated directly.
		/// Instances must be obtained from a suitable factory or query method. </summary>
		/// <seealso cref= GraphicsEnvironment#getScreenDevices </seealso>
		/// <seealso cref= GraphicsEnvironment#getDefaultScreenDevice </seealso>
		/// <seealso cref= GraphicsConfiguration#getDevice </seealso>
		protected internal GraphicsDevice()
		{
		}

		/// <summary>
		/// Device is a raster screen.
		/// </summary>
		public const int TYPE_RASTER_SCREEN = 0;

		/// <summary>
		/// Device is a printer.
		/// </summary>
		public const int TYPE_PRINTER = 1;

		/// <summary>
		/// Device is an image buffer.  This buffer can reside in device
		/// or system memory but it is not physically viewable by the user.
		/// </summary>
		public const int TYPE_IMAGE_BUFFER = 2;

		/// <summary>
		/// Kinds of translucency supported by the underlying system.
		/// </summary>
		/// <seealso cref= #isWindowTranslucencySupported
		/// 
		/// @since 1.7 </seealso>
		public sealed class WindowTranslucency
		{
			/// <summary>
			/// Represents support in the underlying system for windows each pixel
			/// of which is guaranteed to be either completely opaque, with
			/// an alpha value of 1.0, or completely transparent, with an alpha
			/// value of 0.0.
			/// </summary>
			PERPIXEL_TRANSPARENT,
			public static readonly WindowTranslucency PERPIXEL_TRANSPARENT = new WindowTranslucency("PERPIXEL_TRANSPARENT", InnerEnum.PERPIXEL_TRANSPARENT);
			/// <summary>
			/// Represents support in the underlying system for windows all of
			/// the pixels of which have the same alpha value between or including
			/// 0.0 and 1.0.
			/// </summary>
			TRANSLUCENT,
			public static readonly WindowTranslucency TRANSLUCENT = new WindowTranslucency("TRANSLUCENT", InnerEnum.TRANSLUCENT);
			/// <summary>
			/// Represents support in the underlying system for windows that
			/// contain or might contain pixels with arbitrary alpha values
			/// between and including 0.0 and 1.0.
			/// </summary>
			PERPIXEL_TRANSLUCENT
			public static readonly WindowTranslucency PERPIXEL_TRANSLUCENT = new WindowTranslucency("PERPIXEL_TRANSLUCENT", InnerEnum.PERPIXEL_TRANSLUCENT);

			private static readonly IList<WindowTranslucency> valueList = new List<WindowTranslucency>();

			static WindowTranslucency()
			{
				valueList.Add(PERPIXEL_TRANSPARENT);
				valueList.Add(TRANSLUCENT);
				valueList.Add(PERPIXEL_TRANSLUCENT);
			}

			public enum InnerEnum
			{
				PERPIXEL_TRANSPARENT,
				TRANSLUCENT,
				PERPIXEL_TRANSLUCENT
			}

			private readonly string nameValue;
			private readonly int ordinalValue;
			private readonly InnerEnum innerEnumValue;
			private static int nextOrdinal = 0;

			public static IList<WindowTranslucency> values()
			{
				return valueList;
			}

			public InnerEnum InnerEnumValue()
			{
				return innerEnumValue;
			}

			public int ordinal()
			{
				return ordinalValue;
			}

			public override string ToString()
			{
				return nameValue;
			}

			public static WindowTranslucency valueOf(string name)
			{
				foreach (WindowTranslucency enumInstance in WindowTranslucency.values())
				{
					if (enumInstance.nameValue == name)
					{
						return enumInstance;
					}
				}
				throw new System.ArgumentException(name);
			}
		}

		/// <summary>
		/// Returns the type of this <code>GraphicsDevice</code>. </summary>
		/// <returns> the type of this <code>GraphicsDevice</code>, which can
		/// either be TYPE_RASTER_SCREEN, TYPE_PRINTER or TYPE_IMAGE_BUFFER. </returns>
		/// <seealso cref= #TYPE_RASTER_SCREEN </seealso>
		/// <seealso cref= #TYPE_PRINTER </seealso>
		/// <seealso cref= #TYPE_IMAGE_BUFFER </seealso>
		public abstract int Type {get;}

		/// <summary>
		/// Returns the identification string associated with this
		/// <code>GraphicsDevice</code>.
		/// <para>
		/// A particular program might use more than one
		/// <code>GraphicsDevice</code> in a <code>GraphicsEnvironment</code>.
		/// This method returns a <code>String</code> identifying a
		/// particular <code>GraphicsDevice</code> in the local
		/// <code>GraphicsEnvironment</code>.  Although there is
		/// no public method to set this <code>String</code>, a programmer can
		/// use the <code>String</code> for debugging purposes.  Vendors of
		/// the Java&trade; Runtime Environment can
		/// format the return value of the <code>String</code>.  To determine
		/// how to interpret the value of the <code>String</code>, contact the
		/// vendor of your Java Runtime.  To find out who the vendor is, from
		/// your program, call the
		/// <seealso cref="System#getProperty(String) getProperty"/> method of the
		/// System class with "java.vendor".
		/// </para>
		/// </summary>
		/// <returns> a <code>String</code> that is the identification
		/// of this <code>GraphicsDevice</code>. </returns>
		public abstract String IDstring {get;}

		/// <summary>
		/// Returns all of the <code>GraphicsConfiguration</code>
		/// objects associated with this <code>GraphicsDevice</code>. </summary>
		/// <returns> an array of <code>GraphicsConfiguration</code>
		/// objects that are associated with this
		/// <code>GraphicsDevice</code>. </returns>
		public abstract GraphicsConfiguration[] Configurations {get;}

		/// <summary>
		/// Returns the default <code>GraphicsConfiguration</code>
		/// associated with this <code>GraphicsDevice</code>. </summary>
		/// <returns> the default <code>GraphicsConfiguration</code>
		/// of this <code>GraphicsDevice</code>. </returns>
		public abstract GraphicsConfiguration DefaultConfiguration {get;}

		/// <summary>
		/// Returns the "best" configuration possible that passes the
		/// criteria defined in the <seealso cref="GraphicsConfigTemplate"/>. </summary>
		/// <param name="gct"> the <code>GraphicsConfigTemplate</code> object
		/// used to obtain a valid <code>GraphicsConfiguration</code> </param>
		/// <returns> a <code>GraphicsConfiguration</code> that passes
		/// the criteria defined in the specified
		/// <code>GraphicsConfigTemplate</code>. </returns>
		/// <seealso cref= GraphicsConfigTemplate </seealso>
		public virtual GraphicsConfiguration GetBestConfiguration(GraphicsConfigTemplate gct)
		{
			GraphicsConfiguration[] configs = Configurations;
			return gct.GetBestConfiguration(configs);
		}

		/// <summary>
		/// Returns <code>true</code> if this <code>GraphicsDevice</code>
		/// supports full-screen exclusive mode.
		/// If a SecurityManager is installed, its
		/// <code>checkPermission</code> method will be called
		/// with <code>AWTPermission("fullScreenExclusive")</code>.
		/// <code>isFullScreenSupported</code> returns true only if
		/// that permission is granted. </summary>
		/// <returns> whether full-screen exclusive mode is available for
		/// this graphics device </returns>
		/// <seealso cref= java.awt.AWTPermission
		/// @since 1.4 </seealso>
		public virtual bool FullScreenSupported
		{
			get
			{
				return false;
			}
		}

		/// <summary>
		/// Enter full-screen mode, or return to windowed mode.  The entered
		/// full-screen mode may be either exclusive or simulated.  Exclusive
		/// mode is only available if <code>isFullScreenSupported</code>
		/// returns <code>true</code>.
		/// <para>
		/// Exclusive mode implies:
		/// <ul>
		/// <li>Windows cannot overlap the full-screen window.  All other application
		/// windows will always appear beneath the full-screen window in the Z-order.
		/// <li>There can be only one full-screen window on a device at any time,
		/// so calling this method while there is an existing full-screen Window
		/// will cause the existing full-screen window to
		/// return to windowed mode.
		/// <li>Input method windows are disabled.  It is advisable to call
		/// <code>Component.enableInputMethods(false)</code> to make a component
		/// a non-client of the input method framework.
		/// </ul>
		/// </para>
		/// <para>
		/// The simulated full-screen mode places and resizes the window to the maximum
		/// possible visible area of the screen. However, the native windowing system
		/// may modify the requested geometry-related data, so that the {@code Window} object
		/// is placed and sized in a way that corresponds closely to the desktop settings.
		/// </para>
		/// <para>
		/// When entering full-screen mode, if the window to be used as a
		/// full-screen window is not visible, this method will make it visible.
		/// It will remain visible when returning to windowed mode.
		/// </para>
		/// <para>
		/// When entering full-screen mode, all the translucency effects are reset for
		/// the window. Its shape is set to {@code null}, the opacity value is set to
		/// 1.0f, and the background color alpha is set to 255 (completely opaque).
		/// These values are not restored when returning to windowed mode.
		/// </para>
		/// <para>
		/// It is unspecified and platform-dependent how decorated windows operate
		/// in full-screen mode. For this reason, it is recommended to turn off
		/// the decorations in a {@code Frame} or {@code Dialog} object by using the
		/// {@code setUndecorated} method.
		/// </para>
		/// <para>
		/// When returning to windowed mode from an exclusive full-screen window,
		/// any display changes made by calling {@code setDisplayMode} are
		/// automatically restored to their original state.
		/// 
		/// </para>
		/// </summary>
		/// <param name="w"> a window to use as the full-screen window; {@code null}
		/// if returning to windowed mode.  Some platforms expect the
		/// fullscreen window to be a top-level component (i.e., a {@code Frame});
		/// therefore it is preferable to use a {@code Frame} here rather than a
		/// {@code Window}.
		/// </param>
		/// <seealso cref= #isFullScreenSupported </seealso>
		/// <seealso cref= #getFullScreenWindow </seealso>
		/// <seealso cref= #setDisplayMode </seealso>
		/// <seealso cref= Component#enableInputMethods </seealso>
		/// <seealso cref= Component#setVisible </seealso>
		/// <seealso cref= Frame#setUndecorated </seealso>
		/// <seealso cref= Dialog#setUndecorated
		/// 
		/// @since 1.4 </seealso>
		public virtual Window FullScreenWindow
		{
			set
			{
				if (value != null)
				{
					if (value.Shape != null)
					{
						value.Shape = null;
					}
					if (value.Opacity < 1.0f)
					{
						value.Opacity = 1.0f;
					}
					if (!value.Opaque)
					{
						Color bgColor = value.Background;
						bgColor = new Color(bgColor.Red, bgColor.Green, bgColor.Blue, 255);
						value.Background = bgColor;
					}
					// Check if this window is in fullscreen mode on another device.
	//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
	//ORIGINAL LINE: final GraphicsConfiguration gc = value.getGraphicsConfiguration();
					GraphicsConfiguration gc = value.GraphicsConfiguration;
					if (gc != null && gc.Device != this && gc.Device.FullScreenWindow == value)
					{
						gc.Device.FullScreenWindow = null;
					}
				}
				if (FullScreenWindow_Renamed != null && WindowedModeBounds != null)
				{
					// if the window went into fs mode before it was realized it may
					// have (0,0) dimensions
					if (WindowedModeBounds.Width_Renamed == 0)
					{
						WindowedModeBounds.Width_Renamed = 1;
					}
					if (WindowedModeBounds.Height_Renamed == 0)
					{
						WindowedModeBounds.Height_Renamed = 1;
					}
					FullScreenWindow_Renamed.Bounds = WindowedModeBounds;
				}
				// Set the full screen window
				lock (FsAppContextLock)
				{
					// Associate fullscreen window with current AppContext
					if (value == null)
					{
						FullScreenAppContext = null;
					}
					else
					{
						FullScreenAppContext = AppContext.AppContext;
					}
					FullScreenWindow_Renamed = value;
				}
				if (FullScreenWindow_Renamed != null)
				{
					WindowedModeBounds = FullScreenWindow_Renamed.Bounds;
					// Note that we use the graphics configuration of the device,
					// not the window's, because we're setting the fs window for
					// this device.
	//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
	//ORIGINAL LINE: final GraphicsConfiguration gc = getDefaultConfiguration();
					GraphicsConfiguration gc = DefaultConfiguration;
	//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
	//ORIGINAL LINE: final Rectangle screenBounds = gc.getBounds();
					Rectangle screenBounds = gc.Bounds;
					if (SunToolkit.isDispatchThreadForAppContext(FullScreenWindow_Renamed))
					{
						// Update graphics configuration here directly and do not wait
						// asynchronous notification from the peer. Note that
						// setBounds() will reset a GC, if it was set incorrectly.
						FullScreenWindow_Renamed.GraphicsConfiguration = gc;
					}
					FullScreenWindow_Renamed.SetBounds(screenBounds.x, screenBounds.y, screenBounds.Width_Renamed, screenBounds.Height_Renamed);
					FullScreenWindow_Renamed.Visible = true;
					FullScreenWindow_Renamed.ToFront();
				}
			}
			get
			{
				Window returnWindow = null;
				lock (FsAppContextLock)
				{
					// Only return a handle to the current fs window if we are in the
					// same AppContext that set the fs window
					if (FullScreenAppContext == AppContext.AppContext)
					{
						returnWindow = FullScreenWindow_Renamed;
					}
				}
				return returnWindow;
			}
		}


		/// <summary>
		/// Returns <code>true</code> if this <code>GraphicsDevice</code>
		/// supports low-level display changes.
		/// On some platforms low-level display changes may only be allowed in
		/// full-screen exclusive mode (i.e., if <seealso cref="#isFullScreenSupported()"/>
		/// returns {@code true} and the application has already entered
		/// full-screen mode using <seealso cref="#setFullScreenWindow"/>). </summary>
		/// <returns> whether low-level display changes are supported for this
		/// graphics device. </returns>
		/// <seealso cref= #isFullScreenSupported </seealso>
		/// <seealso cref= #setDisplayMode </seealso>
		/// <seealso cref= #setFullScreenWindow
		/// @since 1.4 </seealso>
		public virtual bool DisplayChangeSupported
		{
			get
			{
				return false;
			}
		}

		/// <summary>
		/// Sets the display mode of this graphics device. This is only allowed
		/// if <seealso cref="#isDisplayChangeSupported()"/> returns {@code true} and may
		/// require first entering full-screen exclusive mode using
		/// <seealso cref="#setFullScreenWindow"/> providing that full-screen exclusive mode is
		/// supported (i.e., <seealso cref="#isFullScreenSupported()"/> returns
		/// {@code true}).
		/// <para>
		/// 
		/// The display mode must be one of the display modes returned by
		/// <seealso cref="#getDisplayModes()"/>, with one exception: passing a display mode
		/// with <seealso cref="DisplayMode#REFRESH_RATE_UNKNOWN"/> refresh rate will result in
		/// selecting a display mode from the list of available display modes with
		/// matching width, height and bit depth.
		/// However, passing a display mode with <seealso cref="DisplayMode#BIT_DEPTH_MULTI"/>
		/// for bit depth is only allowed if such mode exists in the list returned by
		/// <seealso cref="#getDisplayModes()"/>.
		/// </para>
		/// <para>
		/// Example code:
		/// <pre><code>
		/// Frame frame;
		/// DisplayMode newDisplayMode;
		/// GraphicsDevice gd;
		/// // create a Frame, select desired DisplayMode from the list of modes
		/// // returned by gd.getDisplayModes() ...
		/// 
		/// if (gd.isFullScreenSupported()) {
		///     gd.setFullScreenWindow(frame);
		/// } else {
		///    // proceed in non-full-screen mode
		///    frame.setSize(...);
		///    frame.setLocation(...);
		///    frame.setVisible(true);
		/// }
		/// 
		/// if (gd.isDisplayChangeSupported()) {
		///     gd.setDisplayMode(newDisplayMode);
		/// }
		/// </code></pre>
		/// 
		/// </para>
		/// </summary>
		/// <param name="dm"> The new display mode of this graphics device. </param>
		/// <exception cref="IllegalArgumentException"> if the <code>DisplayMode</code>
		/// supplied is <code>null</code>, or is not available in the array returned
		/// by <code>getDisplayModes</code> </exception>
		/// <exception cref="UnsupportedOperationException"> if
		/// <code>isDisplayChangeSupported</code> returns <code>false</code> </exception>
		/// <seealso cref= #getDisplayMode </seealso>
		/// <seealso cref= #getDisplayModes </seealso>
		/// <seealso cref= #isDisplayChangeSupported
		/// @since 1.4 </seealso>
		public virtual DisplayMode DisplayMode
		{
			set
			{
				throw new UnsupportedOperationException("Cannot change display mode");
			}
			get
			{
				GraphicsConfiguration gc = DefaultConfiguration;
				Rectangle r = gc.Bounds;
				ColorModel cm = gc.ColorModel;
				return new DisplayMode(r.Width_Renamed, r.Height_Renamed, cm.PixelSize, 0);
			}
		}


		/// <summary>
		/// Returns all display modes available for this
		/// <code>GraphicsDevice</code>.
		/// The returned display modes are allowed to have a refresh rate
		/// <seealso cref="DisplayMode#REFRESH_RATE_UNKNOWN"/> if it is indeterminate.
		/// Likewise, the returned display modes are allowed to have a bit depth
		/// <seealso cref="DisplayMode#BIT_DEPTH_MULTI"/> if it is indeterminate or if multiple
		/// bit depths are supported. </summary>
		/// <returns> all of the display modes available for this graphics device.
		/// @since 1.4 </returns>
		public virtual DisplayMode[] DisplayModes
		{
			get
			{
				return new DisplayMode[] {DisplayMode};
			}
		}

		/// <summary>
		/// This method returns the number of bytes available in
		/// accelerated memory on this device.
		/// Some images are created or cached
		/// in accelerated memory on a first-come,
		/// first-served basis.  On some operating systems,
		/// this memory is a finite resource.  Calling this method
		/// and scheduling the creation and flushing of images carefully may
		/// enable applications to make the most efficient use of
		/// that finite resource.
		/// <br>
		/// Note that the number returned is a snapshot of how much
		/// memory is available; some images may still have problems
		/// being allocated into that memory.  For example, depending
		/// on operating system, driver, memory configuration, and
		/// thread situations, the full extent of the size reported
		/// may not be available for a given image.  There are further
		/// inquiry methods on the <seealso cref="ImageCapabilities"/> object
		/// associated with a VolatileImage that can be used to determine
		/// whether a particular VolatileImage has been created in accelerated
		/// memory. </summary>
		/// <returns> number of bytes available in accelerated memory.
		/// A negative return value indicates that the amount of accelerated memory
		/// on this GraphicsDevice is indeterminate. </returns>
		/// <seealso cref= java.awt.image.VolatileImage#flush </seealso>
		/// <seealso cref= ImageCapabilities#isAccelerated
		/// @since 1.4 </seealso>
		public virtual int AvailableAcceleratedMemory
		{
			get
			{
				return -1;
			}
		}

		/// <summary>
		/// Returns whether the given level of translucency is supported by
		/// this graphics device.
		/// </summary>
		/// <param name="translucencyKind"> a kind of translucency support </param>
		/// <returns> whether the given translucency kind is supported
		/// 
		/// @since 1.7 </returns>
		public virtual bool IsWindowTranslucencySupported(WindowTranslucency translucencyKind)
		{
			switch (translucencyKind.InnerEnumValue())
			{
				case java.awt.GraphicsDevice.WindowTranslucency.InnerEnum.PERPIXEL_TRANSPARENT:
					return WindowShapingSupported;
				case java.awt.GraphicsDevice.WindowTranslucency.InnerEnum.TRANSLUCENT:
					return WindowOpacitySupported;
				case java.awt.GraphicsDevice.WindowTranslucency.InnerEnum.PERPIXEL_TRANSLUCENT:
					return WindowPerpixelTranslucencySupported;
			}
			return false;
		}

		/// <summary>
		/// Returns whether the windowing system supports changing the shape
		/// of top-level windows.
		/// Note that this method may sometimes return true, but the native
		/// windowing system may still not support the concept of
		/// shaping (due to the bugs in the windowing system).
		/// </summary>
		internal static bool WindowShapingSupported
		{
			get
			{
				Toolkit curToolkit = Toolkit.DefaultToolkit;
				if (!(curToolkit is SunToolkit))
				{
					return false;
				}
				return ((SunToolkit)curToolkit).WindowShapingSupported;
			}
		}

		/// <summary>
		/// Returns whether the windowing system supports changing the opacity
		/// value of top-level windows.
		/// Note that this method may sometimes return true, but the native
		/// windowing system may still not support the concept of
		/// translucency (due to the bugs in the windowing system).
		/// </summary>
		internal static bool WindowOpacitySupported
		{
			get
			{
				Toolkit curToolkit = Toolkit.DefaultToolkit;
				if (!(curToolkit is SunToolkit))
				{
					return false;
				}
				return ((SunToolkit)curToolkit).WindowOpacitySupported;
			}
		}

		internal virtual bool WindowPerpixelTranslucencySupported
		{
			get
			{
				/*
				 * Per-pixel alpha is supported if all the conditions are TRUE:
				 *    1. The toolkit is a sort of SunToolkit
				 *    2. The toolkit supports translucency in general
				 *        (isWindowTranslucencySupported())
				 *    3. There's at least one translucency-capable
				 *        GraphicsConfiguration
				 */
				Toolkit curToolkit = Toolkit.DefaultToolkit;
				if (!(curToolkit is SunToolkit))
				{
					return false;
				}
				if (!((SunToolkit)curToolkit).WindowTranslucencySupported)
				{
					return false;
				}
    
				// TODO: cache translucency capable GC
				return TranslucencyCapableGC != null;
			}
		}

		internal virtual GraphicsConfiguration TranslucencyCapableGC
		{
			get
			{
				// If the default GC supports translucency return true.
				// It is important to optimize the verification this way,
				// see CR 6661196 for more details.
				GraphicsConfiguration defaultGC = DefaultConfiguration;
				if (defaultGC.TranslucencyCapable)
				{
					return defaultGC;
				}
    
				// ... otherwise iterate through all the GCs.
				GraphicsConfiguration[] configs = Configurations;
				for (int j = 0; j < configs.Length; j++)
				{
					if (configs[j].TranslucencyCapable)
					{
						return configs[j];
					}
				}
    
				return null;
			}
		}
	}

}
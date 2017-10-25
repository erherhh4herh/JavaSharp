using System;

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


	using FontManager = sun.font.FontManager;
	using FontManagerFactory = sun.font.FontManagerFactory;
	using HeadlessGraphicsEnvironment = sun.java2d.HeadlessGraphicsEnvironment;
	using SunGraphicsEnvironment = sun.java2d.SunGraphicsEnvironment;
	using GetPropertyAction = sun.security.action.GetPropertyAction;

	/// 
	/// <summary>
	/// The <code>GraphicsEnvironment</code> class describes the collection
	/// of <seealso cref="GraphicsDevice"/> objects and <seealso cref="java.awt.Font"/> objects
	/// available to a Java(tm) application on a particular platform.
	/// The resources in this <code>GraphicsEnvironment</code> might be local
	/// or on a remote machine.  <code>GraphicsDevice</code> objects can be
	/// screens, printers or image buffers and are the destination of
	/// <seealso cref="Graphics2D"/> drawing methods.  Each <code>GraphicsDevice</code>
	/// has a number of <seealso cref="GraphicsConfiguration"/> objects associated with
	/// it.  These objects specify the different configurations in which the
	/// <code>GraphicsDevice</code> can be used. </summary>
	/// <seealso cref= GraphicsDevice </seealso>
	/// <seealso cref= GraphicsConfiguration </seealso>

	public abstract class GraphicsEnvironment
	{
		private static GraphicsEnvironment LocalEnv;

		/// <summary>
		/// The headless state of the Toolkit and GraphicsEnvironment
		/// </summary>
		private static Boolean Headless_Renamed;

		/// <summary>
		/// The headless state assumed by default
		/// </summary>
		private static Boolean DefaultHeadless;

		/// <summary>
		/// This is an abstract class and cannot be instantiated directly.
		/// Instances must be obtained from a suitable factory or query method.
		/// </summary>
		protected internal GraphicsEnvironment()
		{
		}

		/// <summary>
		/// Returns the local <code>GraphicsEnvironment</code>. </summary>
		/// <returns> the local <code>GraphicsEnvironment</code> </returns>
		public static GraphicsEnvironment LocalGraphicsEnvironment
		{
			get
			{
				lock (typeof(GraphicsEnvironment))
				{
					if (LocalEnv == null)
					{
						LocalEnv = CreateGE();
					}
            
					return LocalEnv;
				}
			}
		}

		/// <summary>
		/// Creates and returns the GraphicsEnvironment, according to the
		/// system property 'java.awt.graphicsenv'.
		/// </summary>
		/// <returns> the graphics environment </returns>
		private static GraphicsEnvironment CreateGE()
		{
			GraphicsEnvironment ge;
			String nm = AccessController.doPrivileged(new GetPropertyAction("java.awt.graphicsenv", null));
			try
			{
	//          long t0 = System.currentTimeMillis();
				Class geCls;
				try
				{
					// First we try if the bootclassloader finds the requested
					// class. This way we can avoid to run in a privileged block.
					geCls = (Class)Class.ForName(nm);
				}
				catch (ClassNotFoundException)
				{
					// If the bootclassloader fails, we try again with the
					// application classloader.
					ClassLoader cl = ClassLoader.SystemClassLoader;
					geCls = (Class)Class.ForName(nm, true, cl);
				}
				ge = geCls.NewInstance();
	//          long t1 = System.currentTimeMillis();
	//          System.out.println("GE creation took " + (t1-t0)+ "ms.");
				if (Headless)
				{
					ge = new HeadlessGraphicsEnvironment(ge);
				}
			}
			catch (ClassNotFoundException)
			{
				throw new Error("Could not find class: " + nm);
			}
			catch (InstantiationException)
			{
				throw new Error("Could not instantiate Graphics Environment: " + nm);
			}
			catch (IllegalAccessException)
			{
				throw new Error("Could not access Graphics Environment: " + nm);
			}
			return ge;
		}

		/// <summary>
		/// Tests whether or not a display, keyboard, and mouse can be
		/// supported in this environment.  If this method returns true,
		/// a HeadlessException is thrown from areas of the Toolkit
		/// and GraphicsEnvironment that are dependent on a display,
		/// keyboard, or mouse. </summary>
		/// <returns> <code>true</code> if this environment cannot support
		/// a display, keyboard, and mouse; <code>false</code>
		/// otherwise </returns>
		/// <seealso cref= java.awt.HeadlessException
		/// @since 1.4 </seealso>
		public static bool Headless
		{
			get
			{
				return HeadlessProperty;
			}
		}

		/// <returns> warning message if headless state is assumed by default;
		/// null otherwise
		/// @since 1.5 </returns>
		internal static String HeadlessMessage
		{
			get
			{
				if (Headless_Renamed == null)
				{
					HeadlessProperty; // initialize the values
				}
				return DefaultHeadless != true ? null : "\nNo X11 DISPLAY variable was set, " + "but this program performed an operation which requires it.";
			}
		}

		/// <returns> the value of the property "java.awt.headless"
		/// @since 1.4 </returns>
		private static bool HeadlessProperty
		{
			get
			{
				if (Headless_Renamed == null)
				{
//JAVA TO C# CONVERTER TODO TASK: Java lambdas satisfy functional interfaces, while .NET lambdas satisfy delegates - change the appropriate interface to a delegate:
					AccessController.doPrivileged((PrivilegedAction<Void>)() =>
							/* No need to ask for DISPLAY when run in a browser */
					{
						String nm = System.getProperty("java.awt.headless");
						if (nm == null)
						{
							if (System.getProperty("javaplugin.version") != null)
							{
								Headless_Renamed = DefaultHeadless = false;
							}
							else
							{
								String osName = System.getProperty("os.name");
								if (osName.Contains("OS X") && "sun.awt.HToolkit".Equals(System.getProperty("awt.toolkit")))
								{
									Headless_Renamed = DefaultHeadless = true;
								}
								else
								{
	//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
	//ORIGINAL LINE: final String display = System.getenv("DISPLAY");
									String display = System.getenv("DISPLAY");
									Headless_Renamed = DefaultHeadless = ("Linux".Equals(osName) || "SunOS".Equals(osName) || "FreeBSD".Equals(osName) || "NetBSD".Equals(osName) || "OpenBSD".Equals(osName) || "AIX".Equals(osName)) && (display == null || display.Trim().Length == 0);
								}
							}
						}
						else
						{
							Headless_Renamed = Convert.ToBoolean(nm);
						}
						return null;
					});
				}
				return Headless_Renamed;
			}
		}

		/// <summary>
		/// Check for headless state and throw HeadlessException if headless
		/// @since 1.4
		/// </summary>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: static void checkHeadless() throws HeadlessException
		internal static void CheckHeadless()
		{
			if (Headless)
			{
				throw new HeadlessException();
			}
		}

		/// <summary>
		/// Returns whether or not a display, keyboard, and mouse can be
		/// supported in this graphics environment.  If this returns true,
		/// <code>HeadlessException</code> will be thrown from areas of the
		/// graphics environment that are dependent on a display, keyboard, or
		/// mouse. </summary>
		/// <returns> <code>true</code> if a display, keyboard, and mouse
		/// can be supported in this environment; <code>false</code>
		/// otherwise </returns>
		/// <seealso cref= java.awt.HeadlessException </seealso>
		/// <seealso cref= #isHeadless
		/// @since 1.4 </seealso>
		public virtual bool HeadlessInstance
		{
			get
			{
				// By default (local graphics environment), simply check the
				// headless property.
				return HeadlessProperty;
			}
		}

		/// <summary>
		/// Returns an array of all of the screen <code>GraphicsDevice</code>
		/// objects. </summary>
		/// <returns> an array containing all the <code>GraphicsDevice</code>
		/// objects that represent screen devices </returns>
		/// <exception cref="HeadlessException"> if isHeadless() returns true </exception>
		/// <seealso cref= #isHeadless() </seealso>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public abstract GraphicsDevice[] getScreenDevices() throws HeadlessException;
		public abstract GraphicsDevice[] ScreenDevices {get;}

		/// <summary>
		/// Returns the default screen <code>GraphicsDevice</code>. </summary>
		/// <returns> the <code>GraphicsDevice</code> that represents the
		/// default screen device </returns>
		/// <exception cref="HeadlessException"> if isHeadless() returns true </exception>
		/// <seealso cref= #isHeadless() </seealso>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public abstract GraphicsDevice getDefaultScreenDevice() throws HeadlessException;
		public abstract GraphicsDevice DefaultScreenDevice {get;}

		/// <summary>
		/// Returns a <code>Graphics2D</code> object for rendering into the
		/// specified <seealso cref="BufferedImage"/>. </summary>
		/// <param name="img"> the specified <code>BufferedImage</code> </param>
		/// <returns> a <code>Graphics2D</code> to be used for rendering into
		/// the specified <code>BufferedImage</code> </returns>
		/// <exception cref="NullPointerException"> if <code>img</code> is null </exception>
		public abstract Graphics2D CreateGraphics(BufferedImage img);

		/// <summary>
		/// Returns an array containing a one-point size instance of all fonts
		/// available in this <code>GraphicsEnvironment</code>.  Typical usage
		/// would be to allow a user to select a particular font.  Then, the
		/// application can size the font and set various font attributes by
		/// calling the <code>deriveFont</code> method on the chosen instance.
		/// <para>
		/// This method provides for the application the most precise control
		/// over which <code>Font</code> instance is used to render text.
		/// If a font in this <code>GraphicsEnvironment</code> has multiple
		/// programmable variations, only one
		/// instance of that <code>Font</code> is returned in the array, and
		/// other variations must be derived by the application.
		/// </para>
		/// <para>
		/// If a font in this environment has multiple programmable variations,
		/// such as Multiple-Master fonts, only one instance of that font is
		/// returned in the <code>Font</code> array.  The other variations
		/// must be derived by the application.
		/// 
		/// </para>
		/// </summary>
		/// <returns> an array of <code>Font</code> objects </returns>
		/// <seealso cref= #getAvailableFontFamilyNames </seealso>
		/// <seealso cref= java.awt.Font </seealso>
		/// <seealso cref= java.awt.Font#deriveFont </seealso>
		/// <seealso cref= java.awt.Font#getFontName
		/// @since 1.2 </seealso>
		public abstract Font[] AllFonts {get;}

		/// <summary>
		/// Returns an array containing the names of all font families in this
		/// <code>GraphicsEnvironment</code> localized for the default locale,
		/// as returned by <code>Locale.getDefault()</code>.
		/// <para>
		/// Typical usage would be for presentation to a user for selection of
		/// a particular family name. An application can then specify this name
		/// when creating a font, in conjunction with a style, such as bold or
		/// italic, giving the font system flexibility in choosing its own best
		/// match among multiple fonts in the same font family.
		/// 
		/// </para>
		/// </summary>
		/// <returns> an array of <code>String</code> containing font family names
		/// localized for the default locale, or a suitable alternative
		/// name if no name exists for this locale. </returns>
		/// <seealso cref= #getAllFonts </seealso>
		/// <seealso cref= java.awt.Font </seealso>
		/// <seealso cref= java.awt.Font#getFamily
		/// @since 1.2 </seealso>
		public abstract String[] AvailableFontFamilyNames {get;}

		/// <summary>
		/// Returns an array containing the names of all font families in this
		/// <code>GraphicsEnvironment</code> localized for the specified locale.
		/// <para>
		/// Typical usage would be for presentation to a user for selection of
		/// a particular family name. An application can then specify this name
		/// when creating a font, in conjunction with a style, such as bold or
		/// italic, giving the font system flexibility in choosing its own best
		/// match among multiple fonts in the same font family.
		/// 
		/// </para>
		/// </summary>
		/// <param name="l"> a <seealso cref="Locale"/> object that represents a
		/// particular geographical, political, or cultural region.
		/// Specifying <code>null</code> is equivalent to
		/// specifying <code>Locale.getDefault()</code>. </param>
		/// <returns> an array of <code>String</code> containing font family names
		/// localized for the specified <code>Locale</code>, or a
		/// suitable alternative name if no name exists for the specified locale. </returns>
		/// <seealso cref= #getAllFonts </seealso>
		/// <seealso cref= java.awt.Font </seealso>
		/// <seealso cref= java.awt.Font#getFamily
		/// @since 1.2 </seealso>
		public abstract String[] GetAvailableFontFamilyNames(Locale l);

		/// <summary>
		/// Registers a <i>created</i> <code>Font</code>in this
		/// <code>GraphicsEnvironment</code>.
		/// A created font is one that was returned from calling
		/// <seealso cref="Font#createFont"/>, or derived from a created font by
		/// calling <seealso cref="Font#deriveFont"/>.
		/// After calling this method for such a font, it is available to
		/// be used in constructing new <code>Font</code>s by name or family name,
		/// and is enumerated by <seealso cref="#getAvailableFontFamilyNames"/> and
		/// <seealso cref="#getAllFonts"/> within the execution context of this
		/// application or applet. This means applets cannot register fonts in
		/// a way that they are visible to other applets.
		/// <para>
		/// Reasons that this method might not register the font and therefore
		/// return <code>false</code> are:
		/// <ul>
		/// <li>The font is not a <i>created</i> <code>Font</code>.
		/// <li>The font conflicts with a non-created <code>Font</code> already
		/// in this <code>GraphicsEnvironment</code>. For example if the name
		/// is that of a system font, or a logical font as described in the
		/// documentation of the <seealso cref="Font"/> class. It is implementation dependent
		/// whether a font may also conflict if it has the same family name
		/// as a system font.
		/// </para>
		/// <para>Notice that an application can supersede the registration
		/// of an earlier created font with a new one.
		/// </ul>
		/// </para>
		/// </summary>
		/// <returns> true if the <code>font</code> is successfully
		/// registered in this <code>GraphicsEnvironment</code>. </returns>
		/// <exception cref="NullPointerException"> if <code>font</code> is null
		/// @since 1.6 </exception>
		public virtual bool RegisterFont(Font font)
		{
			if (font == null)
			{
				throw new NullPointerException("font cannot be null.");
			}
			FontManager fm = FontManagerFactory.Instance;
			return fm.registerFont(font);
		}

		/// <summary>
		/// Indicates a preference for locale-specific fonts in the mapping of
		/// logical fonts to physical fonts. Calling this method indicates that font
		/// rendering should primarily use fonts specific to the primary writing
		/// system (the one indicated by the default encoding and the initial
		/// default locale). For example, if the primary writing system is
		/// Japanese, then characters should be rendered using a Japanese font
		/// if possible, and other fonts should only be used for characters for
		/// which the Japanese font doesn't have glyphs.
		/// <para>
		/// The actual change in font rendering behavior resulting from a call
		/// to this method is implementation dependent; it may have no effect at
		/// all, or the requested behavior may already match the default behavior.
		/// The behavior may differ between font rendering in lightweight
		/// and peered components.  Since calling this method requests a
		/// different font, clients should expect different metrics, and may need
		/// to recalculate window sizes and layout. Therefore this method should
		/// be called before user interface initialisation.
		/// @since 1.5
		/// </para>
		/// </summary>
		public virtual void PreferLocaleFonts()
		{
			FontManager fm = FontManagerFactory.Instance;
			fm.preferLocaleFonts();
		}

		/// <summary>
		/// Indicates a preference for proportional over non-proportional (e.g.
		/// dual-spaced CJK fonts) fonts in the mapping of logical fonts to
		/// physical fonts. If the default mapping contains fonts for which
		/// proportional and non-proportional variants exist, then calling
		/// this method indicates the mapping should use a proportional variant.
		/// <para>
		/// The actual change in font rendering behavior resulting from a call to
		/// this method is implementation dependent; it may have no effect at all.
		/// The behavior may differ between font rendering in lightweight and
		/// peered components. Since calling this method requests a
		/// different font, clients should expect different metrics, and may need
		/// to recalculate window sizes and layout. Therefore this method should
		/// be called before user interface initialisation.
		/// @since 1.5
		/// </para>
		/// </summary>
		public virtual void PreferProportionalFonts()
		{
			FontManager fm = FontManagerFactory.Instance;
			fm.preferProportionalFonts();
		}

		/// <summary>
		/// Returns the Point where Windows should be centered.
		/// It is recommended that centered Windows be checked to ensure they fit
		/// within the available display area using getMaximumWindowBounds(). </summary>
		/// <returns> the point where Windows should be centered
		/// </returns>
		/// <exception cref="HeadlessException"> if isHeadless() returns true </exception>
		/// <seealso cref= #getMaximumWindowBounds
		/// @since 1.4 </seealso>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public Point getCenterPoint() throws HeadlessException
		public virtual Point CenterPoint
		{
			get
			{
			// Default implementation: return the center of the usable bounds of the
			// default screen device.
				Rectangle usableBounds = SunGraphicsEnvironment.getUsableBounds(DefaultScreenDevice);
				return new Point((usableBounds.Width_Renamed / 2) + usableBounds.x, (usableBounds.Height_Renamed / 2) + usableBounds.y);
			}
		}

		/// <summary>
		/// Returns the maximum bounds for centered Windows.
		/// These bounds account for objects in the native windowing system such as
		/// task bars and menu bars.  The returned bounds will reside on a single
		/// display with one exception: on multi-screen systems where Windows should
		/// be centered across all displays, this method returns the bounds of the
		/// entire display area.
		/// <para>
		/// To get the usable bounds of a single display, use
		/// <code>GraphicsConfiguration.getBounds()</code> and
		/// <code>Toolkit.getScreenInsets()</code>.
		/// </para>
		/// </summary>
		/// <returns>  the maximum bounds for centered Windows
		/// </returns>
		/// <exception cref="HeadlessException"> if isHeadless() returns true </exception>
		/// <seealso cref= #getCenterPoint </seealso>
		/// <seealso cref= GraphicsConfiguration#getBounds </seealso>
		/// <seealso cref= Toolkit#getScreenInsets
		/// @since 1.4 </seealso>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public Rectangle getMaximumWindowBounds() throws HeadlessException
		public virtual Rectangle MaximumWindowBounds
		{
			get
			{
			// Default implementation: return the usable bounds of the default screen
			// device.  This is correct for Microsoft Windows and non-Xinerama X11.
				return SunGraphicsEnvironment.getUsableBounds(DefaultScreenDevice);
			}
		}
	}

}
using System.Diagnostics;
using System.Runtime.InteropServices;

/*
 * Copyright (c) 2005, 2013, Oracle and/or its affiliates. All rights reserved.
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

	using PlatformLogger = sun.util.logging.PlatformLogger;
	using SunWritableRaster = sun.awt.image.SunWritableRaster;

	/// <summary>
	/// The splash screen can be displayed at application startup, before the
	/// Java Virtual Machine (JVM) starts. The splash screen is displayed as an
	/// undecorated window containing an image. You can use GIF, JPEG, or PNG files
	/// for the image. Animation is supported for the GIF format, while transparency
	/// is supported both for GIF and PNG.  The window is positioned at the center
	/// of the screen. The position on multi-monitor systems is not specified. It is
	/// platform and implementation dependent.  The splash screen window is closed
	/// automatically as soon as the first window is displayed by Swing/AWT (may be
	/// also closed manually using the Java API, see below).
	/// <P>
	/// If your application is packaged in a jar file, you can use the
	/// "SplashScreen-Image" option in a manifest file to show a splash screen.
	/// Place the image in the jar archive and specify the path in the option.
	/// The path should not have a leading slash.
	/// <BR>
	/// For example, in the <code>manifest.mf</code> file:
	/// <PRE>
	/// Manifest-Version: 1.0
	/// Main-Class: Test
	/// SplashScreen-Image: filename.gif
	/// </PRE>
	/// <P>
	/// If the Java implementation provides the command-line interface and you run
	/// your application by using the command line or a shortcut, use the Java
	/// application launcher option to show a splash screen. The Oracle reference
	/// implementation allows you to specify the splash screen image location with
	/// the {@code -splash:} option.
	/// <BR>
	/// For example:
	/// <PRE>
	/// java -splash:filename.gif Test
	/// </PRE>
	/// The command line interface has higher precedence over the manifest
	/// setting.
	/// <para>
	/// The splash screen will be displayed as faithfully as possible to present the
	/// whole splash screen image given the limitations of the target platform and
	/// display.
	/// </para>
	/// <para>
	/// It is implied that the specified image is presented on the screen "as is",
	/// i.e. preserving the exact color values as specified in the image file. Under
	/// certain circumstances, though, the presented image may differ, e.g. when
	/// applying color dithering to present a 32 bits per pixel (bpp) image on a 16
	/// or 8 bpp screen. The native platform display configuration may also affect
	/// the colors of the displayed image (e.g.  color profiles, etc.)
	/// </para>
	/// <para>
	/// The {@code SplashScreen} class provides the API for controlling the splash
	/// screen. This class may be used to close the splash screen, change the splash
	/// screen image, get the splash screen native window position/size, and paint
	/// in the splash screen. It cannot be used to create the splash screen. You
	/// should use the options provided by the Java implementation for that.
	/// </para>
	/// <para>
	/// This class cannot be instantiated. Only a single instance of this class
	/// can exist, and it may be obtained by using the <seealso cref="#getSplashScreen()"/>
	/// static method. In case the splash screen has not been created at
	/// application startup via the command line or manifest file option,
	/// the <code>getSplashScreen</code> method returns <code>null</code>.
	/// 
	/// @author Oleg Semenov
	/// @since 1.6
	/// </para>
	/// </summary>
	public sealed class SplashScreen
	{

		internal SplashScreen(long ptr) // non-public constructor
		{
			SplashPtr = ptr;
		}

		/// <summary>
		/// Returns the {@code SplashScreen} object used for
		/// Java startup splash screen control on systems that support display.
		/// </summary>
		/// <exception cref="UnsupportedOperationException"> if the splash screen feature is not
		///         supported by the current toolkit </exception>
		/// <exception cref="HeadlessException"> if {@code GraphicsEnvironment.isHeadless()}
		///         returns true </exception>
		/// <returns> the <seealso cref="SplashScreen"/> instance, or <code>null</code> if there is
		///         none or it has already been closed </returns>
		public static SplashScreen SplashScreen
		{
			get
			{
				lock (typeof(SplashScreen))
				{
					if (GraphicsEnvironment.Headless)
					{
						throw new HeadlessException();
					}
					// SplashScreen class is now a singleton
					if (!WasClosed && TheInstance == null)
					{
						java.security.AccessController.doPrivileged(new PrivilegedActionAnonymousInnerClassHelper());
						long ptr = _getInstance();
						if (ptr != 0 && _isVisible(ptr))
						{
							TheInstance = new SplashScreen(ptr);
						}
					}
					return TheInstance;
				}
			}
		}

		private class PrivilegedActionAnonymousInnerClassHelper : java.security.PrivilegedAction<Void>
		{
			public PrivilegedActionAnonymousInnerClassHelper()
			{
			}

			public virtual Void Run()
			{
//JAVA TO C# CONVERTER TODO TASK: The library is specified in the 'DllImport' attribute for .NET:
//				System.loadLibrary("splashscreen");
				return null;
			}
		}

		/// <summary>
		/// Changes the splash screen image. The new image is loaded from the
		/// specified URL; GIF, JPEG and PNG image formats are supported.
		/// The method returns after the image has finished loading and the window
		/// has been updated.
		/// The splash screen window is resized according to the size of
		/// the image and is centered on the screen.
		/// </summary>
		/// <param name="imageURL"> the non-<code>null</code> URL for the new
		///        splash screen image </param>
		/// <exception cref="NullPointerException"> if {@code imageURL} is <code>null</code> </exception>
		/// <exception cref="IOException"> if there was an error while loading the image </exception>
		/// <exception cref="IllegalStateException"> if the splash screen has already been
		///         closed </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public void setImageURL(java.net.URL imageURL) throws NullPointerException, java.io.IOException, IllegalStateException
		public URL ImageURL
		{
			set
			{
				CheckVisible();
				URLConnection connection = value.OpenConnection();
				connection.Connect();
				int length = connection.ContentLength;
				java.io.InputStream stream = connection.InputStream;
				sbyte[] buf = new sbyte[length];
				int off = 0;
				while (true)
				{
					// check for available data
					int available = stream.Available();
					if (available <= 0)
					{
						// no data available... well, let's try reading one byte
						// we'll see what happens then
						available = 1;
					}
					// check for enough room in buffer, realloc if needed
					// the buffer always grows in size 2x minimum
					if (off + available > length)
					{
						length = off * 2;
						if (off + available > length)
						{
							length = available + off;
						}
						sbyte[] oldBuf = buf;
						buf = new sbyte[length];
						System.Array.Copy(oldBuf, 0, buf, 0, off);
					}
					// now read the data
					int result = stream.Read(buf, off, available);
					if (result < 0)
					{
						break;
					}
					off += result;
				}
				lock (typeof(SplashScreen))
				{
					CheckVisible();
					if (!_setImageData(SplashPtr, buf))
					{
						throw new IOException("Bad image format or i/o error when loading image");
					}
					this.ImageURL_Renamed = value;
				}
			}
			get
			{
				lock (typeof(SplashScreen))
				{
					CheckVisible();
					if (ImageURL_Renamed == null)
					{
						try
						{
							String fileName = _getImageFileName(SplashPtr);
							String jarName = _getImageJarName(SplashPtr);
							if (fileName != null)
							{
								if (jarName != null)
								{
									ImageURL_Renamed = new URL("jar:" + ((new File(jarName)).toURL().ToString()) + "!/" + fileName);
								}
								else
								{
									ImageURL_Renamed = (new File(fileName)).toURL();
								}
							}
						}
						catch (java.net.MalformedURLException e)
						{
							if (Log.isLoggable(PlatformLogger.Level.FINE))
							{
								Log.fine("MalformedURLException caught in the getImageURL() method", e);
							}
						}
					}
					return ImageURL_Renamed;
				}
			}
		}

		private void CheckVisible()
		{
			if (!Visible)
			{
				throw new IllegalStateException("no splash screen available");
			}
		}

		/// <summary>
		/// Returns the bounds of the splash screen window as a <seealso cref="Rectangle"/>.
		/// This may be useful if, for example, you want to replace the splash
		/// screen with your window at the same location.
		/// <para>
		/// You cannot control the size or position of the splash screen.
		/// The splash screen size is adjusted automatically when the image changes.
		/// </para>
		/// <para>
		/// The image may contain transparent areas, and thus the reported bounds may
		/// be larger than the visible splash screen image on the screen.
		/// 
		/// </para>
		/// </summary>
		/// <returns> a {@code Rectangle} containing the splash screen bounds </returns>
		/// <exception cref="IllegalStateException"> if the splash screen has already been closed </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public Rectangle getBounds() throws IllegalStateException
		public Rectangle Bounds
		{
			get
			{
				lock (typeof(SplashScreen))
				{
					CheckVisible();
					float scale = _getScaleFactor(SplashPtr);
					Rectangle bounds = _getBounds(SplashPtr);
					Debug.Assert(scale > 0);
					if (scale > 0 && scale != 1)
					{
						bounds.SetSize((int)(bounds.Width / scale), (int)(bounds.Width / scale));
					}
					return bounds;
				}
			}
		}

		/// <summary>
		/// Returns the size of the splash screen window as a <seealso cref="Dimension"/>.
		/// This may be useful if, for example,
		/// you want to draw on the splash screen overlay surface.
		/// <para>
		/// You cannot control the size or position of the splash screen.
		/// The splash screen size is adjusted automatically when the image changes.
		/// </para>
		/// <para>
		/// The image may contain transparent areas, and thus the reported size may
		/// be larger than the visible splash screen image on the screen.
		/// 
		/// </para>
		/// </summary>
		/// <returns> a <seealso cref="Dimension"/> object indicating the splash screen size </returns>
		/// <exception cref="IllegalStateException"> if the splash screen has already been closed </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public Dimension getSize() throws IllegalStateException
		public Dimension Size
		{
			get
			{
				return Bounds.Size;
			}
		}

		/// <summary>
		/// Creates a graphics context (as a <seealso cref="Graphics2D"/> object) for the splash
		/// screen overlay image, which allows you to draw over the splash screen.
		/// Note that you do not draw on the main image but on the image that is
		/// displayed over the main image using alpha blending. Also note that drawing
		/// on the overlay image does not necessarily update the contents of splash
		/// screen window. You should call {@code update()} on the
		/// <code>SplashScreen</code> when you want the splash screen to be
		/// updated immediately.
		/// <para>
		/// The pixel (0, 0) in the coordinate space of the graphics context
		/// corresponds to the origin of the splash screen native window bounds (see
		/// <seealso cref="#getBounds()"/>).
		/// 
		/// </para>
		/// </summary>
		/// <returns> graphics context for the splash screen overlay surface </returns>
		/// <exception cref="IllegalStateException"> if the splash screen has already been closed </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public Graphics2D createGraphics() throws IllegalStateException
		public Graphics2D CreateGraphics()
		{
			lock (typeof(SplashScreen))
			{
				CheckVisible();
				if (Image == null)
				{
					// get unscaled splash image size
					Dimension dim = _getBounds(SplashPtr).Size;
					Image = new BufferedImage(dim.Width_Renamed, dim.Height_Renamed, BufferedImage.TYPE_INT_ARGB);
				}
				float scale = _getScaleFactor(SplashPtr);
				Graphics2D g = Image.CreateGraphics();
				assert(scale > 0);
				if (scale <= 0)
				{
					scale = 1;
				}
				g.Scale(scale, scale);
				return g;
			}
		}

		/// <summary>
		/// Updates the splash window with current contents of the overlay image.
		/// </summary>
		/// <exception cref="IllegalStateException"> if the overlay image does not exist;
		///         for example, if {@code createGraphics} has never been called,
		///         or if the splash screen has already been closed </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public void update() throws IllegalStateException
		public void Update()
		{
			BufferedImage image;
			lock (typeof(SplashScreen))
			{
				CheckVisible();
				image = this.Image;
			}
			if (image == null)
			{
				throw new IllegalStateException("no overlay image available");
			}
			DataBuffer buf = image.Raster.DataBuffer;
			if (!(buf is DataBufferInt))
			{
//JAVA TO C# CONVERTER WARNING: The .NET Type.FullName property will not always yield results identical to the Java Class.getName method:
				throw new AssertionError("Overlay image DataBuffer is of invalid type == " + buf.GetType().FullName);
			}
			int numBanks = buf.NumBanks;
			if (numBanks != 1)
			{
				throw new AssertionError("Invalid number of banks ==" + numBanks + " in overlay image DataBuffer");
			}
			if (!(image.SampleModel is SinglePixelPackedSampleModel))
			{
//JAVA TO C# CONVERTER WARNING: The .NET Type.FullName property will not always yield results identical to the Java Class.getName method:
				throw new AssertionError("Overlay image has invalid sample model == " + image.SampleModel.GetType().FullName);
			}
			SinglePixelPackedSampleModel sm = (SinglePixelPackedSampleModel)image.SampleModel;
			int scanlineStride = sm.ScanlineStride;
			Rectangle rect = image.Raster.Bounds;
			// Note that we steal the data array here, but just for reading
			// so we do not need to mark the DataBuffer dirty...
			int[] data = SunWritableRaster.stealData((DataBufferInt)buf, 0);
			lock (typeof(SplashScreen))
			{
				CheckVisible();
				_update(SplashPtr, data, rect.x, rect.y, rect.Width_Renamed, rect.Height_Renamed, scanlineStride);
			}
		}

		/// <summary>
		/// Hides the splash screen, closes the window, and releases all associated
		/// resources.
		/// </summary>
		/// <exception cref="IllegalStateException"> if the splash screen has already been closed </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public void close() throws IllegalStateException
		public void Close()
		{
			lock (typeof(SplashScreen))
			{
				CheckVisible();
				_close(SplashPtr);
				Image = null;
				SplashScreen.MarkClosed();
			}
		}

		internal static void MarkClosed()
		{
			lock (typeof(SplashScreen))
			{
				WasClosed = true;
				TheInstance = null;
			}
		}


		/// <summary>
		/// Determines whether the splash screen is visible. The splash screen may
		/// be hidden using <seealso cref="#close()"/>, it is also hidden automatically when
		/// the first AWT/Swing window is made visible.
		/// <para>
		/// Note that the native platform may delay presenting the splash screen
		/// native window on the screen. The return value of {@code true} for this
		/// method only guarantees that the conditions to hide the splash screen
		/// window have not occurred yet.
		/// 
		/// </para>
		/// </summary>
		/// <returns> true if the splash screen is visible (has not been closed yet),
		///         false otherwise </returns>
		public bool Visible
		{
			get
			{
				lock (typeof(SplashScreen))
				{
					return !WasClosed && _isVisible(SplashPtr);
				}
			}
		}

		private BufferedImage Image; // overlay image

		private readonly long SplashPtr; // pointer to native Splash structure
		private static bool WasClosed = false;

		private URL ImageURL_Renamed;

		/// <summary>
		/// The instance reference for the singleton.
		/// (<code>null</code> if no instance exists yet.)
		/// </summary>
		/// <seealso cref= #getSplashScreen </seealso>
		/// <seealso cref= #close </seealso>
		private static SplashScreen TheInstance = null;

		private static readonly PlatformLogger Log = PlatformLogger.getLogger("java.awt.SplashScreen");

//JAVA TO C# CONVERTER TODO TASK: Replace 'unknown' with the appropriate dll name:
		[DllImport("unknown")]
		private extern static void _update(long splashPtr, int[] data, int x, int y, int width, int height, int scanlineStride);
//JAVA TO C# CONVERTER TODO TASK: Replace 'unknown' with the appropriate dll name:
		[DllImport("unknown")]
		private extern static boolean _isVisible(long splashPtr);
//JAVA TO C# CONVERTER TODO TASK: Replace 'unknown' with the appropriate dll name:
		[DllImport("unknown")]
		private extern static Rectangle _getBounds(long splashPtr);
//JAVA TO C# CONVERTER TODO TASK: Replace 'unknown' with the appropriate dll name:
		[DllImport("unknown")]
		private extern static long _getInstance();
//JAVA TO C# CONVERTER TODO TASK: Replace 'unknown' with the appropriate dll name:
		[DllImport("unknown")]
		private extern static void _close(long splashPtr);
//JAVA TO C# CONVERTER TODO TASK: Replace 'unknown' with the appropriate dll name:
		[DllImport("unknown")]
		private extern static String _getImageFileName(long splashPtr);
//JAVA TO C# CONVERTER TODO TASK: Replace 'unknown' with the appropriate dll name:
		[DllImport("unknown")]
		private extern static String _getImageJarName(long SplashPtr);
//JAVA TO C# CONVERTER TODO TASK: Replace 'unknown' with the appropriate dll name:
		[DllImport("unknown")]
		private extern static boolean _setImageData(long SplashPtr, sbyte[] data);
//JAVA TO C# CONVERTER TODO TASK: Replace 'unknown' with the appropriate dll name:
		[DllImport("unknown")]
		private extern static float _getScaleFactor(long SplashPtr);

	}

}
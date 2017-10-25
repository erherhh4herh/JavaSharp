using System;
using System.Runtime.InteropServices;

/*
 * Copyright (c) 1995, 2013, Oracle and/or its affiliates. All rights reserved.
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
	/// The <code>Color</code> class is used to encapsulate colors in the default
	/// sRGB color space or colors in arbitrary color spaces identified by a
	/// <seealso cref="ColorSpace"/>.  Every color has an implicit alpha value of 1.0 or
	/// an explicit one provided in the constructor.  The alpha value
	/// defines the transparency of a color and can be represented by
	/// a float value in the range 0.0&nbsp;-&nbsp;1.0 or 0&nbsp;-&nbsp;255.
	/// An alpha value of 1.0 or 255 means that the color is completely
	/// opaque and an alpha value of 0 or 0.0 means that the color is
	/// completely transparent.
	/// When constructing a <code>Color</code> with an explicit alpha or
	/// getting the color/alpha components of a <code>Color</code>, the color
	/// components are never premultiplied by the alpha component.
	/// <para>
	/// The default color space for the Java 2D(tm) API is sRGB, a proposed
	/// standard RGB color space.  For further information on sRGB,
	/// see <A href="http://www.w3.org/pub/WWW/Graphics/Color/sRGB.html">
	/// http://www.w3.org/pub/WWW/Graphics/Color/sRGB.html
	/// </A>.
	/// </para>
	/// <para>
	/// @version     10 Feb 1997
	/// @author      Sami Shaio
	/// @author      Arthur van Hoff
	/// </para>
	/// </summary>
	/// <seealso cref=         ColorSpace </seealso>
	/// <seealso cref=         AlphaComposite </seealso>
	[Serializable]
	public class Color : Paint
	{

		/// <summary>
		/// The color white.  In the default sRGB space.
		/// </summary>
		public static readonly Color White = new Color(255, 255, 255);

		/// <summary>
		/// The color white.  In the default sRGB space.
		/// @since 1.4
		/// </summary>
		public static readonly Color WHITE = White;

		/// <summary>
		/// The color light gray.  In the default sRGB space.
		/// </summary>
		public static readonly Color LightGray = new Color(192, 192, 192);

		/// <summary>
		/// The color light gray.  In the default sRGB space.
		/// @since 1.4
		/// </summary>
		public static readonly Color LIGHT_GRAY = LightGray;

		/// <summary>
		/// The color gray.  In the default sRGB space.
		/// </summary>
		public static readonly Color Gray = new Color(128, 128, 128);

		/// <summary>
		/// The color gray.  In the default sRGB space.
		/// @since 1.4
		/// </summary>
		public static readonly Color GRAY = Gray;

		/// <summary>
		/// The color dark gray.  In the default sRGB space.
		/// </summary>
		public static readonly Color DarkGray = new Color(64, 64, 64);

		/// <summary>
		/// The color dark gray.  In the default sRGB space.
		/// @since 1.4
		/// </summary>
		public static readonly Color DARK_GRAY = DarkGray;

		/// <summary>
		/// The color black.  In the default sRGB space.
		/// </summary>
		public static readonly Color Black = new Color(0, 0, 0);

		/// <summary>
		/// The color black.  In the default sRGB space.
		/// @since 1.4
		/// </summary>
		public static readonly Color BLACK = Black;

		/// <summary>
		/// The color red.  In the default sRGB space.
		/// </summary>
		public static readonly Color Red_Renamed = new Color(255, 0, 0);

		/// <summary>
		/// The color red.  In the default sRGB space.
		/// @since 1.4
		/// </summary>
		public static readonly Color RED = Red_Renamed;

		/// <summary>
		/// The color pink.  In the default sRGB space.
		/// </summary>
		public static readonly Color Pink = new Color(255, 175, 175);

		/// <summary>
		/// The color pink.  In the default sRGB space.
		/// @since 1.4
		/// </summary>
		public static readonly Color PINK = Pink;

		/// <summary>
		/// The color orange.  In the default sRGB space.
		/// </summary>
		public static readonly Color Orange = new Color(255, 200, 0);

		/// <summary>
		/// The color orange.  In the default sRGB space.
		/// @since 1.4
		/// </summary>
		public static readonly Color ORANGE = Orange;

		/// <summary>
		/// The color yellow.  In the default sRGB space.
		/// </summary>
		public static readonly Color Yellow = new Color(255, 255, 0);

		/// <summary>
		/// The color yellow.  In the default sRGB space.
		/// @since 1.4
		/// </summary>
		public static readonly Color YELLOW = Yellow;

		/// <summary>
		/// The color green.  In the default sRGB space.
		/// </summary>
		public static readonly Color Green_Renamed = new Color(0, 255, 0);

		/// <summary>
		/// The color green.  In the default sRGB space.
		/// @since 1.4
		/// </summary>
		public static readonly Color GREEN = Green_Renamed;

		/// <summary>
		/// The color magenta.  In the default sRGB space.
		/// </summary>
		public static readonly Color Magenta = new Color(255, 0, 255);

		/// <summary>
		/// The color magenta.  In the default sRGB space.
		/// @since 1.4
		/// </summary>
		public static readonly Color MAGENTA = Magenta;

		/// <summary>
		/// The color cyan.  In the default sRGB space.
		/// </summary>
		public static readonly Color Cyan = new Color(0, 255, 255);

		/// <summary>
		/// The color cyan.  In the default sRGB space.
		/// @since 1.4
		/// </summary>
		public static readonly Color CYAN = Cyan;

		/// <summary>
		/// The color blue.  In the default sRGB space.
		/// </summary>
		public static readonly Color Blue_Renamed = new Color(0, 0, 255);

		/// <summary>
		/// The color blue.  In the default sRGB space.
		/// @since 1.4
		/// </summary>
		public static readonly Color BLUE = Blue_Renamed;

		/// <summary>
		/// The color value.
		/// @serial </summary>
		/// <seealso cref= #getRGB </seealso>
		internal int Value;

		/// <summary>
		/// The color value in the default sRGB <code>ColorSpace</code> as
		/// <code>float</code> components (no alpha).
		/// If <code>null</code> after object construction, this must be an
		/// sRGB color constructed with 8-bit precision, so compute from the
		/// <code>int</code> color value.
		/// @serial </summary>
		/// <seealso cref= #getRGBColorComponents </seealso>
		/// <seealso cref= #getRGBComponents </seealso>
		private float[] Frgbvalue = null;

		/// <summary>
		/// The color value in the native <code>ColorSpace</code> as
		/// <code>float</code> components (no alpha).
		/// If <code>null</code> after object construction, this must be an
		/// sRGB color constructed with 8-bit precision, so compute from the
		/// <code>int</code> color value.
		/// @serial </summary>
		/// <seealso cref= #getRGBColorComponents </seealso>
		/// <seealso cref= #getRGBComponents </seealso>
		private float[] Fvalue = null;

		/// <summary>
		/// The alpha value as a <code>float</code> component.
		/// If <code>frgbvalue</code> is <code>null</code>, this is not valid
		/// data, so compute from the <code>int</code> color value.
		/// @serial </summary>
		/// <seealso cref= #getRGBComponents </seealso>
		/// <seealso cref= #getComponents </seealso>
		private float Falpha = 0.0f;

		/// <summary>
		/// The <code>ColorSpace</code>.  If <code>null</code>, then it's
		/// default is sRGB.
		/// @serial </summary>
		/// <seealso cref= #getColor </seealso>
		/// <seealso cref= #getColorSpace </seealso>
		/// <seealso cref= #getColorComponents </seealso>
		private ColorSpace Cs = null;

		/*
		 * JDK 1.1 serialVersionUID
		 */
		 private const long SerialVersionUID = 118526816881161077L;

		/// <summary>
		/// Initialize JNI field and method IDs
		/// </summary>
//JAVA TO C# CONVERTER TODO TASK: Replace 'unknown' with the appropriate dll name:
		[DllImport("unknown")]
		private static extern void initIDs();

		static Color()
		{
			/// <summary>
			/// 4112352 - Calling getDefaultToolkit()
			/// ** here can cause this class to be accessed before it is fully
			/// ** initialized. DON'T DO IT!!!
			/// **
			/// ** Toolkit.getDefaultToolkit();
			/// 
			/// </summary>

			/* ensure that the necessary native libraries are loaded */
			Toolkit.LoadLibraries();
			if (!GraphicsEnvironment.Headless)
			{
				initIDs();
			}
		}

		/// <summary>
		/// Checks the color integer components supplied for validity.
		/// Throws an <seealso cref="IllegalArgumentException"/> if the value is out of
		/// range. </summary>
		/// <param name="r"> the Red component </param>
		/// <param name="g"> the Green component </param>
		/// <param name="b"> the Blue component
		///  </param>
		private static void TestColorValueRange(int r, int g, int b, int a)
		{
			bool rangeError = false;
			String badComponentString = "";

			if (a < 0 || a > 255)
			{
				rangeError = true;
				badComponentString = badComponentString + " Alpha";
			}
			if (r < 0 || r > 255)
			{
				rangeError = true;
				badComponentString = badComponentString + " Red";
			}
			if (g < 0 || g > 255)
			{
				rangeError = true;
				badComponentString = badComponentString + " Green";
			}
			if (b < 0 || b > 255)
			{
				rangeError = true;
				badComponentString = badComponentString + " Blue";
			}
			if (rangeError == true)
			{
			throw new IllegalArgumentException("Color parameter outside of expected range:" + badComponentString);
			}
		}

		/// <summary>
		/// Checks the color <code>float</code> components supplied for
		/// validity.
		/// Throws an <code>IllegalArgumentException</code> if the value is out
		/// of range. </summary>
		/// <param name="r"> the Red component </param>
		/// <param name="g"> the Green component </param>
		/// <param name="b"> the Blue component
		///  </param>
		private static void TestColorValueRange(float r, float g, float b, float a)
		{
			bool rangeError = false;
			String badComponentString = "";
			if (a < 0.0 || a > 1.0)
			{
				rangeError = true;
				badComponentString = badComponentString + " Alpha";
			}
			if (r < 0.0 || r > 1.0)
			{
				rangeError = true;
				badComponentString = badComponentString + " Red";
			}
			if (g < 0.0 || g > 1.0)
			{
				rangeError = true;
				badComponentString = badComponentString + " Green";
			}
			if (b < 0.0 || b > 1.0)
			{
				rangeError = true;
				badComponentString = badComponentString + " Blue";
			}
			if (rangeError == true)
			{
			throw new IllegalArgumentException("Color parameter outside of expected range:" + badComponentString);
			}
		}

		/// <summary>
		/// Creates an opaque sRGB color with the specified red, green,
		/// and blue values in the range (0 - 255).
		/// The actual color used in rendering depends
		/// on finding the best match given the color space
		/// available for a given output device.
		/// Alpha is defaulted to 255.
		/// </summary>
		/// <exception cref="IllegalArgumentException"> if <code>r</code>, <code>g</code>
		///        or <code>b</code> are outside of the range
		///        0 to 255, inclusive </exception>
		/// <param name="r"> the red component </param>
		/// <param name="g"> the green component </param>
		/// <param name="b"> the blue component </param>
		/// <seealso cref= #getRed </seealso>
		/// <seealso cref= #getGreen </seealso>
		/// <seealso cref= #getBlue </seealso>
		/// <seealso cref= #getRGB </seealso>
		public Color(int r, int g, int b) : this(r, g, b, 255)
		{
		}

		/// <summary>
		/// Creates an sRGB color with the specified red, green, blue, and alpha
		/// values in the range (0 - 255).
		/// </summary>
		/// <exception cref="IllegalArgumentException"> if <code>r</code>, <code>g</code>,
		///        <code>b</code> or <code>a</code> are outside of the range
		///        0 to 255, inclusive </exception>
		/// <param name="r"> the red component </param>
		/// <param name="g"> the green component </param>
		/// <param name="b"> the blue component </param>
		/// <param name="a"> the alpha component </param>
		/// <seealso cref= #getRed </seealso>
		/// <seealso cref= #getGreen </seealso>
		/// <seealso cref= #getBlue </seealso>
		/// <seealso cref= #getAlpha </seealso>
		/// <seealso cref= #getRGB </seealso>
//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @ConstructorProperties({"red", "green", "blue", "alpha"}) public Color(int r, int g, int b, int a)
		public Color(int r, int g, int b, int a)
		{
			Value = ((a & 0xFF) << 24) | ((r & 0xFF) << 16) | ((g & 0xFF) << 8) | ((b & 0xFF) << 0);
			TestColorValueRange(r,g,b,a);
		}

		/// <summary>
		/// Creates an opaque sRGB color with the specified combined RGB value
		/// consisting of the red component in bits 16-23, the green component
		/// in bits 8-15, and the blue component in bits 0-7.  The actual color
		/// used in rendering depends on finding the best match given the
		/// color space available for a particular output device.  Alpha is
		/// defaulted to 255.
		/// </summary>
		/// <param name="rgb"> the combined RGB components </param>
		/// <seealso cref= java.awt.image.ColorModel#getRGBdefault </seealso>
		/// <seealso cref= #getRed </seealso>
		/// <seealso cref= #getGreen </seealso>
		/// <seealso cref= #getBlue </seealso>
		/// <seealso cref= #getRGB </seealso>
		public Color(int rgb)
		{
			Value = unchecked((int)0xff000000) | rgb;
		}

		/// <summary>
		/// Creates an sRGB color with the specified combined RGBA value consisting
		/// of the alpha component in bits 24-31, the red component in bits 16-23,
		/// the green component in bits 8-15, and the blue component in bits 0-7.
		/// If the <code>hasalpha</code> argument is <code>false</code>, alpha
		/// is defaulted to 255.
		/// </summary>
		/// <param name="rgba"> the combined RGBA components </param>
		/// <param name="hasalpha"> <code>true</code> if the alpha bits are valid;
		///        <code>false</code> otherwise </param>
		/// <seealso cref= java.awt.image.ColorModel#getRGBdefault </seealso>
		/// <seealso cref= #getRed </seealso>
		/// <seealso cref= #getGreen </seealso>
		/// <seealso cref= #getBlue </seealso>
		/// <seealso cref= #getAlpha </seealso>
		/// <seealso cref= #getRGB </seealso>
		public Color(int rgba, bool hasalpha)
		{
			if (hasalpha)
			{
				Value = rgba;
			}
			else
			{
				Value = unchecked((int)0xff000000) | rgba;
			}
		}

		/// <summary>
		/// Creates an opaque sRGB color with the specified red, green, and blue
		/// values in the range (0.0 - 1.0).  Alpha is defaulted to 1.0.  The
		/// actual color used in rendering depends on finding the best
		/// match given the color space available for a particular output
		/// device.
		/// </summary>
		/// <exception cref="IllegalArgumentException"> if <code>r</code>, <code>g</code>
		///        or <code>b</code> are outside of the range
		///        0.0 to 1.0, inclusive </exception>
		/// <param name="r"> the red component </param>
		/// <param name="g"> the green component </param>
		/// <param name="b"> the blue component </param>
		/// <seealso cref= #getRed </seealso>
		/// <seealso cref= #getGreen </seealso>
		/// <seealso cref= #getBlue </seealso>
		/// <seealso cref= #getRGB </seealso>
		public Color(float r, float g, float b) : this((int)(r * 255 + 0.5), (int)(g * 255 + 0.5), (int)(b * 255 + 0.5))
		{
			TestColorValueRange(r,g,b,1.0f);
			Frgbvalue = new float[3];
			Frgbvalue[0] = r;
			Frgbvalue[1] = g;
			Frgbvalue[2] = b;
			Falpha = 1.0f;
			Fvalue = Frgbvalue;
		}

		/// <summary>
		/// Creates an sRGB color with the specified red, green, blue, and
		/// alpha values in the range (0.0 - 1.0).  The actual color
		/// used in rendering depends on finding the best match given the
		/// color space available for a particular output device. </summary>
		/// <exception cref="IllegalArgumentException"> if <code>r</code>, <code>g</code>
		///        <code>b</code> or <code>a</code> are outside of the range
		///        0.0 to 1.0, inclusive </exception>
		/// <param name="r"> the red component </param>
		/// <param name="g"> the green component </param>
		/// <param name="b"> the blue component </param>
		/// <param name="a"> the alpha component </param>
		/// <seealso cref= #getRed </seealso>
		/// <seealso cref= #getGreen </seealso>
		/// <seealso cref= #getBlue </seealso>
		/// <seealso cref= #getAlpha </seealso>
		/// <seealso cref= #getRGB </seealso>
		public Color(float r, float g, float b, float a) : this((int)(r * 255 + 0.5), (int)(g * 255 + 0.5), (int)(b * 255 + 0.5), (int)(a * 255 + 0.5))
		{
			Frgbvalue = new float[3];
			Frgbvalue[0] = r;
			Frgbvalue[1] = g;
			Frgbvalue[2] = b;
			Falpha = a;
			Fvalue = Frgbvalue;
		}

		/// <summary>
		/// Creates a color in the specified <code>ColorSpace</code>
		/// with the color components specified in the <code>float</code>
		/// array and the specified alpha.  The number of components is
		/// determined by the type of the <code>ColorSpace</code>.  For
		/// example, RGB requires 3 components, but CMYK requires 4
		/// components. </summary>
		/// <param name="cspace"> the <code>ColorSpace</code> to be used to
		///                  interpret the components </param>
		/// <param name="components"> an arbitrary number of color components
		///                      that is compatible with the <code>ColorSpace</code> </param>
		/// <param name="alpha"> alpha value </param>
		/// <exception cref="IllegalArgumentException"> if any of the values in the
		///         <code>components</code> array or <code>alpha</code> is
		///         outside of the range 0.0 to 1.0 </exception>
		/// <seealso cref= #getComponents </seealso>
		/// <seealso cref= #getColorComponents </seealso>
		public Color(ColorSpace cspace, float[] components, float alpha)
		{
			bool rangeError = false;
			String badComponentString = "";
			int n = cspace.NumComponents;
			Fvalue = new float[n];
			for (int i = 0; i < n; i++)
			{
				if (components[i] < 0.0 || components[i] > 1.0)
				{
					rangeError = true;
					badComponentString = badComponentString + "Component " + i + " ";
				}
				else
				{
					Fvalue[i] = components[i];
				}
			}
			if (alpha < 0.0 || alpha > 1.0)
			{
				rangeError = true;
				badComponentString = badComponentString + "Alpha";
			}
			else
			{
				Falpha = alpha;
			}
			if (rangeError)
			{
				throw new IllegalArgumentException("Color parameter outside of expected range: " + badComponentString);
			}
			Frgbvalue = cspace.ToRGB(Fvalue);
			Cs = cspace;
			Value = ((((int)(Falpha * 255)) & 0xFF) << 24) | ((((int)(Frgbvalue[0] * 255)) & 0xFF) << 16) | ((((int)(Frgbvalue[1] * 255)) & 0xFF) << 8) | ((((int)(Frgbvalue[2] * 255)) & 0xFF) << 0);
		}

		/// <summary>
		/// Returns the red component in the range 0-255 in the default sRGB
		/// space. </summary>
		/// <returns> the red component. </returns>
		/// <seealso cref= #getRGB </seealso>
		public virtual int Red
		{
			get
			{
				return (RGB >> 16) & 0xFF;
			}
		}

		/// <summary>
		/// Returns the green component in the range 0-255 in the default sRGB
		/// space. </summary>
		/// <returns> the green component. </returns>
		/// <seealso cref= #getRGB </seealso>
		public virtual int Green
		{
			get
			{
				return (RGB >> 8) & 0xFF;
			}
		}

		/// <summary>
		/// Returns the blue component in the range 0-255 in the default sRGB
		/// space. </summary>
		/// <returns> the blue component. </returns>
		/// <seealso cref= #getRGB </seealso>
		public virtual int Blue
		{
			get
			{
				return (RGB >> 0) & 0xFF;
			}
		}

		/// <summary>
		/// Returns the alpha component in the range 0-255. </summary>
		/// <returns> the alpha component. </returns>
		/// <seealso cref= #getRGB </seealso>
		public virtual int Alpha
		{
			get
			{
				return (RGB >> 24) & 0xff;
			}
		}

		/// <summary>
		/// Returns the RGB value representing the color in the default sRGB
		/// <seealso cref="ColorModel"/>.
		/// (Bits 24-31 are alpha, 16-23 are red, 8-15 are green, 0-7 are
		/// blue). </summary>
		/// <returns> the RGB value of the color in the default sRGB
		///         <code>ColorModel</code>. </returns>
		/// <seealso cref= java.awt.image.ColorModel#getRGBdefault </seealso>
		/// <seealso cref= #getRed </seealso>
		/// <seealso cref= #getGreen </seealso>
		/// <seealso cref= #getBlue
		/// @since JDK1.0 </seealso>
		public virtual int RGB
		{
			get
			{
				return Value;
			}
		}

		private const double FACTOR = 0.7;

		/// <summary>
		/// Creates a new <code>Color</code> that is a brighter version of this
		/// <code>Color</code>.
		/// <para>
		/// This method applies an arbitrary scale factor to each of the three RGB
		/// components of this <code>Color</code> to create a brighter version
		/// of this <code>Color</code>.
		/// The {@code alpha} value is preserved.
		/// Although <code>brighter</code> and
		/// <code>darker</code> are inverse operations, the results of a
		/// series of invocations of these two methods might be inconsistent
		/// because of rounding errors.
		/// </para>
		/// </summary>
		/// <returns>     a new <code>Color</code> object that is
		///                 a brighter version of this <code>Color</code>
		///                 with the same {@code alpha} value. </returns>
		/// <seealso cref=        java.awt.Color#darker
		/// @since      JDK1.0 </seealso>
		public virtual Color Brighter()
		{
			int r = Red;
			int g = Green;
			int b = Blue;
			int alpha = Alpha;

			/* From 2D group:
			 * 1. black.brighter() should return grey
			 * 2. applying brighter to blue will always return blue, brighter
			 * 3. non pure color (non zero rgb) will eventually return white
			 */
			int i = (int)(1.0 / (1.0 - FACTOR));
			if (r == 0 && g == 0 && b == 0)
			{
				return new Color(i, i, i, alpha);
			}
			if (r > 0 && r < i)
			{
				r = i;
			}
			if (g > 0 && g < i)
			{
				g = i;
			}
			if (b > 0 && b < i)
			{
				b = i;
			}

			return new Color(System.Math.Min((int)(r / FACTOR), 255), System.Math.Min((int)(g / FACTOR), 255), System.Math.Min((int)(b / FACTOR), 255), alpha);
		}

		/// <summary>
		/// Creates a new <code>Color</code> that is a darker version of this
		/// <code>Color</code>.
		/// <para>
		/// This method applies an arbitrary scale factor to each of the three RGB
		/// components of this <code>Color</code> to create a darker version of
		/// this <code>Color</code>.
		/// The {@code alpha} value is preserved.
		/// Although <code>brighter</code> and
		/// <code>darker</code> are inverse operations, the results of a series
		/// of invocations of these two methods might be inconsistent because
		/// of rounding errors.
		/// </para>
		/// </summary>
		/// <returns>  a new <code>Color</code> object that is
		///                    a darker version of this <code>Color</code>
		///                    with the same {@code alpha} value. </returns>
		/// <seealso cref=        java.awt.Color#brighter
		/// @since      JDK1.0 </seealso>
		public virtual Color Darker()
		{
			return new Color(System.Math.Max((int)(Red * FACTOR), 0), System.Math.Max((int)(Green * FACTOR), 0), System.Math.Max((int)(Blue * FACTOR), 0), Alpha);
		}

		/// <summary>
		/// Computes the hash code for this <code>Color</code>. </summary>
		/// <returns>     a hash code value for this object.
		/// @since      JDK1.0 </returns>
		public override int HashCode()
		{
			return Value;
		}

		/// <summary>
		/// Determines whether another object is equal to this
		/// <code>Color</code>.
		/// <para>
		/// The result is <code>true</code> if and only if the argument is not
		/// <code>null</code> and is a <code>Color</code> object that has the same
		/// red, green, blue, and alpha values as this object.
		/// </para>
		/// </summary>
		/// <param name="obj">   the object to test for equality with this
		///                          <code>Color</code> </param>
		/// <returns>      <code>true</code> if the objects are the same;
		///                             <code>false</code> otherwise.
		/// @since   JDK1.0 </returns>
		public override bool Equals(Object obj)
		{
			return obj is Color && ((Color)obj).RGB == this.RGB;
		}

		/// <summary>
		/// Returns a string representation of this <code>Color</code>. This
		/// method is intended to be used only for debugging purposes.  The
		/// content and format of the returned string might vary between
		/// implementations. The returned string might be empty but cannot
		/// be <code>null</code>.
		/// </summary>
		/// <returns>  a string representation of this <code>Color</code>. </returns>
		public override String ToString()
		{
//JAVA TO C# CONVERTER WARNING: The .NET Type.FullName property will not always yield results identical to the Java Class.getName method:
			return this.GetType().FullName + "[r=" + Red + ",g=" + Green + ",b=" + Blue + "]";
		}

		/// <summary>
		/// Converts a <code>String</code> to an integer and returns the
		/// specified opaque <code>Color</code>. This method handles string
		/// formats that are used to represent octal and hexadecimal numbers. </summary>
		/// <param name="nm"> a <code>String</code> that represents
		///                            an opaque color as a 24-bit integer </param>
		/// <returns>     the new <code>Color</code> object. </returns>
		/// <seealso cref=        java.lang.Integer#decode </seealso>
		/// <exception cref="NumberFormatException">  if the specified string cannot
		///                      be interpreted as a decimal,
		///                      octal, or hexadecimal integer.
		/// @since      JDK1.1 </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public static Color decode(String nm) throws NumberFormatException
		public static Color Decode(String nm)
		{
			Integer intval = Integer.Decode(nm);
			int i = intval.IntValue();
			return new Color((i >> 16) & 0xFF, (i >> 8) & 0xFF, i & 0xFF);
		}

		/// <summary>
		/// Finds a color in the system properties.
		/// <para>
		/// The argument is treated as the name of a system property to
		/// be obtained. The string value of this property is then interpreted
		/// as an integer which is then converted to a <code>Color</code>
		/// object.
		/// </para>
		/// <para>
		/// If the specified property is not found or could not be parsed as
		/// an integer then <code>null</code> is returned.
		/// </para>
		/// </summary>
		/// <param name="nm"> the name of the color property </param>
		/// <returns>   the <code>Color</code> converted from the system
		///          property. </returns>
		/// <seealso cref=      java.lang.System#getProperty(java.lang.String) </seealso>
		/// <seealso cref=      java.lang.Integer#getInteger(java.lang.String) </seealso>
		/// <seealso cref=      java.awt.Color#Color(int)
		/// @since    JDK1.0 </seealso>
		public static Color GetColor(String nm)
		{
			return GetColor(nm, null);
		}

		/// <summary>
		/// Finds a color in the system properties.
		/// <para>
		/// The first argument is treated as the name of a system property to
		/// be obtained. The string value of this property is then interpreted
		/// as an integer which is then converted to a <code>Color</code>
		/// object.
		/// </para>
		/// <para>
		/// If the specified property is not found or cannot be parsed as
		/// an integer then the <code>Color</code> specified by the second
		/// argument is returned instead.
		/// </para>
		/// </summary>
		/// <param name="nm"> the name of the color property </param>
		/// <param name="v">    the default <code>Color</code> </param>
		/// <returns>   the <code>Color</code> converted from the system
		///          property, or the specified <code>Color</code>. </returns>
		/// <seealso cref=      java.lang.System#getProperty(java.lang.String) </seealso>
		/// <seealso cref=      java.lang.Integer#getInteger(java.lang.String) </seealso>
		/// <seealso cref=      java.awt.Color#Color(int)
		/// @since    JDK1.0 </seealso>
		public static Color GetColor(String nm, Color v)
		{
			Integer intval = Integer.GetInteger(nm);
			if (intval == null)
			{
				return v;
			}
			int i = intval.IntValue();
			return new Color((i >> 16) & 0xFF, (i >> 8) & 0xFF, i & 0xFF);
		}

		/// <summary>
		/// Finds a color in the system properties.
		/// <para>
		/// The first argument is treated as the name of a system property to
		/// be obtained. The string value of this property is then interpreted
		/// as an integer which is then converted to a <code>Color</code>
		/// object.
		/// </para>
		/// <para>
		/// If the specified property is not found or could not be parsed as
		/// an integer then the integer value <code>v</code> is used instead,
		/// and is converted to a <code>Color</code> object.
		/// </para>
		/// </summary>
		/// <param name="nm">  the name of the color property </param>
		/// <param name="v">   the default color value, as an integer </param>
		/// <returns>   the <code>Color</code> converted from the system
		///          property or the <code>Color</code> converted from
		///          the specified integer. </returns>
		/// <seealso cref=      java.lang.System#getProperty(java.lang.String) </seealso>
		/// <seealso cref=      java.lang.Integer#getInteger(java.lang.String) </seealso>
		/// <seealso cref=      java.awt.Color#Color(int)
		/// @since    JDK1.0 </seealso>
		public static Color GetColor(String nm, int v)
		{
			Integer intval = Integer.GetInteger(nm);
			int i = (intval != null) ? intval.IntValue() : v;
			return new Color((i >> 16) & 0xFF, (i >> 8) & 0xFF, (i >> 0) & 0xFF);
		}

		/// <summary>
		/// Converts the components of a color, as specified by the HSB
		/// model, to an equivalent set of values for the default RGB model.
		/// <para>
		/// The <code>saturation</code> and <code>brightness</code> components
		/// should be floating-point values between zero and one
		/// (numbers in the range 0.0-1.0).  The <code>hue</code> component
		/// can be any floating-point number.  The floor of this number is
		/// subtracted from it to create a fraction between 0 and 1.  This
		/// fractional number is then multiplied by 360 to produce the hue
		/// angle in the HSB color model.
		/// </para>
		/// <para>
		/// The integer that is returned by <code>HSBtoRGB</code> encodes the
		/// value of a color in bits 0-23 of an integer value that is the same
		/// format used by the method <seealso cref="#getRGB() getRGB"/>.
		/// This integer can be supplied as an argument to the
		/// <code>Color</code> constructor that takes a single integer argument.
		/// </para>
		/// </summary>
		/// <param name="hue">   the hue component of the color </param>
		/// <param name="saturation">   the saturation of the color </param>
		/// <param name="brightness">   the brightness of the color </param>
		/// <returns>    the RGB value of the color with the indicated hue,
		///                            saturation, and brightness. </returns>
		/// <seealso cref=       java.awt.Color#getRGB() </seealso>
		/// <seealso cref=       java.awt.Color#Color(int) </seealso>
		/// <seealso cref=       java.awt.image.ColorModel#getRGBdefault()
		/// @since     JDK1.0 </seealso>
		public static int HSBtoRGB(float hue, float saturation, float brightness)
		{
			int r = 0, g = 0, b = 0;
			if (saturation == 0)
			{
				r = g = b = (int)(brightness * 255.0f + 0.5f);
			}
			else
			{
				float h = (hue - (float)System.Math.Floor(hue)) * 6.0f;
				float f = h - (float)System.Math.Floor(h);
				float p = brightness * (1.0f - saturation);
				float q = brightness * (1.0f - saturation * f);
				float t = brightness * (1.0f - (saturation * (1.0f - f)));
				switch ((int) h)
				{
				case 0:
					r = (int)(brightness * 255.0f + 0.5f);
					g = (int)(t * 255.0f + 0.5f);
					b = (int)(p * 255.0f + 0.5f);
					break;
				case 1:
					r = (int)(q * 255.0f + 0.5f);
					g = (int)(brightness * 255.0f + 0.5f);
					b = (int)(p * 255.0f + 0.5f);
					break;
				case 2:
					r = (int)(p * 255.0f + 0.5f);
					g = (int)(brightness * 255.0f + 0.5f);
					b = (int)(t * 255.0f + 0.5f);
					break;
				case 3:
					r = (int)(p * 255.0f + 0.5f);
					g = (int)(q * 255.0f + 0.5f);
					b = (int)(brightness * 255.0f + 0.5f);
					break;
				case 4:
					r = (int)(t * 255.0f + 0.5f);
					g = (int)(p * 255.0f + 0.5f);
					b = (int)(brightness * 255.0f + 0.5f);
					break;
				case 5:
					r = (int)(brightness * 255.0f + 0.5f);
					g = (int)(p * 255.0f + 0.5f);
					b = (int)(q * 255.0f + 0.5f);
					break;
				}
			}
			return unchecked((int)0xff000000) | (r << 16) | (g << 8) | (b << 0);
		}

		/// <summary>
		/// Converts the components of a color, as specified by the default RGB
		/// model, to an equivalent set of values for hue, saturation, and
		/// brightness that are the three components of the HSB model.
		/// <para>
		/// If the <code>hsbvals</code> argument is <code>null</code>, then a
		/// new array is allocated to return the result. Otherwise, the method
		/// returns the array <code>hsbvals</code>, with the values put into
		/// that array.
		/// </para>
		/// </summary>
		/// <param name="r">   the red component of the color </param>
		/// <param name="g">   the green component of the color </param>
		/// <param name="b">   the blue component of the color </param>
		/// <param name="hsbvals">  the array used to return the
		///                     three HSB values, or <code>null</code> </param>
		/// <returns>    an array of three elements containing the hue, saturation,
		///                     and brightness (in that order), of the color with
		///                     the indicated red, green, and blue components. </returns>
		/// <seealso cref=       java.awt.Color#getRGB() </seealso>
		/// <seealso cref=       java.awt.Color#Color(int) </seealso>
		/// <seealso cref=       java.awt.image.ColorModel#getRGBdefault()
		/// @since     JDK1.0 </seealso>
		public static float[] RGBtoHSB(int r, int g, int b, float[] hsbvals)
		{
			float hue, saturation, brightness;
			if (hsbvals == null)
			{
				hsbvals = new float[3];
			}
			int cmax = (r > g) ? r : g;
			if (b > cmax)
			{
				cmax = b;
			}
			int cmin = (r < g) ? r : g;
			if (b < cmin)
			{
				cmin = b;
			}

			brightness = ((float) cmax) / 255.0f;
			if (cmax != 0)
			{
				saturation = ((float)(cmax - cmin)) / ((float) cmax);
			}
			else
			{
				saturation = 0;
			}
			if (saturation == 0)
			{
				hue = 0;
			}
			else
			{
				float redc = ((float)(cmax - r)) / ((float)(cmax - cmin));
				float greenc = ((float)(cmax - g)) / ((float)(cmax - cmin));
				float bluec = ((float)(cmax - b)) / ((float)(cmax - cmin));
				if (r == cmax)
				{
					hue = bluec - greenc;
				}
				else if (g == cmax)
				{
					hue = 2.0f + redc - bluec;
				}
				else
				{
					hue = 4.0f + greenc - redc;
				}
				hue = hue / 6.0f;
				if (hue < 0)
				{
					hue = hue + 1.0f;
				}
			}
			hsbvals[0] = hue;
			hsbvals[1] = saturation;
			hsbvals[2] = brightness;
			return hsbvals;
		}

		/// <summary>
		/// Creates a <code>Color</code> object based on the specified values
		/// for the HSB color model.
		/// <para>
		/// The <code>s</code> and <code>b</code> components should be
		/// floating-point values between zero and one
		/// (numbers in the range 0.0-1.0).  The <code>h</code> component
		/// can be any floating-point number.  The floor of this number is
		/// subtracted from it to create a fraction between 0 and 1.  This
		/// fractional number is then multiplied by 360 to produce the hue
		/// angle in the HSB color model.
		/// </para>
		/// </summary>
		/// <param name="h">   the hue component </param>
		/// <param name="s">   the saturation of the color </param>
		/// <param name="b">   the brightness of the color </param>
		/// <returns>  a <code>Color</code> object with the specified hue,
		///                                 saturation, and brightness.
		/// @since   JDK1.0 </returns>
		public static Color GetHSBColor(float h, float s, float b)
		{
			return new Color(HSBtoRGB(h, s, b));
		}

		/// <summary>
		/// Returns a <code>float</code> array containing the color and alpha
		/// components of the <code>Color</code>, as represented in the default
		/// sRGB color space.
		/// If <code>compArray</code> is <code>null</code>, an array of length
		/// 4 is created for the return value.  Otherwise,
		/// <code>compArray</code> must have length 4 or greater,
		/// and it is filled in with the components and returned. </summary>
		/// <param name="compArray"> an array that this method fills with
		///                  color and alpha components and returns </param>
		/// <returns> the RGBA components in a <code>float</code> array. </returns>
		public virtual float[] GetRGBComponents(float[] compArray)
		{
			float[] f;
			if (compArray == null)
			{
				f = new float[4];
			}
			else
			{
				f = compArray;
			}
			if (Frgbvalue == null)
			{
				f[0] = ((float)Red) / 255f;
				f[1] = ((float)Green) / 255f;
				f[2] = ((float)Blue) / 255f;
				f[3] = ((float)Alpha) / 255f;
			}
			else
			{
				f[0] = Frgbvalue[0];
				f[1] = Frgbvalue[1];
				f[2] = Frgbvalue[2];
				f[3] = Falpha;
			}
			return f;
		}

		/// <summary>
		/// Returns a <code>float</code> array containing only the color
		/// components of the <code>Color</code>, in the default sRGB color
		/// space.  If <code>compArray</code> is <code>null</code>, an array of
		/// length 3 is created for the return value.  Otherwise,
		/// <code>compArray</code> must have length 3 or greater, and it is
		/// filled in with the components and returned. </summary>
		/// <param name="compArray"> an array that this method fills with color
		///          components and returns </param>
		/// <returns> the RGB components in a <code>float</code> array. </returns>
		public virtual float[] GetRGBColorComponents(float[] compArray)
		{
			float[] f;
			if (compArray == null)
			{
				f = new float[3];
			}
			else
			{
				f = compArray;
			}
			if (Frgbvalue == null)
			{
				f[0] = ((float)Red) / 255f;
				f[1] = ((float)Green) / 255f;
				f[2] = ((float)Blue) / 255f;
			}
			else
			{
				f[0] = Frgbvalue[0];
				f[1] = Frgbvalue[1];
				f[2] = Frgbvalue[2];
			}
			return f;
		}

		/// <summary>
		/// Returns a <code>float</code> array containing the color and alpha
		/// components of the <code>Color</code>, in the
		/// <code>ColorSpace</code> of the <code>Color</code>.
		/// If <code>compArray</code> is <code>null</code>, an array with
		/// length equal to the number of components in the associated
		/// <code>ColorSpace</code> plus one is created for
		/// the return value.  Otherwise, <code>compArray</code> must have at
		/// least this length and it is filled in with the components and
		/// returned. </summary>
		/// <param name="compArray"> an array that this method fills with the color and
		///          alpha components of this <code>Color</code> in its
		///          <code>ColorSpace</code> and returns </param>
		/// <returns> the color and alpha components in a <code>float</code>
		///          array. </returns>
		public virtual float[] GetComponents(float[] compArray)
		{
			if (Fvalue == null)
			{
				return GetRGBComponents(compArray);
			}
			float[] f;
			int n = Fvalue.Length;
			if (compArray == null)
			{
				f = new float[n + 1];
			}
			else
			{
				f = compArray;
			}
			for (int i = 0; i < n; i++)
			{
				f[i] = Fvalue[i];
			}
			f[n] = Falpha;
			return f;
		}

		/// <summary>
		/// Returns a <code>float</code> array containing only the color
		/// components of the <code>Color</code>, in the
		/// <code>ColorSpace</code> of the <code>Color</code>.
		/// If <code>compArray</code> is <code>null</code>, an array with
		/// length equal to the number of components in the associated
		/// <code>ColorSpace</code> is created for
		/// the return value.  Otherwise, <code>compArray</code> must have at
		/// least this length and it is filled in with the components and
		/// returned. </summary>
		/// <param name="compArray"> an array that this method fills with the color
		///          components of this <code>Color</code> in its
		///          <code>ColorSpace</code> and returns </param>
		/// <returns> the color components in a <code>float</code> array. </returns>
		public virtual float[] GetColorComponents(float[] compArray)
		{
			if (Fvalue == null)
			{
				return GetRGBColorComponents(compArray);
			}
			float[] f;
			int n = Fvalue.Length;
			if (compArray == null)
			{
				f = new float[n];
			}
			else
			{
				f = compArray;
			}
			for (int i = 0; i < n; i++)
			{
				f[i] = Fvalue[i];
			}
			return f;
		}

		/// <summary>
		/// Returns a <code>float</code> array containing the color and alpha
		/// components of the <code>Color</code>, in the
		/// <code>ColorSpace</code> specified by the <code>cspace</code>
		/// parameter.  If <code>compArray</code> is <code>null</code>, an
		/// array with length equal to the number of components in
		/// <code>cspace</code> plus one is created for the return value.
		/// Otherwise, <code>compArray</code> must have at least this
		/// length, and it is filled in with the components and returned. </summary>
		/// <param name="cspace"> a specified <code>ColorSpace</code> </param>
		/// <param name="compArray"> an array that this method fills with the
		///          color and alpha components of this <code>Color</code> in
		///          the specified <code>ColorSpace</code> and returns </param>
		/// <returns> the color and alpha components in a <code>float</code>
		///          array. </returns>
		public virtual float[] GetComponents(ColorSpace cspace, float[] compArray)
		{
			if (Cs == null)
			{
				Cs = ColorSpace.GetInstance(ColorSpace.CS_sRGB_Renamed);
			}
			float[] f;
			if (Fvalue == null)
			{
				f = new float[3];
				f[0] = ((float)Red) / 255f;
				f[1] = ((float)Green) / 255f;
				f[2] = ((float)Blue) / 255f;
			}
			else
			{
				f = Fvalue;
			}
			float[] tmp = Cs.ToCIEXYZ(f);
			float[] tmpout = cspace.FromCIEXYZ(tmp);
			if (compArray == null)
			{
				compArray = new float[tmpout.Length + 1];
			}
			for (int i = 0 ; i < tmpout.Length ; i++)
			{
				compArray[i] = tmpout[i];
			}
			if (Fvalue == null)
			{
				compArray[tmpout.Length] = ((float)Alpha) / 255f;
			}
			else
			{
				compArray[tmpout.Length] = Falpha;
			}
			return compArray;
		}

		/// <summary>
		/// Returns a <code>float</code> array containing only the color
		/// components of the <code>Color</code> in the
		/// <code>ColorSpace</code> specified by the <code>cspace</code>
		/// parameter. If <code>compArray</code> is <code>null</code>, an array
		/// with length equal to the number of components in
		/// <code>cspace</code> is created for the return value.  Otherwise,
		/// <code>compArray</code> must have at least this length, and it is
		/// filled in with the components and returned. </summary>
		/// <param name="cspace"> a specified <code>ColorSpace</code> </param>
		/// <param name="compArray"> an array that this method fills with the color
		///          components of this <code>Color</code> in the specified
		///          <code>ColorSpace</code> </param>
		/// <returns> the color components in a <code>float</code> array. </returns>
		public virtual float[] GetColorComponents(ColorSpace cspace, float[] compArray)
		{
			if (Cs == null)
			{
				Cs = ColorSpace.GetInstance(ColorSpace.CS_sRGB_Renamed);
			}
			float[] f;
			if (Fvalue == null)
			{
				f = new float[3];
				f[0] = ((float)Red) / 255f;
				f[1] = ((float)Green) / 255f;
				f[2] = ((float)Blue) / 255f;
			}
			else
			{
				f = Fvalue;
			}
			float[] tmp = Cs.ToCIEXYZ(f);
			float[] tmpout = cspace.FromCIEXYZ(tmp);
			if (compArray == null)
			{
				return tmpout;
			}
			for (int i = 0 ; i < tmpout.Length ; i++)
			{
				compArray[i] = tmpout[i];
			}
			return compArray;
		}

		/// <summary>
		/// Returns the <code>ColorSpace</code> of this <code>Color</code>. </summary>
		/// <returns> this <code>Color</code> object's <code>ColorSpace</code>. </returns>
		public virtual ColorSpace ColorSpace
		{
			get
			{
				if (Cs == null)
				{
					Cs = ColorSpace.GetInstance(ColorSpace.CS_sRGB_Renamed);
				}
				return Cs;
			}
		}

		/// <summary>
		/// Creates and returns a <seealso cref="PaintContext"/> used to
		/// generate a solid color field pattern.
		/// See the <seealso cref="Paint#createContext specification"/> of the
		/// method in the <seealso cref="Paint"/> interface for information
		/// on null parameter handling.
		/// </summary>
		/// <param name="cm"> the preferred <seealso cref="ColorModel"/> which represents the most convenient
		///           format for the caller to receive the pixel data, or {@code null}
		///           if there is no preference. </param>
		/// <param name="r"> the device space bounding box
		///                     of the graphics primitive being rendered. </param>
		/// <param name="r2d"> the user space bounding box
		///                   of the graphics primitive being rendered. </param>
		/// <param name="xform"> the <seealso cref="AffineTransform"/> from user
		///              space into device space. </param>
		/// <param name="hints"> the set of hints that the context object can use to
		///              choose between rendering alternatives. </param>
		/// <returns> the {@code PaintContext} for
		///         generating color patterns. </returns>
		/// <seealso cref= Paint </seealso>
		/// <seealso cref= PaintContext </seealso>
		/// <seealso cref= ColorModel </seealso>
		/// <seealso cref= Rectangle </seealso>
		/// <seealso cref= Rectangle2D </seealso>
		/// <seealso cref= AffineTransform </seealso>
		/// <seealso cref= RenderingHints </seealso>
		public virtual PaintContext CreateContext(ColorModel cm, Rectangle r, Rectangle2D r2d, AffineTransform xform, RenderingHints hints)
		{
			lock (this)
			{
				return new ColorPaintContext(RGB, cm);
			}
		}

		/// <summary>
		/// Returns the transparency mode for this <code>Color</code>.  This is
		/// required to implement the <code>Paint</code> interface. </summary>
		/// <returns> this <code>Color</code> object's transparency mode. </returns>
		/// <seealso cref= Paint </seealso>
		/// <seealso cref= Transparency </seealso>
		/// <seealso cref= #createContext </seealso>
		public virtual int Transparency
		{
			get
			{
				int alpha = Alpha;
				if (alpha == 0xff)
				{
					return Transparency_Fields.OPAQUE;
				}
				else if (alpha == 0)
				{
					return Transparency_Fields.BITMASK;
				}
				else
				{
					return Transparency_Fields.TRANSLUCENT;
				}
			}
		}

	}

}
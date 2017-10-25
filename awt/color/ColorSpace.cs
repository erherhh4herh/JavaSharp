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

/*
 **********************************************************************
 **********************************************************************
 **********************************************************************
 *** COPYRIGHT (c) Eastman Kodak Company, 1997                      ***
 *** As  an unpublished  work pursuant to Title 17 of the United    ***
 *** States Code.  All rights reserved.                             ***
 **********************************************************************
 **********************************************************************
 **********************************************************************/

namespace java.awt.color
{

	using PCMM = sun.java2d.cmm.PCMM;
	using CMSManager = sun.java2d.cmm.CMSManager;


	/// <summary>
	/// This abstract class is used to serve as a color space tag to identify the
	/// specific color space of a Color object or, via a ColorModel object,
	/// of an Image, a BufferedImage, or a GraphicsDevice.  It contains
	/// methods that transform colors in a specific color space to/from sRGB
	/// and to/from a well-defined CIEXYZ color space.
	/// <para>
	/// For purposes of the methods in this class, colors are represented as
	/// arrays of color components represented as floats in a normalized range
	/// defined by each ColorSpace.  For many ColorSpaces (e.g. sRGB), this
	/// range is 0.0 to 1.0.  However, some ColorSpaces have components whose
	/// values have a different range.  Methods are provided to inquire per
	/// component minimum and maximum normalized values.
	/// </para>
	/// <para>
	/// Several variables are defined for purposes of referring to color
	/// space types (e.g. TYPE_RGB, TYPE_XYZ, etc.) and to refer to specific
	/// color spaces (e.g. CS_sRGB and CS_CIEXYZ).
	/// sRGB is a proposed standard RGB color space.  For more information,
	/// see <A href="http://www.w3.org/pub/WWW/Graphics/Color/sRGB.html">
	/// http://www.w3.org/pub/WWW/Graphics/Color/sRGB.html
	/// </A>.
	/// </para>
	/// <para>
	/// The purpose of the methods to transform to/from the well-defined
	/// CIEXYZ color space is to support conversions between any two color
	/// spaces at a reasonably high degree of accuracy.  It is expected that
	/// particular implementations of subclasses of ColorSpace (e.g.
	/// ICC_ColorSpace) will support high performance conversion based on
	/// underlying platform color management systems.
	/// </para>
	/// <para>
	/// The CS_CIEXYZ space used by the toCIEXYZ/fromCIEXYZ methods can be
	/// described as follows:
	/// <pre>
	/// 
	/// &nbsp;     CIEXYZ
	/// &nbsp;     viewing illuminance: 200 lux
	/// &nbsp;     viewing white point: CIE D50
	/// &nbsp;     media white point: "that of a perfectly reflecting diffuser" -- D50
	/// &nbsp;     media black point: 0 lux or 0 Reflectance
	/// &nbsp;     flare: 1 percent
	/// &nbsp;     surround: 20percent of the media white point
	/// &nbsp;     media description: reflection print (i.e., RLAB, Hunt viewing media)
	/// &nbsp;     note: For developers creating an ICC profile for this conversion
	/// &nbsp;           space, the following is applicable.  Use a simple Von Kries
	/// &nbsp;           white point adaptation folded into the 3X3 matrix parameters
	/// &nbsp;           and fold the flare and surround effects into the three
	/// &nbsp;           one-dimensional lookup tables (assuming one uses the minimal
	/// &nbsp;           model for monitors).
	/// 
	/// </pre>
	/// 
	/// </para>
	/// </summary>
	/// <seealso cref= ICC_ColorSpace </seealso>

	[Serializable]
	public abstract class ColorSpace
	{

		internal const long SerialVersionUID = -409452704308689724L;

		private int Type_Renamed;
		private int NumComponents_Renamed;
		[NonSerialized]
		private String[] CompName = null;

		// Cache of singletons for the predefined color spaces.
		private static ColorSpace SRGBspace;
		private static ColorSpace XYZspace;
		private static ColorSpace PYCCspace;
		private static ColorSpace GRAYspace;
		private static ColorSpace LINEAR_RGBspace;

		/// <summary>
		/// Any of the family of XYZ color spaces.
		/// </summary>
//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Native public static final int TYPE_XYZ = 0;
		public const int TYPE_XYZ = 0;

		/// <summary>
		/// Any of the family of Lab color spaces.
		/// </summary>
//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Native public static final int TYPE_Lab = 1;
		public const int TYPE_Lab = 1;

		/// <summary>
		/// Any of the family of Luv color spaces.
		/// </summary>
//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Native public static final int TYPE_Luv = 2;
		public const int TYPE_Luv = 2;

		/// <summary>
		/// Any of the family of YCbCr color spaces.
		/// </summary>
//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Native public static final int TYPE_YCbCr = 3;
		public const int TYPE_YCbCr = 3;

		/// <summary>
		/// Any of the family of Yxy color spaces.
		/// </summary>
//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Native public static final int TYPE_Yxy = 4;
		public const int TYPE_Yxy = 4;

		/// <summary>
		/// Any of the family of RGB color spaces.
		/// </summary>
//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Native public static final int TYPE_RGB = 5;
		public const int TYPE_RGB = 5;

		/// <summary>
		/// Any of the family of GRAY color spaces.
		/// </summary>
//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Native public static final int TYPE_GRAY = 6;
		public const int TYPE_GRAY = 6;

		/// <summary>
		/// Any of the family of HSV color spaces.
		/// </summary>
//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Native public static final int TYPE_HSV = 7;
		public const int TYPE_HSV = 7;

		/// <summary>
		/// Any of the family of HLS color spaces.
		/// </summary>
//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Native public static final int TYPE_HLS = 8;
		public const int TYPE_HLS = 8;

		/// <summary>
		/// Any of the family of CMYK color spaces.
		/// </summary>
//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Native public static final int TYPE_CMYK = 9;
		public const int TYPE_CMYK = 9;

		/// <summary>
		/// Any of the family of CMY color spaces.
		/// </summary>
//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Native public static final int TYPE_CMY = 11;
		public const int TYPE_CMY = 11;

		/// <summary>
		/// Generic 2 component color spaces.
		/// </summary>
//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Native public static final int TYPE_2CLR = 12;
		public const int TYPE_2CLR = 12;

		/// <summary>
		/// Generic 3 component color spaces.
		/// </summary>
//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Native public static final int TYPE_3CLR = 13;
		public const int TYPE_3CLR = 13;

		/// <summary>
		/// Generic 4 component color spaces.
		/// </summary>
//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Native public static final int TYPE_4CLR = 14;
		public const int TYPE_4CLR = 14;

		/// <summary>
		/// Generic 5 component color spaces.
		/// </summary>
//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Native public static final int TYPE_5CLR = 15;
		public const int TYPE_5CLR = 15;

		/// <summary>
		/// Generic 6 component color spaces.
		/// </summary>
//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Native public static final int TYPE_6CLR = 16;
		public const int TYPE_6CLR = 16;

		/// <summary>
		/// Generic 7 component color spaces.
		/// </summary>
//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Native public static final int TYPE_7CLR = 17;
		public const int TYPE_7CLR = 17;

		/// <summary>
		/// Generic 8 component color spaces.
		/// </summary>
//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Native public static final int TYPE_8CLR = 18;
		public const int TYPE_8CLR = 18;

		/// <summary>
		/// Generic 9 component color spaces.
		/// </summary>
//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Native public static final int TYPE_9CLR = 19;
		public const int TYPE_9CLR = 19;

		/// <summary>
		/// Generic 10 component color spaces.
		/// </summary>
//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Native public static final int TYPE_ACLR = 20;
		public const int TYPE_ACLR = 20;

		/// <summary>
		/// Generic 11 component color spaces.
		/// </summary>
//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Native public static final int TYPE_BCLR = 21;
		public const int TYPE_BCLR = 21;

		/// <summary>
		/// Generic 12 component color spaces.
		/// </summary>
//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Native public static final int TYPE_CCLR = 22;
		public const int TYPE_CCLR = 22;

		/// <summary>
		/// Generic 13 component color spaces.
		/// </summary>
//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Native public static final int TYPE_DCLR = 23;
		public const int TYPE_DCLR = 23;

		/// <summary>
		/// Generic 14 component color spaces.
		/// </summary>
//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Native public static final int TYPE_ECLR = 24;
		public const int TYPE_ECLR = 24;

		/// <summary>
		/// Generic 15 component color spaces.
		/// </summary>
//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Native public static final int TYPE_FCLR = 25;
		public const int TYPE_FCLR = 25;

		/// <summary>
		/// The sRGB color space defined at
		/// <A href="http://www.w3.org/pub/WWW/Graphics/Color/sRGB.html">
		/// http://www.w3.org/pub/WWW/Graphics/Color/sRGB.html
		/// </A>.
		/// </summary>
//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Native public static final int CS_sRGB = 1000;
		public const int CS_sRGB_Renamed = 1000;

		/// <summary>
		/// A built-in linear RGB color space.  This space is based on the
		/// same RGB primaries as CS_sRGB, but has a linear tone reproduction curve.
		/// </summary>
//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Native public static final int CS_LINEAR_RGB = 1004;
		public const int CS_LINEAR_RGB = 1004;

		/// <summary>
		/// The CIEXYZ conversion color space defined above.
		/// </summary>
//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Native public static final int CS_CIEXYZ = 1001;
		public const int CS_CIEXYZ = 1001;

		/// <summary>
		/// The Photo YCC conversion color space.
		/// </summary>
//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Native public static final int CS_PYCC = 1002;
		public const int CS_PYCC = 1002;

		/// <summary>
		/// The built-in linear gray scale color space.
		/// </summary>
//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Native public static final int CS_GRAY = 1003;
		public const int CS_GRAY = 1003;


		/// <summary>
		/// Constructs a ColorSpace object given a color space type
		/// and the number of components. </summary>
		/// <param name="type"> one of the <CODE>ColorSpace</CODE> type constants </param>
		/// <param name="numcomponents"> the number of components in the color space </param>
		protected internal ColorSpace(int type, int numcomponents)
		{
			this.Type_Renamed = type;
			this.NumComponents_Renamed = numcomponents;
		}


		/// <summary>
		/// Returns a ColorSpace representing one of the specific
		/// predefined color spaces. </summary>
		/// <param name="colorspace"> a specific color space identified by one of
		///        the predefined class constants (e.g. CS_sRGB, CS_LINEAR_RGB,
		///        CS_CIEXYZ, CS_GRAY, or CS_PYCC) </param>
		/// <returns> the requested <CODE>ColorSpace</CODE> object </returns>
		// NOTE: This method may be called by privileged threads.
		//       DO NOT INVOKE CLIENT CODE ON THIS THREAD!
		public static ColorSpace GetInstance(int colorspace)
		{
		ColorSpace theColorSpace;

			switch (colorspace)
			{
			case CS_sRGB_Renamed:
				lock (typeof(ColorSpace))
				{
					if (SRGBspace == null)
					{
						ICC_Profile theProfile = ICC_Profile.GetInstance(CS_sRGB_Renamed);
						SRGBspace = new ICC_ColorSpace(theProfile);
					}

					theColorSpace = SRGBspace;
				}
				break;

			case CS_CIEXYZ:
				lock (typeof(ColorSpace))
				{
					if (XYZspace == null)
					{
						ICC_Profile theProfile = ICC_Profile.GetInstance(CS_CIEXYZ);
						XYZspace = new ICC_ColorSpace(theProfile);
					}

					theColorSpace = XYZspace;
				}
				break;

			case CS_PYCC:
				lock (typeof(ColorSpace))
				{
					if (PYCCspace == null)
					{
						ICC_Profile theProfile = ICC_Profile.GetInstance(CS_PYCC);
						PYCCspace = new ICC_ColorSpace(theProfile);
					}

					theColorSpace = PYCCspace;
				}
				break;


			case CS_GRAY:
				lock (typeof(ColorSpace))
				{
					if (GRAYspace == null)
					{
						ICC_Profile theProfile = ICC_Profile.GetInstance(CS_GRAY);
						GRAYspace = new ICC_ColorSpace(theProfile);
						/* to allow access from java.awt.ColorModel */
						CMSManager.GRAYspace = GRAYspace;
					}

					theColorSpace = GRAYspace;
				}
				break;


			case CS_LINEAR_RGB:
				lock (typeof(ColorSpace))
				{
					if (LINEAR_RGBspace == null)
					{
						ICC_Profile theProfile = ICC_Profile.GetInstance(CS_LINEAR_RGB);
						LINEAR_RGBspace = new ICC_ColorSpace(theProfile);
						/* to allow access from java.awt.ColorModel */
						CMSManager.LINEAR_RGBspace = LINEAR_RGBspace;
					}

					theColorSpace = LINEAR_RGBspace;
				}
				break;


			default:
				throw new IllegalArgumentException("Unknown color space");
			}

			return theColorSpace;
		}


		/// <summary>
		/// Returns true if the ColorSpace is CS_sRGB. </summary>
		/// <returns> <CODE>true</CODE> if this is a <CODE>CS_sRGB</CODE> color
		///         space, <code>false</code> if it is not </returns>
		public virtual bool CS_sRGB
		{
			get
			{
				/* REMIND - make sure we know sRGBspace exists already */
				return (this == SRGBspace);
			}
		}

		/// <summary>
		/// Transforms a color value assumed to be in this ColorSpace
		/// into a value in the default CS_sRGB color space.
		/// <para>
		/// This method transforms color values using algorithms designed
		/// to produce the best perceptual match between input and output
		/// colors.  In order to do colorimetric conversion of color values,
		/// you should use the <code>toCIEXYZ</code>
		/// method of this color space to first convert from the input
		/// color space to the CS_CIEXYZ color space, and then use the
		/// <code>fromCIEXYZ</code> method of the CS_sRGB color space to
		/// convert from CS_CIEXYZ to the output color space.
		/// See <seealso cref="#toCIEXYZ(float[]) toCIEXYZ"/> and
		/// <seealso cref="#fromCIEXYZ(float[]) fromCIEXYZ"/> for further information.
		/// </para>
		/// <para>
		/// </para>
		/// </summary>
		/// <param name="colorvalue"> a float array with length of at least the number
		///        of components in this ColorSpace </param>
		/// <returns> a float array of length 3 </returns>
		/// <exception cref="ArrayIndexOutOfBoundsException"> if array length is not
		///         at least the number of components in this ColorSpace </exception>
		public abstract float[] ToRGB(float[] colorvalue);


		/// <summary>
		/// Transforms a color value assumed to be in the default CS_sRGB
		/// color space into this ColorSpace.
		/// <para>
		/// This method transforms color values using algorithms designed
		/// to produce the best perceptual match between input and output
		/// colors.  In order to do colorimetric conversion of color values,
		/// you should use the <code>toCIEXYZ</code>
		/// method of the CS_sRGB color space to first convert from the input
		/// color space to the CS_CIEXYZ color space, and then use the
		/// <code>fromCIEXYZ</code> method of this color space to
		/// convert from CS_CIEXYZ to the output color space.
		/// See <seealso cref="#toCIEXYZ(float[]) toCIEXYZ"/> and
		/// <seealso cref="#fromCIEXYZ(float[]) fromCIEXYZ"/> for further information.
		/// </para>
		/// <para>
		/// </para>
		/// </summary>
		/// <param name="rgbvalue"> a float array with length of at least 3 </param>
		/// <returns> a float array with length equal to the number of
		///         components in this ColorSpace </returns>
		/// <exception cref="ArrayIndexOutOfBoundsException"> if array length is not
		///         at least 3 </exception>
		public abstract float[] FromRGB(float[] rgbvalue);


		/// <summary>
		/// Transforms a color value assumed to be in this ColorSpace
		/// into the CS_CIEXYZ conversion color space.
		/// <para>
		/// This method transforms color values using relative colorimetry,
		/// as defined by the International Color Consortium standard.  This
		/// means that the XYZ values returned by this method are represented
		/// relative to the D50 white point of the CS_CIEXYZ color space.
		/// This representation is useful in a two-step color conversion
		/// process in which colors are transformed from an input color
		/// space to CS_CIEXYZ and then to an output color space.  This
		/// representation is not the same as the XYZ values that would
		/// be measured from the given color value by a colorimeter.
		/// A further transformation is necessary to compute the XYZ values
		/// that would be measured using current CIE recommended practices.
		/// See the <seealso cref="ICC_ColorSpace#toCIEXYZ(float[]) toCIEXYZ"/> method of
		/// <code>ICC_ColorSpace</code> for further information.
		/// </para>
		/// <para>
		/// </para>
		/// </summary>
		/// <param name="colorvalue"> a float array with length of at least the number
		///        of components in this ColorSpace </param>
		/// <returns> a float array of length 3 </returns>
		/// <exception cref="ArrayIndexOutOfBoundsException"> if array length is not
		///         at least the number of components in this ColorSpace. </exception>
		public abstract float[] ToCIEXYZ(float[] colorvalue);


		/// <summary>
		/// Transforms a color value assumed to be in the CS_CIEXYZ conversion
		/// color space into this ColorSpace.
		/// <para>
		/// This method transforms color values using relative colorimetry,
		/// as defined by the International Color Consortium standard.  This
		/// means that the XYZ argument values taken by this method are represented
		/// relative to the D50 white point of the CS_CIEXYZ color space.
		/// This representation is useful in a two-step color conversion
		/// process in which colors are transformed from an input color
		/// space to CS_CIEXYZ and then to an output color space.  The color
		/// values returned by this method are not those that would produce
		/// the XYZ value passed to the method when measured by a colorimeter.
		/// If you have XYZ values corresponding to measurements made using
		/// current CIE recommended practices, they must be converted to D50
		/// relative values before being passed to this method.
		/// See the <seealso cref="ICC_ColorSpace#fromCIEXYZ(float[]) fromCIEXYZ"/> method of
		/// <code>ICC_ColorSpace</code> for further information.
		/// </para>
		/// <para>
		/// </para>
		/// </summary>
		/// <param name="colorvalue"> a float array with length of at least 3 </param>
		/// <returns> a float array with length equal to the number of
		///         components in this ColorSpace </returns>
		/// <exception cref="ArrayIndexOutOfBoundsException"> if array length is not
		///         at least 3 </exception>
		public abstract float[] FromCIEXYZ(float[] colorvalue);

		/// <summary>
		/// Returns the color space type of this ColorSpace (for example
		/// TYPE_RGB, TYPE_XYZ, ...).  The type defines the
		/// number of components of the color space and the interpretation,
		/// e.g. TYPE_RGB identifies a color space with three components - red,
		/// green, and blue.  It does not define the particular color
		/// characteristics of the space, e.g. the chromaticities of the
		/// primaries.
		/// </summary>
		/// <returns> the type constant that represents the type of this
		///         <CODE>ColorSpace</CODE> </returns>
		public virtual int Type
		{
			get
			{
				return Type_Renamed;
			}
		}

		/// <summary>
		/// Returns the number of components of this ColorSpace. </summary>
		/// <returns> The number of components in this <CODE>ColorSpace</CODE>. </returns>
		public virtual int NumComponents
		{
			get
			{
				return NumComponents_Renamed;
			}
		}

		/// <summary>
		/// Returns the name of the component given the component index.
		/// </summary>
		/// <param name="idx"> the component index </param>
		/// <returns> the name of the component at the specified index </returns>
		/// <exception cref="IllegalArgumentException"> if <code>idx</code> is
		///         less than 0 or greater than numComponents - 1 </exception>
		public virtual String GetName(int idx)
		{
			/* REMIND - handle common cases here */
			if ((idx < 0) || (idx > NumComponents_Renamed - 1))
			{
				throw new IllegalArgumentException("Component index out of range: " + idx);
			}

			if (CompName == null)
			{
				switch (Type_Renamed)
				{
					case ColorSpace.TYPE_XYZ:
						CompName = new String[] {"X", "Y", "Z"};
						break;
					case ColorSpace.TYPE_Lab:
						CompName = new String[] {"L", "a", "b"};
						break;
					case ColorSpace.TYPE_Luv:
						CompName = new String[] {"L", "u", "v"};
						break;
					case ColorSpace.TYPE_YCbCr:
						CompName = new String[] {"Y", "Cb", "Cr"};
						break;
					case ColorSpace.TYPE_Yxy:
						CompName = new String[] {"Y", "x", "y"};
						break;
					case ColorSpace.TYPE_RGB:
						CompName = new String[] {"Red", "Green", "Blue"};
						break;
					case ColorSpace.TYPE_GRAY:
						CompName = new String[] {"Gray"};
						break;
					case ColorSpace.TYPE_HSV:
						CompName = new String[] {"Hue", "Saturation", "Value"};
						break;
					case ColorSpace.TYPE_HLS:
						CompName = new String[] {"Hue", "Lightness", "Saturation"};
						break;
					case ColorSpace.TYPE_CMYK:
						CompName = new String[] {"Cyan", "Magenta", "Yellow", "Black"};
						break;
					case ColorSpace.TYPE_CMY:
						CompName = new String[] {"Cyan", "Magenta", "Yellow"};
						break;
					default:
						String[] tmp = new String[NumComponents_Renamed];
						for (int i = 0; i < tmp.Length; i++)
						{
							tmp[i] = "Unnamed color component(" + i + ")";
						}
						CompName = tmp;
					break;
				}
			}
			return CompName[idx];
		}

		/// <summary>
		/// Returns the minimum normalized color component value for the
		/// specified component.  The default implementation in this abstract
		/// class returns 0.0 for all components.  Subclasses should override
		/// this method if necessary.
		/// </summary>
		/// <param name="component"> the component index </param>
		/// <returns> the minimum normalized component value </returns>
		/// <exception cref="IllegalArgumentException"> if component is less than 0 or
		///         greater than numComponents - 1
		/// @since 1.4 </exception>
		public virtual float GetMinValue(int component)
		{
			if ((component < 0) || (component > NumComponents_Renamed - 1))
			{
				throw new IllegalArgumentException("Component index out of range: " + component);
			}
			return 0.0f;
		}

		/// <summary>
		/// Returns the maximum normalized color component value for the
		/// specified component.  The default implementation in this abstract
		/// class returns 1.0 for all components.  Subclasses should override
		/// this method if necessary.
		/// </summary>
		/// <param name="component"> the component index </param>
		/// <returns> the maximum normalized component value </returns>
		/// <exception cref="IllegalArgumentException"> if component is less than 0 or
		///         greater than numComponents - 1
		/// @since 1.4 </exception>
		public virtual float GetMaxValue(int component)
		{
			if ((component < 0) || (component > NumComponents_Renamed - 1))
			{
				throw new IllegalArgumentException("Component index out of range: " + component);
			}
			return 1.0f;
		}

		/* Returns true if cspace is the XYZspace.
		 */
		internal static bool IsCS_CIEXYZ(ColorSpace cspace)
		{
			return (cspace == XYZspace);
		}
	}

}
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

namespace java.awt.image
{


	/// <summary>
	/// A <CODE>ColorModel</CODE> class that works with pixel values that
	/// represent color and alpha information as separate samples and that
	/// store each sample in a separate data element.  This class can be
	/// used with an arbitrary <CODE>ColorSpace</CODE>.  The number of
	/// color samples in the pixel values must be same as the number of
	/// color components in the <CODE>ColorSpace</CODE>. There may be a
	/// single alpha sample.
	/// <para>
	/// For those methods that use
	/// a primitive array pixel representation of type <CODE>transferType</CODE>,
	/// the array length is the same as the number of color and alpha samples.
	/// Color samples are stored first in the array followed by the alpha
	/// sample, if present.  The order of the color samples is specified
	/// by the <CODE>ColorSpace</CODE>.  Typically, this order reflects the
	/// name of the color space type. For example, for <CODE>TYPE_RGB</CODE>,
	/// index 0 corresponds to red, index 1 to green, and index 2 to blue.
	/// </para>
	/// <para>
	/// The translation from pixel sample values to color/alpha components for
	/// display or processing purposes is based on a one-to-one correspondence of
	/// samples to components.
	/// Depending on the transfer type used to create an instance of
	/// <code>ComponentColorModel</code>, the pixel sample values
	/// represented by that instance may be signed or unsigned and may
	/// be of integral type or float or double (see below for details).
	/// The translation from sample values to normalized color/alpha components
	/// must follow certain rules.  For float and double samples, the translation
	/// is an identity, i.e. normalized component values are equal to the
	/// corresponding sample values.  For integral samples, the translation
	/// should be only a simple scale and offset, where the scale and offset
	/// constants may be different for each component.  The result of
	/// applying the scale and offset constants is a set of color/alpha
	/// component values, which are guaranteed to fall within a certain
	/// range.  Typically, the range for a color component will be the range
	/// defined by the <code>getMinValue</code> and <code>getMaxValue</code>
	/// methods of the <code>ColorSpace</code> class.  The range for an
	/// alpha component should be 0.0 to 1.0.
	/// </para>
	/// <para>
	/// Instances of <code>ComponentColorModel</code> created with transfer types
	/// <CODE>DataBuffer.TYPE_BYTE</CODE>, <CODE>DataBuffer.TYPE_USHORT</CODE>,
	/// and <CODE>DataBuffer.TYPE_INT</CODE> have pixel sample values which
	/// are treated as unsigned integral values.
	/// The number of bits in a color or alpha sample of a pixel value might not
	/// be the same as the number of bits for the corresponding color or alpha
	/// sample passed to the
	/// <code>ComponentColorModel(ColorSpace, int[], boolean, boolean, int, int)</code>
	/// constructor.  In
	/// that case, this class assumes that the least significant n bits of a sample
	/// value hold the component value, where n is the number of significant bits
	/// for the component passed to the constructor.  It also assumes that
	/// any higher-order bits in a sample value are zero.  Thus, sample values
	/// range from 0 to 2<sup>n</sup> - 1.  This class maps these sample values
	/// to normalized color component values such that 0 maps to the value
	/// obtained from the <code>ColorSpace's</code> <code>getMinValue</code>
	/// method for each component and 2<sup>n</sup> - 1 maps to the value
	/// obtained from <code>getMaxValue</code>.  To create a
	/// <code>ComponentColorModel</code> with a different color sample mapping
	/// requires subclassing this class and overriding the
	/// <code>getNormalizedComponents(Object, float[], int)</code> method.
	/// The mapping for an alpha sample always maps 0 to 0.0 and
	/// 2<sup>n</sup> - 1 to 1.0.
	/// </para>
	/// <para>
	/// For instances with unsigned sample values,
	/// the unnormalized color/alpha component representation is only
	/// supported if two conditions hold.  First, sample value value 0 must
	/// map to normalized component value 0.0 and sample value 2<sup>n</sup> - 1
	/// to 1.0.  Second the min/max range of all color components of the
	/// <code>ColorSpace</code> must be 0.0 to 1.0.  In this case, the
	/// component representation is the n least
	/// significant bits of the corresponding sample.  Thus each component is
	/// an unsigned integral value between 0 and 2<sup>n</sup> - 1, where
	/// n is the number of significant bits for a particular component.
	/// If these conditions are not met, any method taking an unnormalized
	/// component argument will throw an <code>IllegalArgumentException</code>.
	/// </para>
	/// <para>
	/// Instances of <code>ComponentColorModel</code> created with transfer types
	/// <CODE>DataBuffer.TYPE_SHORT</CODE>, <CODE>DataBuffer.TYPE_FLOAT</CODE>, and
	/// <CODE>DataBuffer.TYPE_DOUBLE</CODE> have pixel sample values which
	/// are treated as signed short, float, or double values.
	/// Such instances do not support the unnormalized color/alpha component
	/// representation, so any methods taking such a representation as an argument
	/// will throw an <code>IllegalArgumentException</code> when called on one
	/// of these instances.  The normalized component values of instances
	/// of this class have a range which depends on the transfer
	/// type as follows: for float samples, the full range of the float data
	/// type; for double samples, the full range of the float data type
	/// (resulting from casting double to float); for short samples,
	/// from approximately -maxVal to +maxVal, where maxVal is the per
	/// component maximum value for the <code>ColorSpace</code>
	/// (-32767 maps to -maxVal, 0 maps to 0.0, and 32767 maps
	/// to +maxVal).  A subclass may override the scaling for short sample
	/// values to normalized component values by overriding the
	/// <code>getNormalizedComponents(Object, float[], int)</code> method.
	/// For float and double samples, the normalized component values are
	/// taken to be equal to the corresponding sample values, and subclasses
	/// should not attempt to add any non-identity scaling for these transfer
	/// types.
	/// </para>
	/// <para>
	/// Instances of <code>ComponentColorModel</code> created with transfer types
	/// <CODE>DataBuffer.TYPE_SHORT</CODE>, <CODE>DataBuffer.TYPE_FLOAT</CODE>, and
	/// <CODE>DataBuffer.TYPE_DOUBLE</CODE>
	/// use all the bits of all sample values.  Thus all color/alpha components
	/// have 16 bits when using <CODE>DataBuffer.TYPE_SHORT</CODE>, 32 bits when
	/// using <CODE>DataBuffer.TYPE_FLOAT</CODE>, and 64 bits when using
	/// <CODE>DataBuffer.TYPE_DOUBLE</CODE>.  When the
	/// <code>ComponentColorModel(ColorSpace, int[], boolean, boolean, int, int)</code>
	/// form of constructor is used with one of these transfer types, the
	/// bits array argument is ignored.
	/// </para>
	/// <para>
	/// It is possible to have color/alpha sample values
	/// which cannot be reasonably interpreted as component values for rendering.
	/// This can happen when <code>ComponentColorModel</code> is subclassed to
	/// override the mapping of unsigned sample values to normalized color
	/// component values or when signed sample values outside a certain range
	/// are used.  (As an example, specifying an alpha component as a signed
	/// short value outside the range 0 to 32767, normalized range 0.0 to 1.0, can
	/// lead to unexpected results.) It is the
	/// responsibility of applications to appropriately scale pixel data before
	/// rendering such that color components fall within the normalized range
	/// of the <code>ColorSpace</code> (obtained using the <code>getMinValue</code>
	/// and <code>getMaxValue</code> methods of the <code>ColorSpace</code> class)
	/// and the alpha component is between 0.0 and 1.0.  If color or alpha
	/// component values fall outside these ranges, rendering results are
	/// indeterminate.
	/// </para>
	/// <para>
	/// Methods that use a single int pixel representation throw
	/// an <CODE>IllegalArgumentException</CODE>, unless the number of components
	/// for the <CODE>ComponentColorModel</CODE> is one and the component
	/// value is unsigned -- in other words,  a single color component using
	/// a transfer type of <CODE>DataBuffer.TYPE_BYTE</CODE>,
	/// <CODE>DataBuffer.TYPE_USHORT</CODE>, or <CODE>DataBuffer.TYPE_INT</CODE>
	/// and no alpha.
	/// </para>
	/// <para>
	/// A <CODE>ComponentColorModel</CODE> can be used in conjunction with a
	/// <CODE>ComponentSampleModel</CODE>, a <CODE>BandedSampleModel</CODE>,
	/// or a <CODE>PixelInterleavedSampleModel</CODE> to construct a
	/// <CODE>BufferedImage</CODE>.
	/// 
	/// </para>
	/// </summary>
	/// <seealso cref= ColorModel </seealso>
	/// <seealso cref= ColorSpace </seealso>
	/// <seealso cref= ComponentSampleModel </seealso>
	/// <seealso cref= BandedSampleModel </seealso>
	/// <seealso cref= PixelInterleavedSampleModel </seealso>
	/// <seealso cref= BufferedImage
	///  </seealso>
	public class ComponentColorModel : ColorModel
	{

		/// <summary>
		/// <code>signed</code>  is <code>true</code> for <code>short</code>,
		/// <code>float</code>, and <code>double</code> transfer types; it
		/// is <code>false</code> for <code>byte</code>, <code>ushort</code>,
		/// and <code>int</code> transfer types.
		/// </summary>
		private bool Signed; // true for transfer types short, float, double
								// false for byte, ushort, int
		private bool Is_sRGB_stdScale;
		private bool Is_LinearRGB_stdScale;
		private bool Is_LinearGray_stdScale;
		private bool Is_ICCGray_stdScale;
		private sbyte[] TosRGB8LUT;
		private sbyte[] FromsRGB8LUT8;
		private short[] FromsRGB8LUT16;
		private sbyte[] FromLinearGray16ToOtherGray8LUT;
		private short[] FromLinearGray16ToOtherGray16LUT;
		private bool NeedScaleInit;
		private bool NoUnnorm;
		private bool NonStdScale;
		private float[] Min;
		private float[] DiffMinMax;
		private float[] CompOffset;
		private float[] CompScale;

		/// <summary>
		/// Constructs a <CODE>ComponentColorModel</CODE> from the specified
		/// parameters. Color components will be in the specified
		/// <CODE>ColorSpace</CODE>.  The supported transfer types are
		/// <CODE>DataBuffer.TYPE_BYTE</CODE>, <CODE>DataBuffer.TYPE_USHORT</CODE>,
		/// <CODE>DataBuffer.TYPE_INT</CODE>,
		/// <CODE>DataBuffer.TYPE_SHORT</CODE>, <CODE>DataBuffer.TYPE_FLOAT</CODE>,
		/// and <CODE>DataBuffer.TYPE_DOUBLE</CODE>.
		/// If not null, the <CODE>bits</CODE> array specifies the
		/// number of significant bits per color and alpha component and its
		/// length should be at least the number of components in the
		/// <CODE>ColorSpace</CODE> if there is no alpha
		/// information in the pixel values, or one more than this number if
		/// there is alpha information.  When the <CODE>transferType</CODE> is
		/// <CODE>DataBuffer.TYPE_SHORT</CODE>, <CODE>DataBuffer.TYPE_FLOAT</CODE>,
		/// or <CODE>DataBuffer.TYPE_DOUBLE</CODE> the <CODE>bits</CODE> array
		/// argument is ignored.  <CODE>hasAlpha</CODE> indicates whether alpha
		/// information is present.  If <CODE>hasAlpha</CODE> is true, then
		/// the boolean <CODE>isAlphaPremultiplied</CODE>
		/// specifies how to interpret color and alpha samples in pixel values.
		/// If the boolean is true, color samples are assumed to have been
		/// multiplied by the alpha sample. The <CODE>transparency</CODE>
		/// specifies what alpha values can be represented by this color model.
		/// The acceptable <code>transparency</code> values are
		/// <CODE>OPAQUE</CODE>, <CODE>BITMASK</CODE> or <CODE>TRANSLUCENT</CODE>.
		/// The <CODE>transferType</CODE> is the type of primitive array used
		/// to represent pixel values.
		/// </summary>
		/// <param name="colorSpace">       The <CODE>ColorSpace</CODE> associated
		///                         with this color model. </param>
		/// <param name="bits">             The number of significant bits per component.
		///                         May be null, in which case all bits of all
		///                         component samples will be significant.
		///                         Ignored if transferType is one of
		///                         <CODE>DataBuffer.TYPE_SHORT</CODE>,
		///                         <CODE>DataBuffer.TYPE_FLOAT</CODE>, or
		///                         <CODE>DataBuffer.TYPE_DOUBLE</CODE>,
		///                         in which case all bits of all component
		///                         samples will be significant. </param>
		/// <param name="hasAlpha">         If true, this color model supports alpha. </param>
		/// <param name="isAlphaPremultiplied"> If true, alpha is premultiplied. </param>
		/// <param name="transparency">     Specifies what alpha values can be represented
		///                         by this color model. </param>
		/// <param name="transferType">     Specifies the type of primitive array used to
		///                         represent pixel values.
		/// </param>
		/// <exception cref="IllegalArgumentException"> If the <CODE>bits</CODE> array
		///         argument is not null, its length is less than the number of
		///         color and alpha components, and transferType is one of
		///         <CODE>DataBuffer.TYPE_BYTE</CODE>,
		///         <CODE>DataBuffer.TYPE_USHORT</CODE>, or
		///         <CODE>DataBuffer.TYPE_INT</CODE>. </exception>
		/// <exception cref="IllegalArgumentException"> If transferType is not one of
		///         <CODE>DataBuffer.TYPE_BYTE</CODE>,
		///         <CODE>DataBuffer.TYPE_USHORT</CODE>,
		///         <CODE>DataBuffer.TYPE_INT</CODE>,
		///         <CODE>DataBuffer.TYPE_SHORT</CODE>,
		///         <CODE>DataBuffer.TYPE_FLOAT</CODE>, or
		///         <CODE>DataBuffer.TYPE_DOUBLE</CODE>.
		/// </exception>
		/// <seealso cref= ColorSpace </seealso>
		/// <seealso cref= java.awt.Transparency </seealso>
		public ComponentColorModel(ColorSpace colorSpace, int[] bits, bool hasAlpha, bool isAlphaPremultiplied, int transparency, int transferType) : base(BitsHelper(transferType, colorSpace, hasAlpha), BitsArrayHelper(bits, transferType, colorSpace, hasAlpha), colorSpace, hasAlpha, isAlphaPremultiplied, transparency, transferType)
		{
			switch (transferType)
			{
				case DataBuffer.TYPE_BYTE:
				case DataBuffer.TYPE_USHORT:
				case DataBuffer.TYPE_INT:
					Signed = false;
					NeedScaleInit = true;
					break;
				case DataBuffer.TYPE_SHORT:
					Signed = true;
					NeedScaleInit = true;
					break;
				case DataBuffer.TYPE_FLOAT:
				case DataBuffer.TYPE_DOUBLE:
					Signed = true;
					NeedScaleInit = false;
					NoUnnorm = true;
					NonStdScale = false;
					break;
				default:
					throw new IllegalArgumentException("This constructor is not " + "compatible with transferType " + transferType);
			}
			SetupLUTs();
		}

		/// <summary>
		/// Constructs a <CODE>ComponentColorModel</CODE> from the specified
		/// parameters. Color components will be in the specified
		/// <CODE>ColorSpace</CODE>.  The supported transfer types are
		/// <CODE>DataBuffer.TYPE_BYTE</CODE>, <CODE>DataBuffer.TYPE_USHORT</CODE>,
		/// <CODE>DataBuffer.TYPE_INT</CODE>,
		/// <CODE>DataBuffer.TYPE_SHORT</CODE>, <CODE>DataBuffer.TYPE_FLOAT</CODE>,
		/// and <CODE>DataBuffer.TYPE_DOUBLE</CODE>.  The number of significant
		/// bits per color and alpha component will be 8, 16, 32, 16, 32,  or 64,
		/// respectively.  The number of color components will be the
		/// number of components in the <CODE>ColorSpace</CODE>.  There will be
		/// an alpha component if <CODE>hasAlpha</CODE> is <CODE>true</CODE>.
		/// If <CODE>hasAlpha</CODE> is true, then
		/// the boolean <CODE>isAlphaPremultiplied</CODE>
		/// specifies how to interpret color and alpha samples in pixel values.
		/// If the boolean is true, color samples are assumed to have been
		/// multiplied by the alpha sample. The <CODE>transparency</CODE>
		/// specifies what alpha values can be represented by this color model.
		/// The acceptable <code>transparency</code> values are
		/// <CODE>OPAQUE</CODE>, <CODE>BITMASK</CODE> or <CODE>TRANSLUCENT</CODE>.
		/// The <CODE>transferType</CODE> is the type of primitive array used
		/// to represent pixel values.
		/// </summary>
		/// <param name="colorSpace">       The <CODE>ColorSpace</CODE> associated
		///                         with this color model. </param>
		/// <param name="hasAlpha">         If true, this color model supports alpha. </param>
		/// <param name="isAlphaPremultiplied"> If true, alpha is premultiplied. </param>
		/// <param name="transparency">     Specifies what alpha values can be represented
		///                         by this color model. </param>
		/// <param name="transferType">     Specifies the type of primitive array used to
		///                         represent pixel values.
		/// </param>
		/// <exception cref="IllegalArgumentException"> If transferType is not one of
		///         <CODE>DataBuffer.TYPE_BYTE</CODE>,
		///         <CODE>DataBuffer.TYPE_USHORT</CODE>,
		///         <CODE>DataBuffer.TYPE_INT</CODE>,
		///         <CODE>DataBuffer.TYPE_SHORT</CODE>,
		///         <CODE>DataBuffer.TYPE_FLOAT</CODE>, or
		///         <CODE>DataBuffer.TYPE_DOUBLE</CODE>.
		/// </exception>
		/// <seealso cref= ColorSpace </seealso>
		/// <seealso cref= java.awt.Transparency
		/// @since 1.4 </seealso>
		public ComponentColorModel(ColorSpace colorSpace, bool hasAlpha, bool isAlphaPremultiplied, int transparency, int transferType) : this(colorSpace, null, hasAlpha, isAlphaPremultiplied, transparency, transferType)
		{
		}

		private static int BitsHelper(int transferType, ColorSpace colorSpace, bool hasAlpha)
		{
			int numBits = DataBuffer.GetDataTypeSize(transferType);
			int numComponents = colorSpace.NumComponents;
			if (hasAlpha)
			{
				++numComponents;
			}
			return numBits * numComponents;
		}

		private static int[] BitsArrayHelper(int[] origBits, int transferType, ColorSpace colorSpace, bool hasAlpha)
		{
			switch (transferType)
			{
				case DataBuffer.TYPE_BYTE:
				case DataBuffer.TYPE_USHORT:
				case DataBuffer.TYPE_INT:
					if (origBits != null)
					{
						return origBits;
					}
					break;
				default:
					break;
			}
			int numBits = DataBuffer.GetDataTypeSize(transferType);
			int numComponents = colorSpace.NumComponents;
			if (hasAlpha)
			{
				++numComponents;
			}
			int[] bits = new int[numComponents];
			for (int i = 0; i < numComponents; i++)
			{
				bits[i] = numBits;
			}
			return bits;
		}

		private void SetupLUTs()
		{
			// REMIND: there is potential to accelerate sRGB, LinearRGB,
			// LinearGray, ICCGray, and non-ICC Gray spaces with non-standard
			// scaling, if that becomes important
			//
			// NOTE: The is_xxx_stdScale and nonStdScale booleans are provisionally
			// set here when this method is called at construction time.  These
			// variables may be set again when initScale is called later.
			// When setupLUTs returns, nonStdScale is true if (the transferType
			// is not float or double) AND (some minimum ColorSpace component
			// value is not 0.0 OR some maximum ColorSpace component value
			// is not 1.0).  This is correct for the calls to
			// getNormalizedComponents(Object, float[], int) from initScale().
			// initScale() may change the value nonStdScale based on the
			// return value of getNormalizedComponents() - this will only
			// happen if getNormalizedComponents() has been overridden by a
			// subclass to make the mapping of min/max pixel sample values
			// something different from min/max color component values.
			if (Is_sRGB)
			{
				Is_sRGB_stdScale = true;
				NonStdScale = false;
			}
			else if (ColorModel.IsLinearRGBspace(ColorSpace_Renamed))
			{
				// Note that the built-in Linear RGB space has a normalized
				// range of 0.0 - 1.0 for each coordinate.  Usage of these
				// LUTs makes that assumption.
				Is_LinearRGB_stdScale = true;
				NonStdScale = false;
				if (TransferType_Renamed == DataBuffer.TYPE_BYTE)
				{
					TosRGB8LUT = ColorModel.LinearRGB8TosRGB8LUT;
					FromsRGB8LUT8 = ColorModel.GetsRGB8ToLinearRGB8LUT();
				}
				else
				{
					TosRGB8LUT = ColorModel.LinearRGB16TosRGB8LUT;
					FromsRGB8LUT16 = ColorModel.GetsRGB8ToLinearRGB16LUT();
				}
			}
			else if ((ColorSpaceType == ColorSpace.TYPE_GRAY) && (ColorSpace_Renamed is ICC_ColorSpace) && (ColorSpace_Renamed.GetMinValue(0) == 0.0f) && (ColorSpace_Renamed.GetMaxValue(0) == 1.0f))
			{
				// Note that a normalized range of 0.0 - 1.0 for the gray
				// component is required, because usage of these LUTs makes
				// that assumption.
				ICC_ColorSpace ics = (ICC_ColorSpace) ColorSpace_Renamed;
				Is_ICCGray_stdScale = true;
				NonStdScale = false;
				FromsRGB8LUT16 = ColorModel.GetsRGB8ToLinearRGB16LUT();
				if (ColorModel.IsLinearGRAYspace(ics))
				{
					Is_LinearGray_stdScale = true;
					if (TransferType_Renamed == DataBuffer.TYPE_BYTE)
					{
						TosRGB8LUT = ColorModel.GetGray8TosRGB8LUT(ics);
					}
					else
					{
						TosRGB8LUT = ColorModel.GetGray16TosRGB8LUT(ics);
					}
				}
				else
				{
					if (TransferType_Renamed == DataBuffer.TYPE_BYTE)
					{
						TosRGB8LUT = ColorModel.GetGray8TosRGB8LUT(ics);
						FromLinearGray16ToOtherGray8LUT = ColorModel.GetLinearGray16ToOtherGray8LUT(ics);
					}
					else
					{
						TosRGB8LUT = ColorModel.GetGray16TosRGB8LUT(ics);
						FromLinearGray16ToOtherGray16LUT = ColorModel.GetLinearGray16ToOtherGray16LUT(ics);
					}
				}
			}
			else if (NeedScaleInit)
			{
				// if transferType is byte, ushort, int, or short and we
				// don't already know the ColorSpace has minVlaue == 0.0f and
				// maxValue == 1.0f for all components, we need to check that
				// now and setup the min[] and diffMinMax[] arrays if necessary.
				NonStdScale = false;
				for (int i = 0; i < NumColorComponents_Renamed; i++)
				{
					if ((ColorSpace_Renamed.GetMinValue(i) != 0.0f) || (ColorSpace_Renamed.GetMaxValue(i) != 1.0f))
					{
						NonStdScale = true;
						break;
					}
				}
				if (NonStdScale)
				{
					Min = new float[NumColorComponents_Renamed];
					DiffMinMax = new float[NumColorComponents_Renamed];
					for (int i = 0; i < NumColorComponents_Renamed; i++)
					{
						Min[i] = ColorSpace_Renamed.GetMinValue(i);
						DiffMinMax[i] = ColorSpace_Renamed.GetMaxValue(i) - Min[i];
					}
				}
			}
		}

		private void InitScale()
		{
			// This method is called the first time any method which uses
			// pixel sample value to color component value scaling information
			// is called if the transferType supports non-standard scaling
			// as defined above (byte, ushort, int, and short), unless the
			// method is getNormalizedComponents(Object, float[], int) (that
			// method must be overridden to use non-standard scaling).  This
			// method also sets up the noUnnorm boolean variable for these
			// transferTypes.  After this method is called, the nonStdScale
			// variable will be true if getNormalizedComponents() maps a
			// sample value of 0 to anything other than 0.0f OR maps a
			// sample value of 2^^n - 1 (2^^15 - 1 for short transferType)
			// to anything other than 1.0f.  Note that this can be independent
			// of the colorSpace min/max component values, if the
			// getNormalizedComponents() method has been overridden for some
			// reason, e.g. to provide greater dynamic range in the sample
			// values than in the color component values.  Unfortunately,
			// this method can't be called at construction time, since a
			// subclass may still have uninitialized state that would cause
			// getNormalizedComponents() to return an incorrect result.
			NeedScaleInit = false; // only needs to called once
			if (NonStdScale || Signed)
			{
				// The unnormalized form is only supported for unsigned
				// transferTypes and when the ColorSpace min/max values
				// are 0.0/1.0.  When this method is called nonStdScale is
				// true if the latter condition does not hold.  In addition,
				// the unnormalized form requires that the full range of
				// the pixel sample values map to the full 0.0 - 1.0 range
				// of color component values.  That condition is checked
				// later in this method.
				NoUnnorm = true;
			}
			else
			{
				NoUnnorm = false;
			}
			float[] lowVal, highVal;
			switch (TransferType_Renamed)
			{
			case DataBuffer.TYPE_BYTE:
			{
					sbyte[] bpixel = new sbyte[NumComponents_Renamed];
					for (int i = 0; i < NumColorComponents_Renamed; i++)
					{
						bpixel[i] = 0;
					}
					if (SupportsAlpha)
					{
						bpixel[NumColorComponents_Renamed] = (sbyte)((1 << NBits[NumColorComponents_Renamed]) - 1);
					}
					lowVal = GetNormalizedComponents(bpixel, null, 0);
					for (int i = 0; i < NumColorComponents_Renamed; i++)
					{
						bpixel[i] = (sbyte)((1 << NBits[i]) - 1);
					}
					highVal = GetNormalizedComponents(bpixel, null, 0);
			}
				break;
			case DataBuffer.TYPE_USHORT:
			{
					short[] uspixel = new short[NumComponents_Renamed];
					for (int i = 0; i < NumColorComponents_Renamed; i++)
					{
						uspixel[i] = 0;
					}
					if (SupportsAlpha)
					{
						uspixel[NumColorComponents_Renamed] = (short)((1 << NBits[NumColorComponents_Renamed]) - 1);
					}
					lowVal = GetNormalizedComponents(uspixel, null, 0);
					for (int i = 0; i < NumColorComponents_Renamed; i++)
					{
						uspixel[i] = (short)((1 << NBits[i]) - 1);
					}
					highVal = GetNormalizedComponents(uspixel, null, 0);
			}
				break;
			case DataBuffer.TYPE_INT:
			{
					int[] ipixel = new int[NumComponents_Renamed];
					for (int i = 0; i < NumColorComponents_Renamed; i++)
					{
						ipixel[i] = 0;
					}
					if (SupportsAlpha)
					{
						ipixel[NumColorComponents_Renamed] = ((1 << NBits[NumColorComponents_Renamed]) - 1);
					}
					lowVal = GetNormalizedComponents(ipixel, null, 0);
					for (int i = 0; i < NumColorComponents_Renamed; i++)
					{
						ipixel[i] = ((1 << NBits[i]) - 1);
					}
					highVal = GetNormalizedComponents(ipixel, null, 0);
			}
				break;
			case DataBuffer.TYPE_SHORT:
			{
					short[] spixel = new short[NumComponents_Renamed];
					for (int i = 0; i < NumColorComponents_Renamed; i++)
					{
						spixel[i] = 0;
					}
					if (SupportsAlpha)
					{
						spixel[NumColorComponents_Renamed] = 32767;
					}
					lowVal = GetNormalizedComponents(spixel, null, 0);
					for (int i = 0; i < NumColorComponents_Renamed; i++)
					{
						spixel[i] = 32767;
					}
					highVal = GetNormalizedComponents(spixel, null, 0);
			}
				break;
			default:
				lowVal = highVal = null; // to keep the compiler from complaining
				break;
			}
			NonStdScale = false;
			for (int i = 0; i < NumColorComponents_Renamed; i++)
			{
				if ((lowVal[i] != 0.0f) || (highVal[i] != 1.0f))
				{
					NonStdScale = true;
					break;
				}
			}
			if (NonStdScale)
			{
				NoUnnorm = true;
				Is_sRGB_stdScale = false;
				Is_LinearRGB_stdScale = false;
				Is_LinearGray_stdScale = false;
				Is_ICCGray_stdScale = false;
				CompOffset = new float[NumColorComponents_Renamed];
				CompScale = new float[NumColorComponents_Renamed];
				for (int i = 0; i < NumColorComponents_Renamed; i++)
				{
					CompOffset[i] = lowVal[i];
					CompScale[i] = 1.0f / (highVal[i] - lowVal[i]);
				}
			}
		}

		private int GetRGBComponent(int pixel, int idx)
		{
			if (NumComponents_Renamed > 1)
			{
				throw new IllegalArgumentException("More than one component per pixel");
			}
			if (Signed)
			{
				throw new IllegalArgumentException("Component value is signed");
			}
			if (NeedScaleInit)
			{
				InitScale();
			}
			// Since there is only 1 component, there is no alpha

			// Normalize the pixel in order to convert it
			Object opixel = null;
			switch (TransferType_Renamed)
			{
			case DataBuffer.TYPE_BYTE:
			{
					sbyte[] bpixel = new sbyte[] {(sbyte) pixel};
					opixel = bpixel;
			}
				break;
			case DataBuffer.TYPE_USHORT:
			{
					short[] spixel = new short[] {(short) pixel};
					opixel = spixel;
			}
				break;
			case DataBuffer.TYPE_INT:
			{
					int[] ipixel = new int[] {pixel};
					opixel = ipixel;
			}
				break;
			}
			float[] norm = GetNormalizedComponents(opixel, null, 0);
			float[] rgb = ColorSpace_Renamed.ToRGB(norm);

			return (int)(rgb[idx] * 255.0f + 0.5f);
		}

		/// <summary>
		/// Returns the red color component for the specified pixel, scaled
		/// from 0 to 255 in the default RGB ColorSpace, sRGB.  A color conversion
		/// is done if necessary.  The pixel value is specified as an int.
		/// The returned value will be a non pre-multiplied value.
		/// If the alpha is premultiplied, this method divides
		/// it out before returning the value (if the alpha value is 0,
		/// the red value will be 0).
		/// </summary>
		/// <param name="pixel"> The pixel from which you want to get the red color component.
		/// </param>
		/// <returns> The red color component for the specified pixel, as an int.
		/// </returns>
		/// <exception cref="IllegalArgumentException"> If there is more than
		/// one component in this <CODE>ColorModel</CODE>. </exception>
		/// <exception cref="IllegalArgumentException"> If the component value for this
		/// <CODE>ColorModel</CODE> is signed </exception>
		public override int GetRed(int pixel)
		{
			return GetRGBComponent(pixel, 0);
		}

		/// <summary>
		/// Returns the green color component for the specified pixel, scaled
		/// from 0 to 255 in the default RGB ColorSpace, sRGB.  A color conversion
		/// is done if necessary.  The pixel value is specified as an int.
		/// The returned value will be a non
		/// pre-multiplied value. If the alpha is premultiplied, this method
		/// divides it out before returning the value (if the alpha value is 0,
		/// the green value will be 0).
		/// </summary>
		/// <param name="pixel"> The pixel from which you want to get the green color component.
		/// </param>
		/// <returns> The green color component for the specified pixel, as an int.
		/// </returns>
		/// <exception cref="IllegalArgumentException"> If there is more than
		/// one component in this <CODE>ColorModel</CODE>. </exception>
		/// <exception cref="IllegalArgumentException"> If the component value for this
		/// <CODE>ColorModel</CODE> is signed </exception>
		public override int GetGreen(int pixel)
		{
			return GetRGBComponent(pixel, 1);
		}

		/// <summary>
		/// Returns the blue color component for the specified pixel, scaled
		/// from 0 to 255 in the default RGB ColorSpace, sRGB.  A color conversion
		/// is done if necessary.  The pixel value is specified as an int.
		/// The returned value will be a non
		/// pre-multiplied value. If the alpha is premultiplied, this method
		/// divides it out before returning the value (if the alpha value is 0,
		/// the blue value will be 0).
		/// </summary>
		/// <param name="pixel"> The pixel from which you want to get the blue color component.
		/// </param>
		/// <returns> The blue color component for the specified pixel, as an int.
		/// </returns>
		/// <exception cref="IllegalArgumentException"> If there is more than
		/// one component in this <CODE>ColorModel</CODE>. </exception>
		/// <exception cref="IllegalArgumentException"> If the component value for this
		/// <CODE>ColorModel</CODE> is signed </exception>
		public override int GetBlue(int pixel)
		{
			return GetRGBComponent(pixel, 2);
		}

		/// <summary>
		/// Returns the alpha component for the specified pixel, scaled
		/// from 0 to 255.   The pixel value is specified as an int.
		/// </summary>
		/// <param name="pixel"> The pixel from which you want to get the alpha component.
		/// </param>
		/// <returns> The alpha component for the specified pixel, as an int.
		/// </returns>
		/// <exception cref="IllegalArgumentException"> If there is more than
		/// one component in this <CODE>ColorModel</CODE>. </exception>
		/// <exception cref="IllegalArgumentException"> If the component value for this
		/// <CODE>ColorModel</CODE> is signed </exception>
		public override int GetAlpha(int pixel)
		{
			if (SupportsAlpha == false)
			{
				return 255;
			}
			if (NumComponents_Renamed > 1)
			{
				throw new IllegalArgumentException("More than one component per pixel");
			}
			if (Signed)
			{
				throw new IllegalArgumentException("Component value is signed");
			}

			return (int)((((float) pixel) / ((1 << NBits[0]) - 1)) * 255.0f + 0.5f);
		}

		/// <summary>
		/// Returns the color/alpha components of the pixel in the default
		/// RGB color model format.  A color conversion is done if necessary.
		/// The returned value will be in a non pre-multiplied format. If
		/// the alpha is premultiplied, this method divides it out of the
		/// color components (if the alpha value is 0, the color values will be 0).
		/// </summary>
		/// <param name="pixel"> The pixel from which you want to get the color/alpha components.
		/// </param>
		/// <returns> The color/alpha components for the specified pixel, as an int.
		/// </returns>
		/// <exception cref="IllegalArgumentException"> If there is more than
		/// one component in this <CODE>ColorModel</CODE>. </exception>
		/// <exception cref="IllegalArgumentException"> If the component value for this
		/// <CODE>ColorModel</CODE> is signed </exception>
		public override int GetRGB(int pixel)
		{
			if (NumComponents_Renamed > 1)
			{
				throw new IllegalArgumentException("More than one component per pixel");
			}
			if (Signed)
			{
				throw new IllegalArgumentException("Component value is signed");
			}

			return (GetAlpha(pixel) << 24) | (GetRed(pixel) << 16) | (GetGreen(pixel) << 8) | (GetBlue(pixel) << 0);
		}

		private int ExtractComponent(Object inData, int idx, int precision)
		{
			// Extract component idx from inData.  The precision argument
			// should be either 8 or 16.  If it's 8, this method will return
			// an 8-bit value.  If it's 16, this method will return a 16-bit
			// value for transferTypes other than TYPE_BYTE.  For TYPE_BYTE,
			// an 8-bit value will be returned.

			// This method maps the input value corresponding to a
			// normalized ColorSpace component value of 0.0 to 0, and the
			// input value corresponding to a normalized ColorSpace
			// component value of 1.0 to 2^n - 1 (where n is 8 or 16), so
			// it is appropriate only for ColorSpaces with min/max component
			// values of 0.0/1.0.  This will be true for sRGB, the built-in
			// Linear RGB and Linear Gray spaces, and any other ICC grayscale
			// spaces for which we have precomputed LUTs.

			bool needAlpha = (SupportsAlpha && IsAlphaPremultiplied);
			int alp = 0;
			int comp;
			int mask = (1 << NBits[idx]) - 1;

			switch (TransferType_Renamed)
			{
				// Note: we do no clamping of the pixel data here - we
				// assume that the data is scaled properly
				case DataBuffer.TYPE_SHORT:
				{
					short[] sdata = (short[]) inData;
					float scalefactor = (float)((1 << precision) - 1);
					if (needAlpha)
					{
						short s = sdata[NumColorComponents_Renamed];
						if (s != (short) 0)
						{
							return (int)((((float) sdata[idx]) / ((float) s)) * scalefactor + 0.5f);
						}
						else
						{
							return 0;
						}
					}
					else
					{
						return (int)((sdata[idx] / 32767.0f) * scalefactor + 0.5f);
					}
				}
					goto case DataBuffer.TYPE_FLOAT;
				case DataBuffer.TYPE_FLOAT:
				{
					float[] fdata = (float[]) inData;
					float scalefactor = (float)((1 << precision) - 1);
					if (needAlpha)
					{
						float f = fdata[NumColorComponents_Renamed];
						if (f != 0.0f)
						{
							return (int)(((fdata[idx] / f) * scalefactor) + 0.5f);
						}
						else
						{
							return 0;
						}
					}
					else
					{
						return (int)(fdata[idx] * scalefactor + 0.5f);
					}
				}
					goto case DataBuffer.TYPE_DOUBLE;
				case DataBuffer.TYPE_DOUBLE:
				{
					double[] ddata = (double[]) inData;
					double scalefactor = (double)((1 << precision) - 1);
					if (needAlpha)
					{
						double d = ddata[NumColorComponents_Renamed];
						if (d != 0.0)
						{
							return (int)(((ddata[idx] / d) * scalefactor) + 0.5);
						}
						else
						{
							return 0;
						}
					}
					else
					{
						return (int)(ddata[idx] * scalefactor + 0.5);
					}
				}
					goto case DataBuffer.TYPE_BYTE;
				case DataBuffer.TYPE_BYTE:
				   sbyte[] bdata = (sbyte[])inData;
				   comp = bdata[idx] & mask;
				   precision = 8;
				   if (needAlpha)
				   {
					   alp = bdata[NumColorComponents_Renamed] & mask;
				   }
				break;
				case DataBuffer.TYPE_USHORT:
				   short[] usdata = (short[])inData;
				   comp = usdata[idx] & mask;
				   if (needAlpha)
				   {
					   alp = usdata[NumColorComponents_Renamed] & mask;
				   }
				break;
				case DataBuffer.TYPE_INT:
				   int[] idata = (int[])inData;
				   comp = idata[idx];
				   if (needAlpha)
				   {
					   alp = idata[NumColorComponents_Renamed];
				   }
				break;
				default:
				   throw new UnsupportedOperationException("This method has not " + "been implemented for transferType " + TransferType_Renamed);
			}
			if (needAlpha)
			{
				if (alp != 0)
				{
					float scalefactor = (float)((1 << precision) - 1);
					float fcomp = ((float) comp) / ((float)mask);
					float invalp = ((float)((1 << NBits[NumColorComponents_Renamed]) - 1)) / ((float) alp);
					return (int)(fcomp * invalp * scalefactor + 0.5f);
				}
				else
				{
					return 0;
				}
			}
			else
			{
				if (NBits[idx] != precision)
				{
					float scalefactor = (float)((1 << precision) - 1);
					float fcomp = ((float) comp) / ((float)mask);
					return (int)(fcomp * scalefactor + 0.5f);
				}
				return comp;
			}
		}

		private int GetRGBComponent(Object inData, int idx)
		{
			if (NeedScaleInit)
			{
				InitScale();
			}
			if (Is_sRGB_stdScale)
			{
				return ExtractComponent(inData, idx, 8);
			}
			else if (Is_LinearRGB_stdScale)
			{
				int lutidx = ExtractComponent(inData, idx, 16);
				return TosRGB8LUT[lutidx] & 0xff;
			}
			else if (Is_ICCGray_stdScale)
			{
				int lutidx = ExtractComponent(inData, 0, 16);
				return TosRGB8LUT[lutidx] & 0xff;
			}

			// Not CS_sRGB, CS_LINEAR_RGB, or any TYPE_GRAY ICC_ColorSpace
			float[] norm = GetNormalizedComponents(inData, null, 0);
			// Note that getNormalizedComponents returns non-premultiplied values
			float[] rgb = ColorSpace_Renamed.ToRGB(norm);
			return (int)(rgb[idx] * 255.0f + 0.5f);
		}

		/// <summary>
		/// Returns the red color component for the specified pixel, scaled
		/// from 0 to 255 in the default RGB ColorSpace, sRGB.  A color conversion
		/// is done if necessary.  The <CODE>pixel</CODE> value is specified by an array
		/// of data elements of type <CODE>transferType</CODE> passed in as an object
		/// reference. The returned value will be a non pre-multiplied value. If the
		/// alpha is premultiplied, this method divides it out before returning
		/// the value (if the alpha value is 0, the red value will be 0). Since
		/// <code>ComponentColorModel</code> can be subclassed, subclasses
		/// inherit the implementation of this method and if they don't override
		/// it then they throw an exception if they use an unsupported
		/// <code>transferType</code>.
		/// </summary>
		/// <param name="inData"> The pixel from which you want to get the red color component,
		/// specified by an array of data elements of type <CODE>transferType</CODE>.
		/// </param>
		/// <returns> The red color component for the specified pixel, as an int.
		/// </returns>
		/// <exception cref="ClassCastException"> If <CODE>inData</CODE> is not a primitive array
		/// of type <CODE>transferType</CODE>. </exception>
		/// <exception cref="ArrayIndexOutOfBoundsException"> if <CODE>inData</CODE> is not
		/// large enough to hold a pixel value for this
		/// <CODE>ColorModel</CODE>. </exception>
		/// <exception cref="UnsupportedOperationException"> If the transfer type of
		/// this <CODE>ComponentColorModel</CODE>
		/// is not one of the supported transfer types:
		/// <CODE>DataBuffer.TYPE_BYTE</CODE>, <CODE>DataBuffer.TYPE_USHORT</CODE>,
		/// <CODE>DataBuffer.TYPE_INT</CODE>, <CODE>DataBuffer.TYPE_SHORT</CODE>,
		/// <CODE>DataBuffer.TYPE_FLOAT</CODE>, or <CODE>DataBuffer.TYPE_DOUBLE</CODE>. </exception>
		public override int GetRed(Object inData)
		{
			return GetRGBComponent(inData, 0);
		}


		/// <summary>
		/// Returns the green color component for the specified pixel, scaled
		/// from 0 to 255 in the default RGB <CODE>ColorSpace</CODE>, sRGB.
		/// A color conversion is done if necessary.  The <CODE>pixel</CODE> value
		/// is specified by an array of data elements of type <CODE>transferType</CODE>
		/// passed in as an object reference. The returned value is a non pre-multiplied
		/// value. If the alpha is premultiplied, this method divides it out before
		/// returning the value (if the alpha value is 0, the green value will be 0).
		/// Since <code>ComponentColorModel</code> can be subclassed,
		/// subclasses inherit the implementation of this method and if they
		/// don't override it then they throw an exception if they use an
		/// unsupported <code>transferType</code>.
		/// </summary>
		/// <param name="inData"> The pixel from which you want to get the green color component,
		/// specified by an array of data elements of type <CODE>transferType</CODE>.
		/// </param>
		/// <returns> The green color component for the specified pixel, as an int.
		/// </returns>
		/// <exception cref="ClassCastException"> If <CODE>inData</CODE> is not a primitive array
		/// of type <CODE>transferType</CODE>. </exception>
		/// <exception cref="ArrayIndexOutOfBoundsException"> if <CODE>inData</CODE> is not
		/// large enough to hold a pixel value for this
		/// <CODE>ColorModel</CODE>. </exception>
		/// <exception cref="UnsupportedOperationException"> If the transfer type of
		/// this <CODE>ComponentColorModel</CODE>
		/// is not one of the supported transfer types:
		/// <CODE>DataBuffer.TYPE_BYTE</CODE>, <CODE>DataBuffer.TYPE_USHORT</CODE>,
		/// <CODE>DataBuffer.TYPE_INT</CODE>, <CODE>DataBuffer.TYPE_SHORT</CODE>,
		/// <CODE>DataBuffer.TYPE_FLOAT</CODE>, or <CODE>DataBuffer.TYPE_DOUBLE</CODE>. </exception>
		public override int GetGreen(Object inData)
		{
			return GetRGBComponent(inData, 1);
		}


		/// <summary>
		/// Returns the blue color component for the specified pixel, scaled
		/// from 0 to 255 in the default RGB <CODE>ColorSpace</CODE>, sRGB.
		/// A color conversion is done if necessary.  The <CODE>pixel</CODE> value is
		/// specified by an array of data elements of type <CODE>transferType</CODE>
		/// passed in as an object reference. The returned value is a non pre-multiplied
		/// value. If the alpha is premultiplied, this method divides it out before
		/// returning the value (if the alpha value is 0, the blue value will be 0).
		/// Since <code>ComponentColorModel</code> can be subclassed,
		/// subclasses inherit the implementation of this method and if they
		/// don't override it then they throw an exception if they use an
		/// unsupported <code>transferType</code>.
		/// </summary>
		/// <param name="inData"> The pixel from which you want to get the blue color component,
		/// specified by an array of data elements of type <CODE>transferType</CODE>.
		/// </param>
		/// <returns> The blue color component for the specified pixel, as an int.
		/// </returns>
		/// <exception cref="ClassCastException"> If <CODE>inData</CODE> is not a primitive array
		/// of type <CODE>transferType</CODE>. </exception>
		/// <exception cref="ArrayIndexOutOfBoundsException"> if <CODE>inData</CODE> is not
		/// large enough to hold a pixel value for this
		/// <CODE>ColorModel</CODE>. </exception>
		/// <exception cref="UnsupportedOperationException"> If the transfer type of
		/// this <CODE>ComponentColorModel</CODE>
		/// is not one of the supported transfer types:
		/// <CODE>DataBuffer.TYPE_BYTE</CODE>, <CODE>DataBuffer.TYPE_USHORT</CODE>,
		/// <CODE>DataBuffer.TYPE_INT</CODE>, <CODE>DataBuffer.TYPE_SHORT</CODE>,
		/// <CODE>DataBuffer.TYPE_FLOAT</CODE>, or <CODE>DataBuffer.TYPE_DOUBLE</CODE>. </exception>
		public override int GetBlue(Object inData)
		{
			return GetRGBComponent(inData, 2);
		}

		/// <summary>
		/// Returns the alpha component for the specified pixel, scaled from
		/// 0 to 255.  The pixel value is specified by an array of data
		/// elements of type <CODE>transferType</CODE> passed in as an
		/// object reference.  Since <code>ComponentColorModel</code> can be
		/// subclassed, subclasses inherit the
		/// implementation of this method and if they don't override it then
		/// they throw an exception if they use an unsupported
		/// <code>transferType</code>.
		/// </summary>
		/// <param name="inData"> The pixel from which you want to get the alpha component,
		/// specified by an array of data elements of type <CODE>transferType</CODE>.
		/// </param>
		/// <returns> The alpha component for the specified pixel, as an int.
		/// </returns>
		/// <exception cref="ClassCastException"> If <CODE>inData</CODE> is not a primitive array
		/// of type <CODE>transferType</CODE>. </exception>
		/// <exception cref="ArrayIndexOutOfBoundsException"> if <CODE>inData</CODE> is not
		/// large enough to hold a pixel value for this
		/// <CODE>ColorModel</CODE>. </exception>
		/// <exception cref="UnsupportedOperationException"> If the transfer type of
		/// this <CODE>ComponentColorModel</CODE>
		/// is not one of the supported transfer types:
		/// <CODE>DataBuffer.TYPE_BYTE</CODE>, <CODE>DataBuffer.TYPE_USHORT</CODE>,
		/// <CODE>DataBuffer.TYPE_INT</CODE>, <CODE>DataBuffer.TYPE_SHORT</CODE>,
		/// <CODE>DataBuffer.TYPE_FLOAT</CODE>, or <CODE>DataBuffer.TYPE_DOUBLE</CODE>. </exception>
		public override int GetAlpha(Object inData)
		{
			if (SupportsAlpha == false)
			{
				return 255;
			}

			int alpha = 0;
			int aIdx = NumColorComponents_Renamed;
			int mask = (1 << NBits[aIdx]) - 1;

			switch (TransferType_Renamed)
			{
				case DataBuffer.TYPE_SHORT:
					short[] sdata = (short[])inData;
					alpha = (int)((sdata[aIdx] / 32767.0f) * 255.0f + 0.5f);
					return alpha;
				case DataBuffer.TYPE_FLOAT:
					float[] fdata = (float[])inData;
					alpha = (int)(fdata[aIdx] * 255.0f + 0.5f);
					return alpha;
				case DataBuffer.TYPE_DOUBLE:
					double[] ddata = (double[])inData;
					alpha = (int)(ddata[aIdx] * 255.0 + 0.5);
					return alpha;
				case DataBuffer.TYPE_BYTE:
				   sbyte[] bdata = (sbyte[])inData;
				   alpha = bdata[aIdx] & mask;
				break;
				case DataBuffer.TYPE_USHORT:
				   short[] usdata = (short[])inData;
				   alpha = usdata[aIdx] & mask;
				break;
				case DataBuffer.TYPE_INT:
				   int[] idata = (int[])inData;
				   alpha = idata[aIdx];
				break;
				default:
				   throw new UnsupportedOperationException("This method has not " + "been implemented for transferType " + TransferType_Renamed);
			}

			if (NBits[aIdx] == 8)
			{
				return alpha;
			}
			else
			{
				return (int)((((float) alpha) / ((float)((1 << NBits[aIdx]) - 1))) * 255.0f + 0.5f);
			}
		}

		/// <summary>
		/// Returns the color/alpha components for the specified pixel in the
		/// default RGB color model format.  A color conversion is done if
		/// necessary.  The pixel value is specified by an
		/// array of data elements of type <CODE>transferType</CODE> passed
		/// in as an object reference.
		/// The returned value is in a non pre-multiplied format. If
		/// the alpha is premultiplied, this method divides it out of the
		/// color components (if the alpha value is 0, the color values will be 0).
		/// Since <code>ComponentColorModel</code> can be subclassed,
		/// subclasses inherit the implementation of this method and if they
		/// don't override it then they throw an exception if they use an
		/// unsupported <code>transferType</code>.
		/// </summary>
		/// <param name="inData"> The pixel from which you want to get the color/alpha components,
		/// specified by an array of data elements of type <CODE>transferType</CODE>.
		/// </param>
		/// <returns> The color/alpha components for the specified pixel, as an int.
		/// </returns>
		/// <exception cref="ClassCastException"> If <CODE>inData</CODE> is not a primitive array
		/// of type <CODE>transferType</CODE>. </exception>
		/// <exception cref="ArrayIndexOutOfBoundsException"> if <CODE>inData</CODE> is not
		/// large enough to hold a pixel value for this
		/// <CODE>ColorModel</CODE>. </exception>
		/// <exception cref="UnsupportedOperationException"> If the transfer type of
		/// this <CODE>ComponentColorModel</CODE>
		/// is not one of the supported transfer types:
		/// <CODE>DataBuffer.TYPE_BYTE</CODE>, <CODE>DataBuffer.TYPE_USHORT</CODE>,
		/// <CODE>DataBuffer.TYPE_INT</CODE>, <CODE>DataBuffer.TYPE_SHORT</CODE>,
		/// <CODE>DataBuffer.TYPE_FLOAT</CODE>, or <CODE>DataBuffer.TYPE_DOUBLE</CODE>. </exception>
		/// <seealso cref= ColorModel#getRGBdefault </seealso>
		public override int GetRGB(Object inData)
		{
			if (NeedScaleInit)
			{
				InitScale();
			}
			if (Is_sRGB_stdScale || Is_LinearRGB_stdScale)
			{
				return (GetAlpha(inData) << 24) | (GetRed(inData) << 16) | (GetGreen(inData) << 8) | (GetBlue(inData));
			}
			else if (ColorSpaceType == ColorSpace.TYPE_GRAY)
			{
				int gray = GetRed(inData); // Red sRGB component should equal
										   // green and blue components
				return (GetAlpha(inData) << 24) | (gray << 16) | (gray << 8) | gray;
			}
			float[] norm = GetNormalizedComponents(inData, null, 0);
			// Note that getNormalizedComponents returns non-premult values
			float[] rgb = ColorSpace_Renamed.ToRGB(norm);
			return (GetAlpha(inData) << 24) | (((int)(rgb[0] * 255.0f + 0.5f)) << 16) | (((int)(rgb[1] * 255.0f + 0.5f)) << 8) | (((int)(rgb[2] * 255.0f + 0.5f)) << 0);
		}

		/// <summary>
		/// Returns a data element array representation of a pixel in this
		/// <CODE>ColorModel</CODE>, given an integer pixel representation
		/// in the default RGB color model.
		/// This array can then be passed to the <CODE>setDataElements</CODE>
		/// method of a <CODE>WritableRaster</CODE> object.  If the
		/// <CODE>pixel</CODE>
		/// parameter is null, a new array is allocated.  Since
		/// <code>ComponentColorModel</code> can be subclassed, subclasses
		/// inherit the implementation of this method and if they don't
		/// override it then
		/// they throw an exception if they use an unsupported
		/// <code>transferType</code>.
		/// </summary>
		/// <param name="rgb"> the integer representation of the pixel in the RGB
		///            color model </param>
		/// <param name="pixel"> the specified pixel </param>
		/// <returns> The data element array representation of a pixel
		/// in this <CODE>ColorModel</CODE>. </returns>
		/// <exception cref="ClassCastException"> If <CODE>pixel</CODE> is not null and
		/// is not a primitive array of type <CODE>transferType</CODE>. </exception>
		/// <exception cref="ArrayIndexOutOfBoundsException"> If <CODE>pixel</CODE> is
		/// not large enough to hold a pixel value for this
		/// <CODE>ColorModel</CODE>. </exception>
		/// <exception cref="UnsupportedOperationException"> If the transfer type of
		/// this <CODE>ComponentColorModel</CODE>
		/// is not one of the supported transfer types:
		/// <CODE>DataBuffer.TYPE_BYTE</CODE>, <CODE>DataBuffer.TYPE_USHORT</CODE>,
		/// <CODE>DataBuffer.TYPE_INT</CODE>, <CODE>DataBuffer.TYPE_SHORT</CODE>,
		/// <CODE>DataBuffer.TYPE_FLOAT</CODE>, or <CODE>DataBuffer.TYPE_DOUBLE</CODE>.
		/// </exception>
		/// <seealso cref= WritableRaster#setDataElements </seealso>
		/// <seealso cref= SampleModel#setDataElements </seealso>
		public override Object GetDataElements(int rgb, Object pixel)
		{
			// REMIND: Use rendering hints?

			int red, grn, blu, alp;
			red = (rgb >> 16) & 0xff;
			grn = (rgb >> 8) & 0xff;
			blu = rgb & 0xff;

			if (NeedScaleInit)
			{
				InitScale();
			}
			if (Signed)
			{
				// Handle SHORT, FLOAT, & DOUBLE here

				switch (TransferType_Renamed)
				{
				case DataBuffer.TYPE_SHORT:
				{
						short[] sdata;
						if (pixel == null)
						{
							sdata = new short[NumComponents_Renamed];
						}
						else
						{
							sdata = (short[])pixel;
						}
						float factor;
						if (Is_sRGB_stdScale || Is_LinearRGB_stdScale)
						{
							factor = 32767.0f / 255.0f;
							if (Is_LinearRGB_stdScale)
							{
								red = FromsRGB8LUT16[red] & 0xffff;
								grn = FromsRGB8LUT16[grn] & 0xffff;
								blu = FromsRGB8LUT16[blu] & 0xffff;
								factor = 32767.0f / 65535.0f;
							}
							if (SupportsAlpha)
							{
								alp = (rgb >> 24) & 0xff;
								sdata[3] = (short)(alp * (32767.0f / 255.0f) + 0.5f);
								if (IsAlphaPremultiplied)
								{
									factor = alp * factor * (1.0f / 255.0f);
								}
							}
							sdata[0] = (short)(red * factor + 0.5f);
							sdata[1] = (short)(grn * factor + 0.5f);
							sdata[2] = (short)(blu * factor + 0.5f);
						}
						else if (Is_LinearGray_stdScale)
						{
							red = FromsRGB8LUT16[red] & 0xffff;
							grn = FromsRGB8LUT16[grn] & 0xffff;
							blu = FromsRGB8LUT16[blu] & 0xffff;
							float gray = ((0.2125f * red) + (0.7154f * grn) + (0.0721f * blu)) / 65535.0f;
							factor = 32767.0f;
							if (SupportsAlpha)
							{
								alp = (rgb >> 24) & 0xff;
								sdata[1] = (short)(alp * (32767.0f / 255.0f) + 0.5f);
								if (IsAlphaPremultiplied)
								{
									factor = alp * factor * (1.0f / 255.0f);
								}
							}
							sdata[0] = (short)(gray * factor + 0.5f);
						}
						else if (Is_ICCGray_stdScale)
						{
							red = FromsRGB8LUT16[red] & 0xffff;
							grn = FromsRGB8LUT16[grn] & 0xffff;
							blu = FromsRGB8LUT16[blu] & 0xffff;
							int gray = (int)((0.2125f * red) + (0.7154f * grn) + (0.0721f * blu) + 0.5f);
							gray = FromLinearGray16ToOtherGray16LUT[gray] & 0xffff;
							factor = 32767.0f / 65535.0f;
							if (SupportsAlpha)
							{
								alp = (rgb >> 24) & 0xff;
								sdata[1] = (short)(alp * (32767.0f / 255.0f) + 0.5f);
								if (IsAlphaPremultiplied)
								{
									factor = alp * factor * (1.0f / 255.0f);
								}
							}
							sdata[0] = (short)(gray * factor + 0.5f);
						}
						else
						{
							factor = 1.0f / 255.0f;
							float[] norm = new float[3];
							norm[0] = red * factor;
							norm[1] = grn * factor;
							norm[2] = blu * factor;
							norm = ColorSpace_Renamed.FromRGB(norm);
							if (NonStdScale)
							{
								for (int i = 0; i < NumColorComponents_Renamed; i++)
								{
									norm[i] = (norm[i] - CompOffset[i]) * CompScale[i];
									// REMIND: need to analyze whether this
									// clamping is necessary
									if (norm[i] < 0.0f)
									{
										norm[i] = 0.0f;
									}
									if (norm[i] > 1.0f)
									{
										norm[i] = 1.0f;
									}
								}
							}
							factor = 32767.0f;
							if (SupportsAlpha)
							{
								alp = (rgb >> 24) & 0xff;
								sdata[NumColorComponents_Renamed] = (short)(alp * (32767.0f / 255.0f) + 0.5f);
								if (IsAlphaPremultiplied)
								{
									factor *= alp * (1.0f / 255.0f);
								}
							}
							for (int i = 0; i < NumColorComponents_Renamed; i++)
							{
								sdata[i] = (short)(norm[i] * factor + 0.5f);
							}
						}
						return sdata;
				}
				case DataBuffer.TYPE_FLOAT:
				{
						float[] fdata;
						if (pixel == null)
						{
							fdata = new float[NumComponents_Renamed];
						}
						else
						{
							fdata = (float[])pixel;
						}
						float factor;
						if (Is_sRGB_stdScale || Is_LinearRGB_stdScale)
						{
							if (Is_LinearRGB_stdScale)
							{
								red = FromsRGB8LUT16[red] & 0xffff;
								grn = FromsRGB8LUT16[grn] & 0xffff;
								blu = FromsRGB8LUT16[blu] & 0xffff;
								factor = 1.0f / 65535.0f;
							}
							else
							{
								factor = 1.0f / 255.0f;
							}
							if (SupportsAlpha)
							{
								alp = (rgb >> 24) & 0xff;
								fdata[3] = alp * (1.0f / 255.0f);
								if (IsAlphaPremultiplied)
								{
									factor *= fdata[3];
								}
							}
							fdata[0] = red * factor;
							fdata[1] = grn * factor;
							fdata[2] = blu * factor;
						}
						else if (Is_LinearGray_stdScale)
						{
							red = FromsRGB8LUT16[red] & 0xffff;
							grn = FromsRGB8LUT16[grn] & 0xffff;
							blu = FromsRGB8LUT16[blu] & 0xffff;
							fdata[0] = ((0.2125f * red) + (0.7154f * grn) + (0.0721f * blu)) / 65535.0f;
							if (SupportsAlpha)
							{
								alp = (rgb >> 24) & 0xff;
								fdata[1] = alp * (1.0f / 255.0f);
								if (IsAlphaPremultiplied)
								{
									fdata[0] *= fdata[1];
								}
							}
						}
						else if (Is_ICCGray_stdScale)
						{
							red = FromsRGB8LUT16[red] & 0xffff;
							grn = FromsRGB8LUT16[grn] & 0xffff;
							blu = FromsRGB8LUT16[blu] & 0xffff;
							int gray = (int)((0.2125f * red) + (0.7154f * grn) + (0.0721f * blu) + 0.5f);
							fdata[0] = (FromLinearGray16ToOtherGray16LUT[gray] & 0xffff) / 65535.0f;
							if (SupportsAlpha)
							{
								alp = (rgb >> 24) & 0xff;
								fdata[1] = alp * (1.0f / 255.0f);
								if (IsAlphaPremultiplied)
								{
									fdata[0] *= fdata[1];
								}
							}
						}
						else
						{
							float[] norm = new float[3];
							factor = 1.0f / 255.0f;
							norm[0] = red * factor;
							norm[1] = grn * factor;
							norm[2] = blu * factor;
							norm = ColorSpace_Renamed.FromRGB(norm);
							if (SupportsAlpha)
							{
								alp = (rgb >> 24) & 0xff;
								fdata[NumColorComponents_Renamed] = alp * factor;
								if (IsAlphaPremultiplied)
								{
									factor *= alp;
									for (int i = 0; i < NumColorComponents_Renamed; i++)
									{
										norm[i] *= factor;
									}
								}
							}
							for (int i = 0; i < NumColorComponents_Renamed; i++)
							{
								fdata[i] = norm[i];
							}
						}
						return fdata;
				}
				case DataBuffer.TYPE_DOUBLE:
				{
						double[] ddata;
						if (pixel == null)
						{
							ddata = new double[NumComponents_Renamed];
						}
						else
						{
							ddata = (double[])pixel;
						}
						if (Is_sRGB_stdScale || Is_LinearRGB_stdScale)
						{
							double factor;
							if (Is_LinearRGB_stdScale)
							{
								red = FromsRGB8LUT16[red] & 0xffff;
								grn = FromsRGB8LUT16[grn] & 0xffff;
								blu = FromsRGB8LUT16[blu] & 0xffff;
								factor = 1.0 / 65535.0;
							}
							else
							{
								factor = 1.0 / 255.0;
							}
							if (SupportsAlpha)
							{
								alp = (rgb >> 24) & 0xff;
								ddata[3] = alp * (1.0 / 255.0);
								if (IsAlphaPremultiplied)
								{
									factor *= ddata[3];
								}
							}
							ddata[0] = red * factor;
							ddata[1] = grn * factor;
							ddata[2] = blu * factor;
						}
						else if (Is_LinearGray_stdScale)
						{
							red = FromsRGB8LUT16[red] & 0xffff;
							grn = FromsRGB8LUT16[grn] & 0xffff;
							blu = FromsRGB8LUT16[blu] & 0xffff;
							ddata[0] = ((0.2125 * red) + (0.7154 * grn) + (0.0721 * blu)) / 65535.0;
							if (SupportsAlpha)
							{
								alp = (rgb >> 24) & 0xff;
								ddata[1] = alp * (1.0 / 255.0);
								if (IsAlphaPremultiplied)
								{
									ddata[0] *= ddata[1];
								}
							}
						}
						else if (Is_ICCGray_stdScale)
						{
							red = FromsRGB8LUT16[red] & 0xffff;
							grn = FromsRGB8LUT16[grn] & 0xffff;
							blu = FromsRGB8LUT16[blu] & 0xffff;
							int gray = (int)((0.2125f * red) + (0.7154f * grn) + (0.0721f * blu) + 0.5f);
							ddata[0] = (FromLinearGray16ToOtherGray16LUT[gray] & 0xffff) / 65535.0;
							if (SupportsAlpha)
							{
								alp = (rgb >> 24) & 0xff;
								ddata[1] = alp * (1.0 / 255.0);
								if (IsAlphaPremultiplied)
								{
									ddata[0] *= ddata[1];
								}
							}
						}
						else
						{
							float factor = 1.0f / 255.0f;
							float[] norm = new float[3];
							norm[0] = red * factor;
							norm[1] = grn * factor;
							norm[2] = blu * factor;
							norm = ColorSpace_Renamed.FromRGB(norm);
							if (SupportsAlpha)
							{
								alp = (rgb >> 24) & 0xff;
								ddata[NumColorComponents_Renamed] = alp * (1.0 / 255.0);
								if (IsAlphaPremultiplied)
								{
									factor *= alp;
									for (int i = 0; i < NumColorComponents_Renamed; i++)
									{
										norm[i] *= factor;
									}
								}
							}
							for (int i = 0; i < NumColorComponents_Renamed; i++)
							{
								ddata[i] = norm[i];
							}
						}
						return ddata;
				}
				}
			}

			// Handle BYTE, USHORT, & INT here
			//REMIND: maybe more efficient not to use int array for
			//DataBuffer.TYPE_USHORT and DataBuffer.TYPE_INT
			int[] intpixel;
			if (TransferType_Renamed == DataBuffer.TYPE_INT && pixel != null)
			{
			   intpixel = (int[])pixel;
			}
			else
			{
				intpixel = new int[NumComponents_Renamed];
			}

			if (Is_sRGB_stdScale || Is_LinearRGB_stdScale)
			{
				int precision;
				float factor;
				if (Is_LinearRGB_stdScale)
				{
					if (TransferType_Renamed == DataBuffer.TYPE_BYTE)
					{
						red = FromsRGB8LUT8[red] & 0xff;
						grn = FromsRGB8LUT8[grn] & 0xff;
						blu = FromsRGB8LUT8[blu] & 0xff;
						precision = 8;
						factor = 1.0f / 255.0f;
					}
					else
					{
						red = FromsRGB8LUT16[red] & 0xffff;
						grn = FromsRGB8LUT16[grn] & 0xffff;
						blu = FromsRGB8LUT16[blu] & 0xffff;
						precision = 16;
						factor = 1.0f / 65535.0f;
					}
				}
				else
				{
					precision = 8;
					factor = 1.0f / 255.0f;
				}
				if (SupportsAlpha)
				{
					alp = (rgb >> 24) & 0xff;
					if (NBits[3] == 8)
					{
						intpixel[3] = alp;
					}
					else
					{
						intpixel[3] = (int)(alp * (1.0f / 255.0f) * ((1 << NBits[3]) - 1) + 0.5f);
					}
					if (IsAlphaPremultiplied)
					{
						factor *= (alp * (1.0f / 255.0f));
						precision = -1; // force component calculations below
					}
				}
				if (NBits[0] == precision)
				{
					intpixel[0] = red;
				}
				else
				{
					intpixel[0] = (int)(red * factor * ((1 << NBits[0]) - 1) + 0.5f);
				}
				if (NBits[1] == precision)
				{
					intpixel[1] = (int)(grn);
				}
				else
				{
					intpixel[1] = (int)(grn * factor * ((1 << NBits[1]) - 1) + 0.5f);
				}
				if (NBits[2] == precision)
				{
					intpixel[2] = (int)(blu);
				}
				else
				{
					intpixel[2] = (int)(blu * factor * ((1 << NBits[2]) - 1) + 0.5f);
				}
			}
			else if (Is_LinearGray_stdScale)
			{
				red = FromsRGB8LUT16[red] & 0xffff;
				grn = FromsRGB8LUT16[grn] & 0xffff;
				blu = FromsRGB8LUT16[blu] & 0xffff;
				float gray = ((0.2125f * red) + (0.7154f * grn) + (0.0721f * blu)) / 65535.0f;
				if (SupportsAlpha)
				{
					alp = (rgb >> 24) & 0xff;
					if (NBits[1] == 8)
					{
						intpixel[1] = alp;
					}
					else
					{
						intpixel[1] = (int)(alp * (1.0f / 255.0f) * ((1 << NBits[1]) - 1) + 0.5f);
					}
					if (IsAlphaPremultiplied)
					{
						gray *= (alp * (1.0f / 255.0f));
					}
				}
				intpixel[0] = (int)(gray * ((1 << NBits[0]) - 1) + 0.5f);
			}
			else if (Is_ICCGray_stdScale)
			{
				red = FromsRGB8LUT16[red] & 0xffff;
				grn = FromsRGB8LUT16[grn] & 0xffff;
				blu = FromsRGB8LUT16[blu] & 0xffff;
				int gray16 = (int)((0.2125f * red) + (0.7154f * grn) + (0.0721f * blu) + 0.5f);
				float gray = (FromLinearGray16ToOtherGray16LUT[gray16] & 0xffff) / 65535.0f;
				if (SupportsAlpha)
				{
					alp = (rgb >> 24) & 0xff;
					if (NBits[1] == 8)
					{
						intpixel[1] = alp;
					}
					else
					{
						intpixel[1] = (int)(alp * (1.0f / 255.0f) * ((1 << NBits[1]) - 1) + 0.5f);
					}
					if (IsAlphaPremultiplied)
					{
						gray *= (alp * (1.0f / 255.0f));
					}
				}
				intpixel[0] = (int)(gray * ((1 << NBits[0]) - 1) + 0.5f);
			}
			else
			{
				// Need to convert the color
				float[] norm = new float[3];
				float factor = 1.0f / 255.0f;
				norm[0] = red * factor;
				norm[1] = grn * factor;
				norm[2] = blu * factor;
				norm = ColorSpace_Renamed.FromRGB(norm);
				if (NonStdScale)
				{
					for (int i = 0; i < NumColorComponents_Renamed; i++)
					{
						norm[i] = (norm[i] - CompOffset[i]) * CompScale[i];
						// REMIND: need to analyze whether this
						// clamping is necessary
						if (norm[i] < 0.0f)
						{
							norm[i] = 0.0f;
						}
						if (norm[i] > 1.0f)
						{
							norm[i] = 1.0f;
						}
					}
				}
				if (SupportsAlpha)
				{
					alp = (rgb >> 24) & 0xff;
					if (NBits[NumColorComponents_Renamed] == 8)
					{
						intpixel[NumColorComponents_Renamed] = alp;
					}
					else
					{
						intpixel[NumColorComponents_Renamed] = (int)(alp * factor * ((1 << NBits[NumColorComponents_Renamed]) - 1) + 0.5f);
					}
					if (IsAlphaPremultiplied)
					{
						factor *= alp;
						for (int i = 0; i < NumColorComponents_Renamed; i++)
						{
							norm[i] *= factor;
						}
					}
				}
				for (int i = 0; i < NumColorComponents_Renamed; i++)
				{
					intpixel[i] = (int)(norm[i] * ((1 << NBits[i]) - 1) + 0.5f);
				}
			}

			switch (TransferType_Renamed)
			{
				case DataBuffer.TYPE_BYTE:
				{
				   sbyte[] bdata;
				   if (pixel == null)
				   {
					   bdata = new sbyte[NumComponents_Renamed];
				   }
				   else
				   {
					   bdata = (sbyte[])pixel;
				   }
				   for (int i = 0; i < NumComponents_Renamed; i++)
				   {
					   bdata[i] = unchecked((sbyte)(0xff & intpixel[i]));
				   }
				   return bdata;
				}
				case DataBuffer.TYPE_USHORT:
				{
				   short[] sdata;
				   if (pixel == null)
				   {
					   sdata = new short[NumComponents_Renamed];
				   }
				   else
				   {
					   sdata = (short[])pixel;
				   }
				   for (int i = 0; i < NumComponents_Renamed; i++)
				   {
					   sdata[i] = unchecked((short)(intpixel[i] & 0xffff));
				   }
				   return sdata;
				}
				case DataBuffer.TYPE_INT:
					if (MaxBits > 23)
					{
						// fix 4412670 - for components of 24 or more bits
						// some calculations done above with float precision
						// may lose enough precision that the integer result
						// overflows nBits, so we need to clamp.
						for (int i = 0; i < NumComponents_Renamed; i++)
						{
							if (intpixel[i] > ((1 << NBits[i]) - 1))
							{
								intpixel[i] = (1 << NBits[i]) - 1;
							}
						}
					}
					return intpixel;
			}
			throw new IllegalArgumentException("This method has not been " + "implemented for transferType " + TransferType_Renamed);
		}

	   /// <summary>
	   /// Returns an array of unnormalized color/alpha components given a pixel
	   /// in this <CODE>ColorModel</CODE>.
	   /// An IllegalArgumentException is thrown if the component value for this
	   /// <CODE>ColorModel</CODE> is not conveniently representable in the
	   /// unnormalized form.  Color/alpha components are stored
	   /// in the <CODE>components</CODE> array starting at <CODE>offset</CODE>
	   /// (even if the array is allocated by this method).
	   /// </summary>
	   /// <param name="pixel"> The pixel value specified as an integer. </param>
	   /// <param name="components"> An integer array in which to store the unnormalized
	   /// color/alpha components. If the <CODE>components</CODE> array is null,
	   /// a new array is allocated. </param>
	   /// <param name="offset"> An offset into the <CODE>components</CODE> array.
	   /// </param>
	   /// <returns> The components array.
	   /// </returns>
	   /// <exception cref="IllegalArgumentException"> If there is more than one
	   /// component in this <CODE>ColorModel</CODE>. </exception>
	   /// <exception cref="IllegalArgumentException"> If this
	   /// <CODE>ColorModel</CODE> does not support the unnormalized form </exception>
	   /// <exception cref="ArrayIndexOutOfBoundsException"> If the <CODE>components</CODE>
	   /// array is not null and is not large enough to hold all the color and
	   /// alpha components (starting at offset). </exception>
		public override int[] GetComponents(int pixel, int[] components, int offset)
		{
			if (NumComponents_Renamed > 1)
			{
				throw new IllegalArgumentException("More than one component per pixel");
			}
			if (NeedScaleInit)
			{
				InitScale();
			}
			if (NoUnnorm)
			{
				throw new IllegalArgumentException("This ColorModel does not support the unnormalized form");
			}
			if (components == null)
			{
				components = new int[offset + 1];
			}

			components[offset + 0] = (pixel & ((1 << NBits[0]) - 1));
			return components;
		}

		/// <summary>
		/// Returns an array of unnormalized color/alpha components given a pixel
		/// in this <CODE>ColorModel</CODE>.  The pixel value is specified by an
		/// array of data elements of type <CODE>transferType</CODE> passed in as
		/// an object reference.
		/// An IllegalArgumentException is thrown if the component values for this
		/// <CODE>ColorModel</CODE> are not conveniently representable in the
		/// unnormalized form.
		/// Color/alpha components are stored in the <CODE>components</CODE> array
		/// starting at  <CODE>offset</CODE> (even if the array is allocated by
		/// this method).  Since <code>ComponentColorModel</code> can be
		/// subclassed, subclasses inherit the
		/// implementation of this method and if they don't override it then
		/// this method might throw an exception if they use an unsupported
		/// <code>transferType</code>.
		/// </summary>
		/// <param name="pixel"> A pixel value specified by an array of data elements of
		/// type <CODE>transferType</CODE>. </param>
		/// <param name="components"> An integer array in which to store the unnormalized
		/// color/alpha components. If the <CODE>components</CODE> array is null,
		/// a new array is allocated. </param>
		/// <param name="offset"> An offset into the <CODE>components</CODE> array.
		/// </param>
		/// <returns> The <CODE>components</CODE> array.
		/// </returns>
		/// <exception cref="IllegalArgumentException"> If this
		/// <CODE>ComponentColorModel</CODE> does not support the unnormalized form </exception>
		/// <exception cref="UnsupportedOperationException"> in some cases iff the
		/// transfer type of this <CODE>ComponentColorModel</CODE>
		/// is not one of the following transfer types:
		/// <CODE>DataBuffer.TYPE_BYTE</CODE>, <CODE>DataBuffer.TYPE_USHORT</CODE>,
		/// or <CODE>DataBuffer.TYPE_INT</CODE>. </exception>
		/// <exception cref="ClassCastException"> If <CODE>pixel</CODE> is not a primitive
		/// array of type <CODE>transferType</CODE>. </exception>
		/// <exception cref="IllegalArgumentException"> If the <CODE>components</CODE> array is
		/// not null and is not large enough to hold all the color and alpha
		/// components (starting at offset), or if <CODE>pixel</CODE> is not large
		/// enough to hold a pixel value for this ColorModel. </exception>
		public override int[] GetComponents(Object pixel, int[] components, int offset)
		{
			int[] intpixel;
			if (NeedScaleInit)
			{
				InitScale();
			}
			if (NoUnnorm)
			{
				throw new IllegalArgumentException("This ColorModel does not support the unnormalized form");
			}
			if (pixel is int[])
			{
				intpixel = (int[])pixel;
			}
			else
			{
				intpixel = DataBuffer.ToIntArray(pixel);
				if (intpixel == null)
				{
				   throw new UnsupportedOperationException("This method has not been " + "implemented for transferType " + TransferType_Renamed);
				}
			}
			if (intpixel.Length < NumComponents_Renamed)
			{
				throw new IllegalArgumentException("Length of pixel array < number of components in model");
			}
			if (components == null)
			{
				components = new int[offset + NumComponents_Renamed];
			}
			else if ((components.Length - offset) < NumComponents_Renamed)
			{
				throw new IllegalArgumentException("Length of components array < number of components in model");
			}
			System.Array.Copy(intpixel, 0, components, offset, NumComponents_Renamed);

			return components;
		}

		/// <summary>
		/// Returns an array of all of the color/alpha components in unnormalized
		/// form, given a normalized component array.  Unnormalized components
		/// are unsigned integral values between 0 and 2<sup>n</sup> - 1, where
		/// n is the number of bits for a particular component.  Normalized
		/// components are float values between a per component minimum and
		/// maximum specified by the <code>ColorSpace</code> object for this
		/// <code>ColorModel</code>.  An <code>IllegalArgumentException</code>
		/// will be thrown if color component values for this
		/// <code>ColorModel</code> are not conveniently representable in the
		/// unnormalized form.  If the
		/// <code>components</code> array is <code>null</code>, a new array
		/// will be allocated.  The <code>components</code> array will
		/// be returned.  Color/alpha components are stored in the
		/// <code>components</code> array starting at <code>offset</code> (even
		/// if the array is allocated by this method). An
		/// <code>ArrayIndexOutOfBoundsException</code> is thrown if the
		/// <code>components</code> array is not <code>null</code> and is not
		/// large enough to hold all the color and alpha
		/// components (starting at <code>offset</code>).  An
		/// <code>IllegalArgumentException</code> is thrown if the
		/// <code>normComponents</code> array is not large enough to hold
		/// all the color and alpha components starting at
		/// <code>normOffset</code>. </summary>
		/// <param name="normComponents"> an array containing normalized components </param>
		/// <param name="normOffset"> the offset into the <code>normComponents</code>
		/// array at which to start retrieving normalized components </param>
		/// <param name="components"> an array that receives the components from
		/// <code>normComponents</code> </param>
		/// <param name="offset"> the index into <code>components</code> at which to
		/// begin storing normalized components from
		/// <code>normComponents</code> </param>
		/// <returns> an array containing unnormalized color and alpha
		/// components. </returns>
		/// <exception cref="IllegalArgumentException"> If this
		/// <CODE>ComponentColorModel</CODE> does not support the unnormalized form </exception>
		/// <exception cref="IllegalArgumentException"> if the length of
		///          <code>normComponents</code> minus <code>normOffset</code>
		///          is less than <code>numComponents</code> </exception>
		public override int[] GetUnnormalizedComponents(float[] normComponents, int normOffset, int[] components, int offset)
		{
			if (NeedScaleInit)
			{
				InitScale();
			}
			if (NoUnnorm)
			{
				throw new IllegalArgumentException("This ColorModel does not support the unnormalized form");
			}
			return base.GetUnnormalizedComponents(normComponents, normOffset, components, offset);
		}

		/// <summary>
		/// Returns an array of all of the color/alpha components in normalized
		/// form, given an unnormalized component array.  Unnormalized components
		/// are unsigned integral values between 0 and 2<sup>n</sup> - 1, where
		/// n is the number of bits for a particular component.  Normalized
		/// components are float values between a per component minimum and
		/// maximum specified by the <code>ColorSpace</code> object for this
		/// <code>ColorModel</code>.  An <code>IllegalArgumentException</code>
		/// will be thrown if color component values for this
		/// <code>ColorModel</code> are not conveniently representable in the
		/// unnormalized form.  If the
		/// <code>normComponents</code> array is <code>null</code>, a new array
		/// will be allocated.  The <code>normComponents</code> array
		/// will be returned.  Color/alpha components are stored in the
		/// <code>normComponents</code> array starting at
		/// <code>normOffset</code> (even if the array is allocated by this
		/// method).  An <code>ArrayIndexOutOfBoundsException</code> is thrown
		/// if the <code>normComponents</code> array is not <code>null</code>
		/// and is not large enough to hold all the color and alpha components
		/// (starting at <code>normOffset</code>).  An
		/// <code>IllegalArgumentException</code> is thrown if the
		/// <code>components</code> array is not large enough to hold all the
		/// color and alpha components starting at <code>offset</code>. </summary>
		/// <param name="components"> an array containing unnormalized components </param>
		/// <param name="offset"> the offset into the <code>components</code> array at
		/// which to start retrieving unnormalized components </param>
		/// <param name="normComponents"> an array that receives the normalized components </param>
		/// <param name="normOffset"> the index into <code>normComponents</code> at
		/// which to begin storing normalized components </param>
		/// <returns> an array containing normalized color and alpha
		/// components. </returns>
		/// <exception cref="IllegalArgumentException"> If this
		/// <CODE>ComponentColorModel</CODE> does not support the unnormalized form </exception>
		public override float[] GetNormalizedComponents(int[] components, int offset, float[] normComponents, int normOffset)
		{
			if (NeedScaleInit)
			{
				InitScale();
			}
			if (NoUnnorm)
			{
				throw new IllegalArgumentException("This ColorModel does not support the unnormalized form");
			}
			return base.GetNormalizedComponents(components, offset, normComponents, normOffset);
		}

		/// <summary>
		/// Returns a pixel value represented as an int in this <CODE>ColorModel</CODE>,
		/// given an array of unnormalized color/alpha components.
		/// </summary>
		/// <param name="components"> An array of unnormalized color/alpha components. </param>
		/// <param name="offset"> An offset into the <CODE>components</CODE> array.
		/// </param>
		/// <returns> A pixel value represented as an int.
		/// </returns>
		/// <exception cref="IllegalArgumentException"> If there is more than one component
		/// in this <CODE>ColorModel</CODE>. </exception>
		/// <exception cref="IllegalArgumentException"> If this
		/// <CODE>ComponentColorModel</CODE> does not support the unnormalized form </exception>
		public override int GetDataElement(int[] components, int offset)
		{
			if (NeedScaleInit)
			{
				InitScale();
			}
			if (NumComponents_Renamed == 1)
			{
				if (NoUnnorm)
				{
					throw new IllegalArgumentException("This ColorModel does not support the unnormalized form");
				}
				return components[offset + 0];
			}
			throw new IllegalArgumentException("This model returns " + NumComponents_Renamed + " elements in the pixel array.");
		}

		/// <summary>
		/// Returns a data element array representation of a pixel in this
		/// <CODE>ColorModel</CODE>, given an array of unnormalized color/alpha
		/// components. This array can then be passed to the <CODE>setDataElements</CODE>
		/// method of a <CODE>WritableRaster</CODE> object.
		/// </summary>
		/// <param name="components"> An array of unnormalized color/alpha components. </param>
		/// <param name="offset"> The integer offset into the <CODE>components</CODE> array. </param>
		/// <param name="obj"> The object in which to store the data element array
		/// representation of the pixel. If <CODE>obj</CODE> variable is null,
		/// a new array is allocated.  If <CODE>obj</CODE> is not null, it must
		/// be a primitive array of type <CODE>transferType</CODE>. An
		/// <CODE>ArrayIndexOutOfBoundsException</CODE> is thrown if
		/// <CODE>obj</CODE> is not large enough to hold a pixel value
		/// for this <CODE>ColorModel</CODE>.  Since
		/// <code>ComponentColorModel</code> can be subclassed, subclasses
		/// inherit the implementation of this method and if they don't
		/// override it then they throw an exception if they use an
		/// unsupported <code>transferType</code>.
		/// </param>
		/// <returns> The data element array representation of a pixel
		/// in this <CODE>ColorModel</CODE>.
		/// </returns>
		/// <exception cref="IllegalArgumentException"> If the components array
		/// is not large enough to hold all the color and alpha components
		/// (starting at offset). </exception>
		/// <exception cref="ClassCastException"> If <CODE>obj</CODE> is not null and is not a
		/// primitive  array of type <CODE>transferType</CODE>. </exception>
		/// <exception cref="ArrayIndexOutOfBoundsException"> If <CODE>obj</CODE> is not large
		/// enough to hold a pixel value for this <CODE>ColorModel</CODE>. </exception>
		/// <exception cref="IllegalArgumentException"> If this
		/// <CODE>ComponentColorModel</CODE> does not support the unnormalized form </exception>
		/// <exception cref="UnsupportedOperationException"> If the transfer type of
		/// this <CODE>ComponentColorModel</CODE>
		/// is not one of the following transfer types:
		/// <CODE>DataBuffer.TYPE_BYTE</CODE>, <CODE>DataBuffer.TYPE_USHORT</CODE>,
		/// or <CODE>DataBuffer.TYPE_INT</CODE>.
		/// </exception>
		/// <seealso cref= WritableRaster#setDataElements </seealso>
		/// <seealso cref= SampleModel#setDataElements </seealso>
		public override Object GetDataElements(int[] components, int offset, Object obj)
		{
			if (NeedScaleInit)
			{
				InitScale();
			}
			if (NoUnnorm)
			{
				throw new IllegalArgumentException("This ColorModel does not support the unnormalized form");
			}
			if ((components.Length - offset) < NumComponents_Renamed)
			{
				throw new IllegalArgumentException("Component array too small" + " (should be " + NumComponents_Renamed);
			}
			switch (TransferType_Renamed)
			{
			case DataBuffer.TYPE_INT:
			{
					int[] pixel;
					if (obj == null)
					{
						pixel = new int[NumComponents_Renamed];
					}
					else
					{
						pixel = (int[]) obj;
					}
					System.Array.Copy(components, offset, pixel, 0, NumComponents_Renamed);
					return pixel;
			}

			case DataBuffer.TYPE_BYTE:
			{
					sbyte[] pixel;
					if (obj == null)
					{
						pixel = new sbyte[NumComponents_Renamed];
					}
					else
					{
						pixel = (sbyte[]) obj;
					}
					for (int i = 0; i < NumComponents_Renamed; i++)
					{
						pixel[i] = unchecked((sbyte)(components[offset + i] & 0xff));
					}
					return pixel;
			}

			case DataBuffer.TYPE_USHORT:
			{
					short[] pixel;
					if (obj == null)
					{
						pixel = new short[NumComponents_Renamed];
					}
					else
					{
						pixel = (short[]) obj;
					}
					for (int i = 0; i < NumComponents_Renamed; i++)
					{
						pixel[i] = unchecked((short)(components[offset + i] & 0xffff));
					}
					return pixel;
			}

			default:
				throw new UnsupportedOperationException("This method has not been " + "implemented for transferType " + TransferType_Renamed);
			}
		}

		/// <summary>
		/// Returns a pixel value represented as an <code>int</code> in this
		/// <code>ColorModel</code>, given an array of normalized color/alpha
		/// components.  This method will throw an
		/// <code>IllegalArgumentException</code> if pixel values for this
		/// <code>ColorModel</code> are not conveniently representable as a
		/// single <code>int</code>.  An
		/// <code>ArrayIndexOutOfBoundsException</code> is thrown if  the
		/// <code>normComponents</code> array is not large enough to hold all the
		/// color and alpha components (starting at <code>normOffset</code>). </summary>
		/// <param name="normComponents"> an array of normalized color and alpha
		/// components </param>
		/// <param name="normOffset"> the index into <code>normComponents</code> at which to
		/// begin retrieving the color and alpha components </param>
		/// <returns> an <code>int</code> pixel value in this
		/// <code>ColorModel</code> corresponding to the specified components. </returns>
		/// <exception cref="IllegalArgumentException"> if
		///  pixel values for this <code>ColorModel</code> are not
		///  conveniently representable as a single <code>int</code> </exception>
		/// <exception cref="ArrayIndexOutOfBoundsException"> if
		///  the <code>normComponents</code> array is not large enough to
		///  hold all of the color and alpha components starting at
		///  <code>normOffset</code>
		/// @since 1.4 </exception>
		public override int GetDataElement(float[] normComponents, int normOffset)
		{
			if (NumComponents_Renamed > 1)
			{
				throw new IllegalArgumentException("More than one component per pixel");
			}
			if (Signed)
			{
				throw new IllegalArgumentException("Component value is signed");
			}
			if (NeedScaleInit)
			{
				InitScale();
			}
			Object pixel = GetDataElements(normComponents, normOffset, null);
			switch (TransferType_Renamed)
			{
			case DataBuffer.TYPE_BYTE:
			{
					sbyte[] bpixel = (sbyte[]) pixel;
					return bpixel[0] & 0xff;
			}
			case DataBuffer.TYPE_USHORT:
			{
					short[] uspixel = (short[]) pixel;
					return uspixel[0] & 0xffff;
			}
			case DataBuffer.TYPE_INT:
			{
					int[] ipixel = (int[]) pixel;
					return ipixel[0];
			}
			default:
				throw new UnsupportedOperationException("This method has not been " + "implemented for transferType " + TransferType_Renamed);
			}
		}

		/// <summary>
		/// Returns a data element array representation of a pixel in this
		/// <code>ColorModel</code>, given an array of normalized color/alpha
		/// components.  This array can then be passed to the
		/// <code>setDataElements</code> method of a <code>WritableRaster</code>
		/// object.  An <code>ArrayIndexOutOfBoundsException</code> is thrown
		/// if the <code>normComponents</code> array is not large enough to hold
		/// all the color and alpha components (starting at
		/// <code>normOffset</code>).  If the <code>obj</code> variable is
		/// <code>null</code>, a new array will be allocated.  If
		/// <code>obj</code> is not <code>null</code>, it must be a primitive
		/// array of type transferType; otherwise, a
		/// <code>ClassCastException</code> is thrown.  An
		/// <code>ArrayIndexOutOfBoundsException</code> is thrown if
		/// <code>obj</code> is not large enough to hold a pixel value for this
		/// <code>ColorModel</code>. </summary>
		/// <param name="normComponents"> an array of normalized color and alpha
		/// components </param>
		/// <param name="normOffset"> the index into <code>normComponents</code> at which to
		/// begin retrieving color and alpha components </param>
		/// <param name="obj"> a primitive data array to hold the returned pixel </param>
		/// <returns> an <code>Object</code> which is a primitive data array
		/// representation of a pixel </returns>
		/// <exception cref="ClassCastException"> if <code>obj</code>
		///  is not a primitive array of type <code>transferType</code> </exception>
		/// <exception cref="ArrayIndexOutOfBoundsException"> if
		///  <code>obj</code> is not large enough to hold a pixel value
		///  for this <code>ColorModel</code> or the <code>normComponents</code>
		///  array is not large enough to hold all of the color and alpha
		///  components starting at <code>normOffset</code> </exception>
		/// <seealso cref= WritableRaster#setDataElements </seealso>
		/// <seealso cref= SampleModel#setDataElements
		/// @since 1.4 </seealso>
		public override Object GetDataElements(float[] normComponents, int normOffset, Object obj)
		{
			bool needAlpha = SupportsAlpha && IsAlphaPremultiplied;
			float[] stdNormComponents;
			if (NeedScaleInit)
			{
				InitScale();
			}
			if (NonStdScale)
			{
				stdNormComponents = new float[NumComponents_Renamed];
				for (int c = 0, nc = normOffset; c < NumColorComponents_Renamed; c++, nc++)
				{
					stdNormComponents[c] = (normComponents[nc] - CompOffset[c]) * CompScale[c];
					// REMIND: need to analyze whether this
					// clamping is necessary
					if (stdNormComponents[c] < 0.0f)
					{
						stdNormComponents[c] = 0.0f;
					}
					if (stdNormComponents[c] > 1.0f)
					{
						stdNormComponents[c] = 1.0f;
					}
				}
				if (SupportsAlpha)
				{
					stdNormComponents[NumColorComponents_Renamed] = normComponents[NumColorComponents_Renamed + normOffset];
				}
				normOffset = 0;
			}
			else
			{
				stdNormComponents = normComponents;
			}
			switch (TransferType_Renamed)
			{
			case DataBuffer.TYPE_BYTE:
				sbyte[] bpixel;
				if (obj == null)
				{
					bpixel = new sbyte[NumComponents_Renamed];
				}
				else
				{
					bpixel = (sbyte[]) obj;
				}
				if (needAlpha)
				{
					float alpha = stdNormComponents[NumColorComponents_Renamed + normOffset];
					for (int c = 0, nc = normOffset; c < NumColorComponents_Renamed; c++, nc++)
					{
						bpixel[c] = (sbyte)((stdNormComponents[nc] * alpha) * ((float)((1 << NBits[c]) - 1)) + 0.5f);
					}
					bpixel[NumColorComponents_Renamed] = (sbyte)(alpha * ((float)((1 << NBits[NumColorComponents_Renamed]) - 1)) + 0.5f);
				}
				else
				{
					for (int c = 0, nc = normOffset; c < NumComponents_Renamed; c++, nc++)
					{
						bpixel[c] = (sbyte)(stdNormComponents[nc] * ((float)((1 << NBits[c]) - 1)) + 0.5f);
					}
				}
				return bpixel;
			case DataBuffer.TYPE_USHORT:
				short[] uspixel;
				if (obj == null)
				{
					uspixel = new short[NumComponents_Renamed];
				}
				else
				{
					uspixel = (short[]) obj;
				}
				if (needAlpha)
				{
					float alpha = stdNormComponents[NumColorComponents_Renamed + normOffset];
					for (int c = 0, nc = normOffset; c < NumColorComponents_Renamed; c++, nc++)
					{
						uspixel[c] = (short)((stdNormComponents[nc] * alpha) * ((float)((1 << NBits[c]) - 1)) + 0.5f);
					}
					uspixel[NumColorComponents_Renamed] = (short)(alpha * ((float)((1 << NBits[NumColorComponents_Renamed]) - 1)) + 0.5f);
				}
				else
				{
					for (int c = 0, nc = normOffset; c < NumComponents_Renamed; c++, nc++)
					{
						uspixel[c] = (short)(stdNormComponents[nc] * ((float)((1 << NBits[c]) - 1)) + 0.5f);
					}
				}
				return uspixel;
			case DataBuffer.TYPE_INT:
				int[] ipixel;
				if (obj == null)
				{
					ipixel = new int[NumComponents_Renamed];
				}
				else
				{
					ipixel = (int[]) obj;
				}
				if (needAlpha)
				{
					float alpha = stdNormComponents[NumColorComponents_Renamed + normOffset];
					for (int c = 0, nc = normOffset; c < NumColorComponents_Renamed; c++, nc++)
					{
						ipixel[c] = (int)((stdNormComponents[nc] * alpha) * ((float)((1 << NBits[c]) - 1)) + 0.5f);
					}
					ipixel[NumColorComponents_Renamed] = (int)(alpha * ((float)((1 << NBits[NumColorComponents_Renamed]) - 1)) + 0.5f);
				}
				else
				{
					for (int c = 0, nc = normOffset; c < NumComponents_Renamed; c++, nc++)
					{
						ipixel[c] = (int)(stdNormComponents[nc] * ((float)((1 << NBits[c]) - 1)) + 0.5f);
					}
				}
				return ipixel;
			case DataBuffer.TYPE_SHORT:
				short[] spixel;
				if (obj == null)
				{
					spixel = new short[NumComponents_Renamed];
				}
				else
				{
					spixel = (short[]) obj;
				}
				if (needAlpha)
				{
					float alpha = stdNormComponents[NumColorComponents_Renamed + normOffset];
					for (int c = 0, nc = normOffset; c < NumColorComponents_Renamed; c++, nc++)
					{
						spixel[c] = (short)(stdNormComponents[nc] * alpha * 32767.0f + 0.5f);
					}
					spixel[NumColorComponents_Renamed] = (short)(alpha * 32767.0f + 0.5f);
				}
				else
				{
					for (int c = 0, nc = normOffset; c < NumComponents_Renamed; c++, nc++)
					{
						spixel[c] = (short)(stdNormComponents[nc] * 32767.0f + 0.5f);
					}
				}
				return spixel;
			case DataBuffer.TYPE_FLOAT:
				float[] fpixel;
				if (obj == null)
				{
					fpixel = new float[NumComponents_Renamed];
				}
				else
				{
					fpixel = (float[]) obj;
				}
				if (needAlpha)
				{
					float alpha = normComponents[NumColorComponents_Renamed + normOffset];
					for (int c = 0, nc = normOffset; c < NumColorComponents_Renamed; c++, nc++)
					{
						fpixel[c] = normComponents[nc] * alpha;
					}
					fpixel[NumColorComponents_Renamed] = alpha;
				}
				else
				{
					for (int c = 0, nc = normOffset; c < NumComponents_Renamed; c++, nc++)
					{
						fpixel[c] = normComponents[nc];
					}
				}
				return fpixel;
			case DataBuffer.TYPE_DOUBLE:
				double[] dpixel;
				if (obj == null)
				{
					dpixel = new double[NumComponents_Renamed];
				}
				else
				{
					dpixel = (double[]) obj;
				}
				if (needAlpha)
				{
					double alpha = (double)(normComponents[NumColorComponents_Renamed + normOffset]);
					for (int c = 0, nc = normOffset; c < NumColorComponents_Renamed; c++, nc++)
					{
						dpixel[c] = normComponents[nc] * alpha;
					}
					dpixel[NumColorComponents_Renamed] = alpha;
				}
				else
				{
					for (int c = 0, nc = normOffset; c < NumComponents_Renamed; c++, nc++)
					{
						dpixel[c] = (double) normComponents[nc];
					}
				}
				return dpixel;
			default:
				throw new UnsupportedOperationException("This method has not been " + "implemented for transferType " + TransferType_Renamed);
			}
		}

		/// <summary>
		/// Returns an array of all of the color/alpha components in normalized
		/// form, given a pixel in this <code>ColorModel</code>.  The pixel
		/// value is specified by an array of data elements of type transferType
		/// passed in as an object reference.  If pixel is not a primitive array
		/// of type transferType, a <code>ClassCastException</code> is thrown.
		/// An <code>ArrayIndexOutOfBoundsException</code> is thrown if
		/// <code>pixel</code> is not large enough to hold a pixel value for this
		/// <code>ColorModel</code>.
		/// Normalized components are float values between a per component minimum
		/// and maximum specified by the <code>ColorSpace</code> object for this
		/// <code>ColorModel</code>.  If the
		/// <code>normComponents</code> array is <code>null</code>, a new array
		/// will be allocated.  The <code>normComponents</code> array
		/// will be returned.  Color/alpha components are stored in the
		/// <code>normComponents</code> array starting at
		/// <code>normOffset</code> (even if the array is allocated by this
		/// method).  An <code>ArrayIndexOutOfBoundsException</code> is thrown
		/// if the <code>normComponents</code> array is not <code>null</code>
		/// and is not large enough to hold all the color and alpha components
		/// (starting at <code>normOffset</code>).
		/// <para>
		/// This method must be overridden by a subclass if that subclass
		/// is designed to translate pixel sample values to color component values
		/// in a non-default way.  The default translations implemented by this
		/// class is described in the class comments.  Any subclass implementing
		/// a non-default translation must follow the constraints on allowable
		/// translations defined there.
		/// </para>
		/// </summary>
		/// <param name="pixel"> the specified pixel </param>
		/// <param name="normComponents"> an array to receive the normalized components </param>
		/// <param name="normOffset"> the offset into the <code>normComponents</code>
		/// array at which to start storing normalized components </param>
		/// <returns> an array containing normalized color and alpha
		/// components. </returns>
		/// <exception cref="ClassCastException"> if <code>pixel</code> is not a primitive
		///          array of type transferType </exception>
		/// <exception cref="ArrayIndexOutOfBoundsException"> if
		///          <code>normComponents</code> is not large enough to hold all
		///          color and alpha components starting at <code>normOffset</code> </exception>
		/// <exception cref="ArrayIndexOutOfBoundsException"> if
		///          <code>pixel</code> is not large enough to hold a pixel
		///          value for this <code>ColorModel</code>.
		/// @since 1.4 </exception>
		public override float[] GetNormalizedComponents(Object pixel, float[] normComponents, int normOffset)
		{
			if (normComponents == null)
			{
				normComponents = new float[NumComponents_Renamed + normOffset];
			}
			switch (TransferType_Renamed)
			{
			case DataBuffer.TYPE_BYTE:
				sbyte[] bpixel = (sbyte[]) pixel;
				for (int c = 0, nc = normOffset; c < NumComponents_Renamed; c++, nc++)
				{
					normComponents[nc] = ((float)(bpixel[c] & 0xff)) / ((float)((1 << NBits[c]) - 1));
				}
				break;
			case DataBuffer.TYPE_USHORT:
				short[] uspixel = (short[]) pixel;
				for (int c = 0, nc = normOffset; c < NumComponents_Renamed; c++, nc++)
				{
					normComponents[nc] = ((float)(uspixel[c] & 0xffff)) / ((float)((1 << NBits[c]) - 1));
				}
				break;
			case DataBuffer.TYPE_INT:
				int[] ipixel = (int[]) pixel;
				for (int c = 0, nc = normOffset; c < NumComponents_Renamed; c++, nc++)
				{
					normComponents[nc] = ((float) ipixel[c]) / ((float)((1 << NBits[c]) - 1));
				}
				break;
			case DataBuffer.TYPE_SHORT:
				short[] spixel = (short[]) pixel;
				for (int c = 0, nc = normOffset; c < NumComponents_Renamed; c++, nc++)
				{
					normComponents[nc] = ((float) spixel[c]) / 32767.0f;
				}
				break;
			case DataBuffer.TYPE_FLOAT:
				float[] fpixel = (float[]) pixel;
				for (int c = 0, nc = normOffset; c < NumComponents_Renamed; c++, nc++)
				{
					normComponents[nc] = fpixel[c];
				}
				break;
			case DataBuffer.TYPE_DOUBLE:
				double[] dpixel = (double[]) pixel;
				for (int c = 0, nc = normOffset; c < NumComponents_Renamed; c++, nc++)
				{
					normComponents[nc] = (float) dpixel[c];
				}
				break;
			default:
				throw new UnsupportedOperationException("This method has not been " + "implemented for transferType " + TransferType_Renamed);
			}

			if (SupportsAlpha && IsAlphaPremultiplied)
			{
				float alpha = normComponents[NumColorComponents_Renamed + normOffset];
				if (alpha != 0.0f)
				{
					float invAlpha = 1.0f / alpha;
					for (int c = normOffset; c < NumColorComponents_Renamed + normOffset; c++)
					{
						normComponents[c] *= invAlpha;
					}
				}
			}
			if (Min != null)
			{
				// Normally (i.e. when this class is not subclassed to override
				// this method), the test (min != null) will be equivalent to
				// the test (nonStdScale).  However, there is an unlikely, but
				// possible case, in which this method is overridden, nonStdScale
				// is set true by initScale(), the subclass method for some
				// reason calls this superclass method, but the min and
				// diffMinMax arrays were never initialized by setupLUTs().  In
				// that case, the right thing to do is follow the intended
				// semantics of this method, and rescale the color components
				// only if the ColorSpace min/max were detected to be other
				// than 0.0/1.0 by setupLUTs().  Note that this implies the
				// transferType is byte, ushort, int, or short - i.e. components
				// derived from float and double pixel data are never rescaled.
				for (int c = 0; c < NumColorComponents_Renamed; c++)
				{
					normComponents[c + normOffset] = Min[c] + DiffMinMax[c] * normComponents[c + normOffset];
				}
			}
			return normComponents;
		}

		/// <summary>
		/// Forces the raster data to match the state specified in the
		/// <CODE>isAlphaPremultiplied</CODE> variable, assuming the data
		/// is currently correctly described by this <CODE>ColorModel</CODE>.
		/// It may multiply or divide the color raster data by alpha, or
		/// do nothing if the data is in the correct state.  If the data needs
		/// to be coerced, this method also returns an instance of
		/// this <CODE>ColorModel</CODE> with
		/// the <CODE>isAlphaPremultiplied</CODE> flag set appropriately.
		/// Since <code>ColorModel</code> can be subclassed, subclasses inherit
		/// the implementation of this method and if they don't override it
		/// then they throw an exception if they use an unsupported
		/// <code>transferType</code>.
		/// </summary>
		/// <exception cref="NullPointerException"> if <code>raster</code> is
		/// <code>null</code> and data coercion is required. </exception>
		/// <exception cref="UnsupportedOperationException"> if the transfer type of
		/// this <CODE>ComponentColorModel</CODE>
		/// is not one of the supported transfer types:
		/// <CODE>DataBuffer.TYPE_BYTE</CODE>, <CODE>DataBuffer.TYPE_USHORT</CODE>,
		/// <CODE>DataBuffer.TYPE_INT</CODE>, <CODE>DataBuffer.TYPE_SHORT</CODE>,
		/// <CODE>DataBuffer.TYPE_FLOAT</CODE>, or <CODE>DataBuffer.TYPE_DOUBLE</CODE>. </exception>
		public override ColorModel CoerceData(WritableRaster raster, bool isAlphaPremultiplied)
		{
			if ((SupportsAlpha == false) || (this.IsAlphaPremultiplied == isAlphaPremultiplied))
			{
				// Nothing to do
				return this;
			}

			int w = raster.Width;
			int h = raster.Height;
			int aIdx = raster.NumBands - 1;
			float normAlpha;
			int rminX = raster.MinX;
			int rY = raster.MinY;
			int rX;
			if (isAlphaPremultiplied)
			{
				switch (TransferType_Renamed)
				{
					case DataBuffer.TYPE_BYTE:
					{
						sbyte[] pixel = null;
						sbyte[] zpixel = null;
						float alphaScale = 1.0f / ((float)((1 << NBits[aIdx]) - 1));
						for (int y = 0; y < h; y++, rY++)
						{
							rX = rminX;
							for (int x = 0; x < w; x++, rX++)
							{
								pixel = (sbyte[])raster.GetDataElements(rX, rY, pixel);
								normAlpha = (pixel[aIdx] & 0xff) * alphaScale;
								if (normAlpha != 0.0f)
								{
									for (int c = 0; c < aIdx; c++)
									{
										pixel[c] = unchecked((sbyte)((pixel[c] & 0xff) * normAlpha + 0.5f));
									}
									raster.SetDataElements(rX, rY, pixel);
								}
								else
								{
									if (zpixel == null)
									{
										zpixel = new sbyte[NumComponents_Renamed];
										Arrays.Fill(zpixel, (sbyte) 0);
									}
									raster.SetDataElements(rX, rY, zpixel);
								}
							}
						}
					}
					break;
					case DataBuffer.TYPE_USHORT:
					{
						short[] pixel = null;
						short[] zpixel = null;
						float alphaScale = 1.0f / ((float)((1 << NBits[aIdx]) - 1));
						for (int y = 0; y < h; y++, rY++)
						{
							rX = rminX;
							for (int x = 0; x < w; x++, rX++)
							{
								pixel = (short[])raster.GetDataElements(rX, rY, pixel);
								normAlpha = (pixel[aIdx] & 0xffff) * alphaScale;
								if (normAlpha != 0.0f)
								{
									for (int c = 0; c < aIdx; c++)
									{
										pixel[c] = unchecked((short)((pixel[c] & 0xffff) * normAlpha + 0.5f));
									}
									raster.SetDataElements(rX, rY, pixel);
								}
								else
								{
									if (zpixel == null)
									{
										zpixel = new short[NumComponents_Renamed];
										Arrays.Fill(zpixel, (short) 0);
									}
									raster.SetDataElements(rX, rY, zpixel);
								}
							}
						}
					}
					break;
					case DataBuffer.TYPE_INT:
					{
						int[] pixel = null;
						int[] zpixel = null;
						float alphaScale = 1.0f / ((float)((1 << NBits[aIdx]) - 1));
						for (int y = 0; y < h; y++, rY++)
						{
							rX = rminX;
							for (int x = 0; x < w; x++, rX++)
							{
								pixel = (int[])raster.GetDataElements(rX, rY, pixel);
								normAlpha = pixel[aIdx] * alphaScale;
								if (normAlpha != 0.0f)
								{
									for (int c = 0; c < aIdx; c++)
									{
										pixel[c] = (int)(pixel[c] * normAlpha + 0.5f);
									}
									raster.SetDataElements(rX, rY, pixel);
								}
								else
								{
									if (zpixel == null)
									{
										zpixel = new int[NumComponents_Renamed];
										Arrays.Fill(zpixel, 0);
									}
									raster.SetDataElements(rX, rY, zpixel);
								}
							}
						}
					}
					break;
					case DataBuffer.TYPE_SHORT:
					{
						short[] pixel = null;
						short[] zpixel = null;
						float alphaScale = 1.0f / 32767.0f;
						for (int y = 0; y < h; y++, rY++)
						{
							rX = rminX;
							for (int x = 0; x < w; x++, rX++)
							{
								pixel = (short[]) raster.GetDataElements(rX, rY, pixel);
								normAlpha = pixel[aIdx] * alphaScale;
								if (normAlpha != 0.0f)
								{
									for (int c = 0; c < aIdx; c++)
									{
										pixel[c] = (short)(pixel[c] * normAlpha + 0.5f);
									}
									raster.SetDataElements(rX, rY, pixel);
								}
								else
								{
									if (zpixel == null)
									{
										zpixel = new short[NumComponents_Renamed];
										Arrays.Fill(zpixel, (short) 0);
									}
									raster.SetDataElements(rX, rY, zpixel);
								}
							}
						}
					}
					break;
					case DataBuffer.TYPE_FLOAT:
					{
						float[] pixel = null;
						float[] zpixel = null;
						for (int y = 0; y < h; y++, rY++)
						{
							rX = rminX;
							for (int x = 0; x < w; x++, rX++)
							{
								pixel = (float[]) raster.GetDataElements(rX, rY, pixel);
								normAlpha = pixel[aIdx];
								if (normAlpha != 0.0f)
								{
									for (int c = 0; c < aIdx; c++)
									{
										pixel[c] *= normAlpha;
									}
									raster.SetDataElements(rX, rY, pixel);
								}
								else
								{
									if (zpixel == null)
									{
										zpixel = new float[NumComponents_Renamed];
										Arrays.Fill(zpixel, 0.0f);
									}
									raster.SetDataElements(rX, rY, zpixel);
								}
							}
						}
					}
					break;
					case DataBuffer.TYPE_DOUBLE:
					{
						double[] pixel = null;
						double[] zpixel = null;
						for (int y = 0; y < h; y++, rY++)
						{
							rX = rminX;
							for (int x = 0; x < w; x++, rX++)
							{
								pixel = (double[]) raster.GetDataElements(rX, rY, pixel);
								double dnormAlpha = pixel[aIdx];
								if (dnormAlpha != 0.0)
								{
									for (int c = 0; c < aIdx; c++)
									{
										pixel[c] *= dnormAlpha;
									}
									raster.SetDataElements(rX, rY, pixel);
								}
								else
								{
									if (zpixel == null)
									{
										zpixel = new double[NumComponents_Renamed];
										Arrays.Fill(zpixel, 0.0);
									}
									raster.SetDataElements(rX, rY, zpixel);
								}
							}
						}
					}
					break;
					default:
						throw new UnsupportedOperationException("This method has not been " + "implemented for transferType " + TransferType_Renamed);
				}
			}
			else
			{
				// We are premultiplied and want to divide it out
				switch (TransferType_Renamed)
				{
					case DataBuffer.TYPE_BYTE:
					{
						sbyte[] pixel = null;
						float alphaScale = 1.0f / ((float)((1 << NBits[aIdx]) - 1));
						for (int y = 0; y < h; y++, rY++)
						{
							rX = rminX;
							for (int x = 0; x < w; x++, rX++)
							{
								pixel = (sbyte[])raster.GetDataElements(rX, rY, pixel);
								normAlpha = (pixel[aIdx] & 0xff) * alphaScale;
								if (normAlpha != 0.0f)
								{
									float invAlpha = 1.0f / normAlpha;
									for (int c = 0; c < aIdx; c++)
									{
										pixel[c] = unchecked((sbyte)((pixel[c] & 0xff) * invAlpha + 0.5f));
									}
									raster.SetDataElements(rX, rY, pixel);
								}
							}
						}
					}
					break;
					case DataBuffer.TYPE_USHORT:
					{
						short[] pixel = null;
						float alphaScale = 1.0f / ((float)((1 << NBits[aIdx]) - 1));
						for (int y = 0; y < h; y++, rY++)
						{
							rX = rminX;
							for (int x = 0; x < w; x++, rX++)
							{
								pixel = (short[])raster.GetDataElements(rX, rY, pixel);
								normAlpha = (pixel[aIdx] & 0xffff) * alphaScale;
								if (normAlpha != 0.0f)
								{
									float invAlpha = 1.0f / normAlpha;
									for (int c = 0; c < aIdx; c++)
									{
										pixel[c] = unchecked((short)((pixel[c] & 0xffff) * invAlpha + 0.5f));
									}
									raster.SetDataElements(rX, rY, pixel);
								}
							}
						}
					}
					break;
					case DataBuffer.TYPE_INT:
					{
						int[] pixel = null;
						float alphaScale = 1.0f / ((float)((1 << NBits[aIdx]) - 1));
						for (int y = 0; y < h; y++, rY++)
						{
							rX = rminX;
							for (int x = 0; x < w; x++, rX++)
							{
								pixel = (int[])raster.GetDataElements(rX, rY, pixel);
								normAlpha = pixel[aIdx] * alphaScale;
								if (normAlpha != 0.0f)
								{
									float invAlpha = 1.0f / normAlpha;
									for (int c = 0; c < aIdx; c++)
									{
										pixel[c] = (int)(pixel[c] * invAlpha + 0.5f);
									}
									raster.SetDataElements(rX, rY, pixel);
								}
							}
						}
					}
					break;
					case DataBuffer.TYPE_SHORT:
					{
						short[] pixel = null;
						float alphaScale = 1.0f / 32767.0f;
						for (int y = 0; y < h; y++, rY++)
						{
							rX = rminX;
							for (int x = 0; x < w; x++, rX++)
							{
								pixel = (short[])raster.GetDataElements(rX, rY, pixel);
								normAlpha = pixel[aIdx] * alphaScale;
								if (normAlpha != 0.0f)
								{
									float invAlpha = 1.0f / normAlpha;
									for (int c = 0; c < aIdx; c++)
									{
										pixel[c] = (short)(pixel[c] * invAlpha + 0.5f);
									}
									raster.SetDataElements(rX, rY, pixel);
								}
							}
						}
					}
					break;
					case DataBuffer.TYPE_FLOAT:
					{
						float[] pixel = null;
						for (int y = 0; y < h; y++, rY++)
						{
							rX = rminX;
							for (int x = 0; x < w; x++, rX++)
							{
								pixel = (float[])raster.GetDataElements(rX, rY, pixel);
								normAlpha = pixel[aIdx];
								if (normAlpha != 0.0f)
								{
									float invAlpha = 1.0f / normAlpha;
									for (int c = 0; c < aIdx; c++)
									{
										pixel[c] *= invAlpha;
									}
									raster.SetDataElements(rX, rY, pixel);
								}
							}
						}
					}
					break;
					case DataBuffer.TYPE_DOUBLE:
					{
						double[] pixel = null;
						for (int y = 0; y < h; y++, rY++)
						{
							rX = rminX;
							for (int x = 0; x < w; x++, rX++)
							{
								pixel = (double[])raster.GetDataElements(rX, rY, pixel);
								double dnormAlpha = pixel[aIdx];
								if (dnormAlpha != 0.0)
								{
									double invAlpha = 1.0 / dnormAlpha;
									for (int c = 0; c < aIdx; c++)
									{
										pixel[c] *= invAlpha;
									}
									raster.SetDataElements(rX, rY, pixel);
								}
							}
						}
					}
					break;
					default:
						throw new UnsupportedOperationException("This method has not been " + "implemented for transferType " + TransferType_Renamed);
				}
			}

			// Return a new color model
			if (!Signed)
			{
				return new ComponentColorModel(ColorSpace_Renamed, NBits, SupportsAlpha, isAlphaPremultiplied, Transparency_Renamed, TransferType_Renamed);
			}
			else
			{
				return new ComponentColorModel(ColorSpace_Renamed, SupportsAlpha, isAlphaPremultiplied, Transparency_Renamed, TransferType_Renamed);
			}

		}

		/// <summary>
		/// Returns true if <CODE>raster</CODE> is compatible with this
		/// <CODE>ColorModel</CODE>; false if it is not.
		/// </summary>
		/// <param name="raster"> The <CODE>Raster</CODE> object to test for compatibility.
		/// </param>
		/// <returns> <CODE>true</CODE> if <CODE>raster</CODE> is compatible with this
		/// <CODE>ColorModel</CODE>, <CODE>false</CODE> if it is not. </returns>
		public override bool IsCompatibleRaster(Raster raster)
		{

			SampleModel sm = raster.SampleModel;

			if (sm is ComponentSampleModel)
			{
				if (sm.NumBands != NumComponents)
				{
					return false;
				}
				for (int i = 0; i < NBits.Length; i++)
				{
					if (sm.GetSampleSize(i) < NBits[i])
					{
						return false;
					}
				}
				return (raster.TransferType == TransferType_Renamed);
			}
			else
			{
				return false;
			}
		}

		/// <summary>
		/// Creates a <CODE>WritableRaster</CODE> with the specified width and height,
		/// that  has a data layout (<CODE>SampleModel</CODE>) compatible with
		/// this <CODE>ColorModel</CODE>.
		/// </summary>
		/// <param name="w"> The width of the <CODE>WritableRaster</CODE> you want to create. </param>
		/// <param name="h"> The height of the <CODE>WritableRaster</CODE> you want to create.
		/// </param>
		/// <returns> A <CODE>WritableRaster</CODE> that is compatible with
		/// this <CODE>ColorModel</CODE>. </returns>
		/// <seealso cref= WritableRaster </seealso>
		/// <seealso cref= SampleModel </seealso>
		public override WritableRaster CreateCompatibleWritableRaster(int w, int h)
		{
			int dataSize = w * h * NumComponents_Renamed;
			WritableRaster raster = null;

			switch (TransferType_Renamed)
			{
			case DataBuffer.TYPE_BYTE:
			case DataBuffer.TYPE_USHORT:
				raster = Raster.CreateInterleavedRaster(TransferType_Renamed, w, h, NumComponents_Renamed, null);
				break;
			default:
				SampleModel sm = CreateCompatibleSampleModel(w, h);
				DataBuffer db = sm.CreateDataBuffer();
				raster = Raster.CreateWritableRaster(sm, db, null);
			break;
			}

			return raster;
		}

		/// <summary>
		/// Creates a <CODE>SampleModel</CODE> with the specified width and height,
		/// that  has a data layout compatible with this <CODE>ColorModel</CODE>.
		/// </summary>
		/// <param name="w"> The width of the <CODE>SampleModel</CODE> you want to create. </param>
		/// <param name="h"> The height of the <CODE>SampleModel</CODE> you want to create.
		/// </param>
		/// <returns> A <CODE>SampleModel</CODE> that is compatible with this
		/// <CODE>ColorModel</CODE>.
		/// </returns>
		/// <seealso cref= SampleModel </seealso>
		public override SampleModel CreateCompatibleSampleModel(int w, int h)
		{
			int[] bandOffsets = new int[NumComponents_Renamed];
			for (int i = 0; i < NumComponents_Renamed; i++)
			{
				bandOffsets[i] = i;
			}
			switch (TransferType_Renamed)
			{
			case DataBuffer.TYPE_BYTE:
			case DataBuffer.TYPE_USHORT:
				return new PixelInterleavedSampleModel(TransferType_Renamed, w, h, NumComponents_Renamed, w * NumComponents_Renamed, bandOffsets);
			default:
				return new ComponentSampleModel(TransferType_Renamed, w, h, NumComponents_Renamed, w * NumComponents_Renamed, bandOffsets);
			}
		}

		/// <summary>
		/// Checks whether or not the specified <CODE>SampleModel</CODE>
		/// is compatible with this <CODE>ColorModel</CODE>.
		/// </summary>
		/// <param name="sm"> The <CODE>SampleModel</CODE> to test for compatibility.
		/// </param>
		/// <returns> <CODE>true</CODE> if the <CODE>SampleModel</CODE> is
		/// compatible with this <CODE>ColorModel</CODE>, <CODE>false</CODE>
		/// if it is not.
		/// </returns>
		/// <seealso cref= SampleModel </seealso>
		public override bool IsCompatibleSampleModel(SampleModel sm)
		{
			if (!(sm is ComponentSampleModel))
			{
				return false;
			}

			// Must have the same number of components
			if (NumComponents_Renamed != sm.NumBands)
			{
				return false;
			}

			if (sm.TransferType != TransferType_Renamed)
			{
				return false;
			}

			return true;
		}

		/// <summary>
		/// Returns a <CODE>Raster</CODE> representing the alpha channel of an image,
		/// extracted from the input <CODE>Raster</CODE>.
		/// This method assumes that <CODE>Raster</CODE> objects associated with
		/// this <CODE>ColorModel</CODE> store the alpha band, if present, as
		/// the last band of image data. Returns null if there is no separate spatial
		/// alpha channel associated with this <CODE>ColorModel</CODE>.
		/// This method creates a new <CODE>Raster</CODE>, but will share the data
		/// array.
		/// </summary>
		/// <param name="raster"> The <CODE>WritableRaster</CODE> from which to extract the
		/// alpha  channel.
		/// </param>
		/// <returns> A <CODE>WritableRaster</CODE> containing the image's alpha channel.
		///  </returns>
		public override WritableRaster GetAlphaRaster(WritableRaster raster)
		{
			if (HasAlpha() == false)
			{
				return null;
			}

			int x = raster.MinX;
			int y = raster.MinY;
			int[] band = new int[1];
			band[0] = raster.NumBands - 1;
			return raster.CreateWritableChild(x, y, raster.Width, raster.Height, x, y, band);
		}

		/// <summary>
		/// Compares this color model with another for equality.
		/// </summary>
		/// <param name="obj"> The object to compare with this color model. </param>
		/// <returns> <CODE>true</CODE> if the color model objects are equal,
		/// <CODE>false</CODE> if they are not. </returns>
		public override bool Equals(Object obj)
		{
			if (!base.Equals(obj))
			{
				return false;
			}

			if (obj.GetType() != this.GetType())
			{
				return false;
			}

			return true;
		}

	}

}
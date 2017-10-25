/*
 * Copyright (c) 1997, 2011, Oracle and/or its affiliates. All rights reserved.
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
	/// The <code>PackedColorModel</code> class is an abstract
	/// <seealso cref="ColorModel"/> class that works with pixel values which represent
	/// color and alpha information as separate samples and which pack all
	/// samples for a single pixel into a single int, short, or byte quantity.
	/// This class can be used with an arbitrary <seealso cref="ColorSpace"/>.  The number of
	/// color samples in the pixel values must be the same as the number of color
	/// components in the <code>ColorSpace</code>.  There can be a single alpha
	/// sample.  The array length is always 1 for those methods that use a
	/// primitive array pixel representation of type <code>transferType</code>.
	/// The transfer types supported are DataBuffer.TYPE_BYTE,
	/// DataBuffer.TYPE_USHORT, and DataBuffer.TYPE_INT.
	/// Color and alpha samples are stored in the single element of the array
	/// in bits indicated by bit masks.  Each bit mask must be contiguous and
	/// masks must not overlap.  The same masks apply to the single int
	/// pixel representation used by other methods.  The correspondence of
	/// masks and color/alpha samples is as follows:
	/// <ul>
	/// <li> Masks are identified by indices running from 0 through
	/// <seealso cref="ColorModel#getNumComponents() getNumComponents"/>&nbsp;-&nbsp;1.
	/// <li> The first
	/// <seealso cref="ColorModel#getNumColorComponents() getNumColorComponents"/>
	/// indices refer to color samples.
	/// <li> If an alpha sample is present, it corresponds the last index.
	/// <li> The order of the color indices is specified
	/// by the <code>ColorSpace</code>.  Typically, this reflects the name of
	/// the color space type (for example, TYPE_RGB), index 0
	/// corresponds to red, index 1 to green, and index 2 to blue.
	/// </ul>
	/// <para>
	/// The translation from pixel values to color/alpha components for
	/// display or processing purposes is a one-to-one correspondence of
	/// samples to components.
	/// A <code>PackedColorModel</code> is typically used with image data
	/// that uses masks to define packed samples.  For example, a
	/// <code>PackedColorModel</code> can be used in conjunction with a
	/// <seealso cref="SinglePixelPackedSampleModel"/> to construct a
	/// <seealso cref="BufferedImage"/>.  Normally the masks used by the
	/// <seealso cref="SampleModel"/> and the <code>ColorModel</code> would be the same.
	/// However, if they are different, the color interpretation of pixel data is
	/// done according to the masks of the <code>ColorModel</code>.
	/// </para>
	/// <para>
	/// A single <code>int</code> pixel representation is valid for all objects
	/// of this class since it is always possible to represent pixel values
	/// used with this class in a single <code>int</code>.  Therefore, methods
	/// that use this representation do not throw an
	/// <code>IllegalArgumentException</code> due to an invalid pixel value.
	/// </para>
	/// <para>
	/// A subclass of <code>PackedColorModel</code> is <seealso cref="DirectColorModel"/>,
	/// which is similar to an X11 TrueColor visual.
	/// 
	/// </para>
	/// </summary>
	/// <seealso cref= DirectColorModel </seealso>
	/// <seealso cref= SinglePixelPackedSampleModel </seealso>
	/// <seealso cref= BufferedImage </seealso>

	public abstract class PackedColorModel : ColorModel
	{
		internal int[] MaskArray;
		internal int[] MaskOffsets;
		internal float[] ScaleFactors;

		/// <summary>
		/// Constructs a <code>PackedColorModel</code> from a color mask array,
		/// which specifies which bits in an <code>int</code> pixel representation
		/// contain each of the color samples, and an alpha mask.  Color
		/// components are in the specified <code>ColorSpace</code>.  The length of
		/// <code>colorMaskArray</code> should be the number of components in
		/// the <code>ColorSpace</code>.  All of the bits in each mask
		/// must be contiguous and fit in the specified number of least significant
		/// bits of an <code>int</code> pixel representation.  If the
		/// <code>alphaMask</code> is 0, there is no alpha.  If there is alpha,
		/// the <code>boolean</code> <code>isAlphaPremultiplied</code> specifies
		/// how to interpret color and alpha samples in pixel values.  If the
		/// <code>boolean</code> is <code>true</code>, color samples are assumed
		/// to have been multiplied by the alpha sample.  The transparency,
		/// <code>trans</code>, specifies what alpha values can be represented
		/// by this color model.  The transfer type is the type of primitive
		/// array used to represent pixel values. </summary>
		/// <param name="space"> the specified <code>ColorSpace</code> </param>
		/// <param name="bits"> the number of bits in the pixel values </param>
		/// <param name="colorMaskArray"> array that specifies the masks representing
		///         the bits of the pixel values that represent the color
		///         components </param>
		/// <param name="alphaMask"> specifies the mask representing
		///         the bits of the pixel values that represent the alpha
		///         component </param>
		/// <param name="isAlphaPremultiplied"> <code>true</code> if color samples are
		///        premultiplied by the alpha sample; <code>false</code> otherwise </param>
		/// <param name="trans"> specifies the alpha value that can be represented by
		///        this color model </param>
		/// <param name="transferType"> the type of array used to represent pixel values </param>
		/// <exception cref="IllegalArgumentException"> if <code>bits</code> is less than
		///         1 or greater than 32 </exception>
		public PackedColorModel(ColorSpace space, int bits, int[] colorMaskArray, int alphaMask, bool isAlphaPremultiplied, int trans, int transferType) : base(bits, PackedColorModel.CreateBitsArray(colorMaskArray, alphaMask), space, (alphaMask == 0 ? false : true), isAlphaPremultiplied, trans, transferType)
		{
			if (bits < 1 || bits > 32)
			{
				throw new IllegalArgumentException("Number of bits must be between" + " 1 and 32.");
			}
			MaskArray = new int[NumComponents_Renamed];
			MaskOffsets = new int[NumComponents_Renamed];
			ScaleFactors = new float[NumComponents_Renamed];

			for (int i = 0; i < NumColorComponents_Renamed; i++)
			{
				// Get the mask offset and #bits
				DecomposeMask(colorMaskArray[i], i, space.GetName(i));
			}
			if (alphaMask != 0)
			{
				DecomposeMask(alphaMask, NumColorComponents_Renamed, "alpha");
				if (NBits[NumComponents_Renamed - 1] == 1)
				{
					Transparency_Renamed = java.awt.Transparency_Fields.BITMASK;
				}
			}
		}

		/// <summary>
		/// Constructs a <code>PackedColorModel</code> from the specified
		/// masks which indicate which bits in an <code>int</code> pixel
		/// representation contain the alpha, red, green and blue color samples.
		/// Color components are in the specified <code>ColorSpace</code>, which
		/// must be of type ColorSpace.TYPE_RGB.  All of the bits in each
		/// mask must be contiguous and fit in the specified number of
		/// least significant bits of an <code>int</code> pixel representation.  If
		/// <code>amask</code> is 0, there is no alpha.  If there is alpha,
		/// the <code>boolean</code> <code>isAlphaPremultiplied</code>
		/// specifies how to interpret color and alpha samples
		/// in pixel values.  If the <code>boolean</code> is <code>true</code>,
		/// color samples are assumed to have been multiplied by the alpha sample.
		/// The transparency, <code>trans</code>, specifies what alpha values
		/// can be represented by this color model.
		/// The transfer type is the type of primitive array used to represent
		/// pixel values. </summary>
		/// <param name="space"> the specified <code>ColorSpace</code> </param>
		/// <param name="bits"> the number of bits in the pixel values </param>
		/// <param name="rmask"> specifies the mask representing
		///         the bits of the pixel values that represent the red
		///         color component </param>
		/// <param name="gmask"> specifies the mask representing
		///         the bits of the pixel values that represent the green
		///         color component </param>
		/// <param name="bmask"> specifies the mask representing
		///         the bits of the pixel values that represent
		///         the blue color component </param>
		/// <param name="amask"> specifies the mask representing
		///         the bits of the pixel values that represent
		///         the alpha component </param>
		/// <param name="isAlphaPremultiplied"> <code>true</code> if color samples are
		///        premultiplied by the alpha sample; <code>false</code> otherwise </param>
		/// <param name="trans"> specifies the alpha value that can be represented by
		///        this color model </param>
		/// <param name="transferType"> the type of array used to represent pixel values </param>
		/// <exception cref="IllegalArgumentException"> if <code>space</code> is not a
		///         TYPE_RGB space </exception>
		/// <seealso cref= ColorSpace </seealso>
		public PackedColorModel(ColorSpace space, int bits, int rmask, int gmask, int bmask, int amask, bool isAlphaPremultiplied, int trans, int transferType) : base(bits, PackedColorModel.CreateBitsArray(rmask, gmask, bmask, amask), space, (amask == 0 ? false : true), isAlphaPremultiplied, trans, transferType)
		{

			if (space.Type != ColorSpace.TYPE_RGB)
			{
				throw new IllegalArgumentException("ColorSpace must be TYPE_RGB.");
			}
			MaskArray = new int[NumComponents_Renamed];
			MaskOffsets = new int[NumComponents_Renamed];
			ScaleFactors = new float[NumComponents_Renamed];

			DecomposeMask(rmask, 0, "red");

			DecomposeMask(gmask, 1, "green");

			DecomposeMask(bmask, 2, "blue");

			if (amask != 0)
			{
				DecomposeMask(amask, 3, "alpha");
				if (NBits[3] == 1)
				{
					Transparency_Renamed = java.awt.Transparency_Fields.BITMASK;
				}
			}
		}

		/// <summary>
		/// Returns the mask indicating which bits in a pixel
		/// contain the specified color/alpha sample.  For color
		/// samples, <code>index</code> corresponds to the placement of color
		/// sample names in the color space.  Thus, an <code>index</code>
		/// equal to 0 for a CMYK ColorSpace would correspond to
		/// Cyan and an <code>index</code> equal to 1 would correspond to
		/// Magenta.  If there is alpha, the alpha <code>index</code> would be:
		/// <pre>
		///      alphaIndex = numComponents() - 1;
		/// </pre> </summary>
		/// <param name="index"> the specified color or alpha sample </param>
		/// <returns> the mask, which indicates which bits of the <code>int</code>
		///         pixel representation contain the color or alpha sample specified
		///         by <code>index</code>. </returns>
		/// <exception cref="ArrayIndexOutOfBoundsException"> if <code>index</code> is
		///         greater than the number of components minus 1 in this
		///         <code>PackedColorModel</code> or if <code>index</code> is
		///         less than zero </exception>
		public int GetMask(int index)
		{
			return MaskArray[index];
		}

		/// <summary>
		/// Returns a mask array indicating which bits in a pixel
		/// contain the color and alpha samples. </summary>
		/// <returns> the mask array , which indicates which bits of the
		///         <code>int</code> pixel
		///         representation contain the color or alpha samples. </returns>
		public int[] Masks
		{
			get
			{
				return (int[]) MaskArray.clone();
			}
		}

		/*
		 * A utility function to compute the mask offset and scalefactor,
		 * store these and the mask in instance arrays, and verify that
		 * the mask fits in the specified pixel size.
		 */
		private void DecomposeMask(int mask, int idx, String componentName)
		{
			int off = 0;
			int count = NBits[idx];

			// Store the mask
			MaskArray[idx] = mask;

			// Now find the shift
			if (mask != 0)
			{
				while ((mask & 1) == 0)
				{
					mask = (int)((uint)mask >> 1);
					off++;
				}
			}

			if (off + count > Pixel_bits)
			{
				throw new IllegalArgumentException(componentName + " mask " + MaskArray[idx].ToString("x") + " overflows pixel (expecting " + Pixel_bits + " bits");
			}

			MaskOffsets[idx] = off;
			if (count == 0)
			{
				// High enough to scale any 0-ff value down to 0.0, but not
				// high enough to get Infinity when scaling back to pixel bits
				ScaleFactors[idx] = 256.0f;
			}
			else
			{
				ScaleFactors[idx] = 255.0f / ((1 << count) - 1);
			}

		}

		/// <summary>
		/// Creates a <code>SampleModel</code> with the specified width and
		/// height that has a data layout compatible with this
		/// <code>ColorModel</code>. </summary>
		/// <param name="w"> the width (in pixels) of the region of the image data
		///          described </param>
		/// <param name="h"> the height (in pixels) of the region of the image data
		///          described </param>
		/// <returns> the newly created <code>SampleModel</code>. </returns>
		/// <exception cref="IllegalArgumentException"> if <code>w</code> or
		///         <code>h</code> is not greater than 0 </exception>
		/// <seealso cref= SampleModel </seealso>
		public override SampleModel CreateCompatibleSampleModel(int w, int h)
		{
			return new SinglePixelPackedSampleModel(TransferType_Renamed, w, h, MaskArray);
		}

		/// <summary>
		/// Checks if the specified <code>SampleModel</code> is compatible
		/// with this <code>ColorModel</code>.  If <code>sm</code> is
		/// <code>null</code>, this method returns <code>false</code>. </summary>
		/// <param name="sm"> the specified <code>SampleModel</code>,
		/// or <code>null</code> </param>
		/// <returns> <code>true</code> if the specified <code>SampleModel</code>
		///         is compatible with this <code>ColorModel</code>;
		///         <code>false</code> otherwise. </returns>
		/// <seealso cref= SampleModel </seealso>
		public override bool IsCompatibleSampleModel(SampleModel sm)
		{
			if (!(sm is SinglePixelPackedSampleModel))
			{
				return false;
			}

			// Must have the same number of components
			if (NumComponents_Renamed != sm.NumBands)
			{
				return false;
			}

			// Transfer type must be the same
			if (sm.TransferType != TransferType_Renamed)
			{
				return false;
			}

			SinglePixelPackedSampleModel sppsm = (SinglePixelPackedSampleModel) sm;
			// Now compare the specific masks
			int[] bitMasks = sppsm.BitMasks;
			if (bitMasks.Length != MaskArray.Length)
			{
				return false;
			}

			/* compare 'effective' masks only, i.e. only part of the mask
			 * which fits the capacity of the transfer type.
			 */
			int maxMask = (int)((1L << DataBuffer.GetDataTypeSize(TransferType_Renamed)) - 1);
			for (int i = 0; i < bitMasks.Length; i++)
			{
				if ((maxMask & bitMasks[i]) != (maxMask & MaskArray[i]))
				{
					return false;
				}
			}

			return true;
		}

		/// <summary>
		/// Returns a <seealso cref="WritableRaster"/> representing the alpha channel of
		/// an image, extracted from the input <code>WritableRaster</code>.
		/// This method assumes that <code>WritableRaster</code> objects
		/// associated with this <code>ColorModel</code> store the alpha band,
		/// if present, as the last band of image data.  Returns <code>null</code>
		/// if there is no separate spatial alpha channel associated with this
		/// <code>ColorModel</code>.  This method creates a new
		/// <code>WritableRaster</code>, but shares the data array. </summary>
		/// <param name="raster"> a <code>WritableRaster</code> containing an image </param>
		/// <returns> a <code>WritableRaster</code> that represents the alpha
		///         channel of the image contained in <code>raster</code>. </returns>
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
		/// Tests if the specified <code>Object</code> is an instance
		/// of <code>PackedColorModel</code> and equals this
		/// <code>PackedColorModel</code>. </summary>
		/// <param name="obj"> the <code>Object</code> to test for equality </param>
		/// <returns> <code>true</code> if the specified <code>Object</code>
		/// is an instance of <code>PackedColorModel</code> and equals this
		/// <code>PackedColorModel</code>; <code>false</code> otherwise. </returns>
		public override bool Equals(Object obj)
		{
			if (!(obj is PackedColorModel))
			{
				return false;
			}

			if (!base.Equals(obj))
			{
				return false;
			}

			PackedColorModel cm = (PackedColorModel) obj;
			int numC = cm.NumComponents;
			if (numC != NumComponents_Renamed)
			{
				return false;
			}
			for (int i = 0; i < numC; i++)
			{
				if (MaskArray[i] != cm.GetMask(i))
				{
					return false;
				}
			}
			return true;
		}

		private static int[] CreateBitsArray(int[] colorMaskArray, int alphaMask)
		{
			int numColors = colorMaskArray.Length;
			int numAlpha = (alphaMask == 0 ? 0 : 1);
			int[] arr = new int[numColors + numAlpha];
			for (int i = 0; i < numColors; i++)
			{
				arr[i] = CountBits(colorMaskArray[i]);
				if (arr[i] < 0)
				{
					throw new IllegalArgumentException("Noncontiguous color mask (" + colorMaskArray[i].ToString("x") + "at index " + i);
				}
			}
			if (alphaMask != 0)
			{
				arr[numColors] = CountBits(alphaMask);
				if (arr[numColors] < 0)
				{
					throw new IllegalArgumentException("Noncontiguous alpha mask (" + alphaMask.ToString("x"));
				}
			}
			return arr;
		}

		private static int[] CreateBitsArray(int rmask, int gmask, int bmask, int amask)
		{
			int[] arr = new int[3 + (amask == 0 ? 0 : 1)];
			arr[0] = CountBits(rmask);
			arr[1] = CountBits(gmask);
			arr[2] = CountBits(bmask);
			if (arr[0] < 0)
			{
				throw new IllegalArgumentException("Noncontiguous red mask (" + rmask.ToString("x"));
			}
			else if (arr[1] < 0)
			{
				throw new IllegalArgumentException("Noncontiguous green mask (" + gmask.ToString("x"));
			}
			else if (arr[2] < 0)
			{
				throw new IllegalArgumentException("Noncontiguous blue mask (" + bmask.ToString("x"));
			}
			if (amask != 0)
			{
				arr[3] = CountBits(amask);
				if (arr[3] < 0)
				{
					throw new IllegalArgumentException("Noncontiguous alpha mask (" + amask.ToString("x"));
				}
			}
			return arr;
		}

		private static int CountBits(int mask)
		{
			int count = 0;
			if (mask != 0)
			{
				while ((mask & 1) == 0)
				{
					mask = (int)((uint)mask >> 1);
				}
				while ((mask & 1) == 1)
				{
					mask = (int)((uint)mask >> 1);
					count++;
				}
			}
			if (mask != 0)
			{
				return -1;
			}
			return count;
		}

	}

}
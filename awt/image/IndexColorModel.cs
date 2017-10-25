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

namespace java.awt.image
{


	/// <summary>
	/// The <code>IndexColorModel</code> class is a <code>ColorModel</code>
	/// class that works with pixel values consisting of a
	/// single sample that is an index into a fixed colormap in the default
	/// sRGB color space.  The colormap specifies red, green, blue, and
	/// optional alpha components corresponding to each index.  All components
	/// are represented in the colormap as 8-bit unsigned integral values.
	/// Some constructors allow the caller to specify "holes" in the colormap
	/// by indicating which colormap entries are valid and which represent
	/// unusable colors via the bits set in a <code>BigInteger</code> object.
	/// This color model is similar to an X11 PseudoColor visual.
	/// <para>
	/// Some constructors provide a means to specify an alpha component
	/// for each pixel in the colormap, while others either provide no
	/// such means or, in some cases, a flag to indicate whether the
	/// colormap data contains alpha values.  If no alpha is supplied to
	/// the constructor, an opaque alpha component (alpha = 1.0) is
	/// assumed for each entry.
	/// An optional transparent pixel value can be supplied that indicates a
	/// pixel to be made completely transparent, regardless of any alpha
	/// component supplied or assumed for that pixel value.
	/// Note that the color components in the colormap of an
	/// <code>IndexColorModel</code> objects are never pre-multiplied with
	/// the alpha components.
	/// </para>
	/// <para>
	/// <a name="transparency">
	/// The transparency of an <code>IndexColorModel</code> object is
	/// determined by examining the alpha components of the colors in the
	/// colormap and choosing the most specific value after considering
	/// the optional alpha values and any transparent index specified.
	/// The transparency value is <code>Transparency.OPAQUE</code>
	/// only if all valid colors in
	/// the colormap are opaque and there is no valid transparent pixel.
	/// If all valid colors
	/// in the colormap are either completely opaque (alpha = 1.0) or
	/// completely transparent (alpha = 0.0), which typically occurs when
	/// a valid transparent pixel is specified,
	/// the value is <code>Transparency.BITMASK</code>.
	/// Otherwise, the value is <code>Transparency.TRANSLUCENT</code>, indicating
	/// that some valid color has an alpha component that is
	/// neither completely transparent nor completely opaque
	/// (0.0 &lt; alpha &lt; 1.0).
	/// </a>
	/// 
	/// </para>
	/// <para>
	/// If an <code>IndexColorModel</code> object has
	/// a transparency value of <code>Transparency.OPAQUE</code>,
	/// then the <code>hasAlpha</code>
	/// and <code>getNumComponents</code> methods
	/// (both inherited from <code>ColorModel</code>)
	/// return false and 3, respectively.
	/// For any other transparency value,
	/// <code>hasAlpha</code> returns true
	/// and <code>getNumComponents</code> returns 4.
	/// 
	/// </para>
	/// <para>
	/// <a name="index_values">
	/// The values used to index into the colormap are taken from the least
	/// significant <em>n</em> bits of pixel representations where
	/// <em>n</em> is based on the pixel size specified in the constructor.
	/// For pixel sizes smaller than 8 bits, <em>n</em> is rounded up to a
	/// power of two (3 becomes 4 and 5,6,7 become 8).
	/// For pixel sizes between 8 and 16 bits, <em>n</em> is equal to the
	/// pixel size.
	/// Pixel sizes larger than 16 bits are not supported by this class.
	/// Higher order bits beyond <em>n</em> are ignored in pixel representations.
	/// Index values greater than or equal to the map size, but less than
	/// 2<sup><em>n</em></sup>, are undefined and return 0 for all color and
	/// alpha components.
	/// </a>
	/// </para>
	/// <para>
	/// For those methods that use a primitive array pixel representation of
	/// type <code>transferType</code>, the array length is always one.
	/// The transfer types supported are <code>DataBuffer.TYPE_BYTE</code> and
	/// <code>DataBuffer.TYPE_USHORT</code>.  A single int pixel
	/// representation is valid for all objects of this class, since it is
	/// always possible to represent pixel values used with this class in a
	/// single int.  Therefore, methods that use this representation do
	/// not throw an <code>IllegalArgumentException</code> due to an invalid
	/// pixel value.
	/// </para>
	/// <para>
	/// Many of the methods in this class are final.  The reason for
	/// this is that the underlying native graphics code makes assumptions
	/// about the layout and operation of this class and those assumptions
	/// are reflected in the implementations of the methods here that are
	/// marked final.  You can subclass this class for other reasons, but
	/// you cannot override or modify the behaviour of those methods.
	/// 
	/// </para>
	/// </summary>
	/// <seealso cref= ColorModel </seealso>
	/// <seealso cref= ColorSpace </seealso>
	/// <seealso cref= DataBuffer
	///  </seealso>
	public class IndexColorModel : ColorModel
	{
		private int[] Rgb;
		private int Map_size;
		private int Pixel_mask;
		private int Transparent_index = -1;
		private bool Allgrayopaque;
		private System.Numerics.BigInteger ValidBits;

		private sun.awt.image.BufImgSurfaceData.ICMColorData ColorData = null;

		private static int[] OpaqueBits = new int[] {8, 8, 8};
		private static int[] AlphaBits = new int[] {8, 8, 8, 8};

//JAVA TO C# CONVERTER TODO TASK: Replace 'unknown' with the appropriate dll name:
		[DllImport("unknown")]
		private static extern void initIDs();
		static IndexColorModel()
		{
			ColorModel.LoadLibraries();
			initIDs();
		}
		/// <summary>
		/// Constructs an <code>IndexColorModel</code> from the specified
		/// arrays of red, green, and blue components.  Pixels described
		/// by this color model all have alpha components of 255
		/// unnormalized (1.0&nbsp;normalized), which means they
		/// are fully opaque.  All of the arrays specifying the color
		/// components must have at least the specified number of entries.
		/// The <code>ColorSpace</code> is the default sRGB space.
		/// Since there is no alpha information in any of the arguments
		/// to this constructor, the transparency value is always
		/// <code>Transparency.OPAQUE</code>.
		/// The transfer type is the smallest of <code>DataBuffer.TYPE_BYTE</code>
		/// or <code>DataBuffer.TYPE_USHORT</code> that can hold a single pixel. </summary>
		/// <param name="bits">      the number of bits each pixel occupies </param>
		/// <param name="size">      the size of the color component arrays </param>
		/// <param name="r">         the array of red color components </param>
		/// <param name="g">         the array of green color components </param>
		/// <param name="b">         the array of blue color components </param>
		/// <exception cref="IllegalArgumentException"> if <code>bits</code> is less
		///         than 1 or greater than 16 </exception>
		/// <exception cref="IllegalArgumentException"> if <code>size</code> is less
		///         than 1 </exception>
		public IndexColorModel(int bits, int size, sbyte[] r, sbyte[] g, sbyte[] b) : base(bits, OpaqueBits, ColorSpace.GetInstance(ColorSpace.CS_sRGB_Renamed), false, false, java.awt.Transparency_Fields.OPAQUE, ColorModel.GetDefaultTransferType(bits))
		{
			if (bits < 1 || bits > 16)
			{
				throw new IllegalArgumentException("Number of bits must be between" + " 1 and 16.");
			}
			SetRGBs(size, r, g, b, null);
			CalculatePixelMask();
		}

		/// <summary>
		/// Constructs an <code>IndexColorModel</code> from the given arrays
		/// of red, green, and blue components.  Pixels described by this color
		/// model all have alpha components of 255 unnormalized
		/// (1.0&nbsp;normalized), which means they are fully opaque, except
		/// for the indicated pixel to be made transparent.  All of the arrays
		/// specifying the color components must have at least the specified
		/// number of entries.
		/// The <code>ColorSpace</code> is the default sRGB space.
		/// The transparency value may be <code>Transparency.OPAQUE</code> or
		/// <code>Transparency.BITMASK</code> depending on the arguments, as
		/// specified in the <a href="#transparency">class description</a> above.
		/// The transfer type is the smallest of <code>DataBuffer.TYPE_BYTE</code>
		/// or <code>DataBuffer.TYPE_USHORT</code> that can hold a
		/// single pixel. </summary>
		/// <param name="bits">      the number of bits each pixel occupies </param>
		/// <param name="size">      the size of the color component arrays </param>
		/// <param name="r">         the array of red color components </param>
		/// <param name="g">         the array of green color components </param>
		/// <param name="b">         the array of blue color components </param>
		/// <param name="trans">     the index of the transparent pixel </param>
		/// <exception cref="IllegalArgumentException"> if <code>bits</code> is less than
		///          1 or greater than 16 </exception>
		/// <exception cref="IllegalArgumentException"> if <code>size</code> is less than
		///          1 </exception>
		public IndexColorModel(int bits, int size, sbyte[] r, sbyte[] g, sbyte[] b, int trans) : base(bits, OpaqueBits, ColorSpace.GetInstance(ColorSpace.CS_sRGB_Renamed), false, false, java.awt.Transparency_Fields.OPAQUE, ColorModel.GetDefaultTransferType(bits))
		{
			if (bits < 1 || bits > 16)
			{
				throw new IllegalArgumentException("Number of bits must be between" + " 1 and 16.");
			}
			SetRGBs(size, r, g, b, null);
			TransparentPixel = trans;
			CalculatePixelMask();
		}

		/// <summary>
		/// Constructs an <code>IndexColorModel</code> from the given
		/// arrays of red, green, blue and alpha components.  All of the
		/// arrays specifying the components must have at least the specified
		/// number of entries.
		/// The <code>ColorSpace</code> is the default sRGB space.
		/// The transparency value may be any of <code>Transparency.OPAQUE</code>,
		/// <code>Transparency.BITMASK</code>,
		/// or <code>Transparency.TRANSLUCENT</code>
		/// depending on the arguments, as specified
		/// in the <a href="#transparency">class description</a> above.
		/// The transfer type is the smallest of <code>DataBuffer.TYPE_BYTE</code>
		/// or <code>DataBuffer.TYPE_USHORT</code> that can hold a single pixel. </summary>
		/// <param name="bits">      the number of bits each pixel occupies </param>
		/// <param name="size">      the size of the color component arrays </param>
		/// <param name="r">         the array of red color components </param>
		/// <param name="g">         the array of green color components </param>
		/// <param name="b">         the array of blue color components </param>
		/// <param name="a">         the array of alpha value components </param>
		/// <exception cref="IllegalArgumentException"> if <code>bits</code> is less
		///           than 1 or greater than 16 </exception>
		/// <exception cref="IllegalArgumentException"> if <code>size</code> is less
		///           than 1 </exception>
		public IndexColorModel(int bits, int size, sbyte[] r, sbyte[] g, sbyte[] b, sbyte[] a) : base(bits, AlphaBits, ColorSpace.GetInstance(ColorSpace.CS_sRGB_Renamed), true, false, java.awt.Transparency_Fields.TRANSLUCENT, ColorModel.GetDefaultTransferType(bits))
		{
			if (bits < 1 || bits > 16)
			{
				throw new IllegalArgumentException("Number of bits must be between" + " 1 and 16.");
			}
			SetRGBs(size, r, g, b, a);
			CalculatePixelMask();
		}

		/// <summary>
		/// Constructs an <code>IndexColorModel</code> from a single
		/// array of interleaved red, green, blue and optional alpha
		/// components.  The array must have enough values in it to
		/// fill all of the needed component arrays of the specified
		/// size.  The <code>ColorSpace</code> is the default sRGB space.
		/// The transparency value may be any of <code>Transparency.OPAQUE</code>,
		/// <code>Transparency.BITMASK</code>,
		/// or <code>Transparency.TRANSLUCENT</code>
		/// depending on the arguments, as specified
		/// in the <a href="#transparency">class description</a> above.
		/// The transfer type is the smallest of
		/// <code>DataBuffer.TYPE_BYTE</code> or <code>DataBuffer.TYPE_USHORT</code>
		/// that can hold a single pixel.
		/// </summary>
		/// <param name="bits">      the number of bits each pixel occupies </param>
		/// <param name="size">      the size of the color component arrays </param>
		/// <param name="cmap">      the array of color components </param>
		/// <param name="start">     the starting offset of the first color component </param>
		/// <param name="hasalpha">  indicates whether alpha values are contained in
		///                  the <code>cmap</code> array </param>
		/// <exception cref="IllegalArgumentException"> if <code>bits</code> is less
		///           than 1 or greater than 16 </exception>
		/// <exception cref="IllegalArgumentException"> if <code>size</code> is less
		///           than 1 </exception>
		public IndexColorModel(int bits, int size, sbyte[] cmap, int start, bool hasalpha) : this(bits, size, cmap, start, hasalpha, -1)
		{
			if (bits < 1 || bits > 16)
			{
				throw new IllegalArgumentException("Number of bits must be between" + " 1 and 16.");
			}
		}

		/// <summary>
		/// Constructs an <code>IndexColorModel</code> from a single array of
		/// interleaved red, green, blue and optional alpha components.  The
		/// specified transparent index represents a pixel that is made
		/// entirely transparent regardless of any alpha value specified
		/// for it.  The array must have enough values in it to fill all
		/// of the needed component arrays of the specified size.
		/// The <code>ColorSpace</code> is the default sRGB space.
		/// The transparency value may be any of <code>Transparency.OPAQUE</code>,
		/// <code>Transparency.BITMASK</code>,
		/// or <code>Transparency.TRANSLUCENT</code>
		/// depending on the arguments, as specified
		/// in the <a href="#transparency">class description</a> above.
		/// The transfer type is the smallest of
		/// <code>DataBuffer.TYPE_BYTE</code> or <code>DataBuffer.TYPE_USHORT</code>
		/// that can hold a single pixel. </summary>
		/// <param name="bits">      the number of bits each pixel occupies </param>
		/// <param name="size">      the size of the color component arrays </param>
		/// <param name="cmap">      the array of color components </param>
		/// <param name="start">     the starting offset of the first color component </param>
		/// <param name="hasalpha">  indicates whether alpha values are contained in
		///                  the <code>cmap</code> array </param>
		/// <param name="trans">     the index of the fully transparent pixel </param>
		/// <exception cref="IllegalArgumentException"> if <code>bits</code> is less than
		///               1 or greater than 16 </exception>
		/// <exception cref="IllegalArgumentException"> if <code>size</code> is less than
		///               1 </exception>
		public IndexColorModel(int bits, int size, sbyte[] cmap, int start, bool hasalpha, int trans) : base(bits, OpaqueBits, ColorSpace.GetInstance(ColorSpace.CS_sRGB_Renamed), false, false, java.awt.Transparency_Fields.OPAQUE, ColorModel.GetDefaultTransferType(bits))
		{
			// REMIND: This assumes the ordering: RGB[A]

			if (bits < 1 || bits > 16)
			{
				throw new IllegalArgumentException("Number of bits must be between" + " 1 and 16.");
			}
			if (size < 1)
			{
				throw new IllegalArgumentException("Map size (" + size + ") must be >= 1");
			}
			Map_size = size;
			Rgb = new int[CalcRealMapSize(bits, size)];
			int j = start;
			int alpha = 0xff;
			bool allgray = true;
			int transparency = java.awt.Transparency_Fields.OPAQUE;
			for (int i = 0; i < size; i++)
			{
				int r = cmap[j++] & 0xff;
				int g = cmap[j++] & 0xff;
				int b = cmap[j++] & 0xff;
				allgray = allgray && (r == g) && (g == b);
				if (hasalpha)
				{
					alpha = cmap[j++] & 0xff;
					if (alpha != 0xff)
					{
						if (alpha == 0x00)
						{
							if (transparency == java.awt.Transparency_Fields.OPAQUE)
							{
								transparency = java.awt.Transparency_Fields.BITMASK;
							}
							if (Transparent_index < 0)
							{
								Transparent_index = i;
							}
						}
						else
						{
							transparency = java.awt.Transparency_Fields.TRANSLUCENT;
						}
						allgray = false;
					}
				}
				Rgb[i] = (alpha << 24) | (r << 16) | (g << 8) | b;
			}
			this.Allgrayopaque = allgray;
			Transparency = transparency;
			TransparentPixel = trans;
			CalculatePixelMask();
		}

		/// <summary>
		/// Constructs an <code>IndexColorModel</code> from an array of
		/// ints where each int is comprised of red, green, blue, and
		/// optional alpha components in the default RGB color model format.
		/// The specified transparent index represents a pixel that is made
		/// entirely transparent regardless of any alpha value specified
		/// for it.  The array must have enough values in it to fill all
		/// of the needed component arrays of the specified size.
		/// The <code>ColorSpace</code> is the default sRGB space.
		/// The transparency value may be any of <code>Transparency.OPAQUE</code>,
		/// <code>Transparency.BITMASK</code>,
		/// or <code>Transparency.TRANSLUCENT</code>
		/// depending on the arguments, as specified
		/// in the <a href="#transparency">class description</a> above. </summary>
		/// <param name="bits">      the number of bits each pixel occupies </param>
		/// <param name="size">      the size of the color component arrays </param>
		/// <param name="cmap">      the array of color components </param>
		/// <param name="start">     the starting offset of the first color component </param>
		/// <param name="hasalpha">  indicates whether alpha values are contained in
		///                  the <code>cmap</code> array </param>
		/// <param name="trans">     the index of the fully transparent pixel </param>
		/// <param name="transferType"> the data type of the array used to represent
		///           pixel values.  The data type must be either
		///           <code>DataBuffer.TYPE_BYTE</code> or
		///           <code>DataBuffer.TYPE_USHORT</code>. </param>
		/// <exception cref="IllegalArgumentException"> if <code>bits</code> is less
		///           than 1 or greater than 16 </exception>
		/// <exception cref="IllegalArgumentException"> if <code>size</code> is less
		///           than 1 </exception>
		/// <exception cref="IllegalArgumentException"> if <code>transferType</code> is not
		///           one of <code>DataBuffer.TYPE_BYTE</code> or
		///           <code>DataBuffer.TYPE_USHORT</code> </exception>
		public IndexColorModel(int bits, int size, int[] cmap, int start, bool hasalpha, int trans, int transferType) : base(bits, OpaqueBits, ColorSpace.GetInstance(ColorSpace.CS_sRGB_Renamed), false, false, java.awt.Transparency_Fields.OPAQUE, transferType)
		{
			// REMIND: This assumes the ordering: RGB[A]

			if (bits < 1 || bits > 16)
			{
				throw new IllegalArgumentException("Number of bits must be between" + " 1 and 16.");
			}
			if (size < 1)
			{
				throw new IllegalArgumentException("Map size (" + size + ") must be >= 1");
			}
			if ((transferType != DataBuffer.TYPE_BYTE) && (transferType != DataBuffer.TYPE_USHORT))
			{
				throw new IllegalArgumentException("transferType must be either" + "DataBuffer.TYPE_BYTE or DataBuffer.TYPE_USHORT");
			}

			SetRGBs(size, cmap, start, hasalpha);
			TransparentPixel = trans;
			CalculatePixelMask();
		}

		/// <summary>
		/// Constructs an <code>IndexColorModel</code> from an
		/// <code>int</code> array where each <code>int</code> is
		/// comprised of red, green, blue, and alpha
		/// components in the default RGB color model format.
		/// The array must have enough values in it to fill all
		/// of the needed component arrays of the specified size.
		/// The <code>ColorSpace</code> is the default sRGB space.
		/// The transparency value may be any of <code>Transparency.OPAQUE</code>,
		/// <code>Transparency.BITMASK</code>,
		/// or <code>Transparency.TRANSLUCENT</code>
		/// depending on the arguments, as specified
		/// in the <a href="#transparency">class description</a> above.
		/// The transfer type must be one of <code>DataBuffer.TYPE_BYTE</code>
		/// <code>DataBuffer.TYPE_USHORT</code>.
		/// The <code>BigInteger</code> object specifies the valid/invalid pixels
		/// in the <code>cmap</code> array.  A pixel is valid if the
		/// <code>BigInteger</code> value at that index is set, and is invalid
		/// if the <code>BigInteger</code> bit  at that index is not set. </summary>
		/// <param name="bits"> the number of bits each pixel occupies </param>
		/// <param name="size"> the size of the color component array </param>
		/// <param name="cmap"> the array of color components </param>
		/// <param name="start"> the starting offset of the first color component </param>
		/// <param name="transferType"> the specified data type </param>
		/// <param name="validBits"> a <code>BigInteger</code> object.  If a bit is
		///    set in the BigInteger, the pixel at that index is valid.
		///    If a bit is not set, the pixel at that index
		///    is considered invalid.  If null, all pixels are valid.
		///    Only bits from 0 to the map size are considered. </param>
		/// <exception cref="IllegalArgumentException"> if <code>bits</code> is less
		///           than 1 or greater than 16 </exception>
		/// <exception cref="IllegalArgumentException"> if <code>size</code> is less
		///           than 1 </exception>
		/// <exception cref="IllegalArgumentException"> if <code>transferType</code> is not
		///           one of <code>DataBuffer.TYPE_BYTE</code> or
		///           <code>DataBuffer.TYPE_USHORT</code>
		/// 
		/// @since 1.3 </exception>
		public IndexColorModel(int bits, int size, int[] cmap, int start, int transferType, System.Numerics.BigInteger validBits) : base(bits, AlphaBits, ColorSpace.GetInstance(ColorSpace.CS_sRGB_Renamed), true, false, java.awt.Transparency_Fields.TRANSLUCENT, transferType)
		{

			if (bits < 1 || bits > 16)
			{
				throw new IllegalArgumentException("Number of bits must be between" + " 1 and 16.");
			}
			if (size < 1)
			{
				throw new IllegalArgumentException("Map size (" + size + ") must be >= 1");
			}
			if ((transferType != DataBuffer.TYPE_BYTE) && (transferType != DataBuffer.TYPE_USHORT))
			{
				throw new IllegalArgumentException("transferType must be either" + "DataBuffer.TYPE_BYTE or DataBuffer.TYPE_USHORT");
			}

			if (validBits != null)
			{
				// Check to see if it is all valid
				for (int i = 0; i < size; i++)
				{
					if (!validBits.testBit(i))
					{
						this.ValidBits = validBits;
						break;
					}
				}
			}

			SetRGBs(size, cmap, start, true);
			CalculatePixelMask();
		}

		private void SetRGBs(int size, sbyte[] r, sbyte[] g, sbyte[] b, sbyte[] a)
		{
			if (size < 1)
			{
				throw new IllegalArgumentException("Map size (" + size + ") must be >= 1");
			}
			Map_size = size;
			Rgb = new int[CalcRealMapSize(Pixel_bits, size)];
			int alpha = 0xff;
			int transparency = java.awt.Transparency_Fields.OPAQUE;
			bool allgray = true;
			for (int i = 0; i < size; i++)
			{
				int rc = r[i] & 0xff;
				int gc = g[i] & 0xff;
				int bc = b[i] & 0xff;
				allgray = allgray && (rc == gc) && (gc == bc);
				if (a != null)
				{
					alpha = a[i] & 0xff;
					if (alpha != 0xff)
					{
						if (alpha == 0x00)
						{
							if (transparency == java.awt.Transparency_Fields.OPAQUE)
							{
								transparency = java.awt.Transparency_Fields.BITMASK;
							}
							if (Transparent_index < 0)
							{
								Transparent_index = i;
							}
						}
						else
						{
							transparency = java.awt.Transparency_Fields.TRANSLUCENT;
						}
						allgray = false;
					}
				}
				Rgb[i] = (alpha << 24) | (rc << 16) | (gc << 8) | bc;
			}
			this.Allgrayopaque = allgray;
			Transparency = transparency;
		}

		private void SetRGBs(int size, int[] cmap, int start, bool hasalpha)
		{
			Map_size = size;
			Rgb = new int[CalcRealMapSize(Pixel_bits, size)];
			int j = start;
			int transparency = java.awt.Transparency_Fields.OPAQUE;
			bool allgray = true;
			System.Numerics.BigInteger validBits = this.ValidBits;
			for (int i = 0; i < size; i++, j++)
			{
				if (validBits != null && !validBits.testBit(i))
				{
					continue;
				}
				int cmaprgb = cmap[j];
				int r = (cmaprgb >> 16) & 0xff;
				int g = (cmaprgb >> 8) & 0xff;
				int b = (cmaprgb) & 0xff;
				allgray = allgray && (r == g) && (g == b);
				if (hasalpha)
				{
					int alpha = (int)((uint)cmaprgb >> 24);
					if (alpha != 0xff)
					{
						if (alpha == 0x00)
						{
							if (transparency == java.awt.Transparency_Fields.OPAQUE)
							{
								transparency = java.awt.Transparency_Fields.BITMASK;
							}
							if (Transparent_index < 0)
							{
								Transparent_index = i;
							}
						}
						else
						{
							transparency = java.awt.Transparency_Fields.TRANSLUCENT;
						}
						allgray = false;
					}
				}
				else
				{
					cmaprgb |= unchecked((int)0xff000000);
				}
				Rgb[i] = cmaprgb;
			}
			this.Allgrayopaque = allgray;
			Transparency = transparency;
		}

		private int CalcRealMapSize(int bits, int size)
		{
			int newSize = System.Math.Max(1 << bits, size);
			return System.Math.Max(newSize, 256);
		}

		private System.Numerics.BigInteger AllValid
		{
			get
			{
				int numbytes = (Map_size+7) / 8;
				sbyte[] valid = new sbyte[numbytes];
				Arrays.Fill(valid, unchecked((sbyte)0xff));
				valid[0] = (sbyte)((int)((uint)0xff >> (numbytes * 8 - Map_size)));
    
				return new System.Numerics.BigInteger(1, valid);
			}
		}

		/// <summary>
		/// Returns the transparency.  Returns either OPAQUE, BITMASK,
		/// or TRANSLUCENT </summary>
		/// <returns> the transparency of this <code>IndexColorModel</code> </returns>
		/// <seealso cref= Transparency#OPAQUE </seealso>
		/// <seealso cref= Transparency#BITMASK </seealso>
		/// <seealso cref= Transparency#TRANSLUCENT </seealso>
		public override int Transparency
		{
			get
			{
				return Transparency_Renamed;
			}
			set
			{
				if (this.Transparency_Renamed != value)
				{
					this.Transparency_Renamed = value;
					if (value == java.awt.Transparency_Fields.OPAQUE)
					{
						SupportsAlpha = false;
						NumComponents_Renamed = 3;
						NBits = OpaqueBits;
					}
					else
					{
						SupportsAlpha = true;
						NumComponents_Renamed = 4;
						NBits = AlphaBits;
					}
				}
			}
		}

		/// <summary>
		/// Returns an array of the number of bits for each color/alpha component.
		/// The array contains the color components in the order red, green,
		/// blue, followed by the alpha component, if present. </summary>
		/// <returns> an array containing the number of bits of each color
		///         and alpha component of this <code>IndexColorModel</code> </returns>
		public override int[] ComponentSize
		{
			get
			{
				if (NBits == null)
				{
					if (SupportsAlpha)
					{
						NBits = new int[4];
						NBits[3] = 8;
					}
					else
					{
						NBits = new int[3];
					}
					NBits[0] = NBits[1] = NBits[2] = 8;
				}
				return NBits.clone();
			}
		}

		/// <summary>
		/// Returns the size of the color/alpha component arrays in this
		/// <code>IndexColorModel</code>. </summary>
		/// <returns> the size of the color and alpha component arrays. </returns>
		public int MapSize
		{
			get
			{
				return Map_size;
			}
		}

		/// <summary>
		/// Returns the index of a transparent pixel in this
		/// <code>IndexColorModel</code> or -1 if there is no pixel
		/// with an alpha value of 0.  If a transparent pixel was
		/// explicitly specified in one of the constructors by its
		/// index, then that index will be preferred, otherwise,
		/// the index of any pixel which happens to be fully transparent
		/// may be returned. </summary>
		/// <returns> the index of a transparent pixel in this
		///         <code>IndexColorModel</code> object, or -1 if there
		///         is no such pixel </returns>
		public int TransparentPixel
		{
			get
			{
				return Transparent_index;
			}
			set
			{
				if (value >= 0 && value < Map_size)
				{
					Rgb[value] &= 0x00ffffff;
					Transparent_index = value;
					Allgrayopaque = false;
					if (this.Transparency_Renamed == java.awt.Transparency_Fields.OPAQUE)
					{
						Transparency = java.awt.Transparency_Fields.BITMASK;
					}
				}
			}
		}

		/// <summary>
		/// Copies the array of red color components into the specified array.
		/// Only the initial entries of the array as specified by
		/// <seealso cref="#getMapSize() getMapSize"/> are written. </summary>
		/// <param name="r"> the specified array into which the elements of the
		///      array of red color components are copied </param>
		public void GetReds(sbyte[] r)
		{
			for (int i = 0; i < Map_size; i++)
			{
				r[i] = (sbyte)(Rgb[i] >> 16);
			}
		}

		/// <summary>
		/// Copies the array of green color components into the specified array.
		/// Only the initial entries of the array as specified by
		/// <code>getMapSize</code> are written. </summary>
		/// <param name="g"> the specified array into which the elements of the
		///      array of green color components are copied </param>
		public void GetGreens(sbyte[] g)
		{
			for (int i = 0; i < Map_size; i++)
			{
				g[i] = (sbyte)(Rgb[i] >> 8);
			}
		}

		/// <summary>
		/// Copies the array of blue color components into the specified array.
		/// Only the initial entries of the array as specified by
		/// <code>getMapSize</code> are written. </summary>
		/// <param name="b"> the specified array into which the elements of the
		///      array of blue color components are copied </param>
		public void GetBlues(sbyte[] b)
		{
			for (int i = 0; i < Map_size; i++)
			{
				b[i] = (sbyte) Rgb[i];
			}
		}

		/// <summary>
		/// Copies the array of alpha transparency components into the
		/// specified array.  Only the initial entries of the array as specified
		/// by <code>getMapSize</code> are written. </summary>
		/// <param name="a"> the specified array into which the elements of the
		///      array of alpha components are copied </param>
		public void GetAlphas(sbyte[] a)
		{
			for (int i = 0; i < Map_size; i++)
			{
				a[i] = (sbyte)(Rgb[i] >> 24);
			}
		}

		/// <summary>
		/// Converts data for each index from the color and alpha component
		/// arrays to an int in the default RGB ColorModel format and copies
		/// the resulting 32-bit ARGB values into the specified array.  Only
		/// the initial entries of the array as specified by
		/// <code>getMapSize</code> are
		/// written. </summary>
		/// <param name="rgb"> the specified array into which the converted ARGB
		///        values from this array of color and alpha components
		///        are copied. </param>
		public void GetRGBs(int[] rgb)
		{
			System.Array.Copy(this.Rgb, 0, rgb, 0, Map_size);
		}



		/// <summary>
		/// This method is called from the constructors to set the pixel_mask
		/// value, which is based on the value of pixel_bits.  The pixel_mask
		/// value is used to mask off the pixel parameters for methods such
		/// as getRed(), getGreen(), getBlue(), getAlpha(), and getRGB().
		/// </summary>
		private void CalculatePixelMask()
		{
			// Note that we adjust the mask so that our masking behavior here
			// is consistent with that of our native rendering loops.
			int maskbits = Pixel_bits;
			if (maskbits == 3)
			{
				maskbits = 4;
			}
			else if (maskbits > 4 && maskbits < 8)
			{
				maskbits = 8;
			}
			Pixel_mask = (1 << maskbits) - 1;
		}

		/// <summary>
		/// Returns the red color component for the specified pixel, scaled
		/// from 0 to 255 in the default RGB ColorSpace, sRGB.  The pixel value
		/// is specified as an int.
		/// Only the lower <em>n</em> bits of the pixel value, as specified in the
		/// <a href="#index_values">class description</a> above, are used to
		/// calculate the returned value.
		/// The returned value is a non pre-multiplied value. </summary>
		/// <param name="pixel"> the specified pixel </param>
		/// <returns> the value of the red color component for the specified pixel </returns>
		public sealed override int GetRed(int pixel)
		{
			return (Rgb[pixel & Pixel_mask] >> 16) & 0xff;
		}

		/// <summary>
		/// Returns the green color component for the specified pixel, scaled
		/// from 0 to 255 in the default RGB ColorSpace, sRGB.  The pixel value
		/// is specified as an int.
		/// Only the lower <em>n</em> bits of the pixel value, as specified in the
		/// <a href="#index_values">class description</a> above, are used to
		/// calculate the returned value.
		/// The returned value is a non pre-multiplied value. </summary>
		/// <param name="pixel"> the specified pixel </param>
		/// <returns> the value of the green color component for the specified pixel </returns>
		public sealed override int GetGreen(int pixel)
		{
			return (Rgb[pixel & Pixel_mask] >> 8) & 0xff;
		}

		/// <summary>
		/// Returns the blue color component for the specified pixel, scaled
		/// from 0 to 255 in the default RGB ColorSpace, sRGB.  The pixel value
		/// is specified as an int.
		/// Only the lower <em>n</em> bits of the pixel value, as specified in the
		/// <a href="#index_values">class description</a> above, are used to
		/// calculate the returned value.
		/// The returned value is a non pre-multiplied value. </summary>
		/// <param name="pixel"> the specified pixel </param>
		/// <returns> the value of the blue color component for the specified pixel </returns>
		public sealed override int GetBlue(int pixel)
		{
			return Rgb[pixel & Pixel_mask] & 0xff;
		}

		/// <summary>
		/// Returns the alpha component for the specified pixel, scaled
		/// from 0 to 255.  The pixel value is specified as an int.
		/// Only the lower <em>n</em> bits of the pixel value, as specified in the
		/// <a href="#index_values">class description</a> above, are used to
		/// calculate the returned value. </summary>
		/// <param name="pixel"> the specified pixel </param>
		/// <returns> the value of the alpha component for the specified pixel </returns>
		public sealed override int GetAlpha(int pixel)
		{
			return (Rgb[pixel & Pixel_mask] >> 24) & 0xff;
		}

		/// <summary>
		/// Returns the color/alpha components of the pixel in the default
		/// RGB color model format.  The pixel value is specified as an int.
		/// Only the lower <em>n</em> bits of the pixel value, as specified in the
		/// <a href="#index_values">class description</a> above, are used to
		/// calculate the returned value.
		/// The returned value is in a non pre-multiplied format. </summary>
		/// <param name="pixel"> the specified pixel </param>
		/// <returns> the color and alpha components of the specified pixel </returns>
		/// <seealso cref= ColorModel#getRGBdefault </seealso>
		public sealed override int GetRGB(int pixel)
		{
			return Rgb[pixel & Pixel_mask];
		}

		private const int CACHESIZE = 40;
		private int[] Lookupcache = new int[CACHESIZE];

		/// <summary>
		/// Returns a data element array representation of a pixel in this
		/// ColorModel, given an integer pixel representation in the
		/// default RGB color model.  This array can then be passed to the
		/// <seealso cref="WritableRaster#setDataElements(int, int, java.lang.Object) setDataElements"/>
		/// method of a <seealso cref="WritableRaster"/> object.  If the pixel variable is
		/// <code>null</code>, a new array is allocated.  If <code>pixel</code>
		/// is not <code>null</code>, it must be
		/// a primitive array of type <code>transferType</code>; otherwise, a
		/// <code>ClassCastException</code> is thrown.  An
		/// <code>ArrayIndexOutOfBoundsException</code> is
		/// thrown if <code>pixel</code> is not large enough to hold a pixel
		/// value for this <code>ColorModel</code>.  The pixel array is returned.
		/// <para>
		/// Since <code>IndexColorModel</code> can be subclassed, subclasses
		/// inherit the implementation of this method and if they don't
		/// override it then they throw an exception if they use an
		/// unsupported <code>transferType</code>.
		/// 
		/// </para>
		/// </summary>
		/// <param name="rgb"> the integer pixel representation in the default RGB
		/// color model </param>
		/// <param name="pixel"> the specified pixel </param>
		/// <returns> an array representation of the specified pixel in this
		///  <code>IndexColorModel</code>. </returns>
		/// <exception cref="ClassCastException"> if <code>pixel</code>
		///  is not a primitive array of type <code>transferType</code> </exception>
		/// <exception cref="ArrayIndexOutOfBoundsException"> if
		///  <code>pixel</code> is not large enough to hold a pixel value
		///  for this <code>ColorModel</code> </exception>
		/// <exception cref="UnsupportedOperationException"> if <code>transferType</code>
		///         is invalid </exception>
		/// <seealso cref= WritableRaster#setDataElements </seealso>
		/// <seealso cref= SampleModel#setDataElements </seealso>
		public override Object GetDataElements(int rgb, Object pixel)
		{
			lock (this)
			{
				int red = (rgb >> 16) & 0xff;
				int green = (rgb >> 8) & 0xff;
				int blue = rgb & 0xff;
				int alpha = ((int)((uint)rgb >> 24));
				int pix = 0;
        
				// Note that pixels are stored at lookupcache[2*i]
				// and the rgb that was searched is stored at
				// lookupcache[2*i+1].  Also, the pixel is first
				// inverted using the unary complement operator
				// before storing in the cache so it can never be 0.
				for (int i = CACHESIZE - 2; i >= 0; i -= 2)
				{
					if ((pix = Lookupcache[i]) == 0)
					{
						break;
					}
					if (rgb == Lookupcache[i + 1])
					{
						return Installpixel(pixel, ~pix);
					}
				}
        
				if (Allgrayopaque)
				{
					// IndexColorModel objects are all tagged as
					// non-premultiplied so ignore the alpha value
					// of the incoming color, convert the
					// non-premultiplied color components to a
					// grayscale value and search for the closest
					// gray value in the palette.  Since all colors
					// in the palette are gray, we only need compare
					// to one of the color components for a match
					// using a simple linear distance formula.
        
					int minDist = 256;
					int d;
					int gray = (int)(red * 77 + green * 150 + blue * 29 + 128) / 256;
        
					for (int i = 0; i < Map_size; i++)
					{
						if (this.Rgb[i] == 0x0)
						{
							// For allgrayopaque colormaps, entries are 0
							// iff they are an invalid color and should be
							// ignored during color searches.
							continue;
						}
						d = (this.Rgb[i] & 0xff) - gray;
						if (d < 0)
						{
							d = -d;
						}
						if (d < minDist)
						{
							pix = i;
							if (d == 0)
							{
								break;
							}
							minDist = d;
						}
					}
				}
				else if (Transparency_Renamed == java.awt.Transparency_Fields.OPAQUE)
				{
					// IndexColorModel objects are all tagged as
					// non-premultiplied so ignore the alpha value
					// of the incoming color and search for closest
					// color match independently using a 3 component
					// Euclidean distance formula.
					// For opaque colormaps, palette entries are 0
					// iff they are an invalid color and should be
					// ignored during color searches.
					// As an optimization, exact color searches are
					// likely to be fairly common in opaque colormaps
					// so first we will do a quick search for an
					// exact match.
        
					int smallestError = Integer.MaxValue;
					int[] lut = this.Rgb;
					int lutrgb;
					for (int i = 0; i < Map_size; i++)
					{
						lutrgb = lut[i];
						if (lutrgb == rgb && lutrgb != 0)
						{
							pix = i;
							smallestError = 0;
							break;
						}
					}
        
					if (smallestError != 0)
					{
						for (int i = 0; i < Map_size; i++)
						{
							lutrgb = lut[i];
							if (lutrgb == 0)
							{
								continue;
							}
        
							int tmp = ((lutrgb >> 16) & 0xff) - red;
							int currentError = tmp * tmp;
							if (currentError < smallestError)
							{
								tmp = ((lutrgb >> 8) & 0xff) - green;
								currentError += tmp * tmp;
								if (currentError < smallestError)
								{
									tmp = (lutrgb & 0xff) - blue;
									currentError += tmp * tmp;
									if (currentError < smallestError)
									{
										pix = i;
										smallestError = currentError;
									}
								}
							}
						}
					}
				}
				else if (alpha == 0 && Transparent_index >= 0)
				{
					// Special case - transparent color maps to the
					// specified transparent pixel, if there is one
        
					pix = Transparent_index;
				}
				else
				{
					// IndexColorModel objects are all tagged as
					// non-premultiplied so use non-premultiplied
					// color components in the distance calculations.
					// Look for closest match using a 4 component
					// Euclidean distance formula.
        
					int smallestError = Integer.MaxValue;
					int[] lut = this.Rgb;
					for (int i = 0; i < Map_size; i++)
					{
						int lutrgb = lut[i];
						if (lutrgb == rgb)
						{
							if (ValidBits != null && !ValidBits.testBit(i))
							{
								continue;
							}
							pix = i;
							break;
						}
        
						int tmp = ((lutrgb >> 16) & 0xff) - red;
						int currentError = tmp * tmp;
						if (currentError < smallestError)
						{
							tmp = ((lutrgb >> 8) & 0xff) - green;
							currentError += tmp * tmp;
							if (currentError < smallestError)
							{
								tmp = (lutrgb & 0xff) - blue;
								currentError += tmp * tmp;
								if (currentError < smallestError)
								{
									tmp = ((int)((uint)lutrgb >> 24)) - alpha;
									currentError += tmp * tmp;
									if (currentError < smallestError && (ValidBits == null || ValidBits.testBit(i)))
									{
										pix = i;
										smallestError = currentError;
									}
								}
							}
						}
					}
				}
				System.Array.Copy(Lookupcache, 2, Lookupcache, 0, CACHESIZE - 2);
				Lookupcache[CACHESIZE - 1] = rgb;
				Lookupcache[CACHESIZE - 2] = ~pix;
				return Installpixel(pixel, pix);
			}
		}

		private Object Installpixel(Object pixel, int pix)
		{
			switch (TransferType_Renamed)
			{
			case DataBuffer.TYPE_INT:
				int[] intObj;
				if (pixel == null)
				{
					pixel = intObj = new int[1];
				}
				else
				{
					intObj = (int[]) pixel;
				}
				intObj[0] = pix;
				break;
			case DataBuffer.TYPE_BYTE:
				sbyte[] byteObj;
				if (pixel == null)
				{
					pixel = byteObj = new sbyte[1];
				}
				else
				{
					byteObj = (sbyte[]) pixel;
				}
				byteObj[0] = (sbyte) pix;
				break;
			case DataBuffer.TYPE_USHORT:
				short[] shortObj;
				if (pixel == null)
				{
					pixel = shortObj = new short[1];
				}
				else
				{
					shortObj = (short[]) pixel;
				}
				shortObj[0] = (short) pix;
				break;
			default:
				throw new UnsupportedOperationException("This method has not been " + "implemented for transferType " + TransferType_Renamed);
			}
			return pixel;
		}

		/// <summary>
		/// Returns an array of unnormalized color/alpha components for a
		/// specified pixel in this <code>ColorModel</code>.  The pixel value
		/// is specified as an int.  If the <code>components</code> array is <code>null</code>,
		/// a new array is allocated that contains
		/// <code>offset + getNumComponents()</code> elements.
		/// The <code>components</code> array is returned,
		/// with the alpha component included
		/// only if <code>hasAlpha</code> returns true.
		/// Color/alpha components are stored in the <code>components</code> array starting
		/// at <code>offset</code> even if the array is allocated by this method.
		/// An <code>ArrayIndexOutOfBoundsException</code>
		/// is thrown if  the <code>components</code> array is not <code>null</code> and is
		/// not large enough to hold all the color and alpha components
		/// starting at <code>offset</code>. </summary>
		/// <param name="pixel"> the specified pixel </param>
		/// <param name="components"> the array to receive the color and alpha
		/// components of the specified pixel </param>
		/// <param name="offset"> the offset into the <code>components</code> array at
		/// which to start storing the color and alpha components </param>
		/// <returns> an array containing the color and alpha components of the
		/// specified pixel starting at the specified offset. </returns>
		/// <seealso cref= ColorModel#hasAlpha </seealso>
		/// <seealso cref= ColorModel#getNumComponents </seealso>
		public override int[] GetComponents(int pixel, int[] components, int offset)
		{
			if (components == null)
			{
				components = new int[offset + NumComponents_Renamed];
			}

			// REMIND: Needs to change if different color space
			components[offset + 0] = GetRed(pixel);
			components[offset + 1] = GetGreen(pixel);
			components[offset + 2] = GetBlue(pixel);
			if (SupportsAlpha && (components.Length - offset) > 3)
			{
				components[offset + 3] = GetAlpha(pixel);
			}

			return components;
		}

		/// <summary>
		/// Returns an array of unnormalized color/alpha components for
		/// a specified pixel in this <code>ColorModel</code>.  The pixel
		/// value is specified by an array of data elements of type
		/// <code>transferType</code> passed in as an object reference.
		/// If <code>pixel</code> is not a primitive array of type
		/// <code>transferType</code>, a <code>ClassCastException</code>
		/// is thrown.  An <code>ArrayIndexOutOfBoundsException</code>
		/// is thrown if <code>pixel</code> is not large enough to hold
		/// a pixel value for this <code>ColorModel</code>.  If the
		/// <code>components</code> array is <code>null</code>, a new array
		/// is allocated that contains
		/// <code>offset + getNumComponents()</code> elements.
		/// The <code>components</code> array is returned,
		/// with the alpha component included
		/// only if <code>hasAlpha</code> returns true.
		/// Color/alpha components are stored in the <code>components</code>
		/// array starting at <code>offset</code> even if the array is
		/// allocated by this method.  An
		/// <code>ArrayIndexOutOfBoundsException</code> is also
		/// thrown if  the <code>components</code> array is not
		/// <code>null</code> and is not large enough to hold all the color
		/// and alpha components starting at <code>offset</code>.
		/// <para>
		/// Since <code>IndexColorModel</code> can be subclassed, subclasses
		/// inherit the implementation of this method and if they don't
		/// override it then they throw an exception if they use an
		/// unsupported <code>transferType</code>.
		/// 
		/// </para>
		/// </summary>
		/// <param name="pixel"> the specified pixel </param>
		/// <param name="components"> an array that receives the color and alpha
		/// components of the specified pixel </param>
		/// <param name="offset"> the index into the <code>components</code> array at
		/// which to begin storing the color and alpha components of the
		/// specified pixel </param>
		/// <returns> an array containing the color and alpha components of the
		/// specified pixel starting at the specified offset. </returns>
		/// <exception cref="ArrayIndexOutOfBoundsException"> if <code>pixel</code>
		///            is not large enough to hold a pixel value for this
		///            <code>ColorModel</code> or if the
		///            <code>components</code> array is not <code>null</code>
		///            and is not large enough to hold all the color
		///            and alpha components starting at <code>offset</code> </exception>
		/// <exception cref="ClassCastException"> if <code>pixel</code> is not a
		///            primitive array of type <code>transferType</code> </exception>
		/// <exception cref="UnsupportedOperationException"> if <code>transferType</code>
		///         is not one of the supported transfer types </exception>
		/// <seealso cref= ColorModel#hasAlpha </seealso>
		/// <seealso cref= ColorModel#getNumComponents </seealso>
		public override int[] GetComponents(Object pixel, int[] components, int offset)
		{
			int intpixel;
			switch (TransferType_Renamed)
			{
				case DataBuffer.TYPE_BYTE:
				   sbyte[] bdata = (sbyte[])pixel;
				   intpixel = bdata[0] & 0xff;
				break;
				case DataBuffer.TYPE_USHORT:
				   short[] sdata = (short[])pixel;
				   intpixel = sdata[0] & 0xffff;
				break;
				case DataBuffer.TYPE_INT:
				   int[] idata = (int[])pixel;
				   intpixel = idata[0];
				break;
				default:
				   throw new UnsupportedOperationException("This method has not been " + "implemented for transferType " + TransferType_Renamed);
			}
			return GetComponents(intpixel, components, offset);
		}

		/// <summary>
		/// Returns a pixel value represented as an int in this
		/// <code>ColorModel</code> given an array of unnormalized
		/// color/alpha components.  An
		/// <code>ArrayIndexOutOfBoundsException</code>
		/// is thrown if the <code>components</code> array is not large
		/// enough to hold all of the color and alpha components starting
		/// at <code>offset</code>.  Since
		/// <code>ColorModel</code> can be subclassed, subclasses inherit the
		/// implementation of this method and if they don't override it then
		/// they throw an exception if they use an unsupported transferType. </summary>
		/// <param name="components"> an array of unnormalized color and alpha
		/// components </param>
		/// <param name="offset"> the index into <code>components</code> at which to
		/// begin retrieving the color and alpha components </param>
		/// <returns> an <code>int</code> pixel value in this
		/// <code>ColorModel</code> corresponding to the specified components. </returns>
		/// <exception cref="ArrayIndexOutOfBoundsException"> if
		///  the <code>components</code> array is not large enough to
		///  hold all of the color and alpha components starting at
		///  <code>offset</code> </exception>
		/// <exception cref="UnsupportedOperationException"> if <code>transferType</code>
		///         is invalid </exception>
		public override int GetDataElement(int[] components, int offset)
		{
			int rgb = (components[offset + 0] << 16) | (components[offset + 1] << 8) | (components[offset + 2]);
			if (SupportsAlpha)
			{
				rgb |= (components[offset + 3] << 24);
			}
			else
			{
				rgb |= unchecked((int)0xff000000);
			}
			Object inData = GetDataElements(rgb, null);
			int pixel;
			switch (TransferType_Renamed)
			{
				case DataBuffer.TYPE_BYTE:
				   sbyte[] bdata = (sbyte[])inData;
				   pixel = bdata[0] & 0xff;
				break;
				case DataBuffer.TYPE_USHORT:
				   short[] sdata = (short[])inData;
				   pixel = sdata[0];
				break;
				case DataBuffer.TYPE_INT:
				   int[] idata = (int[])inData;
				   pixel = idata[0];
				break;
				default:
				   throw new UnsupportedOperationException("This method has not been " + "implemented for transferType " + TransferType_Renamed);
			}
			return pixel;
		}

		/// <summary>
		/// Returns a data element array representation of a pixel in this
		/// <code>ColorModel</code> given an array of unnormalized color/alpha
		/// components.  This array can then be passed to the
		/// <code>setDataElements</code> method of a <code>WritableRaster</code>
		/// object.  An <code>ArrayIndexOutOfBoundsException</code> is
		/// thrown if the
		/// <code>components</code> array is not large enough to hold all of the
		/// color and alpha components starting at <code>offset</code>.
		/// If the pixel variable is <code>null</code>, a new array
		/// is allocated.  If <code>pixel</code> is not <code>null</code>,
		/// it must be a primitive array of type <code>transferType</code>;
		/// otherwise, a <code>ClassCastException</code> is thrown.
		/// An <code>ArrayIndexOutOfBoundsException</code> is thrown if pixel
		/// is not large enough to hold a pixel value for this
		/// <code>ColorModel</code>.
		/// <para>
		/// Since <code>IndexColorModel</code> can be subclassed, subclasses
		/// inherit the implementation of this method and if they don't
		/// override it then they throw an exception if they use an
		/// unsupported <code>transferType</code>
		/// 
		/// </para>
		/// </summary>
		/// <param name="components"> an array of unnormalized color and alpha
		/// components </param>
		/// <param name="offset"> the index into <code>components</code> at which to
		/// begin retrieving color and alpha components </param>
		/// <param name="pixel"> the <code>Object</code> representing an array of color
		/// and alpha components </param>
		/// <returns> an <code>Object</code> representing an array of color and
		/// alpha components. </returns>
		/// <exception cref="ClassCastException"> if <code>pixel</code>
		///  is not a primitive array of type <code>transferType</code> </exception>
		/// <exception cref="ArrayIndexOutOfBoundsException"> if
		///  <code>pixel</code> is not large enough to hold a pixel value
		///  for this <code>ColorModel</code> or the <code>components</code>
		///  array is not large enough to hold all of the color and alpha
		///  components starting at <code>offset</code> </exception>
		/// <exception cref="UnsupportedOperationException"> if <code>transferType</code>
		///         is not one of the supported transfer types </exception>
		/// <seealso cref= WritableRaster#setDataElements </seealso>
		/// <seealso cref= SampleModel#setDataElements </seealso>
		public override Object GetDataElements(int[] components, int offset, Object pixel)
		{
			int rgb = (components[offset + 0] << 16) | (components[offset + 1] << 8) | (components[offset + 2]);
			if (SupportsAlpha)
			{
				rgb |= (components[offset + 3] << 24);
			}
			else
			{
				rgb &= unchecked((int)0xff000000);
			}
			return GetDataElements(rgb, pixel);
		}

		/// <summary>
		/// Creates a <code>WritableRaster</code> with the specified width
		/// and height that has a data layout (<code>SampleModel</code>)
		/// compatible with this <code>ColorModel</code>.  This method
		/// only works for color models with 16 or fewer bits per pixel.
		/// <para>
		/// Since <code>IndexColorModel</code> can be subclassed, any
		/// subclass that supports greater than 16 bits per pixel must
		/// override this method.
		/// 
		/// </para>
		/// </summary>
		/// <param name="w"> the width to apply to the new <code>WritableRaster</code> </param>
		/// <param name="h"> the height to apply to the new <code>WritableRaster</code> </param>
		/// <returns> a <code>WritableRaster</code> object with the specified
		/// width and height. </returns>
		/// <exception cref="UnsupportedOperationException"> if the number of bits in a
		///         pixel is greater than 16 </exception>
		/// <seealso cref= WritableRaster </seealso>
		/// <seealso cref= SampleModel </seealso>
		public override WritableRaster CreateCompatibleWritableRaster(int w, int h)
		{
			WritableRaster raster;

			if (Pixel_bits == 1 || Pixel_bits == 2 || Pixel_bits == 4)
			{
				// TYPE_BINARY
				raster = Raster.CreatePackedRaster(DataBuffer.TYPE_BYTE, w, h, 1, Pixel_bits, null);
			}
			else if (Pixel_bits <= 8)
			{
				raster = Raster.CreateInterleavedRaster(DataBuffer.TYPE_BYTE, w,h,1,null);
			}
			else if (Pixel_bits <= 16)
			{
				raster = Raster.CreateInterleavedRaster(DataBuffer.TYPE_USHORT, w,h,1,null);
			}
			else
			{
				throw new UnsupportedOperationException("This method is not supported " + " for pixel bits > 16.");
			}
			return raster;
		}

		/// <summary>
		/// Returns <code>true</code> if <code>raster</code> is compatible
		/// with this <code>ColorModel</code> or <code>false</code> if it
		/// is not compatible with this <code>ColorModel</code>. </summary>
		/// <param name="raster"> the <seealso cref="Raster"/> object to test for compatibility </param>
		/// <returns> <code>true</code> if <code>raster</code> is compatible
		/// with this <code>ColorModel</code>; <code>false</code> otherwise.
		///   </returns>
		public override bool IsCompatibleRaster(Raster raster)
		{

			int size = raster.SampleModel.GetSampleSize(0);
			return ((raster.TransferType == TransferType_Renamed) && (raster.NumBands == 1) && ((1 << size) >= Map_size));
		}

		/// <summary>
		/// Creates a <code>SampleModel</code> with the specified
		/// width and height that has a data layout compatible with
		/// this <code>ColorModel</code>. </summary>
		/// <param name="w"> the width to apply to the new <code>SampleModel</code> </param>
		/// <param name="h"> the height to apply to the new <code>SampleModel</code> </param>
		/// <returns> a <code>SampleModel</code> object with the specified
		/// width and height. </returns>
		/// <exception cref="IllegalArgumentException"> if <code>w</code> or
		///         <code>h</code> is not greater than 0 </exception>
		/// <seealso cref= SampleModel </seealso>
		public override SampleModel CreateCompatibleSampleModel(int w, int h)
		{
			int[] off = new int[1];
			off[0] = 0;
			if (Pixel_bits == 1 || Pixel_bits == 2 || Pixel_bits == 4)
			{
				return new MultiPixelPackedSampleModel(TransferType_Renamed, w, h, Pixel_bits);
			}
			else
			{
				return new ComponentSampleModel(TransferType_Renamed, w, h, 1, w, off);
			}
		}

		/// <summary>
		/// Checks if the specified <code>SampleModel</code> is compatible
		/// with this <code>ColorModel</code>.  If <code>sm</code> is
		/// <code>null</code>, this method returns <code>false</code>. </summary>
		/// <param name="sm"> the specified <code>SampleModel</code>,
		///           or <code>null</code> </param>
		/// <returns> <code>true</code> if the specified <code>SampleModel</code>
		/// is compatible with this <code>ColorModel</code>; <code>false</code>
		/// otherwise. </returns>
		/// <seealso cref= SampleModel </seealso>
		public override bool IsCompatibleSampleModel(SampleModel sm)
		{
			// fix 4238629
			if (!(sm is ComponentSampleModel) && !(sm is MultiPixelPackedSampleModel))
			{
				return false;
			}

			// Transfer type must be the same
			if (sm.TransferType != TransferType_Renamed)
			{
				return false;
			}

			if (sm.NumBands != 1)
			{
				return false;
			}

			return true;
		}

		/// <summary>
		/// Returns a new <code>BufferedImage</code> of TYPE_INT_ARGB or
		/// TYPE_INT_RGB that has a <code>Raster</code> with pixel data
		/// computed by expanding the indices in the source <code>Raster</code>
		/// using the color/alpha component arrays of this <code>ColorModel</code>.
		/// Only the lower <em>n</em> bits of each index value in the source
		/// <code>Raster</code>, as specified in the
		/// <a href="#index_values">class description</a> above, are used to
		/// compute the color/alpha values in the returned image.
		/// If <code>forceARGB</code> is <code>true</code>, a TYPE_INT_ARGB image is
		/// returned regardless of whether or not this <code>ColorModel</code>
		/// has an alpha component array or a transparent pixel. </summary>
		/// <param name="raster"> the specified <code>Raster</code> </param>
		/// <param name="forceARGB"> if <code>true</code>, the returned
		///     <code>BufferedImage</code> is TYPE_INT_ARGB; otherwise it is
		///     TYPE_INT_RGB </param>
		/// <returns> a <code>BufferedImage</code> created with the specified
		///     <code>Raster</code> </returns>
		/// <exception cref="IllegalArgumentException"> if the raster argument is not
		///           compatible with this IndexColorModel </exception>
		public virtual BufferedImage ConvertToIntDiscrete(Raster raster, bool forceARGB)
		{
			ColorModel cm;

			if (!IsCompatibleRaster(raster))
			{
				throw new IllegalArgumentException("This raster is not compatible" + "with this IndexColorModel.");
			}
			if (forceARGB || Transparency_Renamed == java.awt.Transparency_Fields.TRANSLUCENT)
			{
				cm = ColorModel.RGBdefault;
			}
			else if (Transparency_Renamed == java.awt.Transparency_Fields.BITMASK)
			{
				cm = new DirectColorModel(25, 0xff0000, 0x00ff00, 0x0000ff, 0x1000000);
			}
			else
			{
				cm = new DirectColorModel(24, 0xff0000, 0x00ff00, 0x0000ff);
			}

			int w = raster.Width;
			int h = raster.Height;
			WritableRaster discreteRaster = cm.CreateCompatibleWritableRaster(w, h);
			Object obj = null;
			int[] data = null;

			int rX = raster.MinX;
			int rY = raster.MinY;

			for (int y = 0; y < h; y++, rY++)
			{
				obj = raster.GetDataElements(rX, rY, w, 1, obj);
				if (obj is int[])
				{
					data = (int[])obj;
				}
				else
				{
					data = DataBuffer.ToIntArray(obj);
				}
				for (int x = 0; x < w; x++)
				{
					data[x] = Rgb[data[x] & Pixel_mask];
				}
				discreteRaster.SetDataElements(0, y, w, 1, data);
			}

			return new BufferedImage(cm, discreteRaster, false, null);
		}

		/// <summary>
		/// Returns whether or not the pixel is valid. </summary>
		/// <param name="pixel"> the specified pixel value </param>
		/// <returns> <code>true</code> if <code>pixel</code>
		/// is valid; <code>false</code> otherwise.
		/// @since 1.3 </returns>
		public virtual bool IsValid(int pixel)
		{
			return ((pixel >= 0 && pixel < Map_size) && (ValidBits == null || ValidBits.testBit(pixel)));
		}

		/// <summary>
		/// Returns whether or not all of the pixels are valid. </summary>
		/// <returns> <code>true</code> if all pixels are valid;
		/// <code>false</code> otherwise.
		/// @since 1.3 </returns>
		public virtual bool Valid
		{
			get
			{
				return (ValidBits == null);
			}
		}

		/// <summary>
		/// Returns a <code>BigInteger</code> that indicates the valid/invalid
		/// pixels in the colormap.  A bit is valid if the
		/// <code>BigInteger</code> value at that index is set, and is invalid
		/// if the <code>BigInteger</code> value at that index is not set.
		/// The only valid ranges to query in the <code>BigInteger</code> are
		/// between 0 and the map size. </summary>
		/// <returns> a <code>BigInteger</code> indicating the valid/invalid pixels.
		/// @since 1.3 </returns>
		public virtual System.Numerics.BigInteger ValidPixels
		{
			get
			{
				if (ValidBits == null)
				{
					return AllValid;
				}
				else
				{
					return ValidBits;
				}
			}
		}

		/// <summary>
		/// Disposes of system resources associated with this
		/// <code>ColorModel</code> once this <code>ColorModel</code> is no
		/// longer referenced.
		/// </summary>
		~IndexColorModel()
		{
		}

		/// <summary>
		/// Returns the <code>String</code> representation of the contents of
		/// this <code>ColorModel</code>object. </summary>
		/// <returns> a <code>String</code> representing the contents of this
		/// <code>ColorModel</code> object. </returns>
		public override String ToString()
		{
		   return "IndexColorModel: #pixelBits = " + Pixel_bits + " numComponents = " + NumComponents_Renamed + " color space = " + ColorSpace_Renamed + " transparency = " + Transparency_Renamed + " transIndex   = " + Transparent_index + " has alpha = " + SupportsAlpha + " isAlphaPre = " + IsAlphaPremultiplied;
		}
	}

}
using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;

/*
 * Copyright (c) 1997, 2015, Oracle and/or its affiliates. All rights reserved.
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


	using ByteComponentRaster = sun.awt.image.ByteComponentRaster;
	using BytePackedRaster = sun.awt.image.BytePackedRaster;
	using IntegerComponentRaster = sun.awt.image.IntegerComponentRaster;
	using OffScreenImageSource = sun.awt.image.OffScreenImageSource;
	using ShortComponentRaster = sun.awt.image.ShortComponentRaster;

	/// 
	/// <summary>
	/// The <code>BufferedImage</code> subclass describes an {@link
	/// java.awt.Image Image} with an accessible buffer of image data.
	/// A <code>BufferedImage</code> is comprised of a <seealso cref="ColorModel"/> and a
	/// <seealso cref="Raster"/> of image data.
	/// The number and types of bands in the <seealso cref="SampleModel"/> of the
	/// <code>Raster</code> must match the number and types required by the
	/// <code>ColorModel</code> to represent its color and alpha components.
	/// All <code>BufferedImage</code> objects have an upper left corner
	/// coordinate of (0,&nbsp;0).  Any <code>Raster</code> used to construct a
	/// <code>BufferedImage</code> must therefore have minX=0 and minY=0.
	/// 
	/// <para>
	/// This class relies on the data fetching and setting methods
	/// of <code>Raster</code>,
	/// and on the color characterization methods of <code>ColorModel</code>.
	/// 
	/// </para>
	/// </summary>
	/// <seealso cref= ColorModel </seealso>
	/// <seealso cref= Raster </seealso>
	/// <seealso cref= WritableRaster </seealso>
	public class BufferedImage : java.awt.Image, WritableRenderedImage, Transparency
	{
		private int ImageType = TYPE_CUSTOM;
		private ColorModel ColorModel_Renamed;
		private readonly WritableRaster Raster_Renamed;
		private OffScreenImageSource Osis;
		private Dictionary<String, Object> Properties;

		/// <summary>
		/// Image Type Constants
		/// </summary>

		/// <summary>
		/// Image type is not recognized so it must be a customized
		/// image.  This type is only used as a return value for the getType()
		/// method.
		/// </summary>
		public const int TYPE_CUSTOM = 0;

		/// <summary>
		/// Represents an image with 8-bit RGB color components packed into
		/// integer pixels.  The image has a <seealso cref="DirectColorModel"/> without
		/// alpha.
		/// When data with non-opaque alpha is stored
		/// in an image of this type,
		/// the color data must be adjusted to a non-premultiplied form
		/// and the alpha discarded,
		/// as described in the
		/// <seealso cref="java.awt.AlphaComposite"/> documentation.
		/// </summary>
		public const int TYPE_INT_RGB = 1;

		/// <summary>
		/// Represents an image with 8-bit RGBA color components packed into
		/// integer pixels.  The image has a <code>DirectColorModel</code>
		/// with alpha. The color data in this image is considered not to be
		/// premultiplied with alpha.  When this type is used as the
		/// <code>imageType</code> argument to a <code>BufferedImage</code>
		/// constructor, the created image is consistent with images
		/// created in the JDK1.1 and earlier releases.
		/// </summary>
		public const int TYPE_INT_ARGB = 2;

		/// <summary>
		/// Represents an image with 8-bit RGBA color components packed into
		/// integer pixels.  The image has a <code>DirectColorModel</code>
		/// with alpha.  The color data in this image is considered to be
		/// premultiplied with alpha.
		/// </summary>
		public const int TYPE_INT_ARGB_PRE = 3;

		/// <summary>
		/// Represents an image with 8-bit RGB color components, corresponding
		/// to a Windows- or Solaris- style BGR color model, with the colors
		/// Blue, Green, and Red packed into integer pixels.  There is no alpha.
		/// The image has a <seealso cref="DirectColorModel"/>.
		/// When data with non-opaque alpha is stored
		/// in an image of this type,
		/// the color data must be adjusted to a non-premultiplied form
		/// and the alpha discarded,
		/// as described in the
		/// <seealso cref="java.awt.AlphaComposite"/> documentation.
		/// </summary>
		public const int TYPE_INT_BGR = 4;

		/// <summary>
		/// Represents an image with 8-bit RGB color components, corresponding
		/// to a Windows-style BGR color model) with the colors Blue, Green,
		/// and Red stored in 3 bytes.  There is no alpha.  The image has a
		/// <code>ComponentColorModel</code>.
		/// When data with non-opaque alpha is stored
		/// in an image of this type,
		/// the color data must be adjusted to a non-premultiplied form
		/// and the alpha discarded,
		/// as described in the
		/// <seealso cref="java.awt.AlphaComposite"/> documentation.
		/// </summary>
		public const int TYPE_3BYTE_BGR = 5;

		/// <summary>
		/// Represents an image with 8-bit RGBA color components with the colors
		/// Blue, Green, and Red stored in 3 bytes and 1 byte of alpha.  The
		/// image has a <code>ComponentColorModel</code> with alpha.  The
		/// color data in this image is considered not to be premultiplied with
		/// alpha.  The byte data is interleaved in a single
		/// byte array in the order A, B, G, R
		/// from lower to higher byte addresses within each pixel.
		/// </summary>
		public const int TYPE_4BYTE_ABGR = 6;

		/// <summary>
		/// Represents an image with 8-bit RGBA color components with the colors
		/// Blue, Green, and Red stored in 3 bytes and 1 byte of alpha.  The
		/// image has a <code>ComponentColorModel</code> with alpha. The color
		/// data in this image is considered to be premultiplied with alpha.
		/// The byte data is interleaved in a single byte array in the order
		/// A, B, G, R from lower to higher byte addresses within each pixel.
		/// </summary>
		public const int TYPE_4BYTE_ABGR_PRE = 7;

		/// <summary>
		/// Represents an image with 5-6-5 RGB color components (5-bits red,
		/// 6-bits green, 5-bits blue) with no alpha.  This image has
		/// a <code>DirectColorModel</code>.
		/// When data with non-opaque alpha is stored
		/// in an image of this type,
		/// the color data must be adjusted to a non-premultiplied form
		/// and the alpha discarded,
		/// as described in the
		/// <seealso cref="java.awt.AlphaComposite"/> documentation.
		/// </summary>
		public const int TYPE_USHORT_565_RGB = 8;

		/// <summary>
		/// Represents an image with 5-5-5 RGB color components (5-bits red,
		/// 5-bits green, 5-bits blue) with no alpha.  This image has
		/// a <code>DirectColorModel</code>.
		/// When data with non-opaque alpha is stored
		/// in an image of this type,
		/// the color data must be adjusted to a non-premultiplied form
		/// and the alpha discarded,
		/// as described in the
		/// <seealso cref="java.awt.AlphaComposite"/> documentation.
		/// </summary>
		public const int TYPE_USHORT_555_RGB = 9;

		/// <summary>
		/// Represents a unsigned byte grayscale image, non-indexed.  This
		/// image has a <code>ComponentColorModel</code> with a CS_GRAY
		/// <seealso cref="ColorSpace"/>.
		/// When data with non-opaque alpha is stored
		/// in an image of this type,
		/// the color data must be adjusted to a non-premultiplied form
		/// and the alpha discarded,
		/// as described in the
		/// <seealso cref="java.awt.AlphaComposite"/> documentation.
		/// </summary>
		public const int TYPE_BYTE_GRAY = 10;

		/// <summary>
		/// Represents an unsigned short grayscale image, non-indexed).  This
		/// image has a <code>ComponentColorModel</code> with a CS_GRAY
		/// <code>ColorSpace</code>.
		/// When data with non-opaque alpha is stored
		/// in an image of this type,
		/// the color data must be adjusted to a non-premultiplied form
		/// and the alpha discarded,
		/// as described in the
		/// <seealso cref="java.awt.AlphaComposite"/> documentation.
		/// </summary>
		public const int TYPE_USHORT_GRAY = 11;

		/// <summary>
		/// Represents an opaque byte-packed 1, 2, or 4 bit image.  The
		/// image has an <seealso cref="IndexColorModel"/> without alpha.  When this
		/// type is used as the <code>imageType</code> argument to the
		/// <code>BufferedImage</code> constructor that takes an
		/// <code>imageType</code> argument but no <code>ColorModel</code>
		/// argument, a 1-bit image is created with an
		/// <code>IndexColorModel</code> with two colors in the default
		/// sRGB <code>ColorSpace</code>: {0,&nbsp;0,&nbsp;0} and
		/// {255,&nbsp;255,&nbsp;255}.
		/// 
		/// <para> Images with 2 or 4 bits per pixel may be constructed via
		/// the <code>BufferedImage</code> constructor that takes a
		/// <code>ColorModel</code> argument by supplying a
		/// <code>ColorModel</code> with an appropriate map size.
		/// 
		/// </para>
		/// <para> Images with 8 bits per pixel should use the image types
		/// <code>TYPE_BYTE_INDEXED</code> or <code>TYPE_BYTE_GRAY</code>
		/// depending on their <code>ColorModel</code>.
		/// 
		/// </para>
		/// <para> When color data is stored in an image of this type,
		/// the closest color in the colormap is determined
		/// by the <code>IndexColorModel</code> and the resulting index is stored.
		/// Approximation and loss of alpha or color components
		/// can result, depending on the colors in the
		/// <code>IndexColorModel</code> colormap.
		/// </para>
		/// </summary>
		public const int TYPE_BYTE_BINARY = 12;

		/// <summary>
		/// Represents an indexed byte image.  When this type is used as the
		/// <code>imageType</code> argument to the <code>BufferedImage</code>
		/// constructor that takes an <code>imageType</code> argument
		/// but no <code>ColorModel</code> argument, an
		/// <code>IndexColorModel</code> is created with
		/// a 256-color 6/6/6 color cube palette with the rest of the colors
		/// from 216-255 populated by grayscale values in the
		/// default sRGB ColorSpace.
		/// 
		/// <para> When color data is stored in an image of this type,
		/// the closest color in the colormap is determined
		/// by the <code>IndexColorModel</code> and the resulting index is stored.
		/// Approximation and loss of alpha or color components
		/// can result, depending on the colors in the
		/// <code>IndexColorModel</code> colormap.
		/// </para>
		/// </summary>
		public const int TYPE_BYTE_INDEXED = 13;

		private const int DCM_RED_MASK = 0x00ff0000;
		private const int DCM_GREEN_MASK = 0x0000ff00;
		private const int DCM_BLUE_MASK = 0x000000ff;
		private const int DCM_ALPHA_MASK = unchecked((int)0xff000000);
		private const int DCM_565_RED_MASK = 0xf800;
		private const int DCM_565_GRN_MASK = 0x07E0;
		private const int DCM_565_BLU_MASK = 0x001F;
		private const int DCM_555_RED_MASK = 0x7C00;
		private const int DCM_555_GRN_MASK = 0x03E0;
		private const int DCM_555_BLU_MASK = 0x001F;
		private const int DCM_BGR_RED_MASK = 0x0000ff;
		private const int DCM_BGR_GRN_MASK = 0x00ff00;
		private const int DCM_BGR_BLU_MASK = 0xff0000;


//JAVA TO C# CONVERTER TODO TASK: Replace 'unknown' with the appropriate dll name:
		[DllImport("unknown")]
		private static extern void initIDs();
		static BufferedImage()
		{
			ColorModel.LoadLibraries();
			initIDs();
		}

		/// <summary>
		/// Constructs a <code>BufferedImage</code> of one of the predefined
		/// image types.  The <code>ColorSpace</code> for the image is the
		/// default sRGB space. </summary>
		/// <param name="width">     width of the created image </param>
		/// <param name="height">    height of the created image </param>
		/// <param name="imageType"> type of the created image </param>
		/// <seealso cref= ColorSpace </seealso>
		/// <seealso cref= #TYPE_INT_RGB </seealso>
		/// <seealso cref= #TYPE_INT_ARGB </seealso>
		/// <seealso cref= #TYPE_INT_ARGB_PRE </seealso>
		/// <seealso cref= #TYPE_INT_BGR </seealso>
		/// <seealso cref= #TYPE_3BYTE_BGR </seealso>
		/// <seealso cref= #TYPE_4BYTE_ABGR </seealso>
		/// <seealso cref= #TYPE_4BYTE_ABGR_PRE </seealso>
		/// <seealso cref= #TYPE_BYTE_GRAY </seealso>
		/// <seealso cref= #TYPE_USHORT_GRAY </seealso>
		/// <seealso cref= #TYPE_BYTE_BINARY </seealso>
		/// <seealso cref= #TYPE_BYTE_INDEXED </seealso>
		/// <seealso cref= #TYPE_USHORT_565_RGB </seealso>
		/// <seealso cref= #TYPE_USHORT_555_RGB </seealso>
		public BufferedImage(int width, int height, int imageType)
		{
			switch (imageType)
			{
			case TYPE_INT_RGB:
			{
					ColorModel_Renamed = new DirectColorModel(24, 0x00ff0000, 0x0000ff00, 0x000000ff, 0x0); // Alpha -  Blue -  Green -  Red
					Raster_Renamed = ColorModel_Renamed.CreateCompatibleWritableRaster(width, height);
			}
			break;

			case TYPE_INT_ARGB:
			{
					ColorModel_Renamed = ColorModel.RGBdefault;

					Raster_Renamed = ColorModel_Renamed.CreateCompatibleWritableRaster(width, height);
			}
			break;

			case TYPE_INT_ARGB_PRE:
			{
					ColorModel_Renamed = new DirectColorModel(ColorSpace.GetInstance(ColorSpace.CS_sRGB_Renamed), 32, 0x00ff0000, 0x0000ff00, 0x000000ff, unchecked((int)0xff000000), true, DataBuffer.TYPE_INT); // Alpha Premultiplied -  Alpha -  Blue -  Green -  Red
					Raster_Renamed = ColorModel_Renamed.CreateCompatibleWritableRaster(width, height);
			}
			break;

			case TYPE_INT_BGR:
			{
					ColorModel_Renamed = new DirectColorModel(24, 0x000000ff, 0x0000ff00, 0x00ff0000); // Blue -  Green -  Red
					Raster_Renamed = ColorModel_Renamed.CreateCompatibleWritableRaster(width, height);
			}
			break;

			case TYPE_3BYTE_BGR:
			{
					ColorSpace cs = ColorSpace.GetInstance(ColorSpace.CS_sRGB_Renamed);
					int[] nBits = new int[] {8, 8, 8};
					int[] bOffs = new int[] {2, 1, 0};
					ColorModel_Renamed = new ComponentColorModel(cs, nBits, false, false, java.awt.Transparency_Fields.OPAQUE, DataBuffer.TYPE_BYTE);
					Raster_Renamed = Raster.CreateInterleavedRaster(DataBuffer.TYPE_BYTE, width, height, width * 3, 3, bOffs, null);
			}
			break;

			case TYPE_4BYTE_ABGR:
			{
					ColorSpace cs = ColorSpace.GetInstance(ColorSpace.CS_sRGB_Renamed);
					int[] nBits = new int[] {8, 8, 8, 8};
					int[] bOffs = new int[] {3, 2, 1, 0};
					ColorModel_Renamed = new ComponentColorModel(cs, nBits, true, false, java.awt.Transparency_Fields.TRANSLUCENT, DataBuffer.TYPE_BYTE);
					Raster_Renamed = Raster.CreateInterleavedRaster(DataBuffer.TYPE_BYTE, width, height, width * 4, 4, bOffs, null);
			}
			break;

			case TYPE_4BYTE_ABGR_PRE:
			{
					ColorSpace cs = ColorSpace.GetInstance(ColorSpace.CS_sRGB_Renamed);
					int[] nBits = new int[] {8, 8, 8, 8};
					int[] bOffs = new int[] {3, 2, 1, 0};
					ColorModel_Renamed = new ComponentColorModel(cs, nBits, true, true, java.awt.Transparency_Fields.TRANSLUCENT, DataBuffer.TYPE_BYTE);
					Raster_Renamed = Raster.CreateInterleavedRaster(DataBuffer.TYPE_BYTE, width, height, width * 4, 4, bOffs, null);
			}
			break;

			case TYPE_BYTE_GRAY:
			{
					ColorSpace cs = ColorSpace.GetInstance(ColorSpace.CS_GRAY);
					int[] nBits = new int[] {8};
					ColorModel_Renamed = new ComponentColorModel(cs, nBits, false, true, java.awt.Transparency_Fields.OPAQUE, DataBuffer.TYPE_BYTE);
					Raster_Renamed = ColorModel_Renamed.CreateCompatibleWritableRaster(width, height);
			}
			break;

			case TYPE_USHORT_GRAY:
			{
					ColorSpace cs = ColorSpace.GetInstance(ColorSpace.CS_GRAY);
					int[] nBits = new int[] {16};
					ColorModel_Renamed = new ComponentColorModel(cs, nBits, false, true, java.awt.Transparency_Fields.OPAQUE, DataBuffer.TYPE_USHORT);
					Raster_Renamed = ColorModel_Renamed.CreateCompatibleWritableRaster(width, height);
			}
			break;

			case TYPE_BYTE_BINARY:
			{
					sbyte[] arr = new sbyte[] {(sbyte)0, unchecked((sbyte)0xff)};

					ColorModel_Renamed = new IndexColorModel(1, 2, arr, arr, arr);
					Raster_Renamed = Raster.CreatePackedRaster(DataBuffer.TYPE_BYTE, width, height, 1, 1, null);
			}
			break;

			case TYPE_BYTE_INDEXED:
			{
					// Create a 6x6x6 color cube
					int[] cmap = new int[256];
					int i = 0;
					for (int r = 0; r < 256; r += 51)
					{
						for (int g = 0; g < 256; g += 51)
						{
							for (int b = 0; b < 256; b += 51)
							{
								cmap[i++] = (r << 16) | (g << 8) | b;
							}
						}
					}
					// And populate the rest of the cmap with gray values
					int grayIncr = 256 / (256 - i);

					// The gray ramp will be between 18 and 252
					int gray = grayIncr * 3;
					for (; i < 256; i++)
					{
						cmap[i] = (gray << 16) | (gray << 8) | gray;
						gray += grayIncr;
					}

					ColorModel_Renamed = new IndexColorModel(8, 256, cmap, 0, false, -1, DataBuffer.TYPE_BYTE);
					Raster_Renamed = Raster.CreateInterleavedRaster(DataBuffer.TYPE_BYTE, width, height, 1, null);
			}
			break;

			case TYPE_USHORT_565_RGB:
			{
					ColorModel_Renamed = new DirectColorModel(16, DCM_565_RED_MASK, DCM_565_GRN_MASK, DCM_565_BLU_MASK);
					Raster_Renamed = ColorModel_Renamed.CreateCompatibleWritableRaster(width, height);
			}
				break;

			case TYPE_USHORT_555_RGB:
			{
					ColorModel_Renamed = new DirectColorModel(15, DCM_555_RED_MASK, DCM_555_GRN_MASK, DCM_555_BLU_MASK);
					Raster_Renamed = ColorModel_Renamed.CreateCompatibleWritableRaster(width, height);
			}
				break;

			default:
				throw new IllegalArgumentException("Unknown image type " + imageType);
			}

			this.ImageType = imageType;
		}

		/// <summary>
		/// Constructs a <code>BufferedImage</code> of one of the predefined
		/// image types:
		/// TYPE_BYTE_BINARY or TYPE_BYTE_INDEXED.
		/// 
		/// <para> If the image type is TYPE_BYTE_BINARY, the number of
		/// entries in the color model is used to determine whether the
		/// image should have 1, 2, or 4 bits per pixel.  If the color model
		/// has 1 or 2 entries, the image will have 1 bit per pixel.  If it
		/// has 3 or 4 entries, the image with have 2 bits per pixel.  If
		/// it has between 5 and 16 entries, the image will have 4 bits per
		/// pixel.  Otherwise, an IllegalArgumentException will be thrown.
		/// 
		/// </para>
		/// </summary>
		/// <param name="width">     width of the created image </param>
		/// <param name="height">    height of the created image </param>
		/// <param name="imageType"> type of the created image </param>
		/// <param name="cm">        <code>IndexColorModel</code> of the created image </param>
		/// <exception cref="IllegalArgumentException">   if the imageType is not
		/// TYPE_BYTE_BINARY or TYPE_BYTE_INDEXED or if the imageType is
		/// TYPE_BYTE_BINARY and the color map has more than 16 entries. </exception>
		/// <seealso cref= #TYPE_BYTE_BINARY </seealso>
		/// <seealso cref= #TYPE_BYTE_INDEXED </seealso>
		public BufferedImage(int width, int height, int imageType, IndexColorModel cm)
		{
			if (cm.HasAlpha() && cm.AlphaPremultiplied)
			{
				throw new IllegalArgumentException("This image types do not have " + "premultiplied alpha.");
			}

			switch (imageType)
			{
			case TYPE_BYTE_BINARY:
				int bits; // Will be set below
				int mapSize = cm.MapSize;
				if (mapSize <= 2)
				{
					bits = 1;
				}
				else if (mapSize <= 4)
				{
					bits = 2;
				}
				else if (mapSize <= 16)
				{
					bits = 4;
				}
				else
				{
					throw new IllegalArgumentException("Color map for TYPE_BYTE_BINARY " + "must have no more than 16 entries");
				}
				Raster_Renamed = Raster.CreatePackedRaster(DataBuffer.TYPE_BYTE, width, height, 1, bits, null);
				break;

			case TYPE_BYTE_INDEXED:
				Raster_Renamed = Raster.CreateInterleavedRaster(DataBuffer.TYPE_BYTE, width, height, 1, null);
				break;
			default:
				throw new IllegalArgumentException("Invalid image type (" + imageType + ").  Image type must" + " be either TYPE_BYTE_BINARY or " + " TYPE_BYTE_INDEXED");
			}

			if (!cm.IsCompatibleRaster(Raster_Renamed))
			{
				throw new IllegalArgumentException("Incompatible image type and IndexColorModel");
			}

			ColorModel_Renamed = cm;
			this.ImageType = imageType;
		}

		/// <summary>
		/// Constructs a new <code>BufferedImage</code> with a specified
		/// <code>ColorModel</code> and <code>Raster</code>.  If the number and
		/// types of bands in the <code>SampleModel</code> of the
		/// <code>Raster</code> do not match the number and types required by
		/// the <code>ColorModel</code> to represent its color and alpha
		/// components, a <seealso cref="RasterFormatException"/> is thrown.  This
		/// method can multiply or divide the color <code>Raster</code> data by
		/// alpha to match the <code>alphaPremultiplied</code> state
		/// in the <code>ColorModel</code>.  Properties for this
		/// <code>BufferedImage</code> can be established by passing
		/// in a <seealso cref="Hashtable"/> of <code>String</code>/<code>Object</code>
		/// pairs. </summary>
		/// <param name="cm"> <code>ColorModel</code> for the new image </param>
		/// <param name="raster">     <code>Raster</code> for the image data </param>
		/// <param name="isRasterPremultiplied">   if <code>true</code>, the data in
		///                  the raster has been premultiplied with alpha. </param>
		/// <param name="properties"> <code>Hashtable</code> of
		///                  <code>String</code>/<code>Object</code> pairs. </param>
		/// <exception cref="RasterFormatException"> if the number and
		/// types of bands in the <code>SampleModel</code> of the
		/// <code>Raster</code> do not match the number and types required by
		/// the <code>ColorModel</code> to represent its color and alpha
		/// components. </exception>
		/// <exception cref="IllegalArgumentException"> if
		///          <code>raster</code> is incompatible with <code>cm</code> </exception>
		/// <seealso cref= ColorModel </seealso>
		/// <seealso cref= Raster </seealso>
		/// <seealso cref= WritableRaster </seealso>


	/*
	 *
	 *  FOR NOW THE CODE WHICH DEFINES THE RASTER TYPE IS DUPLICATED BY DVF
	 *  SEE THE METHOD DEFINERASTERTYPE @ RASTEROUTPUTMANAGER
	 *
	 */
		public BufferedImage<T1>(ColorModel cm, WritableRaster raster, bool isRasterPremultiplied, Dictionary<T1> properties)
		{

			if (!cm.IsCompatibleRaster(raster))
			{
				throw new IllegalArgumentException("Raster " + raster + " is incompatible with ColorModel " + cm);
			}

			if ((raster.MinX_Renamed != 0) || (raster.MinY_Renamed != 0))
			{
				throw new IllegalArgumentException("Raster " + raster + " has minX or minY not equal to zero: " + raster.MinX_Renamed + " " + raster.MinY_Renamed);
			}

			ColorModel_Renamed = cm;
			this.Raster_Renamed = raster;
			if (properties != null && properties.Count > 0)
			{
				this.Properties = new Dictionary<>();
				foreach (Object key in properties.Keys)
				{
					if (key is String)
					{
						this.Properties[(String) key] = properties[key];
					}
				}
			}
			int numBands = raster.NumBands;
			bool isAlphaPre = cm.AlphaPremultiplied;
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final boolean isStandard = isStandard(cm, raster);
			bool isStandard = IsStandard(cm, raster);
			ColorSpace cs;

			// Force the raster data alpha state to match the premultiplied
			// state in the color model
			CoerceData(isRasterPremultiplied);

			SampleModel sm = raster.SampleModel;
			cs = cm.ColorSpace;
			int csType = cs.Type;
			if (csType != ColorSpace.TYPE_RGB)
			{
				if (csType == ColorSpace.TYPE_GRAY && isStandard && cm is ComponentColorModel)
				{
					// Check if this might be a child raster (fix for bug 4240596)
					if (sm is ComponentSampleModel && ((ComponentSampleModel)sm).PixelStride != numBands)
					{
						ImageType = TYPE_CUSTOM;
					}
					else if (raster is ByteComponentRaster && raster.NumBands == 1 && cm.GetComponentSize(0) == 8 && ((ByteComponentRaster)raster).PixelStride == 1)
					{
						ImageType = TYPE_BYTE_GRAY;
					}
					else if (raster is ShortComponentRaster && raster.NumBands == 1 && cm.GetComponentSize(0) == 16 && ((ShortComponentRaster)raster).PixelStride == 1)
					{
						ImageType = TYPE_USHORT_GRAY;
					}
				}
				else
				{
					ImageType = TYPE_CUSTOM;
				}
				return;
			}

			if ((raster is IntegerComponentRaster) && (numBands == 3 || numBands == 4))
			{
				IntegerComponentRaster iraster = (IntegerComponentRaster) raster;
				// Check if the raster params and the color model
				// are correct
				int pixSize = cm.PixelSize;
				if (iraster.PixelStride == 1 && isStandard && cm is DirectColorModel && (pixSize == 32 || pixSize == 24))
				{
					// Now check on the DirectColorModel params
					DirectColorModel dcm = (DirectColorModel) cm;
					int rmask = dcm.RedMask;
					int gmask = dcm.GreenMask;
					int bmask = dcm.BlueMask;
					if (rmask == DCM_RED_MASK && gmask == DCM_GREEN_MASK && bmask == DCM_BLUE_MASK)
					{
						if (dcm.AlphaMask == DCM_ALPHA_MASK)
						{
							ImageType = (isAlphaPre ? TYPE_INT_ARGB_PRE : TYPE_INT_ARGB);
						}
						else
						{
							// No Alpha
							if (!dcm.HasAlpha())
							{
								ImageType = TYPE_INT_RGB;
							}
						}
					} // if (dcm.getRedMask() == DCM_RED_MASK &&
					else if (rmask == DCM_BGR_RED_MASK && gmask == DCM_BGR_GRN_MASK && bmask == DCM_BGR_BLU_MASK)
					{
						if (!dcm.HasAlpha())
						{
							ImageType = TYPE_INT_BGR;
						}
					} // if (rmask == DCM_BGR_RED_MASK &&
				} // if (iraster.getPixelStride() == 1
			} // ((raster instanceof IntegerComponentRaster) &&
			else if ((cm is IndexColorModel) && (numBands == 1) && isStandard && (!cm.HasAlpha() || !isAlphaPre))
			{
				IndexColorModel icm = (IndexColorModel) cm;
				int pixSize = icm.PixelSize;

				if (raster is BytePackedRaster)
				{
					ImageType = TYPE_BYTE_BINARY;
				} // if (raster instanceof BytePackedRaster)
				else if (raster is ByteComponentRaster)
				{
					ByteComponentRaster braster = (ByteComponentRaster) raster;
					if (braster.PixelStride == 1 && pixSize <= 8)
					{
						ImageType = TYPE_BYTE_INDEXED;
					}
				}
			} // else if (cm instanceof IndexColorModel) && (numBands == 1))
			else if ((raster is ShortComponentRaster) && (cm is DirectColorModel) && isStandard && (numBands == 3) && !cm.HasAlpha())
			{
				DirectColorModel dcm = (DirectColorModel) cm;
				if (dcm.RedMask == DCM_565_RED_MASK)
				{
					if (dcm.GreenMask == DCM_565_GRN_MASK && dcm.BlueMask == DCM_565_BLU_MASK)
					{
						ImageType = TYPE_USHORT_565_RGB;
					}
				}
				else if (dcm.RedMask == DCM_555_RED_MASK)
				{
					if (dcm.GreenMask == DCM_555_GRN_MASK && dcm.BlueMask == DCM_555_BLU_MASK)
					{
						ImageType = TYPE_USHORT_555_RGB;
					}
				}
			} // else if ((cm instanceof IndexColorModel) && (numBands == 1))
			else if ((raster is ByteComponentRaster) && (cm is ComponentColorModel) && isStandard && (raster.SampleModel is PixelInterleavedSampleModel) && (numBands == 3 || numBands == 4))
			{
				ComponentColorModel ccm = (ComponentColorModel) cm;
				PixelInterleavedSampleModel csm = (PixelInterleavedSampleModel)raster.SampleModel;
				ByteComponentRaster braster = (ByteComponentRaster) raster;
				int[] offs = csm.BandOffsets;
				if (ccm.NumComponents != numBands)
				{
					throw new RasterFormatException("Number of components in " + "ColorModel (" + ccm.NumComponents + ") does not match # in " + " Raster (" + numBands + ")");
				}
				int[] nBits = ccm.ComponentSize;
				bool is8bit = true;
				for (int i = 0; i < numBands; i++)
				{
					if (nBits[i] != 8)
					{
						is8bit = false;
						break;
					}
				}
				if (is8bit && braster.PixelStride == numBands && offs[0] == numBands - 1 && offs[1] == numBands - 2 && offs[2] == numBands - 3)
				{
					if (numBands == 3 && !ccm.HasAlpha())
					{
						ImageType = TYPE_3BYTE_BGR;
					}
					else if (offs[3] == 0 && ccm.HasAlpha())
					{
						ImageType = (isAlphaPre ? TYPE_4BYTE_ABGR_PRE : TYPE_4BYTE_ABGR);
					}
				}
			} // else if ((raster instanceof ByteComponentRaster) &&
		}

		private static bool IsStandard(ColorModel cm, WritableRaster wr)
		{
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final Class cmClass = cm.getClass();
			Class cmClass = cm.GetType();
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final Class wrClass = wr.getClass();
			Class wrClass = wr.GetType();
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final Class smClass = wr.getSampleModel().getClass();
			Class smClass = wr.SampleModel.GetType();

//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final java.security.PrivilegedAction<Boolean> checkClassLoadersAction = new java.security.PrivilegedAction<Boolean>()
			PrivilegedAction<Boolean> checkClassLoadersAction = new PrivilegedActionAnonymousInnerClassHelper(cmClass, wrClass, smClass);
			return AccessController.doPrivileged(checkClassLoadersAction);
		}

		private class PrivilegedActionAnonymousInnerClassHelper : PrivilegedAction<Boolean>
		{
			private Type CmClass;
			private Type WrClass;
			private Type SmClass;

			public PrivilegedActionAnonymousInnerClassHelper(Type cmClass, Type wrClass, Type smClass)
			{
				this.CmClass = cmClass;
				this.WrClass = wrClass;
				this.SmClass = smClass;
			}


			public virtual Boolean Run()
			{
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final ClassLoader std = System.class.getClassLoader();
				ClassLoader std = typeof(System).ClassLoader;

				return (CmClass.ClassLoader == std) && (SmClass.ClassLoader == std) && (WrClass.ClassLoader == std);
			}
		}

		/// <summary>
		/// Returns the image type.  If it is not one of the known types,
		/// TYPE_CUSTOM is returned. </summary>
		/// <returns> the image type of this <code>BufferedImage</code>. </returns>
		/// <seealso cref= #TYPE_INT_RGB </seealso>
		/// <seealso cref= #TYPE_INT_ARGB </seealso>
		/// <seealso cref= #TYPE_INT_ARGB_PRE </seealso>
		/// <seealso cref= #TYPE_INT_BGR </seealso>
		/// <seealso cref= #TYPE_3BYTE_BGR </seealso>
		/// <seealso cref= #TYPE_4BYTE_ABGR </seealso>
		/// <seealso cref= #TYPE_4BYTE_ABGR_PRE </seealso>
		/// <seealso cref= #TYPE_BYTE_GRAY </seealso>
		/// <seealso cref= #TYPE_BYTE_BINARY </seealso>
		/// <seealso cref= #TYPE_BYTE_INDEXED </seealso>
		/// <seealso cref= #TYPE_USHORT_GRAY </seealso>
		/// <seealso cref= #TYPE_USHORT_565_RGB </seealso>
		/// <seealso cref= #TYPE_USHORT_555_RGB </seealso>
		/// <seealso cref= #TYPE_CUSTOM </seealso>
		public virtual int Type
		{
			get
			{
				return ImageType;
			}
		}

		/// <summary>
		/// Returns the <code>ColorModel</code>. </summary>
		/// <returns> the <code>ColorModel</code> of this
		///  <code>BufferedImage</code>. </returns>
		public virtual ColorModel ColorModel
		{
			get
			{
				return ColorModel_Renamed;
			}
		}

		/// <summary>
		/// Returns the <seealso cref="WritableRaster"/>. </summary>
		/// <returns> the <code>WriteableRaster</code> of this
		///  <code>BufferedImage</code>. </returns>
		public virtual WritableRaster Raster
		{
			get
			{
				return Raster_Renamed;
			}
		}


		/// <summary>
		/// Returns a <code>WritableRaster</code> representing the alpha
		/// channel for <code>BufferedImage</code> objects
		/// with <code>ColorModel</code> objects that support a separate
		/// spatial alpha channel, such as <code>ComponentColorModel</code> and
		/// <code>DirectColorModel</code>.  Returns <code>null</code> if there
		/// is no alpha channel associated with the <code>ColorModel</code> in
		/// this image.  This method assumes that for all
		/// <code>ColorModel</code> objects other than
		/// <code>IndexColorModel</code>, if the <code>ColorModel</code>
		/// supports alpha, there is a separate alpha channel
		/// which is stored as the last band of image data.
		/// If the image uses an <code>IndexColorModel</code> that
		/// has alpha in the lookup table, this method returns
		/// <code>null</code> since there is no spatially discrete alpha
		/// channel.  This method creates a new
		/// <code>WritableRaster</code>, but shares the data array. </summary>
		/// <returns> a <code>WritableRaster</code> or <code>null</code> if this
		///          <code>BufferedImage</code> has no alpha channel associated
		///          with its <code>ColorModel</code>. </returns>
		public virtual WritableRaster AlphaRaster
		{
			get
			{
				return ColorModel_Renamed.GetAlphaRaster(Raster_Renamed);
			}
		}

		/// <summary>
		/// Returns an integer pixel in the default RGB color model
		/// (TYPE_INT_ARGB) and default sRGB colorspace.  Color
		/// conversion takes place if this default model does not match
		/// the image <code>ColorModel</code>.  There are only 8-bits of
		/// precision for each color component in the returned data when using
		/// this method.
		/// 
		/// <para>
		/// 
		/// An <code>ArrayOutOfBoundsException</code> may be thrown
		/// if the coordinates are not in bounds.
		/// However, explicit bounds checking is not guaranteed.
		/// 
		/// </para>
		/// </summary>
		/// <param name="x"> the X coordinate of the pixel from which to get
		///          the pixel in the default RGB color model and sRGB
		///          color space </param>
		/// <param name="y"> the Y coordinate of the pixel from which to get
		///          the pixel in the default RGB color model and sRGB
		///          color space </param>
		/// <returns> an integer pixel in the default RGB color model and
		///          default sRGB colorspace. </returns>
		/// <seealso cref= #setRGB(int, int, int) </seealso>
		/// <seealso cref= #setRGB(int, int, int, int, int[], int, int) </seealso>
		public virtual int GetRGB(int x, int y)
		{
			return ColorModel_Renamed.GetRGB(Raster_Renamed.GetDataElements(x, y, null));
		}

		/// <summary>
		/// Returns an array of integer pixels in the default RGB color model
		/// (TYPE_INT_ARGB) and default sRGB color space,
		/// from a portion of the image data.  Color conversion takes
		/// place if the default model does not match the image
		/// <code>ColorModel</code>.  There are only 8-bits of precision for
		/// each color component in the returned data when
		/// using this method.  With a specified coordinate (x,&nbsp;y) in the
		/// image, the ARGB pixel can be accessed in this way:
		/// 
		/// <pre>
		///    pixel   = rgbArray[offset + (y-startY)*scansize + (x-startX)]; </pre>
		/// 
		/// <para>
		/// 
		/// An <code>ArrayOutOfBoundsException</code> may be thrown
		/// if the region is not in bounds.
		/// However, explicit bounds checking is not guaranteed.
		/// 
		/// </para>
		/// </summary>
		/// <param name="startX">      the starting X coordinate </param>
		/// <param name="startY">      the starting Y coordinate </param>
		/// <param name="w">           width of region </param>
		/// <param name="h">           height of region </param>
		/// <param name="rgbArray">    if not <code>null</code>, the rgb pixels are
		///          written here </param>
		/// <param name="offset">      offset into the <code>rgbArray</code> </param>
		/// <param name="scansize">    scanline stride for the <code>rgbArray</code> </param>
		/// <returns>            array of RGB pixels. </returns>
		/// <seealso cref= #setRGB(int, int, int) </seealso>
		/// <seealso cref= #setRGB(int, int, int, int, int[], int, int) </seealso>
		public virtual int[] GetRGB(int startX, int startY, int w, int h, int[] rgbArray, int offset, int scansize)
		{
			int yoff = offset;
			int off;
			Object data;
			int nbands = Raster_Renamed.NumBands;
			int dataType = Raster_Renamed.DataBuffer.DataType;
			switch (dataType)
			{
			case DataBuffer.TYPE_BYTE:
				data = new sbyte[nbands];
				break;
			case DataBuffer.TYPE_USHORT:
				data = new short[nbands];
				break;
			case DataBuffer.TYPE_INT:
				data = new int[nbands];
				break;
			case DataBuffer.TYPE_FLOAT:
				data = new float[nbands];
				break;
			case DataBuffer.TYPE_DOUBLE:
				data = new double[nbands];
				break;
			default:
				throw new IllegalArgumentException("Unknown data buffer type: " + dataType);
			}

			if (rgbArray == null)
			{
				rgbArray = new int[offset + h * scansize];
			}

			for (int y = startY; y < startY + h; y++, yoff += scansize)
			{
				off = yoff;
				for (int x = startX; x < startX + w; x++)
				{
					rgbArray[off++] = ColorModel_Renamed.GetRGB(Raster_Renamed.GetDataElements(x, y, data));
				}
			}

			return rgbArray;
		}


		/// <summary>
		/// Sets a pixel in this <code>BufferedImage</code> to the specified
		/// RGB value. The pixel is assumed to be in the default RGB color
		/// model, TYPE_INT_ARGB, and default sRGB color space.  For images
		/// with an <code>IndexColorModel</code>, the index with the nearest
		/// color is chosen.
		/// 
		/// <para>
		/// 
		/// An <code>ArrayOutOfBoundsException</code> may be thrown
		/// if the coordinates are not in bounds.
		/// However, explicit bounds checking is not guaranteed.
		/// 
		/// </para>
		/// </summary>
		/// <param name="x"> the X coordinate of the pixel to set </param>
		/// <param name="y"> the Y coordinate of the pixel to set </param>
		/// <param name="rgb"> the RGB value </param>
		/// <seealso cref= #getRGB(int, int) </seealso>
		/// <seealso cref= #getRGB(int, int, int, int, int[], int, int) </seealso>
		public virtual void SetRGB(int x, int y, int rgb)
		{
			lock (this)
			{
				Raster_Renamed.SetDataElements(x, y, ColorModel_Renamed.GetDataElements(rgb, null));
			}
		}

		/// <summary>
		/// Sets an array of integer pixels in the default RGB color model
		/// (TYPE_INT_ARGB) and default sRGB color space,
		/// into a portion of the image data.  Color conversion takes place
		/// if the default model does not match the image
		/// <code>ColorModel</code>.  There are only 8-bits of precision for
		/// each color component in the returned data when
		/// using this method.  With a specified coordinate (x,&nbsp;y) in the
		/// this image, the ARGB pixel can be accessed in this way:
		/// <pre>
		///    pixel   = rgbArray[offset + (y-startY)*scansize + (x-startX)];
		/// </pre>
		/// WARNING: No dithering takes place.
		/// 
		/// <para>
		/// 
		/// An <code>ArrayOutOfBoundsException</code> may be thrown
		/// if the region is not in bounds.
		/// However, explicit bounds checking is not guaranteed.
		/// 
		/// </para>
		/// </summary>
		/// <param name="startX">      the starting X coordinate </param>
		/// <param name="startY">      the starting Y coordinate </param>
		/// <param name="w">           width of the region </param>
		/// <param name="h">           height of the region </param>
		/// <param name="rgbArray">    the rgb pixels </param>
		/// <param name="offset">      offset into the <code>rgbArray</code> </param>
		/// <param name="scansize">    scanline stride for the <code>rgbArray</code> </param>
		/// <seealso cref= #getRGB(int, int) </seealso>
		/// <seealso cref= #getRGB(int, int, int, int, int[], int, int) </seealso>
		public virtual void SetRGB(int startX, int startY, int w, int h, int[] rgbArray, int offset, int scansize)
		{
			int yoff = offset;
			int off;
			Object pixel = null;

			for (int y = startY; y < startY + h; y++, yoff += scansize)
			{
				off = yoff;
				for (int x = startX; x < startX + w; x++)
				{
					pixel = ColorModel_Renamed.GetDataElements(rgbArray[off++], pixel);
					Raster_Renamed.SetDataElements(x, y, pixel);
				}
			}
		}


		/// <summary>
		/// Returns the width of the <code>BufferedImage</code>. </summary>
		/// <returns> the width of this <code>BufferedImage</code> </returns>
		public virtual int Width
		{
			get
			{
				return Raster_Renamed.Width;
			}
		}

		/// <summary>
		/// Returns the height of the <code>BufferedImage</code>. </summary>
		/// <returns> the height of this <code>BufferedImage</code> </returns>
		public virtual int Height
		{
			get
			{
				return Raster_Renamed.Height;
			}
		}

		/// <summary>
		/// Returns the width of the <code>BufferedImage</code>. </summary>
		/// <param name="observer"> ignored </param>
		/// <returns> the width of this <code>BufferedImage</code> </returns>
		public override int GetWidth(ImageObserver observer)
		{
			return Raster_Renamed.Width;
		}

		/// <summary>
		/// Returns the height of the <code>BufferedImage</code>. </summary>
		/// <param name="observer"> ignored </param>
		/// <returns> the height of this <code>BufferedImage</code> </returns>
		public override int GetHeight(ImageObserver observer)
		{
			return Raster_Renamed.Height;
		}

		/// <summary>
		/// Returns the object that produces the pixels for the image. </summary>
		/// <returns> the <seealso cref="ImageProducer"/> that is used to produce the
		/// pixels for this image. </returns>
		/// <seealso cref= ImageProducer </seealso>
		public override ImageProducer Source
		{
			get
			{
				if (Osis == null)
				{
					if (Properties == null)
					{
						Properties = new Hashtable();
					}
					Osis = new OffScreenImageSource(this, Properties);
				}
				return Osis;
			}
		}


		/// <summary>
		/// Returns a property of the image by name.  Individual property names
		/// are defined by the various image formats.  If a property is not
		/// defined for a particular image, this method returns the
		/// <code>UndefinedProperty</code> field.  If the properties
		/// for this image are not yet known, then this method returns
		/// <code>null</code> and the <code>ImageObserver</code> object is
		/// notified later.  The property name "comment" should be used to
		/// store an optional comment that can be presented to the user as a
		/// description of the image, its source, or its author. </summary>
		/// <param name="name"> the property name </param>
		/// <param name="observer"> the <code>ImageObserver</code> that receives
		///  notification regarding image information </param>
		/// <returns> an <seealso cref="Object"/> that is the property referred to by the
		///          specified <code>name</code> or <code>null</code> if the
		///          properties of this image are not yet known. </returns>
		/// <exception cref="NullPointerException"> if the property name is null. </exception>
		/// <seealso cref= ImageObserver </seealso>
		/// <seealso cref= java.awt.Image#UndefinedProperty </seealso>
		public override Object GetProperty(String name, ImageObserver observer)
		{
			return GetProperty(name);
		}

		/// <summary>
		/// Returns a property of the image by name. </summary>
		/// <param name="name"> the property name </param>
		/// <returns> an <code>Object</code> that is the property referred to by
		///          the specified <code>name</code>. </returns>
		/// <exception cref="NullPointerException"> if the property name is null. </exception>
		public virtual Object GetProperty(String name)
		{
			if (name == null)
			{
				throw new NullPointerException("null property name is not allowed");
			}
			if (Properties == null)
			{
				return java.awt.Image.UndefinedProperty;
			}
			Object o = Properties[name];
			if (o == null)
			{
				o = java.awt.Image.UndefinedProperty;
			}
			return o;
		}

		/// <summary>
		/// This method returns a <seealso cref="Graphics2D"/>, but is here
		/// for backwards compatibility.  <seealso cref="#createGraphics() createGraphics"/> is more
		/// convenient, since it is declared to return a
		/// <code>Graphics2D</code>. </summary>
		/// <returns> a <code>Graphics2D</code>, which can be used to draw into
		///          this image. </returns>
		public override java.awt.Graphics Graphics
		{
			get
			{
				return CreateGraphics();
			}
		}

		/// <summary>
		/// Creates a <code>Graphics2D</code>, which can be used to draw into
		/// this <code>BufferedImage</code>. </summary>
		/// <returns> a <code>Graphics2D</code>, used for drawing into this
		///          image. </returns>
		public virtual Graphics2D CreateGraphics()
		{
			GraphicsEnvironment env = GraphicsEnvironment.LocalGraphicsEnvironment;
			return env.CreateGraphics(this);
		}

		/// <summary>
		/// Returns a subimage defined by a specified rectangular region.
		/// The returned <code>BufferedImage</code> shares the same
		/// data array as the original image. </summary>
		/// <param name="x"> the X coordinate of the upper-left corner of the
		///          specified rectangular region </param>
		/// <param name="y"> the Y coordinate of the upper-left corner of the
		///          specified rectangular region </param>
		/// <param name="w"> the width of the specified rectangular region </param>
		/// <param name="h"> the height of the specified rectangular region </param>
		/// <returns> a <code>BufferedImage</code> that is the subimage of this
		///          <code>BufferedImage</code>. </returns>
		/// <exception cref="RasterFormatException"> if the specified
		/// area is not contained within this <code>BufferedImage</code>. </exception>
		public virtual BufferedImage GetSubimage(int x, int y, int w, int h)
		{
			return new BufferedImage(ColorModel_Renamed, Raster_Renamed.CreateWritableChild(x, y, w, h, 0, 0, null), ColorModel_Renamed.AlphaPremultiplied, Properties);
		}

		/// <summary>
		/// Returns whether or not the alpha has been premultiplied.  It
		/// returns <code>false</code> if there is no alpha. </summary>
		/// <returns> <code>true</code> if the alpha has been premultiplied;
		///          <code>false</code> otherwise. </returns>
		public virtual bool AlphaPremultiplied
		{
			get
			{
				return ColorModel_Renamed.AlphaPremultiplied;
			}
		}

		/// <summary>
		/// Forces the data to match the state specified in the
		/// <code>isAlphaPremultiplied</code> variable.  It may multiply or
		/// divide the color raster data by alpha, or do nothing if the data is
		/// in the correct state. </summary>
		/// <param name="isAlphaPremultiplied"> <code>true</code> if the alpha has been
		///          premultiplied; <code>false</code> otherwise. </param>
		public virtual void CoerceData(bool isAlphaPremultiplied)
		{
			if (ColorModel_Renamed.HasAlpha() && ColorModel_Renamed.AlphaPremultiplied != isAlphaPremultiplied)
			{
				// Make the color model do the conversion
				ColorModel_Renamed = ColorModel_Renamed.CoerceData(Raster_Renamed, isAlphaPremultiplied);
			}
		}

		/// <summary>
		/// Returns a <code>String</code> representation of this
		/// <code>BufferedImage</code> object and its values. </summary>
		/// <returns> a <code>String</code> representing this
		///          <code>BufferedImage</code>. </returns>
		public override String ToString()
		{
			return "BufferedImage@" + GetHashCode().ToString("x") + ": type = " + ImageType + " " + ColorModel_Renamed + " " + Raster_Renamed;
		}

		/// <summary>
		/// Returns a <seealso cref="Vector"/> of <seealso cref="RenderedImage"/> objects that are
		/// the immediate sources, not the sources of these immediate sources,
		/// of image data for this <code>BufferedImage</code>.  This
		/// method returns <code>null</code> if the <code>BufferedImage</code>
		/// has no information about its immediate sources.  It returns an
		/// empty <code>Vector</code> if the <code>BufferedImage</code> has no
		/// immediate sources. </summary>
		/// <returns> a <code>Vector</code> containing immediate sources of
		///          this <code>BufferedImage</code> object's image date, or
		///          <code>null</code> if this <code>BufferedImage</code> has
		///          no information about its immediate sources, or an empty
		///          <code>Vector</code> if this <code>BufferedImage</code>
		///          has no immediate sources. </returns>
		public virtual List<RenderedImage> Sources
		{
			get
			{
				return null;
			}
		}

		/// <summary>
		/// Returns an array of names recognized by
		/// <seealso cref="#getProperty(String) getProperty(String)"/>
		/// or <code>null</code>, if no property names are recognized. </summary>
		/// <returns> a <code>String</code> array containing all of the property
		///          names that <code>getProperty(String)</code> recognizes;
		///          or <code>null</code> if no property names are recognized. </returns>
		public virtual String[] PropertyNames
		{
			get
			{
				if (Properties == null || Properties.Count == 0)
				{
					return null;
				}
	//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
	//ORIGINAL LINE: final java.util.Set<String> keys = properties.keySet();
				Dictionary<String, Object>.KeyCollection keys = Properties.Keys;
				return keys.toArray(new String[keys.size()]);
			}
		}

		/// <summary>
		/// Returns the minimum x coordinate of this
		/// <code>BufferedImage</code>.  This is always zero. </summary>
		/// <returns> the minimum x coordinate of this
		///          <code>BufferedImage</code>. </returns>
		public virtual int MinX
		{
			get
			{
				return Raster_Renamed.MinX;
			}
		}

		/// <summary>
		/// Returns the minimum y coordinate of this
		/// <code>BufferedImage</code>.  This is always zero. </summary>
		/// <returns> the minimum y coordinate of this
		///          <code>BufferedImage</code>. </returns>
		public virtual int MinY
		{
			get
			{
				return Raster_Renamed.MinY;
			}
		}

		/// <summary>
		/// Returns the <code>SampleModel</code> associated with this
		/// <code>BufferedImage</code>. </summary>
		/// <returns> the <code>SampleModel</code> of this
		///          <code>BufferedImage</code>. </returns>
		public virtual SampleModel SampleModel
		{
			get
			{
				return Raster_Renamed.SampleModel;
			}
		}

		/// <summary>
		/// Returns the number of tiles in the x direction.
		/// This is always one. </summary>
		/// <returns> the number of tiles in the x direction. </returns>
		public virtual int NumXTiles
		{
			get
			{
				return 1;
			}
		}

		/// <summary>
		/// Returns the number of tiles in the y direction.
		/// This is always one. </summary>
		/// <returns> the number of tiles in the y direction. </returns>
		public virtual int NumYTiles
		{
			get
			{
				return 1;
			}
		}

		/// <summary>
		/// Returns the minimum tile index in the x direction.
		/// This is always zero. </summary>
		/// <returns> the minimum tile index in the x direction. </returns>
		public virtual int MinTileX
		{
			get
			{
				return 0;
			}
		}

		/// <summary>
		/// Returns the minimum tile index in the y direction.
		/// This is always zero. </summary>
		/// <returns> the minimum tile index in the y direction. </returns>
		public virtual int MinTileY
		{
			get
			{
				return 0;
			}
		}

		/// <summary>
		/// Returns the tile width in pixels. </summary>
		/// <returns> the tile width in pixels. </returns>
		public virtual int TileWidth
		{
			get
			{
			   return Raster_Renamed.Width;
			}
		}

		/// <summary>
		/// Returns the tile height in pixels. </summary>
		/// <returns> the tile height in pixels. </returns>
		public virtual int TileHeight
		{
			get
			{
			   return Raster_Renamed.Height;
			}
		}

		/// <summary>
		/// Returns the x offset of the tile grid relative to the origin,
		/// For example, the x coordinate of the location of tile
		/// (0,&nbsp;0).  This is always zero. </summary>
		/// <returns> the x offset of the tile grid. </returns>
		public virtual int TileGridXOffset
		{
			get
			{
				return Raster_Renamed.SampleModelTranslateX;
			}
		}

		/// <summary>
		/// Returns the y offset of the tile grid relative to the origin,
		/// For example, the y coordinate of the location of tile
		/// (0,&nbsp;0).  This is always zero. </summary>
		/// <returns> the y offset of the tile grid. </returns>
		public virtual int TileGridYOffset
		{
			get
			{
				return Raster_Renamed.SampleModelTranslateY;
			}
		}

		/// <summary>
		/// Returns tile (<code>tileX</code>,&nbsp;<code>tileY</code>).  Note
		/// that <code>tileX</code> and <code>tileY</code> are indices
		/// into the tile array, not pixel locations.  The <code>Raster</code>
		/// that is returned is live, which means that it is updated if the
		/// image is changed. </summary>
		/// <param name="tileX"> the x index of the requested tile in the tile array </param>
		/// <param name="tileY"> the y index of the requested tile in the tile array </param>
		/// <returns> a <code>Raster</code> that is the tile defined by the
		///          arguments <code>tileX</code> and <code>tileY</code>. </returns>
		/// <exception cref="ArrayIndexOutOfBoundsException"> if both
		///          <code>tileX</code> and <code>tileY</code> are not
		///          equal to 0 </exception>
		public virtual Raster GetTile(int tileX, int tileY)
		{
			if (tileX == 0 && tileY == 0)
			{
				return Raster_Renamed;
			}
			throw new ArrayIndexOutOfBoundsException("BufferedImages only have" + " one tile with index 0,0");
		}

		/// <summary>
		/// Returns the image as one large tile.  The <code>Raster</code>
		/// returned is a copy of the image data is not updated if the
		/// image is changed. </summary>
		/// <returns> a <code>Raster</code> that is a copy of the image data. </returns>
		/// <seealso cref= #setData(Raster) </seealso>
		public virtual Raster Data
		{
			get
			{
    
				// REMIND : this allocates a whole new tile if raster is a
				// subtile.  (It only copies in the requested area)
				// We should do something smarter.
				int width = Raster_Renamed.Width;
				int height = Raster_Renamed.Height;
				int startX = Raster_Renamed.MinX;
				int startY = Raster_Renamed.MinY;
				WritableRaster wr = Raster.CreateWritableRaster(Raster_Renamed.SampleModel, new Point(Raster_Renamed.SampleModelTranslateX, Raster_Renamed.SampleModelTranslateY));
    
				Object tdata = null;
    
				for (int i = startY; i < startY + height; i++)
				{
					tdata = Raster_Renamed.GetDataElements(startX,i,width,1,tdata);
					wr.SetDataElements(startX,i,width,1, tdata);
				}
				return wr;
			}
			set
			{
				int width = value.Width;
				int height = value.Height;
				int startX = value.MinX;
				int startY = value.MinY;
    
				int[] tdata = null;
    
				// Clip to the current Raster
				Rectangle rclip = new Rectangle(startX, startY, width, height);
				Rectangle bclip = new Rectangle(0, 0, Raster_Renamed.Width_Renamed, Raster_Renamed.Height_Renamed);
				Rectangle intersect = rclip.Intersection(bclip);
				if (intersect.Empty)
				{
					return;
				}
				width = intersect.Width_Renamed;
				height = intersect.Height_Renamed;
				startX = intersect.x;
				startY = intersect.y;
    
				// remind use get/setDataElements for speed if Rasters are
				// compatible
				for (int i = startY; i < startY + height; i++)
				{
					tdata = value.GetPixels(startX,i,width,1,tdata);
					Raster_Renamed.SetPixels(startX,i,width,1, tdata);
				}
			}
		}

		/// <summary>
		/// Computes and returns an arbitrary region of the
		/// <code>BufferedImage</code>.  The <code>Raster</code> returned is a
		/// copy of the image data and is not updated if the image is
		/// changed. </summary>
		/// <param name="rect"> the region of the <code>BufferedImage</code> to be
		/// returned. </param>
		/// <returns> a <code>Raster</code> that is a copy of the image data of
		///          the specified region of the <code>BufferedImage</code> </returns>
		/// <seealso cref= #setData(Raster) </seealso>
		public virtual Raster GetData(Rectangle rect)
		{
			SampleModel sm = Raster_Renamed.SampleModel;
			SampleModel nsm = sm.CreateCompatibleSampleModel(rect.Width_Renamed, rect.Height_Renamed);
			WritableRaster wr = Raster.CreateWritableRaster(nsm, rect.Location);
			int width = rect.Width_Renamed;
			int height = rect.Height_Renamed;
			int startX = rect.x;
			int startY = rect.y;

			Object tdata = null;

			for (int i = startY; i < startY + height; i++)
			{
				tdata = Raster_Renamed.GetDataElements(startX,i,width,1,tdata);
				wr.SetDataElements(startX,i,width,1, tdata);
			}
			return wr;
		}

		/// <summary>
		/// Computes an arbitrary rectangular region of the
		/// <code>BufferedImage</code> and copies it into a specified
		/// <code>WritableRaster</code>.  The region to be computed is
		/// determined from the bounds of the specified
		/// <code>WritableRaster</code>.  The specified
		/// <code>WritableRaster</code> must have a
		/// <code>SampleModel</code> that is compatible with this image.  If
		/// <code>outRaster</code> is <code>null</code>,
		/// an appropriate <code>WritableRaster</code> is created. </summary>
		/// <param name="outRaster"> a <code>WritableRaster</code> to hold the returned
		///          part of the image, or <code>null</code> </param>
		/// <returns> a reference to the supplied or created
		///          <code>WritableRaster</code>. </returns>
		public virtual WritableRaster CopyData(WritableRaster outRaster)
		{
			if (outRaster == null)
			{
				return (WritableRaster) Data;
			}
			int width = outRaster.Width;
			int height = outRaster.Height;
			int startX = outRaster.MinX;
			int startY = outRaster.MinY;

			Object tdata = null;

			for (int i = startY; i < startY + height; i++)
			{
				tdata = Raster_Renamed.GetDataElements(startX,i,width,1,tdata);
				outRaster.SetDataElements(startX,i,width,1, tdata);
			}

			return outRaster;
		}



	  /// <summary>
	  /// Adds a tile observer.  If the observer is already present,
	  /// it receives multiple notifications. </summary>
	  /// <param name="to"> the specified <seealso cref="TileObserver"/> </param>
		public virtual void AddTileObserver(TileObserver to)
		{
		}

	  /// <summary>
	  /// Removes a tile observer.  If the observer was not registered,
	  /// nothing happens.  If the observer was registered for multiple
	  /// notifications, it is now registered for one fewer notification. </summary>
	  /// <param name="to"> the specified <code>TileObserver</code>. </param>
		public virtual void RemoveTileObserver(TileObserver to)
		{
		}

		/// <summary>
		/// Returns whether or not a tile is currently checked out for writing. </summary>
		/// <param name="tileX"> the x index of the tile. </param>
		/// <param name="tileY"> the y index of the tile. </param>
		/// <returns> <code>true</code> if the tile specified by the specified
		///          indices is checked out for writing; <code>false</code>
		///          otherwise. </returns>
		/// <exception cref="ArrayIndexOutOfBoundsException"> if both
		///          <code>tileX</code> and <code>tileY</code> are not equal
		///          to 0 </exception>
		public virtual bool IsTileWritable(int tileX, int tileY)
		{
			if (tileX == 0 && tileY == 0)
			{
				return true;
			}
			throw new IllegalArgumentException("Only 1 tile in image");
		}

		/// <summary>
		/// Returns an array of <seealso cref="Point"/> objects indicating which tiles
		/// are checked out for writing.  Returns <code>null</code> if none are
		/// checked out. </summary>
		/// <returns> a <code>Point</code> array that indicates the tiles that
		///          are checked out for writing, or <code>null</code> if no
		///          tiles are checked out for writing. </returns>
		public virtual Point[] WritableTileIndices
		{
			get
			{
				Point[] p = new Point[1];
				p[0] = new Point(0, 0);
    
				return p;
			}
		}

		/// <summary>
		/// Returns whether or not any tile is checked out for writing.
		/// Semantically equivalent to
		/// <pre>
		/// (getWritableTileIndices() != null).
		/// </pre> </summary>
		/// <returns> <code>true</code> if any tile is checked out for writing;
		///          <code>false</code> otherwise. </returns>
		public virtual bool HasTileWriters()
		{
			return true;
		}

	  /// <summary>
	  /// Checks out a tile for writing.  All registered
	  /// <code>TileObservers</code> are notified when a tile goes from having
	  /// no writers to having one writer. </summary>
	  /// <param name="tileX"> the x index of the tile </param>
	  /// <param name="tileY"> the y index of the tile </param>
	  /// <returns> a <code>WritableRaster</code> that is the tile, indicated by
	  ///            the specified indices, to be checked out for writing. </returns>
		public virtual WritableRaster GetWritableTile(int tileX, int tileY)
		{
			return Raster_Renamed;
		}

	  /// <summary>
	  /// Relinquishes permission to write to a tile.  If the caller
	  /// continues to write to the tile, the results are undefined.
	  /// Calls to this method should only appear in matching pairs
	  /// with calls to <seealso cref="#getWritableTile(int, int) getWritableTile(int, int)"/>.  Any other leads
	  /// to undefined results.  All registered <code>TileObservers</code>
	  /// are notified when a tile goes from having one writer to having no
	  /// writers. </summary>
	  /// <param name="tileX"> the x index of the tile </param>
	  /// <param name="tileY"> the y index of the tile </param>
		public virtual void ReleaseWritableTile(int tileX, int tileY)
		{
		}

		/// <summary>
		/// Returns the transparency.  Returns either OPAQUE, BITMASK,
		/// or TRANSLUCENT. </summary>
		/// <returns> the transparency of this <code>BufferedImage</code>. </returns>
		/// <seealso cref= Transparency#OPAQUE </seealso>
		/// <seealso cref= Transparency#BITMASK </seealso>
		/// <seealso cref= Transparency#TRANSLUCENT
		/// @since 1.5 </seealso>
		public virtual int Transparency
		{
			get
			{
				return ColorModel_Renamed.Transparency;
			}
		}
	}

}
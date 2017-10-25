/*
 * Copyright (c) 1997, 2000, Oracle and/or its affiliates. All rights reserved.
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

	using ImagingLib = sun.awt.image.ImagingLib;

	/// <summary>
	/// This class implements a lookup operation from the source
	/// to the destination.  The LookupTable object may contain a single array
	/// or multiple arrays, subject to the restrictions below.
	/// <para>
	/// For Rasters, the lookup operates on bands.  The number of
	/// lookup arrays may be one, in which case the same array is
	/// applied to all bands, or it must equal the number of Source
	/// Raster bands.
	/// </para>
	/// <para>
	/// For BufferedImages, the lookup operates on color and alpha components.
	/// The number of lookup arrays may be one, in which case the
	/// same array is applied to all color (but not alpha) components.
	/// Otherwise, the number of lookup arrays may
	/// equal the number of Source color components, in which case no
	/// lookup of the alpha component (if present) is performed.
	/// If neither of these cases apply, the number of lookup arrays
	/// must equal the number of Source color components plus alpha components,
	/// in which case lookup is performed for all color and alpha components.
	/// This allows non-uniform rescaling of multi-band BufferedImages.
	/// </para>
	/// <para>
	/// BufferedImage sources with premultiplied alpha data are treated in the same
	/// manner as non-premultiplied images for purposes of the lookup.  That is,
	/// the lookup is done per band on the raw data of the BufferedImage source
	/// without regard to whether the data is premultiplied.  If a color conversion
	/// is required to the destination ColorModel, the premultiplied state of
	/// both source and destination will be taken into account for this step.
	/// </para>
	/// <para>
	/// Images with an IndexColorModel cannot be used.
	/// </para>
	/// <para>
	/// If a RenderingHints object is specified in the constructor, the
	/// color rendering hint and the dithering hint may be used when color
	/// conversion is required.
	/// </para>
	/// <para>
	/// This class allows the Source to be the same as the Destination.
	/// 
	/// </para>
	/// </summary>
	/// <seealso cref= LookupTable </seealso>
	/// <seealso cref= java.awt.RenderingHints#KEY_COLOR_RENDERING </seealso>
	/// <seealso cref= java.awt.RenderingHints#KEY_DITHERING </seealso>

	public class LookupOp : BufferedImageOp, RasterOp
	{
		private LookupTable Ltable;
		private int NumComponents;
		internal RenderingHints Hints;

		/// <summary>
		/// Constructs a <code>LookupOp</code> object given the lookup
		/// table and a <code>RenderingHints</code> object, which might
		/// be <code>null</code>. </summary>
		/// <param name="lookup"> the specified <code>LookupTable</code> </param>
		/// <param name="hints"> the specified <code>RenderingHints</code>,
		///        or <code>null</code> </param>
		public LookupOp(LookupTable lookup, RenderingHints hints)
		{
			this.Ltable = lookup;
			this.Hints = hints;
			NumComponents = Ltable.NumComponents;
		}

		/// <summary>
		/// Returns the <code>LookupTable</code>. </summary>
		/// <returns> the <code>LookupTable</code> of this
		///         <code>LookupOp</code>. </returns>
		public LookupTable Table
		{
			get
			{
				return Ltable;
			}
		}

		/// <summary>
		/// Performs a lookup operation on a <code>BufferedImage</code>.
		/// If the color model in the source image is not the same as that
		/// in the destination image, the pixels will be converted
		/// in the destination.  If the destination image is <code>null</code>,
		/// a <code>BufferedImage</code> will be created with an appropriate
		/// <code>ColorModel</code>.  An <code>IllegalArgumentException</code>
		/// might be thrown if the number of arrays in the
		/// <code>LookupTable</code> does not meet the restrictions
		/// stated in the class comment above, or if the source image
		/// has an <code>IndexColorModel</code>. </summary>
		/// <param name="src"> the <code>BufferedImage</code> to be filtered </param>
		/// <param name="dst"> the <code>BufferedImage</code> in which to
		///            store the results of the filter operation </param>
		/// <returns> the filtered <code>BufferedImage</code>. </returns>
		/// <exception cref="IllegalArgumentException"> if the number of arrays in the
		///         <code>LookupTable</code> does not meet the restrictions
		///         described in the class comments, or if the source image
		///         has an <code>IndexColorModel</code>. </exception>
		public BufferedImage Filter(BufferedImage src, BufferedImage dst)
		{
			ColorModel srcCM = src.ColorModel;
			int numBands = srcCM.NumColorComponents;
			ColorModel dstCM;
			if (srcCM is IndexColorModel)
			{
				throw new IllegalArgumentException("LookupOp cannot be " + "performed on an indexed image");
			}
			int numComponents = Ltable.NumComponents;
			if (numComponents != 1 && numComponents != srcCM.NumComponents && numComponents != srcCM.NumColorComponents)
			{
				throw new IllegalArgumentException("Number of arrays in the " + " lookup table (" + numComponents + " is not compatible with the " + " src image: " + src);
			}


			bool needToConvert = false;

			int width = src.Width;
			int height = src.Height;

			if (dst == null)
			{
				dst = CreateCompatibleDestImage(src, null);
				dstCM = srcCM;
			}
			else
			{
				if (width != dst.Width)
				{
					throw new IllegalArgumentException("Src width (" + width + ") not equal to dst width (" + dst.Width + ")");
				}
				if (height != dst.Height)
				{
					throw new IllegalArgumentException("Src height (" + height + ") not equal to dst height (" + dst.Height + ")");
				}

				dstCM = dst.ColorModel;
				if (srcCM.ColorSpace.Type != dstCM.ColorSpace.Type)
				{
					needToConvert = true;
					dst = CreateCompatibleDestImage(src, null);
				}

			}

			BufferedImage origDst = dst;

			if (ImagingLib.filter(this, src, dst) == null)
			{
				// Do it the slow way
				WritableRaster srcRaster = src.Raster;
				WritableRaster dstRaster = dst.Raster;

				if (srcCM.HasAlpha())
				{
					if (numBands - 1 == numComponents || numComponents == 1)
					{
						int minx = srcRaster.MinX;
						int miny = srcRaster.MinY;
						int[] bands = new int[numBands - 1];
						for (int i = 0; i < numBands - 1; i++)
						{
							bands[i] = i;
						}
						srcRaster = srcRaster.CreateWritableChild(minx, miny, srcRaster.Width, srcRaster.Height, minx, miny, bands);
					}
				}
				if (dstCM.HasAlpha())
				{
					int dstNumBands = dstRaster.NumBands;
					if (dstNumBands - 1 == numComponents || numComponents == 1)
					{
						int minx = dstRaster.MinX;
						int miny = dstRaster.MinY;
						int[] bands = new int[numBands - 1];
						for (int i = 0; i < numBands - 1; i++)
						{
							bands[i] = i;
						}
						dstRaster = dstRaster.CreateWritableChild(minx, miny, dstRaster.Width, dstRaster.Height, minx, miny, bands);
					}
				}

				Filter(srcRaster, dstRaster);
			}

			if (needToConvert)
			{
				// ColorModels are not the same
				ColorConvertOp ccop = new ColorConvertOp(Hints);
				ccop.Filter(dst, origDst);
			}

			return origDst;
		}

		/// <summary>
		/// Performs a lookup operation on a <code>Raster</code>.
		/// If the destination <code>Raster</code> is <code>null</code>,
		/// a new <code>Raster</code> will be created.
		/// The <code>IllegalArgumentException</code> might be thrown
		/// if the source <code>Raster</code> and the destination
		/// <code>Raster</code> do not have the same
		/// number of bands or if the number of arrays in the
		/// <code>LookupTable</code> does not meet the
		/// restrictions stated in the class comment above. </summary>
		/// <param name="src"> the source <code>Raster</code> to filter </param>
		/// <param name="dst"> the destination <code>WritableRaster</code> for the
		///            filtered <code>src</code> </param>
		/// <returns> the filtered <code>WritableRaster</code>. </returns>
		/// <exception cref="IllegalArgumentException"> if the source and destinations
		///         rasters do not have the same number of bands, or the
		///         number of arrays in the <code>LookupTable</code> does
		///         not meet the restrictions described in the class comments.
		///  </exception>
		public WritableRaster Filter(Raster src, WritableRaster dst)
		{
			int numBands = src.NumBands;
			int dstLength = dst.NumBands;
			int height = src.Height;
			int width = src.Width;
			int[] srcPix = new int[numBands];

			// Create a new destination Raster, if needed

			if (dst == null)
			{
				dst = CreateCompatibleDestRaster(src);
			}
			else if (height != dst.Height || width != dst.Width)
			{
				throw new IllegalArgumentException("Width or height of Rasters do not " + "match");
			}
			dstLength = dst.NumBands;

			if (numBands != dstLength)
			{
				throw new IllegalArgumentException("Number of channels in the src (" + numBands + ") does not match number of channels" + " in the destination (" + dstLength + ")");
			}
			int numComponents = Ltable.NumComponents;
			if (numComponents != 1 && numComponents != src.NumBands)
			{
				throw new IllegalArgumentException("Number of arrays in the " + " lookup table (" + numComponents + " is not compatible with the " + " src Raster: " + src);
			}


			if (ImagingLib.filter(this, src, dst) != null)
			{
				return dst;
			}

			// Optimize for cases we know about
			if (Ltable is ByteLookupTable)
			{
				ByteFilter((ByteLookupTable) Ltable, src, dst, width, height, numBands);
			}
			else if (Ltable is ShortLookupTable)
			{
				ShortFilter((ShortLookupTable) Ltable, src, dst, width, height, numBands);
			}
			else
			{
				// Not one we recognize so do it slowly
				int sminX = src.MinX;
				int sY = src.MinY;
				int dminX = dst.MinX;
				int dY = dst.MinY;
				for (int y = 0; y < height; y++, sY++, dY++)
				{
					int sX = sminX;
					int dX = dminX;
					for (int x = 0; x < width; x++, sX++, dX++)
					{
						// Find data for all bands at this x,y position
						src.GetPixel(sX, sY, srcPix);

						// Lookup the data for all bands at this x,y position
						Ltable.LookupPixel(srcPix, srcPix);

						// Put it back for all bands
						dst.SetPixel(dX, dY, srcPix);
					}
				}
			}

			return dst;
		}

		/// <summary>
		/// Returns the bounding box of the filtered destination image.  Since
		/// this is not a geometric operation, the bounding box does not
		/// change. </summary>
		/// <param name="src"> the <code>BufferedImage</code> to be filtered </param>
		/// <returns> the bounds of the filtered definition image. </returns>
		public Rectangle2D GetBounds2D(BufferedImage src)
		{
			return GetBounds2D(src.Raster);
		}

		/// <summary>
		/// Returns the bounding box of the filtered destination Raster.  Since
		/// this is not a geometric operation, the bounding box does not
		/// change. </summary>
		/// <param name="src"> the <code>Raster</code> to be filtered </param>
		/// <returns> the bounds of the filtered definition <code>Raster</code>. </returns>
		public Rectangle2D GetBounds2D(Raster src)
		{
			return src.Bounds;

		}

		/// <summary>
		/// Creates a zeroed destination image with the correct size and number of
		/// bands.  If destCM is <code>null</code>, an appropriate
		/// <code>ColorModel</code> will be used. </summary>
		/// <param name="src">       Source image for the filter operation. </param>
		/// <param name="destCM">    the destination's <code>ColorModel</code>, which
		///                  can be <code>null</code>. </param>
		/// <returns> a filtered destination <code>BufferedImage</code>. </returns>
		public virtual BufferedImage CreateCompatibleDestImage(BufferedImage src, ColorModel destCM)
		{
			BufferedImage image;
			int w = src.Width;
			int h = src.Height;
			int transferType = DataBuffer.TYPE_BYTE;
			if (destCM == null)
			{
				ColorModel cm = src.ColorModel;
				Raster raster = src.Raster;
				if (cm is ComponentColorModel)
				{
					DataBuffer db = raster.DataBuffer;
					bool hasAlpha = cm.HasAlpha();
					bool isPre = cm.AlphaPremultiplied;
					int trans = cm.Transparency;
					int[] nbits = null;
					if (Ltable is ByteLookupTable)
					{
						if (db.DataType == db.TYPE_USHORT)
						{
							// Dst raster should be of type byte
							if (hasAlpha)
							{
								nbits = new int[2];
								if (trans == cm.BITMASK)
								{
									nbits[1] = 1;
								}
								else
								{
									nbits[1] = 8;
								}
							}
							else
							{
								nbits = new int[1];
							}
							nbits[0] = 8;
						}
						// For byte, no need to change the cm
					}
					else if (Ltable is ShortLookupTable)
					{
						transferType = DataBuffer.TYPE_USHORT;
						if (db.DataType == db.TYPE_BYTE)
						{
							if (hasAlpha)
							{
								nbits = new int[2];
								if (trans == cm.BITMASK)
								{
									nbits[1] = 1;
								}
								else
								{
									nbits[1] = 16;
								}
							}
							else
							{
								nbits = new int[1];
							}
							nbits[0] = 16;
						}
					}
					if (nbits != null)
					{
						cm = new ComponentColorModel(cm.ColorSpace, nbits, hasAlpha, isPre, trans, transferType);
					}
				}
				image = new BufferedImage(cm, cm.CreateCompatibleWritableRaster(w, h), cm.AlphaPremultiplied, null);
			}
			else
			{
				image = new BufferedImage(destCM, destCM.CreateCompatibleWritableRaster(w, h), destCM.AlphaPremultiplied, null);
			}

			return image;
		}

		/// <summary>
		/// Creates a zeroed-destination <code>Raster</code> with the
		/// correct size and number of bands, given this source. </summary>
		/// <param name="src"> the <code>Raster</code> to be transformed </param>
		/// <returns> the zeroed-destination <code>Raster</code>. </returns>
		public virtual WritableRaster CreateCompatibleDestRaster(Raster src)
		{
			return src.CreateCompatibleWritableRaster();
		}

		/// <summary>
		/// Returns the location of the destination point given a
		/// point in the source.  If <code>dstPt</code> is not
		/// <code>null</code>, it will be used to hold the return value.
		/// Since this is not a geometric operation, the <code>srcPt</code>
		/// will equal the <code>dstPt</code>. </summary>
		/// <param name="srcPt"> a <code>Point2D</code> that represents a point
		///        in the source image </param>
		/// <param name="dstPt"> a <code>Point2D</code>that represents the location
		///        in the destination </param>
		/// <returns> the <code>Point2D</code> in the destination that
		///         corresponds to the specified point in the source. </returns>
		public Point2D GetPoint2D(Point2D srcPt, Point2D dstPt)
		{
			if (dstPt == null)
			{
				dstPt = new Point2D.Float();
			}
			dstPt.SetLocation(srcPt.X, srcPt.Y);

			return dstPt;
		}

		/// <summary>
		/// Returns the rendering hints for this op. </summary>
		/// <returns> the <code>RenderingHints</code> object associated
		///         with this op. </returns>
		public RenderingHints RenderingHints
		{
			get
			{
				return Hints;
			}
		}

		private void ByteFilter(ByteLookupTable lookup, Raster src, WritableRaster dst, int width, int height, int numBands)
		{
			int[] srcPix = null;

			// Find the ref to the table and the offset
			sbyte[][] table = lookup.Table;
			int offset = lookup.Offset;
			int tidx;
			int step = 1;

			// Check if it is one lookup applied to all bands
			if (table.Length == 1)
			{
				step = 0;
			}

			int x;
			int y;
			int band;
			int len = table[0].Length;

			// Loop through the data
			for (y = 0; y < height; y++)
			{
				tidx = 0;
				for (band = 0; band < numBands; band++, tidx += step)
				{
					// Find data for this band, scanline
					srcPix = src.GetSamples(0, y, width, 1, band, srcPix);

					for (x = 0; x < width; x++)
					{
						int index = srcPix[x] - offset;
						if (index < 0 || index > len)
						{
							throw new IllegalArgumentException("index (" + index + "(out of range: " + " srcPix[" + x + "]=" + srcPix[x] + " offset=" + offset);
						}
						// Do the lookup
						srcPix[x] = table[tidx][index];
					}
					// Put it back
					dst.SetSamples(0, y, width, 1, band, srcPix);
				}
			}
		}

		private void ShortFilter(ShortLookupTable lookup, Raster src, WritableRaster dst, int width, int height, int numBands)
		{
			int band;
			int[] srcPix = null;

			// Find the ref to the table and the offset
			short[][] table = lookup.Table;
			int offset = lookup.Offset;
			int tidx;
			int step = 1;

			// Check if it is one lookup applied to all bands
			if (table.Length == 1)
			{
				step = 0;
			}

			int x = 0;
			int y = 0;
			int index;
			int maxShort = (1 << 16) - 1;
			// Loop through the data
			for (y = 0; y < height; y++)
			{
				tidx = 0;
				for (band = 0; band < numBands; band++, tidx += step)
				{
					// Find data for this band, scanline
					srcPix = src.GetSamples(0, y, width, 1, band, srcPix);

					for (x = 0; x < width; x++)
					{
						index = srcPix[x] - offset;
						if (index < 0 || index > maxShort)
						{
							throw new IllegalArgumentException("index out of range " + index + " x is " + x + "srcPix[x]=" + srcPix[x] + " offset=" + offset);
						}
						// Do the lookup
						srcPix[x] = table[tidx][index];
					}
					// Put it back
					dst.SetSamples(0, y, width, 1, band, srcPix);
				}
			}
		}
	}

}
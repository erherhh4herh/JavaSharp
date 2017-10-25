using System;

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
	/// This class performs a pixel-by-pixel rescaling of the data in the
	/// source image by multiplying the sample values for each pixel by a scale
	/// factor and then adding an offset. The scaled sample values are clipped
	/// to the minimum/maximum representable in the destination image.
	/// <para>
	/// The pseudo code for the rescaling operation is as follows:
	/// <pre>
	/// for each pixel from Source object {
	///    for each band/component of the pixel {
	///        dstElement = (srcElement*scaleFactor) + offset
	///    }
	/// }
	/// </pre>
	/// </para>
	/// <para>
	/// For Rasters, rescaling operates on bands.  The number of
	/// sets of scaling constants may be one, in which case the same constants
	/// are applied to all bands, or it must equal the number of Source
	/// Raster bands.
	/// </para>
	/// <para>
	/// For BufferedImages, rescaling operates on color and alpha components.
	/// The number of sets of scaling constants may be one, in which case the
	/// same constants are applied to all color (but not alpha) components.
	/// Otherwise, the  number of sets of scaling constants may
	/// equal the number of Source color components, in which case no
	/// rescaling of the alpha component (if present) is performed.
	/// If neither of these cases apply, the number of sets of scaling constants
	/// must equal the number of Source color components plus alpha components,
	/// in which case all color and alpha components are rescaled.
	/// </para>
	/// <para>
	/// BufferedImage sources with premultiplied alpha data are treated in the same
	/// manner as non-premultiplied images for purposes of rescaling.  That is,
	/// the rescaling is done per band on the raw data of the BufferedImage source
	/// without regard to whether the data is premultiplied.  If a color conversion
	/// is required to the destination ColorModel, the premultiplied state of
	/// both source and destination will be taken into account for this step.
	/// </para>
	/// <para>
	/// Images with an IndexColorModel cannot be rescaled.
	/// </para>
	/// <para>
	/// If a RenderingHints object is specified in the constructor, the
	/// color rendering hint and the dithering hint may be used when color
	/// conversion is required.
	/// </para>
	/// <para>
	/// Note that in-place operation is allowed (i.e. the source and destination can
	/// be the same object).
	/// </para>
	/// </summary>
	/// <seealso cref= java.awt.RenderingHints#KEY_COLOR_RENDERING </seealso>
	/// <seealso cref= java.awt.RenderingHints#KEY_DITHERING </seealso>
	public class RescaleOp : BufferedImageOp, RasterOp
	{
		internal float[] ScaleFactors;
		internal float[] Offsets;
		internal int Length = 0;
		internal RenderingHints Hints;

		private int SrcNbits;
		private int DstNbits;


		/// <summary>
		/// Constructs a new RescaleOp with the desired scale factors
		/// and offsets.  The length of the scaleFactor and offset arrays
		/// must meet the restrictions stated in the class comments above.
		/// The RenderingHints argument may be null. </summary>
		/// <param name="scaleFactors"> the specified scale factors </param>
		/// <param name="offsets"> the specified offsets </param>
		/// <param name="hints"> the specified <code>RenderingHints</code>, or
		///        <code>null</code> </param>
		public RescaleOp(float[] scaleFactors, float[] offsets, RenderingHints hints)
		{
			Length = scaleFactors.Length;
			if (Length > offsets.Length)
			{
				Length = offsets.Length;
			}

			this.ScaleFactors = new float[Length];
			this.Offsets = new float[Length];
			for (int i = 0; i < Length; i++)
			{
				this.ScaleFactors[i] = scaleFactors[i];
				this.Offsets[i] = offsets[i];
			}
			this.Hints = hints;
		}

		/// <summary>
		/// Constructs a new RescaleOp with the desired scale factor
		/// and offset.  The scaleFactor and offset will be applied to
		/// all bands in a source Raster and to all color (but not alpha)
		/// components in a BufferedImage.
		/// The RenderingHints argument may be null. </summary>
		/// <param name="scaleFactor"> the specified scale factor </param>
		/// <param name="offset"> the specified offset </param>
		/// <param name="hints"> the specified <code>RenderingHints</code>, or
		///        <code>null</code> </param>
		public RescaleOp(float scaleFactor, float offset, RenderingHints hints)
		{
			Length = 1;
			this.ScaleFactors = new float[1];
			this.Offsets = new float[1];
			this.ScaleFactors[0] = scaleFactor;
			this.Offsets[0] = offset;
			this.Hints = hints;
		}

		/// <summary>
		/// Returns the scale factors in the given array. The array is also
		/// returned for convenience.  If scaleFactors is null, a new array
		/// will be allocated. </summary>
		/// <param name="scaleFactors"> the array to contain the scale factors of
		///        this <code>RescaleOp</code> </param>
		/// <returns> the scale factors of this <code>RescaleOp</code>. </returns>
		public float[] GetScaleFactors(float[] scaleFactors)
		{
			if (scaleFactors == null)
			{
				return (float[]) this.ScaleFactors.clone();
			}
			System.Array.Copy(this.ScaleFactors, 0, scaleFactors, 0, System.Math.Min(this.ScaleFactors.Length, scaleFactors.Length));
			return scaleFactors;
		}

		/// <summary>
		/// Returns the offsets in the given array. The array is also returned
		/// for convenience.  If offsets is null, a new array
		/// will be allocated. </summary>
		/// <param name="offsets"> the array to contain the offsets of
		///        this <code>RescaleOp</code> </param>
		/// <returns> the offsets of this <code>RescaleOp</code>. </returns>
		public float[] GetOffsets(float[] offsets)
		{
			if (offsets == null)
			{
				return (float[]) this.Offsets.clone();
			}

			System.Array.Copy(this.Offsets, 0, offsets, 0, System.Math.Min(this.Offsets.Length, offsets.Length));
			return offsets;
		}

		/// <summary>
		/// Returns the number of scaling factors and offsets used in this
		/// RescaleOp. </summary>
		/// <returns> the number of scaling factors and offsets of this
		///         <code>RescaleOp</code>. </returns>
		public int NumFactors
		{
			get
			{
				return Length;
			}
		}


		/// <summary>
		/// Creates a ByteLookupTable to implement the rescale.
		/// The table may have either a SHORT or BYTE input. </summary>
		/// <param name="nElems">    Number of elements the table is to have.
		///                  This will generally be 256 for byte and
		///                  65536 for short. </param>
		private ByteLookupTable CreateByteLut(float[] scale, float[] off, int nBands, int nElems)
		{

//JAVA TO C# CONVERTER NOTE: The following call to the 'RectangularArrays' helper class reproduces the rectangular array initialization that is automatic in Java:
//ORIGINAL LINE: sbyte[][] lutData = new sbyte[scale.Length][nElems];
			sbyte[][] lutData = RectangularArrays.ReturnRectangularSbyteArray(scale.Length, nElems);

			for (int band = 0; band < scale.Length; band++)
			{
				float bandScale = scale[band];
				float bandOff = off[band];
				sbyte[] bandLutData = lutData[band];
				for (int i = 0; i < nElems; i++)
				{
					int val = (int)(i * bandScale + bandOff);
					if ((val & 0xffffff00) != 0)
					{
						if (val < 0)
						{
							val = 0;
						}
						else
						{
							val = 255;
						}
					}
					bandLutData[i] = (sbyte)val;
				}

			}

			return new ByteLookupTable(0, lutData);
		}

		/// <summary>
		/// Creates a ShortLookupTable to implement the rescale.
		/// The table may have either a SHORT or BYTE input. </summary>
		/// <param name="nElems">    Number of elements the table is to have.
		///                  This will generally be 256 for byte and
		///                  65536 for short. </param>
		private ShortLookupTable CreateShortLut(float[] scale, float[] off, int nBands, int nElems)
		{

//JAVA TO C# CONVERTER NOTE: The following call to the 'RectangularArrays' helper class reproduces the rectangular array initialization that is automatic in Java:
//ORIGINAL LINE: short[][] lutData = new short[scale.Length][nElems];
			short[][] lutData = RectangularArrays.ReturnRectangularShortArray(scale.Length, nElems);

			for (int band = 0; band < scale.Length; band++)
			{
				float bandScale = scale[band];
				float bandOff = off[band];
				short[] bandLutData = lutData[band];
				for (int i = 0; i < nElems; i++)
				{
					int val = (int)(i * bandScale + bandOff);
					if ((val & 0xffff0000) != 0)
					{
						if (val < 0)
						{
							val = 0;
						}
						else
						{
							val = 65535;
						}
					}
					bandLutData[i] = (short)val;
				}
			}

			return new ShortLookupTable(0, lutData);
		}


		/// <summary>
		/// Determines if the rescale can be performed as a lookup.
		/// The dst must be a byte or short type.
		/// The src must be less than 16 bits.
		/// All source band sizes must be the same and all dst band sizes
		/// must be the same.
		/// </summary>
		private bool CanUseLookup(Raster src, Raster dst)
		{

			//
			// Check that the src datatype is either a BYTE or SHORT
			//
			int datatype = src.DataBuffer.DataType;
			if (datatype != DataBuffer.TYPE_BYTE && datatype != DataBuffer.TYPE_USHORT)
			{
				return false;
			}

			//
			// Check dst sample sizes. All must be 8 or 16 bits.
			//
			SampleModel dstSM = dst.SampleModel;
			DstNbits = dstSM.GetSampleSize(0);

			if (!(DstNbits == 8 || DstNbits == 16))
			{
				return false;
			}
			for (int i = 1; i < src.NumBands; i++)
			{
				int bandSize = dstSM.GetSampleSize(i);
				if (bandSize != DstNbits)
				{
					return false;
				}
			}

			//
			// Check src sample sizes. All must be the same size
			//
			SampleModel srcSM = src.SampleModel;
			SrcNbits = srcSM.GetSampleSize(0);
			if (SrcNbits > 16)
			{
				return false;
			}
			for (int i = 1; i < src.NumBands; i++)
			{
				int bandSize = srcSM.GetSampleSize(i);
				if (bandSize != SrcNbits)
				{
					return false;
				}
			}

			return true;
		}

		/// <summary>
		/// Rescales the source BufferedImage.
		/// If the color model in the source image is not the same as that
		/// in the destination image, the pixels will be converted
		/// in the destination.  If the destination image is null,
		/// a BufferedImage will be created with the source ColorModel.
		/// An IllegalArgumentException may be thrown if the number of
		/// scaling factors/offsets in this object does not meet the
		/// restrictions stated in the class comments above, or if the
		/// source image has an IndexColorModel. </summary>
		/// <param name="src"> the <code>BufferedImage</code> to be filtered </param>
		/// <param name="dst"> the destination for the filtering operation
		///            or <code>null</code> </param>
		/// <returns> the filtered <code>BufferedImage</code>. </returns>
		/// <exception cref="IllegalArgumentException"> if the <code>ColorModel</code>
		///         of <code>src</code> is an <code>IndexColorModel</code>,
		///         or if the number of scaling factors and offsets in this
		///         <code>RescaleOp</code> do not meet the requirements
		///         stated in the class comments. </exception>
		public BufferedImage Filter(BufferedImage src, BufferedImage dst)
		{
			ColorModel srcCM = src.ColorModel;
			ColorModel dstCM;
			int numBands = srcCM.NumColorComponents;


			if (srcCM is IndexColorModel)
			{
				throw new IllegalArgumentException("Rescaling cannot be " + "performed on an indexed image");
			}
			if (Length != 1 && Length != numBands && Length != srcCM.NumComponents)
			{
				throw new IllegalArgumentException("Number of scaling constants " + "does not equal the number of" + " of color or color/alpha " + " components");
			}

			bool needToConvert = false;

			// Include alpha
			if (Length > numBands && srcCM.HasAlpha())
			{
				Length = numBands + 1;
			}

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

			//
			// Try to use a native BI rescale operation first
			//
			if (ImagingLib.filter(this, src, dst) == null)
			{
				//
				// Native BI rescale failed - convert to rasters
				//
				WritableRaster srcRaster = src.Raster;
				WritableRaster dstRaster = dst.Raster;

				if (srcCM.HasAlpha())
				{
					if (numBands - 1 == Length || Length == 1)
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
					if (dstNumBands - 1 == Length || Length == 1)
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

				//
				// Call the raster filter method
				//
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
		/// Rescales the pixel data in the source Raster.
		/// If the destination Raster is null, a new Raster will be created.
		/// The source and destination must have the same number of bands.
		/// Otherwise, an IllegalArgumentException is thrown.
		/// Note that the number of scaling factors/offsets in this object must
		/// meet the restrictions stated in the class comments above.
		/// Otherwise, an IllegalArgumentException is thrown. </summary>
		/// <param name="src"> the <code>Raster</code> to be filtered </param>
		/// <param name="dst"> the destination for the filtering operation
		///            or <code>null</code> </param>
		/// <returns> the filtered <code>WritableRaster</code>. </returns>
		/// <exception cref="IllegalArgumentException"> if <code>src</code> and
		///         <code>dst</code> do not have the same number of bands,
		///         or if the number of scaling factors and offsets in this
		///         <code>RescaleOp</code> do not meet the requirements
		///         stated in the class comments. </exception>
		public WritableRaster Filter(Raster src, WritableRaster dst)
		{
			int numBands = src.NumBands;
			int width = src.Width;
			int height = src.Height;
			int[] srcPix = null;
			int step = 0;
			int tidx = 0;

			// Create a new destination Raster, if needed
			if (dst == null)
			{
				dst = CreateCompatibleDestRaster(src);
			}
			else if (height != dst.Height || width != dst.Width)
			{
				throw new IllegalArgumentException("Width or height of Rasters do not " + "match");
			}
			else if (numBands != dst.NumBands)
			{
				// Make sure that the number of bands are equal
				throw new IllegalArgumentException("Number of bands in src " + numBands + " does not equal number of bands in dest " + dst.NumBands);
			}
			// Make sure that the arrays match
			// Make sure that the low/high/constant arrays match
			if (Length != 1 && Length != src.NumBands)
			{
				throw new IllegalArgumentException("Number of scaling constants " + "does not equal the number of" + " of bands in the src raster");
			}


			//
			// Try for a native raster rescale first
			//
			if (ImagingLib.filter(this, src, dst) != null)
			{
				return dst;
			}

			//
			// Native raster rescale failed.
			// Try to see if a lookup operation can be used
			//
			if (CanUseLookup(src, dst))
			{
				int srcNgray = (1 << SrcNbits);
				int dstNgray = (1 << DstNbits);

				if (dstNgray == 256)
				{
					ByteLookupTable lut = CreateByteLut(ScaleFactors, Offsets, numBands, srcNgray);
					LookupOp op = new LookupOp(lut, Hints);
					op.Filter(src, dst);
				}
				else
				{
					ShortLookupTable lut = CreateShortLut(ScaleFactors, Offsets, numBands, srcNgray);
					LookupOp op = new LookupOp(lut, Hints);
					op.Filter(src, dst);
				}
			}
			else
			{
				//
				// Fall back to the slow code
				//
				if (Length > 1)
				{
					step = 1;
				}

				int sminX = src.MinX;
				int sY = src.MinY;
				int dminX = dst.MinX;
				int dY = dst.MinY;
				int sX;
				int dX;

				//
				//  Determine bits per band to determine maxval for clamps.
				//  The min is assumed to be zero.
				//  REMIND: This must change if we ever support signed data types.
				//
				int nbits;
				int[] dstMax = new int[numBands];
				int[] dstMask = new int[numBands];
				SampleModel dstSM = dst.SampleModel;
				for (int z = 0; z < numBands; z++)
				{
					nbits = dstSM.GetSampleSize(z);
					dstMax[z] = (1 << nbits) - 1;
					dstMask[z] = ~(dstMax[z]);
				}

				int val;
				for (int y = 0; y < height; y++, sY++, dY++)
				{
					dX = dminX;
					sX = sminX;
					for (int x = 0; x < width; x++, sX++, dX++)
					{
						// Get data for all bands at this x,y position
						srcPix = src.GetPixel(sX, sY, srcPix);
						tidx = 0;
						for (int z = 0; z < numBands; z++, tidx += step)
						{
							val = (int)(srcPix[z] * ScaleFactors[tidx] + Offsets[tidx]);
							// Clamp
							if ((val & dstMask[z]) != 0)
							{
								if (val < 0)
								{
									val = 0;
								}
								else
								{
									val = dstMax[z];
								}
							}
							srcPix[z] = val;

						}

						// Put it back for all bands
						dst.SetPixel(dX, dY, srcPix);
					}
				}
			}
			return dst;
		}

		/// <summary>
		/// Returns the bounding box of the rescaled destination image.  Since
		/// this is not a geometric operation, the bounding box does not
		/// change.
		/// </summary>
		public Rectangle2D GetBounds2D(BufferedImage src)
		{
			 return GetBounds2D(src.Raster);
		}

		/// <summary>
		/// Returns the bounding box of the rescaled destination Raster.  Since
		/// this is not a geometric operation, the bounding box does not
		/// change. </summary>
		/// <param name="src"> the rescaled destination <code>Raster</code> </param>
		/// <returns> the bounds of the specified <code>Raster</code>. </returns>
		public Rectangle2D GetBounds2D(Raster src)
		{
			return src.Bounds;
		}

		/// <summary>
		/// Creates a zeroed destination image with the correct size and number of
		/// bands. </summary>
		/// <param name="src">       Source image for the filter operation. </param>
		/// <param name="destCM">    ColorModel of the destination.  If null, the
		///                  ColorModel of the source will be used. </param>
		/// <returns> the zeroed-destination image. </returns>
		public virtual BufferedImage CreateCompatibleDestImage(BufferedImage src, ColorModel destCM)
		{
			BufferedImage image;
			if (destCM == null)
			{
				ColorModel cm = src.ColorModel;
				image = new BufferedImage(cm, src.Raster.CreateCompatibleWritableRaster(), cm.AlphaPremultiplied, null);
			}
			else
			{
				int w = src.Width;
				int h = src.Height;
				image = new BufferedImage(destCM, destCM.CreateCompatibleWritableRaster(w, h), destCM.AlphaPremultiplied, null);
			}

			return image;
		}

		/// <summary>
		/// Creates a zeroed-destination <code>Raster</code> with the correct
		/// size and number of bands, given this source. </summary>
		/// <param name="src">       the source <code>Raster</code> </param>
		/// <returns> the zeroed-destination <code>Raster</code>. </returns>
		public virtual WritableRaster CreateCompatibleDestRaster(Raster src)
		{
			return src.CreateCompatibleWritableRaster(src.Width, src.Height);
		}

		/// <summary>
		/// Returns the location of the destination point given a
		/// point in the source.  If dstPt is non-null, it will
		/// be used to hold the return value.  Since this is not a geometric
		/// operation, the srcPt will equal the dstPt. </summary>
		/// <param name="srcPt"> a point in the source image </param>
		/// <param name="dstPt"> the destination point or <code>null</code> </param>
		/// <returns> the location of the destination point. </returns>
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
		/// <returns> the rendering hints of this <code>RescaleOp</code>. </returns>
		public RenderingHints RenderingHints
		{
			get
			{
				return Hints;
			}
		}
	}

}
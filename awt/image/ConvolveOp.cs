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

	using ImagingLib = sun.awt.image.ImagingLib;

	/// <summary>
	/// This class implements a convolution from the source
	/// to the destination.
	/// Convolution using a convolution kernel is a spatial operation that
	/// computes the output pixel from an input pixel by multiplying the kernel
	/// with the surround of the input pixel.
	/// This allows the output pixel to be affected by the immediate neighborhood
	/// in a way that can be mathematically specified with a kernel.
	/// <para>
	/// This class operates with BufferedImage data in which color components are
	/// premultiplied with the alpha component.  If the Source BufferedImage has
	/// an alpha component, and the color components are not premultiplied with
	/// the alpha component, then the data are premultiplied before being
	/// convolved.  If the Destination has color components which are not
	/// premultiplied, then alpha is divided out before storing into the
	/// Destination (if alpha is 0, the color components are set to 0).  If the
	/// Destination has no alpha component, then the resulting alpha is discarded
	/// after first dividing it out of the color components.
	/// </para>
	/// <para>
	/// Rasters are treated as having no alpha channel.  If the above treatment
	/// of the alpha channel in BufferedImages is not desired, it may be avoided
	/// by getting the Raster of a source BufferedImage and using the filter method
	/// of this class which works with Rasters.
	/// </para>
	/// <para>
	/// If a RenderingHints object is specified in the constructor, the
	/// color rendering hint and the dithering hint may be used when color
	/// conversion is required.
	/// </para>
	/// <para>
	/// Note that the Source and the Destination may not be the same object.
	/// </para>
	/// </summary>
	/// <seealso cref= Kernel </seealso>
	/// <seealso cref= java.awt.RenderingHints#KEY_COLOR_RENDERING </seealso>
	/// <seealso cref= java.awt.RenderingHints#KEY_DITHERING </seealso>
	public class ConvolveOp : BufferedImageOp, RasterOp
	{
		internal Kernel Kernel_Renamed;
		internal int EdgeHint;
		internal RenderingHints Hints;
		/// <summary>
		/// Edge condition constants.
		/// </summary>

		/// <summary>
		/// Pixels at the edge of the destination image are set to zero.  This
		/// is the default.
		/// </summary>

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Native public static final int EDGE_ZERO_FILL = 0;
		public const int EDGE_ZERO_FILL = 0;

		/// <summary>
		/// Pixels at the edge of the source image are copied to
		/// the corresponding pixels in the destination without modification.
		/// </summary>
//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Native public static final int EDGE_NO_OP = 1;
		public const int EDGE_NO_OP = 1;

		/// <summary>
		/// Constructs a ConvolveOp given a Kernel, an edge condition, and a
		/// RenderingHints object (which may be null). </summary>
		/// <param name="kernel"> the specified <code>Kernel</code> </param>
		/// <param name="edgeCondition"> the specified edge condition </param>
		/// <param name="hints"> the specified <code>RenderingHints</code> object </param>
		/// <seealso cref= Kernel </seealso>
		/// <seealso cref= #EDGE_NO_OP </seealso>
		/// <seealso cref= #EDGE_ZERO_FILL </seealso>
		/// <seealso cref= java.awt.RenderingHints </seealso>
		public ConvolveOp(Kernel kernel, int edgeCondition, RenderingHints hints)
		{
			this.Kernel_Renamed = kernel;
			this.EdgeHint = edgeCondition;
			this.Hints = hints;
		}

		/// <summary>
		/// Constructs a ConvolveOp given a Kernel.  The edge condition
		/// will be EDGE_ZERO_FILL. </summary>
		/// <param name="kernel"> the specified <code>Kernel</code> </param>
		/// <seealso cref= Kernel </seealso>
		/// <seealso cref= #EDGE_ZERO_FILL </seealso>
		public ConvolveOp(Kernel kernel)
		{
			this.Kernel_Renamed = kernel;
			this.EdgeHint = EDGE_ZERO_FILL;
		}

		/// <summary>
		/// Returns the edge condition. </summary>
		/// <returns> the edge condition of this <code>ConvolveOp</code>. </returns>
		/// <seealso cref= #EDGE_NO_OP </seealso>
		/// <seealso cref= #EDGE_ZERO_FILL </seealso>
		public virtual int EdgeCondition
		{
			get
			{
				return EdgeHint;
			}
		}

		/// <summary>
		/// Returns the Kernel. </summary>
		/// <returns> the <code>Kernel</code> of this <code>ConvolveOp</code>. </returns>
		public Kernel Kernel
		{
			get
			{
				return (Kernel) Kernel_Renamed.Clone();
			}
		}

		/// <summary>
		/// Performs a convolution on BufferedImages.  Each component of the
		/// source image will be convolved (including the alpha component, if
		/// present).
		/// If the color model in the source image is not the same as that
		/// in the destination image, the pixels will be converted
		/// in the destination.  If the destination image is null,
		/// a BufferedImage will be created with the source ColorModel.
		/// The IllegalArgumentException may be thrown if the source is the
		/// same as the destination. </summary>
		/// <param name="src"> the source <code>BufferedImage</code> to filter </param>
		/// <param name="dst"> the destination <code>BufferedImage</code> for the
		///        filtered <code>src</code> </param>
		/// <returns> the filtered <code>BufferedImage</code> </returns>
		/// <exception cref="NullPointerException"> if <code>src</code> is <code>null</code> </exception>
		/// <exception cref="IllegalArgumentException"> if <code>src</code> equals
		///         <code>dst</code> </exception>
		/// <exception cref="ImagingOpException"> if <code>src</code> cannot be filtered </exception>
		public BufferedImage Filter(BufferedImage src, BufferedImage dst)
		{
			if (src == null)
			{
				throw new NullPointerException("src image is null");
			}
			if (src == dst)
			{
				throw new IllegalArgumentException("src image cannot be the " + "same as the dst image");
			}

			bool needToConvert = false;
			ColorModel srcCM = src.ColorModel;
			ColorModel dstCM;
			BufferedImage origDst = dst;

			// Can't convolve an IndexColorModel.  Need to expand it
			if (srcCM is IndexColorModel)
			{
				IndexColorModel icm = (IndexColorModel) srcCM;
				src = icm.ConvertToIntDiscrete(src.Raster, false);
				srcCM = src.ColorModel;
			}

			if (dst == null)
			{
				dst = CreateCompatibleDestImage(src, null);
				dstCM = srcCM;
				origDst = dst;
			}
			else
			{
				dstCM = dst.ColorModel;
				if (srcCM.ColorSpace.Type != dstCM.ColorSpace.Type)
				{
					needToConvert = true;
					dst = CreateCompatibleDestImage(src, null);
					dstCM = dst.ColorModel;
				}
				else if (dstCM is IndexColorModel)
				{
					dst = CreateCompatibleDestImage(src, null);
					dstCM = dst.ColorModel;
				}
			}

			if (ImagingLib.filter(this, src, dst) == null)
			{
				throw new ImagingOpException("Unable to convolve src image");
			}

			if (needToConvert)
			{
				ColorConvertOp ccop = new ColorConvertOp(Hints);
				ccop.Filter(dst, origDst);
			}
			else if (origDst != dst)
			{
				java.awt.Graphics2D g = origDst.CreateGraphics();
				try
				{
					g.DrawImage(dst, 0, 0, null);
				}
				finally
				{
					g.Dispose();
				}
			}

			return origDst;
		}

		/// <summary>
		/// Performs a convolution on Rasters.  Each band of the source Raster
		/// will be convolved.
		/// The source and destination must have the same number of bands.
		/// If the destination Raster is null, a new Raster will be created.
		/// The IllegalArgumentException may be thrown if the source is
		/// the same as the destination. </summary>
		/// <param name="src"> the source <code>Raster</code> to filter </param>
		/// <param name="dst"> the destination <code>WritableRaster</code> for the
		///        filtered <code>src</code> </param>
		/// <returns> the filtered <code>WritableRaster</code> </returns>
		/// <exception cref="NullPointerException"> if <code>src</code> is <code>null</code> </exception>
		/// <exception cref="ImagingOpException"> if <code>src</code> and <code>dst</code>
		///         do not have the same number of bands </exception>
		/// <exception cref="ImagingOpException"> if <code>src</code> cannot be filtered </exception>
		/// <exception cref="IllegalArgumentException"> if <code>src</code> equals
		///         <code>dst</code> </exception>
		public WritableRaster Filter(Raster src, WritableRaster dst)
		{
			if (dst == null)
			{
				dst = CreateCompatibleDestRaster(src);
			}
			else if (src == dst)
			{
				throw new IllegalArgumentException("src image cannot be the " + "same as the dst image");
			}
			else if (src.NumBands != dst.NumBands)
			{
				throw new ImagingOpException("Different number of bands in src " + " and dst Rasters");
			}

			if (ImagingLib.filter(this, src, dst) == null)
			{
				throw new ImagingOpException("Unable to convolve src image");
			}

			return dst;
		}

		/// <summary>
		/// Creates a zeroed destination image with the correct size and number
		/// of bands.  If destCM is null, an appropriate ColorModel will be used. </summary>
		/// <param name="src">       Source image for the filter operation. </param>
		/// <param name="destCM">    ColorModel of the destination.  Can be null. </param>
		/// <returns> a destination <code>BufferedImage</code> with the correct
		///         size and number of bands. </returns>
		public virtual BufferedImage CreateCompatibleDestImage(BufferedImage src, ColorModel destCM)
		{
			BufferedImage image;

			int w = src.Width;
			int h = src.Height;

			WritableRaster wr = null;

			if (destCM == null)
			{
				destCM = src.ColorModel;
				// Not much support for ICM
				if (destCM is IndexColorModel)
				{
					destCM = ColorModel.RGBdefault;
				}
				else
				{
					/* Create destination image as similar to the source
					 *  as it possible...
					 */
					wr = src.Data.CreateCompatibleWritableRaster(w, h);
				}
			}

			if (wr == null)
			{
				/* This is the case when destination color model
				 * was explicitly specified (and it may be not compatible
				 * with source raster structure) or source is indexed image.
				 * We should use destination color model to create compatible
				 * destination raster here.
				 */
				wr = destCM.CreateCompatibleWritableRaster(w, h);
			}

			image = new BufferedImage(destCM, wr, destCM.AlphaPremultiplied, null);

			return image;
		}

		/// <summary>
		/// Creates a zeroed destination Raster with the correct size and number
		/// of bands, given this source.
		/// </summary>
		public virtual WritableRaster CreateCompatibleDestRaster(Raster src)
		{
			return src.CreateCompatibleWritableRaster();
		}

		/// <summary>
		/// Returns the bounding box of the filtered destination image.  Since
		/// this is not a geometric operation, the bounding box does not
		/// change.
		/// </summary>
		public Rectangle2D GetBounds2D(BufferedImage src)
		{
			return GetBounds2D(src.Raster);
		}

		/// <summary>
		/// Returns the bounding box of the filtered destination Raster.  Since
		/// this is not a geometric operation, the bounding box does not
		/// change.
		/// </summary>
		public Rectangle2D GetBounds2D(Raster src)
		{
			return src.Bounds;
		}

		/// <summary>
		/// Returns the location of the destination point given a
		/// point in the source.  If dstPt is non-null, it will
		/// be used to hold the return value.  Since this is not a geometric
		/// operation, the srcPt will equal the dstPt.
		/// </summary>
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
		/// Returns the rendering hints for this op.
		/// </summary>
		public RenderingHints RenderingHints
		{
			get
			{
				return Hints;
			}
		}
	}

}
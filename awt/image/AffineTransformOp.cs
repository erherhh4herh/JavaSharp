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

namespace java.awt.image
{

	using ImagingLib = sun.awt.image.ImagingLib;

	/// <summary>
	/// This class uses an affine transform to perform a linear mapping from
	/// 2D coordinates in the source image or <CODE>Raster</CODE> to 2D coordinates
	/// in the destination image or <CODE>Raster</CODE>.
	/// The type of interpolation that is used is specified through a constructor,
	/// either by a <CODE>RenderingHints</CODE> object or by one of the integer
	/// interpolation types defined in this class.
	/// <para>
	/// If a <CODE>RenderingHints</CODE> object is specified in the constructor, the
	/// interpolation hint and the rendering quality hint are used to set
	/// the interpolation type for this operation.  The color rendering hint
	/// and the dithering hint can be used when color conversion is required.
	/// </para>
	/// <para>
	/// Note that the following constraints have to be met:
	/// <ul>
	/// <li>The source and destination must be different.
	/// <li>For <CODE>Raster</CODE> objects, the number of bands in the source must
	/// be equal to the number of bands in the destination.
	/// </ul>
	/// </para>
	/// </summary>
	/// <seealso cref= AffineTransform </seealso>
	/// <seealso cref= BufferedImageFilter </seealso>
	/// <seealso cref= java.awt.RenderingHints#KEY_INTERPOLATION </seealso>
	/// <seealso cref= java.awt.RenderingHints#KEY_RENDERING </seealso>
	/// <seealso cref= java.awt.RenderingHints#KEY_COLOR_RENDERING </seealso>
	/// <seealso cref= java.awt.RenderingHints#KEY_DITHERING </seealso>
	public class AffineTransformOp : BufferedImageOp, RasterOp
	{
		private AffineTransform Xform;
		internal RenderingHints Hints;

		/// <summary>
		/// Nearest-neighbor interpolation type.
		/// </summary>
//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Native public static final int TYPE_NEAREST_NEIGHBOR = 1;
		public const int TYPE_NEAREST_NEIGHBOR = 1;

		/// <summary>
		/// Bilinear interpolation type.
		/// </summary>
//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Native public static final int TYPE_BILINEAR = 2;
		public const int TYPE_BILINEAR = 2;

		/// <summary>
		/// Bicubic interpolation type.
		/// </summary>
//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Native public static final int TYPE_BICUBIC = 3;
		public const int TYPE_BICUBIC = 3;

		internal int InterpolationType_Renamed = TYPE_NEAREST_NEIGHBOR;

		/// <summary>
		/// Constructs an <CODE>AffineTransformOp</CODE> given an affine transform.
		/// The interpolation type is determined from the
		/// <CODE>RenderingHints</CODE> object.  If the interpolation hint is
		/// defined, it will be used. Otherwise, if the rendering quality hint is
		/// defined, the interpolation type is determined from its value.  If no
		/// hints are specified (<CODE>hints</CODE> is null),
		/// the interpolation type is {@link #TYPE_NEAREST_NEIGHBOR
		/// TYPE_NEAREST_NEIGHBOR}.
		/// </summary>
		/// <param name="xform"> The <CODE>AffineTransform</CODE> to use for the
		/// operation.
		/// </param>
		/// <param name="hints"> The <CODE>RenderingHints</CODE> object used to specify
		/// the interpolation type for the operation.
		/// </param>
		/// <exception cref="ImagingOpException"> if the transform is non-invertible. </exception>
		/// <seealso cref= java.awt.RenderingHints#KEY_INTERPOLATION </seealso>
		/// <seealso cref= java.awt.RenderingHints#KEY_RENDERING </seealso>
		public AffineTransformOp(AffineTransform xform, RenderingHints hints)
		{
			ValidateTransform(xform);
			this.Xform = (AffineTransform) xform.Clone();
			this.Hints = hints;

			if (hints != null)
			{
				Object value = hints[hints.KEY_INTERPOLATION];
				if (value == null)
				{
					value = hints[hints.KEY_RENDERING];
					if (value == hints.VALUE_RENDER_SPEED)
					{
						InterpolationType_Renamed = TYPE_NEAREST_NEIGHBOR;
					}
					else if (value == hints.VALUE_RENDER_QUALITY)
					{
						InterpolationType_Renamed = TYPE_BILINEAR;
					}
				}
				else if (value == hints.VALUE_INTERPOLATION_NEAREST_NEIGHBOR)
				{
					InterpolationType_Renamed = TYPE_NEAREST_NEIGHBOR;
				}
				else if (value == hints.VALUE_INTERPOLATION_BILINEAR)
				{
					InterpolationType_Renamed = TYPE_BILINEAR;
				}
				else if (value == hints.VALUE_INTERPOLATION_BICUBIC)
				{
					InterpolationType_Renamed = TYPE_BICUBIC;
				}
			}
			else
			{
				InterpolationType_Renamed = TYPE_NEAREST_NEIGHBOR;
			}
		}

		/// <summary>
		/// Constructs an <CODE>AffineTransformOp</CODE> given an affine transform
		/// and the interpolation type.
		/// </summary>
		/// <param name="xform"> The <CODE>AffineTransform</CODE> to use for the operation. </param>
		/// <param name="interpolationType"> One of the integer
		/// interpolation type constants defined by this class:
		/// <seealso cref="#TYPE_NEAREST_NEIGHBOR TYPE_NEAREST_NEIGHBOR"/>,
		/// <seealso cref="#TYPE_BILINEAR TYPE_BILINEAR"/>,
		/// <seealso cref="#TYPE_BICUBIC TYPE_BICUBIC"/>. </param>
		/// <exception cref="ImagingOpException"> if the transform is non-invertible. </exception>
		public AffineTransformOp(AffineTransform xform, int interpolationType)
		{
			ValidateTransform(xform);
			this.Xform = (AffineTransform)xform.Clone();
			switch (interpolationType)
			{
				case TYPE_NEAREST_NEIGHBOR:
				case TYPE_BILINEAR:
				case TYPE_BICUBIC:
					break;
			default:
				throw new IllegalArgumentException("Unknown interpolation type: " + interpolationType);
			}
			this.InterpolationType_Renamed = interpolationType;
		}

		/// <summary>
		/// Returns the interpolation type used by this op. </summary>
		/// <returns> the interpolation type. </returns>
		/// <seealso cref= #TYPE_NEAREST_NEIGHBOR </seealso>
		/// <seealso cref= #TYPE_BILINEAR </seealso>
		/// <seealso cref= #TYPE_BICUBIC </seealso>
		public int InterpolationType
		{
			get
			{
				return InterpolationType_Renamed;
			}
		}

		/// <summary>
		/// Transforms the source <CODE>BufferedImage</CODE> and stores the results
		/// in the destination <CODE>BufferedImage</CODE>.
		/// If the color models for the two images do not match, a color
		/// conversion into the destination color model is performed.
		/// If the destination image is null,
		/// a <CODE>BufferedImage</CODE> is created with the source
		/// <CODE>ColorModel</CODE>.
		/// <para>
		/// The coordinates of the rectangle returned by
		/// <code>getBounds2D(BufferedImage)</code>
		/// are not necessarily the same as the coordinates of the
		/// <code>BufferedImage</code> returned by this method.  If the
		/// upper-left corner coordinates of the rectangle are
		/// negative then this part of the rectangle is not drawn.  If the
		/// upper-left corner coordinates of the  rectangle are positive
		/// then the filtered image is drawn at that position in the
		/// destination <code>BufferedImage</code>.
		/// </para>
		/// <para>
		/// An <CODE>IllegalArgumentException</CODE> is thrown if the source is
		/// the same as the destination.
		/// 
		/// </para>
		/// </summary>
		/// <param name="src"> The <CODE>BufferedImage</CODE> to transform. </param>
		/// <param name="dst"> The <CODE>BufferedImage</CODE> in which to store the results
		/// of the transformation.
		/// </param>
		/// <returns> The filtered <CODE>BufferedImage</CODE>. </returns>
		/// <exception cref="IllegalArgumentException"> if <code>src</code> and
		///         <code>dst</code> are the same </exception>
		/// <exception cref="ImagingOpException"> if the image cannot be transformed
		///         because of a data-processing error that might be
		///         caused by an invalid image format, tile format, or
		///         image-processing operation, or any other unsupported
		///         operation. </exception>
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
					int type = Xform.Type;
					bool needTrans = ((type & (Xform.TYPE_MASK_ROTATION | Xform.TYPE_GENERAL_TRANSFORM)) != 0);
					if (!needTrans && type != Xform.TYPE_TRANSLATION && type != Xform.TYPE_IDENTITY)
					{
						double[] mtx = new double[4];
						Xform.GetMatrix(mtx);
						// Check out the matrix.  A non-integral scale will force ARGB
						// since the edge conditions can't be guaranteed.
						needTrans = (mtx[0] != (int)mtx[0] || mtx[3] != (int)mtx[3]);
					}

					if (needTrans && srcCM.Transparency == java.awt.Transparency_Fields.OPAQUE)
					{
						// Need to convert first
						ColorConvertOp ccop = new ColorConvertOp(Hints);
						BufferedImage tmpSrc = null;
						int sw = src.Width;
						int sh = src.Height;
						if (dstCM.Transparency == java.awt.Transparency_Fields.OPAQUE)
						{
							tmpSrc = new BufferedImage(sw, sh, BufferedImage.TYPE_INT_ARGB);
						}
						else
						{
							WritableRaster r = dstCM.CreateCompatibleWritableRaster(sw, sh);
							tmpSrc = new BufferedImage(dstCM, r, dstCM.AlphaPremultiplied, null);
						}
						src = ccop.Filter(src, tmpSrc);
					}
					else
					{
						needToConvert = true;
						dst = CreateCompatibleDestImage(src, null);
					}
				}

			}

			if (InterpolationType_Renamed != TYPE_NEAREST_NEIGHBOR && dst.ColorModel is IndexColorModel)
			{
				dst = new BufferedImage(dst.Width, dst.Height, BufferedImage.TYPE_INT_ARGB);
			}
			if (ImagingLib.filter(this, src, dst) == null)
			{
				throw new ImagingOpException("Unable to transform src image");
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
					g.Composite = AlphaComposite.Src;
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
		/// Transforms the source <CODE>Raster</CODE> and stores the results in
		/// the destination <CODE>Raster</CODE>.  This operation performs the
		/// transform band by band.
		/// <para>
		/// If the destination <CODE>Raster</CODE> is null, a new
		/// <CODE>Raster</CODE> is created.
		/// An <CODE>IllegalArgumentException</CODE> may be thrown if the source is
		/// the same as the destination or if the number of bands in
		/// the source is not equal to the number of bands in the
		/// destination.
		/// </para>
		/// <para>
		/// The coordinates of the rectangle returned by
		/// <code>getBounds2D(Raster)</code>
		/// are not necessarily the same as the coordinates of the
		/// <code>WritableRaster</code> returned by this method.  If the
		/// upper-left corner coordinates of rectangle are negative then
		/// this part of the rectangle is not drawn.  If the coordinates
		/// of the rectangle are positive then the filtered image is drawn at
		/// that position in the destination <code>Raster</code>.
		/// </para>
		/// <para>
		/// </para>
		/// </summary>
		/// <param name="src"> The <CODE>Raster</CODE> to transform. </param>
		/// <param name="dst"> The <CODE>Raster</CODE> in which to store the results of the
		/// transformation.
		/// </param>
		/// <returns> The transformed <CODE>Raster</CODE>.
		/// </returns>
		/// <exception cref="ImagingOpException"> if the raster cannot be transformed
		///         because of a data-processing error that might be
		///         caused by an invalid image format, tile format, or
		///         image-processing operation, or any other unsupported
		///         operation. </exception>
		public WritableRaster Filter(Raster src, WritableRaster dst)
		{
			if (src == null)
			{
				throw new NullPointerException("src image is null");
			}
			if (dst == null)
			{
				dst = CreateCompatibleDestRaster(src);
			}
			if (src == dst)
			{
				throw new IllegalArgumentException("src image cannot be the " + "same as the dst image");
			}
			if (src.NumBands != dst.NumBands)
			{
				throw new IllegalArgumentException("Number of src bands (" + src.NumBands + ") does not match number of " + " dst bands (" + dst.NumBands + ")");
			}

			if (ImagingLib.filter(this, src, dst) == null)
			{
				throw new ImagingOpException("Unable to transform src image");
			}
			return dst;
		}

		/// <summary>
		/// Returns the bounding box of the transformed destination.  The
		/// rectangle returned is the actual bounding box of the
		/// transformed points.  The coordinates of the upper-left corner
		/// of the returned rectangle might not be (0,&nbsp;0).
		/// </summary>
		/// <param name="src"> The <CODE>BufferedImage</CODE> to be transformed.
		/// </param>
		/// <returns> The <CODE>Rectangle2D</CODE> representing the destination's
		/// bounding box. </returns>
		public Rectangle2D GetBounds2D(BufferedImage src)
		{
			return GetBounds2D(src.Raster);
		}

		/// <summary>
		/// Returns the bounding box of the transformed destination.  The
		/// rectangle returned will be the actual bounding box of the
		/// transformed points.  The coordinates of the upper-left corner
		/// of the returned rectangle might not be (0,&nbsp;0).
		/// </summary>
		/// <param name="src"> The <CODE>Raster</CODE> to be transformed.
		/// </param>
		/// <returns> The <CODE>Rectangle2D</CODE> representing the destination's
		/// bounding box. </returns>
		public Rectangle2D GetBounds2D(Raster src)
		{
			int w = src.Width;
			int h = src.Height;

			// Get the bounding box of the src and transform the corners
			float[] pts = new float[] {0, 0, w, 0, w, h, 0, h};
			Xform.Transform(pts, 0, pts, 0, 4);

			// Get the min, max of the dst
			float fmaxX = pts[0];
			float fmaxY = pts[1];
			float fminX = pts[0];
			float fminY = pts[1];
			for (int i = 2; i < 8; i += 2)
			{
				if (pts[i] > fmaxX)
				{
					fmaxX = pts[i];
				}
				else if (pts[i] < fminX)
				{
					fminX = pts[i];
				}
				if (pts[i + 1] > fmaxY)
				{
					fmaxY = pts[i + 1];
				}
				else if (pts[i + 1] < fminY)
				{
					fminY = pts[i + 1];
				}
			}

			return new Rectangle2D.Float(fminX, fminY, fmaxX - fminX, fmaxY - fminY);
		}

		/// <summary>
		/// Creates a zeroed destination image with the correct size and number of
		/// bands.  A <CODE>RasterFormatException</CODE> may be thrown if the
		/// transformed width or height is equal to 0.
		/// <para>
		/// If <CODE>destCM</CODE> is null,
		/// an appropriate <CODE>ColorModel</CODE> is used; this
		/// <CODE>ColorModel</CODE> may have
		/// an alpha channel even if the source <CODE>ColorModel</CODE> is opaque.
		/// 
		/// </para>
		/// </summary>
		/// <param name="src">  The <CODE>BufferedImage</CODE> to be transformed. </param>
		/// <param name="destCM">  <CODE>ColorModel</CODE> of the destination.  If null,
		/// an appropriate <CODE>ColorModel</CODE> is used.
		/// </param>
		/// <returns> The zeroed destination image. </returns>
		public virtual BufferedImage CreateCompatibleDestImage(BufferedImage src, ColorModel destCM)
		{
			BufferedImage image;
			Rectangle r = GetBounds2D(src).Bounds;

			// If r.x (or r.y) is < 0, then we want to only create an image
			// that is in the positive range.
			// If r.x (or r.y) is > 0, then we need to create an image that
			// includes the translation.
			int w = r.x + r.Width_Renamed;
			int h = r.y + r.Height_Renamed;
			if (w <= 0)
			{
				throw new RasterFormatException("Transformed width (" + w + ") is less than or equal to 0.");
			}
			if (h <= 0)
			{
				throw new RasterFormatException("Transformed height (" + h + ") is less than or equal to 0.");
			}

			if (destCM == null)
			{
				ColorModel cm = src.ColorModel;
				if (InterpolationType_Renamed != TYPE_NEAREST_NEIGHBOR && (cm is IndexColorModel || cm.Transparency == java.awt.Transparency_Fields.OPAQUE))
				{
					image = new BufferedImage(w, h, BufferedImage.TYPE_INT_ARGB);
				}
				else
				{
					image = new BufferedImage(cm, src.Raster.CreateCompatibleWritableRaster(w,h), cm.AlphaPremultiplied, null);
				}
			}
			else
			{
				image = new BufferedImage(destCM, destCM.CreateCompatibleWritableRaster(w,h), destCM.AlphaPremultiplied, null);
			}

			return image;
		}

		/// <summary>
		/// Creates a zeroed destination <CODE>Raster</CODE> with the correct size
		/// and number of bands.  A <CODE>RasterFormatException</CODE> may be thrown
		/// if the transformed width or height is equal to 0.
		/// </summary>
		/// <param name="src"> The <CODE>Raster</CODE> to be transformed.
		/// </param>
		/// <returns> The zeroed destination <CODE>Raster</CODE>. </returns>
		public virtual WritableRaster CreateCompatibleDestRaster(Raster src)
		{
			Rectangle2D r = GetBounds2D(src);

			return src.CreateCompatibleWritableRaster((int)r.X, (int)r.Y, (int)r.Width, (int)r.Height);
		}

		/// <summary>
		/// Returns the location of the corresponding destination point given a
		/// point in the source.  If <CODE>dstPt</CODE> is specified, it
		/// is used to hold the return value.
		/// </summary>
		/// <param name="srcPt"> The <code>Point2D</code> that represents the source
		///              point. </param>
		/// <param name="dstPt"> The <CODE>Point2D</CODE> in which to store the result.
		/// </param>
		/// <returns> The <CODE>Point2D</CODE> in the destination that corresponds to
		/// the specified point in the source. </returns>
		public Point2D GetPoint2D(Point2D srcPt, Point2D dstPt)
		{
			return Xform.Transform(srcPt, dstPt);
		}

		/// <summary>
		/// Returns the affine transform used by this transform operation.
		/// </summary>
		/// <returns> The <CODE>AffineTransform</CODE> associated with this op. </returns>
		public AffineTransform Transform
		{
			get
			{
				return (AffineTransform) Xform.Clone();
			}
		}

		/// <summary>
		/// Returns the rendering hints used by this transform operation.
		/// </summary>
		/// <returns> The <CODE>RenderingHints</CODE> object associated with this op. </returns>
		public RenderingHints RenderingHints
		{
			get
			{
				if (Hints == null)
				{
					Object val;
					switch (InterpolationType_Renamed)
					{
					case TYPE_NEAREST_NEIGHBOR:
						val = RenderingHints.VALUE_INTERPOLATION_NEAREST_NEIGHBOR;
						break;
					case TYPE_BILINEAR:
						val = RenderingHints.VALUE_INTERPOLATION_BILINEAR;
						break;
					case TYPE_BICUBIC:
						val = RenderingHints.VALUE_INTERPOLATION_BICUBIC;
						break;
					default:
						// Should never get here
						throw new InternalError("Unknown interpolation type " + InterpolationType_Renamed);
    
					}
					Hints = new RenderingHints(RenderingHints.KEY_INTERPOLATION, val);
				}
    
				return Hints;
			}
		}

		// We need to be able to invert the transform if we want to
		// transform the image.  If the determinant of the matrix is 0,
		// then we can't invert the transform.
		internal virtual void ValidateTransform(AffineTransform xform)
		{
			if (System.Math.Abs(xform.Determinant) <= Double.Epsilon)
			{
				throw new ImagingOpException("Unable to invert transform " + xform);
			}
		}
	}

}
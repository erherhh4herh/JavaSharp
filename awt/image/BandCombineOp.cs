/*
 * Copyright (c) 1997, 2005, Oracle and/or its affiliates. All rights reserved.
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
	/// This class performs an arbitrary linear combination of the bands
	/// in a <CODE>Raster</CODE>, using a specified matrix.
	/// <para>
	/// The width of the matrix must be equal to the number of bands in the
	/// source <CODE>Raster</CODE>, optionally plus one.  If there is one more
	/// column in the matrix than the number of bands, there is an implied 1 at the
	/// end of the vector of band samples representing a pixel.  The height
	/// of the matrix must be equal to the number of bands in the destination.
	/// </para>
	/// <para>
	/// For example, a 3-banded <CODE>Raster</CODE> might have the following
	/// transformation applied to each pixel in order to invert the second band of
	/// the <CODE>Raster</CODE>.
	/// <pre>
	///   [ 1.0   0.0   0.0    0.0  ]     [ b1 ]
	///   [ 0.0  -1.0   0.0  255.0  ]  x  [ b2 ]
	///   [ 0.0   0.0   1.0    0.0  ]     [ b3 ]
	///                                   [ 1 ]
	/// </pre>
	/// 
	/// </para>
	/// <para>
	/// Note that the source and destination can be the same object.
	/// </para>
	/// </summary>
	public class BandCombineOp : RasterOp
	{
		internal float[][] Matrix_Renamed;
		internal int Nrows = 0;
		internal int Ncols = 0;
		internal RenderingHints Hints;

		/// <summary>
		/// Constructs a <CODE>BandCombineOp</CODE> with the specified matrix.
		/// The width of the matrix must be equal to the number of bands in
		/// the source <CODE>Raster</CODE>, optionally plus one.  If there is one
		/// more column in the matrix than the number of bands, there is an implied
		/// 1 at the end of the vector of band samples representing a pixel.  The
		/// height of the matrix must be equal to the number of bands in the
		/// destination.
		/// <para>
		/// The first subscript is the row index and the second
		/// is the column index.  This operation uses none of the currently
		/// defined rendering hints; the <CODE>RenderingHints</CODE> argument can be
		/// null.
		/// 
		/// </para>
		/// </summary>
		/// <param name="matrix"> The matrix to use for the band combine operation. </param>
		/// <param name="hints"> The <CODE>RenderingHints</CODE> object for this operation.
		/// Not currently used so it can be null. </param>
		public BandCombineOp(float[][] matrix, RenderingHints hints)
		{
			Nrows = matrix.Length;
			Ncols = matrix[0].Length;
			this.Matrix_Renamed = new float[Nrows][];
			for (int i = 0; i < Nrows; i++)
			{
				/* Arrays.copyOf is forgiving of the source array being
				 * too short, but it is also faster than other cloning
				 * methods, so we provide our own protection for short
				 * matrix rows.
				 */
				if (Ncols > matrix[i].Length)
				{
					throw new IndexOutOfBoundsException("row " + i + " too short");
				}
				this.Matrix_Renamed[i] = Arrays.CopyOf(matrix[i], Ncols);
			}
			this.Hints = hints;
		}

		/// <summary>
		/// Returns a copy of the linear combination matrix.
		/// </summary>
		/// <returns> The matrix associated with this band combine operation. </returns>
		public float[][] Matrix
		{
			get
			{
				float[][] ret = new float[Nrows][];
				for (int i = 0; i < Nrows; i++)
				{
					ret[i] = Arrays.CopyOf(Matrix_Renamed[i], Ncols);
				}
				return ret;
			}
		}

		/// <summary>
		/// Transforms the <CODE>Raster</CODE> using the matrix specified in the
		/// constructor. An <CODE>IllegalArgumentException</CODE> may be thrown if
		/// the number of bands in the source or destination is incompatible with
		/// the matrix.  See the class comments for more details.
		/// <para>
		/// If the destination is null, it will be created with a number of bands
		/// equalling the number of rows in the matrix. No exception is thrown
		/// if the operation causes a data overflow.
		/// 
		/// </para>
		/// </summary>
		/// <param name="src"> The <CODE>Raster</CODE> to be filtered. </param>
		/// <param name="dst"> The <CODE>Raster</CODE> in which to store the results
		/// of the filter operation.
		/// </param>
		/// <returns> The filtered <CODE>Raster</CODE>.
		/// </returns>
		/// <exception cref="IllegalArgumentException"> If the number of bands in the
		/// source or destination is incompatible with the matrix. </exception>
		public virtual WritableRaster Filter(Raster src, WritableRaster dst)
		{
			int nBands = src.NumBands;
			if (Ncols != nBands && Ncols != (nBands + 1))
			{
				throw new IllegalArgumentException("Number of columns in the " + "matrix (" + Ncols + ") must be equal to the number" + " of bands ([+1]) in src (" + nBands + ").");
			}
			if (dst == null)
			{
				dst = CreateCompatibleDestRaster(src);
			}
			else if (Nrows != dst.NumBands)
			{
				throw new IllegalArgumentException("Number of rows in the " + "matrix (" + Nrows + ") must be equal to the number" + " of bands ([+1]) in dst (" + nBands + ").");
			}

			if (ImagingLib.filter(this, src, dst) != null)
			{
				return dst;
			}

			int[] pixel = null;
			int[] dstPixel = new int[dst.NumBands];
			float accum;
			int sminX = src.MinX;
			int sY = src.MinY;
			int dminX = dst.MinX;
			int dY = dst.MinY;
			int sX;
			int dX;
			if (Ncols == nBands)
			{
				for (int y = 0; y < src.Height; y++, sY++, dY++)
				{
					dX = dminX;
					sX = sminX;
					for (int x = 0; x < src.Width; x++, sX++, dX++)
					{
						pixel = src.GetPixel(sX, sY, pixel);
						for (int r = 0; r < Nrows; r++)
						{
							accum = 0.0f;
							for (int c = 0; c < Ncols; c++)
							{
								accum += Matrix_Renamed[r][c] * pixel[c];
							}
							dstPixel[r] = (int) accum;
						}
						dst.SetPixel(dX, dY, dstPixel);
					}
				}
			}
			else
			{
				// Need to add constant
				for (int y = 0; y < src.Height; y++, sY++, dY++)
				{
					dX = dminX;
					sX = sminX;
					for (int x = 0; x < src.Width; x++, sX++, dX++)
					{
						pixel = src.GetPixel(sX, sY, pixel);
						for (int r = 0; r < Nrows; r++)
						{
							accum = 0.0f;
							for (int c = 0; c < nBands; c++)
							{
								accum += Matrix_Renamed[r][c] * pixel[c];
							}
							dstPixel[r] = (int)(accum + Matrix_Renamed[r][nBands]);
						}
						dst.SetPixel(dX, dY, dstPixel);
					}
				}
			}

			return dst;
		}

		/// <summary>
		/// Returns the bounding box of the transformed destination.  Since
		/// this is not a geometric operation, the bounding box is the same for
		/// the source and destination.
		/// An <CODE>IllegalArgumentException</CODE> may be thrown if the number of
		/// bands in the source is incompatible with the matrix.  See
		/// the class comments for more details.
		/// </summary>
		/// <param name="src"> The <CODE>Raster</CODE> to be filtered.
		/// </param>
		/// <returns> The <CODE>Rectangle2D</CODE> representing the destination
		/// image's bounding box.
		/// </returns>
		/// <exception cref="IllegalArgumentException"> If the number of bands in the source
		/// is incompatible with the matrix. </exception>
		public Rectangle2D GetBounds2D(Raster src)
		{
			return src.Bounds;
		}


		/// <summary>
		/// Creates a zeroed destination <CODE>Raster</CODE> with the correct size
		/// and number of bands.
		/// An <CODE>IllegalArgumentException</CODE> may be thrown if the number of
		/// bands in the source is incompatible with the matrix.  See
		/// the class comments for more details.
		/// </summary>
		/// <param name="src"> The <CODE>Raster</CODE> to be filtered.
		/// </param>
		/// <returns> The zeroed destination <CODE>Raster</CODE>. </returns>
		public virtual WritableRaster CreateCompatibleDestRaster(Raster src)
		{
			int nBands = src.NumBands;
			if ((Ncols != nBands) && (Ncols != (nBands + 1)))
			{
				throw new IllegalArgumentException("Number of columns in the " + "matrix (" + Ncols + ") must be equal to the number" + " of bands ([+1]) in src (" + nBands + ").");
			}
			if (src.NumBands == Nrows)
			{
				return src.CreateCompatibleWritableRaster();
			}
			else
			{
				throw new IllegalArgumentException("Don't know how to create a " + " compatible Raster with " + Nrows + " bands.");
			}
		}

		/// <summary>
		/// Returns the location of the corresponding destination point given a
		/// point in the source <CODE>Raster</CODE>.  If <CODE>dstPt</CODE> is
		/// specified, it is used to hold the return value.
		/// Since this is not a geometric operation, the point returned
		/// is the same as the specified <CODE>srcPt</CODE>.
		/// </summary>
		/// <param name="srcPt"> The <code>Point2D</code> that represents the point in
		///              the source <code>Raster</code> </param>
		/// <param name="dstPt"> The <CODE>Point2D</CODE> in which to store the result.
		/// </param>
		/// <returns> The <CODE>Point2D</CODE> in the destination image that
		/// corresponds to the specified point in the source image. </returns>
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
		/// Returns the rendering hints for this operation.
		/// </summary>
		/// <returns> The <CODE>RenderingHints</CODE> object associated with this
		/// operation.  Returns null if no hints have been set. </returns>
		public RenderingHints RenderingHints
		{
			get
			{
				return Hints;
			}
		}
	}

}
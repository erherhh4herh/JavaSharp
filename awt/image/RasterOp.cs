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


	/// <summary>
	/// This interface describes single-input/single-output
	/// operations performed on Raster objects.  It is implemented by such
	/// classes as AffineTransformOp, ConvolveOp, and LookupOp.  The Source
	/// and Destination objects must contain the appropriate number
	/// of bands for the particular classes implementing this interface.
	/// Otherwise, an exception is thrown.  This interface cannot be used to
	/// describe more sophisticated Ops such as ones that take multiple sources.
	/// Each class implementing this interface will specify whether or not it
	/// will allow an in-place filtering operation (i.e. source object equal
	/// to the destination object).  Note that the restriction to single-input
	/// operations means that the values of destination pixels prior to the
	/// operation are not used as input to the filter operation. </summary>
	/// <seealso cref= AffineTransformOp </seealso>
	/// <seealso cref= BandCombineOp </seealso>
	/// <seealso cref= ColorConvertOp </seealso>
	/// <seealso cref= ConvolveOp </seealso>
	/// <seealso cref= LookupOp </seealso>
	/// <seealso cref= RescaleOp </seealso>
	public interface RasterOp
	{
		/// <summary>
		/// Performs a single-input/single-output operation from a source Raster
		/// to a destination Raster.  If the destination Raster is null, a
		/// new Raster will be created.  The IllegalArgumentException may be thrown
		/// if the source and/or destination Raster is incompatible with the types
		/// of Rasters allowed by the class implementing this filter. </summary>
		/// <param name="src"> the source <code>Raster</code> </param>
		/// <param name="dest"> the destination <code>WritableRaster</code> </param>
		/// <returns> a <code>WritableRaster</code> that represents the result of
		///         the filtering operation. </returns>
		WritableRaster Filter(Raster src, WritableRaster dest);

		/// <summary>
		/// Returns the bounding box of the filtered destination Raster.
		/// The IllegalArgumentException may be thrown if the source Raster
		/// is incompatible with the types of Rasters allowed
		/// by the class implementing this filter. </summary>
		/// <param name="src"> the source <code>Raster</code> </param>
		/// <returns> a <code>Rectangle2D</code> that is the bounding box of
		///         the <code>Raster</code> resulting from the filtering
		///         operation. </returns>
		Rectangle2D GetBounds2D(Raster src);

		/// <summary>
		/// Creates a zeroed destination Raster with the correct size and number of
		/// bands.
		/// The IllegalArgumentException may be thrown if the source Raster
		/// is incompatible with the types of Rasters allowed
		/// by the class implementing this filter. </summary>
		/// <param name="src"> the source <code>Raster</code> </param>
		/// <returns> a <code>WritableRaster</code> that is compatible with
		///         <code>src</code> </returns>
		WritableRaster CreateCompatibleDestRaster(Raster src);

		/// <summary>
		/// Returns the location of the destination point given a
		/// point in the source Raster.  If dstPt is non-null, it
		/// will be used to hold the return value. </summary>
		/// <param name="srcPt"> the source <code>Point2D</code> </param>
		/// <param name="dstPt"> the destination <code>Point2D</code> </param>
		/// <returns> the location of the destination point. </returns>
		Point2D GetPoint2D(Point2D srcPt, Point2D dstPt);

		/// <summary>
		/// Returns the rendering hints for this RasterOp.  Returns
		/// null if no hints have been set. </summary>
		/// <returns> the <code>RenderingHints</code> object of this
		///         <code>RasterOp</code>. </returns>
		RenderingHints RenderingHints {get;}
	}

}
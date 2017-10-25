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
	/// This class provides an easy way to create an ImageFilter which modifies
	/// the pixels of an image in the default RGB ColorModel.  It is meant to
	/// be used in conjunction with a FilteredImageSource object to produce
	/// filtered versions of existing images.  It is an abstract class that
	/// provides the calls needed to channel all of the pixel data through a
	/// single method which converts pixels one at a time in the default RGB
	/// ColorModel regardless of the ColorModel being used by the ImageProducer.
	/// The only method which needs to be defined to create a useable image
	/// filter is the filterRGB method.  Here is an example of a definition
	/// of a filter which swaps the red and blue components of an image:
	/// <pre>{@code
	/// 
	///      class RedBlueSwapFilter extends RGBImageFilter {
	///          public RedBlueSwapFilter() {
	///              // The filter's operation does not depend on the
	///              // pixel's location, so IndexColorModels can be
	///              // filtered directly.
	///              canFilterIndexColorModel = true;
	///          }
	/// 
	///          public int filterRGB(int x, int y, int rgb) {
	///              return ((rgb & 0xff00ff00)
	///                      | ((rgb & 0xff0000) >> 16)
	///                      | ((rgb & 0xff) << 16));
	///          }
	///      }
	/// 
	/// }</pre>
	/// </summary>
	/// <seealso cref= FilteredImageSource </seealso>
	/// <seealso cref= ImageFilter </seealso>
	/// <seealso cref= ColorModel#getRGBdefault
	/// 
	/// @author      Jim Graham </seealso>
	public abstract class RGBImageFilter : ImageFilter
	{

		/// <summary>
		/// The <code>ColorModel</code> to be replaced by
		/// <code>newmodel</code> when the user calls
		/// <seealso cref="#substituteColorModel(ColorModel, ColorModel) substituteColorModel"/>.
		/// </summary>
		protected internal ColorModel Origmodel;

		/// <summary>
		/// The <code>ColorModel</code> with which to
		/// replace <code>origmodel</code> when the user calls
		/// <code>substituteColorModel</code>.
		/// </summary>
		protected internal ColorModel Newmodel;

		/// <summary>
		/// This boolean indicates whether or not it is acceptable to apply
		/// the color filtering of the filterRGB method to the color table
		/// entries of an IndexColorModel object in lieu of pixel by pixel
		/// filtering.  Subclasses should set this variable to true in their
		/// constructor if their filterRGB method does not depend on the
		/// coordinate of the pixel being filtered. </summary>
		/// <seealso cref= #substituteColorModel </seealso>
		/// <seealso cref= #filterRGB </seealso>
		/// <seealso cref= IndexColorModel </seealso>
		protected internal bool CanFilterIndexColorModel;

		/// <summary>
		/// If the ColorModel is an IndexColorModel and the subclass has
		/// set the canFilterIndexColorModel flag to true, we substitute
		/// a filtered version of the color model here and wherever
		/// that original ColorModel object appears in the setPixels methods.
		/// If the ColorModel is not an IndexColorModel or is null, this method
		/// overrides the default ColorModel used by the ImageProducer and
		/// specifies the default RGB ColorModel instead.
		/// <para>
		/// Note: This method is intended to be called by the
		/// <code>ImageProducer</code> of the <code>Image</code> whose pixels
		/// are being filtered. Developers using
		/// this class to filter pixels from an image should avoid calling
		/// this method directly since that operation could interfere
		/// with the filtering operation.
		/// </para>
		/// </summary>
		/// <seealso cref= ImageConsumer </seealso>
		/// <seealso cref= ColorModel#getRGBdefault </seealso>
		public override ColorModel ColorModel
		{
			set
			{
				if (CanFilterIndexColorModel && (value is IndexColorModel))
				{
					ColorModel newcm = FilterIndexColorModel((IndexColorModel)value);
					SubstituteColorModel(value, newcm);
					Consumer.ColorModel = newcm;
				}
				else
				{
					Consumer.ColorModel = ColorModel.RGBdefault;
				}
			}
		}

		/// <summary>
		/// Registers two ColorModel objects for substitution.  If the oldcm
		/// is encountered during any of the setPixels methods, the newcm
		/// is substituted and the pixels passed through
		/// untouched (but with the new ColorModel object). </summary>
		/// <param name="oldcm"> the ColorModel object to be replaced on the fly </param>
		/// <param name="newcm"> the ColorModel object to replace oldcm on the fly </param>
		public virtual void SubstituteColorModel(ColorModel oldcm, ColorModel newcm)
		{
			Origmodel = oldcm;
			Newmodel = newcm;
		}

		/// <summary>
		/// Filters an IndexColorModel object by running each entry in its
		/// color tables through the filterRGB function that RGBImageFilter
		/// subclasses must provide.  Uses coordinates of -1 to indicate that
		/// a color table entry is being filtered rather than an actual
		/// pixel value. </summary>
		/// <param name="icm"> the IndexColorModel object to be filtered </param>
		/// <exception cref="NullPointerException"> if <code>icm</code> is null </exception>
		/// <returns> a new IndexColorModel representing the filtered colors </returns>
		public virtual IndexColorModel FilterIndexColorModel(IndexColorModel icm)
		{
			int mapsize = icm.MapSize;
			sbyte[] r = new sbyte[mapsize];
			sbyte[] g = new sbyte[mapsize];
			sbyte[] b = new sbyte[mapsize];
			sbyte[] a = new sbyte[mapsize];
			icm.GetReds(r);
			icm.GetGreens(g);
			icm.GetBlues(b);
			icm.GetAlphas(a);
			int trans = icm.TransparentPixel;
			bool needalpha = false;
			for (int i = 0; i < mapsize; i++)
			{
				int rgb = FilterRGB(-1, -1, icm.GetRGB(i));
				a[i] = (sbyte)(rgb >> 24);
				if (a[i] != (unchecked((sbyte)0xff)) && i != trans)
				{
					needalpha = true;
				}
				r[i] = (sbyte)(rgb >> 16);
				g[i] = (sbyte)(rgb >> 8);
				b[i] = (sbyte)(rgb >> 0);
			}
			if (needalpha)
			{
				return new IndexColorModel(icm.PixelSize, mapsize, r, g, b, a);
			}
			else
			{
				return new IndexColorModel(icm.PixelSize, mapsize, r, g, b, trans);
			}
		}

		/// <summary>
		/// Filters a buffer of pixels in the default RGB ColorModel by passing
		/// them one by one through the filterRGB method. </summary>
		/// <param name="x"> the X coordinate of the upper-left corner of the region
		///          of pixels </param>
		/// <param name="y"> the Y coordinate of the upper-left corner of the region
		///          of pixels </param>
		/// <param name="w"> the width of the region of pixels </param>
		/// <param name="h"> the height of the region of pixels </param>
		/// <param name="pixels"> the array of pixels </param>
		/// <param name="off"> the offset into the <code>pixels</code> array </param>
		/// <param name="scansize"> the distance from one row of pixels to the next
		///        in the array </param>
		/// <seealso cref= ColorModel#getRGBdefault </seealso>
		/// <seealso cref= #filterRGB </seealso>
		public virtual void FilterRGBPixels(int x, int y, int w, int h, int[] pixels, int off, int scansize)
		{
			int index = off;
			for (int cy = 0; cy < h; cy++)
			{
				for (int cx = 0; cx < w; cx++)
				{
					pixels[index] = FilterRGB(x + cx, y + cy, pixels[index]);
					index++;
				}
				index += scansize - w;
			}
			Consumer.SetPixels(x, y, w, h, ColorModel.RGBdefault, pixels, off, scansize);
		}

		/// <summary>
		/// If the ColorModel object is the same one that has already
		/// been converted, then simply passes the pixels through with the
		/// converted ColorModel. Otherwise converts the buffer of byte
		/// pixels to the default RGB ColorModel and passes the converted
		/// buffer to the filterRGBPixels method to be converted one by one.
		/// <para>
		/// Note: This method is intended to be called by the
		/// <code>ImageProducer</code> of the <code>Image</code> whose pixels
		/// are being filtered. Developers using
		/// this class to filter pixels from an image should avoid calling
		/// this method directly since that operation could interfere
		/// with the filtering operation.
		/// </para>
		/// </summary>
		/// <seealso cref= ColorModel#getRGBdefault </seealso>
		/// <seealso cref= #filterRGBPixels </seealso>
		public override void SetPixels(int x, int y, int w, int h, ColorModel model, sbyte[] pixels, int off, int scansize)
		{
			if (model == Origmodel)
			{
				Consumer.SetPixels(x, y, w, h, Newmodel, pixels, off, scansize);
			}
			else
			{
				int[] filteredpixels = new int[w];
				int index = off;
				for (int cy = 0; cy < h; cy++)
				{
					for (int cx = 0; cx < w; cx++)
					{
						filteredpixels[cx] = model.GetRGB((pixels[index] & 0xff));
						index++;
					}
					index += scansize - w;
					FilterRGBPixels(x, y + cy, w, 1, filteredpixels, 0, w);
				}
			}
		}

		/// <summary>
		/// If the ColorModel object is the same one that has already
		/// been converted, then simply passes the pixels through with the
		/// converted ColorModel, otherwise converts the buffer of integer
		/// pixels to the default RGB ColorModel and passes the converted
		/// buffer to the filterRGBPixels method to be converted one by one.
		/// Converts a buffer of integer pixels to the default RGB ColorModel
		/// and passes the converted buffer to the filterRGBPixels method.
		/// <para>
		/// Note: This method is intended to be called by the
		/// <code>ImageProducer</code> of the <code>Image</code> whose pixels
		/// are being filtered. Developers using
		/// this class to filter pixels from an image should avoid calling
		/// this method directly since that operation could interfere
		/// with the filtering operation.
		/// </para>
		/// </summary>
		/// <seealso cref= ColorModel#getRGBdefault </seealso>
		/// <seealso cref= #filterRGBPixels </seealso>
		public override void SetPixels(int x, int y, int w, int h, ColorModel model, int[] pixels, int off, int scansize)
		{
			if (model == Origmodel)
			{
				Consumer.SetPixels(x, y, w, h, Newmodel, pixels, off, scansize);
			}
			else
			{
				int[] filteredpixels = new int[w];
				int index = off;
				for (int cy = 0; cy < h; cy++)
				{
					for (int cx = 0; cx < w; cx++)
					{
						filteredpixels[cx] = model.GetRGB(pixels[index]);
						index++;
					}
					index += scansize - w;
					FilterRGBPixels(x, y + cy, w, 1, filteredpixels, 0, w);
				}
			}
		}

		/// <summary>
		/// Subclasses must specify a method to convert a single input pixel
		/// in the default RGB ColorModel to a single output pixel. </summary>
		/// <param name="x"> the X coordinate of the pixel </param>
		/// <param name="y"> the Y coordinate of the pixel </param>
		/// <param name="rgb"> the integer pixel representation in the default RGB
		///            color model </param>
		/// <returns> a filtered pixel in the default RGB color model. </returns>
		/// <seealso cref= ColorModel#getRGBdefault </seealso>
		/// <seealso cref= #filterRGBPixels </seealso>
		public abstract int FilterRGB(int x, int y, int rgb);
	}

}
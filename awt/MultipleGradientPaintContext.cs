using System;

/*
 * Copyright (c) 2006, 2013, Oracle and/or its affiliates. All rights reserved.
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

namespace java.awt
{


	/// <summary>
	/// This is the superclass for all PaintContexts which use a multiple color
	/// gradient to fill in their raster.  It provides the actual color
	/// interpolation functionality.  Subclasses only have to deal with using
	/// the gradient to fill pixels in a raster.
	/// 
	/// @author Nicholas Talian, Vincent Hardy, Jim Graham, Jerry Evans
	/// </summary>
	internal abstract class MultipleGradientPaintContext : PaintContext
	{

		/// <summary>
		/// The PaintContext's ColorModel.  This is ARGB if colors are not all
		/// opaque, otherwise it is RGB.
		/// </summary>
		protected internal ColorModel Model;

		/// <summary>
		/// Color model used if gradient colors are all opaque. </summary>
		private static ColorModel Xrgbmodel = new DirectColorModel(24, 0x00ff0000, 0x0000ff00, 0x000000ff);

		/// <summary>
		/// The cached ColorModel. </summary>
		protected internal static ColorModel CachedModel;

		/// <summary>
		/// The cached raster, which is reusable among instances. </summary>
		protected internal static WeakReference<Raster> Cached;

		/// <summary>
		/// Raster is reused whenever possible. </summary>
		protected internal Raster Saved;

		/// <summary>
		/// The method to use when painting out of the gradient bounds. </summary>
		protected internal CycleMethod CycleMethod;

		/// <summary>
		/// The ColorSpace in which to perform the interpolation </summary>
		protected internal ColorSpaceType ColorSpace;

		/// <summary>
		/// Elements of the inverse transform matrix. </summary>
		protected internal float A00, A01, A10, A11, A02, A12;

		/// <summary>
		/// This boolean specifies whether we are in simple lookup mode, where an
		/// input value between 0 and 1 may be used to directly index into a single
		/// array of gradient colors.  If this boolean value is false, then we have
		/// to use a 2-step process where we have to determine which gradient array
		/// we fall into, then determine the index into that array.
		/// </summary>
		protected internal bool IsSimpleLookup;

		/// <summary>
		/// Size of gradients array for scaling the 0-1 index when looking up
		/// colors the fast way.
		/// </summary>
		protected internal int FastGradientArraySize;

		/// <summary>
		/// Array which contains the interpolated color values for each interval,
		/// used by calculateSingleArrayGradient().  It is protected for possible
		/// direct access by subclasses.
		/// </summary>
		protected internal int[] Gradient;

		/// <summary>
		/// Array of gradient arrays, one array for each interval.  Used by
		/// calculateMultipleArrayGradient().
		/// </summary>
		private int[][] Gradients;

		/// <summary>
		/// Normalized intervals array. </summary>
		private float[] NormalizedIntervals;

		/// <summary>
		/// Fractions array. </summary>
		private float[] Fractions;

		/// <summary>
		/// Used to determine if gradient colors are all opaque. </summary>
		private int TransparencyTest;

		/// <summary>
		/// Color space conversion lookup tables. </summary>
		private static readonly int[] SRGBtoLinearRGB = new int[256];
		private static readonly int[] LinearRGBtoSRGB = new int[256];

		static MultipleGradientPaintContext()
		{
			// build the tables
			for (int k = 0; k < 256; k++)
			{
				SRGBtoLinearRGB[k] = ConvertSRGBtoLinearRGB(k);
				LinearRGBtoSRGB[k] = ConvertLinearRGBtoSRGB(k);
			}
		}

		/// <summary>
		/// Constant number of max colors between any 2 arbitrary colors.
		/// Used for creating and indexing gradients arrays.
		/// </summary>
		protected internal const int GRADIENT_SIZE = 256;
		protected internal static readonly int GRADIENT_SIZE_INDEX = GRADIENT_SIZE -1;

		/// <summary>
		/// Maximum length of the fast single-array.  If the estimated array size
		/// is greater than this, switch over to the slow lookup method.
		/// No particular reason for choosing this number, but it seems to provide
		/// satisfactory performance for the common case (fast lookup).
		/// </summary>
		private const int MAX_GRADIENT_ARRAY_SIZE = 5000;

		/// <summary>
		/// Constructor for MultipleGradientPaintContext superclass.
		/// </summary>
		protected internal MultipleGradientPaintContext(MultipleGradientPaint mgp, ColorModel cm, Rectangle deviceBounds, Rectangle2D userBounds, AffineTransform t, RenderingHints hints, float[] fractions, Color[] colors, CycleMethod cycleMethod, ColorSpaceType colorSpace)
		{
			if (deviceBounds == null)
			{
				throw new NullPointerException("Device bounds cannot be null");
			}

			if (userBounds == null)
			{
				throw new NullPointerException("User bounds cannot be null");
			}

			if (t == null)
			{
				throw new NullPointerException("Transform cannot be null");
			}

			if (hints == null)
			{
				throw new NullPointerException("RenderingHints cannot be null");
			}

			// The inverse transform is needed to go from device to user space.
			// Get all the components of the inverse transform matrix.
			AffineTransform tInv;
			try
			{
				// the following assumes that the caller has copied the incoming
				// transform and is not concerned about it being modified
				t.Invert();
				tInv = t;
			}
			catch (NoninvertibleTransformException)
			{
				// just use identity transform in this case; better to show
				// (incorrect) results than to throw an exception and/or no-op
				tInv = new AffineTransform();
			}
			double[] m = new double[6];
			tInv.GetMatrix(m);
			A00 = (float)m[0];
			A10 = (float)m[1];
			A01 = (float)m[2];
			A11 = (float)m[3];
			A02 = (float)m[4];
			A12 = (float)m[5];

			// copy some flags
			this.CycleMethod = cycleMethod;
			this.ColorSpace = colorSpace;

			// we can avoid copying this array since we do not modify its values
			this.Fractions = fractions;

			// note that only one of these values can ever be non-null (we either
			// store the fast gradient array or the slow one, but never both
			// at the same time)
			int[] gradient = (mgp.Gradient != null) ? mgp.Gradient.get() : null;
			int[][] gradients = (mgp.Gradients != null) ? mgp.Gradients.get() : null;

			if (gradient == null && gradients == null)
			{
				// we need to (re)create the appropriate values
				CalculateLookupData(colors);

				// now cache the calculated values in the
				// MultipleGradientPaint instance for future use
				mgp.Model = this.Model;
				mgp.NormalizedIntervals = this.NormalizedIntervals;
				mgp.IsSimpleLookup = this.IsSimpleLookup;
				if (IsSimpleLookup)
				{
					// only cache the fast array
					mgp.FastGradientArraySize = this.FastGradientArraySize;
					mgp.Gradient = new SoftReference<int[]>(this.Gradient);
				}
				else
				{
					// only cache the slow array
					mgp.Gradients = new SoftReference<int[][]>(this.Gradients);
				}
			}
			else
			{
				// use the values cached in the MultipleGradientPaint instance
				this.Model = mgp.Model;
				this.NormalizedIntervals = mgp.NormalizedIntervals;
				this.IsSimpleLookup = mgp.IsSimpleLookup;
				this.Gradient = gradient;
				this.FastGradientArraySize = mgp.FastGradientArraySize;
				this.Gradients = gradients;
			}
		}

		/// <summary>
		/// This function is the meat of this class.  It calculates an array of
		/// gradient colors based on an array of fractions and color values at
		/// those fractions.
		/// </summary>
		private void CalculateLookupData(Color[] colors)
		{
			Color[] normalizedColors;
			if (ColorSpace == ColorSpaceType.LINEAR_RGB)
			{
				// create a new colors array
				normalizedColors = new Color[colors.Length];
				// convert the colors using the lookup table
				for (int i = 0; i < colors.Length; i++)
				{
					int argb = colors[i].RGB;
					int a = (int)((uint)argb >> 24);
					int r = SRGBtoLinearRGB[(argb >> 16) & 0xff];
					int g = SRGBtoLinearRGB[(argb >> 8) & 0xff];
					int b = SRGBtoLinearRGB[(argb) & 0xff];
					normalizedColors[i] = new Color(r, g, b, a);
				}
			}
			else
			{
				// we can just use this array by reference since we do not
				// modify its values in the case of SRGB
				normalizedColors = colors;
			}

			// this will store the intervals (distances) between gradient stops
			NormalizedIntervals = new float[Fractions.Length - 1];

			// convert from fractions into intervals
			for (int i = 0; i < NormalizedIntervals.Length; i++)
			{
				// interval distance is equal to the difference in positions
				NormalizedIntervals[i] = this.Fractions[i + 1] - this.Fractions[i];
			}

			// initialize to be fully opaque for ANDing with colors
			TransparencyTest = unchecked((int)0xff000000);

			// array of interpolation arrays
			Gradients = new int[NormalizedIntervals.Length][];

			// find smallest interval
			float Imin = 1;
			for (int i = 0; i < NormalizedIntervals.Length; i++)
			{
				Imin = (Imin > NormalizedIntervals[i]) ? NormalizedIntervals[i] : Imin;
			}

			// Estimate the size of the entire gradients array.
			// This is to prevent a tiny interval from causing the size of array
			// to explode.  If the estimated size is too large, break to using
			// separate arrays for each interval, and using an indexing scheme at
			// look-up time.
			int estimatedSize = 0;
			for (int i = 0; i < NormalizedIntervals.Length; i++)
			{
				estimatedSize += (int)((NormalizedIntervals[i] / Imin) * GRADIENT_SIZE);
			}

			if (estimatedSize > MAX_GRADIENT_ARRAY_SIZE)
			{
				// slow method
				CalculateMultipleArrayGradient(normalizedColors);
			}
			else
			{
				// fast method
				CalculateSingleArrayGradient(normalizedColors, Imin);
			}

			// use the most "economical" model
			if (((int)((uint)TransparencyTest >> 24)) == 0xff)
			{
				Model = Xrgbmodel;
			}
			else
			{
				Model = ColorModel.RGBdefault;
			}
		}

		/// <summary>
		/// FAST LOOKUP METHOD
		/// 
		/// This method calculates the gradient color values and places them in a
		/// single int array, gradient[].  It does this by allocating space for
		/// each interval based on its size relative to the smallest interval in
		/// the array.  The smallest interval is allocated 255 interpolated values
		/// (the maximum number of unique in-between colors in a 24 bit color
		/// system), and all other intervals are allocated
		/// size = (255 * the ratio of their size to the smallest interval).
		/// 
		/// This scheme expedites a speedy retrieval because the colors are
		/// distributed along the array according to their user-specified
		/// distribution.  All that is needed is a relative index from 0 to 1.
		/// 
		/// The only problem with this method is that the possibility exists for
		/// the array size to balloon in the case where there is a
		/// disproportionately small gradient interval.  In this case the other
		/// intervals will be allocated huge space, but much of that data is
		/// redundant.  We thus need to use the space conserving scheme below.
		/// </summary>
		/// <param name="Imin"> the size of the smallest interval </param>
		private void CalculateSingleArrayGradient(Color[] colors, float Imin)
		{
			// set the flag so we know later it is a simple (fast) lookup
			IsSimpleLookup = true;

			// 2 colors to interpolate
			int rgb1, rgb2;

			//the eventual size of the single array
			int gradientsTot = 1;

			// for every interval (transition between 2 colors)
			for (int i = 0; i < Gradients.Length; i++)
			{
				// create an array whose size is based on the ratio to the
				// smallest interval
				int nGradients = (int)((NormalizedIntervals[i] / Imin) * 255f);
				gradientsTot += nGradients;
				Gradients[i] = new int[nGradients];

				// the 2 colors (keyframes) to interpolate between
				rgb1 = colors[i].RGB;
				rgb2 = colors[i + 1].RGB;

				// fill this array with the colors in between rgb1 and rgb2
				Interpolate(rgb1, rgb2, Gradients[i]);

				// if the colors are opaque, transparency should still
				// be 0xff000000
				TransparencyTest &= rgb1;
				TransparencyTest &= rgb2;
			}

			// put all gradients in a single array
			Gradient = new int[gradientsTot];
			int curOffset = 0;
			for (int i = 0; i < Gradients.Length; i++)
			{
				System.Array.Copy(Gradients[i], 0, Gradient, curOffset, Gradients[i].Length);
				curOffset += Gradients[i].Length;
			}
			Gradient[Gradient.Length - 1] = colors[colors.Length - 1].RGB;

			// if interpolation occurred in Linear RGB space, convert the
			// gradients back to sRGB using the lookup table
			if (ColorSpace == ColorSpaceType.LINEAR_RGB)
			{
				for (int i = 0; i < Gradient.Length; i++)
				{
					Gradient[i] = ConvertEntireColorLinearRGBtoSRGB(Gradient[i]);
				}
			}

			FastGradientArraySize = Gradient.Length - 1;
		}

		/// <summary>
		/// SLOW LOOKUP METHOD
		/// 
		/// This method calculates the gradient color values for each interval and
		/// places each into its own 255 size array.  The arrays are stored in
		/// gradients[][].  (255 is used because this is the maximum number of
		/// unique colors between 2 arbitrary colors in a 24 bit color system.)
		/// 
		/// This method uses the minimum amount of space (only 255 * number of
		/// intervals), but it aggravates the lookup procedure, because now we
		/// have to find out which interval to select, then calculate the index
		/// within that interval.  This causes a significant performance hit,
		/// because it requires this calculation be done for every point in
		/// the rendering loop.
		/// 
		/// For those of you who are interested, this is a classic example of the
		/// time-space tradeoff.
		/// </summary>
		private void CalculateMultipleArrayGradient(Color[] colors)
		{
			// set the flag so we know later it is a non-simple lookup
			IsSimpleLookup = false;

			// 2 colors to interpolate
			int rgb1, rgb2;

			// for every interval (transition between 2 colors)
			for (int i = 0; i < Gradients.Length; i++)
			{
				// create an array of the maximum theoretical size for
				// each interval
				Gradients[i] = new int[GRADIENT_SIZE];

				// get the the 2 colors
				rgb1 = colors[i].RGB;
				rgb2 = colors[i + 1].RGB;

				// fill this array with the colors in between rgb1 and rgb2
				Interpolate(rgb1, rgb2, Gradients[i]);

				// if the colors are opaque, transparency should still
				// be 0xff000000
				TransparencyTest &= rgb1;
				TransparencyTest &= rgb2;
			}

			// if interpolation occurred in Linear RGB space, convert the
			// gradients back to SRGB using the lookup table
			if (ColorSpace == ColorSpaceType.LINEAR_RGB)
			{
				for (int j = 0; j < Gradients.Length; j++)
				{
					for (int i = 0; i < Gradients[j].Length; i++)
					{
						Gradients[j][i] = ConvertEntireColorLinearRGBtoSRGB(Gradients[j][i]);
					}
				}
			}
		}

		/// <summary>
		/// Yet another helper function.  This one linearly interpolates between
		/// 2 colors, filling up the output array.
		/// </summary>
		/// <param name="rgb1"> the start color </param>
		/// <param name="rgb2"> the end color </param>
		/// <param name="output"> the output array of colors; must not be null </param>
		private void Interpolate(int rgb1, int rgb2, int[] output)
		{
			// color components
			int a1, r1, g1, b1, da, dr, dg, db;

			// step between interpolated values
			float stepSize = 1.0f / output.Length;

			// extract color components from packed integer
			a1 = (rgb1 >> 24) & 0xff;
			r1 = (rgb1 >> 16) & 0xff;
			g1 = (rgb1 >> 8) & 0xff;
			b1 = (rgb1) & 0xff;

			// calculate the total change in alpha, red, green, blue
			da = ((rgb2 >> 24) & 0xff) - a1;
			dr = ((rgb2 >> 16) & 0xff) - r1;
			dg = ((rgb2 >> 8) & 0xff) - g1;
			db = ((rgb2) & 0xff) - b1;

			// for each step in the interval calculate the in-between color by
			// multiplying the normalized current position by the total color
			// change (0.5 is added to prevent truncation round-off error)
			for (int i = 0; i < output.Length; i++)
			{
				output[i] = (((int)((a1 + i * da * stepSize) + 0.5) << 24)) | (((int)((r1 + i * dr * stepSize) + 0.5) << 16)) | (((int)((g1 + i * dg * stepSize) + 0.5) << 8)) | (((int)((b1 + i * db * stepSize) + 0.5)));
			}
		}

		/// <summary>
		/// Yet another helper function.  This one extracts the color components
		/// of an integer RGB triple, converts them from LinearRGB to SRGB, then
		/// recompacts them into an int.
		/// </summary>
		private int ConvertEntireColorLinearRGBtoSRGB(int rgb)
		{
			// color components
			int a1, r1, g1, b1;

			// extract red, green, blue components
			a1 = (rgb >> 24) & 0xff;
			r1 = (rgb >> 16) & 0xff;
			g1 = (rgb >> 8) & 0xff;
			b1 = (rgb) & 0xff;

			// use the lookup table
			r1 = LinearRGBtoSRGB[r1];
			g1 = LinearRGBtoSRGB[g1];
			b1 = LinearRGBtoSRGB[b1];

			// re-compact the components
			return ((a1 << 24) | (r1 << 16) | (g1 << 8) | (b1));
		}

		/// <summary>
		/// Helper function to index into the gradients array.  This is necessary
		/// because each interval has an array of colors with uniform size 255.
		/// However, the color intervals are not necessarily of uniform length, so
		/// a conversion is required.
		/// </summary>
		/// <param name="position"> the unmanipulated position, which will be mapped
		///                 into the range 0 to 1
		/// @returns integer color to display </param>
		protected internal int IndexIntoGradientsArrays(float position)
		{
			// first, manipulate position value depending on the cycle method
			if (CycleMethod == CycleMethod.NO_CYCLE)
			{
				if (position > 1)
				{
					// upper bound is 1
					position = 1;
				}
				else if (position < 0)
				{
					// lower bound is 0
					position = 0;
				}
			}
			else if (CycleMethod == CycleMethod.REPEAT)
			{
				// get the fractional part
				// (modulo behavior discards integer component)
				position = position - (int)position;

				//position should now be between -1 and 1
				if (position < 0)
				{
					// force it to be in the range 0-1
					position = position + 1;
				}
			} // cycleMethod == CycleMethod.REFLECT
			else
			{
				if (position < 0)
				{
					// take absolute value
					position = -position;
				}

				// get the integer part
				int part = (int)position;

				// get the fractional part
				position = position - part;

				if ((part & 1) == 1)
				{
					// integer part is odd, get reflected color instead
					position = 1 - position;
				}
			}

			// now, get the color based on this 0-1 position...

			if (IsSimpleLookup)
			{
				// easy to compute: just scale index by array size
				return Gradient[(int)(position * FastGradientArraySize)];
			}
			else
			{
				// more complicated computation, to save space

				// for all the gradient interval arrays
				for (int i = 0; i < Gradients.Length; i++)
				{
					if (position < Fractions[i + 1])
					{
						// this is the array we want
						float delta = position - Fractions[i];

						// this is the interval we want
						int index = (int)((delta / NormalizedIntervals[i]) * (GRADIENT_SIZE_INDEX));

						return Gradients[i][index];
					}
				}
			}

			return Gradients[Gradients.Length - 1][GRADIENT_SIZE_INDEX];
		}

		/// <summary>
		/// Helper function to convert a color component in sRGB space to linear
		/// RGB space.  Used to build a static lookup table.
		/// </summary>
		private static int ConvertSRGBtoLinearRGB(int color)
		{
			float input, output;

			input = color / 255.0f;
			if (input <= 0.04045f)
			{
				output = input / 12.92f;
			}
			else
			{
				output = (float)System.Math.Pow((input + 0.055) / 1.055, 2.4);
			}

			return System.Math.Round(output * 255.0f);
		}

		/// <summary>
		/// Helper function to convert a color component in linear RGB space to
		/// SRGB space.  Used to build a static lookup table.
		/// </summary>
		private static int ConvertLinearRGBtoSRGB(int color)
		{
			float input, output;

			input = color / 255.0f;
			if (input <= 0.0031308)
			{
				output = input * 12.92f;
			}
			else
			{
				output = (1.055f * ((float) System.Math.Pow(input, (1.0 / 2.4)))) - 0.055f;
			}

			return System.Math.Round(output * 255.0f);
		}

		/// <summary>
		/// {@inheritDoc}
		/// </summary>
		public Raster GetRaster(int x, int y, int w, int h)
		{
			// If working raster is big enough, reuse it. Otherwise,
			// build a large enough new one.
			Raster raster = Saved;
			if (raster == null || raster.Width < w || raster.Height < h)
			{
				raster = GetCachedRaster(Model, w, h);
				Saved = raster;
			}

			// Access raster internal int array. Because we use a DirectColorModel,
			// we know the DataBuffer is of type DataBufferInt and the SampleModel
			// is SinglePixelPackedSampleModel.
			// Adjust for initial offset in DataBuffer and also for the scanline
			// stride.
			// These calls make the DataBuffer non-acceleratable, but the
			// Raster is never Stable long enough to accelerate anyway...
			DataBufferInt rasterDB = (DataBufferInt)raster.DataBuffer;
			int[] pixels = rasterDB.GetData(0);
			int off = rasterDB.Offset;
			int scanlineStride = ((SinglePixelPackedSampleModel) raster.SampleModel).ScanlineStride;
			int adjust = scanlineStride - w;

			FillRaster(pixels, off, adjust, x, y, w, h); // delegate to subclass

			return raster;
		}

		protected internal abstract void FillRaster(int[] pixels, int off, int adjust, int x, int y, int w, int h);


		/// <summary>
		/// Took this cacheRaster code from GradientPaint. It appears to recycle
		/// rasters for use by any other instance, as long as they are sufficiently
		/// large.
		/// </summary>
		private static Raster GetCachedRaster(ColorModel cm, int w, int h)
		{
			lock (typeof(MultipleGradientPaintContext))
			{
				if (cm == CachedModel)
				{
					if (Cached != null)
					{
						Raster ras = (Raster) Cached.get();
						if (ras != null && ras.Width >= w && ras.Height >= h)
						{
							Cached = null;
							return ras;
						}
					}
				}
				return cm.CreateCompatibleWritableRaster(w, h);
			}
		}

		/// <summary>
		/// Took this cacheRaster code from GradientPaint. It appears to recycle
		/// rasters for use by any other instance, as long as they are sufficiently
		/// large.
		/// </summary>
		private static void PutCachedRaster(ColorModel cm, Raster ras)
		{
			lock (typeof(MultipleGradientPaintContext))
			{
				if (Cached != null)
				{
					Raster cras = (Raster) Cached.get();
					if (cras != null)
					{
						int cw = cras.Width;
						int ch = cras.Height;
						int iw = ras.Width;
						int ih = ras.Height;
						if (cw >= iw && ch >= ih)
						{
							return;
						}
						if (cw * ch >= iw * ih)
						{
							return;
						}
					}
				}
				CachedModel = cm;
				Cached = new WeakReference<Raster>(ras);
			}
		}

		/// <summary>
		/// {@inheritDoc}
		/// </summary>
		public void Dispose()
		{
			if (Saved != null)
			{
				PutCachedRaster(Model, Saved);
				Saved = null;
			}
		}

		/// <summary>
		/// {@inheritDoc}
		/// </summary>
		public ColorModel ColorModel
		{
			get
			{
				return Model;
			}
		}
	}

}
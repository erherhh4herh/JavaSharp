/*
 * Copyright (c) 2006, 2011, Oracle and/or its affiliates. All rights reserved.
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
	/// This is the superclass for Paints which use a multiple color
	/// gradient to fill in their raster.  It provides storage for variables and
	/// enumerated values common to
	/// {@code LinearGradientPaint} and {@code RadialGradientPaint}.
	/// 
	/// @author Nicholas Talian, Vincent Hardy, Jim Graham, Jerry Evans
	/// @since 1.6
	/// </summary>
	public abstract class MultipleGradientPaint : Paint
	{
		public abstract PaintContext CreateContext(image.ColorModel cm, Rectangle deviceBounds, geom.Rectangle2D userBounds, geom.AffineTransform xform, RenderingHints hints);

		/// <summary>
		/// The method to use when painting outside the gradient bounds.
		/// @since 1.6
		/// </summary>
		public enum CycleMethod
		{
			/// <summary>
			/// Use the terminal colors to fill the remaining area.
			/// </summary>
			NO_CYCLE,

			/// <summary>
			/// Cycle the gradient colors start-to-end, end-to-start
			/// to fill the remaining area.
			/// </summary>
			REFLECT,

			/// <summary>
			/// Cycle the gradient colors start-to-end, start-to-end
			/// to fill the remaining area.
			/// </summary>
			REPEAT
		}

		/// <summary>
		/// The color space in which to perform the gradient interpolation.
		/// @since 1.6
		/// </summary>
		public enum ColorSpaceType
		{
			/// <summary>
			/// Indicates that the color interpolation should occur in sRGB space.
			/// </summary>
			SRGB,

			/// <summary>
			/// Indicates that the color interpolation should occur in linearized
			/// RGB space.
			/// </summary>
			LINEAR_RGB
		}

		/// <summary>
		/// The transparency of this paint object. </summary>
		internal readonly int Transparency_Renamed;

		/// <summary>
		/// Gradient keyframe values in the range 0 to 1. </summary>
		internal readonly float[] Fractions_Renamed;

		/// <summary>
		/// Gradient colors. </summary>
		internal readonly Color[] Colors_Renamed;

		/// <summary>
		/// Transform to apply to gradient. </summary>
		internal readonly AffineTransform GradientTransform;

		/// <summary>
		/// The method to use when painting outside the gradient bounds. </summary>
		internal readonly CycleMethod CycleMethod_Renamed;

		/// <summary>
		/// The color space in which to perform the gradient interpolation. </summary>
		internal readonly ColorSpaceType ColorSpace_Renamed;

		/// <summary>
		/// The following fields are used only by MultipleGradientPaintContext
		/// to cache certain values that remain constant and do not need to be
		/// recalculated for each context created from this paint instance.
		/// </summary>
		internal ColorModel Model;
		internal float[] NormalizedIntervals;
		internal bool IsSimpleLookup;
		internal SoftReference<int[][]> Gradients;
		internal SoftReference<int[]> Gradient;
		internal int FastGradientArraySize;

		/// <summary>
		/// Package-private constructor.
		/// </summary>
		/// <param name="fractions"> numbers ranging from 0.0 to 1.0 specifying the
		///                  distribution of colors along the gradient </param>
		/// <param name="colors"> array of colors corresponding to each fractional value </param>
		/// <param name="cycleMethod"> either {@code NO_CYCLE}, {@code REFLECT},
		///                    or {@code REPEAT} </param>
		/// <param name="colorSpace"> which color space to use for interpolation,
		///                   either {@code SRGB} or {@code LINEAR_RGB} </param>
		/// <param name="gradientTransform"> transform to apply to the gradient
		/// </param>
		/// <exception cref="NullPointerException">
		/// if {@code fractions} array is null,
		/// or {@code colors} array is null,
		/// or {@code gradientTransform} is null,
		/// or {@code cycleMethod} is null,
		/// or {@code colorSpace} is null </exception>
		/// <exception cref="IllegalArgumentException">
		/// if {@code fractions.length != colors.length},
		/// or {@code colors} is less than 2 in size,
		/// or a {@code fractions} value is less than 0.0 or greater than 1.0,
		/// or the {@code fractions} are not provided in strictly increasing order </exception>
		internal MultipleGradientPaint(float[] fractions, Color[] colors, CycleMethod cycleMethod, ColorSpaceType colorSpace, AffineTransform gradientTransform)
		{
			if (fractions == null)
			{
				throw new NullPointerException("Fractions array cannot be null");
			}

			if (colors == null)
			{
				throw new NullPointerException("Colors array cannot be null");
			}

			if (cycleMethod == null)
			{
				throw new NullPointerException("Cycle method cannot be null");
			}

			if (colorSpace == null)
			{
				throw new NullPointerException("Color space cannot be null");
			}

			if (gradientTransform == null)
			{
				throw new NullPointerException("Gradient transform cannot be " + "null");
			}

			if (fractions.Length != colors.Length)
			{
				throw new IllegalArgumentException("Colors and fractions must " + "have equal size");
			}

			if (colors.Length < 2)
			{
				throw new IllegalArgumentException("User must specify at least " + "2 colors");
			}

			// check that values are in the proper range and progress
			// in increasing order from 0 to 1
			float previousFraction = -1.0f;
			foreach (float currentFraction in fractions)
			{
				if (currentFraction < 0f || currentFraction > 1f)
				{
					throw new IllegalArgumentException("Fraction values must " + "be in the range 0 to 1: " + currentFraction);
				}

				if (currentFraction <= previousFraction)
				{
					throw new IllegalArgumentException("Keyframe fractions " + "must be increasing: " + currentFraction);
				}

				previousFraction = currentFraction;
			}

			// We have to deal with the cases where the first gradient stop is not
			// equal to 0 and/or the last gradient stop is not equal to 1.
			// In both cases, create a new point and replicate the previous
			// extreme point's color.
			bool fixFirst = false;
			bool fixLast = false;
			int len = fractions.Length;
			int off = 0;

			if (fractions[0] != 0f)
			{
				// first stop is not equal to zero, fix this condition
				fixFirst = true;
				len++;
				off++;
			}
			if (fractions[fractions.Length - 1] != 1f)
			{
				// last stop is not equal to one, fix this condition
				fixLast = true;
				len++;
			}

			this.Fractions_Renamed = new float[len];
			System.Array.Copy(fractions, 0, this.Fractions_Renamed, off, fractions.Length);
			this.Colors_Renamed = new Color[len];
			System.Array.Copy(colors, 0, this.Colors_Renamed, off, colors.Length);

			if (fixFirst)
			{
				this.Fractions_Renamed[0] = 0f;
				this.Colors_Renamed[0] = colors[0];
			}
			if (fixLast)
			{
				this.Fractions_Renamed[len - 1] = 1f;
				this.Colors_Renamed[len - 1] = colors[colors.Length - 1];
			}

			// copy some flags
			this.ColorSpace_Renamed = colorSpace;
			this.CycleMethod_Renamed = cycleMethod;

			// copy the gradient transform
			this.GradientTransform = new AffineTransform(gradientTransform);

			// determine transparency
			bool opaque = true;
			for (int i = 0; i < colors.Length; i++)
			{
				opaque = opaque && (colors[i].Alpha == 0xff);
			}
			this.Transparency_Renamed = opaque ? Transparency_Fields.OPAQUE : Transparency_Fields.TRANSLUCENT;
		}

		/// <summary>
		/// Returns a copy of the array of floats used by this gradient
		/// to calculate color distribution.
		/// The returned array always has 0 as its first value and 1 as its
		/// last value, with increasing values in between.
		/// </summary>
		/// <returns> a copy of the array of floats used by this gradient to
		/// calculate color distribution </returns>
		public float[] Fractions
		{
			get
			{
				return Arrays.CopyOf(Fractions_Renamed, Fractions_Renamed.Length);
			}
		}

		/// <summary>
		/// Returns a copy of the array of colors used by this gradient.
		/// The first color maps to the first value in the fractions array,
		/// and the last color maps to the last value in the fractions array.
		/// </summary>
		/// <returns> a copy of the array of colors used by this gradient </returns>
		public Color[] Colors
		{
			get
			{
				return Arrays.CopyOf(Colors_Renamed, Colors_Renamed.Length);
			}
		}

		/// <summary>
		/// Returns the enumerated type which specifies cycling behavior.
		/// </summary>
		/// <returns> the enumerated type which specifies cycling behavior </returns>
		public CycleMethod CycleMethod
		{
			get
			{
				return CycleMethod_Renamed;
			}
		}

		/// <summary>
		/// Returns the enumerated type which specifies color space for
		/// interpolation.
		/// </summary>
		/// <returns> the enumerated type which specifies color space for
		/// interpolation </returns>
		public ColorSpaceType ColorSpace
		{
			get
			{
				return ColorSpace_Renamed;
			}
		}

		/// <summary>
		/// Returns a copy of the transform applied to the gradient.
		/// 
		/// <para>
		/// Note that if no transform is applied to the gradient
		/// when it is created, the identity transform is used.
		/// 
		/// </para>
		/// </summary>
		/// <returns> a copy of the transform applied to the gradient </returns>
		public AffineTransform Transform
		{
			get
			{
				return new AffineTransform(GradientTransform);
			}
		}

		/// <summary>
		/// Returns the transparency mode for this {@code Paint} object.
		/// </summary>
		/// <returns> {@code OPAQUE} if all colors used by this
		///         {@code Paint} object are opaque,
		///         {@code TRANSLUCENT} if at least one of the
		///         colors used by this {@code Paint} object is not opaque. </returns>
		/// <seealso cref= java.awt.Transparency </seealso>
		public int Transparency
		{
			get
			{
				return Transparency_Renamed;
			}
		}
	}

}
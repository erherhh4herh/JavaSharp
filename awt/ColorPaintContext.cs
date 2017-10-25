/*
 * Copyright (c) 1997, 2007, Oracle and/or its affiliates. All rights reserved.
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

	using IntegerComponentRaster = sun.awt.image.IntegerComponentRaster;

	internal class ColorPaintContext : PaintContext
	{
		internal int Color;
		internal WritableRaster SavedTile;

		protected internal ColorPaintContext(int color, ColorModel cm)
		{
			this.Color = color;
		}

		public virtual void Dispose()
		{
		}

		/*
		 * Returns the RGB value representing the color in the default sRGB
		 * {@link ColorModel}.
		 * (Bits 24-31 are alpha, 16-23 are red, 8-15 are green, 0-7 are
		 * blue).
		 * @return the RGB value of the color in the default sRGB
		 *         <code>ColorModel</code>.
		 * @see java.awt.image.ColorModel#getRGBdefault
		 * @see #getRed
		 * @see #getGreen
		 * @see #getBlue
		 */
		internal virtual int RGB
		{
			get
			{
				return Color;
			}
		}

		public virtual ColorModel ColorModel
		{
			get
			{
				return ColorModel.RGBdefault;
			}
		}

		public virtual Raster GetRaster(int x, int y, int w, int h)
		{
			lock (this)
			{
				WritableRaster t = SavedTile;
        
				if (t == null || w > t.Width || h > t.Height)
				{
					t = ColorModel.CreateCompatibleWritableRaster(w, h);
					IntegerComponentRaster icr = (IntegerComponentRaster) t;
					Arrays.Fill(icr.DataStorage, Color);
					// Note - markDirty is probably unnecessary since icr is brand new
					icr.markDirty();
					if (w <= 64 && h <= 64)
					{
						SavedTile = t;
					}
				}
        
				return t;
			}
		}
	}

}
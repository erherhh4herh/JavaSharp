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

namespace java.awt
{

	using IntegerComponentRaster = sun.awt.image.IntegerComponentRaster;

	internal class GradientPaintContext : PaintContext
	{
		internal static ColorModel Xrgbmodel = new DirectColorModel(24, 0x00ff0000, 0x0000ff00, 0x000000ff);
		internal static ColorModel Xbgrmodel = new DirectColorModel(24, 0x000000ff, 0x0000ff00, 0x00ff0000);

		internal static ColorModel CachedModel;
		internal static WeakReference<Raster> Cached;

		internal static Raster GetCachedRaster(ColorModel cm, int w, int h)
		{
			lock (typeof(GradientPaintContext))
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

		internal static void PutCachedRaster(ColorModel cm, Raster ras)
		{
			lock (typeof(GradientPaintContext))
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
				Cached = new WeakReference<>(ras);
			}
		}

		internal double X1;
		internal double Y1;
		internal double Dx;
		internal double Dy;
		internal bool Cyclic;
		internal int[] Interp;
		internal Raster Saved;
		internal ColorModel Model;

		public GradientPaintContext(ColorModel cm, Point2D p1, Point2D p2, AffineTransform xform, Color c1, Color c2, bool cyclic)
		{
			// First calculate the distance moved in user space when
			// we move a single unit along the X & Y axes in device space.
			Point2D xvec = new Point2D.Double(1, 0);
			Point2D yvec = new Point2D.Double(0, 1);
			try
			{
				AffineTransform inverse = xform.CreateInverse();
				inverse.DeltaTransform(xvec, xvec);
				inverse.DeltaTransform(yvec, yvec);
			}
			catch (NoninvertibleTransformException)
			{
				xvec.SetLocation(0, 0);
				yvec.SetLocation(0, 0);
			}

			// Now calculate the (square of the) user space distance
			// between the anchor points. This value equals:
			//     (UserVec . UserVec)
			double udx = p2.X - p1.X;
			double udy = p2.Y - p1.Y;
			double ulenSq = udx * udx + udy * udy;

			if (ulenSq <= Double.Epsilon)
			{
				Dx = 0;
				Dy = 0;
			}
			else
			{
				// Now calculate the proportional distance moved along the
				// vector from p1 to p2 when we move a unit along X & Y in
				// device space.
				//
				// The length of the projection of the Device Axis Vector is
				// its dot product with the Unit User Vector:
				//     (DevAxisVec . (UserVec / Len(UserVec))
				//
				// The "proportional" length is that length divided again
				// by the length of the User Vector:
				//     (DevAxisVec . (UserVec / Len(UserVec))) / Len(UserVec)
				// which simplifies to:
				//     ((DevAxisVec . UserVec) / Len(UserVec)) / Len(UserVec)
				// which simplifies to:
				//     (DevAxisVec . UserVec) / LenSquared(UserVec)
				Dx = (xvec.X * udx + xvec.Y * udy) / ulenSq;
				Dy = (yvec.X * udx + yvec.Y * udy) / ulenSq;

				if (cyclic)
				{
					Dx = Dx % 1.0;
					Dy = Dy % 1.0;
				}
				else
				{
					// We are acyclic
					if (Dx < 0)
					{
						// If we are using the acyclic form below, we need
						// dx to be non-negative for simplicity of scanning
						// across the scan lines for the transition points.
						// To ensure that constraint, we negate the dx/dy
						// values and swap the points and colors.
						Point2D p = p1;
						p1 = p2;
						p2 = p;
						Color c = c1;
						c1 = c2;
						c2 = c;
						Dx = -Dx;
						Dy = -Dy;
					}
				}
			}

			Point2D dp1 = xform.Transform(p1, null);
			this.X1 = dp1.X;
			this.Y1 = dp1.Y;

			this.Cyclic = cyclic;
			int rgb1 = c1.RGB;
			int rgb2 = c2.RGB;
			int a1 = (rgb1 >> 24) & 0xff;
			int r1 = (rgb1 >> 16) & 0xff;
			int g1 = (rgb1 >> 8) & 0xff;
			int b1 = (rgb1) & 0xff;
			int da = ((rgb2 >> 24) & 0xff) - a1;
			int dr = ((rgb2 >> 16) & 0xff) - r1;
			int dg = ((rgb2 >> 8) & 0xff) - g1;
			int db = ((rgb2) & 0xff) - b1;
			if (a1 == 0xff && da == 0)
			{
				Model = Xrgbmodel;
				if (cm is DirectColorModel)
				{
					DirectColorModel dcm = (DirectColorModel) cm;
					int tmp = dcm.AlphaMask;
					if ((tmp == 0 || tmp == 0xff) && dcm.RedMask == 0xff && dcm.GreenMask == 0xff00 && dcm.BlueMask == 0xff0000)
					{
						Model = Xbgrmodel;
						tmp = r1;
						r1 = b1;
						b1 = tmp;
						tmp = dr;
						dr = db;
						db = tmp;
					}
				}
			}
			else
			{
				Model = ColorModel.RGBdefault;
			}
			Interp = new int[cyclic ? 513 : 257];
			for (int i = 0; i <= 256; i++)
			{
				float rel = i / 256.0f;
				int rgb = (((int)(a1 + da * rel)) << 24) | (((int)(r1 + dr * rel)) << 16) | (((int)(g1 + dg * rel)) << 8) | (((int)(b1 + db * rel)));
				Interp[i] = rgb;
				if (cyclic)
				{
					Interp[512 - i] = rgb;
				}
			}
		}

		/// <summary>
		/// Release the resources allocated for the operation.
		/// </summary>
		public virtual void Dispose()
		{
			if (Saved != null)
			{
				PutCachedRaster(Model, Saved);
				Saved = null;
			}
		}

		/// <summary>
		/// Return the ColorModel of the output.
		/// </summary>
		public virtual ColorModel ColorModel
		{
			get
			{
				return Model;
			}
		}

		/// <summary>
		/// Return a Raster containing the colors generated for the graphics
		/// operation. </summary>
		/// <param name="x">,y,w,h The area in device space for which colors are
		/// generated. </param>
		public virtual Raster GetRaster(int x, int y, int w, int h)
		{
			double rowrel = (x - X1) * Dx + (y - Y1) * Dy;

			Raster rast = Saved;
			if (rast == null || rast.Width < w || rast.Height < h)
			{
				rast = GetCachedRaster(Model, w, h);
				Saved = rast;
			}
			IntegerComponentRaster irast = (IntegerComponentRaster) rast;
			int off = irast.getDataOffset(0);
			int adjust = irast.ScanlineStride - w;
			int[] pixels = irast.DataStorage;

			if (Cyclic)
			{
				CycleFillRaster(pixels, off, adjust, w, h, rowrel, Dx, Dy);
			}
			else
			{
				ClipFillRaster(pixels, off, adjust, w, h, rowrel, Dx, Dy);
			}

			irast.markDirty();

			return rast;
		}

		internal virtual void CycleFillRaster(int[] pixels, int off, int adjust, int w, int h, double rowrel, double dx, double dy)
		{
			rowrel = rowrel % 2.0;
			int irowrel = ((int)(rowrel * (1 << 30))) << 1;
			int idx = (int)(-dx * (1 << 31));
			int idy = (int)(-dy * (1 << 31));
			while (--h >= 0)
			{
				int icolrel = irowrel;
				for (int j = w; j > 0; j--)
				{
					pixels[off++] = Interp[(int)((uint)icolrel >> 23)];
					icolrel += idx;
				}

				off += adjust;
				irowrel += idy;
			}
		}

		internal virtual void ClipFillRaster(int[] pixels, int off, int adjust, int w, int h, double rowrel, double dx, double dy)
		{
			while (--h >= 0)
			{
				double colrel = rowrel;
				int j = w;
				if (colrel <= 0.0)
				{
					int rgb = Interp[0];
					do
					{
						pixels[off++] = rgb;
						colrel += dx;
					} while (--j > 0 && colrel <= 0.0);
				}
				while (colrel < 1.0 && --j >= 0)
				{
					pixels[off++] = Interp[(int)(colrel * 256)];
					colrel += dx;
				}
				if (j > 0)
				{
					int rgb = Interp[256];
					do
					{
						pixels[off++] = rgb;
					} while (--j > 0);
				}

				off += adjust;
				rowrel += dy;
			}
		}
	}

}
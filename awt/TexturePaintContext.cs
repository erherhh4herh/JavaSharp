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

namespace java.awt
{

	using SunWritableRaster = sun.awt.image.SunWritableRaster;
	using IntegerInterleavedRaster = sun.awt.image.IntegerInterleavedRaster;
	using ByteInterleavedRaster = sun.awt.image.ByteInterleavedRaster;

	internal abstract class TexturePaintContext : PaintContext
	{
		public static ColorModel Xrgbmodel = new DirectColorModel(24, 0xff0000, 0xff00, 0xff);
		public static ColorModel Argbmodel = ColorModel.RGBdefault;

		internal ColorModel ColorModel_Renamed;
		internal int BWidth;
		internal int BHeight;
		internal int MaxWidth;

		internal WritableRaster OutRas;

		internal double XOrg;
		internal double YOrg;
		internal double IncXAcross;
		internal double IncYAcross;
		internal double IncXDown;
		internal double IncYDown;

		internal int Colincx;
		internal int Colincy;
		internal int Colincxerr;
		internal int Colincyerr;
		internal int Rowincx;
		internal int Rowincy;
		internal int Rowincxerr;
		internal int Rowincyerr;

		public static PaintContext GetContext(BufferedImage bufImg, AffineTransform xform, RenderingHints hints, Rectangle devBounds)
		{
			WritableRaster raster = bufImg.Raster;
			ColorModel cm = bufImg.ColorModel;
			int maxw = devBounds.Width_Renamed;
			Object val = hints[RenderingHints.KEY_INTERPOLATION];
			bool filter = (val == null ? (hints[RenderingHints.KEY_RENDERING] == RenderingHints.VALUE_RENDER_QUALITY) : (val != RenderingHints.VALUE_INTERPOLATION_NEAREST_NEIGHBOR));
			if (raster is IntegerInterleavedRaster && (!filter || IsFilterableDCM(cm)))
			{
				IntegerInterleavedRaster iir = (IntegerInterleavedRaster) raster;
				if (iir.NumDataElements == 1 && iir.PixelStride == 1)
				{
					return new Int(iir, cm, xform, maxw, filter);
				}
			}
			else if (raster is ByteInterleavedRaster)
			{
				ByteInterleavedRaster bir = (ByteInterleavedRaster) raster;
				if (bir.NumDataElements == 1 && bir.PixelStride == 1)
				{
					if (filter)
					{
						if (IsFilterableICM(cm))
						{
							return new ByteFilter(bir, cm, xform, maxw);
						}
					}
					else
					{
						return new Byte(bir, cm, xform, maxw);
					}
				}
			}
			return new Any(raster, cm, xform, maxw, filter);
		}

		public static bool IsFilterableICM(ColorModel cm)
		{
			if (cm is IndexColorModel)
			{
				IndexColorModel icm = (IndexColorModel) cm;
				if (icm.MapSize <= 256)
				{
					return true;
				}
			}
			return false;
		}

		public static bool IsFilterableDCM(ColorModel cm)
		{
			if (cm is DirectColorModel)
			{
				DirectColorModel dcm = (DirectColorModel) cm;
				return (IsMaskOK(dcm.AlphaMask, true) && IsMaskOK(dcm.RedMask, false) && IsMaskOK(dcm.GreenMask, false) && IsMaskOK(dcm.BlueMask, false));
			}
			return false;
		}

		public static bool IsMaskOK(int mask, bool canbezero)
		{
			if (canbezero && mask == 0)
			{
				return true;
			}
			return (mask == 0xff || mask == 0xff00 || mask == 0xff0000 || mask == 0xff000000);
		}

		public static ColorModel GetInternedColorModel(ColorModel cm)
		{
			if (Xrgbmodel == cm || Xrgbmodel.Equals(cm))
			{
				return Xrgbmodel;
			}
			if (Argbmodel == cm || Argbmodel.Equals(cm))
			{
				return Argbmodel;
			}
			return cm;
		}

		internal TexturePaintContext(ColorModel cm, AffineTransform xform, int bWidth, int bHeight, int maxw)
		{
			this.ColorModel_Renamed = GetInternedColorModel(cm);
			this.BWidth = bWidth;
			this.BHeight = bHeight;
			this.MaxWidth = maxw;

			try
			{
				xform = xform.CreateInverse();
			}
			catch (NoninvertibleTransformException)
			{
				xform.SetToScale(0, 0);
			}
			this.IncXAcross = Mod(xform.ScaleX, bWidth);
			this.IncYAcross = Mod(xform.ShearY, bHeight);
			this.IncXDown = Mod(xform.ShearX, bWidth);
			this.IncYDown = Mod(xform.ScaleY, bHeight);
			this.XOrg = xform.TranslateX;
			this.YOrg = xform.TranslateY;
			this.Colincx = (int) IncXAcross;
			this.Colincy = (int) IncYAcross;
			this.Colincxerr = FractAsInt(IncXAcross);
			this.Colincyerr = FractAsInt(IncYAcross);
			this.Rowincx = (int) IncXDown;
			this.Rowincy = (int) IncYDown;
			this.Rowincxerr = FractAsInt(IncXDown);
			this.Rowincyerr = FractAsInt(IncYDown);

		}

		internal static int FractAsInt(double d)
		{
			return (int)((d % 1.0) * Integer.MaxValue);
		}

		internal static double Mod(double num, double den)
		{
			num = num % den;
			if (num < 0)
			{
				num += den;
				if (num >= den)
				{
					// For very small negative numerators, the answer might
					// be such a tiny bit less than den that the difference
					// is smaller than the mantissa of a double allows and
					// the result would then be rounded to den.  If that is
					// the case then we map that number to 0 as the nearest
					// modulus representation.
					num = 0;
				}
			}
			return num;
		}

		/// <summary>
		/// Release the resources allocated for the operation.
		/// </summary>
		public virtual void Dispose()
		{
			DropRaster(ColorModel_Renamed, OutRas);
		}

		/// <summary>
		/// Return the ColorModel of the output.
		/// </summary>
		public virtual ColorModel ColorModel
		{
			get
			{
				return ColorModel_Renamed;
			}
		}

		/// <summary>
		/// Return a Raster containing the colors generated for the graphics
		/// operation. </summary>
		/// <param name="x">,y,w,h The area in device space for which colors are
		/// generated. </param>
		public virtual Raster GetRaster(int x, int y, int w, int h)
		{
			if (OutRas == null || OutRas.Width < w || OutRas.Height < h)
			{
				// If h==1, we will probably get lots of "scanline" rects
				OutRas = MakeRaster((h == 1 ? System.Math.Max(w, MaxWidth) : w), h);
			}
			double X = Mod(XOrg + x * IncXAcross + y * IncXDown, BWidth);
			double Y = Mod(YOrg + x * IncYAcross + y * IncYDown, BHeight);

			SetRaster((int) X, (int) Y, FractAsInt(X), FractAsInt(Y), w, h, BWidth, BHeight, Colincx, Colincxerr, Colincy, Colincyerr, Rowincx, Rowincxerr, Rowincy, Rowincyerr);

			SunWritableRaster.markDirty(OutRas);

			return OutRas;
		}

		private static WeakReference<Raster> XrgbRasRef;
		private static WeakReference<Raster> ArgbRasRef;

		internal static WritableRaster MakeRaster(ColorModel cm, Raster srcRas, int w, int h)
		{
			lock (typeof(TexturePaintContext))
			{
				if (Xrgbmodel == cm)
				{
					if (XrgbRasRef != null)
					{
						WritableRaster wr = (WritableRaster) XrgbRasRef.get();
						if (wr != null && wr.Width >= w && wr.Height >= h)
						{
							XrgbRasRef = null;
							return wr;
						}
					}
					// If we are going to cache this Raster, make it non-tiny
					if (w <= 32 && h <= 32)
					{
						w = h = 32;
					}
				}
				else if (Argbmodel == cm)
				{
					if (ArgbRasRef != null)
					{
						WritableRaster wr = (WritableRaster) ArgbRasRef.get();
						if (wr != null && wr.Width >= w && wr.Height >= h)
						{
							ArgbRasRef = null;
							return wr;
						}
					}
					// If we are going to cache this Raster, make it non-tiny
					if (w <= 32 && h <= 32)
					{
						w = h = 32;
					}
				}
				if (srcRas != null)
				{
					return srcRas.CreateCompatibleWritableRaster(w, h);
				}
				else
				{
					return cm.CreateCompatibleWritableRaster(w, h);
				}
			}
		}

		internal static void DropRaster(ColorModel cm, Raster outRas)
		{
			lock (typeof(TexturePaintContext))
			{
				if (outRas == null)
				{
					return;
				}
				if (Xrgbmodel == cm)
				{
					XrgbRasRef = new WeakReference<>(outRas);
				}
				else if (Argbmodel == cm)
				{
					ArgbRasRef = new WeakReference<>(outRas);
				}
			}
		}

		private static WeakReference<Raster> ByteRasRef;

		internal static WritableRaster MakeByteRaster(Raster srcRas, int w, int h)
		{
			lock (typeof(TexturePaintContext))
			{
				if (ByteRasRef != null)
				{
					WritableRaster wr = (WritableRaster) ByteRasRef.get();
					if (wr != null && wr.Width >= w && wr.Height >= h)
					{
						ByteRasRef = null;
						return wr;
					}
				}
				// If we are going to cache this Raster, make it non-tiny
				if (w <= 32 && h <= 32)
				{
					w = h = 32;
				}
				return srcRas.CreateCompatibleWritableRaster(w, h);
			}
		}

		internal static void DropByteRaster(Raster outRas)
		{
			lock (typeof(TexturePaintContext))
			{
				if (outRas == null)
				{
					return;
				}
				ByteRasRef = new WeakReference<>(outRas);
			}
		}

		public abstract WritableRaster MakeRaster(int w, int h);
		public abstract void SetRaster(int x, int y, int xerr, int yerr, int w, int h, int bWidth, int bHeight, int colincx, int colincxerr, int colincy, int colincyerr, int rowincx, int rowincxerr, int rowincy, int rowincyerr);

		/*
		 * Blends the four ARGB values in the rgbs array using the factors
		 * described by xmul and ymul in the following ratio:
		 *
		 *     rgbs[0] * (1-xmul) * (1-ymul) +
		 *     rgbs[1] * (  xmul) * (1-ymul) +
		 *     rgbs[2] * (1-xmul) * (  ymul) +
		 *     rgbs[3] * (  xmul) * (  ymul)
		 *
		 * xmul and ymul are integer values in the half-open range [0, 2^31)
		 * where 0 == 0.0 and 2^31 == 1.0.
		 *
		 * Note that since the range is half-open, the values are always
		 * logically less than 1.0.  This makes sense because while choosing
		 * pixels to blend, when the error values reach 1.0 we move to the
		 * next pixel and reset them to 0.0.
		 */
		public static int Blend(int[] rgbs, int xmul, int ymul)
		{
			// xmul/ymul are 31 bits wide, (0 => 2^31-1)
			// shift them to 12 bits wide, (0 => 2^12-1)
			xmul = ((int)((uint)xmul >> 19));
			ymul = ((int)((uint)ymul >> 19));
			int accumA, accumR, accumG, accumB;
			accumA = accumR = accumG = accumB = 0;
			for (int i = 0; i < 4; i++)
			{
				int rgb = rgbs[i];
				// The complement of the [xy]mul values (1-[xy]mul) can result
				// in new values in the range (1 => 2^12).  Thus for any given
				// loop iteration, the values could be anywhere in (0 => 2^12).
				xmul = (1 << 12) - xmul;
				if ((i & 1) == 0)
				{
					ymul = (1 << 12) - ymul;
				}
				// xmul and ymul are each 12 bits (0 => 2^12)
				// factor is thus 24 bits (0 => 2^24)
				int factor = xmul * ymul;
				if (factor != 0)
				{
					// accum variables will accumulate 32 bits
					// bytes extracted from rgb fit in 8 bits (0 => 255)
					// byte * factor thus fits in 32 bits (0 => 255 * 2^24)
					accumA += ((((int)((uint)rgb >> 24))) * factor);
					accumR += ((((int)((uint)rgb >> 16)) & 0xff) * factor);
					accumG += ((((int)((uint)rgb >> 8)) & 0xff) * factor);
					accumB += (((rgb) & 0xff) * factor);
				}
			}
			return ((((int)((uint)(accumA + (1 << 23)) >> 24)) << 24) | (((int)((uint)(accumR + (1 << 23)) >> 24)) << 16) | (((int)((uint)(accumG + (1 << 23)) >> 24)) << 8) | (((int)((uint)(accumB + (1 << 23)) >> 24))));
		}

		internal class Int : TexturePaintContext
		{
			internal IntegerInterleavedRaster SrcRas;
			internal int[] InData;
			internal int InOff;
			internal int InSpan;
			internal int[] OutData;
			internal int OutOff;
			internal int OutSpan;
			internal bool Filter;

			public Int(IntegerInterleavedRaster srcRas, ColorModel cm, AffineTransform xform, int maxw, bool filter) : base(cm, xform, srcRas.Width, srcRas.Height, maxw)
			{
				this.SrcRas = srcRas;
				this.InData = srcRas.DataStorage;
				this.InSpan = srcRas.ScanlineStride;
				this.InOff = srcRas.getDataOffset(0);
				this.Filter = filter;
			}

			public override WritableRaster MakeRaster(int w, int h)
			{
				WritableRaster ras = MakeRaster(ColorModel_Renamed, SrcRas, w, h);
				IntegerInterleavedRaster iiRas = (IntegerInterleavedRaster) ras;
				OutData = iiRas.DataStorage;
				OutSpan = iiRas.ScanlineStride;
				OutOff = iiRas.getDataOffset(0);
				return ras;
			}

			public override void SetRaster(int x, int y, int xerr, int yerr, int w, int h, int bWidth, int bHeight, int colincx, int colincxerr, int colincy, int colincyerr, int rowincx, int rowincxerr, int rowincy, int rowincyerr)
			{
				int[] inData = this.InData;
				int[] outData = this.OutData;
				int @out = OutOff;
				int inSpan = this.InSpan;
				int inOff = this.InOff;
				int outSpan = this.OutSpan;
				bool filter = this.Filter;
				bool normalx = (colincx == 1 && colincxerr == 0 && colincy == 0 && colincyerr == 0) && !filter;
				int rowx = x;
				int rowy = y;
				int rowxerr = xerr;
				int rowyerr = yerr;
				if (normalx)
				{
					outSpan -= w;
				}
				int[] rgbs = filter ? new int[4] : null;
				for (int j = 0; j < h; j++)
				{
					if (normalx)
					{
						int @in = inOff + rowy * inSpan + bWidth;
						x = bWidth - rowx;
						@out += w;
						if (bWidth >= 32)
						{
							int i = w;
							while (i > 0)
							{
								int copyw = (i < x) ? i : x;
								System.Array.Copy(inData, @in - x, outData, @out - i, copyw);
								i -= copyw;
								if ((x -= copyw) == 0)
								{
									x = bWidth;
								}
							}
						}
						else
						{
							for (int i = w; i > 0; i--)
							{
								outData[@out - i] = inData[@in - x];
								if (--x == 0)
								{
									x = bWidth;
								}
							}
						}
					}
					else
					{
						x = rowx;
						y = rowy;
						xerr = rowxerr;
						yerr = rowyerr;
						for (int i = 0; i < w; i++)
						{
							if (filter)
							{
								int nextx, nexty;
								if ((nextx = x + 1) >= bWidth)
								{
									nextx = 0;
								}
								if ((nexty = y + 1) >= bHeight)
								{
									nexty = 0;
								}
								rgbs[0] = inData[inOff + y * inSpan + x];
								rgbs[1] = inData[inOff + y * inSpan + nextx];
								rgbs[2] = inData[inOff + nexty * inSpan + x];
								rgbs[3] = inData[inOff + nexty * inSpan + nextx];
								outData[@out + i] = TexturePaintContext.Blend(rgbs, xerr, yerr);
							}
							else
							{
								outData[@out + i] = inData[inOff + y * inSpan + x];
							}
							if ((xerr += colincxerr) < 0)
							{
								xerr &= Integer.MaxValue;
								x++;
							}
							if ((x += colincx) >= bWidth)
							{
								x -= bWidth;
							}
							if ((yerr += colincyerr) < 0)
							{
								yerr &= Integer.MaxValue;
								y++;
							}
							if ((y += colincy) >= bHeight)
							{
								y -= bHeight;
							}
						}
					}
					if ((rowxerr += rowincxerr) < 0)
					{
						rowxerr &= Integer.MaxValue;
						rowx++;
					}
					if ((rowx += rowincx) >= bWidth)
					{
						rowx -= bWidth;
					}
					if ((rowyerr += rowincyerr) < 0)
					{
						rowyerr &= Integer.MaxValue;
						rowy++;
					}
					if ((rowy += rowincy) >= bHeight)
					{
						rowy -= bHeight;
					}
					@out += outSpan;
				}
			}
		}

		internal class Byte : TexturePaintContext
		{
			internal ByteInterleavedRaster SrcRas;
			internal sbyte[] InData;
			internal int InOff;
			internal int InSpan;
			internal sbyte[] OutData;
			internal int OutOff;
			internal int OutSpan;

			public Byte(ByteInterleavedRaster srcRas, ColorModel cm, AffineTransform xform, int maxw) : base(cm, xform, srcRas.Width, srcRas.Height, maxw)
			{
				this.SrcRas = srcRas;
				this.InData = srcRas.DataStorage;
				this.InSpan = srcRas.ScanlineStride;
				this.InOff = srcRas.getDataOffset(0);
			}

			public override WritableRaster MakeRaster(int w, int h)
			{
				WritableRaster ras = MakeByteRaster(SrcRas, w, h);
				ByteInterleavedRaster biRas = (ByteInterleavedRaster) ras;
				OutData = biRas.DataStorage;
				OutSpan = biRas.ScanlineStride;
				OutOff = biRas.getDataOffset(0);
				return ras;
			}

			public override void Dispose()
			{
				DropByteRaster(OutRas);
			}

			public override void SetRaster(int x, int y, int xerr, int yerr, int w, int h, int bWidth, int bHeight, int colincx, int colincxerr, int colincy, int colincyerr, int rowincx, int rowincxerr, int rowincy, int rowincyerr)
			{
				sbyte[] inData = this.InData;
				sbyte[] outData = this.OutData;
				int @out = OutOff;
				int inSpan = this.InSpan;
				int inOff = this.InOff;
				int outSpan = this.OutSpan;
				bool normalx = (colincx == 1 && colincxerr == 0 && colincy == 0 && colincyerr == 0);
				int rowx = x;
				int rowy = y;
				int rowxerr = xerr;
				int rowyerr = yerr;
				if (normalx)
				{
					outSpan -= w;
				}
				for (int j = 0; j < h; j++)
				{
					if (normalx)
					{
						int @in = inOff + rowy * inSpan + bWidth;
						x = bWidth - rowx;
						@out += w;
						if (bWidth >= 32)
						{
							int i = w;
							while (i > 0)
							{
								int copyw = (i < x) ? i : x;
								System.Array.Copy(inData, @in - x, outData, @out - i, copyw);
								i -= copyw;
								if ((x -= copyw) == 0)
								{
									x = bWidth;
								}
							}
						}
						else
						{
							for (int i = w; i > 0; i--)
							{
								outData[@out - i] = inData[@in - x];
								if (--x == 0)
								{
									x = bWidth;
								}
							}
						}
					}
					else
					{
						x = rowx;
						y = rowy;
						xerr = rowxerr;
						yerr = rowyerr;
						for (int i = 0; i < w; i++)
						{
							outData[@out + i] = inData[inOff + y * inSpan + x];
							if ((xerr += colincxerr) < 0)
							{
								xerr &= Integer.MaxValue;
								x++;
							}
							if ((x += colincx) >= bWidth)
							{
								x -= bWidth;
							}
							if ((yerr += colincyerr) < 0)
							{
								yerr &= Integer.MaxValue;
								y++;
							}
							if ((y += colincy) >= bHeight)
							{
								y -= bHeight;
							}
						}
					}
					if ((rowxerr += rowincxerr) < 0)
					{
						rowxerr &= Integer.MaxValue;
						rowx++;
					}
					if ((rowx += rowincx) >= bWidth)
					{
						rowx -= bWidth;
					}
					if ((rowyerr += rowincyerr) < 0)
					{
						rowyerr &= Integer.MaxValue;
						rowy++;
					}
					if ((rowy += rowincy) >= bHeight)
					{
						rowy -= bHeight;
					}
					@out += outSpan;
				}
			}
		}

		internal class ByteFilter : TexturePaintContext
		{
			internal ByteInterleavedRaster SrcRas;
			internal int[] InPalette;
			internal sbyte[] InData;
			internal int InOff;
			internal int InSpan;
			internal int[] OutData;
			internal int OutOff;
			internal int OutSpan;

			public ByteFilter(ByteInterleavedRaster srcRas, ColorModel cm, AffineTransform xform, int maxw) : base((cm.Transparency == Transparency_Fields.OPAQUE ? Xrgbmodel : Argbmodel), xform, srcRas.Width, srcRas.Height, maxw)
			{
				this.InPalette = new int[256];
				((IndexColorModel) cm).GetRGBs(this.InPalette);
				this.SrcRas = srcRas;
				this.InData = srcRas.DataStorage;
				this.InSpan = srcRas.ScanlineStride;
				this.InOff = srcRas.getDataOffset(0);
			}

			public override WritableRaster MakeRaster(int w, int h)
			{
				// Note that we do not pass srcRas to makeRaster since it
				// is a Byte Raster and this colorModel needs an Int Raster
				WritableRaster ras = MakeRaster(ColorModel_Renamed, null, w, h);
				IntegerInterleavedRaster iiRas = (IntegerInterleavedRaster) ras;
				OutData = iiRas.DataStorage;
				OutSpan = iiRas.ScanlineStride;
				OutOff = iiRas.getDataOffset(0);
				return ras;
			}

			public override void SetRaster(int x, int y, int xerr, int yerr, int w, int h, int bWidth, int bHeight, int colincx, int colincxerr, int colincy, int colincyerr, int rowincx, int rowincxerr, int rowincy, int rowincyerr)
			{
				sbyte[] inData = this.InData;
				int[] outData = this.OutData;
				int @out = OutOff;
				int inSpan = this.InSpan;
				int inOff = this.InOff;
				int outSpan = this.OutSpan;
				int rowx = x;
				int rowy = y;
				int rowxerr = xerr;
				int rowyerr = yerr;
				int[] rgbs = new int[4];
				for (int j = 0; j < h; j++)
				{
					x = rowx;
					y = rowy;
					xerr = rowxerr;
					yerr = rowyerr;
					for (int i = 0; i < w; i++)
					{
						int nextx, nexty;
						if ((nextx = x + 1) >= bWidth)
						{
							nextx = 0;
						}
						if ((nexty = y + 1) >= bHeight)
						{
							nexty = 0;
						}
						rgbs[0] = InPalette[0xff & inData[inOff + x + inSpan * y]];
						rgbs[1] = InPalette[0xff & inData[inOff + nextx + inSpan * y]];
						rgbs[2] = InPalette[0xff & inData[inOff + x + inSpan * nexty]];
						rgbs[3] = InPalette[0xff & inData[inOff + nextx + inSpan * nexty]];
						outData[@out + i] = TexturePaintContext.Blend(rgbs, xerr, yerr);
						if ((xerr += colincxerr) < 0)
						{
							xerr &= Integer.MaxValue;
							x++;
						}
						if ((x += colincx) >= bWidth)
						{
							x -= bWidth;
						}
						if ((yerr += colincyerr) < 0)
						{
							yerr &= Integer.MaxValue;
							y++;
						}
						if ((y += colincy) >= bHeight)
						{
							y -= bHeight;
						}
					}
					if ((rowxerr += rowincxerr) < 0)
					{
						rowxerr &= Integer.MaxValue;
						rowx++;
					}
					if ((rowx += rowincx) >= bWidth)
					{
						rowx -= bWidth;
					}
					if ((rowyerr += rowincyerr) < 0)
					{
						rowyerr &= Integer.MaxValue;
						rowy++;
					}
					if ((rowy += rowincy) >= bHeight)
					{
						rowy -= bHeight;
					}
					@out += outSpan;
				}
			}
		}

		internal class Any : TexturePaintContext
		{
			internal WritableRaster SrcRas;
			internal bool Filter;

			public Any(WritableRaster srcRas, ColorModel cm, AffineTransform xform, int maxw, bool filter) : base(cm, xform, srcRas.Width, srcRas.Height, maxw)
			{
				this.SrcRas = srcRas;
				this.Filter = filter;
			}

			public override WritableRaster MakeRaster(int w, int h)
			{
				return MakeRaster(ColorModel_Renamed, SrcRas, w, h);
			}

			public override void SetRaster(int x, int y, int xerr, int yerr, int w, int h, int bWidth, int bHeight, int colincx, int colincxerr, int colincy, int colincyerr, int rowincx, int rowincxerr, int rowincy, int rowincyerr)
			{
				Object data = null;
				int rowx = x;
				int rowy = y;
				int rowxerr = xerr;
				int rowyerr = yerr;
				WritableRaster srcRas = this.SrcRas;
				WritableRaster outRas = this.OutRas;
				int[] rgbs = Filter ? new int[4] : null;
				for (int j = 0; j < h; j++)
				{
					x = rowx;
					y = rowy;
					xerr = rowxerr;
					yerr = rowyerr;
					for (int i = 0; i < w; i++)
					{
						data = srcRas.GetDataElements(x, y, data);
						if (Filter)
						{
							int nextx, nexty;
							if ((nextx = x + 1) >= bWidth)
							{
								nextx = 0;
							}
							if ((nexty = y + 1) >= bHeight)
							{
								nexty = 0;
							}
							rgbs[0] = ColorModel_Renamed.GetRGB(data);
							data = srcRas.GetDataElements(nextx, y, data);
							rgbs[1] = ColorModel_Renamed.GetRGB(data);
							data = srcRas.GetDataElements(x, nexty, data);
							rgbs[2] = ColorModel_Renamed.GetRGB(data);
							data = srcRas.GetDataElements(nextx, nexty, data);
							rgbs[3] = ColorModel_Renamed.GetRGB(data);
							int rgb = TexturePaintContext.Blend(rgbs, xerr, yerr);
							data = ColorModel_Renamed.GetDataElements(rgb, data);
						}
						outRas.SetDataElements(i, j, data);
						if ((xerr += colincxerr) < 0)
						{
							xerr &= Integer.MaxValue;
							x++;
						}
						if ((x += colincx) >= bWidth)
						{
							x -= bWidth;
						}
						if ((yerr += colincyerr) < 0)
						{
							yerr &= Integer.MaxValue;
							y++;
						}
						if ((y += colincy) >= bHeight)
						{
							y -= bHeight;
						}
					}
					if ((rowxerr += rowincxerr) < 0)
					{
						rowxerr &= Integer.MaxValue;
						rowx++;
					}
					if ((rowx += rowincx) >= bWidth)
					{
						rowx -= bWidth;
					}
					if ((rowyerr += rowincyerr) < 0)
					{
						rowyerr &= Integer.MaxValue;
						rowy++;
					}
					if ((rowy += rowincy) >= bHeight)
					{
						rowy -= bHeight;
					}
				}
			}
		}
	}

}
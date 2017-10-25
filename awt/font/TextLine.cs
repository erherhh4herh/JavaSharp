using System;
using System.Collections.Generic;

/*
 * Copyright (c) 1998, 2011, Oracle and/or its affiliates. All rights reserved.
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

/*
 * (C) Copyright IBM Corp. 1998-2003, All Rights Reserved
 *
 */

namespace java.awt.font
{

	using AttributeValues = sun.font.AttributeValues;
	using BidiUtils = sun.font.BidiUtils;
	using CoreMetrics = sun.font.CoreMetrics;
	using Decoration = sun.font.Decoration;
	using FontLineMetrics = sun.font.FontLineMetrics;
	using FontResolver = sun.font.FontResolver;
	using GraphicComponent = sun.font.GraphicComponent;
	using LayoutPathImpl = sun.font.LayoutPathImpl;
	using EmptyPath = sun.font.LayoutPathImpl.EmptyPath;
	using SegmentPathBuilder = sun.font.LayoutPathImpl.SegmentPathBuilder;
	using TextLabelFactory = sun.font.TextLabelFactory;
	using TextLineComponent = sun.font.TextLineComponent;
	using CodePointIterator = sun.text.CodePointIterator;

	internal sealed class TextLine
	{

		internal sealed class TextLineMetrics
		{
			public readonly float Ascent;
			public readonly float Descent;
			public readonly float Leading;
			public readonly float Advance;

			public TextLineMetrics(float ascent, float descent, float leading, float advance)
			{
				this.Ascent = ascent;
				this.Descent = descent;
				this.Leading = leading;
				this.Advance = advance;
			}
		}

		private TextLineComponent[] FComponents;
		private float[] FBaselineOffsets;
		private int[] FComponentVisualOrder; // if null, ltr
		private float[] Locs; // x,y pairs for components in visual order
		private char[] FChars;
		private int FCharsStart;
		private int FCharsLimit;
		private int[] FCharVisualOrder; // if null, ltr
		private int[] FCharLogicalOrder; // if null, ltr
		private sbyte[] FCharLevels; // if null, 0
		private bool FIsDirectionLTR;
		private LayoutPathImpl Lp;
		private bool IsSimple;
		private Rectangle PixelBounds;
		private FontRenderContext Frc;

		private TextLineMetrics FMetrics = null; // built on demand in getMetrics

		public TextLine(FontRenderContext frc, TextLineComponent[] components, float[] baselineOffsets, char[] chars, int charsStart, int charsLimit, int[] charLogicalOrder, sbyte[] charLevels, bool isDirectionLTR)
		{

			int[] componentVisualOrder = ComputeComponentOrder(components, charLogicalOrder);

			this.Frc = frc;
			FComponents = components;
			FBaselineOffsets = baselineOffsets;
			FComponentVisualOrder = componentVisualOrder;
			FChars = chars;
			FCharsStart = charsStart;
			FCharsLimit = charsLimit;
			FCharLogicalOrder = charLogicalOrder;
			FCharLevels = charLevels;
			FIsDirectionLTR = isDirectionLTR;
			CheckCtorArgs();

			Init();
		}

		private void CheckCtorArgs()
		{

			int checkCharCount = 0;
			for (int i = 0; i < FComponents.Length; i++)
			{
				checkCharCount += FComponents[i].NumCharacters;
			}

			if (checkCharCount != this.CharacterCount())
			{
				throw new IllegalArgumentException("Invalid TextLine!  " + "char count is different from " + "sum of char counts of components.");
			}
		}

		private void Init()
		{

			// first, we need to check for graphic components on the TOP or BOTTOM baselines.  So
			// we perform the work that used to be in getMetrics here.

			float ascent = 0;
			float descent = 0;
			float leading = 0;
			float advance = 0;

			// ascent + descent must not be less than this value
			float maxGraphicHeight = 0;
			float maxGraphicHeightWithLeading = 0;

			// walk through EGA's
			TextLineComponent tlc;
			bool fitTopAndBottomGraphics = false;

			IsSimple = true;

			for (int i = 0; i < FComponents.Length; i++)
			{
				tlc = FComponents[i];

				IsSimple &= tlc.Simple;

				CoreMetrics cm = tlc.CoreMetrics;

				sbyte baseline = (sbyte)cm.baselineIndex;

				if (baseline >= 0)
				{
					float baselineOffset = FBaselineOffsets[baseline];

					ascent = System.Math.Max(ascent, -baselineOffset + cm.ascent);

					float gd = baselineOffset + cm.descent;
					descent = System.Math.Max(descent, gd);

					leading = System.Math.Max(leading, gd + cm.leading);
				}
				else
				{
					fitTopAndBottomGraphics = true;
					float graphicHeight = cm.ascent + cm.descent;
					float graphicHeightWithLeading = graphicHeight + cm.leading;
					maxGraphicHeight = System.Math.Max(maxGraphicHeight, graphicHeight);
					maxGraphicHeightWithLeading = System.Math.Max(maxGraphicHeightWithLeading, graphicHeightWithLeading);
				}
			}

			if (fitTopAndBottomGraphics)
			{
				if (maxGraphicHeight > ascent + descent)
				{
					descent = maxGraphicHeight - ascent;
				}
				if (maxGraphicHeightWithLeading > ascent + leading)
				{
					leading = maxGraphicHeightWithLeading - ascent;
				}
			}

			leading -= descent;

			// we now know enough to compute the locs, but we need the final loc
			// for the advance before we can create the metrics object

			if (fitTopAndBottomGraphics)
			{
				// we have top or bottom baselines, so expand the baselines array
				// full offsets are needed by CoreMetrics.effectiveBaselineOffset
				FBaselineOffsets = new float[] {FBaselineOffsets[0], FBaselineOffsets[1], FBaselineOffsets[2], descent, -ascent};
			}

			float x = 0;
			float y = 0;
			CoreMetrics pcm = null;

			bool needPath = false;
			Locs = new float[FComponents.Length * 2 + 2];

			for (int i = 0, n = 0; i < FComponents.Length; ++i, n += 2)
			{
				tlc = FComponents[GetComponentLogicalIndex(i)];
				CoreMetrics cm = tlc.CoreMetrics;

				if ((pcm != null) && (pcm.italicAngle != 0 || cm.italicAngle != 0) && (pcm.italicAngle != cm.italicAngle || pcm.baselineIndex != cm.baselineIndex || pcm.ssOffset != cm.ssOffset)) // adjust because of italics
				{

					// 1) compute the area of overlap - min effective ascent and min effective descent
					// 2) compute the x positions along italic angle of ascent and descent for left and right
					// 3) compute maximum left - right, adjust right position by this value
					// this is a crude form of kerning between textcomponents

					// note glyphvectors preposition glyphs based on offset,
					// so tl doesn't need to adjust glyphvector position
					// 1)
					float pb = pcm.effectiveBaselineOffset(FBaselineOffsets);
					float pa = pb - pcm.ascent;
					float pd = pb + pcm.descent;
					// pb += pcm.ssOffset;

					float cb = cm.effectiveBaselineOffset(FBaselineOffsets);
					float ca = cb - cm.ascent;
					float cd = cb + cm.descent;
					// cb += cm.ssOffset;

					float a = System.Math.Max(pa, ca);
					float d = System.Math.Min(pd, cd);

					// 2)
					float pax = pcm.italicAngle * (pb - a);
					float pdx = pcm.italicAngle * (pb - d);

					float cax = cm.italicAngle * (cb - a);
					float cdx = cm.italicAngle * (cb - d);

					// 3)
					float dax = pax - cax;
					float ddx = pdx - cdx;
					float dx = System.Math.Max(dax, ddx);

					x += dx;
					y = cb;
				}
				else
				{
					// no italic adjustment for x, but still need to compute y
					y = cm.effectiveBaselineOffset(FBaselineOffsets); // + cm.ssOffset;
				}

				Locs[n] = x;
				Locs[n + 1] = y;

				x += tlc.Advance;
				pcm = cm;

				needPath |= tlc.BaselineTransform != null;
			}

			// do we want italic padding at the right of the line?
			if (pcm.italicAngle != 0)
			{
				float pb = pcm.effectiveBaselineOffset(FBaselineOffsets);
				float pa = pb - pcm.ascent;
				float pd = pb + pcm.descent;
				pb += pcm.ssOffset;

				float d;
				if (pcm.italicAngle > 0)
				{
					d = pb + pcm.ascent;
				}
				else
				{
					d = pb - pcm.descent;
				}
				d *= pcm.italicAngle;

				x += d;
			}
			Locs[Locs.Length - 2] = x;
			// locs[locs.length - 1] = 0; // final offset is always back on baseline

			// ok, build fMetrics since we have the final advance
			advance = x;
			FMetrics = new TextLineMetrics(ascent, descent, leading, advance);

			// build path if we need it
			if (needPath)
			{
				IsSimple = false;

				Point2D.Double pt = new Point2D.Double();
				double tx = 0, ty = 0;
				LayoutPathImpl.SegmentPathBuilder builder = new LayoutPathImpl.SegmentPathBuilder();
				builder.moveTo(Locs[0], 0);
				for (int i = 0, n = 0; i < FComponents.Length; ++i, n += 2)
				{
					tlc = FComponents[GetComponentLogicalIndex(i)];
					AffineTransform at = tlc.BaselineTransform;
					if (at != null && ((at.Type & AffineTransform.TYPE_TRANSLATION) != 0))
					{
						double dx = at.TranslateX;
						double dy = at.TranslateY;
						builder.moveTo(tx += dx, ty += dy);
					}
					pt.x = Locs[n + 2] - Locs[n];
					pt.y = 0;
					if (at != null)
					{
						at.DeltaTransform(pt, pt);
					}
					builder.lineTo(tx += pt.x, ty += pt.y);
				}
				Lp = builder.complete();

				if (Lp == null) // empty path
				{
					tlc = FComponents[GetComponentLogicalIndex(0)];
					AffineTransform at = tlc.BaselineTransform;
					if (at != null)
					{
						Lp = new LayoutPathImpl.EmptyPath(at);
					}
				}
			}
		}

		public Rectangle GetPixelBounds(FontRenderContext frc, float x, float y)
		{
			Rectangle result = null;

			// if we have a matching frc, set it to null so we don't have to test it
			// for each component
			if (frc != null && frc.Equals(this.Frc))
			{
				frc = null;
			}

			// only cache integral locations with the default frc, this is a bit strict
			int ix = (int)System.Math.Floor(x);
			int iy = (int)System.Math.Floor(y);
			float rx = x - ix;
			float ry = y - iy;
			bool canCache = frc == null && rx == 0 && ry == 0;

			if (canCache && PixelBounds != null)
			{
				result = new Rectangle(PixelBounds);
				result.x += ix;
				result.y += iy;
				return result;
			}

			// couldn't use cache, or didn't have it, so compute

			if (IsSimple) // all glyphvectors with no decorations, no layout path
			{
				for (int i = 0, n = 0; i < FComponents.Length; i++, n += 2)
				{
					TextLineComponent tlc = FComponents[GetComponentLogicalIndex(i)];
					Rectangle pb = tlc.getPixelBounds(frc, Locs[n] + rx, Locs[n + 1] + ry);
					if (!pb.Empty)
					{
						if (result == null)
						{
							result = pb;
						}
						else
						{
							result.Add(pb);
						}
					}
				}
				if (result == null)
				{
					result = new Rectangle(0, 0, 0, 0);
				}
			} // draw and test
			else
			{
				const int MARGIN = 3;
				Rectangle2D r2d = VisualBounds;
				if (Lp != null)
				{
					r2d = Lp.mapShape(r2d).Bounds;
				}
				Rectangle bounds = r2d.Bounds;
				BufferedImage im = new BufferedImage(bounds.Width_Renamed + MARGIN * 2, bounds.Height_Renamed + MARGIN * 2, BufferedImage.TYPE_INT_ARGB);

				Graphics2D g2d = im.CreateGraphics();
				g2d.Color = Color.WHITE;
				g2d.FillRect(0, 0, im.Width, im.Height);

				g2d.Color = Color.BLACK;
				Draw(g2d, rx + MARGIN - bounds.x, ry + MARGIN - bounds.y);

				result = ComputePixelBounds(im);
				result.x -= MARGIN - bounds.x;
				result.y -= MARGIN - bounds.y;
			}

			if (canCache)
			{
				PixelBounds = new Rectangle(result);
			}

			result.x += ix;
			result.y += iy;
			return result;
		}

		internal static Rectangle ComputePixelBounds(BufferedImage im)
		{
			int w = im.Width;
			int h = im.Height;

			int l = -1, t = -1, r = w, b = h;

			{
				// get top
				int[] buf = new int[w];
				while (++t < h)
				{
					im.GetRGB(0, t, buf.Length, 1, buf, 0, w); // w ignored
					for (int i = 0; i < buf.Length; i++)
					{
						if (buf[i] != -1)
						{
							goto loopBreak;
						}
					}
					loopContinue:;
				}
				loopBreak:;
			}

			{
			// get bottom
				int[] buf = new int[w];
				while (--b > t)
				{
					im.GetRGB(0, b, buf.Length, 1, buf, 0, w); // w ignored
					for (int i = 0; i < buf.Length; ++i)
					{
						if (buf[i] != -1)
						{
							goto loopBreak;
						}
					}
					loopContinue:;
				}
				loopBreak:
				++b;
			}

			{
			// get left
				while (++l < r)
				{
					for (int i = t; i < b; ++i)
					{
						int v = im.GetRGB(l, i);
						if (v != -1)
						{
							goto loopBreak;
						}
					}
					loopContinue:;
				}
				loopBreak:;
			}

			{
			// get right
				while (--r > l)
				{
					for (int i = t; i < b; ++i)
					{
						int v = im.GetRGB(r, i);
						if (v != -1)
						{
							goto loopBreak;
						}
					}
					loopContinue:;
				}
				loopBreak:
				++r;
			}

			return new Rectangle(l, t, r - l, b - t);
		}

		private abstract class Function
		{

			internal abstract float ComputeFunction(TextLine line, int componentIndex, int indexInArray);
		}

		private static Function fgPosAdvF = new FunctionAnonymousInnerClassHelper();

		private class FunctionAnonymousInnerClassHelper : Function
		{
			public FunctionAnonymousInnerClassHelper()
			{
			}

			internal override float ComputeFunction(TextLine line, int componentIndex, int indexInArray)
			{

				TextLineComponent tlc = line.FComponents[componentIndex];
					int vi = line.GetComponentVisualIndex(componentIndex);
				return line.Locs[vi * 2] + tlc.getCharX(indexInArray) + tlc.getCharAdvance(indexInArray);
			}
		}

		private static Function fgAdvanceF = new FunctionAnonymousInnerClassHelper2();

		private class FunctionAnonymousInnerClassHelper2 : Function
		{
			public FunctionAnonymousInnerClassHelper2()
			{
			}


			internal override float ComputeFunction(TextLine line, int componentIndex, int indexInArray)
			{

				TextLineComponent tlc = line.FComponents[componentIndex];
				return tlc.getCharAdvance(indexInArray);
			}
		}

		private static Function fgXPositionF = new FunctionAnonymousInnerClassHelper3();

		private class FunctionAnonymousInnerClassHelper3 : Function
		{
			public FunctionAnonymousInnerClassHelper3()
			{
			}


			internal override float ComputeFunction(TextLine line, int componentIndex, int indexInArray)
			{

					int vi = line.GetComponentVisualIndex(componentIndex);
				TextLineComponent tlc = line.FComponents[componentIndex];
				return line.Locs[vi * 2] + tlc.getCharX(indexInArray);
			}
		}

		private static Function fgYPositionF = new FunctionAnonymousInnerClassHelper4();

		private class FunctionAnonymousInnerClassHelper4 : Function
		{
			public FunctionAnonymousInnerClassHelper4()
			{
			}


			internal override float ComputeFunction(TextLine line, int componentIndex, int indexInArray)
			{

				TextLineComponent tlc = line.FComponents[componentIndex];
				float charPos = tlc.getCharY(indexInArray);

				// charPos is relative to the component - adjust for
				// baseline

				return charPos + line.GetComponentShift(componentIndex);
			}
		}

		public int CharacterCount()
		{

			return FCharsLimit - FCharsStart;
		}

		public bool DirectionLTR
		{
			get
			{
    
				return FIsDirectionLTR;
			}
		}

		public TextLineMetrics Metrics
		{
			get
			{
				return FMetrics;
			}
		}

		public int VisualToLogical(int visualIndex)
		{

			if (FCharLogicalOrder == null)
			{
				return visualIndex;
			}

			if (FCharVisualOrder == null)
			{
				FCharVisualOrder = BidiUtils.createInverseMap(FCharLogicalOrder);
			}

			return FCharVisualOrder[visualIndex];
		}

		public int LogicalToVisual(int logicalIndex)
		{

			return (FCharLogicalOrder == null)? logicalIndex : FCharLogicalOrder[logicalIndex];
		}

		public sbyte GetCharLevel(int logicalIndex)
		{

			return FCharLevels == null? 0 : FCharLevels[logicalIndex];
		}

		public bool IsCharLTR(int logicalIndex)
		{

			return (GetCharLevel(logicalIndex) & 0x1) == 0;
		}

		public int GetCharType(int logicalIndex)
		{

			return Character.GetType(FChars[logicalIndex + FCharsStart]);
		}

		public bool IsCharSpace(int logicalIndex)
		{

			return Character.IsSpaceChar(FChars[logicalIndex + FCharsStart]);
		}

		public bool IsCharWhitespace(int logicalIndex)
		{

			return char.IsWhiteSpace(FChars[logicalIndex + FCharsStart]);
		}

		public float GetCharAngle(int logicalIndex)
		{

			return GetCoreMetricsAt(logicalIndex).italicAngle;
		}

		public CoreMetrics GetCoreMetricsAt(int logicalIndex)
		{

			if (logicalIndex < 0)
			{
				throw new IllegalArgumentException("Negative logicalIndex.");
			}

			if (logicalIndex > FCharsLimit - FCharsStart)
			{
				throw new IllegalArgumentException("logicalIndex too large.");
			}

			int currentTlc = 0;
			int tlcStart = 0;
			int tlcLimit = 0;

			do
			{
				tlcLimit += FComponents[currentTlc].NumCharacters;
				if (tlcLimit > logicalIndex)
				{
					break;
				}
				++currentTlc;
				tlcStart = tlcLimit;
			} while (currentTlc < FComponents.Length);

			return FComponents[currentTlc].CoreMetrics;
		}

		public float GetCharAscent(int logicalIndex)
		{

			return GetCoreMetricsAt(logicalIndex).ascent;
		}

		public float GetCharDescent(int logicalIndex)
		{

			return GetCoreMetricsAt(logicalIndex).descent;
		}

		public float GetCharShift(int logicalIndex)
		{

			return GetCoreMetricsAt(logicalIndex).ssOffset;
		}

		private float ApplyFunctionAtIndex(int logicalIndex, Function f)
		{

			if (logicalIndex < 0)
			{
				throw new IllegalArgumentException("Negative logicalIndex.");
			}

			int tlcStart = 0;

			for (int i = 0; i < FComponents.Length; i++)
			{

				int tlcLimit = tlcStart + FComponents[i].NumCharacters;
				if (tlcLimit > logicalIndex)
				{
					return f.ComputeFunction(this, i, logicalIndex - tlcStart);
				}
				else
				{
					tlcStart = tlcLimit;
				}
			}

			throw new IllegalArgumentException("logicalIndex too large.");
		}

		public float GetCharAdvance(int logicalIndex)
		{

			return ApplyFunctionAtIndex(logicalIndex, fgAdvanceF);
		}

		public float GetCharXPosition(int logicalIndex)
		{

			return ApplyFunctionAtIndex(logicalIndex, fgXPositionF);
		}

		public float GetCharYPosition(int logicalIndex)
		{

			return ApplyFunctionAtIndex(logicalIndex, fgYPositionF);
		}

		public float GetCharLinePosition(int logicalIndex)
		{

			return GetCharXPosition(logicalIndex);
		}

		public float GetCharLinePosition(int logicalIndex, bool leading)
		{
			Function f = IsCharLTR(logicalIndex) == leading ? fgXPositionF : fgPosAdvF;
			return ApplyFunctionAtIndex(logicalIndex, f);
		}

		public bool CaretAtOffsetIsValid(int offset)
		{

			if (offset < 0)
			{
				throw new IllegalArgumentException("Negative offset.");
			}

			int tlcStart = 0;

			for (int i = 0; i < FComponents.Length; i++)
			{

				int tlcLimit = tlcStart + FComponents[i].NumCharacters;
				if (tlcLimit > offset)
				{
					return FComponents[i].caretAtOffsetIsValid(offset - tlcStart);
				}
				else
				{
					tlcStart = tlcLimit;
				}
			}

			throw new IllegalArgumentException("logicalIndex too large.");
		}

		/// <summary>
		/// map a component visual index to the logical index.
		/// </summary>
		private int GetComponentLogicalIndex(int vi)
		{
			if (FComponentVisualOrder == null)
			{
				return vi;
			}
			return FComponentVisualOrder[vi];
		}

		/// <summary>
		/// map a component logical index to the visual index.
		/// </summary>
		private int GetComponentVisualIndex(int li)
		{
			if (FComponentVisualOrder == null)
			{
					return li;
			}
			for (int i = 0; i < FComponentVisualOrder.Length; ++i)
			{
					if (FComponentVisualOrder[i] == li)
					{
						return i;
					}
			}
			throw new IndexOutOfBoundsException("bad component index: " + li);
		}

		public Rectangle2D GetCharBounds(int logicalIndex)
		{

			if (logicalIndex < 0)
			{
				throw new IllegalArgumentException("Negative logicalIndex.");
			}

			int tlcStart = 0;

			for (int i = 0; i < FComponents.Length; i++)
			{

				int tlcLimit = tlcStart + FComponents[i].NumCharacters;
				if (tlcLimit > logicalIndex)
				{

					TextLineComponent tlc = FComponents[i];
					int indexInTlc = logicalIndex - tlcStart;
					Rectangle2D chBounds = tlc.getCharVisualBounds(indexInTlc);

							int vi = GetComponentVisualIndex(i);
					chBounds.SetRect(chBounds.X + Locs[vi * 2], chBounds.Y + Locs[vi * 2 + 1], chBounds.Width, chBounds.Height);
					return chBounds;
				}
				else
				{
					tlcStart = tlcLimit;
				}
			}

			throw new IllegalArgumentException("logicalIndex too large.");
		}

		private float GetComponentShift(int index)
		{
			CoreMetrics cm = FComponents[index].CoreMetrics;
			return cm.effectiveBaselineOffset(FBaselineOffsets);
		}

		public void Draw(Graphics2D g2, float x, float y)
		{
			if (Lp == null)
			{
				for (int i = 0, n = 0; i < FComponents.Length; i++, n += 2)
				{
					TextLineComponent tlc = FComponents[GetComponentLogicalIndex(i)];
					tlc.draw(g2, Locs[n] + x, Locs[n + 1] + y);
				}
			}
			else
			{
				AffineTransform oldTx = g2.Transform;
				Point2D.Float pt = new Point2D.Float();
				for (int i = 0, n = 0; i < FComponents.Length; i++, n += 2)
				{
					TextLineComponent tlc = FComponents[GetComponentLogicalIndex(i)];
					Lp.pathToPoint(Locs[n], Locs[n + 1], false, pt);
					pt.x += x;
					pt.y += y;
					AffineTransform at = tlc.BaselineTransform;

					if (at != null)
					{
						g2.Translate(pt.x - at.TranslateX, pt.y - at.TranslateY);
						g2.Transform(at);
						tlc.draw(g2, 0, 0);
						g2.Transform = oldTx;
					}
					else
					{
						tlc.draw(g2, pt.x, pt.y);
					}
				}
			}
		}

		/// <summary>
		/// Return the union of the visual bounds of all the components.
		/// This incorporates the path.  It does not include logical
		/// bounds (used by carets).
		/// </summary>
		public Rectangle2D VisualBounds
		{
			get
			{
				Rectangle2D result = null;
    
				for (int i = 0, n = 0; i < FComponents.Length; i++, n += 2)
				{
					TextLineComponent tlc = FComponents[GetComponentLogicalIndex(i)];
					Rectangle2D r = tlc.VisualBounds;
    
					Point2D.Float pt = new Point2D.Float(Locs[n], Locs[n + 1]);
					if (Lp == null)
					{
						r.SetRect(r.MinX + pt.x, r.MinY + pt.y, r.Width, r.Height);
					}
					else
					{
						Lp.pathToPoint(pt, false, pt);
    
						AffineTransform at = tlc.BaselineTransform;
						if (at != null)
						{
							AffineTransform tx = AffineTransform.GetTranslateInstance(pt.x - at.TranslateX, pt.y - at.TranslateY);
							tx.Concatenate(at);
							r = tx.CreateTransformedShape(r).Bounds2D;
						}
						else
						{
							r.SetRect(r.MinX + pt.x, r.MinY + pt.y, r.Width, r.Height);
						}
					}
    
					if (result == null)
					{
						result = r;
					}
					else
					{
						result.Add(r);
					}
				}
    
				if (result == null)
				{
					result = new Rectangle2D.Float(Float.MaxValue, Float.MaxValue, Float.Epsilon, Float.Epsilon);
				}
    
				return result;
			}
		}

		public Rectangle2D ItalicBounds
		{
			get
			{
    
				float left = Float.MaxValue, right = -Float.MaxValue;
				float top = Float.MaxValue, bottom = -Float.MaxValue;
    
				for (int i = 0, n = 0; i < FComponents.Length; i++, n += 2)
				{
					TextLineComponent tlc = FComponents[GetComponentLogicalIndex(i)];
    
					Rectangle2D tlcBounds = tlc.ItalicBounds;
					float x = Locs[n];
					float y = Locs[n + 1];
    
					left = System.Math.Min(left, x + (float)tlcBounds.X);
					right = System.Math.Max(right, x + (float)tlcBounds.MaxX);
    
					top = System.Math.Min(top, y + (float)tlcBounds.Y);
					bottom = System.Math.Max(bottom, y + (float)tlcBounds.MaxY);
				}
    
				return new Rectangle2D.Float(left, top, right - left, bottom - top);
			}
		}

		public Shape GetOutline(AffineTransform tx)
		{

			GeneralPath dstShape = new GeneralPath(GeneralPath.WIND_NON_ZERO);

			for (int i = 0, n = 0; i < FComponents.Length; i++, n += 2)
			{
				TextLineComponent tlc = FComponents[GetComponentLogicalIndex(i)];

				dstShape.Append(tlc.getOutline(Locs[n], Locs[n + 1]), false);
			}

			if (tx != null)
			{
				dstShape.Transform(tx);
			}
			return dstShape;
		}

		public override int HashCode()
		{
			return (FComponents.Length << 16) ^ (FComponents[0].HashCode() << 3) ^ (FCharsLimit - FCharsStart);
		}

		public override String ToString()
		{
			StringBuilder buf = new StringBuilder();

			for (int i = 0; i < FComponents.Length; i++)
			{
				buf.Append(FComponents[i]);
			}

			return buf.ToString();
		}

		/// <summary>
		/// Create a TextLine from the text.  The Font must be able to
		/// display all of the text.
		/// attributes==null is equivalent to using an empty Map for
		/// attributes
		/// </summary>
		public static TextLine fastCreateTextLine<T1>(FontRenderContext frc, char[] chars, Font font, CoreMetrics lm, IDictionary<T1> attributes) where T1 : java.text.AttributedCharacterIterator_Attribute
		{

			bool isDirectionLTR = true;
			sbyte[] levels = null;
			int[] charsLtoV = null;
			Bidi bidi = null;
			int characterCount = chars.Length;

			bool requiresBidi = false;
			sbyte[] embs = null;

			AttributeValues values = null;
			if (attributes != null)
			{
				values = AttributeValues.fromMap(attributes);
				if (values.RunDirection >= 0)
				{
					isDirectionLTR = values.RunDirection == 0;
					requiresBidi = !isDirectionLTR;
				}
				if (values.BidiEmbedding != 0)
				{
					requiresBidi = true;
					sbyte level = (sbyte)values.BidiEmbedding;
					embs = new sbyte[characterCount];
					for (int i = 0; i < embs.Length; ++i)
					{
						embs[i] = level;
					}
				}
			}

			// dlf: get baseRot from font for now???

			if (!requiresBidi)
			{
				requiresBidi = Bidi.RequiresBidi(chars, 0, chars.Length);
			}

			if (requiresBidi)
			{
			  int bidiflags = values == null ? Bidi.DIRECTION_DEFAULT_LEFT_TO_RIGHT : values.RunDirection;

			  bidi = new Bidi(chars, 0, embs, 0, chars.Length, bidiflags);
			  if (!bidi.LeftToRight)
			  {
				  levels = BidiUtils.getLevels(bidi);
				  int[] charsVtoL = BidiUtils.createVisualToLogicalMap(levels);
				  charsLtoV = BidiUtils.createInverseMap(charsVtoL);
				  isDirectionLTR = bidi.BaseIsLeftToRight();
			  }
			}

			Decoration decorator = Decoration.getDecoration(values);

			int layoutFlags = 0; // no extra info yet, bidi determines run and line direction
			TextLabelFactory factory = new TextLabelFactory(frc, chars, bidi, layoutFlags);

			TextLineComponent[] components = new TextLineComponent[1];

			components = CreateComponentsOnRun(0, chars.Length, chars, charsLtoV, levels, factory, font, lm, frc, decorator, components, 0);

			int numComponents = components.Length;
			while (components[numComponents - 1] == null)
			{
				numComponents -= 1;
			}

			if (numComponents != components.Length)
			{
				TextLineComponent[] temp = new TextLineComponent[numComponents];
				System.Array.Copy(components, 0, temp, 0, numComponents);
				components = temp;
			}

			return new TextLine(frc, components, lm.baselineOffsets, chars, 0, chars.Length, charsLtoV, levels, isDirectionLTR);
		}

		private static TextLineComponent[] ExpandArray(TextLineComponent[] orig)
		{

			TextLineComponent[] newComponents = new TextLineComponent[orig.Length + 8];
			System.Array.Copy(orig, 0, newComponents, 0, orig.Length);

			return newComponents;
		}

		/// <summary>
		/// Returns an array in logical order of the TextLineComponents on
		/// the text in the given range, with the given attributes.
		/// </summary>
		public static TextLineComponent[] CreateComponentsOnRun(int runStart, int runLimit, char[] chars, int[] charsLtoV, sbyte[] levels, TextLabelFactory factory, Font font, CoreMetrics cm, FontRenderContext frc, Decoration decorator, TextLineComponent[] components, int numComponents)
		{

			int pos = runStart;
			do
			{
				int chunkLimit = FirstVisualChunk(charsLtoV, levels, pos, runLimit); // <= displayLimit

				do
				{
					int startPos = pos;
					int lmCount;

					if (cm == null)
					{
						LineMetrics lineMetrics = font.GetLineMetrics(chars, startPos, chunkLimit, frc);
						cm = CoreMetrics.get(lineMetrics);
						lmCount = lineMetrics.NumChars;
					}
					else
					{
						lmCount = (chunkLimit - startPos);
					}

					TextLineComponent nextComponent = factory.createExtended(font, cm, decorator, startPos, startPos + lmCount);

					++numComponents;
					if (numComponents >= components.Length)
					{
						components = ExpandArray(components);
					}

					components[numComponents - 1] = nextComponent;

					pos += lmCount;
				} while (pos < chunkLimit);

			} while (pos < runLimit);

			return components;
		}

		/// <summary>
		/// Returns an array (in logical order) of the TextLineComponents representing
		/// the text.  The components are both logically and visually contiguous.
		/// </summary>
		public static TextLineComponent[] GetComponents(StyledParagraph styledParagraph, char[] chars, int textStart, int textLimit, int[] charsLtoV, sbyte[] levels, TextLabelFactory factory)
		{

			FontRenderContext frc = factory.FontRenderContext;

			int numComponents = 0;
			TextLineComponent[] tempComponents = new TextLineComponent[1];

			int pos = textStart;
			do
			{
				int runLimit = System.Math.Min(styledParagraph.GetRunLimit(pos), textLimit);

				Decoration decorator = styledParagraph.GetDecorationAt(pos);

				Object graphicOrFont = styledParagraph.GetFontOrGraphicAt(pos);

				if (graphicOrFont is GraphicAttribute)
				{
					// AffineTransform baseRot = styledParagraph.getBaselineRotationAt(pos);
					// !!! For now, let's assign runs of text with both fonts and graphic attributes
					// a null rotation (e.g. the baseline rotation goes away when a graphic
					// is applied.
					AffineTransform baseRot = null;
					GraphicAttribute graphicAttribute = (GraphicAttribute) graphicOrFont;
					do
					{
						int chunkLimit = FirstVisualChunk(charsLtoV, levels, pos, runLimit);

						GraphicComponent nextGraphic = new GraphicComponent(graphicAttribute, decorator, charsLtoV, levels, pos, chunkLimit, baseRot);
						pos = chunkLimit;

						++numComponents;
						if (numComponents >= tempComponents.Length)
						{
							tempComponents = ExpandArray(tempComponents);
						}

						tempComponents[numComponents - 1] = nextGraphic;

					} while (pos < runLimit);
				}
				else
				{
					Font font = (Font) graphicOrFont;

					tempComponents = CreateComponentsOnRun(pos, runLimit, chars, charsLtoV, levels, factory, font, null, frc, decorator, tempComponents, numComponents);
					pos = runLimit;
					numComponents = tempComponents.Length;
					while (tempComponents[numComponents - 1] == null)
					{
						numComponents -= 1;
					}
				}

			} while (pos < textLimit);

			TextLineComponent[] components;
			if (tempComponents.Length == numComponents)
			{
				components = tempComponents;
			}
			else
			{
				components = new TextLineComponent[numComponents];
				System.Array.Copy(tempComponents, 0, components, 0, numComponents);
			}

			return components;
		}

		/// <summary>
		/// Create a TextLine from the Font and character data over the
		/// range.  The range is relative to both the StyledParagraph and the
		/// character array.
		/// </summary>
		public static TextLine CreateLineFromText(char[] chars, StyledParagraph styledParagraph, TextLabelFactory factory, bool isDirectionLTR, float[] baselineOffsets)
		{

			factory.setLineContext(0, chars.Length);

			Bidi lineBidi = factory.LineBidi;
			int[] charsLtoV = null;
			sbyte[] levels = null;

			if (lineBidi != null)
			{
				levels = BidiUtils.getLevels(lineBidi);
				int[] charsVtoL = BidiUtils.createVisualToLogicalMap(levels);
				charsLtoV = BidiUtils.createInverseMap(charsVtoL);
			}

			TextLineComponent[] components = GetComponents(styledParagraph, chars, 0, chars.Length, charsLtoV, levels, factory);

			return new TextLine(factory.FontRenderContext, components, baselineOffsets, chars, 0, chars.Length, charsLtoV, levels, isDirectionLTR);
		}

		/// <summary>
		/// Compute the components order from the given components array and
		/// logical-to-visual character mapping.  May return null if canonical.
		/// </summary>
		private static int[] ComputeComponentOrder(TextLineComponent[] components, int[] charsLtoV)
		{

			/*
			 * Create a visual ordering for the glyph sets.  The important thing
			 * here is that the values have the proper rank with respect to
			 * each other, not the exact values.  For example, the first glyph
			 * set that appears visually should have the lowest value.  The last
			 * should have the highest value.  The values are then normalized
			 * to map 1-1 with positions in glyphs.
			 *
			 */
			int[] componentOrder = null;
			if (charsLtoV != null && components.Length > 1)
			{
				componentOrder = new int[components.Length];
				int gStart = 0;
				for (int i = 0; i < components.Length; i++)
				{
					componentOrder[i] = charsLtoV[gStart];
					gStart += components[i].NumCharacters;
				}

				componentOrder = BidiUtils.createContiguousOrder(componentOrder);
				componentOrder = BidiUtils.createInverseMap(componentOrder);
			}
			return componentOrder;
		}


		/// <summary>
		/// Create a TextLine from the text.  chars is just the text in the iterator.
		/// </summary>
		public static TextLine StandardCreateTextLine(FontRenderContext frc, AttributedCharacterIterator text, char[] chars, float[] baselineOffsets)
		{

			StyledParagraph styledParagraph = new StyledParagraph(text, chars);
			Bidi bidi = new Bidi(text);
			if (bidi.LeftToRight)
			{
				bidi = null;
			}
			int layoutFlags = 0; // no extra info yet, bidi determines run and line direction
			TextLabelFactory factory = new TextLabelFactory(frc, chars, bidi, layoutFlags);

			bool isDirectionLTR = true;
			if (bidi != null)
			{
				isDirectionLTR = bidi.BaseIsLeftToRight();
			}
			return CreateLineFromText(chars, styledParagraph, factory, isDirectionLTR, baselineOffsets);
		}



		/*
		 * A utility to get a range of text that is both logically and visually
		 * contiguous.
		 * If the entire range is ok, return limit, otherwise return the first
		 * directional change after start.  We could do better than this, but
		 * it doesn't seem worth it at the moment.
		private static int firstVisualChunk(int order[], byte direction[],
		                                    int start, int limit)
		{
		    if (order != null) {
		        int min = order[start];
		        int max = order[start];
		        int count = limit - start;
		        for (int i = start + 1; i < limit; i++) {
		            min = Math.min(min, order[i]);
		            max = Math.max(max, order[i]);
		            if (max - min >= count) {
		                if (direction != null) {
		                    byte baseLevel = direction[start];
		                    for (int j = start + 1; j < i; j++) {
		                        if (direction[j] != baseLevel) {
		                            return j;
		                        }
		                    }
		                }
		                return i;
		            }
		        }
		    }
		    return limit;
		}
		 */

		/// <summary>
		/// When this returns, the ACI's current position will be at the start of the
		/// first run which does NOT contain a GraphicAttribute.  If no such run exists
		/// the ACI's position will be at the end, and this method will return false.
		/// </summary>
		internal static bool AdvanceToFirstFont(AttributedCharacterIterator aci)
		{

			for (char ch = aci.First(); ch != java.text.CharacterIterator_Fields.DONE; ch = aci.setIndex(aci.RunLimit))
			{

				if (aci.GetAttribute(TextAttribute.CHAR_REPLACEMENT) == null)
				{
					return true;
				}
			}

			return false;
		}

		internal static float[] GetNormalizedOffsets(float[] baselineOffsets, sbyte baseline)
		{

			if (baselineOffsets[baseline] != 0)
			{
				float @base = baselineOffsets[baseline];
				float[] temp = new float[baselineOffsets.Length];
				for (int i = 0; i < temp.Length; i++)
				{
					temp[i] = baselineOffsets[i] - @base;
				}
				baselineOffsets = temp;
			}
			return baselineOffsets;
		}

		internal static Font GetFontAtCurrentPos(AttributedCharacterIterator aci)
		{

			Object value = aci.GetAttribute(TextAttribute.FONT);
			if (value != null)
			{
				return (Font) value;
			}
			if (aci.GetAttribute(TextAttribute.FAMILY) != null)
			{
				return Font.GetFont(aci.Attributes);
			}

			int ch = CodePointIterator.create(aci).next();
			if (ch != CodePointIterator.DONE)
			{
				FontResolver resolver = FontResolver.Instance;
				return resolver.getFont(resolver.getFontIndex(ch), aci.Attributes);
			}
			return null;
		}

	  /*
	   * The new version requires that chunks be at the same level.
	   */
		private static int FirstVisualChunk(int[] order, sbyte[] direction, int start, int limit)
		{
			if (order != null && direction != null)
			{
			  sbyte dir = direction[start];
			  while (++start < limit && direction[start] == dir)
			  {
			  }
			  return start;
			}
			return limit;
		}

	  /*
	   * create a new line with characters between charStart and charLimit
	   * justified using the provided width and ratio.
	   */
		public TextLine GetJustifiedLine(float justificationWidth, float justifyRatio, int justStart, int justLimit)
		{

			TextLineComponent[] newComponents = new TextLineComponent[FComponents.Length];
			System.Array.Copy(FComponents, 0, newComponents, 0, FComponents.Length);

			float leftHang = 0;
			float adv = 0;
			float justifyDelta = 0;
			bool rejustify = false;
			do
			{
				adv = GetAdvanceBetween(newComponents, 0, CharacterCount());

				// all characters outside the justification range must be in the base direction
				// of the layout, otherwise justification makes no sense.

				float justifyAdvance = GetAdvanceBetween(newComponents, justStart, justLimit);

				// get the actual justification delta
				justifyDelta = (justificationWidth - justifyAdvance) * justifyRatio;

				// generate an array of GlyphJustificationInfo records to pass to
				// the justifier.  Array is visually ordered.

				// get positions that each component will be using
				int[] infoPositions = new int[newComponents.Length];
				int infoCount = 0;
				for (int visIndex = 0; visIndex < newComponents.Length; visIndex++)
				{
						int logIndex = GetComponentLogicalIndex(visIndex);
					infoPositions[logIndex] = infoCount;
					infoCount += newComponents[logIndex].NumJustificationInfos;
				}
				GlyphJustificationInfo[] infos = new GlyphJustificationInfo[infoCount];

				// get justification infos
				int compStart = 0;
				for (int i = 0; i < newComponents.Length; i++)
				{
					TextLineComponent comp = newComponents[i];
					int compLength = comp.NumCharacters;
					int compLimit = compStart + compLength;
					if (compLimit > justStart)
					{
						int rangeMin = System.Math.Max(0, justStart - compStart);
						int rangeMax = System.Math.Min(compLength, justLimit - compStart);
						comp.getJustificationInfos(infos, infoPositions[i], rangeMin, rangeMax);

						if (compLimit >= justLimit)
						{
							break;
						}
					}
				}

				// records are visually ordered, and contiguous, so start and end are
				// simply the places where we didn't fetch records
				int infoStart = 0;
				int infoLimit = infoCount;
				while (infoStart < infoLimit && infos[infoStart] == null)
				{
					++infoStart;
				}

				while (infoLimit > infoStart && infos[infoLimit - 1] == null)
				{
					--infoLimit;
				}

				// invoke justifier on the records
				TextJustifier justifier = new TextJustifier(infos, infoStart, infoLimit);

				float[] deltas = justifier.Justify(justifyDelta);

				bool canRejustify = rejustify == false;
				bool wantRejustify = false;
				bool[] flags = new bool[1];

				// apply justification deltas
				compStart = 0;
				for (int i = 0; i < newComponents.Length; i++)
				{
					TextLineComponent comp = newComponents[i];
					int compLength = comp.NumCharacters;
					int compLimit = compStart + compLength;
					if (compLimit > justStart)
					{
						int rangeMin = System.Math.Max(0, justStart - compStart);
						int rangeMax = System.Math.Min(compLength, justLimit - compStart);
						newComponents[i] = comp.applyJustificationDeltas(deltas, infoPositions[i] * 2, flags);

						wantRejustify |= flags[0];

						if (compLimit >= justLimit)
						{
							break;
						}
					}
				}

				rejustify = wantRejustify && !rejustify; // only make two passes
			} while (rejustify);

			return new TextLine(Frc, newComponents, FBaselineOffsets, FChars, FCharsStart, FCharsLimit, FCharLogicalOrder, FCharLevels, FIsDirectionLTR);
		}

		// return the sum of the advances of text between the logical start and limit
		public static float GetAdvanceBetween(TextLineComponent[] components, int start, int limit)
		{
			float advance = 0;

			int tlcStart = 0;
			for (int i = 0; i < components.Length; i++)
			{
				TextLineComponent comp = components[i];

				int tlcLength = comp.NumCharacters;
				int tlcLimit = tlcStart + tlcLength;
				if (tlcLimit > start)
				{
					int measureStart = System.Math.Max(0, start - tlcStart);
					int measureLimit = System.Math.Min(tlcLength, limit - tlcStart);
					advance += comp.getAdvanceBetween(measureStart, measureLimit);
					if (tlcLimit >= limit)
					{
						break;
					}
				}

				tlcStart = tlcLimit;
			}

			return advance;
		}

		internal LayoutPathImpl LayoutPath
		{
			get
			{
				return Lp;
			}
		}
	}

}
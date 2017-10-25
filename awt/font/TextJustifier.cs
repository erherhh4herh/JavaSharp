using System;

/*
 * Copyright (c) 1997, 1999, Oracle and/or its affiliates. All rights reserved.
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
 * (C) Copyright Taligent, Inc. 1996 - 1997, All Rights Reserved
 * (C) Copyright IBM Corp. 1996 - 1998, All Rights Reserved
 *
 * The original version of this source code and documentation is
 * copyrighted and owned by Taligent, Inc., a wholly-owned subsidiary
 * of IBM. These materials are provided under terms of a License
 * Agreement between Taligent and Sun. This technology is protected
 * by multiple US and International patents.
 *
 * This notice and attribution to Taligent may not be removed.
 * Taligent is a registered trademark of Taligent, Inc.
 *
 */

namespace java.awt.font
{

	/*
	 * one info for each side of each glyph
	 * separate infos for grow and shrink case
	 * !!! this doesn't really need to be a separate class.  If we keep it
	 * separate, probably the newJustify code from TextLayout belongs here as well.
	 */

	internal class TextJustifier
	{
		private GlyphJustificationInfo[] Info;
		private int Start;
		private int Limit;

		internal static bool DEBUG = false;

		/// <summary>
		/// Initialize the justifier with an array of infos corresponding to each
		/// glyph. Start and limit indicate the range of the array to examine.
		/// </summary>
		internal TextJustifier(GlyphJustificationInfo[] info, int start, int limit)
		{
			this.Info = info;
			this.Start = start;
			this.Limit = limit;

			if (DEBUG)
			{
				System.Console.WriteLine("start: " + start + ", limit: " + limit);
				for (int i = start; i < limit; i++)
				{
					GlyphJustificationInfo gji = info[i];
					System.Console.WriteLine("w: " + gji.Weight + ", gp: " + gji.GrowPriority + ", gll: " + gji.GrowLeftLimit + ", grl: " + gji.GrowRightLimit);
				}
			}
		}

		public const int MAX_PRIORITY = 3;

		/// <summary>
		/// Return an array of deltas twice as long as the original info array,
		/// indicating the amount by which each side of each glyph should grow
		/// or shrink.
		/// 
		/// Delta should be positive to expand the line, and negative to compress it.
		/// </summary>
		public virtual float[] Justify(float delta)
		{
			float[] deltas = new float[Info.Length * 2];

			bool grow = delta > 0;

			if (DEBUG)
			{
				System.Console.WriteLine("delta: " + delta);
			}

			// make separate passes through glyphs in order of decreasing priority
			// until justifyDelta is zero or we run out of priorities.
			int fallbackPriority = -1;
			for (int p = 0; delta != 0; p++)
			{
				/*
				 * special case 'fallback' iteration, set flag and recheck
				 * highest priority
				 */
				bool lastPass = p > MAX_PRIORITY;
				if (lastPass)
				{
					p = fallbackPriority;
				}

				// pass through glyphs, first collecting weights and limits
				float weight = 0;
				float gslimit = 0;
				float absorbweight = 0;
				for (int i = Start; i < Limit; i++)
				{
					GlyphJustificationInfo gi = Info[i];
					if ((grow ? gi.GrowPriority : gi.ShrinkPriority) == p)
					{
						if (fallbackPriority == -1)
						{
							fallbackPriority = p;
						}

						if (i != Start) // ignore left of first character
						{
							weight += gi.Weight;
							if (grow)
							{
								gslimit += gi.GrowLeftLimit;
								if (gi.GrowAbsorb)
								{
									absorbweight += gi.Weight;
								}
							}
							else
							{
								gslimit += gi.ShrinkLeftLimit;
								if (gi.ShrinkAbsorb)
								{
									absorbweight += gi.Weight;
								}
							}
						}

						if (i + 1 != Limit) // ignore right of last character
						{
							weight += gi.Weight;
							if (grow)
							{
								gslimit += gi.GrowRightLimit;
								if (gi.GrowAbsorb)
								{
									absorbweight += gi.Weight;
								}
							}
							else
							{
								gslimit += gi.ShrinkRightLimit;
								if (gi.ShrinkAbsorb)
								{
									absorbweight += gi.Weight;
								}
							}
						}
					}
				}

				// did we hit the limit?
				if (!grow)
				{
					gslimit = -gslimit; // negative for negative deltas
				}
				bool hitLimit = (weight == 0) || (!lastPass && ((delta < 0) == (delta < gslimit)));
				bool absorbing = hitLimit && absorbweight > 0;

				// predivide delta by weight
				float weightedDelta = delta / weight; // not used if weight == 0

				float weightedAbsorb = 0;
				if (hitLimit && absorbweight > 0)
				{
					weightedAbsorb = (delta - gslimit) / absorbweight;
				}

				if (DEBUG)
				{
					System.Console.WriteLine("pass: " + p + ", d: " + delta + ", l: " + gslimit + ", w: " + weight + ", aw: " + absorbweight + ", wd: " + weightedDelta + ", wa: " + weightedAbsorb + ", hit: " + (hitLimit ? "y" : "n"));
				}

				// now allocate this based on ratio of weight to total weight
				int n = Start * 2;
				for (int i = Start; i < Limit; i++)
				{
					GlyphJustificationInfo gi = Info[i];
					if ((grow ? gi.GrowPriority : gi.ShrinkPriority) == p)
					{
						if (i != Start) // ignore left
						{
							float d;
							if (hitLimit)
							{
								// factor in sign
								d = grow ? gi.GrowLeftLimit : -gi.ShrinkLeftLimit;
								if (absorbing)
								{
									// sign factored in already
								   d += gi.Weight * weightedAbsorb;
								}
							}
							else
							{
								// sign factored in already
								d = gi.Weight * weightedDelta;
							}

							deltas[n] += d;
						}
						n++;

						if (i + 1 != Limit) // ignore right
						{
							float d;
							if (hitLimit)
							{
								d = grow ? gi.GrowRightLimit : -gi.ShrinkRightLimit;
								if (absorbing)
								{
									d += gi.Weight * weightedAbsorb;
								}
							}
							else
							{
								d = gi.Weight * weightedDelta;
							}

							deltas[n] += d;
						}
						n++;
					}
					else
					{
						n += 2;
					}
				}

				if (!lastPass && hitLimit && !absorbing)
				{
					delta -= gslimit;
				}
				else
				{
					delta = 0; // stop iteration
				}
			}

			if (DEBUG)
			{
				float total = 0;
				for (int i = 0; i < deltas.Length; i++)
				{
					total += deltas[i];
					System.Console.Write(deltas[i] + ", ");
					if (i % 20 == 9)
					{
						System.Console.WriteLine();
					}
				}
				System.Console.WriteLine("\ntotal: " + total);
				System.Console.WriteLine();
			}

			return deltas;
		}
	}

}
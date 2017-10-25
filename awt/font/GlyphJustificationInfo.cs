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
 */

namespace java.awt.font
{

	/// <summary>
	/// The <code>GlyphJustificationInfo</code> class represents information
	/// about the justification properties of a glyph.  A glyph is the visual
	/// representation of one or more characters.  Many different glyphs can
	/// be used to represent a single character or combination of characters.
	/// The four justification properties represented by
	/// <code>GlyphJustificationInfo</code> are weight, priority, absorb and
	/// limit.
	/// <para>
	/// Weight is the overall 'weight' of the glyph in the line.  Generally it is
	/// proportional to the size of the font.  Glyphs with larger weight are
	/// allocated a correspondingly larger amount of the change in space.
	/// </para>
	/// <para>
	/// Priority determines the justification phase in which this glyph is used.
	/// All glyphs of the same priority are examined before glyphs of the next
	/// priority.  If all the change in space can be allocated to these glyphs
	/// without exceeding their limits, then glyphs of the next priority are not
	/// examined. There are four priorities, kashida, whitespace, interchar,
	/// and none.  KASHIDA is the first priority examined. NONE is the last
	/// priority examined.
	/// </para>
	/// <para>
	/// Absorb determines whether a glyph absorbs all change in space.  Within a
	/// given priority, some glyphs may absorb all the change in space.  If any of
	/// these glyphs are present, no glyphs of later priority are examined.
	/// </para>
	/// <para>
	/// Limit determines the maximum or minimum amount by which the glyph can
	/// change. Left and right sides of the glyph can have different limits.
	/// </para>
	/// <para>
	/// Each <code>GlyphJustificationInfo</code> represents two sets of
	/// metrics, which are <i>growing</i> and <i>shrinking</i>.  Growing
	/// metrics are used when the glyphs on a line are to be
	/// spread apart to fit a larger width.  Shrinking metrics are used when
	/// the glyphs are to be moved together to fit a smaller width.
	/// </para>
	/// </summary>

	public sealed class GlyphJustificationInfo
	{

		/// <summary>
		/// Constructs information about the justification properties of a
		/// glyph. </summary>
		/// <param name="weight"> the weight of this glyph when allocating space.  Must be non-negative. </param>
		/// <param name="growAbsorb"> if <code>true</code> this glyph absorbs
		/// all extra space at this priority and lower priority levels when it
		/// grows </param>
		/// <param name="growPriority"> the priority level of this glyph when it
		/// grows </param>
		/// <param name="growLeftLimit"> the maximum amount by which the left side of this
		/// glyph can grow.  Must be non-negative. </param>
		/// <param name="growRightLimit"> the maximum amount by which the right side of this
		/// glyph can grow.  Must be non-negative. </param>
		/// <param name="shrinkAbsorb"> if <code>true</code>, this glyph absorbs all
		/// remaining shrinkage at this and lower priority levels when it
		/// shrinks </param>
		/// <param name="shrinkPriority"> the priority level of this glyph when
		/// it shrinks </param>
		/// <param name="shrinkLeftLimit"> the maximum amount by which the left side of this
		/// glyph can shrink.  Must be non-negative. </param>
		/// <param name="shrinkRightLimit"> the maximum amount by which the right side
		/// of this glyph can shrink.  Must be non-negative. </param>
		 public GlyphJustificationInfo(float weight, bool growAbsorb, int growPriority, float growLeftLimit, float growRightLimit, bool shrinkAbsorb, int shrinkPriority, float shrinkLeftLimit, float shrinkRightLimit)
		 {
			if (weight < 0)
			{
				throw new IllegalArgumentException("weight is negative");
			}

			if (!PriorityIsValid(growPriority))
			{
				throw new IllegalArgumentException("Invalid grow priority");
			}
			if (growLeftLimit < 0)
			{
				throw new IllegalArgumentException("growLeftLimit is negative");
			}
			if (growRightLimit < 0)
			{
				throw new IllegalArgumentException("growRightLimit is negative");
			}

			if (!PriorityIsValid(shrinkPriority))
			{
				throw new IllegalArgumentException("Invalid shrink priority");
			}
			if (shrinkLeftLimit < 0)
			{
				throw new IllegalArgumentException("shrinkLeftLimit is negative");
			}
			if (shrinkRightLimit < 0)
			{
				throw new IllegalArgumentException("shrinkRightLimit is negative");
			}

			this.Weight = weight;
			this.GrowAbsorb = growAbsorb;
			this.GrowPriority = growPriority;
			this.GrowLeftLimit = growLeftLimit;
			this.GrowRightLimit = growRightLimit;
			this.ShrinkAbsorb = shrinkAbsorb;
			this.ShrinkPriority = shrinkPriority;
			this.ShrinkLeftLimit = shrinkLeftLimit;
			this.ShrinkRightLimit = shrinkRightLimit;
		 }

		private static bool PriorityIsValid(int priority)
		{

			return priority >= PRIORITY_KASHIDA && priority <= PRIORITY_NONE;
		}

		/// <summary>
		/// The highest justification priority. </summary>
		public const int PRIORITY_KASHIDA = 0;

		/// <summary>
		/// The second highest justification priority. </summary>
		public const int PRIORITY_WHITESPACE = 1;

		/// <summary>
		/// The second lowest justification priority. </summary>
		public const int PRIORITY_INTERCHAR = 2;

		/// <summary>
		/// The lowest justification priority. </summary>
		public const int PRIORITY_NONE = 3;

		/// <summary>
		/// The weight of this glyph.
		/// </summary>
		public readonly float Weight;

		/// <summary>
		/// The priority level of this glyph as it is growing.
		/// </summary>
		public readonly int GrowPriority;

		/// <summary>
		/// If <code>true</code>, this glyph absorbs all extra
		/// space at this and lower priority levels when it grows.
		/// </summary>
		public readonly bool GrowAbsorb;

		/// <summary>
		/// The maximum amount by which the left side of this glyph can grow.
		/// </summary>
		public readonly float GrowLeftLimit;

		/// <summary>
		/// The maximum amount by which the right side of this glyph can grow.
		/// </summary>
		public readonly float GrowRightLimit;

		/// <summary>
		/// The priority level of this glyph as it is shrinking.
		/// </summary>
		public readonly int ShrinkPriority;

		/// <summary>
		/// If <code>true</code>,this glyph absorbs all remaining shrinkage at
		/// this and lower priority levels as it shrinks.
		/// </summary>
		public readonly bool ShrinkAbsorb;

		/// <summary>
		/// The maximum amount by which the left side of this glyph can shrink
		/// (a positive number).
		/// </summary>
		public readonly float ShrinkLeftLimit;

		/// <summary>
		/// The maximum amount by which the right side of this glyph can shrink
		/// (a positive number).
		/// </summary>
		public readonly float ShrinkRightLimit;
	}

}
/*
 * Copyright (c) 1997, 1998, Oracle and/or its affiliates. All rights reserved.
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

	/// <summary>
	/// The <code>TextHitInfo</code> class represents a character position in a
	/// text model, and a <b>bias</b>, or "side," of the character.  Biases are
	/// either <EM>leading</EM> (the left edge, for a left-to-right character)
	/// or <EM>trailing</EM> (the right edge, for a left-to-right character).
	/// Instances of <code>TextHitInfo</code> are used to specify caret and
	/// insertion positions within text.
	/// <para>
	/// For example, consider the text "abc".  TextHitInfo.trailing(1)
	/// corresponds to the right side of the 'b' in the text.
	/// </para>
	/// <para>
	/// <code>TextHitInfo</code> is used primarily by <seealso cref="TextLayout"/> and
	/// clients of <code>TextLayout</code>.  Clients of <code>TextLayout</code>
	/// query <code>TextHitInfo</code> instances for an insertion offset, where
	/// new text is inserted into the text model.  The insertion offset is equal
	/// to the character position in the <code>TextHitInfo</code> if the bias
	/// is leading, and one character after if the bias is trailing.  The
	/// insertion offset for TextHitInfo.trailing(1) is 2.
	/// </para>
	/// <para>
	/// Sometimes it is convenient to construct a <code>TextHitInfo</code> with
	/// the same insertion offset as an existing one, but on the opposite
	/// character.  The <code>getOtherHit</code> method constructs a new
	/// <code>TextHitInfo</code> with the same insertion offset as an existing
	/// one, with a hit on the character on the other side of the insertion offset.
	/// Calling <code>getOtherHit</code> on trailing(1) would return leading(2).
	/// In general, <code>getOtherHit</code> for trailing(n) returns
	/// leading(n+1) and <code>getOtherHit</code> for leading(n)
	/// returns trailing(n-1).
	/// </para>
	/// <para>
	/// </para>
	/// <strong>Example</strong>:<para>
	/// Converting a graphical point to an insertion point within a text
	/// model
	/// <blockquote><pre>
	/// TextLayout layout = ...;
	/// Point2D.Float hitPoint = ...;
	/// TextHitInfo hitInfo = layout.hitTestChar(hitPoint.x, hitPoint.y);
	/// int insPoint = hitInfo.getInsertionIndex();
	/// // insPoint is relative to layout;  may need to adjust for use
	/// // in a text model
	/// </pre></blockquote>
	/// 
	/// </para>
	/// </summary>
	/// <seealso cref= TextLayout </seealso>

	public sealed class TextHitInfo
	{
		private int CharIndex_Renamed;
		private bool IsLeadingEdge;

		/// <summary>
		/// Constructs a new <code>TextHitInfo</code>. </summary>
		/// <param name="charIndex"> the index of the character hit </param>
		/// <param name="isLeadingEdge"> <code>true</code> if the leading edge of the
		/// character was hit </param>
		private TextHitInfo(int charIndex, bool isLeadingEdge)
		{
			this.CharIndex_Renamed = charIndex;
			this.IsLeadingEdge = isLeadingEdge;
		}

		/// <summary>
		/// Returns the index of the character hit. </summary>
		/// <returns> the index of the character hit. </returns>
		public int CharIndex
		{
			get
			{
				return CharIndex_Renamed;
			}
		}

		/// <summary>
		/// Returns <code>true</code> if the leading edge of the character was
		/// hit. </summary>
		/// <returns> <code>true</code> if the leading edge of the character was
		/// hit; <code>false</code> otherwise. </returns>
		public bool LeadingEdge
		{
			get
			{
				return IsLeadingEdge;
			}
		}

		/// <summary>
		/// Returns the insertion index.  This is the character index if
		/// the leading edge of the character was hit, and one greater
		/// than the character index if the trailing edge was hit. </summary>
		/// <returns> the insertion index. </returns>
		public int InsertionIndex
		{
			get
			{
				return IsLeadingEdge ? CharIndex_Renamed : CharIndex_Renamed + 1;
			}
		}

		/// <summary>
		/// Returns the hash code. </summary>
		/// <returns> the hash code of this <code>TextHitInfo</code>, which is
		/// also the <code>charIndex</code> of this <code>TextHitInfo</code>. </returns>
		public override int HashCode()
		{
			return CharIndex_Renamed;
		}

		/// <summary>
		/// Returns <code>true</code> if the specified <code>Object</code> is a
		/// <code>TextHitInfo</code> and equals this <code>TextHitInfo</code>. </summary>
		/// <param name="obj"> the <code>Object</code> to test for equality </param>
		/// <returns> <code>true</code> if the specified <code>Object</code>
		/// equals this <code>TextHitInfo</code>; <code>false</code> otherwise. </returns>
		public override bool Equals(Object obj)
		{
			return (obj is TextHitInfo) && Equals((TextHitInfo)obj);
		}

		/// <summary>
		/// Returns <code>true</code> if the specified <code>TextHitInfo</code>
		/// has the same <code>charIndex</code> and <code>isLeadingEdge</code>
		/// as this <code>TextHitInfo</code>.  This is not the same as having
		/// the same insertion offset. </summary>
		/// <param name="hitInfo"> a specified <code>TextHitInfo</code> </param>
		/// <returns> <code>true</code> if the specified <code>TextHitInfo</code>
		/// has the same <code>charIndex</code> and <code>isLeadingEdge</code>
		/// as this <code>TextHitInfo</code>. </returns>
		public bool Equals(TextHitInfo hitInfo)
		{
			return hitInfo != null && CharIndex_Renamed == hitInfo.CharIndex_Renamed && IsLeadingEdge == hitInfo.IsLeadingEdge;
		}

		/// <summary>
		/// Returns a <code>String</code> representing the hit for debugging
		/// use only. </summary>
		/// <returns> a <code>String</code> representing this
		/// <code>TextHitInfo</code>. </returns>
		public override String ToString()
		{
			return "TextHitInfo[" + CharIndex_Renamed + (IsLeadingEdge ? "L" : "T") + "]";
		}

		/// <summary>
		/// Creates a <code>TextHitInfo</code> on the leading edge of the
		/// character at the specified <code>charIndex</code>. </summary>
		/// <param name="charIndex"> the index of the character hit </param>
		/// <returns> a <code>TextHitInfo</code> on the leading edge of the
		/// character at the specified <code>charIndex</code>. </returns>
		public static TextHitInfo Leading(int charIndex)
		{
			return new TextHitInfo(charIndex, true);
		}

		/// <summary>
		/// Creates a hit on the trailing edge of the character at
		/// the specified <code>charIndex</code>. </summary>
		/// <param name="charIndex"> the index of the character hit </param>
		/// <returns> a <code>TextHitInfo</code> on the trailing edge of the
		/// character at the specified <code>charIndex</code>. </returns>
		public static TextHitInfo Trailing(int charIndex)
		{
			return new TextHitInfo(charIndex, false);
		}

		/// <summary>
		/// Creates a <code>TextHitInfo</code> at the specified offset,
		/// associated with the character before the offset. </summary>
		/// <param name="offset"> an offset associated with the character before
		/// the offset </param>
		/// <returns> a <code>TextHitInfo</code> at the specified offset. </returns>
		public static TextHitInfo BeforeOffset(int offset)
		{
			return new TextHitInfo(offset - 1, false);
		}

		/// <summary>
		/// Creates a <code>TextHitInfo</code> at the specified offset,
		/// associated with the character after the offset. </summary>
		/// <param name="offset"> an offset associated with the character after
		/// the offset </param>
		/// <returns> a <code>TextHitInfo</code> at the specified offset. </returns>
		public static TextHitInfo AfterOffset(int offset)
		{
			return new TextHitInfo(offset, true);
		}

		/// <summary>
		/// Creates a <code>TextHitInfo</code> on the other side of the
		/// insertion point.  This <code>TextHitInfo</code> remains unchanged. </summary>
		/// <returns> a <code>TextHitInfo</code> on the other side of the
		/// insertion point. </returns>
		public TextHitInfo OtherHit
		{
			get
			{
				if (IsLeadingEdge)
				{
					return Trailing(CharIndex_Renamed - 1);
				}
				else
				{
					return Leading(CharIndex_Renamed + 1);
				}
			}
		}

		/// <summary>
		/// Creates a <code>TextHitInfo</code> whose character index is offset
		/// by <code>delta</code> from the <code>charIndex</code> of this
		/// <code>TextHitInfo</code>. This <code>TextHitInfo</code> remains
		/// unchanged. </summary>
		/// <param name="delta"> the value to offset this <code>charIndex</code> </param>
		/// <returns> a <code>TextHitInfo</code> whose <code>charIndex</code> is
		/// offset by <code>delta</code> from the <code>charIndex</code> of
		/// this <code>TextHitInfo</code>. </returns>
		public TextHitInfo GetOffsetHit(int delta)
		{
			return new TextHitInfo(CharIndex_Renamed + delta, IsLeadingEdge);
		}
	}

}
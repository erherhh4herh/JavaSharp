using System;
using System.Runtime.InteropServices;

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

namespace java.awt
{


	/// <summary>
	/// The <code>FontMetrics</code> class defines a font metrics object, which
	/// encapsulates information about the rendering of a particular font on a
	/// particular screen.
	/// <para>
	/// <b>Note to subclassers</b>: Since many of these methods form closed,
	/// mutually recursive loops, you must take care that you implement
	/// at least one of the methods in each such loop to prevent
	/// infinite recursion when your subclass is used.
	/// In particular, the following is the minimal suggested set of methods
	/// to override in order to ensure correctness and prevent infinite
	/// recursion (though other subsets are equally feasible):
	/// <ul>
	/// <li><seealso cref="#getAscent()"/>
	/// <li><seealso cref="#getLeading()"/>
	/// <li><seealso cref="#getMaxAdvance()"/>
	/// <li><seealso cref="#charWidth(char)"/>
	/// <li><seealso cref="#charsWidth(char[], int, int)"/>
	/// </ul>
	/// </para>
	/// <para>
	/// <img src="doc-files/FontMetrics-1.gif" alt="The letter 'p' showing its 'reference point'"
	/// style="border:15px; float:right; margin: 7px 10px;">
	/// Note that the implementations of these methods are
	/// inefficient, so they are usually overridden with more efficient
	/// toolkit-specific implementations.
	/// </para>
	/// <para>
	/// When an application asks to place a character at the position
	/// (<i>x</i>,&nbsp;<i>y</i>), the character is placed so that its
	/// reference point (shown as the dot in the accompanying image) is
	/// put at that position. The reference point specifies a horizontal
	/// line called the <i>baseline</i> of the character. In normal
	/// printing, the baselines of characters should align.
	/// </para>
	/// <para>
	/// In addition, every character in a font has an <i>ascent</i>, a
	/// <i>descent</i>, and an <i>advance width</i>. The ascent is the
	/// amount by which the character ascends above the baseline. The
	/// descent is the amount by which the character descends below the
	/// baseline. The advance width indicates the position at which AWT
	/// should place the next character.
	/// </para>
	/// <para>
	/// An array of characters or a string can also have an ascent, a
	/// descent, and an advance width. The ascent of the array is the
	/// maximum ascent of any character in the array. The descent is the
	/// maximum descent of any character in the array. The advance width
	/// is the sum of the advance widths of each of the characters in the
	/// character array.  The advance of a <code>String</code> is the
	/// distance along the baseline of the <code>String</code>.  This
	/// distance is the width that should be used for centering or
	/// right-aligning the <code>String</code>.
	/// </para>
	/// <para>Note that the advance of a <code>String</code> is not necessarily
	/// the sum of the advances of its characters measured in isolation
	/// because the width of a character can vary depending on its context.
	/// For example, in Arabic text, the shape of a character can change
	/// in order to connect to other characters.  Also, in some scripts,
	/// certain character sequences can be represented by a single shape,
	/// called a <em>ligature</em>.  Measuring characters individually does
	/// not account for these transformations.
	/// </para>
	/// <para>Font metrics are baseline-relative, meaning that they are
	/// generally independent of the rotation applied to the font (modulo
	/// possible grid hinting effects).  See <seealso cref="java.awt.Font Font"/>.
	/// 
	/// @author      Jim Graham
	/// </para>
	/// </summary>
	/// <seealso cref=         java.awt.Font
	/// @since       JDK1.0 </seealso>
	[Serializable]
	public abstract class FontMetrics
	{

		static FontMetrics()
		{
			/* ensure that the necessary native libraries are loaded */
			Toolkit.LoadLibraries();
			if (!GraphicsEnvironment.Headless)
			{
				initIDs();
			}
		}

		private static readonly FontRenderContext DEFAULT_FRC = new FontRenderContext(null, false, false);

		/// <summary>
		/// The actual <seealso cref="Font"/> from which the font metrics are
		/// created.
		/// This cannot be null.
		/// 
		/// @serial </summary>
		/// <seealso cref= #getFont() </seealso>
		protected internal Font Font_Renamed;

		/*
		 * JDK 1.1 serialVersionUID
		 */
		private const long SerialVersionUID = 1681126225205050147L;

		/// <summary>
		/// Creates a new <code>FontMetrics</code> object for finding out
		/// height and width information about the specified <code>Font</code>
		/// and specific character glyphs in that <code>Font</code>. </summary>
		/// <param name="font"> the <code>Font</code> </param>
		/// <seealso cref=       java.awt.Font </seealso>
		protected internal FontMetrics(Font font)
		{
			this.Font_Renamed = font;
		}

		/// <summary>
		/// Gets the <code>Font</code> described by this
		/// <code>FontMetrics</code> object. </summary>
		/// <returns>    the <code>Font</code> described by this
		/// <code>FontMetrics</code> object. </returns>
		public virtual Font Font
		{
			get
			{
				return Font_Renamed;
			}
		}

		/// <summary>
		/// Gets the <code>FontRenderContext</code> used by this
		/// <code>FontMetrics</code> object to measure text.
		/// <para>
		/// Note that methods in this class which take a <code>Graphics</code>
		/// parameter measure text using the <code>FontRenderContext</code>
		/// of that <code>Graphics</code> object, and not this
		/// <code>FontRenderContext</code>
		/// </para>
		/// </summary>
		/// <returns>    the <code>FontRenderContext</code> used by this
		/// <code>FontMetrics</code> object.
		/// @since 1.6 </returns>
		public virtual FontRenderContext FontRenderContext
		{
			get
			{
				return DEFAULT_FRC;
			}
		}

		/// <summary>
		/// Determines the <em>standard leading</em> of the
		/// <code>Font</code> described by this <code>FontMetrics</code>
		/// object.  The standard leading, or
		/// interline spacing, is the logical amount of space to be reserved
		/// between the descent of one line of text and the ascent of the next
		/// line. The height metric is calculated to include this extra space. </summary>
		/// <returns>    the standard leading of the <code>Font</code>. </returns>
		/// <seealso cref=   #getHeight() </seealso>
		/// <seealso cref=   #getAscent() </seealso>
		/// <seealso cref=   #getDescent() </seealso>
		public virtual int Leading
		{
			get
			{
				return 0;
			}
		}

		/// <summary>
		/// Determines the <em>font ascent</em> of the <code>Font</code>
		/// described by this <code>FontMetrics</code> object. The font ascent
		/// is the distance from the font's baseline to the top of most
		/// alphanumeric characters. Some characters in the <code>Font</code>
		/// might extend above the font ascent line. </summary>
		/// <returns>     the font ascent of the <code>Font</code>. </returns>
		/// <seealso cref=        #getMaxAscent() </seealso>
		public virtual int Ascent
		{
			get
			{
				return Font_Renamed.Size;
			}
		}

		/// <summary>
		/// Determines the <em>font descent</em> of the <code>Font</code>
		/// described by this
		/// <code>FontMetrics</code> object. The font descent is the distance
		/// from the font's baseline to the bottom of most alphanumeric
		/// characters with descenders. Some characters in the
		/// <code>Font</code> might extend
		/// below the font descent line. </summary>
		/// <returns>     the font descent of the <code>Font</code>. </returns>
		/// <seealso cref=        #getMaxDescent() </seealso>
		public virtual int Descent
		{
			get
			{
				return 0;
			}
		}

		/// <summary>
		/// Gets the standard height of a line of text in this font.  This
		/// is the distance between the baseline of adjacent lines of text.
		/// It is the sum of the leading + ascent + descent. Due to rounding
		/// this may not be the same as getAscent() + getDescent() + getLeading().
		/// There is no guarantee that lines of text spaced at this distance are
		/// disjoint; such lines may overlap if some characters overshoot
		/// either the standard ascent or the standard descent metric. </summary>
		/// <returns>    the standard height of the font. </returns>
		/// <seealso cref=       #getLeading() </seealso>
		/// <seealso cref=       #getAscent() </seealso>
		/// <seealso cref=       #getDescent() </seealso>
		public virtual int Height
		{
			get
			{
				return Leading + Ascent + Descent;
			}
		}

		/// <summary>
		/// Determines the maximum ascent of the <code>Font</code>
		/// described by this <code>FontMetrics</code> object.  No character
		/// extends further above the font's baseline than this height. </summary>
		/// <returns>    the maximum ascent of any character in the
		/// <code>Font</code>. </returns>
		/// <seealso cref=       #getAscent() </seealso>
		public virtual int MaxAscent
		{
			get
			{
				return Ascent;
			}
		}

		/// <summary>
		/// Determines the maximum descent of the <code>Font</code>
		/// described by this <code>FontMetrics</code> object.  No character
		/// extends further below the font's baseline than this height. </summary>
		/// <returns>    the maximum descent of any character in the
		/// <code>Font</code>. </returns>
		/// <seealso cref=       #getDescent() </seealso>
		public virtual int MaxDescent
		{
			get
			{
				return Descent;
			}
		}

		/// <summary>
		/// For backward compatibility only. </summary>
		/// <returns>    the maximum descent of any character in the
		/// <code>Font</code>. </returns>
		/// <seealso cref= #getMaxDescent() </seealso>
		/// @deprecated As of JDK version 1.1.1,
		/// replaced by <code>getMaxDescent()</code>. 
		[Obsolete("As of JDK version 1.1.1,")]
		public virtual int MaxDecent
		{
			get
			{
				return MaxDescent;
			}
		}

		/// <summary>
		/// Gets the maximum advance width of any character in this
		/// <code>Font</code>.  The advance is the
		/// distance from the leftmost point to the rightmost point on the
		/// string's baseline.  The advance of a <code>String</code> is
		/// not necessarily the sum of the advances of its characters. </summary>
		/// <returns>    the maximum advance width of any character
		///            in the <code>Font</code>, or <code>-1</code> if the
		///            maximum advance width is not known. </returns>
		public virtual int MaxAdvance
		{
			get
			{
				return -1;
			}
		}

		/// <summary>
		/// Returns the advance width of the specified character in this
		/// <code>Font</code>.  The advance is the
		/// distance from the leftmost point to the rightmost point on the
		/// character's baseline.  Note that the advance of a
		/// <code>String</code> is not necessarily the sum of the advances
		/// of its characters.
		/// 
		/// <para>This method doesn't validate the specified character to be a
		/// valid Unicode code point. The caller must validate the
		/// character value using {@link
		/// java.lang.Character#isValidCodePoint(int)
		/// Character.isValidCodePoint} if necessary.
		/// 
		/// </para>
		/// </summary>
		/// <param name="codePoint"> the character (Unicode code point) to be measured </param>
		/// <returns>    the advance width of the specified character
		///            in the <code>Font</code> described by this
		///            <code>FontMetrics</code> object. </returns>
		/// <seealso cref=   #charsWidth(char[], int, int) </seealso>
		/// <seealso cref=   #stringWidth(String) </seealso>
		public virtual int CharWidth(int codePoint)
		{
			if (!Character.IsValidCodePoint(codePoint))
			{
				codePoint = 0xffff; // substitute missing glyph width
			}

			if (codePoint < 256)
			{
				return Widths[codePoint];
			}
			else
			{
				char[] buffer = new char[2];
				int len = Character.ToChars(codePoint, buffer, 0);
				return CharsWidth(buffer, 0, len);
			}
		}

		/// <summary>
		/// Returns the advance width of the specified character in this
		/// <code>Font</code>.  The advance is the
		/// distance from the leftmost point to the rightmost point on the
		/// character's baseline.  Note that the advance of a
		/// <code>String</code> is not necessarily the sum of the advances
		/// of its characters.
		/// 
		/// <para><b>Note:</b> This method cannot handle <a
		/// href="../lang/Character.html#supplementary"> supplementary
		/// characters</a>. To support all Unicode characters, including
		/// supplementary characters, use the <seealso cref="#charWidth(int)"/> method.
		/// 
		/// </para>
		/// </summary>
		/// <param name="ch"> the character to be measured </param>
		/// <returns>     the advance width of the specified character
		///                  in the <code>Font</code> described by this
		///                  <code>FontMetrics</code> object. </returns>
		/// <seealso cref=        #charsWidth(char[], int, int) </seealso>
		/// <seealso cref=        #stringWidth(String) </seealso>
		public virtual int CharWidth(char ch)
		{
			if (ch < 256)
			{
				return Widths[ch];
			}
			char[] data = new char[] {ch};
			return CharsWidth(data, 0, 1);
		}

		/// <summary>
		/// Returns the total advance width for showing the specified
		/// <code>String</code> in this <code>Font</code>.  The advance
		/// is the distance from the leftmost point to the rightmost point
		/// on the string's baseline.
		/// <para>
		/// Note that the advance of a <code>String</code> is
		/// not necessarily the sum of the advances of its characters.
		/// </para>
		/// </summary>
		/// <param name="str"> the <code>String</code> to be measured </param>
		/// <returns>    the advance width of the specified <code>String</code>
		///                  in the <code>Font</code> described by this
		///                  <code>FontMetrics</code>. </returns>
		/// <exception cref="NullPointerException"> if str is null. </exception>
		/// <seealso cref=       #bytesWidth(byte[], int, int) </seealso>
		/// <seealso cref=       #charsWidth(char[], int, int) </seealso>
		/// <seealso cref=       #getStringBounds(String, Graphics) </seealso>
		public virtual int StringWidth(String str)
		{
			int len = str.Length();
			char[] data = new char[len];
			str.GetChars(0, len, data, 0);
			return CharsWidth(data, 0, len);
		}

		/// <summary>
		/// Returns the total advance width for showing the specified array
		/// of characters in this <code>Font</code>.  The advance is the
		/// distance from the leftmost point to the rightmost point on the
		/// string's baseline.  The advance of a <code>String</code>
		/// is not necessarily the sum of the advances of its characters.
		/// This is equivalent to measuring a <code>String</code> of the
		/// characters in the specified range. </summary>
		/// <param name="data"> the array of characters to be measured </param>
		/// <param name="off"> the start offset of the characters in the array </param>
		/// <param name="len"> the number of characters to be measured from the array </param>
		/// <returns>    the advance width of the subarray of the specified
		///               <code>char</code> array in the font described by
		///               this <code>FontMetrics</code> object. </returns>
		/// <exception cref="NullPointerException"> if <code>data</code> is null. </exception>
		/// <exception cref="IndexOutOfBoundsException"> if the <code>off</code>
		///            and <code>len</code> arguments index characters outside
		///            the bounds of the <code>data</code> array. </exception>
		/// <seealso cref=       #charWidth(int) </seealso>
		/// <seealso cref=       #charWidth(char) </seealso>
		/// <seealso cref=       #bytesWidth(byte[], int, int) </seealso>
		/// <seealso cref=       #stringWidth(String) </seealso>
		public virtual int CharsWidth(char[] data, int off, int len)
		{
			return StringWidth(new String(data, off, len));
		}

		/// <summary>
		/// Returns the total advance width for showing the specified array
		/// of bytes in this <code>Font</code>.  The advance is the
		/// distance from the leftmost point to the rightmost point on the
		/// string's baseline.  The advance of a <code>String</code>
		/// is not necessarily the sum of the advances of its characters.
		/// This is equivalent to measuring a <code>String</code> of the
		/// characters in the specified range. </summary>
		/// <param name="data"> the array of bytes to be measured </param>
		/// <param name="off"> the start offset of the bytes in the array </param>
		/// <param name="len"> the number of bytes to be measured from the array </param>
		/// <returns>    the advance width of the subarray of the specified
		///               <code>byte</code> array in the <code>Font</code>
		///                  described by
		///               this <code>FontMetrics</code> object. </returns>
		/// <exception cref="NullPointerException"> if <code>data</code> is null. </exception>
		/// <exception cref="IndexOutOfBoundsException"> if the <code>off</code>
		///            and <code>len</code> arguments index bytes outside
		///            the bounds of the <code>data</code> array. </exception>
		/// <seealso cref=       #charsWidth(char[], int, int) </seealso>
		/// <seealso cref=       #stringWidth(String) </seealso>
		public virtual int BytesWidth(sbyte[] data, int off, int len)
		{
			return StringWidth(StringHelperClass.NewString(data, 0, off, len));
		}

		/// <summary>
		/// Gets the advance widths of the first 256 characters in the
		/// <code>Font</code>.  The advance is the
		/// distance from the leftmost point to the rightmost point on the
		/// character's baseline.  Note that the advance of a
		/// <code>String</code> is not necessarily the sum of the advances
		/// of its characters. </summary>
		/// <returns>    an array storing the advance widths of the
		///                 characters in the <code>Font</code>
		///                 described by this <code>FontMetrics</code> object. </returns>
		public virtual int[] Widths
		{
			get
			{
				int[] widths = new int[256];
				for (char ch = (char)0 ; ch < 256 ; ch++)
				{
					widths[ch] = CharWidth(ch);
				}
				return widths;
			}
		}

		/// <summary>
		/// Checks to see if the <code>Font</code> has uniform line metrics.  A
		/// composite font may consist of several different fonts to cover
		/// various character sets.  In such cases, the
		/// <code>FontLineMetrics</code> objects are not uniform.
		/// Different fonts may have a different ascent, descent, metrics and
		/// so on.  This information is sometimes necessary for line
		/// measuring and line breaking. </summary>
		/// <returns> <code>true</code> if the font has uniform line metrics;
		/// <code>false</code> otherwise. </returns>
		/// <seealso cref= java.awt.Font#hasUniformLineMetrics() </seealso>
		public virtual bool HasUniformLineMetrics()
		{
			return Font_Renamed.HasUniformLineMetrics();
		}

		/// <summary>
		/// Returns the <seealso cref="LineMetrics"/> object for the specified
		/// <code>String</code> in the specified <seealso cref="Graphics"/> context. </summary>
		/// <param name="str"> the specified <code>String</code> </param>
		/// <param name="context"> the specified <code>Graphics</code> context </param>
		/// <returns> a <code>LineMetrics</code> object created with the
		/// specified <code>String</code> and <code>Graphics</code> context. </returns>
		/// <seealso cref= java.awt.Font#getLineMetrics(String, FontRenderContext) </seealso>
		public virtual LineMetrics GetLineMetrics(String str, Graphics context)
		{
			return Font_Renamed.GetLineMetrics(str, MyFRC(context));
		}

		/// <summary>
		/// Returns the <seealso cref="LineMetrics"/> object for the specified
		/// <code>String</code> in the specified <seealso cref="Graphics"/> context. </summary>
		/// <param name="str"> the specified <code>String</code> </param>
		/// <param name="beginIndex"> the initial offset of <code>str</code> </param>
		/// <param name="limit"> the end offset of <code>str</code> </param>
		/// <param name="context"> the specified <code>Graphics</code> context </param>
		/// <returns> a <code>LineMetrics</code> object created with the
		/// specified <code>String</code> and <code>Graphics</code> context. </returns>
		/// <seealso cref= java.awt.Font#getLineMetrics(String, int, int, FontRenderContext) </seealso>
		public virtual LineMetrics GetLineMetrics(String str, int beginIndex, int limit, Graphics context)
		{
			return Font_Renamed.GetLineMetrics(str, beginIndex, limit, MyFRC(context));
		}

		/// <summary>
		/// Returns the <seealso cref="LineMetrics"/> object for the specified
		/// character array in the specified <seealso cref="Graphics"/> context. </summary>
		/// <param name="chars"> the specified character array </param>
		/// <param name="beginIndex"> the initial offset of <code>chars</code> </param>
		/// <param name="limit"> the end offset of <code>chars</code> </param>
		/// <param name="context"> the specified <code>Graphics</code> context </param>
		/// <returns> a <code>LineMetrics</code> object created with the
		/// specified character array and <code>Graphics</code> context. </returns>
		/// <seealso cref= java.awt.Font#getLineMetrics(char[], int, int, FontRenderContext) </seealso>
		public virtual LineMetrics GetLineMetrics(char[] chars, int beginIndex, int limit, Graphics context)
		{
			return Font_Renamed.GetLineMetrics(chars, beginIndex, limit, MyFRC(context));
		}

		/// <summary>
		/// Returns the <seealso cref="LineMetrics"/> object for the specified
		/// <seealso cref="CharacterIterator"/> in the specified <seealso cref="Graphics"/>
		/// context. </summary>
		/// <param name="ci"> the specified <code>CharacterIterator</code> </param>
		/// <param name="beginIndex"> the initial offset in <code>ci</code> </param>
		/// <param name="limit"> the end index of <code>ci</code> </param>
		/// <param name="context"> the specified <code>Graphics</code> context </param>
		/// <returns> a <code>LineMetrics</code> object created with the
		/// specified arguments. </returns>
		/// <seealso cref= java.awt.Font#getLineMetrics(CharacterIterator, int, int, FontRenderContext) </seealso>
		public virtual LineMetrics GetLineMetrics(CharacterIterator ci, int beginIndex, int limit, Graphics context)
		{
			return Font_Renamed.GetLineMetrics(ci, beginIndex, limit, MyFRC(context));
		}

		/// <summary>
		/// Returns the bounds of the specified <code>String</code> in the
		/// specified <code>Graphics</code> context.  The bounds is used
		/// to layout the <code>String</code>.
		/// <para>Note: The returned bounds is in baseline-relative coordinates
		/// (see <seealso cref="java.awt.FontMetrics class notes"/>).
		/// </para>
		/// </summary>
		/// <param name="str"> the specified <code>String</code> </param>
		/// <param name="context"> the specified <code>Graphics</code> context </param>
		/// <returns> a <seealso cref="Rectangle2D"/> that is the bounding box of the
		/// specified <code>String</code> in the specified
		/// <code>Graphics</code> context. </returns>
		/// <seealso cref= java.awt.Font#getStringBounds(String, FontRenderContext) </seealso>
		public virtual Rectangle2D GetStringBounds(String str, Graphics context)
		{
			return Font_Renamed.GetStringBounds(str, MyFRC(context));
		}

		/// <summary>
		/// Returns the bounds of the specified <code>String</code> in the
		/// specified <code>Graphics</code> context.  The bounds is used
		/// to layout the <code>String</code>.
		/// <para>Note: The returned bounds is in baseline-relative coordinates
		/// (see <seealso cref="java.awt.FontMetrics class notes"/>).
		/// </para>
		/// </summary>
		/// <param name="str"> the specified <code>String</code> </param>
		/// <param name="beginIndex"> the offset of the beginning of <code>str</code> </param>
		/// <param name="limit"> the end offset of <code>str</code> </param>
		/// <param name="context"> the specified <code>Graphics</code> context </param>
		/// <returns> a <code>Rectangle2D</code> that is the bounding box of the
		/// specified <code>String</code> in the specified
		/// <code>Graphics</code> context. </returns>
		/// <seealso cref= java.awt.Font#getStringBounds(String, int, int, FontRenderContext) </seealso>
		public virtual Rectangle2D GetStringBounds(String str, int beginIndex, int limit, Graphics context)
		{
			return Font_Renamed.GetStringBounds(str, beginIndex, limit, MyFRC(context));
		}

	   /// <summary>
	   /// Returns the bounds of the specified array of characters
	   /// in the specified <code>Graphics</code> context.
	   /// The bounds is used to layout the <code>String</code>
	   /// created with the specified array of characters,
	   /// <code>beginIndex</code> and <code>limit</code>.
	   /// <para>Note: The returned bounds is in baseline-relative coordinates
	   /// (see <seealso cref="java.awt.FontMetrics class notes"/>).
	   /// </para>
	   /// </summary>
	   /// <param name="chars"> an array of characters </param>
	   /// <param name="beginIndex"> the initial offset of the array of
	   /// characters </param>
	   /// <param name="limit"> the end offset of the array of characters </param>
	   /// <param name="context"> the specified <code>Graphics</code> context </param>
	   /// <returns> a <code>Rectangle2D</code> that is the bounding box of the
	   /// specified character array in the specified
	   /// <code>Graphics</code> context. </returns>
	   /// <seealso cref= java.awt.Font#getStringBounds(char[], int, int, FontRenderContext) </seealso>
		public virtual Rectangle2D GetStringBounds(char[] chars, int beginIndex, int limit, Graphics context)
		{
			return Font_Renamed.GetStringBounds(chars, beginIndex, limit, MyFRC(context));
		}

	   /// <summary>
	   /// Returns the bounds of the characters indexed in the specified
	   /// <code>CharacterIterator</code> in the
	   /// specified <code>Graphics</code> context.
	   /// <para>Note: The returned bounds is in baseline-relative coordinates
	   /// (see <seealso cref="java.awt.FontMetrics class notes"/>).
	   /// </para>
	   /// </summary>
	   /// <param name="ci"> the specified <code>CharacterIterator</code> </param>
	   /// <param name="beginIndex"> the initial offset in <code>ci</code> </param>
	   /// <param name="limit"> the end index of <code>ci</code> </param>
	   /// <param name="context"> the specified <code>Graphics</code> context </param>
	   /// <returns> a <code>Rectangle2D</code> that is the bounding box of the
	   /// characters indexed in the specified <code>CharacterIterator</code>
	   /// in the specified <code>Graphics</code> context. </returns>
	   /// <seealso cref= java.awt.Font#getStringBounds(CharacterIterator, int, int, FontRenderContext) </seealso>
		public virtual Rectangle2D GetStringBounds(CharacterIterator ci, int beginIndex, int limit, Graphics context)
		{
			return Font_Renamed.GetStringBounds(ci, beginIndex, limit, MyFRC(context));
		}

		/// <summary>
		/// Returns the bounds for the character with the maximum bounds
		/// in the specified <code>Graphics</code> context. </summary>
		/// <param name="context"> the specified <code>Graphics</code> context </param>
		/// <returns> a <code>Rectangle2D</code> that is the
		/// bounding box for the character with the maximum bounds. </returns>
		/// <seealso cref= java.awt.Font#getMaxCharBounds(FontRenderContext) </seealso>
		public virtual Rectangle2D GetMaxCharBounds(Graphics context)
		{
			return Font_Renamed.GetMaxCharBounds(MyFRC(context));
		}

		private FontRenderContext MyFRC(Graphics context)
		{
			if (context is Graphics2D)
			{
				return ((Graphics2D)context).FontRenderContext;
			}
			return DEFAULT_FRC;
		}


		/// <summary>
		/// Returns a representation of this <code>FontMetrics</code>
		/// object's values as a <code>String</code>. </summary>
		/// <returns>    a <code>String</code> representation of this
		/// <code>FontMetrics</code> object.
		/// @since     JDK1.0. </returns>
		public override String ToString()
		{
//JAVA TO C# CONVERTER WARNING: The .NET Type.FullName property will not always yield results identical to the Java Class.getName method:
			return this.GetType().FullName + "[font=" + Font + "ascent=" + Ascent + ", descent=" + Descent + ", height=" + Height + "]";
		}

		/// <summary>
		/// Initialize JNI field and method IDs
		/// </summary>
//JAVA TO C# CONVERTER TODO TASK: Replace 'unknown' with the appropriate dll name:
		[DllImport("unknown")]
		private static extern void initIDs();
	}

}
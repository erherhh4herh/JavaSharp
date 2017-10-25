using System;
using System.Collections.Generic;

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

/*
 * (C) Copyright Taligent, Inc. 1996 - 1997, All Rights Reserved
 * (C) Copyright IBM Corp. 1996-2003, All Rights Reserved
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

	using AttributeValues = sun.font.AttributeValues;
	using CoreMetrics = sun.font.CoreMetrics;
	using Decoration = sun.font.Decoration;
	using FontLineMetrics = sun.font.FontLineMetrics;
	using FontResolver = sun.font.FontResolver;
	using GraphicComponent = sun.font.GraphicComponent;
	using LayoutPathImpl = sun.font.LayoutPathImpl;
	using CodePointIterator = sun.text.CodePointIterator;

	/// 
	/// <summary>
	/// <code>TextLayout</code> is an immutable graphical representation of styled
	/// character data.
	/// <para>
	/// It provides the following capabilities:
	/// <ul>
	/// <li>implicit bidirectional analysis and reordering,
	/// <li>cursor positioning and movement, including split cursors for
	/// mixed directional text,
	/// <li>highlighting, including both logical and visual highlighting
	/// for mixed directional text,
	/// <li>multiple baselines (roman, hanging, and centered),
	/// <li>hit testing,
	/// <li>justification,
	/// <li>default font substitution,
	/// <li>metric information such as ascent, descent, and advance, and
	/// <li>rendering
	/// </ul>
	/// </para>
	/// <para>
	/// A <code>TextLayout</code> object can be rendered using
	/// its <code>draw</code> method.
	/// </para>
	/// <para>
	/// <code>TextLayout</code> can be constructed either directly or through
	/// the use of a <seealso cref="LineBreakMeasurer"/>.  When constructed directly, the
	/// source text represents a single paragraph.  <code>LineBreakMeasurer</code>
	/// allows styled text to be broken into lines that fit within a particular
	/// width.  See the <code>LineBreakMeasurer</code> documentation for more
	/// information.
	/// </para>
	/// <para>
	/// <code>TextLayout</code> construction logically proceeds as follows:
	/// <ul>
	/// <li>paragraph attributes are extracted and examined,
	/// <li>text is analyzed for bidirectional reordering, and reordering
	/// information is computed if needed,
	/// <li>text is segmented into style runs
	/// <li>fonts are chosen for style runs, first by using a font if the
	/// attribute <seealso cref="TextAttribute#FONT"/> is present, otherwise by computing
	/// a default font using the attributes that have been defined
	/// <li>if text is on multiple baselines, the runs or subruns are further
	/// broken into subruns sharing a common baseline,
	/// <li>glyphvectors are generated for each run using the chosen font,
	/// <li>final bidirectional reordering is performed on the glyphvectors
	/// </ul>
	/// </para>
	/// <para>
	/// All graphical information returned from a <code>TextLayout</code>
	/// object's methods is relative to the origin of the
	/// <code>TextLayout</code>, which is the intersection of the
	/// <code>TextLayout</code> object's baseline with its left edge.  Also,
	/// coordinates passed into a <code>TextLayout</code> object's methods
	/// are assumed to be relative to the <code>TextLayout</code> object's
	/// origin.  Clients usually need to translate between a
	/// <code>TextLayout</code> object's coordinate system and the coordinate
	/// system in another object (such as a
	/// <seealso cref="java.awt.Graphics Graphics"/> object).
	/// </para>
	/// <para>
	/// <code>TextLayout</code> objects are constructed from styled text,
	/// but they do not retain a reference to their source text.  Thus,
	/// changes in the text previously used to generate a <code>TextLayout</code>
	/// do not affect the <code>TextLayout</code>.
	/// </para>
	/// <para>
	/// Three methods on a <code>TextLayout</code> object
	/// (<code>getNextRightHit</code>, <code>getNextLeftHit</code>, and
	/// <code>hitTestChar</code>) return instances of <seealso cref="TextHitInfo"/>.
	/// The offsets contained in these <code>TextHitInfo</code> objects
	/// are relative to the start of the <code>TextLayout</code>, <b>not</b>
	/// to the text used to create the <code>TextLayout</code>.  Similarly,
	/// <code>TextLayout</code> methods that accept <code>TextHitInfo</code>
	/// instances as parameters expect the <code>TextHitInfo</code> object's
	/// offsets to be relative to the <code>TextLayout</code>, not to any
	/// underlying text storage model.
	/// </para>
	/// <para>
	/// </para>
	/// <strong>Examples</strong>:<para>
	/// Constructing and drawing a <code>TextLayout</code> and its bounding
	/// rectangle:
	/// <blockquote><pre>
	///   Graphics2D g = ...;
	///   Point2D loc = ...;
	///   Font font = Font.getFont("Helvetica-bold-italic");
	///   FontRenderContext frc = g.getFontRenderContext();
	///   TextLayout layout = new TextLayout("This is a string", font, frc);
	///   layout.draw(g, (float)loc.getX(), (float)loc.getY());
	/// 
	///   Rectangle2D bounds = layout.getBounds();
	///   bounds.setRect(bounds.getX()+loc.getX(),
	///                  bounds.getY()+loc.getY(),
	///                  bounds.getWidth(),
	///                  bounds.getHeight());
	///   g.draw(bounds);
	/// </pre>
	/// </blockquote>
	/// </para>
	/// <para>
	/// Hit-testing a <code>TextLayout</code> (determining which character is at
	/// a particular graphical location):
	/// <blockquote><pre>
	///   Point2D click = ...;
	///   TextHitInfo hit = layout.hitTestChar(
	///                         (float) (click.getX() - loc.getX()),
	///                         (float) (click.getY() - loc.getY()));
	/// </pre>
	/// </blockquote>
	/// </para>
	/// <para>
	/// Responding to a right-arrow key press:
	/// <blockquote><pre>
	///   int insertionIndex = ...;
	///   TextHitInfo next = layout.getNextRightHit(insertionIndex);
	///   if (next != null) {
	///       // translate graphics to origin of layout on screen
	///       g.translate(loc.getX(), loc.getY());
	///       Shape[] carets = layout.getCaretShapes(next.getInsertionIndex());
	///       g.draw(carets[0]);
	///       if (carets[1] != null) {
	///           g.draw(carets[1]);
	///       }
	///   }
	/// </pre></blockquote>
	/// </para>
	/// <para>
	/// Drawing a selection range corresponding to a substring in the source text.
	/// The selected area may not be visually contiguous:
	/// <blockquote><pre>
	///   // selStart, selLimit should be relative to the layout,
	///   // not to the source text
	/// 
	///   int selStart = ..., selLimit = ...;
	///   Color selectionColor = ...;
	///   Shape selection = layout.getLogicalHighlightShape(selStart, selLimit);
	///   // selection may consist of disjoint areas
	///   // graphics is assumed to be tranlated to origin of layout
	///   g.setColor(selectionColor);
	///   g.fill(selection);
	/// </pre></blockquote>
	/// </para>
	/// <para>
	/// Drawing a visually contiguous selection range.  The selection range may
	/// correspond to more than one substring in the source text.  The ranges of
	/// the corresponding source text substrings can be obtained with
	/// <code>getLogicalRangesForVisualSelection()</code>:
	/// <blockquote><pre>
	///   TextHitInfo selStart = ..., selLimit = ...;
	///   Shape selection = layout.getVisualHighlightShape(selStart, selLimit);
	///   g.setColor(selectionColor);
	///   g.fill(selection);
	///   int[] ranges = getLogicalRangesForVisualSelection(selStart, selLimit);
	///   // ranges[0], ranges[1] is the first selection range,
	///   // ranges[2], ranges[3] is the second selection range, etc.
	/// </pre></blockquote>
	/// </para>
	/// <para>
	/// Note: Font rotations can cause text baselines to be rotated, and
	/// multiple runs with different rotations can cause the baseline to
	/// bend or zig-zag.  In order to account for this (rare) possibility,
	/// some APIs are specified to return metrics and take parameters 'in
	/// baseline-relative coordinates' (e.g. ascent, advance), and others
	/// are in 'in standard coordinates' (e.g. getBounds).  Values in
	/// baseline-relative coordinates map the 'x' coordinate to the
	/// distance along the baseline, (positive x is forward along the
	/// baseline), and the 'y' coordinate to a distance along the
	/// perpendicular to the baseline at 'x' (positive y is 90 degrees
	/// clockwise from the baseline vector).  Values in standard
	/// coordinates are measured along the x and y axes, with 0,0 at the
	/// origin of the TextLayout.  Documentation for each relevant API
	/// indicates what values are in what coordinate system.  In general,
	/// measurement-related APIs are in baseline-relative coordinates,
	/// while display-related APIs are in standard coordinates.
	/// 
	/// </para>
	/// </summary>
	/// <seealso cref= LineBreakMeasurer </seealso>
	/// <seealso cref= TextAttribute </seealso>
	/// <seealso cref= TextHitInfo </seealso>
	/// <seealso cref= LayoutPath </seealso>
	public sealed class TextLayout : Cloneable
	{

		private int CharacterCount_Renamed;
		private bool IsVerticalLine = false;
		private sbyte Baseline_Renamed;
		private float[] BaselineOffsets_Renamed; // why have these ?
		private TextLine TextLine;

		// cached values computed from GlyphSets and set info:
		// all are recomputed from scratch in buildCache()
		private TextLine.TextLineMetrics LineMetrics = null;
		private float VisibleAdvance_Renamed;
		private int HashCodeCache;

		/*
		 * TextLayouts are supposedly immutable.  If you mutate a TextLayout under
		 * the covers (like the justification code does) you'll need to set this
		 * back to false.  Could be replaced with textLine != null <--> cacheIsValid.
		 */
		private bool CacheIsValid = false;


		// This value is obtained from an attribute, and constrained to the
		// interval [0,1].  If 0, the layout cannot be justified.
		private float JustifyRatio;

		// If a layout is produced by justification, then that layout
		// cannot be justified.  To enforce this constraint the
		// justifyRatio of the justified layout is set to this value.
		private const float ALREADY_JUSTIFIED = -53.9f;

		// dx and dy specify the distance between the TextLayout's origin
		// and the origin of the leftmost GlyphSet (TextLayoutComponent,
		// actually).  They were used for hanging punctuation support,
		// which is no longer implemented.  Currently they are both always 0,
		// and TextLayout is not guaranteed to work with non-zero dx, dy
		// values right now.  They were left in as an aide and reminder to
		// anyone who implements hanging punctuation or other similar stuff.
		// They are static now so they don't take up space in TextLayout
		// instances.
		private static float Dx;
		private static float Dy;

		/*
		 * Natural bounds is used internally.  It is built on demand in
		 * getNaturalBounds.
		 */
		private Rectangle2D NaturalBounds_Renamed = null;

		/*
		 * boundsRect encloses all of the bits this TextLayout can draw.  It
		 * is build on demand in getBounds.
		 */
		private Rectangle2D BoundsRect = null;

		/*
		 * flag to supress/allow carets inside of ligatures when hit testing or
		 * arrow-keying
		 */
		private bool CaretsInLigaturesAreAllowed = false;

		/// <summary>
		/// Defines a policy for determining the strong caret location.
		/// This class contains one method, <code>getStrongCaret</code>, which
		/// is used to specify the policy that determines the strong caret in
		/// dual-caret text.  The strong caret is used to move the caret to the
		/// left or right. Instances of this class can be passed to
		/// <code>getCaretShapes</code>, <code>getNextLeftHit</code> and
		/// <code>getNextRightHit</code> to customize strong caret
		/// selection.
		/// <para>
		/// To specify alternate caret policies, subclass <code>CaretPolicy</code>
		/// and override <code>getStrongCaret</code>.  <code>getStrongCaret</code>
		/// should inspect the two <code>TextHitInfo</code> arguments and choose
		/// one of them as the strong caret.
		/// </para>
		/// <para>
		/// Most clients do not need to use this class.
		/// </para>
		/// </summary>
		public class CaretPolicy
		{

			/// <summary>
			/// Constructs a <code>CaretPolicy</code>.
			/// </summary>
			 public CaretPolicy()
			 {
			 }

			/// <summary>
			/// Chooses one of the specified <code>TextHitInfo</code> instances as
			/// a strong caret in the specified <code>TextLayout</code>. </summary>
			/// <param name="hit1"> a valid hit in <code>layout</code> </param>
			/// <param name="hit2"> a valid hit in <code>layout</code> </param>
			/// <param name="layout"> the <code>TextLayout</code> in which
			///        <code>hit1</code> and <code>hit2</code> are used </param>
			/// <returns> <code>hit1</code> or <code>hit2</code>
			///        (or an equivalent <code>TextHitInfo</code>), indicating the
			///        strong caret. </returns>
			public virtual TextHitInfo GetStrongCaret(TextHitInfo hit1, TextHitInfo hit2, TextLayout layout)
			{

				// default implementation just calls private method on layout
				return layout.GetStrongHit(hit1, hit2);
			}
		}

		/// <summary>
		/// This <code>CaretPolicy</code> is used when a policy is not specified
		/// by the client.  With this policy, a hit on a character whose direction
		/// is the same as the line direction is stronger than a hit on a
		/// counterdirectional character.  If the characters' directions are
		/// the same, a hit on the leading edge of a character is stronger
		/// than a hit on the trailing edge of a character.
		/// </summary>
		public static readonly CaretPolicy DEFAULT_CARET_POLICY = new CaretPolicy();

		/// <summary>
		/// Constructs a <code>TextLayout</code> from a <code>String</code>
		/// and a <seealso cref="Font"/>.  All the text is styled using the specified
		/// <code>Font</code>.
		/// <para>
		/// The <code>String</code> must specify a single paragraph of text,
		/// because an entire paragraph is required for the bidirectional
		/// algorithm.
		/// </para>
		/// </summary>
		/// <param name="string"> the text to display </param>
		/// <param name="font"> a <code>Font</code> used to style the text </param>
		/// <param name="frc"> contains information about a graphics device which is needed
		///       to measure the text correctly.
		///       Text measurements can vary slightly depending on the
		///       device resolution, and attributes such as antialiasing.  This
		///       parameter does not specify a translation between the
		///       <code>TextLayout</code> and user space. </param>
		public TextLayout(String @string, Font font, FontRenderContext frc)
		{

			if (font == null)
			{
				throw new IllegalArgumentException("Null font passed to TextLayout constructor.");
			}

			if (@string == null)
			{
				throw new IllegalArgumentException("Null string passed to TextLayout constructor.");
			}

			if (@string.Length() == 0)
			{
				throw new IllegalArgumentException("Zero length string passed to TextLayout constructor.");
			}

//JAVA TO C# CONVERTER TODO TASK: Java wildcard generics are not converted to .NET:
//ORIGINAL LINE: java.util.Map<? extends java.text.AttributedCharacterIterator_Attribute, ?> attributes = null;
			IDictionary<?, ?> attributes = null;
			if (font.HasLayoutAttributes())
			{
				attributes = font.Attributes;
			}

			char[] text = @string.ToCharArray();
			if (SameBaselineUpTo(font, text, 0, text.Length) == text.Length)
			{
				FastInit(text, font, attributes, frc);
			}
			else
			{
				AttributedString @as = attributes == null ? new AttributedString(@string) : new AttributedString(@string, attributes);
				@as.AddAttribute(TextAttribute.FONT, font);
				StandardInit(@as.Iterator, text, frc);
			}
		}

		/// <summary>
		/// Constructs a <code>TextLayout</code> from a <code>String</code>
		/// and an attribute set.
		/// <para>
		/// All the text is styled using the provided attributes.
		/// </para>
		/// <para>
		/// <code>string</code> must specify a single paragraph of text because an
		/// entire paragraph is required for the bidirectional algorithm.
		/// </para>
		/// </summary>
		/// <param name="string"> the text to display </param>
		/// <param name="attributes"> the attributes used to style the text </param>
		/// <param name="frc"> contains information about a graphics device which is needed
		///       to measure the text correctly.
		///       Text measurements can vary slightly depending on the
		///       device resolution, and attributes such as antialiasing.  This
		///       parameter does not specify a translation between the
		///       <code>TextLayout</code> and user space. </param>
		public TextLayout<T1>(String @string, IDictionary<T1> attributes, FontRenderContext frc) where T1 : java.text.AttributedCharacterIterator_Attribute
		{
			if (@string == null)
			{
				throw new IllegalArgumentException("Null string passed to TextLayout constructor.");
			}

			if (attributes == null)
			{
				throw new IllegalArgumentException("Null map passed to TextLayout constructor.");
			}

			if (@string.Length() == 0)
			{
				throw new IllegalArgumentException("Zero length string passed to TextLayout constructor.");
			}

			char[] text = @string.ToCharArray();
			Font font = SingleFont(text, 0, text.Length, attributes);
			if (font != null)
			{
				FastInit(text, font, attributes, frc);
			}
			else
			{
				AttributedString @as = new AttributedString(@string, attributes);
				StandardInit(@as.Iterator, text, frc);
			}
		}

		/*
		 * Determines a font for the attributes, and if a single font can render
		 * all the text on one baseline, return it, otherwise null.  If the
		 * attributes specify a font, assume it can display all the text without
		 * checking.
		 * If the AttributeSet contains an embedded graphic, return null.
		 */
		private static Font singleFont<T1>(char[] text, int start, int limit, IDictionary<T1> attributes) where T1 : java.text.AttributedCharacterIterator_Attribute
		{

			if (attributes[TextAttribute.CHAR_REPLACEMENT] != null)
			{
				return null;
			}

			Font font = null;
			try
			{
				font = (Font)attributes[TextAttribute.FONT];
			}
			catch (ClassCastException)
			{
			}
			if (font == null)
			{
				if (attributes[TextAttribute.FAMILY] != null)
				{
					font = Font.GetFont(attributes);
					if (font.CanDisplayUpTo(text, start, limit) != -1)
					{
						return null;
					}
				}
				else
				{
					FontResolver resolver = FontResolver.Instance;
					CodePointIterator iter = CodePointIterator.create(text, start, limit);
					int fontIndex = resolver.nextFontRunIndex(iter);
					if (iter.charIndex() == limit)
					{
						font = resolver.getFont(fontIndex, attributes);
					}
				}
			}

			if (SameBaselineUpTo(font, text, start, limit) != limit)
			{
				return null;
			}

			return font;
		}

		/// <summary>
		/// Constructs a <code>TextLayout</code> from an iterator over styled text.
		/// <para>
		/// The iterator must specify a single paragraph of text because an
		/// entire paragraph is required for the bidirectional
		/// algorithm.
		/// </para>
		/// </summary>
		/// <param name="text"> the styled text to display </param>
		/// <param name="frc"> contains information about a graphics device which is needed
		///       to measure the text correctly.
		///       Text measurements can vary slightly depending on the
		///       device resolution, and attributes such as antialiasing.  This
		///       parameter does not specify a translation between the
		///       <code>TextLayout</code> and user space. </param>
		public TextLayout(AttributedCharacterIterator text, FontRenderContext frc)
		{

			if (text == null)
			{
				throw new IllegalArgumentException("Null iterator passed to TextLayout constructor.");
			}

			int start = text.BeginIndex;
			int limit = text.EndIndex;
			if (start == limit)
			{
				throw new IllegalArgumentException("Zero length iterator passed to TextLayout constructor.");
			}

			int len = limit - start;
			text.First();
			char[] chars = new char[len];
			int n = 0;
			for (char c = text.First(); c != java.text.CharacterIterator_Fields.DONE; c = text.Next())
			{
				chars[n++] = c;
			}

			text.First();
			if (text.RunLimit == limit)
			{

//JAVA TO C# CONVERTER TODO TASK: Java wildcard generics are not converted to .NET:
//ORIGINAL LINE: java.util.Map<? extends java.text.AttributedCharacterIterator_Attribute, ?> attributes = text.getAttributes();
				IDictionary<?, ?> attributes = text.Attributes;
				Font font = SingleFont(chars, 0, len, attributes);
				if (font != null)
				{
					FastInit(chars, font, attributes, frc);
					return;
				}
			}

			StandardInit(text, chars, frc);
		}

		/// <summary>
		/// Creates a <code>TextLayout</code> from a <seealso cref="TextLine"/> and
		/// some paragraph data.  This method is used by <seealso cref="TextMeasurer"/>. </summary>
		/// <param name="textLine"> the line measurement attributes to apply to the
		///       the resulting <code>TextLayout</code> </param>
		/// <param name="baseline"> the baseline of the text </param>
		/// <param name="baselineOffsets"> the baseline offsets for this
		/// <code>TextLayout</code>.  This should already be normalized to
		/// <code>baseline</code> </param>
		/// <param name="justifyRatio"> <code>0</code> if the <code>TextLayout</code>
		///     cannot be justified; <code>1</code> otherwise. </param>
		internal TextLayout(TextLine textLine, sbyte baseline, float[] baselineOffsets, float justifyRatio)
		{

			this.CharacterCount_Renamed = textLine.CharacterCount();
			this.Baseline_Renamed = baseline;
			this.BaselineOffsets_Renamed = baselineOffsets;
			this.TextLine = textLine;
			this.JustifyRatio = justifyRatio;
		}

		/// <summary>
		/// Initialize the paragraph-specific data.
		/// </summary>
		private void paragraphInit<T1>(sbyte aBaseline, CoreMetrics lm, IDictionary<T1> paragraphAttrs, char[] text) where T1 : java.text.AttributedCharacterIterator_Attribute
		{

			Baseline_Renamed = aBaseline;

			// normalize to current baseline
			BaselineOffsets_Renamed = TextLine.GetNormalizedOffsets(lm.baselineOffsets, Baseline_Renamed);

			JustifyRatio = AttributeValues.getJustification(paragraphAttrs);
			NumericShaper shaper = AttributeValues.getNumericShaping(paragraphAttrs);
			if (shaper != null)
			{
				shaper.Shape(text, 0, text.Length);
			}
		}

		/*
		 * the fast init generates a single glyph set.  This requires:
		 * all one style
		 * all renderable by one font (ie no embedded graphics)
		 * all on one baseline
		 */
		private void fastInit<T1>(char[] chars, Font font, IDictionary<T1> attrs, FontRenderContext frc) where T1 : java.text.AttributedCharacterIterator_Attribute
		{

			// Object vf = attrs.get(TextAttribute.ORIENTATION);
			// isVerticalLine = TextAttribute.ORIENTATION_VERTICAL.equals(vf);
			IsVerticalLine = false;

			LineMetrics lm = font.GetLineMetrics(chars, 0, chars.Length, frc);
			CoreMetrics cm = CoreMetrics.get(lm);
			sbyte glyphBaseline = (sbyte) cm.baselineIndex;

			if (attrs == null)
			{
				Baseline_Renamed = glyphBaseline;
				BaselineOffsets_Renamed = cm.baselineOffsets;
				JustifyRatio = 1.0f;
			}
			else
			{
				ParagraphInit(glyphBaseline, cm, attrs, chars);
			}

			CharacterCount_Renamed = chars.Length;

			TextLine = TextLine.FastCreateTextLine(frc, chars, font, cm, attrs);
		}

		/*
		 * the standard init generates multiple glyph sets based on style,
		 * renderable, and baseline runs.
		 * @param chars the text in the iterator, extracted into a char array
		 */
		private void StandardInit(AttributedCharacterIterator text, char[] chars, FontRenderContext frc)
		{

			CharacterCount_Renamed = chars.Length;

			{
			// set paragraph attributes
				// If there's an embedded graphic at the start of the
				// paragraph, look for the first non-graphic character
				// and use it and its font to initialize the paragraph.
				// If not, use the first graphic to initialize.

//JAVA TO C# CONVERTER TODO TASK: Java wildcard generics are not converted to .NET:
//ORIGINAL LINE: java.util.Map<? extends java.text.AttributedCharacterIterator_Attribute, ?> paragraphAttrs = text.getAttributes();
				IDictionary<?, ?> paragraphAttrs = text.Attributes;

				bool haveFont = TextLine.AdvanceToFirstFont(text);

				if (haveFont)
				{
					Font defaultFont = TextLine.GetFontAtCurrentPos(text);
					int charsStart = text.Index - text.BeginIndex;
					LineMetrics lm = defaultFont.GetLineMetrics(chars, charsStart, charsStart + 1, frc);
					CoreMetrics cm = CoreMetrics.get(lm);
					ParagraphInit((sbyte)cm.baselineIndex, cm, paragraphAttrs, chars);
				}
				else
				{
					// hmmm what to do here?  Just try to supply reasonable
					// values I guess.

					GraphicAttribute graphic = (GraphicAttribute) paragraphAttrs[TextAttribute.CHAR_REPLACEMENT];
					sbyte defaultBaseline = GetBaselineFromGraphic(graphic);
					CoreMetrics cm = GraphicComponent.createCoreMetrics(graphic);
					ParagraphInit(defaultBaseline, cm, paragraphAttrs, chars);
				}
			}

			TextLine = TextLine.StandardCreateTextLine(frc, text, chars, BaselineOffsets_Renamed);
		}

		/*
		 * A utility to rebuild the ascent/descent/leading/advance cache.
		 * You'll need to call this if you clone and mutate (like justification,
		 * editing methods do)
		 */
		private void EnsureCache()
		{
			if (!CacheIsValid)
			{
				BuildCache();
			}
		}

		private void BuildCache()
		{
			LineMetrics = TextLine.Metrics;

			// compute visibleAdvance
			if (TextLine.DirectionLTR)
			{

				int lastNonSpace = CharacterCount_Renamed - 1;
				while (lastNonSpace != -1)
				{
					int logIndex = TextLine.VisualToLogical(lastNonSpace);
					if (!TextLine.IsCharSpace(logIndex))
					{
						break;
					}
					else
					{
						--lastNonSpace;
					}
				}
				if (lastNonSpace == CharacterCount_Renamed - 1)
				{
					VisibleAdvance_Renamed = LineMetrics.Advance;
				}
				else if (lastNonSpace == -1)
				{
					VisibleAdvance_Renamed = 0;
				}
				else
				{
					int logIndex = TextLine.VisualToLogical(lastNonSpace);
					VisibleAdvance_Renamed = TextLine.GetCharLinePosition(logIndex) + TextLine.GetCharAdvance(logIndex);
				}
			}
			else
			{

				int leftmostNonSpace = 0;
				while (leftmostNonSpace != CharacterCount_Renamed)
				{
					int logIndex = TextLine.VisualToLogical(leftmostNonSpace);
					if (!TextLine.IsCharSpace(logIndex))
					{
						break;
					}
					else
					{
						++leftmostNonSpace;
					}
				}
				if (leftmostNonSpace == CharacterCount_Renamed)
				{
					VisibleAdvance_Renamed = 0;
				}
				else if (leftmostNonSpace == 0)
				{
					VisibleAdvance_Renamed = LineMetrics.Advance;
				}
				else
				{
					int logIndex = TextLine.VisualToLogical(leftmostNonSpace);
					float pos = TextLine.GetCharLinePosition(logIndex);
					VisibleAdvance_Renamed = LineMetrics.Advance - pos;
				}
			}

			// naturalBounds, boundsRect will be generated on demand
			NaturalBounds_Renamed = null;
			BoundsRect = null;

			// hashCode will be regenerated on demand
			HashCodeCache = 0;

			CacheIsValid = true;
		}

		/// <summary>
		/// The 'natural bounds' encloses all the carets the layout can draw.
		/// 
		/// </summary>
		private Rectangle2D NaturalBounds
		{
			get
			{
				EnsureCache();
    
				if (NaturalBounds_Renamed == null)
				{
					NaturalBounds_Renamed = TextLine.ItalicBounds;
				}
    
				return NaturalBounds_Renamed;
			}
		}

		/// <summary>
		/// Creates a copy of this <code>TextLayout</code>.
		/// </summary>
		protected internal Object Clone()
		{
			/*
			 * !!! I think this is safe.  Once created, nothing mutates the
			 * glyphvectors or arrays.  But we need to make sure.
			 * {jbr} actually, that's not quite true.  The justification code
			 * mutates after cloning.  It doesn't actually change the glyphvectors
			 * (that's impossible) but it replaces them with justified sets.  This
			 * is a problem for GlyphIterator creation, since new GlyphIterators
			 * are created by cloning a prototype.  If the prototype has outdated
			 * glyphvectors, so will the new ones.  A partial solution is to set the
			 * prototypical GlyphIterator to null when the glyphvectors change.  If
			 * you forget this one time, you're hosed.
			 */
			try
			{
				return base.Clone();
			}
			catch (CloneNotSupportedException e)
			{
				throw new InternalError(e);
			}
		}

		/*
		 * Utility to throw an expection if an invalid TextHitInfo is passed
		 * as a parameter.  Avoids code duplication.
		 */
		private void CheckTextHit(TextHitInfo hit)
		{
			if (hit == null)
			{
				throw new IllegalArgumentException("TextHitInfo is null.");
			}

			if (hit.InsertionIndex < 0 || hit.InsertionIndex > CharacterCount_Renamed)
			{
				throw new IllegalArgumentException("TextHitInfo is out of range");
			}
		}

		/// <summary>
		/// Creates a copy of this <code>TextLayout</code> justified to the
		/// specified width.
		/// <para>
		/// If this <code>TextLayout</code> has already been justified, an
		/// exception is thrown.  If this <code>TextLayout</code> object's
		/// justification ratio is zero, a <code>TextLayout</code> identical
		/// to this <code>TextLayout</code> is returned.
		/// </para>
		/// </summary>
		/// <param name="justificationWidth"> the width to use when justifying the line.
		/// For best results, it should not be too different from the current
		/// advance of the line. </param>
		/// <returns> a <code>TextLayout</code> justified to the specified width. </returns>
		/// <exception cref="Error"> if this layout has already been justified, an Error is
		/// thrown. </exception>
		public TextLayout GetJustifiedLayout(float justificationWidth)
		{

			if (justificationWidth <= 0)
			{
				throw new IllegalArgumentException("justificationWidth <= 0 passed to TextLayout.getJustifiedLayout()");
			}

			if (JustifyRatio == ALREADY_JUSTIFIED)
			{
				throw new Error("Can't justify again.");
			}

			EnsureCache(); // make sure textLine is not null

			// default justification range to exclude trailing logical whitespace
			int limit = CharacterCount_Renamed;
			while (limit > 0 && TextLine.IsCharWhitespace(limit - 1))
			{
				--limit;
			}

			TextLine newLine = TextLine.GetJustifiedLine(justificationWidth, JustifyRatio, 0, limit);
			if (newLine != null)
			{
				return new TextLayout(newLine, Baseline_Renamed, BaselineOffsets_Renamed, ALREADY_JUSTIFIED);
			}

			return this;
		}

		/// <summary>
		/// Justify this layout.  Overridden by subclassers to control justification
		/// (if there were subclassers, that is...)
		/// 
		/// The layout will only justify if the paragraph attributes (from the
		/// source text, possibly defaulted by the layout attributes) indicate a
		/// non-zero justification ratio.  The text will be justified to the
		/// indicated width.  The current implementation also adjusts hanging
		/// punctuation and trailing whitespace to overhang the justification width.
		/// Once justified, the layout may not be rejustified.
		/// <para>
		/// Some code may rely on immutablity of layouts.  Subclassers should not
		/// call this directly, but instead should call getJustifiedLayout, which
		/// will call this method on a clone of this layout, preserving
		/// the original.
		/// 
		/// </para>
		/// </summary>
		/// <param name="justificationWidth"> the width to use when justifying the line.
		/// For best results, it should not be too different from the current
		/// advance of the line. </param>
		/// <seealso cref= #getJustifiedLayout(float) </seealso>
		protected internal void HandleJustify(float justificationWidth)
		{
		  // never called
		}


		/// <summary>
		/// Returns the baseline for this <code>TextLayout</code>.
		/// The baseline is one of the values defined in <code>Font</code>,
		/// which are roman, centered and hanging.  Ascent and descent are
		/// relative to this baseline.  The <code>baselineOffsets</code>
		/// are also relative to this baseline. </summary>
		/// <returns> the baseline of this <code>TextLayout</code>. </returns>
		/// <seealso cref= #getBaselineOffsets() </seealso>
		/// <seealso cref= Font </seealso>
		public sbyte Baseline
		{
			get
			{
				return Baseline_Renamed;
			}
		}

		/// <summary>
		/// Returns the offsets array for the baselines used for this
		/// <code>TextLayout</code>.
		/// <para>
		/// The array is indexed by one of the values defined in
		/// <code>Font</code>, which are roman, centered and hanging.  The
		/// values are relative to this <code>TextLayout</code> object's
		/// baseline, so that <code>getBaselineOffsets[getBaseline()] == 0</code>.
		/// Offsets are added to the position of the <code>TextLayout</code>
		/// object's baseline to get the position for the new baseline.
		/// </para>
		/// </summary>
		/// <returns> the offsets array containing the baselines used for this
		///    <code>TextLayout</code>. </returns>
		/// <seealso cref= #getBaseline() </seealso>
		/// <seealso cref= Font </seealso>
		public float[] BaselineOffsets
		{
			get
			{
				float[] offsets = new float[BaselineOffsets_Renamed.Length];
				System.Array.Copy(BaselineOffsets_Renamed, 0, offsets, 0, offsets.Length);
				return offsets;
			}
		}

		/// <summary>
		/// Returns the advance of this <code>TextLayout</code>.
		/// The advance is the distance from the origin to the advance of the
		/// rightmost (bottommost) character.  This is in baseline-relative
		/// coordinates. </summary>
		/// <returns> the advance of this <code>TextLayout</code>. </returns>
		public float Advance
		{
			get
			{
				EnsureCache();
				return LineMetrics.Advance;
			}
		}

		/// <summary>
		/// Returns the advance of this <code>TextLayout</code>, minus trailing
		/// whitespace.  This is in baseline-relative coordinates. </summary>
		/// <returns> the advance of this <code>TextLayout</code> without the
		///      trailing whitespace. </returns>
		/// <seealso cref= #getAdvance() </seealso>
		public float VisibleAdvance
		{
			get
			{
				EnsureCache();
				return VisibleAdvance_Renamed;
			}
		}

		/// <summary>
		/// Returns the ascent of this <code>TextLayout</code>.
		/// The ascent is the distance from the top (right) of the
		/// <code>TextLayout</code> to the baseline.  It is always either
		/// positive or zero.  The ascent is sufficient to
		/// accommodate superscripted text and is the maximum of the sum of the
		/// ascent, offset, and baseline of each glyph.  The ascent is
		/// the maximum ascent from the baseline of all the text in the
		/// TextLayout.  It is in baseline-relative coordinates. </summary>
		/// <returns> the ascent of this <code>TextLayout</code>. </returns>
		public float Ascent
		{
			get
			{
				EnsureCache();
				return LineMetrics.Ascent;
			}
		}

		/// <summary>
		/// Returns the descent of this <code>TextLayout</code>.
		/// The descent is the distance from the baseline to the bottom (left) of
		/// the <code>TextLayout</code>.  It is always either positive or zero.
		/// The descent is sufficient to accommodate subscripted text and is the
		/// maximum of the sum of the descent, offset, and baseline of each glyph.
		/// This is the maximum descent from the baseline of all the text in
		/// the TextLayout.  It is in baseline-relative coordinates. </summary>
		/// <returns> the descent of this <code>TextLayout</code>. </returns>
		public float Descent
		{
			get
			{
				EnsureCache();
				return LineMetrics.Descent;
			}
		}

		/// <summary>
		/// Returns the leading of the <code>TextLayout</code>.
		/// The leading is the suggested interline spacing for this
		/// <code>TextLayout</code>.  This is in baseline-relative
		/// coordinates.
		/// <para>
		/// The leading is computed from the leading, descent, and baseline
		/// of all glyphvectors in the <code>TextLayout</code>.  The algorithm
		/// is roughly as follows:
		/// <blockquote><pre>
		/// maxD = 0;
		/// maxDL = 0;
		/// for (GlyphVector g in all glyphvectors) {
		///    maxD = max(maxD, g.getDescent() + offsets[g.getBaseline()]);
		///    maxDL = max(maxDL, g.getDescent() + g.getLeading() +
		///                       offsets[g.getBaseline()]);
		/// }
		/// return maxDL - maxD;
		/// </pre></blockquote>
		/// </para>
		/// </summary>
		/// <returns> the leading of this <code>TextLayout</code>. </returns>
		public float Leading
		{
			get
			{
				EnsureCache();
				return LineMetrics.Leading;
			}
		}

		/// <summary>
		/// Returns the bounds of this <code>TextLayout</code>.
		/// The bounds are in standard coordinates.
		/// <para>Due to rasterization effects, this bounds might not enclose all of the
		/// pixels rendered by the TextLayout.</para>
		/// It might not coincide exactly with the ascent, descent,
		/// origin or advance of the <code>TextLayout</code>. </summary>
		/// <returns> a <seealso cref="Rectangle2D"/> that is the bounds of this
		///        <code>TextLayout</code>. </returns>
		public Rectangle2D Bounds
		{
			get
			{
				EnsureCache();
    
				if (BoundsRect == null)
				{
					Rectangle2D vb = TextLine.VisualBounds;
					if (Dx != 0 || Dy != 0)
					{
						vb.SetRect(vb.X - Dx, vb.Y - Dy, vb.Width, vb.Height);
					}
					BoundsRect = vb;
				}
    
				Rectangle2D bounds = new Rectangle2D.Float();
				bounds.Rect = BoundsRect;
    
				return bounds;
			}
		}

		/// <summary>
		/// Returns the pixel bounds of this <code>TextLayout</code> when
		/// rendered in a graphics with the given
		/// <code>FontRenderContext</code> at the given location.  The
		/// graphics render context need not be the same as the
		/// <code>FontRenderContext</code> used to create this
		/// <code>TextLayout</code>, and can be null.  If it is null, the
		/// <code>FontRenderContext</code> of this <code>TextLayout</code>
		/// is used. </summary>
		/// <param name="frc"> the <code>FontRenderContext</code> of the <code>Graphics</code>. </param>
		/// <param name="x"> the x-coordinate at which to render this <code>TextLayout</code>. </param>
		/// <param name="y"> the y-coordinate at which to render this <code>TextLayout</code>. </param>
		/// <returns> a <code>Rectangle</code> bounding the pixels that would be affected. </returns>
		/// <seealso cref= GlyphVector#getPixelBounds
		/// @since 1.6 </seealso>
		public Rectangle GetPixelBounds(FontRenderContext frc, float x, float y)
		{
			return TextLine.GetPixelBounds(frc, x, y);
		}

		/// <summary>
		/// Returns <code>true</code> if this <code>TextLayout</code> has
		/// a left-to-right base direction or <code>false</code> if it has
		/// a right-to-left base direction.  The <code>TextLayout</code>
		/// has a base direction of either left-to-right (LTR) or
		/// right-to-left (RTL).  The base direction is independent of the
		/// actual direction of text on the line, which may be either LTR,
		/// RTL, or mixed. Left-to-right layouts by default should position
		/// flush left.  If the layout is on a tabbed line, the
		/// tabs run left to right, so that logically successive layouts position
		/// left to right.  The opposite is true for RTL layouts. By default they
		/// should position flush left, and tabs run right-to-left. </summary>
		/// <returns> <code>true</code> if the base direction of this
		///         <code>TextLayout</code> is left-to-right; <code>false</code>
		///         otherwise. </returns>
		public bool LeftToRight
		{
			get
			{
				return TextLine.DirectionLTR;
			}
		}

		/// <summary>
		/// Returns <code>true</code> if this <code>TextLayout</code> is vertical. </summary>
		/// <returns> <code>true</code> if this <code>TextLayout</code> is vertical;
		///      <code>false</code> otherwise. </returns>
		public bool Vertical
		{
			get
			{
				return IsVerticalLine;
			}
		}

		/// <summary>
		/// Returns the number of characters represented by this
		/// <code>TextLayout</code>. </summary>
		/// <returns> the number of characters in this <code>TextLayout</code>. </returns>
		public int CharacterCount
		{
			get
			{
				return CharacterCount_Renamed;
			}
		}

		/*
		 * carets and hit testing
		 *
		 * Positions on a text line are represented by instances of TextHitInfo.
		 * Any TextHitInfo with characterOffset between 0 and characterCount-1,
		 * inclusive, represents a valid position on the line.  Additionally,
		 * [-1, trailing] and [characterCount, leading] are valid positions, and
		 * represent positions at the logical start and end of the line,
		 * respectively.
		 *
		 * The characterOffsets in TextHitInfo's used and returned by TextLayout
		 * are relative to the beginning of the text layout, not necessarily to
		 * the beginning of the text storage the client is using.
		 *
		 *
		 * Every valid TextHitInfo has either one or two carets associated with it.
		 * A caret is a visual location in the TextLayout indicating where text at
		 * the TextHitInfo will be displayed on screen.  If a TextHitInfo
		 * represents a location on a directional boundary, then there are two
		 * possible visible positions for newly inserted text.  Consider the
		 * following example, in which capital letters indicate right-to-left text,
		 * and the overall line direction is left-to-right:
		 *
		 * Text Storage: [ a, b, C, D, E, f ]
		 * Display:        a b E D C f
		 *
		 * The text hit info (1, t) represents the trailing side of 'b'.  If 'q',
		 * a left-to-right character is inserted into the text storage at this
		 * location, it will be displayed between the 'b' and the 'E':
		 *
		 * Text Storage: [ a, b, q, C, D, E, f ]
		 * Display:        a b q E D C f
		 *
		 * However, if a 'W', which is right-to-left, is inserted into the storage
		 * after 'b', the storage and display will be:
		 *
		 * Text Storage: [ a, b, W, C, D, E, f ]
		 * Display:        a b E D C W f
		 *
		 * So, for the original text storage, two carets should be displayed for
		 * location (1, t): one visually between 'b' and 'E' and one visually
		 * between 'C' and 'f'.
		 *
		 *
		 * When two carets are displayed for a TextHitInfo, one caret is the
		 * 'strong' caret and the other is the 'weak' caret.  The strong caret
		 * indicates where an inserted character will be displayed when that
		 * character's direction is the same as the direction of the TextLayout.
		 * The weak caret shows where an character inserted character will be
		 * displayed when the character's direction is opposite that of the
		 * TextLayout.
		 *
		 *
		 * Clients should not be overly concerned with the details of correct
		 * caret display. TextLayout.getCaretShapes(TextHitInfo) will return an
		 * array of two paths representing where carets should be displayed.
		 * The first path in the array is the strong caret; the second element,
		 * if non-null, is the weak caret.  If the second element is null,
		 * then there is no weak caret for the given TextHitInfo.
		 *
		 *
		 * Since text can be visually reordered, logically consecutive
		 * TextHitInfo's may not be visually consecutive.  One implication of this
		 * is that a client cannot tell from inspecting a TextHitInfo whether the
		 * hit represents the first (or last) caret in the layout.  Clients
		 * can call getVisualOtherHit();  if the visual companion is
		 * (-1, TRAILING) or (characterCount, LEADING), then the hit is at the
		 * first (last) caret position in the layout.
		 */

		private float[] GetCaretInfo(int caret, Rectangle2D bounds, float[] info)
		{

			float top1X, top2X;
			float bottom1X, bottom2X;

			if (caret == 0 || caret == CharacterCount_Renamed)
			{

				float pos;
				int logIndex;
				if (caret == CharacterCount_Renamed)
				{
					logIndex = TextLine.VisualToLogical(CharacterCount_Renamed - 1);
					pos = TextLine.GetCharLinePosition(logIndex) + TextLine.GetCharAdvance(logIndex);
				}
				else
				{
					logIndex = TextLine.VisualToLogical(caret);
					pos = TextLine.GetCharLinePosition(logIndex);
				}
				float angle = TextLine.GetCharAngle(logIndex);
				float shift = TextLine.GetCharShift(logIndex);
				pos += angle * shift;
				top1X = top2X = pos + angle * TextLine.GetCharAscent(logIndex);
				bottom1X = bottom2X = pos - angle * TextLine.GetCharDescent(logIndex);
			}
			else
			{

			{
					int logIndex = TextLine.VisualToLogical(caret - 1);
					float angle1 = TextLine.GetCharAngle(logIndex);
					float pos1 = TextLine.GetCharLinePosition(logIndex) + TextLine.GetCharAdvance(logIndex);
					if (angle1 != 0)
					{
						pos1 += angle1 * TextLine.GetCharShift(logIndex);
						top1X = pos1 + angle1 * TextLine.GetCharAscent(logIndex);
						bottom1X = pos1 - angle1 * TextLine.GetCharDescent(logIndex);
					}
					else
					{
						top1X = bottom1X = pos1;
					}
				}
				{
					int logIndex = TextLine.VisualToLogical(caret);
					float angle2 = TextLine.GetCharAngle(logIndex);
					float pos2 = TextLine.GetCharLinePosition(logIndex);
					if (angle2 != 0)
					{
						pos2 += angle2 * TextLine.GetCharShift(logIndex);
						top2X = pos2 + angle2 * TextLine.GetCharAscent(logIndex);
						bottom2X = pos2 - angle2 * TextLine.GetCharDescent(logIndex);
					}
					else
					{
						top2X = bottom2X = pos2;
					}
				}
			}

			float topX = (top1X + top2X) / 2;
			float bottomX = (bottom1X + bottom2X) / 2;

			if (info == null)
			{
				info = new float[2];
			}

			if (IsVerticalLine)
			{
				info[1] = (float)((topX - bottomX) / bounds.Width);
				info[0] = (float)(topX + (info[1] * bounds.X));
			}
			else
			{
				info[1] = (float)((topX - bottomX) / bounds.Height);
				info[0] = (float)(bottomX + (info[1] * bounds.MaxY));
			}

			return info;
		}

		/// <summary>
		/// Returns information about the caret corresponding to <code>hit</code>.
		/// The first element of the array is the intersection of the caret with
		/// the baseline, as a distance along the baseline. The second element
		/// of the array is the inverse slope (run/rise) of the caret, measured
		/// with respect to the baseline at that point.
		/// <para>
		/// This method is meant for informational use.  To display carets, it
		/// is better to use <code>getCaretShapes</code>.
		/// </para>
		/// </summary>
		/// <param name="hit"> a hit on a character in this <code>TextLayout</code> </param>
		/// <param name="bounds"> the bounds to which the caret info is constructed.
		///     The bounds is in baseline-relative coordinates. </param>
		/// <returns> a two-element array containing the position and slope of
		/// the caret.  The returned caret info is in baseline-relative coordinates. </returns>
		/// <seealso cref= #getCaretShapes(int, Rectangle2D, TextLayout.CaretPolicy) </seealso>
		/// <seealso cref= Font#getItalicAngle </seealso>
		public float[] GetCaretInfo(TextHitInfo hit, Rectangle2D bounds)
		{
			EnsureCache();
			CheckTextHit(hit);

			return GetCaretInfoTestInternal(hit, bounds);
		}

		// this version provides extra info in the float array
		// the first two values are as above
		// the next four values are the endpoints of the caret, as computed
		// using the hit character's offset (baseline + ssoffset) and
		// natural ascent and descent.
		// these  values are trimmed to the bounds where required to fit,
		// but otherwise independent of it.
		private float[] GetCaretInfoTestInternal(TextHitInfo hit, Rectangle2D bounds)
		{
			EnsureCache();
			CheckTextHit(hit);

			float[] info = new float[6];

			// get old data first
			GetCaretInfo(HitToCaret(hit), bounds, info);

			// then add our new data
			double iangle, ixbase, p1x, p1y, p2x, p2y;

			int charix = hit.CharIndex;
			bool lead = hit.LeadingEdge;
			bool ltr = TextLine.DirectionLTR;
			bool horiz = !Vertical;

			if (charix == -1 || charix == CharacterCount_Renamed)
			{
				// !!! note: want non-shifted, baseline ascent and descent here!
				// TextLine should return appropriate line metrics object for these values
				TextLineMetrics m = TextLine.Metrics;
				bool low = ltr == (charix == -1);
				iangle = 0;
				if (horiz)
				{
					p1x = p2x = low ? 0 : m.Advance;
					p1y = -m.Ascent;
					p2y = m.Descent;
				}
				else
				{
					p1y = p2y = low ? 0 : m.Advance;
					p1x = m.Descent;
					p2x = m.Ascent;
				}
			}
			else
			{
				CoreMetrics thiscm = TextLine.GetCoreMetricsAt(charix);
				iangle = thiscm.italicAngle;
				ixbase = TextLine.GetCharLinePosition(charix, lead);
				if (thiscm.baselineIndex < 0)
				{
					// this is a graphic, no italics, use entire line height for caret
					TextLineMetrics m = TextLine.Metrics;
					if (horiz)
					{
						p1x = p2x = ixbase;
						if (thiscm.baselineIndex == GraphicAttribute.TOP_ALIGNMENT)
						{
							p1y = -m.Ascent;
							p2y = p1y + thiscm.height;
						}
						else
						{
							p2y = m.Descent;
							p1y = p2y - thiscm.height;
						}
					}
					else
					{
						p1y = p2y = ixbase;
						p1x = m.Descent;
						p2x = m.Ascent;
						// !!! top/bottom adjustment not implemented for vertical
					}
				}
				else
				{
					float bo = BaselineOffsets_Renamed[thiscm.baselineIndex];
					if (horiz)
					{
						ixbase += iangle * thiscm.ssOffset;
						p1x = ixbase + iangle * thiscm.ascent;
						p2x = ixbase - iangle * thiscm.descent;
						p1y = bo - thiscm.ascent;
						p2y = bo + thiscm.descent;
					}
					else
					{
						ixbase -= iangle * thiscm.ssOffset;
						p1y = ixbase + iangle * thiscm.ascent;
						p2y = ixbase - iangle * thiscm.descent;
						p1x = bo + thiscm.ascent;
						p2x = bo + thiscm.descent;
					}
				}
			}

			info[2] = (float)p1x;
			info[3] = (float)p1y;
			info[4] = (float)p2x;
			info[5] = (float)p2y;

			return info;
		}

		/// <summary>
		/// Returns information about the caret corresponding to <code>hit</code>.
		/// This method is a convenience overload of <code>getCaretInfo</code> and
		/// uses the natural bounds of this <code>TextLayout</code>. </summary>
		/// <param name="hit"> a hit on a character in this <code>TextLayout</code> </param>
		/// <returns> the information about a caret corresponding to a hit.  The
		///     returned caret info is in baseline-relative coordinates. </returns>
		public float[] GetCaretInfo(TextHitInfo hit)
		{

			return GetCaretInfo(hit, NaturalBounds);
		}

		/// <summary>
		/// Returns a caret index corresponding to <code>hit</code>.
		/// Carets are numbered from left to right (top to bottom) starting from
		/// zero. This always places carets next to the character hit, on the
		/// indicated side of the character. </summary>
		/// <param name="hit"> a hit on a character in this <code>TextLayout</code> </param>
		/// <returns> a caret index corresponding to the specified hit. </returns>
		private int HitToCaret(TextHitInfo hit)
		{

			int hitIndex = hit.CharIndex;

			if (hitIndex < 0)
			{
				return TextLine.DirectionLTR ? 0 : CharacterCount_Renamed;
			}
			else if (hitIndex >= CharacterCount_Renamed)
			{
				return TextLine.DirectionLTR ? CharacterCount_Renamed : 0;
			}

			int visIndex = TextLine.LogicalToVisual(hitIndex);

			if (hit.LeadingEdge != TextLine.IsCharLTR(hitIndex))
			{
				++visIndex;
			}

			return visIndex;
		}

		/// <summary>
		/// Given a caret index, return a hit whose caret is at the index.
		/// The hit is NOT guaranteed to be strong!!!
		/// </summary>
		/// <param name="caret"> a caret index. </param>
		/// <returns> a hit on this layout whose strong caret is at the requested
		/// index. </returns>
		private TextHitInfo CaretToHit(int caret)
		{

			if (caret == 0 || caret == CharacterCount_Renamed)
			{

				if ((caret == CharacterCount_Renamed) == TextLine.DirectionLTR)
				{
					return TextHitInfo.Leading(CharacterCount_Renamed);
				}
				else
				{
					return TextHitInfo.Trailing(-1);
				}
			}
			else
			{

				int charIndex = TextLine.VisualToLogical(caret);
				bool leading = TextLine.IsCharLTR(charIndex);

				return leading? TextHitInfo.Leading(charIndex) : TextHitInfo.Trailing(charIndex);
			}
		}

		private bool CaretIsValid(int caret)
		{

			if (caret == CharacterCount_Renamed || caret == 0)
			{
				return true;
			}

			int offset = TextLine.VisualToLogical(caret);

			if (!TextLine.IsCharLTR(offset))
			{
				offset = TextLine.VisualToLogical(caret - 1);
				if (TextLine.IsCharLTR(offset))
				{
					return true;
				}
			}

			// At this point, the leading edge of the character
			// at offset is at the given caret.

			return TextLine.CaretAtOffsetIsValid(offset);
		}

		/// <summary>
		/// Returns the hit for the next caret to the right (bottom); if there
		/// is no such hit, returns <code>null</code>.
		/// If the hit character index is out of bounds, an
		/// <seealso cref="IllegalArgumentException"/> is thrown. </summary>
		/// <param name="hit"> a hit on a character in this layout </param>
		/// <returns> a hit whose caret appears at the next position to the
		/// right (bottom) of the caret of the provided hit or <code>null</code>. </returns>
		public TextHitInfo GetNextRightHit(TextHitInfo hit)
		{
			EnsureCache();
			CheckTextHit(hit);

			int caret = HitToCaret(hit);

			if (caret == CharacterCount_Renamed)
			{
				return null;
			}

			do
			{
				++caret;
			} while (!CaretIsValid(caret));

			return CaretToHit(caret);
		}

		/// <summary>
		/// Returns the hit for the next caret to the right (bottom); if no
		/// such hit, returns <code>null</code>.  The hit is to the right of
		/// the strong caret at the specified offset, as determined by the
		/// specified policy.
		/// The returned hit is the stronger of the two possible
		/// hits, as determined by the specified policy. </summary>
		/// <param name="offset"> an insertion offset in this <code>TextLayout</code>.
		/// Cannot be less than 0 or greater than this <code>TextLayout</code>
		/// object's character count. </param>
		/// <param name="policy"> the policy used to select the strong caret </param>
		/// <returns> a hit whose caret appears at the next position to the
		/// right (bottom) of the caret of the provided hit, or <code>null</code>. </returns>
		public TextHitInfo GetNextRightHit(int offset, CaretPolicy policy)
		{

			if (offset < 0 || offset > CharacterCount_Renamed)
			{
				throw new IllegalArgumentException("Offset out of bounds in TextLayout.getNextRightHit()");
			}

			if (policy == null)
			{
				throw new IllegalArgumentException("Null CaretPolicy passed to TextLayout.getNextRightHit()");
			}

			TextHitInfo hit1 = TextHitInfo.AfterOffset(offset);
			TextHitInfo hit2 = hit1.OtherHit;

			TextHitInfo nextHit = GetNextRightHit(policy.GetStrongCaret(hit1, hit2, this));

			if (nextHit != null)
			{
				TextHitInfo otherHit = GetVisualOtherHit(nextHit);
				return policy.GetStrongCaret(otherHit, nextHit, this);
			}
			else
			{
				return null;
			}
		}

		/// <summary>
		/// Returns the hit for the next caret to the right (bottom); if no
		/// such hit, returns <code>null</code>.  The hit is to the right of
		/// the strong caret at the specified offset, as determined by the
		/// default policy.
		/// The returned hit is the stronger of the two possible
		/// hits, as determined by the default policy. </summary>
		/// <param name="offset"> an insertion offset in this <code>TextLayout</code>.
		/// Cannot be less than 0 or greater than the <code>TextLayout</code>
		/// object's character count. </param>
		/// <returns> a hit whose caret appears at the next position to the
		/// right (bottom) of the caret of the provided hit, or <code>null</code>. </returns>
		public TextHitInfo GetNextRightHit(int offset)
		{

			return GetNextRightHit(offset, DEFAULT_CARET_POLICY);
		}

		/// <summary>
		/// Returns the hit for the next caret to the left (top); if no such
		/// hit, returns <code>null</code>.
		/// If the hit character index is out of bounds, an
		/// <code>IllegalArgumentException</code> is thrown. </summary>
		/// <param name="hit"> a hit on a character in this <code>TextLayout</code>. </param>
		/// <returns> a hit whose caret appears at the next position to the
		/// left (top) of the caret of the provided hit, or <code>null</code>. </returns>
		public TextHitInfo GetNextLeftHit(TextHitInfo hit)
		{
			EnsureCache();
			CheckTextHit(hit);

			int caret = HitToCaret(hit);

			if (caret == 0)
			{
				return null;
			}

			do
			{
				--caret;
			} while (!CaretIsValid(caret));

			return CaretToHit(caret);
		}

		/// <summary>
		/// Returns the hit for the next caret to the left (top); if no
		/// such hit, returns <code>null</code>.  The hit is to the left of
		/// the strong caret at the specified offset, as determined by the
		/// specified policy.
		/// The returned hit is the stronger of the two possible
		/// hits, as determined by the specified policy. </summary>
		/// <param name="offset"> an insertion offset in this <code>TextLayout</code>.
		/// Cannot be less than 0 or greater than this <code>TextLayout</code>
		/// object's character count. </param>
		/// <param name="policy"> the policy used to select the strong caret </param>
		/// <returns> a hit whose caret appears at the next position to the
		/// left (top) of the caret of the provided hit, or <code>null</code>. </returns>
		public TextHitInfo GetNextLeftHit(int offset, CaretPolicy policy)
		{

			if (policy == null)
			{
				throw new IllegalArgumentException("Null CaretPolicy passed to TextLayout.getNextLeftHit()");
			}

			if (offset < 0 || offset > CharacterCount_Renamed)
			{
				throw new IllegalArgumentException("Offset out of bounds in TextLayout.getNextLeftHit()");
			}

			TextHitInfo hit1 = TextHitInfo.AfterOffset(offset);
			TextHitInfo hit2 = hit1.OtherHit;

			TextHitInfo nextHit = GetNextLeftHit(policy.GetStrongCaret(hit1, hit2, this));

			if (nextHit != null)
			{
				TextHitInfo otherHit = GetVisualOtherHit(nextHit);
				return policy.GetStrongCaret(otherHit, nextHit, this);
			}
			else
			{
				return null;
			}
		}

		/// <summary>
		/// Returns the hit for the next caret to the left (top); if no
		/// such hit, returns <code>null</code>.  The hit is to the left of
		/// the strong caret at the specified offset, as determined by the
		/// default policy.
		/// The returned hit is the stronger of the two possible
		/// hits, as determined by the default policy. </summary>
		/// <param name="offset"> an insertion offset in this <code>TextLayout</code>.
		/// Cannot be less than 0 or greater than this <code>TextLayout</code>
		/// object's character count. </param>
		/// <returns> a hit whose caret appears at the next position to the
		/// left (top) of the caret of the provided hit, or <code>null</code>. </returns>
		public TextHitInfo GetNextLeftHit(int offset)
		{

			return GetNextLeftHit(offset, DEFAULT_CARET_POLICY);
		}

		/// <summary>
		/// Returns the hit on the opposite side of the specified hit's caret. </summary>
		/// <param name="hit"> the specified hit </param>
		/// <returns> a hit that is on the opposite side of the specified hit's
		///    caret. </returns>
		public TextHitInfo GetVisualOtherHit(TextHitInfo hit)
		{

			EnsureCache();
			CheckTextHit(hit);

			int hitCharIndex = hit.CharIndex;

			int charIndex;
			bool leading;

			if (hitCharIndex == -1 || hitCharIndex == CharacterCount_Renamed)
			{

				int visIndex;
				if (TextLine.DirectionLTR == (hitCharIndex == -1))
				{
					visIndex = 0;
				}
				else
				{
					visIndex = CharacterCount_Renamed - 1;
				}

				charIndex = TextLine.VisualToLogical(visIndex);

				if (TextLine.DirectionLTR == (hitCharIndex == -1))
				{
					// at left end
					leading = TextLine.IsCharLTR(charIndex);
				}
				else
				{
					// at right end
					leading = !TextLine.IsCharLTR(charIndex);
				}
			}
			else
			{

				int visIndex = TextLine.LogicalToVisual(hitCharIndex);

				bool movedToRight;
				if (TextLine.IsCharLTR(hitCharIndex) == hit.LeadingEdge)
				{
					--visIndex;
					movedToRight = false;
				}
				else
				{
					++visIndex;
					movedToRight = true;
				}

				if (visIndex > -1 && visIndex < CharacterCount_Renamed)
				{
					charIndex = TextLine.VisualToLogical(visIndex);
					leading = movedToRight == TextLine.IsCharLTR(charIndex);
				}
				else
				{
					charIndex = (movedToRight == TextLine.DirectionLTR)? CharacterCount_Renamed : -1;
					leading = charIndex == CharacterCount_Renamed;
				}
			}

			return leading? TextHitInfo.Leading(charIndex) : TextHitInfo.Trailing(charIndex);
		}

		private double[] GetCaretPath(TextHitInfo hit, Rectangle2D bounds)
		{
			float[] info = GetCaretInfo(hit, bounds);
			return new double[] {info[2], info[3], info[4], info[5]};
		}

		/// <summary>
		/// Return an array of four floats corresponding the endpoints of the caret
		/// x0, y0, x1, y1.
		/// 
		/// This creates a line along the slope of the caret intersecting the
		/// baseline at the caret
		/// position, and extending from ascent above the baseline to descent below
		/// it.
		/// </summary>
		private double[] GetCaretPath(int caret, Rectangle2D bounds, bool clipToBounds)
		{

			float[] info = GetCaretInfo(caret, bounds, null);

			double pos = info[0];
			double slope = info[1];

			double x0, y0, x1, y1;
			double x2 = -3141.59, y2 = -2.7; // values are there to make compiler happy

			double left = bounds.X;
			double right = left + bounds.Width;
			double top = bounds.Y;
			double bottom = top + bounds.Height;

			bool threePoints = false;

			if (IsVerticalLine)
			{

				if (slope >= 0)
				{
					x0 = left;
					x1 = right;
				}
				else
				{
					x1 = left;
					x0 = right;
				}

				y0 = pos + x0 * slope;
				y1 = pos + x1 * slope;

				// y0 <= y1, always

				if (clipToBounds)
				{
					if (y0 < top)
					{
						if (slope <= 0 || y1 <= top)
						{
							y0 = y1 = top;
						}
						else
						{
							threePoints = true;
							y0 = top;
							y2 = top;
							x2 = x1 + (top - y1) / slope;
							if (y1 > bottom)
							{
								y1 = bottom;
							}
						}
					}
					else if (y1 > bottom)
					{
						if (slope >= 0 || y0 >= bottom)
						{
							y0 = y1 = bottom;
						}
						else
						{
							threePoints = true;
							y1 = bottom;
							y2 = bottom;
							x2 = x0 + (bottom - x1) / slope;
						}
					}
				}

			}
			else
			{

				if (slope >= 0)
				{
					y0 = bottom;
					y1 = top;
				}
				else
				{
					y1 = bottom;
					y0 = top;
				}

				x0 = pos - y0 * slope;
				x1 = pos - y1 * slope;

				// x0 <= x1, always

				if (clipToBounds)
				{
					if (x0 < left)
					{
						if (slope <= 0 || x1 <= left)
						{
							x0 = x1 = left;
						}
						else
						{
							threePoints = true;
							x0 = left;
							x2 = left;
							y2 = y1 - (left - x1) / slope;
							if (x1 > right)
							{
								x1 = right;
							}
						}
					}
					else if (x1 > right)
					{
						if (slope >= 0 || x0 >= right)
						{
							x0 = x1 = right;
						}
						else
						{
							threePoints = true;
							x1 = right;
							x2 = right;
							y2 = y0 - (right - x0) / slope;
						}
					}
				}
			}

			return threePoints? new double[] {x0, y0, x2, y2, x1, y1} : new double[] {x0, y0, x1, y1};
		}


		private static GeneralPath PathToShape(double[] path, bool close, LayoutPathImpl lp)
		{
			GeneralPath result = new GeneralPath(GeneralPath.WIND_EVEN_ODD, path.Length);
			result.MoveTo((float)path[0], (float)path[1]);
			for (int i = 2; i < path.Length; i += 2)
			{
				result.LineTo((float)path[i], (float)path[i + 1]);
			}
			if (close)
			{
				result.ClosePath();
			}

			if (lp != null)
			{
				result = (GeneralPath)lp.mapShape(result);
			}
			return result;
		}

		/// <summary>
		/// Returns a <seealso cref="Shape"/> representing the caret at the specified
		/// hit inside the specified bounds. </summary>
		/// <param name="hit"> the hit at which to generate the caret </param>
		/// <param name="bounds"> the bounds of the <code>TextLayout</code> to use
		///    in generating the caret.  The bounds is in baseline-relative
		///    coordinates. </param>
		/// <returns> a <code>Shape</code> representing the caret.  The returned
		///    shape is in standard coordinates. </returns>
		public Shape GetCaretShape(TextHitInfo hit, Rectangle2D bounds)
		{
			EnsureCache();
			CheckTextHit(hit);

			if (bounds == null)
			{
				throw new IllegalArgumentException("Null Rectangle2D passed to TextLayout.getCaret()");
			}

			return PathToShape(GetCaretPath(hit, bounds), false, TextLine.LayoutPath);
		}

		/// <summary>
		/// Returns a <code>Shape</code> representing the caret at the specified
		/// hit inside the natural bounds of this <code>TextLayout</code>. </summary>
		/// <param name="hit"> the hit at which to generate the caret </param>
		/// <returns> a <code>Shape</code> representing the caret.  The returned
		///     shape is in standard coordinates. </returns>
		public Shape GetCaretShape(TextHitInfo hit)
		{

			return GetCaretShape(hit, NaturalBounds);
		}

		/// <summary>
		/// Return the "stronger" of the TextHitInfos.  The TextHitInfos
		/// should be logical or visual counterparts.  They are not
		/// checked for validity.
		/// </summary>
		private TextHitInfo GetStrongHit(TextHitInfo hit1, TextHitInfo hit2)
		{

			// right now we're using the following rule for strong hits:
			// A hit on a character with a lower level
			// is stronger than one on a character with a higher level.
			// If this rule ties, the hit on the leading edge of a character wins.
			// If THIS rule ties, hit1 wins.  Both rules shouldn't tie, unless the
			// infos aren't counterparts of some sort.

			sbyte hit1Level = GetCharacterLevel(hit1.CharIndex);
			sbyte hit2Level = GetCharacterLevel(hit2.CharIndex);

			if (hit1Level == hit2Level)
			{
				if (hit2.LeadingEdge && !hit1.LeadingEdge)
				{
					return hit2;
				}
				else
				{
					return hit1;
				}
			}
			else
			{
				return (hit1Level < hit2Level)? hit1 : hit2;
			}
		}

		/// <summary>
		/// Returns the level of the character at <code>index</code>.
		/// Indices -1 and <code>characterCount</code> are assigned the base
		/// level of this <code>TextLayout</code>. </summary>
		/// <param name="index"> the index of the character from which to get the level </param>
		/// <returns> the level of the character at the specified index. </returns>
		public sbyte GetCharacterLevel(int index)
		{

			// hmm, allow indices at endpoints?  For now, yes.
			if (index < -1 || index > CharacterCount_Renamed)
			{
				throw new IllegalArgumentException("Index is out of range in getCharacterLevel.");
			}

			EnsureCache();
			if (index == -1 || index == CharacterCount_Renamed)
			{
				 return (sbyte)(TextLine.DirectionLTR? 0 : 1);
			}

			return TextLine.GetCharLevel(index);
		}

		/// <summary>
		/// Returns two paths corresponding to the strong and weak caret. </summary>
		/// <param name="offset"> an offset in this <code>TextLayout</code> </param>
		/// <param name="bounds"> the bounds to which to extend the carets.  The
		/// bounds is in baseline-relative coordinates. </param>
		/// <param name="policy"> the specified <code>CaretPolicy</code> </param>
		/// <returns> an array of two paths.  Element zero is the strong
		/// caret.  If there are two carets, element one is the weak caret,
		/// otherwise it is <code>null</code>. The returned shapes
		/// are in standard coordinates. </returns>
		public Shape[] GetCaretShapes(int offset, Rectangle2D bounds, CaretPolicy policy)
		{

			EnsureCache();

			if (offset < 0 || offset > CharacterCount_Renamed)
			{
				throw new IllegalArgumentException("Offset out of bounds in TextLayout.getCaretShapes()");
			}

			if (bounds == null)
			{
				throw new IllegalArgumentException("Null Rectangle2D passed to TextLayout.getCaretShapes()");
			}

			if (policy == null)
			{
				throw new IllegalArgumentException("Null CaretPolicy passed to TextLayout.getCaretShapes()");
			}

			Shape[] result = new Shape[2];

			TextHitInfo hit = TextHitInfo.AfterOffset(offset);

			int hitCaret = HitToCaret(hit);

			LayoutPathImpl lp = TextLine.LayoutPath;
			Shape hitShape = PathToShape(GetCaretPath(hit, bounds), false, lp);
			TextHitInfo otherHit = hit.OtherHit;
			int otherCaret = HitToCaret(otherHit);

			if (hitCaret == otherCaret)
			{
				result[0] = hitShape;
			}
			else // more than one caret
			{
				Shape otherShape = PathToShape(GetCaretPath(otherHit, bounds), false, lp);

				TextHitInfo strongHit = policy.GetStrongCaret(hit, otherHit, this);
				bool hitIsStrong = strongHit.Equals(hit);

				if (hitIsStrong) // then other is weak
				{
					result[0] = hitShape;
					result[1] = otherShape;
				}
				else
				{
					result[0] = otherShape;
					result[1] = hitShape;
				}
			}

			return result;
		}

		/// <summary>
		/// Returns two paths corresponding to the strong and weak caret.
		/// This method is a convenience overload of <code>getCaretShapes</code>
		/// that uses the default caret policy. </summary>
		/// <param name="offset"> an offset in this <code>TextLayout</code> </param>
		/// <param name="bounds"> the bounds to which to extend the carets.  This is
		///     in baseline-relative coordinates. </param>
		/// <returns> two paths corresponding to the strong and weak caret as
		///    defined by the <code>DEFAULT_CARET_POLICY</code>.  These are
		///    in standard coordinates. </returns>
		public Shape[] GetCaretShapes(int offset, Rectangle2D bounds)
		{
			// {sfb} parameter checking is done in overloaded version
			return GetCaretShapes(offset, bounds, DEFAULT_CARET_POLICY);
		}

		/// <summary>
		/// Returns two paths corresponding to the strong and weak caret.
		/// This method is a convenience overload of <code>getCaretShapes</code>
		/// that uses the default caret policy and this <code>TextLayout</code>
		/// object's natural bounds. </summary>
		/// <param name="offset"> an offset in this <code>TextLayout</code> </param>
		/// <returns> two paths corresponding to the strong and weak caret as
		///    defined by the <code>DEFAULT_CARET_POLICY</code>.  These are
		///    in standard coordinates. </returns>
		public Shape[] GetCaretShapes(int offset)
		{
			// {sfb} parameter checking is done in overloaded version
			return GetCaretShapes(offset, NaturalBounds, DEFAULT_CARET_POLICY);
		}

		// A utility to return a path enclosing the given path
		// Path0 must be left or top of path1
		// {jbr} no assumptions about size of path0, path1 anymore.
		private GeneralPath BoundingShape(double[] path0, double[] path1)
		{

			// Really, we want the path to be a convex hull around all of the
			// points in path0 and path1.  But we can get by with less than
			// that.  We do need to prevent the two segments which
			// join path0 to path1 from crossing each other.  So, if we
			// traverse path0 from top to bottom, we'll traverse path1 from
			// bottom to top (and vice versa).

			GeneralPath result = PathToShape(path0, false, null);

			bool sameDirection;

			if (IsVerticalLine)
			{
				sameDirection = (path0[1] > path0[path0.Length - 1]) == (path1[1] > path1[path1.Length - 1]);
			}
			else
			{
				sameDirection = (path0[0] > path0[path0.Length - 2]) == (path1[0] > path1[path1.Length - 2]);
			}

			int start;
			int limit;
			int increment;

			if (sameDirection)
			{
				start = path1.Length - 2;
				limit = -2;
				increment = -2;
			}
			else
			{
				start = 0;
				limit = path1.Length;
				increment = 2;
			}

			for (int i = start; i != limit; i += increment)
			{
				result.LineTo((float)path1[i], (float)path1[i + 1]);
			}

			result.ClosePath();

			return result;
		}

		// A utility to convert a pair of carets into a bounding path
		// {jbr} Shape is never outside of bounds.
		private GeneralPath CaretBoundingShape(int caret0, int caret1, Rectangle2D bounds)
		{

			if (caret0 > caret1)
			{
				int temp = caret0;
				caret0 = caret1;
				caret1 = temp;
			}

			return BoundingShape(GetCaretPath(caret0, bounds, true), GetCaretPath(caret1, bounds, true));
		}

		/*
		 * A utility to return the path bounding the area to the left (top) of the
		 * layout.
		 * Shape is never outside of bounds.
		 */
		private GeneralPath LeftShape(Rectangle2D bounds)
		{

			double[] path0;
			if (IsVerticalLine)
			{
				path0 = new double[] {bounds.X, bounds.Y, bounds.X + bounds.Width, bounds.Y};
			}
			else
			{
				path0 = new double[] {bounds.X, bounds.Y + bounds.Height, bounds.X, bounds.Y};
			}

			double[] path1 = GetCaretPath(0, bounds, true);

			return BoundingShape(path0, path1);
		}

		/*
		 * A utility to return the path bounding the area to the right (bottom) of
		 * the layout.
		 */
		private GeneralPath RightShape(Rectangle2D bounds)
		{
			double[] path1;
			if (IsVerticalLine)
			{
				path1 = new double[] {bounds.X, bounds.Y + bounds.Height, bounds.X + bounds.Width, bounds.Y + bounds.Height};
			}
			else
			{
				path1 = new double[] {bounds.X + bounds.Width, bounds.Y + bounds.Height, bounds.X + bounds.Width, bounds.Y};
			}

			double[] path0 = GetCaretPath(CharacterCount_Renamed, bounds, true);

			return BoundingShape(path0, path1);
		}

		/// <summary>
		/// Returns the logical ranges of text corresponding to a visual selection. </summary>
		/// <param name="firstEndpoint"> an endpoint of the visual range </param>
		/// <param name="secondEndpoint"> the other endpoint of the visual range.
		/// This endpoint can be less than <code>firstEndpoint</code>. </param>
		/// <returns> an array of integers representing start/limit pairs for the
		/// selected ranges. </returns>
		/// <seealso cref= #getVisualHighlightShape(TextHitInfo, TextHitInfo, Rectangle2D) </seealso>
		public int[] GetLogicalRangesForVisualSelection(TextHitInfo firstEndpoint, TextHitInfo secondEndpoint)
		{
			EnsureCache();

			CheckTextHit(firstEndpoint);
			CheckTextHit(secondEndpoint);

			// !!! probably want to optimize for all LTR text

			bool[] included = new bool[CharacterCount_Renamed];

			int startIndex = HitToCaret(firstEndpoint);
			int limitIndex = HitToCaret(secondEndpoint);

			if (startIndex > limitIndex)
			{
				int t = startIndex;
				startIndex = limitIndex;
				limitIndex = t;
			}

			/*
			 * now we have the visual indexes of the glyphs at the start and limit
			 * of the selection range walk through runs marking characters that
			 * were included in the visual range there is probably a more efficient
			 * way to do this, but this ought to work, so hey
			 */

			if (startIndex < limitIndex)
			{
				int visIndex = startIndex;
				while (visIndex < limitIndex)
				{
					included[TextLine.VisualToLogical(visIndex)] = true;
					++visIndex;
				}
			}

			/*
			 * count how many runs we have, ought to be one or two, but perhaps
			 * things are especially weird
			 */
			int count = 0;
			bool inrun = false;
			for (int i = 0; i < CharacterCount_Renamed; i++)
			{
				if (included[i] != inrun)
				{
					inrun = !inrun;
					if (inrun)
					{
						count++;
					}
				}
			}

			int[] ranges = new int[count * 2];
			count = 0;
			inrun = false;
			for (int i = 0; i < CharacterCount_Renamed; i++)
			{
				if (included[i] != inrun)
				{
					ranges[count++] = i;
					inrun = !inrun;
				}
			}
			if (inrun)
			{
				ranges[count++] = CharacterCount_Renamed;
			}

			return ranges;
		}

		/// <summary>
		/// Returns a path enclosing the visual selection in the specified range,
		/// extended to <code>bounds</code>.
		/// <para>
		/// If the selection includes the leftmost (topmost) position, the selection
		/// is extended to the left (top) of <code>bounds</code>.  If the
		/// selection includes the rightmost (bottommost) position, the selection
		/// is extended to the right (bottom) of the bounds.  The height
		/// (width on vertical lines) of the selection is always extended to
		/// <code>bounds</code>.
		/// </para>
		/// <para>
		/// Although the selection is always contiguous, the logically selected
		/// text can be discontiguous on lines with mixed-direction text.  The
		/// logical ranges of text selected can be retrieved using
		/// <code>getLogicalRangesForVisualSelection</code>.  For example,
		/// consider the text 'ABCdef' where capital letters indicate
		/// right-to-left text, rendered on a right-to-left line, with a visual
		/// selection from 0L (the leading edge of 'A') to 3T (the trailing edge
		/// of 'd').  The text appears as follows, with bold underlined areas
		/// representing the selection:
		/// <br><pre>
		///    d<u><b>efCBA  </b></u>
		/// </pre>
		/// The logical selection ranges are 0-3, 4-6 (ABC, ef) because the
		/// visually contiguous text is logically discontiguous.  Also note that
		/// since the rightmost position on the layout (to the right of 'A') is
		/// selected, the selection is extended to the right of the bounds.
		/// </para>
		/// </summary>
		/// <param name="firstEndpoint"> one end of the visual selection </param>
		/// <param name="secondEndpoint"> the other end of the visual selection </param>
		/// <param name="bounds"> the bounding rectangle to which to extend the selection.
		///     This is in baseline-relative coordinates. </param>
		/// <returns> a <code>Shape</code> enclosing the selection.  This is in
		///     standard coordinates. </returns>
		/// <seealso cref= #getLogicalRangesForVisualSelection(TextHitInfo, TextHitInfo) </seealso>
		/// <seealso cref= #getLogicalHighlightShape(int, int, Rectangle2D) </seealso>
		public Shape GetVisualHighlightShape(TextHitInfo firstEndpoint, TextHitInfo secondEndpoint, Rectangle2D bounds)
		{
			EnsureCache();

			CheckTextHit(firstEndpoint);
			CheckTextHit(secondEndpoint);

			if (bounds == null)
			{
					throw new IllegalArgumentException("Null Rectangle2D passed to TextLayout.getVisualHighlightShape()");
			}

			GeneralPath result = new GeneralPath(GeneralPath.WIND_EVEN_ODD);

			int firstCaret = HitToCaret(firstEndpoint);
			int secondCaret = HitToCaret(secondEndpoint);

			result.Append(CaretBoundingShape(firstCaret, secondCaret, bounds), false);

			if (firstCaret == 0 || secondCaret == 0)
			{
				GeneralPath ls = LeftShape(bounds);
				if (!ls.Bounds.Empty)
				{
					result.Append(ls, false);
				}
			}

			if (firstCaret == CharacterCount_Renamed || secondCaret == CharacterCount_Renamed)
			{
				GeneralPath rs = RightShape(bounds);
				if (!rs.Bounds.Empty)
				{
					result.Append(rs, false);
				}
			}

			LayoutPathImpl lp = TextLine.LayoutPath;
			if (lp != null)
			{
				result = (GeneralPath)lp.mapShape(result); // dlf cast safe?
			}

			return result;
		}

		/// <summary>
		/// Returns a <code>Shape</code> enclosing the visual selection in the
		/// specified range, extended to the bounds.  This method is a
		/// convenience overload of <code>getVisualHighlightShape</code> that
		/// uses the natural bounds of this <code>TextLayout</code>. </summary>
		/// <param name="firstEndpoint"> one end of the visual selection </param>
		/// <param name="secondEndpoint"> the other end of the visual selection </param>
		/// <returns> a <code>Shape</code> enclosing the selection.  This is
		///     in standard coordinates. </returns>
		public Shape GetVisualHighlightShape(TextHitInfo firstEndpoint, TextHitInfo secondEndpoint)
		{
			return GetVisualHighlightShape(firstEndpoint, secondEndpoint, NaturalBounds);
		}

		/// <summary>
		/// Returns a <code>Shape</code> enclosing the logical selection in the
		/// specified range, extended to the specified <code>bounds</code>.
		/// <para>
		/// If the selection range includes the first logical character, the
		/// selection is extended to the portion of <code>bounds</code> before
		/// the start of this <code>TextLayout</code>.  If the range includes
		/// the last logical character, the selection is extended to the portion
		/// of <code>bounds</code> after the end of this <code>TextLayout</code>.
		/// The height (width on vertical lines) of the selection is always
		/// extended to <code>bounds</code>.
		/// </para>
		/// <para>
		/// The selection can be discontiguous on lines with mixed-direction text.
		/// Only those characters in the logical range between start and limit
		/// appear selected.  For example, consider the text 'ABCdef' where capital
		/// letters indicate right-to-left text, rendered on a right-to-left line,
		/// with a logical selection from 0 to 4 ('ABCd').  The text appears as
		/// follows, with bold standing in for the selection, and underlining for
		/// the extension:
		/// <br><pre>
		///    <u><b>d</b></u>ef<u><b>CBA  </b></u>
		/// </pre>
		/// The selection is discontiguous because the selected characters are
		/// visually discontiguous. Also note that since the range includes the
		/// first logical character (A), the selection is extended to the portion
		/// of the <code>bounds</code> before the start of the layout, which in
		/// this case (a right-to-left line) is the right portion of the
		/// <code>bounds</code>.
		/// </para>
		/// </summary>
		/// <param name="firstEndpoint"> an endpoint in the range of characters to select </param>
		/// <param name="secondEndpoint"> the other endpoint of the range of characters
		/// to select. Can be less than <code>firstEndpoint</code>.  The range
		/// includes the character at min(firstEndpoint, secondEndpoint), but
		/// excludes max(firstEndpoint, secondEndpoint). </param>
		/// <param name="bounds"> the bounding rectangle to which to extend the selection.
		///     This is in baseline-relative coordinates. </param>
		/// <returns> an area enclosing the selection.  This is in standard
		///     coordinates. </returns>
		/// <seealso cref= #getVisualHighlightShape(TextHitInfo, TextHitInfo, Rectangle2D) </seealso>
		public Shape GetLogicalHighlightShape(int firstEndpoint, int secondEndpoint, Rectangle2D bounds)
		{
			if (bounds == null)
			{
				throw new IllegalArgumentException("Null Rectangle2D passed to TextLayout.getLogicalHighlightShape()");
			}

			EnsureCache();

			if (firstEndpoint > secondEndpoint)
			{
				int t = firstEndpoint;
				firstEndpoint = secondEndpoint;
				secondEndpoint = t;
			}

			if (firstEndpoint < 0 || secondEndpoint > CharacterCount_Renamed)
			{
				throw new IllegalArgumentException("Range is invalid in TextLayout.getLogicalHighlightShape()");
			}

			GeneralPath result = new GeneralPath(GeneralPath.WIND_EVEN_ODD);

			int[] carets = new int[10]; // would this ever not handle all cases?
			int count = 0;

			if (firstEndpoint < secondEndpoint)
			{
				int logIndex = firstEndpoint;
				do
				{
					carets[count++] = HitToCaret(TextHitInfo.Leading(logIndex));
					bool ltr = TextLine.IsCharLTR(logIndex);

					do
					{
						logIndex++;
					} while (logIndex < secondEndpoint && TextLine.IsCharLTR(logIndex) == ltr);

					int hitCh = logIndex;
					carets[count++] = HitToCaret(TextHitInfo.Trailing(hitCh - 1));

					if (count == carets.Length)
					{
						int[] temp = new int[carets.Length + 10];
						System.Array.Copy(carets, 0, temp, 0, count);
						carets = temp;
					}
				} while (logIndex < secondEndpoint);
			}
			else
			{
				count = 2;
				carets[0] = carets[1] = HitToCaret(TextHitInfo.Leading(firstEndpoint));
			}

			// now create paths for pairs of carets

			for (int i = 0; i < count; i += 2)
			{
				result.Append(CaretBoundingShape(carets[i], carets[i + 1], bounds), false);
			}

			if (firstEndpoint != secondEndpoint)
			{
				if ((TextLine.DirectionLTR && firstEndpoint == 0) || (!TextLine.DirectionLTR && secondEndpoint == CharacterCount_Renamed))
				{
					GeneralPath ls = LeftShape(bounds);
					if (!ls.Bounds.Empty)
					{
						result.Append(ls, false);
					}
				}

				if ((TextLine.DirectionLTR && secondEndpoint == CharacterCount_Renamed) || (!TextLine.DirectionLTR && firstEndpoint == 0))
				{

					GeneralPath rs = RightShape(bounds);
					if (!rs.Bounds.Empty)
					{
						result.Append(rs, false);
					}
				}
			}

			LayoutPathImpl lp = TextLine.LayoutPath;
			if (lp != null)
			{
				result = (GeneralPath)lp.mapShape(result); // dlf cast safe?
			}
			return result;
		}

		/// <summary>
		/// Returns a <code>Shape</code> enclosing the logical selection in the
		/// specified range, extended to the natural bounds of this
		/// <code>TextLayout</code>.  This method is a convenience overload of
		/// <code>getLogicalHighlightShape</code> that uses the natural bounds of
		/// this <code>TextLayout</code>. </summary>
		/// <param name="firstEndpoint"> an endpoint in the range of characters to select </param>
		/// <param name="secondEndpoint"> the other endpoint of the range of characters
		/// to select. Can be less than <code>firstEndpoint</code>.  The range
		/// includes the character at min(firstEndpoint, secondEndpoint), but
		/// excludes max(firstEndpoint, secondEndpoint). </param>
		/// <returns> a <code>Shape</code> enclosing the selection.  This is in
		///     standard coordinates. </returns>
		public Shape GetLogicalHighlightShape(int firstEndpoint, int secondEndpoint)
		{

			return GetLogicalHighlightShape(firstEndpoint, secondEndpoint, NaturalBounds);
		}

		/// <summary>
		/// Returns the black box bounds of the characters in the specified range.
		/// The black box bounds is an area consisting of the union of the bounding
		/// boxes of all the glyphs corresponding to the characters between start
		/// and limit.  This area can be disjoint. </summary>
		/// <param name="firstEndpoint"> one end of the character range </param>
		/// <param name="secondEndpoint"> the other end of the character range.  Can be
		/// less than <code>firstEndpoint</code>. </param>
		/// <returns> a <code>Shape</code> enclosing the black box bounds.  This is
		///     in standard coordinates. </returns>
		public Shape GetBlackBoxBounds(int firstEndpoint, int secondEndpoint)
		{
			EnsureCache();

			if (firstEndpoint > secondEndpoint)
			{
				int t = firstEndpoint;
				firstEndpoint = secondEndpoint;
				secondEndpoint = t;
			}

			if (firstEndpoint < 0 || secondEndpoint > CharacterCount_Renamed)
			{
				throw new IllegalArgumentException("Invalid range passed to TextLayout.getBlackBoxBounds()");
			}

			/*
			 * return an area that consists of the bounding boxes of all the
			 * characters from firstEndpoint to limit
			 */

			GeneralPath result = new GeneralPath(GeneralPath.WIND_NON_ZERO);

			if (firstEndpoint < CharacterCount_Renamed)
			{
				for (int logIndex = firstEndpoint; logIndex < secondEndpoint; logIndex++)
				{

					Rectangle2D r = TextLine.GetCharBounds(logIndex);
					if (!r.Empty)
					{
						result.Append(r, false);
					}
				}
			}

			if (Dx != 0 || Dy != 0)
			{
				AffineTransform tx = AffineTransform.GetTranslateInstance(Dx, Dy);
				result = (GeneralPath)tx.CreateTransformedShape(result);
			}
			LayoutPathImpl lp = TextLine.LayoutPath;
			if (lp != null)
			{
				result = (GeneralPath)lp.mapShape(result);
			}

			//return new Highlight(result, false);
			return result;
		}

		/// <summary>
		/// Returns the distance from the point (x,&nbsp;y) to the caret along
		/// the line direction defined in <code>caretInfo</code>.  Distance is
		/// negative if the point is to the left of the caret on a horizontal
		/// line, or above the caret on a vertical line.
		/// Utility for use by hitTestChar.
		/// </summary>
		private float CaretToPointDistance(float[] caretInfo, float x, float y)
		{
			// distanceOffBaseline is negative if you're 'above' baseline

			float lineDistance = IsVerticalLine? y : x;
			float distanceOffBaseline = IsVerticalLine? - x : y;

			return lineDistance - caretInfo[0] + (distanceOffBaseline * caretInfo[1]);
		}

		/// <summary>
		/// Returns a <code>TextHitInfo</code> corresponding to the
		/// specified point.
		/// Coordinates outside the bounds of the <code>TextLayout</code>
		/// map to hits on the leading edge of the first logical character,
		/// or the trailing edge of the last logical character, as appropriate,
		/// regardless of the position of that character in the line.  Only the
		/// direction along the baseline is used to make this evaluation. </summary>
		/// <param name="x"> the x offset from the origin of this
		///     <code>TextLayout</code>.  This is in standard coordinates. </param>
		/// <param name="y"> the y offset from the origin of this
		///     <code>TextLayout</code>.  This is in standard coordinates. </param>
		/// <param name="bounds"> the bounds of the <code>TextLayout</code>.  This
		///     is in baseline-relative coordinates. </param>
		/// <returns> a hit describing the character and edge (leading or trailing)
		///     under the specified point. </returns>
		public TextHitInfo HitTestChar(float x, float y, Rectangle2D bounds)
		{
			// check boundary conditions

			LayoutPathImpl lp = TextLine.LayoutPath;
			bool prev = false;
			if (lp != null)
			{
				Point2D.Float pt = new Point2D.Float(x, y);
				prev = lp.pointToPath(pt, pt);
				x = pt.x;
				y = pt.y;
			}

			if (Vertical)
			{
				if (y < bounds.MinY)
				{
					return TextHitInfo.Leading(0);
				}
				else if (y >= bounds.MaxY)
				{
					return TextHitInfo.Trailing(CharacterCount_Renamed - 1);
				}
			}
			else
			{
				if (x < bounds.MinX)
				{
					return LeftToRight ? TextHitInfo.Leading(0) : TextHitInfo.Trailing(CharacterCount_Renamed - 1);
				}
				else if (x >= bounds.MaxX)
				{
					return LeftToRight ? TextHitInfo.Trailing(CharacterCount_Renamed - 1) : TextHitInfo.Leading(0);
				}
			}

			// revised hit test
			// the original seems too complex and fails miserably with italic offsets
			// the natural tendency is to move towards the character you want to hit
			// so we'll just measure distance to the center of each character's visual
			// bounds, pick the closest one, then see which side of the character's
			// center line (italic) the point is on.
			// this tends to make it easier to hit narrow characters, which can be a
			// bit odd if you're visually over an adjacent wide character. this makes
			// a difference with bidi, so perhaps i need to revisit this yet again.

			double distance = Double.MaxValue;
			int index = 0;
			int trail = -1;
			CoreMetrics lcm = null;
			float icx = 0, icy = 0, ia = 0, cy = 0, dya = 0, ydsq = 0;

			for (int i = 0; i < CharacterCount_Renamed; ++i)
			{
				if (!TextLine.CaretAtOffsetIsValid(i))
				{
					continue;
				}
				if (trail == -1)
				{
					trail = i;
				}
				CoreMetrics cm = TextLine.GetCoreMetricsAt(i);
				if (cm != lcm)
				{
					lcm = cm;
					// just work around baseline mess for now
					if (cm.baselineIndex == GraphicAttribute.TOP_ALIGNMENT)
					{
						cy = -(TextLine.Metrics.Ascent - cm.ascent) + cm.ssOffset;
					}
					else if (cm.baselineIndex == GraphicAttribute.BOTTOM_ALIGNMENT)
					{
						cy = TextLine.Metrics.Descent - cm.descent + cm.ssOffset;
					}
					else
					{
						cy = cm.effectiveBaselineOffset(BaselineOffsets_Renamed) + cm.ssOffset;
					}
					float dy = (cm.descent - cm.ascent) / 2 - cy;
					dya = dy * cm.italicAngle;
					cy += dy;
					ydsq = (cy - y) * (cy - y);
				}
				float cx = TextLine.GetCharXPosition(i);
				float ca = TextLine.GetCharAdvance(i);
				float dx = ca / 2;
				cx += dx - dya;

				// proximity in x (along baseline) is two times as important as proximity in y
				double nd = System.Math.Sqrt(4 * (cx - x) * (cx - x) + ydsq);
				if (nd < distance)
				{
					distance = nd;
					index = i;
					trail = -1;
					icx = cx;
					icy = cy;
					ia = cm.italicAngle;
				}
			}
			bool left = x < icx - (y - icy) * ia;
			bool leading = TextLine.IsCharLTR(index) == left;
			if (trail == -1)
			{
				trail = CharacterCount_Renamed;
			}
			TextHitInfo result = leading ? TextHitInfo.Leading(index) : TextHitInfo.Trailing(trail - 1);
			return result;
		}

		/// <summary>
		/// Returns a <code>TextHitInfo</code> corresponding to the
		/// specified point.  This method is a convenience overload of
		/// <code>hitTestChar</code> that uses the natural bounds of this
		/// <code>TextLayout</code>. </summary>
		/// <param name="x"> the x offset from the origin of this
		///     <code>TextLayout</code>.  This is in standard coordinates. </param>
		/// <param name="y"> the y offset from the origin of this
		///     <code>TextLayout</code>.  This is in standard coordinates. </param>
		/// <returns> a hit describing the character and edge (leading or trailing)
		/// under the specified point. </returns>
		public TextHitInfo HitTestChar(float x, float y)
		{

			return HitTestChar(x, y, NaturalBounds);
		}

		/// <summary>
		/// Returns the hash code of this <code>TextLayout</code>. </summary>
		/// <returns> the hash code of this <code>TextLayout</code>. </returns>
		public override int HashCode()
		{
			if (HashCodeCache == 0)
			{
				EnsureCache();
				HashCodeCache = TextLine.HashCode();
			}
			return HashCodeCache;
		}

		/// <summary>
		/// Returns <code>true</code> if the specified <code>Object</code> is a
		/// <code>TextLayout</code> object and if the specified <code>Object</code>
		/// equals this <code>TextLayout</code>. </summary>
		/// <param name="obj"> an <code>Object</code> to test for equality </param>
		/// <returns> <code>true</code> if the specified <code>Object</code>
		///      equals this <code>TextLayout</code>; <code>false</code>
		///      otherwise. </returns>
		public override bool Equals(Object obj)
		{
			return (obj is TextLayout) && Equals((TextLayout)obj);
		}

		/// <summary>
		/// Returns <code>true</code> if the two layouts are equal.
		/// Two layouts are equal if they contain equal glyphvectors in the same order. </summary>
		/// <param name="rhs"> the <code>TextLayout</code> to compare to this
		///       <code>TextLayout</code> </param>
		/// <returns> <code>true</code> if the specified <code>TextLayout</code>
		///      equals this <code>TextLayout</code>.
		///  </returns>
		public bool Equals(TextLayout rhs)
		{

			if (rhs == null)
			{
				return false;
			}
			if (rhs == this)
			{
				return true;
			}

			EnsureCache();
			return TextLine.Equals(rhs.TextLine);
		}

		/// <summary>
		/// Returns debugging information for this <code>TextLayout</code>. </summary>
		/// <returns> the <code>textLine</code> of this <code>TextLayout</code>
		///        as a <code>String</code>. </returns>
		public override String ToString()
		{
			EnsureCache();
			return TextLine.ToString();
		}

		/// <summary>
		/// Renders this <code>TextLayout</code> at the specified location in
		/// the specified <seealso cref="java.awt.Graphics2D Graphics2D"/> context.
		/// The origin of the layout is placed at x,&nbsp;y.  Rendering may touch
		/// any point within <code>getBounds()</code> of this position.  This
		/// leaves the <code>g2</code> unchanged.  Text is rendered along the
		/// baseline path. </summary>
		/// <param name="g2"> the <code>Graphics2D</code> context into which to render
		///         the layout </param>
		/// <param name="x"> the X coordinate of the origin of this <code>TextLayout</code> </param>
		/// <param name="y"> the Y coordinate of the origin of this <code>TextLayout</code> </param>
		/// <seealso cref= #getBounds() </seealso>
		public void Draw(Graphics2D g2, float x, float y)
		{

			if (g2 == null)
			{
				throw new IllegalArgumentException("Null Graphics2D passed to TextLayout.draw()");
			}

			TextLine.Draw(g2, x - Dx, y - Dy);
		}

		/// <summary>
		/// Package-only method for testing ONLY.  Please don't abuse.
		/// </summary>
		internal TextLine TextLineForTesting
		{
			get
			{
    
				return TextLine;
			}
		}

		/// 
		/// <summary>
		/// Return the index of the first character with a different baseline from the
		/// character at start, or limit if all characters between start and limit have
		/// the same baseline.
		/// </summary>
		private static int SameBaselineUpTo(Font font, char[] text, int start, int limit)
		{
			// current implementation doesn't support multiple baselines
			return limit;
			/*
			byte bl = font.getBaselineFor(text[start++]);
			while (start < limit && font.getBaselineFor(text[start]) == bl) {
			    ++start;
			}
			return start;
			*/
		}

		internal static sbyte GetBaselineFromGraphic(GraphicAttribute graphic)
		{

			sbyte alignment = (sbyte) graphic.Alignment;

			if (alignment == GraphicAttribute.BOTTOM_ALIGNMENT || alignment == GraphicAttribute.TOP_ALIGNMENT)
			{

				return (sbyte)GraphicAttribute.ROMAN_BASELINE;
			}
			else
			{
				return alignment;
			}
		}

		/// <summary>
		/// Returns a <code>Shape</code> representing the outline of this
		/// <code>TextLayout</code>. </summary>
		/// <param name="tx"> an optional <seealso cref="AffineTransform"/> to apply to the
		///     outline of this <code>TextLayout</code>. </param>
		/// <returns> a <code>Shape</code> that is the outline of this
		///     <code>TextLayout</code>.  This is in standard coordinates. </returns>
		public Shape GetOutline(AffineTransform tx)
		{
			EnsureCache();
			Shape result = TextLine.GetOutline(tx);
			LayoutPathImpl lp = TextLine.LayoutPath;
			if (lp != null)
			{
				result = lp.mapShape(result);
			}
			return result;
		}

		/// <summary>
		/// Return the LayoutPath, or null if the layout path is the
		/// default path (x maps to advance, y maps to offset). </summary>
		/// <returns> the layout path
		/// @since 1.6 </returns>
		public LayoutPath LayoutPath
		{
			get
			{
				return TextLine.LayoutPath;
			}
		}

	   /// <summary>
	   /// Convert a hit to a point in standard coordinates.  The point is
	   /// on the baseline of the character at the leading or trailing
	   /// edge of the character, as appropriate.  If the path is
	   /// broken at the side of the character represented by the hit, the
	   /// point will be adjacent to the character. </summary>
	   /// <param name="hit"> the hit to check.  This must be a valid hit on
	   /// the TextLayout. </param>
	   /// <param name="point"> the returned point. The point is in standard
	   ///     coordinates. </param>
	   /// <exception cref="IllegalArgumentException"> if the hit is not valid for the
	   /// TextLayout. </exception>
	   /// <exception cref="NullPointerException"> if hit or point is null.
	   /// @since 1.6 </exception>
		public void HitToPoint(TextHitInfo hit, Point2D point)
		{
			if (hit == null || point == null)
			{
				throw new NullPointerException((hit == null ? "hit" : "point") + " can't be null");
			}
			EnsureCache();
			CheckTextHit(hit);

			float adv = 0;
			float off = 0;

			int ix = hit.CharIndex;
			bool leading = hit.LeadingEdge;
			bool ltr;
			if (ix == -1 || ix == TextLine.CharacterCount())
			{
				ltr = TextLine.DirectionLTR;
				adv = (ltr == (ix == -1)) ? 0 : LineMetrics.Advance;
			}
			else
			{
				ltr = TextLine.IsCharLTR(ix);
				adv = TextLine.GetCharLinePosition(ix, leading);
				off = TextLine.GetCharYPosition(ix);
			}
			point.SetLocation(adv, off);
			LayoutPath lp = TextLine.LayoutPath;
			if (lp != null)
			{
				lp.PathToPoint(point, ltr != leading, point);
			}
		}
	}

}
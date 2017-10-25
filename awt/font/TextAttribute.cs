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
	/// The <code>TextAttribute</code> class defines attribute keys and
	/// attribute values used for text rendering.
	/// <para>
	/// <code>TextAttribute</code> instances are used as attribute keys to
	/// identify attributes in
	/// <seealso cref="java.awt.Font Font"/>,
	/// <seealso cref="java.awt.font.TextLayout TextLayout"/>,
	/// <seealso cref="java.text.AttributedCharacterIterator AttributedCharacterIterator"/>,
	/// and other classes handling text attributes. Other constants defined
	/// in this class can be used as attribute values.
	/// </para>
	/// <para>
	/// For each text attribute, the documentation provides:
	/// <UL>
	///   <LI>the type of its value,
	///   <LI>the relevant predefined constants, if any
	///   <LI>the default effect if the attribute is absent
	///   <LI>the valid values if there are limitations
	///   <LI>a description of the effect.
	/// </UL>
	/// </para>
	/// <para>
	/// <H3>Values</H3>
	/// <UL>
	///   <LI>The values of attributes must always be immutable.
	///   <LI>Where value limitations are given, any value outside of that
	///   set is reserved for future use; the value will be treated as
	///   the default.
	///   <LI>The value <code>null</code> is treated the same as the
	///   default value and results in the default behavior.
	///   <li>If the value is not of the proper type, the attribute
	///   will be ignored.
	///   <li>The identity of the value does not matter, only the actual
	///   value.  For example, <code>TextAttribute.WEIGHT_BOLD</code> and
	///   <code>new Float(2.0)</code>
	///   indicate the same <code>WEIGHT</code>.
	///   <li>Attribute values of type <code>Number</code> (used for
	///   <code>WEIGHT</code>, <code>WIDTH</code>, <code>POSTURE</code>,
	///   <code>SIZE</code>, <code>JUSTIFICATION</code>, and
	///   <code>TRACKING</code>) can vary along their natural range and are
	///   not restricted to the predefined constants.
	///   <code>Number.floatValue()</code> is used to get the actual value
	///   from the <code>Number</code>.
	///   <li>The values for <code>WEIGHT</code>, <code>WIDTH</code>, and
	///   <code>POSTURE</code> are interpolated by the system, which
	///   can select the 'nearest available' font or use other techniques to
	///   approximate the user's request.
	/// 
	/// </UL>
	/// 
	/// <h4>Summary of attributes</h4>
	/// </para>
	/// <para>
	/// <table style="float:center" border="0" cellspacing="0" cellpadding="2" width="%95"
	///     summary="Key, value type, principal constants, and default value
	///     behavior of all TextAttributes">
	/// <tr style="background-color:#ccccff">
	/// <th valign="TOP" align="CENTER">Key</th>
	/// <th valign="TOP" align="CENTER">Value Type</th>
	/// <th valign="TOP" align="CENTER">Principal Constants</th>
	/// <th valign="TOP" align="CENTER">Default Value</th>
	/// </tr>
	/// <tr>
	/// <td valign="TOP"><seealso cref="#FAMILY"/></td>
	/// <td valign="TOP">String</td>
	/// <td valign="TOP">See Font <seealso cref="java.awt.Font#DIALOG DIALOG"/>,
	/// <seealso cref="java.awt.Font#DIALOG_INPUT DIALOG_INPUT"/>,<br> <seealso cref="java.awt.Font#SERIF SERIF"/>,
	/// <seealso cref="java.awt.Font#SANS_SERIF SANS_SERIF"/>, and <seealso cref="java.awt.Font#MONOSPACED MONOSPACED"/>.
	/// </td>
	/// <td valign="TOP">"Default" (use platform default)</td>
	/// </tr>
	/// <tr style="background-color:#eeeeff">
	/// <td valign="TOP"><seealso cref="#WEIGHT"/></td>
	/// <td valign="TOP">Number</td>
	/// <td valign="TOP">WEIGHT_REGULAR, WEIGHT_BOLD</td>
	/// <td valign="TOP">WEIGHT_REGULAR</td>
	/// </tr>
	/// <tr>
	/// <td valign="TOP"><seealso cref="#WIDTH"/></td>
	/// <td valign="TOP">Number</td>
	/// <td valign="TOP">WIDTH_CONDENSED, WIDTH_REGULAR,<br>WIDTH_EXTENDED</td>
	/// <td valign="TOP">WIDTH_REGULAR</td>
	/// </tr>
	/// <tr style="background-color:#eeeeff">
	/// <td valign="TOP"><seealso cref="#POSTURE"/></td>
	/// <td valign="TOP">Number</td>
	/// <td valign="TOP">POSTURE_REGULAR, POSTURE_OBLIQUE</td>
	/// <td valign="TOP">POSTURE_REGULAR</td>
	/// </tr>
	/// <tr>
	/// <td valign="TOP"><seealso cref="#SIZE"/></td>
	/// <td valign="TOP">Number</td>
	/// <td valign="TOP">none</td>
	/// <td valign="TOP">12.0</td>
	/// </tr>
	/// <tr style="background-color:#eeeeff">
	/// <td valign="TOP"><seealso cref="#TRANSFORM"/></td>
	/// <td valign="TOP"><seealso cref="TransformAttribute"/></td>
	/// <td valign="TOP">See TransformAttribute <seealso cref="TransformAttribute#IDENTITY IDENTITY"/></td>
	/// <td valign="TOP">TransformAttribute.IDENTITY</td>
	/// </tr>
	/// <tr>
	/// <td valign="TOP"><seealso cref="#SUPERSCRIPT"/></td>
	/// <td valign="TOP">Integer</td>
	/// <td valign="TOP">SUPERSCRIPT_SUPER, SUPERSCRIPT_SUB</td>
	/// <td valign="TOP">0 (use the standard glyphs and metrics)</td>
	/// </tr>
	/// <tr style="background-color:#eeeeff">
	/// <td valign="TOP"><seealso cref="#FONT"/></td>
	/// <td valign="TOP"><seealso cref="java.awt.Font"/></td>
	/// <td valign="TOP">none</td>
	/// <td valign="TOP">null (do not override font resolution)</td>
	/// </tr>
	/// <tr>
	/// <td valign="TOP"><seealso cref="#CHAR_REPLACEMENT"/></td>
	/// <td valign="TOP"><seealso cref="GraphicAttribute"/></td>
	/// <td valign="TOP">none</td>
	/// <td valign="TOP">null (draw text using font glyphs)</td>
	/// </tr>
	/// <tr style="background-color:#eeeeff">
	/// <td valign="TOP"><seealso cref="#FOREGROUND"/></td>
	/// <td valign="TOP"><seealso cref="java.awt.Paint"/></td>
	/// <td valign="TOP">none</td>
	/// <td valign="TOP">null (use current graphics paint)</td>
	/// </tr>
	/// <tr>
	/// <td valign="TOP"><seealso cref="#BACKGROUND"/></td>
	/// <td valign="TOP"><seealso cref="java.awt.Paint"/></td>
	/// <td valign="TOP">none</td>
	/// <td valign="TOP">null (do not render background)</td>
	/// </tr>
	/// <tr style="background-color:#eeeeff">
	/// <td valign="TOP"><seealso cref="#UNDERLINE"/></td>
	/// <td valign="TOP">Integer</td>
	/// <td valign="TOP">UNDERLINE_ON</td>
	/// <td valign="TOP">-1 (do not render underline)</td>
	/// </tr>
	/// <tr>
	/// <td valign="TOP"><seealso cref="#STRIKETHROUGH"/></td>
	/// <td valign="TOP">Boolean</td>
	/// <td valign="TOP">STRIKETHROUGH_ON</td>
	/// <td valign="TOP">false (do not render strikethrough)</td>
	/// </tr>
	/// <tr style="background-color:#eeeeff">
	/// <td valign="TOP"><seealso cref="#RUN_DIRECTION"/></td>
	/// <td valign="TOP">Boolean</td>
	/// <td valign="TOP">RUN_DIRECTION_LTR<br>RUN_DIRECTION_RTL</td>
	/// <td valign="TOP">null (use <seealso cref="java.text.Bidi"/> standard default)</td>
	/// </tr>
	/// <tr>
	/// <td valign="TOP"><seealso cref="#BIDI_EMBEDDING"/></td>
	/// <td valign="TOP">Integer</td>
	/// <td valign="TOP">none</td>
	/// <td valign="TOP">0 (use base line direction)</td>
	/// </tr>
	/// <tr style="background-color:#eeeeff">
	/// <td valign="TOP"><seealso cref="#JUSTIFICATION"/></td>
	/// <td valign="TOP">Number</td>
	/// <td valign="TOP">JUSTIFICATION_FULL</td>
	/// <td valign="TOP">JUSTIFICATION_FULL</td>
	/// </tr>
	/// <tr>
	/// <td valign="TOP"><seealso cref="#INPUT_METHOD_HIGHLIGHT"/></td>
	/// <td valign="TOP"><seealso cref="java.awt.im.InputMethodHighlight"/>,<br><seealso cref="java.text.Annotation"/></td>
	/// <td valign="TOP">(see class)</td>
	/// <td valign="TOP">null (do not apply input highlighting)</td>
	/// </tr>
	/// <tr style="background-color:#eeeeff">
	/// <td valign="TOP"><seealso cref="#INPUT_METHOD_UNDERLINE"/></td>
	/// <td valign="TOP">Integer</td>
	/// <td valign="TOP">UNDERLINE_LOW_ONE_PIXEL,<br>UNDERLINE_LOW_TWO_PIXEL</td>
	/// <td valign="TOP">-1 (do not render underline)</td>
	/// </tr>
	/// <tr>
	/// <td valign="TOP"><seealso cref="#SWAP_COLORS"/></td>
	/// <td valign="TOP">Boolean</td>
	/// <td valign="TOP">SWAP_COLORS_ON</td>
	/// <td valign="TOP">false (do not swap colors)</td>
	/// </tr>
	/// <tr style="background-color:#eeeeff">
	/// <td valign="TOP"><seealso cref="#NUMERIC_SHAPING"/></td>
	/// <td valign="TOP"><seealso cref="java.awt.font.NumericShaper"/></td>
	/// <td valign="TOP">none</td>
	/// <td valign="TOP">null (do not shape digits)</td>
	/// </tr>
	/// <tr>
	/// <td valign="TOP"><seealso cref="#KERNING"/></td>
	/// <td valign="TOP">Integer</td>
	/// <td valign="TOP">KERNING_ON</td>
	/// <td valign="TOP">0 (do not request kerning)</td>
	/// </tr>
	/// <tr style="background-color:#eeeeff">
	/// <td valign="TOP"><seealso cref="#LIGATURES"/></td>
	/// <td valign="TOP">Integer</td>
	/// <td valign="TOP">LIGATURES_ON</td>
	/// <td valign="TOP">0 (do not form optional ligatures)</td>
	/// </tr>
	/// <tr>
	/// <td valign="TOP"><seealso cref="#TRACKING"/></td>
	/// <td valign="TOP">Number</td>
	/// <td valign="TOP">TRACKING_LOOSE, TRACKING_TIGHT</td>
	/// <td valign="TOP">0 (do not add tracking)</td>
	/// </tr>
	/// </table>
	/// 
	/// </para>
	/// </summary>
	/// <seealso cref= java.awt.Font </seealso>
	/// <seealso cref= java.awt.font.TextLayout </seealso>
	/// <seealso cref= java.text.AttributedCharacterIterator </seealso>
	public sealed class TextAttribute : AttributedCharacterIterator_Attribute
	{

		// table of all instances in this class, used by readResolve
		private new static readonly IDictionary<String, TextAttribute> InstanceMap = new Dictionary<String, TextAttribute>(29);

		/// <summary>
		/// Constructs a <code>TextAttribute</code> with the specified name. </summary>
		/// <param name="name"> the attribute name to assign to this
		/// <code>TextAttribute</code> </param>
		protected internal TextAttribute(String name) : base(name)
		{
			if (this.GetType() == typeof(TextAttribute))
			{
				InstanceMap[name] = this;
			}
		}

		/// <summary>
		/// Resolves instances being deserialized to the predefined constants.
		/// </summary>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: protected Object readResolve() throws java.io.InvalidObjectException
		protected internal override Object ReadResolve()
		{
			if (this.GetType() != typeof(TextAttribute))
			{
				throw new InvalidObjectException("subclass didn't correctly implement readResolve");
			}

			TextAttribute instance = InstanceMap[Name];
			if (instance != null)
			{
				return instance;
			}
			else
			{
				throw new InvalidObjectException("unknown attribute name");
			}
		}

		// Serialization compatibility with Java 2 platform v1.2.
		// 1.2 will throw an InvalidObjectException if ever asked to
		// deserialize INPUT_METHOD_UNDERLINE.
		// This shouldn't happen in real life.
		internal new const long SerialVersionUID = 7744112784117861702L;

		//
		// For use with Font.
		//

		/// <summary>
		/// Attribute key for the font name.  Values are instances of
		/// <b><code>String</code></b>.  The default value is
		/// <code>"Default"</code>, which causes the platform default font
		/// family to be used.
		/// 
		/// <para> The <code>Font</code> class defines constants for the logical
		/// font names
		/// <seealso cref="java.awt.Font#DIALOG DIALOG"/>,
		/// <seealso cref="java.awt.Font#DIALOG_INPUT DIALOG_INPUT"/>,
		/// <seealso cref="java.awt.Font#SANS_SERIF SANS_SERIF"/>,
		/// <seealso cref="java.awt.Font#SERIF SERIF"/>, and
		/// <seealso cref="java.awt.Font#MONOSPACED MONOSPACED"/>.
		/// 
		/// </para>
		/// <para>This defines the value passed as <code>name</code> to the
		/// <code>Font</code> constructor.  Both logical and physical
		/// font names are allowed. If a font with the requested name
		/// is not found, the default font is used.
		/// 
		/// </para>
		/// <para><em>Note:</em> This attribute is unfortunately misnamed, as
		/// it specifies the face name and not just the family.  Thus
		/// values such as "Lucida Sans Bold" will select that face if it
		/// exists.  Note, though, that if the requested face does not
		/// exist, the default will be used with <em>regular</em> weight.
		/// The "Bold" in the name is part of the face name, not a separate
		/// request that the font's weight be bold.</para>
		/// </summary>
		public static readonly TextAttribute FAMILY = new TextAttribute("family");

		/// <summary>
		/// Attribute key for the weight of a font.  Values are instances
		/// of <b><code>Number</code></b>.  The default value is
		/// <code>WEIGHT_REGULAR</code>.
		/// 
		/// <para>Several constant values are provided, see {@link
		/// #WEIGHT_EXTRA_LIGHT}, <seealso cref="#WEIGHT_LIGHT"/>, {@link
		/// #WEIGHT_DEMILIGHT}, <seealso cref="#WEIGHT_REGULAR"/>, {@link
		/// #WEIGHT_SEMIBOLD}, <seealso cref="#WEIGHT_MEDIUM"/>, {@link
		/// #WEIGHT_DEMIBOLD}, <seealso cref="#WEIGHT_BOLD"/>, <seealso cref="#WEIGHT_HEAVY"/>,
		/// <seealso cref="#WEIGHT_EXTRABOLD"/>, and <seealso cref="#WEIGHT_ULTRABOLD"/>.  The
		/// value <code>WEIGHT_BOLD</code> corresponds to the
		/// style value <code>Font.BOLD</code> as passed to the
		/// <code>Font</code> constructor.
		/// 
		/// </para>
		/// <para>The value is roughly the ratio of the stem width to that of
		/// the regular weight.
		/// 
		/// </para>
		/// <para>The system can interpolate the provided value.
		/// </para>
		/// </summary>
		public static readonly TextAttribute WEIGHT = new TextAttribute("weight");

		/// <summary>
		/// The lightest predefined weight. </summary>
		/// <seealso cref= #WEIGHT </seealso>
		public static readonly Float WEIGHT_EXTRA_LIGHT = Convert.ToSingle(0.5f);

		/// <summary>
		/// The standard light weight. </summary>
		/// <seealso cref= #WEIGHT </seealso>
		public static readonly Float WEIGHT_LIGHT = Convert.ToSingle(0.75f);

		/// <summary>
		/// An intermediate weight between <code>WEIGHT_LIGHT</code> and
		/// <code>WEIGHT_STANDARD</code>. </summary>
		/// <seealso cref= #WEIGHT </seealso>
		public static readonly Float WEIGHT_DEMILIGHT = Convert.ToSingle(0.875f);

		/// <summary>
		/// The standard weight. This is the default value for <code>WEIGHT</code>. </summary>
		/// <seealso cref= #WEIGHT </seealso>
		public static readonly Float WEIGHT_REGULAR = Convert.ToSingle(1.0f);

		/// <summary>
		/// A moderately heavier weight than <code>WEIGHT_REGULAR</code>. </summary>
		/// <seealso cref= #WEIGHT </seealso>
		public static readonly Float WEIGHT_SEMIBOLD = Convert.ToSingle(1.25f);

		/// <summary>
		/// An intermediate weight between <code>WEIGHT_REGULAR</code> and
		/// <code>WEIGHT_BOLD</code>. </summary>
		/// <seealso cref= #WEIGHT </seealso>
		public static readonly Float WEIGHT_MEDIUM = Convert.ToSingle(1.5f);

		/// <summary>
		/// A moderately lighter weight than <code>WEIGHT_BOLD</code>. </summary>
		/// <seealso cref= #WEIGHT </seealso>
		public static readonly Float WEIGHT_DEMIBOLD = Convert.ToSingle(1.75f);

		/// <summary>
		/// The standard bold weight. </summary>
		/// <seealso cref= #WEIGHT </seealso>
		public static readonly Float WEIGHT_BOLD = Convert.ToSingle(2.0f);

		/// <summary>
		/// A moderately heavier weight than <code>WEIGHT_BOLD</code>. </summary>
		/// <seealso cref= #WEIGHT </seealso>
		public static readonly Float WEIGHT_HEAVY = Convert.ToSingle(2.25f);

		/// <summary>
		/// An extra heavy weight. </summary>
		/// <seealso cref= #WEIGHT </seealso>
		public static readonly Float WEIGHT_EXTRABOLD = Convert.ToSingle(2.5f);

		/// <summary>
		/// The heaviest predefined weight. </summary>
		/// <seealso cref= #WEIGHT </seealso>
		public static readonly Float WEIGHT_ULTRABOLD = Convert.ToSingle(2.75f);

		/// <summary>
		/// Attribute key for the width of a font.  Values are instances of
		/// <b><code>Number</code></b>.  The default value is
		/// <code>WIDTH_REGULAR</code>.
		/// 
		/// <para>Several constant values are provided, see {@link
		/// #WIDTH_CONDENSED}, <seealso cref="#WIDTH_SEMI_CONDENSED"/>, {@link
		/// #WIDTH_REGULAR}, <seealso cref="#WIDTH_SEMI_EXTENDED"/>, {@link
		/// #WIDTH_EXTENDED}.
		/// 
		/// </para>
		/// <para>The value is roughly the ratio of the advance width to that
		/// of the regular width.
		/// 
		/// </para>
		/// <para>The system can interpolate the provided value.
		/// </para>
		/// </summary>
		public static readonly TextAttribute WIDTH = new TextAttribute("width");

		/// <summary>
		/// The most condensed predefined width. </summary>
		/// <seealso cref= #WIDTH </seealso>
		public static readonly Float WIDTH_CONDENSED = Convert.ToSingle(0.75f);

		/// <summary>
		/// A moderately condensed width. </summary>
		/// <seealso cref= #WIDTH </seealso>
		public static readonly Float WIDTH_SEMI_CONDENSED = Convert.ToSingle(0.875f);

		/// <summary>
		/// The standard width. This is the default value for
		/// <code>WIDTH</code>. </summary>
		/// <seealso cref= #WIDTH </seealso>
		public static readonly Float WIDTH_REGULAR = Convert.ToSingle(1.0f);

		/// <summary>
		/// A moderately extended width. </summary>
		/// <seealso cref= #WIDTH </seealso>
		public static readonly Float WIDTH_SEMI_EXTENDED = Convert.ToSingle(1.25f);

		/// <summary>
		/// The most extended predefined width. </summary>
		/// <seealso cref= #WIDTH </seealso>
		public static readonly Float WIDTH_EXTENDED = Convert.ToSingle(1.5f);

		/// <summary>
		/// Attribute key for the posture of a font.  Values are instances
		/// of <b><code>Number</code></b>. The default value is
		/// <code>POSTURE_REGULAR</code>.
		/// 
		/// <para>Two constant values are provided, <seealso cref="#POSTURE_REGULAR"/>
		/// and <seealso cref="#POSTURE_OBLIQUE"/>. The value
		/// <code>POSTURE_OBLIQUE</code> corresponds to the style value
		/// <code>Font.ITALIC</code> as passed to the <code>Font</code>
		/// constructor.
		/// 
		/// </para>
		/// <para>The value is roughly the slope of the stems of the font,
		/// expressed as the run over the rise.  Positive values lean right.
		/// 
		/// </para>
		/// <para>The system can interpolate the provided value.
		/// 
		/// </para>
		/// <para>This will affect the font's italic angle as returned by
		/// <code>Font.getItalicAngle</code>.
		/// 
		/// </para>
		/// </summary>
		/// <seealso cref= java.awt.Font#getItalicAngle() </seealso>
		public static readonly TextAttribute POSTURE = new TextAttribute("posture");

		/// <summary>
		/// The standard posture, upright.  This is the default value for
		/// <code>POSTURE</code>. </summary>
		/// <seealso cref= #POSTURE </seealso>
		public static readonly Float POSTURE_REGULAR = Convert.ToSingle(0.0f);

		/// <summary>
		/// The standard italic posture. </summary>
		/// <seealso cref= #POSTURE </seealso>
		public static readonly Float POSTURE_OBLIQUE = Convert.ToSingle(0.20f);

		/// <summary>
		/// Attribute key for the font size.  Values are instances of
		/// <b><code>Number</code></b>.  The default value is 12pt.
		/// 
		/// <para>This corresponds to the <code>size</code> parameter to the
		/// <code>Font</code> constructor.
		/// 
		/// </para>
		/// <para>Very large or small sizes will impact rendering performance,
		/// and the rendering system might not render text at these sizes.
		/// Negative sizes are illegal and result in the default size.
		/// 
		/// </para>
		/// <para>Note that the appearance and metrics of a 12pt font with a
		/// 2x transform might be different than that of a 24 point font
		/// with no transform.
		/// </para>
		/// </summary>
		public static readonly TextAttribute SIZE = new TextAttribute("size");

		/// <summary>
		/// Attribute key for the transform of a font.  Values are
		/// instances of <b><code>TransformAttribute</code></b>.  The
		/// default value is <code>TransformAttribute.IDENTITY</code>.
		/// 
		/// <para>The <code>TransformAttribute</code> class defines the
		/// constant <seealso cref="TransformAttribute#IDENTITY IDENTITY"/>.
		/// 
		/// </para>
		/// <para>This corresponds to the transform passed to
		/// <code>Font.deriveFont(AffineTransform)</code>.  Since that
		/// transform is mutable and <code>TextAttribute</code> values must
		/// not be, the <code>TransformAttribute</code> wrapper class is
		/// used.
		/// 
		/// </para>
		/// <para>The primary intent is to support scaling and skewing, though
		/// other effects are possible.</para>
		/// 
		/// <para>Some transforms will cause the baseline to be rotated and/or
		/// shifted.  The text and the baseline are transformed together so
		/// that the text follows the new baseline.  For example, with text
		/// on a horizontal baseline, the new baseline follows the
		/// direction of the unit x vector passed through the
		/// transform. Text metrics are measured against this new baseline.
		/// So, for example, with other things being equal, text rendered
		/// with a rotated TRANSFORM and an unrotated TRANSFORM will measure as
		/// having the same ascent, descent, and advance.</para>
		/// 
		/// <para>In styled text, the baselines for each such run are aligned
		/// one after the other to potentially create a non-linear baseline
		/// for the entire run of text. For more information, see {@link
		/// TextLayout#getLayoutPath}.</para>
		/// </summary>
		/// <seealso cref= TransformAttribute </seealso>
		/// <seealso cref= java.awt.geom.AffineTransform </seealso>
		 public static readonly TextAttribute TRANSFORM = new TextAttribute("transform");

		/// <summary>
		/// Attribute key for superscripting and subscripting.  Values are
		/// instances of <b><code>Integer</code></b>.  The default value is
		/// 0, which means that no superscript or subscript is used.
		/// 
		/// <para>Two constant values are provided, see {@link
		/// #SUPERSCRIPT_SUPER} and <seealso cref="#SUPERSCRIPT_SUB"/>.  These have
		/// the values 1 and -1 respectively.  Values of
		/// greater magnitude define greater levels of superscript or
		/// subscripting, for example, 2 corresponds to super-superscript,
		/// 3 to super-super-superscript, and similarly for negative values
		/// and subscript, up to a level of 7 (or -7).  Values beyond this
		/// range are reserved; behavior is platform-dependent.
		/// 
		/// </para>
		/// <para><code>SUPERSCRIPT</code> can
		/// impact the ascent and descent of a font.  The ascent
		/// and descent can never become negative, however.
		/// </para>
		/// </summary>
		public static readonly TextAttribute SUPERSCRIPT = new TextAttribute("superscript");

		/// <summary>
		/// Standard superscript. </summary>
		/// <seealso cref= #SUPERSCRIPT </seealso>
		public static readonly Integer SUPERSCRIPT_SUPER = Convert.ToInt32(1);

		/// <summary>
		/// Standard subscript. </summary>
		/// <seealso cref= #SUPERSCRIPT </seealso>
		public static readonly Integer SUPERSCRIPT_SUB = Convert.ToInt32(-1);

		/// <summary>
		/// Attribute key used to provide the font to use to render text.
		/// Values are instances of <seealso cref="java.awt.Font"/>.  The default
		/// value is null, indicating that normal resolution of a
		/// <code>Font</code> from attributes should be performed.
		/// 
		/// <para><code>TextLayout</code> and
		/// <code>AttributedCharacterIterator</code> work in terms of
		/// <code>Maps</code> of <code>TextAttributes</code>.  Normally,
		/// all the attributes are examined and used to select and
		/// configure a <code>Font</code> instance.  If a <code>FONT</code>
		/// attribute is present, though, its associated <code>Font</code>
		/// will be used.  This provides a way for users to override the
		/// resolution of font attributes into a <code>Font</code>, or
		/// force use of a particular <code>Font</code> instance.  This
		/// also allows users to specify subclasses of <code>Font</code> in
		/// cases where a <code>Font</code> can be subclassed.
		/// 
		/// </para>
		/// <para><code>FONT</code> is used for special situations where
		/// clients already have a <code>Font</code> instance but still
		/// need to use <code>Map</code>-based APIs.  Typically, there will
		/// be no other attributes in the <code>Map</code> except the
		/// <code>FONT</code> attribute.  With <code>Map</code>-based APIs
		/// the common case is to specify all attributes individually, so
		/// <code>FONT</code> is not needed or desireable.
		/// 
		/// </para>
		/// <para>However, if both <code>FONT</code> and other attributes are
		/// present in the <code>Map</code>, the rendering system will
		/// merge the attributes defined in the <code>Font</code> with the
		/// additional attributes.  This merging process classifies
		/// <code>TextAttributes</code> into two groups.  One group, the
		/// 'primary' group, is considered fundamental to the selection and
		/// metric behavior of a font.  These attributes are
		/// <code>FAMILY</code>, <code>WEIGHT</code>, <code>WIDTH</code>,
		/// <code>POSTURE</code>, <code>SIZE</code>,
		/// <code>TRANSFORM</code>, <code>SUPERSCRIPT</code>, and
		/// <code>TRACKING</code>. The other group, the 'secondary' group,
		/// consists of all other defined attributes, with the exception of
		/// <code>FONT</code> itself.
		/// 
		/// </para>
		/// <para>To generate the new <code>Map</code>, first the
		/// <code>Font</code> is obtained from the <code>FONT</code>
		/// attribute, and <em>all</em> of its attributes extracted into a
		/// new <code>Map</code>.  Then only the <em>secondary</em>
		/// attributes from the original <code>Map</code> are added to
		/// those in the new <code>Map</code>.  Thus the values of primary
		/// attributes come solely from the <code>Font</code>, and the
		/// values of secondary attributes originate with the
		/// <code>Font</code> but can be overridden by other values in the
		/// <code>Map</code>.
		/// 
		/// </para>
		/// <para><em>Note:</em><code>Font's</code> <code>Map</code>-based
		/// constructor and <code>deriveFont</code> methods do not process
		/// the <code>FONT</code> attribute, as these are used to create
		/// new <code>Font</code> objects.  Instead, {@link
		/// java.awt.Font#getFont(Map) Font.getFont(Map)} should be used to
		/// handle the <code>FONT</code> attribute.
		/// 
		/// </para>
		/// </summary>
		/// <seealso cref= java.awt.Font </seealso>
		public static readonly TextAttribute FONT = new TextAttribute("font");

		/// <summary>
		/// Attribute key for a user-defined glyph to display in lieu
		/// of the font's standard glyph for a character.  Values are
		/// intances of GraphicAttribute.  The default value is null,
		/// indicating that the standard glyphs provided by the font
		/// should be used.
		/// 
		/// <para>This attribute is used to reserve space for a graphic or
		/// other component embedded in a line of text.  It is required for
		/// correct positioning of 'inline' components within a line when
		/// bidirectional reordering (see <seealso cref="java.text.Bidi"/>) is
		/// performed.  Each character (Unicode code point) will be
		/// rendered using the provided GraphicAttribute. Typically, the
		/// characters to which this attribute is applied should be
		/// <code>&#92;uFFFC</code>.
		/// 
		/// </para>
		/// <para>The GraphicAttribute determines the logical and visual
		/// bounds of the text; the actual Font values are ignored.
		/// 
		/// </para>
		/// </summary>
		/// <seealso cref= GraphicAttribute </seealso>
		public static readonly TextAttribute CHAR_REPLACEMENT = new TextAttribute("char_replacement");

		//
		// Adornments added to text.
		//

		/// <summary>
		/// Attribute key for the paint used to render the text.  Values are
		/// instances of <b><code>Paint</code></b>.  The default value is
		/// null, indicating that the <code>Paint</code> set on the
		/// <code>Graphics2D</code> at the time of rendering is used.
		/// 
		/// <para>Glyphs will be rendered using this
		/// <code>Paint</code> regardless of the <code>Paint</code> value
		/// set on the <code>Graphics</code> (but see <seealso cref="#SWAP_COLORS"/>).
		/// 
		/// </para>
		/// </summary>
		/// <seealso cref= java.awt.Paint </seealso>
		/// <seealso cref= #SWAP_COLORS </seealso>
		public static readonly TextAttribute FOREGROUND = new TextAttribute("foreground");

		/// <summary>
		/// Attribute key for the paint used to render the background of
		/// the text.  Values are instances of <b><code>Paint</code></b>.
		/// The default value is null, indicating that the background
		/// should not be rendered.
		/// 
		/// <para>The logical bounds of the text will be filled using this
		/// <code>Paint</code>, and then the text will be rendered on top
		/// of it (but see <seealso cref="#SWAP_COLORS"/>).
		/// 
		/// </para>
		/// <para>The visual bounds of the text is extended to include the
		/// logical bounds, if necessary.  The outline is not affected.
		/// 
		/// </para>
		/// </summary>
		/// <seealso cref= java.awt.Paint </seealso>
		/// <seealso cref= #SWAP_COLORS </seealso>
		public static readonly TextAttribute BACKGROUND = new TextAttribute("background");

		/// <summary>
		/// Attribute key for underline.  Values are instances of
		/// <b><code>Integer</code></b>.  The default value is -1, which
		/// means no underline.
		/// 
		/// <para>The constant value <seealso cref="#UNDERLINE_ON"/> is provided.
		/// 
		/// </para>
		/// <para>The underline affects both the visual bounds and the outline
		/// of the text.
		/// </para>
		/// </summary>
		public static readonly TextAttribute UNDERLINE = new TextAttribute("underline");

		/// <summary>
		/// Standard underline.
		/// </summary>
		/// <seealso cref= #UNDERLINE </seealso>
		public static readonly Integer UNDERLINE_ON = Convert.ToInt32(0);

		/// <summary>
		/// Attribute key for strikethrough.  Values are instances of
		/// <b><code>Boolean</code></b>.  The default value is
		/// <code>false</code>, which means no strikethrough.
		/// 
		/// <para>The constant value <seealso cref="#STRIKETHROUGH_ON"/> is provided.
		/// 
		/// </para>
		/// <para>The strikethrough affects both the visual bounds and the
		/// outline of the text.
		/// </para>
		/// </summary>
		public static readonly TextAttribute STRIKETHROUGH = new TextAttribute("strikethrough");

		/// <summary>
		/// A single strikethrough.
		/// </summary>
		/// <seealso cref= #STRIKETHROUGH </seealso>
		public static readonly Boolean STRIKETHROUGH_ON = true;

		//
		// Attributes use to control layout of text on a line.
		//

		/// <summary>
		/// Attribute key for the run direction of the line.  Values are
		/// instances of <b><code>Boolean</code></b>.  The default value is
		/// null, which indicates that the standard Bidi algorithm for
		/// determining run direction should be used with the value {@link
		/// java.text.Bidi#DIRECTION_DEFAULT_LEFT_TO_RIGHT}.
		/// 
		/// <para>The constants <seealso cref="#RUN_DIRECTION_RTL"/> and {@link
		/// #RUN_DIRECTION_LTR} are provided.
		/// 
		/// </para>
		/// <para>This determines the value passed to the {@link
		/// java.text.Bidi} constructor to select the primary direction of
		/// the text in the paragraph.
		/// 
		/// </para>
		/// <para><em>Note:</em> This attribute should have the same value for
		/// all the text in a paragraph, otherwise the behavior is
		/// undetermined.
		/// 
		/// </para>
		/// </summary>
		/// <seealso cref= java.text.Bidi </seealso>
		public static readonly TextAttribute RUN_DIRECTION = new TextAttribute("run_direction");

		/// <summary>
		/// Left-to-right run direction. </summary>
		/// <seealso cref= #RUN_DIRECTION </seealso>
		public static readonly Boolean RUN_DIRECTION_LTR = false;

		/// <summary>
		/// Right-to-left run direction. </summary>
		/// <seealso cref= #RUN_DIRECTION </seealso>
		public static readonly Boolean RUN_DIRECTION_RTL = true;

		/// <summary>
		/// Attribute key for the embedding level of the text.  Values are
		/// instances of <b><code>Integer</code></b>.  The default value is
		/// <code>null</code>, indicating that the the Bidirectional
		/// algorithm should run without explicit embeddings.
		/// 
		/// <para>Positive values 1 through 61 are <em>embedding</em> levels,
		/// negative values -1 through -61 are <em>override</em> levels.
		/// The value 0 means that the base line direction is used.  These
		/// levels are passed in the embedding levels array to the {@link
		/// java.text.Bidi} constructor.
		/// 
		/// </para>
		/// <para><em>Note:</em> When this attribute is present anywhere in
		/// a paragraph, then any Unicode bidi control characters (RLO,
		/// LRO, RLE, LRE, and PDF) in the paragraph are
		/// disregarded, and runs of text where this attribute is not
		/// present are treated as though it were present and had the value
		/// 0.
		/// 
		/// </para>
		/// </summary>
		/// <seealso cref= java.text.Bidi </seealso>
		public static readonly TextAttribute BIDI_EMBEDDING = new TextAttribute("bidi_embedding");

		/// <summary>
		/// Attribute key for the justification of a paragraph.  Values are
		/// instances of <b><code>Number</code></b>.  The default value is
		/// 1, indicating that justification should use the full width
		/// provided.  Values are pinned to the range [0..1].
		/// 
		/// <para>The constants <seealso cref="#JUSTIFICATION_FULL"/> and {@link
		/// #JUSTIFICATION_NONE} are provided.
		/// 
		/// </para>
		/// <para>Specifies the fraction of the extra space to use when
		/// justification is requested on a <code>TextLayout</code>. For
		/// example, if the line is 50 points wide and it is requested to
		/// justify to 70 points, a value of 0.75 will pad to use
		/// three-quarters of the remaining space, or 15 points, so that
		/// the resulting line will be 65 points in length.
		/// 
		/// </para>
		/// <para><em>Note:</em> This should have the same value for all the
		/// text in a paragraph, otherwise the behavior is undetermined.
		/// 
		/// </para>
		/// </summary>
		/// <seealso cref= TextLayout#getJustifiedLayout </seealso>
		public static readonly TextAttribute JUSTIFICATION = new TextAttribute("justification");

		/// <summary>
		/// Justify the line to the full requested width.  This is the
		/// default value for <code>JUSTIFICATION</code>. </summary>
		/// <seealso cref= #JUSTIFICATION </seealso>
		public static readonly Float JUSTIFICATION_FULL = Convert.ToSingle(1.0f);

		/// <summary>
		/// Do not allow the line to be justified. </summary>
		/// <seealso cref= #JUSTIFICATION </seealso>
		public static readonly Float JUSTIFICATION_NONE = Convert.ToSingle(0.0f);

		//
		// For use by input method.
		//

		/// <summary>
		/// Attribute key for input method highlight styles.
		/// 
		/// <para>Values are instances of {@link
		/// java.awt.im.InputMethodHighlight} or {@link
		/// java.text.Annotation}.  The default value is <code>null</code>,
		/// which means that input method styles should not be applied
		/// before rendering.
		/// 
		/// </para>
		/// <para>If adjacent runs of text with the same
		/// <code>InputMethodHighlight</code> need to be rendered
		/// separately, the <code>InputMethodHighlights</code> should be
		/// wrapped in <code>Annotation</code> instances.
		/// 
		/// </para>
		/// <para>Input method highlights are used while text is being
		/// composed by an input method. Text editing components should
		/// retain them even if they generally only deal with unstyled
		/// text, and make them available to the drawing routines.
		/// 
		/// </para>
		/// </summary>
		/// <seealso cref= java.awt.Font </seealso>
		/// <seealso cref= java.awt.im.InputMethodHighlight </seealso>
		/// <seealso cref= java.text.Annotation </seealso>
		public static readonly TextAttribute INPUT_METHOD_HIGHLIGHT = new TextAttribute("input method highlight");

		/// <summary>
		/// Attribute key for input method underlines.  Values
		/// are instances of <b><code>Integer</code></b>.  The default
		/// value is <code>-1</code>, which means no underline.
		/// 
		/// <para>Several constant values are provided, see {@link
		/// #UNDERLINE_LOW_ONE_PIXEL}, <seealso cref="#UNDERLINE_LOW_TWO_PIXEL"/>,
		/// <seealso cref="#UNDERLINE_LOW_DOTTED"/>, <seealso cref="#UNDERLINE_LOW_GRAY"/>, and
		/// <seealso cref="#UNDERLINE_LOW_DASHED"/>.
		/// 
		/// </para>
		/// <para>This may be used in conjunction with <seealso cref="#UNDERLINE"/> if
		/// desired.  The primary purpose is for use by input methods.
		/// Other use of these underlines for simple ornamentation might
		/// confuse users.
		/// 
		/// </para>
		/// <para>The input method underline affects both the visual bounds and
		/// the outline of the text.
		/// 
		/// @since 1.3
		/// </para>
		/// </summary>
		public static readonly TextAttribute INPUT_METHOD_UNDERLINE = new TextAttribute("input method underline");

		/// <summary>
		/// Single pixel solid low underline. </summary>
		/// <seealso cref= #INPUT_METHOD_UNDERLINE
		/// @since 1.3 </seealso>
		public static readonly Integer UNDERLINE_LOW_ONE_PIXEL = Convert.ToInt32(1);

		/// <summary>
		/// Double pixel solid low underline. </summary>
		/// <seealso cref= #INPUT_METHOD_UNDERLINE
		/// @since 1.3 </seealso>
		public static readonly Integer UNDERLINE_LOW_TWO_PIXEL = Convert.ToInt32(2);

		/// <summary>
		/// Single pixel dotted low underline. </summary>
		/// <seealso cref= #INPUT_METHOD_UNDERLINE
		/// @since 1.3 </seealso>
		public static readonly Integer UNDERLINE_LOW_DOTTED = Convert.ToInt32(3);

		/// <summary>
		/// Double pixel gray low underline. </summary>
		/// <seealso cref= #INPUT_METHOD_UNDERLINE
		/// @since 1.3 </seealso>
		public static readonly Integer UNDERLINE_LOW_GRAY = Convert.ToInt32(4);

		/// <summary>
		/// Single pixel dashed low underline. </summary>
		/// <seealso cref= #INPUT_METHOD_UNDERLINE
		/// @since 1.3 </seealso>
		public static readonly Integer UNDERLINE_LOW_DASHED = Convert.ToInt32(5);

		/// <summary>
		/// Attribute key for swapping foreground and background
		/// <code>Paints</code>.  Values are instances of
		/// <b><code>Boolean</code></b>.  The default value is
		/// <code>false</code>, which means do not swap colors.
		/// 
		/// <para>The constant value <seealso cref="#SWAP_COLORS_ON"/> is defined.
		/// 
		/// </para>
		/// <para>If the <seealso cref="#FOREGROUND"/> attribute is set, its
		/// <code>Paint</code> will be used as the background, otherwise
		/// the <code>Paint</code> currently on the <code>Graphics</code>
		/// will be used.  If the <seealso cref="#BACKGROUND"/> attribute is set, its
		/// <code>Paint</code> will be used as the foreground, otherwise
		/// the system will find a contrasting color to the
		/// (resolved) background so that the text will be visible.
		/// 
		/// </para>
		/// </summary>
		/// <seealso cref= #FOREGROUND </seealso>
		/// <seealso cref= #BACKGROUND </seealso>
		public static readonly TextAttribute SWAP_COLORS = new TextAttribute("swap_colors");

		/// <summary>
		/// Swap foreground and background. </summary>
		/// <seealso cref= #SWAP_COLORS
		/// @since 1.3 </seealso>
		public static readonly Boolean SWAP_COLORS_ON = true;

		/// <summary>
		/// Attribute key for converting ASCII decimal digits to other
		/// decimal ranges.  Values are instances of <seealso cref="NumericShaper"/>.
		/// The default is <code>null</code>, which means do not perform
		/// numeric shaping.
		/// 
		/// <para>When a numeric shaper is defined, the text is first
		/// processed by the shaper before any other analysis of the text
		/// is performed.
		/// 
		/// </para>
		/// <para><em>Note:</em> This should have the same value for all the
		/// text in the paragraph, otherwise the behavior is undetermined.
		/// 
		/// </para>
		/// </summary>
		/// <seealso cref= NumericShaper
		/// @since 1.4 </seealso>
		public static readonly TextAttribute NUMERIC_SHAPING = new TextAttribute("numeric_shaping");

		/// <summary>
		/// Attribute key to request kerning. Values are instances of
		/// <b><code>Integer</code></b>.  The default value is
		/// <code>0</code>, which does not request kerning.
		/// 
		/// <para>The constant value <seealso cref="#KERNING_ON"/> is provided.
		/// 
		/// </para>
		/// <para>The default advances of single characters are not
		/// appropriate for some character sequences, for example "To" or
		/// "AWAY".  Without kerning the adjacent characters appear to be
		/// separated by too much space.  Kerning causes selected sequences
		/// of characters to be spaced differently for a more pleasing
		/// visual appearance.
		/// 
		/// @since 1.6
		/// </para>
		/// </summary>
		public static readonly TextAttribute KERNING = new TextAttribute("kerning");

		/// <summary>
		/// Request standard kerning. </summary>
		/// <seealso cref= #KERNING
		/// @since 1.6 </seealso>
		public static readonly Integer KERNING_ON = Convert.ToInt32(1);


		/// <summary>
		/// Attribute key for enabling optional ligatures. Values are
		/// instances of <b><code>Integer</code></b>.  The default value is
		/// <code>0</code>, which means do not use optional ligatures.
		/// 
		/// <para>The constant value <seealso cref="#LIGATURES_ON"/> is defined.
		/// 
		/// </para>
		/// <para>Ligatures required by the writing system are always enabled.
		/// 
		/// @since 1.6
		/// </para>
		/// </summary>
		public static readonly TextAttribute LIGATURES = new TextAttribute("ligatures");

		/// <summary>
		/// Request standard optional ligatures. </summary>
		/// <seealso cref= #LIGATURES
		/// @since 1.6 </seealso>
		public static readonly Integer LIGATURES_ON = Convert.ToInt32(1);

		/// <summary>
		/// Attribute key to control tracking.  Values are instances of
		/// <b><code>Number</code></b>.  The default value is
		/// <code>0</code>, which means no additional tracking.
		/// 
		/// <para>The constant values <seealso cref="#TRACKING_TIGHT"/> and {@link
		/// #TRACKING_LOOSE} are provided.
		/// 
		/// </para>
		/// <para>The tracking value is multiplied by the font point size and
		/// passed through the font transform to determine an additional
		/// amount to add to the advance of each glyph cluster.  Positive
		/// tracking values will inhibit formation of optional ligatures.
		/// Tracking values are typically between <code>-0.1</code> and
		/// <code>0.3</code>; values outside this range are generally not
		/// desireable.
		/// 
		/// @since 1.6
		/// </para>
		/// </summary>
		public static readonly TextAttribute TRACKING = new TextAttribute("tracking");

		/// <summary>
		/// Perform tight tracking. </summary>
		/// <seealso cref= #TRACKING
		/// @since 1.6 </seealso>
		public static readonly Float TRACKING_TIGHT = Convert.ToSingle(-.04f);

		/// <summary>
		/// Perform loose tracking. </summary>
		/// <seealso cref= #TRACKING
		/// @since 1.6 </seealso>
		public static readonly Float TRACKING_LOOSE = Convert.ToSingle(.04f);
	}

}
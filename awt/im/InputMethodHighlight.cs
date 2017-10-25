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

namespace java.awt.im
{


	/// <summary>
	/// An InputMethodHighlight is used to describe the highlight
	/// attributes of text being composed.
	/// The description can be at two levels:
	/// at the abstract level it specifies the conversion state and whether the
	/// text is selected; at the concrete level it specifies style attributes used
	/// to render the highlight.
	/// An InputMethodHighlight must provide the description at the
	/// abstract level; it may or may not provide the description at the concrete
	/// level.
	/// If no concrete style is provided, a renderer should use
	/// <seealso cref="java.awt.Toolkit#mapInputMethodHighlight"/> to map to a concrete style.
	/// <para>
	/// The abstract description consists of three fields: <code>selected</code>,
	/// <code>state</code>, and <code>variation</code>.
	/// <code>selected</code> indicates whether the text range is the one that the
	/// input method is currently working on, for example, the segment for which
	/// conversion candidates are currently shown in a menu.
	/// <code>state</code> represents the conversion state. State values are defined
	/// by the input method framework and should be distinguished in all
	/// mappings from abstract to concrete styles. Currently defined state values
	/// are raw (unconverted) and converted.
	/// These state values are recommended for use before and after the
	/// main conversion step of text composition, say, before and after kana-&gt;kanji
	/// or pinyin-&gt;hanzi conversion.
	/// The <code>variation</code> field allows input methods to express additional
	/// information about the conversion results.
	/// </para>
	/// <para>
	/// 
	/// InputMethodHighlight instances are typically used as attribute values
	/// returned from AttributedCharacterIterator for the INPUT_METHOD_HIGHLIGHT
	/// attribute. They may be wrapped into <seealso cref="java.text.Annotation Annotation"/>
	/// instances to indicate separate text segments.
	/// 
	/// </para>
	/// </summary>
	/// <seealso cref= java.text.AttributedCharacterIterator
	/// @since 1.2 </seealso>

	public class InputMethodHighlight
	{

		/// <summary>
		/// Constant for the raw text state.
		/// </summary>
		public const int RAW_TEXT = 0;

		/// <summary>
		/// Constant for the converted text state.
		/// </summary>
		public const int CONVERTED_TEXT = 1;


		/// <summary>
		/// Constant for the default highlight for unselected raw text.
		/// </summary>
		public static readonly InputMethodHighlight UNSELECTED_RAW_TEXT_HIGHLIGHT = new InputMethodHighlight(false, RAW_TEXT);

		/// <summary>
		/// Constant for the default highlight for selected raw text.
		/// </summary>
		public static readonly InputMethodHighlight SELECTED_RAW_TEXT_HIGHLIGHT = new InputMethodHighlight(true, RAW_TEXT);

		/// <summary>
		/// Constant for the default highlight for unselected converted text.
		/// </summary>
		public static readonly InputMethodHighlight UNSELECTED_CONVERTED_TEXT_HIGHLIGHT = new InputMethodHighlight(false, CONVERTED_TEXT);

		/// <summary>
		/// Constant for the default highlight for selected converted text.
		/// </summary>
		public static readonly InputMethodHighlight SELECTED_CONVERTED_TEXT_HIGHLIGHT = new InputMethodHighlight(true, CONVERTED_TEXT);


		/// <summary>
		/// Constructs an input method highlight record.
		/// The variation is set to 0, the style to null. </summary>
		/// <param name="selected"> Whether the text range is selected </param>
		/// <param name="state"> The conversion state for the text range - RAW_TEXT or CONVERTED_TEXT </param>
		/// <seealso cref= InputMethodHighlight#RAW_TEXT </seealso>
		/// <seealso cref= InputMethodHighlight#CONVERTED_TEXT </seealso>
		/// <exception cref="IllegalArgumentException"> if a state other than RAW_TEXT or CONVERTED_TEXT is given </exception>
		public InputMethodHighlight(bool selected, int state) : this(selected, state, 0, null)
		{
		}

		/// <summary>
		/// Constructs an input method highlight record.
		/// The style is set to null. </summary>
		/// <param name="selected"> Whether the text range is selected </param>
		/// <param name="state"> The conversion state for the text range - RAW_TEXT or CONVERTED_TEXT </param>
		/// <param name="variation"> The style variation for the text range </param>
		/// <seealso cref= InputMethodHighlight#RAW_TEXT </seealso>
		/// <seealso cref= InputMethodHighlight#CONVERTED_TEXT </seealso>
		/// <exception cref="IllegalArgumentException"> if a state other than RAW_TEXT or CONVERTED_TEXT is given </exception>
		public InputMethodHighlight(bool selected, int state, int variation) : this(selected, state, variation, null)
		{
		}

		/// <summary>
		/// Constructs an input method highlight record.
		/// The style attributes map provided must be unmodifiable. </summary>
		/// <param name="selected"> whether the text range is selected </param>
		/// <param name="state"> the conversion state for the text range - RAW_TEXT or CONVERTED_TEXT </param>
		/// <param name="variation"> the variation for the text range </param>
		/// <param name="style"> the rendering style attributes for the text range, or null </param>
		/// <seealso cref= InputMethodHighlight#RAW_TEXT </seealso>
		/// <seealso cref= InputMethodHighlight#CONVERTED_TEXT </seealso>
		/// <exception cref="IllegalArgumentException"> if a state other than RAW_TEXT or CONVERTED_TEXT is given
		/// @since 1.3 </exception>
		public InputMethodHighlight<T1>(bool selected, int state, int variation, IDictionary<T1> style)
		{
			this.Selected_Renamed = selected;
			if (!(state == RAW_TEXT || state == CONVERTED_TEXT))
			{
				throw new IllegalArgumentException("unknown input method highlight state");
			}
			this.State_Renamed = state;
			this.Variation_Renamed = variation;
			this.Style_Renamed = style;
		}

		/// <summary>
		/// Returns whether the text range is selected.
		/// </summary>
		public virtual bool Selected
		{
			get
			{
				return Selected_Renamed;
			}
		}

		/// <summary>
		/// Returns the conversion state of the text range. </summary>
		/// <returns> The conversion state for the text range - RAW_TEXT or CONVERTED_TEXT. </returns>
		/// <seealso cref= InputMethodHighlight#RAW_TEXT </seealso>
		/// <seealso cref= InputMethodHighlight#CONVERTED_TEXT </seealso>
		public virtual int State
		{
			get
			{
				return State_Renamed;
			}
		}

		/// <summary>
		/// Returns the variation of the text range.
		/// </summary>
		public virtual int Variation
		{
			get
			{
				return Variation_Renamed;
			}
		}

		/// <summary>
		/// Returns the rendering style attributes for the text range, or null.
		/// @since 1.3
		/// </summary>
//JAVA TO C# CONVERTER TODO TASK: Java wildcard generics are not converted to .NET:
//ORIGINAL LINE: public java.util.Map<java.awt.font.TextAttribute,?> getStyle()
		public virtual IDictionary<TextAttribute, ?> Style
		{
			get
			{
				return Style_Renamed;
			}
		}

		private bool Selected_Renamed;
		private int State_Renamed;
		private int Variation_Renamed;
//JAVA TO C# CONVERTER TODO TASK: Java wildcard generics are not converted to .NET:
//ORIGINAL LINE: private java.util.Map<java.awt.font.TextAttribute, ?> style;
		private IDictionary<TextAttribute, ?> Style_Renamed;

	}

}
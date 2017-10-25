using System;
using System.Collections;
using System.Collections.Generic;

/*
 * Copyright (c) 1999, 2013, Oracle and/or its affiliates. All rights reserved.
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
 *
 */

/*
 * (C) Copyright IBM Corp. 1999,  All rights reserved.
 */
namespace java.awt.font
{

	using Decoration = sun.font.Decoration;
	using FontResolver = sun.font.FontResolver;
	using CodePointIterator = sun.text.CodePointIterator;

	/// <summary>
	/// This class stores Font, GraphicAttribute, and Decoration intervals
	/// on a paragraph of styled text.
	/// <para>
	/// Currently, this class is optimized for a small number of intervals
	/// (preferrably 1).
	/// </para>
	/// </summary>
	internal sealed class StyledParagraph
	{

		// the length of the paragraph
		private int Length;

		// If there is a single Decoration for the whole paragraph, it
		// is stored here.  Otherwise this field is ignored.

		private Decoration Decoration;

		// If there is a single Font or GraphicAttribute for the whole
		// paragraph, it is stored here.  Otherwise this field is ignored.
		private Object Font;

		// If there are multiple Decorations in the paragraph, they are
		// stored in this Vector, in order.  Otherwise this vector and
		// the decorationStarts array are null.
		private List<Decoration> Decorations;
		// If there are multiple Decorations in the paragraph,
		// decorationStarts[i] contains the index where decoration i
		// starts.  For convenience, there is an extra entry at the
		// end of this array with the length of the paragraph.
		internal int[] DecorationStarts;

		// If there are multiple Fonts/GraphicAttributes in the paragraph,
		// they are
		// stored in this Vector, in order.  Otherwise this vector and
		// the fontStarts array are null.
		private List<Object> Fonts;
		// If there are multiple Fonts/GraphicAttributes in the paragraph,
		// fontStarts[i] contains the index where decoration i
		// starts.  For convenience, there is an extra entry at the
		// end of this array with the length of the paragraph.
		internal int[] FontStarts;

		private static int INITIAL_SIZE = 8;

		/// <summary>
		/// Create a new StyledParagraph over the given styled text. </summary>
		/// <param name="aci"> an iterator over the text </param>
		/// <param name="chars"> the characters extracted from aci </param>
		public StyledParagraph(AttributedCharacterIterator aci, char[] chars)
		{

			int start = aci.BeginIndex;
			int end = aci.EndIndex;
			Length = end - start;

			int index = start;
			aci.First();

			do
			{
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final int nextRunStart = aci.getRunLimit();
				int nextRunStart = aci.RunLimit;
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final int localIndex = index-start;
				int localIndex = index - start;

//JAVA TO C# CONVERTER TODO TASK: Java wildcard generics are not converted to .NET:
//ORIGINAL LINE: java.util.Map<? extends java.text.AttributedCharacterIterator_Attribute, ?> attributes = aci.getAttributes();
				IDictionary<?, ?> attributes = aci.Attributes;
				attributes = AddInputMethodAttrs(attributes);
				Decoration d = Decoration.getDecoration(attributes);
				AddDecoration(d, localIndex);

				Object f = GetGraphicOrFont(attributes);
				if (f == null)
				{
					AddFonts(chars, attributes, localIndex, nextRunStart - start);
				}
				else
				{
					AddFont(f, localIndex);
				}

				aci.Index = nextRunStart;
				index = nextRunStart;

			} while (index < end);

			// Add extra entries to starts arrays with the length
			// of the paragraph.  'this' is used as a dummy value
			// in the Vector.
			if (Decorations != null)
			{
				DecorationStarts = AddToVector(this, Length, Decorations, DecorationStarts);
			}
			if (Fonts != null)
			{
				FontStarts = AddToVector(this, Length, Fonts, FontStarts);
			}
		}

		/// <summary>
		/// Adjust indices in starts to reflect an insertion after pos.
		/// Any index in starts greater than pos will be increased by 1.
		/// </summary>
		private static void InsertInto(int pos, int[] starts, int numStarts)
		{

			while (starts[--numStarts] > pos)
			{
				starts[numStarts] += 1;
			}
		}

		/// <summary>
		/// Return a StyledParagraph reflecting the insertion of a single character
		/// into the text.  This method will attempt to reuse the given paragraph,
		/// but may create a new paragraph. </summary>
		/// <param name="aci"> an iterator over the text.  The text should be the same as the
		///     text used to create (or most recently update) oldParagraph, with
		///     the exception of inserting a single character at insertPos. </param>
		/// <param name="chars"> the characters in aci </param>
		/// <param name="insertPos"> the index of the new character in aci </param>
		/// <param name="oldParagraph"> a StyledParagraph for the text in aci before the
		///     insertion </param>
		public static StyledParagraph InsertChar(AttributedCharacterIterator aci, char[] chars, int insertPos, StyledParagraph oldParagraph)
		{

			// If the styles at insertPos match those at insertPos-1,
			// oldParagraph will be reused.  Otherwise we create a new
			// paragraph.

			char ch = aci.setIndex(insertPos);
			int relativePos = System.Math.Max(insertPos - aci.BeginIndex - 1, 0);

//JAVA TO C# CONVERTER TODO TASK: Java wildcard generics are not converted to .NET:
//ORIGINAL LINE: java.util.Map<? extends java.text.AttributedCharacterIterator_Attribute, ?> attributes = addInputMethodAttrs(aci.getAttributes());
			IDictionary<?, ?> attributes = AddInputMethodAttrs(aci.Attributes);
			Decoration d = Decoration.getDecoration(attributes);
			if (!oldParagraph.GetDecorationAt(relativePos).Equals(d))
			{
				return new StyledParagraph(aci, chars);
			}
			Object f = GetGraphicOrFont(attributes);
			if (f == null)
			{
				FontResolver resolver = FontResolver.Instance;
				int fontIndex = resolver.getFontIndex(ch);
				f = resolver.getFont(fontIndex, attributes);
			}
			if (!oldParagraph.GetFontOrGraphicAt(relativePos).Equals(f))
			{
				return new StyledParagraph(aci, chars);
			}

			// insert into existing paragraph
			oldParagraph.Length += 1;
			if (oldParagraph.Decorations != null)
			{
				InsertInto(relativePos, oldParagraph.DecorationStarts, oldParagraph.Decorations.Count);
			}
			if (oldParagraph.Fonts != null)
			{
				InsertInto(relativePos, oldParagraph.FontStarts, oldParagraph.Fonts.Count);
			}
			return oldParagraph;
		}

		/// <summary>
		/// Adjust indices in starts to reflect a deletion after deleteAt.
		/// Any index in starts greater than deleteAt will be increased by 1.
		/// It is the caller's responsibility to make sure that no 0-length
		/// runs result.
		/// </summary>
		private static void DeleteFrom(int deleteAt, int[] starts, int numStarts)
		{

			while (starts[--numStarts] > deleteAt)
			{
				starts[numStarts] -= 1;
			}
		}

		/// <summary>
		/// Return a StyledParagraph reflecting the insertion of a single character
		/// into the text.  This method will attempt to reuse the given paragraph,
		/// but may create a new paragraph. </summary>
		/// <param name="aci"> an iterator over the text.  The text should be the same as the
		///     text used to create (or most recently update) oldParagraph, with
		///     the exception of deleting a single character at deletePos. </param>
		/// <param name="chars"> the characters in aci </param>
		/// <param name="deletePos"> the index where a character was removed </param>
		/// <param name="oldParagraph"> a StyledParagraph for the text in aci before the
		///     insertion </param>
		public static StyledParagraph DeleteChar(AttributedCharacterIterator aci, char[] chars, int deletePos, StyledParagraph oldParagraph)
		{

			// We will reuse oldParagraph unless there was a length-1 run
			// at deletePos.  We could do more work and check the individual
			// Font and Decoration runs, but we don't right now...
			deletePos -= aci.BeginIndex;

			if (oldParagraph.Decorations == null && oldParagraph.Fonts == null)
			{
				oldParagraph.Length -= 1;
				return oldParagraph;
			}

			if (oldParagraph.GetRunLimit(deletePos) == deletePos + 1)
			{
				if (deletePos == 0 || oldParagraph.GetRunLimit(deletePos - 1) == deletePos)
				{
					return new StyledParagraph(aci, chars);
				}
			}

			oldParagraph.Length -= 1;
			if (oldParagraph.Decorations != null)
			{
				DeleteFrom(deletePos, oldParagraph.DecorationStarts, oldParagraph.Decorations.Count);
			}
			if (oldParagraph.Fonts != null)
			{
				DeleteFrom(deletePos, oldParagraph.FontStarts, oldParagraph.Fonts.Count);
			}
			return oldParagraph;
		}

		/// <summary>
		/// Return the index at which there is a different Font, GraphicAttribute, or
		/// Dcoration than at the given index. </summary>
		/// <param name="index"> a valid index in the paragraph </param>
		/// <returns> the first index where there is a change in attributes from
		///      those at index </returns>
		public int GetRunLimit(int index)
		{

			if (index < 0 || index >= Length)
			{
				throw new IllegalArgumentException("index out of range");
			}
			int limit1 = Length;
			if (Decorations != null)
			{
				int run = FindRunContaining(index, DecorationStarts);
				limit1 = DecorationStarts[run + 1];
			}
			int limit2 = Length;
			if (Fonts != null)
			{
				int run = FindRunContaining(index, FontStarts);
				limit2 = FontStarts[run + 1];
			}
			return System.Math.Min(limit1, limit2);
		}

		/// <summary>
		/// Return the Decoration in effect at the given index. </summary>
		/// <param name="index"> a valid index in the paragraph </param>
		/// <returns> the Decoration at index. </returns>
		public Decoration GetDecorationAt(int index)
		{

			if (index < 0 || index >= Length)
			{
				throw new IllegalArgumentException("index out of range");
			}
			if (Decorations == null)
			{
				return Decoration;
			}
			int run = FindRunContaining(index, DecorationStarts);
			return Decorations[run];
		}

		/// <summary>
		/// Return the Font or GraphicAttribute in effect at the given index.
		/// The client must test the type of the return value to determine what
		/// it is. </summary>
		/// <param name="index"> a valid index in the paragraph </param>
		/// <returns> the Font or GraphicAttribute at index. </returns>
		public Object GetFontOrGraphicAt(int index)
		{

			if (index < 0 || index >= Length)
			{
				throw new IllegalArgumentException("index out of range");
			}
			if (Fonts == null)
			{
				return Font;
			}
			int run = FindRunContaining(index, FontStarts);
			return Fonts[run];
		}

		/// <summary>
		/// Return i such that starts[i] &lt;= index &lt; starts[i+1].  starts
		/// must be in increasing order, with at least one element greater
		/// than index.
		/// </summary>
		private static int FindRunContaining(int index, int[] starts)
		{

			for (int i = 1; true; i++)
			{
				if (starts[i] > index)
				{
					return i - 1;
				}
			}
		}

		/// <summary>
		/// Append the given Object to the given Vector.  Add
		/// the given index to the given starts array.  If the
		/// starts array does not have room for the index, a
		/// new array is created and returned.
		/// </summary>
//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @SuppressWarnings({"rawtypes", "unchecked"}) private static int[] addToVector(Object obj, int index, java.util.Vector v, int[] starts)
		private static int[] AddToVector(Object obj, int index, ArrayList v, int[] starts)
		{

			if (!v[v.Count - 1].Equals(obj))
			{
				v.Add(obj);
				int count = v.Count;
				if (starts.Length == count)
				{
					int[] temp = new int[starts.Length * 2];
					System.Array.Copy(starts, 0, temp, 0, starts.Length);
					starts = temp;
				}
				starts[count - 1] = index;
			}
			return starts;
		}

		/// <summary>
		/// Add a new Decoration run with the given Decoration at the
		/// given index.
		/// </summary>
		private void AddDecoration(Decoration d, int index)
		{

			if (Decorations != null)
			{
				DecorationStarts = AddToVector(d, index, Decorations, DecorationStarts);
			}
			else if (Decoration == null)
			{
				Decoration = d;
			}
			else
			{
				if (!Decoration.Equals(d))
				{
					Decorations = new List<Decoration>(INITIAL_SIZE);
					Decorations.Add(Decoration);
					Decorations.Add(d);
					DecorationStarts = new int[INITIAL_SIZE];
					DecorationStarts[0] = 0;
					DecorationStarts[1] = index;
				}
			}
		}

		/// <summary>
		/// Add a new Font/GraphicAttribute run with the given object at the
		/// given index.
		/// </summary>
		private void AddFont(Object f, int index)
		{

			if (Fonts != null)
			{
				FontStarts = AddToVector(f, index, Fonts, FontStarts);
			}
			else if (Font == null)
			{
				Font = f;
			}
			else
			{
				if (!Font.Equals(f))
				{
					Fonts = new List<Object>(INITIAL_SIZE);
					Fonts.Add(Font);
					Fonts.Add(f);
					FontStarts = new int[INITIAL_SIZE];
					FontStarts[0] = 0;
					FontStarts[1] = index;
				}
			}
		}

		/// <summary>
		/// Resolve the given chars into Fonts using FontResolver, then add
		/// font runs for each.
		/// </summary>
		private void addFonts<T1>(char[] chars, IDictionary<T1> attributes, int start, int limit) where T1 : java.text.AttributedCharacterIterator_Attribute
		{

			FontResolver resolver = FontResolver.Instance;
			CodePointIterator iter = CodePointIterator.create(chars, start, limit);
			for (int runStart = iter.charIndex(); runStart < limit; runStart = iter.charIndex())
			{
				int fontIndex = resolver.nextFontRunIndex(iter);
				AddFont(resolver.getFont(fontIndex, attributes), runStart);
			}
		}

		/// <summary>
		/// Return a Map with entries from oldStyles, as well as input
		/// method entries, if any.
		/// </summary>
//JAVA TO C# CONVERTER TODO TASK: Java wildcard generics are not converted to .NET:
//ORIGINAL LINE: static java.util.Map<? extends java.text.AttributedCharacterIterator_Attribute, ?> addInputMethodAttrs(java.util.Map<? extends java.text.AttributedCharacterIterator_Attribute, ?> oldStyles)
//JAVA TO C# CONVERTER TODO TASK: Java wildcard generics are not converted to .NET:
//ORIGINAL LINE: static java.util.Map<? extends java.text.AttributedCharacterIterator_Attribute, ?> addInputMethodAttrs(java.util.Map<? extends java.text.AttributedCharacterIterator_Attribute, ?> oldStyles)
		internal static IDictionary<?, ?> AddInputMethodAttrs(IDictionary<T1> oldStyles) where ? : java.text.AttributedCharacterIterator_Attribute where T1 : java.text.AttributedCharacterIterator_Attribute
		{

			Object value = oldStyles[TextAttribute.INPUT_METHOD_HIGHLIGHT];

			try
			{
				if (value != null)
				{
					if (value is Annotation)
					{
						value = ((Annotation)value).Value;
					}

					InputMethodHighlight hl;
					hl = (InputMethodHighlight) value;

//JAVA TO C# CONVERTER TODO TASK: Java wildcard generics are not converted to .NET:
//ORIGINAL LINE: java.util.Map<? extends java.text.AttributedCharacterIterator_Attribute, ?> imStyles = null;
					IDictionary<?, ?> imStyles = null;
					try
					{
						imStyles = hl.Style;
					}
					catch (NoSuchMethodError)
					{
					}

					if (imStyles == null)
					{
						Toolkit tk = Toolkit.DefaultToolkit;
						imStyles = tk.MapInputMethodHighlight(hl);
					}

					if (imStyles != null)
					{
						Dictionary<AttributedCharacterIterator_Attribute, Object> newStyles = new Dictionary<AttributedCharacterIterator_Attribute, Object>(5, (float)0.9);
//JAVA TO C# CONVERTER TODO TASK: There is no .NET Dictionary equivalent to the Java 'putAll' method:
						newStyles.putAll(oldStyles);

//JAVA TO C# CONVERTER TODO TASK: There is no .NET Dictionary equivalent to the Java 'putAll' method:
						newStyles.putAll(imStyles);

						return newStyles;
					}
				}
			}
			catch (ClassCastException)
			{
			}

			return oldStyles;
		}

		/// <summary>
		/// Extract a GraphicAttribute or Font from the given attributes.
		/// If attributes does not contain a GraphicAttribute, Font, or
		/// Font family entry this method returns null.
		/// </summary>
		private static Object getGraphicOrFont<T1>(IDictionary<T1> attributes) where T1 : java.text.AttributedCharacterIterator_Attribute
		{

			Object value = attributes[TextAttribute.CHAR_REPLACEMENT];
			if (value != null)
			{
				return value;
			}
			value = attributes[TextAttribute.FONT];
			if (value != null)
			{
				return value;
			}

			if (attributes[TextAttribute.FAMILY] != null)
			{
				return Font.GetFont(attributes);
			}
			else
			{
				return null;
			}
		}
	}

}
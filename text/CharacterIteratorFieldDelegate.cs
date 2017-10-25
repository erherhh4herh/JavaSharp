using System;
using System.Collections.Generic;

/*
 * Copyright (c) 2000, 2012, Oracle and/or its affiliates. All rights reserved.
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
namespace java.text
{

	/// <summary>
	/// CharacterIteratorFieldDelegate combines the notifications from a Format
	/// into a resulting <code>AttributedCharacterIterator</code>. The resulting
	/// <code>AttributedCharacterIterator</code> can be retrieved by way of
	/// the <code>getIterator</code> method.
	/// 
	/// </summary>
	internal class CharacterIteratorFieldDelegate : Format.FieldDelegate
	{
		/// <summary>
		/// Array of AttributeStrings. Whenever <code>formatted</code> is invoked
		/// for a region > size, a new instance of AttributedString is added to
		/// attributedStrings. Subsequent invocations of <code>formatted</code>
		/// for existing regions result in invoking addAttribute on the existing
		/// AttributedStrings.
		/// </summary>
		private List<AttributedString> AttributedStrings;
		/// <summary>
		/// Running count of the number of characters that have
		/// been encountered.
		/// </summary>
		private int Size;


		internal CharacterIteratorFieldDelegate()
		{
			AttributedStrings = new List<>();
		}

		public virtual void Formatted(Format.Field attr, Object value, int start, int end, StringBuffer buffer)
		{
			if (start != end)
			{
				if (start < Size)
				{
					// Adjust attributes of existing runs
					int index = Size;
					int asIndex = AttributedStrings.Count - 1;

					while (start < index)
					{
						AttributedString @as = AttributedStrings[asIndex--];
						int newIndex = index - @as.Length();
						int aStart = System.Math.Max(0, start - newIndex);

						@as.AddAttribute(attr, value, aStart, System.Math.Min(end - start, @as.Length() - aStart) + aStart);
						index = newIndex;
					}
				}
				if (Size < start)
				{
					// Pad attributes
					AttributedStrings.Add(new AttributedString(buffer.Substring(Size, start - Size)));
					Size = start;
				}
				if (Size < end)
				{
					// Add new string
					int aStart = System.Math.Max(start, Size);
					AttributedString @string = new AttributedString(buffer.Substring(aStart, end - aStart));

					@string.AddAttribute(attr, value);
					AttributedStrings.Add(@string);
					Size = end;
				}
			}
		}

		public virtual void Formatted(int fieldID, Format.Field attr, Object value, int start, int end, StringBuffer buffer)
		{
			Formatted(attr, value, start, end, buffer);
		}

		/// <summary>
		/// Returns an <code>AttributedCharacterIterator</code> that can be used
		/// to iterate over the resulting formatted String.
		/// 
		/// @pararm string Result of formatting.
		/// </summary>
		public virtual AttributedCharacterIterator GetIterator(String @string)
		{
			// Add the last AttributedCharacterIterator if necessary
			// assert(size <= string.length());
			if (@string.Length() > Size)
			{
				AttributedStrings.Add(new AttributedString(@string.Substring(Size)));
				Size = @string.Length();
			}
			int iCount = AttributedStrings.Count;
			AttributedCharacterIterator[] iterators = new AttributedCharacterIterator[iCount];

			for (int counter = 0; counter < iCount; counter++)
			{
				iterators[counter] = AttributedStrings[counter].Iterator;
			}
			return (new AttributedString(iterators)).Iterator;
		}
	}

}
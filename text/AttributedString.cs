using System;
using System.Collections.Generic;

/*
 * Copyright (c) 1997, 2012, Oracle and/or its affiliates. All rights reserved.
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
	/// An AttributedString holds text and related attribute information. It
	/// may be used as the actual data storage in some cases where a text
	/// reader wants to access attributed text through the AttributedCharacterIterator
	/// interface.
	/// 
	/// <para>
	/// An attribute is a key/value pair, identified by the key.  No two
	/// attributes on a given character can have the same key.
	/// 
	/// </para>
	/// <para>The values for an attribute are immutable, or must not be mutated
	/// by clients or storage.  They are always passed by reference, and not
	/// cloned.
	/// 
	/// </para>
	/// </summary>
	/// <seealso cref= AttributedCharacterIterator </seealso>
	/// <seealso cref= Annotation
	/// @since 1.2 </seealso>

	public class AttributedString
	{

		// since there are no vectors of int, we have to use arrays.
		// We allocate them in chunks of 10 elements so we don't have to allocate all the time.
		private const int ARRAY_SIZE_INCREMENT = 10;

		// field holding the text
		internal String Text;

		// fields holding run attribute information
		// run attributes are organized by run
		internal int RunArraySize; // current size of the arrays
		internal int RunCount; // actual number of runs, <= runArraySize
		internal int[] RunStarts; // start index for each run
		internal Vector<AttributedCharacterIterator_Attribute>[] RunAttributes; // vector of attribute keys for each run
		internal Vector<Object>[] RunAttributeValues; // parallel vector of attribute values for each run

		/// <summary>
		/// Constructs an AttributedString instance with the given
		/// AttributedCharacterIterators.
		/// </summary>
		/// <param name="iterators"> AttributedCharacterIterators to construct
		/// AttributedString from. </param>
		/// <exception cref="NullPointerException"> if iterators is null </exception>
		internal AttributedString(AttributedCharacterIterator[] iterators)
		{
			if (iterators == null)
			{
				throw new NullPointerException("Iterators must not be null");
			}
			if (iterators.Length == 0)
			{
				Text = "";
			}
			else
			{
				// Build the String contents
				StringBuffer buffer = new StringBuffer();
				for (int counter = 0; counter < iterators.Length; counter++)
				{
					AppendContents(buffer, iterators[counter]);
				}

				Text = buffer.ToString();

				if (Text.Length() > 0)
				{
					// Determine the runs, creating a new run when the attributes
					// differ.
					int offset = 0;
					Map<AttributedCharacterIterator_Attribute, Object> last = null;

					for (int counter = 0; counter < iterators.Length; counter++)
					{
						AttributedCharacterIterator iterator = iterators[counter];
						int start = iterator.BeginIndex;
						int end = iterator.EndIndex;
						int index = start;

						while (index < end)
						{
							iterator.Index = index;

							Map<AttributedCharacterIterator_Attribute, Object> attrs = iterator.Attributes;

							if (MapsDiffer(last, attrs))
							{
								SetAttributes(attrs, index - start + offset);
							}
							last = attrs;
							index = iterator.RunLimit;
						}
						offset += (end - start);
					}
				}
			}
		}

		/// <summary>
		/// Constructs an AttributedString instance with the given text. </summary>
		/// <param name="text"> The text for this attributed string. </param>
		/// <exception cref="NullPointerException"> if <code>text</code> is null. </exception>
		public AttributedString(String text)
		{
			if (text == null)
			{
				throw new NullPointerException();
			}
			this.Text = text;
		}

		/// <summary>
		/// Constructs an AttributedString instance with the given text and attributes. </summary>
		/// <param name="text"> The text for this attributed string. </param>
		/// <param name="attributes"> The attributes that apply to the entire string. </param>
		/// <exception cref="NullPointerException"> if <code>text</code> or
		///            <code>attributes</code> is null. </exception>
		/// <exception cref="IllegalArgumentException"> if the text has length 0
		/// and the attributes parameter is not an empty Map (attributes
		/// cannot be applied to a 0-length range). </exception>
		public AttributedString<T1>(String text, Map<T1> attributes) where T1 : java.text.AttributedCharacterIterator_Attribute
		{
			if (text == null || attributes == null)
			{
				throw new NullPointerException();
			}
			this.Text = text;

			if (text.Length() == 0)
			{
				if (attributes.Empty)
				{
					return;
				}
				throw new IllegalArgumentException("Can't add attribute to 0-length text");
			}

			int attributeCount = attributes.Size();
			if (attributeCount > 0)
			{
				CreateRunAttributeDataVectors();
				Vector<AttributedCharacterIterator_Attribute> newRunAttributes = new Vector<AttributedCharacterIterator_Attribute>(attributeCount);
				Vector<Object> newRunAttributeValues = new Vector<Object>(attributeCount);
				RunAttributes[0] = newRunAttributes;
				RunAttributeValues[0] = newRunAttributeValues;

//JAVA TO C# CONVERTER TODO TASK: Java wildcard generics are not converted to .NET:
//ORIGINAL LINE: Iterator<? extends Map_Entry<? extends java.text.AttributedCharacterIterator_Attribute, ?>> iterator = attributes.entrySet().iterator();
				Iterator<?> iterator = attributes.EntrySet().Iterator();
				while (iterator.HasNext())
				{
//JAVA TO C# CONVERTER TODO TASK: Java wildcard generics are not converted to .NET:
//ORIGINAL LINE: Map_Entry<? extends java.text.AttributedCharacterIterator_Attribute, ?> entry = iterator.next();
					Map_Entry<?, ?> entry = iterator.Next();
					newRunAttributes.AddElement(entry.Key);
					newRunAttributeValues.AddElement(entry.Value);
				}
			}
		}

		/// <summary>
		/// Constructs an AttributedString instance with the given attributed
		/// text represented by AttributedCharacterIterator. </summary>
		/// <param name="text"> The text for this attributed string. </param>
		/// <exception cref="NullPointerException"> if <code>text</code> is null. </exception>
		public AttributedString(AttributedCharacterIterator text) : this(text, text.BeginIndex, text.EndIndex, null)
		{
			// If performance is critical, this constructor should be
			// implemented here rather than invoking the constructor for a
			// subrange. We can avoid some range checking in the loops.
		}

		/// <summary>
		/// Constructs an AttributedString instance with the subrange of
		/// the given attributed text represented by
		/// AttributedCharacterIterator. If the given range produces an
		/// empty text, all attributes will be discarded.  Note that any
		/// attributes wrapped by an Annotation object are discarded for a
		/// subrange of the original attribute range.
		/// </summary>
		/// <param name="text"> The text for this attributed string. </param>
		/// <param name="beginIndex"> Index of the first character of the range. </param>
		/// <param name="endIndex"> Index of the character following the last character
		/// of the range. </param>
		/// <exception cref="NullPointerException"> if <code>text</code> is null. </exception>
		/// <exception cref="IllegalArgumentException"> if the subrange given by
		/// beginIndex and endIndex is out of the text range. </exception>
		/// <seealso cref= java.text.Annotation </seealso>
		public AttributedString(AttributedCharacterIterator text, int beginIndex, int endIndex) : this(text, beginIndex, endIndex, null)
		{
		}

		/// <summary>
		/// Constructs an AttributedString instance with the subrange of
		/// the given attributed text represented by
		/// AttributedCharacterIterator.  Only attributes that match the
		/// given attributes will be incorporated into the instance. If the
		/// given range produces an empty text, all attributes will be
		/// discarded. Note that any attributes wrapped by an Annotation
		/// object are discarded for a subrange of the original attribute
		/// range.
		/// </summary>
		/// <param name="text"> The text for this attributed string. </param>
		/// <param name="beginIndex"> Index of the first character of the range. </param>
		/// <param name="endIndex"> Index of the character following the last character
		/// of the range. </param>
		/// <param name="attributes"> Specifies attributes to be extracted
		/// from the text. If null is specified, all available attributes will
		/// be used. </param>
		/// <exception cref="NullPointerException"> if <code>text</code> is null. </exception>
		/// <exception cref="IllegalArgumentException"> if the subrange given by
		/// beginIndex and endIndex is out of the text range. </exception>
		/// <seealso cref= java.text.Annotation </seealso>
		public AttributedString(AttributedCharacterIterator text, int beginIndex, int endIndex, Attribute[] attributes)
		{
			if (text == null)
			{
				throw new NullPointerException();
			}

			// Validate the given subrange
			int textBeginIndex = text.BeginIndex;
			int textEndIndex = text.EndIndex;
			if (beginIndex < textBeginIndex || endIndex > textEndIndex || beginIndex > endIndex)
			{
				throw new IllegalArgumentException("Invalid substring range");
			}

			// Copy the given string
			StringBuffer textBuffer = new StringBuffer();
			text.Index = beginIndex;
			for (char c = text.Current(); text.Index < endIndex; c = text.Next())
			{
				textBuffer.Append(c);
			}
			this.Text = textBuffer.ToString();

			if (beginIndex == endIndex)
			{
				return;
			}

			// Select attribute keys to be taken care of
			HashSet<AttributedCharacterIterator_Attribute> keys = new HashSet<AttributedCharacterIterator_Attribute>();
			if (attributes == null)
			{
				keys.AddAll(text.AllAttributeKeys);
			}
			else
			{
				for (int i = 0; i < attributes.Length; i++)
				{
					keys.Add(attributes[i]);
				}
				keys.RetainAll(text.AllAttributeKeys);
			}
			if (keys.Empty)
			{
				return;
			}

			// Get and set attribute runs for each attribute name. Need to
			// scan from the top of the text so that we can discard any
			// Annotation that is no longer applied to a subset text segment.
			Iterator<AttributedCharacterIterator_Attribute> itr = keys.Iterator();
			while (itr.HasNext())
			{
				AttributedCharacterIterator_Attribute attributeKey = itr.Next();
				text.Index = textBeginIndex;
				while (text.Index < endIndex)
				{
					int start = text.GetRunStart(attributeKey);
					int limit = text.GetRunLimit(attributeKey);
					Object value = text.GetAttribute(attributeKey);

					if (value != null)
					{
						if (value is Annotation)
						{
							if (start >= beginIndex && limit <= endIndex)
							{
								AddAttribute(attributeKey, value, start - beginIndex, limit - beginIndex);
							}
							else
							{
								if (limit > endIndex)
								{
									break;
								}
							}
						}
						else
						{
							// if the run is beyond the given (subset) range, we
							// don't need to process further.
							if (start >= endIndex)
							{
								break;
							}
							if (limit > beginIndex)
							{
								// attribute is applied to any subrange
								if (start < beginIndex)
								{
									start = beginIndex;
								}
								if (limit > endIndex)
								{
									limit = endIndex;
								}
								if (start != limit)
								{
									AddAttribute(attributeKey, value, start - beginIndex, limit - beginIndex);
								}
							}
						}
					}
					text.Index = limit;
				}
			}
		}

		/// <summary>
		/// Adds an attribute to the entire string. </summary>
		/// <param name="attribute"> the attribute key </param>
		/// <param name="value"> the value of the attribute; may be null </param>
		/// <exception cref="NullPointerException"> if <code>attribute</code> is null. </exception>
		/// <exception cref="IllegalArgumentException"> if the AttributedString has length 0
		/// (attributes cannot be applied to a 0-length range). </exception>
		public virtual void AddAttribute(AttributedCharacterIterator_Attribute attribute, Object value)
		{

			if (attribute == null)
			{
				throw new NullPointerException();
			}

			int len = Length();
			if (len == 0)
			{
				throw new IllegalArgumentException("Can't add attribute to 0-length text");
			}

			AddAttributeImpl(attribute, value, 0, len);
		}

		/// <summary>
		/// Adds an attribute to a subrange of the string. </summary>
		/// <param name="attribute"> the attribute key </param>
		/// <param name="value"> The value of the attribute. May be null. </param>
		/// <param name="beginIndex"> Index of the first character of the range. </param>
		/// <param name="endIndex"> Index of the character following the last character of the range. </param>
		/// <exception cref="NullPointerException"> if <code>attribute</code> is null. </exception>
		/// <exception cref="IllegalArgumentException"> if beginIndex is less then 0, endIndex is
		/// greater than the length of the string, or beginIndex and endIndex together don't
		/// define a non-empty subrange of the string. </exception>
		public virtual void AddAttribute(AttributedCharacterIterator_Attribute attribute, Object value, int beginIndex, int endIndex)
		{

			if (attribute == null)
			{
				throw new NullPointerException();
			}

			if (beginIndex < 0 || endIndex > Length() || beginIndex >= endIndex)
			{
				throw new IllegalArgumentException("Invalid substring range");
			}

			AddAttributeImpl(attribute, value, beginIndex, endIndex);
		}

		/// <summary>
		/// Adds a set of attributes to a subrange of the string. </summary>
		/// <param name="attributes"> The attributes to be added to the string. </param>
		/// <param name="beginIndex"> Index of the first character of the range. </param>
		/// <param name="endIndex"> Index of the character following the last
		/// character of the range. </param>
		/// <exception cref="NullPointerException"> if <code>attributes</code> is null. </exception>
		/// <exception cref="IllegalArgumentException"> if beginIndex is less then
		/// 0, endIndex is greater than the length of the string, or
		/// beginIndex and endIndex together don't define a non-empty
		/// subrange of the string and the attributes parameter is not an
		/// empty Map. </exception>
		public virtual void addAttributes<T1>(Map<T1> attributes, int beginIndex, int endIndex) where T1 : java.text.AttributedCharacterIterator_Attribute
		{
			if (attributes == null)
			{
				throw new NullPointerException();
			}

			if (beginIndex < 0 || endIndex > Length() || beginIndex > endIndex)
			{
				throw new IllegalArgumentException("Invalid substring range");
			}
			if (beginIndex == endIndex)
			{
				if (attributes.Empty)
				{
					return;
				}
				throw new IllegalArgumentException("Can't add attribute to 0-length text");
			}

			// make sure we have run attribute data vectors
			if (RunCount == 0)
			{
				CreateRunAttributeDataVectors();
			}

			// break up runs if necessary
			int beginRunIndex = EnsureRunBreak(beginIndex);
			int endRunIndex = EnsureRunBreak(endIndex);

//JAVA TO C# CONVERTER TODO TASK: Java wildcard generics are not converted to .NET:
//ORIGINAL LINE: Iterator<? extends Map_Entry<? extends java.text.AttributedCharacterIterator_Attribute, ?>> iterator = attributes.entrySet().iterator();
			Iterator<?> iterator = attributes.EntrySet().Iterator();
			while (iterator.HasNext())
			{
//JAVA TO C# CONVERTER TODO TASK: Java wildcard generics are not converted to .NET:
//ORIGINAL LINE: Map_Entry<? extends java.text.AttributedCharacterIterator_Attribute, ?> entry = iterator.next();
				Map_Entry<?, ?> entry = iterator.Next();
				AddAttributeRunData(entry.Key, entry.Value, beginRunIndex, endRunIndex);
			}
		}

		private void AddAttributeImpl(AttributedCharacterIterator_Attribute attribute, Object value, int beginIndex, int endIndex)
		{
			lock (this)
			{
        
				// make sure we have run attribute data vectors
				if (RunCount == 0)
				{
					CreateRunAttributeDataVectors();
				}
        
				// break up runs if necessary
				int beginRunIndex = EnsureRunBreak(beginIndex);
				int endRunIndex = EnsureRunBreak(endIndex);
        
				AddAttributeRunData(attribute, value, beginRunIndex, endRunIndex);
			}
		}

		private void CreateRunAttributeDataVectors()
		{
			// use temporary variables so things remain consistent in case of an exception
			int[] newRunStarts = new int[ARRAY_SIZE_INCREMENT];

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @SuppressWarnings("unchecked") Vector<java.text.AttributedCharacterIterator_Attribute> newRunAttributes[] = (Vector<java.text.AttributedCharacterIterator_Attribute>[]) new Vector<?>[ARRAY_SIZE_INCREMENT];
//JAVA TO C# CONVERTER TODO TASK: Java wildcard generics are not converted to .NET:
			Vector<AttributedCharacterIterator_Attribute>[] newRunAttributes = (Vector<AttributedCharacterIterator_Attribute>[]) new Vector<?>[ARRAY_SIZE_INCREMENT];

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @SuppressWarnings("unchecked") Vector<Object> newRunAttributeValues[] = (Vector<Object>[]) new Vector<?>[ARRAY_SIZE_INCREMENT];
//JAVA TO C# CONVERTER TODO TASK: Java wildcard generics are not converted to .NET:
			Vector<Object>[] newRunAttributeValues = (Vector<Object>[]) new Vector<?>[ARRAY_SIZE_INCREMENT];

			RunStarts = newRunStarts;
			RunAttributes = newRunAttributes;
			RunAttributeValues = newRunAttributeValues;
			RunArraySize = ARRAY_SIZE_INCREMENT;
			RunCount = 1; // assume initial run starting at index 0
		}

		// ensure there's a run break at offset, return the index of the run
		private int EnsureRunBreak(int offset)
		{
			return EnsureRunBreak(offset, true);
		}

		/// <summary>
		/// Ensures there is a run break at offset, returning the index of
		/// the run. If this results in splitting a run, two things can happen:
		/// <ul>
		/// <li>If copyAttrs is true, the attributes from the existing run
		///     will be placed in both of the newly created runs.
		/// <li>If copyAttrs is false, the attributes from the existing run
		/// will NOT be copied to the run to the right (>= offset) of the break,
		/// but will exist on the run to the left (< offset).
		/// </ul>
		/// </summary>
		private int EnsureRunBreak(int offset, bool copyAttrs)
		{
			if (offset == Length())
			{
				return RunCount;
			}

			// search for the run index where this offset should be
			int runIndex = 0;
			while (runIndex < RunCount && RunStarts[runIndex] < offset)
			{
				runIndex++;
			}

			// if the offset is at a run start already, we're done
			if (runIndex < RunCount && RunStarts[runIndex] == offset)
			{
				return runIndex;
			}

			// we'll have to break up a run
			// first, make sure we have enough space in our arrays
			if (RunCount == RunArraySize)
			{
				int newArraySize = RunArraySize + ARRAY_SIZE_INCREMENT;
				int[] newRunStarts = new int[newArraySize];

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @SuppressWarnings("unchecked") Vector<java.text.AttributedCharacterIterator_Attribute> newRunAttributes[] = (Vector<java.text.AttributedCharacterIterator_Attribute>[]) new Vector<?>[newArraySize];
//JAVA TO C# CONVERTER TODO TASK: Java wildcard generics are not converted to .NET:
				Vector<AttributedCharacterIterator_Attribute>[] newRunAttributes = (Vector<AttributedCharacterIterator_Attribute>[]) new Vector<?>[newArraySize];

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @SuppressWarnings("unchecked") Vector<Object> newRunAttributeValues[] = (Vector<Object>[]) new Vector<?>[newArraySize];
//JAVA TO C# CONVERTER TODO TASK: Java wildcard generics are not converted to .NET:
				Vector<Object>[] newRunAttributeValues = (Vector<Object>[]) new Vector<?>[newArraySize];

				for (int i = 0; i < RunArraySize; i++)
				{
					newRunStarts[i] = RunStarts[i];
					newRunAttributes[i] = RunAttributes[i];
					newRunAttributeValues[i] = RunAttributeValues[i];
				}
				RunStarts = newRunStarts;
				RunAttributes = newRunAttributes;
				RunAttributeValues = newRunAttributeValues;
				RunArraySize = newArraySize;
			}

			// make copies of the attribute information of the old run that the new one used to be part of
			// use temporary variables so things remain consistent in case of an exception
			Vector<AttributedCharacterIterator_Attribute> newRunAttributes = null;
			Vector<Object> newRunAttributeValues = null;

			if (copyAttrs)
			{
				Vector<AttributedCharacterIterator_Attribute> oldRunAttributes = RunAttributes[runIndex - 1];
				Vector<Object> oldRunAttributeValues = RunAttributeValues[runIndex - 1];
				if (oldRunAttributes != null)
				{
					newRunAttributes = new Vector<>(oldRunAttributes);
				}
				if (oldRunAttributeValues != null)
				{
					newRunAttributeValues = new Vector<>(oldRunAttributeValues);
				}
			}

			// now actually break up the run
			RunCount++;
			for (int i = RunCount - 1; i > runIndex; i--)
			{
				RunStarts[i] = RunStarts[i - 1];
				RunAttributes[i] = RunAttributes[i - 1];
				RunAttributeValues[i] = RunAttributeValues[i - 1];
			}
			RunStarts[runIndex] = offset;
			RunAttributes[runIndex] = newRunAttributes;
			RunAttributeValues[runIndex] = newRunAttributeValues;

			return runIndex;
		}

		// add the attribute attribute/value to all runs where beginRunIndex <= runIndex < endRunIndex
		private void AddAttributeRunData(AttributedCharacterIterator_Attribute attribute, Object value, int beginRunIndex, int endRunIndex)
		{

			for (int i = beginRunIndex; i < endRunIndex; i++)
			{
				int keyValueIndex = -1; // index of key and value in our vectors; assume we don't have an entry yet
				if (RunAttributes[i] == null)
				{
					Vector<AttributedCharacterIterator_Attribute> newRunAttributes = new Vector<AttributedCharacterIterator_Attribute>();
					Vector<Object> newRunAttributeValues = new Vector<Object>();
					RunAttributes[i] = newRunAttributes;
					RunAttributeValues[i] = newRunAttributeValues;
				}
				else
				{
					// check whether we have an entry already
					keyValueIndex = RunAttributes[i].IndexOf(attribute);
				}

				if (keyValueIndex == -1)
				{
					// create new entry
					int oldSize = RunAttributes[i].Size();
					RunAttributes[i].AddElement(attribute);
					try
					{
						RunAttributeValues[i].AddElement(value);
					}
					catch (Exception)
					{
						RunAttributes[i].Size = oldSize;
						RunAttributeValues[i].Size = oldSize;
					}
				}
				else
				{
					// update existing entry
					RunAttributeValues[i].Set(keyValueIndex, value);
				}
			}
		}

		/// <summary>
		/// Creates an AttributedCharacterIterator instance that provides access to the entire contents of
		/// this string.
		/// </summary>
		/// <returns> An iterator providing access to the text and its attributes. </returns>
		public virtual AttributedCharacterIterator Iterator
		{
			get
			{
				return GetIterator(null, 0, Length());
			}
		}

		/// <summary>
		/// Creates an AttributedCharacterIterator instance that provides access to
		/// selected contents of this string.
		/// Information about attributes not listed in attributes that the
		/// implementor may have need not be made accessible through the iterator.
		/// If the list is null, all available attribute information should be made
		/// accessible.
		/// </summary>
		/// <param name="attributes"> a list of attributes that the client is interested in </param>
		/// <returns> an iterator providing access to the entire text and its selected attributes </returns>
		public virtual AttributedCharacterIterator GetIterator(AttributedCharacterIterator_Attribute[] attributes)
		{
			return GetIterator(attributes, 0, Length());
		}

		/// <summary>
		/// Creates an AttributedCharacterIterator instance that provides access to
		/// selected contents of this string.
		/// Information about attributes not listed in attributes that the
		/// implementor may have need not be made accessible through the iterator.
		/// If the list is null, all available attribute information should be made
		/// accessible.
		/// </summary>
		/// <param name="attributes"> a list of attributes that the client is interested in </param>
		/// <param name="beginIndex"> the index of the first character </param>
		/// <param name="endIndex"> the index of the character following the last character </param>
		/// <returns> an iterator providing access to the text and its attributes </returns>
		/// <exception cref="IllegalArgumentException"> if beginIndex is less then 0,
		/// endIndex is greater than the length of the string, or beginIndex is
		/// greater than endIndex. </exception>
		public virtual AttributedCharacterIterator GetIterator(AttributedCharacterIterator_Attribute[] attributes, int beginIndex, int endIndex)
		{
			return new AttributedStringIterator(this, attributes, beginIndex, endIndex);
		}

		// all (with the exception of length) reading operations are private,
		// since AttributedString instances are accessed through iterators.

		// length is package private so that CharacterIteratorFieldDelegate can
		// access it without creating an AttributedCharacterIterator.
		internal virtual int Length()
		{
			return Text.Length();
		}

		private char CharAt(int index)
		{
			return Text.CharAt(index);
		}

		private Object GetAttribute(AttributedCharacterIterator_Attribute attribute, int runIndex)
		{
			lock (this)
			{
				Vector<AttributedCharacterIterator_Attribute> currentRunAttributes = RunAttributes[runIndex];
				Vector<Object> currentRunAttributeValues = RunAttributeValues[runIndex];
				if (currentRunAttributes == null)
				{
					return null;
				}
				int attributeIndex = currentRunAttributes.IndexOf(attribute);
				if (attributeIndex != -1)
				{
					return currentRunAttributeValues.ElementAt(attributeIndex);
				}
				else
				{
					return null;
				}
			}
		}

		// gets an attribute value, but returns an annotation only if it's range does not extend outside the range beginIndex..endIndex
		private Object GetAttributeCheckRange(AttributedCharacterIterator_Attribute attribute, int runIndex, int beginIndex, int endIndex)
		{
			Object value = GetAttribute(attribute, runIndex);
			if (value is Annotation)
			{
				// need to check whether the annotation's range extends outside the iterator's range
				if (beginIndex > 0)
				{
					int currIndex = runIndex;
					int runStart = RunStarts[currIndex];
					while (runStart >= beginIndex && ValuesMatch(value, GetAttribute(attribute, currIndex - 1)))
					{
						currIndex--;
						runStart = RunStarts[currIndex];
					}
					if (runStart < beginIndex)
					{
						// annotation's range starts before iterator's range
						return null;
					}
				}
				int textLength = Length();
				if (endIndex < textLength)
				{
					int currIndex = runIndex;
					int runLimit = (currIndex < RunCount - 1) ? RunStarts[currIndex + 1] : textLength;
					while (runLimit <= endIndex && ValuesMatch(value, GetAttribute(attribute, currIndex + 1)))
					{
						currIndex++;
						runLimit = (currIndex < RunCount - 1) ? RunStarts[currIndex + 1] : textLength;
					}
					if (runLimit > endIndex)
					{
						// annotation's range ends after iterator's range
						return null;
					}
				}
				// annotation's range is subrange of iterator's range,
				// so we can return the value
			}
			return value;
		}

		// returns whether all specified attributes have equal values in the runs with the given indices
		private bool attributeValuesMatch<T1>(Set<T1> attributes, int runIndex1, int runIndex2) where T1 : java.text.AttributedCharacterIterator_Attribute
		{
//JAVA TO C# CONVERTER TODO TASK: Java wildcard generics are not converted to .NET:
//ORIGINAL LINE: Iterator<? extends java.text.AttributedCharacterIterator_Attribute> iterator = attributes.iterator();
			Iterator<?> iterator = attributes.Iterator();
			while (iterator.HasNext())
			{
				AttributedCharacterIterator_Attribute key = iterator.Next();
			   if (!ValuesMatch(GetAttribute(key, runIndex1), GetAttribute(key, runIndex2)))
			   {
					return false;
			   }
			}
			return true;
		}

		// returns whether the two objects are either both null or equal
		private static bool ValuesMatch(Object value1, Object value2)
		{
			if (value1 == null)
			{
				return value2 == null;
			}
			else
			{
				return value1.Equals(value2);
			}
		}

		/// <summary>
		/// Appends the contents of the CharacterIterator iterator into the
		/// StringBuffer buf.
		/// </summary>
		private void AppendContents(StringBuffer buf, CharacterIterator iterator)
		{
			int index = iterator.BeginIndex;
			int end = iterator.EndIndex;

			while (index < end)
			{
				iterator.Index = index++;
				buf.Append(iterator.Current());
			}
		}

		/// <summary>
		/// Sets the attributes for the range from offset to the next run break
		/// (typically the end of the text) to the ones specified in attrs.
		/// This is only meant to be called from the constructor!
		/// </summary>
		private void SetAttributes(Map<AttributedCharacterIterator_Attribute, Object> attrs, int offset)
		{
			if (RunCount == 0)
			{
				CreateRunAttributeDataVectors();
			}

			int index = EnsureRunBreak(offset, false);
			int size;

			if (attrs != null && (size = attrs.Size()) > 0)
			{
				Vector<AttributedCharacterIterator_Attribute> runAttrs = new Vector<AttributedCharacterIterator_Attribute>(size);
				Vector<Object> runValues = new Vector<Object>(size);
				Iterator<Map_Entry<AttributedCharacterIterator_Attribute, Object>> iterator = attrs.EntrySet().Iterator();

				while (iterator.HasNext())
				{
					Map_Entry<AttributedCharacterIterator_Attribute, Object> entry = iterator.Next();

					runAttrs.Add(entry.Key);
					runValues.Add(entry.Value);
				}
				RunAttributes[index] = runAttrs;
				RunAttributeValues[index] = runValues;
			}
		}

		/// <summary>
		/// Returns true if the attributes specified in last and attrs differ.
		/// </summary>
		private static bool mapsDiffer<K, V>(Map<K, V> last, Map<K, V> attrs)
		{
			if (last == null)
			{
				return (attrs != null && attrs.Size() > 0);
			}
			return (!last.Equals(attrs));
		}


		// the iterator class associated with this string class

		private sealed class AttributedStringIterator : AttributedCharacterIterator
		{
			private readonly AttributedString OuterInstance;


			// note on synchronization:
			// we don't synchronize on the iterator, assuming that an iterator is only used in one thread.
			// we do synchronize access to the AttributedString however, since it's more likely to be shared between threads.

			// start and end index for our iteration
			internal int BeginIndex_Renamed;
			internal int EndIndex_Renamed;

			// attributes that our client is interested in
			internal AttributedCharacterIterator_Attribute[] RelevantAttributes;

			// the current index for our iteration
			// invariant: beginIndex <= currentIndex <= endIndex
			internal int CurrentIndex;

			// information about the run that includes currentIndex
			internal int CurrentRunIndex;
			internal int CurrentRunStart;
			internal int CurrentRunLimit;

			// constructor
			internal AttributedStringIterator(AttributedString outerInstance, AttributedCharacterIterator_Attribute[] attributes, int beginIndex, int endIndex)
			{
				this.OuterInstance = outerInstance;

				if (beginIndex < 0 || beginIndex > endIndex || endIndex > outerInstance.Length())
				{
					throw new IllegalArgumentException("Invalid substring range");
				}

				this.BeginIndex_Renamed = beginIndex;
				this.EndIndex_Renamed = endIndex;
				this.CurrentIndex = beginIndex;
				UpdateRunInfo();
				if (attributes != null)
				{
					RelevantAttributes = attributes.clone();
				}
			}

			// Object methods. See documentation in that class.

			public override bool Equals(Object obj)
			{
				if (this == obj)
				{
					return true;
				}
				if (!(obj is AttributedStringIterator))
				{
					return false;
				}

				AttributedStringIterator that = (AttributedStringIterator) obj;

				if (OuterInstance != that.String)
				{
					return false;
				}
				if (CurrentIndex != that.CurrentIndex || BeginIndex_Renamed != that.BeginIndex_Renamed || EndIndex_Renamed != that.EndIndex_Renamed)
				{
					return false;
				}
				return true;
			}

			public override int HashCode()
			{
				return outerInstance.Text.HashCode() ^ CurrentIndex ^ BeginIndex_Renamed ^ EndIndex_Renamed;
			}

			public Object Clone()
			{
				try
				{
					AttributedStringIterator other = (AttributedStringIterator) base.Clone();
					return other;
				}
				catch (CloneNotSupportedException e)
				{
					throw new InternalError(e);
				}
			}

			// CharacterIterator methods. See documentation in that interface.

			public char First()
			{
				return InternalSetIndex(BeginIndex_Renamed);
			}

			public char Last()
			{
				if (EndIndex_Renamed == BeginIndex_Renamed)
				{
					return InternalSetIndex(EndIndex_Renamed);
				}
				else
				{
					return InternalSetIndex(EndIndex_Renamed - 1);
				}
			}

			public char Current()
			{
				if (CurrentIndex == EndIndex_Renamed)
				{
					return CharacterIterator_Fields.DONE;
				}
				else
				{
					return outerInstance.CharAt(CurrentIndex);
				}
			}

			public char Next()
			{
				if (CurrentIndex < EndIndex_Renamed)
				{
					return InternalSetIndex(CurrentIndex + 1);
				}
				else
				{
					return CharacterIterator_Fields.DONE;
				}
			}

			public char Previous()
			{
				if (CurrentIndex > BeginIndex_Renamed)
				{
					return InternalSetIndex(CurrentIndex - 1);
				}
				else
				{
					return CharacterIterator_Fields.DONE;
				}
			}

			public char SetIndex(int position)
			{
				if (position < BeginIndex_Renamed || position > EndIndex_Renamed)
				{
					throw new IllegalArgumentException("Invalid index");
				}
				return InternalSetIndex(position);
			}

			public int BeginIndex
			{
				get
				{
					return BeginIndex_Renamed;
				}
			}

			public int EndIndex
			{
				get
				{
					return EndIndex_Renamed;
				}
			}

			public int Index
			{
				get
				{
					return CurrentIndex;
				}
			}

			// AttributedCharacterIterator methods. See documentation in that interface.

			public int RunStart
			{
				get
				{
					return CurrentRunStart;
				}
			}

			public int GetRunStart(AttributedCharacterIterator_Attribute attribute)
			{
				if (CurrentRunStart == BeginIndex_Renamed || CurrentRunIndex == -1)
				{
					return CurrentRunStart;
				}
				else
				{
					Object value = GetAttribute(attribute);
					int runStart = CurrentRunStart;
					int runIndex = CurrentRunIndex;
					while (runStart > BeginIndex_Renamed && ValuesMatch(value, OuterInstance.GetAttribute(attribute, runIndex - 1)))
					{
						runIndex--;
						runStart = outerInstance.RunStarts[runIndex];
					}
					if (runStart < BeginIndex_Renamed)
					{
						runStart = BeginIndex_Renamed;
					}
					return runStart;
				}
			}

			public int getRunStart<T1>(Set<T1> attributes) where T1 : java.text.AttributedCharacterIterator_Attribute
			{
				if (CurrentRunStart == BeginIndex_Renamed || CurrentRunIndex == -1)
				{
					return CurrentRunStart;
				}
				else
				{
					int runStart = CurrentRunStart;
					int runIndex = CurrentRunIndex;
					while (runStart > BeginIndex_Renamed && OuterInstance.AttributeValuesMatch(attributes, CurrentRunIndex, runIndex - 1))
					{
						runIndex--;
						runStart = outerInstance.RunStarts[runIndex];
					}
					if (runStart < BeginIndex_Renamed)
					{
						runStart = BeginIndex_Renamed;
					}
					return runStart;
				}
			}

			public int RunLimit
			{
				get
				{
					return CurrentRunLimit;
				}
			}

			public int GetRunLimit(AttributedCharacterIterator_Attribute attribute)
			{
				if (CurrentRunLimit == EndIndex_Renamed || CurrentRunIndex == -1)
				{
					return CurrentRunLimit;
				}
				else
				{
					Object value = GetAttribute(attribute);
					int runLimit = CurrentRunLimit;
					int runIndex = CurrentRunIndex;
					while (runLimit < EndIndex_Renamed && ValuesMatch(value, OuterInstance.GetAttribute(attribute, runIndex + 1)))
					{
						runIndex++;
						runLimit = runIndex < outerInstance.RunCount - 1 ? outerInstance.RunStarts[runIndex + 1] : EndIndex_Renamed;
					}
					if (runLimit > EndIndex_Renamed)
					{
						runLimit = EndIndex_Renamed;
					}
					return runLimit;
				}
			}

			public int getRunLimit<T1>(Set<T1> attributes) where T1 : java.text.AttributedCharacterIterator_Attribute
			{
				if (CurrentRunLimit == EndIndex_Renamed || CurrentRunIndex == -1)
				{
					return CurrentRunLimit;
				}
				else
				{
					int runLimit = CurrentRunLimit;
					int runIndex = CurrentRunIndex;
					while (runLimit < EndIndex_Renamed && OuterInstance.AttributeValuesMatch(attributes, CurrentRunIndex, runIndex + 1))
					{
						runIndex++;
						runLimit = runIndex < outerInstance.RunCount - 1 ? outerInstance.RunStarts[runIndex + 1] : EndIndex_Renamed;
					}
					if (runLimit > EndIndex_Renamed)
					{
						runLimit = EndIndex_Renamed;
					}
					return runLimit;
				}
			}

			public Map<AttributedCharacterIterator_Attribute, Object> Attributes
			{
				get
				{
					if (outerInstance.RunAttributes == null || CurrentRunIndex == -1 || outerInstance.RunAttributes[CurrentRunIndex] == null)
					{
						// ??? would be nice to return null, but current spec doesn't allow it
						// returning Hashtable saves AttributeMap from dealing with emptiness
						return new Dictionary<>();
					}
					return new AttributeMap(OuterInstance, CurrentRunIndex, BeginIndex_Renamed, EndIndex_Renamed);
				}
			}

			public Set<AttributedCharacterIterator_Attribute> AllAttributeKeys
			{
				get
				{
					// ??? This should screen out attribute keys that aren't relevant to the client
					if (outerInstance.RunAttributes == null)
					{
						// ??? would be nice to return null, but current spec doesn't allow it
						// returning HashSet saves us from dealing with emptiness
						return new HashSet<>();
					}
					lock (OuterInstance)
					{
						// ??? should try to create this only once, then update if necessary,
						// and give callers read-only view
						Set<AttributedCharacterIterator_Attribute> keys = new HashSet<AttributedCharacterIterator_Attribute>();
						int i = 0;
						while (i < outerInstance.RunCount)
						{
							if (outerInstance.RunStarts[i] < EndIndex_Renamed && (i == outerInstance.RunCount - 1 || outerInstance.RunStarts[i + 1] > BeginIndex_Renamed))
							{
								Vector<AttributedCharacterIterator_Attribute> currentRunAttributes = outerInstance.RunAttributes[i];
								if (currentRunAttributes != null)
								{
									int j = currentRunAttributes.Size();
									while (j-- > 0)
									{
										keys.Add(currentRunAttributes.Get(j));
									}
								}
							}
							i++;
						}
						return keys;
					}
				}
			}

			public Object GetAttribute(AttributedCharacterIterator_Attribute attribute)
			{
				int runIndex = CurrentRunIndex;
				if (runIndex < 0)
				{
					return null;
				}
				return OuterInstance.GetAttributeCheckRange(attribute, runIndex, BeginIndex_Renamed, EndIndex_Renamed);
			}

			// internally used methods

			internal AttributedString String
			{
				get
				{
					return OuterInstance;
				}
			}

			// set the current index, update information about the current run if necessary,
			// return the character at the current index
			internal char InternalSetIndex(int position)
			{
				CurrentIndex = position;
				if (position < CurrentRunStart || position >= CurrentRunLimit)
				{
					UpdateRunInfo();
				}
				if (CurrentIndex == EndIndex_Renamed)
				{
					return CharacterIterator_Fields.DONE;
				}
				else
				{
					return outerInstance.CharAt(position);
				}
			}

			// update the information about the current run
			internal void UpdateRunInfo()
			{
				if (CurrentIndex == EndIndex_Renamed)
				{
					CurrentRunStart = CurrentRunLimit = EndIndex_Renamed;
					CurrentRunIndex = -1;
				}
				else
				{
					lock (OuterInstance)
					{
						int runIndex = -1;
						while (runIndex < outerInstance.RunCount - 1 && outerInstance.RunStarts[runIndex + 1] <= CurrentIndex)
						{
							runIndex++;
						}
						CurrentRunIndex = runIndex;
						if (runIndex >= 0)
						{
							CurrentRunStart = outerInstance.RunStarts[runIndex];
							if (CurrentRunStart < BeginIndex_Renamed)
							{
								CurrentRunStart = BeginIndex_Renamed;
							}
						}
						else
						{
							CurrentRunStart = BeginIndex_Renamed;
						}
						if (runIndex < outerInstance.RunCount - 1)
						{
							CurrentRunLimit = outerInstance.RunStarts[runIndex + 1];
							if (CurrentRunLimit > EndIndex_Renamed)
							{
								CurrentRunLimit = EndIndex_Renamed;
							}
						}
						else
						{
							CurrentRunLimit = EndIndex_Renamed;
						}
					}
				}
			}

		}

		// the map class associated with this string class, giving access to the attributes of one run

		private sealed class AttributeMap : AbstractMap<AttributedCharacterIterator_Attribute, Object>
		{
			private readonly AttributedString OuterInstance;


			internal int RunIndex;
			internal int BeginIndex;
			internal int EndIndex;

			internal AttributeMap(AttributedString outerInstance, int runIndex, int beginIndex, int endIndex)
			{
				this.OuterInstance = outerInstance;
				this.RunIndex = runIndex;
				this.BeginIndex = beginIndex;
				this.EndIndex = endIndex;
			}

			public Set<Map_Entry<AttributedCharacterIterator_Attribute, Object>> EntrySet()
			{
				HashSet<Map_Entry<AttributedCharacterIterator_Attribute, Object>> set = new HashSet<Map_Entry<AttributedCharacterIterator_Attribute, Object>>();
				lock (OuterInstance)
				{
					int size = outerInstance.RunAttributes[RunIndex].Size();
					for (int i = 0; i < size; i++)
					{
						AttributedCharacterIterator_Attribute key = outerInstance.RunAttributes[RunIndex].Get(i);
						Object value = outerInstance.RunAttributeValues[RunIndex].Get(i);
						if (value is Annotation)
						{
							value = OuterInstance.GetAttributeCheckRange(key, RunIndex, BeginIndex, EndIndex);
							if (value == java.util.Map_Fields.Null)
							{
								continue;
							}
						}

						Map_Entry<AttributedCharacterIterator_Attribute, Object> entry = new AttributeEntry(key, value);
						set.Add(entry);
					}
				}
				return set;
			}

			public Object Get(Object key)
			{
				return OuterInstance.GetAttributeCheckRange((AttributedCharacterIterator_Attribute) key, RunIndex, BeginIndex, EndIndex);
			}
		}
	}

	internal class AttributeEntry : Map_Entry<AttributedCharacterIterator_Attribute, Object>
	{

		private AttributedCharacterIterator_Attribute Key_Renamed;
		private Object Value_Renamed;

		internal AttributeEntry(AttributedCharacterIterator_Attribute key, Object value)
		{
			this.Key_Renamed = key;
			this.Value_Renamed = value;
		}

		public override bool Equals(Object o)
		{
			if (!(o is AttributeEntry))
			{
				return false;
			}
			AttributeEntry other = (AttributeEntry) o;
			return other.Key_Renamed.Equals(Key_Renamed) && (Value_Renamed == null ? other.Value_Renamed == null : other.Value_Renamed.Equals(Value_Renamed));
		}

		public virtual AttributedCharacterIterator_Attribute Key
		{
			get
			{
				return Key_Renamed;
			}
		}

		public virtual Object Value
		{
			get
			{
				return Value_Renamed;
			}
		}

		public virtual Object SetValue(Object newValue)
		{
			throw new UnsupportedOperationException();
		}

		public override int HashCode()
		{
			return Key_Renamed.HashCode() ^ (Value_Renamed == null ? 0 : Value_Renamed.HashCode());
		}

		public override String ToString()
		{
			return Key_Renamed.ToString() + "=" + Value_Renamed.ToString();
		}
	}

}
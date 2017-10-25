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

namespace java.text
{

	/// <summary>
	/// An Annotation object is used as a wrapper for a text attribute value if
	/// the attribute has annotation characteristics. These characteristics are:
	/// <ul>
	/// <li>The text range that the attribute is applied to is critical to the
	/// semantics of the range. That means, the attribute cannot be applied to subranges
	/// of the text range that it applies to, and, if two adjacent text ranges have
	/// the same value for this attribute, the attribute still cannot be applied to
	/// the combined range as a whole with this value.
	/// <li>The attribute or its value usually do no longer apply if the underlying text is
	/// changed.
	/// </ul>
	/// 
	/// An example is grammatical information attached to a sentence:
	/// For the previous sentence, you can say that "an example"
	/// is the subject, but you cannot say the same about "an", "example", or "exam".
	/// When the text is changed, the grammatical information typically becomes invalid.
	/// Another example is Japanese reading information (yomi).
	/// 
	/// <para>
	/// Wrapping the attribute value into an Annotation object guarantees that
	/// adjacent text runs don't get merged even if the attribute values are equal,
	/// and indicates to text containers that the attribute should be discarded if
	/// the underlying text is modified.
	/// 
	/// </para>
	/// </summary>
	/// <seealso cref= AttributedCharacterIterator
	/// @since 1.2 </seealso>

	public class Annotation
	{

		/// <summary>
		/// Constructs an annotation record with the given value, which
		/// may be null.
		/// </summary>
		/// <param name="value"> the value of the attribute </param>
		public Annotation(Object value)
		{
			this.Value_Renamed = value;
		}

		/// <summary>
		/// Returns the value of the attribute, which may be null.
		/// </summary>
		/// <returns> the value of the attribute </returns>
		public virtual Object Value
		{
			get
			{
				return Value_Renamed;
			}
		}

		/// <summary>
		/// Returns the String representation of this Annotation.
		/// </summary>
		/// <returns> the {@code String} representation of this {@code Annotation} </returns>
		public override String ToString()
		{
//JAVA TO C# CONVERTER WARNING: The .NET Type.FullName property will not always yield results identical to the Java Class.getName method:
			return this.GetType().FullName + "[value=" + Value_Renamed + "]";
		}

		private Object Value_Renamed;

	}

}
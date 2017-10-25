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

namespace java.text
{


	/// <summary>
	/// An {@code AttributedCharacterIterator} allows iteration through both text and
	/// related attribute information.
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
	/// <para>A <em>run with respect to an attribute</em> is a maximum text range for
	/// which:
	/// <ul>
	/// <li>the attribute is undefined or {@code null} for the entire range, or
	/// <li>the attribute value is defined and has the same non-{@code null} value for the
	///     entire range.
	/// </ul>
	/// 
	/// </para>
	/// <para>A <em>run with respect to a set of attributes</em> is a maximum text range for
	/// which this condition is met for each member attribute.
	/// 
	/// </para>
	/// <para>When getting a run with no explicit attributes specified (i.e.,
	/// calling <seealso cref="#getRunStart()"/> and <seealso cref="#getRunLimit()"/>), any
	/// contiguous text segments having the same attributes (the same set
	/// of attribute/value pairs) are treated as separate runs if the
	/// attributes have been given to those text segments separately.
	/// 
	/// </para>
	/// <para>The returned indexes are limited to the range of the iterator.
	/// 
	/// </para>
	/// <para>The returned attribute information is limited to runs that contain
	/// the current character.
	/// 
	/// </para>
	/// <para>
	/// Attribute keys are instances of <seealso cref="AttributedCharacterIterator.Attribute"/> and its
	/// subclasses, such as <seealso cref="java.awt.font.TextAttribute"/>.
	/// 
	/// </para>
	/// </summary>
	/// <seealso cref= AttributedCharacterIterator.Attribute </seealso>
	/// <seealso cref= java.awt.font.TextAttribute </seealso>
	/// <seealso cref= AttributedString </seealso>
	/// <seealso cref= Annotation
	/// @since 1.2 </seealso>

	public interface AttributedCharacterIterator : CharacterIterator
	{

		/// <summary>
		/// Defines attribute keys that are used to identify text attributes. These
		/// keys are used in {@code AttributedCharacterIterator} and {@code AttributedString}. </summary>
		/// <seealso cref= AttributedCharacterIterator </seealso>
		/// <seealso cref= AttributedString
		/// @since 1.2 </seealso>

		/// <summary>
		/// Returns the index of the first character of the run
		/// with respect to all attributes containing the current character.
		/// 
		/// <para>Any contiguous text segments having the same attributes (the
		/// same set of attribute/value pairs) are treated as separate runs
		/// if the attributes have been given to those text segments separately.
		/// 
		/// </para>
		/// </summary>
		/// <returns> the index of the first character of the run </returns>
		int RunStart {get;}

		/// <summary>
		/// Returns the index of the first character of the run
		/// with respect to the given {@code attribute} containing the current character.
		/// </summary>
		/// <param name="attribute"> the desired attribute. </param>
		/// <returns> the index of the first character of the run </returns>
		int GetRunStart(AttributedCharacterIterator_Attribute attribute);

		/// <summary>
		/// Returns the index of the first character of the run
		/// with respect to the given {@code attributes} containing the current character.
		/// </summary>
		/// <param name="attributes"> a set of the desired attributes. </param>
		/// <returns> the index of the first character of the run </returns>
		int getRunStart<T1>(Set<T1> attributes) where T1 : AttributedCharacterIterator_Attribute;

		/// <summary>
		/// Returns the index of the first character following the run
		/// with respect to all attributes containing the current character.
		/// 
		/// <para>Any contiguous text segments having the same attributes (the
		/// same set of attribute/value pairs) are treated as separate runs
		/// if the attributes have been given to those text segments separately.
		/// 
		/// </para>
		/// </summary>
		/// <returns> the index of the first character following the run </returns>
		int RunLimit {get;}

		/// <summary>
		/// Returns the index of the first character following the run
		/// with respect to the given {@code attribute} containing the current character.
		/// </summary>
		/// <param name="attribute"> the desired attribute </param>
		/// <returns> the index of the first character following the run </returns>
		int GetRunLimit(AttributedCharacterIterator_Attribute attribute);

		/// <summary>
		/// Returns the index of the first character following the run
		/// with respect to the given {@code attributes} containing the current character.
		/// </summary>
		/// <param name="attributes"> a set of the desired attributes </param>
		/// <returns> the index of the first character following the run </returns>
		int getRunLimit<T1>(Set<T1> attributes) where T1 : AttributedCharacterIterator_Attribute;

		/// <summary>
		/// Returns a map with the attributes defined on the current
		/// character.
		/// </summary>
		/// <returns> a map with the attributes defined on the current character </returns>
		IDictionary<AttributedCharacterIterator_Attribute, Object> Attributes {get;}

		/// <summary>
		/// Returns the value of the named {@code attribute} for the current character.
		/// Returns {@code null} if the {@code attribute} is not defined.
		/// </summary>
		/// <param name="attribute"> the desired attribute </param>
		/// <returns> the value of the named {@code attribute} or {@code null} </returns>
		Object GetAttribute(AttributedCharacterIterator_Attribute attribute);

		/// <summary>
		/// Returns the keys of all attributes defined on the
		/// iterator's text range. The set is empty if no
		/// attributes are defined.
		/// </summary>
		/// <returns> the keys of all attributes </returns>
		Set<AttributedCharacterIterator_Attribute> AllAttributeKeys {get;}
	}

	[Serializable]
	public class AttributedCharacterIterator_Attribute
	{

		/// <summary>
		/// The name of this {@code Attribute}. The name is used primarily by {@code readResolve}
		/// to look up the corresponding predefined instance when deserializing
		/// an instance.
		/// @serial
		/// </summary>
		internal String Name_Renamed;

		// table of all instances in this class, used by readResolve
		internal static readonly IDictionary<String, AttributedCharacterIterator_Attribute> InstanceMap = new Dictionary<String, AttributedCharacterIterator_Attribute>(7);

		/// <summary>
		/// Constructs an {@code Attribute} with the given name.
		/// </summary>
		/// <param name="name"> the name of {@code Attribute} </param>
		protected internal AttributedCharacterIterator_Attribute(String name)
		{
			this.Name_Renamed = name;
			if (this.GetType() == typeof(AttributedCharacterIterator_Attribute))
			{
				InstanceMap[name] = this;
			}
		}

		/// <summary>
		/// Compares two objects for equality. This version only returns true
		/// for {@code x.equals(y)} if {@code x} and {@code y} refer
		/// to the same object, and guarantees this for all subclasses.
		/// </summary>
		public sealed override bool Equals(Object obj)
		{
			return base.Equals(obj);
		}

		/// <summary>
		/// Returns a hash code value for the object. This version is identical to
		/// the one in {@code Object}, but is also final.
		/// </summary>
		public sealed override int HashCode()
		{
			return base.HashCode();
		}

		/// <summary>
		/// Returns a string representation of the object. This version returns the
		/// concatenation of class name, {@code "("}, a name identifying the attribute
		/// and {@code ")"}.
		/// </summary>
		public override String ToString()
		{
//JAVA TO C# CONVERTER WARNING: The .NET Type.FullName property will not always yield results identical to the Java Class.getName method:
			return this.GetType().FullName + "(" + Name_Renamed + ")";
		}

		/// <summary>
		/// Returns the name of the attribute.
		/// </summary>
		/// <returns> the name of {@code Attribute} </returns>
		protected internal virtual String Name
		{
			get
			{
				return Name_Renamed;
			}
		}

		/// <summary>
		/// Resolves instances being deserialized to the predefined constants.
		/// </summary>
		/// <returns> the resolved {@code Attribute} object </returns>
		/// <exception cref="InvalidObjectException"> if the object to resolve is not
		///                                an instance of {@code Attribute} </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: protected Object readResolve() throws java.io.InvalidObjectException
		protected internal virtual Object ReadResolve()
		{
			if (this.GetType() != typeof(AttributedCharacterIterator_Attribute))
			{
				throw new InvalidObjectException("subclass didn't correctly implement readResolve");
			}

			AttributedCharacterIterator_Attribute instance = InstanceMap[Name];
			if (instance != null)
			{
				return instance;
			}
			else
			{
				throw new InvalidObjectException("unknown attribute name");
			}
		}

		/// <summary>
		/// Attribute key for the language of some text.
		/// <para> Values are instances of <seealso cref="java.util.Locale Locale"/>.
		/// </para>
		/// </summary>
		/// <seealso cref= java.util.Locale </seealso>
		public static readonly AttributedCharacterIterator_Attribute LANGUAGE = new AttributedCharacterIterator_Attribute("language");

		/// <summary>
		/// Attribute key for the reading of some text. In languages where the written form
		/// and the pronunciation of a word are only loosely related (such as Japanese),
		/// it is often necessary to store the reading (pronunciation) along with the
		/// written form.
		/// <para>Values are instances of <seealso cref="Annotation"/> holding instances of <seealso cref="String"/>.
		/// 
		/// </para>
		/// </summary>
		/// <seealso cref= Annotation </seealso>
		/// <seealso cref= java.lang.String </seealso>
		public static readonly AttributedCharacterIterator_Attribute READING = new AttributedCharacterIterator_Attribute("reading");

		/// <summary>
		/// Attribute key for input method segments. Input methods often break
		/// up text into segments, which usually correspond to words.
		/// <para>Values are instances of <seealso cref="Annotation"/> holding a {@code null} reference.
		/// </para>
		/// </summary>
		/// <seealso cref= Annotation </seealso>
		public static readonly AttributedCharacterIterator_Attribute INPUT_METHOD_SEGMENT = new AttributedCharacterIterator_Attribute("input_method_segment");

		// make sure the serial version doesn't change between compiler versions
		internal const long SerialVersionUID = -9142742483513960612L;

	}

}
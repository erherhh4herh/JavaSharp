using System;

/*
 * Copyright (c) 2003, 2011, Oracle and/or its affiliates. All rights reserved.
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

namespace java.lang
{


	/// <summary>
	/// This is the common base class of all Java language enumeration types.
	/// 
	/// More information about enums, including descriptions of the
	/// implicitly declared methods synthesized by the compiler, can be
	/// found in section 8.9 of
	/// <cite>The Java&trade; Language Specification</cite>.
	/// 
	/// <para> Note that when using an enumeration type as the type of a set
	/// or as the type of the keys in a map, specialized and efficient
	/// <seealso cref="java.util.EnumSet set"/> and {@linkplain
	/// java.util.EnumMap map} implementations are available.
	/// 
	/// </para>
	/// </summary>
	/// @param <E> The enum type subclass
	/// @author  Josh Bloch
	/// @author  Neal Gafter </param>
	/// <seealso cref=     Class#getEnumConstants() </seealso>
	/// <seealso cref=     java.util.EnumSet </seealso>
	/// <seealso cref=     java.util.EnumMap
	/// @since   1.5 </seealso>
	[Serializable]
	public abstract class Enum<E> : Comparable<E> where E : Enum<E>
	{
		/// <summary>
		/// The name of this enum constant, as declared in the enum declaration.
		/// Most programmers should use the <seealso cref="#toString"/> method rather than
		/// accessing this field.
		/// </summary>
		private readonly String Name_Renamed;

		/// <summary>
		/// Returns the name of this enum constant, exactly as declared in its
		/// enum declaration.
		/// 
		/// <b>Most programmers should use the <seealso cref="#toString"/> method in
		/// preference to this one, as the toString method may return
		/// a more user-friendly name.</b>  This method is designed primarily for
		/// use in specialized situations where correctness depends on getting the
		/// exact name, which will not vary from release to release.
		/// </summary>
		/// <returns> the name of this enum constant </returns>
		public String Name()
		{
			return Name_Renamed;
		}

		/// <summary>
		/// The ordinal of this enumeration constant (its position
		/// in the enum declaration, where the initial constant is assigned
		/// an ordinal of zero).
		/// 
		/// Most programmers will have no use for this field.  It is designed
		/// for use by sophisticated enum-based data structures, such as
		/// <seealso cref="java.util.EnumSet"/> and <seealso cref="java.util.EnumMap"/>.
		/// </summary>
		private readonly int Ordinal_Renamed;

		/// <summary>
		/// Returns the ordinal of this enumeration constant (its position
		/// in its enum declaration, where the initial constant is assigned
		/// an ordinal of zero).
		/// 
		/// Most programmers will have no use for this method.  It is
		/// designed for use by sophisticated enum-based data structures, such
		/// as <seealso cref="java.util.EnumSet"/> and <seealso cref="java.util.EnumMap"/>.
		/// </summary>
		/// <returns> the ordinal of this enumeration constant </returns>
		public int Ordinal()
		{
			return Ordinal_Renamed;
		}

		/// <summary>
		/// Sole constructor.  Programmers cannot invoke this constructor.
		/// It is for use by code emitted by the compiler in response to
		/// enum type declarations.
		/// </summary>
		/// <param name="name"> - The name of this enum constant, which is the identifier
		///               used to declare it. </param>
		/// <param name="ordinal"> - The ordinal of this enumeration constant (its position
		///         in the enum declaration, where the initial constant is assigned
		///         an ordinal of zero). </param>
		protected internal Enum(String name, int ordinal)
		{
			this.Name_Renamed = name;
			this.Ordinal_Renamed = ordinal;
		}

		/// <summary>
		/// Returns the name of this enum constant, as contained in the
		/// declaration.  This method may be overridden, though it typically
		/// isn't necessary or desirable.  An enum type should override this
		/// method when a more "programmer-friendly" string form exists.
		/// </summary>
		/// <returns> the name of this enum constant </returns>
		public override String ToString()
		{
			return Name_Renamed;
		}

		/// <summary>
		/// Returns true if the specified object is equal to this
		/// enum constant.
		/// </summary>
		/// <param name="other"> the object to be compared for equality with this object. </param>
		/// <returns>  true if the specified object is equal to this
		///          enum constant. </returns>
		public sealed override bool Equals(Object other)
		{
			return this == other;
		}

		/// <summary>
		/// Returns a hash code for this enum constant.
		/// </summary>
		/// <returns> a hash code for this enum constant. </returns>
		public sealed override int HashCode()
		{
			return base.HashCode();
		}

		/// <summary>
		/// Throws CloneNotSupportedException.  This guarantees that enums
		/// are never cloned, which is necessary to preserve their "singleton"
		/// status.
		/// </summary>
		/// <returns> (never returns) </returns>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: protected final Object clone() throws CloneNotSupportedException
		protected internal Object Clone()
		{
			throw new CloneNotSupportedException();
		}

		/// <summary>
		/// Compares this enum with the specified object for order.  Returns a
		/// negative integer, zero, or a positive integer as this object is less
		/// than, equal to, or greater than the specified object.
		/// 
		/// Enum constants are only comparable to other enum constants of the
		/// same enum type.  The natural order implemented by this
		/// method is the order in which the constants are declared.
		/// </summary>
		public int CompareTo(E o)
		{
//JAVA TO C# CONVERTER TODO TASK: Java wildcard generics are not converted to .NET:
//ORIGINAL LINE: Enum<?> other = (Enum<?>)o;
			Enum<?> other = (Enum<?>)o;
			Enum<E> self = this;
			if (self.GetType() != other.GetType() && self.DeclaringClass != other.DeclaringClass) // optimization
			{
				throw new ClassCastException();
			}
			return self.Ordinal_Renamed - other.Ordinal_Renamed;
		}

		/// <summary>
		/// Returns the Class object corresponding to this enum constant's
		/// enum type.  Two enum constants e1 and  e2 are of the
		/// same enum type if and only if
		///   e1.getDeclaringClass() == e2.getDeclaringClass().
		/// (The value returned by this method may differ from the one returned
		/// by the <seealso cref="Object#getClass"/> method for enum constants with
		/// constant-specific class bodies.)
		/// </summary>
		/// <returns> the Class object corresponding to this enum constant's
		///     enum type </returns>
//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @SuppressWarnings("unchecked") public final Class getDeclaringClass()
		public Class DeclaringClass
		{
			get
			{
				Class clazz = this.GetType();
				Class zuper = clazz.BaseType;
				return (zuper == typeof(Enum)) ? (Class)clazz : (Class)zuper;
			}
		}

		/// <summary>
		/// Returns the enum constant of the specified enum type with the
		/// specified name.  The name must match exactly an identifier used
		/// to declare an enum constant in this type.  (Extraneous whitespace
		/// characters are not permitted.)
		/// 
		/// <para>Note that for a particular enum type {@code T}, the
		/// implicitly declared {@code public static T valueOf(String)}
		/// method on that enum may be used instead of this method to map
		/// from a name to the corresponding enum constant.  All the
		/// constants of an enum type can be obtained by calling the
		/// implicit {@code public static T[] values()} method of that
		/// type.
		/// 
		/// </para>
		/// </summary>
		/// @param <T> The enum type whose constant is to be returned </param>
		/// <param name="enumType"> the {@code Class} object of the enum type from which
		///      to return a constant </param>
		/// <param name="name"> the name of the constant to return </param>
		/// <returns> the enum constant of the specified enum type with the
		///      specified name </returns>
		/// <exception cref="IllegalArgumentException"> if the specified enum type has
		///         no constant with the specified name, or the specified
		///         class object does not represent an enum type </exception>
		/// <exception cref="NullPointerException"> if {@code enumType} or {@code name}
		///         is null
		/// @since 1.5 </exception>
		public static T valueOf<T>(Class enumType, String name) where T : Enum<T>
		{
			T result = enumType.EnumConstantDirectory()[name];
			if (result != null)
			{
				return result;
			}
			if (name == null)
			{
				throw new NullPointerException("Name is null");
			}
			throw new IllegalArgumentException("No enum constant " + enumType.CanonicalName + "." + name);
		}

		/// <summary>
		/// enum classes cannot have finalize methods.
		/// </summary>
		~Enum()
		{
		}

		/// <summary>
		/// prevent default deserialization
		/// </summary>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: private void readObject(java.io.ObjectInputStream in) throws java.io.IOException, ClassNotFoundException
		private void ReadObject(ObjectInputStream @in)
		{
			throw new InvalidObjectException("can't deserialize enum");
		}

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: private void readObjectNoData() throws java.io.ObjectStreamException
		private void ReadObjectNoData()
		{
			throw new InvalidObjectException("can't deserialize enum");
		}
	}

}
using System;

/*
 * Copyright (c) 1994, 2013, Oracle and/or its affiliates. All rights reserved.
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
	/// The Boolean class wraps a value of the primitive type
	/// {@code boolean} in an object. An object of type
	/// {@code Boolean} contains a single field whose type is
	/// {@code boolean}.
	/// <para>
	/// In addition, this class provides many methods for
	/// converting a {@code boolean} to a {@code String} and a
	/// {@code String} to a {@code boolean}, as well as other
	/// constants and methods useful when dealing with a
	/// {@code boolean}.
	/// 
	/// @author  Arthur van Hoff
	/// @since   JDK1.0
	/// </para>
	/// </summary>
	[Serializable]
	public sealed class Boolean : Comparable<Boolean>
	{
		/// <summary>
		/// The {@code Boolean} object corresponding to the primitive
		/// value {@code true}.
		/// </summary>
		public static readonly Boolean TRUE = new Boolean(true);

		/// <summary>
		/// The {@code Boolean} object corresponding to the primitive
		/// value {@code false}.
		/// </summary>
		public static readonly Boolean FALSE = new Boolean(false);

		/// <summary>
		/// The Class object representing the primitive type boolean.
		/// 
		/// @since   JDK1.1
		/// </summary>
//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @SuppressWarnings("unchecked") public static final Class TYPE = (Class) Class.getPrimitiveClass("boolean");
		public static readonly Class TYPE = (Class) Class.getPrimitiveClass("boolean");

		/// <summary>
		/// The value of the Boolean.
		/// 
		/// @serial
		/// </summary>
		private readonly bool Value;

		/// <summary>
		/// use serialVersionUID from JDK 1.0.2 for interoperability </summary>
		private const long SerialVersionUID = -3665804199014368530L;

		/// <summary>
		/// Allocates a {@code Boolean} object representing the
		/// {@code value} argument.
		/// 
		/// <para><b>Note: It is rarely appropriate to use this constructor.
		/// Unless a <i>new</i> instance is required, the static factory
		/// <seealso cref="#valueOf(boolean)"/> is generally a better choice. It is
		/// likely to yield significantly better space and time performance.</b>
		/// 
		/// </para>
		/// </summary>
		/// <param name="value">   the value of the {@code Boolean}. </param>
		public Boolean(bool value)
		{
			this.Value = value;
		}

		/// <summary>
		/// Allocates a {@code Boolean} object representing the value
		/// {@code true} if the string argument is not {@code null}
		/// and is equal, ignoring case, to the string {@code "true"}.
		/// Otherwise, allocate a {@code Boolean} object representing the
		/// value {@code false}. Examples:<para>
		/// {@code new Boolean("True")} produces a {@code Boolean} object
		/// that represents {@code true}.<br>
		/// {@code new Boolean("yes")} produces a {@code Boolean} object
		/// that represents {@code false}.
		/// 
		/// </para>
		/// </summary>
		/// <param name="s">   the string to be converted to a {@code Boolean}. </param>
		public Boolean(String s) : this(ParseBoolean(s))
		{
		}

		/// <summary>
		/// Parses the string argument as a boolean.  The {@code boolean}
		/// returned represents the value {@code true} if the string argument
		/// is not {@code null} and is equal, ignoring case, to the string
		/// {@code "true"}. <para>
		/// Example: {@code Boolean.parseBoolean("True")} returns {@code true}.<br>
		/// Example: {@code Boolean.parseBoolean("yes")} returns {@code false}.
		/// 
		/// </para>
		/// </summary>
		/// <param name="s">   the {@code String} containing the boolean
		///                 representation to be parsed </param>
		/// <returns>     the boolean represented by the string argument
		/// @since 1.5 </returns>
		public static bool ParseBoolean(String s)
		{
			return ((s != null) && s.EqualsIgnoreCase("true"));
		}

		/// <summary>
		/// Returns the value of this {@code Boolean} object as a boolean
		/// primitive.
		/// </summary>
		/// <returns>  the primitive {@code boolean} value of this object. </returns>
		public bool BooleanValue()
		{
			return Value;
		}

		/// <summary>
		/// Returns a {@code Boolean} instance representing the specified
		/// {@code boolean} value.  If the specified {@code boolean} value
		/// is {@code true}, this method returns {@code Boolean.TRUE};
		/// if it is {@code false}, this method returns {@code Boolean.FALSE}.
		/// If a new {@code Boolean} instance is not required, this method
		/// should generally be used in preference to the constructor
		/// <seealso cref="#Boolean(boolean)"/>, as this method is likely to yield
		/// significantly better space and time performance.
		/// </summary>
		/// <param name="b"> a boolean value. </param>
		/// <returns> a {@code Boolean} instance representing {@code b}.
		/// @since  1.4 </returns>
		public static Boolean ValueOf(bool b)
		{
			return (b ? TRUE : FALSE);
		}

		/// <summary>
		/// Returns a {@code Boolean} with a value represented by the
		/// specified string.  The {@code Boolean} returned represents a
		/// true value if the string argument is not {@code null}
		/// and is equal, ignoring case, to the string {@code "true"}.
		/// </summary>
		/// <param name="s">   a string. </param>
		/// <returns>  the {@code Boolean} value represented by the string. </returns>
		public static Boolean ValueOf(String s)
		{
			return ParseBoolean(s) ? TRUE : FALSE;
		}

		/// <summary>
		/// Returns a {@code String} object representing the specified
		/// boolean.  If the specified boolean is {@code true}, then
		/// the string {@code "true"} will be returned, otherwise the
		/// string {@code "false"} will be returned.
		/// </summary>
		/// <param name="b"> the boolean to be converted </param>
		/// <returns> the string representation of the specified {@code boolean}
		/// @since 1.4 </returns>
		public static String ToString(bool b)
		{
			return b ? "true" : "false";
		}

		/// <summary>
		/// Returns a {@code String} object representing this Boolean's
		/// value.  If this object represents the value {@code true},
		/// a string equal to {@code "true"} is returned. Otherwise, a
		/// string equal to {@code "false"} is returned.
		/// </summary>
		/// <returns>  a string representation of this object. </returns>
		public override String ToString()
		{
			return Value ? "true" : "false";
		}

		/// <summary>
		/// Returns a hash code for this {@code Boolean} object.
		/// </summary>
		/// <returns>  the integer {@code 1231} if this object represents
		/// {@code true}; returns the integer {@code 1237} if this
		/// object represents {@code false}. </returns>
		public override int HashCode()
		{
			return Boolean.HashCode(Value);
		}

		/// <summary>
		/// Returns a hash code for a {@code boolean} value; compatible with
		/// {@code Boolean.hashCode()}.
		/// </summary>
		/// <param name="value"> the value to hash </param>
		/// <returns> a hash code value for a {@code boolean} value.
		/// @since 1.8 </returns>
		public static int HashCode(bool value)
		{
			return value ? 1231 : 1237;
		}

	   /// <summary>
	   /// Returns {@code true} if and only if the argument is not
	   /// {@code null} and is a {@code Boolean} object that
	   /// represents the same {@code boolean} value as this object.
	   /// </summary>
	   /// <param name="obj">   the object to compare with. </param>
	   /// <returns>  {@code true} if the Boolean objects represent the
	   ///          same value; {@code false} otherwise. </returns>
		public override bool Equals(Object obj)
		{
			if (obj is Boolean)
			{
				return Value == ((Boolean)obj).BooleanValue();
			}
			return false;
		}

		/// <summary>
		/// Returns {@code true} if and only if the system property
		/// named by the argument exists and is equal to the string
		/// {@code "true"}. (Beginning with version 1.0.2 of the
		/// Java<small><sup>TM</sup></small> platform, the test of
		/// this string is case insensitive.) A system property is accessible
		/// through {@code getProperty}, a method defined by the
		/// {@code System} class.
		/// <para>
		/// If there is no property with the specified name, or if the specified
		/// name is empty or null, then {@code false} is returned.
		/// 
		/// </para>
		/// </summary>
		/// <param name="name">   the system property name. </param>
		/// <returns>  the {@code boolean} value of the system property. </returns>
		/// <exception cref="SecurityException"> for the same reasons as
		///          <seealso cref="System#getProperty(String) System.getProperty"/> </exception>
		/// <seealso cref=     java.lang.System#getProperty(java.lang.String) </seealso>
		/// <seealso cref=     java.lang.System#getProperty(java.lang.String, java.lang.String) </seealso>
		public static bool GetBoolean(String name)
		{
			bool result = false;
			try
			{
				result = ParseBoolean(System.getProperty(name));
			}
//JAVA TO C# CONVERTER TODO TASK: There is no equivalent in C# to Java 'multi-catch' syntax:
			catch (IllegalArgumentException | NullPointerException e)
			{
			}
			return result;
		}

		/// <summary>
		/// Compares this {@code Boolean} instance with another.
		/// </summary>
		/// <param name="b"> the {@code Boolean} instance to be compared </param>
		/// <returns>  zero if this object represents the same boolean value as the
		///          argument; a positive value if this object represents true
		///          and the argument represents false; and a negative value if
		///          this object represents false and the argument represents true </returns>
		/// <exception cref="NullPointerException"> if the argument is {@code null} </exception>
		/// <seealso cref=     Comparable
		/// @since  1.5 </seealso>
		public int CompareTo(Boolean b)
		{
			return Compare(this.Value, b.Value);
		}

		/// <summary>
		/// Compares two {@code boolean} values.
		/// The value returned is identical to what would be returned by:
		/// <pre>
		///    Boolean.valueOf(x).compareTo(Boolean.valueOf(y))
		/// </pre>
		/// </summary>
		/// <param name="x"> the first {@code boolean} to compare </param>
		/// <param name="y"> the second {@code boolean} to compare </param>
		/// <returns> the value {@code 0} if {@code x == y};
		///         a value less than {@code 0} if {@code !x && y}; and
		///         a value greater than {@code 0} if {@code x && !y}
		/// @since 1.7 </returns>
		public static int Compare(bool x, bool y)
		{
			return (x == y) ? 0 : (x ? 1 : -1);
		}

		/// <summary>
		/// Returns the result of applying the logical AND operator to the
		/// specified {@code boolean} operands.
		/// </summary>
		/// <param name="a"> the first operand </param>
		/// <param name="b"> the second operand </param>
		/// <returns> the logical AND of {@code a} and {@code b} </returns>
		/// <seealso cref= java.util.function.BinaryOperator
		/// @since 1.8 </seealso>
		public static bool LogicalAnd(bool a, bool b)
		{
			return a && b;
		}

		/// <summary>
		/// Returns the result of applying the logical OR operator to the
		/// specified {@code boolean} operands.
		/// </summary>
		/// <param name="a"> the first operand </param>
		/// <param name="b"> the second operand </param>
		/// <returns> the logical OR of {@code a} and {@code b} </returns>
		/// <seealso cref= java.util.function.BinaryOperator
		/// @since 1.8 </seealso>
		public static bool LogicalOr(bool a, bool b)
		{
			return a || b;
		}

		/// <summary>
		/// Returns the result of applying the logical XOR operator to the
		/// specified {@code boolean} operands.
		/// </summary>
		/// <param name="a"> the first operand </param>
		/// <param name="b"> the second operand </param>
		/// <returns>  the logical XOR of {@code a} and {@code b} </returns>
		/// <seealso cref= java.util.function.BinaryOperator
		/// @since 1.8 </seealso>
		public static bool LogicalXor(bool a, bool b)
		{
			return a ^ b;
		}
	}

}
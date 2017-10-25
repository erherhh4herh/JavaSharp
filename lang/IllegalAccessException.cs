/*
 * Copyright (c) 1995, 2008, Oracle and/or its affiliates. All rights reserved.
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
	/// An IllegalAccessException is thrown when an application tries
	/// to reflectively create an instance (other than an array),
	/// set or get a field, or invoke a method, but the currently
	/// executing method does not have access to the definition of
	/// the specified class, field, method or constructor.
	/// 
	/// @author  unascribed </summary>
	/// <seealso cref=     Class#newInstance() </seealso>
	/// <seealso cref=     java.lang.reflect.Field#set(Object, Object) </seealso>
	/// <seealso cref=     java.lang.reflect.Field#setBoolean(Object, boolean) </seealso>
	/// <seealso cref=     java.lang.reflect.Field#setByte(Object, byte) </seealso>
	/// <seealso cref=     java.lang.reflect.Field#setShort(Object, short) </seealso>
	/// <seealso cref=     java.lang.reflect.Field#setChar(Object, char) </seealso>
	/// <seealso cref=     java.lang.reflect.Field#setInt(Object, int) </seealso>
	/// <seealso cref=     java.lang.reflect.Field#setLong(Object, long) </seealso>
	/// <seealso cref=     java.lang.reflect.Field#setFloat(Object, float) </seealso>
	/// <seealso cref=     java.lang.reflect.Field#setDouble(Object, double) </seealso>
	/// <seealso cref=     java.lang.reflect.Field#get(Object) </seealso>
	/// <seealso cref=     java.lang.reflect.Field#getBoolean(Object) </seealso>
	/// <seealso cref=     java.lang.reflect.Field#getByte(Object) </seealso>
	/// <seealso cref=     java.lang.reflect.Field#getShort(Object) </seealso>
	/// <seealso cref=     java.lang.reflect.Field#getChar(Object) </seealso>
	/// <seealso cref=     java.lang.reflect.Field#getInt(Object) </seealso>
	/// <seealso cref=     java.lang.reflect.Field#getLong(Object) </seealso>
	/// <seealso cref=     java.lang.reflect.Field#getFloat(Object) </seealso>
	/// <seealso cref=     java.lang.reflect.Field#getDouble(Object) </seealso>
	/// <seealso cref=     java.lang.reflect.Method#invoke(Object, Object[]) </seealso>
	/// <seealso cref=     java.lang.reflect.Constructor#newInstance(Object[])
	/// @since   JDK1.0 </seealso>
	public class IllegalAccessException : ReflectiveOperationException
	{
		private new const long SerialVersionUID = 6616958222490762034L;

		/// <summary>
		/// Constructs an <code>IllegalAccessException</code> without a
		/// detail message.
		/// </summary>
		public IllegalAccessException() : base()
		{
		}

		/// <summary>
		/// Constructs an <code>IllegalAccessException</code> with a detail message.
		/// </summary>
		/// <param name="s">   the detail message. </param>
		public IllegalAccessException(String s) : base(s)
		{
		}
	}

}
/*
 * Copyright (c) 2000, 2013, Oracle and/or its affiliates. All rights reserved.
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

namespace java.nio
{


	/// <summary>
	/// A typesafe enumeration for byte orders.
	/// 
	/// @author Mark Reinhold
	/// @author JSR-51 Expert Group
	/// @since 1.4
	/// </summary>

	public sealed class ByteOrder
	{

		private String Name;

		private ByteOrder(String name)
		{
			this.Name = name;
		}

		/// <summary>
		/// Constant denoting big-endian byte order.  In this order, the bytes of a
		/// multibyte value are ordered from most significant to least significant.
		/// </summary>
		public static readonly ByteOrder BIG_ENDIAN = new ByteOrder("BIG_ENDIAN");

		/// <summary>
		/// Constant denoting little-endian byte order.  In this order, the bytes of
		/// a multibyte value are ordered from least significant to most
		/// significant.
		/// </summary>
		public static readonly ByteOrder LITTLE_ENDIAN = new ByteOrder("LITTLE_ENDIAN");

		/// <summary>
		/// Retrieves the native byte order of the underlying platform.
		/// 
		/// <para> This method is defined so that performance-sensitive Java code can
		/// allocate direct buffers with the same byte order as the hardware.
		/// Native code libraries are often more efficient when such buffers are
		/// used.  </para>
		/// </summary>
		/// <returns>  The native byte order of the hardware upon which this Java
		///          virtual machine is running </returns>
		public static ByteOrder NativeOrder()
		{
			return Bits.ByteOrder();
		}

		/// <summary>
		/// Constructs a string describing this object.
		/// 
		/// <para> This method returns the string <tt>"BIG_ENDIAN"</tt> for {@link
		/// #BIG_ENDIAN} and <tt>"LITTLE_ENDIAN"</tt> for <seealso cref="#LITTLE_ENDIAN"/>.
		/// </para>
		/// </summary>
		/// <returns>  The specified string </returns>
		public override String ToString()
		{
			return Name;
		}

	}

}
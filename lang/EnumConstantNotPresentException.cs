/*
 * Copyright (c) 2004, 2011, Oracle and/or its affiliates. All rights reserved.
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
	/// Thrown when an application tries to access an enum constant by name
	/// and the enum type contains no constant with the specified name.
	/// This exception can be thrown by the {@linkplain
	/// java.lang.reflect.AnnotatedElement API used to read annotations
	/// reflectively}.
	/// 
	/// @author  Josh Bloch </summary>
	/// <seealso cref=     java.lang.reflect.AnnotatedElement
	/// @since   1.5 </seealso>
//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @SuppressWarnings("rawtypes") public class EnumConstantNotPresentException extends RuntimeException
	public class EnumConstantNotPresentException : RuntimeException // rawtypes are part of the public api
	{
		private new const long SerialVersionUID = -6046998521960521108L;

		/// <summary>
		/// The type of the missing enum constant.
		/// </summary>
		private Class EnumType_Renamed;

		/// <summary>
		/// The name of the missing enum constant.
		/// </summary>
		private String ConstantName_Renamed;

		/// <summary>
		/// Constructs an <tt>EnumConstantNotPresentException</tt> for the
		/// specified constant.
		/// </summary>
		/// <param name="enumType"> the type of the missing enum constant </param>
		/// <param name="constantName"> the name of the missing enum constant </param>
		public EnumConstantNotPresentException(Class enumType, String constantName) : base(enumType.Name + "." + constantName)
		{
			this.EnumType_Renamed = enumType;
			this.ConstantName_Renamed = constantName;
		}

		/// <summary>
		/// Returns the type of the missing enum constant.
		/// </summary>
		/// <returns> the type of the missing enum constant </returns>
		public virtual Class EnumType()
		{
			return EnumType_Renamed;
		}

		/// <summary>
		/// Returns the name of the missing enum constant.
		/// </summary>
		/// <returns> the name of the missing enum constant </returns>
		public virtual String ConstantName()
		{
			return ConstantName_Renamed;
		}
	}

}
/*
 * Copyright (c) 1997, 2003, Oracle and/or its affiliates. All rights reserved.
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

namespace java.security.spec
{

	/// <summary>
	/// This is the exception for invalid parameter specifications.
	/// 
	/// @author Jan Luehe
	/// 
	/// </summary>
	/// <seealso cref= java.security.AlgorithmParameters </seealso>
	/// <seealso cref= AlgorithmParameterSpec </seealso>
	/// <seealso cref= DSAParameterSpec
	/// 
	/// @since 1.2 </seealso>

	public class InvalidParameterSpecException : GeneralSecurityException
	{

		private new const long SerialVersionUID = -970468769593399342L;

		/// <summary>
		/// Constructs an InvalidParameterSpecException with no detail message. A
		/// detail message is a String that describes this particular
		/// exception.
		/// </summary>
		public InvalidParameterSpecException() : base()
		{
		}

		/// <summary>
		/// Constructs an InvalidParameterSpecException with the specified detail
		/// message. A detail message is a String that describes this
		/// particular exception.
		/// </summary>
		/// <param name="msg"> the detail message. </param>
		public InvalidParameterSpecException(String msg) : base(msg)
		{
		}
	}

}
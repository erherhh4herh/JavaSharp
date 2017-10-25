/*
 * Copyright (c) 1996, 1999, Oracle and/or its affiliates. All rights reserved.
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

namespace java.io
{

	/// <summary>
	/// Callback interface to allow validation of objects within a graph.
	/// Allows an object to be called when a complete graph of objects has
	/// been deserialized.
	/// 
	/// @author  unascribed </summary>
	/// <seealso cref=     ObjectInputStream </seealso>
	/// <seealso cref=     ObjectInputStream#registerValidation(java.io.ObjectInputValidation, int)
	/// @since   JDK1.1 </seealso>
	public interface ObjectInputValidation
	{
		/// <summary>
		/// Validates the object.
		/// </summary>
		/// <exception cref="InvalidObjectException"> If the object cannot validate itself. </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public void validateObject() throws InvalidObjectException;
		void ValidateObject();
	}

}
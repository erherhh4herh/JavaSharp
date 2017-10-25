/*
 * Copyright (c) 1996, 2004, Oracle and/or its affiliates. All rights reserved.
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
	/// Only the identity of the class of an Externalizable instance is
	/// written in the serialization stream and it is the responsibility
	/// of the class to save and restore the contents of its instances.
	/// 
	/// The writeExternal and readExternal methods of the Externalizable
	/// interface are implemented by a class to give the class complete
	/// control over the format and contents of the stream for an object
	/// and its supertypes. These methods must explicitly
	/// coordinate with the supertype to save its state. These methods supersede
	/// customized implementations of writeObject and readObject methods.<br>
	/// 
	/// Object Serialization uses the Serializable and Externalizable
	/// interfaces.  Object persistence mechanisms can use them as well.  Each
	/// object to be stored is tested for the Externalizable interface. If
	/// the object supports Externalizable, the writeExternal method is called. If the
	/// object does not support Externalizable and does implement
	/// Serializable, the object is saved using
	/// ObjectOutputStream. <br> When an Externalizable object is
	/// reconstructed, an instance is created using the public no-arg
	/// constructor, then the readExternal method called.  Serializable
	/// objects are restored by reading them from an ObjectInputStream.<br>
	/// 
	/// An Externalizable instance can designate a substitution object via
	/// the writeReplace and readResolve methods documented in the Serializable
	/// interface.<br>
	/// 
	/// @author  unascribed </summary>
	/// <seealso cref= java.io.ObjectOutputStream </seealso>
	/// <seealso cref= java.io.ObjectInputStream </seealso>
	/// <seealso cref= java.io.ObjectOutput </seealso>
	/// <seealso cref= java.io.ObjectInput </seealso>
	/// <seealso cref= java.io.Serializable
	/// @since   JDK1.1 </seealso>
	public interface Externalizable
	{
		/// <summary>
		/// The object implements the writeExternal method to save its contents
		/// by calling the methods of DataOutput for its primitive values or
		/// calling the writeObject method of ObjectOutput for objects, strings,
		/// and arrays.
		/// 
		/// @serialData Overriding methods should use this tag to describe
		///             the data layout of this Externalizable object.
		///             List the sequence of element types and, if possible,
		///             relate the element to a public/protected field and/or
		///             method of this Externalizable class.
		/// </summary>
		/// <param name="out"> the stream to write the object to </param>
		/// <exception cref="IOException"> Includes any I/O exceptions that may occur </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: void writeExternal(java.io.ObjectOutput out) throws IOException;
		void WriteExternal(ObjectOutput @out);

		/// <summary>
		/// The object implements the readExternal method to restore its
		/// contents by calling the methods of DataInput for primitive
		/// types and readObject for objects, strings and arrays.  The
		/// readExternal method must read the values in the same sequence
		/// and with the same types as were written by writeExternal.
		/// </summary>
		/// <param name="in"> the stream to read data from in order to restore the object </param>
		/// <exception cref="IOException"> if I/O errors occur </exception>
		/// <exception cref="ClassNotFoundException"> If the class for an object being
		///              restored cannot be found. </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: void readExternal(java.io.ObjectInput in) throws IOException, ClassNotFoundException;
		void ReadExternal(ObjectInput @in);
	}

}
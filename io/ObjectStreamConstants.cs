/*
 * Copyright (c) 1996, 2013, Oracle and/or its affiliates. All rights reserved.
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
	/// Constants written into the Object Serialization Stream.
	/// 
	/// @author  unascribed
	/// @since JDK 1.1
	/// </summary>
	public interface ObjectStreamConstants
	{

		/// <summary>
		/// Magic number that is written to the stream header.
		/// </summary>

		/// <summary>
		/// Version number that is written to the stream header.
		/// </summary>

		/* Each item in the stream is preceded by a tag
		 */

		/// <summary>
		/// First tag value.
		/// </summary>

		/// <summary>
		/// Null object reference.
		/// </summary>

		/// <summary>
		/// Reference to an object already written into the stream.
		/// </summary>

		/// <summary>
		/// new Class Descriptor.
		/// </summary>

		/// <summary>
		/// new Object.
		/// </summary>

		/// <summary>
		/// new String.
		/// </summary>

		/// <summary>
		/// new Array.
		/// </summary>

		/// <summary>
		/// Reference to Class.
		/// </summary>

		/// <summary>
		/// Block of optional data. Byte following tag indicates number
		/// of bytes in this block data.
		/// </summary>

		/// <summary>
		/// End of optional block data blocks for an object.
		/// </summary>

		/// <summary>
		/// Reset stream context. All handles written into stream are reset.
		/// </summary>

		/// <summary>
		/// long Block data. The long following the tag indicates the
		/// number of bytes in this block data.
		/// </summary>

		/// <summary>
		/// Exception during write.
		/// </summary>

		/// <summary>
		/// Long string.
		/// </summary>

		/// <summary>
		/// new Proxy Class Descriptor.
		/// </summary>

		/// <summary>
		/// new Enum constant.
		/// @since 1.5
		/// </summary>

		/// <summary>
		/// Last tag value.
		/// </summary>

		/// <summary>
		/// First wire handle to be assigned.
		/// </summary>


		/// <summary>
		///*************************************************** </summary>
		/* Bit masks for ObjectStreamClass flag.*/

		/// <summary>
		/// Bit mask for ObjectStreamClass flag. Indicates a Serializable class
		/// defines its own writeObject method.
		/// </summary>

		/// <summary>
		/// Bit mask for ObjectStreamClass flag. Indicates Externalizable data
		/// written in Block Data mode.
		/// Added for PROTOCOL_VERSION_2.
		/// </summary>
		/// <seealso cref= #PROTOCOL_VERSION_2
		/// @since 1.2 </seealso>

		/// <summary>
		/// Bit mask for ObjectStreamClass flag. Indicates class is Serializable.
		/// </summary>

		/// <summary>
		/// Bit mask for ObjectStreamClass flag. Indicates class is Externalizable.
		/// </summary>

		/// <summary>
		/// Bit mask for ObjectStreamClass flag. Indicates class is an enum type.
		/// @since 1.5
		/// </summary>


		/* *******************************************************************/
		/* Security permissions */

		/// <summary>
		/// Enable substitution of one object for another during
		/// serialization/deserialization.
		/// </summary>
		/// <seealso cref= java.io.ObjectOutputStream#enableReplaceObject(boolean) </seealso>
		/// <seealso cref= java.io.ObjectInputStream#enableResolveObject(boolean)
		/// @since 1.2 </seealso>

		/// <summary>
		/// Enable overriding of readObject and writeObject.
		/// </summary>
		/// <seealso cref= java.io.ObjectOutputStream#writeObjectOverride(Object) </seealso>
		/// <seealso cref= java.io.ObjectInputStream#readObjectOverride()
		/// @since 1.2 </seealso>
	   /// <summary>
	   /// A Stream Protocol Version. <para>
	   /// 
	   /// All externalizable data is written in JDK 1.1 external data
	   /// format after calling this method. This version is needed to write
	   /// streams containing Externalizable data that can be read by
	   /// pre-JDK 1.1.6 JVMs.
	   /// 
	   /// </para>
	   /// </summary>
	   /// <seealso cref= java.io.ObjectOutputStream#useProtocolVersion(int)
	   /// @since 1.2 </seealso>


	   /// <summary>
	   /// A Stream Protocol Version. <para>
	   /// 
	   /// This protocol is written by JVM 1.2.
	   /// 
	   /// Externalizable data is written in block data mode and is
	   /// terminated with TC_ENDBLOCKDATA. Externalizable class descriptor
	   /// flags has SC_BLOCK_DATA enabled. JVM 1.1.6 and greater can
	   /// read this format change.
	   /// 
	   /// Enables writing a nonSerializable class descriptor into the
	   /// stream. The serialVersionUID of a nonSerializable class is
	   /// set to 0L.
	   /// 
	   /// </para>
	   /// </summary>
	   /// <seealso cref= java.io.ObjectOutputStream#useProtocolVersion(int) </seealso>
	   /// <seealso cref= #SC_BLOCK_DATA
	   /// @since 1.2 </seealso>
	}

	public static class ObjectStreamConstants_Fields
	{
		public static readonly short STREAM_MAGIC = unchecked((short)0xaced);
		public const short STREAM_VERSION = 5;
		public const sbyte TC_BASE = 0x70;
		public static readonly sbyte TC_NULL = (sbyte)0x70;
		public static readonly sbyte TC_REFERENCE = (sbyte)0x71;
		public static readonly sbyte TC_CLASSDESC = (sbyte)0x72;
		public static readonly sbyte TC_OBJECT = (sbyte)0x73;
		public static readonly sbyte TC_STRING = (sbyte)0x74;
		public static readonly sbyte TC_ARRAY = (sbyte)0x75;
		public static readonly sbyte TC_CLASS = (sbyte)0x76;
		public static readonly sbyte TC_BLOCKDATA = (sbyte)0x77;
		public static readonly sbyte TC_ENDBLOCKDATA = (sbyte)0x78;
		public static readonly sbyte TC_RESET = (sbyte)0x79;
		public static readonly sbyte TC_BLOCKDATALONG = (sbyte)0x7A;
		public static readonly sbyte TC_EXCEPTION = (sbyte)0x7B;
		public static readonly sbyte TC_LONGSTRING = (sbyte)0x7C;
		public static readonly sbyte TC_PROXYCLASSDESC = (sbyte)0x7D;
		public static readonly sbyte TC_ENUM = (sbyte)0x7E;
		public static readonly sbyte TC_MAX = (sbyte)0x7E;
		public const int BaseWireHandle = 0x7e0000;
		public const sbyte SC_WRITE_METHOD = 0x01;
		public const sbyte SC_BLOCK_DATA = 0x08;
		public const sbyte SC_SERIALIZABLE = 0x02;
		public const sbyte SC_EXTERNALIZABLE = 0x04;
		public const sbyte SC_ENUM = 0x10;
		public static readonly SerializablePermission SUBSTITUTION_PERMISSION = new SerializablePermission("enableSubstitution");
		public static readonly SerializablePermission SUBCLASS_IMPLEMENTATION_PERMISSION = new SerializablePermission("enableSubclassImplementation");
		public const int PROTOCOL_VERSION_1 = 1;
		public const int PROTOCOL_VERSION_2 = 2;
	}

}
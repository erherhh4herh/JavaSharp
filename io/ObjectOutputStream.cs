using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Concurrent;
using System.Runtime.InteropServices;

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

	using ReflectUtil = sun.reflect.misc.ReflectUtil;

	/// <summary>
	/// An ObjectOutputStream writes primitive data types and graphs of Java objects
	/// to an OutputStream.  The objects can be read (reconstituted) using an
	/// ObjectInputStream.  Persistent storage of objects can be accomplished by
	/// using a file for the stream.  If the stream is a network socket stream, the
	/// objects can be reconstituted on another host or in another process.
	/// 
	/// <para>Only objects that support the java.io.Serializable interface can be
	/// written to streams.  The class of each serializable object is encoded
	/// including the class name and signature of the class, the values of the
	/// object's fields and arrays, and the closure of any other objects referenced
	/// from the initial objects.
	/// 
	/// </para>
	/// <para>The method writeObject is used to write an object to the stream.  Any
	/// object, including Strings and arrays, is written with writeObject. Multiple
	/// objects or primitives can be written to the stream.  The objects must be
	/// read back from the corresponding ObjectInputstream with the same types and
	/// in the same order as they were written.
	/// 
	/// </para>
	/// <para>Primitive data types can also be written to the stream using the
	/// appropriate methods from DataOutput. Strings can also be written using the
	/// writeUTF method.
	/// 
	/// </para>
	/// <para>The default serialization mechanism for an object writes the class of the
	/// object, the class signature, and the values of all non-transient and
	/// non-static fields.  References to other objects (except in transient or
	/// static fields) cause those objects to be written also. Multiple references
	/// to a single object are encoded using a reference sharing mechanism so that
	/// graphs of objects can be restored to the same shape as when the original was
	/// written.
	/// 
	/// </para>
	/// <para>For example to write an object that can be read by the example in
	/// ObjectInputStream:
	/// <br>
	/// <pre>
	///      FileOutputStream fos = new FileOutputStream("t.tmp");
	///      ObjectOutputStream oos = new ObjectOutputStream(fos);
	/// 
	///      oos.writeInt(12345);
	///      oos.writeObject("Today");
	///      oos.writeObject(new Date());
	/// 
	///      oos.close();
	/// </pre>
	/// 
	/// </para>
	/// <para>Classes that require special handling during the serialization and
	/// deserialization process must implement special methods with these exact
	/// signatures:
	/// <br>
	/// <pre>
	/// private void readObject(java.io.ObjectInputStream stream)
	///     throws IOException, ClassNotFoundException;
	/// private void writeObject(java.io.ObjectOutputStream stream)
	///     throws IOException
	/// private void readObjectNoData()
	///     throws ObjectStreamException;
	/// </pre>
	/// 
	/// </para>
	/// <para>The writeObject method is responsible for writing the state of the object
	/// for its particular class so that the corresponding readObject method can
	/// restore it.  The method does not need to concern itself with the state
	/// belonging to the object's superclasses or subclasses.  State is saved by
	/// writing the individual fields to the ObjectOutputStream using the
	/// writeObject method or by using the methods for primitive data types
	/// supported by DataOutput.
	/// 
	/// </para>
	/// <para>Serialization does not write out the fields of any object that does not
	/// implement the java.io.Serializable interface.  Subclasses of Objects that
	/// are not serializable can be serializable. In this case the non-serializable
	/// class must have a no-arg constructor to allow its fields to be initialized.
	/// In this case it is the responsibility of the subclass to save and restore
	/// the state of the non-serializable class. It is frequently the case that the
	/// fields of that class are accessible (public, package, or protected) or that
	/// there are get and set methods that can be used to restore the state.
	/// 
	/// </para>
	/// <para>Serialization of an object can be prevented by implementing writeObject
	/// and readObject methods that throw the NotSerializableException.  The
	/// exception will be caught by the ObjectOutputStream and abort the
	/// serialization process.
	/// 
	/// </para>
	/// <para>Implementing the Externalizable interface allows the object to assume
	/// complete control over the contents and format of the object's serialized
	/// form.  The methods of the Externalizable interface, writeExternal and
	/// readExternal, are called to save and restore the objects state.  When
	/// implemented by a class they can write and read their own state using all of
	/// the methods of ObjectOutput and ObjectInput.  It is the responsibility of
	/// the objects to handle any versioning that occurs.
	/// 
	/// </para>
	/// <para>Enum constants are serialized differently than ordinary serializable or
	/// externalizable objects.  The serialized form of an enum constant consists
	/// solely of its name; field values of the constant are not transmitted.  To
	/// serialize an enum constant, ObjectOutputStream writes the string returned by
	/// the constant's name method.  Like other serializable or externalizable
	/// objects, enum constants can function as the targets of back references
	/// appearing subsequently in the serialization stream.  The process by which
	/// enum constants are serialized cannot be customized; any class-specific
	/// writeObject and writeReplace methods defined by enum types are ignored
	/// during serialization.  Similarly, any serialPersistentFields or
	/// serialVersionUID field declarations are also ignored--all enum types have a
	/// fixed serialVersionUID of 0L.
	/// 
	/// </para>
	/// <para>Primitive data, excluding serializable fields and externalizable data, is
	/// written to the ObjectOutputStream in block-data records. A block data record
	/// is composed of a header and data. The block data header consists of a marker
	/// and the number of bytes to follow the header.  Consecutive primitive data
	/// writes are merged into one block-data record.  The blocking factor used for
	/// a block-data record will be 1024 bytes.  Each block-data record will be
	/// filled up to 1024 bytes, or be written whenever there is a termination of
	/// block-data mode.  Calls to the ObjectOutputStream methods writeObject,
	/// defaultWriteObject and writeFields initially terminate any existing
	/// block-data record.
	/// 
	/// @author      Mike Warres
	/// @author      Roger Riggs
	/// </para>
	/// </summary>
	/// <seealso cref= java.io.DataOutput </seealso>
	/// <seealso cref= java.io.ObjectInputStream </seealso>
	/// <seealso cref= java.io.Serializable </seealso>
	/// <seealso cref= java.io.Externalizable </seealso>
	/// <seealso cref= <a href="../../../platform/serialization/spec/output.html">Object Serialization Specification, Section 2, Object Output Classes</a>
	/// @since       JDK1.1 </seealso>
	public class ObjectOutputStream : OutputStream, ObjectOutput, ObjectStreamConstants
	{

		private class Caches
		{
			/// <summary>
			/// cache of subclass security audit results </summary>
			internal static readonly ConcurrentMap<WeakClassKey, Boolean> SubclassAudits = new ConcurrentDictionary<WeakClassKey, Boolean>();

			/// <summary>
			/// queue for WeakReferences to audited subclasses </summary>
			internal static readonly ReferenceQueue<Class> SubclassAuditsQueue = new ReferenceQueue<Class>();
		}

		/// <summary>
		/// filter stream for handling block data conversion </summary>
		private readonly BlockDataOutputStream Bout;
		/// <summary>
		/// obj -> wire handle map </summary>
		private readonly HandleTable Handles;
		/// <summary>
		/// obj -> replacement obj map </summary>
		private readonly ReplaceTable Subs;
		/// <summary>
		/// stream protocol version </summary>
		private int Protocol = ObjectStreamConstants_Fields.PROTOCOL_VERSION_2;
		/// <summary>
		/// recursion depth </summary>
		private int Depth;

		/// <summary>
		/// buffer for writing primitive field values </summary>
		private sbyte[] PrimVals;

		/// <summary>
		/// if true, invoke writeObjectOverride() instead of writeObject() </summary>
		private readonly bool EnableOverride;
		/// <summary>
		/// if true, invoke replaceObject() </summary>
		private bool EnableReplace;

		// values below valid only during upcalls to writeObject()/writeExternal()
		/// <summary>
		/// Context during upcalls to class-defined writeObject methods; holds
		/// object currently being serialized and descriptor for current class.
		/// Null when not during writeObject upcall.
		/// </summary>
		private SerialCallbackContext CurContext;
		/// <summary>
		/// current PutField object </summary>
		private PutFieldImpl CurPut;

		/// <summary>
		/// custom storage for debug trace info </summary>
		private readonly DebugTraceInfoStack DebugInfoStack;

		/// <summary>
		/// value of "sun.io.serialization.extendedDebugInfo" property,
		/// as true or false for extended information about exception's place
		/// </summary>
		private static readonly bool ExtendedDebugInfo = (bool)AccessController.doPrivileged(new sun.security.action.GetBooleanAction("sun.io.serialization.extendedDebugInfo"));

		/// <summary>
		/// Creates an ObjectOutputStream that writes to the specified OutputStream.
		/// This constructor writes the serialization stream header to the
		/// underlying stream; callers may wish to flush the stream immediately to
		/// ensure that constructors for receiving ObjectInputStreams will not block
		/// when reading the header.
		/// 
		/// <para>If a security manager is installed, this constructor will check for
		/// the "enableSubclassImplementation" SerializablePermission when invoked
		/// directly or indirectly by the constructor of a subclass which overrides
		/// the ObjectOutputStream.putFields or ObjectOutputStream.writeUnshared
		/// methods.
		/// 
		/// </para>
		/// </summary>
		/// <param name="out"> output stream to write to </param>
		/// <exception cref="IOException"> if an I/O error occurs while writing stream header </exception>
		/// <exception cref="SecurityException"> if untrusted subclass illegally overrides
		///          security-sensitive methods </exception>
		/// <exception cref="NullPointerException"> if <code>out</code> is <code>null</code>
		/// @since   1.4 </exception>
		/// <seealso cref=     ObjectOutputStream#ObjectOutputStream() </seealso>
		/// <seealso cref=     ObjectOutputStream#putFields() </seealso>
		/// <seealso cref=     ObjectInputStream#ObjectInputStream(InputStream) </seealso>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public ObjectOutputStream(OutputStream out) throws IOException
		public ObjectOutputStream(OutputStream @out)
		{
			VerifySubclass();
			Bout = new BlockDataOutputStream(@out);
			Handles = new HandleTable(10, (float) 3.00);
			Subs = new ReplaceTable(10, (float) 3.00);
			EnableOverride = false;
			WriteStreamHeader();
			Bout.BlockDataMode = true;
			if (ExtendedDebugInfo)
			{
				DebugInfoStack = new DebugTraceInfoStack();
			}
			else
			{
				DebugInfoStack = null;
			}
		}

		/// <summary>
		/// Provide a way for subclasses that are completely reimplementing
		/// ObjectOutputStream to not have to allocate private data just used by
		/// this implementation of ObjectOutputStream.
		/// 
		/// <para>If there is a security manager installed, this method first calls the
		/// security manager's <code>checkPermission</code> method with a
		/// <code>SerializablePermission("enableSubclassImplementation")</code>
		/// permission to ensure it's ok to enable subclassing.
		/// 
		/// </para>
		/// </summary>
		/// <exception cref="SecurityException"> if a security manager exists and its
		///          <code>checkPermission</code> method denies enabling
		///          subclassing. </exception>
		/// <exception cref="IOException"> if an I/O error occurs while creating this stream </exception>
		/// <seealso cref= SecurityManager#checkPermission </seealso>
		/// <seealso cref= java.io.SerializablePermission </seealso>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: protected ObjectOutputStream() throws IOException, SecurityException
		protected internal ObjectOutputStream()
		{
			SecurityManager sm = System.SecurityManager;
			if (sm != null)
			{
				sm.CheckPermission(ObjectStreamConstants_Fields.SUBCLASS_IMPLEMENTATION_PERMISSION);
			}
			Bout = null;
			Handles = null;
			Subs = null;
			EnableOverride = true;
			DebugInfoStack = null;
		}

		/// <summary>
		/// Specify stream protocol version to use when writing the stream.
		/// 
		/// <para>This routine provides a hook to enable the current version of
		/// Serialization to write in a format that is backwards compatible to a
		/// previous version of the stream format.
		/// 
		/// </para>
		/// <para>Every effort will be made to avoid introducing additional
		/// backwards incompatibilities; however, sometimes there is no
		/// other alternative.
		/// 
		/// </para>
		/// </summary>
		/// <param name="version"> use ProtocolVersion from java.io.ObjectStreamConstants. </param>
		/// <exception cref="IllegalStateException"> if called after any objects
		///          have been serialized. </exception>
		/// <exception cref="IllegalArgumentException"> if invalid version is passed in. </exception>
		/// <exception cref="IOException"> if I/O errors occur </exception>
		/// <seealso cref= java.io.ObjectStreamConstants#PROTOCOL_VERSION_1 </seealso>
		/// <seealso cref= java.io.ObjectStreamConstants#PROTOCOL_VERSION_2
		/// @since   1.2 </seealso>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public void useProtocolVersion(int version) throws IOException
		public virtual void UseProtocolVersion(int version)
		{
			if (Handles.Size() != 0)
			{
				// REMIND: implement better check for pristine stream?
				throw new IllegalStateException("stream non-empty");
			}
			switch (version)
			{
				case ObjectStreamConstants_Fields.PROTOCOL_VERSION_1:
				case ObjectStreamConstants_Fields.PROTOCOL_VERSION_2:
					Protocol = version;
					break;

				default:
					throw new IllegalArgumentException("unknown version: " + version);
			}
		}

		/// <summary>
		/// Write the specified object to the ObjectOutputStream.  The class of the
		/// object, the signature of the class, and the values of the non-transient
		/// and non-static fields of the class and all of its supertypes are
		/// written.  Default serialization for a class can be overridden using the
		/// writeObject and the readObject methods.  Objects referenced by this
		/// object are written transitively so that a complete equivalent graph of
		/// objects can be reconstructed by an ObjectInputStream.
		/// 
		/// <para>Exceptions are thrown for problems with the OutputStream and for
		/// classes that should not be serialized.  All exceptions are fatal to the
		/// OutputStream, which is left in an indeterminate state, and it is up to
		/// the caller to ignore or recover the stream state.
		/// 
		/// </para>
		/// </summary>
		/// <exception cref="InvalidClassException"> Something is wrong with a class used by
		///          serialization. </exception>
		/// <exception cref="NotSerializableException"> Some object to be serialized does not
		///          implement the java.io.Serializable interface. </exception>
		/// <exception cref="IOException"> Any exception thrown by the underlying
		///          OutputStream. </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public final void writeObject(Object obj) throws IOException
		public void WriteObject(Object obj)
		{
			if (EnableOverride)
			{
				WriteObjectOverride(obj);
				return;
			}
			try
			{
				WriteObject0(obj, false);
			}
			catch (IOException ex)
			{
				if (Depth == 0)
				{
					WriteFatalException(ex);
				}
				throw ex;
			}
		}

		/// <summary>
		/// Method used by subclasses to override the default writeObject method.
		/// This method is called by trusted subclasses of ObjectInputStream that
		/// constructed ObjectInputStream using the protected no-arg constructor.
		/// The subclass is expected to provide an override method with the modifier
		/// "final".
		/// </summary>
		/// <param name="obj"> object to be written to the underlying stream </param>
		/// <exception cref="IOException"> if there are I/O errors while writing to the
		///          underlying stream </exception>
		/// <seealso cref= #ObjectOutputStream() </seealso>
		/// <seealso cref= #writeObject(Object)
		/// @since 1.2 </seealso>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: protected void writeObjectOverride(Object obj) throws IOException
		protected internal virtual void WriteObjectOverride(Object obj)
		{
		}

		/// <summary>
		/// Writes an "unshared" object to the ObjectOutputStream.  This method is
		/// identical to writeObject, except that it always writes the given object
		/// as a new, unique object in the stream (as opposed to a back-reference
		/// pointing to a previously serialized instance).  Specifically:
		/// <ul>
		///   <li>An object written via writeUnshared is always serialized in the
		///       same manner as a newly appearing object (an object that has not
		///       been written to the stream yet), regardless of whether or not the
		///       object has been written previously.
		/// 
		///   <li>If writeObject is used to write an object that has been previously
		///       written with writeUnshared, the previous writeUnshared operation
		///       is treated as if it were a write of a separate object.  In other
		///       words, ObjectOutputStream will never generate back-references to
		///       object data written by calls to writeUnshared.
		/// </ul>
		/// While writing an object via writeUnshared does not in itself guarantee a
		/// unique reference to the object when it is deserialized, it allows a
		/// single object to be defined multiple times in a stream, so that multiple
		/// calls to readUnshared by the receiver will not conflict.  Note that the
		/// rules described above only apply to the base-level object written with
		/// writeUnshared, and not to any transitively referenced sub-objects in the
		/// object graph to be serialized.
		/// 
		/// <para>ObjectOutputStream subclasses which override this method can only be
		/// constructed in security contexts possessing the
		/// "enableSubclassImplementation" SerializablePermission; any attempt to
		/// instantiate such a subclass without this permission will cause a
		/// SecurityException to be thrown.
		/// 
		/// </para>
		/// </summary>
		/// <param name="obj"> object to write to stream </param>
		/// <exception cref="NotSerializableException"> if an object in the graph to be
		///          serialized does not implement the Serializable interface </exception>
		/// <exception cref="InvalidClassException"> if a problem exists with the class of an
		///          object to be serialized </exception>
		/// <exception cref="IOException"> if an I/O error occurs during serialization
		/// @since 1.4 </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public void writeUnshared(Object obj) throws IOException
		public virtual void WriteUnshared(Object obj)
		{
			try
			{
				WriteObject0(obj, true);
			}
			catch (IOException ex)
			{
				if (Depth == 0)
				{
					WriteFatalException(ex);
				}
				throw ex;
			}
		}

		/// <summary>
		/// Write the non-static and non-transient fields of the current class to
		/// this stream.  This may only be called from the writeObject method of the
		/// class being serialized. It will throw the NotActiveException if it is
		/// called otherwise.
		/// </summary>
		/// <exception cref="IOException"> if I/O errors occur while writing to the underlying
		///          <code>OutputStream</code> </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public void defaultWriteObject() throws IOException
		public virtual void DefaultWriteObject()
		{
			SerialCallbackContext ctx = CurContext;
			if (ctx == null)
			{
				throw new NotActiveException("not in call to writeObject");
			}
			Object curObj = ctx.Obj;
			ObjectStreamClass curDesc = ctx.Desc;
			Bout.BlockDataMode = false;
			DefaultWriteFields(curObj, curDesc);
			Bout.BlockDataMode = true;
		}

		/// <summary>
		/// Retrieve the object used to buffer persistent fields to be written to
		/// the stream.  The fields will be written to the stream when writeFields
		/// method is called.
		/// </summary>
		/// <returns>  an instance of the class Putfield that holds the serializable
		///          fields </returns>
		/// <exception cref="IOException"> if I/O errors occur
		/// @since 1.2 </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public ObjectOutputStream.PutField putFields() throws IOException
		public virtual ObjectOutputStream.PutField PutFields()
		{
			if (CurPut == null)
			{
				SerialCallbackContext ctx = CurContext;
				if (ctx == null)
				{
					throw new NotActiveException("not in call to writeObject");
				}
				Object curObj = ctx.Obj;
				ObjectStreamClass curDesc = ctx.Desc;
				CurPut = new PutFieldImpl(this, curDesc);
			}
			return CurPut;
		}

		/// <summary>
		/// Write the buffered fields to the stream.
		/// </summary>
		/// <exception cref="IOException"> if I/O errors occur while writing to the underlying
		///          stream </exception>
		/// <exception cref="NotActiveException"> Called when a classes writeObject method was
		///          not called to write the state of the object.
		/// @since 1.2 </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public void writeFields() throws IOException
		public virtual void WriteFields()
		{
			if (CurPut == null)
			{
				throw new NotActiveException("no current PutField object");
			}
			Bout.BlockDataMode = false;
			CurPut.WriteFields();
			Bout.BlockDataMode = true;
		}

		/// <summary>
		/// Reset will disregard the state of any objects already written to the
		/// stream.  The state is reset to be the same as a new ObjectOutputStream.
		/// The current point in the stream is marked as reset so the corresponding
		/// ObjectInputStream will be reset at the same point.  Objects previously
		/// written to the stream will not be referred to as already being in the
		/// stream.  They will be written to the stream again.
		/// </summary>
		/// <exception cref="IOException"> if reset() is invoked while serializing an object. </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public void reset() throws IOException
		public virtual void Reset()
		{
			if (Depth != 0)
			{
				throw new IOException("stream active");
			}
			Bout.BlockDataMode = false;
			Bout.WriteByte(ObjectStreamConstants_Fields.TC_RESET);
			Clear();
			Bout.BlockDataMode = true;
		}

		/// <summary>
		/// Subclasses may implement this method to allow class data to be stored in
		/// the stream. By default this method does nothing.  The corresponding
		/// method in ObjectInputStream is resolveClass.  This method is called
		/// exactly once for each unique class in the stream.  The class name and
		/// signature will have already been written to the stream.  This method may
		/// make free use of the ObjectOutputStream to save any representation of
		/// the class it deems suitable (for example, the bytes of the class file).
		/// The resolveClass method in the corresponding subclass of
		/// ObjectInputStream must read and use any data or objects written by
		/// annotateClass.
		/// </summary>
		/// <param name="cl"> the class to annotate custom data for </param>
		/// <exception cref="IOException"> Any exception thrown by the underlying
		///          OutputStream. </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: protected void annotateClass(Class cl) throws IOException
		protected internal virtual void AnnotateClass(Class cl)
		{
		}

		/// <summary>
		/// Subclasses may implement this method to store custom data in the stream
		/// along with descriptors for dynamic proxy classes.
		/// 
		/// <para>This method is called exactly once for each unique proxy class
		/// descriptor in the stream.  The default implementation of this method in
		/// <code>ObjectOutputStream</code> does nothing.
		/// 
		/// </para>
		/// <para>The corresponding method in <code>ObjectInputStream</code> is
		/// <code>resolveProxyClass</code>.  For a given subclass of
		/// <code>ObjectOutputStream</code> that overrides this method, the
		/// <code>resolveProxyClass</code> method in the corresponding subclass of
		/// <code>ObjectInputStream</code> must read any data or objects written by
		/// <code>annotateProxyClass</code>.
		/// 
		/// </para>
		/// </summary>
		/// <param name="cl"> the proxy class to annotate custom data for </param>
		/// <exception cref="IOException"> any exception thrown by the underlying
		///          <code>OutputStream</code> </exception>
		/// <seealso cref= ObjectInputStream#resolveProxyClass(String[])
		/// @since   1.3 </seealso>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: protected void annotateProxyClass(Class cl) throws IOException
		protected internal virtual void AnnotateProxyClass(Class cl)
		{
		}

		/// <summary>
		/// This method will allow trusted subclasses of ObjectOutputStream to
		/// substitute one object for another during serialization. Replacing
		/// objects is disabled until enableReplaceObject is called. The
		/// enableReplaceObject method checks that the stream requesting to do
		/// replacement can be trusted.  The first occurrence of each object written
		/// into the serialization stream is passed to replaceObject.  Subsequent
		/// references to the object are replaced by the object returned by the
		/// original call to replaceObject.  To ensure that the private state of
		/// objects is not unintentionally exposed, only trusted streams may use
		/// replaceObject.
		/// 
		/// <para>The ObjectOutputStream.writeObject method takes a parameter of type
		/// Object (as opposed to type Serializable) to allow for cases where
		/// non-serializable objects are replaced by serializable ones.
		/// 
		/// </para>
		/// <para>When a subclass is replacing objects it must insure that either a
		/// complementary substitution must be made during deserialization or that
		/// the substituted object is compatible with every field where the
		/// reference will be stored.  Objects whose type is not a subclass of the
		/// type of the field or array element abort the serialization by raising an
		/// exception and the object is not be stored.
		/// 
		/// </para>
		/// <para>This method is called only once when each object is first
		/// encountered.  All subsequent references to the object will be redirected
		/// to the new object. This method should return the object to be
		/// substituted or the original object.
		/// 
		/// </para>
		/// <para>Null can be returned as the object to be substituted, but may cause
		/// NullReferenceException in classes that contain references to the
		/// original object since they may be expecting an object instead of
		/// null.
		/// 
		/// </para>
		/// </summary>
		/// <param name="obj"> the object to be replaced </param>
		/// <returns>  the alternate object that replaced the specified one </returns>
		/// <exception cref="IOException"> Any exception thrown by the underlying
		///          OutputStream. </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: protected Object replaceObject(Object obj) throws IOException
		protected internal virtual Object ReplaceObject(Object obj)
		{
			return obj;
		}

		/// <summary>
		/// Enable the stream to do replacement of objects in the stream.  When
		/// enabled, the replaceObject method is called for every object being
		/// serialized.
		/// 
		/// <para>If <code>enable</code> is true, and there is a security manager
		/// installed, this method first calls the security manager's
		/// <code>checkPermission</code> method with a
		/// <code>SerializablePermission("enableSubstitution")</code> permission to
		/// ensure it's ok to enable the stream to do replacement of objects in the
		/// stream.
		/// 
		/// </para>
		/// </summary>
		/// <param name="enable"> boolean parameter to enable replacement of objects </param>
		/// <returns>  the previous setting before this method was invoked </returns>
		/// <exception cref="SecurityException"> if a security manager exists and its
		///          <code>checkPermission</code> method denies enabling the stream
		///          to do replacement of objects in the stream. </exception>
		/// <seealso cref= SecurityManager#checkPermission </seealso>
		/// <seealso cref= java.io.SerializablePermission </seealso>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: protected boolean enableReplaceObject(boolean enable) throws SecurityException
		protected internal virtual bool EnableReplaceObject(bool enable)
		{
			if (enable == EnableReplace)
			{
				return enable;
			}
			if (enable)
			{
				SecurityManager sm = System.SecurityManager;
				if (sm != null)
				{
					sm.CheckPermission(ObjectStreamConstants_Fields.SUBSTITUTION_PERMISSION);
				}
			}
			EnableReplace = enable;
			return !EnableReplace;
		}

		/// <summary>
		/// The writeStreamHeader method is provided so subclasses can append or
		/// prepend their own header to the stream.  It writes the magic number and
		/// version to the stream.
		/// </summary>
		/// <exception cref="IOException"> if I/O errors occur while writing to the underlying
		///          stream </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: protected void writeStreamHeader() throws IOException
		protected internal virtual void WriteStreamHeader()
		{
			Bout.WriteShort(ObjectStreamConstants_Fields.STREAM_MAGIC);
			Bout.WriteShort(ObjectStreamConstants_Fields.STREAM_VERSION);
		}

		/// <summary>
		/// Write the specified class descriptor to the ObjectOutputStream.  Class
		/// descriptors are used to identify the classes of objects written to the
		/// stream.  Subclasses of ObjectOutputStream may override this method to
		/// customize the way in which class descriptors are written to the
		/// serialization stream.  The corresponding method in ObjectInputStream,
		/// <code>readClassDescriptor</code>, should then be overridden to
		/// reconstitute the class descriptor from its custom stream representation.
		/// By default, this method writes class descriptors according to the format
		/// defined in the Object Serialization specification.
		/// 
		/// <para>Note that this method will only be called if the ObjectOutputStream
		/// is not using the old serialization stream format (set by calling
		/// ObjectOutputStream's <code>useProtocolVersion</code> method).  If this
		/// serialization stream is using the old format
		/// (<code>PROTOCOL_VERSION_1</code>), the class descriptor will be written
		/// internally in a manner that cannot be overridden or customized.
		/// 
		/// </para>
		/// </summary>
		/// <param name="desc"> class descriptor to write to the stream </param>
		/// <exception cref="IOException"> If an I/O error has occurred. </exception>
		/// <seealso cref= java.io.ObjectInputStream#readClassDescriptor() </seealso>
		/// <seealso cref= #useProtocolVersion(int) </seealso>
		/// <seealso cref= java.io.ObjectStreamConstants#PROTOCOL_VERSION_1
		/// @since 1.3 </seealso>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: protected void writeClassDescriptor(ObjectStreamClass desc) throws IOException
		protected internal virtual void WriteClassDescriptor(ObjectStreamClass desc)
		{
			desc.WriteNonProxy(this);
		}

		/// <summary>
		/// Writes a byte. This method will block until the byte is actually
		/// written.
		/// </summary>
		/// <param name="val"> the byte to be written to the stream </param>
		/// <exception cref="IOException"> If an I/O error has occurred. </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public void write(int val) throws IOException
		public override void Write(int val)
		{
			Bout.Write(val);
		}

		/// <summary>
		/// Writes an array of bytes. This method will block until the bytes are
		/// actually written.
		/// </summary>
		/// <param name="buf"> the data to be written </param>
		/// <exception cref="IOException"> If an I/O error has occurred. </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public void write(byte[] buf) throws IOException
		public override void Write(sbyte[] buf)
		{
			Bout.Write(buf, 0, buf.Length, false);
		}

		/// <summary>
		/// Writes a sub array of bytes.
		/// </summary>
		/// <param name="buf"> the data to be written </param>
		/// <param name="off"> the start offset in the data </param>
		/// <param name="len"> the number of bytes that are written </param>
		/// <exception cref="IOException"> If an I/O error has occurred. </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public void write(byte[] buf, int off, int len) throws IOException
		public override void Write(sbyte[] buf, int off, int len)
		{
			if (buf == null)
			{
				throw new NullPointerException();
			}
			int endoff = off + len;
			if (off < 0 || len < 0 || endoff > buf.Length || endoff < 0)
			{
				throw new IndexOutOfBoundsException();
			}
			Bout.Write(buf, off, len, false);
		}

		/// <summary>
		/// Flushes the stream. This will write any buffered output bytes and flush
		/// through to the underlying stream.
		/// </summary>
		/// <exception cref="IOException"> If an I/O error has occurred. </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public void flush() throws IOException
		public override void Flush()
		{
			Bout.Flush();
		}

		/// <summary>
		/// Drain any buffered data in ObjectOutputStream.  Similar to flush but
		/// does not propagate the flush to the underlying stream.
		/// </summary>
		/// <exception cref="IOException"> if I/O errors occur while writing to the underlying
		///          stream </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: protected void drain() throws IOException
		protected internal virtual void Drain()
		{
			Bout.Drain();
		}

		/// <summary>
		/// Closes the stream. This method must be called to release any resources
		/// associated with the stream.
		/// </summary>
		/// <exception cref="IOException"> If an I/O error has occurred. </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public void close() throws IOException
		public override void Close()
		{
			Flush();
			Clear();
			Bout.Close();
		}

		/// <summary>
		/// Writes a boolean.
		/// </summary>
		/// <param name="val"> the boolean to be written </param>
		/// <exception cref="IOException"> if I/O errors occur while writing to the underlying
		///          stream </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public void writeBoolean(boolean val) throws IOException
		public virtual void WriteBoolean(bool val)
		{
			Bout.WriteBoolean(val);
		}

		/// <summary>
		/// Writes an 8 bit byte.
		/// </summary>
		/// <param name="val"> the byte value to be written </param>
		/// <exception cref="IOException"> if I/O errors occur while writing to the underlying
		///          stream </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public void writeByte(int val) throws IOException
		public virtual void WriteByte(int val)
		{
			Bout.WriteByte(val);
		}

		/// <summary>
		/// Writes a 16 bit short.
		/// </summary>
		/// <param name="val"> the short value to be written </param>
		/// <exception cref="IOException"> if I/O errors occur while writing to the underlying
		///          stream </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public void writeShort(int val) throws IOException
		public virtual void WriteShort(int val)
		{
			Bout.WriteShort(val);
		}

		/// <summary>
		/// Writes a 16 bit char.
		/// </summary>
		/// <param name="val"> the char value to be written </param>
		/// <exception cref="IOException"> if I/O errors occur while writing to the underlying
		///          stream </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public void writeChar(int val) throws IOException
		public virtual void WriteChar(int val)
		{
			Bout.WriteChar(val);
		}

		/// <summary>
		/// Writes a 32 bit int.
		/// </summary>
		/// <param name="val"> the integer value to be written </param>
		/// <exception cref="IOException"> if I/O errors occur while writing to the underlying
		///          stream </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public void writeInt(int val) throws IOException
		public virtual void WriteInt(int val)
		{
			Bout.WriteInt(val);
		}

		/// <summary>
		/// Writes a 64 bit long.
		/// </summary>
		/// <param name="val"> the long value to be written </param>
		/// <exception cref="IOException"> if I/O errors occur while writing to the underlying
		///          stream </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public void writeLong(long val) throws IOException
		public virtual void WriteLong(long val)
		{
			Bout.WriteLong(val);
		}

		/// <summary>
		/// Writes a 32 bit float.
		/// </summary>
		/// <param name="val"> the float value to be written </param>
		/// <exception cref="IOException"> if I/O errors occur while writing to the underlying
		///          stream </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public void writeFloat(float val) throws IOException
		public virtual void WriteFloat(float val)
		{
			Bout.WriteFloat(val);
		}

		/// <summary>
		/// Writes a 64 bit double.
		/// </summary>
		/// <param name="val"> the double value to be written </param>
		/// <exception cref="IOException"> if I/O errors occur while writing to the underlying
		///          stream </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public void writeDouble(double val) throws IOException
		public virtual void WriteDouble(double val)
		{
			Bout.WriteDouble(val);
		}

		/// <summary>
		/// Writes a String as a sequence of bytes.
		/// </summary>
		/// <param name="str"> the String of bytes to be written </param>
		/// <exception cref="IOException"> if I/O errors occur while writing to the underlying
		///          stream </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public void writeBytes(String str) throws IOException
		public virtual void WriteBytes(String str)
		{
			Bout.WriteBytes(str);
		}

		/// <summary>
		/// Writes a String as a sequence of chars.
		/// </summary>
		/// <param name="str"> the String of chars to be written </param>
		/// <exception cref="IOException"> if I/O errors occur while writing to the underlying
		///          stream </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public void writeChars(String str) throws IOException
		public virtual void WriteChars(String str)
		{
			Bout.WriteChars(str);
		}

		/// <summary>
		/// Primitive data write of this String in
		/// <a href="DataInput.html#modified-utf-8">modified UTF-8</a>
		/// format.  Note that there is a
		/// significant difference between writing a String into the stream as
		/// primitive data or as an Object. A String instance written by writeObject
		/// is written into the stream as a String initially. Future writeObject()
		/// calls write references to the string into the stream.
		/// </summary>
		/// <param name="str"> the String to be written </param>
		/// <exception cref="IOException"> if I/O errors occur while writing to the underlying
		///          stream </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public void writeUTF(String str) throws IOException
		public virtual void WriteUTF(String str)
		{
			Bout.WriteUTF(str);
		}

		/// <summary>
		/// Provide programmatic access to the persistent fields to be written
		/// to ObjectOutput.
		/// 
		/// @since 1.2
		/// </summary>
		public abstract class PutField
		{

			/// <summary>
			/// Put the value of the named boolean field into the persistent field.
			/// </summary>
			/// <param name="name"> the name of the serializable field </param>
			/// <param name="val"> the value to assign to the field </param>
			/// <exception cref="IllegalArgumentException"> if <code>name</code> does not
			/// match the name of a serializable field for the class whose fields
			/// are being written, or if the type of the named field is not
			/// <code>boolean</code> </exception>
			public abstract void Put(String name, bool val);

			/// <summary>
			/// Put the value of the named byte field into the persistent field.
			/// </summary>
			/// <param name="name"> the name of the serializable field </param>
			/// <param name="val"> the value to assign to the field </param>
			/// <exception cref="IllegalArgumentException"> if <code>name</code> does not
			/// match the name of a serializable field for the class whose fields
			/// are being written, or if the type of the named field is not
			/// <code>byte</code> </exception>
			public abstract void Put(String name, sbyte val);

			/// <summary>
			/// Put the value of the named char field into the persistent field.
			/// </summary>
			/// <param name="name"> the name of the serializable field </param>
			/// <param name="val"> the value to assign to the field </param>
			/// <exception cref="IllegalArgumentException"> if <code>name</code> does not
			/// match the name of a serializable field for the class whose fields
			/// are being written, or if the type of the named field is not
			/// <code>char</code> </exception>
			public abstract void Put(String name, char val);

			/// <summary>
			/// Put the value of the named short field into the persistent field.
			/// </summary>
			/// <param name="name"> the name of the serializable field </param>
			/// <param name="val"> the value to assign to the field </param>
			/// <exception cref="IllegalArgumentException"> if <code>name</code> does not
			/// match the name of a serializable field for the class whose fields
			/// are being written, or if the type of the named field is not
			/// <code>short</code> </exception>
			public abstract void Put(String name, short val);

			/// <summary>
			/// Put the value of the named int field into the persistent field.
			/// </summary>
			/// <param name="name"> the name of the serializable field </param>
			/// <param name="val"> the value to assign to the field </param>
			/// <exception cref="IllegalArgumentException"> if <code>name</code> does not
			/// match the name of a serializable field for the class whose fields
			/// are being written, or if the type of the named field is not
			/// <code>int</code> </exception>
			public abstract void Put(String name, int val);

			/// <summary>
			/// Put the value of the named long field into the persistent field.
			/// </summary>
			/// <param name="name"> the name of the serializable field </param>
			/// <param name="val"> the value to assign to the field </param>
			/// <exception cref="IllegalArgumentException"> if <code>name</code> does not
			/// match the name of a serializable field for the class whose fields
			/// are being written, or if the type of the named field is not
			/// <code>long</code> </exception>
			public abstract void Put(String name, long val);

			/// <summary>
			/// Put the value of the named float field into the persistent field.
			/// </summary>
			/// <param name="name"> the name of the serializable field </param>
			/// <param name="val"> the value to assign to the field </param>
			/// <exception cref="IllegalArgumentException"> if <code>name</code> does not
			/// match the name of a serializable field for the class whose fields
			/// are being written, or if the type of the named field is not
			/// <code>float</code> </exception>
			public abstract void Put(String name, float val);

			/// <summary>
			/// Put the value of the named double field into the persistent field.
			/// </summary>
			/// <param name="name"> the name of the serializable field </param>
			/// <param name="val"> the value to assign to the field </param>
			/// <exception cref="IllegalArgumentException"> if <code>name</code> does not
			/// match the name of a serializable field for the class whose fields
			/// are being written, or if the type of the named field is not
			/// <code>double</code> </exception>
			public abstract void Put(String name, double val);

			/// <summary>
			/// Put the value of the named Object field into the persistent field.
			/// </summary>
			/// <param name="name"> the name of the serializable field </param>
			/// <param name="val"> the value to assign to the field
			///         (which may be <code>null</code>) </param>
			/// <exception cref="IllegalArgumentException"> if <code>name</code> does not
			/// match the name of a serializable field for the class whose fields
			/// are being written, or if the type of the named field is not a
			/// reference type </exception>
			public abstract void Put(String name, Object val);

			/// <summary>
			/// Write the data and fields to the specified ObjectOutput stream,
			/// which must be the same stream that produced this
			/// <code>PutField</code> object.
			/// </summary>
			/// <param name="out"> the stream to write the data and fields to </param>
			/// <exception cref="IOException"> if I/O errors occur while writing to the
			///         underlying stream </exception>
			/// <exception cref="IllegalArgumentException"> if the specified stream is not
			///         the same stream that produced this <code>PutField</code>
			///         object </exception>
			/// @deprecated This method does not write the values contained by this
			///         <code>PutField</code> object in a proper format, and may
			///         result in corruption of the serialization stream.  The
			///         correct way to write <code>PutField</code> data is by
			///         calling the <seealso cref="java.io.ObjectOutputStream#writeFields()"/>
			///         method. 
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: @Deprecated("This method does not write the values contained by this") public abstract void write(ObjectOutput out) throws IOException;
			[Obsolete("This method does not write the values contained by this")]
			public abstract void Write(ObjectOutput @out);
		}


		/// <summary>
		/// Returns protocol version in use.
		/// </summary>
		internal virtual int ProtocolVersion
		{
			get
			{
				return Protocol;
			}
		}

		/// <summary>
		/// Writes string without allowing it to be replaced in stream.  Used by
		/// ObjectStreamClass to write class descriptor type strings.
		/// </summary>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: void writeTypeString(String str) throws IOException
		internal virtual void WriteTypeString(String str)
		{
			int handle;
			if (str == null)
			{
				WriteNull();
			}
			else if ((handle = Handles.Lookup(str)) != -1)
			{
				WriteHandle(handle);
			}
			else
			{
				WriteString(str, false);
			}
		}

		/// <summary>
		/// Verifies that this (possibly subclass) instance can be constructed
		/// without violating security constraints: the subclass must not override
		/// security-sensitive non-final methods, or else the
		/// "enableSubclassImplementation" SerializablePermission is checked.
		/// </summary>
		private void VerifySubclass()
		{
			Class cl = this.GetType();
			if (cl == typeof(ObjectOutputStream))
			{
				return;
			}
			SecurityManager sm = System.SecurityManager;
			if (sm == null)
			{
				return;
			}
			processQueue(Caches.SubclassAuditsQueue, Caches.SubclassAudits);
			WeakClassKey key = new WeakClassKey(cl, Caches.SubclassAuditsQueue);
			Boolean result = Caches.SubclassAudits[key];
			if (result == null)
			{
				result = Convert.ToBoolean(AuditSubclass(cl));
				Caches.SubclassAudits.PutIfAbsent(key, result);
			}
			if (result.BooleanValue())
			{
				return;
			}
			sm.CheckPermission(ObjectStreamConstants_Fields.SUBCLASS_IMPLEMENTATION_PERMISSION);
		}

		/// <summary>
		/// Performs reflective checks on given subclass to verify that it doesn't
		/// override security-sensitive non-final methods.  Returns true if subclass
		/// is "safe", false otherwise.
		/// </summary>
//JAVA TO C# CONVERTER WARNING: 'final' parameters are not allowed in .NET:
//ORIGINAL LINE: private static boolean auditSubclass(final Class subcl)
		private static bool AuditSubclass(Class subcl)
		{
			Boolean result = AccessController.doPrivileged(new PrivilegedActionAnonymousInnerClassHelper(subcl)
		   );
			return result.BooleanValue();
		}

		private class PrivilegedActionAnonymousInnerClassHelper : PrivilegedAction<Boolean>
		{
			private Type Subcl;

			public PrivilegedActionAnonymousInnerClassHelper(Type subcl)
			{
				this.Subcl = subcl;
			}

			public virtual Boolean Run()
			{
				for (Class cl = Subcl; cl != typeof(ObjectOutputStream); cl = cl.BaseType)
				{
					try
					{
						cl.GetDeclaredMethod("writeUnshared", new Class[] {typeof(Object)});
						return false;
					}
					catch (NoSuchMethodException)
					{
					}
					try
					{
						cl.getDeclaredMethod("putFields", (Class[]) null);
						return false;
					}
					catch (NoSuchMethodException)
					{
					}
				}
				return true;
			}
		}

		/// <summary>
		/// Clears internal data structures.
		/// </summary>
		private void Clear()
		{
			Subs.Clear();
			Handles.Clear();
		}

		/// <summary>
		/// Underlying writeObject/writeUnshared implementation.
		/// </summary>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: private void writeObject0(Object obj, boolean unshared) throws IOException
		private void WriteObject0(Object obj, bool unshared)
		{
			bool oldMode = Bout.setBlockDataMode(false);
			Depth++;
			try
			{
				// handle previously written and non-replaceable objects
				int h;
				if ((obj = Subs.Lookup(obj)) == null)
				{
					WriteNull();
					return;
				}
				else if (!unshared && (h = Handles.Lookup(obj)) != -1)
				{
					WriteHandle(h);
					return;
				}
				else if (obj is Class)
				{
					WriteClass((Class) obj, unshared);
					return;
				}
				else if (obj is ObjectStreamClass)
				{
					WriteClassDesc((ObjectStreamClass) obj, unshared);
					return;
				}

				// check for replacement object
				Object orig = obj;
				Class cl = obj.GetType();
				ObjectStreamClass desc;
				for (;;)
				{
					// REMIND: skip this check for strings/arrays?
					Class repCl;
					desc = ObjectStreamClass.Lookup(cl, true);
					if (!desc.HasWriteReplaceMethod() || (obj = desc.InvokeWriteReplace(obj)) == null || (repCl = obj.GetType()) == cl)
					{
						break;
					}
					cl = repCl;
				}
				if (EnableReplace)
				{
					Object rep = ReplaceObject(obj);
					if (rep != obj && rep != null)
					{
						cl = rep.GetType();
						desc = ObjectStreamClass.Lookup(cl, true);
					}
					obj = rep;
				}

				// if object replaced, run through original checks a second time
				if (obj != orig)
				{
					Subs.Assign(orig, obj);
					if (obj == null)
					{
						WriteNull();
						return;
					}
					else if (!unshared && (h = Handles.Lookup(obj)) != -1)
					{
						WriteHandle(h);
						return;
					}
					else if (obj is Class)
					{
						WriteClass((Class) obj, unshared);
						return;
					}
					else if (obj is ObjectStreamClass)
					{
						WriteClassDesc((ObjectStreamClass) obj, unshared);
						return;
					}
				}

				// remaining cases
				if (obj is String)
				{
					WriteString((String) obj, unshared);
				}
				else if (cl.Array)
				{
					WriteArray(obj, desc, unshared);
				}
				else if (obj is Enum)
				{
//JAVA TO C# CONVERTER TODO TASK: Java wildcard generics are not converted to .NET:
//ORIGINAL LINE: writeEnum((Enum<?>) obj, desc, unshared);
					WriteEnum((Enum<?>) obj, desc, unshared);
				}
				else if (obj is Serializable)
				{
					WriteOrdinaryObject(obj, desc, unshared);
				}
				else
				{
					if (ExtendedDebugInfo)
					{
						throw new NotSerializableException(cl.Name + "\n" + DebugInfoStack.ToString());
					}
					else
					{
						throw new NotSerializableException(cl.Name);
					}
				}
			}
			finally
			{
				Depth--;
				Bout.BlockDataMode = oldMode;
			}
		}

		/// <summary>
		/// Writes null code to stream.
		/// </summary>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: private void writeNull() throws IOException
		private void WriteNull()
		{
			Bout.WriteByte(ObjectStreamConstants_Fields.TC_NULL);
		}

		/// <summary>
		/// Writes given object handle to stream.
		/// </summary>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: private void writeHandle(int handle) throws IOException
		private void WriteHandle(int handle)
		{
			Bout.WriteByte(ObjectStreamConstants_Fields.TC_REFERENCE);
			Bout.WriteInt(ObjectStreamConstants_Fields.BaseWireHandle + handle);
		}

		/// <summary>
		/// Writes representation of given class to stream.
		/// </summary>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: private void writeClass(Class cl, boolean unshared) throws IOException
		private void WriteClass(Class cl, bool unshared)
		{
			Bout.WriteByte(ObjectStreamConstants_Fields.TC_CLASS);
			WriteClassDesc(ObjectStreamClass.Lookup(cl, true), false);
			Handles.Assign(unshared ? null : cl);
		}

		/// <summary>
		/// Writes representation of given class descriptor to stream.
		/// </summary>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: private void writeClassDesc(ObjectStreamClass desc, boolean unshared) throws IOException
		private void WriteClassDesc(ObjectStreamClass desc, bool unshared)
		{
			int handle;
			if (desc == null)
			{
				WriteNull();
			}
			else if (!unshared && (handle = Handles.Lookup(desc)) != -1)
			{
				WriteHandle(handle);
			}
			else if (desc.Proxy)
			{
				WriteProxyDesc(desc, unshared);
			}
			else
			{
				WriteNonProxyDesc(desc, unshared);
			}
		}

		private bool CustomSubclass
		{
			get
			{
				// Return true if this class is a custom subclass of ObjectOutputStream
				return this.GetType().ClassLoader != typeof(ObjectOutputStream).ClassLoader;
			}
		}

		/// <summary>
		/// Writes class descriptor representing a dynamic proxy class to stream.
		/// </summary>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: private void writeProxyDesc(ObjectStreamClass desc, boolean unshared) throws IOException
		private void WriteProxyDesc(ObjectStreamClass desc, bool unshared)
		{
			Bout.WriteByte(ObjectStreamConstants_Fields.TC_PROXYCLASSDESC);
			Handles.Assign(unshared ? null : desc);

			Class cl = desc.ForClass();
			Class[] ifaces = cl.Interfaces;
			Bout.WriteInt(ifaces.Length);
			for (int i = 0; i < ifaces.Length; i++)
			{
				Bout.WriteUTF(ifaces[i].Name);
			}

			Bout.BlockDataMode = true;
			if (cl != null && CustomSubclass)
			{
				ReflectUtil.checkPackageAccess(cl);
			}
			AnnotateProxyClass(cl);
			Bout.BlockDataMode = false;
			Bout.WriteByte(ObjectStreamConstants_Fields.TC_ENDBLOCKDATA);

			WriteClassDesc(desc.SuperDesc, false);
		}

		/// <summary>
		/// Writes class descriptor representing a standard (i.e., not a dynamic
		/// proxy) class to stream.
		/// </summary>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: private void writeNonProxyDesc(ObjectStreamClass desc, boolean unshared) throws IOException
		private void WriteNonProxyDesc(ObjectStreamClass desc, bool unshared)
		{
			Bout.WriteByte(ObjectStreamConstants_Fields.TC_CLASSDESC);
			Handles.Assign(unshared ? null : desc);

			if (Protocol == ObjectStreamConstants_Fields.PROTOCOL_VERSION_1)
			{
				// do not invoke class descriptor write hook with old protocol
				desc.WriteNonProxy(this);
			}
			else
			{
				WriteClassDescriptor(desc);
			}

			Class cl = desc.ForClass();
			Bout.BlockDataMode = true;
			if (cl != null && CustomSubclass)
			{
				ReflectUtil.checkPackageAccess(cl);
			}
			AnnotateClass(cl);
			Bout.BlockDataMode = false;
			Bout.WriteByte(ObjectStreamConstants_Fields.TC_ENDBLOCKDATA);

			WriteClassDesc(desc.SuperDesc, false);
		}

		/// <summary>
		/// Writes given string to stream, using standard or long UTF format
		/// depending on string length.
		/// </summary>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: private void writeString(String str, boolean unshared) throws IOException
		private void WriteString(String str, bool unshared)
		{
			Handles.Assign(unshared ? null : str);
			long utflen = Bout.GetUTFLength(str);
			if (utflen <= 0xFFFF)
			{
				Bout.WriteByte(ObjectStreamConstants_Fields.TC_STRING);
				Bout.WriteUTF(str, utflen);
			}
			else
			{
				Bout.WriteByte(ObjectStreamConstants_Fields.TC_LONGSTRING);
				Bout.WriteLongUTF(str, utflen);
			}
		}

		/// <summary>
		/// Writes given array object to stream.
		/// </summary>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: private void writeArray(Object array, ObjectStreamClass desc, boolean unshared) throws IOException
		private void WriteArray(Object array, ObjectStreamClass desc, bool unshared)
		{
			Bout.WriteByte(ObjectStreamConstants_Fields.TC_ARRAY);
			WriteClassDesc(desc, false);
			Handles.Assign(unshared ? null : array);

			Class ccl = desc.ForClass().ComponentType;
			if (ccl.Primitive)
			{
				if (ccl == Integer.TYPE)
				{
					int[] ia = (int[]) array;
					Bout.WriteInt(ia.Length);
					Bout.WriteInts(ia, 0, ia.Length);
				}
				else if (ccl == Byte.TYPE)
				{
					sbyte[] ba = (sbyte[]) array;
					Bout.WriteInt(ba.Length);
					Bout.Write(ba, 0, ba.Length, true);
				}
				else if (ccl == Long.TYPE)
				{
					long[] ja = (long[]) array;
					Bout.WriteInt(ja.Length);
					Bout.WriteLongs(ja, 0, ja.Length);
				}
				else if (ccl == Float.TYPE)
				{
					float[] fa = (float[]) array;
					Bout.WriteInt(fa.Length);
					Bout.WriteFloats(fa, 0, fa.Length);
				}
				else if (ccl == Double.TYPE)
				{
					double[] da = (double[]) array;
					Bout.WriteInt(da.Length);
					Bout.WriteDoubles(da, 0, da.Length);
				}
				else if (ccl == Short.TYPE)
				{
					short[] sa = (short[]) array;
					Bout.WriteInt(sa.Length);
					Bout.WriteShorts(sa, 0, sa.Length);
				}
				else if (ccl == Character.TYPE)
				{
					char[] ca = (char[]) array;
					Bout.WriteInt(ca.Length);
					Bout.WriteChars(ca, 0, ca.Length);
				}
				else if (ccl == Boolean.TYPE)
				{
					bool[] za = (bool[]) array;
					Bout.WriteInt(za.Length);
					Bout.WriteBooleans(za, 0, za.Length);
				}
				else
				{
					throw new InternalError();
				}
			}
			else
			{
				Object[] objs = (Object[]) array;
				int len = objs.Length;
				Bout.WriteInt(len);
				if (ExtendedDebugInfo)
				{
//JAVA TO C# CONVERTER WARNING: The .NET Type.FullName property will not always yield results identical to the Java Class.getName method:
					DebugInfoStack.Push("array (class \"" + array.GetType().FullName + "\", size: " + len + ")");
				}
				try
				{
					for (int i = 0; i < len; i++)
					{
						if (ExtendedDebugInfo)
						{
							DebugInfoStack.Push("element of array (index: " + i + ")");
						}
						try
						{
							WriteObject0(objs[i], false);
						}
						finally
						{
							if (ExtendedDebugInfo)
							{
								DebugInfoStack.Pop();
							}
						}
					}
				}
				finally
				{
					if (ExtendedDebugInfo)
					{
						DebugInfoStack.Pop();
					}
				}
			}
		}

		/// <summary>
		/// Writes given enum constant to stream.
		/// </summary>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: private void writeEnum(Enum<?> en, ObjectStreamClass desc, boolean unshared) throws IOException
		private void writeEnum<T1>(Enum<T1> en, ObjectStreamClass desc, bool unshared)
		{
			Bout.WriteByte(ObjectStreamConstants_Fields.TC_ENUM);
			ObjectStreamClass sdesc = desc.SuperDesc;
			WriteClassDesc((sdesc.ForClass() == typeof(Enum)) ? desc : sdesc, false);
			Handles.Assign(unshared ? null : en);
			WriteString(en.Name(), false);
		}

		/// <summary>
		/// Writes representation of a "ordinary" (i.e., not a String, Class,
		/// ObjectStreamClass, array, or enum constant) serializable object to the
		/// stream.
		/// </summary>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: private void writeOrdinaryObject(Object obj, ObjectStreamClass desc, boolean unshared) throws IOException
		private void WriteOrdinaryObject(Object obj, ObjectStreamClass desc, bool unshared)
		{
			if (ExtendedDebugInfo)
			{
//JAVA TO C# CONVERTER WARNING: The .NET Type.FullName property will not always yield results identical to the Java Class.getName method:
				DebugInfoStack.Push((Depth == 1 ? "root " : "") + "object (class \"" + obj.GetType().FullName + "\", " + obj.ToString() + ")");
			}
			try
			{
				desc.CheckSerialize();

				Bout.WriteByte(ObjectStreamConstants_Fields.TC_OBJECT);
				WriteClassDesc(desc, false);
				Handles.Assign(unshared ? null : obj);
				if (desc.Externalizable && !desc.Proxy)
				{
					WriteExternalData((Externalizable) obj);
				}
				else
				{
					WriteSerialData(obj, desc);
				}
			}
			finally
			{
				if (ExtendedDebugInfo)
				{
					DebugInfoStack.Pop();
				}
			}
		}

		/// <summary>
		/// Writes externalizable data of given object by invoking its
		/// writeExternal() method.
		/// </summary>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: private void writeExternalData(Externalizable obj) throws IOException
		private void WriteExternalData(Externalizable obj)
		{
			PutFieldImpl oldPut = CurPut;
			CurPut = null;

			if (ExtendedDebugInfo)
			{
				DebugInfoStack.Push("writeExternal data");
			}
			SerialCallbackContext oldContext = CurContext;
			try
			{
				CurContext = null;
				if (Protocol == ObjectStreamConstants_Fields.PROTOCOL_VERSION_1)
				{
					obj.WriteExternal(this);
				}
				else
				{
					Bout.BlockDataMode = true;
					obj.WriteExternal(this);
					Bout.BlockDataMode = false;
					Bout.WriteByte(ObjectStreamConstants_Fields.TC_ENDBLOCKDATA);
				}
			}
			finally
			{
				CurContext = oldContext;
				if (ExtendedDebugInfo)
				{
					DebugInfoStack.Pop();
				}
			}

			CurPut = oldPut;
		}

		/// <summary>
		/// Writes instance data for each serializable class of given object, from
		/// superclass to subclass.
		/// </summary>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: private void writeSerialData(Object obj, ObjectStreamClass desc) throws IOException
		private void WriteSerialData(Object obj, ObjectStreamClass desc)
		{
			ObjectStreamClass.ClassDataSlot[] slots = desc.ClassDataLayout;
			for (int i = 0; i < slots.Length; i++)
			{
				ObjectStreamClass slotDesc = slots[i].Desc;
				if (slotDesc.HasWriteObjectMethod())
				{
					PutFieldImpl oldPut = CurPut;
					CurPut = null;
					SerialCallbackContext oldContext = CurContext;

					if (ExtendedDebugInfo)
					{
						DebugInfoStack.Push("custom writeObject data (class \"" + slotDesc.Name + "\")");
					}
					try
					{
						CurContext = new SerialCallbackContext(obj, slotDesc);
						Bout.BlockDataMode = true;
						slotDesc.InvokeWriteObject(obj, this);
						Bout.BlockDataMode = false;
						Bout.WriteByte(ObjectStreamConstants_Fields.TC_ENDBLOCKDATA);
					}
					finally
					{
						CurContext.SetUsed();
						CurContext = oldContext;
						if (ExtendedDebugInfo)
						{
							DebugInfoStack.Pop();
						}
					}

					CurPut = oldPut;
				}
				else
				{
					DefaultWriteFields(obj, slotDesc);
				}
			}
		}

		/// <summary>
		/// Fetches and writes values of serializable fields of given object to
		/// stream.  The given class descriptor specifies which field values to
		/// write, and in which order they should be written.
		/// </summary>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: private void defaultWriteFields(Object obj, ObjectStreamClass desc) throws IOException
		private void DefaultWriteFields(Object obj, ObjectStreamClass desc)
		{
			Class cl = desc.ForClass();
			if (cl != null && obj != null && !cl.isInstance(obj))
			{
				throw new ClassCastException();
			}

			desc.CheckDefaultSerialize();

			int primDataSize = desc.PrimDataSize;
			if (PrimVals == null || PrimVals.Length < primDataSize)
			{
				PrimVals = new sbyte[primDataSize];
			}
			desc.GetPrimFieldValues(obj, PrimVals);
			Bout.Write(PrimVals, 0, primDataSize, false);

			ObjectStreamField[] fields = desc.GetFields(false);
			Object[] objVals = new Object[desc.NumObjFields];
			int numPrimFields = fields.Length - objVals.Length;
			desc.GetObjFieldValues(obj, objVals);
			for (int i = 0; i < objVals.Length; i++)
			{
				if (ExtendedDebugInfo)
				{
					DebugInfoStack.Push("field (class \"" + desc.Name + "\", name: \"" + fields[numPrimFields + i].Name + "\", type: \"" + fields[numPrimFields + i].Type + "\")");
				}
				try
				{
					WriteObject0(objVals[i], fields[numPrimFields + i].Unshared);
				}
				finally
				{
					if (ExtendedDebugInfo)
					{
						DebugInfoStack.Pop();
					}
				}
			}
		}

		/// <summary>
		/// Attempts to write to stream fatal IOException that has caused
		/// serialization to abort.
		/// </summary>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: private void writeFatalException(IOException ex) throws IOException
		private void WriteFatalException(IOException ex)
		{
			/*
			 * Note: the serialization specification states that if a second
			 * IOException occurs while attempting to serialize the original fatal
			 * exception to the stream, then a StreamCorruptedException should be
			 * thrown (section 2.1).  However, due to a bug in previous
			 * implementations of serialization, StreamCorruptedExceptions were
			 * rarely (if ever) actually thrown--the "root" exceptions from
			 * underlying streams were thrown instead.  This historical behavior is
			 * followed here for consistency.
			 */
			Clear();
			bool oldMode = Bout.setBlockDataMode(false);
			try
			{
				Bout.WriteByte(ObjectStreamConstants_Fields.TC_EXCEPTION);
				WriteObject0(ex, false);
				Clear();
			}
			finally
			{
				Bout.BlockDataMode = oldMode;
			}
		}

		/// <summary>
		/// Converts specified span of float values into byte values.
		/// </summary>
		// REMIND: remove once hotspot inlines Float.floatToIntBits
//JAVA TO C# CONVERTER TODO TASK: Replace 'unknown' with the appropriate dll name:
		[DllImport("unknown")]
		private static extern void floatsToBytes(float[] src, int srcpos, sbyte[] dst, int dstpos, int nfloats);

		/// <summary>
		/// Converts specified span of double values into byte values.
		/// </summary>
		// REMIND: remove once hotspot inlines Double.doubleToLongBits
//JAVA TO C# CONVERTER TODO TASK: Replace 'unknown' with the appropriate dll name:
		[DllImport("unknown")]
		private static extern void doublesToBytes(double[] src, int srcpos, sbyte[] dst, int dstpos, int ndoubles);

		/// <summary>
		/// Default PutField implementation.
		/// </summary>
		private class PutFieldImpl : PutField
		{
			private readonly ObjectOutputStream OuterInstance;


			/// <summary>
			/// class descriptor describing serializable fields </summary>
			internal readonly ObjectStreamClass Desc;
			/// <summary>
			/// primitive field values </summary>
			internal readonly sbyte[] PrimVals;
			/// <summary>
			/// object field values </summary>
			internal readonly Object[] ObjVals;

			/// <summary>
			/// Creates PutFieldImpl object for writing fields defined in given
			/// class descriptor.
			/// </summary>
			internal PutFieldImpl(ObjectOutputStream outerInstance, ObjectStreamClass desc)
			{
				this.OuterInstance = outerInstance;
				this.Desc = desc;
				PrimVals = new sbyte[desc.PrimDataSize];
				ObjVals = new Object[desc.NumObjFields];
			}

			public override void Put(String name, bool val)
			{
				Bits.PutBoolean(PrimVals, GetFieldOffset(name, Boolean.TYPE), val);
			}

			public override void Put(String name, sbyte val)
			{
				PrimVals[GetFieldOffset(name, Byte.TYPE)] = val;
			}

			public override void Put(String name, char val)
			{
				Bits.PutChar(PrimVals, GetFieldOffset(name, Character.TYPE), val);
			}

			public override void Put(String name, short val)
			{
				Bits.PutShort(PrimVals, GetFieldOffset(name, Short.TYPE), val);
			}

			public override void Put(String name, int val)
			{
				Bits.PutInt(PrimVals, GetFieldOffset(name, Integer.TYPE), val);
			}

			public override void Put(String name, float val)
			{
				Bits.PutFloat(PrimVals, GetFieldOffset(name, Float.TYPE), val);
			}

			public override void Put(String name, long val)
			{
				Bits.PutLong(PrimVals, GetFieldOffset(name, Long.TYPE), val);
			}

			public override void Put(String name, double val)
			{
				Bits.PutDouble(PrimVals, GetFieldOffset(name, Double.TYPE), val);
			}

			public override void Put(String name, Object val)
			{
				ObjVals[GetFieldOffset(name, typeof(Object))] = val;
			}

			// deprecated in ObjectOutputStream.PutField
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public void write(ObjectOutput out) throws IOException
			public override void Write(ObjectOutput @out)
			{
				/*
				 * Applications should *not* use this method to write PutField
				 * data, as it will lead to stream corruption if the PutField
				 * object writes any primitive data (since block data mode is not
				 * unset/set properly, as is done in OOS.writeFields()).  This
				 * broken implementation is being retained solely for behavioral
				 * compatibility, in order to support applications which use
				 * OOS.PutField.write() for writing only non-primitive data.
				 *
				 * Serialization of unshared objects is not implemented here since
				 * it is not necessary for backwards compatibility; also, unshared
				 * semantics may not be supported by the given ObjectOutput
				 * instance.  Applications which write unshared objects using the
				 * PutField API must use OOS.writeFields().
				 */
				if (OuterInstance != @out)
				{
					throw new IllegalArgumentException("wrong stream");
				}
				@out.Write(PrimVals, 0, PrimVals.Length);

				ObjectStreamField[] fields = Desc.GetFields(false);
				int numPrimFields = fields.Length - ObjVals.Length;
				// REMIND: warn if numPrimFields > 0?
				for (int i = 0; i < ObjVals.Length; i++)
				{
					if (fields[numPrimFields + i].Unshared)
					{
						throw new IOException("cannot write unshared object");
					}
					@out.WriteObject(ObjVals[i]);
				}
			}

			/// <summary>
			/// Writes buffered primitive data and object fields to stream.
			/// </summary>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: void writeFields() throws IOException
			internal virtual void WriteFields()
			{
				outerInstance.Bout.Write(PrimVals, 0, PrimVals.Length, false);

				ObjectStreamField[] fields = Desc.GetFields(false);
				int numPrimFields = fields.Length - ObjVals.Length;
				for (int i = 0; i < ObjVals.Length; i++)
				{
					if (ExtendedDebugInfo)
					{
						outerInstance.DebugInfoStack.Push("field (class \"" + Desc.Name + "\", name: \"" + fields[numPrimFields + i].Name + "\", type: \"" + fields[numPrimFields + i].Type + "\")");
					}
					try
					{
						outerInstance.WriteObject0(ObjVals[i], fields[numPrimFields + i].Unshared);
					}
					finally
					{
						if (ExtendedDebugInfo)
						{
							outerInstance.DebugInfoStack.Pop();
						}
					}
				}
			}

			/// <summary>
			/// Returns offset of field with given name and type.  A specified type
			/// of null matches all types, Object.class matches all non-primitive
			/// types, and any other non-null type matches assignable types only.
			/// Throws IllegalArgumentException if no matching field found.
			/// </summary>
			internal virtual int GetFieldOffset(String name, Class type)
			{
				ObjectStreamField field = Desc.GetField(name, type);
				if (field == null)
				{
					throw new IllegalArgumentException("no such field " + name + " with type " + type);
				}
				return field.Offset;
			}
		}

		/// <summary>
		/// Buffered output stream with two modes: in default mode, outputs data in
		/// same format as DataOutputStream; in "block data" mode, outputs data
		/// bracketed by block data markers (see object serialization specification
		/// for details).
		/// </summary>
		private class BlockDataOutputStream : OutputStream, DataOutput
		{
			/// <summary>
			/// maximum data block length </summary>
			internal const int MAX_BLOCK_SIZE = 1024;
			/// <summary>
			/// maximum data block header length </summary>
			internal const int MAX_HEADER_SIZE = 5;
			/// <summary>
			/// (tunable) length of char buffer (for writing strings) </summary>
			internal const int CHAR_BUF_SIZE = 256;

			/// <summary>
			/// buffer for writing general/block data </summary>
			internal readonly sbyte[] Buf = new sbyte[MAX_BLOCK_SIZE];
			/// <summary>
			/// buffer for writing block data headers </summary>
			internal readonly sbyte[] Hbuf = new sbyte[MAX_HEADER_SIZE];
			/// <summary>
			/// char buffer for fast string writes </summary>
			internal readonly char[] Cbuf = new char[CHAR_BUF_SIZE];

			/// <summary>
			/// block data mode </summary>
			internal bool Blkmode = false;
			/// <summary>
			/// current offset into buf </summary>
			internal int Pos = 0;

			/// <summary>
			/// underlying output stream </summary>
			internal readonly OutputStream @out;
			/// <summary>
			/// loopback stream (for data writes that span data blocks) </summary>
			internal readonly DataOutputStream Dout;

			/// <summary>
			/// Creates new BlockDataOutputStream on top of given underlying stream.
			/// Block data mode is turned off by default.
			/// </summary>
			internal BlockDataOutputStream(OutputStream @out)
			{
				this.@out = @out;
				Dout = new DataOutputStream(this);
			}

			/// <summary>
			/// Sets block data mode to the given mode (true == on, false == off)
			/// and returns the previous mode value.  If the new mode is the same as
			/// the old mode, no action is taken.  If the new mode differs from the
			/// old mode, any buffered data is flushed before switching to the new
			/// mode.
			/// </summary>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: boolean setBlockDataMode(boolean mode) throws IOException
			internal virtual bool SetBlockDataMode(bool mode)
			{
				if (Blkmode == mode)
				{
					return Blkmode;
				}
				Drain();
				Blkmode = mode;
				return !Blkmode;
			}

			/// <summary>
			/// Returns true if the stream is currently in block data mode, false
			/// otherwise.
			/// </summary>
			internal virtual bool BlockDataMode
			{
				get
				{
					return Blkmode;
				}
			}

			/* ----------------- generic output stream methods ----------------- */
			/*
			 * The following methods are equivalent to their counterparts in
			 * OutputStream, except that they partition written data into data
			 * blocks when in block data mode.
			 */

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public void write(int b) throws IOException
			public override void Write(int b)
			{
				if (Pos >= MAX_BLOCK_SIZE)
				{
					Drain();
				}
				Buf[Pos++] = (sbyte) b;
			}

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public void write(byte[] b) throws IOException
			public override void Write(sbyte[] b)
			{
				Write(b, 0, b.Length, false);
			}

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public void write(byte[] b, int off, int len) throws IOException
			public override void Write(sbyte[] b, int off, int len)
			{
				Write(b, off, len, false);
			}

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public void flush() throws IOException
			public override void Flush()
			{
				Drain();
				@out.Flush();
			}

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public void close() throws IOException
			public override void Close()
			{
				Flush();
				@out.Close();
			}

			/// <summary>
			/// Writes specified span of byte values from given array.  If copy is
			/// true, copies the values to an intermediate buffer before writing
			/// them to underlying stream (to avoid exposing a reference to the
			/// original byte array).
			/// </summary>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: void write(byte[] b, int off, int len, boolean copy) throws IOException
			internal virtual void Write(sbyte[] b, int off, int len, bool copy)
			{
				if (!(copy || Blkmode)) // write directly
				{
					Drain();
					@out.Write(b, off, len);
					return;
				}

				while (len > 0)
				{
					if (Pos >= MAX_BLOCK_SIZE)
					{
						Drain();
					}
					if (len >= MAX_BLOCK_SIZE && !copy && Pos == 0)
					{
						// avoid unnecessary copy
						WriteBlockHeader(MAX_BLOCK_SIZE);
						@out.Write(b, off, MAX_BLOCK_SIZE);
						off += MAX_BLOCK_SIZE;
						len -= MAX_BLOCK_SIZE;
					}
					else
					{
						int wlen = System.Math.Min(len, MAX_BLOCK_SIZE - Pos);
						System.Array.Copy(b, off, Buf, Pos, wlen);
						Pos += wlen;
						off += wlen;
						len -= wlen;
					}
				}
			}

			/// <summary>
			/// Writes all buffered data from this stream to the underlying stream,
			/// but does not flush underlying stream.
			/// </summary>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: void drain() throws IOException
			internal virtual void Drain()
			{
				if (Pos == 0)
				{
					return;
				}
				if (Blkmode)
				{
					WriteBlockHeader(Pos);
				}
				@out.Write(Buf, 0, Pos);
				Pos = 0;
			}

			/// <summary>
			/// Writes block data header.  Data blocks shorter than 256 bytes are
			/// prefixed with a 2-byte header; all others start with a 5-byte
			/// header.
			/// </summary>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: private void writeBlockHeader(int len) throws IOException
			internal virtual void WriteBlockHeader(int len)
			{
				if (len <= 0xFF)
				{
					Hbuf[0] = ObjectStreamConstants_Fields.TC_BLOCKDATA;
					Hbuf[1] = (sbyte) len;
					@out.Write(Hbuf, 0, 2);
				}
				else
				{
					Hbuf[0] = ObjectStreamConstants_Fields.TC_BLOCKDATALONG;
					Bits.PutInt(Hbuf, 1, len);
					@out.Write(Hbuf, 0, 5);
				}
			}


			/* ----------------- primitive data output methods ----------------- */
			/*
			 * The following methods are equivalent to their counterparts in
			 * DataOutputStream, except that they partition written data into data
			 * blocks when in block data mode.
			 */

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public void writeBoolean(boolean v) throws IOException
			public virtual void WriteBoolean(bool v)
			{
				if (Pos >= MAX_BLOCK_SIZE)
				{
					Drain();
				}
				Bits.PutBoolean(Buf, Pos++, v);
			}

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public void writeByte(int v) throws IOException
			public virtual void WriteByte(int v)
			{
				if (Pos >= MAX_BLOCK_SIZE)
				{
					Drain();
				}
				Buf[Pos++] = (sbyte) v;
			}

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public void writeChar(int v) throws IOException
			public virtual void WriteChar(int v)
			{
				if (Pos + 2 <= MAX_BLOCK_SIZE)
				{
					Bits.PutChar(Buf, Pos, (char) v);
					Pos += 2;
				}
				else
				{
					Dout.WriteChar(v);
				}
			}

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public void writeShort(int v) throws IOException
			public virtual void WriteShort(int v)
			{
				if (Pos + 2 <= MAX_BLOCK_SIZE)
				{
					Bits.PutShort(Buf, Pos, (short) v);
					Pos += 2;
				}
				else
				{
					Dout.WriteShort(v);
				}
			}

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public void writeInt(int v) throws IOException
			public virtual void WriteInt(int v)
			{
				if (Pos + 4 <= MAX_BLOCK_SIZE)
				{
					Bits.PutInt(Buf, Pos, v);
					Pos += 4;
				}
				else
				{
					Dout.WriteInt(v);
				}
			}

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public void writeFloat(float v) throws IOException
			public virtual void WriteFloat(float v)
			{
				if (Pos + 4 <= MAX_BLOCK_SIZE)
				{
					Bits.PutFloat(Buf, Pos, v);
					Pos += 4;
				}
				else
				{
					Dout.WriteFloat(v);
				}
			}

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public void writeLong(long v) throws IOException
			public virtual void WriteLong(long v)
			{
				if (Pos + 8 <= MAX_BLOCK_SIZE)
				{
					Bits.PutLong(Buf, Pos, v);
					Pos += 8;
				}
				else
				{
					Dout.WriteLong(v);
				}
			}

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public void writeDouble(double v) throws IOException
			public virtual void WriteDouble(double v)
			{
				if (Pos + 8 <= MAX_BLOCK_SIZE)
				{
					Bits.PutDouble(Buf, Pos, v);
					Pos += 8;
				}
				else
				{
					Dout.WriteDouble(v);
				}
			}

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public void writeBytes(String s) throws IOException
			public virtual void WriteBytes(String s)
			{
				int endoff = s.Length();
				int cpos = 0;
				int csize = 0;
				for (int off = 0; off < endoff;)
				{
					if (cpos >= csize)
					{
						cpos = 0;
						csize = System.Math.Min(endoff - off, CHAR_BUF_SIZE);
						s.GetChars(off, off + csize, Cbuf, 0);
					}
					if (Pos >= MAX_BLOCK_SIZE)
					{
						Drain();
					}
					int n = System.Math.Min(csize - cpos, MAX_BLOCK_SIZE - Pos);
					int stop = Pos + n;
					while (Pos < stop)
					{
						Buf[Pos++] = (sbyte) Cbuf[cpos++];
					}
					off += n;
				}
			}

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public void writeChars(String s) throws IOException
			public virtual void WriteChars(String s)
			{
				int endoff = s.Length();
				for (int off = 0; off < endoff;)
				{
					int csize = System.Math.Min(endoff - off, CHAR_BUF_SIZE);
					s.GetChars(off, off + csize, Cbuf, 0);
					WriteChars(Cbuf, 0, csize);
					off += csize;
				}
			}

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public void writeUTF(String s) throws IOException
			public virtual void WriteUTF(String s)
			{
				WriteUTF(s, GetUTFLength(s));
			}


			/* -------------- primitive data array output methods -------------- */
			/*
			 * The following methods write out spans of primitive data values.
			 * Though equivalent to calling the corresponding primitive write
			 * methods repeatedly, these methods are optimized for writing groups
			 * of primitive data values more efficiently.
			 */

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: void writeBooleans(boolean[] v, int off, int len) throws IOException
			internal virtual void WriteBooleans(bool[] v, int off, int len)
			{
				int endoff = off + len;
				while (off < endoff)
				{
					if (Pos >= MAX_BLOCK_SIZE)
					{
						Drain();
					}
					int stop = System.Math.Min(endoff, off + (MAX_BLOCK_SIZE - Pos));
					while (off < stop)
					{
						Bits.PutBoolean(Buf, Pos++, v[off++]);
					}
				}
			}

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: void writeChars(char[] v, int off, int len) throws IOException
			internal virtual void WriteChars(char[] v, int off, int len)
			{
				int limit = MAX_BLOCK_SIZE - 2;
				int endoff = off + len;
				while (off < endoff)
				{
					if (Pos <= limit)
					{
						int avail = (MAX_BLOCK_SIZE - Pos) >> 1;
						int stop = System.Math.Min(endoff, off + avail);
						while (off < stop)
						{
							Bits.PutChar(Buf, Pos, v[off++]);
							Pos += 2;
						}
					}
					else
					{
						Dout.WriteChar(v[off++]);
					}
				}
			}

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: void writeShorts(short[] v, int off, int len) throws IOException
			internal virtual void WriteShorts(short[] v, int off, int len)
			{
				int limit = MAX_BLOCK_SIZE - 2;
				int endoff = off + len;
				while (off < endoff)
				{
					if (Pos <= limit)
					{
						int avail = (MAX_BLOCK_SIZE - Pos) >> 1;
						int stop = System.Math.Min(endoff, off + avail);
						while (off < stop)
						{
							Bits.PutShort(Buf, Pos, v[off++]);
							Pos += 2;
						}
					}
					else
					{
						Dout.WriteShort(v[off++]);
					}
				}
			}

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: void writeInts(int[] v, int off, int len) throws IOException
			internal virtual void WriteInts(int[] v, int off, int len)
			{
				int limit = MAX_BLOCK_SIZE - 4;
				int endoff = off + len;
				while (off < endoff)
				{
					if (Pos <= limit)
					{
						int avail = (MAX_BLOCK_SIZE - Pos) >> 2;
						int stop = System.Math.Min(endoff, off + avail);
						while (off < stop)
						{
							Bits.PutInt(Buf, Pos, v[off++]);
							Pos += 4;
						}
					}
					else
					{
						Dout.WriteInt(v[off++]);
					}
				}
			}

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: void writeFloats(float[] v, int off, int len) throws IOException
			internal virtual void WriteFloats(float[] v, int off, int len)
			{
				int limit = MAX_BLOCK_SIZE - 4;
				int endoff = off + len;
				while (off < endoff)
				{
					if (Pos <= limit)
					{
						int avail = (MAX_BLOCK_SIZE - Pos) >> 2;
						int chunklen = System.Math.Min(endoff - off, avail);
						floatsToBytes(v, off, Buf, Pos, chunklen);
						off += chunklen;
						Pos += chunklen << 2;
					}
					else
					{
						Dout.WriteFloat(v[off++]);
					}
				}
			}

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: void writeLongs(long[] v, int off, int len) throws IOException
			internal virtual void WriteLongs(long[] v, int off, int len)
			{
				int limit = MAX_BLOCK_SIZE - 8;
				int endoff = off + len;
				while (off < endoff)
				{
					if (Pos <= limit)
					{
						int avail = (MAX_BLOCK_SIZE - Pos) >> 3;
						int stop = System.Math.Min(endoff, off + avail);
						while (off < stop)
						{
							Bits.PutLong(Buf, Pos, v[off++]);
							Pos += 8;
						}
					}
					else
					{
						Dout.WriteLong(v[off++]);
					}
				}
			}

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: void writeDoubles(double[] v, int off, int len) throws IOException
			internal virtual void WriteDoubles(double[] v, int off, int len)
			{
				int limit = MAX_BLOCK_SIZE - 8;
				int endoff = off + len;
				while (off < endoff)
				{
					if (Pos <= limit)
					{
						int avail = (MAX_BLOCK_SIZE - Pos) >> 3;
						int chunklen = System.Math.Min(endoff - off, avail);
						doublesToBytes(v, off, Buf, Pos, chunklen);
						off += chunklen;
						Pos += chunklen << 3;
					}
					else
					{
						Dout.WriteDouble(v[off++]);
					}
				}
			}

			/// <summary>
			/// Returns the length in bytes of the UTF encoding of the given string.
			/// </summary>
			internal virtual long GetUTFLength(String s)
			{
				int len = s.Length();
				long utflen = 0;
				for (int off = 0; off < len;)
				{
					int csize = System.Math.Min(len - off, CHAR_BUF_SIZE);
					s.GetChars(off, off + csize, Cbuf, 0);
					for (int cpos = 0; cpos < csize; cpos++)
					{
						char c = Cbuf[cpos];
						if (c >= 0x0001 && c <= 0x007F)
						{
							utflen++;
						}
						else if (c > 0x07FF)
						{
							utflen += 3;
						}
						else
						{
							utflen += 2;
						}
					}
					off += csize;
				}
				return utflen;
			}

			/// <summary>
			/// Writes the given string in UTF format.  This method is used in
			/// situations where the UTF encoding length of the string is already
			/// known; specifying it explicitly avoids a prescan of the string to
			/// determine its UTF length.
			/// </summary>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: void writeUTF(String s, long utflen) throws IOException
			internal virtual void WriteUTF(String s, long utflen)
			{
				if (utflen > 0xFFFFL)
				{
					throw new UTFDataFormatException();
				}
				WriteShort((int) utflen);
				if (utflen == (long) s.Length())
				{
					WriteBytes(s);
				}
				else
				{
					WriteUTFBody(s);
				}
			}

			/// <summary>
			/// Writes given string in "long" UTF format.  "Long" UTF format is
			/// identical to standard UTF, except that it uses an 8 byte header
			/// (instead of the standard 2 bytes) to convey the UTF encoding length.
			/// </summary>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: void writeLongUTF(String s) throws IOException
			internal virtual void WriteLongUTF(String s)
			{
				WriteLongUTF(s, GetUTFLength(s));
			}

			/// <summary>
			/// Writes given string in "long" UTF format, where the UTF encoding
			/// length of the string is already known.
			/// </summary>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: void writeLongUTF(String s, long utflen) throws IOException
			internal virtual void WriteLongUTF(String s, long utflen)
			{
				WriteLong(utflen);
				if (utflen == (long) s.Length())
				{
					WriteBytes(s);
				}
				else
				{
					WriteUTFBody(s);
				}
			}

			/// <summary>
			/// Writes the "body" (i.e., the UTF representation minus the 2-byte or
			/// 8-byte length header) of the UTF encoding for the given string.
			/// </summary>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: private void writeUTFBody(String s) throws IOException
			internal virtual void WriteUTFBody(String s)
			{
				int limit = MAX_BLOCK_SIZE - 3;
				int len = s.Length();
				for (int off = 0; off < len;)
				{
					int csize = System.Math.Min(len - off, CHAR_BUF_SIZE);
					s.GetChars(off, off + csize, Cbuf, 0);
					for (int cpos = 0; cpos < csize; cpos++)
					{
						char c = Cbuf[cpos];
						if (Pos <= limit)
						{
							if (c <= 0x007F && c != 0)
							{
								Buf[Pos++] = (sbyte) c;
							}
							else if (c > 0x07FF)
							{
								Buf[Pos + 2] = unchecked((sbyte)(0x80 | ((c >> 0) & 0x3F)));
								Buf[Pos + 1] = unchecked((sbyte)(0x80 | ((c >> 6) & 0x3F)));
								Buf[Pos + 0] = unchecked((sbyte)(0xE0 | ((c >> 12) & 0x0F)));
								Pos += 3;
							}
							else
							{
								Buf[Pos + 1] = unchecked((sbyte)(0x80 | ((c >> 0) & 0x3F)));
								Buf[Pos + 0] = unchecked((sbyte)(0xC0 | ((c >> 6) & 0x1F)));
								Pos += 2;
							}
						} // write one byte at a time to normalize block
						else
						{
							if (c <= 0x007F && c != 0)
							{
								Write(c);
							}
							else if (c > 0x07FF)
							{
								Write(0xE0 | ((c >> 12) & 0x0F));
								Write(0x80 | ((c >> 6) & 0x3F));
								Write(0x80 | ((c >> 0) & 0x3F));
							}
							else
							{
								Write(0xC0 | ((c >> 6) & 0x1F));
								Write(0x80 | ((c >> 0) & 0x3F));
							}
						}
					}
					off += csize;
				}
			}
		}

		/// <summary>
		/// Lightweight identity hash table which maps objects to integer handles,
		/// assigned in ascending order.
		/// </summary>
		private class HandleTable
		{

			/* number of mappings in table/next available handle */
			internal int Size_Renamed;
			/* size threshold determining when to expand hash spine */
			internal int Threshold;
			/* factor for computing size threshold */
			internal readonly float LoadFactor;
			/* maps hash value -> candidate handle value */
			internal int[] Spine;
			/* maps handle value -> next candidate handle value */
			internal int[] Next;
			/* maps handle value -> associated object */
			internal Object[] Objs;

			/// <summary>
			/// Creates new HandleTable with given capacity and load factor.
			/// </summary>
			internal HandleTable(int initialCapacity, float loadFactor)
			{
				this.LoadFactor = loadFactor;
				Spine = new int[initialCapacity];
				Next = new int[initialCapacity];
				Objs = new Object[initialCapacity];
				Threshold = (int)(initialCapacity * loadFactor);
				Clear();
			}

			/// <summary>
			/// Assigns next available handle to given object, and returns handle
			/// value.  Handles are assigned in ascending order starting at 0.
			/// </summary>
			internal virtual int Assign(Object obj)
			{
				if (Size_Renamed >= Next.Length)
				{
					GrowEntries();
				}
				if (Size_Renamed >= Threshold)
				{
					GrowSpine();
				}
				Insert(obj, Size_Renamed);
				return Size_Renamed++;
			}

			/// <summary>
			/// Looks up and returns handle associated with given object, or -1 if
			/// no mapping found.
			/// </summary>
			internal virtual int Lookup(Object obj)
			{
				if (Size_Renamed == 0)
				{
					return -1;
				}
				int index = Hash(obj) % Spine.Length;
				for (int i = Spine[index]; i >= 0; i = Next[i])
				{
					if (Objs[i] == obj)
					{
						return i;
					}
				}
				return -1;
			}

			/// <summary>
			/// Resets table to its initial (empty) state.
			/// </summary>
			internal virtual void Clear()
			{
				Arrays.Fill(Spine, -1);
				Arrays.Fill(Objs, 0, Size_Renamed, null);
				Size_Renamed = 0;
			}

			/// <summary>
			/// Returns the number of mappings currently in table.
			/// </summary>
			internal virtual int Size()
			{
				return Size_Renamed;
			}

			/// <summary>
			/// Inserts mapping object -> handle mapping into table.  Assumes table
			/// is large enough to accommodate new mapping.
			/// </summary>
			internal virtual void Insert(Object obj, int handle)
			{
				int index = Hash(obj) % Spine.Length;
				Objs[handle] = obj;
				Next[handle] = Spine[index];
				Spine[index] = handle;
			}

			/// <summary>
			/// Expands the hash "spine" -- equivalent to increasing the number of
			/// buckets in a conventional hash table.
			/// </summary>
			internal virtual void GrowSpine()
			{
				Spine = new int[(Spine.Length << 1) + 1];
				Threshold = (int)(Spine.Length * LoadFactor);
				Arrays.Fill(Spine, -1);
				for (int i = 0; i < Size_Renamed; i++)
				{
					Insert(Objs[i], i);
				}
			}

			/// <summary>
			/// Increases hash table capacity by lengthening entry arrays.
			/// </summary>
			internal virtual void GrowEntries()
			{
				int newLength = (Next.Length << 1) + 1;
				int[] newNext = new int[newLength];
				System.Array.Copy(Next, 0, newNext, 0, Size_Renamed);
				Next = newNext;

				Object[] newObjs = new Object[newLength];
				System.Array.Copy(Objs, 0, newObjs, 0, Size_Renamed);
				Objs = newObjs;
			}

			/// <summary>
			/// Returns hash value for given object.
			/// </summary>
			internal virtual int Hash(Object obj)
			{
				return System.identityHashCode(obj) & 0x7FFFFFFF;
			}
		}

		/// <summary>
		/// Lightweight identity hash table which maps objects to replacement
		/// objects.
		/// </summary>
		private class ReplaceTable
		{

			/* maps object -> index */
			internal readonly HandleTable Htab;
			/* maps index -> replacement object */
			internal Object[] Reps;

			/// <summary>
			/// Creates new ReplaceTable with given capacity and load factor.
			/// </summary>
			internal ReplaceTable(int initialCapacity, float loadFactor)
			{
				Htab = new HandleTable(initialCapacity, loadFactor);
				Reps = new Object[initialCapacity];
			}

			/// <summary>
			/// Enters mapping from object to replacement object.
			/// </summary>
			internal virtual void Assign(Object obj, Object rep)
			{
				int index = Htab.Assign(obj);
				while (index >= Reps.Length)
				{
					Grow();
				}
				Reps[index] = rep;
			}

			/// <summary>
			/// Looks up and returns replacement for given object.  If no
			/// replacement is found, returns the lookup object itself.
			/// </summary>
			internal virtual Object Lookup(Object obj)
			{
				int index = Htab.Lookup(obj);
				return (index >= 0) ? Reps[index] : obj;
			}

			/// <summary>
			/// Resets table to its initial (empty) state.
			/// </summary>
			internal virtual void Clear()
			{
				Arrays.Fill(Reps, 0, Htab.Size(), null);
				Htab.Clear();
			}

			/// <summary>
			/// Returns the number of mappings currently in table.
			/// </summary>
			internal virtual int Size()
			{
				return Htab.Size();
			}

			/// <summary>
			/// Increases table capacity.
			/// </summary>
			internal virtual void Grow()
			{
				Object[] newReps = new Object[(Reps.Length << 1) + 1];
				System.Array.Copy(Reps, 0, newReps, 0, Reps.Length);
				Reps = newReps;
			}
		}

		/// <summary>
		/// Stack to keep debug information about the state of the
		/// serialization process, for embedding in exception messages.
		/// </summary>
		private class DebugTraceInfoStack
		{
			internal readonly IList<String> Stack;

			internal DebugTraceInfoStack()
			{
				Stack = new List<>();
			}

			/// <summary>
			/// Removes all of the elements from enclosed list.
			/// </summary>
			internal virtual void Clear()
			{
				Stack.Clear();
			}

			/// <summary>
			/// Removes the object at the top of enclosed list.
			/// </summary>
			internal virtual void Pop()
			{
				Stack.Remove(Stack.Count - 1);
			}

			/// <summary>
			/// Pushes a String onto the top of enclosed list.
			/// </summary>
			internal virtual void Push(String entry)
			{
				Stack.Add("\t- " + entry);
			}

			/// <summary>
			/// Returns a string representation of this object
			/// </summary>
			public override String ToString()
			{
				StringBuilder buffer = new StringBuilder();
				if (Stack.Count > 0)
				{
					for (int i = Stack.Count; i > 0; i--)
					{
						buffer.Append(Stack[i - 1] + ((i != 1) ? "\n" : ""));
					}
				}
				return buffer.ToString();
			}
		}

	}

}
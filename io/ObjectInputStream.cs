using System;
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
	/// An ObjectInputStream deserializes primitive data and objects previously
	/// written using an ObjectOutputStream.
	/// 
	/// <para>ObjectOutputStream and ObjectInputStream can provide an application with
	/// persistent storage for graphs of objects when used with a FileOutputStream
	/// and FileInputStream respectively.  ObjectInputStream is used to recover
	/// those objects previously serialized. Other uses include passing objects
	/// between hosts using a socket stream or for marshaling and unmarshaling
	/// arguments and parameters in a remote communication system.
	/// 
	/// </para>
	/// <para>ObjectInputStream ensures that the types of all objects in the graph
	/// created from the stream match the classes present in the Java Virtual
	/// Machine.  Classes are loaded as required using the standard mechanisms.
	/// 
	/// </para>
	/// <para>Only objects that support the java.io.Serializable or
	/// java.io.Externalizable interface can be read from streams.
	/// 
	/// </para>
	/// <para>The method <code>readObject</code> is used to read an object from the
	/// stream.  Java's safe casting should be used to get the desired type.  In
	/// Java, strings and arrays are objects and are treated as objects during
	/// serialization. When read they need to be cast to the expected type.
	/// 
	/// </para>
	/// <para>Primitive data types can be read from the stream using the appropriate
	/// method on DataInput.
	/// 
	/// </para>
	/// <para>The default deserialization mechanism for objects restores the contents
	/// of each field to the value and type it had when it was written.  Fields
	/// declared as transient or static are ignored by the deserialization process.
	/// References to other objects cause those objects to be read from the stream
	/// as necessary.  Graphs of objects are restored correctly using a reference
	/// sharing mechanism.  New objects are always allocated when deserializing,
	/// which prevents existing objects from being overwritten.
	/// 
	/// </para>
	/// <para>Reading an object is analogous to running the constructors of a new
	/// object.  Memory is allocated for the object and initialized to zero (NULL).
	/// No-arg constructors are invoked for the non-serializable classes and then
	/// the fields of the serializable classes are restored from the stream starting
	/// with the serializable class closest to java.lang.object and finishing with
	/// the object's most specific class.
	/// 
	/// </para>
	/// <para>For example to read from a stream as written by the example in
	/// ObjectOutputStream:
	/// <br>
	/// <pre>
	///      FileInputStream fis = new FileInputStream("t.tmp");
	///      ObjectInputStream ois = new ObjectInputStream(fis);
	/// 
	///      int i = ois.readInt();
	///      String today = (String) ois.readObject();
	///      Date date = (Date) ois.readObject();
	/// 
	///      ois.close();
	/// </pre>
	/// 
	/// </para>
	/// <para>Classes control how they are serialized by implementing either the
	/// java.io.Serializable or java.io.Externalizable interfaces.
	/// 
	/// </para>
	/// <para>Implementing the Serializable interface allows object serialization to
	/// save and restore the entire state of the object and it allows classes to
	/// evolve between the time the stream is written and the time it is read.  It
	/// automatically traverses references between objects, saving and restoring
	/// entire graphs.
	/// 
	/// </para>
	/// <para>Serializable classes that require special handling during the
	/// serialization and deserialization process should implement the following
	/// methods:
	/// 
	/// <pre>
	/// private void writeObject(java.io.ObjectOutputStream stream)
	///     throws IOException;
	/// private void readObject(java.io.ObjectInputStream stream)
	///     throws IOException, ClassNotFoundException;
	/// private void readObjectNoData()
	///     throws ObjectStreamException;
	/// </pre>
	/// 
	/// </para>
	/// <para>The readObject method is responsible for reading and restoring the state
	/// of the object for its particular class using data written to the stream by
	/// the corresponding writeObject method.  The method does not need to concern
	/// itself with the state belonging to its superclasses or subclasses.  State is
	/// restored by reading data from the ObjectInputStream for the individual
	/// fields and making assignments to the appropriate fields of the object.
	/// Reading primitive data types is supported by DataInput.
	/// 
	/// </para>
	/// <para>Any attempt to read object data which exceeds the boundaries of the
	/// custom data written by the corresponding writeObject method will cause an
	/// OptionalDataException to be thrown with an eof field value of true.
	/// Non-object reads which exceed the end of the allotted data will reflect the
	/// end of data in the same way that they would indicate the end of the stream:
	/// bytewise reads will return -1 as the byte read or number of bytes read, and
	/// primitive reads will throw EOFExceptions.  If there is no corresponding
	/// writeObject method, then the end of default serialized data marks the end of
	/// the allotted data.
	/// 
	/// </para>
	/// <para>Primitive and object read calls issued from within a readExternal method
	/// behave in the same manner--if the stream is already positioned at the end of
	/// data written by the corresponding writeExternal method, object reads will
	/// throw OptionalDataExceptions with eof set to true, bytewise reads will
	/// return -1, and primitive reads will throw EOFExceptions.  Note that this
	/// behavior does not hold for streams written with the old
	/// <code>ObjectStreamConstants.PROTOCOL_VERSION_1</code> protocol, in which the
	/// end of data written by writeExternal methods is not demarcated, and hence
	/// cannot be detected.
	/// 
	/// </para>
	/// <para>The readObjectNoData method is responsible for initializing the state of
	/// the object for its particular class in the event that the serialization
	/// stream does not list the given class as a superclass of the object being
	/// deserialized.  This may occur in cases where the receiving party uses a
	/// different version of the deserialized instance's class than the sending
	/// party, and the receiver's version extends classes that are not extended by
	/// the sender's version.  This may also occur if the serialization stream has
	/// been tampered; hence, readObjectNoData is useful for initializing
	/// deserialized objects properly despite a "hostile" or incomplete source
	/// stream.
	/// 
	/// </para>
	/// <para>Serialization does not read or assign values to the fields of any object
	/// that does not implement the java.io.Serializable interface.  Subclasses of
	/// Objects that are not serializable can be serializable. In this case the
	/// non-serializable class must have a no-arg constructor to allow its fields to
	/// be initialized.  In this case it is the responsibility of the subclass to
	/// save and restore the state of the non-serializable class. It is frequently
	/// the case that the fields of that class are accessible (public, package, or
	/// protected) or that there are get and set methods that can be used to restore
	/// the state.
	/// 
	/// </para>
	/// <para>Any exception that occurs while deserializing an object will be caught by
	/// the ObjectInputStream and abort the reading process.
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
	/// <para>Enum constants are deserialized differently than ordinary serializable or
	/// externalizable objects.  The serialized form of an enum constant consists
	/// solely of its name; field values of the constant are not transmitted.  To
	/// deserialize an enum constant, ObjectInputStream reads the constant name from
	/// the stream; the deserialized constant is then obtained by calling the static
	/// method <code>Enum.valueOf(Class, String)</code> with the enum constant's
	/// base type and the received constant name as arguments.  Like other
	/// serializable or externalizable objects, enum constants can function as the
	/// targets of back references appearing subsequently in the serialization
	/// stream.  The process by which enum constants are deserialized cannot be
	/// customized: any class-specific readObject, readObjectNoData, and readResolve
	/// methods defined by enum types are ignored during deserialization.
	/// Similarly, any serialPersistentFields or serialVersionUID field declarations
	/// are also ignored--all enum types have a fixed serialVersionUID of 0L.
	/// 
	/// @author      Mike Warres
	/// @author      Roger Riggs
	/// </para>
	/// </summary>
	/// <seealso cref= java.io.DataInput </seealso>
	/// <seealso cref= java.io.ObjectOutputStream </seealso>
	/// <seealso cref= java.io.Serializable </seealso>
	/// <seealso cref= <a href="../../../platform/serialization/spec/input.html"> Object Serialization Specification, Section 3, Object Input Classes</a>
	/// @since   JDK1.1 </seealso>
	public class ObjectInputStream : InputStream, ObjectInput, ObjectStreamConstants
	{
		/// <summary>
		/// handle value representing null </summary>
		private const int NULL_HANDLE = -1;

		/// <summary>
		/// marker for unshared objects in internal handle table </summary>
		private static readonly Object UnsharedMarker = new Object();

		/// <summary>
		/// table mapping primitive type names to corresponding class objects </summary>
		private static readonly Dictionary<String, Class> PrimClasses = new Dictionary<String, Class>(8, 1.0F);
		static ObjectInputStream()
		{
			PrimClasses["boolean"] = typeof(bool);
			PrimClasses["byte"] = typeof(sbyte);
			PrimClasses["char"] = typeof(char);
			PrimClasses["short"] = typeof(short);
			PrimClasses["int"] = typeof(int);
			PrimClasses["long"] = typeof(long);
			PrimClasses["float"] = typeof(float);
			PrimClasses["double"] = typeof(double);
			PrimClasses["void"] = typeof(void);
		}

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
		private readonly BlockDataInputStream Bin;
		/// <summary>
		/// validation callback list </summary>
		private readonly ValidationList Vlist;
		/// <summary>
		/// recursion depth </summary>
		private int Depth;
		/// <summary>
		/// whether stream is closed </summary>
		private bool Closed;

		/// <summary>
		/// wire handle -> obj/exception map </summary>
		private readonly HandleTable Handles;
		/// <summary>
		/// scratch field for passing handle values up/down call stack </summary>
		private int PassHandle = NULL_HANDLE;
		/// <summary>
		/// flag set when at end of field value block with no TC_ENDBLOCKDATA </summary>
		private bool DefaultDataEnd = false;

		/// <summary>
		/// buffer for reading primitive field values </summary>
		private sbyte[] PrimVals;

		/// <summary>
		/// if true, invoke readObjectOverride() instead of readObject() </summary>
		private readonly bool EnableOverride;
		/// <summary>
		/// if true, invoke resolveObject() </summary>
		private bool EnableResolve;

		/// <summary>
		/// Context during upcalls to class-defined readObject methods; holds
		/// object currently being deserialized and descriptor for current class.
		/// Null when not during readObject upcall.
		/// </summary>
		private SerialCallbackContext CurContext;

		/// <summary>
		/// Creates an ObjectInputStream that reads from the specified InputStream.
		/// A serialization stream header is read from the stream and verified.
		/// This constructor will block until the corresponding ObjectOutputStream
		/// has written and flushed the header.
		/// 
		/// <para>If a security manager is installed, this constructor will check for
		/// the "enableSubclassImplementation" SerializablePermission when invoked
		/// directly or indirectly by the constructor of a subclass which overrides
		/// the ObjectInputStream.readFields or ObjectInputStream.readUnshared
		/// methods.
		/// 
		/// </para>
		/// </summary>
		/// <param name="in"> input stream to read from </param>
		/// <exception cref="StreamCorruptedException"> if the stream header is incorrect </exception>
		/// <exception cref="IOException"> if an I/O error occurs while reading stream header </exception>
		/// <exception cref="SecurityException"> if untrusted subclass illegally overrides
		///          security-sensitive methods </exception>
		/// <exception cref="NullPointerException"> if <code>in</code> is <code>null</code> </exception>
		/// <seealso cref=     ObjectInputStream#ObjectInputStream() </seealso>
		/// <seealso cref=     ObjectInputStream#readFields() </seealso>
		/// <seealso cref=     ObjectOutputStream#ObjectOutputStream(OutputStream) </seealso>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public ObjectInputStream(InputStream in) throws IOException
		public ObjectInputStream(InputStream @in)
		{
			VerifySubclass();
			Bin = new BlockDataInputStream(this, @in);
			Handles = new HandleTable(10);
			Vlist = new ValidationList();
			EnableOverride = false;
			ReadStreamHeader();
			Bin.BlockDataMode = true;
		}

		/// <summary>
		/// Provide a way for subclasses that are completely reimplementing
		/// ObjectInputStream to not have to allocate private data just used by this
		/// implementation of ObjectInputStream.
		/// 
		/// <para>If there is a security manager installed, this method first calls the
		/// security manager's <code>checkPermission</code> method with the
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
//ORIGINAL LINE: protected ObjectInputStream() throws IOException, SecurityException
		protected internal ObjectInputStream()
		{
			SecurityManager sm = System.SecurityManager;
			if (sm != null)
			{
				sm.CheckPermission(ObjectStreamConstants_Fields.SUBCLASS_IMPLEMENTATION_PERMISSION);
			}
			Bin = null;
			Handles = null;
			Vlist = null;
			EnableOverride = true;
		}

		/// <summary>
		/// Read an object from the ObjectInputStream.  The class of the object, the
		/// signature of the class, and the values of the non-transient and
		/// non-static fields of the class and all of its supertypes are read.
		/// Default deserializing for a class can be overriden using the writeObject
		/// and readObject methods.  Objects referenced by this object are read
		/// transitively so that a complete equivalent graph of objects is
		/// reconstructed by readObject.
		/// 
		/// <para>The root object is completely restored when all of its fields and the
		/// objects it references are completely restored.  At this point the object
		/// validation callbacks are executed in order based on their registered
		/// priorities. The callbacks are registered by objects (in the readObject
		/// special methods) as they are individually restored.
		/// 
		/// </para>
		/// <para>Exceptions are thrown for problems with the InputStream and for
		/// classes that should not be deserialized.  All exceptions are fatal to
		/// the InputStream and leave it in an indeterminate state; it is up to the
		/// caller to ignore or recover the stream state.
		/// 
		/// </para>
		/// </summary>
		/// <exception cref="ClassNotFoundException"> Class of a serialized object cannot be
		///          found. </exception>
		/// <exception cref="InvalidClassException"> Something is wrong with a class used by
		///          serialization. </exception>
		/// <exception cref="StreamCorruptedException"> Control information in the
		///          stream is inconsistent. </exception>
		/// <exception cref="OptionalDataException"> Primitive data was found in the
		///          stream instead of objects. </exception>
		/// <exception cref="IOException"> Any of the usual Input/Output related exceptions. </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public final Object readObject() throws IOException, ClassNotFoundException
		public Object ReadObject()
		{
			if (EnableOverride)
			{
				return ReadObjectOverride();
			}

			// if nested read, passHandle contains handle of enclosing object
			int outerHandle = PassHandle;
			try
			{
				Object obj = ReadObject0(false);
				Handles.MarkDependency(outerHandle, PassHandle);
				ClassNotFoundException ex = Handles.LookupException(PassHandle);
				if (ex != null)
				{
					throw ex;
				}
				if (Depth == 0)
				{
					Vlist.DoCallbacks();
				}
				return obj;
			}
			finally
			{
				PassHandle = outerHandle;
				if (Closed && Depth == 0)
				{
					Clear();
				}
			}
		}

		/// <summary>
		/// This method is called by trusted subclasses of ObjectOutputStream that
		/// constructed ObjectOutputStream using the protected no-arg constructor.
		/// The subclass is expected to provide an override method with the modifier
		/// "final".
		/// </summary>
		/// <returns>  the Object read from the stream. </returns>
		/// <exception cref="ClassNotFoundException"> Class definition of a serialized object
		///          cannot be found. </exception>
		/// <exception cref="OptionalDataException"> Primitive data was found in the stream
		///          instead of objects. </exception>
		/// <exception cref="IOException"> if I/O errors occurred while reading from the
		///          underlying stream </exception>
		/// <seealso cref= #ObjectInputStream() </seealso>
		/// <seealso cref= #readObject()
		/// @since 1.2 </seealso>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: protected Object readObjectOverride() throws IOException, ClassNotFoundException
		protected internal virtual Object ReadObjectOverride()
		{
			return null;
		}

		/// <summary>
		/// Reads an "unshared" object from the ObjectInputStream.  This method is
		/// identical to readObject, except that it prevents subsequent calls to
		/// readObject and readUnshared from returning additional references to the
		/// deserialized instance obtained via this call.  Specifically:
		/// <ul>
		///   <li>If readUnshared is called to deserialize a back-reference (the
		///       stream representation of an object which has been written
		///       previously to the stream), an ObjectStreamException will be
		///       thrown.
		/// 
		///   <li>If readUnshared returns successfully, then any subsequent attempts
		///       to deserialize back-references to the stream handle deserialized
		///       by readUnshared will cause an ObjectStreamException to be thrown.
		/// </ul>
		/// Deserializing an object via readUnshared invalidates the stream handle
		/// associated with the returned object.  Note that this in itself does not
		/// always guarantee that the reference returned by readUnshared is unique;
		/// the deserialized object may define a readResolve method which returns an
		/// object visible to other parties, or readUnshared may return a Class
		/// object or enum constant obtainable elsewhere in the stream or through
		/// external means. If the deserialized object defines a readResolve method
		/// and the invocation of that method returns an array, then readUnshared
		/// returns a shallow clone of that array; this guarantees that the returned
		/// array object is unique and cannot be obtained a second time from an
		/// invocation of readObject or readUnshared on the ObjectInputStream,
		/// even if the underlying data stream has been manipulated.
		/// 
		/// <para>ObjectInputStream subclasses which override this method can only be
		/// constructed in security contexts possessing the
		/// "enableSubclassImplementation" SerializablePermission; any attempt to
		/// instantiate such a subclass without this permission will cause a
		/// SecurityException to be thrown.
		/// 
		/// </para>
		/// </summary>
		/// <returns>  reference to deserialized object </returns>
		/// <exception cref="ClassNotFoundException"> if class of an object to deserialize
		///          cannot be found </exception>
		/// <exception cref="StreamCorruptedException"> if control information in the stream
		///          is inconsistent </exception>
		/// <exception cref="ObjectStreamException"> if object to deserialize has already
		///          appeared in stream </exception>
		/// <exception cref="OptionalDataException"> if primitive data is next in stream </exception>
		/// <exception cref="IOException"> if an I/O error occurs during deserialization
		/// @since   1.4 </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public Object readUnshared() throws IOException, ClassNotFoundException
		public virtual Object ReadUnshared()
		{
			// if nested read, passHandle contains handle of enclosing object
			int outerHandle = PassHandle;
			try
			{
				Object obj = ReadObject0(true);
				Handles.MarkDependency(outerHandle, PassHandle);
				ClassNotFoundException ex = Handles.LookupException(PassHandle);
				if (ex != null)
				{
					throw ex;
				}
				if (Depth == 0)
				{
					Vlist.DoCallbacks();
				}
				return obj;
			}
			finally
			{
				PassHandle = outerHandle;
				if (Closed && Depth == 0)
				{
					Clear();
				}
			}
		}

		/// <summary>
		/// Read the non-static and non-transient fields of the current class from
		/// this stream.  This may only be called from the readObject method of the
		/// class being deserialized. It will throw the NotActiveException if it is
		/// called otherwise.
		/// </summary>
		/// <exception cref="ClassNotFoundException"> if the class of a serialized object
		///          could not be found. </exception>
		/// <exception cref="IOException"> if an I/O error occurs. </exception>
		/// <exception cref="NotActiveException"> if the stream is not currently reading
		///          objects. </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public void defaultReadObject() throws IOException, ClassNotFoundException
		public virtual void DefaultReadObject()
		{
			SerialCallbackContext ctx = CurContext;
			if (ctx == null)
			{
				throw new NotActiveException("not in call to readObject");
			}
			Object curObj = ctx.Obj;
			ObjectStreamClass curDesc = ctx.Desc;
			Bin.BlockDataMode = false;
			DefaultReadFields(curObj, curDesc);
			Bin.BlockDataMode = true;
			if (!curDesc.HasWriteObjectData())
			{
				/*
				 * Fix for 4360508: since stream does not contain terminating
				 * TC_ENDBLOCKDATA tag, set flag so that reading code elsewhere
				 * knows to simulate end-of-custom-data behavior.
				 */
				DefaultDataEnd = true;
			}
			ClassNotFoundException ex = Handles.LookupException(PassHandle);
			if (ex != null)
			{
				throw ex;
			}
		}

		/// <summary>
		/// Reads the persistent fields from the stream and makes them available by
		/// name.
		/// </summary>
		/// <returns>  the <code>GetField</code> object representing the persistent
		///          fields of the object being deserialized </returns>
		/// <exception cref="ClassNotFoundException"> if the class of a serialized object
		///          could not be found. </exception>
		/// <exception cref="IOException"> if an I/O error occurs. </exception>
		/// <exception cref="NotActiveException"> if the stream is not currently reading
		///          objects.
		/// @since 1.2 </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public ObjectInputStream.GetField readFields() throws IOException, ClassNotFoundException
		public virtual ObjectInputStream.GetField ReadFields()
		{
			SerialCallbackContext ctx = CurContext;
			if (ctx == null)
			{
				throw new NotActiveException("not in call to readObject");
			}
			Object curObj = ctx.Obj;
			ObjectStreamClass curDesc = ctx.Desc;
			Bin.BlockDataMode = false;
			GetFieldImpl getField = new GetFieldImpl(this, curDesc);
			getField.ReadFields();
			Bin.BlockDataMode = true;
			if (!curDesc.HasWriteObjectData())
			{
				/*
				 * Fix for 4360508: since stream does not contain terminating
				 * TC_ENDBLOCKDATA tag, set flag so that reading code elsewhere
				 * knows to simulate end-of-custom-data behavior.
				 */
				DefaultDataEnd = true;
			}

			return getField;
		}

		/// <summary>
		/// Register an object to be validated before the graph is returned.  While
		/// similar to resolveObject these validations are called after the entire
		/// graph has been reconstituted.  Typically, a readObject method will
		/// register the object with the stream so that when all of the objects are
		/// restored a final set of validations can be performed.
		/// </summary>
		/// <param name="obj"> the object to receive the validation callback. </param>
		/// <param name="prio"> controls the order of callbacks;zero is a good default.
		///          Use higher numbers to be called back earlier, lower numbers for
		///          later callbacks. Within a priority, callbacks are processed in
		///          no particular order. </param>
		/// <exception cref="NotActiveException"> The stream is not currently reading objects
		///          so it is invalid to register a callback. </exception>
		/// <exception cref="InvalidObjectException"> The validation object is null. </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public void registerValidation(ObjectInputValidation obj, int prio) throws NotActiveException, InvalidObjectException
		public virtual void RegisterValidation(ObjectInputValidation obj, int prio)
		{
			if (Depth == 0)
			{
				throw new NotActiveException("stream inactive");
			}
			Vlist.Register(obj, prio);
		}

		/// <summary>
		/// Load the local class equivalent of the specified stream class
		/// description.  Subclasses may implement this method to allow classes to
		/// be fetched from an alternate source.
		/// 
		/// <para>The corresponding method in <code>ObjectOutputStream</code> is
		/// <code>annotateClass</code>.  This method will be invoked only once for
		/// each unique class in the stream.  This method can be implemented by
		/// subclasses to use an alternate loading mechanism but must return a
		/// <code>Class</code> object. Once returned, if the class is not an array
		/// class, its serialVersionUID is compared to the serialVersionUID of the
		/// serialized class, and if there is a mismatch, the deserialization fails
		/// and an <seealso cref="InvalidClassException"/> is thrown.
		/// 
		/// </para>
		/// <para>The default implementation of this method in
		/// <code>ObjectInputStream</code> returns the result of calling
		/// <pre>
		///     Class.forName(desc.getName(), false, loader)
		/// </pre>
		/// where <code>loader</code> is determined as follows: if there is a
		/// method on the current thread's stack whose declaring class was
		/// defined by a user-defined class loader (and was not a generated to
		/// implement reflective invocations), then <code>loader</code> is class
		/// loader corresponding to the closest such method to the currently
		/// executing frame; otherwise, <code>loader</code> is
		/// <code>null</code>. If this call results in a
		/// <code>ClassNotFoundException</code> and the name of the passed
		/// <code>ObjectStreamClass</code> instance is the Java language keyword
		/// for a primitive type or void, then the <code>Class</code> object
		/// representing that primitive type or void will be returned
		/// (e.g., an <code>ObjectStreamClass</code> with the name
		/// <code>"int"</code> will be resolved to <code>Integer.TYPE</code>).
		/// Otherwise, the <code>ClassNotFoundException</code> will be thrown to
		/// the caller of this method.
		/// 
		/// </para>
		/// </summary>
		/// <param name="desc"> an instance of class <code>ObjectStreamClass</code> </param>
		/// <returns>  a <code>Class</code> object corresponding to <code>desc</code> </returns>
		/// <exception cref="IOException"> any of the usual Input/Output exceptions. </exception>
		/// <exception cref="ClassNotFoundException"> if class of a serialized object cannot
		///          be found. </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: protected Class resolveClass(ObjectStreamClass desc) throws IOException, ClassNotFoundException
		protected internal virtual Class ResolveClass(ObjectStreamClass desc)
		{
			String name = desc.Name;
			try
			{
				return Class.ForName(name, false, LatestUserDefinedLoader());
			}
			catch (ClassNotFoundException ex)
			{
				Class cl = PrimClasses[name];
				if (cl != null)
				{
					return cl;
				}
				else
				{
					throw ex;
				}
			}
		}

		/// <summary>
		/// Returns a proxy class that implements the interfaces named in a proxy
		/// class descriptor; subclasses may implement this method to read custom
		/// data from the stream along with the descriptors for dynamic proxy
		/// classes, allowing them to use an alternate loading mechanism for the
		/// interfaces and the proxy class.
		/// 
		/// <para>This method is called exactly once for each unique proxy class
		/// descriptor in the stream.
		/// 
		/// </para>
		/// <para>The corresponding method in <code>ObjectOutputStream</code> is
		/// <code>annotateProxyClass</code>.  For a given subclass of
		/// <code>ObjectInputStream</code> that overrides this method, the
		/// <code>annotateProxyClass</code> method in the corresponding subclass of
		/// <code>ObjectOutputStream</code> must write any data or objects read by
		/// this method.
		/// 
		/// </para>
		/// <para>The default implementation of this method in
		/// <code>ObjectInputStream</code> returns the result of calling
		/// <code>Proxy.getProxyClass</code> with the list of <code>Class</code>
		/// objects for the interfaces that are named in the <code>interfaces</code>
		/// parameter.  The <code>Class</code> object for each interface name
		/// <code>i</code> is the value returned by calling
		/// <pre>
		///     Class.forName(i, false, loader)
		/// </pre>
		/// where <code>loader</code> is that of the first non-<code>null</code>
		/// class loader up the execution stack, or <code>null</code> if no
		/// non-<code>null</code> class loaders are on the stack (the same class
		/// loader choice used by the <code>resolveClass</code> method).  Unless any
		/// of the resolved interfaces are non-public, this same value of
		/// <code>loader</code> is also the class loader passed to
		/// <code>Proxy.getProxyClass</code>; if non-public interfaces are present,
		/// their class loader is passed instead (if more than one non-public
		/// interface class loader is encountered, an
		/// <code>IllegalAccessError</code> is thrown).
		/// If <code>Proxy.getProxyClass</code> throws an
		/// <code>IllegalArgumentException</code>, <code>resolveProxyClass</code>
		/// will throw a <code>ClassNotFoundException</code> containing the
		/// <code>IllegalArgumentException</code>.
		/// 
		/// </para>
		/// </summary>
		/// <param name="interfaces"> the list of interface names that were
		///                deserialized in the proxy class descriptor </param>
		/// <returns>  a proxy class for the specified interfaces </returns>
		/// <exception cref="IOException"> any exception thrown by the underlying
		///                <code>InputStream</code> </exception>
		/// <exception cref="ClassNotFoundException"> if the proxy class or any of the
		///                named interfaces could not be found </exception>
		/// <seealso cref= ObjectOutputStream#annotateProxyClass(Class)
		/// @since 1.3 </seealso>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: protected Class resolveProxyClass(String[] interfaces) throws IOException, ClassNotFoundException
		protected internal virtual Class ResolveProxyClass(String[] interfaces)
		{
			ClassLoader latestLoader = LatestUserDefinedLoader();
			ClassLoader nonPublicLoader = null;
			bool hasNonPublicInterface = false;

			// define proxy in class loader of non-public interface(s), if any
			Class[] classObjs = new Class[interfaces.Length];
			for (int i = 0; i < interfaces.Length; i++)
			{
				Class cl = Class.ForName(interfaces[i], false, latestLoader);
				if ((cl.Modifiers & Modifier.PUBLIC) == 0)
				{
					if (hasNonPublicInterface)
					{
						if (nonPublicLoader != cl.ClassLoader)
						{
							throw new IllegalAccessError("conflicting non-public interface class loaders");
						}
					}
					else
					{
						nonPublicLoader = cl.ClassLoader;
						hasNonPublicInterface = true;
					}
				}
				classObjs[i] = cl;
			}
			try
			{
				return Proxy.getProxyClass(hasNonPublicInterface ? nonPublicLoader : latestLoader, classObjs);
			}
			catch (IllegalArgumentException e)
			{
				throw new ClassNotFoundException(null, e);
			}
		}

		/// <summary>
		/// This method will allow trusted subclasses of ObjectInputStream to
		/// substitute one object for another during deserialization. Replacing
		/// objects is disabled until enableResolveObject is called. The
		/// enableResolveObject method checks that the stream requesting to resolve
		/// object can be trusted. Every reference to serializable objects is passed
		/// to resolveObject.  To insure that the private state of objects is not
		/// unintentionally exposed only trusted streams may use resolveObject.
		/// 
		/// <para>This method is called after an object has been read but before it is
		/// returned from readObject.  The default resolveObject method just returns
		/// the same object.
		/// 
		/// </para>
		/// <para>When a subclass is replacing objects it must insure that the
		/// substituted object is compatible with every field where the reference
		/// will be stored.  Objects whose type is not a subclass of the type of the
		/// field or array element abort the serialization by raising an exception
		/// and the object is not be stored.
		/// 
		/// </para>
		/// <para>This method is called only once when each object is first
		/// encountered.  All subsequent references to the object will be redirected
		/// to the new object.
		/// 
		/// </para>
		/// </summary>
		/// <param name="obj"> object to be substituted </param>
		/// <returns>  the substituted object </returns>
		/// <exception cref="IOException"> Any of the usual Input/Output exceptions. </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: protected Object resolveObject(Object obj) throws IOException
		protected internal virtual Object ResolveObject(Object obj)
		{
			return obj;
		}

		/// <summary>
		/// Enable the stream to allow objects read from the stream to be replaced.
		/// When enabled, the resolveObject method is called for every object being
		/// deserialized.
		/// 
		/// <para>If <i>enable</i> is true, and there is a security manager installed,
		/// this method first calls the security manager's
		/// <code>checkPermission</code> method with the
		/// <code>SerializablePermission("enableSubstitution")</code> permission to
		/// ensure it's ok to enable the stream to allow objects read from the
		/// stream to be replaced.
		/// 
		/// </para>
		/// </summary>
		/// <param name="enable"> true for enabling use of <code>resolveObject</code> for
		///          every object being deserialized </param>
		/// <returns>  the previous setting before this method was invoked </returns>
		/// <exception cref="SecurityException"> if a security manager exists and its
		///          <code>checkPermission</code> method denies enabling the stream
		///          to allow objects read from the stream to be replaced. </exception>
		/// <seealso cref= SecurityManager#checkPermission </seealso>
		/// <seealso cref= java.io.SerializablePermission </seealso>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: protected boolean enableResolveObject(boolean enable) throws SecurityException
		protected internal virtual bool EnableResolveObject(bool enable)
		{
			if (enable == EnableResolve)
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
			EnableResolve = enable;
			return !EnableResolve;
		}

		/// <summary>
		/// The readStreamHeader method is provided to allow subclasses to read and
		/// verify their own stream headers. It reads and verifies the magic number
		/// and version number.
		/// </summary>
		/// <exception cref="IOException"> if there are I/O errors while reading from the
		///          underlying <code>InputStream</code> </exception>
		/// <exception cref="StreamCorruptedException"> if control information in the stream
		///          is inconsistent </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: protected void readStreamHeader() throws IOException, StreamCorruptedException
		protected internal virtual void ReadStreamHeader()
		{
			short s0 = Bin.ReadShort();
			short s1 = Bin.ReadShort();
			if (s0 != ObjectStreamConstants_Fields.STREAM_MAGIC || s1 != ObjectStreamConstants_Fields.STREAM_VERSION)
			{
				throw new StreamCorruptedException(string.Format("invalid stream header: {0:X4}{1:X4}", s0, s1));
			}
		}

		/// <summary>
		/// Read a class descriptor from the serialization stream.  This method is
		/// called when the ObjectInputStream expects a class descriptor as the next
		/// item in the serialization stream.  Subclasses of ObjectInputStream may
		/// override this method to read in class descriptors that have been written
		/// in non-standard formats (by subclasses of ObjectOutputStream which have
		/// overridden the <code>writeClassDescriptor</code> method).  By default,
		/// this method reads class descriptors according to the format defined in
		/// the Object Serialization specification.
		/// </summary>
		/// <returns>  the class descriptor read </returns>
		/// <exception cref="IOException"> If an I/O error has occurred. </exception>
		/// <exception cref="ClassNotFoundException"> If the Class of a serialized object used
		///          in the class descriptor representation cannot be found </exception>
		/// <seealso cref= java.io.ObjectOutputStream#writeClassDescriptor(java.io.ObjectStreamClass)
		/// @since 1.3 </seealso>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: protected ObjectStreamClass readClassDescriptor() throws IOException, ClassNotFoundException
		protected internal virtual ObjectStreamClass ReadClassDescriptor()
		{
			ObjectStreamClass desc = new ObjectStreamClass();
			desc.ReadNonProxy(this);
			return desc;
		}

		/// <summary>
		/// Reads a byte of data. This method will block if no input is available.
		/// </summary>
		/// <returns>  the byte read, or -1 if the end of the stream is reached. </returns>
		/// <exception cref="IOException"> If an I/O error has occurred. </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public int read() throws IOException
		public override int Read()
		{
			return Bin.Read();
		}

		/// <summary>
		/// Reads into an array of bytes.  This method will block until some input
		/// is available. Consider using java.io.DataInputStream.readFully to read
		/// exactly 'length' bytes.
		/// </summary>
		/// <param name="buf"> the buffer into which the data is read </param>
		/// <param name="off"> the start offset of the data </param>
		/// <param name="len"> the maximum number of bytes read </param>
		/// <returns>  the actual number of bytes read, -1 is returned when the end of
		///          the stream is reached. </returns>
		/// <exception cref="IOException"> If an I/O error has occurred. </exception>
		/// <seealso cref= java.io.DataInputStream#readFully(byte[],int,int) </seealso>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public int read(byte[] buf, int off, int len) throws IOException
		public override int Read(sbyte[] buf, int off, int len)
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
			return Bin.Read(buf, off, len, false);
		}

		/// <summary>
		/// Returns the number of bytes that can be read without blocking.
		/// </summary>
		/// <returns>  the number of available bytes. </returns>
		/// <exception cref="IOException"> if there are I/O errors while reading from the
		///          underlying <code>InputStream</code> </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public int available() throws IOException
		public override int Available()
		{
			return Bin.Available();
		}

		/// <summary>
		/// Closes the input stream. Must be called to release any resources
		/// associated with the stream.
		/// </summary>
		/// <exception cref="IOException"> If an I/O error has occurred. </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public void close() throws IOException
		public override void Close()
		{
			/*
			 * Even if stream already closed, propagate redundant close to
			 * underlying stream to stay consistent with previous implementations.
			 */
			Closed = true;
			if (Depth == 0)
			{
				Clear();
			}
			Bin.Close();
		}

		/// <summary>
		/// Reads in a boolean.
		/// </summary>
		/// <returns>  the boolean read. </returns>
		/// <exception cref="EOFException"> If end of file is reached. </exception>
		/// <exception cref="IOException"> If other I/O error has occurred. </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public boolean readBoolean() throws IOException
		public virtual bool ReadBoolean()
		{
			return Bin.ReadBoolean();
		}

		/// <summary>
		/// Reads an 8 bit byte.
		/// </summary>
		/// <returns>  the 8 bit byte read. </returns>
		/// <exception cref="EOFException"> If end of file is reached. </exception>
		/// <exception cref="IOException"> If other I/O error has occurred. </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public byte readByte() throws IOException
		public virtual sbyte ReadByte()
		{
			return Bin.ReadByte();
		}

		/// <summary>
		/// Reads an unsigned 8 bit byte.
		/// </summary>
		/// <returns>  the 8 bit byte read. </returns>
		/// <exception cref="EOFException"> If end of file is reached. </exception>
		/// <exception cref="IOException"> If other I/O error has occurred. </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public int readUnsignedByte() throws IOException
		public virtual int ReadUnsignedByte()
		{
			return Bin.ReadUnsignedByte();
		}

		/// <summary>
		/// Reads a 16 bit char.
		/// </summary>
		/// <returns>  the 16 bit char read. </returns>
		/// <exception cref="EOFException"> If end of file is reached. </exception>
		/// <exception cref="IOException"> If other I/O error has occurred. </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public char readChar() throws IOException
		public virtual char ReadChar()
		{
			return Bin.ReadChar();
		}

		/// <summary>
		/// Reads a 16 bit short.
		/// </summary>
		/// <returns>  the 16 bit short read. </returns>
		/// <exception cref="EOFException"> If end of file is reached. </exception>
		/// <exception cref="IOException"> If other I/O error has occurred. </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public short readShort() throws IOException
		public virtual short ReadShort()
		{
			return Bin.ReadShort();
		}

		/// <summary>
		/// Reads an unsigned 16 bit short.
		/// </summary>
		/// <returns>  the 16 bit short read. </returns>
		/// <exception cref="EOFException"> If end of file is reached. </exception>
		/// <exception cref="IOException"> If other I/O error has occurred. </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public int readUnsignedShort() throws IOException
		public virtual int ReadUnsignedShort()
		{
			return Bin.ReadUnsignedShort();
		}

		/// <summary>
		/// Reads a 32 bit int.
		/// </summary>
		/// <returns>  the 32 bit integer read. </returns>
		/// <exception cref="EOFException"> If end of file is reached. </exception>
		/// <exception cref="IOException"> If other I/O error has occurred. </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public int readInt() throws IOException
		public virtual int ReadInt()
		{
			return Bin.ReadInt();
		}

		/// <summary>
		/// Reads a 64 bit long.
		/// </summary>
		/// <returns>  the read 64 bit long. </returns>
		/// <exception cref="EOFException"> If end of file is reached. </exception>
		/// <exception cref="IOException"> If other I/O error has occurred. </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public long readLong() throws IOException
		public virtual long ReadLong()
		{
			return Bin.ReadLong();
		}

		/// <summary>
		/// Reads a 32 bit float.
		/// </summary>
		/// <returns>  the 32 bit float read. </returns>
		/// <exception cref="EOFException"> If end of file is reached. </exception>
		/// <exception cref="IOException"> If other I/O error has occurred. </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public float readFloat() throws IOException
		public virtual float ReadFloat()
		{
			return Bin.ReadFloat();
		}

		/// <summary>
		/// Reads a 64 bit double.
		/// </summary>
		/// <returns>  the 64 bit double read. </returns>
		/// <exception cref="EOFException"> If end of file is reached. </exception>
		/// <exception cref="IOException"> If other I/O error has occurred. </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public double readDouble() throws IOException
		public virtual double ReadDouble()
		{
			return Bin.ReadDouble();
		}

		/// <summary>
		/// Reads bytes, blocking until all bytes are read.
		/// </summary>
		/// <param name="buf"> the buffer into which the data is read </param>
		/// <exception cref="EOFException"> If end of file is reached. </exception>
		/// <exception cref="IOException"> If other I/O error has occurred. </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public void readFully(byte[] buf) throws IOException
		public virtual void ReadFully(sbyte[] buf)
		{
			Bin.ReadFully(buf, 0, buf.Length, false);
		}

		/// <summary>
		/// Reads bytes, blocking until all bytes are read.
		/// </summary>
		/// <param name="buf"> the buffer into which the data is read </param>
		/// <param name="off"> the start offset of the data </param>
		/// <param name="len"> the maximum number of bytes to read </param>
		/// <exception cref="EOFException"> If end of file is reached. </exception>
		/// <exception cref="IOException"> If other I/O error has occurred. </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public void readFully(byte[] buf, int off, int len) throws IOException
		public virtual void ReadFully(sbyte[] buf, int off, int len)
		{
			int endoff = off + len;
			if (off < 0 || len < 0 || endoff > buf.Length || endoff < 0)
			{
				throw new IndexOutOfBoundsException();
			}
			Bin.ReadFully(buf, off, len, false);
		}

		/// <summary>
		/// Skips bytes.
		/// </summary>
		/// <param name="len"> the number of bytes to be skipped </param>
		/// <returns>  the actual number of bytes skipped. </returns>
		/// <exception cref="IOException"> If an I/O error has occurred. </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public int skipBytes(int len) throws IOException
		public virtual int SkipBytes(int len)
		{
			return Bin.SkipBytes(len);
		}

		/// <summary>
		/// Reads in a line that has been terminated by a \n, \r, \r\n or EOF.
		/// </summary>
		/// <returns>  a String copy of the line. </returns>
		/// <exception cref="IOException"> if there are I/O errors while reading from the
		///          underlying <code>InputStream</code> </exception>
		/// @deprecated This method does not properly convert bytes to characters.
		///          see DataInputStream for the details and alternatives. 
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: @Deprecated("This method does not properly convert bytes to characters.") public String readLine() throws IOException
		[Obsolete("This method does not properly convert bytes to characters.")]
		public virtual String ReadLine()
		{
			return Bin.ReadLine();
		}

		/// <summary>
		/// Reads a String in
		/// <a href="DataInput.html#modified-utf-8">modified UTF-8</a>
		/// format.
		/// </summary>
		/// <returns>  the String. </returns>
		/// <exception cref="IOException"> if there are I/O errors while reading from the
		///          underlying <code>InputStream</code> </exception>
		/// <exception cref="UTFDataFormatException"> if read bytes do not represent a valid
		///          modified UTF-8 encoding of a string </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public String readUTF() throws IOException
		public virtual String ReadUTF()
		{
			return Bin.ReadUTF();
		}

		/// <summary>
		/// Provide access to the persistent fields read from the input stream.
		/// </summary>
		public abstract class GetField
		{

			/// <summary>
			/// Get the ObjectStreamClass that describes the fields in the stream.
			/// </summary>
			/// <returns>  the descriptor class that describes the serializable fields </returns>
			public abstract ObjectStreamClass ObjectStreamClass {get;}

			/// <summary>
			/// Return true if the named field is defaulted and has no value in this
			/// stream.
			/// </summary>
			/// <param name="name"> the name of the field </param>
			/// <returns> true, if and only if the named field is defaulted </returns>
			/// <exception cref="IOException"> if there are I/O errors while reading from
			///         the underlying <code>InputStream</code> </exception>
			/// <exception cref="IllegalArgumentException"> if <code>name</code> does not
			///         correspond to a serializable field </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public abstract boolean defaulted(String name) throws IOException;
			public abstract bool Defaulted(String name);

			/// <summary>
			/// Get the value of the named boolean field from the persistent field.
			/// </summary>
			/// <param name="name"> the name of the field </param>
			/// <param name="val"> the default value to use if <code>name</code> does not
			///         have a value </param>
			/// <returns> the value of the named <code>boolean</code> field </returns>
			/// <exception cref="IOException"> if there are I/O errors while reading from the
			///         underlying <code>InputStream</code> </exception>
			/// <exception cref="IllegalArgumentException"> if type of <code>name</code> is
			///         not serializable or if the field type is incorrect </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public abstract boolean get(String name, boolean val) throws IOException;
			public abstract bool Get(String name, bool val);

			/// <summary>
			/// Get the value of the named byte field from the persistent field.
			/// </summary>
			/// <param name="name"> the name of the field </param>
			/// <param name="val"> the default value to use if <code>name</code> does not
			///         have a value </param>
			/// <returns> the value of the named <code>byte</code> field </returns>
			/// <exception cref="IOException"> if there are I/O errors while reading from the
			///         underlying <code>InputStream</code> </exception>
			/// <exception cref="IllegalArgumentException"> if type of <code>name</code> is
			///         not serializable or if the field type is incorrect </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public abstract byte get(String name, byte val) throws IOException;
			public abstract sbyte Get(String name, sbyte val);

			/// <summary>
			/// Get the value of the named char field from the persistent field.
			/// </summary>
			/// <param name="name"> the name of the field </param>
			/// <param name="val"> the default value to use if <code>name</code> does not
			///         have a value </param>
			/// <returns> the value of the named <code>char</code> field </returns>
			/// <exception cref="IOException"> if there are I/O errors while reading from the
			///         underlying <code>InputStream</code> </exception>
			/// <exception cref="IllegalArgumentException"> if type of <code>name</code> is
			///         not serializable or if the field type is incorrect </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public abstract char get(String name, char val) throws IOException;
			public abstract char Get(String name, char val);

			/// <summary>
			/// Get the value of the named short field from the persistent field.
			/// </summary>
			/// <param name="name"> the name of the field </param>
			/// <param name="val"> the default value to use if <code>name</code> does not
			///         have a value </param>
			/// <returns> the value of the named <code>short</code> field </returns>
			/// <exception cref="IOException"> if there are I/O errors while reading from the
			///         underlying <code>InputStream</code> </exception>
			/// <exception cref="IllegalArgumentException"> if type of <code>name</code> is
			///         not serializable or if the field type is incorrect </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public abstract short get(String name, short val) throws IOException;
			public abstract short Get(String name, short val);

			/// <summary>
			/// Get the value of the named int field from the persistent field.
			/// </summary>
			/// <param name="name"> the name of the field </param>
			/// <param name="val"> the default value to use if <code>name</code> does not
			///         have a value </param>
			/// <returns> the value of the named <code>int</code> field </returns>
			/// <exception cref="IOException"> if there are I/O errors while reading from the
			///         underlying <code>InputStream</code> </exception>
			/// <exception cref="IllegalArgumentException"> if type of <code>name</code> is
			///         not serializable or if the field type is incorrect </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public abstract int get(String name, int val) throws IOException;
			public abstract int Get(String name, int val);

			/// <summary>
			/// Get the value of the named long field from the persistent field.
			/// </summary>
			/// <param name="name"> the name of the field </param>
			/// <param name="val"> the default value to use if <code>name</code> does not
			///         have a value </param>
			/// <returns> the value of the named <code>long</code> field </returns>
			/// <exception cref="IOException"> if there are I/O errors while reading from the
			///         underlying <code>InputStream</code> </exception>
			/// <exception cref="IllegalArgumentException"> if type of <code>name</code> is
			///         not serializable or if the field type is incorrect </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public abstract long get(String name, long val) throws IOException;
			public abstract long Get(String name, long val);

			/// <summary>
			/// Get the value of the named float field from the persistent field.
			/// </summary>
			/// <param name="name"> the name of the field </param>
			/// <param name="val"> the default value to use if <code>name</code> does not
			///         have a value </param>
			/// <returns> the value of the named <code>float</code> field </returns>
			/// <exception cref="IOException"> if there are I/O errors while reading from the
			///         underlying <code>InputStream</code> </exception>
			/// <exception cref="IllegalArgumentException"> if type of <code>name</code> is
			///         not serializable or if the field type is incorrect </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public abstract float get(String name, float val) throws IOException;
			public abstract float Get(String name, float val);

			/// <summary>
			/// Get the value of the named double field from the persistent field.
			/// </summary>
			/// <param name="name"> the name of the field </param>
			/// <param name="val"> the default value to use if <code>name</code> does not
			///         have a value </param>
			/// <returns> the value of the named <code>double</code> field </returns>
			/// <exception cref="IOException"> if there are I/O errors while reading from the
			///         underlying <code>InputStream</code> </exception>
			/// <exception cref="IllegalArgumentException"> if type of <code>name</code> is
			///         not serializable or if the field type is incorrect </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public abstract double get(String name, double val) throws IOException;
			public abstract double Get(String name, double val);

			/// <summary>
			/// Get the value of the named Object field from the persistent field.
			/// </summary>
			/// <param name="name"> the name of the field </param>
			/// <param name="val"> the default value to use if <code>name</code> does not
			///         have a value </param>
			/// <returns> the value of the named <code>Object</code> field </returns>
			/// <exception cref="IOException"> if there are I/O errors while reading from the
			///         underlying <code>InputStream</code> </exception>
			/// <exception cref="IllegalArgumentException"> if type of <code>name</code> is
			///         not serializable or if the field type is incorrect </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public abstract Object get(String name, Object val) throws IOException;
			public abstract Object Get(String name, Object val);
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
			if (cl == typeof(ObjectInputStream))
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
				for (Class cl = Subcl; cl != typeof(ObjectInputStream); cl = cl.BaseType)
				{
					try
					{
						cl.GetDeclaredMethod("readUnshared", (Class[]) null);
						return false;
					}
					catch (NoSuchMethodException)
					{
					}
					try
					{
						cl.getDeclaredMethod("readFields", (Class[]) null);
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
			Handles.Clear();
			Vlist.Clear();
		}

		/// <summary>
		/// Underlying readObject implementation.
		/// </summary>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: private Object readObject0(boolean unshared) throws IOException
		private Object ReadObject0(bool unshared)
		{
			bool oldMode = Bin.BlockDataMode;
			if (oldMode)
			{
				int remain = Bin.CurrentBlockRemaining();
				if (remain > 0)
				{
					throw new OptionalDataException(remain);
				}
				else if (DefaultDataEnd)
				{
					/*
					 * Fix for 4360508: stream is currently at the end of a field
					 * value block written via default serialization; since there
					 * is no terminating TC_ENDBLOCKDATA tag, simulate
					 * end-of-custom-data behavior explicitly.
					 */
					throw new OptionalDataException(true);
				}
				Bin.BlockDataMode = false;
			}

			sbyte tc;
			while ((tc = Bin.PeekByte()) == ObjectStreamConstants_Fields.TC_RESET)
			{
				Bin.ReadByte();
				HandleReset();
			}

			Depth++;
			try
			{
				switch (tc)
				{
					case ObjectStreamConstants_Fields.TC_NULL:
						return ReadNull();

					case ObjectStreamConstants_Fields.TC_REFERENCE:
						return ReadHandle(unshared);

					case ObjectStreamConstants_Fields.TC_CLASS:
						return ReadClass(unshared);

					case ObjectStreamConstants_Fields.TC_CLASSDESC:
					case ObjectStreamConstants_Fields.TC_PROXYCLASSDESC:
						return ReadClassDesc(unshared);

					case ObjectStreamConstants_Fields.TC_STRING:
					case ObjectStreamConstants_Fields.TC_LONGSTRING:
						return CheckResolve(ReadString(unshared));

					case ObjectStreamConstants_Fields.TC_ARRAY:
						return CheckResolve(ReadArray(unshared));

					case ObjectStreamConstants_Fields.TC_ENUM:
						return CheckResolve(ReadEnum(unshared));

					case ObjectStreamConstants_Fields.TC_OBJECT:
						return CheckResolve(ReadOrdinaryObject(unshared));

					case ObjectStreamConstants_Fields.TC_EXCEPTION:
						IOException ex = ReadFatalException();
						throw new WriteAbortedException("writing aborted", ex);

					case ObjectStreamConstants_Fields.TC_BLOCKDATA:
					case ObjectStreamConstants_Fields.TC_BLOCKDATALONG:
						if (oldMode)
						{
							Bin.BlockDataMode = true;
							Bin.Peek(); // force header read
							throw new OptionalDataException(Bin.CurrentBlockRemaining());
						}
						else
						{
							throw new StreamCorruptedException("unexpected block data");
						}

					case ObjectStreamConstants_Fields.TC_ENDBLOCKDATA:
						if (oldMode)
						{
							throw new OptionalDataException(true);
						}
						else
						{
							throw new StreamCorruptedException("unexpected end of block data");
						}

					default:
						throw new StreamCorruptedException(string.Format("invalid type code: {0:X2}", tc));
				}
			}
			finally
			{
				Depth--;
				Bin.BlockDataMode = oldMode;
			}
		}

		/// <summary>
		/// If resolveObject has been enabled and given object does not have an
		/// exception associated with it, calls resolveObject to determine
		/// replacement for object, and updates handle table accordingly.  Returns
		/// replacement object, or echoes provided object if no replacement
		/// occurred.  Expects that passHandle is set to given object's handle prior
		/// to calling this method.
		/// </summary>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: private Object checkResolve(Object obj) throws IOException
		private Object CheckResolve(Object obj)
		{
			if (!EnableResolve || Handles.LookupException(PassHandle) != null)
			{
				return obj;
			}
			Object rep = ResolveObject(obj);
			if (rep != obj)
			{
				Handles.SetObject(PassHandle, rep);
			}
			return rep;
		}

		/// <summary>
		/// Reads string without allowing it to be replaced in stream.  Called from
		/// within ObjectStreamClass.read().
		/// </summary>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: String readTypeString() throws IOException
		internal virtual String ReadTypeString()
		{
			int oldHandle = PassHandle;
			try
			{
				sbyte tc = Bin.PeekByte();
				switch (tc)
				{
					case ObjectStreamConstants_Fields.TC_NULL:
						return (String) ReadNull();

					case ObjectStreamConstants_Fields.TC_REFERENCE:
						return (String) ReadHandle(false);

					case ObjectStreamConstants_Fields.TC_STRING:
					case ObjectStreamConstants_Fields.TC_LONGSTRING:
						return ReadString(false);

					default:
						throw new StreamCorruptedException(string.Format("invalid type code: {0:X2}", tc));
				}
			}
			finally
			{
				PassHandle = oldHandle;
			}
		}

		/// <summary>
		/// Reads in null code, sets passHandle to NULL_HANDLE and returns null.
		/// </summary>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: private Object readNull() throws IOException
		private Object ReadNull()
		{
			if (Bin.ReadByte() != ObjectStreamConstants_Fields.TC_NULL)
			{
				throw new InternalError();
			}
			PassHandle = NULL_HANDLE;
			return null;
		}

		/// <summary>
		/// Reads in object handle, sets passHandle to the read handle, and returns
		/// object associated with the handle.
		/// </summary>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: private Object readHandle(boolean unshared) throws IOException
		private Object ReadHandle(bool unshared)
		{
			if (Bin.ReadByte() != ObjectStreamConstants_Fields.TC_REFERENCE)
			{
				throw new InternalError();
			}
			PassHandle = Bin.ReadInt() - ObjectStreamConstants_Fields.BaseWireHandle;
			if (PassHandle < 0 || PassHandle >= Handles.Size())
			{
				throw new StreamCorruptedException(string.Format("invalid handle value: {0:X8}", PassHandle + ObjectStreamConstants_Fields.BaseWireHandle));
			}
			if (unshared)
			{
				// REMIND: what type of exception to throw here?
				throw new InvalidObjectException("cannot read back reference as unshared");
			}

			Object obj = Handles.LookupObject(PassHandle);
			if (obj == UnsharedMarker)
			{
				// REMIND: what type of exception to throw here?
				throw new InvalidObjectException("cannot read back reference to unshared object");
			}
			return obj;
		}

		/// <summary>
		/// Reads in and returns class object.  Sets passHandle to class object's
		/// assigned handle.  Returns null if class is unresolvable (in which case a
		/// ClassNotFoundException will be associated with the class' handle in the
		/// handle table).
		/// </summary>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: private Class readClass(boolean unshared) throws IOException
		private Class ReadClass(bool unshared)
		{
			if (Bin.ReadByte() != ObjectStreamConstants_Fields.TC_CLASS)
			{
				throw new InternalError();
			}
			ObjectStreamClass desc = ReadClassDesc(false);
			Class cl = desc.ForClass();
			PassHandle = Handles.Assign(unshared ? UnsharedMarker : cl);

			ClassNotFoundException resolveEx = desc.ResolveException;
			if (resolveEx != null)
			{
				Handles.MarkException(PassHandle, resolveEx);
			}

			Handles.Finish(PassHandle);
			return cl;
		}

		/// <summary>
		/// Reads in and returns (possibly null) class descriptor.  Sets passHandle
		/// to class descriptor's assigned handle.  If class descriptor cannot be
		/// resolved to a class in the local VM, a ClassNotFoundException is
		/// associated with the class descriptor's handle.
		/// </summary>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: private ObjectStreamClass readClassDesc(boolean unshared) throws IOException
		private ObjectStreamClass ReadClassDesc(bool unshared)
		{
			sbyte tc = Bin.PeekByte();
			switch (tc)
			{
				case ObjectStreamConstants_Fields.TC_NULL:
					return (ObjectStreamClass) ReadNull();

				case ObjectStreamConstants_Fields.TC_REFERENCE:
					return (ObjectStreamClass) ReadHandle(unshared);

				case ObjectStreamConstants_Fields.TC_PROXYCLASSDESC:
					return ReadProxyDesc(unshared);

				case ObjectStreamConstants_Fields.TC_CLASSDESC:
					return ReadNonProxyDesc(unshared);

				default:
					throw new StreamCorruptedException(string.Format("invalid type code: {0:X2}", tc));
			}
		}

		private bool CustomSubclass
		{
			get
			{
				// Return true if this class is a custom subclass of ObjectInputStream
				return this.GetType().ClassLoader != typeof(ObjectInputStream).ClassLoader;
			}
		}

		/// <summary>
		/// Reads in and returns class descriptor for a dynamic proxy class.  Sets
		/// passHandle to proxy class descriptor's assigned handle.  If proxy class
		/// descriptor cannot be resolved to a class in the local VM, a
		/// ClassNotFoundException is associated with the descriptor's handle.
		/// </summary>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: private ObjectStreamClass readProxyDesc(boolean unshared) throws IOException
		private ObjectStreamClass ReadProxyDesc(bool unshared)
		{
			if (Bin.ReadByte() != ObjectStreamConstants_Fields.TC_PROXYCLASSDESC)
			{
				throw new InternalError();
			}

			ObjectStreamClass desc = new ObjectStreamClass();
			int descHandle = Handles.Assign(unshared ? UnsharedMarker : desc);
			PassHandle = NULL_HANDLE;

			int numIfaces = Bin.ReadInt();
			String[] ifaces = new String[numIfaces];
			for (int i = 0; i < numIfaces; i++)
			{
				ifaces[i] = Bin.ReadUTF();
			}

			Class cl = null;
			ClassNotFoundException resolveEx = null;
			Bin.BlockDataMode = true;
			try
			{
				if ((cl = ResolveProxyClass(ifaces)) == null)
				{
					resolveEx = new ClassNotFoundException("null class");
				}
				else if (!Proxy.isProxyClass(cl))
				{
					throw new InvalidClassException("Not a proxy");
				}
				else
				{
					// ReflectUtil.checkProxyPackageAccess makes a test
					// equivalent to isCustomSubclass so there's no need
					// to condition this call to isCustomSubclass == true here.
					ReflectUtil.checkProxyPackageAccess(this.GetType().ClassLoader, cl.Interfaces);
				}
			}
			catch (ClassNotFoundException ex)
			{
				resolveEx = ex;
			}
			SkipCustomData();

			desc.InitProxy(cl, resolveEx, ReadClassDesc(false));

			Handles.Finish(descHandle);
			PassHandle = descHandle;
			return desc;
		}

		/// <summary>
		/// Reads in and returns class descriptor for a class that is not a dynamic
		/// proxy class.  Sets passHandle to class descriptor's assigned handle.  If
		/// class descriptor cannot be resolved to a class in the local VM, a
		/// ClassNotFoundException is associated with the descriptor's handle.
		/// </summary>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: private ObjectStreamClass readNonProxyDesc(boolean unshared) throws IOException
		private ObjectStreamClass ReadNonProxyDesc(bool unshared)
		{
			if (Bin.ReadByte() != ObjectStreamConstants_Fields.TC_CLASSDESC)
			{
				throw new InternalError();
			}

			ObjectStreamClass desc = new ObjectStreamClass();
			int descHandle = Handles.Assign(unshared ? UnsharedMarker : desc);
			PassHandle = NULL_HANDLE;

			ObjectStreamClass readDesc = null;
			try
			{
				readDesc = ReadClassDescriptor();
			}
			catch (ClassNotFoundException ex)
			{
				throw (IOException) (new InvalidClassException("failed to read class descriptor")).InitCause(ex);
			}

			Class cl = null;
			ClassNotFoundException resolveEx = null;
			Bin.BlockDataMode = true;
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final boolean checksRequired = isCustomSubclass();
			bool checksRequired = CustomSubclass;
			try
			{
				if ((cl = ResolveClass(readDesc)) == null)
				{
					resolveEx = new ClassNotFoundException("null class");
				}
				else if (checksRequired)
				{
					ReflectUtil.checkPackageAccess(cl);
				}
			}
			catch (ClassNotFoundException ex)
			{
				resolveEx = ex;
			}
			SkipCustomData();

			desc.InitNonProxy(readDesc, cl, resolveEx, ReadClassDesc(false));

			Handles.Finish(descHandle);
			PassHandle = descHandle;
			return desc;
		}

		/// <summary>
		/// Reads in and returns new string.  Sets passHandle to new string's
		/// assigned handle.
		/// </summary>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: private String readString(boolean unshared) throws IOException
		private String ReadString(bool unshared)
		{
			String str;
			sbyte tc = Bin.ReadByte();
			switch (tc)
			{
				case ObjectStreamConstants_Fields.TC_STRING:
					str = Bin.ReadUTF();
					break;

				case ObjectStreamConstants_Fields.TC_LONGSTRING:
					str = Bin.ReadLongUTF();
					break;

				default:
					throw new StreamCorruptedException(string.Format("invalid type code: {0:X2}", tc));
			}
			PassHandle = Handles.Assign(unshared ? UnsharedMarker : str);
			Handles.Finish(PassHandle);
			return str;
		}

		/// <summary>
		/// Reads in and returns array object, or null if array class is
		/// unresolvable.  Sets passHandle to array's assigned handle.
		/// </summary>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: private Object readArray(boolean unshared) throws IOException
		private Object ReadArray(bool unshared)
		{
			if (Bin.ReadByte() != ObjectStreamConstants_Fields.TC_ARRAY)
			{
				throw new InternalError();
			}

			ObjectStreamClass desc = ReadClassDesc(false);
			int len = Bin.ReadInt();

			Object array = null;
			Class cl , ccl = null;
			if ((cl = desc.ForClass()) != null)
			{
				ccl = cl.ComponentType;
				array = Array.newInstance(ccl, len);
			}

			int arrayHandle = Handles.Assign(unshared ? UnsharedMarker : array);
			ClassNotFoundException resolveEx = desc.ResolveException;
			if (resolveEx != null)
			{
				Handles.MarkException(arrayHandle, resolveEx);
			}

			if (ccl == null)
			{
				for (int i = 0; i < len; i++)
				{
					ReadObject0(false);
				}
			}
			else if (ccl.Primitive)
			{
				if (ccl == Integer.TYPE)
				{
					Bin.ReadInts((int[]) array, 0, len);
				}
				else if (ccl == Byte.TYPE)
				{
					Bin.ReadFully((sbyte[]) array, 0, len, true);
				}
				else if (ccl == Long.TYPE)
				{
					Bin.ReadLongs((long[]) array, 0, len);
				}
				else if (ccl == Float.TYPE)
				{
					Bin.ReadFloats((float[]) array, 0, len);
				}
				else if (ccl == Double.TYPE)
				{
					Bin.ReadDoubles((double[]) array, 0, len);
				}
				else if (ccl == Short.TYPE)
				{
					Bin.ReadShorts((short[]) array, 0, len);
				}
				else if (ccl == Character.TYPE)
				{
					Bin.ReadChars((char[]) array, 0, len);
				}
				else if (ccl == Boolean.TYPE)
				{
					Bin.ReadBooleans((bool[]) array, 0, len);
				}
				else
				{
					throw new InternalError();
				}
			}
			else
			{
				Object[] oa = (Object[]) array;
				for (int i = 0; i < len; i++)
				{
					oa[i] = ReadObject0(false);
					Handles.MarkDependency(arrayHandle, PassHandle);
				}
			}

			Handles.Finish(arrayHandle);
			PassHandle = arrayHandle;
			return array;
		}

		/// <summary>
		/// Reads in and returns enum constant, or null if enum type is
		/// unresolvable.  Sets passHandle to enum constant's assigned handle.
		/// </summary>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: private Enum<?> readEnum(boolean unshared) throws IOException
//JAVA TO C# CONVERTER TODO TASK: Java wildcard generics are not converted to .NET:
		private Enum<?> ReadEnum(bool unshared)
		{
			if (Bin.ReadByte() != ObjectStreamConstants_Fields.TC_ENUM)
			{
				throw new InternalError();
			}

			ObjectStreamClass desc = ReadClassDesc(false);
			if (!desc.Enum)
			{
				throw new InvalidClassException("non-enum class: " + desc);
			}

			int enumHandle = Handles.Assign(unshared ? UnsharedMarker : null);
			ClassNotFoundException resolveEx = desc.ResolveException;
			if (resolveEx != null)
			{
				Handles.MarkException(enumHandle, resolveEx);
			}

			String name = ReadString(false);
//JAVA TO C# CONVERTER TODO TASK: Java wildcard generics are not converted to .NET:
//ORIGINAL LINE: Enum<?> result = null;
			Enum<?> result = null;
			Class cl = desc.ForClass();
			if (cl != null)
			{
				try
				{
//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @SuppressWarnings("unchecked") Enum<?> en = Enum.valueOf((Class)cl, name);
//JAVA TO C# CONVERTER TODO TASK: Java wildcard generics are not converted to .NET:
					Enum<?> en = Enum.ValueOf((Class)cl, name);
					result = en;
				}
				catch (IllegalArgumentException ex)
				{
					throw (IOException) (new InvalidObjectException("enum constant " + name + " does not exist in " + cl)).InitCause(ex);
				}
				if (!unshared)
				{
					Handles.SetObject(enumHandle, result);
				}
			}

			Handles.Finish(enumHandle);
			PassHandle = enumHandle;
			return result;
		}

		/// <summary>
		/// Reads and returns "ordinary" (i.e., not a String, Class,
		/// ObjectStreamClass, array, or enum constant) object, or null if object's
		/// class is unresolvable (in which case a ClassNotFoundException will be
		/// associated with object's handle).  Sets passHandle to object's assigned
		/// handle.
		/// </summary>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: private Object readOrdinaryObject(boolean unshared) throws IOException
		private Object ReadOrdinaryObject(bool unshared)
		{
			if (Bin.ReadByte() != ObjectStreamConstants_Fields.TC_OBJECT)
			{
				throw new InternalError();
			}

			ObjectStreamClass desc = ReadClassDesc(false);
			desc.CheckDeserialize();

			Class cl = desc.ForClass();
			if (cl == typeof(String) || cl == typeof(Class) || cl == typeof(ObjectStreamClass))
			{
				throw new InvalidClassException("invalid class descriptor");
			}

			Object obj;
			try
			{
				obj = desc.Instantiable ? desc.NewInstance() : null;
			}
			catch (Exception ex)
			{
				throw (IOException) (new InvalidClassException(desc.ForClass().Name, "unable to create instance")).InitCause(ex);
			}

			PassHandle = Handles.Assign(unshared ? UnsharedMarker : obj);
			ClassNotFoundException resolveEx = desc.ResolveException;
			if (resolveEx != null)
			{
				Handles.MarkException(PassHandle, resolveEx);
			}

			if (desc.Externalizable)
			{
				ReadExternalData((Externalizable) obj, desc);
			}
			else
			{
				ReadSerialData(obj, desc);
			}

			Handles.Finish(PassHandle);

			if (obj != null && Handles.LookupException(PassHandle) == null && desc.HasReadResolveMethod())
			{
				Object rep = desc.InvokeReadResolve(obj);
				if (unshared && rep.GetType().IsArray)
				{
					rep = CloneArray(rep);
				}
				if (rep != obj)
				{
					Handles.SetObject(PassHandle, obj = rep);
				}
			}

			return obj;
		}

		/// <summary>
		/// If obj is non-null, reads externalizable data by invoking readExternal()
		/// method of obj; otherwise, attempts to skip over externalizable data.
		/// Expects that passHandle is set to obj's handle before this method is
		/// called.
		/// </summary>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: private void readExternalData(Externalizable obj, ObjectStreamClass desc) throws IOException
		private void ReadExternalData(Externalizable obj, ObjectStreamClass desc)
		{
			SerialCallbackContext oldContext = CurContext;
			if (oldContext != null)
			{
				oldContext.Check();
			}
			CurContext = null;
			try
			{
				bool blocked = desc.HasBlockExternalData();
				if (blocked)
				{
					Bin.BlockDataMode = true;
				}
				if (obj != null)
				{
					try
					{
						obj.ReadExternal(this);
					}
					catch (ClassNotFoundException ex)
					{
						/*
						 * In most cases, the handle table has already propagated
						 * a CNFException to passHandle at this point; this mark
						 * call is included to address cases where the readExternal
						 * method has cons'ed and thrown a new CNFException of its
						 * own.
						 */
						 Handles.MarkException(PassHandle, ex);
					}
				}
				if (blocked)
				{
					SkipCustomData();
				}
			}
			finally
			{
				if (oldContext != null)
				{
					oldContext.Check();
				}
				CurContext = oldContext;
			}
			/*
			 * At this point, if the externalizable data was not written in
			 * block-data form and either the externalizable class doesn't exist
			 * locally (i.e., obj == null) or readExternal() just threw a
			 * CNFException, then the stream is probably in an inconsistent state,
			 * since some (or all) of the externalizable data may not have been
			 * consumed.  Since there's no "correct" action to take in this case,
			 * we mimic the behavior of past serialization implementations and
			 * blindly hope that the stream is in sync; if it isn't and additional
			 * externalizable data remains in the stream, a subsequent read will
			 * most likely throw a StreamCorruptedException.
			 */
		}

		/// <summary>
		/// Reads (or attempts to skip, if obj is null or is tagged with a
		/// ClassNotFoundException) instance data for each serializable class of
		/// object in stream, from superclass to subclass.  Expects that passHandle
		/// is set to obj's handle before this method is called.
		/// </summary>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: private void readSerialData(Object obj, ObjectStreamClass desc) throws IOException
		private void ReadSerialData(Object obj, ObjectStreamClass desc)
		{
			ObjectStreamClass.ClassDataSlot[] slots = desc.ClassDataLayout;
			for (int i = 0; i < slots.Length; i++)
			{
				ObjectStreamClass slotDesc = slots[i].Desc;

				if (slots[i].HasData)
				{
					if (obj == null || Handles.LookupException(PassHandle) != null)
					{
						DefaultReadFields(null, slotDesc); // skip field values
					}
					else if (slotDesc.HasReadObjectMethod())
					{
						SerialCallbackContext oldContext = CurContext;
						if (oldContext != null)
						{
							oldContext.Check();
						}
						try
						{
							CurContext = new SerialCallbackContext(obj, slotDesc);

							Bin.BlockDataMode = true;
							slotDesc.InvokeReadObject(obj, this);
						}
						catch (ClassNotFoundException ex)
						{
							/*
							 * In most cases, the handle table has already
							 * propagated a CNFException to passHandle at this
							 * point; this mark call is included to address cases
							 * where the custom readObject method has cons'ed and
							 * thrown a new CNFException of its own.
							 */
							Handles.MarkException(PassHandle, ex);
						}
						finally
						{
							CurContext.SetUsed();
							if (oldContext != null)
							{
								oldContext.Check();
							}
							CurContext = oldContext;
						}

						/*
						 * defaultDataEnd may have been set indirectly by custom
						 * readObject() method when calling defaultReadObject() or
						 * readFields(); clear it to restore normal read behavior.
						 */
						DefaultDataEnd = false;
					}
					else
					{
						DefaultReadFields(obj, slotDesc);
					}

					if (slotDesc.HasWriteObjectData())
					{
						SkipCustomData();
					}
					else
					{
						Bin.BlockDataMode = false;
					}
				}
				else
				{
					if (obj != null && slotDesc.HasReadObjectNoDataMethod() && Handles.LookupException(PassHandle) == null)
					{
						slotDesc.InvokeReadObjectNoData(obj);
					}
				}
			}
		}

		/// <summary>
		/// Skips over all block data and objects until TC_ENDBLOCKDATA is
		/// encountered.
		/// </summary>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: private void skipCustomData() throws IOException
		private void SkipCustomData()
		{
			int oldHandle = PassHandle;
			for (;;)
			{
				if (Bin.BlockDataMode)
				{
					Bin.SkipBlockData();
					Bin.BlockDataMode = false;
				}
				switch (Bin.PeekByte())
				{
					case ObjectStreamConstants_Fields.TC_BLOCKDATA:
					case ObjectStreamConstants_Fields.TC_BLOCKDATALONG:
						Bin.BlockDataMode = true;
						break;

					case ObjectStreamConstants_Fields.TC_ENDBLOCKDATA:
						Bin.ReadByte();
						PassHandle = oldHandle;
						return;

					default:
						ReadObject0(false);
						break;
				}
			}
		}

		/// <summary>
		/// Reads in values of serializable fields declared by given class
		/// descriptor.  If obj is non-null, sets field values in obj.  Expects that
		/// passHandle is set to obj's handle before this method is called.
		/// </summary>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: private void defaultReadFields(Object obj, ObjectStreamClass desc) throws IOException
		private void DefaultReadFields(Object obj, ObjectStreamClass desc)
		{
			Class cl = desc.ForClass();
			if (cl != null && obj != null && !cl.isInstance(obj))
			{
				throw new ClassCastException();
			}

			int primDataSize = desc.PrimDataSize;
			if (PrimVals == null || PrimVals.Length < primDataSize)
			{
				PrimVals = new sbyte[primDataSize];
			}
			Bin.ReadFully(PrimVals, 0, primDataSize, false);
			if (obj != null)
			{
				desc.SetPrimFieldValues(obj, PrimVals);
			}

			int objHandle = PassHandle;
			ObjectStreamField[] fields = desc.GetFields(false);
			Object[] objVals = new Object[desc.NumObjFields];
			int numPrimFields = fields.Length - objVals.Length;
			for (int i = 0; i < objVals.Length; i++)
			{
				ObjectStreamField f = fields[numPrimFields + i];
				objVals[i] = ReadObject0(f.Unshared);
				if (f.Field != null)
				{
					Handles.MarkDependency(objHandle, PassHandle);
				}
			}
			if (obj != null)
			{
				desc.SetObjFieldValues(obj, objVals);
			}
			PassHandle = objHandle;
		}

		/// <summary>
		/// Reads in and returns IOException that caused serialization to abort.
		/// All stream state is discarded prior to reading in fatal exception.  Sets
		/// passHandle to fatal exception's handle.
		/// </summary>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: private IOException readFatalException() throws IOException
		private IOException ReadFatalException()
		{
			if (Bin.ReadByte() != ObjectStreamConstants_Fields.TC_EXCEPTION)
			{
				throw new InternalError();
			}
			Clear();
			return (IOException) ReadObject0(false);
		}

		/// <summary>
		/// If recursion depth is 0, clears internal data structures; otherwise,
		/// throws a StreamCorruptedException.  This method is called when a
		/// TC_RESET typecode is encountered.
		/// </summary>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: private void handleReset() throws StreamCorruptedException
		private void HandleReset()
		{
			if (Depth > 0)
			{
				throw new StreamCorruptedException("unexpected reset; recursion depth: " + Depth);
			}
			Clear();
		}

		/// <summary>
		/// Converts specified span of bytes into float values.
		/// </summary>
		// REMIND: remove once hotspot inlines Float.intBitsToFloat
//JAVA TO C# CONVERTER TODO TASK: Replace 'unknown' with the appropriate dll name:
		[DllImport("unknown")]
		private static extern void bytesToFloats(sbyte[] src, int srcpos, float[] dst, int dstpos, int nfloats);

		/// <summary>
		/// Converts specified span of bytes into double values.
		/// </summary>
		// REMIND: remove once hotspot inlines Double.longBitsToDouble
//JAVA TO C# CONVERTER TODO TASK: Replace 'unknown' with the appropriate dll name:
		[DllImport("unknown")]
		private static extern void bytesToDoubles(sbyte[] src, int srcpos, double[] dst, int dstpos, int ndoubles);

		/// <summary>
		/// Returns the first non-null class loader (not counting class loaders of
		/// generated reflection implementation classes) up the execution stack, or
		/// null if only code from the null class loader is on the stack.  This
		/// method is also called via reflection by the following RMI-IIOP class:
		/// 
		///     com.sun.corba.se.internal.util.JDKClassLoader
		/// 
		/// This method should not be removed or its signature changed without
		/// corresponding modifications to the above class.
		/// </summary>
		private static ClassLoader LatestUserDefinedLoader()
		{
			return sun.misc.VM.latestUserDefinedLoader();
		}

		/// <summary>
		/// Default GetField implementation.
		/// </summary>
		private class GetFieldImpl : GetField
		{
			private readonly ObjectInputStream OuterInstance;


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
			/// object field value handles </summary>
			internal readonly int[] ObjHandles;

			/// <summary>
			/// Creates GetFieldImpl object for reading fields defined in given
			/// class descriptor.
			/// </summary>
			internal GetFieldImpl(ObjectInputStream outerInstance, ObjectStreamClass desc)
			{
				this.OuterInstance = outerInstance;
				this.Desc = desc;
				PrimVals = new sbyte[desc.PrimDataSize];
				ObjVals = new Object[desc.NumObjFields];
				ObjHandles = new int[ObjVals.Length];
			}

			public override ObjectStreamClass ObjectStreamClass
			{
				get
				{
					return Desc;
				}
			}

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public boolean defaulted(String name) throws IOException
			public override bool Defaulted(String name)
			{
				return (GetFieldOffset(name, null) < 0);
			}

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public boolean get(String name, boolean val) throws IOException
			public override bool Get(String name, bool val)
			{
				int off = GetFieldOffset(name, Boolean.TYPE);
				return (off >= 0) ? Bits.GetBoolean(PrimVals, off) : val;
			}

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public byte get(String name, byte val) throws IOException
			public override sbyte Get(String name, sbyte val)
			{
				int off = GetFieldOffset(name, Byte.TYPE);
				return (off >= 0) ? PrimVals[off] : val;
			}

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public char get(String name, char val) throws IOException
			public override char Get(String name, char val)
			{
				int off = GetFieldOffset(name, Character.TYPE);
				return (off >= 0) ? Bits.GetChar(PrimVals, off) : val;
			}

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public short get(String name, short val) throws IOException
			public override short Get(String name, short val)
			{
				int off = GetFieldOffset(name, Short.TYPE);
				return (off >= 0) ? Bits.GetShort(PrimVals, off) : val;
			}

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public int get(String name, int val) throws IOException
			public override int Get(String name, int val)
			{
				int off = GetFieldOffset(name, Integer.TYPE);
				return (off >= 0) ? Bits.GetInt(PrimVals, off) : val;
			}

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public float get(String name, float val) throws IOException
			public override float Get(String name, float val)
			{
				int off = GetFieldOffset(name, Float.TYPE);
				return (off >= 0) ? Bits.GetFloat(PrimVals, off) : val;
			}

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public long get(String name, long val) throws IOException
			public override long Get(String name, long val)
			{
				int off = GetFieldOffset(name, Long.TYPE);
				return (off >= 0) ? Bits.GetLong(PrimVals, off) : val;
			}

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public double get(String name, double val) throws IOException
			public override double Get(String name, double val)
			{
				int off = GetFieldOffset(name, Double.TYPE);
				return (off >= 0) ? Bits.GetDouble(PrimVals, off) : val;
			}

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public Object get(String name, Object val) throws IOException
			public override Object Get(String name, Object val)
			{
				int off = GetFieldOffset(name, typeof(Object));
				if (off >= 0)
				{
					int objHandle = ObjHandles[off];
					outerInstance.Handles.MarkDependency(outerInstance.PassHandle, objHandle);
					return (outerInstance.Handles.LookupException(objHandle) == null) ? ObjVals[off] : null;
				}
				else
				{
					return val;
				}
			}

			/// <summary>
			/// Reads primitive and object field values from stream.
			/// </summary>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: void readFields() throws IOException
			internal virtual void ReadFields()
			{
				outerInstance.Bin.ReadFully(PrimVals, 0, PrimVals.Length, false);

				int oldHandle = outerInstance.PassHandle;
				ObjectStreamField[] fields = Desc.GetFields(false);
				int numPrimFields = fields.Length - ObjVals.Length;
				for (int i = 0; i < ObjVals.Length; i++)
				{
					ObjVals[i] = outerInstance.ReadObject0(fields[numPrimFields + i].Unshared);
					ObjHandles[i] = outerInstance.PassHandle;
				}
				outerInstance.PassHandle = oldHandle;
			}

			/// <summary>
			/// Returns offset of field with given name and type.  A specified type
			/// of null matches all types, Object.class matches all non-primitive
			/// types, and any other non-null type matches assignable types only.
			/// If no matching field is found in the (incoming) class
			/// descriptor but a matching field is present in the associated local
			/// class descriptor, returns -1.  Throws IllegalArgumentException if
			/// neither incoming nor local class descriptor contains a match.
			/// </summary>
			internal virtual int GetFieldOffset(String name, Class type)
			{
				ObjectStreamField field = Desc.GetField(name, type);
				if (field != null)
				{
					return field.Offset;
				}
				else if (Desc.LocalDesc.GetField(name, type) != null)
				{
					return -1;
				}
				else
				{
					throw new IllegalArgumentException("no such field " + name + " with type " + type);
				}
			}
		}

		/// <summary>
		/// Prioritized list of callbacks to be performed once object graph has been
		/// completely deserialized.
		/// </summary>
		private class ValidationList
		{

			private class Callback
			{
				internal readonly ObjectInputValidation Obj;
				internal readonly int Priority;
				internal Callback Next;
				internal readonly AccessControlContext Acc;

				internal Callback(ObjectInputValidation obj, int priority, Callback next, AccessControlContext acc)
				{
					this.Obj = obj;
					this.Priority = priority;
					this.Next = next;
					this.Acc = acc;
				}
			}

			/// <summary>
			/// linked list of callbacks </summary>
			internal Callback List;

			/// <summary>
			/// Creates new (empty) ValidationList.
			/// </summary>
			internal ValidationList()
			{
			}

			/// <summary>
			/// Registers callback.  Throws InvalidObjectException if callback
			/// object is null.
			/// </summary>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: void register(ObjectInputValidation obj, int priority) throws InvalidObjectException
			internal virtual void Register(ObjectInputValidation obj, int priority)
			{
				if (obj == null)
				{
					throw new InvalidObjectException("null callback");
				}

				Callback prev = null, cur = List;
				while (cur != null && priority < cur.Priority)
				{
					prev = cur;
					cur = cur.Next;
				}
				AccessControlContext acc = AccessController.Context;
				if (prev != null)
				{
					prev.Next = new Callback(obj, priority, cur, acc);
				}
				else
				{
					List = new Callback(obj, priority, List, acc);
				}
			}

			/// <summary>
			/// Invokes all registered callbacks and clears the callback list.
			/// Callbacks with higher priorities are called first; those with equal
			/// priorities may be called in any order.  If any of the callbacks
			/// throws an InvalidObjectException, the callback process is terminated
			/// and the exception propagated upwards.
			/// </summary>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: void doCallbacks() throws InvalidObjectException
			internal virtual void DoCallbacks()
			{
				try
				{
					while (List != null)
					{
						AccessController.doPrivileged(new PrivilegedExceptionActionAnonymousInnerClassHelper(this), List.Acc);
						List = List.Next;
					}
				}
				catch (PrivilegedActionException ex)
				{
					List = null;
					throw (InvalidObjectException) ex.Exception;
				}
			}

			private class PrivilegedExceptionActionAnonymousInnerClassHelper : PrivilegedExceptionAction<Void>
			{
				private readonly ValidationList OuterInstance;

				public PrivilegedExceptionActionAnonymousInnerClassHelper(ValidationList outerInstance)
				{
					this.OuterInstance = outerInstance;
				}

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public Void run() throws InvalidObjectException
				public virtual Void Run()
				{
					OuterInstance.List.Obj.ValidateObject();
					return null;
				}
			}

			/// <summary>
			/// Resets the callback list to its initial (empty) state.
			/// </summary>
			public virtual void Clear()
			{
				List = null;
			}
		}

		/// <summary>
		/// Input stream supporting single-byte peek operations.
		/// </summary>
		private class PeekInputStream : InputStream
		{

			/// <summary>
			/// underlying stream </summary>
			internal readonly InputStream @in;
			/// <summary>
			/// peeked byte </summary>
			internal int Peekb = -1;

			/// <summary>
			/// Creates new PeekInputStream on top of given underlying stream.
			/// </summary>
			internal PeekInputStream(InputStream @in)
			{
				this.@in = @in;
			}

			/// <summary>
			/// Peeks at next byte value in stream.  Similar to read(), except
			/// that it does not consume the read value.
			/// </summary>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: int peek() throws IOException
			internal virtual int Peek()
			{
				return (Peekb >= 0) ? Peekb : (Peekb = @in.Read());
			}

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public int read() throws IOException
			public override int Read()
			{
				if (Peekb >= 0)
				{
					int v = Peekb;
					Peekb = -1;
					return v;
				}
				else
				{
					return @in.Read();
				}
			}

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public int read(byte[] b, int off, int len) throws IOException
			public override int Read(sbyte[] b, int off, int len)
			{
				if (len == 0)
				{
					return 0;
				}
				else if (Peekb < 0)
				{
					return @in.Read(b, off, len);
				}
				else
				{
					b[off++] = (sbyte) Peekb;
					len--;
					Peekb = -1;
					int n = @in.Read(b, off, len);
					return (n >= 0) ? (n + 1) : 1;
				}
			}

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: void readFully(byte[] b, int off, int len) throws IOException
			internal virtual void ReadFully(sbyte[] b, int off, int len)
			{
				int n = 0;
				while (n < len)
				{
					int count = Read(b, off + n, len - n);
					if (count < 0)
					{
						throw new EOFException();
					}
					n += count;
				}
			}

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public long skip(long n) throws IOException
			public override long Skip(long n)
			{
				if (n <= 0)
				{
					return 0;
				}
				int skipped = 0;
				if (Peekb >= 0)
				{
					Peekb = -1;
					skipped++;
					n--;
				}
				return skipped + Skip(n);
			}

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public int available() throws IOException
			public override int Available()
			{
				return @in.Available() + ((Peekb >= 0) ? 1 : 0);
			}

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public void close() throws IOException
			public override void Close()
			{
				@in.Close();
			}
		}

		/// <summary>
		/// Input stream with two modes: in default mode, inputs data written in the
		/// same format as DataOutputStream; in "block data" mode, inputs data
		/// bracketed by block data markers (see object serialization specification
		/// for details).  Buffering depends on block data mode: when in default
		/// mode, no data is buffered in advance; when in block data mode, all data
		/// for the current data block is read in at once (and buffered).
		/// </summary>
		private class BlockDataInputStream : InputStream, DataInput
		{
			private readonly ObjectInputStream OuterInstance;

			/// <summary>
			/// maximum data block length </summary>
			internal const int MAX_BLOCK_SIZE = 1024;
			/// <summary>
			/// maximum data block header length </summary>
			internal const int MAX_HEADER_SIZE = 5;
			/// <summary>
			/// (tunable) length of char buffer (for reading strings) </summary>
			internal const int CHAR_BUF_SIZE = 256;
			/// <summary>
			/// readBlockHeader() return value indicating header read may block </summary>
			internal const int HEADER_BLOCKED = -2;

			/// <summary>
			/// buffer for reading general/block data </summary>
			internal readonly sbyte[] Buf = new sbyte[MAX_BLOCK_SIZE];
			/// <summary>
			/// buffer for reading block data headers </summary>
			internal readonly sbyte[] Hbuf = new sbyte[MAX_HEADER_SIZE];
			/// <summary>
			/// char buffer for fast string reads </summary>
			internal readonly char[] Cbuf = new char[CHAR_BUF_SIZE];

			/// <summary>
			/// block data mode </summary>
			internal bool Blkmode = false;

			// block data state fields; values meaningful only when blkmode true
			/// <summary>
			/// current offset into buf </summary>
			internal int Pos = 0;
			/// <summary>
			/// end offset of valid data in buf, or -1 if no more block data </summary>
			internal int End = -1;
			/// <summary>
			/// number of bytes in current block yet to be read from stream </summary>
			internal int Unread = 0;

			/// <summary>
			/// underlying stream (wrapped in peekable filter stream) </summary>
			internal readonly PeekInputStream @in;
			/// <summary>
			/// loopback stream (for data reads that span data blocks) </summary>
			internal readonly DataInputStream Din;

			/// <summary>
			/// Creates new BlockDataInputStream on top of given underlying stream.
			/// Block data mode is turned off by default.
			/// </summary>
			internal BlockDataInputStream(ObjectInputStream outerInstance, InputStream @in)
			{
				this.OuterInstance = outerInstance;
				this.@in = new PeekInputStream(@in);
				Din = new DataInputStream(this);
			}

			/// <summary>
			/// Sets block data mode to the given mode (true == on, false == off)
			/// and returns the previous mode value.  If the new mode is the same as
			/// the old mode, no action is taken.  Throws IllegalStateException if
			/// block data mode is being switched from on to off while unconsumed
			/// block data is still present in the stream.
			/// </summary>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: boolean setBlockDataMode(boolean newmode) throws IOException
			internal virtual bool SetBlockDataMode(bool newmode)
			{
				if (Blkmode == newmode)
				{
					return Blkmode;
				}
				if (newmode)
				{
					Pos = 0;
					End = 0;
					Unread = 0;
				}
				else if (Pos < End)
				{
					throw new IllegalStateException("unread block data");
				}
				Blkmode = newmode;
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

			/// <summary>
			/// If in block data mode, skips to the end of the current group of data
			/// blocks (but does not unset block data mode).  If not in block data
			/// mode, throws an IllegalStateException.
			/// </summary>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: void skipBlockData() throws IOException
			internal virtual void SkipBlockData()
			{
				if (!Blkmode)
				{
					throw new IllegalStateException("not in block data mode");
				}
				while (End >= 0)
				{
					Refill();
				}
			}

			/// <summary>
			/// Attempts to read in the next block data header (if any).  If
			/// canBlock is false and a full header cannot be read without possibly
			/// blocking, returns HEADER_BLOCKED, else if the next element in the
			/// stream is a block data header, returns the block data length
			/// specified by the header, else returns -1.
			/// </summary>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: private int readBlockHeader(boolean canBlock) throws IOException
			internal virtual int ReadBlockHeader(bool canBlock)
			{
				if (outerInstance.DefaultDataEnd)
				{
					/*
					 * Fix for 4360508: stream is currently at the end of a field
					 * value block written via default serialization; since there
					 * is no terminating TC_ENDBLOCKDATA tag, simulate
					 * end-of-custom-data behavior explicitly.
					 */
					return -1;
				}
				try
				{
					for (;;)
					{
						int avail = canBlock ? Integer.MaxValue : @in.Available();
						if (avail == 0)
						{
							return HEADER_BLOCKED;
						}

						int tc = @in.Peek();
						switch (tc)
						{
							case ObjectStreamConstants_Fields.TC_BLOCKDATA:
								if (avail < 2)
								{
									return HEADER_BLOCKED;
								}
								@in.ReadFully(Hbuf, 0, 2);
								return Hbuf[1] & 0xFF;

							case ObjectStreamConstants_Fields.TC_BLOCKDATALONG:
								if (avail < 5)
								{
									return HEADER_BLOCKED;
								}
								@in.ReadFully(Hbuf, 0, 5);
								int len = Bits.GetInt(Hbuf, 1);
								if (len < 0)
								{
									throw new StreamCorruptedException("illegal block data header length: " + len);
								}
								return len;

							/*
							 * TC_RESETs may occur in between data blocks.
							 * Unfortunately, this case must be parsed at a lower
							 * level than other typecodes, since primitive data
							 * reads may span data blocks separated by a TC_RESET.
							 */
							case ObjectStreamConstants_Fields.TC_RESET:
								@in.Read();
								outerInstance.HandleReset();
								break;

							default:
								if (tc >= 0 && (tc < ObjectStreamConstants_Fields.TC_BASE || tc > ObjectStreamConstants_Fields.TC_MAX))
								{
									throw new StreamCorruptedException(string.Format("invalid type code: {0:X2}", tc));
								}
								return -1;
						}
					}
				}
				catch (EOFException)
				{
					throw new StreamCorruptedException("unexpected EOF while reading block data header");
				}
			}

			/// <summary>
			/// Refills internal buffer buf with block data.  Any data in buf at the
			/// time of the call is considered consumed.  Sets the pos, end, and
			/// unread fields to reflect the new amount of available block data; if
			/// the next element in the stream is not a data block, sets pos and
			/// unread to 0 and end to -1.
			/// </summary>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: private void refill() throws IOException
			internal virtual void Refill()
			{
				try
				{
					do
					{
						Pos = 0;
						if (Unread > 0)
						{
							int n = @in.Read(Buf, 0, System.Math.Min(Unread, MAX_BLOCK_SIZE));
							if (n >= 0)
							{
								End = n;
								Unread -= n;
							}
							else
							{
								throw new StreamCorruptedException("unexpected EOF in middle of data block");
							}
						}
						else
						{
							int n = ReadBlockHeader(true);
							if (n >= 0)
							{
								End = 0;
								Unread = n;
							}
							else
							{
								End = -1;
								Unread = 0;
							}
						}
					} while (Pos == End);
				}
				catch (IOException ex)
				{
					Pos = 0;
					End = -1;
					Unread = 0;
					throw ex;
				}
			}

			/// <summary>
			/// If in block data mode, returns the number of unconsumed bytes
			/// remaining in the current data block.  If not in block data mode,
			/// throws an IllegalStateException.
			/// </summary>
			internal virtual int CurrentBlockRemaining()
			{
				if (Blkmode)
				{
					return (End >= 0) ? (End - Pos) + Unread : 0;
				}
				else
				{
					throw new IllegalStateException();
				}
			}

			/// <summary>
			/// Peeks at (but does not consume) and returns the next byte value in
			/// the stream, or -1 if the end of the stream/block data (if in block
			/// data mode) has been reached.
			/// </summary>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: int peek() throws IOException
			internal virtual int Peek()
			{
				if (Blkmode)
				{
					if (Pos == End)
					{
						Refill();
					}
					return (End >= 0) ? (Buf[Pos] & 0xFF) : -1;
				}
				else
				{
					return @in.Peek();
				}
			}

			/// <summary>
			/// Peeks at (but does not consume) and returns the next byte value in
			/// the stream, or throws EOFException if end of stream/block data has
			/// been reached.
			/// </summary>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: byte peekByte() throws IOException
			internal virtual sbyte PeekByte()
			{
				int val = Peek();
				if (val < 0)
				{
					throw new EOFException();
				}
				return (sbyte) val;
			}


			/* ----------------- generic input stream methods ------------------ */
			/*
			 * The following methods are equivalent to their counterparts in
			 * InputStream, except that they interpret data block boundaries and
			 * read the requested data from within data blocks when in block data
			 * mode.
			 */

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public int read() throws IOException
			public override int Read()
			{
				if (Blkmode)
				{
					if (Pos == End)
					{
						Refill();
					}
					return (End >= 0) ? (Buf[Pos++] & 0xFF) : -1;
				}
				else
				{
					return @in.Read();
				}
			}

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public int read(byte[] b, int off, int len) throws IOException
			public override int Read(sbyte[] b, int off, int len)
			{
				return Read(b, off, len, false);
			}

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public long skip(long len) throws IOException
			public override long Skip(long len)
			{
				long remain = len;
				while (remain > 0)
				{
					if (Blkmode)
					{
						if (Pos == End)
						{
							Refill();
						}
						if (End < 0)
						{
							break;
						}
						int nread = (int) System.Math.Min(remain, End - Pos);
						remain -= nread;
						Pos += nread;
					}
					else
					{
						int nread = (int) System.Math.Min(remain, MAX_BLOCK_SIZE);
						if ((nread = @in.Read(Buf, 0, nread)) < 0)
						{
							break;
						}
						remain -= nread;
					}
				}
				return len - remain;
			}

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public int available() throws IOException
			public override int Available()
			{
				if (Blkmode)
				{
					if ((Pos == End) && (Unread == 0))
					{
						int n;
						while ((n = ReadBlockHeader(false)) == 0);
						switch (n)
						{
							case HEADER_BLOCKED:
								break;

							case -1:
								Pos = 0;
								End = -1;
								break;

							default:
								Pos = 0;
								End = 0;
								Unread = n;
								break;
						}
					}
					// avoid unnecessary call to in.available() if possible
					int unreadAvail = (Unread > 0) ? System.Math.Min(@in.Available(), Unread) : 0;
					return (End >= 0) ? (End - Pos) + unreadAvail : 0;
				}
				else
				{
					return @in.Available();
				}
			}

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public void close() throws IOException
			public override void Close()
			{
				if (Blkmode)
				{
					Pos = 0;
					End = -1;
					Unread = 0;
				}
				@in.Close();
			}

			/// <summary>
			/// Attempts to read len bytes into byte array b at offset off.  Returns
			/// the number of bytes read, or -1 if the end of stream/block data has
			/// been reached.  If copy is true, reads values into an intermediate
			/// buffer before copying them to b (to avoid exposing a reference to
			/// b).
			/// </summary>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: int read(byte[] b, int off, int len, boolean copy) throws IOException
			internal virtual int Read(sbyte[] b, int off, int len, bool copy)
			{
				if (len == 0)
				{
					return 0;
				}
				else if (Blkmode)
				{
					if (Pos == End)
					{
						Refill();
					}
					if (End < 0)
					{
						return -1;
					}
					int nread = System.Math.Min(len, End - Pos);
					System.Array.Copy(Buf, Pos, b, off, nread);
					Pos += nread;
					return nread;
				}
				else if (copy)
				{
					int nread = @in.Read(Buf, 0, System.Math.Min(len, MAX_BLOCK_SIZE));
					if (nread > 0)
					{
						System.Array.Copy(Buf, 0, b, off, nread);
					}
					return nread;
				}
				else
				{
					return @in.Read(b, off, len);
				}
			}

			/* ----------------- primitive data input methods ------------------ */
			/*
			 * The following methods are equivalent to their counterparts in
			 * DataInputStream, except that they interpret data block boundaries
			 * and read the requested data from within data blocks when in block
			 * data mode.
			 */

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public void readFully(byte[] b) throws IOException
			public virtual void ReadFully(sbyte[] b)
			{
				ReadFully(b, 0, b.Length, false);
			}

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public void readFully(byte[] b, int off, int len) throws IOException
			public virtual void ReadFully(sbyte[] b, int off, int len)
			{
				ReadFully(b, off, len, false);
			}

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public void readFully(byte[] b, int off, int len, boolean copy) throws IOException
			public virtual void ReadFully(sbyte[] b, int off, int len, bool copy)
			{
				while (len > 0)
				{
					int n = Read(b, off, len, copy);
					if (n < 0)
					{
						throw new EOFException();
					}
					off += n;
					len -= n;
				}
			}

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public int skipBytes(int n) throws IOException
			public virtual int SkipBytes(int n)
			{
				return Din.SkipBytes(n);
			}

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public boolean readBoolean() throws IOException
			public virtual bool ReadBoolean()
			{
				int v = Read();
				if (v < 0)
				{
					throw new EOFException();
				}
				return (v != 0);
			}

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public byte readByte() throws IOException
			public virtual sbyte ReadByte()
			{
				int v = Read();
				if (v < 0)
				{
					throw new EOFException();
				}
				return (sbyte) v;
			}

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public int readUnsignedByte() throws IOException
			public virtual int ReadUnsignedByte()
			{
				int v = Read();
				if (v < 0)
				{
					throw new EOFException();
				}
				return v;
			}

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public char readChar() throws IOException
			public virtual char ReadChar()
			{
				if (!Blkmode)
				{
					Pos = 0;
					@in.ReadFully(Buf, 0, 2);
				}
				else if (End - Pos < 2)
				{
					return Din.ReadChar();
				}
				char v = Bits.GetChar(Buf, Pos);
				Pos += 2;
				return v;
			}

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public short readShort() throws IOException
			public virtual short ReadShort()
			{
				if (!Blkmode)
				{
					Pos = 0;
					@in.ReadFully(Buf, 0, 2);
				}
				else if (End - Pos < 2)
				{
					return Din.ReadShort();
				}
				short v = Bits.GetShort(Buf, Pos);
				Pos += 2;
				return v;
			}

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public int readUnsignedShort() throws IOException
			public virtual int ReadUnsignedShort()
			{
				if (!Blkmode)
				{
					Pos = 0;
					@in.ReadFully(Buf, 0, 2);
				}
				else if (End - Pos < 2)
				{
					return Din.ReadUnsignedShort();
				}
				int v = Bits.GetShort(Buf, Pos) & 0xFFFF;
				Pos += 2;
				return v;
			}

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public int readInt() throws IOException
			public virtual int ReadInt()
			{
				if (!Blkmode)
				{
					Pos = 0;
					@in.ReadFully(Buf, 0, 4);
				}
				else if (End - Pos < 4)
				{
					return Din.ReadInt();
				}
				int v = Bits.GetInt(Buf, Pos);
				Pos += 4;
				return v;
			}

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public float readFloat() throws IOException
			public virtual float ReadFloat()
			{
				if (!Blkmode)
				{
					Pos = 0;
					@in.ReadFully(Buf, 0, 4);
				}
				else if (End - Pos < 4)
				{
					return Din.ReadFloat();
				}
				float v = Bits.GetFloat(Buf, Pos);
				Pos += 4;
				return v;
			}

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public long readLong() throws IOException
			public virtual long ReadLong()
			{
				if (!Blkmode)
				{
					Pos = 0;
					@in.ReadFully(Buf, 0, 8);
				}
				else if (End - Pos < 8)
				{
					return Din.ReadLong();
				}
				long v = Bits.GetLong(Buf, Pos);
				Pos += 8;
				return v;
			}

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public double readDouble() throws IOException
			public virtual double ReadDouble()
			{
				if (!Blkmode)
				{
					Pos = 0;
					@in.ReadFully(Buf, 0, 8);
				}
				else if (End - Pos < 8)
				{
					return Din.ReadDouble();
				}
				double v = Bits.GetDouble(Buf, Pos);
				Pos += 8;
				return v;
			}

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public String readUTF() throws IOException
			public virtual String ReadUTF()
			{
				return ReadUTFBody(ReadUnsignedShort());
			}

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @SuppressWarnings("deprecation") public String readLine() throws IOException
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
			public virtual String ReadLine()
			{
				return Din.ReadLine(); // deprecated, not worth optimizing
			}

			/* -------------- primitive data array input methods --------------- */
			/*
			 * The following methods read in spans of primitive data values.
			 * Though equivalent to calling the corresponding primitive read
			 * methods repeatedly, these methods are optimized for reading groups
			 * of primitive data values more efficiently.
			 */

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: void readBooleans(boolean[] v, int off, int len) throws IOException
			internal virtual void ReadBooleans(bool[] v, int off, int len)
			{
				int stop , endoff = off + len;
				while (off < endoff)
				{
					if (!Blkmode)
					{
						int span = System.Math.Min(endoff - off, MAX_BLOCK_SIZE);
						@in.ReadFully(Buf, 0, span);
						stop = off + span;
						Pos = 0;
					}
					else if (End - Pos < 1)
					{
						v[off++] = Din.ReadBoolean();
						continue;
					}
					else
					{
						stop = System.Math.Min(endoff, off + End - Pos);
					}

					while (off < stop)
					{
						v[off++] = Bits.GetBoolean(Buf, Pos++);
					}
				}
			}

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: void readChars(char[] v, int off, int len) throws IOException
			internal virtual void ReadChars(char[] v, int off, int len)
			{
				int stop , endoff = off + len;
				while (off < endoff)
				{
					if (!Blkmode)
					{
						int span = System.Math.Min(endoff - off, MAX_BLOCK_SIZE >> 1);
						@in.ReadFully(Buf, 0, span << 1);
						stop = off + span;
						Pos = 0;
					}
					else if (End - Pos < 2)
					{
						v[off++] = Din.ReadChar();
						continue;
					}
					else
					{
						stop = System.Math.Min(endoff, off + ((End - Pos) >> 1));
					}

					while (off < stop)
					{
						v[off++] = Bits.GetChar(Buf, Pos);
						Pos += 2;
					}
				}
			}

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: void readShorts(short[] v, int off, int len) throws IOException
			internal virtual void ReadShorts(short[] v, int off, int len)
			{
				int stop , endoff = off + len;
				while (off < endoff)
				{
					if (!Blkmode)
					{
						int span = System.Math.Min(endoff - off, MAX_BLOCK_SIZE >> 1);
						@in.ReadFully(Buf, 0, span << 1);
						stop = off + span;
						Pos = 0;
					}
					else if (End - Pos < 2)
					{
						v[off++] = Din.ReadShort();
						continue;
					}
					else
					{
						stop = System.Math.Min(endoff, off + ((End - Pos) >> 1));
					}

					while (off < stop)
					{
						v[off++] = Bits.GetShort(Buf, Pos);
						Pos += 2;
					}
				}
			}

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: void readInts(int[] v, int off, int len) throws IOException
			internal virtual void ReadInts(int[] v, int off, int len)
			{
				int stop , endoff = off + len;
				while (off < endoff)
				{
					if (!Blkmode)
					{
						int span = System.Math.Min(endoff - off, MAX_BLOCK_SIZE >> 2);
						@in.ReadFully(Buf, 0, span << 2);
						stop = off + span;
						Pos = 0;
					}
					else if (End - Pos < 4)
					{
						v[off++] = Din.ReadInt();
						continue;
					}
					else
					{
						stop = System.Math.Min(endoff, off + ((End - Pos) >> 2));
					}

					while (off < stop)
					{
						v[off++] = Bits.GetInt(Buf, Pos);
						Pos += 4;
					}
				}
			}

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: void readFloats(float[] v, int off, int len) throws IOException
			internal virtual void ReadFloats(float[] v, int off, int len)
			{
				int span , endoff = off + len;
				while (off < endoff)
				{
					if (!Blkmode)
					{
						span = System.Math.Min(endoff - off, MAX_BLOCK_SIZE >> 2);
						@in.ReadFully(Buf, 0, span << 2);
						Pos = 0;
					}
					else if (End - Pos < 4)
					{
						v[off++] = Din.ReadFloat();
						continue;
					}
					else
					{
						span = System.Math.Min(endoff - off, ((End - Pos) >> 2));
					}

					bytesToFloats(Buf, Pos, v, off, span);
					off += span;
					Pos += span << 2;
				}
			}

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: void readLongs(long[] v, int off, int len) throws IOException
			internal virtual void ReadLongs(long[] v, int off, int len)
			{
				int stop , endoff = off + len;
				while (off < endoff)
				{
					if (!Blkmode)
					{
						int span = System.Math.Min(endoff - off, MAX_BLOCK_SIZE >> 3);
						@in.ReadFully(Buf, 0, span << 3);
						stop = off + span;
						Pos = 0;
					}
					else if (End - Pos < 8)
					{
						v[off++] = Din.ReadLong();
						continue;
					}
					else
					{
						stop = System.Math.Min(endoff, off + ((End - Pos) >> 3));
					}

					while (off < stop)
					{
						v[off++] = Bits.GetLong(Buf, Pos);
						Pos += 8;
					}
				}
			}

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: void readDoubles(double[] v, int off, int len) throws IOException
			internal virtual void ReadDoubles(double[] v, int off, int len)
			{
				int span , endoff = off + len;
				while (off < endoff)
				{
					if (!Blkmode)
					{
						span = System.Math.Min(endoff - off, MAX_BLOCK_SIZE >> 3);
						@in.ReadFully(Buf, 0, span << 3);
						Pos = 0;
					}
					else if (End - Pos < 8)
					{
						v[off++] = Din.ReadDouble();
						continue;
					}
					else
					{
						span = System.Math.Min(endoff - off, ((End - Pos) >> 3));
					}

					bytesToDoubles(Buf, Pos, v, off, span);
					off += span;
					Pos += span << 3;
				}
			}

			/// <summary>
			/// Reads in string written in "long" UTF format.  "Long" UTF format is
			/// identical to standard UTF, except that it uses an 8 byte header
			/// (instead of the standard 2 bytes) to convey the UTF encoding length.
			/// </summary>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: String readLongUTF() throws IOException
			internal virtual String ReadLongUTF()
			{
				return ReadUTFBody(ReadLong());
			}

			/// <summary>
			/// Reads in the "body" (i.e., the UTF representation minus the 2-byte
			/// or 8-byte length header) of a UTF encoding, which occupies the next
			/// utflen bytes.
			/// </summary>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: private String readUTFBody(long utflen) throws IOException
			internal virtual String ReadUTFBody(long utflen)
			{
				StringBuilder sbuf = new StringBuilder();
				if (!Blkmode)
				{
					End = Pos = 0;
				}

				while (utflen > 0)
				{
					int avail = End - Pos;
					if (avail >= 3 || (long) avail == utflen)
					{
						utflen -= ReadUTFSpan(sbuf, utflen);
					}
					else
					{
						if (Blkmode)
						{
							// near block boundary, read one byte at a time
							utflen -= ReadUTFChar(sbuf, utflen);
						}
						else
						{
							// shift and refill buffer manually
							if (avail > 0)
							{
								System.Array.Copy(Buf, Pos, Buf, 0, avail);
							}
							Pos = 0;
							End = (int) System.Math.Min(MAX_BLOCK_SIZE, utflen);
							@in.ReadFully(Buf, avail, End - avail);
						}
					}
				}

				return sbuf.ToString();
			}

			/// <summary>
			/// Reads span of UTF-encoded characters out of internal buffer
			/// (starting at offset pos and ending at or before offset end),
			/// consuming no more than utflen bytes.  Appends read characters to
			/// sbuf.  Returns the number of bytes consumed.
			/// </summary>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: private long readUTFSpan(StringBuilder sbuf, long utflen) throws IOException
			internal virtual long ReadUTFSpan(StringBuilder sbuf, long utflen)
			{
				int cpos = 0;
				int start = Pos;
				int avail = System.Math.Min(End - Pos, CHAR_BUF_SIZE);
				// stop short of last char unless all of utf bytes in buffer
				int stop = Pos + ((utflen > avail) ? avail - 2 : (int) utflen);
				bool outOfBounds = false;

				try
				{
					while (Pos < stop)
					{
						int b1, b2, b3;
						b1 = Buf[Pos++] & 0xFF;
						switch (b1 >> 4)
						{
							case 0:
							case 1:
							case 2:
							case 3:
							case 4:
							case 5:
							case 6:
							case 7: // 1 byte format: 0xxxxxxx
								Cbuf[cpos++] = (char) b1;
								break;

							case 12:
							case 13: // 2 byte format: 110xxxxx 10xxxxxx
								b2 = Buf[Pos++];
								if ((b2 & 0xC0) != 0x80)
								{
									throw new UTFDataFormatException();
								}
								Cbuf[cpos++] = (char)(((b1 & 0x1F) << 6) | ((b2 & 0x3F) << 0));
								break;

							case 14: // 3 byte format: 1110xxxx 10xxxxxx 10xxxxxx
								b3 = Buf[Pos + 1];
								b2 = Buf[Pos + 0];
								Pos += 2;
								if ((b2 & 0xC0) != 0x80 || (b3 & 0xC0) != 0x80)
								{
									throw new UTFDataFormatException();
								}
								Cbuf[cpos++] = (char)(((b1 & 0x0F) << 12) | ((b2 & 0x3F) << 6) | ((b3 & 0x3F) << 0));
								break;

							default: // 10xx xxxx, 1111 xxxx
								throw new UTFDataFormatException();
						}
					}
				}
				catch (ArrayIndexOutOfBoundsException)
				{
					outOfBounds = true;
				}
				finally
				{
					if (outOfBounds || (Pos - start) > utflen)
					{
						/*
						 * Fix for 4450867: if a malformed utf char causes the
						 * conversion loop to scan past the expected end of the utf
						 * string, only consume the expected number of utf bytes.
						 */
						Pos = start + (int) utflen;
						throw new UTFDataFormatException();
					}
				}

				sbuf.Append(Cbuf, 0, cpos);
				return Pos - start;
			}

			/// <summary>
			/// Reads in single UTF-encoded character one byte at a time, appends
			/// the character to sbuf, and returns the number of bytes consumed.
			/// This method is used when reading in UTF strings written in block
			/// data mode to handle UTF-encoded characters which (potentially)
			/// straddle block-data boundaries.
			/// </summary>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: private int readUTFChar(StringBuilder sbuf, long utflen) throws IOException
			internal virtual int ReadUTFChar(StringBuilder sbuf, long utflen)
			{
				int b1, b2, b3;
				b1 = ReadByte() & 0xFF;
				switch (b1 >> 4)
				{
					case 0:
					case 1:
					case 2:
					case 3:
					case 4:
					case 5:
					case 6:
					case 7: // 1 byte format: 0xxxxxxx
						sbuf.Append((char) b1);
						return 1;

					case 12:
					case 13: // 2 byte format: 110xxxxx 10xxxxxx
						if (utflen < 2)
						{
							throw new UTFDataFormatException();
						}
						b2 = ReadByte();
						if ((b2 & 0xC0) != 0x80)
						{
							throw new UTFDataFormatException();
						}
						sbuf.Append((char)(((b1 & 0x1F) << 6) | ((b2 & 0x3F) << 0)));
						return 2;

					case 14: // 3 byte format: 1110xxxx 10xxxxxx 10xxxxxx
						if (utflen < 3)
						{
							if (utflen == 2)
							{
								ReadByte(); // consume remaining byte
							}
							throw new UTFDataFormatException();
						}
						b2 = ReadByte();
						b3 = ReadByte();
						if ((b2 & 0xC0) != 0x80 || (b3 & 0xC0) != 0x80)
						{
							throw new UTFDataFormatException();
						}
						sbuf.Append((char)(((b1 & 0x0F) << 12) | ((b2 & 0x3F) << 6) | ((b3 & 0x3F) << 0)));
						return 3;

					default: // 10xx xxxx, 1111 xxxx
						throw new UTFDataFormatException();
				}
			}
		}

		/// <summary>
		/// Unsynchronized table which tracks wire handle to object mappings, as
		/// well as ClassNotFoundExceptions associated with deserialized objects.
		/// This class implements an exception-propagation algorithm for
		/// determining which objects should have ClassNotFoundExceptions associated
		/// with them, taking into account cycles and discontinuities (e.g., skipped
		/// fields) in the object graph.
		/// 
		/// <para>General use of the table is as follows: during deserialization, a
		/// given object is first assigned a handle by calling the assign method.
		/// This method leaves the assigned handle in an "open" state, wherein
		/// dependencies on the exception status of other handles can be registered
		/// by calling the markDependency method, or an exception can be directly
		/// associated with the handle by calling markException.  When a handle is
		/// tagged with an exception, the HandleTable assumes responsibility for
		/// propagating the exception to any other objects which depend
		/// (transitively) on the exception-tagged object.
		/// 
		/// </para>
		/// <para>Once all exception information/dependencies for the handle have been
		/// registered, the handle should be "closed" by calling the finish method
		/// on it.  The act of finishing a handle allows the exception propagation
		/// algorithm to aggressively prune dependency links, lessening the
		/// performance/memory impact of exception tracking.
		/// 
		/// </para>
		/// <para>Note that the exception propagation algorithm used depends on handles
		/// being assigned/finished in LIFO order; however, for simplicity as well
		/// as memory conservation, it does not enforce this constraint.
		/// </para>
		/// </summary>
		// REMIND: add full description of exception propagation algorithm?
		private class HandleTable
		{

			/* status codes indicating whether object has associated exception */
			internal const sbyte STATUS_OK = 1;
			internal const sbyte STATUS_UNKNOWN = 2;
			internal const sbyte STATUS_EXCEPTION = 3;

			/// <summary>
			/// array mapping handle -> object status </summary>
			internal sbyte[] Status;
			/// <summary>
			/// array mapping handle -> object/exception (depending on status) </summary>
			internal Object[] Entries;
			/// <summary>
			/// array mapping handle -> list of dependent handles (if any) </summary>
			internal HandleList[] Deps;
			/// <summary>
			/// lowest unresolved dependency </summary>
			internal int LowDep = -1;
			/// <summary>
			/// number of handles in table </summary>
			internal int Size_Renamed = 0;

			/// <summary>
			/// Creates handle table with the given initial capacity.
			/// </summary>
			internal HandleTable(int initialCapacity)
			{
				Status = new sbyte[initialCapacity];
				Entries = new Object[initialCapacity];
				Deps = new HandleList[initialCapacity];
			}

			/// <summary>
			/// Assigns next available handle to given object, and returns assigned
			/// handle.  Once object has been completely deserialized (and all
			/// dependencies on other objects identified), the handle should be
			/// "closed" by passing it to finish().
			/// </summary>
			internal virtual int Assign(Object obj)
			{
				if (Size_Renamed >= Entries.Length)
				{
					Grow();
				}
				Status[Size_Renamed] = STATUS_UNKNOWN;
				Entries[Size_Renamed] = obj;
				return Size_Renamed++;
			}

			/// <summary>
			/// Registers a dependency (in exception status) of one handle on
			/// another.  The dependent handle must be "open" (i.e., assigned, but
			/// not finished yet).  No action is taken if either dependent or target
			/// handle is NULL_HANDLE.
			/// </summary>
			internal virtual void MarkDependency(int dependent, int target)
			{
				if (dependent == NULL_HANDLE || target == NULL_HANDLE)
				{
					return;
				}
				switch (Status[dependent])
				{

					case STATUS_UNKNOWN:
						switch (Status[target])
						{
							case STATUS_OK:
								// ignore dependencies on objs with no exception
								break;

							case STATUS_EXCEPTION:
								// eagerly propagate exception
								MarkException(dependent, (ClassNotFoundException) Entries[target]);
								break;

							case STATUS_UNKNOWN:
								// add to dependency list of target
								if (Deps[target] == null)
								{
									Deps[target] = new HandleList();
								}
								Deps[target].Add(dependent);

								// remember lowest unresolved target seen
								if (LowDep < 0 || LowDep > target)
								{
									LowDep = target;
								}
								break;

							default:
								throw new InternalError();
						}
						break;

					case STATUS_EXCEPTION:
						break;

					default:
						throw new InternalError();
				}
			}

			/// <summary>
			/// Associates a ClassNotFoundException (if one not already associated)
			/// with the currently active handle and propagates it to other
			/// referencing objects as appropriate.  The specified handle must be
			/// "open" (i.e., assigned, but not finished yet).
			/// </summary>
			internal virtual void MarkException(int handle, ClassNotFoundException ex)
			{
				switch (Status[handle])
				{
					case STATUS_UNKNOWN:
						Status[handle] = STATUS_EXCEPTION;
						Entries[handle] = ex;

						// propagate exception to dependents
						HandleList dlist = Deps[handle];
						if (dlist != null)
						{
							int ndeps = dlist.Size();
							for (int i = 0; i < ndeps; i++)
							{
								MarkException(dlist.Get(i), ex);
							}
							Deps[handle] = null;
						}
						break;

					case STATUS_EXCEPTION:
						break;

					default:
						throw new InternalError();
				}
			}

			/// <summary>
			/// Marks given handle as finished, meaning that no new dependencies
			/// will be marked for handle.  Calls to the assign and finish methods
			/// must occur in LIFO order.
			/// </summary>
			internal virtual void Finish(int handle)
			{
				int end;
				if (LowDep < 0)
				{
					// no pending unknowns, only resolve current handle
					end = handle + 1;
				}
				else if (LowDep >= handle)
				{
					// pending unknowns now clearable, resolve all upward handles
					end = Size_Renamed;
					LowDep = -1;
				}
				else
				{
					// unresolved backrefs present, can't resolve anything yet
					return;
				}

				// change STATUS_UNKNOWN -> STATUS_OK in selected span of handles
				for (int i = handle; i < end; i++)
				{
					switch (Status[i])
					{
						case STATUS_UNKNOWN:
							Status[i] = STATUS_OK;
							Deps[i] = null;
							break;

						case STATUS_OK:
						case STATUS_EXCEPTION:
							break;

						default:
							throw new InternalError();
					}
				}
			}

			/// <summary>
			/// Assigns a new object to the given handle.  The object previously
			/// associated with the handle is forgotten.  This method has no effect
			/// if the given handle already has an exception associated with it.
			/// This method may be called at any time after the handle is assigned.
			/// </summary>
			internal virtual void SetObject(int handle, Object obj)
			{
				switch (Status[handle])
				{
					case STATUS_UNKNOWN:
					case STATUS_OK:
						Entries[handle] = obj;
						break;

					case STATUS_EXCEPTION:
						break;

					default:
						throw new InternalError();
				}
			}

			/// <summary>
			/// Looks up and returns object associated with the given handle.
			/// Returns null if the given handle is NULL_HANDLE, or if it has an
			/// associated ClassNotFoundException.
			/// </summary>
			internal virtual Object LookupObject(int handle)
			{
				return (handle != NULL_HANDLE && Status[handle] != STATUS_EXCEPTION) ? Entries[handle] : null;
			}

			/// <summary>
			/// Looks up and returns ClassNotFoundException associated with the
			/// given handle.  Returns null if the given handle is NULL_HANDLE, or
			/// if there is no ClassNotFoundException associated with the handle.
			/// </summary>
			internal virtual ClassNotFoundException LookupException(int handle)
			{
				return (handle != NULL_HANDLE && Status[handle] == STATUS_EXCEPTION) ? (ClassNotFoundException) Entries[handle] : null;
			}

			/// <summary>
			/// Resets table to its initial state.
			/// </summary>
			internal virtual void Clear()
			{
				Arrays.Fill(Status, 0, Size_Renamed, (sbyte) 0);
				Arrays.Fill(Entries, 0, Size_Renamed, null);
				Arrays.Fill(Deps, 0, Size_Renamed, null);
				LowDep = -1;
				Size_Renamed = 0;
			}

			/// <summary>
			/// Returns number of handles registered in table.
			/// </summary>
			internal virtual int Size()
			{
				return Size_Renamed;
			}

			/// <summary>
			/// Expands capacity of internal arrays.
			/// </summary>
			internal virtual void Grow()
			{
				int newCapacity = (Entries.Length << 1) + 1;

				sbyte[] newStatus = new sbyte[newCapacity];
				Object[] newEntries = new Object[newCapacity];
				HandleList[] newDeps = new HandleList[newCapacity];

				System.Array.Copy(Status, 0, newStatus, 0, Size_Renamed);
				System.Array.Copy(Entries, 0, newEntries, 0, Size_Renamed);
				System.Array.Copy(Deps, 0, newDeps, 0, Size_Renamed);

				Status = newStatus;
				Entries = newEntries;
				Deps = newDeps;
			}

			/// <summary>
			/// Simple growable list of (integer) handles.
			/// </summary>
			private class HandleList
			{
				internal int[] List = new int[4];
				internal int Size_Renamed = 0;

				public HandleList()
				{
				}

				public virtual void Add(int handle)
				{
					if (Size_Renamed >= List.Length)
					{
						int[] newList = new int[List.Length << 1];
						System.Array.Copy(List, 0, newList, 0, List.Length);
						List = newList;
					}
					List[Size_Renamed++] = handle;
				}

				public virtual int Get(int index)
				{
					if (index >= Size_Renamed)
					{
						throw new ArrayIndexOutOfBoundsException();
					}
					return List[index];
				}

				public virtual int Size()
				{
					return Size_Renamed;
				}
			}
		}

		/// <summary>
		/// Method for cloning arrays in case of using unsharing reading
		/// </summary>
		private static Object CloneArray(Object array)
		{
			if (array is Object[])
			{
				return ((Object[]) array).clone();
			}
			else if (array is bool[])
			{
				return ((bool[]) array).clone();
			}
			else if (array is sbyte[])
			{
				return ((sbyte[]) array).clone();
			}
			else if (array is char[])
			{
				return ((char[]) array).clone();
			}
			else if (array is double[])
			{
				return ((double[]) array).clone();
			}
			else if (array is float[])
			{
				return ((float[]) array).clone();
			}
			else if (array is int[])
			{
				return ((int[]) array).clone();
			}
			else if (array is long[])
			{
				return ((long[]) array).clone();
			}
			else if (array is short[])
			{
				return ((short[]) array).clone();
			}
			else
			{
				throw new AssertionError();
			}
		}

	}

}
using System;
using System.Collections.Generic;
using System.Collections.Concurrent;
using System.Threading;
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

	using Unsafe = sun.misc.Unsafe;
	using CallerSensitive = sun.reflect.CallerSensitive;
	using Reflection = sun.reflect.Reflection;
	using ReflectionFactory = sun.reflect.ReflectionFactory;
	using ReflectUtil = sun.reflect.misc.ReflectUtil;

	/// <summary>
	/// Serialization's descriptor for classes.  It contains the name and
	/// serialVersionUID of the class.  The ObjectStreamClass for a specific class
	/// loaded in this Java VM can be found/created using the lookup method.
	/// 
	/// <para>The algorithm to compute the SerialVersionUID is described in
	/// <a href="../../../platform/serialization/spec/class.html#4100">Object
	/// Serialization Specification, Section 4.6, Stream Unique Identifiers</a>.
	/// 
	/// @author      Mike Warres
	/// @author      Roger Riggs
	/// </para>
	/// </summary>
	/// <seealso cref= ObjectStreamField </seealso>
	/// <seealso cref= <a href="../../../platform/serialization/spec/class.html">Object Serialization Specification, Section 4, Class Descriptors</a>
	/// @since   JDK1.1 </seealso>
	[Serializable]
	public class ObjectStreamClass
	{

		/// <summary>
		/// serialPersistentFields value indicating no serializable fields </summary>
		public static readonly ObjectStreamField[] NO_FIELDS = new ObjectStreamField[0];

		private const long SerialVersionUID_Renamed = -6120832682080437368L;
		private static readonly ObjectStreamField[] SerialPersistentFields = NO_FIELDS;

		/// <summary>
		/// reflection factory for obtaining serialization constructors </summary>
		private static readonly ReflectionFactory ReflFactory = AccessController.doPrivileged(new ReflectionFactory.GetReflectionFactoryAction());

		private class Caches
		{
			/// <summary>
			/// cache mapping local classes -> descriptors </summary>
//JAVA TO C# CONVERTER TODO TASK: Java wildcard generics are not converted to .NET:
//ORIGINAL LINE: static final java.util.concurrent.ConcurrentMap<WeakClassKey,Reference<?>> localDescs = new java.util.concurrent.ConcurrentHashMap<>();
			internal static readonly ConcurrentMap<WeakClassKey, Reference<?>> LocalDescs = new ConcurrentDictionary<WeakClassKey, Reference<?>>();

			/// <summary>
			/// cache mapping field group/local desc pairs -> field reflectors </summary>
//JAVA TO C# CONVERTER TODO TASK: Java wildcard generics are not converted to .NET:
//ORIGINAL LINE: static final java.util.concurrent.ConcurrentMap<FieldReflectorKey,Reference<?>> reflectors = new java.util.concurrent.ConcurrentHashMap<>();
			internal static readonly ConcurrentMap<FieldReflectorKey, Reference<?>> Reflectors = new ConcurrentDictionary<FieldReflectorKey, Reference<?>>();

			/// <summary>
			/// queue for WeakReferences to local classes </summary>
			internal static readonly ReferenceQueue<Class> LocalDescsQueue = new ReferenceQueue<Class>();
			/// <summary>
			/// queue for WeakReferences to field reflectors keys </summary>
			internal static readonly ReferenceQueue<Class> ReflectorsQueue = new ReferenceQueue<Class>();
		}

		/// <summary>
		/// class associated with this descriptor (if any) </summary>
		private Class Cl;
		/// <summary>
		/// name of class represented by this descriptor </summary>
		private String Name_Renamed;
		/// <summary>
		/// serialVersionUID of represented class (null if not computed yet) </summary>
		private volatile Long Suid;

		/// <summary>
		/// true if represents dynamic proxy class </summary>
		private bool IsProxy;
		/// <summary>
		/// true if represents enum type </summary>
		private bool IsEnum;
		/// <summary>
		/// true if represented class implements Serializable </summary>
		private bool Serializable_Renamed;
		/// <summary>
		/// true if represented class implements Externalizable </summary>
		private bool Externalizable_Renamed;
		/// <summary>
		/// true if desc has data written by class-defined writeObject method </summary>
		private bool HasWriteObjectData_Renamed;
		/// <summary>
		/// true if desc has externalizable data written in block data format; this
		/// must be true by default to accommodate ObjectInputStream subclasses which
		/// override readClassDescriptor() to return class descriptors obtained from
		/// ObjectStreamClass.lookup() (see 4461737)
		/// </summary>
		private bool HasBlockExternalData_Renamed = true;

		/// <summary>
		/// Contains information about InvalidClassException instances to be thrown
		/// when attempting operations on an invalid class. Note that instances of
		/// this class are immutable and are potentially shared among
		/// ObjectStreamClass instances.
		/// </summary>
		private class ExceptionInfo
		{
			internal readonly String ClassName;
			internal readonly String Message;

			internal ExceptionInfo(String cn, String msg)
			{
				ClassName = cn;
				Message = msg;
			}

			/// <summary>
			/// Returns (does not throw) an InvalidClassException instance created
			/// from the information in this object, suitable for being thrown by
			/// the caller.
			/// </summary>
			internal virtual InvalidClassException NewInvalidClassException()
			{
				return new InvalidClassException(ClassName, Message);
			}
		}

		/// <summary>
		/// exception (if any) thrown while attempting to resolve class </summary>
		private ClassNotFoundException ResolveEx;
		/// <summary>
		/// exception (if any) to throw if non-enum deserialization attempted </summary>
		private ExceptionInfo DeserializeEx;
		/// <summary>
		/// exception (if any) to throw if non-enum serialization attempted </summary>
		private ExceptionInfo SerializeEx;
		/// <summary>
		/// exception (if any) to throw if default serialization attempted </summary>
		private ExceptionInfo DefaultSerializeEx;

		/// <summary>
		/// serializable fields </summary>
		private ObjectStreamField[] Fields_Renamed;
		/// <summary>
		/// aggregate marshalled size of primitive fields </summary>
		private int PrimDataSize_Renamed;
		/// <summary>
		/// number of non-primitive fields </summary>
		private int NumObjFields_Renamed;
		/// <summary>
		/// reflector for setting/getting serializable field values </summary>
		private FieldReflector FieldRefl;
		/// <summary>
		/// data layout of serialized objects described by this class desc </summary>
		private volatile ClassDataSlot[] DataLayout;

		/// <summary>
		/// serialization-appropriate constructor, or null if none </summary>
//JAVA TO C# CONVERTER TODO TASK: Java wildcard generics are not converted to .NET:
//ORIGINAL LINE: private Constructor<?> cons;
		private Constructor<?> Cons;
		/// <summary>
		/// class-defined writeObject method, or null if none </summary>
		private Method WriteObjectMethod;
		/// <summary>
		/// class-defined readObject method, or null if none </summary>
		private Method ReadObjectMethod;
		/// <summary>
		/// class-defined readObjectNoData method, or null if none </summary>
		private Method ReadObjectNoDataMethod;
		/// <summary>
		/// class-defined writeReplace method, or null if none </summary>
		private Method WriteReplaceMethod;
		/// <summary>
		/// class-defined readResolve method, or null if none </summary>
		private Method ReadResolveMethod;

		/// <summary>
		/// local class descriptor for represented class (may point to self) </summary>
		private ObjectStreamClass LocalDesc_Renamed;
		/// <summary>
		/// superclass descriptor appearing in stream </summary>
		private ObjectStreamClass SuperDesc_Renamed;

		/// <summary>
		/// true if, and only if, the object has been correctly initialized </summary>
		private bool Initialized;

		/// <summary>
		/// Initializes native code.
		/// </summary>
//JAVA TO C# CONVERTER TODO TASK: Replace 'unknown' with the appropriate dll name:
		[DllImport("unknown")]
		private static extern void initNative();
		static ObjectStreamClass()
		{
			initNative();
		}

		/// <summary>
		/// Find the descriptor for a class that can be serialized.  Creates an
		/// ObjectStreamClass instance if one does not exist yet for class. Null is
		/// returned if the specified class does not implement java.io.Serializable
		/// or java.io.Externalizable.
		/// </summary>
		/// <param name="cl"> class for which to get the descriptor </param>
		/// <returns>  the class descriptor for the specified class </returns>
		public static ObjectStreamClass Lookup(Class cl)
		{
			return Lookup(cl, false);
		}

		/// <summary>
		/// Returns the descriptor for any class, regardless of whether it
		/// implements <seealso cref="Serializable"/>.
		/// </summary>
		/// <param name="cl"> class for which to get the descriptor </param>
		/// <returns>       the class descriptor for the specified class
		/// @since 1.6 </returns>
		public static ObjectStreamClass LookupAny(Class cl)
		{
			return Lookup(cl, true);
		}

		/// <summary>
		/// Returns the name of the class described by this descriptor.
		/// This method returns the name of the class in the format that
		/// is used by the <seealso cref="Class#getName"/> method.
		/// </summary>
		/// <returns> a string representing the name of the class </returns>
		public virtual String Name
		{
			get
			{
				return Name_Renamed;
			}
		}

		/// <summary>
		/// Return the serialVersionUID for this class.  The serialVersionUID
		/// defines a set of classes all with the same name that have evolved from a
		/// common root class and agree to be serialized and deserialized using a
		/// common format.  NonSerializable classes have a serialVersionUID of 0L.
		/// </summary>
		/// <returns>  the SUID of the class described by this descriptor </returns>
		public virtual long SerialVersionUID
		{
			get
			{
				// REMIND: synchronize instead of relying on volatile?
				if (Suid == null)
				{
					Suid = AccessController.doPrivileged(new PrivilegedActionAnonymousInnerClassHelper(this)
				   );
				}
				return Suid.LongValue();
			}
		}

		private class PrivilegedActionAnonymousInnerClassHelper : PrivilegedAction<Long>
		{
			private readonly ObjectStreamClass OuterInstance;

			public PrivilegedActionAnonymousInnerClassHelper(ObjectStreamClass outerInstance)
			{
				this.OuterInstance = outerInstance;
			}

			public virtual Long Run()
			{
				return ComputeDefaultSUID(OuterInstance.Cl);
			}
		}

		/// <summary>
		/// Return the class in the local VM that this version is mapped to.  Null
		/// is returned if there is no corresponding local class.
		/// </summary>
		/// <returns>  the <code>Class</code> instance that this descriptor represents </returns>
//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @CallerSensitive public Class forClass()
		public virtual Class ForClass()
		{
			if (Cl == null)
			{
				return null;
			}
			RequireInitialized();
			if (System.SecurityManager != null)
			{
				Class caller = Reflection.CallerClass;
				if (ReflectUtil.needsPackageAccessCheck(caller.ClassLoader, Cl.ClassLoader))
				{
					ReflectUtil.checkPackageAccess(Cl);
				}
			}
			return Cl;
		}

		/// <summary>
		/// Return an array of the fields of this serializable class.
		/// </summary>
		/// <returns>  an array containing an element for each persistent field of
		///          this class. Returns an array of length zero if there are no
		///          fields.
		/// @since 1.2 </returns>
		public virtual ObjectStreamField[] Fields
		{
			get
			{
				return GetFields(true);
			}
		}

		/// <summary>
		/// Get the field of this class by name.
		/// </summary>
		/// <param name="name"> the name of the data field to look for </param>
		/// <returns>  The ObjectStreamField object of the named field or null if
		///          there is no such named field. </returns>
		public virtual ObjectStreamField GetField(String name)
		{
			return GetField(name, null);
		}

		/// <summary>
		/// Return a string describing this ObjectStreamClass.
		/// </summary>
		public override String ToString()
		{
			return Name_Renamed + ": static final long serialVersionUID = " + SerialVersionUID + "L;";
		}

		/// <summary>
		/// Looks up and returns class descriptor for given class, or null if class
		/// is non-serializable and "all" is set to false.
		/// </summary>
		/// <param name="cl"> class to look up </param>
		/// <param name="all"> if true, return descriptors for all classes; if false, only
		///          return descriptors for serializable classes </param>
		internal static ObjectStreamClass Lookup(Class cl, bool all)
		{
			if (!(all || cl.IsSubclassOf(typeof(Serializable))))
			{
				return null;
			}
			ProcessQueue(Caches.LocalDescsQueue, Caches.LocalDescs);
			WeakClassKey key = new WeakClassKey(cl, Caches.LocalDescsQueue);
//JAVA TO C# CONVERTER TODO TASK: Java wildcard generics are not converted to .NET:
//ORIGINAL LINE: Reference<?> ref = Caches.localDescs.get(key);
			Reference<?> @ref = Caches.LocalDescs[key];
			Object entry = null;
			if (@ref != null)
			{
				entry = @ref.get();
			}
			EntryFuture future = null;
			if (entry == null)
			{
				EntryFuture newEntry = new EntryFuture();
//JAVA TO C# CONVERTER TODO TASK: Java wildcard generics are not converted to .NET:
//ORIGINAL LINE: Reference<?> newRef = new SoftReference<>(newEntry);
				Reference<?> newRef = new SoftReference<?>(newEntry);
				do
				{
					if (@ref != null)
					{
						Caches.LocalDescs.Remove(key, @ref);
					}
					@ref = Caches.LocalDescs.PutIfAbsent(key, newRef);
					if (@ref != null)
					{
						entry = @ref.get();
					}
				} while (@ref != null && entry == null);
				if (entry == null)
				{
					future = newEntry;
				}
			}

			if (entry is ObjectStreamClass) // check common case first
			{
				return (ObjectStreamClass) entry;
			}
			if (entry is EntryFuture)
			{
				future = (EntryFuture) entry;
				if (future.Owner == Thread.CurrentThread)
				{
					/*
					 * Handle nested call situation described by 4803747: waiting
					 * for future value to be set by a lookup() call further up the
					 * stack will result in deadlock, so calculate and set the
					 * future value here instead.
					 */
					entry = null;
				}
				else
				{
					entry = future.Get();
				}
			}
			if (entry == null)
			{
				try
				{
					entry = new ObjectStreamClass(cl);
				}
				catch (Throwable th)
				{
					entry = th;
				}
				if (future.Set(entry))
				{
					Caches.LocalDescs[key] = new SoftReference<Object>(entry);
				}
				else
				{
					// nested lookup call already set future
					entry = future.Get();
				}
			}

			if (entry is ObjectStreamClass)
			{
				return (ObjectStreamClass) entry;
			}
			else if (entry is RuntimeException)
			{
				throw (RuntimeException) entry;
			}
			else if (entry is Error)
			{
				throw (Error) entry;
			}
			else
			{
				throw new InternalError("unexpected entry: " + entry);
			}
		}

		/// <summary>
		/// Placeholder used in class descriptor and field reflector lookup tables
		/// for an entry in the process of being initialized.  (Internal) callers
		/// which receive an EntryFuture belonging to another thread as the result
		/// of a lookup should call the get() method of the EntryFuture; this will
		/// return the actual entry once it is ready for use and has been set().  To
		/// conserve objects, EntryFutures synchronize on themselves.
		/// </summary>
		private class EntryFuture
		{

			internal static readonly Object Unset = new Object();
			internal readonly Thread Owner_Renamed = Thread.CurrentThread;
			internal Object Entry = Unset;

			/// <summary>
			/// Attempts to set the value contained by this EntryFuture.  If the
			/// EntryFuture's value has not been set already, then the value is
			/// saved, any callers blocked in the get() method are notified, and
			/// true is returned.  If the value has already been set, then no saving
			/// or notification occurs, and false is returned.
			/// </summary>
			internal virtual bool Set(Object entry)
			{
				lock (this)
				{
					if (this.Entry != Unset)
					{
						return false;
					}
					this.Entry = entry;
					Monitor.PulseAll(this);
					return true;
				}
			}

			/// <summary>
			/// Returns the value contained by this EntryFuture, blocking if
			/// necessary until a value is set.
			/// </summary>
			internal virtual Object Get()
			{
				lock (this)
				{
					bool interrupted = false;
					while (Entry == Unset)
					{
						try
						{
							Monitor.Wait(this);
						}
						catch (InterruptedException)
						{
							interrupted = true;
						}
					}
					if (interrupted)
					{
						AccessController.doPrivileged(new PrivilegedActionAnonymousInnerClassHelper2(this)
					   );
					}
					return Entry;
				}
			}

			private class PrivilegedActionAnonymousInnerClassHelper2 : PrivilegedAction<Void>
			{
				private readonly EntryFuture OuterInstance;

				public PrivilegedActionAnonymousInnerClassHelper2(EntryFuture outerInstance)
				{
					this.OuterInstance = outerInstance;
				}

				public virtual Void Run()
				{
					Thread.CurrentThread.Interrupt();
					return null;
				}
			}

			/// <summary>
			/// Returns the thread that created this EntryFuture.
			/// </summary>
			internal virtual Thread Owner
			{
				get
				{
					return Owner_Renamed;
				}
			}
		}

		/// <summary>
		/// Creates local class descriptor representing given class.
		/// </summary>
//JAVA TO C# CONVERTER WARNING: 'final' parameters are not allowed in .NET:
//ORIGINAL LINE: private ObjectStreamClass(final Class cl)
		private ObjectStreamClass(Class cl)
		{
			this.Cl = cl;
			Name_Renamed = cl.Name;
			IsProxy = Proxy.isProxyClass(cl);
			IsEnum = cl.IsSubclassOf(typeof(Enum));
			Serializable_Renamed = cl.IsSubclassOf(typeof(Serializable));
			Externalizable_Renamed = cl.IsSubclassOf(typeof(Externalizable));

			Class superCl = cl.BaseType;
			SuperDesc_Renamed = (superCl != null) ? Lookup(superCl, false) : null;
			LocalDesc_Renamed = this;

			if (Serializable_Renamed)
			{
				AccessController.doPrivileged(new PrivilegedActionAnonymousInnerClassHelper(this, cl));
			}
			else
			{
				Suid = Convert.ToInt64(0);
				Fields_Renamed = NO_FIELDS;
			}

			try
			{
				FieldRefl = GetReflector(Fields_Renamed, this);
			}
			catch (InvalidClassException ex)
			{
				// field mismatches impossible when matching local fields vs. self
				throw new InternalError(ex);
			}

			if (DeserializeEx == null)
			{
				if (IsEnum)
				{
					DeserializeEx = new ExceptionInfo(Name_Renamed, "enum type");
				}
				else if (Cons == null)
				{
					DeserializeEx = new ExceptionInfo(Name_Renamed, "no valid constructor");
				}
			}
			for (int i = 0; i < Fields_Renamed.Length; i++)
			{
				if (Fields_Renamed[i].Field == null)
				{
					DefaultSerializeEx = new ExceptionInfo(Name_Renamed, "unmatched serializable field(s) declared");
				}
			}
			Initialized = true;
		}

		private class PrivilegedActionAnonymousInnerClassHelper : PrivilegedAction<Void>
		{
			private readonly ObjectStreamClass OuterInstance;

			private Type Cl;

			public PrivilegedActionAnonymousInnerClassHelper(ObjectStreamClass outerInstance, Type cl)
			{
				this.OuterInstance = outerInstance;
				this.Cl = cl;
			}

			public virtual Void Run()
			{
				if (OuterInstance.IsEnum)
				{
					OuterInstance.Suid = Convert.ToInt64(0);
					OuterInstance.Fields_Renamed = NO_FIELDS;
					return null;
				}
				if (Cl.Array)
				{
					OuterInstance.Fields_Renamed = NO_FIELDS;
					return null;
				}

				OuterInstance.Suid = GetDeclaredSUID(Cl);
				try
				{
					OuterInstance.Fields_Renamed = GetSerialFields(Cl);
					outerInstance.ComputeFieldOffsets();
				}
				catch (InvalidClassException e)
				{
					OuterInstance.SerializeEx = OuterInstance.DeserializeEx = new ExceptionInfo(e.Classname, e.Message);
					OuterInstance.Fields_Renamed = NO_FIELDS;
				}

				if (OuterInstance.Externalizable_Renamed)
				{
					OuterInstance.Cons = GetExternalizableConstructor(Cl);
				}
				else
				{
					OuterInstance.Cons = GetSerializableConstructor(Cl);
					OuterInstance.WriteObjectMethod = GetPrivateMethod(Cl, "writeObject", new Class[] {typeof(ObjectOutputStream)}, Void.TYPE);
					OuterInstance.ReadObjectMethod = GetPrivateMethod(Cl, "readObject", new Class[] {typeof(ObjectInputStream)}, Void.TYPE);
					OuterInstance.ReadObjectNoDataMethod = GetPrivateMethod(Cl, "readObjectNoData", null, Void.TYPE);
					OuterInstance.HasWriteObjectData_Renamed = (OuterInstance.WriteObjectMethod != null);
				}
				OuterInstance.WriteReplaceMethod = GetInheritableMethod(Cl, "writeReplace", null, typeof(Object));
				OuterInstance.ReadResolveMethod = GetInheritableMethod(Cl, "readResolve", null, typeof(Object));
				return null;
			}
		}

		/// <summary>
		/// Creates blank class descriptor which should be initialized via a
		/// subsequent call to initProxy(), initNonProxy() or readNonProxy().
		/// </summary>
		internal ObjectStreamClass()
		{
		}

		/// <summary>
		/// Initializes class descriptor representing a proxy class.
		/// </summary>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: void initProxy(Class cl, ClassNotFoundException resolveEx, ObjectStreamClass superDesc) throws InvalidClassException
		internal virtual void InitProxy(Class cl, ClassNotFoundException resolveEx, ObjectStreamClass superDesc)
		{
			ObjectStreamClass osc = null;
			if (cl != null)
			{
				osc = Lookup(cl, true);
				if (!osc.IsProxy)
				{
					throw new InvalidClassException("cannot bind proxy descriptor to a non-proxy class");
				}
			}
			this.Cl = cl;
			this.ResolveEx = resolveEx;
			this.SuperDesc_Renamed = superDesc;
			IsProxy = true;
			Serializable_Renamed = true;
			Suid = Convert.ToInt64(0);
			Fields_Renamed = NO_FIELDS;
			if (osc != null)
			{
				LocalDesc_Renamed = osc;
				Name_Renamed = LocalDesc_Renamed.Name_Renamed;
				Externalizable_Renamed = LocalDesc_Renamed.Externalizable_Renamed;
				WriteReplaceMethod = LocalDesc_Renamed.WriteReplaceMethod;
				ReadResolveMethod = LocalDesc_Renamed.ReadResolveMethod;
				DeserializeEx = LocalDesc_Renamed.DeserializeEx;
				Cons = LocalDesc_Renamed.Cons;
			}
			FieldRefl = GetReflector(Fields_Renamed, LocalDesc_Renamed);
			Initialized = true;
		}

		/// <summary>
		/// Initializes class descriptor representing a non-proxy class.
		/// </summary>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: void initNonProxy(ObjectStreamClass model, Class cl, ClassNotFoundException resolveEx, ObjectStreamClass superDesc) throws InvalidClassException
		internal virtual void InitNonProxy(ObjectStreamClass model, Class cl, ClassNotFoundException resolveEx, ObjectStreamClass superDesc)
		{
			long suid = Convert.ToInt64(model.SerialVersionUID);
			ObjectStreamClass osc = null;
			if (cl != null)
			{
				osc = Lookup(cl, true);
				if (osc.IsProxy)
				{
					throw new InvalidClassException("cannot bind non-proxy descriptor to a proxy class");
				}
				if (model.IsEnum != osc.IsEnum)
				{
					throw new InvalidClassException(model.IsEnum ? "cannot bind enum descriptor to a non-enum class" : "cannot bind non-enum descriptor to an enum class");
				}

				if (model.Serializable_Renamed == osc.Serializable_Renamed && !cl.Array && suid != osc.SerialVersionUID)
				{
					throw new InvalidClassException(osc.Name_Renamed, "local class incompatible: " + "stream classdesc serialVersionUID = " + suid + ", local class serialVersionUID = " + osc.SerialVersionUID);
				}

				if (!ClassNamesEqual(model.Name_Renamed, osc.Name_Renamed))
				{
					throw new InvalidClassException(osc.Name_Renamed, "local class name incompatible with stream class " + "name \"" + model.Name_Renamed + "\"");
				}

				if (!model.IsEnum)
				{
					if ((model.Serializable_Renamed == osc.Serializable_Renamed) && (model.Externalizable_Renamed != osc.Externalizable_Renamed))
					{
						throw new InvalidClassException(osc.Name_Renamed, "Serializable incompatible with Externalizable");
					}

					if ((model.Serializable_Renamed != osc.Serializable_Renamed) || (model.Externalizable_Renamed != osc.Externalizable_Renamed) || !(model.Serializable_Renamed || model.Externalizable_Renamed))
					{
						DeserializeEx = new ExceptionInfo(osc.Name_Renamed, "class invalid for deserialization");
					}
				}
			}

			this.Cl = cl;
			this.ResolveEx = resolveEx;
			this.SuperDesc_Renamed = superDesc;
			Name_Renamed = model.Name_Renamed;
			this.Suid = suid;
			IsProxy = false;
			IsEnum = model.IsEnum;
			Serializable_Renamed = model.Serializable_Renamed;
			Externalizable_Renamed = model.Externalizable_Renamed;
			HasBlockExternalData_Renamed = model.HasBlockExternalData_Renamed;
			HasWriteObjectData_Renamed = model.HasWriteObjectData_Renamed;
			Fields_Renamed = model.Fields_Renamed;
			PrimDataSize_Renamed = model.PrimDataSize_Renamed;
			NumObjFields_Renamed = model.NumObjFields_Renamed;

			if (osc != null)
			{
				LocalDesc_Renamed = osc;
				WriteObjectMethod = LocalDesc_Renamed.WriteObjectMethod;
				ReadObjectMethod = LocalDesc_Renamed.ReadObjectMethod;
				ReadObjectNoDataMethod = LocalDesc_Renamed.ReadObjectNoDataMethod;
				WriteReplaceMethod = LocalDesc_Renamed.WriteReplaceMethod;
				ReadResolveMethod = LocalDesc_Renamed.ReadResolveMethod;
				if (DeserializeEx == null)
				{
					DeserializeEx = LocalDesc_Renamed.DeserializeEx;
				}
				Cons = LocalDesc_Renamed.Cons;
			}

			FieldRefl = GetReflector(Fields_Renamed, LocalDesc_Renamed);
			// reassign to matched fields so as to reflect local unshared settings
			Fields_Renamed = FieldRefl.Fields;
			Initialized = true;
		}

		/// <summary>
		/// Reads non-proxy class descriptor information from given input stream.
		/// The resulting class descriptor is not fully functional; it can only be
		/// used as input to the ObjectInputStream.resolveClass() and
		/// ObjectStreamClass.initNonProxy() methods.
		/// </summary>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: void readNonProxy(ObjectInputStream in) throws IOException, ClassNotFoundException
		internal virtual void ReadNonProxy(ObjectInputStream @in)
		{
			Name_Renamed = @in.ReadUTF();
			Suid = Convert.ToInt64(@in.ReadLong());
			IsProxy = false;

			sbyte flags = @in.ReadByte();
			HasWriteObjectData_Renamed = ((flags & ObjectStreamConstants_Fields.SC_WRITE_METHOD) != 0);
			HasBlockExternalData_Renamed = ((flags & ObjectStreamConstants_Fields.SC_BLOCK_DATA) != 0);
			Externalizable_Renamed = ((flags & ObjectStreamConstants_Fields.SC_EXTERNALIZABLE) != 0);
			bool sflag = ((flags & ObjectStreamConstants_Fields.SC_SERIALIZABLE) != 0);
			if (Externalizable_Renamed && sflag)
			{
				throw new InvalidClassException(Name_Renamed, "serializable and externalizable flags conflict");
			}
			Serializable_Renamed = Externalizable_Renamed || sflag;
			IsEnum = ((flags & ObjectStreamConstants_Fields.SC_ENUM) != 0);
			if (IsEnum && Suid.LongValue() != 0L)
			{
				throw new InvalidClassException(Name_Renamed, "enum descriptor has non-zero serialVersionUID: " + Suid);
			}

			int numFields = @in.ReadShort();
			if (IsEnum && numFields != 0)
			{
				throw new InvalidClassException(Name_Renamed, "enum descriptor has non-zero field count: " + numFields);
			}
			Fields_Renamed = (numFields > 0) ? new ObjectStreamField[numFields] : NO_FIELDS;
			for (int i = 0; i < numFields; i++)
			{
				char tcode = (char) @in.ReadByte();
				String fname = @in.ReadUTF();
				String signature = ((tcode == 'L') || (tcode == '[')) ? @in.ReadTypeString() : new String(new char[] {tcode});
				try
				{
					Fields_Renamed[i] = new ObjectStreamField(fname, signature, false);
				}
				catch (RuntimeException e)
				{
					throw (IOException) (new InvalidClassException(Name_Renamed, "invalid descriptor for field " + fname)).InitCause(e);
				}
			}
			ComputeFieldOffsets();
		}

		/// <summary>
		/// Writes non-proxy class descriptor information to given output stream.
		/// </summary>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: void writeNonProxy(ObjectOutputStream out) throws IOException
		internal virtual void WriteNonProxy(ObjectOutputStream @out)
		{
			@out.WriteUTF(Name_Renamed);
			@out.WriteLong(SerialVersionUID);

			sbyte flags = 0;
			if (Externalizable_Renamed)
			{
				flags |= ObjectStreamConstants_Fields.SC_EXTERNALIZABLE;
				int protocol = @out.ProtocolVersion;
				if (protocol != ObjectStreamConstants_Fields.PROTOCOL_VERSION_1)
				{
					flags |= ObjectStreamConstants_Fields.SC_BLOCK_DATA;
				}
			}
			else if (Serializable_Renamed)
			{
				flags |= ObjectStreamConstants_Fields.SC_SERIALIZABLE;
			}
			if (HasWriteObjectData_Renamed)
			{
				flags |= ObjectStreamConstants_Fields.SC_WRITE_METHOD;
			}
			if (IsEnum)
			{
				flags |= ObjectStreamConstants_Fields.SC_ENUM;
			}
			@out.WriteByte(flags);

			@out.WriteShort(Fields_Renamed.Length);
			for (int i = 0; i < Fields_Renamed.Length; i++)
			{
				ObjectStreamField f = Fields_Renamed[i];
				@out.WriteByte(f.TypeCode);
				@out.WriteUTF(f.Name);
				if (!f.Primitive)
				{
					@out.WriteTypeString(f.TypeString);
				}
			}
		}

		/// <summary>
		/// Returns ClassNotFoundException (if any) thrown while attempting to
		/// resolve local class corresponding to this class descriptor.
		/// </summary>
		internal virtual ClassNotFoundException ResolveException
		{
			get
			{
				return ResolveEx;
			}
		}

		/// <summary>
		/// Throws InternalError if not initialized.
		/// </summary>
		private void RequireInitialized()
		{
			if (!Initialized)
			{
				throw new InternalError("Unexpected call when not initialized");
			}
		}

		/// <summary>
		/// Throws an InvalidClassException if object instances referencing this
		/// class descriptor should not be allowed to deserialize.  This method does
		/// not apply to deserialization of enum constants.
		/// </summary>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: void checkDeserialize() throws InvalidClassException
		internal virtual void CheckDeserialize()
		{
			RequireInitialized();
			if (DeserializeEx != null)
			{
				throw DeserializeEx.NewInvalidClassException();
			}
		}

		/// <summary>
		/// Throws an InvalidClassException if objects whose class is represented by
		/// this descriptor should not be allowed to serialize.  This method does
		/// not apply to serialization of enum constants.
		/// </summary>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: void checkSerialize() throws InvalidClassException
		internal virtual void CheckSerialize()
		{
			RequireInitialized();
			if (SerializeEx != null)
			{
				throw SerializeEx.NewInvalidClassException();
			}
		}

		/// <summary>
		/// Throws an InvalidClassException if objects whose class is represented by
		/// this descriptor should not be permitted to use default serialization
		/// (e.g., if the class declares serializable fields that do not correspond
		/// to actual fields, and hence must use the GetField API).  This method
		/// does not apply to deserialization of enum constants.
		/// </summary>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: void checkDefaultSerialize() throws InvalidClassException
		internal virtual void CheckDefaultSerialize()
		{
			RequireInitialized();
			if (DefaultSerializeEx != null)
			{
				throw DefaultSerializeEx.NewInvalidClassException();
			}
		}

		/// <summary>
		/// Returns superclass descriptor.  Note that on the receiving side, the
		/// superclass descriptor may be bound to a class that is not a superclass
		/// of the subclass descriptor's bound class.
		/// </summary>
		internal virtual ObjectStreamClass SuperDesc
		{
			get
			{
				RequireInitialized();
				return SuperDesc_Renamed;
			}
		}

		/// <summary>
		/// Returns the "local" class descriptor for the class associated with this
		/// class descriptor (i.e., the result of
		/// ObjectStreamClass.lookup(this.forClass())) or null if there is no class
		/// associated with this descriptor.
		/// </summary>
		internal virtual ObjectStreamClass LocalDesc
		{
			get
			{
				RequireInitialized();
				return LocalDesc_Renamed;
			}
		}

		/// <summary>
		/// Returns arrays of ObjectStreamFields representing the serializable
		/// fields of the represented class.  If copy is true, a clone of this class
		/// descriptor's field array is returned, otherwise the array itself is
		/// returned.
		/// </summary>
		internal virtual ObjectStreamField[] GetFields(bool copy)
		{
			return copy ? Fields_Renamed.clone() : Fields_Renamed;
		}

		/// <summary>
		/// Looks up a serializable field of the represented class by name and type.
		/// A specified type of null matches all types, Object.class matches all
		/// non-primitive types, and any other non-null type matches assignable
		/// types only.  Returns matching field, or null if no match found.
		/// </summary>
		internal virtual ObjectStreamField GetField(String name, Class type)
		{
			for (int i = 0; i < Fields_Renamed.Length; i++)
			{
				ObjectStreamField f = Fields_Renamed[i];
				if (f.Name.Equals(name))
				{
					if (type == null || (type == typeof(Object) && !f.Primitive))
					{
						return f;
					}
					Class ftype = f.Type;
					if (ftype != null && ftype.IsSubclassOf(type))
					{
						return f;
					}
				}
			}
			return null;
		}

		/// <summary>
		/// Returns true if class descriptor represents a dynamic proxy class, false
		/// otherwise.
		/// </summary>
		internal virtual bool Proxy
		{
			get
			{
				RequireInitialized();
				return IsProxy;
			}
		}

		/// <summary>
		/// Returns true if class descriptor represents an enum type, false
		/// otherwise.
		/// </summary>
		internal virtual bool Enum
		{
			get
			{
				RequireInitialized();
				return IsEnum;
			}
		}

		/// <summary>
		/// Returns true if represented class implements Externalizable, false
		/// otherwise.
		/// </summary>
		internal virtual bool Externalizable
		{
			get
			{
				RequireInitialized();
				return Externalizable_Renamed;
			}
		}

		/// <summary>
		/// Returns true if represented class implements Serializable, false
		/// otherwise.
		/// </summary>
		internal virtual bool Serializable
		{
			get
			{
				RequireInitialized();
				return Serializable_Renamed;
			}
		}

		/// <summary>
		/// Returns true if class descriptor represents externalizable class that
		/// has written its data in 1.2 (block data) format, false otherwise.
		/// </summary>
		internal virtual bool HasBlockExternalData()
		{
			RequireInitialized();
			return HasBlockExternalData_Renamed;
		}

		/// <summary>
		/// Returns true if class descriptor represents serializable (but not
		/// externalizable) class which has written its data via a custom
		/// writeObject() method, false otherwise.
		/// </summary>
		internal virtual bool HasWriteObjectData()
		{
			RequireInitialized();
			return HasWriteObjectData_Renamed;
		}

		/// <summary>
		/// Returns true if represented class is serializable/externalizable and can
		/// be instantiated by the serialization runtime--i.e., if it is
		/// externalizable and defines a public no-arg constructor, or if it is
		/// non-externalizable and its first non-serializable superclass defines an
		/// accessible no-arg constructor.  Otherwise, returns false.
		/// </summary>
		internal virtual bool Instantiable
		{
			get
			{
				RequireInitialized();
				return (Cons != null);
			}
		}

		/// <summary>
		/// Returns true if represented class is serializable (but not
		/// externalizable) and defines a conformant writeObject method.  Otherwise,
		/// returns false.
		/// </summary>
		internal virtual bool HasWriteObjectMethod()
		{
			RequireInitialized();
			return (WriteObjectMethod != null);
		}

		/// <summary>
		/// Returns true if represented class is serializable (but not
		/// externalizable) and defines a conformant readObject method.  Otherwise,
		/// returns false.
		/// </summary>
		internal virtual bool HasReadObjectMethod()
		{
			RequireInitialized();
			return (ReadObjectMethod != null);
		}

		/// <summary>
		/// Returns true if represented class is serializable (but not
		/// externalizable) and defines a conformant readObjectNoData method.
		/// Otherwise, returns false.
		/// </summary>
		internal virtual bool HasReadObjectNoDataMethod()
		{
			RequireInitialized();
			return (ReadObjectNoDataMethod != null);
		}

		/// <summary>
		/// Returns true if represented class is serializable or externalizable and
		/// defines a conformant writeReplace method.  Otherwise, returns false.
		/// </summary>
		internal virtual bool HasWriteReplaceMethod()
		{
			RequireInitialized();
			return (WriteReplaceMethod != null);
		}

		/// <summary>
		/// Returns true if represented class is serializable or externalizable and
		/// defines a conformant readResolve method.  Otherwise, returns false.
		/// </summary>
		internal virtual bool HasReadResolveMethod()
		{
			RequireInitialized();
			return (ReadResolveMethod != null);
		}

		/// <summary>
		/// Creates a new instance of the represented class.  If the class is
		/// externalizable, invokes its public no-arg constructor; otherwise, if the
		/// class is serializable, invokes the no-arg constructor of the first
		/// non-serializable superclass.  Throws UnsupportedOperationException if
		/// this class descriptor is not associated with a class, if the associated
		/// class is non-serializable or if the appropriate no-arg constructor is
		/// inaccessible/unavailable.
		/// </summary>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: Object newInstance() throws InstantiationException, InvocationTargetException, UnsupportedOperationException
		internal virtual Object NewInstance()
		{
			RequireInitialized();
			if (Cons != null)
			{
				try
				{
					return Cons.newInstance();
				}
				catch (IllegalAccessException ex)
				{
					// should not occur, as access checks have been suppressed
					throw new InternalError(ex);
				}
			}
			else
			{
				throw new UnsupportedOperationException();
			}
		}

		/// <summary>
		/// Invokes the writeObject method of the represented serializable class.
		/// Throws UnsupportedOperationException if this class descriptor is not
		/// associated with a class, or if the class is externalizable,
		/// non-serializable or does not define writeObject.
		/// </summary>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: void invokeWriteObject(Object obj, ObjectOutputStream out) throws IOException, UnsupportedOperationException
		internal virtual void InvokeWriteObject(Object obj, ObjectOutputStream @out)
		{
			RequireInitialized();
			if (WriteObjectMethod != null)
			{
				try
				{
					WriteObjectMethod.invoke(obj, new Object[]{@out});
				}
				catch (InvocationTargetException ex)
				{
					Throwable th = ex.TargetException;
					if (th is IOException)
					{
						throw (IOException) th;
					}
					else
					{
						ThrowMiscException(th);
					}
				}
				catch (IllegalAccessException ex)
				{
					// should not occur, as access checks have been suppressed
					throw new InternalError(ex);
				}
			}
			else
			{
				throw new UnsupportedOperationException();
			}
		}

		/// <summary>
		/// Invokes the readObject method of the represented serializable class.
		/// Throws UnsupportedOperationException if this class descriptor is not
		/// associated with a class, or if the class is externalizable,
		/// non-serializable or does not define readObject.
		/// </summary>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: void invokeReadObject(Object obj, ObjectInputStream in) throws ClassNotFoundException, IOException, UnsupportedOperationException
		internal virtual void InvokeReadObject(Object obj, ObjectInputStream @in)
		{
			RequireInitialized();
			if (ReadObjectMethod != null)
			{
				try
				{
					ReadObjectMethod.invoke(obj, new Object[]{@in});
				}
				catch (InvocationTargetException ex)
				{
					Throwable th = ex.TargetException;
					if (th is ClassNotFoundException)
					{
						throw (ClassNotFoundException) th;
					}
					else if (th is IOException)
					{
						throw (IOException) th;
					}
					else
					{
						ThrowMiscException(th);
					}
				}
				catch (IllegalAccessException ex)
				{
					// should not occur, as access checks have been suppressed
					throw new InternalError(ex);
				}
			}
			else
			{
				throw new UnsupportedOperationException();
			}
		}

		/// <summary>
		/// Invokes the readObjectNoData method of the represented serializable
		/// class.  Throws UnsupportedOperationException if this class descriptor is
		/// not associated with a class, or if the class is externalizable,
		/// non-serializable or does not define readObjectNoData.
		/// </summary>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: void invokeReadObjectNoData(Object obj) throws IOException, UnsupportedOperationException
		internal virtual void InvokeReadObjectNoData(Object obj)
		{
			RequireInitialized();
			if (ReadObjectNoDataMethod != null)
			{
				try
				{
					ReadObjectNoDataMethod.invoke(obj, (Object[]) null);
				}
				catch (InvocationTargetException ex)
				{
					Throwable th = ex.TargetException;
					if (th is ObjectStreamException)
					{
						throw (ObjectStreamException) th;
					}
					else
					{
						ThrowMiscException(th);
					}
				}
				catch (IllegalAccessException ex)
				{
					// should not occur, as access checks have been suppressed
					throw new InternalError(ex);
				}
			}
			else
			{
				throw new UnsupportedOperationException();
			}
		}

		/// <summary>
		/// Invokes the writeReplace method of the represented serializable class and
		/// returns the result.  Throws UnsupportedOperationException if this class
		/// descriptor is not associated with a class, or if the class is
		/// non-serializable or does not define writeReplace.
		/// </summary>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: Object invokeWriteReplace(Object obj) throws IOException, UnsupportedOperationException
		internal virtual Object InvokeWriteReplace(Object obj)
		{
			RequireInitialized();
			if (WriteReplaceMethod != null)
			{
				try
				{
					return WriteReplaceMethod.invoke(obj, (Object[]) null);
				}
				catch (InvocationTargetException ex)
				{
					Throwable th = ex.TargetException;
					if (th is ObjectStreamException)
					{
						throw (ObjectStreamException) th;
					}
					else
					{
						ThrowMiscException(th);
						throw new InternalError(th); // never reached
					}
				}
				catch (IllegalAccessException ex)
				{
					// should not occur, as access checks have been suppressed
					throw new InternalError(ex);
				}
			}
			else
			{
				throw new UnsupportedOperationException();
			}
		}

		/// <summary>
		/// Invokes the readResolve method of the represented serializable class and
		/// returns the result.  Throws UnsupportedOperationException if this class
		/// descriptor is not associated with a class, or if the class is
		/// non-serializable or does not define readResolve.
		/// </summary>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: Object invokeReadResolve(Object obj) throws IOException, UnsupportedOperationException
		internal virtual Object InvokeReadResolve(Object obj)
		{
			RequireInitialized();
			if (ReadResolveMethod != null)
			{
				try
				{
					return ReadResolveMethod.invoke(obj, (Object[]) null);
				}
				catch (InvocationTargetException ex)
				{
					Throwable th = ex.TargetException;
					if (th is ObjectStreamException)
					{
						throw (ObjectStreamException) th;
					}
					else
					{
						ThrowMiscException(th);
						throw new InternalError(th); // never reached
					}
				}
				catch (IllegalAccessException ex)
				{
					// should not occur, as access checks have been suppressed
					throw new InternalError(ex);
				}
			}
			else
			{
				throw new UnsupportedOperationException();
			}
		}

		/// <summary>
		/// Class representing the portion of an object's serialized form allotted
		/// to data described by a given class descriptor.  If "hasData" is false,
		/// the object's serialized form does not contain data associated with the
		/// class descriptor.
		/// </summary>
		internal class ClassDataSlot
		{

			/// <summary>
			/// class descriptor "occupying" this slot </summary>
			internal readonly ObjectStreamClass Desc;
			/// <summary>
			/// true if serialized form includes data for this slot's descriptor </summary>
			internal readonly bool HasData;

			internal ClassDataSlot(ObjectStreamClass desc, bool hasData)
			{
				this.Desc = desc;
				this.HasData = hasData;
			}
		}

		/// <summary>
		/// Returns array of ClassDataSlot instances representing the data layout
		/// (including superclass data) for serialized objects described by this
		/// class descriptor.  ClassDataSlots are ordered by inheritance with those
		/// containing "higher" superclasses appearing first.  The final
		/// ClassDataSlot contains a reference to this descriptor.
		/// </summary>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: ClassDataSlot[] getClassDataLayout() throws InvalidClassException
		internal virtual ClassDataSlot[] ClassDataLayout
		{
			get
			{
				// REMIND: synchronize instead of relying on volatile?
				if (DataLayout == null)
				{
					DataLayout = ClassDataLayout0;
				}
				return DataLayout;
			}
		}

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: private ClassDataSlot[] getClassDataLayout0() throws InvalidClassException
		private ClassDataSlot[] ClassDataLayout0
		{
			get
			{
				List<ClassDataSlot> slots = new List<ClassDataSlot>();
				Class start = Cl, end = Cl;
    
				// locate closest non-serializable superclass
				while (end != null && end.IsSubclassOf(typeof(Serializable)))
				{
					end = end.BaseType;
				}
    
				HashSet<String> oscNames = new HashSet<String>(3);
    
				for (ObjectStreamClass d = this; d != null; d = d.SuperDesc_Renamed)
				{
					if (oscNames.Contains(d.Name_Renamed))
					{
						throw new InvalidClassException("Circular reference.");
					}
					else
					{
						oscNames.Add(d.name);
					}
    
					// search up inheritance hierarchy for class with matching name
					String searchName = (d.cl != null) ? d.cl.Name : d.name;
					Class match = null;
					for (Class c = start; c != end; c = c.BaseType)
					{
						if (searchName.Equals(c.Name))
						{
							match = c;
							break;
						}
					}
    
					// add "no data" slot for each unmatched class below match
					if (match != null)
					{
						for (Class c = start; c != match; c = c.BaseType)
						{
							slots.Add(new ClassDataSlot(ObjectStreamClass.Lookup(c, true), false));
						}
						start = match.BaseType;
					}
    
					// record descriptor/class pairing
					slots.Add(new ClassDataSlot(d.getVariantFor(match), true));
				}
    
				// add "no data" slot for any leftover unmatched classes
				for (Class c = start; c != end; c = c.BaseType)
				{
					slots.Add(new ClassDataSlot(ObjectStreamClass.Lookup(c, true), false));
				}
    
				// order slots from superclass -> subclass
				slots.Reverse();
				return slots.ToArray();
			}
		}

		/// <summary>
		/// Returns aggregate size (in bytes) of marshalled primitive field values
		/// for represented class.
		/// </summary>
		internal virtual int PrimDataSize
		{
			get
			{
				return PrimDataSize_Renamed;
			}
		}

		/// <summary>
		/// Returns number of non-primitive serializable fields of represented
		/// class.
		/// </summary>
		internal virtual int NumObjFields
		{
			get
			{
				return NumObjFields_Renamed;
			}
		}

		/// <summary>
		/// Fetches the serializable primitive field values of object obj and
		/// marshals them into byte array buf starting at offset 0.  It is the
		/// responsibility of the caller to ensure that obj is of the proper type if
		/// non-null.
		/// </summary>
		internal virtual void GetPrimFieldValues(Object obj, sbyte[] buf)
		{
			FieldRefl.GetPrimFieldValues(obj, buf);
		}

		/// <summary>
		/// Sets the serializable primitive fields of object obj using values
		/// unmarshalled from byte array buf starting at offset 0.  It is the
		/// responsibility of the caller to ensure that obj is of the proper type if
		/// non-null.
		/// </summary>
		internal virtual void SetPrimFieldValues(Object obj, sbyte[] buf)
		{
			FieldRefl.SetPrimFieldValues(obj, buf);
		}

		/// <summary>
		/// Fetches the serializable object field values of object obj and stores
		/// them in array vals starting at offset 0.  It is the responsibility of
		/// the caller to ensure that obj is of the proper type if non-null.
		/// </summary>
		internal virtual void GetObjFieldValues(Object obj, Object[] vals)
		{
			FieldRefl.GetObjFieldValues(obj, vals);
		}

		/// <summary>
		/// Sets the serializable object fields of object obj using values from
		/// array vals starting at offset 0.  It is the responsibility of the caller
		/// to ensure that obj is of the proper type if non-null.
		/// </summary>
		internal virtual void SetObjFieldValues(Object obj, Object[] vals)
		{
			FieldRefl.SetObjFieldValues(obj, vals);
		}

		/// <summary>
		/// Calculates and sets serializable field offsets, as well as primitive
		/// data size and object field count totals.  Throws InvalidClassException
		/// if fields are illegally ordered.
		/// </summary>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: private void computeFieldOffsets() throws InvalidClassException
		private void ComputeFieldOffsets()
		{
			PrimDataSize_Renamed = 0;
			NumObjFields_Renamed = 0;
			int firstObjIndex = -1;

			for (int i = 0; i < Fields_Renamed.Length; i++)
			{
				ObjectStreamField f = Fields_Renamed[i];
				switch (f.TypeCode)
				{
					case 'Z':
					case 'B':
						f.Offset = PrimDataSize_Renamed++;
						break;

					case 'C':
					case 'S':
						f.Offset = PrimDataSize_Renamed;
						PrimDataSize_Renamed += 2;
						break;

					case 'I':
					case 'F':
						f.Offset = PrimDataSize_Renamed;
						PrimDataSize_Renamed += 4;
						break;

					case 'J':
					case 'D':
						f.Offset = PrimDataSize_Renamed;
						PrimDataSize_Renamed += 8;
						break;

					case '[':
					case 'L':
						f.Offset = NumObjFields_Renamed++;
						if (firstObjIndex == -1)
						{
							firstObjIndex = i;
						}
						break;

					default:
						throw new InternalError();
				}
			}
			if (firstObjIndex != -1 && firstObjIndex + NumObjFields_Renamed != Fields_Renamed.Length)
			{
				throw new InvalidClassException(Name_Renamed, "illegal field order");
			}
		}

		/// <summary>
		/// If given class is the same as the class associated with this class
		/// descriptor, returns reference to this class descriptor.  Otherwise,
		/// returns variant of this class descriptor bound to given class.
		/// </summary>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: private ObjectStreamClass getVariantFor(Class cl) throws InvalidClassException
		private ObjectStreamClass GetVariantFor(Class cl)
		{
			if (this.Cl == cl)
			{
				return this;
			}
			ObjectStreamClass desc = new ObjectStreamClass();
			if (IsProxy)
			{
				desc.InitProxy(cl, null, SuperDesc_Renamed);
			}
			else
			{
				desc.InitNonProxy(this, cl, null, SuperDesc_Renamed);
			}
			return desc;
		}

		/// <summary>
		/// Returns public no-arg constructor of given class, or null if none found.
		/// Access checks are disabled on the returned constructor (if any), since
		/// the defining class may still be non-public.
		/// </summary>
//JAVA TO C# CONVERTER TODO TASK: Java wildcard generics are not converted to .NET:
//ORIGINAL LINE: private static Constructor<?> getExternalizableConstructor(Class cl)
		private static Constructor<?> GetExternalizableConstructor(Class cl)
		{
			try
			{
//JAVA TO C# CONVERTER TODO TASK: Java wildcard generics are not converted to .NET:
//ORIGINAL LINE: Constructor<?> cons = cl.getDeclaredConstructor((Class[]) null);
				Constructor<?> cons = cl.GetDeclaredConstructor((Class[]) null);
				cons.Accessible = true;
				return ((cons.Modifiers & Modifier.PUBLIC) != 0) ? cons : null;
			}
			catch (NoSuchMethodException)
			{
				return null;
			}
		}

		/// <summary>
		/// Returns subclass-accessible no-arg constructor of first non-serializable
		/// superclass, or null if none found.  Access checks are disabled on the
		/// returned constructor (if any).
		/// </summary>
//JAVA TO C# CONVERTER TODO TASK: Java wildcard generics are not converted to .NET:
//ORIGINAL LINE: private static Constructor<?> getSerializableConstructor(Class cl)
		private static Constructor<?> GetSerializableConstructor(Class cl)
		{
			Class initCl = cl;
			while (initCl.IsSubclassOf(typeof(Serializable)))
			{
				if ((initCl = initCl.BaseType) == null)
				{
					return null;
				}
			}
			try
			{
//JAVA TO C# CONVERTER TODO TASK: Java wildcard generics are not converted to .NET:
//ORIGINAL LINE: Constructor<?> cons = initCl.getDeclaredConstructor((Class[]) null);
				Constructor<?> cons = initCl.GetDeclaredConstructor((Class[]) null);
				int mods = cons.Modifiers;
				if ((mods & Modifier.PRIVATE) != 0 || ((mods & (Modifier.PUBLIC | Modifier.PROTECTED)) == 0 && !PackageEquals(cl, initCl)))
				{
					return null;
				}
				cons = ReflFactory.newConstructorForSerialization(cl, cons);
				cons.Accessible = true;
				return cons;
			}
			catch (NoSuchMethodException)
			{
				return null;
			}
		}

		/// <summary>
		/// Returns non-static, non-abstract method with given signature provided it
		/// is defined by or accessible (via inheritance) by the given class, or
		/// null if no match found.  Access checks are disabled on the returned
		/// method (if any).
		/// </summary>
		private static Method GetInheritableMethod(Class cl, String name, Class[] argTypes, Class returnType)
		{
			Method meth = null;
			Class defCl = cl;
			while (defCl != null)
			{
				try
				{
					meth = defCl.GetDeclaredMethod(name, argTypes);
					break;
				}
				catch (NoSuchMethodException)
				{
					defCl = defCl.BaseType;
				}
			}

			if ((meth == null) || (meth.ReturnType != returnType))
			{
				return null;
			}
			meth.Accessible = true;
			int mods = meth.Modifiers;
			if ((mods & (Modifier.STATIC | Modifier.ABSTRACT)) != 0)
			{
				return null;
			}
			else if ((mods & (Modifier.PUBLIC | Modifier.PROTECTED)) != 0)
			{
				return meth;
			}
			else if ((mods & Modifier.PRIVATE) != 0)
			{
				return (cl == defCl) ? meth : null;
			}
			else
			{
				return PackageEquals(cl, defCl) ? meth : null;
			}
		}

		/// <summary>
		/// Returns non-static private method with given signature defined by given
		/// class, or null if none found.  Access checks are disabled on the
		/// returned method (if any).
		/// </summary>
		private static Method GetPrivateMethod(Class cl, String name, Class[] argTypes, Class returnType)
		{
			try
			{
				Method meth = cl.GetDeclaredMethod(name, argTypes);
				meth.Accessible = true;
				int mods = meth.Modifiers;
				return ((meth.ReturnType == returnType) && ((mods & Modifier.STATIC) == 0) && ((mods & Modifier.PRIVATE) != 0)) ? meth : null;
			}
			catch (NoSuchMethodException)
			{
				return null;
			}
		}

		/// <summary>
		/// Returns true if classes are defined in the same runtime package, false
		/// otherwise.
		/// </summary>
		private static bool PackageEquals(Class cl1, Class cl2)
		{
			return (cl1.ClassLoader == cl2.ClassLoader && GetPackageName(cl1).Equals(GetPackageName(cl2)));
		}

		/// <summary>
		/// Returns package name of given class.
		/// </summary>
		private static String GetPackageName(Class cl)
		{
			String s = cl.Name;
			int i = s.LastIndexOf('[');
			if (i >= 0)
			{
				s = s.Substring(i + 2);
			}
			i = s.LastIndexOf('.');
			return (i >= 0) ? s.Substring(0, i) : "";
		}

		/// <summary>
		/// Compares class names for equality, ignoring package names.  Returns true
		/// if class names equal, false otherwise.
		/// </summary>
		private static bool ClassNamesEqual(String name1, String name2)
		{
			name1 = name1.Substring(name1.LastIndexOf('.') + 1);
			name2 = name2.Substring(name2.LastIndexOf('.') + 1);
			return name1.Equals(name2);
		}

		/// <summary>
		/// Returns JVM type signature for given class.
		/// </summary>
		private static String GetClassSignature(Class cl)
		{
			StringBuilder sbuf = new StringBuilder();
			while (cl.Array)
			{
				sbuf.Append('[');
				cl = cl.ComponentType;
			}
			if (cl.Primitive)
			{
				if (cl == Integer.TYPE)
				{
					sbuf.Append('I');
				}
				else if (cl == Byte.TYPE)
				{
					sbuf.Append('B');
				}
				else if (cl == Long.TYPE)
				{
					sbuf.Append('J');
				}
				else if (cl == Float.TYPE)
				{
					sbuf.Append('F');
				}
				else if (cl == Double.TYPE)
				{
					sbuf.Append('D');
				}
				else if (cl == Short.TYPE)
				{
					sbuf.Append('S');
				}
				else if (cl == Character.TYPE)
				{
					sbuf.Append('C');
				}
				else if (cl == Boolean.TYPE)
				{
					sbuf.Append('Z');
				}
				else if (cl == Void.TYPE)
				{
					sbuf.Append('V');
				}
				else
				{
					throw new InternalError();
				}
			}
			else
			{
				sbuf.Append('L' + cl.Name.Replace('.', '/') + ';');
			}
			return sbuf.ToString();
		}

		/// <summary>
		/// Returns JVM type signature for given list of parameters and return type.
		/// </summary>
		private static String GetMethodSignature(Class[] paramTypes, Class retType)
		{
			StringBuilder sbuf = new StringBuilder();
			sbuf.Append('(');
			for (int i = 0; i < paramTypes.Length; i++)
			{
				sbuf.Append(GetClassSignature(paramTypes[i]));
			}
			sbuf.Append(')');
			sbuf.Append(GetClassSignature(retType));
			return sbuf.ToString();
		}

		/// <summary>
		/// Convenience method for throwing an exception that is either a
		/// RuntimeException, Error, or of some unexpected type (in which case it is
		/// wrapped inside an IOException).
		/// </summary>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: private static void throwMiscException(Throwable th) throws IOException
		private static void ThrowMiscException(Throwable th)
		{
			if (th is RuntimeException)
			{
				throw (RuntimeException) th;
			}
			else if (th is Error)
			{
				throw (Error) th;
			}
			else
			{
				IOException ex = new IOException("unexpected exception type");
				ex.InitCause(th);
				throw ex;
			}
		}

		/// <summary>
		/// Returns ObjectStreamField array describing the serializable fields of
		/// the given class.  Serializable fields backed by an actual field of the
		/// class are represented by ObjectStreamFields with corresponding non-null
		/// Field objects.  Throws InvalidClassException if the (explicitly
		/// declared) serializable fields are invalid.
		/// </summary>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: private static ObjectStreamField[] getSerialFields(Class cl) throws InvalidClassException
		private static ObjectStreamField[] GetSerialFields(Class cl)
		{
			ObjectStreamField[] fields;
			if (cl.IsSubclassOf(typeof(Serializable)) && !cl.IsSubclassOf(typeof(Externalizable)) && !Proxy.isProxyClass(cl) && !cl.Interface)
			{
				if ((fields = GetDeclaredSerialFields(cl)) == null)
				{
					fields = GetDefaultSerialFields(cl);
				}
				Arrays.Sort(fields);
			}
			else
			{
				fields = NO_FIELDS;
			}
			return fields;
		}

		/// <summary>
		/// Returns serializable fields of given class as defined explicitly by a
		/// "serialPersistentFields" field, or null if no appropriate
		/// "serialPersistentFields" field is defined.  Serializable fields backed
		/// by an actual field of the class are represented by ObjectStreamFields
		/// with corresponding non-null Field objects.  For compatibility with past
		/// releases, a "serialPersistentFields" field with a null value is
		/// considered equivalent to not declaring "serialPersistentFields".  Throws
		/// InvalidClassException if the declared serializable fields are
		/// invalid--e.g., if multiple fields share the same name.
		/// </summary>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: private static ObjectStreamField[] getDeclaredSerialFields(Class cl) throws InvalidClassException
		private static ObjectStreamField[] GetDeclaredSerialFields(Class cl)
		{
			ObjectStreamField[] serialPersistentFields = null;
			try
			{
				Field f = cl.GetDeclaredField("serialPersistentFields");
				int mask = Modifier.PRIVATE | Modifier.STATIC | Modifier.FINAL;
				if ((f.Modifiers & mask) == mask)
				{
					f.Accessible = true;
					serialPersistentFields = (ObjectStreamField[]) f.get(null);
				}
			}
			catch (Exception)
			{
			}
			if (serialPersistentFields == null)
			{
				return null;
			}
			else if (serialPersistentFields.Length == 0)
			{
				return NO_FIELDS;
			}

			ObjectStreamField[] boundFields = new ObjectStreamField[serialPersistentFields.Length];
			Set<String> fieldNames = new HashSet<String>(serialPersistentFields.Length);

			for (int i = 0; i < serialPersistentFields.Length; i++)
			{
				ObjectStreamField spf = serialPersistentFields[i];

				String fname = spf.Name;
				if (fieldNames.Contains(fname))
				{
					throw new InvalidClassException("multiple serializable fields named " + fname);
				}
				fieldNames.Add(fname);

				try
				{
					Field f = cl.GetDeclaredField(fname);
					if ((f.Type == spf.Type) && ((f.Modifiers & Modifier.STATIC) == 0))
					{
						boundFields[i] = new ObjectStreamField(f, spf.Unshared, true);
					}
				}
				catch (NoSuchFieldException)
				{
				}
				if (boundFields[i] == null)
				{
					boundFields[i] = new ObjectStreamField(fname, spf.Type, spf.Unshared);
				}
			}
			return boundFields;
		}

		/// <summary>
		/// Returns array of ObjectStreamFields corresponding to all non-static
		/// non-transient fields declared by given class.  Each ObjectStreamField
		/// contains a Field object for the field it represents.  If no default
		/// serializable fields exist, NO_FIELDS is returned.
		/// </summary>
		private static ObjectStreamField[] GetDefaultSerialFields(Class cl)
		{
			Field[] clFields = cl.DeclaredFields;
			List<ObjectStreamField> list = new List<ObjectStreamField>();
			int mask = Modifier.STATIC | Modifier.TRANSIENT;

			for (int i = 0; i < clFields.Length; i++)
			{
				if ((clFields[i].Modifiers & mask) == 0)
				{
					list.Add(new ObjectStreamField(clFields[i], false, true));
				}
			}
			int size = list.Count;
			return (size == 0) ? NO_FIELDS : list.ToArray();
		}

		/// <summary>
		/// Returns explicit serial version UID value declared by given class, or
		/// null if none.
		/// </summary>
		private static Long GetDeclaredSUID(Class cl)
		{
			try
			{
				Field f = cl.GetDeclaredField("serialVersionUID");
				int mask = Modifier.STATIC | Modifier.FINAL;
				if ((f.Modifiers & mask) == mask)
				{
					f.Accessible = true;
					return Convert.ToInt64(f.getLong(null));
				}
			}
			catch (Exception)
			{
			}
			return null;
		}

		/// <summary>
		/// Computes the default serial version UID value for the given class.
		/// </summary>
		private static long ComputeDefaultSUID(Class cl)
		{
			if (!cl.IsSubclassOf(typeof(Serializable)) || Proxy.isProxyClass(cl))
			{
				return 0L;
			}

			try
			{
				ByteArrayOutputStream bout = new ByteArrayOutputStream();
				DataOutputStream dout = new DataOutputStream(bout);

				dout.WriteUTF(cl.Name);

				int classMods = cl.Modifiers & (Modifier.PUBLIC | Modifier.FINAL | Modifier.INTERFACE | Modifier.ABSTRACT);

				/*
				 * compensate for javac bug in which ABSTRACT bit was set for an
				 * interface only if the interface declared methods
				 */
				Method[] methods = cl.DeclaredMethods;
				if ((classMods & Modifier.INTERFACE) != 0)
				{
					classMods = (methods.Length > 0) ? (classMods | Modifier.ABSTRACT) : (classMods & ~Modifier.ABSTRACT);
				}
				dout.WriteInt(classMods);

				if (!cl.Array)
				{
					/*
					 * compensate for change in 1.2FCS in which
					 * Class.getInterfaces() was modified to return Cloneable and
					 * Serializable for array classes.
					 */
					Class[] interfaces = cl.Interfaces;
					String[] ifaceNames = new String[interfaces.Length];
					for (int i = 0; i < interfaces.Length; i++)
					{
						ifaceNames[i] = interfaces[i].Name;
					}
					Arrays.Sort(ifaceNames);
					for (int i = 0; i < ifaceNames.Length; i++)
					{
						dout.WriteUTF(ifaceNames[i]);
					}
				}

				Field[] fields = cl.DeclaredFields;
				MemberSignature[] fieldSigs = new MemberSignature[fields.Length];
				for (int i = 0; i < fields.Length; i++)
				{
					fieldSigs[i] = new MemberSignature(fields[i]);
				}
				Arrays.Sort(fieldSigs, new ComparatorAnonymousInnerClassHelper());
				for (int i = 0; i < fieldSigs.Length; i++)
				{
					MemberSignature sig = fieldSigs[i];
					int mods = sig.Member.Modifiers & (Modifier.PUBLIC | Modifier.PRIVATE | Modifier.PROTECTED | Modifier.STATIC | Modifier.FINAL | Modifier.VOLATILE | Modifier.TRANSIENT);
					if (((mods & Modifier.PRIVATE) == 0) || ((mods & (Modifier.STATIC | Modifier.TRANSIENT)) == 0))
					{
						dout.WriteUTF(sig.Name);
						dout.WriteInt(mods);
						dout.WriteUTF(sig.Signature);
					}
				}

				if (hasStaticInitializer(cl))
				{
					dout.WriteUTF("<clinit>");
					dout.WriteInt(Modifier.STATIC);
					dout.WriteUTF("()V");
				}

//JAVA TO C# CONVERTER TODO TASK: Java wildcard generics are not converted to .NET:
//ORIGINAL LINE: Constructor<?>[] cons = cl.getDeclaredConstructors();
				Constructor<?>[] cons = cl.DeclaredConstructors;
				MemberSignature[] consSigs = new MemberSignature[cons.Length];
				for (int i = 0; i < cons.Length; i++)
				{
					consSigs[i] = new MemberSignature(cons[i]);
				}
				Arrays.Sort(consSigs, new ComparatorAnonymousInnerClassHelper2());
				for (int i = 0; i < consSigs.Length; i++)
				{
					MemberSignature sig = consSigs[i];
					int mods = sig.Member.Modifiers & (Modifier.PUBLIC | Modifier.PRIVATE | Modifier.PROTECTED | Modifier.STATIC | Modifier.FINAL | Modifier.SYNCHRONIZED | Modifier.NATIVE | Modifier.ABSTRACT | Modifier.STRICT);
					if ((mods & Modifier.PRIVATE) == 0)
					{
						dout.WriteUTF("<init>");
						dout.WriteInt(mods);
						dout.WriteUTF(sig.Signature.Replace('/', '.'));
					}
				}

				MemberSignature[] methSigs = new MemberSignature[methods.Length];
				for (int i = 0; i < methods.Length; i++)
				{
					methSigs[i] = new MemberSignature(methods[i]);
				}
				Arrays.Sort(methSigs, new ComparatorAnonymousInnerClassHelper3());
				for (int i = 0; i < methSigs.Length; i++)
				{
					MemberSignature sig = methSigs[i];
					int mods = sig.Member.Modifiers & (Modifier.PUBLIC | Modifier.PRIVATE | Modifier.PROTECTED | Modifier.STATIC | Modifier.FINAL | Modifier.SYNCHRONIZED | Modifier.NATIVE | Modifier.ABSTRACT | Modifier.STRICT);
					if ((mods & Modifier.PRIVATE) == 0)
					{
						dout.WriteUTF(sig.Name);
						dout.WriteInt(mods);
						dout.WriteUTF(sig.Signature.Replace('/', '.'));
					}
				}

				dout.Flush();

				MessageDigest md = MessageDigest.GetInstance("SHA");
				sbyte[] hashBytes = md.Digest(bout.ToByteArray());
				long hash = 0;
				for (int i = System.Math.Min(hashBytes.Length, 8) - 1; i >= 0; i--)
				{
					hash = (hash << 8) | (hashBytes[i] & 0xFF);
				}
				return hash;
			}
			catch (IOException ex)
			{
				throw new InternalError(ex);
			}
			catch (NoSuchAlgorithmException ex)
			{
				throw new SecurityException(ex.Message);
			}
		}

		private class ComparatorAnonymousInnerClassHelper : Comparator<MemberSignature>
		{
			public ComparatorAnonymousInnerClassHelper()
			{
			}

			public virtual int Compare(MemberSignature ms1, MemberSignature ms2)
			{
				return ms1.Name.CompareTo(ms2.Name);
			}
		}

		private class ComparatorAnonymousInnerClassHelper2 : Comparator<MemberSignature>
		{
			public ComparatorAnonymousInnerClassHelper2()
			{
			}

			public virtual int Compare(MemberSignature ms1, MemberSignature ms2)
			{
				return ms1.Signature.CompareTo(ms2.Signature);
			}
		}

		private class ComparatorAnonymousInnerClassHelper3 : Comparator<MemberSignature>
		{
			public ComparatorAnonymousInnerClassHelper3()
			{
			}

			public virtual int Compare(MemberSignature ms1, MemberSignature ms2)
			{
				int comp = ms1.Name.CompareTo(ms2.Name);
				if (comp == 0)
				{
					comp = ms1.Signature.CompareTo(ms2.Signature);
				}
				return comp;
			}
		}

		/// <summary>
		/// Returns true if the given class defines a static initializer method,
		/// false otherwise.
		/// </summary>
//JAVA TO C# CONVERTER TODO TASK: Replace 'unknown' with the appropriate dll name:
		[DllImport("unknown")]
		private extern static boolean hasStaticInitializer(Class cl);

		/// <summary>
		/// Class for computing and caching field/constructor/method signatures
		/// during serialVersionUID calculation.
		/// </summary>
		private class MemberSignature
		{

			public readonly Member Member;
			public readonly String Name;
			public readonly String Signature;

			public MemberSignature(Field field)
			{
				Member = field;
				Name = field.Name;
				Signature = GetClassSignature(field.Type);
			}

			public MemberSignature<T1>(Constructor<T1> cons)
			{
				Member = cons;
				Name = cons.Name;
				Signature = GetMethodSignature(cons.ParameterTypes, Void.TYPE);
			}

			public MemberSignature(Method meth)
			{
				Member = meth;
				Name = meth.Name;
				Signature = GetMethodSignature(meth.ParameterTypes, meth.ReturnType);
			}
		}

		/// <summary>
		/// Class for setting and retrieving serializable field values in batch.
		/// </summary>
		// REMIND: dynamically generate these?
		private class FieldReflector
		{

			/// <summary>
			/// handle for performing unsafe operations </summary>
			internal static readonly Unsafe @unsafe = Unsafe.Unsafe;

			/// <summary>
			/// fields to operate on </summary>
			internal readonly ObjectStreamField[] Fields_Renamed;
			/// <summary>
			/// number of primitive fields </summary>
			internal readonly int NumPrimFields;
			/// <summary>
			/// unsafe field keys for reading fields - may contain dupes </summary>
			internal readonly long[] ReadKeys;
			/// <summary>
			/// unsafe fields keys for writing fields - no dupes </summary>
			internal readonly long[] WriteKeys;
			/// <summary>
			/// field data offsets </summary>
			internal readonly int[] Offsets;
			/// <summary>
			/// field type codes </summary>
			internal readonly char[] TypeCodes;
			/// <summary>
			/// field types </summary>
			internal readonly Class[] Types;

			/// <summary>
			/// Constructs FieldReflector capable of setting/getting values from the
			/// subset of fields whose ObjectStreamFields contain non-null
			/// reflective Field objects.  ObjectStreamFields with null Fields are
			/// treated as filler, for which get operations return default values
			/// and set operations discard given values.
			/// </summary>
			internal FieldReflector(ObjectStreamField[] fields)
			{
				this.Fields_Renamed = fields;
				int nfields = fields.Length;
				ReadKeys = new long[nfields];
				WriteKeys = new long[nfields];
				Offsets = new int[nfields];
				TypeCodes = new char[nfields];
				List<Class> typeList = new List<Class>();
				Set<Long> usedKeys = new HashSet<Long>();


				for (int i = 0; i < nfields; i++)
				{
					ObjectStreamField f = fields[i];
					Field rf = f.Field;
					long key = (rf != null) ? @unsafe.objectFieldOffset(rf) : Unsafe.INVALID_FIELD_OFFSET;
					ReadKeys[i] = key;
					WriteKeys[i] = usedKeys.Add(key) ? key : Unsafe.INVALID_FIELD_OFFSET;
					Offsets[i] = f.Offset;
					TypeCodes[i] = f.TypeCode;
					if (!f.Primitive)
					{
						typeList.Add((rf != null) ? rf.Type : null);
					}
				}

				Types = typeList.ToArray();
				NumPrimFields = nfields - Types.Length;
			}

			/// <summary>
			/// Returns list of ObjectStreamFields representing fields operated on
			/// by this reflector.  The shared/unshared values and Field objects
			/// contained by ObjectStreamFields in the list reflect their bindings
			/// to locally defined serializable fields.
			/// </summary>
			internal virtual ObjectStreamField[] Fields
			{
				get
				{
					return Fields_Renamed;
				}
			}

			/// <summary>
			/// Fetches the serializable primitive field values of object obj and
			/// marshals them into byte array buf starting at offset 0.  The caller
			/// is responsible for ensuring that obj is of the proper type.
			/// </summary>
			internal virtual void GetPrimFieldValues(Object obj, sbyte[] buf)
			{
				if (obj == null)
				{
					throw new NullPointerException();
				}
				/* assuming checkDefaultSerialize() has been called on the class
				 * descriptor this FieldReflector was obtained from, no field keys
				 * in array should be equal to Unsafe.INVALID_FIELD_OFFSET.
				 */
				for (int i = 0; i < NumPrimFields; i++)
				{
					long key = ReadKeys[i];
					int off = Offsets[i];
					switch (TypeCodes[i])
					{
						case 'Z':
							Bits.PutBoolean(buf, off, @unsafe.getBoolean(obj, key));
							break;

						case 'B':
							buf[off] = @unsafe.getByte(obj, key);
							break;

						case 'C':
							Bits.PutChar(buf, off, @unsafe.getChar(obj, key));
							break;

						case 'S':
							Bits.PutShort(buf, off, @unsafe.getShort(obj, key));
							break;

						case 'I':
							Bits.PutInt(buf, off, @unsafe.getInt(obj, key));
							break;

						case 'F':
							Bits.PutFloat(buf, off, @unsafe.getFloat(obj, key));
							break;

						case 'J':
							Bits.PutLong(buf, off, @unsafe.getLong(obj, key));
							break;

						case 'D':
							Bits.PutDouble(buf, off, @unsafe.getDouble(obj, key));
							break;

						default:
							throw new InternalError();
					}
				}
			}

			/// <summary>
			/// Sets the serializable primitive fields of object obj using values
			/// unmarshalled from byte array buf starting at offset 0.  The caller
			/// is responsible for ensuring that obj is of the proper type.
			/// </summary>
			internal virtual void SetPrimFieldValues(Object obj, sbyte[] buf)
			{
				if (obj == null)
				{
					throw new NullPointerException();
				}
				for (int i = 0; i < NumPrimFields; i++)
				{
					long key = WriteKeys[i];
					if (key == Unsafe.INVALID_FIELD_OFFSET)
					{
						continue; // discard value
					}
					int off = Offsets[i];
					switch (TypeCodes[i])
					{
						case 'Z':
							@unsafe.putBoolean(obj, key, Bits.GetBoolean(buf, off));
							break;

						case 'B':
							@unsafe.putByte(obj, key, buf[off]);
							break;

						case 'C':
							@unsafe.putChar(obj, key, Bits.GetChar(buf, off));
							break;

						case 'S':
							@unsafe.putShort(obj, key, Bits.GetShort(buf, off));
							break;

						case 'I':
							@unsafe.putInt(obj, key, Bits.GetInt(buf, off));
							break;

						case 'F':
							@unsafe.putFloat(obj, key, Bits.GetFloat(buf, off));
							break;

						case 'J':
							@unsafe.putLong(obj, key, Bits.GetLong(buf, off));
							break;

						case 'D':
							@unsafe.putDouble(obj, key, Bits.GetDouble(buf, off));
							break;

						default:
							throw new InternalError();
					}
				}
			}

			/// <summary>
			/// Fetches the serializable object field values of object obj and
			/// stores them in array vals starting at offset 0.  The caller is
			/// responsible for ensuring that obj is of the proper type.
			/// </summary>
			internal virtual void GetObjFieldValues(Object obj, Object[] vals)
			{
				if (obj == null)
				{
					throw new NullPointerException();
				}
				/* assuming checkDefaultSerialize() has been called on the class
				 * descriptor this FieldReflector was obtained from, no field keys
				 * in array should be equal to Unsafe.INVALID_FIELD_OFFSET.
				 */
				for (int i = NumPrimFields; i < Fields_Renamed.Length; i++)
				{
					switch (TypeCodes[i])
					{
						case 'L':
						case '[':
							vals[Offsets[i]] = @unsafe.getObject(obj, ReadKeys[i]);
							break;

						default:
							throw new InternalError();
					}
				}
			}

			/// <summary>
			/// Sets the serializable object fields of object obj using values from
			/// array vals starting at offset 0.  The caller is responsible for
			/// ensuring that obj is of the proper type; however, attempts to set a
			/// field with a value of the wrong type will trigger an appropriate
			/// ClassCastException.
			/// </summary>
			internal virtual void SetObjFieldValues(Object obj, Object[] vals)
			{
				if (obj == null)
				{
					throw new NullPointerException();
				}
				for (int i = NumPrimFields; i < Fields_Renamed.Length; i++)
				{
					long key = WriteKeys[i];
					if (key == Unsafe.INVALID_FIELD_OFFSET)
					{
						continue; // discard value
					}
					switch (TypeCodes[i])
					{
						case 'L':
						case '[':
							Object val = vals[Offsets[i]];
							if (val != null && !Types[i - NumPrimFields].isInstance(val))
							{
								Field f = Fields_Renamed[i].Field;
//JAVA TO C# CONVERTER WARNING: The .NET Type.FullName property will not always yield results identical to the Java Class.getName method:
								throw new ClassCastException("cannot assign instance of " + val.GetType().FullName + " to field " + f.DeclaringClass.Name + "." + f.Name + " of type " + f.Type.Name + " in instance of " + obj.GetType().FullName);
							}
							@unsafe.putObject(obj, key, val);
							break;

						default:
							throw new InternalError();
					}
				}
			}
		}

		/// <summary>
		/// Matches given set of serializable fields with serializable fields
		/// described by the given local class descriptor, and returns a
		/// FieldReflector instance capable of setting/getting values from the
		/// subset of fields that match (non-matching fields are treated as filler,
		/// for which get operations return default values and set operations
		/// discard given values).  Throws InvalidClassException if unresolvable
		/// type conflicts exist between the two sets of fields.
		/// </summary>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: private static FieldReflector getReflector(ObjectStreamField[] fields, ObjectStreamClass localDesc) throws InvalidClassException
		private static FieldReflector GetReflector(ObjectStreamField[] fields, ObjectStreamClass localDesc)
		{
			// class irrelevant if no fields
			Class cl = (localDesc != null && fields.Length > 0) ? localDesc.Cl : null;
			ProcessQueue(Caches.ReflectorsQueue, Caches.Reflectors);
			FieldReflectorKey key = new FieldReflectorKey(cl, fields, Caches.ReflectorsQueue);
//JAVA TO C# CONVERTER TODO TASK: Java wildcard generics are not converted to .NET:
//ORIGINAL LINE: Reference<?> ref = Caches.reflectors.get(key);
			Reference<?> @ref = Caches.Reflectors[key];
			Object entry = null;
			if (@ref != null)
			{
				entry = @ref.get();
			}
			EntryFuture future = null;
			if (entry == null)
			{
				EntryFuture newEntry = new EntryFuture();
//JAVA TO C# CONVERTER TODO TASK: Java wildcard generics are not converted to .NET:
//ORIGINAL LINE: Reference<?> newRef = new SoftReference<>(newEntry);
				Reference<?> newRef = new SoftReference<?>(newEntry);
				do
				{
					if (@ref != null)
					{
						Caches.Reflectors.Remove(key, @ref);
					}
					@ref = Caches.Reflectors.PutIfAbsent(key, newRef);
					if (@ref != null)
					{
						entry = @ref.get();
					}
				} while (@ref != null && entry == null);
				if (entry == null)
				{
					future = newEntry;
				}
			}

			if (entry is FieldReflector) // check common case first
			{
				return (FieldReflector) entry;
			}
			else if (entry is EntryFuture)
			{
				entry = ((EntryFuture) entry).Get();
			}
			else if (entry == null)
			{
				try
				{
					entry = new FieldReflector(MatchFields(fields, localDesc));
				}
				catch (Throwable th)
				{
					entry = th;
				}
				future.Set(entry);
				Caches.Reflectors[key] = new SoftReference<Object>(entry);
			}

			if (entry is FieldReflector)
			{
				return (FieldReflector) entry;
			}
			else if (entry is InvalidClassException)
			{
				throw (InvalidClassException) entry;
			}
			else if (entry is RuntimeException)
			{
				throw (RuntimeException) entry;
			}
			else if (entry is Error)
			{
				throw (Error) entry;
			}
			else
			{
				throw new InternalError("unexpected entry: " + entry);
			}
		}

		/// <summary>
		/// FieldReflector cache lookup key.  Keys are considered equal if they
		/// refer to the same class and equivalent field formats.
		/// </summary>
		private class FieldReflectorKey : WeakReference<Class>
		{

			internal readonly String Sigs;
			internal readonly int Hash;
			internal readonly bool NullClass;

			internal FieldReflectorKey(Class cl, ObjectStreamField[] fields, ReferenceQueue<Class> queue) : base(cl, queue)
			{
				NullClass = (cl == null);
				StringBuilder sbuf = new StringBuilder();
				for (int i = 0; i < fields.Length; i++)
				{
					ObjectStreamField f = fields[i];
					sbuf.Append(f.Name).Append(f.Signature);
				}
				Sigs = sbuf.ToString();
				Hash = System.identityHashCode(cl) + Sigs.HashCode();
			}

			public override int HashCode()
			{
				return Hash;
			}

			public override bool Equals(Object obj)
			{
				if (obj == this)
				{
					return true;
				}

				if (obj is FieldReflectorKey)
				{
					FieldReflectorKey other = (FieldReflectorKey) obj;
					Class referent;
					return (NullClass ? other.NullClass : ((referent = get()) != null) && (referent == other.get())) && Sigs.Equals(other.Sigs);
				}
				else
				{
					return false;
				}
			}
		}

		/// <summary>
		/// Matches given set of serializable fields with serializable fields
		/// obtained from the given local class descriptor (which contain bindings
		/// to reflective Field objects).  Returns list of ObjectStreamFields in
		/// which each ObjectStreamField whose signature matches that of a local
		/// field contains a Field object for that field; unmatched
		/// ObjectStreamFields contain null Field objects.  Shared/unshared settings
		/// of the returned ObjectStreamFields also reflect those of matched local
		/// ObjectStreamFields.  Throws InvalidClassException if unresolvable type
		/// conflicts exist between the two sets of fields.
		/// </summary>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: private static ObjectStreamField[] matchFields(ObjectStreamField[] fields, ObjectStreamClass localDesc) throws InvalidClassException
		private static ObjectStreamField[] MatchFields(ObjectStreamField[] fields, ObjectStreamClass localDesc)
		{
			ObjectStreamField[] localFields = (localDesc != null) ? localDesc.Fields_Renamed : NO_FIELDS;

			/*
			 * Even if fields == localFields, we cannot simply return localFields
			 * here.  In previous implementations of serialization,
			 * ObjectStreamField.getType() returned Object.class if the
			 * ObjectStreamField represented a non-primitive field and belonged to
			 * a non-local class descriptor.  To preserve this (questionable)
			 * behavior, the ObjectStreamField instances returned by matchFields
			 * cannot report non-primitive types other than Object.class; hence
			 * localFields cannot be returned directly.
			 */

			ObjectStreamField[] matches = new ObjectStreamField[fields.Length];
			for (int i = 0; i < fields.Length; i++)
			{
				ObjectStreamField f = fields[i], m = null;
				for (int j = 0; j < localFields.Length; j++)
				{
					ObjectStreamField lf = localFields[j];
					if (f.Name.Equals(lf.Name))
					{
						if ((f.Primitive || lf.Primitive) && f.TypeCode != lf.TypeCode)
						{
							throw new InvalidClassException(localDesc.Name_Renamed, "incompatible types for field " + f.Name);
						}
						if (lf.Field != null)
						{
							m = new ObjectStreamField(lf.Field, lf.Unshared, false);
						}
						else
						{
							m = new ObjectStreamField(lf.Name, lf.Signature, lf.Unshared);
						}
					}
				}
				if (m == null)
				{
					m = new ObjectStreamField(f.Name, f.Signature, false);
				}
				m.Offset = f.Offset;
				matches[i] = m;
			}
			return matches;
		}

		/// <summary>
		/// Removes from the specified map any keys that have been enqueued
		/// on the specified reference queue.
		/// </summary>
		internal static void processQueue<T1>(ReferenceQueue<Class> queue, ConcurrentMap<T1> map) where T1 : WeakReference<Class>
		{
//JAVA TO C# CONVERTER TODO TASK: Java wildcard generics are not converted to .NET:
//ORIGINAL LINE: Reference<? extends Class> ref;
			Reference<?> @ref;
			while ((@ref = queue.poll()) != null)
			{
				map.Remove(@ref);
			}
		}

		/// <summary>
		///  Weak key for Class objects.
		/// 
		/// 
		/// </summary>
		internal class WeakClassKey : WeakReference<Class>
		{
			/// <summary>
			/// saved value of the referent's identity hash code, to maintain
			/// a consistent hash code after the referent has been cleared
			/// </summary>
			internal readonly int Hash;

			/// <summary>
			/// Create a new WeakClassKey to the given object, registered
			/// with a queue.
			/// </summary>
			internal WeakClassKey(Class cl, ReferenceQueue<Class> refQueue) : base(cl, refQueue)
			{
				Hash = System.identityHashCode(cl);
			}

			/// <summary>
			/// Returns the identity hash code of the original referent.
			/// </summary>
			public override int HashCode()
			{
				return Hash;
			}

			/// <summary>
			/// Returns true if the given object is this identical
			/// WeakClassKey instance, or, if this object's referent has not
			/// been cleared, if the given object is another WeakClassKey
			/// instance with the identical non-null referent as this one.
			/// </summary>
			public override bool Equals(Object obj)
			{
				if (obj == this)
				{
					return true;
				}

				if (obj is WeakClassKey)
				{
					Object referent = get();
					return (referent != null) && (referent == ((WeakClassKey) obj).get());
				}
				else
				{
					return false;
				}
			}
		}
	}

}
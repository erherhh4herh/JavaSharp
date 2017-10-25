using System;
using System.Diagnostics;
using System.Collections.Generic;

/*
 * Copyright (c) 2008, 2013, Oracle and/or its affiliates. All rights reserved.
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

namespace java.lang.invoke
{

	using BytecodeDescriptor = sun.invoke.util.BytecodeDescriptor;
	using VerifyAccess = sun.invoke.util.VerifyAccess;


	/// <summary>
	/// A {@code MemberName} is a compact symbolic datum which fully characterizes
	/// a method or field reference.
	/// A member name refers to a field, method, constructor, or member type.
	/// Every member name has a simple name (a string) and a type (either a Class or MethodType).
	/// A member name may also have a non-null declaring class, or it may be simply
	/// a naked name/type pair.
	/// A member name may also have non-zero modifier flags.
	/// Finally, a member name may be either resolved or unresolved.
	/// If it is resolved, the existence of the named
	/// <para>
	/// Whether resolved or not, a member name provides no access rights or
	/// invocation capability to its possessor.  It is merely a compact
	/// representation of all symbolic information necessary to link to
	/// and properly use the named member.
	/// </para>
	/// <para>
	/// When resolved, a member name's internal implementation may include references to JVM metadata.
	/// This representation is stateless and only decriptive.
	/// It provides no private information and no capability to use the member.
	/// </para>
	/// <para>
	/// By contrast, a <seealso cref="java.lang.reflect.Method"/> contains fuller information
	/// about the internals of a method (except its bytecodes) and also
	/// allows invocation.  A MemberName is much lighter than a Method,
	/// since it contains about 7 fields to the 16 of Method (plus its sub-arrays),
	/// and those seven fields omit much of the information in Method.
	/// @author jrose
	/// </para>
	/// </summary>
	/*non-public*/	 internal sealed class MemberName : Member, Cloneable
	 {
		private Class Clazz; // class in which the method is defined
		private String Name_Renamed; // may be null if not yet materialized
		private Object Type_Renamed; // may be null if not yet materialized
		private int Flags; // modifier bits; see reflect.Modifier
		//@Injected JVM_Method* vmtarget;
		//@Injected int         vmindex;
		private Object Resolution; // if null, this guy is resolved

		/// <summary>
		/// Return the declaring class of this member.
		///  In the case of a bare name and type, the declaring class will be null.
		/// </summary>
		public Class DeclaringClass
		{
			get
			{
				return Clazz;
			}
		}

		/// <summary>
		/// Utility method producing the class loader of the declaring class. </summary>
		public ClassLoader ClassLoader
		{
			get
			{
				return Clazz.ClassLoader;
			}
		}

		/// <summary>
		/// Return the simple name of this member.
		///  For a type, it is the same as <seealso cref="Class#getSimpleName"/>.
		///  For a method or field, it is the simple name of the member.
		///  For a constructor, it is always {@code "&lt;init&gt;"}.
		/// </summary>
		public String Name
		{
			get
			{
				if (Name_Renamed == null)
				{
					ExpandFromVM();
					if (Name_Renamed == null)
					{
						return null;
					}
				}
				return Name_Renamed;
			}
		}

		public MethodType MethodOrFieldType
		{
			get
			{
				if (Invocable)
				{
					return MethodType;
				}
				if (Getter)
				{
					return MethodType.MethodType(FieldType);
				}
				if (Setter)
				{
					return MethodType.MethodType(typeof(void), FieldType);
				}
				throw new InternalError("not a method or field: " + this);
			}
		}

		/// <summary>
		/// Return the declared type of this member, which
		///  must be a method or constructor.
		/// </summary>
		public MethodType MethodType
		{
			get
			{
				if (Type_Renamed == null)
				{
					ExpandFromVM();
					if (Type_Renamed == null)
					{
						return null;
					}
				}
				if (!Invocable)
				{
					throw newIllegalArgumentException("not invocable, no method type");
				}
    
				{
					// Get a snapshot of type which doesn't get changed by racing threads.
	//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
	//ORIGINAL LINE: final Object type = this.type;
					Object type = this.Type_Renamed;
					if (type is MethodType)
					{
						return (MethodType) type;
					}
				}
    
				// type is not a MethodType yet.  Convert it thread-safely.
				lock (this)
				{
					if (Type_Renamed is String)
					{
						String sig = (String) Type_Renamed;
						MethodType res = MethodType.FromMethodDescriptorString(sig, ClassLoader);
						Type_Renamed = res;
					}
					else if (Type_Renamed is Object[])
					{
						Object[] typeInfo = (Object[]) Type_Renamed;
						Class[] ptypes = (Class[]) typeInfo[1];
						Class rtype = (Class) typeInfo[0];
						MethodType res = MethodType.MethodType(rtype, ptypes);
						Type_Renamed = res;
					}
					// Make sure type is a MethodType for racing threads.
					Debug.Assert(Type_Renamed is MethodType, "bad method type " + Type_Renamed);
				}
				return (MethodType) Type_Renamed;
			}
		}

		/// <summary>
		/// Return the actual type under which this method or constructor must be invoked.
		///  For non-static methods or constructors, this is the type with a leading parameter,
		///  a reference to declaring class.  For static methods, it is the same as the declared type.
		/// </summary>
		public MethodType InvocationType
		{
			get
			{
				MethodType itype = MethodOrFieldType;
				if (Constructor && ReferenceKind == REF_newInvokeSpecial)
				{
					return itype.ChangeReturnType(Clazz);
				}
				if (!Static)
				{
					return itype.InsertParameterTypes(0, Clazz);
				}
				return itype;
			}
		}

		/// <summary>
		/// Utility method producing the parameter types of the method type. </summary>
		public Class[] ParameterTypes
		{
			get
			{
				return MethodType.ParameterArray();
			}
		}

		/// <summary>
		/// Utility method producing the return type of the method type. </summary>
		public Class ReturnType
		{
			get
			{
				return MethodType.ReturnType();
			}
		}

		/// <summary>
		/// Return the declared type of this member, which
		///  must be a field or type.
		///  If it is a type member, that type itself is returned.
		/// </summary>
		public Class FieldType
		{
			get
			{
				if (Type_Renamed == null)
				{
					ExpandFromVM();
					if (Type_Renamed == null)
					{
						return null;
					}
				}
				if (Invocable)
				{
					throw newIllegalArgumentException("not a field or nested class, no simple type");
				}
    
				{
					// Get a snapshot of type which doesn't get changed by racing threads.
	//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
	//ORIGINAL LINE: final Object type = this.type;
					Object type = this.Type_Renamed;
					if (type is Class)
					{
						return (Class) type;
					}
				}
    
				// type is not a Class yet.  Convert it thread-safely.
				lock (this)
				{
					if (Type_Renamed is String)
					{
						String sig = (String) Type_Renamed;
						MethodType mtype = MethodType.FromMethodDescriptorString("()" + sig, ClassLoader);
						Class res = mtype.ReturnType();
						Type_Renamed = res;
					}
					// Make sure type is a Class for racing threads.
					Debug.Assert(Type_Renamed is Class, "bad field type " + Type_Renamed);
				}
				return (Class) Type_Renamed;
			}
		}

		/// <summary>
		/// Utility method to produce either the method type or field type of this member. </summary>
		public Object Type
		{
			get
			{
				return (Invocable ? MethodType : FieldType);
			}
		}

		/// <summary>
		/// Utility method to produce the signature of this member,
		///  used within the class file format to describe its type.
		/// </summary>
		public String Signature
		{
			get
			{
				if (Type_Renamed == null)
				{
					ExpandFromVM();
					if (Type_Renamed == null)
					{
						return null;
					}
				}
				if (Invocable)
				{
					return BytecodeDescriptor.unparse(MethodType);
				}
				else
				{
					return BytecodeDescriptor.unparse(FieldType);
				}
			}
		}

		/// <summary>
		/// Return the modifier flags of this member. </summary>
		///  <seealso cref= java.lang.reflect.Modifier </seealso>
		public int Modifiers
		{
			get
			{
				return (Flags & RECOGNIZED_MODIFIERS);
			}
		}

		/// <summary>
		/// Return the reference kind of this member, or zero if none.
		/// </summary>
		public sbyte ReferenceKind
		{
			get
			{
				return (sbyte)(((int)((uint)Flags >> MN_REFERENCE_KIND_SHIFT)) & MN_REFERENCE_KIND_MASK);
			}
		}
		private bool ReferenceKindIsConsistent()
		{
			sbyte refKind = ReferenceKind;
			if (refKind == REF_NONE)
			{
				return Type;
			}
			if (Field)
			{
				assert(StaticIsConsistent());
				assert(MethodHandleNatives.RefKindIsField(refKind));
			}
			else if (Constructor)
			{
				assert(refKind == REF_newInvokeSpecial || refKind == REF_invokeSpecial);
			}
			else if (Method)
			{
				assert(StaticIsConsistent());
				assert(MethodHandleNatives.RefKindIsMethod(refKind));
				if (Clazz.Interface)
				{
					assert(refKind == REF_invokeInterface || refKind == REF_invokeStatic || refKind == REF_invokeSpecial || refKind == REF_invokeVirtual && ObjectPublicMethod);
				}
			}
			else
			{
				assert(false);
			}
			return true;
		}
		private bool ObjectPublicMethod
		{
			get
			{
				if (Clazz == typeof(Object))
				{
					return true;
				}
				MethodType mtype = MethodType;
				if (Name_Renamed.Equals("toString") && mtype.ReturnType() == typeof(String) && mtype.ParameterCount() == 0)
				{
					return true;
				}
				if (Name_Renamed.Equals("hashCode") && mtype.ReturnType() == typeof(int) && mtype.ParameterCount() == 0)
				{
					return true;
				}
				if (Name_Renamed.Equals("equals") && mtype.ReturnType() == typeof(bool) && mtype.ParameterCount() == 1 && mtype.ParameterType(0) == typeof(Object))
				{
					return true;
				}
				return false;
			}
		}
		/*non-public*/	 internal bool ReferenceKindIsConsistentWith(int originalRefKind)
	 {
			int refKind = ReferenceKind;
			if (refKind == originalRefKind)
			{
				return true;
			}
			switch (originalRefKind)
			{
			case REF_invokeInterface:
				// Looking up an interface method, can get (e.g.) Object.hashCode
				assert(refKind == REF_invokeVirtual || refKind == REF_invokeSpecial) : this;
				return true;
			case REF_invokeVirtual:
			case REF_newInvokeSpecial:
				// Looked up a virtual, can get (e.g.) final String.hashCode.
				assert(refKind == REF_invokeSpecial) : this;
				return true;
			}
			assert(false) : this + " != " + MethodHandleNatives.RefKindName((sbyte)originalRefKind);
			return true;
	 }
		private bool StaticIsConsistent()
		{
			sbyte refKind = ReferenceKind;
			return MethodHandleNatives.RefKindIsStatic(refKind) == Static || Modifiers == 0;
		}
		private bool VminfoIsConsistent()
		{
			sbyte refKind = ReferenceKind;
			assert(Resolved); // else don't call
			Object vminfo = MethodHandleNatives.getMemberVMInfo(this);
			assert(vminfo is Object[]);
			long vmindex = (Long)((Object[])vminfo)[0];
			Object vmtarget = ((Object[])vminfo)[1];
			if (MethodHandleNatives.RefKindIsField(refKind))
			{
				assert(vmindex >= 0) : vmindex + ":" + this;
				assert(vmtarget is Class);
			}
			else
			{
				if (MethodHandleNatives.RefKindDoesDispatch(refKind))
				{
					assert(vmindex >= 0) : vmindex + ":" + this;
				}
				else
				{
					assert(vmindex < 0) : vmindex;
				}
				assert(vmtarget is MemberName) : vmtarget + " in " + this;
			}
			return true;
		}

		private MemberName ChangeReferenceKind(sbyte refKind, sbyte oldKind)
		{
			assert(ReferenceKind == oldKind);
			assert(MethodHandleNatives.RefKindIsValid(refKind));
			Flags += (((int)refKind - oldKind) << MN_REFERENCE_KIND_SHIFT);
			return this;
		}

		private bool TestFlags(int mask, int value)
		{
			return (Flags & mask) == value;
		}
		private bool TestAllFlags(int mask)
		{
			return TestFlags(mask, mask);
		}
		private bool TestAnyFlags(int mask)
		{
			return !TestFlags(mask, 0);
		}

		/// <summary>
		/// Utility method to query if this member is a method handle invocation (invoke or invokeExact).
		///  Also returns true for the non-public MH.invokeBasic.
		/// </summary>
		public bool MethodHandleInvoke
		{
			get
			{
	//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
	//ORIGINAL LINE: final int bits = MH_INVOKE_MODS &~ Modifier.PUBLIC;
				int bits = MH_INVOKE_MODS & ~Modifier.PUBLIC;
	//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
	//ORIGINAL LINE: final int negs = Modifier.STATIC;
				int negs = Modifier.STATIC;
				if (TestFlags(bits | negs, bits) && Clazz == typeof(MethodHandle))
				{
					return IsMethodHandleInvokeName(Name_Renamed);
				}
				return false;
			}
		}
		public static bool IsMethodHandleInvokeName(String name)
		{
			switch (name)
			{
			case "invoke":
			case "invokeExact":
			case "invokeBasic": // internal sig-poly method
				return true;
			default:
				return false;
			}
		}
		private static readonly int MH_INVOKE_MODS = Modifier.NATIVE | Modifier.FINAL | Modifier.PUBLIC;

		/// <summary>
		/// Utility method to query the modifier flags of this member. </summary>
		public bool Static
		{
			get
			{
				return Modifier.isStatic(Flags);
			}
		}
		/// <summary>
		/// Utility method to query the modifier flags of this member. </summary>
		public bool Public
		{
			get
			{
				return Modifier.isPublic(Flags);
			}
		}
		/// <summary>
		/// Utility method to query the modifier flags of this member. </summary>
		public bool Private
		{
			get
			{
				return Modifier.isPrivate(Flags);
			}
		}
		/// <summary>
		/// Utility method to query the modifier flags of this member. </summary>
		public bool Protected
		{
			get
			{
				return Modifier.isProtected(Flags);
			}
		}
		/// <summary>
		/// Utility method to query the modifier flags of this member. </summary>
		public bool Final
		{
			get
			{
				return Modifier.isFinal(Flags);
			}
		}
		/// <summary>
		/// Utility method to query whether this member or its defining class is final. </summary>
		public bool CanBeStaticallyBound()
		{
			return Modifier.isFinal(Flags | Clazz.Modifiers);
		}
		/// <summary>
		/// Utility method to query the modifier flags of this member. </summary>
		public bool Volatile
		{
			get
			{
				return Modifier.isVolatile(Flags);
			}
		}
		/// <summary>
		/// Utility method to query the modifier flags of this member. </summary>
		public bool Abstract
		{
			get
			{
				return Modifier.isAbstract(Flags);
			}
		}
		/// <summary>
		/// Utility method to query the modifier flags of this member. </summary>
		public bool Native
		{
			get
			{
				return Modifier.isNative(Flags);
			}
		}
		// let the rest (native, volatile, transient, etc.) be tested via Modifier.isFoo

		// unofficial modifier flags, used by HotSpot:
		internal const int BRIDGE = 0x00000040;
		internal const int VARARGS = 0x00000080;
		internal const int SYNTHETIC = 0x00001000;
		internal const int ANNOTATION = 0x00002000;
		internal const int ENUM = 0x00004000;
		/// <summary>
		/// Utility method to query the modifier flags of this member; returns false if the member is not a method. </summary>
		public bool Bridge
		{
			get
			{
				return TestAllFlags(IS_METHOD | BRIDGE);
			}
		}
		/// <summary>
		/// Utility method to query the modifier flags of this member; returns false if the member is not a method. </summary>
		public bool Varargs
		{
			get
			{
				return TestAllFlags(VARARGS) && Invocable;
			}
		}
		/// <summary>
		/// Utility method to query the modifier flags of this member; returns false if the member is not a method. </summary>
		public bool Synthetic
		{
			get
			{
				return TestAllFlags(SYNTHETIC);
			}
		}

		internal const String CONSTRUCTOR_NAME = "<init>"; // the ever-popular

		// modifiers exported by the JVM:
		internal const int RECOGNIZED_MODIFIERS = 0xFFFF;

		// private flags, not part of RECOGNIZED_MODIFIERS:
		internal static readonly int IS_METHOD = MN_IS_METHOD, IS_CONSTRUCTOR = MN_IS_CONSTRUCTOR, IS_FIELD = MN_IS_FIELD, IS_TYPE = MN_IS_TYPE, CALLER_SENSITIVE = MN_CALLER_SENSITIVE; // @CallerSensitive annotation detected -  nested type -  field -  constructor -  method (not constructor)

		internal static readonly int ALL_ACCESS = Modifier.PUBLIC | Modifier.PRIVATE | Modifier.PROTECTED;
		internal static readonly int ALL_KINDS = IS_METHOD | IS_CONSTRUCTOR | IS_FIELD | IS_TYPE;
		internal static readonly int IS_INVOCABLE = IS_METHOD | IS_CONSTRUCTOR;
		internal static readonly int IS_FIELD_OR_METHOD = IS_METHOD | IS_FIELD;
		internal static readonly int SEARCH_ALL_SUPERS = MN_SEARCH_SUPERCLASSES | MN_SEARCH_INTERFACES;

		/// <summary>
		/// Utility method to query whether this member is a method or constructor. </summary>
		public bool Invocable
		{
			get
			{
				return TestAnyFlags(IS_INVOCABLE);
			}
		}
		/// <summary>
		/// Utility method to query whether this member is a method, constructor, or field. </summary>
		public bool FieldOrMethod
		{
			get
			{
				return TestAnyFlags(IS_FIELD_OR_METHOD);
			}
		}
		/// <summary>
		/// Query whether this member is a method. </summary>
		public bool Method
		{
			get
			{
				return TestAllFlags(IS_METHOD);
			}
		}
		/// <summary>
		/// Query whether this member is a constructor. </summary>
		public bool Constructor
		{
			get
			{
				return TestAllFlags(IS_CONSTRUCTOR);
			}
		}
		/// <summary>
		/// Query whether this member is a field. </summary>
		public bool Field
		{
			get
			{
				return TestAllFlags(IS_FIELD);
			}
		}
		/// <summary>
		/// Query whether this member is a type. </summary>
		public bool Type
		{
			get
			{
				return TestAllFlags(IS_TYPE);
			}
		}
		/// <summary>
		/// Utility method to query whether this member is neither public, private, nor protected. </summary>
		public bool Package
		{
			get
			{
				return !TestAnyFlags(ALL_ACCESS);
			}
		}
		/// <summary>
		/// Query whether this member has a CallerSensitive annotation. </summary>
		public bool CallerSensitive
		{
			get
			{
				return TestAllFlags(CALLER_SENSITIVE);
			}
		}

		/// <summary>
		/// Utility method to query whether this member is accessible from a given lookup class. </summary>
		public bool IsAccessibleFrom(Class lookupClass)
		{
			return VerifyAccess.isMemberAccessible(this.DeclaringClass, this.DeclaringClass, Flags, lookupClass, ALL_ACCESS | MethodHandles.Lookup.PACKAGE);
		}

		/// <summary>
		/// Initialize a query.   It is not resolved. </summary>
		private void Init(Class defClass, String name, Object type, int flags)
		{
			// defining class is allowed to be null (for a naked name/type pair)
			//name.toString();  // null check
			//type.equals(type);  // null check
			// fill in fields:
			this.Clazz = defClass;
			this.Name_Renamed = name;
			this.Type_Renamed = type;
			this.Flags = flags;
			assert(TestAnyFlags(ALL_KINDS));
			assert(this.Resolution == null); // nobody should have touched this yet
			//assert(referenceKindIsConsistent());  // do this after resolution
		}

		/// <summary>
		/// Calls down to the VM to fill in the fields.  This method is
		/// synchronized to avoid racing calls.
		/// </summary>
		private void ExpandFromVM()
		{
			if (Type_Renamed != null)
			{
				return;
			}
			if (!Resolved)
			{
				return;
			}
			MethodHandleNatives.expand(this);
		}

		// Capturing information from the Core Reflection API:
		private static int FlagsMods(int flags, int mods, sbyte refKind)
		{
			assert((flags & RECOGNIZED_MODIFIERS) == 0);
			assert((mods & ~RECOGNIZED_MODIFIERS) == 0);
			assert((refKind & ~MN_REFERENCE_KIND_MASK) == 0);
			return flags | mods | (refKind << MN_REFERENCE_KIND_SHIFT);
		}
		/// <summary>
		/// Create a name for the given reflected method.  The resulting name will be in a resolved state. </summary>
		public MemberName(Method m) : this(m, false)
		{
		}
//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @SuppressWarnings("LeakingThisInConstructor") public MemberName(Method m, boolean wantSpecial)
		public MemberName(Method m, bool wantSpecial)
		{
			m.GetType(); // NPE check
			// fill in vmtarget, vmindex while we have m in hand:
			MethodHandleNatives.init(this, m);
			if (Clazz == null) // MHN.init failed
			{
				if (m.DeclaringClass == typeof(MethodHandle) && IsMethodHandleInvokeName(m.Name))
				{
					// The JVM did not reify this signature-polymorphic instance.
					// Need a special case here.
					// See comments on MethodHandleNatives.linkMethod.
					MethodType type = MethodType.MethodType(m.ReturnType, m.ParameterTypes);
					int flags = FlagsMods(IS_METHOD, m.Modifiers, REF_invokeVirtual);
					Init(typeof(MethodHandle), m.Name, type, flags);
					if (MethodHandleInvoke)
					{
						return;
					}
				}
				throw new LinkageError(m.ToString());
			}
			assert(Resolved && this.Clazz != null);
			this.Name_Renamed = m.Name;
			if (this.Type_Renamed == null)
			{
				this.Type_Renamed = new Object[] {m.ReturnType, m.ParameterTypes};
			}
			if (wantSpecial)
			{
				if (Abstract)
				{
					throw new AbstractMethodError(this.ToString());
				}
				if (ReferenceKind == REF_invokeVirtual)
				{
					ChangeReferenceKind(REF_invokeSpecial, REF_invokeVirtual);
				}
				else if (ReferenceKind == REF_invokeInterface)
				{
					// invokeSpecial on a default method
					ChangeReferenceKind(REF_invokeSpecial, REF_invokeInterface);
				}
			}
		}
		public MemberName AsSpecial()
		{
			switch (ReferenceKind)
			{
			case REF_invokeSpecial:
				return this;
			case REF_invokeVirtual:
				return Clone().ChangeReferenceKind(REF_invokeSpecial, REF_invokeVirtual);
			case REF_invokeInterface:
				return Clone().ChangeReferenceKind(REF_invokeSpecial, REF_invokeInterface);
			case REF_newInvokeSpecial:
				return Clone().ChangeReferenceKind(REF_invokeSpecial, REF_newInvokeSpecial);
			}
			throw new IllegalArgumentException(this.ToString());
		}
		/// <summary>
		/// If this MN is not REF_newInvokeSpecial, return a clone with that ref. kind.
		///  In that case it must already be REF_invokeSpecial.
		/// </summary>
		public MemberName AsConstructor()
		{
			switch (ReferenceKind)
			{
			case REF_invokeSpecial:
				return Clone().ChangeReferenceKind(REF_newInvokeSpecial, REF_invokeSpecial);
			case REF_newInvokeSpecial:
				return this;
			}
			throw new IllegalArgumentException(this.ToString());
		}
		/// <summary>
		/// If this MN is a REF_invokeSpecial, return a clone with the "normal" kind
		///  REF_invokeVirtual; also switch either to REF_invokeInterface if clazz.isInterface.
		///  The end result is to get a fully virtualized version of the MN.
		///  (Note that resolving in the JVM will sometimes devirtualize, changing
		///  REF_invokeVirtual of a final to REF_invokeSpecial, and REF_invokeInterface
		///  in some corner cases to either of the previous two; this transform
		///  undoes that change under the assumption that it occurred.)
		/// </summary>
		public MemberName AsNormalOriginal()
		{
			sbyte normalVirtual = Clazz.Interface ? REF_invokeInterface : REF_invokeVirtual;
			sbyte refKind = ReferenceKind;
			sbyte newRefKind = refKind;
			MemberName result = this;
			switch (refKind)
			{
			case REF_invokeInterface:
			case REF_invokeVirtual:
			case REF_invokeSpecial:
				newRefKind = normalVirtual;
				break;
			}
			if (newRefKind == refKind)
			{
				return this;
			}
			result = Clone().ChangeReferenceKind(newRefKind, refKind);
			assert(this.ReferenceKindIsConsistentWith(result.ReferenceKind));
			return result;
		}
		/// <summary>
		/// Create a name for the given reflected constructor.  The resulting name will be in a resolved state. </summary>
//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @SuppressWarnings("LeakingThisInConstructor") public MemberName(Constructor<?> ctor)
		public MemberName<T1>(Constructor<T1> ctor)
		{
			ctor.GetType(); // NPE check
			// fill in vmtarget, vmindex while we have ctor in hand:
			MethodHandleNatives.init(this, ctor);
			assert(Resolved && this.Clazz != null);
			this.Name_Renamed = CONSTRUCTOR_NAME;
			if (this.Type_Renamed == null)
			{
				this.Type_Renamed = new Object[] {typeof(void), ctor.ParameterTypes};
			}
		}
		/// <summary>
		/// Create a name for the given reflected field.  The resulting name will be in a resolved state.
		/// </summary>
		public MemberName(Field fld) : this(fld, false)
		{
		}
//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @SuppressWarnings("LeakingThisInConstructor") public MemberName(Field fld, boolean makeSetter)
		public MemberName(Field fld, bool makeSetter)
		{
			fld.GetType(); // NPE check
			// fill in vmtarget, vmindex while we have fld in hand:
			MethodHandleNatives.init(this, fld);
			assert(Resolved && this.Clazz != null);
			this.Name_Renamed = fld.Name;
			this.Type_Renamed = fld.Type;
			assert((REF_putStatic - REF_getStatic) == (REF_putField - REF_getField));
			sbyte refKind = this.ReferenceKind;
			assert(refKind == (Static ? REF_getStatic : REF_getField));
			if (makeSetter)
			{
				ChangeReferenceKind((sbyte)(refKind + (REF_putStatic - REF_getStatic)), refKind);
			}
		}
		public bool Getter
		{
			get
			{
				return MethodHandleNatives.RefKindIsGetter(ReferenceKind);
			}
		}
		public bool Setter
		{
			get
			{
				return MethodHandleNatives.RefKindIsSetter(ReferenceKind);
			}
		}
		public MemberName AsSetter()
		{
			sbyte refKind = ReferenceKind;
			assert(MethodHandleNatives.RefKindIsGetter(refKind));
			assert((REF_putStatic - REF_getStatic) == (REF_putField - REF_getField));
			sbyte setterRefKind = (sbyte)(refKind + (REF_putField - REF_getField));
			return Clone().ChangeReferenceKind(setterRefKind, refKind);
		}
		/// <summary>
		/// Create a name for the given class.  The resulting name will be in a resolved state. </summary>
		public MemberName(Class type)
		{
			Init(type.DeclaringClass, type.SimpleName, type, FlagsMods(IS_TYPE, type.Modifiers, REF_NONE));
			InitResolved(true);
		}

		/// <summary>
		/// Create a name for a signature-polymorphic invoker.
		/// This is a placeholder for a signature-polymorphic instance
		/// (of MH.invokeExact, etc.) that the JVM does not reify.
		/// See comments on <seealso cref="MethodHandleNatives#linkMethod"/>.
		/// </summary>
		internal static MemberName MakeMethodHandleInvoke(String name, MethodType type)
		{
			return MakeMethodHandleInvoke(name, type, MH_INVOKE_MODS | SYNTHETIC);
		}
		internal static MemberName MakeMethodHandleInvoke(String name, MethodType type, int mods)
		{
			MemberName mem = new MemberName(typeof(MethodHandle), name, type, REF_invokeVirtual);
			mem.Flags |= mods; // it's not resolved, but add these modifiers anyway
			assert(mem.MethodHandleInvoke) : mem;
			return mem;
		}

		// bare-bones constructor; the JVM will fill it in
		internal MemberName()
		{
		}

		// locally useful cloner
		protected internal override MemberName Clone()
		{
			try
			{
				return (MemberName) base.Clone();
			}
			catch (CloneNotSupportedException ex)
			{
				throw newInternalError(ex);
			}
		}

		/// <summary>
		/// Get the definition of this member name.
		///  This may be in a super-class of the declaring class of this member.
		/// </summary>
		public MemberName Definition
		{
			get
			{
				if (!Resolved)
				{
					throw new IllegalStateException("must be resolved: " + this);
				}
				if (Type)
				{
					return this;
				}
				MemberName res = this.Clone();
				res.Clazz = null;
				res.Type_Renamed = null;
				res.Name_Renamed = null;
				res.Resolution = res;
				res.ExpandFromVM();
				assert(res.Name.Equals(this.Name));
				return res;
			}
		}

		public override int HashCode()
		{
			return Objects.hash(Clazz, ReferenceKind, Name_Renamed, Type);
		}
		public override bool Equals(Object that)
		{
			return (that is MemberName && this.Equals((MemberName)that));
		}

		/// <summary>
		/// Decide if two member names have exactly the same symbolic content.
		///  Does not take into account any actual class members, so even if
		///  two member names resolve to the same actual member, they may
		///  be distinct references.
		/// </summary>
		public bool Equals(MemberName that)
		{
			if (this == that)
			{
				return true;
			}
			if (that == null)
			{
				return false;
			}
			return this.Clazz == that.Clazz && this.ReferenceKind == that.ReferenceKind && Objects.Equals(this.Name_Renamed, that.Name_Renamed) && Objects.Equals(this.Type, that.Type);
		}

		// Construction from symbolic parts, for queries:
		/// <summary>
		/// Create a field or type name from the given components:
		///  Declaring class, name, type, reference kind.
		///  The declaring class may be supplied as null if this is to be a bare name and type.
		///  The resulting name will in an unresolved state.
		/// </summary>
		public MemberName(Class defClass, String name, Class type, sbyte refKind)
		{
			Init(defClass, name, type, FlagsMods(IS_FIELD, 0, refKind));
			InitResolved(false);
		}
		/// <summary>
		/// Create a method or constructor name from the given components:
		///  Declaring class, name, type, reference kind.
		///  It will be a constructor if and only if the name is {@code "&lt;init&gt;"}.
		///  The declaring class may be supplied as null if this is to be a bare name and type.
		///  The last argument is optional, a boolean which requests REF_invokeSpecial.
		///  The resulting name will in an unresolved state.
		/// </summary>
		public MemberName(Class defClass, String name, MethodType type, sbyte refKind)
		{
			int initFlags = (name != null && name.Equals(CONSTRUCTOR_NAME) ? IS_CONSTRUCTOR : IS_METHOD);
			Init(defClass, name, type, FlagsMods(initFlags, 0, refKind));
			InitResolved(false);
		}
		/// <summary>
		/// Create a method, constructor, or field name from the given components:
		///  Reference kind, declaring class, name, type.
		/// </summary>
		public MemberName(sbyte refKind, Class defClass, String name, Object type)
		{
			int kindFlags;
			if (MethodHandleNatives.RefKindIsField(refKind))
			{
				kindFlags = IS_FIELD;
				if (!(type is Class))
				{
					throw newIllegalArgumentException("not a field type");
				}
			}
			else if (MethodHandleNatives.RefKindIsMethod(refKind))
			{
				kindFlags = IS_METHOD;
				if (!(type is MethodType))
				{
					throw newIllegalArgumentException("not a method type");
				}
			}
			else if (refKind == REF_newInvokeSpecial)
			{
				kindFlags = IS_CONSTRUCTOR;
				if (!(type is MethodType) || !CONSTRUCTOR_NAME.Equals(name))
				{
					throw newIllegalArgumentException("not a constructor type or name");
				}
			}
			else
			{
				throw newIllegalArgumentException("bad reference kind " + refKind);
			}
			Init(defClass, name, type, FlagsMods(kindFlags, 0, refKind));
			InitResolved(false);
		}
		/// <summary>
		/// Query whether this member name is resolved to a non-static, non-final method.
		/// </summary>
		public bool HasReceiverTypeDispatch()
		{
			return MethodHandleNatives.RefKindDoesDispatch(ReferenceKind);
		}

		/// <summary>
		/// Query whether this member name is resolved.
		///  A resolved member name is one for which the JVM has found
		///  a method, constructor, field, or type binding corresponding exactly to the name.
		///  (Document?)
		/// </summary>
		public bool Resolved
		{
			get
			{
				return Resolution == null;
			}
		}

		private void InitResolved(bool isResolved)
		{
			assert(this.Resolution == null); // not initialized yet!
			if (!isResolved)
			{
				this.Resolution = this;
			}
			assert(Resolved == isResolved);
		}

		internal void CheckForTypeAlias()
		{
			if (Invocable)
			{
				MethodType type;
				if (this.Type_Renamed is MethodType)
				{
					type = (MethodType) this.Type_Renamed;
				}
				else
				{
					this.Type_Renamed = type = MethodType;
				}
				if (type.Erase() == type)
				{
					return;
				}
				if (VerifyAccess.isTypeVisible(type, Clazz))
				{
					return;
				}
				throw new LinkageError("bad method type alias: " + type + " not visible from " + Clazz);
			}
			else
			{
				Class type;
				if (this.Type_Renamed is Class)
				{
					type = (Class) this.Type_Renamed;
				}
				else
				{
					this.Type_Renamed = type = FieldType;
				}
				if (VerifyAccess.isTypeVisible(type, Clazz))
				{
					return;
				}
				throw new LinkageError("bad field type alias: " + type + " not visible from " + Clazz);
			}
		}


		/// <summary>
		/// Produce a string form of this member name.
		///  For types, it is simply the type's own string (as reported by {@code toString}).
		///  For fields, it is {@code "DeclaringClass.name/type"}.
		///  For methods and constructors, it is {@code "DeclaringClass.name(ptype...)rtype"}.
		///  If the declaring class is null, the prefix {@code "DeclaringClass."} is omitted.
		///  If the member is unresolved, a prefix {@code "*."} is prepended.
		/// </summary>
//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @SuppressWarnings("LocalVariableHidesMemberVariable") @Override public String toString()
		public override String ToString()
		{
			if (Type)
			{
				return Type_Renamed.ToString(); // class java.lang.String
			}
			// else it is a field, method, or constructor
			StringBuilder buf = new StringBuilder();
			if (DeclaringClass != null)
			{
				buf.Append(GetName(Clazz));
				buf.Append('.');
			}
			String name = Name;
			buf.Append(name == null ? "*" : name);
			Object type = Type;
			if (!Invocable)
			{
				buf.Append('/');
				buf.Append(type == null ? "*" : GetName(type));
			}
			else
			{
				buf.Append(type == null ? "(*)*" : GetName(type));
			}
			sbyte refKind = ReferenceKind;
			if (refKind != REF_NONE)
			{
				buf.Append('/');
				buf.Append(MethodHandleNatives.RefKindName(refKind));
			}
			//buf.append("#").append(System.identityHashCode(this));
			return buf.ToString();
		}
		private static String GetName(Object obj)
		{
			if (obj is Class)
			{
				return ((Class)obj).Name;
			}
			return Convert.ToString(obj);
		}

		public IllegalAccessException MakeAccessException(String message, Object from)
		{
			message = message + ": " + ToString();
			if (from != null)
			{
				message += ", from " + from;
			}
			return new IllegalAccessException(message);
		}
		private String Message()
		{
			if (Resolved)
			{
				return "no access";
			}
			else if (Constructor)
			{
				return "no such constructor";
			}
			else if (Method)
			{
				return "no such method";
			}
			else
			{
				return "no such field";
			}
		}
		public ReflectiveOperationException MakeAccessException()
		{
			String message = Message() + ": " + ToString();
			ReflectiveOperationException ex;
			if (Resolved || !(Resolution is NoSuchMethodError || Resolution is NoSuchFieldError))
			{
				ex = new IllegalAccessException(message);
			}
			else if (Constructor)
			{
				ex = new NoSuchMethodException(message);
			}
			else if (Method)
			{
				ex = new NoSuchMethodException(message);
			}
			else
			{
				ex = new NoSuchFieldException(message);
			}
			if (Resolution is Throwable)
			{
				ex.InitCause((Throwable) Resolution);
			}
			return ex;
		}

		/// <summary>
		/// Actually making a query requires an access check. </summary>
		/*non-public*/
	 internal static Factory Factory
	 {
		 get
		 {
				return Factory.INSTANCE;
		 }
	 }
		/// <summary>
		/// A factory type for resolving member names with the help of the VM.
		///  TBD: Define access-safe public constructors for this factory.
		/// </summary>
		/*non-public*/	 internal class Factory
	 {
			internal Factory() // singleton pattern
			{
			}
			internal static Factory INSTANCE = new Factory();

			internal static int ALLOWED_FLAGS = ALL_KINDS;

			/// Queries
			internal virtual IList<MemberName> GetMembers(Class defc, String matchName, Object matchType, int matchFlags, Class lookupClass)
			{
				matchFlags &= ALLOWED_FLAGS;
				String matchSig = null;
				if (matchType != null)
				{
					matchSig = BytecodeDescriptor.unparse(matchType);
					if (matchSig.StartsWith("("))
					{
						matchFlags &= ~(ALL_KINDS & ~IS_INVOCABLE);
					}
					else
					{
						matchFlags &= ~(ALL_KINDS & ~IS_FIELD);
					}
				}
				const int BUF_MAX = 0x2000;
				int len1 = matchName == null ? 10 : matchType == null ? 4 : 1;
				MemberName[] buf = NewMemberBuffer(len1);
				int totalCount = 0;
				List<MemberName[]> bufs = null;
				int bufCount = 0;
				for (;;)
				{
					bufCount = MethodHandleNatives.getMembers(defc, matchName, matchSig, matchFlags, lookupClass, totalCount, buf);
					if (bufCount <= buf.Length)
					{
						if (bufCount < 0)
						{
							bufCount = 0;
						}
						totalCount += bufCount;
						break;
					}
					// JVM returned to us with an intentional overflow!
					totalCount += buf.Length;
					int excess = bufCount - buf.Length;
					if (bufs == null)
					{
						bufs = new List<>(1);
					}
					bufs.Add(buf);
					int len2 = buf.Length;
					len2 = System.Math.Max(len2, excess);
					len2 = System.Math.Max(len2, totalCount / 4);
					buf = NewMemberBuffer(System.Math.Min(BUF_MAX, len2));
				}
				List<MemberName> result = new List<MemberName>(totalCount);
				if (bufs != null)
				{
					foreach (MemberName[] buf0 in bufs)
					{
						Collections.AddAll(result, buf0);
					}
				}
				result.AddRange(Arrays.AsList(buf).SubList(0, bufCount));
				// Signature matching is not the same as type matching, since
				// one signature might correspond to several types.
				// So if matchType is a Class or MethodType, refilter the results.
				if (matchType != null && matchType != matchSig)
				{
					for (IEnumerator<MemberName> it = result.GetEnumerator(); it.MoveNext();)
					{
						MemberName m = it.Current;
						if (!matchType.Equals(m.Type))
						{
							it.remove();
						}
					}
				}
				return result;
			}
			/// <summary>
			/// Produce a resolved version of the given member.
			///  Super types are searched (for inherited members) if {@code searchSupers} is true.
			///  Access checking is performed on behalf of the given {@code lookupClass}.
			///  If lookup fails or access is not permitted, null is returned.
			///  Otherwise a fresh copy of the given member is returned, with modifier bits filled in.
			/// </summary>
			internal virtual MemberName Resolve(sbyte refKind, MemberName @ref, Class lookupClass)
			{
				MemberName m = @ref.Clone(); // JVM will side-effect the ref
				assert(refKind == m.ReferenceKind);
				try
				{
					m = MethodHandleNatives.resolve(m, lookupClass);
					m.CheckForTypeAlias();
					m.Resolution = null;
				}
				catch (LinkageError ex)
				{
					// JVM reports that the "bytecode behavior" would get an error
					assert(!m.Resolved);
					m.Resolution = ex;
					return m;
				}
				assert(m.ReferenceKindIsConsistent());
				m.InitResolved(true);
				assert(m.VminfoIsConsistent());
				return m;
			}
			/// <summary>
			/// Produce a resolved version of the given member.
			///  Super types are searched (for inherited members) if {@code searchSupers} is true.
			///  Access checking is performed on behalf of the given {@code lookupClass}.
			///  If lookup fails or access is not permitted, a <seealso cref="ReflectiveOperationException"/> is thrown.
			///  Otherwise a fresh copy of the given member is returned, with modifier bits filled in.
			/// </summary>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public <NoSuchMemberException extends ReflectiveOperationException> MemberName resolveOrFail(byte refKind, MemberName m, Class lookupClass, Class nsmClass) throws IllegalAccessException, NoSuchMemberException
			public virtual MemberName resolveOrFail<NoSuchMemberException>(sbyte refKind, MemberName m, Class lookupClass, Class nsmClass) where NoSuchMemberException : ReflectiveOperationException
			{
				MemberName result = Resolve(refKind, m, lookupClass);
				if (result.Resolved)
				{
					return result;
				}
				ReflectiveOperationException ex = result.MakeAccessException();
				if (ex is IllegalAccessException)
				{
					throw (IllegalAccessException) ex;
				}
				throw nsmClass.Cast(ex);
			}
			/// <summary>
			/// Produce a resolved version of the given member.
			///  Super types are searched (for inherited members) if {@code searchSupers} is true.
			///  Access checking is performed on behalf of the given {@code lookupClass}.
			///  If lookup fails or access is not permitted, return null.
			///  Otherwise a fresh copy of the given member is returned, with modifier bits filled in.
			/// </summary>
			public virtual MemberName ResolveOrNull(sbyte refKind, MemberName m, Class lookupClass)
			{
				MemberName result = Resolve(refKind, m, lookupClass);
				if (result.Resolved)
				{
					return result;
				}
				return null;
			}
			/// <summary>
			/// Return a list of all methods defined by the given class.
			///  Super types are searched (for inherited members) if {@code searchSupers} is true.
			///  Access checking is performed on behalf of the given {@code lookupClass}.
			///  Inaccessible members are not added to the last.
			/// </summary>
			public virtual IList<MemberName> GetMethods(Class defc, bool searchSupers, Class lookupClass)
			{
				return GetMethods(defc, searchSupers, null, null, lookupClass);
			}
			/// <summary>
			/// Return a list of matching methods defined by the given class.
			///  Super types are searched (for inherited members) if {@code searchSupers} is true.
			///  Returned methods will match the name (if not null) and the type (if not null).
			///  Access checking is performed on behalf of the given {@code lookupClass}.
			///  Inaccessible members are not added to the last.
			/// </summary>
			public virtual IList<MemberName> GetMethods(Class defc, bool searchSupers, String name, MethodType type, Class lookupClass)
			{
				int matchFlags = IS_METHOD | (searchSupers ? SEARCH_ALL_SUPERS : 0);
				return GetMembers(defc, name, type, matchFlags, lookupClass);
			}
			/// <summary>
			/// Return a list of all constructors defined by the given class.
			///  Access checking is performed on behalf of the given {@code lookupClass}.
			///  Inaccessible members are not added to the last.
			/// </summary>
			public virtual IList<MemberName> GetConstructors(Class defc, Class lookupClass)
			{
				return GetMembers(defc, null, null, IS_CONSTRUCTOR, lookupClass);
			}
			/// <summary>
			/// Return a list of all fields defined by the given class.
			///  Super types are searched (for inherited members) if {@code searchSupers} is true.
			///  Access checking is performed on behalf of the given {@code lookupClass}.
			///  Inaccessible members are not added to the last.
			/// </summary>
			public virtual IList<MemberName> GetFields(Class defc, bool searchSupers, Class lookupClass)
			{
				return GetFields(defc, searchSupers, null, null, lookupClass);
			}
			/// <summary>
			/// Return a list of all fields defined by the given class.
			///  Super types are searched (for inherited members) if {@code searchSupers} is true.
			///  Returned fields will match the name (if not null) and the type (if not null).
			///  Access checking is performed on behalf of the given {@code lookupClass}.
			///  Inaccessible members are not added to the last.
			/// </summary>
			public virtual IList<MemberName> GetFields(Class defc, bool searchSupers, String name, Class type, Class lookupClass)
			{
				int matchFlags = IS_FIELD | (searchSupers ? SEARCH_ALL_SUPERS : 0);
				return GetMembers(defc, name, type, matchFlags, lookupClass);
			}
			/// <summary>
			/// Return a list of all nested types defined by the given class.
			///  Super types are searched (for inherited members) if {@code searchSupers} is true.
			///  Access checking is performed on behalf of the given {@code lookupClass}.
			///  Inaccessible members are not added to the last.
			/// </summary>
			public virtual IList<MemberName> GetNestedTypes(Class defc, bool searchSupers, Class lookupClass)
			{
				int matchFlags = IS_TYPE | (searchSupers ? SEARCH_ALL_SUPERS : 0);
				return GetMembers(defc, null, null, matchFlags, lookupClass);
			}
			internal static MemberName[] NewMemberBuffer(int length)
			{
				MemberName[] buf = new MemberName[length];
				// fill the buffer with dummy structs for the JVM to fill in
				for (int i = 0; i < length; i++)
				{
					buf[i] = new MemberName();
				}
				return buf;
			}
	 }

	//    static {
	//        System.out.println("Hello world!  My methods are:");
	//        System.out.println(Factory.INSTANCE.getMethods(MemberName.class, true, null));
	//    }
	 }

}
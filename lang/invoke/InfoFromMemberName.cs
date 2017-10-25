/*
 * Copyright (c) 2012, 2013, Oracle and/or its affiliates. All rights reserved.
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


	/*
	 * Auxiliary to MethodHandleInfo, wants to nest in MethodHandleInfo but must be non-public.
	 */
	/*non-public*/
	internal sealed class InfoFromMemberName : MethodHandleInfo
	{
		private readonly MemberName Member;
		private readonly int ReferenceKind_Renamed;

		internal InfoFromMemberName(Lookup lookup, MemberName member, sbyte referenceKind)
		{
			assert(member.Resolved || member.MethodHandleInvoke);
			assert(member.ReferenceKindIsConsistentWith(referenceKind));
			this.Member = member;
			this.ReferenceKind_Renamed = referenceKind;
		}

		public Class DeclaringClass
		{
			get
			{
				return Member.DeclaringClass;
			}
		}

		public String Name
		{
			get
			{
				return Member.Name;
			}
		}

		public MethodType MethodType
		{
			get
			{
				return Member.MethodOrFieldType;
			}
		}

		public int Modifiers
		{
			get
			{
				return Member.Modifiers;
			}
		}

		public int ReferenceKind
		{
			get
			{
				return ReferenceKind_Renamed;
			}
		}

		public override String ToString()
		{
			return MethodHandleInfo.ToString(ReferenceKind, DeclaringClass, Name, MethodType);
		}

		public T reflectAs<T>(Class expected, Lookup lookup) where T : Member
		{
			if (Member.MethodHandleInvoke && !Member.Varargs)
			{
				// This member is an instance of a signature-polymorphic method, which cannot be reflected
				// A method handle invoker can come in either of two forms:
				// A generic placeholder (present in the source code, and varargs)
				// and a signature-polymorphic instance (synthetic and not varargs).
				// For more information see comments on {@link MethodHandleNatives#linkMethod}.
				throw new IllegalArgumentException("cannot reflect signature polymorphic method");
			}
			Member mem = AccessController.doPrivileged(new PrivilegedActionAnonymousInnerClassHelper(this));
			try
			{
				Class defc = DeclaringClass;
				sbyte refKind = (sbyte) ReferenceKind;
				lookup.checkAccess(refKind, defc, ConvertToMemberName(refKind, mem));
			}
			catch (IllegalAccessException ex)
			{
				throw new IllegalArgumentException(ex);
			}
			return expected.Cast(mem);
		}

		private class PrivilegedActionAnonymousInnerClassHelper : PrivilegedAction<Member>
		{
			private readonly InfoFromMemberName OuterInstance;

			public PrivilegedActionAnonymousInnerClassHelper(InfoFromMemberName outerInstance)
			{
				this.OuterInstance = outerInstance;
			}

			public virtual Member Run()
			{
				try
				{
					return outerInstance.ReflectUnchecked();
				}
				catch (ReflectiveOperationException ex)
				{
					throw new IllegalArgumentException(ex);
				}
			}
		}

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: private Member reflectUnchecked() throws ReflectiveOperationException
		private Member ReflectUnchecked()
		{
			sbyte refKind = (sbyte) ReferenceKind;
			Class defc = DeclaringClass;
			bool isPublic = Modifier.IsPublic(Modifiers);
			if (MethodHandleNatives.RefKindIsMethod(refKind))
			{
				if (isPublic)
				{
					return defc.GetMethod(Name, MethodType.ParameterArray());
				}
				else
				{
					return defc.GetDeclaredMethod(Name, MethodType.ParameterArray());
				}
			}
			else if (MethodHandleNatives.RefKindIsConstructor(refKind))
			{
				if (isPublic)
				{
					return defc.GetConstructor(MethodType.ParameterArray());
				}
				else
				{
					return defc.GetDeclaredConstructor(MethodType.ParameterArray());
				}
			}
			else if (MethodHandleNatives.RefKindIsField(refKind))
			{
				if (isPublic)
				{
					return defc.GetField(Name);
				}
				else
				{
					return defc.GetDeclaredField(Name);
				}
			}
			else
			{
				throw new IllegalArgumentException("referenceKind=" + refKind);
			}
		}

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: private static MemberName convertToMemberName(byte refKind, Member mem) throws IllegalAccessException
		private static MemberName ConvertToMemberName(sbyte refKind, Member mem)
		{
			if (mem is Method)
			{
				bool wantSpecial = (refKind == REF_invokeSpecial);
				return new MemberName((Method) mem, wantSpecial);
			}
			else if (mem is Constructor)
			{
				return new MemberName((Constructor) mem);
			}
			else if (mem is Field)
			{
				bool isSetter = (refKind == REF_putField || refKind == REF_putStatic);
				return new MemberName((Field) mem, isSetter);
			}
//JAVA TO C# CONVERTER WARNING: The .NET Type.FullName property will not always yield results identical to the Java Class.getName method:
			throw new InternalError(mem.GetType().FullName);
		}
	}

}
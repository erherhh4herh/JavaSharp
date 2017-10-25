/*
 * Copyright (c) 2013, Oracle and/or its affiliates. All rights reserved.
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

namespace java.beans
{


//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to .NET:
	import static sun.reflect.misc.ReflectUtil.isPackageAccessible;

	internal sealed class MethodRef
	{
		private String Signature;
		private SoftReference<Method> MethodRef;
		private WeakReference<Class> TypeRef;

		internal void Set(Method method)
		{
			if (method == null)
			{
				this.Signature = null;
				this.MethodRef = null;
				this.TypeRef = null;
			}
			else
			{
				this.Signature = method.toGenericString();
				this.MethodRef = new SoftReference<>(method);
				this.TypeRef = new WeakReference<Class>(method.DeclaringClass);
			}
		}

		internal bool Set
		{
			get
			{
				return this.MethodRef != null;
			}
		}

		internal Method Get()
		{
			if (this.MethodRef == null)
			{
				return null;
			}
			Method method = this.MethodRef.get();
			if (method == null)
			{
				method = Find(this.TypeRef.get(), this.Signature);
				if (method == null)
				{
					this.Signature = null;
					this.MethodRef = null;
					this.TypeRef = null;
				}
				else
				{
					this.MethodRef = new SoftReference<>(method);
				}
			}
			return isPackageAccessible(method.DeclaringClass) ? method : null;
		}

		private static Method Find(Class type, String signature)
		{
			if (type != null)
			{
				foreach (Method method in type.Methods)
				{
					if (type.Equals(method.DeclaringClass))
					{
						if (method.toGenericString().Equals(signature))
						{
							return method;
						}
					}
				}
			}
			return null;
		}
	}

}
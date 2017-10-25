/*
 * Copyright (c) 2001, 2013, Oracle and/or its affiliates. All rights reserved.
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

namespace java.lang.reflect
{

	using MethodAccessor = sun.reflect.MethodAccessor;
	using ConstructorAccessor = sun.reflect.ConstructorAccessor;

	/// <summary>
	/// Package-private class implementing the
	///    sun.reflect.LangReflectAccess interface, allowing the java.lang
	///    package to instantiate objects in this package. 
	/// </summary>

	internal class ReflectAccess : sun.reflect.LangReflectAccess
	{
		public virtual Field NewField(Class declaringClass, String name, Class type, int modifiers, int slot, String signature, sbyte[] annotations)
		{
			return new Field(declaringClass, name, type, modifiers, slot, signature, annotations);
		}

		public virtual Method NewMethod(Class declaringClass, String name, Class[] parameterTypes, Class returnType, Class[] checkedExceptions, int modifiers, int slot, String signature, sbyte[] annotations, sbyte[] parameterAnnotations, sbyte[] annotationDefault)
		{
			return new Method(declaringClass, name, parameterTypes, returnType, checkedExceptions, modifiers, slot, signature, annotations, parameterAnnotations, annotationDefault);
		}

		public virtual Constructor<T> newConstructor<T>(Class declaringClass, Class[] parameterTypes, Class[] checkedExceptions, int modifiers, int slot, String signature, sbyte[] annotations, sbyte[] parameterAnnotations)
		{
			return new Constructor<>(declaringClass, parameterTypes, checkedExceptions, modifiers, slot, signature, annotations, parameterAnnotations);
		}

		public virtual MethodAccessor GetMethodAccessor(Method m)
		{
			return m.MethodAccessor;
		}

		public virtual void SetMethodAccessor(Method m, MethodAccessor accessor)
		{
			m.MethodAccessor = accessor;
		}

		public virtual ConstructorAccessor getConstructorAccessor<T1>(Constructor<T1> c)
		{
			return c.ConstructorAccessor;
		}

		public virtual void setConstructorAccessor<T1>(Constructor<T1> c, ConstructorAccessor accessor)
		{
			c.ConstructorAccessor = accessor;
		}

		public virtual int getConstructorSlot<T1>(Constructor<T1> c)
		{
			return c.Slot;
		}

		public virtual String getConstructorSignature<T1>(Constructor<T1> c)
		{
			return c.Signature;
		}

		public virtual sbyte[] getConstructorAnnotations<T1>(Constructor<T1> c)
		{
			return c.RawAnnotations;
		}

		public virtual sbyte[] getConstructorParameterAnnotations<T1>(Constructor<T1> c)
		{
			return c.RawParameterAnnotations;
		}

		public virtual sbyte[] GetExecutableTypeAnnotationBytes(Executable ex)
		{
			return ex.TypeAnnotationBytes;
		}

		//
		// Copying routines, needed to quickly fabricate new Field,
		// Method, and Constructor objects from templates
		//
		public virtual Method CopyMethod(Method arg)
		{
			return arg.Copy();
		}

		public virtual Field CopyField(Field arg)
		{
			return arg.Copy();
		}

		public virtual Constructor<T> copyConstructor<T>(Constructor<T> arg)
		{
			return arg.Copy();
		}
	}

}
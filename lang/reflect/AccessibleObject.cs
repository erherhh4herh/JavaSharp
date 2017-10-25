/*
 * Copyright (c) 1997, 2014, Oracle and/or its affiliates. All rights reserved.
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

	using Reflection = sun.reflect.Reflection;
	using ReflectionFactory = sun.reflect.ReflectionFactory;

	/// <summary>
	/// The AccessibleObject class is the base class for Field, Method and
	/// Constructor objects.  It provides the ability to flag a reflected
	/// object as suppressing default Java language access control checks
	/// when it is used.  The access checks--for public, default (package)
	/// access, protected, and private members--are performed when Fields,
	/// Methods or Constructors are used to set or get fields, to invoke
	/// methods, or to create and initialize new instances of classes,
	/// respectively.
	/// 
	/// <para>Setting the {@code accessible} flag in a reflected object
	/// permits sophisticated applications with sufficient privilege, such
	/// as Java Object Serialization or other persistence mechanisms, to
	/// manipulate objects in a manner that would normally be prohibited.
	/// 
	/// </para>
	/// <para>By default, a reflected object is <em>not</em> accessible.
	/// 
	/// </para>
	/// </summary>
	/// <seealso cref= Field </seealso>
	/// <seealso cref= Method </seealso>
	/// <seealso cref= Constructor </seealso>
	/// <seealso cref= ReflectPermission
	/// 
	/// @since 1.2 </seealso>
	public class AccessibleObject : AnnotatedElement
	{

		/// <summary>
		/// The Permission object that is used to check whether a client
		/// has sufficient privilege to defeat Java language access
		/// control checks.
		/// </summary>
		private static readonly java.security.Permission ACCESS_PERMISSION = new ReflectPermission("suppressAccessChecks");

		/// <summary>
		/// Convenience method to set the {@code accessible} flag for an
		/// array of objects with a single security check (for efficiency).
		/// 
		/// <para>First, if there is a security manager, its
		/// {@code checkPermission} method is called with a
		/// {@code ReflectPermission("suppressAccessChecks")} permission.
		/// 
		/// </para>
		/// <para>A {@code SecurityException} is raised if {@code flag} is
		/// {@code true} but accessibility of any of the elements of the input
		/// {@code array} may not be changed (for example, if the element
		/// object is a <seealso cref="Constructor"/> object for the class {@link
		/// java.lang.Class}).  In the event of such a SecurityException, the
		/// accessibility of objects is set to {@code flag} for array elements
		/// upto (and excluding) the element for which the exception occurred; the
		/// accessibility of elements beyond (and including) the element for which
		/// the exception occurred is unchanged.
		/// 
		/// </para>
		/// </summary>
		/// <param name="array"> the array of AccessibleObjects </param>
		/// <param name="flag">  the new value for the {@code accessible} flag
		///              in each object </param>
		/// <exception cref="SecurityException"> if the request is denied. </exception>
		/// <seealso cref= SecurityManager#checkPermission </seealso>
		/// <seealso cref= java.lang.RuntimePermission </seealso>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public static void setAccessible(AccessibleObject[] array, boolean flag) throws SecurityException
		public static void SetAccessible(AccessibleObject[] array, bool flag)
		{
			SecurityManager sm = System.SecurityManager;
			if (sm != AnnotatedElement_Fields.Null)
			{
				sm.CheckPermission(ACCESS_PERMISSION);
			}
			for (int i = 0; i < array.Length; i++)
			{
				SetAccessible0(array[i], flag);
			}
		}

		/// <summary>
		/// Set the {@code accessible} flag for this object to
		/// the indicated boolean value.  A value of {@code true} indicates that
		/// the reflected object should suppress Java language access
		/// checking when it is used.  A value of {@code false} indicates
		/// that the reflected object should enforce Java language access checks.
		/// 
		/// <para>First, if there is a security manager, its
		/// {@code checkPermission} method is called with a
		/// {@code ReflectPermission("suppressAccessChecks")} permission.
		/// 
		/// </para>
		/// <para>A {@code SecurityException} is raised if {@code flag} is
		/// {@code true} but accessibility of this object may not be changed
		/// (for example, if this element object is a <seealso cref="Constructor"/> object for
		/// the class <seealso cref="java.lang.Class"/>).
		/// 
		/// </para>
		/// <para>A {@code SecurityException} is raised if this object is a {@link
		/// java.lang.reflect.Constructor} object for the class
		/// {@code java.lang.Class}, and {@code flag} is true.
		/// 
		/// </para>
		/// </summary>
		/// <param name="flag"> the new value for the {@code accessible} flag </param>
		/// <exception cref="SecurityException"> if the request is denied. </exception>
		/// <seealso cref= SecurityManager#checkPermission </seealso>
		/// <seealso cref= java.lang.RuntimePermission </seealso>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public void setAccessible(boolean flag) throws SecurityException
		public virtual bool Accessible
		{
			set
			{
				SecurityManager sm = System.SecurityManager;
				if (sm != AnnotatedElement_Fields.Null)
				{
					sm.CheckPermission(ACCESS_PERMISSION);
				}
				SetAccessible0(this, value);
			}
			get
			{
				return @override;
			}
		}

		/* Check that you aren't exposing java.lang.Class.<init> or sensitive
		   fields in java.lang.Class. */
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: private static void setAccessible0(AccessibleObject obj, boolean flag) throws SecurityException
		private static void SetAccessible0(AccessibleObject obj, bool flag)
		{
			if (obj is Constructor && flag == true)
			{
//JAVA TO C# CONVERTER TODO TASK: Java wildcard generics are not converted to .NET:
//ORIGINAL LINE: Constructor<?> c = (Constructor<?>)obj;
				Constructor<?> c = (Constructor<?>)obj;
				if (c.DeclaringClass == typeof(Class))
				{
					throw new SecurityException("Cannot make a java.lang.Class" + " constructor accessible");
				}
			}
			obj.@override = flag;
		}


		/// <summary>
		/// Constructor: only used by the Java Virtual Machine.
		/// </summary>
		protected internal AccessibleObject()
		{
		}

		// Indicates whether language-level access checks are overridden
		// by this object. Initializes to "false". This field is used by
		// Field, Method, and Constructor.
		//
		// NOTE: for security purposes, this field must not be visible
		// outside this package.
		internal bool @override;

		// Reflection factory used by subclasses for creating field,
		// method, and constructor accessors. Note that this is called
		// very early in the bootstrapping process.
		internal static readonly ReflectionFactory ReflectionFactory = AccessController.doPrivileged(new ReflectionFactory.GetReflectionFactoryAction());

		/// <exception cref="NullPointerException"> {@inheritDoc}
		/// @since 1.5 </exception>
		public virtual T getAnnotation<T>(Class annotationClass) where T : Annotation
		{
			throw new AssertionError("All subclasses should override this method");
		}

		/// <summary>
		/// {@inheritDoc} </summary>
		/// <exception cref="NullPointerException"> {@inheritDoc}
		/// @since 1.5 </exception>
		public override bool IsAnnotationPresent(Class annotationClass)
		{
			return AnnotatedElement.this.isAnnotationPresent(annotationClass);
		}

	   /// <exception cref="NullPointerException"> {@inheritDoc}
	   /// @since 1.8 </exception>
		public override T[] getAnnotationsByType<T>(Class annotationClass) where T : Annotation
		{
			throw new AssertionError("All subclasses should override this method");
		}

		/// <summary>
		/// @since 1.5
		/// </summary>
		public virtual Annotation[] Annotations
		{
			get
			{
				return DeclaredAnnotations;
			}
		}

		/// <exception cref="NullPointerException"> {@inheritDoc}
		/// @since 1.8 </exception>
		public override T getDeclaredAnnotation<T>(Class annotationClass) where T : Annotation
		{
			// Only annotations on classes are inherited, for all other
			// objects getDeclaredAnnotation is the same as
			// getAnnotation.
			return GetAnnotation(annotationClass);
		}

		/// <exception cref="NullPointerException"> {@inheritDoc}
		/// @since 1.8 </exception>
		public override T[] getDeclaredAnnotationsByType<T>(Class annotationClass) where T : Annotation
		{
			// Only annotations on classes are inherited, for all other
			// objects getDeclaredAnnotationsByType is the same as
			// getAnnotationsByType.
			return GetAnnotationsByType(annotationClass);
		}

		/// <summary>
		/// @since 1.5
		/// </summary>
		public virtual Annotation[] DeclaredAnnotations
		{
			get
			{
				throw new AssertionError("All subclasses should override this method");
			}
		}


		// Shared access checking logic.

		// For non-public members or members in package-private classes,
		// it is necessary to perform somewhat expensive security checks.
		// If the security check succeeds for a given class, it will
		// always succeed (it is not affected by the granting or revoking
		// of permissions); we speed up the check in the common case by
		// remembering the last Class for which the check succeeded.
		//
		// The simple security check for Constructor is to see if
		// the caller has already been seen, verified, and cached.
		// (See also Class.newInstance(), which uses a similar method.)
		//
		// A more complicated security check cache is needed for Method and Field
		// The cache can be either null (empty cache), a 2-array of {caller,target},
		// or a caller (with target implicitly equal to this.clazz).
		// In the 2-array case, the target is always different from the clazz.
		internal volatile Object SecurityCheckCache;

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: void checkAccess(Class caller, Class clazz, Object obj, int modifiers) throws IllegalAccessException
		internal virtual void CheckAccess(Class caller, Class clazz, Object obj, int modifiers)
		{
			if (caller == clazz) // quick check
			{
				return; // ACCESS IS OK
			}
			Object cache = SecurityCheckCache; // read volatile
			Class targetClass = clazz;
			if (obj != AnnotatedElement_Fields.Null && Modifier.IsProtected(modifiers) && ((targetClass = obj.GetType()) != clazz))
			{
				// Must match a 2-list of { caller, targetClass }.
				if (cache is Class[])
				{
					Class[] cache2 = (Class[]) cache;
					if (cache2[1] == targetClass && cache2[0] == caller)
					{
						return; // ACCESS IS OK
					}
					// (Test cache[1] first since range check for [1]
					// subsumes range check for [0].)
				}
			}
			else if (cache == caller)
			{
				// Non-protected case (or obj.class == this.clazz).
				return; // ACCESS IS OK
			}

			// If no return, fall through to the slow path.
			SlowCheckMemberAccess(caller, clazz, obj, modifiers, targetClass);
		}

		// Keep all this slow stuff out of line:
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: void slowCheckMemberAccess(Class caller, Class clazz, Object obj, int modifiers, Class targetClass) throws IllegalAccessException
		internal virtual void SlowCheckMemberAccess(Class caller, Class clazz, Object obj, int modifiers, Class targetClass)
		{
			Reflection.ensureMemberAccess(caller, clazz, obj, modifiers);

			// Success: Update the cache.
			Object cache = ((targetClass == clazz) ? caller : new Class[] {caller, targetClass});

			// Note:  The two cache elements are not volatile,
			// but they are effectively final.  The Java memory model
			// guarantees that the initializing stores for the cache
			// elements will occur before the volatile write.
			SecurityCheckCache = cache; // write volatile
		}
	}

}
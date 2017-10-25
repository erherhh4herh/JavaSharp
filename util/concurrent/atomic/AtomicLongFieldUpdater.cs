using System;

/*
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

/*
 *
 *
 *
 *
 *
 * Written by Doug Lea with assistance from members of JCP JSR-166
 * Expert Group and released to the public domain, as explained at
 * http://creativecommons.org/publicdomain/zero/1.0/
 */

namespace java.util.concurrent.atomic
{
	using Unsafe = sun.misc.Unsafe;
	using CallerSensitive = sun.reflect.CallerSensitive;
	using Reflection = sun.reflect.Reflection;

	/// <summary>
	/// A reflection-based utility that enables atomic updates to
	/// designated {@code volatile long} fields of designated classes.
	/// This class is designed for use in atomic data structures in which
	/// several fields of the same node are independently subject to atomic
	/// updates.
	/// 
	/// <para>Note that the guarantees of the {@code compareAndSet}
	/// method in this class are weaker than in other atomic classes.
	/// Because this class cannot ensure that all uses of the field
	/// are appropriate for purposes of atomic access, it can
	/// guarantee atomicity only with respect to other invocations of
	/// {@code compareAndSet} and {@code set} on the same updater.
	/// 
	/// @since 1.5
	/// @author Doug Lea
	/// </para>
	/// </summary>
	/// @param <T> The type of the object holding the updatable field </param>
	public abstract class AtomicLongFieldUpdater<T>
	{
		/// <summary>
		/// Creates and returns an updater for objects with the given field.
		/// The Class argument is needed to check that reflective types and
		/// generic types match.
		/// </summary>
		/// <param name="tclass"> the class of the objects holding the field </param>
		/// <param name="fieldName"> the name of the field to be updated </param>
		/// @param <U> the type of instances of tclass </param>
		/// <returns> the updater </returns>
		/// <exception cref="IllegalArgumentException"> if the field is not a
		/// volatile long type </exception>
		/// <exception cref="RuntimeException"> with a nested reflection-based
		/// exception if the class does not hold field or is the wrong type,
		/// or the field is inaccessible to the caller according to Java language
		/// access control </exception>
//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @CallerSensitive public static <U> AtomicLongFieldUpdater<U> newUpdater(Class tclass, String fieldName)
		public static AtomicLongFieldUpdater<U> newUpdater<U>(Class tclass, String fieldName)
		{
			Class caller = Reflection.CallerClass;
			if (AtomicLong.VM_SUPPORTS_LONG_CAS)
			{
				return new CASUpdater<U>(tclass, fieldName, caller);
			}
			else
			{
				return new LockedUpdater<U>(tclass, fieldName, caller);
			}
		}

		/// <summary>
		/// Protected do-nothing constructor for use by subclasses.
		/// </summary>
		protected internal AtomicLongFieldUpdater()
		{
		}

		/// <summary>
		/// Atomically sets the field of the given object managed by this updater
		/// to the given updated value if the current value {@code ==} the
		/// expected value. This method is guaranteed to be atomic with respect to
		/// other calls to {@code compareAndSet} and {@code set}, but not
		/// necessarily with respect to other changes in the field.
		/// </summary>
		/// <param name="obj"> An object whose field to conditionally set </param>
		/// <param name="expect"> the expected value </param>
		/// <param name="update"> the new value </param>
		/// <returns> {@code true} if successful </returns>
		/// <exception cref="ClassCastException"> if {@code obj} is not an instance
		/// of the class possessing the field established in the constructor </exception>
		public abstract bool CompareAndSet(T obj, long expect, long update);

		/// <summary>
		/// Atomically sets the field of the given object managed by this updater
		/// to the given updated value if the current value {@code ==} the
		/// expected value. This method is guaranteed to be atomic with respect to
		/// other calls to {@code compareAndSet} and {@code set}, but not
		/// necessarily with respect to other changes in the field.
		/// 
		/// <para><a href="package-summary.html#weakCompareAndSet">May fail
		/// spuriously and does not provide ordering guarantees</a>, so is
		/// only rarely an appropriate alternative to {@code compareAndSet}.
		/// 
		/// </para>
		/// </summary>
		/// <param name="obj"> An object whose field to conditionally set </param>
		/// <param name="expect"> the expected value </param>
		/// <param name="update"> the new value </param>
		/// <returns> {@code true} if successful </returns>
		/// <exception cref="ClassCastException"> if {@code obj} is not an instance
		/// of the class possessing the field established in the constructor </exception>
		public abstract bool WeakCompareAndSet(T obj, long expect, long update);

		/// <summary>
		/// Sets the field of the given object managed by this updater to the
		/// given updated value. This operation is guaranteed to act as a volatile
		/// store with respect to subsequent invocations of {@code compareAndSet}.
		/// </summary>
		/// <param name="obj"> An object whose field to set </param>
		/// <param name="newValue"> the new value </param>
		public abstract void Set(T obj, long newValue);

		/// <summary>
		/// Eventually sets the field of the given object managed by this
		/// updater to the given updated value.
		/// </summary>
		/// <param name="obj"> An object whose field to set </param>
		/// <param name="newValue"> the new value
		/// @since 1.6 </param>
		public abstract void LazySet(T obj, long newValue);

		/// <summary>
		/// Gets the current value held in the field of the given object managed
		/// by this updater.
		/// </summary>
		/// <param name="obj"> An object whose field to get </param>
		/// <returns> the current value </returns>
		public abstract long Get(T obj);

		/// <summary>
		/// Atomically sets the field of the given object managed by this updater
		/// to the given value and returns the old value.
		/// </summary>
		/// <param name="obj"> An object whose field to get and set </param>
		/// <param name="newValue"> the new value </param>
		/// <returns> the previous value </returns>
		public virtual long GetAndSet(T obj, long newValue)
		{
			long prev;
			do
			{
				prev = Get(obj);
			} while (!CompareAndSet(obj, prev, newValue));
			return prev;
		}

		/// <summary>
		/// Atomically increments by one the current value of the field of the
		/// given object managed by this updater.
		/// </summary>
		/// <param name="obj"> An object whose field to get and set </param>
		/// <returns> the previous value </returns>
		public virtual long GetAndIncrement(T obj)
		{
			long prev, next;
			do
			{
				prev = Get(obj);
				next = prev + 1;
			} while (!CompareAndSet(obj, prev, next));
			return prev;
		}

		/// <summary>
		/// Atomically decrements by one the current value of the field of the
		/// given object managed by this updater.
		/// </summary>
		/// <param name="obj"> An object whose field to get and set </param>
		/// <returns> the previous value </returns>
		public virtual long GetAndDecrement(T obj)
		{
			long prev, next;
			do
			{
				prev = Get(obj);
				next = prev - 1;
			} while (!CompareAndSet(obj, prev, next));
			return prev;
		}

		/// <summary>
		/// Atomically adds the given value to the current value of the field of
		/// the given object managed by this updater.
		/// </summary>
		/// <param name="obj"> An object whose field to get and set </param>
		/// <param name="delta"> the value to add </param>
		/// <returns> the previous value </returns>
		public virtual long GetAndAdd(T obj, long delta)
		{
			long prev, next;
			do
			{
				prev = Get(obj);
				next = prev + delta;
			} while (!CompareAndSet(obj, prev, next));
			return prev;
		}

		/// <summary>
		/// Atomically increments by one the current value of the field of the
		/// given object managed by this updater.
		/// </summary>
		/// <param name="obj"> An object whose field to get and set </param>
		/// <returns> the updated value </returns>
		public virtual long IncrementAndGet(T obj)
		{
			long prev, next;
			do
			{
				prev = Get(obj);
				next = prev + 1;
			} while (!CompareAndSet(obj, prev, next));
			return next;
		}

		/// <summary>
		/// Atomically decrements by one the current value of the field of the
		/// given object managed by this updater.
		/// </summary>
		/// <param name="obj"> An object whose field to get and set </param>
		/// <returns> the updated value </returns>
		public virtual long DecrementAndGet(T obj)
		{
			long prev, next;
			do
			{
				prev = Get(obj);
				next = prev - 1;
			} while (!CompareAndSet(obj, prev, next));
			return next;
		}

		/// <summary>
		/// Atomically adds the given value to the current value of the field of
		/// the given object managed by this updater.
		/// </summary>
		/// <param name="obj"> An object whose field to get and set </param>
		/// <param name="delta"> the value to add </param>
		/// <returns> the updated value </returns>
		public virtual long AddAndGet(T obj, long delta)
		{
			long prev, next;
			do
			{
				prev = Get(obj);
				next = prev + delta;
			} while (!CompareAndSet(obj, prev, next));
			return next;
		}

		/// <summary>
		/// Atomically updates the field of the given object managed by this updater
		/// with the results of applying the given function, returning the previous
		/// value. The function should be side-effect-free, since it may be
		/// re-applied when attempted updates fail due to contention among threads.
		/// </summary>
		/// <param name="obj"> An object whose field to get and set </param>
		/// <param name="updateFunction"> a side-effect-free function </param>
		/// <returns> the previous value
		/// @since 1.8 </returns>
		public long GetAndUpdate(T obj, LongUnaryOperator updateFunction)
		{
			long prev, next;
			do
			{
				prev = Get(obj);
				next = updateFunction.ApplyAsLong(prev);
			} while (!CompareAndSet(obj, prev, next));
			return prev;
		}

		/// <summary>
		/// Atomically updates the field of the given object managed by this updater
		/// with the results of applying the given function, returning the updated
		/// value. The function should be side-effect-free, since it may be
		/// re-applied when attempted updates fail due to contention among threads.
		/// </summary>
		/// <param name="obj"> An object whose field to get and set </param>
		/// <param name="updateFunction"> a side-effect-free function </param>
		/// <returns> the updated value
		/// @since 1.8 </returns>
		public long UpdateAndGet(T obj, LongUnaryOperator updateFunction)
		{
			long prev, next;
			do
			{
				prev = Get(obj);
				next = updateFunction.ApplyAsLong(prev);
			} while (!CompareAndSet(obj, prev, next));
			return next;
		}

		/// <summary>
		/// Atomically updates the field of the given object managed by this
		/// updater with the results of applying the given function to the
		/// current and given values, returning the previous value. The
		/// function should be side-effect-free, since it may be re-applied
		/// when attempted updates fail due to contention among threads.  The
		/// function is applied with the current value as its first argument,
		/// and the given update as the second argument.
		/// </summary>
		/// <param name="obj"> An object whose field to get and set </param>
		/// <param name="x"> the update value </param>
		/// <param name="accumulatorFunction"> a side-effect-free function of two arguments </param>
		/// <returns> the previous value
		/// @since 1.8 </returns>
		public long GetAndAccumulate(T obj, long x, LongBinaryOperator accumulatorFunction)
		{
			long prev, next;
			do
			{
				prev = Get(obj);
				next = accumulatorFunction.ApplyAsLong(prev, x);
			} while (!CompareAndSet(obj, prev, next));
			return prev;
		}

		/// <summary>
		/// Atomically updates the field of the given object managed by this
		/// updater with the results of applying the given function to the
		/// current and given values, returning the updated value. The
		/// function should be side-effect-free, since it may be re-applied
		/// when attempted updates fail due to contention among threads.  The
		/// function is applied with the current value as its first argument,
		/// and the given update as the second argument.
		/// </summary>
		/// <param name="obj"> An object whose field to get and set </param>
		/// <param name="x"> the update value </param>
		/// <param name="accumulatorFunction"> a side-effect-free function of two arguments </param>
		/// <returns> the updated value
		/// @since 1.8 </returns>
		public long AccumulateAndGet(T obj, long x, LongBinaryOperator accumulatorFunction)
		{
			long prev, next;
			do
			{
				prev = Get(obj);
				next = accumulatorFunction.ApplyAsLong(prev, x);
			} while (!CompareAndSet(obj, prev, next));
			return next;
		}

		private class CASUpdater<T> : AtomicLongFieldUpdater<T>
		{
			internal static readonly Unsafe @unsafe = Unsafe.Unsafe;
			internal readonly long Offset;
			internal readonly Class Tclass;
			internal readonly Class Cclass;

//JAVA TO C# CONVERTER WARNING: 'final' parameters are not allowed in .NET:
//ORIGINAL LINE: CASUpdater(final Class tclass, final String fieldName, final Class caller)
			internal CASUpdater(Class tclass, String fieldName, Class caller)
			{
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final Field field;
				Field field;
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final int modifiers;
				int modifiers;
				try
				{
					field = AccessController.doPrivileged(new PrivilegedExceptionActionAnonymousInnerClassHelper(this, tclass, fieldName));
					modifiers = field.Modifiers;
					sun.reflect.misc.ReflectUtil.ensureMemberAccess(caller, tclass, null, modifiers);
					ClassLoader cl = tclass.ClassLoader;
					ClassLoader ccl = caller.ClassLoader;
					if ((ccl != null) && (ccl != cl) && ((cl == null) || !IsAncestor(cl, ccl)))
					{
					  sun.reflect.misc.ReflectUtil.checkPackageAccess(tclass);
					}
				}
				catch (PrivilegedActionException pae)
				{
					throw new RuntimeException(pae.Exception);
				}
				catch (Exception ex)
				{
					throw new RuntimeException(ex);
				}

				Class fieldt = field.Type;
				if (fieldt != typeof(long))
				{
					throw new IllegalArgumentException("Must be long type");
				}

				if (!Modifier.isVolatile(modifiers))
				{
					throw new IllegalArgumentException("Must be volatile type");
				}

				this.Cclass = (Modifier.isProtected(modifiers) && caller != tclass) ? caller : null;
				this.Tclass = tclass;
				Offset = @unsafe.objectFieldOffset(field);
			}

			private class PrivilegedExceptionActionAnonymousInnerClassHelper : PrivilegedExceptionAction<Field>
			{
				private readonly CASUpdater<T> OuterInstance;

				private Type Tclass;
				private string FieldName;

				public PrivilegedExceptionActionAnonymousInnerClassHelper(CASUpdater<T> outerInstance, Type tclass, string fieldName)
				{
					this.outerInstance = outerInstance;
					this.Tclass = tclass;
					this.FieldName = fieldName;
				}

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public Field run() throws NoSuchFieldException
				public virtual Field Run()
				{
					return Tclass.GetDeclaredField(FieldName);
				}
			}

			internal virtual void FullCheck(T obj)
			{
				if (!Tclass.isInstance(obj))
				{
					throw new ClassCastException();
				}
				if (Cclass != null)
				{
					EnsureProtectedAccess(obj);
				}
			}

			public virtual bool CompareAndSet(T obj, long expect, long update)
			{
				if (obj == null || obj.GetType() != Tclass || Cclass != null)
				{
					FullCheck(obj);
				}
				return @unsafe.compareAndSwapLong(obj, Offset, expect, update);
			}

			public virtual bool WeakCompareAndSet(T obj, long expect, long update)
			{
				if (obj == null || obj.GetType() != Tclass || Cclass != null)
				{
					FullCheck(obj);
				}
				return @unsafe.compareAndSwapLong(obj, Offset, expect, update);
			}

			public virtual void Set(T obj, long newValue)
			{
				if (obj == null || obj.GetType() != Tclass || Cclass != null)
				{
					FullCheck(obj);
				}
				@unsafe.putLongVolatile(obj, Offset, newValue);
			}

			public virtual void LazySet(T obj, long newValue)
			{
				if (obj == null || obj.GetType() != Tclass || Cclass != null)
				{
					FullCheck(obj);
				}
				@unsafe.putOrderedLong(obj, Offset, newValue);
			}

			public virtual long Get(T obj)
			{
				if (obj == null || obj.GetType() != Tclass || Cclass != null)
				{
					FullCheck(obj);
				}
				return @unsafe.getLongVolatile(obj, Offset);
			}

			public virtual long GetAndSet(T obj, long newValue)
			{
				if (obj == null || obj.GetType() != Tclass || Cclass != null)
				{
					FullCheck(obj);
				}
				return @unsafe.getAndSetLong(obj, Offset, newValue);
			}

			public virtual long GetAndIncrement(T obj)
			{
				return GetAndAdd(obj, 1);
			}

			public virtual long GetAndDecrement(T obj)
			{
				return GetAndAdd(obj, -1);
			}

			public virtual long GetAndAdd(T obj, long delta)
			{
				if (obj == null || obj.GetType() != Tclass || Cclass != null)
				{
					FullCheck(obj);
				}
				return @unsafe.getAndAddLong(obj, Offset, delta);
			}

			public virtual long IncrementAndGet(T obj)
			{
				return GetAndAdd(obj, 1) + 1;
			}

			public virtual long DecrementAndGet(T obj)
			{
				 return GetAndAdd(obj, -1) - 1;
			}

			public virtual long AddAndGet(T obj, long delta)
			{
				return GetAndAdd(obj, delta) + delta;
			}

			internal virtual void EnsureProtectedAccess(T obj)
			{
				if (Cclass.isInstance(obj))
				{
					return;
				}
//JAVA TO C# CONVERTER WARNING: The .NET Type.FullName property will not always yield results identical to the Java Class.getName method:
				throw new RuntimeException(new IllegalAccessException("Class " + Cclass.Name + " can not access a protected member of class " + Tclass.Name + " using an instance of " + obj.GetType().FullName)
			   );
			}
		}


		private class LockedUpdater<T> : AtomicLongFieldUpdater<T>
		{
			internal static readonly Unsafe @unsafe = Unsafe.Unsafe;
			internal readonly long Offset;
			internal readonly Class Tclass;
			internal readonly Class Cclass;

//JAVA TO C# CONVERTER WARNING: 'final' parameters are not allowed in .NET:
//ORIGINAL LINE: LockedUpdater(final Class tclass, final String fieldName, final Class caller)
			internal LockedUpdater(Class tclass, String fieldName, Class caller)
			{
				Field field = null;
				int modifiers = 0;
				try
				{
					field = AccessController.doPrivileged(new PrivilegedExceptionActionAnonymousInnerClassHelper(this, tclass, fieldName));
					modifiers = field.Modifiers;
					sun.reflect.misc.ReflectUtil.ensureMemberAccess(caller, tclass, null, modifiers);
					ClassLoader cl = tclass.ClassLoader;
					ClassLoader ccl = caller.ClassLoader;
					if ((ccl != null) && (ccl != cl) && ((cl == null) || !IsAncestor(cl, ccl)))
					{
					  sun.reflect.misc.ReflectUtil.checkPackageAccess(tclass);
					}
				}
				catch (PrivilegedActionException pae)
				{
					throw new RuntimeException(pae.Exception);
				}
				catch (Exception ex)
				{
					throw new RuntimeException(ex);
				}

				Class fieldt = field.Type;
				if (fieldt != typeof(long))
				{
					throw new IllegalArgumentException("Must be long type");
				}

				if (!Modifier.isVolatile(modifiers))
				{
					throw new IllegalArgumentException("Must be volatile type");
				}

				this.Cclass = (Modifier.isProtected(modifiers) && caller != tclass) ? caller : null;
				this.Tclass = tclass;
				Offset = @unsafe.objectFieldOffset(field);
			}

			private class PrivilegedExceptionActionAnonymousInnerClassHelper : PrivilegedExceptionAction<Field>
			{
				private readonly LockedUpdater<T> OuterInstance;

				private Type Tclass;
				private string FieldName;

				public PrivilegedExceptionActionAnonymousInnerClassHelper(LockedUpdater<T> outerInstance, Type tclass, string fieldName)
				{
					this.outerInstance = outerInstance;
					this.Tclass = tclass;
					this.FieldName = fieldName;
				}

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public Field run() throws NoSuchFieldException
				public virtual Field Run()
				{
					return Tclass.GetDeclaredField(FieldName);
				}
			}

			internal virtual void FullCheck(T obj)
			{
				if (!Tclass.isInstance(obj))
				{
					throw new ClassCastException();
				}
				if (Cclass != null)
				{
					EnsureProtectedAccess(obj);
				}
			}

			public virtual bool CompareAndSet(T obj, long expect, long update)
			{
				if (obj == null || obj.GetType() != Tclass || Cclass != null)
				{
					FullCheck(obj);
				}
				lock (this)
				{
					long v = @unsafe.getLong(obj, Offset);
					if (v != expect)
					{
						return false;
					}
					@unsafe.putLong(obj, Offset, update);
					return true;
				}
			}

			public virtual bool WeakCompareAndSet(T obj, long expect, long update)
			{
				return CompareAndSet(obj, expect, update);
			}

			public virtual void Set(T obj, long newValue)
			{
				if (obj == null || obj.GetType() != Tclass || Cclass != null)
				{
					FullCheck(obj);
				}
				lock (this)
				{
					@unsafe.putLong(obj, Offset, newValue);
				}
			}

			public virtual void LazySet(T obj, long newValue)
			{
				Set(obj, newValue);
			}

			public virtual long Get(T obj)
			{
				if (obj == null || obj.GetType() != Tclass || Cclass != null)
				{
					FullCheck(obj);
				}
				lock (this)
				{
					return @unsafe.getLong(obj, Offset);
				}
			}

			internal virtual void EnsureProtectedAccess(T obj)
			{
				if (Cclass.isInstance(obj))
				{
					return;
				}
//JAVA TO C# CONVERTER WARNING: The .NET Type.FullName property will not always yield results identical to the Java Class.getName method:
				throw new RuntimeException(new IllegalAccessException("Class " + Cclass.Name + " can not access a protected member of class " + Tclass.Name + " using an instance of " + obj.GetType().FullName)
			   );
			}
		}

		/// <summary>
		/// Returns true if the second classloader can be found in the first
		/// classloader's delegation chain.
		/// Equivalent to the inaccessible: first.isAncestor(second).
		/// </summary>
		private static bool IsAncestor(ClassLoader first, ClassLoader second)
		{
			ClassLoader acl = first;
			do
			{
				acl = acl.Parent;
				if (second == acl)
				{
					return true;
				}
			} while (acl != null);
			return false;
		}
	}

}
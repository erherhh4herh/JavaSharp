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
	/// designated {@code volatile} reference fields of designated
	/// classes.  This class is designed for use in atomic data structures
	/// in which several reference fields of the same node are
	/// independently subject to atomic updates. For example, a tree node
	/// might be declared as
	/// 
	///  <pre> {@code
	/// class Node {
	///   private volatile Node left, right;
	/// 
	///   private static final AtomicReferenceFieldUpdater<Node, Node> leftUpdater =
	///     AtomicReferenceFieldUpdater.newUpdater(Node.class, Node.class, "left");
	///   private static AtomicReferenceFieldUpdater<Node, Node> rightUpdater =
	///     AtomicReferenceFieldUpdater.newUpdater(Node.class, Node.class, "right");
	/// 
	///   Node getLeft() { return left;  }
	///   boolean compareAndSetLeft(Node expect, Node update) {
	///     return leftUpdater.compareAndSet(this, expect, update);
	///   }
	///   // ... and so on
	/// }}</pre>
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
	/// @param <V> The type of the field </param>
	public abstract class AtomicReferenceFieldUpdater<T, V>
	{

		/// <summary>
		/// Creates and returns an updater for objects with the given field.
		/// The Class arguments are needed to check that reflective types and
		/// generic types match.
		/// </summary>
		/// <param name="tclass"> the class of the objects holding the field </param>
		/// <param name="vclass"> the class of the field </param>
		/// <param name="fieldName"> the name of the field to be updated </param>
		/// @param <U> the type of instances of tclass </param>
		/// @param <W> the type of instances of vclass </param>
		/// <returns> the updater </returns>
		/// <exception cref="ClassCastException"> if the field is of the wrong type </exception>
		/// <exception cref="IllegalArgumentException"> if the field is not volatile </exception>
		/// <exception cref="RuntimeException"> with a nested reflection-based
		/// exception if the class does not hold field or is the wrong type,
		/// or the field is inaccessible to the caller according to Java language
		/// access control </exception>
//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @CallerSensitive public static <U,W> AtomicReferenceFieldUpdater<U,W> newUpdater(Class tclass, Class vclass, String fieldName)
		public static AtomicReferenceFieldUpdater<U, W> newUpdater<U, W>(Class tclass, Class vclass, String fieldName)
		{
			return new AtomicReferenceFieldUpdaterImpl<U, W> (tclass, vclass, fieldName, Reflection.CallerClass);
		}

		/// <summary>
		/// Protected do-nothing constructor for use by subclasses.
		/// </summary>
		protected internal AtomicReferenceFieldUpdater()
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
		public abstract bool CompareAndSet(T obj, V expect, V update);

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
		public abstract bool WeakCompareAndSet(T obj, V expect, V update);

		/// <summary>
		/// Sets the field of the given object managed by this updater to the
		/// given updated value. This operation is guaranteed to act as a volatile
		/// store with respect to subsequent invocations of {@code compareAndSet}.
		/// </summary>
		/// <param name="obj"> An object whose field to set </param>
		/// <param name="newValue"> the new value </param>
		public abstract void Set(T obj, V newValue);

		/// <summary>
		/// Eventually sets the field of the given object managed by this
		/// updater to the given updated value.
		/// </summary>
		/// <param name="obj"> An object whose field to set </param>
		/// <param name="newValue"> the new value
		/// @since 1.6 </param>
		public abstract void LazySet(T obj, V newValue);

		/// <summary>
		/// Gets the current value held in the field of the given object managed
		/// by this updater.
		/// </summary>
		/// <param name="obj"> An object whose field to get </param>
		/// <returns> the current value </returns>
		public abstract V Get(T obj);

		/// <summary>
		/// Atomically sets the field of the given object managed by this updater
		/// to the given value and returns the old value.
		/// </summary>
		/// <param name="obj"> An object whose field to get and set </param>
		/// <param name="newValue"> the new value </param>
		/// <returns> the previous value </returns>
		public virtual V GetAndSet(T obj, V newValue)
		{
			V prev;
			do
			{
				prev = Get(obj);
			} while (!CompareAndSet(obj, prev, newValue));
			return prev;
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
		public V GetAndUpdate(T obj, UnaryOperator<V> updateFunction)
		{
			V prev, next;
			do
			{
				prev = Get(obj);
				next = updateFunction.Apply(prev);
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
		public V UpdateAndGet(T obj, UnaryOperator<V> updateFunction)
		{
			V prev, next;
			do
			{
				prev = Get(obj);
				next = updateFunction.Apply(prev);
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
		public V GetAndAccumulate(T obj, V x, BinaryOperator<V> accumulatorFunction)
		{
			V prev, next;
			do
			{
				prev = Get(obj);
				next = accumulatorFunction.Apply(prev, x);
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
		public V AccumulateAndGet(T obj, V x, BinaryOperator<V> accumulatorFunction)
		{
			V prev, next;
			do
			{
				prev = Get(obj);
				next = accumulatorFunction.Apply(prev, x);
			} while (!CompareAndSet(obj, prev, next));
			return next;
		}

		private sealed class AtomicReferenceFieldUpdaterImpl<T, V> : AtomicReferenceFieldUpdater<T, V>
		{
			internal static readonly Unsafe @unsafe = Unsafe.Unsafe;
			internal readonly long Offset;
			internal readonly Class Tclass;
			internal readonly Class Vclass;
			internal readonly Class Cclass;

			/*
			 * Internal type checks within all update methods contain
			 * internal inlined optimizations checking for the common
			 * cases where the class is final (in which case a simple
			 * getClass comparison suffices) or is of type Object (in
			 * which case no check is needed because all objects are
			 * instances of Object). The Object case is handled simply by
			 * setting vclass to null in constructor.  The targetCheck and
			 * updateCheck methods are invoked when these faster
			 * screenings fail.
			 */

//JAVA TO C# CONVERTER WARNING: 'final' parameters are not allowed in .NET:
//ORIGINAL LINE: AtomicReferenceFieldUpdaterImpl(final Class tclass, final Class vclass, final String fieldName, final Class caller)
			internal AtomicReferenceFieldUpdaterImpl(Class tclass, Class vclass, String fieldName, Class caller)
			{
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final Field field;
				Field field;
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final Class fieldClass;
				Class fieldClass;
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
					fieldClass = field.Type;
				}
				catch (PrivilegedActionException pae)
				{
					throw new RuntimeException(pae.Exception);
				}
				catch (Exception ex)
				{
					throw new RuntimeException(ex);
				}

				if (vclass != fieldClass)
				{
					throw new ClassCastException();
				}
				if (vclass.Primitive)
				{
					throw new IllegalArgumentException("Must be reference type");
				}

				if (!Modifier.isVolatile(modifiers))
				{
					throw new IllegalArgumentException("Must be volatile type");
				}

				this.Cclass = (Modifier.isProtected(modifiers) && caller != tclass) ? caller : null;
				this.Tclass = tclass;
				if (vclass == typeof(Object))
				{
					this.Vclass = null;
				}
				else
				{
					this.Vclass = vclass;
				}
				Offset = @unsafe.objectFieldOffset(field);
			}

			private class PrivilegedExceptionActionAnonymousInnerClassHelper : PrivilegedExceptionAction<Field>
			{
				private readonly AtomicReferenceFieldUpdaterImpl<T, V> OuterInstance;

				private Type Tclass;
				private string FieldName;

				public PrivilegedExceptionActionAnonymousInnerClassHelper(AtomicReferenceFieldUpdaterImpl<T, V> outerInstance, Type tclass, string fieldName)
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

			/// <summary>
			/// Returns true if the second classloader can be found in the first
			/// classloader's delegation chain.
			/// Equivalent to the inaccessible: first.isAncestor(second).
			/// </summary>
			internal static bool IsAncestor(ClassLoader first, ClassLoader second)
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

			internal void TargetCheck(T obj)
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

			internal void UpdateCheck(T obj, V update)
			{
				if (!Tclass.isInstance(obj) || (update != null && Vclass != null && !Vclass.isInstance(update)))
				{
					throw new ClassCastException();
				}
				if (Cclass != null)
				{
					EnsureProtectedAccess(obj);
				}
			}

			public bool CompareAndSet(T obj, V expect, V update)
			{
				if (obj == null || obj.GetType() != Tclass || Cclass != null || (update != null && Vclass != null && Vclass != update.GetType()))
				{
					UpdateCheck(obj, update);
				}
				return @unsafe.compareAndSwapObject(obj, Offset, expect, update);
			}

			public bool WeakCompareAndSet(T obj, V expect, V update)
			{
				// same implementation as strong form for now
				if (obj == null || obj.GetType() != Tclass || Cclass != null || (update != null && Vclass != null && Vclass != update.GetType()))
				{
					UpdateCheck(obj, update);
				}
				return @unsafe.compareAndSwapObject(obj, Offset, expect, update);
			}

			public void Set(T obj, V newValue)
			{
				if (obj == null || obj.GetType() != Tclass || Cclass != null || (newValue != null && Vclass != null && Vclass != newValue.GetType()))
				{
					UpdateCheck(obj, newValue);
				}
				@unsafe.putObjectVolatile(obj, Offset, newValue);
			}

			public void LazySet(T obj, V newValue)
			{
				if (obj == null || obj.GetType() != Tclass || Cclass != null || (newValue != null && Vclass != null && Vclass != newValue.GetType()))
				{
					UpdateCheck(obj, newValue);
				}
				@unsafe.putOrderedObject(obj, Offset, newValue);
			}

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @SuppressWarnings("unchecked") public V get(T obj)
			public V Get(T obj)
			{
				if (obj == null || obj.GetType() != Tclass || Cclass != null)
				{
					TargetCheck(obj);
				}
				return (V)@unsafe.getObjectVolatile(obj, Offset);
			}

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @SuppressWarnings("unchecked") public V getAndSet(T obj, V newValue)
			public V GetAndSet(T obj, V newValue)
			{
				if (obj == null || obj.GetType() != Tclass || Cclass != null || (newValue != null && Vclass != null && Vclass != newValue.GetType()))
				{
					UpdateCheck(obj, newValue);
				}
				return (V)@unsafe.getAndSetObject(obj, Offset, newValue);
			}

			internal void EnsureProtectedAccess(T obj)
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
	}

}
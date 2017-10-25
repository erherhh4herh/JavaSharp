/*
 * Copyright (c) 2010, 2011, Oracle and/or its affiliates. All rights reserved.
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

	/// <summary>
	/// A {@code VolatileCallSite} is a <seealso cref="CallSite"/> whose target acts like a volatile variable.
	/// An {@code invokedynamic} instruction linked to a {@code VolatileCallSite} sees updates
	/// to its call site target immediately, even if the update occurs in another thread.
	/// There may be a performance penalty for such tight coupling between threads.
	/// <para>
	/// Unlike {@code MutableCallSite}, there is no
	/// <seealso cref="MutableCallSite#syncAll syncAll operation"/> on volatile
	/// call sites, since every write to a volatile variable is implicitly
	/// synchronized with reader threads.
	/// </para>
	/// <para>
	/// In other respects, a {@code VolatileCallSite} is interchangeable
	/// with {@code MutableCallSite}.
	/// </para>
	/// </summary>
	/// <seealso cref= MutableCallSite
	/// @author John Rose, JSR 292 EG </seealso>
	public class VolatileCallSite : CallSite
	{
		/// <summary>
		/// Creates a call site with a volatile binding to its target.
		/// The initial target is set to a method handle
		/// of the given type which will throw an {@code IllegalStateException} if called. </summary>
		/// <param name="type"> the method type that this call site will have </param>
		/// <exception cref="NullPointerException"> if the proposed type is null </exception>
		public VolatileCallSite(MethodType type) : base(type)
		{
		}

		/// <summary>
		/// Creates a call site with a volatile binding to its target.
		/// The target is set to the given value. </summary>
		/// <param name="target"> the method handle that will be the initial target of the call site </param>
		/// <exception cref="NullPointerException"> if the proposed target is null </exception>
		public VolatileCallSite(MethodHandle target) : base(target)
		{
		}

		/// <summary>
		/// Returns the target method of the call site, which behaves
		/// like a {@code volatile} field of the {@code VolatileCallSite}.
		/// <para>
		/// The interactions of {@code getTarget} with memory are the same
		/// as of a read from a {@code volatile} field.
		/// </para>
		/// <para>
		/// In particular, the current thread is required to issue a fresh
		/// read of the target from memory, and must not fail to see
		/// a recent update to the target by another thread.
		/// 
		/// </para>
		/// </summary>
		/// <returns> the linkage state of this call site, a method handle which can change over time </returns>
		/// <seealso cref= #setTarget </seealso>
		public override sealed MethodHandle Target
		{
			get
			{
				return TargetVolatile;
			}
			set
			{
				CheckTargetChange(TargetVolatile, value);
				TargetVolatile = value;
			}
		}


		/// <summary>
		/// {@inheritDoc}
		/// </summary>
		public override sealed MethodHandle DynamicInvoker()
		{
			return MakeDynamicInvoker();
		}
	}

}
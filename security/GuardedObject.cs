using System;

/*
 * Copyright (c) 1997, 2013, Oracle and/or its affiliates. All rights reserved.
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

namespace java.security
{

	/// <summary>
	/// A GuardedObject is an object that is used to protect access to
	/// another object.
	/// 
	/// <para>A GuardedObject encapsulates a target object and a Guard object,
	/// such that access to the target object is possible
	/// only if the Guard object allows it.
	/// Once an object is encapsulated by a GuardedObject,
	/// access to that object is controlled by the {@code getObject}
	/// method, which invokes the
	/// {@code checkGuard} method on the Guard object that is
	/// guarding access. If access is not allowed,
	/// an exception is thrown.
	/// 
	/// </para>
	/// </summary>
	/// <seealso cref= Guard </seealso>
	/// <seealso cref= Permission
	/// 
	/// @author Roland Schemers
	/// @author Li Gong </seealso>

	[Serializable]
	public class GuardedObject
	{

		private const long SerialVersionUID = -5240450096227834308L;

		private Object @object; // the object we are guarding
		private Guard Guard; // the guard

		/// <summary>
		/// Constructs a GuardedObject using the specified object and guard.
		/// If the Guard object is null, then no restrictions will
		/// be placed on who can access the object.
		/// </summary>
		/// <param name="object"> the object to be guarded.
		/// </param>
		/// <param name="guard"> the Guard object that guards access to the object. </param>

		public GuardedObject(Object @object, Guard guard)
		{
			this.Guard = guard;
			this.@object = @object;
		}

		/// <summary>
		/// Retrieves the guarded object, or throws an exception if access
		/// to the guarded object is denied by the guard.
		/// </summary>
		/// <returns> the guarded object.
		/// </returns>
		/// <exception cref="SecurityException"> if access to the guarded object is
		/// denied. </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public Object getObject() throws SecurityException
		public virtual Object Object
		{
			get
			{
				if (Guard != null)
				{
					Guard.CheckGuard(@object);
				}
    
				return @object;
			}
		}

		/// <summary>
		/// Writes this object out to a stream (i.e., serializes it).
		/// We check the guard if there is one.
		/// </summary>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: private void writeObject(java.io.ObjectOutputStream oos) throws java.io.IOException
		private void WriteObject(java.io.ObjectOutputStream oos)
		{
			if (Guard != null)
			{
				Guard.CheckGuard(@object);
			}

			oos.DefaultWriteObject();
		}
	}

}
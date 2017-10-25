/*
 * Copyright (c) 2005, 2013, Oracle and/or its affiliates. All rights reserved.
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

namespace java.lang.management
{

	using LockInfoCompositeData = sun.management.LockInfoCompositeData;

	/// <summary>
	/// Information about a <em>lock</em>.  A lock can be a built-in object monitor,
	/// an <em>ownable synchronizer</em>, or the <seealso cref="Condition Condition"/>
	/// object associated with synchronizers.
	/// <para>
	/// <a name="OwnableSynchronizer">An ownable synchronizer</a> is
	/// a synchronizer that may be exclusively owned by a thread and uses
	/// <seealso cref="AbstractOwnableSynchronizer AbstractOwnableSynchronizer"/>
	/// (or its subclass) to implement its synchronization property.
	/// <seealso cref="ReentrantLock ReentrantLock"/> and
	/// <seealso cref="ReentrantReadWriteLock ReentrantReadWriteLock"/> are
	/// two examples of ownable synchronizers provided by the platform.
	/// 
	/// <h3><a name="MappedType">MXBean Mapping</a></h3>
	/// <tt>LockInfo</tt> is mapped to a <seealso cref="CompositeData CompositeData"/>
	/// as specified in the <seealso cref="#from from"/> method.
	/// 
	/// </para>
	/// </summary>
	/// <seealso cref= java.util.concurrent.locks.AbstractOwnableSynchronizer </seealso>
	/// <seealso cref= java.util.concurrent.locks.Condition
	/// 
	/// @author  Mandy Chung
	/// @since   1.6 </seealso>

	public class LockInfo
	{

		private String ClassName_Renamed;
		private int IdentityHashCode_Renamed;

		/// <summary>
		/// Constructs a <tt>LockInfo</tt> object.
		/// </summary>
		/// <param name="className"> the fully qualified name of the class of the lock object. </param>
		/// <param name="identityHashCode"> the {@link System#identityHashCode
		///                         identity hash code} of the lock object. </param>
		public LockInfo(String className, int identityHashCode)
		{
			if (className == null)
			{
				throw new NullPointerException("Parameter className cannot be null");
			}
			this.ClassName_Renamed = className;
			this.IdentityHashCode_Renamed = identityHashCode;
		}

		/// <summary>
		/// package-private constructors
		/// </summary>
		internal LockInfo(Object @lock)
		{
//JAVA TO C# CONVERTER WARNING: The .NET Type.FullName property will not always yield results identical to the Java Class.getName method:
			this.ClassName_Renamed = @lock.GetType().FullName;
			this.IdentityHashCode_Renamed = System.identityHashCode(@lock);
		}

		/// <summary>
		/// Returns the fully qualified name of the class of the lock object.
		/// </summary>
		/// <returns> the fully qualified name of the class of the lock object. </returns>
		public virtual String ClassName
		{
			get
			{
				return ClassName_Renamed;
			}
		}

		/// <summary>
		/// Returns the identity hash code of the lock object
		/// returned from the <seealso cref="System#identityHashCode"/> method.
		/// </summary>
		/// <returns> the identity hash code of the lock object. </returns>
		public virtual int IdentityHashCode
		{
			get
			{
				return IdentityHashCode_Renamed;
			}
		}

		/// <summary>
		/// Returns a {@code LockInfo} object represented by the
		/// given {@code CompositeData}.
		/// The given {@code CompositeData} must contain the following attributes:
		/// <blockquote>
		/// <table border summary="The attributes and the types the given CompositeData contains">
		/// <tr>
		///   <th align=left>Attribute Name</th>
		///   <th align=left>Type</th>
		/// </tr>
		/// <tr>
		///   <td>className</td>
		///   <td><tt>java.lang.String</tt></td>
		/// </tr>
		/// <tr>
		///   <td>identityHashCode</td>
		///   <td><tt>java.lang.Integer</tt></td>
		/// </tr>
		/// </table>
		/// </blockquote>
		/// </summary>
		/// <param name="cd"> {@code CompositeData} representing a {@code LockInfo}
		/// </param>
		/// <exception cref="IllegalArgumentException"> if {@code cd} does not
		///   represent a {@code LockInfo} with the attributes described
		///   above. </exception>
		/// <returns> a {@code LockInfo} object represented
		///         by {@code cd} if {@code cd} is not {@code null};
		///         {@code null} otherwise.
		/// 
		/// @since 1.8 </returns>
		public static LockInfo From(CompositeData cd)
		{
			if (cd == null)
			{
				return null;
			}

			if (cd is LockInfoCompositeData)
			{
				return ((LockInfoCompositeData) cd).LockInfo;
			}
			else
			{
				return LockInfoCompositeData.toLockInfo(cd);
			}
		}

		/// <summary>
		/// Returns a string representation of a lock.  The returned
		/// string representation consists of the name of the class of the
		/// lock object, the at-sign character `@', and the unsigned
		/// hexadecimal representation of the <em>identity</em> hash code
		/// of the object.  This method returns a string equals to the value of:
		/// <blockquote>
		/// <pre>
		/// lock.getClass().getName() + '@' + Integer.toHexString(System.identityHashCode(lock))
		/// </pre></blockquote>
		/// where <tt>lock</tt> is the lock object.
		/// </summary>
		/// <returns> the string representation of a lock. </returns>
		public override String ToString()
		{
			return ClassName_Renamed + '@' + IdentityHashCode_Renamed.ToString("x");
		}
	}

}
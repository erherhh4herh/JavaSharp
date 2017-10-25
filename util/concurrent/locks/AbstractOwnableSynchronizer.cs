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

namespace java.util.concurrent.locks
{

	/// <summary>
	/// A synchronizer that may be exclusively owned by a thread.  This
	/// class provides a basis for creating locks and related synchronizers
	/// that may entail a notion of ownership.  The
	/// {@code AbstractOwnableSynchronizer} class itself does not manage or
	/// use this information. However, subclasses and tools may use
	/// appropriately maintained values to help control and monitor access
	/// and provide diagnostics.
	/// 
	/// @since 1.6
	/// @author Doug Lea
	/// </summary>
	[Serializable]
	public abstract class AbstractOwnableSynchronizer
	{

		/// <summary>
		/// Use serial ID even though all fields transient. </summary>
		private const long SerialVersionUID = 3737899427754241961L;

		/// <summary>
		/// Empty constructor for use by subclasses.
		/// </summary>
		protected internal AbstractOwnableSynchronizer()
		{
		}

		/// <summary>
		/// The current owner of exclusive mode synchronization.
		/// </summary>
		[NonSerialized]
		private Thread ExclusiveOwnerThread_Renamed;

		/// <summary>
		/// Sets the thread that currently owns exclusive access.
		/// A {@code null} argument indicates that no thread owns access.
		/// This method does not otherwise impose any synchronization or
		/// {@code volatile} field accesses. </summary>
		/// <param name="thread"> the owner thread </param>
		protected internal Thread ExclusiveOwnerThread
		{
			set
			{
				ExclusiveOwnerThread_Renamed = value;
			}
			get
			{
				return ExclusiveOwnerThread_Renamed;
			}
		}

	}

}
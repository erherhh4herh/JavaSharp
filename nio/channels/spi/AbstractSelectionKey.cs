/*
 * Copyright (c) 2000, 2013, Oracle and/or its affiliates. All rights reserved.
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

namespace java.nio.channels.spi
{


	/// <summary>
	/// Base implementation class for selection keys.
	/// 
	/// <para> This class tracks the validity of the key and implements cancellation.
	/// 
	/// @author Mark Reinhold
	/// @author JSR-51 Expert Group
	/// @since 1.4
	/// </para>
	/// </summary>

	public abstract class AbstractSelectionKey : SelectionKey
	{

		/// <summary>
		/// Initializes a new instance of this class.
		/// </summary>
		protected internal AbstractSelectionKey()
		{
		}

		private volatile bool Valid_Renamed = true;

		public sealed override bool Valid
		{
			get
			{
				return Valid_Renamed;
			}
		}

		internal virtual void Invalidate() // package-private
		{
			Valid_Renamed = false;
		}

		/// <summary>
		/// Cancels this key.
		/// 
		/// <para> If this key has not yet been cancelled then it is added to its
		/// selector's cancelled-key set while synchronized on that set.  </para>
		/// </summary>
		public sealed override void Cancel()
		{
			// Synchronizing "this" to prevent this key from getting canceled
			// multiple times by different threads, which might cause race
			// condition between selector's select() and channel's close().
			lock (this)
			{
				if (Valid_Renamed)
				{
					Valid_Renamed = false;
					((AbstractSelector)Selector()).Cancel(this);
				}
			}
		}
	}

}
/*
 * Copyright (c) 1996, 1999, Oracle and/or its affiliates. All rights reserved.
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

namespace java.awt.@event
{

	/// <summary>
	/// The listener interface for receiving adjustment events.
	/// 
	/// @author Amy Fowler
	/// @since 1.1
	/// </summary>
	public interface AdjustmentListener : EventListener
	{

		/// <summary>
		/// Invoked when the value of the adjustable has changed.
		/// </summary>
		void AdjustmentValueChanged(AdjustmentEvent e);

	}

}
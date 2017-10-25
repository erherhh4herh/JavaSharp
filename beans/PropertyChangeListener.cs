/*
 * Copyright (c) 1996, Oracle and/or its affiliates. All rights reserved.
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

namespace java.beans
{

	/// <summary>
	/// A "PropertyChange" event gets fired whenever a bean changes a "bound"
	/// property.  You can register a PropertyChangeListener with a source
	/// bean so as to be notified of any bound property updates.
	/// </summary>

	public interface PropertyChangeListener : java.util.EventListener
	{

		/// <summary>
		/// This method gets called when a bound property is changed. </summary>
		/// <param name="evt"> A PropertyChangeEvent object describing the event source
		///          and the property that has changed. </param>

		void PropertyChange(PropertyChangeEvent evt);

	}

}
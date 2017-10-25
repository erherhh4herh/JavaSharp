/*
 * Copyright (c) 2000, Oracle and/or its affiliates. All rights reserved.
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

namespace java.util.prefs
{

	/// <summary>
	/// A listener for receiving preference node change events.
	/// 
	/// @author  Josh Bloch </summary>
	/// <seealso cref=     Preferences </seealso>
	/// <seealso cref=     NodeChangeEvent </seealso>
	/// <seealso cref=     PreferenceChangeListener
	/// @since   1.4 </seealso>

	public interface NodeChangeListener : java.util.EventListener
	{
		/// <summary>
		/// This method gets called when a child node is added.
		/// </summary>
		/// <param name="evt"> A node change event object describing the parent
		///            and child node. </param>
		void ChildAdded(NodeChangeEvent evt);

		/// <summary>
		/// This method gets called when a child node is removed.
		/// </summary>
		/// <param name="evt"> A node change event object describing the parent
		///            and child node. </param>
		void ChildRemoved(NodeChangeEvent evt);
	}

}
/*
 * Copyright (c) 1996, 1998, Oracle and/or its affiliates. All rights reserved.
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
	/// Under some circumstances a bean may be run on servers where a GUI
	/// is not available.  This interface can be used to query a bean to
	/// determine whether it absolutely needs a gui, and to advise the
	/// bean whether a GUI is available.
	/// <para>
	/// This interface is for expert developers, and is not needed
	/// for normal simple beans.  To avoid confusing end-users we
	/// avoid using getXXX setXXX design patterns for these methods.
	/// </para>
	/// </summary>

	public interface Visibility
	{

		/// <summary>
		/// Determines whether this bean needs a GUI.
		/// </summary>
		/// <returns> True if the bean absolutely needs a GUI available in
		///          order to get its work done. </returns>
		bool NeedsGui();

		/// <summary>
		/// This method instructs the bean that it should not use the Gui.
		/// </summary>
		void DontUseGui();

		/// <summary>
		/// This method instructs the bean that it is OK to use the Gui.
		/// </summary>
		void OkToUseGui();

		/// <summary>
		/// Determines whether this bean is avoiding using a GUI.
		/// </summary>
		/// <returns> true if the bean is currently avoiding use of the Gui.
		///   e.g. due to a call on dontUseGui(). </returns>
		bool AvoidingGui();

	}

}
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

namespace java.util.prefs
{

	/// <summary>
	/// A listener for receiving preference change events.
	/// 
	/// @author  Josh Bloch </summary>
	/// <seealso cref= Preferences </seealso>
	/// <seealso cref= PreferenceChangeEvent </seealso>
	/// <seealso cref= NodeChangeListener
	/// @since   1.4 </seealso>
//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @FunctionalInterface public interface PreferenceChangeListener extends java.util.EventListener
	public interface PreferenceChangeListener : java.util.EventListener
	{
		/// <summary>
		/// This method gets called when a preference is added, removed or when
		/// its value is changed.
		/// <para>
		/// </para>
		/// </summary>
		/// <param name="evt"> A PreferenceChangeEvent object describing the event source
		///          and the preference that has changed. </param>
		void PreferenceChange(PreferenceChangeEvent evt);
	}

}
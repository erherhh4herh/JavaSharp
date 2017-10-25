/*
 * Copyright (c) 2000, 2001, Oracle and/or its affiliates. All rights reserved.
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
	/// Implementation of  <tt>PreferencesFactory</tt> to return
	/// WindowsPreferences objects.
	/// 
	/// @author  Konstantin Kladko </summary>
	/// <seealso cref= Preferences </seealso>
	/// <seealso cref= WindowsPreferences
	/// @since 1.4 </seealso>
	internal class WindowsPreferencesFactory : PreferencesFactory
	{

		/// <summary>
		/// Returns WindowsPreferences.userRoot
		/// </summary>
		public virtual Preferences UserRoot()
		{
			return WindowsPreferences.UserRoot;
		}

		/// <summary>
		/// Returns WindowsPreferences.systemRoot
		/// </summary>
		public virtual Preferences SystemRoot()
		{
			return WindowsPreferences.SystemRoot;
		}
	}

}
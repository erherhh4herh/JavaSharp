/*
 * Copyright (c) 2003, Oracle and/or its affiliates. All rights reserved.
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
namespace java.awt.datatransfer
{


	/// <summary>
	/// Defines an object which listens for <seealso cref="FlavorEvent"/>s.
	/// 
	/// @author Alexander Gerasimov
	/// @since 1.5
	/// </summary>
	public interface FlavorListener : EventListener
	{
		/// <summary>
		/// Invoked when the target <seealso cref="Clipboard"/> of the listener
		/// has changed its available <seealso cref="DataFlavor"/>s.
		/// <para>
		/// Some notifications may be redundant &#151; they are not
		/// caused by a change of the set of DataFlavors available
		/// on the clipboard.
		/// For example, if the clipboard subsystem supposes that
		/// the system clipboard's contents has been changed but it
		/// can't ascertain whether its DataFlavors have been changed
		/// because of some exceptional condition when accessing the
		/// clipboard, the notification is sent to ensure from omitting
		/// a significant notification. Ordinarily, those redundant
		/// notifications should be occasional.
		/// 
		/// </para>
		/// </summary>
		/// <param name="e">  a <code>FlavorEvent</code> object </param>
		void FlavorsChanged(FlavorEvent e);
	}

}
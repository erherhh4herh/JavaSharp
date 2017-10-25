/*
 * Copyright (c) 1995, 2014, Oracle and/or its affiliates. All rights reserved.
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
namespace java.awt.peer
{


	/// <summary>
	/// The peer interface for <seealso cref="Checkbox"/>.
	/// 
	/// The peer interfaces are intended only for use in porting
	/// the AWT. They are not intended for use by application
	/// developers, and developers should not implement peers
	/// nor invoke any of the peer methods directly on the peer
	/// instances.
	/// </summary>
	public interface CheckboxPeer : ComponentPeer
	{

		/// <summary>
		/// Sets the state of the checkbox to be checked {@code true} or
		/// unchecked {@code false}.
		/// </summary>
		/// <param name="state"> the state to set on the checkbox
		/// </param>
		/// <seealso cref= Checkbox#setState(boolean) </seealso>
		bool State {set;}

		/// <summary>
		/// Sets the checkbox group for this checkbox. Checkboxes in one checkbox
		/// group can only be selected exclusively (like radio buttons). A value
		/// of {@code null} removes this checkbox from any checkbox group.
		/// </summary>
		/// <param name="g"> the checkbox group to set, or {@code null} when this
		///          checkbox should not be placed in any group
		/// </param>
		/// <seealso cref= Checkbox#setCheckboxGroup(CheckboxGroup) </seealso>
		CheckboxGroup CheckboxGroup {set;}

		/// <summary>
		/// Sets the label that should be displayed on the checkbox. A value of
		/// {@code null} means that no label should be displayed.
		/// </summary>
		/// <param name="label"> the label to be displayed on the checkbox, or
		///              {@code null} when no label should be displayed. </param>
		String Label {set;}

	}

}
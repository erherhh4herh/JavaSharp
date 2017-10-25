/*
 * Copyright (c) 1997, 1999, Oracle and/or its affiliates. All rights reserved.
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

namespace java.beans.beancontext
{


	/// <summary>
	/// <para>
	/// Compliant BeanContexts fire events on this interface when the state of
	/// the membership of the BeanContext changes.
	/// </para>
	/// 
	/// @author      Laurence P. G. Cable
	/// @since       1.2 </summary>
	/// <seealso cref=         java.beans.beancontext.BeanContext </seealso>

	public interface BeanContextMembershipListener : EventListener
	{

		/// <summary>
		/// Called when a child or list of children is added to a
		/// <code>BeanContext</code> that this listener is registered with. </summary>
		/// <param name="bcme"> The <code>BeanContextMembershipEvent</code>
		/// describing the change that occurred. </param>
		void ChildrenAdded(BeanContextMembershipEvent bcme);

		/// <summary>
		/// Called when a child or list of children is removed
		/// from a <code>BeanContext</code> that this listener
		/// is registered with. </summary>
		/// <param name="bcme"> The <code>BeanContextMembershipEvent</code>
		/// describing the change that occurred. </param>
		void ChildrenRemoved(BeanContextMembershipEvent bcme);
	}

}
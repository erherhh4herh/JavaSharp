/*
 * Copyright (c) 1998, 2002, Oracle and/or its affiliates. All rights reserved.
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
	/// This interface is implemented by BeanContexts' that have an AWT Container
	/// associated with them.
	/// </para>
	/// 
	/// @author Laurence P. G. Cable
	/// @since 1.2
	/// </summary>
	/// <seealso cref= java.beans.beancontext.BeanContext </seealso>
	/// <seealso cref= java.beans.beancontext.BeanContextSupport </seealso>

	public interface BeanContextContainerProxy
	{

		/// <summary>
		/// Gets the <code>java.awt.Container</code> associated
		/// with this <code>BeanContext</code>. </summary>
		/// <returns> the <code>java.awt.Container</code> associated
		/// with this <code>BeanContext</code>. </returns>
		Container Container {get;}
	}

}
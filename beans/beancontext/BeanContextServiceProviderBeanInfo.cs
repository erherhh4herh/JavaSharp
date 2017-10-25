/*
 * Copyright (c) 1998, 1999, Oracle and/or its affiliates. All rights reserved.
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
	/// A BeanContextServiceProvider implementor who wishes to provide explicit
	/// information about the services their bean may provide shall implement a
	/// BeanInfo class that implements this BeanInfo subinterface and provides
	/// explicit information about the methods, properties, events, etc, of their
	/// services.
	/// </summary>

	public interface BeanContextServiceProviderBeanInfo : BeanInfo
	{

		/// <summary>
		/// Gets a <code>BeanInfo</code> array, one for each
		/// service class or interface statically available
		/// from this ServiceProvider. </summary>
		/// <returns> the <code>BeanInfo</code> array </returns>
		BeanInfo[] ServicesBeanInfo {get;}
	}

}
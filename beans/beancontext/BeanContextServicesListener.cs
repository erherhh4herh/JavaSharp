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
	/// The listener interface for receiving
	/// <code>BeanContextServiceAvailableEvent</code> objects.
	/// A class that is interested in processing a
	/// <code>BeanContextServiceAvailableEvent</code> implements this interface.
	/// </summary>
	public interface BeanContextServicesListener : BeanContextServiceRevokedListener
	{

		/// <summary>
		/// The service named has been registered. getService requests for
		/// this service may now be made. </summary>
		/// <param name="bcsae"> the <code>BeanContextServiceAvailableEvent</code> </param>
		void ServiceAvailable(BeanContextServiceAvailableEvent bcsae);
	}

}
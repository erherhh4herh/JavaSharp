using System.Collections;

/*
 * Copyright (c) 1998, 2004, Oracle and/or its affiliates. All rights reserved.
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
	/// One of the primary functions of a BeanContext is to act a as rendezvous
	/// between JavaBeans, and BeanContextServiceProviders.
	/// </para>
	/// <para>
	/// A JavaBean nested within a BeanContext, may ask that BeanContext to
	/// provide an instance of a "service", based upon a reference to a Java
	/// Class object that represents that service.
	/// </para>
	/// <para>
	/// If such a service has been registered with the context, or one of its
	/// nesting context's, in the case where a context delegate to its context
	/// to satisfy a service request, then the BeanContextServiceProvider associated with
	/// the service is asked to provide an instance of that service.
	/// </para>
	/// <para>
	/// The ServcieProvider may always return the same instance, or it may
	/// construct a new instance for each request.
	/// </para>
	/// </summary>

	public interface BeanContextServiceProvider
	{

	   /// <summary>
	   /// Invoked by <code>BeanContextServices</code>, this method
	   /// requests an instance of a
	   /// service from this <code>BeanContextServiceProvider</code>.
	   /// </summary>
	   /// <param name="bcs"> The <code>BeanContextServices</code> associated with this
	   /// particular request. This parameter enables the
	   /// <code>BeanContextServiceProvider</code> to distinguish service
	   /// requests from multiple sources.
	   /// </param>
	   /// <param name="requestor">          The object requesting the service
	   /// </param>
	   /// <param name="serviceClass">       The service requested
	   /// </param>
	   /// <param name="serviceSelector"> the service dependent parameter
	   /// for a particular service, or <code>null</code> if not applicable.
	   /// </param>
	   /// <returns> a reference to the requested service </returns>
		Object GetService(BeanContextServices bcs, Object requestor, Class serviceClass, Object serviceSelector);

		/// <summary>
		/// Invoked by <code>BeanContextServices</code>,
		/// this method releases a nested <code>BeanContextChild</code>'s
		/// (or any arbitrary object associated with a
		/// <code>BeanContextChild</code>) reference to the specified service.
		/// </summary>
		/// <param name="bcs"> the <code>BeanContextServices</code> associated with this
		/// particular release request
		/// </param>
		/// <param name="requestor"> the object requesting the service to be released
		/// </param>
		/// <param name="service"> the service that is to be released </param>
		void ReleaseService(BeanContextServices bcs, Object requestor, Object service);

		/// <summary>
		/// Invoked by <code>BeanContextServices</code>, this method
		/// gets the current service selectors for the specified service.
		/// A service selector is a service specific parameter,
		/// typical examples of which could include: a
		/// parameter to a constructor for the service implementation class,
		/// a value for a particular service's property, or a key into a
		/// map of existing implementations.
		/// </summary>
		/// <param name="bcs">           the <code>BeanContextServices</code> for this request </param>
		/// <param name="serviceClass">  the specified service </param>
		/// <returns>   the current service selectors for the specified serviceClass </returns>
		IEnumerator GetCurrentServiceSelectors(BeanContextServices bcs, Class serviceClass);
	}

}
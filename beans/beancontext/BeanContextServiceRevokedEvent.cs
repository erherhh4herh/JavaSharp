/*
 * Copyright (c) 1998, 2009, Oracle and/or its affiliates. All rights reserved.
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
	/// This event type is used by the
	/// <code>BeanContextServiceRevokedListener</code> in order to
	/// identify the service being revoked.
	/// </para>
	/// </summary>
	public class BeanContextServiceRevokedEvent : BeanContextEvent
	{
		private const long SerialVersionUID = -1295543154724961754L;

		/// <summary>
		/// Construct a <code>BeanContextServiceEvent</code>. </summary>
		/// <param name="bcs"> the <code>BeanContextServices</code>
		/// from which this service is being revoked </param>
		/// <param name="sc"> the service that is being revoked </param>
		/// <param name="invalidate"> <code>true</code> for immediate revocation </param>
		public BeanContextServiceRevokedEvent(BeanContextServices bcs, Class sc, bool invalidate) : base((BeanContext)bcs)
		{

			ServiceClass_Renamed = sc;
			InvalidateRefs = invalidate;
		}

		/// <summary>
		/// Gets the source as a reference of type <code>BeanContextServices</code> </summary>
		/// <returns> the <code>BeanContextServices</code> from which
		/// this service is being revoked </returns>
		public virtual BeanContextServices SourceAsBeanContextServices
		{
			get
			{
				return (BeanContextServices)BeanContext;
			}
		}

		/// <summary>
		/// Gets the service class that is the subject of this notification </summary>
		/// <returns> A <code>Class</code> reference to the
		/// service that is being revoked </returns>
		public virtual Class ServiceClass
		{
			get
			{
				return ServiceClass_Renamed;
			}
		}

		/// <summary>
		/// Checks this event to determine whether or not
		/// the service being revoked is of a particular class. </summary>
		/// <param name="service"> the service of interest (should be non-null) </param>
		/// <returns> <code>true</code> if the service being revoked is of the
		/// same class as the specified service </returns>
		public virtual bool IsServiceClass(Class service)
		{
			return ServiceClass_Renamed.Equals(service);
		}

		/// <summary>
		/// Reports if the current service is being forcibly revoked,
		/// in which case the references are now invalidated and unusable. </summary>
		/// <returns> <code>true</code> if current service is being forcibly revoked </returns>
		public virtual bool CurrentServiceInvalidNow
		{
			get
			{
				return InvalidateRefs;
			}
		}

		/// <summary>
		/// fields
		/// </summary>

		/// <summary>
		/// A <code>Class</code> reference to the service that is being revoked.
		/// </summary>
		protected internal Class ServiceClass_Renamed;
		private bool InvalidateRefs;
	}

}
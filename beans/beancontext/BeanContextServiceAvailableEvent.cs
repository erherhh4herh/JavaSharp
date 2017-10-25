using System.Collections;

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
	/// This event type is used by the BeanContextServicesListener in order to
	/// identify the service being registered.
	/// </para>
	/// </summary>

	public class BeanContextServiceAvailableEvent : BeanContextEvent
	{
		private const long SerialVersionUID = -5333985775656400778L;

		/// <summary>
		/// Construct a <code>BeanContextAvailableServiceEvent</code>. </summary>
		/// <param name="bcs"> The context in which the service has become available </param>
		/// <param name="sc"> A <code>Class</code> reference to the newly available service </param>
		public BeanContextServiceAvailableEvent(BeanContextServices bcs, Class sc) : base((BeanContext)bcs)
		{

			ServiceClass_Renamed = sc;
		}

		/// <summary>
		/// Gets the source as a reference of type <code>BeanContextServices</code>. </summary>
		/// <returns> The context in which the service has become available </returns>
		public virtual BeanContextServices SourceAsBeanContextServices
		{
			get
			{
				return (BeanContextServices)BeanContext;
			}
		}

		/// <summary>
		/// Gets the service class that is the subject of this notification. </summary>
		/// <returns> A <code>Class</code> reference to the newly available service </returns>
		public virtual Class ServiceClass
		{
			get
			{
				return ServiceClass_Renamed;
			}
		}

		/// <summary>
		/// Gets the list of service dependent selectors. </summary>
		/// <returns> the current selectors available from the service </returns>
		public virtual IEnumerator CurrentServiceSelectors
		{
			get
			{
				return ((BeanContextServices)Source).GetCurrentServiceSelectors(ServiceClass_Renamed);
			}
		}

		/*
		 * fields
		 */

		/// <summary>
		/// A <code>Class</code> reference to the newly available service
		/// </summary>
		protected internal Class ServiceClass_Renamed;
	}

}
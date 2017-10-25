/*
 * Copyright (c) 1997, 2009, Oracle and/or its affiliates. All rights reserved.
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
	/// <code>BeanContextEvent</code> is the abstract root event class
	/// for all events emitted
	/// from, and pertaining to the semantics of, a <code>BeanContext</code>.
	/// This class introduces a mechanism to allow the propagation of
	/// <code>BeanContextEvent</code> subclasses through a hierarchy of
	/// <code>BeanContext</code>s. The <code>setPropagatedFrom()</code>
	/// and <code>getPropagatedFrom()</code> methods allow a
	/// <code>BeanContext</code> to identify itself as the source
	/// of a propagated event.
	/// </para>
	/// 
	/// @author      Laurence P. G. Cable
	/// @since       1.2 </summary>
	/// <seealso cref=         java.beans.beancontext.BeanContext </seealso>

	public abstract class BeanContextEvent : EventObject
	{
		private const long SerialVersionUID = 7267998073569045052L;

		/// <summary>
		/// Contruct a BeanContextEvent
		/// </summary>
		/// <param name="bc">        The BeanContext source </param>
		protected internal BeanContextEvent(BeanContext bc) : base(bc)
		{
		}

		/// <summary>
		/// Gets the <code>BeanContext</code> associated with this event. </summary>
		/// <returns> the <code>BeanContext</code> associated with this event. </returns>
		public virtual BeanContext BeanContext
		{
			get
			{
				return (BeanContext)Source;
			}
		}

		/// <summary>
		/// Sets the <code>BeanContext</code> from which this event was propagated. </summary>
		/// <param name="bc"> the <code>BeanContext</code> from which this event
		/// was propagated </param>
		public virtual BeanContext PropagatedFrom
		{
			set
			{
				lock (this)
				{
					PropagatedFrom_Renamed = value;
				}
			}
			get
			{
				lock (this)
				{
					return PropagatedFrom_Renamed;
				}
			}
		}


		/// <summary>
		/// Reports whether or not this event is
		/// propagated from some other <code>BeanContext</code>. </summary>
		/// <returns> <code>true</code> if propagated, <code>false</code>
		/// if not </returns>
		public virtual bool Propagated
		{
			get
			{
				lock (this)
				{
					return PropagatedFrom_Renamed != null;
				}
			}
		}

		/*
		 * fields
		 */

		/// <summary>
		/// The <code>BeanContext</code> from which this event was propagated
		/// </summary>
		protected internal BeanContext PropagatedFrom_Renamed;
	}

}
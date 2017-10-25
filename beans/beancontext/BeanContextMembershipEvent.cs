using System.Collections;

/*
 * Copyright (c) 1997, 2013, Oracle and/or its affiliates. All rights reserved.
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
	/// A <code>BeanContextMembershipEvent</code> encapsulates
	/// the list of children added to, or removed from,
	/// the membership of a particular <code>BeanContext</code>.
	/// An instance of this event is fired whenever a successful
	/// add(), remove(), retainAll(), removeAll(), or clear() is
	/// invoked on a given <code>BeanContext</code> instance.
	/// Objects interested in receiving events of this type must
	/// implement the <code>BeanContextMembershipListener</code>
	/// interface, and must register their intent via the
	/// <code>BeanContext</code>'s
	/// <code>addBeanContextMembershipListener(BeanContextMembershipListener bcml)
	/// </code> method.
	/// 
	/// @author      Laurence P. G. Cable
	/// @since       1.2 </summary>
	/// <seealso cref=         java.beans.beancontext.BeanContext </seealso>
	/// <seealso cref=         java.beans.beancontext.BeanContextEvent </seealso>
	/// <seealso cref=         java.beans.beancontext.BeanContextMembershipListener </seealso>
	public class BeanContextMembershipEvent : BeanContextEvent
	{
		private const long SerialVersionUID = 3499346510334590959L;

		/// <summary>
		/// Contruct a BeanContextMembershipEvent
		/// </summary>
		/// <param name="bc">        The BeanContext source </param>
		/// <param name="changes">   The Children affected </param>
		/// <exception cref="NullPointerException"> if <CODE>changes</CODE> is <CODE>null</CODE> </exception>

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @SuppressWarnings("rawtypes") public BeanContextMembershipEvent(java.beans.beancontext.BeanContext bc, java.util.Collection changes)
		public BeanContextMembershipEvent(BeanContext bc, ICollection changes) : base(bc)
		{

			if (changes == null)
			{
				throw new NullPointerException("BeanContextMembershipEvent constructor:  changes is null.");
			}

			Children = changes;
		}

		/// <summary>
		/// Contruct a BeanContextMembershipEvent
		/// </summary>
		/// <param name="bc">        The BeanContext source </param>
		/// <param name="changes">   The Children effected </param>
		/// <exception cref="NullPointerException"> if changes associated with this
		///                  event are null. </exception>

		public BeanContextMembershipEvent(BeanContext bc, Object[] changes) : base(bc)
		{

			if (changes == null)
			{
				throw new NullPointerException("BeanContextMembershipEvent:  changes is null.");
			}

			Children = Arrays.AsList(changes);
		}

		/// <summary>
		/// Gets the number of children affected by the notification. </summary>
		/// <returns> the number of children affected by the notification </returns>
		public virtual int Size()
		{
			return Children.Count;
		}

		/// <summary>
		/// Is the child specified affected by the event? </summary>
		/// <returns> <code>true</code> if affected, <code>false</code>
		/// if not </returns>
		/// <param name="child"> the object to check for being affected </param>
		public virtual bool Contains(Object child)
		{
			return Children.Contains(child);
		}

		/// <summary>
		/// Gets the array of children affected by this event. </summary>
		/// <returns> the array of children affected </returns>
		public virtual Object[] ToArray()
		{
			return Children.ToArray();
		}

		/// <summary>
		/// Gets the array of children affected by this event. </summary>
		/// <returns> the array of children effected </returns>
//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @SuppressWarnings("rawtypes") public java.util.Iterator iterator()
		public virtual IEnumerator Iterator()
		/*
		 * fields
		 */
	   /// <summary>
	   /// The list of children affected by this
	   /// event notification.
	   /// </summary>
		{
			return Children.GetEnumerator();
		}
//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @SuppressWarnings("rawtypes") protected java.util.Collection children;
		protected internal ICollection Children;
	}

}
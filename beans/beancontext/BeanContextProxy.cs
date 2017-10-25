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
	/// This interface is implemented by a JavaBean that does
	/// not directly have a BeanContext(Child) associated with
	/// it (via implementing that interface or a subinterface thereof),
	/// but has a public BeanContext(Child) delegated from it.
	/// For example, a subclass of java.awt.Container may have a BeanContext
	/// associated with it that all Component children of that Container shall
	/// be contained within.
	/// </para>
	/// <para>
	/// An Object may not implement this interface and the
	/// BeanContextChild interface
	/// (or any subinterfaces thereof) they are mutually exclusive.
	/// </para>
	/// <para>
	/// Callers of this interface shall examine the return type in order to
	/// obtain a particular subinterface of BeanContextChild as follows:
	/// <code>
	/// BeanContextChild bcc = o.getBeanContextProxy();
	/// 
	/// if (bcc instanceof BeanContext) {
	///      // ...
	/// }
	/// </code>
	/// or
	/// <code>
	/// BeanContextChild bcc = o.getBeanContextProxy();
	/// BeanContext      bc  = null;
	/// 
	/// try {
	///     bc = (BeanContext)bcc;
	/// } catch (ClassCastException cce) {
	///     // cast failed, bcc is not an instanceof BeanContext
	/// }
	/// </code>
	/// </para>
	/// <para>
	/// The return value is a constant for the lifetime of the implementing
	/// instance
	/// </para>
	/// @author Laurence P. G. Cable
	/// @since 1.2
	/// </summary>
	/// <seealso cref= java.beans.beancontext.BeanContextChild </seealso>
	/// <seealso cref= java.beans.beancontext.BeanContextChildSupport </seealso>

	public interface BeanContextProxy
	{

		/// <summary>
		/// Gets the <code>BeanContextChild</code> (or subinterface)
		/// associated with this object. </summary>
		/// <returns> the <code>BeanContextChild</code> (or subinterface)
		/// associated with this object </returns>
		BeanContextChild BeanContextProxy {get;}
	}

}
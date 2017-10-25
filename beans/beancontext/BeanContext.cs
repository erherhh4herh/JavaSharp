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
	/// <para>
	/// The BeanContext acts a logical hierarchical container for JavaBeans.
	/// </para>
	/// 
	/// @author Laurence P. G. Cable
	/// @since 1.2
	/// </summary>
	/// <seealso cref= java.beans.Beans </seealso>
	/// <seealso cref= java.beans.beancontext.BeanContextChild </seealso>
	/// <seealso cref= java.beans.beancontext.BeanContextMembershipListener </seealso>
	/// <seealso cref= java.beans.PropertyChangeEvent </seealso>
	/// <seealso cref= java.beans.DesignMode </seealso>
	/// <seealso cref= java.beans.Visibility </seealso>
	/// <seealso cref= java.util.Collection </seealso>

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @SuppressWarnings("rawtypes") public interface BeanContext extends BeanContextChild, java.util.Collection, java.beans.DesignMode, java.beans.Visibility
	public interface BeanContext : BeanContextChild, ICollection, DesignMode, Visibility
	{

		/// <summary>
		/// Instantiate the javaBean named as a
		/// child of this <code>BeanContext</code>.
		/// The implementation of the JavaBean is
		/// derived from the value of the beanName parameter,
		/// and is defined by the
		/// <code>java.beans.Beans.instantiate()</code> method.
		/// </summary>
		/// <returns> a javaBean named as a child of this
		/// <code>BeanContext</code> </returns>
		/// <param name="beanName"> The name of the JavaBean to instantiate
		/// as a child of this <code>BeanContext</code> </param>
		/// <exception cref="IOException"> if an IO problem occurs </exception>
		/// <exception cref="ClassNotFoundException"> if the class identified
		/// by the beanName parameter is not found </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: Object instantiateChild(String beanName) throws java.io.IOException, ClassNotFoundException;
		Object InstantiateChild(String beanName);

		/// <summary>
		/// Analagous to <code>java.lang.ClassLoader.getResourceAsStream()</code>,
		/// this method allows a <code>BeanContext</code> implementation
		/// to interpose behavior between the child <code>Component</code>
		/// and underlying <code>ClassLoader</code>.
		/// </summary>
		/// <param name="name"> the resource name </param>
		/// <param name="bcc"> the specified child </param>
		/// <returns> an <code>InputStream</code> for reading the resource,
		/// or <code>null</code> if the resource could not
		/// be found. </returns>
		/// <exception cref="IllegalArgumentException"> if
		/// the resource is not valid </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: java.io.InputStream getResourceAsStream(String name, BeanContextChild bcc) throws IllegalArgumentException;
		InputStream GetResourceAsStream(String name, BeanContextChild bcc);

		/// <summary>
		/// Analagous to <code>java.lang.ClassLoader.getResource()</code>, this
		/// method allows a <code>BeanContext</code> implementation to interpose
		/// behavior between the child <code>Component</code>
		/// and underlying <code>ClassLoader</code>.
		/// </summary>
		/// <param name="name"> the resource name </param>
		/// <param name="bcc"> the specified child </param>
		/// <returns> a <code>URL</code> for the named
		/// resource for the specified child </returns>
		/// <exception cref="IllegalArgumentException">
		/// if the resource is not valid </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: java.net.URL getResource(String name, BeanContextChild bcc) throws IllegalArgumentException;
		URL GetResource(String name, BeanContextChild bcc);

		 /// <summary>
		 /// Adds the specified <code>BeanContextMembershipListener</code>
		 /// to receive <code>BeanContextMembershipEvents</code> from
		 /// this <code>BeanContext</code> whenever it adds
		 /// or removes a child <code>Component</code>(s).
		 /// </summary>
		 /// <param name="bcml"> the BeanContextMembershipListener to be added </param>
		void AddBeanContextMembershipListener(BeanContextMembershipListener bcml);

		 /// <summary>
		 /// Removes the specified <code>BeanContextMembershipListener</code>
		 /// so that it no longer receives <code>BeanContextMembershipEvent</code>s
		 /// when the child <code>Component</code>(s) are added or removed.
		 /// </summary>
		 /// <param name="bcml"> the <code>BeanContextMembershipListener</code>
		 /// to be removed </param>
		void RemoveBeanContextMembershipListener(BeanContextMembershipListener bcml);

		/// <summary>
		/// This global lock is used by both <code>BeanContext</code>
		/// and <code>BeanContextServices</code> implementors
		/// to serialize changes in a <code>BeanContext</code>
		/// hierarchy and any service requests etc.
		/// </summary>
	}

	public static class BeanContext_Fields
	{
		public static readonly Object GlobalHierarchyLock = new Object();
	}

}
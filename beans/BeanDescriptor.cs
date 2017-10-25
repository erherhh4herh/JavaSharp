/*
 * Copyright (c) 1996, 2011, Oracle and/or its affiliates. All rights reserved.
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

namespace java.beans
{

	/// <summary>
	/// A BeanDescriptor provides global information about a "bean",
	/// including its Java class, its displayName, etc.
	/// <para>
	/// This is one of the kinds of descriptor returned by a BeanInfo object,
	/// which also returns descriptors for properties, method, and events.
	/// </para>
	/// </summary>

	public class BeanDescriptor : FeatureDescriptor
	{

//JAVA TO C# CONVERTER TODO TASK: Java wildcard generics are not converted to .NET:
//ORIGINAL LINE: private Reference<? extends Class> beanClassRef;
		private Reference<?> BeanClassRef;
//JAVA TO C# CONVERTER TODO TASK: Java wildcard generics are not converted to .NET:
//ORIGINAL LINE: private Reference<? extends Class> customizerClassRef;
		private Reference<?> CustomizerClassRef;

		/// <summary>
		/// Create a BeanDescriptor for a bean that doesn't have a customizer.
		/// </summary>
		/// <param name="beanClass">  The Class object of the Java class that implements
		///          the bean.  For example sun.beans.OurButton.class. </param>
		public BeanDescriptor(Class beanClass) : this(beanClass, null)
		{
		}

		/// <summary>
		/// Create a BeanDescriptor for a bean that has a customizer.
		/// </summary>
		/// <param name="beanClass">  The Class object of the Java class that implements
		///          the bean.  For example sun.beans.OurButton.class. </param>
		/// <param name="customizerClass">  The Class object of the Java class that implements
		///          the bean's Customizer.  For example sun.beans.OurButtonCustomizer.class. </param>
		public BeanDescriptor(Class beanClass, Class customizerClass)
		{
			this.BeanClassRef = GetWeakReference(beanClass);
			this.CustomizerClassRef = GetWeakReference(customizerClass);

			String name = beanClass.Name;
			while (name.IndexOf('.') >= 0)
			{
				name = name.Substring(name.IndexOf('.') + 1);
			}
			Name = name;
		}

		/// <summary>
		/// Gets the bean's Class object.
		/// </summary>
		/// <returns> The Class object for the bean. </returns>
		public virtual Class BeanClass
		{
			get
			{
				return (this.BeanClassRef != null) ? this.BeanClassRef.get() : null;
			}
		}

		/// <summary>
		/// Gets the Class object for the bean's customizer.
		/// </summary>
		/// <returns> The Class object for the bean's customizer.  This may
		/// be null if the bean doesn't have a customizer. </returns>
		public virtual Class CustomizerClass
		{
			get
			{
				return (this.CustomizerClassRef != null) ? this.CustomizerClassRef.get() : null;
			}
		}

		/*
		 * Package-private dup constructor
		 * This must isolate the new object from any changes to the old object.
		 */
		internal BeanDescriptor(BeanDescriptor old) : base(old)
		{
			BeanClassRef = old.BeanClassRef;
			CustomizerClassRef = old.CustomizerClassRef;
		}

		internal override void AppendTo(StringBuilder sb)
		{
			AppendTo(sb, "beanClass", this.BeanClassRef);
			AppendTo(sb, "customizerClass", this.CustomizerClassRef);
		}
	}

}
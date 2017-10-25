/*
 * Copyright (c) 1996, 2012, Oracle and/or its affiliates. All rights reserved.
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
	/// Use the {@code BeanInfo} interface
	/// to create a {@code BeanInfo} class
	/// and provide explicit information about the methods,
	/// properties, events, and other features of your beans.
	/// <para>
	/// When developing your bean, you can implement
	/// the bean features required for your application task
	/// omitting the rest of the {@code BeanInfo} features.
	/// They will be obtained through the automatic analysis
	/// by using the low-level reflection of the bean methods
	/// and applying standard design patterns.
	/// You have an opportunity to provide additional bean information
	/// through various descriptor classes.
	/// </para>
	/// <para>
	/// See the <seealso cref="SimpleBeanInfo"/> class that is
	/// a convenient basic class for {@code BeanInfo} classes.
	/// You can override the methods and properties of
	/// the {@code SimpleBeanInfo} class to define specific information.
	/// </para>
	/// <para>
	/// See also the <seealso cref="Introspector"/> class to learn more about bean behavior.
	/// </para>
	/// </summary>
	public interface BeanInfo
	{

		/// <summary>
		/// Returns the bean descriptor
		/// that provides overall information about the bean,
		/// such as its display name or its customizer.
		/// </summary>
		/// <returns>  a <seealso cref="BeanDescriptor"/> object,
		///          or {@code null} if the information is to
		///          be obtained through the automatic analysis </returns>
		BeanDescriptor BeanDescriptor {get;}

		/// <summary>
		/// Returns the event descriptors of the bean
		/// that define the types of events fired by this bean.
		/// </summary>
		/// <returns>  an array of <seealso cref="EventSetDescriptor"/> objects,
		///          or {@code null} if the information is to
		///          be obtained through the automatic analysis </returns>
		EventSetDescriptor[] EventSetDescriptors {get;}

		/// <summary>
		/// A bean may have a default event typically applied when this bean is used.
		/// </summary>
		/// <returns>  index of the default event in the {@code EventSetDescriptor} array
		///          returned by the {@code getEventSetDescriptors} method,
		///          or -1 if there is no default event </returns>
		int DefaultEventIndex {get;}

		/// <summary>
		/// Returns descriptors for all properties of the bean.
		/// <para>
		/// If a property is indexed, then its entry in the result array
		/// belongs to the <seealso cref="IndexedPropertyDescriptor"/> subclass
		/// of the <seealso cref="PropertyDescriptor"/> class.
		/// A client of the {@code getPropertyDescriptors} method
		/// can use the {@code instanceof} operator to check
		/// whether a given {@code PropertyDescriptor}
		/// is an {@code IndexedPropertyDescriptor}.
		/// 
		/// </para>
		/// </summary>
		/// <returns>  an array of {@code PropertyDescriptor} objects,
		///          or {@code null} if the information is to
		///          be obtained through the automatic analysis </returns>
		PropertyDescriptor[] PropertyDescriptors {get;}

		/// <summary>
		/// A bean may have a default property commonly updated when this bean is customized.
		/// </summary>
		/// <returns>  index of the default property in the {@code PropertyDescriptor} array
		///          returned by the {@code getPropertyDescriptors} method,
		///          or -1 if there is no default property </returns>
		int DefaultPropertyIndex {get;}

		/// <summary>
		/// Returns the method descriptors of the bean
		/// that define the externally visible methods supported by this bean.
		/// </summary>
		/// <returns>  an array of <seealso cref="MethodDescriptor"/> objects,
		///          or {@code null} if the information is to
		///          be obtained through the automatic analysis </returns>
		MethodDescriptor[] MethodDescriptors {get;}

		/// <summary>
		/// This method enables the current {@code BeanInfo} object
		/// to return an arbitrary collection of other {@code BeanInfo} objects
		/// that provide additional information about the current bean.
		/// <para>
		/// If there are conflicts or overlaps between the information
		/// provided by different {@code BeanInfo} objects,
		/// the current {@code BeanInfo} object takes priority
		/// over the additional {@code BeanInfo} objects.
		/// Array elements with higher indices take priority
		/// over the elements with lower indices.
		/// 
		/// </para>
		/// </summary>
		/// <returns>  an array of {@code BeanInfo} objects,
		///          or {@code null} if there are no additional {@code BeanInfo} objects </returns>
		BeanInfo[] AdditionalBeanInfo {get;}

		/// <summary>
		/// Returns an image that can be used to represent the bean in toolboxes or toolbars.
		/// <para>
		/// There are four possible types of icons:
		/// 16 x 16 color, 32 x 32 color, 16 x 16 mono, and 32 x 32 mono.
		/// If you implement a bean so that it supports a single icon,
		/// it is recommended to use 16 x 16 color.
		/// Another recommendation is to set a transparent background for the icons.
		/// 
		/// </para>
		/// </summary>
		/// <param name="iconKind">  the kind of icon requested </param>
		/// <returns>           an image object representing the requested icon,
		///                   or {@code null} if no suitable icon is available
		/// </returns>
		/// <seealso cref= #ICON_COLOR_16x16 </seealso>
		/// <seealso cref= #ICON_COLOR_32x32 </seealso>
		/// <seealso cref= #ICON_MONO_16x16 </seealso>
		/// <seealso cref= #ICON_MONO_32x32 </seealso>
		Image GetIcon(int iconKind);

		/// <summary>
		/// Constant to indicate a 16 x 16 color icon.
		/// </summary>

		/// <summary>
		/// Constant to indicate a 32 x 32 color icon.
		/// </summary>

		/// <summary>
		/// Constant to indicate a 16 x 16 monochrome icon.
		/// </summary>

		/// <summary>
		/// Constant to indicate a 32 x 32 monochrome icon.
		/// </summary>
	}

	public static class BeanInfo_Fields
	{
		public const int ICON_COLOR_16x16 = 1;
		public const int ICON_COLOR_32x32 = 2;
		public const int ICON_MONO_16x16 = 3;
		public const int ICON_MONO_32x32 = 4;
	}

}
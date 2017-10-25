/*
 * Copyright (c) 2012, Oracle and/or its affiliates. All rights reserved.
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

namespace java.util.spi
{

	/// <summary>
	/// An interface for service providers that provide implementations of {@link
	/// java.util.ResourceBundle.Control}. The <a
	/// href="../ResourceBundle.html#default_behavior">default resource bundle loading
	/// behavior</a> of the {@code ResourceBundle.getBundle} factory methods that take
	/// no <seealso cref="java.util.ResourceBundle.Control"/> instance can be modified with {@code
	/// ResourceBundleControlProvider} implementations.
	/// 
	/// <para>Provider implementations must be packaged using the <a
	/// href="../../../../technotes/guides/extensions/index.html">Java Extension
	/// Mechanism</a> as installed extensions. Refer to <seealso cref="java.util.ServiceLoader"/>
	/// for the extension packaging. Any installed {@code
	/// ResourceBundleControlProvider} implementations are loaded using {@link
	/// java.util.ServiceLoader} at the {@code ResourceBundle} class loading time.
	/// 
	/// @author Masayoshi Okutsu
	/// @since 1.8
	/// </para>
	/// </summary>
	/// <seealso cref= ResourceBundle#getBundle(String, java.util.Locale, ClassLoader, ResourceBundle.Control)
	///      ResourceBundle.getBundle </seealso>
	/// <seealso cref= java.util.ServiceLoader#loadInstalled(Class) </seealso>
	public interface ResourceBundleControlProvider
	{
		/// <summary>
		/// Returns a {@code ResourceBundle.Control} instance that is used
		/// to handle resource bundle loading for the given {@code
		/// baseName}. This method must return {@code null} if the given
		/// {@code baseName} isn't handled by this provider.
		/// </summary>
		/// <param name="baseName"> the base name of the resource bundle </param>
		/// <returns> a {@code ResourceBundle.Control} instance,
		///         or {@code null} if the given {@code baseName} is not
		///         applicable to this provider. </returns>
		/// <exception cref="NullPointerException"> if {@code baseName} is {@code null} </exception>
		ResourceBundle.Control GetControl(String baseName);
	}

}
/*
 * Copyright (c) 1998, 1999, Oracle and/or its affiliates. All rights reserved.
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


namespace java.awt.im.spi
{


	/// <summary>
	/// Defines methods that provide sufficient information about an input method
	/// to enable selection and loading of that input method.
	/// The input method itself is only loaded when it is actually used.
	/// 
	/// @since 1.3
	/// </summary>

	public interface InputMethodDescriptor
	{

		/// <summary>
		/// Returns the locales supported by the corresponding input method.
		/// The locale may describe just the language, or may also include
		/// country and variant information if needed.
		/// The information is used to select input methods by locale
		/// (<seealso cref="java.awt.im.InputContext#selectInputMethod(Locale)"/>). It may also
		/// be used to sort input methods by locale in a user-visible
		/// list of input methods.
		/// <para>
		/// Only the input method's primary locales should be returned.
		/// For example, if a Japanese input method also has a pass-through
		/// mode for Roman characters, typically still only Japanese would
		/// be returned. Thus, the list of locales returned is typically
		/// a subset of the locales for which the corresponding input method's
		/// implementation of <seealso cref="java.awt.im.spi.InputMethod#setLocale"/> returns true.
		/// </para>
		/// <para>
		/// If <seealso cref="#hasDynamicLocaleList"/> returns true, this method is
		/// called each time the information is needed. This
		/// gives input methods that depend on network resources the chance
		/// to add or remove locales as resources become available or
		/// unavailable.
		/// 
		/// </para>
		/// </summary>
		/// <returns> the locales supported by the input method </returns>
		/// <exception cref="AWTException"> if it can be determined that the input method
		/// is inoperable, for example, because of incomplete installation. </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: java.util.Locale[] getAvailableLocales() throws java.awt.AWTException;
		Locale[] AvailableLocales {get;}

		/// <summary>
		/// Returns whether the list of available locales can change
		/// at runtime. This may be the case, for example, for adapters
		/// that access real input methods over the network.
		/// </summary>
		bool HasDynamicLocaleList();

		/// <summary>
		/// Returns the user-visible name of the corresponding
		/// input method for the given input locale in the language in which
		/// the name will be displayed.
		/// <para>
		/// The inputLocale parameter specifies the locale for which text
		/// is input.
		/// This parameter can only take values obtained from this descriptor's
		/// <seealso cref="#getAvailableLocales"/> method or null. If it is null, an
		/// input locale independent name for the input method should be
		/// returned.
		/// </para>
		/// <para>
		/// If a name for the desired display language is not available, the
		/// method may fall back to some other language.
		/// 
		/// </para>
		/// </summary>
		/// <param name="inputLocale"> the locale for which text input is supported, or null </param>
		/// <param name="displayLanguage"> the language in which the name will be displayed </param>
		String GetInputMethodDisplayName(Locale inputLocale, Locale displayLanguage);

		/// <summary>
		/// Returns an icon for the corresponding input method.
		/// The icon may be used by a user interface for selecting input methods.
		/// <para>
		/// The inputLocale parameter specifies the locale for which text
		/// is input.
		/// This parameter can only take values obtained from this descriptor's
		/// <seealso cref="#getAvailableLocales"/> method or null. If it is null, an
		/// input locale independent icon for the input method should be
		/// returned.
		/// </para>
		/// <para>
		/// The icon's size should be 16&times;16 pixels.
		/// 
		/// </para>
		/// </summary>
		/// <param name="inputLocale"> the locale for which text input is supported, or null </param>
		/// <returns> an icon for the corresponding input method, or null </returns>
		Image GetInputMethodIcon(Locale inputLocale);

		/// <summary>
		/// Creates a new instance of the corresponding input method.
		/// </summary>
		/// <returns> a new instance of the corresponding input method </returns>
		/// <exception cref="Exception"> any exception that may occur while creating the
		/// input method instance </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: InputMethod createInputMethod() throws Exception;
		InputMethod CreateInputMethod();
	}

}
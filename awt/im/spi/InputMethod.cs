/*
 * Copyright (c) 1997, 2004, Oracle and/or its affiliates. All rights reserved.
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

	using Subset = Character.Subset;


	/// <summary>
	/// Defines the interface for an input method that supports complex text input.
	/// Input methods traditionally support text input for languages that have
	/// more characters than can be represented on a standard-size keyboard,
	/// such as Chinese, Japanese, and Korean. However, they may also be used to
	/// support phonetic text input for English or character reordering for Thai.
	/// <para>
	/// Subclasses of InputMethod can be loaded by the input method framework; they
	/// can then be selected either through the API
	/// (<seealso cref="java.awt.im.InputContext#selectInputMethod InputContext.selectInputMethod"/>)
	/// or the user interface (the input method selection menu).
	/// 
	/// @since 1.3
	/// 
	/// @author JavaSoft International
	/// </para>
	/// </summary>

	public interface InputMethod
	{

		/// <summary>
		/// Sets the input method context, which is used to dispatch input method
		/// events to the client component and to request information from
		/// the client component.
		/// <para>
		/// This method is called once immediately after instantiating this input
		/// method.
		/// 
		/// </para>
		/// </summary>
		/// <param name="context"> the input method context for this input method </param>
		/// <exception cref="NullPointerException"> if <code>context</code> is null </exception>
		InputMethodContext InputMethodContext {set;}

		/// <summary>
		/// Attempts to set the input locale. If the input method supports the
		/// desired locale, it changes its behavior to support input for the locale
		/// and returns true.
		/// Otherwise, it returns false and does not change its behavior.
		/// <para>
		/// This method is called
		/// <ul>
		/// <li>by <seealso cref="java.awt.im.InputContext#selectInputMethod InputContext.selectInputMethod"/>,
		/// <li>when switching to this input method through the user interface if the user
		///     specified a locale or if the previously selected input method's
		///     <seealso cref="java.awt.im.spi.InputMethod#getLocale getLocale"/> method
		///     returns a non-null value.
		/// </ul>
		/// 
		/// </para>
		/// </summary>
		/// <param name="locale"> locale to input </param>
		/// <returns> whether the specified locale is supported </returns>
		/// <exception cref="NullPointerException"> if <code>locale</code> is null </exception>
		bool SetLocale(Locale locale);

		/// <summary>
		/// Returns the current input locale. Might return null in exceptional cases.
		/// <para>
		/// This method is called
		/// <ul>
		/// <li>by <seealso cref="java.awt.im.InputContext#getLocale InputContext.getLocale"/> and
		/// <li>when switching from this input method to a different one through the
		///     user interface.
		/// </ul>
		/// 
		/// </para>
		/// </summary>
		/// <returns> the current input locale, or null </returns>
		Locale Locale {get;}

		/// <summary>
		/// Sets the subsets of the Unicode character set that this input method
		/// is allowed to input. Null may be passed in to indicate that all
		/// characters are allowed.
		/// <para>
		/// This method is called
		/// <ul>
		/// <li>immediately after instantiating this input method,
		/// <li>when switching to this input method from a different one, and
		/// <li>by <seealso cref="java.awt.im.InputContext#setCharacterSubsets InputContext.setCharacterSubsets"/>.
		/// </ul>
		/// 
		/// </para>
		/// </summary>
		/// <param name="subsets"> the subsets of the Unicode character set from which
		/// characters may be input </param>
		Subset[] CharacterSubsets {set;}

		/// <summary>
		/// Enables or disables this input method for composition,
		/// depending on the value of the parameter <code>enable</code>.
		/// <para>
		/// An input method that is enabled for composition interprets incoming
		/// events for both composition and control purposes, while a
		/// disabled input method does not interpret events for composition.
		/// Note however that events are passed on to the input method regardless
		/// whether it is enabled or not, and that an input method that is disabled
		/// for composition may still interpret events for control purposes,
		/// including to enable or disable itself for composition.
		/// </para>
		/// <para>
		/// For input methods provided by host operating systems, it is not always possible to
		/// determine whether this operation is supported. For example, an input method may enable
		/// composition only for some locales, and do nothing for other locales. For such input
		/// methods, it is possible that this method does not throw
		/// <seealso cref="java.lang.UnsupportedOperationException UnsupportedOperationException"/>,
		/// but also does not affect whether composition is enabled.
		/// </para>
		/// <para>
		/// This method is called
		/// <ul>
		/// <li>by <seealso cref="java.awt.im.InputContext#setCompositionEnabled InputContext.setCompositionEnabled"/>,
		/// <li>when switching to this input method from a different one using the
		///     user interface or
		///     <seealso cref="java.awt.im.InputContext#selectInputMethod InputContext.selectInputMethod"/>,
		///     if the previously selected input method's
		///     <seealso cref="java.awt.im.spi.InputMethod#isCompositionEnabled isCompositionEnabled"/>
		///     method returns without throwing an exception.
		/// </ul>
		/// 
		/// </para>
		/// </summary>
		/// <param name="enable"> whether to enable the input method for composition </param>
		/// <exception cref="UnsupportedOperationException"> if this input method does not
		/// support the enabling/disabling operation </exception>
		/// <seealso cref= #isCompositionEnabled </seealso>
		bool CompositionEnabled {set;get;}


		/// <summary>
		/// Starts the reconversion operation. The input method obtains the
		/// text to be reconverted from the current client component using the
		/// <seealso cref="java.awt.im.InputMethodRequests#getSelectedText InputMethodRequests.getSelectedText"/>
		/// method. It can use other <code>InputMethodRequests</code>
		/// methods to request additional information required for the
		/// reconversion operation. The composed and committed text
		/// produced by the operation is sent to the client component as a
		/// sequence of <code>InputMethodEvent</code>s. If the given text
		/// cannot be reconverted, the same text should be sent to the
		/// client component as committed text.
		/// <para>
		/// This method is called by
		/// <seealso cref="java.awt.im.InputContext#reconvert() InputContext.reconvert"/>.
		/// 
		/// </para>
		/// </summary>
		/// <exception cref="UnsupportedOperationException"> if the input method does not
		/// support the reconversion operation. </exception>
		void Reconvert();

		/// <summary>
		/// Dispatches the event to the input method. If input method support is
		/// enabled for the focussed component, incoming events of certain types
		/// are dispatched to the current input method for this component before
		/// they are dispatched to the component's methods or event listeners.
		/// The input method decides whether it needs to handle the event. If it
		/// does, it also calls the event's <code>consume</code> method; this
		/// causes the event to not get dispatched to the component's event
		/// processing methods or event listeners.
		/// <para>
		/// Events are dispatched if they are instances of InputEvent or its
		/// subclasses.
		/// This includes instances of the AWT classes KeyEvent and MouseEvent.
		/// </para>
		/// <para>
		/// This method is called by <seealso cref="java.awt.im.InputContext#dispatchEvent InputContext.dispatchEvent"/>.
		/// 
		/// </para>
		/// </summary>
		/// <param name="event"> the event being dispatched to the input method </param>
		/// <exception cref="NullPointerException"> if <code>event</code> is null </exception>
		void DispatchEvent(AWTEvent @event);

		/// <summary>
		/// Notifies this input method of changes in the client window
		/// location or state. This method is called while this input
		/// method is the current input method of its input context and
		/// notifications for it are enabled (see {@link
		/// InputMethodContext#enableClientWindowNotification
		/// InputMethodContext.enableClientWindowNotification}). Calls
		/// to this method are temporarily suspended if the input context's
		/// <seealso cref="java.awt.im.InputContext#removeNotify removeNotify"/>
		/// method is called, and resume when the input method is activated
		/// for a new client component. It is called in the following
		/// situations:
		/// <ul>
		/// <li>
		/// when the window containing the current client component changes
		/// in location, size, visibility, iconification state, or when the
		/// window is closed.</li>
		/// <li>
		/// from <code> enableClientWindowNotification(inputMethod,
		/// true)</code> if the current client component exists,</li>
		/// <li>
		/// when activating the input method for the first time after it
		/// called
		/// <code>enableClientWindowNotification(inputMethod,
		/// true)</code> if during the call no current client component was
		/// available,</li>
		/// <li>
		/// when activating the input method for a new client component
		/// after the input context's removeNotify method has been
		/// called.</li>
		/// </ul> </summary>
		/// <param name="bounds"> client window's {@link
		/// java.awt.Component#getBounds bounds} on the screen; or null if
		/// the client window is iconified or invisible </param>
		void NotifyClientWindowChange(Rectangle bounds);

		/// <summary>
		/// Activates the input method for immediate input processing.
		/// <para>
		/// If an input method provides its own windows, it should make sure
		/// at this point that all necessary windows are open and visible.
		/// </para>
		/// <para>
		/// This method is called
		/// <ul>
		/// <li>by <seealso cref="java.awt.im.InputContext#dispatchEvent InputContext.dispatchEvent"/>
		///     when a client component receives a FOCUS_GAINED event,
		/// <li>when switching to this input method from a different one using the
		///     user interface or
		///     <seealso cref="java.awt.im.InputContext#selectInputMethod InputContext.selectInputMethod"/>.
		/// </ul>
		/// The method is only called when the input method is inactive.
		/// A newly instantiated input method is assumed to be inactive.
		/// </para>
		/// </summary>
		void Activate();

		/// <summary>
		/// Deactivates the input method.
		/// The isTemporary argument has the same meaning as in
		/// <seealso cref="java.awt.event.FocusEvent#isTemporary FocusEvent.isTemporary"/>.
		/// <para>
		/// If an input method provides its own windows, only windows that relate
		/// to the current composition (such as a lookup choice window) should be
		/// closed at this point.
		/// It is possible that the input method will be immediately activated again
		/// for a different client component, and closing and reopening more
		/// persistent windows (such as a control panel) would create unnecessary
		/// screen flicker.
		/// Before an instance of a different input method class is activated,
		/// <seealso cref="#hideWindows"/> is called on the current input method.
		/// </para>
		/// <para>
		/// This method is called
		/// <ul>
		/// <li>by <seealso cref="java.awt.im.InputContext#dispatchEvent InputContext.dispatchEvent"/>
		///     when a client component receives a FOCUS_LOST event,
		/// <li>when switching from this input method to a different one using the
		///     user interface or
		///     <seealso cref="java.awt.im.InputContext#selectInputMethod InputContext.selectInputMethod"/>,
		/// <li>before <seealso cref="#removeNotify removeNotify"/> if the current client component is
		///     removed.
		/// </ul>
		/// The method is only called when the input method is active.
		/// 
		/// </para>
		/// </summary>
		/// <param name="isTemporary"> whether the focus change is temporary </param>
		void Deactivate(bool isTemporary);

		/// <summary>
		/// Closes or hides all windows opened by this input method instance or
		/// its class.
		/// <para>
		/// This method is called
		/// <ul>
		/// <li>before calling <seealso cref="#activate activate"/> on an instance of a different input
		///     method class,
		/// <li>before calling <seealso cref="#dispose dispose"/> on this input method.
		/// </ul>
		/// The method is only called when the input method is inactive.
		/// </para>
		/// </summary>
		void HideWindows();

		/// <summary>
		/// Notifies the input method that a client component has been
		/// removed from its containment hierarchy, or that input method
		/// support has been disabled for the component.
		/// <para>
		/// This method is called by <seealso cref="java.awt.im.InputContext#removeNotify InputContext.removeNotify"/>.
		/// </para>
		/// <para>
		/// The method is only called when the input method is inactive.
		/// </para>
		/// </summary>
		void RemoveNotify();

		/// <summary>
		/// Ends any input composition that may currently be going on in this
		/// context. Depending on the platform and possibly user preferences,
		/// this may commit or delete uncommitted text. Any changes to the text
		/// are communicated to the active component using an input method event.
		/// 
		/// <para>
		/// A text editing component may call this in a variety of situations,
		/// for example, when the user moves the insertion point within the text
		/// (but outside the composed text), or when the component's text is
		/// saved to a file or copied to the clipboard.
		/// </para>
		/// <para>
		/// This method is called
		/// <ul>
		/// <li>by <seealso cref="java.awt.im.InputContext#endComposition InputContext.endComposition"/>,
		/// <li>by <seealso cref="java.awt.im.InputContext#dispatchEvent InputContext.dispatchEvent"/>
		///     when switching to a different client component
		/// <li>when switching from this input method to a different one using the
		///     user interface or
		///     <seealso cref="java.awt.im.InputContext#selectInputMethod InputContext.selectInputMethod"/>.
		/// </ul>
		/// </para>
		/// </summary>
		void EndComposition();

		/// <summary>
		/// Releases the resources used by this input method.
		/// In particular, the input method should dispose windows and close files that are no
		/// longer needed.
		/// <para>
		/// This method is called by <seealso cref="java.awt.im.InputContext#dispose InputContext.dispose"/>.
		/// </para>
		/// <para>
		/// The method is only called when the input method is inactive.
		/// No method of this interface is called on this instance after dispose.
		/// </para>
		/// </summary>
		void Dispose();

		/// <summary>
		/// Returns a control object from this input method, or null. A
		/// control object provides methods that control the behavior of the
		/// input method or obtain information from the input method. The type
		/// of the object is an input method specific class. Clients have to
		/// compare the result against known input method control object
		/// classes and cast to the appropriate class to invoke the methods
		/// provided.
		/// <para>
		/// This method is called by
		/// <seealso cref="java.awt.im.InputContext#getInputMethodControlObject InputContext.getInputMethodControlObject"/>.
		/// 
		/// </para>
		/// </summary>
		/// <returns> a control object from this input method, or null </returns>
		Object ControlObject {get;}

	}

}
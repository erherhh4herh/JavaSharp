using System;

/*
 * Copyright (c) 1995, 2013, Oracle and/or its affiliates. All rights reserved.
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
namespace java.awt
{

	using InputMethodSupport = sun.awt.InputMethodSupport;
	using SecurityConstants = sun.security.util.SecurityConstants;

	/// <summary>
	/// The <code>TextComponent</code> class is the superclass of
	/// any component that allows the editing of some text.
	/// <para>
	/// A text component embodies a string of text.  The
	/// <code>TextComponent</code> class defines a set of methods
	/// that determine whether or not this text is editable. If the
	/// component is editable, it defines another set of methods
	/// that supports a text insertion caret.
	/// </para>
	/// <para>
	/// In addition, the class defines methods that are used
	/// to maintain a current <em>selection</em> from the text.
	/// The text selection, a substring of the component's text,
	/// is the target of editing operations. It is also referred
	/// to as the <em>selected text</em>.
	/// 
	/// @author      Sami Shaio
	/// @author      Arthur van Hoff
	/// @since       JDK1.0
	/// </para>
	/// </summary>
	public class TextComponent : Component, Accessible
	{

		/// <summary>
		/// The value of the text.
		/// A <code>null</code> value is the same as "".
		/// 
		/// @serial </summary>
		/// <seealso cref= #setText(String) </seealso>
		/// <seealso cref= #getText() </seealso>
		internal String Text_Renamed;

		/// <summary>
		/// A boolean indicating whether or not this
		/// <code>TextComponent</code> is editable.
		/// It will be <code>true</code> if the text component
		/// is editable and <code>false</code> if not.
		/// 
		/// @serial </summary>
		/// <seealso cref= #isEditable() </seealso>
		internal bool Editable_Renamed = true;

		/// <summary>
		/// The selection refers to the selected text, and the
		/// <code>selectionStart</code> is the start position
		/// of the selected text.
		/// 
		/// @serial </summary>
		/// <seealso cref= #getSelectionStart() </seealso>
		/// <seealso cref= #setSelectionStart(int) </seealso>
		internal int SelectionStart_Renamed;

		/// <summary>
		/// The selection refers to the selected text, and the
		/// <code>selectionEnd</code>
		/// is the end position of the selected text.
		/// 
		/// @serial </summary>
		/// <seealso cref= #getSelectionEnd() </seealso>
		/// <seealso cref= #setSelectionEnd(int) </seealso>
		internal int SelectionEnd_Renamed;

		// A flag used to tell whether the background has been set by
		// developer code (as opposed to AWT code).  Used to determine
		// the background color of non-editable TextComponents.
		internal bool BackgroundSetByClientCode = false;

		[NonSerialized]
		protected internal TextListener TextListener;

		/*
		 * JDK 1.1 serialVersionUID
		 */
		private const long SerialVersionUID = -2214773872412987419L;

		/// <summary>
		/// Constructs a new text component initialized with the
		/// specified text. Sets the value of the cursor to
		/// <code>Cursor.TEXT_CURSOR</code>. </summary>
		/// <param name="text">       the text to be displayed; if
		///             <code>text</code> is <code>null</code>, the empty
		///             string <code>""</code> will be displayed </param>
		/// <exception cref="HeadlessException"> if
		///             <code>GraphicsEnvironment.isHeadless</code>
		///             returns true </exception>
		/// <seealso cref=        java.awt.GraphicsEnvironment#isHeadless </seealso>
		/// <seealso cref=        java.awt.Cursor </seealso>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: TextComponent(String text) throws HeadlessException
		internal TextComponent(String text)
		{
			GraphicsEnvironment.CheckHeadless();
			this.Text_Renamed = (text != null) ? text : "";
			Cursor = Cursor.GetPredefinedCursor(Cursor.TEXT_CURSOR);
		}

		private void EnableInputMethodsIfNecessary()
		{
			if (CheckForEnableIM)
			{
				CheckForEnableIM = false;
				try
				{
					Toolkit toolkit = Toolkit.DefaultToolkit;
					bool shouldEnable = false;
					if (toolkit is InputMethodSupport)
					{
						shouldEnable = ((InputMethodSupport)toolkit).enableInputMethodsForTextComponent();
					}
					EnableInputMethods(shouldEnable);
				}
				catch (Exception)
				{
					// if something bad happens, just don't enable input methods
				}
			}
		}

		/// <summary>
		/// Enables or disables input method support for this text component. If input
		/// method support is enabled and the text component also processes key events,
		/// incoming events are offered to the current input method and will only be
		/// processed by the component or dispatched to its listeners if the input method
		/// does not consume them. Whether and how input method support for this text
		/// component is enabled or disabled by default is implementation dependent.
		/// </summary>
		/// <param name="enable"> true to enable, false to disable </param>
		/// <seealso cref= #processKeyEvent
		/// @since 1.2 </seealso>
		public override void EnableInputMethods(bool enable)
		{
			CheckForEnableIM = false;
			base.EnableInputMethods(enable);
		}

		internal override bool AreInputMethodsEnabled()
		{
			// moved from the constructor above to here and addNotify below,
			// this call will initialize the toolkit if not already initialized.
			if (CheckForEnableIM)
			{
				EnableInputMethodsIfNecessary();
			}

			// TextComponent handles key events without touching the eventMask or
			// having a key listener, so just check whether the flag is set
			return (EventMask & AWTEvent.INPUT_METHODS_ENABLED_MASK) != 0;
		}

		public override InputMethodRequests InputMethodRequests
		{
			get
			{
				TextComponentPeer peer = (TextComponentPeer)this.Peer_Renamed;
				if (peer != null)
				{
					return peer.InputMethodRequests;
				}
				else
				{
					return null;
				}
			}
		}



		/// <summary>
		/// Makes this Component displayable by connecting it to a
		/// native screen resource.
		/// This method is called internally by the toolkit and should
		/// not be called directly by programs. </summary>
		/// <seealso cref=       java.awt.TextComponent#removeNotify </seealso>
		public override void AddNotify()
		{
			base.AddNotify();
			EnableInputMethodsIfNecessary();
		}

		/// <summary>
		/// Removes the <code>TextComponent</code>'s peer.
		/// The peer allows us to modify the appearance of the
		/// <code>TextComponent</code> without changing its
		/// functionality.
		/// </summary>
		public override void RemoveNotify()
		{
			lock (TreeLock)
			{
				TextComponentPeer peer = (TextComponentPeer)this.Peer_Renamed;
				if (peer != null)
				{
					Text_Renamed = peer.Text;
					SelectionStart_Renamed = peer.SelectionStart;
					SelectionEnd_Renamed = peer.SelectionEnd;
				}
				base.RemoveNotify();
			}
		}

		/// <summary>
		/// Sets the text that is presented by this
		/// text component to be the specified text. </summary>
		/// <param name="t">   the new text;
		///                  if this parameter is <code>null</code> then
		///                  the text is set to the empty string "" </param>
		/// <seealso cref=         java.awt.TextComponent#getText </seealso>
		public virtual String Text
		{
			set
			{
				lock (this)
				{
					bool skipTextEvent = (Text_Renamed == null || Text_Renamed.Empty) && (value == null || value.Empty);
					Text_Renamed = (value != null) ? value : "";
					TextComponentPeer peer = (TextComponentPeer)this.Peer_Renamed;
					// Please note that we do not want to post an event
					// if TextArea.setText() or TextField.setText() replaces an empty text
					// by an empty text, that is, if component's text remains unchanged.
					if (peer != null && !skipTextEvent)
					{
						peer.Text = Text_Renamed;
					}
				}
			}
			get
			{
				lock (this)
				{
					TextComponentPeer peer = (TextComponentPeer)this.Peer_Renamed;
					if (peer != null)
					{
						Text_Renamed = peer.Text;
					}
					return Text_Renamed;
				}
			}
		}


		/// <summary>
		/// Returns the selected text from the text that is
		/// presented by this text component. </summary>
		/// <returns>      the selected text of this text component </returns>
		/// <seealso cref=         java.awt.TextComponent#select </seealso>
		public virtual String SelectedText
		{
			get
			{
				lock (this)
				{
					return StringHelperClass.SubstringSpecial(Text, SelectionStart, SelectionEnd);
				}
			}
		}

		/// <summary>
		/// Indicates whether or not this text component is editable. </summary>
		/// <returns>     <code>true</code> if this text component is
		///                  editable; <code>false</code> otherwise. </returns>
		/// <seealso cref=        java.awt.TextComponent#setEditable
		/// @since      JDK1.0 </seealso>
		public virtual bool Editable
		{
			get
			{
				return Editable_Renamed;
			}
			set
			{
				lock (this)
				{
					if (Editable_Renamed == value)
					{
						return;
					}
            
					Editable_Renamed = value;
					TextComponentPeer peer = (TextComponentPeer)this.Peer_Renamed;
					if (peer != null)
					{
						peer.Editable = value;
					}
				}
			}
		}


		/// <summary>
		/// Gets the background color of this text component.
		/// 
		/// By default, non-editable text components have a background color
		/// of SystemColor.control.  This default can be overridden by
		/// calling setBackground.
		/// </summary>
		/// <returns> This text component's background color.
		///         If this text component does not have a background color,
		///         the background color of its parent is returned. </returns>
		/// <seealso cref= #setBackground(Color)
		/// @since JDK1.0 </seealso>
		public override Color Background
		{
			get
			{
				if (!Editable_Renamed && !BackgroundSetByClientCode)
				{
					return SystemColor.Control;
				}
    
				return base.Background;
			}
			set
			{
				BackgroundSetByClientCode = true;
				base.Background = value;
			}
		}


		/// <summary>
		/// Gets the start position of the selected text in
		/// this text component. </summary>
		/// <returns>      the start position of the selected text </returns>
		/// <seealso cref=         java.awt.TextComponent#setSelectionStart </seealso>
		/// <seealso cref=         java.awt.TextComponent#getSelectionEnd </seealso>
		public virtual int SelectionStart
		{
			get
			{
				lock (this)
				{
					TextComponentPeer peer = (TextComponentPeer)this.Peer_Renamed;
					if (peer != null)
					{
						SelectionStart_Renamed = peer.SelectionStart;
					}
					return SelectionStart_Renamed;
				}
			}
			set
			{
				lock (this)
				{
					/* Route through select method to enforce consistent policy
					 * between value and selectionEnd.
					 */
					Select(value, SelectionEnd);
				}
			}
		}


		/// <summary>
		/// Gets the end position of the selected text in
		/// this text component. </summary>
		/// <returns>      the end position of the selected text </returns>
		/// <seealso cref=         java.awt.TextComponent#setSelectionEnd </seealso>
		/// <seealso cref=         java.awt.TextComponent#getSelectionStart </seealso>
		public virtual int SelectionEnd
		{
			get
			{
				lock (this)
				{
					TextComponentPeer peer = (TextComponentPeer)this.Peer_Renamed;
					if (peer != null)
					{
						SelectionEnd_Renamed = peer.SelectionEnd;
					}
					return SelectionEnd_Renamed;
				}
			}
			set
			{
				lock (this)
				{
					/* Route through select method to enforce consistent policy
					 * between selectionStart and value.
					 */
					Select(SelectionStart, value);
				}
			}
		}


		/// <summary>
		/// Selects the text between the specified start and end positions.
		/// <para>
		/// This method sets the start and end positions of the
		/// selected text, enforcing the restriction that the start position
		/// must be greater than or equal to zero.  The end position must be
		/// greater than or equal to the start position, and less than or
		/// equal to the length of the text component's text.  The
		/// character positions are indexed starting with zero.
		/// The length of the selection is
		/// <code>endPosition</code> - <code>startPosition</code>, so the
		/// character at <code>endPosition</code> is not selected.
		/// If the start and end positions of the selected text are equal,
		/// all text is deselected.
		/// </para>
		/// <para>
		/// If the caller supplies values that are inconsistent or out of
		/// bounds, the method enforces these constraints silently, and
		/// without failure. Specifically, if the start position or end
		/// position is greater than the length of the text, it is reset to
		/// equal the text length. If the start position is less than zero,
		/// it is reset to zero, and if the end position is less than the
		/// start position, it is reset to the start position.
		/// 
		/// </para>
		/// </summary>
		/// <param name="selectionStart"> the zero-based index of the first
		///                   character (<code>char</code> value) to be selected </param>
		/// <param name="selectionEnd"> the zero-based end position of the
		///                   text to be selected; the character (<code>char</code> value) at
		///                   <code>selectionEnd</code> is not selected </param>
		/// <seealso cref=          java.awt.TextComponent#setSelectionStart </seealso>
		/// <seealso cref=          java.awt.TextComponent#setSelectionEnd </seealso>
		/// <seealso cref=          java.awt.TextComponent#selectAll </seealso>
		public virtual void Select(int selectionStart, int selectionEnd)
		{
			lock (this)
			{
				String text = Text;
				if (selectionStart < 0)
				{
					selectionStart = 0;
				}
				if (selectionStart > text.Length())
				{
					selectionStart = text.Length();
				}
				if (selectionEnd > text.Length())
				{
					selectionEnd = text.Length();
				}
				if (selectionEnd < selectionStart)
				{
					selectionEnd = selectionStart;
				}
        
				this.SelectionStart_Renamed = selectionStart;
				this.SelectionEnd_Renamed = selectionEnd;
        
				TextComponentPeer peer = (TextComponentPeer)this.Peer_Renamed;
				if (peer != null)
				{
					peer.Select(selectionStart, selectionEnd);
				}
			}
		}

		/// <summary>
		/// Selects all the text in this text component. </summary>
		/// <seealso cref=        java.awt.TextComponent#select </seealso>
		public virtual void SelectAll()
		{
			lock (this)
			{
				this.SelectionStart_Renamed = 0;
				this.SelectionEnd_Renamed = Text.Length();
        
				TextComponentPeer peer = (TextComponentPeer)this.Peer_Renamed;
				if (peer != null)
				{
					peer.Select(SelectionStart_Renamed, SelectionEnd_Renamed);
				}
			}
		}

		/// <summary>
		/// Sets the position of the text insertion caret.
		/// The caret position is constrained to be between 0
		/// and the last character of the text, inclusive.
		/// If the passed-in value is greater than this range,
		/// the value is set to the last character (or 0 if
		/// the <code>TextComponent</code> contains no text)
		/// and no error is returned.  If the passed-in value is
		/// less than 0, an <code>IllegalArgumentException</code>
		/// is thrown.
		/// </summary>
		/// <param name="position"> the position of the text insertion caret </param>
		/// <exception cref="IllegalArgumentException"> if <code>position</code>
		///               is less than zero
		/// @since        JDK1.1 </exception>
		public virtual int CaretPosition
		{
			set
			{
				lock (this)
				{
					if (value < 0)
					{
						throw new IllegalArgumentException("position less than zero.");
					}
            
					int maxposition = Text.Length();
					if (value > maxposition)
					{
						value = maxposition;
					}
            
					TextComponentPeer peer = (TextComponentPeer)this.Peer_Renamed;
					if (peer != null)
					{
						peer.CaretPosition = value;
					}
					else
					{
						Select(value, value);
					}
				}
			}
			get
			{
				lock (this)
				{
					TextComponentPeer peer = (TextComponentPeer)this.Peer_Renamed;
					int position = 0;
            
					if (peer != null)
					{
						position = peer.CaretPosition;
					}
					else
					{
						position = SelectionStart_Renamed;
					}
					int maxposition = Text.Length();
					if (position > maxposition)
					{
						position = maxposition;
					}
					return position;
				}
			}
		}


		/// <summary>
		/// Adds the specified text event listener to receive text events
		/// from this text component.
		/// If <code>l</code> is <code>null</code>, no exception is
		/// thrown and no action is performed.
		/// <para>Refer to <a href="doc-files/AWTThreadIssues.html#ListenersThreads"
		/// >AWT Threading Issues</a> for details on AWT's threading model.
		/// 
		/// </para>
		/// </summary>
		/// <param name="l"> the text event listener </param>
		/// <seealso cref=             #removeTextListener </seealso>
		/// <seealso cref=             #getTextListeners </seealso>
		/// <seealso cref=             java.awt.event.TextListener </seealso>
		public virtual void AddTextListener(TextListener l)
		{
			lock (this)
			{
				if (l == null)
				{
					return;
				}
				TextListener = AWTEventMulticaster.Add(TextListener, l);
				NewEventsOnly = true;
			}
		}

		/// <summary>
		/// Removes the specified text event listener so that it no longer
		/// receives text events from this text component
		/// If <code>l</code> is <code>null</code>, no exception is
		/// thrown and no action is performed.
		/// <para>Refer to <a href="doc-files/AWTThreadIssues.html#ListenersThreads"
		/// >AWT Threading Issues</a> for details on AWT's threading model.
		/// 
		/// </para>
		/// </summary>
		/// <param name="l">     the text listener </param>
		/// <seealso cref=             #addTextListener </seealso>
		/// <seealso cref=             #getTextListeners </seealso>
		/// <seealso cref=             java.awt.event.TextListener
		/// @since           JDK1.1 </seealso>
		public virtual void RemoveTextListener(TextListener l)
		{
			lock (this)
			{
				if (l == null)
				{
					return;
				}
				TextListener = AWTEventMulticaster.Remove(TextListener, l);
			}
		}

		/// <summary>
		/// Returns an array of all the text listeners
		/// registered on this text component.
		/// </summary>
		/// <returns> all of this text component's <code>TextListener</code>s
		///         or an empty array if no text
		///         listeners are currently registered
		/// 
		/// </returns>
		/// <seealso cref= #addTextListener </seealso>
		/// <seealso cref= #removeTextListener
		/// @since 1.4 </seealso>
		public virtual TextListener[] TextListeners
		{
			get
			{
				lock (this)
				{
					return GetListeners(typeof(TextListener));
				}
			}
		}

		/// <summary>
		/// Returns an array of all the objects currently registered
		/// as <code><em>Foo</em>Listener</code>s
		/// upon this <code>TextComponent</code>.
		/// <code><em>Foo</em>Listener</code>s are registered using the
		/// <code>add<em>Foo</em>Listener</code> method.
		/// 
		/// <para>
		/// You can specify the <code>listenerType</code> argument
		/// with a class literal, such as
		/// <code><em>Foo</em>Listener.class</code>.
		/// For example, you can query a
		/// <code>TextComponent</code> <code>t</code>
		/// for its text listeners with the following code:
		/// 
		/// <pre>TextListener[] tls = (TextListener[])(t.getListeners(TextListener.class));</pre>
		/// 
		/// If no such listeners exist, this method returns an empty array.
		/// 
		/// </para>
		/// </summary>
		/// <param name="listenerType"> the type of listeners requested; this parameter
		///          should specify an interface that descends from
		///          <code>java.util.EventListener</code> </param>
		/// <returns> an array of all objects registered as
		///          <code><em>Foo</em>Listener</code>s on this text component,
		///          or an empty array if no such
		///          listeners have been added </returns>
		/// <exception cref="ClassCastException"> if <code>listenerType</code>
		///          doesn't specify a class or interface that implements
		///          <code>java.util.EventListener</code>
		/// </exception>
		/// <seealso cref= #getTextListeners
		/// @since 1.3 </seealso>
		public override T[] getListeners<T>(Class listenerType) where T : java.util.EventListener
		{
			EventListener l = null;
			if (listenerType == typeof(TextListener))
			{
				l = TextListener;
			}
			else
			{
				return base.GetListeners(listenerType);
			}
			return AWTEventMulticaster.GetListeners(l, listenerType);
		}

		// REMIND: remove when filtering is done at lower level
		internal override bool EventEnabled(AWTEvent e)
		{
			if (e.Id == TextEvent.TEXT_VALUE_CHANGED)
			{
				if ((EventMask & AWTEvent.TEXT_EVENT_MASK) != 0 || TextListener != null)
				{
					return true;
				}
				return false;
			}
			return base.EventEnabled(e);
		}

		/// <summary>
		/// Processes events on this text component. If the event is a
		/// <code>TextEvent</code>, it invokes the <code>processTextEvent</code>
		/// method else it invokes its superclass's <code>processEvent</code>.
		/// <para>Note that if the event parameter is <code>null</code>
		/// the behavior is unspecified and may result in an
		/// exception.
		/// 
		/// </para>
		/// </summary>
		/// <param name="e"> the event </param>
		protected internal override void ProcessEvent(AWTEvent e)
		{
			if (e is TextEvent)
			{
				ProcessTextEvent((TextEvent)e);
				return;
			}
			base.ProcessEvent(e);
		}

		/// <summary>
		/// Processes text events occurring on this text component by
		/// dispatching them to any registered <code>TextListener</code> objects.
		/// <para>
		/// NOTE: This method will not be called unless text events
		/// are enabled for this component. This happens when one of the
		/// following occurs:
		/// <ul>
		/// <li>A <code>TextListener</code> object is registered
		/// via <code>addTextListener</code>
		/// <li>Text events are enabled via <code>enableEvents</code>
		/// </ul>
		/// </para>
		/// <para>Note that if the event parameter is <code>null</code>
		/// the behavior is unspecified and may result in an
		/// exception.
		/// 
		/// </para>
		/// </summary>
		/// <param name="e"> the text event </param>
		/// <seealso cref= Component#enableEvents </seealso>
		protected internal virtual void ProcessTextEvent(TextEvent e)
		{
			TextListener listener = TextListener;
			if (listener != null)
			{
				int id = e.ID;
				switch (id)
				{
				case TextEvent.TEXT_VALUE_CHANGED:
					listener.TextValueChanged(e);
					break;
				}
			}
		}

		/// <summary>
		/// Returns a string representing the state of this
		/// <code>TextComponent</code>. This
		/// method is intended to be used only for debugging purposes, and the
		/// content and format of the returned string may vary between
		/// implementations. The returned string may be empty but may not be
		/// <code>null</code>.
		/// </summary>
		/// <returns>      the parameter string of this text component </returns>
		protected internal override String ParamString()
		{
			String str = base.ParamString() + ",text=" + Text;
			if (Editable_Renamed)
			{
				str += ",editable";
			}
			return str + ",selection=" + SelectionStart + "-" + SelectionEnd;
		}

		/// <summary>
		/// Assigns a valid value to the canAccessClipboard instance variable.
		/// </summary>
		private bool CanAccessClipboard()
		{
			SecurityManager sm = System.SecurityManager;
			if (sm == null)
			{
				return true;
			}
			try
			{
				sm.CheckPermission(SecurityConstants.AWT.ACCESS_CLIPBOARD_PERMISSION);
				return true;
			}
			catch (SecurityException)
			{
			}
			return false;
		}

		/*
		 * Serialization support.
		 */
		/// <summary>
		/// The textComponent SerializedDataVersion.
		/// 
		/// @serial
		/// </summary>
		private int TextComponentSerializedDataVersion = 1;

		/// <summary>
		/// Writes default serializable fields to stream.  Writes
		/// a list of serializable TextListener(s) as optional data.
		/// The non-serializable TextListener(s) are detected and
		/// no attempt is made to serialize them.
		/// 
		/// @serialData Null terminated sequence of zero or more pairs.
		///             A pair consists of a String and Object.
		///             The String indicates the type of object and
		///             is one of the following :
		///             textListenerK indicating and TextListener object.
		/// </summary>
		/// <seealso cref= AWTEventMulticaster#save(ObjectOutputStream, String, EventListener) </seealso>
		/// <seealso cref= java.awt.Component#textListenerK </seealso>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: private void writeObject(java.io.ObjectOutputStream s) throws java.io.IOException
		private void WriteObject(ObjectOutputStream s)
		{
			// Serialization support.  Since the value of the fields
			// selectionStart, selectionEnd, and text aren't necessarily
			// up to date, we sync them up with the peer before serializing.
			TextComponentPeer peer = (TextComponentPeer)this.Peer_Renamed;
			if (peer != null)
			{
				Text_Renamed = peer.Text;
				SelectionStart_Renamed = peer.SelectionStart;
				SelectionEnd_Renamed = peer.SelectionEnd;
			}

			s.DefaultWriteObject();

			AWTEventMulticaster.Save(s, TextListenerK, TextListener);
			s.WriteObject(null);
		}

		/// <summary>
		/// Read the ObjectInputStream, and if it isn't null,
		/// add a listener to receive text events fired by the
		/// TextComponent.  Unrecognized keys or values will be
		/// ignored.
		/// </summary>
		/// <exception cref="HeadlessException"> if
		/// <code>GraphicsEnvironment.isHeadless()</code> returns
		/// <code>true</code> </exception>
		/// <seealso cref= #removeTextListener </seealso>
		/// <seealso cref= #addTextListener </seealso>
		/// <seealso cref= java.awt.GraphicsEnvironment#isHeadless </seealso>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: private void readObject(java.io.ObjectInputStream s) throws ClassNotFoundException, java.io.IOException, HeadlessException
		private void ReadObject(ObjectInputStream s)
		{
			GraphicsEnvironment.CheckHeadless();
			s.DefaultReadObject();

			// Make sure the state we just read in for text,
			// selectionStart and selectionEnd has legal values
			this.Text_Renamed = (Text_Renamed != null) ? Text_Renamed : "";
			Select(SelectionStart_Renamed, SelectionEnd_Renamed);

			Object keyOrNull;
			while (null != (keyOrNull = s.ReadObject()))
			{
				String key = ((String)keyOrNull).intern();

				if (TextListenerK == key)
				{
					AddTextListener((TextListener)(s.ReadObject()));
				}
				else
				{
					// skip value for unrecognized key
					s.ReadObject();
				}
			}
			EnableInputMethodsIfNecessary();
		}


	/////////////////
	// Accessibility support
	////////////////

		/// <summary>
		/// Gets the AccessibleContext associated with this TextComponent.
		/// For text components, the AccessibleContext takes the form of an
		/// AccessibleAWTTextComponent.
		/// A new AccessibleAWTTextComponent instance is created if necessary.
		/// </summary>
		/// <returns> an AccessibleAWTTextComponent that serves as the
		///         AccessibleContext of this TextComponent
		/// @since 1.3 </returns>
		public override AccessibleContext AccessibleContext
		{
			get
			{
				if (AccessibleContext_Renamed == null)
				{
					AccessibleContext_Renamed = new AccessibleAWTTextComponent(this);
				}
				return AccessibleContext_Renamed;
			}
		}

		/// <summary>
		/// This class implements accessibility support for the
		/// <code>TextComponent</code> class.  It provides an implementation of the
		/// Java Accessibility API appropriate to text component user-interface
		/// elements.
		/// @since 1.3
		/// </summary>
		protected internal class AccessibleAWTTextComponent : AccessibleAWTComponent, AccessibleText, TextListener
		{
			private readonly TextComponent OuterInstance;

			/*
			 * JDK 1.3 serialVersionUID
			 */
			internal const long SerialVersionUID = 3631432373506317811L;

			/// <summary>
			/// Constructs an AccessibleAWTTextComponent.  Adds a listener to track
			/// caret change.
			/// </summary>
			public AccessibleAWTTextComponent(TextComponent outerInstance) : base(outerInstance)
			{
				this.OuterInstance = outerInstance;
				outerInstance.AddTextListener(this);
			}

			/// <summary>
			/// TextListener notification of a text value change.
			/// </summary>
			public virtual void TextValueChanged(TextEvent textEvent)
			{
				Integer cpos = Convert.ToInt32(OuterInstance.CaretPosition);
				outerInstance.FirePropertyChange(ACCESSIBLE_TEXT_PROPERTY, null, cpos);
			}

			/// <summary>
			/// Gets the state set of the TextComponent.
			/// The AccessibleStateSet of an object is composed of a set of
			/// unique AccessibleStates.  A change in the AccessibleStateSet
			/// of an object will cause a PropertyChangeEvent to be fired
			/// for the AccessibleContext.ACCESSIBLE_STATE_PROPERTY property.
			/// </summary>
			/// <returns> an instance of AccessibleStateSet containing the
			/// current state set of the object </returns>
			/// <seealso cref= AccessibleStateSet </seealso>
			/// <seealso cref= AccessibleState </seealso>
			/// <seealso cref= #addPropertyChangeListener </seealso>
			public override AccessibleStateSet AccessibleStateSet
			{
				get
				{
					AccessibleStateSet states = base.AccessibleStateSet;
					if (OuterInstance.Editable)
					{
						states.add(AccessibleState.EDITABLE);
					}
					return states;
				}
			}


			/// <summary>
			/// Gets the role of this object.
			/// </summary>
			/// <returns> an instance of AccessibleRole describing the role of the
			/// object (AccessibleRole.TEXT) </returns>
			/// <seealso cref= AccessibleRole </seealso>
			public override AccessibleRole AccessibleRole
			{
				get
				{
					return AccessibleRole.TEXT;
				}
			}

			/// <summary>
			/// Get the AccessibleText associated with this object.  In the
			/// implementation of the Java Accessibility API for this class,
			/// return this object, which is responsible for implementing the
			/// AccessibleText interface on behalf of itself.
			/// </summary>
			/// <returns> this object </returns>
			public virtual AccessibleText AccessibleText
			{
				get
				{
					return this;
				}
			}


			// --- interface AccessibleText methods ------------------------

			/// <summary>
			/// Many of these methods are just convenience methods; they
			/// just call the equivalent on the parent
			/// </summary>

			/// <summary>
			/// Given a point in local coordinates, return the zero-based index
			/// of the character under that Point.  If the point is invalid,
			/// this method returns -1.
			/// </summary>
			/// <param name="p"> the Point in local coordinates </param>
			/// <returns> the zero-based index of the character under Point p. </returns>
			public virtual int GetIndexAtPoint(Point p)
			{
				return -1;
			}

			/// <summary>
			/// Determines the bounding box of the character at the given
			/// index into the string.  The bounds are returned in local
			/// coordinates.  If the index is invalid a null rectangle
			/// is returned.
			/// </summary>
			/// <param name="i"> the index into the String &gt;= 0 </param>
			/// <returns> the screen coordinates of the character's bounding box </returns>
			public virtual Rectangle GetCharacterBounds(int i)
			{
				return null;
			}

			/// <summary>
			/// Returns the number of characters (valid indicies)
			/// </summary>
			/// <returns> the number of characters &gt;= 0 </returns>
			public virtual int CharCount
			{
				get
				{
					return OuterInstance.Text.Length();
				}
			}

			/// <summary>
			/// Returns the zero-based offset of the caret.
			/// 
			/// Note: The character to the right of the caret will have the
			/// same index value as the offset (the caret is between
			/// two characters).
			/// </summary>
			/// <returns> the zero-based offset of the caret. </returns>
			public virtual int CaretPosition
			{
				get
				{
					return OuterInstance.CaretPosition;
				}
			}

			/// <summary>
			/// Returns the AttributeSet for a given character (at a given index).
			/// </summary>
			/// <param name="i"> the zero-based index into the text </param>
			/// <returns> the AttributeSet of the character </returns>
			public virtual AttributeSet GetCharacterAttribute(int i)
			{
				return null; // No attributes in TextComponent
			}

			/// <summary>
			/// Returns the start offset within the selected text.
			/// If there is no selection, but there is
			/// a caret, the start and end offsets will be the same.
			/// Return 0 if the text is empty, or the caret position
			/// if no selection.
			/// </summary>
			/// <returns> the index into the text of the start of the selection &gt;= 0 </returns>
			public virtual int SelectionStart
			{
				get
				{
					return OuterInstance.SelectionStart;
				}
			}

			/// <summary>
			/// Returns the end offset within the selected text.
			/// If there is no selection, but there is
			/// a caret, the start and end offsets will be the same.
			/// Return 0 if the text is empty, or the caret position
			/// if no selection.
			/// </summary>
			/// <returns> the index into the text of the end of the selection &gt;= 0 </returns>
			public virtual int SelectionEnd
			{
				get
				{
					return OuterInstance.SelectionEnd;
				}
			}

			/// <summary>
			/// Returns the portion of the text that is selected.
			/// </summary>
			/// <returns> the text, null if no selection </returns>
			public virtual String SelectedText
			{
				get
				{
					String selText = OuterInstance.SelectedText;
					// Fix for 4256662
					if (selText == null || selText.Equals(""))
					{
						return null;
					}
					return selText;
				}
			}

			/// <summary>
			/// Returns the String at a given index.
			/// </summary>
			/// <param name="part"> the AccessibleText.CHARACTER, AccessibleText.WORD,
			/// or AccessibleText.SENTENCE to retrieve </param>
			/// <param name="index"> an index within the text &gt;= 0 </param>
			/// <returns> the letter, word, or sentence,
			///   null for an invalid index or part </returns>
			public virtual String GetAtIndex(int part, int index)
			{
				if (index < 0 || index >= OuterInstance.Text.Length())
				{
					return null;
				}
				switch (part)
				{
				case AccessibleText.CHARACTER:
					return OuterInstance.Text.Substring(index, 1);
				case AccessibleText.WORD:
				{
						String s = OuterInstance.Text;
						BreakIterator words = BreakIterator.WordInstance;
						words.SetText(s);
						int end = words.Following(index);
						return StringHelperClass.SubstringSpecial(s, words.Previous(), end);
				}
				case AccessibleText.SENTENCE:
				{
						String s = OuterInstance.Text;
						BreakIterator sentence = BreakIterator.SentenceInstance;
						sentence.SetText(s);
						int end = sentence.Following(index);
						return StringHelperClass.SubstringSpecial(s, sentence.Previous(), end);
				}
				default:
					return null;
				}
			}

			internal const bool NEXT = true;
			internal const bool PREVIOUS = false;

			/// <summary>
			/// Needed to unify forward and backward searching.
			/// The method assumes that s is the text assigned to words.
			/// </summary>
			internal virtual int FindWordLimit(int index, BreakIterator words, bool direction, String s)
			{
				// Fix for 4256660 and 4256661.
				// Words iterator is different from character and sentence iterators
				// in that end of one word is not necessarily start of another word.
				// Please see java.text.BreakIterator JavaDoc. The code below is
				// based on nextWordStartAfter example from BreakIterator.java.
				int last = (direction == NEXT) ? words.Following(index) : words.Preceding(index);
				int current = (direction == NEXT) ? words.Next() : words.Previous();
				while (current != BreakIterator.DONE)
				{
					for (int p = System.Math.Min(last, current); p < System.Math.Max(last, current); p++)
					{
						if (char.IsLetter(s.CharAt(p)))
						{
							return last;
						}
					}
					last = current;
					current = (direction == NEXT) ? words.Next() : words.Previous();
				}
				return BreakIterator.DONE;
			}

			/// <summary>
			/// Returns the String after a given index.
			/// </summary>
			/// <param name="part"> the AccessibleText.CHARACTER, AccessibleText.WORD,
			/// or AccessibleText.SENTENCE to retrieve </param>
			/// <param name="index"> an index within the text &gt;= 0 </param>
			/// <returns> the letter, word, or sentence, null for an invalid
			///  index or part </returns>
			public virtual String GetAfterIndex(int part, int index)
			{
				if (index < 0 || index >= OuterInstance.Text.Length())
				{
					return null;
				}
				switch (part)
				{
				case AccessibleText.CHARACTER:
					if (index + 1 >= OuterInstance.Text.Length())
					{
					   return null;
					}
					return StringHelperClass.SubstringSpecial(OuterInstance.Text, index + 1, index + 2);
				case AccessibleText.WORD:
				{
						String s = OuterInstance.Text;
						BreakIterator words = BreakIterator.WordInstance;
						words.SetText(s);
						int start = FindWordLimit(index, words, NEXT, s);
						if (start == BreakIterator.DONE || start >= s.Length())
						{
							return null;
						}
						int end = words.Following(start);
						if (end == BreakIterator.DONE || end >= s.Length())
						{
							return null;
						}
						return s.Substring(start, end - start);
				}
				case AccessibleText.SENTENCE:
				{
						String s = OuterInstance.Text;
						BreakIterator sentence = BreakIterator.SentenceInstance;
						sentence.SetText(s);
						int start = sentence.Following(index);
						if (start == BreakIterator.DONE || start >= s.Length())
						{
							return null;
						}
						int end = sentence.Following(start);
						if (end == BreakIterator.DONE || end >= s.Length())
						{
							return null;
						}
						return s.Substring(start, end - start);
				}
				default:
					return null;
				}
			}


			/// <summary>
			/// Returns the String before a given index.
			/// </summary>
			/// <param name="part"> the AccessibleText.CHARACTER, AccessibleText.WORD,
			///   or AccessibleText.SENTENCE to retrieve </param>
			/// <param name="index"> an index within the text &gt;= 0 </param>
			/// <returns> the letter, word, or sentence, null for an invalid index
			///  or part </returns>
			public virtual String GetBeforeIndex(int part, int index)
			{
				if (index < 0 || index > OuterInstance.Text.Length() - 1)
				{
					return null;
				}
				switch (part)
				{
				case AccessibleText.CHARACTER:
					if (index == 0)
					{
						return null;
					}
					return StringHelperClass.SubstringSpecial(OuterInstance.Text, index - 1, index);
				case AccessibleText.WORD:
				{
						String s = OuterInstance.Text;
						BreakIterator words = BreakIterator.WordInstance;
						words.SetText(s);
						int end = FindWordLimit(index, words, PREVIOUS, s);
						if (end == BreakIterator.DONE)
						{
							return null;
						}
						int start = words.Preceding(end);
						if (start == BreakIterator.DONE)
						{
							return null;
						}
						return s.Substring(start, end - start);
				}
				case AccessibleText.SENTENCE:
				{
						String s = OuterInstance.Text;
						BreakIterator sentence = BreakIterator.SentenceInstance;
						sentence.SetText(s);
						int end = sentence.Following(index);
						end = sentence.Previous();
						int start = sentence.Previous();
						if (start == BreakIterator.DONE)
						{
							return null;
						}
						return s.Substring(start, end - start);
				}
				default:
					return null;
				}
			}
		} // end of AccessibleAWTTextComponent

		private bool CheckForEnableIM = true;
	}

}
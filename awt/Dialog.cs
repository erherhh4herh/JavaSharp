using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;

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

	using AppContext = sun.awt.AppContext;
	using SunToolkit = sun.awt.SunToolkit;
	using PeerEvent = sun.awt.PeerEvent;
	using IdentityArrayList = sun.awt.util.IdentityArrayList;
	using IdentityLinkedList = sun.awt.util.IdentityLinkedList;
	using SecurityConstants = sun.security.util.SecurityConstants;

	/// <summary>
	/// A Dialog is a top-level window with a title and a border
	/// that is typically used to take some form of input from the user.
	/// 
	/// The size of the dialog includes any area designated for the
	/// border.  The dimensions of the border area can be obtained
	/// using the <code>getInsets</code> method, however, since
	/// these dimensions are platform-dependent, a valid insets
	/// value cannot be obtained until the dialog is made displayable
	/// by either calling <code>pack</code> or <code>show</code>.
	/// Since the border area is included in the overall size of the
	/// dialog, the border effectively obscures a portion of the dialog,
	/// constraining the area available for rendering and/or displaying
	/// subcomponents to the rectangle which has an upper-left corner
	/// location of <code>(insets.left, insets.top)</code>, and has a size of
	/// <code>width - (insets.left + insets.right)</code> by
	/// <code>height - (insets.top + insets.bottom)</code>.
	/// <para>
	/// The default layout for a dialog is <code>BorderLayout</code>.
	/// </para>
	/// <para>
	/// A dialog may have its native decorations (i.e. Frame &amp; Titlebar) turned off
	/// with <code>setUndecorated</code>.  This can only be done while the dialog
	/// is not <seealso cref="Component#isDisplayable() displayable"/>.
	/// </para>
	/// <para>
	/// A dialog may have another window as its owner when it's constructed.  When
	/// the owner window of a visible dialog is minimized, the dialog will
	/// automatically be hidden from the user. When the owner window is subsequently
	/// restored, the dialog is made visible to the user again.
	/// </para>
	/// <para>
	/// In a multi-screen environment, you can create a <code>Dialog</code>
	/// on a different screen device than its owner.  See <seealso cref="java.awt.Frame"/> for
	/// more information.
	/// </para>
	/// <para>
	/// A dialog can be either modeless (the default) or modal.  A modal
	/// dialog is one which blocks input to some other top-level windows
	/// in the application, except for any windows created with the dialog
	/// as their owner. See <a href="doc-files/Modality.html">AWT Modality</a>
	/// specification for details.
	/// </para>
	/// <para>
	/// Dialogs are capable of generating the following
	/// <code>WindowEvents</code>:
	/// <code>WindowOpened</code>, <code>WindowClosing</code>,
	/// <code>WindowClosed</code>, <code>WindowActivated</code>,
	/// <code>WindowDeactivated</code>, <code>WindowGainedFocus</code>,
	/// <code>WindowLostFocus</code>.
	/// 
	/// </para>
	/// </summary>
	/// <seealso cref= WindowEvent </seealso>
	/// <seealso cref= Window#addWindowListener
	/// 
	/// @author      Sami Shaio
	/// @author      Arthur van Hoff
	/// @since       JDK1.0 </seealso>
	public class Dialog : Window
	{

		static Dialog()
		{
			/* ensure that the necessary native libraries are loaded */
			Toolkit.LoadLibraries();
			if (!GraphicsEnvironment.Headless)
			{
				initIDs();
			}
		}

		/// <summary>
		/// A dialog's resizable property. Will be true
		/// if the Dialog is to be resizable, otherwise
		/// it will be false.
		/// 
		/// @serial </summary>
		/// <seealso cref= #setResizable(boolean) </seealso>
		internal bool Resizable_Renamed = true;


		/// <summary>
		/// This field indicates whether the dialog is undecorated.
		/// This property can only be changed while the dialog is not displayable.
		/// <code>undecorated</code> will be true if the dialog is
		/// undecorated, otherwise it will be false.
		/// 
		/// @serial </summary>
		/// <seealso cref= #setUndecorated(boolean) </seealso>
		/// <seealso cref= #isUndecorated() </seealso>
		/// <seealso cref= Component#isDisplayable()
		/// @since 1.4 </seealso>
		internal bool Undecorated_Renamed = false;

		[NonSerialized]
		private bool Initialized = false;

		/// <summary>
		/// Modal dialogs block all input to some top-level windows.
		/// Whether a particular window is blocked depends on dialog's type
		/// of modality; this is called the "scope of blocking". The
		/// <code>ModalityType</code> enum specifies modal types and their
		/// associated scopes.
		/// </summary>
		/// <seealso cref= Dialog#getModalityType </seealso>
		/// <seealso cref= Dialog#setModalityType </seealso>
		/// <seealso cref= Toolkit#isModalityTypeSupported
		/// 
		/// @since 1.6 </seealso>
		public enum ModalityType
		{
			/// <summary>
			/// <code>MODELESS</code> dialog doesn't block any top-level windows.
			/// </summary>
			MODELESS,
			/// <summary>
			/// A <code>DOCUMENT_MODAL</code> dialog blocks input to all top-level windows
			/// from the same document except those from its own child hierarchy.
			/// A document is a top-level window without an owner. It may contain child
			/// windows that, together with the top-level window are treated as a single
			/// solid document. Since every top-level window must belong to some
			/// document, its root can be found as the top-nearest window without an owner.
			/// </summary>
			DOCUMENT_MODAL,
			/// <summary>
			/// An <code>APPLICATION_MODAL</code> dialog blocks all top-level windows
			/// from the same Java application except those from its own child hierarchy.
			/// If there are several applets launched in a browser, they can be
			/// treated either as separate applications or a single one. This behavior
			/// is implementation-dependent.
			/// </summary>
			APPLICATION_MODAL,
			/// <summary>
			/// A <code>TOOLKIT_MODAL</code> dialog blocks all top-level windows run
			/// from the same toolkit except those from its own child hierarchy. If there
			/// are several applets launched in a browser, all of them run with the same
			/// toolkit; thus, a toolkit-modal dialog displayed by an applet may affect
			/// other applets and all windows of the browser instance which embeds the
			/// Java runtime environment for this toolkit.
			/// Special <code>AWTPermission</code> "toolkitModality" must be granted to use
			/// toolkit-modal dialogs. If a <code>TOOLKIT_MODAL</code> dialog is being created
			/// and this permission is not granted, a <code>SecurityException</code> will be
			/// thrown, and no dialog will be created. If a modality type is being changed
			/// to <code>TOOLKIT_MODAL</code> and this permission is not granted, a
			/// <code>SecurityException</code> will be thrown, and the modality type will
			/// be left unchanged.
			/// </summary>
			TOOLKIT_MODAL
		}

		/// <summary>
		/// Default modality type for modal dialogs. The default modality type is
		/// <code>APPLICATION_MODAL</code>. Calling the oldstyle <code>setModal(true)</code>
		/// is equal to <code>setModalityType(DEFAULT_MODALITY_TYPE)</code>.
		/// </summary>
		/// <seealso cref= java.awt.Dialog.ModalityType </seealso>
		/// <seealso cref= java.awt.Dialog#setModal
		/// 
		/// @since 1.6 </seealso>
		public const ModalityType DEFAULT_MODALITY_TYPE = ModalityType.APPLICATION_MODAL;

		/// <summary>
		/// True if this dialog is modal, false is the dialog is modeless.
		/// A modal dialog blocks user input to some application top-level
		/// windows. This field is kept only for backwards compatibility. Use the
		/// <seealso cref="Dialog.ModalityType ModalityType"/> enum instead.
		/// 
		/// @serial
		/// </summary>
		/// <seealso cref= #isModal </seealso>
		/// <seealso cref= #setModal </seealso>
		/// <seealso cref= #getModalityType </seealso>
		/// <seealso cref= #setModalityType </seealso>
		/// <seealso cref= ModalityType </seealso>
		/// <seealso cref= ModalityType#MODELESS </seealso>
		/// <seealso cref= #DEFAULT_MODALITY_TYPE </seealso>
		internal bool Modal_Renamed;

		/// <summary>
		/// Modality type of this dialog. If the dialog's modality type is not
		/// <seealso cref="Dialog.ModalityType#MODELESS ModalityType.MODELESS"/>, it blocks all
		/// user input to some application top-level windows.
		/// 
		/// @serial
		/// </summary>
		/// <seealso cref= ModalityType </seealso>
		/// <seealso cref= #getModalityType </seealso>
		/// <seealso cref= #setModalityType
		/// 
		/// @since 1.6 </seealso>
		internal ModalityType ModalityType_Renamed;

		/// <summary>
		/// Any top-level window can be marked not to be blocked by modal
		/// dialogs. This is called "modal exclusion". This enum specifies
		/// the possible modal exclusion types.
		/// </summary>
		/// <seealso cref= Window#getModalExclusionType </seealso>
		/// <seealso cref= Window#setModalExclusionType </seealso>
		/// <seealso cref= Toolkit#isModalExclusionTypeSupported
		/// 
		/// @since 1.6 </seealso>
		public enum ModalExclusionType
		{
			/// <summary>
			/// No modal exclusion.
			/// </summary>
			NO_EXCLUDE,
			/// <summary>
			/// <code>APPLICATION_EXCLUDE</code> indicates that a top-level window
			/// won't be blocked by any application-modal dialogs. Also, it isn't
			/// blocked by document-modal dialogs from outside of its child hierarchy.
			/// </summary>
			APPLICATION_EXCLUDE,
			/// <summary>
			/// <code>TOOLKIT_EXCLUDE</code> indicates that a top-level window
			/// won't be blocked by  application-modal or toolkit-modal dialogs. Also,
			/// it isn't blocked by document-modal dialogs from outside of its
			/// child hierarchy.
			/// The "toolkitModality" <code>AWTPermission</code> must be granted
			/// for this exclusion. If an exclusion property is being changed to
			/// <code>TOOLKIT_EXCLUDE</code> and this permission is not granted, a
			/// <code>SecurityEcxeption</code> will be thrown, and the exclusion
			/// property will be left unchanged.
			/// </summary>
			TOOLKIT_EXCLUDE
		}

		/* operations with this list should be synchronized on tree lock*/
		[NonSerialized]
		internal static IdentityArrayList<Dialog> ModalDialogs = new IdentityArrayList<Dialog>();

		[NonSerialized]
		internal IdentityArrayList<Window> BlockedWindows = new IdentityArrayList<Window>();

		/// <summary>
		/// Specifies the title of the Dialog.
		/// This field can be null.
		/// 
		/// @serial </summary>
		/// <seealso cref= #getTitle() </seealso>
		/// <seealso cref= #setTitle(String) </seealso>
		internal String Title_Renamed;

		[NonSerialized]
		private ModalEventFilter ModalFilter;
		[NonSerialized]
		private volatile SecondaryLoop SecondaryLoop;

		/*
		 * Indicates that this dialog is being hidden. This flag is set to true at
		 * the beginning of hide() and to false at the end of hide().
		 *
		 * @see #hide()
		 * @see #hideAndDisposePreHandler()
		 * @see #hideAndDisposeHandler()
		 * @see #shouldBlock()
		 */
		[NonSerialized]
		internal volatile bool IsInHide = false;

		/*
		 * Indicates that this dialog is being disposed. This flag is set to true at
		 * the beginning of doDispose() and to false at the end of doDispose().
		 *
		 * @see #hide()
		 * @see #hideAndDisposePreHandler()
		 * @see #hideAndDisposeHandler()
		 * @see #doDispose()
		 */
		[NonSerialized]
		internal volatile bool IsInDispose = false;

		private const String @base = "dialog";
		private static int NameCounter = 0;

		/*
		 * JDK 1.1 serialVersionUID
		 */
		private const long SerialVersionUID = 5920926903803293709L;

		/// <summary>
		/// Constructs an initially invisible, modeless <code>Dialog</code> with
		/// the specified owner <code>Frame</code> and an empty title.
		/// </summary>
		/// <param name="owner"> the owner of the dialog or <code>null</code> if
		///     this dialog has no owner </param>
		/// <exception cref="java.lang.IllegalArgumentException"> if the <code>owner</code>'s
		///    <code>GraphicsConfiguration</code> is not from a screen device </exception>
		/// <exception cref="HeadlessException"> when
		///    <code>GraphicsEnvironment.isHeadless()</code> returns <code>true</code>
		/// </exception>
		/// <seealso cref= java.awt.GraphicsEnvironment#isHeadless </seealso>
		/// <seealso cref= Component#setSize </seealso>
		/// <seealso cref= Component#setVisible </seealso>
		 public Dialog(Frame owner) : this(owner, "", false)
		 {
		 }

		/// <summary>
		/// Constructs an initially invisible <code>Dialog</code> with the specified
		/// owner <code>Frame</code> and modality and an empty title.
		/// </summary>
		/// <param name="owner"> the owner of the dialog or <code>null</code> if
		///     this dialog has no owner </param>
		/// <param name="modal"> specifies whether dialog blocks user input to other top-level
		///     windows when shown. If <code>false</code>, the dialog is <code>MODELESS</code>;
		///     if <code>true</code>, the modality type property is set to
		///     <code>DEFAULT_MODALITY_TYPE</code> </param>
		/// <exception cref="java.lang.IllegalArgumentException"> if the <code>owner</code>'s
		///    <code>GraphicsConfiguration</code> is not from a screen device </exception>
		/// <exception cref="HeadlessException"> when
		///     <code>GraphicsEnvironment.isHeadless()</code> returns <code>true</code>
		/// </exception>
		/// <seealso cref= java.awt.Dialog.ModalityType </seealso>
		/// <seealso cref= java.awt.Dialog.ModalityType#MODELESS </seealso>
		/// <seealso cref= java.awt.Dialog#DEFAULT_MODALITY_TYPE </seealso>
		/// <seealso cref= java.awt.Dialog#setModal </seealso>
		/// <seealso cref= java.awt.Dialog#setModalityType </seealso>
		/// <seealso cref= java.awt.GraphicsEnvironment#isHeadless </seealso>
		 public Dialog(Frame owner, bool modal) : this(owner, "", modal)
		 {
		 }

		/// <summary>
		/// Constructs an initially invisible, modeless <code>Dialog</code> with
		/// the specified owner <code>Frame</code> and title.
		/// </summary>
		/// <param name="owner"> the owner of the dialog or <code>null</code> if
		///     this dialog has no owner </param>
		/// <param name="title"> the title of the dialog or <code>null</code> if this dialog
		///     has no title </param>
		/// <exception cref="IllegalArgumentException"> if the <code>owner</code>'s
		///     <code>GraphicsConfiguration</code> is not from a screen device </exception>
		/// <exception cref="HeadlessException"> when
		///     <code>GraphicsEnvironment.isHeadless()</code> returns <code>true</code>
		/// </exception>
		/// <seealso cref= java.awt.GraphicsEnvironment#isHeadless </seealso>
		/// <seealso cref= Component#setSize </seealso>
		/// <seealso cref= Component#setVisible </seealso>
		 public Dialog(Frame owner, String title) : this(owner, title, false)
		 {
		 }

		/// <summary>
		/// Constructs an initially invisible <code>Dialog</code> with the
		/// specified owner <code>Frame</code>, title and modality.
		/// </summary>
		/// <param name="owner"> the owner of the dialog or <code>null</code> if
		///     this dialog has no owner </param>
		/// <param name="title"> the title of the dialog or <code>null</code> if this dialog
		///     has no title </param>
		/// <param name="modal"> specifies whether dialog blocks user input to other top-level
		///     windows when shown. If <code>false</code>, the dialog is <code>MODELESS</code>;
		///     if <code>true</code>, the modality type property is set to
		///     <code>DEFAULT_MODALITY_TYPE</code> </param>
		/// <exception cref="java.lang.IllegalArgumentException"> if the <code>owner</code>'s
		///    <code>GraphicsConfiguration</code> is not from a screen device </exception>
		/// <exception cref="HeadlessException"> when
		///    <code>GraphicsEnvironment.isHeadless()</code> returns <code>true</code>
		/// </exception>
		/// <seealso cref= java.awt.Dialog.ModalityType </seealso>
		/// <seealso cref= java.awt.Dialog.ModalityType#MODELESS </seealso>
		/// <seealso cref= java.awt.Dialog#DEFAULT_MODALITY_TYPE </seealso>
		/// <seealso cref= java.awt.Dialog#setModal </seealso>
		/// <seealso cref= java.awt.Dialog#setModalityType </seealso>
		/// <seealso cref= java.awt.GraphicsEnvironment#isHeadless </seealso>
		/// <seealso cref= Component#setSize </seealso>
		/// <seealso cref= Component#setVisible </seealso>
		 public Dialog(Frame owner, String title, bool modal) : this(owner, title, modal ? DEFAULT_MODALITY_TYPE : ModalityType.MODELESS)
		 {
		 }

		/// <summary>
		/// Constructs an initially invisible <code>Dialog</code> with the specified owner
		/// <code>Frame</code>, title, modality, and <code>GraphicsConfiguration</code>. </summary>
		/// <param name="owner"> the owner of the dialog or <code>null</code> if this dialog
		///     has no owner </param>
		/// <param name="title"> the title of the dialog or <code>null</code> if this dialog
		///     has no title </param>
		/// <param name="modal"> specifies whether dialog blocks user input to other top-level
		///     windows when shown. If <code>false</code>, the dialog is <code>MODELESS</code>;
		///     if <code>true</code>, the modality type property is set to
		///     <code>DEFAULT_MODALITY_TYPE</code> </param>
		/// <param name="gc"> the <code>GraphicsConfiguration</code> of the target screen device;
		///     if <code>null</code>, the default system <code>GraphicsConfiguration</code>
		///     is assumed </param>
		/// <exception cref="java.lang.IllegalArgumentException"> if <code>gc</code>
		///     is not from a screen device </exception>
		/// <exception cref="HeadlessException"> when
		///     <code>GraphicsEnvironment.isHeadless()</code> returns <code>true</code>
		/// </exception>
		/// <seealso cref= java.awt.Dialog.ModalityType </seealso>
		/// <seealso cref= java.awt.Dialog.ModalityType#MODELESS </seealso>
		/// <seealso cref= java.awt.Dialog#DEFAULT_MODALITY_TYPE </seealso>
		/// <seealso cref= java.awt.Dialog#setModal </seealso>
		/// <seealso cref= java.awt.Dialog#setModalityType </seealso>
		/// <seealso cref= java.awt.GraphicsEnvironment#isHeadless </seealso>
		/// <seealso cref= Component#setSize </seealso>
		/// <seealso cref= Component#setVisible
		/// @since 1.4 </seealso>
		 public Dialog(Frame owner, String title, bool modal, GraphicsConfiguration gc) : this(owner, title, modal ? DEFAULT_MODALITY_TYPE : ModalityType.MODELESS, gc)
		 {
		 }

		/// <summary>
		/// Constructs an initially invisible, modeless <code>Dialog</code> with
		/// the specified owner <code>Dialog</code> and an empty title.
		/// </summary>
		/// <param name="owner"> the owner of the dialog or <code>null</code> if this
		///     dialog has no owner </param>
		/// <exception cref="java.lang.IllegalArgumentException"> if the <code>owner</code>'s
		///     <code>GraphicsConfiguration</code> is not from a screen device </exception>
		/// <exception cref="HeadlessException"> when
		///     <code>GraphicsEnvironment.isHeadless()</code> returns <code>true</code> </exception>
		/// <seealso cref= java.awt.GraphicsEnvironment#isHeadless
		/// @since 1.2 </seealso>
		 public Dialog(Dialog owner) : this(owner, "", false)
		 {
		 }

		/// <summary>
		/// Constructs an initially invisible, modeless <code>Dialog</code>
		/// with the specified owner <code>Dialog</code> and title.
		/// </summary>
		/// <param name="owner"> the owner of the dialog or <code>null</code> if this
		///     has no owner </param>
		/// <param name="title"> the title of the dialog or <code>null</code> if this dialog
		///     has no title </param>
		/// <exception cref="java.lang.IllegalArgumentException"> if the <code>owner</code>'s
		///     <code>GraphicsConfiguration</code> is not from a screen device </exception>
		/// <exception cref="HeadlessException"> when
		///     <code>GraphicsEnvironment.isHeadless()</code> returns <code>true</code>
		/// </exception>
		/// <seealso cref= java.awt.GraphicsEnvironment#isHeadless
		/// @since 1.2 </seealso>
		 public Dialog(Dialog owner, String title) : this(owner, title, false)
		 {
		 }

		/// <summary>
		/// Constructs an initially invisible <code>Dialog</code> with the
		/// specified owner <code>Dialog</code>, title, and modality.
		/// </summary>
		/// <param name="owner"> the owner of the dialog or <code>null</code> if this
		///     dialog has no owner </param>
		/// <param name="title"> the title of the dialog or <code>null</code> if this
		///     dialog has no title </param>
		/// <param name="modal"> specifies whether dialog blocks user input to other top-level
		///     windows when shown. If <code>false</code>, the dialog is <code>MODELESS</code>;
		///     if <code>true</code>, the modality type property is set to
		///     <code>DEFAULT_MODALITY_TYPE</code> </param>
		/// <exception cref="IllegalArgumentException"> if the <code>owner</code>'s
		///    <code>GraphicsConfiguration</code> is not from a screen device </exception>
		/// <exception cref="HeadlessException"> when
		///    <code>GraphicsEnvironment.isHeadless()</code> returns <code>true</code>
		/// </exception>
		/// <seealso cref= java.awt.Dialog.ModalityType </seealso>
		/// <seealso cref= java.awt.Dialog.ModalityType#MODELESS </seealso>
		/// <seealso cref= java.awt.Dialog#DEFAULT_MODALITY_TYPE </seealso>
		/// <seealso cref= java.awt.Dialog#setModal </seealso>
		/// <seealso cref= java.awt.Dialog#setModalityType </seealso>
		/// <seealso cref= java.awt.GraphicsEnvironment#isHeadless
		/// 
		/// @since 1.2 </seealso>
		 public Dialog(Dialog owner, String title, bool modal) : this(owner, title, modal ? DEFAULT_MODALITY_TYPE : ModalityType.MODELESS)
		 {
		 }

		/// <summary>
		/// Constructs an initially invisible <code>Dialog</code> with the
		/// specified owner <code>Dialog</code>, title, modality and
		/// <code>GraphicsConfiguration</code>.
		/// </summary>
		/// <param name="owner"> the owner of the dialog or <code>null</code> if this
		///     dialog has no owner </param>
		/// <param name="title"> the title of the dialog or <code>null</code> if this
		///     dialog has no title </param>
		/// <param name="modal"> specifies whether dialog blocks user input to other top-level
		///     windows when shown. If <code>false</code>, the dialog is <code>MODELESS</code>;
		///     if <code>true</code>, the modality type property is set to
		///     <code>DEFAULT_MODALITY_TYPE</code> </param>
		/// <param name="gc"> the <code>GraphicsConfiguration</code> of the target screen device;
		///     if <code>null</code>, the default system <code>GraphicsConfiguration</code>
		///     is assumed </param>
		/// <exception cref="java.lang.IllegalArgumentException"> if <code>gc</code>
		///    is not from a screen device </exception>
		/// <exception cref="HeadlessException"> when
		///    <code>GraphicsEnvironment.isHeadless()</code> returns <code>true</code>
		/// </exception>
		/// <seealso cref= java.awt.Dialog.ModalityType </seealso>
		/// <seealso cref= java.awt.Dialog.ModalityType#MODELESS </seealso>
		/// <seealso cref= java.awt.Dialog#DEFAULT_MODALITY_TYPE </seealso>
		/// <seealso cref= java.awt.Dialog#setModal </seealso>
		/// <seealso cref= java.awt.Dialog#setModalityType </seealso>
		/// <seealso cref= java.awt.GraphicsEnvironment#isHeadless </seealso>
		/// <seealso cref= Component#setSize </seealso>
		/// <seealso cref= Component#setVisible
		/// 
		/// @since 1.4 </seealso>
		 public Dialog(Dialog owner, String title, bool modal, GraphicsConfiguration gc) : this(owner, title, modal ? DEFAULT_MODALITY_TYPE : ModalityType.MODELESS, gc)
		 {
		 }

		/// <summary>
		/// Constructs an initially invisible, modeless <code>Dialog</code> with the
		/// specified owner <code>Window</code> and an empty title.
		/// </summary>
		/// <param name="owner"> the owner of the dialog. The owner must be an instance of
		///     <seealso cref="java.awt.Dialog Dialog"/>, <seealso cref="java.awt.Frame Frame"/>, any
		///     of their descendents or <code>null</code>
		/// </param>
		/// <exception cref="java.lang.IllegalArgumentException"> if the <code>owner</code>
		///     is not an instance of <seealso cref="java.awt.Dialog Dialog"/> or {@link
		///     java.awt.Frame Frame} </exception>
		/// <exception cref="java.lang.IllegalArgumentException"> if the <code>owner</code>'s
		///     <code>GraphicsConfiguration</code> is not from a screen device </exception>
		/// <exception cref="HeadlessException"> when
		///     <code>GraphicsEnvironment.isHeadless()</code> returns <code>true</code>
		/// </exception>
		/// <seealso cref= java.awt.GraphicsEnvironment#isHeadless
		/// 
		/// @since 1.6 </seealso>
		public Dialog(Window owner) : this(owner, "", ModalityType.MODELESS)
		{
		}

		/// <summary>
		/// Constructs an initially invisible, modeless <code>Dialog</code> with
		/// the specified owner <code>Window</code> and title.
		/// </summary>
		/// <param name="owner"> the owner of the dialog. The owner must be an instance of
		///    <seealso cref="java.awt.Dialog Dialog"/>, <seealso cref="java.awt.Frame Frame"/>, any
		///    of their descendents or <code>null</code> </param>
		/// <param name="title"> the title of the dialog or <code>null</code> if this dialog
		///    has no title
		/// </param>
		/// <exception cref="java.lang.IllegalArgumentException"> if the <code>owner</code>
		///    is not an instance of <seealso cref="java.awt.Dialog Dialog"/> or {@link
		///    java.awt.Frame Frame} </exception>
		/// <exception cref="java.lang.IllegalArgumentException"> if the <code>owner</code>'s
		///    <code>GraphicsConfiguration</code> is not from a screen device </exception>
		/// <exception cref="HeadlessException"> when
		///    <code>GraphicsEnvironment.isHeadless()</code> returns <code>true</code>
		/// </exception>
		/// <seealso cref= java.awt.GraphicsEnvironment#isHeadless
		/// 
		/// @since 1.6 </seealso>
		public Dialog(Window owner, String title) : this(owner, title, ModalityType.MODELESS)
		{
		}

		/// <summary>
		/// Constructs an initially invisible <code>Dialog</code> with the
		/// specified owner <code>Window</code> and modality and an empty title.
		/// </summary>
		/// <param name="owner"> the owner of the dialog. The owner must be an instance of
		///    <seealso cref="java.awt.Dialog Dialog"/>, <seealso cref="java.awt.Frame Frame"/>, any
		///    of their descendents or <code>null</code> </param>
		/// <param name="modalityType"> specifies whether dialog blocks input to other
		///    windows when shown. <code>null</code> value and unsupported modality
		///    types are equivalent to <code>MODELESS</code>
		/// </param>
		/// <exception cref="java.lang.IllegalArgumentException"> if the <code>owner</code>
		///    is not an instance of <seealso cref="java.awt.Dialog Dialog"/> or {@link
		///    java.awt.Frame Frame} </exception>
		/// <exception cref="java.lang.IllegalArgumentException"> if the <code>owner</code>'s
		///    <code>GraphicsConfiguration</code> is not from a screen device </exception>
		/// <exception cref="HeadlessException"> when
		///    <code>GraphicsEnvironment.isHeadless()</code> returns <code>true</code> </exception>
		/// <exception cref="SecurityException"> if the calling thread does not have permission
		///    to create modal dialogs with the given <code>modalityType</code>
		/// </exception>
		/// <seealso cref= java.awt.Dialog.ModalityType </seealso>
		/// <seealso cref= java.awt.Dialog#setModal </seealso>
		/// <seealso cref= java.awt.Dialog#setModalityType </seealso>
		/// <seealso cref= java.awt.GraphicsEnvironment#isHeadless </seealso>
		/// <seealso cref= java.awt.Toolkit#isModalityTypeSupported
		/// 
		/// @since 1.6 </seealso>
		public Dialog(Window owner, ModalityType modalityType) : this(owner, "", modalityType)
		{
		}

		/// <summary>
		/// Constructs an initially invisible <code>Dialog</code> with the
		/// specified owner <code>Window</code>, title and modality.
		/// </summary>
		/// <param name="owner"> the owner of the dialog. The owner must be an instance of
		///     <seealso cref="java.awt.Dialog Dialog"/>, <seealso cref="java.awt.Frame Frame"/>, any
		///     of their descendents or <code>null</code> </param>
		/// <param name="title"> the title of the dialog or <code>null</code> if this dialog
		///     has no title </param>
		/// <param name="modalityType"> specifies whether dialog blocks input to other
		///    windows when shown. <code>null</code> value and unsupported modality
		///    types are equivalent to <code>MODELESS</code>
		/// </param>
		/// <exception cref="java.lang.IllegalArgumentException"> if the <code>owner</code>
		///     is not an instance of <seealso cref="java.awt.Dialog Dialog"/> or {@link
		///     java.awt.Frame Frame} </exception>
		/// <exception cref="java.lang.IllegalArgumentException"> if the <code>owner</code>'s
		///     <code>GraphicsConfiguration</code> is not from a screen device </exception>
		/// <exception cref="HeadlessException"> when
		///     <code>GraphicsEnvironment.isHeadless()</code> returns <code>true</code> </exception>
		/// <exception cref="SecurityException"> if the calling thread does not have permission
		///     to create modal dialogs with the given <code>modalityType</code>
		/// </exception>
		/// <seealso cref= java.awt.Dialog.ModalityType </seealso>
		/// <seealso cref= java.awt.Dialog#setModal </seealso>
		/// <seealso cref= java.awt.Dialog#setModalityType </seealso>
		/// <seealso cref= java.awt.GraphicsEnvironment#isHeadless </seealso>
		/// <seealso cref= java.awt.Toolkit#isModalityTypeSupported
		/// 
		/// @since 1.6 </seealso>
		public Dialog(Window owner, String title, ModalityType modalityType) : base(owner)
		{

			if ((owner != null) && !(owner is Frame) && !(owner is Dialog))
			{
				throw new IllegalArgumentException("Wrong parent window");
			}

			this.Title_Renamed = title;
			ModalityType = modalityType;
			SunToolkit.checkAndSetPolicy(this);
			Initialized = true;
		}

		/// <summary>
		/// Constructs an initially invisible <code>Dialog</code> with the
		/// specified owner <code>Window</code>, title, modality and
		/// <code>GraphicsConfiguration</code>.
		/// </summary>
		/// <param name="owner"> the owner of the dialog. The owner must be an instance of
		///     <seealso cref="java.awt.Dialog Dialog"/>, <seealso cref="java.awt.Frame Frame"/>, any
		///     of their descendents or <code>null</code> </param>
		/// <param name="title"> the title of the dialog or <code>null</code> if this dialog
		///     has no title </param>
		/// <param name="modalityType"> specifies whether dialog blocks input to other
		///    windows when shown. <code>null</code> value and unsupported modality
		///    types are equivalent to <code>MODELESS</code> </param>
		/// <param name="gc"> the <code>GraphicsConfiguration</code> of the target screen device;
		///     if <code>null</code>, the default system <code>GraphicsConfiguration</code>
		///     is assumed
		/// </param>
		/// <exception cref="java.lang.IllegalArgumentException"> if the <code>owner</code>
		///     is not an instance of <seealso cref="java.awt.Dialog Dialog"/> or {@link
		///     java.awt.Frame Frame} </exception>
		/// <exception cref="java.lang.IllegalArgumentException"> if <code>gc</code>
		///     is not from a screen device </exception>
		/// <exception cref="HeadlessException"> when
		///     <code>GraphicsEnvironment.isHeadless()</code> returns <code>true</code> </exception>
		/// <exception cref="SecurityException"> if the calling thread does not have permission
		///     to create modal dialogs with the given <code>modalityType</code>
		/// </exception>
		/// <seealso cref= java.awt.Dialog.ModalityType </seealso>
		/// <seealso cref= java.awt.Dialog#setModal </seealso>
		/// <seealso cref= java.awt.Dialog#setModalityType </seealso>
		/// <seealso cref= java.awt.GraphicsEnvironment#isHeadless </seealso>
		/// <seealso cref= java.awt.Toolkit#isModalityTypeSupported
		/// 
		/// @since 1.6 </seealso>
		public Dialog(Window owner, String title, ModalityType modalityType, GraphicsConfiguration gc) : base(owner, gc)
		{

			if ((owner != null) && !(owner is Frame) && !(owner is Dialog))
			{
				throw new IllegalArgumentException("wrong owner window");
			}

			this.Title_Renamed = title;
			ModalityType = modalityType;
			SunToolkit.checkAndSetPolicy(this);
			Initialized = true;
		}

		/// <summary>
		/// Construct a name for this component.  Called by getName() when the
		/// name is null.
		/// </summary>
		internal override String ConstructComponentName()
		{
			lock (typeof(Dialog))
			{
				return @base + NameCounter++;
			}
		}

		/// <summary>
		/// Makes this Dialog displayable by connecting it to
		/// a native screen resource.  Making a dialog displayable will
		/// cause any of its children to be made displayable.
		/// This method is called internally by the toolkit and should
		/// not be called directly by programs. </summary>
		/// <seealso cref= Component#isDisplayable </seealso>
		/// <seealso cref= #removeNotify </seealso>
		public override void AddNotify()
		{
			lock (TreeLock)
			{
				if (Parent_Renamed != null && Parent_Renamed.Peer == null)
				{
					Parent_Renamed.AddNotify();
				}

				if (Peer_Renamed == null)
				{
					Peer_Renamed = Toolkit.CreateDialog(this);
				}
				base.AddNotify();
			}
		}

		/// <summary>
		/// Indicates whether the dialog is modal.
		/// <para>
		/// This method is obsolete and is kept for backwards compatibility only.
		/// Use <seealso cref="#getModalityType getModalityType()"/> instead.
		/// 
		/// </para>
		/// </summary>
		/// <returns>    <code>true</code> if this dialog window is modal;
		///            <code>false</code> otherwise
		/// </returns>
		/// <seealso cref=       java.awt.Dialog#DEFAULT_MODALITY_TYPE </seealso>
		/// <seealso cref=       java.awt.Dialog.ModalityType#MODELESS </seealso>
		/// <seealso cref=       java.awt.Dialog#setModal </seealso>
		/// <seealso cref=       java.awt.Dialog#getModalityType </seealso>
		/// <seealso cref=       java.awt.Dialog#setModalityType </seealso>
		public virtual bool Modal
		{
			get
			{
				return Modal_NoClientCode;
			}
			set
			{
				this.Modal_Renamed = value;
				ModalityType = value ? DEFAULT_MODALITY_TYPE : ModalityType.MODELESS;
			}
		}
		internal bool Modal_NoClientCode
		{
			get
			{
				return ModalityType_Renamed != ModalityType.MODELESS;
			}
		}


		/// <summary>
		/// Returns the modality type of this dialog.
		/// </summary>
		/// <returns> modality type of this dialog
		/// </returns>
		/// <seealso cref= java.awt.Dialog#setModalityType
		/// 
		/// @since 1.6 </seealso>
		public virtual ModalityType ModalityType
		{
			get
			{
				return ModalityType_Renamed;
			}
			set
			{
				if (value == null)
				{
					value = Dialog.ModalityType.MODELESS;
				}
				if (!Toolkit.DefaultToolkit.IsModalityTypeSupported(value))
				{
					value = Dialog.ModalityType.MODELESS;
				}
				if (ModalityType_Renamed == value)
				{
					return;
				}
    
				CheckModalityPermission(value);
    
				ModalityType_Renamed = value;
				Modal_Renamed = (ModalityType_Renamed != ModalityType.MODELESS);
			}
		}


		/// <summary>
		/// Gets the title of the dialog. The title is displayed in the
		/// dialog's border. </summary>
		/// <returns>    the title of this dialog window. The title may be
		///            <code>null</code>. </returns>
		/// <seealso cref=       java.awt.Dialog#setTitle </seealso>
		public virtual String Title
		{
			get
			{
				return Title_Renamed;
			}
			set
			{
				String oldTitle = this.Title_Renamed;
    
				lock (this)
				{
					this.Title_Renamed = value;
					DialogPeer peer = (DialogPeer)this.Peer_Renamed;
					if (peer != null)
					{
						peer.Title = value;
					}
				}
				FirePropertyChange("title", oldTitle, value);
			}
		}


		/// <returns> true if we actually showed, false if we just called toFront() </returns>
		private bool ConditionalShow(Component toFocus, AtomicLong time)
		{
			bool retval;

			CloseSplashScreen();

			lock (TreeLock)
			{
				if (Peer_Renamed == null)
				{
					AddNotify();
				}
				ValidateUnconditionally();
				if (Visible_Renamed)
				{
					ToFront();
					retval = false;
				}
				else
				{
					Visible_Renamed = retval = true;

					// check if this dialog should be modal blocked BEFORE calling peer.show(),
					// otherwise, a pair of FOCUS_GAINED and FOCUS_LOST may be mistakenly
					// generated for the dialog
					if (!Modal)
					{
						CheckShouldBeBlocked(this);
					}
					else
					{
						ModalDialogs.add(this);
						ModalShow();
					}

					if (toFocus != null && time != null && Focusable && Enabled && !ModalBlocked)
					{
						// keep the KeyEvents from being dispatched
						// until the focus has been transfered
						time.Set(Toolkit.EventQueue.MostRecentKeyEventTime);
						KeyboardFocusManager.CurrentKeyboardFocusManager.EnqueueKeyEvents(time.Get(), toFocus);
					}

					// This call is required as the show() method of the Dialog class
					// does not invoke the super.show(). So wried... :(
					MixOnShowing();

					Peer_Renamed.Visible = true; // now guaranteed never to block
					if (ModalBlocked)
					{
						ModalBlocker_Renamed.ToFront();
					}

					LocationByPlatform = false;
					for (int i = 0; i < OwnedWindowList.Count; i++)
					{
						Window child = OwnedWindowList[i].get();
						if ((child != null) && child.ShowWithParent)
						{
							child.Show();
							child.ShowWithParent = false;
						} // endif
					} // endfor
					Window.UpdateChildFocusableWindowState(this);

					CreateHierarchyEvents(HierarchyEvent.HIERARCHY_CHANGED, this, Parent_Renamed, HierarchyEvent.SHOWING_CHANGED, Toolkit.EnabledOnToolkit(AWTEvent.HIERARCHY_EVENT_MASK));
					if (ComponentListener != null || (EventMask & AWTEvent.COMPONENT_EVENT_MASK) != 0 || Toolkit.EnabledOnToolkit(AWTEvent.COMPONENT_EVENT_MASK))
					{
						ComponentEvent e = new ComponentEvent(this, ComponentEvent.COMPONENT_SHOWN);
						Toolkit.EventQueue.PostEvent(e);
					}
				}
			}

			if (retval && (State & OPENED) == 0)
			{
				PostWindowEvent(WindowEvent.WINDOW_OPENED);
				State |= OPENED;
			}

			return retval;
		}

		/// <summary>
		/// Shows or hides this {@code Dialog} depending on the value of parameter
		/// {@code b}. </summary>
		/// <param name="b"> if {@code true}, makes the {@code Dialog} visible,
		/// otherwise hides the {@code Dialog}.
		/// If the dialog and/or its owner
		/// are not yet displayable, both are made displayable.  The
		/// dialog will be validated prior to being made visible.
		/// If {@code false}, hides the {@code Dialog} and then causes {@code setVisible(true)}
		/// to return if it is currently blocked.
		/// <para>
		/// <b>Notes for modal dialogs</b>.
		/// <ul>
		/// <li>{@code setVisible(true)}:  If the dialog is not already
		/// visible, this call will not return until the dialog is
		/// hidden by calling {@code setVisible(false)} or
		/// {@code dispose}.
		/// <li>{@code setVisible(false)}:  Hides the dialog and then
		/// returns on {@code setVisible(true)} if it is currently blocked.
		/// <li>It is OK to call this method from the event dispatching
		/// thread because the toolkit ensures that other events are
		/// not blocked while this method is blocked.
		/// </ul>
		/// </para>
		/// </param>
		/// <seealso cref= java.awt.Window#setVisible </seealso>
		/// <seealso cref= java.awt.Window#dispose </seealso>
		/// <seealso cref= java.awt.Component#isDisplayable </seealso>
		/// <seealso cref= java.awt.Component#validate </seealso>
		/// <seealso cref= java.awt.Dialog#isModal </seealso>
		public override bool Visible
		{
			set
			{
				base.Visible = value;
			}
		}

	   /// <summary>
	   /// Makes the {@code Dialog} visible. If the dialog and/or its owner
	   /// are not yet displayable, both are made displayable.  The
	   /// dialog will be validated prior to being made visible.
	   /// If the dialog is already visible, this will bring the dialog
	   /// to the front.
	   /// <para>
	   /// If the dialog is modal and is not already visible, this call
	   /// will not return until the dialog is hidden by calling hide or
	   /// dispose. It is permissible to show modal dialogs from the event
	   /// dispatching thread because the toolkit will ensure that another
	   /// event pump runs while the one which invoked this method is blocked.
	   /// </para>
	   /// </summary>
	   /// <seealso cref= Component#hide </seealso>
	   /// <seealso cref= Component#isDisplayable </seealso>
	   /// <seealso cref= Component#validate </seealso>
	   /// <seealso cref= #isModal </seealso>
	   /// <seealso cref= Window#setVisible(boolean) </seealso>
	   /// @deprecated As of JDK version 1.5, replaced by
	   /// <seealso cref="#setVisible(boolean) setVisible(boolean)"/>. 
		[Obsolete("As of JDK version 1.5, replaced by")]
		public override void Show()
		{
			if (!Initialized)
			{
				throw new IllegalStateException("The dialog component " + "has not been initialized properly");
			}

			BeforeFirstShow = false;
			if (!Modal)
			{
				ConditionalShow(null, null);
			}
			else
			{
				AppContext showAppContext = AppContext.AppContext;

				AtomicLong time = new AtomicLong();
				Component predictedFocusOwner = null;
				try
				{
					predictedFocusOwner = MostRecentFocusOwner;
					if (ConditionalShow(predictedFocusOwner, time))
					{
						ModalFilter = ModalEventFilter.CreateFilterForDialog(this);
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final Conditional cond = new Conditional()
						Conditional cond = new ConditionalAnonymousInnerClassHelper(this);

						// if this dialog is toolkit-modal, the filter should be added
						// to all EDTs (for all AppContexts)
						if (ModalityType_Renamed == ModalityType.TOOLKIT_MODAL)
						{
							IEnumerator<AppContext> it = AppContext.AppContexts.GetEnumerator();
							while (it.MoveNext())
							{
								AppContext appContext = it.Current;
								if (appContext == showAppContext)
								{
									continue;
								}
								EventQueue eventQueue = (EventQueue)appContext.get(AppContext.EVENT_QUEUE_KEY);
								// it may occur that EDT for appContext hasn't been started yet, so
								// we post an empty invocation event to trigger EDT initialization
								Runnable createEDT = new RunnableAnonymousInnerClassHelper(this);
								eventQueue.PostEvent(new InvocationEvent(this, createEDT));
								EventDispatchThread edt = eventQueue.DispatchThread;
								edt.AddEventFilter(ModalFilter);
							}
						}

						ModalityPushed();
						try
						{
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final EventQueue eventQueue = java.security.AccessController.doPrivileged(new java.security.PrivilegedAction<EventQueue>()
							EventQueue eventQueue = AccessController.doPrivileged(new PrivilegedActionAnonymousInnerClassHelper(this));
							SecondaryLoop = eventQueue.CreateSecondaryLoop(cond, ModalFilter, 0);
							if (!SecondaryLoop.Enter())
							{
								SecondaryLoop = null;
							}
						}
						finally
						{
							ModalityPopped();
						}

						// if this dialog is toolkit-modal, its filter must be removed
						// from all EDTs (for all AppContexts)
						if (ModalityType_Renamed == ModalityType.TOOLKIT_MODAL)
						{
							IEnumerator<AppContext> it = AppContext.AppContexts.GetEnumerator();
							while (it.MoveNext())
							{
								AppContext appContext = it.Current;
								if (appContext == showAppContext)
								{
									continue;
								}
								EventQueue eventQueue = (EventQueue)appContext.get(AppContext.EVENT_QUEUE_KEY);
								EventDispatchThread edt = eventQueue.DispatchThread;
								edt.RemoveEventFilter(ModalFilter);
							}
						}

						if (WindowClosingException != null)
						{
							WindowClosingException.FillInStackTrace();
							throw WindowClosingException;
						}
					}
				}
				finally
				{
					if (predictedFocusOwner != null)
					{
						// Restore normal key event dispatching
						KeyboardFocusManager.CurrentKeyboardFocusManager.DequeueKeyEvents(time.Get(), predictedFocusOwner);
					}
				}
			}
		}

		private class ConditionalAnonymousInnerClassHelper : Conditional
		{
			private readonly Dialog OuterInstance;

			public ConditionalAnonymousInnerClassHelper(Dialog outerInstance)
			{
				this.OuterInstance = outerInstance;
			}

			public virtual bool Evaluate()
			{
				return OuterInstance.WindowClosingException == null;
			}
		}

		private class RunnableAnonymousInnerClassHelper : Runnable
		{
			private readonly Dialog OuterInstance;

			public RunnableAnonymousInnerClassHelper(Dialog outerInstance)
			{
				this.OuterInstance = outerInstance;
			}

			public virtual void Run()
			{
			};
		}

		private class PrivilegedActionAnonymousInnerClassHelper : PrivilegedAction<EventQueue>
		{
			private readonly Dialog OuterInstance;

			public PrivilegedActionAnonymousInnerClassHelper(Dialog outerInstance)
			{
				this.OuterInstance = outerInstance;
			}

			public virtual EventQueue Run()
			{
				return Toolkit.DefaultToolkit.SystemEventQueue;
			}
		}

		internal void ModalityPushed()
		{
			Toolkit tk = Toolkit.DefaultToolkit;
			if (tk is SunToolkit)
			{
				SunToolkit stk = (SunToolkit)tk;
				stk.notifyModalityPushed(this);
			}
		}

		internal void ModalityPopped()
		{
			Toolkit tk = Toolkit.DefaultToolkit;
			if (tk is SunToolkit)
			{
				SunToolkit stk = (SunToolkit)tk;
				stk.notifyModalityPopped(this);
			}
		}

		internal virtual void InterruptBlocking()
		{
			if (Modal)
			{
				DisposeImpl();
			}
			else if (WindowClosingException != null)
			{
				WindowClosingException.FillInStackTrace();
				Console.WriteLine(WindowClosingException.ToString());
				Console.Write(WindowClosingException.StackTrace);
				WindowClosingException = null;
			}
		}

		private void HideAndDisposePreHandler()
		{
			IsInHide = true;
			lock (TreeLock)
			{
				if (SecondaryLoop != null)
				{
					ModalHide();
					// dialog can be shown and then disposed before its
					// modal filter is created
					if (ModalFilter != null)
					{
						ModalFilter.Disable();
					}
					ModalDialogs.remove(this);
				}
			}
		}
		private void HideAndDisposeHandler()
		{
			if (SecondaryLoop != null)
			{
				SecondaryLoop.Exit();
				SecondaryLoop = null;
			}
			IsInHide = false;
		}

		/// <summary>
		/// Hides the Dialog and then causes {@code show} to return if it is currently
		/// blocked. </summary>
		/// <seealso cref= Window#show </seealso>
		/// <seealso cref= Window#dispose </seealso>
		/// <seealso cref= Window#setVisible(boolean) </seealso>
		/// @deprecated As of JDK version 1.5, replaced by
		/// <seealso cref="#setVisible(boolean) setVisible(boolean)"/>. 
		[Obsolete("As of JDK version 1.5, replaced by")]
		public override void Hide()
		{
			HideAndDisposePreHandler();
			base.Hide();
			// fix for 5048370: if hide() is called from super.doDispose(), then
			// hideAndDisposeHandler() should not be called here as it will be called
			// at the end of doDispose()
			if (!IsInDispose)
			{
				HideAndDisposeHandler();
			}
		}

		/// <summary>
		/// Disposes the Dialog and then causes show() to return if it is currently
		/// blocked.
		/// </summary>
		internal override void DoDispose()
		{
			// fix for 5048370: set isInDispose flag to true to prevent calling
			// to hideAndDisposeHandler() from hide()
			IsInDispose = true;
			base.DoDispose();
			HideAndDisposeHandler();
			IsInDispose = false;
		}

		/// <summary>
		/// {@inheritDoc}
		/// <para>
		/// If this dialog is modal and blocks some windows, then all of them are
		/// also sent to the back to keep them below the blocking dialog.
		/// 
		/// </para>
		/// </summary>
		/// <seealso cref= java.awt.Window#toBack </seealso>
		public override void ToBack()
		{
			base.ToBack();
			if (Visible_Renamed)
			{
				lock (TreeLock)
				{
					foreach (Window w in BlockedWindows)
					{
						w.ToBack_NoClientCode();
					}
				}
			}
		}

		/// <summary>
		/// Indicates whether this dialog is resizable by the user.
		/// By default, all dialogs are initially resizable. </summary>
		/// <returns>    <code>true</code> if the user can resize the dialog;
		///            <code>false</code> otherwise. </returns>
		/// <seealso cref=       java.awt.Dialog#setResizable </seealso>
		public virtual bool Resizable
		{
			get
			{
				return Resizable_Renamed;
			}
			set
			{
				bool testvalid = false;
    
				lock (this)
				{
					this.Resizable_Renamed = value;
					DialogPeer peer = (DialogPeer)this.Peer_Renamed;
					if (peer != null)
					{
						peer.Resizable = value;
						testvalid = true;
					}
				}
    
				// On some platforms, changing the value state affects
				// the insets of the Dialog. If we could, we'd call invalidate()
				// from the peer, but we need to guarantee that we're not holding
				// the Dialog lock when we call invalidate().
				if (testvalid)
				{
					InvalidateIfValid();
				}
			}
		}



		/// <summary>
		/// Disables or enables decorations for this dialog.
		/// <para>
		/// This method can only be called while the dialog is not displayable. To
		/// make this dialog decorated, it must be opaque and have the default shape,
		/// otherwise the {@code IllegalComponentStateException} will be thrown.
		/// Refer to <seealso cref="Window#setShape"/>, <seealso cref="Window#setOpacity"/> and {@link
		/// Window#setBackground} for details
		/// 
		/// </para>
		/// </summary>
		/// <param name="undecorated"> {@code true} if no dialog decorations are to be
		///         enabled; {@code false} if dialog decorations are to be enabled
		/// </param>
		/// <exception cref="IllegalComponentStateException"> if the dialog is displayable </exception>
		/// <exception cref="IllegalComponentStateException"> if {@code undecorated} is
		///      {@code false}, and this dialog does not have the default shape </exception>
		/// <exception cref="IllegalComponentStateException"> if {@code undecorated} is
		///      {@code false}, and this dialog opacity is less than {@code 1.0f} </exception>
		/// <exception cref="IllegalComponentStateException"> if {@code undecorated} is
		///      {@code false}, and the alpha value of this dialog background
		///      color is less than {@code 1.0f}
		/// </exception>
		/// <seealso cref=    #isUndecorated </seealso>
		/// <seealso cref=    Component#isDisplayable </seealso>
		/// <seealso cref=    Window#getShape </seealso>
		/// <seealso cref=    Window#getOpacity </seealso>
		/// <seealso cref=    Window#getBackground
		/// 
		/// @since 1.4 </seealso>
		public virtual bool Undecorated
		{
			set
			{
				/* Make sure we don't run in the middle of peer creation.*/
				lock (TreeLock)
				{
					if (Displayable)
					{
						throw new IllegalComponentStateException("The dialog is displayable.");
					}
					if (!value)
					{
						if (Opacity < 1.0f)
						{
							throw new IllegalComponentStateException("The dialog is not opaque");
						}
						if (Shape != null)
						{
							throw new IllegalComponentStateException("The dialog does not have a default shape");
						}
						Color bg = Background;
						if ((bg != null) && (bg.Alpha < 255))
						{
							throw new IllegalComponentStateException("The dialog background color is not opaque");
						}
					}
					this.Undecorated_Renamed = value;
				}
			}
			get
			{
				return Undecorated_Renamed;
			}
		}


		/// <summary>
		/// {@inheritDoc}
		/// </summary>
		public override float Opacity
		{
			set
			{
				lock (TreeLock)
				{
					if ((value < 1.0f) && !Undecorated)
					{
						throw new IllegalComponentStateException("The dialog is decorated");
					}
					base.Opacity = value;
				}
			}
		}

		/// <summary>
		/// {@inheritDoc}
		/// </summary>
		public override Shape Shape
		{
			set
			{
				lock (TreeLock)
				{
					if ((value != null) && !Undecorated)
					{
						throw new IllegalComponentStateException("The dialog is decorated");
					}
					base.Shape = value;
				}
			}
		}

		/// <summary>
		/// {@inheritDoc}
		/// </summary>
		public override Color Background
		{
			set
			{
				lock (TreeLock)
				{
					if ((value != null) && (value.Alpha < 255) && !Undecorated)
					{
						throw new IllegalComponentStateException("The dialog is decorated");
					}
					base.Background = value;
				}
			}
		}

		/// <summary>
		/// Returns a string representing the state of this dialog. This
		/// method is intended to be used only for debugging purposes, and the
		/// content and format of the returned string may vary between
		/// implementations. The returned string may be empty but may not be
		/// <code>null</code>.
		/// </summary>
		/// <returns>    the parameter string of this dialog window. </returns>
		protected internal override String ParamString()
		{
			String str = base.ParamString() + "," + ModalityType_Renamed;
			if (Title_Renamed != null)
			{
				str += ",title=" + Title_Renamed;
			}
			return str;
		}

		/// <summary>
		/// Initialize JNI field and method IDs
		/// </summary>
//JAVA TO C# CONVERTER TODO TASK: Replace 'unknown' with the appropriate dll name:
		[DllImport("unknown")]
		private static extern void initIDs();

		/*
		 * --- Modality support ---
		 *
		 */

		/*
		 * This method is called only for modal dialogs.
		 *
		 * Goes through the list of all visible top-level windows and
		 * divide them into three distinct groups: blockers of this dialog,
		 * blocked by this dialog and all others. Then blocks this dialog
		 * by first met dialog from the first group (if any) and blocks all
		 * the windows from the second group.
		 */
		internal virtual void ModalShow()
		{
			// find all the dialogs that block this one
			IdentityArrayList<Dialog> blockers = new IdentityArrayList<Dialog>();
			foreach (Dialog d in ModalDialogs)
			{
				if (d.ShouldBlock(this))
				{
					Window w = d;
					while ((w != null) && (w != this))
					{
						w = w.Owner_NoClientCode;
					}
					if ((w == this) || !ShouldBlock(d) || (ModalityType_Renamed.CompareTo(d.ModalityType) < 0))
					{
						blockers.add(d);
					}
				}
			}

			// add all blockers' blockers to blockers :)
			for (int i = 0; i < blockers.size(); i++)
			{
				Dialog blocker = blockers.get(i);
				if (blocker.ModalBlocked)
				{
					Dialog blockerBlocker = blocker.ModalBlocker;
					if (!blockers.contains(blockerBlocker))
					{
						blockers.add(i + 1, blockerBlocker);
					}
				}
			}

			if (blockers.size() > 0)
			{
				blockers.get(0).blockWindow(this);
			}

			// find all windows from blockers' hierarchies
			IdentityArrayList<Window> blockersHierarchies = new IdentityArrayList<Window>(blockers);
			int k = 0;
			while (k < blockersHierarchies.size())
			{
				Window w = blockersHierarchies.get(k);
				Window[] ownedWindows = w.OwnedWindows_NoClientCode;
				foreach (Window win in ownedWindows)
				{
					blockersHierarchies.add(win);
				}
				k++;
			}

			IList<Window> toBlock = new IdentityLinkedList<Window>();
			// block all windows from scope of blocking except from blockers' hierarchies
			IdentityArrayList<Window> unblockedWindows = Window.AllUnblockedWindows;
			foreach (Window w in unblockedWindows)
			{
				if (ShouldBlock(w) && !blockersHierarchies.contains(w))
				{
					if ((w is Dialog) && ((Dialog)w).Modal_NoClientCode)
					{
						Dialog wd = (Dialog)w;
						if (wd.ShouldBlock(this) && (ModalDialogs.IndexOf(wd) > ModalDialogs.IndexOf(this)))
						{
							continue;
						}
					}
					toBlock.Add(w);
				}
			}
			BlockWindows(toBlock);

			if (!ModalBlocked)
			{
				UpdateChildrenBlocking();
			}
		}

		/*
		 * This method is called only for modal dialogs.
		 *
		 * Unblocks all the windows blocked by this modal dialog. After
		 * each of them has been unblocked, it is checked to be blocked by
		 * any other modal dialogs.
		 */
		internal virtual void ModalHide()
		{
			// we should unblock all the windows first...
			IdentityArrayList<Window> save = new IdentityArrayList<Window>();
			int blockedWindowsCount = BlockedWindows.size();
			for (int i = 0; i < blockedWindowsCount; i++)
			{
				Window w = BlockedWindows.get(0);
				save.add(w);
				UnblockWindow(w); // also removes w from blockedWindows
			}
			// ... and only after that check if they should be blocked
			// by another dialogs
			for (int i = 0; i < blockedWindowsCount; i++)
			{
				Window w = save.get(i);
				if ((w is Dialog) && ((Dialog)w).Modal_NoClientCode)
				{
					Dialog d = (Dialog)w;
					d.ModalShow();
				}
				else
				{
					CheckShouldBeBlocked(w);
				}
			}
		}

		/*
		 * Returns whether the given top-level window should be blocked by
		 * this dialog. Note, that the given window can be also a modal dialog
		 * and it should block this dialog, but this method do not take such
		 * situations into consideration (such checks are performed in the
		 * modalShow() and modalHide() methods).
		 *
		 * This method should be called on the getTreeLock() lock.
		 */
		internal virtual bool ShouldBlock(Window w)
		{
			if (!Visible_NoClientCode || (!w.Visible_NoClientCode && !w.IsInShow) || IsInHide || (w == this) || !Modal_NoClientCode)
			{
				return false;
			}
			if ((w is Dialog) && ((Dialog)w).IsInHide)
			{
				return false;
			}
			// check if w is from children hierarchy
			// fix for 6271546: we should also take into consideration child hierarchies
			// of this dialog's blockers
			Window blockerToCheck = this;
			while (blockerToCheck != null)
			{
				Component c = w;
				while ((c != null) && (c != blockerToCheck))
				{
					c = c.Parent_NoClientCode;
				}
				if (c == blockerToCheck)
				{
					return false;
				}
				blockerToCheck = blockerToCheck.ModalBlocker;
			}
			switch (ModalityType_Renamed)
			{
				case java.awt.Dialog.ModalityType.MODELESS:
					return false;
				case java.awt.Dialog.ModalityType.DOCUMENT_MODAL:
					if (w.IsModalExcluded(ModalExclusionType.APPLICATION_EXCLUDE))
					{
						// application- and toolkit-excluded windows are not blocked by
						// document-modal dialogs from outside their children hierarchy
						Component c = this;
						while ((c != null) && (c != w))
						{
							c = c.Parent_NoClientCode;
						}
						return c == w;
					}
					else
					{
						return DocumentRoot == w.DocumentRoot;
					}
				case java.awt.Dialog.ModalityType.APPLICATION_MODAL:
					return !w.IsModalExcluded(ModalExclusionType.APPLICATION_EXCLUDE) && (AppContext == w.AppContext);
				case java.awt.Dialog.ModalityType.TOOLKIT_MODAL:
					return !w.IsModalExcluded(ModalExclusionType.TOOLKIT_EXCLUDE);
			}

			return false;
		}

		/*
		 * Adds the given top-level window to the list of blocked
		 * windows for this dialog and marks it as modal blocked.
		 * If the window is already blocked by some modal dialog,
		 * does nothing.
		 */
		internal virtual void BlockWindow(Window w)
		{
			if (!w.ModalBlocked)
			{
				w.SetModalBlocked(this, true, true);
				BlockedWindows.add(w);
			}
		}

		internal virtual void BlockWindows(IList<Window> toBlock)
		{
			DialogPeer dpeer = (DialogPeer)Peer_Renamed;
			if (dpeer == null)
			{
				return;
			}
			IEnumerator<Window> it = toBlock.GetEnumerator();
			while (it.MoveNext())
			{
				Window w = it.Current;
				if (!w.ModalBlocked)
				{
					w.SetModalBlocked(this, true, false);
				}
				else
				{
					it.remove();
				}
			}
			dpeer.BlockWindows(toBlock);
			BlockedWindows.addAll(toBlock);
		}

		/*
		 * Removes the given top-level window from the list of blocked
		 * windows for this dialog and marks it as unblocked. If the
		 * window is not modal blocked, does nothing.
		 */
		internal virtual void UnblockWindow(Window w)
		{
			if (w.ModalBlocked && BlockedWindows.contains(w))
			{
				BlockedWindows.remove(w);
				w.SetModalBlocked(this, false, true);
			}
		}

		/*
		 * Checks if any other modal dialog D blocks the given window.
		 * If such D exists, mark the window as blocked by D.
		 */
		internal static void CheckShouldBeBlocked(Window w)
		{
			lock (w.TreeLock)
			{
				for (int i = 0; i < ModalDialogs.size(); i++)
				{
					Dialog modalDialog = ModalDialogs.get(i);
					if (modalDialog.ShouldBlock(w))
					{
						modalDialog.BlockWindow(w);
						break;
					}
				}
			}
		}

		private void CheckModalityPermission(ModalityType mt)
		{
			if (mt == ModalityType.TOOLKIT_MODAL)
			{
				SecurityManager sm = System.SecurityManager;
				if (sm != null)
				{
					sm.CheckPermission(SecurityConstants.AWT.TOOLKIT_MODALITY_PERMISSION);
				}
			}
		}

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: private void readObject(java.io.ObjectInputStream s) throws ClassNotFoundException, java.io.IOException, HeadlessException
		private void ReadObject(ObjectInputStream s)
		{
			GraphicsEnvironment.CheckHeadless();

			ObjectInputStream.GetField fields = s.ReadFields();

			ModalityType localModalityType = (ModalityType)fields.Get("modalityType", null);

			try
			{
				CheckModalityPermission(localModalityType);
			}
			catch (AccessControlException)
			{
				localModalityType = DEFAULT_MODALITY_TYPE;
			}

			// in 1.5 or earlier modalityType was absent, so use "modal" instead
			if (localModalityType == null)
			{
				this.Modal_Renamed = fields.Get("modal", false);
				Modal = Modal_Renamed;
			}
			else
			{
				this.ModalityType_Renamed = localModalityType;
			}

			this.Resizable_Renamed = fields.Get("resizable", true);
			this.Undecorated_Renamed = fields.Get("undecorated", false);
			this.Title_Renamed = (String)fields.Get("title", "");

			BlockedWindows = new IdentityArrayList<>();

			SunToolkit.checkAndSetPolicy(this);

			Initialized = true;

		}

		/*
		 * --- Accessibility Support ---
		 *
		 */

		/// <summary>
		/// Gets the AccessibleContext associated with this Dialog.
		/// For dialogs, the AccessibleContext takes the form of an
		/// AccessibleAWTDialog.
		/// A new AccessibleAWTDialog instance is created if necessary.
		/// </summary>
		/// <returns> an AccessibleAWTDialog that serves as the
		///         AccessibleContext of this Dialog
		/// @since 1.3 </returns>
		public override AccessibleContext AccessibleContext
		{
			get
			{
				if (AccessibleContext_Renamed == null)
				{
					AccessibleContext_Renamed = new AccessibleAWTDialog(this);
				}
				return AccessibleContext_Renamed;
			}
		}

		/// <summary>
		/// This class implements accessibility support for the
		/// <code>Dialog</code> class.  It provides an implementation of the
		/// Java Accessibility API appropriate to dialog user-interface elements.
		/// @since 1.3
		/// </summary>
		protected internal class AccessibleAWTDialog : AccessibleAWTWindow
		{
			private readonly Dialog OuterInstance;

			public AccessibleAWTDialog(Dialog outerInstance) : base(outerInstance)
			{
				this.OuterInstance = outerInstance;
			}

			/*
			 * JDK 1.3 serialVersionUID
			 */
			internal const long SerialVersionUID = 4837230331833941201L;

			/// <summary>
			/// Get the role of this object.
			/// </summary>
			/// <returns> an instance of AccessibleRole describing the role of the
			/// object </returns>
			/// <seealso cref= AccessibleRole </seealso>
			public override AccessibleRole AccessibleRole
			{
				get
				{
					return AccessibleRole.DIALOG;
				}
			}

			/// <summary>
			/// Get the state of this object.
			/// </summary>
			/// <returns> an instance of AccessibleStateSet containing the current
			/// state set of the object </returns>
			/// <seealso cref= AccessibleState </seealso>
			public override AccessibleStateSet AccessibleStateSet
			{
				get
				{
					AccessibleStateSet states = base.AccessibleStateSet;
					if (outerInstance.FocusOwner != null)
					{
						states.add(AccessibleState.ACTIVE);
					}
					if (outerInstance.Modal)
					{
						states.add(AccessibleState.MODAL);
					}
					if (outerInstance.Resizable)
					{
						states.add(AccessibleState.RESIZABLE);
					}
					return states;
				}
			}

		} // inner class AccessibleAWTDialog
	}

}
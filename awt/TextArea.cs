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


	/// <summary>
	/// A <code>TextArea</code> object is a multi-line region
	/// that displays text. It can be set to allow editing or
	/// to be read-only.
	/// <para>
	/// The following image shows the appearance of a text area:
	/// </para>
	/// <para>
	/// <img src="doc-files/TextArea-1.gif" alt="A TextArea showing the word 'Hello!'"
	/// style="float:center; margin: 7px 10px;">
	/// </para>
	/// <para>
	/// This text area could be created by the following line of code:
	/// 
	/// <hr><blockquote><pre>
	/// new TextArea("Hello", 5, 40);
	/// </pre></blockquote><hr>
	/// </para>
	/// <para>
	/// @author      Sami Shaio
	/// @since       JDK1.0
	/// </para>
	/// </summary>
	public class TextArea : TextComponent
	{

		/// <summary>
		/// The number of rows in the <code>TextArea</code>.
		/// This parameter will determine the text area's height.
		/// Guaranteed to be non-negative.
		/// 
		/// @serial </summary>
		/// <seealso cref= #getRows() </seealso>
		/// <seealso cref= #setRows(int) </seealso>
		internal int Rows_Renamed;

		/// <summary>
		/// The number of columns in the <code>TextArea</code>.
		/// A column is an approximate average character
		/// width that is platform-dependent.
		/// This parameter will determine the text area's width.
		/// Guaranteed to be non-negative.
		/// 
		/// @serial </summary>
		/// <seealso cref=  #setColumns(int) </seealso>
		/// <seealso cref=  #getColumns() </seealso>
		internal int Columns_Renamed;

		private const String @base = "text";
		private static int NameCounter = 0;

		/// <summary>
		/// Create and display both vertical and horizontal scrollbars.
		/// @since JDK1.1
		/// </summary>
		public const int SCROLLBARS_BOTH = 0;

		/// <summary>
		/// Create and display vertical scrollbar only.
		/// @since JDK1.1
		/// </summary>
		public const int SCROLLBARS_VERTICAL_ONLY = 1;

		/// <summary>
		/// Create and display horizontal scrollbar only.
		/// @since JDK1.1
		/// </summary>
		public const int SCROLLBARS_HORIZONTAL_ONLY = 2;

		/// <summary>
		/// Do not create or display any scrollbars for the text area.
		/// @since JDK1.1
		/// </summary>
		public const int SCROLLBARS_NONE = 3;

		/// <summary>
		/// Determines which scrollbars are created for the
		/// text area. It can be one of four values :
		/// <code>SCROLLBARS_BOTH</code> = both scrollbars.<BR>
		/// <code>SCROLLBARS_HORIZONTAL_ONLY</code> = Horizontal bar only.<BR>
		/// <code>SCROLLBARS_VERTICAL_ONLY</code> = Vertical bar only.<BR>
		/// <code>SCROLLBARS_NONE</code> = No scrollbars.<BR>
		/// 
		/// @serial </summary>
		/// <seealso cref= #getScrollbarVisibility() </seealso>
		private int ScrollbarVisibility_Renamed;

		/// <summary>
		/// Cache the Sets of forward and backward traversal keys so we need not
		/// look them up each time.
		/// </summary>
		private static Set<AWTKeyStroke> ForwardTraversalKeys, BackwardTraversalKeys;

		/*
		 * JDK 1.1 serialVersionUID
		 */
		 private const long SerialVersionUID = 3692302836626095722L;

		/// <summary>
		/// Initialize JNI field and method ids
		/// </summary>
//JAVA TO C# CONVERTER TODO TASK: Replace 'unknown' with the appropriate dll name:
		[DllImport("unknown")]
		private static extern void initIDs();

		static TextArea()
		{
			/* ensure that the necessary native libraries are loaded */
			Toolkit.LoadLibraries();
			if (!GraphicsEnvironment.Headless)
			{
				initIDs();
			}
			ForwardTraversalKeys = KeyboardFocusManager.InitFocusTraversalKeysSet("ctrl TAB", new HashSet<AWTKeyStroke>());
			BackwardTraversalKeys = KeyboardFocusManager.InitFocusTraversalKeysSet("ctrl shift TAB", new HashSet<AWTKeyStroke>());
		}

		/// <summary>
		/// Constructs a new text area with the empty string as text.
		/// This text area is created with scrollbar visibility equal to
		/// <seealso cref="#SCROLLBARS_BOTH"/>, so both vertical and horizontal
		/// scrollbars will be visible for this text area. </summary>
		/// <exception cref="HeadlessException"> if
		///    <code>GraphicsEnvironment.isHeadless</code> returns true </exception>
		/// <seealso cref= java.awt.GraphicsEnvironment#isHeadless() </seealso>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public TextArea() throws HeadlessException
		public TextArea() : this("", 0, 0, SCROLLBARS_BOTH)
		{
		}

		/// <summary>
		/// Constructs a new text area with the specified text.
		/// This text area is created with scrollbar visibility equal to
		/// <seealso cref="#SCROLLBARS_BOTH"/>, so both vertical and horizontal
		/// scrollbars will be visible for this text area. </summary>
		/// <param name="text">       the text to be displayed; if
		///             <code>text</code> is <code>null</code>, the empty
		///             string <code>""</code> will be displayed </param>
		/// <exception cref="HeadlessException"> if
		///        <code>GraphicsEnvironment.isHeadless</code> returns true </exception>
		/// <seealso cref= java.awt.GraphicsEnvironment#isHeadless() </seealso>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public TextArea(String text) throws HeadlessException
		public TextArea(String text) : this(text, 0, 0, SCROLLBARS_BOTH)
		{
		}

		/// <summary>
		/// Constructs a new text area with the specified number of
		/// rows and columns and the empty string as text.
		/// A column is an approximate average character
		/// width that is platform-dependent.  The text area is created with
		/// scrollbar visibility equal to <seealso cref="#SCROLLBARS_BOTH"/>, so both
		/// vertical and horizontal scrollbars will be visible for this
		/// text area. </summary>
		/// <param name="rows"> the number of rows </param>
		/// <param name="columns"> the number of columns </param>
		/// <exception cref="HeadlessException"> if
		///     <code>GraphicsEnvironment.isHeadless</code> returns true </exception>
		/// <seealso cref= java.awt.GraphicsEnvironment#isHeadless() </seealso>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public TextArea(int rows, int columns) throws HeadlessException
		public TextArea(int rows, int columns) : this("", rows, columns, SCROLLBARS_BOTH)
		{
		}

		/// <summary>
		/// Constructs a new text area with the specified text,
		/// and with the specified number of rows and columns.
		/// A column is an approximate average character
		/// width that is platform-dependent.  The text area is created with
		/// scrollbar visibility equal to <seealso cref="#SCROLLBARS_BOTH"/>, so both
		/// vertical and horizontal scrollbars will be visible for this
		/// text area. </summary>
		/// <param name="text">       the text to be displayed; if
		///             <code>text</code> is <code>null</code>, the empty
		///             string <code>""</code> will be displayed </param>
		/// <param name="rows">      the number of rows </param>
		/// <param name="columns">   the number of columns </param>
		/// <exception cref="HeadlessException"> if
		///   <code>GraphicsEnvironment.isHeadless</code> returns true </exception>
		/// <seealso cref= java.awt.GraphicsEnvironment#isHeadless() </seealso>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public TextArea(String text, int rows, int columns) throws HeadlessException
		public TextArea(String text, int rows, int columns) : this(text, rows, columns, SCROLLBARS_BOTH)
		{
		}

		/// <summary>
		/// Constructs a new text area with the specified text,
		/// and with the rows, columns, and scroll bar visibility
		/// as specified.  All <code>TextArea</code> constructors defer to
		/// this one.
		/// <para>
		/// The <code>TextArea</code> class defines several constants
		/// that can be supplied as values for the
		/// <code>scrollbars</code> argument:
		/// <ul>
		/// <li><code>SCROLLBARS_BOTH</code>,
		/// <li><code>SCROLLBARS_VERTICAL_ONLY</code>,
		/// <li><code>SCROLLBARS_HORIZONTAL_ONLY</code>,
		/// <li><code>SCROLLBARS_NONE</code>.
		/// </ul>
		/// Any other value for the
		/// <code>scrollbars</code> argument is invalid and will result in
		/// this text area being created with scrollbar visibility equal to
		/// the default value of <seealso cref="#SCROLLBARS_BOTH"/>.
		/// </para>
		/// </summary>
		/// <param name="text">       the text to be displayed; if
		///             <code>text</code> is <code>null</code>, the empty
		///             string <code>""</code> will be displayed </param>
		/// <param name="rows">       the number of rows; if
		///             <code>rows</code> is less than <code>0</code>,
		///             <code>rows</code> is set to <code>0</code> </param>
		/// <param name="columns">    the number of columns; if
		///             <code>columns</code> is less than <code>0</code>,
		///             <code>columns</code> is set to <code>0</code> </param>
		/// <param name="scrollbars">  a constant that determines what
		///             scrollbars are created to view the text area
		/// @since      JDK1.1 </param>
		/// <exception cref="HeadlessException"> if
		///    <code>GraphicsEnvironment.isHeadless</code> returns true </exception>
		/// <seealso cref= java.awt.GraphicsEnvironment#isHeadless() </seealso>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public TextArea(String text, int rows, int columns, int scrollbars) throws HeadlessException
		public TextArea(String text, int rows, int columns, int scrollbars) : base(text)
		{

			this.Rows_Renamed = (rows >= 0) ? rows : 0;
			this.Columns_Renamed = (columns >= 0) ? columns : 0;

			if (scrollbars >= SCROLLBARS_BOTH && scrollbars <= SCROLLBARS_NONE)
			{
				this.ScrollbarVisibility_Renamed = scrollbars;
			}
			else
			{
				this.ScrollbarVisibility_Renamed = SCROLLBARS_BOTH;
			}

			SetFocusTraversalKeys(KeyboardFocusManager.FORWARD_TRAVERSAL_KEYS, ForwardTraversalKeys);
			SetFocusTraversalKeys(KeyboardFocusManager.BACKWARD_TRAVERSAL_KEYS, BackwardTraversalKeys);
		}

		/// <summary>
		/// Construct a name for this component.  Called by <code>getName</code>
		/// when the name is <code>null</code>.
		/// </summary>
		internal override String ConstructComponentName()
		{
			lock (typeof(TextArea))
			{
				return @base + NameCounter++;
			}
		}

		/// <summary>
		/// Creates the <code>TextArea</code>'s peer.  The peer allows us to modify
		/// the appearance of the <code>TextArea</code> without changing any of its
		/// functionality.
		/// </summary>
		public override void AddNotify()
		{
			lock (TreeLock)
			{
				if (Peer_Renamed == null)
				{
					Peer_Renamed = Toolkit.CreateTextArea(this);
				}
				base.AddNotify();
			}
		}

		/// <summary>
		/// Inserts the specified text at the specified position
		/// in this text area.
		/// <para>Note that passing <code>null</code> or inconsistent
		/// parameters is invalid and will result in unspecified
		/// behavior.
		/// 
		/// </para>
		/// </summary>
		/// <param name="str"> the non-<code>null</code> text to insert </param>
		/// <param name="pos"> the position at which to insert </param>
		/// <seealso cref=        java.awt.TextComponent#setText </seealso>
		/// <seealso cref=        java.awt.TextArea#replaceRange </seealso>
		/// <seealso cref=        java.awt.TextArea#append
		/// @since      JDK1.1 </seealso>
		public virtual void Insert(String str, int pos)
		{
			InsertText(str, pos);
		}

		/// @deprecated As of JDK version 1.1,
		/// replaced by <code>insert(String, int)</code>. 
		[Obsolete("As of JDK version 1.1,")]
		public virtual void InsertText(String str, int pos)
		{
			lock (this)
			{
				TextAreaPeer peer = (TextAreaPeer)this.Peer_Renamed;
				if (peer != null)
				{
					peer.Insert(str, pos);
				}
				else
				{
					Text_Renamed = Text_Renamed.Substring(0, pos) + str + Text_Renamed.Substring(pos);
				}
			}
		}

		/// <summary>
		/// Appends the given text to the text area's current text.
		/// <para>Note that passing <code>null</code> or inconsistent
		/// parameters is invalid and will result in unspecified
		/// behavior.
		/// 
		/// </para>
		/// </summary>
		/// <param name="str"> the non-<code>null</code> text to append </param>
		/// <seealso cref=       java.awt.TextArea#insert
		/// @since     JDK1.1 </seealso>
		public virtual void Append(String str)
		{
			AppendText(str);
		}

		/// @deprecated As of JDK version 1.1,
		/// replaced by <code>append(String)</code>. 
		[Obsolete("As of JDK version 1.1,")]
		public virtual void AppendText(String str)
		{
			lock (this)
			{
				if (Peer_Renamed != null)
				{
					InsertText(str, Text.Length());
				}
				else
				{
					Text_Renamed = Text_Renamed + str;
				}
			}
		}

		/// <summary>
		/// Replaces text between the indicated start and end positions
		/// with the specified replacement text.  The text at the end
		/// position will not be replaced.  The text at the start
		/// position will be replaced (unless the start position is the
		/// same as the end position).
		/// The text position is zero-based.  The inserted substring may be
		/// of a different length than the text it replaces.
		/// <para>Note that passing <code>null</code> or inconsistent
		/// parameters is invalid and will result in unspecified
		/// behavior.
		/// 
		/// </para>
		/// </summary>
		/// <param name="str">      the non-<code>null</code> text to use as
		///                     the replacement </param>
		/// <param name="start">    the start position </param>
		/// <param name="end">      the end position </param>
		/// <seealso cref=       java.awt.TextArea#insert
		/// @since     JDK1.1 </seealso>
		public virtual void ReplaceRange(String str, int start, int end)
		{
			ReplaceText(str, start, end);
		}

		/// @deprecated As of JDK version 1.1,
		/// replaced by <code>replaceRange(String, int, int)</code>. 
		[Obsolete("As of JDK version 1.1,")]
		public virtual void ReplaceText(String str, int start, int end)
		{
			lock (this)
			{
				TextAreaPeer peer = (TextAreaPeer)this.Peer_Renamed;
				if (peer != null)
				{
					peer.ReplaceRange(str, start, end);
				}
				else
				{
					Text_Renamed = Text_Renamed.Substring(0, start) + str + Text_Renamed.Substring(end);
				}
			}
		}

		/// <summary>
		/// Returns the number of rows in the text area. </summary>
		/// <returns>    the number of rows in the text area </returns>
		/// <seealso cref=       #setRows(int) </seealso>
		/// <seealso cref=       #getColumns()
		/// @since     JDK1 </seealso>
		public virtual int Rows
		{
			get
			{
				return Rows_Renamed;
			}
			set
			{
				int oldVal = this.Rows_Renamed;
				if (value < 0)
				{
					throw new IllegalArgumentException("rows less than zero.");
				}
				if (value != oldVal)
				{
					this.Rows_Renamed = value;
					Invalidate();
				}
			}
		}


		/// <summary>
		/// Returns the number of columns in this text area. </summary>
		/// <returns>    the number of columns in the text area </returns>
		/// <seealso cref=       #setColumns(int) </seealso>
		/// <seealso cref=       #getRows() </seealso>
		public virtual int Columns
		{
			get
			{
				return Columns_Renamed;
			}
			set
			{
				int oldVal = this.Columns_Renamed;
				if (value < 0)
				{
					throw new IllegalArgumentException("columns less than zero.");
				}
				if (value != oldVal)
				{
					this.Columns_Renamed = value;
					Invalidate();
				}
			}
		}


		/// <summary>
		/// Returns an enumerated value that indicates which scroll bars
		/// the text area uses.
		/// <para>
		/// The <code>TextArea</code> class defines four integer constants
		/// that are used to specify which scroll bars are available.
		/// <code>TextArea</code> has one constructor that gives the
		/// application discretion over scroll bars.
		/// 
		/// </para>
		/// </summary>
		/// <returns>     an integer that indicates which scroll bars are used </returns>
		/// <seealso cref=        java.awt.TextArea#SCROLLBARS_BOTH </seealso>
		/// <seealso cref=        java.awt.TextArea#SCROLLBARS_VERTICAL_ONLY </seealso>
		/// <seealso cref=        java.awt.TextArea#SCROLLBARS_HORIZONTAL_ONLY </seealso>
		/// <seealso cref=        java.awt.TextArea#SCROLLBARS_NONE </seealso>
		/// <seealso cref=        java.awt.TextArea#TextArea(java.lang.String, int, int, int)
		/// @since      JDK1.1 </seealso>
		public virtual int ScrollbarVisibility
		{
			get
			{
				return ScrollbarVisibility_Renamed;
			}
		}


		/// <summary>
		/// Determines the preferred size of a text area with the specified
		/// number of rows and columns. </summary>
		/// <param name="rows">   the number of rows </param>
		/// <param name="columns">   the number of columns </param>
		/// <returns>    the preferred dimensions required to display
		///                       the text area with the specified
		///                       number of rows and columns </returns>
		/// <seealso cref=       java.awt.Component#getPreferredSize
		/// @since     JDK1.1 </seealso>
		public virtual Dimension GetPreferredSize(int rows, int columns)
		{
			return PreferredSize(rows, columns);
		}

		/// @deprecated As of JDK version 1.1,
		/// replaced by <code>getPreferredSize(int, int)</code>. 
		[Obsolete("As of JDK version 1.1,")]
		public virtual Dimension PreferredSize(int rows, int columns)
		{
			lock (TreeLock)
			{
				TextAreaPeer peer = (TextAreaPeer)this.Peer_Renamed;
				return (peer != null) ? peer.GetPreferredSize(rows, columns) : base.PreferredSize();
			}
		}

		/// <summary>
		/// Determines the preferred size of this text area. </summary>
		/// <returns>    the preferred dimensions needed for this text area </returns>
		/// <seealso cref=       java.awt.Component#getPreferredSize
		/// @since     JDK1.1 </seealso>
		public override Dimension PreferredSize
		{
			get
			{
				return PreferredSize();
			}
		}

		/// @deprecated As of JDK version 1.1,
		/// replaced by <code>getPreferredSize()</code>. 
		[Obsolete("As of JDK version 1.1,")]
		public override Dimension PreferredSize()
		{
			lock (TreeLock)
			{
				return ((Rows_Renamed > 0) && (Columns_Renamed > 0)) ? PreferredSize(Rows_Renamed, Columns_Renamed) : base.PreferredSize();
			}
		}

		/// <summary>
		/// Determines the minimum size of a text area with the specified
		/// number of rows and columns. </summary>
		/// <param name="rows">   the number of rows </param>
		/// <param name="columns">   the number of columns </param>
		/// <returns>    the minimum dimensions required to display
		///                       the text area with the specified
		///                       number of rows and columns </returns>
		/// <seealso cref=       java.awt.Component#getMinimumSize
		/// @since     JDK1.1 </seealso>
		public virtual Dimension GetMinimumSize(int rows, int columns)
		{
			return MinimumSize(rows, columns);
		}

		/// @deprecated As of JDK version 1.1,
		/// replaced by <code>getMinimumSize(int, int)</code>. 
		[Obsolete("As of JDK version 1.1,")]
		public virtual Dimension MinimumSize(int rows, int columns)
		{
			lock (TreeLock)
			{
				TextAreaPeer peer = (TextAreaPeer)this.Peer_Renamed;
				return (peer != null) ? peer.GetMinimumSize(rows, columns) : base.MinimumSize();
			}
		}

		/// <summary>
		/// Determines the minimum size of this text area. </summary>
		/// <returns>    the preferred dimensions needed for this text area </returns>
		/// <seealso cref=       java.awt.Component#getPreferredSize
		/// @since     JDK1.1 </seealso>
		public override Dimension MinimumSize
		{
			get
			{
				return MinimumSize();
			}
		}

		/// @deprecated As of JDK version 1.1,
		/// replaced by <code>getMinimumSize()</code>. 
		[Obsolete("As of JDK version 1.1,")]
		public override Dimension MinimumSize()
		{
			lock (TreeLock)
			{
				return ((Rows_Renamed > 0) && (Columns_Renamed > 0)) ? MinimumSize(Rows_Renamed, Columns_Renamed) : base.MinimumSize();
			}
		}

		/// <summary>
		/// Returns a string representing the state of this <code>TextArea</code>.
		/// This method is intended to be used only for debugging purposes, and the
		/// content and format of the returned string may vary between
		/// implementations. The returned string may be empty but may not be
		/// <code>null</code>.
		/// </summary>
		/// <returns>      the parameter string of this text area </returns>
		protected internal override String ParamString()
		{
			String sbVisStr;
			switch (ScrollbarVisibility_Renamed)
			{
				case SCROLLBARS_BOTH:
					sbVisStr = "both";
					break;
				case SCROLLBARS_VERTICAL_ONLY:
					sbVisStr = "vertical-only";
					break;
				case SCROLLBARS_HORIZONTAL_ONLY:
					sbVisStr = "horizontal-only";
					break;
				case SCROLLBARS_NONE:
					sbVisStr = "none";
					break;
				default:
					sbVisStr = "invalid display policy";
				break;
			}

			return base.ParamString() + ",rows=" + Rows_Renamed + ",columns=" + Columns_Renamed + ",scrollbarVisibility=" + sbVisStr;
		}


		/*
		 * Serialization support.
		 */
		/// <summary>
		/// The textArea Serialized Data Version.
		/// 
		/// @serial
		/// </summary>
		private int TextAreaSerializedDataVersion = 2;

		/// <summary>
		/// Read the ObjectInputStream. </summary>
		/// <exception cref="HeadlessException"> if
		/// <code>GraphicsEnvironment.isHeadless()</code> returns
		/// <code>true</code> </exception>
		/// <seealso cref= java.awt.GraphicsEnvironment#isHeadless </seealso>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: private void readObject(java.io.ObjectInputStream s) throws ClassNotFoundException, java.io.IOException, HeadlessException
		private void ReadObject(ObjectInputStream s)
		{
			// HeadlessException will be thrown by TextComponent's readObject
			s.DefaultReadObject();

			// Make sure the state we just read in for columns, rows,
			// and scrollbarVisibility has legal values
			if (Columns_Renamed < 0)
			{
				Columns_Renamed = 0;
			}
			if (Rows_Renamed < 0)
			{
				Rows_Renamed = 0;
			}

			if ((ScrollbarVisibility_Renamed < SCROLLBARS_BOTH) || (ScrollbarVisibility_Renamed > SCROLLBARS_NONE))
			{
				this.ScrollbarVisibility_Renamed = SCROLLBARS_BOTH;
			}

			if (TextAreaSerializedDataVersion < 2)
			{
				SetFocusTraversalKeys(KeyboardFocusManager.FORWARD_TRAVERSAL_KEYS, ForwardTraversalKeys);
				SetFocusTraversalKeys(KeyboardFocusManager.BACKWARD_TRAVERSAL_KEYS, BackwardTraversalKeys);
			}
		}


	/////////////////
	// Accessibility support
	////////////////


		/// <summary>
		/// Returns the <code>AccessibleContext</code> associated with
		/// this <code>TextArea</code>. For text areas, the
		/// <code>AccessibleContext</code> takes the form of an
		/// <code>AccessibleAWTTextArea</code>.
		/// A new <code>AccessibleAWTTextArea</code> instance is created if necessary.
		/// </summary>
		/// <returns> an <code>AccessibleAWTTextArea</code> that serves as the
		///         <code>AccessibleContext</code> of this <code>TextArea</code>
		/// @since 1.3 </returns>
		public override AccessibleContext AccessibleContext
		{
			get
			{
				if (AccessibleContext_Renamed == null)
				{
					AccessibleContext_Renamed = new AccessibleAWTTextArea(this);
				}
				return AccessibleContext_Renamed;
			}
		}

		/// <summary>
		/// This class implements accessibility support for the
		/// <code>TextArea</code> class.  It provides an implementation of the
		/// Java Accessibility API appropriate to text area user-interface elements.
		/// @since 1.3
		/// </summary>
		protected internal class AccessibleAWTTextArea : AccessibleAWTTextComponent
		{
			private readonly TextArea OuterInstance;

			public AccessibleAWTTextArea(TextArea outerInstance) : base(outerInstance)
			{
				this.OuterInstance = outerInstance;
			}

			/*
			 * JDK 1.3 serialVersionUID
			 */
			internal const long SerialVersionUID = 3472827823632144419L;

			/// <summary>
			/// Gets the state set of this object.
			/// </summary>
			/// <returns> an instance of AccessibleStateSet describing the states
			/// of the object </returns>
			/// <seealso cref= AccessibleStateSet </seealso>
			public override AccessibleStateSet AccessibleStateSet
			{
				get
				{
					AccessibleStateSet states = base.AccessibleStateSet;
					states.add(AccessibleState.MULTI_LINE);
					return states;
				}
			}
		}


	}

}
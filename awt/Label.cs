using System.Runtime.InteropServices;

/*
 * Copyright (c) 1995, 2014, Oracle and/or its affiliates. All rights reserved.
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
	/// A <code>Label</code> object is a component for placing text in a
	/// container. A label displays a single line of read-only text.
	/// The text can be changed by the application, but a user cannot edit it
	/// directly.
	/// <para>
	/// For example, the code&nbsp;.&nbsp;.&nbsp;.
	/// 
	/// <hr><blockquote><pre>
	/// setLayout(new FlowLayout(FlowLayout.CENTER, 10, 10));
	/// add(new Label("Hi There!"));
	/// add(new Label("Another Label"));
	/// </pre></blockquote><hr>
	/// </para>
	/// <para>
	/// produces the following labels:
	/// </para>
	/// <para>
	/// <img src="doc-files/Label-1.gif" alt="Two labels: 'Hi There!' and 'Another label'"
	/// style="float:center; margin: 7px 10px;">
	/// 
	/// @author      Sami Shaio
	/// @since       JDK1.0
	/// </para>
	/// </summary>
	public class Label : Component, Accessible
	{

		static Label()
		{
			/* ensure that the necessary native libraries are loaded */
			Toolkit.LoadLibraries();
			if (!GraphicsEnvironment.Headless)
			{
				initIDs();
			}
		}

		/// <summary>
		/// Indicates that the label should be left justified.
		/// </summary>
		public const int LEFT = 0;

		/// <summary>
		/// Indicates that the label should be centered.
		/// </summary>
		public const int CENTER = 1;

		/// <summary>
		/// Indicates that the label should be right justified.
		/// @since   JDK1.0t.
		/// </summary>
		public const int RIGHT = 2;

		/// <summary>
		/// The text of this label.
		/// This text can be modified by the program
		/// but never by the user.
		/// 
		/// @serial </summary>
		/// <seealso cref= #getText() </seealso>
		/// <seealso cref= #setText(String) </seealso>
		internal String Text_Renamed;

		/// <summary>
		/// The label's alignment.  The default alignment is set
		/// to be left justified.
		/// 
		/// @serial </summary>
		/// <seealso cref= #getAlignment() </seealso>
		/// <seealso cref= #setAlignment(int) </seealso>
		internal int Alignment_Renamed = LEFT;

		private const String @base = "label";
		private static int NameCounter = 0;

		/*
		 * JDK 1.1 serialVersionUID
		 */
		 private const long SerialVersionUID = 3094126758329070636L;

		/// <summary>
		/// Constructs an empty label.
		/// The text of the label is the empty string <code>""</code>. </summary>
		/// <exception cref="HeadlessException"> if GraphicsEnvironment.isHeadless()
		/// returns true. </exception>
		/// <seealso cref= java.awt.GraphicsEnvironment#isHeadless </seealso>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public Label() throws HeadlessException
		public Label() : this("", LEFT)
		{
		}

		/// <summary>
		/// Constructs a new label with the specified string of text,
		/// left justified. </summary>
		/// <param name="text"> the string that the label presents.
		///        A <code>null</code> value
		///        will be accepted without causing a NullPointerException
		///        to be thrown. </param>
		/// <exception cref="HeadlessException"> if GraphicsEnvironment.isHeadless()
		/// returns true. </exception>
		/// <seealso cref= java.awt.GraphicsEnvironment#isHeadless </seealso>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public Label(String text) throws HeadlessException
		public Label(String text) : this(text, LEFT)
		{
		}

		/// <summary>
		/// Constructs a new label that presents the specified string of
		/// text with the specified alignment.
		/// Possible values for <code>alignment</code> are <code>Label.LEFT</code>,
		/// <code>Label.RIGHT</code>, and <code>Label.CENTER</code>. </summary>
		/// <param name="text"> the string that the label presents.
		///        A <code>null</code> value
		///        will be accepted without causing a NullPointerException
		///        to be thrown. </param>
		/// <param name="alignment">   the alignment value. </param>
		/// <exception cref="HeadlessException"> if GraphicsEnvironment.isHeadless()
		/// returns true. </exception>
		/// <seealso cref= java.awt.GraphicsEnvironment#isHeadless </seealso>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public Label(String text, int alignment) throws HeadlessException
		public Label(String text, int alignment)
		{
			GraphicsEnvironment.CheckHeadless();
			this.Text_Renamed = text;
			Alignment = alignment;
		}

		/// <summary>
		/// Read a label from an object input stream. </summary>
		/// <exception cref="HeadlessException"> if
		/// <code>GraphicsEnvironment.isHeadless()</code> returns
		/// <code>true</code>
		/// @serial
		/// @since 1.4 </exception>
		/// <seealso cref= java.awt.GraphicsEnvironment#isHeadless </seealso>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: private void readObject(java.io.ObjectInputStream s) throws ClassNotFoundException, java.io.IOException, HeadlessException
		private void ReadObject(ObjectInputStream s)
		{
			GraphicsEnvironment.CheckHeadless();
			s.DefaultReadObject();
		}

		/// <summary>
		/// Construct a name for this component.  Called by getName() when the
		/// name is <code>null</code>.
		/// </summary>
		internal override String ConstructComponentName()
		{
			lock (typeof(Label))
			{
				return @base + NameCounter++;
			}
		}

		/// <summary>
		/// Creates the peer for this label.  The peer allows us to
		/// modify the appearance of the label without changing its
		/// functionality.
		/// </summary>
		public override void AddNotify()
		{
			lock (TreeLock)
			{
				if (Peer_Renamed == null)
				{
					Peer_Renamed = Toolkit.CreateLabel(this);
				}
				base.AddNotify();
			}
		}

		/// <summary>
		/// Gets the current alignment of this label. Possible values are
		/// <code>Label.LEFT</code>, <code>Label.RIGHT</code>, and
		/// <code>Label.CENTER</code>. </summary>
		/// <seealso cref=        java.awt.Label#setAlignment </seealso>
		public virtual int Alignment
		{
			get
			{
				return Alignment_Renamed;
			}
			set
			{
				lock (this)
				{
					switch (value)
					{
					  case LEFT:
					  case CENTER:
					  case RIGHT:
						this.Alignment_Renamed = value;
						LabelPeer peer = (LabelPeer)this.Peer_Renamed;
						if (peer != null)
						{
							peer.Alignment = value;
						}
						return;
					}
					throw new IllegalArgumentException("improper alignment: " + value);
				}
			}
		}


		/// <summary>
		/// Gets the text of this label. </summary>
		/// <returns>     the text of this label, or <code>null</code> if
		///             the text has been set to <code>null</code>. </returns>
		/// <seealso cref=        java.awt.Label#setText </seealso>
		public virtual String Text
		{
			get
			{
				return Text_Renamed;
			}
			set
			{
				bool testvalid = false;
				lock (this)
				{
					if (value != this.Text_Renamed && (this.Text_Renamed == null || !this.Text_Renamed.Equals(value)))
					{
						this.Text_Renamed = value;
						LabelPeer peer = (LabelPeer)this.Peer_Renamed;
						if (peer != null)
						{
							peer.Text = value;
						}
						testvalid = true;
					}
				}
    
				// This could change the preferred size of the Component.
				if (testvalid)
				{
					InvalidateIfValid();
				}
			}
		}


		/// <summary>
		/// Returns a string representing the state of this <code>Label</code>.
		/// This method is intended to be used only for debugging purposes, and the
		/// content and format of the returned string may vary between
		/// implementations. The returned string may be empty but may not be
		/// <code>null</code>.
		/// </summary>
		/// <returns>     the parameter string of this label </returns>
		protected internal override String ParamString()
		{
			String align = "";
			switch (Alignment_Renamed)
			{
				case LEFT:
					align = "left";
					break;
				case CENTER:
					align = "center";
					break;
				case RIGHT:
					align = "right";
					break;
			}
			return base.ParamString() + ",align=" + align + ",text=" + Text_Renamed;
		}

		/// <summary>
		/// Initialize JNI field and method IDs
		/// </summary>
//JAVA TO C# CONVERTER TODO TASK: Replace 'unknown' with the appropriate dll name:
		[DllImport("unknown")]
		private static extern void initIDs();


	/////////////////
	// Accessibility support
	////////////////


		/// <summary>
		/// Gets the AccessibleContext associated with this Label.
		/// For labels, the AccessibleContext takes the form of an
		/// AccessibleAWTLabel.
		/// A new AccessibleAWTLabel instance is created if necessary.
		/// </summary>
		/// <returns> an AccessibleAWTLabel that serves as the
		///         AccessibleContext of this Label
		/// @since 1.3 </returns>
		public override AccessibleContext AccessibleContext
		{
			get
			{
				if (AccessibleContext_Renamed == null)
				{
					AccessibleContext_Renamed = new AccessibleAWTLabel(this);
				}
				return AccessibleContext_Renamed;
			}
		}

		/// <summary>
		/// This class implements accessibility support for the
		/// <code>Label</code> class.  It provides an implementation of the
		/// Java Accessibility API appropriate to label user-interface elements.
		/// @since 1.3
		/// </summary>
		protected internal class AccessibleAWTLabel : AccessibleAWTComponent
		{
			private readonly Label OuterInstance;

			/*
			 * JDK 1.3 serialVersionUID
			 */
			internal const long SerialVersionUID = -3568967560160480438L;

			public AccessibleAWTLabel(Label outerInstance) : base(outerInstance)
			{
				this.OuterInstance = outerInstance;
			}

			/// <summary>
			/// Get the accessible name of this object.
			/// </summary>
			/// <returns> the localized name of the object -- can be null if this
			/// object does not have a name </returns>
			/// <seealso cref= AccessibleContext#setAccessibleName </seealso>
			public override String AccessibleName
			{
				get
				{
					if (accessibleName != null)
					{
						return accessibleName;
					}
					else
					{
						if (outerInstance.Text == null)
						{
							return base.AccessibleName;
						}
						else
						{
							return outerInstance.Text;
						}
					}
				}
			}

			/// <summary>
			/// Get the role of this object.
			/// </summary>
			/// <returns> an instance of AccessibleRole describing the role of the object </returns>
			/// <seealso cref= AccessibleRole </seealso>
			public override AccessibleRole AccessibleRole
			{
				get
				{
					return AccessibleRole.LABEL;
				}
			}

		} // inner class AccessibleAWTLabel

	}

}
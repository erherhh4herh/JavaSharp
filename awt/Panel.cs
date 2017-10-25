/*
 * Copyright (c) 1995, 2007, Oracle and/or its affiliates. All rights reserved.
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
	/// <code>Panel</code> is the simplest container class. A panel
	/// provides space in which an application can attach any other
	/// component, including other panels.
	/// <para>
	/// The default layout manager for a panel is the
	/// <code>FlowLayout</code> layout manager.
	/// 
	/// @author      Sami Shaio
	/// </para>
	/// </summary>
	/// <seealso cref=     java.awt.FlowLayout
	/// @since   JDK1.0 </seealso>
	public class Panel : Container, Accessible
	{
		private const String @base = "panel";
		private static int NameCounter = 0;

		/*
		 * JDK 1.1 serialVersionUID
		 */
		 private const long SerialVersionUID = -2728009084054400034L;

		/// <summary>
		/// Creates a new panel using the default layout manager.
		/// The default layout manager for all panels is the
		/// <code>FlowLayout</code> class.
		/// </summary>
		public Panel() : this(new FlowLayout())
		{
		}

		/// <summary>
		/// Creates a new panel with the specified layout manager. </summary>
		/// <param name="layout"> the layout manager for this panel.
		/// @since JDK1.1 </param>
		public Panel(LayoutManager layout)
		{
			Layout = layout;
		}

		/// <summary>
		/// Construct a name for this component.  Called by getName() when the
		/// name is null.
		/// </summary>
		internal override String ConstructComponentName()
		{
			lock (typeof(Panel))
			{
				return @base + NameCounter++;
			}
		}

		/// <summary>
		/// Creates the Panel's peer.  The peer allows you to modify the
		/// appearance of the panel without changing its functionality.
		/// </summary>

		public override void AddNotify()
		{
			lock (TreeLock)
			{
				if (Peer_Renamed == null)
				{
					Peer_Renamed = Toolkit.CreatePanel(this);
				}
				base.AddNotify();
			}
		}

	/////////////////
	// Accessibility support
	////////////////

		/// <summary>
		/// Gets the AccessibleContext associated with this Panel.
		/// For panels, the AccessibleContext takes the form of an
		/// AccessibleAWTPanel.
		/// A new AccessibleAWTPanel instance is created if necessary.
		/// </summary>
		/// <returns> an AccessibleAWTPanel that serves as the
		///         AccessibleContext of this Panel
		/// @since 1.3 </returns>
		public override AccessibleContext AccessibleContext
		{
			get
			{
				if (AccessibleContext_Renamed == null)
				{
					AccessibleContext_Renamed = new AccessibleAWTPanel(this);
				}
				return AccessibleContext_Renamed;
			}
		}

		/// <summary>
		/// This class implements accessibility support for the
		/// <code>Panel</code> class.  It provides an implementation of the
		/// Java Accessibility API appropriate to panel user-interface elements.
		/// @since 1.3
		/// </summary>
		protected internal class AccessibleAWTPanel : AccessibleAWTContainer
		{
			private readonly Panel OuterInstance;

			public AccessibleAWTPanel(Panel outerInstance) : base(outerInstance)
			{
				this.OuterInstance = outerInstance;
			}


			internal const long SerialVersionUID = -6409552226660031050L;

			/// <summary>
			/// Get the role of this object.
			/// </summary>
			/// <returns> an instance of AccessibleRole describing the role of the
			/// object </returns>
			public override AccessibleRole AccessibleRole
			{
				get
				{
					return AccessibleRole.PANEL;
				}
			}
		}

	}

}
/*
 * Copyright (c) 2000, 2013, Oracle and/or its affiliates. All rights reserved.
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
	/// A FocusTraversalPolicy that determines traversal order based on the order
	/// of child Components in a Container. From a particular focus cycle root, the
	/// policy makes a pre-order traversal of the Component hierarchy, and traverses
	/// a Container's children according to the ordering of the array returned by
	/// <code>Container.getComponents()</code>. Portions of the hierarchy that are
	/// not visible and displayable will not be searched.
	/// <para>
	/// If client code has explicitly set the focusability of a Component by either
	/// overriding <code>Component.isFocusTraversable()</code> or
	/// <code>Component.isFocusable()</code>, or by calling
	/// <code>Component.setFocusable()</code>, then a DefaultFocusTraversalPolicy
	/// behaves exactly like a ContainerOrderFocusTraversalPolicy. If, however, the
	/// Component is relying on default focusability, then a
	/// DefaultFocusTraversalPolicy will reject all Components with non-focusable
	/// peers. This is the default FocusTraversalPolicy for all AWT Containers.
	/// </para>
	/// <para>
	/// The focusability of a peer is implementation-dependent. Sun recommends that
	/// all implementations for a particular native platform construct peers with
	/// the same focusability. The recommendations for Windows and Unix are that
	/// Canvases, Labels, Panels, Scrollbars, ScrollPanes, Windows, and lightweight
	/// Components have non-focusable peers, and all other Components have focusable
	/// peers. These recommendations are used in the Sun AWT implementations. Note
	/// that the focusability of a Component's peer is different from, and does not
	/// impact, the focusability of the Component itself.
	/// </para>
	/// <para>
	/// Please see
	/// <a href="https://docs.oracle.com/javase/tutorial/uiswing/misc/focus.html">
	/// How to Use the Focus Subsystem</a>,
	/// a section in <em>The Java Tutorial</em>, and the
	/// <a href="../../java/awt/doc-files/FocusSpec.html">Focus Specification</a>
	/// for more information.
	/// 
	/// @author David Mendenhall
	/// 
	/// </para>
	/// </summary>
	/// <seealso cref= Container#getComponents </seealso>
	/// <seealso cref= Component#isFocusable </seealso>
	/// <seealso cref= Component#setFocusable
	/// @since 1.4 </seealso>
	public class DefaultFocusTraversalPolicy : ContainerOrderFocusTraversalPolicy
	{
		/*
		 * serialVersionUID
		 */
		private const long SerialVersionUID = 8876966522510157497L;

		/// <summary>
		/// Determines whether a Component is an acceptable choice as the new
		/// focus owner. The Component must be visible, displayable, and enabled
		/// to be accepted. If client code has explicitly set the focusability
		/// of the Component by either overriding
		/// <code>Component.isFocusTraversable()</code> or
		/// <code>Component.isFocusable()</code>, or by calling
		/// <code>Component.setFocusable()</code>, then the Component will be
		/// accepted if and only if it is focusable. If, however, the Component is
		/// relying on default focusability, then all Canvases, Labels, Panels,
		/// Scrollbars, ScrollPanes, Windows, and lightweight Components will be
		/// rejected.
		/// </summary>
		/// <param name="aComponent"> the Component whose fitness as a focus owner is to
		///        be tested </param>
		/// <returns> <code>true</code> if aComponent meets the above requirements;
		///         <code>false</code> otherwise </returns>
		protected internal override bool Accept(Component aComponent)
		{
			if (!(aComponent.Visible && aComponent.Displayable && aComponent.Enabled))
			{
				return false;
			}

			// Verify that the Component is recursively enabled. Disabling a
			// heavyweight Container disables its children, whereas disabling
			// a lightweight Container does not.
			if (!(aComponent is Window))
			{
				for (Container enableTest = aComponent.Parent; enableTest != null; enableTest = enableTest.Parent)
				{
					if (!(enableTest.Enabled || enableTest.Lightweight))
					{
						return false;
					}
					if (enableTest is Window)
					{
						break;
					}
				}
			}

			bool focusable = aComponent.Focusable;
			if (aComponent.FocusTraversableOverridden)
			{
				return focusable;
			}

			ComponentPeer peer = aComponent.Peer;
			return (peer != null && peer.Focusable);
		}
	}

}
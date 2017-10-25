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
	/// A FocusTraversalPolicy defines the order in which Components with a
	/// particular focus cycle root are traversed. Instances can apply the policy to
	/// arbitrary focus cycle roots, allowing themselves to be shared across
	/// Containers. They do not need to be reinitialized when the focus cycle roots
	/// of a Component hierarchy change.
	/// <para>
	/// The core responsibility of a FocusTraversalPolicy is to provide algorithms
	/// determining the next and previous Components to focus when traversing
	/// forward or backward in a UI. Each FocusTraversalPolicy must also provide
	/// algorithms for determining the first, last, and default Components in a
	/// traversal cycle. First and last Components are used when normal forward and
	/// backward traversal, respectively, wraps. The default Component is the first
	/// to receive focus when traversing down into a new focus traversal cycle.
	/// A FocusTraversalPolicy can optionally provide an algorithm for determining
	/// a Window's initial Component. The initial Component is the first to receive
	/// focus when a Window is first made visible.
	/// </para>
	/// <para>
	/// FocusTraversalPolicy takes into account <a
	/// href="doc-files/FocusSpec.html#FocusTraversalPolicyProviders">focus traversal
	/// policy providers</a>.  When searching for first/last/next/previous Component,
	/// if a focus traversal policy provider is encountered, its focus traversal
	/// policy is used to perform the search operation.
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
	/// <seealso cref= Container#setFocusTraversalPolicy </seealso>
	/// <seealso cref= Container#getFocusTraversalPolicy </seealso>
	/// <seealso cref= Container#setFocusCycleRoot </seealso>
	/// <seealso cref= Container#isFocusCycleRoot </seealso>
	/// <seealso cref= Container#setFocusTraversalPolicyProvider </seealso>
	/// <seealso cref= Container#isFocusTraversalPolicyProvider </seealso>
	/// <seealso cref= KeyboardFocusManager#setDefaultFocusTraversalPolicy </seealso>
	/// <seealso cref= KeyboardFocusManager#getDefaultFocusTraversalPolicy
	/// @since 1.4 </seealso>
	public abstract class FocusTraversalPolicy
	{

		/// <summary>
		/// Returns the Component that should receive the focus after aComponent.
		/// aContainer must be a focus cycle root of aComponent or a focus traversal
		/// policy provider.
		/// </summary>
		/// <param name="aContainer"> a focus cycle root of aComponent or focus traversal
		///        policy provider </param>
		/// <param name="aComponent"> a (possibly indirect) child of aContainer, or
		///        aContainer itself </param>
		/// <returns> the Component that should receive the focus after aComponent, or
		///         null if no suitable Component can be found </returns>
		/// <exception cref="IllegalArgumentException"> if aContainer is not a focus cycle
		///         root of aComponent or a focus traversal policy provider, or if
		///         either aContainer or aComponent is null </exception>
		public abstract Component GetComponentAfter(Container aContainer, Component aComponent);

		/// <summary>
		/// Returns the Component that should receive the focus before aComponent.
		/// aContainer must be a focus cycle root of aComponent or a focus traversal
		/// policy provider.
		/// </summary>
		/// <param name="aContainer"> a focus cycle root of aComponent or focus traversal
		///        policy provider </param>
		/// <param name="aComponent"> a (possibly indirect) child of aContainer, or
		///        aContainer itself </param>
		/// <returns> the Component that should receive the focus before aComponent,
		///         or null if no suitable Component can be found </returns>
		/// <exception cref="IllegalArgumentException"> if aContainer is not a focus cycle
		///         root of aComponent or a focus traversal policy provider, or if
		///         either aContainer or aComponent is null </exception>
		public abstract Component GetComponentBefore(Container aContainer, Component aComponent);

		/// <summary>
		/// Returns the first Component in the traversal cycle. This method is used
		/// to determine the next Component to focus when traversal wraps in the
		/// forward direction.
		/// </summary>
		/// <param name="aContainer"> the focus cycle root or focus traversal policy provider
		///        whose first Component is to be returned </param>
		/// <returns> the first Component in the traversal cycle of aContainer,
		///         or null if no suitable Component can be found </returns>
		/// <exception cref="IllegalArgumentException"> if aContainer is null </exception>
		public abstract Component GetFirstComponent(Container aContainer);

		/// <summary>
		/// Returns the last Component in the traversal cycle. This method is used
		/// to determine the next Component to focus when traversal wraps in the
		/// reverse direction.
		/// </summary>
		/// <param name="aContainer"> the focus cycle root or focus traversal policy
		///        provider whose last Component is to be returned </param>
		/// <returns> the last Component in the traversal cycle of aContainer,
		///         or null if no suitable Component can be found </returns>
		/// <exception cref="IllegalArgumentException"> if aContainer is null </exception>
		public abstract Component GetLastComponent(Container aContainer);

		/// <summary>
		/// Returns the default Component to focus. This Component will be the first
		/// to receive focus when traversing down into a new focus traversal cycle
		/// rooted at aContainer.
		/// </summary>
		/// <param name="aContainer"> the focus cycle root or focus traversal policy
		///        provider whose default Component is to be returned </param>
		/// <returns> the default Component in the traversal cycle of aContainer,
		///         or null if no suitable Component can be found </returns>
		/// <exception cref="IllegalArgumentException"> if aContainer is null </exception>
		public abstract Component GetDefaultComponent(Container aContainer);

		/// <summary>
		/// Returns the Component that should receive the focus when a Window is
		/// made visible for the first time. Once the Window has been made visible
		/// by a call to <code>show()</code> or <code>setVisible(true)</code>, the
		/// initial Component will not be used again. Instead, if the Window loses
		/// and subsequently regains focus, or is made invisible or undisplayable
		/// and subsequently made visible and displayable, the Window's most
		/// recently focused Component will become the focus owner. The default
		/// implementation of this method returns the default Component.
		/// </summary>
		/// <param name="window"> the Window whose initial Component is to be returned </param>
		/// <returns> the Component that should receive the focus when window is made
		///         visible for the first time, or null if no suitable Component can
		///         be found </returns>
		/// <seealso cref= #getDefaultComponent </seealso>
		/// <seealso cref= Window#getMostRecentFocusOwner </seealso>
		/// <exception cref="IllegalArgumentException"> if window is null </exception>
		public virtual Component GetInitialComponent(Window window)
		{
			if (window == null)
			{
				throw new IllegalArgumentException("window cannot be equal to null.");
			}
			Component def = GetDefaultComponent(window);
			if (def == null && window.FocusableWindow)
			{
				def = window;
			}
			return def;
		}
	}

}
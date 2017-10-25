/*
 * Copyright (c) 1995, 2005, Oracle and/or its affiliates. All rights reserved.
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
	/// Defines the interface for classes that know how to lay out
	/// <code>Container</code>s.
	/// <para>
	/// Swing's painting architecture assumes the children of a
	/// <code>JComponent</code> do not overlap.  If a
	/// <code>JComponent</code>'s <code>LayoutManager</code> allows
	/// children to overlap, the <code>JComponent</code> must override
	/// <code>isOptimizedDrawingEnabled</code> to return false.
	/// 
	/// </para>
	/// </summary>
	/// <seealso cref= Container </seealso>
	/// <seealso cref= javax.swing.JComponent#isOptimizedDrawingEnabled
	/// 
	/// @author      Sami Shaio
	/// @author      Arthur van Hoff </seealso>
	public interface LayoutManager
	{
		/// <summary>
		/// If the layout manager uses a per-component string,
		/// adds the component <code>comp</code> to the layout,
		/// associating it
		/// with the string specified by <code>name</code>.
		/// </summary>
		/// <param name="name"> the string to be associated with the component </param>
		/// <param name="comp"> the component to be added </param>
		void AddLayoutComponent(String name, Component comp);

		/// <summary>
		/// Removes the specified component from the layout. </summary>
		/// <param name="comp"> the component to be removed </param>
		void RemoveLayoutComponent(Component comp);

		/// <summary>
		/// Calculates the preferred size dimensions for the specified
		/// container, given the components it contains. </summary>
		/// <param name="parent"> the container to be laid out
		/// </param>
		/// <seealso cref= #minimumLayoutSize </seealso>
		Dimension PreferredLayoutSize(Container parent);

		/// <summary>
		/// Calculates the minimum size dimensions for the specified
		/// container, given the components it contains. </summary>
		/// <param name="parent"> the component to be laid out </param>
		/// <seealso cref= #preferredLayoutSize </seealso>
		Dimension MinimumLayoutSize(Container parent);

		/// <summary>
		/// Lays out the specified container. </summary>
		/// <param name="parent"> the container to be laid out </param>
		void LayoutContainer(Container parent);
	}

}
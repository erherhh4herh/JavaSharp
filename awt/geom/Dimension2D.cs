/*
 * Copyright (c) 1997, 2011, Oracle and/or its affiliates. All rights reserved.
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

namespace java.awt.geom
{

	/// <summary>
	/// The <code>Dimension2D</code> class is to encapsulate a width
	/// and a height dimension.
	/// <para>
	/// This class is only the abstract superclass for all objects that
	/// store a 2D dimension.
	/// The actual storage representation of the sizes is left to
	/// the subclass.
	/// 
	/// @author      Jim Graham
	/// @since 1.2
	/// </para>
	/// </summary>
	public abstract class Dimension2D : Cloneable
	{

		/// <summary>
		/// This is an abstract class that cannot be instantiated directly.
		/// Type-specific implementation subclasses are available for
		/// instantiation and provide a number of formats for storing
		/// the information necessary to satisfy the various accessor
		/// methods below.
		/// </summary>
		/// <seealso cref= java.awt.Dimension
		/// @since 1.2 </seealso>
		protected internal Dimension2D()
		{
		}

		/// <summary>
		/// Returns the width of this <code>Dimension</code> in double
		/// precision. </summary>
		/// <returns> the width of this <code>Dimension</code>.
		/// @since 1.2 </returns>
		public abstract double Width {get;}

		/// <summary>
		/// Returns the height of this <code>Dimension</code> in double
		/// precision. </summary>
		/// <returns> the height of this <code>Dimension</code>.
		/// @since 1.2 </returns>
		public abstract double Height {get;}

		/// <summary>
		/// Sets the size of this <code>Dimension</code> object to the
		/// specified width and height.
		/// This method is included for completeness, to parallel the
		/// <seealso cref="java.awt.Component#getSize getSize"/> method of
		/// <seealso cref="java.awt.Component"/>. </summary>
		/// <param name="width">  the new width for the <code>Dimension</code>
		/// object </param>
		/// <param name="height">  the new height for the <code>Dimension</code>
		/// object
		/// @since 1.2 </param>
		public abstract void SetSize(double width, double height);

		/// <summary>
		/// Sets the size of this <code>Dimension2D</code> object to
		/// match the specified size.
		/// This method is included for completeness, to parallel the
		/// <code>getSize</code> method of <code>Component</code>. </summary>
		/// <param name="d">  the new size for the <code>Dimension2D</code>
		/// object
		/// @since 1.2 </param>
		public virtual Dimension2D Size
		{
			set
			{
				SetSize(value.Width, value.Height);
			}
		}

		/// <summary>
		/// Creates a new object of the same class as this object.
		/// </summary>
		/// <returns>     a clone of this instance. </returns>
		/// <exception cref="OutOfMemoryError">            if there is not enough memory. </exception>
		/// <seealso cref=        java.lang.Cloneable
		/// @since      1.2 </seealso>
		public virtual Object Clone()
		{
			try
			{
				return base.Clone();
			}
			catch (CloneNotSupportedException e)
			{
				// this shouldn't happen, since we are Cloneable
				throw new InternalError(e);
			}
		}
	}

}
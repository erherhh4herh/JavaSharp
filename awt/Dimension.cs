using System;
using System.Runtime.InteropServices;

/*
 * Copyright (c) 1995, 2008, Oracle and/or its affiliates. All rights reserved.
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
	/// The <code>Dimension</code> class encapsulates the width and
	/// height of a component (in integer precision) in a single object.
	/// The class is
	/// associated with certain properties of components. Several methods
	/// defined by the <code>Component</code> class and the
	/// <code>LayoutManager</code> interface return a
	/// <code>Dimension</code> object.
	/// <para>
	/// Normally the values of <code>width</code>
	/// and <code>height</code> are non-negative integers.
	/// The constructors that allow you to create a dimension do
	/// not prevent you from setting a negative value for these properties.
	/// If the value of <code>width</code> or <code>height</code> is
	/// negative, the behavior of some methods defined by other objects is
	/// undefined.
	/// 
	/// @author      Sami Shaio
	/// @author      Arthur van Hoff
	/// </para>
	/// </summary>
	/// <seealso cref=         java.awt.Component </seealso>
	/// <seealso cref=         java.awt.LayoutManager
	/// @since       1.0 </seealso>
	[Serializable]
	public class Dimension : Dimension2D
	{

		/// <summary>
		/// The width dimension; negative values can be used.
		/// 
		/// @serial </summary>
		/// <seealso cref= #getSize </seealso>
		/// <seealso cref= #setSize
		/// @since 1.0 </seealso>
		public int Width_Renamed;

		/// <summary>
		/// The height dimension; negative values can be used.
		/// 
		/// @serial </summary>
		/// <seealso cref= #getSize </seealso>
		/// <seealso cref= #setSize
		/// @since 1.0 </seealso>
		public int Height_Renamed;

		/*
		 * JDK 1.1 serialVersionUID
		 */
		 private const long SerialVersionUID = 4723952579491349524L;

		/// <summary>
		/// Initialize JNI field and method IDs
		/// </summary>
//JAVA TO C# CONVERTER TODO TASK: Replace 'unknown' with the appropriate dll name:
		[DllImport("unknown")]
		private static extern void initIDs();

		static Dimension()
		{
			/* ensure that the necessary native libraries are loaded */
			Toolkit.LoadLibraries();
			if (!GraphicsEnvironment.Headless)
			{
				initIDs();
			}
		}

		/// <summary>
		/// Creates an instance of <code>Dimension</code> with a width
		/// of zero and a height of zero.
		/// </summary>
		public Dimension() : this(0, 0)
		{
		}

		/// <summary>
		/// Creates an instance of <code>Dimension</code> whose width
		/// and height are the same as for the specified dimension.
		/// </summary>
		/// <param name="d">   the specified dimension for the
		///               <code>width</code> and
		///               <code>height</code> values </param>
		public Dimension(Dimension d) : this(d.Width_Renamed, d.Height_Renamed)
		{
		}

		/// <summary>
		/// Constructs a <code>Dimension</code> and initializes
		/// it to the specified width and specified height.
		/// </summary>
		/// <param name="width"> the specified width </param>
		/// <param name="height"> the specified height </param>
		public Dimension(int width, int height)
		{
			this.Width_Renamed = width;
			this.Height_Renamed = height;
		}

		/// <summary>
		/// {@inheritDoc}
		/// @since 1.2
		/// </summary>
		public override double Width
		{
			get
			{
				return Width_Renamed;
			}
		}

		/// <summary>
		/// {@inheritDoc}
		/// @since 1.2
		/// </summary>
		public override double Height
		{
			get
			{
				return Height_Renamed;
			}
		}

		/// <summary>
		/// Sets the size of this <code>Dimension</code> object to
		/// the specified width and height in double precision.
		/// Note that if <code>width</code> or <code>height</code>
		/// are larger than <code>Integer.MAX_VALUE</code>, they will
		/// be reset to <code>Integer.MAX_VALUE</code>.
		/// </summary>
		/// <param name="width">  the new width for the <code>Dimension</code> object </param>
		/// <param name="height"> the new height for the <code>Dimension</code> object
		/// @since 1.2 </param>
		public override void SetSize(double width, double height)
		{
			this.Width_Renamed = (int) System.Math.Ceiling(width);
			this.Height_Renamed = (int) System.Math.Ceiling(height);
		}

		/// <summary>
		/// Gets the size of this <code>Dimension</code> object.
		/// This method is included for completeness, to parallel the
		/// <code>getSize</code> method defined by <code>Component</code>.
		/// </summary>
		/// <returns>   the size of this dimension, a new instance of
		///           <code>Dimension</code> with the same width and height </returns>
		/// <seealso cref=      java.awt.Dimension#setSize </seealso>
		/// <seealso cref=      java.awt.Component#getSize
		/// @since    1.1 </seealso>
//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Transient public Dimension getSize()
		public virtual Dimension Size
		{
			get
			{
				return new Dimension(Width_Renamed, Height_Renamed);
			}
			set
			{
				SetSize(value.Width_Renamed, value.Height_Renamed);
			}
		}


		/// <summary>
		/// Sets the size of this <code>Dimension</code> object
		/// to the specified width and height.
		/// This method is included for completeness, to parallel the
		/// <code>setSize</code> method defined by <code>Component</code>.
		/// </summary>
		/// <param name="width">   the new width for this <code>Dimension</code> object </param>
		/// <param name="height">  the new height for this <code>Dimension</code> object </param>
		/// <seealso cref=      java.awt.Dimension#getSize </seealso>
		/// <seealso cref=      java.awt.Component#setSize
		/// @since    1.1 </seealso>
		public virtual void SetSize(int width, int height)
		{
			this.Width_Renamed = width;
			this.Height_Renamed = height;
		}

		/// <summary>
		/// Checks whether two dimension objects have equal values.
		/// </summary>
		public override bool Equals(Object obj)
		{
			if (obj is Dimension)
			{
				Dimension d = (Dimension)obj;
				return (Width_Renamed == d.Width_Renamed) && (Height_Renamed == d.Height_Renamed);
			}
			return false;
		}

		/// <summary>
		/// Returns the hash code for this <code>Dimension</code>.
		/// </summary>
		/// <returns>    a hash code for this <code>Dimension</code> </returns>
		public override int HashCode()
		{
			int sum = Width_Renamed + Height_Renamed;
			return sum * (sum + 1) / 2 + Width_Renamed;
		}

		/// <summary>
		/// Returns a string representation of the values of this
		/// <code>Dimension</code> object's <code>height</code> and
		/// <code>width</code> fields. This method is intended to be used only
		/// for debugging purposes, and the content and format of the returned
		/// string may vary between implementations. The returned string may be
		/// empty but may not be <code>null</code>.
		/// </summary>
		/// <returns>  a string representation of this <code>Dimension</code>
		///          object </returns>
		public override String ToString()
		{
//JAVA TO C# CONVERTER WARNING: The .NET Type.FullName property will not always yield results identical to the Java Class.getName method:
			return this.GetType().FullName + "[width=" + Width_Renamed + ",height=" + Height_Renamed + "]";
		}
	}

}
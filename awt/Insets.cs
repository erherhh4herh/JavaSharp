using System;
using System.Runtime.InteropServices;

/*
 * Copyright (c) 1995, 2011, Oracle and/or its affiliates. All rights reserved.
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
	/// An <code>Insets</code> object is a representation of the borders
	/// of a container. It specifies the space that a container must leave
	/// at each of its edges. The space can be a border, a blank space, or
	/// a title.
	/// 
	/// @author      Arthur van Hoff
	/// @author      Sami Shaio </summary>
	/// <seealso cref=         java.awt.LayoutManager </seealso>
	/// <seealso cref=         java.awt.Container
	/// @since       JDK1.0 </seealso>
	[Serializable]
	public class Insets : Cloneable
	{

		/// <summary>
		/// The inset from the top.
		/// This value is added to the Top of the rectangle
		/// to yield a new location for the Top.
		/// 
		/// @serial </summary>
		/// <seealso cref= #clone() </seealso>
		public int Top;

		/// <summary>
		/// The inset from the left.
		/// This value is added to the Left of the rectangle
		/// to yield a new location for the Left edge.
		/// 
		/// @serial </summary>
		/// <seealso cref= #clone() </seealso>
		public int Left;

		/// <summary>
		/// The inset from the bottom.
		/// This value is subtracted from the Bottom of the rectangle
		/// to yield a new location for the Bottom.
		/// 
		/// @serial </summary>
		/// <seealso cref= #clone() </seealso>
		public int Bottom;

		/// <summary>
		/// The inset from the right.
		/// This value is subtracted from the Right of the rectangle
		/// to yield a new location for the Right edge.
		/// 
		/// @serial </summary>
		/// <seealso cref= #clone() </seealso>
		public int Right;

		/*
		 * JDK 1.1 serialVersionUID
		 */
		private const long SerialVersionUID = -2272572637695466749L;

		static Insets()
		{
			/* ensure that the necessary native libraries are loaded */
			Toolkit.LoadLibraries();
			if (!GraphicsEnvironment.Headless)
			{
				initIDs();
			}
		}

		/// <summary>
		/// Creates and initializes a new <code>Insets</code> object with the
		/// specified top, left, bottom, and right insets. </summary>
		/// <param name="top">   the inset from the top. </param>
		/// <param name="left">   the inset from the left. </param>
		/// <param name="bottom">   the inset from the bottom. </param>
		/// <param name="right">   the inset from the right. </param>
		public Insets(int top, int left, int bottom, int right)
		{
			this.Top = top;
			this.Left = left;
			this.Bottom = bottom;
			this.Right = right;
		}

		/// <summary>
		/// Set top, left, bottom, and right to the specified values
		/// </summary>
		/// <param name="top">   the inset from the top. </param>
		/// <param name="left">   the inset from the left. </param>
		/// <param name="bottom">   the inset from the bottom. </param>
		/// <param name="right">   the inset from the right.
		/// @since 1.5 </param>
		public virtual void Set(int top, int left, int bottom, int right)
		{
			this.Top = top;
			this.Left = left;
			this.Bottom = bottom;
			this.Right = right;
		}

		/// <summary>
		/// Checks whether two insets objects are equal. Two instances
		/// of <code>Insets</code> are equal if the four integer values
		/// of the fields <code>top</code>, <code>left</code>,
		/// <code>bottom</code>, and <code>right</code> are all equal. </summary>
		/// <returns>      <code>true</code> if the two insets are equal;
		///                          otherwise <code>false</code>.
		/// @since       JDK1.1 </returns>
		public override bool Equals(Object obj)
		{
			if (obj is Insets)
			{
				Insets insets = (Insets)obj;
				return ((Top == insets.Top) && (Left == insets.Left) && (Bottom == insets.Bottom) && (Right == insets.Right));
			}
			return false;
		}

		/// <summary>
		/// Returns the hash code for this Insets.
		/// </summary>
		/// <returns>    a hash code for this Insets. </returns>
		public override int HashCode()
		{
			int sum1 = Left + Bottom;
			int sum2 = Right + Top;
			int val1 = sum1 * (sum1 + 1) / 2 + Left;
			int val2 = sum2 * (sum2 + 1) / 2 + Top;
			int sum3 = val1 + val2;
			return sum3 * (sum3 + 1) / 2 + val2;
		}

		/// <summary>
		/// Returns a string representation of this <code>Insets</code> object.
		/// This method is intended to be used only for debugging purposes, and
		/// the content and format of the returned string may vary between
		/// implementations. The returned string may be empty but may not be
		/// <code>null</code>.
		/// </summary>
		/// <returns>  a string representation of this <code>Insets</code> object. </returns>
		public override String ToString()
		{
//JAVA TO C# CONVERTER WARNING: The .NET Type.FullName property will not always yield results identical to the Java Class.getName method:
			return this.GetType().FullName + "[top=" + Top + ",left=" + Left + ",bottom=" + Bottom + ",right=" + Right + "]";
		}

		/// <summary>
		/// Create a copy of this object. </summary>
		/// <returns>     a copy of this <code>Insets</code> object. </returns>
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
		/// <summary>
		/// Initialize JNI field and method IDs
		/// </summary>
//JAVA TO C# CONVERTER TODO TASK: Replace 'unknown' with the appropriate dll name:
		[DllImport("unknown")]
		private static extern void initIDs();

	}

}
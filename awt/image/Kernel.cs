using System.Runtime.InteropServices;

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

namespace java.awt.image
{


	/// <summary>
	/// The <code>Kernel</code> class defines a matrix that describes how a
	/// specified pixel and its surrounding pixels affect the value
	/// computed for the pixel's position in the output image of a filtering
	/// operation.  The X origin and Y origin indicate the kernel matrix element
	/// that corresponds to the pixel position for which an output value is
	/// being computed.
	/// </summary>
	/// <seealso cref= ConvolveOp </seealso>
	public class Kernel : Cloneable
	{
		private int Width_Renamed;
		private int Height_Renamed;
		private int XOrigin_Renamed;
		private int YOrigin_Renamed;
		private float[] Data;

//JAVA TO C# CONVERTER TODO TASK: Replace 'unknown' with the appropriate dll name:
		[DllImport("unknown")]
		private static extern void initIDs();
		static Kernel()
		{
			ColorModel.LoadLibraries();
			initIDs();
		}

		/// <summary>
		/// Constructs a <code>Kernel</code> object from an array of floats.
		/// The first <code>width</code>*<code>height</code> elements of
		/// the <code>data</code> array are copied.
		/// If the length of the <code>data</code> array is less
		/// than width*height, an <code>IllegalArgumentException</code> is thrown.
		/// The X origin is (width-1)/2 and the Y origin is (height-1)/2. </summary>
		/// <param name="width">         width of the kernel </param>
		/// <param name="height">        height of the kernel </param>
		/// <param name="data">          kernel data in row major order </param>
		/// <exception cref="IllegalArgumentException"> if the length of <code>data</code>
		///         is less than the product of <code>width</code> and
		///         <code>height</code> </exception>
		public Kernel(int width, int height, float[] data)
		{
			this.Width_Renamed = width;
			this.Height_Renamed = height;
			this.XOrigin_Renamed = (width - 1) >> 1;
			this.YOrigin_Renamed = (height - 1) >> 1;
			int len = width * height;
			if (data.Length < len)
			{
				throw new IllegalArgumentException("Data array too small " + "(is " + data.Length + " and should be " + len);
			}
			this.Data = new float[len];
			System.Array.Copy(data, 0, this.Data, 0, len);

		}

		/// <summary>
		/// Returns the X origin of this <code>Kernel</code>. </summary>
		/// <returns> the X origin. </returns>
		public int XOrigin
		{
			get
			{
				return XOrigin_Renamed;
			}
		}

		/// <summary>
		/// Returns the Y origin of this <code>Kernel</code>. </summary>
		/// <returns> the Y origin. </returns>
		public int YOrigin
		{
			get
			{
				return YOrigin_Renamed;
			}
		}

		/// <summary>
		/// Returns the width of this <code>Kernel</code>. </summary>
		/// <returns> the width of this <code>Kernel</code>. </returns>
		public int Width
		{
			get
			{
				return Width_Renamed;
			}
		}

		/// <summary>
		/// Returns the height of this <code>Kernel</code>. </summary>
		/// <returns> the height of this <code>Kernel</code>. </returns>
		public int Height
		{
			get
			{
				return Height_Renamed;
			}
		}

		/// <summary>
		/// Returns the kernel data in row major order.
		/// The <code>data</code> array is returned.  If <code>data</code>
		/// is <code>null</code>, a new array is allocated. </summary>
		/// <param name="data">  if non-null, contains the returned kernel data </param>
		/// <returns> the <code>data</code> array containing the kernel data
		///         in row major order or, if <code>data</code> is
		///         <code>null</code>, a newly allocated array containing
		///         the kernel data in row major order </returns>
		/// <exception cref="IllegalArgumentException"> if <code>data</code> is less
		///         than the size of this <code>Kernel</code> </exception>
		public float[] GetKernelData(float[] data)
		{
			if (data == null)
			{
				data = new float[this.Data.Length];
			}
			else if (data.Length < this.Data.Length)
			{
				throw new IllegalArgumentException("Data array too small " + "(should be " + this.Data.Length + " but is " + data.Length + " )");
			}
			System.Array.Copy(this.Data, 0, data, 0, this.Data.Length);

			return data;
		}

		/// <summary>
		/// Clones this object. </summary>
		/// <returns> a clone of this object. </returns>
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
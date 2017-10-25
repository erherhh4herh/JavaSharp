/*
 * Copyright (c) 2000, 2011, Oracle and/or its affiliates. All rights reserved.
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
	/// Capabilities and properties of images.
	/// @author Michael Martak
	/// @since 1.4
	/// </summary>
	public class ImageCapabilities : Cloneable
	{

		private bool Accelerated_Renamed = false;

		/// <summary>
		/// Creates a new object for specifying image capabilities. </summary>
		/// <param name="accelerated"> whether or not an accelerated image is desired </param>
		public ImageCapabilities(bool accelerated)
		{
			this.Accelerated_Renamed = accelerated;
		}

		/// <summary>
		/// Returns <code>true</code> if the object whose capabilities are
		/// encapsulated in this <code>ImageCapabilities</code> can be or is
		/// accelerated. </summary>
		/// <returns> whether or not an image can be, or is, accelerated.  There are
		/// various platform-specific ways to accelerate an image, including
		/// pixmaps, VRAM, AGP.  This is the general acceleration method (as
		/// opposed to residing in system memory). </returns>
		public virtual bool Accelerated
		{
			get
			{
				return Accelerated_Renamed;
			}
		}

		/// <summary>
		/// Returns <code>true</code> if the <code>VolatileImage</code>
		/// described by this <code>ImageCapabilities</code> can lose
		/// its surfaces. </summary>
		/// <returns> whether or not a volatile image is subject to losing its surfaces
		/// at the whim of the operating system. </returns>
		public virtual bool TrueVolatile
		{
			get
			{
				return false;
			}
		}

		/// <returns> a copy of this ImageCapabilities object. </returns>
		public virtual Object Clone()
		{
			try
			{
				return base.Clone();
			}
			catch (CloneNotSupportedException e)
			{
				// Since we implement Cloneable, this should never happen
				throw new InternalError(e);
			}
		}

	}

}
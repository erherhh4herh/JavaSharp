/*
 * Copyright (c) 2003, 2014, Oracle and/or its affiliates. All rights reserved.
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
	/// A class that describes the pointer position.
	/// It provides the {@code GraphicsDevice} where the pointer is and
	/// the {@code Point} that represents the coordinates of the pointer.
	/// <para>
	/// Instances of this class should be obtained via
	/// <seealso cref="MouseInfo#getPointerInfo"/>.
	/// The {@code PointerInfo} instance is not updated dynamically as the mouse
	/// moves. To get the updated location, you must call
	/// <seealso cref="MouseInfo#getPointerInfo"/> again.
	/// 
	/// </para>
	/// </summary>
	/// <seealso cref= MouseInfo#getPointerInfo
	/// @author Roman Poborchiy
	/// @since 1.5 </seealso>
	public class PointerInfo
	{

		private readonly GraphicsDevice Device_Renamed;
		private readonly Point Location_Renamed;

		/// <summary>
		/// Package-private constructor to prevent instantiation.
		/// </summary>
//JAVA TO C# CONVERTER WARNING: 'final' parameters are not allowed in .NET:
//ORIGINAL LINE: PointerInfo(final GraphicsDevice device, final Point location)
		internal PointerInfo(GraphicsDevice device, Point location)
		{
			this.Device_Renamed = device;
			this.Location_Renamed = location;
		}

		/// <summary>
		/// Returns the {@code GraphicsDevice} where the mouse pointer was at the
		/// moment this {@code PointerInfo} was created.
		/// </summary>
		/// <returns> {@code GraphicsDevice} corresponding to the pointer
		/// @since 1.5 </returns>
		public virtual GraphicsDevice Device
		{
			get
			{
				return Device_Renamed;
			}
		}

		/// <summary>
		/// Returns the {@code Point} that represents the coordinates of the pointer
		/// on the screen. See <seealso cref="MouseInfo#getPointerInfo"/> for more information
		/// about coordinate calculation for multiscreen systems.
		/// </summary>
		/// <returns> coordinates of mouse pointer </returns>
		/// <seealso cref= MouseInfo </seealso>
		/// <seealso cref= MouseInfo#getPointerInfo
		/// @since 1.5 </seealso>
		public virtual Point Location
		{
			get
			{
				return Location_Renamed;
			}
		}
	}

}
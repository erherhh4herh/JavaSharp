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
	/// The <code>DisplayMode</code> class encapsulates the bit depth, height,
	/// width, and refresh rate of a <code>GraphicsDevice</code>. The ability to
	/// change graphics device's display mode is platform- and
	/// configuration-dependent and may not always be available
	/// (see <seealso cref="GraphicsDevice#isDisplayChangeSupported"/>).
	/// <para>
	/// For more information on full-screen exclusive mode API, see the
	/// <a href="https://docs.oracle.com/javase/tutorial/extra/fullscreen/index.html">
	/// Full-Screen Exclusive Mode API Tutorial</a>.
	/// 
	/// </para>
	/// </summary>
	/// <seealso cref= GraphicsDevice </seealso>
	/// <seealso cref= GraphicsDevice#isDisplayChangeSupported </seealso>
	/// <seealso cref= GraphicsDevice#getDisplayModes </seealso>
	/// <seealso cref= GraphicsDevice#setDisplayMode
	/// @author Michael Martak
	/// @since 1.4 </seealso>

	public sealed class DisplayMode
	{

		private Dimension Size;
		private int BitDepth_Renamed;
		private int RefreshRate_Renamed;

		/// <summary>
		/// Create a new display mode object with the supplied parameters. </summary>
		/// <param name="width"> the width of the display, in pixels </param>
		/// <param name="height"> the height of the display, in pixels </param>
		/// <param name="bitDepth"> the bit depth of the display, in bits per
		///        pixel.  This can be <code>BIT_DEPTH_MULTI</code> if multiple
		///        bit depths are available. </param>
		/// <param name="refreshRate"> the refresh rate of the display, in hertz.
		///        This can be <code>REFRESH_RATE_UNKNOWN</code> if the
		///        information is not available. </param>
		/// <seealso cref= #BIT_DEPTH_MULTI </seealso>
		/// <seealso cref= #REFRESH_RATE_UNKNOWN </seealso>
		public DisplayMode(int width, int height, int bitDepth, int refreshRate)
		{
			this.Size = new Dimension(width, height);
			this.BitDepth_Renamed = bitDepth;
			this.RefreshRate_Renamed = refreshRate;
		}

		/// <summary>
		/// Returns the height of the display, in pixels. </summary>
		/// <returns> the height of the display, in pixels </returns>
		public int Height
		{
			get
			{
				return Size.Height_Renamed;
			}
		}

		/// <summary>
		/// Returns the width of the display, in pixels. </summary>
		/// <returns> the width of the display, in pixels </returns>
		public int Width
		{
			get
			{
				return Size.Width_Renamed;
			}
		}

		/// <summary>
		/// Value of the bit depth if multiple bit depths are supported in this
		/// display mode. </summary>
		/// <seealso cref= #getBitDepth </seealso>
//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Native public final static int BIT_DEPTH_MULTI = -1;
		public const int BIT_DEPTH_MULTI = -1;

		/// <summary>
		/// Returns the bit depth of the display, in bits per pixel.  This may be
		/// <code>BIT_DEPTH_MULTI</code> if multiple bit depths are supported in
		/// this display mode.
		/// </summary>
		/// <returns> the bit depth of the display, in bits per pixel. </returns>
		/// <seealso cref= #BIT_DEPTH_MULTI </seealso>
		public int BitDepth
		{
			get
			{
				return BitDepth_Renamed;
			}
		}

		/// <summary>
		/// Value of the refresh rate if not known. </summary>
		/// <seealso cref= #getRefreshRate </seealso>
//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Native public final static int REFRESH_RATE_UNKNOWN = 0;
		public const int REFRESH_RATE_UNKNOWN = 0;

		/// <summary>
		/// Returns the refresh rate of the display, in hertz.  This may be
		/// <code>REFRESH_RATE_UNKNOWN</code> if the information is not available.
		/// </summary>
		/// <returns> the refresh rate of the display, in hertz. </returns>
		/// <seealso cref= #REFRESH_RATE_UNKNOWN </seealso>
		public int RefreshRate
		{
			get
			{
				return RefreshRate_Renamed;
			}
		}

		/// <summary>
		/// Returns whether the two display modes are equal. </summary>
		/// <returns> whether the two display modes are equal </returns>
		public bool Equals(DisplayMode dm)
		{
			if (dm == null)
			{
				return false;
			}
			return (Height == dm.Height && Width == dm.Width && BitDepth == dm.BitDepth && RefreshRate == dm.RefreshRate);
		}

		/// <summary>
		/// {@inheritDoc}
		/// </summary>
		public override bool Equals(Object dm)
		{
			if (dm is DisplayMode)
			{
				return Equals((DisplayMode)dm);
			}
			else
			{
				return false;
			}
		}

		/// <summary>
		/// {@inheritDoc}
		/// </summary>
		public override int HashCode()
		{
			return Width + Height + BitDepth * 7 + RefreshRate * 13;
		}

	}

}
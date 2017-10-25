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
	/// Capabilities and properties of buffers.
	/// </summary>
	/// <seealso cref= java.awt.image.BufferStrategy#getCapabilities() </seealso>
	/// <seealso cref= GraphicsConfiguration#getBufferCapabilities
	/// @author Michael Martak
	/// @since 1.4 </seealso>
	public class BufferCapabilities : Cloneable
	{

		private ImageCapabilities FrontCaps;
		private ImageCapabilities BackCaps;
		private FlipContents FlipContents_Renamed;

		/// <summary>
		/// Creates a new object for specifying buffering capabilities </summary>
		/// <param name="frontCaps"> the capabilities of the front buffer; cannot be
		/// <code>null</code> </param>
		/// <param name="backCaps"> the capabilities of the back and intermediate buffers;
		/// cannot be <code>null</code> </param>
		/// <param name="flipContents"> the contents of the back buffer after page-flipping,
		/// <code>null</code> if page flipping is not used (implies blitting) </param>
		/// <exception cref="IllegalArgumentException"> if frontCaps or backCaps are
		/// <code>null</code> </exception>
		public BufferCapabilities(ImageCapabilities frontCaps, ImageCapabilities backCaps, FlipContents flipContents)
		{
			if (frontCaps == null || backCaps == null)
			{
				throw new IllegalArgumentException("Image capabilities specified cannot be null");
			}
			this.FrontCaps = frontCaps;
			this.BackCaps = backCaps;
			this.FlipContents_Renamed = flipContents;
		}

		/// <returns> the image capabilities of the front (displayed) buffer </returns>
		public virtual ImageCapabilities FrontBufferCapabilities
		{
			get
			{
				return FrontCaps;
			}
		}

		/// <returns> the image capabilities of all back buffers (intermediate buffers
		/// are considered back buffers) </returns>
		public virtual ImageCapabilities BackBufferCapabilities
		{
			get
			{
				return BackCaps;
			}
		}

		/// <returns> whether or not the buffer strategy uses page flipping; a set of
		/// buffers that uses page flipping
		/// can swap the contents internally between the front buffer and one or
		/// more back buffers by switching the video pointer (or by copying memory
		/// internally).  A non-flipping set of
		/// buffers uses blitting to copy the contents from one buffer to
		/// another; when this is the case, <code>getFlipContents</code> returns
		/// <code>null</code> </returns>
		public virtual bool PageFlipping
		{
			get
			{
				return (FlipContents != null);
			}
		}

		/// <returns> the resulting contents of the back buffer after page-flipping.
		/// This value is <code>null</code> when the <code>isPageFlipping</code>
		/// returns <code>false</code>, implying blitting.  It can be one of
		/// <code>FlipContents.UNDEFINED</code>
		/// (the assumed default), <code>FlipContents.BACKGROUND</code>,
		/// <code>FlipContents.PRIOR</code>, or
		/// <code>FlipContents.COPIED</code>. </returns>
		/// <seealso cref= #isPageFlipping </seealso>
		/// <seealso cref= FlipContents#UNDEFINED </seealso>
		/// <seealso cref= FlipContents#BACKGROUND </seealso>
		/// <seealso cref= FlipContents#PRIOR </seealso>
		/// <seealso cref= FlipContents#COPIED </seealso>
		public virtual FlipContents FlipContents
		{
			get
			{
				return FlipContents_Renamed;
			}
		}

		/// <returns> whether page flipping is only available in full-screen mode.  If this
		/// is <code>true</code>, full-screen exclusive mode is required for
		/// page-flipping. </returns>
		/// <seealso cref= #isPageFlipping </seealso>
		/// <seealso cref= GraphicsDevice#setFullScreenWindow </seealso>
		public virtual bool FullScreenRequired
		{
			get
			{
				return false;
			}
		}

		/// <returns> whether or not
		/// page flipping can be performed using more than two buffers (one or more
		/// intermediate buffers as well as the front and back buffer). </returns>
		/// <seealso cref= #isPageFlipping </seealso>
		public virtual bool MultiBufferAvailable
		{
			get
			{
				return false;
			}
		}

		/// <returns> a copy of this BufferCapabilities object. </returns>
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

		// Inner class FlipContents
		/// <summary>
		/// A type-safe enumeration of the possible back buffer contents after
		/// page-flipping
		/// @since 1.4
		/// </summary>
		public sealed class FlipContents : AttributeValue
		{

			internal static int I_UNDEFINED = 0;
			internal static int I_BACKGROUND = 1;
			internal static int I_PRIOR = 2;
			internal static int I_COPIED = 3;

			internal static readonly String[] NAMES = new String[] {"undefined", "background", "prior", "copied"};

			/// <summary>
			/// When flip contents are <code>UNDEFINED</code>, the
			/// contents of the back buffer are undefined after flipping. </summary>
			/// <seealso cref= #isPageFlipping </seealso>
			/// <seealso cref= #getFlipContents </seealso>
			/// <seealso cref= #BACKGROUND </seealso>
			/// <seealso cref= #PRIOR </seealso>
			/// <seealso cref= #COPIED </seealso>
			public static readonly FlipContents UNDEFINED = new FlipContents(I_UNDEFINED);

			/// <summary>
			/// When flip contents are <code>BACKGROUND</code>, the
			/// contents of the back buffer are cleared with the background color after
			/// flipping. </summary>
			/// <seealso cref= #isPageFlipping </seealso>
			/// <seealso cref= #getFlipContents </seealso>
			/// <seealso cref= #UNDEFINED </seealso>
			/// <seealso cref= #PRIOR </seealso>
			/// <seealso cref= #COPIED </seealso>
			public static readonly FlipContents BACKGROUND = new FlipContents(I_BACKGROUND);

			/// <summary>
			/// When flip contents are <code>PRIOR</code>, the
			/// contents of the back buffer are the prior contents of the front buffer
			/// (a true page flip). </summary>
			/// <seealso cref= #isPageFlipping </seealso>
			/// <seealso cref= #getFlipContents </seealso>
			/// <seealso cref= #UNDEFINED </seealso>
			/// <seealso cref= #BACKGROUND </seealso>
			/// <seealso cref= #COPIED </seealso>
			public static readonly FlipContents PRIOR = new FlipContents(I_PRIOR);

			/// <summary>
			/// When flip contents are <code>COPIED</code>, the
			/// contents of the back buffer are copied to the front buffer when
			/// flipping. </summary>
			/// <seealso cref= #isPageFlipping </seealso>
			/// <seealso cref= #getFlipContents </seealso>
			/// <seealso cref= #UNDEFINED </seealso>
			/// <seealso cref= #BACKGROUND </seealso>
			/// <seealso cref= #PRIOR </seealso>
			public static readonly FlipContents COPIED = new FlipContents(I_COPIED);

			internal FlipContents(int type) : base(type, NAMES)
			{
			}

		} // Inner class FlipContents

	}

}
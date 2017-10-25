using System;

/*
 * Copyright (c) 1998, 2013, Oracle and/or its affiliates. All rights reserved.
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

/*
 * @author Charlton Innovations, Inc.
 */

namespace java.awt.font
{


	/// <summary>
	///   The <code>FontRenderContext</code> class is a container for the
	///   information needed to correctly measure text.  The measurement of text
	///   can vary because of rules that map outlines to pixels, and rendering
	///   hints provided by an application.
	///   <para>
	///   One such piece of information is a transform that scales
	///   typographical points to pixels. (A point is defined to be exactly 1/72
	///   of an inch, which is slightly different than
	///   the traditional mechanical measurement of a point.)  A character that
	///   is rendered at 12pt on a 600dpi device might have a different size
	///   than the same character rendered at 12pt on a 72dpi device because of
	///   such factors as rounding to pixel boundaries and hints that the font
	///   designer may have specified.
	/// </para>
	///   <para>
	///   Anti-aliasing and Fractional-metrics specified by an application can also
	///   affect the size of a character because of rounding to pixel
	///   boundaries.
	/// </para>
	///   <para>
	///   Typically, instances of <code>FontRenderContext</code> are
	///   obtained from a <seealso cref="java.awt.Graphics2D Graphics2D"/> object.  A
	///   <code>FontRenderContext</code> which is directly constructed will
	///   most likely not represent any actual graphics device, and may lead
	///   to unexpected or incorrect results.
	/// </para>
	/// </summary>
	///   <seealso cref= java.awt.RenderingHints#KEY_TEXT_ANTIALIASING </seealso>
	///   <seealso cref= java.awt.RenderingHints#KEY_FRACTIONALMETRICS </seealso>
	///   <seealso cref= java.awt.Graphics2D#getFontRenderContext() </seealso>
	///   <seealso cref= java.awt.font.LineMetrics </seealso>

	public class FontRenderContext
	{
		[NonSerialized]
		private AffineTransform Tx;
		[NonSerialized]
		private Object AaHintValue;
		[NonSerialized]
		private Object FmHintValue;
		[NonSerialized]
		private bool Defaulting;

		/// <summary>
		/// Constructs a new <code>FontRenderContext</code>
		/// object.
		/// 
		/// </summary>
		protected internal FontRenderContext()
		{
			AaHintValue = VALUE_TEXT_ANTIALIAS_DEFAULT;
			FmHintValue = VALUE_FRACTIONALMETRICS_DEFAULT;
			Defaulting = true;
		}

		/// <summary>
		/// Constructs a <code>FontRenderContext</code> object from an
		/// optional <seealso cref="AffineTransform"/> and two <code>boolean</code>
		/// values that determine if the newly constructed object has
		/// anti-aliasing or fractional metrics.
		/// In each case the boolean values <CODE>true</CODE> and <CODE>false</CODE>
		/// correspond to the rendering hint values <CODE>ON</CODE> and
		/// <CODE>OFF</CODE> respectively.
		/// <para>
		/// To specify other hint values, use the constructor which
		/// specifies the rendering hint values as parameters :
		/// <seealso cref="#FontRenderContext(AffineTransform, Object, Object)"/>.
		/// </para>
		/// </summary>
		/// <param name="tx"> the transform which is used to scale typographical points
		/// to pixels in this <code>FontRenderContext</code>.  If null, an
		/// identity transform is used. </param>
		/// <param name="isAntiAliased"> determines if the newly constructed object
		/// has anti-aliasing. </param>
		/// <param name="usesFractionalMetrics"> determines if the newly constructed
		/// object has fractional metrics. </param>
		public FontRenderContext(AffineTransform tx, bool isAntiAliased, bool usesFractionalMetrics)
		{
			if (tx != null && !tx.Identity)
			{
				this.Tx = new AffineTransform(tx);
			}
			if (isAntiAliased)
			{
				AaHintValue = VALUE_TEXT_ANTIALIAS_ON;
			}
			else
			{
				AaHintValue = VALUE_TEXT_ANTIALIAS_OFF;
			}
			if (usesFractionalMetrics)
			{
				FmHintValue = VALUE_FRACTIONALMETRICS_ON;
			}
			else
			{
				FmHintValue = VALUE_FRACTIONALMETRICS_OFF;
			}
		}

		/// <summary>
		/// Constructs a <code>FontRenderContext</code> object from an
		/// optional <seealso cref="AffineTransform"/> and two <code>Object</code>
		/// values that determine if the newly constructed object has
		/// anti-aliasing or fractional metrics. </summary>
		/// <param name="tx"> the transform which is used to scale typographical points
		/// to pixels in this <code>FontRenderContext</code>.  If null, an
		/// identity transform is used. </param>
		/// <param name="aaHint"> - one of the text antialiasing rendering hint values
		/// defined in <seealso cref="java.awt.RenderingHints java.awt.RenderingHints"/>.
		/// Any other value will throw <code>IllegalArgumentException</code>.
		/// <seealso cref="java.awt.RenderingHints#VALUE_TEXT_ANTIALIAS_DEFAULT VALUE_TEXT_ANTIALIAS_DEFAULT"/>
		/// may be specified, in which case the mode used is implementation
		/// dependent. </param>
		/// <param name="fmHint"> - one of the text fractional rendering hint values defined
		/// in <seealso cref="java.awt.RenderingHints java.awt.RenderingHints"/>.
		/// <seealso cref="java.awt.RenderingHints#VALUE_FRACTIONALMETRICS_DEFAULT VALUE_FRACTIONALMETRICS_DEFAULT"/>
		/// may be specified, in which case the mode used is implementation
		/// dependent.
		/// Any other value will throw <code>IllegalArgumentException</code> </param>
		/// <exception cref="IllegalArgumentException"> if the hints are not one of the
		/// legal values.
		/// @since 1.6 </exception>
		public FontRenderContext(AffineTransform tx, Object aaHint, Object fmHint)
		{
			if (tx != null && !tx.Identity)
			{
				this.Tx = new AffineTransform(tx);
			}
			try
			{
				if (KEY_TEXT_ANTIALIASING.isCompatibleValue(aaHint))
				{
					AaHintValue = aaHint;
				}
				else
				{
					throw new IllegalArgumentException("AA hint:" + aaHint);
				}
			}
			catch (Exception)
			{
				throw new IllegalArgumentException("AA hint:" + aaHint);
			}
			try
			{
				if (KEY_FRACTIONALMETRICS.isCompatibleValue(fmHint))
				{
					FmHintValue = fmHint;
				}
				else
				{
					throw new IllegalArgumentException("FM hint:" + fmHint);
				}
			}
			catch (Exception)
			{
				throw new IllegalArgumentException("FM hint:" + fmHint);
			}
		}

		/// <summary>
		/// Indicates whether or not this <code>FontRenderContext</code> object
		/// measures text in a transformed render context. </summary>
		/// <returns>  <code>true</code> if this <code>FontRenderContext</code>
		///          object has a non-identity AffineTransform attribute.
		///          <code>false</code> otherwise. </returns>
		/// <seealso cref=     java.awt.font.FontRenderContext#getTransform
		/// @since   1.6 </seealso>
		public virtual bool Transformed
		{
			get
			{
				if (!Defaulting)
				{
					return Tx != null;
				}
				else
				{
					return !Transform.Identity;
				}
			}
		}

		/// <summary>
		/// Returns the integer type of the affine transform for this
		/// <code>FontRenderContext</code> as specified by
		/// <seealso cref="java.awt.geom.AffineTransform#getType()"/> </summary>
		/// <returns> the type of the transform. </returns>
		/// <seealso cref= AffineTransform
		/// @since 1.6 </seealso>
		public virtual int TransformType
		{
			get
			{
				if (!Defaulting)
				{
					if (Tx == null)
					{
						return AffineTransform.TYPE_IDENTITY;
					}
					else
					{
						return Tx.Type;
					}
				}
				else
				{
					return Transform.Type;
				}
			}
		}

		/// <summary>
		///   Gets the transform that is used to scale typographical points
		///   to pixels in this <code>FontRenderContext</code>. </summary>
		///   <returns> the <code>AffineTransform</code> of this
		///    <code>FontRenderContext</code>. </returns>
		///   <seealso cref= AffineTransform </seealso>
		public virtual AffineTransform Transform
		{
			get
			{
				return (Tx == null) ? new AffineTransform() : new AffineTransform(Tx);
			}
		}

		/// <summary>
		/// Returns a boolean which indicates whether or not some form of
		/// antialiasing is specified by this <code>FontRenderContext</code>.
		/// Call <seealso cref="#getAntiAliasingHint() getAntiAliasingHint()"/>
		/// for the specific rendering hint value. </summary>
		///   <returns>    <code>true</code>, if text is anti-aliased in this
		///   <code>FontRenderContext</code>; <code>false</code> otherwise. </returns>
		///   <seealso cref=        java.awt.RenderingHints#KEY_TEXT_ANTIALIASING </seealso>
		///   <seealso cref= #FontRenderContext(AffineTransform,boolean,boolean) </seealso>
		///   <seealso cref= #FontRenderContext(AffineTransform,Object,Object) </seealso>
		public virtual bool AntiAliased
		{
			get
			{
				return !(AaHintValue == VALUE_TEXT_ANTIALIAS_OFF || AaHintValue == VALUE_TEXT_ANTIALIAS_DEFAULT);
			}
		}

		/// <summary>
		/// Returns a boolean which whether text fractional metrics mode
		/// is used in this <code>FontRenderContext</code>.
		/// Call <seealso cref="#getFractionalMetricsHint() getFractionalMetricsHint()"/>
		/// to obtain the corresponding rendering hint value. </summary>
		///   <returns>    <code>true</code>, if layout should be performed with
		///   fractional metrics; <code>false</code> otherwise.
		///               in this <code>FontRenderContext</code>. </returns>
		///   <seealso cref= java.awt.RenderingHints#KEY_FRACTIONALMETRICS </seealso>
		///   <seealso cref= #FontRenderContext(AffineTransform,boolean,boolean) </seealso>
		///   <seealso cref= #FontRenderContext(AffineTransform,Object,Object) </seealso>
		public virtual bool UsesFractionalMetrics()
		{
			return !(FmHintValue == VALUE_FRACTIONALMETRICS_OFF || FmHintValue == VALUE_FRACTIONALMETRICS_DEFAULT);
		}

		/// <summary>
		/// Return the text anti-aliasing rendering mode hint used in this
		/// <code>FontRenderContext</code>.
		/// This will be one of the text antialiasing rendering hint values
		/// defined in <seealso cref="java.awt.RenderingHints java.awt.RenderingHints"/>. </summary>
		/// <returns>  text anti-aliasing rendering mode hint used in this
		/// <code>FontRenderContext</code>.
		/// @since 1.6 </returns>
		public virtual Object AntiAliasingHint
		{
			get
			{
				if (Defaulting)
				{
					if (AntiAliased)
					{
						 return VALUE_TEXT_ANTIALIAS_ON;
					}
					else
					{
						return VALUE_TEXT_ANTIALIAS_OFF;
					}
				}
				return AaHintValue;
			}
		}

		/// <summary>
		/// Return the text fractional metrics rendering mode hint used in this
		/// <code>FontRenderContext</code>.
		/// This will be one of the text fractional metrics rendering hint values
		/// defined in <seealso cref="java.awt.RenderingHints java.awt.RenderingHints"/>. </summary>
		/// <returns> the text fractional metrics rendering mode hint used in this
		/// <code>FontRenderContext</code>.
		/// @since 1.6 </returns>
		public virtual Object FractionalMetricsHint
		{
			get
			{
				if (Defaulting)
				{
					if (UsesFractionalMetrics())
					{
						 return VALUE_FRACTIONALMETRICS_ON;
					}
					else
					{
						return VALUE_FRACTIONALMETRICS_OFF;
					}
				}
				return FmHintValue;
			}
		}

		/// <summary>
		/// Return true if obj is an instance of FontRenderContext and has the same
		/// transform, antialiasing, and fractional metrics values as this. </summary>
		/// <param name="obj"> the object to test for equality </param>
		/// <returns> <code>true</code> if the specified object is equal to
		///         this <code>FontRenderContext</code>; <code>false</code>
		///         otherwise. </returns>
		public override bool Equals(Object obj)
		{
			try
			{
				return Equals((FontRenderContext)obj);
			}
			catch (ClassCastException)
			{
				return false;
			}
		}

		/// <summary>
		/// Return true if rhs has the same transform, antialiasing,
		/// and fractional metrics values as this. </summary>
		/// <param name="rhs"> the <code>FontRenderContext</code> to test for equality </param>
		/// <returns> <code>true</code> if <code>rhs</code> is equal to
		///         this <code>FontRenderContext</code>; <code>false</code>
		///         otherwise.
		/// @since 1.4 </returns>
		public virtual bool Equals(FontRenderContext rhs)
		{
			if (this == rhs)
			{
				return true;
			}
			if (rhs == null)
			{
				return false;
			}

			/* if neither instance is a subclass, reference values directly. */
			if (!rhs.Defaulting && !Defaulting)
			{
				if (rhs.AaHintValue == AaHintValue && rhs.FmHintValue == FmHintValue)
				{

					return Tx == null ? rhs.Tx == null : Tx.Equals(rhs.Tx);
				}
				return false;
			}
			else
			{
				return rhs.AntiAliasingHint == AntiAliasingHint && rhs.FractionalMetricsHint == FractionalMetricsHint && rhs.Transform.Equals(Transform);
			}
		}

		/// <summary>
		/// Return a hashcode for this FontRenderContext.
		/// </summary>
		public override int HashCode()
		{
			int hash = Tx == null ? 0 : Tx.HashCode();
			/* SunHints value objects have identity hashcode, so we can rely on
			 * this to ensure that two equal FRC's have the same hashcode.
			 */
			if (Defaulting)
			{
				hash += AntiAliasingHint.HashCode();
				hash += FractionalMetricsHint.HashCode();
			}
			else
			{
				hash += AaHintValue.HashCode();
				hash += FmHintValue.HashCode();
			}
			return hash;
		}
	}

}
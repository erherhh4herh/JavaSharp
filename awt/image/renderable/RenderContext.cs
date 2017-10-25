using System;

/*
 * Copyright (c) 1998, 2008, Oracle and/or its affiliates. All rights reserved.
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

/* ********************************************************************
 **********************************************************************
 **********************************************************************
 *** COPYRIGHT (c) Eastman Kodak Company, 1997                      ***
 *** As  an unpublished  work pursuant to Title 17 of the United    ***
 *** States Code.  All rights reserved.                             ***
 **********************************************************************
 **********************************************************************
 **********************************************************************/

namespace java.awt.image.renderable
{

	/// <summary>
	/// A RenderContext encapsulates the information needed to produce a
	/// specific rendering from a RenderableImage.  It contains the area to
	/// be rendered specified in rendering-independent terms, the
	/// resolution at which the rendering is to be performed, and hints
	/// used to control the rendering process.
	/// 
	/// <para> Users create RenderContexts and pass them to the
	/// RenderableImage via the createRendering method.  Most of the methods of
	/// RenderContexts are not meant to be used directly by applications,
	/// but by the RenderableImage and operator classes to which it is
	/// passed.
	/// 
	/// </para>
	/// <para> The AffineTransform parameter passed into and out of this class
	/// are cloned.  The RenderingHints and Shape parameters are not
	/// necessarily cloneable and are therefore only reference copied.
	/// Altering RenderingHints or Shape instances that are in use by
	/// instances of RenderContext may have undesired side effects.
	/// </para>
	/// </summary>
	public class RenderContext : Cloneable
	{

		/// <summary>
		/// Table of hints. May be null. </summary>
		internal RenderingHints Hints;

		/// <summary>
		/// Transform to convert user coordinates to device coordinates. </summary>
		internal AffineTransform Usr2dev;

		/// <summary>
		/// The area of interest.  May be null. </summary>
		internal Shape Aoi;

		// Various constructors that allow different levels of
		// specificity. If the Shape is missing the whole renderable area
		// is assumed. If hints is missing no hints are assumed.

		/// <summary>
		/// Constructs a RenderContext with a given transform.
		/// The area of interest is supplied as a Shape,
		/// and the rendering hints are supplied as a RenderingHints object.
		/// </summary>
		/// <param name="usr2dev"> an AffineTransform. </param>
		/// <param name="aoi"> a Shape representing the area of interest. </param>
		/// <param name="hints"> a RenderingHints object containing rendering hints. </param>
		public RenderContext(AffineTransform usr2dev, Shape aoi, RenderingHints hints)
		{
			this.Hints = hints;
			this.Aoi = aoi;
			this.Usr2dev = (AffineTransform)usr2dev.Clone();
		}

		/// <summary>
		/// Constructs a RenderContext with a given transform.
		/// The area of interest is taken to be the entire renderable area.
		/// No rendering hints are used.
		/// </summary>
		/// <param name="usr2dev"> an AffineTransform. </param>
		public RenderContext(AffineTransform usr2dev) : this(usr2dev, null, null)
		{
		}

		/// <summary>
		/// Constructs a RenderContext with a given transform and rendering hints.
		/// The area of interest is taken to be the entire renderable area.
		/// </summary>
		/// <param name="usr2dev"> an AffineTransform. </param>
		/// <param name="hints"> a RenderingHints object containing rendering hints. </param>
		public RenderContext(AffineTransform usr2dev, RenderingHints hints) : this(usr2dev, null, hints)
		{
		}

		/// <summary>
		/// Constructs a RenderContext with a given transform and area of interest.
		/// The area of interest is supplied as a Shape.
		/// No rendering hints are used.
		/// </summary>
		/// <param name="usr2dev"> an AffineTransform. </param>
		/// <param name="aoi"> a Shape representing the area of interest. </param>
		public RenderContext(AffineTransform usr2dev, Shape aoi) : this(usr2dev, aoi, null)
		{
		}

		/// <summary>
		/// Gets the rendering hints of this <code>RenderContext</code>. </summary>
		/// <returns> a <code>RenderingHints</code> object that represents
		/// the rendering hints of this <code>RenderContext</code>. </returns>
		/// <seealso cref= #setRenderingHints(RenderingHints) </seealso>
		public virtual RenderingHints RenderingHints
		{
			get
			{
				return Hints;
			}
			set
			{
				this.Hints = value;
			}
		}


		/// <summary>
		/// Sets the current user-to-device AffineTransform contained
		/// in the RenderContext to a given transform.
		/// </summary>
		/// <param name="newTransform"> the new AffineTransform. </param>
		/// <seealso cref= #getTransform </seealso>
		public virtual AffineTransform Transform
		{
			set
			{
				Usr2dev = (AffineTransform)value.Clone();
			}
			get
			{
				return (AffineTransform)Usr2dev.Clone();
			}
		}

		/// <summary>
		/// Modifies the current user-to-device transform by prepending another
		/// transform.  In matrix notation the operation is:
		/// <pre>
		/// [this] = [modTransform] x [this]
		/// </pre>
		/// </summary>
		/// <param name="modTransform"> the AffineTransform to prepend to the
		///        current usr2dev transform.
		/// @since 1.3 </param>
		public virtual void PreConcatenateTransform(AffineTransform modTransform)
		{
			this.PreConcetenateTransform(modTransform);
		}

		/// <summary>
		/// Modifies the current user-to-device transform by prepending another
		/// transform.  In matrix notation the operation is:
		/// <pre>
		/// [this] = [modTransform] x [this]
		/// </pre>
		/// This method does the same thing as the preConcatenateTransform
		/// method.  It is here for backward compatibility with previous releases
		/// which misspelled the method name.
		/// </summary>
		/// <param name="modTransform"> the AffineTransform to prepend to the
		///        current usr2dev transform. </param>
		/// @deprecated     replaced by
		///                 <code>preConcatenateTransform(AffineTransform)</code>. 
		[Obsolete("    replaced by")]
		public virtual void PreConcetenateTransform(AffineTransform modTransform)
		{
			Usr2dev.PreConcatenate(modTransform);
		}

		/// <summary>
		/// Modifies the current user-to-device transform by appending another
		/// transform.  In matrix notation the operation is:
		/// <pre>
		/// [this] = [this] x [modTransform]
		/// </pre>
		/// </summary>
		/// <param name="modTransform"> the AffineTransform to append to the
		///        current usr2dev transform.
		/// @since 1.3 </param>
		public virtual void ConcatenateTransform(AffineTransform modTransform)
		{
			this.ConcetenateTransform(modTransform);
		}

		/// <summary>
		/// Modifies the current user-to-device transform by appending another
		/// transform.  In matrix notation the operation is:
		/// <pre>
		/// [this] = [this] x [modTransform]
		/// </pre>
		/// This method does the same thing as the concatenateTransform
		/// method.  It is here for backward compatibility with previous releases
		/// which misspelled the method name.
		/// </summary>
		/// <param name="modTransform"> the AffineTransform to append to the
		///        current usr2dev transform. </param>
		/// @deprecated     replaced by
		///                 <code>concatenateTransform(AffineTransform)</code>. 
		[Obsolete("    replaced by")]
		public virtual void ConcetenateTransform(AffineTransform modTransform)
		{
			Usr2dev.Concatenate(modTransform);
		}


		/// <summary>
		/// Sets the current area of interest.  The old area is discarded.
		/// </summary>
		/// <param name="newAoi"> The new area of interest. </param>
		/// <seealso cref= #getAreaOfInterest </seealso>
		public virtual Shape AreaOfInterest
		{
			set
			{
				Aoi = value;
			}
			get
			{
				return Aoi;
			}
		}


		/// <summary>
		/// Makes a copy of a RenderContext. The area of interest is copied
		/// by reference.  The usr2dev AffineTransform and hints are cloned,
		/// while the area of interest is copied by reference.
		/// </summary>
		/// <returns> the new cloned RenderContext. </returns>
		public virtual Object Clone()
		{
			RenderContext newRenderContext = new RenderContext(Usr2dev, Aoi, Hints);
			return newRenderContext;
		}
	}

}
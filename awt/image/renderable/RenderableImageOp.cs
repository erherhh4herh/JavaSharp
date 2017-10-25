using System;
using System.Collections;
using System.Collections.Generic;

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
	/// This class handles the renderable aspects of an operation with help
	/// from its associated instance of a ContextualRenderedImageFactory.
	/// </summary>
	public class RenderableImageOp : RenderableImage
	{

		/// <summary>
		/// A ParameterBlock containing source and parameters. </summary>
		internal ParameterBlock ParamBlock;

		/// <summary>
		/// The associated ContextualRenderedImageFactory. </summary>
		internal ContextualRenderedImageFactory MyCRIF;

		/// <summary>
		/// The bounding box of the results of this RenderableImageOp. </summary>
		internal Rectangle2D BoundingBox;


		/// <summary>
		/// Constructs a RenderedImageOp given a
		/// ContextualRenderedImageFactory object, and
		/// a ParameterBlock containing RenderableImage sources and other
		/// parameters.  Any RenderedImage sources referenced by the
		/// ParameterBlock will be ignored.
		/// </summary>
		/// <param name="CRIF"> a ContextualRenderedImageFactory object </param>
		/// <param name="paramBlock"> a ParameterBlock containing this operation's source
		///        images and other parameters necessary for the operation
		///        to run. </param>
		public RenderableImageOp(ContextualRenderedImageFactory CRIF, ParameterBlock paramBlock)
		{
			this.MyCRIF = CRIF;
			this.ParamBlock = (ParameterBlock) paramBlock.Clone();
		}

		/// <summary>
		/// Returns a vector of RenderableImages that are the sources of
		/// image data for this RenderableImage. Note that this method may
		/// return an empty vector, to indicate that the image has no sources,
		/// or null, to indicate that no information is available.
		/// </summary>
		/// <returns> a (possibly empty) Vector of RenderableImages, or null. </returns>
		public virtual List<RenderableImage> Sources
		{
			get
			{
				return RenderableSources;
			}
		}

		private ArrayList RenderableSources
		{
			get
			{
				ArrayList sources = null;
    
				if (ParamBlock.NumSources > 0)
				{
					sources = new ArrayList();
					int i = 0;
					while (i < ParamBlock.NumSources)
					{
						Object o = ParamBlock.GetSource(i);
						if (o is RenderableImage)
						{
							sources.Add((RenderableImage)o);
							i++;
						}
						else
						{
							break;
						}
					}
				}
				return sources;
			}
		}

		/// <summary>
		/// Gets a property from the property set of this image.
		/// If the property name is not recognized, java.awt.Image.UndefinedProperty
		/// will be returned.
		/// </summary>
		/// <param name="name"> the name of the property to get, as a String. </param>
		/// <returns> a reference to the property Object, or the value
		///         java.awt.Image.UndefinedProperty. </returns>
		public virtual Object GetProperty(String name)
		{
			return MyCRIF.GetProperty(ParamBlock, name);
		}

		/// <summary>
		/// Return a list of names recognized by getProperty. </summary>
		/// <returns> a list of property names. </returns>
		public virtual String[] PropertyNames
		{
			get
			{
				return MyCRIF.PropertyNames;
			}
		}

		/// <summary>
		/// Returns true if successive renderings (that is, calls to
		/// createRendering() or createScaledRendering()) with the same arguments
		/// may produce different results.  This method may be used to
		/// determine whether an existing rendering may be cached and
		/// reused.  The CRIF's isDynamic method will be called. </summary>
		/// <returns> <code>true</code> if successive renderings with the
		///         same arguments might produce different results;
		///         <code>false</code> otherwise. </returns>
		public virtual bool Dynamic
		{
			get
			{
				return MyCRIF.Dynamic;
			}
		}

		/// <summary>
		/// Gets the width in user coordinate space.  By convention, the
		/// usual width of a RenderableImage is equal to the image's aspect
		/// ratio (width divided by height).
		/// </summary>
		/// <returns> the width of the image in user coordinates. </returns>
		public virtual float Width
		{
			get
			{
				if (BoundingBox == null)
				{
					BoundingBox = MyCRIF.GetBounds2D(ParamBlock);
				}
				return (float)BoundingBox.Width;
			}
		}

		/// <summary>
		/// Gets the height in user coordinate space.  By convention, the
		/// usual height of a RenderedImage is equal to 1.0F.
		/// </summary>
		/// <returns> the height of the image in user coordinates. </returns>
		public virtual float Height
		{
			get
			{
				if (BoundingBox == null)
				{
					BoundingBox = MyCRIF.GetBounds2D(ParamBlock);
				}
				return (float)BoundingBox.Height;
			}
		}

		/// <summary>
		/// Gets the minimum X coordinate of the rendering-independent image data.
		/// </summary>
		public virtual float MinX
		{
			get
			{
				if (BoundingBox == null)
				{
					BoundingBox = MyCRIF.GetBounds2D(ParamBlock);
				}
				return (float)BoundingBox.MinX;
			}
		}

		/// <summary>
		/// Gets the minimum Y coordinate of the rendering-independent image data.
		/// </summary>
		public virtual float MinY
		{
			get
			{
				if (BoundingBox == null)
				{
					BoundingBox = MyCRIF.GetBounds2D(ParamBlock);
				}
				return (float)BoundingBox.MinY;
			}
		}

		/// <summary>
		/// Change the current ParameterBlock of the operation, allowing
		/// editing of image rendering chains.  The effects of such a
		/// change will be visible when a new rendering is created from
		/// this RenderableImageOp or any dependent RenderableImageOp.
		/// </summary>
		/// <param name="paramBlock"> the new ParameterBlock. </param>
		/// <returns> the old ParameterBlock. </returns>
		/// <seealso cref= #getParameterBlock </seealso>
		public virtual ParameterBlock SetParameterBlock(ParameterBlock paramBlock)
		{
			ParameterBlock oldParamBlock = this.ParamBlock;
			this.ParamBlock = (ParameterBlock)paramBlock.Clone();
			return oldParamBlock;
		}

		/// <summary>
		/// Returns a reference to the current parameter block. </summary>
		/// <returns> the <code>ParameterBlock</code> of this
		///         <code>RenderableImageOp</code>. </returns>
		/// <seealso cref= #setParameterBlock(ParameterBlock) </seealso>
		public virtual ParameterBlock ParameterBlock
		{
			get
			{
				return ParamBlock;
			}
		}

		/// <summary>
		/// Creates a RenderedImage instance of this image with width w, and
		/// height h in pixels.  The RenderContext is built automatically
		/// with an appropriate usr2dev transform and an area of interest
		/// of the full image.  All the rendering hints come from hints
		/// passed in.
		/// 
		/// <para> If w == 0, it will be taken to equal
		/// Math.round(h*(getWidth()/getHeight())).
		/// Similarly, if h == 0, it will be taken to equal
		/// Math.round(w*(getHeight()/getWidth())).  One of
		/// w or h must be non-zero or else an IllegalArgumentException
		/// will be thrown.
		/// 
		/// </para>
		/// <para> The created RenderedImage may have a property identified
		/// by the String HINTS_OBSERVED to indicate which RenderingHints
		/// were used to create the image.  In addition any RenderedImages
		/// that are obtained via the getSources() method on the created
		/// RenderedImage may have such a property.
		/// 
		/// </para>
		/// </summary>
		/// <param name="w"> the width of rendered image in pixels, or 0. </param>
		/// <param name="h"> the height of rendered image in pixels, or 0. </param>
		/// <param name="hints"> a RenderingHints object containing hints. </param>
		/// <returns> a RenderedImage containing the rendered data. </returns>
		public virtual RenderedImage CreateScaledRendering(int w, int h, RenderingHints hints)
		{
			// DSR -- code to try to get a unit scale
			double sx = (double)w / Width;
			double sy = (double)h / Height;
			if (System.Math.Abs(sx / sy - 1.0) < 0.01)
			{
				sx = sy;
			}
			AffineTransform usr2dev = AffineTransform.GetScaleInstance(sx, sy);
			RenderContext newRC = new RenderContext(usr2dev, hints);
			return CreateRendering(newRC);
		}

		/// <summary>
		/// Gets a RenderedImage instance of this image with a default
		/// width and height in pixels.  The RenderContext is built
		/// automatically with an appropriate usr2dev transform and an area
		/// of interest of the full image.  All the rendering hints come
		/// from hints passed in.  Implementors of this interface must be
		/// sure that there is a defined default width and height.
		/// </summary>
		/// <returns> a RenderedImage containing the rendered data. </returns>
		public virtual RenderedImage CreateDefaultRendering()
		{
			AffineTransform usr2dev = new AffineTransform(); // Identity
			RenderContext newRC = new RenderContext(usr2dev);
			return CreateRendering(newRC);
		}

		/// <summary>
		/// Creates a RenderedImage which represents this
		/// RenderableImageOp (including its Renderable sources) rendered
		/// according to the given RenderContext.
		/// 
		/// <para> This method supports chaining of either Renderable or
		/// RenderedImage operations.  If sources in
		/// the ParameterBlock used to construct the RenderableImageOp are
		/// RenderableImages, then a three step process is followed:
		/// 
		/// <ol>
		/// <li> mapRenderContext() is called on the associated CRIF for
		/// each RenderableImage source;
		/// <li> createRendering() is called on each of the RenderableImage sources
		/// using the backwards-mapped RenderContexts obtained in step 1,
		/// resulting in a rendering of each source;
		/// <li> ContextualRenderedImageFactory.create() is called
		/// with a new ParameterBlock containing the parameters of
		/// the RenderableImageOp and the RenderedImages that were created by the
		/// createRendering() calls.
		/// </ol>
		/// 
		/// </para>
		/// <para> If the elements of the source Vector of
		/// the ParameterBlock used to construct the RenderableImageOp are
		/// instances of RenderedImage, then the CRIF.create() method is
		/// called immediately using the original ParameterBlock.
		/// This provides a basis case for the recursion.
		/// 
		/// </para>
		/// <para> The created RenderedImage may have a property identified
		/// by the String HINTS_OBSERVED to indicate which RenderingHints
		/// (from the RenderContext) were used to create the image.
		/// In addition any RenderedImages
		/// that are obtained via the getSources() method on the created
		/// RenderedImage may have such a property.
		/// 
		/// </para>
		/// </summary>
		/// <param name="renderContext"> The RenderContext to use to perform the rendering. </param>
		/// <returns> a RenderedImage containing the desired output image. </returns>
		public virtual RenderedImage CreateRendering(RenderContext renderContext)
		{
			RenderedImage image = null;
			RenderContext rcOut = null;

			// Clone the original ParameterBlock; if the ParameterBlock
			// contains RenderableImage sources, they will be replaced by
			// RenderedImages.
			ParameterBlock renderedParamBlock = (ParameterBlock)ParamBlock.Clone();
			ArrayList sources = RenderableSources;

			try
			{
				// This assumes that if there is no renderable source, that there
				// is a rendered source in paramBlock

				if (sources != null)
				{
					ArrayList renderedSources = new ArrayList();
					for (int i = 0; i < sources.Count; i++)
					{
						rcOut = MyCRIF.MapRenderContext(i, renderContext, ParamBlock, this);
						RenderedImage rdrdImage = ((RenderableImage)sources[i]).CreateRendering(rcOut);
						if (rdrdImage == null)
						{
							return null;
						}

						// Add this rendered image to the ParameterBlock's
						// list of RenderedImages.
						renderedSources.Add(rdrdImage);
					}

					if (renderedSources.Count > 0)
					{
						renderedParamBlock.Sources = renderedSources;
					}
				}

				return MyCRIF.Create(renderContext, renderedParamBlock);
			}
			catch (ArrayIndexOutOfBoundsException)
			{
				// This should never happen
				return null;
			}
		}
	}

}
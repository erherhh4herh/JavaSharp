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
	/// ContextualRenderedImageFactory provides an interface for the
	/// functionality that may differ between instances of
	/// RenderableImageOp.  Thus different operations on RenderableImages
	/// may be performed by a single class such as RenderedImageOp through
	/// the use of multiple instances of ContextualRenderedImageFactory.
	/// The name ContextualRenderedImageFactory is commonly shortened to
	/// "CRIF."
	/// 
	/// <para> All operations that are to be used in a rendering-independent
	/// chain must implement ContextualRenderedImageFactory.
	/// 
	/// </para>
	/// <para> Classes that implement this interface must provide a
	/// constructor with no arguments.
	/// </para>
	/// </summary>
	public interface ContextualRenderedImageFactory : RenderedImageFactory
	{

		/// <summary>
		/// Maps the operation's output RenderContext into a RenderContext
		/// for each of the operation's sources.  This is useful for
		/// operations that can be expressed in whole or in part simply as
		/// alterations in the RenderContext, such as an affine mapping, or
		/// operations that wish to obtain lower quality renderings of
		/// their sources in order to save processing effort or
		/// transmission bandwith.  Some operations, such as blur, can also
		/// use this mechanism to avoid obtaining sources of higher quality
		/// than necessary.
		/// </summary>
		/// <param name="i"> the index of the source image. </param>
		/// <param name="renderContext"> the RenderContext being applied to the operation. </param>
		/// <param name="paramBlock"> a ParameterBlock containing the operation's
		///        sources and parameters. </param>
		/// <param name="image"> the RenderableImage being rendered. </param>
		/// <returns> a <code>RenderContext</code> for
		///         the source at the specified index of the parameters
		///         Vector contained in the specified ParameterBlock. </returns>
		RenderContext MapRenderContext(int i, RenderContext renderContext, ParameterBlock paramBlock, RenderableImage image);

		/// <summary>
		/// Creates a rendering, given a RenderContext and a ParameterBlock
		/// containing the operation's sources and parameters.  The output
		/// is a RenderedImage that takes the RenderContext into account to
		/// determine its dimensions and placement on the image plane.
		/// This method houses the "intelligence" that allows a
		/// rendering-independent operation to adapt to a specific
		/// RenderContext.
		/// </summary>
		/// <param name="renderContext"> The RenderContext specifying the rendering </param>
		/// <param name="paramBlock"> a ParameterBlock containing the operation's
		///        sources and parameters </param>
		/// <returns> a <code>RenderedImage</code> from the sources and parameters
		///         in the specified ParameterBlock and according to the
		///         rendering instructions in the specified RenderContext. </returns>
		RenderedImage Create(RenderContext renderContext, ParameterBlock paramBlock);

		/// <summary>
		/// Returns the bounding box for the output of the operation,
		/// performed on a given set of sources, in rendering-independent
		/// space.  The bounds are returned as a Rectangle2D, that is, an
		/// axis-aligned rectangle with floating-point corner coordinates.
		/// </summary>
		/// <param name="paramBlock"> a ParameterBlock containing the operation's
		///        sources and parameters. </param>
		/// <returns> a Rectangle2D specifying the rendering-independent
		///         bounding box of the output. </returns>
		Rectangle2D GetBounds2D(ParameterBlock paramBlock);

		/// <summary>
		/// Gets the appropriate instance of the property specified by the name
		/// parameter.  This method must determine which instance of a property to
		/// return when there are multiple sources that each specify the property.
		/// </summary>
		/// <param name="paramBlock"> a ParameterBlock containing the operation's
		///        sources and parameters. </param>
		/// <param name="name"> a String naming the desired property. </param>
		/// <returns> an object reference to the value of the property requested. </returns>
		Object GetProperty(ParameterBlock paramBlock, String name);

		/// <summary>
		/// Returns a list of names recognized by getProperty. </summary>
		/// <returns> the list of property names. </returns>
		String[] PropertyNames {get;}

		/// <summary>
		/// Returns true if successive renderings (that is, calls to
		/// create(RenderContext, ParameterBlock)) with the same arguments
		/// may produce different results.  This method may be used to
		/// determine whether an existing rendering may be cached and
		/// reused.  It is always safe to return true. </summary>
		/// <returns> <code>true</code> if successive renderings with the
		///         same arguments might produce different results;
		///         <code>false</code> otherwise. </returns>
		bool Dynamic {get;}
	}

}
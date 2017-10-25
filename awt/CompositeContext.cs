/*
 * Copyright (c) 1997, 1998, Oracle and/or its affiliates. All rights reserved.
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
	/// The <code>CompositeContext</code> interface defines the encapsulated
	/// and optimized environment for a compositing operation.
	/// <code>CompositeContext</code> objects maintain state for
	/// compositing operations.  In a multi-threaded environment, several
	/// contexts can exist simultaneously for a single <seealso cref="Composite"/>
	/// object. </summary>
	/// <seealso cref= Composite </seealso>

	public interface CompositeContext
	{
		/// <summary>
		/// Releases resources allocated for a context.
		/// </summary>
		void Dispose();

		/// <summary>
		/// Composes the two source <seealso cref="Raster"/> objects and
		/// places the result in the destination
		/// <seealso cref="WritableRaster"/>.  Note that the destination
		/// can be the same object as either the first or second
		/// source. Note that <code>dstIn</code> and
		/// <code>dstOut</code> must be compatible with the
		/// <code>dstColorModel</code> passed to the
		/// <seealso cref="Composite#createContext(java.awt.image.ColorModel, java.awt.image.ColorModel, java.awt.RenderingHints) createContext"/>
		/// method of the <code>Composite</code> interface. </summary>
		/// <param name="src"> the first source for the compositing operation </param>
		/// <param name="dstIn"> the second source for the compositing operation </param>
		/// <param name="dstOut"> the <code>WritableRaster</code> into which the
		/// result of the operation is stored </param>
		/// <seealso cref= Composite </seealso>
		void Compose(Raster src, Raster dstIn, WritableRaster dstOut);


	}

}
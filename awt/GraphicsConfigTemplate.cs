using System;

/*
 * Copyright (c) 1997, 2007, Oracle and/or its affiliates. All rights reserved.
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
	/// The <code>GraphicsConfigTemplate</code> class is used to obtain a valid
	/// <seealso cref="GraphicsConfiguration"/>.  A user instantiates one of these
	/// objects and then sets all non-default attributes as desired.  The
	/// <seealso cref="GraphicsDevice#getBestConfiguration"/> method found in the
	/// <seealso cref="GraphicsDevice"/> class is then called with this
	/// <code>GraphicsConfigTemplate</code>.  A valid
	/// <code>GraphicsConfiguration</code> is returned that meets or exceeds
	/// what was requested in the <code>GraphicsConfigTemplate</code>. </summary>
	/// <seealso cref= GraphicsDevice </seealso>
	/// <seealso cref= GraphicsConfiguration
	/// 
	/// @since       1.2 </seealso>
	[Serializable]
	public abstract class GraphicsConfigTemplate
	{
		/*
		 * serialVersionUID
		 */
		private const long SerialVersionUID = -8061369279557787079L;

		/// <summary>
		/// This class is an abstract class so only subclasses can be
		/// instantiated.
		/// </summary>
		public GraphicsConfigTemplate()
		{
		}

		/// <summary>
		/// Value used for "Enum" (Integer) type.  States that this
		/// feature is required for the <code>GraphicsConfiguration</code>
		/// object.  If this feature is not available, do not select the
		/// <code>GraphicsConfiguration</code> object.
		/// </summary>
		public const int REQUIRED = 1;

		/// <summary>
		/// Value used for "Enum" (Integer) type.  States that this
		/// feature is desired for the <code>GraphicsConfiguration</code>
		/// object.  A selection with this feature is preferred over a
		/// selection that does not include this feature, although both
		/// selections can be considered valid matches.
		/// </summary>
		public const int PREFERRED = 2;

		/// <summary>
		/// Value used for "Enum" (Integer) type.  States that this
		/// feature is not necessary for the selection of the
		/// <code>GraphicsConfiguration</code> object.  A selection
		/// without this feature is preferred over a selection that
		/// includes this feature since it is not used.
		/// </summary>
		public const int UNNECESSARY = 3;

		/// <summary>
		/// Returns the "best" configuration possible that passes the
		/// criteria defined in the <code>GraphicsConfigTemplate</code>. </summary>
		/// <param name="gc"> the array of <code>GraphicsConfiguration</code>
		/// objects to choose from. </param>
		/// <returns> a <code>GraphicsConfiguration</code> object that is
		/// the best configuration possible. </returns>
		/// <seealso cref= GraphicsConfiguration </seealso>
		public abstract GraphicsConfiguration GetBestConfiguration(GraphicsConfiguration[] gc);

		/// <summary>
		/// Returns a <code>boolean</code> indicating whether or
		/// not the specified <code>GraphicsConfiguration</code> can be
		/// used to create a drawing surface that supports the indicated
		/// features. </summary>
		/// <param name="gc"> the <code>GraphicsConfiguration</code> object to test </param>
		/// <returns> <code>true</code> if this
		/// <code>GraphicsConfiguration</code> object can be used to create
		/// surfaces that support the indicated features;
		/// <code>false</code> if the <code>GraphicsConfiguration</code> can
		/// not be used to create a drawing surface usable by this Java(tm)
		/// API. </returns>
		public abstract bool IsGraphicsConfigSupported(GraphicsConfiguration gc);

	}

}
/*
 * Copyright (c) 1996, 2006, Oracle and/or its affiliates. All rights reserved.
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

namespace java.awt.geom
{

	/// <summary>
	/// The {@code GeneralPath} class represents a geometric path
	/// constructed from straight lines, and quadratic and cubic
	/// (B&eacute;zier) curves.  It can contain multiple subpaths.
	/// <para>
	/// {@code GeneralPath} is a legacy final class which exactly
	/// implements the behavior of its superclass <seealso cref="Path2D.Float"/>.
	/// Together with <seealso cref="Path2D.Double"/>, the <seealso cref="Path2D"/> classes
	/// provide full implementations of a general geometric path that
	/// support all of the functionality of the <seealso cref="Shape"/> and
	/// <seealso cref="PathIterator"/> interfaces with the ability to explicitly
	/// select different levels of internal coordinate precision.
	/// </para>
	/// <para>
	/// Use {@code Path2D.Float} (or this legacy {@code GeneralPath}
	/// subclass) when dealing with data that can be represented
	/// and used with floating point precision.  Use {@code Path2D.Double}
	/// for data that requires the accuracy or range of double precision.
	/// 
	/// @author Jim Graham
	/// @since 1.2
	/// </para>
	/// </summary>
	public sealed class GeneralPath : Path2D.Float
	{
		/// <summary>
		/// Constructs a new empty single precision {@code GeneralPath} object
		/// with a default winding rule of <seealso cref="#WIND_NON_ZERO"/>.
		/// 
		/// @since 1.2
		/// </summary>
		public GeneralPath() : base(WIND_NON_ZERO, INIT_SIZE)
		{
		}

		/// <summary>
		/// Constructs a new <code>GeneralPath</code> object with the specified
		/// winding rule to control operations that require the interior of the
		/// path to be defined.
		/// </summary>
		/// <param name="rule"> the winding rule </param>
		/// <seealso cref= #WIND_EVEN_ODD </seealso>
		/// <seealso cref= #WIND_NON_ZERO
		/// @since 1.2 </seealso>
		public GeneralPath(int rule) : base(rule, INIT_SIZE)
		{
		}

		/// <summary>
		/// Constructs a new <code>GeneralPath</code> object with the specified
		/// winding rule and the specified initial capacity to store path
		/// coordinates.
		/// This number is an initial guess as to how many path segments
		/// will be added to the path, but the storage is expanded as
		/// needed to store whatever path segments are added.
		/// </summary>
		/// <param name="rule"> the winding rule </param>
		/// <param name="initialCapacity"> the estimate for the number of path segments
		///                        in the path </param>
		/// <seealso cref= #WIND_EVEN_ODD </seealso>
		/// <seealso cref= #WIND_NON_ZERO
		/// @since 1.2 </seealso>
		public GeneralPath(int rule, int initialCapacity) : base(rule, initialCapacity)
		{
		}

		/// <summary>
		/// Constructs a new <code>GeneralPath</code> object from an arbitrary
		/// <seealso cref="Shape"/> object.
		/// All of the initial geometry and the winding rule for this path are
		/// taken from the specified <code>Shape</code> object.
		/// </summary>
		/// <param name="s"> the specified <code>Shape</code> object
		/// @since 1.2 </param>
		public GeneralPath(Shape s) : base(s, null)
		{
		}

		internal GeneralPath(int windingRule, sbyte[] pointTypes, int numTypes, float[] pointCoords, int numCoords)
		{
			// used to construct from native

			this.WindingRule_Renamed = windingRule;
			this.PointTypes = pointTypes;
			this.NumTypes = numTypes;
			this.FloatCoords = pointCoords;
			this.NumCoords = numCoords;
		}

		/*
		 * JDK 1.6 serialVersionUID
		 */
		private const long SerialVersionUID = -8327096662768731142L;
	}

}
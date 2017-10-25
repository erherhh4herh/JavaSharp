using System;
using System.Diagnostics;

/*
 * Copyright (c) 2006, 2015, Oracle and/or its affiliates. All rights reserved.
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

	using Curve = sun.awt.geom.Curve;

	/// <summary>
	/// The {@code Path2D} class provides a simple, yet flexible
	/// shape which represents an arbitrary geometric path.
	/// It can fully represent any path which can be iterated by the
	/// <seealso cref="PathIterator"/> interface including all of its segment
	/// types and winding rules and it implements all of the
	/// basic hit testing methods of the <seealso cref="Shape"/> interface.
	/// <para>
	/// Use <seealso cref="Path2D.Float"/> when dealing with data that can be represented
	/// and used with floating point precision.  Use <seealso cref="Path2D.Double"/>
	/// for data that requires the accuracy or range of double precision.
	/// </para>
	/// <para>
	/// {@code Path2D} provides exactly those facilities required for
	/// basic construction and management of a geometric path and
	/// implementation of the above interfaces with little added
	/// interpretation.
	/// If it is useful to manipulate the interiors of closed
	/// geometric shapes beyond simple hit testing then the
	/// <seealso cref="Area"/> class provides additional capabilities
	/// specifically targeted at closed figures.
	/// While both classes nominally implement the {@code Shape}
	/// interface, they differ in purpose and together they provide
	/// two useful views of a geometric shape where {@code Path2D}
	/// deals primarily with a trajectory formed by path segments
	/// and {@code Area} deals more with interpretation and manipulation
	/// of enclosed regions of 2D geometric space.
	/// </para>
	/// <para>
	/// The <seealso cref="PathIterator"/> interface has more detailed descriptions
	/// of the types of segments that make up a path and the winding rules
	/// that control how to determine which regions are inside or outside
	/// the path.
	/// 
	/// @author Jim Graham
	/// @since 1.6
	/// </para>
	/// </summary>
	public abstract class Path2D : Shape, Cloneable
	{
		public abstract java.awt.geom.PathIterator GetPathIterator(AffineTransform at);
		public abstract java.awt.geom.Rectangle2D Bounds2D {get;}
		/// <summary>
		/// An even-odd winding rule for determining the interior of
		/// a path.
		/// </summary>
		/// <seealso cref= PathIterator#WIND_EVEN_ODD
		/// @since 1.6 </seealso>
		public const int WIND_EVEN_ODD = PathIterator_Fields.WIND_EVEN_ODD;

		/// <summary>
		/// A non-zero winding rule for determining the interior of a
		/// path.
		/// </summary>
		/// <seealso cref= PathIterator#WIND_NON_ZERO
		/// @since 1.6 </seealso>
		public const int WIND_NON_ZERO = PathIterator_Fields.WIND_NON_ZERO;

		// For code simplicity, copy these constants to our namespace
		// and cast them to byte constants for easy storage.
		private static readonly sbyte SEG_MOVETO = (sbyte) PathIterator_Fields.SEG_MOVETO;
		private static readonly sbyte SEG_LINETO = (sbyte) PathIterator_Fields.SEG_LINETO;
		private static readonly sbyte SEG_QUADTO = (sbyte) PathIterator_Fields.SEG_QUADTO;
		private static readonly sbyte SEG_CUBICTO = (sbyte) PathIterator_Fields.SEG_CUBICTO;
		private static readonly sbyte SEG_CLOSE = (sbyte) PathIterator_Fields.SEG_CLOSE;

		[NonSerialized]
		internal sbyte[] PointTypes;
		[NonSerialized]
		internal int NumTypes;
		[NonSerialized]
		internal int NumCoords;
		[NonSerialized]
		internal int WindingRule_Renamed;

		internal const int INIT_SIZE = 20;
		internal const int EXPAND_MAX = 500;
		internal static readonly int EXPAND_MAX_COORDS = EXPAND_MAX * 2;
		internal const int EXPAND_MIN = 10; // ensure > 6 (cubics)

		/// <summary>
		/// Constructs a new empty {@code Path2D} object.
		/// It is assumed that the package sibling subclass that is
		/// defaulting to this constructor will fill in all values.
		/// 
		/// @since 1.6
		/// </summary>
		/* private protected */
		internal Path2D()
		{
		}

		/// <summary>
		/// Constructs a new {@code Path2D} object from the given
		/// specified initial values.
		/// This method is only intended for internal use and should
		/// not be made public if the other constructors for this class
		/// are ever exposed.
		/// </summary>
		/// <param name="rule"> the winding rule </param>
		/// <param name="initialTypes"> the size to make the initial array to
		///                     store the path segment types
		/// @since 1.6 </param>
		/* private protected */
		internal Path2D(int rule, int initialTypes)
		{
			WindingRule = rule;
			this.PointTypes = new sbyte[initialTypes];
		}

		internal abstract float[] CloneCoordsFloat(AffineTransform at);
		internal abstract double[] CloneCoordsDouble(AffineTransform at);
		internal abstract void Append(float x, float y);
		internal abstract void Append(double x, double y);
		internal abstract Point2D GetPoint(int coordindex);
		internal abstract void NeedRoom(bool needMove, int newCoords);
		internal abstract int PointCrossings(double px, double py);
		internal abstract int RectCrossings(double rxmin, double rymin, double rxmax, double rymax);

		internal static sbyte[] ExpandPointTypes(sbyte[] oldPointTypes, int needed)
		{
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final int oldSize = oldPointTypes.length;
			int oldSize = oldPointTypes.Length;
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final int newSizeMin = oldSize + needed;
			int newSizeMin = oldSize + needed;
			if (newSizeMin < oldSize)
			{
				// hard overflow failure - we can't even accommodate
				// new items without overflowing
				throw new ArrayIndexOutOfBoundsException("pointTypes exceeds maximum capacity !");
			}
			// growth algorithm computation
			int grow = oldSize;
			if (grow > EXPAND_MAX)
			{
				grow = System.Math.Max(EXPAND_MAX, oldSize >> 3); // 1/8th min
			}
			else if (grow < EXPAND_MIN)
			{
				grow = EXPAND_MIN;
			}
			Debug.Assert(grow > 0);

			int newSize = oldSize + grow;
			if (newSize < newSizeMin)
			{
				// overflow in growth algorithm computation
				newSize = Integer.MaxValue;
			}
			while (true)
			{
				try
				{
					// try allocating the larger array
					return Arrays.CopyOf(oldPointTypes, newSize);
				}
				catch (OutOfMemoryError oome)
				{
					if (newSize == newSizeMin)
					{
						throw oome;
					}
				}
				newSize = newSizeMin + (newSize - newSizeMin) / 2;
			}
		}

		/// <summary>
		/// The {@code Float} class defines a geometric path with
		/// coordinates stored in single precision floating point.
		/// 
		/// @since 1.6
		/// </summary>
		[Serializable]
		public class Float : Path2D
		{
			[NonSerialized]
			internal float[] FloatCoords;

			/// <summary>
			/// Constructs a new empty single precision {@code Path2D} object
			/// with a default winding rule of <seealso cref="#WIND_NON_ZERO"/>.
			/// 
			/// @since 1.6
			/// </summary>
			public Float() : this(WIND_NON_ZERO, INIT_SIZE)
			{
			}

			/// <summary>
			/// Constructs a new empty single precision {@code Path2D} object
			/// with the specified winding rule to control operations that
			/// require the interior of the path to be defined.
			/// </summary>
			/// <param name="rule"> the winding rule </param>
			/// <seealso cref= #WIND_EVEN_ODD </seealso>
			/// <seealso cref= #WIND_NON_ZERO
			/// @since 1.6 </seealso>
			public Float(int rule) : this(rule, INIT_SIZE)
			{
			}

			/// <summary>
			/// Constructs a new empty single precision {@code Path2D} object
			/// with the specified winding rule and the specified initial
			/// capacity to store path segments.
			/// This number is an initial guess as to how many path segments
			/// will be added to the path, but the storage is expanded as
			/// needed to store whatever path segments are added.
			/// </summary>
			/// <param name="rule"> the winding rule </param>
			/// <param name="initialCapacity"> the estimate for the number of path segments
			///                        in the path </param>
			/// <seealso cref= #WIND_EVEN_ODD </seealso>
			/// <seealso cref= #WIND_NON_ZERO
			/// @since 1.6 </seealso>
			public Float(int rule, int initialCapacity) : base(rule, initialCapacity)
			{
				FloatCoords = new float[initialCapacity * 2];
			}

			/// <summary>
			/// Constructs a new single precision {@code Path2D} object
			/// from an arbitrary <seealso cref="Shape"/> object.
			/// All of the initial geometry and the winding rule for this path are
			/// taken from the specified {@code Shape} object.
			/// </summary>
			/// <param name="s"> the specified {@code Shape} object
			/// @since 1.6 </param>
			public Float(Shape s) : this(s, null)
			{
			}

			/// <summary>
			/// Constructs a new single precision {@code Path2D} object
			/// from an arbitrary <seealso cref="Shape"/> object, transformed by an
			/// <seealso cref="AffineTransform"/> object.
			/// All of the initial geometry and the winding rule for this path are
			/// taken from the specified {@code Shape} object and transformed
			/// by the specified {@code AffineTransform} object.
			/// </summary>
			/// <param name="s"> the specified {@code Shape} object </param>
			/// <param name="at"> the specified {@code AffineTransform} object
			/// @since 1.6 </param>
			public Float(Shape s, AffineTransform at)
			{
				if (s is Path2D)
				{
					Path2D p2d = (Path2D) s;
					WindingRule = p2d.WindingRule_Renamed;
					this.NumTypes = p2d.NumTypes;
					// trim arrays:
					this.PointTypes = Arrays.CopyOf(p2d.PointTypes, p2d.NumTypes);
					this.NumCoords = p2d.NumCoords;
					this.FloatCoords = p2d.CloneCoordsFloat(at);
				}
				else
				{
					PathIterator pi = s.GetPathIterator(at);
					WindingRule = pi.WindingRule;
					this.PointTypes = new sbyte[INIT_SIZE];
					this.FloatCoords = new float[INIT_SIZE * 2];
					Append(pi, false);
				}
			}

			internal override float[] CloneCoordsFloat(AffineTransform at)
			{
				// trim arrays:
				float[] ret;
				if (at == null)
				{
					ret = Arrays.CopyOf(FloatCoords, NumCoords);
				}
				else
				{
					ret = new float[NumCoords];
					at.Transform(FloatCoords, 0, ret, 0, NumCoords / 2);
				}
				return ret;
			}

			internal override double[] CloneCoordsDouble(AffineTransform at)
			{
				// trim arrays:
				double[] ret = new double[NumCoords];
				if (at == null)
				{
					for (int i = 0; i < NumCoords; i++)
					{
						ret[i] = FloatCoords[i];
					}
				}
				else
				{
					at.Transform(FloatCoords, 0, ret, 0, NumCoords / 2);
				}
				return ret;
			}

			internal override void Append(float x, float y)
			{
				FloatCoords[NumCoords++] = x;
				FloatCoords[NumCoords++] = y;
			}

			internal override void Append(double x, double y)
			{
				FloatCoords[NumCoords++] = (float) x;
				FloatCoords[NumCoords++] = (float) y;
			}

			internal override Point2D GetPoint(int coordindex)
			{
				return new Point2D.Float(FloatCoords[coordindex], FloatCoords[coordindex + 1]);
			}

			internal override void NeedRoom(bool needMove, int newCoords)
			{
				if ((NumTypes == 0) && needMove)
				{
					throw new IllegalPathStateException("missing initial moveto " + "in path definition");
				}
				if (NumTypes >= PointTypes.Length)
				{
					PointTypes = ExpandPointTypes(PointTypes, 1);
				}
				if (NumCoords > (FloatCoords.Length - newCoords))
				{
					FloatCoords = ExpandCoords(FloatCoords, newCoords);
				}
			}

			internal static float[] ExpandCoords(float[] oldCoords, int needed)
			{
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final int oldSize = oldCoords.length;
				int oldSize = oldCoords.Length;
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final int newSizeMin = oldSize + needed;
				int newSizeMin = oldSize + needed;
				if (newSizeMin < oldSize)
				{
					// hard overflow failure - we can't even accommodate
					// new items without overflowing
					throw new ArrayIndexOutOfBoundsException("coords exceeds maximum capacity !");
				}
				// growth algorithm computation
				int grow = oldSize;
				if (grow > EXPAND_MAX_COORDS)
				{
					grow = System.Math.Max(EXPAND_MAX_COORDS, oldSize >> 3); // 1/8th min
				}
				else if (grow < EXPAND_MIN)
				{
					grow = EXPAND_MIN;
				}
				Debug.Assert(grow > needed);

				int newSize = oldSize + grow;
				if (newSize < newSizeMin)
				{
					// overflow in growth algorithm computation
					newSize = Integer.MaxValue;
				}
				while (true)
				{
					try
					{
						// try allocating the larger array
						return Arrays.CopyOf(oldCoords, newSize);
					}
					catch (OutOfMemoryError oome)
					{
						if (newSize == newSizeMin)
						{
							throw oome;
						}
					}
					newSize = newSizeMin + (newSize - newSizeMin) / 2;
				}
			}

			/// <summary>
			/// {@inheritDoc}
			/// @since 1.6
			/// </summary>
			public sealed override void MoveTo(double x, double y)
			{
				lock (this)
				{
					if (NumTypes > 0 && PointTypes[NumTypes - 1] == SEG_MOVETO)
					{
						FloatCoords[NumCoords - 2] = (float) x;
						FloatCoords[NumCoords - 1] = (float) y;
					}
					else
					{
						NeedRoom(false, 2);
						PointTypes[NumTypes++] = SEG_MOVETO;
						FloatCoords[NumCoords++] = (float) x;
						FloatCoords[NumCoords++] = (float) y;
					}
				}
			}

			/// <summary>
			/// Adds a point to the path by moving to the specified
			/// coordinates specified in float precision.
			/// <para>
			/// This method provides a single precision variant of
			/// the double precision {@code moveTo()} method on the
			/// base {@code Path2D} class.
			/// 
			/// </para>
			/// </summary>
			/// <param name="x"> the specified X coordinate </param>
			/// <param name="y"> the specified Y coordinate </param>
			/// <seealso cref= Path2D#moveTo
			/// @since 1.6 </seealso>
			public void MoveTo(float x, float y)
			{
				lock (this)
				{
					if (NumTypes > 0 && PointTypes[NumTypes - 1] == SEG_MOVETO)
					{
						FloatCoords[NumCoords - 2] = x;
						FloatCoords[NumCoords - 1] = y;
					}
					else
					{
						NeedRoom(false, 2);
						PointTypes[NumTypes++] = SEG_MOVETO;
						FloatCoords[NumCoords++] = x;
						FloatCoords[NumCoords++] = y;
					}
				}
			}

			/// <summary>
			/// {@inheritDoc}
			/// @since 1.6
			/// </summary>
			public sealed override void LineTo(double x, double y)
			{
				lock (this)
				{
					NeedRoom(true, 2);
					PointTypes[NumTypes++] = SEG_LINETO;
					FloatCoords[NumCoords++] = (float) x;
					FloatCoords[NumCoords++] = (float) y;
				}
			}

			/// <summary>
			/// Adds a point to the path by drawing a straight line from the
			/// current coordinates to the new specified coordinates
			/// specified in float precision.
			/// <para>
			/// This method provides a single precision variant of
			/// the double precision {@code lineTo()} method on the
			/// base {@code Path2D} class.
			/// 
			/// </para>
			/// </summary>
			/// <param name="x"> the specified X coordinate </param>
			/// <param name="y"> the specified Y coordinate </param>
			/// <seealso cref= Path2D#lineTo
			/// @since 1.6 </seealso>
			public void LineTo(float x, float y)
			{
				lock (this)
				{
					NeedRoom(true, 2);
					PointTypes[NumTypes++] = SEG_LINETO;
					FloatCoords[NumCoords++] = x;
					FloatCoords[NumCoords++] = y;
				}
			}

			/// <summary>
			/// {@inheritDoc}
			/// @since 1.6
			/// </summary>
			public sealed override void QuadTo(double x1, double y1, double x2, double y2)
			{
				lock (this)
				{
					NeedRoom(true, 4);
					PointTypes[NumTypes++] = SEG_QUADTO;
					FloatCoords[NumCoords++] = (float) x1;
					FloatCoords[NumCoords++] = (float) y1;
					FloatCoords[NumCoords++] = (float) x2;
					FloatCoords[NumCoords++] = (float) y2;
				}
			}

			/// <summary>
			/// Adds a curved segment, defined by two new points, to the path by
			/// drawing a Quadratic curve that intersects both the current
			/// coordinates and the specified coordinates {@code (x2,y2)},
			/// using the specified point {@code (x1,y1)} as a quadratic
			/// parametric control point.
			/// All coordinates are specified in float precision.
			/// <para>
			/// This method provides a single precision variant of
			/// the double precision {@code quadTo()} method on the
			/// base {@code Path2D} class.
			/// 
			/// </para>
			/// </summary>
			/// <param name="x1"> the X coordinate of the quadratic control point </param>
			/// <param name="y1"> the Y coordinate of the quadratic control point </param>
			/// <param name="x2"> the X coordinate of the final end point </param>
			/// <param name="y2"> the Y coordinate of the final end point </param>
			/// <seealso cref= Path2D#quadTo
			/// @since 1.6 </seealso>
			public void QuadTo(float x1, float y1, float x2, float y2)
			{
				lock (this)
				{
					NeedRoom(true, 4);
					PointTypes[NumTypes++] = SEG_QUADTO;
					FloatCoords[NumCoords++] = x1;
					FloatCoords[NumCoords++] = y1;
					FloatCoords[NumCoords++] = x2;
					FloatCoords[NumCoords++] = y2;
				}
			}

			/// <summary>
			/// {@inheritDoc}
			/// @since 1.6
			/// </summary>
			public sealed override void CurveTo(double x1, double y1, double x2, double y2, double x3, double y3)
			{
				lock (this)
				{
					NeedRoom(true, 6);
					PointTypes[NumTypes++] = SEG_CUBICTO;
					FloatCoords[NumCoords++] = (float) x1;
					FloatCoords[NumCoords++] = (float) y1;
					FloatCoords[NumCoords++] = (float) x2;
					FloatCoords[NumCoords++] = (float) y2;
					FloatCoords[NumCoords++] = (float) x3;
					FloatCoords[NumCoords++] = (float) y3;
				}
			}

			/// <summary>
			/// Adds a curved segment, defined by three new points, to the path by
			/// drawing a B&eacute;zier curve that intersects both the current
			/// coordinates and the specified coordinates {@code (x3,y3)},
			/// using the specified points {@code (x1,y1)} and {@code (x2,y2)} as
			/// B&eacute;zier control points.
			/// All coordinates are specified in float precision.
			/// <para>
			/// This method provides a single precision variant of
			/// the double precision {@code curveTo()} method on the
			/// base {@code Path2D} class.
			/// 
			/// </para>
			/// </summary>
			/// <param name="x1"> the X coordinate of the first B&eacute;zier control point </param>
			/// <param name="y1"> the Y coordinate of the first B&eacute;zier control point </param>
			/// <param name="x2"> the X coordinate of the second B&eacute;zier control point </param>
			/// <param name="y2"> the Y coordinate of the second B&eacute;zier control point </param>
			/// <param name="x3"> the X coordinate of the final end point </param>
			/// <param name="y3"> the Y coordinate of the final end point </param>
			/// <seealso cref= Path2D#curveTo
			/// @since 1.6 </seealso>
			public void CurveTo(float x1, float y1, float x2, float y2, float x3, float y3)
			{
				lock (this)
				{
					NeedRoom(true, 6);
					PointTypes[NumTypes++] = SEG_CUBICTO;
					FloatCoords[NumCoords++] = x1;
					FloatCoords[NumCoords++] = y1;
					FloatCoords[NumCoords++] = x2;
					FloatCoords[NumCoords++] = y2;
					FloatCoords[NumCoords++] = x3;
					FloatCoords[NumCoords++] = y3;
				}
			}

			internal override int PointCrossings(double px, double py)
			{
				if (NumTypes == 0)
				{
					return 0;
				}
				double movx, movy, curx, cury, endx, endy;
				float[] coords = FloatCoords;
				curx = movx = coords[0];
				cury = movy = coords[1];
				int crossings = 0;
				int ci = 2;
				for (int i = 1; i < NumTypes; i++)
				{
					switch (PointTypes[i])
					{
					case PathIterator_Fields.SEG_MOVETO:
						if (cury != movy)
						{
							crossings += Curve.pointCrossingsForLine(px, py, curx, cury, movx, movy);
						}
						movx = curx = coords[ci++];
						movy = cury = coords[ci++];
						break;
					case PathIterator_Fields.SEG_LINETO:
						crossings += Curve.pointCrossingsForLine(px, py, curx, cury, endx = coords[ci++], endy = coords[ci++]);
						curx = endx;
						cury = endy;
						break;
					case PathIterator_Fields.SEG_QUADTO:
						crossings += Curve.pointCrossingsForQuad(px, py, curx, cury, coords[ci++], coords[ci++], endx = coords[ci++], endy = coords[ci++], 0);
						curx = endx;
						cury = endy;
						break;
				case PathIterator_Fields.SEG_CUBICTO:
						crossings += Curve.pointCrossingsForCubic(px, py, curx, cury, coords[ci++], coords[ci++], coords[ci++], coords[ci++], endx = coords[ci++], endy = coords[ci++], 0);
						curx = endx;
						cury = endy;
						break;
					case PathIterator_Fields.SEG_CLOSE:
						if (cury != movy)
						{
							crossings += Curve.pointCrossingsForLine(px, py, curx, cury, movx, movy);
						}
						curx = movx;
						cury = movy;
						break;
					}
				}
				if (cury != movy)
				{
					crossings += Curve.pointCrossingsForLine(px, py, curx, cury, movx, movy);
				}
				return crossings;
			}

			internal override int RectCrossings(double rxmin, double rymin, double rxmax, double rymax)
			{
				if (NumTypes == 0)
				{
					return 0;
				}
				float[] coords = FloatCoords;
				double curx, cury, movx, movy, endx, endy;
				curx = movx = coords[0];
				cury = movy = coords[1];
				int crossings = 0;
				int ci = 2;
				for (int i = 1; crossings != Curve.RECT_INTERSECTS && i < NumTypes; i++)
				{
					switch (PointTypes[i])
					{
					case PathIterator_Fields.SEG_MOVETO:
						if (curx != movx || cury != movy)
						{
							crossings = Curve.rectCrossingsForLine(crossings, rxmin, rymin, rxmax, rymax, curx, cury, movx, movy);
						}
						// Count should always be a multiple of 2 here.
						// assert((crossings & 1) != 0);
						movx = curx = coords[ci++];
						movy = cury = coords[ci++];
						break;
					case PathIterator_Fields.SEG_LINETO:
						crossings = Curve.rectCrossingsForLine(crossings, rxmin, rymin, rxmax, rymax, curx, cury, endx = coords[ci++], endy = coords[ci++]);
						curx = endx;
						cury = endy;
						break;
					case PathIterator_Fields.SEG_QUADTO:
						crossings = Curve.rectCrossingsForQuad(crossings, rxmin, rymin, rxmax, rymax, curx, cury, coords[ci++], coords[ci++], endx = coords[ci++], endy = coords[ci++], 0);
						curx = endx;
						cury = endy;
						break;
					case PathIterator_Fields.SEG_CUBICTO:
						crossings = Curve.rectCrossingsForCubic(crossings, rxmin, rymin, rxmax, rymax, curx, cury, coords[ci++], coords[ci++], coords[ci++], coords[ci++], endx = coords[ci++], endy = coords[ci++], 0);
						curx = endx;
						cury = endy;
						break;
					case PathIterator_Fields.SEG_CLOSE:
						if (curx != movx || cury != movy)
						{
							crossings = Curve.rectCrossingsForLine(crossings, rxmin, rymin, rxmax, rymax, curx, cury, movx, movy);
						}
						curx = movx;
						cury = movy;
						// Count should always be a multiple of 2 here.
						// assert((crossings & 1) != 0);
						break;
					}
				}
				if (crossings != Curve.RECT_INTERSECTS && (curx != movx || cury != movy))
				{
					crossings = Curve.rectCrossingsForLine(crossings, rxmin, rymin, rxmax, rymax, curx, cury, movx, movy);
				}
				// Count should always be a multiple of 2 here.
				// assert((crossings & 1) != 0);
				return crossings;
			}

			/// <summary>
			/// {@inheritDoc}
			/// @since 1.6
			/// </summary>
			public sealed override void Append(PathIterator pi, bool connect)
			{
				float[] coords = new float[6];
				while (!pi.Done)
				{
					switch (pi.CurrentSegment(coords))
					{
					case SEG_MOVETO:
						if (!connect || NumTypes < 1 || NumCoords < 1)
						{
							MoveTo(coords[0], coords[1]);
							break;
						}
						if (PointTypes[NumTypes - 1] != SEG_CLOSE && FloatCoords[NumCoords - 2] == coords[0] && FloatCoords[NumCoords - 1] == coords[1])
						{
							// Collapse out initial moveto/lineto
							break;
						}
						LineTo(coords[0], coords[1]);
						break;
					case SEG_LINETO:
						LineTo(coords[0], coords[1]);
						break;
					case SEG_QUADTO:
						QuadTo(coords[0], coords[1], coords[2], coords[3]);
						break;
					case SEG_CUBICTO:
						CurveTo(coords[0], coords[1], coords[2], coords[3], coords[4], coords[5]);
						break;
					case SEG_CLOSE:
						ClosePath();
						break;
					}
					pi.Next();
					connect = false;
				}
			}

			/// <summary>
			/// {@inheritDoc}
			/// @since 1.6
			/// </summary>
			public sealed override void Transform(AffineTransform at)
			{
				at.Transform(FloatCoords, 0, FloatCoords, 0, NumCoords / 2);
			}

			/// <summary>
			/// {@inheritDoc}
			/// @since 1.6
			/// </summary>
			public sealed override Rectangle2D Bounds2D
			{
				get
				{
					lock (this)
					{
						float x1, y1, x2, y2;
						int i = NumCoords;
						if (i > 0)
						{
							y1 = y2 = FloatCoords[--i];
							x1 = x2 = FloatCoords[--i];
							while (i > 0)
							{
								float y = FloatCoords[--i];
								float x = FloatCoords[--i];
								if (x < x1)
								{
									x1 = x;
								}
								if (y < y1)
								{
									y1 = y;
								}
								if (x > x2)
								{
									x2 = x;
								}
								if (y > y2)
								{
									y2 = y;
								}
							}
						}
						else
						{
							x1 = y1 = x2 = y2 = 0.0f;
						}
						return new Rectangle2D.Float(x1, y1, x2 - x1, y2 - y1);
					}
				}
			}

			/// <summary>
			/// {@inheritDoc}
			/// <para>
			/// The iterator for this class is not multi-threaded safe,
			/// which means that the {@code Path2D} class does not
			/// guarantee that modifications to the geometry of this
			/// {@code Path2D} object do not affect any iterations of
			/// that geometry that are already in process.
			/// 
			/// @since 1.6
			/// </para>
			/// </summary>
			public sealed override PathIterator GetPathIterator(AffineTransform at)
			{
				if (at == null)
				{
					return new CopyIterator(this);
				}
				else
				{
					return new TxIterator(this, at);
				}
			}

			/// <summary>
			/// Creates a new object of the same class as this object.
			/// </summary>
			/// <returns>     a clone of this instance. </returns>
			/// <exception cref="OutOfMemoryError">    if there is not enough memory. </exception>
			/// <seealso cref=        java.lang.Cloneable
			/// @since      1.6 </seealso>
			public sealed override Object Clone()
			{
				// Note: It would be nice to have this return Path2D
				// but one of our subclasses (GeneralPath) needs to
				// offer "public Object clone()" for backwards
				// compatibility so we cannot restrict it further.
				// REMIND: Can we do both somehow?
				if (this is GeneralPath)
				{
					return new GeneralPath(this);
				}
				else
				{
					return new Path2D.Float(this);
				}
			}

			/*
			 * JDK 1.6 serialVersionUID
			 */
			internal const long SerialVersionUID = 6990832515060788886L;

			/// <summary>
			/// Writes the default serializable fields to the
			/// {@code ObjectOutputStream} followed by an explicit
			/// serialization of the path segments stored in this
			/// path.
			/// 
			/// @serialData
			/// <a name="Path2DSerialData"><!-- --></a>
			/// <ol>
			/// <li>The default serializable fields.
			/// There are no default serializable fields as of 1.6.
			/// <li>followed by
			/// a byte indicating the storage type of the original object
			/// as a hint (SERIAL_STORAGE_FLT_ARRAY)
			/// <li>followed by
			/// an integer indicating the number of path segments to follow (NP)
			/// or -1 to indicate an unknown number of path segments follows
			/// <li>followed by
			/// an integer indicating the total number of coordinates to follow (NC)
			/// or -1 to indicate an unknown number of coordinates follows
			/// (NC should always be even since coordinates always appear in pairs
			///  representing an x,y pair)
			/// <li>followed by
			/// a byte indicating the winding rule
			/// (<seealso cref="#WIND_EVEN_ODD WIND_EVEN_ODD"/> or
			///  <seealso cref="#WIND_NON_ZERO WIND_NON_ZERO"/>)
			/// <li>followed by
			/// {@code NP} (or unlimited if {@code NP < 0}) sets of values consisting of
			/// a single byte indicating a path segment type
			/// followed by one or more pairs of float or double
			/// values representing the coordinates of the path segment
			/// <li>followed by
			/// a byte indicating the end of the path (SERIAL_PATH_END).
			/// </ol>
			/// <para>
			/// The following byte value constants are used in the serialized form
			/// of {@code Path2D} objects:
			/// <table>
			/// <tr>
			/// <th>Constant Name</th>
			/// <th>Byte Value</th>
			/// <th>Followed by</th>
			/// <th>Description</th>
			/// </tr>
			/// <tr>
			/// <td>{@code SERIAL_STORAGE_FLT_ARRAY}</td>
			/// <td>0x30</td>
			/// <td></td>
			/// <td>A hint that the original {@code Path2D} object stored
			/// the coordinates in a Java array of floats.</td>
			/// </tr>
			/// <tr>
			/// <td>{@code SERIAL_STORAGE_DBL_ARRAY}</td>
			/// <td>0x31</td>
			/// <td></td>
			/// <td>A hint that the original {@code Path2D} object stored
			/// the coordinates in a Java array of doubles.</td>
			/// </tr>
			/// <tr>
			/// <td>{@code SERIAL_SEG_FLT_MOVETO}</td>
			/// <td>0x40</td>
			/// <td>2 floats</td>
			/// <td>A <seealso cref="#moveTo moveTo"/> path segment follows.</td>
			/// </tr>
			/// <tr>
			/// <td>{@code SERIAL_SEG_FLT_LINETO}</td>
			/// <td>0x41</td>
			/// <td>2 floats</td>
			/// <td>A <seealso cref="#lineTo lineTo"/> path segment follows.</td>
			/// </tr>
			/// <tr>
			/// <td>{@code SERIAL_SEG_FLT_QUADTO}</td>
			/// <td>0x42</td>
			/// <td>4 floats</td>
			/// <td>A <seealso cref="#quadTo quadTo"/> path segment follows.</td>
			/// </tr>
			/// <tr>
			/// <td>{@code SERIAL_SEG_FLT_CUBICTO}</td>
			/// <td>0x43</td>
			/// <td>6 floats</td>
			/// <td>A <seealso cref="#curveTo curveTo"/> path segment follows.</td>
			/// </tr>
			/// <tr>
			/// <td>{@code SERIAL_SEG_DBL_MOVETO}</td>
			/// <td>0x50</td>
			/// <td>2 doubles</td>
			/// <td>A <seealso cref="#moveTo moveTo"/> path segment follows.</td>
			/// </tr>
			/// <tr>
			/// <td>{@code SERIAL_SEG_DBL_LINETO}</td>
			/// <td>0x51</td>
			/// <td>2 doubles</td>
			/// <td>A <seealso cref="#lineTo lineTo"/> path segment follows.</td>
			/// </tr>
			/// <tr>
			/// <td>{@code SERIAL_SEG_DBL_QUADTO}</td>
			/// <td>0x52</td>
			/// <td>4 doubles</td>
			/// <td>A <seealso cref="#curveTo curveTo"/> path segment follows.</td>
			/// </tr>
			/// <tr>
			/// <td>{@code SERIAL_SEG_DBL_CUBICTO}</td>
			/// <td>0x53</td>
			/// <td>6 doubles</td>
			/// <td>A <seealso cref="#curveTo curveTo"/> path segment follows.</td>
			/// </tr>
			/// <tr>
			/// <td>{@code SERIAL_SEG_CLOSE}</td>
			/// <td>0x60</td>
			/// <td></td>
			/// <td>A <seealso cref="#closePath closePath"/> path segment.</td>
			/// </tr>
			/// <tr>
			/// <td>{@code SERIAL_PATH_END}</td>
			/// <td>0x61</td>
			/// <td></td>
			/// <td>There are no more path segments following.</td>
			/// </table>
			/// 
			/// @since 1.6
			/// </para>
			/// </summary>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: private void writeObject(java.io.ObjectOutputStream s) throws java.io.IOException
			internal virtual void WriteObject(java.io.ObjectOutputStream s)
			{
				base.WriteObject(s, false);
			}

			/// <summary>
			/// Reads the default serializable fields from the
			/// {@code ObjectInputStream} followed by an explicit
			/// serialization of the path segments stored in this
			/// path.
			/// <para>
			/// There are no default serializable fields as of 1.6.
			/// </para>
			/// <para>
			/// The serial data for this object is described in the
			/// writeObject method.
			/// 
			/// @since 1.6
			/// </para>
			/// </summary>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: private void readObject(java.io.ObjectInputStream s) throws java.lang.ClassNotFoundException, java.io.IOException
			internal virtual void ReadObject(java.io.ObjectInputStream s)
			{
				base.ReadObject(s, false);
			}

			internal class CopyIterator : Path2D.Iterator
			{
				internal float[] FloatCoords;

				internal CopyIterator(Path2D.Float p2df) : base(p2df)
				{
					this.FloatCoords = p2df.FloatCoords;
				}

				public override int CurrentSegment(float[] coords)
				{
					int type = Path.PointTypes[TypeIdx];
					int numCoords = Curvecoords[type];
					if (numCoords > 0)
					{
						System.Array.Copy(FloatCoords, PointIdx, coords, 0, numCoords);
					}
					return type;
				}

				public override int CurrentSegment(double[] coords)
				{
					int type = Path.PointTypes[TypeIdx];
					int numCoords = Curvecoords[type];
					if (numCoords > 0)
					{
						for (int i = 0; i < numCoords; i++)
						{
							coords[i] = FloatCoords[PointIdx + i];
						}
					}
					return type;
				}
			}

			internal class TxIterator : Path2D.Iterator
			{
				internal float[] FloatCoords;
				internal AffineTransform Affine;

				internal TxIterator(Path2D.Float p2df, AffineTransform at) : base(p2df)
				{
					this.FloatCoords = p2df.FloatCoords;
					this.Affine = at;
				}

				public override int CurrentSegment(float[] coords)
				{
					int type = Path.PointTypes[TypeIdx];
					int numCoords = Curvecoords[type];
					if (numCoords > 0)
					{
						Affine.Transform(FloatCoords, PointIdx, coords, 0, numCoords / 2);
					}
					return type;
				}

				public override int CurrentSegment(double[] coords)
				{
					int type = Path.PointTypes[TypeIdx];
					int numCoords = Curvecoords[type];
					if (numCoords > 0)
					{
						Affine.Transform(FloatCoords, PointIdx, coords, 0, numCoords / 2);
					}
					return type;
				}
			}

		}

		/// <summary>
		/// The {@code Double} class defines a geometric path with
		/// coordinates stored in double precision floating point.
		/// 
		/// @since 1.6
		/// </summary>
		[Serializable]
		public class Double : Path2D
		{
			[NonSerialized]
			internal double[] DoubleCoords;

			/// <summary>
			/// Constructs a new empty double precision {@code Path2D} object
			/// with a default winding rule of <seealso cref="#WIND_NON_ZERO"/>.
			/// 
			/// @since 1.6
			/// </summary>
			public Double() : this(WIND_NON_ZERO, INIT_SIZE)
			{
			}

			/// <summary>
			/// Constructs a new empty double precision {@code Path2D} object
			/// with the specified winding rule to control operations that
			/// require the interior of the path to be defined.
			/// </summary>
			/// <param name="rule"> the winding rule </param>
			/// <seealso cref= #WIND_EVEN_ODD </seealso>
			/// <seealso cref= #WIND_NON_ZERO
			/// @since 1.6 </seealso>
			public Double(int rule) : this(rule, INIT_SIZE)
			{
			}

			/// <summary>
			/// Constructs a new empty double precision {@code Path2D} object
			/// with the specified winding rule and the specified initial
			/// capacity to store path segments.
			/// This number is an initial guess as to how many path segments
			/// are in the path, but the storage is expanded as needed to store
			/// whatever path segments are added to this path.
			/// </summary>
			/// <param name="rule"> the winding rule </param>
			/// <param name="initialCapacity"> the estimate for the number of path segments
			///                        in the path </param>
			/// <seealso cref= #WIND_EVEN_ODD </seealso>
			/// <seealso cref= #WIND_NON_ZERO
			/// @since 1.6 </seealso>
			public Double(int rule, int initialCapacity) : base(rule, initialCapacity)
			{
				DoubleCoords = new double[initialCapacity * 2];
			}

			/// <summary>
			/// Constructs a new double precision {@code Path2D} object
			/// from an arbitrary <seealso cref="Shape"/> object.
			/// All of the initial geometry and the winding rule for this path are
			/// taken from the specified {@code Shape} object.
			/// </summary>
			/// <param name="s"> the specified {@code Shape} object
			/// @since 1.6 </param>
			public Double(Shape s) : this(s, null)
			{
			}

			/// <summary>
			/// Constructs a new double precision {@code Path2D} object
			/// from an arbitrary <seealso cref="Shape"/> object, transformed by an
			/// <seealso cref="AffineTransform"/> object.
			/// All of the initial geometry and the winding rule for this path are
			/// taken from the specified {@code Shape} object and transformed
			/// by the specified {@code AffineTransform} object.
			/// </summary>
			/// <param name="s"> the specified {@code Shape} object </param>
			/// <param name="at"> the specified {@code AffineTransform} object
			/// @since 1.6 </param>
			public Double(Shape s, AffineTransform at)
			{
				if (s is Path2D)
				{
					Path2D p2d = (Path2D) s;
					WindingRule = p2d.WindingRule_Renamed;
					this.NumTypes = p2d.NumTypes;
					// trim arrays:
					this.PointTypes = Arrays.CopyOf(p2d.PointTypes, p2d.NumTypes);
					this.NumCoords = p2d.NumCoords;
					this.DoubleCoords = p2d.CloneCoordsDouble(at);
				}
				else
				{
					PathIterator pi = s.GetPathIterator(at);
					WindingRule = pi.WindingRule;
					this.PointTypes = new sbyte[INIT_SIZE];
					this.DoubleCoords = new double[INIT_SIZE * 2];
					Append(pi, false);
				}
			}

			internal override float[] CloneCoordsFloat(AffineTransform at)
			{
				// trim arrays:
				float[] ret = new float[NumCoords];
				if (at == null)
				{
					for (int i = 0; i < NumCoords; i++)
					{
						ret[i] = (float) DoubleCoords[i];
					}
				}
				else
				{
					at.Transform(DoubleCoords, 0, ret, 0, NumCoords / 2);
				}
				return ret;
			}

			internal override double[] CloneCoordsDouble(AffineTransform at)
			{
				// trim arrays:
				double[] ret;
				if (at == null)
				{
					ret = Arrays.CopyOf(DoubleCoords, NumCoords);
				}
				else
				{
					ret = new double[NumCoords];
					at.Transform(DoubleCoords, 0, ret, 0, NumCoords / 2);
				}
				return ret;
			}

			internal override void Append(float x, float y)
			{
				DoubleCoords[NumCoords++] = x;
				DoubleCoords[NumCoords++] = y;
			}

			internal override void Append(double x, double y)
			{
				DoubleCoords[NumCoords++] = x;
				DoubleCoords[NumCoords++] = y;
			}

			internal override Point2D GetPoint(int coordindex)
			{
				return new Point2D.Double(DoubleCoords[coordindex], DoubleCoords[coordindex + 1]);
			}

			internal override void NeedRoom(bool needMove, int newCoords)
			{
				if ((NumTypes == 0) && needMove)
				{
					throw new IllegalPathStateException("missing initial moveto " + "in path definition");
				}
				if (NumTypes >= PointTypes.Length)
				{
					PointTypes = ExpandPointTypes(PointTypes, 1);
				}
				if (NumCoords > (DoubleCoords.Length - newCoords))
				{
					DoubleCoords = ExpandCoords(DoubleCoords, newCoords);
				}
			}

			internal static double[] ExpandCoords(double[] oldCoords, int needed)
			{
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final int oldSize = oldCoords.length;
				int oldSize = oldCoords.Length;
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final int newSizeMin = oldSize + needed;
				int newSizeMin = oldSize + needed;
				if (newSizeMin < oldSize)
				{
					// hard overflow failure - we can't even accommodate
					// new items without overflowing
					throw new ArrayIndexOutOfBoundsException("coords exceeds maximum capacity !");
				}
				// growth algorithm computation
				int grow = oldSize;
				if (grow > EXPAND_MAX_COORDS)
				{
					grow = System.Math.Max(EXPAND_MAX_COORDS, oldSize >> 3); // 1/8th min
				}
				else if (grow < EXPAND_MIN)
				{
					grow = EXPAND_MIN;
				}
				Debug.Assert(grow > needed);

				int newSize = oldSize + grow;
				if (newSize < newSizeMin)
				{
					// overflow in growth algorithm computation
					newSize = Integer.MaxValue;
				}
				while (true)
				{
					try
					{
						// try allocating the larger array
						return Arrays.CopyOf(oldCoords, newSize);
					}
					catch (OutOfMemoryError oome)
					{
						if (newSize == newSizeMin)
						{
							throw oome;
						}
					}
					newSize = newSizeMin + (newSize - newSizeMin) / 2;
				}
			}

			/// <summary>
			/// {@inheritDoc}
			/// @since 1.6
			/// </summary>
			public sealed override void MoveTo(double x, double y)
			{
				lock (this)
				{
					if (NumTypes > 0 && PointTypes[NumTypes - 1] == SEG_MOVETO)
					{
						DoubleCoords[NumCoords - 2] = x;
						DoubleCoords[NumCoords - 1] = y;
					}
					else
					{
						NeedRoom(false, 2);
						PointTypes[NumTypes++] = SEG_MOVETO;
						DoubleCoords[NumCoords++] = x;
						DoubleCoords[NumCoords++] = y;
					}
				}
			}

			/// <summary>
			/// {@inheritDoc}
			/// @since 1.6
			/// </summary>
			public sealed override void LineTo(double x, double y)
			{
				lock (this)
				{
					NeedRoom(true, 2);
					PointTypes[NumTypes++] = SEG_LINETO;
					DoubleCoords[NumCoords++] = x;
					DoubleCoords[NumCoords++] = y;
				}
			}

			/// <summary>
			/// {@inheritDoc}
			/// @since 1.6
			/// </summary>
			public sealed override void QuadTo(double x1, double y1, double x2, double y2)
			{
				lock (this)
				{
					NeedRoom(true, 4);
					PointTypes[NumTypes++] = SEG_QUADTO;
					DoubleCoords[NumCoords++] = x1;
					DoubleCoords[NumCoords++] = y1;
					DoubleCoords[NumCoords++] = x2;
					DoubleCoords[NumCoords++] = y2;
				}
			}

			/// <summary>
			/// {@inheritDoc}
			/// @since 1.6
			/// </summary>
			public sealed override void CurveTo(double x1, double y1, double x2, double y2, double x3, double y3)
			{
				lock (this)
				{
					NeedRoom(true, 6);
					PointTypes[NumTypes++] = SEG_CUBICTO;
					DoubleCoords[NumCoords++] = x1;
					DoubleCoords[NumCoords++] = y1;
					DoubleCoords[NumCoords++] = x2;
					DoubleCoords[NumCoords++] = y2;
					DoubleCoords[NumCoords++] = x3;
					DoubleCoords[NumCoords++] = y3;
				}
			}

			internal override int PointCrossings(double px, double py)
			{
				if (NumTypes == 0)
				{
					return 0;
				}
				double movx, movy, curx, cury, endx, endy;
				double[] coords = DoubleCoords;
				curx = movx = coords[0];
				cury = movy = coords[1];
				int crossings = 0;
				int ci = 2;
				for (int i = 1; i < NumTypes; i++)
				{
					switch (PointTypes[i])
					{
					case PathIterator_Fields.SEG_MOVETO:
						if (cury != movy)
						{
							crossings += Curve.pointCrossingsForLine(px, py, curx, cury, movx, movy);
						}
						movx = curx = coords[ci++];
						movy = cury = coords[ci++];
						break;
					case PathIterator_Fields.SEG_LINETO:
						crossings += Curve.pointCrossingsForLine(px, py, curx, cury, endx = coords[ci++], endy = coords[ci++]);
						curx = endx;
						cury = endy;
						break;
					case PathIterator_Fields.SEG_QUADTO:
						crossings += Curve.pointCrossingsForQuad(px, py, curx, cury, coords[ci++], coords[ci++], endx = coords[ci++], endy = coords[ci++], 0);
						curx = endx;
						cury = endy;
						break;
				case PathIterator_Fields.SEG_CUBICTO:
						crossings += Curve.pointCrossingsForCubic(px, py, curx, cury, coords[ci++], coords[ci++], coords[ci++], coords[ci++], endx = coords[ci++], endy = coords[ci++], 0);
						curx = endx;
						cury = endy;
						break;
					case PathIterator_Fields.SEG_CLOSE:
						if (cury != movy)
						{
							crossings += Curve.pointCrossingsForLine(px, py, curx, cury, movx, movy);
						}
						curx = movx;
						cury = movy;
						break;
					}
				}
				if (cury != movy)
				{
					crossings += Curve.pointCrossingsForLine(px, py, curx, cury, movx, movy);
				}
				return crossings;
			}

			internal override int RectCrossings(double rxmin, double rymin, double rxmax, double rymax)
			{
				if (NumTypes == 0)
				{
					return 0;
				}
				double[] coords = DoubleCoords;
				double curx, cury, movx, movy, endx, endy;
				curx = movx = coords[0];
				cury = movy = coords[1];
				int crossings = 0;
				int ci = 2;
				for (int i = 1; crossings != Curve.RECT_INTERSECTS && i < NumTypes; i++)
				{
					switch (PointTypes[i])
					{
					case PathIterator_Fields.SEG_MOVETO:
						if (curx != movx || cury != movy)
						{
							crossings = Curve.rectCrossingsForLine(crossings, rxmin, rymin, rxmax, rymax, curx, cury, movx, movy);
						}
						// Count should always be a multiple of 2 here.
						// assert((crossings & 1) != 0);
						movx = curx = coords[ci++];
						movy = cury = coords[ci++];
						break;
					case PathIterator_Fields.SEG_LINETO:
						endx = coords[ci++];
						endy = coords[ci++];
						crossings = Curve.rectCrossingsForLine(crossings, rxmin, rymin, rxmax, rymax, curx, cury, endx, endy);
						curx = endx;
						cury = endy;
						break;
					case PathIterator_Fields.SEG_QUADTO:
						crossings = Curve.rectCrossingsForQuad(crossings, rxmin, rymin, rxmax, rymax, curx, cury, coords[ci++], coords[ci++], endx = coords[ci++], endy = coords[ci++], 0);
						curx = endx;
						cury = endy;
						break;
					case PathIterator_Fields.SEG_CUBICTO:
						crossings = Curve.rectCrossingsForCubic(crossings, rxmin, rymin, rxmax, rymax, curx, cury, coords[ci++], coords[ci++], coords[ci++], coords[ci++], endx = coords[ci++], endy = coords[ci++], 0);
						curx = endx;
						cury = endy;
						break;
					case PathIterator_Fields.SEG_CLOSE:
						if (curx != movx || cury != movy)
						{
							crossings = Curve.rectCrossingsForLine(crossings, rxmin, rymin, rxmax, rymax, curx, cury, movx, movy);
						}
						curx = movx;
						cury = movy;
						// Count should always be a multiple of 2 here.
						// assert((crossings & 1) != 0);
						break;
					}
				}
				if (crossings != Curve.RECT_INTERSECTS && (curx != movx || cury != movy))
				{
					crossings = Curve.rectCrossingsForLine(crossings, rxmin, rymin, rxmax, rymax, curx, cury, movx, movy);
				}
				// Count should always be a multiple of 2 here.
				// assert((crossings & 1) != 0);
				return crossings;
			}

			/// <summary>
			/// {@inheritDoc}
			/// @since 1.6
			/// </summary>
			public sealed override void Append(PathIterator pi, bool connect)
			{
				double[] coords = new double[6];
				while (!pi.Done)
				{
					switch (pi.CurrentSegment(coords))
					{
					case SEG_MOVETO:
						if (!connect || NumTypes < 1 || NumCoords < 1)
						{
							MoveTo(coords[0], coords[1]);
							break;
						}
						if (PointTypes[NumTypes - 1] != SEG_CLOSE && DoubleCoords[NumCoords - 2] == coords[0] && DoubleCoords[NumCoords - 1] == coords[1])
						{
							// Collapse out initial moveto/lineto
							break;
						}
						LineTo(coords[0], coords[1]);
						break;
					case SEG_LINETO:
						LineTo(coords[0], coords[1]);
						break;
					case SEG_QUADTO:
						QuadTo(coords[0], coords[1], coords[2], coords[3]);
						break;
					case SEG_CUBICTO:
						CurveTo(coords[0], coords[1], coords[2], coords[3], coords[4], coords[5]);
						break;
					case SEG_CLOSE:
						ClosePath();
						break;
					}
					pi.Next();
					connect = false;
				}
			}

			/// <summary>
			/// {@inheritDoc}
			/// @since 1.6
			/// </summary>
			public sealed override void Transform(AffineTransform at)
			{
				at.Transform(DoubleCoords, 0, DoubleCoords, 0, NumCoords / 2);
			}

			/// <summary>
			/// {@inheritDoc}
			/// @since 1.6
			/// </summary>
			public sealed override Rectangle2D Bounds2D
			{
				get
				{
					lock (this)
					{
						double x1, y1, x2, y2;
						int i = NumCoords;
						if (i > 0)
						{
							y1 = y2 = DoubleCoords[--i];
							x1 = x2 = DoubleCoords[--i];
							while (i > 0)
							{
								double y = DoubleCoords[--i];
								double x = DoubleCoords[--i];
								if (x < x1)
								{
									x1 = x;
								}
								if (y < y1)
								{
									y1 = y;
								}
								if (x > x2)
								{
									x2 = x;
								}
								if (y > y2)
								{
									y2 = y;
								}
							}
						}
						else
						{
							x1 = y1 = x2 = y2 = 0.0;
						}
						return new Rectangle2D.Double(x1, y1, x2 - x1, y2 - y1);
					}
				}
			}

			/// <summary>
			/// {@inheritDoc}
			/// <para>
			/// The iterator for this class is not multi-threaded safe,
			/// which means that the {@code Path2D} class does not
			/// guarantee that modifications to the geometry of this
			/// {@code Path2D} object do not affect any iterations of
			/// that geometry that are already in process.
			/// 
			/// </para>
			/// </summary>
			/// <param name="at"> an {@code AffineTransform} </param>
			/// <returns> a new {@code PathIterator} that iterates along the boundary
			///         of this {@code Shape} and provides access to the geometry
			///         of this {@code Shape}'s outline
			/// @since 1.6 </returns>
			public sealed override PathIterator GetPathIterator(AffineTransform at)
			{
				if (at == null)
				{
					return new CopyIterator(this);
				}
				else
				{
					return new TxIterator(this, at);
				}
			}

			/// <summary>
			/// Creates a new object of the same class as this object.
			/// </summary>
			/// <returns>     a clone of this instance. </returns>
			/// <exception cref="OutOfMemoryError">    if there is not enough memory. </exception>
			/// <seealso cref=        java.lang.Cloneable
			/// @since      1.6 </seealso>
			public sealed override Object Clone()
			{
				// Note: It would be nice to have this return Path2D
				// but one of our subclasses (GeneralPath) needs to
				// offer "public Object clone()" for backwards
				// compatibility so we cannot restrict it further.
				// REMIND: Can we do both somehow?
				return new Path2D.Double(this);
			}

			/*
			 * JDK 1.6 serialVersionUID
			 */
			internal const long SerialVersionUID = 1826762518450014216L;

			/// <summary>
			/// Writes the default serializable fields to the
			/// {@code ObjectOutputStream} followed by an explicit
			/// serialization of the path segments stored in this
			/// path.
			/// 
			/// @serialData
			/// <a name="Path2DSerialData"><!-- --></a>
			/// <ol>
			/// <li>The default serializable fields.
			/// There are no default serializable fields as of 1.6.
			/// <li>followed by
			/// a byte indicating the storage type of the original object
			/// as a hint (SERIAL_STORAGE_DBL_ARRAY)
			/// <li>followed by
			/// an integer indicating the number of path segments to follow (NP)
			/// or -1 to indicate an unknown number of path segments follows
			/// <li>followed by
			/// an integer indicating the total number of coordinates to follow (NC)
			/// or -1 to indicate an unknown number of coordinates follows
			/// (NC should always be even since coordinates always appear in pairs
			///  representing an x,y pair)
			/// <li>followed by
			/// a byte indicating the winding rule
			/// (<seealso cref="#WIND_EVEN_ODD WIND_EVEN_ODD"/> or
			///  <seealso cref="#WIND_NON_ZERO WIND_NON_ZERO"/>)
			/// <li>followed by
			/// {@code NP} (or unlimited if {@code NP < 0}) sets of values consisting of
			/// a single byte indicating a path segment type
			/// followed by one or more pairs of float or double
			/// values representing the coordinates of the path segment
			/// <li>followed by
			/// a byte indicating the end of the path (SERIAL_PATH_END).
			/// </ol>
			/// <para>
			/// The following byte value constants are used in the serialized form
			/// of {@code Path2D} objects:
			/// <table>
			/// <tr>
			/// <th>Constant Name</th>
			/// <th>Byte Value</th>
			/// <th>Followed by</th>
			/// <th>Description</th>
			/// </tr>
			/// <tr>
			/// <td>{@code SERIAL_STORAGE_FLT_ARRAY}</td>
			/// <td>0x30</td>
			/// <td></td>
			/// <td>A hint that the original {@code Path2D} object stored
			/// the coordinates in a Java array of floats.</td>
			/// </tr>
			/// <tr>
			/// <td>{@code SERIAL_STORAGE_DBL_ARRAY}</td>
			/// <td>0x31</td>
			/// <td></td>
			/// <td>A hint that the original {@code Path2D} object stored
			/// the coordinates in a Java array of doubles.</td>
			/// </tr>
			/// <tr>
			/// <td>{@code SERIAL_SEG_FLT_MOVETO}</td>
			/// <td>0x40</td>
			/// <td>2 floats</td>
			/// <td>A <seealso cref="#moveTo moveTo"/> path segment follows.</td>
			/// </tr>
			/// <tr>
			/// <td>{@code SERIAL_SEG_FLT_LINETO}</td>
			/// <td>0x41</td>
			/// <td>2 floats</td>
			/// <td>A <seealso cref="#lineTo lineTo"/> path segment follows.</td>
			/// </tr>
			/// <tr>
			/// <td>{@code SERIAL_SEG_FLT_QUADTO}</td>
			/// <td>0x42</td>
			/// <td>4 floats</td>
			/// <td>A <seealso cref="#quadTo quadTo"/> path segment follows.</td>
			/// </tr>
			/// <tr>
			/// <td>{@code SERIAL_SEG_FLT_CUBICTO}</td>
			/// <td>0x43</td>
			/// <td>6 floats</td>
			/// <td>A <seealso cref="#curveTo curveTo"/> path segment follows.</td>
			/// </tr>
			/// <tr>
			/// <td>{@code SERIAL_SEG_DBL_MOVETO}</td>
			/// <td>0x50</td>
			/// <td>2 doubles</td>
			/// <td>A <seealso cref="#moveTo moveTo"/> path segment follows.</td>
			/// </tr>
			/// <tr>
			/// <td>{@code SERIAL_SEG_DBL_LINETO}</td>
			/// <td>0x51</td>
			/// <td>2 doubles</td>
			/// <td>A <seealso cref="#lineTo lineTo"/> path segment follows.</td>
			/// </tr>
			/// <tr>
			/// <td>{@code SERIAL_SEG_DBL_QUADTO}</td>
			/// <td>0x52</td>
			/// <td>4 doubles</td>
			/// <td>A <seealso cref="#curveTo curveTo"/> path segment follows.</td>
			/// </tr>
			/// <tr>
			/// <td>{@code SERIAL_SEG_DBL_CUBICTO}</td>
			/// <td>0x53</td>
			/// <td>6 doubles</td>
			/// <td>A <seealso cref="#curveTo curveTo"/> path segment follows.</td>
			/// </tr>
			/// <tr>
			/// <td>{@code SERIAL_SEG_CLOSE}</td>
			/// <td>0x60</td>
			/// <td></td>
			/// <td>A <seealso cref="#closePath closePath"/> path segment.</td>
			/// </tr>
			/// <tr>
			/// <td>{@code SERIAL_PATH_END}</td>
			/// <td>0x61</td>
			/// <td></td>
			/// <td>There are no more path segments following.</td>
			/// </table>
			/// 
			/// @since 1.6
			/// </para>
			/// </summary>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: private void writeObject(java.io.ObjectOutputStream s) throws java.io.IOException
			internal virtual void WriteObject(java.io.ObjectOutputStream s)
			{
				base.WriteObject(s, true);
			}

			/// <summary>
			/// Reads the default serializable fields from the
			/// {@code ObjectInputStream} followed by an explicit
			/// serialization of the path segments stored in this
			/// path.
			/// <para>
			/// There are no default serializable fields as of 1.6.
			/// </para>
			/// <para>
			/// The serial data for this object is described in the
			/// writeObject method.
			/// 
			/// @since 1.6
			/// </para>
			/// </summary>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: private void readObject(java.io.ObjectInputStream s) throws java.lang.ClassNotFoundException, java.io.IOException
			internal virtual void ReadObject(java.io.ObjectInputStream s)
			{
				base.ReadObject(s, true);
			}

			internal class CopyIterator : Path2D.Iterator
			{
				internal double[] DoubleCoords;

				internal CopyIterator(Path2D.Double p2dd) : base(p2dd)
				{
					this.DoubleCoords = p2dd.DoubleCoords;
				}

				public override int CurrentSegment(float[] coords)
				{
					int type = Path.PointTypes[TypeIdx];
					int numCoords = Curvecoords[type];
					if (numCoords > 0)
					{
						for (int i = 0; i < numCoords; i++)
						{
							coords[i] = (float) DoubleCoords[PointIdx + i];
						}
					}
					return type;
				}

				public override int CurrentSegment(double[] coords)
				{
					int type = Path.PointTypes[TypeIdx];
					int numCoords = Curvecoords[type];
					if (numCoords > 0)
					{
						System.Array.Copy(DoubleCoords, PointIdx, coords, 0, numCoords);
					}
					return type;
				}
			}

			internal class TxIterator : Path2D.Iterator
			{
				internal double[] DoubleCoords;
				internal AffineTransform Affine;

				internal TxIterator(Path2D.Double p2dd, AffineTransform at) : base(p2dd)
				{
					this.DoubleCoords = p2dd.DoubleCoords;
					this.Affine = at;
				}

				public override int CurrentSegment(float[] coords)
				{
					int type = Path.PointTypes[TypeIdx];
					int numCoords = Curvecoords[type];
					if (numCoords > 0)
					{
						Affine.Transform(DoubleCoords, PointIdx, coords, 0, numCoords / 2);
					}
					return type;
				}

				public override int CurrentSegment(double[] coords)
				{
					int type = Path.PointTypes[TypeIdx];
					int numCoords = Curvecoords[type];
					if (numCoords > 0)
					{
						Affine.Transform(DoubleCoords, PointIdx, coords, 0, numCoords / 2);
					}
					return type;
				}
			}
		}

		/// <summary>
		/// Adds a point to the path by moving to the specified
		/// coordinates specified in double precision.
		/// </summary>
		/// <param name="x"> the specified X coordinate </param>
		/// <param name="y"> the specified Y coordinate
		/// @since 1.6 </param>
		public abstract void MoveTo(double x, double y);

		/// <summary>
		/// Adds a point to the path by drawing a straight line from the
		/// current coordinates to the new specified coordinates
		/// specified in double precision.
		/// </summary>
		/// <param name="x"> the specified X coordinate </param>
		/// <param name="y"> the specified Y coordinate
		/// @since 1.6 </param>
		public abstract void LineTo(double x, double y);

		/// <summary>
		/// Adds a curved segment, defined by two new points, to the path by
		/// drawing a Quadratic curve that intersects both the current
		/// coordinates and the specified coordinates {@code (x2,y2)},
		/// using the specified point {@code (x1,y1)} as a quadratic
		/// parametric control point.
		/// All coordinates are specified in double precision.
		/// </summary>
		/// <param name="x1"> the X coordinate of the quadratic control point </param>
		/// <param name="y1"> the Y coordinate of the quadratic control point </param>
		/// <param name="x2"> the X coordinate of the final end point </param>
		/// <param name="y2"> the Y coordinate of the final end point
		/// @since 1.6 </param>
		public abstract void QuadTo(double x1, double y1, double x2, double y2);

		/// <summary>
		/// Adds a curved segment, defined by three new points, to the path by
		/// drawing a B&eacute;zier curve that intersects both the current
		/// coordinates and the specified coordinates {@code (x3,y3)},
		/// using the specified points {@code (x1,y1)} and {@code (x2,y2)} as
		/// B&eacute;zier control points.
		/// All coordinates are specified in double precision.
		/// </summary>
		/// <param name="x1"> the X coordinate of the first B&eacute;zier control point </param>
		/// <param name="y1"> the Y coordinate of the first B&eacute;zier control point </param>
		/// <param name="x2"> the X coordinate of the second B&eacute;zier control point </param>
		/// <param name="y2"> the Y coordinate of the second B&eacute;zier control point </param>
		/// <param name="x3"> the X coordinate of the final end point </param>
		/// <param name="y3"> the Y coordinate of the final end point
		/// @since 1.6 </param>
		public abstract void CurveTo(double x1, double y1, double x2, double y2, double x3, double y3);

		/// <summary>
		/// Closes the current subpath by drawing a straight line back to
		/// the coordinates of the last {@code moveTo}.  If the path is already
		/// closed then this method has no effect.
		/// 
		/// @since 1.6
		/// </summary>
		public void ClosePath()
		{
			lock (this)
			{
				if (NumTypes == 0 || PointTypes[NumTypes - 1] != SEG_CLOSE)
				{
					NeedRoom(true, 0);
					PointTypes[NumTypes++] = SEG_CLOSE;
				}
			}
		}

		/// <summary>
		/// Appends the geometry of the specified {@code Shape} object to the
		/// path, possibly connecting the new geometry to the existing path
		/// segments with a line segment.
		/// If the {@code connect} parameter is {@code true} and the
		/// path is not empty then any initial {@code moveTo} in the
		/// geometry of the appended {@code Shape}
		/// is turned into a {@code lineTo} segment.
		/// If the destination coordinates of such a connecting {@code lineTo}
		/// segment match the ending coordinates of a currently open
		/// subpath then the segment is omitted as superfluous.
		/// The winding rule of the specified {@code Shape} is ignored
		/// and the appended geometry is governed by the winding
		/// rule specified for this path.
		/// </summary>
		/// <param name="s"> the {@code Shape} whose geometry is appended
		///          to this path </param>
		/// <param name="connect"> a boolean to control whether or not to turn an initial
		///                {@code moveTo} segment into a {@code lineTo} segment
		///                to connect the new geometry to the existing path
		/// @since 1.6 </param>
		public void Append(Shape s, bool connect)
		{
			Append(s.GetPathIterator(null), connect);
		}

		/// <summary>
		/// Appends the geometry of the specified
		/// <seealso cref="PathIterator"/> object
		/// to the path, possibly connecting the new geometry to the existing
		/// path segments with a line segment.
		/// If the {@code connect} parameter is {@code true} and the
		/// path is not empty then any initial {@code moveTo} in the
		/// geometry of the appended {@code Shape} is turned into a
		/// {@code lineTo} segment.
		/// If the destination coordinates of such a connecting {@code lineTo}
		/// segment match the ending coordinates of a currently open
		/// subpath then the segment is omitted as superfluous.
		/// The winding rule of the specified {@code Shape} is ignored
		/// and the appended geometry is governed by the winding
		/// rule specified for this path.
		/// </summary>
		/// <param name="pi"> the {@code PathIterator} whose geometry is appended to
		///           this path </param>
		/// <param name="connect"> a boolean to control whether or not to turn an initial
		///                {@code moveTo} segment into a {@code lineTo} segment
		///                to connect the new geometry to the existing path
		/// @since 1.6 </param>
		public abstract void Append(PathIterator pi, bool connect);

		/// <summary>
		/// Returns the fill style winding rule.
		/// </summary>
		/// <returns> an integer representing the current winding rule. </returns>
		/// <seealso cref= #WIND_EVEN_ODD </seealso>
		/// <seealso cref= #WIND_NON_ZERO </seealso>
		/// <seealso cref= #setWindingRule
		/// @since 1.6 </seealso>
		public int WindingRule
		{
			get
			{
				lock (this)
				{
					return WindingRule_Renamed;
				}
			}
			set
			{
				if (value != WIND_EVEN_ODD && value != WIND_NON_ZERO)
				{
					throw new IllegalArgumentException("winding rule must be " + "WIND_EVEN_ODD or " + "WIND_NON_ZERO");
				}
				WindingRule_Renamed = value;
			}
		}


		/// <summary>
		/// Returns the coordinates most recently added to the end of the path
		/// as a <seealso cref="Point2D"/> object.
		/// </summary>
		/// <returns> a {@code Point2D} object containing the ending coordinates of
		///         the path or {@code null} if there are no points in the path.
		/// @since 1.6 </returns>
		public Point2D CurrentPoint
		{
			get
			{
				lock (this)
				{
					int index = NumCoords;
					if (NumTypes < 1 || index < 1)
					{
						return null;
					}
					if (PointTypes[NumTypes - 1] == SEG_CLOSE)
					{
						for (int i = NumTypes - 2; i > 0; i--)
						{
							switch (PointTypes[i])
							{
							case SEG_MOVETO:
								goto loopBreak;
							case SEG_LINETO:
								index -= 2;
								break;
							case SEG_QUADTO:
								index -= 4;
								break;
							case SEG_CUBICTO:
								index -= 6;
								break;
							case SEG_CLOSE:
								break;
							}
						loopContinue:;
						}
					loopBreak:;
					}
					return GetPoint(index - 2);
				}
			}
		}

		/// <summary>
		/// Resets the path to empty.  The append position is set back to the
		/// beginning of the path and all coordinates and point types are
		/// forgotten.
		/// 
		/// @since 1.6
		/// </summary>
		public void Reset()
		{
			lock (this)
			{
				NumTypes = NumCoords = 0;
			}
		}

		/// <summary>
		/// Transforms the geometry of this path using the specified
		/// <seealso cref="AffineTransform"/>.
		/// The geometry is transformed in place, which permanently changes the
		/// boundary defined by this object.
		/// </summary>
		/// <param name="at"> the {@code AffineTransform} used to transform the area
		/// @since 1.6 </param>
		public abstract void Transform(AffineTransform at);

		/// <summary>
		/// Returns a new {@code Shape} representing a transformed version
		/// of this {@code Path2D}.
		/// Note that the exact type and coordinate precision of the return
		/// value is not specified for this method.
		/// The method will return a Shape that contains no less precision
		/// for the transformed geometry than this {@code Path2D} currently
		/// maintains, but it may contain no more precision either.
		/// If the tradeoff of precision vs. storage size in the result is
		/// important then the convenience constructors in the
		/// <seealso cref="Path2D.Float#Path2D.Float(Shape, AffineTransform) Path2D.Float"/>
		/// and
		/// <seealso cref="Path2D.Double#Path2D.Double(Shape, AffineTransform) Path2D.Double"/>
		/// subclasses should be used to make the choice explicit.
		/// </summary>
		/// <param name="at"> the {@code AffineTransform} used to transform a
		///           new {@code Shape}. </param>
		/// <returns> a new {@code Shape}, transformed with the specified
		///         {@code AffineTransform}.
		/// @since 1.6 </returns>
		public Shape CreateTransformedShape(AffineTransform at)
		{
			lock (this)
			{
				Path2D p2d = (Path2D) Clone();
				if (at != null)
				{
					p2d.Transform(at);
				}
				return p2d;
			}
		}

		/// <summary>
		/// {@inheritDoc}
		/// @since 1.6
		/// </summary>
		public Rectangle Bounds
		{
			get
			{
				return Bounds2D.Bounds;
			}
		}

		/// <summary>
		/// Tests if the specified coordinates are inside the closed
		/// boundary of the specified <seealso cref="PathIterator"/>.
		/// <para>
		/// This method provides a basic facility for implementors of
		/// the <seealso cref="Shape"/> interface to implement support for the
		/// <seealso cref="Shape#contains(double, double)"/> method.
		/// 
		/// </para>
		/// </summary>
		/// <param name="pi"> the specified {@code PathIterator} </param>
		/// <param name="x"> the specified X coordinate </param>
		/// <param name="y"> the specified Y coordinate </param>
		/// <returns> {@code true} if the specified coordinates are inside the
		///         specified {@code PathIterator}; {@code false} otherwise
		/// @since 1.6 </returns>
		public static bool Contains(PathIterator pi, double x, double y)
		{
			if (x * 0.0 + y * 0.0 == 0.0)
			{
				/* N * 0.0 is 0.0 only if N is finite.
				 * Here we know that both x and y are finite.
				 */
				int mask = (pi.WindingRule == WIND_NON_ZERO ? - 1 : 1);
				int cross = Curve.pointCrossingsForPath(pi, x, y);
				return ((cross & mask) != 0);
			}
			else
			{
				/* Either x or y was infinite or NaN.
				 * A NaN always produces a negative response to any test
				 * and Infinity values cannot be "inside" any path so
				 * they should return false as well.
				 */
				return false;
			}
		}

		/// <summary>
		/// Tests if the specified <seealso cref="Point2D"/> is inside the closed
		/// boundary of the specified <seealso cref="PathIterator"/>.
		/// <para>
		/// This method provides a basic facility for implementors of
		/// the <seealso cref="Shape"/> interface to implement support for the
		/// <seealso cref="Shape#contains(Point2D)"/> method.
		/// 
		/// </para>
		/// </summary>
		/// <param name="pi"> the specified {@code PathIterator} </param>
		/// <param name="p"> the specified {@code Point2D} </param>
		/// <returns> {@code true} if the specified coordinates are inside the
		///         specified {@code PathIterator}; {@code false} otherwise
		/// @since 1.6 </returns>
		public static bool Contains(PathIterator pi, Point2D p)
		{
			return Contains(pi, p.X, p.Y);
		}

		/// <summary>
		/// {@inheritDoc}
		/// @since 1.6
		/// </summary>
		public bool Contains(double x, double y)
		{
			if (x * 0.0 + y * 0.0 == 0.0)
			{
				/* N * 0.0 is 0.0 only if N is finite.
				 * Here we know that both x and y are finite.
				 */
				if (NumTypes < 2)
				{
					return false;
				}
				int mask = (WindingRule_Renamed == WIND_NON_ZERO ? - 1 : 1);
				return ((PointCrossings(x, y) & mask) != 0);
			}
			else
			{
				/* Either x or y was infinite or NaN.
				 * A NaN always produces a negative response to any test
				 * and Infinity values cannot be "inside" any path so
				 * they should return false as well.
				 */
				return false;
			}
		}

		/// <summary>
		/// {@inheritDoc}
		/// @since 1.6
		/// </summary>
		public bool Contains(Point2D p)
		{
			return Contains(p.X, p.Y);
		}

		/// <summary>
		/// Tests if the specified rectangular area is entirely inside the
		/// closed boundary of the specified <seealso cref="PathIterator"/>.
		/// <para>
		/// This method provides a basic facility for implementors of
		/// the <seealso cref="Shape"/> interface to implement support for the
		/// <seealso cref="Shape#contains(double, double, double, double)"/> method.
		/// </para>
		/// <para>
		/// This method object may conservatively return false in
		/// cases where the specified rectangular area intersects a
		/// segment of the path, but that segment does not represent a
		/// boundary between the interior and exterior of the path.
		/// Such segments could lie entirely within the interior of the
		/// path if they are part of a path with a <seealso cref="#WIND_NON_ZERO"/>
		/// winding rule or if the segments are retraced in the reverse
		/// direction such that the two sets of segments cancel each
		/// other out without any exterior area falling between them.
		/// To determine whether segments represent true boundaries of
		/// the interior of the path would require extensive calculations
		/// involving all of the segments of the path and the winding
		/// rule and are thus beyond the scope of this implementation.
		/// 
		/// </para>
		/// </summary>
		/// <param name="pi"> the specified {@code PathIterator} </param>
		/// <param name="x"> the specified X coordinate </param>
		/// <param name="y"> the specified Y coordinate </param>
		/// <param name="w"> the width of the specified rectangular area </param>
		/// <param name="h"> the height of the specified rectangular area </param>
		/// <returns> {@code true} if the specified {@code PathIterator} contains
		///         the specified rectangular area; {@code false} otherwise.
		/// @since 1.6 </returns>
		public static bool Contains(PathIterator pi, double x, double y, double w, double h)
		{
			if (Double.IsNaN(x + w) || Double.IsNaN(y + h))
			{
				/* [xy]+[wh] is NaN if any of those values are NaN,
				 * or if adding the two together would produce NaN
				 * by virtue of adding opposing Infinte values.
				 * Since we need to add them below, their sum must
				 * not be NaN.
				 * We return false because NaN always produces a
				 * negative response to tests
				 */
				return false;
			}
			if (w <= 0 || h <= 0)
			{
				return false;
			}
			int mask = (pi.WindingRule == WIND_NON_ZERO ? - 1 : 2);
			int crossings = Curve.rectCrossingsForPath(pi, x, y, x + w, y + h);
			return (crossings != Curve.RECT_INTERSECTS && (crossings & mask) != 0);
		}

		/// <summary>
		/// Tests if the specified <seealso cref="Rectangle2D"/> is entirely inside the
		/// closed boundary of the specified <seealso cref="PathIterator"/>.
		/// <para>
		/// This method provides a basic facility for implementors of
		/// the <seealso cref="Shape"/> interface to implement support for the
		/// <seealso cref="Shape#contains(Rectangle2D)"/> method.
		/// </para>
		/// <para>
		/// This method object may conservatively return false in
		/// cases where the specified rectangular area intersects a
		/// segment of the path, but that segment does not represent a
		/// boundary between the interior and exterior of the path.
		/// Such segments could lie entirely within the interior of the
		/// path if they are part of a path with a <seealso cref="#WIND_NON_ZERO"/>
		/// winding rule or if the segments are retraced in the reverse
		/// direction such that the two sets of segments cancel each
		/// other out without any exterior area falling between them.
		/// To determine whether segments represent true boundaries of
		/// the interior of the path would require extensive calculations
		/// involving all of the segments of the path and the winding
		/// rule and are thus beyond the scope of this implementation.
		/// 
		/// </para>
		/// </summary>
		/// <param name="pi"> the specified {@code PathIterator} </param>
		/// <param name="r"> a specified {@code Rectangle2D} </param>
		/// <returns> {@code true} if the specified {@code PathIterator} contains
		///         the specified {@code Rectangle2D}; {@code false} otherwise.
		/// @since 1.6 </returns>
		public static bool Contains(PathIterator pi, Rectangle2D r)
		{
			return Contains(pi, r.X, r.Y, r.Width, r.Height);
		}

		/// <summary>
		/// {@inheritDoc}
		/// <para>
		/// This method object may conservatively return false in
		/// cases where the specified rectangular area intersects a
		/// segment of the path, but that segment does not represent a
		/// boundary between the interior and exterior of the path.
		/// Such segments could lie entirely within the interior of the
		/// path if they are part of a path with a <seealso cref="#WIND_NON_ZERO"/>
		/// winding rule or if the segments are retraced in the reverse
		/// direction such that the two sets of segments cancel each
		/// other out without any exterior area falling between them.
		/// To determine whether segments represent true boundaries of
		/// the interior of the path would require extensive calculations
		/// involving all of the segments of the path and the winding
		/// rule and are thus beyond the scope of this implementation.
		/// 
		/// @since 1.6
		/// </para>
		/// </summary>
		public bool Contains(double x, double y, double w, double h)
		{
			if (Double.IsNaN(x + w) || Double.IsNaN(y + h))
			{
				/* [xy]+[wh] is NaN if any of those values are NaN,
				 * or if adding the two together would produce NaN
				 * by virtue of adding opposing Infinte values.
				 * Since we need to add them below, their sum must
				 * not be NaN.
				 * We return false because NaN always produces a
				 * negative response to tests
				 */
				return false;
			}
			if (w <= 0 || h <= 0)
			{
				return false;
			}
			int mask = (WindingRule_Renamed == WIND_NON_ZERO ? - 1 : 2);
			int crossings = RectCrossings(x, y, x + w, y + h);
			return (crossings != Curve.RECT_INTERSECTS && (crossings & mask) != 0);
		}

		/// <summary>
		/// {@inheritDoc}
		/// <para>
		/// This method object may conservatively return false in
		/// cases where the specified rectangular area intersects a
		/// segment of the path, but that segment does not represent a
		/// boundary between the interior and exterior of the path.
		/// Such segments could lie entirely within the interior of the
		/// path if they are part of a path with a <seealso cref="#WIND_NON_ZERO"/>
		/// winding rule or if the segments are retraced in the reverse
		/// direction such that the two sets of segments cancel each
		/// other out without any exterior area falling between them.
		/// To determine whether segments represent true boundaries of
		/// the interior of the path would require extensive calculations
		/// involving all of the segments of the path and the winding
		/// rule and are thus beyond the scope of this implementation.
		/// 
		/// @since 1.6
		/// </para>
		/// </summary>
		public bool Contains(Rectangle2D r)
		{
			return Contains(r.X, r.Y, r.Width, r.Height);
		}

		/// <summary>
		/// Tests if the interior of the specified <seealso cref="PathIterator"/>
		/// intersects the interior of a specified set of rectangular
		/// coordinates.
		/// <para>
		/// This method provides a basic facility for implementors of
		/// the <seealso cref="Shape"/> interface to implement support for the
		/// <seealso cref="Shape#intersects(double, double, double, double)"/> method.
		/// </para>
		/// <para>
		/// This method object may conservatively return true in
		/// cases where the specified rectangular area intersects a
		/// segment of the path, but that segment does not represent a
		/// boundary between the interior and exterior of the path.
		/// Such a case may occur if some set of segments of the
		/// path are retraced in the reverse direction such that the
		/// two sets of segments cancel each other out without any
		/// interior area between them.
		/// To determine whether segments represent true boundaries of
		/// the interior of the path would require extensive calculations
		/// involving all of the segments of the path and the winding
		/// rule and are thus beyond the scope of this implementation.
		/// 
		/// </para>
		/// </summary>
		/// <param name="pi"> the specified {@code PathIterator} </param>
		/// <param name="x"> the specified X coordinate </param>
		/// <param name="y"> the specified Y coordinate </param>
		/// <param name="w"> the width of the specified rectangular coordinates </param>
		/// <param name="h"> the height of the specified rectangular coordinates </param>
		/// <returns> {@code true} if the specified {@code PathIterator} and
		///         the interior of the specified set of rectangular
		///         coordinates intersect each other; {@code false} otherwise.
		/// @since 1.6 </returns>
		public static bool Intersects(PathIterator pi, double x, double y, double w, double h)
		{
			if (Double.IsNaN(x + w) || Double.IsNaN(y + h))
			{
				/* [xy]+[wh] is NaN if any of those values are NaN,
				 * or if adding the two together would produce NaN
				 * by virtue of adding opposing Infinte values.
				 * Since we need to add them below, their sum must
				 * not be NaN.
				 * We return false because NaN always produces a
				 * negative response to tests
				 */
				return false;
			}
			if (w <= 0 || h <= 0)
			{
				return false;
			}
			int mask = (pi.WindingRule == WIND_NON_ZERO ? - 1 : 2);
			int crossings = Curve.rectCrossingsForPath(pi, x, y, x + w, y + h);
			return (crossings == Curve.RECT_INTERSECTS || (crossings & mask) != 0);
		}

		/// <summary>
		/// Tests if the interior of the specified <seealso cref="PathIterator"/>
		/// intersects the interior of a specified <seealso cref="Rectangle2D"/>.
		/// <para>
		/// This method provides a basic facility for implementors of
		/// the <seealso cref="Shape"/> interface to implement support for the
		/// <seealso cref="Shape#intersects(Rectangle2D)"/> method.
		/// </para>
		/// <para>
		/// This method object may conservatively return true in
		/// cases where the specified rectangular area intersects a
		/// segment of the path, but that segment does not represent a
		/// boundary between the interior and exterior of the path.
		/// Such a case may occur if some set of segments of the
		/// path are retraced in the reverse direction such that the
		/// two sets of segments cancel each other out without any
		/// interior area between them.
		/// To determine whether segments represent true boundaries of
		/// the interior of the path would require extensive calculations
		/// involving all of the segments of the path and the winding
		/// rule and are thus beyond the scope of this implementation.
		/// 
		/// </para>
		/// </summary>
		/// <param name="pi"> the specified {@code PathIterator} </param>
		/// <param name="r"> the specified {@code Rectangle2D} </param>
		/// <returns> {@code true} if the specified {@code PathIterator} and
		///         the interior of the specified {@code Rectangle2D}
		///         intersect each other; {@code false} otherwise.
		/// @since 1.6 </returns>
		public static bool Intersects(PathIterator pi, Rectangle2D r)
		{
			return Intersects(pi, r.X, r.Y, r.Width, r.Height);
		}

		/// <summary>
		/// {@inheritDoc}
		/// <para>
		/// This method object may conservatively return true in
		/// cases where the specified rectangular area intersects a
		/// segment of the path, but that segment does not represent a
		/// boundary between the interior and exterior of the path.
		/// Such a case may occur if some set of segments of the
		/// path are retraced in the reverse direction such that the
		/// two sets of segments cancel each other out without any
		/// interior area between them.
		/// To determine whether segments represent true boundaries of
		/// the interior of the path would require extensive calculations
		/// involving all of the segments of the path and the winding
		/// rule and are thus beyond the scope of this implementation.
		/// 
		/// @since 1.6
		/// </para>
		/// </summary>
		public bool Intersects(double x, double y, double w, double h)
		{
			if (Double.IsNaN(x + w) || Double.IsNaN(y + h))
			{
				/* [xy]+[wh] is NaN if any of those values are NaN,
				 * or if adding the two together would produce NaN
				 * by virtue of adding opposing Infinte values.
				 * Since we need to add them below, their sum must
				 * not be NaN.
				 * We return false because NaN always produces a
				 * negative response to tests
				 */
				return false;
			}
			if (w <= 0 || h <= 0)
			{
				return false;
			}
			int mask = (WindingRule_Renamed == WIND_NON_ZERO ? - 1 : 2);
			int crossings = RectCrossings(x, y, x + w, y + h);
			return (crossings == Curve.RECT_INTERSECTS || (crossings & mask) != 0);
		}

		/// <summary>
		/// {@inheritDoc}
		/// <para>
		/// This method object may conservatively return true in
		/// cases where the specified rectangular area intersects a
		/// segment of the path, but that segment does not represent a
		/// boundary between the interior and exterior of the path.
		/// Such a case may occur if some set of segments of the
		/// path are retraced in the reverse direction such that the
		/// two sets of segments cancel each other out without any
		/// interior area between them.
		/// To determine whether segments represent true boundaries of
		/// the interior of the path would require extensive calculations
		/// involving all of the segments of the path and the winding
		/// rule and are thus beyond the scope of this implementation.
		/// 
		/// @since 1.6
		/// </para>
		/// </summary>
		public bool Intersects(Rectangle2D r)
		{
			return Intersects(r.X, r.Y, r.Width, r.Height);
		}

		/// <summary>
		/// {@inheritDoc}
		/// <para>
		/// The iterator for this class is not multi-threaded safe,
		/// which means that this {@code Path2D} class does not
		/// guarantee that modifications to the geometry of this
		/// {@code Path2D} object do not affect any iterations of
		/// that geometry that are already in process.
		/// 
		/// @since 1.6
		/// </para>
		/// </summary>
		public PathIterator GetPathIterator(AffineTransform at, double flatness)
		{
			return new FlatteningPathIterator(GetPathIterator(at), flatness);
		}

		/// <summary>
		/// Creates a new object of the same class as this object.
		/// </summary>
		/// <returns>     a clone of this instance. </returns>
		/// <exception cref="OutOfMemoryError">            if there is not enough memory. </exception>
		/// <seealso cref=        java.lang.Cloneable
		/// @since      1.6 </seealso>
		public abstract Object Clone();
			// Note: It would be nice to have this return Path2D
			// but one of our subclasses (GeneralPath) needs to
			// offer "public Object clone()" for backwards
			// compatibility so we cannot restrict it further.
			// REMIND: Can we do both somehow?

		/*
		 * Support fields and methods for serializing the subclasses.
		 */
		private const sbyte SERIAL_STORAGE_FLT_ARRAY = 0x30;
		private const sbyte SERIAL_STORAGE_DBL_ARRAY = 0x31;

		private const sbyte SERIAL_SEG_FLT_MOVETO = 0x40;
		private const sbyte SERIAL_SEG_FLT_LINETO = 0x41;
		private const sbyte SERIAL_SEG_FLT_QUADTO = 0x42;
		private const sbyte SERIAL_SEG_FLT_CUBICTO = 0x43;

		private const sbyte SERIAL_SEG_DBL_MOVETO = 0x50;
		private const sbyte SERIAL_SEG_DBL_LINETO = 0x51;
		private const sbyte SERIAL_SEG_DBL_QUADTO = 0x52;
		private const sbyte SERIAL_SEG_DBL_CUBICTO = 0x53;

		private const sbyte SERIAL_SEG_CLOSE = 0x60;
		private const sbyte SERIAL_PATH_END = 0x61;

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: final void writeObject(java.io.ObjectOutputStream s, boolean isdbl) throws java.io.IOException
		internal void WriteObject(java.io.ObjectOutputStream s, bool isdbl)
		{
			s.DefaultWriteObject();

			float[] fCoords;
			double[] dCoords;

			if (isdbl)
			{
				dCoords = ((Path2D.Double) this).DoubleCoords;
				fCoords = null;
			}
			else
			{
				fCoords = ((Path2D.Float) this).FloatCoords;
				dCoords = null;
			}

			int numTypes = this.NumTypes;

			s.WriteByte(isdbl ? SERIAL_STORAGE_DBL_ARRAY : SERIAL_STORAGE_FLT_ARRAY);
			s.WriteInt(numTypes);
			s.WriteInt(NumCoords);
			s.WriteByte((sbyte) WindingRule_Renamed);

			int cindex = 0;
			for (int i = 0; i < numTypes; i++)
			{
				int npoints;
				sbyte serialtype;
				switch (PointTypes[i])
				{
				case SEG_MOVETO:
					npoints = 1;
					serialtype = (isdbl ? SERIAL_SEG_DBL_MOVETO : SERIAL_SEG_FLT_MOVETO);
					break;
				case SEG_LINETO:
					npoints = 1;
					serialtype = (isdbl ? SERIAL_SEG_DBL_LINETO : SERIAL_SEG_FLT_LINETO);
					break;
				case SEG_QUADTO:
					npoints = 2;
					serialtype = (isdbl ? SERIAL_SEG_DBL_QUADTO : SERIAL_SEG_FLT_QUADTO);
					break;
				case SEG_CUBICTO:
					npoints = 3;
					serialtype = (isdbl ? SERIAL_SEG_DBL_CUBICTO : SERIAL_SEG_FLT_CUBICTO);
					break;
				case SEG_CLOSE:
					npoints = 0;
					serialtype = SERIAL_SEG_CLOSE;
					break;

				default:
					// Should never happen
					throw new InternalError("unrecognized path type");
				}
				s.WriteByte(serialtype);
				while (--npoints >= 0)
				{
					if (isdbl)
					{
						s.WriteDouble(dCoords[cindex++]);
						s.WriteDouble(dCoords[cindex++]);
					}
					else
					{
						s.WriteFloat(fCoords[cindex++]);
						s.WriteFloat(fCoords[cindex++]);
					}
				}
			}
			s.WriteByte(SERIAL_PATH_END);
		}

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: final void readObject(java.io.ObjectInputStream s, boolean storedbl) throws java.lang.ClassNotFoundException, java.io.IOException
		internal void ReadObject(java.io.ObjectInputStream s, bool storedbl)
		{
			s.DefaultReadObject();

			// The subclass calls this method with the storage type that
			// they want us to use (storedbl) so we ignore the storage
			// method hint from the stream.
			s.ReadByte();
			int nT = s.ReadInt();
			int nC = s.ReadInt();
			try
			{
				WindingRule = s.ReadByte();
			}
			catch (IllegalArgumentException iae)
			{
				throw new java.io.InvalidObjectException(iae.Message);
			}

			PointTypes = new sbyte[(nT < 0) ? INIT_SIZE : nT];
			if (nC < 0)
			{
				nC = INIT_SIZE * 2;
			}
			if (storedbl)
			{
				((Path2D.Double) this).DoubleCoords = new double[nC];
			}
			else
			{
				((Path2D.Float) this).FloatCoords = new float[nC];
			}

			for (int i = 0; nT < 0 || i < nT; i++)
			{
				bool isdbl;
				int npoints;
				sbyte segtype;

				sbyte serialtype = s.ReadByte();
				switch (serialtype)
				{
				case SERIAL_SEG_FLT_MOVETO:
					isdbl = false;
					npoints = 1;
					segtype = SEG_MOVETO;
					break;
				case SERIAL_SEG_FLT_LINETO:
					isdbl = false;
					npoints = 1;
					segtype = SEG_LINETO;
					break;
				case SERIAL_SEG_FLT_QUADTO:
					isdbl = false;
					npoints = 2;
					segtype = SEG_QUADTO;
					break;
				case SERIAL_SEG_FLT_CUBICTO:
					isdbl = false;
					npoints = 3;
					segtype = SEG_CUBICTO;
					break;

				case SERIAL_SEG_DBL_MOVETO:
					isdbl = true;
					npoints = 1;
					segtype = SEG_MOVETO;
					break;
				case SERIAL_SEG_DBL_LINETO:
					isdbl = true;
					npoints = 1;
					segtype = SEG_LINETO;
					break;
				case SERIAL_SEG_DBL_QUADTO:
					isdbl = true;
					npoints = 2;
					segtype = SEG_QUADTO;
					break;
				case SERIAL_SEG_DBL_CUBICTO:
					isdbl = true;
					npoints = 3;
					segtype = SEG_CUBICTO;
					break;

				case SERIAL_SEG_CLOSE:
					isdbl = false;
					npoints = 0;
					segtype = SEG_CLOSE;
					break;

				case SERIAL_PATH_END:
					if (nT < 0)
					{
						goto PATHDONEBreak;
					}
					throw new StreamCorruptedException("unexpected PATH_END");

				default:
					throw new StreamCorruptedException("unrecognized path type");
				}
				NeedRoom(segtype != SEG_MOVETO, npoints * 2);
				if (isdbl)
				{
					while (--npoints >= 0)
					{
						Append(s.ReadDouble(), s.ReadDouble());
					}
				}
				else
				{
					while (--npoints >= 0)
					{
						Append(s.ReadFloat(), s.ReadFloat());
					}
				}
				PointTypes[NumTypes++] = segtype;
			PATHDONEContinue:;
			}
		PATHDONEBreak:
			if (nT >= 0 && s.ReadByte() != SERIAL_PATH_END)
			{
				throw new StreamCorruptedException("missing PATH_END");
			}
		}

		internal abstract class Iterator : PathIterator
		{
			public abstract int CurrentSegment(double[] coords);
			public abstract int CurrentSegment(float[] coords);
			internal int TypeIdx;
			internal int PointIdx;
			internal Path2D Path;

			internal static readonly int[] Curvecoords = new int[] {2, 2, 4, 6, 0};

			internal Iterator(Path2D path)
			{
				this.Path = path;
			}

			public virtual int WindingRule
			{
				get
				{
					return Path.WindingRule;
				}
			}

			public virtual bool Done
			{
				get
				{
					return (TypeIdx >= Path.NumTypes);
				}
			}

			public virtual void Next()
			{
				int type = Path.PointTypes[TypeIdx++];
				PointIdx += Curvecoords[type];
			}
		}
	}

}
using System;

/*
 * Copyright (c) 1997, 2013, Oracle and/or its affiliates. All rights reserved.
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
	/// The <code>FlatteningPathIterator</code> class returns a flattened view of
	/// another <seealso cref="PathIterator"/> object.  Other <seealso cref="java.awt.Shape Shape"/>
	/// classes can use this class to provide flattening behavior for their paths
	/// without having to perform the interpolation calculations themselves.
	/// 
	/// @author Jim Graham
	/// </summary>
	public class FlatteningPathIterator : PathIterator
	{
		internal const int GROW_SIZE = 24; // Multiple of cubic & quad curve size

		internal PathIterator Src; // The source iterator

		internal double Squareflat; // Square of the flatness parameter
											// for testing against squared lengths

		internal int Limit; // Maximum number of recursion levels

		internal double[] Hold = new double[14]; // The cache of interpolated coords
											// Note that this must be long enough
											// to store a full cubic segment and
											// a relative cubic segment to avoid
											// aliasing when copying the coords
											// of a curve to the end of the array.
											// This is also serendipitously equal
											// to the size of a full quad segment
											// and 2 relative quad segments.

		internal double Curx, Cury; // The ending x,y of the last segment

		internal double Movx, Movy; // The x,y of the last move segment

		internal int HoldType; // The type of the curve being held
											// for interpolation

		internal int HoldEnd; // The index of the last curve segment
											// being held for interpolation

		internal int HoldIndex; // The index of the curve segment
											// that was last interpolated.  This
											// is the curve segment ready to be
											// returned in the next call to
											// currentSegment().

		internal int[] Levels; // The recursion level at which
											// each curve being held in storage
											// was generated.

		internal int LevelIndex; // The index of the entry in the
											// levels array of the curve segment
											// at the holdIndex

		internal bool Done_Renamed; // True when iteration is done

		/// <summary>
		/// Constructs a new <code>FlatteningPathIterator</code> object that
		/// flattens a path as it iterates over it.  The iterator does not
		/// subdivide any curve read from the source iterator to more than
		/// 10 levels of subdivision which yields a maximum of 1024 line
		/// segments per curve. </summary>
		/// <param name="src"> the original unflattened path being iterated over </param>
		/// <param name="flatness"> the maximum allowable distance between the
		/// control points and the flattened curve </param>
		public FlatteningPathIterator(PathIterator src, double flatness) : this(src, flatness, 10)
		{
		}

		/// <summary>
		/// Constructs a new <code>FlatteningPathIterator</code> object
		/// that flattens a path as it iterates over it.
		/// The <code>limit</code> parameter allows you to control the
		/// maximum number of recursive subdivisions that the iterator
		/// can make before it assumes that the curve is flat enough
		/// without measuring against the <code>flatness</code> parameter.
		/// The flattened iteration therefore never generates more than
		/// a maximum of <code>(2^limit)</code> line segments per curve. </summary>
		/// <param name="src"> the original unflattened path being iterated over </param>
		/// <param name="flatness"> the maximum allowable distance between the
		/// control points and the flattened curve </param>
		/// <param name="limit"> the maximum number of recursive subdivisions
		/// allowed for any curved segment </param>
		/// <exception cref="IllegalArgumentException"> if
		///          <code>flatness</code> or <code>limit</code>
		///          is less than zero </exception>
		public FlatteningPathIterator(PathIterator src, double flatness, int limit)
		{
			if (flatness < 0.0)
			{
				throw new IllegalArgumentException("flatness must be >= 0");
			}
			if (limit < 0)
			{
				throw new IllegalArgumentException("limit must be >= 0");
			}
			this.Src = src;
			this.Squareflat = flatness * flatness;
			this.Limit = limit;
			this.Levels = new int[limit + 1];
			// prime the first path segment
			Next(false);
		}

		/// <summary>
		/// Returns the flatness of this iterator. </summary>
		/// <returns> the flatness of this <code>FlatteningPathIterator</code>. </returns>
		public virtual double Flatness
		{
			get
			{
				return System.Math.Sqrt(Squareflat);
			}
		}

		/// <summary>
		/// Returns the recursion limit of this iterator. </summary>
		/// <returns> the recursion limit of this
		/// <code>FlatteningPathIterator</code>. </returns>
		public virtual int RecursionLimit
		{
			get
			{
				return Limit;
			}
		}

		/// <summary>
		/// Returns the winding rule for determining the interior of the
		/// path. </summary>
		/// <returns> the winding rule of the original unflattened path being
		/// iterated over. </returns>
		/// <seealso cref= PathIterator#WIND_EVEN_ODD </seealso>
		/// <seealso cref= PathIterator#WIND_NON_ZERO </seealso>
		public virtual int WindingRule
		{
			get
			{
				return Src.WindingRule;
			}
		}

		/// <summary>
		/// Tests if the iteration is complete. </summary>
		/// <returns> <code>true</code> if all the segments have
		/// been read; <code>false</code> otherwise. </returns>
		public virtual bool Done
		{
			get
			{
				return Done_Renamed;
			}
		}

		/*
		 * Ensures that the hold array can hold up to (want) more values.
		 * It is currently holding (hold.length - holdIndex) values.
		 */
		internal virtual void EnsureHoldCapacity(int want)
		{
			if (HoldIndex - want < 0)
			{
				int have = Hold.Length - HoldIndex;
				int newsize = Hold.Length + GROW_SIZE;
				double[] newhold = new double[newsize];
				System.Array.Copy(Hold, HoldIndex, newhold, HoldIndex + GROW_SIZE, have);
				Hold = newhold;
				HoldIndex += GROW_SIZE;
				HoldEnd += GROW_SIZE;
			}
		}

		/// <summary>
		/// Moves the iterator to the next segment of the path forwards
		/// along the primary direction of traversal as long as there are
		/// more points in that direction.
		/// </summary>
		public virtual void Next()
		{
			Next(true);
		}

		private void Next(bool doNext)
		{
			int level;

			if (HoldIndex >= HoldEnd)
			{
				if (doNext)
				{
					Src.Next();
				}
				if (Src.Done)
				{
					Done_Renamed = true;
					return;
				}
				HoldType = Src.CurrentSegment(Hold);
				LevelIndex = 0;
				Levels[0] = 0;
			}

			switch (HoldType)
			{
			case PathIterator_Fields.SEG_MOVETO:
			case PathIterator_Fields.SEG_LINETO:
				Curx = Hold[0];
				Cury = Hold[1];
				if (HoldType == PathIterator_Fields.SEG_MOVETO)
				{
					Movx = Curx;
					Movy = Cury;
				}
				HoldIndex = 0;
				HoldEnd = 0;
				break;
			case PathIterator_Fields.SEG_CLOSE:
				Curx = Movx;
				Cury = Movy;
				HoldIndex = 0;
				HoldEnd = 0;
				break;
			case PathIterator_Fields.SEG_QUADTO:
				if (HoldIndex >= HoldEnd)
				{
					// Move the coordinates to the end of the array.
					HoldIndex = Hold.Length - 6;
					HoldEnd = Hold.Length - 2;
					Hold[HoldIndex + 0] = Curx;
					Hold[HoldIndex + 1] = Cury;
					Hold[HoldIndex + 2] = Hold[0];
					Hold[HoldIndex + 3] = Hold[1];
					Hold[HoldIndex + 4] = Curx = Hold[2];
					Hold[HoldIndex + 5] = Cury = Hold[3];
				}

				level = Levels[LevelIndex];
				while (level < Limit)
				{
					if (QuadCurve2D.GetFlatnessSq(Hold, HoldIndex) < Squareflat)
					{
						break;
					}

					EnsureHoldCapacity(4);
					QuadCurve2D.Subdivide(Hold, HoldIndex, Hold, HoldIndex - 4, Hold, HoldIndex);
					HoldIndex -= 4;

					// Now that we have subdivided, we have constructed
					// two curves of one depth lower than the original
					// curve.  One of those curves is in the place of
					// the former curve and one of them is in the next
					// set of held coordinate slots.  We now set both
					// curves level values to the next higher level.
					level++;
					Levels[LevelIndex] = level;
					LevelIndex++;
					Levels[LevelIndex] = level;
				}

				// This curve segment is flat enough, or it is too deep
				// in recursion levels to try to flatten any more.  The
				// two coordinates at holdIndex+4 and holdIndex+5 now
				// contain the endpoint of the curve which can be the
				// endpoint of an approximating line segment.
				HoldIndex += 4;
				LevelIndex--;
				break;
			case PathIterator_Fields.SEG_CUBICTO:
				if (HoldIndex >= HoldEnd)
				{
					// Move the coordinates to the end of the array.
					HoldIndex = Hold.Length - 8;
					HoldEnd = Hold.Length - 2;
					Hold[HoldIndex + 0] = Curx;
					Hold[HoldIndex + 1] = Cury;
					Hold[HoldIndex + 2] = Hold[0];
					Hold[HoldIndex + 3] = Hold[1];
					Hold[HoldIndex + 4] = Hold[2];
					Hold[HoldIndex + 5] = Hold[3];
					Hold[HoldIndex + 6] = Curx = Hold[4];
					Hold[HoldIndex + 7] = Cury = Hold[5];
				}

				level = Levels[LevelIndex];
				while (level < Limit)
				{
					if (CubicCurve2D.GetFlatnessSq(Hold, HoldIndex) < Squareflat)
					{
						break;
					}

					EnsureHoldCapacity(6);
					CubicCurve2D.Subdivide(Hold, HoldIndex, Hold, HoldIndex - 6, Hold, HoldIndex);
					HoldIndex -= 6;

					// Now that we have subdivided, we have constructed
					// two curves of one depth lower than the original
					// curve.  One of those curves is in the place of
					// the former curve and one of them is in the next
					// set of held coordinate slots.  We now set both
					// curves level values to the next higher level.
					level++;
					Levels[LevelIndex] = level;
					LevelIndex++;
					Levels[LevelIndex] = level;
				}

				// This curve segment is flat enough, or it is too deep
				// in recursion levels to try to flatten any more.  The
				// two coordinates at holdIndex+6 and holdIndex+7 now
				// contain the endpoint of the curve which can be the
				// endpoint of an approximating line segment.
				HoldIndex += 6;
				LevelIndex--;
				break;
			}
		}

		/// <summary>
		/// Returns the coordinates and type of the current path segment in
		/// the iteration.
		/// The return value is the path segment type:
		/// SEG_MOVETO, SEG_LINETO, or SEG_CLOSE.
		/// A float array of length 6 must be passed in and can be used to
		/// store the coordinates of the point(s).
		/// Each point is stored as a pair of float x,y coordinates.
		/// SEG_MOVETO and SEG_LINETO types return one point,
		/// and SEG_CLOSE does not return any points. </summary>
		/// <param name="coords"> an array that holds the data returned from
		/// this method </param>
		/// <returns> the path segment type of the current path segment. </returns>
		/// <exception cref="NoSuchElementException"> if there
		///          are no more elements in the flattening path to be
		///          returned. </exception>
		/// <seealso cref= PathIterator#SEG_MOVETO </seealso>
		/// <seealso cref= PathIterator#SEG_LINETO </seealso>
		/// <seealso cref= PathIterator#SEG_CLOSE </seealso>
		public virtual int CurrentSegment(float[] coords)
		{
			if (Done)
			{
				throw new NoSuchElementException("flattening iterator out of bounds");
			}
			int type = HoldType;
			if (type != PathIterator_Fields.SEG_CLOSE)
			{
				coords[0] = (float) Hold[HoldIndex + 0];
				coords[1] = (float) Hold[HoldIndex + 1];
				if (type != PathIterator_Fields.SEG_MOVETO)
				{
					type = PathIterator_Fields.SEG_LINETO;
				}
			}
			return type;
		}

		/// <summary>
		/// Returns the coordinates and type of the current path segment in
		/// the iteration.
		/// The return value is the path segment type:
		/// SEG_MOVETO, SEG_LINETO, or SEG_CLOSE.
		/// A double array of length 6 must be passed in and can be used to
		/// store the coordinates of the point(s).
		/// Each point is stored as a pair of double x,y coordinates.
		/// SEG_MOVETO and SEG_LINETO types return one point,
		/// and SEG_CLOSE does not return any points. </summary>
		/// <param name="coords"> an array that holds the data returned from
		/// this method </param>
		/// <returns> the path segment type of the current path segment. </returns>
		/// <exception cref="NoSuchElementException"> if there
		///          are no more elements in the flattening path to be
		///          returned. </exception>
		/// <seealso cref= PathIterator#SEG_MOVETO </seealso>
		/// <seealso cref= PathIterator#SEG_LINETO </seealso>
		/// <seealso cref= PathIterator#SEG_CLOSE </seealso>
		public virtual int CurrentSegment(double[] coords)
		{
			if (Done)
			{
				throw new NoSuchElementException("flattening iterator out of bounds");
			}
			int type = HoldType;
			if (type != PathIterator_Fields.SEG_CLOSE)
			{
				coords[0] = Hold[HoldIndex + 0];
				coords[1] = Hold[HoldIndex + 1];
				if (type != PathIterator_Fields.SEG_MOVETO)
				{
					type = PathIterator_Fields.SEG_LINETO;
				}
			}
			return type;
		}
	}

}
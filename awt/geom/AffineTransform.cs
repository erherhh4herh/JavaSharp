using System;

/*
 * Copyright (c) 1996, 2013, Oracle and/or its affiliates. All rights reserved.
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
	/// The <code>AffineTransform</code> class represents a 2D affine transform
	/// that performs a linear mapping from 2D coordinates to other 2D
	/// coordinates that preserves the "straightness" and
	/// "parallelness" of lines.  Affine transformations can be constructed
	/// using sequences of translations, scales, flips, rotations, and shears.
	/// <para>
	/// Such a coordinate transformation can be represented by a 3 row by
	/// 3 column matrix with an implied last row of [ 0 0 1 ].  This matrix
	/// transforms source coordinates {@code (x,y)} into
	/// destination coordinates {@code (x',y')} by considering
	/// them to be a column vector and multiplying the coordinate vector
	/// by the matrix according to the following process:
	/// <pre>
	///      [ x']   [  m00  m01  m02  ] [ x ]   [ m00x + m01y + m02 ]
	///      [ y'] = [  m10  m11  m12  ] [ y ] = [ m10x + m11y + m12 ]
	///      [ 1 ]   [   0    0    1   ] [ 1 ]   [         1         ]
	/// </pre>
	/// <h3><a name="quadrantapproximation">Handling 90-Degree Rotations</a></h3>
	/// </para>
	/// <para>
	/// In some variations of the <code>rotate</code> methods in the
	/// <code>AffineTransform</code> class, a double-precision argument
	/// specifies the angle of rotation in radians.
	/// These methods have special handling for rotations of approximately
	/// 90 degrees (including multiples such as 180, 270, and 360 degrees),
	/// so that the common case of quadrant rotation is handled more
	/// efficiently.
	/// This special handling can cause angles very close to multiples of
	/// 90 degrees to be treated as if they were exact multiples of
	/// 90 degrees.
	/// For small multiples of 90 degrees the range of angles treated
	/// as a quadrant rotation is approximately 0.00000121 degrees wide.
	/// This section explains why such special care is needed and how
	/// it is implemented.
	/// </para>
	/// <para>
	/// Since 90 degrees is represented as <code>PI/2</code> in radians,
	/// and since PI is a transcendental (and therefore irrational) number,
	/// it is not possible to exactly represent a multiple of 90 degrees as
	/// an exact double precision value measured in radians.
	/// As a result it is theoretically impossible to describe quadrant
	/// rotations (90, 180, 270 or 360 degrees) using these values.
	/// Double precision floating point values can get very close to
	/// non-zero multiples of <code>PI/2</code> but never close enough
	/// for the sine or cosine to be exactly 0.0, 1.0 or -1.0.
	/// The implementations of <code>Math.sin()</code> and
	/// <code>Math.cos()</code> correspondingly never return 0.0
	/// for any case other than <code>Math.sin(0.0)</code>.
	/// These same implementations do, however, return exactly 1.0 and
	/// -1.0 for some range of numbers around each multiple of 90
	/// degrees since the correct answer is so close to 1.0 or -1.0 that
	/// the double precision significand cannot represent the difference
	/// as accurately as it can for numbers that are near 0.0.
	/// </para>
	/// <para>
	/// The net result of these issues is that if the
	/// <code>Math.sin()</code> and <code>Math.cos()</code> methods
	/// are used to directly generate the values for the matrix modifications
	/// during these radian-based rotation operations then the resulting
	/// transform is never strictly classifiable as a quadrant rotation
	/// even for a simple case like <code>rotate(Math.PI/2.0)</code>,
	/// due to minor variations in the matrix caused by the non-0.0 values
	/// obtained for the sine and cosine.
	/// If these transforms are not classified as quadrant rotations then
	/// subsequent code which attempts to optimize further operations based
	/// upon the type of the transform will be relegated to its most general
	/// implementation.
	/// </para>
	/// <para>
	/// Because quadrant rotations are fairly common,
	/// this class should handle these cases reasonably quickly, both in
	/// applying the rotations to the transform and in applying the resulting
	/// transform to the coordinates.
	/// To facilitate this optimal handling, the methods which take an angle
	/// of rotation measured in radians attempt to detect angles that are
	/// intended to be quadrant rotations and treat them as such.
	/// These methods therefore treat an angle <em>theta</em> as a quadrant
	/// rotation if either <code>Math.sin(<em>theta</em>)</code> or
	/// <code>Math.cos(<em>theta</em>)</code> returns exactly 1.0 or -1.0.
	/// As a rule of thumb, this property holds true for a range of
	/// approximately 0.0000000211 radians (or 0.00000121 degrees) around
	/// small multiples of <code>Math.PI/2.0</code>.
	/// 
	/// @author Jim Graham
	/// @since 1.2
	/// </para>
	/// </summary>
	[Serializable]
	public class AffineTransform : Cloneable
	{

		/*
		 * This constant is only useful for the cached type field.
		 * It indicates that the type has been decached and must be recalculated.
		 */
		private const int TYPE_UNKNOWN = -1;

		/// <summary>
		/// This constant indicates that the transform defined by this object
		/// is an identity transform.
		/// An identity transform is one in which the output coordinates are
		/// always the same as the input coordinates.
		/// If this transform is anything other than the identity transform,
		/// the type will either be the constant GENERAL_TRANSFORM or a
		/// combination of the appropriate flag bits for the various coordinate
		/// conversions that this transform performs. </summary>
		/// <seealso cref= #TYPE_TRANSLATION </seealso>
		/// <seealso cref= #TYPE_UNIFORM_SCALE </seealso>
		/// <seealso cref= #TYPE_GENERAL_SCALE </seealso>
		/// <seealso cref= #TYPE_FLIP </seealso>
		/// <seealso cref= #TYPE_QUADRANT_ROTATION </seealso>
		/// <seealso cref= #TYPE_GENERAL_ROTATION </seealso>
		/// <seealso cref= #TYPE_GENERAL_TRANSFORM </seealso>
		/// <seealso cref= #getType
		/// @since 1.2 </seealso>
		public const int TYPE_IDENTITY = 0;

		/// <summary>
		/// This flag bit indicates that the transform defined by this object
		/// performs a translation in addition to the conversions indicated
		/// by other flag bits.
		/// A translation moves the coordinates by a constant amount in x
		/// and y without changing the length or angle of vectors. </summary>
		/// <seealso cref= #TYPE_IDENTITY </seealso>
		/// <seealso cref= #TYPE_UNIFORM_SCALE </seealso>
		/// <seealso cref= #TYPE_GENERAL_SCALE </seealso>
		/// <seealso cref= #TYPE_FLIP </seealso>
		/// <seealso cref= #TYPE_QUADRANT_ROTATION </seealso>
		/// <seealso cref= #TYPE_GENERAL_ROTATION </seealso>
		/// <seealso cref= #TYPE_GENERAL_TRANSFORM </seealso>
		/// <seealso cref= #getType
		/// @since 1.2 </seealso>
		public const int TYPE_TRANSLATION = 1;

		/// <summary>
		/// This flag bit indicates that the transform defined by this object
		/// performs a uniform scale in addition to the conversions indicated
		/// by other flag bits.
		/// A uniform scale multiplies the length of vectors by the same amount
		/// in both the x and y directions without changing the angle between
		/// vectors.
		/// This flag bit is mutually exclusive with the TYPE_GENERAL_SCALE flag. </summary>
		/// <seealso cref= #TYPE_IDENTITY </seealso>
		/// <seealso cref= #TYPE_TRANSLATION </seealso>
		/// <seealso cref= #TYPE_GENERAL_SCALE </seealso>
		/// <seealso cref= #TYPE_FLIP </seealso>
		/// <seealso cref= #TYPE_QUADRANT_ROTATION </seealso>
		/// <seealso cref= #TYPE_GENERAL_ROTATION </seealso>
		/// <seealso cref= #TYPE_GENERAL_TRANSFORM </seealso>
		/// <seealso cref= #getType
		/// @since 1.2 </seealso>
		public const int TYPE_UNIFORM_SCALE = 2;

		/// <summary>
		/// This flag bit indicates that the transform defined by this object
		/// performs a general scale in addition to the conversions indicated
		/// by other flag bits.
		/// A general scale multiplies the length of vectors by different
		/// amounts in the x and y directions without changing the angle
		/// between perpendicular vectors.
		/// This flag bit is mutually exclusive with the TYPE_UNIFORM_SCALE flag. </summary>
		/// <seealso cref= #TYPE_IDENTITY </seealso>
		/// <seealso cref= #TYPE_TRANSLATION </seealso>
		/// <seealso cref= #TYPE_UNIFORM_SCALE </seealso>
		/// <seealso cref= #TYPE_FLIP </seealso>
		/// <seealso cref= #TYPE_QUADRANT_ROTATION </seealso>
		/// <seealso cref= #TYPE_GENERAL_ROTATION </seealso>
		/// <seealso cref= #TYPE_GENERAL_TRANSFORM </seealso>
		/// <seealso cref= #getType
		/// @since 1.2 </seealso>
		public const int TYPE_GENERAL_SCALE = 4;

		/// <summary>
		/// This constant is a bit mask for any of the scale flag bits. </summary>
		/// <seealso cref= #TYPE_UNIFORM_SCALE </seealso>
		/// <seealso cref= #TYPE_GENERAL_SCALE
		/// @since 1.2 </seealso>
		public static readonly int TYPE_MASK_SCALE = (TYPE_UNIFORM_SCALE | TYPE_GENERAL_SCALE);

		/// <summary>
		/// This flag bit indicates that the transform defined by this object
		/// performs a mirror image flip about some axis which changes the
		/// normally right handed coordinate system into a left handed
		/// system in addition to the conversions indicated by other flag bits.
		/// A right handed coordinate system is one where the positive X
		/// axis rotates counterclockwise to overlay the positive Y axis
		/// similar to the direction that the fingers on your right hand
		/// curl when you stare end on at your thumb.
		/// A left handed coordinate system is one where the positive X
		/// axis rotates clockwise to overlay the positive Y axis similar
		/// to the direction that the fingers on your left hand curl.
		/// There is no mathematical way to determine the angle of the
		/// original flipping or mirroring transformation since all angles
		/// of flip are identical given an appropriate adjusting rotation. </summary>
		/// <seealso cref= #TYPE_IDENTITY </seealso>
		/// <seealso cref= #TYPE_TRANSLATION </seealso>
		/// <seealso cref= #TYPE_UNIFORM_SCALE </seealso>
		/// <seealso cref= #TYPE_GENERAL_SCALE </seealso>
		/// <seealso cref= #TYPE_QUADRANT_ROTATION </seealso>
		/// <seealso cref= #TYPE_GENERAL_ROTATION </seealso>
		/// <seealso cref= #TYPE_GENERAL_TRANSFORM </seealso>
		/// <seealso cref= #getType
		/// @since 1.2 </seealso>
		public const int TYPE_FLIP = 64;
		/* NOTE: TYPE_FLIP was added after GENERAL_TRANSFORM was in public
		 * circulation and the flag bits could no longer be conveniently
		 * renumbered without introducing binary incompatibility in outside
		 * code.
		 */

		/// <summary>
		/// This flag bit indicates that the transform defined by this object
		/// performs a quadrant rotation by some multiple of 90 degrees in
		/// addition to the conversions indicated by other flag bits.
		/// A rotation changes the angles of vectors by the same amount
		/// regardless of the original direction of the vector and without
		/// changing the length of the vector.
		/// This flag bit is mutually exclusive with the TYPE_GENERAL_ROTATION flag. </summary>
		/// <seealso cref= #TYPE_IDENTITY </seealso>
		/// <seealso cref= #TYPE_TRANSLATION </seealso>
		/// <seealso cref= #TYPE_UNIFORM_SCALE </seealso>
		/// <seealso cref= #TYPE_GENERAL_SCALE </seealso>
		/// <seealso cref= #TYPE_FLIP </seealso>
		/// <seealso cref= #TYPE_GENERAL_ROTATION </seealso>
		/// <seealso cref= #TYPE_GENERAL_TRANSFORM </seealso>
		/// <seealso cref= #getType
		/// @since 1.2 </seealso>
		public const int TYPE_QUADRANT_ROTATION = 8;

		/// <summary>
		/// This flag bit indicates that the transform defined by this object
		/// performs a rotation by an arbitrary angle in addition to the
		/// conversions indicated by other flag bits.
		/// A rotation changes the angles of vectors by the same amount
		/// regardless of the original direction of the vector and without
		/// changing the length of the vector.
		/// This flag bit is mutually exclusive with the
		/// TYPE_QUADRANT_ROTATION flag. </summary>
		/// <seealso cref= #TYPE_IDENTITY </seealso>
		/// <seealso cref= #TYPE_TRANSLATION </seealso>
		/// <seealso cref= #TYPE_UNIFORM_SCALE </seealso>
		/// <seealso cref= #TYPE_GENERAL_SCALE </seealso>
		/// <seealso cref= #TYPE_FLIP </seealso>
		/// <seealso cref= #TYPE_QUADRANT_ROTATION </seealso>
		/// <seealso cref= #TYPE_GENERAL_TRANSFORM </seealso>
		/// <seealso cref= #getType
		/// @since 1.2 </seealso>
		public const int TYPE_GENERAL_ROTATION = 16;

		/// <summary>
		/// This constant is a bit mask for any of the rotation flag bits. </summary>
		/// <seealso cref= #TYPE_QUADRANT_ROTATION </seealso>
		/// <seealso cref= #TYPE_GENERAL_ROTATION
		/// @since 1.2 </seealso>
		public static readonly int TYPE_MASK_ROTATION = (TYPE_QUADRANT_ROTATION | TYPE_GENERAL_ROTATION);

		/// <summary>
		/// This constant indicates that the transform defined by this object
		/// performs an arbitrary conversion of the input coordinates.
		/// If this transform can be classified by any of the above constants,
		/// the type will either be the constant TYPE_IDENTITY or a
		/// combination of the appropriate flag bits for the various coordinate
		/// conversions that this transform performs. </summary>
		/// <seealso cref= #TYPE_IDENTITY </seealso>
		/// <seealso cref= #TYPE_TRANSLATION </seealso>
		/// <seealso cref= #TYPE_UNIFORM_SCALE </seealso>
		/// <seealso cref= #TYPE_GENERAL_SCALE </seealso>
		/// <seealso cref= #TYPE_FLIP </seealso>
		/// <seealso cref= #TYPE_QUADRANT_ROTATION </seealso>
		/// <seealso cref= #TYPE_GENERAL_ROTATION </seealso>
		/// <seealso cref= #getType
		/// @since 1.2 </seealso>
		public const int TYPE_GENERAL_TRANSFORM = 32;

		/// <summary>
		/// This constant is used for the internal state variable to indicate
		/// that no calculations need to be performed and that the source
		/// coordinates only need to be copied to their destinations to
		/// complete the transformation equation of this transform. </summary>
		/// <seealso cref= #APPLY_TRANSLATE </seealso>
		/// <seealso cref= #APPLY_SCALE </seealso>
		/// <seealso cref= #APPLY_SHEAR </seealso>
		/// <seealso cref= #state </seealso>
		internal const int APPLY_IDENTITY = 0;

		/// <summary>
		/// This constant is used for the internal state variable to indicate
		/// that the translation components of the matrix (m02 and m12) need
		/// to be added to complete the transformation equation of this transform. </summary>
		/// <seealso cref= #APPLY_IDENTITY </seealso>
		/// <seealso cref= #APPLY_SCALE </seealso>
		/// <seealso cref= #APPLY_SHEAR </seealso>
		/// <seealso cref= #state </seealso>
		internal const int APPLY_TRANSLATE = 1;

		/// <summary>
		/// This constant is used for the internal state variable to indicate
		/// that the scaling components of the matrix (m00 and m11) need
		/// to be factored in to complete the transformation equation of
		/// this transform.  If the APPLY_SHEAR bit is also set then it
		/// indicates that the scaling components are not both 0.0.  If the
		/// APPLY_SHEAR bit is not also set then it indicates that the
		/// scaling components are not both 1.0.  If neither the APPLY_SHEAR
		/// nor the APPLY_SCALE bits are set then the scaling components
		/// are both 1.0, which means that the x and y components contribute
		/// to the transformed coordinate, but they are not multiplied by
		/// any scaling factor. </summary>
		/// <seealso cref= #APPLY_IDENTITY </seealso>
		/// <seealso cref= #APPLY_TRANSLATE </seealso>
		/// <seealso cref= #APPLY_SHEAR </seealso>
		/// <seealso cref= #state </seealso>
		internal const int APPLY_SCALE = 2;

		/// <summary>
		/// This constant is used for the internal state variable to indicate
		/// that the shearing components of the matrix (m01 and m10) need
		/// to be factored in to complete the transformation equation of this
		/// transform.  The presence of this bit in the state variable changes
		/// the interpretation of the APPLY_SCALE bit as indicated in its
		/// documentation. </summary>
		/// <seealso cref= #APPLY_IDENTITY </seealso>
		/// <seealso cref= #APPLY_TRANSLATE </seealso>
		/// <seealso cref= #APPLY_SCALE </seealso>
		/// <seealso cref= #state </seealso>
		internal const int APPLY_SHEAR = 4;

		/*
		 * For methods which combine together the state of two separate
		 * transforms and dispatch based upon the combination, these constants
		 * specify how far to shift one of the states so that the two states
		 * are mutually non-interfering and provide constants for testing the
		 * bits of the shifted (HI) state.  The methods in this class use
		 * the convention that the state of "this" transform is unshifted and
		 * the state of the "other" or "argument" transform is shifted (HI).
		 */
		private const int HI_SHIFT = 3;
		private static readonly int HI_IDENTITY = APPLY_IDENTITY << HI_SHIFT;
		private static readonly int HI_TRANSLATE = APPLY_TRANSLATE << HI_SHIFT;
		private static readonly int HI_SCALE = APPLY_SCALE << HI_SHIFT;
		private static readonly int HI_SHEAR = APPLY_SHEAR << HI_SHIFT;

		/// <summary>
		/// The X coordinate scaling element of the 3x3
		/// affine transformation matrix.
		/// 
		/// @serial
		/// </summary>
		internal double M00;

		/// <summary>
		/// The Y coordinate shearing element of the 3x3
		/// affine transformation matrix.
		/// 
		/// @serial
		/// </summary>
		 internal double M10;

		/// <summary>
		/// The X coordinate shearing element of the 3x3
		/// affine transformation matrix.
		/// 
		/// @serial
		/// </summary>
		 internal double M01;

		/// <summary>
		/// The Y coordinate scaling element of the 3x3
		/// affine transformation matrix.
		/// 
		/// @serial
		/// </summary>
		 internal double M11;

		/// <summary>
		/// The X coordinate of the translation element of the
		/// 3x3 affine transformation matrix.
		/// 
		/// @serial
		/// </summary>
		 internal double M02;

		/// <summary>
		/// The Y coordinate of the translation element of the
		/// 3x3 affine transformation matrix.
		/// 
		/// @serial
		/// </summary>
		 internal double M12;

		/// <summary>
		/// This field keeps track of which components of the matrix need to
		/// be applied when performing a transformation. </summary>
		/// <seealso cref= #APPLY_IDENTITY </seealso>
		/// <seealso cref= #APPLY_TRANSLATE </seealso>
		/// <seealso cref= #APPLY_SCALE </seealso>
		/// <seealso cref= #APPLY_SHEAR </seealso>
		[NonSerialized]
		internal int State;

		/// <summary>
		/// This field caches the current transformation type of the matrix. </summary>
		/// <seealso cref= #TYPE_IDENTITY </seealso>
		/// <seealso cref= #TYPE_TRANSLATION </seealso>
		/// <seealso cref= #TYPE_UNIFORM_SCALE </seealso>
		/// <seealso cref= #TYPE_GENERAL_SCALE </seealso>
		/// <seealso cref= #TYPE_FLIP </seealso>
		/// <seealso cref= #TYPE_QUADRANT_ROTATION </seealso>
		/// <seealso cref= #TYPE_GENERAL_ROTATION </seealso>
		/// <seealso cref= #TYPE_GENERAL_TRANSFORM </seealso>
		/// <seealso cref= #TYPE_UNKNOWN </seealso>
		/// <seealso cref= #getType </seealso>
		[NonSerialized]
		private int Type_Renamed;

		private AffineTransform(double m00, double m10, double m01, double m11, double m02, double m12, int state)
		{
			this.M00 = m00;
			this.M10 = m10;
			this.M01 = m01;
			this.M11 = m11;
			this.M02 = m02;
			this.M12 = m12;
			this.State = state;
			this.Type_Renamed = TYPE_UNKNOWN;
		}

		/// <summary>
		/// Constructs a new <code>AffineTransform</code> representing the
		/// Identity transformation.
		/// @since 1.2
		/// </summary>
		public AffineTransform()
		{
			M00 = M11 = 1.0;
			// m01 = m10 = m02 = m12 = 0.0;         /* Not needed. */
			// state = APPLY_IDENTITY;              /* Not needed. */
			// type = TYPE_IDENTITY;                /* Not needed. */
		}

		/// <summary>
		/// Constructs a new <code>AffineTransform</code> that is a copy of
		/// the specified <code>AffineTransform</code> object. </summary>
		/// <param name="Tx"> the <code>AffineTransform</code> object to copy
		/// @since 1.2 </param>
		public AffineTransform(AffineTransform Tx)
		{
			this.M00 = Tx.M00;
			this.M10 = Tx.M10;
			this.M01 = Tx.M01;
			this.M11 = Tx.M11;
			this.M02 = Tx.M02;
			this.M12 = Tx.M12;
			this.State = Tx.State;
			this.Type_Renamed = Tx.Type_Renamed;
		}

		/// <summary>
		/// Constructs a new <code>AffineTransform</code> from 6 floating point
		/// values representing the 6 specifiable entries of the 3x3
		/// transformation matrix.
		/// </summary>
		/// <param name="m00"> the X coordinate scaling element of the 3x3 matrix </param>
		/// <param name="m10"> the Y coordinate shearing element of the 3x3 matrix </param>
		/// <param name="m01"> the X coordinate shearing element of the 3x3 matrix </param>
		/// <param name="m11"> the Y coordinate scaling element of the 3x3 matrix </param>
		/// <param name="m02"> the X coordinate translation element of the 3x3 matrix </param>
		/// <param name="m12"> the Y coordinate translation element of the 3x3 matrix
		/// @since 1.2 </param>
//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @ConstructorProperties({ "scaleX", "shearY", "shearX", "scaleY", "translateX", "translateY" }) public AffineTransform(float m00, float m10, float m01, float m11, float m02, float m12)
		public AffineTransform(float m00, float m10, float m01, float m11, float m02, float m12)
		{
			this.M00 = m00;
			this.M10 = m10;
			this.M01 = m01;
			this.M11 = m11;
			this.M02 = m02;
			this.M12 = m12;
			UpdateState();
		}

		/// <summary>
		/// Constructs a new <code>AffineTransform</code> from an array of
		/// floating point values representing either the 4 non-translation
		/// entries or the 6 specifiable entries of the 3x3 transformation
		/// matrix.  The values are retrieved from the array as
		/// {&nbsp;m00&nbsp;m10&nbsp;m01&nbsp;m11&nbsp;[m02&nbsp;m12]}. </summary>
		/// <param name="flatmatrix"> the float array containing the values to be set
		/// in the new <code>AffineTransform</code> object. The length of the
		/// array is assumed to be at least 4. If the length of the array is
		/// less than 6, only the first 4 values are taken. If the length of
		/// the array is greater than 6, the first 6 values are taken.
		/// @since 1.2 </param>
		public AffineTransform(float[] flatmatrix)
		{
			M00 = flatmatrix[0];
			M10 = flatmatrix[1];
			M01 = flatmatrix[2];
			M11 = flatmatrix[3];
			if (flatmatrix.Length > 5)
			{
				M02 = flatmatrix[4];
				M12 = flatmatrix[5];
			}
			UpdateState();
		}

		/// <summary>
		/// Constructs a new <code>AffineTransform</code> from 6 double
		/// precision values representing the 6 specifiable entries of the 3x3
		/// transformation matrix.
		/// </summary>
		/// <param name="m00"> the X coordinate scaling element of the 3x3 matrix </param>
		/// <param name="m10"> the Y coordinate shearing element of the 3x3 matrix </param>
		/// <param name="m01"> the X coordinate shearing element of the 3x3 matrix </param>
		/// <param name="m11"> the Y coordinate scaling element of the 3x3 matrix </param>
		/// <param name="m02"> the X coordinate translation element of the 3x3 matrix </param>
		/// <param name="m12"> the Y coordinate translation element of the 3x3 matrix
		/// @since 1.2 </param>
		public AffineTransform(double m00, double m10, double m01, double m11, double m02, double m12)
		{
			this.M00 = m00;
			this.M10 = m10;
			this.M01 = m01;
			this.M11 = m11;
			this.M02 = m02;
			this.M12 = m12;
			UpdateState();
		}

		/// <summary>
		/// Constructs a new <code>AffineTransform</code> from an array of
		/// double precision values representing either the 4 non-translation
		/// entries or the 6 specifiable entries of the 3x3 transformation
		/// matrix. The values are retrieved from the array as
		/// {&nbsp;m00&nbsp;m10&nbsp;m01&nbsp;m11&nbsp;[m02&nbsp;m12]}. </summary>
		/// <param name="flatmatrix"> the double array containing the values to be set
		/// in the new <code>AffineTransform</code> object. The length of the
		/// array is assumed to be at least 4. If the length of the array is
		/// less than 6, only the first 4 values are taken. If the length of
		/// the array is greater than 6, the first 6 values are taken.
		/// @since 1.2 </param>
		public AffineTransform(double[] flatmatrix)
		{
			M00 = flatmatrix[0];
			M10 = flatmatrix[1];
			M01 = flatmatrix[2];
			M11 = flatmatrix[3];
			if (flatmatrix.Length > 5)
			{
				M02 = flatmatrix[4];
				M12 = flatmatrix[5];
			}
			UpdateState();
		}

		/// <summary>
		/// Returns a transform representing a translation transformation.
		/// The matrix representing the returned transform is:
		/// <pre>
		///          [   1    0    tx  ]
		///          [   0    1    ty  ]
		///          [   0    0    1   ]
		/// </pre> </summary>
		/// <param name="tx"> the distance by which coordinates are translated in the
		/// X axis direction </param>
		/// <param name="ty"> the distance by which coordinates are translated in the
		/// Y axis direction </param>
		/// <returns> an <code>AffineTransform</code> object that represents a
		///  translation transformation, created with the specified vector.
		/// @since 1.2 </returns>
		public static AffineTransform GetTranslateInstance(double tx, double ty)
		{
			AffineTransform Tx = new AffineTransform();
			Tx.SetToTranslation(tx, ty);
			return Tx;
		}

		/// <summary>
		/// Returns a transform representing a rotation transformation.
		/// The matrix representing the returned transform is:
		/// <pre>
		///          [   cos(theta)    -sin(theta)    0   ]
		///          [   sin(theta)     cos(theta)    0   ]
		///          [       0              0         1   ]
		/// </pre>
		/// Rotating by a positive angle theta rotates points on the positive
		/// X axis toward the positive Y axis.
		/// Note also the discussion of
		/// <a href="#quadrantapproximation">Handling 90-Degree Rotations</a>
		/// above. </summary>
		/// <param name="theta"> the angle of rotation measured in radians </param>
		/// <returns> an <code>AffineTransform</code> object that is a rotation
		///  transformation, created with the specified angle of rotation.
		/// @since 1.2 </returns>
		public static AffineTransform GetRotateInstance(double theta)
		{
			AffineTransform Tx = new AffineTransform();
			Tx.ToRotation = theta;
			return Tx;
		}

		/// <summary>
		/// Returns a transform that rotates coordinates around an anchor point.
		/// This operation is equivalent to translating the coordinates so
		/// that the anchor point is at the origin (S1), then rotating them
		/// about the new origin (S2), and finally translating so that the
		/// intermediate origin is restored to the coordinates of the original
		/// anchor point (S3).
		/// <para>
		/// This operation is equivalent to the following sequence of calls:
		/// <pre>
		///     AffineTransform Tx = new AffineTransform();
		///     Tx.translate(anchorx, anchory);    // S3: final translation
		///     Tx.rotate(theta);                  // S2: rotate around anchor
		///     Tx.translate(-anchorx, -anchory);  // S1: translate anchor to origin
		/// </pre>
		/// The matrix representing the returned transform is:
		/// <pre>
		///          [   cos(theta)    -sin(theta)    x-x*cos+y*sin  ]
		///          [   sin(theta)     cos(theta)    y-x*sin-y*cos  ]
		///          [       0              0               1        ]
		/// </pre>
		/// Rotating by a positive angle theta rotates points on the positive
		/// X axis toward the positive Y axis.
		/// Note also the discussion of
		/// <a href="#quadrantapproximation">Handling 90-Degree Rotations</a>
		/// above.
		/// 
		/// </para>
		/// </summary>
		/// <param name="theta"> the angle of rotation measured in radians </param>
		/// <param name="anchorx"> the X coordinate of the rotation anchor point </param>
		/// <param name="anchory"> the Y coordinate of the rotation anchor point </param>
		/// <returns> an <code>AffineTransform</code> object that rotates
		///  coordinates around the specified point by the specified angle of
		///  rotation.
		/// @since 1.2 </returns>
		public static AffineTransform GetRotateInstance(double theta, double anchorx, double anchory)
		{
			AffineTransform Tx = new AffineTransform();
			Tx.SetToRotation(theta, anchorx, anchory);
			return Tx;
		}

		/// <summary>
		/// Returns a transform that rotates coordinates according to
		/// a rotation vector.
		/// All coordinates rotate about the origin by the same amount.
		/// The amount of rotation is such that coordinates along the former
		/// positive X axis will subsequently align with the vector pointing
		/// from the origin to the specified vector coordinates.
		/// If both <code>vecx</code> and <code>vecy</code> are 0.0,
		/// an identity transform is returned.
		/// This operation is equivalent to calling:
		/// <pre>
		///     AffineTransform.getRotateInstance(Math.atan2(vecy, vecx));
		/// </pre>
		/// </summary>
		/// <param name="vecx"> the X coordinate of the rotation vector </param>
		/// <param name="vecy"> the Y coordinate of the rotation vector </param>
		/// <returns> an <code>AffineTransform</code> object that rotates
		///  coordinates according to the specified rotation vector.
		/// @since 1.6 </returns>
		public static AffineTransform GetRotateInstance(double vecx, double vecy)
		{
			AffineTransform Tx = new AffineTransform();
			Tx.SetToRotation(vecx, vecy);
			return Tx;
		}

		/// <summary>
		/// Returns a transform that rotates coordinates around an anchor
		/// point according to a rotation vector.
		/// All coordinates rotate about the specified anchor coordinates
		/// by the same amount.
		/// The amount of rotation is such that coordinates along the former
		/// positive X axis will subsequently align with the vector pointing
		/// from the origin to the specified vector coordinates.
		/// If both <code>vecx</code> and <code>vecy</code> are 0.0,
		/// an identity transform is returned.
		/// This operation is equivalent to calling:
		/// <pre>
		///     AffineTransform.getRotateInstance(Math.atan2(vecy, vecx),
		///                                       anchorx, anchory);
		/// </pre>
		/// </summary>
		/// <param name="vecx"> the X coordinate of the rotation vector </param>
		/// <param name="vecy"> the Y coordinate of the rotation vector </param>
		/// <param name="anchorx"> the X coordinate of the rotation anchor point </param>
		/// <param name="anchory"> the Y coordinate of the rotation anchor point </param>
		/// <returns> an <code>AffineTransform</code> object that rotates
		///  coordinates around the specified point according to the
		///  specified rotation vector.
		/// @since 1.6 </returns>
		public static AffineTransform GetRotateInstance(double vecx, double vecy, double anchorx, double anchory)
		{
			AffineTransform Tx = new AffineTransform();
			Tx.SetToRotation(vecx, vecy, anchorx, anchory);
			return Tx;
		}

		/// <summary>
		/// Returns a transform that rotates coordinates by the specified
		/// number of quadrants.
		/// This operation is equivalent to calling:
		/// <pre>
		///     AffineTransform.getRotateInstance(numquadrants * Math.PI / 2.0);
		/// </pre>
		/// Rotating by a positive number of quadrants rotates points on
		/// the positive X axis toward the positive Y axis. </summary>
		/// <param name="numquadrants"> the number of 90 degree arcs to rotate by </param>
		/// <returns> an <code>AffineTransform</code> object that rotates
		///  coordinates by the specified number of quadrants.
		/// @since 1.6 </returns>
		public static AffineTransform GetQuadrantRotateInstance(int numquadrants)
		{
			AffineTransform Tx = new AffineTransform();
			Tx.ToQuadrantRotation = numquadrants;
			return Tx;
		}

		/// <summary>
		/// Returns a transform that rotates coordinates by the specified
		/// number of quadrants around the specified anchor point.
		/// This operation is equivalent to calling:
		/// <pre>
		///     AffineTransform.getRotateInstance(numquadrants * Math.PI / 2.0,
		///                                       anchorx, anchory);
		/// </pre>
		/// Rotating by a positive number of quadrants rotates points on
		/// the positive X axis toward the positive Y axis.
		/// </summary>
		/// <param name="numquadrants"> the number of 90 degree arcs to rotate by </param>
		/// <param name="anchorx"> the X coordinate of the rotation anchor point </param>
		/// <param name="anchory"> the Y coordinate of the rotation anchor point </param>
		/// <returns> an <code>AffineTransform</code> object that rotates
		///  coordinates by the specified number of quadrants around the
		///  specified anchor point.
		/// @since 1.6 </returns>
		public static AffineTransform GetQuadrantRotateInstance(int numquadrants, double anchorx, double anchory)
		{
			AffineTransform Tx = new AffineTransform();
			Tx.SetToQuadrantRotation(numquadrants, anchorx, anchory);
			return Tx;
		}

		/// <summary>
		/// Returns a transform representing a scaling transformation.
		/// The matrix representing the returned transform is:
		/// <pre>
		///          [   sx   0    0   ]
		///          [   0    sy   0   ]
		///          [   0    0    1   ]
		/// </pre> </summary>
		/// <param name="sx"> the factor by which coordinates are scaled along the
		/// X axis direction </param>
		/// <param name="sy"> the factor by which coordinates are scaled along the
		/// Y axis direction </param>
		/// <returns> an <code>AffineTransform</code> object that scales
		///  coordinates by the specified factors.
		/// @since 1.2 </returns>
		public static AffineTransform GetScaleInstance(double sx, double sy)
		{
			AffineTransform Tx = new AffineTransform();
			Tx.SetToScale(sx, sy);
			return Tx;
		}

		/// <summary>
		/// Returns a transform representing a shearing transformation.
		/// The matrix representing the returned transform is:
		/// <pre>
		///          [   1   shx   0   ]
		///          [  shy   1    0   ]
		///          [   0    0    1   ]
		/// </pre> </summary>
		/// <param name="shx"> the multiplier by which coordinates are shifted in the
		/// direction of the positive X axis as a factor of their Y coordinate </param>
		/// <param name="shy"> the multiplier by which coordinates are shifted in the
		/// direction of the positive Y axis as a factor of their X coordinate </param>
		/// <returns> an <code>AffineTransform</code> object that shears
		///  coordinates by the specified multipliers.
		/// @since 1.2 </returns>
		public static AffineTransform GetShearInstance(double shx, double shy)
		{
			AffineTransform Tx = new AffineTransform();
			Tx.SetToShear(shx, shy);
			return Tx;
		}

		/// <summary>
		/// Retrieves the flag bits describing the conversion properties of
		/// this transform.
		/// The return value is either one of the constants TYPE_IDENTITY
		/// or TYPE_GENERAL_TRANSFORM, or a combination of the
		/// appropriate flag bits.
		/// A valid combination of flag bits is an exclusive OR operation
		/// that can combine
		/// the TYPE_TRANSLATION flag bit
		/// in addition to either of the
		/// TYPE_UNIFORM_SCALE or TYPE_GENERAL_SCALE flag bits
		/// as well as either of the
		/// TYPE_QUADRANT_ROTATION or TYPE_GENERAL_ROTATION flag bits. </summary>
		/// <returns> the OR combination of any of the indicated flags that
		/// apply to this transform </returns>
		/// <seealso cref= #TYPE_IDENTITY </seealso>
		/// <seealso cref= #TYPE_TRANSLATION </seealso>
		/// <seealso cref= #TYPE_UNIFORM_SCALE </seealso>
		/// <seealso cref= #TYPE_GENERAL_SCALE </seealso>
		/// <seealso cref= #TYPE_QUADRANT_ROTATION </seealso>
		/// <seealso cref= #TYPE_GENERAL_ROTATION </seealso>
		/// <seealso cref= #TYPE_GENERAL_TRANSFORM
		/// @since 1.2 </seealso>
		public virtual int Type
		{
			get
			{
				if (Type_Renamed == TYPE_UNKNOWN)
				{
					CalculateType();
				}
				return Type_Renamed;
			}
		}

		/// <summary>
		/// This is the utility function to calculate the flag bits when
		/// they have not been cached. </summary>
		/// <seealso cref= #getType </seealso>
//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @SuppressWarnings("fallthrough") private void calculateType()
		private void CalculateType()
		{
			int ret = TYPE_IDENTITY;
			bool sgn0, sgn1;
			double M0, M1, M2, M3;
			UpdateState();
			switch (State)
			{
			default:
				StateError();
				/* NOTREACHED */
				goto case (APPLY_SHEAR | APPLY_SCALE | APPLY_TRANSLATE);
			case (APPLY_SHEAR | APPLY_SCALE | APPLY_TRANSLATE):
				ret = TYPE_TRANSLATION;
				/* NOBREAK */
				goto case (APPLY_SHEAR | APPLY_SCALE);
			case (APPLY_SHEAR | APPLY_SCALE):
				if ((M0 = M00) * (M2 = M01) + (M3 = M10) * (M1 = M11) != 0)
				{
					// Transformed unit vectors are not perpendicular...
					this.Type_Renamed = TYPE_GENERAL_TRANSFORM;
					return;
				}
				sgn0 = (M0 >= 0.0);
				sgn1 = (M1 >= 0.0);
				if (sgn0 == sgn1)
				{
					// sgn(M0) == sgn(M1) therefore sgn(M2) == -sgn(M3)
					// This is the "unflipped" (right-handed) state
					if (M0 != M1 || M2 != -M3)
					{
						ret |= (TYPE_GENERAL_ROTATION | TYPE_GENERAL_SCALE);
					}
					else if (M0 * M1 - M2 * M3 != 1.0)
					{
						ret |= (TYPE_GENERAL_ROTATION | TYPE_UNIFORM_SCALE);
					}
					else
					{
						ret |= TYPE_GENERAL_ROTATION;
					}
				}
				else
				{
					// sgn(M0) == -sgn(M1) therefore sgn(M2) == sgn(M3)
					// This is the "flipped" (left-handed) state
					if (M0 != -M1 || M2 != M3)
					{
						ret |= (TYPE_GENERAL_ROTATION | TYPE_FLIP | TYPE_GENERAL_SCALE);
					}
					else if (M0 * M1 - M2 * M3 != 1.0)
					{
						ret |= (TYPE_GENERAL_ROTATION | TYPE_FLIP | TYPE_UNIFORM_SCALE);
					}
					else
					{
						ret |= (TYPE_GENERAL_ROTATION | TYPE_FLIP);
					}
				}
				break;
			case (APPLY_SHEAR | APPLY_TRANSLATE):
				ret = TYPE_TRANSLATION;
				/* NOBREAK */
				goto case (APPLY_SHEAR);
			case (APPLY_SHEAR):
				sgn0 = ((M0 = M01) >= 0.0);
				sgn1 = ((M1 = M10) >= 0.0);
				if (sgn0 != sgn1)
				{
					// Different signs - simple 90 degree rotation
					if (M0 != -M1)
					{
						ret |= (TYPE_QUADRANT_ROTATION | TYPE_GENERAL_SCALE);
					}
					else if (M0 != 1.0 && M0 != -1.0)
					{
						ret |= (TYPE_QUADRANT_ROTATION | TYPE_UNIFORM_SCALE);
					}
					else
					{
						ret |= TYPE_QUADRANT_ROTATION;
					}
				}
				else
				{
					// Same signs - 90 degree rotation plus an axis flip too
					if (M0 == M1)
					{
						ret |= (TYPE_QUADRANT_ROTATION | TYPE_FLIP | TYPE_UNIFORM_SCALE);
					}
					else
					{
						ret |= (TYPE_QUADRANT_ROTATION | TYPE_FLIP | TYPE_GENERAL_SCALE);
					}
				}
				break;
			case (APPLY_SCALE | APPLY_TRANSLATE):
				ret = TYPE_TRANSLATION;
				/* NOBREAK */
				goto case (APPLY_SCALE);
			case (APPLY_SCALE):
				sgn0 = ((M0 = M00) >= 0.0);
				sgn1 = ((M1 = M11) >= 0.0);
				if (sgn0 == sgn1)
				{
					if (sgn0)
					{
						// Both scaling factors non-negative - simple scale
						// Note: APPLY_SCALE implies M0, M1 are not both 1
						if (M0 == M1)
						{
							ret |= TYPE_UNIFORM_SCALE;
						}
						else
						{
							ret |= TYPE_GENERAL_SCALE;
						}
					}
					else
					{
						// Both scaling factors negative - 180 degree rotation
						if (M0 != M1)
						{
							ret |= (TYPE_QUADRANT_ROTATION | TYPE_GENERAL_SCALE);
						}
						else if (M0 != -1.0)
						{
							ret |= (TYPE_QUADRANT_ROTATION | TYPE_UNIFORM_SCALE);
						}
						else
						{
							ret |= TYPE_QUADRANT_ROTATION;
						}
					}
				}
				else
				{
					// Scaling factor signs different - flip about some axis
					if (M0 == -M1)
					{
						if (M0 == 1.0 || M0 == -1.0)
						{
							ret |= TYPE_FLIP;
						}
						else
						{
							ret |= (TYPE_FLIP | TYPE_UNIFORM_SCALE);
						}
					}
					else
					{
						ret |= (TYPE_FLIP | TYPE_GENERAL_SCALE);
					}
				}
				break;
			case (APPLY_TRANSLATE):
				ret = TYPE_TRANSLATION;
				break;
			case (APPLY_IDENTITY):
				break;
			}
			this.Type_Renamed = ret;
		}

		/// <summary>
		/// Returns the determinant of the matrix representation of the transform.
		/// The determinant is useful both to determine if the transform can
		/// be inverted and to get a single value representing the
		/// combined X and Y scaling of the transform.
		/// <para>
		/// If the determinant is non-zero, then this transform is
		/// invertible and the various methods that depend on the inverse
		/// transform do not need to throw a
		/// <seealso cref="NoninvertibleTransformException"/>.
		/// If the determinant is zero then this transform can not be
		/// inverted since the transform maps all input coordinates onto
		/// a line or a point.
		/// If the determinant is near enough to zero then inverse transform
		/// operations might not carry enough precision to produce meaningful
		/// results.
		/// </para>
		/// <para>
		/// If this transform represents a uniform scale, as indicated by
		/// the <code>getType</code> method then the determinant also
		/// represents the square of the uniform scale factor by which all of
		/// the points are expanded from or contracted towards the origin.
		/// If this transform represents a non-uniform scale or more general
		/// transform then the determinant is not likely to represent a
		/// value useful for any purpose other than determining if inverse
		/// transforms are possible.
		/// </para>
		/// <para>
		/// Mathematically, the determinant is calculated using the formula:
		/// <pre>
		///          |  m00  m01  m02  |
		///          |  m10  m11  m12  |  =  m00 * m11 - m01 * m10
		///          |   0    0    1   |
		/// </pre>
		/// 
		/// </para>
		/// </summary>
		/// <returns> the determinant of the matrix used to transform the
		/// coordinates. </returns>
		/// <seealso cref= #getType </seealso>
		/// <seealso cref= #createInverse </seealso>
		/// <seealso cref= #inverseTransform </seealso>
		/// <seealso cref= #TYPE_UNIFORM_SCALE
		/// @since 1.2 </seealso>
//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @SuppressWarnings("fallthrough") public double getDeterminant()
		public virtual double Determinant
		{
			get
			{
				switch (State)
				{
				default:
					StateError();
					/* NOTREACHED */
					goto case (APPLY_SHEAR | APPLY_SCALE | APPLY_TRANSLATE);
				case (APPLY_SHEAR | APPLY_SCALE | APPLY_TRANSLATE):
				case (APPLY_SHEAR | APPLY_SCALE):
					return M00 * M11 - M01 * M10;
				case (APPLY_SHEAR | APPLY_TRANSLATE):
				case (APPLY_SHEAR):
					return -(M01 * M10);
				case (APPLY_SCALE | APPLY_TRANSLATE):
				case (APPLY_SCALE):
					return M00 * M11;
				case (APPLY_TRANSLATE):
				case (APPLY_IDENTITY):
					return 1.0;
				}
			}
		}

		/// <summary>
		/// Manually recalculates the state of the transform when the matrix
		/// changes too much to predict the effects on the state.
		/// The following table specifies what the various settings of the
		/// state field say about the values of the corresponding matrix
		/// element fields.
		/// Note that the rules governing the SCALE fields are slightly
		/// different depending on whether the SHEAR flag is also set.
		/// <pre>
		///                     SCALE            SHEAR          TRANSLATE
		///                    m00/m11          m01/m10          m02/m12
		/// 
		/// IDENTITY             1.0              0.0              0.0
		/// TRANSLATE (TR)       1.0              0.0          not both 0.0
		/// SCALE (SC)       not both 1.0         0.0              0.0
		/// TR | SC          not both 1.0         0.0          not both 0.0
		/// SHEAR (SH)           0.0          not both 0.0         0.0
		/// TR | SH              0.0          not both 0.0     not both 0.0
		/// SC | SH          not both 0.0     not both 0.0         0.0
		/// TR | SC | SH     not both 0.0     not both 0.0     not both 0.0
		/// </pre>
		/// </summary>
		internal virtual void UpdateState()
		{
			if (M01 == 0.0 && M10 == 0.0)
			{
				if (M00 == 1.0 && M11 == 1.0)
				{
					if (M02 == 0.0 && M12 == 0.0)
					{
						State = APPLY_IDENTITY;
						Type_Renamed = TYPE_IDENTITY;
					}
					else
					{
						State = APPLY_TRANSLATE;
						Type_Renamed = TYPE_TRANSLATION;
					}
				}
				else
				{
					if (M02 == 0.0 && M12 == 0.0)
					{
						State = APPLY_SCALE;
						Type_Renamed = TYPE_UNKNOWN;
					}
					else
					{
						State = (APPLY_SCALE | APPLY_TRANSLATE);
						Type_Renamed = TYPE_UNKNOWN;
					}
				}
			}
			else
			{
				if (M00 == 0.0 && M11 == 0.0)
				{
					if (M02 == 0.0 && M12 == 0.0)
					{
						State = APPLY_SHEAR;
						Type_Renamed = TYPE_UNKNOWN;
					}
					else
					{
						State = (APPLY_SHEAR | APPLY_TRANSLATE);
						Type_Renamed = TYPE_UNKNOWN;
					}
				}
				else
				{
					if (M02 == 0.0 && M12 == 0.0)
					{
						State = (APPLY_SHEAR | APPLY_SCALE);
						Type_Renamed = TYPE_UNKNOWN;
					}
					else
					{
						State = (APPLY_SHEAR | APPLY_SCALE | APPLY_TRANSLATE);
						Type_Renamed = TYPE_UNKNOWN;
					}
				}
			}
		}

		/*
		 * Convenience method used internally to throw exceptions when
		 * a case was forgotten in a switch statement.
		 */
		private void StateError()
		{
			throw new InternalError("missing case in transform state switch");
		}

		/// <summary>
		/// Retrieves the 6 specifiable values in the 3x3 affine transformation
		/// matrix and places them into an array of double precisions values.
		/// The values are stored in the array as
		/// {&nbsp;m00&nbsp;m10&nbsp;m01&nbsp;m11&nbsp;m02&nbsp;m12&nbsp;}.
		/// An array of 4 doubles can also be specified, in which case only the
		/// first four elements representing the non-transform
		/// parts of the array are retrieved and the values are stored into
		/// the array as {&nbsp;m00&nbsp;m10&nbsp;m01&nbsp;m11&nbsp;} </summary>
		/// <param name="flatmatrix"> the double array used to store the returned
		/// values. </param>
		/// <seealso cref= #getScaleX </seealso>
		/// <seealso cref= #getScaleY </seealso>
		/// <seealso cref= #getShearX </seealso>
		/// <seealso cref= #getShearY </seealso>
		/// <seealso cref= #getTranslateX </seealso>
		/// <seealso cref= #getTranslateY
		/// @since 1.2 </seealso>
		public virtual void GetMatrix(double[] flatmatrix)
		{
			flatmatrix[0] = M00;
			flatmatrix[1] = M10;
			flatmatrix[2] = M01;
			flatmatrix[3] = M11;
			if (flatmatrix.Length > 5)
			{
				flatmatrix[4] = M02;
				flatmatrix[5] = M12;
			}
		}

		/// <summary>
		/// Returns the X coordinate scaling element (m00) of the 3x3
		/// affine transformation matrix. </summary>
		/// <returns> a double value that is the X coordinate of the scaling
		///  element of the affine transformation matrix. </returns>
		/// <seealso cref= #getMatrix
		/// @since 1.2 </seealso>
		public virtual double ScaleX
		{
			get
			{
				return M00;
			}
		}

		/// <summary>
		/// Returns the Y coordinate scaling element (m11) of the 3x3
		/// affine transformation matrix. </summary>
		/// <returns> a double value that is the Y coordinate of the scaling
		///  element of the affine transformation matrix. </returns>
		/// <seealso cref= #getMatrix
		/// @since 1.2 </seealso>
		public virtual double ScaleY
		{
			get
			{
				return M11;
			}
		}

		/// <summary>
		/// Returns the X coordinate shearing element (m01) of the 3x3
		/// affine transformation matrix. </summary>
		/// <returns> a double value that is the X coordinate of the shearing
		///  element of the affine transformation matrix. </returns>
		/// <seealso cref= #getMatrix
		/// @since 1.2 </seealso>
		public virtual double ShearX
		{
			get
			{
				return M01;
			}
		}

		/// <summary>
		/// Returns the Y coordinate shearing element (m10) of the 3x3
		/// affine transformation matrix. </summary>
		/// <returns> a double value that is the Y coordinate of the shearing
		///  element of the affine transformation matrix. </returns>
		/// <seealso cref= #getMatrix
		/// @since 1.2 </seealso>
		public virtual double ShearY
		{
			get
			{
				return M10;
			}
		}

		/// <summary>
		/// Returns the X coordinate of the translation element (m02) of the
		/// 3x3 affine transformation matrix. </summary>
		/// <returns> a double value that is the X coordinate of the translation
		///  element of the affine transformation matrix. </returns>
		/// <seealso cref= #getMatrix
		/// @since 1.2 </seealso>
		public virtual double TranslateX
		{
			get
			{
				return M02;
			}
		}

		/// <summary>
		/// Returns the Y coordinate of the translation element (m12) of the
		/// 3x3 affine transformation matrix. </summary>
		/// <returns> a double value that is the Y coordinate of the translation
		///  element of the affine transformation matrix. </returns>
		/// <seealso cref= #getMatrix
		/// @since 1.2 </seealso>
		public virtual double TranslateY
		{
			get
			{
				return M12;
			}
		}

		/// <summary>
		/// Concatenates this transform with a translation transformation.
		/// This is equivalent to calling concatenate(T), where T is an
		/// <code>AffineTransform</code> represented by the following matrix:
		/// <pre>
		///          [   1    0    tx  ]
		///          [   0    1    ty  ]
		///          [   0    0    1   ]
		/// </pre> </summary>
		/// <param name="tx"> the distance by which coordinates are translated in the
		/// X axis direction </param>
		/// <param name="ty"> the distance by which coordinates are translated in the
		/// Y axis direction
		/// @since 1.2 </param>
		public virtual void Translate(double tx, double ty)
		{
			switch (State)
			{
			default:
				StateError();
				/* NOTREACHED */
				return;
			case (APPLY_SHEAR | APPLY_SCALE | APPLY_TRANSLATE):
				M02 = tx * M00 + ty * M01 + M02;
				M12 = tx * M10 + ty * M11 + M12;
				if (M02 == 0.0 && M12 == 0.0)
				{
					State = APPLY_SHEAR | APPLY_SCALE;
					if (Type_Renamed != TYPE_UNKNOWN)
					{
						Type_Renamed -= TYPE_TRANSLATION;
					}
				}
				return;
			case (APPLY_SHEAR | APPLY_SCALE):
				M02 = tx * M00 + ty * M01;
				M12 = tx * M10 + ty * M11;
				if (M02 != 0.0 || M12 != 0.0)
				{
					State = APPLY_SHEAR | APPLY_SCALE | APPLY_TRANSLATE;
					Type_Renamed |= TYPE_TRANSLATION;
				}
				return;
			case (APPLY_SHEAR | APPLY_TRANSLATE):
				M02 = ty * M01 + M02;
				M12 = tx * M10 + M12;
				if (M02 == 0.0 && M12 == 0.0)
				{
					State = APPLY_SHEAR;
					if (Type_Renamed != TYPE_UNKNOWN)
					{
						Type_Renamed -= TYPE_TRANSLATION;
					}
				}
				return;
			case (APPLY_SHEAR):
				M02 = ty * M01;
				M12 = tx * M10;
				if (M02 != 0.0 || M12 != 0.0)
				{
					State = APPLY_SHEAR | APPLY_TRANSLATE;
					Type_Renamed |= TYPE_TRANSLATION;
				}
				return;
			case (APPLY_SCALE | APPLY_TRANSLATE):
				M02 = tx * M00 + M02;
				M12 = ty * M11 + M12;
				if (M02 == 0.0 && M12 == 0.0)
				{
					State = APPLY_SCALE;
					if (Type_Renamed != TYPE_UNKNOWN)
					{
						Type_Renamed -= TYPE_TRANSLATION;
					}
				}
				return;
			case (APPLY_SCALE):
				M02 = tx * M00;
				M12 = ty * M11;
				if (M02 != 0.0 || M12 != 0.0)
				{
					State = APPLY_SCALE | APPLY_TRANSLATE;
					Type_Renamed |= TYPE_TRANSLATION;
				}
				return;
			case (APPLY_TRANSLATE):
				M02 = tx + M02;
				M12 = ty + M12;
				if (M02 == 0.0 && M12 == 0.0)
				{
					State = APPLY_IDENTITY;
					Type_Renamed = TYPE_IDENTITY;
				}
				return;
			case (APPLY_IDENTITY):
				M02 = tx;
				M12 = ty;
				if (tx != 0.0 || ty != 0.0)
				{
					State = APPLY_TRANSLATE;
					Type_Renamed = TYPE_TRANSLATION;
				}
				return;
			}
		}

		// Utility methods to optimize rotate methods.
		// These tables translate the flags during predictable quadrant
		// rotations where the shear and scale values are swapped and negated.
		private static readonly int[] Rot90conversion = new int[] {APPLY_SHEAR, APPLY_SHEAR | APPLY_TRANSLATE, APPLY_SHEAR, APPLY_SHEAR | APPLY_TRANSLATE, APPLY_SCALE, APPLY_SCALE | APPLY_TRANSLATE, APPLY_SHEAR | APPLY_SCALE, APPLY_SHEAR | APPLY_SCALE | APPLY_TRANSLATE};
		private void Rotate90()
		{
			double M0 = M00;
			M00 = M01;
			M01 = -M0;
			M0 = M10;
			M10 = M11;
			M11 = -M0;
			int state = Rot90conversion[this.State];
			if ((state & (APPLY_SHEAR | APPLY_SCALE)) == APPLY_SCALE && M00 == 1.0 && M11 == 1.0)
			{
				state -= APPLY_SCALE;
			}
			this.State = state;
			Type_Renamed = TYPE_UNKNOWN;
		}
		private void Rotate180()
		{
			M00 = -M00;
			M11 = -M11;
			int state = this.State;
			if ((state & (APPLY_SHEAR)) != 0)
			{
				// If there was a shear, then this rotation has no
				// effect on the state.
				M01 = -M01;
				M10 = -M10;
			}
			else
			{
				// No shear means the SCALE state may toggle when
				// m00 and m11 are negated.
				if (M00 == 1.0 && M11 == 1.0)
				{
					this.State = state & ~APPLY_SCALE;
				}
				else
				{
					this.State = state | APPLY_SCALE;
				}
			}
			Type_Renamed = TYPE_UNKNOWN;
		}
		private void Rotate270()
		{
			double M0 = M00;
			M00 = -M01;
			M01 = M0;
			M0 = M10;
			M10 = -M11;
			M11 = M0;
			int state = Rot90conversion[this.State];
			if ((state & (APPLY_SHEAR | APPLY_SCALE)) == APPLY_SCALE && M00 == 1.0 && M11 == 1.0)
			{
				state -= APPLY_SCALE;
			}
			this.State = state;
			Type_Renamed = TYPE_UNKNOWN;
		}

		/// <summary>
		/// Concatenates this transform with a rotation transformation.
		/// This is equivalent to calling concatenate(R), where R is an
		/// <code>AffineTransform</code> represented by the following matrix:
		/// <pre>
		///          [   cos(theta)    -sin(theta)    0   ]
		///          [   sin(theta)     cos(theta)    0   ]
		///          [       0              0         1   ]
		/// </pre>
		/// Rotating by a positive angle theta rotates points on the positive
		/// X axis toward the positive Y axis.
		/// Note also the discussion of
		/// <a href="#quadrantapproximation">Handling 90-Degree Rotations</a>
		/// above. </summary>
		/// <param name="theta"> the angle of rotation measured in radians
		/// @since 1.2 </param>
		public virtual void Rotate(double theta)
		{
			double sin = System.Math.Sin(theta);
			if (sin == 1.0)
			{
				Rotate90();
			}
			else if (sin == -1.0)
			{
				Rotate270();
			}
			else
			{
				double cos = System.Math.Cos(theta);
				if (cos == -1.0)
				{
					Rotate180();
				}
				else if (cos != 1.0)
				{
					double M0, M1;
					M0 = M00;
					M1 = M01;
					M00 = cos * M0 + sin * M1;
					M01 = -sin * M0 + cos * M1;
					M0 = M10;
					M1 = M11;
					M10 = cos * M0 + sin * M1;
					M11 = -sin * M0 + cos * M1;
					UpdateState();
				}
			}
		}

		/// <summary>
		/// Concatenates this transform with a transform that rotates
		/// coordinates around an anchor point.
		/// This operation is equivalent to translating the coordinates so
		/// that the anchor point is at the origin (S1), then rotating them
		/// about the new origin (S2), and finally translating so that the
		/// intermediate origin is restored to the coordinates of the original
		/// anchor point (S3).
		/// <para>
		/// This operation is equivalent to the following sequence of calls:
		/// <pre>
		///     translate(anchorx, anchory);      // S3: final translation
		///     rotate(theta);                    // S2: rotate around anchor
		///     translate(-anchorx, -anchory);    // S1: translate anchor to origin
		/// </pre>
		/// Rotating by a positive angle theta rotates points on the positive
		/// X axis toward the positive Y axis.
		/// Note also the discussion of
		/// <a href="#quadrantapproximation">Handling 90-Degree Rotations</a>
		/// above.
		/// 
		/// </para>
		/// </summary>
		/// <param name="theta"> the angle of rotation measured in radians </param>
		/// <param name="anchorx"> the X coordinate of the rotation anchor point </param>
		/// <param name="anchory"> the Y coordinate of the rotation anchor point
		/// @since 1.2 </param>
		public virtual void Rotate(double theta, double anchorx, double anchory)
		{
			// REMIND: Simple for now - optimize later
			Translate(anchorx, anchory);
			Rotate(theta);
			Translate(-anchorx, -anchory);
		}

		/// <summary>
		/// Concatenates this transform with a transform that rotates
		/// coordinates according to a rotation vector.
		/// All coordinates rotate about the origin by the same amount.
		/// The amount of rotation is such that coordinates along the former
		/// positive X axis will subsequently align with the vector pointing
		/// from the origin to the specified vector coordinates.
		/// If both <code>vecx</code> and <code>vecy</code> are 0.0,
		/// no additional rotation is added to this transform.
		/// This operation is equivalent to calling:
		/// <pre>
		///          rotate(Math.atan2(vecy, vecx));
		/// </pre>
		/// </summary>
		/// <param name="vecx"> the X coordinate of the rotation vector </param>
		/// <param name="vecy"> the Y coordinate of the rotation vector
		/// @since 1.6 </param>
		public virtual void Rotate(double vecx, double vecy)
		{
			if (vecy == 0.0)
			{
				if (vecx < 0.0)
				{
					Rotate180();
				}
				// If vecx > 0.0 - no rotation
				// If vecx == 0.0 - undefined rotation - treat as no rotation
			}
			else if (vecx == 0.0)
			{
				if (vecy > 0.0)
				{
					Rotate90();
				} // vecy must be < 0.0
				else
				{
					Rotate270();
				}
			}
			else
			{
				double len = System.Math.Sqrt(vecx * vecx + vecy * vecy);
				double sin = vecy / len;
				double cos = vecx / len;
				double M0, M1;
				M0 = M00;
				M1 = M01;
				M00 = cos * M0 + sin * M1;
				M01 = -sin * M0 + cos * M1;
				M0 = M10;
				M1 = M11;
				M10 = cos * M0 + sin * M1;
				M11 = -sin * M0 + cos * M1;
				UpdateState();
			}
		}

		/// <summary>
		/// Concatenates this transform with a transform that rotates
		/// coordinates around an anchor point according to a rotation
		/// vector.
		/// All coordinates rotate about the specified anchor coordinates
		/// by the same amount.
		/// The amount of rotation is such that coordinates along the former
		/// positive X axis will subsequently align with the vector pointing
		/// from the origin to the specified vector coordinates.
		/// If both <code>vecx</code> and <code>vecy</code> are 0.0,
		/// the transform is not modified in any way.
		/// This method is equivalent to calling:
		/// <pre>
		///     rotate(Math.atan2(vecy, vecx), anchorx, anchory);
		/// </pre>
		/// </summary>
		/// <param name="vecx"> the X coordinate of the rotation vector </param>
		/// <param name="vecy"> the Y coordinate of the rotation vector </param>
		/// <param name="anchorx"> the X coordinate of the rotation anchor point </param>
		/// <param name="anchory"> the Y coordinate of the rotation anchor point
		/// @since 1.6 </param>
		public virtual void Rotate(double vecx, double vecy, double anchorx, double anchory)
		{
			// REMIND: Simple for now - optimize later
			Translate(anchorx, anchory);
			Rotate(vecx, vecy);
			Translate(-anchorx, -anchory);
		}

		/// <summary>
		/// Concatenates this transform with a transform that rotates
		/// coordinates by the specified number of quadrants.
		/// This is equivalent to calling:
		/// <pre>
		///     rotate(numquadrants * Math.PI / 2.0);
		/// </pre>
		/// Rotating by a positive number of quadrants rotates points on
		/// the positive X axis toward the positive Y axis. </summary>
		/// <param name="numquadrants"> the number of 90 degree arcs to rotate by
		/// @since 1.6 </param>
		public virtual void QuadrantRotate(int numquadrants)
		{
			switch (numquadrants & 3)
			{
			case 0:
				break;
			case 1:
				Rotate90();
				break;
			case 2:
				Rotate180();
				break;
			case 3:
				Rotate270();
				break;
			}
		}

		/// <summary>
		/// Concatenates this transform with a transform that rotates
		/// coordinates by the specified number of quadrants around
		/// the specified anchor point.
		/// This method is equivalent to calling:
		/// <pre>
		///     rotate(numquadrants * Math.PI / 2.0, anchorx, anchory);
		/// </pre>
		/// Rotating by a positive number of quadrants rotates points on
		/// the positive X axis toward the positive Y axis.
		/// </summary>
		/// <param name="numquadrants"> the number of 90 degree arcs to rotate by </param>
		/// <param name="anchorx"> the X coordinate of the rotation anchor point </param>
		/// <param name="anchory"> the Y coordinate of the rotation anchor point
		/// @since 1.6 </param>
		public virtual void QuadrantRotate(int numquadrants, double anchorx, double anchory)
		{
			switch (numquadrants & 3)
			{
			case 0:
				return;
			case 1:
				M02 += anchorx * (M00 - M01) + anchory * (M01 + M00);
				M12 += anchorx * (M10 - M11) + anchory * (M11 + M10);
				Rotate90();
				break;
			case 2:
				M02 += anchorx * (M00 + M00) + anchory * (M01 + M01);
				M12 += anchorx * (M10 + M10) + anchory * (M11 + M11);
				Rotate180();
				break;
			case 3:
				M02 += anchorx * (M00 + M01) + anchory * (M01 - M00);
				M12 += anchorx * (M10 + M11) + anchory * (M11 - M10);
				Rotate270();
				break;
			}
			if (M02 == 0.0 && M12 == 0.0)
			{
				State &= ~APPLY_TRANSLATE;
			}
			else
			{
				State |= APPLY_TRANSLATE;
			}
		}

		/// <summary>
		/// Concatenates this transform with a scaling transformation.
		/// This is equivalent to calling concatenate(S), where S is an
		/// <code>AffineTransform</code> represented by the following matrix:
		/// <pre>
		///          [   sx   0    0   ]
		///          [   0    sy   0   ]
		///          [   0    0    1   ]
		/// </pre> </summary>
		/// <param name="sx"> the factor by which coordinates are scaled along the
		/// X axis direction </param>
		/// <param name="sy"> the factor by which coordinates are scaled along the
		/// Y axis direction
		/// @since 1.2 </param>
//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @SuppressWarnings("fallthrough") public void scale(double sx, double sy)
		public virtual void Scale(double sx, double sy)
		{
			int state = this.State;
			switch (state)
			{
			default:
				StateError();
				/* NOTREACHED */
				goto case (APPLY_SHEAR | APPLY_SCALE | APPLY_TRANSLATE);
			case (APPLY_SHEAR | APPLY_SCALE | APPLY_TRANSLATE):
			case (APPLY_SHEAR | APPLY_SCALE):
				M00 *= sx;
				M11 *= sy;
				/* NOBREAK */
				goto case (APPLY_SHEAR | APPLY_TRANSLATE);
			case (APPLY_SHEAR | APPLY_TRANSLATE):
			case (APPLY_SHEAR):
				M01 *= sy;
				M10 *= sx;
				if (M01 == 0 && M10 == 0)
				{
					state &= APPLY_TRANSLATE;
					if (M00 == 1.0 && M11 == 1.0)
					{
						this.Type_Renamed = (state == APPLY_IDENTITY ? TYPE_IDENTITY : TYPE_TRANSLATION);
					}
					else
					{
						state |= APPLY_SCALE;
						this.Type_Renamed = TYPE_UNKNOWN;
					}
					this.State = state;
				}
				return;
			case (APPLY_SCALE | APPLY_TRANSLATE):
			case (APPLY_SCALE):
				M00 *= sx;
				M11 *= sy;
				if (M00 == 1.0 && M11 == 1.0)
				{
					this.State = (state &= APPLY_TRANSLATE);
					this.Type_Renamed = (state == APPLY_IDENTITY ? TYPE_IDENTITY : TYPE_TRANSLATION);
				}
				else
				{
					this.Type_Renamed = TYPE_UNKNOWN;
				}
				return;
			case (APPLY_TRANSLATE):
			case (APPLY_IDENTITY):
				M00 = sx;
				M11 = sy;
				if (sx != 1.0 || sy != 1.0)
				{
					this.State = state | APPLY_SCALE;
					this.Type_Renamed = TYPE_UNKNOWN;
				}
				return;
			}
		}

		/// <summary>
		/// Concatenates this transform with a shearing transformation.
		/// This is equivalent to calling concatenate(SH), where SH is an
		/// <code>AffineTransform</code> represented by the following matrix:
		/// <pre>
		///          [   1   shx   0   ]
		///          [  shy   1    0   ]
		///          [   0    0    1   ]
		/// </pre> </summary>
		/// <param name="shx"> the multiplier by which coordinates are shifted in the
		/// direction of the positive X axis as a factor of their Y coordinate </param>
		/// <param name="shy"> the multiplier by which coordinates are shifted in the
		/// direction of the positive Y axis as a factor of their X coordinate
		/// @since 1.2 </param>
		public virtual void Shear(double shx, double shy)
		{
			int state = this.State;
			switch (state)
			{
			default:
				StateError();
				/* NOTREACHED */
				return;
			case (APPLY_SHEAR | APPLY_SCALE | APPLY_TRANSLATE):
			case (APPLY_SHEAR | APPLY_SCALE):
				double M0, M1;
				M0 = M00;
				M1 = M01;
				M00 = M0 + M1 * shy;
				M01 = M0 * shx + M1;

				M0 = M10;
				M1 = M11;
				M10 = M0 + M1 * shy;
				M11 = M0 * shx + M1;
				UpdateState();
				return;
			case (APPLY_SHEAR | APPLY_TRANSLATE):
			case (APPLY_SHEAR):
				M00 = M01 * shy;
				M11 = M10 * shx;
				if (M00 != 0.0 || M11 != 0.0)
				{
					this.State = state | APPLY_SCALE;
				}
				this.Type_Renamed = TYPE_UNKNOWN;
				return;
			case (APPLY_SCALE | APPLY_TRANSLATE):
			case (APPLY_SCALE):
				M01 = M00 * shx;
				M10 = M11 * shy;
				if (M01 != 0.0 || M10 != 0.0)
				{
					this.State = state | APPLY_SHEAR;
				}
				this.Type_Renamed = TYPE_UNKNOWN;
				return;
			case (APPLY_TRANSLATE):
			case (APPLY_IDENTITY):
				M01 = shx;
				M10 = shy;
				if (M01 != 0.0 || M10 != 0.0)
				{
					this.State = state | APPLY_SCALE | APPLY_SHEAR;
					this.Type_Renamed = TYPE_UNKNOWN;
				}
				return;
			}
		}

		/// <summary>
		/// Resets this transform to the Identity transform.
		/// @since 1.2
		/// </summary>
		public virtual void SetToIdentity()
		{
			M00 = M11 = 1.0;
			M10 = M01 = M02 = M12 = 0.0;
			State = APPLY_IDENTITY;
			Type_Renamed = TYPE_IDENTITY;
		}

		/// <summary>
		/// Sets this transform to a translation transformation.
		/// The matrix representing this transform becomes:
		/// <pre>
		///          [   1    0    tx  ]
		///          [   0    1    ty  ]
		///          [   0    0    1   ]
		/// </pre> </summary>
		/// <param name="tx"> the distance by which coordinates are translated in the
		/// X axis direction </param>
		/// <param name="ty"> the distance by which coordinates are translated in the
		/// Y axis direction
		/// @since 1.2 </param>
		public virtual void SetToTranslation(double tx, double ty)
		{
			M00 = 1.0;
			M10 = 0.0;
			M01 = 0.0;
			M11 = 1.0;
			M02 = tx;
			M12 = ty;
			if (tx != 0.0 || ty != 0.0)
			{
				State = APPLY_TRANSLATE;
				Type_Renamed = TYPE_TRANSLATION;
			}
			else
			{
				State = APPLY_IDENTITY;
				Type_Renamed = TYPE_IDENTITY;
			}
		}

		/// <summary>
		/// Sets this transform to a rotation transformation.
		/// The matrix representing this transform becomes:
		/// <pre>
		///          [   cos(theta)    -sin(theta)    0   ]
		///          [   sin(theta)     cos(theta)    0   ]
		///          [       0              0         1   ]
		/// </pre>
		/// Rotating by a positive angle theta rotates points on the positive
		/// X axis toward the positive Y axis.
		/// Note also the discussion of
		/// <a href="#quadrantapproximation">Handling 90-Degree Rotations</a>
		/// above. </summary>
		/// <param name="theta"> the angle of rotation measured in radians
		/// @since 1.2 </param>
		public virtual double ToRotation
		{
			set
			{
				double sin = System.Math.Sin(value);
				double cos;
				if (sin == 1.0 || sin == -1.0)
				{
					cos = 0.0;
					State = APPLY_SHEAR;
					Type_Renamed = TYPE_QUADRANT_ROTATION;
				}
				else
				{
					cos = System.Math.Cos(value);
					if (cos == -1.0)
					{
						sin = 0.0;
						State = APPLY_SCALE;
						Type_Renamed = TYPE_QUADRANT_ROTATION;
					}
					else if (cos == 1.0)
					{
						sin = 0.0;
						State = APPLY_IDENTITY;
						Type_Renamed = TYPE_IDENTITY;
					}
					else
					{
						State = APPLY_SHEAR | APPLY_SCALE;
						Type_Renamed = TYPE_GENERAL_ROTATION;
					}
				}
				M00 = cos;
				M10 = sin;
				M01 = -sin;
				M11 = cos;
				M02 = 0.0;
				M12 = 0.0;
			}
		}

		/// <summary>
		/// Sets this transform to a translated rotation transformation.
		/// This operation is equivalent to translating the coordinates so
		/// that the anchor point is at the origin (S1), then rotating them
		/// about the new origin (S2), and finally translating so that the
		/// intermediate origin is restored to the coordinates of the original
		/// anchor point (S3).
		/// <para>
		/// This operation is equivalent to the following sequence of calls:
		/// <pre>
		///     setToTranslation(anchorx, anchory); // S3: final translation
		///     rotate(theta);                      // S2: rotate around anchor
		///     translate(-anchorx, -anchory);      // S1: translate anchor to origin
		/// </pre>
		/// The matrix representing this transform becomes:
		/// <pre>
		///          [   cos(theta)    -sin(theta)    x-x*cos+y*sin  ]
		///          [   sin(theta)     cos(theta)    y-x*sin-y*cos  ]
		///          [       0              0               1        ]
		/// </pre>
		/// Rotating by a positive angle theta rotates points on the positive
		/// X axis toward the positive Y axis.
		/// Note also the discussion of
		/// <a href="#quadrantapproximation">Handling 90-Degree Rotations</a>
		/// above.
		/// 
		/// </para>
		/// </summary>
		/// <param name="theta"> the angle of rotation measured in radians </param>
		/// <param name="anchorx"> the X coordinate of the rotation anchor point </param>
		/// <param name="anchory"> the Y coordinate of the rotation anchor point
		/// @since 1.2 </param>
		public virtual void SetToRotation(double theta, double anchorx, double anchory)
		{
			ToRotation = theta;
			double sin = M10;
			double oneMinusCos = 1.0 - M00;
			M02 = anchorx * oneMinusCos + anchory * sin;
			M12 = anchory * oneMinusCos - anchorx * sin;
			if (M02 != 0.0 || M12 != 0.0)
			{
				State |= APPLY_TRANSLATE;
				Type_Renamed |= TYPE_TRANSLATION;
			}
		}

		/// <summary>
		/// Sets this transform to a rotation transformation that rotates
		/// coordinates according to a rotation vector.
		/// All coordinates rotate about the origin by the same amount.
		/// The amount of rotation is such that coordinates along the former
		/// positive X axis will subsequently align with the vector pointing
		/// from the origin to the specified vector coordinates.
		/// If both <code>vecx</code> and <code>vecy</code> are 0.0,
		/// the transform is set to an identity transform.
		/// This operation is equivalent to calling:
		/// <pre>
		///     setToRotation(Math.atan2(vecy, vecx));
		/// </pre>
		/// </summary>
		/// <param name="vecx"> the X coordinate of the rotation vector </param>
		/// <param name="vecy"> the Y coordinate of the rotation vector
		/// @since 1.6 </param>
		public virtual void SetToRotation(double vecx, double vecy)
		{
			double sin, cos;
			if (vecy == 0)
			{
				sin = 0.0;
				if (vecx < 0.0)
				{
					cos = -1.0;
					State = APPLY_SCALE;
					Type_Renamed = TYPE_QUADRANT_ROTATION;
				}
				else
				{
					cos = 1.0;
					State = APPLY_IDENTITY;
					Type_Renamed = TYPE_IDENTITY;
				}
			}
			else if (vecx == 0)
			{
				cos = 0.0;
				sin = (vecy > 0.0) ? 1.0 : -1.0;
				State = APPLY_SHEAR;
				Type_Renamed = TYPE_QUADRANT_ROTATION;
			}
			else
			{
				double len = System.Math.Sqrt(vecx * vecx + vecy * vecy);
				cos = vecx / len;
				sin = vecy / len;
				State = APPLY_SHEAR | APPLY_SCALE;
				Type_Renamed = TYPE_GENERAL_ROTATION;
			}
			M00 = cos;
			M10 = sin;
			M01 = -sin;
			M11 = cos;
			M02 = 0.0;
			M12 = 0.0;
		}

		/// <summary>
		/// Sets this transform to a rotation transformation that rotates
		/// coordinates around an anchor point according to a rotation
		/// vector.
		/// All coordinates rotate about the specified anchor coordinates
		/// by the same amount.
		/// The amount of rotation is such that coordinates along the former
		/// positive X axis will subsequently align with the vector pointing
		/// from the origin to the specified vector coordinates.
		/// If both <code>vecx</code> and <code>vecy</code> are 0.0,
		/// the transform is set to an identity transform.
		/// This operation is equivalent to calling:
		/// <pre>
		///     setToTranslation(Math.atan2(vecy, vecx), anchorx, anchory);
		/// </pre>
		/// </summary>
		/// <param name="vecx"> the X coordinate of the rotation vector </param>
		/// <param name="vecy"> the Y coordinate of the rotation vector </param>
		/// <param name="anchorx"> the X coordinate of the rotation anchor point </param>
		/// <param name="anchory"> the Y coordinate of the rotation anchor point
		/// @since 1.6 </param>
		public virtual void SetToRotation(double vecx, double vecy, double anchorx, double anchory)
		{
			SetToRotation(vecx, vecy);
			double sin = M10;
			double oneMinusCos = 1.0 - M00;
			M02 = anchorx * oneMinusCos + anchory * sin;
			M12 = anchory * oneMinusCos - anchorx * sin;
			if (M02 != 0.0 || M12 != 0.0)
			{
				State |= APPLY_TRANSLATE;
				Type_Renamed |= TYPE_TRANSLATION;
			}
		}

		/// <summary>
		/// Sets this transform to a rotation transformation that rotates
		/// coordinates by the specified number of quadrants.
		/// This operation is equivalent to calling:
		/// <pre>
		///     setToRotation(numquadrants * Math.PI / 2.0);
		/// </pre>
		/// Rotating by a positive number of quadrants rotates points on
		/// the positive X axis toward the positive Y axis. </summary>
		/// <param name="numquadrants"> the number of 90 degree arcs to rotate by
		/// @since 1.6 </param>
		public virtual int ToQuadrantRotation
		{
			set
			{
				switch (value & 3)
				{
				case 0:
					M00 = 1.0;
					M10 = 0.0;
					M01 = 0.0;
					M11 = 1.0;
					M02 = 0.0;
					M12 = 0.0;
					State = APPLY_IDENTITY;
					Type_Renamed = TYPE_IDENTITY;
					break;
				case 1:
					M00 = 0.0;
					M10 = 1.0;
					M01 = -1.0;
					M11 = 0.0;
					M02 = 0.0;
					M12 = 0.0;
					State = APPLY_SHEAR;
					Type_Renamed = TYPE_QUADRANT_ROTATION;
					break;
				case 2:
					M00 = -1.0;
					M10 = 0.0;
					M01 = 0.0;
					M11 = -1.0;
					M02 = 0.0;
					M12 = 0.0;
					State = APPLY_SCALE;
					Type_Renamed = TYPE_QUADRANT_ROTATION;
					break;
				case 3:
					M00 = 0.0;
					M10 = -1.0;
					M01 = 1.0;
					M11 = 0.0;
					M02 = 0.0;
					M12 = 0.0;
					State = APPLY_SHEAR;
					Type_Renamed = TYPE_QUADRANT_ROTATION;
					break;
				}
			}
		}

		/// <summary>
		/// Sets this transform to a translated rotation transformation
		/// that rotates coordinates by the specified number of quadrants
		/// around the specified anchor point.
		/// This operation is equivalent to calling:
		/// <pre>
		///     setToRotation(numquadrants * Math.PI / 2.0, anchorx, anchory);
		/// </pre>
		/// Rotating by a positive number of quadrants rotates points on
		/// the positive X axis toward the positive Y axis.
		/// </summary>
		/// <param name="numquadrants"> the number of 90 degree arcs to rotate by </param>
		/// <param name="anchorx"> the X coordinate of the rotation anchor point </param>
		/// <param name="anchory"> the Y coordinate of the rotation anchor point
		/// @since 1.6 </param>
		public virtual void SetToQuadrantRotation(int numquadrants, double anchorx, double anchory)
		{
			switch (numquadrants & 3)
			{
			case 0:
				M00 = 1.0;
				M10 = 0.0;
				M01 = 0.0;
				M11 = 1.0;
				M02 = 0.0;
				M12 = 0.0;
				State = APPLY_IDENTITY;
				Type_Renamed = TYPE_IDENTITY;
				break;
			case 1:
				M00 = 0.0;
				M10 = 1.0;
				M01 = -1.0;
				M11 = 0.0;
				M02 = anchorx + anchory;
				M12 = anchory - anchorx;
				if (M02 == 0.0 && M12 == 0.0)
				{
					State = APPLY_SHEAR;
					Type_Renamed = TYPE_QUADRANT_ROTATION;
				}
				else
				{
					State = APPLY_SHEAR | APPLY_TRANSLATE;
					Type_Renamed = TYPE_QUADRANT_ROTATION | TYPE_TRANSLATION;
				}
				break;
			case 2:
				M00 = -1.0;
				M10 = 0.0;
				M01 = 0.0;
				M11 = -1.0;
				M02 = anchorx + anchorx;
				M12 = anchory + anchory;
				if (M02 == 0.0 && M12 == 0.0)
				{
					State = APPLY_SCALE;
					Type_Renamed = TYPE_QUADRANT_ROTATION;
				}
				else
				{
					State = APPLY_SCALE | APPLY_TRANSLATE;
					Type_Renamed = TYPE_QUADRANT_ROTATION | TYPE_TRANSLATION;
				}
				break;
			case 3:
				M00 = 0.0;
				M10 = -1.0;
				M01 = 1.0;
				M11 = 0.0;
				M02 = anchorx - anchory;
				M12 = anchory + anchorx;
				if (M02 == 0.0 && M12 == 0.0)
				{
					State = APPLY_SHEAR;
					Type_Renamed = TYPE_QUADRANT_ROTATION;
				}
				else
				{
					State = APPLY_SHEAR | APPLY_TRANSLATE;
					Type_Renamed = TYPE_QUADRANT_ROTATION | TYPE_TRANSLATION;
				}
				break;
			}
		}

		/// <summary>
		/// Sets this transform to a scaling transformation.
		/// The matrix representing this transform becomes:
		/// <pre>
		///          [   sx   0    0   ]
		///          [   0    sy   0   ]
		///          [   0    0    1   ]
		/// </pre> </summary>
		/// <param name="sx"> the factor by which coordinates are scaled along the
		/// X axis direction </param>
		/// <param name="sy"> the factor by which coordinates are scaled along the
		/// Y axis direction
		/// @since 1.2 </param>
		public virtual void SetToScale(double sx, double sy)
		{
			M00 = sx;
			M10 = 0.0;
			M01 = 0.0;
			M11 = sy;
			M02 = 0.0;
			M12 = 0.0;
			if (sx != 1.0 || sy != 1.0)
			{
				State = APPLY_SCALE;
				Type_Renamed = TYPE_UNKNOWN;
			}
			else
			{
				State = APPLY_IDENTITY;
				Type_Renamed = TYPE_IDENTITY;
			}
		}

		/// <summary>
		/// Sets this transform to a shearing transformation.
		/// The matrix representing this transform becomes:
		/// <pre>
		///          [   1   shx   0   ]
		///          [  shy   1    0   ]
		///          [   0    0    1   ]
		/// </pre> </summary>
		/// <param name="shx"> the multiplier by which coordinates are shifted in the
		/// direction of the positive X axis as a factor of their Y coordinate </param>
		/// <param name="shy"> the multiplier by which coordinates are shifted in the
		/// direction of the positive Y axis as a factor of their X coordinate
		/// @since 1.2 </param>
		public virtual void SetToShear(double shx, double shy)
		{
			M00 = 1.0;
			M01 = shx;
			M10 = shy;
			M11 = 1.0;
			M02 = 0.0;
			M12 = 0.0;
			if (shx != 0.0 || shy != 0.0)
			{
				State = (APPLY_SHEAR | APPLY_SCALE);
				Type_Renamed = TYPE_UNKNOWN;
			}
			else
			{
				State = APPLY_IDENTITY;
				Type_Renamed = TYPE_IDENTITY;
			}
		}

		/// <summary>
		/// Sets this transform to a copy of the transform in the specified
		/// <code>AffineTransform</code> object. </summary>
		/// <param name="Tx"> the <code>AffineTransform</code> object from which to
		/// copy the transform
		/// @since 1.2 </param>
		public virtual AffineTransform Transform
		{
			set
			{
				this.M00 = value.M00;
				this.M10 = value.M10;
				this.M01 = value.M01;
				this.M11 = value.M11;
				this.M02 = value.M02;
				this.M12 = value.M12;
				this.State = value.State;
				this.Type_Renamed = value.Type_Renamed;
			}
		}

		/// <summary>
		/// Sets this transform to the matrix specified by the 6
		/// double precision values.
		/// </summary>
		/// <param name="m00"> the X coordinate scaling element of the 3x3 matrix </param>
		/// <param name="m10"> the Y coordinate shearing element of the 3x3 matrix </param>
		/// <param name="m01"> the X coordinate shearing element of the 3x3 matrix </param>
		/// <param name="m11"> the Y coordinate scaling element of the 3x3 matrix </param>
		/// <param name="m02"> the X coordinate translation element of the 3x3 matrix </param>
		/// <param name="m12"> the Y coordinate translation element of the 3x3 matrix
		/// @since 1.2 </param>
		public virtual void SetTransform(double m00, double m10, double m01, double m11, double m02, double m12)
		{
			this.M00 = m00;
			this.M10 = m10;
			this.M01 = m01;
			this.M11 = m11;
			this.M02 = m02;
			this.M12 = m12;
			UpdateState();
		}

		/// <summary>
		/// Concatenates an <code>AffineTransform</code> <code>Tx</code> to
		/// this <code>AffineTransform</code> Cx in the most commonly useful
		/// way to provide a new user space
		/// that is mapped to the former user space by <code>Tx</code>.
		/// Cx is updated to perform the combined transformation.
		/// Transforming a point p by the updated transform Cx' is
		/// equivalent to first transforming p by <code>Tx</code> and then
		/// transforming the result by the original transform Cx like this:
		/// Cx'(p) = Cx(Tx(p))
		/// In matrix notation, if this transform Cx is
		/// represented by the matrix [this] and <code>Tx</code> is represented
		/// by the matrix [Tx] then this method does the following:
		/// <pre>
		///          [this] = [this] x [Tx]
		/// </pre> </summary>
		/// <param name="Tx"> the <code>AffineTransform</code> object to be
		/// concatenated with this <code>AffineTransform</code> object. </param>
		/// <seealso cref= #preConcatenate
		/// @since 1.2 </seealso>
//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @SuppressWarnings("fallthrough") public void concatenate(AffineTransform Tx)
		public virtual void Concatenate(AffineTransform Tx)
		{
			double M0, M1;
			double T00, T01, T10, T11;
			double T02, T12;
			int mystate = State;
			int txstate = Tx.State;
			switch ((txstate << HI_SHIFT) | mystate)
			{

				/* ---------- Tx == IDENTITY cases ---------- */
			case (HI_IDENTITY | APPLY_IDENTITY):
			case (HI_IDENTITY | APPLY_TRANSLATE):
			case (HI_IDENTITY | APPLY_SCALE):
			case (HI_IDENTITY | APPLY_SCALE | APPLY_TRANSLATE):
			case (HI_IDENTITY | APPLY_SHEAR):
			case (HI_IDENTITY | APPLY_SHEAR | APPLY_TRANSLATE):
			case (HI_IDENTITY | APPLY_SHEAR | APPLY_SCALE):
			case (HI_IDENTITY | APPLY_SHEAR | APPLY_SCALE | APPLY_TRANSLATE):
				return;

				/* ---------- this == IDENTITY cases ---------- */
			case (HI_SHEAR | HI_SCALE | HI_TRANSLATE | APPLY_IDENTITY):
				M01 = Tx.M01;
				M10 = Tx.M10;
				/* NOBREAK */
				goto case (HI_SCALE | HI_TRANSLATE | APPLY_IDENTITY);
			case (HI_SCALE | HI_TRANSLATE | APPLY_IDENTITY):
				M00 = Tx.M00;
				M11 = Tx.M11;
				/* NOBREAK */
				goto case (HI_TRANSLATE | APPLY_IDENTITY);
			case (HI_TRANSLATE | APPLY_IDENTITY):
				M02 = Tx.M02;
				M12 = Tx.M12;
				State = txstate;
				Type_Renamed = Tx.Type_Renamed;
				return;
			case (HI_SHEAR | HI_SCALE | APPLY_IDENTITY):
				M01 = Tx.M01;
				M10 = Tx.M10;
				/* NOBREAK */
				goto case (HI_SCALE | APPLY_IDENTITY);
			case (HI_SCALE | APPLY_IDENTITY):
				M00 = Tx.M00;
				M11 = Tx.M11;
				State = txstate;
				Type_Renamed = Tx.Type_Renamed;
				return;
			case (HI_SHEAR | HI_TRANSLATE | APPLY_IDENTITY):
				M02 = Tx.M02;
				M12 = Tx.M12;
				/* NOBREAK */
				goto case (HI_SHEAR | APPLY_IDENTITY);
			case (HI_SHEAR | APPLY_IDENTITY):
				M01 = Tx.M01;
				M10 = Tx.M10;
				M00 = M11 = 0.0;
				State = txstate;
				Type_Renamed = Tx.Type_Renamed;
				return;

				/* ---------- Tx == TRANSLATE cases ---------- */
			case (HI_TRANSLATE | APPLY_SHEAR | APPLY_SCALE | APPLY_TRANSLATE):
			case (HI_TRANSLATE | APPLY_SHEAR | APPLY_SCALE):
			case (HI_TRANSLATE | APPLY_SHEAR | APPLY_TRANSLATE):
			case (HI_TRANSLATE | APPLY_SHEAR):
			case (HI_TRANSLATE | APPLY_SCALE | APPLY_TRANSLATE):
			case (HI_TRANSLATE | APPLY_SCALE):
			case (HI_TRANSLATE | APPLY_TRANSLATE):
				Translate(Tx.M02, Tx.M12);
				return;

				/* ---------- Tx == SCALE cases ---------- */
			case (HI_SCALE | APPLY_SHEAR | APPLY_SCALE | APPLY_TRANSLATE):
			case (HI_SCALE | APPLY_SHEAR | APPLY_SCALE):
			case (HI_SCALE | APPLY_SHEAR | APPLY_TRANSLATE):
			case (HI_SCALE | APPLY_SHEAR):
			case (HI_SCALE | APPLY_SCALE | APPLY_TRANSLATE):
			case (HI_SCALE | APPLY_SCALE):
			case (HI_SCALE | APPLY_TRANSLATE):
				Scale(Tx.M00, Tx.M11);
				return;

				/* ---------- Tx == SHEAR cases ---------- */
			case (HI_SHEAR | APPLY_SHEAR | APPLY_SCALE | APPLY_TRANSLATE):
			case (HI_SHEAR | APPLY_SHEAR | APPLY_SCALE):
				T01 = Tx.M01;
				T10 = Tx.M10;
				M0 = M00;
				M00 = M01 * T10;
				M01 = M0 * T01;
				M0 = M10;
				M10 = M11 * T10;
				M11 = M0 * T01;
				Type_Renamed = TYPE_UNKNOWN;
				return;
			case (HI_SHEAR | APPLY_SHEAR | APPLY_TRANSLATE):
			case (HI_SHEAR | APPLY_SHEAR):
				M00 = M01 * Tx.M10;
				M01 = 0.0;
				M11 = M10 * Tx.M01;
				M10 = 0.0;
				State = mystate ^ (APPLY_SHEAR | APPLY_SCALE);
				Type_Renamed = TYPE_UNKNOWN;
				return;
			case (HI_SHEAR | APPLY_SCALE | APPLY_TRANSLATE):
			case (HI_SHEAR | APPLY_SCALE):
				M01 = M00 * Tx.M01;
				M00 = 0.0;
				M10 = M11 * Tx.M10;
				M11 = 0.0;
				State = mystate ^ (APPLY_SHEAR | APPLY_SCALE);
				Type_Renamed = TYPE_UNKNOWN;
				return;
			case (HI_SHEAR | APPLY_TRANSLATE):
				M00 = 0.0;
				M01 = Tx.M01;
				M10 = Tx.M10;
				M11 = 0.0;
				State = APPLY_TRANSLATE | APPLY_SHEAR;
				Type_Renamed = TYPE_UNKNOWN;
				return;
			}
			// If Tx has more than one attribute, it is not worth optimizing
			// all of those cases...
			T00 = Tx.M00;
			T01 = Tx.M01;
			T02 = Tx.M02;
			T10 = Tx.M10;
			T11 = Tx.M11;
			T12 = Tx.M12;
			switch (mystate)
			{
			default:
				StateError();
				/* NOTREACHED */
				goto case (APPLY_SHEAR | APPLY_SCALE);
			case (APPLY_SHEAR | APPLY_SCALE):
				State = mystate | txstate;
				/* NOBREAK */
				goto case (APPLY_SHEAR | APPLY_SCALE | APPLY_TRANSLATE);
			case (APPLY_SHEAR | APPLY_SCALE | APPLY_TRANSLATE):
				M0 = M00;
				M1 = M01;
				M00 = T00 * M0 + T10 * M1;
				M01 = T01 * M0 + T11 * M1;
				M02 += T02 * M0 + T12 * M1;

				M0 = M10;
				M1 = M11;
				M10 = T00 * M0 + T10 * M1;
				M11 = T01 * M0 + T11 * M1;
				M12 += T02 * M0 + T12 * M1;
				Type_Renamed = TYPE_UNKNOWN;
				return;

			case (APPLY_SHEAR | APPLY_TRANSLATE):
			case (APPLY_SHEAR):
				M0 = M01;
				M00 = T10 * M0;
				M01 = T11 * M0;
				M02 += T12 * M0;

				M0 = M10;
				M10 = T00 * M0;
				M11 = T01 * M0;
				M12 += T02 * M0;
				break;

			case (APPLY_SCALE | APPLY_TRANSLATE):
			case (APPLY_SCALE):
				M0 = M00;
				M00 = T00 * M0;
				M01 = T01 * M0;
				M02 += T02 * M0;

				M0 = M11;
				M10 = T10 * M0;
				M11 = T11 * M0;
				M12 += T12 * M0;
				break;

			case (APPLY_TRANSLATE):
				M00 = T00;
				M01 = T01;
				M02 += T02;

				M10 = T10;
				M11 = T11;
				M12 += T12;
				State = txstate | APPLY_TRANSLATE;
				Type_Renamed = TYPE_UNKNOWN;
				return;
			}
			UpdateState();
		}

		/// <summary>
		/// Concatenates an <code>AffineTransform</code> <code>Tx</code> to
		/// this <code>AffineTransform</code> Cx
		/// in a less commonly used way such that <code>Tx</code> modifies the
		/// coordinate transformation relative to the absolute pixel
		/// space rather than relative to the existing user space.
		/// Cx is updated to perform the combined transformation.
		/// Transforming a point p by the updated transform Cx' is
		/// equivalent to first transforming p by the original transform
		/// Cx and then transforming the result by
		/// <code>Tx</code> like this:
		/// Cx'(p) = Tx(Cx(p))
		/// In matrix notation, if this transform Cx
		/// is represented by the matrix [this] and <code>Tx</code> is
		/// represented by the matrix [Tx] then this method does the
		/// following:
		/// <pre>
		///          [this] = [Tx] x [this]
		/// </pre> </summary>
		/// <param name="Tx"> the <code>AffineTransform</code> object to be
		/// concatenated with this <code>AffineTransform</code> object. </param>
		/// <seealso cref= #concatenate
		/// @since 1.2 </seealso>
//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @SuppressWarnings("fallthrough") public void preConcatenate(AffineTransform Tx)
		public virtual void PreConcatenate(AffineTransform Tx)
		{
			double M0, M1;
			double T00, T01, T10, T11;
			double T02, T12;
			int mystate = State;
			int txstate = Tx.State;
			switch ((txstate << HI_SHIFT) | mystate)
			{
			case (HI_IDENTITY | APPLY_IDENTITY):
			case (HI_IDENTITY | APPLY_TRANSLATE):
			case (HI_IDENTITY | APPLY_SCALE):
			case (HI_IDENTITY | APPLY_SCALE | APPLY_TRANSLATE):
			case (HI_IDENTITY | APPLY_SHEAR):
			case (HI_IDENTITY | APPLY_SHEAR | APPLY_TRANSLATE):
			case (HI_IDENTITY | APPLY_SHEAR | APPLY_SCALE):
			case (HI_IDENTITY | APPLY_SHEAR | APPLY_SCALE | APPLY_TRANSLATE):
				// Tx is IDENTITY...
				return;

			case (HI_TRANSLATE | APPLY_IDENTITY):
			case (HI_TRANSLATE | APPLY_SCALE):
			case (HI_TRANSLATE | APPLY_SHEAR):
			case (HI_TRANSLATE | APPLY_SHEAR | APPLY_SCALE):
				// Tx is TRANSLATE, this has no TRANSLATE
				M02 = Tx.M02;
				M12 = Tx.M12;
				State = mystate | APPLY_TRANSLATE;
				Type_Renamed |= TYPE_TRANSLATION;
				return;

			case (HI_TRANSLATE | APPLY_TRANSLATE):
			case (HI_TRANSLATE | APPLY_SCALE | APPLY_TRANSLATE):
			case (HI_TRANSLATE | APPLY_SHEAR | APPLY_TRANSLATE):
			case (HI_TRANSLATE | APPLY_SHEAR | APPLY_SCALE | APPLY_TRANSLATE):
				// Tx is TRANSLATE, this has one too
				M02 = M02 + Tx.M02;
				M12 = M12 + Tx.M12;
				return;

			case (HI_SCALE | APPLY_TRANSLATE):
			case (HI_SCALE | APPLY_IDENTITY):
				// Only these two existing states need a new state
				State = mystate | APPLY_SCALE;
				/* NOBREAK */
				goto case (HI_SCALE | APPLY_SHEAR | APPLY_SCALE | APPLY_TRANSLATE);
			case (HI_SCALE | APPLY_SHEAR | APPLY_SCALE | APPLY_TRANSLATE):
			case (HI_SCALE | APPLY_SHEAR | APPLY_SCALE):
			case (HI_SCALE | APPLY_SHEAR | APPLY_TRANSLATE):
			case (HI_SCALE | APPLY_SHEAR):
			case (HI_SCALE | APPLY_SCALE | APPLY_TRANSLATE):
			case (HI_SCALE | APPLY_SCALE):
				// Tx is SCALE, this is anything
				T00 = Tx.M00;
				T11 = Tx.M11;
				if ((mystate & APPLY_SHEAR) != 0)
				{
					M01 = M01 * T00;
					M10 = M10 * T11;
					if ((mystate & APPLY_SCALE) != 0)
					{
						M00 = M00 * T00;
						M11 = M11 * T11;
					}
				}
				else
				{
					M00 = M00 * T00;
					M11 = M11 * T11;
				}
				if ((mystate & APPLY_TRANSLATE) != 0)
				{
					M02 = M02 * T00;
					M12 = M12 * T11;
				}
				Type_Renamed = TYPE_UNKNOWN;
				return;
			case (HI_SHEAR | APPLY_SHEAR | APPLY_TRANSLATE):
			case (HI_SHEAR | APPLY_SHEAR):
				mystate = mystate | APPLY_SCALE;
				/* NOBREAK */
				goto case (HI_SHEAR | APPLY_TRANSLATE);
			case (HI_SHEAR | APPLY_TRANSLATE):
			case (HI_SHEAR | APPLY_IDENTITY):
			case (HI_SHEAR | APPLY_SCALE | APPLY_TRANSLATE):
			case (HI_SHEAR | APPLY_SCALE):
				State = mystate ^ APPLY_SHEAR;
				/* NOBREAK */
				goto case (HI_SHEAR | APPLY_SHEAR | APPLY_SCALE | APPLY_TRANSLATE);
			case (HI_SHEAR | APPLY_SHEAR | APPLY_SCALE | APPLY_TRANSLATE):
			case (HI_SHEAR | APPLY_SHEAR | APPLY_SCALE):
				// Tx is SHEAR, this is anything
				T01 = Tx.M01;
				T10 = Tx.M10;

				M0 = M00;
				M00 = M10 * T01;
				M10 = M0 * T10;

				M0 = M01;
				M01 = M11 * T01;
				M11 = M0 * T10;

				M0 = M02;
				M02 = M12 * T01;
				M12 = M0 * T10;
				Type_Renamed = TYPE_UNKNOWN;
				return;
			}
			// If Tx has more than one attribute, it is not worth optimizing
			// all of those cases...
			T00 = Tx.M00;
			T01 = Tx.M01;
			T02 = Tx.M02;
			T10 = Tx.M10;
			T11 = Tx.M11;
			T12 = Tx.M12;
			switch (mystate)
			{
			default:
				StateError();
				/* NOTREACHED */
				goto case (APPLY_SHEAR | APPLY_SCALE | APPLY_TRANSLATE);
			case (APPLY_SHEAR | APPLY_SCALE | APPLY_TRANSLATE):
				M0 = M02;
				M1 = M12;
				T02 += M0 * T00 + M1 * T01;
				T12 += M0 * T10 + M1 * T11;

				/* NOBREAK */
				goto case (APPLY_SHEAR | APPLY_SCALE);
			case (APPLY_SHEAR | APPLY_SCALE):
				M02 = T02;
				M12 = T12;

				M0 = M00;
				M1 = M10;
				M00 = M0 * T00 + M1 * T01;
				M10 = M0 * T10 + M1 * T11;

				M0 = M01;
				M1 = M11;
				M01 = M0 * T00 + M1 * T01;
				M11 = M0 * T10 + M1 * T11;
				break;

			case (APPLY_SHEAR | APPLY_TRANSLATE):
				M0 = M02;
				M1 = M12;
				T02 += M0 * T00 + M1 * T01;
				T12 += M0 * T10 + M1 * T11;

				/* NOBREAK */
				goto case (APPLY_SHEAR);
			case (APPLY_SHEAR):
				M02 = T02;
				M12 = T12;

				M0 = M10;
				M00 = M0 * T01;
				M10 = M0 * T11;

				M0 = M01;
				M01 = M0 * T00;
				M11 = M0 * T10;
				break;

			case (APPLY_SCALE | APPLY_TRANSLATE):
				M0 = M02;
				M1 = M12;
				T02 += M0 * T00 + M1 * T01;
				T12 += M0 * T10 + M1 * T11;

				/* NOBREAK */
				goto case (APPLY_SCALE);
			case (APPLY_SCALE):
				M02 = T02;
				M12 = T12;

				M0 = M00;
				M00 = M0 * T00;
				M10 = M0 * T10;

				M0 = M11;
				M01 = M0 * T01;
				M11 = M0 * T11;
				break;

			case (APPLY_TRANSLATE):
				M0 = M02;
				M1 = M12;
				T02 += M0 * T00 + M1 * T01;
				T12 += M0 * T10 + M1 * T11;

				/* NOBREAK */
				goto case (APPLY_IDENTITY);
			case (APPLY_IDENTITY):
				M02 = T02;
				M12 = T12;

				M00 = T00;
				M10 = T10;

				M01 = T01;
				M11 = T11;

				State = mystate | txstate;
				Type_Renamed = TYPE_UNKNOWN;
				return;
			}
			UpdateState();
		}

		/// <summary>
		/// Returns an <code>AffineTransform</code> object representing the
		/// inverse transformation.
		/// The inverse transform Tx' of this transform Tx
		/// maps coordinates transformed by Tx back
		/// to their original coordinates.
		/// In other words, Tx'(Tx(p)) = p = Tx(Tx'(p)).
		/// <para>
		/// If this transform maps all coordinates onto a point or a line
		/// then it will not have an inverse, since coordinates that do
		/// not lie on the destination point or line will not have an inverse
		/// mapping.
		/// The <code>getDeterminant</code> method can be used to determine if this
		/// transform has no inverse, in which case an exception will be
		/// thrown if the <code>createInverse</code> method is called.
		/// </para>
		/// </summary>
		/// <returns> a new <code>AffineTransform</code> object representing the
		/// inverse transformation. </returns>
		/// <seealso cref= #getDeterminant </seealso>
		/// <exception cref="NoninvertibleTransformException">
		/// if the matrix cannot be inverted.
		/// @since 1.2 </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public AffineTransform createInverse() throws NoninvertibleTransformException
		public virtual AffineTransform CreateInverse()
		{
			double det;
			switch (State)
			{
			default:
				StateError();
				/* NOTREACHED */
				return null;
			case (APPLY_SHEAR | APPLY_SCALE | APPLY_TRANSLATE):
				det = M00 * M11 - M01 * M10;
				if (System.Math.Abs(det) <= Double.Epsilon)
				{
					throw new NoninvertibleTransformException("Determinant is " + det);
				}
				return new AffineTransform(M11 / det, -M10 / det, -M01 / det, M00 / det, (M01 * M12 - M11 * M02) / det, (M10 * M02 - M00 * M12) / det, (APPLY_SHEAR | APPLY_SCALE | APPLY_TRANSLATE));
			case (APPLY_SHEAR | APPLY_SCALE):
				det = M00 * M11 - M01 * M10;
				if (System.Math.Abs(det) <= Double.Epsilon)
				{
					throw new NoninvertibleTransformException("Determinant is " + det);
				}
				return new AffineTransform(M11 / det, -M10 / det, -M01 / det, M00 / det, 0.0, 0.0, (APPLY_SHEAR | APPLY_SCALE));
			case (APPLY_SHEAR | APPLY_TRANSLATE):
				if (M01 == 0.0 || M10 == 0.0)
				{
					throw new NoninvertibleTransformException("Determinant is 0");
				}
				return new AffineTransform(0.0, 1.0 / M01, 1.0 / M10, 0.0, -M12 / M10, -M02 / M01, (APPLY_SHEAR | APPLY_TRANSLATE));
			case (APPLY_SHEAR):
				if (M01 == 0.0 || M10 == 0.0)
				{
					throw new NoninvertibleTransformException("Determinant is 0");
				}
				return new AffineTransform(0.0, 1.0 / M01, 1.0 / M10, 0.0, 0.0, 0.0, (APPLY_SHEAR));
			case (APPLY_SCALE | APPLY_TRANSLATE):
				if (M00 == 0.0 || M11 == 0.0)
				{
					throw new NoninvertibleTransformException("Determinant is 0");
				}
				return new AffineTransform(1.0 / M00, 0.0, 0.0, 1.0 / M11, -M02 / M00, -M12 / M11, (APPLY_SCALE | APPLY_TRANSLATE));
			case (APPLY_SCALE):
				if (M00 == 0.0 || M11 == 0.0)
				{
					throw new NoninvertibleTransformException("Determinant is 0");
				}
				return new AffineTransform(1.0 / M00, 0.0, 0.0, 1.0 / M11, 0.0, 0.0, (APPLY_SCALE));
			case (APPLY_TRANSLATE):
				return new AffineTransform(1.0, 0.0, 0.0, 1.0, -M02, -M12, (APPLY_TRANSLATE));
			case (APPLY_IDENTITY):
				return new AffineTransform();
			}

			/* NOTREACHED */
		}

		/// <summary>
		/// Sets this transform to the inverse of itself.
		/// The inverse transform Tx' of this transform Tx
		/// maps coordinates transformed by Tx back
		/// to their original coordinates.
		/// In other words, Tx'(Tx(p)) = p = Tx(Tx'(p)).
		/// <para>
		/// If this transform maps all coordinates onto a point or a line
		/// then it will not have an inverse, since coordinates that do
		/// not lie on the destination point or line will not have an inverse
		/// mapping.
		/// The <code>getDeterminant</code> method can be used to determine if this
		/// transform has no inverse, in which case an exception will be
		/// thrown if the <code>invert</code> method is called.
		/// </para>
		/// </summary>
		/// <seealso cref= #getDeterminant </seealso>
		/// <exception cref="NoninvertibleTransformException">
		/// if the matrix cannot be inverted.
		/// @since 1.6 </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public void invert() throws NoninvertibleTransformException
		public virtual void Invert()
		{
			double M00, M01, M02;
			double M10, M11, M12;
			double det;
			switch (State)
			{
			default:
				StateError();
				/* NOTREACHED */
				return;
			case (APPLY_SHEAR | APPLY_SCALE | APPLY_TRANSLATE):
				M00 = M00;
				M01 = M01;
				M02 = M02;
				M10 = M10;
				M11 = M11;
				M12 = M12;
				det = M00 * M11 - M01 * M10;
				if (System.Math.Abs(det) <= Double.Epsilon)
				{
					throw new NoninvertibleTransformException("Determinant is " + det);
				}
				M00 = M11 / det;
				M10 = -M10 / det;
				M01 = -M01 / det;
				M11 = M00 / det;
				M02 = (M01 * M12 - M11 * M02) / det;
				M12 = (M10 * M02 - M00 * M12) / det;
				break;
			case (APPLY_SHEAR | APPLY_SCALE):
				M00 = M00;
				M01 = M01;
				M10 = M10;
				M11 = M11;
				det = M00 * M11 - M01 * M10;
				if (System.Math.Abs(det) <= Double.Epsilon)
				{
					throw new NoninvertibleTransformException("Determinant is " + det);
				}
				M00 = M11 / det;
				M10 = -M10 / det;
				M01 = -M01 / det;
				M11 = M00 / det;
				// m02 = 0.0;
				// m12 = 0.0;
				break;
			case (APPLY_SHEAR | APPLY_TRANSLATE):
				M01 = M01;
				M02 = M02;
				M10 = M10;
				M12 = M12;
				if (M01 == 0.0 || M10 == 0.0)
				{
					throw new NoninvertibleTransformException("Determinant is 0");
				}
				// m00 = 0.0;
				M10 = 1.0 / M01;
				M01 = 1.0 / M10;
				// m11 = 0.0;
				M02 = -M12 / M10;
				M12 = -M02 / M01;
				break;
			case (APPLY_SHEAR):
				M01 = M01;
				M10 = M10;
				if (M01 == 0.0 || M10 == 0.0)
				{
					throw new NoninvertibleTransformException("Determinant is 0");
				}
				// m00 = 0.0;
				M10 = 1.0 / M01;
				M01 = 1.0 / M10;
				// m11 = 0.0;
				// m02 = 0.0;
				// m12 = 0.0;
				break;
			case (APPLY_SCALE | APPLY_TRANSLATE):
				M00 = M00;
				M02 = M02;
				M11 = M11;
				M12 = M12;
				if (M00 == 0.0 || M11 == 0.0)
				{
					throw new NoninvertibleTransformException("Determinant is 0");
				}
				M00 = 1.0 / M00;
				// m10 = 0.0;
				// m01 = 0.0;
				M11 = 1.0 / M11;
				M02 = -M02 / M00;
				M12 = -M12 / M11;
				break;
			case (APPLY_SCALE):
				M00 = M00;
				M11 = M11;
				if (M00 == 0.0 || M11 == 0.0)
				{
					throw new NoninvertibleTransformException("Determinant is 0");
				}
				M00 = 1.0 / M00;
				// m10 = 0.0;
				// m01 = 0.0;
				M11 = 1.0 / M11;
				// m02 = 0.0;
				// m12 = 0.0;
				break;
			case (APPLY_TRANSLATE):
				// m00 = 1.0;
				// m10 = 0.0;
				// m01 = 0.0;
				// m11 = 1.0;
				M02 = -M02;
				M12 = -M12;
				break;
			case (APPLY_IDENTITY):
				// m00 = 1.0;
				// m10 = 0.0;
				// m01 = 0.0;
				// m11 = 1.0;
				// m02 = 0.0;
				// m12 = 0.0;
				break;
			}
		}

		/// <summary>
		/// Transforms the specified <code>ptSrc</code> and stores the result
		/// in <code>ptDst</code>.
		/// If <code>ptDst</code> is <code>null</code>, a new <seealso cref="Point2D"/>
		/// object is allocated and then the result of the transformation is
		/// stored in this object.
		/// In either case, <code>ptDst</code>, which contains the
		/// transformed point, is returned for convenience.
		/// If <code>ptSrc</code> and <code>ptDst</code> are the same
		/// object, the input point is correctly overwritten with
		/// the transformed point. </summary>
		/// <param name="ptSrc"> the specified <code>Point2D</code> to be transformed </param>
		/// <param name="ptDst"> the specified <code>Point2D</code> that stores the
		/// result of transforming <code>ptSrc</code> </param>
		/// <returns> the <code>ptDst</code> after transforming
		/// <code>ptSrc</code> and storing the result in <code>ptDst</code>.
		/// @since 1.2 </returns>
		public virtual Point2D Transform(Point2D ptSrc, Point2D ptDst)
		{
			if (ptDst == null)
			{
				if (ptSrc is Point2D.Double)
				{
					ptDst = new Point2D.Double();
				}
				else
				{
					ptDst = new Point2D.Float();
				}
			}
			// Copy source coords into local variables in case src == dst
			double x = ptSrc.X;
			double y = ptSrc.Y;
			switch (State)
			{
			default:
				StateError();
				/* NOTREACHED */
				return null;
			case (APPLY_SHEAR | APPLY_SCALE | APPLY_TRANSLATE):
				ptDst.SetLocation(x * M00 + y * M01 + M02, x * M10 + y * M11 + M12);
				return ptDst;
			case (APPLY_SHEAR | APPLY_SCALE):
				ptDst.SetLocation(x * M00 + y * M01, x * M10 + y * M11);
				return ptDst;
			case (APPLY_SHEAR | APPLY_TRANSLATE):
				ptDst.SetLocation(y * M01 + M02, x * M10 + M12);
				return ptDst;
			case (APPLY_SHEAR):
				ptDst.SetLocation(y * M01, x * M10);
				return ptDst;
			case (APPLY_SCALE | APPLY_TRANSLATE):
				ptDst.SetLocation(x * M00 + M02, y * M11 + M12);
				return ptDst;
			case (APPLY_SCALE):
				ptDst.SetLocation(x * M00, y * M11);
				return ptDst;
			case (APPLY_TRANSLATE):
				ptDst.SetLocation(x + M02, y + M12);
				return ptDst;
			case (APPLY_IDENTITY):
				ptDst.SetLocation(x, y);
				return ptDst;
			}

			/* NOTREACHED */
		}

		/// <summary>
		/// Transforms an array of point objects by this transform.
		/// If any element of the <code>ptDst</code> array is
		/// <code>null</code>, a new <code>Point2D</code> object is allocated
		/// and stored into that element before storing the results of the
		/// transformation.
		/// <para>
		/// Note that this method does not take any precautions to
		/// avoid problems caused by storing results into <code>Point2D</code>
		/// objects that will be used as the source for calculations
		/// further down the source array.
		/// This method does guarantee that if a specified <code>Point2D</code>
		/// object is both the source and destination for the same single point
		/// transform operation then the results will not be stored until
		/// the calculations are complete to avoid storing the results on
		/// top of the operands.
		/// If, however, the destination <code>Point2D</code> object for one
		/// operation is the same object as the source <code>Point2D</code>
		/// object for another operation further down the source array then
		/// the original coordinates in that point are overwritten before
		/// they can be converted.
		/// </para>
		/// </summary>
		/// <param name="ptSrc"> the array containing the source point objects </param>
		/// <param name="ptDst"> the array into which the transform point objects are
		/// returned </param>
		/// <param name="srcOff"> the offset to the first point object to be
		/// transformed in the source array </param>
		/// <param name="dstOff"> the offset to the location of the first
		/// transformed point object that is stored in the destination array </param>
		/// <param name="numPts"> the number of point objects to be transformed
		/// @since 1.2 </param>
		public virtual void Transform(Point2D[] ptSrc, int srcOff, Point2D[] ptDst, int dstOff, int numPts)
		{
			int state = this.State;
			while (--numPts >= 0)
			{
				// Copy source coords into local variables in case src == dst
				Point2D src = ptSrc[srcOff++];
				double x = src.X;
				double y = src.Y;
				Point2D dst = ptDst[dstOff++];
				if (dst == null)
				{
					if (src is Point2D.Double)
					{
						dst = new Point2D.Double();
					}
					else
					{
						dst = new Point2D.Float();
					}
					ptDst[dstOff - 1] = dst;
				}
				switch (state)
				{
				default:
					StateError();
					/* NOTREACHED */
					return;
				case (APPLY_SHEAR | APPLY_SCALE | APPLY_TRANSLATE):
					dst.SetLocation(x * M00 + y * M01 + M02, x * M10 + y * M11 + M12);
					break;
				case (APPLY_SHEAR | APPLY_SCALE):
					dst.SetLocation(x * M00 + y * M01, x * M10 + y * M11);
					break;
				case (APPLY_SHEAR | APPLY_TRANSLATE):
					dst.SetLocation(y * M01 + M02, x * M10 + M12);
					break;
				case (APPLY_SHEAR):
					dst.SetLocation(y * M01, x * M10);
					break;
				case (APPLY_SCALE | APPLY_TRANSLATE):
					dst.SetLocation(x * M00 + M02, y * M11 + M12);
					break;
				case (APPLY_SCALE):
					dst.SetLocation(x * M00, y * M11);
					break;
				case (APPLY_TRANSLATE):
					dst.SetLocation(x + M02, y + M12);
					break;
				case (APPLY_IDENTITY):
					dst.SetLocation(x, y);
					break;
				}
			}

			/* NOTREACHED */
		}

		/// <summary>
		/// Transforms an array of floating point coordinates by this transform.
		/// The two coordinate array sections can be exactly the same or
		/// can be overlapping sections of the same array without affecting the
		/// validity of the results.
		/// This method ensures that no source coordinates are overwritten by a
		/// previous operation before they can be transformed.
		/// The coordinates are stored in the arrays starting at the specified
		/// offset in the order <code>[x0, y0, x1, y1, ..., xn, yn]</code>. </summary>
		/// <param name="srcPts"> the array containing the source point coordinates.
		/// Each point is stored as a pair of x,&nbsp;y coordinates. </param>
		/// <param name="dstPts"> the array into which the transformed point coordinates
		/// are returned.  Each point is stored as a pair of x,&nbsp;y
		/// coordinates. </param>
		/// <param name="srcOff"> the offset to the first point to be transformed
		/// in the source array </param>
		/// <param name="dstOff"> the offset to the location of the first
		/// transformed point that is stored in the destination array </param>
		/// <param name="numPts"> the number of points to be transformed
		/// @since 1.2 </param>
		public virtual void Transform(float[] srcPts, int srcOff, float[] dstPts, int dstOff, int numPts)
		{
			double M00, M01, M02, M10, M11, M12; // For caching
			if (dstPts == srcPts && dstOff > srcOff && dstOff < srcOff + numPts * 2)
			{
				// If the arrays overlap partially with the destination higher
				// than the source and we transform the coordinates normally
				// we would overwrite some of the later source coordinates
				// with results of previous transformations.
				// To get around this we use arraycopy to copy the points
				// to their final destination with correct overwrite
				// handling and then transform them in place in the new
				// safer location.
				System.Array.Copy(srcPts, srcOff, dstPts, dstOff, numPts * 2);
				// srcPts = dstPts;         // They are known to be equal.
				srcOff = dstOff;
			}
			switch (State)
			{
			default:
				StateError();
				/* NOTREACHED */
				return;
			case (APPLY_SHEAR | APPLY_SCALE | APPLY_TRANSLATE):
				M00 = M00;
				M01 = M01;
				M02 = M02;
				M10 = M10;
				M11 = M11;
				M12 = M12;
				while (--numPts >= 0)
				{
					double x = srcPts[srcOff++];
					double y = srcPts[srcOff++];
					dstPts[dstOff++] = (float)(M00 * x + M01 * y + M02);
					dstPts[dstOff++] = (float)(M10 * x + M11 * y + M12);
				}
				return;
			case (APPLY_SHEAR | APPLY_SCALE):
				M00 = M00;
				M01 = M01;
				M10 = M10;
				M11 = M11;
				while (--numPts >= 0)
				{
					double x = srcPts[srcOff++];
					double y = srcPts[srcOff++];
					dstPts[dstOff++] = (float)(M00 * x + M01 * y);
					dstPts[dstOff++] = (float)(M10 * x + M11 * y);
				}
				return;
			case (APPLY_SHEAR | APPLY_TRANSLATE):
				M01 = M01;
				M02 = M02;
				M10 = M10;
				M12 = M12;
				while (--numPts >= 0)
				{
					double x = srcPts[srcOff++];
					dstPts[dstOff++] = (float)(M01 * srcPts[srcOff++] + M02);
					dstPts[dstOff++] = (float)(M10 * x + M12);
				}
				return;
			case (APPLY_SHEAR):
				M01 = M01;
				M10 = M10;
				while (--numPts >= 0)
				{
					double x = srcPts[srcOff++];
					dstPts[dstOff++] = (float)(M01 * srcPts[srcOff++]);
					dstPts[dstOff++] = (float)(M10 * x);
				}
				return;
			case (APPLY_SCALE | APPLY_TRANSLATE):
				M00 = M00;
				M02 = M02;
				M11 = M11;
				M12 = M12;
				while (--numPts >= 0)
				{
					dstPts[dstOff++] = (float)(M00 * srcPts[srcOff++] + M02);
					dstPts[dstOff++] = (float)(M11 * srcPts[srcOff++] + M12);
				}
				return;
			case (APPLY_SCALE):
				M00 = M00;
				M11 = M11;
				while (--numPts >= 0)
				{
					dstPts[dstOff++] = (float)(M00 * srcPts[srcOff++]);
					dstPts[dstOff++] = (float)(M11 * srcPts[srcOff++]);
				}
				return;
			case (APPLY_TRANSLATE):
				M02 = M02;
				M12 = M12;
				while (--numPts >= 0)
				{
					dstPts[dstOff++] = (float)(srcPts[srcOff++] + M02);
					dstPts[dstOff++] = (float)(srcPts[srcOff++] + M12);
				}
				return;
			case (APPLY_IDENTITY):
				if (srcPts != dstPts || srcOff != dstOff)
				{
					System.Array.Copy(srcPts, srcOff, dstPts, dstOff, numPts * 2);
				}
				return;
			}

			/* NOTREACHED */
		}

		/// <summary>
		/// Transforms an array of double precision coordinates by this transform.
		/// The two coordinate array sections can be exactly the same or
		/// can be overlapping sections of the same array without affecting the
		/// validity of the results.
		/// This method ensures that no source coordinates are
		/// overwritten by a previous operation before they can be transformed.
		/// The coordinates are stored in the arrays starting at the indicated
		/// offset in the order <code>[x0, y0, x1, y1, ..., xn, yn]</code>. </summary>
		/// <param name="srcPts"> the array containing the source point coordinates.
		/// Each point is stored as a pair of x,&nbsp;y coordinates. </param>
		/// <param name="dstPts"> the array into which the transformed point
		/// coordinates are returned.  Each point is stored as a pair of
		/// x,&nbsp;y coordinates. </param>
		/// <param name="srcOff"> the offset to the first point to be transformed
		/// in the source array </param>
		/// <param name="dstOff"> the offset to the location of the first
		/// transformed point that is stored in the destination array </param>
		/// <param name="numPts"> the number of point objects to be transformed
		/// @since 1.2 </param>
		public virtual void Transform(double[] srcPts, int srcOff, double[] dstPts, int dstOff, int numPts)
		{
			double M00, M01, M02, M10, M11, M12; // For caching
			if (dstPts == srcPts && dstOff > srcOff && dstOff < srcOff + numPts * 2)
			{
				// If the arrays overlap partially with the destination higher
				// than the source and we transform the coordinates normally
				// we would overwrite some of the later source coordinates
				// with results of previous transformations.
				// To get around this we use arraycopy to copy the points
				// to their final destination with correct overwrite
				// handling and then transform them in place in the new
				// safer location.
				System.Array.Copy(srcPts, srcOff, dstPts, dstOff, numPts * 2);
				// srcPts = dstPts;         // They are known to be equal.
				srcOff = dstOff;
			}
			switch (State)
			{
			default:
				StateError();
				/* NOTREACHED */
				return;
			case (APPLY_SHEAR | APPLY_SCALE | APPLY_TRANSLATE):
				M00 = M00;
				M01 = M01;
				M02 = M02;
				M10 = M10;
				M11 = M11;
				M12 = M12;
				while (--numPts >= 0)
				{
					double x = srcPts[srcOff++];
					double y = srcPts[srcOff++];
					dstPts[dstOff++] = M00 * x + M01 * y + M02;
					dstPts[dstOff++] = M10 * x + M11 * y + M12;
				}
				return;
			case (APPLY_SHEAR | APPLY_SCALE):
				M00 = M00;
				M01 = M01;
				M10 = M10;
				M11 = M11;
				while (--numPts >= 0)
				{
					double x = srcPts[srcOff++];
					double y = srcPts[srcOff++];
					dstPts[dstOff++] = M00 * x + M01 * y;
					dstPts[dstOff++] = M10 * x + M11 * y;
				}
				return;
			case (APPLY_SHEAR | APPLY_TRANSLATE):
				M01 = M01;
				M02 = M02;
				M10 = M10;
				M12 = M12;
				while (--numPts >= 0)
				{
					double x = srcPts[srcOff++];
					dstPts[dstOff++] = M01 * srcPts[srcOff++] + M02;
					dstPts[dstOff++] = M10 * x + M12;
				}
				return;
			case (APPLY_SHEAR):
				M01 = M01;
				M10 = M10;
				while (--numPts >= 0)
				{
					double x = srcPts[srcOff++];
					dstPts[dstOff++] = M01 * srcPts[srcOff++];
					dstPts[dstOff++] = M10 * x;
				}
				return;
			case (APPLY_SCALE | APPLY_TRANSLATE):
				M00 = M00;
				M02 = M02;
				M11 = M11;
				M12 = M12;
				while (--numPts >= 0)
				{
					dstPts[dstOff++] = M00 * srcPts[srcOff++] + M02;
					dstPts[dstOff++] = M11 * srcPts[srcOff++] + M12;
				}
				return;
			case (APPLY_SCALE):
				M00 = M00;
				M11 = M11;
				while (--numPts >= 0)
				{
					dstPts[dstOff++] = M00 * srcPts[srcOff++];
					dstPts[dstOff++] = M11 * srcPts[srcOff++];
				}
				return;
			case (APPLY_TRANSLATE):
				M02 = M02;
				M12 = M12;
				while (--numPts >= 0)
				{
					dstPts[dstOff++] = srcPts[srcOff++] + M02;
					dstPts[dstOff++] = srcPts[srcOff++] + M12;
				}
				return;
			case (APPLY_IDENTITY):
				if (srcPts != dstPts || srcOff != dstOff)
				{
					System.Array.Copy(srcPts, srcOff, dstPts, dstOff, numPts * 2);
				}
				return;
			}

			/* NOTREACHED */
		}

		/// <summary>
		/// Transforms an array of floating point coordinates by this transform
		/// and stores the results into an array of doubles.
		/// The coordinates are stored in the arrays starting at the specified
		/// offset in the order <code>[x0, y0, x1, y1, ..., xn, yn]</code>. </summary>
		/// <param name="srcPts"> the array containing the source point coordinates.
		/// Each point is stored as a pair of x,&nbsp;y coordinates. </param>
		/// <param name="dstPts"> the array into which the transformed point coordinates
		/// are returned.  Each point is stored as a pair of x,&nbsp;y
		/// coordinates. </param>
		/// <param name="srcOff"> the offset to the first point to be transformed
		/// in the source array </param>
		/// <param name="dstOff"> the offset to the location of the first
		/// transformed point that is stored in the destination array </param>
		/// <param name="numPts"> the number of points to be transformed
		/// @since 1.2 </param>
		public virtual void Transform(float[] srcPts, int srcOff, double[] dstPts, int dstOff, int numPts)
		{
			double M00, M01, M02, M10, M11, M12; // For caching
			switch (State)
			{
			default:
				StateError();
				/* NOTREACHED */
				return;
			case (APPLY_SHEAR | APPLY_SCALE | APPLY_TRANSLATE):
				M00 = M00;
				M01 = M01;
				M02 = M02;
				M10 = M10;
				M11 = M11;
				M12 = M12;
				while (--numPts >= 0)
				{
					double x = srcPts[srcOff++];
					double y = srcPts[srcOff++];
					dstPts[dstOff++] = M00 * x + M01 * y + M02;
					dstPts[dstOff++] = M10 * x + M11 * y + M12;
				}
				return;
			case (APPLY_SHEAR | APPLY_SCALE):
				M00 = M00;
				M01 = M01;
				M10 = M10;
				M11 = M11;
				while (--numPts >= 0)
				{
					double x = srcPts[srcOff++];
					double y = srcPts[srcOff++];
					dstPts[dstOff++] = M00 * x + M01 * y;
					dstPts[dstOff++] = M10 * x + M11 * y;
				}
				return;
			case (APPLY_SHEAR | APPLY_TRANSLATE):
				M01 = M01;
				M02 = M02;
				M10 = M10;
				M12 = M12;
				while (--numPts >= 0)
				{
					double x = srcPts[srcOff++];
					dstPts[dstOff++] = M01 * srcPts[srcOff++] + M02;
					dstPts[dstOff++] = M10 * x + M12;
				}
				return;
			case (APPLY_SHEAR):
				M01 = M01;
				M10 = M10;
				while (--numPts >= 0)
				{
					double x = srcPts[srcOff++];
					dstPts[dstOff++] = M01 * srcPts[srcOff++];
					dstPts[dstOff++] = M10 * x;
				}
				return;
			case (APPLY_SCALE | APPLY_TRANSLATE):
				M00 = M00;
				M02 = M02;
				M11 = M11;
				M12 = M12;
				while (--numPts >= 0)
				{
					dstPts[dstOff++] = M00 * srcPts[srcOff++] + M02;
					dstPts[dstOff++] = M11 * srcPts[srcOff++] + M12;
				}
				return;
			case (APPLY_SCALE):
				M00 = M00;
				M11 = M11;
				while (--numPts >= 0)
				{
					dstPts[dstOff++] = M00 * srcPts[srcOff++];
					dstPts[dstOff++] = M11 * srcPts[srcOff++];
				}
				return;
			case (APPLY_TRANSLATE):
				M02 = M02;
				M12 = M12;
				while (--numPts >= 0)
				{
					dstPts[dstOff++] = srcPts[srcOff++] + M02;
					dstPts[dstOff++] = srcPts[srcOff++] + M12;
				}
				return;
			case (APPLY_IDENTITY):
				while (--numPts >= 0)
				{
					dstPts[dstOff++] = srcPts[srcOff++];
					dstPts[dstOff++] = srcPts[srcOff++];
				}
				return;
			}

			/* NOTREACHED */
		}

		/// <summary>
		/// Transforms an array of double precision coordinates by this transform
		/// and stores the results into an array of floats.
		/// The coordinates are stored in the arrays starting at the specified
		/// offset in the order <code>[x0, y0, x1, y1, ..., xn, yn]</code>. </summary>
		/// <param name="srcPts"> the array containing the source point coordinates.
		/// Each point is stored as a pair of x,&nbsp;y coordinates. </param>
		/// <param name="dstPts"> the array into which the transformed point
		/// coordinates are returned.  Each point is stored as a pair of
		/// x,&nbsp;y coordinates. </param>
		/// <param name="srcOff"> the offset to the first point to be transformed
		/// in the source array </param>
		/// <param name="dstOff"> the offset to the location of the first
		/// transformed point that is stored in the destination array </param>
		/// <param name="numPts"> the number of point objects to be transformed
		/// @since 1.2 </param>
		public virtual void Transform(double[] srcPts, int srcOff, float[] dstPts, int dstOff, int numPts)
		{
			double M00, M01, M02, M10, M11, M12; // For caching
			switch (State)
			{
			default:
				StateError();
				/* NOTREACHED */
				return;
			case (APPLY_SHEAR | APPLY_SCALE | APPLY_TRANSLATE):
				M00 = M00;
				M01 = M01;
				M02 = M02;
				M10 = M10;
				M11 = M11;
				M12 = M12;
				while (--numPts >= 0)
				{
					double x = srcPts[srcOff++];
					double y = srcPts[srcOff++];
					dstPts[dstOff++] = (float)(M00 * x + M01 * y + M02);
					dstPts[dstOff++] = (float)(M10 * x + M11 * y + M12);
				}
				return;
			case (APPLY_SHEAR | APPLY_SCALE):
				M00 = M00;
				M01 = M01;
				M10 = M10;
				M11 = M11;
				while (--numPts >= 0)
				{
					double x = srcPts[srcOff++];
					double y = srcPts[srcOff++];
					dstPts[dstOff++] = (float)(M00 * x + M01 * y);
					dstPts[dstOff++] = (float)(M10 * x + M11 * y);
				}
				return;
			case (APPLY_SHEAR | APPLY_TRANSLATE):
				M01 = M01;
				M02 = M02;
				M10 = M10;
				M12 = M12;
				while (--numPts >= 0)
				{
					double x = srcPts[srcOff++];
					dstPts[dstOff++] = (float)(M01 * srcPts[srcOff++] + M02);
					dstPts[dstOff++] = (float)(M10 * x + M12);
				}
				return;
			case (APPLY_SHEAR):
				M01 = M01;
				M10 = M10;
				while (--numPts >= 0)
				{
					double x = srcPts[srcOff++];
					dstPts[dstOff++] = (float)(M01 * srcPts[srcOff++]);
					dstPts[dstOff++] = (float)(M10 * x);
				}
				return;
			case (APPLY_SCALE | APPLY_TRANSLATE):
				M00 = M00;
				M02 = M02;
				M11 = M11;
				M12 = M12;
				while (--numPts >= 0)
				{
					dstPts[dstOff++] = (float)(M00 * srcPts[srcOff++] + M02);
					dstPts[dstOff++] = (float)(M11 * srcPts[srcOff++] + M12);
				}
				return;
			case (APPLY_SCALE):
				M00 = M00;
				M11 = M11;
				while (--numPts >= 0)
				{
					dstPts[dstOff++] = (float)(M00 * srcPts[srcOff++]);
					dstPts[dstOff++] = (float)(M11 * srcPts[srcOff++]);
				}
				return;
			case (APPLY_TRANSLATE):
				M02 = M02;
				M12 = M12;
				while (--numPts >= 0)
				{
					dstPts[dstOff++] = (float)(srcPts[srcOff++] + M02);
					dstPts[dstOff++] = (float)(srcPts[srcOff++] + M12);
				}
				return;
			case (APPLY_IDENTITY):
				while (--numPts >= 0)
				{
					dstPts[dstOff++] = (float)(srcPts[srcOff++]);
					dstPts[dstOff++] = (float)(srcPts[srcOff++]);
				}
				return;
			}

			/* NOTREACHED */
		}

		/// <summary>
		/// Inverse transforms the specified <code>ptSrc</code> and stores the
		/// result in <code>ptDst</code>.
		/// If <code>ptDst</code> is <code>null</code>, a new
		/// <code>Point2D</code> object is allocated and then the result of the
		/// transform is stored in this object.
		/// In either case, <code>ptDst</code>, which contains the transformed
		/// point, is returned for convenience.
		/// If <code>ptSrc</code> and <code>ptDst</code> are the same
		/// object, the input point is correctly overwritten with the
		/// transformed point. </summary>
		/// <param name="ptSrc"> the point to be inverse transformed </param>
		/// <param name="ptDst"> the resulting transformed point </param>
		/// <returns> <code>ptDst</code>, which contains the result of the
		/// inverse transform. </returns>
		/// <exception cref="NoninvertibleTransformException">  if the matrix cannot be
		///                                         inverted.
		/// @since 1.2 </exception>
//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @SuppressWarnings("fallthrough") public Point2D inverseTransform(Point2D ptSrc, Point2D ptDst) throws NoninvertibleTransformException
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
		public virtual Point2D InverseTransform(Point2D ptSrc, Point2D ptDst)
		{
			if (ptDst == null)
			{
				if (ptSrc is Point2D.Double)
				{
					ptDst = new Point2D.Double();
				}
				else
				{
					ptDst = new Point2D.Float();
				}
			}
			// Copy source coords into local variables in case src == dst
			double x = ptSrc.X;
			double y = ptSrc.Y;
			switch (State)
			{
			default:
				StateError();
				/* NOTREACHED */
				goto case (APPLY_SHEAR | APPLY_SCALE | APPLY_TRANSLATE);
			case (APPLY_SHEAR | APPLY_SCALE | APPLY_TRANSLATE):
				x -= M02;
				y -= M12;
				/* NOBREAK */
				goto case (APPLY_SHEAR | APPLY_SCALE);
			case (APPLY_SHEAR | APPLY_SCALE):
				double det = M00 * M11 - M01 * M10;
				if (System.Math.Abs(det) <= Double.Epsilon)
				{
					throw new NoninvertibleTransformException("Determinant is " + det);
				}
				ptDst.SetLocation((x * M11 - y * M01) / det, (y * M00 - x * M10) / det);
				return ptDst;
			case (APPLY_SHEAR | APPLY_TRANSLATE):
				x -= M02;
				y -= M12;
				/* NOBREAK */
				goto case (APPLY_SHEAR);
			case (APPLY_SHEAR):
				if (M01 == 0.0 || M10 == 0.0)
				{
					throw new NoninvertibleTransformException("Determinant is 0");
				}
				ptDst.SetLocation(y / M10, x / M01);
				return ptDst;
			case (APPLY_SCALE | APPLY_TRANSLATE):
				x -= M02;
				y -= M12;
				/* NOBREAK */
				goto case (APPLY_SCALE);
			case (APPLY_SCALE):
				if (M00 == 0.0 || M11 == 0.0)
				{
					throw new NoninvertibleTransformException("Determinant is 0");
				}
				ptDst.SetLocation(x / M00, y / M11);
				return ptDst;
			case (APPLY_TRANSLATE):
				ptDst.SetLocation(x - M02, y - M12);
				return ptDst;
			case (APPLY_IDENTITY):
				ptDst.SetLocation(x, y);
				return ptDst;
			}

			/* NOTREACHED */
		}

		/// <summary>
		/// Inverse transforms an array of double precision coordinates by
		/// this transform.
		/// The two coordinate array sections can be exactly the same or
		/// can be overlapping sections of the same array without affecting the
		/// validity of the results.
		/// This method ensures that no source coordinates are
		/// overwritten by a previous operation before they can be transformed.
		/// The coordinates are stored in the arrays starting at the specified
		/// offset in the order <code>[x0, y0, x1, y1, ..., xn, yn]</code>. </summary>
		/// <param name="srcPts"> the array containing the source point coordinates.
		/// Each point is stored as a pair of x,&nbsp;y coordinates. </param>
		/// <param name="dstPts"> the array into which the transformed point
		/// coordinates are returned.  Each point is stored as a pair of
		/// x,&nbsp;y coordinates. </param>
		/// <param name="srcOff"> the offset to the first point to be transformed
		/// in the source array </param>
		/// <param name="dstOff"> the offset to the location of the first
		/// transformed point that is stored in the destination array </param>
		/// <param name="numPts"> the number of point objects to be transformed </param>
		/// <exception cref="NoninvertibleTransformException">  if the matrix cannot be
		///                                         inverted.
		/// @since 1.2 </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public void inverseTransform(double[] srcPts, int srcOff, double[] dstPts, int dstOff, int numPts) throws NoninvertibleTransformException
		public virtual void InverseTransform(double[] srcPts, int srcOff, double[] dstPts, int dstOff, int numPts)
		{
			double M00, M01, M02, M10, M11, M12; // For caching
			double det;
			if (dstPts == srcPts && dstOff > srcOff && dstOff < srcOff + numPts * 2)
			{
				// If the arrays overlap partially with the destination higher
				// than the source and we transform the coordinates normally
				// we would overwrite some of the later source coordinates
				// with results of previous transformations.
				// To get around this we use arraycopy to copy the points
				// to their final destination with correct overwrite
				// handling and then transform them in place in the new
				// safer location.
				System.Array.Copy(srcPts, srcOff, dstPts, dstOff, numPts * 2);
				// srcPts = dstPts;         // They are known to be equal.
				srcOff = dstOff;
			}
			switch (State)
			{
			default:
				StateError();
				/* NOTREACHED */
				return;
			case (APPLY_SHEAR | APPLY_SCALE | APPLY_TRANSLATE):
				M00 = M00;
				M01 = M01;
				M02 = M02;
				M10 = M10;
				M11 = M11;
				M12 = M12;
				det = M00 * M11 - M01 * M10;
				if (System.Math.Abs(det) <= Double.Epsilon)
				{
					throw new NoninvertibleTransformException("Determinant is " + det);
				}
				while (--numPts >= 0)
				{
					double x = srcPts[srcOff++] - M02;
					double y = srcPts[srcOff++] - M12;
					dstPts[dstOff++] = (x * M11 - y * M01) / det;
					dstPts[dstOff++] = (y * M00 - x * M10) / det;
				}
				return;
			case (APPLY_SHEAR | APPLY_SCALE):
				M00 = M00;
				M01 = M01;
				M10 = M10;
				M11 = M11;
				det = M00 * M11 - M01 * M10;
				if (System.Math.Abs(det) <= Double.Epsilon)
				{
					throw new NoninvertibleTransformException("Determinant is " + det);
				}
				while (--numPts >= 0)
				{
					double x = srcPts[srcOff++];
					double y = srcPts[srcOff++];
					dstPts[dstOff++] = (x * M11 - y * M01) / det;
					dstPts[dstOff++] = (y * M00 - x * M10) / det;
				}
				return;
			case (APPLY_SHEAR | APPLY_TRANSLATE):
				M01 = M01;
				M02 = M02;
				M10 = M10;
				M12 = M12;
				if (M01 == 0.0 || M10 == 0.0)
				{
					throw new NoninvertibleTransformException("Determinant is 0");
				}
				while (--numPts >= 0)
				{
					double x = srcPts[srcOff++] - M02;
					dstPts[dstOff++] = (srcPts[srcOff++] - M12) / M10;
					dstPts[dstOff++] = x / M01;
				}
				return;
			case (APPLY_SHEAR):
				M01 = M01;
				M10 = M10;
				if (M01 == 0.0 || M10 == 0.0)
				{
					throw new NoninvertibleTransformException("Determinant is 0");
				}
				while (--numPts >= 0)
				{
					double x = srcPts[srcOff++];
					dstPts[dstOff++] = srcPts[srcOff++] / M10;
					dstPts[dstOff++] = x / M01;
				}
				return;
			case (APPLY_SCALE | APPLY_TRANSLATE):
				M00 = M00;
				M02 = M02;
				M11 = M11;
				M12 = M12;
				if (M00 == 0.0 || M11 == 0.0)
				{
					throw new NoninvertibleTransformException("Determinant is 0");
				}
				while (--numPts >= 0)
				{
					dstPts[dstOff++] = (srcPts[srcOff++] - M02) / M00;
					dstPts[dstOff++] = (srcPts[srcOff++] - M12) / M11;
				}
				return;
			case (APPLY_SCALE):
				M00 = M00;
				M11 = M11;
				if (M00 == 0.0 || M11 == 0.0)
				{
					throw new NoninvertibleTransformException("Determinant is 0");
				}
				while (--numPts >= 0)
				{
					dstPts[dstOff++] = srcPts[srcOff++] / M00;
					dstPts[dstOff++] = srcPts[srcOff++] / M11;
				}
				return;
			case (APPLY_TRANSLATE):
				M02 = M02;
				M12 = M12;
				while (--numPts >= 0)
				{
					dstPts[dstOff++] = srcPts[srcOff++] - M02;
					dstPts[dstOff++] = srcPts[srcOff++] - M12;
				}
				return;
			case (APPLY_IDENTITY):
				if (srcPts != dstPts || srcOff != dstOff)
				{
					System.Array.Copy(srcPts, srcOff, dstPts, dstOff, numPts * 2);
				}
				return;
			}

			/* NOTREACHED */
		}

		/// <summary>
		/// Transforms the relative distance vector specified by
		/// <code>ptSrc</code> and stores the result in <code>ptDst</code>.
		/// A relative distance vector is transformed without applying the
		/// translation components of the affine transformation matrix
		/// using the following equations:
		/// <pre>
		///  [  x' ]   [  m00  m01 (m02) ] [  x  ]   [ m00x + m01y ]
		///  [  y' ] = [  m10  m11 (m12) ] [  y  ] = [ m10x + m11y ]
		///  [ (1) ]   [  (0)  (0) ( 1 ) ] [ (1) ]   [     (1)     ]
		/// </pre>
		/// If <code>ptDst</code> is <code>null</code>, a new
		/// <code>Point2D</code> object is allocated and then the result of the
		/// transform is stored in this object.
		/// In either case, <code>ptDst</code>, which contains the
		/// transformed point, is returned for convenience.
		/// If <code>ptSrc</code> and <code>ptDst</code> are the same object,
		/// the input point is correctly overwritten with the transformed
		/// point. </summary>
		/// <param name="ptSrc"> the distance vector to be delta transformed </param>
		/// <param name="ptDst"> the resulting transformed distance vector </param>
		/// <returns> <code>ptDst</code>, which contains the result of the
		/// transformation.
		/// @since 1.2 </returns>
		public virtual Point2D DeltaTransform(Point2D ptSrc, Point2D ptDst)
		{
			if (ptDst == null)
			{
				if (ptSrc is Point2D.Double)
				{
					ptDst = new Point2D.Double();
				}
				else
				{
					ptDst = new Point2D.Float();
				}
			}
			// Copy source coords into local variables in case src == dst
			double x = ptSrc.X;
			double y = ptSrc.Y;
			switch (State)
			{
			default:
				StateError();
				/* NOTREACHED */
				return null;
			case (APPLY_SHEAR | APPLY_SCALE | APPLY_TRANSLATE):
			case (APPLY_SHEAR | APPLY_SCALE):
				ptDst.SetLocation(x * M00 + y * M01, x * M10 + y * M11);
				return ptDst;
			case (APPLY_SHEAR | APPLY_TRANSLATE):
			case (APPLY_SHEAR):
				ptDst.SetLocation(y * M01, x * M10);
				return ptDst;
			case (APPLY_SCALE | APPLY_TRANSLATE):
			case (APPLY_SCALE):
				ptDst.SetLocation(x * M00, y * M11);
				return ptDst;
			case (APPLY_TRANSLATE):
			case (APPLY_IDENTITY):
				ptDst.SetLocation(x, y);
				return ptDst;
			}

			/* NOTREACHED */
		}

		/// <summary>
		/// Transforms an array of relative distance vectors by this
		/// transform.
		/// A relative distance vector is transformed without applying the
		/// translation components of the affine transformation matrix
		/// using the following equations:
		/// <pre>
		///  [  x' ]   [  m00  m01 (m02) ] [  x  ]   [ m00x + m01y ]
		///  [  y' ] = [  m10  m11 (m12) ] [  y  ] = [ m10x + m11y ]
		///  [ (1) ]   [  (0)  (0) ( 1 ) ] [ (1) ]   [     (1)     ]
		/// </pre>
		/// The two coordinate array sections can be exactly the same or
		/// can be overlapping sections of the same array without affecting the
		/// validity of the results.
		/// This method ensures that no source coordinates are
		/// overwritten by a previous operation before they can be transformed.
		/// The coordinates are stored in the arrays starting at the indicated
		/// offset in the order <code>[x0, y0, x1, y1, ..., xn, yn]</code>. </summary>
		/// <param name="srcPts"> the array containing the source distance vectors.
		/// Each vector is stored as a pair of relative x,&nbsp;y coordinates. </param>
		/// <param name="dstPts"> the array into which the transformed distance vectors
		/// are returned.  Each vector is stored as a pair of relative
		/// x,&nbsp;y coordinates. </param>
		/// <param name="srcOff"> the offset to the first vector to be transformed
		/// in the source array </param>
		/// <param name="dstOff"> the offset to the location of the first
		/// transformed vector that is stored in the destination array </param>
		/// <param name="numPts"> the number of vector coordinate pairs to be
		/// transformed
		/// @since 1.2 </param>
		public virtual void DeltaTransform(double[] srcPts, int srcOff, double[] dstPts, int dstOff, int numPts)
		{
			double M00, M01, M10, M11; // For caching
			if (dstPts == srcPts && dstOff > srcOff && dstOff < srcOff + numPts * 2)
			{
				// If the arrays overlap partially with the destination higher
				// than the source and we transform the coordinates normally
				// we would overwrite some of the later source coordinates
				// with results of previous transformations.
				// To get around this we use arraycopy to copy the points
				// to their final destination with correct overwrite
				// handling and then transform them in place in the new
				// safer location.
				System.Array.Copy(srcPts, srcOff, dstPts, dstOff, numPts * 2);
				// srcPts = dstPts;         // They are known to be equal.
				srcOff = dstOff;
			}
			switch (State)
			{
			default:
				StateError();
				/* NOTREACHED */
				return;
			case (APPLY_SHEAR | APPLY_SCALE | APPLY_TRANSLATE):
			case (APPLY_SHEAR | APPLY_SCALE):
				M00 = M00;
				M01 = M01;
				M10 = M10;
				M11 = M11;
				while (--numPts >= 0)
				{
					double x = srcPts[srcOff++];
					double y = srcPts[srcOff++];
					dstPts[dstOff++] = x * M00 + y * M01;
					dstPts[dstOff++] = x * M10 + y * M11;
				}
				return;
			case (APPLY_SHEAR | APPLY_TRANSLATE):
			case (APPLY_SHEAR):
				M01 = M01;
				M10 = M10;
				while (--numPts >= 0)
				{
					double x = srcPts[srcOff++];
					dstPts[dstOff++] = srcPts[srcOff++] * M01;
					dstPts[dstOff++] = x * M10;
				}
				return;
			case (APPLY_SCALE | APPLY_TRANSLATE):
			case (APPLY_SCALE):
				M00 = M00;
				M11 = M11;
				while (--numPts >= 0)
				{
					dstPts[dstOff++] = srcPts[srcOff++] * M00;
					dstPts[dstOff++] = srcPts[srcOff++] * M11;
				}
				return;
			case (APPLY_TRANSLATE):
			case (APPLY_IDENTITY):
				if (srcPts != dstPts || srcOff != dstOff)
				{
					System.Array.Copy(srcPts, srcOff, dstPts, dstOff, numPts * 2);
				}
				return;
			}

			/* NOTREACHED */
		}

		/// <summary>
		/// Returns a new <seealso cref="Shape"/> object defined by the geometry of the
		/// specified <code>Shape</code> after it has been transformed by
		/// this transform. </summary>
		/// <param name="pSrc"> the specified <code>Shape</code> object to be
		/// transformed by this transform. </param>
		/// <returns> a new <code>Shape</code> object that defines the geometry
		/// of the transformed <code>Shape</code>, or null if {@code pSrc} is null.
		/// @since 1.2 </returns>
		public virtual Shape CreateTransformedShape(Shape pSrc)
		{
			if (pSrc == null)
			{
				return null;
			}
			return new Path2D.Double(pSrc, this);
		}

		// Round values to sane precision for printing
		// Note that Math.sin(Math.PI) has an error of about 10^-16
		private static double _matround(double matval)
		{
			return Math.Rint(matval * 1E15) / 1E15;
		}

		/// <summary>
		/// Returns a <code>String</code> that represents the value of this
		/// <seealso cref="Object"/>. </summary>
		/// <returns> a <code>String</code> representing the value of this
		/// <code>Object</code>.
		/// @since 1.2 </returns>
		public override String ToString()
		{
			return ("AffineTransform[[" + _matround(M00) + ", " + _matround(M01) + ", " + _matround(M02) + "], [" + _matround(M10) + ", " + _matround(M11) + ", " + _matround(M12) + "]]");
		}

		/// <summary>
		/// Returns <code>true</code> if this <code>AffineTransform</code> is
		/// an identity transform. </summary>
		/// <returns> <code>true</code> if this <code>AffineTransform</code> is
		/// an identity transform; <code>false</code> otherwise.
		/// @since 1.2 </returns>
		public virtual bool Identity
		{
			get
			{
				return (State == APPLY_IDENTITY || (Type == TYPE_IDENTITY));
			}
		}

		/// <summary>
		/// Returns a copy of this <code>AffineTransform</code> object. </summary>
		/// <returns> an <code>Object</code> that is a copy of this
		/// <code>AffineTransform</code> object.
		/// @since 1.2 </returns>
		public virtual Object Clone()
		{
			try
			{
				return base.Clone();
			}
			catch (CloneNotSupportedException e)
			{
				// this shouldn't happen, since we are Cloneable
				throw new InternalError(e);
			}
		}

		/// <summary>
		/// Returns the hashcode for this transform. </summary>
		/// <returns>      a hash code for this transform.
		/// @since 1.2 </returns>
		public override int HashCode()
		{
			long bits = Double.DoubleToLongBits(M00);
			bits = bits * 31 + Double.DoubleToLongBits(M01);
			bits = bits * 31 + Double.DoubleToLongBits(M02);
			bits = bits * 31 + Double.DoubleToLongBits(M10);
			bits = bits * 31 + Double.DoubleToLongBits(M11);
			bits = bits * 31 + Double.DoubleToLongBits(M12);
			return (((int) bits) ^ ((int)(bits >> 32)));
		}

		/// <summary>
		/// Returns <code>true</code> if this <code>AffineTransform</code>
		/// represents the same affine coordinate transform as the specified
		/// argument. </summary>
		/// <param name="obj"> the <code>Object</code> to test for equality with this
		/// <code>AffineTransform</code> </param>
		/// <returns> <code>true</code> if <code>obj</code> equals this
		/// <code>AffineTransform</code> object; <code>false</code> otherwise.
		/// @since 1.2 </returns>
		public override bool Equals(Object obj)
		{
			if (!(obj is AffineTransform))
			{
				return false;
			}

			AffineTransform a = (AffineTransform)obj;

			return ((M00 == a.M00) && (M01 == a.M01) && (M02 == a.M02) && (M10 == a.M10) && (M11 == a.M11) && (M12 == a.M12));
		}

		/* Serialization support.  A readObject method is neccessary because
		 * the state field is part of the implementation of this particular
		 * AffineTransform and not part of the public specification.  The
		 * state variable's value needs to be recalculated on the fly by the
		 * readObject method as it is in the 6-argument matrix constructor.
		 */

		/*
		 * JDK 1.2 serialVersionUID
		 */
		private const long SerialVersionUID = 1330973210523860834L;

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: private void writeObject(java.io.ObjectOutputStream s) throws java.lang.ClassNotFoundException, java.io.IOException
		private void WriteObject(java.io.ObjectOutputStream s)
		{
			s.DefaultWriteObject();
		}

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: private void readObject(java.io.ObjectInputStream s) throws java.lang.ClassNotFoundException, java.io.IOException
		private void ReadObject(java.io.ObjectInputStream s)
		{
			s.DefaultReadObject();
			UpdateState();
		}
	}

}
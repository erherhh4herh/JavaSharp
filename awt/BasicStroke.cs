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

namespace java.awt
{


	/// <summary>
	/// The <code>BasicStroke</code> class defines a basic set of rendering
	/// attributes for the outlines of graphics primitives, which are rendered
	/// with a <seealso cref="Graphics2D"/> object that has its Stroke attribute set to
	/// this <code>BasicStroke</code>.
	/// The rendering attributes defined by <code>BasicStroke</code> describe
	/// the shape of the mark made by a pen drawn along the outline of a
	/// <seealso cref="Shape"/> and the decorations applied at the ends and joins of
	/// path segments of the <code>Shape</code>.
	/// These rendering attributes include:
	/// <dl>
	/// <dt><i>width</i>
	/// <dd>The pen width, measured perpendicularly to the pen trajectory.
	/// <dt><i>end caps</i>
	/// <dd>The decoration applied to the ends of unclosed subpaths and
	/// dash segments.  Subpaths that start and end on the same point are
	/// still considered unclosed if they do not have a CLOSE segment.
	/// See <seealso cref="java.awt.geom.PathIterator#SEG_CLOSE SEG_CLOSE"/>
	/// for more information on the CLOSE segment.
	/// The three different decorations are: <seealso cref="#CAP_BUTT"/>,
	/// <seealso cref="#CAP_ROUND"/>, and <seealso cref="#CAP_SQUARE"/>.
	/// <dt><i>line joins</i>
	/// <dd>The decoration applied at the intersection of two path segments
	/// and at the intersection of the endpoints of a subpath that is closed
	/// using <seealso cref="java.awt.geom.PathIterator#SEG_CLOSE SEG_CLOSE"/>.
	/// The three different decorations are: <seealso cref="#JOIN_BEVEL"/>,
	/// <seealso cref="#JOIN_MITER"/>, and <seealso cref="#JOIN_ROUND"/>.
	/// <dt><i>miter limit</i>
	/// <dd>The limit to trim a line join that has a JOIN_MITER decoration.
	/// A line join is trimmed when the ratio of miter length to stroke
	/// width is greater than the miterlimit value.  The miter length is
	/// the diagonal length of the miter, which is the distance between
	/// the inside corner and the outside corner of the intersection.
	/// The smaller the angle formed by two line segments, the longer
	/// the miter length and the sharper the angle of intersection.  The
	/// default miterlimit value of 10.0f causes all angles less than
	/// 11 degrees to be trimmed.  Trimming miters converts
	/// the decoration of the line join to bevel.
	/// <dt><i>dash attributes</i>
	/// <dd>The definition of how to make a dash pattern by alternating
	/// between opaque and transparent sections.
	/// </dl>
	/// All attributes that specify measurements and distances controlling
	/// the shape of the returned outline are measured in the same
	/// coordinate system as the original unstroked <code>Shape</code>
	/// argument.  When a <code>Graphics2D</code> object uses a
	/// <code>Stroke</code> object to redefine a path during the execution
	/// of one of its <code>draw</code> methods, the geometry is supplied
	/// in its original form before the <code>Graphics2D</code> transform
	/// attribute is applied.  Therefore, attributes such as the pen width
	/// are interpreted in the user space coordinate system of the
	/// <code>Graphics2D</code> object and are subject to the scaling and
	/// shearing effects of the user-space-to-device-space transform in that
	/// particular <code>Graphics2D</code>.
	/// For example, the width of a rendered shape's outline is determined
	/// not only by the width attribute of this <code>BasicStroke</code>,
	/// but also by the transform attribute of the
	/// <code>Graphics2D</code> object.  Consider this code:
	/// <blockquote><tt>
	///      // sets the Graphics2D object's Transform attribute
	///      g2d.scale(10, 10);
	///      // sets the Graphics2D object's Stroke attribute
	///      g2d.setStroke(new BasicStroke(1.5f));
	/// </tt></blockquote>
	/// Assuming there are no other scaling transforms added to the
	/// <code>Graphics2D</code> object, the resulting line
	/// will be approximately 15 pixels wide.
	/// As the example code demonstrates, a floating-point line
	/// offers better precision, especially when large transforms are
	/// used with a <code>Graphics2D</code> object.
	/// When a line is diagonal, the exact width depends on how the
	/// rendering pipeline chooses which pixels to fill as it traces the
	/// theoretical widened outline.  The choice of which pixels to turn
	/// on is affected by the antialiasing attribute because the
	/// antialiasing rendering pipeline can choose to color
	/// partially-covered pixels.
	/// <para>
	/// For more information on the user space coordinate system and the
	/// rendering process, see the <code>Graphics2D</code> class comments.
	/// </para>
	/// </summary>
	/// <seealso cref= Graphics2D
	/// @author Jim Graham </seealso>
	public class BasicStroke : Stroke
	{

		/// <summary>
		/// Joins path segments by extending their outside edges until
		/// they meet.
		/// </summary>
//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Native public final static int JOIN_MITER = 0;
		public const int JOIN_MITER = 0;

		/// <summary>
		/// Joins path segments by rounding off the corner at a radius
		/// of half the line width.
		/// </summary>
//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Native public final static int JOIN_ROUND = 1;
		public const int JOIN_ROUND = 1;

		/// <summary>
		/// Joins path segments by connecting the outer corners of their
		/// wide outlines with a straight segment.
		/// </summary>
//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Native public final static int JOIN_BEVEL = 2;
		public const int JOIN_BEVEL = 2;

		/// <summary>
		/// Ends unclosed subpaths and dash segments with no added
		/// decoration.
		/// </summary>
//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Native public final static int CAP_BUTT = 0;
		public const int CAP_BUTT = 0;

		/// <summary>
		/// Ends unclosed subpaths and dash segments with a round
		/// decoration that has a radius equal to half of the width
		/// of the pen.
		/// </summary>
//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Native public final static int CAP_ROUND = 1;
		public const int CAP_ROUND = 1;

		/// <summary>
		/// Ends unclosed subpaths and dash segments with a square
		/// projection that extends beyond the end of the segment
		/// to a distance equal to half of the line width.
		/// </summary>
//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Native public final static int CAP_SQUARE = 2;
		public const int CAP_SQUARE = 2;

		internal float Width;

		internal int Join;
		internal int Cap;
		internal float Miterlimit;

		internal float[] Dash;
		internal float Dash_phase;

		/// <summary>
		/// Constructs a new <code>BasicStroke</code> with the specified
		/// attributes. </summary>
		/// <param name="width"> the width of this <code>BasicStroke</code>.  The
		///         width must be greater than or equal to 0.0f.  If width is
		///         set to 0.0f, the stroke is rendered as the thinnest
		///         possible line for the target device and the antialias
		///         hint setting. </param>
		/// <param name="cap"> the decoration of the ends of a <code>BasicStroke</code> </param>
		/// <param name="join"> the decoration applied where path segments meet </param>
		/// <param name="miterlimit"> the limit to trim the miter join.  The miterlimit
		///        must be greater than or equal to 1.0f. </param>
		/// <param name="dash"> the array representing the dashing pattern </param>
		/// <param name="dash_phase"> the offset to start the dashing pattern </param>
		/// <exception cref="IllegalArgumentException"> if <code>width</code> is negative </exception>
		/// <exception cref="IllegalArgumentException"> if <code>cap</code> is not either
		///         CAP_BUTT, CAP_ROUND or CAP_SQUARE </exception>
		/// <exception cref="IllegalArgumentException"> if <code>miterlimit</code> is less
		///         than 1 and <code>join</code> is JOIN_MITER </exception>
		/// <exception cref="IllegalArgumentException"> if <code>join</code> is not
		///         either JOIN_ROUND, JOIN_BEVEL, or JOIN_MITER </exception>
		/// <exception cref="IllegalArgumentException"> if <code>dash_phase</code>
		///         is negative and <code>dash</code> is not <code>null</code> </exception>
		/// <exception cref="IllegalArgumentException"> if the length of
		///         <code>dash</code> is zero </exception>
		/// <exception cref="IllegalArgumentException"> if dash lengths are all zero. </exception>
//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @ConstructorProperties({ "lineWidth", "endCap", "lineJoin", "miterLimit", "dashArray", "dashPhase" }) public BasicStroke(float width, int cap, int join, float miterlimit, float dash[] , float dash_phase)
		public BasicStroke(float width, int cap, int join, float miterlimit, float[] dash, float dash_phase)
		{
			if (width < 0.0f)
			{
				throw new IllegalArgumentException("negative width");
			}
			if (cap != CAP_BUTT && cap != CAP_ROUND && cap != CAP_SQUARE)
			{
				throw new IllegalArgumentException("illegal end cap value");
			}
			if (join == JOIN_MITER)
			{
				if (miterlimit < 1.0f)
				{
					throw new IllegalArgumentException("miter limit < 1");
				}
			}
			else if (join != JOIN_ROUND && join != JOIN_BEVEL)
			{
				throw new IllegalArgumentException("illegal line join value");
			}
			if (dash != null)
			{
				if (dash_phase < 0.0f)
				{
					throw new IllegalArgumentException("negative dash phase");
				}
				bool allzero = true;
				for (int i = 0; i < dash.Length; i++)
				{
					float d = dash[i];
					if (d > 0.0)
					{
						allzero = false;
					}
					else if (d < 0.0)
					{
						throw new IllegalArgumentException("negative dash length");
					}
				}
				if (allzero)
				{
					throw new IllegalArgumentException("dash lengths all zero");
				}
			}
			this.Width = width;
			this.Cap = cap;
			this.Join = join;
			this.Miterlimit = miterlimit;
			if (dash != null)
			{
				this.Dash = (float []) dash.clone();
			}
			this.Dash_phase = dash_phase;
		}

		/// <summary>
		/// Constructs a solid <code>BasicStroke</code> with the specified
		/// attributes. </summary>
		/// <param name="width"> the width of the <code>BasicStroke</code> </param>
		/// <param name="cap"> the decoration of the ends of a <code>BasicStroke</code> </param>
		/// <param name="join"> the decoration applied where path segments meet </param>
		/// <param name="miterlimit"> the limit to trim the miter join </param>
		/// <exception cref="IllegalArgumentException"> if <code>width</code> is negative </exception>
		/// <exception cref="IllegalArgumentException"> if <code>cap</code> is not either
		///         CAP_BUTT, CAP_ROUND or CAP_SQUARE </exception>
		/// <exception cref="IllegalArgumentException"> if <code>miterlimit</code> is less
		///         than 1 and <code>join</code> is JOIN_MITER </exception>
		/// <exception cref="IllegalArgumentException"> if <code>join</code> is not
		///         either JOIN_ROUND, JOIN_BEVEL, or JOIN_MITER </exception>
		public BasicStroke(float width, int cap, int join, float miterlimit) : this(width, cap, join, miterlimit, null, 0.0f)
		{
		}

		/// <summary>
		/// Constructs a solid <code>BasicStroke</code> with the specified
		/// attributes.  The <code>miterlimit</code> parameter is
		/// unnecessary in cases where the default is allowable or the
		/// line joins are not specified as JOIN_MITER. </summary>
		/// <param name="width"> the width of the <code>BasicStroke</code> </param>
		/// <param name="cap"> the decoration of the ends of a <code>BasicStroke</code> </param>
		/// <param name="join"> the decoration applied where path segments meet </param>
		/// <exception cref="IllegalArgumentException"> if <code>width</code> is negative </exception>
		/// <exception cref="IllegalArgumentException"> if <code>cap</code> is not either
		///         CAP_BUTT, CAP_ROUND or CAP_SQUARE </exception>
		/// <exception cref="IllegalArgumentException"> if <code>join</code> is not
		///         either JOIN_ROUND, JOIN_BEVEL, or JOIN_MITER </exception>
		public BasicStroke(float width, int cap, int join) : this(width, cap, join, 10.0f, null, 0.0f)
		{
		}

		/// <summary>
		/// Constructs a solid <code>BasicStroke</code> with the specified
		/// line width and with default values for the cap and join
		/// styles. </summary>
		/// <param name="width"> the width of the <code>BasicStroke</code> </param>
		/// <exception cref="IllegalArgumentException"> if <code>width</code> is negative </exception>
		public BasicStroke(float width) : this(width, CAP_SQUARE, JOIN_MITER, 10.0f, null, 0.0f)
		{
		}

		/// <summary>
		/// Constructs a new <code>BasicStroke</code> with defaults for all
		/// attributes.
		/// The default attributes are a solid line of width 1.0, CAP_SQUARE,
		/// JOIN_MITER, a miter limit of 10.0.
		/// </summary>
		public BasicStroke() : this(1.0f, CAP_SQUARE, JOIN_MITER, 10.0f, null, 0.0f)
		{
		}


		/// <summary>
		/// Returns a <code>Shape</code> whose interior defines the
		/// stroked outline of a specified <code>Shape</code>. </summary>
		/// <param name="s"> the <code>Shape</code> boundary be stroked </param>
		/// <returns> the <code>Shape</code> of the stroked outline. </returns>
		public virtual Shape CreateStrokedShape(Shape s)
		{
			sun.java2d.pipe.RenderingEngine re = sun.java2d.pipe.RenderingEngine.Instance;
			return re.createStrokedShape(s, Width, Cap, Join, Miterlimit, Dash, Dash_phase);
		}

		/// <summary>
		/// Returns the line width.  Line width is represented in user space,
		/// which is the default-coordinate space used by Java 2D.  See the
		/// <code>Graphics2D</code> class comments for more information on
		/// the user space coordinate system. </summary>
		/// <returns> the line width of this <code>BasicStroke</code>. </returns>
		/// <seealso cref= Graphics2D </seealso>
		public virtual float LineWidth
		{
			get
			{
				return Width;
			}
		}

		/// <summary>
		/// Returns the end cap style. </summary>
		/// <returns> the end cap style of this <code>BasicStroke</code> as one
		/// of the static <code>int</code> values that define possible end cap
		/// styles. </returns>
		public virtual int EndCap
		{
			get
			{
				return Cap;
			}
		}

		/// <summary>
		/// Returns the line join style. </summary>
		/// <returns> the line join style of the <code>BasicStroke</code> as one
		/// of the static <code>int</code> values that define possible line
		/// join styles. </returns>
		public virtual int LineJoin
		{
			get
			{
				return Join;
			}
		}

		/// <summary>
		/// Returns the limit of miter joins. </summary>
		/// <returns> the limit of miter joins of the <code>BasicStroke</code>. </returns>
		public virtual float MiterLimit
		{
			get
			{
				return Miterlimit;
			}
		}

		/// <summary>
		/// Returns the array representing the lengths of the dash segments.
		/// Alternate entries in the array represent the user space lengths
		/// of the opaque and transparent segments of the dashes.
		/// As the pen moves along the outline of the <code>Shape</code>
		/// to be stroked, the user space
		/// distance that the pen travels is accumulated.  The distance
		/// value is used to index into the dash array.
		/// The pen is opaque when its current cumulative distance maps
		/// to an even element of the dash array and transparent otherwise. </summary>
		/// <returns> the dash array. </returns>
		public virtual float[] DashArray
		{
			get
			{
				if (Dash == null)
				{
					return null;
				}
    
				return (float[]) Dash.clone();
			}
		}

		/// <summary>
		/// Returns the current dash phase.
		/// The dash phase is a distance specified in user coordinates that
		/// represents an offset into the dashing pattern. In other words, the dash
		/// phase defines the point in the dashing pattern that will correspond to
		/// the beginning of the stroke. </summary>
		/// <returns> the dash phase as a <code>float</code> value. </returns>
		public virtual float DashPhase
		{
			get
			{
				return Dash_phase;
			}
		}

		/// <summary>
		/// Returns the hashcode for this stroke. </summary>
		/// <returns>      a hash code for this stroke. </returns>
		public override int HashCode()
		{
			int hash = Float.FloatToIntBits(Width);
			hash = hash * 31 + Join;
			hash = hash * 31 + Cap;
			hash = hash * 31 + Float.FloatToIntBits(Miterlimit);
			if (Dash != null)
			{
				hash = hash * 31 + Float.FloatToIntBits(Dash_phase);
				for (int i = 0; i < Dash.Length; i++)
				{
					hash = hash * 31 + Float.FloatToIntBits(Dash[i]);
				}
			}
			return hash;
		}

		/// <summary>
		/// Returns true if this BasicStroke represents the same
		/// stroking operation as the given argument.
		/// </summary>
	   /// <summary>
	   /// Tests if a specified object is equal to this <code>BasicStroke</code>
	   /// by first testing if it is a <code>BasicStroke</code> and then comparing
	   /// its width, join, cap, miter limit, dash, and dash phase attributes with
	   /// those of this <code>BasicStroke</code>. </summary>
	   /// <param name="obj"> the specified object to compare to this
	   ///              <code>BasicStroke</code> </param>
	   /// <returns> <code>true</code> if the width, join, cap, miter limit, dash, and
	   ///            dash phase are the same for both objects;
	   ///            <code>false</code> otherwise. </returns>
		public override bool Equals(Object obj)
		{
			if (!(obj is BasicStroke))
			{
				return false;
			}

			BasicStroke bs = (BasicStroke) obj;
			if (Width != bs.Width)
			{
				return false;
			}

			if (Join != bs.Join)
			{
				return false;
			}

			if (Cap != bs.Cap)
			{
				return false;
			}

			if (Miterlimit != bs.Miterlimit)
			{
				return false;
			}

			if (Dash != null)
			{
				if (Dash_phase != bs.Dash_phase)
				{
					return false;
				}

				if (!System.Array.Equals(Dash, bs.Dash))
				{
					return false;
				}
			}
			else if (bs.Dash != null)
			{
				return false;
			}

			return true;
		}
	}

}
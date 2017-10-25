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

namespace java.awt
{


	/// <summary>
	/// The interface for objects which have an adjustable numeric value
	/// contained within a bounded range of values.
	/// 
	/// @author Amy Fowler
	/// @author Tim Prinzing
	/// </summary>
	public interface Adjustable
	{

		/// <summary>
		/// Indicates that the <code>Adjustable</code> has horizontal orientation.
		/// </summary>

		/// <summary>
		/// Indicates that the <code>Adjustable</code> has vertical orientation.
		/// </summary>

		/// <summary>
		/// Indicates that the <code>Adjustable</code> has no orientation.
		/// </summary>

		/// <summary>
		/// Gets the orientation of the adjustable object. </summary>
		/// <returns> the orientation of the adjustable object;
		///   either <code>HORIZONTAL</code>, <code>VERTICAL</code>,
		///   or <code>NO_ORIENTATION</code> </returns>
		int Orientation {get;}

		/// <summary>
		/// Sets the minimum value of the adjustable object. </summary>
		/// <param name="min"> the minimum value </param>
		int Minimum {set;get;}


		/// <summary>
		/// Sets the maximum value of the adjustable object. </summary>
		/// <param name="max"> the maximum value </param>
		int Maximum {set;get;}


		/// <summary>
		/// Sets the unit value increment for the adjustable object. </summary>
		/// <param name="u"> the unit increment </param>
		int UnitIncrement {set;get;}


		/// <summary>
		/// Sets the block value increment for the adjustable object. </summary>
		/// <param name="b"> the block increment </param>
		int BlockIncrement {set;get;}


		/// <summary>
		/// Sets the length of the proportional indicator of the
		/// adjustable object. </summary>
		/// <param name="v"> the length of the indicator </param>
		int VisibleAmount {set;get;}


		/// <summary>
		/// Sets the current value of the adjustable object. If
		/// the value supplied is less than <code>minimum</code>
		/// or greater than <code>maximum</code> - <code>visibleAmount</code>,
		/// then one of those values is substituted, as appropriate.
		/// <para>
		/// Calling this method does not fire an
		/// <code>AdjustmentEvent</code>.
		/// 
		/// </para>
		/// </summary>
		/// <param name="v"> the current value, between <code>minimum</code>
		///    and <code>maximum</code> - <code>visibleAmount</code> </param>
		int Value {set;get;}


		/// <summary>
		/// Adds a listener to receive adjustment events when the value of
		/// the adjustable object changes. </summary>
		/// <param name="l"> the listener to receive events </param>
		/// <seealso cref= AdjustmentEvent </seealso>
		void AddAdjustmentListener(AdjustmentListener l);

		/// <summary>
		/// Removes an adjustment listener. </summary>
		/// <param name="l"> the listener being removed </param>
		/// <seealso cref= AdjustmentEvent </seealso>
		void RemoveAdjustmentListener(AdjustmentListener l);

	}

	public static class Adjustable_Fields
	{
//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Native public static final int HORIZONTAL = 0;
		public const int HORIZONTAL = 0;
//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Native public static final int VERTICAL = 1;
		public const int VERTICAL = 1;
//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Native public static final int NO_ORIENTATION = 2;
		public const int NO_ORIENTATION = 2;
	}

}
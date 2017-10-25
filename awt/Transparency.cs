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
	/// The <code>Transparency</code> interface defines the common transparency
	/// modes for implementing classes.
	/// </summary>
	public interface Transparency
	{

		/// <summary>
		/// Represents image data that is guaranteed to be completely opaque,
		/// meaning that all pixels have an alpha value of 1.0.
		/// </summary>

		/// <summary>
		/// Represents image data that is guaranteed to be either completely
		/// opaque, with an alpha value of 1.0, or completely transparent,
		/// with an alpha value of 0.0.
		/// </summary>

		/// <summary>
		/// Represents image data that contains or might contain arbitrary
		/// alpha values between and including 0.0 and 1.0.
		/// </summary>

		/// <summary>
		/// Returns the type of this <code>Transparency</code>. </summary>
		/// <returns> the field type of this <code>Transparency</code>, which is
		///          either OPAQUE, BITMASK or TRANSLUCENT. </returns>
		int Transparency {get;}
	}

	public static class Transparency_Fields
	{
//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Native public final static int OPAQUE = 1;
		public const int OPAQUE = 1;
//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Native public final static int BITMASK = 2;
		public const int BITMASK = 2;
//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Native public final static int TRANSLUCENT = 3;
		public const int TRANSLUCENT = 3;
	}

}
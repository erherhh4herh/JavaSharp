/*
 * Copyright (c) 1996, 1997, Oracle and/or its affiliates. All rights reserved.
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
	/// An abstract class which provides a print graphics context for a page.
	/// 
	/// @author      Amy Fowler
	/// </summary>
	public interface PrintGraphics
	{

		/// <summary>
		/// Returns the PrintJob object from which this PrintGraphics
		/// object originated.
		/// </summary>
		PrintJob PrintJob {get;}

	}

}
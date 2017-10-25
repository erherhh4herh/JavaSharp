using System;

/*
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

/*
    Created by gbp, October 25, 1997

 *
 */
/*
 **********************************************************************
 **********************************************************************
 **********************************************************************
 *** COPYRIGHT (c) Eastman Kodak Company, 1997                      ***
 *** As  an unpublished  work pursuant to Title 17 of the United    ***
 *** States Code.  All rights reserved.                             ***
 **********************************************************************
 **********************************************************************
 **********************************************************************/


namespace java.awt.color
{


	/// <summary>
	/// This exception is thrown if the native CMM returns an error.
	/// </summary>

	public class CMMException : Exception
	{

		/// <summary>
		///  Constructs a CMMException with the specified detail message. </summary>
		///  <param name="s"> the specified detail message </param>
		public CMMException(String s) : base(s)
		{
		}
	}

}
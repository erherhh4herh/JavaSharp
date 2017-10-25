/*
 * Copyright (c) 2007, 2009, Oracle and/or its affiliates. All rights reserved.
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

namespace java.nio.file
{

	/// <summary>
	/// Unchecked exception thrown when an attempt is made to invoke an operation on
	/// a watch service that is closed.
	/// </summary>

	public class ClosedWatchServiceException : IllegalStateException
	{
		internal new const long SerialVersionUID = 1853336266231677732L;

		/// <summary>
		/// Constructs an instance of this class.
		/// </summary>
		public ClosedWatchServiceException()
		{
		}
	}

}
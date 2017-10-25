using System;

/*
 * Copyright (c) 2000, Oracle and/or its affiliates. All rights reserved.
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
namespace java.beans
{

	/// <summary>
	/// An ExceptionListener is notified of internal exceptions.
	/// 
	/// @since 1.4
	/// 
	/// @author Philip Milne
	/// </summary>
	public interface ExceptionListener
	{
		/// <summary>
		/// This method is called when a recoverable exception has
		/// been caught.
		/// </summary>
		/// <param name="e"> The exception that was caught.
		///  </param>
		void ExceptionThrown(Exception e);
	}

}
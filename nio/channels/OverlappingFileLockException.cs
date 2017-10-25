/*
 * Copyright (c) 2000, 2007, Oracle and/or its affiliates. All rights reserved.
 *
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
 *
 */

// -- This file was mechanically generated: Do not edit! -- //

namespace java.nio.channels
{


	/// <summary>
	/// Unchecked exception thrown when an attempt is made to acquire a lock on a
	/// region of a file that overlaps a region already locked by the same Java
	/// virtual machine, or when another thread is already waiting to lock an
	/// overlapping region of the same file.
	/// 
	/// @since 1.4
	/// </summary>

	public class OverlappingFileLockException : IllegalStateException
	{

		private new const long SerialVersionUID = 2047812138163068433L;

		/// <summary>
		/// Constructs an instance of this class.
		/// </summary>
		public OverlappingFileLockException()
		{
		}

	}

}
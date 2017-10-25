/*
 * Copyright (c) 1998, 2013, Oracle and/or its affiliates. All rights reserved.
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

namespace java.io
{


	/// <summary>
	/// A filter for abstract pathnames.
	/// 
	/// <para> Instances of this interface may be passed to the <code>{@link
	/// File#listFiles(java.io.FileFilter) listFiles(FileFilter)}</code> method
	/// of the <code><seealso cref="java.io.File"/></code> class.
	/// 
	/// @since 1.2
	/// </para>
	/// </summary>
//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @FunctionalInterface public interface FileFilter
	public interface FileFilter
	{

		/// <summary>
		/// Tests whether or not the specified abstract pathname should be
		/// included in a pathname list.
		/// </summary>
		/// <param name="pathname">  The abstract pathname to be tested </param>
		/// <returns>  <code>true</code> if and only if <code>pathname</code>
		///          should be included </returns>
		bool Accept(File pathname);
	}

}
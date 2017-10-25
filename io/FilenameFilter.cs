/*
 * Copyright (c) 1994, 2013, Oracle and/or its affiliates. All rights reserved.
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
	/// Instances of classes that implement this interface are used to
	/// filter filenames. These instances are used to filter directory
	/// listings in the <code>list</code> method of class
	/// <code>File</code>, and by the Abstract Window Toolkit's file
	/// dialog component.
	/// 
	/// @author  Arthur van Hoff
	/// @author  Jonathan Payne </summary>
	/// <seealso cref=     java.awt.FileDialog#setFilenameFilter(java.io.FilenameFilter) </seealso>
	/// <seealso cref=     java.io.File </seealso>
	/// <seealso cref=     java.io.File#list(java.io.FilenameFilter)
	/// @since   JDK1.0 </seealso>
//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @FunctionalInterface public interface FilenameFilter
	public interface FilenameFilter
	{
		/// <summary>
		/// Tests if a specified file should be included in a file list.
		/// </summary>
		/// <param name="dir">    the directory in which the file was found. </param>
		/// <param name="name">   the name of the file. </param>
		/// <returns>  <code>true</code> if and only if the name should be
		/// included in the file list; <code>false</code> otherwise. </returns>
		bool Accept(File dir, String name);
	}

}
/*
 * Copyright (c) 2007, 2011, Oracle and/or its affiliates. All rights reserved.
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

namespace java.nio.file.spi
{


	/// <summary>
	/// A file type detector for probing a file to guess its file type.
	/// 
	/// <para> A file type detector is a concrete implementation of this class, has a
	/// zero-argument constructor, and implements the abstract methods specified
	/// below.
	/// 
	/// </para>
	/// <para> The means by which a file type detector determines the file type is
	/// highly implementation specific. A simple implementation might examine the
	/// <em>file extension</em> (a convention used in some platforms) and map it to
	/// a file type. In other cases, the file type may be stored as a file <a
	/// href="../attribute/package-summary.html"> attribute</a> or the bytes in a
	/// file may be examined to guess its file type.
	/// 
	/// </para>
	/// </summary>
	/// <seealso cref= java.nio.file.Files#probeContentType(Path)
	/// 
	/// @since 1.7 </seealso>

	public abstract class FileTypeDetector
	{

		private static Void CheckPermission()
		{
			SecurityManager sm = System.SecurityManager;
			if (sm != null)
			{
				sm.CheckPermission(new RuntimePermission("fileTypeDetector"));
			}
			return null;
		}
		private FileTypeDetector(Void ignore)
		{
		}

		/// <summary>
		/// Initializes a new instance of this class.
		/// </summary>
		/// <exception cref="SecurityException">
		///          If a security manager has been installed and it denies
		///          <seealso cref="RuntimePermission"/><tt>("fileTypeDetector")</tt> </exception>
		protected internal FileTypeDetector() : this(CheckPermission())
		{
		}

		/// <summary>
		/// Probes the given file to guess its content type.
		/// 
		/// <para> The means by which this method determines the file type is highly
		/// implementation specific. It may simply examine the file name, it may use
		/// a file <a href="../attribute/package-summary.html">attribute</a>,
		/// or it may examines bytes in the file.
		/// 
		/// </para>
		/// <para> The probe result is the string form of the value of a
		/// Multipurpose Internet Mail Extension (MIME) content type as
		/// defined by <a href="http://www.ietf.org/rfc/rfc2045.txt"><i>RFC&nbsp;2045:
		/// Multipurpose Internet Mail Extensions (MIME) Part One: Format of Internet
		/// Message Bodies</i></a>. The string must be parsable according to the
		/// grammar in the RFC 2045.
		/// 
		/// </para>
		/// </summary>
		/// <param name="path">
		///          the path to the file to probe
		/// </param>
		/// <returns>  The content type or {@code null} if the file type is not
		///          recognized
		/// </returns>
		/// <exception cref="IOException">
		///          An I/O error occurs </exception>
		/// <exception cref="SecurityException">
		///          If the implementation requires to access the file, and a
		///          security manager is installed, and it denies an unspecified
		///          permission required by a file system provider implementation.
		///          If the file reference is associated with the default file system
		///          provider then the <seealso cref="SecurityManager#checkRead(String)"/> method
		///          is invoked to check read access to the file.
		/// </exception>
		/// <seealso cref= java.nio.file.Files#probeContentType </seealso>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public abstract String probeContentType(java.nio.file.Path path) throws java.io.IOException;
		public abstract String ProbeContentType(Path path);
	}

}
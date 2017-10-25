/*
 * Copyright (c) 2005, 2013, Oracle and/or its affiliates. All rights reserved.
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

namespace java.awt.peer
{



	/// <summary>
	/// The {@code DesktopPeer} interface provides methods for the operation
	/// of open, edit, print, browse and mail with the given URL or file, by
	/// launching the associated application.
	/// <para>
	/// Each platform has an implementation class for this interface.
	/// 
	/// </para>
	/// </summary>
	public interface DesktopPeer
	{

		/// <summary>
		/// Returns whether the given action is supported on the current platform. </summary>
		/// <param name="action"> the action type to be tested if it's supported on the
		///        current platform. </param>
		/// <returns> {@code true} if the given action is supported on
		///         the current platform; {@code false} otherwise. </returns>
		bool IsSupported(Action action);

		/// <summary>
		/// Launches the associated application to open the given file. The
		/// associated application is registered to be the default file viewer for
		/// the file type of the given file.
		/// </summary>
		/// <param name="file"> the given file. </param>
		/// <exception cref="IOException"> If the given file has no associated application,
		///         or the associated application fails to be launched. </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: void open(java.io.File file) throws java.io.IOException;
		void Open(File file);

		/// <summary>
		/// Launches the associated editor and opens the given file for editing. The
		/// associated editor is registered to be the default editor for the file
		/// type of the given file.
		/// </summary>
		/// <param name="file"> the given file. </param>
		/// <exception cref="IOException"> If the given file has no associated editor, or
		///         the associated application fails to be launched. </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: void edit(java.io.File file) throws java.io.IOException;
		void Edit(File file);

		/// <summary>
		/// Prints the given file with the native desktop printing facility, using
		/// the associated application's print command.
		/// </summary>
		/// <param name="file"> the given file. </param>
		/// <exception cref="IOException"> If the given file has no associated application
		///         that can be used to print it. </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: void print(java.io.File file) throws java.io.IOException;
		void Print(File file);

		/// <summary>
		/// Launches the mail composing window of the user default mail client,
		/// filling the message fields including to, cc, etc, with the values
		/// specified by the given mailto URL.
		/// </summary>
		/// <param name="mailtoURL"> represents a mailto URL with specified values of the message.
		///        The syntax of mailto URL is defined by
		///        <a href="http://www.ietf.org/rfc/rfc2368.txt">RFC2368: The mailto
		///        URL scheme</a> </param>
		/// <exception cref="IOException"> If the user default mail client is not found,
		///         or it fails to be launched. </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: void mail(java.net.URI mailtoURL) throws java.io.IOException;
		void Mail(URI mailtoURL);

		/// <summary>
		/// Launches the user default browser to display the given URI.
		/// </summary>
		/// <param name="uri"> the given URI. </param>
		/// <exception cref="IOException"> If the user default browser is not found,
		///         or it fails to be launched. </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: void browse(java.net.URI uri) throws java.io.IOException;
		void Browse(URI uri);
	}

}
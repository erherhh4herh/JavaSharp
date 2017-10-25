using System;

/*
 * Copyright (c) 2005, 2006, Oracle and/or its affiliates. All rights reserved.
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

	using SunToolkit = sun.awt.SunToolkit;
	using HeadlessToolkit = sun.awt.HeadlessToolkit;
	using SecurityConstants = sun.security.util.SecurityConstants;

	/// <summary>
	/// The {@code Desktop} class allows a Java application to launch
	/// associated applications registered on the native desktop to handle
	/// a <seealso cref="java.net.URI"/> or a file.
	/// 
	/// <para> Supported operations include:
	/// <ul>
	///   <li>launching the user-default browser to show a specified
	///       URI;</li>
	///   <li>launching the user-default mail client with an optional
	///       {@code mailto} URI;</li>
	///   <li>launching a registered application to open, edit or print a
	///       specified file.</li>
	/// </ul>
	/// 
	/// </para>
	/// <para> This class provides methods corresponding to these
	/// operations. The methods look for the associated application
	/// registered on the current platform, and launch it to handle a URI
	/// or file. If there is no associated application or the associated
	/// application fails to be launched, an exception is thrown.
	/// 
	/// </para>
	/// <para> An application is registered to a URI or file type; for
	/// example, the {@code "sxi"} file extension is typically registered
	/// to StarOffice.  The mechanism of registering, accessing, and
	/// launching the associated application is platform-dependent.
	/// 
	/// </para>
	/// <para> Each operation is an action type represented by the {@link
	/// Desktop.Action} class.
	/// 
	/// </para>
	/// <para> Note: when some action is invoked and the associated
	/// application is executed, it will be executed on the same system as
	/// the one on which the Java application was launched.
	/// 
	/// @since 1.6
	/// @author Armin Chen
	/// @author George Zhang
	/// </para>
	/// </summary>
	public class Desktop
	{

		/// <summary>
		/// Represents an action type.  Each platform supports a different
		/// set of actions.  You may use the <seealso cref="Desktop#isSupported"/>
		/// method to determine if the given action is supported by the
		/// current platform. </summary>
		/// <seealso cref= java.awt.Desktop#isSupported(java.awt.Desktop.Action)
		/// @since 1.6 </seealso>
		public enum Action
		{
			/// <summary>
			/// Represents an "open" action. </summary>
			/// <seealso cref= Desktop#open(java.io.File) </seealso>
			OPEN,
			/// <summary>
			/// Represents an "edit" action. </summary>
			/// <seealso cref= Desktop#edit(java.io.File) </seealso>
			EDIT,
			/// <summary>
			/// Represents a "print" action. </summary>
			/// <seealso cref= Desktop#print(java.io.File) </seealso>
			PRINT,
			/// <summary>
			/// Represents a "mail" action. </summary>
			/// <seealso cref= Desktop#mail() </seealso>
			/// <seealso cref= Desktop#mail(java.net.URI) </seealso>
			MAIL,
			/// <summary>
			/// Represents a "browse" action. </summary>
			/// <seealso cref= Desktop#browse(java.net.URI) </seealso>
			BROWSE
		}

		private DesktopPeer Peer;

		/// <summary>
		/// Suppresses default constructor for noninstantiability.
		/// </summary>
		private Desktop()
		{
			Peer = Toolkit.DefaultToolkit.CreateDesktopPeer(this);
		}

		/// <summary>
		/// Returns the <code>Desktop</code> instance of the current
		/// browser context.  On some platforms the Desktop API may not be
		/// supported; use the <seealso cref="#isDesktopSupported"/> method to
		/// determine if the current desktop is supported. </summary>
		/// <returns> the Desktop instance of the current browser context </returns>
		/// <exception cref="HeadlessException"> if {@link
		/// GraphicsEnvironment#isHeadless()} returns {@code true} </exception>
		/// <exception cref="UnsupportedOperationException"> if this class is not
		/// supported on the current platform </exception>
		/// <seealso cref= #isDesktopSupported() </seealso>
		/// <seealso cref= java.awt.GraphicsEnvironment#isHeadless </seealso>
		public static Desktop Desktop
		{
			get
			{
				lock (typeof(Desktop))
				{
					if (GraphicsEnvironment.Headless)
					{
						throw new HeadlessException();
					}
					if (!Desktop.DesktopSupported)
					{
						throw new UnsupportedOperationException("Desktop API is not " + "supported on the current platform");
					}
            
					sun.awt.AppContext context = sun.awt.AppContext.AppContext;
					Desktop desktop = (Desktop)context.get(typeof(Desktop));
            
					if (desktop == null)
					{
						desktop = new Desktop();
						context.put(typeof(Desktop), desktop);
					}
            
					return desktop;
				}
			}
		}

		/// <summary>
		/// Tests whether this class is supported on the current platform.
		/// If it's supported, use <seealso cref="#getDesktop()"/> to retrieve an
		/// instance.
		/// </summary>
		/// <returns> <code>true</code> if this class is supported on the
		///         current platform; <code>false</code> otherwise </returns>
		/// <seealso cref= #getDesktop() </seealso>
		public static bool DesktopSupported
		{
			get
			{
				Toolkit defaultToolkit = Toolkit.DefaultToolkit;
				if (defaultToolkit is SunToolkit)
				{
					return ((SunToolkit)defaultToolkit).DesktopSupported;
				}
				return false;
			}
		}

		/// <summary>
		/// Tests whether an action is supported on the current platform.
		/// 
		/// <para>Even when the platform supports an action, a file or URI may
		/// not have a registered application for the action.  For example,
		/// most of the platforms support the <seealso cref="Desktop.Action#OPEN"/>
		/// action.  But for a specific file, there may not be an
		/// application registered to open it.  In this case, {@link
		/// #isSupported} may return {@code true}, but the corresponding
		/// action method will throw an <seealso cref="IOException"/>.
		/// 
		/// </para>
		/// </summary>
		/// <param name="action"> the specified <seealso cref="Action"/> </param>
		/// <returns> <code>true</code> if the specified action is supported on
		///         the current platform; <code>false</code> otherwise </returns>
		/// <seealso cref= Desktop.Action </seealso>
		public virtual bool IsSupported(Action action)
		{
			return Peer.IsSupported(action);
		}

		/// <summary>
		/// Checks if the file is a valid file and readable.
		/// </summary>
		/// <exception cref="SecurityException"> If a security manager exists and its
		///         <seealso cref="SecurityManager#checkRead(java.lang.String)"/> method
		///         denies read access to the file </exception>
		/// <exception cref="NullPointerException"> if file is null </exception>
		/// <exception cref="IllegalArgumentException"> if file doesn't exist </exception>
		private static void CheckFileValidation(File file)
		{
			if (file == null)
			{
				throw new NullPointerException("File must not be null");
			}

			if (!file.Exists())
			{
				throw new IllegalArgumentException("The file: " + file.Path + " doesn't exist.");
			}

			file.CanRead();
		}

		/// <summary>
		/// Checks if the action type is supported.
		/// </summary>
		/// <param name="actionType"> the action type in question </param>
		/// <exception cref="UnsupportedOperationException"> if the specified action type is not
		///         supported on the current platform </exception>
		private void CheckActionSupport(Action actionType)
		{
			if (!IsSupported(actionType))
			{
				throw new UnsupportedOperationException("The " + actionType.name() + " action is not supported on the current platform!");
			}
		}


		/// <summary>
		///  Calls to the security manager's <code>checkPermission</code> method with
		///  an <code>AWTPermission("showWindowWithoutWarningBanner")</code>
		///  permission.
		/// </summary>
		private void CheckAWTPermission()
		{
			SecurityManager sm = System.SecurityManager;
			if (sm != null)
			{
				sm.CheckPermission(new AWTPermission("showWindowWithoutWarningBanner"));
			}
		}

		/// <summary>
		/// Launches the associated application to open the file.
		/// 
		/// <para> If the specified file is a directory, the file manager of
		/// the current platform is launched to open it.
		/// 
		/// </para>
		/// </summary>
		/// <param name="file"> the file to be opened with the associated application </param>
		/// <exception cref="NullPointerException"> if {@code file} is {@code null} </exception>
		/// <exception cref="IllegalArgumentException"> if the specified file doesn't
		/// exist </exception>
		/// <exception cref="UnsupportedOperationException"> if the current platform
		/// does not support the <seealso cref="Desktop.Action#OPEN"/> action </exception>
		/// <exception cref="IOException"> if the specified file has no associated
		/// application or the associated application fails to be launched </exception>
		/// <exception cref="SecurityException"> if a security manager exists and its
		/// <seealso cref="java.lang.SecurityManager#checkRead(java.lang.String)"/>
		/// method denies read access to the file, or it denies the
		/// <code>AWTPermission("showWindowWithoutWarningBanner")</code>
		/// permission, or the calling thread is not allowed to create a
		/// subprocess </exception>
		/// <seealso cref= java.awt.AWTPermission </seealso>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public void open(java.io.File file) throws java.io.IOException
		public virtual void Open(File file)
		{
			CheckAWTPermission();
			CheckExec();
			CheckActionSupport(Action.OPEN);
			CheckFileValidation(file);

			Peer.Open(file);
		}

		/// <summary>
		/// Launches the associated editor application and opens a file for
		/// editing.
		/// </summary>
		/// <param name="file"> the file to be opened for editing </param>
		/// <exception cref="NullPointerException"> if the specified file is {@code null} </exception>
		/// <exception cref="IllegalArgumentException"> if the specified file doesn't
		/// exist </exception>
		/// <exception cref="UnsupportedOperationException"> if the current platform
		/// does not support the <seealso cref="Desktop.Action#EDIT"/> action </exception>
		/// <exception cref="IOException"> if the specified file has no associated
		/// editor, or the associated application fails to be launched </exception>
		/// <exception cref="SecurityException"> if a security manager exists and its
		/// <seealso cref="java.lang.SecurityManager#checkRead(java.lang.String)"/>
		/// method denies read access to the file, or {@link
		/// java.lang.SecurityManager#checkWrite(java.lang.String)} method
		/// denies write access to the file, or it denies the
		/// <code>AWTPermission("showWindowWithoutWarningBanner")</code>
		/// permission, or the calling thread is not allowed to create a
		/// subprocess </exception>
		/// <seealso cref= java.awt.AWTPermission </seealso>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public void edit(java.io.File file) throws java.io.IOException
		public virtual void Edit(File file)
		{
			CheckAWTPermission();
			CheckExec();
			CheckActionSupport(Action.EDIT);
			file.CanWrite();
			CheckFileValidation(file);

			Peer.Edit(file);
		}

		/// <summary>
		/// Prints a file with the native desktop printing facility, using
		/// the associated application's print command.
		/// </summary>
		/// <param name="file"> the file to be printed </param>
		/// <exception cref="NullPointerException"> if the specified file is {@code
		/// null} </exception>
		/// <exception cref="IllegalArgumentException"> if the specified file doesn't
		/// exist </exception>
		/// <exception cref="UnsupportedOperationException"> if the current platform
		///         does not support the <seealso cref="Desktop.Action#PRINT"/> action </exception>
		/// <exception cref="IOException"> if the specified file has no associated
		/// application that can be used to print it </exception>
		/// <exception cref="SecurityException"> if a security manager exists and its
		/// <seealso cref="java.lang.SecurityManager#checkRead(java.lang.String)"/>
		/// method denies read access to the file, or its {@link
		/// java.lang.SecurityManager#checkPrintJobAccess()} method denies
		/// the permission to print the file, or the calling thread is not
		/// allowed to create a subprocess </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public void print(java.io.File file) throws java.io.IOException
		public virtual void Print(File file)
		{
			CheckExec();
			SecurityManager sm = System.SecurityManager;
			if (sm != null)
			{
				sm.CheckPrintJobAccess();
			}
			CheckActionSupport(Action.PRINT);
			CheckFileValidation(file);

			Peer.Print(file);
		}

		/// <summary>
		/// Launches the default browser to display a {@code URI}.
		/// If the default browser is not able to handle the specified
		/// {@code URI}, the application registered for handling
		/// {@code URIs} of the specified type is invoked. The application
		/// is determined from the protocol and path of the {@code URI}, as
		/// defined by the {@code URI} class.
		/// <para>
		/// If the calling thread does not have the necessary permissions,
		/// and this is invoked from within an applet,
		/// {@code AppletContext.showDocument()} is used. Similarly, if the calling
		/// does not have the necessary permissions, and this is invoked from within
		/// a Java Web Started application, {@code BasicService.showDocument()}
		/// is used.
		/// 
		/// </para>
		/// </summary>
		/// <param name="uri"> the URI to be displayed in the user default browser </param>
		/// <exception cref="NullPointerException"> if {@code uri} is {@code null} </exception>
		/// <exception cref="UnsupportedOperationException"> if the current platform
		/// does not support the <seealso cref="Desktop.Action#BROWSE"/> action </exception>
		/// <exception cref="IOException"> if the user default browser is not found,
		/// or it fails to be launched, or the default handler application
		/// failed to be launched </exception>
		/// <exception cref="SecurityException"> if a security manager exists and it
		/// denies the
		/// <code>AWTPermission("showWindowWithoutWarningBanner")</code>
		/// permission, or the calling thread is not allowed to create a
		/// subprocess; and not invoked from within an applet or Java Web Started
		/// application </exception>
		/// <exception cref="IllegalArgumentException"> if the necessary permissions
		/// are not available and the URI can not be converted to a {@code URL} </exception>
		/// <seealso cref= java.net.URI </seealso>
		/// <seealso cref= java.awt.AWTPermission </seealso>
		/// <seealso cref= java.applet.AppletContext </seealso>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public void browse(java.net.URI uri) throws java.io.IOException
		public virtual void Browse(URI uri)
		{
			SecurityException securityException = null;
			try
			{
				CheckAWTPermission();
				CheckExec();
			}
			catch (SecurityException e)
			{
				securityException = e;
			}
			CheckActionSupport(Action.BROWSE);
			if (uri == null)
			{
				throw new NullPointerException();
			}
			if (securityException == null)
			{
				Peer.Browse(uri);
				return;
			}

			// Calling thread doesn't have necessary priviledges.
			// Delegate to DesktopBrowse so that it can work in
			// applet/webstart.
			URL url = null;
			try
			{
				url = uri.ToURL();
			}
			catch (MalformedURLException e)
			{
				throw new IllegalArgumentException("Unable to convert URI to URL", e);
			}
			sun.awt.DesktopBrowse db = sun.awt.DesktopBrowse.Instance;
			if (db == null)
			{
				// Not in webstart/applet, throw the exception.
				throw securityException;
			}
			db.browse(url);
		}

		/// <summary>
		/// Launches the mail composing window of the user default mail
		/// client.
		/// </summary>
		/// <exception cref="UnsupportedOperationException"> if the current platform
		/// does not support the <seealso cref="Desktop.Action#MAIL"/> action </exception>
		/// <exception cref="IOException"> if the user default mail client is not
		/// found, or it fails to be launched </exception>
		/// <exception cref="SecurityException"> if a security manager exists and it
		/// denies the
		/// <code>AWTPermission("showWindowWithoutWarningBanner")</code>
		/// permission, or the calling thread is not allowed to create a
		/// subprocess </exception>
		/// <seealso cref= java.awt.AWTPermission </seealso>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public void mail() throws java.io.IOException
		public virtual void Mail()
		{
			CheckAWTPermission();
			CheckExec();
			CheckActionSupport(Action.MAIL);
			URI mailtoURI = null;
			try
			{
				mailtoURI = new URI("mailto:?");
				Peer.Mail(mailtoURI);
			}
			catch (URISyntaxException)
			{
				// won't reach here.
			}
		}

		/// <summary>
		/// Launches the mail composing window of the user default mail
		/// client, filling the message fields specified by a {@code
		/// mailto:} URI.
		/// 
		/// <para> A <code>mailto:</code> URI can specify message fields
		/// including <i>"to"</i>, <i>"cc"</i>, <i>"subject"</i>,
		/// <i>"body"</i>, etc.  See <a
		/// href="http://www.ietf.org/rfc/rfc2368.txt">The mailto URL
		/// scheme (RFC 2368)</a> for the {@code mailto:} URI specification
		/// details.
		/// 
		/// </para>
		/// </summary>
		/// <param name="mailtoURI"> the specified {@code mailto:} URI </param>
		/// <exception cref="NullPointerException"> if the specified URI is {@code
		/// null} </exception>
		/// <exception cref="IllegalArgumentException"> if the URI scheme is not
		///         <code>"mailto"</code> </exception>
		/// <exception cref="UnsupportedOperationException"> if the current platform
		/// does not support the <seealso cref="Desktop.Action#MAIL"/> action </exception>
		/// <exception cref="IOException"> if the user default mail client is not
		/// found or fails to be launched </exception>
		/// <exception cref="SecurityException"> if a security manager exists and it
		/// denies the
		/// <code>AWTPermission("showWindowWithoutWarningBanner")</code>
		/// permission, or the calling thread is not allowed to create a
		/// subprocess </exception>
		/// <seealso cref= java.net.URI </seealso>
		/// <seealso cref= java.awt.AWTPermission </seealso>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public void mail(java.net.URI mailtoURI) throws java.io.IOException
		public virtual void Mail(URI mailtoURI)
		{
			CheckAWTPermission();
			CheckExec();
			CheckActionSupport(Action.MAIL);
			if (mailtoURI == null)
			{
				throw new NullPointerException();
			}

			if (!"mailto".Equals(mailtoURI.Scheme, StringComparison.CurrentCultureIgnoreCase))
			{
				throw new IllegalArgumentException("URI scheme is not \"mailto\"");
			}

			Peer.Mail(mailtoURI);
		}

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: private void checkExec() throws SecurityException
		private void CheckExec()
		{
			SecurityManager sm = System.SecurityManager;
			if (sm != null)
			{
				sm.CheckPermission(new FilePermission("<<ALL FILES>>", SecurityConstants.FILE_EXECUTE_ACTION));
			}
		}
	}

}
/*
 * Copyright (c) 1995, 2013, Oracle and/or its affiliates. All rights reserved.
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
namespace java.applet
{

	/// <summary>
	/// When an applet is first created, an applet stub is attached to it
	/// using the applet's <code>setStub</code> method. This stub
	/// serves as the interface between the applet and the browser
	/// environment or applet viewer environment in which the application
	/// is running.
	/// 
	/// @author      Arthur van Hoff </summary>
	/// <seealso cref=         java.applet.Applet#setStub(java.applet.AppletStub)
	/// @since       JDK1.0 </seealso>
	public interface AppletStub
	{
		/// <summary>
		/// Determines if the applet is active. An applet is active just
		/// before its <code>start</code> method is called. It becomes
		/// inactive just before its <code>stop</code> method is called.
		/// </summary>
		/// <returns>  <code>true</code> if the applet is active;
		///          <code>false</code> otherwise. </returns>
		bool Active {get;}


		/// <summary>
		/// Gets the URL of the document in which the applet is embedded.
		/// For example, suppose an applet is contained
		/// within the document:
		/// <blockquote><pre>
		///    http://www.oracle.com/technetwork/java/index.html
		/// </pre></blockquote>
		/// The document base is:
		/// <blockquote><pre>
		///    http://www.oracle.com/technetwork/java/index.html
		/// </pre></blockquote>
		/// </summary>
		/// <returns>  the <seealso cref="java.net.URL"/> of the document that contains the
		///          applet. </returns>
		/// <seealso cref=     java.applet.AppletStub#getCodeBase() </seealso>
		URL DocumentBase {get;}

		/// <summary>
		/// Gets the base URL. This is the URL of the directory which contains the applet.
		/// </summary>
		/// <returns>  the base <seealso cref="java.net.URL"/> of
		///          the directory which contains the applet. </returns>
		/// <seealso cref=     java.applet.AppletStub#getDocumentBase() </seealso>
		URL CodeBase {get;}

		/// <summary>
		/// Returns the value of the named parameter in the HTML tag. For
		/// example, if an applet is specified as
		/// <blockquote><pre>
		/// &lt;applet code="Clock" width=50 height=50&gt;
		/// &lt;param name=Color value="blue"&gt;
		/// &lt;/applet&gt;
		/// </pre></blockquote>
		/// <para>
		/// then a call to <code>getParameter("Color")</code> returns the
		/// value <code>"blue"</code>.
		/// 
		/// </para>
		/// </summary>
		/// <param name="name">   a parameter name. </param>
		/// <returns>  the value of the named parameter,
		/// or <tt>null</tt> if not set. </returns>
		String GetParameter(String name);

		/// <summary>
		/// Returns the applet's context.
		/// </summary>
		/// <returns>  the applet's context. </returns>
		AppletContext AppletContext {get;}

		/// <summary>
		/// Called when the applet wants to be resized.
		/// </summary>
		/// <param name="width">    the new requested width for the applet. </param>
		/// <param name="height">   the new requested height for the applet. </param>
		void AppletResize(int width, int height);
	}

}
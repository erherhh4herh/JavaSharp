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

namespace java.net
{

	/// <summary>
	/// The abstract class {@code ContentHandler} is the superclass
	/// of all classes that read an {@code Object} from a
	/// {@code URLConnection}.
	/// <para>
	/// An application does not generally call the
	/// {@code getContent} method in this class directly. Instead, an
	/// application calls the {@code getContent} method in class
	/// {@code URL} or in {@code URLConnection}.
	/// The application's content handler factory (an instance of a class that
	/// implements the interface {@code ContentHandlerFactory} set
	/// up by a call to {@code setContentHandler}) is
	/// called with a {@code String} giving the MIME type of the
	/// object being received on the socket. The factory returns an
	/// instance of a subclass of {@code ContentHandler}, and its
	/// {@code getContent} method is called to create the object.
	/// </para>
	/// <para>
	/// If no content handler could be found, URLConnection will
	/// look for a content handler in a user-defineable set of places.
	/// By default it looks in sun.net.www.content, but users can define a
	/// vertical-bar delimited set of class prefixes to search through in
	/// addition by defining the java.content.handler.pkgs property.
	/// The class name must be of the form:
	/// <pre>
	///     {package-prefix}.{major}.{minor}
	/// e.g.
	///     YoyoDyne.experimental.text.plain
	/// </pre>
	/// If the loading of the content handler class would be performed by
	/// a classloader that is outside of the delegation chain of the caller,
	/// the JVM will need the RuntimePermission "getClassLoader".
	/// 
	/// @author  James Gosling
	/// </para>
	/// </summary>
	/// <seealso cref=     java.net.ContentHandler#getContent(java.net.URLConnection) </seealso>
	/// <seealso cref=     java.net.ContentHandlerFactory </seealso>
	/// <seealso cref=     java.net.URL#getContent() </seealso>
	/// <seealso cref=     java.net.URLConnection </seealso>
	/// <seealso cref=     java.net.URLConnection#getContent() </seealso>
	/// <seealso cref=     java.net.URLConnection#setContentHandlerFactory(java.net.ContentHandlerFactory)
	/// @since   JDK1.0 </seealso>
	public abstract class ContentHandler
	{
		/// <summary>
		/// Given a URL connect stream positioned at the beginning of the
		/// representation of an object, this method reads that stream and
		/// creates an object from it.
		/// </summary>
		/// <param name="urlc">   a URL connection. </param>
		/// <returns>     the object read by the {@code ContentHandler}. </returns>
		/// <exception cref="IOException">  if an I/O error occurs while reading the object. </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public abstract Object getContent(URLConnection urlc) throws java.io.IOException;
		public abstract Object GetContent(URLConnection urlc);

		/// <summary>
		/// Given a URL connect stream positioned at the beginning of the
		/// representation of an object, this method reads that stream and
		/// creates an object that matches one of the types specified.
		/// 
		/// The default implementation of this method should call getContent()
		/// and screen the return type for a match of the suggested types.
		/// </summary>
		/// <param name="urlc">   a URL connection. </param>
		/// <param name="classes">      an array of types requested </param>
		/// <returns>     the object read by the {@code ContentHandler} that is
		///                 the first match of the suggested types.
		///                 null if none of the requested  are supported. </returns>
		/// <exception cref="IOException">  if an I/O error occurs while reading the object.
		/// @since 1.3 </exception>
//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @SuppressWarnings("rawtypes") public Object getContent(URLConnection urlc, Class[] classes) throws java.io.IOException
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
		public virtual Object GetContent(URLConnection urlc, Class[] classes)
		{
			Object obj = GetContent(urlc);

			for (int i = 0; i < classes.Length; i++)
			{
			  if (classes[i].isInstance(obj))
			  {
					return obj;
			  }
			}
			return null;
		}

	}

}
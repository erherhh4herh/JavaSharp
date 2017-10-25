/*
 * Copyright (c) 1997, 2013, Oracle and/or its affiliates. All rights reserved.
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

	using ParseUtil = sun.net.www.ParseUtil;

	/// <summary>
	/// A URL Connection to a Java ARchive (JAR) file or an entry in a JAR
	/// file.
	/// 
	/// <para>The syntax of a JAR URL is:
	/// 
	/// <pre>
	/// jar:&lt;url&gt;!/{entry}
	/// </pre>
	/// 
	/// </para>
	/// <para>for example:
	/// 
	/// </para>
	/// <para>{@code jar:http://www.foo.com/bar/baz.jar!/COM/foo/Quux.class}
	/// 
	/// </para>
	/// <para>Jar URLs should be used to refer to a JAR file or entries in
	/// a JAR file. The example above is a JAR URL which refers to a JAR
	/// entry. If the entry name is omitted, the URL refers to the whole
	/// JAR file:
	/// 
	/// {@code jar:http://www.foo.com/bar/baz.jar!/}
	/// 
	/// </para>
	/// <para>Users should cast the generic URLConnection to a
	/// JarURLConnection when they know that the URL they created is a JAR
	/// URL, and they need JAR-specific functionality. For example:
	/// 
	/// <pre>
	/// URL url = new URL("jar:file:/home/duke/duke.jar!/");
	/// JarURLConnection jarConnection = (JarURLConnection)url.openConnection();
	/// Manifest manifest = jarConnection.getManifest();
	/// </pre>
	/// 
	/// </para>
	/// <para>JarURLConnection instances can only be used to read from JAR files.
	/// It is not possible to get a <seealso cref="java.io.OutputStream"/> to modify or write
	/// to the underlying JAR file using this class.
	/// </para>
	/// <para>Examples:
	/// 
	/// <dl>
	/// 
	/// <dt>A Jar entry
	/// <dd>{@code jar:http://www.foo.com/bar/baz.jar!/COM/foo/Quux.class}
	/// 
	/// <dt>A Jar file
	/// <dd>{@code jar:http://www.foo.com/bar/baz.jar!/}
	/// 
	/// <dt>A Jar directory
	/// <dd>{@code jar:http://www.foo.com/bar/baz.jar!/COM/foo/}
	/// 
	/// </dl>
	/// 
	/// </para>
	/// <para>{@code !/} is referred to as the <em>separator</em>.
	/// 
	/// </para>
	/// <para>When constructing a JAR url via {@code new URL(context, spec)},
	/// the following rules apply:
	/// 
	/// <ul>
	/// 
	/// <li>if there is no context URL and the specification passed to the
	/// URL constructor doesn't contain a separator, the URL is considered
	/// to refer to a JarFile.
	/// 
	/// <li>if there is a context URL, the context URL is assumed to refer
	/// to a JAR file or a Jar directory.
	/// 
	/// <li>if the specification begins with a '/', the Jar directory is
	/// ignored, and the spec is considered to be at the root of the Jar
	/// file.
	/// 
	/// </para>
	/// <para>Examples:
	/// 
	/// <dl>
	/// 
	/// <dt>context: <b>jar:http://www.foo.com/bar/jar.jar!/</b>,
	/// spec:<b>baz/entry.txt</b>
	/// 
	/// <dd>url:<b>jar:http://www.foo.com/bar/jar.jar!/baz/entry.txt</b>
	/// 
	/// <dt>context: <b>jar:http://www.foo.com/bar/jar.jar!/baz</b>,
	/// spec:<b>entry.txt</b>
	/// 
	/// <dd>url:<b>jar:http://www.foo.com/bar/jar.jar!/baz/entry.txt</b>
	/// 
	/// <dt>context: <b>jar:http://www.foo.com/bar/jar.jar!/baz</b>,
	/// spec:<b>/entry.txt</b>
	/// 
	/// <dd>url:<b>jar:http://www.foo.com/bar/jar.jar!/entry.txt</b>
	/// 
	/// </dl>
	/// 
	/// </ul>
	/// 
	/// </para>
	/// </summary>
	/// <seealso cref= java.net.URL </seealso>
	/// <seealso cref= java.net.URLConnection
	/// </seealso>
	/// <seealso cref= java.util.jar.JarFile </seealso>
	/// <seealso cref= java.util.jar.JarInputStream </seealso>
	/// <seealso cref= java.util.jar.Manifest </seealso>
	/// <seealso cref= java.util.zip.ZipEntry
	/// 
	/// @author Benjamin Renaud
	/// @since 1.2 </seealso>
	public abstract class JarURLConnection : URLConnection
	{

		private URL JarFileURL_Renamed;
		private String EntryName_Renamed;

		/// <summary>
		/// The connection to the JAR file URL, if the connection has been
		/// initiated. This should be set by connect.
		/// </summary>
		protected internal URLConnection JarFileURLConnection;

		/// <summary>
		/// Creates the new JarURLConnection to the specified URL. </summary>
		/// <param name="url"> the URL </param>
		/// <exception cref="MalformedURLException"> if no legal protocol
		/// could be found in a specification string or the
		/// string could not be parsed. </exception>

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: protected JarURLConnection(URL url) throws MalformedURLException
		protected internal JarURLConnection(URL url) : base(url)
		{
			ParseSpecs(url);
		}

		/* get the specs for a given url out of the cache, and compute and
		 * cache them if they're not there.
		 */
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: private void parseSpecs(URL url) throws MalformedURLException
		private void ParseSpecs(URL url)
		{
			String spec = url.File;

			int separator = spec.IndexOf("!/");
			/*
			 * REMIND: we don't handle nested JAR URLs
			 */
			if (separator == -1)
			{
				throw new MalformedURLException("no !/ found in url spec:" + spec);
			}

			JarFileURL_Renamed = new URL(spec.Substring(0, separator++));
			EntryName_Renamed = null;

			/* if ! is the last letter of the innerURL, entryName is null */
			if (++separator != spec.Length())
			{
				EntryName_Renamed = spec.Substring(separator, spec.Length() - separator);
				EntryName_Renamed = ParseUtil.decode(EntryName_Renamed);
			}
		}

		/// <summary>
		/// Returns the URL for the Jar file for this connection.
		/// </summary>
		/// <returns> the URL for the Jar file for this connection. </returns>
		public virtual URL JarFileURL
		{
			get
			{
				return JarFileURL_Renamed;
			}
		}

		/// <summary>
		/// Return the entry name for this connection. This method
		/// returns null if the JAR file URL corresponding to this
		/// connection points to a JAR file and not a JAR file entry.
		/// </summary>
		/// <returns> the entry name for this connection, if any. </returns>
		public virtual String EntryName
		{
			get
			{
				return EntryName_Renamed;
			}
		}

		/// <summary>
		/// Return the JAR file for this connection.
		/// </summary>
		/// <returns> the JAR file for this connection. If the connection is
		/// a connection to an entry of a JAR file, the JAR file object is
		/// returned
		/// </returns>
		/// <exception cref="IOException"> if an IOException occurs while trying to
		/// connect to the JAR file for this connection.
		/// </exception>
		/// <seealso cref= #connect </seealso>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public abstract java.util.jar.JarFile getJarFile() throws java.io.IOException;
		public abstract JarFile JarFile {get;}

		/// <summary>
		/// Returns the Manifest for this connection, or null if none.
		/// </summary>
		/// <returns> the manifest object corresponding to the JAR file object
		/// for this connection.
		/// </returns>
		/// <exception cref="IOException"> if getting the JAR file for this
		/// connection causes an IOException to be thrown.
		/// </exception>
		/// <seealso cref= #getJarFile </seealso>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public java.util.jar.Manifest getManifest() throws java.io.IOException
		public virtual Manifest Manifest
		{
			get
			{
				return JarFile.Manifest;
			}
		}

		/// <summary>
		/// Return the JAR entry object for this connection, if any. This
		/// method returns null if the JAR file URL corresponding to this
		/// connection points to a JAR file and not a JAR file entry.
		/// </summary>
		/// <returns> the JAR entry object for this connection, or null if
		/// the JAR URL for this connection points to a JAR file.
		/// </returns>
		/// <exception cref="IOException"> if getting the JAR file for this
		/// connection causes an IOException to be thrown.
		/// </exception>
		/// <seealso cref= #getJarFile </seealso>
		/// <seealso cref= #getJarEntry </seealso>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public java.util.jar.JarEntry getJarEntry() throws java.io.IOException
		public virtual JarEntry JarEntry
		{
			get
			{
				return JarFile.GetJarEntry(EntryName_Renamed);
			}
		}

		/// <summary>
		/// Return the Attributes object for this connection if the URL
		/// for it points to a JAR file entry, null otherwise.
		/// </summary>
		/// <returns> the Attributes object for this connection if the URL
		/// for it points to a JAR file entry, null otherwise.
		/// </returns>
		/// <exception cref="IOException"> if getting the JAR entry causes an
		/// IOException to be thrown.
		/// </exception>
		/// <seealso cref= #getJarEntry </seealso>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public java.util.jar.Attributes getAttributes() throws java.io.IOException
		public virtual Attributes Attributes
		{
			get
			{
				JarEntry e = JarEntry;
				return e != null ? e.Attributes : null;
			}
		}

		/// <summary>
		/// Returns the main Attributes for the JAR file for this
		/// connection.
		/// </summary>
		/// <returns> the main Attributes for the JAR file for this
		/// connection.
		/// </returns>
		/// <exception cref="IOException"> if getting the manifest causes an
		/// IOException to be thrown.
		/// </exception>
		/// <seealso cref= #getJarFile </seealso>
		/// <seealso cref= #getManifest </seealso>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public java.util.jar.Attributes getMainAttributes() throws java.io.IOException
		public virtual Attributes MainAttributes
		{
			get
			{
				Manifest man = Manifest;
				return man != null ? man.MainAttributes : null;
			}
		}

		/// <summary>
		/// Return the Certificate object for this connection if the URL
		/// for it points to a JAR file entry, null otherwise. This method
		/// can only be called once
		/// the connection has been completely verified by reading
		/// from the input stream until the end of the stream has been
		/// reached. Otherwise, this method will return {@code null}
		/// </summary>
		/// <returns> the Certificate object for this connection if the URL
		/// for it points to a JAR file entry, null otherwise.
		/// </returns>
		/// <exception cref="IOException"> if getting the JAR entry causes an
		/// IOException to be thrown.
		/// </exception>
		/// <seealso cref= #getJarEntry </seealso>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public java.security.cert.Certificate[] getCertificates() throws java.io.IOException
		public virtual java.security.cert.Certificate[] Certificates
		{
			get
			{
				JarEntry e = JarEntry;
				return e != null ? e.Certificates : null;
			}
		}
	}

}
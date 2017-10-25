using System;
using System.Collections.Generic;

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

	using SecurityConstants = sun.security.util.SecurityConstants;
	using MessageHeader = sun.net.www.MessageHeader;

	/// <summary>
	/// The abstract class {@code URLConnection} is the superclass
	/// of all classes that represent a communications link between the
	/// application and a URL. Instances of this class can be used both to
	/// read from and to write to the resource referenced by the URL. In
	/// general, creating a connection to a URL is a multistep process:
	/// 
	/// <center><table border=2 summary="Describes the process of creating a connection to a URL: openConnection() and connect() over time.">
	/// <tr><th>{@code openConnection()}</th>
	///     <th>{@code connect()}</th></tr>
	/// <tr><td>Manipulate parameters that affect the connection to the remote
	///         resource.</td>
	///     <td>Interact with the resource; query header fields and
	///         contents.</td></tr>
	/// </table>
	/// ----------------------------&gt;
	/// <br>time</center>
	/// 
	/// <ol>
	/// <li>The connection object is created by invoking the
	///     {@code openConnection} method on a URL.
	/// <li>The setup parameters and general request properties are manipulated.
	/// <li>The actual connection to the remote object is made, using the
	///    {@code connect} method.
	/// <li>The remote object becomes available. The header fields and the contents
	///     of the remote object can be accessed.
	/// </ol>
	/// <para>
	/// The setup parameters are modified using the following methods:
	/// <ul>
	///   <li>{@code setAllowUserInteraction}
	///   <li>{@code setDoInput}
	///   <li>{@code setDoOutput}
	///   <li>{@code setIfModifiedSince}
	///   <li>{@code setUseCaches}
	/// </ul>
	/// </para>
	/// <para>
	/// and the general request properties are modified using the method:
	/// <ul>
	///   <li>{@code setRequestProperty}
	/// </ul>
	/// </para>
	/// <para>
	/// Default values for the {@code AllowUserInteraction} and
	/// {@code UseCaches} parameters can be set using the methods
	/// {@code setDefaultAllowUserInteraction} and
	/// {@code setDefaultUseCaches}.
	/// </para>
	/// <para>
	/// Each of the above {@code set} methods has a corresponding
	/// {@code get} method to retrieve the value of the parameter or
	/// general request property. The specific parameters and general
	/// request properties that are applicable are protocol specific.
	/// </para>
	/// <para>
	/// The following methods are used to access the header fields and
	/// the contents after the connection is made to the remote object:
	/// <ul>
	///   <li>{@code getContent}
	///   <li>{@code getHeaderField}
	///   <li>{@code getInputStream}
	///   <li>{@code getOutputStream}
	/// </ul>
	/// </para>
	/// <para>
	/// Certain header fields are accessed frequently. The methods:
	/// <ul>
	///   <li>{@code getContentEncoding}
	///   <li>{@code getContentLength}
	///   <li>{@code getContentType}
	///   <li>{@code getDate}
	///   <li>{@code getExpiration}
	///   <li>{@code getLastModifed}
	/// </ul>
	/// </para>
	/// <para>
	/// provide convenient access to these fields. The
	/// {@code getContentType} method is used by the
	/// {@code getContent} method to determine the type of the remote
	/// object; subclasses may find it convenient to override the
	/// {@code getContentType} method.
	/// </para>
	/// <para>
	/// In the common case, all of the pre-connection parameters and
	/// general request properties can be ignored: the pre-connection
	/// parameters and request properties default to sensible values. For
	/// most clients of this interface, there are only two interesting
	/// methods: {@code getInputStream} and {@code getContent},
	/// which are mirrored in the {@code URL} class by convenience methods.
	/// </para>
	/// <para>
	/// More information on the request properties and header fields of
	/// an {@code http} connection can be found at:
	/// <blockquote><pre>
	/// <a href="http://www.ietf.org/rfc/rfc2616.txt">http://www.ietf.org/rfc/rfc2616.txt</a>
	/// </pre></blockquote>
	/// 
	/// Invoking the {@code close()} methods on the {@code InputStream} or {@code OutputStream} of an
	/// {@code URLConnection} after a request may free network resources associated with this
	/// instance, unless particular protocol specifications specify different behaviours
	/// for it.
	/// 
	/// @author  James Gosling
	/// </para>
	/// </summary>
	/// <seealso cref=     java.net.URL#openConnection() </seealso>
	/// <seealso cref=     java.net.URLConnection#connect() </seealso>
	/// <seealso cref=     java.net.URLConnection#getContent() </seealso>
	/// <seealso cref=     java.net.URLConnection#getContentEncoding() </seealso>
	/// <seealso cref=     java.net.URLConnection#getContentLength() </seealso>
	/// <seealso cref=     java.net.URLConnection#getContentType() </seealso>
	/// <seealso cref=     java.net.URLConnection#getDate() </seealso>
	/// <seealso cref=     java.net.URLConnection#getExpiration() </seealso>
	/// <seealso cref=     java.net.URLConnection#getHeaderField(int) </seealso>
	/// <seealso cref=     java.net.URLConnection#getHeaderField(java.lang.String) </seealso>
	/// <seealso cref=     java.net.URLConnection#getInputStream() </seealso>
	/// <seealso cref=     java.net.URLConnection#getLastModified() </seealso>
	/// <seealso cref=     java.net.URLConnection#getOutputStream() </seealso>
	/// <seealso cref=     java.net.URLConnection#setAllowUserInteraction(boolean) </seealso>
	/// <seealso cref=     java.net.URLConnection#setDefaultUseCaches(boolean) </seealso>
	/// <seealso cref=     java.net.URLConnection#setDoInput(boolean) </seealso>
	/// <seealso cref=     java.net.URLConnection#setDoOutput(boolean) </seealso>
	/// <seealso cref=     java.net.URLConnection#setIfModifiedSince(long) </seealso>
	/// <seealso cref=     java.net.URLConnection#setRequestProperty(java.lang.String, java.lang.String) </seealso>
	/// <seealso cref=     java.net.URLConnection#setUseCaches(boolean)
	/// @since   JDK1.0 </seealso>
	public abstract class URLConnection
	{

	   /// <summary>
	   /// The URL represents the remote object on the World Wide Web to
	   /// which this connection is opened.
	   /// <para>
	   /// The value of this field can be accessed by the
	   /// {@code getURL} method.
	   /// </para>
	   /// <para>
	   /// The default value of this variable is the value of the URL
	   /// argument in the {@code URLConnection} constructor.
	   ///  
	   /// </para>
	   /// </summary>
	   /// <seealso cref=     java.net.URLConnection#getURL() </seealso>
	   /// <seealso cref=     java.net.URLConnection#url </seealso>
		protected internal URL Url;

	   /// <summary>
	   /// This variable is set by the {@code setDoInput} method. Its
	   /// value is returned by the {@code getDoInput} method.
	   /// <para>
	   /// A URL connection can be used for input and/or output. Setting the
	   /// {@code doInput} flag to {@code true} indicates that
	   /// the application intends to read data from the URL connection.
	   /// </para>
	   /// <para>
	   /// The default value of this field is {@code true}.
	   ///  
	   /// </para>
	   /// </summary>
	   /// <seealso cref=     java.net.URLConnection#getDoInput() </seealso>
	   /// <seealso cref=     java.net.URLConnection#setDoInput(boolean) </seealso>
		protected internal bool DoInput_Renamed = true;

	   /// <summary>
	   /// This variable is set by the {@code setDoOutput} method. Its
	   /// value is returned by the {@code getDoOutput} method.
	   /// <para>
	   /// A URL connection can be used for input and/or output. Setting the
	   /// {@code doOutput} flag to {@code true} indicates
	   /// that the application intends to write data to the URL connection.
	   /// </para>
	   /// <para>
	   /// The default value of this field is {@code false}.
	   ///  
	   /// </para>
	   /// </summary>
	   /// <seealso cref=     java.net.URLConnection#getDoOutput() </seealso>
	   /// <seealso cref=     java.net.URLConnection#setDoOutput(boolean) </seealso>
		protected internal bool DoOutput_Renamed = false;

		private static bool DefaultAllowUserInteraction_Renamed = false;

	   /// <summary>
	   /// If {@code true}, this {@code URL} is being examined in
	   /// a context in which it makes sense to allow user interactions such
	   /// as popping up an authentication dialog. If {@code false},
	   /// then no user interaction is allowed.
	   /// <para>
	   /// The value of this field can be set by the
	   /// {@code setAllowUserInteraction} method.
	   /// Its value is returned by the
	   /// {@code getAllowUserInteraction} method.
	   /// Its default value is the value of the argument in the last invocation
	   /// of the {@code setDefaultAllowUserInteraction} method.
	   ///  
	   /// </para>
	   /// </summary>
	   /// <seealso cref=     java.net.URLConnection#getAllowUserInteraction() </seealso>
	   /// <seealso cref=     java.net.URLConnection#setAllowUserInteraction(boolean) </seealso>
	   /// <seealso cref=     java.net.URLConnection#setDefaultAllowUserInteraction(boolean) </seealso>
		protected internal bool AllowUserInteraction_Renamed = DefaultAllowUserInteraction_Renamed;

		private static bool DefaultUseCaches_Renamed = true;

	   /// <summary>
	   /// If {@code true}, the protocol is allowed to use caching
	   /// whenever it can. If {@code false}, the protocol must always
	   /// try to get a fresh copy of the object.
	   /// <para>
	   /// This field is set by the {@code setUseCaches} method. Its
	   /// value is returned by the {@code getUseCaches} method.
	   /// </para>
	   /// <para>
	   /// Its default value is the value given in the last invocation of the
	   /// {@code setDefaultUseCaches} method.
	   ///  
	   /// </para>
	   /// </summary>
	   /// <seealso cref=     java.net.URLConnection#setUseCaches(boolean) </seealso>
	   /// <seealso cref=     java.net.URLConnection#getUseCaches() </seealso>
	   /// <seealso cref=     java.net.URLConnection#setDefaultUseCaches(boolean) </seealso>
		protected internal bool UseCaches_Renamed = DefaultUseCaches_Renamed;

	   /// <summary>
	   /// Some protocols support skipping the fetching of the object unless
	   /// the object has been modified more recently than a certain time.
	   /// <para>
	   /// A nonzero value gives a time as the number of milliseconds since
	   /// January 1, 1970, GMT. The object is fetched only if it has been
	   /// modified more recently than that time.
	   /// </para>
	   /// <para>
	   /// This variable is set by the {@code setIfModifiedSince}
	   /// method. Its value is returned by the
	   /// {@code getIfModifiedSince} method.
	   /// </para>
	   /// <para>
	   /// The default value of this field is {@code 0}, indicating
	   /// that the fetching must always occur.
	   ///  
	   /// </para>
	   /// </summary>
	   /// <seealso cref=     java.net.URLConnection#getIfModifiedSince() </seealso>
	   /// <seealso cref=     java.net.URLConnection#setIfModifiedSince(long) </seealso>
		protected internal long IfModifiedSince_Renamed = 0;

	   /// <summary>
	   /// If {@code false}, this connection object has not created a
	   /// communications link to the specified URL. If {@code true},
	   /// the communications link has been established.
	   /// </summary>
		protected internal bool Connected = false;

		/// <summary>
		/// @since 1.5
		/// </summary>
		private int ConnectTimeout_Renamed;
		private int ReadTimeout_Renamed;

		/// <summary>
		/// @since 1.6
		/// </summary>
		private MessageHeader Requests;

	   /// <summary>
	   /// @since   JDK1.1
	   /// </summary>
		private static FileNameMap FileNameMap_Renamed;

		/// <summary>
		/// @since 1.2.2
		/// </summary>
		private static bool FileNameMapLoaded = false;

		/// <summary>
		/// Loads filename map (a mimetable) from a data file. It will
		/// first try to load the user-specific table, defined
		/// by &quot;content.types.user.table&quot; property. If that fails,
		/// it tries to load the default built-in table.
		/// </summary>
		/// <returns> the FileNameMap
		/// @since 1.2 </returns>
		/// <seealso cref= #setFileNameMap(java.net.FileNameMap) </seealso>
		public static FileNameMap FileNameMap
		{
			get
			{
				lock (typeof(URLConnection))
				{
					if ((FileNameMap_Renamed == null) && !FileNameMapLoaded)
					{
						FileNameMap_Renamed = sun.net.www.MimeTable.loadTable();
						FileNameMapLoaded = true;
					}
            
					return new FileNameMapAnonymousInnerClassHelper();
				}
			}
			set
			{
				SecurityManager sm = System.SecurityManager;
				if (sm != null)
				{
					sm.CheckSetFactory();
				}
				FileNameMap_Renamed = value;
			}
		}

		private class FileNameMapAnonymousInnerClassHelper : FileNameMap
		{
			public FileNameMapAnonymousInnerClassHelper()
			{
			}

			private FileNameMap map = FileNameMap_Renamed;
			public virtual String GetContentTypeFor(String fileName)
			{
				return map.getContentTypeFor(fileName);
			}
		}


		/// <summary>
		/// Opens a communications link to the resource referenced by this
		/// URL, if such a connection has not already been established.
		/// <para>
		/// If the {@code connect} method is called when the connection
		/// has already been opened (indicated by the {@code connected}
		/// field having the value {@code true}), the call is ignored.
		/// </para>
		/// <para>
		/// URLConnection objects go through two phases: first they are
		/// created, then they are connected.  After being created, and
		/// before being connected, various options can be specified
		/// (e.g., doInput and UseCaches).  After connecting, it is an
		/// error to try to set them.  Operations that depend on being
		/// connected, like getContentLength, will implicitly perform the
		/// connection, if necessary.
		/// 
		/// </para>
		/// </summary>
		/// <exception cref="SocketTimeoutException"> if the timeout expires before
		///               the connection can be established </exception>
		/// <exception cref="IOException">  if an I/O error occurs while opening the
		///               connection. </exception>
		/// <seealso cref= java.net.URLConnection#connected </seealso>
		/// <seealso cref= #getConnectTimeout() </seealso>
		/// <seealso cref= #setConnectTimeout(int) </seealso>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public abstract void connect() throws java.io.IOException;
		public abstract void Connect();

		/// <summary>
		/// Sets a specified timeout value, in milliseconds, to be used
		/// when opening a communications link to the resource referenced
		/// by this URLConnection.  If the timeout expires before the
		/// connection can be established, a
		/// java.net.SocketTimeoutException is raised. A timeout of zero is
		/// interpreted as an infinite timeout.
		/// 
		/// <para> Some non-standard implementation of this method may ignore
		/// the specified timeout. To see the connect timeout set, please
		/// call getConnectTimeout().
		/// 
		/// </para>
		/// </summary>
		/// <param name="timeout"> an {@code int} that specifies the connect
		///               timeout value in milliseconds </param>
		/// <exception cref="IllegalArgumentException"> if the timeout parameter is negative
		/// </exception>
		/// <seealso cref= #getConnectTimeout() </seealso>
		/// <seealso cref= #connect()
		/// @since 1.5 </seealso>
		public virtual int ConnectTimeout
		{
			set
			{
				if (value < 0)
				{
					throw new IllegalArgumentException("timeout can not be negative");
				}
				ConnectTimeout_Renamed = value;
			}
			get
			{
				return ConnectTimeout_Renamed;
			}
		}


		/// <summary>
		/// Sets the read timeout to a specified timeout, in
		/// milliseconds. A non-zero value specifies the timeout when
		/// reading from Input stream when a connection is established to a
		/// resource. If the timeout expires before there is data available
		/// for read, a java.net.SocketTimeoutException is raised. A
		/// timeout of zero is interpreted as an infinite timeout.
		/// 
		/// <para> Some non-standard implementation of this method ignores the
		/// specified timeout. To see the read timeout set, please call
		/// getReadTimeout().
		/// 
		/// </para>
		/// </summary>
		/// <param name="timeout"> an {@code int} that specifies the timeout
		/// value to be used in milliseconds </param>
		/// <exception cref="IllegalArgumentException"> if the timeout parameter is negative
		/// </exception>
		/// <seealso cref= #getReadTimeout() </seealso>
		/// <seealso cref= InputStream#read()
		/// @since 1.5 </seealso>
		public virtual int ReadTimeout
		{
			set
			{
				if (value < 0)
				{
					throw new IllegalArgumentException("timeout can not be negative");
				}
				ReadTimeout_Renamed = value;
			}
			get
			{
				return ReadTimeout_Renamed;
			}
		}


		/// <summary>
		/// Constructs a URL connection to the specified URL. A connection to
		/// the object referenced by the URL is not created.
		/// </summary>
		/// <param name="url">   the specified URL. </param>
		protected internal URLConnection(URL url)
		{
			this.Url = url;
		}

		/// <summary>
		/// Returns the value of this {@code URLConnection}'s {@code URL}
		/// field.
		/// </summary>
		/// <returns>  the value of this {@code URLConnection}'s {@code URL}
		///          field. </returns>
		/// <seealso cref=     java.net.URLConnection#url </seealso>
		public virtual URL URL
		{
			get
			{
				return Url;
			}
		}

		/// <summary>
		/// Returns the value of the {@code content-length} header field.
		/// <P>
		/// <B>Note</B>: <seealso cref="#getContentLengthLong() getContentLengthLong()"/>
		/// should be preferred over this method, since it returns a {@code long}
		/// instead and is therefore more portable.</P>
		/// </summary>
		/// <returns>  the content length of the resource that this connection's URL
		///          references, {@code -1} if the content length is not known,
		///          or if the content length is greater than Integer.MAX_VALUE. </returns>
		public virtual int ContentLength
		{
			get
			{
				long l = ContentLengthLong;
				if (l > Integer.MaxValue)
				{
					return -1;
				}
				return (int) l;
			}
		}

		/// <summary>
		/// Returns the value of the {@code content-length} header field as a
		/// long.
		/// </summary>
		/// <returns>  the content length of the resource that this connection's URL
		///          references, or {@code -1} if the content length is
		///          not known.
		/// @since 7.0 </returns>
		public virtual long ContentLengthLong
		{
			get
			{
				return GetHeaderFieldLong("content-length", -1);
			}
		}

		/// <summary>
		/// Returns the value of the {@code content-type} header field.
		/// </summary>
		/// <returns>  the content type of the resource that the URL references,
		///          or {@code null} if not known. </returns>
		/// <seealso cref=     java.net.URLConnection#getHeaderField(java.lang.String) </seealso>
		public virtual String ContentType
		{
			get
			{
				return GetHeaderField("content-type");
			}
		}

		/// <summary>
		/// Returns the value of the {@code content-encoding} header field.
		/// </summary>
		/// <returns>  the content encoding of the resource that the URL references,
		///          or {@code null} if not known. </returns>
		/// <seealso cref=     java.net.URLConnection#getHeaderField(java.lang.String) </seealso>
		public virtual String ContentEncoding
		{
			get
			{
				return GetHeaderField("content-encoding");
			}
		}

		/// <summary>
		/// Returns the value of the {@code expires} header field.
		/// </summary>
		/// <returns>  the expiration date of the resource that this URL references,
		///          or 0 if not known. The value is the number of milliseconds since
		///          January 1, 1970 GMT. </returns>
		/// <seealso cref=     java.net.URLConnection#getHeaderField(java.lang.String) </seealso>
		public virtual long Expiration
		{
			get
			{
				return GetHeaderFieldDate("expires", 0);
			}
		}

		/// <summary>
		/// Returns the value of the {@code date} header field.
		/// </summary>
		/// <returns>  the sending date of the resource that the URL references,
		///          or {@code 0} if not known. The value returned is the
		///          number of milliseconds since January 1, 1970 GMT. </returns>
		/// <seealso cref=     java.net.URLConnection#getHeaderField(java.lang.String) </seealso>
		public virtual long Date
		{
			get
			{
				return GetHeaderFieldDate("date", 0);
			}
		}

		/// <summary>
		/// Returns the value of the {@code last-modified} header field.
		/// The result is the number of milliseconds since January 1, 1970 GMT.
		/// </summary>
		/// <returns>  the date the resource referenced by this
		///          {@code URLConnection} was last modified, or 0 if not known. </returns>
		/// <seealso cref=     java.net.URLConnection#getHeaderField(java.lang.String) </seealso>
		public virtual long LastModified
		{
			get
			{
				return GetHeaderFieldDate("last-modified", 0);
			}
		}

		/// <summary>
		/// Returns the value of the named header field.
		/// <para>
		/// If called on a connection that sets the same header multiple times
		/// with possibly different values, only the last value is returned.
		/// 
		/// 
		/// </para>
		/// </summary>
		/// <param name="name">   the name of a header field. </param>
		/// <returns>  the value of the named header field, or {@code null}
		///          if there is no such field in the header. </returns>
		public virtual String GetHeaderField(String name)
		{
			return null;
		}

		/// <summary>
		/// Returns an unmodifiable Map of the header fields.
		/// The Map keys are Strings that represent the
		/// response-header field names. Each Map value is an
		/// unmodifiable List of Strings that represents
		/// the corresponding field values.
		/// </summary>
		/// <returns> a Map of header fields
		/// @since 1.4 </returns>
		public virtual IDictionary<String, IList<String>> HeaderFields
		{
			get
			{
				return Collections.EmptyMap();
			}
		}

		/// <summary>
		/// Returns the value of the named field parsed as a number.
		/// <para>
		/// This form of {@code getHeaderField} exists because some
		/// connection types (e.g., {@code http-ng}) have pre-parsed
		/// headers. Classes for that connection type can override this method
		/// and short-circuit the parsing.
		/// 
		/// </para>
		/// </summary>
		/// <param name="name">      the name of the header field. </param>
		/// <param name="Default">   the default value. </param>
		/// <returns>  the value of the named field, parsed as an integer. The
		///          {@code Default} value is returned if the field is
		///          missing or malformed. </returns>
		public virtual int GetHeaderFieldInt(String name, int Default)
		{
			String value = GetHeaderField(name);
			try
			{
				return Convert.ToInt32(value);
			}
			catch (Exception)
			{
			}
			return Default;
		}

		/// <summary>
		/// Returns the value of the named field parsed as a number.
		/// <para>
		/// This form of {@code getHeaderField} exists because some
		/// connection types (e.g., {@code http-ng}) have pre-parsed
		/// headers. Classes for that connection type can override this method
		/// and short-circuit the parsing.
		/// 
		/// </para>
		/// </summary>
		/// <param name="name">      the name of the header field. </param>
		/// <param name="Default">   the default value. </param>
		/// <returns>  the value of the named field, parsed as a long. The
		///          {@code Default} value is returned if the field is
		///          missing or malformed.
		/// @since 7.0 </returns>
		public virtual long GetHeaderFieldLong(String name, long Default)
		{
			String value = GetHeaderField(name);
			try
			{
				return Convert.ToInt64(value);
			}
			catch (Exception)
			{
			}
			return Default;
		}

		/// <summary>
		/// Returns the value of the named field parsed as date.
		/// The result is the number of milliseconds since January 1, 1970 GMT
		/// represented by the named field.
		/// <para>
		/// This form of {@code getHeaderField} exists because some
		/// connection types (e.g., {@code http-ng}) have pre-parsed
		/// headers. Classes for that connection type can override this method
		/// and short-circuit the parsing.
		/// 
		/// </para>
		/// </summary>
		/// <param name="name">     the name of the header field. </param>
		/// <param name="Default">   a default value. </param>
		/// <returns>  the value of the field, parsed as a date. The value of the
		///          {@code Default} argument is returned if the field is
		///          missing or malformed. </returns>
//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @SuppressWarnings("deprecation") public long getHeaderFieldDate(String name, long Default)
		public virtual long GetHeaderFieldDate(String name, long Default)
		{
			String value = GetHeaderField(name);
			try
			{
				return DateTime.Parse(value);
			}
			catch (Exception)
			{
			}
			return Default;
		}

		/// <summary>
		/// Returns the key for the {@code n}<sup>th</sup> header field.
		/// It returns {@code null} if there are fewer than {@code n+1} fields.
		/// </summary>
		/// <param name="n">   an index, where {@code n>=0} </param>
		/// <returns>  the key for the {@code n}<sup>th</sup> header field,
		///          or {@code null} if there are fewer than {@code n+1}
		///          fields. </returns>
		public virtual String GetHeaderFieldKey(int n)
		{
			return null;
		}

		/// <summary>
		/// Returns the value for the {@code n}<sup>th</sup> header field.
		/// It returns {@code null} if there are fewer than
		/// {@code n+1}fields.
		/// <para>
		/// This method can be used in conjunction with the
		/// <seealso cref="#getHeaderFieldKey(int) getHeaderFieldKey"/> method to iterate through all
		/// the headers in the message.
		/// 
		/// </para>
		/// </summary>
		/// <param name="n">   an index, where {@code n>=0} </param>
		/// <returns>  the value of the {@code n}<sup>th</sup> header field
		///          or {@code null} if there are fewer than {@code n+1} fields </returns>
		/// <seealso cref=     java.net.URLConnection#getHeaderFieldKey(int) </seealso>
		public virtual String GetHeaderField(int n)
		{
			return null;
		}

		/// <summary>
		/// Retrieves the contents of this URL connection.
		/// <para>
		/// This method first determines the content type of the object by
		/// calling the {@code getContentType} method. If this is
		/// the first time that the application has seen that specific content
		/// type, a content handler for that content type is created:
		/// <ol>
		/// <li>If the application has set up a content handler factory instance
		///     using the {@code setContentHandlerFactory} method, the
		///     {@code createContentHandler} method of that instance is called
		///     with the content type as an argument; the result is a content
		///     handler for that content type.
		/// <li>If no content handler factory has yet been set up, or if the
		///     factory's {@code createContentHandler} method returns
		///     {@code null}, then the application loads the class named:
		///     <blockquote><pre>
		///         sun.net.www.content.&lt;<i>contentType</i>&gt;
		///     </pre></blockquote>
		///     where &lt;<i>contentType</i>&gt; is formed by taking the
		///     content-type string, replacing all slash characters with a
		///     {@code period} ('.'), and all other non-alphanumeric characters
		///     with the underscore character '{@code _}'. The alphanumeric
		///     characters are specifically the 26 uppercase ASCII letters
		///     '{@code A}' through '{@code Z}', the 26 lowercase ASCII
		///     letters '{@code a}' through '{@code z}', and the 10 ASCII
		///     digits '{@code 0}' through '{@code 9}'. If the specified
		///     class does not exist, or is not a subclass of
		///     {@code ContentHandler}, then an
		///     {@code UnknownServiceException} is thrown.
		/// </ol>
		/// 
		/// </para>
		/// </summary>
		/// <returns>     the object fetched. The {@code instanceof} operator
		///               should be used to determine the specific kind of object
		///               returned. </returns>
		/// <exception cref="IOException">              if an I/O error occurs while
		///               getting the content. </exception>
		/// <exception cref="UnknownServiceException">  if the protocol does not support
		///               the content type. </exception>
		/// <seealso cref=        java.net.ContentHandlerFactory#createContentHandler(java.lang.String) </seealso>
		/// <seealso cref=        java.net.URLConnection#getContentType() </seealso>
		/// <seealso cref=        java.net.URLConnection#setContentHandlerFactory(java.net.ContentHandlerFactory) </seealso>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public Object getContent() throws java.io.IOException
		public virtual Object Content
		{
			get
			{
				// Must call getInputStream before GetHeaderField gets called
				// so that FileNotFoundException has a chance to be thrown up
				// from here without being caught.
				InputStream;
				return ContentHandler.GetContent(this);
			}
		}

		/// <summary>
		/// Retrieves the contents of this URL connection.
		/// </summary>
		/// <param name="classes"> the {@code Class} array
		/// indicating the requested types </param>
		/// <returns>     the object fetched that is the first match of the type
		///               specified in the classes array. null if none of
		///               the requested types are supported.
		///               The {@code instanceof} operator should be used to
		///               determine the specific kind of object returned. </returns>
		/// <exception cref="IOException">              if an I/O error occurs while
		///               getting the content. </exception>
		/// <exception cref="UnknownServiceException">  if the protocol does not support
		///               the content type. </exception>
		/// <seealso cref=        java.net.URLConnection#getContent() </seealso>
		/// <seealso cref=        java.net.ContentHandlerFactory#createContentHandler(java.lang.String) </seealso>
		/// <seealso cref=        java.net.URLConnection#getContent(java.lang.Class[]) </seealso>
		/// <seealso cref=        java.net.URLConnection#setContentHandlerFactory(java.net.ContentHandlerFactory)
		/// @since 1.3 </seealso>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public Object getContent(Class[] classes) throws java.io.IOException
		public virtual Object GetContent(Class[] classes)
		{
			// Must call getInputStream before GetHeaderField gets called
			// so that FileNotFoundException has a chance to be thrown up
			// from here without being caught.
			InputStream;
			return ContentHandler.GetContent(this, classes);
		}

		/// <summary>
		/// Returns a permission object representing the permission
		/// necessary to make the connection represented by this
		/// object. This method returns null if no permission is
		/// required to make the connection. By default, this method
		/// returns {@code java.security.AllPermission}. Subclasses
		/// should override this method and return the permission
		/// that best represents the permission required to make a
		/// a connection to the URL. For example, a {@code URLConnection}
		/// representing a {@code file:} URL would return a
		/// {@code java.io.FilePermission} object.
		/// 
		/// <para>The permission returned may dependent upon the state of the
		/// connection. For example, the permission before connecting may be
		/// different from that after connecting. For example, an HTTP
		/// sever, say foo.com, may redirect the connection to a different
		/// host, say bar.com. Before connecting the permission returned by
		/// the connection will represent the permission needed to connect
		/// to foo.com, while the permission returned after connecting will
		/// be to bar.com.
		/// 
		/// </para>
		/// <para>Permissions are generally used for two purposes: to protect
		/// caches of objects obtained through URLConnections, and to check
		/// the right of a recipient to learn about a particular URL. In
		/// the first case, the permission should be obtained
		/// <em>after</em> the object has been obtained. For example, in an
		/// HTTP connection, this will represent the permission to connect
		/// to the host from which the data was ultimately fetched. In the
		/// second case, the permission should be obtained and tested
		/// <em>before</em> connecting.
		/// 
		/// </para>
		/// </summary>
		/// <returns> the permission object representing the permission
		/// necessary to make the connection represented by this
		/// URLConnection.
		/// </returns>
		/// <exception cref="IOException"> if the computation of the permission
		/// requires network or file I/O and an exception occurs while
		/// computing it. </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public java.security.Permission getPermission() throws java.io.IOException
		public virtual Permission Permission
		{
			get
			{
				return SecurityConstants.ALL_PERMISSION;
			}
		}

		/// <summary>
		/// Returns an input stream that reads from this open connection.
		/// 
		/// A SocketTimeoutException can be thrown when reading from the
		/// returned input stream if the read timeout expires before data
		/// is available for read.
		/// </summary>
		/// <returns>     an input stream that reads from this open connection. </returns>
		/// <exception cref="IOException">              if an I/O error occurs while
		///               creating the input stream. </exception>
		/// <exception cref="UnknownServiceException">  if the protocol does not support
		///               input. </exception>
		/// <seealso cref= #setReadTimeout(int) </seealso>
		/// <seealso cref= #getReadTimeout() </seealso>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public java.io.InputStream getInputStream() throws java.io.IOException
		public virtual InputStream InputStream
		{
			get
			{
				throw new UnknownServiceException("protocol doesn't support input");
			}
		}

		/// <summary>
		/// Returns an output stream that writes to this connection.
		/// </summary>
		/// <returns>     an output stream that writes to this connection. </returns>
		/// <exception cref="IOException">              if an I/O error occurs while
		///               creating the output stream. </exception>
		/// <exception cref="UnknownServiceException">  if the protocol does not support
		///               output. </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public java.io.OutputStream getOutputStream() throws java.io.IOException
		public virtual OutputStream OutputStream
		{
			get
			{
				throw new UnknownServiceException("protocol doesn't support output");
			}
		}

		/// <summary>
		/// Returns a {@code String} representation of this URL connection.
		/// </summary>
		/// <returns>  a string representation of this {@code URLConnection}. </returns>
		public override String ToString()
		{
//JAVA TO C# CONVERTER WARNING: The .NET Type.FullName property will not always yield results identical to the Java Class.getName method:
			return this.GetType().FullName + ":" + Url;
		}

		/// <summary>
		/// Sets the value of the {@code doInput} field for this
		/// {@code URLConnection} to the specified value.
		/// <para>
		/// A URL connection can be used for input and/or output.  Set the DoInput
		/// flag to true if you intend to use the URL connection for input,
		/// false if not.  The default is true.
		/// 
		/// </para>
		/// </summary>
		/// <param name="doinput">   the new value. </param>
		/// <exception cref="IllegalStateException"> if already connected </exception>
		/// <seealso cref=     java.net.URLConnection#doInput </seealso>
		/// <seealso cref= #getDoInput() </seealso>
		public virtual bool DoInput
		{
			set
			{
				if (Connected)
				{
					throw new IllegalStateException("Already connected");
				}
				DoInput_Renamed = value;
			}
			get
			{
				return DoInput_Renamed;
			}
		}


		/// <summary>
		/// Sets the value of the {@code doOutput} field for this
		/// {@code URLConnection} to the specified value.
		/// <para>
		/// A URL connection can be used for input and/or output.  Set the DoOutput
		/// flag to true if you intend to use the URL connection for output,
		/// false if not.  The default is false.
		/// 
		/// </para>
		/// </summary>
		/// <param name="dooutput">   the new value. </param>
		/// <exception cref="IllegalStateException"> if already connected </exception>
		/// <seealso cref= #getDoOutput() </seealso>
		public virtual bool DoOutput
		{
			set
			{
				if (Connected)
				{
					throw new IllegalStateException("Already connected");
				}
				DoOutput_Renamed = value;
			}
			get
			{
				return DoOutput_Renamed;
			}
		}


		/// <summary>
		/// Set the value of the {@code allowUserInteraction} field of
		/// this {@code URLConnection}.
		/// </summary>
		/// <param name="allowuserinteraction">   the new value. </param>
		/// <exception cref="IllegalStateException"> if already connected </exception>
		/// <seealso cref=     #getAllowUserInteraction() </seealso>
		public virtual bool AllowUserInteraction
		{
			set
			{
				if (Connected)
				{
					throw new IllegalStateException("Already connected");
				}
				AllowUserInteraction_Renamed = value;
			}
			get
			{
				return AllowUserInteraction_Renamed;
			}
		}


		/// <summary>
		/// Sets the default value of the
		/// {@code allowUserInteraction} field for all future
		/// {@code URLConnection} objects to the specified value.
		/// </summary>
		/// <param name="defaultallowuserinteraction">   the new value. </param>
		/// <seealso cref=     #getDefaultAllowUserInteraction() </seealso>
		public static bool DefaultAllowUserInteraction
		{
			set
			{
				DefaultAllowUserInteraction_Renamed = value;
			}
			get
			{
				return DefaultAllowUserInteraction_Renamed;
			}
		}


		/// <summary>
		/// Sets the value of the {@code useCaches} field of this
		/// {@code URLConnection} to the specified value.
		/// <para>
		/// Some protocols do caching of documents.  Occasionally, it is important
		/// to be able to "tunnel through" and ignore the caches (e.g., the
		/// "reload" button in a browser).  If the UseCaches flag on a connection
		/// is true, the connection is allowed to use whatever caches it can.
		///  If false, caches are to be ignored.
		///  The default value comes from DefaultUseCaches, which defaults to
		/// true.
		/// 
		/// </para>
		/// </summary>
		/// <param name="usecaches"> a {@code boolean} indicating whether
		/// or not to allow caching </param>
		/// <exception cref="IllegalStateException"> if already connected </exception>
		/// <seealso cref= #getUseCaches() </seealso>
		public virtual bool UseCaches
		{
			set
			{
				if (Connected)
				{
					throw new IllegalStateException("Already connected");
				}
				UseCaches_Renamed = value;
			}
			get
			{
				return UseCaches_Renamed;
			}
		}


		/// <summary>
		/// Sets the value of the {@code ifModifiedSince} field of
		/// this {@code URLConnection} to the specified value.
		/// </summary>
		/// <param name="ifmodifiedsince">   the new value. </param>
		/// <exception cref="IllegalStateException"> if already connected </exception>
		/// <seealso cref=     #getIfModifiedSince() </seealso>
		public virtual long IfModifiedSince
		{
			set
			{
				if (Connected)
				{
					throw new IllegalStateException("Already connected");
				}
				IfModifiedSince_Renamed = value;
			}
			get
			{
				return IfModifiedSince_Renamed;
			}
		}


	   /// <summary>
	   /// Returns the default value of a {@code URLConnection}'s
	   /// {@code useCaches} flag.
	   /// <para>
	   /// Ths default is "sticky", being a part of the static state of all
	   /// URLConnections.  This flag applies to the next, and all following
	   /// URLConnections that are created.
	   ///  
	   /// </para>
	   /// </summary>
	   /// <returns>  the default value of a {@code URLConnection}'s
	   ///          {@code useCaches} flag. </returns>
	   /// <seealso cref=     #setDefaultUseCaches(boolean) </seealso>
		public virtual bool DefaultUseCaches
		{
			get
			{
				return DefaultUseCaches_Renamed;
			}
			set
			{
				DefaultUseCaches_Renamed = value;
			}
		}


		/// <summary>
		/// Sets the general request property. If a property with the key already
		/// exists, overwrite its value with the new value.
		/// 
		/// <para> NOTE: HTTP requires all request properties which can
		/// legally have multiple instances with the same key
		/// to use a comma-separated list syntax which enables multiple
		/// properties to be appended into a single property.
		/// 
		/// </para>
		/// </summary>
		/// <param name="key">     the keyword by which the request is known
		///                  (e.g., "{@code Accept}"). </param>
		/// <param name="value">   the value associated with it. </param>
		/// <exception cref="IllegalStateException"> if already connected </exception>
		/// <exception cref="NullPointerException"> if key is <CODE>null</CODE> </exception>
		/// <seealso cref= #getRequestProperty(java.lang.String) </seealso>
		public virtual void SetRequestProperty(String key, String value)
		{
			if (Connected)
			{
				throw new IllegalStateException("Already connected");
			}
			if (key == null)
			{
				throw new NullPointerException("key is null");
			}

			if (Requests == null)
			{
				Requests = new MessageHeader();
			}

			Requests.set(key, value);
		}

		/// <summary>
		/// Adds a general request property specified by a
		/// key-value pair.  This method will not overwrite
		/// existing values associated with the same key.
		/// </summary>
		/// <param name="key">     the keyword by which the request is known
		///                  (e.g., "{@code Accept}"). </param>
		/// <param name="value">  the value associated with it. </param>
		/// <exception cref="IllegalStateException"> if already connected </exception>
		/// <exception cref="NullPointerException"> if key is null </exception>
		/// <seealso cref= #getRequestProperties()
		/// @since 1.4 </seealso>
		public virtual void AddRequestProperty(String key, String value)
		{
			if (Connected)
			{
				throw new IllegalStateException("Already connected");
			}
			if (key == null)
			{
				throw new NullPointerException("key is null");
			}

			if (Requests == null)
			{
				Requests = new MessageHeader();
			}

			Requests.add(key, value);
		}


		/// <summary>
		/// Returns the value of the named general request property for this
		/// connection.
		/// </summary>
		/// <param name="key"> the keyword by which the request is known (e.g., "Accept"). </param>
		/// <returns>  the value of the named general request property for this
		///           connection. If key is null, then null is returned. </returns>
		/// <exception cref="IllegalStateException"> if already connected </exception>
		/// <seealso cref= #setRequestProperty(java.lang.String, java.lang.String) </seealso>
		public virtual String GetRequestProperty(String key)
		{
			if (Connected)
			{
				throw new IllegalStateException("Already connected");
			}

			if (Requests == null)
			{
				return null;
			}

			return Requests.findValue(key);
		}

		/// <summary>
		/// Returns an unmodifiable Map of general request
		/// properties for this connection. The Map keys
		/// are Strings that represent the request-header
		/// field names. Each Map value is a unmodifiable List
		/// of Strings that represents the corresponding
		/// field values.
		/// </summary>
		/// <returns>  a Map of the general request properties for this connection. </returns>
		/// <exception cref="IllegalStateException"> if already connected
		/// @since 1.4 </exception>
		public virtual IDictionary<String, IList<String>> RequestProperties
		{
			get
			{
				if (Connected)
				{
					throw new IllegalStateException("Already connected");
				}
    
				if (Requests == null)
				{
					return Collections.EmptyMap();
				}
    
				return Requests.getHeaders(null);
			}
		}

		/// <summary>
		/// Sets the default value of a general request property. When a
		/// {@code URLConnection} is created, it is initialized with
		/// these properties.
		/// </summary>
		/// <param name="key">     the keyword by which the request is known
		///                  (e.g., "{@code Accept}"). </param>
		/// <param name="value">   the value associated with the key.
		/// </param>
		/// <seealso cref= java.net.URLConnection#setRequestProperty(java.lang.String,java.lang.String)
		/// </seealso>
		/// @deprecated The instance specific setRequestProperty method
		/// should be used after an appropriate instance of URLConnection
		/// is obtained. Invoking this method will have no effect.
		/// 
		/// <seealso cref= #getDefaultRequestProperty(java.lang.String) </seealso>
		[Obsolete("The instance specific setRequestProperty method")]
		public static void SetDefaultRequestProperty(String key, String value)
		{
		}

		/// <summary>
		/// Returns the value of the default request property. Default request
		/// properties are set for every connection.
		/// </summary>
		/// <param name="key"> the keyword by which the request is known (e.g., "Accept"). </param>
		/// <returns>  the value of the default request property
		/// for the specified key.
		/// </returns>
		/// <seealso cref= java.net.URLConnection#getRequestProperty(java.lang.String)
		/// </seealso>
		/// @deprecated The instance specific getRequestProperty method
		/// should be used after an appropriate instance of URLConnection
		/// is obtained.
		/// 
		/// <seealso cref= #setDefaultRequestProperty(java.lang.String, java.lang.String) </seealso>
		[Obsolete("The instance specific getRequestProperty method")]
		public static String GetDefaultRequestProperty(String key)
		{
			return null;
		}

		/// <summary>
		/// The ContentHandler factory.
		/// </summary>
		internal static ContentHandlerFactory Factory;

		/// <summary>
		/// Sets the {@code ContentHandlerFactory} of an
		/// application. It can be called at most once by an application.
		/// <para>
		/// The {@code ContentHandlerFactory} instance is used to
		/// construct a content handler from a content type
		/// </para>
		/// <para>
		/// If there is a security manager, this method first calls
		/// the security manager's {@code checkSetFactory} method
		/// to ensure the operation is allowed.
		/// This could result in a SecurityException.
		/// 
		/// </para>
		/// </summary>
		/// <param name="fac">   the desired factory. </param>
		/// <exception cref="Error">  if the factory has already been defined. </exception>
		/// <exception cref="SecurityException">  if a security manager exists and its
		///             {@code checkSetFactory} method doesn't allow the operation. </exception>
		/// <seealso cref=        java.net.ContentHandlerFactory </seealso>
		/// <seealso cref=        java.net.URLConnection#getContent() </seealso>
		/// <seealso cref=        SecurityManager#checkSetFactory </seealso>
		public static ContentHandlerFactory ContentHandlerFactory
		{
			set
			{
				lock (typeof(URLConnection))
				{
					if (Factory != null)
					{
						throw new Error("factory already defined");
					}
					SecurityManager security = System.SecurityManager;
					if (security != null)
					{
						security.CheckSetFactory();
					}
					Factory = value;
				}
			}
		}

		private static Dictionary<String, ContentHandler> Handlers = new Dictionary<String, ContentHandler>();

		/// <summary>
		/// Gets the Content Handler appropriate for this connection.
		/// </summary>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: synchronized ContentHandler getContentHandler() throws UnknownServiceException
		internal virtual ContentHandler ContentHandler
		{
			get
			{
				lock (this)
				{
					String contentType = StripOffParameters(ContentType);
					ContentHandler handler = null;
					if (contentType == null)
					{
						throw new UnknownServiceException("no content-type");
					}
					try
					{
						handler = Handlers[contentType];
						if (handler != null)
						{
							return handler;
						}
					}
					catch (Exception)
					{
					}
            
					if (Factory != null)
					{
						handler = Factory.CreateContentHandler(contentType);
					}
					if (handler == null)
					{
						try
						{
							handler = LookupContentHandlerClassFor(contentType);
						}
						catch (Exception e)
						{
							e.PrintStackTrace();
							handler = UnknownContentHandler.INSTANCE;
						}
						Handlers[contentType] = handler;
					}
					return handler;
				}
			}
		}

		/*
		 * Media types are in the format: type/subtype*(; parameter).
		 * For looking up the content handler, we should ignore those
		 * parameters.
		 */
		private String StripOffParameters(String contentType)
		{
			if (contentType == null)
			{
				return null;
			}
			int index = contentType.IndexOf(';');

			if (index > 0)
			{
				return contentType.Substring(0, index);
			}
			else
			{
				return contentType;
			}
		}

		private const String ContentClassPrefix = "sun.net.www.content";
		private const String ContentPathProp = "java.content.handler.pkgs";

		/// <summary>
		/// Looks for a content handler in a user-defineable set of places.
		/// By default it looks in sun.net.www.content, but users can define a
		/// vertical-bar delimited set of class prefixes to search through in
		/// addition by defining the java.content.handler.pkgs property.
		/// The class name must be of the form:
		/// <pre>
		///     {package-prefix}.{major}.{minor}
		/// e.g.
		///     YoyoDyne.experimental.text.plain
		/// </pre>
		/// </summary>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: private ContentHandler lookupContentHandlerClassFor(String contentType) throws InstantiationException, IllegalAccessException, ClassNotFoundException
		private ContentHandler LookupContentHandlerClassFor(String contentType)
		{
			String contentHandlerClassName = TypeToPackageName(contentType);

			String contentHandlerPkgPrefixes = ContentHandlerPkgPrefixes;

			StringTokenizer packagePrefixIter = new StringTokenizer(contentHandlerPkgPrefixes, "|");

			while (packagePrefixIter.HasMoreTokens())
			{
				String packagePrefix = packagePrefixIter.NextToken().Trim();

				try
				{
					String clsName = packagePrefix + "." + contentHandlerClassName;
					Class cls = null;
					try
					{
						cls = Class.ForName(clsName);
					}
					catch (ClassNotFoundException)
					{
						ClassLoader cl = ClassLoader.SystemClassLoader;
						if (cl != null)
						{
							cls = cl.LoadClass(clsName);
						}
					}
					if (cls != null)
					{
						ContentHandler handler = (ContentHandler)cls.NewInstance();
						return handler;
					}
				}
				catch (Exception)
				{
				}
			}

			return UnknownContentHandler.INSTANCE;
		}

		/// <summary>
		/// Utility function to map a MIME content type into an equivalent
		/// pair of class name components.  For example: "text/html" would
		/// be returned as "text.html"
		/// </summary>
		private String TypeToPackageName(String contentType)
		{
			// make sure we canonicalize the class name: all lower case
			contentType = contentType.ToLowerCase();
			int len = contentType.Length();
			char[] nm = new char[len];
			contentType.GetChars(0, len, nm, 0);
			for (int i = 0; i < len; i++)
			{
				char c = nm[i];
				if (c == '/')
				{
					nm[i] = '.';
				}
				else if (!('A' <= c && c <= 'Z' || 'a' <= c && c <= 'z' || '0' <= c && c <= '9'))
				{
					nm[i] = '_';
				}
			}
			return new String(nm);
		}


		/// <summary>
		/// Returns a vertical bar separated list of package prefixes for potential
		/// content handlers.  Tries to get the java.content.handler.pkgs property
		/// to use as a set of package prefixes to search.  Whether or not
		/// that property has been defined, the sun.net.www.content is always
		/// the last one on the returned package list.
		/// </summary>
		private String ContentHandlerPkgPrefixes
		{
			get
			{
				String packagePrefixList = AccessController.doPrivileged(new sun.security.action.GetPropertyAction(ContentPathProp, ""));
    
				if (packagePrefixList != "")
				{
				packagePrefixList += "|";
				}
    
				return packagePrefixList + ContentClassPrefix;
			}
		}

		/// <summary>
		/// Tries to determine the content type of an object, based
		/// on the specified "file" component of a URL.
		/// This is a convenience method that can be used by
		/// subclasses that override the {@code getContentType} method.
		/// </summary>
		/// <param name="fname">   a filename. </param>
		/// <returns>  a guess as to what the content type of the object is,
		///          based upon its file name. </returns>
		/// <seealso cref=     java.net.URLConnection#getContentType() </seealso>
		public static String GuessContentTypeFromName(String fname)
		{
			return FileNameMap.GetContentTypeFor(fname);
		}

		/// <summary>
		/// Tries to determine the type of an input stream based on the
		/// characters at the beginning of the input stream. This method can
		/// be used by subclasses that override the
		/// {@code getContentType} method.
		/// <para>
		/// Ideally, this routine would not be needed. But many
		/// {@code http} servers return the incorrect content type; in
		/// addition, there are many nonstandard extensions. Direct inspection
		/// of the bytes to determine the content type is often more accurate
		/// than believing the content type claimed by the {@code http} server.
		/// 
		/// </para>
		/// </summary>
		/// <param name="is">   an input stream that supports marks. </param>
		/// <returns>     a guess at the content type, or {@code null} if none
		///             can be determined. </returns>
		/// <exception cref="IOException">  if an I/O error occurs while reading the
		///               input stream. </exception>
		/// <seealso cref=        java.io.InputStream#mark(int) </seealso>
		/// <seealso cref=        java.io.InputStream#markSupported() </seealso>
		/// <seealso cref=        java.net.URLConnection#getContentType() </seealso>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public static String guessContentTypeFromStream(java.io.InputStream is) throws java.io.IOException
		public static String GuessContentTypeFromStream(InputStream @is)
		{
			// If we can't read ahead safely, just give up on guessing
			if (!@is.MarkSupported())
			{
				return null;
			}

			@is.Mark(16);
			int c1 = @is.Read();
			int c2 = @is.Read();
			int c3 = @is.Read();
			int c4 = @is.Read();
			int c5 = @is.Read();
			int c6 = @is.Read();
			int c7 = @is.Read();
			int c8 = @is.Read();
			int c9 = @is.Read();
			int c10 = @is.Read();
			int c11 = @is.Read();
			int c12 = @is.Read();
			int c13 = @is.Read();
			int c14 = @is.Read();
			int c15 = @is.Read();
			int c16 = @is.Read();
			@is.Reset();

			if (c1 == 0xCA && c2 == 0xFE && c3 == 0xBA && c4 == 0xBE)
			{
				return "application/java-vm";
			}

			if (c1 == 0xAC && c2 == 0xED)
			{
				// next two bytes are version number, currently 0x00 0x05
				return "application/x-java-serialized-object";
			}

			if (c1 == '<')
			{
				if (c2 == '!' || ((c2 == 'h' && (c3 == 't' && c4 == 'm' && c5 == 'l' || c3 == 'e' && c4 == 'a' && c5 == 'd') || (c2 == 'b' && c3 == 'o' && c4 == 'd' && c5 == 'y'))) || ((c2 == 'H' && (c3 == 'T' && c4 == 'M' && c5 == 'L' || c3 == 'E' && c4 == 'A' && c5 == 'D') || (c2 == 'B' && c3 == 'O' && c4 == 'D' && c5 == 'Y'))))
				{
					return "text/html";
				}

				if (c2 == '?' && c3 == 'x' && c4 == 'm' && c5 == 'l' && c6 == ' ')
				{
					return "application/xml";
				}
			}

			// big and little (identical) endian UTF-8 encodings, with BOM
			if (c1 == 0xef && c2 == 0xbb && c3 == 0xbf)
			{
				if (c4 == '<' && c5 == '?' && c6 == 'x')
				{
					return "application/xml";
				}
			}

			// big and little endian UTF-16 encodings, with byte order mark
			if (c1 == 0xfe && c2 == 0xff)
			{
				if (c3 == 0 && c4 == '<' && c5 == 0 && c6 == '?' && c7 == 0 && c8 == 'x')
				{
					return "application/xml";
				}
			}

			if (c1 == 0xff && c2 == 0xfe)
			{
				if (c3 == '<' && c4 == 0 && c5 == '?' && c6 == 0 && c7 == 'x' && c8 == 0)
				{
					return "application/xml";
				}
			}

			// big and little endian UTF-32 encodings, with BOM
			if (c1 == 0x00 && c2 == 0x00 && c3 == 0xfe && c4 == 0xff)
			{
				if (c5 == 0 && c6 == 0 && c7 == 0 && c8 == '<' && c9 == 0 && c10 == 0 && c11 == 0 && c12 == '?' && c13 == 0 && c14 == 0 && c15 == 0 && c16 == 'x')
				{
					return "application/xml";
				}
			}

			if (c1 == 0xff && c2 == 0xfe && c3 == 0x00 && c4 == 0x00)
			{
				if (c5 == '<' && c6 == 0 && c7 == 0 && c8 == 0 && c9 == '?' && c10 == 0 && c11 == 0 && c12 == 0 && c13 == 'x' && c14 == 0 && c15 == 0 && c16 == 0)
				{
					return "application/xml";
				}
			}

			if (c1 == 'G' && c2 == 'I' && c3 == 'F' && c4 == '8')
			{
				return "image/gif";
			}

			if (c1 == '#' && c2 == 'd' && c3 == 'e' && c4 == 'f')
			{
				return "image/x-bitmap";
			}

			if (c1 == '!' && c2 == ' ' && c3 == 'X' && c4 == 'P' && c5 == 'M' && c6 == '2')
			{
				return "image/x-pixmap";
			}

			if (c1 == 137 && c2 == 80 && c3 == 78 && c4 == 71 && c5 == 13 && c6 == 10 && c7 == 26 && c8 == 10)
			{
				return "image/png";
			}

			if (c1 == 0xFF && c2 == 0xD8 && c3 == 0xFF)
			{
				if (c4 == 0xE0)
				{
					return "image/jpeg";
				}

				/// <summary>
				/// File format used by digital cameras to store images.
				/// Exif Format can be read by any application supporting
				/// JPEG. Exif Spec can be found at:
				/// http://www.pima.net/standards/it10/PIMA15740/Exif_2-1.PDF
				/// </summary>
				if ((c4 == 0xE1) && (c7 == 'E' && c8 == 'x' && c9 == 'i' && c10 == 'f' && c11 == 0))
				{
					return "image/jpeg";
				}

				if (c4 == 0xEE)
				{
					return "image/jpg";
				}
			}

			if (c1 == 0xD0 && c2 == 0xCF && c3 == 0x11 && c4 == 0xE0 && c5 == 0xA1 && c6 == 0xB1 && c7 == 0x1A && c8 == 0xE1)
			{

				/* Above is signature of Microsoft Structured Storage.
				 * Below this, could have tests for various SS entities.
				 * For now, just test for FlashPix.
				 */
				if (Checkfpx(@is))
				{
					return "image/vnd.fpx";
				}
			}

			if (c1 == 0x2E && c2 == 0x73 && c3 == 0x6E && c4 == 0x64)
			{
				return "audio/basic"; // .au format, big endian
			}

			if (c1 == 0x64 && c2 == 0x6E && c3 == 0x73 && c4 == 0x2E)
			{
				return "audio/basic"; // .au format, little endian
			}

			if (c1 == 'R' && c2 == 'I' && c3 == 'F' && c4 == 'F')
			{
				/* I don't know if this is official but evidence
				 * suggests that .wav files start with "RIFF" - brown
				 */
				return "audio/x-wav";
			}
			return null;
		}

		/// <summary>
		/// Check for FlashPix image data in InputStream is.  Return true if
		/// the stream has FlashPix data, false otherwise.  Before calling this
		/// method, the stream should have already been checked to be sure it
		/// contains Microsoft Structured Storage data.
		/// </summary>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: private static boolean checkfpx(java.io.InputStream is) throws java.io.IOException
		private static bool Checkfpx(InputStream @is)
		{

			/* Test for FlashPix image data in Microsoft Structured Storage format.
			 * In general, should do this with calls to an SS implementation.
			 * Lacking that, need to dig via offsets to get to the FlashPix
			 * ClassID.  Details:
			 *
			 * Offset to Fpx ClsID from beginning of stream should be:
			 *
			 * FpxClsidOffset = rootEntryOffset + clsidOffset
			 *
			 * where: clsidOffset = 0x50.
			 *        rootEntryOffset = headerSize + sectorSize*sectDirStart
			 *                          + 128*rootEntryDirectory
			 *
			 *        where:  headerSize = 0x200 (always)
			 *                sectorSize = 2 raised to power of uSectorShift,
			 *                             which is found in the header at
			 *                             offset 0x1E.
			 *                sectDirStart = found in the header at offset 0x30.
			 *                rootEntryDirectory = in general, should search for
			 *                                     directory labelled as root.
			 *                                     We will assume value of 0 (i.e.,
			 *                                     rootEntry is in first directory)
			 */

			// Mark the stream so we can reset it. 0x100 is enough for the first
			// few reads, but the mark will have to be reset and set again once
			// the offset to the root directory entry is computed. That offset
			// can be very large and isn't know until the stream has been read from
			@is.Mark(0x100);

			// Get the byte ordering located at 0x1E. 0xFE is Intel,
			// 0xFF is other
			long toSkip = (long)0x1C;
			long posn;

			if ((posn = SkipForward(@is, toSkip)) < toSkip)
			{
			  @is.Reset();
			  return false;
			}

			int[] c = new int[16];
			if (ReadBytes(c, 2, @is) < 0)
			{
				@is.Reset();
				return false;
			}

			int byteOrder = c[0];

			posn += 2;
			int uSectorShift;
			if (ReadBytes(c, 2, @is) < 0)
			{
				@is.Reset();
				return false;
			}

			if (byteOrder == 0xFE)
			{
				uSectorShift = c[0];
				uSectorShift += c[1] << 8;
			}
			else
			{
				uSectorShift = c[0] << 8;
				uSectorShift += c[1];
			}

			posn += 2;
			toSkip = (long)0x30 - posn;
			long skipped = 0;
			if ((skipped = SkipForward(@is, toSkip)) < toSkip)
			{
			  @is.Reset();
			  return false;
			}
			posn += skipped;

			if (ReadBytes(c, 4, @is) < 0)
			{
				@is.Reset();
				return false;
			}

			int sectDirStart;
			if (byteOrder == 0xFE)
			{
				sectDirStart = c[0];
				sectDirStart += c[1] << 8;
				sectDirStart += c[2] << 16;
				sectDirStart += c[3] << 24;
			}
			else
			{
				sectDirStart = c[0] << 24;
				sectDirStart += c[1] << 16;
				sectDirStart += c[2] << 8;
				sectDirStart += c[3];
			}
			posn += 4;
			@is.Reset(); // Reset back to the beginning

			toSkip = 0x200L + (long)(1 << uSectorShift) * sectDirStart + 0x50L;

			// Sanity check!
			if (toSkip < 0)
			{
				return false;
			}

			/*
			 * How far can we skip? Is there any performance problem here?
			 * This skip can be fairly long, at least 0x4c650 in at least
			 * one case. Have to assume that the skip will fit in an int.
			 * Leave room to read whole root dir
			 */
			@is.Mark((int)toSkip + 0x30);

			if ((SkipForward(@is, toSkip)) < toSkip)
			{
				@is.Reset();
				return false;
			}

			/* should be at beginning of ClassID, which is as follows
			 * (in Intel byte order):
			 *    00 67 61 56 54 C1 CE 11 85 53 00 AA 00 A1 F9 5B
			 *
			 * This is stored from Windows as long,short,short,char[8]
			 * so for byte order changes, the order only changes for
			 * the first 8 bytes in the ClassID.
			 *
			 * Test against this, ignoring second byte (Intel) since
			 * this could change depending on part of Fpx file we have.
			 */

			if (ReadBytes(c, 16, @is) < 0)
			{
				@is.Reset();
				return false;
			}

			// intel byte order
			if (byteOrder == 0xFE && c[0] == 0x00 && c[2] == 0x61 && c[3] == 0x56 && c[4] == 0x54 && c[5] == 0xC1 && c[6] == 0xCE && c[7] == 0x11 && c[8] == 0x85 && c[9] == 0x53 && c[10] == 0x00 && c[11] == 0xAA && c[12] == 0x00 && c[13] == 0xA1 && c[14] == 0xF9 && c[15] == 0x5B)
			{
				@is.Reset();
				return true;
			}

			// non-intel byte order
			else if (c[3] == 0x00 && c[1] == 0x61 && c[0] == 0x56 && c[5] == 0x54 && c[4] == 0xC1 && c[7] == 0xCE && c[6] == 0x11 && c[8] == 0x85 && c[9] == 0x53 && c[10] == 0x00 && c[11] == 0xAA && c[12] == 0x00 && c[13] == 0xA1 && c[14] == 0xF9 && c[15] == 0x5B)
			{
				@is.Reset();
				return true;
			}
			@is.Reset();
			return false;
		}

		/// <summary>
		/// Tries to read the specified number of bytes from the stream
		/// Returns -1, If EOF is reached before len bytes are read, returns 0
		/// otherwise
		/// </summary>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: private static int readBytes(int c[] , int len, java.io.InputStream is) throws java.io.IOException
		private static int ReadBytes(int[] c, int len, InputStream @is)
		{

			sbyte[] buf = new sbyte[len];
			if (@is.Read(buf, 0, len) < len)
			{
				return -1;
			}

			// fill the passed in int array
			for (int i = 0; i < len; i++)
			{
				 c[i] = buf[i] & 0xff;
			}
			return 0;
		}


		/// <summary>
		/// Skips through the specified number of bytes from the stream
		/// until either EOF is reached, or the specified
		/// number of bytes have been skipped
		/// </summary>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: private static long skipForward(java.io.InputStream is, long toSkip) throws java.io.IOException
		private static long SkipForward(InputStream @is, long toSkip)
		{

			long eachSkip = 0;
			long skipped = 0;

			while (skipped != toSkip)
			{
				eachSkip = @is.Skip(toSkip - skipped);

				// check if EOF is reached
				if (eachSkip <= 0)
				{
					if (@is.Read() == -1)
					{
						return skipped;
					}
					else
					{
						skipped++;
					}
				}
				skipped += eachSkip;
			}
			return skipped;
		}

	}


	internal class UnknownContentHandler : ContentHandler
	{
		internal static readonly ContentHandler INSTANCE = new UnknownContentHandler();

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public Object getContent(URLConnection uc) throws java.io.IOException
		public override Object GetContent(URLConnection uc)
		{
			return uc.InputStream;
		}
	}

}
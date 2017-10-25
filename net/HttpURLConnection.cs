using System;

/*
 * Copyright (c) 1996, 2013, Oracle and/or its affiliates. All rights reserved.
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
	/// A URLConnection with support for HTTP-specific features. See
	/// <A HREF="http://www.w3.org/pub/WWW/Protocols/"> the spec </A> for
	/// details.
	/// <para>
	/// 
	/// Each HttpURLConnection instance is used to make a single request
	/// but the underlying network connection to the HTTP server may be
	/// transparently shared by other instances. Calling the close() methods
	/// on the InputStream or OutputStream of an HttpURLConnection
	/// after a request may free network resources associated with this
	/// instance but has no effect on any shared persistent connection.
	/// Calling the disconnect() method may close the underlying socket
	/// if a persistent connection is otherwise idle at that time.
	/// 
	/// <P>The HTTP protocol handler has a few settings that can be accessed through
	/// System Properties. This covers
	/// <a href="doc-files/net-properties.html#Proxies">Proxy settings</a> as well as
	/// <a href="doc-files/net-properties.html#MiscHTTP"> various other settings</a>.
	/// </P>
	/// </para>
	/// <para>
	/// <b>Security permissions</b>
	/// </para>
	/// <para>
	/// If a security manager is installed, and if a method is called which results in an
	/// attempt to open a connection, the caller must possess either:-
	/// <ul><li>a "connect" <seealso cref="SocketPermission"/> to the host/port combination of the
	/// destination URL or</li>
	/// <li>a <seealso cref="URLPermission"/> that permits this request.</li>
	/// </para>
	/// </ul><para>
	/// If automatic redirection is enabled, and this request is redirected to another
	/// destination, then the caller must also have permission to connect to the
	/// redirected host/URL.
	/// 
	/// </para>
	/// </summary>
	/// <seealso cref=     java.net.HttpURLConnection#disconnect()
	/// @since JDK1.1 </seealso>
	public abstract class HttpURLConnection : URLConnection
	{
		/* instance variables */

		/// <summary>
		/// The HTTP method (GET,POST,PUT,etc.).
		/// </summary>
		protected internal String Method = "GET";

		/// <summary>
		/// The chunk-length when using chunked encoding streaming mode for output.
		/// A value of {@code -1} means chunked encoding is disabled for output.
		/// @since 1.5
		/// </summary>
		protected internal int ChunkLength = -1;

		/// <summary>
		/// The fixed content-length when using fixed-length streaming mode.
		/// A value of {@code -1} means fixed-length streaming mode is disabled
		/// for output.
		/// 
		/// <P> <B>NOTE:</B> <seealso cref="#fixedContentLengthLong"/> is recommended instead
		/// of this field, as it allows larger content lengths to be set.
		/// 
		/// @since 1.5
		/// </summary>
		protected internal int FixedContentLength = -1;

		/// <summary>
		/// The fixed content-length when using fixed-length streaming mode.
		/// A value of {@code -1} means fixed-length streaming mode is disabled
		/// for output.
		/// 
		/// @since 1.7
		/// </summary>
		protected internal long FixedContentLengthLong = -1;

		/// <summary>
		/// Returns the key for the {@code n}<sup>th</sup> header field.
		/// Some implementations may treat the {@code 0}<sup>th</sup>
		/// header field as special, i.e. as the status line returned by the HTTP
		/// server. In this case, <seealso cref="#getHeaderField(int) getHeaderField(0)"/> returns the status
		/// line, but {@code getHeaderFieldKey(0)} returns null.
		/// </summary>
		/// <param name="n">   an index, where {@code n >=0}. </param>
		/// <returns>  the key for the {@code n}<sup>th</sup> header field,
		///          or {@code null} if the key does not exist. </returns>
		public override String GetHeaderFieldKey(int n)
		{
			return null;
		}

		/// <summary>
		/// This method is used to enable streaming of a HTTP request body
		/// without internal buffering, when the content length is known in
		/// advance.
		/// <para>
		/// An exception will be thrown if the application
		/// attempts to write more data than the indicated
		/// content-length, or if the application closes the OutputStream
		/// before writing the indicated amount.
		/// </para>
		/// <para>
		/// When output streaming is enabled, authentication
		/// and redirection cannot be handled automatically.
		/// A HttpRetryException will be thrown when reading
		/// the response if authentication or redirection are required.
		/// This exception can be queried for the details of the error.
		/// </para>
		/// <para>
		/// This method must be called before the URLConnection is connected.
		/// </para>
		/// <para>
		/// <B>NOTE:</B> <seealso cref="#setFixedLengthStreamingMode(long)"/> is recommended
		/// instead of this method as it allows larger content lengths to be set.
		/// 
		/// </para>
		/// </summary>
		/// <param name="contentLength"> The number of bytes which will be written
		///          to the OutputStream.
		/// </param>
		/// <exception cref="IllegalStateException"> if URLConnection is already connected
		///          or if a different streaming mode is already enabled.
		/// </exception>
		/// <exception cref="IllegalArgumentException"> if a content length less than
		///          zero is specified.
		/// </exception>
		/// <seealso cref=     #setChunkedStreamingMode(int)
		/// @since 1.5 </seealso>
		public virtual int FixedLengthStreamingMode
		{
			set
			{
				if (Connected)
				{
					throw new IllegalStateException("Already connected");
				}
				if (ChunkLength != -1)
				{
					throw new IllegalStateException("Chunked encoding streaming mode set");
				}
				if (value < 0)
				{
					throw new IllegalArgumentException("invalid content length");
				}
				FixedContentLength = value;
			}
		}

		/// <summary>
		/// This method is used to enable streaming of a HTTP request body
		/// without internal buffering, when the content length is known in
		/// advance.
		/// 
		/// <P> An exception will be thrown if the application attempts to write
		/// more data than the indicated content-length, or if the application
		/// closes the OutputStream before writing the indicated amount.
		/// 
		/// <P> When output streaming is enabled, authentication and redirection
		/// cannot be handled automatically. A <seealso cref="HttpRetryException"/> will
		/// be thrown when reading the response if authentication or redirection
		/// are required. This exception can be queried for the details of the
		/// error.
		/// 
		/// <P> This method must be called before the URLConnection is connected.
		/// 
		/// <P> The content length set by invoking this method takes precedence
		/// over any value set by <seealso cref="#setFixedLengthStreamingMode(int)"/>.
		/// </summary>
		/// <param name="contentLength">
		///         The number of bytes which will be written to the OutputStream.
		/// </param>
		/// <exception cref="IllegalStateException">
		///          if URLConnection is already connected or if a different
		///          streaming mode is already enabled.
		/// </exception>
		/// <exception cref="IllegalArgumentException">
		///          if a content length less than zero is specified.
		/// 
		/// @since 1.7 </exception>
		public virtual long FixedLengthStreamingMode
		{
			set
			{
				if (Connected)
				{
					throw new IllegalStateException("Already connected");
				}
				if (ChunkLength != -1)
				{
					throw new IllegalStateException("Chunked encoding streaming mode set");
				}
				if (value < 0)
				{
					throw new IllegalArgumentException("invalid content length");
				}
				FixedContentLengthLong = value;
			}
		}

		/* Default chunk size (including chunk header) if not specified;
		 * we want to keep this in sync with the one defined in
		 * sun.net.www.http.ChunkedOutputStream
		 */
		private const int DEFAULT_CHUNK_SIZE = 4096;

		/// <summary>
		/// This method is used to enable streaming of a HTTP request body
		/// without internal buffering, when the content length is <b>not</b>
		/// known in advance. In this mode, chunked transfer encoding
		/// is used to send the request body. Note, not all HTTP servers
		/// support this mode.
		/// <para>
		/// When output streaming is enabled, authentication
		/// and redirection cannot be handled automatically.
		/// A HttpRetryException will be thrown when reading
		/// the response if authentication or redirection are required.
		/// This exception can be queried for the details of the error.
		/// </para>
		/// <para>
		/// This method must be called before the URLConnection is connected.
		/// 
		/// </para>
		/// </summary>
		/// <param name="chunklen"> The number of bytes to write in each chunk.
		///          If chunklen is less than or equal to zero, a default
		///          value will be used.
		/// </param>
		/// <exception cref="IllegalStateException"> if URLConnection is already connected
		///          or if a different streaming mode is already enabled.
		/// </exception>
		/// <seealso cref=     #setFixedLengthStreamingMode(int)
		/// @since 1.5 </seealso>
		public virtual int ChunkedStreamingMode
		{
			set
			{
				if (Connected)
				{
					throw new IllegalStateException("Can't set streaming mode: already connected");
				}
				if (FixedContentLength != -1 || FixedContentLengthLong != -1)
				{
					throw new IllegalStateException("Fixed length streaming mode set");
				}
				ChunkLength = value <= 0? DEFAULT_CHUNK_SIZE : value;
			}
		}

		/// <summary>
		/// Returns the value for the {@code n}<sup>th</sup> header field.
		/// Some implementations may treat the {@code 0}<sup>th</sup>
		/// header field as special, i.e. as the status line returned by the HTTP
		/// server.
		/// <para>
		/// This method can be used in conjunction with the
		/// <seealso cref="#getHeaderFieldKey getHeaderFieldKey"/> method to iterate through all
		/// the headers in the message.
		/// 
		/// </para>
		/// </summary>
		/// <param name="n">   an index, where {@code n>=0}. </param>
		/// <returns>  the value of the {@code n}<sup>th</sup> header field,
		///          or {@code null} if the value does not exist. </returns>
		/// <seealso cref=     java.net.HttpURLConnection#getHeaderFieldKey(int) </seealso>
		public override String GetHeaderField(int n)
		{
			return null;
		}

		/// <summary>
		/// An {@code int} representing the three digit HTTP Status-Code.
		/// <ul>
		/// <li> 1xx: Informational
		/// <li> 2xx: Success
		/// <li> 3xx: Redirection
		/// <li> 4xx: Client Error
		/// <li> 5xx: Server Error
		/// </ul>
		/// </summary>
		protected internal int ResponseCode_Renamed = -1;

		/// <summary>
		/// The HTTP response message.
		/// </summary>
		protected internal String ResponseMessage_Renamed = null;

		/* static variables */

		/* do we automatically follow redirects? The default is true. */
		private static bool FollowRedirects_Renamed = true;

		/// <summary>
		/// If {@code true}, the protocol will automatically follow redirects.
		/// If {@code false}, the protocol will not automatically follow
		/// redirects.
		/// <para>
		/// This field is set by the {@code setInstanceFollowRedirects}
		/// method. Its value is returned by the {@code getInstanceFollowRedirects}
		/// method.
		/// </para>
		/// <para>
		/// Its default value is based on the value of the static followRedirects
		/// at HttpURLConnection construction time.
		/// 
		/// </para>
		/// </summary>
		/// <seealso cref=     java.net.HttpURLConnection#setInstanceFollowRedirects(boolean) </seealso>
		/// <seealso cref=     java.net.HttpURLConnection#getInstanceFollowRedirects() </seealso>
		/// <seealso cref=     java.net.HttpURLConnection#setFollowRedirects(boolean) </seealso>
		protected internal bool InstanceFollowRedirects_Renamed = FollowRedirects_Renamed;

		/* valid HTTP methods */
		private static readonly String[] Methods = new String[] {"GET", "POST", "HEAD", "OPTIONS", "PUT", "DELETE", "TRACE"};

		/// <summary>
		/// Constructor for the HttpURLConnection. </summary>
		/// <param name="u"> the URL </param>
		protected internal HttpURLConnection(URL u) : base(u)
		{
		}

		/// <summary>
		/// Sets whether HTTP redirects  (requests with response code 3xx) should
		/// be automatically followed by this class.  True by default.  Applets
		/// cannot change this variable.
		/// <para>
		/// If there is a security manager, this method first calls
		/// the security manager's {@code checkSetFactory} method
		/// to ensure the operation is allowed.
		/// This could result in a SecurityException.
		/// 
		/// </para>
		/// </summary>
		/// <param name="set"> a {@code boolean} indicating whether or not
		/// to follow HTTP redirects. </param>
		/// <exception cref="SecurityException">  if a security manager exists and its
		///             {@code checkSetFactory} method doesn't
		///             allow the operation. </exception>
		/// <seealso cref=        SecurityManager#checkSetFactory </seealso>
		/// <seealso cref= #getFollowRedirects() </seealso>
		public static bool FollowRedirects
		{
			set
			{
				SecurityManager sec = System.SecurityManager;
				if (sec != null)
				{
					// seems to be the best check here...
					sec.CheckSetFactory();
				}
				FollowRedirects_Renamed = value;
			}
			get
			{
				return FollowRedirects_Renamed;
			}
		}


		/// <summary>
		/// Sets whether HTTP redirects (requests with response code 3xx) should
		/// be automatically followed by this {@code HttpURLConnection}
		/// instance.
		/// <para>
		/// The default value comes from followRedirects, which defaults to
		/// true.
		/// 
		/// </para>
		/// </summary>
		/// <param name="followRedirects"> a {@code boolean} indicating
		/// whether or not to follow HTTP redirects.
		/// </param>
		/// <seealso cref=    java.net.HttpURLConnection#instanceFollowRedirects </seealso>
		/// <seealso cref= #getInstanceFollowRedirects
		/// @since 1.3 </seealso>
		 public virtual bool InstanceFollowRedirects
		 {
			 set
			 {
				InstanceFollowRedirects_Renamed = value;
			 }
			 get
			 {
				 return InstanceFollowRedirects_Renamed;
			 }
		 }


		/// <summary>
		/// Set the method for the URL request, one of:
		/// <UL>
		///  <LI>GET
		///  <LI>POST
		///  <LI>HEAD
		///  <LI>OPTIONS
		///  <LI>PUT
		///  <LI>DELETE
		///  <LI>TRACE
		/// </UL> are legal, subject to protocol restrictions.  The default
		/// method is GET.
		/// </summary>
		/// <param name="method"> the HTTP method </param>
		/// <exception cref="ProtocolException"> if the method cannot be reset or if
		///              the requested method isn't valid for HTTP. </exception>
		/// <exception cref="SecurityException"> if a security manager is set and the
		///              method is "TRACE", but the "allowHttpTrace"
		///              NetPermission is not granted. </exception>
		/// <seealso cref= #getRequestMethod() </seealso>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public void setRequestMethod(String method) throws ProtocolException
		public virtual String RequestMethod
		{
			set
			{
				if (Connected)
				{
					throw new ProtocolException("Can't reset method: already connected");
				}
				// This restriction will prevent people from using this class to
				// experiment w/ new HTTP methods using java.  But it should
				// be placed for security - the request String could be
				// arbitrarily long.
    
				for (int i = 0; i < Methods.Length; i++)
				{
					if (Methods[i].Equals(value))
					{
						if (value.Equals("TRACE"))
						{
							SecurityManager s = System.SecurityManager;
							if (s != null)
							{
								s.CheckPermission(new NetPermission("allowHttpTrace"));
							}
						}
						this.Method = value;
						return;
					}
				}
				throw new ProtocolException("Invalid HTTP method: " + value);
			}
			get
			{
				return Method;
			}
		}


		/// <summary>
		/// Gets the status code from an HTTP response message.
		/// For example, in the case of the following status lines:
		/// <PRE>
		/// HTTP/1.0 200 OK
		/// HTTP/1.0 401 Unauthorized
		/// </PRE>
		/// It will return 200 and 401 respectively.
		/// Returns -1 if no code can be discerned
		/// from the response (i.e., the response is not valid HTTP). </summary>
		/// <exception cref="IOException"> if an error occurred connecting to the server. </exception>
		/// <returns> the HTTP Status-Code, or -1 </returns>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public int getResponseCode() throws java.io.IOException
		public virtual int ResponseCode
		{
			get
			{
				/*
				 * We're got the response code already
				 */
				if (ResponseCode_Renamed != -1)
				{
					return ResponseCode_Renamed;
				}
    
				/*
				 * Ensure that we have connected to the server. Record
				 * exception as we need to re-throw it if there isn't
				 * a status line.
				 */
				Exception exc = null;
				try
				{
					InputStream;
				}
				catch (Exception e)
				{
					exc = e;
				}
    
				/*
				 * If we can't a status-line then re-throw any exception
				 * that getInputStream threw.
				 */
				String statusLine = GetHeaderField(0);
				if (statusLine == null)
				{
					if (exc != null)
					{
						if (exc is RuntimeException)
						{
							throw (RuntimeException)exc;
						}
						else
						{
							throw (IOException)exc;
						}
					}
					return -1;
				}
    
				/*
				 * Examine the status-line - should be formatted as per
				 * section 6.1 of RFC 2616 :-
				 *
				 * Status-Line = HTTP-Version SP Status-Code SP Reason-Phrase
				 *
				 * If status line can't be parsed return -1.
				 */
				if (statusLine.StartsWith("HTTP/1."))
				{
					int codePos = statusLine.IndexOf(' ');
					if (codePos > 0)
					{
    
						int phrasePos = statusLine.IndexOf(' ', codePos + 1);
						if (phrasePos > 0 && phrasePos < statusLine.Length())
						{
							ResponseMessage_Renamed = statusLine.Substring(phrasePos + 1);
						}
    
						// deviation from RFC 2616 - don't reject status line
						// if SP Reason-Phrase is not included.
						if (phrasePos < 0)
						{
							phrasePos = statusLine.Length();
						}
    
						try
						{
							ResponseCode_Renamed = Convert.ToInt32(StringHelperClass.SubstringSpecial(statusLine, codePos + 1, phrasePos));
							return ResponseCode_Renamed;
						}
						catch (NumberFormatException)
						{
						}
					}
				}
				return -1;
			}
		}

		/// <summary>
		/// Gets the HTTP response message, if any, returned along with the
		/// response code from a server.  From responses like:
		/// <PRE>
		/// HTTP/1.0 200 OK
		/// HTTP/1.0 404 Not Found
		/// </PRE>
		/// Extracts the Strings "OK" and "Not Found" respectively.
		/// Returns null if none could be discerned from the responses
		/// (the result was not valid HTTP). </summary>
		/// <exception cref="IOException"> if an error occurred connecting to the server. </exception>
		/// <returns> the HTTP response message, or {@code null} </returns>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public String getResponseMessage() throws java.io.IOException
		public virtual String ResponseMessage
		{
			get
			{
				ResponseCode;
				return ResponseMessage_Renamed;
			}
		}

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @SuppressWarnings("deprecation") public long getHeaderFieldDate(String name, long Default)
		public override long GetHeaderFieldDate(String name, long Default)
		{
			String dateString = GetHeaderField(name);
			try
			{
				if (dateString.IndexOf("GMT") == -1)
				{
					dateString = dateString + " GMT";
				}
				return DateTime.Parse(dateString);
			}
			catch (Exception)
			{
			}
			return Default;
		}


		/// <summary>
		/// Indicates that other requests to the server
		/// are unlikely in the near future. Calling disconnect()
		/// should not imply that this HttpURLConnection
		/// instance can be reused for other requests.
		/// </summary>
		public abstract void Disconnect();

		/// <summary>
		/// Indicates if the connection is going through a proxy. </summary>
		/// <returns> a boolean indicating if the connection is
		/// using a proxy. </returns>
		public abstract bool UsingProxy();

		/// <summary>
		/// Returns a <seealso cref="SocketPermission"/> object representing the
		/// permission necessary to connect to the destination host and port.
		/// </summary>
		/// <exception cref="IOException"> if an error occurs while computing
		///            the permission.
		/// </exception>
		/// <returns> a {@code SocketPermission} object representing the
		///         permission necessary to connect to the destination
		///         host and port. </returns>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public java.security.Permission getPermission() throws java.io.IOException
		public override Permission Permission
		{
			get
			{
				int port = Url.Port;
				port = port < 0 ? 80 : port;
				String host = Url.Host + ":" + port;
				Permission permission = new SocketPermission(host, "connect");
				return permission;
			}
		}

	   /// <summary>
	   /// Returns the error stream if the connection failed
	   /// but the server sent useful data nonetheless. The
	   /// typical example is when an HTTP server responds
	   /// with a 404, which will cause a FileNotFoundException
	   /// to be thrown in connect, but the server sent an HTML
	   /// help page with suggestions as to what to do.
	   /// 
	   /// <para>This method will not cause a connection to be initiated.  If
	   /// the connection was not connected, or if the server did not have
	   /// an error while connecting or if the server had an error but
	   /// no error data was sent, this method will return null. This is
	   /// the default.
	   /// 
	   /// </para>
	   /// </summary>
	   /// <returns> an error stream if any, null if there have been no
	   /// errors, the connection is not connected or the server sent no
	   /// useful data. </returns>
		public virtual InputStream ErrorStream
		{
			get
			{
				return null;
			}
		}

		/// <summary>
		/// The response codes for HTTP, as of version 1.1.
		/// </summary>

		// REMIND: do we want all these??
		// Others not here that we do want??

		/* 2XX: generally "OK" */

		/// <summary>
		/// HTTP Status-Code 200: OK.
		/// </summary>
		public const int HTTP_OK = 200;

		/// <summary>
		/// HTTP Status-Code 201: Created.
		/// </summary>
		public const int HTTP_CREATED = 201;

		/// <summary>
		/// HTTP Status-Code 202: Accepted.
		/// </summary>
		public const int HTTP_ACCEPTED = 202;

		/// <summary>
		/// HTTP Status-Code 203: Non-Authoritative Information.
		/// </summary>
		public const int HTTP_NOT_AUTHORITATIVE = 203;

		/// <summary>
		/// HTTP Status-Code 204: No Content.
		/// </summary>
		public const int HTTP_NO_CONTENT = 204;

		/// <summary>
		/// HTTP Status-Code 205: Reset Content.
		/// </summary>
		public const int HTTP_RESET = 205;

		/// <summary>
		/// HTTP Status-Code 206: Partial Content.
		/// </summary>
		public const int HTTP_PARTIAL = 206;

		/* 3XX: relocation/redirect */

		/// <summary>
		/// HTTP Status-Code 300: Multiple Choices.
		/// </summary>
		public const int HTTP_MULT_CHOICE = 300;

		/// <summary>
		/// HTTP Status-Code 301: Moved Permanently.
		/// </summary>
		public const int HTTP_MOVED_PERM = 301;

		/// <summary>
		/// HTTP Status-Code 302: Temporary Redirect.
		/// </summary>
		public const int HTTP_MOVED_TEMP = 302;

		/// <summary>
		/// HTTP Status-Code 303: See Other.
		/// </summary>
		public const int HTTP_SEE_OTHER = 303;

		/// <summary>
		/// HTTP Status-Code 304: Not Modified.
		/// </summary>
		public const int HTTP_NOT_MODIFIED = 304;

		/// <summary>
		/// HTTP Status-Code 305: Use Proxy.
		/// </summary>
		public const int HTTP_USE_PROXY = 305;

		/* 4XX: client error */

		/// <summary>
		/// HTTP Status-Code 400: Bad Request.
		/// </summary>
		public const int HTTP_BAD_REQUEST = 400;

		/// <summary>
		/// HTTP Status-Code 401: Unauthorized.
		/// </summary>
		public const int HTTP_UNAUTHORIZED = 401;

		/// <summary>
		/// HTTP Status-Code 402: Payment Required.
		/// </summary>
		public const int HTTP_PAYMENT_REQUIRED = 402;

		/// <summary>
		/// HTTP Status-Code 403: Forbidden.
		/// </summary>
		public const int HTTP_FORBIDDEN = 403;

		/// <summary>
		/// HTTP Status-Code 404: Not Found.
		/// </summary>
		public const int HTTP_NOT_FOUND = 404;

		/// <summary>
		/// HTTP Status-Code 405: Method Not Allowed.
		/// </summary>
		public const int HTTP_BAD_METHOD = 405;

		/// <summary>
		/// HTTP Status-Code 406: Not Acceptable.
		/// </summary>
		public const int HTTP_NOT_ACCEPTABLE = 406;

		/// <summary>
		/// HTTP Status-Code 407: Proxy Authentication Required.
		/// </summary>
		public const int HTTP_PROXY_AUTH = 407;

		/// <summary>
		/// HTTP Status-Code 408: Request Time-Out.
		/// </summary>
		public const int HTTP_CLIENT_TIMEOUT = 408;

		/// <summary>
		/// HTTP Status-Code 409: Conflict.
		/// </summary>
		public const int HTTP_CONFLICT = 409;

		/// <summary>
		/// HTTP Status-Code 410: Gone.
		/// </summary>
		public const int HTTP_GONE = 410;

		/// <summary>
		/// HTTP Status-Code 411: Length Required.
		/// </summary>
		public const int HTTP_LENGTH_REQUIRED = 411;

		/// <summary>
		/// HTTP Status-Code 412: Precondition Failed.
		/// </summary>
		public const int HTTP_PRECON_FAILED = 412;

		/// <summary>
		/// HTTP Status-Code 413: Request Entity Too Large.
		/// </summary>
		public const int HTTP_ENTITY_TOO_LARGE = 413;

		/// <summary>
		/// HTTP Status-Code 414: Request-URI Too Large.
		/// </summary>
		public const int HTTP_REQ_TOO_LONG = 414;

		/// <summary>
		/// HTTP Status-Code 415: Unsupported Media Type.
		/// </summary>
		public const int HTTP_UNSUPPORTED_TYPE = 415;

		/* 5XX: server error */

		/// <summary>
		/// HTTP Status-Code 500: Internal Server Error. </summary>
		/// @deprecated   it is misplaced and shouldn't have existed. 
		[Obsolete("  it is misplaced and shouldn't have existed.")]
		public const int HTTP_SERVER_ERROR = 500;

		/// <summary>
		/// HTTP Status-Code 500: Internal Server Error.
		/// </summary>
		public const int HTTP_INTERNAL_ERROR = 500;

		/// <summary>
		/// HTTP Status-Code 501: Not Implemented.
		/// </summary>
		public const int HTTP_NOT_IMPLEMENTED = 501;

		/// <summary>
		/// HTTP Status-Code 502: Bad Gateway.
		/// </summary>
		public const int HTTP_BAD_GATEWAY = 502;

		/// <summary>
		/// HTTP Status-Code 503: Service Unavailable.
		/// </summary>
		public const int HTTP_UNAVAILABLE = 503;

		/// <summary>
		/// HTTP Status-Code 504: Gateway Timeout.
		/// </summary>
		public const int HTTP_GATEWAY_TIMEOUT = 504;

		/// <summary>
		/// HTTP Status-Code 505: HTTP Version Not Supported.
		/// </summary>
		public const int HTTP_VERSION = 505;

	}

}
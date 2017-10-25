/*
 * Copyright (c) 2004, 2013, Oracle and/or its affiliates. All rights reserved.
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
	/// Thrown to indicate that a HTTP request needs to be retried
	/// but cannot be retried automatically, due to streaming mode
	/// being enabled.
	/// 
	/// @author  Michael McMahon
	/// @since   1.5
	/// </summary>
	public class HttpRetryException : IOException
	{
		private new const long SerialVersionUID = -9186022286469111381L;

		private int ResponseCode_Renamed;
		private String Location_Renamed;

		/// <summary>
		/// Constructs a new {@code HttpRetryException} from the
		/// specified response code and exception detail message
		/// </summary>
		/// <param name="detail">   the detail message. </param>
		/// <param name="code">   the HTTP response code from server. </param>
		public HttpRetryException(String detail, int code) : base(detail)
		{
			ResponseCode_Renamed = code;
		}

		/// <summary>
		/// Constructs a new {@code HttpRetryException} with detail message
		/// responseCode and the contents of the Location response header field.
		/// </summary>
		/// <param name="detail">   the detail message. </param>
		/// <param name="code">   the HTTP response code from server. </param>
		/// <param name="location">   the URL to be redirected to </param>
		public HttpRetryException(String detail, int code, String location) : base(detail)
		{
			ResponseCode_Renamed = code;
			this.Location_Renamed = location;
		}

		/// <summary>
		/// Returns the http response code
		/// </summary>
		/// <returns>  The http response code. </returns>
		public virtual int ResponseCode()
		{
			return ResponseCode_Renamed;
		}

		/// <summary>
		/// Returns a string explaining why the http request could
		/// not be retried.
		/// </summary>
		/// <returns>  The reason string </returns>
		public virtual String Reason
		{
			get
			{
				return base.Message;
			}
		}

		/// <summary>
		/// Returns the value of the Location header field if the
		/// error resulted from redirection.
		/// </summary>
		/// <returns> The location string </returns>
		public virtual String Location
		{
			get
			{
				return Location_Renamed;
			}
		}
	}

}
using System;

/*
 * Copyright (c) 2000, 2013, Oracle and/or its affiliates. All rights reserved.
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


namespace java.util.logging
{

	/// <summary>
	/// A <tt>Handler</tt> object takes log messages from a <tt>Logger</tt> and
	/// exports them.  It might for example, write them to a console
	/// or write them to a file, or send them to a network logging service,
	/// or forward them to an OS log, or whatever.
	/// <para>
	/// A <tt>Handler</tt> can be disabled by doing a <tt>setLevel(Level.OFF)</tt>
	/// and can  be re-enabled by doing a <tt>setLevel</tt> with an appropriate level.
	/// </para>
	/// <para>
	/// <tt>Handler</tt> classes typically use <tt>LogManager</tt> properties to set
	/// default values for the <tt>Handler</tt>'s <tt>Filter</tt>, <tt>Formatter</tt>,
	/// and <tt>Level</tt>.  See the specific documentation for each concrete
	/// <tt>Handler</tt> class.
	/// 
	/// 
	/// @since 1.4
	/// </para>
	/// </summary>

	public abstract class Handler
	{
		private static readonly int OffValue = Level.OFF.IntValue();
		private readonly LogManager Manager = LogManager.LogManager;

		// We're using volatile here to avoid synchronizing getters, which
		// would prevent other threads from calling isLoggable()
		// while publish() is executing.
		// On the other hand, setters will be synchronized to exclude concurrent
		// execution with more complex methods, such as StreamHandler.publish().
		// We wouldn't want 'level' to be changed by another thread in the middle
		// of the execution of a 'publish' call.
		private volatile Filter Filter_Renamed;
		private volatile Formatter Formatter_Renamed;
		private volatile Level LogLevel = Level.ALL;
		private volatile ErrorManager ErrorManager_Renamed = new ErrorManager();
		private volatile String Encoding_Renamed;

		// Package private support for security checking.  When sealed
		// is true, we access check updates to the class.
		internal bool @sealed = true;

		/// <summary>
		/// Default constructor.  The resulting <tt>Handler</tt> has a log
		/// level of <tt>Level.ALL</tt>, no <tt>Formatter</tt>, and no
		/// <tt>Filter</tt>.  A default <tt>ErrorManager</tt> instance is installed
		/// as the <tt>ErrorManager</tt>.
		/// </summary>
		protected internal Handler()
		{
		}

		/// <summary>
		/// Publish a <tt>LogRecord</tt>.
		/// <para>
		/// The logging request was made initially to a <tt>Logger</tt> object,
		/// which initialized the <tt>LogRecord</tt> and forwarded it here.
		/// </para>
		/// <para>
		/// The <tt>Handler</tt>  is responsible for formatting the message, when and
		/// if necessary.  The formatting should include localization.
		/// 
		/// </para>
		/// </summary>
		/// <param name="record">  description of the log event. A null record is
		///                 silently ignored and is not published </param>
		public abstract void Publish(LogRecord record);

		/// <summary>
		/// Flush any buffered output.
		/// </summary>
		public abstract void Flush();

		/// <summary>
		/// Close the <tt>Handler</tt> and free all associated resources.
		/// <para>
		/// The close method will perform a <tt>flush</tt> and then close the
		/// <tt>Handler</tt>.   After close has been called this <tt>Handler</tt>
		/// should no longer be used.  Method calls may either be silently
		/// ignored or may throw runtime exceptions.
		/// 
		/// </para>
		/// </summary>
		/// <exception cref="SecurityException">  if a security manager exists and if
		///             the caller does not have <tt>LoggingPermission("control")</tt>. </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public abstract void close() throws SecurityException;
		public abstract void Close();

		/// <summary>
		/// Set a <tt>Formatter</tt>.  This <tt>Formatter</tt> will be used
		/// to format <tt>LogRecords</tt> for this <tt>Handler</tt>.
		/// <para>
		/// Some <tt>Handlers</tt> may not use <tt>Formatters</tt>, in
		/// which case the <tt>Formatter</tt> will be remembered, but not used.
		/// </para>
		/// <para>
		/// </para>
		/// </summary>
		/// <param name="newFormatter"> the <tt>Formatter</tt> to use (may not be null) </param>
		/// <exception cref="SecurityException">  if a security manager exists and if
		///             the caller does not have <tt>LoggingPermission("control")</tt>. </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public synchronized void setFormatter(Formatter newFormatter) throws SecurityException
		public virtual Formatter Formatter
		{
			set
			{
				lock (this)
				{
					CheckPermission();
					// Check for a null pointer:
					value.GetType();
					Formatter_Renamed = value;
				}
			}
			get
			{
				return Formatter_Renamed;
			}
		}


		/// <summary>
		/// Set the character encoding used by this <tt>Handler</tt>.
		/// <para>
		/// The encoding should be set before any <tt>LogRecords</tt> are written
		/// to the <tt>Handler</tt>.
		/// 
		/// </para>
		/// </summary>
		/// <param name="encoding">  The name of a supported character encoding.
		///        May be null, to indicate the default platform encoding. </param>
		/// <exception cref="SecurityException">  if a security manager exists and if
		///             the caller does not have <tt>LoggingPermission("control")</tt>. </exception>
		/// <exception cref="UnsupportedEncodingException"> if the named encoding is
		///          not supported. </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public synchronized void setEncoding(String encoding) throws SecurityException, java.io.UnsupportedEncodingException
		public virtual String Encoding
		{
			set
			{
				lock (this)
				{
					CheckPermission();
					if (value != null)
					{
						try
						{
							if (!java.nio.charset.Charset.IsSupported(value))
							{
								throw new UnsupportedEncodingException(value);
							}
						}
						catch (java.nio.charset.IllegalCharsetNameException)
						{
							throw new UnsupportedEncodingException(value);
						}
					}
					this.Encoding_Renamed = value;
				}
			}
			get
			{
				return Encoding_Renamed;
			}
		}


		/// <summary>
		/// Set a <tt>Filter</tt> to control output on this <tt>Handler</tt>.
		/// <P>
		/// For each call of <tt>publish</tt> the <tt>Handler</tt> will call
		/// this <tt>Filter</tt> (if it is non-null) to check if the
		/// <tt>LogRecord</tt> should be published or discarded.
		/// </summary>
		/// <param name="newFilter">  a <tt>Filter</tt> object (may be null) </param>
		/// <exception cref="SecurityException">  if a security manager exists and if
		///             the caller does not have <tt>LoggingPermission("control")</tt>. </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public synchronized void setFilter(Filter newFilter) throws SecurityException
		public virtual Filter Filter
		{
			set
			{
				lock (this)
				{
					CheckPermission();
					Filter_Renamed = value;
				}
			}
			get
			{
				return Filter_Renamed;
			}
		}


		/// <summary>
		/// Define an ErrorManager for this Handler.
		/// <para>
		/// The ErrorManager's "error" method will be invoked if any
		/// errors occur while using this Handler.
		/// 
		/// </para>
		/// </summary>
		/// <param name="em">  the new ErrorManager </param>
		/// <exception cref="SecurityException">  if a security manager exists and if
		///             the caller does not have <tt>LoggingPermission("control")</tt>. </exception>
		public virtual ErrorManager ErrorManager
		{
			set
			{
				lock (this)
				{
					CheckPermission();
					if (value == null)
					{
					   throw new NullPointerException();
					}
					ErrorManager_Renamed = value;
				}
			}
			get
			{
				CheckPermission();
				return ErrorManager_Renamed;
			}
		}


	   /// <summary>
	   /// Protected convenience method to report an error to this Handler's
	   /// ErrorManager.  Note that this method retrieves and uses the ErrorManager
	   /// without doing a security check.  It can therefore be used in
	   /// environments where the caller may be non-privileged.
	   /// </summary>
	   /// <param name="msg">    a descriptive string (may be null) </param>
	   /// <param name="ex">     an exception (may be null) </param>
	   /// <param name="code">   an error code defined in ErrorManager </param>
		protected internal virtual void ReportError(String msg, Exception ex, int code)
		{
			try
			{
				ErrorManager_Renamed.Error(msg, ex, code);
			}
			catch (Exception ex2)
			{
				System.Console.Error.WriteLine("Handler.reportError caught:");
				ex2.PrintStackTrace();
			}
		}

		/// <summary>
		/// Set the log level specifying which message levels will be
		/// logged by this <tt>Handler</tt>.  Message levels lower than this
		/// value will be discarded.
		/// <para>
		/// The intention is to allow developers to turn on voluminous
		/// logging, but to limit the messages that are sent to certain
		/// <tt>Handlers</tt>.
		/// 
		/// </para>
		/// </summary>
		/// <param name="newLevel">   the new value for the log level </param>
		/// <exception cref="SecurityException">  if a security manager exists and if
		///             the caller does not have <tt>LoggingPermission("control")</tt>. </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public synchronized void setLevel(Level newLevel) throws SecurityException
		public virtual Level Level
		{
			set
			{
				lock (this)
				{
					if (value == null)
					{
						throw new NullPointerException();
					}
					CheckPermission();
					LogLevel = value;
				}
			}
			get
			{
				return LogLevel;
			}
		}


		/// <summary>
		/// Check if this <tt>Handler</tt> would actually log a given <tt>LogRecord</tt>.
		/// <para>
		/// This method checks if the <tt>LogRecord</tt> has an appropriate
		/// <tt>Level</tt> and  whether it satisfies any <tt>Filter</tt>.  It also
		/// may make other <tt>Handler</tt> specific checks that might prevent a
		/// handler from logging the <tt>LogRecord</tt>. It will return false if
		/// the <tt>LogRecord</tt> is null.
		/// </para>
		/// <para>
		/// </para>
		/// </summary>
		/// <param name="record">  a <tt>LogRecord</tt> </param>
		/// <returns> true if the <tt>LogRecord</tt> would be logged.
		///  </returns>
		public virtual bool IsLoggable(LogRecord record)
		{
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final int levelValue = getLevel().intValue();
			int levelValue = Level.IntValue();
			if (record.Level.IntValue() < levelValue || levelValue == OffValue)
			{
				return false;
			}
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final Filter filter = getFilter();
			Filter filter = Filter;
			if (filter == null)
			{
				return true;
			}
			return filter.IsLoggable(record);
		}

		// Package-private support method for security checks.
		// If "sealed" is true, we check that the caller has
		// appropriate security privileges to update Handler
		// state and if not throw a SecurityException.
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: void checkPermission() throws SecurityException
		internal virtual void CheckPermission()
		{
			if (@sealed)
			{
				Manager.CheckPermission();
			}
		}
	}

}
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
	/// Stream based logging <tt>Handler</tt>.
	/// <para>
	/// This is primarily intended as a base class or support class to
	/// be used in implementing other logging <tt>Handlers</tt>.
	/// </para>
	/// <para>
	/// <tt>LogRecords</tt> are published to a given <tt>java.io.OutputStream</tt>.
	/// </para>
	/// <para>
	/// <b>Configuration:</b>
	/// By default each <tt>StreamHandler</tt> is initialized using the following
	/// <tt>LogManager</tt> configuration properties where <tt>&lt;handler-name&gt;</tt>
	/// refers to the fully-qualified class name of the handler.
	/// If properties are not defined
	/// (or have invalid values) then the specified default values are used.
	/// <ul>
	/// <li>   &lt;handler-name&gt;.level
	///        specifies the default level for the <tt>Handler</tt>
	///        (defaults to <tt>Level.INFO</tt>). </li>
	/// <li>   &lt;handler-name&gt;.filter
	///        specifies the name of a <tt>Filter</tt> class to use
	///         (defaults to no <tt>Filter</tt>). </li>
	/// <li>   &lt;handler-name&gt;.formatter
	///        specifies the name of a <tt>Formatter</tt> class to use
	///        (defaults to <tt>java.util.logging.SimpleFormatter</tt>). </li>
	/// <li>   &lt;handler-name&gt;.encoding
	///        the name of the character set encoding to use (defaults to
	///        the default platform encoding). </li>
	/// </ul>
	/// </para>
	/// <para>
	/// For example, the properties for {@code StreamHandler} would be:
	/// <ul>
	/// <li>   java.util.logging.StreamHandler.level=INFO </li>
	/// <li>   java.util.logging.StreamHandler.formatter=java.util.logging.SimpleFormatter </li>
	/// </ul>
	/// </para>
	/// <para>
	/// For a custom handler, e.g. com.foo.MyHandler, the properties would be:
	/// <ul>
	/// <li>   com.foo.MyHandler.level=INFO </li>
	/// <li>   com.foo.MyHandler.formatter=java.util.logging.SimpleFormatter </li>
	/// </ul>
	/// </para>
	/// <para>
	/// @since 1.4
	/// </para>
	/// </summary>

	public class StreamHandler : Handler
	{
		private OutputStream Output;
		private bool DoneHeader;
		private volatile Writer Writer;

		// Private method to configure a StreamHandler from LogManager
		// properties and/or default values as specified in the class
		// javadoc.
		private void Configure()
		{
			LogManager manager = LogManager.LogManager;
//JAVA TO C# CONVERTER WARNING: The .NET Type.FullName property will not always yield results identical to the Java Class.getName method:
			String cname = this.GetType().FullName;

			Level = manager.GetLevelProperty(cname + ".level", Level.INFO);
			Filter = manager.GetFilterProperty(cname + ".filter", null);
			Formatter = manager.GetFormatterProperty(cname + ".formatter", new SimpleFormatter());
			try
			{
				Encoding = manager.GetStringProperty(cname + ".encoding", null);
			}
			catch (Exception)
			{
				try
				{
					Encoding = null;
				}
				catch (Exception)
				{
					// doing a setEncoding with null should always work.
					// assert false;
				}
			}
		}

		/// <summary>
		/// Create a <tt>StreamHandler</tt>, with no current output stream.
		/// </summary>
		public StreamHandler()
		{
			@sealed = false;
			Configure();
			@sealed = true;
		}

		/// <summary>
		/// Create a <tt>StreamHandler</tt> with a given <tt>Formatter</tt>
		/// and output stream.
		/// <para>
		/// </para>
		/// </summary>
		/// <param name="out">         the target output stream </param>
		/// <param name="formatter">   Formatter to be used to format output </param>
		public StreamHandler(OutputStream @out, Formatter formatter)
		{
			@sealed = false;
			Configure();
			Formatter = formatter;
			OutputStream = @out;
			@sealed = true;
		}

		/// <summary>
		/// Change the output stream.
		/// <P>
		/// If there is a current output stream then the <tt>Formatter</tt>'s
		/// tail string is written and the stream is flushed and closed.
		/// Then the output stream is replaced with the new output stream.
		/// </summary>
		/// <param name="out">   New output stream.  May not be null. </param>
		/// <exception cref="SecurityException">  if a security manager exists and if
		///             the caller does not have <tt>LoggingPermission("control")</tt>. </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: protected synchronized void setOutputStream(OutputStream out) throws SecurityException
		protected internal virtual OutputStream OutputStream
		{
			set
			{
				lock (this)
				{
					if (value == null)
					{
						throw new NullPointerException();
					}
					FlushAndClose();
					Output = value;
					DoneHeader = false;
					String encoding = Encoding;
					if (encoding == null)
					{
						Writer = new OutputStreamWriter(Output);
					}
					else
					{
						try
						{
							Writer = new OutputStreamWriter(Output, encoding);
						}
						catch (UnsupportedEncodingException ex)
						{
							// This shouldn't happen.  The setEncoding method
							// should have validated that the encoding is OK.
							throw new Error("Unexpected exception " + ex);
						}
					}
				}
			}
		}

		/// <summary>
		/// Set (or change) the character encoding used by this <tt>Handler</tt>.
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
//ORIGINAL LINE: @Override public synchronized void setEncoding(String encoding) throws SecurityException, java.io.UnsupportedEncodingException
		public override String Encoding
		{
			set
			{
				lock (this)
				{
					base.Encoding = value;
					if (Output == null)
					{
						return;
					}
					// Replace the current writer with a writer for the new value.
					Flush();
					if (value == null)
					{
						Writer = new OutputStreamWriter(Output);
					}
					else
					{
						Writer = new OutputStreamWriter(Output, value);
					}
				}
			}
		}

		/// <summary>
		/// Format and publish a <tt>LogRecord</tt>.
		/// <para>
		/// The <tt>StreamHandler</tt> first checks if there is an <tt>OutputStream</tt>
		/// and if the given <tt>LogRecord</tt> has at least the required log level.
		/// If not it silently returns.  If so, it calls any associated
		/// <tt>Filter</tt> to check if the record should be published.  If so,
		/// it calls its <tt>Formatter</tt> to format the record and then writes
		/// the result to the current output stream.
		/// </para>
		/// <para>
		/// If this is the first <tt>LogRecord</tt> to be written to a given
		/// <tt>OutputStream</tt>, the <tt>Formatter</tt>'s "head" string is
		/// written to the stream before the <tt>LogRecord</tt> is written.
		/// 
		/// </para>
		/// </summary>
		/// <param name="record">  description of the log event. A null record is
		///                 silently ignored and is not published </param>
		public override void Publish(LogRecord record)
		{
			lock (this)
			{
				if (!IsLoggable(record))
				{
					return;
				}
				String msg;
				try
				{
					msg = Formatter.format(record);
				}
				catch (Exception ex)
				{
					// We don't want to throw an exception here, but we
					// report the exception to any registered ErrorManager.
					ReportError(null, ex, ErrorManager.FORMAT_FAILURE);
					return;
				}
        
				try
				{
					if (!DoneHeader)
					{
						Writer.Write(Formatter.getHead(this));
						DoneHeader = true;
					}
					Writer.Write(msg);
				}
				catch (Exception ex)
				{
					// We don't want to throw an exception here, but we
					// report the exception to any registered ErrorManager.
					ReportError(null, ex, ErrorManager.WRITE_FAILURE);
				}
			}
		}


		/// <summary>
		/// Check if this <tt>Handler</tt> would actually log a given <tt>LogRecord</tt>.
		/// <para>
		/// This method checks if the <tt>LogRecord</tt> has an appropriate level and
		/// whether it satisfies any <tt>Filter</tt>.  It will also return false if
		/// no output stream has been assigned yet or the LogRecord is null.
		/// </para>
		/// <para>
		/// </para>
		/// </summary>
		/// <param name="record">  a <tt>LogRecord</tt> </param>
		/// <returns> true if the <tt>LogRecord</tt> would be logged.
		///  </returns>
		public override bool IsLoggable(LogRecord record)
		{
			if (Writer == null || record == null)
			{
				return false;
			}
			return base.IsLoggable(record);
		}

		/// <summary>
		/// Flush any buffered messages.
		/// </summary>
		public override void Flush()
		{
			lock (this)
			{
				if (Writer != null)
				{
					try
					{
						Writer.Flush();
					}
					catch (Exception ex)
					{
						// We don't want to throw an exception here, but we
						// report the exception to any registered ErrorManager.
						ReportError(null, ex, ErrorManager.FLUSH_FAILURE);
					}
				}
			}
		}

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: private synchronized void flushAndClose() throws SecurityException
		private void FlushAndClose()
		{
			lock (this)
			{
				CheckPermission();
				if (Writer != null)
				{
					try
					{
						if (!DoneHeader)
						{
							Writer.Write(Formatter.getHead(this));
							DoneHeader = true;
						}
						Writer.Write(Formatter.getTail(this));
						Writer.Flush();
						Writer.Close();
					}
					catch (Exception ex)
					{
						// We don't want to throw an exception here, but we
						// report the exception to any registered ErrorManager.
						ReportError(null, ex, ErrorManager.CLOSE_FAILURE);
					}
					Writer = null;
					Output = null;
				}
			}
		}

		/// <summary>
		/// Close the current output stream.
		/// <para>
		/// The <tt>Formatter</tt>'s "tail" string is written to the stream before it
		/// is closed.  In addition, if the <tt>Formatter</tt>'s "head" string has not
		/// yet been written to the stream, it will be written before the
		/// "tail" string.
		/// 
		/// </para>
		/// </summary>
		/// <exception cref="SecurityException">  if a security manager exists and if
		///             the caller does not have LoggingPermission("control"). </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: @Override public synchronized void close() throws SecurityException
		public override void Close()
		{
			lock (this)
			{
				FlushAndClose();
			}
		}
	}

}
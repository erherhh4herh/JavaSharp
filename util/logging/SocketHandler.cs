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
	/// Simple network logging <tt>Handler</tt>.
	/// <para>
	/// <tt>LogRecords</tt> are published to a network stream connection.  By default
	/// the <tt>XMLFormatter</tt> class is used for formatting.
	/// </para>
	/// <para>
	/// <b>Configuration:</b>
	/// By default each <tt>SocketHandler</tt> is initialized using the following
	/// <tt>LogManager</tt> configuration properties where <tt>&lt;handler-name&gt;</tt>
	/// refers to the fully-qualified class name of the handler.
	/// If properties are not defined
	/// (or have invalid values) then the specified default values are used.
	/// <ul>
	/// <li>   &lt;handler-name&gt;.level
	///        specifies the default level for the <tt>Handler</tt>
	///        (defaults to <tt>Level.ALL</tt>). </li>
	/// <li>   &lt;handler-name&gt;.filter
	///        specifies the name of a <tt>Filter</tt> class to use
	///        (defaults to no <tt>Filter</tt>). </li>
	/// <li>   &lt;handler-name&gt;.formatter
	///        specifies the name of a <tt>Formatter</tt> class to use
	///        (defaults to <tt>java.util.logging.XMLFormatter</tt>). </li>
	/// <li>   &lt;handler-name&gt;.encoding
	///        the name of the character set encoding to use (defaults to
	///        the default platform encoding). </li>
	/// <li>   &lt;handler-name&gt;.host
	///        specifies the target host name to connect to (no default). </li>
	/// <li>   &lt;handler-name&gt;.port
	///        specifies the target TCP port to use (no default). </li>
	/// </ul>
	/// </para>
	/// <para>
	/// For example, the properties for {@code SocketHandler} would be:
	/// <ul>
	/// <li>   java.util.logging.SocketHandler.level=INFO </li>
	/// <li>   java.util.logging.SocketHandler.formatter=java.util.logging.SimpleFormatter </li>
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
	/// The output IO stream is buffered, but is flushed after each
	/// <tt>LogRecord</tt> is written.
	/// 
	/// @since 1.4
	/// </para>
	/// </summary>

	public class SocketHandler : StreamHandler
	{
		private Socket Sock;
		private String Host;
		private int Port;

		// Private method to configure a SocketHandler from LogManager
		// properties and/or default values as specified in the class
		// javadoc.
		private void Configure()
		{
			LogManager manager = LogManager.LogManager;
//JAVA TO C# CONVERTER WARNING: The .NET Type.FullName property will not always yield results identical to the Java Class.getName method:
			String cname = this.GetType().FullName;

			Level = manager.GetLevelProperty(cname + ".level", Level.ALL);
			Filter = manager.GetFilterProperty(cname + ".filter", null);
			Formatter = manager.GetFormatterProperty(cname + ".formatter", new XMLFormatter());
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
			Port = manager.GetIntProperty(cname + ".port", 0);
			Host = manager.GetStringProperty(cname + ".host", null);
		}


		/// <summary>
		/// Create a <tt>SocketHandler</tt>, using only <tt>LogManager</tt> properties
		/// (or their defaults). </summary>
		/// <exception cref="IllegalArgumentException"> if the host or port are invalid or
		///          are not specified as LogManager properties. </exception>
		/// <exception cref="IOException"> if we are unable to connect to the target
		///         host and port. </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public SocketHandler() throws IOException
		public SocketHandler()
		{
			// We are going to use the logging defaults.
			@sealed = false;
			Configure();

			try
			{
				Connect();
			}
			catch (IOException ix)
			{
				System.Console.Error.WriteLine("SocketHandler: connect failed to " + Host + ":" + Port);
				throw ix;
			}
			@sealed = true;
		}

		/// <summary>
		/// Construct a <tt>SocketHandler</tt> using a specified host and port.
		/// 
		/// The <tt>SocketHandler</tt> is configured based on <tt>LogManager</tt>
		/// properties (or their default values) except that the given target host
		/// and port arguments are used. If the host argument is empty, but not
		/// null String then the localhost is used.
		/// </summary>
		/// <param name="host"> target host. </param>
		/// <param name="port"> target port.
		/// </param>
		/// <exception cref="IllegalArgumentException"> if the host or port are invalid. </exception>
		/// <exception cref="IOException"> if we are unable to connect to the target
		///         host and port. </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public SocketHandler(String host, int port) throws IOException
		public SocketHandler(String host, int port)
		{
			@sealed = false;
			Configure();
			@sealed = true;
			this.Port = port;
			this.Host = host;
			Connect();
		}

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: private void connect() throws IOException
		private void Connect()
		{
			// Check the arguments are valid.
			if (Port == 0)
			{
				throw new IllegalArgumentException("Bad port: " + Port);
			}
			if (Host == null)
			{
				throw new IllegalArgumentException("Null host name: " + Host);
			}

			// Try to open a new socket.
			Sock = new Socket(Host, Port);
			OutputStream @out = Sock.OutputStream;
			BufferedOutputStream bout = new BufferedOutputStream(@out);
			OutputStream = bout;
		}

		/// <summary>
		/// Close this output stream.
		/// </summary>
		/// <exception cref="SecurityException">  if a security manager exists and if
		///             the caller does not have <tt>LoggingPermission("control")</tt>. </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: @Override public synchronized void close() throws SecurityException
		public override void Close()
		{
			lock (this)
			{
				base.Close();
				if (Sock != null)
				{
					try
					{
						Sock.Close();
					}
					catch (IOException)
					{
						// drop through.
					}
				}
				Sock = null;
			}
		}

		/// <summary>
		/// Format and publish a <tt>LogRecord</tt>.
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
				base.Publish(record);
				Flush();
			}
		}
	}

}
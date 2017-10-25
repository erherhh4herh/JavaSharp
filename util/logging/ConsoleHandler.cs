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
	/// This <tt>Handler</tt> publishes log records to <tt>System.err</tt>.
	/// By default the <tt>SimpleFormatter</tt> is used to generate brief summaries.
	/// <para>
	/// <b>Configuration:</b>
	/// By default each <tt>ConsoleHandler</tt> is initialized using the following
	/// <tt>LogManager</tt> configuration properties where {@code <handler-name>}
	/// refers to the fully-qualified class name of the handler.
	/// If properties are not defined
	/// (or have invalid values) then the specified default values are used.
	/// <ul>
	/// <li>   &lt;handler-name&gt;.level
	///        specifies the default level for the <tt>Handler</tt>
	///        (defaults to <tt>Level.INFO</tt>). </li>
	/// <li>   &lt;handler-name&gt;.filter
	///        specifies the name of a <tt>Filter</tt> class to use
	///        (defaults to no <tt>Filter</tt>). </li>
	/// <li>   &lt;handler-name&gt;.formatter
	///        specifies the name of a <tt>Formatter</tt> class to use
	///        (defaults to <tt>java.util.logging.SimpleFormatter</tt>). </li>
	/// <li>   &lt;handler-name&gt;.encoding
	///        the name of the character set encoding to use (defaults to
	///        the default platform encoding). </li>
	/// </ul>
	/// </para>
	/// <para>
	/// For example, the properties for {@code ConsoleHandler} would be:
	/// <ul>
	/// <li>   java.util.logging.ConsoleHandler.level=INFO </li>
	/// <li>   java.util.logging.ConsoleHandler.formatter=java.util.logging.SimpleFormatter </li>
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
	public class ConsoleHandler : StreamHandler
	{
		// Private method to configure a ConsoleHandler from LogManager
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
		/// Create a <tt>ConsoleHandler</tt> for <tt>System.err</tt>.
		/// <para>
		/// The <tt>ConsoleHandler</tt> is configured based on
		/// <tt>LogManager</tt> properties (or their default values).
		/// 
		/// </para>
		/// </summary>
		public ConsoleHandler()
		{
			@sealed = false;
			Configure();
			OutputStream = System.err;
			@sealed = true;
		}

		/// <summary>
		/// Publish a <tt>LogRecord</tt>.
		/// <para>
		/// The logging request was made initially to a <tt>Logger</tt> object,
		/// which initialized the <tt>LogRecord</tt> and forwarded it here.
		/// </para>
		/// <para>
		/// </para>
		/// </summary>
		/// <param name="record">  description of the log event. A null record is
		///                 silently ignored and is not published </param>
		public override void Publish(LogRecord record)
		{
			base.Publish(record);
			Flush();
		}

		/// <summary>
		/// Override <tt>StreamHandler.close</tt> to do a flush but not
		/// to close the output stream.  That is, we do <b>not</b>
		/// close <tt>System.err</tt>.
		/// </summary>
		public override void Close()
		{
			Flush();
		}
	}

}
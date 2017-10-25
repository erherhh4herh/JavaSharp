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
	/// <tt>Handler</tt> that buffers requests in a circular buffer in memory.
	/// <para>
	/// Normally this <tt>Handler</tt> simply stores incoming <tt>LogRecords</tt>
	/// into its memory buffer and discards earlier records.  This buffering
	/// is very cheap and avoids formatting costs.  On certain trigger
	/// conditions, the <tt>MemoryHandler</tt> will push out its current buffer
	/// contents to a target <tt>Handler</tt>, which will typically publish
	/// them to the outside world.
	/// </para>
	/// <para>
	/// There are three main models for triggering a push of the buffer:
	/// <ul>
	/// <li>
	/// An incoming <tt>LogRecord</tt> has a type that is greater than
	/// a pre-defined level, the <tt>pushLevel</tt>. </li>
	/// <li>
	/// An external class calls the <tt>push</tt> method explicitly. </li>
	/// <li>
	/// A subclass overrides the <tt>log</tt> method and scans each incoming
	/// <tt>LogRecord</tt> and calls <tt>push</tt> if a record matches some
	/// desired criteria. </li>
	/// </ul>
	/// </para>
	/// <para>
	/// <b>Configuration:</b>
	/// By default each <tt>MemoryHandler</tt> is initialized using the following
	/// <tt>LogManager</tt> configuration properties where <tt>&lt;handler-name&gt;</tt>
	/// refers to the fully-qualified class name of the handler.
	/// If properties are not defined
	/// (or have invalid values) then the specified default values are used.
	/// If no default value is defined then a RuntimeException is thrown.
	/// <ul>
	/// <li>   &lt;handler-name&gt;.level
	///        specifies the level for the <tt>Handler</tt>
	///        (defaults to <tt>Level.ALL</tt>). </li>
	/// <li>   &lt;handler-name&gt;.filter
	///        specifies the name of a <tt>Filter</tt> class to use
	///        (defaults to no <tt>Filter</tt>). </li>
	/// <li>   &lt;handler-name&gt;.size
	///        defines the buffer size (defaults to 1000). </li>
	/// <li>   &lt;handler-name&gt;.push
	///        defines the <tt>pushLevel</tt> (defaults to <tt>level.SEVERE</tt>). </li>
	/// <li>   &lt;handler-name&gt;.target
	///        specifies the name of the target <tt>Handler </tt> class.
	///        (no default). </li>
	/// </ul>
	/// </para>
	/// <para>
	/// For example, the properties for {@code MemoryHandler} would be:
	/// <ul>
	/// <li>   java.util.logging.MemoryHandler.level=INFO </li>
	/// <li>   java.util.logging.MemoryHandler.formatter=java.util.logging.SimpleFormatter </li>
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

	public class MemoryHandler : Handler
	{
		private const int DEFAULT_SIZE = 1000;
		private volatile Level PushLevel_Renamed;
		private int Size;
		private Handler Target;
		private LogRecord[] Buffer;
		internal int Start, Count;

		// Private method to configure a MemoryHandler from LogManager
		// properties and/or default values as specified in the class
		// javadoc.
		private void Configure()
		{
			LogManager manager = LogManager.LogManager;
//JAVA TO C# CONVERTER WARNING: The .NET Type.FullName property will not always yield results identical to the Java Class.getName method:
			String cname = this.GetType().FullName;

			PushLevel_Renamed = manager.GetLevelProperty(cname + ".push", Level.SEVERE);
			Size = manager.GetIntProperty(cname + ".size", DEFAULT_SIZE);
			if (Size <= 0)
			{
				Size = DEFAULT_SIZE;
			}
			Level = manager.GetLevelProperty(cname + ".level", Level.ALL);
			Filter = manager.GetFilterProperty(cname + ".filter", null);
			Formatter = manager.GetFormatterProperty(cname + ".formatter", new SimpleFormatter());
		}

		/// <summary>
		/// Create a <tt>MemoryHandler</tt> and configure it based on
		/// <tt>LogManager</tt> configuration properties.
		/// </summary>
		public MemoryHandler()
		{
			@sealed = false;
			Configure();
			@sealed = true;

			LogManager manager = LogManager.LogManager;
//JAVA TO C# CONVERTER WARNING: The .NET Type.FullName property will not always yield results identical to the Java Class.getName method:
			String handlerName = this.GetType().FullName;
			String targetName = manager.GetProperty(handlerName + ".target");
			if (targetName == null)
			{
				throw new RuntimeException("The handler " + handlerName + " does not specify a target");
			}
			Class clz;
			try
			{
				clz = ClassLoader.SystemClassLoader.LoadClass(targetName);
				Target = (Handler) clz.NewInstance();
			}
//JAVA TO C# CONVERTER TODO TASK: There is no equivalent in C# to Java 'multi-catch' syntax:
			catch (ClassNotFoundException | InstantiationException | IllegalAccessException e)
			{
				throw new RuntimeException("MemoryHandler can't load handler target \"" + targetName + "\"", e);
			}
			Init();
		}

		// Initialize.  Size is a count of LogRecords.
		private void Init()
		{
			Buffer = new LogRecord[Size];
			Start = 0;
			Count = 0;
		}

		/// <summary>
		/// Create a <tt>MemoryHandler</tt>.
		/// <para>
		/// The <tt>MemoryHandler</tt> is configured based on <tt>LogManager</tt>
		/// properties (or their default values) except that the given <tt>pushLevel</tt>
		/// argument and buffer size argument are used.
		/// 
		/// </para>
		/// </summary>
		/// <param name="target">  the Handler to which to publish output. </param>
		/// <param name="size">    the number of log records to buffer (must be greater than zero) </param>
		/// <param name="pushLevel">  message level to push on
		/// </param>
		/// <exception cref="IllegalArgumentException"> if {@code size is <= 0} </exception>
		public MemoryHandler(Handler target, int size, Level pushLevel)
		{
			if (target == null || pushLevel == null)
			{
				throw new NullPointerException();
			}
			if (size <= 0)
			{
				throw new IllegalArgumentException();
			}
			@sealed = false;
			Configure();
			@sealed = true;
			this.Target = target;
			this.PushLevel_Renamed = pushLevel;
			this.Size = size;
			Init();
		}

		/// <summary>
		/// Store a <tt>LogRecord</tt> in an internal buffer.
		/// <para>
		/// If there is a <tt>Filter</tt>, its <tt>isLoggable</tt>
		/// method is called to check if the given log record is loggable.
		/// If not we return.  Otherwise the given record is copied into
		/// an internal circular buffer.  Then the record's level property is
		/// compared with the <tt>pushLevel</tt>. If the given level is
		/// greater than or equal to the <tt>pushLevel</tt> then <tt>push</tt>
		/// is called to write all buffered records to the target output
		/// <tt>Handler</tt>.
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
				int ix = (Start + Count) % Buffer.Length;
				Buffer[ix] = record;
				if (Count < Buffer.Length)
				{
					Count++;
				}
				else
				{
					Start++;
					Start %= Buffer.Length;
				}
				if (record.Level.IntValue() >= PushLevel_Renamed.IntValue())
				{
					Push();
				}
			}
		}

		/// <summary>
		/// Push any buffered output to the target <tt>Handler</tt>.
		/// <para>
		/// The buffer is then cleared.
		/// </para>
		/// </summary>
		public virtual void Push()
		{
			lock (this)
			{
				for (int i = 0; i < Count; i++)
				{
					int ix = (Start + i) % Buffer.Length;
					LogRecord record = Buffer[ix];
					Target.Publish(record);
				}
				// Empty the buffer.
				Start = 0;
				Count = 0;
			}
		}

		/// <summary>
		/// Causes a flush on the target <tt>Handler</tt>.
		/// <para>
		/// Note that the current contents of the <tt>MemoryHandler</tt>
		/// buffer are <b>not</b> written out.  That requires a "push".
		/// </para>
		/// </summary>
		public override void Flush()
		{
			Target.Flush();
		}

		/// <summary>
		/// Close the <tt>Handler</tt> and free all associated resources.
		/// This will also close the target <tt>Handler</tt>.
		/// </summary>
		/// <exception cref="SecurityException">  if a security manager exists and if
		///             the caller does not have <tt>LoggingPermission("control")</tt>. </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: @Override public void close() throws SecurityException
		public override void Close()
		{
			Target.Close();
			Level = Level.OFF;
		}

		/// <summary>
		/// Set the <tt>pushLevel</tt>.  After a <tt>LogRecord</tt> is copied
		/// into our internal buffer, if its level is greater than or equal to
		/// the <tt>pushLevel</tt>, then <tt>push</tt> will be called.
		/// </summary>
		/// <param name="newLevel"> the new value of the <tt>pushLevel</tt> </param>
		/// <exception cref="SecurityException">  if a security manager exists and if
		///             the caller does not have <tt>LoggingPermission("control")</tt>. </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public synchronized void setPushLevel(Level newLevel) throws SecurityException
		public virtual Level PushLevel
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
					PushLevel_Renamed = value;
				}
			}
			get
			{
				return PushLevel_Renamed;
			}
		}


		/// <summary>
		/// Check if this <tt>Handler</tt> would actually log a given
		/// <tt>LogRecord</tt> into its internal buffer.
		/// <para>
		/// This method checks if the <tt>LogRecord</tt> has an appropriate level and
		/// whether it satisfies any <tt>Filter</tt>.  However it does <b>not</b>
		/// check whether the <tt>LogRecord</tt> would result in a "push" of the
		/// buffer contents. It will return false if the <tt>LogRecord</tt> is null.
		/// </para>
		/// <para>
		/// </para>
		/// </summary>
		/// <param name="record">  a <tt>LogRecord</tt> </param>
		/// <returns> true if the <tt>LogRecord</tt> would be logged.
		///  </returns>
		public override bool IsLoggable(LogRecord record)
		{
			return base.IsLoggable(record);
		}
	}

}
using System;
using System.Threading;

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

	using JavaLangAccess = sun.misc.JavaLangAccess;
	using SharedSecrets = sun.misc.SharedSecrets;

	/// <summary>
	/// LogRecord objects are used to pass logging requests between
	/// the logging framework and individual log Handlers.
	/// <para>
	/// When a LogRecord is passed into the logging framework it
	/// logically belongs to the framework and should no longer be
	/// used or updated by the client application.
	/// </para>
	/// <para>
	/// Note that if the client application has not specified an
	/// explicit source method name and source class name, then the
	/// LogRecord class will infer them automatically when they are
	/// first accessed (due to a call on getSourceMethodName or
	/// getSourceClassName) by analyzing the call stack.  Therefore,
	/// if a logging Handler wants to pass off a LogRecord to another
	/// thread, or to transmit it over RMI, and if it wishes to subsequently
	/// obtain method name or class name information it should call
	/// one of getSourceClassName or getSourceMethodName to force
	/// the values to be filled in.
	/// </para>
	/// <para>
	/// <b> Serialization notes:</b>
	/// <ul>
	/// <li>The LogRecord class is serializable.
	/// 
	/// <li> Because objects in the parameters array may not be serializable,
	/// during serialization all objects in the parameters array are
	/// written as the corresponding Strings (using Object.toString).
	/// 
	/// <li> The ResourceBundle is not transmitted as part of the serialized
	/// form, but the resource bundle name is, and the recipient object's
	/// readObject method will attempt to locate a suitable resource bundle.
	/// 
	/// </ul>
	/// 
	/// @since 1.4
	/// </para>
	/// </summary>

	[Serializable]
	public class LogRecord
	{
		private static readonly AtomicLong GlobalSequenceNumber = new AtomicLong(0);

		/// <summary>
		/// The default value of threadID will be the current thread's
		/// thread id, for ease of correlation, unless it is greater than
		/// MIN_SEQUENTIAL_THREAD_ID, in which case we try harder to keep
		/// our promise to keep threadIDs unique by avoiding collisions due
		/// to 32-bit wraparound.  Unfortunately, LogRecord.getThreadID()
		/// returns int, while Thread.getId() returns long.
		/// </summary>
		private static readonly int MIN_SEQUENTIAL_THREAD_ID = Integer.MaxValue / 2;

		private static readonly AtomicInteger NextThreadId = new AtomicInteger(MIN_SEQUENTIAL_THREAD_ID);

		private static readonly ThreadLocal<Integer> ThreadIds = new ThreadLocal<Integer>();

		/// <summary>
		/// @serial Logging message level
		/// </summary>
		private Level Level_Renamed;

		/// <summary>
		/// @serial Sequence number
		/// </summary>
		private long SequenceNumber_Renamed;

		/// <summary>
		/// @serial Class that issued logging call
		/// </summary>
		private String SourceClassName_Renamed;

		/// <summary>
		/// @serial Method that issued logging call
		/// </summary>
		private String SourceMethodName_Renamed;

		/// <summary>
		/// @serial Non-localized raw message text
		/// </summary>
		private String Message_Renamed;

		/// <summary>
		/// @serial Thread ID for thread that issued logging call.
		/// </summary>
		private int ThreadID_Renamed;

		/// <summary>
		/// @serial Event time in milliseconds since 1970
		/// </summary>
		private long Millis_Renamed;

		/// <summary>
		/// @serial The Throwable (if any) associated with log message
		/// </summary>
		private Throwable Thrown_Renamed;

		/// <summary>
		/// @serial Name of the source Logger.
		/// </summary>
		private String LoggerName_Renamed;

		/// <summary>
		/// @serial Resource bundle name to localized log message.
		/// </summary>
		private String ResourceBundleName_Renamed;

		[NonSerialized]
		private bool NeedToInferCaller;
		[NonSerialized]
		private Object[] Parameters_Renamed;
		[NonSerialized]
		private ResourceBundle ResourceBundle_Renamed;

		/// <summary>
		/// Returns the default value for a new LogRecord's threadID.
		/// </summary>
		private int DefaultThreadID()
		{
			long tid = Thread.CurrentThread.Id;
			if (tid < MIN_SEQUENTIAL_THREAD_ID)
			{
				return (int) tid;
			}
			else
			{
				Integer id = ThreadIds.Get();
				if (id == null)
				{
					id = NextThreadId.AndIncrement;
					ThreadIds.Set(id);
				}
				return id;
			}
		}

		/// <summary>
		/// Construct a LogRecord with the given level and message values.
		/// <para>
		/// The sequence property will be initialized with a new unique value.
		/// These sequence values are allocated in increasing order within a VM.
		/// </para>
		/// <para>
		/// The millis property will be initialized to the current time.
		/// </para>
		/// <para>
		/// The thread ID property will be initialized with a unique ID for
		/// the current thread.
		/// </para>
		/// <para>
		/// All other properties will be initialized to "null".
		/// 
		/// </para>
		/// </summary>
		/// <param name="level">  a logging level value </param>
		/// <param name="msg">  the raw non-localized logging message (may be null) </param>
		public LogRecord(Level level, String msg)
		{
			// Make sure level isn't null, by calling random method.
			level.GetType();
			this.Level_Renamed = level;
			Message_Renamed = msg;
			// Assign a thread ID and a unique sequence number.
			SequenceNumber_Renamed = GlobalSequenceNumber.AndIncrement;
			ThreadID_Renamed = DefaultThreadID();
			Millis_Renamed = DateTimeHelperClass.CurrentUnixTimeMillis();
			NeedToInferCaller = true;
		}

		/// <summary>
		/// Get the source Logger's name.
		/// </summary>
		/// <returns> source logger name (may be null) </returns>
		public virtual String LoggerName
		{
			get
			{
				return LoggerName_Renamed;
			}
			set
			{
				LoggerName_Renamed = value;
			}
		}


		/// <summary>
		/// Get the localization resource bundle
		/// <para>
		/// This is the ResourceBundle that should be used to localize
		/// the message string before formatting it.  The result may
		/// be null if the message is not localizable, or if no suitable
		/// ResourceBundle is available.
		/// </para>
		/// </summary>
		/// <returns> the localization resource bundle </returns>
		public virtual ResourceBundle ResourceBundle
		{
			get
			{
				return ResourceBundle_Renamed;
			}
			set
			{
				ResourceBundle_Renamed = value;
			}
		}


		/// <summary>
		/// Get the localization resource bundle name
		/// <para>
		/// This is the name for the ResourceBundle that should be
		/// used to localize the message string before formatting it.
		/// The result may be null if the message is not localizable.
		/// </para>
		/// </summary>
		/// <returns> the localization resource bundle name </returns>
		public virtual String ResourceBundleName
		{
			get
			{
				return ResourceBundleName_Renamed;
			}
			set
			{
				ResourceBundleName_Renamed = value;
			}
		}


		/// <summary>
		/// Get the logging message level, for example Level.SEVERE. </summary>
		/// <returns> the logging message level </returns>
		public virtual Level Level
		{
			get
			{
				return Level_Renamed;
			}
			set
			{
				if (value == null)
				{
					throw new NullPointerException();
				}
				this.Level_Renamed = value;
			}
		}


		/// <summary>
		/// Get the sequence number.
		/// <para>
		/// Sequence numbers are normally assigned in the LogRecord
		/// constructor, which assigns unique sequence numbers to
		/// each new LogRecord in increasing order.
		/// </para>
		/// </summary>
		/// <returns> the sequence number </returns>
		public virtual long SequenceNumber
		{
			get
			{
				return SequenceNumber_Renamed;
			}
			set
			{
				SequenceNumber_Renamed = value;
			}
		}


		/// <summary>
		/// Get the  name of the class that (allegedly) issued the logging request.
		/// <para>
		/// Note that this sourceClassName is not verified and may be spoofed.
		/// This information may either have been provided as part of the
		/// logging call, or it may have been inferred automatically by the
		/// logging framework.  In the latter case, the information may only
		/// be approximate and may in fact describe an earlier call on the
		/// stack frame.
		/// </para>
		/// <para>
		/// May be null if no information could be obtained.
		/// 
		/// </para>
		/// </summary>
		/// <returns> the source class name </returns>
		public virtual String SourceClassName
		{
			get
			{
				if (NeedToInferCaller)
				{
					InferCaller();
				}
				return SourceClassName_Renamed;
			}
			set
			{
				this.SourceClassName_Renamed = value;
				NeedToInferCaller = false;
			}
		}


		/// <summary>
		/// Get the  name of the method that (allegedly) issued the logging request.
		/// <para>
		/// Note that this sourceMethodName is not verified and may be spoofed.
		/// This information may either have been provided as part of the
		/// logging call, or it may have been inferred automatically by the
		/// logging framework.  In the latter case, the information may only
		/// be approximate and may in fact describe an earlier call on the
		/// stack frame.
		/// </para>
		/// <para>
		/// May be null if no information could be obtained.
		/// 
		/// </para>
		/// </summary>
		/// <returns> the source method name </returns>
		public virtual String SourceMethodName
		{
			get
			{
				if (NeedToInferCaller)
				{
					InferCaller();
				}
				return SourceMethodName_Renamed;
			}
			set
			{
				this.SourceMethodName_Renamed = value;
				NeedToInferCaller = false;
			}
		}


		/// <summary>
		/// Get the "raw" log message, before localization or formatting.
		/// <para>
		/// May be null, which is equivalent to the empty string "".
		/// </para>
		/// <para>
		/// This message may be either the final text or a localization key.
		/// </para>
		/// <para>
		/// During formatting, if the source logger has a localization
		/// ResourceBundle and if that ResourceBundle has an entry for
		/// this message string, then the message string is replaced
		/// with the localized value.
		/// 
		/// </para>
		/// </summary>
		/// <returns> the raw message string </returns>
		public virtual String Message
		{
			get
			{
				return Message_Renamed;
			}
			set
			{
				this.Message_Renamed = value;
			}
		}


		/// <summary>
		/// Get the parameters to the log message.
		/// </summary>
		/// <returns> the log message parameters.  May be null if
		///                  there are no parameters. </returns>
		public virtual Object[] Parameters
		{
			get
			{
				return Parameters_Renamed;
			}
			set
			{
				this.Parameters_Renamed = value;
			}
		}


		/// <summary>
		/// Get an identifier for the thread where the message originated.
		/// <para>
		/// This is a thread identifier within the Java VM and may or
		/// may not map to any operating system ID.
		/// 
		/// </para>
		/// </summary>
		/// <returns> thread ID </returns>
		public virtual int ThreadID
		{
			get
			{
				return ThreadID_Renamed;
			}
			set
			{
				this.ThreadID_Renamed = value;
			}
		}


		/// <summary>
		/// Get event time in milliseconds since 1970.
		/// </summary>
		/// <returns> event time in millis since 1970 </returns>
		public virtual long Millis
		{
			get
			{
				return Millis_Renamed;
			}
			set
			{
				this.Millis_Renamed = value;
			}
		}


		/// <summary>
		/// Get any throwable associated with the log record.
		/// <para>
		/// If the event involved an exception, this will be the
		/// exception object. Otherwise null.
		/// 
		/// </para>
		/// </summary>
		/// <returns> a throwable </returns>
		public virtual Throwable Thrown
		{
			get
			{
				return Thrown_Renamed;
			}
			set
			{
				this.Thrown_Renamed = value;
			}
		}


		private const long SerialVersionUID = 5372048053134512534L;

		/// <summary>
		/// @serialData Default fields, followed by a two byte version number
		/// (major byte, followed by minor byte), followed by information on
		/// the log record parameter array.  If there is no parameter array,
		/// then -1 is written.  If there is a parameter array (possible of zero
		/// length) then the array length is written as an integer, followed
		/// by String values for each parameter.  If a parameter is null, then
		/// a null String is written.  Otherwise the output of Object.toString()
		/// is written.
		/// </summary>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: private void writeObject(ObjectOutputStream out) throws IOException
		private void WriteObject(ObjectOutputStream @out)
		{
			// We have to call defaultWriteObject first.
			@out.DefaultWriteObject();

			// Write our version number.
			@out.WriteByte(1);
			@out.WriteByte(0);
			if (Parameters_Renamed == null)
			{
				@out.WriteInt(-1);
				return;
			}
			@out.WriteInt(Parameters_Renamed.Length);
			// Write string values for the parameters.
			for (int i = 0; i < Parameters_Renamed.Length; i++)
			{
				if (Parameters_Renamed[i] == null)
				{
					@out.WriteObject(null);
				}
				else
				{
					@out.WriteObject(Parameters_Renamed[i].ToString());
				}
			}
		}

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: private void readObject(ObjectInputStream in) throws IOException, ClassNotFoundException
		private void ReadObject(ObjectInputStream @in)
		{
			// We have to call defaultReadObject first.
			@in.DefaultReadObject();

			// Read version number.
			sbyte major = @in.ReadByte();
			sbyte minor = @in.ReadByte();
			if (major != 1)
			{
				throw new IOException("LogRecord: bad version: " + major + "." + minor);
			}
			int len = @in.ReadInt();
			if (len == -1)
			{
				Parameters_Renamed = null;
			}
			else
			{
				Parameters_Renamed = new Object[len];
				for (int i = 0; i < Parameters_Renamed.Length; i++)
				{
					Parameters_Renamed[i] = @in.ReadObject();
				}
			}
			// If necessary, try to regenerate the resource bundle.
			if (ResourceBundleName_Renamed != null)
			{
				try
				{
					// use system class loader to ensure the ResourceBundle
					// instance is a different instance than null loader uses
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final ResourceBundle bundle = ResourceBundle.getBundle(resourceBundleName, Locale.getDefault(), ClassLoader.getSystemClassLoader());
					ResourceBundle bundle = ResourceBundle.GetBundle(ResourceBundleName_Renamed, Locale.Default, ClassLoader.SystemClassLoader);
					ResourceBundle_Renamed = bundle;
				}
				catch (MissingResourceException)
				{
					// This is not a good place to throw an exception,
					// so we simply leave the resourceBundle null.
					ResourceBundle_Renamed = null;
				}
			}

			NeedToInferCaller = false;
		}

		// Private method to infer the caller's class and method names
		private void InferCaller()
		{
			NeedToInferCaller = false;
			JavaLangAccess access = SharedSecrets.JavaLangAccess;
			Throwable throwable = new Throwable();
			int depth = access.getStackTraceDepth(throwable);

			bool lookingForLogger = true;
			for (int ix = 0; ix < depth; ix++)
			{
				// Calling getStackTraceElement directly prevents the VM
				// from paying the cost of building the entire stack frame.
				StackTraceElement frame = access.getStackTraceElement(throwable, ix);
				String cname = frame.ClassName;
				bool isLoggerImpl = IsLoggerImplFrame(cname);
				if (lookingForLogger)
				{
					// Skip all frames until we have found the first logger frame.
					if (isLoggerImpl)
					{
						lookingForLogger = false;
					}
				}
				else
				{
					if (!isLoggerImpl)
					{
						// skip reflection call
						if (!cname.StartsWith("java.lang.reflect.") && !cname.StartsWith("sun.reflect."))
						{
						   // We've found the relevant frame.
						   SourceClassName = cname;
						   SourceMethodName = frame.MethodName;
						   return;
						}
					}
				}
			}
			// We haven't found a suitable frame, so just punt.  This is
			// OK as we are only committed to making a "best effort" here.
		}

		private bool IsLoggerImplFrame(String cname)
		{
			// the log record could be created for a platform logger
			return (cname.Equals("java.util.logging.Logger") || cname.StartsWith("java.util.logging.LoggingProxyImpl") || cname.StartsWith("sun.util.logging."));
		}
	}

}
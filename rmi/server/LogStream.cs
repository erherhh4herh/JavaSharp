using System;
using System.Threading;

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
namespace java.rmi.server
{


	/// <summary>
	/// <code>LogStream</code> provides a mechanism for logging errors that are
	/// of possible interest to those monitoring a system.
	/// 
	/// @author  Ann Wollrath (lots of code stolen from Ken Arnold)
	/// @since   JDK1.1 </summary>
	/// @deprecated no replacement 
	[Obsolete("no replacement")]
	public class LogStream : PrintStream
	{

		/// <summary>
		/// table mapping known log names to log stream objects </summary>
		private static Map<String, LogStream> Known = new HashMap<String, LogStream>(5);
		/// <summary>
		/// default output stream for new logs </summary>
		private static PrintStream DefaultStream_Renamed = System.err;

		/// <summary>
		/// log name for this log </summary>
		private String Name;

		/// <summary>
		/// stream where output of this log is sent to </summary>
		private OutputStream LogOut;

		/// <summary>
		/// string writer for writing message prefixes to log stream </summary>
		private OutputStreamWriter LogWriter;

		/// <summary>
		/// string buffer used for constructing log message prefixes </summary>
		private StringBuffer Buffer = new StringBuffer();

		/// <summary>
		/// stream used for buffering lines </summary>
		private ByteArrayOutputStream BufOut;

		/// <summary>
		/// Create a new LogStream object.  Since this only constructor is
		/// private, users must have a LogStream created through the "log"
		/// method. </summary>
		/// <param name="name"> string identifying messages from this log
		/// @out output stream that log messages will be sent to
		/// @since JDK1.1 </param>
		/// @deprecated no replacement 
		[Obsolete("no replacement")]
		private LogStream(String name, OutputStream @out) : base(new ByteArrayOutputStream())
		{
			BufOut = (ByteArrayOutputStream) base.@out;

			this.Name = name;
			OutputStream = @out;
		}

		/// <summary>
		/// Return the LogStream identified by the given name.  If
		/// a log corresponding to "name" does not exist, a log using
		/// the default stream is created. </summary>
		/// <param name="name"> name identifying the desired LogStream </param>
		/// <returns> log associated with given name
		/// @since JDK1.1 </returns>
		/// @deprecated no replacement 
		[Obsolete("no replacement")]
		public static LogStream Log(String name)
		{
			LogStream stream;
			lock (Known)
			{
				stream = Known.Get(name);
				if (stream == null)
				{
					stream = new LogStream(name, DefaultStream_Renamed);
				}
				Known.Put(name, stream);
			}
			return stream;
		}

		/// <summary>
		/// Return the current default stream for new logs. </summary>
		/// <returns> default log stream </returns>
		/// <seealso cref= #setDefaultStream
		/// @since JDK1.1 </seealso>
		/// @deprecated no replacement 
		[Obsolete("no replacement")]
		public static PrintStream DefaultStream
		{
			get
			{
				lock (typeof(LogStream))
				{
					return DefaultStream_Renamed;
				}
			}
			set
			{
				lock (typeof(LogStream))
				{
					SecurityManager sm = System.SecurityManager;
            
					if (sm != null)
					{
						sm.CheckPermission(new java.util.logging.LoggingPermission("control", null));
					}
            
					DefaultStream_Renamed = value;
				}
			}
		}


		/// <summary>
		/// Return the current stream to which output from this log is sent. </summary>
		/// <returns> output stream for this log </returns>
		/// <seealso cref= #setOutputStream
		/// @since JDK1.1 </seealso>
		/// @deprecated no replacement 
		[Obsolete("no replacement")]
		public virtual OutputStream OutputStream
		{
			get
			{
				lock (this)
				{
					return LogOut;
				}
			}
			set
			{
				lock (this)
				{
					LogOut = value;
					// Maintain an OutputStreamWriter with default CharToByteConvertor
					// (just like new PrintStream) for writing log message prefixes.
					LogWriter = new OutputStreamWriter(LogOut);
				}
			}
		}


		/// <summary>
		/// Write a byte of data to the stream.  If it is not a newline, then
		/// the byte is appended to the internal buffer.  If it is a newline,
		/// then the currently buffered line is sent to the log's output
		/// stream, prefixed with the appropriate logging information.
		/// @since JDK1.1 </summary>
		/// @deprecated no replacement 
		[Obsolete("no replacement")]
		public override void Write(int b)
		{
			if (b == '\n')
			{
				// synchronize on "this" first to avoid potential deadlock
				lock (this)
				{
					lock (LogOut)
					{
						// construct prefix for log messages:
						Buffer.Length = 0;
						Buffer.Append((DateTime.Now).ToString()); // date/time stamp...
						Buffer.Append(':');
						Buffer.Append(Name); // ...log name...
						Buffer.Append(':');
						Buffer.Append(Thread.CurrentThread.Name);
						Buffer.Append(':'); // ...and thread name

						try
						{
							// write prefix through to underlying byte stream
							LogWriter.Write(Buffer.ToString());
							LogWriter.Flush();

							// finally, write the already converted bytes of
							// the log message
							BufOut.WriteTo(LogOut);
							LogOut.Write(b);
							LogOut.Flush();
						}
						catch (IOException)
						{
							SetError();
						}
						finally
						{
							BufOut.Reset();
						}
					}
				}
			}
			else
			{
				base.Write(b);
			}
		}

		/// <summary>
		/// Write a subarray of bytes.  Pass each through write byte method.
		/// @since JDK1.1 </summary>
		/// @deprecated no replacement 
		[Obsolete("no replacement")]
		public override void Write(sbyte[] b, int off, int len)
		{
			if (len < 0)
			{
				throw new ArrayIndexOutOfBoundsException(len);
			}
			for (int i = 0; i < len; ++i)
			{
				Write(b[off + i]);
			}
		}

		/// <summary>
		/// Return log name as string representation. </summary>
		/// <returns> log name
		/// @since JDK1.1 </returns>
		/// @deprecated no replacement 
		[Obsolete("no replacement")]
		public override String ToString()
		{
			return Name;
		}

		/// <summary>
		/// log level constant (no logging). </summary>
		public const int SILENT = 0;
		/// <summary>
		/// log level constant (brief logging). </summary>
		public const int BRIEF = 10;
		/// <summary>
		/// log level constant (verbose logging). </summary>
		public const int VERBOSE = 20;

		/// <summary>
		/// Convert a string name of a logging level to its internal
		/// integer representation. </summary>
		/// <param name="s"> name of logging level (e.g., 'SILENT', 'BRIEF', 'VERBOSE') </param>
		/// <returns> corresponding integer log level
		/// @since JDK1.1 </returns>
		/// @deprecated no replacement 
		[Obsolete("no replacement")]
		public static int ParseLevel(String s)
		{
			if ((s == null) || (s.Length() < 1))
			{
				return -1;
			}

			try
			{
				return Convert.ToInt32(s);
			}
			catch (NumberFormatException)
			{
			}
			if (s.Length() < 1)
			{
				return -1;
			}

			if ("SILENT".StartsWith(s.ToUpperCase()))
			{
				return SILENT;
			}
			else if ("BRIEF".StartsWith(s.ToUpperCase()))
			{
				return BRIEF;
			}
			else if ("VERBOSE".StartsWith(s.ToUpperCase()))
			{
				return VERBOSE;
			}

			return -1;
		}
	}

}
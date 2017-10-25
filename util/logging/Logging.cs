using System.Collections.Generic;

/*
 * Copyright (c) 2003, 2013, Oracle and/or its affiliates. All rights reserved.
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
	/// Logging is the implementation class of LoggingMXBean.
	/// 
	/// The <tt>LoggingMXBean</tt> interface provides a standard
	/// method for management access to the individual
	/// {@code Logger} objects available at runtime.
	/// 
	/// @author Ron Mann
	/// @author Mandy Chung
	/// @since 1.5
	/// </summary>
	/// <seealso cref= javax.management </seealso>
	/// <seealso cref= Logger </seealso>
	/// <seealso cref= LogManager </seealso>
	internal class Logging : LoggingMXBean
	{

		private static LogManager LogManager = LogManager.LogManager;

		/// <summary>
		/// Constructor of Logging which is the implementation class
		///  of LoggingMXBean.
		/// </summary>
		internal Logging()
		{
		}

		public virtual IList<String> LoggerNames
		{
			get
			{
				IEnumerator<String> loggers = LogManager.LoggerNames;
				List<String> array = new List<String>();
    
				while (loggers.MoveNext())
				{
					array.Add(loggers.Current);
				}
				return array;
			}
		}

		private static String EMPTY_STRING = "";
		public virtual String GetLoggerLevel(String loggerName)
		{
			Logger l = LogManager.GetLogger(loggerName);
			if (l == null)
			{
				return null;
			}

			Level level = l.Level;
			if (level == null)
			{
				return EMPTY_STRING;
			}
			else
			{
				return level.LevelName;
			}
		}

		public virtual void SetLoggerLevel(String loggerName, String levelName)
		{
			if (loggerName == null)
			{
				throw new NullPointerException("loggerName is null");
			}

			Logger logger = LogManager.GetLogger(loggerName);
			if (logger == null)
			{
				throw new IllegalArgumentException("Logger " + loggerName + "does not exist");
			}

			Level level = null;
			if (levelName != null)
			{
				// parse will throw IAE if logLevel is invalid
				level = Level.FindLevel(levelName);
				if (level == null)
				{
					throw new IllegalArgumentException("Unknown level \"" + levelName + "\"");
				}
			}

			logger.Level = level;
		}

		public virtual String GetParentLoggerName(String loggerName)
		{
			Logger l = LogManager.GetLogger(loggerName);
			if (l == null)
			{
				return null;
			}

			Logger p = l.Parent;
			if (p == null)
			{
				// root logger
				return EMPTY_STRING;
			}
			else
			{
				return p.Name;
			}
		}
	}

}
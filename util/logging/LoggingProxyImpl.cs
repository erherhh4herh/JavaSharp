using System.Collections.Generic;

/*
 * Copyright (c) 2009, 2013, Oracle and/or its affiliates. All rights reserved.
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

	using LoggingProxy = sun.util.logging.LoggingProxy;

	/// <summary>
	/// Implementation of LoggingProxy when java.util.logging classes exist.
	/// </summary>
	internal class LoggingProxyImpl : LoggingProxy
	{
		internal static readonly LoggingProxy INSTANCE = new LoggingProxyImpl();

		private LoggingProxyImpl()
		{
		}

		public override Object GetLogger(String name)
		{
			// always create a platform logger with the resource bundle name
			return Logger.GetPlatformLogger(name);
		}

		public override Object GetLevel(Object logger)
		{
			return ((Logger) logger).Level;
		}

		public override void SetLevel(Object logger, Object newLevel)
		{
			((Logger) logger).Level = (Level) newLevel;
		}

		public override bool IsLoggable(Object logger, Object level)
		{
			return ((Logger) logger).IsLoggable((Level) level);
		}

		public override void Log(Object logger, Object level, String msg)
		{
			((Logger) logger).Log((Level) level, msg);
		}

		public override void Log(Object logger, Object level, String msg, Throwable t)
		{
			((Logger) logger).Log((Level) level, msg, t);
		}

		public override void Log(Object logger, Object level, String msg, params Object[] @params)
		{
			((Logger) logger).Log((Level) level, msg, @params);
		}

		public override IList<String> LoggerNames
		{
			get
			{
				return LogManager.LoggingMXBean.LoggerNames;
			}
		}

		public override String GetLoggerLevel(String loggerName)
		{
			return LogManager.LoggingMXBean.GetLoggerLevel(loggerName);
		}

		public override void SetLoggerLevel(String loggerName, String levelName)
		{
			LogManager.LoggingMXBean.SetLoggerLevel(loggerName, levelName);
		}

		public override String GetParentLoggerName(String loggerName)
		{
			return LogManager.LoggingMXBean.GetParentLoggerName(loggerName);
		}

		public override Object ParseLevel(String levelName)
		{
			Level level = Level.FindLevel(levelName);
			if (level == null)
			{
				throw new IllegalArgumentException("Unknown level \"" + levelName + "\"");
			}
			return level;
		}

		public override String GetLevelName(Object level)
		{
			return ((Level) level).LevelName;
		}

		public override int GetLevelValue(Object level)
		{
			return ((Level) level).IntValue();
		}

		public override String GetProperty(String key)
		{
			return LogManager.LogManager.GetProperty(key);
		}
	}

}
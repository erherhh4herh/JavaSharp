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
	/// A Filter can be used to provide fine grain control over
	/// what is logged, beyond the control provided by log levels.
	/// <para>
	/// Each Logger and each Handler can have a filter associated with it.
	/// The Logger or Handler will call the isLoggable method to check
	/// if a given LogRecord should be published.  If isLoggable returns
	/// false, the LogRecord will be discarded.
	/// 
	/// @since 1.4
	/// </para>
	/// </summary>
//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @FunctionalInterface public interface Filter
	public interface Filter
	{

		/// <summary>
		/// Check if a given log record should be published. </summary>
		/// <param name="record">  a LogRecord </param>
		/// <returns> true if the log record should be published. </returns>
		bool IsLoggable(LogRecord record);
	}

}
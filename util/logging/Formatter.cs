using System;

/*
 * Copyright (c) 2000, 2006, Oracle and/or its affiliates. All rights reserved.
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
	/// A Formatter provides support for formatting LogRecords.
	/// <para>
	/// Typically each logging Handler will have a Formatter associated
	/// with it.  The Formatter takes a LogRecord and converts it to
	/// a string.
	/// </para>
	/// <para>
	/// Some formatters (such as the XMLFormatter) need to wrap head
	/// and tail strings around a set of formatted records. The getHeader
	/// and getTail methods can be used to obtain these strings.
	/// 
	/// @since 1.4
	/// </para>
	/// </summary>

	public abstract class Formatter
	{

		/// <summary>
		/// Construct a new formatter.
		/// </summary>
		protected internal Formatter()
		{
		}

		/// <summary>
		/// Format the given log record and return the formatted string.
		/// <para>
		/// The resulting formatted String will normally include a
		/// localized and formatted version of the LogRecord's message field.
		/// It is recommended to use the <seealso cref="Formatter#formatMessage"/>
		/// convenience method to localize and format the message field.
		/// 
		/// </para>
		/// </summary>
		/// <param name="record"> the log record to be formatted. </param>
		/// <returns> the formatted log record </returns>
		public abstract String Format(LogRecord record);


		/// <summary>
		/// Return the header string for a set of formatted records.
		/// <para>
		/// This base class returns an empty string, but this may be
		/// overridden by subclasses.
		/// 
		/// </para>
		/// </summary>
		/// <param name="h">  The target handler (can be null) </param>
		/// <returns>  header string </returns>
		public virtual String GetHead(Handler h)
		{
			return "";
		}

		/// <summary>
		/// Return the tail string for a set of formatted records.
		/// <para>
		/// This base class returns an empty string, but this may be
		/// overridden by subclasses.
		/// 
		/// </para>
		/// </summary>
		/// <param name="h">  The target handler (can be null) </param>
		/// <returns>  tail string </returns>
		public virtual String GetTail(Handler h)
		{
			return "";
		}


		/// <summary>
		/// Localize and format the message string from a log record.  This
		/// method is provided as a convenience for Formatter subclasses to
		/// use when they are performing formatting.
		/// <para>
		/// The message string is first localized to a format string using
		/// the record's ResourceBundle.  (If there is no ResourceBundle,
		/// or if the message key is not found, then the key is used as the
		/// format string.)  The format String uses java.text style
		/// formatting.
		/// <ul>
		/// <li>If there are no parameters, no formatter is used.
		/// <li>Otherwise, if the string contains "{0" then
		///     java.text.MessageFormat  is used to format the string.
		/// <li>Otherwise no formatting is performed.
		/// </ul>
		/// </para>
		/// <para>
		/// 
		/// </para>
		/// </summary>
		/// <param name="record">  the log record containing the raw message </param>
		/// <returns>   a localized and formatted message </returns>
		public virtual String FormatMessage(LogRecord record)
		{
			lock (this)
			{
				String format = record.Message;
				java.util.ResourceBundle catalog = record.ResourceBundle;
				if (catalog != null)
				{
					try
					{
						format = catalog.GetString(record.Message);
					}
					catch (java.util.MissingResourceException)
					{
						// Drop through.  Use record message as format
						format = record.Message;
					}
				}
				// Do the formatting.
				try
				{
					Object[] parameters = record.Parameters;
					if (parameters == null || parameters.Length == 0)
					{
						// No parameters.  Just return format string.
						return format;
					}
					// Is it a java.text style format?
					// Ideally we could match with
					// Pattern.compile("\\{\\d").matcher(format).find())
					// However the cost is 14% higher, so we cheaply check for
					// 1 of the first 4 parameters
					if (format.IndexOf("{0") >= 0 || format.IndexOf("{1") >= 0 || format.IndexOf("{2") >= 0 || format.IndexOf("{3") >= 0)
					{
						return java.text.MessageFormat.Format(format, parameters);
					}
					return format;
        
				}
				catch (Exception)
				{
					// Formatting failed: use localized format string.
					return format;
				}
			}
		}
	}

}
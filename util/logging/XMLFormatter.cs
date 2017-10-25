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
	/// Format a LogRecord into a standard XML format.
	/// <para>
	/// The DTD specification is provided as Appendix A to the
	/// Java Logging APIs specification.
	/// </para>
	/// <para>
	/// The XMLFormatter can be used with arbitrary character encodings,
	/// but it is recommended that it normally be used with UTF-8.  The
	/// character encoding can be set on the output Handler.
	/// 
	/// @since 1.4
	/// </para>
	/// </summary>

	public class XMLFormatter : Formatter
	{
		private LogManager Manager = LogManager.LogManager;

		// Append a two digit number.
		private void A2(StringBuilder sb, int x)
		{
			if (x < 10)
			{
				sb.Append('0');
			}
			sb.Append(x);
		}

		// Append the time and date in ISO 8601 format
		private void AppendISO8601(StringBuilder sb, long millis)
		{
			GregorianCalendar cal = new GregorianCalendar();
			cal.TimeInMillis = millis;
			sb.Append(cal.Get(Calendar.YEAR));
			sb.Append('-');
			A2(sb, cal.Get(Calendar.MONTH) + 1);
			sb.Append('-');
			A2(sb, cal.Get(Calendar.DAY_OF_MONTH));
			sb.Append('T');
			A2(sb, cal.Get(Calendar.HOUR_OF_DAY));
			sb.Append(':');
			A2(sb, cal.Get(Calendar.MINUTE));
			sb.Append(':');
			A2(sb, cal.Get(Calendar.SECOND));
		}

		// Append to the given StringBuilder an escaped version of the
		// given text string where XML special characters have been escaped.
		// For a null string we append "<null>"
		private void Escape(StringBuilder sb, String text)
		{
			if (text == null)
			{
				text = "<null>";
			}
			for (int i = 0; i < text.Length(); i++)
			{
				char ch = text.CharAt(i);
				if (ch == '<')
				{
					sb.Append("&lt;");
				}
				else if (ch == '>')
				{
					sb.Append("&gt;");
				}
				else if (ch == '&')
				{
					sb.Append("&amp;");
				}
				else
				{
					sb.Append(ch);
				}
			}
		}

		/// <summary>
		/// Format the given message to XML.
		/// <para>
		/// This method can be overridden in a subclass.
		/// It is recommended to use the <seealso cref="Formatter#formatMessage"/>
		/// convenience method to localize and format the message field.
		/// 
		/// </para>
		/// </summary>
		/// <param name="record"> the log record to be formatted. </param>
		/// <returns> a formatted log record </returns>
		public override String Format(LogRecord record)
		{
			StringBuilder sb = new StringBuilder(500);
			sb.Append("<record>\n");

			sb.Append("  <date>");
			AppendISO8601(sb, record.Millis);
			sb.Append("</date>\n");

			sb.Append("  <millis>");
			sb.Append(record.Millis);
			sb.Append("</millis>\n");

			sb.Append("  <sequence>");
			sb.Append(record.SequenceNumber);
			sb.Append("</sequence>\n");

			String name = record.LoggerName;
			if (name != null)
			{
				sb.Append("  <logger>");
				Escape(sb, name);
				sb.Append("</logger>\n");
			}

			sb.Append("  <level>");
			Escape(sb, record.Level.ToString());
			sb.Append("</level>\n");

			if (record.SourceClassName != null)
			{
				sb.Append("  <class>");
				Escape(sb, record.SourceClassName);
				sb.Append("</class>\n");
			}

			if (record.SourceMethodName != null)
			{
				sb.Append("  <method>");
				Escape(sb, record.SourceMethodName);
				sb.Append("</method>\n");
			}

			sb.Append("  <thread>");
			sb.Append(record.ThreadID);
			sb.Append("</thread>\n");

			if (record.Message != null)
			{
				// Format the message string and its accompanying parameters.
				String message = formatMessage(record);
				sb.Append("  <message>");
				Escape(sb, message);
				sb.Append("</message>");
				sb.Append("\n");
			}

			// If the message is being localized, output the key, resource
			// bundle name, and params.
			ResourceBundle bundle = record.ResourceBundle;
			try
			{
				if (bundle != null && bundle.GetString(record.Message) != null)
				{
					sb.Append("  <key>");
					Escape(sb, record.Message);
					sb.Append("</key>\n");
					sb.Append("  <catalog>");
					Escape(sb, record.ResourceBundleName);
					sb.Append("</catalog>\n");
				}
			}
			catch (Exception)
			{
				// The message is not in the catalog.  Drop through.
			}

			Object[] parameters = record.Parameters;
			//  Check to see if the parameter was not a messagetext format
			//  or was not null or empty
			if (parameters != null && parameters.Length != 0 && record.Message.IndexOf("{") == -1)
			{
				for (int i = 0; i < parameters.Length; i++)
				{
					sb.Append("  <param>");
					try
					{
						Escape(sb, parameters[i].ToString());
					}
					catch (Exception)
					{
						sb.Append("???");
					}
					sb.Append("</param>\n");
				}
			}

			if (record.Thrown != null)
			{
				// Report on the state of the throwable.
				Throwable th = record.Thrown;
				sb.Append("  <exception>\n");
				sb.Append("    <message>");
				Escape(sb, th.ToString());
				sb.Append("</message>\n");
				StackTraceElement[] trace = th.StackTrace;
				for (int i = 0; i < trace.Length; i++)
				{
					StackTraceElement frame = trace[i];
					sb.Append("    <frame>\n");
					sb.Append("      <class>");
					Escape(sb, frame.ClassName);
					sb.Append("</class>\n");
					sb.Append("      <method>");
					Escape(sb, frame.MethodName);
					sb.Append("</method>\n");
					// Check for a line number.
					if (frame.LineNumber >= 0)
					{
						sb.Append("      <line>");
						sb.Append(frame.LineNumber);
						sb.Append("</line>\n");
					}
					sb.Append("    </frame>\n");
				}
				sb.Append("  </exception>\n");
			}

			sb.Append("</record>\n");
			return sb.ToString();
		}

		/// <summary>
		/// Return the header string for a set of XML formatted records.
		/// </summary>
		/// <param name="h">  The target handler (can be null) </param>
		/// <returns>  a valid XML string </returns>
		public override String GetHead(Handler h)
		{
			StringBuilder sb = new StringBuilder();
			String encoding;
			sb.Append("<?xml version=\"1.0\"");

			if (h != null)
			{
				encoding = h.Encoding;
			}
			else
			{
				encoding = null;
			}

			if (encoding == null)
			{
				// Figure out the default encoding.
				encoding = Charset.DefaultCharset().Name();
			}
			// Try to map the encoding name to a canonical name.
			try
			{
				Charset cs = Charset.ForName(encoding);
				encoding = cs.Name();
			}
			catch (Exception)
			{
				// We hit problems finding a canonical name.
				// Just use the raw encoding name.
			}

			sb.Append(" encoding=\"");
			sb.Append(encoding);
			sb.Append("\"");
			sb.Append(" standalone=\"no\"?>\n");
			sb.Append("<!DOCTYPE log SYSTEM \"logger.dtd\">\n");
			sb.Append("<log>\n");
			return sb.ToString();
		}

		/// <summary>
		/// Return the tail string for a set of XML formatted records.
		/// </summary>
		/// <param name="h">  The target handler (can be null) </param>
		/// <returns>  a valid XML string </returns>
		public override String GetTail(Handler h)
		{
			return "</log>\n";
		}
	}

}
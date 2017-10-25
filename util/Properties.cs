using System;
using System.Collections.Generic;

/*
 * Copyright (c) 1995, 2013, Oracle and/or its affiliates. All rights reserved.
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

namespace java.util
{


	using XmlPropertiesProvider = sun.util.spi.XmlPropertiesProvider;

	/// <summary>
	/// The {@code Properties} class represents a persistent set of
	/// properties. The {@code Properties} can be saved to a stream
	/// or loaded from a stream. Each key and its corresponding value in
	/// the property list is a string.
	/// <para>
	/// A property list can contain another property list as its
	/// "defaults"; this second property list is searched if
	/// the property key is not found in the original property list.
	/// </para>
	/// <para>
	/// Because {@code Properties} inherits from {@code Hashtable}, the
	/// {@code put} and {@code putAll} methods can be applied to a
	/// {@code Properties} object.  Their use is strongly discouraged as they
	/// allow the caller to insert entries whose keys or values are not
	/// {@code Strings}.  The {@code setProperty} method should be used
	/// instead.  If the {@code store} or {@code save} method is called
	/// on a "compromised" {@code Properties} object that contains a
	/// non-{@code String} key or value, the call will fail. Similarly,
	/// the call to the {@code propertyNames} or {@code list} method
	/// will fail if it is called on a "compromised" {@code Properties}
	/// object that contains a non-{@code String} key.
	/// 
	/// </para>
	/// <para>
	/// The <seealso cref="#load(java.io.Reader) load(Reader)"/> <tt>/</tt>
	/// <seealso cref="#store(java.io.Writer, java.lang.String) store(Writer, String)"/>
	/// methods load and store properties from and to a character based stream
	/// in a simple line-oriented format specified below.
	/// 
	/// The <seealso cref="#load(java.io.InputStream) load(InputStream)"/> <tt>/</tt>
	/// <seealso cref="#store(java.io.OutputStream, java.lang.String) store(OutputStream, String)"/>
	/// methods work the same way as the load(Reader)/store(Writer, String) pair, except
	/// the input/output stream is encoded in ISO 8859-1 character encoding.
	/// Characters that cannot be directly represented in this encoding can be written using
	/// Unicode escapes as defined in section 3.3 of
	/// <cite>The Java&trade; Language Specification</cite>;
	/// only a single 'u' character is allowed in an escape
	/// sequence. The native2ascii tool can be used to convert property files to and
	/// from other character encodings.
	/// 
	/// </para>
	/// <para> The <seealso cref="#loadFromXML(InputStream)"/> and {@link
	/// #storeToXML(OutputStream, String, String)} methods load and store properties
	/// in a simple XML format.  By default the UTF-8 character encoding is used,
	/// however a specific encoding may be specified if required. Implementations
	/// are required to support UTF-8 and UTF-16 and may support other encodings.
	/// An XML properties document has the following DOCTYPE declaration:
	/// 
	/// <pre>
	/// &lt;!DOCTYPE properties SYSTEM "http://java.sun.com/dtd/properties.dtd"&gt;
	/// </pre>
	/// Note that the system URI (http://java.sun.com/dtd/properties.dtd) is
	/// <i>not</i> accessed when exporting or importing properties; it merely
	/// serves as a string to uniquely identify the DTD, which is:
	/// <pre>
	///    &lt;?xml version="1.0" encoding="UTF-8"?&gt;
	/// 
	///    &lt;!-- DTD for properties --&gt;
	/// 
	///    &lt;!ELEMENT properties ( comment?, entry* ) &gt;
	/// 
	///    &lt;!ATTLIST properties version CDATA #FIXED "1.0"&gt;
	/// 
	///    &lt;!ELEMENT comment (#PCDATA) &gt;
	/// 
	///    &lt;!ELEMENT entry (#PCDATA) &gt;
	/// 
	///    &lt;!ATTLIST entry key CDATA #REQUIRED&gt;
	/// </pre>
	/// 
	/// </para>
	/// <para>This class is thread-safe: multiple threads can share a single
	/// <tt>Properties</tt> object without the need for external synchronization.
	/// 
	/// </para>
	/// </summary>
	/// <seealso cref= <a href="../../../technotes/tools/solaris/native2ascii.html">native2ascii tool for Solaris</a> </seealso>
	/// <seealso cref= <a href="../../../technotes/tools/windows/native2ascii.html">native2ascii tool for Windows</a>
	/// 
	/// @author  Arthur van Hoff
	/// @author  Michael McCloskey
	/// @author  Xueming Shen
	/// @since   JDK1.0 </seealso>
	public class Properties : Dictionary<Object, Object>
	{
		/// <summary>
		/// use serialVersionUID from JDK 1.1.X for interoperability
		/// </summary>
		 private const long SerialVersionUID = 4112578634029874840L;

		/// <summary>
		/// A property list that contains default values for any keys not
		/// found in this property list.
		/// 
		/// @serial
		/// </summary>
		protected internal Properties Defaults;

		/// <summary>
		/// Creates an empty property list with no default values.
		/// </summary>
		public Properties() : this(Map_Fields.Null)
		{
		}

		/// <summary>
		/// Creates an empty property list with the specified defaults.
		/// </summary>
		/// <param name="defaults">   the defaults. </param>
		public Properties(Properties defaults)
		{
			this.Defaults = defaults;
		}

		/// <summary>
		/// Calls the <tt>Hashtable</tt> method {@code put}. Provided for
		/// parallelism with the <tt>getProperty</tt> method. Enforces use of
		/// strings for property keys and values. The value returned is the
		/// result of the <tt>Hashtable</tt> call to {@code put}.
		/// </summary>
		/// <param name="key"> the key to be placed into this property list. </param>
		/// <param name="value"> the value corresponding to <tt>key</tt>. </param>
		/// <returns>     the previous value of the specified key in this property
		///             list, or {@code null} if it did not have one. </returns>
		/// <seealso cref= #getProperty
		/// @since    1.2 </seealso>
		public virtual Object SetProperty(String key, String value)
		{
			lock (this)
			{
				return this[key] = value;
			}
		}


		/// <summary>
		/// Reads a property list (key and element pairs) from the input
		/// character stream in a simple line-oriented format.
		/// <para>
		/// Properties are processed in terms of lines. There are two
		/// kinds of line, <i>natural lines</i> and <i>logical lines</i>.
		/// A natural line is defined as a line of
		/// characters that is terminated either by a set of line terminator
		/// characters ({@code \n} or {@code \r} or {@code \r\n})
		/// or by the end of the stream. A natural line may be either a blank line,
		/// a comment line, or hold all or some of a key-element pair. A logical
		/// line holds all the data of a key-element pair, which may be spread
		/// out across several adjacent natural lines by escaping
		/// the line terminator sequence with a backslash character
		/// {@code \}.  Note that a comment line cannot be extended
		/// in this manner; every natural line that is a comment must have
		/// its own comment indicator, as described below. Lines are read from
		/// input until the end of the stream is reached.
		/// 
		/// </para>
		/// <para>
		/// A natural line that contains only white space characters is
		/// considered blank and is ignored.  A comment line has an ASCII
		/// {@code '#'} or {@code '!'} as its first non-white
		/// space character; comment lines are also ignored and do not
		/// encode key-element information.  In addition to line
		/// terminators, this format considers the characters space
		/// ({@code ' '}, {@code '\u005Cu0020'}), tab
		/// ({@code '\t'}, {@code '\u005Cu0009'}), and form feed
		/// ({@code '\f'}, {@code '\u005Cu000C'}) to be white
		/// space.
		/// 
		/// </para>
		/// <para>
		/// If a logical line is spread across several natural lines, the
		/// backslash escaping the line terminator sequence, the line
		/// terminator sequence, and any white space at the start of the
		/// following line have no affect on the key or element values.
		/// The remainder of the discussion of key and element parsing
		/// (when loading) will assume all the characters constituting
		/// the key and element appear on a single natural line after
		/// line continuation characters have been removed.  Note that
		/// it is <i>not</i> sufficient to only examine the character
		/// preceding a line terminator sequence to decide if the line
		/// terminator is escaped; there must be an odd number of
		/// contiguous backslashes for the line terminator to be escaped.
		/// Since the input is processed from left to right, a
		/// non-zero even number of 2<i>n</i> contiguous backslashes
		/// before a line terminator (or elsewhere) encodes <i>n</i>
		/// backslashes after escape processing.
		/// 
		/// </para>
		/// <para>
		/// The key contains all of the characters in the line starting
		/// with the first non-white space character and up to, but not
		/// including, the first unescaped {@code '='},
		/// {@code ':'}, or white space character other than a line
		/// terminator. All of these key termination characters may be
		/// included in the key by escaping them with a preceding backslash
		/// </para>
		/// character; for example,<para>
		/// 
		/// </para>
		/// {@code \:\=}<para>
		/// 
		/// would be the two-character key {@code ":="}.  Line
		/// terminator characters can be included using {@code \r} and
		/// {@code \n} escape sequences.  Any white space after the
		/// key is skipped; if the first non-white space character after
		/// the key is {@code '='} or {@code ':'}, then it is
		/// ignored and any white space characters after it are also
		/// skipped.  All remaining characters on the line become part of
		/// the associated element string; if there are no remaining
		/// characters, the element is the empty string
		/// {@code ""}.  Once the raw character sequences
		/// constituting the key and element are identified, escape
		/// processing is performed as described above.
		/// 
		/// </para>
		/// <para>
		/// As an example, each of the following three lines specifies the key
		/// {@code "Truth"} and the associated element value
		/// {@code "Beauty"}:
		/// <pre>
		/// Truth = Beauty
		///  Truth:Beauty
		/// Truth                    :Beauty
		/// </pre>
		/// As another example, the following three lines specify a single
		/// property:
		/// <pre>
		/// fruits                           apple, banana, pear, \
		///                                  cantaloupe, watermelon, \
		///                                  kiwi, mango
		/// </pre>
		/// The key is {@code "fruits"} and the associated element is:
		/// <pre>"apple, banana, pear, cantaloupe, watermelon, kiwi, mango"</pre>
		/// Note that a space appears before each {@code \} so that a space
		/// will appear after each comma in the final result; the {@code \},
		/// line terminator, and leading white space on the continuation line are
		/// merely discarded and are <i>not</i> replaced by one or more other
		/// characters.
		/// </para>
		/// <para>
		/// As a third example, the line:
		/// <pre>cheeses
		/// </pre>
		/// specifies that the key is {@code "cheeses"} and the associated
		/// element is the empty string {@code ""}.
		/// </para>
		/// <para>
		/// <a name="unicodeescapes"></a>
		/// Characters in keys and elements can be represented in escape
		/// sequences similar to those used for character and string literals
		/// (see sections 3.3 and 3.10.6 of
		/// <cite>The Java&trade; Language Specification</cite>).
		/// 
		/// The differences from the character escape sequences and Unicode
		/// escapes used for characters and strings are:
		/// 
		/// <ul>
		/// <li> Octal escapes are not recognized.
		/// 
		/// <li> The character sequence {@code \b} does <i>not</i>
		/// represent a backspace character.
		/// 
		/// <li> The method does not treat a backslash character,
		/// {@code \}, before a non-valid escape character as an
		/// error; the backslash is silently dropped.  For example, in a
		/// Java string the sequence {@code "\z"} would cause a
		/// compile time error.  In contrast, this method silently drops
		/// the backslash.  Therefore, this method treats the two character
		/// sequence {@code "\b"} as equivalent to the single
		/// character {@code 'b'}.
		/// 
		/// <li> Escapes are not necessary for single and double quotes;
		/// however, by the rule above, single and double quote characters
		/// preceded by a backslash still yield single and double quote
		/// characters, respectively.
		/// 
		/// <li> Only a single 'u' character is allowed in a Unicode escape
		/// sequence.
		/// 
		/// </ul>
		/// </para>
		/// <para>
		/// The specified stream remains open after this method returns.
		/// 
		/// </para>
		/// </summary>
		/// <param name="reader">   the input character stream. </param>
		/// <exception cref="IOException">  if an error occurred when reading from the
		///          input stream. </exception>
		/// <exception cref="IllegalArgumentException"> if a malformed Unicode escape
		///          appears in the input.
		/// @since   1.6 </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public synchronized void load(java.io.Reader reader) throws java.io.IOException
		public virtual void Load(Reader reader)
		{
			lock (this)
			{
				Load0(new LineReader(this, reader));
			}
		}

		/// <summary>
		/// Reads a property list (key and element pairs) from the input
		/// byte stream. The input stream is in a simple line-oriented
		/// format as specified in
		/// <seealso cref="#load(java.io.Reader) load(Reader)"/> and is assumed to use
		/// the ISO 8859-1 character encoding; that is each byte is one Latin1
		/// character. Characters not in Latin1, and certain special characters,
		/// are represented in keys and elements using Unicode escapes as defined in
		/// section 3.3 of
		/// <cite>The Java&trade; Language Specification</cite>.
		/// <para>
		/// The specified stream remains open after this method returns.
		/// 
		/// </para>
		/// </summary>
		/// <param name="inStream">   the input stream. </param>
		/// <exception cref="IOException">  if an error occurred when reading from the
		///             input stream. </exception>
		/// <exception cref="IllegalArgumentException"> if the input stream contains a
		///             malformed Unicode escape sequence.
		/// @since 1.2 </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public synchronized void load(java.io.InputStream inStream) throws java.io.IOException
		public virtual void Load(InputStream inStream)
		{
			lock (this)
			{
				Load0(new LineReader(this, inStream));
			}
		}

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: private void load0(LineReader lr) throws java.io.IOException
		private void Load0(LineReader lr)
		{
			char[] convtBuf = new char[1024];
			int limit;
			int keyLen;
			int valueStart;
			char c;
			bool hasSep;
			bool precedingBackslash;

			while ((limit = lr.ReadLine()) >= 0)
			{
				c = (char)0;
				keyLen = 0;
				valueStart = limit;
				hasSep = Map_Fields.False;

				//System.out.println("line=<" + new String(lineBuf, 0, limit) + ">");
				precedingBackslash = Map_Fields.False;
				while (keyLen < limit)
				{
					c = lr.LineBuf[keyLen];
					//need check if escaped.
					if ((c == '=' || c == ':') && !precedingBackslash)
					{
						valueStart = keyLen + 1;
						hasSep = Map_Fields.True;
						break;
					}
					else if ((c == ' ' || c == '\t' || c == '\f') && !precedingBackslash)
					{
						valueStart = keyLen + 1;
						break;
					}
					if (c == '\\')
					{
						precedingBackslash = !precedingBackslash;
					}
					else
					{
						precedingBackslash = Map_Fields.False;
					}
					keyLen++;
				}
				while (valueStart < limit)
				{
					c = lr.LineBuf[valueStart];
					if (c != ' ' && c != '\t' && c != '\f')
					{
						if (!hasSep && (c == '=' || c == ':'))
						{
							hasSep = Map_Fields.True;
						}
						else
						{
							break;
						}
					}
					valueStart++;
				}
				String key = LoadConvert(lr.LineBuf, 0, keyLen, convtBuf);
				String value = LoadConvert(lr.LineBuf, valueStart, limit - valueStart, convtBuf);
				this[key] = value;
			}
		}

		/* Read in a "logical line" from an InputStream/Reader, skip all comment
		 * and blank lines and filter out those leading whitespace characters
		 * (\u0020, \u0009 and \u000c) from the beginning of a "natural line".
		 * Method returns the char length of the "logical line" and stores
		 * the line in "lineBuf".
		 */
		internal class LineReader
		{
			private readonly Properties OuterInstance;

			public LineReader(Properties outerInstance, InputStream inStream)
			{
				this.OuterInstance = outerInstance;
				this.InStream = inStream;
				InByteBuf = new sbyte[8192];
			}

			public LineReader(Properties outerInstance, Reader reader)
			{
				this.OuterInstance = outerInstance;
				this.Reader = reader;
				InCharBuf = new char[8192];
			}

			internal sbyte[] InByteBuf;
			internal char[] InCharBuf;
			internal char[] LineBuf = new char[1024];
			internal int InLimit = 0;
			internal int InOff = 0;
			internal InputStream InStream;
			internal Reader Reader;

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: int readLine() throws java.io.IOException
			internal virtual int ReadLine()
			{
				int len = 0;
				char c = (char)0;

				bool skipWhiteSpace = Map_Fields.True;
				bool isCommentLine = Map_Fields.False;
				bool isNewLine = Map_Fields.True;
				bool appendedLineBegin = Map_Fields.False;
				bool precedingBackslash = Map_Fields.False;
				bool skipLF = Map_Fields.False;

				while (Map_Fields.True)
				{
					if (InOff >= InLimit)
					{
						InLimit = (InStream == Map_Fields.Null)?Reader.Read(InCharBuf) :InStream.Read(InByteBuf);
						InOff = 0;
						if (InLimit <= 0)
						{
							if (len == 0 || isCommentLine)
							{
								return -1;
							}
							if (precedingBackslash)
							{
								len--;
							}
							return len;
						}
					}
					if (InStream != Map_Fields.Null)
					{
						//The line below is equivalent to calling a
						//ISO8859-1 decoder.
						c = (char)(0xff & InByteBuf[InOff++]);
					}
					else
					{
						c = InCharBuf[InOff++];
					}
					if (skipLF)
					{
						skipLF = Map_Fields.False;
						if (c == '\n')
						{
							continue;
						}
					}
					if (skipWhiteSpace)
					{
						if (c == ' ' || c == '\t' || c == '\f')
						{
							continue;
						}
						if (!appendedLineBegin && (c == '\r' || c == '\n'))
						{
							continue;
						}
						skipWhiteSpace = Map_Fields.False;
						appendedLineBegin = Map_Fields.False;
					}
					if (isNewLine)
					{
						isNewLine = Map_Fields.False;
						if (c == '#' || c == '!')
						{
							isCommentLine = Map_Fields.True;
							continue;
						}
					}

					if (c != '\n' && c != '\r')
					{
						LineBuf[len++] = c;
						if (len == LineBuf.Length)
						{
							int newLength = LineBuf.Length * 2;
							if (newLength < 0)
							{
								newLength = Integer.MaxValue;
							}
							char[] buf = new char[newLength];
							System.Array.Copy(LineBuf, 0, buf, 0, LineBuf.Length);
							LineBuf = buf;
						}
						//flip the preceding backslash flag
						if (c == '\\')
						{
							precedingBackslash = !precedingBackslash;
						}
						else
						{
							precedingBackslash = Map_Fields.False;
						}
					}
					else
					{
						// reached EOL
						if (isCommentLine || len == 0)
						{
							isCommentLine = Map_Fields.False;
							isNewLine = Map_Fields.True;
							skipWhiteSpace = Map_Fields.True;
							len = 0;
							continue;
						}
						if (InOff >= InLimit)
						{
							InLimit = (InStream == Map_Fields.Null) ?Reader.Read(InCharBuf) :InStream.Read(InByteBuf);
							InOff = 0;
							if (InLimit <= 0)
							{
								if (precedingBackslash)
								{
									len--;
								}
								return len;
							}
						}
						if (precedingBackslash)
						{
							len -= 1;
							//skip the leading whitespace characters in following line
							skipWhiteSpace = Map_Fields.True;
							appendedLineBegin = Map_Fields.True;
							precedingBackslash = Map_Fields.False;
							if (c == '\r')
							{
								skipLF = Map_Fields.True;
							}
						}
						else
						{
							return len;
						}
					}
				}
			}
		}

		/*
		 * Converts encoded &#92;uxxxx to unicode chars
		 * and changes special saved chars to their original forms
		 */
		private String LoadConvert(char[] @in, int off, int len, char[] convtBuf)
		{
			if (convtBuf.Length < len)
			{
				int newLen = len * 2;
				if (newLen < 0)
				{
					newLen = Integer.MaxValue;
				}
				convtBuf = new char[newLen];
			}
			char aChar;
			char[] @out = convtBuf;
			int outLen = 0;
			int end = off + len;

			while (off < end)
			{
				aChar = @in[off++];
				if (aChar == '\\')
				{
					aChar = @in[off++];
					if (aChar == 'u')
					{
						// Read the xxxx
						int value = 0;
						for (int i = 0; i < 4; i++)
						{
							aChar = @in[off++];
							switch (aChar)
							{
							  case '0':
						  case '1':
					  case '2':
				  case '3':
			  case '4':
							  case '5':
						  case '6':
					  case '7':
				  case '8':
			  case '9':
								 value = (value << 4) + aChar - '0';
								 break;
							  case 'a':
						  case 'b':
					  case 'c':
							  case 'd':
						  case 'e':
					  case 'f':
								 value = (value << 4) + 10 + aChar - 'a';
								 break;
							  case 'A':
						  case 'B':
					  case 'C':
							  case 'D':
						  case 'E':
					  case 'F':
								 value = (value << 4) + 10 + aChar - 'A';
								 break;
							  default:
								  throw new IllegalArgumentException("Malformed \\uxxxx encoding.");
							}
						}
						@out[outLen++] = (char)value;
					}
					else
					{
						if (aChar == 't')
						{
							aChar = '\t';
						}
						else if (aChar == 'r')
						{
							aChar = '\r';
						}
						else if (aChar == 'n')
						{
							aChar = '\n';
						}
						else if (aChar == 'f')
						{
							aChar = '\f';
						}
						@out[outLen++] = aChar;
					}
				}
				else
				{
					@out[outLen++] = aChar;
				}
			}
			return new String(@out, 0, outLen);
		}

		/*
		 * Converts unicodes to encoded &#92;uxxxx and escapes
		 * special characters with a preceding slash
		 */
		private String SaveConvert(String theString, bool escapeSpace, bool escapeUnicode)
		{
			int len = theString.Length();
			int bufLen = len * 2;
			if (bufLen < 0)
			{
				bufLen = Integer.MaxValue;
			}
			StringBuffer outBuffer = new StringBuffer(bufLen);

			for (int x = 0; x < len; x++)
			{
				char aChar = theString.CharAt(x);
				// Handle common case first, selecting largest block that
				// avoids the specials below
				if ((aChar > 61) && (aChar < 127))
				{
					if (aChar == '\\')
					{
						outBuffer.Append('\\');
						outBuffer.Append('\\');
						continue;
					}
					outBuffer.Append(aChar);
					continue;
				}
				switch (aChar)
				{
					case ' ':
						if (x == 0 || escapeSpace)
						{
							outBuffer.Append('\\');
						}
						outBuffer.Append(' ');
						break;
					case '\t':
						outBuffer.Append('\\');
						outBuffer.Append('t');
							  break;
					case '\n':
						outBuffer.Append('\\');
						outBuffer.Append('n');
							  break;
					case '\r':
						outBuffer.Append('\\');
						outBuffer.Append('r');
							  break;
					case '\f':
						outBuffer.Append('\\');
						outBuffer.Append('f');
							  break;
					case '=': // Fall through
					case ':': // Fall through
					case '#': // Fall through
					case '!':
						outBuffer.Append('\\');
						outBuffer.Append(aChar);
						break;
					default:
						if (((aChar < 0x0020) || (aChar > 0x007e)) & escapeUnicode)
						{
							outBuffer.Append('\\');
							outBuffer.Append('u');
							outBuffer.Append(ToHex((aChar >> 12) & 0xF));
							outBuffer.Append(ToHex((aChar >> 8) & 0xF));
							outBuffer.Append(ToHex((aChar >> 4) & 0xF));
							outBuffer.Append(ToHex(aChar & 0xF));
						}
						else
						{
							outBuffer.Append(aChar);
						}
					break;
				}
			}
			return outBuffer.ToString();
		}

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: private static void writeComments(java.io.BufferedWriter bw, String comments) throws java.io.IOException
		private static void WriteComments(BufferedWriter bw, String comments)
		{
			bw.Write("#");
			int len = comments.Length();
			int current = 0;
			int last = 0;
			char[] uu = new char[6];
			uu[0] = '\\';
			uu[1] = 'u';
			while (current < len)
			{
				char c = comments.CharAt(current);
				if (c > '\u00ff' || c == '\n' || c == '\r')
				{
					if (last != current)
					{
						bw.Write(comments.Substring(last, current - last));
					}
					if (c > '\u00ff')
					{
						uu[2] = ToHex((c >> 12) & 0xf);
						uu[3] = ToHex((c >> 8) & 0xf);
						uu[4] = ToHex((c >> 4) & 0xf);
						uu[5] = ToHex(c & 0xf);
						bw.Write(new String(uu));
					}
					else
					{
						bw.NewLine();
						if (c == '\r' && current != len - 1 && comments.CharAt(current + 1) == '\n')
						{
							current++;
						}
						if (current == len - 1 || (comments.CharAt(current + 1) != '#' && comments.CharAt(current + 1) != '!'))
						{
							bw.Write("#");
						}
					}
					last = current + 1;
				}
				current++;
			}
			if (last != current)
			{
				bw.Write(comments.Substring(last, current - last));
			}
			bw.NewLine();
		}

		/// <summary>
		/// Calls the {@code store(OutputStream out, String comments)} method
		/// and suppresses IOExceptions that were thrown.
		/// </summary>
		/// @deprecated This method does not throw an IOException if an I/O error
		/// occurs while saving the property list.  The preferred way to save a
		/// properties list is via the {@code store(OutputStream out,
		/// String comments)} method or the
		/// {@code storeToXML(OutputStream os, String comment)} method.
		/// 
		/// <param name="out">      an output stream. </param>
		/// <param name="comments">   a description of the property list. </param>
		/// <exception cref="ClassCastException">  if this {@code Properties} object
		///             contains any keys or values that are not
		///             {@code Strings}. </exception>
		[Obsolete("This method does not throw an IOException if an I/O error")]
		public virtual void Save(OutputStream @out, String comments)
		{
			try
			{
				Store(@out, comments);
			}
			catch (IOException)
			{
			}
		}

		/// <summary>
		/// Writes this property list (key and element pairs) in this
		/// {@code Properties} table to the output character stream in a
		/// format suitable for using the <seealso cref="#load(java.io.Reader) load(Reader)"/>
		/// method.
		/// <para>
		/// Properties from the defaults table of this {@code Properties}
		/// table (if any) are <i>not</i> written out by this method.
		/// </para>
		/// <para>
		/// If the comments argument is not null, then an ASCII {@code #}
		/// character, the comments string, and a line separator are first written
		/// to the output stream. Thus, the {@code comments} can serve as an
		/// identifying comment. Any one of a line feed ('\n'), a carriage
		/// return ('\r'), or a carriage return followed immediately by a line feed
		/// in comments is replaced by a line separator generated by the {@code Writer}
		/// and if the next character in comments is not character {@code #} or
		/// character {@code !} then an ASCII {@code #} is written out
		/// after that line separator.
		/// </para>
		/// <para>
		/// Next, a comment line is always written, consisting of an ASCII
		/// {@code #} character, the current date and time (as if produced
		/// by the {@code toString} method of {@code Date} for the
		/// current time), and a line separator as generated by the {@code Writer}.
		/// </para>
		/// <para>
		/// Then every entry in this {@code Properties} table is
		/// written out, one per line. For each entry the key string is
		/// written, then an ASCII {@code =}, then the associated
		/// element string. For the key, all space characters are
		/// written with a preceding {@code \} character.  For the
		/// element, leading space characters, but not embedded or trailing
		/// space characters, are written with a preceding {@code \}
		/// character. The key and element characters {@code #},
		/// {@code !}, {@code =}, and {@code :} are written
		/// with a preceding backslash to ensure that they are properly loaded.
		/// </para>
		/// <para>
		/// After the entries have been written, the output stream is flushed.
		/// The output stream remains open after this method returns.
		/// </para>
		/// <para>
		/// 
		/// </para>
		/// </summary>
		/// <param name="writer">      an output character stream writer. </param>
		/// <param name="comments">   a description of the property list. </param>
		/// <exception cref="IOException"> if writing this property list to the specified
		///             output stream throws an <tt>IOException</tt>. </exception>
		/// <exception cref="ClassCastException">  if this {@code Properties} object
		///             contains any keys or values that are not {@code Strings}. </exception>
		/// <exception cref="NullPointerException">  if {@code writer} is null.
		/// @since 1.6 </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public void store(java.io.Writer writer, String comments) throws java.io.IOException
		public virtual void Store(Writer writer, String comments)
		{
			Store0((writer is BufferedWriter)?(BufferedWriter)writer : new BufferedWriter(writer), comments, Map_Fields.False);
		}

		/// <summary>
		/// Writes this property list (key and element pairs) in this
		/// {@code Properties} table to the output stream in a format suitable
		/// for loading into a {@code Properties} table using the
		/// <seealso cref="#load(InputStream) load(InputStream)"/> method.
		/// <para>
		/// Properties from the defaults table of this {@code Properties}
		/// table (if any) are <i>not</i> written out by this method.
		/// </para>
		/// <para>
		/// This method outputs the comments, properties keys and values in
		/// the same format as specified in
		/// <seealso cref="#store(java.io.Writer, java.lang.String) store(Writer)"/>,
		/// with the following differences:
		/// <ul>
		/// <li>The stream is written using the ISO 8859-1 character encoding.
		/// 
		/// <li>Characters not in Latin-1 in the comments are written as
		/// {@code \u005Cu}<i>xxxx</i> for their appropriate unicode
		/// hexadecimal value <i>xxxx</i>.
		/// 
		/// <li>Characters less than {@code \u005Cu0020} and characters greater
		/// than {@code \u005Cu007E} in property keys or values are written
		/// as {@code \u005Cu}<i>xxxx</i> for the appropriate hexadecimal
		/// value <i>xxxx</i>.
		/// </ul>
		/// </para>
		/// <para>
		/// After the entries have been written, the output stream is flushed.
		/// The output stream remains open after this method returns.
		/// </para>
		/// <para>
		/// </para>
		/// </summary>
		/// <param name="out">      an output stream. </param>
		/// <param name="comments">   a description of the property list. </param>
		/// <exception cref="IOException"> if writing this property list to the specified
		///             output stream throws an <tt>IOException</tt>. </exception>
		/// <exception cref="ClassCastException">  if this {@code Properties} object
		///             contains any keys or values that are not {@code Strings}. </exception>
		/// <exception cref="NullPointerException">  if {@code out} is null.
		/// @since 1.2 </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public void store(java.io.OutputStream out, String comments) throws java.io.IOException
		public virtual void Store(OutputStream @out, String comments)
		{
			Store0(new BufferedWriter(new OutputStreamWriter(@out, "8859_1")), comments, Map_Fields.True);
		}

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: private void store0(java.io.BufferedWriter bw, String comments, boolean escUnicode) throws java.io.IOException
		private void Store0(BufferedWriter bw, String comments, bool escUnicode)
		{
			if (comments != Map_Fields.Null)
			{
				WriteComments(bw, comments);
			}
			bw.Write("#" + (DateTime.Now).ToString());
			bw.NewLine();
			lock (this)
			{
//JAVA TO C# CONVERTER TODO TASK: Java wildcard generics are not converted to .NET:
//ORIGINAL LINE: for (java.util.Iterator<?> e = keys(); e.hasMoreElements();)
				for (IEnumerator<?> e = this.Keys.GetEnumerator(); e.MoveNext();)
				{
					String key = (String)e.Current;
					String val = (String)this[key];
					key = SaveConvert(key, Map_Fields.True, escUnicode);
					/* No need to escape embedded and trailing spaces for value, hence
					 * pass false to flag.
					 */
					val = SaveConvert(val, Map_Fields.False, escUnicode);
					bw.Write(key + "=" + val);
					bw.NewLine();
				}
			}
			bw.Flush();
		}

		/// <summary>
		/// Loads all of the properties represented by the XML document on the
		/// specified input stream into this properties table.
		/// 
		/// <para>The XML document must have the following DOCTYPE declaration:
		/// <pre>
		/// &lt;!DOCTYPE properties SYSTEM "http://java.sun.com/dtd/properties.dtd"&gt;
		/// </pre>
		/// Furthermore, the document must satisfy the properties DTD described
		/// above.
		/// 
		/// </para>
		/// <para> An implementation is required to read XML documents that use the
		/// "{@code UTF-8}" or "{@code UTF-16}" encoding. An implementation may
		/// support additional encodings.
		/// 
		/// </para>
		/// <para>The specified stream is closed after this method returns.
		/// 
		/// </para>
		/// </summary>
		/// <param name="in"> the input stream from which to read the XML document. </param>
		/// <exception cref="IOException"> if reading from the specified input stream
		///         results in an <tt>IOException</tt>. </exception>
		/// <exception cref="java.io.UnsupportedEncodingException"> if the document's encoding
		///         declaration can be read and it specifies an encoding that is not
		///         supported </exception>
		/// <exception cref="InvalidPropertiesFormatException"> Data on input stream does not
		///         constitute a valid XML document with the mandated document type. </exception>
		/// <exception cref="NullPointerException"> if {@code in} is null. </exception>
		/// <seealso cref=    #storeToXML(OutputStream, String, String) </seealso>
		/// <seealso cref=    <a href="http://www.w3.org/TR/REC-xml/#charencoding">Character
		///         Encoding in Entities</a>
		/// @since 1.5 </seealso>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public synchronized void loadFromXML(java.io.InputStream in) throws java.io.IOException, InvalidPropertiesFormatException
		public virtual void LoadFromXML(InputStream @in)
		{
			lock (this)
			{
				XmlSupport.Load(this, Objects.RequireNonNull(@in));
				@in.Close();
			}
		}

		/// <summary>
		/// Emits an XML document representing all of the properties contained
		/// in this table.
		/// 
		/// <para> An invocation of this method of the form <tt>props.storeToXML(os,
		/// comment)</tt> behaves in exactly the same way as the invocation
		/// <tt>props.storeToXML(os, comment, "UTF-8");</tt>.
		/// 
		/// </para>
		/// </summary>
		/// <param name="os"> the output stream on which to emit the XML document. </param>
		/// <param name="comment"> a description of the property list, or {@code null}
		///        if no comment is desired. </param>
		/// <exception cref="IOException"> if writing to the specified output stream
		///         results in an <tt>IOException</tt>. </exception>
		/// <exception cref="NullPointerException"> if {@code os} is null. </exception>
		/// <exception cref="ClassCastException">  if this {@code Properties} object
		///         contains any keys or values that are not
		///         {@code Strings}. </exception>
		/// <seealso cref=    #loadFromXML(InputStream)
		/// @since 1.5 </seealso>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public void storeToXML(java.io.OutputStream os, String comment) throws java.io.IOException
		public virtual void StoreToXML(OutputStream os, String comment)
		{
			StoreToXML(os, comment, "UTF-8");
		}

		/// <summary>
		/// Emits an XML document representing all of the properties contained
		/// in this table, using the specified encoding.
		/// 
		/// <para>The XML document will have the following DOCTYPE declaration:
		/// <pre>
		/// &lt;!DOCTYPE properties SYSTEM "http://java.sun.com/dtd/properties.dtd"&gt;
		/// </pre>
		/// 
		/// </para>
		/// <para>If the specified comment is {@code null} then no comment
		/// will be stored in the document.
		/// 
		/// </para>
		/// <para> An implementation is required to support writing of XML documents
		/// that use the "{@code UTF-8}" or "{@code UTF-16}" encoding. An
		/// implementation may support additional encodings.
		/// 
		/// </para>
		/// <para>The specified stream remains open after this method returns.
		/// 
		/// </para>
		/// </summary>
		/// <param name="os">        the output stream on which to emit the XML document. </param>
		/// <param name="comment">   a description of the property list, or {@code null}
		///                  if no comment is desired. </param>
		/// <param name="encoding"> the name of a supported
		///                  <a href="../lang/package-summary.html#charenc">
		///                  character encoding</a>
		/// </param>
		/// <exception cref="IOException"> if writing to the specified output stream
		///         results in an <tt>IOException</tt>. </exception>
		/// <exception cref="java.io.UnsupportedEncodingException"> if the encoding is not
		///         supported by the implementation. </exception>
		/// <exception cref="NullPointerException"> if {@code os} is {@code null},
		///         or if {@code encoding} is {@code null}. </exception>
		/// <exception cref="ClassCastException">  if this {@code Properties} object
		///         contains any keys or values that are not
		///         {@code Strings}. </exception>
		/// <seealso cref=    #loadFromXML(InputStream) </seealso>
		/// <seealso cref=    <a href="http://www.w3.org/TR/REC-xml/#charencoding">Character
		///         Encoding in Entities</a>
		/// @since 1.5 </seealso>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public void storeToXML(java.io.OutputStream os, String comment, String encoding) throws java.io.IOException
		public virtual void StoreToXML(OutputStream os, String comment, String encoding)
		{
			XmlSupport.Save(this, Objects.RequireNonNull(os), comment, Objects.RequireNonNull(encoding));
		}

		/// <summary>
		/// Searches for the property with the specified key in this property list.
		/// If the key is not found in this property list, the default property list,
		/// and its defaults, recursively, are then checked. The method returns
		/// {@code null} if the property is not found.
		/// </summary>
		/// <param name="key">   the property key. </param>
		/// <returns>  the value in this property list with the specified key value. </returns>
		/// <seealso cref=     #setProperty </seealso>
		/// <seealso cref=     #defaults </seealso>
		public virtual String GetProperty(String key)
		{
			Object oval = base.Get(key);
			String sval = (oval is String) ? (String)oval : Map_Fields.Null;
			return ((sval == Map_Fields.Null) && (Defaults != Map_Fields.Null)) ? Defaults.GetProperty(key) : sval;
		}

		/// <summary>
		/// Searches for the property with the specified key in this property list.
		/// If the key is not found in this property list, the default property list,
		/// and its defaults, recursively, are then checked. The method returns the
		/// default value argument if the property is not found.
		/// </summary>
		/// <param name="key">            the hashtable key. </param>
		/// <param name="defaultValue">   a default value.
		/// </param>
		/// <returns>  the value in this property list with the specified key value. </returns>
		/// <seealso cref=     #setProperty </seealso>
		/// <seealso cref=     #defaults </seealso>
		public virtual String GetProperty(String key, String defaultValue)
		{
			String val = GetProperty(key);
			return (val == Map_Fields.Null) ? defaultValue : val;
		}

		/// <summary>
		/// Returns an enumeration of all the keys in this property list,
		/// including distinct keys in the default property list if a key
		/// of the same name has not already been found from the main
		/// properties list.
		/// </summary>
		/// <returns>  an enumeration of all the keys in this property list, including
		///          the keys in the default property list. </returns>
		/// <exception cref="ClassCastException"> if any key in this property list
		///          is not a string. </exception>
		/// <seealso cref=     java.util.Enumeration </seealso>
		/// <seealso cref=     java.util.Properties#defaults </seealso>
		/// <seealso cref=     #stringPropertyNames </seealso>
//JAVA TO C# CONVERTER TODO TASK: Java wildcard generics are not converted to .NET:
//ORIGINAL LINE: public java.util.Iterator<?> propertyNames()
		public virtual IEnumerator<?> PropertyNames()
		{
			Dictionary<String, Object> h = new Dictionary<String, Object>();
			Enumerate(h);
			return h.Keys();
		}

		/// <summary>
		/// Returns a set of keys in this property list where
		/// the key and its corresponding value are strings,
		/// including distinct keys in the default property list if a key
		/// of the same name has not already been found from the main
		/// properties list.  Properties whose key or value is not
		/// of type <tt>String</tt> are omitted.
		/// <para>
		/// The returned set is not backed by the <tt>Properties</tt> object.
		/// Changes to this <tt>Properties</tt> are not reflected in the set,
		/// or vice versa.
		/// 
		/// </para>
		/// </summary>
		/// <returns>  a set of keys in this property list where
		///          the key and its corresponding value are strings,
		///          including the keys in the default property list. </returns>
		/// <seealso cref=     java.util.Properties#defaults
		/// @since   1.6 </seealso>
		public virtual Set<String> StringPropertyNames()
		{
			Dictionary<String, String> h = new Dictionary<String, String>();
			EnumerateStringProperties(h);
			return h.KeySet();
		}

		/// <summary>
		/// Prints this property list out to the specified output stream.
		/// This method is useful for debugging.
		/// </summary>
		/// <param name="out">   an output stream. </param>
		/// <exception cref="ClassCastException"> if any key in this property list
		///          is not a string. </exception>
		public virtual void List(PrintStream @out)
		{
			@out.Println("-- listing properties --");
			Dictionary<String, Object> h = new Dictionary<String, Object>();
			Enumerate(h);
			for (IEnumerator<String> e = h.Keys(); e.MoveNext();)
			{
				String key = e.Current;
				String val = (String)h.Get(key);
				if (val.Length() > 40)
				{
					val = val.Substring(0, 37) + "...";
				}
				@out.Println(key + "=" + val);
			}
		}

		/// <summary>
		/// Prints this property list out to the specified output stream.
		/// This method is useful for debugging.
		/// </summary>
		/// <param name="out">   an output stream. </param>
		/// <exception cref="ClassCastException"> if any key in this property list
		///          is not a string.
		/// @since   JDK1.1 </exception>
		/*
		 * Rather than use an anonymous inner class to share common code, this
		 * method is duplicated in order to ensure that a non-1.1 compiler can
		 * compile this file.
		 */
		public virtual void List(PrintWriter @out)
		{
			@out.Println("-- listing properties --");
			Dictionary<String, Object> h = new Dictionary<String, Object>();
			Enumerate(h);
			for (IEnumerator<String> e = h.Keys(); e.MoveNext();)
			{
				String key = e.Current;
				String val = (String)h.Get(key);
				if (val.Length() > 40)
				{
					val = val.Substring(0, 37) + "...";
				}
				@out.Println(key + "=" + val);
			}
		}

		/// <summary>
		/// Enumerates all key/value pairs in the specified hashtable. </summary>
		/// <param name="h"> the hashtable </param>
		/// <exception cref="ClassCastException"> if any of the property keys
		///         is not of String type. </exception>
		private void Enumerate(Dictionary<String, Object> h)
		{
			lock (this)
			{
				if (Defaults != Map_Fields.Null)
				{
					Defaults.Enumerate(h);
				}
//JAVA TO C# CONVERTER TODO TASK: Java wildcard generics are not converted to .NET:
//ORIGINAL LINE: for (java.util.Iterator<?> e = keys() ; e.hasMoreElements() ;)
				for (IEnumerator<?> e = this.Keys.GetEnumerator(); e.MoveNext();)
				{
					String key = (String)e.Current;
					h.Put(key, this[key]);
				}
			}
		}

		/// <summary>
		/// Enumerates all key/value pairs in the specified hashtable
		/// and omits the property if the key or value is not a string. </summary>
		/// <param name="h"> the hashtable </param>
		private void EnumerateStringProperties(Dictionary<String, String> h)
		{
			lock (this)
			{
				if (Defaults != Map_Fields.Null)
				{
					Defaults.EnumerateStringProperties(h);
				}
//JAVA TO C# CONVERTER TODO TASK: Java wildcard generics are not converted to .NET:
//ORIGINAL LINE: for (java.util.Iterator<?> e = keys() ; e.hasMoreElements() ;)
				for (IEnumerator<?> e = this.Keys.GetEnumerator(); e.MoveNext();)
				{
					Object Map_Fields.k = e.Current;
					Object Map_Fields.v = this[Map_Fields.k];
					if (Map_Fields.k is String && Map_Fields.v is String)
					{
						h.Put((String) Map_Fields.k, (String) Map_Fields.v);
					}
				}
			}
		}

		/// <summary>
		/// Convert a nibble to a hex character </summary>
		/// <param name="nibble">  the nibble to convert. </param>
		private static char ToHex(int nibble)
		{
			return HexDigit[(nibble & 0xF)];
		}

		/// <summary>
		/// A table of hex digits </summary>
		private static readonly char[] HexDigit = new char[] {'0','1','2','3','4','5','6','7','8','9','A','B','C','D','E','F'};

		/// <summary>
		/// Supporting class for loading/storing properties in XML format.
		/// 
		/// <para> The {@code load} and {@code store} methods defined here delegate to a
		/// system-wide {@code XmlPropertiesProvider}. On first invocation of either
		/// method then the system-wide provider is located as follows: </para>
		/// 
		/// <ol>
		///   <li> If the system property {@code sun.util.spi.XmlPropertiesProvider}
		///   is defined then it is taken to be the full-qualified name of a concrete
		///   provider class. The class is loaded with the system class loader as the
		///   initiating loader. If it cannot be loaded or instantiated using a zero
		///   argument constructor then an unspecified error is thrown. </li>
		/// 
		///   <li> If the system property is not defined then the service-provider
		///   loading facility defined by the <seealso cref="ServiceLoader"/> class is used to
		///   locate a provider with the system class loader as the initiating
		///   loader and {@code sun.util.spi.XmlPropertiesProvider} as the service
		///   type. If this process fails then an unspecified error is thrown. If
		///   there is more than one service provider installed then it is
		///   not specified as to which provider will be used. </li>
		/// 
		///   <li> If the provider is not found by the above means then a system
		///   default provider will be instantiated and used. </li>
		/// </ol>
		/// </summary>
		private class XmlSupport
		{

			internal static XmlPropertiesProvider LoadProviderFromProperty(ClassLoader cl)
			{
				String cn = System.getProperty("sun.util.spi.XmlPropertiesProvider");
				if (cn == Map_Fields.Null)
				{
					return Map_Fields.Null;
				}
				try
				{
					Class c = Class.ForName(cn, Map_Fields.True, cl);
					return (XmlPropertiesProvider)c.NewInstance();
				}
//JAVA TO C# CONVERTER TODO TASK: There is no equivalent in C# to Java 'multi-catch' syntax:
				catch (ClassNotFoundException | IllegalAccessException | InstantiationException x)
				{
					throw new ServiceConfigurationError(Map_Fields.Null, x);
				}
			}

			internal static XmlPropertiesProvider LoadProviderAsService(ClassLoader cl)
			{
				Iterator<XmlPropertiesProvider> iterator = ServiceLoader.Load(typeof(XmlPropertiesProvider), cl).Iterator();
				return iterator.HasNext() ? iterator.Next() : Map_Fields.Null;
			}

			internal static XmlPropertiesProvider LoadProvider()
			{
				return AccessController.doPrivileged(new PrivilegedActionAnonymousInnerClassHelper());
			}

			private class PrivilegedActionAnonymousInnerClassHelper : PrivilegedAction<XmlPropertiesProvider>
			{
				public PrivilegedActionAnonymousInnerClassHelper()
				{
				}

				public virtual XmlPropertiesProvider Run()
				{
					ClassLoader cl = ClassLoader.SystemClassLoader;
					XmlPropertiesProvider provider = LoadProviderFromProperty(cl);
					if (provider != Map_Fields.Null)
					{
						return provider;
					}
					provider = LoadProviderAsService(cl);
					if (provider != Map_Fields.Null)
					{
						return provider;
					}
					return new jdk.@internal.util.xml.BasicXmlPropertiesProvider();
				}
			}

			internal static readonly XmlPropertiesProvider PROVIDER = LoadProvider();

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: static void load(Properties props, java.io.InputStream in) throws java.io.IOException, InvalidPropertiesFormatException
			internal static void Load(Properties props, InputStream @in)
			{
				PROVIDER.load(props, @in);
			}

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: static void save(Properties props, java.io.OutputStream os, String comment, String encoding) throws java.io.IOException
			internal static void Save(Properties props, OutputStream os, String comment, String encoding)
			{
				PROVIDER.store(props, os, comment, encoding);
			}
		}
	}

}
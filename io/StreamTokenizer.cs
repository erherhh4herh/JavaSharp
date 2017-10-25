using System;

/*
 * Copyright (c) 1995, 2012, Oracle and/or its affiliates. All rights reserved.
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

namespace java.io
{

	/// <summary>
	/// The {@code StreamTokenizer} class takes an input stream and
	/// parses it into "tokens", allowing the tokens to be
	/// read one at a time. The parsing process is controlled by a table
	/// and a number of flags that can be set to various states. The
	/// stream tokenizer can recognize identifiers, numbers, quoted
	/// strings, and various comment styles.
	/// <para>
	/// Each byte read from the input stream is regarded as a character
	/// in the range {@code '\u005Cu0000'} through {@code '\u005Cu00FF'}.
	/// The character value is used to look up five possible attributes of
	/// the character: <i>white space</i>, <i>alphabetic</i>,
	/// <i>numeric</i>, <i>string quote</i>, and <i>comment character</i>.
	/// Each character can have zero or more of these attributes.
	/// </para>
	/// <para>
	/// In addition, an instance has four flags. These flags indicate:
	/// <ul>
	/// <li>Whether line terminators are to be returned as tokens or treated
	///     as white space that merely separates tokens.
	/// <li>Whether C-style comments are to be recognized and skipped.
	/// <li>Whether C++-style comments are to be recognized and skipped.
	/// <li>Whether the characters of identifiers are converted to lowercase.
	/// </ul>
	/// </para>
	/// <para>
	/// A typical application first constructs an instance of this class,
	/// sets up the syntax tables, and then repeatedly loops calling the
	/// {@code nextToken} method in each iteration of the loop until
	/// it returns the value {@code TT_EOF}.
	/// 
	/// @author  James Gosling
	/// </para>
	/// </summary>
	/// <seealso cref=     java.io.StreamTokenizer#nextToken() </seealso>
	/// <seealso cref=     java.io.StreamTokenizer#TT_EOF
	/// @since   JDK1.0 </seealso>

	public class StreamTokenizer
	{

		/* Only one of these will be non-null */
		private Reader Reader = null;
		private InputStream Input = null;

		private char[] Buf = new char[20];

		/// <summary>
		/// The next character to be considered by the nextToken method.  May also
		/// be NEED_CHAR to indicate that a new character should be read, or SKIP_LF
		/// to indicate that a new character should be read and, if it is a '\n'
		/// character, it should be discarded and a second new character should be
		/// read.
		/// </summary>
		private int Peekc = NEED_CHAR;

		private const int NEED_CHAR = Integer.MaxValue;
		private static readonly int SKIP_LF = Integer.MaxValue - 1;

		private bool PushedBack;
		private bool ForceLower;
		/// <summary>
		/// The line number of the last token read </summary>
		private int LINENO = 1;

		private bool EolIsSignificantP = false;
		private bool SlashSlashCommentsP = false;
		private bool SlashStarCommentsP = false;

		private sbyte[] Ctype = new sbyte[256];
		private const sbyte CT_WHITESPACE = 1;
		private const sbyte CT_DIGIT = 2;
		private const sbyte CT_ALPHA = 4;
		private const sbyte CT_QUOTE = 8;
		private const sbyte CT_COMMENT = 16;

		/// <summary>
		/// After a call to the {@code nextToken} method, this field
		/// contains the type of the token just read. For a single character
		/// token, its value is the single character, converted to an integer.
		/// For a quoted string token, its value is the quote character.
		/// Otherwise, its value is one of the following:
		/// <ul>
		/// <li>{@code TT_WORD} indicates that the token is a word.
		/// <li>{@code TT_NUMBER} indicates that the token is a number.
		/// <li>{@code TT_EOL} indicates that the end of line has been read.
		///     The field can only have this value if the
		///     {@code eolIsSignificant} method has been called with the
		///     argument {@code true}.
		/// <li>{@code TT_EOF} indicates that the end of the input stream
		///     has been reached.
		/// </ul>
		/// <para>
		/// The initial value of this field is -4.
		/// 
		/// </para>
		/// </summary>
		/// <seealso cref=     java.io.StreamTokenizer#eolIsSignificant(boolean) </seealso>
		/// <seealso cref=     java.io.StreamTokenizer#nextToken() </seealso>
		/// <seealso cref=     java.io.StreamTokenizer#quoteChar(int) </seealso>
		/// <seealso cref=     java.io.StreamTokenizer#TT_EOF </seealso>
		/// <seealso cref=     java.io.StreamTokenizer#TT_EOL </seealso>
		/// <seealso cref=     java.io.StreamTokenizer#TT_NUMBER </seealso>
		/// <seealso cref=     java.io.StreamTokenizer#TT_WORD </seealso>
		public int Ttype = TT_NOTHING;

		/// <summary>
		/// A constant indicating that the end of the stream has been read.
		/// </summary>
		public const int TT_EOF = -1;

		/// <summary>
		/// A constant indicating that the end of the line has been read.
		/// </summary>
		public const int TT_EOL = '\n';

		/// <summary>
		/// A constant indicating that a number token has been read.
		/// </summary>
		public const int TT_NUMBER = -2;

		/// <summary>
		/// A constant indicating that a word token has been read.
		/// </summary>
		public const int TT_WORD = -3;

		/* A constant indicating that no token has been read, used for
		 * initializing ttype.  FIXME This could be made public and
		 * made available as the part of the API in a future release.
		 */
		private const int TT_NOTHING = -4;

		/// <summary>
		/// If the current token is a word token, this field contains a
		/// string giving the characters of the word token. When the current
		/// token is a quoted string token, this field contains the body of
		/// the string.
		/// <para>
		/// The current token is a word when the value of the
		/// {@code ttype} field is {@code TT_WORD}. The current token is
		/// a quoted string token when the value of the {@code ttype} field is
		/// a quote character.
		/// </para>
		/// <para>
		/// The initial value of this field is null.
		/// 
		/// </para>
		/// </summary>
		/// <seealso cref=     java.io.StreamTokenizer#quoteChar(int) </seealso>
		/// <seealso cref=     java.io.StreamTokenizer#TT_WORD </seealso>
		/// <seealso cref=     java.io.StreamTokenizer#ttype </seealso>
		public String Sval;

		/// <summary>
		/// If the current token is a number, this field contains the value
		/// of that number. The current token is a number when the value of
		/// the {@code ttype} field is {@code TT_NUMBER}.
		/// <para>
		/// The initial value of this field is 0.0.
		/// 
		/// </para>
		/// </summary>
		/// <seealso cref=     java.io.StreamTokenizer#TT_NUMBER </seealso>
		/// <seealso cref=     java.io.StreamTokenizer#ttype </seealso>
		public double Nval;

		/// <summary>
		/// Private constructor that initializes everything except the streams. </summary>
		private StreamTokenizer()
		{
			WordChars('a', 'z');
			WordChars('A', 'Z');
			WordChars(128 + 32, 255);
			WhitespaceChars(0, ' ');
			CommentChar('/');
			QuoteChar('"');
			QuoteChar('\'');
			ParseNumbers();
		}

		/// <summary>
		/// Creates a stream tokenizer that parses the specified input
		/// stream. The stream tokenizer is initialized to the following
		/// default state:
		/// <ul>
		/// <li>All byte values {@code 'A'} through {@code 'Z'},
		///     {@code 'a'} through {@code 'z'}, and
		///     {@code '\u005Cu00A0'} through {@code '\u005Cu00FF'} are
		///     considered to be alphabetic.
		/// <li>All byte values {@code '\u005Cu0000'} through
		///     {@code '\u005Cu0020'} are considered to be white space.
		/// <li>{@code '/'} is a comment character.
		/// <li>Single quote {@code '\u005C''} and double quote {@code '"'}
		///     are string quote characters.
		/// <li>Numbers are parsed.
		/// <li>Ends of lines are treated as white space, not as separate tokens.
		/// <li>C-style and C++-style comments are not recognized.
		/// </ul>
		/// </summary>
		/// @deprecated As of JDK version 1.1, the preferred way to tokenize an
		/// input stream is to convert it into a character stream, for example:
		/// <blockquote><pre>
		///   Reader r = new BufferedReader(new InputStreamReader(is));
		///   StreamTokenizer st = new StreamTokenizer(r);
		/// </pre></blockquote>
		/// 
		/// <param name="is">        an input stream. </param>
		/// <seealso cref=        java.io.BufferedReader </seealso>
		/// <seealso cref=        java.io.InputStreamReader </seealso>
		/// <seealso cref=        java.io.StreamTokenizer#StreamTokenizer(java.io.Reader) </seealso>
		[Obsolete("As of JDK version 1.1, the preferred way to tokenize an")]
		public StreamTokenizer(InputStream @is) : this()
		{
			if (@is == null)
			{
				throw new NullPointerException();
			}
			Input = @is;
		}

		/// <summary>
		/// Create a tokenizer that parses the given character stream.
		/// </summary>
		/// <param name="r">  a Reader object providing the input stream.
		/// @since   JDK1.1 </param>
		public StreamTokenizer(Reader r) : this()
		{
			if (r == null)
			{
				throw new NullPointerException();
			}
			Reader = r;
		}

		/// <summary>
		/// Resets this tokenizer's syntax table so that all characters are
		/// "ordinary." See the {@code ordinaryChar} method
		/// for more information on a character being ordinary.
		/// </summary>
		/// <seealso cref=     java.io.StreamTokenizer#ordinaryChar(int) </seealso>
		public virtual void ResetSyntax()
		{
			for (int i = Ctype.Length; --i >= 0;)
			{
				Ctype[i] = 0;
			}
		}

		/// <summary>
		/// Specifies that all characters <i>c</i> in the range
		/// <code>low&nbsp;&lt;=&nbsp;<i>c</i>&nbsp;&lt;=&nbsp;high</code>
		/// are word constituents. A word token consists of a word constituent
		/// followed by zero or more word constituents or number constituents.
		/// </summary>
		/// <param name="low">   the low end of the range. </param>
		/// <param name="hi">    the high end of the range. </param>
		public virtual void WordChars(int low, int hi)
		{
			if (low < 0)
			{
				low = 0;
			}
			if (hi >= Ctype.Length)
			{
				hi = Ctype.Length - 1;
			}
			while (low <= hi)
			{
				Ctype[low++] |= CT_ALPHA;
			}
		}

		/// <summary>
		/// Specifies that all characters <i>c</i> in the range
		/// <code>low&nbsp;&lt;=&nbsp;<i>c</i>&nbsp;&lt;=&nbsp;high</code>
		/// are white space characters. White space characters serve only to
		/// separate tokens in the input stream.
		/// 
		/// <para>Any other attribute settings for the characters in the specified
		/// range are cleared.
		/// 
		/// </para>
		/// </summary>
		/// <param name="low">   the low end of the range. </param>
		/// <param name="hi">    the high end of the range. </param>
		public virtual void WhitespaceChars(int low, int hi)
		{
			if (low < 0)
			{
				low = 0;
			}
			if (hi >= Ctype.Length)
			{
				hi = Ctype.Length - 1;
			}
			while (low <= hi)
			{
				Ctype[low++] = CT_WHITESPACE;
			}
		}

		/// <summary>
		/// Specifies that all characters <i>c</i> in the range
		/// <code>low&nbsp;&lt;=&nbsp;<i>c</i>&nbsp;&lt;=&nbsp;high</code>
		/// are "ordinary" in this tokenizer. See the
		/// {@code ordinaryChar} method for more information on a
		/// character being ordinary.
		/// </summary>
		/// <param name="low">   the low end of the range. </param>
		/// <param name="hi">    the high end of the range. </param>
		/// <seealso cref=     java.io.StreamTokenizer#ordinaryChar(int) </seealso>
		public virtual void OrdinaryChars(int low, int hi)
		{
			if (low < 0)
			{
				low = 0;
			}
			if (hi >= Ctype.Length)
			{
				hi = Ctype.Length - 1;
			}
			while (low <= hi)
			{
				Ctype[low++] = 0;
			}
		}

		/// <summary>
		/// Specifies that the character argument is "ordinary"
		/// in this tokenizer. It removes any special significance the
		/// character has as a comment character, word component, string
		/// delimiter, white space, or number character. When such a character
		/// is encountered by the parser, the parser treats it as a
		/// single-character token and sets {@code ttype} field to the
		/// character value.
		/// 
		/// <para>Making a line terminator character "ordinary" may interfere
		/// with the ability of a {@code StreamTokenizer} to count
		/// lines. The {@code lineno} method may no longer reflect
		/// the presence of such terminator characters in its line count.
		/// 
		/// </para>
		/// </summary>
		/// <param name="ch">   the character. </param>
		/// <seealso cref=     java.io.StreamTokenizer#ttype </seealso>
		public virtual void OrdinaryChar(int ch)
		{
			if (ch >= 0 && ch < Ctype.Length)
			{
				Ctype[ch] = 0;
			}
		}

		/// <summary>
		/// Specified that the character argument starts a single-line
		/// comment. All characters from the comment character to the end of
		/// the line are ignored by this stream tokenizer.
		/// 
		/// <para>Any other attribute settings for the specified character are cleared.
		/// 
		/// </para>
		/// </summary>
		/// <param name="ch">   the character. </param>
		public virtual void CommentChar(int ch)
		{
			if (ch >= 0 && ch < Ctype.Length)
			{
				Ctype[ch] = CT_COMMENT;
			}
		}

		/// <summary>
		/// Specifies that matching pairs of this character delimit string
		/// constants in this tokenizer.
		/// <para>
		/// When the {@code nextToken} method encounters a string
		/// constant, the {@code ttype} field is set to the string
		/// delimiter and the {@code sval} field is set to the body of
		/// the string.
		/// </para>
		/// <para>
		/// If a string quote character is encountered, then a string is
		/// recognized, consisting of all characters after (but not including)
		/// the string quote character, up to (but not including) the next
		/// occurrence of that same string quote character, or a line
		/// terminator, or end of file. The usual escape sequences such as
		/// {@code "\u005Cn"} and {@code "\u005Ct"} are recognized and
		/// converted to single characters as the string is parsed.
		/// 
		/// </para>
		/// <para>Any other attribute settings for the specified character are cleared.
		/// 
		/// </para>
		/// </summary>
		/// <param name="ch">   the character. </param>
		/// <seealso cref=     java.io.StreamTokenizer#nextToken() </seealso>
		/// <seealso cref=     java.io.StreamTokenizer#sval </seealso>
		/// <seealso cref=     java.io.StreamTokenizer#ttype </seealso>
		public virtual void QuoteChar(int ch)
		{
			if (ch >= 0 && ch < Ctype.Length)
			{
				Ctype[ch] = CT_QUOTE;
			}
		}

		/// <summary>
		/// Specifies that numbers should be parsed by this tokenizer. The
		/// syntax table of this tokenizer is modified so that each of the twelve
		/// characters:
		/// <blockquote><pre>
		///      0 1 2 3 4 5 6 7 8 9 . -
		/// </pre></blockquote>
		/// <para>
		/// has the "numeric" attribute.
		/// </para>
		/// <para>
		/// When the parser encounters a word token that has the format of a
		/// double precision floating-point number, it treats the token as a
		/// number rather than a word, by setting the {@code ttype}
		/// field to the value {@code TT_NUMBER} and putting the numeric
		/// value of the token into the {@code nval} field.
		/// 
		/// </para>
		/// </summary>
		/// <seealso cref=     java.io.StreamTokenizer#nval </seealso>
		/// <seealso cref=     java.io.StreamTokenizer#TT_NUMBER </seealso>
		/// <seealso cref=     java.io.StreamTokenizer#ttype </seealso>
		public virtual void ParseNumbers()
		{
			for (int i = '0'; i <= '9'; i++)
			{
				Ctype[i] |= CT_DIGIT;
			}
			Ctype['.'] |= CT_DIGIT;
			Ctype['-'] |= CT_DIGIT;
		}

		/// <summary>
		/// Determines whether or not ends of line are treated as tokens.
		/// If the flag argument is true, this tokenizer treats end of lines
		/// as tokens; the {@code nextToken} method returns
		/// {@code TT_EOL} and also sets the {@code ttype} field to
		/// this value when an end of line is read.
		/// <para>
		/// A line is a sequence of characters ending with either a
		/// carriage-return character ({@code '\u005Cr'}) or a newline
		/// character ({@code '\u005Cn'}). In addition, a carriage-return
		/// character followed immediately by a newline character is treated
		/// as a single end-of-line token.
		/// </para>
		/// <para>
		/// If the {@code flag} is false, end-of-line characters are
		/// treated as white space and serve only to separate tokens.
		/// 
		/// </para>
		/// </summary>
		/// <param name="flag">   {@code true} indicates that end-of-line characters
		///                 are separate tokens; {@code false} indicates that
		///                 end-of-line characters are white space. </param>
		/// <seealso cref=     java.io.StreamTokenizer#nextToken() </seealso>
		/// <seealso cref=     java.io.StreamTokenizer#ttype </seealso>
		/// <seealso cref=     java.io.StreamTokenizer#TT_EOL </seealso>
		public virtual void EolIsSignificant(bool flag)
		{
			EolIsSignificantP = flag;
		}

		/// <summary>
		/// Determines whether or not the tokenizer recognizes C-style comments.
		/// If the flag argument is {@code true}, this stream tokenizer
		/// recognizes C-style comments. All text between successive
		/// occurrences of {@code /*} and <code>*&#47;</code> are discarded.
		/// <para>
		/// If the flag argument is {@code false}, then C-style comments
		/// are not treated specially.
		/// 
		/// </para>
		/// </summary>
		/// <param name="flag">   {@code true} indicates to recognize and ignore
		///                 C-style comments. </param>
		public virtual void SlashStarComments(bool flag)
		{
			SlashStarCommentsP = flag;
		}

		/// <summary>
		/// Determines whether or not the tokenizer recognizes C++-style comments.
		/// If the flag argument is {@code true}, this stream tokenizer
		/// recognizes C++-style comments. Any occurrence of two consecutive
		/// slash characters ({@code '/'}) is treated as the beginning of
		/// a comment that extends to the end of the line.
		/// <para>
		/// If the flag argument is {@code false}, then C++-style
		/// comments are not treated specially.
		/// 
		/// </para>
		/// </summary>
		/// <param name="flag">   {@code true} indicates to recognize and ignore
		///                 C++-style comments. </param>
		public virtual void SlashSlashComments(bool flag)
		{
			SlashSlashCommentsP = flag;
		}

		/// <summary>
		/// Determines whether or not word token are automatically lowercased.
		/// If the flag argument is {@code true}, then the value in the
		/// {@code sval} field is lowercased whenever a word token is
		/// returned (the {@code ttype} field has the
		/// value {@code TT_WORD} by the {@code nextToken} method
		/// of this tokenizer.
		/// <para>
		/// If the flag argument is {@code false}, then the
		/// {@code sval} field is not modified.
		/// 
		/// </para>
		/// </summary>
		/// <param name="fl">   {@code true} indicates that all word tokens should
		///               be lowercased. </param>
		/// <seealso cref=     java.io.StreamTokenizer#nextToken() </seealso>
		/// <seealso cref=     java.io.StreamTokenizer#ttype </seealso>
		/// <seealso cref=     java.io.StreamTokenizer#TT_WORD </seealso>
		public virtual void LowerCaseMode(bool fl)
		{
			ForceLower = fl;
		}

		/// <summary>
		/// Read the next character </summary>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: private int read() throws IOException
		private int Read()
		{
			if (Reader != null)
			{
				return Reader.Read();
			}
			else if (Input != null)
			{
				return Input.Read();
			}
			else
			{
				throw new IllegalStateException();
			}
		}

		/// <summary>
		/// Parses the next token from the input stream of this tokenizer.
		/// The type of the next token is returned in the {@code ttype}
		/// field. Additional information about the token may be in the
		/// {@code nval} field or the {@code sval} field of this
		/// tokenizer.
		/// <para>
		/// Typical clients of this
		/// class first set up the syntax tables and then sit in a loop
		/// calling nextToken to parse successive tokens until TT_EOF
		/// is returned.
		/// 
		/// </para>
		/// </summary>
		/// <returns>     the value of the {@code ttype} field. </returns>
		/// <exception cref="IOException">  if an I/O error occurs. </exception>
		/// <seealso cref=        java.io.StreamTokenizer#nval </seealso>
		/// <seealso cref=        java.io.StreamTokenizer#sval </seealso>
		/// <seealso cref=        java.io.StreamTokenizer#ttype </seealso>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public int nextToken() throws IOException
		public virtual int NextToken()
		{
			if (PushedBack)
			{
				PushedBack = false;
				return Ttype;
			}
			sbyte[] ct = Ctype;
			Sval = null;

			int c = Peekc;
			if (c < 0)
			{
				c = NEED_CHAR;
			}
			if (c == SKIP_LF)
			{
				c = Read();
				if (c < 0)
				{
					return Ttype = TT_EOF;
				}
				if (c == '\n')
				{
					c = NEED_CHAR;
				}
			}
			if (c == NEED_CHAR)
			{
				c = Read();
				if (c < 0)
				{
					return Ttype = TT_EOF;
				}
			}
			Ttype = c; // Just to be safe

			/* Set peekc so that the next invocation of nextToken will read
			 * another character unless peekc is reset in this invocation
			 */
			Peekc = NEED_CHAR;

			int ctype = c < 256 ? ct[c] : CT_ALPHA;
			while ((ctype & CT_WHITESPACE) != 0)
			{
				if (c == '\r')
				{
					LINENO++;
					if (EolIsSignificantP)
					{
						Peekc = SKIP_LF;
						return Ttype = TT_EOL;
					}
					c = Read();
					if (c == '\n')
					{
						c = Read();
					}
				}
				else
				{
					if (c == '\n')
					{
						LINENO++;
						if (EolIsSignificantP)
						{
							return Ttype = TT_EOL;
						}
					}
					c = Read();
				}
				if (c < 0)
				{
					return Ttype = TT_EOF;
				}
				ctype = c < 256 ? ct[c] : CT_ALPHA;
			}

			if ((ctype & CT_DIGIT) != 0)
			{
				bool neg = false;
				if (c == '-')
				{
					c = Read();
					if (c != '.' && (c < '0' || c > '9'))
					{
						Peekc = c;
						return Ttype = '-';
					}
					neg = true;
				}
				double v = 0;
				int decexp = 0;
				int seendot = 0;
				while (true)
				{
					if (c == '.' && seendot == 0)
					{
						seendot = 1;
					}
					else if ('0' <= c && c <= '9')
					{
						v = v * 10 + (c - '0');
						decexp += seendot;
					}
					else
					{
						break;
					}
					c = Read();
				}
				Peekc = c;
				if (decexp != 0)
				{
					double denom = 10;
					decexp--;
					while (decexp > 0)
					{
						denom *= 10;
						decexp--;
					}
					/* Do one division of a likely-to-be-more-accurate number */
					v = v / denom;
				}
				Nval = neg ? - v : v;
				return Ttype = TT_NUMBER;
			}

			if ((ctype & CT_ALPHA) != 0)
			{
				int i = 0;
				do
				{
					if (i >= Buf.Length)
					{
						Buf = Arrays.CopyOf(Buf, Buf.Length * 2);
					}
					Buf[i++] = (char) c;
					c = Read();
					ctype = c < 0 ? CT_WHITESPACE : c < 256 ? ct[c] : CT_ALPHA;
				} while ((ctype & (CT_ALPHA | CT_DIGIT)) != 0);
				Peekc = c;
				Sval = String.CopyValueOf(Buf, 0, i);
				if (ForceLower)
				{
					Sval = Sval.ToLowerCase();
				}
				return Ttype = TT_WORD;
			}

			if ((ctype & CT_QUOTE) != 0)
			{
				Ttype = c;
				int i = 0;
				/* Invariants (because \Octal needs a lookahead):
				 *   (i)  c contains char value
				 *   (ii) d contains the lookahead
				 */
				int d = Read();
				while (d >= 0 && d != Ttype && d != '\n' && d != '\r')
				{
					if (d == '\\')
					{
						c = Read();
						int first = c; // To allow \377, but not \477
						if (c >= '0' && c <= '7')
						{
							c = c - '0';
							int c2 = Read();
							if ('0' <= c2 && c2 <= '7')
							{
								c = (c << 3) + (c2 - '0');
								c2 = Read();
								if ('0' <= c2 && c2 <= '7' && first <= '3')
								{
									c = (c << 3) + (c2 - '0');
									d = Read();
								}
								else
								{
									d = c2;
								}
							}
							else
							{
							  d = c2;
							}
						}
						else
						{
							switch (c)
							{
							case 'a':
								c = 0x7;
								break;
							case 'b':
								c = '\b';
								break;
							case 'f':
								c = 0xC;
								break;
							case 'n':
								c = '\n';
								break;
							case 'r':
								c = '\r';
								break;
							case 't':
								c = '\t';
								break;
							case 'v':
								c = 0xB;
								break;
							}
							d = Read();
						}
					}
					else
					{
						c = d;
						d = Read();
					}
					if (i >= Buf.Length)
					{
						Buf = Arrays.CopyOf(Buf, Buf.Length * 2);
					}
					Buf[i++] = (char)c;
				}

				/* If we broke out of the loop because we found a matching quote
				 * character then arrange to read a new character next time
				 * around; otherwise, save the character.
				 */
				Peekc = (d == Ttype) ? NEED_CHAR : d;

				Sval = String.CopyValueOf(Buf, 0, i);
				return Ttype;
			}

			if (c == '/' && (SlashSlashCommentsP || SlashStarCommentsP))
			{
				c = Read();
				if (c == '*' && SlashStarCommentsP)
				{
					int prevc = 0;
					while ((c = Read()) != '/' || prevc != '*')
					{
						if (c == '\r')
						{
							LINENO++;
							c = Read();
							if (c == '\n')
							{
								c = Read();
							}
						}
						else
						{
							if (c == '\n')
							{
								LINENO++;
								c = Read();
							}
						}
						if (c < 0)
						{
							return Ttype = TT_EOF;
						}
						prevc = c;
					}
					return NextToken();
				}
				else if (c == '/' && SlashSlashCommentsP)
				{
					while ((c = Read()) != '\n' && c != '\r' && c >= 0);
					Peekc = c;
					return NextToken();
				}
				else
				{
					/* Now see if it is still a single line comment */
					if ((ct['/'] & CT_COMMENT) != 0)
					{
						while ((c = Read()) != '\n' && c != '\r' && c >= 0);
						Peekc = c;
						return NextToken();
					}
					else
					{
						Peekc = c;
						return Ttype = '/';
					}
				}
			}

			if ((ctype & CT_COMMENT) != 0)
			{
				while ((c = Read()) != '\n' && c != '\r' && c >= 0);
				Peekc = c;
				return NextToken();
			}

			return Ttype = c;
		}

		/// <summary>
		/// Causes the next call to the {@code nextToken} method of this
		/// tokenizer to return the current value in the {@code ttype}
		/// field, and not to modify the value in the {@code nval} or
		/// {@code sval} field.
		/// </summary>
		/// <seealso cref=     java.io.StreamTokenizer#nextToken() </seealso>
		/// <seealso cref=     java.io.StreamTokenizer#nval </seealso>
		/// <seealso cref=     java.io.StreamTokenizer#sval </seealso>
		/// <seealso cref=     java.io.StreamTokenizer#ttype </seealso>
		public virtual void PushBack()
		{
			if (Ttype != TT_NOTHING) // No-op if nextToken() not called
			{
				PushedBack = true;
			}
		}

		/// <summary>
		/// Return the current line number.
		/// </summary>
		/// <returns>  the current line number of this stream tokenizer. </returns>
		public virtual int Lineno()
		{
			return LINENO;
		}

		/// <summary>
		/// Returns the string representation of the current stream token and
		/// the line number it occurs on.
		/// 
		/// <para>The precise string returned is unspecified, although the following
		/// example can be considered typical:
		/// 
		/// <blockquote><pre>Token['a'], line 10</pre></blockquote>
		/// 
		/// </para>
		/// </summary>
		/// <returns>  a string representation of the token </returns>
		/// <seealso cref=     java.io.StreamTokenizer#nval </seealso>
		/// <seealso cref=     java.io.StreamTokenizer#sval </seealso>
		/// <seealso cref=     java.io.StreamTokenizer#ttype </seealso>
		public override String ToString()
		{
			String ret;
			switch (Ttype)
			{
			  case TT_EOF:
				ret = "EOF";
				break;
			  case TT_EOL:
				ret = "EOL";
				break;
			  case TT_WORD:
				ret = Sval;
				break;
			  case TT_NUMBER:
				ret = "n=" + Nval;
				break;
			  case TT_NOTHING:
				ret = "NOTHING";
				break;
			  default:
			  {
					/*
					 * ttype is the first character of either a quoted string or
					 * is an ordinary character. ttype can definitely not be less
					 * than 0, since those are reserved values used in the previous
					 * case statements
					 */
					if (Ttype < 256 && ((Ctype[Ttype] & CT_QUOTE) != 0))
					{
						ret = Sval;
						break;
					}

					char[] s = new char[3];
					s[0] = s[2] = '\'';
					s[1] = (char) Ttype;
					ret = new String(s);
					break;
			  }
			}
			return "Token[" + ret + "], line " + LINENO;
		}

	}

}
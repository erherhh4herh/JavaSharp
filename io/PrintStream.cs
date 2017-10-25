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

namespace java.io
{


	/// <summary>
	/// A <code>PrintStream</code> adds functionality to another output stream,
	/// namely the ability to print representations of various data values
	/// conveniently.  Two other features are provided as well.  Unlike other output
	/// streams, a <code>PrintStream</code> never throws an
	/// <code>IOException</code>; instead, exceptional situations merely set an
	/// internal flag that can be tested via the <code>checkError</code> method.
	/// Optionally, a <code>PrintStream</code> can be created so as to flush
	/// automatically; this means that the <code>flush</code> method is
	/// automatically invoked after a byte array is written, one of the
	/// <code>println</code> methods is invoked, or a newline character or byte
	/// (<code>'\n'</code>) is written.
	/// 
	/// <para> All characters printed by a <code>PrintStream</code> are converted into
	/// bytes using the platform's default character encoding.  The <code>{@link
	/// PrintWriter}</code> class should be used in situations that require writing
	/// characters rather than bytes.
	/// 
	/// @author     Frank Yellin
	/// @author     Mark Reinhold
	/// @since      JDK1.0
	/// </para>
	/// </summary>

	public class PrintStream : FilterOutputStream, Appendable, Closeable
	{

		private readonly bool AutoFlush;
		private bool Trouble = false;
		private Formatter Formatter;

		/// <summary>
		/// Track both the text- and character-output streams, so that their buffers
		/// can be flushed without flushing the entire stream.
		/// </summary>
		private BufferedWriter TextOut;
		private OutputStreamWriter CharOut;

		/// <summary>
		/// requireNonNull is explicitly declared here so as not to create an extra
		/// dependency on java.util.Objects.requireNonNull. PrintStream is loaded
		/// early during system initialization.
		/// </summary>
		private static T requireNonNull<T>(T obj, String message)
		{
			if (obj == null)
			{
				throw new NullPointerException(message);
			}
			return obj;
		}

		/// <summary>
		/// Returns a charset object for the given charset name. </summary>
		/// <exception cref="NullPointerException">          is csn is null </exception>
		/// <exception cref="UnsupportedEncodingException">  if the charset is not supported </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: private static java.nio.charset.Charset toCharset(String csn) throws UnsupportedEncodingException
		private static Charset ToCharset(String csn)
		{
			RequireNonNull(csn, "charsetName");
			try
			{
				return Charset.ForName(csn);
			}
//JAVA TO C# CONVERTER TODO TASK: There is no equivalent in C# to Java 'multi-catch' syntax:
			catch (IllegalCharsetNameException | UnsupportedCharsetException unused)
			{
				// UnsupportedEncodingException should be thrown
				throw new UnsupportedEncodingException(csn);
			}
		}

		/* Private constructors */
		private PrintStream(bool autoFlush, OutputStream @out) : base(@out)
		{
			this.AutoFlush = autoFlush;
			this.CharOut = new OutputStreamWriter(this);
			this.TextOut = new BufferedWriter(CharOut);
		}

		private PrintStream(bool autoFlush, OutputStream @out, Charset charset) : base(@out)
		{
			this.AutoFlush = autoFlush;
			this.CharOut = new OutputStreamWriter(this, charset);
			this.TextOut = new BufferedWriter(CharOut);
		}

		/* Variant of the private constructor so that the given charset name
		 * can be verified before evaluating the OutputStream argument. Used
		 * by constructors creating a FileOutputStream that also take a
		 * charset name.
		 */
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: private PrintStream(boolean autoFlush, java.nio.charset.Charset charset, OutputStream out) throws UnsupportedEncodingException
		private PrintStream(bool autoFlush, Charset charset, OutputStream @out) : this(autoFlush, @out, charset)
		{
		}

		/// <summary>
		/// Creates a new print stream.  This stream will not flush automatically.
		/// </summary>
		/// <param name="out">        The output stream to which values and objects will be
		///                    printed
		/// </param>
		/// <seealso cref= java.io.PrintWriter#PrintWriter(java.io.OutputStream) </seealso>
		public PrintStream(OutputStream @out) : this(@out, false)
		{
		}

		/// <summary>
		/// Creates a new print stream.
		/// </summary>
		/// <param name="out">        The output stream to which values and objects will be
		///                    printed </param>
		/// <param name="autoFlush">  A boolean; if true, the output buffer will be flushed
		///                    whenever a byte array is written, one of the
		///                    <code>println</code> methods is invoked, or a newline
		///                    character or byte (<code>'\n'</code>) is written
		/// </param>
		/// <seealso cref= java.io.PrintWriter#PrintWriter(java.io.OutputStream, boolean) </seealso>
		public PrintStream(OutputStream @out, bool autoFlush) : this(autoFlush, RequireNonNull(@out, "Null output stream"))
		{
		}

		/// <summary>
		/// Creates a new print stream.
		/// </summary>
		/// <param name="out">        The output stream to which values and objects will be
		///                    printed </param>
		/// <param name="autoFlush">  A boolean; if true, the output buffer will be flushed
		///                    whenever a byte array is written, one of the
		///                    <code>println</code> methods is invoked, or a newline
		///                    character or byte (<code>'\n'</code>) is written </param>
		/// <param name="encoding">   The name of a supported
		///                    <a href="../lang/package-summary.html#charenc">
		///                    character encoding</a>
		/// </param>
		/// <exception cref="UnsupportedEncodingException">
		///          If the named encoding is not supported
		/// 
		/// @since  1.4 </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public PrintStream(OutputStream out, boolean autoFlush, String encoding) throws UnsupportedEncodingException
		public PrintStream(OutputStream @out, bool autoFlush, String encoding) : this(autoFlush, RequireNonNull(@out, "Null output stream"), ToCharset(encoding))
		{
		}

		/// <summary>
		/// Creates a new print stream, without automatic line flushing, with the
		/// specified file name.  This convenience constructor creates
		/// the necessary intermediate {@link java.io.OutputStreamWriter
		/// OutputStreamWriter}, which will encode characters using the
		/// <seealso cref="java.nio.charset.Charset#defaultCharset() default charset"/>
		/// for this instance of the Java virtual machine.
		/// </summary>
		/// <param name="fileName">
		///         The name of the file to use as the destination of this print
		///         stream.  If the file exists, then it will be truncated to
		///         zero size; otherwise, a new file will be created.  The output
		///         will be written to the file and is buffered.
		/// </param>
		/// <exception cref="FileNotFoundException">
		///          If the given file object does not denote an existing, writable
		///          regular file and a new regular file of that name cannot be
		///          created, or if some other error occurs while opening or
		///          creating the file
		/// </exception>
		/// <exception cref="SecurityException">
		///          If a security manager is present and {@link
		///          SecurityManager#checkWrite checkWrite(fileName)} denies write
		///          access to the file
		/// 
		/// @since  1.5 </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public PrintStream(String fileName) throws FileNotFoundException
		public PrintStream(String fileName) : this(false, new FileOutputStream(fileName))
		{
		}

		/// <summary>
		/// Creates a new print stream, without automatic line flushing, with the
		/// specified file name and charset.  This convenience constructor creates
		/// the necessary intermediate {@link java.io.OutputStreamWriter
		/// OutputStreamWriter}, which will encode characters using the provided
		/// charset.
		/// </summary>
		/// <param name="fileName">
		///         The name of the file to use as the destination of this print
		///         stream.  If the file exists, then it will be truncated to
		///         zero size; otherwise, a new file will be created.  The output
		///         will be written to the file and is buffered.
		/// </param>
		/// <param name="csn">
		///         The name of a supported {@link java.nio.charset.Charset
		///         charset}
		/// </param>
		/// <exception cref="FileNotFoundException">
		///          If the given file object does not denote an existing, writable
		///          regular file and a new regular file of that name cannot be
		///          created, or if some other error occurs while opening or
		///          creating the file
		/// </exception>
		/// <exception cref="SecurityException">
		///          If a security manager is present and {@link
		///          SecurityManager#checkWrite checkWrite(fileName)} denies write
		///          access to the file
		/// </exception>
		/// <exception cref="UnsupportedEncodingException">
		///          If the named charset is not supported
		/// 
		/// @since  1.5 </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public PrintStream(String fileName, String csn) throws FileNotFoundException, UnsupportedEncodingException
		public PrintStream(String fileName, String csn) : this(false, ToCharset(csn), new FileOutputStream(fileName))
		{
			// ensure charset is checked before the file is opened
		}

		/// <summary>
		/// Creates a new print stream, without automatic line flushing, with the
		/// specified file.  This convenience constructor creates the necessary
		/// intermediate <seealso cref="java.io.OutputStreamWriter OutputStreamWriter"/>,
		/// which will encode characters using the {@linkplain
		/// java.nio.charset.Charset#defaultCharset() default charset} for this
		/// instance of the Java virtual machine.
		/// </summary>
		/// <param name="file">
		///         The file to use as the destination of this print stream.  If the
		///         file exists, then it will be truncated to zero size; otherwise,
		///         a new file will be created.  The output will be written to the
		///         file and is buffered.
		/// </param>
		/// <exception cref="FileNotFoundException">
		///          If the given file object does not denote an existing, writable
		///          regular file and a new regular file of that name cannot be
		///          created, or if some other error occurs while opening or
		///          creating the file
		/// </exception>
		/// <exception cref="SecurityException">
		///          If a security manager is present and {@link
		///          SecurityManager#checkWrite checkWrite(file.getPath())}
		///          denies write access to the file
		/// 
		/// @since  1.5 </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public PrintStream(File file) throws FileNotFoundException
		public PrintStream(File file) : this(false, new FileOutputStream(file))
		{
		}

		/// <summary>
		/// Creates a new print stream, without automatic line flushing, with the
		/// specified file and charset.  This convenience constructor creates
		/// the necessary intermediate {@link java.io.OutputStreamWriter
		/// OutputStreamWriter}, which will encode characters using the provided
		/// charset.
		/// </summary>
		/// <param name="file">
		///         The file to use as the destination of this print stream.  If the
		///         file exists, then it will be truncated to zero size; otherwise,
		///         a new file will be created.  The output will be written to the
		///         file and is buffered.
		/// </param>
		/// <param name="csn">
		///         The name of a supported {@link java.nio.charset.Charset
		///         charset}
		/// </param>
		/// <exception cref="FileNotFoundException">
		///          If the given file object does not denote an existing, writable
		///          regular file and a new regular file of that name cannot be
		///          created, or if some other error occurs while opening or
		///          creating the file
		/// </exception>
		/// <exception cref="SecurityException">
		///          If a security manager is present and {@link
		///          SecurityManager#checkWrite checkWrite(file.getPath())}
		///          denies write access to the file
		/// </exception>
		/// <exception cref="UnsupportedEncodingException">
		///          If the named charset is not supported
		/// 
		/// @since  1.5 </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public PrintStream(File file, String csn) throws FileNotFoundException, UnsupportedEncodingException
		public PrintStream(File file, String csn) : this(false, ToCharset(csn), new FileOutputStream(file))
		{
			// ensure charset is checked before the file is opened
		}

		/// <summary>
		/// Check to make sure that the stream has not been closed </summary>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: private void ensureOpen() throws IOException
		private void EnsureOpen()
		{
			if (@out == null)
			{
				throw new IOException("Stream closed");
			}
		}

		/// <summary>
		/// Flushes the stream.  This is done by writing any buffered output bytes to
		/// the underlying output stream and then flushing that stream.
		/// </summary>
		/// <seealso cref=        java.io.OutputStream#flush() </seealso>
		public override void Flush()
		{
			lock (this)
			{
				try
				{
					EnsureOpen();
					@out.Flush();
				}
				catch (IOException)
				{
					Trouble = true;
				}
			}
		}

		private bool Closing = false; // To avoid recursive closing

		/// <summary>
		/// Closes the stream.  This is done by flushing the stream and then closing
		/// the underlying output stream.
		/// </summary>
		/// <seealso cref=        java.io.OutputStream#close() </seealso>
		public override void Close()
		{
			lock (this)
			{
				if (!Closing)
				{
					Closing = true;
					try
					{
						TextOut.Close();
						@out.Close();
					}
					catch (IOException)
					{
						Trouble = true;
					}
					TextOut = null;
					CharOut = null;
					@out = null;
				}
			}
		}

		/// <summary>
		/// Flushes the stream and checks its error state. The internal error state
		/// is set to <code>true</code> when the underlying output stream throws an
		/// <code>IOException</code> other than <code>InterruptedIOException</code>,
		/// and when the <code>setError</code> method is invoked.  If an operation
		/// on the underlying output stream throws an
		/// <code>InterruptedIOException</code>, then the <code>PrintStream</code>
		/// converts the exception back into an interrupt by doing:
		/// <pre>
		///     Thread.currentThread().interrupt();
		/// </pre>
		/// or the equivalent.
		/// </summary>
		/// <returns> <code>true</code> if and only if this stream has encountered an
		///         <code>IOException</code> other than
		///         <code>InterruptedIOException</code>, or the
		///         <code>setError</code> method has been invoked </returns>
		public virtual bool CheckError()
		{
			if (@out != null)
			{
				Flush();
			}
			if (@out is java.io.PrintStream)
			{
				PrintStream ps = (PrintStream) @out;
				return ps.CheckError();
			}
			return Trouble;
		}

		/// <summary>
		/// Sets the error state of the stream to <code>true</code>.
		/// 
		/// <para> This method will cause subsequent invocations of {@link
		/// #checkError()} to return <tt>true</tt> until {@link
		/// #clearError()} is invoked.
		/// 
		/// @since JDK1.1
		/// </para>
		/// </summary>
		protected internal virtual void SetError()
		{
			Trouble = true;
		}

		/// <summary>
		/// Clears the internal error state of this stream.
		/// 
		/// <para> This method will cause subsequent invocations of {@link
		/// #checkError()} to return <tt>false</tt> until another write
		/// operation fails and invokes <seealso cref="#setError()"/>.
		/// 
		/// @since 1.6
		/// </para>
		/// </summary>
		protected internal virtual void ClearError()
		{
			Trouble = false;
		}

		/*
		 * Exception-catching, synchronized output operations,
		 * which also implement the write() methods of OutputStream
		 */

		/// <summary>
		/// Writes the specified byte to this stream.  If the byte is a newline and
		/// automatic flushing is enabled then the <code>flush</code> method will be
		/// invoked.
		/// 
		/// <para> Note that the byte is written as given; to write a character that
		/// will be translated according to the platform's default character
		/// encoding, use the <code>print(char)</code> or <code>println(char)</code>
		/// methods.
		/// 
		/// </para>
		/// </summary>
		/// <param name="b">  The byte to be written </param>
		/// <seealso cref= #print(char) </seealso>
		/// <seealso cref= #println(char) </seealso>
		public override void Write(int b)
		{
			try
			{
				lock (this)
				{
					EnsureOpen();
					@out.Write(b);
					if ((b == '\n') && AutoFlush)
					{
						@out.Flush();
					}
				}
			}
			catch (InterruptedIOException)
			{
				Thread.CurrentThread.Interrupt();
			}
			catch (IOException)
			{
				Trouble = true;
			}
		}

		/// <summary>
		/// Writes <code>len</code> bytes from the specified byte array starting at
		/// offset <code>off</code> to this stream.  If automatic flushing is
		/// enabled then the <code>flush</code> method will be invoked.
		/// 
		/// <para> Note that the bytes will be written as given; to write characters
		/// that will be translated according to the platform's default character
		/// encoding, use the <code>print(char)</code> or <code>println(char)</code>
		/// methods.
		/// 
		/// </para>
		/// </summary>
		/// <param name="buf">   A byte array </param>
		/// <param name="off">   Offset from which to start taking bytes </param>
		/// <param name="len">   Number of bytes to write </param>
		public override void Write(sbyte[] buf, int off, int len)
		{
			try
			{
				lock (this)
				{
					EnsureOpen();
					@out.Write(buf, off, len);
					if (AutoFlush)
					{
						@out.Flush();
					}
				}
			}
			catch (InterruptedIOException)
			{
				Thread.CurrentThread.Interrupt();
			}
			catch (IOException)
			{
				Trouble = true;
			}
		}

		/*
		 * The following private methods on the text- and character-output streams
		 * always flush the stream buffers, so that writes to the underlying byte
		 * stream occur as promptly as with the original PrintStream.
		 */

		private void Write(char[] buf)
		{
			try
			{
				lock (this)
				{
					EnsureOpen();
					TextOut.Write(buf);
					TextOut.FlushBuffer();
					CharOut.FlushBuffer();
					if (AutoFlush)
					{
						for (int i = 0; i < buf.Length; i++)
						{
							if (buf[i] == '\n')
							{
								@out.Flush();
							}
						}
					}
				}
			}
			catch (InterruptedIOException)
			{
				Thread.CurrentThread.Interrupt();
			}
			catch (IOException)
			{
				Trouble = true;
			}
		}

		private void Write(String s)
		{
			try
			{
				lock (this)
				{
					EnsureOpen();
					TextOut.Write(s);
					TextOut.FlushBuffer();
					CharOut.FlushBuffer();
					if (AutoFlush && (s.IndexOf('\n') >= 0))
					{
						@out.Flush();
					}
				}
			}
			catch (InterruptedIOException)
			{
				Thread.CurrentThread.Interrupt();
			}
			catch (IOException)
			{
				Trouble = true;
			}
		}

		private void NewLine()
		{
			try
			{
				lock (this)
				{
					EnsureOpen();
					TextOut.NewLine();
					TextOut.FlushBuffer();
					CharOut.FlushBuffer();
					if (AutoFlush)
					{
						@out.Flush();
					}
				}
			}
			catch (InterruptedIOException)
			{
				Thread.CurrentThread.Interrupt();
			}
			catch (IOException)
			{
				Trouble = true;
			}
		}

		/* Methods that do not terminate lines */

		/// <summary>
		/// Prints a boolean value.  The string produced by <code>{@link
		/// java.lang.String#valueOf(boolean)}</code> is translated into bytes
		/// according to the platform's default character encoding, and these bytes
		/// are written in exactly the manner of the
		/// <code><seealso cref="#write(int)"/></code> method.
		/// </summary>
		/// <param name="b">   The <code>boolean</code> to be printed </param>
		public virtual void Print(bool b)
		{
			Write(b ? "true" : "false");
		}

		/// <summary>
		/// Prints a character.  The character is translated into one or more bytes
		/// according to the platform's default character encoding, and these bytes
		/// are written in exactly the manner of the
		/// <code><seealso cref="#write(int)"/></code> method.
		/// </summary>
		/// <param name="c">   The <code>char</code> to be printed </param>
		public virtual void Print(char c)
		{
			Write(Convert.ToString(c));
		}

		/// <summary>
		/// Prints an integer.  The string produced by <code>{@link
		/// java.lang.String#valueOf(int)}</code> is translated into bytes
		/// according to the platform's default character encoding, and these bytes
		/// are written in exactly the manner of the
		/// <code><seealso cref="#write(int)"/></code> method.
		/// </summary>
		/// <param name="i">   The <code>int</code> to be printed </param>
		/// <seealso cref=        java.lang.Integer#toString(int) </seealso>
		public virtual void Print(int i)
		{
			Write(Convert.ToString(i));
		}

		/// <summary>
		/// Prints a long integer.  The string produced by <code>{@link
		/// java.lang.String#valueOf(long)}</code> is translated into bytes
		/// according to the platform's default character encoding, and these bytes
		/// are written in exactly the manner of the
		/// <code><seealso cref="#write(int)"/></code> method.
		/// </summary>
		/// <param name="l">   The <code>long</code> to be printed </param>
		/// <seealso cref=        java.lang.Long#toString(long) </seealso>
		public virtual void Print(long l)
		{
			Write(Convert.ToString(l));
		}

		/// <summary>
		/// Prints a floating-point number.  The string produced by <code>{@link
		/// java.lang.String#valueOf(float)}</code> is translated into bytes
		/// according to the platform's default character encoding, and these bytes
		/// are written in exactly the manner of the
		/// <code><seealso cref="#write(int)"/></code> method.
		/// </summary>
		/// <param name="f">   The <code>float</code> to be printed </param>
		/// <seealso cref=        java.lang.Float#toString(float) </seealso>
		public virtual void Print(float f)
		{
			Write(Convert.ToString(f));
		}

		/// <summary>
		/// Prints a double-precision floating-point number.  The string produced by
		/// <code><seealso cref="java.lang.String#valueOf(double)"/></code> is translated into
		/// bytes according to the platform's default character encoding, and these
		/// bytes are written in exactly the manner of the <code>{@link
		/// #write(int)}</code> method.
		/// </summary>
		/// <param name="d">   The <code>double</code> to be printed </param>
		/// <seealso cref=        java.lang.Double#toString(double) </seealso>
		public virtual void Print(double d)
		{
			Write(Convert.ToString(d));
		}

		/// <summary>
		/// Prints an array of characters.  The characters are converted into bytes
		/// according to the platform's default character encoding, and these bytes
		/// are written in exactly the manner of the
		/// <code><seealso cref="#write(int)"/></code> method.
		/// </summary>
		/// <param name="s">   The array of chars to be printed
		/// </param>
		/// <exception cref="NullPointerException">  If <code>s</code> is <code>null</code> </exception>
		public virtual void Print(char[] s)
		{
			Write(s);
		}

		/// <summary>
		/// Prints a string.  If the argument is <code>null</code> then the string
		/// <code>"null"</code> is printed.  Otherwise, the string's characters are
		/// converted into bytes according to the platform's default character
		/// encoding, and these bytes are written in exactly the manner of the
		/// <code><seealso cref="#write(int)"/></code> method.
		/// </summary>
		/// <param name="s">   The <code>String</code> to be printed </param>
		public virtual void Print(String s)
		{
			if (s == null)
			{
				s = "null";
			}
			Write(s);
		}

		/// <summary>
		/// Prints an object.  The string produced by the <code>{@link
		/// java.lang.String#valueOf(Object)}</code> method is translated into bytes
		/// according to the platform's default character encoding, and these bytes
		/// are written in exactly the manner of the
		/// <code><seealso cref="#write(int)"/></code> method.
		/// </summary>
		/// <param name="obj">   The <code>Object</code> to be printed </param>
		/// <seealso cref=        java.lang.Object#toString() </seealso>
		public virtual void Print(Object obj)
		{
			Write(Convert.ToString(obj));
		}


		/* Methods that do terminate lines */

		/// <summary>
		/// Terminates the current line by writing the line separator string.  The
		/// line separator string is defined by the system property
		/// <code>line.separator</code>, and is not necessarily a single newline
		/// character (<code>'\n'</code>).
		/// </summary>
		public virtual void Println()
		{
			NewLine();
		}

		/// <summary>
		/// Prints a boolean and then terminate the line.  This method behaves as
		/// though it invokes <code><seealso cref="#print(boolean)"/></code> and then
		/// <code><seealso cref="#println()"/></code>.
		/// </summary>
		/// <param name="x">  The <code>boolean</code> to be printed </param>
		public virtual void Println(bool x)
		{
			lock (this)
			{
				Print(x);
				NewLine();
			}
		}

		/// <summary>
		/// Prints a character and then terminate the line.  This method behaves as
		/// though it invokes <code><seealso cref="#print(char)"/></code> and then
		/// <code><seealso cref="#println()"/></code>.
		/// </summary>
		/// <param name="x">  The <code>char</code> to be printed. </param>
		public virtual void Println(char x)
		{
			lock (this)
			{
				Print(x);
				NewLine();
			}
		}

		/// <summary>
		/// Prints an integer and then terminate the line.  This method behaves as
		/// though it invokes <code><seealso cref="#print(int)"/></code> and then
		/// <code><seealso cref="#println()"/></code>.
		/// </summary>
		/// <param name="x">  The <code>int</code> to be printed. </param>
		public virtual void Println(int x)
		{
			lock (this)
			{
				Print(x);
				NewLine();
			}
		}

		/// <summary>
		/// Prints a long and then terminate the line.  This method behaves as
		/// though it invokes <code><seealso cref="#print(long)"/></code> and then
		/// <code><seealso cref="#println()"/></code>.
		/// </summary>
		/// <param name="x">  a The <code>long</code> to be printed. </param>
		public virtual void Println(long x)
		{
			lock (this)
			{
				Print(x);
				NewLine();
			}
		}

		/// <summary>
		/// Prints a float and then terminate the line.  This method behaves as
		/// though it invokes <code><seealso cref="#print(float)"/></code> and then
		/// <code><seealso cref="#println()"/></code>.
		/// </summary>
		/// <param name="x">  The <code>float</code> to be printed. </param>
		public virtual void Println(float x)
		{
			lock (this)
			{
				Print(x);
				NewLine();
			}
		}

		/// <summary>
		/// Prints a double and then terminate the line.  This method behaves as
		/// though it invokes <code><seealso cref="#print(double)"/></code> and then
		/// <code><seealso cref="#println()"/></code>.
		/// </summary>
		/// <param name="x">  The <code>double</code> to be printed. </param>
		public virtual void Println(double x)
		{
			lock (this)
			{
				Print(x);
				NewLine();
			}
		}

		/// <summary>
		/// Prints an array of characters and then terminate the line.  This method
		/// behaves as though it invokes <code><seealso cref="#print(char[])"/></code> and
		/// then <code><seealso cref="#println()"/></code>.
		/// </summary>
		/// <param name="x">  an array of chars to print. </param>
		public virtual void Println(char[] x)
		{
			lock (this)
			{
				Print(x);
				NewLine();
			}
		}

		/// <summary>
		/// Prints a String and then terminate the line.  This method behaves as
		/// though it invokes <code><seealso cref="#print(String)"/></code> and then
		/// <code><seealso cref="#println()"/></code>.
		/// </summary>
		/// <param name="x">  The <code>String</code> to be printed. </param>
		public virtual void Println(String x)
		{
			lock (this)
			{
				Print(x);
				NewLine();
			}
		}

		/// <summary>
		/// Prints an Object and then terminate the line.  This method calls
		/// at first String.valueOf(x) to get the printed object's string value,
		/// then behaves as
		/// though it invokes <code><seealso cref="#print(String)"/></code> and then
		/// <code><seealso cref="#println()"/></code>.
		/// </summary>
		/// <param name="x">  The <code>Object</code> to be printed. </param>
		public virtual void Println(Object x)
		{
			String s = Convert.ToString(x);
			lock (this)
			{
				Print(s);
				NewLine();
			}
		}


		/// <summary>
		/// A convenience method to write a formatted string to this output stream
		/// using the specified format string and arguments.
		/// 
		/// <para> An invocation of this method of the form <tt>out.printf(format,
		/// args)</tt> behaves in exactly the same way as the invocation
		/// 
		/// <pre>
		///     out.format(format, args) </pre>
		/// 
		/// </para>
		/// </summary>
		/// <param name="format">
		///         A format string as described in <a
		///         href="../util/Formatter.html#syntax">Format string syntax</a>
		/// </param>
		/// <param name="args">
		///         Arguments referenced by the format specifiers in the format
		///         string.  If there are more arguments than format specifiers, the
		///         extra arguments are ignored.  The number of arguments is
		///         variable and may be zero.  The maximum number of arguments is
		///         limited by the maximum dimension of a Java array as defined by
		///         <cite>The Java&trade; Virtual Machine Specification</cite>.
		///         The behaviour on a
		///         <tt>null</tt> argument depends on the <a
		///         href="../util/Formatter.html#syntax">conversion</a>.
		/// </param>
		/// <exception cref="java.util.IllegalFormatException">
		///          If a format string contains an illegal syntax, a format
		///          specifier that is incompatible with the given arguments,
		///          insufficient arguments given the format string, or other
		///          illegal conditions.  For specification of all possible
		///          formatting errors, see the <a
		///          href="../util/Formatter.html#detail">Details</a> section of the
		///          formatter class specification.
		/// </exception>
		/// <exception cref="NullPointerException">
		///          If the <tt>format</tt> is <tt>null</tt>
		/// </exception>
		/// <returns>  This output stream
		/// 
		/// @since  1.5 </returns>
		public virtual PrintStream Printf(String format, params Object[] args)
		{
			return Format(format, args);
		}

		/// <summary>
		/// A convenience method to write a formatted string to this output stream
		/// using the specified format string and arguments.
		/// 
		/// <para> An invocation of this method of the form <tt>out.printf(l, format,
		/// args)</tt> behaves in exactly the same way as the invocation
		/// 
		/// <pre>
		///     out.format(l, format, args) </pre>
		/// 
		/// </para>
		/// </summary>
		/// <param name="l">
		///         The <seealso cref="java.util.Locale locale"/> to apply during
		///         formatting.  If <tt>l</tt> is <tt>null</tt> then no localization
		///         is applied.
		/// </param>
		/// <param name="format">
		///         A format string as described in <a
		///         href="../util/Formatter.html#syntax">Format string syntax</a>
		/// </param>
		/// <param name="args">
		///         Arguments referenced by the format specifiers in the format
		///         string.  If there are more arguments than format specifiers, the
		///         extra arguments are ignored.  The number of arguments is
		///         variable and may be zero.  The maximum number of arguments is
		///         limited by the maximum dimension of a Java array as defined by
		///         <cite>The Java&trade; Virtual Machine Specification</cite>.
		///         The behaviour on a
		///         <tt>null</tt> argument depends on the <a
		///         href="../util/Formatter.html#syntax">conversion</a>.
		/// </param>
		/// <exception cref="java.util.IllegalFormatException">
		///          If a format string contains an illegal syntax, a format
		///          specifier that is incompatible with the given arguments,
		///          insufficient arguments given the format string, or other
		///          illegal conditions.  For specification of all possible
		///          formatting errors, see the <a
		///          href="../util/Formatter.html#detail">Details</a> section of the
		///          formatter class specification.
		/// </exception>
		/// <exception cref="NullPointerException">
		///          If the <tt>format</tt> is <tt>null</tt>
		/// </exception>
		/// <returns>  This output stream
		/// 
		/// @since  1.5 </returns>
		public virtual PrintStream Printf(Locale l, String format, params Object[] args)
		{
			return Format(l, format, args);
		}

		/// <summary>
		/// Writes a formatted string to this output stream using the specified
		/// format string and arguments.
		/// 
		/// <para> The locale always used is the one returned by {@link
		/// java.util.Locale#getDefault() Locale.getDefault()}, regardless of any
		/// previous invocations of other formatting methods on this object.
		/// 
		/// </para>
		/// </summary>
		/// <param name="format">
		///         A format string as described in <a
		///         href="../util/Formatter.html#syntax">Format string syntax</a>
		/// </param>
		/// <param name="args">
		///         Arguments referenced by the format specifiers in the format
		///         string.  If there are more arguments than format specifiers, the
		///         extra arguments are ignored.  The number of arguments is
		///         variable and may be zero.  The maximum number of arguments is
		///         limited by the maximum dimension of a Java array as defined by
		///         <cite>The Java&trade; Virtual Machine Specification</cite>.
		///         The behaviour on a
		///         <tt>null</tt> argument depends on the <a
		///         href="../util/Formatter.html#syntax">conversion</a>.
		/// </param>
		/// <exception cref="java.util.IllegalFormatException">
		///          If a format string contains an illegal syntax, a format
		///          specifier that is incompatible with the given arguments,
		///          insufficient arguments given the format string, or other
		///          illegal conditions.  For specification of all possible
		///          formatting errors, see the <a
		///          href="../util/Formatter.html#detail">Details</a> section of the
		///          formatter class specification.
		/// </exception>
		/// <exception cref="NullPointerException">
		///          If the <tt>format</tt> is <tt>null</tt>
		/// </exception>
		/// <returns>  This output stream
		/// 
		/// @since  1.5 </returns>
		public virtual PrintStream Format(String format, params Object[] args)
		{
			try
			{
				lock (this)
				{
					EnsureOpen();
					if ((Formatter == null) || (Formatter.Locale() != Locale.Default))
					{
						Formatter = new Formatter((Appendable) this);
					}
					Formatter.Format(Locale.Default, format, args);
				}
			}
			catch (InterruptedIOException)
			{
				Thread.CurrentThread.Interrupt();
			}
			catch (IOException)
			{
				Trouble = true;
			}
			return this;
		}

		/// <summary>
		/// Writes a formatted string to this output stream using the specified
		/// format string and arguments.
		/// </summary>
		/// <param name="l">
		///         The <seealso cref="java.util.Locale locale"/> to apply during
		///         formatting.  If <tt>l</tt> is <tt>null</tt> then no localization
		///         is applied.
		/// </param>
		/// <param name="format">
		///         A format string as described in <a
		///         href="../util/Formatter.html#syntax">Format string syntax</a>
		/// </param>
		/// <param name="args">
		///         Arguments referenced by the format specifiers in the format
		///         string.  If there are more arguments than format specifiers, the
		///         extra arguments are ignored.  The number of arguments is
		///         variable and may be zero.  The maximum number of arguments is
		///         limited by the maximum dimension of a Java array as defined by
		///         <cite>The Java&trade; Virtual Machine Specification</cite>.
		///         The behaviour on a
		///         <tt>null</tt> argument depends on the <a
		///         href="../util/Formatter.html#syntax">conversion</a>.
		/// </param>
		/// <exception cref="java.util.IllegalFormatException">
		///          If a format string contains an illegal syntax, a format
		///          specifier that is incompatible with the given arguments,
		///          insufficient arguments given the format string, or other
		///          illegal conditions.  For specification of all possible
		///          formatting errors, see the <a
		///          href="../util/Formatter.html#detail">Details</a> section of the
		///          formatter class specification.
		/// </exception>
		/// <exception cref="NullPointerException">
		///          If the <tt>format</tt> is <tt>null</tt>
		/// </exception>
		/// <returns>  This output stream
		/// 
		/// @since  1.5 </returns>
		public virtual PrintStream Format(Locale l, String format, params Object[] args)
		{
			try
			{
				lock (this)
				{
					EnsureOpen();
					if ((Formatter == null) || (Formatter.Locale() != l))
					{
						Formatter = new Formatter(this, l);
					}
					Formatter.Format(l, format, args);
				}
			}
			catch (InterruptedIOException)
			{
				Thread.CurrentThread.Interrupt();
			}
			catch (IOException)
			{
				Trouble = true;
			}
			return this;
		}

		/// <summary>
		/// Appends the specified character sequence to this output stream.
		/// 
		/// <para> An invocation of this method of the form <tt>out.append(csq)</tt>
		/// behaves in exactly the same way as the invocation
		/// 
		/// <pre>
		///     out.print(csq.toString()) </pre>
		/// 
		/// </para>
		/// <para> Depending on the specification of <tt>toString</tt> for the
		/// character sequence <tt>csq</tt>, the entire sequence may not be
		/// appended.  For instance, invoking then <tt>toString</tt> method of a
		/// character buffer will return a subsequence whose content depends upon
		/// the buffer's position and limit.
		/// 
		/// </para>
		/// </summary>
		/// <param name="csq">
		///         The character sequence to append.  If <tt>csq</tt> is
		///         <tt>null</tt>, then the four characters <tt>"null"</tt> are
		///         appended to this output stream.
		/// </param>
		/// <returns>  This output stream
		/// 
		/// @since  1.5 </returns>
		public virtual PrintStream Append(CharSequence csq)
		{
			if (csq == null)
			{
				Print("null");
			}
			else
			{
				Print(csq.ToString());
			}
			return this;
		}

		/// <summary>
		/// Appends a subsequence of the specified character sequence to this output
		/// stream.
		/// 
		/// <para> An invocation of this method of the form <tt>out.append(csq, start,
		/// end)</tt> when <tt>csq</tt> is not <tt>null</tt>, behaves in
		/// exactly the same way as the invocation
		/// 
		/// <pre>
		///     out.print(csq.subSequence(start, end).toString()) </pre>
		/// 
		/// </para>
		/// </summary>
		/// <param name="csq">
		///         The character sequence from which a subsequence will be
		///         appended.  If <tt>csq</tt> is <tt>null</tt>, then characters
		///         will be appended as if <tt>csq</tt> contained the four
		///         characters <tt>"null"</tt>.
		/// </param>
		/// <param name="start">
		///         The index of the first character in the subsequence
		/// </param>
		/// <param name="end">
		///         The index of the character following the last character in the
		///         subsequence
		/// </param>
		/// <returns>  This output stream
		/// </returns>
		/// <exception cref="IndexOutOfBoundsException">
		///          If <tt>start</tt> or <tt>end</tt> are negative, <tt>start</tt>
		///          is greater than <tt>end</tt>, or <tt>end</tt> is greater than
		///          <tt>csq.length()</tt>
		/// 
		/// @since  1.5 </exception>
		public virtual PrintStream Append(CharSequence csq, int start, int end)
		{
			CharSequence cs = (csq == null ? "null" : csq);
			Write(cs.SubSequence(start, end).ToString());
			return this;
		}

		/// <summary>
		/// Appends the specified character to this output stream.
		/// 
		/// <para> An invocation of this method of the form <tt>out.append(c)</tt>
		/// behaves in exactly the same way as the invocation
		/// 
		/// <pre>
		///     out.print(c) </pre>
		/// 
		/// </para>
		/// </summary>
		/// <param name="c">
		///         The 16-bit character to append
		/// </param>
		/// <returns>  This output stream
		/// 
		/// @since  1.5 </returns>
		public virtual PrintStream Append(char c)
		{
			Print(c);
			return this;
		}

	}

}
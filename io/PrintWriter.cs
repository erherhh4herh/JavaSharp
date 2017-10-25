using System;
using System.Threading;

/*
 * Copyright (c) 1996, 2012, Oracle and/or its affiliates. All rights reserved.
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
	/// Prints formatted representations of objects to a text-output stream.  This
	/// class implements all of the <tt>print</tt> methods found in {@link
	/// PrintStream}.  It does not contain methods for writing raw bytes, for which
	/// a program should use unencoded byte streams.
	/// 
	/// <para> Unlike the <seealso cref="PrintStream"/> class, if automatic flushing is enabled
	/// it will be done only when one of the <tt>println</tt>, <tt>printf</tt>, or
	/// <tt>format</tt> methods is invoked, rather than whenever a newline character
	/// happens to be output.  These methods use the platform's own notion of line
	/// separator rather than the newline character.
	/// 
	/// </para>
	/// <para> Methods in this class never throw I/O exceptions, although some of its
	/// constructors may.  The client may inquire as to whether any errors have
	/// occurred by invoking <seealso cref="#checkError checkError()"/>.
	/// 
	/// @author      Frank Yellin
	/// @author      Mark Reinhold
	/// @since       JDK1.1
	/// </para>
	/// </summary>

	public class PrintWriter : Writer
	{

		/// <summary>
		/// The underlying character-output stream of this
		/// <code>PrintWriter</code>.
		/// 
		/// @since 1.2
		/// </summary>
		protected internal Writer @out;

		private readonly bool AutoFlush;
		private bool Trouble = false;
		private Formatter Formatter;
		private PrintStream PsOut = null;

		/// <summary>
		/// Line separator string.  This is the value of the line.separator
		/// property at the moment that the stream was created.
		/// </summary>
		private readonly String LineSeparator;

		/// <summary>
		/// Returns a charset object for the given charset name. </summary>
		/// <exception cref="NullPointerException">          is csn is null </exception>
		/// <exception cref="UnsupportedEncodingException">  if the charset is not supported </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: private static java.nio.charset.Charset toCharset(String csn) throws UnsupportedEncodingException
		private static Charset ToCharset(String csn)
		{
			Objects.RequireNonNull(csn, "charsetName");
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

		/// <summary>
		/// Creates a new PrintWriter, without automatic line flushing.
		/// </summary>
		/// <param name="out">        A character-output stream </param>
		public PrintWriter(Writer @out) : this(@out, false)
		{
		}

		/// <summary>
		/// Creates a new PrintWriter.
		/// </summary>
		/// <param name="out">        A character-output stream </param>
		/// <param name="autoFlush">  A boolean; if true, the <tt>println</tt>,
		///                    <tt>printf</tt>, or <tt>format</tt> methods will
		///                    flush the output buffer </param>
		public PrintWriter(Writer @out, bool autoFlush) : base(@out)
		{
			this.@out = @out;
			this.AutoFlush = autoFlush;
			LineSeparator = java.security.AccessController.doPrivileged(new sun.security.action.GetPropertyAction("line.separator"));
		}

		/// <summary>
		/// Creates a new PrintWriter, without automatic line flushing, from an
		/// existing OutputStream.  This convenience constructor creates the
		/// necessary intermediate OutputStreamWriter, which will convert characters
		/// into bytes using the default character encoding.
		/// </summary>
		/// <param name="out">        An output stream
		/// </param>
		/// <seealso cref= java.io.OutputStreamWriter#OutputStreamWriter(java.io.OutputStream) </seealso>
		public PrintWriter(OutputStream @out) : this(@out, false)
		{
		}

		/// <summary>
		/// Creates a new PrintWriter from an existing OutputStream.  This
		/// convenience constructor creates the necessary intermediate
		/// OutputStreamWriter, which will convert characters into bytes using the
		/// default character encoding.
		/// </summary>
		/// <param name="out">        An output stream </param>
		/// <param name="autoFlush">  A boolean; if true, the <tt>println</tt>,
		///                    <tt>printf</tt>, or <tt>format</tt> methods will
		///                    flush the output buffer
		/// </param>
		/// <seealso cref= java.io.OutputStreamWriter#OutputStreamWriter(java.io.OutputStream) </seealso>
		public PrintWriter(OutputStream @out, bool autoFlush) : this(new BufferedWriter(new OutputStreamWriter(@out)), autoFlush)
		{

			// save print stream for error propagation
			if (@out is java.io.PrintStream)
			{
				PsOut = (PrintStream) @out;
			}
		}

		/// <summary>
		/// Creates a new PrintWriter, without automatic line flushing, with the
		/// specified file name.  This convenience constructor creates the necessary
		/// intermediate <seealso cref="java.io.OutputStreamWriter OutputStreamWriter"/>,
		/// which will encode characters using the {@linkplain
		/// java.nio.charset.Charset#defaultCharset() default charset} for this
		/// instance of the Java virtual machine.
		/// </summary>
		/// <param name="fileName">
		///         The name of the file to use as the destination of this writer.
		///         If the file exists then it will be truncated to zero size;
		///         otherwise, a new file will be created.  The output will be
		///         written to the file and is buffered.
		/// </param>
		/// <exception cref="FileNotFoundException">
		///          If the given string does not denote an existing, writable
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
//ORIGINAL LINE: public PrintWriter(String fileName) throws FileNotFoundException
		public PrintWriter(String fileName) : this(new BufferedWriter(new OutputStreamWriter(new FileOutputStream(fileName))), false)
		{
		}

		/* Private constructor */
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: private PrintWriter(java.nio.charset.Charset charset, File file) throws FileNotFoundException
		private PrintWriter(Charset charset, File file) : this(new BufferedWriter(new OutputStreamWriter(new FileOutputStream(file), charset)), false)
		{
		}

		/// <summary>
		/// Creates a new PrintWriter, without automatic line flushing, with the
		/// specified file name and charset.  This convenience constructor creates
		/// the necessary intermediate {@link java.io.OutputStreamWriter
		/// OutputStreamWriter}, which will encode characters using the provided
		/// charset.
		/// </summary>
		/// <param name="fileName">
		///         The name of the file to use as the destination of this writer.
		///         If the file exists then it will be truncated to zero size;
		///         otherwise, a new file will be created.  The output will be
		///         written to the file and is buffered.
		/// </param>
		/// <param name="csn">
		///         The name of a supported {@link java.nio.charset.Charset
		///         charset}
		/// </param>
		/// <exception cref="FileNotFoundException">
		///          If the given string does not denote an existing, writable
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
//ORIGINAL LINE: public PrintWriter(String fileName, String csn) throws FileNotFoundException, UnsupportedEncodingException
		public PrintWriter(String fileName, String csn) : this(ToCharset(csn), new File(fileName))
		{
		}

		/// <summary>
		/// Creates a new PrintWriter, without automatic line flushing, with the
		/// specified file.  This convenience constructor creates the necessary
		/// intermediate <seealso cref="java.io.OutputStreamWriter OutputStreamWriter"/>,
		/// which will encode characters using the {@linkplain
		/// java.nio.charset.Charset#defaultCharset() default charset} for this
		/// instance of the Java virtual machine.
		/// </summary>
		/// <param name="file">
		///         The file to use as the destination of this writer.  If the file
		///         exists then it will be truncated to zero size; otherwise, a new
		///         file will be created.  The output will be written to the file
		///         and is buffered.
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
//ORIGINAL LINE: public PrintWriter(File file) throws FileNotFoundException
		public PrintWriter(File file) : this(new BufferedWriter(new OutputStreamWriter(new FileOutputStream(file))), false)
		{
		}

		/// <summary>
		/// Creates a new PrintWriter, without automatic line flushing, with the
		/// specified file and charset.  This convenience constructor creates the
		/// necessary intermediate {@link java.io.OutputStreamWriter
		/// OutputStreamWriter}, which will encode characters using the provided
		/// charset.
		/// </summary>
		/// <param name="file">
		///         The file to use as the destination of this writer.  If the file
		///         exists then it will be truncated to zero size; otherwise, a new
		///         file will be created.  The output will be written to the file
		///         and is buffered.
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
//ORIGINAL LINE: public PrintWriter(File file, String csn) throws FileNotFoundException, UnsupportedEncodingException
		public PrintWriter(File file, String csn) : this(ToCharset(csn), file)
		{
		}

		/// <summary>
		/// Checks to make sure that the stream has not been closed </summary>
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
		/// Flushes the stream. </summary>
		/// <seealso cref= #checkError() </seealso>
		public override void Flush()
		{
			try
			{
				lock (@lock)
				{
					EnsureOpen();
					@out.Flush();
				}
			}
			catch (IOException)
			{
				Trouble = true;
			}
		}

		/// <summary>
		/// Closes the stream and releases any system resources associated
		/// with it. Closing a previously closed stream has no effect.
		/// </summary>
		/// <seealso cref= #checkError() </seealso>
		public override void Close()
		{
			try
			{
				lock (@lock)
				{
					if (@out == null)
					{
						return;
					}
					@out.Close();
					@out = null;
				}
			}
			catch (IOException)
			{
				Trouble = true;
			}
		}

		/// <summary>
		/// Flushes the stream if it's not closed and checks its error state.
		/// </summary>
		/// <returns> <code>true</code> if the print stream has encountered an error,
		///          either on the underlying output stream or during a format
		///          conversion. </returns>
		public virtual bool CheckError()
		{
			if (@out != null)
			{
				Flush();
			}
			if (@out is java.io.PrintWriter)
			{
				PrintWriter pw = (PrintWriter) @out;
				return pw.CheckError();
			}
			else if (PsOut != null)
			{
				return PsOut.CheckError();
			}
			return Trouble;
		}

		/// <summary>
		/// Indicates that an error has occurred.
		/// 
		/// <para> This method will cause subsequent invocations of {@link
		/// #checkError()} to return <tt>true</tt> until {@link
		/// #clearError()} is invoked.
		/// </para>
		/// </summary>
		protected internal virtual void SetError()
		{
			Trouble = true;
		}

		/// <summary>
		/// Clears the error state of this stream.
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
		 * which also implement the write() methods of Writer
		 */

		/// <summary>
		/// Writes a single character. </summary>
		/// <param name="c"> int specifying a character to be written. </param>
		public override void Write(int c)
		{
			try
			{
				lock (@lock)
				{
					EnsureOpen();
					@out.Write(c);
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
		/// Writes A Portion of an array of characters. </summary>
		/// <param name="buf"> Array of characters </param>
		/// <param name="off"> Offset from which to start writing characters </param>
		/// <param name="len"> Number of characters to write </param>
		public override void Write(char[] buf, int off, int len)
		{
			try
			{
				lock (@lock)
				{
					EnsureOpen();
					@out.Write(buf, off, len);
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
		/// Writes an array of characters.  This method cannot be inherited from the
		/// Writer class because it must suppress I/O exceptions. </summary>
		/// <param name="buf"> Array of characters to be written </param>
		public override void Write(char[] buf)
		{
			Write(buf, 0, buf.Length);
		}

		/// <summary>
		/// Writes a portion of a string. </summary>
		/// <param name="s"> A String </param>
		/// <param name="off"> Offset from which to start writing characters </param>
		/// <param name="len"> Number of characters to write </param>
		public override void Write(String s, int off, int len)
		{
			try
			{
				lock (@lock)
				{
					EnsureOpen();
					@out.Write(s, off, len);
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
		/// Writes a string.  This method cannot be inherited from the Writer class
		/// because it must suppress I/O exceptions. </summary>
		/// <param name="s"> String to be written </param>
		public override void Write(String s)
		{
			Write(s, 0, s.Length());
		}

		private void NewLine()
		{
			try
			{
				lock (@lock)
				{
					EnsureOpen();
					@out.Write(LineSeparator);
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
		/// are written in exactly the manner of the <code>{@link
		/// #write(int)}</code> method.
		/// </summary>
		/// <param name="b">   The <code>boolean</code> to be printed </param>
		public virtual void Print(bool b)
		{
			Write(b ? "true" : "false");
		}

		/// <summary>
		/// Prints a character.  The character is translated into one or more bytes
		/// according to the platform's default character encoding, and these bytes
		/// are written in exactly the manner of the <code>{@link
		/// #write(int)}</code> method.
		/// </summary>
		/// <param name="c">   The <code>char</code> to be printed </param>
		public virtual void Print(char c)
		{
			Write(c);
		}

		/// <summary>
		/// Prints an integer.  The string produced by <code>{@link
		/// java.lang.String#valueOf(int)}</code> is translated into bytes according
		/// to the platform's default character encoding, and these bytes are
		/// written in exactly the manner of the <code><seealso cref="#write(int)"/></code>
		/// method.
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
		/// are written in exactly the manner of the <code><seealso cref="#write(int)"/></code>
		/// method.
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
		/// are written in exactly the manner of the <code><seealso cref="#write(int)"/></code>
		/// method.
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
		/// are written in exactly the manner of the <code><seealso cref="#write(int)"/></code>
		/// method.
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
		/// are written in exactly the manner of the <code><seealso cref="#write(int)"/></code>
		/// method.
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
		/// Prints a boolean value and then terminates the line.  This method behaves
		/// as though it invokes <code><seealso cref="#print(boolean)"/></code> and then
		/// <code><seealso cref="#println()"/></code>.
		/// </summary>
		/// <param name="x"> the <code>boolean</code> value to be printed </param>
		public virtual void Println(bool x)
		{
			lock (@lock)
			{
				Print(x);
				Println();
			}
		}

		/// <summary>
		/// Prints a character and then terminates the line.  This method behaves as
		/// though it invokes <code><seealso cref="#print(char)"/></code> and then <code>{@link
		/// #println()}</code>.
		/// </summary>
		/// <param name="x"> the <code>char</code> value to be printed </param>
		public virtual void Println(char x)
		{
			lock (@lock)
			{
				Print(x);
				Println();
			}
		}

		/// <summary>
		/// Prints an integer and then terminates the line.  This method behaves as
		/// though it invokes <code><seealso cref="#print(int)"/></code> and then <code>{@link
		/// #println()}</code>.
		/// </summary>
		/// <param name="x"> the <code>int</code> value to be printed </param>
		public virtual void Println(int x)
		{
			lock (@lock)
			{
				Print(x);
				Println();
			}
		}

		/// <summary>
		/// Prints a long integer and then terminates the line.  This method behaves
		/// as though it invokes <code><seealso cref="#print(long)"/></code> and then
		/// <code><seealso cref="#println()"/></code>.
		/// </summary>
		/// <param name="x"> the <code>long</code> value to be printed </param>
		public virtual void Println(long x)
		{
			lock (@lock)
			{
				Print(x);
				Println();
			}
		}

		/// <summary>
		/// Prints a floating-point number and then terminates the line.  This method
		/// behaves as though it invokes <code><seealso cref="#print(float)"/></code> and then
		/// <code><seealso cref="#println()"/></code>.
		/// </summary>
		/// <param name="x"> the <code>float</code> value to be printed </param>
		public virtual void Println(float x)
		{
			lock (@lock)
			{
				Print(x);
				Println();
			}
		}

		/// <summary>
		/// Prints a double-precision floating-point number and then terminates the
		/// line.  This method behaves as though it invokes <code>{@link
		/// #print(double)}</code> and then <code><seealso cref="#println()"/></code>.
		/// </summary>
		/// <param name="x"> the <code>double</code> value to be printed </param>
		public virtual void Println(double x)
		{
			lock (@lock)
			{
				Print(x);
				Println();
			}
		}

		/// <summary>
		/// Prints an array of characters and then terminates the line.  This method
		/// behaves as though it invokes <code><seealso cref="#print(char[])"/></code> and then
		/// <code><seealso cref="#println()"/></code>.
		/// </summary>
		/// <param name="x"> the array of <code>char</code> values to be printed </param>
		public virtual void Println(char[] x)
		{
			lock (@lock)
			{
				Print(x);
				Println();
			}
		}

		/// <summary>
		/// Prints a String and then terminates the line.  This method behaves as
		/// though it invokes <code><seealso cref="#print(String)"/></code> and then
		/// <code><seealso cref="#println()"/></code>.
		/// </summary>
		/// <param name="x"> the <code>String</code> value to be printed </param>
		public virtual void Println(String x)
		{
			lock (@lock)
			{
				Print(x);
				Println();
			}
		}

		/// <summary>
		/// Prints an Object and then terminates the line.  This method calls
		/// at first String.valueOf(x) to get the printed object's string value,
		/// then behaves as
		/// though it invokes <code><seealso cref="#print(String)"/></code> and then
		/// <code><seealso cref="#println()"/></code>.
		/// </summary>
		/// <param name="x">  The <code>Object</code> to be printed. </param>
		public virtual void Println(Object x)
		{
			String s = Convert.ToString(x);
			lock (@lock)
			{
				Print(s);
				Println();
			}
		}

		/// <summary>
		/// A convenience method to write a formatted string to this writer using
		/// the specified format string and arguments.  If automatic flushing is
		/// enabled, calls to this method will flush the output buffer.
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
		///         href="../util/Formatter.html#syntax">Format string syntax</a>.
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
		/// <returns>  This writer
		/// 
		/// @since  1.5 </returns>
		public virtual PrintWriter Printf(String format, params Object[] args)
		{
			return Format(format, args);
		}

		/// <summary>
		/// A convenience method to write a formatted string to this writer using
		/// the specified format string and arguments.  If automatic flushing is
		/// enabled, calls to this method will flush the output buffer.
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
		///         href="../util/Formatter.html#syntax">Format string syntax</a>.
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
		/// <returns>  This writer
		/// 
		/// @since  1.5 </returns>
		public virtual PrintWriter Printf(Locale l, String format, params Object[] args)
		{
			return Format(l, format, args);
		}

		/// <summary>
		/// Writes a formatted string to this writer using the specified format
		/// string and arguments.  If automatic flushing is enabled, calls to this
		/// method will flush the output buffer.
		/// 
		/// <para> The locale always used is the one returned by {@link
		/// java.util.Locale#getDefault() Locale.getDefault()}, regardless of any
		/// previous invocations of other formatting methods on this object.
		/// 
		/// </para>
		/// </summary>
		/// <param name="format">
		///         A format string as described in <a
		///         href="../util/Formatter.html#syntax">Format string syntax</a>.
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
		///          Formatter class specification.
		/// </exception>
		/// <exception cref="NullPointerException">
		///          If the <tt>format</tt> is <tt>null</tt>
		/// </exception>
		/// <returns>  This writer
		/// 
		/// @since  1.5 </returns>
		public virtual PrintWriter Format(String format, params Object[] args)
		{
			try
			{
				lock (@lock)
				{
					EnsureOpen();
					if ((Formatter == null) || (Formatter.Locale() != Locale.Default))
					{
						Formatter = new Formatter(this);
					}
					Formatter.Format(Locale.Default, format, args);
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
			return this;
		}

		/// <summary>
		/// Writes a formatted string to this writer using the specified format
		/// string and arguments.  If automatic flushing is enabled, calls to this
		/// method will flush the output buffer.
		/// </summary>
		/// <param name="l">
		///         The <seealso cref="java.util.Locale locale"/> to apply during
		///         formatting.  If <tt>l</tt> is <tt>null</tt> then no localization
		///         is applied.
		/// </param>
		/// <param name="format">
		///         A format string as described in <a
		///         href="../util/Formatter.html#syntax">Format string syntax</a>.
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
		/// <returns>  This writer
		/// 
		/// @since  1.5 </returns>
		public virtual PrintWriter Format(Locale l, String format, params Object[] args)
		{
			try
			{
				lock (@lock)
				{
					EnsureOpen();
					if ((Formatter == null) || (Formatter.Locale() != l))
					{
						Formatter = new Formatter(this, l);
					}
					Formatter.Format(l, format, args);
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
			return this;
		}

		/// <summary>
		/// Appends the specified character sequence to this writer.
		/// 
		/// <para> An invocation of this method of the form <tt>out.append(csq)</tt>
		/// behaves in exactly the same way as the invocation
		/// 
		/// <pre>
		///     out.write(csq.toString()) </pre>
		/// 
		/// </para>
		/// <para> Depending on the specification of <tt>toString</tt> for the
		/// character sequence <tt>csq</tt>, the entire sequence may not be
		/// appended. For instance, invoking the <tt>toString</tt> method of a
		/// character buffer will return a subsequence whose content depends upon
		/// the buffer's position and limit.
		/// 
		/// </para>
		/// </summary>
		/// <param name="csq">
		///         The character sequence to append.  If <tt>csq</tt> is
		///         <tt>null</tt>, then the four characters <tt>"null"</tt> are
		///         appended to this writer.
		/// </param>
		/// <returns>  This writer
		/// 
		/// @since  1.5 </returns>
		public override PrintWriter Append(CharSequence csq)
		{
			if (csq == null)
			{
				Write("null");
			}
			else
			{
				Write(csq.ToString());
			}
			return this;
		}

		/// <summary>
		/// Appends a subsequence of the specified character sequence to this writer.
		/// 
		/// <para> An invocation of this method of the form <tt>out.append(csq, start,
		/// end)</tt> when <tt>csq</tt> is not <tt>null</tt>, behaves in
		/// exactly the same way as the invocation
		/// 
		/// <pre>
		///     out.write(csq.subSequence(start, end).toString()) </pre>
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
		/// <returns>  This writer
		/// </returns>
		/// <exception cref="IndexOutOfBoundsException">
		///          If <tt>start</tt> or <tt>end</tt> are negative, <tt>start</tt>
		///          is greater than <tt>end</tt>, or <tt>end</tt> is greater than
		///          <tt>csq.length()</tt>
		/// 
		/// @since  1.5 </exception>
		public override PrintWriter Append(CharSequence csq, int start, int end)
		{
			CharSequence cs = (csq == null ? "null" : csq);
			Write(cs.SubSequence(start, end).ToString());
			return this;
		}

		/// <summary>
		/// Appends the specified character to this writer.
		/// 
		/// <para> An invocation of this method of the form <tt>out.append(c)</tt>
		/// behaves in exactly the same way as the invocation
		/// 
		/// <pre>
		///     out.write(c) </pre>
		/// 
		/// </para>
		/// </summary>
		/// <param name="c">
		///         The 16-bit character to append
		/// </param>
		/// <returns>  This writer
		/// 
		/// @since 1.5 </returns>
		public override PrintWriter Append(char c)
		{
			Write(c);
			return this;
		}
	}

}
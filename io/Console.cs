using System;
using System.Diagnostics;
using System.Runtime.InteropServices;

/*
 * Copyright (c) 2005, 2013, Oracle and/or its affiliates. All rights reserved.
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

	using StreamDecoder = sun.nio.cs.StreamDecoder;
	using StreamEncoder = sun.nio.cs.StreamEncoder;

	/// <summary>
	/// Methods to access the character-based console device, if any, associated
	/// with the current Java virtual machine.
	/// 
	/// <para> Whether a virtual machine has a console is dependent upon the
	/// underlying platform and also upon the manner in which the virtual
	/// machine is invoked.  If the virtual machine is started from an
	/// interactive command line without redirecting the standard input and
	/// output streams then its console will exist and will typically be
	/// connected to the keyboard and display from which the virtual machine
	/// was launched.  If the virtual machine is started automatically, for
	/// example by a background job scheduler, then it will typically not
	/// have a console.
	/// </para>
	/// <para>
	/// If this virtual machine has a console then it is represented by a
	/// unique instance of this class which can be obtained by invoking the
	/// <seealso cref="java.lang.System#console()"/> method.  If no console device is
	/// available then an invocation of that method will return <tt>null</tt>.
	/// </para>
	/// <para>
	/// Read and write operations are synchronized to guarantee the atomic
	/// completion of critical operations; therefore invoking methods
	/// <seealso cref="#readLine()"/>, <seealso cref="#readPassword()"/>, <seealso cref="#format format()"/>,
	/// <seealso cref="#printf printf()"/> as well as the read, format and write operations
	/// on the objects returned by <seealso cref="#reader()"/> and <seealso cref="#writer()"/> may
	/// block in multithreaded scenarios.
	/// </para>
	/// <para>
	/// Invoking <tt>close()</tt> on the objects returned by the <seealso cref="#reader()"/>
	/// and the <seealso cref="#writer()"/> will not close the underlying stream of those
	/// objects.
	/// </para>
	/// <para>
	/// The console-read methods return <tt>null</tt> when the end of the
	/// console input stream is reached, for example by typing control-D on
	/// Unix or control-Z on Windows.  Subsequent read operations will succeed
	/// if additional characters are later entered on the console's input
	/// device.
	/// </para>
	/// <para>
	/// Unless otherwise specified, passing a <tt>null</tt> argument to any method
	/// in this class will cause a <seealso cref="NullPointerException"/> to be thrown.
	/// </para>
	/// <para>
	/// <b>Security note:</b>
	/// If an application needs to read a password or other secure data, it should
	/// use <seealso cref="#readPassword()"/> or <seealso cref="#readPassword(String, Object...)"/> and
	/// manually zero the returned character array after processing to minimize the
	/// lifetime of sensitive data in memory.
	/// 
	/// <blockquote><pre>{@code
	/// Console cons;
	/// char[] passwd;
	/// if ((cons = System.console()) != null &&
	///     (passwd = cons.readPassword("[%s]", "Password:")) != null) {
	///     ...
	///     java.util.Arrays.fill(passwd, ' ');
	/// }
	/// }</pre></blockquote>
	/// 
	/// @author  Xueming Shen
	/// @since   1.6
	/// </para>
	/// </summary>

	public sealed class Console : Flushable
	{
	   /// <summary>
	   /// Retrieves the unique <seealso cref="java.io.PrintWriter PrintWriter"/> object
	   /// associated with this console.
	   /// </summary>
	   /// <returns>  The printwriter associated with this console </returns>
		public PrintWriter Writer()
		{
			return Pw;
		}

	   /// <summary>
	   /// Retrieves the unique <seealso cref="java.io.Reader Reader"/> object associated
	   /// with this console.
	   /// <para>
	   /// This method is intended to be used by sophisticated applications, for
	   /// example, a <seealso cref="java.util.Scanner"/> object which utilizes the rich
	   /// parsing/scanning functionality provided by the <tt>Scanner</tt>:
	   /// <blockquote><pre>
	   /// Console con = System.console();
	   /// if (con != null) {
	   ///     Scanner sc = new Scanner(con.reader());
	   ///     ...
	   /// }
	   /// </pre></blockquote>
	   /// </para>
	   /// <para>
	   /// For simple applications requiring only line-oriented reading, use
	   /// <tt><seealso cref="#readLine"/></tt>.
	   /// </para>
	   /// <para>
	   /// The bulk read operations <seealso cref="java.io.Reader#read(char[]) read(char[]) "/>,
	   /// <seealso cref="java.io.Reader#read(char[], int, int) read(char[], int, int) "/> and
	   /// <seealso cref="java.io.Reader#read(java.nio.CharBuffer) read(java.nio.CharBuffer)"/>
	   /// on the returned object will not read in characters beyond the line
	   /// bound for each invocation, even if the destination buffer has space for
	   /// more characters. The {@code Reader}'s {@code read} methods may block if a
	   /// line bound has not been entered or reached on the console's input device.
	   /// A line bound is considered to be any one of a line feed (<tt>'\n'</tt>),
	   /// a carriage return (<tt>'\r'</tt>), a carriage return followed immediately
	   /// by a linefeed, or an end of stream.
	   /// 
	   /// </para>
	   /// </summary>
	   /// <returns>  The reader associated with this console </returns>
		public Reader Reader()
		{
			return Reader_Renamed;
		}

	   /// <summary>
	   /// Writes a formatted string to this console's output stream using
	   /// the specified format string and arguments.
	   /// </summary>
	   /// <param name="fmt">
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
	   /// <exception cref="IllegalFormatException">
	   ///          If a format string contains an illegal syntax, a format
	   ///          specifier that is incompatible with the given arguments,
	   ///          insufficient arguments given the format string, or other
	   ///          illegal conditions.  For specification of all possible
	   ///          formatting errors, see the <a
	   ///          href="../util/Formatter.html#detail">Details</a> section
	   ///          of the formatter class specification.
	   /// </exception>
	   /// <returns>  This console </returns>
		public Console Format(String fmt, params Object[] args)
		{
			Formatter.Format(fmt, args).Flush();
			return this;
		}

	   /// <summary>
	   /// A convenience method to write a formatted string to this console's
	   /// output stream using the specified format string and arguments.
	   /// 
	   /// <para> An invocation of this method of the form <tt>con.printf(format,
	   /// args)</tt> behaves in exactly the same way as the invocation of
	   /// <pre>con.format(format, args)</pre>.
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
	   /// <exception cref="IllegalFormatException">
	   ///          If a format string contains an illegal syntax, a format
	   ///          specifier that is incompatible with the given arguments,
	   ///          insufficient arguments given the format string, or other
	   ///          illegal conditions.  For specification of all possible
	   ///          formatting errors, see the <a
	   ///          href="../util/Formatter.html#detail">Details</a> section of the
	   ///          formatter class specification.
	   /// </exception>
	   /// <returns>  This console </returns>
		public Console Printf(String format, params Object[] args)
		{
			return Format(format, args);
		}

	   /// <summary>
	   /// Provides a formatted prompt, then reads a single line of text from the
	   /// console.
	   /// </summary>
	   /// <param name="fmt">
	   ///         A format string as described in <a
	   ///         href="../util/Formatter.html#syntax">Format string syntax</a>.
	   /// </param>
	   /// <param name="args">
	   ///         Arguments referenced by the format specifiers in the format
	   ///         string.  If there are more arguments than format specifiers, the
	   ///         extra arguments are ignored.  The maximum number of arguments is
	   ///         limited by the maximum dimension of a Java array as defined by
	   ///         <cite>The Java&trade; Virtual Machine Specification</cite>.
	   /// </param>
	   /// <exception cref="IllegalFormatException">
	   ///          If a format string contains an illegal syntax, a format
	   ///          specifier that is incompatible with the given arguments,
	   ///          insufficient arguments given the format string, or other
	   ///          illegal conditions.  For specification of all possible
	   ///          formatting errors, see the <a
	   ///          href="../util/Formatter.html#detail">Details</a> section
	   ///          of the formatter class specification.
	   /// </exception>
	   /// <exception cref="IOError">
	   ///         If an I/O error occurs.
	   /// </exception>
	   /// <returns>  A string containing the line read from the console, not
	   ///          including any line-termination characters, or <tt>null</tt>
	   ///          if an end of stream has been reached. </returns>
		public String ReadLine(String fmt, params Object[] args)
		{
			String line = null;
			lock (WriteLock)
			{
				lock (ReadLock)
				{
					if (fmt.Length() != 0)
					{
						Pw.Format(fmt, args);
					}
					try
					{
						char[] ca = Readline(false);
						if (ca != null)
						{
							line = new String(ca);
						}
					}
					catch (IOException x)
					{
						throw new IOError(x);
					}
				}
			}
			return line;
		}

	   /// <summary>
	   /// Reads a single line of text from the console.
	   /// </summary>
	   /// <exception cref="IOError">
	   ///         If an I/O error occurs.
	   /// </exception>
	   /// <returns>  A string containing the line read from the console, not
	   ///          including any line-termination characters, or <tt>null</tt>
	   ///          if an end of stream has been reached. </returns>
		public String ReadLine()
		{
			return readLine("");
		}

	   /// <summary>
	   /// Provides a formatted prompt, then reads a password or passphrase from
	   /// the console with echoing disabled.
	   /// </summary>
	   /// <param name="fmt">
	   ///         A format string as described in <a
	   ///         href="../util/Formatter.html#syntax">Format string syntax</a>
	   ///         for the prompt text.
	   /// </param>
	   /// <param name="args">
	   ///         Arguments referenced by the format specifiers in the format
	   ///         string.  If there are more arguments than format specifiers, the
	   ///         extra arguments are ignored.  The maximum number of arguments is
	   ///         limited by the maximum dimension of a Java array as defined by
	   ///         <cite>The Java&trade; Virtual Machine Specification</cite>.
	   /// </param>
	   /// <exception cref="IllegalFormatException">
	   ///          If a format string contains an illegal syntax, a format
	   ///          specifier that is incompatible with the given arguments,
	   ///          insufficient arguments given the format string, or other
	   ///          illegal conditions.  For specification of all possible
	   ///          formatting errors, see the <a
	   ///          href="../util/Formatter.html#detail">Details</a>
	   ///          section of the formatter class specification.
	   /// </exception>
	   /// <exception cref="IOError">
	   ///         If an I/O error occurs.
	   /// </exception>
	   /// <returns>  A character array containing the password or passphrase read
	   ///          from the console, not including any line-termination characters,
	   ///          or <tt>null</tt> if an end of stream has been reached. </returns>
		public char[] ReadPassword(String fmt, params Object[] args)
		{
			char[] passwd = null;
			lock (WriteLock)
			{
				lock (ReadLock)
				{
					try
					{
						EchoOff = echo(false);
					}
					catch (IOException x)
					{
						throw new IOError(x);
					}
					IOError ioe = null;
					try
					{
						if (fmt.Length() != 0)
						{
							Pw.Format(fmt, args);
						}
						passwd = Readline(true);
					}
					catch (IOException x)
					{
						ioe = new IOError(x);
					}
					finally
					{
						try
						{
							EchoOff = echo(true);
						}
						catch (IOException x)
						{
							if (ioe == null)
							{
								ioe = new IOError(x);
							}
							else
							{
								ioe.AddSuppressed(x);
							}
						}
						if (ioe != null)
						{
							throw ioe;
						}
					}
					Pw.Println();
				}
			}
			return passwd;
		}

	   /// <summary>
	   /// Reads a password or passphrase from the console with echoing disabled
	   /// </summary>
	   /// <exception cref="IOError">
	   ///         If an I/O error occurs.
	   /// </exception>
	   /// <returns>  A character array containing the password or passphrase read
	   ///          from the console, not including any line-termination characters,
	   ///          or <tt>null</tt> if an end of stream has been reached. </returns>
		public char[] ReadPassword()
		{
			return readPassword("");
		}

		/// <summary>
		/// Flushes the console and forces any buffered output to be written
		/// immediately .
		/// </summary>
		public void Flush()
		{
			Pw.Flush();
		}

		private Object ReadLock;
		private Object WriteLock;
		private Reader Reader_Renamed;
		private Writer @out;
		private PrintWriter Pw;
		private Formatter Formatter;
		private Charset Cs;
		private char[] Rcb;
//JAVA TO C# CONVERTER TODO TASK: Replace 'unknown' with the appropriate dll name:
		[DllImport("unknown")]
		private static extern String encoding();
//JAVA TO C# CONVERTER TODO TASK: Replace 'unknown' with the appropriate dll name:
		[DllImport("unknown")]
		private static extern boolean echo(bool on);
		private static bool EchoOff;

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: private char[] readline(boolean zeroOut) throws IOException
		private char[] Readline(bool zeroOut)
		{
			int len = Reader_Renamed.Read(Rcb, 0, Rcb.Length);
			if (len < 0)
			{
				return null; //EOL
			}
			if (Rcb[len - 1] == '\r')
			{
				len--; //remove CR at end;
			}
			else if (Rcb[len - 1] == '\n')
			{
				len--; //remove LF at end;
				if (len > 0 && Rcb[len - 1] == '\r')
				{
					len--; //remove the CR, if there is one
				}
			}
			char[] b = new char[len];
			if (len > 0)
			{
				System.Array.Copy(Rcb, 0, b, 0, len);
				if (zeroOut)
				{
					Arrays.Fill(Rcb, 0, len, ' ');
				}
			}
			return b;
		}

		private char[] Grow()
		{
			Debug.Assert(Thread.holdsLock(ReadLock));
			char[] t = new char[Rcb.Length * 2];
			System.Array.Copy(Rcb, 0, t, 0, Rcb.Length);
			Rcb = t;
			return Rcb;
		}

		internal class LineReader : Reader
		{
			private readonly Console OuterInstance;

			internal Reader @in;
			internal char[] Cb;
			internal int NChars, NextChar;
			internal bool LeftoverLF;
			internal LineReader(Console outerInstance, Reader @in)
			{
				this.OuterInstance = outerInstance;
				this.@in = @in;
				Cb = new char[1024];
				NextChar = NChars = 0;
				LeftoverLF = false;
			}
			public override void Close()
			{
			}
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public boolean ready() throws IOException
			public override bool Ready()
			{
				//in.ready synchronizes on readLock already
				return @in.Ready();
			}

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public int read(char cbuf[] , int offset, int length) throws IOException
			public override int Read(char[] cbuf, int offset, int length)
			{
				int off = offset;
				int end = offset + length;
				if (offset < 0 || offset > cbuf.Length || length < 0 || end < 0 || end > cbuf.Length)
				{
					throw new IndexOutOfBoundsException();
				}
				lock (outerInstance.ReadLock)
				{
					bool eof = false;
					char c = (char)0;
					for (;;)
					{
						if (NextChar >= NChars) //fill
						{
							int n = 0;
							do
							{
								n = @in.Read(Cb, 0, Cb.Length);
							} while (n == 0);
							if (n > 0)
							{
								NChars = n;
								NextChar = 0;
								if (n < Cb.Length && Cb[n - 1] != '\n' && Cb[n - 1] != '\r')
								{
									/*
									 * we're in canonical mode so each "fill" should
									 * come back with an eol. if there no lf or nl at
									 * the end of returned bytes we reached an eof.
									 */
									eof = true;
								}
							} //EOF
							else
							{
								if (off - offset == 0)
								{
									return -1;
								}
								return off - offset;
							}
						}
						if (LeftoverLF && cbuf == outerInstance.Rcb && Cb[NextChar] == '\n')
						{
							/*
							 * if invoked by our readline, skip the leftover, otherwise
							 * return the LF.
							 */
							NextChar++;
						}
						LeftoverLF = false;
						while (NextChar < NChars)
						{
							c = cbuf[off++] = Cb[NextChar];
//JAVA TO C# CONVERTER TODO TASK: The following line could not be converted:
							cb[nextChar++] = 0;
							if (c == '\n')
							{
								return off - offset;
							}
							else if (c == '\r')
							{
								if (off == end)
								{
									/* no space left even the next is LF, so return
									 * whatever we have if the invoker is not our
									 * readLine()
									 */
									if (cbuf == outerInstance.Rcb)
									{
										cbuf = outerInstance.Grow();
										end = cbuf.Length;
									}
									else
									{
										LeftoverLF = true;
										return off - offset;
									}
								}
								if (NextChar == NChars && @in.Ready())
								{
									/*
									 * we have a CR and we reached the end of
									 * the read in buffer, fill to make sure we
									 * don't miss a LF, if there is one, it's possible
									 * that it got cut off during last round reading
									 * simply because the read in buffer was full.
									 */
									NChars = @in.Read(Cb, 0, Cb.Length);
									NextChar = 0;
								}
								if (NextChar < NChars && Cb[NextChar] == '\n')
								{
									cbuf[off++] = '\n';
									NextChar++;
								}
								return off - offset;
							}
							else if (off == end)
							{
							   if (cbuf == outerInstance.Rcb)
							   {
									cbuf = outerInstance.Grow();
									end = cbuf.Length;
							   }
							   else
							   {
								   return off - offset;
							   }
							}
						}
						if (eof)
						{
							return off - offset;
						}
					}
				}
			}
		}

		// Set up JavaIOAccess in SharedSecrets
		static Console()
		{
			try
			{
				// Add a shutdown hook to restore console's echo state should
				// it be necessary.
				sun.misc.SharedSecrets.JavaLangAccess.registerShutdownHook(0, false, new RunnableAnonymousInnerClassHelper()); // only register if shutdown is not in progress -  shutdown hook invocation order
			}
			catch (IllegalStateException)
			{
				// shutdown is already in progress and console is first used
				// by a shutdown hook
			}

			sun.misc.SharedSecrets.JavaIOAccess = new JavaIOAccessAnonymousInnerClassHelper();
		}

		private class RunnableAnonymousInnerClassHelper : Runnable
		{
			public RunnableAnonymousInnerClassHelper()
			{
			}

			public virtual void Run()
			{
				try
				{
					if (EchoOff)
					{
						echo(true);
					}
				}
				catch (IOException)
				{
				}
			}
		}

		private class JavaIOAccessAnonymousInnerClassHelper : sun.misc.JavaIOAccess
		{
			public JavaIOAccessAnonymousInnerClassHelper()
			{
			}

			public virtual Console Console()
			{
				if (istty())
				{
					if (Cons == null)
					{
						Cons = new Console();
					}
					return Cons;
				}
				return null;
			}

			public virtual Charset Charset()
			{
				// This method is called in sun.security.util.Password,
				// cons already exists when this method is called
				return Cons.Cs;
			}
		}
		private static Console Cons;
//JAVA TO C# CONVERTER TODO TASK: Replace 'unknown' with the appropriate dll name:
		[DllImport("unknown")]
		private extern static boolean istty();
		private Console()
		{
			ReadLock = new Object();
			WriteLock = new Object();
			String csname = encoding();
			if (csname != null)
			{
				try
				{
					Cs = Charset.ForName(csname);
				}
				catch (Exception)
				{
				}
			}
			if (Cs == null)
			{
				Cs = Charset.DefaultCharset();
			}
			@out = StreamEncoder.forOutputStreamWriter(new FileOutputStream(FileDescriptor.@out), WriteLock, Cs);
			Pw = new PrintWriterAnonymousInnerClassHelper(this, @out);
			Formatter = new Formatter(@out);
			Reader_Renamed = new LineReader(this, StreamDecoder.forInputStreamReader(new FileInputStream(FileDescriptor.@in), ReadLock, Cs));
			Rcb = new char[1024];
		}

		private class PrintWriterAnonymousInnerClassHelper : PrintWriter
		{
			private readonly Console OuterInstance;

			public PrintWriterAnonymousInnerClassHelper(Console outerInstance, java.io.Writer @out) : base(@out, true)
			{
				this.OuterInstance = outerInstance;
			}

			public override void Close()
			{
			}
		}
	}

}
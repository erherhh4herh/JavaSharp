using System;
using System.Diagnostics;
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

namespace java.lang
{


	/// <summary>
	/// This class is used to create operating system processes.
	/// 
	/// <para>Each {@code ProcessBuilder} instance manages a collection
	/// of process attributes.  The <seealso cref="#start()"/> method creates a new
	/// <seealso cref="Process"/> instance with those attributes.  The {@link
	/// #start()} method can be invoked repeatedly from the same instance
	/// to create new subprocesses with identical or related attributes.
	/// 
	/// </para>
	/// <para>Each process builder manages these process attributes:
	/// 
	/// <ul>
	/// 
	/// <li>a <i>command</i>, a list of strings which signifies the
	/// external program file to be invoked and its arguments, if any.
	/// Which string lists represent a valid operating system command is
	/// system-dependent.  For example, it is common for each conceptual
	/// argument to be an element in this list, but there are operating
	/// systems where programs are expected to tokenize command line
	/// strings themselves - on such a system a Java implementation might
	/// require commands to contain exactly two elements.
	/// 
	/// <li>an <i>environment</i>, which is a system-dependent mapping from
	/// <i>variables</i> to <i>values</i>.  The initial value is a copy of
	/// the environment of the current process (see <seealso cref="System#getenv()"/>).
	/// 
	/// <li>a <i>working directory</i>.  The default value is the current
	/// working directory of the current process, usually the directory
	/// named by the system property {@code user.dir}.
	/// 
	/// <li><a name="redirect-input">a source of <i>standard input</i></a>.
	/// By default, the subprocess reads input from a pipe.  Java code
	/// can access this pipe via the output stream returned by
	/// <seealso cref="Process#getOutputStream()"/>.  However, standard input may
	/// be redirected to another source using
	/// <seealso cref="#redirectInput(Redirect) redirectInput"/>.
	/// In this case, <seealso cref="Process#getOutputStream()"/> will return a
	/// <i>null output stream</i>, for which:
	/// 
	/// <ul>
	/// <li>the <seealso cref="OutputStream#write(int) write"/> methods always
	/// throw {@code IOException}
	/// <li>the <seealso cref="OutputStream#close() close"/> method does nothing
	/// </ul>
	/// 
	/// <li><a name="redirect-output">a destination for <i>standard output</i>
	/// and <i>standard error</i></a>.  By default, the subprocess writes standard
	/// output and standard error to pipes.  Java code can access these pipes
	/// via the input streams returned by <seealso cref="Process#getInputStream()"/> and
	/// <seealso cref="Process#getErrorStream()"/>.  However, standard output and
	/// standard error may be redirected to other destinations using
	/// <seealso cref="#redirectOutput(Redirect) redirectOutput"/> and
	/// <seealso cref="#redirectError(Redirect) redirectError"/>.
	/// In this case, <seealso cref="Process#getInputStream()"/> and/or
	/// <seealso cref="Process#getErrorStream()"/> will return a <i>null input
	/// stream</i>, for which:
	/// 
	/// <ul>
	/// <li>the <seealso cref="InputStream#read() read"/> methods always return
	/// {@code -1}
	/// <li>the <seealso cref="InputStream#available() available"/> method always returns
	/// {@code 0}
	/// <li>the <seealso cref="InputStream#close() close"/> method does nothing
	/// </ul>
	/// 
	/// <li>a <i>redirectErrorStream</i> property.  Initially, this property
	/// is {@code false}, meaning that the standard output and error
	/// output of a subprocess are sent to two separate streams, which can
	/// be accessed using the <seealso cref="Process#getInputStream()"/> and {@link
	/// Process#getErrorStream()} methods.
	/// 
	/// </para>
	/// <para>If the value is set to {@code true}, then:
	/// 
	/// <ul>
	/// <li>standard error is merged with the standard output and always sent
	/// to the same destination (this makes it easier to correlate error
	/// messages with the corresponding output)
	/// <li>the common destination of standard error and standard output can be
	/// redirected using
	/// <seealso cref="#redirectOutput(Redirect) redirectOutput"/>
	/// <li>any redirection set by the
	/// <seealso cref="#redirectError(Redirect) redirectError"/>
	/// method is ignored when creating a subprocess
	/// <li>the stream returned from <seealso cref="Process#getErrorStream()"/> will
	/// always be a <a href="#redirect-output">null input stream</a>
	/// </ul>
	/// 
	/// </ul>
	/// 
	/// </para>
	/// <para>Modifying a process builder's attributes will affect processes
	/// subsequently started by that object's <seealso cref="#start()"/> method, but
	/// will never affect previously started processes or the Java process
	/// itself.
	/// 
	/// </para>
	/// <para>Most error checking is performed by the <seealso cref="#start()"/> method.
	/// It is possible to modify the state of an object so that {@link
	/// #start()} will fail.  For example, setting the command attribute to
	/// an empty list will not throw an exception unless <seealso cref="#start()"/>
	/// is invoked.
	/// 
	/// </para>
	/// <para><strong>Note that this class is not synchronized.</strong>
	/// If multiple threads access a {@code ProcessBuilder} instance
	/// concurrently, and at least one of the threads modifies one of the
	/// attributes structurally, it <i>must</i> be synchronized externally.
	/// 
	/// </para>
	/// <para>Starting a new process which uses the default working directory
	/// and environment is easy:
	/// 
	/// <pre> {@code
	/// Process p = new ProcessBuilder("myCommand", "myArg").start();
	/// }</pre>
	/// 
	/// </para>
	/// <para>Here is an example that starts a process with a modified working
	/// directory and environment, and redirects standard output and error
	/// to be appended to a log file:
	/// 
	/// <pre> {@code
	/// ProcessBuilder pb =
	///   new ProcessBuilder("myCommand", "myArg1", "myArg2");
	/// Map<String, String> env = pb.environment();
	/// env.put("VAR1", "myValue");
	/// env.remove("OTHERVAR");
	/// env.put("VAR2", env.get("VAR1") + "suffix");
	/// pb.directory(new File("myDir"));
	/// File log = new File("log");
	/// pb.redirectErrorStream(true);
	/// pb.redirectOutput(Redirect.appendTo(log));
	/// Process p = pb.start();
	/// assert pb.redirectInput() == Redirect.PIPE;
	/// assert pb.redirectOutput().file() == log;
	/// assert p.getInputStream().read() == -1;
	/// }</pre>
	/// 
	/// </para>
	/// <para>To start a process with an explicit set of environment
	/// variables, first call <seealso cref="java.util.Map#clear() Map.clear()"/>
	/// before adding environment variables.
	/// 
	/// @author Martin Buchholz
	/// @since 1.5
	/// </para>
	/// </summary>

	public sealed class ProcessBuilder
	{
		private IList<String> Command_Renamed;
		private File Directory_Renamed;
		private IDictionary<String, String> Environment_Renamed;
		private bool RedirectErrorStream_Renamed;
		private Redirect[] Redirects_Renamed;

		/// <summary>
		/// Constructs a process builder with the specified operating
		/// system program and arguments.  This constructor does <i>not</i>
		/// make a copy of the {@code command} list.  Subsequent
		/// updates to the list will be reflected in the state of the
		/// process builder.  It is not checked whether
		/// {@code command} corresponds to a valid operating system
		/// command.
		/// </summary>
		/// <param name="command"> the list containing the program and its arguments </param>
		/// <exception cref="NullPointerException"> if the argument is null </exception>
		public ProcessBuilder(IList<String> command)
		{
			if (command == null)
			{
				throw new NullPointerException();
			}
			this.Command_Renamed = command;
		}

		/// <summary>
		/// Constructs a process builder with the specified operating
		/// system program and arguments.  This is a convenience
		/// constructor that sets the process builder's command to a string
		/// list containing the same strings as the {@code command}
		/// array, in the same order.  It is not checked whether
		/// {@code command} corresponds to a valid operating system
		/// command.
		/// </summary>
		/// <param name="command"> a string array containing the program and its arguments </param>
		public ProcessBuilder(params String[] command)
		{
			this.Command_Renamed = new List<>(command.Length);
			foreach (String arg in command)
			{
				this.Command_Renamed.Add(arg);
			}
		}

		/// <summary>
		/// Sets this process builder's operating system program and
		/// arguments.  This method does <i>not</i> make a copy of the
		/// {@code command} list.  Subsequent updates to the list will
		/// be reflected in the state of the process builder.  It is not
		/// checked whether {@code command} corresponds to a valid
		/// operating system command.
		/// </summary>
		/// <param name="command"> the list containing the program and its arguments </param>
		/// <returns> this process builder
		/// </returns>
		/// <exception cref="NullPointerException"> if the argument is null </exception>
		public ProcessBuilder Command(IList<String> command)
		{
			if (command == null)
			{
				throw new NullPointerException();
			}
			this.Command_Renamed = command;
			return this;
		}

		/// <summary>
		/// Sets this process builder's operating system program and
		/// arguments.  This is a convenience method that sets the command
		/// to a string list containing the same strings as the
		/// {@code command} array, in the same order.  It is not
		/// checked whether {@code command} corresponds to a valid
		/// operating system command.
		/// </summary>
		/// <param name="command"> a string array containing the program and its arguments </param>
		/// <returns> this process builder </returns>
		public ProcessBuilder Command(params String[] command)
		{
			this.Command_Renamed = new List<>(command.Length);
			foreach (String arg in command)
			{
				this.Command_Renamed.Add(arg);
			}
			return this;
		}

		/// <summary>
		/// Returns this process builder's operating system program and
		/// arguments.  The returned list is <i>not</i> a copy.  Subsequent
		/// updates to the list will be reflected in the state of this
		/// process builder.
		/// </summary>
		/// <returns> this process builder's program and its arguments </returns>
		public IList<String> Command()
		{
			return Command_Renamed;
		}

		/// <summary>
		/// Returns a string map view of this process builder's environment.
		/// 
		/// Whenever a process builder is created, the environment is
		/// initialized to a copy of the current process environment (see
		/// <seealso cref="System#getenv()"/>).  Subprocesses subsequently started by
		/// this object's <seealso cref="#start()"/> method will use this map as
		/// their environment.
		/// 
		/// <para>The returned object may be modified using ordinary {@link
		/// java.util.Map Map} operations.  These modifications will be
		/// visible to subprocesses started via the <seealso cref="#start()"/>
		/// method.  Two {@code ProcessBuilder} instances always
		/// contain independent process environments, so changes to the
		/// returned map will never be reflected in any other
		/// {@code ProcessBuilder} instance or the values returned by
		/// <seealso cref="System#getenv System.getenv"/>.
		/// 
		/// </para>
		/// <para>If the system does not support environment variables, an
		/// empty map is returned.
		/// 
		/// </para>
		/// <para>The returned map does not permit null keys or values.
		/// Attempting to insert or query the presence of a null key or
		/// value will throw a <seealso cref="NullPointerException"/>.
		/// Attempting to query the presence of a key or value which is not
		/// of type <seealso cref="String"/> will throw a <seealso cref="ClassCastException"/>.
		/// 
		/// </para>
		/// <para>The behavior of the returned map is system-dependent.  A
		/// system may not allow modifications to environment variables or
		/// may forbid certain variable names or values.  For this reason,
		/// attempts to modify the map may fail with
		/// <seealso cref="UnsupportedOperationException"/> or
		/// <seealso cref="IllegalArgumentException"/>
		/// if the modification is not permitted by the operating system.
		/// 
		/// </para>
		/// <para>Since the external format of environment variable names and
		/// values is system-dependent, there may not be a one-to-one
		/// mapping between them and Java's Unicode strings.  Nevertheless,
		/// the map is implemented in such a way that environment variables
		/// which are not modified by Java code will have an unmodified
		/// native representation in the subprocess.
		/// 
		/// </para>
		/// <para>The returned map and its collection views may not obey the
		/// general contract of the <seealso cref="Object#equals"/> and
		/// <seealso cref="Object#hashCode"/> methods.
		/// 
		/// </para>
		/// <para>The returned map is typically case-sensitive on all platforms.
		/// 
		/// </para>
		/// <para>If a security manager exists, its
		/// <seealso cref="SecurityManager#checkPermission checkPermission"/> method
		/// is called with a
		/// <seealso cref="RuntimePermission"/>{@code ("getenv.*")} permission.
		/// This may result in a <seealso cref="SecurityException"/> being thrown.
		/// 
		/// </para>
		/// <para>When passing information to a Java subprocess,
		/// <a href=System.html#EnvironmentVSSystemProperties>system properties</a>
		/// are generally preferred over environment variables.
		/// 
		/// </para>
		/// </summary>
		/// <returns> this process builder's environment
		/// </returns>
		/// <exception cref="SecurityException">
		///         if a security manager exists and its
		///         <seealso cref="SecurityManager#checkPermission checkPermission"/>
		///         method doesn't allow access to the process environment
		/// </exception>
		/// <seealso cref=    Runtime#exec(String[],String[],java.io.File) </seealso>
		/// <seealso cref=    System#getenv() </seealso>
		public IDictionary<String, String> Environment()
		{
			SecurityManager security = System.SecurityManager;
			if (security != null)
			{
				security.CheckPermission(new RuntimePermission("getenv.*"));
			}

			if (Environment_Renamed == null)
			{
				Environment_Renamed = ProcessEnvironment.Environment();
			}

			Debug.Assert(Environment_Renamed != null);

			return Environment_Renamed;
		}

		// Only for use by Runtime.exec(...envp...)
		internal ProcessBuilder Environment(String[] envp)
		{
			Debug.Assert(Environment_Renamed == null);
			if (envp != null)
			{
				Environment_Renamed = ProcessEnvironment.EmptyEnvironment(envp.Length);
				Debug.Assert(Environment_Renamed != null);

				foreach (String envstring in envp)
				{
					// Before 1.5, we blindly passed invalid envstrings
					// to the child process.
					// We would like to throw an exception, but do not,
					// for compatibility with old broken code.

					// Silently discard any trailing junk.
					if (envstring.IndexOf((int) '\u0000') != -1)
					{
						envstring = envstring.ReplaceFirst("\u0000.*", "");
					}

					int eqlsign = envstring.IndexOf('=', ProcessEnvironment.MIN_NAME_LENGTH);
					// Silently ignore envstrings lacking the required `='.
					if (eqlsign != -1)
					{
						Environment_Renamed[envstring.Substring(0,eqlsign)] = envstring.Substring(eqlsign + 1);
					}
				}
			}
			return this;
		}

		/// <summary>
		/// Returns this process builder's working directory.
		/// 
		/// Subprocesses subsequently started by this object's {@link
		/// #start()} method will use this as their working directory.
		/// The returned value may be {@code null} -- this means to use
		/// the working directory of the current Java process, usually the
		/// directory named by the system property {@code user.dir},
		/// as the working directory of the child process.
		/// </summary>
		/// <returns> this process builder's working directory </returns>
		public File Directory()
		{
			return Directory_Renamed;
		}

		/// <summary>
		/// Sets this process builder's working directory.
		/// 
		/// Subprocesses subsequently started by this object's {@link
		/// #start()} method will use this as their working directory.
		/// The argument may be {@code null} -- this means to use the
		/// working directory of the current Java process, usually the
		/// directory named by the system property {@code user.dir},
		/// as the working directory of the child process.
		/// </summary>
		/// <param name="directory"> the new working directory </param>
		/// <returns> this process builder </returns>
		public ProcessBuilder Directory(File directory)
		{
			this.Directory_Renamed = directory;
			return this;
		}

		// ---------------- I/O Redirection ----------------

		/// <summary>
		/// Implements a <a href="#redirect-output">null input stream</a>.
		/// </summary>
		internal class NullInputStream : InputStream
		{
			internal static readonly NullInputStream INSTANCE = new NullInputStream();
			internal NullInputStream()
			{
			}
			public override int Read()
			{
				return -1;
			}
			public override int Available()
			{
				return 0;
			}
		}

		/// <summary>
		/// Implements a <a href="#redirect-input">null output stream</a>.
		/// </summary>
		internal class NullOutputStream : OutputStream
		{
			internal static readonly NullOutputStream INSTANCE = new NullOutputStream();
			internal NullOutputStream()
			{
			}
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public void write(int b) throws java.io.IOException
			public override void Write(int b)
			{
				throw new IOException("Stream closed");
			}
		}

		/// <summary>
		/// Represents a source of subprocess input or a destination of
		/// subprocess output.
		/// 
		/// Each {@code Redirect} instance is one of the following:
		/// 
		/// <ul>
		/// <li>the special value <seealso cref="#PIPE Redirect.PIPE"/>
		/// <li>the special value <seealso cref="#INHERIT Redirect.INHERIT"/>
		/// <li>a redirection to read from a file, created by an invocation of
		///     <seealso cref="Redirect#from Redirect.from(File)"/>
		/// <li>a redirection to write to a file,  created by an invocation of
		///     <seealso cref="Redirect#to Redirect.to(File)"/>
		/// <li>a redirection to append to a file, created by an invocation of
		///     <seealso cref="Redirect#appendTo Redirect.appendTo(File)"/>
		/// </ul>
		/// 
		/// <para>Each of the above categories has an associated unique
		/// <seealso cref="Type Type"/>.
		/// 
		/// @since 1.7
		/// </para>
		/// </summary>
		public abstract class Redirect
		{
			/// <summary>
			/// The type of a <seealso cref="Redirect"/>.
			/// </summary>
			public enum Type
			{
				/// <summary>
				/// The type of <seealso cref="Redirect#PIPE Redirect.PIPE"/>.
				/// </summary>
				PIPE,

				/// <summary>
				/// The type of <seealso cref="Redirect#INHERIT Redirect.INHERIT"/>.
				/// </summary>
				INHERIT,

				/// <summary>
				/// The type of redirects returned from
				/// <seealso cref="Redirect#from Redirect.from(File)"/>.
				/// </summary>
				READ,

				/// <summary>
				/// The type of redirects returned from
				/// <seealso cref="Redirect#to Redirect.to(File)"/>.
				/// </summary>
				WRITE,

				/// <summary>
				/// The type of redirects returned from
				/// <seealso cref="Redirect#appendTo Redirect.appendTo(File)"/>.
				/// </summary>
				APPEND
			}

			/// <summary>
			/// Returns the type of this {@code Redirect}. </summary>
			/// <returns> the type of this {@code Redirect} </returns>
			public abstract Type Type();

			/// <summary>
			/// Indicates that subprocess I/O will be connected to the
			/// current Java process over a pipe.
			/// 
			/// This is the default handling of subprocess standard I/O.
			/// 
			/// <para>It will always be true that
			///  <pre> {@code
			/// Redirect.PIPE.file() == null &&
			/// Redirect.PIPE.type() == Redirect.Type.PIPE
			/// }</pre>
			/// </para>
			/// </summary>
			public static readonly Redirect PIPE = new RedirectAnonymousInnerClassHelper();

			private class RedirectAnonymousInnerClassHelper : Redirect
			{
				public RedirectAnonymousInnerClassHelper()
				{
				}

				public override Type Type()
				{
					return Type.PIPE;
				}
				public override String ToString()
				{
					return outerInstance.Type().ToString();
				}
			}

			/// <summary>
			/// Indicates that subprocess I/O source or destination will be the
			/// same as those of the current process.  This is the normal
			/// behavior of most operating system command interpreters (shells).
			/// 
			/// <para>It will always be true that
			///  <pre> {@code
			/// Redirect.INHERIT.file() == null &&
			/// Redirect.INHERIT.type() == Redirect.Type.INHERIT
			/// }</pre>
			/// </para>
			/// </summary>
			public static readonly Redirect INHERIT = new RedirectAnonymousInnerClassHelper2();

			private class RedirectAnonymousInnerClassHelper2 : Redirect
			{
				public RedirectAnonymousInnerClassHelper2()
				{
				}

				public override Type Type()
				{
					return Type.INHERIT;
				}
				public override String ToString()
				{
					return outerInstance.Type().ToString();
				}
			}

			/// <summary>
			/// Returns the <seealso cref="File"/> source or destination associated
			/// with this redirect, or {@code null} if there is no such file.
			/// </summary>
			/// <returns> the file associated with this redirect,
			///         or {@code null} if there is no such file </returns>
			public virtual File File()
			{
				return null;
			}

			/// <summary>
			/// When redirected to a destination file, indicates if the output
			/// is to be written to the end of the file.
			/// </summary>
			internal virtual bool Append()
			{
				throw new UnsupportedOperationException();
			}

			/// <summary>
			/// Returns a redirect to read from the specified file.
			/// 
			/// <para>It will always be true that
			///  <pre> {@code
			/// Redirect.from(file).file() == file &&
			/// Redirect.from(file).type() == Redirect.Type.READ
			/// }</pre>
			/// 
			/// </para>
			/// </summary>
			/// <param name="file"> The {@code File} for the {@code Redirect}. </param>
			/// <exception cref="NullPointerException"> if the specified file is null </exception>
			/// <returns> a redirect to read from the specified file </returns>
//JAVA TO C# CONVERTER WARNING: 'final' parameters are not allowed in .NET:
//ORIGINAL LINE: public static Redirect from(final java.io.File file)
			public static Redirect From(File file)
			{
				if (file == null)
				{
					throw new NullPointerException();
				}
				return new RedirectAnonymousInnerClassHelper3(file);
			}

			private class RedirectAnonymousInnerClassHelper3 : Redirect
			{
				private File File;

				public RedirectAnonymousInnerClassHelper3(File file)
				{
					this.File = file;
				}

				public override Type Type()
				{
					return Type.READ;
				}
				public override File File()
				{
					return File;
				}
				public override String ToString()
				{
					return "redirect to read from file \"" + File + "\"";
				}
			}

			/// <summary>
			/// Returns a redirect to write to the specified file.
			/// If the specified file exists when the subprocess is started,
			/// its previous contents will be discarded.
			/// 
			/// <para>It will always be true that
			///  <pre> {@code
			/// Redirect.to(file).file() == file &&
			/// Redirect.to(file).type() == Redirect.Type.WRITE
			/// }</pre>
			/// 
			/// </para>
			/// </summary>
			/// <param name="file"> The {@code File} for the {@code Redirect}. </param>
			/// <exception cref="NullPointerException"> if the specified file is null </exception>
			/// <returns> a redirect to write to the specified file </returns>
//JAVA TO C# CONVERTER WARNING: 'final' parameters are not allowed in .NET:
//ORIGINAL LINE: public static Redirect to(final java.io.File file)
			public static Redirect To(File file)
			{
				if (file == null)
				{
					throw new NullPointerException();
				}
				return new RedirectAnonymousInnerClassHelper4(file);
			}

			private class RedirectAnonymousInnerClassHelper4 : Redirect
			{
				private File File;

				public RedirectAnonymousInnerClassHelper4(File file)
				{
					this.File = file;
				}

				public override Type Type()
				{
					return Type.WRITE;
				}
				public override File File()
				{
					return File;
				}
				public override String ToString()
				{
					return "redirect to write to file \"" + File + "\"";
				}
				internal override bool Append()
				{
					return false;
				}
			}

			/// <summary>
			/// Returns a redirect to append to the specified file.
			/// Each write operation first advances the position to the
			/// end of the file and then writes the requested data.
			/// Whether the advancement of the position and the writing
			/// of the data are done in a single atomic operation is
			/// system-dependent and therefore unspecified.
			/// 
			/// <para>It will always be true that
			///  <pre> {@code
			/// Redirect.appendTo(file).file() == file &&
			/// Redirect.appendTo(file).type() == Redirect.Type.APPEND
			/// }</pre>
			/// 
			/// </para>
			/// </summary>
			/// <param name="file"> The {@code File} for the {@code Redirect}. </param>
			/// <exception cref="NullPointerException"> if the specified file is null </exception>
			/// <returns> a redirect to append to the specified file </returns>
//JAVA TO C# CONVERTER WARNING: 'final' parameters are not allowed in .NET:
//ORIGINAL LINE: public static Redirect appendTo(final java.io.File file)
			public static Redirect AppendTo(File file)
			{
				if (file == null)
				{
					throw new NullPointerException();
				}
				return new RedirectAnonymousInnerClassHelper5(file);
			}

			private class RedirectAnonymousInnerClassHelper5 : Redirect
			{
				private File File;

				public RedirectAnonymousInnerClassHelper5(File file)
				{
					this.File = file;
				}

				public override Type Type()
				{
					return Type.APPEND;
				}
				public override File File()
				{
					return File;
				}
				public override String ToString()
				{
					return "redirect to append to file \"" + File + "\"";
				}
				internal override bool Append()
				{
					return true;
				}
			}

			/// <summary>
			/// Compares the specified object with this {@code Redirect} for
			/// equality.  Returns {@code true} if and only if the two
			/// objects are identical or both objects are {@code Redirect}
			/// instances of the same type associated with non-null equal
			/// {@code File} instances.
			/// </summary>
			public override bool Equals(Object obj)
			{
				if (obj == this)
				{
					return true;
				}
				if (!(obj is Redirect))
				{
					return false;
				}
				Redirect r = (Redirect) obj;
				if (r.Type() != this.Type())
				{
					return false;
				}
				Debug.Assert(this.File() != null);
				return this.File().Equals(r.File());
			}

			/// <summary>
			/// Returns a hash code value for this {@code Redirect}. </summary>
			/// <returns> a hash code value for this {@code Redirect} </returns>
			public override int HashCode()
			{
				File file = File();
				if (file == null)
				{
					return base.HashCode();
				}
				else
				{
					return file.HashCode();
				}
			}

			/// <summary>
			/// No public constructors.  Clients must use predefined
			/// static {@code Redirect} instances or factory methods.
			/// </summary>
			internal Redirect()
			{
			}
		}

		private Redirect[] Redirects()
		{
			if (Redirects_Renamed == null)
			{
				Redirects_Renamed = new Redirect[] {Redirect.PIPE, Redirect.PIPE, Redirect.PIPE};
			}
			return Redirects_Renamed;
		}

		/// <summary>
		/// Sets this process builder's standard input source.
		/// 
		/// Subprocesses subsequently started by this object's <seealso cref="#start()"/>
		/// method obtain their standard input from this source.
		/// 
		/// <para>If the source is <seealso cref="Redirect#PIPE Redirect.PIPE"/>
		/// (the initial value), then the standard input of a
		/// subprocess can be written to using the output stream
		/// returned by <seealso cref="Process#getOutputStream()"/>.
		/// If the source is set to any other value, then
		/// <seealso cref="Process#getOutputStream()"/> will return a
		/// <a href="#redirect-input">null output stream</a>.
		/// 
		/// </para>
		/// </summary>
		/// <param name="source"> the new standard input source </param>
		/// <returns> this process builder </returns>
		/// <exception cref="IllegalArgumentException">
		///         if the redirect does not correspond to a valid source
		///         of data, that is, has type
		///         <seealso cref="Redirect.Type#WRITE WRITE"/> or
		///         <seealso cref="Redirect.Type#APPEND APPEND"/>
		/// @since  1.7 </exception>
		public ProcessBuilder RedirectInput(Redirect source)
		{
			if (source.Type() == Redirect.Type.WRITE || source.Type() == Redirect.Type.APPEND)
			{
				throw new IllegalArgumentException("Redirect invalid for reading: " + source);
			}
			Redirects()[0] = source;
			return this;
		}

		/// <summary>
		/// Sets this process builder's standard output destination.
		/// 
		/// Subprocesses subsequently started by this object's <seealso cref="#start()"/>
		/// method send their standard output to this destination.
		/// 
		/// <para>If the destination is <seealso cref="Redirect#PIPE Redirect.PIPE"/>
		/// (the initial value), then the standard output of a subprocess
		/// can be read using the input stream returned by {@link
		/// Process#getInputStream()}.
		/// If the destination is set to any other value, then
		/// <seealso cref="Process#getInputStream()"/> will return a
		/// <a href="#redirect-output">null input stream</a>.
		/// 
		/// </para>
		/// </summary>
		/// <param name="destination"> the new standard output destination </param>
		/// <returns> this process builder </returns>
		/// <exception cref="IllegalArgumentException">
		///         if the redirect does not correspond to a valid
		///         destination of data, that is, has type
		///         <seealso cref="Redirect.Type#READ READ"/>
		/// @since  1.7 </exception>
		public ProcessBuilder RedirectOutput(Redirect destination)
		{
			if (destination.Type() == Redirect.Type.READ)
			{
				throw new IllegalArgumentException("Redirect invalid for writing: " + destination);
			}
			Redirects()[1] = destination;
			return this;
		}

		/// <summary>
		/// Sets this process builder's standard error destination.
		/// 
		/// Subprocesses subsequently started by this object's <seealso cref="#start()"/>
		/// method send their standard error to this destination.
		/// 
		/// <para>If the destination is <seealso cref="Redirect#PIPE Redirect.PIPE"/>
		/// (the initial value), then the error output of a subprocess
		/// can be read using the input stream returned by {@link
		/// Process#getErrorStream()}.
		/// If the destination is set to any other value, then
		/// <seealso cref="Process#getErrorStream()"/> will return a
		/// <a href="#redirect-output">null input stream</a>.
		/// 
		/// </para>
		/// <para>If the <seealso cref="#redirectErrorStream redirectErrorStream"/>
		/// attribute has been set {@code true}, then the redirection set
		/// by this method has no effect.
		/// 
		/// </para>
		/// </summary>
		/// <param name="destination"> the new standard error destination </param>
		/// <returns> this process builder </returns>
		/// <exception cref="IllegalArgumentException">
		///         if the redirect does not correspond to a valid
		///         destination of data, that is, has type
		///         <seealso cref="Redirect.Type#READ READ"/>
		/// @since  1.7 </exception>
		public ProcessBuilder RedirectError(Redirect destination)
		{
			if (destination.Type() == Redirect.Type.READ)
			{
				throw new IllegalArgumentException("Redirect invalid for writing: " + destination);
			}
			Redirects()[2] = destination;
			return this;
		}

		/// <summary>
		/// Sets this process builder's standard input source to a file.
		/// 
		/// <para>This is a convenience method.  An invocation of the form
		/// {@code redirectInput(file)}
		/// behaves in exactly the same way as the invocation
		/// <seealso cref="#redirectInput(Redirect) redirectInput"/>
		/// {@code (Redirect.from(file))}.
		/// 
		/// </para>
		/// </summary>
		/// <param name="file"> the new standard input source </param>
		/// <returns> this process builder
		/// @since  1.7 </returns>
		public ProcessBuilder RedirectInput(File file)
		{
			return RedirectInput(Redirect.From(file));
		}

		/// <summary>
		/// Sets this process builder's standard output destination to a file.
		/// 
		/// <para>This is a convenience method.  An invocation of the form
		/// {@code redirectOutput(file)}
		/// behaves in exactly the same way as the invocation
		/// <seealso cref="#redirectOutput(Redirect) redirectOutput"/>
		/// {@code (Redirect.to(file))}.
		/// 
		/// </para>
		/// </summary>
		/// <param name="file"> the new standard output destination </param>
		/// <returns> this process builder
		/// @since  1.7 </returns>
		public ProcessBuilder RedirectOutput(File file)
		{
			return RedirectOutput(Redirect.To(file));
		}

		/// <summary>
		/// Sets this process builder's standard error destination to a file.
		/// 
		/// <para>This is a convenience method.  An invocation of the form
		/// {@code redirectError(file)}
		/// behaves in exactly the same way as the invocation
		/// <seealso cref="#redirectError(Redirect) redirectError"/>
		/// {@code (Redirect.to(file))}.
		/// 
		/// </para>
		/// </summary>
		/// <param name="file"> the new standard error destination </param>
		/// <returns> this process builder
		/// @since  1.7 </returns>
		public ProcessBuilder RedirectError(File file)
		{
			return RedirectError(Redirect.To(file));
		}

		/// <summary>
		/// Returns this process builder's standard input source.
		/// 
		/// Subprocesses subsequently started by this object's <seealso cref="#start()"/>
		/// method obtain their standard input from this source.
		/// The initial value is <seealso cref="Redirect#PIPE Redirect.PIPE"/>.
		/// </summary>
		/// <returns> this process builder's standard input source
		/// @since  1.7 </returns>
		public Redirect RedirectInput()
		{
			return (Redirects_Renamed == null) ? Redirect.PIPE : Redirects_Renamed[0];
		}

		/// <summary>
		/// Returns this process builder's standard output destination.
		/// 
		/// Subprocesses subsequently started by this object's <seealso cref="#start()"/>
		/// method redirect their standard output to this destination.
		/// The initial value is <seealso cref="Redirect#PIPE Redirect.PIPE"/>.
		/// </summary>
		/// <returns> this process builder's standard output destination
		/// @since  1.7 </returns>
		public Redirect RedirectOutput()
		{
			return (Redirects_Renamed == null) ? Redirect.PIPE : Redirects_Renamed[1];
		}

		/// <summary>
		/// Returns this process builder's standard error destination.
		/// 
		/// Subprocesses subsequently started by this object's <seealso cref="#start()"/>
		/// method redirect their standard error to this destination.
		/// The initial value is <seealso cref="Redirect#PIPE Redirect.PIPE"/>.
		/// </summary>
		/// <returns> this process builder's standard error destination
		/// @since  1.7 </returns>
		public Redirect RedirectError()
		{
			return (Redirects_Renamed == null) ? Redirect.PIPE : Redirects_Renamed[2];
		}

		/// <summary>
		/// Sets the source and destination for subprocess standard I/O
		/// to be the same as those of the current Java process.
		/// 
		/// <para>This is a convenience method.  An invocation of the form
		///  <pre> {@code
		/// pb.inheritIO()
		/// }</pre>
		/// behaves in exactly the same way as the invocation
		///  <pre> {@code
		/// pb.redirectInput(Redirect.INHERIT)
		///   .redirectOutput(Redirect.INHERIT)
		///   .redirectError(Redirect.INHERIT)
		/// }</pre>
		/// 
		/// This gives behavior equivalent to most operating system
		/// command interpreters, or the standard C library function
		/// {@code system()}.
		/// 
		/// </para>
		/// </summary>
		/// <returns> this process builder
		/// @since  1.7 </returns>
		public ProcessBuilder InheritIO()
		{
			Arrays.Fill(Redirects(), Redirect.INHERIT);
			return this;
		}

		/// <summary>
		/// Tells whether this process builder merges standard error and
		/// standard output.
		/// 
		/// <para>If this property is {@code true}, then any error output
		/// generated by subprocesses subsequently started by this object's
		/// <seealso cref="#start()"/> method will be merged with the standard
		/// output, so that both can be read using the
		/// <seealso cref="Process#getInputStream()"/> method.  This makes it easier
		/// to correlate error messages with the corresponding output.
		/// The initial value is {@code false}.
		/// 
		/// </para>
		/// </summary>
		/// <returns> this process builder's {@code redirectErrorStream} property </returns>
		public bool RedirectErrorStream()
		{
			return RedirectErrorStream_Renamed;
		}

		/// <summary>
		/// Sets this process builder's {@code redirectErrorStream} property.
		/// 
		/// <para>If this property is {@code true}, then any error output
		/// generated by subprocesses subsequently started by this object's
		/// <seealso cref="#start()"/> method will be merged with the standard
		/// output, so that both can be read using the
		/// <seealso cref="Process#getInputStream()"/> method.  This makes it easier
		/// to correlate error messages with the corresponding output.
		/// The initial value is {@code false}.
		/// 
		/// </para>
		/// </summary>
		/// <param name="redirectErrorStream"> the new property value </param>
		/// <returns> this process builder </returns>
		public ProcessBuilder RedirectErrorStream(bool redirectErrorStream)
		{
			this.RedirectErrorStream_Renamed = redirectErrorStream;
			return this;
		}

		/// <summary>
		/// Starts a new process using the attributes of this process builder.
		/// 
		/// <para>The new process will
		/// invoke the command and arguments given by <seealso cref="#command()"/>,
		/// in a working directory as given by <seealso cref="#directory()"/>,
		/// with a process environment as given by <seealso cref="#environment()"/>.
		/// 
		/// </para>
		/// <para>This method checks that the command is a valid operating
		/// system command.  Which commands are valid is system-dependent,
		/// but at the very least the command must be a non-empty list of
		/// non-null strings.
		/// 
		/// </para>
		/// <para>A minimal set of system dependent environment variables may
		/// be required to start a process on some operating systems.
		/// As a result, the subprocess may inherit additional environment variable
		/// settings beyond those in the process builder's <seealso cref="#environment()"/>.
		/// 
		/// </para>
		/// <para>If there is a security manager, its
		/// <seealso cref="SecurityManager#checkExec checkExec"/>
		/// method is called with the first component of this object's
		/// {@code command} array as its argument. This may result in
		/// a <seealso cref="SecurityException"/> being thrown.
		/// 
		/// </para>
		/// <para>Starting an operating system process is highly system-dependent.
		/// Among the many things that can go wrong are:
		/// <ul>
		/// <li>The operating system program file was not found.
		/// <li>Access to the program file was denied.
		/// <li>The working directory does not exist.
		/// </ul>
		/// 
		/// </para>
		/// <para>In such cases an exception will be thrown.  The exact nature
		/// of the exception is system-dependent, but it will always be a
		/// subclass of <seealso cref="IOException"/>.
		/// 
		/// </para>
		/// <para>Subsequent modifications to this process builder will not
		/// affect the returned <seealso cref="Process"/>.
		/// 
		/// </para>
		/// </summary>
		/// <returns> a new <seealso cref="Process"/> object for managing the subprocess
		/// </returns>
		/// <exception cref="NullPointerException">
		///         if an element of the command list is null
		/// </exception>
		/// <exception cref="IndexOutOfBoundsException">
		///         if the command is an empty list (has size {@code 0})
		/// </exception>
		/// <exception cref="SecurityException">
		///         if a security manager exists and
		///         <ul>
		/// 
		///         <li>its
		///         <seealso cref="SecurityManager#checkExec checkExec"/>
		///         method doesn't allow creation of the subprocess, or
		/// 
		///         <li>the standard input to the subprocess was
		///         <seealso cref="#redirectInput redirected from a file"/>
		///         and the security manager's
		///         <seealso cref="SecurityManager#checkRead checkRead"/> method
		///         denies read access to the file, or
		/// 
		///         <li>the standard output or standard error of the
		///         subprocess was
		///         <seealso cref="#redirectOutput redirected to a file"/>
		///         and the security manager's
		///         <seealso cref="SecurityManager#checkWrite checkWrite"/> method
		///         denies write access to the file
		/// 
		///         </ul>
		/// </exception>
		/// <exception cref="IOException"> if an I/O error occurs
		/// </exception>
		/// <seealso cref= Runtime#exec(String[], String[], java.io.File) </seealso>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public Process start() throws java.io.IOException
		public Process Start()
		{
			// Must convert to array first -- a malicious user-supplied
			// list might try to circumvent the security check.
			String[] cmdarray = Command_Renamed.ToArray();
			cmdarray = cmdarray.clone();

			foreach (String arg in cmdarray)
			{
				if (arg == null)
				{
					throw new NullPointerException();
				}
			}
			// Throws IndexOutOfBoundsException if command is empty
			String prog = cmdarray[0];

			SecurityManager security = System.SecurityManager;
			if (security != null)
			{
				security.CheckExec(prog);
			}

			String dir = Directory_Renamed == null ? null : Directory_Renamed.ToString();

			for (int i = 1; i < cmdarray.Length; i++)
			{
				if (cmdarray[i].IndexOf('\u0000') >= 0)
				{
					throw new IOException("invalid null character in command");
				}
			}

			try
			{
				return ProcessImpl.Start(cmdarray, Environment_Renamed, dir, Redirects_Renamed, RedirectErrorStream_Renamed);
			}
//JAVA TO C# CONVERTER TODO TASK: There is no equivalent in C# to Java 'multi-catch' syntax:
			catch (IOException | IllegalArgumentException e)
			{
				String exceptionInfo = ": " + e.Message;
				Throwable cause = e;
				if ((e is IOException) && security != null)
				{
					// Can not disclose the fail reason for read-protected files.
					try
					{
						security.CheckRead(prog);
					}
					catch (SecurityException se)
					{
						exceptionInfo = "";
						cause = se;
					}
				}
				// It's much easier for us to create a high-quality error
				// message than the low-level C code which found the problem.
				throw new IOException("Cannot run program \"" + prog + "\"" + (dir == null ? "" : " (in directory \"" + dir + "\")") + exceptionInfo, cause);
			}
		}
	}

}
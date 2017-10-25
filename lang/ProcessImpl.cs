using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;

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

namespace java.lang
{


	/* This class is for the exclusive use of ProcessBuilder.start() to
	 * create new processes.
	 *
	 * @author Martin Buchholz
	 * @since   1.5
	 */

	internal sealed class ProcessImpl : Process
	{
		private static readonly sun.misc.JavaIOFileDescriptorAccess FdAccess = sun.misc.SharedSecrets.JavaIOFileDescriptorAccess;

		/// <summary>
		/// Open a file for writing. If {@code append} is {@code true} then the file
		/// is opened for atomic append directly and a FileOutputStream constructed
		/// with the resulting handle. This is because a FileOutputStream created
		/// to append to a file does not open the file in a manner that guarantees
		/// that writes by the child process will be atomic.
		/// </summary>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: private static java.io.FileOutputStream newFileOutputStream(java.io.File f, boolean append) throws java.io.IOException
		private static FileOutputStream NewFileOutputStream(File f, bool append)
		{
			if (append)
			{
				String path = f.Path;
				SecurityManager sm = System.SecurityManager;
				if (sm != null)
				{
					sm.CheckWrite(path);
				}
				long handle = openForAtomicAppend(path);
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final java.io.FileDescriptor fd = new java.io.FileDescriptor();
				FileDescriptor fd = new FileDescriptor();
				FdAccess.setHandle(fd, handle);
				return AccessController.doPrivileged(new PrivilegedActionAnonymousInnerClassHelper(fd)
			   );
			}
			else
			{
				return new FileOutputStream(f);
			}
		}

		private class PrivilegedActionAnonymousInnerClassHelper : PrivilegedAction<FileOutputStream>
		{
			private FileDescriptor Fd;

			public PrivilegedActionAnonymousInnerClassHelper(FileDescriptor fd)
			{
				this.Fd = fd;
			}

			public virtual FileOutputStream Run()
			{
				return new FileOutputStream(Fd);
			}
		}

		// System-dependent portion of ProcessBuilder.start()
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: static Process start(String cmdarray[] , java.util.Map<String,String> environment, String dir, ProcessBuilder.Redirect[] redirects, boolean redirectErrorStream) throws java.io.IOException
		internal static Process Start(String[] cmdarray, IDictionary<String, String> environment, String dir, ProcessBuilder.Redirect[] redirects, bool redirectErrorStream)
		{
			String envblock = ProcessEnvironment.ToEnvironmentBlock(environment);

			FileInputStream f0 = null;
			FileOutputStream f1 = null;
			FileOutputStream f2 = null;

			try
			{
				long[] stdHandles;
				if (redirects == null)
				{
					stdHandles = new long[] {-1L, -1L, -1L};
				}
				else
				{
					stdHandles = new long[3];

					if (redirects[0] == Redirect.PIPE)
					{
						stdHandles[0] = -1L;
					}
					else if (redirects[0] == Redirect.INHERIT)
					{
						stdHandles[0] = FdAccess.getHandle(FileDescriptor.@in);
					}
					else
					{
						f0 = new FileInputStream(redirects[0].File());
						stdHandles[0] = FdAccess.getHandle(f0.FD);
					}

					if (redirects[1] == Redirect.PIPE)
					{
						stdHandles[1] = -1L;
					}
					else if (redirects[1] == Redirect.INHERIT)
					{
						stdHandles[1] = FdAccess.getHandle(FileDescriptor.@out);
					}
					else
					{
						f1 = NewFileOutputStream(redirects[1].File(), redirects[1].Append());
						stdHandles[1] = FdAccess.getHandle(f1.FD);
					}

					if (redirects[2] == Redirect.PIPE)
					{
						stdHandles[2] = -1L;
					}
					else if (redirects[2] == Redirect.INHERIT)
					{
						stdHandles[2] = FdAccess.getHandle(FileDescriptor.Err);
					}
					else
					{
						f2 = NewFileOutputStream(redirects[2].File(), redirects[2].Append());
						stdHandles[2] = FdAccess.getHandle(f2.FD);
					}
				}

				return new ProcessImpl(cmdarray, envblock, dir, stdHandles, redirectErrorStream);
			}
			finally
			{
				// In theory, close() can throw IOException
				// (although it is rather unlikely to happen here)
				try
				{
					if (f0 != null)
					{
						f0.Close();
					}
				}
				finally
				{
					try
					{
						if (f1 != null)
						{
							f1.Close();
						}
					}
					finally
					{
						if (f2 != null)
						{
							f2.Close();
						}
					}
				}
			}

		}

		private class LazyPattern
		{
			// Escape-support version:
			//    "(\")((?:\\\\\\1|.)+?)\\1|([^\\s\"]+)";
			internal static readonly Pattern PATTERN = Pattern.Compile("[^\\s\"]+|\"[^\"]*\"");
		}

		/* Parses the command string parameter into the executable name and
		 * program arguments.
		 *
		 * The command string is broken into tokens. The token separator is a space
		 * or quota character. The space inside quotation is not a token separator.
		 * There are no escape sequences.
		 */
		private static String[] GetTokensFromCommand(String command)
		{
			List<String> matchList = new List<String>(8);
			Matcher regexMatcher = LazyPattern.PATTERN.Matcher(command);
			while (regexMatcher.Find())
			{
				matchList.Add(regexMatcher.Group());
			}
			return matchList.ToArray();
		}

		private const int VERIFICATION_CMD_BAT = 0;
		private const int VERIFICATION_WIN32 = 1;
		private const int VERIFICATION_LEGACY = 2;
		private static readonly char[][] ESCAPE_VERIFICATION = new char[][] {new char[] {' ', '\t', '<', '>', '&', '|', '^'}, new char[] {' ', '\t', '<', '>'}, new char[] {' ', '\t'}};

//JAVA TO C# CONVERTER WARNING: 'final' parameters are not allowed in .NET:
//ORIGINAL LINE: private static String createCommandLine(int verificationType, final String executablePath, final String cmd[])
		private static String CreateCommandLine(int verificationType, String executablePath, String[] cmd)
		{
			StringBuilder cmdbuf = new StringBuilder(80);

			cmdbuf.Append(executablePath);

			for (int i = 1; i < cmd.Length; ++i)
			{
				cmdbuf.Append(' ');
				String s = cmd[i];
				if (NeedsEscaping(verificationType, s))
				{
					cmdbuf.Append('"').Append(s);

					// The code protects the [java.exe] and console command line
					// parser, that interprets the [\"] combination as an escape
					// sequence for the ["] char.
					//     http://msdn.microsoft.com/en-us/library/17w5ykft.aspx
					//
					// If the argument is an FS path, doubling of the tail [\]
					// char is not a problem for non-console applications.
					//
					// The [\"] sequence is not an escape sequence for the [cmd.exe]
					// command line parser. The case of the [""] tail escape
					// sequence could not be realized due to the argument validation
					// procedure.
					if ((verificationType != VERIFICATION_CMD_BAT) && s.EndsWith("\\"))
					{
						cmdbuf.Append('\\');
					}
					cmdbuf.Append('"');
				}
				else
				{
					cmdbuf.Append(s);
				}
			}
			return cmdbuf.ToString();
		}

		private static bool IsQuoted(bool noQuotesInside, String arg, String errorMessage)
		{
			int lastPos = arg.Length() - 1;
			if (lastPos >= 1 && arg.CharAt(0) == '"' && arg.CharAt(lastPos) == '"')
			{
				// The argument has already been quoted.
				if (noQuotesInside)
				{
					if (arg.IndexOf('"', 1) != lastPos)
					{
						// There is ["] inside.
						throw new IllegalArgumentException(errorMessage);
					}
				}
				return true;
			}
			if (noQuotesInside)
			{
				if (arg.IndexOf('"') >= 0)
				{
					// There is ["] inside.
					throw new IllegalArgumentException(errorMessage);
				}
			}
			return false;
		}

		private static bool NeedsEscaping(int verificationType, String arg)
		{
			// Switch off MS heuristic for internal ["].
			// Please, use the explicit [cmd.exe] call
			// if you need the internal ["].
			//    Example: "cmd.exe", "/C", "Extended_MS_Syntax"

			// For [.exe] or [.com] file the unpaired/internal ["]
			// in the argument is not a problem.
			bool argIsQuoted = IsQuoted((verificationType == VERIFICATION_CMD_BAT), arg, "Argument has embedded quote, use the explicit CMD.EXE call.");

			if (!argIsQuoted)
			{
				char[] testEscape = ESCAPE_VERIFICATION[verificationType];
				for (int i = 0; i < testEscape.Length; ++i)
				{
					if (arg.IndexOf(testEscape[i]) >= 0)
					{
						return true;
					}
				}
			}
			return false;
		}

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: private static String getExecutablePath(String path) throws java.io.IOException
		private static String GetExecutablePath(String path)
		{
			bool pathIsQuoted = IsQuoted(true, path, "Executable name has embedded quote, split the arguments");

			// Win32 CreateProcess requires path to be normalized
			File fileToRun = new File(pathIsQuoted ? path.Substring(1, path.Length() - 1 - 1) : path);

			// From the [CreateProcess] function documentation:
			//
			// "If the file name does not contain an extension, .exe is appended.
			// Therefore, if the file name extension is .com, this parameter
			// must include the .com extension. If the file name ends in
			// a period (.) with no extension, or if the file name contains a path,
			// .exe is not appended."
			//
			// "If the file name !does not contain a directory path!,
			// the system searches for the executable file in the following
			// sequence:..."
			//
			// In practice ANY non-existent path is extended by [.exe] extension
			// in the [CreateProcess] funcion with the only exception:
			// the path ends by (.)

			return fileToRun.Path;
		}


		private bool IsShellFile(String executablePath)
		{
			String upPath = executablePath.ToUpperCase();
			return (upPath.EndsWith(".CMD") || upPath.EndsWith(".BAT"));
		}

		private String QuoteString(String arg)
		{
			StringBuilder argbuf = new StringBuilder(arg.Length() + 2);
			return argbuf.Append('"').Append(arg).Append('"').ToString();
		}


		private long Handle = 0;
		private OutputStream Stdin_stream;
		private InputStream Stdout_stream;
		private InputStream Stderr_stream;

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: private ProcessImpl(String cmd[] , final String envblock, final String path, final long[] stdHandles, final boolean redirectErrorStream) throws java.io.IOException
//JAVA TO C# CONVERTER WARNING: 'final' parameters are not allowed in .NET:
		private ProcessImpl(String[] cmd, String envblock, String path, long[] stdHandles, bool redirectErrorStream)
		{
			String cmdstr;
			SecurityManager security = System.SecurityManager;
			bool allowAmbiguousCommands = false;
			if (security == null)
			{
				allowAmbiguousCommands = true;
				String value = System.getProperty("jdk.lang.Process.allowAmbiguousCommands");
				if (value != null)
				{
					allowAmbiguousCommands = !"false".Equals(value, StringComparison.CurrentCultureIgnoreCase);
				}
			}
			if (allowAmbiguousCommands)
			{
				// Legacy mode.

				// Normalize path if possible.
				String executablePath = (new File(cmd[0])).Path;

				// No worry about internal, unpaired ["], and redirection/piping.
				if (NeedsEscaping(VERIFICATION_LEGACY, executablePath))
				{
					executablePath = QuoteString(executablePath);
				}

				cmdstr = CreateCommandLine(VERIFICATION_LEGACY, executablePath, cmd);
					//legacy mode doesn't worry about extended verification
			}
			else
			{
				String executablePath;
				try
				{
					executablePath = GetExecutablePath(cmd[0]);
				}
				catch (IllegalArgumentException)
				{
					// Workaround for the calls like
					// Runtime.getRuntime().exec("\"C:\\Program Files\\foo\" bar")

					// No chance to avoid CMD/BAT injection, except to do the work
					// right from the beginning. Otherwise we have too many corner
					// cases from
					//    Runtime.getRuntime().exec(String[] cmd [, ...])
					// calls with internal ["] and escape sequences.

					// Restore original command line.
					StringBuilder join = new StringBuilder();
					// terminal space in command line is ok
					foreach (String s in cmd)
					{
						join.Append(s).Append(' ');
					}

					// Parse the command line again.
					cmd = GetTokensFromCommand(join.ToString());
					executablePath = GetExecutablePath(cmd[0]);

					// Check new executable name once more
					if (security != null)
					{
						security.CheckExec(executablePath);
					}
				}

				// Quotation protects from interpretation of the [path] argument as
				// start of longer path with spaces. Quotation has no influence to
				// [.exe] extension heuristic.
				cmdstr = CreateCommandLine(IsShellFile(executablePath) ? VERIFICATION_CMD_BAT : VERIFICATION_WIN32, QuoteString(executablePath), cmd);
						// We need the extended verification procedure for CMD files.
			}

			Handle = create(cmdstr, envblock, path, stdHandles, redirectErrorStream);

			AccessController.doPrivileged(new PrivilegedActionAnonymousInnerClassHelper(this, stdHandles));
		}

		private class PrivilegedActionAnonymousInnerClassHelper : PrivilegedAction<Void>
		{
			private readonly ProcessImpl OuterInstance;

			private long[] StdHandles;

			public PrivilegedActionAnonymousInnerClassHelper(ProcessImpl outerInstance, long[] stdHandles)
			{
				this.OuterInstance = outerInstance;
				this.StdHandles = stdHandles;
			}

			public virtual Void Run()
			{
				if (StdHandles[0] == -1L)
				{
					OuterInstance.Stdin_stream = ProcessBuilder.NullOutputStream.INSTANCE;
				}
				else
				{
					FileDescriptor stdin_fd = new FileDescriptor();
					FdAccess.setHandle(stdin_fd, StdHandles[0]);
					OuterInstance.Stdin_stream = new BufferedOutputStream(new FileOutputStream(stdin_fd));
				}

				if (StdHandles[1] == -1L)
				{
					OuterInstance.Stdout_stream = ProcessBuilder.NullInputStream.INSTANCE;
				}
				else
				{
					FileDescriptor stdout_fd = new FileDescriptor();
					FdAccess.setHandle(stdout_fd, StdHandles[1]);
					OuterInstance.Stdout_stream = new BufferedInputStream(new FileInputStream(stdout_fd));
				}

				if (StdHandles[2] == -1L)
				{
					OuterInstance.Stderr_stream = ProcessBuilder.NullInputStream.INSTANCE;
				}
				else
				{
					FileDescriptor stderr_fd = new FileDescriptor();
					FdAccess.setHandle(stderr_fd, StdHandles[2]);
					OuterInstance.Stderr_stream = new FileInputStream(stderr_fd);
				}

				return null;
			}
		}

		public override OutputStream OutputStream
		{
			get
			{
				return Stdin_stream;
			}
		}

		public override InputStream InputStream
		{
			get
			{
				return Stdout_stream;
			}
		}

		public override InputStream ErrorStream
		{
			get
			{
				return Stderr_stream;
			}
		}

		~ProcessImpl()
		{
			closeHandle(Handle);
		}

		private static readonly int STILL_ACTIVE = StillActive;
//JAVA TO C# CONVERTER TODO TASK: Replace 'unknown' with the appropriate dll name:
		[DllImport("unknown")]
		private static extern int getStillActive();

		public override int ExitValue()
		{
			int exitCode = getExitCodeProcess(Handle);
			if (exitCode == STILL_ACTIVE)
			{
				throw new IllegalThreadStateException("process has not exited");
			}
			return exitCode;
		}
//JAVA TO C# CONVERTER TODO TASK: Replace 'unknown' with the appropriate dll name:
		[DllImport("unknown")]
		private static extern int getExitCodeProcess(long handle);

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public int waitFor() throws InterruptedException
		public override int WaitFor()
		{
			waitForInterruptibly(Handle);
			if (Thread.Interrupted())
			{
				throw new InterruptedException();
			}
			return ExitValue();
		}

//JAVA TO C# CONVERTER TODO TASK: Replace 'unknown' with the appropriate dll name:
		[DllImport("unknown")]
		private static extern void waitForInterruptibly(long handle);

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: @Override public boolean waitFor(long timeout, java.util.concurrent.TimeUnit unit) throws InterruptedException
		public override bool WaitFor(long timeout, TimeUnit unit)
		{
			if (getExitCodeProcess(Handle) != STILL_ACTIVE)
			{
				return true;
			}
			if (timeout <= 0)
			{
				return false;
			}

			long remainingNanos = unit.ToNanos(timeout);
			long deadline = System.nanoTime() + remainingNanos;

			do
			{
				// Round up to next millisecond
				long msTimeout = TimeUnit.NANOSECONDS.ToMillis(remainingNanos + 999_999L);
				waitForTimeoutInterruptibly(Handle, msTimeout);
				if (Thread.Interrupted())
				{
					throw new InterruptedException();
				}
				if (getExitCodeProcess(Handle) != STILL_ACTIVE)
				{
					return true;
				}
				remainingNanos = deadline - System.nanoTime();
			} while (remainingNanos > 0);

			return (getExitCodeProcess(Handle) != STILL_ACTIVE);
		}

//JAVA TO C# CONVERTER TODO TASK: Replace 'unknown' with the appropriate dll name:
		[DllImport("unknown")]
		private static extern void waitForTimeoutInterruptibly(long handle, long timeout);

		public override void Destroy()
		{
			terminateProcess(Handle);
		}

		public override Process DestroyForcibly()
		{
			Destroy();
			return this;
		}

//JAVA TO C# CONVERTER TODO TASK: Replace 'unknown' with the appropriate dll name:
		[DllImport("unknown")]
		private static extern void terminateProcess(long handle);

		public override bool Alive
		{
			get
			{
				return isProcessAlive(Handle);
			}
		}

//JAVA TO C# CONVERTER TODO TASK: Replace 'unknown' with the appropriate dll name:
		[DllImport("unknown")]
		private static extern boolean isProcessAlive(long handle);

		/// <summary>
		/// Create a process using the win32 function CreateProcess.
		/// The method is synchronized due to MS kb315939 problem.
		/// All native handles should restore the inherit flag at the end of call.
		/// </summary>
		/// <param name="cmdstr"> the Windows command line </param>
		/// <param name="envblock"> NUL-separated, double-NUL-terminated list of
		///        environment strings in VAR=VALUE form </param>
		/// <param name="dir"> the working directory of the process, or null if
		///        inheriting the current directory from the parent process </param>
		/// <param name="stdHandles"> array of windows HANDLEs.  Indexes 0, 1, and
		///        2 correspond to standard input, standard output and
		///        standard error, respectively.  On input, a value of -1
		///        means to create a pipe to connect child and parent
		///        processes.  On output, a value which is not -1 is the
		///        parent pipe handle corresponding to the pipe which has
		///        been created.  An element of this array is -1 on input
		///        if and only if it is <em>not</em> -1 on output. </param>
		/// <param name="redirectErrorStream"> redirectErrorStream attribute </param>
		/// <returns> the native subprocess HANDLE returned by CreateProcess </returns>
//JAVA TO C# CONVERTER TODO TASK: Replace 'unknown' with the appropriate dll name:
		[DllImport("unknown")]
		private static extern long create(String cmdstr, String envblock, String dir, long[] stdHandles, bool redirectErrorStream);

		/// <summary>
		/// Opens a file for atomic append. The file is created if it doesn't
		/// already exist.
		/// </summary>
		/// <param name="file"> the file to open or create </param>
		/// <returns> the native HANDLE </returns>
//JAVA TO C# CONVERTER TODO TASK: Replace 'unknown' with the appropriate dll name:
		[DllImport("unknown")]
		private static extern long openForAtomicAppend(String path);

//JAVA TO C# CONVERTER TODO TASK: Replace 'unknown' with the appropriate dll name:
		[DllImport("unknown")]
		private static extern boolean closeHandle(long handle);
	}

}
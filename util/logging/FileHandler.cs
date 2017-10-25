using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;

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
	/// Simple file logging <tt>Handler</tt>.
	/// <para>
	/// The <tt>FileHandler</tt> can either write to a specified file,
	/// or it can write to a rotating set of files.
	/// </para>
	/// <para>
	/// For a rotating set of files, as each file reaches a given size
	/// limit, it is closed, rotated out, and a new file opened.
	/// Successively older files are named by adding "0", "1", "2",
	/// etc. into the base filename.
	/// </para>
	/// <para>
	/// By default buffering is enabled in the IO libraries but each log
	/// record is flushed out when it is complete.
	/// </para>
	/// <para>
	/// By default the <tt>XMLFormatter</tt> class is used for formatting.
	/// </para>
	/// <para>
	/// <b>Configuration:</b>
	/// By default each <tt>FileHandler</tt> is initialized using the following
	/// <tt>LogManager</tt> configuration properties where <tt>&lt;handler-name&gt;</tt>
	/// refers to the fully-qualified class name of the handler.
	/// If properties are not defined
	/// (or have invalid values) then the specified default values are used.
	/// <ul>
	/// <li>   &lt;handler-name&gt;.level
	///        specifies the default level for the <tt>Handler</tt>
	///        (defaults to <tt>Level.ALL</tt>). </li>
	/// <li>   &lt;handler-name&gt;.filter
	///        specifies the name of a <tt>Filter</tt> class to use
	///        (defaults to no <tt>Filter</tt>). </li>
	/// <li>   &lt;handler-name&gt;.formatter
	///        specifies the name of a <tt>Formatter</tt> class to use
	///        (defaults to <tt>java.util.logging.XMLFormatter</tt>) </li>
	/// <li>   &lt;handler-name&gt;.encoding
	///        the name of the character set encoding to use (defaults to
	///        the default platform encoding). </li>
	/// <li>   &lt;handler-name&gt;.limit
	///        specifies an approximate maximum amount to write (in bytes)
	///        to any one file.  If this is zero, then there is no limit.
	///        (Defaults to no limit). </li>
	/// <li>   &lt;handler-name&gt;.count
	///        specifies how many output files to cycle through (defaults to 1). </li>
	/// <li>   &lt;handler-name&gt;.pattern
	///        specifies a pattern for generating the output file name.  See
	///        below for details. (Defaults to "%h/java%u.log"). </li>
	/// <li>   &lt;handler-name&gt;.append
	///        specifies whether the FileHandler should append onto
	///        any existing files (defaults to false). </li>
	/// </ul>
	/// </para>
	/// <para>
	/// For example, the properties for {@code FileHandler} would be:
	/// <ul>
	/// <li>   java.util.logging.FileHandler.level=INFO </li>
	/// <li>   java.util.logging.FileHandler.formatter=java.util.logging.SimpleFormatter </li>
	/// </ul>
	/// </para>
	/// <para>
	/// For a custom handler, e.g. com.foo.MyHandler, the properties would be:
	/// <ul>
	/// <li>   com.foo.MyHandler.level=INFO </li>
	/// <li>   com.foo.MyHandler.formatter=java.util.logging.SimpleFormatter </li>
	/// </ul>
	/// </para>
	/// <para>
	/// A pattern consists of a string that includes the following special
	/// components that will be replaced at runtime:
	/// <ul>
	/// <li>    "/"    the local pathname separator </li>
	/// <li>     "%t"   the system temporary directory </li>
	/// <li>     "%h"   the value of the "user.home" system property </li>
	/// <li>     "%g"   the generation number to distinguish rotated logs </li>
	/// <li>     "%u"   a unique number to resolve conflicts </li>
	/// <li>     "%%"   translates to a single percent sign "%" </li>
	/// </ul>
	/// If no "%g" field has been specified and the file count is greater
	/// than one, then the generation number will be added to the end of
	/// the generated filename, after a dot.
	/// </para>
	/// <para>
	/// Thus for example a pattern of "%t/java%g.log" with a count of 2
	/// would typically cause log files to be written on Solaris to
	/// /var/tmp/java0.log and /var/tmp/java1.log whereas on Windows 95 they
	/// would be typically written to C:\TEMP\java0.log and C:\TEMP\java1.log
	/// </para>
	/// <para>
	/// Generation numbers follow the sequence 0, 1, 2, etc.
	/// </para>
	/// <para>
	/// Normally the "%u" unique field is set to 0.  However, if the <tt>FileHandler</tt>
	/// tries to open the filename and finds the file is currently in use by
	/// another process it will increment the unique number field and try
	/// again.  This will be repeated until <tt>FileHandler</tt> finds a file name that
	/// is  not currently in use. If there is a conflict and no "%u" field has
	/// been specified, it will be added at the end of the filename after a dot.
	/// (This will be after any automatically added generation number.)
	/// </para>
	/// <para>
	/// Thus if three processes were all trying to log to fred%u.%g.txt then
	/// they  might end up using fred0.0.txt, fred1.0.txt, fred2.0.txt as
	/// the first file in their rotating sequences.
	/// </para>
	/// <para>
	/// Note that the use of unique ids to avoid conflicts is only guaranteed
	/// to work reliably when using a local disk file system.
	/// 
	/// @since 1.4
	/// </para>
	/// </summary>

	public class FileHandler : StreamHandler
	{
		private MeteredStream Meter;
		private bool Append;
		private int Limit; // zero => no limit.
		private int Count;
		private String Pattern;
		private String LockFileName;
		private FileChannel LockFileChannel;
		private File[] Files;
		private const int MAX_LOCKS = 100;
		private static readonly Set<String> Locks = new HashSet<String>();

		/// <summary>
		/// A metered stream is a subclass of OutputStream that
		/// (a) forwards all its output to a target stream
		/// (b) keeps track of how many bytes have been written
		/// </summary>
		private class MeteredStream : OutputStream
		{
			private readonly FileHandler OuterInstance;

			internal readonly OutputStream @out;
			internal int Written;

			internal MeteredStream(FileHandler outerInstance, OutputStream @out, int written)
			{
				this.OuterInstance = outerInstance;
				this.@out = @out;
				this.Written = written;
			}

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: @Override public void write(int b) throws java.io.IOException
			public override void Write(int b)
			{
				@out.Write(b);
				Written++;
			}

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: @Override public void write(byte buff[]) throws java.io.IOException
			public override void Write(sbyte[] buff)
			{
				@out.Write(buff);
				Written += buff.Length;
			}

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: @Override public void write(byte buff[] , int off, int len) throws java.io.IOException
			public override void Write(sbyte[] buff, int off, int len)
			{
				@out.Write(buff,off,len);
				Written += len;
			}

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: @Override public void flush() throws java.io.IOException
			public override void Flush()
			{
				@out.Flush();
			}

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: @Override public void close() throws java.io.IOException
			public override void Close()
			{
				@out.Close();
			}
		}

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: private void open(java.io.File fname, boolean append) throws java.io.IOException
		private void Open(File fname, bool append)
		{
			int len = 0;
			if (append)
			{
				len = (int)fname.Length();
			}
			FileOutputStream fout = new FileOutputStream(fname.ToString(), append);
			BufferedOutputStream bout = new BufferedOutputStream(fout);
			Meter = new MeteredStream(this, bout, len);
			OutputStream = Meter;
		}

		/// <summary>
		/// Configure a FileHandler from LogManager properties and/or default values
		/// as specified in the class javadoc.
		/// </summary>
		private void Configure()
		{
			LogManager manager = LogManager.LogManager;

//JAVA TO C# CONVERTER WARNING: The .NET Type.FullName property will not always yield results identical to the Java Class.getName method:
			String cname = this.GetType().FullName;

			Pattern = manager.GetStringProperty(cname + ".pattern", "%h/java%u.log");
			Limit = manager.GetIntProperty(cname + ".limit", 0);
			if (Limit < 0)
			{
				Limit = 0;
			}
			Count = manager.GetIntProperty(cname + ".count", 1);
			if (Count <= 0)
			{
				Count = 1;
			}
			Append = manager.GetBooleanProperty(cname + ".append", false);
			Level = manager.GetLevelProperty(cname + ".level", Level.ALL);
			Filter = manager.GetFilterProperty(cname + ".filter", null);
			Formatter = manager.GetFormatterProperty(cname + ".formatter", new XMLFormatter());
			try
			{
				Encoding = manager.GetStringProperty(cname + ".encoding", null);
			}
			catch (Exception)
			{
				try
				{
					Encoding = null;
				}
				catch (Exception)
				{
					// doing a setEncoding with null should always work.
					// assert false;
				}
			}
		}


		/// <summary>
		/// Construct a default <tt>FileHandler</tt>.  This will be configured
		/// entirely from <tt>LogManager</tt> properties (or their default values).
		/// <para>
		/// </para>
		/// </summary>
		/// <exception cref="IOException"> if there are IO problems opening the files. </exception>
		/// <exception cref="SecurityException">  if a security manager exists and if
		///             the caller does not have <tt>LoggingPermission("control"))</tt>. </exception>
		/// <exception cref="NullPointerException"> if pattern property is an empty String. </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public FileHandler() throws java.io.IOException, SecurityException
		public FileHandler()
		{
			CheckPermission();
			Configure();
			OpenFiles();
		}

		/// <summary>
		/// Initialize a <tt>FileHandler</tt> to write to the given filename.
		/// <para>
		/// The <tt>FileHandler</tt> is configured based on <tt>LogManager</tt>
		/// properties (or their default values) except that the given pattern
		/// argument is used as the filename pattern, the file limit is
		/// set to no limit, and the file count is set to one.
		/// </para>
		/// <para>
		/// There is no limit on the amount of data that may be written,
		/// so use this with care.
		/// 
		/// </para>
		/// </summary>
		/// <param name="pattern">  the name of the output file </param>
		/// <exception cref="IOException"> if there are IO problems opening the files. </exception>
		/// <exception cref="SecurityException">  if a security manager exists and if
		///             the caller does not have <tt>LoggingPermission("control")</tt>. </exception>
		/// <exception cref="IllegalArgumentException"> if pattern is an empty string </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public FileHandler(String pattern) throws java.io.IOException, SecurityException
		public FileHandler(String pattern)
		{
			if (pattern.Length() < 1)
			{
				throw new IllegalArgumentException();
			}
			CheckPermission();
			Configure();
			this.Pattern = pattern;
			this.Limit = 0;
			this.Count = 1;
			OpenFiles();
		}

		/// <summary>
		/// Initialize a <tt>FileHandler</tt> to write to the given filename,
		/// with optional append.
		/// <para>
		/// The <tt>FileHandler</tt> is configured based on <tt>LogManager</tt>
		/// properties (or their default values) except that the given pattern
		/// argument is used as the filename pattern, the file limit is
		/// set to no limit, the file count is set to one, and the append
		/// mode is set to the given <tt>append</tt> argument.
		/// </para>
		/// <para>
		/// There is no limit on the amount of data that may be written,
		/// so use this with care.
		/// 
		/// </para>
		/// </summary>
		/// <param name="pattern">  the name of the output file </param>
		/// <param name="append">  specifies append mode </param>
		/// <exception cref="IOException"> if there are IO problems opening the files. </exception>
		/// <exception cref="SecurityException">  if a security manager exists and if
		///             the caller does not have <tt>LoggingPermission("control")</tt>. </exception>
		/// <exception cref="IllegalArgumentException"> if pattern is an empty string </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public FileHandler(String pattern, boolean append) throws java.io.IOException, SecurityException
		public FileHandler(String pattern, bool append)
		{
			if (pattern.Length() < 1)
			{
				throw new IllegalArgumentException();
			}
			CheckPermission();
			Configure();
			this.Pattern = pattern;
			this.Limit = 0;
			this.Count = 1;
			this.Append = append;
			OpenFiles();
		}

		/// <summary>
		/// Initialize a <tt>FileHandler</tt> to write to a set of files.  When
		/// (approximately) the given limit has been written to one file,
		/// another file will be opened.  The output will cycle through a set
		/// of count files.
		/// <para>
		/// The <tt>FileHandler</tt> is configured based on <tt>LogManager</tt>
		/// properties (or their default values) except that the given pattern
		/// argument is used as the filename pattern, the file limit is
		/// set to the limit argument, and the file count is set to the
		/// given count argument.
		/// </para>
		/// <para>
		/// The count must be at least 1.
		/// 
		/// </para>
		/// </summary>
		/// <param name="pattern">  the pattern for naming the output file </param>
		/// <param name="limit">  the maximum number of bytes to write to any one file </param>
		/// <param name="count">  the number of files to use </param>
		/// <exception cref="IOException"> if there are IO problems opening the files. </exception>
		/// <exception cref="SecurityException">  if a security manager exists and if
		///             the caller does not have <tt>LoggingPermission("control")</tt>. </exception>
		/// <exception cref="IllegalArgumentException"> if {@code limit < 0}, or {@code count < 1}. </exception>
		/// <exception cref="IllegalArgumentException"> if pattern is an empty string </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public FileHandler(String pattern, int limit, int count) throws java.io.IOException, SecurityException
		public FileHandler(String pattern, int limit, int count)
		{
			if (limit < 0 || count < 1 || pattern.Length() < 1)
			{
				throw new IllegalArgumentException();
			}
			CheckPermission();
			Configure();
			this.Pattern = pattern;
			this.Limit = limit;
			this.Count = count;
			OpenFiles();
		}

		/// <summary>
		/// Initialize a <tt>FileHandler</tt> to write to a set of files
		/// with optional append.  When (approximately) the given limit has
		/// been written to one file, another file will be opened.  The
		/// output will cycle through a set of count files.
		/// <para>
		/// The <tt>FileHandler</tt> is configured based on <tt>LogManager</tt>
		/// properties (or their default values) except that the given pattern
		/// argument is used as the filename pattern, the file limit is
		/// set to the limit argument, and the file count is set to the
		/// given count argument, and the append mode is set to the given
		/// <tt>append</tt> argument.
		/// </para>
		/// <para>
		/// The count must be at least 1.
		/// 
		/// </para>
		/// </summary>
		/// <param name="pattern">  the pattern for naming the output file </param>
		/// <param name="limit">  the maximum number of bytes to write to any one file </param>
		/// <param name="count">  the number of files to use </param>
		/// <param name="append">  specifies append mode </param>
		/// <exception cref="IOException"> if there are IO problems opening the files. </exception>
		/// <exception cref="SecurityException">  if a security manager exists and if
		///             the caller does not have <tt>LoggingPermission("control")</tt>. </exception>
		/// <exception cref="IllegalArgumentException"> if {@code limit < 0}, or {@code count < 1}. </exception>
		/// <exception cref="IllegalArgumentException"> if pattern is an empty string
		///  </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public FileHandler(String pattern, int limit, int count, boolean append) throws java.io.IOException, SecurityException
		public FileHandler(String pattern, int limit, int count, bool append)
		{
			if (limit < 0 || count < 1 || pattern.Length() < 1)
			{
				throw new IllegalArgumentException();
			}
			CheckPermission();
			Configure();
			this.Pattern = pattern;
			this.Limit = limit;
			this.Count = count;
			this.Append = append;
			OpenFiles();
		}

		private bool IsParentWritable(Path path)
		{
			Path parent = path.Parent;
			if (parent == null)
			{
				parent = path.ToAbsolutePath().Parent;
			}
			return parent != null && Files.IsWritable(parent);
		}

		/// <summary>
		/// Open the set of output files, based on the configured
		/// instance variables.
		/// </summary>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: private void openFiles() throws java.io.IOException
		private void OpenFiles()
		{
			LogManager manager = LogManager.LogManager;
			manager.CheckPermission();
			if (Count < 1)
			{
			   throw new IllegalArgumentException("file count = " + Count);
			}
			if (Limit < 0)
			{
				Limit = 0;
			}

			// We register our own ErrorManager during initialization
			// so we can record exceptions.
			InitializationErrorManager em = new InitializationErrorManager();
			ErrorManager = em;

			// Create a lock file.  This grants us exclusive access
			// to our set of output files, as long as we are alive.
			int unique = -1;
			for (;;)
			{
				unique++;
				if (unique > MAX_LOCKS)
				{
					throw new IOException("Couldn't get lock for " + Pattern);
				}
				// Generate a lock file name from the "unique" int.
				LockFileName = Generate(Pattern, 0, unique).ToString() + ".lck";
				// Now try to lock that filename.
				// Because some systems (e.g., Solaris) can only do file locks
				// between processes (and not within a process), we first check
				// if we ourself already have the file locked.
				lock (Locks)
				{
					if (Locks.Contains(LockFileName))
					{
						// We already own this lock, for a different FileHandler
						// object.  Try again.
						continue;
					}

//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final java.nio.file.Path lockFilePath = java.nio.file.Paths.get(lockFileName);
					Path lockFilePath = Paths.Get(LockFileName);
					FileChannel channel = null;
					int retries = -1;
					bool fileCreated = false;
					while (channel == null && retries++ < 1)
					{
						try
						{
							channel = FileChannel.Open(lockFilePath, CREATE_NEW, WRITE);
							fileCreated = true;
						}
						catch (FileAlreadyExistsException)
						{
							// This may be a zombie file left over by a previous
							// execution. Reuse it - but only if we can actually
							// write to its directory.
							// Note that this is a situation that may happen,
							// but not too frequently.
							if (Files.IsRegularFile(lockFilePath, LinkOption.NOFOLLOW_LINKS) && IsParentWritable(lockFilePath))
							{
								try
								{
									channel = FileChannel.Open(lockFilePath, WRITE, APPEND);
								}
								catch (NoSuchFileException)
								{
									// Race condition - retry once, and if that
									// fails again just try the next name in
									// the sequence.
									continue;
								}
								catch (IOException)
								{
									// the file may not be writable for us.
									// try the next name in the sequence
									break;
								}
							}
							else
							{
								// at this point channel should still be null.
								// break and try the next name in the sequence.
								break;
							}
						}
					}

					if (channel == null) // try the next name;
					{
						continue;
					}
					LockFileChannel = channel;

					bool available;
					try
					{
						available = LockFileChannel.TryLock() != null;
						// We got the lock OK.
						// At this point we could call File.deleteOnExit().
						// However, this could have undesirable side effects
						// as indicated by JDK-4872014. So we will instead
						// rely on the fact that close() will remove the lock
						// file and that whoever is creating FileHandlers should
						// be responsible for closing them.
					}
					catch (IOException)
					{
						// We got an IOException while trying to get the lock.
						// This normally indicates that locking is not supported
						// on the target directory.  We have to proceed without
						// getting a lock.   Drop through, but only if we did
						// create the file...
						available = fileCreated;
					}
					catch (OverlappingFileLockException)
					{
						// someone already locked this file in this VM, through
						// some other channel - that is - using something else
						// than new FileHandler(...);
						// continue searching for an available lock.
						available = false;
					}
					if (available)
					{
						// We got the lock.  Remember it.
						Locks.Add(LockFileName);
						break;
					}

					// We failed to get the lock.  Try next file.
					LockFileChannel.Close();
				}
			}

			Files = new File[Count];
			for (int i = 0; i < Count; i++)
			{
				Files[i] = Generate(Pattern, i, unique);
			}

			// Create the initial log file.
			if (Append)
			{
				Open(Files[0], true);
			}
			else
			{
				Rotate();
			}

			// Did we detect any exceptions during initialization?
			Exception ex = em.LastException;
			if (ex != null)
			{
				if (ex is IOException)
				{
					throw (IOException) ex;
				}
				else if (ex is SecurityException)
				{
					throw (SecurityException) ex;
				}
				else
				{
					throw new IOException("Exception: " + ex);
				}
			}

			// Install the normal default ErrorManager.
			ErrorManager = new ErrorManager();
		}

		/// <summary>
		/// Generate a file based on a user-supplied pattern, generation number,
		/// and an integer uniqueness suffix </summary>
		/// <param name="pattern"> the pattern for naming the output file </param>
		/// <param name="generation"> the generation number to distinguish rotated logs </param>
		/// <param name="unique"> a unique number to resolve conflicts </param>
		/// <returns> the generated File </returns>
		/// <exception cref="IOException"> </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: private java.io.File generate(String pattern, int generation, int unique) throws java.io.IOException
		private File Generate(String pattern, int generation, int unique)
		{
			File file = null;
			String word = "";
			int ix = 0;
			bool sawg = false;
			bool sawu = false;
			while (ix < pattern.Length())
			{
				char ch = pattern.CharAt(ix);
				ix++;
				char ch2 = (char)0;
				if (ix < pattern.Length())
				{
					ch2 = char.ToLower(pattern.CharAt(ix));
				}
				if (ch == '/')
				{
					if (file == null)
					{
						file = new File(word);
					}
					else
					{
						file = new File(file, word);
					}
					word = "";
					continue;
				}
				else if (ch == '%')
				{
					if (ch2 == 't')
					{
						String tmpDir = System.getProperty("java.io.tmpdir");
						if (tmpDir == null)
						{
							tmpDir = System.getProperty("user.home");
						}
						file = new File(tmpDir);
						ix++;
						word = "";
						continue;
					}
					else if (ch2 == 'h')
					{
						file = new File(System.getProperty("user.home"));
						if (SetUID)
						{
							// Ok, we are in a set UID program.  For safety's sake
							// we disallow attempts to open files relative to %h.
							throw new IOException("can't use %h in set UID program");
						}
						ix++;
						word = "";
						continue;
					}
					else if (ch2 == 'g')
					{
						word = word + generation;
						sawg = true;
						ix++;
						continue;
					}
					else if (ch2 == 'u')
					{
						word = word + unique;
						sawu = true;
						ix++;
						continue;
					}
					else if (ch2 == '%')
					{
						word = word + "%";
						ix++;
						continue;
					}
				}
				word = word + ch;
			}
			if (Count > 1 && !sawg)
			{
				word = word + "." + generation;
			}
			if (unique > 0 && !sawu)
			{
				word = word + "." + unique;
			}
			if (word.Length() > 0)
			{
				if (file == null)
				{
					file = new File(word);
				}
				else
				{
					file = new File(file, word);
				}
			}
			return file;
		}

		/// <summary>
		/// Rotate the set of output files
		/// </summary>
		private void Rotate()
		{
			lock (this)
			{
				Level oldLevel = Level;
				Level = Level.OFF;
        
				base.Close();
				for (int i = Count - 2; i >= 0; i--)
				{
					File f1 = Files[i];
					File f2 = Files[i + 1];
					if (f1.Exists())
					{
						if (f2.Exists())
						{
							f2.Delete();
						}
						f1.RenameTo(f2);
					}
				}
				try
				{
					Open(Files[0], false);
				}
				catch (IOException ix)
				{
					// We don't want to throw an exception here, but we
					// report the exception to any registered ErrorManager.
					ReportError(null, ix, ErrorManager.OPEN_FAILURE);
        
				}
				Level = oldLevel;
			}
		}

		/// <summary>
		/// Format and publish a <tt>LogRecord</tt>.
		/// </summary>
		/// <param name="record">  description of the log event. A null record is
		///                 silently ignored and is not published </param>
		public override void Publish(LogRecord record)
		{
			lock (this)
			{
				if (!IsLoggable(record))
				{
					return;
				}
				base.Publish(record);
				Flush();
				if (Limit > 0 && Meter.Written >= Limit)
				{
					// We performed access checks in the "init" method to make sure
					// we are only initialized from trusted code.  So we assume
					// it is OK to write the target files, even if we are
					// currently being called from untrusted code.
					// So it is safe to raise privilege here.
					AccessController.doPrivileged(new PrivilegedActionAnonymousInnerClassHelper(this));
				}
			}
		}

		private class PrivilegedActionAnonymousInnerClassHelper : PrivilegedAction<Object>
		{
			private readonly FileHandler OuterInstance;

			public PrivilegedActionAnonymousInnerClassHelper(FileHandler outerInstance)
			{
				this.OuterInstance = outerInstance;
			}

			public virtual Object Run()
			{
				outerInstance.Rotate();
				return null;
			}
		}

		/// <summary>
		/// Close all the files.
		/// </summary>
		/// <exception cref="SecurityException">  if a security manager exists and if
		///             the caller does not have <tt>LoggingPermission("control")</tt>. </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: @Override public synchronized void close() throws SecurityException
		public override void Close()
		{
			lock (this)
			{
				base.Close();
				// Unlock any lock file.
				if (LockFileName == null)
				{
					return;
				}
				try
				{
					// Close the lock file channel (which also will free any locks)
					LockFileChannel.Close();
				}
				catch (Exception)
				{
					// Problems closing the stream.  Punt.
				}
				lock (Locks)
				{
					Locks.Remove(LockFileName);
				}
				if (System.IO.Directory.Exists(LockFileName)) System.IO.Directory.Delete(LockFileName, true); else System.IO.File.Delete(LockFileName);
				LockFileName = null;
				LockFileChannel = null;
			}
		}

		private class InitializationErrorManager : ErrorManager
		{
			internal Exception LastException;
			public override void Error(String msg, Exception ex, int code)
			{
				LastException = ex;
			}
		}

		/// <summary>
		/// check if we are in a set UID program.
		/// </summary>
//JAVA TO C# CONVERTER TODO TASK: Replace 'unknown' with the appropriate dll name:
		[DllImport("unknown")]
		private static extern boolean isSetUID();
	}

}
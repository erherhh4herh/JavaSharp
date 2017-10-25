using System;
using System.Diagnostics;
using System.Collections.Generic;

/*
 * Copyright (c) 2007, 2013, Oracle and/or its affiliates. All rights reserved.
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

namespace java.nio.file
{


	/// <summary>
	/// This class consists exclusively of static methods that operate on files,
	/// directories, or other types of files.
	/// 
	/// <para> In most cases, the methods defined here will delegate to the associated
	/// file system provider to perform the file operations.
	/// 
	/// @since 1.7
	/// </para>
	/// </summary>

	public sealed class Files
	{
		private Files()
		{
		}

		/// <summary>
		/// Returns the {@code FileSystemProvider} to delegate to.
		/// </summary>
		private static FileSystemProvider Provider(Path path)
		{
			return path.FileSystem.Provider();
		}

		/// <summary>
		/// Convert a Closeable to a Runnable by converting checked IOException
		/// to UncheckedIOException
		/// </summary>
		private static Runnable AsUncheckedRunnable(Closeable c)
		{
//JAVA TO C# CONVERTER TODO TASK: Java lambdas satisfy functional interfaces, while .NET lambdas satisfy delegates - change the appropriate interface to a delegate:
			return () =>
			{
				try
				{
					c.Close();
				}
				catch (IOException e)
				{
					throw new UncheckedIOException(e);
				}
			};
		}

		// -- File contents --

		/// <summary>
		/// Opens a file, returning an input stream to read from the file. The stream
		/// will not be buffered, and is not required to support the {@link
		/// InputStream#mark mark} or <seealso cref="InputStream#reset reset"/> methods. The
		/// stream will be safe for access by multiple concurrent threads. Reading
		/// commences at the beginning of the file. Whether the returned stream is
		/// <i>asynchronously closeable</i> and/or <i>interruptible</i> is highly
		/// file system provider specific and therefore not specified.
		/// 
		/// <para> The {@code options} parameter determines how the file is opened.
		/// If no options are present then it is equivalent to opening the file with
		/// the <seealso cref="StandardOpenOption#READ READ"/> option. In addition to the {@code
		/// READ} option, an implementation may also support additional implementation
		/// specific options.
		/// 
		/// </para>
		/// </summary>
		/// <param name="path">
		///          the path to the file to open </param>
		/// <param name="options">
		///          options specifying how the file is opened
		/// </param>
		/// <returns>  a new input stream
		/// </returns>
		/// <exception cref="IllegalArgumentException">
		///          if an invalid combination of options is specified </exception>
		/// <exception cref="UnsupportedOperationException">
		///          if an unsupported option is specified </exception>
		/// <exception cref="IOException">
		///          if an I/O error occurs </exception>
		/// <exception cref="SecurityException">
		///          In the case of the default provider, and a security manager is
		///          installed, the <seealso cref="SecurityManager#checkRead(String) checkRead"/>
		///          method is invoked to check read access to the file. </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public static java.io.InputStream newInputStream(Path path, OpenOption... options) throws java.io.IOException
		public static InputStream NewInputStream(Path path, params OpenOption[] options)
		{
			return Provider(path).NewInputStream(path, options);
		}

		/// <summary>
		/// Opens or creates a file, returning an output stream that may be used to
		/// write bytes to the file. The resulting stream will not be buffered. The
		/// stream will be safe for access by multiple concurrent threads. Whether
		/// the returned stream is <i>asynchronously closeable</i> and/or
		/// <i>interruptible</i> is highly file system provider specific and
		/// therefore not specified.
		/// 
		/// <para> This method opens or creates a file in exactly the manner specified
		/// by the <seealso cref="#newByteChannel(Path,Set,FileAttribute[]) newByteChannel"/>
		/// method with the exception that the <seealso cref="StandardOpenOption#READ READ"/>
		/// option may not be present in the array of options. If no options are
		/// present then this method works as if the {@link StandardOpenOption#CREATE
		/// CREATE}, <seealso cref="StandardOpenOption#TRUNCATE_EXISTING TRUNCATE_EXISTING"/>,
		/// and <seealso cref="StandardOpenOption#WRITE WRITE"/> options are present. In other
		/// words, it opens the file for writing, creating the file if it doesn't
		/// exist, or initially truncating an existing {@link #isRegularFile
		/// regular-file} to a size of {@code 0} if it exists.
		/// 
		/// </para>
		/// <para> <b>Usage Examples:</b>
		/// <pre>
		///     Path path = ...
		/// 
		///     // truncate and overwrite an existing file, or create the file if
		///     // it doesn't initially exist
		///     OutputStream out = Files.newOutputStream(path);
		/// 
		///     // append to an existing file, fail if the file does not exist
		///     out = Files.newOutputStream(path, APPEND);
		/// 
		///     // append to an existing file, create file if it doesn't initially exist
		///     out = Files.newOutputStream(path, CREATE, APPEND);
		/// 
		///     // always create new file, failing if it already exists
		///     out = Files.newOutputStream(path, CREATE_NEW);
		/// </pre>
		/// 
		/// </para>
		/// </summary>
		/// <param name="path">
		///          the path to the file to open or create </param>
		/// <param name="options">
		///          options specifying how the file is opened
		/// </param>
		/// <returns>  a new output stream
		/// </returns>
		/// <exception cref="IllegalArgumentException">
		///          if {@code options} contains an invalid combination of options </exception>
		/// <exception cref="UnsupportedOperationException">
		///          if an unsupported option is specified </exception>
		/// <exception cref="IOException">
		///          if an I/O error occurs </exception>
		/// <exception cref="SecurityException">
		///          In the case of the default provider, and a security manager is
		///          installed, the <seealso cref="SecurityManager#checkWrite(String) checkWrite"/>
		///          method is invoked to check write access to the file. The {@link
		///          SecurityManager#checkDelete(String) checkDelete} method is
		///          invoked to check delete access if the file is opened with the
		///          {@code DELETE_ON_CLOSE} option. </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public static java.io.OutputStream newOutputStream(Path path, OpenOption... options) throws java.io.IOException
		public static OutputStream NewOutputStream(Path path, params OpenOption[] options)
		{
			return Provider(path).NewOutputStream(path, options);
		}

		/// <summary>
		/// Opens or creates a file, returning a seekable byte channel to access the
		/// file.
		/// 
		/// <para> The {@code options} parameter determines how the file is opened.
		/// The <seealso cref="StandardOpenOption#READ READ"/> and {@link
		/// StandardOpenOption#WRITE WRITE} options determine if the file should be
		/// opened for reading and/or writing. If neither option (or the {@link
		/// StandardOpenOption#APPEND APPEND} option) is present then the file is
		/// opened for reading. By default reading or writing commence at the
		/// beginning of the file.
		/// 
		/// </para>
		/// <para> In the addition to {@code READ} and {@code WRITE}, the following
		/// options may be present:
		/// 
		/// <table border=1 cellpadding=5 summary="Options">
		/// <tr> <th>Option</th> <th>Description</th> </tr>
		/// <tr>
		///   <td> <seealso cref="StandardOpenOption#APPEND APPEND"/> </td>
		///   <td> If this option is present then the file is opened for writing and
		///     each invocation of the channel's {@code write} method first advances
		///     the position to the end of the file and then writes the requested
		///     data. Whether the advancement of the position and the writing of the
		///     data are done in a single atomic operation is system-dependent and
		///     therefore unspecified. This option may not be used in conjunction
		///     with the {@code READ} or {@code TRUNCATE_EXISTING} options. </td>
		/// </tr>
		/// <tr>
		///   <td> <seealso cref="StandardOpenOption#TRUNCATE_EXISTING TRUNCATE_EXISTING"/> </td>
		///   <td> If this option is present then the existing file is truncated to
		///   a size of 0 bytes. This option is ignored when the file is opened only
		///   for reading. </td>
		/// </tr>
		/// <tr>
		///   <td> <seealso cref="StandardOpenOption#CREATE_NEW CREATE_NEW"/> </td>
		///   <td> If this option is present then a new file is created, failing if
		///   the file already exists or is a symbolic link. When creating a file the
		///   check for the existence of the file and the creation of the file if it
		///   does not exist is atomic with respect to other file system operations.
		///   This option is ignored when the file is opened only for reading. </td>
		/// </tr>
		/// <tr>
		///   <td > <seealso cref="StandardOpenOption#CREATE CREATE"/> </td>
		///   <td> If this option is present then an existing file is opened if it
		///   exists, otherwise a new file is created. This option is ignored if the
		///   {@code CREATE_NEW} option is also present or the file is opened only
		///   for reading. </td>
		/// </tr>
		/// <tr>
		///   <td > <seealso cref="StandardOpenOption#DELETE_ON_CLOSE DELETE_ON_CLOSE"/> </td>
		///   <td> When this option is present then the implementation makes a
		///   <em>best effort</em> attempt to delete the file when closed by the
		///   <seealso cref="SeekableByteChannel#close close"/> method. If the {@code close}
		///   method is not invoked then a <em>best effort</em> attempt is made to
		///   delete the file when the Java virtual machine terminates. </td>
		/// </tr>
		/// <tr>
		///   <td><seealso cref="StandardOpenOption#SPARSE SPARSE"/> </td>
		///   <td> When creating a new file this option is a <em>hint</em> that the
		///   new file will be sparse. This option is ignored when not creating
		///   a new file. </td>
		/// </tr>
		/// <tr>
		///   <td> <seealso cref="StandardOpenOption#SYNC SYNC"/> </td>
		///   <td> Requires that every update to the file's content or metadata be
		///   written synchronously to the underlying storage device. (see <a
		///   href="package-summary.html#integrity"> Synchronized I/O file
		///   integrity</a>). </td>
		/// </tr>
		/// <tr>
		///   <td> <seealso cref="StandardOpenOption#DSYNC DSYNC"/> </td>
		///   <td> Requires that every update to the file's content be written
		///   synchronously to the underlying storage device. (see <a
		///   href="package-summary.html#integrity"> Synchronized I/O file
		///   integrity</a>). </td>
		/// </tr>
		/// </table>
		/// 
		/// </para>
		/// <para> An implementation may also support additional implementation specific
		/// options.
		/// 
		/// </para>
		/// <para> The {@code attrs} parameter is optional {@link FileAttribute
		/// file-attributes} to set atomically when a new file is created.
		/// 
		/// </para>
		/// <para> In the case of the default provider, the returned seekable byte channel
		/// is a <seealso cref="java.nio.channels.FileChannel"/>.
		/// 
		/// </para>
		/// <para> <b>Usage Examples:</b>
		/// <pre>
		///     Path path = ...
		/// 
		///     // open file for reading
		///     ReadableByteChannel rbc = Files.newByteChannel(path, EnumSet.of(READ)));
		/// 
		///     // open file for writing to the end of an existing file, creating
		///     // the file if it doesn't already exist
		///     WritableByteChannel wbc = Files.newByteChannel(path, EnumSet.of(CREATE,APPEND));
		/// 
		///     // create file with initial permissions, opening it for both reading and writing
		///     {@code FileAttribute<Set<PosixFilePermission>> perms = ...}
		///     SeekableByteChannel sbc = Files.newByteChannel(path, EnumSet.of(CREATE_NEW,READ,WRITE), perms);
		/// </pre>
		/// 
		/// </para>
		/// </summary>
		/// <param name="path">
		///          the path to the file to open or create </param>
		/// <param name="options">
		///          options specifying how the file is opened </param>
		/// <param name="attrs">
		///          an optional list of file attributes to set atomically when
		///          creating the file
		/// </param>
		/// <returns>  a new seekable byte channel
		/// </returns>
		/// <exception cref="IllegalArgumentException">
		///          if the set contains an invalid combination of options </exception>
		/// <exception cref="UnsupportedOperationException">
		///          if an unsupported open option is specified or the array contains
		///          attributes that cannot be set atomically when creating the file </exception>
		/// <exception cref="FileAlreadyExistsException">
		///          if a file of that name already exists and the {@link
		///          StandardOpenOption#CREATE_NEW CREATE_NEW} option is specified
		///          <i>(optional specific exception)</i> </exception>
		/// <exception cref="IOException">
		///          if an I/O error occurs </exception>
		/// <exception cref="SecurityException">
		///          In the case of the default provider, and a security manager is
		///          installed, the <seealso cref="SecurityManager#checkRead(String) checkRead"/>
		///          method is invoked to check read access to the path if the file is
		///          opened for reading. The {@link SecurityManager#checkWrite(String)
		///          checkWrite} method is invoked to check write access to the path
		///          if the file is opened for writing. The {@link
		///          SecurityManager#checkDelete(String) checkDelete} method is
		///          invoked to check delete access if the file is opened with the
		///          {@code DELETE_ON_CLOSE} option.
		/// </exception>
		/// <seealso cref= java.nio.channels.FileChannel#open(Path,Set,FileAttribute[]) </seealso>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public static java.nio.channels.SeekableByteChannel newByteChannel(Path path, java.util.Set<? extends OpenOption> options, java.nio.file.attribute.FileAttribute<?>... attrs) throws java.io.IOException
//JAVA TO C# CONVERTER TODO TASK: Java wildcard generics are not converted to .NET:
		public static SeekableByteChannel newByteChannel<T1>(Path path, Set<T1> options, params FileAttribute<?>[] attrs) where T1 : OpenOption
		{
			return Provider(path).NewByteChannel(path, options, attrs);
		}

		/// <summary>
		/// Opens or creates a file, returning a seekable byte channel to access the
		/// file.
		/// 
		/// <para> This method opens or creates a file in exactly the manner specified
		/// by the <seealso cref="#newByteChannel(Path,Set,FileAttribute[]) newByteChannel"/>
		/// method.
		/// 
		/// </para>
		/// </summary>
		/// <param name="path">
		///          the path to the file to open or create </param>
		/// <param name="options">
		///          options specifying how the file is opened
		/// </param>
		/// <returns>  a new seekable byte channel
		/// </returns>
		/// <exception cref="IllegalArgumentException">
		///          if the set contains an invalid combination of options </exception>
		/// <exception cref="UnsupportedOperationException">
		///          if an unsupported open option is specified </exception>
		/// <exception cref="FileAlreadyExistsException">
		///          if a file of that name already exists and the {@link
		///          StandardOpenOption#CREATE_NEW CREATE_NEW} option is specified
		///          <i>(optional specific exception)</i> </exception>
		/// <exception cref="IOException">
		///          if an I/O error occurs </exception>
		/// <exception cref="SecurityException">
		///          In the case of the default provider, and a security manager is
		///          installed, the <seealso cref="SecurityManager#checkRead(String) checkRead"/>
		///          method is invoked to check read access to the path if the file is
		///          opened for reading. The {@link SecurityManager#checkWrite(String)
		///          checkWrite} method is invoked to check write access to the path
		///          if the file is opened for writing. The {@link
		///          SecurityManager#checkDelete(String) checkDelete} method is
		///          invoked to check delete access if the file is opened with the
		///          {@code DELETE_ON_CLOSE} option.
		/// </exception>
		/// <seealso cref= java.nio.channels.FileChannel#open(Path,OpenOption[]) </seealso>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public static java.nio.channels.SeekableByteChannel newByteChannel(Path path, OpenOption... options) throws java.io.IOException
		public static SeekableByteChannel NewByteChannel(Path path, params OpenOption[] options)
		{
			Set<OpenOption> set = new HashSet<OpenOption>(options.Length);
			Collections.AddAll(set, options);
			return NewByteChannel(path, set);
		}

		// -- Directories --

		private class AcceptAllFilter : DirectoryStream_Filter<Path>
		{
			internal AcceptAllFilter()
			{
			}

			public override bool Accept(Path entry)
			{
				return true;
			}

			internal static readonly AcceptAllFilter FILTER = new AcceptAllFilter();
		}

		/// <summary>
		/// Opens a directory, returning a <seealso cref="DirectoryStream"/> to iterate over
		/// all entries in the directory. The elements returned by the directory
		/// stream's <seealso cref="DirectoryStream#iterator iterator"/> are of type {@code
		/// Path}, each one representing an entry in the directory. The {@code Path}
		/// objects are obtained as if by <seealso cref="Path#resolve(Path) resolving"/> the
		/// name of the directory entry against {@code dir}.
		/// 
		/// <para> When not using the try-with-resources construct, then directory
		/// stream's {@code close} method should be invoked after iteration is
		/// completed so as to free any resources held for the open directory.
		/// 
		/// </para>
		/// <para> When an implementation supports operations on entries in the
		/// directory that execute in a race-free manner then the returned directory
		/// stream is a <seealso cref="SecureDirectoryStream"/>.
		/// 
		/// </para>
		/// </summary>
		/// <param name="dir">
		///          the path to the directory
		/// </param>
		/// <returns>  a new and open {@code DirectoryStream} object
		/// </returns>
		/// <exception cref="NotDirectoryException">
		///          if the file could not otherwise be opened because it is not
		///          a directory <i>(optional specific exception)</i> </exception>
		/// <exception cref="IOException">
		///          if an I/O error occurs </exception>
		/// <exception cref="SecurityException">
		///          In the case of the default provider, and a security manager is
		///          installed, the <seealso cref="SecurityManager#checkRead(String) checkRead"/>
		///          method is invoked to check read access to the directory. </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public static DirectoryStream<Path> newDirectoryStream(Path dir) throws java.io.IOException
		public static DirectoryStream<Path> NewDirectoryStream(Path dir)
		{
			return Provider(dir).NewDirectoryStream(dir, AcceptAllFilter.FILTER);
		}

		/// <summary>
		/// Opens a directory, returning a <seealso cref="DirectoryStream"/> to iterate over
		/// the entries in the directory. The elements returned by the directory
		/// stream's <seealso cref="DirectoryStream#iterator iterator"/> are of type {@code
		/// Path}, each one representing an entry in the directory. The {@code Path}
		/// objects are obtained as if by <seealso cref="Path#resolve(Path) resolving"/> the
		/// name of the directory entry against {@code dir}. The entries returned by
		/// the iterator are filtered by matching the {@code String} representation
		/// of their file names against the given <em>globbing</em> pattern.
		/// 
		/// <para> For example, suppose we want to iterate over the files ending with
		/// ".java" in a directory:
		/// <pre>
		///     Path dir = ...
		///     try (DirectoryStream&lt;Path&gt; stream = Files.newDirectoryStream(dir, "*.java")) {
		///         :
		///     }
		/// </pre>
		/// 
		/// </para>
		/// <para> The globbing pattern is specified by the {@link
		/// FileSystem#getPathMatcher getPathMatcher} method.
		/// 
		/// </para>
		/// <para> When not using the try-with-resources construct, then directory
		/// stream's {@code close} method should be invoked after iteration is
		/// completed so as to free any resources held for the open directory.
		/// 
		/// </para>
		/// <para> When an implementation supports operations on entries in the
		/// directory that execute in a race-free manner then the returned directory
		/// stream is a <seealso cref="SecureDirectoryStream"/>.
		/// 
		/// </para>
		/// </summary>
		/// <param name="dir">
		///          the path to the directory </param>
		/// <param name="glob">
		///          the glob pattern
		/// </param>
		/// <returns>  a new and open {@code DirectoryStream} object
		/// </returns>
		/// <exception cref="java.util.regex.PatternSyntaxException">
		///          if the pattern is invalid </exception>
		/// <exception cref="NotDirectoryException">
		///          if the file could not otherwise be opened because it is not
		///          a directory <i>(optional specific exception)</i> </exception>
		/// <exception cref="IOException">
		///          if an I/O error occurs </exception>
		/// <exception cref="SecurityException">
		///          In the case of the default provider, and a security manager is
		///          installed, the <seealso cref="SecurityManager#checkRead(String) checkRead"/>
		///          method is invoked to check read access to the directory. </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public static DirectoryStream<Path> newDirectoryStream(Path dir, String glob) throws java.io.IOException
		public static DirectoryStream<Path> NewDirectoryStream(Path dir, String glob)
		{
			// avoid creating a matcher if all entries are required.
			if (glob.Equals("*"))
			{
				return NewDirectoryStream(dir);
			}

			// create a matcher and return a filter that uses it.
			FileSystem fs = dir.FileSystem;
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final PathMatcher matcher = fs.getPathMatcher("glob:" + glob);
			PathMatcher matcher = fs.GetPathMatcher("glob:" + glob);
			DirectoryStream_Filter<Path> filter = new DirectoryStream_FilterAnonymousInnerClassHelper(matcher);
			return fs.Provider().NewDirectoryStream(dir, filter);
		}

		private class DirectoryStream_FilterAnonymousInnerClassHelper : DirectoryStream_Filter<Path>
		{
			private java.nio.file.PathMatcher Matcher;

			public DirectoryStream_FilterAnonymousInnerClassHelper(java.nio.file.PathMatcher matcher)
			{
				this.Matcher = matcher;
			}

			public virtual bool Accept(Path entry)
			{
				return Matcher.Matches(entry.FileName);
			}
		}

		/// <summary>
		/// Opens a directory, returning a <seealso cref="DirectoryStream"/> to iterate over
		/// the entries in the directory. The elements returned by the directory
		/// stream's <seealso cref="DirectoryStream#iterator iterator"/> are of type {@code
		/// Path}, each one representing an entry in the directory. The {@code Path}
		/// objects are obtained as if by <seealso cref="Path#resolve(Path) resolving"/> the
		/// name of the directory entry against {@code dir}. The entries returned by
		/// the iterator are filtered by the given {@link DirectoryStream.Filter
		/// filter}.
		/// 
		/// <para> When not using the try-with-resources construct, then directory
		/// stream's {@code close} method should be invoked after iteration is
		/// completed so as to free any resources held for the open directory.
		/// 
		/// </para>
		/// <para> Where the filter terminates due to an uncaught error or runtime
		/// exception then it is propagated to the {@link Iterator#hasNext()
		/// hasNext} or <seealso cref="Iterator#next() next"/> method. Where an {@code
		/// IOException} is thrown, it results in the {@code hasNext} or {@code
		/// next} method throwing a <seealso cref="DirectoryIteratorException"/> with the
		/// {@code IOException} as the cause.
		/// 
		/// </para>
		/// <para> When an implementation supports operations on entries in the
		/// directory that execute in a race-free manner then the returned directory
		/// stream is a <seealso cref="SecureDirectoryStream"/>.
		/// 
		/// </para>
		/// <para> <b>Usage Example:</b>
		/// Suppose we want to iterate over the files in a directory that are
		/// larger than 8K.
		/// <pre>
		///     DirectoryStream.Filter&lt;Path&gt; filter = new DirectoryStream.Filter&lt;Path&gt;() {
		///         public boolean accept(Path file) throws IOException {
		///             return (Files.size(file) &gt; 8192L);
		///         }
		///     };
		///     Path dir = ...
		///     try (DirectoryStream&lt;Path&gt; stream = Files.newDirectoryStream(dir, filter)) {
		///         :
		///     }
		/// </pre>
		/// 
		/// </para>
		/// </summary>
		/// <param name="dir">
		///          the path to the directory </param>
		/// <param name="filter">
		///          the directory stream filter
		/// </param>
		/// <returns>  a new and open {@code DirectoryStream} object
		/// </returns>
		/// <exception cref="NotDirectoryException">
		///          if the file could not otherwise be opened because it is not
		///          a directory <i>(optional specific exception)</i> </exception>
		/// <exception cref="IOException">
		///          if an I/O error occurs </exception>
		/// <exception cref="SecurityException">
		///          In the case of the default provider, and a security manager is
		///          installed, the <seealso cref="SecurityManager#checkRead(String) checkRead"/>
		///          method is invoked to check read access to the directory. </exception>
//JAVA TO C# CONVERTER TODO TASK: There is no .NET equivalent to the Java 'super' constraint:
//ORIGINAL LINE: public static DirectoryStream<Path> newDirectoryStream(Path dir, DirectoryStream_Filter<? base Path> filter) throws java.io.IOException
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
		public static DirectoryStream<Path> NewDirectoryStream(Path dir, DirectoryStream_Filter<T1> filter)
		{
			return Provider(dir).NewDirectoryStream(dir, filter);
		}

		// -- Creation and deletion --

		/// <summary>
		/// Creates a new and empty file, failing if the file already exists. The
		/// check for the existence of the file and the creation of the new file if
		/// it does not exist are a single operation that is atomic with respect to
		/// all other filesystem activities that might affect the directory.
		/// 
		/// <para> The {@code attrs} parameter is optional {@link FileAttribute
		/// file-attributes} to set atomically when creating the file. Each attribute
		/// is identified by its <seealso cref="FileAttribute#name name"/>. If more than one
		/// attribute of the same name is included in the array then all but the last
		/// occurrence is ignored.
		/// 
		/// </para>
		/// </summary>
		/// <param name="path">
		///          the path to the file to create </param>
		/// <param name="attrs">
		///          an optional list of file attributes to set atomically when
		///          creating the file
		/// </param>
		/// <returns>  the file
		/// </returns>
		/// <exception cref="UnsupportedOperationException">
		///          if the array contains an attribute that cannot be set atomically
		///          when creating the file </exception>
		/// <exception cref="FileAlreadyExistsException">
		///          if a file of that name already exists
		///          <i>(optional specific exception)</i> </exception>
		/// <exception cref="IOException">
		///          if an I/O error occurs or the parent directory does not exist </exception>
		/// <exception cref="SecurityException">
		///          In the case of the default provider, and a security manager is
		///          installed, the <seealso cref="SecurityManager#checkWrite(String) checkWrite"/>
		///          method is invoked to check write access to the new file. </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public static Path createFile(Path path, java.nio.file.attribute.FileAttribute<?>... attrs) throws java.io.IOException
//JAVA TO C# CONVERTER TODO TASK: Java wildcard generics are not converted to .NET:
		public static Path CreateFile(Path path, params FileAttribute<?>[] attrs)
		{
			EnumSet<StandardOpenOption> options = EnumSet.Of<StandardOpenOption>(StandardOpenOption.CREATE_NEW, StandardOpenOption.WRITE);
			NewByteChannel(path, options, attrs).Close();
			return path;
		}

		/// <summary>
		/// Creates a new directory. The check for the existence of the file and the
		/// creation of the directory if it does not exist are a single operation
		/// that is atomic with respect to all other filesystem activities that might
		/// affect the directory. The <seealso cref="#createDirectories createDirectories"/>
		/// method should be used where it is required to create all nonexistent
		/// parent directories first.
		/// 
		/// <para> The {@code attrs} parameter is optional {@link FileAttribute
		/// file-attributes} to set atomically when creating the directory. Each
		/// attribute is identified by its <seealso cref="FileAttribute#name name"/>. If more
		/// than one attribute of the same name is included in the array then all but
		/// the last occurrence is ignored.
		/// 
		/// </para>
		/// </summary>
		/// <param name="dir">
		///          the directory to create </param>
		/// <param name="attrs">
		///          an optional list of file attributes to set atomically when
		///          creating the directory
		/// </param>
		/// <returns>  the directory
		/// </returns>
		/// <exception cref="UnsupportedOperationException">
		///          if the array contains an attribute that cannot be set atomically
		///          when creating the directory </exception>
		/// <exception cref="FileAlreadyExistsException">
		///          if a directory could not otherwise be created because a file of
		///          that name already exists <i>(optional specific exception)</i> </exception>
		/// <exception cref="IOException">
		///          if an I/O error occurs or the parent directory does not exist </exception>
		/// <exception cref="SecurityException">
		///          In the case of the default provider, and a security manager is
		///          installed, the <seealso cref="SecurityManager#checkWrite(String) checkWrite"/>
		///          method is invoked to check write access to the new directory. </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public static Path createDirectory(Path dir, java.nio.file.attribute.FileAttribute<?>... attrs) throws java.io.IOException
//JAVA TO C# CONVERTER TODO TASK: Java wildcard generics are not converted to .NET:
		public static Path CreateDirectory(Path dir, params FileAttribute<?>[] attrs)
		{
			Provider(dir).CreateDirectory(dir, attrs);
			return dir;
		}

		/// <summary>
		/// Creates a directory by creating all nonexistent parent directories first.
		/// Unlike the <seealso cref="#createDirectory createDirectory"/> method, an exception
		/// is not thrown if the directory could not be created because it already
		/// exists.
		/// 
		/// <para> The {@code attrs} parameter is optional {@link FileAttribute
		/// file-attributes} to set atomically when creating the nonexistent
		/// directories. Each file attribute is identified by its {@link
		/// FileAttribute#name name}. If more than one attribute of the same name is
		/// included in the array then all but the last occurrence is ignored.
		/// 
		/// </para>
		/// <para> If this method fails, then it may do so after creating some, but not
		/// all, of the parent directories.
		/// 
		/// </para>
		/// </summary>
		/// <param name="dir">
		///          the directory to create
		/// </param>
		/// <param name="attrs">
		///          an optional list of file attributes to set atomically when
		///          creating the directory
		/// </param>
		/// <returns>  the directory
		/// </returns>
		/// <exception cref="UnsupportedOperationException">
		///          if the array contains an attribute that cannot be set atomically
		///          when creating the directory </exception>
		/// <exception cref="FileAlreadyExistsException">
		///          if {@code dir} exists but is not a directory <i>(optional specific
		///          exception)</i> </exception>
		/// <exception cref="IOException">
		///          if an I/O error occurs </exception>
		/// <exception cref="SecurityException">
		///          in the case of the default provider, and a security manager is
		///          installed, the <seealso cref="SecurityManager#checkWrite(String) checkWrite"/>
		///          method is invoked prior to attempting to create a directory and
		///          its <seealso cref="SecurityManager#checkRead(String) checkRead"/> is
		///          invoked for each parent directory that is checked. If {@code
		///          dir} is not an absolute path then its {@link Path#toAbsolutePath
		///          toAbsolutePath} may need to be invoked to get its absolute path.
		///          This may invoke the security manager's {@link
		///          SecurityManager#checkPropertyAccess(String) checkPropertyAccess}
		///          method to check access to the system property {@code user.dir} </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public static Path createDirectories(Path dir, java.nio.file.attribute.FileAttribute<?>... attrs) throws java.io.IOException
//JAVA TO C# CONVERTER TODO TASK: Java wildcard generics are not converted to .NET:
		public static Path CreateDirectories(Path dir, params FileAttribute<?>[] attrs)
		{
			// attempt to create the directory
			try
			{
				CreateAndCheckIsDirectory(dir, attrs);
				return dir;
			}
			catch (FileAlreadyExistsException x)
			{
				// file exists and is not a directory
				throw x;
			}
			catch (IOException)
			{
				// parent may not exist or other reason
			}
			SecurityException se = null;
			try
			{
				dir = dir.ToAbsolutePath();
			}
			catch (SecurityException x)
			{
				// don't have permission to get absolute path
				se = x;
			}
			// find a decendent that exists
			Path parent = dir.Parent;
			while (parent != null)
			{
				try
				{
					Provider(parent).checkAccess(parent);
					break;
				}
				catch (NoSuchFileException)
				{
					// does not exist
				}
				parent = parent.Parent;
			}
			if (parent == null)
			{
				// unable to find existing parent
				if (se == null)
				{
					throw new FileSystemException(dir.ToString(), null, "Unable to determine if root directory exists");
				}
				else
				{
					throw se;
				}
			}

			// create directories
			Path child = parent;
			foreach (Path name in parent.Relativize(dir))
			{
				child = child.Resolve(name);
				CreateAndCheckIsDirectory(child, attrs);
			}
			return dir;
		}

		/// <summary>
		/// Used by createDirectories to attempt to create a directory. A no-op
		/// if the directory already exists.
		/// </summary>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: private static void createAndCheckIsDirectory(Path dir, java.nio.file.attribute.FileAttribute<?>... attrs) throws java.io.IOException
//JAVA TO C# CONVERTER TODO TASK: Java wildcard generics are not converted to .NET:
		private static void CreateAndCheckIsDirectory(Path dir, params FileAttribute<?>[] attrs)
		{
			try
			{
				CreateDirectory(dir, attrs);
			}
			catch (FileAlreadyExistsException x)
			{
				if (!IsDirectory(dir, LinkOption.NOFOLLOW_LINKS))
				{
					throw x;
				}
			}
		}

		/// <summary>
		/// Creates a new empty file in the specified directory, using the given
		/// prefix and suffix strings to generate its name. The resulting
		/// {@code Path} is associated with the same {@code FileSystem} as the given
		/// directory.
		/// 
		/// <para> The details as to how the name of the file is constructed is
		/// implementation dependent and therefore not specified. Where possible
		/// the {@code prefix} and {@code suffix} are used to construct candidate
		/// names in the same manner as the {@link
		/// java.io.File#createTempFile(String,String,File)} method.
		/// 
		/// </para>
		/// <para> As with the {@code File.createTempFile} methods, this method is only
		/// part of a temporary-file facility. Where used as a <em>work files</em>,
		/// the resulting file may be opened using the {@link
		/// StandardOpenOption#DELETE_ON_CLOSE DELETE_ON_CLOSE} option so that the
		/// file is deleted when the appropriate {@code close} method is invoked.
		/// Alternatively, a <seealso cref="Runtime#addShutdownHook shutdown-hook"/>, or the
		/// <seealso cref="java.io.File#deleteOnExit"/> mechanism may be used to delete the
		/// file automatically.
		/// 
		/// </para>
		/// <para> The {@code attrs} parameter is optional {@link FileAttribute
		/// file-attributes} to set atomically when creating the file. Each attribute
		/// is identified by its <seealso cref="FileAttribute#name name"/>. If more than one
		/// attribute of the same name is included in the array then all but the last
		/// occurrence is ignored. When no file attributes are specified, then the
		/// resulting file may have more restrictive access permissions to files
		/// created by the <seealso cref="java.io.File#createTempFile(String,String,File)"/>
		/// method.
		/// 
		/// </para>
		/// </summary>
		/// <param name="dir">
		///          the path to directory in which to create the file </param>
		/// <param name="prefix">
		///          the prefix string to be used in generating the file's name;
		///          may be {@code null} </param>
		/// <param name="suffix">
		///          the suffix string to be used in generating the file's name;
		///          may be {@code null}, in which case "{@code .tmp}" is used </param>
		/// <param name="attrs">
		///          an optional list of file attributes to set atomically when
		///          creating the file
		/// </param>
		/// <returns>  the path to the newly created file that did not exist before
		///          this method was invoked
		/// </returns>
		/// <exception cref="IllegalArgumentException">
		///          if the prefix or suffix parameters cannot be used to generate
		///          a candidate file name </exception>
		/// <exception cref="UnsupportedOperationException">
		///          if the array contains an attribute that cannot be set atomically
		///          when creating the directory </exception>
		/// <exception cref="IOException">
		///          if an I/O error occurs or {@code dir} does not exist </exception>
		/// <exception cref="SecurityException">
		///          In the case of the default provider, and a security manager is
		///          installed, the <seealso cref="SecurityManager#checkWrite(String) checkWrite"/>
		///          method is invoked to check write access to the file. </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public static Path createTempFile(Path dir, String prefix, String suffix, java.nio.file.attribute.FileAttribute<?>... attrs) throws java.io.IOException
//JAVA TO C# CONVERTER TODO TASK: Java wildcard generics are not converted to .NET:
		public static Path CreateTempFile(Path dir, String prefix, String suffix, params FileAttribute<?>[] attrs)
		{
			return TempFileHelper.CreateTempFile(Objects.RequireNonNull(dir), prefix, suffix, attrs);
		}

		/// <summary>
		/// Creates an empty file in the default temporary-file directory, using
		/// the given prefix and suffix to generate its name. The resulting {@code
		/// Path} is associated with the default {@code FileSystem}.
		/// 
		/// <para> This method works in exactly the manner specified by the
		/// <seealso cref="#createTempFile(Path,String,String,FileAttribute[])"/> method for
		/// the case that the {@code dir} parameter is the temporary-file directory.
		/// 
		/// </para>
		/// </summary>
		/// <param name="prefix">
		///          the prefix string to be used in generating the file's name;
		///          may be {@code null} </param>
		/// <param name="suffix">
		///          the suffix string to be used in generating the file's name;
		///          may be {@code null}, in which case "{@code .tmp}" is used </param>
		/// <param name="attrs">
		///          an optional list of file attributes to set atomically when
		///          creating the file
		/// </param>
		/// <returns>  the path to the newly created file that did not exist before
		///          this method was invoked
		/// </returns>
		/// <exception cref="IllegalArgumentException">
		///          if the prefix or suffix parameters cannot be used to generate
		///          a candidate file name </exception>
		/// <exception cref="UnsupportedOperationException">
		///          if the array contains an attribute that cannot be set atomically
		///          when creating the directory </exception>
		/// <exception cref="IOException">
		///          if an I/O error occurs or the temporary-file directory does not
		///          exist </exception>
		/// <exception cref="SecurityException">
		///          In the case of the default provider, and a security manager is
		///          installed, the <seealso cref="SecurityManager#checkWrite(String) checkWrite"/>
		///          method is invoked to check write access to the file. </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public static Path createTempFile(String prefix, String suffix, java.nio.file.attribute.FileAttribute<?>... attrs) throws java.io.IOException
//JAVA TO C# CONVERTER TODO TASK: Java wildcard generics are not converted to .NET:
		public static Path CreateTempFile(String prefix, String suffix, params FileAttribute<?>[] attrs)
		{
			return TempFileHelper.CreateTempFile(null, prefix, suffix, attrs);
		}

		/// <summary>
		/// Creates a new directory in the specified directory, using the given
		/// prefix to generate its name.  The resulting {@code Path} is associated
		/// with the same {@code FileSystem} as the given directory.
		/// 
		/// <para> The details as to how the name of the directory is constructed is
		/// implementation dependent and therefore not specified. Where possible
		/// the {@code prefix} is used to construct candidate names.
		/// 
		/// </para>
		/// <para> As with the {@code createTempFile} methods, this method is only
		/// part of a temporary-file facility. A {@link Runtime#addShutdownHook
		/// shutdown-hook}, or the <seealso cref="java.io.File#deleteOnExit"/> mechanism may be
		/// used to delete the directory automatically.
		/// 
		/// </para>
		/// <para> The {@code attrs} parameter is optional {@link FileAttribute
		/// file-attributes} to set atomically when creating the directory. Each
		/// attribute is identified by its <seealso cref="FileAttribute#name name"/>. If more
		/// than one attribute of the same name is included in the array then all but
		/// the last occurrence is ignored.
		/// 
		/// </para>
		/// </summary>
		/// <param name="dir">
		///          the path to directory in which to create the directory </param>
		/// <param name="prefix">
		///          the prefix string to be used in generating the directory's name;
		///          may be {@code null} </param>
		/// <param name="attrs">
		///          an optional list of file attributes to set atomically when
		///          creating the directory
		/// </param>
		/// <returns>  the path to the newly created directory that did not exist before
		///          this method was invoked
		/// </returns>
		/// <exception cref="IllegalArgumentException">
		///          if the prefix cannot be used to generate a candidate directory name </exception>
		/// <exception cref="UnsupportedOperationException">
		///          if the array contains an attribute that cannot be set atomically
		///          when creating the directory </exception>
		/// <exception cref="IOException">
		///          if an I/O error occurs or {@code dir} does not exist </exception>
		/// <exception cref="SecurityException">
		///          In the case of the default provider, and a security manager is
		///          installed, the <seealso cref="SecurityManager#checkWrite(String) checkWrite"/>
		///          method is invoked to check write access when creating the
		///          directory. </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public static Path createTempDirectory(Path dir, String prefix, java.nio.file.attribute.FileAttribute<?>... attrs) throws java.io.IOException
//JAVA TO C# CONVERTER TODO TASK: Java wildcard generics are not converted to .NET:
		public static Path CreateTempDirectory(Path dir, String prefix, params FileAttribute<?>[] attrs)
		{
			return TempFileHelper.CreateTempDirectory(Objects.RequireNonNull(dir), prefix, attrs);
		}

		/// <summary>
		/// Creates a new directory in the default temporary-file directory, using
		/// the given prefix to generate its name. The resulting {@code Path} is
		/// associated with the default {@code FileSystem}.
		/// 
		/// <para> This method works in exactly the manner specified by {@link
		/// #createTempDirectory(Path,String,FileAttribute[])} method for the case
		/// that the {@code dir} parameter is the temporary-file directory.
		/// 
		/// </para>
		/// </summary>
		/// <param name="prefix">
		///          the prefix string to be used in generating the directory's name;
		///          may be {@code null} </param>
		/// <param name="attrs">
		///          an optional list of file attributes to set atomically when
		///          creating the directory
		/// </param>
		/// <returns>  the path to the newly created directory that did not exist before
		///          this method was invoked
		/// </returns>
		/// <exception cref="IllegalArgumentException">
		///          if the prefix cannot be used to generate a candidate directory name </exception>
		/// <exception cref="UnsupportedOperationException">
		///          if the array contains an attribute that cannot be set atomically
		///          when creating the directory </exception>
		/// <exception cref="IOException">
		///          if an I/O error occurs or the temporary-file directory does not
		///          exist </exception>
		/// <exception cref="SecurityException">
		///          In the case of the default provider, and a security manager is
		///          installed, the <seealso cref="SecurityManager#checkWrite(String) checkWrite"/>
		///          method is invoked to check write access when creating the
		///          directory. </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public static Path createTempDirectory(String prefix, java.nio.file.attribute.FileAttribute<?>... attrs) throws java.io.IOException
//JAVA TO C# CONVERTER TODO TASK: Java wildcard generics are not converted to .NET:
		public static Path CreateTempDirectory(String prefix, params FileAttribute<?>[] attrs)
		{
			return TempFileHelper.CreateTempDirectory(null, prefix, attrs);
		}

		/// <summary>
		/// Creates a symbolic link to a target <i>(optional operation)</i>.
		/// 
		/// <para> The {@code target} parameter is the target of the link. It may be an
		/// <seealso cref="Path#isAbsolute absolute"/> or relative path and may not exist. When
		/// the target is a relative path then file system operations on the resulting
		/// link are relative to the path of the link.
		/// 
		/// </para>
		/// <para> The {@code attrs} parameter is optional {@link FileAttribute
		/// attributes} to set atomically when creating the link. Each attribute is
		/// identified by its <seealso cref="FileAttribute#name name"/>. If more than one attribute
		/// of the same name is included in the array then all but the last occurrence
		/// is ignored.
		/// 
		/// </para>
		/// <para> Where symbolic links are supported, but the underlying <seealso cref="FileStore"/>
		/// does not support symbolic links, then this may fail with an {@link
		/// IOException}. Additionally, some operating systems may require that the
		/// Java virtual machine be started with implementation specific privileges to
		/// create symbolic links, in which case this method may throw {@code IOException}.
		/// 
		/// </para>
		/// </summary>
		/// <param name="link">
		///          the path of the symbolic link to create </param>
		/// <param name="target">
		///          the target of the symbolic link </param>
		/// <param name="attrs">
		///          the array of attributes to set atomically when creating the
		///          symbolic link
		/// </param>
		/// <returns>  the path to the symbolic link
		/// </returns>
		/// <exception cref="UnsupportedOperationException">
		///          if the implementation does not support symbolic links or the
		///          array contains an attribute that cannot be set atomically when
		///          creating the symbolic link </exception>
		/// <exception cref="FileAlreadyExistsException">
		///          if a file with the name already exists <i>(optional specific
		///          exception)</i> </exception>
		/// <exception cref="IOException">
		///          if an I/O error occurs </exception>
		/// <exception cref="SecurityException">
		///          In the case of the default provider, and a security manager
		///          is installed, it denies <seealso cref="LinkPermission"/><tt>("symbolic")</tt>
		///          or its <seealso cref="SecurityManager#checkWrite(String) checkWrite"/>
		///          method denies write access to the path of the symbolic link. </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public static Path createSymbolicLink(Path link, Path target, java.nio.file.attribute.FileAttribute<?>... attrs) throws java.io.IOException
//JAVA TO C# CONVERTER TODO TASK: Java wildcard generics are not converted to .NET:
		public static Path CreateSymbolicLink(Path link, Path target, params FileAttribute<?>[] attrs)
		{
			Provider(link).CreateSymbolicLink(link, target, attrs);
			return link;
		}

		/// <summary>
		/// Creates a new link (directory entry) for an existing file <i>(optional
		/// operation)</i>.
		/// 
		/// <para> The {@code link} parameter locates the directory entry to create.
		/// The {@code existing} parameter is the path to an existing file. This
		/// method creates a new directory entry for the file so that it can be
		/// accessed using {@code link} as the path. On some file systems this is
		/// known as creating a "hard link". Whether the file attributes are
		/// maintained for the file or for each directory entry is file system
		/// specific and therefore not specified. Typically, a file system requires
		/// that all links (directory entries) for a file be on the same file system.
		/// Furthermore, on some platforms, the Java virtual machine may require to
		/// be started with implementation specific privileges to create hard links
		/// or to create links to directories.
		/// 
		/// </para>
		/// </summary>
		/// <param name="link">
		///          the link (directory entry) to create </param>
		/// <param name="existing">
		///          a path to an existing file
		/// </param>
		/// <returns>  the path to the link (directory entry)
		/// </returns>
		/// <exception cref="UnsupportedOperationException">
		///          if the implementation does not support adding an existing file
		///          to a directory </exception>
		/// <exception cref="FileAlreadyExistsException">
		///          if the entry could not otherwise be created because a file of
		///          that name already exists <i>(optional specific exception)</i> </exception>
		/// <exception cref="IOException">
		///          if an I/O error occurs </exception>
		/// <exception cref="SecurityException">
		///          In the case of the default provider, and a security manager
		///          is installed, it denies <seealso cref="LinkPermission"/><tt>("hard")</tt>
		///          or its <seealso cref="SecurityManager#checkWrite(String) checkWrite"/>
		///          method denies write access to either the link or the
		///          existing file. </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public static Path createLink(Path link, Path existing) throws java.io.IOException
		public static Path CreateLink(Path link, Path existing)
		{
			Provider(link).CreateLink(link, existing);
			return link;
		}

		/// <summary>
		/// Deletes a file.
		/// 
		/// <para> An implementation may require to examine the file to determine if the
		/// file is a directory. Consequently this method may not be atomic with respect
		/// to other file system operations.  If the file is a symbolic link then the
		/// symbolic link itself, not the final target of the link, is deleted.
		/// 
		/// </para>
		/// <para> If the file is a directory then the directory must be empty. In some
		/// implementations a directory has entries for special files or links that
		/// are created when the directory is created. In such implementations a
		/// directory is considered empty when only the special entries exist.
		/// This method can be used with the <seealso cref="#walkFileTree walkFileTree"/>
		/// method to delete a directory and all entries in the directory, or an
		/// entire <i>file-tree</i> where required.
		/// 
		/// </para>
		/// <para> On some operating systems it may not be possible to remove a file when
		/// it is open and in use by this Java virtual machine or other programs.
		/// 
		/// </para>
		/// </summary>
		/// <param name="path">
		///          the path to the file to delete
		/// </param>
		/// <exception cref="NoSuchFileException">
		///          if the file does not exist <i>(optional specific exception)</i> </exception>
		/// <exception cref="DirectoryNotEmptyException">
		///          if the file is a directory and could not otherwise be deleted
		///          because the directory is not empty <i>(optional specific
		///          exception)</i> </exception>
		/// <exception cref="IOException">
		///          if an I/O error occurs </exception>
		/// <exception cref="SecurityException">
		///          In the case of the default provider, and a security manager is
		///          installed, the <seealso cref="SecurityManager#checkDelete(String)"/> method
		///          is invoked to check delete access to the file </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public static void delete(Path path) throws java.io.IOException
		public static void Delete(Path path)
		{
			Provider(path).Delete(path);
		}

		/// <summary>
		/// Deletes a file if it exists.
		/// 
		/// <para> As with the <seealso cref="#delete(Path) delete(Path)"/> method, an
		/// implementation may need to examine the file to determine if the file is a
		/// directory. Consequently this method may not be atomic with respect to
		/// other file system operations.  If the file is a symbolic link, then the
		/// symbolic link itself, not the final target of the link, is deleted.
		/// 
		/// </para>
		/// <para> If the file is a directory then the directory must be empty. In some
		/// implementations a directory has entries for special files or links that
		/// are created when the directory is created. In such implementations a
		/// directory is considered empty when only the special entries exist.
		/// 
		/// </para>
		/// <para> On some operating systems it may not be possible to remove a file when
		/// it is open and in use by this Java virtual machine or other programs.
		/// 
		/// </para>
		/// </summary>
		/// <param name="path">
		///          the path to the file to delete
		/// </param>
		/// <returns>  {@code true} if the file was deleted by this method; {@code
		///          false} if the file could not be deleted because it did not
		///          exist
		/// </returns>
		/// <exception cref="DirectoryNotEmptyException">
		///          if the file is a directory and could not otherwise be deleted
		///          because the directory is not empty <i>(optional specific
		///          exception)</i> </exception>
		/// <exception cref="IOException">
		///          if an I/O error occurs </exception>
		/// <exception cref="SecurityException">
		///          In the case of the default provider, and a security manager is
		///          installed, the <seealso cref="SecurityManager#checkDelete(String)"/> method
		///          is invoked to check delete access to the file. </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public static boolean deleteIfExists(Path path) throws java.io.IOException
		public static bool DeleteIfExists(Path path)
		{
			return Provider(path).DeleteIfExists(path);
		}

		// -- Copying and moving files --

		/// <summary>
		/// Copy a file to a target file.
		/// 
		/// <para> This method copies a file to the target file with the {@code
		/// options} parameter specifying how the copy is performed. By default, the
		/// copy fails if the target file already exists or is a symbolic link,
		/// except if the source and target are the <seealso cref="#isSameFile same"/> file, in
		/// which case the method completes without copying the file. File attributes
		/// are not required to be copied to the target file. If symbolic links are
		/// supported, and the file is a symbolic link, then the final target of the
		/// link is copied. If the file is a directory then it creates an empty
		/// directory in the target location (entries in the directory are not
		/// copied). This method can be used with the {@link #walkFileTree
		/// walkFileTree} method to copy a directory and all entries in the directory,
		/// or an entire <i>file-tree</i> where required.
		/// 
		/// </para>
		/// <para> The {@code options} parameter may include any of the following:
		/// 
		/// <table border=1 cellpadding=5 summary="">
		/// <tr> <th>Option</th> <th>Description</th> </tr>
		/// <tr>
		///   <td> <seealso cref="StandardCopyOption#REPLACE_EXISTING REPLACE_EXISTING"/> </td>
		///   <td> If the target file exists, then the target file is replaced if it
		///     is not a non-empty directory. If the target file exists and is a
		///     symbolic link, then the symbolic link itself, not the target of
		///     the link, is replaced. </td>
		/// </tr>
		/// <tr>
		///   <td> <seealso cref="StandardCopyOption#COPY_ATTRIBUTES COPY_ATTRIBUTES"/> </td>
		///   <td> Attempts to copy the file attributes associated with this file to
		///     the target file. The exact file attributes that are copied is platform
		///     and file system dependent and therefore unspecified. Minimally, the
		///     <seealso cref="BasicFileAttributes#lastModifiedTime last-modified-time"/> is
		///     copied to the target file if supported by both the source and target
		///     file stores. Copying of file timestamps may result in precision
		///     loss. </td>
		/// </tr>
		/// <tr>
		///   <td> <seealso cref="LinkOption#NOFOLLOW_LINKS NOFOLLOW_LINKS"/> </td>
		///   <td> Symbolic links are not followed. If the file is a symbolic link,
		///     then the symbolic link itself, not the target of the link, is copied.
		///     It is implementation specific if file attributes can be copied to the
		///     new link. In other words, the {@code COPY_ATTRIBUTES} option may be
		///     ignored when copying a symbolic link. </td>
		/// </tr>
		/// </table>
		/// 
		/// </para>
		/// <para> An implementation of this interface may support additional
		/// implementation specific options.
		/// 
		/// </para>
		/// <para> Copying a file is not an atomic operation. If an <seealso cref="IOException"/>
		/// is thrown, then it is possible that the target file is incomplete or some
		/// of its file attributes have not been copied from the source file. When
		/// the {@code REPLACE_EXISTING} option is specified and the target file
		/// exists, then the target file is replaced. The check for the existence of
		/// the file and the creation of the new file may not be atomic with respect
		/// to other file system activities.
		/// 
		/// </para>
		/// <para> <b>Usage Example:</b>
		/// Suppose we want to copy a file into a directory, giving it the same file
		/// name as the source file:
		/// <pre>
		///     Path source = ...
		///     Path newdir = ...
		///     Files.copy(source, newdir.resolve(source.getFileName());
		/// </pre>
		/// 
		/// </para>
		/// </summary>
		/// <param name="source">
		///          the path to the file to copy </param>
		/// <param name="target">
		///          the path to the target file (may be associated with a different
		///          provider to the source path) </param>
		/// <param name="options">
		///          options specifying how the copy should be done
		/// </param>
		/// <returns>  the path to the target file
		/// </returns>
		/// <exception cref="UnsupportedOperationException">
		///          if the array contains a copy option that is not supported </exception>
		/// <exception cref="FileAlreadyExistsException">
		///          if the target file exists but cannot be replaced because the
		///          {@code REPLACE_EXISTING} option is not specified <i>(optional
		///          specific exception)</i> </exception>
		/// <exception cref="DirectoryNotEmptyException">
		///          the {@code REPLACE_EXISTING} option is specified but the file
		///          cannot be replaced because it is a non-empty directory
		///          <i>(optional specific exception)</i> </exception>
		/// <exception cref="IOException">
		///          if an I/O error occurs </exception>
		/// <exception cref="SecurityException">
		///          In the case of the default provider, and a security manager is
		///          installed, the <seealso cref="SecurityManager#checkRead(String) checkRead"/>
		///          method is invoked to check read access to the source file, the
		///          <seealso cref="SecurityManager#checkWrite(String) checkWrite"/> is invoked
		///          to check write access to the target file. If a symbolic link is
		///          copied the security manager is invoked to check {@link
		///          LinkPermission}{@code ("symbolic")}. </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public static Path copy(Path source, Path target, CopyOption... options) throws java.io.IOException
		public static Path Copy(Path source, Path target, params CopyOption[] options)
		{
			FileSystemProvider provider = Provider(source);
			if (Provider(target) == provider)
			{
				// same provider
				provider.Copy(source, target, options);
			}
			else
			{
				// different providers
				CopyMoveHelper.CopyToForeignTarget(source, target, options);
			}
			return target;
		}

		/// <summary>
		/// Move or rename a file to a target file.
		/// 
		/// <para> By default, this method attempts to move the file to the target
		/// file, failing if the target file exists except if the source and
		/// target are the <seealso cref="#isSameFile same"/> file, in which case this method
		/// has no effect. If the file is a symbolic link then the symbolic link
		/// itself, not the target of the link, is moved. This method may be
		/// invoked to move an empty directory. In some implementations a directory
		/// has entries for special files or links that are created when the
		/// directory is created. In such implementations a directory is considered
		/// empty when only the special entries exist. When invoked to move a
		/// directory that is not empty then the directory is moved if it does not
		/// require moving the entries in the directory.  For example, renaming a
		/// directory on the same <seealso cref="FileStore"/> will usually not require moving
		/// the entries in the directory. When moving a directory requires that its
		/// entries be moved then this method fails (by throwing an {@code
		/// IOException}). To move a <i>file tree</i> may involve copying rather
		/// than moving directories and this can be done using the {@link
		/// #copy copy} method in conjunction with the {@link
		/// #walkFileTree Files.walkFileTree} utility method.
		/// 
		/// </para>
		/// <para> The {@code options} parameter may include any of the following:
		/// 
		/// <table border=1 cellpadding=5 summary="">
		/// <tr> <th>Option</th> <th>Description</th> </tr>
		/// <tr>
		///   <td> <seealso cref="StandardCopyOption#REPLACE_EXISTING REPLACE_EXISTING"/> </td>
		///   <td> If the target file exists, then the target file is replaced if it
		///     is not a non-empty directory. If the target file exists and is a
		///     symbolic link, then the symbolic link itself, not the target of
		///     the link, is replaced. </td>
		/// </tr>
		/// <tr>
		///   <td> <seealso cref="StandardCopyOption#ATOMIC_MOVE ATOMIC_MOVE"/> </td>
		///   <td> The move is performed as an atomic file system operation and all
		///     other options are ignored. If the target file exists then it is
		///     implementation specific if the existing file is replaced or this method
		///     fails by throwing an <seealso cref="IOException"/>. If the move cannot be
		///     performed as an atomic file system operation then {@link
		///     AtomicMoveNotSupportedException} is thrown. This can arise, for
		///     example, when the target location is on a different {@code FileStore}
		///     and would require that the file be copied, or target location is
		///     associated with a different provider to this object. </td>
		/// </table>
		/// 
		/// </para>
		/// <para> An implementation of this interface may support additional
		/// implementation specific options.
		/// 
		/// </para>
		/// <para> Moving a file will copy the {@link
		/// BasicFileAttributes#lastModifiedTime last-modified-time} to the target
		/// file if supported by both source and target file stores. Copying of file
		/// timestamps may result in precision loss. An implementation may also
		/// attempt to copy other file attributes but is not required to fail if the
		/// file attributes cannot be copied. When the move is performed as
		/// a non-atomic operation, and an {@code IOException} is thrown, then the
		/// state of the files is not defined. The original file and the target file
		/// may both exist, the target file may be incomplete or some of its file
		/// attributes may not been copied from the original file.
		/// 
		/// </para>
		/// <para> <b>Usage Examples:</b>
		/// Suppose we want to rename a file to "newname", keeping the file in the
		/// same directory:
		/// <pre>
		///     Path source = ...
		///     Files.move(source, source.resolveSibling("newname"));
		/// </pre>
		/// Alternatively, suppose we want to move a file to new directory, keeping
		/// the same file name, and replacing any existing file of that name in the
		/// directory:
		/// <pre>
		///     Path source = ...
		///     Path newdir = ...
		///     Files.move(source, newdir.resolve(source.getFileName()), REPLACE_EXISTING);
		/// </pre>
		/// 
		/// </para>
		/// </summary>
		/// <param name="source">
		///          the path to the file to move </param>
		/// <param name="target">
		///          the path to the target file (may be associated with a different
		///          provider to the source path) </param>
		/// <param name="options">
		///          options specifying how the move should be done
		/// </param>
		/// <returns>  the path to the target file
		/// </returns>
		/// <exception cref="UnsupportedOperationException">
		///          if the array contains a copy option that is not supported </exception>
		/// <exception cref="FileAlreadyExistsException">
		///          if the target file exists but cannot be replaced because the
		///          {@code REPLACE_EXISTING} option is not specified <i>(optional
		///          specific exception)</i> </exception>
		/// <exception cref="DirectoryNotEmptyException">
		///          the {@code REPLACE_EXISTING} option is specified but the file
		///          cannot be replaced because it is a non-empty directory
		///          <i>(optional specific exception)</i> </exception>
		/// <exception cref="AtomicMoveNotSupportedException">
		///          if the options array contains the {@code ATOMIC_MOVE} option but
		///          the file cannot be moved as an atomic file system operation. </exception>
		/// <exception cref="IOException">
		///          if an I/O error occurs </exception>
		/// <exception cref="SecurityException">
		///          In the case of the default provider, and a security manager is
		///          installed, the <seealso cref="SecurityManager#checkWrite(String) checkWrite"/>
		///          method is invoked to check write access to both the source and
		///          target file. </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public static Path move(Path source, Path target, CopyOption... options) throws java.io.IOException
		public static Path Move(Path source, Path target, params CopyOption[] options)
		{
			FileSystemProvider provider = Provider(source);
			if (Provider(target) == provider)
			{
				// same provider
				provider.Move(source, target, options);
			}
			else
			{
				// different providers
				CopyMoveHelper.MoveToForeignTarget(source, target, options);
			}
			return target;
		}

		// -- Miscellenous --

		/// <summary>
		/// Reads the target of a symbolic link <i>(optional operation)</i>.
		/// 
		/// <para> If the file system supports <a href="package-summary.html#links">symbolic
		/// links</a> then this method is used to read the target of the link, failing
		/// if the file is not a symbolic link. The target of the link need not exist.
		/// The returned {@code Path} object will be associated with the same file
		/// system as {@code link}.
		/// 
		/// </para>
		/// </summary>
		/// <param name="link">
		///          the path to the symbolic link
		/// </param>
		/// <returns>  a {@code Path} object representing the target of the link
		/// </returns>
		/// <exception cref="UnsupportedOperationException">
		///          if the implementation does not support symbolic links </exception>
		/// <exception cref="NotLinkException">
		///          if the target could otherwise not be read because the file
		///          is not a symbolic link <i>(optional specific exception)</i> </exception>
		/// <exception cref="IOException">
		///          if an I/O error occurs </exception>
		/// <exception cref="SecurityException">
		///          In the case of the default provider, and a security manager
		///          is installed, it checks that {@code FilePermission} has been
		///          granted with the "{@code readlink}" action to read the link. </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public static Path readSymbolicLink(Path link) throws java.io.IOException
		public static Path ReadSymbolicLink(Path link)
		{
			return Provider(link).ReadSymbolicLink(link);
		}

		/// <summary>
		/// Returns the <seealso cref="FileStore"/> representing the file store where a file
		/// is located.
		/// 
		/// <para> Once a reference to the {@code FileStore} is obtained it is
		/// implementation specific if operations on the returned {@code FileStore},
		/// or <seealso cref="FileStoreAttributeView"/> objects obtained from it, continue
		/// to depend on the existence of the file. In particular the behavior is not
		/// defined for the case that the file is deleted or moved to a different
		/// file store.
		/// 
		/// </para>
		/// </summary>
		/// <param name="path">
		///          the path to the file
		/// </param>
		/// <returns>  the file store where the file is stored
		/// </returns>
		/// <exception cref="IOException">
		///          if an I/O error occurs </exception>
		/// <exception cref="SecurityException">
		///          In the case of the default provider, and a security manager is
		///          installed, the <seealso cref="SecurityManager#checkRead(String) checkRead"/>
		///          method is invoked to check read access to the file, and in
		///          addition it checks <seealso cref="RuntimePermission"/><tt>
		///          ("getFileStoreAttributes")</tt> </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public static FileStore getFileStore(Path path) throws java.io.IOException
		public static FileStore GetFileStore(Path path)
		{
			return Provider(path).GetFileStore(path);
		}

		/// <summary>
		/// Tests if two paths locate the same file.
		/// 
		/// <para> If both {@code Path} objects are <seealso cref="Path#equals(Object) equal"/>
		/// then this method returns {@code true} without checking if the file exists.
		/// If the two {@code Path} objects are associated with different providers
		/// then this method returns {@code false}. Otherwise, this method checks if
		/// both {@code Path} objects locate the same file, and depending on the
		/// implementation, may require to open or access both files.
		/// 
		/// </para>
		/// <para> If the file system and files remain static, then this method implements
		/// an equivalence relation for non-null {@code Paths}.
		/// <ul>
		/// <li>It is <i>reflexive</i>: for {@code Path} {@code f},
		///     {@code isSameFile(f,f)} should return {@code true}.
		/// <li>It is <i>symmetric</i>: for two {@code Paths} {@code f} and {@code g},
		///     {@code isSameFile(f,g)} will equal {@code isSameFile(g,f)}.
		/// <li>It is <i>transitive</i>: for three {@code Paths}
		///     {@code f}, {@code g}, and {@code h}, if {@code isSameFile(f,g)} returns
		///     {@code true} and {@code isSameFile(g,h)} returns {@code true}, then
		///     {@code isSameFile(f,h)} will return return {@code true}.
		/// </ul>
		/// 
		/// </para>
		/// </summary>
		/// <param name="path">
		///          one path to the file </param>
		/// <param name="path2">
		///          the other path
		/// </param>
		/// <returns>  {@code true} if, and only if, the two paths locate the same file
		/// </returns>
		/// <exception cref="IOException">
		///          if an I/O error occurs </exception>
		/// <exception cref="SecurityException">
		///          In the case of the default provider, and a security manager is
		///          installed, the <seealso cref="SecurityManager#checkRead(String) checkRead"/>
		///          method is invoked to check read access to both files.
		/// </exception>
		/// <seealso cref= java.nio.file.attribute.BasicFileAttributes#fileKey </seealso>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public static boolean isSameFile(Path path, Path path2) throws java.io.IOException
		public static bool IsSameFile(Path path, Path path2)
		{
			return Provider(path).IsSameFile(path, path2);
		}

		/// <summary>
		/// Tells whether or not a file is considered <em>hidden</em>. The exact
		/// definition of hidden is platform or provider dependent. On UNIX for
		/// example a file is considered to be hidden if its name begins with a
		/// period character ('.'). On Windows a file is considered hidden if it
		/// isn't a directory and the DOS <seealso cref="DosFileAttributes#isHidden hidden"/>
		/// attribute is set.
		/// 
		/// <para> Depending on the implementation this method may require to access
		/// the file system to determine if the file is considered hidden.
		/// 
		/// </para>
		/// </summary>
		/// <param name="path">
		///          the path to the file to test
		/// </param>
		/// <returns>  {@code true} if the file is considered hidden
		/// </returns>
		/// <exception cref="IOException">
		///          if an I/O error occurs </exception>
		/// <exception cref="SecurityException">
		///          In the case of the default provider, and a security manager is
		///          installed, the <seealso cref="SecurityManager#checkRead(String) checkRead"/>
		///          method is invoked to check read access to the file. </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public static boolean isHidden(Path path) throws java.io.IOException
		public static bool IsHidden(Path path)
		{
			return Provider(path).IsHidden(path);
		}

		// lazy loading of default and installed file type detectors
		private class FileTypeDetectors
		{
			internal static readonly FileTypeDetector DefaultFileTypeDetector = CreateDefaultFileTypeDetector();
			internal static readonly IList<FileTypeDetector> InstalleDetectors = LoadInstalledDetectors();

			// creates the default file type detector
			internal static FileTypeDetector CreateDefaultFileTypeDetector()
			{
				return AccessController.doPrivileged(new PrivilegedActionAnonymousInnerClassHelper());
			}

			private class PrivilegedActionAnonymousInnerClassHelper : PrivilegedAction<FileTypeDetector>
			{
				public PrivilegedActionAnonymousInnerClassHelper()
				{
				}

				public virtual FileTypeDetector Run()
				{
					return sun.nio.fs.DefaultFileTypeDetector.create();
				}
			}

			// loads all installed file type detectors
			internal static IList<FileTypeDetector> LoadInstalledDetectors()
			{
				return AccessController.doPrivileged(new PrivilegedActionAnonymousInnerClassHelper2());
			}

			private class PrivilegedActionAnonymousInnerClassHelper2 : PrivilegedAction<IList<FileTypeDetector>>
			{
				public PrivilegedActionAnonymousInnerClassHelper2()
				{
				}

				public virtual IList<FileTypeDetector> Run()
				{
					IList<FileTypeDetector> list = new List<FileTypeDetector>();
					ServiceLoader<FileTypeDetector> loader = ServiceLoader.Load(typeof(FileTypeDetector), ClassLoader.SystemClassLoader);
					foreach (FileTypeDetector detector in loader)
					{
						list.Add(detector);
					}
					return list;
				}
			}
		}

		/// <summary>
		/// Probes the content type of a file.
		/// 
		/// <para> This method uses the installed <seealso cref="FileTypeDetector"/> implementations
		/// to probe the given file to determine its content type. Each file type
		/// detector's <seealso cref="FileTypeDetector#probeContentType probeContentType"/> is
		/// invoked, in turn, to probe the file type. If the file is recognized then
		/// the content type is returned. If the file is not recognized by any of the
		/// installed file type detectors then a system-default file type detector is
		/// invoked to guess the content type.
		/// 
		/// </para>
		/// <para> A given invocation of the Java virtual machine maintains a system-wide
		/// list of file type detectors. Installed file type detectors are loaded
		/// using the service-provider loading facility defined by the <seealso cref="ServiceLoader"/>
		/// class. Installed file type detectors are loaded using the system class
		/// loader. If the system class loader cannot be found then the extension class
		/// loader is used; If the extension class loader cannot be found then the
		/// bootstrap class loader is used. File type detectors are typically installed
		/// by placing them in a JAR file on the application class path or in the
		/// extension directory, the JAR file contains a provider-configuration file
		/// named {@code java.nio.file.spi.FileTypeDetector} in the resource directory
		/// {@code META-INF/services}, and the file lists one or more fully-qualified
		/// names of concrete subclass of {@code FileTypeDetector } that have a zero
		/// argument constructor. If the process of locating or instantiating the
		/// installed file type detectors fails then an unspecified error is thrown.
		/// The ordering that installed providers are located is implementation
		/// specific.
		/// 
		/// </para>
		/// <para> The return value of this method is the string form of the value of a
		/// Multipurpose Internet Mail Extension (MIME) content type as
		/// defined by <a href="http://www.ietf.org/rfc/rfc2045.txt"><i>RFC&nbsp;2045:
		/// Multipurpose Internet Mail Extensions (MIME) Part One: Format of Internet
		/// Message Bodies</i></a>. The string is guaranteed to be parsable according
		/// to the grammar in the RFC.
		/// 
		/// </para>
		/// </summary>
		/// <param name="path">
		///          the path to the file to probe
		/// </param>
		/// <returns>  The content type of the file, or {@code null} if the content
		///          type cannot be determined
		/// </returns>
		/// <exception cref="IOException">
		///          if an I/O error occurs </exception>
		/// <exception cref="SecurityException">
		///          If a security manager is installed and it denies an unspecified
		///          permission required by a file type detector implementation. </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public static String probeContentType(Path path) throws java.io.IOException
		public static String ProbeContentType(Path path)
		{
			// try installed file type detectors
			foreach (FileTypeDetector detector in FileTypeDetectors.InstalleDetectors)
			{
				String result = detector.ProbeContentType(path);
				if (result != null)
				{
					return result;
				}
			}

			// fallback to default
			return FileTypeDetectors.DefaultFileTypeDetector.ProbeContentType(path);
		}

		// -- File Attributes --

		/// <summary>
		/// Returns a file attribute view of a given type.
		/// 
		/// <para> A file attribute view provides a read-only or updatable view of a
		/// set of file attributes. This method is intended to be used where the file
		/// attribute view defines type-safe methods to read or update the file
		/// attributes. The {@code type} parameter is the type of the attribute view
		/// required and the method returns an instance of that type if supported.
		/// The <seealso cref="BasicFileAttributeView"/> type supports access to the basic
		/// attributes of a file. Invoking this method to select a file attribute
		/// view of that type will always return an instance of that class.
		/// 
		/// </para>
		/// <para> The {@code options} array may be used to indicate how symbolic links
		/// are handled by the resulting file attribute view for the case that the
		/// file is a symbolic link. By default, symbolic links are followed. If the
		/// option <seealso cref="LinkOption#NOFOLLOW_LINKS NOFOLLOW_LINKS"/> is present then
		/// symbolic links are not followed. This option is ignored by implementations
		/// that do not support symbolic links.
		/// 
		/// </para>
		/// <para> <b>Usage Example:</b>
		/// Suppose we want read or set a file's ACL, if supported:
		/// <pre>
		///     Path path = ...
		///     AclFileAttributeView view = Files.getFileAttributeView(path, AclFileAttributeView.class);
		///     if (view != null) {
		///         List&lt;AclEntry&gt; acl = view.getAcl();
		///         :
		///     }
		/// </pre>
		/// 
		/// </para>
		/// </summary>
		/// @param   <V>
		///          The {@code FileAttributeView} type </param>
		/// <param name="path">
		///          the path to the file </param>
		/// <param name="type">
		///          the {@code Class} object corresponding to the file attribute view </param>
		/// <param name="options">
		///          options indicating how symbolic links are handled
		/// </param>
		/// <returns>  a file attribute view of the specified type, or {@code null} if
		///          the attribute view type is not available </returns>
		public static V getFileAttributeView<V>(Path path, Class type, params LinkOption[] options) where V : java.nio.file.attribute.FileAttributeView
		{
			return Provider(path).GetFileAttributeView(path, type, options);
		}

		/// <summary>
		/// Reads a file's attributes as a bulk operation.
		/// 
		/// <para> The {@code type} parameter is the type of the attributes required
		/// and this method returns an instance of that type if supported. All
		/// implementations support a basic set of file attributes and so invoking
		/// this method with a  {@code type} parameter of {@code
		/// BasicFileAttributes.class} will not throw {@code
		/// UnsupportedOperationException}.
		/// 
		/// </para>
		/// <para> The {@code options} array may be used to indicate how symbolic links
		/// are handled for the case that the file is a symbolic link. By default,
		/// symbolic links are followed and the file attribute of the final target
		/// of the link is read. If the option {@link LinkOption#NOFOLLOW_LINKS
		/// NOFOLLOW_LINKS} is present then symbolic links are not followed.
		/// 
		/// </para>
		/// <para> It is implementation specific if all file attributes are read as an
		/// atomic operation with respect to other file system operations.
		/// 
		/// </para>
		/// <para> <b>Usage Example:</b>
		/// Suppose we want to read a file's attributes in bulk:
		/// <pre>
		///    Path path = ...
		///    BasicFileAttributes attrs = Files.readAttributes(path, BasicFileAttributes.class);
		/// </pre>
		/// Alternatively, suppose we want to read file's POSIX attributes without
		/// following symbolic links:
		/// <pre>
		///    PosixFileAttributes attrs = Files.readAttributes(path, PosixFileAttributes.class, NOFOLLOW_LINKS);
		/// </pre>
		/// 
		/// </para>
		/// </summary>
		/// @param   <A>
		///          The {@code BasicFileAttributes} type </param>
		/// <param name="path">
		///          the path to the file </param>
		/// <param name="type">
		///          the {@code Class} of the file attributes required
		///          to read </param>
		/// <param name="options">
		///          options indicating how symbolic links are handled
		/// </param>
		/// <returns>  the file attributes
		/// </returns>
		/// <exception cref="UnsupportedOperationException">
		///          if an attributes of the given type are not supported </exception>
		/// <exception cref="IOException">
		///          if an I/O error occurs </exception>
		/// <exception cref="SecurityException">
		///          In the case of the default provider, a security manager is
		///          installed, its <seealso cref="SecurityManager#checkRead(String) checkRead"/>
		///          method is invoked to check read access to the file. If this
		///          method is invoked to read security sensitive attributes then the
		///          security manager may be invoke to check for additional permissions. </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public static <A extends java.nio.file.attribute.BasicFileAttributes> A readAttributes(Path path, Class type, LinkOption... options) throws java.io.IOException
		public static A readAttributes<A>(Path path, Class type, params LinkOption[] options) where A : java.nio.file.attribute.BasicFileAttributes
		{
			return Provider(path).ReadAttributes(path, type, options);
		}

		/// <summary>
		/// Sets the value of a file attribute.
		/// 
		/// <para> The {@code attribute} parameter identifies the attribute to be set
		/// and takes the form:
		/// <blockquote>
		/// [<i>view-name</i><b>:</b>]<i>attribute-name</i>
		/// </blockquote>
		/// where square brackets [...] delineate an optional component and the
		/// character {@code ':'} stands for itself.
		/// 
		/// </para>
		/// <para> <i>view-name</i> is the <seealso cref="FileAttributeView#name name"/> of a {@link
		/// FileAttributeView} that identifies a set of file attributes. If not
		/// specified then it defaults to {@code "basic"}, the name of the file
		/// attribute view that identifies the basic set of file attributes common to
		/// many file systems. <i>attribute-name</i> is the name of the attribute
		/// within the set.
		/// 
		/// </para>
		/// <para> The {@code options} array may be used to indicate how symbolic links
		/// are handled for the case that the file is a symbolic link. By default,
		/// symbolic links are followed and the file attribute of the final target
		/// of the link is set. If the option {@link LinkOption#NOFOLLOW_LINKS
		/// NOFOLLOW_LINKS} is present then symbolic links are not followed.
		/// 
		/// </para>
		/// <para> <b>Usage Example:</b>
		/// Suppose we want to set the DOS "hidden" attribute:
		/// <pre>
		///    Path path = ...
		///    Files.setAttribute(path, "dos:hidden", true);
		/// </pre>
		/// 
		/// </para>
		/// </summary>
		/// <param name="path">
		///          the path to the file </param>
		/// <param name="attribute">
		///          the attribute to set </param>
		/// <param name="value">
		///          the attribute value </param>
		/// <param name="options">
		///          options indicating how symbolic links are handled
		/// </param>
		/// <returns>  the {@code path} parameter
		/// </returns>
		/// <exception cref="UnsupportedOperationException">
		///          if the attribute view is not available </exception>
		/// <exception cref="IllegalArgumentException">
		///          if the attribute name is not specified, or is not recognized, or
		///          the attribute value is of the correct type but has an
		///          inappropriate value </exception>
		/// <exception cref="ClassCastException">
		///          if the attribute value is not of the expected type or is a
		///          collection containing elements that are not of the expected
		///          type </exception>
		/// <exception cref="IOException">
		///          if an I/O error occurs </exception>
		/// <exception cref="SecurityException">
		///          In the case of the default provider, and a security manager is
		///          installed, its <seealso cref="SecurityManager#checkWrite(String) checkWrite"/>
		///          method denies write access to the file. If this method is invoked
		///          to set security sensitive attributes then the security manager
		///          may be invoked to check for additional permissions. </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public static Path setAttribute(Path path, String attribute, Object value, LinkOption... options) throws java.io.IOException
		public static Path SetAttribute(Path path, String attribute, Object value, params LinkOption[] options)
		{
			Provider(path).SetAttribute(path, attribute, value, options);
			return path;
		}

		/// <summary>
		/// Reads the value of a file attribute.
		/// 
		/// <para> The {@code attribute} parameter identifies the attribute to be read
		/// and takes the form:
		/// <blockquote>
		/// [<i>view-name</i><b>:</b>]<i>attribute-name</i>
		/// </blockquote>
		/// where square brackets [...] delineate an optional component and the
		/// character {@code ':'} stands for itself.
		/// 
		/// </para>
		/// <para> <i>view-name</i> is the <seealso cref="FileAttributeView#name name"/> of a {@link
		/// FileAttributeView} that identifies a set of file attributes. If not
		/// specified then it defaults to {@code "basic"}, the name of the file
		/// attribute view that identifies the basic set of file attributes common to
		/// many file systems. <i>attribute-name</i> is the name of the attribute.
		/// 
		/// </para>
		/// <para> The {@code options} array may be used to indicate how symbolic links
		/// are handled for the case that the file is a symbolic link. By default,
		/// symbolic links are followed and the file attribute of the final target
		/// of the link is read. If the option {@link LinkOption#NOFOLLOW_LINKS
		/// NOFOLLOW_LINKS} is present then symbolic links are not followed.
		/// 
		/// </para>
		/// <para> <b>Usage Example:</b>
		/// Suppose we require the user ID of the file owner on a system that
		/// supports a "{@code unix}" view:
		/// <pre>
		///    Path path = ...
		///    int uid = (Integer)Files.getAttribute(path, "unix:uid");
		/// </pre>
		/// 
		/// </para>
		/// </summary>
		/// <param name="path">
		///          the path to the file </param>
		/// <param name="attribute">
		///          the attribute to read </param>
		/// <param name="options">
		///          options indicating how symbolic links are handled
		/// </param>
		/// <returns>  the attribute value
		/// </returns>
		/// <exception cref="UnsupportedOperationException">
		///          if the attribute view is not available </exception>
		/// <exception cref="IllegalArgumentException">
		///          if the attribute name is not specified or is not recognized </exception>
		/// <exception cref="IOException">
		///          if an I/O error occurs </exception>
		/// <exception cref="SecurityException">
		///          In the case of the default provider, and a security manager is
		///          installed, its <seealso cref="SecurityManager#checkRead(String) checkRead"/>
		///          method denies read access to the file. If this method is invoked
		///          to read security sensitive attributes then the security manager
		///          may be invoked to check for additional permissions. </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public static Object getAttribute(Path path, String attribute, LinkOption... options) throws java.io.IOException
		public static Object GetAttribute(Path path, String attribute, params LinkOption[] options)
		{
			// only one attribute should be read
			if (attribute.IndexOf('*') >= 0 || attribute.IndexOf(',') >= 0)
			{
				throw new IllegalArgumentException(attribute);
			}
			IDictionary<String, Object> map = ReadAttributes(path, attribute, options);
			Debug.Assert(map.Count == 1);
			String name;
			int pos = attribute.IndexOf(':');
			if (pos == -1)
			{
				name = attribute;
			}
			else
			{
				name = (pos == attribute.Length()) ? "" : attribute.Substring(pos + 1);
			}
			return map[name];
		}

		/// <summary>
		/// Reads a set of file attributes as a bulk operation.
		/// 
		/// <para> The {@code attributes} parameter identifies the attributes to be read
		/// and takes the form:
		/// <blockquote>
		/// [<i>view-name</i><b>:</b>]<i>attribute-list</i>
		/// </blockquote>
		/// where square brackets [...] delineate an optional component and the
		/// character {@code ':'} stands for itself.
		/// 
		/// </para>
		/// <para> <i>view-name</i> is the <seealso cref="FileAttributeView#name name"/> of a {@link
		/// FileAttributeView} that identifies a set of file attributes. If not
		/// specified then it defaults to {@code "basic"}, the name of the file
		/// attribute view that identifies the basic set of file attributes common to
		/// many file systems.
		/// 
		/// </para>
		/// <para> The <i>attribute-list</i> component is a comma separated list of
		/// zero or more names of attributes to read. If the list contains the value
		/// {@code "*"} then all attributes are read. Attributes that are not supported
		/// are ignored and will not be present in the returned map. It is
		/// implementation specific if all attributes are read as an atomic operation
		/// with respect to other file system operations.
		/// 
		/// </para>
		/// <para> The following examples demonstrate possible values for the {@code
		/// attributes} parameter:
		/// 
		/// <blockquote>
		/// <table border="0" summary="Possible values">
		/// <tr>
		///   <td> {@code "*"} </td>
		///   <td> Read all <seealso cref="BasicFileAttributes basic-file-attributes"/>. </td>
		/// </tr>
		/// <tr>
		///   <td> {@code "size,lastModifiedTime,lastAccessTime"} </td>
		///   <td> Reads the file size, last modified time, and last access time
		///     attributes. </td>
		/// </tr>
		/// <tr>
		///   <td> {@code "posix:*"} </td>
		///   <td> Read all <seealso cref="PosixFileAttributes POSIX-file-attributes"/>. </td>
		/// </tr>
		/// <tr>
		///   <td> {@code "posix:permissions,owner,size"} </td>
		///   <td> Reads the POSX file permissions, owner, and file size. </td>
		/// </tr>
		/// </table>
		/// </blockquote>
		/// 
		/// </para>
		/// <para> The {@code options} array may be used to indicate how symbolic links
		/// are handled for the case that the file is a symbolic link. By default,
		/// symbolic links are followed and the file attribute of the final target
		/// of the link is read. If the option {@link LinkOption#NOFOLLOW_LINKS
		/// NOFOLLOW_LINKS} is present then symbolic links are not followed.
		/// 
		/// </para>
		/// </summary>
		/// <param name="path">
		///          the path to the file </param>
		/// <param name="attributes">
		///          the attributes to read </param>
		/// <param name="options">
		///          options indicating how symbolic links are handled
		/// </param>
		/// <returns>  a map of the attributes returned; The map's keys are the
		///          attribute names, its values are the attribute values
		/// </returns>
		/// <exception cref="UnsupportedOperationException">
		///          if the attribute view is not available </exception>
		/// <exception cref="IllegalArgumentException">
		///          if no attributes are specified or an unrecognized attributes is
		///          specified </exception>
		/// <exception cref="IOException">
		///          if an I/O error occurs </exception>
		/// <exception cref="SecurityException">
		///          In the case of the default provider, and a security manager is
		///          installed, its <seealso cref="SecurityManager#checkRead(String) checkRead"/>
		///          method denies read access to the file. If this method is invoked
		///          to read security sensitive attributes then the security manager
		///          may be invoke to check for additional permissions. </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public static java.util.Map<String,Object> readAttributes(Path path, String attributes, LinkOption... options) throws java.io.IOException
		public static IDictionary<String, Object> ReadAttributes(Path path, String attributes, params LinkOption[] options)
		{
			return Provider(path).ReadAttributes(path, attributes, options);
		}

		/// <summary>
		/// Returns a file's POSIX file permissions.
		/// 
		/// <para> The {@code path} parameter is associated with a {@code FileSystem}
		/// that supports the <seealso cref="PosixFileAttributeView"/>. This attribute view
		/// provides access to file attributes commonly associated with files on file
		/// systems used by operating systems that implement the Portable Operating
		/// System Interface (POSIX) family of standards.
		/// 
		/// </para>
		/// <para> The {@code options} array may be used to indicate how symbolic links
		/// are handled for the case that the file is a symbolic link. By default,
		/// symbolic links are followed and the file attribute of the final target
		/// of the link is read. If the option {@link LinkOption#NOFOLLOW_LINKS
		/// NOFOLLOW_LINKS} is present then symbolic links are not followed.
		/// 
		/// </para>
		/// </summary>
		/// <param name="path">
		///          the path to the file </param>
		/// <param name="options">
		///          options indicating how symbolic links are handled
		/// </param>
		/// <returns>  the file permissions
		/// </returns>
		/// <exception cref="UnsupportedOperationException">
		///          if the associated file system does not support the {@code
		///          PosixFileAttributeView} </exception>
		/// <exception cref="IOException">
		///          if an I/O error occurs </exception>
		/// <exception cref="SecurityException">
		///          In the case of the default provider, a security manager is
		///          installed, and it denies <seealso cref="RuntimePermission"/><tt>("accessUserInformation")</tt>
		///          or its <seealso cref="SecurityManager#checkRead(String) checkRead"/> method
		///          denies read access to the file. </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public static java.util.Set<java.nio.file.attribute.PosixFilePermission> getPosixFilePermissions(Path path, LinkOption... options) throws java.io.IOException
		public static Set<PosixFilePermission> GetPosixFilePermissions(Path path, params LinkOption[] options)
		{
			return ReadAttributes(path, typeof(PosixFileAttributes), options).permissions();
		}

		/// <summary>
		/// Sets a file's POSIX permissions.
		/// 
		/// <para> The {@code path} parameter is associated with a {@code FileSystem}
		/// that supports the <seealso cref="PosixFileAttributeView"/>. This attribute view
		/// provides access to file attributes commonly associated with files on file
		/// systems used by operating systems that implement the Portable Operating
		/// System Interface (POSIX) family of standards.
		/// 
		/// </para>
		/// </summary>
		/// <param name="path">
		///          The path to the file </param>
		/// <param name="perms">
		///          The new set of permissions
		/// </param>
		/// <returns>  The path
		/// </returns>
		/// <exception cref="UnsupportedOperationException">
		///          if the associated file system does not support the {@code
		///          PosixFileAttributeView} </exception>
		/// <exception cref="ClassCastException">
		///          if the sets contains elements that are not of type {@code
		///          PosixFilePermission} </exception>
		/// <exception cref="IOException">
		///          if an I/O error occurs </exception>
		/// <exception cref="SecurityException">
		///          In the case of the default provider, and a security manager is
		///          installed, it denies <seealso cref="RuntimePermission"/><tt>("accessUserInformation")</tt>
		///          or its <seealso cref="SecurityManager#checkWrite(String) checkWrite"/>
		///          method denies write access to the file. </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public static Path setPosixFilePermissions(Path path, java.util.Set<java.nio.file.attribute.PosixFilePermission> perms) throws java.io.IOException
		public static Path SetPosixFilePermissions(Path path, Set<PosixFilePermission> perms)
		{
			PosixFileAttributeView view = getFileAttributeView(path, typeof(PosixFileAttributeView));
			if (view == null)
			{
				throw new UnsupportedOperationException();
			}
			view.Permissions = perms;
			return path;
		}

		/// <summary>
		/// Returns the owner of a file.
		/// 
		/// <para> The {@code path} parameter is associated with a file system that
		/// supports <seealso cref="FileOwnerAttributeView"/>. This file attribute view provides
		/// access to a file attribute that is the owner of the file.
		/// 
		/// </para>
		/// </summary>
		/// <param name="path">
		///          The path to the file </param>
		/// <param name="options">
		///          options indicating how symbolic links are handled
		/// </param>
		/// <returns>  A user principal representing the owner of the file
		/// </returns>
		/// <exception cref="UnsupportedOperationException">
		///          if the associated file system does not support the {@code
		///          FileOwnerAttributeView} </exception>
		/// <exception cref="IOException">
		///          if an I/O error occurs </exception>
		/// <exception cref="SecurityException">
		///          In the case of the default provider, and a security manager is
		///          installed, it denies <seealso cref="RuntimePermission"/><tt>("accessUserInformation")</tt>
		///          or its <seealso cref="SecurityManager#checkRead(String) checkRead"/> method
		///          denies read access to the file. </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public static java.nio.file.attribute.UserPrincipal getOwner(Path path, LinkOption... options) throws java.io.IOException
		public static UserPrincipal GetOwner(Path path, params LinkOption[] options)
		{
			FileOwnerAttributeView view = GetFileAttributeView(path, typeof(FileOwnerAttributeView), options);
			if (view == null)
			{
				throw new UnsupportedOperationException();
			}
			return view.Owner;
		}

		/// <summary>
		/// Updates the file owner.
		/// 
		/// <para> The {@code path} parameter is associated with a file system that
		/// supports <seealso cref="FileOwnerAttributeView"/>. This file attribute view provides
		/// access to a file attribute that is the owner of the file.
		/// 
		/// </para>
		/// <para> <b>Usage Example:</b>
		/// Suppose we want to make "joe" the owner of a file:
		/// <pre>
		///     Path path = ...
		///     UserPrincipalLookupService lookupService =
		///         provider(path).getUserPrincipalLookupService();
		///     UserPrincipal joe = lookupService.lookupPrincipalByName("joe");
		///     Files.setOwner(path, joe);
		/// </pre>
		/// 
		/// </para>
		/// </summary>
		/// <param name="path">
		///          The path to the file </param>
		/// <param name="owner">
		///          The new file owner
		/// </param>
		/// <returns>  The path
		/// </returns>
		/// <exception cref="UnsupportedOperationException">
		///          if the associated file system does not support the {@code
		///          FileOwnerAttributeView} </exception>
		/// <exception cref="IOException">
		///          if an I/O error occurs </exception>
		/// <exception cref="SecurityException">
		///          In the case of the default provider, and a security manager is
		///          installed, it denies <seealso cref="RuntimePermission"/><tt>("accessUserInformation")</tt>
		///          or its <seealso cref="SecurityManager#checkWrite(String) checkWrite"/>
		///          method denies write access to the file.
		/// </exception>
		/// <seealso cref= FileSystem#getUserPrincipalLookupService </seealso>
		/// <seealso cref= java.nio.file.attribute.UserPrincipalLookupService </seealso>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public static Path setOwner(Path path, java.nio.file.attribute.UserPrincipal owner) throws java.io.IOException
		public static Path SetOwner(Path path, UserPrincipal owner)
		{
			FileOwnerAttributeView view = getFileAttributeView(path, typeof(FileOwnerAttributeView));
			if (view == null)
			{
				throw new UnsupportedOperationException();
			}
			view.Owner = owner;
			return path;
		}

		/// <summary>
		/// Tests whether a file is a symbolic link.
		/// 
		/// <para> Where it is required to distinguish an I/O exception from the case
		/// that the file is not a symbolic link then the file attributes can be
		/// read with the {@link #readAttributes(Path,Class,LinkOption[])
		/// readAttributes} method and the file type tested with the {@link
		/// BasicFileAttributes#isSymbolicLink} method.
		/// 
		/// </para>
		/// </summary>
		/// <param name="path">  The path to the file
		/// </param>
		/// <returns>  {@code true} if the file is a symbolic link; {@code false} if
		///          the file does not exist, is not a symbolic link, or it cannot
		///          be determined if the file is a symbolic link or not.
		/// </returns>
		/// <exception cref="SecurityException">
		///          In the case of the default provider, and a security manager is
		///          installed, its <seealso cref="SecurityManager#checkRead(String) checkRead"/>
		///          method denies read access to the file. </exception>
		public static bool IsSymbolicLink(Path path)
		{
			try
			{
				return ReadAttributes(path, typeof(BasicFileAttributes), LinkOption.NOFOLLOW_LINKS).SymbolicLink;
			}
			catch (IOException)
			{
				return false;
			}
		}

		/// <summary>
		/// Tests whether a file is a directory.
		/// 
		/// <para> The {@code options} array may be used to indicate how symbolic links
		/// are handled for the case that the file is a symbolic link. By default,
		/// symbolic links are followed and the file attribute of the final target
		/// of the link is read. If the option {@link LinkOption#NOFOLLOW_LINKS
		/// NOFOLLOW_LINKS} is present then symbolic links are not followed.
		/// 
		/// </para>
		/// <para> Where it is required to distinguish an I/O exception from the case
		/// that the file is not a directory then the file attributes can be
		/// read with the {@link #readAttributes(Path,Class,LinkOption[])
		/// readAttributes} method and the file type tested with the {@link
		/// BasicFileAttributes#isDirectory} method.
		/// 
		/// </para>
		/// </summary>
		/// <param name="path">
		///          the path to the file to test </param>
		/// <param name="options">
		///          options indicating how symbolic links are handled
		/// </param>
		/// <returns>  {@code true} if the file is a directory; {@code false} if
		///          the file does not exist, is not a directory, or it cannot
		///          be determined if the file is a directory or not.
		/// </returns>
		/// <exception cref="SecurityException">
		///          In the case of the default provider, and a security manager is
		///          installed, its <seealso cref="SecurityManager#checkRead(String) checkRead"/>
		///          method denies read access to the file. </exception>
		public static bool IsDirectory(Path path, params LinkOption[] options)
		{
			try
			{
				return ReadAttributes(path, typeof(BasicFileAttributes), options).Directory;
			}
			catch (IOException)
			{
				return false;
			}
		}

		/// <summary>
		/// Tests whether a file is a regular file with opaque content.
		/// 
		/// <para> The {@code options} array may be used to indicate how symbolic links
		/// are handled for the case that the file is a symbolic link. By default,
		/// symbolic links are followed and the file attribute of the final target
		/// of the link is read. If the option {@link LinkOption#NOFOLLOW_LINKS
		/// NOFOLLOW_LINKS} is present then symbolic links are not followed.
		/// 
		/// </para>
		/// <para> Where it is required to distinguish an I/O exception from the case
		/// that the file is not a regular file then the file attributes can be
		/// read with the {@link #readAttributes(Path,Class,LinkOption[])
		/// readAttributes} method and the file type tested with the {@link
		/// BasicFileAttributes#isRegularFile} method.
		/// 
		/// </para>
		/// </summary>
		/// <param name="path">
		///          the path to the file </param>
		/// <param name="options">
		///          options indicating how symbolic links are handled
		/// </param>
		/// <returns>  {@code true} if the file is a regular file; {@code false} if
		///          the file does not exist, is not a regular file, or it
		///          cannot be determined if the file is a regular file or not.
		/// </returns>
		/// <exception cref="SecurityException">
		///          In the case of the default provider, and a security manager is
		///          installed, its <seealso cref="SecurityManager#checkRead(String) checkRead"/>
		///          method denies read access to the file. </exception>
		public static bool IsRegularFile(Path path, params LinkOption[] options)
		{
			try
			{
				return ReadAttributes(path, typeof(BasicFileAttributes), options).RegularFile;
			}
			catch (IOException)
			{
				return false;
			}
		}

		/// <summary>
		/// Returns a file's last modified time.
		/// 
		/// <para> The {@code options} array may be used to indicate how symbolic links
		/// are handled for the case that the file is a symbolic link. By default,
		/// symbolic links are followed and the file attribute of the final target
		/// of the link is read. If the option {@link LinkOption#NOFOLLOW_LINKS
		/// NOFOLLOW_LINKS} is present then symbolic links are not followed.
		/// 
		/// </para>
		/// </summary>
		/// <param name="path">
		///          the path to the file </param>
		/// <param name="options">
		///          options indicating how symbolic links are handled
		/// </param>
		/// <returns>  a {@code FileTime} representing the time the file was last
		///          modified, or an implementation specific default when a time
		///          stamp to indicate the time of last modification is not supported
		///          by the file system
		/// </returns>
		/// <exception cref="IOException">
		///          if an I/O error occurs </exception>
		/// <exception cref="SecurityException">
		///          In the case of the default provider, and a security manager is
		///          installed, its <seealso cref="SecurityManager#checkRead(String) checkRead"/>
		///          method denies read access to the file.
		/// </exception>
		/// <seealso cref= BasicFileAttributes#lastModifiedTime </seealso>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public static java.nio.file.attribute.FileTime getLastModifiedTime(Path path, LinkOption... options) throws java.io.IOException
		public static FileTime GetLastModifiedTime(Path path, params LinkOption[] options)
		{
			return ReadAttributes(path, typeof(BasicFileAttributes), options).lastModifiedTime();
		}

		/// <summary>
		/// Updates a file's last modified time attribute. The file time is converted
		/// to the epoch and precision supported by the file system. Converting from
		/// finer to coarser granularities result in precision loss. The behavior of
		/// this method when attempting to set the last modified time when it is not
		/// supported by the file system or is outside the range supported by the
		/// underlying file store is not defined. It may or not fail by throwing an
		/// {@code IOException}.
		/// 
		/// <para> <b>Usage Example:</b>
		/// Suppose we want to set the last modified time to the current time:
		/// <pre>
		///    Path path = ...
		///    FileTime now = FileTime.fromMillis(System.currentTimeMillis());
		///    Files.setLastModifiedTime(path, now);
		/// </pre>
		/// 
		/// </para>
		/// </summary>
		/// <param name="path">
		///          the path to the file </param>
		/// <param name="time">
		///          the new last modified time
		/// </param>
		/// <returns>  the path
		/// </returns>
		/// <exception cref="IOException">
		///          if an I/O error occurs </exception>
		/// <exception cref="SecurityException">
		///          In the case of the default provider, the security manager's {@link
		///          SecurityManager#checkWrite(String) checkWrite} method is invoked
		///          to check write access to file
		/// </exception>
		/// <seealso cref= BasicFileAttributeView#setTimes </seealso>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public static Path setLastModifiedTime(Path path, java.nio.file.attribute.FileTime time) throws java.io.IOException
		public static Path SetLastModifiedTime(Path path, FileTime time)
		{
			getFileAttributeView(path, typeof(BasicFileAttributeView)).setTimes(time, null, null);
			return path;
		}

		/// <summary>
		/// Returns the size of a file (in bytes). The size may differ from the
		/// actual size on the file system due to compression, support for sparse
		/// files, or other reasons. The size of files that are not {@link
		/// #isRegularFile regular} files is implementation specific and
		/// therefore unspecified.
		/// </summary>
		/// <param name="path">
		///          the path to the file
		/// </param>
		/// <returns>  the file size, in bytes
		/// </returns>
		/// <exception cref="IOException">
		///          if an I/O error occurs </exception>
		/// <exception cref="SecurityException">
		///          In the case of the default provider, and a security manager is
		///          installed, its <seealso cref="SecurityManager#checkRead(String) checkRead"/>
		///          method denies read access to the file.
		/// </exception>
		/// <seealso cref= BasicFileAttributes#size </seealso>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public static long size(Path path) throws java.io.IOException
		public static long Size(Path path)
		{
			return readAttributes(path, typeof(BasicFileAttributes)).size();
		}

		// -- Accessibility --

		/// <summary>
		/// Returns {@code false} if NOFOLLOW_LINKS is present.
		/// </summary>
		private static bool FollowLinks(params LinkOption[] options)
		{
			bool followLinks = true;
			foreach (LinkOption opt in options)
			{
				if (opt == LinkOption.NOFOLLOW_LINKS)
				{
					followLinks = false;
					continue;
				}
				if (opt == null)
				{
					throw new NullPointerException();
				}
				throw new AssertionError("Should not get here");
			}
			return followLinks;
		}

		/// <summary>
		/// Tests whether a file exists.
		/// 
		/// <para> The {@code options} parameter may be used to indicate how symbolic links
		/// are handled for the case that the file is a symbolic link. By default,
		/// symbolic links are followed. If the option {@link LinkOption#NOFOLLOW_LINKS
		/// NOFOLLOW_LINKS} is present then symbolic links are not followed.
		/// 
		/// </para>
		/// <para> Note that the result of this method is immediately outdated. If this
		/// method indicates the file exists then there is no guarantee that a
		/// subsequence access will succeed. Care should be taken when using this
		/// method in security sensitive applications.
		/// 
		/// </para>
		/// </summary>
		/// <param name="path">
		///          the path to the file to test </param>
		/// <param name="options">
		///          options indicating how symbolic links are handled
		/// . </param>
		/// <returns>  {@code true} if the file exists; {@code false} if the file does
		///          not exist or its existence cannot be determined.
		/// </returns>
		/// <exception cref="SecurityException">
		///          In the case of the default provider, the {@link
		///          SecurityManager#checkRead(String)} is invoked to check
		///          read access to the file.
		/// </exception>
		/// <seealso cref= #notExists </seealso>
		public static bool Exists(Path path, params LinkOption[] options)
		{
			try
			{
				if (FollowLinks(options))
				{
					Provider(path).checkAccess(path);
				}
				else
				{
					// attempt to read attributes without following links
					ReadAttributes(path, typeof(BasicFileAttributes), LinkOption.NOFOLLOW_LINKS);
				}
				// file exists
				return true;
			}
			catch (IOException)
			{
				// does not exist or unable to determine if file exists
				return false;
			}

		}

		/// <summary>
		/// Tests whether the file located by this path does not exist. This method
		/// is intended for cases where it is required to take action when it can be
		/// confirmed that a file does not exist.
		/// 
		/// <para> The {@code options} parameter may be used to indicate how symbolic links
		/// are handled for the case that the file is a symbolic link. By default,
		/// symbolic links are followed. If the option {@link LinkOption#NOFOLLOW_LINKS
		/// NOFOLLOW_LINKS} is present then symbolic links are not followed.
		/// 
		/// </para>
		/// <para> Note that this method is not the complement of the {@link #exists
		/// exists} method. Where it is not possible to determine if a file exists
		/// or not then both methods return {@code false}. As with the {@code exists}
		/// method, the result of this method is immediately outdated. If this
		/// method indicates the file does exist then there is no guarantee that a
		/// subsequence attempt to create the file will succeed. Care should be taken
		/// when using this method in security sensitive applications.
		/// 
		/// </para>
		/// </summary>
		/// <param name="path">
		///          the path to the file to test </param>
		/// <param name="options">
		///          options indicating how symbolic links are handled
		/// </param>
		/// <returns>  {@code true} if the file does not exist; {@code false} if the
		///          file exists or its existence cannot be determined
		/// </returns>
		/// <exception cref="SecurityException">
		///          In the case of the default provider, the {@link
		///          SecurityManager#checkRead(String)} is invoked to check
		///          read access to the file. </exception>
		public static bool NotExists(Path path, params LinkOption[] options)
		{
			try
			{
				if (FollowLinks(options))
				{
					Provider(path).checkAccess(path);
				}
				else
				{
					// attempt to read attributes without following links
					ReadAttributes(path, typeof(BasicFileAttributes), LinkOption.NOFOLLOW_LINKS);
				}
				// file exists
				return false;
			}
			catch (NoSuchFileException)
			{
				// file confirmed not to exist
				return true;
			}
			catch (IOException)
			{
				return false;
			}
		}

		/// <summary>
		/// Used by isReadbale, isWritable, isExecutable to test access to a file.
		/// </summary>
		private static bool IsAccessible(Path path, params AccessMode[] modes)
		{
			try
			{
				Provider(path).CheckAccess(path, modes);
				return true;
			}
			catch (IOException)
			{
				return false;
			}
		}

		/// <summary>
		/// Tests whether a file is readable. This method checks that a file exists
		/// and that this Java virtual machine has appropriate privileges that would
		/// allow it open the file for reading. Depending on the implementation, this
		/// method may require to read file permissions, access control lists, or
		/// other file attributes in order to check the effective access to the file.
		/// Consequently, this method may not be atomic with respect to other file
		/// system operations.
		/// 
		/// <para> Note that the result of this method is immediately outdated, there is
		/// no guarantee that a subsequent attempt to open the file for reading will
		/// succeed (or even that it will access the same file). Care should be taken
		/// when using this method in security sensitive applications.
		/// 
		/// </para>
		/// </summary>
		/// <param name="path">
		///          the path to the file to check
		/// </param>
		/// <returns>  {@code true} if the file exists and is readable; {@code false}
		///          if the file does not exist, read access would be denied because
		///          the Java virtual machine has insufficient privileges, or access
		///          cannot be determined
		/// </returns>
		/// <exception cref="SecurityException">
		///          In the case of the default provider, and a security manager is
		///          installed, the <seealso cref="SecurityManager#checkRead(String) checkRead"/>
		///          is invoked to check read access to the file. </exception>
		public static bool IsReadable(Path path)
		{
			return IsAccessible(path, AccessMode.READ);
		}

		/// <summary>
		/// Tests whether a file is writable. This method checks that a file exists
		/// and that this Java virtual machine has appropriate privileges that would
		/// allow it open the file for writing. Depending on the implementation, this
		/// method may require to read file permissions, access control lists, or
		/// other file attributes in order to check the effective access to the file.
		/// Consequently, this method may not be atomic with respect to other file
		/// system operations.
		/// 
		/// <para> Note that result of this method is immediately outdated, there is no
		/// guarantee that a subsequent attempt to open the file for writing will
		/// succeed (or even that it will access the same file). Care should be taken
		/// when using this method in security sensitive applications.
		/// 
		/// </para>
		/// </summary>
		/// <param name="path">
		///          the path to the file to check
		/// </param>
		/// <returns>  {@code true} if the file exists and is writable; {@code false}
		///          if the file does not exist, write access would be denied because
		///          the Java virtual machine has insufficient privileges, or access
		///          cannot be determined
		/// </returns>
		/// <exception cref="SecurityException">
		///          In the case of the default provider, and a security manager is
		///          installed, the <seealso cref="SecurityManager#checkWrite(String) checkWrite"/>
		///          is invoked to check write access to the file. </exception>
		public static bool IsWritable(Path path)
		{
			return IsAccessible(path, AccessMode.WRITE);
		}

		/// <summary>
		/// Tests whether a file is executable. This method checks that a file exists
		/// and that this Java virtual machine has appropriate privileges to {@link
		/// Runtime#exec execute} the file. The semantics may differ when checking
		/// access to a directory. For example, on UNIX systems, checking for
		/// execute access checks that the Java virtual machine has permission to
		/// search the directory in order to access file or subdirectories.
		/// 
		/// <para> Depending on the implementation, this method may require to read file
		/// permissions, access control lists, or other file attributes in order to
		/// check the effective access to the file. Consequently, this method may not
		/// be atomic with respect to other file system operations.
		/// 
		/// </para>
		/// <para> Note that the result of this method is immediately outdated, there is
		/// no guarantee that a subsequent attempt to execute the file will succeed
		/// (or even that it will access the same file). Care should be taken when
		/// using this method in security sensitive applications.
		/// 
		/// </para>
		/// </summary>
		/// <param name="path">
		///          the path to the file to check
		/// </param>
		/// <returns>  {@code true} if the file exists and is executable; {@code false}
		///          if the file does not exist, execute access would be denied because
		///          the Java virtual machine has insufficient privileges, or access
		///          cannot be determined
		/// </returns>
		/// <exception cref="SecurityException">
		///          In the case of the default provider, and a security manager is
		///          installed, the {@link SecurityManager#checkExec(String)
		///          checkExec} is invoked to check execute access to the file. </exception>
		public static bool IsExecutable(Path path)
		{
			return IsAccessible(path, AccessMode.EXECUTE);
		}

		// -- Recursive operations --

		/// <summary>
		/// Walks a file tree.
		/// 
		/// <para> This method walks a file tree rooted at a given starting file. The
		/// file tree traversal is <em>depth-first</em> with the given {@link
		/// FileVisitor} invoked for each file encountered. File tree traversal
		/// completes when all accessible files in the tree have been visited, or a
		/// visit method returns a result of {@link FileVisitResult#TERMINATE
		/// TERMINATE}. Where a visit method terminates due an {@code IOException},
		/// an uncaught error, or runtime exception, then the traversal is terminated
		/// and the error or exception is propagated to the caller of this method.
		/// 
		/// </para>
		/// <para> For each file encountered this method attempts to read its {@link
		/// java.nio.file.attribute.BasicFileAttributes}. If the file is not a
		/// directory then the <seealso cref="FileVisitor#visitFile visitFile"/> method is
		/// invoked with the file attributes. If the file attributes cannot be read,
		/// due to an I/O exception, then the {@link FileVisitor#visitFileFailed
		/// visitFileFailed} method is invoked with the I/O exception.
		/// 
		/// </para>
		/// <para> Where the file is a directory, and the directory could not be opened,
		/// then the {@code visitFileFailed} method is invoked with the I/O exception,
		/// after which, the file tree walk continues, by default, at the next
		/// <em>sibling</em> of the directory.
		/// 
		/// </para>
		/// <para> Where the directory is opened successfully, then the entries in the
		/// directory, and their <em>descendants</em> are visited. When all entries
		/// have been visited, or an I/O error occurs during iteration of the
		/// directory, then the directory is closed and the visitor's {@link
		/// FileVisitor#postVisitDirectory postVisitDirectory} method is invoked.
		/// The file tree walk then continues, by default, at the next <em>sibling</em>
		/// of the directory.
		/// 
		/// </para>
		/// <para> By default, symbolic links are not automatically followed by this
		/// method. If the {@code options} parameter contains the {@link
		/// FileVisitOption#FOLLOW_LINKS FOLLOW_LINKS} option then symbolic links are
		/// followed. When following links, and the attributes of the target cannot
		/// be read, then this method attempts to get the {@code BasicFileAttributes}
		/// of the link. If they can be read then the {@code visitFile} method is
		/// invoked with the attributes of the link (otherwise the {@code visitFileFailed}
		/// method is invoked as specified above).
		/// 
		/// </para>
		/// <para> If the {@code options} parameter contains the {@link
		/// FileVisitOption#FOLLOW_LINKS FOLLOW_LINKS} option then this method keeps
		/// track of directories visited so that cycles can be detected. A cycle
		/// arises when there is an entry in a directory that is an ancestor of the
		/// directory. Cycle detection is done by recording the {@link
		/// java.nio.file.attribute.BasicFileAttributes#fileKey file-key} of directories,
		/// or if file keys are not available, by invoking the {@link #isSameFile
		/// isSameFile} method to test if a directory is the same file as an
		/// ancestor. When a cycle is detected it is treated as an I/O error, and the
		/// <seealso cref="FileVisitor#visitFileFailed visitFileFailed"/> method is invoked with
		/// an instance of <seealso cref="FileSystemLoopException"/>.
		/// 
		/// </para>
		/// <para> The {@code maxDepth} parameter is the maximum number of levels of
		/// directories to visit. A value of {@code 0} means that only the starting
		/// file is visited, unless denied by the security manager. A value of
		/// <seealso cref="Integer#MAX_VALUE MAX_VALUE"/> may be used to indicate that all
		/// levels should be visited. The {@code visitFile} method is invoked for all
		/// files, including directories, encountered at {@code maxDepth}, unless the
		/// basic file attributes cannot be read, in which case the {@code
		/// visitFileFailed} method is invoked.
		/// 
		/// </para>
		/// <para> If a visitor returns a result of {@code null} then {@code
		/// NullPointerException} is thrown.
		/// 
		/// </para>
		/// <para> When a security manager is installed and it denies access to a file
		/// (or directory), then it is ignored and the visitor is not invoked for
		/// that file (or directory).
		/// 
		/// </para>
		/// </summary>
		/// <param name="start">
		///          the starting file </param>
		/// <param name="options">
		///          options to configure the traversal </param>
		/// <param name="maxDepth">
		///          the maximum number of directory levels to visit </param>
		/// <param name="visitor">
		///          the file visitor to invoke for each file
		/// </param>
		/// <returns>  the starting file
		/// </returns>
		/// <exception cref="IllegalArgumentException">
		///          if the {@code maxDepth} parameter is negative </exception>
		/// <exception cref="SecurityException">
		///          If the security manager denies access to the starting file.
		///          In the case of the default provider, the {@link
		///          SecurityManager#checkRead(String) checkRead} method is invoked
		///          to check read access to the directory. </exception>
		/// <exception cref="IOException">
		///          if an I/O error is thrown by a visitor method </exception>
//JAVA TO C# CONVERTER TODO TASK: There is no .NET equivalent to the Java 'super' constraint:
//ORIGINAL LINE: public static Path walkFileTree(Path start, java.util.Set<FileVisitOption> options, int maxDepth, FileVisitor<? base Path> visitor) throws java.io.IOException
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
		public static Path walkFileTree<T1>(Path start, Set<FileVisitOption> options, int maxDepth, FileVisitor<T1> visitor)
		{
			/// <summary>
			/// Create a FileTreeWalker to walk the file tree, invoking the visitor
			/// for each event.
			/// </summary>
			using (FileTreeWalker walker = new FileTreeWalker(options, maxDepth))
			{
				FileTreeWalker.Event ev = walker.walk(start);
				do
				{
					FileVisitResult result;
					switch (ev.Type())
					{
						case ENTRY :
							IOException ioe = ev.IoeException();
							if (ioe == null)
							{
								Debug.Assert(ev.Attributes() != null);
								result = visitor.VisitFile(ev.File(), ev.Attributes());
							}
							else
							{
								result = visitor.VisitFileFailed(ev.File(), ioe);
							}
							break;

						case START_DIRECTORY :
							result = visitor.PreVisitDirectory(ev.File(), ev.Attributes());

							// if SKIP_SIBLINGS and SKIP_SUBTREE is returned then
							// there shouldn't be any more events for the current
							// directory.
							if (result == FileVisitResult.SKIP_SUBTREE || result == FileVisitResult.SKIP_SIBLINGS)
							{
								walker.pop();
							}
							break;

						case END_DIRECTORY :
							result = visitor.PostVisitDirectory(ev.File(), ev.IoeException());

							// SKIP_SIBLINGS is a no-op for postVisitDirectory
							if (result == FileVisitResult.SKIP_SIBLINGS)
							{
								result = FileVisitResult.CONTINUE;
							}
							break;

						default :
							throw new AssertionError("Should not get here");
					}

					if (Objects.RequireNonNull(result) != FileVisitResult.CONTINUE)
					{
						if (result == FileVisitResult.TERMINATE)
						{
							break;
						}
						else if (result == FileVisitResult.SKIP_SIBLINGS)
						{
							walker.skipRemainingSiblings();
						}
					}
					ev = walker.next();
				} while (ev != null);
			}

			return start;
		}

		/// <summary>
		/// Walks a file tree.
		/// 
		/// <para> This method works as if invoking it were equivalent to evaluating the
		/// expression:
		/// <blockquote><pre>
		/// walkFileTree(start, EnumSet.noneOf(FileVisitOption.class), Integer.MAX_VALUE, visitor)
		/// </pre></blockquote>
		/// In other words, it does not follow symbolic links, and visits all levels
		/// of the file tree.
		/// 
		/// </para>
		/// </summary>
		/// <param name="start">
		///          the starting file </param>
		/// <param name="visitor">
		///          the file visitor to invoke for each file
		/// </param>
		/// <returns>  the starting file
		/// </returns>
		/// <exception cref="SecurityException">
		///          If the security manager denies access to the starting file.
		///          In the case of the default provider, the {@link
		///          SecurityManager#checkRead(String) checkRead} method is invoked
		///          to check read access to the directory. </exception>
		/// <exception cref="IOException">
		///          if an I/O error is thrown by a visitor method </exception>
//JAVA TO C# CONVERTER TODO TASK: There is no .NET equivalent to the Java 'super' constraint:
//ORIGINAL LINE: public static Path walkFileTree(Path start, FileVisitor<? base Path> visitor) throws java.io.IOException
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
		public static Path walkFileTree<T1>(Path start, FileVisitor<T1> visitor)
		{
			return WalkFileTree(start, EnumSet.NoneOf(typeof(FileVisitOption)), Integer.MaxValue, visitor);
		}


		// -- Utility methods for simple usages --

		// buffer size used for reading and writing
		private const int BUFFER_SIZE = 8192;

		/// <summary>
		/// Opens a file for reading, returning a {@code BufferedReader} that may be
		/// used to read text from the file in an efficient manner. Bytes from the
		/// file are decoded into characters using the specified charset. Reading
		/// commences at the beginning of the file.
		/// 
		/// <para> The {@code Reader} methods that read from the file throw {@code
		/// IOException} if a malformed or unmappable byte sequence is read.
		/// 
		/// </para>
		/// </summary>
		/// <param name="path">
		///          the path to the file </param>
		/// <param name="cs">
		///          the charset to use for decoding
		/// </param>
		/// <returns>  a new buffered reader, with default buffer size, to read text
		///          from the file
		/// </returns>
		/// <exception cref="IOException">
		///          if an I/O error occurs opening the file </exception>
		/// <exception cref="SecurityException">
		///          In the case of the default provider, and a security manager is
		///          installed, the <seealso cref="SecurityManager#checkRead(String) checkRead"/>
		///          method is invoked to check read access to the file.
		/// </exception>
		/// <seealso cref= #readAllLines </seealso>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public static java.io.BufferedReader newBufferedReader(Path path, java.nio.charset.Charset cs) throws java.io.IOException
		public static BufferedReader NewBufferedReader(Path path, Charset cs)
		{
			CharsetDecoder decoder = cs.NewDecoder();
			Reader reader = new InputStreamReader(newInputStream(path), decoder);
			return new BufferedReader(reader);
		}

		/// <summary>
		/// Opens a file for reading, returning a {@code BufferedReader} to read text
		/// from the file in an efficient manner. Bytes from the file are decoded into
		/// characters using the <seealso cref="StandardCharsets#UTF_8 UTF-8"/> {@link Charset
		/// charset}.
		/// 
		/// <para> This method works as if invoking it were equivalent to evaluating the
		/// expression:
		/// <pre>{@code
		/// Files.newBufferedReader(path, StandardCharsets.UTF_8)
		/// }</pre>
		/// 
		/// </para>
		/// </summary>
		/// <param name="path">
		///          the path to the file
		/// </param>
		/// <returns>  a new buffered reader, with default buffer size, to read text
		///          from the file
		/// </returns>
		/// <exception cref="IOException">
		///          if an I/O error occurs opening the file </exception>
		/// <exception cref="SecurityException">
		///          In the case of the default provider, and a security manager is
		///          installed, the <seealso cref="SecurityManager#checkRead(String) checkRead"/>
		///          method is invoked to check read access to the file.
		/// 
		/// @since 1.8 </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public static java.io.BufferedReader newBufferedReader(Path path) throws java.io.IOException
		public static BufferedReader NewBufferedReader(Path path)
		{
			return NewBufferedReader(path, StandardCharsets.UTF_8);
		}

		/// <summary>
		/// Opens or creates a file for writing, returning a {@code BufferedWriter}
		/// that may be used to write text to the file in an efficient manner.
		/// The {@code options} parameter specifies how the the file is created or
		/// opened. If no options are present then this method works as if the {@link
		/// StandardOpenOption#CREATE CREATE}, {@link
		/// StandardOpenOption#TRUNCATE_EXISTING TRUNCATE_EXISTING}, and {@link
		/// StandardOpenOption#WRITE WRITE} options are present. In other words, it
		/// opens the file for writing, creating the file if it doesn't exist, or
		/// initially truncating an existing <seealso cref="#isRegularFile regular-file"/> to
		/// a size of {@code 0} if it exists.
		/// 
		/// <para> The {@code Writer} methods to write text throw {@code IOException}
		/// if the text cannot be encoded using the specified charset.
		/// 
		/// </para>
		/// </summary>
		/// <param name="path">
		///          the path to the file </param>
		/// <param name="cs">
		///          the charset to use for encoding </param>
		/// <param name="options">
		///          options specifying how the file is opened
		/// </param>
		/// <returns>  a new buffered writer, with default buffer size, to write text
		///          to the file
		/// </returns>
		/// <exception cref="IOException">
		///          if an I/O error occurs opening or creating the file </exception>
		/// <exception cref="UnsupportedOperationException">
		///          if an unsupported option is specified </exception>
		/// <exception cref="SecurityException">
		///          In the case of the default provider, and a security manager is
		///          installed, the <seealso cref="SecurityManager#checkWrite(String) checkWrite"/>
		///          method is invoked to check write access to the file.
		/// </exception>
		/// <seealso cref= #write(Path,Iterable,Charset,OpenOption[]) </seealso>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public static java.io.BufferedWriter newBufferedWriter(Path path, java.nio.charset.Charset cs, OpenOption... options) throws java.io.IOException
		public static BufferedWriter NewBufferedWriter(Path path, Charset cs, params OpenOption[] options)
		{
			CharsetEncoder encoder = cs.NewEncoder();
			Writer writer = new OutputStreamWriter(NewOutputStream(path, options), encoder);
			return new BufferedWriter(writer);
		}

		/// <summary>
		/// Opens or creates a file for writing, returning a {@code BufferedWriter}
		/// to write text to the file in an efficient manner. The text is encoded
		/// into bytes for writing using the <seealso cref="StandardCharsets#UTF_8 UTF-8"/>
		/// <seealso cref="Charset charset"/>.
		/// 
		/// <para> This method works as if invoking it were equivalent to evaluating the
		/// expression:
		/// <pre>{@code
		/// Files.newBufferedWriter(path, StandardCharsets.UTF_8, options)
		/// }</pre>
		/// 
		/// </para>
		/// </summary>
		/// <param name="path">
		///          the path to the file </param>
		/// <param name="options">
		///          options specifying how the file is opened
		/// </param>
		/// <returns>  a new buffered writer, with default buffer size, to write text
		///          to the file
		/// </returns>
		/// <exception cref="IOException">
		///          if an I/O error occurs opening or creating the file </exception>
		/// <exception cref="UnsupportedOperationException">
		///          if an unsupported option is specified </exception>
		/// <exception cref="SecurityException">
		///          In the case of the default provider, and a security manager is
		///          installed, the <seealso cref="SecurityManager#checkWrite(String) checkWrite"/>
		///          method is invoked to check write access to the file.
		/// 
		/// @since 1.8 </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public static java.io.BufferedWriter newBufferedWriter(Path path, OpenOption... options) throws java.io.IOException
		public static BufferedWriter NewBufferedWriter(Path path, params OpenOption[] options)
		{
			return NewBufferedWriter(path, StandardCharsets.UTF_8, options);
		}

		/// <summary>
		/// Reads all bytes from an input stream and writes them to an output stream.
		/// </summary>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: private static long copy(java.io.InputStream source, java.io.OutputStream sink) throws java.io.IOException
		private static long Copy(InputStream source, OutputStream sink)
		{
			long nread = 0L;
			sbyte[] buf = new sbyte[BUFFER_SIZE];
			int n;
			while ((n = source.Read(buf)) > 0)
			{
				sink.Write(buf, 0, n);
				nread += n;
			}
			return nread;
		}

		/// <summary>
		/// Copies all bytes from an input stream to a file. On return, the input
		/// stream will be at end of stream.
		/// 
		/// <para> By default, the copy fails if the target file already exists or is a
		/// symbolic link. If the {@link StandardCopyOption#REPLACE_EXISTING
		/// REPLACE_EXISTING} option is specified, and the target file already exists,
		/// then it is replaced if it is not a non-empty directory. If the target
		/// file exists and is a symbolic link, then the symbolic link is replaced.
		/// In this release, the {@code REPLACE_EXISTING} option is the only option
		/// required to be supported by this method. Additional options may be
		/// supported in future releases.
		/// 
		/// </para>
		/// <para>  If an I/O error occurs reading from the input stream or writing to
		/// the file, then it may do so after the target file has been created and
		/// after some bytes have been read or written. Consequently the input
		/// stream may not be at end of stream and may be in an inconsistent state.
		/// It is strongly recommended that the input stream be promptly closed if an
		/// I/O error occurs.
		/// 
		/// </para>
		/// <para> This method may block indefinitely reading from the input stream (or
		/// writing to the file). The behavior for the case that the input stream is
		/// <i>asynchronously closed</i> or the thread interrupted during the copy is
		/// highly input stream and file system provider specific and therefore not
		/// specified.
		/// 
		/// </para>
		/// <para> <b>Usage example</b>: Suppose we want to capture a web page and save
		/// it to a file:
		/// <pre>
		///     Path path = ...
		///     URI u = URI.create("http://java.sun.com/");
		///     try (InputStream in = u.toURL().openStream()) {
		///         Files.copy(in, path);
		///     }
		/// </pre>
		/// 
		/// </para>
		/// </summary>
		/// <param name="in">
		///          the input stream to read from </param>
		/// <param name="target">
		///          the path to the file </param>
		/// <param name="options">
		///          options specifying how the copy should be done
		/// </param>
		/// <returns>  the number of bytes read or written
		/// </returns>
		/// <exception cref="IOException">
		///          if an I/O error occurs when reading or writing </exception>
		/// <exception cref="FileAlreadyExistsException">
		///          if the target file exists but cannot be replaced because the
		///          {@code REPLACE_EXISTING} option is not specified <i>(optional
		///          specific exception)</i> </exception>
		/// <exception cref="DirectoryNotEmptyException">
		///          the {@code REPLACE_EXISTING} option is specified but the file
		///          cannot be replaced because it is a non-empty directory
		///          <i>(optional specific exception)</i>     * </exception>
		/// <exception cref="UnsupportedOperationException">
		///          if {@code options} contains a copy option that is not supported </exception>
		/// <exception cref="SecurityException">
		///          In the case of the default provider, and a security manager is
		///          installed, the <seealso cref="SecurityManager#checkWrite(String) checkWrite"/>
		///          method is invoked to check write access to the file. Where the
		///          {@code REPLACE_EXISTING} option is specified, the security
		///          manager's <seealso cref="SecurityManager#checkDelete(String) checkDelete"/>
		///          method is invoked to check that an existing file can be deleted. </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public static long copy(java.io.InputStream in, Path target, CopyOption... options) throws java.io.IOException
		public static long Copy(InputStream @in, Path target, params CopyOption[] options)
		{
			// ensure not null before opening file
			Objects.RequireNonNull(@in);

			// check for REPLACE_EXISTING
			bool replaceExisting = false;
			foreach (CopyOption opt in options)
			{
				if (opt == StandardCopyOption.REPLACE_EXISTING)
				{
					replaceExisting = true;
				}
				else
				{
					if (opt == null)
					{
						throw new NullPointerException("options contains 'null'");
					}
					else
					{
						throw new UnsupportedOperationException(opt + " not supported");
					}
				}
			}

			// attempt to delete an existing file
			SecurityException se = null;
			if (replaceExisting)
			{
				try
				{
					DeleteIfExists(target);
				}
				catch (SecurityException x)
				{
					se = x;
				}
			}

			// attempt to create target file. If it fails with
			// FileAlreadyExistsException then it may be because the security
			// manager prevented us from deleting the file, in which case we just
			// throw the SecurityException.
			OutputStream ostream;
			try
			{
				ostream = newOutputStream(target, StandardOpenOption.CREATE_NEW, StandardOpenOption.WRITE);
			}
			catch (FileAlreadyExistsException x)
			{
				if (se != null)
				{
					throw se;
				}
				// someone else won the race and created the file
				throw x;
			}

			// do the copy
			using (OutputStream @out = ostream)
			{
				return Copy(@in, @out);
			}
		}

		/// <summary>
		/// Copies all bytes from a file to an output stream.
		/// 
		/// <para> If an I/O error occurs reading from the file or writing to the output
		/// stream, then it may do so after some bytes have been read or written.
		/// Consequently the output stream may be in an inconsistent state. It is
		/// strongly recommended that the output stream be promptly closed if an I/O
		/// error occurs.
		/// 
		/// </para>
		/// <para> This method may block indefinitely writing to the output stream (or
		/// reading from the file). The behavior for the case that the output stream
		/// is <i>asynchronously closed</i> or the thread interrupted during the copy
		/// is highly output stream and file system provider specific and therefore
		/// not specified.
		/// 
		/// </para>
		/// <para> Note that if the given output stream is <seealso cref="java.io.Flushable"/>
		/// then its <seealso cref="java.io.Flushable#flush flush"/> method may need to invoked
		/// after this method completes so as to flush any buffered output.
		/// 
		/// </para>
		/// </summary>
		/// <param name="source">
		///          the  path to the file </param>
		/// <param name="out">
		///          the output stream to write to
		/// </param>
		/// <returns>  the number of bytes read or written
		/// </returns>
		/// <exception cref="IOException">
		///          if an I/O error occurs when reading or writing </exception>
		/// <exception cref="SecurityException">
		///          In the case of the default provider, and a security manager is
		///          installed, the <seealso cref="SecurityManager#checkRead(String) checkRead"/>
		///          method is invoked to check read access to the file. </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public static long copy(Path source, java.io.OutputStream out) throws java.io.IOException
		public static long Copy(Path source, OutputStream @out)
		{
			// ensure not null before opening file
			Objects.RequireNonNull(@out);

			using (InputStream @in = newInputStream(source))
			{
				return Copy(@in, @out);
			}
		}

		/// <summary>
		/// The maximum size of array to allocate.
		/// Some VMs reserve some header words in an array.
		/// Attempts to allocate larger arrays may result in
		/// OutOfMemoryError: Requested array size exceeds VM limit
		/// </summary>
		private static readonly int MAX_BUFFER_SIZE = Integer.MaxValue - 8;

		/// <summary>
		/// Reads all the bytes from an input stream. Uses {@code initialSize} as a hint
		/// about how many bytes the stream will have.
		/// </summary>
		/// <param name="source">
		///          the input stream to read from </param>
		/// <param name="initialSize">
		///          the initial size of the byte array to allocate
		/// </param>
		/// <returns>  a byte array containing the bytes read from the file
		/// </returns>
		/// <exception cref="IOException">
		///          if an I/O error occurs reading from the stream </exception>
		/// <exception cref="OutOfMemoryError">
		///          if an array of the required size cannot be allocated </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: private static byte[] read(java.io.InputStream source, int initialSize) throws java.io.IOException
		private static sbyte[] Read(InputStream source, int initialSize)
		{
			int capacity = initialSize;
			sbyte[] buf = new sbyte[capacity];
			int nread = 0;
			int n;
			for (;;)
			{
				// read to EOF which may read more or less than initialSize (eg: file
				// is truncated while we are reading)
				while ((n = source.Read(buf, nread, capacity - nread)) > 0)
				{
					nread += n;
				}

				// if last call to source.read() returned -1, we are done
				// otherwise, try to read one more byte; if that failed we're done too
				if (n < 0 || (n = source.Read()) < 0)
				{
					break;
				}

				// one more byte was read; need to allocate a larger buffer
				if (capacity <= MAX_BUFFER_SIZE - capacity)
				{
					capacity = System.Math.Max(capacity << 1, BUFFER_SIZE);
				}
				else
				{
					if (capacity == MAX_BUFFER_SIZE)
					{
						throw new OutOfMemoryError("Required array size too large");
					}
					capacity = MAX_BUFFER_SIZE;
				}
				buf = Arrays.CopyOf(buf, capacity);
				buf[nread++] = (sbyte)n;
			}
			return (capacity == nread) ? buf : Arrays.CopyOf(buf, nread);
		}

		/// <summary>
		/// Reads all the bytes from a file. The method ensures that the file is
		/// closed when all bytes have been read or an I/O error, or other runtime
		/// exception, is thrown.
		/// 
		/// <para> Note that this method is intended for simple cases where it is
		/// convenient to read all bytes into a byte array. It is not intended for
		/// reading in large files.
		/// 
		/// </para>
		/// </summary>
		/// <param name="path">
		///          the path to the file
		/// </param>
		/// <returns>  a byte array containing the bytes read from the file
		/// </returns>
		/// <exception cref="IOException">
		///          if an I/O error occurs reading from the stream </exception>
		/// <exception cref="OutOfMemoryError">
		///          if an array of the required size cannot be allocated, for
		///          example the file is larger that {@code 2GB} </exception>
		/// <exception cref="SecurityException">
		///          In the case of the default provider, and a security manager is
		///          installed, the <seealso cref="SecurityManager#checkRead(String) checkRead"/>
		///          method is invoked to check read access to the file. </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public static byte[] readAllBytes(Path path) throws java.io.IOException
		public static sbyte[] ReadAllBytes(Path path)
		{
			using (SeekableByteChannel sbc = Files.newByteChannel(path), InputStream @in = Channels.NewInputStream(sbc))
			{
				long size = sbc.size();
				if (size > (long)MAX_BUFFER_SIZE)
				{
					throw new OutOfMemoryError("Required array size too large");
				}

				return Read(@in, (int)size);
			}
		}

		/// <summary>
		/// Read all lines from a file. This method ensures that the file is
		/// closed when all bytes have been read or an I/O error, or other runtime
		/// exception, is thrown. Bytes from the file are decoded into characters
		/// using the specified charset.
		/// 
		/// <para> This method recognizes the following as line terminators:
		/// <ul>
		///   <li> <code>&#92;u000D</code> followed by <code>&#92;u000A</code>,
		///     CARRIAGE RETURN followed by LINE FEED </li>
		///   <li> <code>&#92;u000A</code>, LINE FEED </li>
		///   <li> <code>&#92;u000D</code>, CARRIAGE RETURN </li>
		/// </ul>
		/// </para>
		/// <para> Additional Unicode line terminators may be recognized in future
		/// releases.
		/// 
		/// </para>
		/// <para> Note that this method is intended for simple cases where it is
		/// convenient to read all lines in a single operation. It is not intended
		/// for reading in large files.
		/// 
		/// </para>
		/// </summary>
		/// <param name="path">
		///          the path to the file </param>
		/// <param name="cs">
		///          the charset to use for decoding
		/// </param>
		/// <returns>  the lines from the file as a {@code List}; whether the {@code
		///          List} is modifiable or not is implementation dependent and
		///          therefore not specified
		/// </returns>
		/// <exception cref="IOException">
		///          if an I/O error occurs reading from the file or a malformed or
		///          unmappable byte sequence is read </exception>
		/// <exception cref="SecurityException">
		///          In the case of the default provider, and a security manager is
		///          installed, the <seealso cref="SecurityManager#checkRead(String) checkRead"/>
		///          method is invoked to check read access to the file.
		/// </exception>
		/// <seealso cref= #newBufferedReader </seealso>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public static java.util.List<String> readAllLines(Path path, java.nio.charset.Charset cs) throws java.io.IOException
		public static IList<String> ReadAllLines(Path path, Charset cs)
		{
			using (BufferedReader reader = NewBufferedReader(path, cs))
			{
				IList<String> result = new List<String>();
				for (;;)
				{
					String line = reader.readLine();
					if (line == null)
					{
						break;
					}
					result.Add(line);
				}
				return result;
			}
		}

		/// <summary>
		/// Read all lines from a file. Bytes from the file are decoded into characters
		/// using the <seealso cref="StandardCharsets#UTF_8 UTF-8"/> <seealso cref="Charset charset"/>.
		/// 
		/// <para> This method works as if invoking it were equivalent to evaluating the
		/// expression:
		/// <pre>{@code
		/// Files.readAllLines(path, StandardCharsets.UTF_8)
		/// }</pre>
		/// 
		/// </para>
		/// </summary>
		/// <param name="path">
		///          the path to the file
		/// </param>
		/// <returns>  the lines from the file as a {@code List}; whether the {@code
		///          List} is modifiable or not is implementation dependent and
		///          therefore not specified
		/// </returns>
		/// <exception cref="IOException">
		///          if an I/O error occurs reading from the file or a malformed or
		///          unmappable byte sequence is read </exception>
		/// <exception cref="SecurityException">
		///          In the case of the default provider, and a security manager is
		///          installed, the <seealso cref="SecurityManager#checkRead(String) checkRead"/>
		///          method is invoked to check read access to the file.
		/// 
		/// @since 1.8 </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public static java.util.List<String> readAllLines(Path path) throws java.io.IOException
		public static IList<String> ReadAllLines(Path path)
		{
			return ReadAllLines(path, StandardCharsets.UTF_8);
		}

		/// <summary>
		/// Writes bytes to a file. The {@code options} parameter specifies how the
		/// the file is created or opened. If no options are present then this method
		/// works as if the <seealso cref="StandardOpenOption#CREATE CREATE"/>, {@link
		/// StandardOpenOption#TRUNCATE_EXISTING TRUNCATE_EXISTING}, and {@link
		/// StandardOpenOption#WRITE WRITE} options are present. In other words, it
		/// opens the file for writing, creating the file if it doesn't exist, or
		/// initially truncating an existing <seealso cref="#isRegularFile regular-file"/> to
		/// a size of {@code 0}. All bytes in the byte array are written to the file.
		/// The method ensures that the file is closed when all bytes have been
		/// written (or an I/O error or other runtime exception is thrown). If an I/O
		/// error occurs then it may do so after the file has created or truncated,
		/// or after some bytes have been written to the file.
		/// 
		/// <para> <b>Usage example</b>: By default the method creates a new file or
		/// overwrites an existing file. Suppose you instead want to append bytes
		/// to an existing file:
		/// <pre>
		///     Path path = ...
		///     byte[] bytes = ...
		///     Files.write(path, bytes, StandardOpenOption.APPEND);
		/// </pre>
		/// 
		/// </para>
		/// </summary>
		/// <param name="path">
		///          the path to the file </param>
		/// <param name="bytes">
		///          the byte array with the bytes to write </param>
		/// <param name="options">
		///          options specifying how the file is opened
		/// </param>
		/// <returns>  the path
		/// </returns>
		/// <exception cref="IOException">
		///          if an I/O error occurs writing to or creating the file </exception>
		/// <exception cref="UnsupportedOperationException">
		///          if an unsupported option is specified </exception>
		/// <exception cref="SecurityException">
		///          In the case of the default provider, and a security manager is
		///          installed, the <seealso cref="SecurityManager#checkWrite(String) checkWrite"/>
		///          method is invoked to check write access to the file. </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public static Path write(Path path, byte[] bytes, OpenOption... options) throws java.io.IOException
		public static Path Write(Path path, sbyte[] bytes, params OpenOption[] options)
		{
			// ensure bytes is not null before opening file
			Objects.RequireNonNull(bytes);

			using (OutputStream @out = Files.NewOutputStream(path, options))
			{
				int len = bytes.Length;
				int rem = len;
				while (rem > 0)
				{
					int n = System.Math.Min(rem, BUFFER_SIZE);
					@out.write(bytes, (len - rem), n);
					rem -= n;
				}
			}
			return path;
		}

		/// <summary>
		/// Write lines of text to a file. Each line is a char sequence and is
		/// written to the file in sequence with each line terminated by the
		/// platform's line separator, as defined by the system property {@code
		/// line.separator}. Characters are encoded into bytes using the specified
		/// charset.
		/// 
		/// <para> The {@code options} parameter specifies how the the file is created
		/// or opened. If no options are present then this method works as if the
		/// <seealso cref="StandardOpenOption#CREATE CREATE"/>, {@link
		/// StandardOpenOption#TRUNCATE_EXISTING TRUNCATE_EXISTING}, and {@link
		/// StandardOpenOption#WRITE WRITE} options are present. In other words, it
		/// opens the file for writing, creating the file if it doesn't exist, or
		/// initially truncating an existing <seealso cref="#isRegularFile regular-file"/> to
		/// a size of {@code 0}. The method ensures that the file is closed when all
		/// lines have been written (or an I/O error or other runtime exception is
		/// thrown). If an I/O error occurs then it may do so after the file has
		/// created or truncated, or after some bytes have been written to the file.
		/// 
		/// </para>
		/// </summary>
		/// <param name="path">
		///          the path to the file </param>
		/// <param name="lines">
		///          an object to iterate over the char sequences </param>
		/// <param name="cs">
		///          the charset to use for encoding </param>
		/// <param name="options">
		///          options specifying how the file is opened
		/// </param>
		/// <returns>  the path
		/// </returns>
		/// <exception cref="IOException">
		///          if an I/O error occurs writing to or creating the file, or the
		///          text cannot be encoded using the specified charset </exception>
		/// <exception cref="UnsupportedOperationException">
		///          if an unsupported option is specified </exception>
		/// <exception cref="SecurityException">
		///          In the case of the default provider, and a security manager is
		///          installed, the <seealso cref="SecurityManager#checkWrite(String) checkWrite"/>
		///          method is invoked to check write access to the file. </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public static Path write(Path path, Iterable<? extends CharSequence> lines, java.nio.charset.Charset cs, OpenOption... options) throws java.io.IOException
		public static Path write<T1>(Path path, Iterable<T1> lines, Charset cs, params OpenOption[] options) where T1 : CharSequence
		{
			// ensure lines is not null before opening file
			Objects.RequireNonNull(lines);
			CharsetEncoder encoder = cs.NewEncoder();
			OutputStream @out = NewOutputStream(path, options);
			using (BufferedWriter writer = new BufferedWriter(new OutputStreamWriter(@out, encoder)))
			{
				foreach (CharSequence line in lines)
				{
					writer.append(line);
					writer.newLine();
				}
			}
			return path;
		}

		/// <summary>
		/// Write lines of text to a file. Characters are encoded into bytes using
		/// the <seealso cref="StandardCharsets#UTF_8 UTF-8"/> <seealso cref="Charset charset"/>.
		/// 
		/// <para> This method works as if invoking it were equivalent to evaluating the
		/// expression:
		/// <pre>{@code
		/// Files.write(path, lines, StandardCharsets.UTF_8, options);
		/// }</pre>
		/// 
		/// </para>
		/// </summary>
		/// <param name="path">
		///          the path to the file </param>
		/// <param name="lines">
		///          an object to iterate over the char sequences </param>
		/// <param name="options">
		///          options specifying how the file is opened
		/// </param>
		/// <returns>  the path
		/// </returns>
		/// <exception cref="IOException">
		///          if an I/O error occurs writing to or creating the file, or the
		///          text cannot be encoded as {@code UTF-8} </exception>
		/// <exception cref="UnsupportedOperationException">
		///          if an unsupported option is specified </exception>
		/// <exception cref="SecurityException">
		///          In the case of the default provider, and a security manager is
		///          installed, the <seealso cref="SecurityManager#checkWrite(String) checkWrite"/>
		///          method is invoked to check write access to the file.
		/// 
		/// @since 1.8 </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public static Path write(Path path, Iterable<? extends CharSequence> lines, OpenOption... options) throws java.io.IOException
		public static Path write<T1>(Path path, Iterable<T1> lines, params OpenOption[] options) where T1 : CharSequence
		{
			return Write(path, lines, StandardCharsets.UTF_8, options);
		}

		// -- Stream APIs --

		/// <summary>
		/// Return a lazily populated {@code Stream}, the elements of
		/// which are the entries in the directory.  The listing is not recursive.
		/// 
		/// <para> The elements of the stream are <seealso cref="Path"/> objects that are
		/// obtained as if by <seealso cref="Path#resolve(Path) resolving"/> the name of the
		/// directory entry against {@code dir}. Some file systems maintain special
		/// links to the directory itself and the directory's parent directory.
		/// Entries representing these links are not included.
		/// 
		/// </para>
		/// <para> The stream is <i>weakly consistent</i>. It is thread safe but does
		/// not freeze the directory while iterating, so it may (or may not)
		/// reflect updates to the directory that occur after returning from this
		/// method.
		/// 
		/// </para>
		/// <para> The returned stream encapsulates a <seealso cref="DirectoryStream"/>.
		/// If timely disposal of file system resources is required, the
		/// {@code try}-with-resources construct should be used to ensure that the
		/// stream's <seealso cref="Stream#close close"/> method is invoked after the stream
		/// operations are completed.
		/// 
		/// </para>
		/// <para> Operating on a closed stream behaves as if the end of stream
		/// has been reached. Due to read-ahead, one or more elements may be
		/// returned after the stream has been closed.
		/// 
		/// </para>
		/// <para> If an <seealso cref="IOException"/> is thrown when accessing the directory
		/// after this method has returned, it is wrapped in an {@link
		/// UncheckedIOException} which will be thrown from the method that caused
		/// the access to take place.
		/// 
		/// </para>
		/// </summary>
		/// <param name="dir">  The path to the directory
		/// </param>
		/// <returns>  The {@code Stream} describing the content of the
		///          directory
		/// </returns>
		/// <exception cref="NotDirectoryException">
		///          if the file could not otherwise be opened because it is not
		///          a directory <i>(optional specific exception)</i> </exception>
		/// <exception cref="IOException">
		///          if an I/O error occurs when opening the directory </exception>
		/// <exception cref="SecurityException">
		///          In the case of the default provider, and a security manager is
		///          installed, the <seealso cref="SecurityManager#checkRead(String) checkRead"/>
		///          method is invoked to check read access to the directory.
		/// </exception>
		/// <seealso cref=     #newDirectoryStream(Path)
		/// @since   1.8 </seealso>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public static java.util.stream.Stream<Path> list(Path dir) throws java.io.IOException
		public static Stream<Path> List(Path dir)
		{
			DirectoryStream<Path> ds = Files.NewDirectoryStream(dir);
			try
			{
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final java.util.Iterator<Path> delegate = ds.iterator();
				IEnumerator<Path> @delegate = ds.Iterator();

				// Re-wrap DirectoryIteratorException to UncheckedIOException
				IEnumerator<Path> it = new IteratorAnonymousInnerClassHelper(@delegate);

				return StreamSupport.Stream(Spliterators.SpliteratorUnknownSize(it, java.util.Spliterator_Fields.DISTINCT), false).OnClose(AsUncheckedRunnable(ds));
			}
//JAVA TO C# CONVERTER TODO TASK: There is no equivalent in C# to Java 'multi-catch' syntax:
			catch (Error | RuntimeException e)
			{
				try
				{
					ds.Close();
				}
				catch (IOException ex)
				{
					try
					{
						e.addSuppressed(ex);
					}
					catch (Throwable)
					{
					}
				}
				throw e;
			}
		}

		private class IteratorAnonymousInnerClassHelper : Iterator<Path>
		{
			private IEnumerator<Path> @delegate;

			public IteratorAnonymousInnerClassHelper(IEnumerator<Path> @delegate)
			{
				this.@delegate = @delegate;
			}

			public virtual bool HasNext()
			{
				try
				{
//JAVA TO C# CONVERTER TODO TASK: Java iterators are only converted within the context of 'while' and 'for' loops:
					return @delegate.hasNext();
				}
				catch (DirectoryIteratorException e)
				{
					throw new UncheckedIOException(e.InnerException);
				}
			}
			public virtual Path Next()
			{
				try
				{
//JAVA TO C# CONVERTER TODO TASK: Java iterators are only converted within the context of 'while' and 'for' loops:
					return @delegate.next();
				}
				catch (DirectoryIteratorException e)
				{
					throw new UncheckedIOException(e.InnerException);
				}
			}
		}

		/// <summary>
		/// Return a {@code Stream} that is lazily populated with {@code
		/// Path} by walking the file tree rooted at a given starting file.  The
		/// file tree is traversed <em>depth-first</em>, the elements in the stream
		/// are <seealso cref="Path"/> objects that are obtained as if by {@link
		/// Path#resolve(Path) resolving} the relative path against {@code start}.
		/// 
		/// <para> The {@code stream} walks the file tree as elements are consumed.
		/// The {@code Stream} returned is guaranteed to have at least one
		/// element, the starting file itself. For each file visited, the stream
		/// attempts to read its <seealso cref="BasicFileAttributes"/>. If the file is a
		/// directory and can be opened successfully, entries in the directory, and
		/// their <em>descendants</em> will follow the directory in the stream as
		/// they are encountered. When all entries have been visited, then the
		/// directory is closed. The file tree walk then continues at the next
		/// <em>sibling</em> of the directory.
		/// 
		/// </para>
		/// <para> The stream is <i>weakly consistent</i>. It does not freeze the
		/// file tree while iterating, so it may (or may not) reflect updates to
		/// the file tree that occur after returned from this method.
		/// 
		/// </para>
		/// <para> By default, symbolic links are not automatically followed by this
		/// method. If the {@code options} parameter contains the {@link
		/// FileVisitOption#FOLLOW_LINKS FOLLOW_LINKS} option then symbolic links are
		/// followed. When following links, and the attributes of the target cannot
		/// be read, then this method attempts to get the {@code BasicFileAttributes}
		/// of the link.
		/// 
		/// </para>
		/// <para> If the {@code options} parameter contains the {@link
		/// FileVisitOption#FOLLOW_LINKS FOLLOW_LINKS} option then the stream keeps
		/// track of directories visited so that cycles can be detected. A cycle
		/// arises when there is an entry in a directory that is an ancestor of the
		/// directory. Cycle detection is done by recording the {@link
		/// java.nio.file.attribute.BasicFileAttributes#fileKey file-key} of directories,
		/// or if file keys are not available, by invoking the {@link #isSameFile
		/// isSameFile} method to test if a directory is the same file as an
		/// ancestor. When a cycle is detected it is treated as an I/O error with
		/// an instance of <seealso cref="FileSystemLoopException"/>.
		/// 
		/// </para>
		/// <para> The {@code maxDepth} parameter is the maximum number of levels of
		/// directories to visit. A value of {@code 0} means that only the starting
		/// file is visited, unless denied by the security manager. A value of
		/// <seealso cref="Integer#MAX_VALUE MAX_VALUE"/> may be used to indicate that all
		/// levels should be visited.
		/// 
		/// </para>
		/// <para> When a security manager is installed and it denies access to a file
		/// (or directory), then it is ignored and not included in the stream.
		/// 
		/// </para>
		/// <para> The returned stream encapsulates one or more <seealso cref="DirectoryStream"/>s.
		/// If timely disposal of file system resources is required, the
		/// {@code try}-with-resources construct should be used to ensure that the
		/// stream's <seealso cref="Stream#close close"/> method is invoked after the stream
		/// operations are completed.  Operating on a closed stream will result in an
		/// <seealso cref="java.lang.IllegalStateException"/>.
		/// 
		/// </para>
		/// <para> If an <seealso cref="IOException"/> is thrown when accessing the directory
		/// after this method has returned, it is wrapped in an {@link
		/// UncheckedIOException} which will be thrown from the method that caused
		/// the access to take place.
		/// 
		/// </para>
		/// </summary>
		/// <param name="start">
		///          the starting file </param>
		/// <param name="maxDepth">
		///          the maximum number of directory levels to visit </param>
		/// <param name="options">
		///          options to configure the traversal
		/// </param>
		/// <returns>  the <seealso cref="Stream"/> of <seealso cref="Path"/>
		/// </returns>
		/// <exception cref="IllegalArgumentException">
		///          if the {@code maxDepth} parameter is negative </exception>
		/// <exception cref="SecurityException">
		///          If the security manager denies access to the starting file.
		///          In the case of the default provider, the {@link
		///          SecurityManager#checkRead(String) checkRead} method is invoked
		///          to check read access to the directory. </exception>
		/// <exception cref="IOException">
		///          if an I/O error is thrown when accessing the starting file.
		/// @since   1.8 </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public static java.util.stream.Stream<Path> walk(Path start, int maxDepth, FileVisitOption... options) throws java.io.IOException
		public static Stream<Path> Walk(Path start, int maxDepth, params FileVisitOption[] options)
		{
			FileTreeIterator iterator = new FileTreeIterator(start, maxDepth, options);
			try
			{
//JAVA TO C# CONVERTER TODO TASK: Java lambdas satisfy functional interfaces, while .NET lambdas satisfy delegates - change the appropriate interface to a delegate:
				return StreamSupport.Stream(Spliterators.SpliteratorUnknownSize(iterator, java.util.Spliterator_Fields.DISTINCT), false).OnClose(iterator::close).Map(entry => entry.file());
			}
//JAVA TO C# CONVERTER TODO TASK: There is no equivalent in C# to Java 'multi-catch' syntax:
			catch (Error | RuntimeException e)
			{
				iterator.Close();
				throw e;
			}
		}

		/// <summary>
		/// Return a {@code Stream} that is lazily populated with {@code
		/// Path} by walking the file tree rooted at a given starting file.  The
		/// file tree is traversed <em>depth-first</em>, the elements in the stream
		/// are <seealso cref="Path"/> objects that are obtained as if by {@link
		/// Path#resolve(Path) resolving} the relative path against {@code start}.
		/// 
		/// <para> This method works as if invoking it were equivalent to evaluating the
		/// expression:
		/// <blockquote><pre>
		/// walk(start, Integer.MAX_VALUE, options)
		/// </pre></blockquote>
		/// In other words, it visits all levels of the file tree.
		/// 
		/// </para>
		/// <para> The returned stream encapsulates one or more <seealso cref="DirectoryStream"/>s.
		/// If timely disposal of file system resources is required, the
		/// {@code try}-with-resources construct should be used to ensure that the
		/// stream's <seealso cref="Stream#close close"/> method is invoked after the stream
		/// operations are completed.  Operating on a closed stream will result in an
		/// <seealso cref="java.lang.IllegalStateException"/>.
		/// 
		/// </para>
		/// </summary>
		/// <param name="start">
		///          the starting file </param>
		/// <param name="options">
		///          options to configure the traversal
		/// </param>
		/// <returns>  the <seealso cref="Stream"/> of <seealso cref="Path"/>
		/// </returns>
		/// <exception cref="SecurityException">
		///          If the security manager denies access to the starting file.
		///          In the case of the default provider, the {@link
		///          SecurityManager#checkRead(String) checkRead} method is invoked
		///          to check read access to the directory. </exception>
		/// <exception cref="IOException">
		///          if an I/O error is thrown when accessing the starting file.
		/// </exception>
		/// <seealso cref=     #walk(Path, int, FileVisitOption...)
		/// @since   1.8 </seealso>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public static java.util.stream.Stream<Path> walk(Path start, FileVisitOption... options) throws java.io.IOException
		public static Stream<Path> Walk(Path start, params FileVisitOption[] options)
		{
			return Walk(start, Integer.MaxValue, options);
		}

		/// <summary>
		/// Return a {@code Stream} that is lazily populated with {@code
		/// Path} by searching for files in a file tree rooted at a given starting
		/// file.
		/// 
		/// <para> This method walks the file tree in exactly the manner specified by
		/// the <seealso cref="#walk walk"/> method. For each file encountered, the given
		/// <seealso cref="BiPredicate"/> is invoked with its <seealso cref="Path"/> and {@link
		/// BasicFileAttributes}. The {@code Path} object is obtained as if by
		/// <seealso cref="Path#resolve(Path) resolving"/> the relative path against {@code
		/// start} and is only included in the returned <seealso cref="Stream"/> if
		/// the {@code BiPredicate} returns true. Compare to calling {@link
		/// java.util.stream.Stream#filter filter} on the {@code Stream}
		/// returned by {@code walk} method, this method may be more efficient by
		/// avoiding redundant retrieval of the {@code BasicFileAttributes}.
		/// 
		/// </para>
		/// <para> The returned stream encapsulates one or more <seealso cref="DirectoryStream"/>s.
		/// If timely disposal of file system resources is required, the
		/// {@code try}-with-resources construct should be used to ensure that the
		/// stream's <seealso cref="Stream#close close"/> method is invoked after the stream
		/// operations are completed.  Operating on a closed stream will result in an
		/// <seealso cref="java.lang.IllegalStateException"/>.
		/// 
		/// </para>
		/// <para> If an <seealso cref="IOException"/> is thrown when accessing the directory
		/// after returned from this method, it is wrapped in an {@link
		/// UncheckedIOException} which will be thrown from the method that caused
		/// the access to take place.
		/// 
		/// </para>
		/// </summary>
		/// <param name="start">
		///          the starting file </param>
		/// <param name="maxDepth">
		///          the maximum number of directory levels to search </param>
		/// <param name="matcher">
		///          the function used to decide whether a file should be included
		///          in the returned stream </param>
		/// <param name="options">
		///          options to configure the traversal
		/// </param>
		/// <returns>  the <seealso cref="Stream"/> of <seealso cref="Path"/>
		/// </returns>
		/// <exception cref="IllegalArgumentException">
		///          if the {@code maxDepth} parameter is negative </exception>
		/// <exception cref="SecurityException">
		///          If the security manager denies access to the starting file.
		///          In the case of the default provider, the {@link
		///          SecurityManager#checkRead(String) checkRead} method is invoked
		///          to check read access to the directory. </exception>
		/// <exception cref="IOException">
		///          if an I/O error is thrown when accessing the starting file.
		/// </exception>
		/// <seealso cref=     #walk(Path, int, FileVisitOption...)
		/// @since   1.8 </seealso>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public static java.util.stream.Stream<Path> find(Path start, int maxDepth, java.util.function.BiPredicate<Path, java.nio.file.attribute.BasicFileAttributes> matcher, FileVisitOption... options) throws java.io.IOException
		public static Stream<Path> Find(Path start, int maxDepth, BiPredicate<Path, BasicFileAttributes> matcher, params FileVisitOption[] options)
		{
			FileTreeIterator iterator = new FileTreeIterator(start, maxDepth, options);
			try
			{
//JAVA TO C# CONVERTER TODO TASK: Java lambdas satisfy functional interfaces, while .NET lambdas satisfy delegates - change the appropriate interface to a delegate:
				return StreamSupport.Stream(Spliterators.SpliteratorUnknownSize(iterator, java.util.Spliterator_Fields.DISTINCT), false).OnClose(iterator::close).Filter(entry => matcher.Test(entry.file(), entry.attributes())).Map(entry => entry.file());
			}
//JAVA TO C# CONVERTER TODO TASK: There is no equivalent in C# to Java 'multi-catch' syntax:
			catch (Error | RuntimeException e)
			{
				iterator.Close();
				throw e;
			}
		}

		/// <summary>
		/// Read all lines from a file as a {@code Stream}. Unlike {@link
		/// #readAllLines(Path, Charset) readAllLines}, this method does not read
		/// all lines into a {@code List}, but instead populates lazily as the stream
		/// is consumed.
		/// 
		/// <para> Bytes from the file are decoded into characters using the specified
		/// charset and the same line terminators as specified by {@code
		/// readAllLines} are supported.
		/// 
		/// </para>
		/// <para> After this method returns, then any subsequent I/O exception that
		/// occurs while reading from the file or when a malformed or unmappable byte
		/// sequence is read, is wrapped in an <seealso cref="UncheckedIOException"/> that will
		/// be thrown from the
		/// <seealso cref="java.util.stream.Stream"/> method that caused the read to take
		/// place. In case an {@code IOException} is thrown when closing the file,
		/// it is also wrapped as an {@code UncheckedIOException}.
		/// 
		/// </para>
		/// <para> The returned stream encapsulates a <seealso cref="Reader"/>.  If timely
		/// disposal of file system resources is required, the try-with-resources
		/// construct should be used to ensure that the stream's
		/// <seealso cref="Stream#close close"/> method is invoked after the stream operations
		/// are completed.
		/// 
		/// 
		/// </para>
		/// </summary>
		/// <param name="path">
		///          the path to the file </param>
		/// <param name="cs">
		///          the charset to use for decoding
		/// </param>
		/// <returns>  the lines from the file as a {@code Stream}
		/// </returns>
		/// <exception cref="IOException">
		///          if an I/O error occurs opening the file </exception>
		/// <exception cref="SecurityException">
		///          In the case of the default provider, and a security manager is
		///          installed, the <seealso cref="SecurityManager#checkRead(String) checkRead"/>
		///          method is invoked to check read access to the file.
		/// </exception>
		/// <seealso cref=     #readAllLines(Path, Charset) </seealso>
		/// <seealso cref=     #newBufferedReader(Path, Charset) </seealso>
		/// <seealso cref=     java.io.BufferedReader#lines()
		/// @since   1.8 </seealso>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public static java.util.stream.Stream<String> lines(Path path, java.nio.charset.Charset cs) throws java.io.IOException
		public static Stream<String> Lines(Path path, Charset cs)
		{
			BufferedReader br = Files.NewBufferedReader(path, cs);
			try
			{
				return br.Lines().OnClose(AsUncheckedRunnable(br));
			}
//JAVA TO C# CONVERTER TODO TASK: There is no equivalent in C# to Java 'multi-catch' syntax:
			catch (Error | RuntimeException e)
			{
				try
				{
					br.Close();
				}
				catch (IOException ex)
				{
					try
					{
						e.addSuppressed(ex);
					}
					catch (Throwable)
					{
					}
				}
				throw e;
			}
		}

		/// <summary>
		/// Read all lines from a file as a {@code Stream}. Bytes from the file are
		/// decoded into characters using the <seealso cref="StandardCharsets#UTF_8 UTF-8"/>
		/// <seealso cref="Charset charset"/>.
		/// 
		/// <para> This method works as if invoking it were equivalent to evaluating the
		/// expression:
		/// <pre>{@code
		/// Files.lines(path, StandardCharsets.UTF_8)
		/// }</pre>
		/// 
		/// </para>
		/// </summary>
		/// <param name="path">
		///          the path to the file
		/// </param>
		/// <returns>  the lines from the file as a {@code Stream}
		/// </returns>
		/// <exception cref="IOException">
		///          if an I/O error occurs opening the file </exception>
		/// <exception cref="SecurityException">
		///          In the case of the default provider, and a security manager is
		///          installed, the <seealso cref="SecurityManager#checkRead(String) checkRead"/>
		///          method is invoked to check read access to the file.
		/// 
		/// @since 1.8 </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public static java.util.stream.Stream<String> lines(Path path) throws java.io.IOException
		public static Stream<String> Lines(Path path)
		{
			return Lines(path, StandardCharsets.UTF_8);
		}
	}

}
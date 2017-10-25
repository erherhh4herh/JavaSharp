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

namespace java.nio.file.spi
{


	/// <summary>
	/// Service-provider class for file systems. The methods defined by the {@link
	/// java.nio.file.Files} class will typically delegate to an instance of this
	/// class.
	/// 
	/// <para> A file system provider is a concrete implementation of this class that
	/// implements the abstract methods defined by this class. A provider is
	/// identified by a {@code URI} <seealso cref="#getScheme() scheme"/>. The default provider
	/// is identified by the URI scheme "file". It creates the <seealso cref="FileSystem"/> that
	/// provides access to the file systems accessible to the Java virtual machine.
	/// The <seealso cref="FileSystems"/> class defines how file system providers are located
	/// and loaded. The default provider is typically a system-default provider but
	/// may be overridden if the system property {@code
	/// java.nio.file.spi.DefaultFileSystemProvider} is set. In that case, the
	/// provider has a one argument constructor whose formal parameter type is {@code
	/// FileSystemProvider}. All other providers have a zero argument constructor
	/// that initializes the provider.
	/// 
	/// </para>
	/// <para> A provider is a factory for one or more <seealso cref="FileSystem"/> instances. Each
	/// file system is identified by a {@code URI} where the URI's scheme matches
	/// the provider's <seealso cref="#getScheme scheme"/>. The default file system, for example,
	/// is identified by the URI {@code "file:///"}. A memory-based file system,
	/// for example, may be identified by a URI such as {@code "memory:///?name=logfs"}.
	/// The <seealso cref="#newFileSystem newFileSystem"/> method may be used to create a file
	/// system, and the <seealso cref="#getFileSystem getFileSystem"/> method may be used to
	/// obtain a reference to an existing file system created by the provider. Where
	/// a provider is the factory for a single file system then it is provider dependent
	/// if the file system is created when the provider is initialized, or later when
	/// the {@code newFileSystem} method is invoked. In the case of the default
	/// provider, the {@code FileSystem} is created when the provider is initialized.
	/// 
	/// </para>
	/// <para> All of the methods in this class are safe for use by multiple concurrent
	/// threads.
	/// 
	/// @since 1.7
	/// </para>
	/// </summary>

	public abstract class FileSystemProvider
	{
		// lock using when loading providers
		private static readonly Object @lock = new Object();

		// installed providers
		private static volatile List<FileSystemProvider> InstalledProviders_Renamed;

		// used to avoid recursive loading of instaled providers
		private static bool LoadingProviders = false;

		private static Void CheckPermission()
		{
			SecurityManager sm = System.SecurityManager;
			if (sm != null)
			{
				sm.CheckPermission(new RuntimePermission("fileSystemProvider"));
			}
			return null;
		}
		private FileSystemProvider(Void ignore)
		{
		}

		/// <summary>
		/// Initializes a new instance of this class.
		/// 
		/// <para> During construction a provider may safely access files associated
		/// with the default provider but care needs to be taken to avoid circular
		/// loading of other installed providers. If circular loading of installed
		/// providers is detected then an unspecified error is thrown.
		/// 
		/// </para>
		/// </summary>
		/// <exception cref="SecurityException">
		///          If a security manager has been installed and it denies
		///          <seealso cref="RuntimePermission"/><tt>("fileSystemProvider")</tt> </exception>
		protected internal FileSystemProvider() : this(CheckPermission())
		{
		}

		// loads all installed providers
		private static List<FileSystemProvider> LoadInstalledProviders()
		{
			List<FileSystemProvider> list = new List<FileSystemProvider>();

			ServiceLoader<FileSystemProvider> sl = ServiceLoader.Load(typeof(FileSystemProvider), ClassLoader.SystemClassLoader);

			// ServiceConfigurationError may be throw here
			foreach (FileSystemProvider provider in sl)
			{
				String scheme = provider.Scheme;

				// add to list if the provider is not "file" and isn't a duplicate
				if (!scheme.EqualsIgnoreCase("file"))
				{
					bool found = false;
					foreach (FileSystemProvider p in list)
					{
						if (p.Scheme.EqualsIgnoreCase(scheme))
						{
							found = true;
							break;
						}
					}
					if (!found)
					{
						list.Add(provider);
					}
				}
			}
			return list;
		}

		/// <summary>
		/// Returns a list of the installed file system providers.
		/// 
		/// <para> The first invocation of this method causes the default provider to be
		/// initialized (if not already initialized) and loads any other installed
		/// providers as described by the <seealso cref="FileSystems"/> class.
		/// 
		/// </para>
		/// </summary>
		/// <returns>  An unmodifiable list of the installed file system providers. The
		///          list contains at least one element, that is the default file
		///          system provider
		/// </returns>
		/// <exception cref="ServiceConfigurationError">
		///          When an error occurs while loading a service provider </exception>
		public static List<FileSystemProvider> InstalledProviders()
		{
			if (InstalledProviders_Renamed == null)
			{
				// ensure default provider is initialized
				FileSystemProvider defaultProvider = FileSystems.Default.Provider();

				lock (@lock)
				{
					if (InstalledProviders_Renamed == null)
					{
						if (LoadingProviders)
						{
							throw new Error("Circular loading of installed providers detected");
						}
						LoadingProviders = true;

						List<FileSystemProvider> list = AccessController.doPrivileged(new PrivilegedActionAnonymousInnerClassHelper());

						// insert the default provider at the start of the list
						list.Add(0, defaultProvider);

						InstalledProviders_Renamed = Collections.UnmodifiableList(list);
					}
				}
			}
			return InstalledProviders_Renamed;
		}

		private class PrivilegedActionAnonymousInnerClassHelper : PrivilegedAction<List<FileSystemProvider>>
		{
			public PrivilegedActionAnonymousInnerClassHelper()
			{
			}

			public virtual List<FileSystemProvider> Run()
			{
				return LoadInstalledProviders();
			}
		}

		/// <summary>
		/// Returns the URI scheme that identifies this provider.
		/// </summary>
		/// <returns>  The URI scheme </returns>
		public abstract String Scheme {get;}

		/// <summary>
		/// Constructs a new {@code FileSystem} object identified by a URI. This
		/// method is invoked by the <seealso cref="FileSystems#newFileSystem(URI,Map)"/>
		/// method to open a new file system identified by a URI.
		/// 
		/// <para> The {@code uri} parameter is an absolute, hierarchical URI, with a
		/// scheme equal (without regard to case) to the scheme supported by this
		/// provider. The exact form of the URI is highly provider dependent. The
		/// {@code env} parameter is a map of provider specific properties to configure
		/// the file system.
		/// 
		/// </para>
		/// <para> This method throws <seealso cref="FileSystemAlreadyExistsException"/> if the
		/// file system already exists because it was previously created by an
		/// invocation of this method. Once a file system is {@link
		/// java.nio.file.FileSystem#close closed} it is provider-dependent if the
		/// provider allows a new file system to be created with the same URI as a
		/// file system it previously created.
		/// 
		/// </para>
		/// </summary>
		/// <param name="uri">
		///          URI reference </param>
		/// <param name="env">
		///          A map of provider specific properties to configure the file system;
		///          may be empty
		/// </param>
		/// <returns>  A new file system
		/// </returns>
		/// <exception cref="IllegalArgumentException">
		///          If the pre-conditions for the {@code uri} parameter aren't met,
		///          or the {@code env} parameter does not contain properties required
		///          by the provider, or a property value is invalid </exception>
		/// <exception cref="IOException">
		///          An I/O error occurs creating the file system </exception>
		/// <exception cref="SecurityException">
		///          If a security manager is installed and it denies an unspecified
		///          permission required by the file system provider implementation </exception>
		/// <exception cref="FileSystemAlreadyExistsException">
		///          If the file system has already been created </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public abstract FileSystem newFileSystem(java.net.URI uri, Map<String,?> env) throws java.io.IOException;
		public abstract FileSystem newFileSystem<T1>(URI uri, Map<T1> env);

		/// <summary>
		/// Returns an existing {@code FileSystem} created by this provider.
		/// 
		/// <para> This method returns a reference to a {@code FileSystem} that was
		/// created by invoking the <seealso cref="#newFileSystem(URI,Map) newFileSystem(URI,Map)"/>
		/// method. File systems created the {@link #newFileSystem(Path,Map)
		/// newFileSystem(Path,Map)} method are not returned by this method.
		/// The file system is identified by its {@code URI}. Its exact form
		/// is highly provider dependent. In the case of the default provider the URI's
		/// path component is {@code "/"} and the authority, query and fragment components
		/// are undefined (Undefined components are represented by {@code null}).
		/// 
		/// </para>
		/// <para> Once a file system created by this provider is {@link
		/// java.nio.file.FileSystem#close closed} it is provider-dependent if this
		/// method returns a reference to the closed file system or throws {@link
		/// FileSystemNotFoundException}. If the provider allows a new file system to
		/// be created with the same URI as a file system it previously created then
		/// this method throws the exception if invoked after the file system is
		/// closed (and before a new instance is created by the {@link #newFileSystem
		/// newFileSystem} method).
		/// 
		/// </para>
		/// <para> If a security manager is installed then a provider implementation
		/// may require to check a permission before returning a reference to an
		/// existing file system. In the case of the {@link FileSystems#getDefault
		/// default} file system, no permission check is required.
		/// 
		/// </para>
		/// </summary>
		/// <param name="uri">
		///          URI reference
		/// </param>
		/// <returns>  The file system
		/// </returns>
		/// <exception cref="IllegalArgumentException">
		///          If the pre-conditions for the {@code uri} parameter aren't met </exception>
		/// <exception cref="FileSystemNotFoundException">
		///          If the file system does not exist </exception>
		/// <exception cref="SecurityException">
		///          If a security manager is installed and it denies an unspecified
		///          permission. </exception>
		public abstract FileSystem GetFileSystem(URI uri);

		/// <summary>
		/// Return a {@code Path} object by converting the given <seealso cref="URI"/>. The
		/// resulting {@code Path} is associated with a <seealso cref="FileSystem"/> that
		/// already exists or is constructed automatically.
		/// 
		/// <para> The exact form of the URI is file system provider dependent. In the
		/// case of the default provider, the URI scheme is {@code "file"} and the
		/// given URI has a non-empty path component, and undefined query, and
		/// fragment components. The resulting {@code Path} is associated with the
		/// default <seealso cref="FileSystems#getDefault default"/> {@code FileSystem}.
		/// 
		/// </para>
		/// <para> If a security manager is installed then a provider implementation
		/// may require to check a permission. In the case of the {@link
		/// FileSystems#getDefault default} file system, no permission check is
		/// required.
		/// 
		/// </para>
		/// </summary>
		/// <param name="uri">
		///          The URI to convert
		/// </param>
		/// <returns>  The resulting {@code Path}
		/// </returns>
		/// <exception cref="IllegalArgumentException">
		///          If the URI scheme does not identify this provider or other
		///          preconditions on the uri parameter do not hold </exception>
		/// <exception cref="FileSystemNotFoundException">
		///          The file system, identified by the URI, does not exist and
		///          cannot be created automatically </exception>
		/// <exception cref="SecurityException">
		///          If a security manager is installed and it denies an unspecified
		///          permission. </exception>
		public abstract Path GetPath(URI uri);

		/// <summary>
		/// Constructs a new {@code FileSystem} to access the contents of a file as a
		/// file system.
		/// 
		/// <para> This method is intended for specialized providers of pseudo file
		/// systems where the contents of one or more files is treated as a file
		/// system. The {@code env} parameter is a map of provider specific properties
		/// to configure the file system.
		/// 
		/// </para>
		/// <para> If this provider does not support the creation of such file systems
		/// or if the provider does not recognize the file type of the given file then
		/// it throws {@code UnsupportedOperationException}. The default implementation
		/// of this method throws {@code UnsupportedOperationException}.
		/// 
		/// </para>
		/// </summary>
		/// <param name="path">
		///          The path to the file </param>
		/// <param name="env">
		///          A map of provider specific properties to configure the file system;
		///          may be empty
		/// </param>
		/// <returns>  A new file system
		/// </returns>
		/// <exception cref="UnsupportedOperationException">
		///          If this provider does not support access to the contents as a
		///          file system or it does not recognize the file type of the
		///          given file </exception>
		/// <exception cref="IllegalArgumentException">
		///          If the {@code env} parameter does not contain properties required
		///          by the provider, or a property value is invalid </exception>
		/// <exception cref="IOException">
		///          If an I/O error occurs </exception>
		/// <exception cref="SecurityException">
		///          If a security manager is installed and it denies an unspecified
		///          permission. </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public FileSystem newFileSystem(Path path, Map<String,?> env) throws java.io.IOException
		public virtual FileSystem newFileSystem<T1>(Path path, Map<T1> env)
		{
			throw new UnsupportedOperationException();
		}

		/// <summary>
		/// Opens a file, returning an input stream to read from the file. This
		/// method works in exactly the manner specified by the {@link
		/// Files#newInputStream} method.
		/// 
		/// <para> The default implementation of this method opens a channel to the file
		/// as if by invoking the <seealso cref="#newByteChannel"/> method and constructs a
		/// stream that reads bytes from the channel. This method should be overridden
		/// where appropriate.
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
//ORIGINAL LINE: public java.io.InputStream newInputStream(Path path, OpenOption... options) throws java.io.IOException
		public virtual InputStream NewInputStream(Path path, params OpenOption[] options)
		{
			if (options.Length > 0)
			{
				foreach (OpenOption opt in options)
				{
					// All OpenOption values except for APPEND and WRITE are allowed
					if (opt == StandardOpenOption.APPEND || opt == StandardOpenOption.WRITE)
					{
						throw new UnsupportedOperationException("'" + opt + "' not allowed");
					}
				}
			}
			return Channels.NewInputStream(Files.NewByteChannel(path, options));
		}

		/// <summary>
		/// Opens or creates a file, returning an output stream that may be used to
		/// write bytes to the file. This method works in exactly the manner
		/// specified by the <seealso cref="Files#newOutputStream"/> method.
		/// 
		/// <para> The default implementation of this method opens a channel to the file
		/// as if by invoking the <seealso cref="#newByteChannel"/> method and constructs a
		/// stream that writes bytes to the channel. This method should be overridden
		/// where appropriate.
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
//ORIGINAL LINE: public java.io.OutputStream newOutputStream(Path path, OpenOption... options) throws java.io.IOException
		public virtual OutputStream NewOutputStream(Path path, params OpenOption[] options)
		{
			int len = options.Length;
			Set<OpenOption> opts = new HashSet<OpenOption>(len + 3);
			if (len == 0)
			{
				opts.Add(StandardOpenOption.CREATE);
				opts.Add(StandardOpenOption.TRUNCATE_EXISTING);
			}
			else
			{
				foreach (OpenOption opt in options)
				{
					if (opt == StandardOpenOption.READ)
					{
						throw new IllegalArgumentException("READ not allowed");
					}
					opts.Add(opt);
				}
			}
			opts.Add(StandardOpenOption.WRITE);
			return Channels.NewOutputStream(newByteChannel(path, opts));
		}

		/// <summary>
		/// Opens or creates a file for reading and/or writing, returning a file
		/// channel to access the file. This method works in exactly the manner
		/// specified by the {@link FileChannel#open(Path,Set,FileAttribute[])
		/// FileChannel.open} method. A provider that does not support all the
		/// features required to construct a file channel throws {@code
		/// UnsupportedOperationException}. The default provider is required to
		/// support the creation of file channels. When not overridden, the default
		/// implementation throws {@code UnsupportedOperationException}.
		/// </summary>
		/// <param name="path">
		///          the path of the file to open or create </param>
		/// <param name="options">
		///          options specifying how the file is opened </param>
		/// <param name="attrs">
		///          an optional list of file attributes to set atomically when
		///          creating the file
		/// </param>
		/// <returns>  a new file channel
		/// </returns>
		/// <exception cref="IllegalArgumentException">
		///          If the set contains an invalid combination of options </exception>
		/// <exception cref="UnsupportedOperationException">
		///          If this provider that does not support creating file channels,
		///          or an unsupported open option or file attribute is specified </exception>
		/// <exception cref="IOException">
		///          If an I/O error occurs </exception>
		/// <exception cref="SecurityException">
		///          In the case of the default file system, the {@link
		///          SecurityManager#checkRead(String)} method is invoked to check
		///          read access if the file is opened for reading. The {@link
		///          SecurityManager#checkWrite(String)} method is invoked to check
		///          write access if the file is opened for writing </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public FileChannel newFileChannel(Path path, Set<? extends OpenOption> options, FileAttribute<?>... attrs) throws java.io.IOException
//JAVA TO C# CONVERTER TODO TASK: Java wildcard generics are not converted to .NET:
		public virtual FileChannel newFileChannel<T1>(Path path, Set<T1> options, params FileAttribute<?>[] attrs) where T1 : OpenOption
		{
			throw new UnsupportedOperationException();
		}

		/// <summary>
		/// Opens or creates a file for reading and/or writing, returning an
		/// asynchronous file channel to access the file. This method works in
		/// exactly the manner specified by the {@link
		/// AsynchronousFileChannel#open(Path,Set,ExecutorService,FileAttribute[])
		/// AsynchronousFileChannel.open} method.
		/// A provider that does not support all the features required to construct
		/// an asynchronous file channel throws {@code UnsupportedOperationException}.
		/// The default provider is required to support the creation of asynchronous
		/// file channels. When not overridden, the default implementation of this
		/// method throws {@code UnsupportedOperationException}.
		/// </summary>
		/// <param name="path">
		///          the path of the file to open or create </param>
		/// <param name="options">
		///          options specifying how the file is opened </param>
		/// <param name="executor">
		///          the thread pool or {@code null} to associate the channel with
		///          the default thread pool </param>
		/// <param name="attrs">
		///          an optional list of file attributes to set atomically when
		///          creating the file
		/// </param>
		/// <returns>  a new asynchronous file channel
		/// </returns>
		/// <exception cref="IllegalArgumentException">
		///          If the set contains an invalid combination of options </exception>
		/// <exception cref="UnsupportedOperationException">
		///          If this provider that does not support creating asynchronous file
		///          channels, or an unsupported open option or file attribute is
		///          specified </exception>
		/// <exception cref="IOException">
		///          If an I/O error occurs </exception>
		/// <exception cref="SecurityException">
		///          In the case of the default file system, the {@link
		///          SecurityManager#checkRead(String)} method is invoked to check
		///          read access if the file is opened for reading. The {@link
		///          SecurityManager#checkWrite(String)} method is invoked to check
		///          write access if the file is opened for writing </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public AsynchronousFileChannel newAsynchronousFileChannel(Path path, Set<? extends OpenOption> options, java.util.concurrent.ExecutorService executor, FileAttribute<?>... attrs) throws java.io.IOException
//JAVA TO C# CONVERTER TODO TASK: Java wildcard generics are not converted to .NET:
		public virtual AsynchronousFileChannel newAsynchronousFileChannel<T1>(Path path, Set<T1> options, ExecutorService executor, params FileAttribute<?>[] attrs) where T1 : OpenOption
		{
			throw new UnsupportedOperationException();
		}

		/// <summary>
		/// Opens or creates a file, returning a seekable byte channel to access the
		/// file. This method works in exactly the manner specified by the {@link
		/// Files#newByteChannel(Path,Set,FileAttribute[])} method.
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
		///          {@code DELETE_ON_CLOSE} option. </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public abstract SeekableByteChannel newByteChannel(Path path, Set<? extends OpenOption> options, FileAttribute<?>... attrs) throws java.io.IOException;
//JAVA TO C# CONVERTER TODO TASK: Java wildcard generics are not converted to .NET:
		public abstract SeekableByteChannel newByteChannel<T1>(Path path, Set<T1> options, params FileAttribute<?>[] attrs) where T1 : OpenOption;

		/// <summary>
		/// Opens a directory, returning a {@code DirectoryStream} to iterate over
		/// the entries in the directory. This method works in exactly the manner
		/// specified by the {@link
		/// Files#newDirectoryStream(java.nio.file.Path, java.nio.file.DirectoryStream.Filter)}
		/// method.
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
//ORIGINAL LINE: public abstract DirectoryStream<Path> newDirectoryStream(Path dir, DirectoryStream_Filter<? base Path> filter) throws java.io.IOException;
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
		public abstract DirectoryStream<Path> NewDirectoryStream(Path dir, DirectoryStream_Filter<T1> filter);

		/// <summary>
		/// Creates a new directory. This method works in exactly the manner
		/// specified by the <seealso cref="Files#createDirectory"/> method.
		/// </summary>
		/// <param name="dir">
		///          the directory to create </param>
		/// <param name="attrs">
		///          an optional list of file attributes to set atomically when
		///          creating the directory
		/// </param>
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
//ORIGINAL LINE: public abstract void createDirectory(Path dir, FileAttribute<?>... attrs) throws java.io.IOException;
//JAVA TO C# CONVERTER TODO TASK: Java wildcard generics are not converted to .NET:
		public abstract void CreateDirectory(Path dir, params FileAttribute<?>[] attrs);

		/// <summary>
		/// Creates a symbolic link to a target. This method works in exactly the
		/// manner specified by the <seealso cref="Files#createSymbolicLink"/> method.
		/// 
		/// <para> The default implementation of this method throws {@code
		/// UnsupportedOperationException}.
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
//ORIGINAL LINE: public void createSymbolicLink(Path link, Path target, FileAttribute<?>... attrs) throws java.io.IOException
//JAVA TO C# CONVERTER TODO TASK: Java wildcard generics are not converted to .NET:
		public virtual void CreateSymbolicLink(Path link, Path target, params FileAttribute<?>[] attrs)
		{
			throw new UnsupportedOperationException();
		}

		/// <summary>
		/// Creates a new link (directory entry) for an existing file. This method
		/// works in exactly the manner specified by the <seealso cref="Files#createLink"/>
		/// method.
		/// 
		/// <para> The default implementation of this method throws {@code
		/// UnsupportedOperationException}.
		/// 
		/// </para>
		/// </summary>
		/// <param name="link">
		///          the link (directory entry) to create </param>
		/// <param name="existing">
		///          a path to an existing file
		/// </param>
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
		///          method denies write access to either the  link or the
		///          existing file. </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public void createLink(Path link, Path existing) throws java.io.IOException
		public virtual void CreateLink(Path link, Path existing)
		{
			throw new UnsupportedOperationException();
		}

		/// <summary>
		/// Deletes a file. This method works in exactly the  manner specified by the
		/// <seealso cref="Files#delete"/> method.
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
//ORIGINAL LINE: public abstract void delete(Path path) throws java.io.IOException;
		public abstract void Delete(Path path);

		/// <summary>
		/// Deletes a file if it exists. This method works in exactly the manner
		/// specified by the <seealso cref="Files#deleteIfExists"/> method.
		/// 
		/// <para> The default implementation of this method simply invokes {@link
		/// #delete} ignoring the {@code NoSuchFileException} when the file does not
		/// exist. It may be overridden where appropriate.
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
		///          is invoked to check delete access to the file </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public boolean deleteIfExists(Path path) throws java.io.IOException
		public virtual bool DeleteIfExists(Path path)
		{
			try
			{
				Delete(path);
				return true;
			}
			catch (NoSuchFileException)
			{
				return false;
			}
		}

		/// <summary>
		/// Reads the target of a symbolic link. This method works in exactly the
		/// manner specified by the <seealso cref="Files#readSymbolicLink"/> method.
		/// 
		/// <para> The default implementation of this method throws {@code
		/// UnsupportedOperationException}.
		/// 
		/// </para>
		/// </summary>
		/// <param name="link">
		///          the path to the symbolic link
		/// </param>
		/// <returns>  The target of the symbolic link
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
//ORIGINAL LINE: public Path readSymbolicLink(Path link) throws java.io.IOException
		public virtual Path ReadSymbolicLink(Path link)
		{
			throw new UnsupportedOperationException();
		}

		/// <summary>
		/// Copy a file to a target file. This method works in exactly the manner
		/// specified by the <seealso cref="Files#copy(Path,Path,CopyOption[])"/> method
		/// except that both the source and target paths must be associated with
		/// this provider.
		/// </summary>
		/// <param name="source">
		///          the path to the file to copy </param>
		/// <param name="target">
		///          the path to the target file </param>
		/// <param name="options">
		///          options specifying how the copy should be done
		/// </param>
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
//ORIGINAL LINE: public abstract void copy(Path source, Path target, CopyOption... options) throws java.io.IOException;
		public abstract void Copy(Path source, Path target, params CopyOption[] options);

		/// <summary>
		/// Move or rename a file to a target file. This method works in exactly the
		/// manner specified by the <seealso cref="Files#move"/> method except that both the
		/// source and target paths must be associated with this provider.
		/// </summary>
		/// <param name="source">
		///          the path to the file to move </param>
		/// <param name="target">
		///          the path to the target file </param>
		/// <param name="options">
		///          options specifying how the move should be done
		/// </param>
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
//ORIGINAL LINE: public abstract void move(Path source, Path target, CopyOption... options) throws java.io.IOException;
		public abstract void Move(Path source, Path target, params CopyOption[] options);

		/// <summary>
		/// Tests if two paths locate the same file. This method works in exactly the
		/// manner specified by the <seealso cref="Files#isSameFile"/> method.
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
		///          method is invoked to check read access to both files. </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public abstract boolean isSameFile(Path path, Path path2) throws java.io.IOException;
		public abstract bool IsSameFile(Path path, Path path2);

		/// <summary>
		/// Tells whether or not a file is considered <em>hidden</em>. This method
		/// works in exactly the manner specified by the <seealso cref="Files#isHidden"/>
		/// method.
		/// 
		/// <para> This method is invoked by the <seealso cref="Files#isHidden isHidden"/> method.
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
//ORIGINAL LINE: public abstract boolean isHidden(Path path) throws java.io.IOException;
		public abstract bool IsHidden(Path path);

		/// <summary>
		/// Returns the <seealso cref="FileStore"/> representing the file store where a file
		/// is located. This method works in exactly the manner specified by the
		/// <seealso cref="Files#getFileStore"/> method.
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
//ORIGINAL LINE: public abstract FileStore getFileStore(Path path) throws java.io.IOException;
		public abstract FileStore GetFileStore(Path path);

		/// <summary>
		/// Checks the existence, and optionally the accessibility, of a file.
		/// 
		/// <para> This method may be used by the <seealso cref="Files#isReadable isReadable"/>,
		/// <seealso cref="Files#isWritable isWritable"/> and {@link Files#isExecutable
		/// isExecutable} methods to check the accessibility of a file.
		/// 
		/// </para>
		/// <para> This method checks the existence of a file and that this Java virtual
		/// machine has appropriate privileges that would allow it access the file
		/// according to all of access modes specified in the {@code modes} parameter
		/// as follows:
		/// 
		/// <table border=1 cellpadding=5 summary="">
		/// <tr> <th>Value</th> <th>Description</th> </tr>
		/// <tr>
		///   <td> <seealso cref="AccessMode#READ READ"/> </td>
		///   <td> Checks that the file exists and that the Java virtual machine has
		///     permission to read the file. </td>
		/// </tr>
		/// <tr>
		///   <td> <seealso cref="AccessMode#WRITE WRITE"/> </td>
		///   <td> Checks that the file exists and that the Java virtual machine has
		///     permission to write to the file, </td>
		/// </tr>
		/// <tr>
		///   <td> <seealso cref="AccessMode#EXECUTE EXECUTE"/> </td>
		///   <td> Checks that the file exists and that the Java virtual machine has
		///     permission to <seealso cref="Runtime#exec execute"/> the file. The semantics
		///     may differ when checking access to a directory. For example, on UNIX
		///     systems, checking for {@code EXECUTE} access checks that the Java
		///     virtual machine has permission to search the directory in order to
		///     access file or subdirectories. </td>
		/// </tr>
		/// </table>
		/// 
		/// </para>
		/// <para> If the {@code modes} parameter is of length zero, then the existence
		/// of the file is checked.
		/// 
		/// </para>
		/// <para> This method follows symbolic links if the file referenced by this
		/// object is a symbolic link. Depending on the implementation, this method
		/// may require to read file permissions, access control lists, or other
		/// file attributes in order to check the effective access to the file. To
		/// determine the effective access to a file may require access to several
		/// attributes and so in some implementations this method may not be atomic
		/// with respect to other file system operations.
		/// 
		/// </para>
		/// </summary>
		/// <param name="path">
		///          the path to the file to check </param>
		/// <param name="modes">
		///          The access modes to check; may have zero elements
		/// </param>
		/// <exception cref="UnsupportedOperationException">
		///          an implementation is required to support checking for
		///          {@code READ}, {@code WRITE}, and {@code EXECUTE} access. This
		///          exception is specified to allow for the {@code Access} enum to
		///          be extended in future releases. </exception>
		/// <exception cref="NoSuchFileException">
		///          if a file does not exist <i>(optional specific exception)</i> </exception>
		/// <exception cref="AccessDeniedException">
		///          the requested access would be denied or the access cannot be
		///          determined because the Java virtual machine has insufficient
		///          privileges or other reasons. <i>(optional specific exception)</i> </exception>
		/// <exception cref="IOException">
		///          if an I/O error occurs </exception>
		/// <exception cref="SecurityException">
		///          In the case of the default provider, and a security manager is
		///          installed, the <seealso cref="SecurityManager#checkRead(String) checkRead"/>
		///          is invoked when checking read access to the file or only the
		///          existence of the file, the {@link SecurityManager#checkWrite(String)
		///          checkWrite} is invoked when checking write access to the file,
		///          and <seealso cref="SecurityManager#checkExec(String) checkExec"/> is invoked
		///          when checking execute access. </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public abstract void checkAccess(Path path, AccessMode... modes) throws java.io.IOException;
		public abstract void CheckAccess(Path path, params AccessMode[] modes);

		/// <summary>
		/// Returns a file attribute view of a given type. This method works in
		/// exactly the manner specified by the <seealso cref="Files#getFileAttributeView"/>
		/// method.
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
		public abstract V getFileAttributeView<V>(Path path, Class type, params LinkOption[] options) where V : FileAttributeView;

		/// <summary>
		/// Reads a file's attributes as a bulk operation. This method works in
		/// exactly the manner specified by the {@link
		/// Files#readAttributes(Path,Class,LinkOption[])} method.
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
		///          method is invoked to check read access to the file </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public abstract <A extends BasicFileAttributes> A readAttributes(Path path, Class type, LinkOption... options) throws java.io.IOException;
		public abstract A readAttributes<A>(Path path, Class type, params LinkOption[] options) where A : BasicFileAttributes;

		/// <summary>
		/// Reads a set of file attributes as a bulk operation. This method works in
		/// exactly the manner specified by the {@link
		/// Files#readAttributes(Path,String,LinkOption[])} method.
		/// </summary>
		/// <param name="path">
		///          the path to the file </param>
		/// <param name="attributes">
		///          the attributes to read </param>
		/// <param name="options">
		///          options indicating how symbolic links are handled
		/// </param>
		/// <returns>  a map of the attributes returned; may be empty. The map's keys
		///          are the attribute names, its values are the attribute values
		/// </returns>
		/// <exception cref="UnsupportedOperationException">
		///          if the attribute view is not available </exception>
		/// <exception cref="IllegalArgumentException">
		///          if no attributes are specified or an unrecognized attributes is
		///          specified </exception>
		/// <exception cref="IOException">
		///          If an I/O error occurs </exception>
		/// <exception cref="SecurityException">
		///          In the case of the default provider, and a security manager is
		///          installed, its <seealso cref="SecurityManager#checkRead(String) checkRead"/>
		///          method denies read access to the file. If this method is invoked
		///          to read security sensitive attributes then the security manager
		///          may be invoke to check for additional permissions. </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public abstract Map<String,Object> readAttributes(Path path, String attributes, LinkOption... options) throws java.io.IOException;
		public abstract Map<String, Object> ReadAttributes(Path path, String attributes, params LinkOption[] options);

		/// <summary>
		/// Sets the value of a file attribute. This method works in exactly the
		/// manner specified by the <seealso cref="Files#setAttribute"/> method.
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
		/// <exception cref="UnsupportedOperationException">
		///          if the attribute view is not available </exception>
		/// <exception cref="IllegalArgumentException">
		///          if the attribute name is not specified, or is not recognized, or
		///          the attribute value is of the correct type but has an
		///          inappropriate value </exception>
		/// <exception cref="ClassCastException">
		///          If the attribute value is not of the expected type or is a
		///          collection containing elements that are not of the expected
		///          type </exception>
		/// <exception cref="IOException">
		///          If an I/O error occurs </exception>
		/// <exception cref="SecurityException">
		///          In the case of the default provider, and a security manager is
		///          installed, its <seealso cref="SecurityManager#checkWrite(String) checkWrite"/>
		///          method denies write access to the file. If this method is invoked
		///          to set security sensitive attributes then the security manager
		///          may be invoked to check for additional permissions. </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public abstract void setAttribute(Path path, String attribute, Object value, LinkOption... options) throws java.io.IOException;
		public abstract void SetAttribute(Path path, String attribute, Object value, params LinkOption[] options);
	}

}
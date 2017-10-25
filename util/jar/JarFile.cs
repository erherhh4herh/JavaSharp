using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;

/*
 * Copyright (c) 1997, 2013, Oracle and/or its affiliates. All rights reserved.
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

namespace java.util.jar
{

	using IOUtils = sun.misc.IOUtils;
	using GetPropertyAction = sun.security.action.GetPropertyAction;
	using ManifestEntryVerifier = sun.security.util.ManifestEntryVerifier;
	using SharedSecrets = sun.misc.SharedSecrets;
	using SignatureFileVerifier = sun.security.util.SignatureFileVerifier;

	/// <summary>
	/// The <code>JarFile</code> class is used to read the contents of a jar file
	/// from any file that can be opened with <code>java.io.RandomAccessFile</code>.
	/// It extends the class <code>java.util.zip.ZipFile</code> with support
	/// for reading an optional <code>Manifest</code> entry. The
	/// <code>Manifest</code> can be used to specify meta-information about the
	/// jar file and its entries.
	/// 
	/// <para> Unless otherwise noted, passing a <tt>null</tt> argument to a constructor
	/// or method in this class will cause a <seealso cref="NullPointerException"/> to be
	/// thrown.
	/// 
	/// If the verify flag is on when opening a signed jar file, the content of the
	/// file is verified against its signature embedded inside the file. Please note
	/// that the verification process does not include validating the signer's
	/// certificate. A caller should inspect the return value of
	/// <seealso cref="JarEntry#getCodeSigners()"/> to further determine if the signature
	/// can be trusted.
	/// 
	/// @author  David Connelly
	/// </para>
	/// </summary>
	/// <seealso cref=     Manifest </seealso>
	/// <seealso cref=     java.util.zip.ZipFile </seealso>
	/// <seealso cref=     java.util.jar.JarEntry
	/// @since   1.2 </seealso>
	public class JarFile : ZipFile
	{
		private SoftReference<Manifest> ManRef;
		private JarEntry ManEntry_Renamed;
		private JarVerifier Jv;
		private bool JvInitialized;
		private bool Verify;

		// indicates if Class-Path attribute present (only valid if hasCheckedSpecialAttributes true)
		private bool HasClassPathAttribute_Renamed;
		// true if manifest checked for special attributes
		private volatile bool HasCheckedSpecialAttributes;

		// Set up JavaUtilJarAccess in SharedSecrets
		static JarFile()
		{
			SharedSecrets.JavaUtilJarAccess = new JavaUtilJarAccessImpl();
			CLASSPATH_LASTOCC = new int[128];
			CLASSPATH_OPTOSFT = new int[10];
			CLASSPATH_LASTOCC[(int)'c'] = 1;
			CLASSPATH_LASTOCC[(int)'l'] = 2;
			CLASSPATH_LASTOCC[(int)'s'] = 5;
			CLASSPATH_LASTOCC[(int)'-'] = 6;
			CLASSPATH_LASTOCC[(int)'p'] = 7;
			CLASSPATH_LASTOCC[(int)'a'] = 8;
			CLASSPATH_LASTOCC[(int)'t'] = 9;
			CLASSPATH_LASTOCC[(int)'h'] = 10;
			for (int i = 0; i < 9; i++)
			{
				CLASSPATH_OPTOSFT[i] = 10;
			}
			CLASSPATH_OPTOSFT[9] = 1;
		}

		/// <summary>
		/// The JAR manifest file name.
		/// </summary>
		public const String MANIFEST_NAME = "META-INF/MANIFEST.MF";

		/// <summary>
		/// Creates a new <code>JarFile</code> to read from the specified
		/// file <code>name</code>. The <code>JarFile</code> will be verified if
		/// it is signed. </summary>
		/// <param name="name"> the name of the jar file to be opened for reading </param>
		/// <exception cref="IOException"> if an I/O error has occurred </exception>
		/// <exception cref="SecurityException"> if access to the file is denied
		///         by the SecurityManager </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public JarFile(String name) throws IOException
		public JarFile(String name) : this(new File(name), true, ZipFile.OPEN_READ)
		{
		}

		/// <summary>
		/// Creates a new <code>JarFile</code> to read from the specified
		/// file <code>name</code>. </summary>
		/// <param name="name"> the name of the jar file to be opened for reading </param>
		/// <param name="verify"> whether or not to verify the jar file if
		/// it is signed. </param>
		/// <exception cref="IOException"> if an I/O error has occurred </exception>
		/// <exception cref="SecurityException"> if access to the file is denied
		///         by the SecurityManager </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public JarFile(String name, boolean verify) throws IOException
		public JarFile(String name, bool verify) : this(new File(name), verify, ZipFile.OPEN_READ)
		{
		}

		/// <summary>
		/// Creates a new <code>JarFile</code> to read from the specified
		/// <code>File</code> object. The <code>JarFile</code> will be verified if
		/// it is signed. </summary>
		/// <param name="file"> the jar file to be opened for reading </param>
		/// <exception cref="IOException"> if an I/O error has occurred </exception>
		/// <exception cref="SecurityException"> if access to the file is denied
		///         by the SecurityManager </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public JarFile(File file) throws IOException
		public JarFile(File file) : this(file, true, ZipFile.OPEN_READ)
		{
		}


		/// <summary>
		/// Creates a new <code>JarFile</code> to read from the specified
		/// <code>File</code> object. </summary>
		/// <param name="file"> the jar file to be opened for reading </param>
		/// <param name="verify"> whether or not to verify the jar file if
		/// it is signed. </param>
		/// <exception cref="IOException"> if an I/O error has occurred </exception>
		/// <exception cref="SecurityException"> if access to the file is denied
		///         by the SecurityManager. </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public JarFile(File file, boolean verify) throws IOException
		public JarFile(File file, bool verify) : this(file, verify, ZipFile.OPEN_READ)
		{
		}


		/// <summary>
		/// Creates a new <code>JarFile</code> to read from the specified
		/// <code>File</code> object in the specified mode.  The mode argument
		/// must be either <tt>OPEN_READ</tt> or <tt>OPEN_READ | OPEN_DELETE</tt>.
		/// </summary>
		/// <param name="file"> the jar file to be opened for reading </param>
		/// <param name="verify"> whether or not to verify the jar file if
		/// it is signed. </param>
		/// <param name="mode"> the mode in which the file is to be opened </param>
		/// <exception cref="IOException"> if an I/O error has occurred </exception>
		/// <exception cref="IllegalArgumentException">
		///         if the <tt>mode</tt> argument is invalid </exception>
		/// <exception cref="SecurityException"> if access to the file is denied
		///         by the SecurityManager
		/// @since 1.3 </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public JarFile(File file, boolean verify, int mode) throws IOException
		public JarFile(File file, bool verify, int mode) : base(file, mode)
		{
			this.Verify = verify;
		}

		/// <summary>
		/// Returns the jar file manifest, or <code>null</code> if none.
		/// </summary>
		/// <returns> the jar file manifest, or <code>null</code> if none
		/// </returns>
		/// <exception cref="IllegalStateException">
		///         may be thrown if the jar file has been closed </exception>
		/// <exception cref="IOException">  if an I/O error has occurred </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public Manifest getManifest() throws IOException
		public virtual Manifest Manifest
		{
			get
			{
				return ManifestFromReference;
			}
		}

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: private Manifest getManifestFromReference() throws IOException
		private Manifest ManifestFromReference
		{
			get
			{
				Manifest man = ManRef != null ? ManRef.get() : null;
    
				if (man == null)
				{
    
					JarEntry manEntry = ManEntry;
    
					// If found then load the manifest
					if (manEntry != null)
					{
						if (Verify)
						{
							sbyte[] b = GetBytes(manEntry);
							man = new Manifest(new ByteArrayInputStream(b));
							if (!JvInitialized)
							{
								Jv = new JarVerifier(b);
							}
						}
						else
						{
							man = new Manifest(base.GetInputStream(manEntry));
						}
						ManRef = new SoftReference<>(man);
					}
				}
				return man;
			}
		}

//JAVA TO C# CONVERTER TODO TASK: Replace 'unknown' with the appropriate dll name:
		[DllImport("unknown")]
		private extern String[] getMetaInfEntryNames();

		/// <summary>
		/// Returns the <code>JarEntry</code> for the given entry name or
		/// <code>null</code> if not found.
		/// </summary>
		/// <param name="name"> the jar file entry name </param>
		/// <returns> the <code>JarEntry</code> for the given entry name or
		///         <code>null</code> if not found.
		/// </returns>
		/// <exception cref="IllegalStateException">
		///         may be thrown if the jar file has been closed
		/// </exception>
		/// <seealso cref= java.util.jar.JarEntry </seealso>
		public virtual JarEntry GetJarEntry(String name)
		{
			return (JarEntry)GetEntry(name);
		}

		/// <summary>
		/// Returns the <code>ZipEntry</code> for the given entry name or
		/// <code>null</code> if not found.
		/// </summary>
		/// <param name="name"> the jar file entry name </param>
		/// <returns> the <code>ZipEntry</code> for the given entry name or
		///         <code>null</code> if not found
		/// </returns>
		/// <exception cref="IllegalStateException">
		///         may be thrown if the jar file has been closed
		/// </exception>
		/// <seealso cref= java.util.zip.ZipEntry </seealso>
		public override ZipEntry GetEntry(String name)
		{
			ZipEntry ze = base.GetEntry(name);
			if (ze != null)
			{
				return new JarFileEntry(this, ze);
			}
			return null;
		}

		private class JarEntryIterator : IEnumerator<JarEntry>, Iterator<JarEntry>
		{
			internal bool InstanceFieldsInitialized = false;

			internal virtual void InitializeInstanceFields()
			{
				e = OuterInstance.Entries();
			}

			private readonly JarFile OuterInstance;

			public JarEntryIterator(JarFile outerInstance)
			{
				this.OuterInstance = outerInstance;

				if (!InstanceFieldsInitialized)
				{
					InitializeInstanceFields();
					InstanceFieldsInitialized = true;
				}
			}

//JAVA TO C# CONVERTER TODO TASK: Java wildcard generics are not converted to .NET:
//ORIGINAL LINE: final java.util.Iterator<? extends ZipEntry> e = JarFile.this.entries();
			internal IEnumerator<?> e;

			public virtual bool HasNext()
			{
//JAVA TO C# CONVERTER TODO TASK: Java iterators are only converted within the context of 'while' and 'for' loops:
				return e.hasMoreElements();
			}

			public virtual JarEntry Next()
			{
//JAVA TO C# CONVERTER TODO TASK: Java iterators are only converted within the context of 'while' and 'for' loops:
				ZipEntry ze = e.nextElement();
				return new JarFileEntry(OuterInstance, ze);
			}

			public virtual bool HasMoreElements()
			{
				return HasNext();
			}

			public virtual JarEntry NextElement()
			{
				return Next();
			}
		}

		/// <summary>
		/// Returns an enumeration of the zip file entries.
		/// </summary>
		public override IEnumerator<JarEntry> Entries()
		{
			return new JarEntryIterator(this);
		}

		public override Stream<JarEntry> Stream()
		{
			return StreamSupport.Stream(Spliterators.Spliterator(new JarEntryIterator(this), Size(), Spliterator_Fields.ORDERED | Spliterator_Fields.DISTINCT | Spliterator_Fields.IMMUTABLE | Spliterator_Fields.NONNULL), false);
		}

		private class JarFileEntry : JarEntry
		{
			private readonly JarFile OuterInstance;

			internal JarFileEntry(JarFile outerInstance, ZipEntry ze) : base(ze)
			{
				this.OuterInstance = outerInstance;
			}
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public Attributes getAttributes() throws IOException
			public override Attributes Attributes
			{
				get
				{
					Manifest man = OuterInstance.Manifest;
					if (man != null)
					{
						return man.GetAttributes(Name);
					}
					else
					{
						return null;
					}
				}
			}
			public override Certificate[] Certificates
			{
				get
				{
					try
					{
						outerInstance.MaybeInstantiateVerifier();
					}
					catch (IOException e)
					{
						throw new RuntimeException(e);
					}
					if (Certs == null && outerInstance.Jv != null)
					{
						Certs = outerInstance.Jv.GetCerts(OuterInstance, this);
					}
					return Certs == null ? null : Certs.clone();
				}
			}
			public override CodeSigner[] CodeSigners
			{
				get
				{
					try
					{
						outerInstance.MaybeInstantiateVerifier();
					}
					catch (IOException e)
					{
						throw new RuntimeException(e);
					}
					if (Signers == null && outerInstance.Jv != null)
					{
						Signers = outerInstance.Jv.GetCodeSigners(OuterInstance, this);
					}
					return Signers == null ? null : Signers.clone();
				}
			}
		}

		/*
		 * Ensures that the JarVerifier has been created if one is
		 * necessary (i.e., the jar appears to be signed.) This is done as
		 * a quick check to avoid processing of the manifest for unsigned
		 * jars.
		 */
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: private void maybeInstantiateVerifier() throws IOException
		private void MaybeInstantiateVerifier()
		{
			if (Jv != null)
			{
				return;
			}

			if (Verify)
			{
				String[] names = MetaInfEntryNames;
				if (names != null)
				{
					for (int i = 0; i < names.Length; i++)
					{
						String name = names[i].ToUpperCase(Locale.ENGLISH);
						if (name.EndsWith(".DSA") || name.EndsWith(".RSA") || name.EndsWith(".EC") || name.EndsWith(".SF"))
						{
							// Assume since we found a signature-related file
							// that the jar is signed and that we therefore
							// need a JarVerifier and Manifest
							Manifest;
							return;
						}
					}
				}
				// No signature-related files; don't instantiate a
				// verifier
				Verify = false;
			}
		}


		/*
		 * Initializes the verifier object by reading all the manifest
		 * entries and passing them to the verifier.
		 */
		private void InitializeVerifier()
		{
			ManifestEntryVerifier mev = null;

			// Verify "META-INF/" entries...
			try
			{
				String[] names = MetaInfEntryNames;
				if (names != null)
				{
					for (int i = 0; i < names.Length; i++)
					{
						String uname = names[i].ToUpperCase(Locale.ENGLISH);
						if (MANIFEST_NAME.Equals(uname) || SignatureFileVerifier.isBlockOrSF(uname))
						{
							JarEntry e = GetJarEntry(names[i]);
							if (e == null)
							{
								throw new JarException("corrupted jar file");
							}
							if (mev == null)
							{
								mev = new ManifestEntryVerifier(ManifestFromReference);
							}
							sbyte[] b = GetBytes(e);
							if (b != null && b.Length > 0)
							{
								Jv.BeginEntry(e, mev);
								Jv.Update(b.Length, b, 0, b.Length, mev);
								Jv.Update(-1, null, 0, 0, mev);
							}
						}
					}
				}
			}
			catch (IOException ex)
			{
				// if we had an error parsing any blocks, just
				// treat the jar file as being unsigned
				Jv = null;
				Verify = false;
				if (JarVerifier.Debug != null)
				{
					JarVerifier.Debug.println("jarfile parsing error!");
					Console.WriteLine(ex.ToString());
					Console.Write(ex.StackTrace);
				}
			}

			// if after initializing the verifier we have nothing
			// signed, we null it out.

			if (Jv != null)
			{

				Jv.DoneWithMeta();
				if (JarVerifier.Debug != null)
				{
					JarVerifier.Debug.println("done with meta!");
				}

				if (Jv.NothingToVerify())
				{
					if (JarVerifier.Debug != null)
					{
						JarVerifier.Debug.println("nothing to verify!");
					}
					Jv = null;
					Verify = false;
				}
			}
		}

		/*
		 * Reads all the bytes for a given entry. Used to process the
		 * META-INF files.
		 */
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: private byte[] getBytes(ZipEntry ze) throws IOException
		private sbyte[] GetBytes(ZipEntry ze)
		{
			using (InputStream @is = base.GetInputStream(ze))
			{
				return IOUtils.readFully(@is, (int)ze.Size, true);
			}
		}

		/// <summary>
		/// Returns an input stream for reading the contents of the specified
		/// zip file entry. </summary>
		/// <param name="ze"> the zip file entry </param>
		/// <returns> an input stream for reading the contents of the specified
		///         zip file entry </returns>
		/// <exception cref="ZipException"> if a zip file format error has occurred </exception>
		/// <exception cref="IOException"> if an I/O error has occurred </exception>
		/// <exception cref="SecurityException"> if any of the jar file entries
		///         are incorrectly signed. </exception>
		/// <exception cref="IllegalStateException">
		///         may be thrown if the jar file has been closed </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public synchronized InputStream getInputStream(ZipEntry ze) throws IOException
		public override InputStream GetInputStream(ZipEntry ze)
		{
			lock (this)
			{
				MaybeInstantiateVerifier();
				if (Jv == null)
				{
					return base.GetInputStream(ze);
				}
				if (!JvInitialized)
				{
					InitializeVerifier();
					JvInitialized = true;
					// could be set to null after a call to
					// initializeVerifier if we have nothing to
					// verify
					if (Jv == null)
					{
						return base.GetInputStream(ze);
					}
				}
        
				// wrap a verifier stream around the real stream
				return new JarVerifier.VerifierStream(ManifestFromReference, ze is JarFileEntry ? (JarEntry) ze : GetJarEntry(ze.Name), base.GetInputStream(ze), Jv);
			}
		}

		// Statics for hand-coded Boyer-Moore search
		private static readonly char[] CLASSPATH_CHARS = new char[] {'c','l','a','s','s','-','p','a','t','h'};
		// The bad character shift for "class-path"
		private static readonly int[] CLASSPATH_LASTOCC;
		// The good suffix shift for "class-path"
		private static readonly int[] CLASSPATH_OPTOSFT;


		private JarEntry ManEntry
		{
			get
			{
				if (ManEntry_Renamed == null)
				{
					// First look up manifest entry using standard name
					ManEntry_Renamed = GetJarEntry(MANIFEST_NAME);
					if (ManEntry_Renamed == null)
					{
						// If not found, then iterate through all the "META-INF/"
						// entries to find a match.
						String[] names = MetaInfEntryNames;
						if (names != null)
						{
							for (int i = 0; i < names.Length; i++)
							{
								if (MANIFEST_NAME.Equals(names[i].ToUpperCase(Locale.ENGLISH)))
								{
									ManEntry_Renamed = GetJarEntry(names[i]);
									break;
								}
							}
						}
					}
				}
				return ManEntry_Renamed;
			}
		}

	   /// <summary>
	   /// Returns {@code true} iff this JAR file has a manifest with the
	   /// Class-Path attribute
	   /// </summary>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: boolean hasClassPathAttribute() throws IOException
		internal virtual bool HasClassPathAttribute()
		{
			CheckForSpecialAttributes();
			return HasClassPathAttribute_Renamed;
		}

		/// <summary>
		/// Returns true if the pattern {@code src} is found in {@code b}.
		/// The {@code lastOcc} and {@code optoSft} arrays are the precomputed
		/// bad character and good suffix shifts.
		/// </summary>
		private bool Match(char[] src, sbyte[] b, int[] lastOcc, int[] optoSft)
		{
			int len = src.Length;
			int last = b.Length - len;
			int i = 0;
			while (i <= last)
			{
				for (int j = (len - 1); j >= 0; j--)
				{
					char c = (char) b[i + j];
					c = (((c - 'A') | ('Z' - c)) >= 0) ? (char)(c + 32) : c;
					if (c != src[j])
					{
						i += System.Math.Max(j + 1 - lastOcc[c & 0x7F], optoSft[j]);
						goto nextContinue;
					}
				}
				return true;
				nextContinue:;
			}
			nextBreak:
			return false;
		}

		/// <summary>
		/// On first invocation, check if the JAR file has the Class-Path
		/// attribute. A no-op on subsequent calls.
		/// </summary>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: private void checkForSpecialAttributes() throws IOException
		private void CheckForSpecialAttributes()
		{
			if (HasCheckedSpecialAttributes)
			{
				return;
			}
			if (!KnownNotToHaveSpecialAttributes)
			{
				JarEntry manEntry = ManEntry;
				if (manEntry != null)
				{
					sbyte[] b = GetBytes(manEntry);
					if (Match(CLASSPATH_CHARS, b, CLASSPATH_LASTOCC, CLASSPATH_OPTOSFT))
					{
						HasClassPathAttribute_Renamed = true;
					}
				}
			}
			HasCheckedSpecialAttributes = true;
		}

		private static String JavaHome;
		private static volatile String[] JarNames;
		private bool KnownNotToHaveSpecialAttributes
		{
			get
			{
				// Optimize away even scanning of manifest for jar files we
				// deliver which don't have a class-path attribute. If one of
				// these jars is changed to include such an attribute this code
				// must be changed.
				if (JavaHome == null)
				{
					JavaHome = AccessController.doPrivileged(new GetPropertyAction("java.home"));
				}
				if (JarNames == null)
				{
					String[] names = new String[11];
					String fileSep = File.Separator;
					int i = 0;
					names[i++] = fileSep + "rt.jar";
					names[i++] = fileSep + "jsse.jar";
					names[i++] = fileSep + "jce.jar";
					names[i++] = fileSep + "charsets.jar";
					names[i++] = fileSep + "dnsns.jar";
					names[i++] = fileSep + "zipfs.jar";
					names[i++] = fileSep + "localedata.jar";
					names[i++] = fileSep = "cldrdata.jar";
					names[i++] = fileSep + "sunjce_provider.jar";
					names[i++] = fileSep + "sunpkcs11.jar";
					names[i++] = fileSep + "sunec.jar";
					JarNames = names;
				}
    
				String name = Name;
				String localJavaHome = JavaHome;
				if (name.StartsWith(localJavaHome))
				{
					String[] names = JarNames;
					for (int i = 0; i < names.Length; i++)
					{
						if (name.EndsWith(names[i]))
						{
							return true;
						}
					}
				}
				return false;
			}
		}

		private void EnsureInitialization()
		{
			lock (this)
			{
				try
				{
					MaybeInstantiateVerifier();
				}
				catch (IOException e)
				{
					throw new RuntimeException(e);
				}
				if (Jv != null && !JvInitialized)
				{
					InitializeVerifier();
					JvInitialized = true;
				}
			}
		}

		internal virtual JarEntry NewEntry(ZipEntry ze)
		{
			return new JarFileEntry(this, ze);
		}

		internal virtual IEnumerator<String> EntryNames(CodeSource[] cs)
		{
			EnsureInitialization();
			if (Jv != null)
			{
				return Jv.EntryNames(this, cs);
			}

			/*
			 * JAR file has no signed content. Is there a non-signing
			 * code source?
			 */
			bool includeUnsigned = false;
			for (int i = 0; i < cs.Length; i++)
			{
				if (cs[i].CodeSigners == null)
				{
					includeUnsigned = true;
					break;
				}
			}
			if (includeUnsigned)
			{
				return UnsignedEntryNames();
			}
			else
			{
				return new IteratorAnonymousInnerClassHelper(this);
			}
		}

		private class IteratorAnonymousInnerClassHelper : IEnumerator<String>
		{
			private readonly JarFile OuterInstance;

			public IteratorAnonymousInnerClassHelper(JarFile outerInstance)
			{
				this.OuterInstance = outerInstance;
			}


			public virtual bool HasMoreElements()
			{
				return false;
			}

			public virtual String NextElement()
			{
				throw new NoSuchElementException();
			}
		}

		/// <summary>
		/// Returns an enumeration of the zip file entries
		/// excluding internal JAR mechanism entries and including
		/// signed entries missing from the ZIP directory.
		/// </summary>
		internal virtual IEnumerator<JarEntry> Entries2()
		{
			EnsureInitialization();
			if (Jv != null)
			{
				return Jv.Entries2(this, base.Entries());
			}

			// screen out entries which are never signed
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final java.util.Iterator<? extends ZipEntry> enum_ = base.entries();
//JAVA TO C# CONVERTER TODO TASK: Java wildcard generics are not converted to .NET:
			IEnumerator<?> enum_ = base.Entries();
			return new IteratorAnonymousInnerClassHelper2(this, enum_);
		}

		private class IteratorAnonymousInnerClassHelper2 : IEnumerator<JarEntry>
		{
			private readonly JarFile OuterInstance;

//JAVA TO C# CONVERTER TODO TASK: Java wildcard generics are not converted to .NET:
//ORIGINAL LINE: private IEnumerator<JavaToDotNetGenericWildcard> enum_;
			private IEnumerator<?> Enum_;

			public IteratorAnonymousInnerClassHelper2<T1>(JarFile outerInstance, IEnumerator<T1> enum_)
			{
				this.OuterInstance = outerInstance;
				this.Enum_ = enum_;
			}


			internal ZipEntry entry;

			public virtual bool HasMoreElements()
			{
				if (entry != null)
				{
					return true;
				}
				while (Enum_.MoveNext())
				{
					ZipEntry ze = Enum_.Current;
					if (JarVerifier.IsSigningRelated(ze.Name))
					{
						continue;
					}
					entry = ze;
					return true;
				}
				return false;
			}

			public virtual JarFileEntry NextElement()
			{
				if (hasMoreElements())
				{
					ZipEntry ze = entry;
					entry = null;
					return new JarFileEntry(OuterInstance, ze);
				}
				throw new NoSuchElementException();
			}
		}

		internal virtual CodeSource[] GetCodeSources(URL url)
		{
			EnsureInitialization();
			if (Jv != null)
			{
				return Jv.GetCodeSources(this, url);
			}

			/*
			 * JAR file has no signed content. Is there a non-signing
			 * code source?
			 */
			IEnumerator<String> unsigned = UnsignedEntryNames();
//JAVA TO C# CONVERTER TODO TASK: Java iterators are only converted within the context of 'while' and 'for' loops:
			if (unsigned.hasMoreElements())
			{
				return new CodeSource[]{JarVerifier.GetUnsignedCS(url)};
			}
			else
			{
				return null;
			}
		}

		private IEnumerator<String> UnsignedEntryNames()
		{
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final java.util.Iterator<JarEntry> entries = entries();
			IEnumerator<JarEntry> entries = Entries();
			return new IteratorAnonymousInnerClassHelper3(this, entries);
		}

		private class IteratorAnonymousInnerClassHelper3 : IEnumerator<String>
		{
			private readonly JarFile OuterInstance;

			private IEnumerator<JarEntry> Entries;

			public IteratorAnonymousInnerClassHelper3(JarFile outerInstance, IEnumerator<JarEntry> entries)
			{
				this.OuterInstance = outerInstance;
				this.Entries = entries;
			}


			internal String name;

			/*
			 * Grab entries from ZIP directory but screen out
			 * metadata.
			 */
			public virtual bool HasMoreElements()
			{
				if (name != null)
				{
					return true;
				}
				while (Entries.MoveNext())
				{
					String value;
					ZipEntry e = Entries.Current;
					value = e.Name;
					if (e.Directory || JarVerifier.IsSigningRelated(value))
					{
						continue;
					}
					name = value;
					return true;
				}
				return false;
			}

			public virtual String NextElement()
			{
				if (hasMoreElements())
				{
					String value = name;
					name = null;
					return value;
				}
				throw new NoSuchElementException();
			}
		}

		internal virtual CodeSource GetCodeSource(URL url, String name)
		{
			EnsureInitialization();
			if (Jv != null)
			{
				if (Jv.EagerValidation_Renamed)
				{
					CodeSource cs = null;
					JarEntry je = GetJarEntry(name);
					if (je != null)
					{
						cs = Jv.GetCodeSource(url, this, je);
					}
					else
					{
						cs = Jv.GetCodeSource(url, name);
					}
					return cs;
				}
				else
				{
					return Jv.GetCodeSource(url, name);
				}
			}

			return JarVerifier.GetUnsignedCS(url);
		}

		internal virtual bool EagerValidation
		{
			set
			{
				try
				{
					MaybeInstantiateVerifier();
				}
				catch (IOException e)
				{
					throw new RuntimeException(e);
				}
				if (Jv != null)
				{
					Jv.EagerValidation = value;
				}
			}
		}

		internal virtual List<Object> ManifestDigests
		{
			get
			{
				EnsureInitialization();
				if (Jv != null)
				{
					return Jv.ManifestDigests;
				}
				return new List<Object>();
			}
		}
	}

}
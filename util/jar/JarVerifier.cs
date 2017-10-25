using System;
using System.Collections.Generic;

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


	using JarIndex = sun.misc.JarIndex;
	using ManifestDigester = sun.security.util.ManifestDigester;
	using ManifestEntryVerifier = sun.security.util.ManifestEntryVerifier;
	using SignatureFileVerifier = sun.security.util.SignatureFileVerifier;
	using Debug = sun.security.util.Debug;

	/// 
	/// <summary>
	/// @author      Roland Schemers
	/// </summary>
	internal class JarVerifier
	{

		/* Are we debugging ? */
		internal static readonly Debug Debug = Debug.getInstance("jar");

		/* a table mapping names to code signers, for jar entries that have
		   had their actual hashes verified */
		private Dictionary<String, CodeSigner[]> VerifiedSigners;

		/* a table mapping names to code signers, for jar entries that have
		   passed the .SF/.DSA/.EC -> MANIFEST check */
		private Dictionary<String, CodeSigner[]> SigFileSigners;

		/* a hash table to hold .SF bytes */
		private Dictionary<String, sbyte[]> SigFileData;

		/// <summary>
		/// "queue" of pending PKCS7 blocks that we couldn't parse
		///  until we parsed the .SF file 
		/// </summary>
		private List<SignatureFileVerifier> PendingBlocks;

		/* cache of CodeSigner objects */
		private List<CodeSigner[]> SignerCache;

		/* Are we parsing a block? */
		private bool ParsingBlockOrSF = false;

		/* Are we done parsing META-INF entries? */
		private bool ParsingMeta = true;

		/* Are there are files to verify? */
		private bool AnyToVerify = true;

		/* The output stream to use when keeping track of files we are interested
		   in */
		private ByteArrayOutputStream Baos;

		/// <summary>
		/// The ManifestDigester object </summary>
		private volatile ManifestDigester ManDig;

		/// <summary>
		/// the bytes for the manDig object </summary>
		internal sbyte[] ManifestRawBytes = null;

		/// <summary>
		/// controls eager signature validation </summary>
		internal bool EagerValidation_Renamed;

		/// <summary>
		/// makes code source singleton instances unique to us </summary>
		private Object Csdomain = new Object();

		/// <summary>
		/// collect -DIGEST-MANIFEST values for blacklist </summary>
		private List<Object> ManifestDigests_Renamed;

		public JarVerifier(sbyte[] rawBytes)
		{
			ManifestRawBytes = rawBytes;
			SigFileSigners = new Dictionary<>();
			VerifiedSigners = new Dictionary<>();
			SigFileData = new Dictionary<>(11);
			PendingBlocks = new List<>();
			Baos = new ByteArrayOutputStream();
			ManifestDigests_Renamed = new List<>();
		}

		/// <summary>
		/// This method scans to see which entry we're parsing and
		/// keeps various state information depending on what type of
		/// file is being parsed.
		/// </summary>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public void beginEntry(JarEntry je, sun.security.util.ManifestEntryVerifier mev) throws IOException
		public virtual void BeginEntry(JarEntry je, ManifestEntryVerifier mev)
		{
			if (je == null)
			{
				return;
			}

			if (Debug != null)
			{
				Debug.println("beginEntry " + je.Name);
			}

			String name = je.Name;

			/*
			 * Assumptions:
			 * 1. The manifest should be the first entry in the META-INF directory.
			 * 2. The .SF/.DSA/.EC files follow the manifest, before any normal entries
			 * 3. Any of the following will throw a SecurityException:
			 *    a. digest mismatch between a manifest section and
			 *       the SF section.
			 *    b. digest mismatch between the actual jar entry and the manifest
			 */

			if (ParsingMeta)
			{
				String uname = name.ToUpperCase(Locale.ENGLISH);
				if ((uname.StartsWith("META-INF/") || uname.StartsWith("/META-INF/")))
				{

					if (je.Directory)
					{
						mev.setEntry(null, je);
						return;
					}

					if (uname.Equals(JarFile.MANIFEST_NAME) || uname.Equals(JarIndex.INDEX_NAME))
					{
						return;
					}

					if (SignatureFileVerifier.isBlockOrSF(uname))
					{
						/* We parse only DSA, RSA or EC PKCS7 blocks. */
						ParsingBlockOrSF = true;
						Baos.Reset();
						mev.setEntry(null, je);
						return;
					}

					// If a META-INF entry is not MF or block or SF, they should
					// be normal entries. According to 2 above, no more block or
					// SF will appear. Let's doneWithMeta.
				}
			}

			if (ParsingMeta)
			{
				DoneWithMeta();
			}

			if (je.Directory)
			{
				mev.setEntry(null, je);
				return;
			}

			// be liberal in what you accept. If the name starts with ./, remove
			// it as we internally canonicalize it with out the ./.
			if (name.StartsWith("./"))
			{
				name = name.Substring(2);
			}

			// be liberal in what you accept. If the name starts with /, remove
			// it as we internally canonicalize it with out the /.
			if (name.StartsWith("/"))
			{
				name = name.Substring(1);
			}

			// only set the jev object for entries that have a signature
			// (either verified or not)
			if (SigFileSigners.Get(name) != null || VerifiedSigners.Get(name) != null)
			{
				mev.setEntry(name, je);
				return;
			}

			// don't compute the digest for this entry
			mev.setEntry(null, je);

			return;
		}

		/// <summary>
		/// update a single byte.
		/// </summary>

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public void update(int b, sun.security.util.ManifestEntryVerifier mev) throws IOException
		public virtual void Update(int b, ManifestEntryVerifier mev)
		{
			if (b != -1)
			{
				if (ParsingBlockOrSF)
				{
					Baos.Write(b);
				}
				else
				{
					mev.update((sbyte)b);
				}
			}
			else
			{
				ProcessEntry(mev);
			}
		}

		/// <summary>
		/// update an array of bytes.
		/// </summary>

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public void update(int n, byte[] b, int off, int len, sun.security.util.ManifestEntryVerifier mev) throws IOException
		public virtual void Update(int n, sbyte[] b, int off, int len, ManifestEntryVerifier mev)
		{
			if (n != -1)
			{
				if (ParsingBlockOrSF)
				{
					Baos.Write(b, off, n);
				}
				else
				{
					mev.update(b, off, n);
				}
			}
			else
			{
				ProcessEntry(mev);
			}
		}

		/// <summary>
		/// called when we reach the end of entry in one of the read() methods.
		/// </summary>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: private void processEntry(sun.security.util.ManifestEntryVerifier mev) throws IOException
		private void ProcessEntry(ManifestEntryVerifier mev)
		{
			if (!ParsingBlockOrSF)
			{
				JarEntry je = mev.Entry;
				if ((je != null) && (je.Signers == null))
				{
					je.Signers = mev.verify(VerifiedSigners, SigFileSigners);
					je.Certs = MapSignersToCertArray(je.Signers);
				}
			}
			else
			{

				try
				{
					ParsingBlockOrSF = false;

					if (Debug != null)
					{
						Debug.println("processEntry: processing block");
					}

					String uname = mev.Entry.Name.ToUpperCase(Locale.ENGLISH);

					if (uname.EndsWith(".SF"))
					{
						String key = uname.Substring(0, uname.Length() - 3);
						sbyte[] bytes = Baos.ToByteArray();
						// add to sigFileData in case future blocks need it
						SigFileData.Put(key, bytes);
						// check pending blocks, we can now process
						// anyone waiting for this .SF file
						Iterator<SignatureFileVerifier> it = PendingBlocks.Iterator();
						while (it.HasNext())
						{
							SignatureFileVerifier sfv = it.Next();
							if (sfv.needSignatureFile(key))
							{
								if (Debug != null)
								{
									Debug.println("processEntry: processing pending block");
								}

								sfv.SignatureFile = bytes;
								sfv.process(SigFileSigners, ManifestDigests_Renamed);
							}
						}
						return;
					}

					// now we are parsing a signature block file

					String key = uname.Substring(0, uname.LastIndexOf("."));

					if (SignerCache == null)
					{
						SignerCache = new List<>();
					}

					if (ManDig == null)
					{
						lock (ManifestRawBytes)
						{
							if (ManDig == null)
							{
								ManDig = new ManifestDigester(ManifestRawBytes);
								ManifestRawBytes = null;
							}
						}
					}

					SignatureFileVerifier sfv = new SignatureFileVerifier(SignerCache, ManDig, uname, Baos.ToByteArray());

					if (sfv.needSignatureFileBytes())
					{
						// see if we have already parsed an external .SF file
						sbyte[] bytes = SigFileData.Get(key);

						if (bytes == null)
						{
							// put this block on queue for later processing
							// since we don't have the .SF bytes yet
							// (uname, block);
							if (Debug != null)
							{
								Debug.println("adding pending block");
							}
							PendingBlocks.Add(sfv);
							return;
						}
						else
						{
							sfv.SignatureFile = bytes;
						}
					}
					sfv.process(SigFileSigners, ManifestDigests_Renamed);

				}
				catch (IOException ioe)
				{
					// e.g. sun.security.pkcs.ParsingException
					if (Debug != null)
					{
						Debug.println("processEntry caught: " + ioe);
					}
					// ignore and treat as unsigned
				}
				catch (SignatureException se)
				{
					if (Debug != null)
					{
						Debug.println("processEntry caught: " + se);
					}
					// ignore and treat as unsigned
				}
				catch (NoSuchAlgorithmException nsae)
				{
					if (Debug != null)
					{
						Debug.println("processEntry caught: " + nsae);
					}
					// ignore and treat as unsigned
				}
				catch (CertificateException ce)
				{
					if (Debug != null)
					{
						Debug.println("processEntry caught: " + ce);
					}
					// ignore and treat as unsigned
				}
			}
		}

		/// <summary>
		/// Return an array of java.security.cert.Certificate objects for
		/// the given file in the jar.
		/// @deprecated
		/// </summary>
		[Obsolete]
		public virtual java.security.cert.Certificate[] GetCerts(String name)
		{
			return MapSignersToCertArray(GetCodeSigners(name));
		}

		public virtual java.security.cert.Certificate[] GetCerts(JarFile jar, JarEntry entry)
		{
			return MapSignersToCertArray(GetCodeSigners(jar, entry));
		}

		/// <summary>
		/// return an array of CodeSigner objects for
		/// the given file in the jar. this array is not cloned.
		/// 
		/// </summary>
		public virtual CodeSigner[] GetCodeSigners(String name)
		{
			return VerifiedSigners.Get(name);
		}

		public virtual CodeSigner[] GetCodeSigners(JarFile jar, JarEntry entry)
		{
			String name = entry.Name;
			if (EagerValidation_Renamed && SigFileSigners.Get(name) != null)
			{
				/*
				 * Force a read of the entry data to generate the
				 * verification hash.
				 */
				try
				{
					InputStream s = jar.GetInputStream(entry);
					sbyte[] buffer = new sbyte[1024];
					int n = buffer.Length;
					while (n != -1)
					{
						n = s.Read(buffer, 0, buffer.Length);
					}
					s.Close();
				}
				catch (IOException)
				{
				}
			}
			return GetCodeSigners(name);
		}

		/*
		 * Convert an array of signers into an array of concatenated certificate
		 * arrays.
		 */
		private static java.security.cert.Certificate[] MapSignersToCertArray(CodeSigner[] signers)
		{

			if (signers != null)
			{
				List<java.security.cert.Certificate> certChains = new List<java.security.cert.Certificate>();
				for (int i = 0; i < signers.Length; i++)
				{
					certChains.AddAll(signers[i].SignerCertPath.Certificates);
				}

				// Convert into a Certificate[]
				return certChains.ToArray(new java.security.cert.Certificate[certChains.Size()]);
			}
			return null;
		}

		/// <summary>
		/// returns true if there no files to verify.
		/// should only be called after all the META-INF entries
		/// have been processed.
		/// </summary>
		internal virtual bool NothingToVerify()
		{
			return (AnyToVerify == false);
		}

		/// <summary>
		/// called to let us know we have processed all the
		/// META-INF entries, and if we re-read one of them, don't
		/// re-process it. Also gets rid of any data structures
		/// we needed when parsing META-INF entries.
		/// </summary>
		internal virtual void DoneWithMeta()
		{
			ParsingMeta = false;
			AnyToVerify = !SigFileSigners.Empty;
			Baos = null;
			SigFileData = null;
			PendingBlocks = null;
			SignerCache = null;
			ManDig = null;
			// MANIFEST.MF is always treated as signed and verified,
			// move its signers from sigFileSigners to verifiedSigners.
			if (SigFileSigners.ContainsKey(JarFile.MANIFEST_NAME))
			{
				CodeSigner[] codeSigners = SigFileSigners.Remove(JarFile.MANIFEST_NAME);
				VerifiedSigners.Put(JarFile.MANIFEST_NAME, codeSigners);
			}
		}

		internal class VerifierStream : java.io.InputStream
		{

			internal InputStream @is;
			internal JarVerifier Jv;
			internal ManifestEntryVerifier Mev;
			internal long NumLeft;

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: VerifierStream(Manifest man, JarEntry je, InputStream is, JarVerifier jv) throws IOException
			internal VerifierStream(Manifest man, JarEntry je, InputStream @is, JarVerifier jv)
			{
				this.@is = @is;
				this.Jv = jv;
				this.Mev = new ManifestEntryVerifier(man);
				this.Jv.BeginEntry(je, Mev);
				this.NumLeft = je.Size;
				if (this.NumLeft == 0)
				{
					this.Jv.Update(-1, this.Mev);
				}
			}

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public int read() throws IOException
			public override int Read()
			{
				if (NumLeft > 0)
				{
					int b = @is.Read();
					Jv.Update(b, Mev);
					NumLeft--;
					if (NumLeft == 0)
					{
						Jv.Update(-1, Mev);
					}
					return b;
				}
				else
				{
					return -1;
				}
			}

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public int read(byte b[] , int off, int len) throws IOException
			public override int Read(sbyte[] b, int off, int len)
			{
				if ((NumLeft > 0) && (NumLeft < len))
				{
					len = (int)NumLeft;
				}

				if (NumLeft > 0)
				{
					int n = @is.Read(b, off, len);
					Jv.Update(n, b, off, len, Mev);
					NumLeft -= n;
					if (NumLeft == 0)
					{
						Jv.Update(-1, b, off, len, Mev);
					}
					return n;
				}
				else
				{
					return -1;
				}
			}

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public void close() throws IOException
			public override void Close()
			{
				if (@is != null)
				{
					@is.Close();
				}
				@is = null;
				Mev = null;
				Jv = null;
			}

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public int available() throws IOException
			public override int Available()
			{
				return @is.Available();
			}

		}

		// Extended JavaUtilJarAccess CodeSource API Support

		private Map<URL, Map<CodeSigner[], CodeSource>> UrlToCodeSourceMap = new HashMap<URL, Map<CodeSigner[], CodeSource>>();
		private Map<CodeSigner[], CodeSource> SignerToCodeSource = new HashMap<CodeSigner[], CodeSource>();
		private URL LastURL;
		private Map<CodeSigner[], CodeSource> LastURLMap;

		/*
		 * Create a unique mapping from codeSigner cache entries to CodeSource.
		 * In theory, multiple URLs origins could map to a single locally cached
		 * and shared JAR file although in practice there will be a single URL in use.
		 */
		private CodeSource MapSignersToCodeSource(URL url, CodeSigner[] signers)
		{
			lock (this)
			{
				Map<CodeSigner[], CodeSource> map;
				if (url == LastURL)
				{
					map = LastURLMap;
				}
				else
				{
					map = UrlToCodeSourceMap.Get(url);
					if (map == null)
					{
						map = new HashMap<>();
						UrlToCodeSourceMap.Put(url, map);
					}
					LastURLMap = map;
					LastURL = url;
				}
				CodeSource cs = map.Get(signers);
				if (cs == null)
				{
					cs = new VerifierCodeSource(Csdomain, url, signers);
					SignerToCodeSource.Put(signers, cs);
				}
				return cs;
			}
		}

		private CodeSource[] MapSignersToCodeSources(URL url, List<CodeSigner[]> signers, bool unsigned)
		{
			List<CodeSource> sources = new List<CodeSource>();

			for (int i = 0; i < signers.Count; i++)
			{
				sources.Add(MapSignersToCodeSource(url, signers.Get(i)));
			}
			if (unsigned)
			{
				sources.Add(MapSignersToCodeSource(url, null));
			}
			return sources.ToArray(new CodeSource[sources.Count]);
		}
		private CodeSigner[] EmptySigner = new CodeSigner[0];

		/*
		 * Match CodeSource to a CodeSigner[] in the signer cache.
		 */
		private CodeSigner[] FindMatchingSigners(CodeSource cs)
		{
			if (cs is VerifierCodeSource)
			{
				VerifierCodeSource vcs = (VerifierCodeSource) cs;
				if (vcs.IsSameDomain(Csdomain))
				{
					return ((VerifierCodeSource) cs).PrivateSigners;
				}
			}

			/*
			 * In practice signers should always be optimized above
			 * but this handles a CodeSource of any type, just in case.
			 */
			CodeSource[] sources = MapSignersToCodeSources(cs.Location, JarCodeSigners, true);
			List<CodeSource> sourceList = new List<CodeSource>();
			for (int i = 0; i < sources.Length; i++)
			{
				sourceList.Add(sources[i]);
			}
			int j = sourceList.IndexOf(cs);
			if (j != -1)
			{
				CodeSigner[] match;
				match = ((VerifierCodeSource) sourceList.Get(j)).PrivateSigners;
				if (match == null)
				{
					match = EmptySigner;
				}
				return match;
			}
			return null;
		}

		/*
		 * Instances of this class hold uncopied references to internal
		 * signing data that can be compared by object reference identity.
		 */
		private class VerifierCodeSource : CodeSource
		{
			internal const long SerialVersionUID = -9047366145967768825L;

			internal URL Vlocation;
			internal CodeSigner[] Vsigners;
			internal java.security.cert.Certificate[] Vcerts;
			internal Object Csdomain;

			internal VerifierCodeSource(Object csdomain, URL location, CodeSigner[] signers) : base(location, signers)
			{
				this.Csdomain = csdomain;
				Vlocation = location;
				Vsigners = signers; // from signerCache
			}

			internal VerifierCodeSource(Object csdomain, URL location, java.security.cert.Certificate[] certs) : base(location, certs)
			{
				this.Csdomain = csdomain;
				Vlocation = location;
				Vcerts = certs; // from signerCache
			}

			/*
			 * All VerifierCodeSource instances are constructed based on
			 * singleton signerCache or signerCacheCert entries for each unique signer.
			 * No CodeSigner<->Certificate[] conversion is required.
			 * We use these assumptions to optimize equality comparisons.
			 */
			public override bool Equals(Object obj)
			{
				if (obj == this)
				{
					return true;
				}
				if (obj is VerifierCodeSource)
				{
					VerifierCodeSource that = (VerifierCodeSource) obj;

					/*
					 * Only compare against other per-signer singletons constructed
					 * on behalf of the same JarFile instance. Otherwise, compare
					 * things the slower way.
					 */
					if (IsSameDomain(that.Csdomain))
					{
						if (that.Vsigners != this.Vsigners || that.Vcerts != this.Vcerts)
						{
							return false;
						}
						if (that.Vlocation != null)
						{
							return that.Vlocation.Equals(this.Vlocation);
						}
						else if (this.Vlocation != null)
						{
							return this.Vlocation.Equals(that.Vlocation);
						} // both null
						else
						{
							return true;
						}
					}
				}
				return base.Equals(obj);
			}

			internal virtual bool IsSameDomain(Object csdomain)
			{
				return this.Csdomain == csdomain;
			}

			internal virtual CodeSigner[] PrivateSigners
			{
				get
				{
					return Vsigners;
				}
			}

			internal virtual java.security.cert.Certificate[] PrivateCertificates
			{
				get
				{
					return Vcerts;
				}
			}
		}
		private Map<String, CodeSigner[]> SignerMap_Renamed;

		private Map<String, CodeSigner[]> SignerMap()
		{
			lock (this)
			{
				if (SignerMap_Renamed == null)
				{
					/*
					 * Snapshot signer state so it doesn't change on us. We care
					 * only about the asserted signatures. Verification of
					 * signature validity happens via the JarEntry apis.
					 */
					SignerMap_Renamed = new HashMap<>(VerifiedSigners.Size() + SigFileSigners.Size());
					SignerMap_Renamed.PutAll(VerifiedSigners);
					SignerMap_Renamed.PutAll(SigFileSigners);
				}
				return SignerMap_Renamed;
			}
		}

//JAVA TO C# CONVERTER WARNING: 'final' parameters are not allowed in .NET:
//ORIGINAL LINE: public synchronized java.util.Iterator<String> entryNames(JarFile jar, final CodeSource[] cs)
		public virtual IEnumerator<String> EntryNames(JarFile jar, CodeSource[] cs)
		{
			lock (this)
			{
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final Map<String, CodeSigner[]> map = signerMap();
				Map<String, CodeSigner[]> map = SignerMap();
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final Iterator<Map_Entry<String, CodeSigner[]>> itor = map.entrySet().iterator();
				Iterator<Map_Entry<String, CodeSigner[]>> itor = map.EntrySet().Iterator();
				bool matchUnsigned = false;
        
				/*
				 * Grab a single copy of the CodeSigner arrays. Check
				 * to see if we can optimize CodeSigner equality test.
				 */
				List<CodeSigner[]> req = new List<CodeSigner[]>(cs.Length);
				for (int i = 0; i < cs.Length; i++)
				{
					CodeSigner[] match = FindMatchingSigners(cs[i]);
					if (match != null)
					{
						if (match.Length > 0)
						{
							req.Add(match);
						}
						else
						{
							matchUnsigned = true;
						}
					}
					else
					{
						matchUnsigned = true;
					}
				}
        
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final List<CodeSigner[]> signersReq = req;
				List<CodeSigner[]> signersReq = req;
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final java.util.Iterator<String> enum2 = (matchUnsigned) ? unsignedEntryNames(jar) : emptyEnumeration;
				IEnumerator<String> enum2 = (matchUnsigned) ? UnsignedEntryNames(jar) : emptyEnumeration;
        
				return new IteratorAnonymousInnerClassHelper(this, itor, signersReq, enum2);
			}
		}

		private class IteratorAnonymousInnerClassHelper : IEnumerator<String>
		{
			private readonly JarVerifier OuterInstance;

			private IEnumerator<Map_Entry<String, CodeSigner[]>> Itor;
			private IList<CodeSigner[]> SignersReq;
			private IEnumerator<String> Enum2;

			public IteratorAnonymousInnerClassHelper(JarVerifier outerInstance, IEnumerator<Map_Entry<String, CodeSigner[]>> itor, IList<CodeSigner[]> signersReq, IEnumerator<String> enum2)
			{
				this.OuterInstance = outerInstance;
				this.Itor = itor;
				this.SignersReq = signersReq;
				this.Enum2 = enum2;
			}


			internal String name;

			public virtual bool HasMoreElements()
			{
				if (name != null)
				{
					return true;
				}

				while (Itor.HasNext())
				{
					Map_Entry<String, CodeSigner[]> e = Itor.Next();
					if (SignersReq.Contains(e.Value))
					{
						name = e.Key;
						return true;
					}
				}
				while (Enum2.MoveNext())
				{
					name = Enum2.Current;
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

		/*
		 * Like entries() but screens out internal JAR mechanism entries
		 * and includes signed entries with no ZIP data.
		 */
//JAVA TO C# CONVERTER WARNING: 'final' parameters are not allowed in .NET:
//ORIGINAL LINE: public java.util.Iterator<JarEntry> entries2(final JarFile jar, java.util.Iterator<? extends java.util.zip.ZipEntry> e)
		public virtual IEnumerator<JarEntry> Entries2(JarFile jar, IEnumerator<T1> e) where T1 : java.util.zip.ZipEntry
		{
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final Map<String, CodeSigner[]> map = new HashMap<>();
			Map<String, CodeSigner[]> map = new HashMap<String, CodeSigner[]>();
			map.PutAll(SignerMap());
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final java.util.Iterator<? extends java.util.zip.ZipEntry> enum_ = e;
//JAVA TO C# CONVERTER TODO TASK: Java wildcard generics are not converted to .NET:
			IEnumerator<?> enum_ = e;
			return new IteratorAnonymousInnerClassHelper2(this, jar, map, enum_);
		}

		private class IteratorAnonymousInnerClassHelper2 : IEnumerator<JarEntry>
		{
			private readonly JarVerifier OuterInstance;

			private java.util.jar.JarFile Jar;
			private IDictionary<String, CodeSigner[]> Map;
//JAVA TO C# CONVERTER TODO TASK: Java wildcard generics are not converted to .NET:
//ORIGINAL LINE: private IEnumerator<JavaToDotNetGenericWildcard> enum_;
			private IEnumerator<?> Enum_;

			public IteratorAnonymousInnerClassHelper2<T1>(JarVerifier outerInstance, java.util.jar.JarFile jar, IDictionary<String, CodeSigner[]> map, IEnumerator<T1> enum_)
			{
				this.OuterInstance = outerInstance;
				this.Jar = jar;
				this.Map = map;
				this.Enum_ = enum_;
				signers = null;
			}


			internal IEnumerator<String> signers;
			internal JarEntry entry;

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
					entry = Jar.NewEntry(ze);
					return true;
				}
				if (signers == null)
				{
					signers = Collections.Enumeration(Map.KeySet());
				}
				while (signers.hasMoreElements())
				{
					String name = signers.nextElement();
					entry = Jar.NewEntry(new ZipEntry(name));
					return true;
				}

				// Any map entries left?
				return false;
			}

			public virtual JarEntry NextElement()
			{
				if (hasMoreElements())
				{
					JarEntry je = entry;
					Map.Remove(je.Name);
					entry = null;
					return je;
				}
				throw new NoSuchElementException();
			}
		}
		private IEnumerator<String> emptyEnumeration = new IteratorAnonymousInnerClassHelper3();

		private class IteratorAnonymousInnerClassHelper3 : IEnumerator<String>
		{
			public IteratorAnonymousInnerClassHelper3()
			{
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

		// true if file is part of the signature mechanism itself
		internal static bool IsSigningRelated(String name)
		{
			return SignatureFileVerifier.isSigningRelated(name);
		}

		private IEnumerator<String> UnsignedEntryNames(JarFile jar)
		{
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final Map<String, CodeSigner[]> map = signerMap();
			Map<String, CodeSigner[]> map = SignerMap();
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final java.util.Iterator<JarEntry> entries = jar.entries();
			IEnumerator<JarEntry> entries = jar.Entries();
			return new IteratorAnonymousInnerClassHelper4(this, map, entries);
		}

		private class IteratorAnonymousInnerClassHelper4 : IEnumerator<String>
		{
			private readonly JarVerifier OuterInstance;

			private IDictionary<String, CodeSigner[]> Map;
			private IEnumerator<JarEntry> Entries;

			public IteratorAnonymousInnerClassHelper4(JarVerifier outerInstance, IDictionary<String, CodeSigner[]> map, IEnumerator<JarEntry> entries)
			{
				this.OuterInstance = outerInstance;
				this.Map = map;
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
					if (e.Directory || IsSigningRelated(value))
					{
						continue;
					}
					if (Map.Get(value) == null)
					{
						name = value;
						return true;
					}
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
		private List<CodeSigner[]> JarCodeSigners_Renamed;

		private List<CodeSigner[]> JarCodeSigners
		{
			get
			{
				lock (this)
				{
					CodeSigner[] signers;
					if (JarCodeSigners_Renamed == null)
					{
						HashSet<CodeSigner[]> set = new HashSet<CodeSigner[]>();
						set.AddAll(SignerMap().Values());
						JarCodeSigners_Renamed = new List<>();
						JarCodeSigners_Renamed.AddAll(set);
					}
					return JarCodeSigners_Renamed;
				}
			}
		}

		public virtual CodeSource[] GetCodeSources(JarFile jar, URL url)
		{
			lock (this)
			{
//JAVA TO C# CONVERTER TODO TASK: Java iterators are only converted within the context of 'while' and 'for' loops:
				bool hasUnsigned = UnsignedEntryNames(jar).hasMoreElements();
        
				return MapSignersToCodeSources(url, JarCodeSigners, hasUnsigned);
			}
		}

		public virtual CodeSource GetCodeSource(URL url, String name)
		{
			CodeSigner[] signers;

			signers = SignerMap().Get(name);
			return MapSignersToCodeSource(url, signers);
		}

		public virtual CodeSource GetCodeSource(URL url, JarFile jar, JarEntry je)
		{
			CodeSigner[] signers;

			return MapSignersToCodeSource(url, GetCodeSigners(jar, je));
		}

		public virtual bool EagerValidation
		{
			set
			{
				EagerValidation_Renamed = value;
			}
		}

		public virtual List<Object> ManifestDigests
		{
			get
			{
				lock (this)
				{
					return Collections.UnmodifiableList(ManifestDigests_Renamed);
				}
			}
		}

		internal static CodeSource GetUnsignedCS(URL url)
		{
			return new VerifierCodeSource(null, url, (java.security.cert.Certificate[]) null);
		}
	}

}
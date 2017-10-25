/*
 * Copyright (c) 1997, 2011, Oracle and/or its affiliates. All rights reserved.
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

	using ManifestEntryVerifier = sun.security.util.ManifestEntryVerifier;
	using JarIndex = sun.misc.JarIndex;

	/// <summary>
	/// The <code>JarInputStream</code> class is used to read the contents of
	/// a JAR file from any input stream. It extends the class
	/// <code>java.util.zip.ZipInputStream</code> with support for reading
	/// an optional <code>Manifest</code> entry. The <code>Manifest</code>
	/// can be used to store meta-information about the JAR file and its entries.
	/// 
	/// @author  David Connelly </summary>
	/// <seealso cref=     Manifest </seealso>
	/// <seealso cref=     java.util.zip.ZipInputStream
	/// @since   1.2 </seealso>
	public class JarInputStream : ZipInputStream
	{
		private Manifest Man;
		private JarEntry First;
		private JarVerifier Jv;
		private ManifestEntryVerifier Mev;
		private readonly bool DoVerify;
		private bool TryManifest;

		/// <summary>
		/// Creates a new <code>JarInputStream</code> and reads the optional
		/// manifest. If a manifest is present, also attempts to verify
		/// the signatures if the JarInputStream is signed. </summary>
		/// <param name="in"> the actual input stream </param>
		/// <exception cref="IOException"> if an I/O error has occurred </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public JarInputStream(InputStream in) throws IOException
		public JarInputStream(InputStream @in) : this(@in, true)
		{
		}

		/// <summary>
		/// Creates a new <code>JarInputStream</code> and reads the optional
		/// manifest. If a manifest is present and verify is true, also attempts
		/// to verify the signatures if the JarInputStream is signed.
		/// </summary>
		/// <param name="in"> the actual input stream </param>
		/// <param name="verify"> whether or not to verify the JarInputStream if
		/// it is signed. </param>
		/// <exception cref="IOException"> if an I/O error has occurred </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public JarInputStream(InputStream in, boolean verify) throws IOException
		public JarInputStream(InputStream @in, bool verify) : base(@in)
		{
			this.DoVerify = verify;

			// This implementation assumes the META-INF/MANIFEST.MF entry
			// should be either the first or the second entry (when preceded
			// by the dir META-INF/). It skips the META-INF/ and then
			// "consumes" the MANIFEST.MF to initialize the Manifest object.
			JarEntry e = (JarEntry)base.NextEntry;
			if (e != null && e.Name.EqualsIgnoreCase("META-INF/"))
			{
				e = (JarEntry)base.NextEntry;
			}
			First = CheckManifest(e);
		}

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: private JarEntry checkManifest(JarEntry e) throws IOException
		private JarEntry CheckManifest(JarEntry e)
		{
			if (e != null && JarFile.MANIFEST_NAME.EqualsIgnoreCase(e.Name))
			{
				Man = new Manifest();
				sbyte[] bytes = GetBytes(new BufferedInputStream(this));
				Man.Read(new ByteArrayInputStream(bytes));
				CloseEntry();
				if (DoVerify)
				{
					Jv = new JarVerifier(bytes);
					Mev = new ManifestEntryVerifier(Man);
				}
				return (JarEntry)base.NextEntry;
			}
			return e;
		}

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: private byte[] getBytes(InputStream is) throws IOException
		private sbyte[] GetBytes(InputStream @is)
		{
			sbyte[] buffer = new sbyte[8192];
			ByteArrayOutputStream baos = new ByteArrayOutputStream(2048);
			int n;
			while ((n = @is.Read(buffer, 0, buffer.Length)) != -1)
			{
				baos.Write(buffer, 0, n);
			}
			return baos.ToByteArray();
		}

		/// <summary>
		/// Returns the <code>Manifest</code> for this JAR file, or
		/// <code>null</code> if none.
		/// </summary>
		/// <returns> the <code>Manifest</code> for this JAR file, or
		///         <code>null</code> if none. </returns>
		public virtual Manifest Manifest
		{
			get
			{
				return Man;
			}
		}

		/// <summary>
		/// Reads the next ZIP file entry and positions the stream at the
		/// beginning of the entry data. If verification has been enabled,
		/// any invalid signature detected while positioning the stream for
		/// the next entry will result in an exception. </summary>
		/// <exception cref="ZipException"> if a ZIP file error has occurred </exception>
		/// <exception cref="IOException"> if an I/O error has occurred </exception>
		/// <exception cref="SecurityException"> if any of the jar file entries
		///         are incorrectly signed. </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public ZipEntry getNextEntry() throws IOException
		public override ZipEntry NextEntry
		{
			get
			{
				JarEntry e;
				if (First == null)
				{
					e = (JarEntry)base.NextEntry;
					if (TryManifest)
					{
						e = CheckManifest(e);
						TryManifest = false;
					}
				}
				else
				{
					e = First;
					if (First.Name.EqualsIgnoreCase(JarIndex.INDEX_NAME))
					{
						TryManifest = true;
					}
					First = null;
				}
				if (Jv != null && e != null)
				{
					// At this point, we might have parsed all the meta-inf
					// entries and have nothing to verify. If we have
					// nothing to verify, get rid of the JarVerifier object.
					if (Jv.NothingToVerify() == true)
					{
						Jv = null;
						Mev = null;
					}
					else
					{
						Jv.BeginEntry(e, Mev);
					}
				}
				return e;
			}
		}

		/// <summary>
		/// Reads the next JAR file entry and positions the stream at the
		/// beginning of the entry data. If verification has been enabled,
		/// any invalid signature detected while positioning the stream for
		/// the next entry will result in an exception. </summary>
		/// <returns> the next JAR file entry, or null if there are no more entries </returns>
		/// <exception cref="ZipException"> if a ZIP file error has occurred </exception>
		/// <exception cref="IOException"> if an I/O error has occurred </exception>
		/// <exception cref="SecurityException"> if any of the jar file entries
		///         are incorrectly signed. </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public JarEntry getNextJarEntry() throws IOException
		public virtual JarEntry NextJarEntry
		{
			get
			{
				return (JarEntry)NextEntry;
			}
		}

		/// <summary>
		/// Reads from the current JAR file entry into an array of bytes.
		/// If <code>len</code> is not zero, the method
		/// blocks until some input is available; otherwise, no
		/// bytes are read and <code>0</code> is returned.
		/// If verification has been enabled, any invalid signature
		/// on the current entry will be reported at some point before the
		/// end of the entry is reached. </summary>
		/// <param name="b"> the buffer into which the data is read </param>
		/// <param name="off"> the start offset in the destination array <code>b</code> </param>
		/// <param name="len"> the maximum number of bytes to read </param>
		/// <returns> the actual number of bytes read, or -1 if the end of the
		///         entry is reached </returns>
		/// <exception cref="NullPointerException"> If <code>b</code> is <code>null</code>. </exception>
		/// <exception cref="IndexOutOfBoundsException"> If <code>off</code> is negative,
		/// <code>len</code> is negative, or <code>len</code> is greater than
		/// <code>b.length - off</code> </exception>
		/// <exception cref="ZipException"> if a ZIP file error has occurred </exception>
		/// <exception cref="IOException"> if an I/O error has occurred </exception>
		/// <exception cref="SecurityException"> if any of the jar file entries
		///         are incorrectly signed. </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public int read(byte[] b, int off, int len) throws IOException
		public override int Read(sbyte[] b, int off, int len)
		{
			int n;
			if (First == null)
			{
				n = base.Read(b, off, len);
			}
			else
			{
				n = -1;
			}
			if (Jv != null)
			{
				Jv.Update(n, b, off, len, Mev);
			}
			return n;
		}

		/// <summary>
		/// Creates a new <code>JarEntry</code> (<code>ZipEntry</code>) for the
		/// specified JAR file entry name. The manifest attributes of
		/// the specified JAR file entry name will be copied to the new
		/// <CODE>JarEntry</CODE>.
		/// </summary>
		/// <param name="name"> the name of the JAR/ZIP file entry </param>
		/// <returns> the <code>JarEntry</code> object just created </returns>
		protected internal override ZipEntry CreateZipEntry(String name)
		{
			JarEntry e = new JarEntry(name);
			if (Man != null)
			{
				e.Attr = Man.GetAttributes(name);
			}
			return e;
		}
	}

}
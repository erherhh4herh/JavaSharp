/*
 * Copyright (c) 1997, 2012, Oracle and/or its affiliates. All rights reserved.
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


	/// <summary>
	/// The <code>JarOutputStream</code> class is used to write the contents
	/// of a JAR file to any output stream. It extends the class
	/// <code>java.util.zip.ZipOutputStream</code> with support
	/// for writing an optional <code>Manifest</code> entry. The
	/// <code>Manifest</code> can be used to specify meta-information about
	/// the JAR file and its entries.
	/// 
	/// @author  David Connelly </summary>
	/// <seealso cref=     Manifest </seealso>
	/// <seealso cref=     java.util.zip.ZipOutputStream
	/// @since   1.2 </seealso>
	public class JarOutputStream : ZipOutputStream
	{
		private const int JAR_MAGIC = 0xCAFE;

		/// <summary>
		/// Creates a new <code>JarOutputStream</code> with the specified
		/// <code>Manifest</code>. The manifest is written as the first
		/// entry to the output stream.
		/// </summary>
		/// <param name="out"> the actual output stream </param>
		/// <param name="man"> the optional <code>Manifest</code> </param>
		/// <exception cref="IOException"> if an I/O error has occurred </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public JarOutputStream(OutputStream out, Manifest man) throws IOException
		public JarOutputStream(OutputStream @out, Manifest man) : base(@out)
		{
			if (man == null)
			{
				throw new NullPointerException("man");
			}
			ZipEntry e = new ZipEntry(JarFile.MANIFEST_NAME);
			PutNextEntry(e);
			man.Write(new BufferedOutputStream(this));
			CloseEntry();
		}

		/// <summary>
		/// Creates a new <code>JarOutputStream</code> with no manifest. </summary>
		/// <param name="out"> the actual output stream </param>
		/// <exception cref="IOException"> if an I/O error has occurred </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public JarOutputStream(OutputStream out) throws IOException
		public JarOutputStream(OutputStream @out) : base(@out)
		{
		}

		/// <summary>
		/// Begins writing a new JAR file entry and positions the stream
		/// to the start of the entry data. This method will also close
		/// any previous entry. The default compression method will be
		/// used if no compression method was specified for the entry.
		/// The current time will be used if the entry has no set modification
		/// time.
		/// </summary>
		/// <param name="ze"> the ZIP/JAR entry to be written </param>
		/// <exception cref="ZipException"> if a ZIP error has occurred </exception>
		/// <exception cref="IOException"> if an I/O error has occurred </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public void putNextEntry(ZipEntry ze) throws IOException
		public override void PutNextEntry(ZipEntry ze)
		{
			if (FirstEntry)
			{
				// Make sure that extra field data for first JAR
				// entry includes JAR magic number id.
				sbyte[] edata = ze.Extra;
				if (edata == null || !HasMagic(edata))
				{
					if (edata == null)
					{
						edata = new sbyte[4];
					}
					else
					{
						// Prepend magic to existing extra data
						sbyte[] tmp = new sbyte[edata.Length + 4];
						System.Array.Copy(edata, 0, tmp, 4, edata.Length);
						edata = tmp;
					}
					Set16(edata, 0, JAR_MAGIC); // extra field id
					Set16(edata, 2, 0); // extra field size
					ze.Extra = edata;
				}
				FirstEntry = false;
			}
			base.PutNextEntry(ze);
		}

		private bool FirstEntry = true;

		/*
		 * Returns true if specified byte array contains the
		 * jar magic extra field id.
		 */
		private static bool HasMagic(sbyte[] edata)
		{
			try
			{
				int i = 0;
				while (i < edata.Length)
				{
					if (Get16(edata, i) == JAR_MAGIC)
					{
						return true;
					}
					i += Get16(edata, i + 2) + 4;
				}
			}
			catch (ArrayIndexOutOfBoundsException)
			{
				// Invalid extra field data
			}
			return false;
		}

		/*
		 * Fetches unsigned 16-bit value from byte array at specified offset.
		 * The bytes are assumed to be in Intel (little-endian) byte order.
		 */
		private static int Get16(sbyte[] b, int off)
		{
			return Byte.ToUnsignedInt(b[off]) | (Byte.ToUnsignedInt(b[off + 1]) << 8);
		}

		/*
		 * Sets 16-bit value at specified offset. The bytes are assumed to
		 * be in Intel (little-endian) byte order.
		 */
		private static void Set16(sbyte[] b, int off, int value)
		{
			b[off + 0] = (sbyte)value;
			b[off + 1] = (sbyte)(value >> 8);
		}
	}

}
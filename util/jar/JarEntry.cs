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


	/// <summary>
	/// This class is used to represent a JAR file entry.
	/// </summary>
	public class JarEntry : ZipEntry
	{
		internal Attributes Attr;
		internal Certificate[] Certs;
		internal CodeSigner[] Signers;

		/// <summary>
		/// Creates a new <code>JarEntry</code> for the specified JAR file
		/// entry name.
		/// </summary>
		/// <param name="name"> the JAR file entry name </param>
		/// <exception cref="NullPointerException"> if the entry name is <code>null</code> </exception>
		/// <exception cref="IllegalArgumentException"> if the entry name is longer than
		///            0xFFFF bytes. </exception>
		public JarEntry(String name) : base(name)
		{
		}

		/// <summary>
		/// Creates a new <code>JarEntry</code> with fields taken from the
		/// specified <code>ZipEntry</code> object. </summary>
		/// <param name="ze"> the <code>ZipEntry</code> object to create the
		///           <code>JarEntry</code> from </param>
		public JarEntry(ZipEntry ze) : base(ze)
		{
		}

		/// <summary>
		/// Creates a new <code>JarEntry</code> with fields taken from the
		/// specified <code>JarEntry</code> object.
		/// </summary>
		/// <param name="je"> the <code>JarEntry</code> to copy </param>
		public JarEntry(JarEntry je) : this((ZipEntry)je)
		{
			this.Attr = je.Attr;
			this.Certs = je.Certs;
			this.Signers = je.Signers;
		}

		/// <summary>
		/// Returns the <code>Manifest</code> <code>Attributes</code> for this
		/// entry, or <code>null</code> if none.
		/// </summary>
		/// <returns> the <code>Manifest</code> <code>Attributes</code> for this
		/// entry, or <code>null</code> if none </returns>
		/// <exception cref="IOException">  if an I/O error has occurred </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public Attributes getAttributes() throws java.io.IOException
		public virtual Attributes Attributes
		{
			get
			{
				return Attr;
			}
		}

		/// <summary>
		/// Returns the <code>Certificate</code> objects for this entry, or
		/// <code>null</code> if none. This method can only be called once
		/// the <code>JarEntry</code> has been completely verified by reading
		/// from the entry input stream until the end of the stream has been
		/// reached. Otherwise, this method will return <code>null</code>.
		/// 
		/// <para>The returned certificate array comprises all the signer certificates
		/// that were used to verify this entry. Each signer certificate is
		/// followed by its supporting certificate chain (which may be empty).
		/// Each signer certificate and its supporting certificate chain are ordered
		/// bottom-to-top (i.e., with the signer certificate first and the (root)
		/// certificate authority last).
		/// 
		/// </para>
		/// </summary>
		/// <returns> the <code>Certificate</code> objects for this entry, or
		/// <code>null</code> if none. </returns>
		public virtual Certificate[] Certificates
		{
			get
			{
				return Certs == null ? null : Certs.clone();
			}
		}

		/// <summary>
		/// Returns the <code>CodeSigner</code> objects for this entry, or
		/// <code>null</code> if none. This method can only be called once
		/// the <code>JarEntry</code> has been completely verified by reading
		/// from the entry input stream until the end of the stream has been
		/// reached. Otherwise, this method will return <code>null</code>.
		/// 
		/// <para>The returned array comprises all the code signers that have signed
		/// this entry.
		/// 
		/// </para>
		/// </summary>
		/// <returns> the <code>CodeSigner</code> objects for this entry, or
		/// <code>null</code> if none.
		/// 
		/// @since 1.5 </returns>
		public virtual CodeSigner[] CodeSigners
		{
			get
			{
				return Signers == null ? null : Signers.clone();
			}
		}
	}

}
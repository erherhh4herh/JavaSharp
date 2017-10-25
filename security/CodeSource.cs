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

namespace java.security
{



	/// 
	/// <summary>
	/// <para>This class extends the concept of a codebase to
	/// encapsulate not only the location (URL) but also the certificate chains
	/// that were used to verify signed code originating from that location.
	/// 
	/// @author Li Gong
	/// @author Roland Schemers
	/// </para>
	/// </summary>

	[Serializable]
	public class CodeSource
	{

		private const long SerialVersionUID = 4977541819976013951L;

		/// <summary>
		/// The code location.
		/// 
		/// @serial
		/// </summary>
		private URL Location_Renamed;

		/*
		 * The code signers.
		 */
		[NonSerialized]
		private CodeSigner[] Signers = null;

		/*
		 * The code signers. Certificate chains are concatenated.
		 */
		[NonSerialized]
		private java.security.cert.Certificate[] Certs = null;

		// cached SocketPermission used for matchLocation
		[NonSerialized]
		private SocketPermission Sp;

		// for generating cert paths
		[NonSerialized]
		private CertificateFactory Factory = null;

		/// <summary>
		/// Constructs a CodeSource and associates it with the specified
		/// location and set of certificates.
		/// </summary>
		/// <param name="url"> the location (URL).
		/// </param>
		/// <param name="certs"> the certificate(s). It may be null. The contents of the
		/// array are copied to protect against subsequent modification. </param>
		public CodeSource(URL url, java.security.cert.Certificate[] certs)
		{
			this.Location_Renamed = url;

			// Copy the supplied certs
			if (certs != null)
			{
				this.Certs = certs.clone();
			}
		}

		/// <summary>
		/// Constructs a CodeSource and associates it with the specified
		/// location and set of code signers.
		/// </summary>
		/// <param name="url"> the location (URL). </param>
		/// <param name="signers"> the code signers. It may be null. The contents of the
		/// array are copied to protect against subsequent modification.
		/// 
		/// @since 1.5 </param>
		public CodeSource(URL url, CodeSigner[] signers)
		{
			this.Location_Renamed = url;

			// Copy the supplied signers
			if (signers != null)
			{
				this.Signers = signers.clone();
			}
		}

		/// <summary>
		/// Returns the hash code value for this object.
		/// </summary>
		/// <returns> a hash code value for this object. </returns>
		public override int HashCode()
		{
			if (Location_Renamed != null)
			{
				return Location_Renamed.HashCode();
			}
			else
			{
				return 0;
			}
		}

		/// <summary>
		/// Tests for equality between the specified object and this
		/// object. Two CodeSource objects are considered equal if their
		/// locations are of identical value and if their signer certificate
		/// chains are of identical value. It is not required that
		/// the certificate chains be in the same order.
		/// </summary>
		/// <param name="obj"> the object to test for equality with this object.
		/// </param>
		/// <returns> true if the objects are considered equal, false otherwise. </returns>
		public override bool Equals(Object obj)
		{
			if (obj == this)
			{
				return true;
			}

			// objects types must be equal
			if (!(obj is CodeSource))
			{
				return false;
			}

			CodeSource cs = (CodeSource) obj;

			// URLs must match
			if (Location_Renamed == null)
			{
				// if location is null, then cs.location must be null as well
				if (cs.Location_Renamed != null)
				{
					return false;
				}
			}
			else
			{
				// if location is not null, then it must equal cs.location
				if (!Location_Renamed.Equals(cs.Location_Renamed))
				{
					return false;
				}
			}

			// certs must match
			return MatchCerts(cs, true);
		}

		/// <summary>
		/// Returns the location associated with this CodeSource.
		/// </summary>
		/// <returns> the location (URL). </returns>
		public URL Location
		{
			get
			{
				/* since URL is practically immutable, returning itself is not
				   a security problem */
				return this.Location_Renamed;
			}
		}

		/// <summary>
		/// Returns the certificates associated with this CodeSource.
		/// <para>
		/// If this CodeSource object was created using the
		/// <seealso cref="#CodeSource(URL url, CodeSigner[] signers)"/>
		/// constructor then its certificate chains are extracted and used to
		/// create an array of Certificate objects. Each signer certificate is
		/// followed by its supporting certificate chain (which may be empty).
		/// Each signer certificate and its supporting certificate chain is ordered
		/// bottom-to-top (i.e., with the signer certificate first and the (root)
		/// certificate authority last).
		/// 
		/// </para>
		/// </summary>
		/// <returns> A copy of the certificates array, or null if there is none. </returns>
		public java.security.cert.Certificate[] Certificates
		{
			get
			{
				if (Certs != null)
				{
					return Certs.clone();
    
				}
				else if (Signers != null)
				{
					// Convert the code signers to certs
					List<java.security.cert.Certificate> certChains = new List<java.security.cert.Certificate>();
					for (int i = 0; i < Signers.Length; i++)
					{
						certChains.AddRange(Signers[i].SignerCertPath.Certificates);
					}
					Certs = certChains.ToArray();
					return Certs.clone();
    
				}
				else
				{
					return null;
				}
			}
		}

		/// <summary>
		/// Returns the code signers associated with this CodeSource.
		/// <para>
		/// If this CodeSource object was created using the
		/// <seealso cref="#CodeSource(URL url, java.security.cert.Certificate[] certs)"/>
		/// constructor then its certificate chains are extracted and used to
		/// create an array of CodeSigner objects. Note that only X.509 certificates
		/// are examined - all other certificate types are ignored.
		/// 
		/// </para>
		/// </summary>
		/// <returns> A copy of the code signer array, or null if there is none.
		/// 
		/// @since 1.5 </returns>
		public CodeSigner[] CodeSigners
		{
			get
			{
				if (Signers != null)
				{
					return Signers.clone();
    
				}
				else if (Certs != null)
				{
					// Convert the certs to code signers
					Signers = ConvertCertArrayToSignerArray(Certs);
					return Signers.clone();
    
				}
				else
				{
					return null;
				}
			}
		}

		/// <summary>
		/// Returns true if this CodeSource object "implies" the specified CodeSource.
		/// <para>
		/// More specifically, this method makes the following checks.
		/// If any fail, it returns false. If they all succeed, it returns true.
		/// <ul>
		/// <li> <i>codesource</i> must not be null.
		/// <li> If this object's certificates are not null, then all
		/// of this object's certificates must be present in <i>codesource</i>'s
		/// certificates.
		/// <li> If this object's location (getLocation()) is not null, then the
		/// following checks are made against this object's location and
		/// <i>codesource</i>'s:
		///   <ul>
		///     <li>  <i>codesource</i>'s location must not be null.
		/// 
		///     <li>  If this object's location
		///           equals <i>codesource</i>'s location, then return true.
		/// 
		///     <li>  This object's protocol (getLocation().getProtocol()) must be
		///           equal to <i>codesource</i>'s protocol, ignoring case.
		/// 
		///     <li>  If this object's host (getLocation().getHost()) is not null,
		///           then the SocketPermission
		///           constructed with this object's host must imply the
		///           SocketPermission constructed with <i>codesource</i>'s host.
		/// 
		///     <li>  If this object's port (getLocation().getPort()) is not
		///           equal to -1 (that is, if a port is specified), it must equal
		///           <i>codesource</i>'s port or default port
		///           (codesource.getLocation().getDefaultPort()).
		/// 
		///     <li>  If this object's file (getLocation().getFile()) doesn't equal
		///           <i>codesource</i>'s file, then the following checks are made:
		///           If this object's file ends with "/-",
		///           then <i>codesource</i>'s file must start with this object's
		///           file (exclusive the trailing "-").
		///           If this object's file ends with a "/*",
		///           then <i>codesource</i>'s file must start with this object's
		///           file and must not have any further "/" separators.
		///           If this object's file doesn't end with a "/",
		///           then <i>codesource</i>'s file must match this object's
		///           file with a '/' appended.
		/// 
		///     <li>  If this object's reference (getLocation().getRef()) is
		///           not null, it must equal <i>codesource</i>'s reference.
		/// 
		///   </ul>
		/// </ul>
		/// </para>
		/// <para>
		/// For example, the codesource objects with the following locations
		/// and null certificates all imply
		/// the codesource with the location "http://java.sun.com/classes/foo.jar"
		/// and null certificates:
		/// <pre>
		///     http:
		///     http://*.sun.com/classes/*
		///     http://java.sun.com/classes/-
		///     http://java.sun.com/classes/foo.jar
		/// </pre>
		/// 
		/// Note that if this CodeSource has a null location and a null
		/// certificate chain, then it implies every other CodeSource.
		/// 
		/// </para>
		/// </summary>
		/// <param name="codesource"> CodeSource to compare against.
		/// </param>
		/// <returns> true if the specified codesource is implied by this codesource,
		/// false if not. </returns>

		public virtual bool Implies(CodeSource codesource)
		{
			if (codesource == null)
			{
				return false;
			}

			return MatchCerts(codesource, false) && MatchLocation(codesource);
		}

		/// <summary>
		/// Returns true if all the certs in this
		/// CodeSource are also in <i>that</i>.
		/// </summary>
		/// <param name="that"> the CodeSource to check against. </param>
		/// <param name="strict"> If true then a strict equality match is performed.
		///               Otherwise a subset match is performed. </param>
		private bool MatchCerts(CodeSource that, bool strict)
		{
			bool match;

			// match any key
			if (Certs == null && Signers == null)
			{
				if (strict)
				{
					return (that.Certs == null && that.Signers == null);
				}
				else
				{
					return true;
				}
			// both have signers
			}
			else if (Signers != null && that.Signers != null)
			{
				if (strict && Signers.Length != that.Signers.Length)
				{
					return false;
				}
				for (int i = 0; i < Signers.Length; i++)
				{
					match = false;
					for (int j = 0; j < that.Signers.Length; j++)
					{
						if (Signers[i].Equals(that.Signers[j]))
						{
							match = true;
							break;
						}
					}
					if (!match)
					{
						return false;
					}
				}
				return true;

			// both have certs
			}
			else if (Certs != null && that.Certs != null)
			{
				if (strict && Certs.Length != that.Certs.Length)
				{
					return false;
				}
				for (int i = 0; i < Certs.Length; i++)
				{
					match = false;
					for (int j = 0; j < that.Certs.Length; j++)
					{
						if (Certs[i].Equals(that.Certs[j]))
						{
							match = true;
							break;
						}
					}
					if (!match)
					{
						return false;
					}
				}
				return true;
			}

			return false;
		}


		/// <summary>
		/// Returns true if two CodeSource's have the "same" location.
		/// </summary>
		/// <param name="that"> CodeSource to compare against </param>
		private bool MatchLocation(CodeSource that)
		{
			if (Location_Renamed == null)
			{
				return true;
			}

			if ((that == null) || (that.Location_Renamed == null))
			{
				return false;
			}

			if (Location_Renamed.Equals(that.Location_Renamed))
			{
				return true;
			}

			if (!Location_Renamed.Protocol.EqualsIgnoreCase(that.Location_Renamed.Protocol))
			{
				return false;
			}

			int thisPort = Location_Renamed.Port;
			if (thisPort != -1)
			{
				int thatPort = that.Location_Renamed.Port;
				int port = thatPort != -1 ? thatPort : that.Location_Renamed.DefaultPort;
				if (thisPort != port)
				{
					return false;
				}
			}

			if (Location_Renamed.File.EndsWith("/-"))
			{
				// Matches the directory and (recursively) all files
				// and subdirectories contained in that directory.
				// For example, "/a/b/-" implies anything that starts with
				// "/a/b/"
				String thisPath = Location_Renamed.File.Substring(0, Location_Renamed.File.Length() - 1);
				if (!that.Location_Renamed.File.StartsWith(thisPath))
				{
					return false;
				}
			}
			else if (Location_Renamed.File.EndsWith("/*"))
			{
				// Matches the directory and all the files contained in that
				// directory.
				// For example, "/a/b/*" implies anything that starts with
				// "/a/b/" but has no further slashes
				int last = that.Location_Renamed.File.LastIndexOf('/');
				if (last == -1)
				{
					return false;
				}
				String thisPath = Location_Renamed.File.Substring(0, Location_Renamed.File.Length() - 1);
				String thatPath = that.Location_Renamed.File.Substring(0, last + 1);
				if (!thatPath.Equals(thisPath))
				{
					return false;
				}
			}
			else
			{
				// Exact matches only.
				// For example, "/a/b" and "/a/b/" both imply "/a/b/"
				if ((!that.Location_Renamed.File.Equals(Location_Renamed.File)) && (!that.Location_Renamed.File.Equals(Location_Renamed.File + "/")))
				{
					return false;
				}
			}

			if (Location_Renamed.Ref != null && !Location_Renamed.Ref.Equals(that.Location_Renamed.Ref))
			{
				return false;
			}

			String thisHost = Location_Renamed.Host;
			String thatHost = that.Location_Renamed.Host;
			if (thisHost != null)
			{
				if (("".Equals(thisHost) || "localhost".Equals(thisHost)) && ("".Equals(thatHost) || "localhost".Equals(thatHost)))
				{
					// ok
				}
				else if (!thisHost.Equals(thatHost))
				{
					if (thatHost == null)
					{
						return false;
					}
					if (this.Sp == null)
					{
						this.Sp = new SocketPermission(thisHost, "resolve");
					}
					if (that.Sp == null)
					{
						that.Sp = new SocketPermission(thatHost, "resolve");
					}
					if (!this.Sp.Implies(that.Sp))
					{
						return false;
					}
				}
			}
			// everything matches
			return true;
		}

		/// <summary>
		/// Returns a string describing this CodeSource, telling its
		/// URL and certificates.
		/// </summary>
		/// <returns> information about this CodeSource. </returns>
		public override String ToString()
		{
			StringBuilder sb = new StringBuilder();
			sb.Append("(");
			sb.Append(this.Location_Renamed);

			if (this.Certs != null && this.Certs.Length > 0)
			{
				for (int i = 0; i < this.Certs.Length; i++)
				{
					sb.Append(" " + this.Certs[i]);
				}

			}
			else if (this.Signers != null && this.Signers.Length > 0)
			{
				for (int i = 0; i < this.Signers.Length; i++)
				{
					sb.Append(" " + this.Signers[i]);
				}
			}
			else
			{
				sb.Append(" <no signer certificates>");
			}
			sb.Append(")");
			return sb.ToString();
		}

		/// <summary>
		/// Writes this object out to a stream (i.e., serializes it).
		/// 
		/// @serialData An initial {@code URL} is followed by an
		/// {@code int} indicating the number of certificates to follow
		/// (a value of "zero" denotes that there are no certificates associated
		/// with this object).
		/// Each certificate is written out starting with a {@code String}
		/// denoting the certificate type, followed by an
		/// {@code int} specifying the length of the certificate encoding,
		/// followed by the certificate encoding itself which is written out as an
		/// array of bytes. Finally, if any code signers are present then the array
		/// of code signers is serialized and written out too.
		/// </summary>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: private void writeObject(java.io.ObjectOutputStream oos) throws java.io.IOException
		private void WriteObject(java.io.ObjectOutputStream oos)
		{
			oos.DefaultWriteObject(); // location

			// Serialize the array of certs
			if (Certs == null || Certs.Length == 0)
			{
				oos.WriteInt(0);
			}
			else
			{
				// write out the total number of certs
				oos.WriteInt(Certs.Length);
				// write out each cert, including its type
				for (int i = 0; i < Certs.Length; i++)
				{
					java.security.cert.Certificate cert = Certs[i];
					try
					{
						oos.WriteUTF(cert.Type);
						sbyte[] encoded = cert.Encoded;
						oos.WriteInt(encoded.Length);
						oos.Write(encoded);
					}
					catch (CertificateEncodingException cee)
					{
						throw new IOException(cee.Message);
					}
				}
			}

			// Serialize the array of code signers (if any)
			if (Signers != null && Signers.Length > 0)
			{
				oos.WriteObject(Signers);
			}
		}

		/// <summary>
		/// Restores this object from a stream (i.e., deserializes it).
		/// </summary>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: private void readObject(java.io.ObjectInputStream ois) throws java.io.IOException, ClassNotFoundException
		private void ReadObject(java.io.ObjectInputStream ois)
		{
			CertificateFactory cf;
			Dictionary<String, CertificateFactory> cfs = null;

			ois.DefaultReadObject(); // location

			// process any new-style certs in the stream (if present)
			int size = ois.ReadInt();
			if (size > 0)
			{
				// we know of 3 different cert types: X.509, PGP, SDSI, which
				// could all be present in the stream at the same time
				cfs = new Dictionary<String, CertificateFactory>(3);
				this.Certs = new java.security.cert.Certificate[size];
			}

			for (int i = 0; i < size; i++)
			{
				// read the certificate type, and instantiate a certificate
				// factory of that type (reuse existing factory if possible)
				String certType = ois.ReadUTF();
				if (cfs.ContainsKey(certType))
				{
					// reuse certificate factory
					cf = cfs[certType];
				}
				else
				{
					// create new certificate factory
					try
					{
						cf = CertificateFactory.GetInstance(certType);
					}
					catch (CertificateException)
					{
						throw new ClassNotFoundException("Certificate factory for " + certType + " not found");
					}
					// store the certificate factory so we can reuse it later
					cfs[certType] = cf;
				}
				// parse the certificate
				sbyte[] encoded = null;
				try
				{
					encoded = new sbyte[ois.ReadInt()];
				}
				catch (OutOfMemoryError)
				{
					throw new IOException("Certificate too big");
				}
				ois.ReadFully(encoded);
				ByteArrayInputStream bais = new ByteArrayInputStream(encoded);
				try
				{
					this.Certs[i] = cf.GenerateCertificate(bais);
				}
				catch (CertificateException ce)
				{
					throw new IOException(ce.Message);
				}
				bais.Close();
			}

			// Deserialize array of code signers (if any)
			try
			{
				this.Signers = ((CodeSigner[])ois.ReadObject()).clone();
			}
			catch (IOException)
			{
				// no signers present
			}
		}

		/*
		 * Convert an array of certificates to an array of code signers.
		 * The array of certificates is a concatenation of certificate chains
		 * where the initial certificate in each chain is the end-entity cert.
		 *
		 * @return An array of code signers or null if none are generated.
		 */
		private CodeSigner[] ConvertCertArrayToSignerArray(java.security.cert.Certificate[] certs)
		{

			if (certs == null)
			{
				return null;
			}

			try
			{
				// Initialize certificate factory
				if (Factory == null)
				{
					Factory = CertificateFactory.GetInstance("X.509");
				}

				// Iterate through all the certificates
				int i = 0;
				IList<CodeSigner> signers = new List<CodeSigner>();
				while (i < certs.Length)
				{
					IList<java.security.cert.Certificate> certChain = new List<java.security.cert.Certificate>();
					certChain.Add(certs[i++]); // first cert is an end-entity cert
					int j = i;

					// Extract chain of certificates
					// (loop while certs are not end-entity certs)
					while (j < certs.Length && certs[j] is X509Certificate && ((X509Certificate)certs[j]).BasicConstraints != -1)
					{
						certChain.Add(certs[j]);
						j++;
					}
					i = j;
					CertPath certPath = Factory.GenerateCertPath(certChain);
					signers.Add(new CodeSigner(certPath, null));
				}

				if (signers.Count == 0)
				{
					return null;
				}
				else
				{
					return signers.ToArray();
				}

			}
			catch (CertificateException)
			{
				return null; //TODO - may be better to throw an ex. here
			}
		}
	}

}
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


	/// <summary>
	/// The Manifest class is used to maintain Manifest entry names and their
	/// associated Attributes. There are main Manifest Attributes as well as
	/// per-entry Attributes. For information on the Manifest format, please
	/// see the
	/// <a href="../../../../technotes/guides/jar/jar.html">
	/// Manifest format specification</a>.
	/// 
	/// @author  David Connelly </summary>
	/// <seealso cref=     Attributes
	/// @since   1.2 </seealso>
	public class Manifest : Cloneable
	{
		// manifest main attributes
		private Attributes Attr = new Attributes();

		// manifest entries
		private IDictionary<String, Attributes> Entries_Renamed = new Dictionary<String, Attributes>();

		/// <summary>
		/// Constructs a new, empty Manifest.
		/// </summary>
		public Manifest()
		{
		}

		/// <summary>
		/// Constructs a new Manifest from the specified input stream.
		/// </summary>
		/// <param name="is"> the input stream containing manifest data </param>
		/// <exception cref="IOException"> if an I/O error has occurred </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public Manifest(java.io.InputStream is) throws java.io.IOException
		public Manifest(InputStream @is)
		{
			Read(@is);
		}

		/// <summary>
		/// Constructs a new Manifest that is a copy of the specified Manifest.
		/// </summary>
		/// <param name="man"> the Manifest to copy </param>
		public Manifest(Manifest man)
		{
//JAVA TO C# CONVERTER TODO TASK: There is no .NET Dictionary equivalent to the Java 'putAll' method:
			Attr.PutAll(man.MainAttributes);
//JAVA TO C# CONVERTER TODO TASK: There is no .NET Dictionary equivalent to the Java 'putAll' method:
			Entries_Renamed.putAll(man.Entries);
		}

		/// <summary>
		/// Returns the main Attributes for the Manifest. </summary>
		/// <returns> the main Attributes for the Manifest </returns>
		public virtual Attributes MainAttributes
		{
			get
			{
				return Attr;
			}
		}

		/// <summary>
		/// Returns a Map of the entries contained in this Manifest. Each entry
		/// is represented by a String name (key) and associated Attributes (value).
		/// The Map permits the {@code null} key, but no entry with a null key is
		/// created by <seealso cref="#read"/>, nor is such an entry written by using {@link
		/// #write}.
		/// </summary>
		/// <returns> a Map of the entries contained in this Manifest </returns>
		public virtual IDictionary<String, Attributes> Entries
		{
			get
			{
				return Entries_Renamed;
			}
		}

		/// <summary>
		/// Returns the Attributes for the specified entry name.
		/// This method is defined as:
		/// <pre>
		///      return (Attributes)getEntries().get(name)
		/// </pre>
		/// Though {@code null} is a valid {@code name}, when
		/// {@code getAttributes(null)} is invoked on a {@code Manifest}
		/// obtained from a jar file, {@code null} will be returned.  While jar
		/// files themselves do not allow {@code null}-named attributes, it is
		/// possible to invoke <seealso cref="#getEntries"/> on a {@code Manifest}, and
		/// on that result, invoke {@code put} with a null key and an
		/// arbitrary value.  Subsequent invocations of
		/// {@code getAttributes(null)} will return the just-{@code put}
		/// value.
		/// <para>
		/// Note that this method does not return the manifest's main attributes;
		/// see <seealso cref="#getMainAttributes"/>.
		/// 
		/// </para>
		/// </summary>
		/// <param name="name"> entry name </param>
		/// <returns> the Attributes for the specified entry name </returns>
		public virtual Attributes GetAttributes(String name)
		{
			return Entries[name];
		}

		/// <summary>
		/// Clears the main Attributes as well as the entries in this Manifest.
		/// </summary>
		public virtual void Clear()
		{
			Attr.Clear();
			Entries_Renamed.Clear();
		}

		/// <summary>
		/// Writes the Manifest to the specified OutputStream.
		/// Attributes.Name.MANIFEST_VERSION must be set in
		/// MainAttributes prior to invoking this method.
		/// </summary>
		/// <param name="out"> the output stream </param>
		/// <exception cref="IOException"> if an I/O error has occurred </exception>
		/// <seealso cref= #getMainAttributes </seealso>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public void write(java.io.OutputStream out) throws java.io.IOException
		public virtual void Write(OutputStream @out)
		{
			DataOutputStream dos = new DataOutputStream(@out);
			// Write out the main attributes for the manifest
			Attr.WriteMain(dos);
			// Now write out the pre-entry attributes
			IEnumerator<java.util.Map_Entry<String, Attributes>> it = Entries_Renamed.GetEnumerator();
			while (it.MoveNext())
			{
				java.util.Map_Entry<String, Attributes> e = it.Current;
				StringBuffer buffer = new StringBuffer("Name: ");
				String value = e.Key;
				if (value != null)
				{
					sbyte[] vb = value.GetBytes("UTF8");
					value = StringHelperClass.NewString(vb, 0, 0, vb.Length);
				}
				buffer.Append(value);
				buffer.Append("\r\n");
				Make72Safe(buffer);
				dos.WriteBytes(buffer.ToString());
				e.Value.Write(dos);
			}
			dos.Flush();
		}

		/// <summary>
		/// Adds line breaks to enforce a maximum 72 bytes per line.
		/// </summary>
		internal static void Make72Safe(StringBuffer line)
		{
			int length = line.Length();
			if (length > 72)
			{
				int index = 70;
				while (index < length - 2)
				{
					line.Insert(index, "\r\n ");
					index += 72;
					length += 3;
				}
			}
			return;
		}

		/// <summary>
		/// Reads the Manifest from the specified InputStream. The entry
		/// names and attributes read will be merged in with the current
		/// manifest entries.
		/// </summary>
		/// <param name="is"> the input stream </param>
		/// <exception cref="IOException"> if an I/O error has occurred </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public void read(java.io.InputStream is) throws java.io.IOException
		public virtual void Read(InputStream @is)
		{
			// Buffered input stream for reading manifest data
			FastInputStream fis = new FastInputStream(@is);
			// Line buffer
			sbyte[] lbuf = new sbyte[512];
			// Read the main attributes for the manifest
			Attr.Read(fis, lbuf);
			// Total number of entries, attributes read
			int ecount = 0, acount = 0;
			// Average size of entry attributes
			int asize = 2;
			// Now parse the manifest entries
			int len;
			String name = null;
			bool skipEmptyLines = true;
			sbyte[] lastline = null;

			while ((len = fis.ReadLine(lbuf)) != -1)
			{
				if (lbuf[--len] != '\n')
				{
					throw new IOException("manifest line too long");
				}
				if (len > 0 && lbuf[len - 1] == '\r')
				{
					--len;
				}
				if (len == 0 && skipEmptyLines)
				{
					continue;
				}
				skipEmptyLines = false;

				if (name == null)
				{
					name = ParseName(lbuf, len);
					if (name == null)
					{
						throw new IOException("invalid manifest format");
					}
					if (fis.Peek() == ' ')
					{
						// name is wrapped
						lastline = new sbyte[len - 6];
						System.Array.Copy(lbuf, 6, lastline, 0, len - 6);
						continue;
					}
				}
				else
				{
					// continuation line
					sbyte[] buf = new sbyte[lastline.Length + len - 1];
					System.Array.Copy(lastline, 0, buf, 0, lastline.Length);
					System.Array.Copy(lbuf, 1, buf, lastline.Length, len - 1);
					if (fis.Peek() == ' ')
					{
						// name is wrapped
						lastline = buf;
						continue;
					}
					name = StringHelperClass.NewString(buf, 0, buf.Length, "UTF8");
					lastline = null;
				}
				Attributes attr = GetAttributes(name);
				if (attr == null)
				{
					attr = new Attributes(asize);
					Entries_Renamed[name] = attr;
				}
				attr.Read(fis, lbuf);
				ecount++;
				acount += attr.Count;
				//XXX: Fix for when the average is 0. When it is 0,
				// you get an Attributes object with an initial
				// capacity of 0, which tickles a bug in HashMap.
				asize = System.Math.Max(2, acount / ecount);

				name = null;
				skipEmptyLines = true;
			}
		}

		private String ParseName(sbyte[] lbuf, int len)
		{
			if (ToLower(lbuf[0]) == 'n' && ToLower(lbuf[1]) == 'a' && ToLower(lbuf[2]) == 'm' && ToLower(lbuf[3]) == 'e' && lbuf[4] == ':' && lbuf[5] == ' ')
			{
				try
				{
					return StringHelperClass.NewString(lbuf, 6, len - 6, "UTF8");
				}
				catch (Exception)
				{
				}
			}
			return null;
		}

		private int ToLower(int c)
		{
			return (c >= 'A' && c <= 'Z') ? 'a' + (c - 'A') : c;
		}

		/// <summary>
		/// Returns true if the specified Object is also a Manifest and has
		/// the same main Attributes and entries.
		/// </summary>
		/// <param name="o"> the object to be compared </param>
		/// <returns> true if the specified Object is also a Manifest and has
		/// the same main Attributes and entries </returns>
		public override bool Equals(Object o)
		{
			if (o is Manifest)
			{
				Manifest m = (Manifest)o;
				return Attr.Equals(m.MainAttributes) && Entries_Renamed.Equals(m.Entries);
			}
			else
			{
				return false;
			}
		}

		/// <summary>
		/// Returns the hash code for this Manifest.
		/// </summary>
		public override int HashCode()
		{
			return Attr.HashCode() + Entries_Renamed.HashCode();
		}

		/// <summary>
		/// Returns a shallow copy of this Manifest.  The shallow copy is
		/// implemented as follows:
		/// <pre>
		///     public Object clone() { return new Manifest(this); }
		/// </pre> </summary>
		/// <returns> a shallow copy of this Manifest </returns>
		public virtual Object Clone()
		{
			return new Manifest(this);
		}

		/*
		 * A fast buffered input stream for parsing manifest files.
		 */
		internal class FastInputStream : FilterInputStream
		{
			internal sbyte[] Buf;
			internal int Count = 0;
			internal int Pos = 0;

			internal FastInputStream(InputStream @in) : this(@in, 8192)
			{
			}

			internal FastInputStream(InputStream @in, int size) : base(@in)
			{
				Buf = new sbyte[size];
			}

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public int read() throws java.io.IOException
			public override int Read()
			{
				if (Pos >= Count)
				{
					Fill();
					if (Pos >= Count)
					{
						return -1;
					}
				}
				return Byte.ToUnsignedInt(Buf[Pos++]);
			}

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public int read(byte[] b, int off, int len) throws java.io.IOException
			public override int Read(sbyte[] b, int off, int len)
			{
				int avail = Count - Pos;
				if (avail <= 0)
				{
					if (len >= Buf.Length)
					{
						return @in.Read(b, off, len);
					}
					Fill();
					avail = Count - Pos;
					if (avail <= 0)
					{
						return -1;
					}
				}
				if (len > avail)
				{
					len = avail;
				}
				System.Array.Copy(Buf, Pos, b, off, len);
				Pos += len;
				return len;
			}

			/*
			 * Reads 'len' bytes from the input stream, or until an end-of-line
			 * is reached. Returns the number of bytes read.
			 */
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public int readLine(byte[] b, int off, int len) throws java.io.IOException
			public virtual int ReadLine(sbyte[] b, int off, int len)
			{
				sbyte[] tbuf = this.Buf;
				int total = 0;
				while (total < len)
				{
					int avail = Count - Pos;
					if (avail <= 0)
					{
						Fill();
						avail = Count - Pos;
						if (avail <= 0)
						{
							return -1;
						}
					}
					int n = len - total;
					if (n > avail)
					{
						n = avail;
					}
					int tpos = Pos;
					int maxpos = tpos + n;
					while (tpos < maxpos && tbuf[tpos++] != '\n');
					n = tpos - Pos;
					System.Array.Copy(tbuf, Pos, b, off, n);
					off += n;
					total += n;
					Pos = tpos;
					if (tbuf[tpos - 1] == '\n')
					{
						break;
					}
				}
				return total;
			}

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public byte peek() throws java.io.IOException
			public virtual sbyte Peek()
			{
				if (Pos == Count)
				{
					Fill();
				}
				if (Pos == Count)
				{
					return -1; // nothing left in buffer
				}
				return Buf[Pos];
			}

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public int readLine(byte[] b) throws java.io.IOException
			public virtual int ReadLine(sbyte[] b)
			{
				return ReadLine(b, 0, b.Length);
			}

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public long skip(long n) throws java.io.IOException
			public override long Skip(long n)
			{
				if (n <= 0)
				{
					return 0;
				}
				long avail = Count - Pos;
				if (avail <= 0)
				{
					return @in.Skip(n);
				}
				if (n > avail)
				{
					n = avail;
				}
				Pos += (int)n;
				return n;
			}

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public int available() throws java.io.IOException
			public override int Available()
			{
				return (Count - Pos) + @in.Available();
			}

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public void close() throws java.io.IOException
			public override void Close()
			{
				if (@in != null)
				{
					@in.Close();
					@in = null;
					Buf = null;
				}
			}

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: private void fill() throws java.io.IOException
			internal virtual void Fill()
			{
				Count = Pos = 0;
				int n = @in.Read(Buf, 0, Buf.Length);
				if (n > 0)
				{
					Count = n;
				}
			}
		}
	}

}
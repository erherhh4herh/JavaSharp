using System;

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

namespace java.rmi
{

	using MarshalInputStream = sun.rmi.server.MarshalInputStream;
	using MarshalOutputStream = sun.rmi.server.MarshalOutputStream;

	/// <summary>
	/// A <code>MarshalledObject</code> contains a byte stream with the serialized
	/// representation of an object given to its constructor.  The <code>get</code>
	/// method returns a new copy of the original object, as deserialized from
	/// the contained byte stream.  The contained object is serialized and
	/// deserialized with the same serialization semantics used for marshaling
	/// and unmarshaling parameters and return values of RMI calls:  When the
	/// serialized form is created:
	/// 
	/// <ul>
	/// <li> classes are annotated with a codebase URL from where the class
	///      can be loaded (if available), and
	/// <li> any remote object in the <code>MarshalledObject</code> is
	///      represented by a serialized instance of its stub.
	/// </ul>
	/// 
	/// <para>When copy of the object is retrieved (via the <code>get</code> method),
	/// if the class is not available locally, it will be loaded from the
	/// appropriate location (specified the URL annotated with the class descriptor
	/// when the class was serialized.
	/// 
	/// </para>
	/// <para><code>MarshalledObject</code> facilitates passing objects in RMI calls
	/// that are not automatically deserialized immediately by the remote peer.
	/// 
	/// </para>
	/// </summary>
	/// @param <T> the type of the object contained in this
	/// <code>MarshalledObject</code>
	/// 
	/// @author  Ann Wollrath
	/// @author  Peter Jones
	/// @since   1.2 </param>
	[Serializable]
	public sealed class MarshalledObject<T>
	{
		/// <summary>
		/// @serial Bytes of serialized representation.  If <code>objBytes</code> is
		/// <code>null</code> then the object marshalled was a <code>null</code>
		/// reference.
		/// </summary>
		private sbyte[] ObjBytes = null;

		/// <summary>
		/// @serial Bytes of location annotations, which are ignored by
		/// <code>equals</code>.  If <code>locBytes</code> is null, there were no
		/// non-<code>null</code> annotations during marshalling.
		/// </summary>
		private sbyte[] LocBytes = null;

		/// <summary>
		/// @serial Stored hash code of contained object.
		/// </summary>
		/// <seealso cref= #hashCode </seealso>
		private int Hash;

		/// <summary>
		/// Indicate compatibility with 1.2 version of class. </summary>
		private const long SerialVersionUID = 8988374069173025854L;

		/// <summary>
		/// Creates a new <code>MarshalledObject</code> that contains the
		/// serialized representation of the current state of the supplied object.
		/// The object is serialized with the semantics used for marshaling
		/// parameters for RMI calls.
		/// </summary>
		/// <param name="obj"> the object to be serialized (must be serializable) </param>
		/// <exception cref="IOException"> if an <code>IOException</code> occurs; an
		/// <code>IOException</code> may occur if <code>obj</code> is not
		/// serializable.
		/// @since 1.2 </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public MarshalledObject(T obj) throws java.io.IOException
		public MarshalledObject(T obj)
		{
			if (obj == null)
			{
				Hash = 13;
				return;
			}

			ByteArrayOutputStream bout = new ByteArrayOutputStream();
			ByteArrayOutputStream lout = new ByteArrayOutputStream();
			MarshalledObjectOutputStream @out = new MarshalledObjectOutputStream(bout, lout);
			@out.WriteObject(obj);
			@out.Flush();
			ObjBytes = bout.ToByteArray();
			// locBytes is null if no annotations
			LocBytes = (@out.HadAnnotations() ? lout.ToByteArray() : null);

			/*
			 * Calculate hash from the marshalled representation of object
			 * so the hashcode will be comparable when sent between VMs.
			 */
			int h = 0;
			for (int i = 0; i < ObjBytes.Length; i++)
			{
				h = 31 * h + ObjBytes[i];
			}
			Hash = h;
		}

		/// <summary>
		/// Returns a new copy of the contained marshalledobject.  The internal
		/// representation is deserialized with the semantics used for
		/// unmarshaling parameters for RMI calls.
		/// </summary>
		/// <returns> a copy of the contained object </returns>
		/// <exception cref="IOException"> if an <code>IOException</code> occurs while
		/// deserializing the object from its internal representation. </exception>
		/// <exception cref="ClassNotFoundException"> if a
		/// <code>ClassNotFoundException</code> occurs while deserializing the
		/// object from its internal representation.
		/// could not be found
		/// @since 1.2 </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public T get() throws java.io.IOException, ClassNotFoundException
		public T Get()
		{
			if (ObjBytes == null) // must have been a null object
			{
				return null;
			}

			ByteArrayInputStream bin = new ByteArrayInputStream(ObjBytes);
			// locBytes is null if no annotations
			ByteArrayInputStream lin = (LocBytes == null ? null : new ByteArrayInputStream(LocBytes));
			MarshalledObjectInputStream @in = new MarshalledObjectInputStream(bin, lin);
//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @SuppressWarnings("unchecked") T obj = (T) in.readObject();
			T obj = (T) @in.readObject();
			@in.close();
			return obj;
		}

		/// <summary>
		/// Return a hash code for this <code>MarshalledObject</code>.
		/// </summary>
		/// <returns> a hash code </returns>
		public override int HashCode()
		{
			return Hash;
		}

		/// <summary>
		/// Compares this <code>MarshalledObject</code> to another object.
		/// Returns true if and only if the argument refers to a
		/// <code>MarshalledObject</code> that contains exactly the same
		/// serialized representation of an object as this one does. The
		/// comparison ignores any class codebase annotation, meaning that
		/// two objects are equivalent if they have the same serialized
		/// representation <i>except</i> for the codebase of each class
		/// in the serialized representation.
		/// </summary>
		/// <param name="obj"> the object to compare with this <code>MarshalledObject</code> </param>
		/// <returns> <code>true</code> if the argument contains an equivalent
		/// serialized object; <code>false</code> otherwise
		/// @since 1.2 </returns>
		public override bool Equals(Object obj)
		{
			if (obj == this)
			{
				return true;
			}

			if (obj != null && obj is MarshalledObject)
			{
//JAVA TO C# CONVERTER TODO TASK: Java wildcard generics are not converted to .NET:
//ORIGINAL LINE: MarshalledObject<?> other = (MarshalledObject<?>) obj;
				MarshalledObject<?> other = (MarshalledObject<?>) obj;

				// if either is a ref to null, both must be
				if (ObjBytes == null || other.ObjBytes == null)
				{
					return ObjBytes == other.ObjBytes;
				}

				// quick, easy test
				if (ObjBytes.Length != other.ObjBytes.Length)
				{
					return false;
				}

				//!! There is talk about adding an array comparision method
				//!! at 1.2 -- if so, this should be rewritten.  -arnold
				for (int i = 0; i < ObjBytes.Length; ++i)
				{
					if (ObjBytes[i] != other.ObjBytes[i])
					{
						return false;
					}
				}
				return true;
			}
			else
			{
				return false;
			}
		}

		/// <summary>
		/// This class is used to marshal objects for
		/// <code>MarshalledObject</code>.  It places the location annotations
		/// to one side so that two <code>MarshalledObject</code>s can be
		/// compared for equality if they differ only in location
		/// annotations.  Objects written using this stream should be read back
		/// from a <code>MarshalledObjectInputStream</code>.
		/// </summary>
		/// <seealso cref= java.rmi.MarshalledObject </seealso>
		/// <seealso cref= MarshalledObjectInputStream </seealso>
		private class MarshalledObjectOutputStream : MarshalOutputStream
		{
			/// <summary>
			/// The stream on which location objects are written. </summary>
			internal ObjectOutputStream LocOut;

			/// <summary>
			/// <code>true</code> if non-<code>null</code> annotations are
			///  written.
			/// </summary>
			internal bool HadAnnotations_Renamed;

			/// <summary>
			/// Creates a new <code>MarshalledObjectOutputStream</code> whose
			/// non-location bytes will be written to <code>objOut</code> and whose
			/// location annotations (if any) will be written to
			/// <code>locOut</code>.
			/// </summary>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: MarshalledObjectOutputStream(java.io.OutputStream objOut, java.io.OutputStream locOut) throws java.io.IOException
			internal MarshalledObjectOutputStream(OutputStream objOut, OutputStream locOut) : base(objOut)
			{
				this.useProtocolVersion(java.io.ObjectStreamConstants_Fields.PROTOCOL_VERSION_2);
				this.LocOut = new ObjectOutputStream(locOut);
				HadAnnotations_Renamed = false;
			}

			/// <summary>
			/// Returns <code>true</code> if any non-<code>null</code> location
			/// annotations have been written to this stream.
			/// </summary>
			internal virtual bool HadAnnotations()
			{
				return HadAnnotations_Renamed;
			}

			/// <summary>
			/// Overrides MarshalOutputStream.writeLocation implementation to write
			/// annotations to the location stream.
			/// </summary>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: protected void writeLocation(String loc) throws java.io.IOException
			protected internal virtual void WriteLocation(String loc)
			{
				HadAnnotations_Renamed |= (loc != null);
				LocOut.WriteObject(loc);
			}


//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public void flush() throws java.io.IOException
			public virtual void Flush()
			{
				base.Flush();
				LocOut.Flush();
			}
		}

		/// <summary>
		/// The counterpart to <code>MarshalledObjectOutputStream</code>.
		/// </summary>
		/// <seealso cref= MarshalledObjectOutputStream </seealso>
		private class MarshalledObjectInputStream : MarshalInputStream
		{
			/// <summary>
			/// The stream from which annotations will be read.  If this is
			/// <code>null</code>, then all annotations were <code>null</code>.
			/// </summary>
			internal ObjectInputStream LocIn;

			/// <summary>
			/// Creates a new <code>MarshalledObjectInputStream</code> that
			/// reads its objects from <code>objIn</code> and annotations
			/// from <code>locIn</code>.  If <code>locIn</code> is
			/// <code>null</code>, then all annotations will be
			/// <code>null</code>.
			/// </summary>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: MarshalledObjectInputStream(java.io.InputStream objIn, java.io.InputStream locIn) throws java.io.IOException
			internal MarshalledObjectInputStream(InputStream objIn, InputStream locIn) : base(objIn)
			{
				this.LocIn = (locIn == null ? null : new ObjectInputStream(locIn));
			}

			/// <summary>
			/// Overrides MarshalInputStream.readLocation to return locations from
			/// the stream we were given, or <code>null</code> if we were given a
			/// <code>null</code> location stream.
			/// </summary>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: protected Object readLocation() throws java.io.IOException, ClassNotFoundException
			protected internal virtual Object ReadLocation()
			{
				return (LocIn == null ? null : LocIn.ReadObject());
			}
		}

	}

}
using System;

/*
 * Copyright (c) 1996, 2006, Oracle and/or its affiliates. All rights reserved.
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
namespace java.rmi.server
{

	using GetPropertyAction = sun.security.action.GetPropertyAction;

	/// <summary>
	/// An <code>ObjID</code> is used to identify a remote object exported
	/// to an RMI runtime.  When a remote object is exported, it is assigned
	/// an object identifier either implicitly or explicitly, depending on
	/// the API used to export.
	/// 
	/// <para>The <seealso cref="#ObjID()"/> constructor can be used to generate a unique
	/// object identifier.  Such an <code>ObjID</code> is unique over time
	/// with respect to the host it is generated on.
	/// 
	/// The <seealso cref="#ObjID(int)"/> constructor can be used to create a
	/// "well-known" object identifier.  The scope of a well-known
	/// <code>ObjID</code> depends on the RMI runtime it is exported to.
	/// 
	/// </para>
	/// <para>An <code>ObjID</code> instance contains an object number (of type
	/// <code>long</code>) and an address space identifier (of type
	/// <seealso cref="UID"/>).  In a unique <code>ObjID</code>, the address space
	/// identifier is unique with respect to a given host over time.  In a
	/// well-known <code>ObjID</code>, the address space identifier is
	/// equivalent to one returned by invoking the <seealso cref="UID#UID(short)"/>
	/// constructor with the value zero.
	/// 
	/// </para>
	/// <para>If the system property <code>java.rmi.server.randomIDs</code>
	/// is defined to equal the string <code>"true"</code> (case insensitive),
	/// then the <seealso cref="#ObjID()"/> constructor will use a cryptographically
	/// strong random number generator to choose the object number of the
	/// returned <code>ObjID</code>.
	/// 
	/// @author      Ann Wollrath
	/// @author      Peter Jones
	/// @since       JDK1.1
	/// </para>
	/// </summary>
	[Serializable]
	public sealed class ObjID
	{

		/// <summary>
		/// Object number for well-known <code>ObjID</code> of the registry. </summary>
		public const int REGISTRY_ID = 0;

		/// <summary>
		/// Object number for well-known <code>ObjID</code> of the activator. </summary>
		public const int ACTIVATOR_ID = 1;

		/// <summary>
		/// Object number for well-known <code>ObjID</code> of
		/// the distributed garbage collector.
		/// </summary>
		public const int DGC_ID = 2;

		/// <summary>
		/// indicate compatibility with JDK 1.1.x version of class </summary>
		private const long SerialVersionUID = -6386392263968365220L;

		private static readonly AtomicLong NextObjNum = new AtomicLong(0);
		private static readonly UID MySpace = new UID();
		private static readonly SecureRandom SecureRandom = new SecureRandom();

		/// <summary>
		/// @serial object number </summary>
		/// <seealso cref= #hashCode </seealso>
		private readonly long ObjNum;

		/// <summary>
		/// @serial address space identifier (unique to host over time)
		/// </summary>
		private readonly UID Space;

		/// <summary>
		/// Generates a unique object identifier.
		/// 
		/// <para>If the system property <code>java.rmi.server.randomIDs</code>
		/// is defined to equal the string <code>"true"</code> (case insensitive),
		/// then this constructor will use a cryptographically
		/// strong random number generator to choose the object number of the
		/// returned <code>ObjID</code>.
		/// </para>
		/// </summary>
		public ObjID()
		{
			/*
			 * If generating random object numbers, create a new UID to
			 * ensure uniqueness; otherwise, use a shared UID because
			 * sequential object numbers already ensure uniqueness.
			 */
			if (UseRandomIDs())
			{
				Space = new UID();
				ObjNum = SecureRandom.NextLong();
			}
			else
			{
				Space = MySpace;
				ObjNum = NextObjNum.AndIncrement;
			}
		}

		/// <summary>
		/// Creates a "well-known" object identifier.
		/// 
		/// <para>An <code>ObjID</code> created via this constructor will not
		/// clash with any <code>ObjID</code>s generated via the no-arg
		/// constructor.
		/// 
		/// </para>
		/// </summary>
		/// <param name="objNum"> object number for well-known object identifier </param>
		public ObjID(int objNum)
		{
			Space = new UID((short) 0);
			this.ObjNum = objNum;
		}

		/// <summary>
		/// Constructs an object identifier given data read from a stream.
		/// </summary>
		private ObjID(long objNum, UID space)
		{
			this.ObjNum = objNum;
			this.Space = space;
		}

		/// <summary>
		/// Marshals a binary representation of this <code>ObjID</code> to
		/// an <code>ObjectOutput</code> instance.
		/// 
		/// <para>Specifically, this method first invokes the given stream's
		/// <seealso cref="ObjectOutput#writeLong(long)"/> method with this object
		/// identifier's object number, and then it writes its address
		/// space identifier by invoking its <seealso cref="UID#write(DataOutput)"/>
		/// method with the stream.
		/// 
		/// </para>
		/// </summary>
		/// <param name="out"> the <code>ObjectOutput</code> instance to write
		/// this <code>ObjID</code> to
		/// </param>
		/// <exception cref="IOException"> if an I/O error occurs while performing
		/// this operation </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public void write(java.io.ObjectOutput out) throws java.io.IOException
		public void Write(ObjectOutput @out)
		{
			@out.WriteLong(ObjNum);
			Space.Write(@out);
		}

		/// <summary>
		/// Constructs and returns a new <code>ObjID</code> instance by
		/// unmarshalling a binary representation from an
		/// <code>ObjectInput</code> instance.
		/// 
		/// <para>Specifically, this method first invokes the given stream's
		/// <seealso cref="ObjectInput#readLong()"/> method to read an object number,
		/// then it invokes <seealso cref="UID#read(DataInput)"/> with the
		/// stream to read an address space identifier, and then it
		/// creates and returns a new <code>ObjID</code> instance that
		/// contains the object number and address space identifier that
		/// were read from the stream.
		/// 
		/// </para>
		/// </summary>
		/// <param name="in"> the <code>ObjectInput</code> instance to read
		/// <code>ObjID</code> from
		/// </param>
		/// <returns>  unmarshalled <code>ObjID</code> instance
		/// </returns>
		/// <exception cref="IOException"> if an I/O error occurs while performing
		/// this operation </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public static ObjID read(java.io.ObjectInput in) throws java.io.IOException
		public static ObjID Read(ObjectInput @in)
		{
			long num = @in.ReadLong();
			UID space = UID.Read(@in);
			return new ObjID(num, space);
		}

		/// <summary>
		/// Returns the hash code value for this object identifier, the
		/// object number.
		/// </summary>
		/// <returns>  the hash code value for this object identifier </returns>
		public override int HashCode()
		{
			return (int) ObjNum;
		}

		/// <summary>
		/// Compares the specified object with this <code>ObjID</code> for
		/// equality.
		/// 
		/// This method returns <code>true</code> if and only if the
		/// specified object is an <code>ObjID</code> instance with the same
		/// object number and address space identifier as this one.
		/// </summary>
		/// <param name="obj"> the object to compare this <code>ObjID</code> to
		/// </param>
		/// <returns>  <code>true</code> if the given object is equivalent to
		/// this one, and <code>false</code> otherwise </returns>
		public override bool Equals(Object obj)
		{
			if (obj is ObjID)
			{
				ObjID id = (ObjID) obj;
				return ObjNum == id.ObjNum && Space.Equals(id.Space);
			}
			else
			{
				return false;
			}
		}

		/// <summary>
		/// Returns a string representation of this object identifier.
		/// </summary>
		/// <returns>  a string representation of this object identifier </returns>
		/*
		 * The address space identifier is only included in the string
		 * representation if it does not denote the local address space
		 * (or if the randomIDs property was set).
		 */
		public override String ToString()
		{
			return "[" + (Space.Equals(MySpace) ? "" : Space + ", ") + ObjNum + "]";
		}

		private static bool UseRandomIDs()
		{
			String value = AccessController.doPrivileged(new GetPropertyAction("java.rmi.server.randomIDs"));
			return value == null ? true : Convert.ToBoolean(value);
		}
	}

}
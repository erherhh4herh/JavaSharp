using System;

/*
 * Copyright (c) 1996, 2011, Oracle and/or its affiliates. All rights reserved.
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

	using Util = sun.rmi.server.Util;

	/// <summary>
	/// The <code>RemoteObject</code> class implements the
	/// <code>java.lang.Object</code> behavior for remote objects.
	/// <code>RemoteObject</code> provides the remote semantics of Object by
	/// implementing methods for hashCode, equals, and toString.
	/// 
	/// @author      Ann Wollrath
	/// @author      Laird Dornin
	/// @author      Peter Jones
	/// @since       JDK1.1
	/// </summary>
	[Serializable]
	public abstract class RemoteObject : Remote
	{

		/// <summary>
		/// The object's remote reference. </summary>
		[NonSerialized]
		protected internal RemoteRef @ref;

		/// <summary>
		/// indicate compatibility with JDK 1.1.x version of class </summary>
		private const long SerialVersionUID = -3215090123894869218L;

		/// <summary>
		/// Creates a remote object.
		/// </summary>
		protected internal RemoteObject()
		{
			@ref = null;
		}

		/// <summary>
		/// Creates a remote object, initialized with the specified remote
		/// reference. </summary>
		/// <param name="newref"> remote reference </param>
		protected internal RemoteObject(RemoteRef newref)
		{
			@ref = newref;
		}

		/// <summary>
		/// Returns the remote reference for the remote object.
		/// 
		/// <para>Note: The object returned from this method may be an instance of
		/// an implementation-specific class.  The <code>RemoteObject</code>
		/// class ensures serialization portability of its instances' remote
		/// references through the behavior of its custom
		/// <code>writeObject</code> and <code>readObject</code> methods.  An
		/// instance of <code>RemoteRef</code> should not be serialized outside
		/// of its <code>RemoteObject</code> wrapper instance or the result may
		/// be unportable.
		/// 
		/// </para>
		/// </summary>
		/// <returns> remote reference for the remote object
		/// @since 1.2 </returns>
		public virtual RemoteRef Ref
		{
			get
			{
				return @ref;
			}
		}

		/// <summary>
		/// Returns the stub for the remote object <code>obj</code> passed
		/// as a parameter. This operation is only valid <i>after</i>
		/// the object has been exported. </summary>
		/// <param name="obj"> the remote object whose stub is needed </param>
		/// <returns> the stub for the remote object, <code>obj</code>. </returns>
		/// <exception cref="NoSuchObjectException"> if the stub for the
		/// remote object could not be found.
		/// @since 1.2 </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public static java.rmi.Remote toStub(java.rmi.Remote obj) throws java.rmi.NoSuchObjectException
		public static Remote ToStub(Remote obj)
		{
			if (obj is RemoteStub || (obj != null && Proxy.isProxyClass(obj.GetType()) && Proxy.getInvocationHandler(obj) is RemoteObjectInvocationHandler))
			{
				return obj;
			}
			else
			{
				return sun.rmi.transport.ObjectTable.getStub(obj);
			}
		}

		/// <summary>
		/// Returns a hashcode for a remote object.  Two remote object stubs
		/// that refer to the same remote object will have the same hash code
		/// (in order to support remote objects as keys in hash tables).
		/// </summary>
		/// <seealso cref=             java.util.Hashtable </seealso>
		public override int HashCode()
		{
			return (@ref == null) ? base.HashCode() : @ref.RemoteHashCode();
		}

		/// <summary>
		/// Compares two remote objects for equality.
		/// Returns a boolean that indicates whether this remote object is
		/// equivalent to the specified Object. This method is used when a
		/// remote object is stored in a hashtable.
		/// If the specified Object is not itself an instance of RemoteObject,
		/// then this method delegates by returning the result of invoking the
		/// <code>equals</code> method of its parameter with this remote object
		/// as the argument. </summary>
		/// <param name="obj">     the Object to compare with </param>
		/// <returns>  true if these Objects are equal; false otherwise. </returns>
		/// <seealso cref=             java.util.Hashtable </seealso>
		public override bool Equals(Object obj)
		{
			if (obj is RemoteObject)
			{
				if (@ref == null)
				{
					return obj == this;
				}
				else
				{
					return @ref.RemoteEquals(((RemoteObject)obj).@ref);
				}
			}
			else if (obj != null)
			{
				/*
				 * Fix for 4099660: if object is not an instance of RemoteObject,
				 * use the result of its equals method, to support symmetry is a
				 * remote object implementation class that does not extend
				 * RemoteObject wishes to support equality with its stub objects.
				 */
				return obj.Equals(this);
			}
			else
			{
				return false;
			}
		}

		/// <summary>
		/// Returns a String that represents the value of this remote object.
		/// </summary>
		public override String ToString()
		{
			String classname = Util.getUnqualifiedName(this.GetType());
			return (@ref == null) ? classname : classname + "[" + @ref.RemoteToString() + "]";
		}

		/// <summary>
		/// <code>writeObject</code> for custom serialization.
		/// 
		/// <para>This method writes this object's serialized form for this class
		/// as follows:
		/// 
		/// </para>
		/// <para>The <seealso cref="RemoteRef#getRefClass(java.io.ObjectOutput) getRefClass"/>
		/// method is invoked on this object's <code>ref</code> field
		/// to obtain its external ref type name.
		/// If the value returned by <code>getRefClass</code> was
		/// a non-<code>null</code> string of length greater than zero,
		/// the <code>writeUTF</code> method is invoked on <code>out</code>
		/// with the value returned by <code>getRefClass</code>, and then
		/// the <code>writeExternal</code> method is invoked on
		/// this object's <code>ref</code> field passing <code>out</code>
		/// as the argument; otherwise,
		/// the <code>writeUTF</code> method is invoked on <code>out</code>
		/// with a zero-length string (<code>""</code>), and then
		/// the <code>writeObject</code> method is invoked on <code>out</code>
		/// passing this object's <code>ref</code> field as the argument.
		/// 
		/// @serialData
		/// 
		/// The serialized data for this class comprises a string (written with
		/// <code>ObjectOutput.writeUTF</code>) that is either the external
		/// ref type name of the contained <code>RemoteRef</code> instance
		/// (the <code>ref</code> field) or a zero-length string, followed by
		/// either the external form of the <code>ref</code> field as written by
		/// its <code>writeExternal</code> method if the string was of non-zero
		/// length, or the serialized form of the <code>ref</code> field as
		/// written by passing it to the serialization stream's
		/// <code>writeObject</code> if the string was of zero length.
		/// 
		/// </para>
		/// <para>If this object is an instance of
		/// <seealso cref="RemoteStub"/> or <seealso cref="RemoteObjectInvocationHandler"/>
		/// that was returned from any of
		/// the <code>UnicastRemoteObject.exportObject</code> methods
		/// and custom socket factories are not used,
		/// the external ref type name is <code>"UnicastRef"</code>.
		/// 
		/// If this object is an instance of
		/// <code>RemoteStub</code> or <code>RemoteObjectInvocationHandler</code>
		/// that was returned from any of
		/// the <code>UnicastRemoteObject.exportObject</code> methods
		/// and custom socket factories are used,
		/// the external ref type name is <code>"UnicastRef2"</code>.
		/// 
		/// If this object is an instance of
		/// <code>RemoteStub</code> or <code>RemoteObjectInvocationHandler</code>
		/// that was returned from any of
		/// the <code>java.rmi.activation.Activatable.exportObject</code> methods,
		/// the external ref type name is <code>"ActivatableRef"</code>.
		/// 
		/// If this object is an instance of
		/// <code>RemoteStub</code> or <code>RemoteObjectInvocationHandler</code>
		/// that was returned from
		/// the <code>RemoteObject.toStub</code> method (and the argument passed
		/// to <code>toStub</code> was not itself a <code>RemoteStub</code>),
		/// the external ref type name is a function of how the remote object
		/// passed to <code>toStub</code> was exported, as described above.
		/// 
		/// If this object is an instance of
		/// <code>RemoteStub</code> or <code>RemoteObjectInvocationHandler</code>
		/// that was originally created via deserialization,
		/// the external ref type name is the same as that which was read
		/// when this object was deserialized.
		/// 
		/// </para>
		/// <para>If this object is an instance of
		/// <code>java.rmi.server.UnicastRemoteObject</code> that does not
		/// use custom socket factories,
		/// the external ref type name is <code>"UnicastServerRef"</code>.
		/// 
		/// If this object is an instance of
		/// <code>UnicastRemoteObject</code> that does
		/// use custom socket factories,
		/// the external ref type name is <code>"UnicastServerRef2"</code>.
		/// 
		/// </para>
		/// <para>Following is the data that must be written by the
		/// <code>writeExternal</code> method and read by the
		/// <code>readExternal</code> method of <code>RemoteRef</code>
		/// implementation classes that correspond to the each of the
		/// defined external ref type names:
		/// 
		/// </para>
		/// <para>For <code>"UnicastRef"</code>:
		/// 
		/// <ul>
		/// 
		/// <li>the hostname of the referenced remote object,
		/// written by <seealso cref="java.io.ObjectOutput#writeUTF(String)"/>
		/// 
		/// <li>the port of the referenced remote object,
		/// written by <seealso cref="java.io.ObjectOutput#writeInt(int)"/>
		/// 
		/// <li>the data written as a result of calling
		/// {link java.rmi.server.ObjID#write(java.io.ObjectOutput)}
		/// on the <code>ObjID</code> instance contained in the reference
		/// 
		/// <li>the boolean value <code>false</code>,
		/// written by <seealso cref="java.io.ObjectOutput#writeBoolean(boolean)"/>
		/// 
		/// </ul>
		/// 
		/// </para>
		/// <para>For <code>"UnicastRef2"</code> with a
		/// <code>null</code> client socket factory:
		/// 
		/// <ul>
		/// 
		/// <li>the byte value <code>0x00</code>
		/// (indicating <code>null</code> client socket factory),
		/// written by <seealso cref="java.io.ObjectOutput#writeByte(int)"/>
		/// 
		/// <li>the hostname of the referenced remote object,
		/// written by <seealso cref="java.io.ObjectOutput#writeUTF(String)"/>
		/// 
		/// <li>the port of the referenced remote object,
		/// written by <seealso cref="java.io.ObjectOutput#writeInt(int)"/>
		/// 
		/// <li>the data written as a result of calling
		/// {link java.rmi.server.ObjID#write(java.io.ObjectOutput)}
		/// on the <code>ObjID</code> instance contained in the reference
		/// 
		/// <li>the boolean value <code>false</code>,
		/// written by <seealso cref="java.io.ObjectOutput#writeBoolean(boolean)"/>
		/// 
		/// </ul>
		/// 
		/// </para>
		/// <para>For <code>"UnicastRef2"</code> with a
		/// non-<code>null</code> client socket factory:
		/// 
		/// <ul>
		/// 
		/// <li>the byte value <code>0x01</code>
		/// (indicating non-<code>null</code> client socket factory),
		/// written by <seealso cref="java.io.ObjectOutput#writeByte(int)"/>
		/// 
		/// <li>the hostname of the referenced remote object,
		/// written by <seealso cref="java.io.ObjectOutput#writeUTF(String)"/>
		/// 
		/// <li>the port of the referenced remote object,
		/// written by <seealso cref="java.io.ObjectOutput#writeInt(int)"/>
		/// 
		/// <li>a client socket factory (object of type
		/// <code>java.rmi.server.RMIClientSocketFactory</code>),
		/// written by passing it to an invocation of
		/// <code>writeObject</code> on the stream instance
		/// 
		/// <li>the data written as a result of calling
		/// {link java.rmi.server.ObjID#write(java.io.ObjectOutput)}
		/// on the <code>ObjID</code> instance contained in the reference
		/// 
		/// <li>the boolean value <code>false</code>,
		/// written by <seealso cref="java.io.ObjectOutput#writeBoolean(boolean)"/>
		/// 
		/// </ul>
		/// 
		/// </para>
		/// <para>For <code>"ActivatableRef"</code> with a
		/// <code>null</code> nested remote reference:
		/// 
		/// <ul>
		/// 
		/// <li>an instance of
		/// <code>java.rmi.activation.ActivationID</code>,
		/// written by passing it to an invocation of
		/// <code>writeObject</code> on the stream instance
		/// 
		/// <li>a zero-length string (<code>""</code>),
		/// written by <seealso cref="java.io.ObjectOutput#writeUTF(String)"/>
		/// 
		/// </ul>
		/// 
		/// </para>
		/// <para>For <code>"ActivatableRef"</code> with a
		/// non-<code>null</code> nested remote reference:
		/// 
		/// <ul>
		/// 
		/// <li>an instance of
		/// <code>java.rmi.activation.ActivationID</code>,
		/// written by passing it to an invocation of
		/// <code>writeObject</code> on the stream instance
		/// 
		/// <li>the external ref type name of the nested remote reference,
		/// which must be <code>"UnicastRef2"</code>,
		/// written by <seealso cref="java.io.ObjectOutput#writeUTF(String)"/>
		/// 
		/// <li>the external form of the nested remote reference,
		/// written by invoking its <code>writeExternal</code> method
		/// with the stream instance
		/// (see the description of the external form for
		/// <code>"UnicastRef2"</code> above)
		/// 
		/// </ul>
		/// 
		/// </para>
		/// <para>For <code>"UnicastServerRef"</code> and
		/// <code>"UnicastServerRef2"</code>, no data is written by the
		/// <code>writeExternal</code> method or read by the
		/// <code>readExternal</code> method.
		/// </para>
		/// </summary>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: private void writeObject(java.io.ObjectOutputStream out) throws java.io.IOException, java.lang.ClassNotFoundException
		private void WriteObject(java.io.ObjectOutputStream @out)
		{
			if (@ref == null)
			{
				throw new java.rmi.MarshalException("Invalid remote object");
			}
			else
			{
				String refClassName = @ref.GetRefClass(@out);
				if (refClassName == null || refClassName.Length() == 0)
				{
					/*
					 * No reference class name specified, so serialize
					 * remote reference.
					 */
					@out.WriteUTF("");
					@out.WriteObject(@ref);
				}
				else
				{
					/*
					 * Built-in reference class specified, so delegate
					 * to reference to write out its external form.
					 */
					@out.WriteUTF(refClassName);
					@ref.WriteExternal(@out);
				}
			}
		}

		/// <summary>
		/// <code>readObject</code> for custom serialization.
		/// 
		/// <para>This method reads this object's serialized form for this class
		/// as follows:
		/// 
		/// </para>
		/// <para>The <code>readUTF</code> method is invoked on <code>in</code>
		/// to read the external ref type name for the <code>RemoteRef</code>
		/// instance to be filled in to this object's <code>ref</code> field.
		/// If the string returned by <code>readUTF</code> has length zero,
		/// the <code>readObject</code> method is invoked on <code>in</code>,
		/// and than the value returned by <code>readObject</code> is cast to
		/// <code>RemoteRef</code> and this object's <code>ref</code> field is
		/// set to that value.
		/// Otherwise, this object's <code>ref</code> field is set to a
		/// <code>RemoteRef</code> instance that is created of an
		/// implementation-specific class corresponding to the external ref
		/// type name returned by <code>readUTF</code>, and then
		/// the <code>readExternal</code> method is invoked on
		/// this object's <code>ref</code> field.
		/// 
		/// </para>
		/// <para>If the external ref type name is
		/// <code>"UnicastRef"</code>, <code>"UnicastServerRef"</code>,
		/// <code>"UnicastRef2"</code>, <code>"UnicastServerRef2"</code>,
		/// or <code>"ActivatableRef"</code>, a corresponding
		/// implementation-specific class must be found, and its
		/// <code>readExternal</code> method must read the serial data
		/// for that external ref type name as specified to be written
		/// in the <b>serialData</b> documentation for this class.
		/// If the external ref type name is any other string (of non-zero
		/// length), a <code>ClassNotFoundException</code> will be thrown,
		/// unless the implementation provides an implementation-specific
		/// class corresponding to that external ref type name, in which
		/// case this object's <code>ref</code> field will be set to an
		/// instance of that implementation-specific class.
		/// </para>
		/// </summary>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: private void readObject(java.io.ObjectInputStream in) throws java.io.IOException, java.lang.ClassNotFoundException
		private void ReadObject(java.io.ObjectInputStream @in)
		{
			String refClassName = @in.ReadUTF();
			if (refClassName == null || refClassName.Length() == 0)
			{
				/*
				 * No reference class name specified, so construct
				 * remote reference from its serialized form.
				 */
				@ref = (RemoteRef) @in.ReadObject();
			}
			else
			{
				/*
				 * Built-in reference class specified, so delegate to
				 * internal reference class to initialize its fields from
				 * its external form.
				 */
				String internalRefClassName = RemoteRef_Fields.PackagePrefix + "." + refClassName;
				Class refClass = Class.ForName(internalRefClassName);
				try
				{
					@ref = (RemoteRef) refClass.NewInstance();

					/*
					 * If this step fails, assume we found an internal
					 * class that is not meant to be a serializable ref
					 * type.
					 */
				}
				catch (InstantiationException e)
				{
					throw new ClassNotFoundException(internalRefClassName, e);
				}
				catch (IllegalAccessException e)
				{
					throw new ClassNotFoundException(internalRefClassName, e);
				}
				catch (ClassCastException e)
				{
					throw new ClassNotFoundException(internalRefClassName, e);
				}
				@ref.ReadExternal(@in);
			}
		}
	}

}
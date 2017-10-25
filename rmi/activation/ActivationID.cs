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

namespace java.rmi.activation
{


	/// <summary>
	/// Activation makes use of special identifiers to denote remote
	/// objects that can be activated over time. An activation identifier
	/// (an instance of the class <code>ActivationID</code>) contains several
	/// pieces of information needed for activating an object:
	/// <ul>
	/// <li> a remote reference to the object's activator (a {@link
	/// java.rmi.server.RemoteRef RemoteRef}
	/// instance), and
	/// <li> a unique identifier (a <seealso cref="java.rmi.server.UID UID"/>
	/// instance) for the object. </ul> <para>
	/// 
	/// An activation identifier for an object can be obtained by registering
	/// an object with the activation system. Registration is accomplished
	/// in a few ways: <ul>
	/// <li>via the <code>Activatable.register</code> method
	/// <li>via the first <code>Activatable</code> constructor (that takes
	/// three arguments and both registers and exports the object, and
	/// <li>via the first <code>Activatable.exportObject</code> method
	/// that takes the activation descriptor, object and port as arguments;
	/// this method both registers and exports the object. </ul>
	/// 
	/// @author      Ann Wollrath
	/// </para>
	/// </summary>
	/// <seealso cref=         Activatable
	/// @since       1.2 </seealso>
	[Serializable]
	public class ActivationID
	{
		/// <summary>
		/// the object's activator
		/// </summary>
		[NonSerialized]
		private Activator Activator;

		/// <summary>
		/// the object's unique id
		/// </summary>
		[NonSerialized]
		private UID Uid = new UID();

		/// <summary>
		/// indicate compatibility with the Java 2 SDK v1.2 version of class </summary>
		private const long SerialVersionUID = -4608673054848209235L;

		/// <summary>
		/// The constructor for <code>ActivationID</code> takes a single
		/// argument, activator, that specifies a remote reference to the
		/// activator responsible for activating the object associated with
		/// this identifier. An instance of <code>ActivationID</code> is globally
		/// unique.
		/// </summary>
		/// <param name="activator"> reference to the activator responsible for
		/// activating the object </param>
		/// <exception cref="UnsupportedOperationException"> if and only if activation is
		///         not supported by this implementation
		/// @since 1.2 </exception>
		public ActivationID(Activator activator)
		{
			this.Activator = activator;
		}

		/// <summary>
		/// Activate the object for this id.
		/// </summary>
		/// <param name="force"> if true, forces the activator to contact the group
		/// when activating the object (instead of returning a cached reference);
		/// if false, returning a cached value is acceptable. </param>
		/// <returns> the reference to the active remote object </returns>
		/// <exception cref="ActivationException"> if activation fails </exception>
		/// <exception cref="UnknownObjectException"> if the object is unknown </exception>
		/// <exception cref="RemoteException"> if remote call fails
		/// @since 1.2 </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public java.rmi.Remote activate(boolean force) throws ActivationException, UnknownObjectException, java.rmi.RemoteException
		public virtual Remote Activate(bool force)
		{
			try
			{
//JAVA TO C# CONVERTER TODO TASK: Java wildcard generics are not converted to .NET:
//ORIGINAL LINE: java.rmi.MarshalledObject<? extends java.rmi.Remote> mobj = activator.activate(this, force);
				MarshalledObject<?> mobj = Activator.Activate(this, force);
				return mobj.Get();
			}
			catch (RemoteException e)
			{
				throw e;
			}
			catch (IOException e)
			{
				throw new UnmarshalException("activation failed", e);
			}
			catch (ClassNotFoundException e)
			{
				throw new UnmarshalException("activation failed", e);
			}

		}

		/// <summary>
		/// Returns a hashcode for the activation id.  Two identifiers that
		/// refer to the same remote object will have the same hash code.
		/// </summary>
		/// <seealso cref= java.util.Hashtable
		/// @since 1.2 </seealso>
		public override int HashCode()
		{
			return Uid.HashCode();
		}

		/// <summary>
		/// Compares two activation ids for content equality.
		/// Returns true if both of the following conditions are true:
		/// 1) the unique identifiers equivalent (by content), and
		/// 2) the activator specified in each identifier
		///    refers to the same remote object.
		/// </summary>
		/// <param name="obj">     the Object to compare with </param>
		/// <returns>  true if these Objects are equal; false otherwise. </returns>
		/// <seealso cref=             java.util.Hashtable
		/// @since 1.2 </seealso>
		public override bool Equals(Object obj)
		{
			if (obj is ActivationID)
			{
				ActivationID id = (ActivationID) obj;
				return (Uid.Equals(id.Uid) && Activator.Equals(id.Activator));
			}
			else
			{
				return false;
			}
		}

		/// <summary>
		/// <code>writeObject</code> for custom serialization.
		/// 
		/// <para>This method writes this object's serialized form for
		/// this class as follows:
		/// 
		/// </para>
		/// <para>The <code>writeObject</code> method is invoked on
		/// <code>out</code> passing this object's unique identifier
		/// (a <seealso cref="java.rmi.server.UID UID"/> instance) as the argument.
		/// 
		/// </para>
		/// <para>Next, the {@link
		/// java.rmi.server.RemoteRef#getRefClass(java.io.ObjectOutput)
		/// getRefClass} method is invoked on the activator's
		/// <code>RemoteRef</code> instance to obtain its external ref
		/// type name.  Next, the <code>writeUTF</code> method is
		/// invoked on <code>out</code> with the value returned by
		/// <code>getRefClass</code>, and then the
		/// <code>writeExternal</code> method is invoked on the
		/// <code>RemoteRef</code> instance passing <code>out</code>
		/// as the argument.
		/// 
		/// @serialData The serialized data for this class comprises a
		/// <code>java.rmi.server.UID</code> (written with
		/// <code>ObjectOutput.writeObject</code>) followed by the
		/// external ref type name of the activator's
		/// <code>RemoteRef</code> instance (a string written with
		/// <code>ObjectOutput.writeUTF</code>), followed by the
		/// external form of the <code>RemoteRef</code> instance as
		/// written by its <code>writeExternal</code> method.
		/// 
		/// </para>
		/// <para>The external ref type name of the
		/// <code>RemoteRef</Code> instance is
		/// determined using the definitions of external ref type
		/// names specified in the {@link java.rmi.server.RemoteObject
		/// RemoteObject} <code>writeObject</code> method
		/// <b>serialData</b> specification.  Similarly, the data
		/// written by the <code>writeExternal</code> method and read
		/// by the <code>readExternal</code> method of
		/// <code>RemoteRef</code> implementation classes
		/// corresponding to each of the defined external ref type
		/// names is specified in the {@link
		/// java.rmi.server.RemoteObject RemoteObject}
		/// <code>writeObject</code> method <b>serialData</b>
		/// specification.
		/// 
		/// </para>
		/// </summary>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: private void writeObject(java.io.ObjectOutputStream out) throws java.io.IOException, ClassNotFoundException
		private void WriteObject(ObjectOutputStream @out)
		{
			@out.WriteObject(Uid);

			RemoteRef @ref;
			if (Activator is RemoteObject)
			{
				@ref = ((RemoteObject) Activator).Ref;
			}
			else if (Proxy.isProxyClass(Activator.GetType()))
			{
				InvocationHandler handler = Proxy.getInvocationHandler(Activator);
				if (!(handler is RemoteObjectInvocationHandler))
				{
					throw new InvalidObjectException("unexpected invocation handler");
				}
				@ref = ((RemoteObjectInvocationHandler) handler).Ref;

			}
			else
			{
				throw new InvalidObjectException("unexpected activator type");
			}
			@out.WriteUTF(@ref.GetRefClass(@out));
			@ref.WriteExternal(@out);
		}

		/// <summary>
		/// <code>readObject</code> for custom serialization.
		/// 
		/// <para>This method reads this object's serialized form for this
		/// class as follows:
		/// 
		/// </para>
		/// <para>The <code>readObject</code> method is invoked on
		/// <code>in</code> to read this object's unique identifier
		/// (a <seealso cref="java.rmi.server.UID UID"/> instance).
		/// 
		/// </para>
		/// <para>Next, the <code>readUTF</code> method is invoked on
		/// <code>in</code> to read the external ref type name of the
		/// <code>RemoteRef</code> instance for this object's
		/// activator.  Next, the <code>RemoteRef</code>
		/// instance is created of an implementation-specific class
		/// corresponding to the external ref type name (returned by
		/// <code>readUTF</code>), and the <code>readExternal</code>
		/// method is invoked on that <code>RemoteRef</code> instance
		/// to read the external form corresponding to the external
		/// ref type name.
		/// 
		/// </para>
		/// <para>Note: If the external ref type name is
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
		/// case the <code>RemoteRef</code> will be an instance of
		/// that implementation-specific class.
		/// </para>
		/// </summary>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: private void readObject(java.io.ObjectInputStream in) throws java.io.IOException, ClassNotFoundException
		private void ReadObject(ObjectInputStream @in)
		{
			Uid = (UID)@in.ReadObject();

			try
			{
				Class refClass = Class.ForName(java.rmi.server.RemoteRef_Fields.PackagePrefix + "." + @in.ReadUTF()).AsSubclass(typeof(RemoteRef));
				RemoteRef @ref = refClass.NewInstance();
				@ref.ReadExternal(@in);
				Activator = (Activator) Proxy.newProxyInstance(null, new Class[] {typeof(Activator)}, new RemoteObjectInvocationHandler(@ref));

			}
			catch (InstantiationException e)
			{
				throw (IOException) (new InvalidObjectException("Unable to create remote reference")).InitCause(e);
			}
			catch (IllegalAccessException e)
			{
				throw (IOException) (new InvalidObjectException("Unable to create remote reference")).InitCause(e);
			}
		}
	}

}
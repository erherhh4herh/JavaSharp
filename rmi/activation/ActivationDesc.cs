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
	/// An activation descriptor contains the information necessary to
	/// activate an object: <ul>
	/// <li> the object's group identifier,
	/// <li> the object's fully-qualified class name,
	/// <li> the object's code location (the location of the class), a codebase URL
	/// path,
	/// <li> the object's restart "mode", and,
	/// <li> a "marshalled" object that can contain object specific
	/// initialization data. </ul>
	/// 
	/// <para>A descriptor registered with the activation system can be used to
	/// recreate/activate the object specified by the descriptor. The
	/// <code>MarshalledObject</code> in the object's descriptor is passed
	/// as the second argument to the remote object's constructor for
	/// object to use during reinitialization/activation.
	/// 
	/// @author      Ann Wollrath
	/// @since       1.2
	/// </para>
	/// </summary>
	/// <seealso cref=         java.rmi.activation.Activatable </seealso>
	[Serializable]
	public sealed class ActivationDesc
	{

		/// <summary>
		/// @serial the group's identifier
		/// </summary>
		private ActivationGroupID GroupID_Renamed;

		/// <summary>
		/// @serial the object's class name
		/// </summary>
		private String ClassName_Renamed;

		/// <summary>
		/// @serial the object's code location
		/// </summary>
		private String Location_Renamed;

		/// <summary>
		/// @serial the object's initialization data
		/// </summary>
//JAVA TO C# CONVERTER TODO TASK: Java wildcard generics are not converted to .NET:
//ORIGINAL LINE: private java.rmi.MarshalledObject<?> data;
		private MarshalledObject<?> Data_Renamed;

		/// <summary>
		/// @serial indicates whether the object should be restarted
		/// </summary>
		private bool Restart;

		/// <summary>
		/// indicate compatibility with the Java 2 SDK v1.2 version of class </summary>
		private const long SerialVersionUID = 7455834104417690957L;

		/// <summary>
		/// Constructs an object descriptor for an object whose class name
		/// is <code>className</code>, that can be loaded from the
		/// code <code>location</code> and whose initialization
		/// information is <code>data</code>. If this form of the constructor
		/// is used, the <code>groupID</code> defaults to the current id for
		/// <code>ActivationGroup</code> for this VM. All objects with the
		/// same <code>ActivationGroupID</code> are activated in the same VM.
		/// 
		/// <para>Note that objects specified by a descriptor created with this
		/// constructor will only be activated on demand (by default, the restart
		/// mode is <code>false</code>).  If an activatable object requires restart
		/// services, use one of the <code>ActivationDesc</code> constructors that
		/// takes a boolean parameter, <code>restart</code>.
		/// 
		/// </para>
		/// <para> This constructor will throw <code>ActivationException</code> if
		/// there is no current activation group for this VM.  To create an
		/// <code>ActivationGroup</code> use the
		/// <code>ActivationGroup.createGroup</code> method.
		/// 
		/// </para>
		/// </summary>
		/// <param name="className"> the object's fully package qualified class name </param>
		/// <param name="location"> the object's code location (from where the class is
		/// loaded) </param>
		/// <param name="data"> the object's initialization (activation) data contained
		/// in marshalled form. </param>
		/// <exception cref="ActivationException"> if the current group is nonexistent </exception>
		/// <exception cref="UnsupportedOperationException"> if and only if activation is
		/// not supported by this implementation
		/// @since 1.2 </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public ActivationDesc(String className, String location, java.rmi.MarshalledObject<?> data) throws ActivationException
		public ActivationDesc<T1>(String className, String location, MarshalledObject<T1> data) : this(ActivationGroup.InternalCurrentGroupID(), className, location, data, false)
		{
		}

		/// <summary>
		/// Constructs an object descriptor for an object whose class name
		/// is <code>className</code>, that can be loaded from the
		/// code <code>location</code> and whose initialization
		/// information is <code>data</code>. If this form of the constructor
		/// is used, the <code>groupID</code> defaults to the current id for
		/// <code>ActivationGroup</code> for this VM. All objects with the
		/// same <code>ActivationGroupID</code> are activated in the same VM.
		/// 
		/// <para>This constructor will throw <code>ActivationException</code> if
		/// there is no current activation group for this VM.  To create an
		/// <code>ActivationGroup</code> use the
		/// <code>ActivationGroup.createGroup</code> method.
		/// 
		/// </para>
		/// </summary>
		/// <param name="className"> the object's fully package qualified class name </param>
		/// <param name="location"> the object's code location (from where the class is
		/// loaded) </param>
		/// <param name="data"> the object's initialization (activation) data contained
		/// in marshalled form. </param>
		/// <param name="restart"> if true, the object is restarted (reactivated) when
		/// either the activator is restarted or the object's activation group
		/// is restarted after an unexpected crash; if false, the object is only
		/// activated on demand.  Specifying <code>restart</code> to be
		/// <code>true</code> does not force an initial immediate activation of
		/// a newly registered object;  initial activation is lazy. </param>
		/// <exception cref="ActivationException"> if the current group is nonexistent </exception>
		/// <exception cref="UnsupportedOperationException"> if and only if activation is
		/// not supported by this implementation
		/// @since 1.2 </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public ActivationDesc(String className, String location, java.rmi.MarshalledObject<?> data, boolean restart) throws ActivationException
		public ActivationDesc<T1>(String className, String location, MarshalledObject<T1> data, bool restart) : this(ActivationGroup.InternalCurrentGroupID(), className, location, data, restart)
		{
		}

		/// <summary>
		/// Constructs an object descriptor for an object whose class name
		/// is <code>className</code> that can be loaded from the
		/// code <code>location</code> and whose initialization
		/// information is <code>data</code>. All objects with the same
		/// <code>groupID</code> are activated in the same Java VM.
		/// 
		/// <para>Note that objects specified by a descriptor created with this
		/// constructor will only be activated on demand (by default, the restart
		/// mode is <code>false</code>).  If an activatable object requires restart
		/// services, use one of the <code>ActivationDesc</code> constructors that
		/// takes a boolean parameter, <code>restart</code>.
		/// 
		/// </para>
		/// </summary>
		/// <param name="groupID"> the group's identifier (obtained from registering
		/// <code>ActivationSystem.registerGroup</code> method). The group
		/// indicates the VM in which the object should be activated. </param>
		/// <param name="className"> the object's fully package-qualified class name </param>
		/// <param name="location"> the object's code location (from where the class is
		/// loaded) </param>
		/// <param name="data">  the object's initialization (activation) data contained
		/// in marshalled form. </param>
		/// <exception cref="IllegalArgumentException"> if <code>groupID</code> is null </exception>
		/// <exception cref="UnsupportedOperationException"> if and only if activation is
		/// not supported by this implementation
		/// @since 1.2 </exception>
		public ActivationDesc<T1>(ActivationGroupID groupID, String className, String location, MarshalledObject<T1> data) : this(groupID, className, location, data, false)
		{
		}

		/// <summary>
		/// Constructs an object descriptor for an object whose class name
		/// is <code>className</code> that can be loaded from the
		/// code <code>location</code> and whose initialization
		/// information is <code>data</code>. All objects with the same
		/// <code>groupID</code> are activated in the same Java VM.
		/// </summary>
		/// <param name="groupID"> the group's identifier (obtained from registering
		/// <code>ActivationSystem.registerGroup</code> method). The group
		/// indicates the VM in which the object should be activated. </param>
		/// <param name="className"> the object's fully package-qualified class name </param>
		/// <param name="location"> the object's code location (from where the class is
		/// loaded) </param>
		/// <param name="data">  the object's initialization (activation) data contained
		/// in marshalled form. </param>
		/// <param name="restart"> if true, the object is restarted (reactivated) when
		/// either the activator is restarted or the object's activation group
		/// is restarted after an unexpected crash; if false, the object is only
		/// activated on demand.  Specifying <code>restart</code> to be
		/// <code>true</code> does not force an initial immediate activation of
		/// a newly registered object;  initial activation is lazy. </param>
		/// <exception cref="IllegalArgumentException"> if <code>groupID</code> is null </exception>
		/// <exception cref="UnsupportedOperationException"> if and only if activation is
		/// not supported by this implementation
		/// @since 1.2 </exception>
		public ActivationDesc<T1>(ActivationGroupID groupID, String className, String location, MarshalledObject<T1> data, bool restart)
		{
			if (groupID == null)
			{
				throw new IllegalArgumentException("groupID can't be null");
			}
			this.GroupID_Renamed = groupID;
			this.ClassName_Renamed = className;
			this.Location_Renamed = location;
			this.Data_Renamed = data;
			this.Restart = restart;
		}

		/// <summary>
		/// Returns the group identifier for the object specified by this
		/// descriptor. A group provides a way to aggregate objects into a
		/// single Java virtual machine. RMI creates/activates objects with
		/// the same <code>groupID</code> in the same virtual machine.
		/// </summary>
		/// <returns> the group identifier
		/// @since 1.2 </returns>
		public ActivationGroupID GroupID
		{
			get
			{
				return GroupID_Renamed;
			}
		}

		/// <summary>
		/// Returns the class name for the object specified by this
		/// descriptor. </summary>
		/// <returns> the class name
		/// @since 1.2 </returns>
		public String ClassName
		{
			get
			{
				return ClassName_Renamed;
			}
		}

		/// <summary>
		/// Returns the code location for the object specified by
		/// this descriptor. </summary>
		/// <returns> the code location
		/// @since 1.2 </returns>
		public String Location
		{
			get
			{
				return Location_Renamed;
			}
		}

		/// <summary>
		/// Returns a "marshalled object" containing intialization/activation
		/// data for the object specified by this descriptor. </summary>
		/// <returns> the object specific "initialization" data
		/// @since 1.2 </returns>
//JAVA TO C# CONVERTER TODO TASK: Java wildcard generics are not converted to .NET:
//ORIGINAL LINE: public java.rmi.MarshalledObject<?> getData()
		public MarshalledObject<?> Data
		{
			get
			{
				return Data_Renamed;
			}
		}

		/// <summary>
		/// Returns the "restart" mode of the object associated with
		/// this activation descriptor.
		/// </summary>
		/// <returns> true if the activatable object associated with this
		/// activation descriptor is restarted via the activation
		/// daemon when either the daemon comes up or the object's group
		/// is restarted after an unexpected crash; otherwise it returns false,
		/// meaning that the object is only activated on demand via a
		/// method call.  Note that if the restart mode is <code>true</code>, the
		/// activator does not force an initial immediate activation of
		/// a newly registered object;  initial activation is lazy.
		/// @since 1.2 </returns>
		public bool RestartMode
		{
			get
			{
				return Restart;
			}
		}

		/// <summary>
		/// Compares two activation descriptors for content equality.
		/// </summary>
		/// <param name="obj">     the Object to compare with </param>
		/// <returns>  true if these Objects are equal; false otherwise. </returns>
		/// <seealso cref=             java.util.Hashtable
		/// @since 1.2 </seealso>
		public override bool Equals(Object obj)
		{

			if (obj is ActivationDesc)
			{
				ActivationDesc desc = (ActivationDesc) obj;
				return ((GroupID_Renamed == null ? desc.GroupID_Renamed == null : GroupID_Renamed.Equals(desc.GroupID_Renamed)) && (ClassName_Renamed == null ? desc.ClassName_Renamed == null : ClassName_Renamed.Equals(desc.ClassName_Renamed)) && (Location_Renamed == null ? desc.Location_Renamed == null: Location_Renamed.Equals(desc.Location_Renamed)) && (Data_Renamed == null ? desc.Data_Renamed == null : Data_Renamed.Equals(desc.Data_Renamed)) && (Restart == desc.Restart));

			}
			else
			{
				return false;
			}
		}

		/// <summary>
		/// Return the same hashCode for similar <code>ActivationDesc</code>s. </summary>
		/// <returns> an integer </returns>
		/// <seealso cref= java.util.Hashtable </seealso>
		public override int HashCode()
		{
			return ((Location_Renamed == null ? 0 : Location_Renamed.HashCode() << 24) ^ (GroupID_Renamed == null ? 0 : GroupID_Renamed.HashCode() << 16) ^ (ClassName_Renamed == null ? 0 : ClassName_Renamed.HashCode() << 9) ^ (Data_Renamed == null ? 0 : Data_Renamed.HashCode() << 1) ^ (Restart ? 1 : 0));
		}
	}

}
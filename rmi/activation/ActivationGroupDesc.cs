using System;

/*
 * Copyright (c) 1997, 2008, Oracle and/or its affiliates. All rights reserved.
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
	/// An activation group descriptor contains the information necessary to
	/// create/recreate an activation group in which to activate objects.
	/// Such a descriptor contains: <ul>
	/// <li> the group's class name,
	/// <li> the group's code location (the location of the group's class), and
	/// <li> a "marshalled" object that can contain group specific
	/// initialization data. </ul> <para>
	/// 
	/// The group's class must be a concrete subclass of
	/// <code>ActivationGroup</code>. A subclass of
	/// <code>ActivationGroup</code> is created/recreated via the
	/// <code>ActivationGroup.createGroup</code> static method that invokes
	/// a special constructor that takes two arguments: <ul>
	/// 
	/// <li> the group's <code>ActivationGroupID</code>, and
	/// <li> the group's initialization data (in a
	/// </para>
	/// <code>java.rmi.MarshalledObject</code>)</ul><para>
	/// 
	/// @author      Ann Wollrath
	/// @since       1.2
	/// </para>
	/// </summary>
	/// <seealso cref=         ActivationGroup </seealso>
	/// <seealso cref=         ActivationGroupID </seealso>
	[Serializable]
	public sealed class ActivationGroupDesc
	{

		/// <summary>
		/// @serial The group's fully package qualified class name.
		/// </summary>
		private String ClassName_Renamed;

		/// <summary>
		/// @serial The location from where to load the group's class.
		/// </summary>
		private String Location_Renamed;

		/// <summary>
		/// @serial The group's initialization data.
		/// </summary>
//JAVA TO C# CONVERTER TODO TASK: Java wildcard generics are not converted to .NET:
//ORIGINAL LINE: private java.rmi.MarshalledObject<?> data;
		private MarshalledObject<?> Data_Renamed;

		/// <summary>
		/// @serial The controlling options for executing the VM in
		/// another process.
		/// </summary>
		private CommandEnvironment Env;

		/// <summary>
		/// @serial A properties map which will override those set
		/// by default in the subprocess environment.
		/// </summary>
		private Properties Props;

		/// <summary>
		/// indicate compatibility with the Java 2 SDK v1.2 version of class </summary>
		private const long SerialVersionUID = -4936225423168276595L;

		/// <summary>
		/// Constructs a group descriptor that uses the system defaults for group
		/// implementation and code location.  Properties specify Java
		/// environment overrides (which will override system properties in
		/// the group implementation's VM).  The command
		/// environment can control the exact command/options used in
		/// starting the child VM, or can be <code>null</code> to accept
		/// rmid's default.
		/// 
		/// <para>This constructor will create an <code>ActivationGroupDesc</code>
		/// with a <code>null</code> group class name, which indicates the system's
		/// default <code>ActivationGroup</code> implementation.
		/// 
		/// </para>
		/// </summary>
		/// <param name="overrides"> the set of properties to set when the group is
		/// recreated. </param>
		/// <param name="cmd"> the controlling options for executing the VM in
		/// another process (or <code>null</code>).
		/// @since 1.2 </param>
		public ActivationGroupDesc(Properties overrides, CommandEnvironment cmd) : this(null, null, null, overrides, cmd)
		{
		}

		/// <summary>
		/// Specifies an alternate group implementation and execution
		/// environment to be used for the group.
		/// </summary>
		/// <param name="className"> the group's package qualified class name or
		/// <code>null</code>. A <code>null</code> group class name indicates
		/// the system's default <code>ActivationGroup</code> implementation. </param>
		/// <param name="location"> the location from where to load the group's
		/// class </param>
		/// <param name="data"> the group's initialization data contained in
		/// marshalled form (could contain properties, for example) </param>
		/// <param name="overrides"> a properties map which will override those set
		/// by default in the subprocess environment (will be translated
		/// into <code>-D</code> options), or <code>null</code>. </param>
		/// <param name="cmd"> the controlling options for executing the VM in
		/// another process (or <code>null</code>).
		/// @since 1.2 </param>
		public ActivationGroupDesc<T1>(String className, String location, MarshalledObject<T1> data, Properties overrides, CommandEnvironment cmd)
		{
			this.Props = overrides;
			this.Env = cmd;
			this.Data_Renamed = data;
			this.Location_Renamed = location;
			this.ClassName_Renamed = className;
		}

		/// <summary>
		/// Returns the group's class name (possibly <code>null</code>).  A
		/// <code>null</code> group class name indicates the system's default
		/// <code>ActivationGroup</code> implementation. </summary>
		/// <returns> the group's class name
		/// @since 1.2 </returns>
		public String ClassName
		{
			get
			{
				return ClassName_Renamed;
			}
		}

		/// <summary>
		/// Returns the group's code location. </summary>
		/// <returns> the group's code location
		/// @since 1.2 </returns>
		public String Location
		{
			get
			{
				return Location_Renamed;
			}
		}

		/// <summary>
		/// Returns the group's initialization data. </summary>
		/// <returns> the group's initialization data
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
		/// Returns the group's property-override list. </summary>
		/// <returns> the property-override list, or <code>null</code>
		/// @since 1.2 </returns>
		public Properties PropertyOverrides
		{
			get
			{
				return (Props != null) ? (Properties) Props.Clone() : null;
			}
		}

		/// <summary>
		/// Returns the group's command-environment control object. </summary>
		/// <returns> the command-environment object, or <code>null</code>
		/// @since 1.2 </returns>
		public CommandEnvironment CommandEnvironment
		{
			get
			{
				return this.Env;
			}
		}


		/// <summary>
		/// Startup options for ActivationGroup implementations.
		/// 
		/// This class allows overriding default system properties and
		/// specifying implementation-defined options for ActivationGroups.
		/// @since 1.2
		/// </summary>
		[Serializable]
		public class CommandEnvironment
		{
			internal const long SerialVersionUID = 6165754737887770191L;

			/// <summary>
			/// @serial
			/// </summary>
			internal String Command;

			/// <summary>
			/// @serial
			/// </summary>
			internal String[] Options;

			/// <summary>
			/// Create a CommandEnvironment with all the necessary
			/// information.
			/// </summary>
			/// <param name="cmdpath"> the name of the java executable, including
			/// the full path, or <code>null</code>, meaning "use rmid's default".
			/// The named program <em>must</em> be able to accept multiple
			/// <code>-Dpropname=value</code> options (as documented for the
			/// "java" tool)
			/// </param>
			/// <param name="argv"> extra options which will be used in creating the
			/// ActivationGroup.  Null has the same effect as an empty
			/// list.
			/// @since 1.2 </param>
			public CommandEnvironment(String cmdpath, String[] argv)
			{
				this.Command = cmdpath; // might be null

				// Hold a safe copy of argv in this.options
				if (argv == null)
				{
					this.Options = new String[0];
				}
				else
				{
					this.Options = new String[argv.Length];
					System.Array.Copy(argv, 0, this.Options, 0, argv.Length);
				}
			}

			/// <summary>
			/// Fetch the configured path-qualified java command name.
			/// </summary>
			/// <returns> the configured name, or <code>null</code> if configured to
			/// accept the default
			/// @since 1.2 </returns>
			public virtual String CommandPath
			{
				get
				{
					return (this.Command);
				}
			}

			/// <summary>
			/// Fetch the configured java command options.
			/// </summary>
			/// <returns> An array of the command options which will be passed
			/// to the new child command by rmid.
			/// Note that rmid may add other options before or after these
			/// options, or both.
			/// Never returns <code>null</code>.
			/// @since 1.2 </returns>
			public virtual String[] CommandOptions
			{
				get
				{
					return Options.clone();
				}
			}

			/// <summary>
			/// Compares two command environments for content equality.
			/// </summary>
			/// <param name="obj">     the Object to compare with </param>
			/// <returns>      true if these Objects are equal; false otherwise. </returns>
			/// <seealso cref=         java.util.Hashtable
			/// @since 1.2 </seealso>
			public override bool Equals(Object obj)
			{

				if (obj is CommandEnvironment)
				{
					CommandEnvironment env = (CommandEnvironment) obj;
					return ((Command == null ? env.Command == null : Command.Equals(env.Command)) && Arrays.Equals(Options, env.Options));
				}
				else
				{
					return false;
				}
			}

			/// <summary>
			/// Return identical values for similar
			/// <code>CommandEnvironment</code>s. </summary>
			/// <returns> an integer </returns>
			/// <seealso cref= java.util.Hashtable </seealso>
			public override int HashCode()
			{
				// hash command and ignore possibly expensive options
				return (Command == null ? 0 : Command.HashCode());
			}

			/// <summary>
			/// <code>readObject</code> for custom serialization.
			/// 
			/// <para>This method reads this object's serialized form for this
			/// class as follows:
			/// 
			/// </para>
			/// <para>This method first invokes <code>defaultReadObject</code> on
			/// the specified object input stream, and if <code>options</code>
			/// is <code>null</code>, then <code>options</code> is set to a
			/// zero-length array of <code>String</code>.
			/// </para>
			/// </summary>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: private void readObject(java.io.ObjectInputStream in) throws java.io.IOException, ClassNotFoundException
			internal virtual void ReadObject(ObjectInputStream @in)
			{
				@in.DefaultReadObject();
				if (Options == null)
				{
					Options = new String[0];
				}
			}
		}

		/// <summary>
		/// Compares two activation group descriptors for content equality.
		/// </summary>
		/// <param name="obj">     the Object to compare with </param>
		/// <returns>  true if these Objects are equal; false otherwise. </returns>
		/// <seealso cref=             java.util.Hashtable
		/// @since 1.2 </seealso>
		public override bool Equals(Object obj)
		{

			if (obj is ActivationGroupDesc)
			{
				ActivationGroupDesc desc = (ActivationGroupDesc) obj;
				return ((ClassName_Renamed == null ? desc.ClassName_Renamed == null : ClassName_Renamed.Equals(desc.ClassName_Renamed)) && (Location_Renamed == null ? desc.Location_Renamed == null : Location_Renamed.Equals(desc.Location_Renamed)) && (Data_Renamed == null ? desc.Data_Renamed == null : Data_Renamed.Equals(desc.Data_Renamed)) && (Env == null ? desc.Env == null : Env.Equals(desc.Env)) && (Props == null ? desc.Props == null : Props.Equals(desc.Props)));
			}
			else
			{
				return false;
			}
		}

		/// <summary>
		/// Produce identical numbers for similar <code>ActivationGroupDesc</code>s. </summary>
		/// <returns> an integer </returns>
		/// <seealso cref= java.util.Hashtable </seealso>
		public override int HashCode()
		{
			// hash location, className, data, and env
			// but omit props (may be expensive)
			return ((Location_Renamed == null ? 0 : Location_Renamed.HashCode() << 24) ^ (Env == null ? 0 : Env.HashCode() << 16) ^ (ClassName_Renamed == null ? 0 : ClassName_Renamed.HashCode() << 8) ^ (Data_Renamed == null ? 0 : Data_Renamed.HashCode()));
		}
	}

}
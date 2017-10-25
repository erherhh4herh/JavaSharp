/*
 * Copyright (c) 1996, 2013, Oracle and/or its affiliates. All rights reserved.
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

namespace java.sql
{

	/// <summary>
	/// <para>Driver properties for making a connection. The
	/// <code>DriverPropertyInfo</code> class is of interest only to advanced programmers
	/// who need to interact with a Driver via the method
	/// <code>getDriverProperties</code> to discover
	/// and supply properties for connections.
	/// </para>
	/// </summary>

	public class DriverPropertyInfo
	{

		/// <summary>
		/// Constructs a <code>DriverPropertyInfo</code> object with a  given
		/// name and value.  The <code>description</code> and <code>choices</code>
		/// are initialized to <code>null</code> and <code>required</code> is initialized
		/// to <code>false</code>.
		/// </summary>
		/// <param name="name"> the name of the property </param>
		/// <param name="value"> the current value, which may be null </param>
		public DriverPropertyInfo(String name, String value)
		{
			this.Name = name;
			this.Value = value;
		}

		/// <summary>
		/// The name of the property.
		/// </summary>
		public String Name;

		/// <summary>
		/// A brief description of the property, which may be null.
		/// </summary>
		public String Description = null;

		/// <summary>
		/// The <code>required</code> field is <code>true</code> if a value must be
		/// supplied for this property
		/// during <code>Driver.connect</code> and <code>false</code> otherwise.
		/// </summary>
		public bool Required = false;

		/// <summary>
		/// The <code>value</code> field specifies the current value of
		/// the property, based on a combination of the information
		/// supplied to the method <code>getPropertyInfo</code>, the
		/// Java environment, and the driver-supplied default values.  This field
		/// may be null if no value is known.
		/// </summary>
		public String Value = null;

		/// <summary>
		/// An array of possible values if the value for the field
		/// <code>DriverPropertyInfo.value</code> may be selected
		/// from a particular set of values; otherwise null.
		/// </summary>
		public String[] Choices = null;
	}

}
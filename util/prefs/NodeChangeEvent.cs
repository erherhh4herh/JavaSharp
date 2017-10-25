/*
 * Copyright (c) 2000, 2003, Oracle and/or its affiliates. All rights reserved.
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

namespace java.util.prefs
{

	/// <summary>
	/// An event emitted by a <tt>Preferences</tt> node to indicate that
	/// a child of that node has been added or removed.<para>
	/// 
	/// Note, that although NodeChangeEvent inherits Serializable interface from
	/// java.util.EventObject, it is not intended to be Serializable. Appropriate
	/// serialization methods are implemented to throw NotSerializableException.
	/// 
	/// @author  Josh Bloch
	/// </para>
	/// </summary>
	/// <seealso cref=     Preferences </seealso>
	/// <seealso cref=     NodeChangeListener </seealso>
	/// <seealso cref=     PreferenceChangeEvent
	/// @since   1.4
	/// @serial  exclude </seealso>

	public class NodeChangeEvent : java.util.EventObject
	{
		/// <summary>
		/// The node that was added or removed.
		/// 
		/// @serial
		/// </summary>
		private Preferences Child_Renamed;

		/// <summary>
		/// Constructs a new <code>NodeChangeEvent</code> instance.
		/// </summary>
		/// <param name="parent">  The parent of the node that was added or removed. </param>
		/// <param name="child">   The node that was added or removed. </param>
		public NodeChangeEvent(Preferences parent, Preferences child) : base(parent)
		{
			this.Child_Renamed = child;
		}

		/// <summary>
		/// Returns the parent of the node that was added or removed.
		/// </summary>
		/// <returns>  The parent Preferences node whose child was added or removed </returns>
		public virtual Preferences Parent
		{
			get
			{
				return (Preferences) Source;
			}
		}

		/// <summary>
		/// Returns the node that was added or removed.
		/// </summary>
		/// <returns>  The node that was added or removed. </returns>
		public virtual Preferences Child
		{
			get
			{
				return Child_Renamed;
			}
		}

		/// <summary>
		/// Throws NotSerializableException, since NodeChangeEvent objects are not
		/// intended to be serializable.
		/// </summary>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: private void writeObject(java.io.ObjectOutputStream out) throws java.io.NotSerializableException
		 private void WriteObject(java.io.ObjectOutputStream @out)
		 {
			 throw new NotSerializableException("Not serializable.");
		 }

		/// <summary>
		/// Throws NotSerializableException, since NodeChangeEvent objects are not
		/// intended to be serializable.
		/// </summary>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: private void readObject(java.io.ObjectInputStream in) throws java.io.NotSerializableException
		 private void ReadObject(java.io.ObjectInputStream @in)
		 {
			 throw new NotSerializableException("Not serializable.");
		 }

		// Defined so that this class isn't flagged as a potential problem when
		// searches for missing serialVersionUID fields are done.
		private const long SerialVersionUID = 8068949086596572957L;
	}

}
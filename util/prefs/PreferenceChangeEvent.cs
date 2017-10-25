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
	/// a preference has been added, removed or has had its value changed.<para>
	/// 
	/// Note, that although PreferenceChangeEvent inherits Serializable interface
	/// from EventObject, it is not intended to be Serializable. Appropriate
	/// serialization methods are implemented to throw NotSerializableException.
	/// 
	/// @author  Josh Bloch
	/// </para>
	/// </summary>
	/// <seealso cref= Preferences </seealso>
	/// <seealso cref= PreferenceChangeListener </seealso>
	/// <seealso cref= NodeChangeEvent
	/// @since   1.4
	/// @serial exclude </seealso>
	public class PreferenceChangeEvent : java.util.EventObject
	{

		/// <summary>
		/// Key of the preference that changed.
		/// 
		/// @serial
		/// </summary>
		private String Key_Renamed;

		/// <summary>
		/// New value for preference, or <tt>null</tt> if it was removed.
		/// 
		/// @serial
		/// </summary>
		private String NewValue_Renamed;

		/// <summary>
		/// Constructs a new <code>PreferenceChangeEvent</code> instance.
		/// </summary>
		/// <param name="node">  The Preferences node that emitted the event. </param>
		/// <param name="key">  The key of the preference that was changed. </param>
		/// <param name="newValue">  The new value of the preference, or <tt>null</tt>
		///                  if the preference is being removed. </param>
		public PreferenceChangeEvent(Preferences node, String key, String newValue) : base(node)
		{
			this.Key_Renamed = key;
			this.NewValue_Renamed = newValue;
		}

		/// <summary>
		/// Returns the preference node that emitted the event.
		/// </summary>
		/// <returns>  The preference node that emitted the event. </returns>
		public virtual Preferences Node
		{
			get
			{
				return (Preferences) Source;
			}
		}

		/// <summary>
		/// Returns the key of the preference that was changed.
		/// </summary>
		/// <returns>  The key of the preference that was changed. </returns>
		public virtual String Key
		{
			get
			{
				return Key_Renamed;
			}
		}

		/// <summary>
		/// Returns the new value for the preference.
		/// </summary>
		/// <returns>  The new value for the preference, or <tt>null</tt> if the
		///          preference was removed. </returns>
		public virtual String NewValue
		{
			get
			{
				return NewValue_Renamed;
			}
		}

		/// <summary>
		/// Throws NotSerializableException, since NodeChangeEvent objects
		/// are not intended to be serializable.
		/// </summary>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: private void writeObject(java.io.ObjectOutputStream out) throws java.io.NotSerializableException
		 private void WriteObject(java.io.ObjectOutputStream @out)
		 {
			 throw new NotSerializableException("Not serializable.");
		 }

		/// <summary>
		/// Throws NotSerializableException, since PreferenceChangeEvent objects
		/// are not intended to be serializable.
		/// </summary>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: private void readObject(java.io.ObjectInputStream in) throws java.io.NotSerializableException
		 private void ReadObject(java.io.ObjectInputStream @in)
		 {
			 throw new NotSerializableException("Not serializable.");
		 }

		// Defined so that this class isn't flagged as a potential problem when
		// searches for missing serialVersionUID fields are done.
		private const long SerialVersionUID = 793724513368024975L;
	}

}
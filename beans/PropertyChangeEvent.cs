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

namespace java.beans
{

	/// <summary>
	/// A "PropertyChange" event gets delivered whenever a bean changes a "bound"
	/// or "constrained" property.  A PropertyChangeEvent object is sent as an
	/// argument to the PropertyChangeListener and VetoableChangeListener methods.
	/// <P>
	/// Normally PropertyChangeEvents are accompanied by the name and the old
	/// and new value of the changed property.  If the new value is a primitive
	/// type (such as int or boolean) it must be wrapped as the
	/// corresponding java.lang.* Object type (such as Integer or Boolean).
	/// <P>
	/// Null values may be provided for the old and the new values if their
	/// true values are not known.
	/// <P>
	/// An event source may send a null object as the name to indicate that an
	/// arbitrary set of if its properties have changed.  In this case the
	/// old and new values should also be null.
	/// </summary>
	public class PropertyChangeEvent : EventObject
	{
		private const long SerialVersionUID = 7042693688939648123L;

		/// <summary>
		/// Constructs a new {@code PropertyChangeEvent}.
		/// </summary>
		/// <param name="source">        the bean that fired the event </param>
		/// <param name="propertyName">  the programmatic name of the property that was changed </param>
		/// <param name="oldValue">      the old value of the property </param>
		/// <param name="newValue">      the new value of the property
		/// </param>
		/// <exception cref="IllegalArgumentException"> if {@code source} is {@code null} </exception>
		public PropertyChangeEvent(Object source, String propertyName, Object oldValue, Object newValue) : base(source)
		{
			this.PropertyName_Renamed = propertyName;
			this.NewValue_Renamed = newValue;
			this.OldValue_Renamed = oldValue;
		}

		/// <summary>
		/// Gets the programmatic name of the property that was changed.
		/// </summary>
		/// <returns>  The programmatic name of the property that was changed.
		///          May be null if multiple properties have changed. </returns>
		public virtual String PropertyName
		{
			get
			{
				return PropertyName_Renamed;
			}
		}

		/// <summary>
		/// Gets the new value for the property, expressed as an Object.
		/// </summary>
		/// <returns>  The new value for the property, expressed as an Object.
		///          May be null if multiple properties have changed. </returns>
		public virtual Object NewValue
		{
			get
			{
				return NewValue_Renamed;
			}
		}

		/// <summary>
		/// Gets the old value for the property, expressed as an Object.
		/// </summary>
		/// <returns>  The old value for the property, expressed as an Object.
		///          May be null if multiple properties have changed. </returns>
		public virtual Object OldValue
		{
			get
			{
				return OldValue_Renamed;
			}
		}

		/// <summary>
		/// Sets the propagationId object for the event.
		/// </summary>
		/// <param name="propagationId">  The propagationId object for the event. </param>
		public virtual Object PropagationId
		{
			set
			{
				this.PropagationId_Renamed = value;
			}
			get
			{
				return PropagationId_Renamed;
			}
		}


		/// <summary>
		/// name of the property that changed.  May be null, if not known.
		/// @serial
		/// </summary>
		private String PropertyName_Renamed;

		/// <summary>
		/// New value for property.  May be null if not known.
		/// @serial
		/// </summary>
		private Object NewValue_Renamed;

		/// <summary>
		/// Previous value for property.  May be null if not known.
		/// @serial
		/// </summary>
		private Object OldValue_Renamed;

		/// <summary>
		/// Propagation ID.  May be null.
		/// @serial </summary>
		/// <seealso cref= #getPropagationId </seealso>
		private Object PropagationId_Renamed;

		/// <summary>
		/// Returns a string representation of the object.
		/// </summary>
		/// <returns> a string representation of the object
		/// 
		/// @since 1.7 </returns>
		public override String ToString()
		{
//JAVA TO C# CONVERTER WARNING: The .NET Type.FullName property will not always yield results identical to the Java Class.getName method:
			StringBuilder sb = new StringBuilder(this.GetType().FullName);
			sb.Append("[propertyName=").Append(PropertyName);
			AppendTo(sb);
			sb.Append("; oldValue=").Append(OldValue);
			sb.Append("; newValue=").Append(NewValue);
			sb.Append("; propagationId=").Append(PropagationId);
			sb.Append("; source=").Append(Source);
			return sb.Append("]").ToString();
		}

		internal virtual void AppendTo(StringBuilder sb)
		{
		}
	}

}
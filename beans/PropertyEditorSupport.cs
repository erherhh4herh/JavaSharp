using System.Collections.Generic;

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

namespace java.beans
{

	/// <summary>
	/// This is a support class to help build property editors.
	/// <para>
	/// It can be used either as a base class or as a delegate.
	/// </para>
	/// </summary>

	public class PropertyEditorSupport : PropertyEditor
	{

		/// <summary>
		/// Constructs a <code>PropertyEditorSupport</code> object.
		/// 
		/// @since 1.5
		/// </summary>
		public PropertyEditorSupport()
		{
			Source = this;
		}

		/// <summary>
		/// Constructs a <code>PropertyEditorSupport</code> object.
		/// </summary>
		/// <param name="source"> the source used for event firing
		/// @since 1.5 </param>
		public PropertyEditorSupport(Object source)
		{
			if (source == null)
			{
			   throw new NullPointerException();
			}
			Source = source;
		}

		/// <summary>
		/// Returns the bean that is used as the
		/// source of events. If the source has not
		/// been explicitly set then this instance of
		/// <code>PropertyEditorSupport</code> is returned.
		/// </summary>
		/// <returns> the source object or this instance
		/// @since 1.5 </returns>
		public virtual Object Source
		{
			get
			{
				return Source_Renamed;
			}
			set
			{
				this.Source_Renamed = value;
			}
		}


		/// <summary>
		/// Set (or change) the object that is to be edited.
		/// </summary>
		/// <param name="value"> The new target object to be edited.  Note that this
		///     object should not be modified by the PropertyEditor, rather
		///     the PropertyEditor should create a new object to hold any
		///     modified value. </param>
		public virtual Object Value
		{
			set
			{
				this.Value_Renamed = value;
				FirePropertyChange();
			}
			get
			{
				return Value_Renamed;
			}
		}


		//----------------------------------------------------------------------

		/// <summary>
		/// Determines whether the class will honor the paintValue method.
		/// </summary>
		/// <returns>  True if the class will honor the paintValue method. </returns>

		public virtual bool Paintable
		{
			get
			{
				return false;
			}
		}

		/// <summary>
		/// Paint a representation of the value into a given area of screen
		/// real estate.  Note that the propertyEditor is responsible for doing
		/// its own clipping so that it fits into the given rectangle.
		/// <para>
		/// If the PropertyEditor doesn't honor paint requests (see isPaintable)
		/// this method should be a silent noop.
		/// 
		/// </para>
		/// </summary>
		/// <param name="gfx">  Graphics object to paint into. </param>
		/// <param name="box">  Rectangle within graphics object into which we should paint. </param>
		public virtual void PaintValue(java.awt.Graphics gfx, java.awt.Rectangle box)
		{
		}

		//----------------------------------------------------------------------

		/// <summary>
		/// This method is intended for use when generating Java code to set
		/// the value of the property.  It should return a fragment of Java code
		/// that can be used to initialize a variable with the current property
		/// value.
		/// <para>
		/// Example results are "2", "new Color(127,127,34)", "Color.orange", etc.
		/// 
		/// </para>
		/// </summary>
		/// <returns> A fragment of Java code representing an initializer for the
		///          current value. </returns>
		public virtual String JavaInitializationString
		{
			get
			{
				return "???";
			}
		}

		//----------------------------------------------------------------------

		/// <summary>
		/// Gets the property value as a string suitable for presentation
		/// to a human to edit.
		/// </summary>
		/// <returns> The property value as a string suitable for presentation
		///       to a human to edit.
		/// <para>   Returns null if the value can't be expressed as a string.
		/// </para>
		/// <para>   If a non-null value is returned, then the PropertyEditor should
		///       be prepared to parse that string back in setAsText(). </returns>
		public virtual String AsText
		{
			get
			{
				return (this.Value_Renamed != null) ? this.Value_Renamed.ToString() : null;
			}
			set
			{
				if (Value_Renamed is String)
				{
					Value = value;
					return;
				}
				throw new System.ArgumentException(value);
			}
		}


		//----------------------------------------------------------------------

		/// <summary>
		/// If the property value must be one of a set of known tagged values,
		/// then this method should return an array of the tag values.  This can
		/// be used to represent (for example) enum values.  If a PropertyEditor
		/// supports tags, then it should support the use of setAsText with
		/// a tag value as a way of setting the value.
		/// </summary>
		/// <returns> The tag values for this property.  May be null if this
		///   property cannot be represented as a tagged value.
		///  </returns>
		public virtual String[] Tags
		{
			get
			{
				return null;
			}
		}

		//----------------------------------------------------------------------

		/// <summary>
		/// A PropertyEditor may chose to make available a full custom Component
		/// that edits its property value.  It is the responsibility of the
		/// PropertyEditor to hook itself up to its editor Component itself and
		/// to report property value changes by firing a PropertyChange event.
		/// <P>
		/// The higher-level code that calls getCustomEditor may either embed
		/// the Component in some larger property sheet, or it may put it in
		/// its own individual dialog, or ...
		/// </summary>
		/// <returns> A java.awt.Component that will allow a human to directly
		///      edit the current property value.  May be null if this is
		///      not supported. </returns>

		public virtual java.awt.Component CustomEditor
		{
			get
			{
				return null;
			}
		}

		/// <summary>
		/// Determines whether the propertyEditor can provide a custom editor.
		/// </summary>
		/// <returns>  True if the propertyEditor can provide a custom editor. </returns>
		public virtual bool SupportsCustomEditor()
		{
			return false;
		}

		//----------------------------------------------------------------------

		/// <summary>
		/// Adds a listener for the value change.
		/// When the property editor changes its value
		/// it should fire a <seealso cref="PropertyChangeEvent"/>
		/// on all registered <seealso cref="PropertyChangeListener"/>s,
		/// specifying the {@code null} value for the property name.
		/// If the source property is set,
		/// it should be used as the source of the event.
		/// <para>
		/// The same listener object may be added more than once,
		/// and will be called as many times as it is added.
		/// If {@code listener} is {@code null},
		/// no exception is thrown and no action is taken.
		/// 
		/// </para>
		/// </summary>
		/// <param name="listener">  the <seealso cref="PropertyChangeListener"/> to add </param>
		public virtual void AddPropertyChangeListener(PropertyChangeListener listener)
		{
			lock (this)
			{
				if (Listeners == null)
				{
					Listeners = new List<>();
				}
				Listeners.Add(listener);
			}
		}

		/// <summary>
		/// Removes a listener for the value change.
		/// <para>
		/// If the same listener was added more than once,
		/// it will be notified one less time after being removed.
		/// If {@code listener} is {@code null}, or was never added,
		/// no exception is thrown and no action is taken.
		/// 
		/// </para>
		/// </summary>
		/// <param name="listener">  the <seealso cref="PropertyChangeListener"/> to remove </param>
		public virtual void RemovePropertyChangeListener(PropertyChangeListener listener)
		{
			lock (this)
			{
				if (Listeners == null)
				{
					return;
				}
				Listeners.Remove(listener);
			}
		}

		/// <summary>
		/// Report that we have been modified to any interested listeners.
		/// </summary>
		public virtual void FirePropertyChange()
		{
			List<PropertyChangeListener> targets;
			lock (this)
			{
				if (Listeners == null)
				{
					return;
				}
				targets = UnsafeClone(Listeners);
			}
			// Tell our listeners that "everything" has changed.
			PropertyChangeEvent evt = new PropertyChangeEvent(Source_Renamed, null, null, null);

			for (int i = 0; i < targets.Count; i++)
			{
				PropertyChangeListener target = targets[i];
				target.PropertyChange(evt);
			}
		}

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @SuppressWarnings("unchecked") private <T> java.util.Vector<T> unsafeClone(java.util.Vector<T> v)
		private List<T> unsafeClone<T>(List<T> v)
		{
			return (List<T>)v.clone();
		}

		//----------------------------------------------------------------------

		private Object Value_Renamed;
		private Object Source_Renamed;
		private List<PropertyChangeListener> Listeners;
	}

}
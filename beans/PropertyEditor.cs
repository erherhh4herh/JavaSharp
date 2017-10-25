/*
 * Copyright (c) 1996, 2003, Oracle and/or its affiliates. All rights reserved.
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
	/// A PropertyEditor class provides support for GUIs that want to
	/// allow users to edit a property value of a given type.
	/// <para>
	/// PropertyEditor supports a variety of different kinds of ways of
	/// displaying and updating property values.  Most PropertyEditors will
	/// only need to support a subset of the different options available in
	/// this API.
	/// <P>
	/// Simple PropertyEditors may only support the getAsText and setAsText
	/// methods and need not support (say) paintValue or getCustomEditor.  More
	/// complex types may be unable to support getAsText and setAsText but will
	/// instead support paintValue and getCustomEditor.
	/// </para>
	/// <para>
	/// Every propertyEditor must support one or more of the three simple
	/// display styles.  Thus it can either (1) support isPaintable or (2)
	/// both return a non-null String[] from getTags() and return a non-null
	/// value from getAsText or (3) simply return a non-null String from
	/// getAsText().
	/// </para>
	/// <para>
	/// Every property editor must support a call on setValue when the argument
	/// object is of the type for which this is the corresponding propertyEditor.
	/// In addition, each property editor must either support a custom editor,
	/// or support setAsText.
	/// </para>
	/// <para>
	/// Each PropertyEditor should have a null constructor.
	/// </para>
	/// </summary>

	public interface PropertyEditor
	{

		/// <summary>
		/// Set (or change) the object that is to be edited.  Primitive types such
		/// as "int" must be wrapped as the corresponding object type such as
		/// "java.lang.Integer".
		/// </summary>
		/// <param name="value"> The new target object to be edited.  Note that this
		///     object should not be modified by the PropertyEditor, rather
		///     the PropertyEditor should create a new object to hold any
		///     modified value. </param>
		Object Value {set;get;}



		//----------------------------------------------------------------------

		/// <summary>
		/// Determines whether this property editor is paintable.
		/// </summary>
		/// <returns>  True if the class will honor the paintValue method. </returns>

		bool Paintable {get;}

		/// <summary>
		/// Paint a representation of the value into a given area of screen
		/// real estate.  Note that the propertyEditor is responsible for doing
		/// its own clipping so that it fits into the given rectangle.
		/// <para>
		/// If the PropertyEditor doesn't honor paint requests (see isPaintable)
		/// this method should be a silent noop.
		/// </para>
		/// <para>
		/// The given Graphics object will have the default font, color, etc of
		/// the parent container.  The PropertyEditor may change graphics attributes
		/// such as font and color and doesn't need to restore the old values.
		/// 
		/// </para>
		/// </summary>
		/// <param name="gfx">  Graphics object to paint into. </param>
		/// <param name="box">  Rectangle within graphics object into which we should paint. </param>
		void PaintValue(java.awt.Graphics gfx, java.awt.Rectangle box);

		//----------------------------------------------------------------------

		/// <summary>
		/// Returns a fragment of Java code that can be used to set a property
		/// to match the editors current state. This method is intended
		/// for use when generating Java code to reflect changes made through the
		/// property editor.
		/// <para>
		/// The code fragment should be context free and must be a legal Java
		/// expression as specified by the JLS.
		/// </para>
		/// <para>
		/// Specifically, if the expression represents a computation then all
		/// classes and static members should be fully qualified. This rule
		/// applies to constructors, static methods and non primitive arguments.
		/// </para>
		/// <para>
		/// Caution should be used when evaluating the expression as it may throw
		/// exceptions. In particular, code generators must ensure that generated
		/// code will compile in the presence of an expression that can throw
		/// checked exceptions.
		/// </para>
		/// <para>
		/// Example results are:
		/// <ul>
		/// <li>Primitive expresssion: <code>2</code>
		/// <li>Class constructor: <code>new java.awt.Color(127,127,34)</code>
		/// <li>Static field: <code>java.awt.Color.orange</code>
		/// <li>Static method: <code>javax.swing.Box.createRigidArea(new
		///                                   java.awt.Dimension(0, 5))</code>
		/// </ul>
		/// 
		/// </para>
		/// </summary>
		/// <returns> a fragment of Java code representing an initializer for the
		///         current value. It should not contain a semi-colon
		///         ('<code>;</code>') to end the expression. </returns>
		String JavaInitializationString {get;}

		//----------------------------------------------------------------------

		/// <summary>
		/// Gets the property value as text.
		/// </summary>
		/// <returns> The property value as a human editable string.
		/// <para>   Returns null if the value can't be expressed as an editable string.
		/// </para>
		/// <para>   If a non-null value is returned, then the PropertyEditor should
		///       be prepared to parse that string back in setAsText(). </returns>
		String AsText {get;set;}

		/// <summary>
		/// Set the property value by parsing a given String.  May raise
		/// java.lang.IllegalArgumentException if either the String is
		/// badly formatted or if this kind of property can't be expressed
		/// as text. </summary>
		/// <param name="text">  The string to be parsed. </param>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: void setAsText(String text) throws java.lang.IllegalArgumentException;

		//----------------------------------------------------------------------

		/// <summary>
		/// If the property value must be one of a set of known tagged values,
		/// then this method should return an array of the tags.  This can
		/// be used to represent (for example) enum values.  If a PropertyEditor
		/// supports tags, then it should support the use of setAsText with
		/// a tag value as a way of setting the value and the use of getAsText
		/// to identify the current value.
		/// </summary>
		/// <returns> The tag values for this property.  May be null if this
		///   property cannot be represented as a tagged value.
		///  </returns>
		String[] Tags {get;}

		//----------------------------------------------------------------------

		/// <summary>
		/// A PropertyEditor may choose to make available a full custom Component
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

		java.awt.Component CustomEditor {get;}

		/// <summary>
		/// Determines whether this property editor supports a custom editor.
		/// </summary>
		/// <returns>  True if the propertyEditor can provide a custom editor. </returns>
		bool SupportsCustomEditor();

		//----------------------------------------------------------------------

		/// <summary>
		/// Adds a listener for the value change.
		/// When the property editor changes its value
		/// it should fire a <seealso cref="PropertyChangeEvent"/>
		/// on all registered <seealso cref="PropertyChangeListener"/>s,
		/// specifying the {@code null} value for the property name
		/// and itself as the source.
		/// </summary>
		/// <param name="listener">  the <seealso cref="PropertyChangeListener"/> to add </param>
		void AddPropertyChangeListener(PropertyChangeListener listener);

		/// <summary>
		/// Removes a listener for the value change.
		/// </summary>
		/// <param name="listener">  the <seealso cref="PropertyChangeListener"/> to remove </param>
		void RemovePropertyChangeListener(PropertyChangeListener listener);

	}

}
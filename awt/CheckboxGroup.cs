using System;

/*
 * Copyright (c) 1995, 2013, Oracle and/or its affiliates. All rights reserved.
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
namespace java.awt
{

	/// <summary>
	/// The <code>CheckboxGroup</code> class is used to group together
	/// a set of <code>Checkbox</code> buttons.
	/// <para>
	/// Exactly one check box button in a <code>CheckboxGroup</code> can
	/// be in the "on" state at any given time. Pushing any
	/// button sets its state to "on" and forces any other button that
	/// is in the "on" state into the "off" state.
	/// </para>
	/// <para>
	/// The following code example produces a new check box group,
	/// with three check boxes:
	/// 
	/// <hr><blockquote><pre>
	/// setLayout(new GridLayout(3, 1));
	/// CheckboxGroup cbg = new CheckboxGroup();
	/// add(new Checkbox("one", cbg, true));
	/// add(new Checkbox("two", cbg, false));
	/// add(new Checkbox("three", cbg, false));
	/// </pre></blockquote><hr>
	/// </para>
	/// <para>
	/// This image depicts the check box group created by this example:
	/// </para>
	/// <para>
	/// <img src="doc-files/CheckboxGroup-1.gif"
	/// alt="Shows three checkboxes, arranged vertically, labeled one, two, and three. Checkbox one is in the on state."
	/// style="float:center; margin: 7px 10px;">
	/// </para>
	/// <para>
	/// @author      Sami Shaio
	/// </para>
	/// </summary>
	/// <seealso cref=         java.awt.Checkbox
	/// @since       JDK1.0 </seealso>
	[Serializable]
	public class CheckboxGroup
	{
		/// <summary>
		/// The current choice.
		/// @serial </summary>
		/// <seealso cref= #getCurrent() </seealso>
		/// <seealso cref= #setCurrent(Checkbox) </seealso>
		internal Checkbox SelectedCheckbox_Renamed = null;

		/*
		 * JDK 1.1 serialVersionUID
		 */
		private const long SerialVersionUID = 3729780091441768983L;

		/// <summary>
		/// Creates a new instance of <code>CheckboxGroup</code>.
		/// </summary>
		public CheckboxGroup()
		{
		}

		/// <summary>
		/// Gets the current choice from this check box group.
		/// The current choice is the check box in this
		/// group that is currently in the "on" state,
		/// or <code>null</code> if all check boxes in the
		/// group are off. </summary>
		/// <returns>   the check box that is currently in the
		///                 "on" state, or <code>null</code>. </returns>
		/// <seealso cref=      java.awt.Checkbox </seealso>
		/// <seealso cref=      java.awt.CheckboxGroup#setSelectedCheckbox
		/// @since    JDK1.1 </seealso>
		public virtual Checkbox SelectedCheckbox
		{
			get
			{
				return Current;
			}
			set
			{
				Current = value;
			}
		}

		/// @deprecated As of JDK version 1.1,
		/// replaced by <code>getSelectedCheckbox()</code>. 
		[Obsolete("As of JDK version 1.1,")]
		public virtual Checkbox Current
		{
			get
			{
				return SelectedCheckbox_Renamed;
			}
			set
			{
				lock (this)
				{
					if (value != null && value.Group != this)
					{
						return;
					}
					Checkbox oldChoice = this.SelectedCheckbox_Renamed;
					this.SelectedCheckbox_Renamed = value;
					if (oldChoice != null && oldChoice != value && oldChoice.Group == this)
					{
						oldChoice.State = false;
					}
					if (value != null && oldChoice != value && !value.State)
					{
						value.StateInternal = true;
					}
				}
			}
		}



		/// <summary>
		/// Returns a string representation of this check box group,
		/// including the value of its current selection. </summary>
		/// <returns>    a string representation of this check box group. </returns>
		public override String ToString()
		{
//JAVA TO C# CONVERTER WARNING: The .NET Type.FullName property will not always yield results identical to the Java Class.getName method:
			return this.GetType().FullName + "[selectedCheckbox=" + SelectedCheckbox_Renamed + "]";
		}

	}

}
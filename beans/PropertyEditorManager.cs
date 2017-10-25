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
	/// The PropertyEditorManager can be used to locate a property editor for
	/// any given type name.  This property editor must support the
	/// java.beans.PropertyEditor interface for editing a given object.
	/// <P>
	/// The PropertyEditorManager uses three techniques for locating an editor
	/// for a given type.  First, it provides a registerEditor method to allow
	/// an editor to be specifically registered for a given type.  Second it
	/// tries to locate a suitable class by adding "Editor" to the full
	/// qualified classname of the given type (e.g. "foo.bah.FozEditor").
	/// Finally it takes the simple classname (without the package name) adds
	/// "Editor" to it and looks in a search-path of packages for a matching
	/// class.
	/// <P>
	/// So for an input class foo.bah.Fred, the PropertyEditorManager would
	/// first look in its tables to see if an editor had been registered for
	/// foo.bah.Fred and if so use that.  Then it will look for a
	/// foo.bah.FredEditor class.  Then it will look for (say)
	/// standardEditorsPackage.FredEditor class.
	/// <para>
	/// Default PropertyEditors will be provided for the Java primitive types
	/// "boolean", "byte", "short", "int", "long", "float", and "double"; and
	/// for the classes java.lang.String. java.awt.Color, and java.awt.Font.
	/// </para>
	/// </summary>

	public class PropertyEditorManager
	{

		/// <summary>
		/// Registers an editor class to edit values of the given target class.
		/// If the editor class is {@code null},
		/// then any existing definition will be removed.
		/// Thus this method can be used to cancel the registration.
		/// The registration is canceled automatically
		/// if either the target or editor class is unloaded.
		/// <para>
		/// If there is a security manager, its {@code checkPropertiesAccess}
		/// method is called. This could result in a <seealso cref="SecurityException"/>.
		/// 
		/// </para>
		/// </summary>
		/// <param name="targetType">   the class object of the type to be edited </param>
		/// <param name="editorClass">  the class object of the editor class </param>
		/// <exception cref="SecurityException">  if a security manager exists and
		///                            its {@code checkPropertiesAccess} method
		///                            doesn't allow setting of system properties
		/// </exception>
		/// <seealso cref= SecurityManager#checkPropertiesAccess </seealso>
		public static void RegisterEditor(Class targetType, Class editorClass)
		{
			SecurityManager sm = System.SecurityManager;
			if (sm != null)
			{
				sm.CheckPropertiesAccess();
			}
			ThreadGroupContext.Context.PropertyEditorFinder.register(targetType, editorClass);
		}

		/// <summary>
		/// Locate a value editor for a given target type.
		/// </summary>
		/// <param name="targetType">  The Class object for the type to be edited </param>
		/// <returns> An editor object for the given target class.
		/// The result is null if no suitable editor can be found. </returns>
		public static PropertyEditor FindEditor(Class targetType)
		{
			return ThreadGroupContext.Context.PropertyEditorFinder.find(targetType);
		}

		/// <summary>
		/// Gets the package names that will be searched for property editors.
		/// </summary>
		/// <returns>  The array of package names that will be searched in
		///          order to find property editors.
		/// <para>     The default value for this array is implementation-dependent,
		///         e.g. Sun implementation initially sets to  {"sun.beans.editors"}. </returns>
		public static String[] EditorSearchPath
		{
			get
			{
				return ThreadGroupContext.Context.PropertyEditorFinder.Packages;
			}
			set
			{
				SecurityManager sm = System.SecurityManager;
				if (sm != null)
				{
					sm.CheckPropertiesAccess();
				}
				ThreadGroupContext.Context.PropertyEditorFinder.Packages = value;
			}
		}

	}

}
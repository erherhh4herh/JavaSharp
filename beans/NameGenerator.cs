using System.Collections.Generic;

/*
 * Copyright (c) 2000, 2011, Oracle and/or its affiliates. All rights reserved.
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
	/// A utility class which generates unique names for object instances.
	/// The name will be a concatenation of the unqualified class name
	/// and an instance number.
	/// <para>
	/// For example, if the first object instance javax.swing.JButton
	/// is passed into <code>instanceName</code> then the returned
	/// string identifier will be &quot;JButton0&quot;.
	/// 
	/// @author Philip Milne
	/// </para>
	/// </summary>
	internal class NameGenerator
	{

		private IDictionary<Object, String> ValueToName;
		private IDictionary<String, Integer> NameToCount;

		public NameGenerator()
		{
			ValueToName = new IdentityHashMap<>();
			NameToCount = new Dictionary<>();
		}

		/// <summary>
		/// Clears the name cache. Should be called to near the end of
		/// the encoding cycle.
		/// </summary>
		public virtual void Clear()
		{
			ValueToName.Clear();
			NameToCount.Clear();
		}

		/// <summary>
		/// Returns the root name of the class.
		/// </summary>
//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @SuppressWarnings("rawtypes") public static String unqualifiedClassName(Class type)
		public static String UnqualifiedClassName(Class type)
		{
			if (type.Array)
			{
				return UnqualifiedClassName(type.ComponentType) + "Array";
			}
			String name = type.Name;
			return name.Substring(name.LastIndexOf('.') + 1);
		}

		/// <summary>
		/// Returns a String which capitalizes the first letter of the string.
		/// </summary>
		public static String Capitalize(String name)
		{
			if (name == null || name.Length() == 0)
			{
				return name;
			}
			return name.Substring(0, 1).ToUpperCase(ENGLISH) + name.Substring(1);
		}

		/// <summary>
		/// Returns a unique string which identifies the object instance.
		/// Invocations are cached so that if an object has been previously
		/// passed into this method then the same identifier is returned.
		/// </summary>
		/// <param name="instance"> object used to generate string </param>
		/// <returns> a unique string representing the object </returns>
		public virtual String InstanceName(Object instance)
		{
			if (instance == null)
			{
				return "null";
			}
			if (instance is Class)
			{
				return UnqualifiedClassName((Class)instance);
			}
			else
			{
				String result = ValueToName[instance];
				if (result != null)
				{
					return result;
				}
				Class type = instance.GetType();
				String className = UnqualifiedClassName(type);

				Integer size = NameToCount[className];
				int instanceNumber = (size == null) ? 0 : (size).IntValue() + 1;
				NameToCount[className] = new Integer(instanceNumber);

				result = className + instanceNumber;
				ValueToName[instance] = result;
				return result;
			}
		}
	}

}
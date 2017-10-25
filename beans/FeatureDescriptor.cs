using System.Collections.Generic;

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

	using TypeResolver = com.sun.beans.TypeResolver;



	/// <summary>
	/// The FeatureDescriptor class is the common baseclass for PropertyDescriptor,
	/// EventSetDescriptor, and MethodDescriptor, etc.
	/// <para>
	/// It supports some common information that can be set and retrieved for
	/// any of the introspection descriptors.
	/// </para>
	/// <para>
	/// In addition it provides an extension mechanism so that arbitrary
	/// attribute/value pairs can be associated with a design feature.
	/// </para>
	/// </summary>

	public class FeatureDescriptor
	{
		private const String TRANSIENT = "transient";

//JAVA TO C# CONVERTER TODO TASK: Java wildcard generics are not converted to .NET:
//ORIGINAL LINE: private Reference<? extends Class> classRef;
		private Reference<?> ClassRef;

		/// <summary>
		/// Constructs a <code>FeatureDescriptor</code>.
		/// </summary>
		public FeatureDescriptor()
		{
		}

		/// <summary>
		/// Gets the programmatic name of this feature.
		/// </summary>
		/// <returns> The programmatic name of the property/method/event </returns>
		public virtual String Name
		{
			get
			{
				return Name_Renamed;
			}
			set
			{
				this.Name_Renamed = value;
			}
		}


		/// <summary>
		/// Gets the localized display name of this feature.
		/// </summary>
		/// <returns> The localized display name for the property/method/event.
		///  This defaults to the same as its programmatic name from getName. </returns>
		public virtual String DisplayName
		{
			get
			{
				if (DisplayName_Renamed == null)
				{
					return Name;
				}
				return DisplayName_Renamed;
			}
			set
			{
				this.DisplayName_Renamed = value;
			}
		}


		/// <summary>
		/// The "expert" flag is used to distinguish between those features that are
		/// intended for expert users from those that are intended for normal users.
		/// </summary>
		/// <returns> True if this feature is intended for use by experts only. </returns>
		public virtual bool Expert
		{
			get
			{
				return Expert_Renamed;
			}
			set
			{
				this.Expert_Renamed = value;
			}
		}


		/// <summary>
		/// The "hidden" flag is used to identify features that are intended only
		/// for tool use, and which should not be exposed to humans.
		/// </summary>
		/// <returns> True if this feature should be hidden from human users. </returns>
		public virtual bool Hidden
		{
			get
			{
				return Hidden_Renamed;
			}
			set
			{
				this.Hidden_Renamed = value;
			}
		}


		/// <summary>
		/// The "preferred" flag is used to identify features that are particularly
		/// important for presenting to humans.
		/// </summary>
		/// <returns> True if this feature should be preferentially shown to human users. </returns>
		public virtual bool Preferred
		{
			get
			{
				return Preferred_Renamed;
			}
			set
			{
				this.Preferred_Renamed = value;
			}
		}


		/// <summary>
		/// Gets the short description of this feature.
		/// </summary>
		/// <returns>  A localized short description associated with this
		///   property/method/event.  This defaults to be the display name. </returns>
		public virtual String ShortDescription
		{
			get
			{
				if (ShortDescription_Renamed == null)
				{
					return DisplayName;
				}
				return ShortDescription_Renamed;
			}
			set
			{
				ShortDescription_Renamed = value;
			}
		}


		/// <summary>
		/// Associate a named attribute with this feature.
		/// </summary>
		/// <param name="attributeName">  The locale-independent name of the attribute </param>
		/// <param name="value">  The value. </param>
		public virtual void SetValue(String attributeName, Object value)
		{
			Table[attributeName] = value;
		}

		/// <summary>
		/// Retrieve a named attribute with this feature.
		/// </summary>
		/// <param name="attributeName">  The locale-independent name of the attribute </param>
		/// <returns>  The value of the attribute.  May be null if
		///     the attribute is unknown. </returns>
		public virtual Object GetValue(String attributeName)
		{
			return (this.Table_Renamed != null) ? this.Table_Renamed[attributeName] : null;
		}

		/// <summary>
		/// Gets an enumeration of the locale-independent names of this
		/// feature.
		/// </summary>
		/// <returns>  An enumeration of the locale-independent names of any
		///    attributes that have been registered with setValue. </returns>
		public virtual IEnumerator<String> AttributeNames()
		{
			return Table.Keys.GetEnumerator();
		}

		/// <summary>
		/// Package-private constructor,
		/// Merge information from two FeatureDescriptors.
		/// The merged hidden and expert flags are formed by or-ing the values.
		/// In the event of other conflicts, the second argument (y) is
		/// given priority over the first argument (x).
		/// </summary>
		/// <param name="x">  The first (lower priority) MethodDescriptor </param>
		/// <param name="y">  The second (higher priority) MethodDescriptor </param>
		internal FeatureDescriptor(FeatureDescriptor x, FeatureDescriptor y)
		{
			Expert_Renamed = x.Expert_Renamed | y.Expert_Renamed;
			Hidden_Renamed = x.Hidden_Renamed | y.Hidden_Renamed;
			Preferred_Renamed = x.Preferred_Renamed | y.Preferred_Renamed;
			Name_Renamed = y.Name_Renamed;
			ShortDescription_Renamed = x.ShortDescription_Renamed;
			if (y.ShortDescription_Renamed != null)
			{
				ShortDescription_Renamed = y.ShortDescription_Renamed;
			}
			DisplayName_Renamed = x.DisplayName_Renamed;
			if (y.DisplayName_Renamed != null)
			{
				DisplayName_Renamed = y.DisplayName_Renamed;
			}
			ClassRef = x.ClassRef;
			if (y.ClassRef != null)
			{
				ClassRef = y.ClassRef;
			}
			AddTable(x.Table_Renamed);
			AddTable(y.Table_Renamed);
		}

		/*
		 * Package-private dup constructor
		 * This must isolate the new object from any changes to the old object.
		 */
		internal FeatureDescriptor(FeatureDescriptor old)
		{
			Expert_Renamed = old.Expert_Renamed;
			Hidden_Renamed = old.Hidden_Renamed;
			Preferred_Renamed = old.Preferred_Renamed;
			Name_Renamed = old.Name_Renamed;
			ShortDescription_Renamed = old.ShortDescription_Renamed;
			DisplayName_Renamed = old.DisplayName_Renamed;
			ClassRef = old.ClassRef;

			AddTable(old.Table_Renamed);
		}

		/// <summary>
		/// Copies all values from the specified attribute table.
		/// If some attribute is exist its value should be overridden.
		/// </summary>
		/// <param name="table">  the attribute table with new values </param>
		private void AddTable(Dictionary<String, Object> table)
		{
			if ((table != null) && table.Count > 0)
			{
//JAVA TO C# CONVERTER TODO TASK: There is no .NET Dictionary equivalent to the Java 'putAll' method:
				Table.putAll(table);
			}
		}

		/// <summary>
		/// Returns the initialized attribute table.
		/// </summary>
		/// <returns> the initialized attribute table </returns>
		private Dictionary<String, Object> Table
		{
			get
			{
				if (this.Table_Renamed == null)
				{
					this.Table_Renamed = new Dictionary<>();
				}
				return this.Table_Renamed;
			}
		}

		/// <summary>
		/// Sets the "transient" attribute according to the annotation.
		/// If the "transient" attribute is already set
		/// it should not be changed.
		/// </summary>
		/// <param name="annotation">  the annotation of the element of the feature </param>
		internal virtual void SetTransient(Transient annotation)
		{
			if ((annotation != null) && (null == GetValue(TRANSIENT)))
			{
				SetValue(TRANSIENT, annotation.value());
			}
		}

		/// <summary>
		/// Indicates whether the feature is transient.
		/// </summary>
		/// <returns> {@code true} if the feature is transient,
		///         {@code false} otherwise </returns>
		internal virtual bool IsTransient()
		{
			Object value = GetValue(TRANSIENT);
			return (value is Boolean) ? (Boolean) value : false;
		}

		// Package private methods for recreating the weak/soft referent

		internal virtual Class Class0
		{
			set
			{
				this.ClassRef = GetWeakReference(value);
			}
			get
			{
				return (this.ClassRef != null) ? this.ClassRef.get() : null;
			}
		}


		/// <summary>
		/// Creates a new soft reference that refers to the given object.
		/// </summary>
		/// <returns> a new soft reference or <code>null</code> if object is <code>null</code>
		/// </returns>
		/// <seealso cref= SoftReference </seealso>
		internal static Reference<T> getSoftReference<T>(T @object)
		{
			return (@object != null) ? new SoftReference<>(@object) : null;
		}

		/// <summary>
		/// Creates a new weak reference that refers to the given object.
		/// </summary>
		/// <returns> a new weak reference or <code>null</code> if object is <code>null</code>
		/// </returns>
		/// <seealso cref= WeakReference </seealso>
		internal static Reference<T> getWeakReference<T>(T @object)
		{
			return (@object != null) ? new WeakReference<>(@object) : null;
		}

		/// <summary>
		/// Resolves the return type of the method.
		/// </summary>
		/// <param name="base">    the class that contains the method in the hierarchy </param>
		/// <param name="method">  the object that represents the method </param>
		/// <returns> a class identifying the return type of the method
		/// </returns>
		/// <seealso cref= Method#getGenericReturnType </seealso>
		/// <seealso cref= Method#getReturnType </seealso>
		internal static Class GetReturnType(Class @base, Method method)
		{
			if (@base == null)
			{
				@base = method.DeclaringClass;
			}
			return TypeResolver.erase(TypeResolver.resolveInClass(@base, method.GenericReturnType));
		}

		/// <summary>
		/// Resolves the parameter types of the method.
		/// </summary>
		/// <param name="base">    the class that contains the method in the hierarchy </param>
		/// <param name="method">  the object that represents the method </param>
		/// <returns> an array of classes identifying the parameter types of the method
		/// </returns>
		/// <seealso cref= Method#getGenericParameterTypes </seealso>
		/// <seealso cref= Method#getParameterTypes </seealso>
		internal static Class[] GetParameterTypes(Class @base, Method method)
		{
			if (@base == null)
			{
				@base = method.DeclaringClass;
			}
			return TypeResolver.erase(TypeResolver.resolveInClass(@base, method.GenericParameterTypes));
		}

		private bool Expert_Renamed;
		private bool Hidden_Renamed;
		private bool Preferred_Renamed;
		private String ShortDescription_Renamed;
		private String Name_Renamed;
		private String DisplayName_Renamed;
		private Dictionary<String, Object> Table_Renamed;

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
			sb.Append("[name=").Append(this.Name_Renamed);
			AppendTo(sb, "displayName", this.DisplayName_Renamed);
			AppendTo(sb, "shortDescription", this.ShortDescription_Renamed);
			AppendTo(sb, "preferred", this.Preferred_Renamed);
			AppendTo(sb, "hidden", this.Hidden_Renamed);
			AppendTo(sb, "expert", this.Expert_Renamed);
			if ((this.Table_Renamed != null) && this.Table_Renamed.Count > 0)
			{
				sb.Append("; values={");
				foreach (Map_Entry<String, Object> entry in this.Table_Renamed)
				{
					sb.Append(entry.Key).Append("=").Append(entry.Value).Append("; ");
				}
				sb.Length = sb.Length() - 2;
				sb.Append("}");
			}
			AppendTo(sb);
			return sb.Append("]").ToString();
		}

		internal virtual void AppendTo(StringBuilder sb)
		{
		}

		internal static void appendTo<T1>(StringBuilder sb, String name, Reference<T1> reference)
		{
			if (reference != null)
			{
				AppendTo(sb, name, reference.get());
			}
		}

		internal static void AppendTo(StringBuilder sb, String name, Object value)
		{
			if (value != null)
			{
				sb.Append("; ").Append(name).Append("=").Append(value);
			}
		}

		internal static void AppendTo(StringBuilder sb, String name, bool value)
		{
			if (value)
			{
				sb.Append("; ").Append(name);
			}
		}
	}

}
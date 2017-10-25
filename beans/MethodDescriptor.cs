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
	/// A MethodDescriptor describes a particular method that a Java Bean
	/// supports for external access from other components.
	/// </summary>

	public class MethodDescriptor : FeatureDescriptor
	{

		private readonly MethodRef MethodRef = new MethodRef();

		private String[] ParamNames_Renamed;

		private IList<WeakReference<Class>> @params;

		private ParameterDescriptor[] ParameterDescriptors_Renamed;

		/// <summary>
		/// Constructs a <code>MethodDescriptor</code> from a
		/// <code>Method</code>.
		/// </summary>
		/// <param name="method">    The low-level method information. </param>
		public MethodDescriptor(Method method) : this(method, null)
		{
		}


		/// <summary>
		/// Constructs a <code>MethodDescriptor</code> from a
		/// <code>Method</code> providing descriptive information for each
		/// of the method's parameters.
		/// </summary>
		/// <param name="method">    The low-level method information. </param>
		/// <param name="parameterDescriptors">  Descriptive information for each of the
		///                          method's parameters. </param>
		public MethodDescriptor(Method method, ParameterDescriptor[] parameterDescriptors)
		{
			Name = method.Name;
			Method = method;
			this.ParameterDescriptors_Renamed = (parameterDescriptors != null) ? parameterDescriptors.clone() : null;
		}

		/// <summary>
		/// Gets the method that this MethodDescriptor encapsulates.
		/// </summary>
		/// <returns> The low-level description of the method </returns>
		public virtual Method Method
		{
			get
			{
				lock (this)
				{
					Method method = this.MethodRef.Get();
					if (method == null)
					{
						Class cls = Class0;
						String name = Name;
						if ((cls != null) && (name != null))
						{
							Class[] @params = Params;
							if (@params == null)
							{
								for (int i = 0; i < 3; i++)
								{
									// Find methods for up to 2 params. We are guessing here.
									// This block should never execute unless the classloader
									// that loaded the argument classes disappears.
									method = Introspector.FindMethod(cls, name, i, null);
									if (method != null)
									{
										break;
									}
								}
							}
							else
							{
								method = Introspector.FindMethod(cls, name, @params.Length, @params);
							}
							Method = method;
						}
					}
					return method;
				}
			}
			set
			{
				lock (this)
				{
					if (value == null)
					{
						return;
					}
					if (Class0 == null)
					{
						Class0 = value.DeclaringClass;
					}
					Params = GetParameterTypes(Class0, value);
					this.MethodRef.Set(value);
				}
			}
		}


		private Class[] Params
		{
			set
			{
				lock (this)
				{
					if (value == null)
					{
						return;
					}
					ParamNames_Renamed = new String[value.Length];
					@params = new List<>(value.Length);
					for (int i = 0; i < value.Length; i++)
					{
						ParamNames_Renamed[i] = value[i].Name;
						@params.Add(new WeakReference<Class>(value[i]));
					}
				}
			}
			get
			{
				lock (this)
				{
					Class[] clss = new Class[@params.Count];
            
					for (int i = 0; i < @params.Count; i++)
					{
	//JAVA TO C# CONVERTER TODO TASK: Java wildcard generics are not converted to .NET:
	//ORIGINAL LINE: Reference<? extends Class> ref = (Reference<? extends Class>)params.get(i);
						Reference<?> @ref = (Reference<?>)@params[i];
						Class cls = @ref.get();
						if (cls == null)
						{
							return null;
						}
						else
						{
							clss[i] = cls;
						}
					}
					return clss;
				}
			}
		}

		// pp getParamNames used as an optimization to avoid method.getParameterTypes.
		internal virtual String[] ParamNames
		{
			get
			{
				return ParamNames_Renamed;
			}
		}


		/// <summary>
		/// Gets the ParameterDescriptor for each of this MethodDescriptor's
		/// method's parameters.
		/// </summary>
		/// <returns> The locale-independent names of the parameters.  May return
		///          a null array if the parameter names aren't known. </returns>
		public virtual ParameterDescriptor[] ParameterDescriptors
		{
			get
			{
				return (this.ParameterDescriptors_Renamed != null) ? this.ParameterDescriptors_Renamed.clone() : null;
			}
		}

		private static Method Resolve(Method oldMethod, Method newMethod)
		{
			if (oldMethod == null)
			{
				return newMethod;
			}
			if (newMethod == null)
			{
				return oldMethod;
			}
			return !oldMethod.Synthetic && newMethod.Synthetic ? oldMethod : newMethod;
		}

		/*
		 * Package-private constructor
		 * Merge two method descriptors.  Where they conflict, give the
		 * second argument (y) priority over the first argument (x).
		 * @param x  The first (lower priority) MethodDescriptor
		 * @param y  The second (higher priority) MethodDescriptor
		 */

		internal MethodDescriptor(MethodDescriptor x, MethodDescriptor y) : base(x, y)
		{

			this.MethodRef.Set(Resolve(x.MethodRef.Get(), y.MethodRef.Get()));
			@params = x.@params;
			if (y.@params != null)
			{
				@params = y.@params;
			}
			ParamNames_Renamed = x.ParamNames_Renamed;
			if (y.ParamNames_Renamed != null)
			{
				ParamNames_Renamed = y.ParamNames_Renamed;
			}

			ParameterDescriptors_Renamed = x.ParameterDescriptors_Renamed;
			if (y.ParameterDescriptors_Renamed != null)
			{
				ParameterDescriptors_Renamed = y.ParameterDescriptors_Renamed;
			}
		}

		/*
		 * Package-private dup constructor
		 * This must isolate the new object from any changes to the old object.
		 */
		internal MethodDescriptor(MethodDescriptor old) : base(old)
		{

			this.MethodRef.Set(old.Method);
			@params = old.@params;
			ParamNames_Renamed = old.ParamNames_Renamed;

			if (old.ParameterDescriptors_Renamed != null)
			{
				int len = old.ParameterDescriptors_Renamed.Length;
				ParameterDescriptors_Renamed = new ParameterDescriptor[len];
				for (int i = 0; i < len ; i++)
				{
					ParameterDescriptors_Renamed[i] = new ParameterDescriptor(old.ParameterDescriptors_Renamed[i]);
				}
			}
		}

		internal override void AppendTo(StringBuilder sb)
		{
			AppendTo(sb, "method", this.MethodRef.Get());
			if (this.ParameterDescriptors_Renamed != null)
			{
				sb.Append("; parameterDescriptors={");
				foreach (ParameterDescriptor pd in this.ParameterDescriptors_Renamed)
				{
					sb.Append(pd).Append(", ");
				}
				sb.Length = sb.Length() - 2;
				sb.Append("}");
			}
		}
	}

}
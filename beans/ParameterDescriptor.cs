/*
 * Copyright (c) 1996, 1997, Oracle and/or its affiliates. All rights reserved.
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
	/// The ParameterDescriptor class allows bean implementors to provide
	/// additional information on each of their parameters, beyond the
	/// low level type information provided by the java.lang.reflect.Method
	/// class.
	/// <para>
	/// Currently all our state comes from the FeatureDescriptor base class.
	/// </para>
	/// </summary>

	public class ParameterDescriptor : FeatureDescriptor
	{

		/// <summary>
		/// Public default constructor.
		/// </summary>
		public ParameterDescriptor()
		{
		}

		/// <summary>
		/// Package private dup constructor.
		/// This must isolate the new object from any changes to the old object.
		/// </summary>
		internal ParameterDescriptor(ParameterDescriptor old) : base(old)
		{
		}

	}

}
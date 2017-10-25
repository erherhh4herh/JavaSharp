/*
 * Copyright (c) 2015, Oracle and/or its affiliates. All rights reserved.
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

namespace java.lang.invoke
{

	/// <summary>
	/// Internal marker for some methods in the JSR 292 implementation.
	/// </summary>
	/*non-public*/
	[AttributeUsage(AttributeTargets.Method | AttributeTargets.Constructor, AllowMultiple = false, Inherited = false]
	internal class InjectedProfile : System.Attribute
	{
	}

}
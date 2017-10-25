/*
 * Copyright (c) 1998, Oracle and/or its affiliates. All rights reserved.
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
	/// Conditional is used by the EventDispatchThread's message pumps to
	/// determine if a given pump should continue to run, or should instead exit
	/// and yield control to the parent pump.
	/// 
	/// @author David Mendenhall
	/// </summary>
	internal interface Conditional
	{
		bool Evaluate();
	}

}
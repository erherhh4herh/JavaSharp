/*
 * Copyright (c) 1997, 2013, Oracle and/or its affiliates. All rights reserved.
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

namespace java.lang.@ref
{

	/// <summary>
	/// Final references, used to implement finalization
	/// </summary>
	internal class FinalReference<T> : Reference<T>
	{

//JAVA TO C# CONVERTER TODO TASK: There is no .NET equivalent to the Java 'super' constraint:
//ORIGINAL LINE: public FinalReference(T referent, ReferenceQueue<? base T> q)
		public FinalReference<T1>(T referent, ReferenceQueue<T1> q) : base(referent, q)
		{
		}
	}

}
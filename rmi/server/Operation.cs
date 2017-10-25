using System;

/*
 * Copyright (c) 1996, 2004, Oracle and/or its affiliates. All rights reserved.
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

namespace java.rmi.server
{

	/// <summary>
	/// An <code>Operation</code> contains a description of a Java method.
	/// <code>Operation</code> objects were used in JDK1.1 version stubs and
	/// skeletons. The <code>Operation</code> class is not needed for 1.2 style
	/// stubs (stubs generated with <code>rmic -v1.2</code>); hence, this class
	/// is deprecated.
	/// 
	/// @since JDK1.1 </summary>
	/// @deprecated no replacement 
	[Obsolete("no replacement")]
	public class Operation
	{
		private String Operation_Renamed;

		/// <summary>
		/// Creates a new Operation object. </summary>
		/// <param name="op"> method name </param>
		/// @deprecated no replacement
		/// @since JDK1.1 
		[Obsolete("no replacement")]
		public Operation(String op)
		{
			Operation_Renamed = op;
		}

		/// <summary>
		/// Returns the name of the method. </summary>
		/// <returns> method name </returns>
		/// @deprecated no replacement
		/// @since JDK1.1 
		[Obsolete("no replacement")]
		public virtual String Operation
		{
			get
			{
				return Operation_Renamed;
			}
		}

		/// <summary>
		/// Returns the string representation of the operation. </summary>
		/// @deprecated no replacement
		/// @since JDK1.1 
		[Obsolete("no replacement")]
		public override String ToString()
		{
			return Operation_Renamed;
		}
	}

}
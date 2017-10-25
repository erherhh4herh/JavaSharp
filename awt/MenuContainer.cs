using System;

/*
 * Copyright (c) 1995, 2004, Oracle and/or its affiliates. All rights reserved.
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
	/// The super class of all menu related containers.
	/// 
	/// @author      Arthur van Hoff
	/// </summary>

	public interface MenuContainer
	{
		Font Font {get;}
		void Remove(MenuComponent comp);

		/// @deprecated As of JDK version 1.1
		/// replaced by dispatchEvent(AWTEvent). 
		[Obsolete("As of JDK version 1.1")]
		bool PostEvent(Event evt);
	}

}
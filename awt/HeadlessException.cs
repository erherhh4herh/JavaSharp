/*
 * Copyright (c) 2000, 2004, Oracle and/or its affiliates. All rights reserved.
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
	/// Thrown when code that is dependent on a keyboard, display, or mouse
	/// is called in an environment that does not support a keyboard, display,
	/// or mouse.
	/// 
	/// @since 1.4
	/// @author  Michael Martak
	/// </summary>
	public class HeadlessException : UnsupportedOperationException
	{
		/*
		 * JDK 1.4 serialVersionUID
		 */
		private new const long SerialVersionUID = 167183644944358563L;
		public HeadlessException()
		{
		}
		public HeadlessException(String msg) : base(msg)
		{
		}
		public override String Message
		{
			get
			{
				String superMessage = base.Message;
				String headlessMessage = GraphicsEnvironment.HeadlessMessage;
    
				if (superMessage == null)
				{
					return headlessMessage;
				}
				else if (headlessMessage == null)
				{
					return superMessage;
				}
				else
				{
					return superMessage + headlessMessage;
				}
			}
		}
	}

}
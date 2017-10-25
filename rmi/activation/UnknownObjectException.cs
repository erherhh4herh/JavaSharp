/*
 * Copyright (c) 1997, 1999, Oracle and/or its affiliates. All rights reserved.
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

namespace java.rmi.activation
{

	/// <summary>
	/// An <code>UnknownObjectException</code> is thrown by methods of classes and
	/// interfaces in the <code>java.rmi.activation</code> package when the
	/// <code>ActivationID</code> parameter to the method is determined to be
	/// invalid.  An <code>ActivationID</code> is invalid if it is not currently
	/// known by the <code>ActivationSystem</code>.  An <code>ActivationID</code>
	/// is obtained by the <code>ActivationSystem.registerObject</code> method.
	/// An <code>ActivationID</code> is also obtained during the
	/// <code>Activatable.register</code> call.
	/// 
	/// @author  Ann Wollrath
	/// @since   1.2 </summary>
	/// <seealso cref=     java.rmi.activation.Activatable </seealso>
	/// <seealso cref=     java.rmi.activation.ActivationGroup </seealso>
	/// <seealso cref=     java.rmi.activation.ActivationID </seealso>
	/// <seealso cref=     java.rmi.activation.ActivationMonitor </seealso>
	/// <seealso cref=     java.rmi.activation.ActivationSystem </seealso>
	/// <seealso cref=     java.rmi.activation.Activator </seealso>
	public class UnknownObjectException : ActivationException
	{

		/// <summary>
		/// indicate compatibility with the Java 2 SDK v1.2 version of class </summary>
		private new const long SerialVersionUID = 3425547551622251430L;

		/// <summary>
		/// Constructs an <code>UnknownObjectException</code> with the specified
		/// detail message.
		/// </summary>
		/// <param name="s"> the detail message
		/// @since 1.2 </param>
		public UnknownObjectException(String s) : base(s)
		{
		}
	}

}
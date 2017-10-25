using System;

/*
 * Copyright (c) 1996, 2009, Oracle and/or its affiliates. All rights reserved.
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
	/// A PropertyVetoException is thrown when a proposed change to a
	/// property represents an unacceptable value.
	/// </summary>

	public class PropertyVetoException : Exception
	{
		private new const long SerialVersionUID = 129596057694162164L;

		/// <summary>
		/// Constructs a <code>PropertyVetoException</code> with a
		/// detailed message.
		/// </summary>
		/// <param name="mess"> Descriptive message </param>
		/// <param name="evt"> A PropertyChangeEvent describing the vetoed change. </param>
		public PropertyVetoException(String mess, PropertyChangeEvent evt) : base(mess)
		{
			this.Evt = evt;
		}

		 /// <summary>
		 /// Gets the vetoed <code>PropertyChangeEvent</code>.
		 /// </summary>
		 /// <returns> A PropertyChangeEvent describing the vetoed change. </returns>
		public virtual PropertyChangeEvent PropertyChangeEvent
		{
			get
			{
				return Evt;
			}
		}

		/// <summary>
		/// A PropertyChangeEvent describing the vetoed change.
		/// @serial
		/// </summary>
		private PropertyChangeEvent Evt;
	}

}
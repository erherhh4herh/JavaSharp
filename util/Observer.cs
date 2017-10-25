/*
 * Copyright (c) 1994, 1998, Oracle and/or its affiliates. All rights reserved.
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
namespace java.util
{

	/// <summary>
	/// A class can implement the <code>Observer</code> interface when it
	/// wants to be informed of changes in observable objects.
	/// 
	/// @author  Chris Warth </summary>
	/// <seealso cref=     java.util.Observable
	/// @since   JDK1.0 </seealso>
	public interface Observer
	{
		/// <summary>
		/// This method is called whenever the observed object is changed. An
		/// application calls an <tt>Observable</tt> object's
		/// <code>notifyObservers</code> method to have all the object's
		/// observers notified of the change.
		/// </summary>
		/// <param name="o">     the observable object. </param>
		/// <param name="arg">   an argument passed to the <code>notifyObservers</code>
		///                 method. </param>
		void Update(Observable o, Object arg);
	}

}
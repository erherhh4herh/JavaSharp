using System;

/*
 * Copyright (c) 1996, 2008, Oracle and/or its affiliates. All rights reserved.
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
	/// <para>
	/// The <code> TooManyListenersException </code> Exception is used as part of
	/// the Java Event model to annotate and implement a unicast special case of
	/// a multicast Event Source.
	/// </para>
	/// <para>
	/// The presence of a "throws TooManyListenersException" clause on any given
	/// concrete implementation of the normally multicast "void addXyzEventListener"
	/// event listener registration pattern is used to annotate that interface as
	/// implementing a unicast Listener special case, that is, that one and only
	/// one Listener may be registered on the particular event listener source
	/// concurrently.
	/// </para>
	/// </summary>
	/// <seealso cref= java.util.EventObject </seealso>
	/// <seealso cref= java.util.EventListener
	/// 
	/// @author Laurence P. G. Cable
	/// @since  JDK1.1 </seealso>

	public class TooManyListenersException : Exception
	{
		private new const long SerialVersionUID = 5074640544770687831L;

		/// <summary>
		/// Constructs a TooManyListenersException with no detail message.
		/// A detail message is a String that describes this particular exception.
		/// </summary>

		public TooManyListenersException() : base()
		{
		}

		/// <summary>
		/// Constructs a TooManyListenersException with the specified detail message.
		/// A detail message is a String that describes this particular exception. </summary>
		/// <param name="s"> the detail message </param>

		public TooManyListenersException(String s) : base(s)
		{
		}
	}

}
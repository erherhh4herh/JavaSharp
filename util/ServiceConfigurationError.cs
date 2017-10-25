/*
 * Copyright (c) 2005, 2006, Oracle and/or its affiliates. All rights reserved.
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
	/// Error thrown when something goes wrong while loading a service provider.
	/// 
	/// <para> This error will be thrown in the following situations:
	/// 
	/// <ul>
	/// 
	///   <li> The format of a provider-configuration file violates the <a
	///   href="ServiceLoader.html#format">specification</a>; </li>
	/// 
	///   <li> An <seealso cref="java.io.IOException IOException"/> occurs while reading a
	///   provider-configuration file; </li>
	/// 
	///   <li> A concrete provider class named in a provider-configuration file
	///   cannot be found; </li>
	/// 
	///   <li> A concrete provider class is not a subclass of the service class;
	///   </li>
	/// 
	///   <li> A concrete provider class cannot be instantiated; or
	/// 
	///   <li> Some other kind of error occurs. </li>
	/// 
	/// </ul>
	/// 
	/// 
	/// @author Mark Reinhold
	/// @since 1.6
	/// </para>
	/// </summary>

	public class ServiceConfigurationError : Error
	{

		private new const long SerialVersionUID = 74132770414881L;

		/// <summary>
		/// Constructs a new instance with the specified message.
		/// </summary>
		/// <param name="msg">  The message, or <tt>null</tt> if there is no message
		///  </param>
		public ServiceConfigurationError(String msg) : base(msg)
		{
		}

		/// <summary>
		/// Constructs a new instance with the specified message and cause.
		/// </summary>
		/// <param name="msg">  The message, or <tt>null</tt> if there is no message
		/// </param>
		/// <param name="cause">  The cause, or <tt>null</tt> if the cause is nonexistent
		///                or unknown </param>
		public ServiceConfigurationError(String msg, Throwable cause) : base(msg, cause)
		{
		}

	}

}
﻿/*
 * Copyright (c) 2005, 2013, Oracle and/or its affiliates. All rights reserved.
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

namespace java.sql
{

	/// <summary>
	/// The subclass of <seealso cref="SQLException"/> thrown when the SQLState class value
	/// is '<i>28</i>', or under vendor-specified conditions. This indicates that
	/// the authorization credentials presented during connection establishment
	/// are not valid.
	/// <para>
	/// Please consult your driver vendor documentation for the vendor-specified
	/// conditions for which this <code>Exception</code> may be thrown.
	/// @since 1.6
	/// </para>
	/// </summary>
	public class SQLInvalidAuthorizationSpecException : SQLNonTransientException
	{

			/// <summary>
			/// Constructs a <code>SQLInvalidAuthorizationSpecException</code> object.
			/// The <code>reason</code>, <code>SQLState</code> are initialized
			/// to <code>null</code> and the vendor code is initialized to 0.
			/// 
			/// The <code>cause</code> is not initialized, and may subsequently be
			/// initialized by a call to the
			/// <seealso cref="Throwable#initCause(java.lang.Throwable)"/> method.
			/// <para>
			/// @since 1.6
			/// </para>
			/// </summary>
			public SQLInvalidAuthorizationSpecException() : base()
			{
			}

			/// <summary>
			/// Constructs a <code>SQLInvalidAuthorizationSpecException</code> object
			///  with a given <code>reason</code>. The <code>SQLState</code>
			/// is initialized to <code>null</code> and the vendor code is initialized
			/// to 0.
			/// 
			/// The <code>cause</code> is not initialized, and may subsequently be
			/// initialized by a call to the
			/// <seealso cref="Throwable#initCause(java.lang.Throwable)"/> method.
			/// <para>
			/// </para>
			/// </summary>
			/// <param name="reason"> a description of the exception
			/// @since 1.6 </param>
			public SQLInvalidAuthorizationSpecException(String reason) : base(reason)
			{
			}

			/// <summary>
			/// Constructs a <code>SQLInvalidAuthorizationSpecException</code> object
			/// with a given <code>reason</code> and <code>SQLState</code>.
			/// 
			/// The <code>cause</code> is not initialized, and may subsequently be
			/// initialized by a call to the
			/// <seealso cref="Throwable#initCause(java.lang.Throwable)"/> method. The vendor code
			/// is initialized to 0.
			/// <para>
			/// </para>
			/// </summary>
			/// <param name="reason"> a description of the exception </param>
			/// <param name="SQLState"> an XOPEN or SQL:2003 code identifying the exception
			/// @since 1.6 </param>
			public SQLInvalidAuthorizationSpecException(String reason, String SQLState) : base(reason,SQLState)
			{
			}

			/// <summary>
			/// Constructs a <code>SQLInvalidAuthorizationSpecException</code> object
			/// with a given <code>reason</code>, <code>SQLState</code>  and
			/// <code>vendorCode</code>.
			/// 
			/// The <code>cause</code> is not initialized, and may subsequently be
			/// initialized by a call to the
			/// <seealso cref="Throwable#initCause(java.lang.Throwable)"/> method.
			/// <para>
			/// </para>
			/// </summary>
			/// <param name="reason"> a description of the exception </param>
			/// <param name="SQLState"> an XOPEN or SQL:2003 code identifying the exception </param>
			/// <param name="vendorCode"> a database vendor specific exception code
			/// @since 1.6 </param>
			public SQLInvalidAuthorizationSpecException(String reason, String SQLState, int vendorCode) : base(reason,SQLState,vendorCode)
			{
			}

			/// <summary>
			/// Constructs a <code>SQLInvalidAuthorizationSpecException</code> object
			/// with a given  <code>cause</code>.
			/// The <code>SQLState</code> is initialized
			/// to <code>null</code> and the vendor code is initialized to 0.
			/// The <code>reason</code>  is initialized to <code>null</code> if
			/// <code>cause==null</code> or to <code>cause.toString()</code> if
			/// <code>cause!=null</code>.
			/// <para>
			/// </para>
			/// </summary>
			/// <param name="cause"> the underlying reason for this <code>SQLException</code> (which is saved for later retrieval by the <code>getCause()</code> method); may be null indicating
			///     the cause is non-existent or unknown.
			/// @since 1.6 </param>
		public SQLInvalidAuthorizationSpecException(Throwable cause) : base(cause)
		{
		}

		/// <summary>
		/// Constructs a <code>SQLInvalidAuthorizationSpecException</code> object
		///  with a given
		/// <code>reason</code> and  <code>cause</code>.
		/// The <code>SQLState</code> is  initialized to <code>null</code>
		/// and the vendor code is initialized to 0.
		/// <para>
		/// </para>
		/// </summary>
		/// <param name="reason"> a description of the exception. </param>
		/// <param name="cause"> the underlying reason for this <code>SQLException</code> (which is saved for later retrieval by the <code>getCause()</code> method); may be null indicating
		///     the cause is non-existent or unknown.
		/// @since 1.6 </param>
		public SQLInvalidAuthorizationSpecException(String reason, Throwable cause) : base(reason,cause)
		{
		}

		/// <summary>
		/// Constructs a <code>SQLInvalidAuthorizationSpecException</code> object
		///  with a given
		/// <code>reason</code>, <code>SQLState</code> and  <code>cause</code>.
		/// The vendor code is initialized to 0.
		/// <para>
		/// </para>
		/// </summary>
		/// <param name="reason"> a description of the exception. </param>
		/// <param name="SQLState"> an XOPEN or SQL:2003 code identifying the exception </param>
		/// <param name="cause"> the underlying reason for this <code>SQLException</code> (which is saved for later retrieval by the <code>getCause()</code>  method); may be null indicating
		///     the cause is non-existent or unknown.
		/// @since 1.6 </param>
		public SQLInvalidAuthorizationSpecException(String reason, String SQLState, Throwable cause) : base(reason,SQLState,cause)
		{
		}

		/// <summary>
		///  Constructs a <code>SQLInvalidAuthorizationSpecException</code> object
		/// with a given
		/// <code>reason</code>, <code>SQLState</code>, <code>vendorCode</code>
		/// and  <code>cause</code>.
		/// <para>
		/// </para>
		/// </summary>
		/// <param name="reason"> a description of the exception </param>
		/// <param name="SQLState"> an XOPEN or SQL:2003 code identifying the exception </param>
		/// <param name="vendorCode"> a database vendor-specific exception code </param>
		/// <param name="cause"> the underlying reason for this <code>SQLException</code> (which is saved for later retrieval by the <code>getCause()</code> method); may be null indicating
		///     the cause is non-existent or unknown.
		/// @since 1.6 </param>
		public SQLInvalidAuthorizationSpecException(String reason, String SQLState, int vendorCode, Throwable cause) : base(reason,SQLState,vendorCode,cause)
		{
		}

	   private new const long SerialVersionUID = -64105250450891498L;
	}

}
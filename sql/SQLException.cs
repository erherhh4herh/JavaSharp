using System;
using System.Collections.Generic;

/*
 * Copyright (c) 1996, 2013, Oracle and/or its affiliates. All rights reserved.
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
	/// <P>An exception that provides information on a database access
	/// error or other errors.
	/// 
	/// <P>Each <code>SQLException</code> provides several kinds of information:
	/// <UL>
	///   <LI> a string describing the error.  This is used as the Java Exception
	///       message, available via the method <code>getMesasge</code>.
	///   <LI> a "SQLstate" string, which follows either the XOPEN SQLstate conventions
	///        or the SQL:2003 conventions.
	///       The values of the SQLState string are described in the appropriate spec.
	///       The <code>DatabaseMetaData</code> method <code>getSQLStateType</code>
	///       can be used to discover whether the driver returns the XOPEN type or
	///       the SQL:2003 type.
	///   <LI> an integer error code that is specific to each vendor.  Normally this will
	///       be the actual error code returned by the underlying database.
	///   <LI> a chain to a next Exception.  This can be used to provide additional
	///       error information.
	///   <LI> the causal relationship, if any for this <code>SQLException</code>.
	/// </UL>
	/// </summary>
	public class SQLException : Exception, Iterable<Throwable>
	{

		/// <summary>
		///  Constructs a <code>SQLException</code> object with a given
		/// <code>reason</code>, <code>SQLState</code>  and
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
		/// <param name="vendorCode"> a database vendor-specific exception code </param>
		public SQLException(String reason, String SQLState, int vendorCode) : base(reason)
		{
			this.SQLState_Renamed = SQLState;
			this.VendorCode = vendorCode;
			if (!(this is SQLWarning))
			{
				if (DriverManager.LogWriter != null)
				{
					DriverManager.Println("SQLState(" + SQLState + ") vendor code(" + vendorCode + ")");
					PrintStackTrace(DriverManager.LogWriter);
				}
			}
		}


		/// <summary>
		/// Constructs a <code>SQLException</code> object with a given
		/// <code>reason</code> and <code>SQLState</code>.
		/// 
		/// The <code>cause</code> is not initialized, and may subsequently be
		/// initialized by a call to the
		/// <seealso cref="Throwable#initCause(java.lang.Throwable)"/> method. The vendor code
		/// is initialized to 0.
		/// <para>
		/// </para>
		/// </summary>
		/// <param name="reason"> a description of the exception </param>
		/// <param name="SQLState"> an XOPEN or SQL:2003 code identifying the exception </param>
		public SQLException(String reason, String SQLState) : base(reason)
		{
			this.SQLState_Renamed = SQLState;
			this.VendorCode = 0;
			if (!(this is SQLWarning))
			{
				if (DriverManager.LogWriter != null)
				{
					PrintStackTrace(DriverManager.LogWriter);
					DriverManager.Println("SQLException: SQLState(" + SQLState + ")");
				}
			}
		}

		/// <summary>
		///  Constructs a <code>SQLException</code> object with a given
		/// <code>reason</code>. The  <code>SQLState</code>  is initialized to
		/// <code>null</code> and the vendor code is initialized to 0.
		/// 
		/// The <code>cause</code> is not initialized, and may subsequently be
		/// initialized by a call to the
		/// <seealso cref="Throwable#initCause(java.lang.Throwable)"/> method.
		/// <para>
		/// </para>
		/// </summary>
		/// <param name="reason"> a description of the exception </param>
		public SQLException(String reason) : base(reason)
		{
			this.SQLState_Renamed = null;
			this.VendorCode = 0;
			if (!(this is SQLWarning))
			{
				if (DriverManager.LogWriter != null)
				{
					PrintStackTrace(DriverManager.LogWriter);
				}
			}
		}

		/// <summary>
		/// Constructs a <code>SQLException</code> object.
		/// The <code>reason</code>, <code>SQLState</code> are initialized
		/// to <code>null</code> and the vendor code is initialized to 0.
		/// 
		/// The <code>cause</code> is not initialized, and may subsequently be
		/// initialized by a call to the
		/// <seealso cref="Throwable#initCause(java.lang.Throwable)"/> method.
		/// 
		/// </summary>
		public SQLException() : base()
		{
			this.SQLState_Renamed = null;
			this.VendorCode = 0;
			if (!(this is SQLWarning))
			{
				if (DriverManager.LogWriter != null)
				{
					PrintStackTrace(DriverManager.LogWriter);
				}
			}
		}

		/// <summary>
		///  Constructs a <code>SQLException</code> object with a given
		/// <code>cause</code>.
		/// The <code>SQLState</code> is initialized
		/// to <code>null</code> and the vendor code is initialized to 0.
		/// The <code>reason</code>  is initialized to <code>null</code> if
		/// <code>cause==null</code> or to <code>cause.toString()</code> if
		/// <code>cause!=null</code>.
		/// <para>
		/// </para>
		/// </summary>
		/// <param name="cause"> the underlying reason for this <code>SQLException</code>
		/// (which is saved for later retrieval by the <code>getCause()</code> method);
		/// may be null indicating the cause is non-existent or unknown.
		/// @since 1.6 </param>
		public SQLException(Throwable cause) : base(cause)
		{

			if (!(this is SQLWarning))
			{
				if (DriverManager.LogWriter != null)
				{
					PrintStackTrace(DriverManager.LogWriter);
				}
			}
		}

		/// <summary>
		/// Constructs a <code>SQLException</code> object with a given
		/// <code>reason</code> and  <code>cause</code>.
		/// The <code>SQLState</code> is  initialized to <code>null</code>
		/// and the vendor code is initialized to 0.
		/// <para>
		/// </para>
		/// </summary>
		/// <param name="reason"> a description of the exception. </param>
		/// <param name="cause"> the underlying reason for this <code>SQLException</code>
		/// (which is saved for later retrieval by the <code>getCause()</code> method);
		/// may be null indicating the cause is non-existent or unknown.
		/// @since 1.6 </param>
		public SQLException(String reason, Throwable cause) : base(reason,cause)
		{

			if (!(this is SQLWarning))
			{
				if (DriverManager.LogWriter != null)
				{
						PrintStackTrace(DriverManager.LogWriter);
				}
			}
		}

		/// <summary>
		/// Constructs a <code>SQLException</code> object with a given
		/// <code>reason</code>, <code>SQLState</code> and  <code>cause</code>.
		/// The vendor code is initialized to 0.
		/// <para>
		/// </para>
		/// </summary>
		/// <param name="reason"> a description of the exception. </param>
		/// <param name="sqlState"> an XOPEN or SQL:2003 code identifying the exception </param>
		/// <param name="cause"> the underlying reason for this <code>SQLException</code>
		/// (which is saved for later retrieval by the
		/// <code>getCause()</code> method); may be null indicating
		///     the cause is non-existent or unknown.
		/// @since 1.6 </param>
		public SQLException(String reason, String sqlState, Throwable cause) : base(reason,cause)
		{

			this.SQLState_Renamed = sqlState;
			this.VendorCode = 0;
			if (!(this is SQLWarning))
			{
				if (DriverManager.LogWriter != null)
				{
					PrintStackTrace(DriverManager.LogWriter);
					DriverManager.Println("SQLState(" + SQLState_Renamed + ")");
				}
			}
		}

		/// <summary>
		/// Constructs a <code>SQLException</code> object with a given
		/// <code>reason</code>, <code>SQLState</code>, <code>vendorCode</code>
		/// and  <code>cause</code>.
		/// <para>
		/// </para>
		/// </summary>
		/// <param name="reason"> a description of the exception </param>
		/// <param name="sqlState"> an XOPEN or SQL:2003 code identifying the exception </param>
		/// <param name="vendorCode"> a database vendor-specific exception code </param>
		/// <param name="cause"> the underlying reason for this <code>SQLException</code>
		/// (which is saved for later retrieval by the <code>getCause()</code> method);
		/// may be null indicating the cause is non-existent or unknown.
		/// @since 1.6 </param>
		public SQLException(String reason, String sqlState, int vendorCode, Throwable cause) : base(reason,cause)
		{

			this.SQLState_Renamed = sqlState;
			this.VendorCode = vendorCode;
			if (!(this is SQLWarning))
			{
				if (DriverManager.LogWriter != null)
				{
					DriverManager.Println("SQLState(" + SQLState_Renamed + ") vendor code(" + vendorCode + ")");
					PrintStackTrace(DriverManager.LogWriter);
				}
			}
		}

		/// <summary>
		/// Retrieves the SQLState for this <code>SQLException</code> object.
		/// </summary>
		/// <returns> the SQLState value </returns>
		public virtual String SQLState
		{
			get
			{
				return (SQLState_Renamed);
			}
		}

		/// <summary>
		/// Retrieves the vendor-specific exception code
		/// for this <code>SQLException</code> object.
		/// </summary>
		/// <returns> the vendor's error code </returns>
		public virtual int ErrorCode
		{
			get
			{
				return (VendorCode);
			}
		}

		/// <summary>
		/// Retrieves the exception chained to this
		/// <code>SQLException</code> object by setNextException(SQLException ex).
		/// </summary>
		/// <returns> the next <code>SQLException</code> object in the chain;
		///         <code>null</code> if there are none </returns>
		/// <seealso cref= #setNextException </seealso>
		public virtual SQLException NextException
		{
			get
			{
				return (Next);
			}
			set
			{
    
				SQLException current = this;
				for (;;)
				{
					SQLException next = current.Next;
					if (next != null)
					{
						current = next;
						continue;
					}
    
					if (NextUpdater.CompareAndSet(current,null,value))
					{
						return;
					}
					current = current.Next;
				}
			}
		}


		/// <summary>
		/// Returns an iterator over the chained SQLExceptions.  The iterator will
		/// be used to iterate over each SQLException and its underlying cause
		/// (if any).
		/// </summary>
		/// <returns> an iterator over the chained SQLExceptions and causes in the proper
		/// order
		/// 
		/// @since 1.6 </returns>
		public virtual IEnumerator<Throwable> GetEnumerator()
		{

		   return new IteratorAnonymousInnerClassHelper(this);

		}

		private class IteratorAnonymousInnerClassHelper : Iterator<Throwable>
		{
			private readonly SQLException OuterInstance;

			public IteratorAnonymousInnerClassHelper(SQLException outerInstance)
			{
				this.OuterInstance = outerInstance;
				firstException = outerInstance;
				nextException = firstException.NextException;
				cause = firstException.Cause;
			}


			internal SQLException firstException;
			internal SQLException nextException;
			internal Throwable cause;

			public virtual bool HasNext()
			{
				if (firstException != null || nextException != null || cause != null)
				{
					return true;
				}
				return false;
			}

			public virtual Throwable Next()
			{
				Throwable throwable = null;
				if (firstException != null)
				{
					throwable = firstException;
					firstException = null;
				}
				else if (cause != null)
				{
					throwable = cause;
					cause = cause.Cause;
				}
				else if (nextException != null)
				{
					throwable = nextException;
					cause = nextException.Cause;
					nextException = nextException.NextException;
				}
				else
				{
					throw new NoSuchElementException();
				}
				return throwable;
			}

			public virtual void Remove()
			{
				throw new UnsupportedOperationException();
			}

		}

		/// <summary>
		/// @serial
		/// </summary>
		private String SQLState_Renamed;

			/// <summary>
			/// @serial
			/// </summary>
		private int VendorCode;

			/// <summary>
			/// @serial
			/// </summary>
		private volatile SQLException Next;

		private static readonly AtomicReferenceFieldUpdater<SQLException, SQLException> NextUpdater = AtomicReferenceFieldUpdater.NewUpdater(typeof(SQLException),typeof(SQLException),"next");

		private new const long SerialVersionUID = 2135244094396331484L;
	}

}
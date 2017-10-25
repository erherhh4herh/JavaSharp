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
	/// An exception  thrown as a <code>DataTruncation</code> exception
	/// (on writes) or reported as a
	/// <code>DataTruncation</code> warning (on reads)
	///  when a data values is unexpectedly truncated for reasons other than its having
	///  exceeded <code>MaxFieldSize</code>.
	/// 
	/// <P>The SQLstate for a <code>DataTruncation</code> during read is <code>01004</code>.
	/// <P>The SQLstate for a <code>DataTruncation</code> during write is <code>22001</code>.
	/// </summary>

	public class DataTruncation : SQLWarning
	{

		/// <summary>
		/// Creates a <code>DataTruncation</code> object
		/// with the SQLState initialized
		/// to 01004 when <code>read</code> is set to <code>true</code> and 22001
		/// when <code>read</code> is set to <code>false</code>,
		/// the reason set to "Data truncation", the
		/// vendor code set to 0, and
		/// the other fields set to the given values.
		/// The <code>cause</code> is not initialized, and may subsequently be
		/// initialized by a call to the
		/// <seealso cref="Throwable#initCause(java.lang.Throwable)"/> method.
		/// <para>
		/// 
		/// </para>
		/// </summary>
		/// <param name="index"> The index of the parameter or column value </param>
		/// <param name="parameter"> true if a parameter value was truncated </param>
		/// <param name="read"> true if a read was truncated </param>
		/// <param name="dataSize"> the original size of the data </param>
		/// <param name="transferSize"> the size after truncation </param>
		public DataTruncation(int index, bool parameter, bool read, int dataSize, int transferSize) : base("Data truncation", read == true?"01004":"22001")
		{
			this.Index_Renamed = index;
			this.Parameter_Renamed = parameter;
			this.Read_Renamed = read;
			this.DataSize_Renamed = dataSize;
			this.TransferSize_Renamed = transferSize;

		}

		/// <summary>
		/// Creates a <code>DataTruncation</code> object
		/// with the SQLState initialized
		/// to 01004 when <code>read</code> is set to <code>true</code> and 22001
		/// when <code>read</code> is set to <code>false</code>,
		/// the reason set to "Data truncation", the
		/// vendor code set to 0, and
		/// the other fields set to the given values.
		/// <para>
		/// 
		/// </para>
		/// </summary>
		/// <param name="index"> The index of the parameter or column value </param>
		/// <param name="parameter"> true if a parameter value was truncated </param>
		/// <param name="read"> true if a read was truncated </param>
		/// <param name="dataSize"> the original size of the data </param>
		/// <param name="transferSize"> the size after truncation </param>
		/// <param name="cause"> the underlying reason for this <code>DataTruncation</code>
		/// (which is saved for later retrieval by the <code>getCause()</code> method);
		/// may be null indicating the cause is non-existent or unknown.
		/// 
		/// @since 1.6 </param>
		public DataTruncation(int index, bool parameter, bool read, int dataSize, int transferSize, Throwable cause) : base("Data truncation", read == true?"01004":"22001",cause)
		{
			this.Index_Renamed = index;
			this.Parameter_Renamed = parameter;
			this.Read_Renamed = read;
			this.DataSize_Renamed = dataSize;
			this.TransferSize_Renamed = transferSize;
		}

		/// <summary>
		/// Retrieves the index of the column or parameter that was truncated.
		/// 
		/// <P>This may be -1 if the column or parameter index is unknown, in
		/// which case the <code>parameter</code> and <code>read</code> fields should be ignored.
		/// </summary>
		/// <returns> the index of the truncated parameter or column value </returns>
		public virtual int Index
		{
			get
			{
				return Index_Renamed;
			}
		}

		/// <summary>
		/// Indicates whether the value truncated was a parameter value or
		/// a column value.
		/// </summary>
		/// <returns> <code>true</code> if the value truncated was a parameter;
		///         <code>false</code> if it was a column value </returns>
		public virtual bool Parameter
		{
			get
			{
				return Parameter_Renamed;
			}
		}

		/// <summary>
		/// Indicates whether or not the value was truncated on a read.
		/// </summary>
		/// <returns> <code>true</code> if the value was truncated when read from
		///         the database; <code>false</code> if the data was truncated on a write </returns>
		public virtual bool Read
		{
			get
			{
				return Read_Renamed;
			}
		}

		/// <summary>
		/// Gets the number of bytes of data that should have been transferred.
		/// This number may be approximate if data conversions were being
		/// performed.  The value may be <code>-1</code> if the size is unknown.
		/// </summary>
		/// <returns> the number of bytes of data that should have been transferred </returns>
		public virtual int DataSize
		{
			get
			{
				return DataSize_Renamed;
			}
		}

		/// <summary>
		/// Gets the number of bytes of data actually transferred.
		/// The value may be <code>-1</code> if the size is unknown.
		/// </summary>
		/// <returns> the number of bytes of data actually transferred </returns>
		public virtual int TransferSize
		{
			get
			{
				return TransferSize_Renamed;
			}
		}

			/// <summary>
			/// @serial
			/// </summary>
		private int Index_Renamed;

			/// <summary>
			/// @serial
			/// </summary>
		private bool Parameter_Renamed;

			/// <summary>
			/// @serial
			/// </summary>
		private bool Read_Renamed;

			/// <summary>
			/// @serial
			/// </summary>
		private int DataSize_Renamed;

			/// <summary>
			/// @serial
			/// </summary>
		private int TransferSize_Renamed;

		/// <summary>
		/// @serial
		/// </summary>
		private new const long SerialVersionUID = 6464298989504059473L;

	}

}
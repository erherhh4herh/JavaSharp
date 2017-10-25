/*
 * Copyright (c) 1998, 2013, Oracle and/or its affiliates. All rights reserved.
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
	/// The subclass of <seealso cref="SQLException"/> thrown when an error
	/// occurs during a batch update operation.  In addition to the
	/// information provided by <seealso cref="SQLException"/>, a
	/// <code>BatchUpdateException</code> provides the update
	/// counts for all commands that were executed successfully during the
	/// batch update, that is, all commands that were executed before the error
	/// occurred.  The order of elements in an array of update counts
	/// corresponds to the order in which commands were added to the batch.
	/// <P>
	/// After a command in a batch update fails to execute properly
	/// and a <code>BatchUpdateException</code> is thrown, the driver
	/// may or may not continue to process the remaining commands in
	/// the batch.  If the driver continues processing after a failure,
	/// the array returned by the method
	/// <code>BatchUpdateException.getUpdateCounts</code> will have
	/// an element for every command in the batch rather than only
	/// elements for the commands that executed successfully before
	/// the error.  In the case where the driver continues processing
	/// commands, the array element for any command
	/// that failed is <code>Statement.EXECUTE_FAILED</code>.
	/// <P>
	/// A JDBC driver implementation should use
	/// the constructor {@code BatchUpdateException(String reason, String SQLState,
	/// int vendorCode, long []updateCounts,Throwable cause) } instead of
	/// constructors that take {@code int[]} for the update counts to avoid the
	/// possibility of overflow.
	/// <para>
	/// If {@code Statement.executeLargeBatch} method is invoked it is recommended that
	/// {@code getLargeUpdateCounts} be called instead of {@code getUpdateCounts}
	/// in order to avoid a possible overflow of the integer update count.
	/// @since 1.2
	/// </para>
	/// </summary>

	public class BatchUpdateException : SQLException
	{

	  /// <summary>
	  /// Constructs a <code>BatchUpdateException</code> object initialized with a given
	  /// <code>reason</code>, <code>SQLState</code>, <code>vendorCode</code> and
	  /// <code>updateCounts</code>.
	  /// The <code>cause</code> is not initialized, and may subsequently be
	  /// initialized by a call to the
	  /// <seealso cref="Throwable#initCause(java.lang.Throwable)"/> method.
	  /// <para>
	  /// <strong>Note:</strong> There is no validation of {@code updateCounts} for
	  /// overflow and because of this it is recommended that you use the constructor
	  /// {@code BatchUpdateException(String reason, String SQLState,
	  /// int vendorCode, long []updateCounts,Throwable cause) }.
	  /// </para> </summary>
	  /// <param name="reason"> a description of the error </param>
	  /// <param name="SQLState"> an XOPEN or SQL:2003 code identifying the exception </param>
	  /// <param name="vendorCode"> an exception code used by a particular
	  /// database vendor </param>
	  /// <param name="updateCounts"> an array of <code>int</code>, with each element
	  /// indicating the update count, <code>Statement.SUCCESS_NO_INFO</code> or
	  /// <code>Statement.EXECUTE_FAILED</code> for each SQL command in
	  /// the batch for JDBC drivers that continue processing
	  /// after a command failure; an update count or
	  /// <code>Statement.SUCCESS_NO_INFO</code> for each SQL command in the batch
	  /// prior to the failure for JDBC drivers that stop processing after a command
	  /// failure
	  /// @since 1.2 </param>
	  /// <seealso cref= #BatchUpdateException(java.lang.String, java.lang.String, int, long[],
	  /// java.lang.Throwable) </seealso>
	  public BatchUpdateException(String reason, String SQLState, int vendorCode, int[] updateCounts) : base(reason, SQLState, vendorCode)
	  {
		  this.UpdateCounts_Renamed = (updateCounts == null) ? null : Arrays.CopyOf(updateCounts, updateCounts.Length);
		  this.LongUpdateCounts = (updateCounts == null) ? null : CopyUpdateCount(updateCounts);
	  }

	  /// <summary>
	  /// Constructs a <code>BatchUpdateException</code> object initialized with a given
	  /// <code>reason</code>, <code>SQLState</code> and
	  /// <code>updateCounts</code>.
	  /// The <code>cause</code> is not initialized, and may subsequently be
	  /// initialized by a call to the
	  /// <seealso cref="Throwable#initCause(java.lang.Throwable)"/> method. The vendor code
	  /// is initialized to 0.
	  /// <para>
	  /// <strong>Note:</strong> There is no validation of {@code updateCounts} for
	  /// overflow and because of this it is recommended that you use the constructor
	  /// {@code BatchUpdateException(String reason, String SQLState,
	  /// int vendorCode, long []updateCounts,Throwable cause) }.
	  /// </para> </summary>
	  /// <param name="reason"> a description of the exception </param>
	  /// <param name="SQLState"> an XOPEN or SQL:2003 code identifying the exception </param>
	  /// <param name="updateCounts"> an array of <code>int</code>, with each element
	  /// indicating the update count, <code>Statement.SUCCESS_NO_INFO</code> or
	  /// <code>Statement.EXECUTE_FAILED</code> for each SQL command in
	  /// the batch for JDBC drivers that continue processing
	  /// after a command failure; an update count or
	  /// <code>Statement.SUCCESS_NO_INFO</code> for each SQL command in the batch
	  /// prior to the failure for JDBC drivers that stop processing after a command
	  /// failure
	  /// @since 1.2 </param>
	  /// <seealso cref= #BatchUpdateException(java.lang.String, java.lang.String, int, long[],
	  /// java.lang.Throwable) </seealso>
	  public BatchUpdateException(String reason, String SQLState, int[] updateCounts) : this(reason, SQLState, 0, updateCounts)
	  {
	  }

	  /// <summary>
	  /// Constructs a <code>BatchUpdateException</code> object initialized with a given
	  /// <code>reason</code> and <code>updateCounts</code>.
	  /// The <code>cause</code> is not initialized, and may subsequently be
	  /// initialized by a call to the
	  /// <seealso cref="Throwable#initCause(java.lang.Throwable)"/> method.  The
	  /// <code>SQLState</code> is initialized to <code>null</code>
	  /// and the vendor code is initialized to 0.
	  /// <para>
	  /// <strong>Note:</strong> There is no validation of {@code updateCounts} for
	  /// overflow and because of this it is recommended that you use the constructor
	  /// {@code BatchUpdateException(String reason, String SQLState,
	  /// int vendorCode, long []updateCounts,Throwable cause) }.
	  /// </para> </summary>
	  /// <param name="reason"> a description of the exception </param>
	  /// <param name="updateCounts"> an array of <code>int</code>, with each element
	  /// indicating the update count, <code>Statement.SUCCESS_NO_INFO</code> or
	  /// <code>Statement.EXECUTE_FAILED</code> for each SQL command in
	  /// the batch for JDBC drivers that continue processing
	  /// after a command failure; an update count or
	  /// <code>Statement.SUCCESS_NO_INFO</code> for each SQL command in the batch
	  /// prior to the failure for JDBC drivers that stop processing after a command
	  /// failure
	  /// @since 1.2 </param>
	  /// <seealso cref= #BatchUpdateException(java.lang.String, java.lang.String, int, long[],
	  /// java.lang.Throwable) </seealso>
	  public BatchUpdateException(String reason, int[] updateCounts) : this(reason, null, 0, updateCounts)
	  {
	  }

	  /// <summary>
	  /// Constructs a <code>BatchUpdateException</code> object initialized with a given
	  /// <code>updateCounts</code>.
	  /// initialized by a call to the
	  /// <seealso cref="Throwable#initCause(java.lang.Throwable)"/> method. The  <code>reason</code>
	  /// and <code>SQLState</code> are initialized to null and the vendor code
	  /// is initialized to 0.
	  /// <para>
	  /// <strong>Note:</strong> There is no validation of {@code updateCounts} for
	  /// overflow and because of this it is recommended that you use the constructor
	  /// {@code BatchUpdateException(String reason, String SQLState,
	  /// int vendorCode, long []updateCounts,Throwable cause) }.
	  /// </para> </summary>
	  /// <param name="updateCounts"> an array of <code>int</code>, with each element
	  /// indicating the update count, <code>Statement.SUCCESS_NO_INFO</code> or
	  /// <code>Statement.EXECUTE_FAILED</code> for each SQL command in
	  /// the batch for JDBC drivers that continue processing
	  /// after a command failure; an update count or
	  /// <code>Statement.SUCCESS_NO_INFO</code> for each SQL command in the batch
	  /// prior to the failure for JDBC drivers that stop processing after a command
	  /// failure
	  /// @since 1.2 </param>
	  /// <seealso cref= #BatchUpdateException(java.lang.String, java.lang.String, int, long[],
	  /// java.lang.Throwable) </seealso>
	  public BatchUpdateException(int[] updateCounts) : this(null, null, 0, updateCounts)
	  {
	  }

	  /// <summary>
	  /// Constructs a <code>BatchUpdateException</code> object.
	  /// The <code>reason</code>, <code>SQLState</code> and <code>updateCounts</code>
	  ///  are initialized to <code>null</code> and the vendor code is initialized to 0.
	  /// The <code>cause</code> is not initialized, and may subsequently be
	  /// initialized by a call to the
	  /// <seealso cref="Throwable#initCause(java.lang.Throwable)"/> method.
	  /// <para>
	  /// 
	  /// @since 1.2
	  /// </para>
	  /// </summary>
	  /// <seealso cref= #BatchUpdateException(java.lang.String, java.lang.String, int, long[],
	  /// java.lang.Throwable) </seealso>
	  public BatchUpdateException() : this(null, null, 0, null)
	  {
	  }

	  /// <summary>
	  /// Constructs a <code>BatchUpdateException</code> object initialized with
	  ///  a given <code>cause</code>.
	  /// The <code>SQLState</code> and <code>updateCounts</code>
	  /// are initialized
	  /// to <code>null</code> and the vendor code is initialized to 0.
	  /// The <code>reason</code>  is initialized to <code>null</code> if
	  /// <code>cause==null</code> or to <code>cause.toString()</code> if
	  ///  <code>cause!=null</code>. </summary>
	  /// <param name="cause"> the underlying reason for this <code>SQLException</code>
	  /// (which is saved for later retrieval by the <code>getCause()</code> method);
	  /// may be null indicating the cause is non-existent or unknown.
	  /// @since 1.6 </param>
	  /// <seealso cref= #BatchUpdateException(java.lang.String, java.lang.String, int, long[],
	  /// java.lang.Throwable) </seealso>
	  public BatchUpdateException(Throwable cause) : this((cause == null ? null : cause.ToString()), null, 0, (int[])null, cause)
	  {
	  }

	  /// <summary>
	  /// Constructs a <code>BatchUpdateException</code> object initialized with a
	  /// given <code>cause</code> and <code>updateCounts</code>.
	  /// The <code>SQLState</code> is initialized
	  /// to <code>null</code> and the vendor code is initialized to 0.
	  /// The <code>reason</code>  is initialized to <code>null</code> if
	  /// <code>cause==null</code> or to <code>cause.toString()</code> if
	  /// <code>cause!=null</code>.
	  /// <para>
	  /// <strong>Note:</strong> There is no validation of {@code updateCounts} for
	  /// overflow and because of this it is recommended that you use the constructor
	  /// {@code BatchUpdateException(String reason, String SQLState,
	  /// int vendorCode, long []updateCounts,Throwable cause) }.
	  /// </para> </summary>
	  /// <param name="updateCounts"> an array of <code>int</code>, with each element
	  /// indicating the update count, <code>Statement.SUCCESS_NO_INFO</code> or
	  /// <code>Statement.EXECUTE_FAILED</code> for each SQL command in
	  /// the batch for JDBC drivers that continue processing
	  /// after a command failure; an update count or
	  /// <code>Statement.SUCCESS_NO_INFO</code> for each SQL command in the batch
	  /// prior to the failure for JDBC drivers that stop processing after a command
	  /// failure </param>
	  /// <param name="cause"> the underlying reason for this <code>SQLException</code>
	  /// (which is saved for later retrieval by the <code>getCause()</code> method); may be null indicating
	  /// the cause is non-existent or unknown.
	  /// @since 1.6 </param>
	  /// <seealso cref= #BatchUpdateException(java.lang.String, java.lang.String, int, long[],
	  /// java.lang.Throwable) </seealso>
	  public BatchUpdateException(int[] updateCounts, Throwable cause) : this((cause == null ? null : cause.ToString()), null, 0, updateCounts, cause)
	  {
	  }

	  /// <summary>
	  /// Constructs a <code>BatchUpdateException</code> object initialized with
	  /// a given <code>reason</code>, <code>cause</code>
	  /// and <code>updateCounts</code>. The <code>SQLState</code> is initialized
	  /// to <code>null</code> and the vendor code is initialized to 0.
	  /// <para>
	  /// <strong>Note:</strong> There is no validation of {@code updateCounts} for
	  /// overflow and because of this it is recommended that you use the constructor
	  /// {@code BatchUpdateException(String reason, String SQLState,
	  /// int vendorCode, long []updateCounts,Throwable cause) }.
	  /// </para> </summary>
	  /// <param name="reason"> a description of the exception </param>
	  /// <param name="updateCounts"> an array of <code>int</code>, with each element
	  /// indicating the update count, <code>Statement.SUCCESS_NO_INFO</code> or
	  /// <code>Statement.EXECUTE_FAILED</code> for each SQL command in
	  /// the batch for JDBC drivers that continue processing
	  /// after a command failure; an update count or
	  /// <code>Statement.SUCCESS_NO_INFO</code> for each SQL command in the batch
	  /// prior to the failure for JDBC drivers that stop processing after a command
	  /// failure </param>
	  /// <param name="cause"> the underlying reason for this <code>SQLException</code> (which is saved for later retrieval by the <code>getCause()</code> method);
	  /// may be null indicating
	  /// the cause is non-existent or unknown.
	  /// @since 1.6 </param>
	  /// <seealso cref= #BatchUpdateException(java.lang.String, java.lang.String, int, long[],
	  /// java.lang.Throwable) </seealso>
	  public BatchUpdateException(String reason, int[] updateCounts, Throwable cause) : this(reason, null, 0, updateCounts, cause)
	  {
	  }

	  /// <summary>
	  /// Constructs a <code>BatchUpdateException</code> object initialized with
	  /// a given <code>reason</code>, <code>SQLState</code>,<code>cause</code>, and
	  /// <code>updateCounts</code>. The vendor code is initialized to 0.
	  /// </summary>
	  /// <param name="reason"> a description of the exception </param>
	  /// <param name="SQLState"> an XOPEN or SQL:2003 code identifying the exception </param>
	  /// <param name="updateCounts"> an array of <code>int</code>, with each element
	  /// indicating the update count, <code>Statement.SUCCESS_NO_INFO</code> or
	  /// <code>Statement.EXECUTE_FAILED</code> for each SQL command in
	  /// the batch for JDBC drivers that continue processing
	  /// after a command failure; an update count or
	  /// <code>Statement.SUCCESS_NO_INFO</code> for each SQL command in the batch
	  /// prior to the failure for JDBC drivers that stop processing after a command
	  /// failure
	  /// <para>
	  /// <strong>Note:</strong> There is no validation of {@code updateCounts} for
	  /// overflow and because of this it is recommended that you use the constructor
	  /// {@code BatchUpdateException(String reason, String SQLState,
	  /// int vendorCode, long []updateCounts,Throwable cause) }.
	  /// </para> </param>
	  /// <param name="cause"> the underlying reason for this <code>SQLException</code>
	  /// (which is saved for later retrieval by the <code>getCause()</code> method);
	  /// may be null indicating
	  /// the cause is non-existent or unknown.
	  /// @since 1.6 </param>
	  /// <seealso cref= #BatchUpdateException(java.lang.String, java.lang.String, int, long[],
	  /// java.lang.Throwable) </seealso>
	  public BatchUpdateException(String reason, String SQLState, int[] updateCounts, Throwable cause) : this(reason, SQLState, 0, updateCounts, cause)
	  {
	  }

	  /// <summary>
	  /// Constructs a <code>BatchUpdateException</code> object initialized with
	  /// a given <code>reason</code>, <code>SQLState</code>, <code>vendorCode</code>
	  /// <code>cause</code> and <code>updateCounts</code>.
	  /// </summary>
	  /// <param name="reason"> a description of the error </param>
	  /// <param name="SQLState"> an XOPEN or SQL:2003 code identifying the exception </param>
	  /// <param name="vendorCode"> an exception code used by a particular
	  /// database vendor </param>
	  /// <param name="updateCounts"> an array of <code>int</code>, with each element
	  /// indicating the update count, <code>Statement.SUCCESS_NO_INFO</code> or
	  /// <code>Statement.EXECUTE_FAILED</code> for each SQL command in
	  /// the batch for JDBC drivers that continue processing
	  /// after a command failure; an update count or
	  /// <code>Statement.SUCCESS_NO_INFO</code> for each SQL command in the batch
	  /// prior to the failure for JDBC drivers that stop processing after a command
	  /// failure
	  /// <para>
	  /// <strong>Note:</strong> There is no validation of {@code updateCounts} for
	  /// overflow and because of this it is recommended that you use the constructor
	  /// {@code BatchUpdateException(String reason, String SQLState,
	  /// int vendorCode, long []updateCounts,Throwable cause) }.
	  /// </para> </param>
	  /// <param name="cause"> the underlying reason for this <code>SQLException</code> (which is saved for later retrieval by the <code>getCause()</code> method);
	  /// may be null indicating
	  /// the cause is non-existent or unknown.
	  /// @since 1.6 </param>
	  /// <seealso cref= #BatchUpdateException(java.lang.String, java.lang.String, int, long[],
	  /// java.lang.Throwable) </seealso>
	  public BatchUpdateException(String reason, String SQLState, int vendorCode, int[] updateCounts, Throwable cause) : base(reason, SQLState, vendorCode, cause)
	  {
			this.UpdateCounts_Renamed = (updateCounts == null) ? null : Arrays.CopyOf(updateCounts, updateCounts.Length);
			this.LongUpdateCounts = (updateCounts == null) ? null : CopyUpdateCount(updateCounts);
	  }

	  /// <summary>
	  /// Retrieves the update count for each update statement in the batch
	  /// update that executed successfully before this exception occurred.
	  /// A driver that implements batch updates may or may not continue to
	  /// process the remaining commands in a batch when one of the commands
	  /// fails to execute properly. If the driver continues processing commands,
	  /// the array returned by this method will have as many elements as
	  /// there are commands in the batch; otherwise, it will contain an
	  /// update count for each command that executed successfully before
	  /// the <code>BatchUpdateException</code> was thrown.
	  /// <P>
	  /// The possible return values for this method were modified for
	  /// the Java 2 SDK, Standard Edition, version 1.3.  This was done to
	  /// accommodate the new option of continuing to process commands
	  /// in a batch update after a <code>BatchUpdateException</code> object
	  /// has been thrown.
	  /// </summary>
	  /// <returns> an array of <code>int</code> containing the update counts
	  /// for the updates that were executed successfully before this error
	  /// occurred.  Or, if the driver continues to process commands after an
	  /// error, one of the following for every command in the batch:
	  /// <OL>
	  /// <LI>an update count
	  ///  <LI><code>Statement.SUCCESS_NO_INFO</code> to indicate that the command
	  ///     executed successfully but the number of rows affected is unknown
	  ///  <LI><code>Statement.EXECUTE_FAILED</code> to indicate that the command
	  ///     failed to execute successfully
	  /// </OL>
	  /// @since 1.3 </returns>
	  /// <seealso cref= #getLargeUpdateCounts() </seealso>
	  public virtual int[] UpdateCounts
	  {
		  get
		  {
			  return (UpdateCounts_Renamed == null) ? null : Arrays.CopyOf(UpdateCounts_Renamed, UpdateCounts_Renamed.Length);
		  }
	  }

	  /// <summary>
	  /// Constructs a <code>BatchUpdateException</code> object initialized with
	  /// a given <code>reason</code>, <code>SQLState</code>, <code>vendorCode</code>
	  /// <code>cause</code> and <code>updateCounts</code>.
	  /// <para>
	  /// This constructor should be used when the returned update count may exceed
	  /// <seealso cref="Integer#MAX_VALUE"/>.
	  /// </para>
	  /// <para>
	  /// </para>
	  /// </summary>
	  /// <param name="reason"> a description of the error </param>
	  /// <param name="SQLState"> an XOPEN or SQL:2003 code identifying the exception </param>
	  /// <param name="vendorCode"> an exception code used by a particular
	  /// database vendor </param>
	  /// <param name="updateCounts"> an array of <code>long</code>, with each element
	  /// indicating the update count, <code>Statement.SUCCESS_NO_INFO</code> or
	  /// <code>Statement.EXECUTE_FAILED</code> for each SQL command in
	  /// the batch for JDBC drivers that continue processing
	  /// after a command failure; an update count or
	  /// <code>Statement.SUCCESS_NO_INFO</code> for each SQL command in the batch
	  /// prior to the failure for JDBC drivers that stop processing after a command
	  /// failure </param>
	  /// <param name="cause"> the underlying reason for this <code>SQLException</code>
	  /// (which is saved for later retrieval by the <code>getCause()</code> method);
	  /// may be null indicating the cause is non-existent or unknown.
	  /// @since 1.8 </param>
	  public BatchUpdateException(String reason, String SQLState, int vendorCode, long[] updateCounts, Throwable cause) : base(reason, SQLState, vendorCode, cause)
	  {
		  this.LongUpdateCounts = (updateCounts == null) ? null : Arrays.CopyOf(updateCounts, updateCounts.Length);
		  this.UpdateCounts_Renamed = (LongUpdateCounts == null) ? null : CopyUpdateCount(LongUpdateCounts);
	  }

	  /// <summary>
	  /// Retrieves the update count for each update statement in the batch
	  /// update that executed successfully before this exception occurred.
	  /// A driver that implements batch updates may or may not continue to
	  /// process the remaining commands in a batch when one of the commands
	  /// fails to execute properly. If the driver continues processing commands,
	  /// the array returned by this method will have as many elements as
	  /// there are commands in the batch; otherwise, it will contain an
	  /// update count for each command that executed successfully before
	  /// the <code>BatchUpdateException</code> was thrown.
	  /// <para>
	  /// This method should be used when {@code Statement.executeLargeBatch} is
	  /// invoked and the returned update count may exceed <seealso cref="Integer#MAX_VALUE"/>.
	  /// </para>
	  /// <para>
	  /// </para>
	  /// </summary>
	  /// <returns> an array of <code>long</code> containing the update counts
	  /// for the updates that were executed successfully before this error
	  /// occurred.  Or, if the driver continues to process commands after an
	  /// error, one of the following for every command in the batch:
	  /// <OL>
	  /// <LI>an update count
	  ///  <LI><code>Statement.SUCCESS_NO_INFO</code> to indicate that the command
	  ///     executed successfully but the number of rows affected is unknown
	  ///  <LI><code>Statement.EXECUTE_FAILED</code> to indicate that the command
	  ///     failed to execute successfully
	  /// </OL>
	  /// @since 1.8 </returns>
	  public virtual long[] LargeUpdateCounts
	  {
		  get
		  {
			  return (LongUpdateCounts == null) ? null : Arrays.CopyOf(LongUpdateCounts, LongUpdateCounts.Length);
		  }
	  }

	  /// <summary>
	  /// The array that describes the outcome of a batch execution.
	  /// @serial
	  /// @since 1.2
	  /// </summary>
	  private int[] UpdateCounts_Renamed;

	  /*
	   * Starting with Java SE 8, JDBC has added support for returning an update
	   * count > Integer.MAX_VALUE.  Because of this the following changes were made
	   * to BatchUpdateException:
	   * <ul>
	   * <li>Add field longUpdateCounts</li>
	   * <li>Add Constructorr which takes long[] for update counts</li>
	   * <li>Add getLargeUpdateCounts method</li>
	   * </ul>
	   * When any of the constructors are called, the int[] and long[] updateCount
	   * fields are populated by copying the one array to each other.
	   *
	   * As the JDBC driver passes in the updateCounts, there has always been the
	   * possiblity for overflow and BatchUpdateException does not need to account
	   * for that, it simply copies the arrays.
	   *
	   * JDBC drivers should always use the constructor that specifies long[] and
	   * JDBC application developers should call getLargeUpdateCounts.
	   */

	  /// <summary>
	  /// The array that describes the outcome of a batch execution.
	  /// @serial
	  /// @since 1.8
	  /// </summary>
	  private long[] LongUpdateCounts;

	  private new const long SerialVersionUID = 5977529877145521757L;

	  /*
	   * Utility method to copy int[] updateCount to long[] updateCount
	   */
	  private static long[] CopyUpdateCount(int[] uc)
	  {
		  long[] copy = new long[uc.Length];
		  for (int i = 0; i < uc.Length; i++)
		  {
			  copy[i] = uc[i];
		  }
		  return copy;
	  }

	  /*
	   * Utility method to copy long[] updateCount to int[] updateCount.
	   * No checks for overflow will be done as it is expected a  user will call
	   * getLargeUpdateCounts.
	   */
	  private static int[] CopyUpdateCount(long[] uc)
	  {
		  int[] copy = new int[uc.Length];
		  for (int i = 0; i < uc.Length; i++)
		  {
			  copy[i] = (int) uc[i];
		  }
		  return copy;
	  }
		/// <summary>
		/// readObject is called to restore the state of the
		/// {@code BatchUpdateException} from a stream.
		/// </summary>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: private void readObject(java.io.ObjectInputStream s) throws java.io.IOException, ClassNotFoundException
		private void ReadObject(ObjectInputStream s)
		{

		   ObjectInputStream.GetField fields = s.ReadFields();
		   int[] tmp = (int[])fields.Get("updateCounts", null);
		   long[] tmp2 = (long[])fields.Get("longUpdateCounts", null);
		   if (tmp != null && tmp2 != null && tmp.Length != tmp2.Length)
		   {
			   throw new InvalidObjectException("update counts are not the expected size");
		   }
		   if (tmp != null)
		   {
			   UpdateCounts_Renamed = tmp.clone();
		   }
		   if (tmp2 != null)
		   {
			   LongUpdateCounts = tmp2.clone();
		   }
		   if (UpdateCounts_Renamed == null && LongUpdateCounts != null)
		   {
			   UpdateCounts_Renamed = CopyUpdateCount(LongUpdateCounts);
		   }
		   if (LongUpdateCounts == null && UpdateCounts_Renamed != null)
		   {
			   LongUpdateCounts = CopyUpdateCount(UpdateCounts_Renamed);
		   }

		}

		/// <summary>
		/// writeObject is called to save the state of the {@code BatchUpdateException}
		/// to a stream.
		/// </summary>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: private void writeObject(java.io.ObjectOutputStream s) throws java.io.IOException, ClassNotFoundException
		private void WriteObject(ObjectOutputStream s)
		{

			ObjectOutputStream.PutField fields = s.PutFields();
			fields.Put("updateCounts", UpdateCounts_Renamed);
			fields.Put("longUpdateCounts", LongUpdateCounts);
			s.WriteFields();
		}
	}

}
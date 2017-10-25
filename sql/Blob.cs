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
	/// The representation (mapping) in
	/// the Java&trade; programming
	/// language of an SQL
	/// <code>BLOB</code> value.  An SQL <code>BLOB</code> is a built-in type
	/// that stores a Binary Large Object as a column value in a row of
	/// a database table. By default drivers implement <code>Blob</code> using
	/// an SQL <code>locator(BLOB)</code>, which means that a
	/// <code>Blob</code> object contains a logical pointer to the
	/// SQL <code>BLOB</code> data rather than the data itself.
	/// A <code>Blob</code> object is valid for the duration of the
	/// transaction in which is was created.
	/// 
	/// <P>Methods in the interfaces <seealso cref="ResultSet"/>,
	/// <seealso cref="CallableStatement"/>, and <seealso cref="PreparedStatement"/>, such as
	/// <code>getBlob</code> and <code>setBlob</code> allow a programmer to
	/// access an SQL <code>BLOB</code> value.
	/// The <code>Blob</code> interface provides methods for getting the
	/// length of an SQL <code>BLOB</code> (Binary Large Object) value,
	/// for materializing a <code>BLOB</code> value on the client, and for
	/// determining the position of a pattern of bytes within a
	/// <code>BLOB</code> value. In addition, this interface has methods for updating
	/// a <code>BLOB</code> value.
	/// <para>
	/// All methods on the <code>Blob</code> interface must be fully implemented if the
	/// JDBC driver supports the data type.
	/// 
	/// @since 1.2
	/// </para>
	/// </summary>

	public interface Blob
	{

	  /// <summary>
	  /// Returns the number of bytes in the <code>BLOB</code> value
	  /// designated by this <code>Blob</code> object. </summary>
	  /// <returns> length of the <code>BLOB</code> in bytes </returns>
	  /// <exception cref="SQLException"> if there is an error accessing the
	  /// length of the <code>BLOB</code> </exception>
	  /// <exception cref="SQLFeatureNotSupportedException"> if the JDBC driver does not support
	  /// this method
	  /// @since 1.2 </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: long length() throws SQLException;
	  long Length();

	  /// <summary>
	  /// Retrieves all or part of the <code>BLOB</code>
	  /// value that this <code>Blob</code> object represents, as an array of
	  /// bytes.  This <code>byte</code> array contains up to <code>length</code>
	  /// consecutive bytes starting at position <code>pos</code>.
	  /// </summary>
	  /// <param name="pos"> the ordinal position of the first byte in the
	  ///        <code>BLOB</code> value to be extracted; the first byte is at
	  ///        position 1 </param>
	  /// <param name="length"> the number of consecutive bytes to be copied; the value
	  /// for length must be 0 or greater </param>
	  /// <returns> a byte array containing up to <code>length</code>
	  ///         consecutive bytes from the <code>BLOB</code> value designated
	  ///         by this <code>Blob</code> object, starting with the
	  ///         byte at position <code>pos</code> </returns>
	  /// <exception cref="SQLException"> if there is an error accessing the
	  ///            <code>BLOB</code> value; if pos is less than 1 or length is
	  /// less than 0 </exception>
	  /// <exception cref="SQLFeatureNotSupportedException"> if the JDBC driver does not support
	  /// this method </exception>
	  /// <seealso cref= #setBytes
	  /// @since 1.2 </seealso>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: byte[] getBytes(long pos, int length) throws SQLException;
	  sbyte[] GetBytes(long pos, int length);

	  /// <summary>
	  /// Retrieves the <code>BLOB</code> value designated by this
	  /// <code>Blob</code> instance as a stream.
	  /// </summary>
	  /// <returns> a stream containing the <code>BLOB</code> data </returns>
	  /// <exception cref="SQLException"> if there is an error accessing the
	  ///            <code>BLOB</code> value </exception>
	  /// <exception cref="SQLFeatureNotSupportedException"> if the JDBC driver does not support
	  /// this method </exception>
	  /// <seealso cref= #setBinaryStream
	  /// @since 1.2 </seealso>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: java.io.InputStream getBinaryStream() throws SQLException;
	  InputStream BinaryStream {get;}

	  /// <summary>
	  /// Retrieves the byte position at which the specified byte array
	  /// <code>pattern</code> begins within the <code>BLOB</code>
	  /// value that this <code>Blob</code> object represents.  The
	  /// search for <code>pattern</code> begins at position
	  /// <code>start</code>.
	  /// </summary>
	  /// <param name="pattern"> the byte array for which to search </param>
	  /// <param name="start"> the position at which to begin searching; the
	  ///        first position is 1 </param>
	  /// <returns> the position at which the pattern appears, else -1 </returns>
	  /// <exception cref="SQLException"> if there is an error accessing the
	  /// <code>BLOB</code> or if start is less than 1 </exception>
	  /// <exception cref="SQLFeatureNotSupportedException"> if the JDBC driver does not support
	  /// this method
	  /// @since 1.2 </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: long position(byte pattern[] , long start) throws SQLException;
	  long Position(sbyte[] pattern, long start);

	  /// <summary>
	  /// Retrieves the byte position in the <code>BLOB</code> value
	  /// designated by this <code>Blob</code> object at which
	  /// <code>pattern</code> begins.  The search begins at position
	  /// <code>start</code>.
	  /// </summary>
	  /// <param name="pattern"> the <code>Blob</code> object designating
	  /// the <code>BLOB</code> value for which to search </param>
	  /// <param name="start"> the position in the <code>BLOB</code> value
	  ///        at which to begin searching; the first position is 1 </param>
	  /// <returns> the position at which the pattern begins, else -1 </returns>
	  /// <exception cref="SQLException"> if there is an error accessing the
	  ///            <code>BLOB</code> value or if start is less than 1 </exception>
	  /// <exception cref="SQLFeatureNotSupportedException"> if the JDBC driver does not support
	  /// this method
	  /// @since 1.2 </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: long position(Blob pattern, long start) throws SQLException;
	  long Position(Blob pattern, long start);

		// -------------------------- JDBC 3.0 -----------------------------------

		/// <summary>
		/// Writes the given array of bytes to the <code>BLOB</code> value that
		/// this <code>Blob</code> object represents, starting at position
		/// <code>pos</code>, and returns the number of bytes written.
		/// The array of bytes will overwrite the existing bytes
		/// in the <code>Blob</code> object starting at the position
		/// <code>pos</code>.  If the end of the <code>Blob</code> value is reached
		/// while writing the array of bytes, then the length of the <code>Blob</code>
		/// value will be increased to accommodate the extra bytes.
		/// <para>
		/// <b>Note:</b> If the value specified for <code>pos</code>
		/// is greater then the length+1 of the <code>BLOB</code> value then the
		/// behavior is undefined. Some JDBC drivers may throw a
		/// <code>SQLException</code> while other drivers may support this
		/// operation.
		/// 
		/// </para>
		/// </summary>
		/// <param name="pos"> the position in the <code>BLOB</code> object at which
		///        to start writing; the first position is 1 </param>
		/// <param name="bytes"> the array of bytes to be written to the <code>BLOB</code>
		///        value that this <code>Blob</code> object represents </param>
		/// <returns> the number of bytes written </returns>
		/// <exception cref="SQLException"> if there is an error accessing the
		///            <code>BLOB</code> value or if pos is less than 1 </exception>
		/// <exception cref="SQLFeatureNotSupportedException"> if the JDBC driver does not support
		/// this method </exception>
		/// <seealso cref= #getBytes
		/// @since 1.4 </seealso>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: int setBytes(long pos, byte[] bytes) throws SQLException;
		int SetBytes(long pos, sbyte[] bytes);

		/// <summary>
		/// Writes all or part of the given <code>byte</code> array to the
		/// <code>BLOB</code> value that this <code>Blob</code> object represents
		/// and returns the number of bytes written.
		/// Writing starts at position <code>pos</code> in the <code>BLOB</code>
		/// value; <code>len</code> bytes from the given byte array are written.
		/// The array of bytes will overwrite the existing bytes
		/// in the <code>Blob</code> object starting at the position
		/// <code>pos</code>.  If the end of the <code>Blob</code> value is reached
		/// while writing the array of bytes, then the length of the <code>Blob</code>
		/// value will be increased to accommodate the extra bytes.
		/// <para>
		/// <b>Note:</b> If the value specified for <code>pos</code>
		/// is greater then the length+1 of the <code>BLOB</code> value then the
		/// behavior is undefined. Some JDBC drivers may throw a
		/// <code>SQLException</code> while other drivers may support this
		/// operation.
		/// 
		/// </para>
		/// </summary>
		/// <param name="pos"> the position in the <code>BLOB</code> object at which
		///        to start writing; the first position is 1 </param>
		/// <param name="bytes"> the array of bytes to be written to this <code>BLOB</code>
		///        object </param>
		/// <param name="offset"> the offset into the array <code>bytes</code> at which
		///        to start reading the bytes to be set </param>
		/// <param name="len"> the number of bytes to be written to the <code>BLOB</code>
		///        value from the array of bytes <code>bytes</code> </param>
		/// <returns> the number of bytes written </returns>
		/// <exception cref="SQLException"> if there is an error accessing the
		///            <code>BLOB</code> value or if pos is less than 1 </exception>
		/// <exception cref="SQLFeatureNotSupportedException"> if the JDBC driver does not support
		/// this method </exception>
		/// <seealso cref= #getBytes
		/// @since 1.4 </seealso>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: int setBytes(long pos, byte[] bytes, int offset, int len) throws SQLException;
		int SetBytes(long pos, sbyte[] bytes, int offset, int len);

		/// <summary>
		/// Retrieves a stream that can be used to write to the <code>BLOB</code>
		/// value that this <code>Blob</code> object represents.  The stream begins
		/// at position <code>pos</code>.
		/// The  bytes written to the stream will overwrite the existing bytes
		/// in the <code>Blob</code> object starting at the position
		/// <code>pos</code>.  If the end of the <code>Blob</code> value is reached
		/// while writing to the stream, then the length of the <code>Blob</code>
		/// value will be increased to accommodate the extra bytes.
		/// <para>
		/// <b>Note:</b> If the value specified for <code>pos</code>
		/// is greater then the length+1 of the <code>BLOB</code> value then the
		/// behavior is undefined. Some JDBC drivers may throw a
		/// <code>SQLException</code> while other drivers may support this
		/// operation.
		/// 
		/// </para>
		/// </summary>
		/// <param name="pos"> the position in the <code>BLOB</code> value at which
		///        to start writing; the first position is 1 </param>
		/// <returns> a <code>java.io.OutputStream</code> object to which data can
		///         be written </returns>
		/// <exception cref="SQLException"> if there is an error accessing the
		///            <code>BLOB</code> value or if pos is less than 1 </exception>
		/// <exception cref="SQLFeatureNotSupportedException"> if the JDBC driver does not support
		/// this method </exception>
		/// <seealso cref= #getBinaryStream
		/// @since 1.4 </seealso>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: java.io.OutputStream setBinaryStream(long pos) throws SQLException;
		java.io.OutputStream SetBinaryStream(long pos);

		/// <summary>
		/// Truncates the <code>BLOB</code> value that this <code>Blob</code>
		/// object represents to be <code>len</code> bytes in length.
		/// <para>
		/// <b>Note:</b> If the value specified for <code>pos</code>
		/// is greater then the length+1 of the <code>BLOB</code> value then the
		/// behavior is undefined. Some JDBC drivers may throw a
		/// <code>SQLException</code> while other drivers may support this
		/// operation.
		/// 
		/// </para>
		/// </summary>
		/// <param name="len"> the length, in bytes, to which the <code>BLOB</code> value
		///        that this <code>Blob</code> object represents should be truncated </param>
		/// <exception cref="SQLException"> if there is an error accessing the
		///            <code>BLOB</code> value or if len is less than 0 </exception>
		/// <exception cref="SQLFeatureNotSupportedException"> if the JDBC driver does not support
		/// this method
		/// @since 1.4 </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: void truncate(long len) throws SQLException;
		void Truncate(long len);

		/// <summary>
		/// This method frees the <code>Blob</code> object and releases the resources that
		/// it holds. The object is invalid once the <code>free</code>
		/// method is called.
		/// <para>
		/// After <code>free</code> has been called, any attempt to invoke a
		/// method other than <code>free</code> will result in a <code>SQLException</code>
		/// being thrown.  If <code>free</code> is called multiple times, the subsequent
		/// calls to <code>free</code> are treated as a no-op.
		/// </para>
		/// <para>
		/// 
		/// </para>
		/// </summary>
		/// <exception cref="SQLException"> if an error occurs releasing
		/// the Blob's resources </exception>
		/// <exception cref="SQLFeatureNotSupportedException"> if the JDBC driver does not support
		/// this method
		/// @since 1.6 </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: void free() throws SQLException;
		void Free();

		/// <summary>
		/// Returns an <code>InputStream</code> object that contains a partial <code>Blob</code> value,
		/// starting  with the byte specified by pos, which is length bytes in length.
		/// </summary>
		/// <param name="pos"> the offset to the first byte of the partial value to be retrieved.
		///  The first byte in the <code>Blob</code> is at position 1 </param>
		/// <param name="length"> the length in bytes of the partial value to be retrieved </param>
		/// <returns> <code>InputStream</code> through which the partial <code>Blob</code> value can be read. </returns>
		/// <exception cref="SQLException"> if pos is less than 1 or if pos is greater than the number of bytes
		/// in the <code>Blob</code> or if pos + length is greater than the number of bytes
		/// in the <code>Blob</code>
		/// </exception>
		/// <exception cref="SQLFeatureNotSupportedException"> if the JDBC driver does not support
		/// this method
		/// @since 1.6 </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: java.io.InputStream getBinaryStream(long pos, long length) throws SQLException;
		InputStream GetBinaryStream(long pos, long length);
	}

}
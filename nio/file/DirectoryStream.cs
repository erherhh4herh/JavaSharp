using System.Collections.Generic;

/*
 * Copyright (c) 2007, 2013, Oracle and/or its affiliates. All rights reserved.
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

namespace java.nio.file
{


	/// <summary>
	/// An object to iterate over the entries in a directory. A directory stream
	/// allows for the convenient use of the for-each construct to iterate over a
	/// directory.
	/// 
	/// <para> <b> While {@code DirectoryStream} extends {@code Iterable}, it is not a
	/// general-purpose {@code Iterable} as it supports only a single {@code
	/// Iterator}; invoking the <seealso cref="#iterator iterator"/> method to obtain a second
	/// or subsequent iterator throws {@code IllegalStateException}. </b>
	/// 
	/// </para>
	/// <para> An important property of the directory stream's {@code Iterator} is that
	/// its <seealso cref="Iterator#hasNext() hasNext"/> method is guaranteed to read-ahead by
	/// at least one element. If {@code hasNext} method returns {@code true}, and is
	/// followed by a call to the {@code next} method, it is guaranteed that the
	/// {@code next} method will not throw an exception due to an I/O error, or
	/// because the stream has been <seealso cref="#close closed"/>. The {@code Iterator} does
	/// not support the <seealso cref="Iterator#remove remove"/> operation.
	/// 
	/// </para>
	/// <para> A {@code DirectoryStream} is opened upon creation and is closed by
	/// invoking the {@code close} method. Closing a directory stream releases any
	/// resources associated with the stream. Failure to close the stream may result
	/// in a resource leak. The try-with-resources statement provides a useful
	/// construct to ensure that the stream is closed:
	/// <pre>
	///   Path dir = ...
	///   try (DirectoryStream&lt;Path&gt; stream = Files.newDirectoryStream(dir)) {
	///       for (Path entry: stream) {
	///           ...
	///       }
	///   }
	/// </pre>
	/// 
	/// </para>
	/// <para> Once a directory stream is closed, then further access to the directory,
	/// using the {@code Iterator}, behaves as if the end of stream has been reached.
	/// Due to read-ahead, the {@code Iterator} may return one or more elements
	/// after the directory stream has been closed. Once these buffered elements
	/// have been read, then subsequent calls to the {@code hasNext} method returns
	/// {@code false}, and subsequent calls to the {@code next} method will throw
	/// {@code NoSuchElementException}.
	/// 
	/// </para>
	/// <para> A directory stream is not required to be <i>asynchronously closeable</i>.
	/// If a thread is blocked on the directory stream's iterator reading from the
	/// directory, and another thread invokes the {@code close} method, then the
	/// second thread may block until the read operation is complete.
	/// 
	/// </para>
	/// <para> If an I/O error is encountered when accessing the directory then it
	/// causes the {@code Iterator}'s {@code hasNext} or {@code next} methods to
	/// throw <seealso cref="DirectoryIteratorException"/> with the <seealso cref="IOException"/> as the
	/// cause. As stated above, the {@code hasNext} method is guaranteed to
	/// read-ahead by at least one element. This means that if {@code hasNext} method
	/// returns {@code true}, and is followed by a call to the {@code next} method,
	/// then it is guaranteed that the {@code next} method will not fail with a
	/// {@code DirectoryIteratorException}.
	/// 
	/// </para>
	/// <para> The elements returned by the iterator are in no specific order. Some file
	/// systems maintain special links to the directory itself and the directory's
	/// parent directory. Entries representing these links are not returned by the
	/// iterator.
	/// 
	/// </para>
	/// <para> The iterator is <i>weakly consistent</i>. It is thread safe but does not
	/// freeze the directory while iterating, so it may (or may not) reflect updates
	/// to the directory that occur after the {@code DirectoryStream} is created.
	/// 
	/// </para>
	/// <para> <b>Usage Examples:</b>
	/// Suppose we want a list of the source files in a directory. This example uses
	/// both the for-each and try-with-resources constructs.
	/// <pre>
	///   List&lt;Path&gt; listSourceFiles(Path dir) throws IOException {
	///       List&lt;Path&gt; result = new ArrayList&lt;&gt;();
	///       try (DirectoryStream&lt;Path&gt; stream = Files.newDirectoryStream(dir, "*.{c,h,cpp,hpp,java}")) {
	///           for (Path entry: stream) {
	///               result.add(entry);
	///           }
	///       } catch (DirectoryIteratorException ex) {
	///           // I/O error encounted during the iteration, the cause is an IOException
	///           throw ex.getCause();
	///       }
	///       return result;
	///   }
	/// </pre>
	/// </para>
	/// </summary>
	/// @param   <T>     The type of element returned by the iterator
	/// 
	/// @since 1.7
	/// </param>
	/// <seealso cref= Files#newDirectoryStream(Path) </seealso>

	public interface DirectoryStream<T> : Closeable, Iterable<T>
	{
		/// <summary>
		/// An interface that is implemented by objects that decide if a directory
		/// entry should be accepted or filtered. A {@code Filter} is passed as the
		/// parameter to the <seealso cref="Files#newDirectoryStream(Path,DirectoryStream.Filter)"/>
		/// method when opening a directory to iterate over the entries in the
		/// directory.
		/// </summary>
		/// @param   <T>     the type of the directory entry
		/// 
		/// @since 1.7 </param>

		/// <summary>
		/// Returns the iterator associated with this {@code DirectoryStream}.
		/// </summary>
		/// <returns>  the iterator associated with this {@code DirectoryStream}
		/// </returns>
		/// <exception cref="IllegalStateException">
		///          if this directory stream is closed or the iterator has already
		///          been returned </exception>
		IEnumerator<T> Iterator();
	}

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @FunctionalInterface public static interface DirectoryStream_Filter<T>
	public interface DirectoryStream_Filter<T>
	{
		/// <summary>
		/// Decides if the given directory entry should be accepted or filtered.
		/// </summary>
		/// <param name="entry">
		///          the directory entry to be tested
		/// </param>
		/// <returns>  {@code true} if the directory entry should be accepted
		/// </returns>
		/// <exception cref="IOException">
		///          If an I/O error occurs </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: boolean accept(T entry) throws java.io.IOException;
		bool Accept(T entry);
	}

}
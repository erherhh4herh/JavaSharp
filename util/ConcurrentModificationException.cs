/*
 * Copyright (c) 1997, 2013, Oracle and/or its affiliates. All rights reserved.
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
	/// This exception may be thrown by methods that have detected concurrent
	/// modification of an object when such modification is not permissible.
	/// <para>
	/// For example, it is not generally permissible for one thread to modify a Collection
	/// while another thread is iterating over it.  In general, the results of the
	/// iteration are undefined under these circumstances.  Some Iterator
	/// implementations (including those of all the general purpose collection implementations
	/// provided by the JRE) may choose to throw this exception if this behavior is
	/// detected.  Iterators that do this are known as <i>fail-fast</i> iterators,
	/// as they fail quickly and cleanly, rather that risking arbitrary,
	/// non-deterministic behavior at an undetermined time in the future.
	/// </para>
	/// <para>
	/// Note that this exception does not always indicate that an object has
	/// been concurrently modified by a <i>different</i> thread.  If a single
	/// thread issues a sequence of method invocations that violates the
	/// contract of an object, the object may throw this exception.  For
	/// example, if a thread modifies a collection directly while it is
	/// iterating over the collection with a fail-fast iterator, the iterator
	/// will throw this exception.
	/// 
	/// </para>
	/// <para>Note that fail-fast behavior cannot be guaranteed as it is, generally
	/// speaking, impossible to make any hard guarantees in the presence of
	/// unsynchronized concurrent modification.  Fail-fast operations
	/// throw {@code ConcurrentModificationException} on a best-effort basis.
	/// Therefore, it would be wrong to write a program that depended on this
	/// exception for its correctness: <i>{@code ConcurrentModificationException}
	/// should be used only to detect bugs.</i>
	/// 
	/// @author  Josh Bloch
	/// </para>
	/// </summary>
	/// <seealso cref=     Collection </seealso>
	/// <seealso cref=     Iterator </seealso>
	/// <seealso cref=     Spliterator </seealso>
	/// <seealso cref=     ListIterator </seealso>
	/// <seealso cref=     Vector </seealso>
	/// <seealso cref=     LinkedList </seealso>
	/// <seealso cref=     HashSet </seealso>
	/// <seealso cref=     Hashtable </seealso>
	/// <seealso cref=     TreeMap </seealso>
	/// <seealso cref=     AbstractList
	/// @since   1.2 </seealso>
	public class ConcurrentModificationException : RuntimeException
	{
		private new const long SerialVersionUID = -3666751008965953603L;

		/// <summary>
		/// Constructs a ConcurrentModificationException with no
		/// detail message.
		/// </summary>
		public ConcurrentModificationException()
		{
		}

		/// <summary>
		/// Constructs a {@code ConcurrentModificationException} with the
		/// specified detail message.
		/// </summary>
		/// <param name="message"> the detail message pertaining to this exception. </param>
		public ConcurrentModificationException(String message) : base(message)
		{
		}

		/// <summary>
		/// Constructs a new exception with the specified cause and a detail
		/// message of {@code (cause==null ? null : cause.toString())} (which
		/// typically contains the class and detail message of {@code cause}.
		/// </summary>
		/// <param name="cause"> the cause (which is saved for later retrieval by the
		///         <seealso cref="Throwable#getCause()"/> method).  (A {@code null} value is
		///         permitted, and indicates that the cause is nonexistent or
		///         unknown.)
		/// @since  1.7 </param>
		public ConcurrentModificationException(Throwable cause) : base(cause)
		{
		}

		/// <summary>
		/// Constructs a new exception with the specified detail message and
		/// cause.
		/// 
		/// <para>Note that the detail message associated with <code>cause</code> is
		/// <i>not</i> automatically incorporated in this exception's detail
		/// message.
		/// 
		/// </para>
		/// </summary>
		/// <param name="message"> the detail message (which is saved for later retrieval
		///         by the <seealso cref="Throwable#getMessage()"/> method). </param>
		/// <param name="cause"> the cause (which is saved for later retrieval by the
		///         <seealso cref="Throwable#getCause()"/> method).  (A {@code null} value
		///         is permitted, and indicates that the cause is nonexistent or
		///         unknown.)
		/// @since 1.7 </param>
		public ConcurrentModificationException(String message, Throwable cause) : base(message, cause)
		{
		}
	}

}
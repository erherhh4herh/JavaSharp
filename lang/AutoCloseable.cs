/*
 * Copyright (c) 2009, 2013, Oracle and/or its affiliates. All rights reserved.
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

namespace java.lang
{

	/// <summary>
	/// An object that may hold resources (such as file or socket handles)
	/// until it is closed. The <seealso cref="#close()"/> method of an {@code AutoCloseable}
	/// object is called automatically when exiting a {@code
	/// try}-with-resources block for which the object has been declared in
	/// the resource specification header. This construction ensures prompt
	/// release, avoiding resource exhaustion exceptions and errors that
	/// may otherwise occur.
	/// 
	/// @apiNote
	/// <para>It is possible, and in fact common, for a base class to
	/// implement AutoCloseable even though not all of its subclasses or
	/// instances will hold releasable resources.  For code that must operate
	/// in complete generality, or when it is known that the {@code AutoCloseable}
	/// instance requires resource release, it is recommended to use {@code
	/// try}-with-resources constructions. However, when using facilities such as
	/// <seealso cref="java.util.stream.Stream"/> that support both I/O-based and
	/// non-I/O-based forms, {@code try}-with-resources blocks are in
	/// general unnecessary when using non-I/O-based forms.
	/// 
	/// @author Josh Bloch
	/// @since 1.7
	/// </para>
	/// </summary>
	public interface AutoCloseable
	{
		/// <summary>
		/// Closes this resource, relinquishing any underlying resources.
		/// This method is invoked automatically on objects managed by the
		/// {@code try}-with-resources statement.
		/// 
		/// <para>While this interface method is declared to throw {@code
		/// Exception}, implementers are <em>strongly</em> encouraged to
		/// declare concrete implementations of the {@code close} method to
		/// throw more specific exceptions, or to throw no exception at all
		/// if the close operation cannot fail.
		/// 
		/// </para>
		/// <para> Cases where the close operation may fail require careful
		/// attention by implementers. It is strongly advised to relinquish
		/// the underlying resources and to internally <em>mark</em> the
		/// resource as closed, prior to throwing the exception. The {@code
		/// close} method is unlikely to be invoked more than once and so
		/// this ensures that the resources are released in a timely manner.
		/// Furthermore it reduces problems that could arise when the resource
		/// wraps, or is wrapped, by another resource.
		/// 
		/// </para>
		/// <para><em>Implementers of this interface are also strongly advised
		/// to not have the {@code close} method throw {@link
		/// InterruptedException}.</em>
		/// 
		/// This exception interacts with a thread's interrupted status,
		/// and runtime misbehavior is likely to occur if an {@code
		/// InterruptedException} is {@link Throwable#addSuppressed
		/// suppressed}.
		/// 
		/// More generally, if it would cause problems for an
		/// exception to be suppressed, the {@code AutoCloseable.close}
		/// method should not throw it.
		/// 
		/// </para>
		/// <para>Note that unlike the <seealso cref="java.io.Closeable#close close"/>
		/// method of <seealso cref="java.io.Closeable"/>, this {@code close} method
		/// is <em>not</em> required to be idempotent.  In other words,
		/// calling this {@code close} method more than once may have some
		/// visible side effect, unlike {@code Closeable.close} which is
		/// required to have no effect if called more than once.
		/// 
		/// However, implementers of this interface are strongly encouraged
		/// to make their {@code close} methods idempotent.
		/// 
		/// </para>
		/// </summary>
		/// <exception cref="Exception"> if this resource cannot be closed </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: void close() throws Exception;
		void Close();
	}

}
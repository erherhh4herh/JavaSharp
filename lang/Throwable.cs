using System;
using System.Diagnostics;
using System.Collections.Generic;
using System.Runtime.InteropServices;

/*
 * Copyright (c) 1994, 2013, Oracle and/or its affiliates. All rights reserved.
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
	/// The {@code Throwable} class is the superclass of all errors and
	/// exceptions in the Java language. Only objects that are instances of this
	/// class (or one of its subclasses) are thrown by the Java Virtual Machine or
	/// can be thrown by the Java {@code throw} statement. Similarly, only
	/// this class or one of its subclasses can be the argument type in a
	/// {@code catch} clause.
	/// 
	/// For the purposes of compile-time checking of exceptions, {@code
	/// Throwable} and any subclass of {@code Throwable} that is not also a
	/// subclass of either <seealso cref="RuntimeException"/> or <seealso cref="Error"/> are
	/// regarded as checked exceptions.
	/// 
	/// <para>Instances of two subclasses, <seealso cref="java.lang.Error"/> and
	/// <seealso cref="java.lang.Exception"/>, are conventionally used to indicate
	/// that exceptional situations have occurred. Typically, these instances
	/// are freshly created in the context of the exceptional situation so
	/// as to include relevant information (such as stack trace data).
	/// 
	/// </para>
	/// <para>A throwable contains a snapshot of the execution stack of its
	/// thread at the time it was created. It can also contain a message
	/// string that gives more information about the error. Over time, a
	/// throwable can <seealso cref="Throwable#addSuppressed suppress"/> other
	/// throwables from being propagated.  Finally, the throwable can also
	/// contain a <i>cause</i>: another throwable that caused this
	/// throwable to be constructed.  The recording of this causal information
	/// is referred to as the <i>chained exception</i> facility, as the
	/// cause can, itself, have a cause, and so on, leading to a "chain" of
	/// exceptions, each caused by another.
	/// 
	/// </para>
	/// <para>One reason that a throwable may have a cause is that the class that
	/// throws it is built atop a lower layered abstraction, and an operation on
	/// the upper layer fails due to a failure in the lower layer.  It would be bad
	/// design to let the throwable thrown by the lower layer propagate outward, as
	/// it is generally unrelated to the abstraction provided by the upper layer.
	/// Further, doing so would tie the API of the upper layer to the details of
	/// its implementation, assuming the lower layer's exception was a checked
	/// exception.  Throwing a "wrapped exception" (i.e., an exception containing a
	/// cause) allows the upper layer to communicate the details of the failure to
	/// its caller without incurring either of these shortcomings.  It preserves
	/// the flexibility to change the implementation of the upper layer without
	/// changing its API (in particular, the set of exceptions thrown by its
	/// methods).
	/// 
	/// </para>
	/// <para>A second reason that a throwable may have a cause is that the method
	/// that throws it must conform to a general-purpose interface that does not
	/// permit the method to throw the cause directly.  For example, suppose
	/// a persistent collection conforms to the {@link java.util.Collection
	/// Collection} interface, and that its persistence is implemented atop
	/// {@code java.io}.  Suppose the internals of the {@code add} method
	/// can throw an <seealso cref="java.io.IOException IOException"/>.  The implementation
	/// can communicate the details of the {@code IOException} to its caller
	/// while conforming to the {@code Collection} interface by wrapping the
	/// {@code IOException} in an appropriate unchecked exception.  (The
	/// specification for the persistent collection should indicate that it is
	/// capable of throwing such exceptions.)
	/// 
	/// </para>
	/// <para>A cause can be associated with a throwable in two ways: via a
	/// constructor that takes the cause as an argument, or via the
	/// <seealso cref="#initCause(Throwable)"/> method.  New throwable classes that
	/// wish to allow causes to be associated with them should provide constructors
	/// that take a cause and delegate (perhaps indirectly) to one of the
	/// {@code Throwable} constructors that takes a cause.
	/// 
	/// Because the {@code initCause} method is public, it allows a cause to be
	/// associated with any throwable, even a "legacy throwable" whose
	/// implementation predates the addition of the exception chaining mechanism to
	/// {@code Throwable}.
	/// 
	/// </para>
	/// <para>By convention, class {@code Throwable} and its subclasses have two
	/// constructors, one that takes no arguments and one that takes a
	/// {@code String} argument that can be used to produce a detail message.
	/// Further, those subclasses that might likely have a cause associated with
	/// them should have two more constructors, one that takes a
	/// {@code Throwable} (the cause), and one that takes a
	/// {@code String} (the detail message) and a {@code Throwable} (the
	/// cause).
	/// 
	/// @author  unascribed
	/// @author  Josh Bloch (Added exception chaining and programmatic access to
	///          stack trace in 1.4.)
	/// @jls 11.2 Compile-Time Checking of Exceptions
	/// @since JDK1.0
	/// </para>
	/// </summary>
	[Serializable]
	public class Throwable
	{
		private bool InstanceFieldsInitialized = false;

		private void InitializeInstanceFields()
		{
			Cause_Renamed = this;
		}

		/// <summary>
		/// use serialVersionUID from JDK 1.0.2 for interoperability </summary>
		private const long SerialVersionUID = -3042686055658047285L;

		/// <summary>
		/// Native code saves some indication of the stack backtrace in this slot.
		/// </summary>
		[NonSerialized]
		private Object Backtrace;

		/// <summary>
		/// Specific details about the Throwable.  For example, for
		/// {@code FileNotFoundException}, this contains the name of
		/// the file that could not be found.
		/// 
		/// @serial
		/// </summary>
		private String DetailMessage;


		/// <summary>
		/// Holder class to defer initializing sentinel objects only used
		/// for serialization.
		/// </summary>
		private class SentinelHolder
		{
			/// <summary>
			/// {@link #setStackTrace(StackTraceElement[]) Setting the
			/// stack trace} to a one-element array containing this sentinel
			/// value indicates future attempts to set the stack trace will be
			/// ignored.  The sentinal is equal to the result of calling:<br>
			/// {@code new StackTraceElement("", "", null, Integer.MIN_VALUE)}
			/// </summary>
			public static readonly StackTraceElement STACK_TRACE_ELEMENT_SENTINEL = new StackTraceElement("", "", null, Integer.MinValue);

			/// <summary>
			/// Sentinel value used in the serial form to indicate an immutable
			/// stack trace.
			/// </summary>
			public static readonly StackTraceElement[] STACK_TRACE_SENTINEL = new StackTraceElement[] {STACK_TRACE_ELEMENT_SENTINEL};
		}

		/// <summary>
		/// A shared value for an empty stack.
		/// </summary>
		private static readonly StackTraceElement[] UNASSIGNED_STACK = new StackTraceElement[0];

		/*
		 * To allow Throwable objects to be made immutable and safely
		 * reused by the JVM, such as OutOfMemoryErrors, fields of
		 * Throwable that are writable in response to user actions, cause,
		 * stackTrace, and suppressedExceptions obey the following
		 * protocol:
		 *
		 * 1) The fields are initialized to a non-null sentinel value
		 * which indicates the value has logically not been set.
		 *
		 * 2) Writing a null to the field indicates further writes
		 * are forbidden
		 *
		 * 3) The sentinel value may be replaced with another non-null
		 * value.
		 *
		 * For example, implementations of the HotSpot JVM have
		 * preallocated OutOfMemoryError objects to provide for better
		 * diagnosability of that situation.  These objects are created
		 * without calling the constructor for that class and the fields
		 * in question are initialized to null.  To support this
		 * capability, any new fields added to Throwable that require
		 * being initialized to a non-null value require a coordinated JVM
		 * change.
		 */

		/// <summary>
		/// The throwable that caused this throwable to get thrown, or null if this
		/// throwable was not caused by another throwable, or if the causative
		/// throwable is unknown.  If this field is equal to this throwable itself,
		/// it indicates that the cause of this throwable has not yet been
		/// initialized.
		/// 
		/// @serial
		/// @since 1.4
		/// </summary>
		private Throwable Cause_Renamed;

		/// <summary>
		/// The stack trace, as returned by <seealso cref="#getStackTrace()"/>.
		/// 
		/// The field is initialized to a zero-length array.  A {@code
		/// null} value of this field indicates subsequent calls to {@link
		/// #setStackTrace(StackTraceElement[])} and {@link
		/// #fillInStackTrace()} will be be no-ops.
		/// 
		/// @serial
		/// @since 1.4
		/// </summary>
		private StackTraceElement[] StackTrace_Renamed = UNASSIGNED_STACK;

		// Setting this static field introduces an acceptable
		// initialization dependency on a few java.util classes.
		private static readonly List<Throwable> SUPPRESSED_SENTINEL = Collections.UnmodifiableList(new List<Throwable>(0));

		/// <summary>
		/// The list of suppressed exceptions, as returned by {@link
		/// #getSuppressed()}.  The list is initialized to a zero-element
		/// unmodifiable sentinel list.  When a serialized Throwable is
		/// read in, if the {@code suppressedExceptions} field points to a
		/// zero-element list, the field is reset to the sentinel value.
		/// 
		/// @serial
		/// @since 1.7
		/// </summary>
		private List<Throwable> SuppressedExceptions = SUPPRESSED_SENTINEL;

		/// <summary>
		/// Message for trying to suppress a null exception. </summary>
		private const String NULL_CAUSE_MESSAGE = "Cannot suppress a null exception.";

		/// <summary>
		/// Message for trying to suppress oneself. </summary>
		private const String SELF_SUPPRESSION_MESSAGE = "Self-suppression not permitted";

		/// <summary>
		/// Caption  for labeling causative exception stack traces </summary>
		private const String CAUSE_CAPTION = "Caused by: ";

		/// <summary>
		/// Caption for labeling suppressed exception stack traces </summary>
		private const String SUPPRESSED_CAPTION = "Suppressed: ";

		/// <summary>
		/// Constructs a new throwable with {@code null} as its detail message.
		/// The cause is not initialized, and may subsequently be initialized by a
		/// call to <seealso cref="#initCause"/>.
		/// 
		/// <para>The <seealso cref="#fillInStackTrace()"/> method is called to initialize
		/// the stack trace data in the newly created throwable.
		/// </para>
		/// </summary>
		public Throwable()
		{
			if (!InstanceFieldsInitialized)
			{
				InitializeInstanceFields();
				InstanceFieldsInitialized = true;
			}
			FillInStackTrace();
		}

		/// <summary>
		/// Constructs a new throwable with the specified detail message.  The
		/// cause is not initialized, and may subsequently be initialized by
		/// a call to <seealso cref="#initCause"/>.
		/// 
		/// <para>The <seealso cref="#fillInStackTrace()"/> method is called to initialize
		/// the stack trace data in the newly created throwable.
		/// 
		/// </para>
		/// </summary>
		/// <param name="message">   the detail message. The detail message is saved for
		///          later retrieval by the <seealso cref="#getMessage()"/> method. </param>
		public Throwable(String message)
		{
			if (!InstanceFieldsInitialized)
			{
				InitializeInstanceFields();
				InstanceFieldsInitialized = true;
			}
			FillInStackTrace();
			DetailMessage = message;
		}

		/// <summary>
		/// Constructs a new throwable with the specified detail message and
		/// cause.  <para>Note that the detail message associated with
		/// {@code cause} is <i>not</i> automatically incorporated in
		/// this throwable's detail message.
		/// 
		/// </para>
		/// <para>The <seealso cref="#fillInStackTrace()"/> method is called to initialize
		/// the stack trace data in the newly created throwable.
		/// 
		/// </para>
		/// </summary>
		/// <param name="message"> the detail message (which is saved for later retrieval
		///         by the <seealso cref="#getMessage()"/> method). </param>
		/// <param name="cause"> the cause (which is saved for later retrieval by the
		///         <seealso cref="#getCause()"/> method).  (A {@code null} value is
		///         permitted, and indicates that the cause is nonexistent or
		///         unknown.)
		/// @since  1.4 </param>
		public Throwable(String message, Throwable cause)
		{
			if (!InstanceFieldsInitialized)
			{
				InitializeInstanceFields();
				InstanceFieldsInitialized = true;
			}
			FillInStackTrace();
			DetailMessage = message;
			this.Cause_Renamed = cause;
		}

		/// <summary>
		/// Constructs a new throwable with the specified cause and a detail
		/// message of {@code (cause==null ? null : cause.toString())} (which
		/// typically contains the class and detail message of {@code cause}).
		/// This constructor is useful for throwables that are little more than
		/// wrappers for other throwables (for example, {@link
		/// java.security.PrivilegedActionException}).
		/// 
		/// <para>The <seealso cref="#fillInStackTrace()"/> method is called to initialize
		/// the stack trace data in the newly created throwable.
		/// 
		/// </para>
		/// </summary>
		/// <param name="cause"> the cause (which is saved for later retrieval by the
		///         <seealso cref="#getCause()"/> method).  (A {@code null} value is
		///         permitted, and indicates that the cause is nonexistent or
		///         unknown.)
		/// @since  1.4 </param>
		public Throwable(Throwable cause)
		{
			if (!InstanceFieldsInitialized)
			{
				InitializeInstanceFields();
				InstanceFieldsInitialized = true;
			}
			FillInStackTrace();
			DetailMessage = (cause == null ? null : cause.ToString());
			this.Cause_Renamed = cause;
		}

		/// <summary>
		/// Constructs a new throwable with the specified detail message,
		/// cause, <seealso cref="#addSuppressed suppression"/> enabled or
		/// disabled, and writable stack trace enabled or disabled.  If
		/// suppression is disabled, <seealso cref="#getSuppressed"/> for this object
		/// will return a zero-length array and calls to {@link
		/// #addSuppressed} that would otherwise append an exception to the
		/// suppressed list will have no effect.  If the writable stack
		/// trace is false, this constructor will not call {@link
		/// #fillInStackTrace()}, a {@code null} will be written to the
		/// {@code stackTrace} field, and subsequent calls to {@code
		/// fillInStackTrace} and {@link
		/// #setStackTrace(StackTraceElement[])} will not set the stack
		/// trace.  If the writable stack trace is false, {@link
		/// #getStackTrace} will return a zero length array.
		/// 
		/// <para>Note that the other constructors of {@code Throwable} treat
		/// suppression as being enabled and the stack trace as being
		/// writable.  Subclasses of {@code Throwable} should document any
		/// conditions under which suppression is disabled and document
		/// conditions under which the stack trace is not writable.
		/// Disabling of suppression should only occur in exceptional
		/// circumstances where special requirements exist, such as a
		/// virtual machine reusing exception objects under low-memory
		/// situations.  Circumstances where a given exception object is
		/// repeatedly caught and rethrown, such as to implement control
		/// flow between two sub-systems, is another situation where
		/// immutable throwable objects would be appropriate.
		/// 
		/// </para>
		/// </summary>
		/// <param name="message"> the detail message. </param>
		/// <param name="cause"> the cause.  (A {@code null} value is permitted,
		/// and indicates that the cause is nonexistent or unknown.) </param>
		/// <param name="enableSuppression"> whether or not suppression is enabled or disabled </param>
		/// <param name="writableStackTrace"> whether or not the stack trace should be
		///                           writable
		/// </param>
		/// <seealso cref= OutOfMemoryError </seealso>
		/// <seealso cref= NullPointerException </seealso>
		/// <seealso cref= ArithmeticException
		/// @since 1.7 </seealso>
		protected internal Throwable(String message, Throwable cause, bool enableSuppression, bool writableStackTrace)
		{
			if (!InstanceFieldsInitialized)
			{
				InitializeInstanceFields();
				InstanceFieldsInitialized = true;
			}
			if (writableStackTrace)
			{
				FillInStackTrace();
			}
			else
			{
				StackTrace_Renamed = null;
			}
			DetailMessage = message;
			this.Cause_Renamed = cause;
			if (!enableSuppression)
			{
				SuppressedExceptions = null;
			}
		}

		/// <summary>
		/// Returns the detail message string of this throwable.
		/// </summary>
		/// <returns>  the detail message string of this {@code Throwable} instance
		///          (which may be {@code null}). </returns>
		public virtual String Message
		{
			get
			{
				return DetailMessage;
			}
		}

		/// <summary>
		/// Creates a localized description of this throwable.
		/// Subclasses may override this method in order to produce a
		/// locale-specific message.  For subclasses that do not override this
		/// method, the default implementation returns the same result as
		/// {@code getMessage()}.
		/// </summary>
		/// <returns>  The localized description of this throwable.
		/// @since   JDK1.1 </returns>
		public virtual String LocalizedMessage
		{
			get
			{
				return Message;
			}
		}

		/// <summary>
		/// Returns the cause of this throwable or {@code null} if the
		/// cause is nonexistent or unknown.  (The cause is the throwable that
		/// caused this throwable to get thrown.)
		/// 
		/// <para>This implementation returns the cause that was supplied via one of
		/// the constructors requiring a {@code Throwable}, or that was set after
		/// creation with the <seealso cref="#initCause(Throwable)"/> method.  While it is
		/// typically unnecessary to override this method, a subclass can override
		/// it to return a cause set by some other means.  This is appropriate for
		/// a "legacy chained throwable" that predates the addition of chained
		/// exceptions to {@code Throwable}.  Note that it is <i>not</i>
		/// necessary to override any of the {@code PrintStackTrace} methods,
		/// all of which invoke the {@code getCause} method to determine the
		/// cause of a throwable.
		/// 
		/// </para>
		/// </summary>
		/// <returns>  the cause of this throwable or {@code null} if the
		///          cause is nonexistent or unknown.
		/// @since 1.4 </returns>
		public virtual Throwable Cause
		{
			get
			{
				lock (this)
				{
					return (Cause_Renamed == this ? null : Cause_Renamed);
				}
			}
		}

		/// <summary>
		/// Initializes the <i>cause</i> of this throwable to the specified value.
		/// (The cause is the throwable that caused this throwable to get thrown.)
		/// 
		/// <para>This method can be called at most once.  It is generally called from
		/// within the constructor, or immediately after creating the
		/// throwable.  If this throwable was created
		/// with <seealso cref="#Throwable(Throwable)"/> or
		/// <seealso cref="#Throwable(String,Throwable)"/>, this method cannot be called
		/// even once.
		/// 
		/// </para>
		/// <para>An example of using this method on a legacy throwable type
		/// without other support for setting the cause is:
		/// 
		/// <pre>
		/// try {
		///     lowLevelOp();
		/// } catch (LowLevelException le) {
		///     throw (HighLevelException)
		///           new HighLevelException().initCause(le); // Legacy constructor
		/// }
		/// </pre>
		/// 
		/// </para>
		/// </summary>
		/// <param name="cause"> the cause (which is saved for later retrieval by the
		///         <seealso cref="#getCause()"/> method).  (A {@code null} value is
		///         permitted, and indicates that the cause is nonexistent or
		///         unknown.) </param>
		/// <returns>  a reference to this {@code Throwable} instance. </returns>
		/// <exception cref="IllegalArgumentException"> if {@code cause} is this
		///         throwable.  (A throwable cannot be its own cause.) </exception>
		/// <exception cref="IllegalStateException"> if this throwable was
		///         created with <seealso cref="#Throwable(Throwable)"/> or
		///         <seealso cref="#Throwable(String,Throwable)"/>, or this method has already
		///         been called on this throwable.
		/// @since  1.4 </exception>
		public virtual Throwable InitCause(Throwable cause)
		{
			lock (this)
			{
				if (this.Cause_Renamed != this)
				{
					throw new IllegalStateException("Can't overwrite cause with " + Objects.ToString(cause, "a null"), this);
				}
				if (cause == this)
				{
					throw new IllegalArgumentException("Self-causation not permitted", this);
				}
				this.Cause_Renamed = cause;
				return this;
			}
		}

		/// <summary>
		/// Returns a short description of this throwable.
		/// The result is the concatenation of:
		/// <ul>
		/// <li> the <seealso cref="Class#getName() name"/> of the class of this object
		/// <li> ": " (a colon and a space)
		/// <li> the result of invoking this object's <seealso cref="#getLocalizedMessage"/>
		///      method
		/// </ul>
		/// If {@code getLocalizedMessage} returns {@code null}, then just
		/// the class name is returned.
		/// </summary>
		/// <returns> a string representation of this throwable. </returns>
		public override String ToString()
		{
//JAVA TO C# CONVERTER WARNING: The .NET Type.FullName property will not always yield results identical to the Java Class.getName method:
			String s = this.GetType().FullName;
			String message = LocalizedMessage;
			return (message != null) ? (s + ": " + message) : s;
		}

		/// <summary>
		/// Prints this throwable and its backtrace to the
		/// standard error stream. This method prints a stack trace for this
		/// {@code Throwable} object on the error output stream that is
		/// the value of the field {@code System.err}. The first line of
		/// output contains the result of the <seealso cref="#toString()"/> method for
		/// this object.  Remaining lines represent data previously recorded by
		/// the method <seealso cref="#fillInStackTrace()"/>. The format of this
		/// information depends on the implementation, but the following
		/// example may be regarded as typical:
		/// <blockquote><pre>
		/// java.lang.NullPointerException
		///         at MyClass.mash(MyClass.java:9)
		///         at MyClass.crunch(MyClass.java:6)
		///         at MyClass.main(MyClass.java:3)
		/// </pre></blockquote>
		/// This example was produced by running the program:
		/// <pre>
		/// class MyClass {
		///     public static void main(String[] args) {
		///         crunch(null);
		///     }
		///     static void crunch(int[] a) {
		///         mash(a);
		///     }
		///     static void mash(int[] b) {
		///         System.out.println(b[0]);
		///     }
		/// }
		/// </pre>
		/// The backtrace for a throwable with an initialized, non-null cause
		/// should generally include the backtrace for the cause.  The format
		/// of this information depends on the implementation, but the following
		/// example may be regarded as typical:
		/// <pre>
		/// HighLevelException: MidLevelException: LowLevelException
		///         at Junk.a(Junk.java:13)
		///         at Junk.main(Junk.java:4)
		/// Caused by: MidLevelException: LowLevelException
		///         at Junk.c(Junk.java:23)
		///         at Junk.b(Junk.java:17)
		///         at Junk.a(Junk.java:11)
		///         ... 1 more
		/// Caused by: LowLevelException
		///         at Junk.e(Junk.java:30)
		///         at Junk.d(Junk.java:27)
		///         at Junk.c(Junk.java:21)
		///         ... 3 more
		/// </pre>
		/// Note the presence of lines containing the characters {@code "..."}.
		/// These lines indicate that the remainder of the stack trace for this
		/// exception matches the indicated number of frames from the bottom of the
		/// stack trace of the exception that was caused by this exception (the
		/// "enclosing" exception).  This shorthand can greatly reduce the length
		/// of the output in the common case where a wrapped exception is thrown
		/// from same method as the "causative exception" is caught.  The above
		/// example was produced by running the program:
		/// <pre>
		/// public class Junk {
		///     public static void main(String args[]) {
		///         try {
		///             a();
		///         } catch(HighLevelException e) {
		///             e.printStackTrace();
		///         }
		///     }
		///     static void a() throws HighLevelException {
		///         try {
		///             b();
		///         } catch(MidLevelException e) {
		///             throw new HighLevelException(e);
		///         }
		///     }
		///     static void b() throws MidLevelException {
		///         c();
		///     }
		///     static void c() throws MidLevelException {
		///         try {
		///             d();
		///         } catch(LowLevelException e) {
		///             throw new MidLevelException(e);
		///         }
		///     }
		///     static void d() throws LowLevelException {
		///        e();
		///     }
		///     static void e() throws LowLevelException {
		///         throw new LowLevelException();
		///     }
		/// }
		/// 
		/// class HighLevelException extends Exception {
		///     HighLevelException(Throwable cause) { super(cause); }
		/// }
		/// 
		/// class MidLevelException extends Exception {
		///     MidLevelException(Throwable cause)  { super(cause); }
		/// }
		/// 
		/// class LowLevelException extends Exception {
		/// }
		/// </pre>
		/// As of release 7, the platform supports the notion of
		/// <i>suppressed exceptions</i> (in conjunction with the {@code
		/// try}-with-resources statement). Any exceptions that were
		/// suppressed in order to deliver an exception are printed out
		/// beneath the stack trace.  The format of this information
		/// depends on the implementation, but the following example may be
		/// regarded as typical:
		/// 
		/// <pre>
		/// Exception in thread "main" java.lang.Exception: Something happened
		///  at Foo.bar(Foo.java:10)
		///  at Foo.main(Foo.java:5)
		///  Suppressed: Resource$CloseFailException: Resource ID = 0
		///          at Resource.close(Resource.java:26)
		///          at Foo.bar(Foo.java:9)
		///          ... 1 more
		/// </pre>
		/// Note that the "... n more" notation is used on suppressed exceptions
		/// just at it is used on causes. Unlike causes, suppressed exceptions are
		/// indented beyond their "containing exceptions."
		/// 
		/// <para>An exception can have both a cause and one or more suppressed
		/// exceptions:
		/// <pre>
		/// Exception in thread "main" java.lang.Exception: Main block
		///  at Foo3.main(Foo3.java:7)
		///  Suppressed: Resource$CloseFailException: Resource ID = 2
		///          at Resource.close(Resource.java:26)
		///          at Foo3.main(Foo3.java:5)
		///  Suppressed: Resource$CloseFailException: Resource ID = 1
		///          at Resource.close(Resource.java:26)
		///          at Foo3.main(Foo3.java:5)
		/// Caused by: java.lang.Exception: I did it
		///  at Foo3.main(Foo3.java:8)
		/// </pre>
		/// Likewise, a suppressed exception can have a cause:
		/// <pre>
		/// Exception in thread "main" java.lang.Exception: Main block
		///  at Foo4.main(Foo4.java:6)
		///  Suppressed: Resource2$CloseFailException: Resource ID = 1
		///          at Resource2.close(Resource2.java:20)
		///          at Foo4.main(Foo4.java:5)
		///  Caused by: java.lang.Exception: Rats, you caught me
		///          at Resource2$CloseFailException.&lt;init&gt;(Resource2.java:45)
		///          ... 2 more
		/// </pre>
		/// </para>
		/// </summary>
		public virtual void PrintStackTrace()
		{
			PrintStackTrace(System.err);
		}

		/// <summary>
		/// Prints this throwable and its backtrace to the specified print stream.
		/// </summary>
		/// <param name="s"> {@code PrintStream} to use for output </param>
		public virtual void PrintStackTrace(PrintStream s)
		{
			PrintStackTrace(new WrappedPrintStream(s));
		}

		private void PrintStackTrace(PrintStreamOrWriter s)
		{
			// Guard against malicious overrides of Throwable.equals by
			// using a Set with identity equality semantics.
			Set<Throwable> dejaVu = Collections.NewSetFromMap(new IdentityHashMap<Throwable, Boolean>());
			dejaVu.Add(this);

			lock (s.@lock())
			{
				// Print our stack trace
				s.Println(this);
				StackTraceElement[] trace = OurStackTrace;
				foreach (StackTraceElement traceElement in trace)
				{
					s.Println("\tat " + traceElement);
				}

				// Print suppressed exceptions, if any
				foreach (Throwable se in Suppressed)
				{
					se.PrintEnclosedStackTrace(s, trace, SUPPRESSED_CAPTION, "\t", dejaVu);
				}

				// Print cause, if any
				Throwable ourCause = Cause;
				if (ourCause != null)
				{
					ourCause.PrintEnclosedStackTrace(s, trace, CAUSE_CAPTION, "", dejaVu);
				}
			}
		}

		/// <summary>
		/// Print our stack trace as an enclosed exception for the specified
		/// stack trace.
		/// </summary>
		private void PrintEnclosedStackTrace(PrintStreamOrWriter s, StackTraceElement[] enclosingTrace, String caption, String prefix, Set<Throwable> dejaVu)
		{
			Debug.Assert(Thread.holdsLock(s.@lock()));
			if (dejaVu.Contains(this))
			{
				s.Println("\t[CIRCULAR REFERENCE:" + this + "]");
			}
			else
			{
				dejaVu.Add(this);
				// Compute number of frames in common between this and enclosing trace
				StackTraceElement[] trace = OurStackTrace;
				int m = trace.Length - 1;
				int n = enclosingTrace.Length - 1;
				while (m >= 0 && n >= 0 && trace[m].Equals(enclosingTrace[n]))
				{
					m--;
					n--;
				}
				int framesInCommon = trace.Length - 1 - m;

				// Print our stack trace
				s.Println(prefix + caption + this);
				for (int i = 0; i <= m; i++)
				{
					s.Println(prefix + "\tat " + trace[i]);
				}
				if (framesInCommon != 0)
				{
					s.Println(prefix + "\t... " + framesInCommon + " more");
				}

				// Print suppressed exceptions, if any
				foreach (Throwable se in Suppressed)
				{
					se.PrintEnclosedStackTrace(s, trace, SUPPRESSED_CAPTION, prefix + "\t", dejaVu);
				}

				// Print cause, if any
				Throwable ourCause = Cause;
				if (ourCause != null)
				{
					ourCause.PrintEnclosedStackTrace(s, trace, CAUSE_CAPTION, prefix, dejaVu);
				}
			}
		}

		/// <summary>
		/// Prints this throwable and its backtrace to the specified
		/// print writer.
		/// </summary>
		/// <param name="s"> {@code PrintWriter} to use for output
		/// @since   JDK1.1 </param>
		public virtual void PrintStackTrace(PrintWriter s)
		{
			PrintStackTrace(new WrappedPrintWriter(s));
		}

		/// <summary>
		/// Wrapper class for PrintStream and PrintWriter to enable a single
		/// implementation of printStackTrace.
		/// </summary>
		private abstract class PrintStreamOrWriter
		{
			/// <summary>
			/// Returns the object to be locked when using this StreamOrWriter </summary>
			internal abstract Object @lock();

			/// <summary>
			/// Prints the specified string as a line on this StreamOrWriter </summary>
			internal abstract void Println(Object o);
		}

		private class WrappedPrintStream : PrintStreamOrWriter
		{
			internal readonly PrintStream PrintStream;

			internal WrappedPrintStream(PrintStream printStream)
			{
				this.PrintStream = printStream;
			}

			internal override Object @lock()
			{
				return PrintStream;
			}

			internal override void Println(Object o)
			{
				PrintStream.Println(o);
			}
		}

		private class WrappedPrintWriter : PrintStreamOrWriter
		{
			internal readonly PrintWriter PrintWriter;

			internal WrappedPrintWriter(PrintWriter printWriter)
			{
				this.PrintWriter = printWriter;
			}

			internal override Object @lock()
			{
				return PrintWriter;
			}

			internal override void Println(Object o)
			{
				PrintWriter.Println(o);
			}
		}

		/// <summary>
		/// Fills in the execution stack trace. This method records within this
		/// {@code Throwable} object information about the current state of
		/// the stack frames for the current thread.
		/// 
		/// <para>If the stack trace of this {@code Throwable} {@linkplain
		/// Throwable#Throwable(String, Throwable, boolean, boolean) is not
		/// writable}, calling this method has no effect.
		/// 
		/// </para>
		/// </summary>
		/// <returns>  a reference to this {@code Throwable} instance. </returns>
		/// <seealso cref=     java.lang.Throwable#printStackTrace() </seealso>
		public virtual Throwable FillInStackTrace()
		{
			lock (this)
			{
				if (StackTrace_Renamed != null || Backtrace != null) // Out of protocol state
				{
					fillInStackTrace(0);
					StackTrace_Renamed = UNASSIGNED_STACK;
				}
				return this;
			}
		}

//JAVA TO C# CONVERTER TODO TASK: Replace 'unknown' with the appropriate dll name:
		[DllImport("unknown")]
		private extern Throwable fillInStackTrace(int dummy);

		/// <summary>
		/// Provides programmatic access to the stack trace information printed by
		/// <seealso cref="#printStackTrace()"/>.  Returns an array of stack trace elements,
		/// each representing one stack frame.  The zeroth element of the array
		/// (assuming the array's length is non-zero) represents the top of the
		/// stack, which is the last method invocation in the sequence.  Typically,
		/// this is the point at which this throwable was created and thrown.
		/// The last element of the array (assuming the array's length is non-zero)
		/// represents the bottom of the stack, which is the first method invocation
		/// in the sequence.
		/// 
		/// <para>Some virtual machines may, under some circumstances, omit one
		/// or more stack frames from the stack trace.  In the extreme case,
		/// a virtual machine that has no stack trace information concerning
		/// this throwable is permitted to return a zero-length array from this
		/// method.  Generally speaking, the array returned by this method will
		/// contain one element for every frame that would be printed by
		/// {@code printStackTrace}.  Writes to the returned array do not
		/// affect future calls to this method.
		/// 
		/// </para>
		/// </summary>
		/// <returns> an array of stack trace elements representing the stack trace
		///         pertaining to this throwable.
		/// @since  1.4 </returns>
		public virtual StackTraceElement[] StackTrace
		{
			get
			{
				return OurStackTrace.clone();
			}
			set
			{
				// Validate argument
				StackTraceElement[] defensiveCopy = value.clone();
				for (int i = 0; i < defensiveCopy.Length; i++)
				{
					if (defensiveCopy[i] == null)
					{
						throw new NullPointerException("stackTrace[" + i + "]");
					}
				}
    
				lock (this)
				{
					if (this.StackTrace_Renamed == null && Backtrace == null) // Test for out of protocol state -  Immutable stack
					{
						return;
					}
					this.StackTrace_Renamed = defensiveCopy;
				}
			}
		}

		private StackTraceElement[] OurStackTrace
		{
			get
			{
				lock (this)
				{
					// Initialize stack trace field with information from
					// backtrace if this is the first call to this method
					if (StackTrace_Renamed == UNASSIGNED_STACK || (StackTrace_Renamed == null && Backtrace != null)) // Out of protocol state
					{
						int depth = StackTraceDepth;
						StackTrace_Renamed = new StackTraceElement[depth];
						for (int i = 0; i < depth; i++)
						{
							StackTrace_Renamed[i] = getStackTraceElement(i);
						}
					}
					else if (StackTrace_Renamed == null)
					{
						return UNASSIGNED_STACK;
					}
					return StackTrace_Renamed;
				}
			}
		}


		/// <summary>
		/// Returns the number of elements in the stack trace (or 0 if the stack
		/// trace is unavailable).
		/// 
		/// package-protection for use by SharedSecrets.
		/// </summary>
//JAVA TO C# CONVERTER TODO TASK: Replace 'unknown' with the appropriate dll name:
		[DllImport("unknown")]
		internal extern int getStackTraceDepth();

		/// <summary>
		/// Returns the specified element of the stack trace.
		/// 
		/// package-protection for use by SharedSecrets.
		/// </summary>
		/// <param name="index"> index of the element to return. </param>
		/// <exception cref="IndexOutOfBoundsException"> if {@code index < 0 ||
		///         index >= getStackTraceDepth() } </exception>
//JAVA TO C# CONVERTER TODO TASK: Replace 'unknown' with the appropriate dll name:
		[DllImport("unknown")]
		internal extern StackTraceElement getStackTraceElement(int index);

		/// <summary>
		/// Reads a {@code Throwable} from a stream, enforcing
		/// well-formedness constraints on fields.  Null entries and
		/// self-pointers are not allowed in the list of {@code
		/// suppressedExceptions}.  Null entries are not allowed for stack
		/// trace elements.  A null stack trace in the serial form results
		/// in a zero-length stack element array. A single-element stack
		/// trace whose entry is equal to {@code new StackTraceElement("",
		/// "", null, Integer.MIN_VALUE)} results in a {@code null} {@code
		/// stackTrace} field.
		/// 
		/// Note that there are no constraints on the value the {@code
		/// cause} field can hold; both {@code null} and {@code this} are
		/// valid values for the field.
		/// </summary>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: private void readObject(ObjectInputStream s) throws IOException, ClassNotFoundException
		private void ReadObject(ObjectInputStream s)
		{
			s.DefaultReadObject(); // read in all fields
			if (SuppressedExceptions != null)
			{
				List<Throwable> suppressed = null;
				if (SuppressedExceptions.Count == 0)
				{
					// Use the sentinel for a zero-length list
					suppressed = SUPPRESSED_SENTINEL;
				} // Copy Throwables to new list
				else
				{
					suppressed = new List<>(1);
					foreach (Throwable t in SuppressedExceptions)
					{
						// Enforce constraints on suppressed exceptions in
						// case of corrupt or malicious stream.
						if (t == null)
						{
							throw new NullPointerException(NULL_CAUSE_MESSAGE);
						}
						if (t == this)
						{
							throw new IllegalArgumentException(SELF_SUPPRESSION_MESSAGE);
						}
						suppressed.Add(t);
					}
				}
				SuppressedExceptions = suppressed;
			} // else a null suppressedExceptions field remains null

			/*
			 * For zero-length stack traces, use a clone of
			 * UNASSIGNED_STACK rather than UNASSIGNED_STACK itself to
			 * allow identity comparison against UNASSIGNED_STACK in
			 * getOurStackTrace.  The identity of UNASSIGNED_STACK in
			 * stackTrace indicates to the getOurStackTrace method that
			 * the stackTrace needs to be constructed from the information
			 * in backtrace.
			 */
			if (StackTrace_Renamed != null)
			{
				if (StackTrace_Renamed.Length == 0)
				{
					StackTrace_Renamed = UNASSIGNED_STACK.clone();
				}
				else if (StackTrace_Renamed.Length == 1 && SentinelHolder.STACK_TRACE_ELEMENT_SENTINEL.Equals(StackTrace_Renamed[0]))
				{
							// Check for the marker of an immutable stack trace
					StackTrace_Renamed = null;
				} // Verify stack trace elements are non-null.
				else
				{
					foreach (StackTraceElement ste in StackTrace_Renamed)
					{
						if (ste == null)
						{
							throw new NullPointerException("null StackTraceElement in serial stream. ");
						}
					}
				}
			}
			else
			{
				// A null stackTrace field in the serial form can result
				// from an exception serialized without that field in
				// older JDK releases; treat such exceptions as having
				// empty stack traces.
				StackTrace_Renamed = UNASSIGNED_STACK.clone();
			}
		}

		/// <summary>
		/// Write a {@code Throwable} object to a stream.
		/// 
		/// A {@code null} stack trace field is represented in the serial
		/// form as a one-element array whose element is equal to {@code
		/// new StackTraceElement("", "", null, Integer.MIN_VALUE)}.
		/// </summary>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: private synchronized void writeObject(ObjectOutputStream s) throws IOException
		private void WriteObject(ObjectOutputStream s)
		{
			lock (this)
			{
				// Ensure that the stackTrace field is initialized to a
				// non-null value, if appropriate.  As of JDK 7, a null stack
				// trace field is a valid value indicating the stack trace
				// should not be set.
				OurStackTrace;
        
				StackTraceElement[] oldStackTrace = StackTrace_Renamed;
				try
				{
					if (StackTrace_Renamed == null)
					{
						StackTrace_Renamed = SentinelHolder.STACK_TRACE_SENTINEL;
					}
					s.DefaultWriteObject();
				}
				finally
				{
					StackTrace_Renamed = oldStackTrace;
				}
			}
		}

		/// <summary>
		/// Appends the specified exception to the exceptions that were
		/// suppressed in order to deliver this exception. This method is
		/// thread-safe and typically called (automatically and implicitly)
		/// by the {@code try}-with-resources statement.
		/// 
		/// <para>The suppression behavior is enabled <em>unless</em> disabled
		/// {@link #Throwable(String, Throwable, boolean, boolean) via
		/// a constructor}.  When suppression is disabled, this method does
		/// nothing other than to validate its argument.
		/// 
		/// </para>
		/// <para>Note that when one exception {@linkplain
		/// #initCause(Throwable) causes} another exception, the first
		/// exception is usually caught and then the second exception is
		/// thrown in response.  In other words, there is a causal
		/// connection between the two exceptions.
		/// 
		/// In contrast, there are situations where two independent
		/// exceptions can be thrown in sibling code blocks, in particular
		/// in the {@code try} block of a {@code try}-with-resources
		/// statement and the compiler-generated {@code finally} block
		/// which closes the resource.
		/// 
		/// In these situations, only one of the thrown exceptions can be
		/// propagated.  In the {@code try}-with-resources statement, when
		/// there are two such exceptions, the exception originating from
		/// the {@code try} block is propagated and the exception from the
		/// {@code finally} block is added to the list of exceptions
		/// suppressed by the exception from the {@code try} block.  As an
		/// exception unwinds the stack, it can accumulate multiple
		/// suppressed exceptions.
		/// 
		/// </para>
		/// <para>An exception may have suppressed exceptions while also being
		/// caused by another exception.  Whether or not an exception has a
		/// cause is semantically known at the time of its creation, unlike
		/// whether or not an exception will suppress other exceptions
		/// which is typically only determined after an exception is
		/// thrown.
		/// 
		/// </para>
		/// <para>Note that programmer written code is also able to take
		/// advantage of calling this method in situations where there are
		/// multiple sibling exceptions and only one can be propagated.
		/// 
		/// </para>
		/// </summary>
		/// <param name="exception"> the exception to be added to the list of
		///        suppressed exceptions </param>
		/// <exception cref="IllegalArgumentException"> if {@code exception} is this
		///         throwable; a throwable cannot suppress itself. </exception>
		/// <exception cref="NullPointerException"> if {@code exception} is {@code null}
		/// @since 1.7 </exception>
		public void AddSuppressed(Throwable exception)
		{
			lock (this)
			{
				if (exception == this)
				{
					throw new IllegalArgumentException(SELF_SUPPRESSION_MESSAGE, exception);
				}
        
				if (exception == null)
				{
					throw new NullPointerException(NULL_CAUSE_MESSAGE);
				}
        
				if (SuppressedExceptions == null) // Suppressed exceptions not recorded
				{
					return;
				}
        
				if (SuppressedExceptions == SUPPRESSED_SENTINEL)
				{
					SuppressedExceptions = new List<>(1);
				}
        
				SuppressedExceptions.Add(exception);
			}
		}

		private static readonly Throwable[] EMPTY_THROWABLE_ARRAY = new Throwable[0];

		/// <summary>
		/// Returns an array containing all of the exceptions that were
		/// suppressed, typically by the {@code try}-with-resources
		/// statement, in order to deliver this exception.
		/// 
		/// If no exceptions were suppressed or {@linkplain
		/// #Throwable(String, Throwable, boolean, boolean) suppression is
		/// disabled}, an empty array is returned.  This method is
		/// thread-safe.  Writes to the returned array do not affect future
		/// calls to this method.
		/// </summary>
		/// <returns> an array containing all of the exceptions that were
		///         suppressed to deliver this exception.
		/// @since 1.7 </returns>
		public Throwable[] Suppressed
		{
			get
			{
				lock (this)
				{
					if (SuppressedExceptions == SUPPRESSED_SENTINEL || SuppressedExceptions == null)
					{
						return EMPTY_THROWABLE_ARRAY;
					}
					else
					{
						return SuppressedExceptions.ToArray(EMPTY_THROWABLE_ARRAY);
					}
				}
			}
		}
	}

}
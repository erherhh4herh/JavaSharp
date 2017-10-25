using System;
using System.Threading;

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

namespace java.awt.@event
{

	using AWTAccessor = sun.awt.AWTAccessor;


	/// <summary>
	/// An event which executes the <code>run()</code> method on a <code>Runnable
	/// </code> when dispatched by the AWT event dispatcher thread. This class can
	/// be used as a reference implementation of <code>ActiveEvent</code> rather
	/// than declaring a new class and defining <code>dispatch()</code>.<para>
	/// 
	/// Instances of this class are placed on the <code>EventQueue</code> by calls
	/// to <code>invokeLater</code> and <code>invokeAndWait</code>. Client code
	/// can use this fact to write replacement functions for <code>invokeLater
	/// </code> and <code>invokeAndWait</code> without writing special-case code
	/// in any <code>AWTEventListener</code> objects.
	/// </para>
	/// <para>
	/// An unspecified behavior will be caused if the {@code id} parameter
	/// of any particular {@code InvocationEvent} instance is not
	/// in the range from {@code INVOCATION_FIRST} to {@code INVOCATION_LAST}.
	/// 
	/// @author      Fred Ecks
	/// @author      David Mendenhall
	/// 
	/// </para>
	/// </summary>
	/// <seealso cref=         java.awt.ActiveEvent </seealso>
	/// <seealso cref=         java.awt.EventQueue#invokeLater </seealso>
	/// <seealso cref=         java.awt.EventQueue#invokeAndWait </seealso>
	/// <seealso cref=         AWTEventListener
	/// 
	/// @since       1.2 </seealso>
	public class InvocationEvent : AWTEvent, ActiveEvent
	{

		static InvocationEvent()
		{
			AWTAccessor.InvocationEventAccessor = new InvocationEventAccessorAnonymousInnerClassHelper();
		}

		private class InvocationEventAccessorAnonymousInnerClassHelper : AWTAccessor.InvocationEventAccessor
		{
			public InvocationEventAccessorAnonymousInnerClassHelper()
			{
			}

			public override void Dispose(InvocationEvent invocationEvent)
			{
				invocationEvent.FinishedDispatching(false);
			}
		}

		/// <summary>
		/// Marks the first integer id for the range of invocation event ids.
		/// </summary>
		public const int INVOCATION_FIRST = 1200;

		/// <summary>
		/// The default id for all InvocationEvents.
		/// </summary>
		public const int INVOCATION_DEFAULT = INVOCATION_FIRST;

		/// <summary>
		/// Marks the last integer id for the range of invocation event ids.
		/// </summary>
		public const int INVOCATION_LAST = INVOCATION_DEFAULT;

		/// <summary>
		/// The Runnable whose run() method will be called.
		/// </summary>
		protected internal Runnable Runnable;

		/// <summary>
		/// The (potentially null) Object whose notifyAll() method will be called
		/// immediately after the Runnable.run() method has returned or thrown an exception
		/// or after the event was disposed.
		/// </summary>
		/// <seealso cref= #isDispatched </seealso>
		protected internal volatile Object Notifier;

		/// <summary>
		/// The (potentially null) Runnable whose run() method will be called
		/// immediately after the event was dispatched or disposed.
		/// </summary>
		/// <seealso cref= #isDispatched
		/// @since 1.8 </seealso>
		private readonly Runnable Listener;

		/// <summary>
		/// Indicates whether the <code>run()</code> method of the <code>runnable</code>
		/// was executed or not.
		/// </summary>
		/// <seealso cref= #isDispatched
		/// @since 1.7 </seealso>
		private volatile bool Dispatched_Renamed = false;

		/// <summary>
		/// Set to true if dispatch() catches Throwable and stores it in the
		/// exception instance variable. If false, Throwables are propagated up
		/// to the EventDispatchThread's dispatch loop.
		/// </summary>
		protected internal bool CatchExceptions;

		/// <summary>
		/// The (potentially null) Exception thrown during execution of the
		/// Runnable.run() method. This variable will also be null if a particular
		/// instance does not catch exceptions.
		/// </summary>
		private Exception Exception_Renamed = null;

		/// <summary>
		/// The (potentially null) Throwable thrown during execution of the
		/// Runnable.run() method. This variable will also be null if a particular
		/// instance does not catch exceptions.
		/// </summary>
		private Throwable Throwable_Renamed = null;

		/// <summary>
		/// The timestamp of when this event occurred.
		/// 
		/// @serial </summary>
		/// <seealso cref= #getWhen </seealso>
		private long When_Renamed;

		/*
		 * JDK 1.1 serialVersionUID.
		 */
		private const long SerialVersionUID = 436056344909459450L;

		/// <summary>
		/// Constructs an <code>InvocationEvent</code> with the specified
		/// source which will execute the runnable's <code>run</code>
		/// method when dispatched.
		/// <para>This is a convenience constructor.  An invocation of the form
		/// <tt>InvocationEvent(source, runnable)</tt>
		/// behaves in exactly the same way as the invocation of
		/// <tt><seealso cref="#InvocationEvent(Object, Runnable, Object, boolean) InvocationEvent"/>(source, runnable, null, false)</tt>.
		/// </para>
		/// <para> This method throws an <code>IllegalArgumentException</code>
		/// if <code>source</code> is <code>null</code>.
		/// 
		/// </para>
		/// </summary>
		/// <param name="source">    The <code>Object</code> that originated the event </param>
		/// <param name="runnable">  The <code>Runnable</code> whose <code>run</code>
		///                  method will be executed </param>
		/// <exception cref="IllegalArgumentException"> if <code>source</code> is null
		/// </exception>
		/// <seealso cref= #getSource() </seealso>
		/// <seealso cref= #InvocationEvent(Object, Runnable, Object, boolean) </seealso>
		public InvocationEvent(Object source, Runnable runnable) : this(source, INVOCATION_DEFAULT, runnable, null, null, false)
		{
		}

		/// <summary>
		/// Constructs an <code>InvocationEvent</code> with the specified
		/// source which will execute the runnable's <code>run</code>
		/// method when dispatched.  If notifier is non-<code>null</code>,
		/// <code>notifyAll()</code> will be called on it
		/// immediately after <code>run</code> has returned or thrown an exception.
		/// <para>An invocation of the form <tt>InvocationEvent(source,
		/// runnable, notifier, catchThrowables)</tt>
		/// behaves in exactly the same way as the invocation of
		/// <tt><seealso cref="#InvocationEvent(Object, int, Runnable, Object, boolean) InvocationEvent"/>(source, InvocationEvent.INVOCATION_DEFAULT, runnable, notifier, catchThrowables)</tt>.
		/// </para>
		/// <para>This method throws an <code>IllegalArgumentException</code>
		/// if <code>source</code> is <code>null</code>.
		/// 
		/// </para>
		/// </summary>
		/// <param name="source">            The <code>Object</code> that originated
		///                          the event </param>
		/// <param name="runnable">          The <code>Runnable</code> whose
		///                          <code>run</code> method will be
		///                          executed </param>
		/// <param name="notifier">          The {@code Object} whose <code>notifyAll</code>
		///                          method will be called after
		///                          <code>Runnable.run</code> has returned or
		///                          thrown an exception or after the event was
		///                          disposed </param>
		/// <param name="catchThrowables">   Specifies whether <code>dispatch</code>
		///                          should catch Throwable when executing
		///                          the <code>Runnable</code>'s <code>run</code>
		///                          method, or should instead propagate those
		///                          Throwables to the EventDispatchThread's
		///                          dispatch loop </param>
		/// <exception cref="IllegalArgumentException"> if <code>source</code> is null
		/// </exception>
		/// <seealso cref= #getSource() </seealso>
		/// <seealso cref=     #InvocationEvent(Object, int, Runnable, Object, boolean) </seealso>
		public InvocationEvent(Object source, Runnable runnable, Object notifier, bool catchThrowables) : this(source, INVOCATION_DEFAULT, runnable, notifier, null, catchThrowables)
		{
		}

		/// <summary>
		/// Constructs an <code>InvocationEvent</code> with the specified
		/// source which will execute the runnable's <code>run</code>
		/// method when dispatched.  If listener is non-<code>null</code>,
		/// <code>listener.run()</code> will be called immediately after
		/// <code>run</code> has returned, thrown an exception or the event
		/// was disposed.
		/// <para>This method throws an <code>IllegalArgumentException</code>
		/// if <code>source</code> is <code>null</code>.
		/// 
		/// </para>
		/// </summary>
		/// <param name="source">            The <code>Object</code> that originated
		///                          the event </param>
		/// <param name="runnable">          The <code>Runnable</code> whose
		///                          <code>run</code> method will be
		///                          executed </param>
		/// <param name="listener">          The <code>Runnable</code>Runnable whose
		///                          <code>run()</code> method will be called
		///                          after the {@code InvocationEvent}
		///                          was dispatched or disposed </param>
		/// <param name="catchThrowables">   Specifies whether <code>dispatch</code>
		///                          should catch Throwable when executing
		///                          the <code>Runnable</code>'s <code>run</code>
		///                          method, or should instead propagate those
		///                          Throwables to the EventDispatchThread's
		///                          dispatch loop </param>
		/// <exception cref="IllegalArgumentException"> if <code>source</code> is null </exception>
		public InvocationEvent(Object source, Runnable runnable, Runnable listener, bool catchThrowables) : this(source, INVOCATION_DEFAULT, runnable, null, listener, catchThrowables)
		{
		}

		/// <summary>
		/// Constructs an <code>InvocationEvent</code> with the specified
		/// source and ID which will execute the runnable's <code>run</code>
		/// method when dispatched.  If notifier is non-<code>null</code>,
		/// <code>notifyAll</code> will be called on it immediately after
		/// <code>run</code> has returned or thrown an exception.
		/// <para>This method throws an
		/// <code>IllegalArgumentException</code> if <code>source</code>
		/// is <code>null</code>.
		/// 
		/// </para>
		/// </summary>
		/// <param name="source">            The <code>Object</code> that originated
		///                          the event </param>
		/// <param name="id">     An integer indicating the type of event.
		///                     For information on allowable values, see
		///                     the class description for <seealso cref="InvocationEvent"/> </param>
		/// <param name="runnable">          The <code>Runnable</code> whose
		///                          <code>run</code> method will be executed </param>
		/// <param name="notifier">          The <code>Object</code> whose <code>notifyAll</code>
		///                          method will be called after
		///                          <code>Runnable.run</code> has returned or
		///                          thrown an exception or after the event was
		///                          disposed </param>
		/// <param name="catchThrowables">   Specifies whether <code>dispatch</code>
		///                          should catch Throwable when executing the
		///                          <code>Runnable</code>'s <code>run</code>
		///                          method, or should instead propagate those
		///                          Throwables to the EventDispatchThread's
		///                          dispatch loop </param>
		/// <exception cref="IllegalArgumentException"> if <code>source</code> is null </exception>
		/// <seealso cref= #getSource() </seealso>
		/// <seealso cref= #getID() </seealso>
		protected internal InvocationEvent(Object source, int id, Runnable runnable, Object notifier, bool catchThrowables) : this(source, id, runnable, notifier, null, catchThrowables)
		{
		}

		private InvocationEvent(Object source, int id, Runnable runnable, Object notifier, Runnable listener, bool catchThrowables) : base(source, id)
		{
			this.Runnable = runnable;
			this.Notifier = notifier;
			this.Listener = listener;
			this.CatchExceptions = catchThrowables;
			this.When_Renamed = DateTimeHelperClass.CurrentUnixTimeMillis();
		}
		/// <summary>
		/// Executes the Runnable's <code>run()</code> method and notifies the
		/// notifier (if any) when <code>run()</code> has returned or thrown an exception.
		/// </summary>
		/// <seealso cref= #isDispatched </seealso>
		public virtual void Dispatch()
		{
			try
			{
				if (CatchExceptions)
				{
					try
					{
						Runnable.Run();
					}
					catch (Throwable t)
					{
						if (t is Exception)
						{
							Exception_Renamed = (Exception) t;
						}
						Throwable_Renamed = t;
					}
				}
				else
				{
					Runnable.Run();
				}
			}
			finally
			{
				FinishedDispatching(true);
			}
		}

		/// <summary>
		/// Returns any Exception caught while executing the Runnable's <code>run()
		/// </code> method.
		/// </summary>
		/// <returns>  A reference to the Exception if one was thrown; null if no
		///          Exception was thrown or if this InvocationEvent does not
		///          catch exceptions </returns>
		public virtual Exception Exception
		{
			get
			{
				return (CatchExceptions) ? Exception_Renamed : null;
			}
		}

		/// <summary>
		/// Returns any Throwable caught while executing the Runnable's <code>run()
		/// </code> method.
		/// </summary>
		/// <returns>  A reference to the Throwable if one was thrown; null if no
		///          Throwable was thrown or if this InvocationEvent does not
		///          catch Throwables
		/// @since 1.5 </returns>
		public virtual Throwable Throwable
		{
			get
			{
				return (CatchExceptions) ? Throwable_Renamed : null;
			}
		}

		/// <summary>
		/// Returns the timestamp of when this event occurred.
		/// </summary>
		/// <returns> this event's timestamp
		/// @since 1.4 </returns>
		public virtual long When
		{
			get
			{
				return When_Renamed;
			}
		}

		/// <summary>
		/// Returns {@code true} if the event is dispatched or any exception is
		/// thrown while dispatching, {@code false} otherwise. The method should
		/// be called by a waiting thread that calls the {@code notifier.wait()} method.
		/// Since spurious wakeups are possible (as explained in <seealso cref="Object#wait()"/>),
		/// this method should be used in a waiting loop to ensure that the event
		/// got dispatched:
		/// <pre>
		///     while (!event.isDispatched()) {
		///         notifier.wait();
		///     }
		/// </pre>
		/// If the waiting thread wakes up without dispatching the event,
		/// the {@code isDispatched()} method returns {@code false}, and
		/// the {@code while} loop executes once more, thus, causing
		/// the awakened thread to revert to the waiting mode.
		/// <para>
		/// If the {@code notifier.notifyAll()} happens before the waiting thread
		/// enters the {@code notifier.wait()} method, the {@code while} loop ensures
		/// that the waiting thread will not enter the {@code notifier.wait()} method.
		/// Otherwise, there is no guarantee that the waiting thread will ever be woken
		/// from the wait.
		/// 
		/// </para>
		/// </summary>
		/// <returns> {@code true} if the event has been dispatched, or any exception
		/// has been thrown while dispatching, {@code false} otherwise </returns>
		/// <seealso cref= #dispatch </seealso>
		/// <seealso cref= #notifier </seealso>
		/// <seealso cref= #catchExceptions
		/// @since 1.7 </seealso>
		public virtual bool Dispatched
		{
			get
			{
				return Dispatched_Renamed;
			}
		}

		/// <summary>
		/// Called when the event was dispatched or disposed </summary>
		/// <param name="dispatched"> true if the event was dispatched
		///                   false if the event was disposed </param>
		private void FinishedDispatching(bool dispatched)
		{
			this.Dispatched_Renamed = dispatched;

			if (Notifier != null)
			{
				lock (Notifier)
				{
					Monitor.PulseAll(Notifier);
				}
			}

			if (Listener != null)
			{
				Listener.Run();
			}
		}

		/// <summary>
		/// Returns a parameter string identifying this event.
		/// This method is useful for event-logging and for debugging.
		/// </summary>
		/// <returns>  A string identifying the event and its attributes </returns>
		public override String ParamString()
		{
			String typeStr;
			switch (Id)
			{
				case INVOCATION_DEFAULT:
					typeStr = "INVOCATION_DEFAULT";
					break;
				default:
					typeStr = "unknown type";
				break;
			}
			return typeStr + ",runnable=" + Runnable + ",notifier=" + Notifier + ",catchExceptions=" + CatchExceptions + ",when=" + When_Renamed;
		}
	}

}
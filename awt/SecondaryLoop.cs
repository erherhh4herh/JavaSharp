/*
 * Copyright (c) 2010, Oracle and/or its affiliates. All rights reserved.
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

namespace java.awt
{

	/// <summary>
	/// A helper interface to run the nested event loop.
	/// <para>
	/// Objects that implement this interface are created with the
	/// <seealso cref="EventQueue#createSecondaryLoop"/> method. The interface
	/// provides two methods, <seealso cref="#enter"/> and <seealso cref="#exit"/>,
	/// which can be used to start and stop the event loop.
	/// </para>
	/// <para>
	/// When the <seealso cref="#enter"/> method is called, the current
	/// thread is blocked until the loop is terminated by the
	/// <seealso cref="#exit"/> method. Also, a new event loop is started
	/// on the event dispatch thread, which may or may not be
	/// the current thread. The loop can be terminated on any
	/// thread by calling its <seealso cref="#exit"/> method. After the
	/// loop is terminated, the {@code SecondaryLoop} object can
	/// be reused to run a new nested event loop.
	/// </para>
	/// <para>
	/// A typical use case of applying this interface is AWT
	/// and Swing modal dialogs. When a modal dialog is shown on
	/// the event dispatch thread, it enters a new secondary loop.
	/// Later, when the dialog is hidden or disposed, it exits
	/// the loop, and the thread continues its execution.
	/// </para>
	/// <para>
	/// The following example illustrates a simple use case of
	/// secondary loops:
	/// 
	/// <pre>
	///   SecondaryLoop loop;
	/// 
	///   JButton jButton = new JButton("Button");
	///   jButton.addActionListener(new ActionListener() {
	///       {@code @Override}
	///       public void actionPerformed(ActionEvent e) {
	///           Toolkit tk = Toolkit.getDefaultToolkit();
	///           EventQueue eq = tk.getSystemEventQueue();
	///           loop = eq.createSecondaryLoop();
	/// 
	///           // Spawn a new thread to do the work
	///           Thread worker = new WorkerThread();
	///           worker.start();
	/// 
	///           // Enter the loop to block the current event
	///           // handler, but leave UI responsive
	///           if (!loop.enter()) {
	///               // Report an error
	///           }
	///       }
	///   });
	/// 
	///   class WorkerThread extends Thread {
	///       {@code @Override}
	///       public void run() {
	///           // Perform calculations
	///           doSomethingUseful();
	/// 
	///           // Exit the loop
	///           loop.exit();
	///       }
	///   }
	/// </pre>
	/// 
	/// </para>
	/// </summary>
	/// <seealso cref= Dialog#show </seealso>
	/// <seealso cref= EventQueue#createSecondaryLoop </seealso>
	/// <seealso cref= Toolkit#getSystemEventQueue
	/// 
	/// @author Anton Tarasov, Artem Ananiev
	/// 
	/// @since 1.7 </seealso>
	public interface SecondaryLoop
	{

		/// <summary>
		/// Blocks the execution of the current thread and enters a new
		/// secondary event loop on the event dispatch thread.
		/// <para>
		/// This method can be called by any thread including the event
		/// dispatch thread. This thread will be blocked until the {@link
		/// #exit} method is called or the loop is terminated. A new
		/// secondary loop will be created on the event dispatch thread
		/// for dispatching events in either case.
		/// </para>
		/// <para>
		/// This method can only start one new event loop at a time per
		/// object. If a secondary event loop has already been started
		/// by this object and is currently still running, this method
		/// returns {@code false} to indicate that it was not successful
		/// in starting a new event loop. Otherwise, this method blocks
		/// the calling thread and later returns {@code true} when the
		/// new event loop is terminated. At such time, this object can
		/// again be used to start another new event loop.
		/// 
		/// </para>
		/// </summary>
		/// <returns> {@code true} after termination of the secondary loop,
		///         if the secondary loop was started by this call,
		///         {@code false} otherwise </returns>
		bool Enter();

		/// <summary>
		/// Unblocks the execution of the thread blocked by the {@link
		/// #enter} method and exits the secondary loop.
		/// <para>
		/// This method resumes the thread that called the <seealso cref="#enter"/>
		/// method and exits the secondary loop that was created when
		/// the <seealso cref="#enter"/> method was invoked.
		/// </para>
		/// <para>
		/// Note that if any other secondary loop is started while this
		/// loop is running, the blocked thread will not resume execution
		/// until the nested loop is terminated.
		/// </para>
		/// <para>
		/// If this secondary loop has not been started with the {@link
		/// #enter} method, or this secondary loop has already finished
		/// with the <seealso cref="#exit"/> method, this method returns {@code
		/// false}, otherwise {@code true} is returned.
		/// 
		/// </para>
		/// </summary>
		/// <returns> {@code true} if this loop was previously started and
		///         has not yet been finished with the <seealso cref="#exit"/> method,
		///         {@code false} otherwise </returns>
		bool Exit();

	}

}
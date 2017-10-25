using System;
using System.Collections.Generic;
using System.Threading;

/*
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

/*
 *
 *
 *
 *
 *
 * Written by Doug Lea with assistance from members of JCP JSR-166
 * Expert Group and released to the public domain, as explained at
 * http://creativecommons.org/publicdomain/zero/1.0/
 */

namespace java.util.concurrent
{

	using UncaughtExceptionHandler = Thread.UncaughtExceptionHandler;

	/// <summary>
	/// An <seealso cref="ExecutorService"/> for running <seealso cref="ForkJoinTask"/>s.
	/// A {@code ForkJoinPool} provides the entry point for submissions
	/// from non-{@code ForkJoinTask} clients, as well as management and
	/// monitoring operations.
	/// 
	/// <para>A {@code ForkJoinPool} differs from other kinds of {@link
	/// ExecutorService} mainly by virtue of employing
	/// <em>work-stealing</em>: all threads in the pool attempt to find and
	/// execute tasks submitted to the pool and/or created by other active
	/// tasks (eventually blocking waiting for work if none exist). This
	/// enables efficient processing when most tasks spawn other subtasks
	/// (as do most {@code ForkJoinTask}s), as well as when many small
	/// tasks are submitted to the pool from external clients.  Especially
	/// when setting <em>asyncMode</em> to true in constructors, {@code
	/// ForkJoinPool}s may also be appropriate for use with event-style
	/// tasks that are never joined.
	/// 
	/// </para>
	/// <para>A static <seealso cref="#commonPool()"/> is available and appropriate for
	/// most applications. The common pool is used by any ForkJoinTask that
	/// is not explicitly submitted to a specified pool. Using the common
	/// pool normally reduces resource usage (its threads are slowly
	/// reclaimed during periods of non-use, and reinstated upon subsequent
	/// use).
	/// 
	/// </para>
	/// <para>For applications that require separate or custom pools, a {@code
	/// ForkJoinPool} may be constructed with a given target parallelism
	/// level; by default, equal to the number of available processors.
	/// The pool attempts to maintain enough active (or available) threads
	/// by dynamically adding, suspending, or resuming internal worker
	/// threads, even if some tasks are stalled waiting to join others.
	/// However, no such adjustments are guaranteed in the face of blocked
	/// I/O or other unmanaged synchronization. The nested {@link
	/// ManagedBlocker} interface enables extension of the kinds of
	/// synchronization accommodated.
	/// 
	/// </para>
	/// <para>In addition to execution and lifecycle control methods, this
	/// class provides status check methods (for example
	/// <seealso cref="#getStealCount"/>) that are intended to aid in developing,
	/// tuning, and monitoring fork/join applications. Also, method
	/// <seealso cref="#toString"/> returns indications of pool state in a
	/// convenient form for informal monitoring.
	/// 
	/// </para>
	/// <para>As is the case with other ExecutorServices, there are three
	/// main task execution methods summarized in the following table.
	/// These are designed to be used primarily by clients not already
	/// engaged in fork/join computations in the current pool.  The main
	/// forms of these methods accept instances of {@code ForkJoinTask},
	/// but overloaded forms also allow mixed execution of plain {@code
	/// Runnable}- or {@code Callable}- based activities as well.  However,
	/// tasks that are already executing in a pool should normally instead
	/// use the within-computation forms listed in the table unless using
	/// async event-style tasks that are not usually joined, in which case
	/// there is little difference among choice of methods.
	/// 
	/// <table BORDER CELLPADDING=3 CELLSPACING=1>
	/// <caption>Summary of task execution methods</caption>
	///  <tr>
	///    <td></td>
	///    <td ALIGN=CENTER> <b>Call from non-fork/join clients</b></td>
	///    <td ALIGN=CENTER> <b>Call from within fork/join computations</b></td>
	///  </tr>
	///  <tr>
	///    <td> <b>Arrange async execution</b></td>
	///    <td> <seealso cref="#execute(ForkJoinTask)"/></td>
	///    <td> <seealso cref="ForkJoinTask#fork"/></td>
	///  </tr>
	///  <tr>
	///    <td> <b>Await and obtain result</b></td>
	///    <td> <seealso cref="#invoke(ForkJoinTask)"/></td>
	///    <td> <seealso cref="ForkJoinTask#invoke"/></td>
	///  </tr>
	///  <tr>
	///    <td> <b>Arrange exec and obtain Future</b></td>
	///    <td> <seealso cref="#submit(ForkJoinTask)"/></td>
	///    <td> <seealso cref="ForkJoinTask#fork"/> (ForkJoinTasks <em>are</em> Futures)</td>
	///  </tr>
	/// </table>
	/// 
	/// </para>
	/// <para>The common pool is by default constructed with default
	/// parameters, but these may be controlled by setting three
	/// <seealso cref="System#getProperty system properties"/>:
	/// <ul>
	/// <li>{@code java.util.concurrent.ForkJoinPool.common.parallelism}
	/// - the parallelism level, a non-negative integer
	/// <li>{@code java.util.concurrent.ForkJoinPool.common.threadFactory}
	/// - the class name of a <seealso cref="ForkJoinWorkerThreadFactory"/>
	/// <li>{@code java.util.concurrent.ForkJoinPool.common.exceptionHandler}
	/// - the class name of a <seealso cref="UncaughtExceptionHandler"/>
	/// </ul>
	/// If a <seealso cref="SecurityManager"/> is present and no factory is
	/// specified, then the default pool uses a factory supplying
	/// threads that have no <seealso cref="Permissions"/> enabled.
	/// The system class loader is used to load these classes.
	/// Upon any error in establishing these settings, default parameters
	/// are used. It is possible to disable or limit the use of threads in
	/// the common pool by setting the parallelism property to zero, and/or
	/// using a factory that may return {@code null}. However doing so may
	/// cause unjoined tasks to never be executed.
	/// 
	/// </para>
	/// <para><b>Implementation notes</b>: This implementation restricts the
	/// maximum number of running threads to 32767. Attempts to create
	/// pools with greater than the maximum number result in
	/// {@code IllegalArgumentException}.
	/// 
	/// </para>
	/// <para>This implementation rejects submitted tasks (that is, by throwing
	/// <seealso cref="RejectedExecutionException"/>) only when the pool is shut down
	/// or internal resources have been exhausted.
	/// 
	/// @since 1.7
	/// @author Doug Lea
	/// </para>
	/// </summary>
//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @sun.misc.Contended public class ForkJoinPool extends java.util.concurrent.AbstractExecutorService
	public class ForkJoinPool : AbstractExecutorService
	{

		/*
		 * Implementation Overview
		 *
		 * This class and its nested classes provide the main
		 * functionality and control for a set of worker threads:
		 * Submissions from non-FJ threads enter into submission queues.
		 * Workers take these tasks and typically split them into subtasks
		 * that may be stolen by other workers.  Preference rules give
		 * first priority to processing tasks from their own queues (LIFO
		 * or FIFO, depending on mode), then to randomized FIFO steals of
		 * tasks in other queues.  This framework began as vehicle for
		 * supporting tree-structured parallelism using work-stealing.
		 * Over time, its scalability advantages led to extensions and
		 * changes to better support more diverse usage contexts.  Because
		 * most internal methods and nested classes are interrelated,
		 * their main rationale and descriptions are presented here;
		 * individual methods and nested classes contain only brief
		 * comments about details.
		 *
		 * WorkQueues
		 * ==========
		 *
		 * Most operations occur within work-stealing queues (in nested
		 * class WorkQueue).  These are special forms of Deques that
		 * support only three of the four possible end-operations -- push,
		 * pop, and poll (aka steal), under the further constraints that
		 * push and pop are called only from the owning thread (or, as
		 * extended here, under a lock), while poll may be called from
		 * other threads.  (If you are unfamiliar with them, you probably
		 * want to read Herlihy and Shavit's book "The Art of
		 * Multiprocessor programming", chapter 16 describing these in
		 * more detail before proceeding.)  The main work-stealing queue
		 * design is roughly similar to those in the papers "Dynamic
		 * Circular Work-Stealing Deque" by Chase and Lev, SPAA 2005
		 * (http://research.sun.com/scalable/pubs/index.html) and
		 * "Idempotent work stealing" by Michael, Saraswat, and Vechev,
		 * PPoPP 2009 (http://portal.acm.org/citation.cfm?id=1504186).
		 * The main differences ultimately stem from GC requirements that
		 * we null out taken slots as soon as we can, to maintain as small
		 * a footprint as possible even in programs generating huge
		 * numbers of tasks. To accomplish this, we shift the CAS
		 * arbitrating pop vs poll (steal) from being on the indices
		 * ("base" and "top") to the slots themselves.
		 *
		 * Adding tasks then takes the form of a classic array push(task):
		 *    q.array[q.top] = task; ++q.top;
		 *
		 * (The actual code needs to null-check and size-check the array,
		 * properly fence the accesses, and possibly signal waiting
		 * workers to start scanning -- see below.)  Both a successful pop
		 * and poll mainly entail a CAS of a slot from non-null to null.
		 *
		 * The pop operation (always performed by owner) is:
		 *   if ((base != top) and
		 *        (the task at top slot is not null) and
		 *        (CAS slot to null))
		 *           decrement top and return task;
		 *
		 * And the poll operation (usually by a stealer) is
		 *    if ((base != top) and
		 *        (the task at base slot is not null) and
		 *        (base has not changed) and
		 *        (CAS slot to null))
		 *           increment base and return task;
		 *
		 * Because we rely on CASes of references, we do not need tag bits
		 * on base or top.  They are simple ints as used in any circular
		 * array-based queue (see for example ArrayDeque).  Updates to the
		 * indices guarantee that top == base means the queue is empty,
		 * but otherwise may err on the side of possibly making the queue
		 * appear nonempty when a push, pop, or poll have not fully
		 * committed. (Method isEmpty() checks the case of a partially
		 * completed removal of the last element.)  Because of this, the
		 * poll operation, considered individually, is not wait-free. One
		 * thief cannot successfully continue until another in-progress
		 * one (or, if previously empty, a push) completes.  However, in
		 * the aggregate, we ensure at least probabilistic
		 * non-blockingness.  If an attempted steal fails, a thief always
		 * chooses a different random victim target to try next. So, in
		 * order for one thief to progress, it suffices for any
		 * in-progress poll or new push on any empty queue to
		 * complete. (This is why we normally use method pollAt and its
		 * variants that try once at the apparent base index, else
		 * consider alternative actions, rather than method poll, which
		 * retries.)
		 *
		 * This approach also enables support of a user mode in which
		 * local task processing is in FIFO, not LIFO order, simply by
		 * using poll rather than pop.  This can be useful in
		 * message-passing frameworks in which tasks are never joined.
		 * However neither mode considers affinities, loads, cache
		 * localities, etc, so rarely provide the best possible
		 * performance on a given machine, but portably provide good
		 * throughput by averaging over these factors.  Further, even if
		 * we did try to use such information, we do not usually have a
		 * basis for exploiting it.  For example, some sets of tasks
		 * profit from cache affinities, but others are harmed by cache
		 * pollution effects. Additionally, even though it requires
		 * scanning, long-term throughput is often best using random
		 * selection rather than directed selection policies, so cheap
		 * randomization of sufficient quality is used whenever
		 * applicable.  Various Marsaglia XorShifts (some with different
		 * shift constants) are inlined at use points.
		 *
		 * WorkQueues are also used in a similar way for tasks submitted
		 * to the pool. We cannot mix these tasks in the same queues used
		 * by workers. Instead, we randomly associate submission queues
		 * with submitting threads, using a form of hashing.  The
		 * ThreadLocalRandom probe value serves as a hash code for
		 * choosing existing queues, and may be randomly repositioned upon
		 * contention with other submitters.  In essence, submitters act
		 * like workers except that they are restricted to executing local
		 * tasks that they submitted (or in the case of CountedCompleters,
		 * others with the same root task).  Insertion of tasks in shared
		 * mode requires a lock (mainly to protect in the case of
		 * resizing) but we use only a simple spinlock (using field
		 * qlock), because submitters encountering a busy queue move on to
		 * try or create other queues -- they block only when creating and
		 * registering new queues. Additionally, "qlock" saturates to an
		 * unlockable value (-1) at shutdown. Unlocking still can be and
		 * is performed by cheaper ordered writes of "qlock" in successful
		 * cases, but uses CAS in unsuccessful cases.
		 *
		 * Management
		 * ==========
		 *
		 * The main throughput advantages of work-stealing stem from
		 * decentralized control -- workers mostly take tasks from
		 * themselves or each other, at rates that can exceed a billion
		 * per second.  The pool itself creates, activates (enables
		 * scanning for and running tasks), deactivates, blocks, and
		 * terminates threads, all with minimal central information.
		 * There are only a few properties that we can globally track or
		 * maintain, so we pack them into a small number of variables,
		 * often maintaining atomicity without blocking or locking.
		 * Nearly all essentially atomic control state is held in two
		 * volatile variables that are by far most often read (not
		 * written) as status and consistency checks. (Also, field
		 * "config" holds unchanging configuration state.)
		 *
		 * Field "ctl" contains 64 bits holding information needed to
		 * atomically decide to add, inactivate, enqueue (on an event
		 * queue), dequeue, and/or re-activate workers.  To enable this
		 * packing, we restrict maximum parallelism to (1<<15)-1 (which is
		 * far in excess of normal operating range) to allow ids, counts,
		 * and their negations (used for thresholding) to fit into 16bit
		 * subfields.
		 *
		 * Field "runState" holds lockable state bits (STARTED, STOP, etc)
		 * also protecting updates to the workQueues array.  When used as
		 * a lock, it is normally held only for a few instructions (the
		 * only exceptions are one-time array initialization and uncommon
		 * resizing), so is nearly always available after at most a brief
		 * spin. But to be extra-cautious, after spinning, method
		 * awaitRunStateLock (called only if an initial CAS fails), uses a
		 * wait/notify mechanics on a builtin monitor to block when
		 * (rarely) needed. This would be a terrible idea for a highly
		 * contended lock, but most pools run without the lock ever
		 * contending after the spin limit, so this works fine as a more
		 * conservative alternative. Because we don't otherwise have an
		 * internal Object to use as a monitor, the "stealCounter" (an
		 * AtomicLong) is used when available (it too must be lazily
		 * initialized; see externalSubmit).
		 *
		 * Usages of "runState" vs "ctl" interact in only one case:
		 * deciding to add a worker thread (see tryAddWorker), in which
		 * case the ctl CAS is performed while the lock is held.
		 *
		 * Recording WorkQueues.  WorkQueues are recorded in the
		 * "workQueues" array. The array is created upon first use (see
		 * externalSubmit) and expanded if necessary.  Updates to the
		 * array while recording new workers and unrecording terminated
		 * ones are protected from each other by the runState lock, but
		 * the array is otherwise concurrently readable, and accessed
		 * directly. We also ensure that reads of the array reference
		 * itself never become too stale. To simplify index-based
		 * operations, the array size is always a power of two, and all
		 * readers must tolerate null slots. Worker queues are at odd
		 * indices. Shared (submission) queues are at even indices, up to
		 * a maximum of 64 slots, to limit growth even if array needs to
		 * expand to add more workers. Grouping them together in this way
		 * simplifies and speeds up task scanning.
		 *
		 * All worker thread creation is on-demand, triggered by task
		 * submissions, replacement of terminated workers, and/or
		 * compensation for blocked workers. However, all other support
		 * code is set up to work with other policies.  To ensure that we
		 * do not hold on to worker references that would prevent GC, All
		 * accesses to workQueues are via indices into the workQueues
		 * array (which is one source of some of the messy code
		 * constructions here). In essence, the workQueues array serves as
		 * a weak reference mechanism. Thus for example the stack top
		 * subfield of ctl stores indices, not references.
		 *
		 * Queuing Idle Workers. Unlike HPC work-stealing frameworks, we
		 * cannot let workers spin indefinitely scanning for tasks when
		 * none can be found immediately, and we cannot start/resume
		 * workers unless there appear to be tasks available.  On the
		 * other hand, we must quickly prod them into action when new
		 * tasks are submitted or generated. In many usages, ramp-up time
		 * to activate workers is the main limiting factor in overall
		 * performance, which is compounded at program start-up by JIT
		 * compilation and allocation. So we streamline this as much as
		 * possible.
		 *
		 * The "ctl" field atomically maintains active and total worker
		 * counts as well as a queue to place waiting threads so they can
		 * be located for signalling. Active counts also play the role of
		 * quiescence indicators, so are decremented when workers believe
		 * that there are no more tasks to execute. The "queue" is
		 * actually a form of Treiber stack.  A stack is ideal for
		 * activating threads in most-recently used order. This improves
		 * performance and locality, outweighing the disadvantages of
		 * being prone to contention and inability to release a worker
		 * unless it is topmost on stack.  We park/unpark workers after
		 * pushing on the idle worker stack (represented by the lower
		 * 32bit subfield of ctl) when they cannot find work.  The top
		 * stack state holds the value of the "scanState" field of the
		 * worker: its index and status, plus a version counter that, in
		 * addition to the count subfields (also serving as version
		 * stamps) provide protection against Treiber stack ABA effects.
		 *
		 * Field scanState is used by both workers and the pool to manage
		 * and track whether a worker is INACTIVE (possibly blocked
		 * waiting for a signal), or SCANNING for tasks (when neither hold
		 * it is busy running tasks).  When a worker is inactivated, its
		 * scanState field is set, and is prevented from executing tasks,
		 * even though it must scan once for them to avoid queuing
		 * races. Note that scanState updates lag queue CAS releases so
		 * usage requires care. When queued, the lower 16 bits of
		 * scanState must hold its pool index. So we place the index there
		 * upon initialization (see registerWorker) and otherwise keep it
		 * there or restore it when necessary.
		 *
		 * Memory ordering.  See "Correct and Efficient Work-Stealing for
		 * Weak Memory Models" by Le, Pop, Cohen, and Nardelli, PPoPP 2013
		 * (http://www.di.ens.fr/~zappa/readings/ppopp13.pdf) for an
		 * analysis of memory ordering requirements in work-stealing
		 * algorithms similar to the one used here.  We usually need
		 * stronger than minimal ordering because we must sometimes signal
		 * workers, requiring Dekker-like full-fences to avoid lost
		 * signals.  Arranging for enough ordering without expensive
		 * over-fencing requires tradeoffs among the supported means of
		 * expressing access constraints. The most central operations,
		 * taking from queues and updating ctl state, require full-fence
		 * CAS.  Array slots are read using the emulation of volatiles
		 * provided by Unsafe.  Access from other threads to WorkQueue
		 * base, top, and array requires a volatile load of the first of
		 * any of these read.  We use the convention of declaring the
		 * "base" index volatile, and always read it before other fields.
		 * The owner thread must ensure ordered updates, so writes use
		 * ordered intrinsics unless they can piggyback on those for other
		 * writes.  Similar conventions and rationales hold for other
		 * WorkQueue fields (such as "currentSteal") that are only written
		 * by owners but observed by others.
		 *
		 * Creating workers. To create a worker, we pre-increment total
		 * count (serving as a reservation), and attempt to construct a
		 * ForkJoinWorkerThread via its factory. Upon construction, the
		 * new thread invokes registerWorker, where it constructs a
		 * WorkQueue and is assigned an index in the workQueues array
		 * (expanding the array if necessary). The thread is then
		 * started. Upon any exception across these steps, or null return
		 * from factory, deregisterWorker adjusts counts and records
		 * accordingly.  If a null return, the pool continues running with
		 * fewer than the target number workers. If exceptional, the
		 * exception is propagated, generally to some external caller.
		 * Worker index assignment avoids the bias in scanning that would
		 * occur if entries were sequentially packed starting at the front
		 * of the workQueues array. We treat the array as a simple
		 * power-of-two hash table, expanding as needed. The seedIndex
		 * increment ensures no collisions until a resize is needed or a
		 * worker is deregistered and replaced, and thereafter keeps
		 * probability of collision low. We cannot use
		 * ThreadLocalRandom.getProbe() for similar purposes here because
		 * the thread has not started yet, but do so for creating
		 * submission queues for existing external threads.
		 *
		 * Deactivation and waiting. Queuing encounters several intrinsic
		 * races; most notably that a task-producing thread can miss
		 * seeing (and signalling) another thread that gave up looking for
		 * work but has not yet entered the wait queue.  When a worker
		 * cannot find a task to steal, it deactivates and enqueues. Very
		 * often, the lack of tasks is transient due to GC or OS
		 * scheduling. To reduce false-alarm deactivation, scanners
		 * compute checksums of queue states during sweeps.  (The
		 * stability checks used here and elsewhere are probabilistic
		 * variants of snapshot techniques -- see Herlihy & Shavit.)
		 * Workers give up and try to deactivate only after the sum is
		 * stable across scans. Further, to avoid missed signals, they
		 * repeat this scanning process after successful enqueuing until
		 * again stable.  In this state, the worker cannot take/run a task
		 * it sees until it is released from the queue, so the worker
		 * itself eventually tries to release itself or any successor (see
		 * tryRelease).  Otherwise, upon an empty scan, a deactivated
		 * worker uses an adaptive local spin construction (see awaitWork)
		 * before blocking (via park). Note the unusual conventions about
		 * Thread.interrupts surrounding parking and other blocking:
		 * Because interrupts are used solely to alert threads to check
		 * termination, which is checked anyway upon blocking, we clear
		 * status (using Thread.interrupted) before any call to park, so
		 * that park does not immediately return due to status being set
		 * via some other unrelated call to interrupt in user code.
		 *
		 * Signalling and activation.  Workers are created or activated
		 * only when there appears to be at least one task they might be
		 * able to find and execute.  Upon push (either by a worker or an
		 * external submission) to a previously (possibly) empty queue,
		 * workers are signalled if idle, or created if fewer exist than
		 * the given parallelism level.  These primary signals are
		 * buttressed by others whenever other threads remove a task from
		 * a queue and notice that there are other tasks there as well.
		 * On most platforms, signalling (unpark) overhead time is
		 * noticeably long, and the time between signalling a thread and
		 * it actually making progress can be very noticeably long, so it
		 * is worth offloading these delays from critical paths as much as
		 * possible. Also, because inactive workers are often rescanning
		 * or spinning rather than blocking, we set and clear the "parker"
		 * field of WorkQueues to reduce unnecessary calls to unpark.
		 * (This requires a secondary recheck to avoid missed signals.)
		 *
		 * Trimming workers. To release resources after periods of lack of
		 * use, a worker starting to wait when the pool is quiescent will
		 * time out and terminate (see awaitWork) if the pool has remained
		 * quiescent for period IDLE_TIMEOUT, increasing the period as the
		 * number of threads decreases, eventually removing all workers.
		 * Also, when more than two spare threads exist, excess threads
		 * are immediately terminated at the next quiescent point.
		 * (Padding by two avoids hysteresis.)
		 *
		 * Shutdown and Termination. A call to shutdownNow invokes
		 * tryTerminate to atomically set a runState bit. The calling
		 * thread, as well as every other worker thereafter terminating,
		 * helps terminate others by setting their (qlock) status,
		 * cancelling their unprocessed tasks, and waking them up, doing
		 * so repeatedly until stable (but with a loop bounded by the
		 * number of workers).  Calls to non-abrupt shutdown() preface
		 * this by checking whether termination should commence. This
		 * relies primarily on the active count bits of "ctl" maintaining
		 * consensus -- tryTerminate is called from awaitWork whenever
		 * quiescent. However, external submitters do not take part in
		 * this consensus.  So, tryTerminate sweeps through queues (until
		 * stable) to ensure lack of in-flight submissions and workers
		 * about to process them before triggering the "STOP" phase of
		 * termination. (Note: there is an intrinsic conflict if
		 * helpQuiescePool is called when shutdown is enabled. Both wait
		 * for quiescence, but tryTerminate is biased to not trigger until
		 * helpQuiescePool completes.)
		 *
		 *
		 * Joining Tasks
		 * =============
		 *
		 * Any of several actions may be taken when one worker is waiting
		 * to join a task stolen (or always held) by another.  Because we
		 * are multiplexing many tasks on to a pool of workers, we can't
		 * just let them block (as in Thread.join).  We also cannot just
		 * reassign the joiner's run-time stack with another and replace
		 * it later, which would be a form of "continuation", that even if
		 * possible is not necessarily a good idea since we may need both
		 * an unblocked task and its continuation to progress.  Instead we
		 * combine two tactics:
		 *
		 *   Helping: Arranging for the joiner to execute some task that it
		 *      would be running if the steal had not occurred.
		 *
		 *   Compensating: Unless there are already enough live threads,
		 *      method tryCompensate() may create or re-activate a spare
		 *      thread to compensate for blocked joiners until they unblock.
		 *
		 * A third form (implemented in tryRemoveAndExec) amounts to
		 * helping a hypothetical compensator: If we can readily tell that
		 * a possible action of a compensator is to steal and execute the
		 * task being joined, the joining thread can do so directly,
		 * without the need for a compensation thread (although at the
		 * expense of larger run-time stacks, but the tradeoff is
		 * typically worthwhile).
		 *
		 * The ManagedBlocker extension API can't use helping so relies
		 * only on compensation in method awaitBlocker.
		 *
		 * The algorithm in helpStealer entails a form of "linear
		 * helping".  Each worker records (in field currentSteal) the most
		 * recent task it stole from some other worker (or a submission).
		 * It also records (in field currentJoin) the task it is currently
		 * actively joining. Method helpStealer uses these markers to try
		 * to find a worker to help (i.e., steal back a task from and
		 * execute it) that could hasten completion of the actively joined
		 * task.  Thus, the joiner executes a task that would be on its
		 * own local deque had the to-be-joined task not been stolen. This
		 * is a conservative variant of the approach described in Wagner &
		 * Calder "Leapfrogging: a portable technique for implementing
		 * efficient futures" SIGPLAN Notices, 1993
		 * (http://portal.acm.org/citation.cfm?id=155354). It differs in
		 * that: (1) We only maintain dependency links across workers upon
		 * steals, rather than use per-task bookkeeping.  This sometimes
		 * requires a linear scan of workQueues array to locate stealers,
		 * but often doesn't because stealers leave hints (that may become
		 * stale/wrong) of where to locate them.  It is only a hint
		 * because a worker might have had multiple steals and the hint
		 * records only one of them (usually the most current).  Hinting
		 * isolates cost to when it is needed, rather than adding to
		 * per-task overhead.  (2) It is "shallow", ignoring nesting and
		 * potentially cyclic mutual steals.  (3) It is intentionally
		 * racy: field currentJoin is updated only while actively joining,
		 * which means that we miss links in the chain during long-lived
		 * tasks, GC stalls etc (which is OK since blocking in such cases
		 * is usually a good idea).  (4) We bound the number of attempts
		 * to find work using checksums and fall back to suspending the
		 * worker and if necessary replacing it with another.
		 *
		 * Helping actions for CountedCompleters do not require tracking
		 * currentJoins: Method helpComplete takes and executes any task
		 * with the same root as the task being waited on (preferring
		 * local pops to non-local polls). However, this still entails
		 * some traversal of completer chains, so is less efficient than
		 * using CountedCompleters without explicit joins.
		 *
		 * Compensation does not aim to keep exactly the target
		 * parallelism number of unblocked threads running at any given
		 * time. Some previous versions of this class employed immediate
		 * compensations for any blocked join. However, in practice, the
		 * vast majority of blockages are transient byproducts of GC and
		 * other JVM or OS activities that are made worse by replacement.
		 * Currently, compensation is attempted only after validating that
		 * all purportedly active threads are processing tasks by checking
		 * field WorkQueue.scanState, which eliminates most false
		 * positives.  Also, compensation is bypassed (tolerating fewer
		 * threads) in the most common case in which it is rarely
		 * beneficial: when a worker with an empty queue (thus no
		 * continuation tasks) blocks on a join and there still remain
		 * enough threads to ensure liveness.
		 *
		 * The compensation mechanism may be bounded.  Bounds for the
		 * commonPool (see commonMaxSpares) better enable JVMs to cope
		 * with programming errors and abuse before running out of
		 * resources to do so. In other cases, users may supply factories
		 * that limit thread construction. The effects of bounding in this
		 * pool (like all others) is imprecise.  Total worker counts are
		 * decremented when threads deregister, not when they exit and
		 * resources are reclaimed by the JVM and OS. So the number of
		 * simultaneously live threads may transiently exceed bounds.
		 *
		 * Common Pool
		 * ===========
		 *
		 * The static common pool always exists after static
		 * initialization.  Since it (or any other created pool) need
		 * never be used, we minimize initial construction overhead and
		 * footprint to the setup of about a dozen fields, with no nested
		 * allocation. Most bootstrapping occurs within method
		 * externalSubmit during the first submission to the pool.
		 *
		 * When external threads submit to the common pool, they can
		 * perform subtask processing (see externalHelpComplete and
		 * related methods) upon joins.  This caller-helps policy makes it
		 * sensible to set common pool parallelism level to one (or more)
		 * less than the total number of available cores, or even zero for
		 * pure caller-runs.  We do not need to record whether external
		 * submissions are to the common pool -- if not, external help
		 * methods return quickly. These submitters would otherwise be
		 * blocked waiting for completion, so the extra effort (with
		 * liberally sprinkled task status checks) in inapplicable cases
		 * amounts to an odd form of limited spin-wait before blocking in
		 * ForkJoinTask.join.
		 *
		 * As a more appropriate default in managed environments, unless
		 * overridden by system properties, we use workers of subclass
		 * InnocuousForkJoinWorkerThread when there is a SecurityManager
		 * present. These workers have no permissions set, do not belong
		 * to any user-defined ThreadGroup, and erase all ThreadLocals
		 * after executing any top-level task (see WorkQueue.runTask).
		 * The associated mechanics (mainly in ForkJoinWorkerThread) may
		 * be JVM-dependent and must access particular Thread class fields
		 * to achieve this effect.
		 *
		 * Style notes
		 * ===========
		 *
		 * Memory ordering relies mainly on Unsafe intrinsics that carry
		 * the further responsibility of explicitly performing null- and
		 * bounds- checks otherwise carried out implicitly by JVMs.  This
		 * can be awkward and ugly, but also reflects the need to control
		 * outcomes across the unusual cases that arise in very racy code
		 * with very few invariants. So these explicit checks would exist
		 * in some form anyway.  All fields are read into locals before
		 * use, and null-checked if they are references.  This is usually
		 * done in a "C"-like style of listing declarations at the heads
		 * of methods or blocks, and using inline assignments on first
		 * encounter.  Array bounds-checks are usually performed by
		 * masking with array.length-1, which relies on the invariant that
		 * these arrays are created with positive lengths, which is itself
		 * paranoically checked. Nearly all explicit checks lead to
		 * bypass/return, not exception throws, because they may
		 * legitimately arise due to cancellation/revocation during
		 * shutdown.
		 *
		 * There is a lot of representation-level coupling among classes
		 * ForkJoinPool, ForkJoinWorkerThread, and ForkJoinTask.  The
		 * fields of WorkQueue maintain data structures managed by
		 * ForkJoinPool, so are directly accessed.  There is little point
		 * trying to reduce this, since any associated future changes in
		 * representations will need to be accompanied by algorithmic
		 * changes anyway. Several methods intrinsically sprawl because
		 * they must accumulate sets of consistent reads of fields held in
		 * local variables.  There are also other coding oddities
		 * (including several unnecessary-looking hoisted null checks)
		 * that help some methods perform reasonably even when interpreted
		 * (not compiled).
		 *
		 * The order of declarations in this file is (with a few exceptions):
		 * (1) Static utility functions
		 * (2) Nested (static) classes
		 * (3) Static fields
		 * (4) Fields, along with constants used when unpacking some of them
		 * (5) Internal control methods
		 * (6) Callbacks and other support for ForkJoinTask methods
		 * (7) Exported methods
		 * (8) Static block initializing statics in minimally dependent order
		 */

		// Static utilities

		/// <summary>
		/// If there is a security manager, makes sure caller has
		/// permission to modify threads.
		/// </summary>
		private static void CheckPermission()
		{
			SecurityManager security = System.SecurityManager;
			if (security != null)
			{
				security.CheckPermission(ModifyThreadPermission);
			}
		}

		// Nested classes

		/// <summary>
		/// Factory for creating new <seealso cref="ForkJoinWorkerThread"/>s.
		/// A {@code ForkJoinWorkerThreadFactory} must be defined and used
		/// for {@code ForkJoinWorkerThread} subclasses that extend base
		/// functionality or initialize threads with different contexts.
		/// </summary>
		public interface ForkJoinWorkerThreadFactory
		{
			/// <summary>
			/// Returns a new worker thread operating in the given pool.
			/// </summary>
			/// <param name="pool"> the pool this thread works in </param>
			/// <returns> the new worker thread </returns>
			/// <exception cref="NullPointerException"> if the pool is null </exception>
			ForkJoinWorkerThread NewThread(ForkJoinPool pool);
		}

		/// <summary>
		/// Default ForkJoinWorkerThreadFactory implementation; creates a
		/// new ForkJoinWorkerThread.
		/// </summary>
		internal sealed class DefaultForkJoinWorkerThreadFactory : ForkJoinWorkerThreadFactory
		{
			public ForkJoinWorkerThread NewThread(ForkJoinPool pool)
			{
				return new ForkJoinWorkerThread(pool);
			}
		}

		/// <summary>
		/// Class for artificial tasks that are used to replace the target
		/// of local joins if they are removed from an interior queue slot
		/// in WorkQueue.tryRemoveAndExec. We don't need the proxy to
		/// actually do anything beyond having a unique identity.
		/// </summary>
		internal sealed class EmptyTask : ForkJoinTask<Void>
		{
			internal const long SerialVersionUID = -7721805057305804111L;
			internal EmptyTask() // force done
			{
				Status = ForkJoinTask.NORMAL;
			}
			public Void RawResult
			{
				get
				{
					return null;
				}
				set
				{
				}
			}
			public bool Exec()
			{
				return true;
			}
		}

		// Constants shared across ForkJoinPool and WorkQueue

		// Bounds
		internal const int SMASK = 0xffff; // short bits == max index
		internal const int MAX_CAP = 0x7fff; // max #workers - 1
		internal const int EVENMASK = 0xfffe; // even short bits
		internal const int SQMASK = 0x007e; // max 64 (even) slots

		// Masks and units for WorkQueue.scanState and ctl sp subfield
		internal const int SCANNING = 1; // false when running tasks
		internal static readonly int INACTIVE = 1 << 31; // must be negative
		internal static readonly int SS_SEQ = 1 << 16; // version count

		// Mode bits for ForkJoinPool.config and WorkQueue.config
		internal static readonly int MODE_MASK = 0xffff << 16; // top half of int
		internal const int LIFO_QUEUE = 0;
		internal static readonly int FIFO_QUEUE = 1 << 16;
		internal static readonly int SHARED_QUEUE = 1 << 31; // must be negative

		/// <summary>
		/// Queues supporting work-stealing as well as external task
		/// submission. See above for descriptions and algorithms.
		/// Performance on most platforms is very sensitive to placement of
		/// instances of both WorkQueues and their arrays -- we absolutely
		/// do not want multiple WorkQueue instances or multiple queue
		/// arrays sharing cache lines. The @Contended annotation alerts
		/// JVMs to try to keep instances apart.
		/// </summary>
//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @sun.misc.Contended static final class WorkQueue
		internal sealed class WorkQueue
		{

			/// <summary>
			/// Capacity of work-stealing queue array upon initialization.
			/// Must be a power of two; at least 4, but should be larger to
			/// reduce or eliminate cacheline sharing among queues.
			/// Currently, it is much larger, as a partial workaround for
			/// the fact that JVMs often place arrays in locations that
			/// share GC bookkeeping (especially cardmarks) such that
			/// per-write accesses encounter serious memory contention.
			/// </summary>
			internal static readonly int INITIAL_QUEUE_CAPACITY = 1 << 13;

			/// <summary>
			/// Maximum size for queue arrays. Must be a power of two less
			/// than or equal to 1 << (31 - width of array entry) to ensure
			/// lack of wraparound of index calculations, but defined to a
			/// value a bit less than this to help users trap runaway
			/// programs before saturating systems.
			/// </summary>
			internal static readonly int MAXIMUM_QUEUE_CAPACITY = 1 << 26; // 64M

			// Instance fields
			internal volatile int ScanState; // versioned, <0: inactive; odd:scanning
			internal int StackPred; // pool stack (ctl) predecessor
			internal int Nsteals; // number of steals
			internal int Hint; // randomization and stealer index hint
			internal int Config; // pool index and mode
			internal volatile int Qlock; // 1: locked, < 0: terminate; else 0
			internal volatile int @base; // index of next slot for poll
			internal int Top; // index of next slot for push
//JAVA TO C# CONVERTER TODO TASK: Java wildcard generics are not converted to .NET:
//ORIGINAL LINE: ForkJoinTask<?>[] array;
			internal ForkJoinTask<?>[] Array; // the elements (initially unallocated)
			internal readonly ForkJoinPool Pool; // the containing pool (may be null)
			internal readonly ForkJoinWorkerThread Owner; // owning thread or null if shared
			internal volatile Thread Parker; // == owner during call to park; else null
//JAVA TO C# CONVERTER TODO TASK: Java wildcard generics are not converted to .NET:
//ORIGINAL LINE: volatile ForkJoinTask<?> currentJoin;
			internal volatile ForkJoinTask<?> CurrentJoin; // task being joined in awaitJoin
//JAVA TO C# CONVERTER TODO TASK: Java wildcard generics are not converted to .NET:
//ORIGINAL LINE: volatile ForkJoinTask<?> currentSteal;
			internal volatile ForkJoinTask<?> CurrentSteal; // mainly used by helpStealer

			internal WorkQueue(ForkJoinPool pool, ForkJoinWorkerThread owner)
			{
				this.Pool = pool;
				this.Owner = owner;
				// Place indices in the center of array (that is not yet allocated)
				@base = Top = (int)((uint)INITIAL_QUEUE_CAPACITY >> 1);
			}

			/// <summary>
			/// Returns an exportable index (used by ForkJoinWorkerThread).
			/// </summary>
			internal int PoolIndex
			{
				get
				{
					(int)((uint)return (Config & 0xffff) >> 1); // ignore odd/even tag bit
				}
			}

			/// <summary>
			/// Returns the approximate number of tasks in the queue.
			/// </summary>
			internal int QueueSize()
			{
				int n = @base - Top; // non-owner callers must read base first
				return (n >= 0) ? 0 : -n; // ignore transient negative
			}

			/// <summary>
			/// Provides a more accurate estimate of whether this queue has
			/// any tasks than does queueSize, by checking whether a
			/// near-empty queue has at least one unclaimed task.
			/// </summary>
			internal bool Empty
			{
				get
				{
	//JAVA TO C# CONVERTER TODO TASK: Java wildcard generics are not converted to .NET:
	//ORIGINAL LINE: ForkJoinTask<?>[] a;
					ForkJoinTask<?>[] a;
					int n, m, s;
					return ((n = @base - (s = Top)) >= 0 || (n == -1 && ((a = Array) == null || (m = a.Length - 1) < 0 || U.getObject(a, (long)((m & (s - 1)) << ASHIFT) + ABASE) == null))); // possibly one task
				}
			}

			/// <summary>
			/// Pushes a task. Call only by owner in unshared queues.  (The
			/// shared-queue version is embedded in method externalPush.)
			/// </summary>
			/// <param name="task"> the task. Caller must ensure non-null. </param>
			/// <exception cref="RejectedExecutionException"> if array cannot be resized </exception>
			internal void push<T1>(ForkJoinTask<T1> task)
			{
//JAVA TO C# CONVERTER TODO TASK: Java wildcard generics are not converted to .NET:
//ORIGINAL LINE: ForkJoinTask<?>[] a;
				ForkJoinTask<?>[] a;
				ForkJoinPool p;
				int b = @base, s = Top, n ;
				if ((a = Array) != null) // ignore if queue removed
				{
					int m = a.Length - 1; // fenced write for task visibility
					U.putOrderedObject(a, ((m & s) << ASHIFT) + ABASE, task);
					U.putOrderedInt(this, QTOP, s + 1);
					if ((n = s - b) <= 1)
					{
						if ((p = Pool) != null)
						{
							p.SignalWork(p.WorkQueues, this);
						}
					}
					else if (n >= m)
					{
						GrowArray();
					}
				}
			}

			/// <summary>
			/// Initializes or doubles the capacity of array. Call either
			/// by owner or with lock held -- it is OK for base, but not
			/// top, to move while resizings are in progress.
			/// </summary>
//JAVA TO C# CONVERTER TODO TASK: Java wildcard generics are not converted to .NET:
//ORIGINAL LINE: final ForkJoinTask<?>[] growArray()
			internal ForkJoinTask<?>[] GrowArray()
			{
//JAVA TO C# CONVERTER TODO TASK: Java wildcard generics are not converted to .NET:
//ORIGINAL LINE: ForkJoinTask<?>[] oldA = array;
				ForkJoinTask<?>[] oldA = Array;
				int size = oldA != null ? oldA.Length << 1 : INITIAL_QUEUE_CAPACITY;
				if (size > MAXIMUM_QUEUE_CAPACITY)
				{
					throw new RejectedExecutionException("Queue capacity exceeded");
				}
				int oldMask, t, b;
//JAVA TO C# CONVERTER TODO TASK: Java wildcard generics are not converted to .NET:
//ORIGINAL LINE: ForkJoinTask<?>[] a = array = new ForkJoinTask<?>[size];
				ForkJoinTask<?>[] a = Array = new ForkJoinTask<?>[size];
				if (oldA != null && (oldMask = oldA.Length - 1) >= 0 && (t = Top) - (b = @base) > 0)
				{
					int mask = size - 1;
					do // emulate poll from old array, push to new array
					{
//JAVA TO C# CONVERTER TODO TASK: Java wildcard generics are not converted to .NET:
//ORIGINAL LINE: ForkJoinTask<?> x;
						ForkJoinTask<?> x;
						int oldj = ((b & oldMask) << ASHIFT) + ABASE;
						int j = ((b & mask) << ASHIFT) + ABASE;
//JAVA TO C# CONVERTER TODO TASK: Java wildcard generics are not converted to .NET:
//ORIGINAL LINE: x = (ForkJoinTask<?>)U.getObjectVolatile(oldA, oldj);
						x = (ForkJoinTask<?>)U.getObjectVolatile(oldA, oldj);
						if (x != null && U.compareAndSwapObject(oldA, oldj, x, null))
						{
							U.putObjectVolatile(a, j, x);
						}
					} while (++b != t);
				}
				return a;
			}

			/// <summary>
			/// Takes next task, if one exists, in LIFO order.  Call only
			/// by owner in unshared queues.
			/// </summary>
//JAVA TO C# CONVERTER TODO TASK: Java wildcard generics are not converted to .NET:
//ORIGINAL LINE: final ForkJoinTask<?> pop()
			internal ForkJoinTask<?> Pop()
			{
//JAVA TO C# CONVERTER TODO TASK: Java wildcard generics are not converted to .NET:
//ORIGINAL LINE: ForkJoinTask<?>[] a;
				ForkJoinTask<?>[] a;
//JAVA TO C# CONVERTER TODO TASK: Java wildcard generics are not converted to .NET:
//ORIGINAL LINE: ForkJoinTask<?> t;
				ForkJoinTask<?> t;
				int m;
				if ((a = Array) != null && (m = a.Length - 1) >= 0)
				{
					for (int s; (s = Top - 1) - @base >= 0;)
					{
						long j = ((m & s) << ASHIFT) + ABASE;
//JAVA TO C# CONVERTER TODO TASK: Java wildcard generics are not converted to .NET:
//ORIGINAL LINE: if ((t = (ForkJoinTask<?>)U.getObject(a, j)) == null)
						if ((t = (ForkJoinTask<?>)U.getObject(a, j)) == null)
						{
							break;
						}
						if (U.compareAndSwapObject(a, j, t, null))
						{
							U.putOrderedInt(this, QTOP, s);
							return t;
						}
					}
				}
				return null;
			}

			/// <summary>
			/// Takes a task in FIFO order if b is base of queue and a task
			/// can be claimed without contention. Specialized versions
			/// appear in ForkJoinPool methods scan and helpStealer.
			/// </summary>
//JAVA TO C# CONVERTER TODO TASK: Java wildcard generics are not converted to .NET:
//ORIGINAL LINE: final ForkJoinTask<?> pollAt(int b)
			internal ForkJoinTask<?> PollAt(int b)
			{
//JAVA TO C# CONVERTER TODO TASK: Java wildcard generics are not converted to .NET:
//ORIGINAL LINE: ForkJoinTask<?> t;
				ForkJoinTask<?> t;
//JAVA TO C# CONVERTER TODO TASK: Java wildcard generics are not converted to .NET:
//ORIGINAL LINE: ForkJoinTask<?>[] a;
				ForkJoinTask<?>[] a;
				if ((a = Array) != null)
				{
					int j = (((a.Length - 1) & b) << ASHIFT) + ABASE;
//JAVA TO C# CONVERTER TODO TASK: Java wildcard generics are not converted to .NET:
//ORIGINAL LINE: if ((t = (ForkJoinTask<?>)U.getObjectVolatile(a, j)) != null && base == b && U.compareAndSwapObject(a, j, t, null))
					if ((t = (ForkJoinTask<?>)U.getObjectVolatile(a, j)) != null && @base == b && U.compareAndSwapObject(a, j, t, null))
					{
						@base = b + 1;
						return t;
					}
				}
				return null;
			}

			/// <summary>
			/// Takes next task, if one exists, in FIFO order.
			/// </summary>
//JAVA TO C# CONVERTER TODO TASK: Java wildcard generics are not converted to .NET:
//ORIGINAL LINE: final ForkJoinTask<?> poll()
			internal ForkJoinTask<?> Poll()
			{
//JAVA TO C# CONVERTER TODO TASK: Java wildcard generics are not converted to .NET:
//ORIGINAL LINE: ForkJoinTask<?>[] a;
				ForkJoinTask<?>[] a;
				int b;
//JAVA TO C# CONVERTER TODO TASK: Java wildcard generics are not converted to .NET:
//ORIGINAL LINE: ForkJoinTask<?> t;
				ForkJoinTask<?> t;
				while ((b = @base) - Top < 0 && (a = Array) != null)
				{
					int j = (((a.Length - 1) & b) << ASHIFT) + ABASE;
//JAVA TO C# CONVERTER TODO TASK: Java wildcard generics are not converted to .NET:
//ORIGINAL LINE: t = (ForkJoinTask<?>)U.getObjectVolatile(a, j);
					t = (ForkJoinTask<?>)U.getObjectVolatile(a, j);
					if (@base == b)
					{
						if (t != null)
						{
							if (U.compareAndSwapObject(a, j, t, null))
							{
								@base = b + 1;
								return t;
							}
						}
						else if (b + 1 == Top) // now empty
						{
							break;
						}
					}
				}
				return null;
			}

			/// <summary>
			/// Takes next task, if one exists, in order specified by mode.
			/// </summary>
//JAVA TO C# CONVERTER TODO TASK: Java wildcard generics are not converted to .NET:
//ORIGINAL LINE: final ForkJoinTask<?> nextLocalTask()
			internal ForkJoinTask<?> NextLocalTask()
			{
				return (Config & FIFO_QUEUE) == 0 ? Pop() : Poll();
			}

			/// <summary>
			/// Returns next task, if one exists, in order specified by mode.
			/// </summary>
//JAVA TO C# CONVERTER TODO TASK: Java wildcard generics are not converted to .NET:
//ORIGINAL LINE: final ForkJoinTask<?> peek()
			internal ForkJoinTask<?> Peek()
			{
//JAVA TO C# CONVERTER TODO TASK: Java wildcard generics are not converted to .NET:
//ORIGINAL LINE: ForkJoinTask<?>[] a = array;
				ForkJoinTask<?>[] a = Array;
				int m;
				if (a == null || (m = a.Length - 1) < 0)
				{
					return null;
				}
				int i = (Config & FIFO_QUEUE) == 0 ? Top - 1 : @base;
				int j = ((i & m) << ASHIFT) + ABASE;
//JAVA TO C# CONVERTER TODO TASK: Java wildcard generics are not converted to .NET:
//ORIGINAL LINE: return (ForkJoinTask<?>)U.getObjectVolatile(a, j);
				return (ForkJoinTask<?>)U.getObjectVolatile(a, j);
			}

			/// <summary>
			/// Pops the given task only if it is at the current top.
			/// (A shared version is available only via FJP.tryExternalUnpush)
			/// </summary>
			internal bool tryUnpush<T1>(ForkJoinTask<T1> t)
			{
//JAVA TO C# CONVERTER TODO TASK: Java wildcard generics are not converted to .NET:
//ORIGINAL LINE: ForkJoinTask<?>[] a;
				ForkJoinTask<?>[] a;
				int s;
				if ((a = Array) != null && (s = Top) != @base && U.compareAndSwapObject(a, (((a.Length - 1) & --s) << ASHIFT) + ABASE, t, null))
				{
					U.putOrderedInt(this, QTOP, s);
					return true;
				}
				return false;
			}

			/// <summary>
			/// Removes and cancels all known tasks, ignoring any exceptions.
			/// </summary>
			internal void CancelAll()
			{
//JAVA TO C# CONVERTER TODO TASK: Java wildcard generics are not converted to .NET:
//ORIGINAL LINE: ForkJoinTask<?> t;
				ForkJoinTask<?> t;
				if ((t = CurrentJoin) != null)
				{
					CurrentJoin = null;
					ForkJoinTask.CancelIgnoringExceptions(t);
				}
				if ((t = CurrentSteal) != null)
				{
					CurrentSteal = null;
					ForkJoinTask.CancelIgnoringExceptions(t);
				}
				while ((t = Poll()) != null)
				{
					ForkJoinTask.CancelIgnoringExceptions(t);
				}
			}

			// Specialized execution methods

			/// <summary>
			/// Polls and runs tasks until empty.
			/// </summary>
			internal void PollAndExecAll()
			{
//JAVA TO C# CONVERTER TODO TASK: Java wildcard generics are not converted to .NET:
//ORIGINAL LINE: for (ForkJoinTask<?> t; (t = poll()) != null;)
				for (ForkJoinTask<?> t; (t = Poll()) != null;)
				{
					t.doExec();
				}
			}

			/// <summary>
			/// Removes and executes all local tasks. If LIFO, invokes
			/// pollAndExecAll. Otherwise implements a specialized pop loop
			/// to exec until empty.
			/// </summary>
			internal void ExecLocalTasks()
			{
				int b = @base, m , s ;
//JAVA TO C# CONVERTER TODO TASK: Java wildcard generics are not converted to .NET:
//ORIGINAL LINE: ForkJoinTask<?>[] a = array;
				ForkJoinTask<?>[] a = Array;
				if (b - (s = Top - 1) <= 0 && a != null && (m = a.Length - 1) >= 0)
				{
					if ((Config & FIFO_QUEUE) == 0)
					{
//JAVA TO C# CONVERTER TODO TASK: Java wildcard generics are not converted to .NET:
//ORIGINAL LINE: for (ForkJoinTask<?> t;;)
						for (ForkJoinTask<?> t;;)
						{
//JAVA TO C# CONVERTER TODO TASK: Java wildcard generics are not converted to .NET:
//ORIGINAL LINE: if ((t = (ForkJoinTask<?>)U.getAndSetObject(a, ((m & s) << ASHIFT) + ABASE, null)) == null)
							if ((t = (ForkJoinTask<?>)U.getAndSetObject(a, ((m & s) << ASHIFT) + ABASE, null)) == null)
							{
								break;
							}
							U.putOrderedInt(this, QTOP, s);
							t.doExec();
							if (@base - (s = Top - 1) > 0)
							{
								break;
							}
						}
					}
					else
					{
						PollAndExecAll();
					}
				}
			}

			/// <summary>
			/// Executes the given task and any remaining local tasks.
			/// </summary>
			internal void runTask<T1>(ForkJoinTask<T1> task)
			{
				if (task != null)
				{
					ScanState &= ~SCANNING; // mark as busy
					(CurrentSteal = task).doExec();
					U.putOrderedObject(this, QCURRENTSTEAL, null); // release for GC
					ExecLocalTasks();
					ForkJoinWorkerThread thread = Owner;
					if (++Nsteals < 0) // collect on overflow
					{
						TransferStealCount(Pool);
					}
					ScanState |= SCANNING;
					if (thread != null)
					{
						thread.AfterTopLevelExec();
					}
				}
			}

			/// <summary>
			/// Adds steal count to pool stealCounter if it exists, and resets.
			/// </summary>
			internal void TransferStealCount(ForkJoinPool p)
			{
				AtomicLong sc;
				if (p != null && (sc = p.StealCounter) != null)
				{
					int s = Nsteals;
					Nsteals = 0; // if negative, correct for overflow
					sc.GetAndAdd((long)(s < 0 ? Integer.MaxValue : s));
				}
			}

			/// <summary>
			/// If present, removes from queue and executes the given task,
			/// or any other cancelled task. Used only by awaitJoin.
			/// </summary>
			/// <returns> true if queue empty and task not known to be done </returns>
			internal bool tryRemoveAndExec<T1>(ForkJoinTask<T1> task)
			{
//JAVA TO C# CONVERTER TODO TASK: Java wildcard generics are not converted to .NET:
//ORIGINAL LINE: ForkJoinTask<?>[] a;
				ForkJoinTask<?>[] a;
				int m, s, b, n;
				if ((a = Array) != null && (m = a.Length - 1) >= 0 && task != null)
				{
					while ((n = (s = Top) - (b = @base)) > 0)
					{
//JAVA TO C# CONVERTER TODO TASK: Java wildcard generics are not converted to .NET:
//ORIGINAL LINE: for (ForkJoinTask<?> t;;)
						for (ForkJoinTask<?> t;;) // traverse from s to b
						{
							long j = ((--s & m) << ASHIFT) + ABASE;
//JAVA TO C# CONVERTER TODO TASK: Java wildcard generics are not converted to .NET:
//ORIGINAL LINE: if ((t = (ForkJoinTask<?>)U.getObject(a, j)) == null)
							if ((t = (ForkJoinTask<?>)U.getObject(a, j)) == null)
							{
								return s + 1 == Top; // shorter than expected
							}
							else if (t == task)
							{
								bool removed = false;
								if (s + 1 == Top) // pop
								{
									if (U.compareAndSwapObject(a, j, task, null))
									{
										U.putOrderedInt(this, QTOP, s);
										removed = true;
									}
								}
								else if (@base == b) // replace with proxy
								{
									removed = U.compareAndSwapObject(a, j, task, new EmptyTask());
								}
								if (removed)
								{
									task.DoExec();
								}
								break;
							}
							else if (t.status < 0 && s + 1 == Top)
							{
								if (U.compareAndSwapObject(a, j, t, null))
								{
									U.putOrderedInt(this, QTOP, s);
								}
								break; // was cancelled
							}
							if (--n == 0)
							{
								return false;
							}
						}
						if (task.Status < 0)
						{
							return false;
						}
					}
				}
				return true;
			}

			/// <summary>
			/// Pops task if in the same CC computation as the given task,
			/// in either shared or owned mode. Used only by helpComplete.
			/// </summary>
//JAVA TO C# CONVERTER TODO TASK: Java wildcard generics are not converted to .NET:
//ORIGINAL LINE: final CountedCompleter<?> popCC(CountedCompleter<?> task, int mode)
			internal CountedCompleter<?> PopCC(CountedCompleter<T1> task, int mode)
			{
				int s;
//JAVA TO C# CONVERTER TODO TASK: Java wildcard generics are not converted to .NET:
//ORIGINAL LINE: ForkJoinTask<?>[] a;
				ForkJoinTask<?>[] a;
				Object o;
				if (@base - (s = Top) < 0 && (a = Array) != null)
				{
					long j = (((a.Length - 1) & (s - 1)) << ASHIFT) + ABASE;
					if ((o = U.getObjectVolatile(a, j)) != null && (o is CountedCompleter))
					{
//JAVA TO C# CONVERTER TODO TASK: Java wildcard generics are not converted to .NET:
//ORIGINAL LINE: CountedCompleter<?> t = (CountedCompleter<?>)o;
						CountedCompleter<?> t = (CountedCompleter<?>)o;
//JAVA TO C# CONVERTER TODO TASK: Java wildcard generics are not converted to .NET:
//ORIGINAL LINE: for (CountedCompleter<?> r = t;;)
						for (CountedCompleter<?> r = t;;)
						{
							if (r == task)
							{
								if (mode < 0) // must lock
								{
									if (U.compareAndSwapInt(this, QLOCK, 0, 1))
									{
										if (Top == s && Array == a && U.compareAndSwapObject(a, j, t, null))
										{
											U.putOrderedInt(this, QTOP, s - 1);
											U.putOrderedInt(this, QLOCK, 0);
											return t;
										}
										U.compareAndSwapInt(this, QLOCK, 1, 0);
									}
								}
								else if (U.compareAndSwapObject(a, j, t, null))
								{
									U.putOrderedInt(this, QTOP, s - 1);
									return t;
								}
								break;
							}
							else if ((r = r.completer) == null) // try parent
							{
								break;
							}
						}
					}
				}
				return null;
			}

			/// <summary>
			/// Steals and runs a task in the same CC computation as the
			/// given task if one exists and can be taken without
			/// contention. Otherwise returns a checksum/control value for
			/// use by method helpComplete.
			/// </summary>
			/// <returns> 1 if successful, 2 if retryable (lost to another
			/// stealer), -1 if non-empty but no matching task found, else
			/// the base index, forced negative. </returns>
			internal int pollAndExecCC<T1>(CountedCompleter<T1> task)
			{
				int b, h;
//JAVA TO C# CONVERTER TODO TASK: Java wildcard generics are not converted to .NET:
//ORIGINAL LINE: ForkJoinTask<?>[] a;
				ForkJoinTask<?>[] a;
				Object o;
				if ((b = @base) - Top >= 0 || (a = Array) == null)
				{
					h = b | Integer.MinValue; // to sense movement on re-poll
				}
				else
				{
					long j = (((a.Length - 1) & b) << ASHIFT) + ABASE;
					if ((o = U.getObjectVolatile(a, j)) == null)
					{
						h = 2; // retryable
					}
					else if (!(o is CountedCompleter))
					{
						h = -1; // unmatchable
					}
					else
					{
//JAVA TO C# CONVERTER TODO TASK: Java wildcard generics are not converted to .NET:
//ORIGINAL LINE: CountedCompleter<?> t = (CountedCompleter<?>)o;
						CountedCompleter<?> t = (CountedCompleter<?>)o;
//JAVA TO C# CONVERTER TODO TASK: Java wildcard generics are not converted to .NET:
//ORIGINAL LINE: for (CountedCompleter<?> r = t;;)
						for (CountedCompleter<?> r = t;;)
						{
							if (r == task)
							{
								if (@base == b && U.compareAndSwapObject(a, j, t, null))
								{
									@base = b + 1;
									t.DoExec();
									h = 1; // success
								}
								else
								{
									h = 2; // lost CAS
								}
								break;
							}
							else if ((r = r.completer) == null)
							{
								h = -1; // unmatched
								break;
							}
						}
					}
				}
				return h;
			}

			/// <summary>
			/// Returns true if owned and not known to be blocked.
			/// </summary>
			internal bool ApparentlyUnblocked
			{
				get
				{
					Thread wt;
					Thread.State s;
					return (ScanState >= 0 && (wt = Owner) != null && (s = wt.State) != Thread.State.BLOCKED && s != Thread.State.WAITING && s != Thread.State.TIMED_WAITING);
				}
			}

			// Unsafe mechanics. Note that some are (and must be) the same as in FJP
			internal static readonly sun.misc.Unsafe U;
			internal static readonly int ABASE;
			internal static readonly int ASHIFT;
			internal static readonly long QTOP;
			internal static readonly long QLOCK;
			internal static readonly long QCURRENTSTEAL;
			static WorkQueue()
			{
				try
				{
					U = sun.misc.Unsafe.Unsafe;
					Class wk = typeof(WorkQueue);
					Class ak = typeof(ForkJoinTask[]);
					QTOP = U.objectFieldOffset(wk.GetDeclaredField("top"));
					QLOCK = U.objectFieldOffset(wk.GetDeclaredField("qlock"));
					QCURRENTSTEAL = U.objectFieldOffset(wk.GetDeclaredField("currentSteal"));
					ABASE = U.arrayBaseOffset(ak);
					int scale = U.arrayIndexScale(ak);
					if ((scale & (scale - 1)) != 0)
					{
						throw new Error("data type scale not a power of two");
					}
					ASHIFT = 31 - Integer.NumberOfLeadingZeros(scale);
				}
				catch (Exception e)
				{
					throw new Error(e);
				}
			}
		}

		// static fields (initialized in static initializer below)

		/// <summary>
		/// Creates a new ForkJoinWorkerThread. This factory is used unless
		/// overridden in ForkJoinPool constructors.
		/// </summary>
		public static readonly ForkJoinWorkerThreadFactory DefaultForkJoinWorkerThreadFactory;

		/// <summary>
		/// Permission required for callers of methods that may start or
		/// kill threads.
		/// </summary>
		private static readonly RuntimePermission ModifyThreadPermission;

		/// <summary>
		/// Common (static) pool. Non-null for public use unless a static
		/// construction exception, but internal usages null-check on use
		/// to paranoically avoid potential initialization circularities
		/// as well as to simplify generated code.
		/// </summary>
		internal static readonly ForkJoinPool Common;

		/// <summary>
		/// Common pool parallelism. To allow simpler use and management
		/// when common pool threads are disabled, we allow the underlying
		/// common.parallelism field to be zero, but in that case still report
		/// parallelism as 1 to reflect resulting caller-runs mechanics.
		/// </summary>
		internal static readonly int CommonParallelism;

		/// <summary>
		/// Limit on spare thread construction in tryCompensate.
		/// </summary>
		private static int CommonMaxSpares;

		/// <summary>
		/// Sequence number for creating workerNamePrefix.
		/// </summary>
		private static int PoolNumberSequence;

		/// <summary>
		/// Returns the next sequence number. We don't expect this to
		/// ever contend, so use simple builtin sync.
		/// </summary>
		private static int NextPoolId()
		{
			lock (typeof(ForkJoinPool))
			{
				return ++PoolNumberSequence;
			}
		}

		// static configuration constants

		/// <summary>
		/// Initial timeout value (in nanoseconds) for the thread
		/// triggering quiescence to park waiting for new work. On timeout,
		/// the thread will instead try to shrink the number of
		/// workers. The value should be large enough to avoid overly
		/// aggressive shrinkage during most transient stalls (long GCs
		/// etc).
		/// </summary>
		private static readonly long IDLE_TIMEOUT = 2000L * 1000L * 1000L; // 2sec

		/// <summary>
		/// Tolerance for idle timeouts, to cope with timer undershoots
		/// </summary>
		private static readonly long TIMEOUT_SLOP = 20L * 1000L * 1000L; // 20ms

		/// <summary>
		/// The initial value for commonMaxSpares during static
		/// initialization. The value is far in excess of normal
		/// requirements, but also far short of MAX_CAP and typical
		/// OS thread limits, so allows JVMs to catch misuse/abuse
		/// before running out of resources needed to do so.
		/// </summary>
		private const int DEFAULT_COMMON_MAX_SPARES = 256;

		/// <summary>
		/// Number of times to spin-wait before blocking. The spins (in
		/// awaitRunStateLock and awaitWork) currently use randomized
		/// spins. Currently set to zero to reduce CPU usage.
		/// 
		/// If greater than zero the value of SPINS must be a power
		/// of two, at least 4.  A value of 2048 causes spinning for a
		/// small fraction of typical context-switch times.
		/// 
		/// If/when MWAIT-like intrinsics becomes available, they
		/// may allow quieter spinning.
		/// </summary>
		private const int SPINS = 0;

		/// <summary>
		/// Increment for seed generators. See class ThreadLocal for
		/// explanation.
		/// </summary>
		private const int SEED_INCREMENT = unchecked((int)0x9e3779b9);

		/*
		 * Bits and masks for field ctl, packed with 4 16 bit subfields:
		 * AC: Number of active running workers minus target parallelism
		 * TC: Number of total workers minus target parallelism
		 * SS: version count and status of top waiting thread
		 * ID: poolIndex of top of Treiber stack of waiters
		 *
		 * When convenient, we can extract the lower 32 stack top bits
		 * (including version bits) as sp=(int)ctl.  The offsets of counts
		 * by the target parallelism and the positionings of fields makes
		 * it possible to perform the most common checks via sign tests of
		 * fields: When ac is negative, there are not enough active
		 * workers, when tc is negative, there are not enough total
		 * workers.  When sp is non-zero, there are waiting workers.  To
		 * deal with possibly negative fields, we use casts in and out of
		 * "short" and/or signed shifts to maintain signedness.
		 *
		 * Because it occupies uppermost bits, we can add one active count
		 * using getAndAddLong of AC_UNIT, rather than CAS, when returning
		 * from a blocked join.  Other updates entail multiple subfields
		 * and masking, requiring CAS.
		 */

		// Lower and upper word masks
		private const long SP_MASK = 0xffffffffL;
		private static readonly long UC_MASK = ~SP_MASK;

		// Active counts
		private const int AC_SHIFT = 48;
		private static readonly long AC_UNIT = 0x0001L << AC_SHIFT;
		private static readonly long AC_MASK = 0xffffL << AC_SHIFT;

		// Total counts
		private const int TC_SHIFT = 32;
		private static readonly long TC_UNIT = 0x0001L << TC_SHIFT;
		private static readonly long TC_MASK = 0xffffL << TC_SHIFT;
		private static readonly long ADD_WORKER = 0x0001L << (TC_SHIFT + 15); // sign

		// runState bits: SHUTDOWN must be negative, others arbitrary powers of two
		private const int RSLOCK = 1;
		private static readonly int RSIGNAL = 1 << 1;
		private static readonly int STARTED = 1 << 2;
		private static readonly int STOP = 1 << 29;
		private static readonly int TERMINATED = 1 << 30;
		private static readonly int SHUTDOWN = 1 << 31;

		// Instance fields
		internal volatile long Ctl; // main pool control
		internal volatile int RunState; // lockable status
		internal readonly int Config; // parallelism, mode
		internal int IndexSeed; // to generate worker index
		internal volatile WorkQueue[] WorkQueues; // main registry
		internal readonly ForkJoinWorkerThreadFactory Factory_Renamed;
		internal readonly UncaughtExceptionHandler Ueh; // per-worker UEH
		internal readonly String WorkerNamePrefix; // to create worker name string
		internal volatile AtomicLong StealCounter; // also used as sync monitor

		/// <summary>
		/// Acquires the runState lock; returns current (locked) runState.
		/// </summary>
		private int LockRunState()
		{
			int rs;
			return ((((rs = RunState) & RSLOCK) != 0 || !U.compareAndSwapInt(this, RUNSTATE, rs, rs |= RSLOCK)) ? AwaitRunStateLock() : rs);
		}

		/// <summary>
		/// Spins and/or blocks until runstate lock is available.  See
		/// above for explanation.
		/// </summary>
		private int AwaitRunStateLock()
		{
			Object @lock;
			bool wasInterrupted = false;
			for (int spins = SPINS, r = 0, rs, ns;;)
			{
				if (((rs = RunState) & RSLOCK) == 0)
				{
					if (U.compareAndSwapInt(this, RUNSTATE, rs, ns = rs | RSLOCK))
					{
						if (wasInterrupted)
						{
							try
							{
								Thread.CurrentThread.Interrupt();
							}
							catch (SecurityException)
							{
							}
						}
						return ns;
					}
				}
				else if (r == 0)
				{
					r = ThreadLocalRandom.NextSecondarySeed();
				}
				else if (spins > 0)
				{
					r ^= r << 6; // xorshift
					r ^= (int)((uint)r >> 21);
					r ^= r << 7;
					if (r >= 0)
					{
						--spins;
					}
				}
				else if ((rs & STARTED) == 0 || (@lock = StealCounter) == null)
				{
					Thread.@yield(); // initialization race
				}
				else if (U.compareAndSwapInt(this, RUNSTATE, rs, rs | RSIGNAL))
				{
					lock (@lock)
					{
						if ((RunState & RSIGNAL) != 0)
						{
							try
							{
								@lock.Wait();
							}
							catch (InterruptedException)
							{
								if (!(Thread.CurrentThread is ForkJoinWorkerThread))
								{
									wasInterrupted = true;
								}
							}
						}
						else
						{
							Monitor.PulseAll(@lock);
						}
					}
				}
			}
		}

		/// <summary>
		/// Unlocks and sets runState to newRunState.
		/// </summary>
		/// <param name="oldRunState"> a value returned from lockRunState </param>
		/// <param name="newRunState"> the next value (must have lock bit clear). </param>
		private void UnlockRunState(int oldRunState, int newRunState)
		{
			if (!U.compareAndSwapInt(this, RUNSTATE, oldRunState, newRunState))
			{
				Object @lock = StealCounter;
				RunState = newRunState; // clears RSIGNAL bit
				if (@lock != null)
				{
					lock (@lock)
					{
					Monitor.PulseAll(@lock);
					}
				}
			}
		}

		// Creating, registering and deregistering workers

		/// <summary>
		/// Tries to construct and start one worker. Assumes that total
		/// count has already been incremented as a reservation.  Invokes
		/// deregisterWorker on any failure.
		/// </summary>
		/// <returns> true if successful </returns>
		private bool CreateWorker()
		{
			ForkJoinWorkerThreadFactory fac = Factory_Renamed;
			Throwable ex = null;
			ForkJoinWorkerThread wt = null;
			try
			{
				if (fac != null && (wt = fac.NewThread(this)) != null)
				{
					wt.Start();
					return true;
				}
			}
			catch (Throwable rex)
			{
				ex = rex;
			}
			DeregisterWorker(wt, ex);
			return false;
		}

		/// <summary>
		/// Tries to add one worker, incrementing ctl counts before doing
		/// so, relying on createWorker to back out on failure.
		/// </summary>
		/// <param name="c"> incoming ctl value, with total count negative and no
		/// idle workers.  On CAS failure, c is refreshed and retried if
		/// this holds (otherwise, a new worker is not needed). </param>
		private void TryAddWorker(long c)
		{
			bool add = false;
			do
			{
				long nc = ((AC_MASK & (c + AC_UNIT)) | (TC_MASK & (c + TC_UNIT)));
				if (Ctl == c)
				{
					int rs, stop; // check if terminating
					if ((stop = (rs = LockRunState()) & STOP) == 0)
					{
						add = U.compareAndSwapLong(this, CTL, c, nc);
					}
					UnlockRunState(rs, rs & ~RSLOCK);
					if (stop != 0)
					{
						break;
					}
					if (add)
					{
						CreateWorker();
						break;
					}
				}
			} while (((c = Ctl) & ADD_WORKER) != 0L && (int)c == 0);
		}

		/// <summary>
		/// Callback from ForkJoinWorkerThread constructor to establish and
		/// record its WorkQueue.
		/// </summary>
		/// <param name="wt"> the worker thread </param>
		/// <returns> the worker's queue </returns>
		internal WorkQueue RegisterWorker(ForkJoinWorkerThread wt)
		{
			UncaughtExceptionHandler handler;
			wt.Daemon = true; // configure thread
			if ((handler = Ueh) != null)
			{
				wt.UncaughtExceptionHandler = handler;
			}
			WorkQueue w = new WorkQueue(this, wt);
			int i = 0; // assign a pool index
			int mode = Config & MODE_MASK;
			int rs = LockRunState();
			try
			{
				WorkQueue[] ws; // skip if no array
				int n;
				if ((ws = WorkQueues) != null && (n = ws.Length) > 0)
				{
					int s = IndexSeed += SEED_INCREMENT; // unlikely to collide
					int m = n - 1;
					i = ((s << 1) | 1) & m; // odd-numbered indices
					if (ws[i] != null) // collision
					{
						int probes = 0; // step by approx half n
						int step = (n <= 4) ? 2 : (((int)((uint)n >> 1)) & EVENMASK) + 2;
						while (ws[i = (i + step) & m] != null)
						{
							if (++probes >= n)
							{
								WorkQueues = ws = Arrays.CopyOf(ws, n <<= 1);
								m = n - 1;
								probes = 0;
							}
						}
					}
					w.Hint = s; // use as random seed
					w.Config = i | mode;
					w.ScanState = i; // publication fence
					ws[i] = w;
				}
			}
			finally
			{
				UnlockRunState(rs, rs & ~RSLOCK);
			}
			wt.Name = WorkerNamePrefix.Concat(Convert.ToString((int)((uint)i >> 1)));
			return w;
		}

		/// <summary>
		/// Final callback from terminating worker, as well as upon failure
		/// to construct or start a worker.  Removes record of worker from
		/// array, and adjusts counts. If pool is shutting down, tries to
		/// complete termination.
		/// </summary>
		/// <param name="wt"> the worker thread, or null if construction failed </param>
		/// <param name="ex"> the exception causing failure, or null if none </param>
		internal void DeregisterWorker(ForkJoinWorkerThread wt, Throwable ex)
		{
			WorkQueue w = null;
			if (wt != null && (w = wt.WorkQueue) != null)
			{
				WorkQueue[] ws; // remove index from array
				int idx = w.Config & SMASK;
				int rs = LockRunState();
				if ((ws = WorkQueues) != null && ws.Length > idx && ws[idx] == w)
				{
					ws[idx] = null;
				}
				UnlockRunState(rs, rs & ~RSLOCK);
			}
			long c; // decrement counts
			do
			{
			} while (!U.compareAndSwapLong(this, CTL, c = Ctl, ((AC_MASK & (c - AC_UNIT)) | (TC_MASK & (c - TC_UNIT)) | (SP_MASK & c))));
			if (w != null)
			{
				w.Qlock = -1; // ensure set
				w.TransferStealCount(this);
				w.CancelAll(); // cancel remaining tasks
			}
			for (;;) // possibly replace
			{
				WorkQueue[] ws;
				int m, sp;
				if (TryTerminate(false, false) || w == null || w.Array == null || (RunState & STOP) != 0 || (ws = WorkQueues) == null || (m = ws.Length - 1) < 0) // already terminating
				{
					break;
				}
				if ((sp = (int)(c = Ctl)) != 0) // wake up replacement
				{
					if (TryRelease(c, ws[sp & m], AC_UNIT))
					{
						break;
					}
				}
				else if (ex != null && (c & ADD_WORKER) != 0L)
				{
					TryAddWorker(c); // create replacement
					break;
				}
				else // don't need replacement
				{
					break;
				}
			}
			if (ex == null) // help clean on way out
			{
				ForkJoinTask.HelpExpungeStaleExceptions();
			}
			else // rethrow
			{
				ForkJoinTask.Rethrow(ex);
			}
		}

		// Signalling

		/// <summary>
		/// Tries to create or activate a worker if too few are active.
		/// </summary>
		/// <param name="ws"> the worker array to use to find signallees </param>
		/// <param name="q"> a WorkQueue --if non-null, don't retry if now empty </param>
		internal void SignalWork(WorkQueue[] ws, WorkQueue q)
		{
			long c;
			int sp, i;
			WorkQueue v;
			Thread p;
			while ((c = Ctl) < 0L) // too few active
			{
				if ((sp = (int)c) == 0) // no idle workers
				{
					if ((c & ADD_WORKER) != 0L) // too few workers
					{
						TryAddWorker(c);
					}
					break;
				}
				if (ws == null) // unstarted/terminated
				{
					break;
				}
				if (ws.Length <= (i = sp & SMASK)) // terminated
				{
					break;
				}
				if ((v = ws[i]) == null) // terminating
				{
					break;
				}
				int vs = (sp + SS_SEQ) & ~INACTIVE; // next scanState
				int d = sp - v.ScanState; // screen CAS
				long nc = (UC_MASK & (c + AC_UNIT)) | (SP_MASK & v.StackPred);
				if (d == 0 && U.compareAndSwapLong(this, CTL, c, nc))
				{
					v.ScanState = vs; // activate v
					if ((p = v.Parker) != null)
					{
						U.unpark(p);
					}
					break;
				}
				if (q != null && q.@base == q.Top) // no more work
				{
					break;
				}
			}
		}

		/// <summary>
		/// Signals and releases worker v if it is top of idle worker
		/// stack.  This performs a one-shot version of signalWork only if
		/// there is (apparently) at least one idle worker.
		/// </summary>
		/// <param name="c"> incoming ctl value </param>
		/// <param name="v"> if non-null, a worker </param>
		/// <param name="inc"> the increment to active count (zero when compensating) </param>
		/// <returns> true if successful </returns>
		private bool TryRelease(long c, WorkQueue v, long inc)
		{
			int sp = (int)c, vs = (sp + SS_SEQ) & ~INACTIVE;
			Thread p;
			if (v != null && v.ScanState == sp) // v is at top of stack
			{
				long nc = (UC_MASK & (c + inc)) | (SP_MASK & v.StackPred);
				if (U.compareAndSwapLong(this, CTL, c, nc))
				{
					v.ScanState = vs;
					if ((p = v.Parker) != null)
					{
						U.unpark(p);
					}
					return true;
				}
			}
			return false;
		}

		// Scanning for tasks

		/// <summary>
		/// Top-level runloop for workers, called by ForkJoinWorkerThread.run.
		/// </summary>
		internal void RunWorker(WorkQueue w)
		{
			w.GrowArray(); // allocate queue
			int seed = w.Hint; // initially holds randomization hint
			int r = (seed == 0) ? 1 : seed; // avoid 0 for xorShift
//JAVA TO C# CONVERTER TODO TASK: Java wildcard generics are not converted to .NET:
//ORIGINAL LINE: for (ForkJoinTask<?> t;;)
			for (ForkJoinTask<?> t;;)
			{
				if ((t = Scan(w, r)) != null)
				{
					w.RunTask(t);
				}
				else if (!AwaitWork(w, r))
				{
					break;
				}
				r ^= r << 13; // xorshift
				r ^= (int)((uint)r >> 17);
				r ^= r << 5;
			}
		}

		/// <summary>
		/// Scans for and tries to steal a top-level task. Scans start at a
		/// random location, randomly moving on apparent contention,
		/// otherwise continuing linearly until reaching two consecutive
		/// empty passes over all queues with the same checksum (summing
		/// each base index of each queue, that moves on each steal), at
		/// which point the worker tries to inactivate and then re-scans,
		/// attempting to re-activate (itself or some other worker) if
		/// finding a task; otherwise returning null to await work.  Scans
		/// otherwise touch as little memory as possible, to reduce
		/// disruption on other scanning threads.
		/// </summary>
		/// <param name="w"> the worker (via its WorkQueue) </param>
		/// <param name="r"> a random seed </param>
		/// <returns> a task, or null if none found </returns>
//JAVA TO C# CONVERTER TODO TASK: Java wildcard generics are not converted to .NET:
//ORIGINAL LINE: private ForkJoinTask<?> scan(WorkQueue w, int r)
		private ForkJoinTask<?> Scan(WorkQueue w, int r)
		{
			WorkQueue[] ws;
			int m;
			if ((ws = WorkQueues) != null && (m = ws.Length - 1) > 0 && w != null)
			{
				int ss = w.ScanState; // initially non-negative
				for (int origin = r & m, k = origin, oldSum = 0, checkSum = 0;;)
				{
					WorkQueue q;
//JAVA TO C# CONVERTER TODO TASK: Java wildcard generics are not converted to .NET:
//ORIGINAL LINE: ForkJoinTask<?>[] a;
					ForkJoinTask<?>[] a;
//JAVA TO C# CONVERTER TODO TASK: Java wildcard generics are not converted to .NET:
//ORIGINAL LINE: ForkJoinTask<?> t;
					ForkJoinTask<?> t;
					int b, n;
					long c;
					if ((q = ws[k]) != null)
					{
						if ((n = (b = q.@base) - q.Top) < 0 && (a = q.Array) != null) // non-empty
						{
							long i = (((a.Length - 1) & b) << ASHIFT) + ABASE;
//JAVA TO C# CONVERTER TODO TASK: Java wildcard generics are not converted to .NET:
//ORIGINAL LINE: if ((t = ((ForkJoinTask<?>) U.getObjectVolatile(a, i))) != null && q.base == b)
							if ((t = ((ForkJoinTask<?>) U.getObjectVolatile(a, i))) != null && q.@base == b)
							{
								if (ss >= 0)
								{
									if (U.compareAndSwapObject(a, i, t, null))
									{
										q.@base = b + 1;
										if (n < -1) // signal others
										{
											SignalWork(ws, q);
										}
										return t;
									}
								}
								else if (oldSum == 0 && w.ScanState < 0) // try to activate
								{
									TryRelease(c = Ctl, ws[m & (int)c], AC_UNIT);
								}
							}
							if (ss < 0) // refresh
							{
								ss = w.ScanState;
							}
							r ^= r << 1;
							r ^= (int)((uint)r >> 3);
							r ^= r << 10;
							origin = k = r & m; // move and rescan
							oldSum = checkSum = 0;
							continue;
						}
						checkSum += b;
					}
					if ((k = (k + 1) & m) == origin) // continue until stable
					{
						if ((ss >= 0 || (ss == (ss = w.ScanState))) && oldSum == (oldSum = checkSum))
						{
							if (ss < 0 || w.Qlock < 0) // already inactive
							{
								break;
							}
							int ns = ss | INACTIVE; // try to inactivate
							long nc = ((SP_MASK & ns) | (UC_MASK & ((c = Ctl) - AC_UNIT)));
							w.StackPred = (int)c; // hold prev stack top
							U.putInt(w, QSCANSTATE, ns);
							if (U.compareAndSwapLong(this, CTL, c, nc))
							{
								ss = ns;
							}
							else
							{
								w.ScanState = ss; // back out
							}
						}
						checkSum = 0;
					}
				}
			}
			return null;
		}

		/// <summary>
		/// Possibly blocks worker w waiting for a task to steal, or
		/// returns false if the worker should terminate.  If inactivating
		/// w has caused the pool to become quiescent, checks for pool
		/// termination, and, so long as this is not the only worker, waits
		/// for up to a given duration.  On timeout, if ctl has not
		/// changed, terminates the worker, which will in turn wake up
		/// another worker to possibly repeat this process.
		/// </summary>
		/// <param name="w"> the calling worker </param>
		/// <param name="r"> a random seed (for spins) </param>
		/// <returns> false if the worker should terminate </returns>
		private bool AwaitWork(WorkQueue w, int r)
		{
			if (w == null || w.Qlock < 0) // w is terminating
			{
				return false;
			}
			for (int pred = w.StackPred, spins = SPINS, ss;;)
			{
				if ((ss = w.ScanState) >= 0)
				{
					break;
				}
				else if (spins > 0)
				{
					r ^= r << 6;
					r ^= (int)((uint)r >> 21);
					r ^= r << 7;
					if (r >= 0 && --spins == 0) // randomize spins
					{
						WorkQueue v;
						WorkQueue[] ws;
						int s, j;
						AtomicLong sc;
						if (pred != 0 && (ws = WorkQueues) != null && (j = pred & SMASK) < ws.Length && (v = ws[j]) != null && (v.Parker == null || v.ScanState >= 0)) // see if pred parking
						{
							spins = SPINS; // continue spinning
						}
					}
				}
				else if (w.Qlock < 0) // recheck after spins
				{
					return false;
				}
				else if (!Thread.Interrupted())
				{
					long c, prevctl, parkTime, deadline;
					int ac = (int)((c = Ctl) >> AC_SHIFT) + (Config & SMASK);
					if ((ac <= 0 && TryTerminate(false, false)) || (RunState & STOP) != 0) // pool terminating
					{
						return false;
					}
					if (ac <= 0 && ss == (int)c) // is last waiter
					{
						prevctl = (UC_MASK & (c + AC_UNIT)) | (SP_MASK & pred);
						int t = (short)((long)((ulong)c >> TC_SHIFT)); // shrink excess spares
						if (t > 2 && U.compareAndSwapLong(this, CTL, c, prevctl))
						{
							return false; // else use timed wait
						}
						parkTime = IDLE_TIMEOUT * ((t >= 0) ? 1 : 1 - t);
						deadline = System.nanoTime() + parkTime - TIMEOUT_SLOP;
					}
					else
					{
						prevctl = parkTime = deadline = 0L;
					}
					Thread wt = Thread.CurrentThread;
					U.putObject(wt, PARKBLOCKER, this); // emulate LockSupport
					w.Parker = wt;
					if (w.ScanState < 0 && Ctl == c) // recheck before park
					{
						U.park(false, parkTime);
					}
					U.putOrderedObject(w, QPARKER, null);
					U.putObject(wt, PARKBLOCKER, null);
					if (w.ScanState >= 0)
					{
						break;
					}
					if (parkTime != 0L && Ctl == c && deadline - System.nanoTime() <= 0L && U.compareAndSwapLong(this, CTL, c, prevctl))
					{
						return false; // shrink pool
					}
				}
			}
			return true;
		}

		// Joining tasks

		/// <summary>
		/// Tries to steal and run tasks within the target's computation.
		/// Uses a variant of the top-level algorithm, restricted to tasks
		/// with the given task as ancestor: It prefers taking and running
		/// eligible tasks popped from the worker's own queue (via
		/// popCC). Otherwise it scans others, randomly moving on
		/// contention or execution, deciding to give up based on a
		/// checksum (via return codes frob pollAndExecCC). The maxTasks
		/// argument supports external usages; internal calls use zero,
		/// allowing unbounded steps (external calls trap non-positive
		/// values).
		/// </summary>
		/// <param name="w"> caller </param>
		/// <param name="maxTasks"> if non-zero, the maximum number of other tasks to run </param>
		/// <returns> task status on exit </returns>
		internal int helpComplete<T1>(WorkQueue w, CountedCompleter<T1> task, int maxTasks)
		{
			WorkQueue[] ws;
			int s = 0, m ;
			if ((ws = WorkQueues) != null && (m = ws.Length - 1) >= 0 && task != null && w != null)
			{
				int mode = w.Config; // for popCC
				int r = w.Hint ^ w.Top; // arbitrary seed for origin
				int origin = r & m; // first queue to scan
				int h = 1; // 1:ran, >1:contended, <0:hash
				for (int k = origin, oldSum = 0, checkSum = 0;;)
				{
//JAVA TO C# CONVERTER TODO TASK: Java wildcard generics are not converted to .NET:
//ORIGINAL LINE: CountedCompleter<?> p;
					CountedCompleter<?> p;
					WorkQueue q;
					if ((s = task.Status) < 0)
					{
						break;
					}
					if (h == 1 && (p = w.PopCC(task, mode)) != null)
					{
						p.DoExec(); // run local task
						if (maxTasks != 0 && --maxTasks == 0)
						{
							break;
						}
						origin = k; // reset
						oldSum = checkSum = 0;
					}
					else // poll other queues
					{
						if ((q = ws[k]) == null)
						{
							h = 0;
						}
						else if ((h = q.PollAndExecCC(task)) < 0)
						{
							checkSum += h;
						}
						if (h > 0)
						{
							if (h == 1 && maxTasks != 0 && --maxTasks == 0)
							{
								break;
							}
							r ^= r << 13; // xorshift
							r ^= (int)((uint)r >> 17);
							r ^= r << 5;
							origin = k = r & m; // move and restart
							oldSum = checkSum = 0;
						}
						else if ((k = (k + 1) & m) == origin)
						{
							if (oldSum == (oldSum = checkSum))
							{
								break;
							}
							checkSum = 0;
						}
					}
				}
			}
			return s;
		}

		/// <summary>
		/// Tries to locate and execute tasks for a stealer of the given
		/// task, or in turn one of its stealers, Traces currentSteal ->
		/// currentJoin links looking for a thread working on a descendant
		/// of the given task and with a non-empty queue to steal back and
		/// execute tasks from. The first call to this method upon a
		/// waiting join will often entail scanning/search, (which is OK
		/// because the joiner has nothing better to do), but this method
		/// leaves hints in workers to speed up subsequent calls.
		/// </summary>
		/// <param name="w"> caller </param>
		/// <param name="task"> the task to join </param>
		private void helpStealer<T1>(WorkQueue w, ForkJoinTask<T1> task)
		{
			WorkQueue[] ws = WorkQueues;
			int oldSum = 0, checkSum , m ;
			if (ws != null && (m = ws.Length - 1) >= 0 && w != null && task != null)
			{
				do // restart point
				{
					checkSum = 0; // for stability check
//JAVA TO C# CONVERTER TODO TASK: Java wildcard generics are not converted to .NET:
//ORIGINAL LINE: ForkJoinTask<?> subtask;
					ForkJoinTask<?> subtask;
					WorkQueue j = w, v ; // v is subtask stealer
					for (subtask = task; subtask.Status >= 0;)
					{
						for (int h = j.Hint | 1, k = 0, i; ; k += 2)
						{
							if (k > m) // can't find stealer
							{
								goto descentBreak;
							}
							if ((v = ws[i = (h + k) & m]) != null)
							{
								if (v.CurrentSteal == subtask)
								{
									j.Hint = i;
									break;
								}
								checkSum += v.@base;
							}
						}
						for (;;) // help v or descend
						{
//JAVA TO C# CONVERTER TODO TASK: Java wildcard generics are not converted to .NET:
//ORIGINAL LINE: ForkJoinTask<?>[] a;
							ForkJoinTask<?>[] a;
							int b;
							checkSum += (b = v.@base);
//JAVA TO C# CONVERTER TODO TASK: Java wildcard generics are not converted to .NET:
//ORIGINAL LINE: ForkJoinTask<?> next = v.currentJoin;
							ForkJoinTask<?> next = v.CurrentJoin;
							if (subtask.Status < 0 || j.CurrentJoin != subtask || v.CurrentSteal != subtask) // stale
							{
								goto descentBreak;
							}
							if (b - v.Top >= 0 || (a = v.Array) == null)
							{
								if ((subtask = next) == null)
								{
									goto descentBreak;
								}
								j = v;
								break;
							}
							int i = (((a.Length - 1) & b) << ASHIFT) + ABASE;
//JAVA TO C# CONVERTER TODO TASK: Java wildcard generics are not converted to .NET:
//ORIGINAL LINE: ForkJoinTask<?> t = ((ForkJoinTask<?>) U.getObjectVolatile(a, i));
							ForkJoinTask<?> t = ((ForkJoinTask<?>) U.getObjectVolatile(a, i));
							if (v.@base == b)
							{
								if (t == null) // stale
								{
									goto descentBreak;
								}
								if (U.compareAndSwapObject(a, i, t, null))
								{
									v.@base = b + 1;
//JAVA TO C# CONVERTER TODO TASK: Java wildcard generics are not converted to .NET:
//ORIGINAL LINE: ForkJoinTask<?> ps = w.currentSteal;
									ForkJoinTask<?> ps = w.CurrentSteal;
									int top = w.Top;
									do
									{
										U.putOrderedObject(w, QCURRENTSTEAL, t);
										t.DoExec(); // clear local tasks too
									} while (task.Status >= 0 && w.Top != top && (t = w.Pop()) != null);
									U.putOrderedObject(w, QCURRENTSTEAL, ps);
									if (w.@base != w.Top)
									{
										return; // can't further help
									}
								}
							}
						}
						descentContinue:;
					}
					descentBreak:;
				} while (task.Status >= 0 && oldSum != (oldSum = checkSum));
			}
		}

		/// <summary>
		/// Tries to decrement active count (sometimes implicitly) and
		/// possibly release or create a compensating worker in preparation
		/// for blocking. Returns false (retryable by caller), on
		/// contention, detected staleness, instability, or termination.
		/// </summary>
		/// <param name="w"> caller </param>
		private bool TryCompensate(WorkQueue w)
		{
			bool canBlock;
			WorkQueue[] ws;
			long c;
			int m, pc, sp;
			if (w == null || w.Qlock < 0 || (ws = WorkQueues) == null || (m = ws.Length - 1) <= 0 || (pc = Config & SMASK) == 0) // parallelism disabled -  caller terminating
			{
				canBlock = false;
			}
			else if ((sp = (int)(c = Ctl)) != 0) // release idle worker
			{
				canBlock = TryRelease(c, ws[sp & m], 0L);
			}
			else
			{
				int ac = (int)(c >> AC_SHIFT) + pc;
				int tc = (short)(c >> TC_SHIFT) + pc;
				int nbusy = 0; // validate saturation
				for (int i = 0; i <= m; ++i) // two passes of odd indices
				{
					WorkQueue v;
					if ((v = ws[((i << 1) | 1) & m]) != null)
					{
						if ((v.ScanState & SCANNING) != 0)
						{
							break;
						}
						++nbusy;
					}
				}
				if (nbusy != (tc << 1) || Ctl != c)
				{
					canBlock = false; // unstable or stale
				}
				else if (tc >= pc && ac > 1 && w.Empty)
				{
					long nc = ((AC_MASK & (c - AC_UNIT)) | (~AC_MASK & c)); // uncompensated
					canBlock = U.compareAndSwapLong(this, CTL, c, nc);
				}
				else if (tc >= MAX_CAP || (this == Common && tc >= pc + CommonMaxSpares))
				{
					throw new RejectedExecutionException("Thread limit exceeded replacing blocked worker");
				}
				else // similar to tryAddWorker
				{
					bool add = false; // CAS within lock
					int rs;
					long nc = ((AC_MASK & c) | (TC_MASK & (c + TC_UNIT)));
					if (((rs = LockRunState()) & STOP) == 0)
					{
						add = U.compareAndSwapLong(this, CTL, c, nc);
					}
					UnlockRunState(rs, rs & ~RSLOCK);
					canBlock = add && CreateWorker(); // throws on exception
				}
			}
			return canBlock;
		}

		/// <summary>
		/// Helps and/or blocks until the given task is done or timeout.
		/// </summary>
		/// <param name="w"> caller </param>
		/// <param name="task"> the task </param>
		/// <param name="deadline"> for timed waits, if nonzero </param>
		/// <returns> task status on exit </returns>
		internal int awaitJoin<T1>(WorkQueue w, ForkJoinTask<T1> task, long deadline)
		{
			int s = 0;
			if (task != null && w != null)
			{
//JAVA TO C# CONVERTER TODO TASK: Java wildcard generics are not converted to .NET:
//ORIGINAL LINE: ForkJoinTask<?> prevJoin = w.currentJoin;
				ForkJoinTask<?> prevJoin = w.CurrentJoin;
				U.putOrderedObject(w, QCURRENTJOIN, task);
//JAVA TO C# CONVERTER TODO TASK: Java wildcard generics are not converted to .NET:
//ORIGINAL LINE: CountedCompleter<?> cc = (task instanceof CountedCompleter) ? (CountedCompleter<?>)task : null;
				CountedCompleter<?> cc = (task is CountedCompleter) ? (CountedCompleter<?>)task : null;
				for (;;)
				{
					if ((s = task.Status) < 0)
					{
						break;
					}
					if (cc != null)
					{
						HelpComplete(w, cc, 0);
					}
					else if (w.@base == w.Top || w.TryRemoveAndExec(task))
					{
						HelpStealer(w, task);
					}
					if ((s = task.Status) < 0)
					{
						break;
					}
					long ms, ns;
					if (deadline == 0L)
					{
						ms = 0L;
					}
					else if ((ns = deadline - System.nanoTime()) <= 0L)
					{
						break;
					}
					else if ((ms = TimeUnit.NANOSECONDS.ToMillis(ns)) <= 0L)
					{
						ms = 1L;
					}
					if (TryCompensate(w))
					{
						task.InternalWait(ms);
						U.getAndAddLong(this, CTL, AC_UNIT);
					}
				}
				U.putOrderedObject(w, QCURRENTJOIN, prevJoin);
			}
			return s;
		}

		// Specialized scanning

		/// <summary>
		/// Returns a (probably) non-empty steal queue, if one is found
		/// during a scan, else null.  This method must be retried by
		/// caller if, by the time it tries to use the queue, it is empty.
		/// </summary>
		private WorkQueue FindNonEmptyStealQueue()
		{
			WorkQueue[] ws; // one-shot version of scan loop
			int m;
			int r = ThreadLocalRandom.NextSecondarySeed();
			if ((ws = WorkQueues) != null && (m = ws.Length - 1) >= 0)
			{
				for (int origin = r & m, k = origin, oldSum = 0, checkSum = 0;;)
				{
					WorkQueue q;
					int b;
					if ((q = ws[k]) != null)
					{
						if ((b = q.@base) - q.Top < 0)
						{
							return q;
						}
						checkSum += b;
					}
					if ((k = (k + 1) & m) == origin)
					{
						if (oldSum == (oldSum = checkSum))
						{
							break;
						}
						checkSum = 0;
					}
				}
			}
			return null;
		}

		/// <summary>
		/// Runs tasks until {@code isQuiescent()}. We piggyback on
		/// active count ctl maintenance, but rather than blocking
		/// when tasks cannot be found, we rescan until all others cannot
		/// find tasks either.
		/// </summary>
		internal void HelpQuiescePool(WorkQueue w)
		{
//JAVA TO C# CONVERTER TODO TASK: Java wildcard generics are not converted to .NET:
//ORIGINAL LINE: ForkJoinTask<?> ps = w.currentSteal;
			ForkJoinTask<?> ps = w.CurrentSteal; // save context
			for (bool active = true;;)
			{
				long c;
				WorkQueue q;
//JAVA TO C# CONVERTER TODO TASK: Java wildcard generics are not converted to .NET:
//ORIGINAL LINE: ForkJoinTask<?> t;
				ForkJoinTask<?> t;
				int b;
				w.ExecLocalTasks(); // run locals before each scan
				if ((q = FindNonEmptyStealQueue()) != null)
				{
					if (!active) // re-establish active count
					{
						active = true;
						U.getAndAddLong(this, CTL, AC_UNIT);
					}
					if ((b = q.@base) - q.Top < 0 && (t = q.PollAt(b)) != null)
					{
						U.putOrderedObject(w, QCURRENTSTEAL, t);
						t.DoExec();
						if (++w.Nsteals < 0)
						{
							w.TransferStealCount(this);
						}
					}
				}
				else if (active) // decrement active count without queuing
				{
					long nc = (AC_MASK & ((c = Ctl) - AC_UNIT)) | (~AC_MASK & c);
					if ((int)(nc >> AC_SHIFT) + (Config & SMASK) <= 0)
					{
						break; // bypass decrement-then-increment
					}
					if (U.compareAndSwapLong(this, CTL, c, nc))
					{
						active = false;
					}
				}
				else if ((int)((c = Ctl) >> AC_SHIFT) + (Config & SMASK) <= 0 && U.compareAndSwapLong(this, CTL, c, c + AC_UNIT))
				{
					break;
				}
			}
			U.putOrderedObject(w, QCURRENTSTEAL, ps);
		}

		/// <summary>
		/// Gets and removes a local or stolen task for the given worker.
		/// </summary>
		/// <returns> a task, if available </returns>
//JAVA TO C# CONVERTER TODO TASK: Java wildcard generics are not converted to .NET:
//ORIGINAL LINE: final ForkJoinTask<?> nextTaskFor(WorkQueue w)
		internal ForkJoinTask<?> NextTaskFor(WorkQueue w)
		{
//JAVA TO C# CONVERTER TODO TASK: Java wildcard generics are not converted to .NET:
//ORIGINAL LINE: for (ForkJoinTask<?> t;;)
			for (ForkJoinTask<?> t;;)
			{
				WorkQueue q;
				int b;
				if ((t = w.NextLocalTask()) != null)
				{
					return t;
				}
				if ((q = FindNonEmptyStealQueue()) == null)
				{
					return null;
				}
				if ((b = q.@base) - q.Top < 0 && (t = q.PollAt(b)) != null)
				{
					return t;
				}
			}
		}

		/// <summary>
		/// Returns a cheap heuristic guide for task partitioning when
		/// programmers, frameworks, tools, or languages have little or no
		/// idea about task granularity.  In essence, by offering this
		/// method, we ask users only about tradeoffs in overhead vs
		/// expected throughput and its variance, rather than how finely to
		/// partition tasks.
		/// 
		/// In a steady state strict (tree-structured) computation, each
		/// thread makes available for stealing enough tasks for other
		/// threads to remain active. Inductively, if all threads play by
		/// the same rules, each thread should make available only a
		/// constant number of tasks.
		/// 
		/// The minimum useful constant is just 1. But using a value of 1
		/// would require immediate replenishment upon each steal to
		/// maintain enough tasks, which is infeasible.  Further,
		/// partitionings/granularities of offered tasks should minimize
		/// steal rates, which in general means that threads nearer the top
		/// of computation tree should generate more than those nearer the
		/// bottom. In perfect steady state, each thread is at
		/// approximately the same level of computation tree. However,
		/// producing extra tasks amortizes the uncertainty of progress and
		/// diffusion assumptions.
		/// 
		/// So, users will want to use values larger (but not much larger)
		/// than 1 to both smooth over transient shortages and hedge
		/// against uneven progress; as traded off against the cost of
		/// extra task overhead. We leave the user to pick a threshold
		/// value to compare with the results of this call to guide
		/// decisions, but recommend values such as 3.
		/// 
		/// When all threads are active, it is on average OK to estimate
		/// surplus strictly locally. In steady-state, if one thread is
		/// maintaining say 2 surplus tasks, then so are others. So we can
		/// just use estimated queue length.  However, this strategy alone
		/// leads to serious mis-estimates in some non-steady-state
		/// conditions (ramp-up, ramp-down, other stalls). We can detect
		/// many of these by further considering the number of "idle"
		/// threads, that are known to have zero queued tasks, so
		/// compensate by a factor of (#idle/#active) threads.
		/// </summary>
		internal static int SurplusQueuedTaskCount
		{
			get
			{
				Thread t;
				ForkJoinWorkerThread wt;
				ForkJoinPool pool;
				WorkQueue q;
				if (((t = Thread.CurrentThread) is ForkJoinWorkerThread))
				{
					int p = (pool = (wt = (ForkJoinWorkerThread)t).pool).config & SMASK;
					int n = (q = wt.WorkQueue).top - q.@base;
					int a = (int)(pool.Ctl >> AC_SHIFT) + p;
					return n - (a > (p = (int)((uint)p >> 1)) ? 0 : a > (p >> >= 1) ? 1 : a > (p >> >= 1) ? 2 : a > (p >> >= 1) ? 4 : 8);
				}
				return 0;
			}
		}

		//  Termination

		/// <summary>
		/// Possibly initiates and/or completes termination.
		/// </summary>
		/// <param name="now"> if true, unconditionally terminate, else only
		/// if no work and no active workers </param>
		/// <param name="enable"> if true, enable shutdown when next possible </param>
		/// <returns> true if now terminating or terminated </returns>
		private bool TryTerminate(bool now, bool enable)
		{
			int rs;
			if (this == Common) // cannot shut down
			{
				return false;
			}
			if ((rs = RunState) >= 0)
			{
				if (!enable)
				{
					return false;
				}
				rs = LockRunState(); // enter SHUTDOWN phase
				UnlockRunState(rs, (rs & ~RSLOCK) | SHUTDOWN);
			}

			if ((rs & STOP) == 0)
			{
				if (!now) // check quiescence
				{
					for (long oldSum = 0L;;) // repeat until stable
					{
						WorkQueue[] ws;
						WorkQueue w;
						int m, b;
						long c;
						long checkSum = Ctl;
						if ((int)(checkSum >> AC_SHIFT) + (Config & SMASK) > 0)
						{
							return false; // still active workers
						}
						if ((ws = WorkQueues) == null || (m = ws.Length - 1) <= 0)
						{
							break; // check queues
						}
						for (int i = 0; i <= m; ++i)
						{
							if ((w = ws[i]) != null)
							{
								if ((b = w.@base) != w.Top || w.ScanState >= 0 || w.CurrentSteal != null)
								{
									TryRelease(c = Ctl, ws[m & (int)c], AC_UNIT);
									return false; // arrange for recheck
								}
								checkSum += b;
								if ((i & 1) == 0)
								{
									w.Qlock = -1; // try to disable external
								}
							}
						}
						if (oldSum == (oldSum = checkSum))
						{
							break;
						}
					}
				}
				if ((RunState & STOP) == 0)
				{
					rs = LockRunState(); // enter STOP phase
					UnlockRunState(rs, (rs & ~RSLOCK) | STOP);
				}
			}

			int pass = 0; // 3 passes to help terminate
			for (long oldSum = 0L;;) // or until done or stable
			{
				WorkQueue[] ws;
				WorkQueue w;
				ForkJoinWorkerThread wt;
				int m;
				long checkSum = Ctl;
				if ((short)((long)((ulong)checkSum >> TC_SHIFT)) + (Config & SMASK) <= 0 || (ws = WorkQueues) == null || (m = ws.Length - 1) <= 0)
				{
					if ((RunState & TERMINATED) == 0)
					{
						rs = LockRunState(); // done
						UnlockRunState(rs, (rs & ~RSLOCK) | TERMINATED);
						lock (this) // for awaitTermination
						{
							Monitor.PulseAll(this);
						}
					}
					break;
				}
				for (int i = 0; i <= m; ++i)
				{
					if ((w = ws[i]) != null)
					{
						checkSum += w.@base;
						w.Qlock = -1; // try to disable
						if (pass > 0)
						{
							w.CancelAll(); // clear queue
							if (pass > 1 && (wt = w.Owner) != null)
							{
								if (!wt.Interrupted)
								{
									try // unblock join
									{
										wt.Interrupt();
									}
									catch (Throwable)
									{
									}
								}
								if (w.ScanState < 0)
								{
									U.unpark(wt); // wake up
								}
							}
						}
					}
				}
				if (checkSum != oldSum) // unstable
				{
					oldSum = checkSum;
					pass = 0;
				}
				else if (pass > 3 && pass > m) // can't further help
				{
					break;
				}
				else if (++pass > 1) // try to dequeue
				{
					long c; // bound attempts
					int j = 0, sp ;
					while (j++ <= m && (sp = (int)(c = Ctl)) != 0)
					{
						TryRelease(c, ws[sp & m], AC_UNIT);
					}
				}
			}
			return true;
		}

		// External operations

		/// <summary>
		/// Full version of externalPush, handling uncommon cases, as well
		/// as performing secondary initialization upon the first
		/// submission of the first task to the pool.  It also detects
		/// first submission by an external thread and creates a new shared
		/// queue if the one at index if empty or contended.
		/// </summary>
		/// <param name="task"> the task. Caller must ensure non-null. </param>
		private void externalSubmit<T1>(ForkJoinTask<T1> task)
		{
			int r; // initialize caller's probe
			if ((r = ThreadLocalRandom.Probe) == 0)
			{
				ThreadLocalRandom.LocalInit();
				r = ThreadLocalRandom.Probe;
			}
			for (;;)
			{
				WorkQueue[] ws;
				WorkQueue q;
				int rs, m, k;
				bool move = false;
				if ((rs = RunState) < 0)
				{
					TryTerminate(false, false); // help terminate
					throw new RejectedExecutionException();
				}
				else if ((rs & STARTED) == 0 || ((ws = WorkQueues) == null || (m = ws.Length - 1) < 0)) // initialize
				{
					int ns = 0;
					rs = LockRunState();
					try
					{
						if ((rs & STARTED) == 0)
						{
							U.compareAndSwapObject(this, STEALCOUNTER, null, new AtomicLong());
							// create workQueues array with size a power of two
							int p = Config & SMASK; // ensure at least 2 slots
							int n = (p > 1) ? p - 1 : 1;
							n |= (int)((uint)n >> 1);
							n |= (int)((uint)n >> 2);
							n |= (int)((uint)n >> 4);
							n |= (int)((uint)n >> 8);
							n |= (int)((uint)n >> 16);
							n = (n + 1) << 1;
							WorkQueues = new WorkQueue[n];
							ns = STARTED;
						}
					}
					finally
					{
						UnlockRunState(rs, (rs & ~RSLOCK) | ns);
					}
				}
				else if ((q = ws[k = r & m & SQMASK]) != null)
				{
					if (q.Qlock == 0 && U.compareAndSwapInt(q, QLOCK, 0, 1))
					{
//JAVA TO C# CONVERTER TODO TASK: Java wildcard generics are not converted to .NET:
//ORIGINAL LINE: ForkJoinTask<?>[] a = q.array;
						ForkJoinTask<?>[] a = q.Array;
						int s = q.Top;
						bool submitted = false; // initial submission or resizing
						try // locked version of push
						{
							if ((a != null && a.Length > s + 1 - q.@base) || (a = q.GrowArray()) != null)
							{
								int j = (((a.Length - 1) & s) << ASHIFT) + ABASE;
								U.putOrderedObject(a, j, task);
								U.putOrderedInt(q, QTOP, s + 1);
								submitted = true;
							}
						}
						finally
						{
							U.compareAndSwapInt(q, QLOCK, 1, 0);
						}
						if (submitted)
						{
							SignalWork(ws, q);
							return;
						}
					}
					move = true; // move on failure
				}
				else if (((rs = RunState) & RSLOCK) == 0) // create new queue
				{
					q = new WorkQueue(this, null);
					q.Hint = r;
					q.Config = k | SHARED_QUEUE;
					q.ScanState = INACTIVE;
					rs = LockRunState(); // publish index
					if (rs > 0 && (ws = WorkQueues) != null && k < ws.Length && ws[k] == null)
					{
						ws[k] = q; // else terminated
					}
					UnlockRunState(rs, rs & ~RSLOCK);
				}
				else
				{
					move = true; // move if busy
				}
				if (move)
				{
					r = ThreadLocalRandom.AdvanceProbe(r);
				}
			}
		}

		/// <summary>
		/// Tries to add the given task to a submission queue at
		/// submitter's current queue. Only the (vastly) most common path
		/// is directly handled in this method, while screening for need
		/// for externalSubmit.
		/// </summary>
		/// <param name="task"> the task. Caller must ensure non-null. </param>
		internal void externalPush<T1>(ForkJoinTask<T1> task)
		{
			WorkQueue[] ws;
			WorkQueue q;
			int m;
			int r = ThreadLocalRandom.Probe;
			int rs = RunState;
			if ((ws = WorkQueues) != null && (m = (ws.Length - 1)) >= 0 && (q = ws[m & r & SQMASK]) != null && r != 0 && rs > 0 && U.compareAndSwapInt(q, QLOCK, 0, 1))
			{
//JAVA TO C# CONVERTER TODO TASK: Java wildcard generics are not converted to .NET:
//ORIGINAL LINE: ForkJoinTask<?>[] a;
				ForkJoinTask<?>[] a;
				int am, n, s;
				if ((a = q.Array) != null && (am = a.Length - 1) > (n = (s = q.Top) - q.@base))
				{
					int j = ((am & s) << ASHIFT) + ABASE;
					U.putOrderedObject(a, j, task);
					U.putOrderedInt(q, QTOP, s + 1);
					U.putIntVolatile(q, QLOCK, 0);
					if (n <= 1)
					{
						SignalWork(ws, q);
					}
					return;
				}
				U.compareAndSwapInt(q, QLOCK, 1, 0);
			}
			ExternalSubmit(task);
		}

		/// <summary>
		/// Returns common pool queue for an external thread.
		/// </summary>
		internal static WorkQueue CommonSubmitterQueue()
		{
			ForkJoinPool p = Common;
			int r = ThreadLocalRandom.Probe;
			WorkQueue[] ws;
			int m;
			return (p != null && (ws = p.WorkQueues) != null && (m = ws.Length - 1) >= 0) ? ws[m & r & SQMASK] : null;
		}

		/// <summary>
		/// Performs tryUnpush for an external submitter: Finds queue,
		/// locks if apparently non-empty, validates upon locking, and
		/// adjusts top. Each check can fail but rarely does.
		/// </summary>
		internal bool tryExternalUnpush<T1>(ForkJoinTask<T1> task)
		{
			WorkQueue[] ws;
			WorkQueue w;
//JAVA TO C# CONVERTER TODO TASK: Java wildcard generics are not converted to .NET:
//ORIGINAL LINE: ForkJoinTask<?>[] a;
			ForkJoinTask<?>[] a;
			int m, s;
			int r = ThreadLocalRandom.Probe;
			if ((ws = WorkQueues) != null && (m = ws.Length - 1) >= 0 && (w = ws[m & r & SQMASK]) != null && (a = w.Array) != null && (s = w.Top) != w.@base)
			{
				long j = (((a.Length - 1) & (s - 1)) << ASHIFT) + ABASE;
				if (U.compareAndSwapInt(w, QLOCK, 0, 1))
				{
					if (w.Top == s && w.Array == a && U.getObject(a, j) == task && U.compareAndSwapObject(a, j, task, null))
					{
						U.putOrderedInt(w, QTOP, s - 1);
						U.putOrderedInt(w, QLOCK, 0);
						return true;
					}
					U.compareAndSwapInt(w, QLOCK, 1, 0);
				}
			}
			return false;
		}

		/// <summary>
		/// Performs helpComplete for an external submitter.
		/// </summary>
		internal int externalHelpComplete<T1>(CountedCompleter<T1> task, int maxTasks)
		{
			WorkQueue[] ws;
			int n;
			int r = ThreadLocalRandom.Probe;
			return ((ws = WorkQueues) == null || (n = ws.Length) == 0) ? 0 : HelpComplete(ws[(n - 1) & r & SQMASK], task, maxTasks);
		}

		// Exported methods

		// Constructors

		/// <summary>
		/// Creates a {@code ForkJoinPool} with parallelism equal to {@link
		/// java.lang.Runtime#availableProcessors}, using the {@linkplain
		/// #defaultForkJoinWorkerThreadFactory default thread factory},
		/// no UncaughtExceptionHandler, and non-async LIFO processing mode.
		/// </summary>
		/// <exception cref="SecurityException"> if a security manager exists and
		///         the caller is not permitted to modify threads
		///         because it does not hold {@link
		///         java.lang.RuntimePermission}{@code ("modifyThread")} </exception>
		public ForkJoinPool() : this(System.Math.Min(MAX_CAP, Runtime.Runtime.availableProcessors()), DefaultForkJoinWorkerThreadFactory, null, false)
		{
		}

		/// <summary>
		/// Creates a {@code ForkJoinPool} with the indicated parallelism
		/// level, the {@linkplain
		/// #defaultForkJoinWorkerThreadFactory default thread factory},
		/// no UncaughtExceptionHandler, and non-async LIFO processing mode.
		/// </summary>
		/// <param name="parallelism"> the parallelism level </param>
		/// <exception cref="IllegalArgumentException"> if parallelism less than or
		///         equal to zero, or greater than implementation limit </exception>
		/// <exception cref="SecurityException"> if a security manager exists and
		///         the caller is not permitted to modify threads
		///         because it does not hold {@link
		///         java.lang.RuntimePermission}{@code ("modifyThread")} </exception>
		public ForkJoinPool(int parallelism) : this(parallelism, DefaultForkJoinWorkerThreadFactory, null, false)
		{
		}

		/// <summary>
		/// Creates a {@code ForkJoinPool} with the given parameters.
		/// </summary>
		/// <param name="parallelism"> the parallelism level. For default value,
		/// use <seealso cref="java.lang.Runtime#availableProcessors"/>. </param>
		/// <param name="factory"> the factory for creating new threads. For default value,
		/// use <seealso cref="#defaultForkJoinWorkerThreadFactory"/>. </param>
		/// <param name="handler"> the handler for internal worker threads that
		/// terminate due to unrecoverable errors encountered while executing
		/// tasks. For default value, use {@code null}. </param>
		/// <param name="asyncMode"> if true,
		/// establishes local first-in-first-out scheduling mode for forked
		/// tasks that are never joined. This mode may be more appropriate
		/// than default locally stack-based mode in applications in which
		/// worker threads only process event-style asynchronous tasks.
		/// For default value, use {@code false}. </param>
		/// <exception cref="IllegalArgumentException"> if parallelism less than or
		///         equal to zero, or greater than implementation limit </exception>
		/// <exception cref="NullPointerException"> if the factory is null </exception>
		/// <exception cref="SecurityException"> if a security manager exists and
		///         the caller is not permitted to modify threads
		///         because it does not hold {@link
		///         java.lang.RuntimePermission}{@code ("modifyThread")} </exception>
		public ForkJoinPool(int parallelism, ForkJoinWorkerThreadFactory factory, UncaughtExceptionHandler handler, bool asyncMode) : this(CheckParallelism(parallelism), CheckFactory(factory), handler, asyncMode ? FIFO_QUEUE : LIFO_QUEUE, "ForkJoinPool-" + NextPoolId() + "-worker-")
		{
			CheckPermission();
		}

		private static int CheckParallelism(int parallelism)
		{
			if (parallelism <= 0 || parallelism > MAX_CAP)
			{
				throw new IllegalArgumentException();
			}
			return parallelism;
		}

		private static ForkJoinWorkerThreadFactory CheckFactory(ForkJoinWorkerThreadFactory factory)
		{
			if (factory == null)
			{
				throw new NullPointerException();
			}
			return factory;
		}

		/// <summary>
		/// Creates a {@code ForkJoinPool} with the given parameters, without
		/// any security checks or parameter validation.  Invoked directly by
		/// makeCommonPool.
		/// </summary>
		private ForkJoinPool(int parallelism, ForkJoinWorkerThreadFactory factory, UncaughtExceptionHandler handler, int mode, String workerNamePrefix)
		{
			this.WorkerNamePrefix = workerNamePrefix;
			this.Factory_Renamed = factory;
			this.Ueh = handler;
			this.Config = (parallelism & SMASK) | mode;
			long np = (long)(-parallelism); // offset ctl counts
			this.Ctl = ((np << AC_SHIFT) & AC_MASK) | ((np << TC_SHIFT) & TC_MASK);
		}

		/// <summary>
		/// Returns the common pool instance. This pool is statically
		/// constructed; its run state is unaffected by attempts to {@link
		/// #shutdown} or <seealso cref="#shutdownNow"/>. However this pool and any
		/// ongoing processing are automatically terminated upon program
		/// <seealso cref="System#exit"/>.  Any program that relies on asynchronous
		/// task processing to complete before program termination should
		/// invoke {@code commonPool().}<seealso cref="#awaitQuiescence awaitQuiescence"/>,
		/// before exit.
		/// </summary>
		/// <returns> the common pool instance
		/// @since 1.8 </returns>
		public static ForkJoinPool CommonPool()
		{
			// assert common != null : "static init error";
			return Common;
		}

		// Execution methods

		/// <summary>
		/// Performs the given task, returning its result upon completion.
		/// If the computation encounters an unchecked Exception or Error,
		/// it is rethrown as the outcome of this invocation.  Rethrown
		/// exceptions behave in the same way as regular exceptions, but,
		/// when possible, contain stack traces (as displayed for example
		/// using {@code ex.printStackTrace()}) of both the current thread
		/// as well as the thread actually encountering the exception;
		/// minimally only the latter.
		/// </summary>
		/// <param name="task"> the task </param>
		/// @param <T> the type of the task's result </param>
		/// <returns> the task's result </returns>
		/// <exception cref="NullPointerException"> if the task is null </exception>
		/// <exception cref="RejectedExecutionException"> if the task cannot be
		///         scheduled for execution </exception>
		public virtual T invoke<T>(ForkJoinTask<T> task)
		{
			if (task == null)
			{
				throw new NullPointerException();
			}
			ExternalPush(task);
			return task.Join();
		}

		/// <summary>
		/// Arranges for (asynchronous) execution of the given task.
		/// </summary>
		/// <param name="task"> the task </param>
		/// <exception cref="NullPointerException"> if the task is null </exception>
		/// <exception cref="RejectedExecutionException"> if the task cannot be
		///         scheduled for execution </exception>
		public virtual void execute<T1>(ForkJoinTask<T1> task)
		{
			if (task == null)
			{
				throw new NullPointerException();
			}
			ExternalPush(task);
		}

		// AbstractExecutorService methods

		/// <exception cref="NullPointerException"> if the task is null </exception>
		/// <exception cref="RejectedExecutionException"> if the task cannot be
		///         scheduled for execution </exception>
		public override void Execute(Runnable task)
		{
			if (task == null)
			{
				throw new NullPointerException();
			}
//JAVA TO C# CONVERTER TODO TASK: Java wildcard generics are not converted to .NET:
//ORIGINAL LINE: ForkJoinTask<?> job;
			ForkJoinTask<?> job;
//JAVA TO C# CONVERTER TODO TASK: Java wildcard generics are not converted to .NET:
//ORIGINAL LINE: if (task instanceof ForkJoinTask<?>)
			if (task is ForkJoinTask<?>) // avoid re-wrap
			{
//JAVA TO C# CONVERTER TODO TASK: Java wildcard generics are not converted to .NET:
//ORIGINAL LINE: job = (ForkJoinTask<?>) task;
				job = (ForkJoinTask<?>) task;
			}
			else
			{
				job = new ForkJoinTask.RunnableExecuteAction(task);
			}
			ExternalPush(job);
		}

		/// <summary>
		/// Submits a ForkJoinTask for execution.
		/// </summary>
		/// <param name="task"> the task to submit </param>
		/// @param <T> the type of the task's result </param>
		/// <returns> the task </returns>
		/// <exception cref="NullPointerException"> if the task is null </exception>
		/// <exception cref="RejectedExecutionException"> if the task cannot be
		///         scheduled for execution </exception>
		public virtual ForkJoinTask<T> submit<T>(ForkJoinTask<T> task)
		{
			if (task == null)
			{
				throw new NullPointerException();
			}
			ExternalPush(task);
			return task;
		}

		/// <exception cref="NullPointerException"> if the task is null </exception>
		/// <exception cref="RejectedExecutionException"> if the task cannot be
		///         scheduled for execution </exception>
		public override ForkJoinTask<T> submit<T>(Callable<T> task)
		{
			ForkJoinTask<T> job = new ForkJoinTask.AdaptedCallable<T>(task);
			ExternalPush(job);
			return job;
		}

		/// <exception cref="NullPointerException"> if the task is null </exception>
		/// <exception cref="RejectedExecutionException"> if the task cannot be
		///         scheduled for execution </exception>
		public override ForkJoinTask<T> submit<T>(Runnable task, T result)
		{
			ForkJoinTask<T> job = new ForkJoinTask.AdaptedRunnable<T>(task, result);
			ExternalPush(job);
			return job;
		}

		/// <exception cref="NullPointerException"> if the task is null </exception>
		/// <exception cref="RejectedExecutionException"> if the task cannot be
		///         scheduled for execution </exception>
//JAVA TO C# CONVERTER TODO TASK: Java wildcard generics are not converted to .NET:
//ORIGINAL LINE: public ForkJoinTask<?> submit(Runnable task)
		public override ForkJoinTask<?> Submit(Runnable task)
		{
			if (task == null)
			{
				throw new NullPointerException();
			}
//JAVA TO C# CONVERTER TODO TASK: Java wildcard generics are not converted to .NET:
//ORIGINAL LINE: ForkJoinTask<?> job;
			ForkJoinTask<?> job;
//JAVA TO C# CONVERTER TODO TASK: Java wildcard generics are not converted to .NET:
//ORIGINAL LINE: if (task instanceof ForkJoinTask<?>)
			if (task is ForkJoinTask<?>) // avoid re-wrap
			{
//JAVA TO C# CONVERTER TODO TASK: Java wildcard generics are not converted to .NET:
//ORIGINAL LINE: job = (ForkJoinTask<?>) task;
				job = (ForkJoinTask<?>) task;
			}
			else
			{
				job = new ForkJoinTask.AdaptedRunnableAction(task);
			}
			ExternalPush(job);
			return job;
		}

		/// <exception cref="NullPointerException">       {@inheritDoc} </exception>
		/// <exception cref="RejectedExecutionException"> {@inheritDoc} </exception>
		public override IList<Future<T>> invokeAll<T, T1>(ICollection<T1> tasks) where T1 : java.util.concurrent.Callable<T>
		{
			// In previous versions of this class, this method constructed
			// a task to run ForkJoinTask.invokeAll, but now external
			// invocation of multiple tasks is at least as efficient.
			List<Future<T>> futures = new List<Future<T>>(tasks.Count);

			bool done = false;
			try
			{
				foreach (Callable<T> t in tasks)
				{
					ForkJoinTask<T> f = new ForkJoinTask.AdaptedCallable<T>(t);
					futures.Add(f);
					ExternalPush(f);
				}
				for (int i = 0, size = futures.Size(); i < size; i++)
				{
//JAVA TO C# CONVERTER TODO TASK: Java wildcard generics are not converted to .NET:
//ORIGINAL LINE: ((ForkJoinTask<?>)futures.get(i)).quietlyJoin();
					((ForkJoinTask<?>)futures.Get(i)).QuietlyJoin();
				}
				done = true;
				return futures;
			}
			finally
			{
				if (!done)
				{
					for (int i = 0, size = futures.Size(); i < size; i++)
					{
						futures.Get(i).Cancel(false);
					}
				}
			}
		}

		/// <summary>
		/// Returns the factory used for constructing new workers.
		/// </summary>
		/// <returns> the factory used for constructing new workers </returns>
		public virtual ForkJoinWorkerThreadFactory Factory
		{
			get
			{
				return Factory_Renamed;
			}
		}

		/// <summary>
		/// Returns the handler for internal worker threads that terminate
		/// due to unrecoverable errors encountered while executing tasks.
		/// </summary>
		/// <returns> the handler, or {@code null} if none </returns>
		public virtual UncaughtExceptionHandler UncaughtExceptionHandler
		{
			get
			{
				return Ueh;
			}
		}

		/// <summary>
		/// Returns the targeted parallelism level of this pool.
		/// </summary>
		/// <returns> the targeted parallelism level of this pool </returns>
		public virtual int Parallelism
		{
			get
			{
				int par;
				return ((par = Config & SMASK) > 0) ? par : 1;
			}
		}

		/// <summary>
		/// Returns the targeted parallelism level of the common pool.
		/// </summary>
		/// <returns> the targeted parallelism level of the common pool
		/// @since 1.8 </returns>
		public static int CommonPoolParallelism
		{
			get
			{
				return CommonParallelism;
			}
		}

		/// <summary>
		/// Returns the number of worker threads that have started but not
		/// yet terminated.  The result returned by this method may differ
		/// from <seealso cref="#getParallelism"/> when threads are created to
		/// maintain parallelism when others are cooperatively blocked.
		/// </summary>
		/// <returns> the number of worker threads </returns>
		public virtual int PoolSize
		{
			get
			{
				return (Config & SMASK) + (short)((long)((ulong)Ctl >> TC_SHIFT));
			}
		}

		/// <summary>
		/// Returns {@code true} if this pool uses local first-in-first-out
		/// scheduling mode for forked tasks that are never joined.
		/// </summary>
		/// <returns> {@code true} if this pool uses async mode </returns>
		public virtual bool AsyncMode
		{
			get
			{
				return (Config & FIFO_QUEUE) != 0;
			}
		}

		/// <summary>
		/// Returns an estimate of the number of worker threads that are
		/// not blocked waiting to join tasks or for other managed
		/// synchronization. This method may overestimate the
		/// number of running threads.
		/// </summary>
		/// <returns> the number of worker threads </returns>
		public virtual int RunningThreadCount
		{
			get
			{
				int rc = 0;
				WorkQueue[] ws;
				WorkQueue w;
				if ((ws = WorkQueues) != null)
				{
					for (int i = 1; i < ws.Length; i += 2)
					{
						if ((w = ws[i]) != null && w.ApparentlyUnblocked)
						{
							++rc;
						}
					}
				}
				return rc;
			}
		}

		/// <summary>
		/// Returns an estimate of the number of threads that are currently
		/// stealing or executing tasks. This method may overestimate the
		/// number of active threads.
		/// </summary>
		/// <returns> the number of active threads </returns>
		public virtual int ActiveThreadCount
		{
			get
			{
				int r = (Config & SMASK) + (int)(Ctl >> AC_SHIFT);
				return (r <= 0) ? 0 : r; // suppress momentarily negative values
			}
		}

		/// <summary>
		/// Returns {@code true} if all worker threads are currently idle.
		/// An idle worker is one that cannot obtain a task to execute
		/// because none are available to steal from other threads, and
		/// there are no pending submissions to the pool. This method is
		/// conservative; it might not return {@code true} immediately upon
		/// idleness of all threads, but will eventually become true if
		/// threads remain inactive.
		/// </summary>
		/// <returns> {@code true} if all threads are currently idle </returns>
		public virtual bool Quiescent
		{
			get
			{
				return (Config & SMASK) + (int)(Ctl >> AC_SHIFT) <= 0;
			}
		}

		/// <summary>
		/// Returns an estimate of the total number of tasks stolen from
		/// one thread's work queue by another. The reported value
		/// underestimates the actual total number of steals when the pool
		/// is not quiescent. This value may be useful for monitoring and
		/// tuning fork/join programs: in general, steal counts should be
		/// high enough to keep threads busy, but low enough to avoid
		/// overhead and contention across threads.
		/// </summary>
		/// <returns> the number of steals </returns>
		public virtual long StealCount
		{
			get
			{
				AtomicLong sc = StealCounter;
				long count = (sc == null) ? 0L : sc.Get();
				WorkQueue[] ws;
				WorkQueue w;
				if ((ws = WorkQueues) != null)
				{
					for (int i = 1; i < ws.Length; i += 2)
					{
						if ((w = ws[i]) != null)
						{
							count += w.Nsteals;
						}
					}
				}
				return count;
			}
		}

		/// <summary>
		/// Returns an estimate of the total number of tasks currently held
		/// in queues by worker threads (but not including tasks submitted
		/// to the pool that have not begun executing). This value is only
		/// an approximation, obtained by iterating across all threads in
		/// the pool. This method may be useful for tuning task
		/// granularities.
		/// </summary>
		/// <returns> the number of queued tasks </returns>
		public virtual long QueuedTaskCount
		{
			get
			{
				long count = 0;
				WorkQueue[] ws;
				WorkQueue w;
				if ((ws = WorkQueues) != null)
				{
					for (int i = 1; i < ws.Length; i += 2)
					{
						if ((w = ws[i]) != null)
						{
							count += w.QueueSize();
						}
					}
				}
				return count;
			}
		}

		/// <summary>
		/// Returns an estimate of the number of tasks submitted to this
		/// pool that have not yet begun executing.  This method may take
		/// time proportional to the number of submissions.
		/// </summary>
		/// <returns> the number of queued submissions </returns>
		public virtual int QueuedSubmissionCount
		{
			get
			{
				int count = 0;
				WorkQueue[] ws;
				WorkQueue w;
				if ((ws = WorkQueues) != null)
				{
					for (int i = 0; i < ws.Length; i += 2)
					{
						if ((w = ws[i]) != null)
						{
							count += w.QueueSize();
						}
					}
				}
				return count;
			}
		}

		/// <summary>
		/// Returns {@code true} if there are any tasks submitted to this
		/// pool that have not yet begun executing.
		/// </summary>
		/// <returns> {@code true} if there are any queued submissions </returns>
		public virtual bool HasQueuedSubmissions()
		{
			WorkQueue[] ws;
			WorkQueue w;
			if ((ws = WorkQueues) != null)
			{
				for (int i = 0; i < ws.Length; i += 2)
				{
					if ((w = ws[i]) != null && !w.Empty)
					{
						return true;
					}
				}
			}
			return false;
		}

		/// <summary>
		/// Removes and returns the next unexecuted submission if one is
		/// available.  This method may be useful in extensions to this
		/// class that re-assign work in systems with multiple pools.
		/// </summary>
		/// <returns> the next submission, or {@code null} if none </returns>
//JAVA TO C# CONVERTER TODO TASK: Java wildcard generics are not converted to .NET:
//ORIGINAL LINE: protected ForkJoinTask<?> pollSubmission()
		protected internal virtual ForkJoinTask<?> PollSubmission()
		{
			WorkQueue[] ws;
			WorkQueue w;
//JAVA TO C# CONVERTER TODO TASK: Java wildcard generics are not converted to .NET:
//ORIGINAL LINE: ForkJoinTask<?> t;
			ForkJoinTask<?> t;
			if ((ws = WorkQueues) != null)
			{
				for (int i = 0; i < ws.Length; i += 2)
				{
					if ((w = ws[i]) != null && (t = w.Poll()) != null)
					{
						return t;
					}
				}
			}
			return null;
		}

		/// <summary>
		/// Removes all available unexecuted submitted and forked tasks
		/// from scheduling queues and adds them to the given collection,
		/// without altering their execution status. These may include
		/// artificially generated or wrapped tasks. This method is
		/// designed to be invoked only when the pool is known to be
		/// quiescent. Invocations at other times may not remove all
		/// tasks. A failure encountered while attempting to add elements
		/// to collection {@code c} may result in elements being in
		/// neither, either or both collections when the associated
		/// exception is thrown.  The behavior of this operation is
		/// undefined if the specified collection is modified while the
		/// operation is in progress.
		/// </summary>
		/// <param name="c"> the collection to transfer elements into </param>
		/// <returns> the number of elements transferred </returns>
//JAVA TO C# CONVERTER TODO TASK: There is no .NET equivalent to the Java 'super' constraint:
//ORIGINAL LINE: protected int drainTasksTo(java.util.Collection<? base ForkJoinTask<?>> c)
		protected internal virtual int drainTasksTo<T1>(ICollection<T1> c)
		{
			int count = 0;
			WorkQueue[] ws;
			WorkQueue w;
//JAVA TO C# CONVERTER TODO TASK: Java wildcard generics are not converted to .NET:
//ORIGINAL LINE: ForkJoinTask<?> t;
			ForkJoinTask<?> t;
			if ((ws = WorkQueues) != null)
			{
				for (int i = 0; i < ws.Length; ++i)
				{
					if ((w = ws[i]) != null)
					{
						while ((t = w.Poll()) != null)
						{
							c.Add(t);
							++count;
						}
					}
				}
			}
			return count;
		}

		/// <summary>
		/// Returns a string identifying this pool, as well as its state,
		/// including indications of run state, parallelism level, and
		/// worker and task counts.
		/// </summary>
		/// <returns> a string identifying this pool, as well as its state </returns>
		public override String ToString()
		{
			// Use a single pass through workQueues to collect counts
			long qt = 0L, qs = 0L;
			int rc = 0;
			AtomicLong sc = StealCounter;
			long st = (sc == null) ? 0L : sc.Get();
			long c = Ctl;
			WorkQueue[] ws;
			WorkQueue w;
			if ((ws = WorkQueues) != null)
			{
				for (int i = 0; i < ws.Length; ++i)
				{
					if ((w = ws[i]) != null)
					{
						int size = w.QueueSize();
						if ((i & 1) == 0)
						{
							qs += size;
						}
						else
						{
							qt += size;
							st += w.Nsteals;
							if (w.ApparentlyUnblocked)
							{
								++rc;
							}
						}
					}
				}
			}
			int pc = (Config & SMASK);
			int tc = pc + (short)((long)((ulong)c >> TC_SHIFT));
			int ac = pc + (int)(c >> AC_SHIFT);
			if (ac < 0) // ignore transient negative
			{
				ac = 0;
			}
			int rs = RunState;
			String level = ((rs & TERMINATED) != 0 ? "Terminated" : (rs & STOP) != 0 ? "Terminating" : (rs & SHUTDOWN) != 0 ? "Shutting down" : "Running");
			return base.ToString() + "[" + level + ", parallelism = " + pc + ", size = " + tc + ", active = " + ac + ", running = " + rc + ", steals = " + st + ", tasks = " + qt + ", submissions = " + qs + "]";
		}

		/// <summary>
		/// Possibly initiates an orderly shutdown in which previously
		/// submitted tasks are executed, but no new tasks will be
		/// accepted. Invocation has no effect on execution state if this
		/// is the <seealso cref="#commonPool()"/>, and no additional effect if
		/// already shut down.  Tasks that are in the process of being
		/// submitted concurrently during the course of this method may or
		/// may not be rejected.
		/// </summary>
		/// <exception cref="SecurityException"> if a security manager exists and
		///         the caller is not permitted to modify threads
		///         because it does not hold {@link
		///         java.lang.RuntimePermission}{@code ("modifyThread")} </exception>
		public override void Shutdown()
		{
			CheckPermission();
			TryTerminate(false, true);
		}

		/// <summary>
		/// Possibly attempts to cancel and/or stop all tasks, and reject
		/// all subsequently submitted tasks.  Invocation has no effect on
		/// execution state if this is the <seealso cref="#commonPool()"/>, and no
		/// additional effect if already shut down. Otherwise, tasks that
		/// are in the process of being submitted or executed concurrently
		/// during the course of this method may or may not be
		/// rejected. This method cancels both existing and unexecuted
		/// tasks, in order to permit termination in the presence of task
		/// dependencies. So the method always returns an empty list
		/// (unlike the case for some other Executors).
		/// </summary>
		/// <returns> an empty list </returns>
		/// <exception cref="SecurityException"> if a security manager exists and
		///         the caller is not permitted to modify threads
		///         because it does not hold {@link
		///         java.lang.RuntimePermission}{@code ("modifyThread")} </exception>
		public override IList<Runnable> ShutdownNow()
		{
			CheckPermission();
			TryTerminate(true, true);
			return Collections.EmptyList();
		}

		/// <summary>
		/// Returns {@code true} if all tasks have completed following shut down.
		/// </summary>
		/// <returns> {@code true} if all tasks have completed following shut down </returns>
		public override bool Terminated
		{
			get
			{
				return (RunState & TERMINATED) != 0;
			}
		}

		/// <summary>
		/// Returns {@code true} if the process of termination has
		/// commenced but not yet completed.  This method may be useful for
		/// debugging. A return of {@code true} reported a sufficient
		/// period after shutdown may indicate that submitted tasks have
		/// ignored or suppressed interruption, or are waiting for I/O,
		/// causing this executor not to properly terminate. (See the
		/// advisory notes for class <seealso cref="ForkJoinTask"/> stating that
		/// tasks should not normally entail blocking operations.  But if
		/// they do, they must abort them on interrupt.)
		/// </summary>
		/// <returns> {@code true} if terminating but not yet terminated </returns>
		public virtual bool Terminating
		{
			get
			{
				int rs = RunState;
				return (rs & STOP) != 0 && (rs & TERMINATED) == 0;
			}
		}

		/// <summary>
		/// Returns {@code true} if this pool has been shut down.
		/// </summary>
		/// <returns> {@code true} if this pool has been shut down </returns>
		public override bool Shutdown
		{
			get
			{
				return (RunState & SHUTDOWN) != 0;
			}
		}

		/// <summary>
		/// Blocks until all tasks have completed execution after a
		/// shutdown request, or the timeout occurs, or the current thread
		/// is interrupted, whichever happens first. Because the {@link
		/// #commonPool()} never terminates until program shutdown, when
		/// applied to the common pool, this method is equivalent to {@link
		/// #awaitQuiescence(long, TimeUnit)} but always returns {@code false}.
		/// </summary>
		/// <param name="timeout"> the maximum time to wait </param>
		/// <param name="unit"> the time unit of the timeout argument </param>
		/// <returns> {@code true} if this executor terminated and
		///         {@code false} if the timeout elapsed before termination </returns>
		/// <exception cref="InterruptedException"> if interrupted while waiting </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public boolean awaitTermination(long timeout, java.util.concurrent.TimeUnit unit) throws InterruptedException
		public override bool AwaitTermination(long timeout, TimeUnit unit)
		{
			if (Thread.Interrupted())
			{
				throw new InterruptedException();
			}
			if (this == Common)
			{
				AwaitQuiescence(timeout, unit);
				return false;
			}
			long nanos = unit.ToNanos(timeout);
			if (Terminated)
			{
				return true;
			}
			if (nanos <= 0L)
			{
				return false;
			}
			long deadline = System.nanoTime() + nanos;
			lock (this)
			{
				for (;;)
				{
					if (Terminated)
					{
						return true;
					}
					if (nanos <= 0L)
					{
						return false;
					}
					long millis = TimeUnit.NANOSECONDS.ToMillis(nanos);
					Monitor.Wait(this, TimeSpan.FromMilliseconds(millis > 0L ? millis : 1L));
					nanos = deadline - System.nanoTime();
				}
			}
		}

		/// <summary>
		/// If called by a ForkJoinTask operating in this pool, equivalent
		/// in effect to <seealso cref="ForkJoinTask#helpQuiesce"/>. Otherwise,
		/// waits and/or attempts to assist performing tasks until this
		/// pool <seealso cref="#isQuiescent"/> or the indicated timeout elapses.
		/// </summary>
		/// <param name="timeout"> the maximum time to wait </param>
		/// <param name="unit"> the time unit of the timeout argument </param>
		/// <returns> {@code true} if quiescent; {@code false} if the
		/// timeout elapsed. </returns>
		public virtual bool AwaitQuiescence(long timeout, TimeUnit unit)
		{
			long nanos = unit.ToNanos(timeout);
			ForkJoinWorkerThread wt;
			Thread thread = Thread.CurrentThread;
			if ((thread is ForkJoinWorkerThread) && (wt = (ForkJoinWorkerThread)thread).pool == this)
			{
				HelpQuiescePool(wt.WorkQueue);
				return true;
			}
			long startTime = System.nanoTime();
			WorkQueue[] ws;
			int r = 0, m ;
			bool found = true;
			while (!Quiescent && (ws = WorkQueues) != null && (m = ws.Length - 1) >= 0)
			{
				if (!found)
				{
					if ((System.nanoTime() - startTime) > nanos)
					{
						return false;
					}
					Thread.@yield(); // cannot block
				}
				found = false;
				for (int j = (m + 1) << 2; j >= 0; --j)
				{
//JAVA TO C# CONVERTER TODO TASK: Java wildcard generics are not converted to .NET:
//ORIGINAL LINE: ForkJoinTask<?> t;
					ForkJoinTask<?> t;
					WorkQueue q;
					int b, k;
					if ((k = r++ & m) <= m && k >= 0 && (q = ws[k]) != null && (b = q.@base) - q.Top < 0)
					{
						found = true;
						if ((t = q.PollAt(b)) != null)
						{
							t.DoExec();
						}
						break;
					}
				}
			}
			return true;
		}

		/// <summary>
		/// Waits and/or attempts to assist performing tasks indefinitely
		/// until the <seealso cref="#commonPool()"/> <seealso cref="#isQuiescent"/>.
		/// </summary>
		internal static void QuiesceCommonPool()
		{
			Common.AwaitQuiescence(Long.MaxValue, TimeUnit.NANOSECONDS);
		}

		/// <summary>
		/// Interface for extending managed parallelism for tasks running
		/// in <seealso cref="ForkJoinPool"/>s.
		/// 
		/// <para>A {@code ManagedBlocker} provides two methods.  Method
		/// <seealso cref="#isReleasable"/> must return {@code true} if blocking is
		/// not necessary. Method <seealso cref="#block"/> blocks the current thread
		/// if necessary (perhaps internally invoking {@code isReleasable}
		/// before actually blocking). These actions are performed by any
		/// thread invoking <seealso cref="ForkJoinPool#managedBlock(ManagedBlocker)"/>.
		/// The unusual methods in this API accommodate synchronizers that
		/// may, but don't usually, block for long periods. Similarly, they
		/// allow more efficient internal handling of cases in which
		/// additional workers may be, but usually are not, needed to
		/// ensure sufficient parallelism.  Toward this end,
		/// implementations of method {@code isReleasable} must be amenable
		/// to repeated invocation.
		/// 
		/// </para>
		/// <para>For example, here is a ManagedBlocker based on a
		/// ReentrantLock:
		///  <pre> {@code
		/// class ManagedLocker implements ManagedBlocker {
		///   final ReentrantLock lock;
		///   boolean hasLock = false;
		///   ManagedLocker(ReentrantLock lock) { this.lock = lock; }
		///   public boolean block() {
		///     if (!hasLock)
		///       lock.lock();
		///     return true;
		///   }
		///   public boolean isReleasable() {
		///     return hasLock || (hasLock = lock.tryLock());
		///   }
		/// }}</pre>
		/// 
		/// </para>
		/// <para>Here is a class that possibly blocks waiting for an
		/// item on a given queue:
		///  <pre> {@code
		/// class QueueTaker<E> implements ManagedBlocker {
		///   final BlockingQueue<E> queue;
		///   volatile E item = null;
		///   QueueTaker(BlockingQueue<E> q) { this.queue = q; }
		///   public boolean block() throws InterruptedException {
		///     if (item == null)
		///       item = queue.take();
		///     return true;
		///   }
		///   public boolean isReleasable() {
		///     return item != null || (item = queue.poll()) != null;
		///   }
		///   public E getItem() { // call after pool.managedBlock completes
		///     return item;
		///   }
		/// }}</pre>
		/// </para>
		/// </summary>
		public interface ManagedBlocker
		{
			/// <summary>
			/// Possibly blocks the current thread, for example waiting for
			/// a lock or condition.
			/// </summary>
			/// <returns> {@code true} if no additional blocking is necessary
			/// (i.e., if isReleasable would return true) </returns>
			/// <exception cref="InterruptedException"> if interrupted while waiting
			/// (the method is not required to do so, but is allowed to) </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: boolean block() throws InterruptedException;
			bool Block();

			/// <summary>
			/// Returns {@code true} if blocking is unnecessary. </summary>
			/// <returns> {@code true} if blocking is unnecessary </returns>
			bool Releasable {get;}
		}

		/// <summary>
		/// Runs the given possibly blocking task.  When {@linkplain
		/// ForkJoinTask#inForkJoinPool() running in a ForkJoinPool}, this
		/// method possibly arranges for a spare thread to be activated if
		/// necessary to ensure sufficient parallelism while the current
		/// thread is blocked in <seealso cref="ManagedBlocker#block blocker.block()"/>.
		/// 
		/// <para>This method repeatedly calls {@code blocker.isReleasable()} and
		/// {@code blocker.block()} until either method returns {@code true}.
		/// Every call to {@code blocker.block()} is preceded by a call to
		/// {@code blocker.isReleasable()} that returned {@code false}.
		/// 
		/// </para>
		/// <para>If not running in a ForkJoinPool, this method is
		/// behaviorally equivalent to
		///  <pre> {@code
		/// while (!blocker.isReleasable())
		///   if (blocker.block())
		///     break;}</pre>
		/// 
		/// If running in a ForkJoinPool, the pool may first be expanded to
		/// ensure sufficient parallelism available during the call to
		/// {@code blocker.block()}.
		/// 
		/// </para>
		/// </summary>
		/// <param name="blocker"> the blocker task </param>
		/// <exception cref="InterruptedException"> if {@code blocker.block()} did so </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public static void managedBlock(ManagedBlocker blocker) throws InterruptedException
		public static void ManagedBlock(ManagedBlocker blocker)
		{
			ForkJoinPool p;
			ForkJoinWorkerThread wt;
			Thread t = Thread.CurrentThread;
			if ((t is ForkJoinWorkerThread) && (p = (wt = (ForkJoinWorkerThread)t).pool) != null)
			{
				WorkQueue w = wt.WorkQueue;
				while (!blocker.Releasable)
				{
					if (p.TryCompensate(w))
					{
						try
						{
							do
							{
							} while (!blocker.Releasable && !blocker.Block());
						}
						finally
						{
							U.getAndAddLong(p, CTL, AC_UNIT);
						}
						break;
					}
				}
			}
			else
			{
				do
				{
				} while (!blocker.Releasable && !blocker.Block());
			}
		}

		// AbstractExecutorService overrides.  These rely on undocumented
		// fact that ForkJoinTask.adapt returns ForkJoinTasks that also
		// implement RunnableFuture.

		protected internal override RunnableFuture<T> newTaskFor<T>(Runnable runnable, T value)
		{
			return new ForkJoinTask.AdaptedRunnable<T>(runnable, value);
		}

		protected internal override RunnableFuture<T> newTaskFor<T>(Callable<T> callable)
		{
			return new ForkJoinTask.AdaptedCallable<T>(callable);
		}

		// Unsafe mechanics
		private static readonly sun.misc.Unsafe U;
		private static readonly int ABASE;
		private static readonly int ASHIFT;
		private static readonly long CTL;
		private static readonly long RUNSTATE;
		private static readonly long STEALCOUNTER;
		private static readonly long PARKBLOCKER;
		private static readonly long QTOP;
		private static readonly long QLOCK;
		private static readonly long QSCANSTATE;
		private static readonly long QPARKER;
		private static readonly long QCURRENTSTEAL;
		private static readonly long QCURRENTJOIN;

		static ForkJoinPool()
		{
			// initialize field offsets for CAS etc
			try
			{
				U = sun.misc.Unsafe.Unsafe;
				Class k = typeof(ForkJoinPool);
				CTL = U.objectFieldOffset(k.GetDeclaredField("ctl"));
				RUNSTATE = U.objectFieldOffset(k.GetDeclaredField("runState"));
				STEALCOUNTER = U.objectFieldOffset(k.GetDeclaredField("stealCounter"));
				Class tk = typeof(Thread);
				PARKBLOCKER = U.objectFieldOffset(tk.GetDeclaredField("parkBlocker"));
				Class wk = typeof(WorkQueue);
				QTOP = U.objectFieldOffset(wk.GetDeclaredField("top"));
				QLOCK = U.objectFieldOffset(wk.GetDeclaredField("qlock"));
				QSCANSTATE = U.objectFieldOffset(wk.GetDeclaredField("scanState"));
				QPARKER = U.objectFieldOffset(wk.GetDeclaredField("parker"));
				QCURRENTSTEAL = U.objectFieldOffset(wk.GetDeclaredField("currentSteal"));
				QCURRENTJOIN = U.objectFieldOffset(wk.GetDeclaredField("currentJoin"));
				Class ak = typeof(ForkJoinTask[]);
				ABASE = U.arrayBaseOffset(ak);
				int scale = U.arrayIndexScale(ak);
				if ((scale & (scale - 1)) != 0)
				{
					throw new Error("data type scale not a power of two");
				}
				ASHIFT = 31 - Integer.NumberOfLeadingZeros(scale);
			}
			catch (Exception e)
			{
				throw new Error(e);
			}

			CommonMaxSpares = DEFAULT_COMMON_MAX_SPARES;
			DefaultForkJoinWorkerThreadFactory = new DefaultForkJoinWorkerThreadFactory();
			ModifyThreadPermission = new RuntimePermission("modifyThread");

			Common = java.security.AccessController.doPrivileged(new PrivilegedActionAnonymousInnerClassHelper());
			int par = Common.Config & SMASK; // report 1 even if threads disabled
			CommonParallelism = par > 0 ? par : 1;
				Permissions innocuousPerms = new Permissions();
				innocuousPerms.Add(ModifyThreadPermission);
				innocuousPerms.Add(new RuntimePermission("enableContextClassLoaderOverride"));
				innocuousPerms.Add(new RuntimePermission("modifyThreadGroup"));
				innocuousAcc = new AccessControlContext(new ProtectionDomain[] { new ProtectionDomain(null, innocuousPerms)
		}

		private class PrivilegedActionAnonymousInnerClassHelper : java.security.PrivilegedAction<ForkJoinPool>
		{
			public PrivilegedActionAnonymousInnerClassHelper()
			{
			}

			public virtual ForkJoinPool Run()
			{
				return MakeCommonPool();
			}
		}

		/// <summary>
		/// Creates and returns the common pool, respecting user settings
		/// specified via system properties.
		/// </summary>
		private static ForkJoinPool MakeCommonPool()
		{
			int parallelism = -1;
			ForkJoinWorkerThreadFactory factory = null;
			UncaughtExceptionHandler handler = null;
			try // ignore exceptions in accessing/parsing properties
			{
				String pp = System.getProperty("java.util.concurrent.ForkJoinPool.common.parallelism");
				String fp = System.getProperty("java.util.concurrent.ForkJoinPool.common.threadFactory");
				String hp = System.getProperty("java.util.concurrent.ForkJoinPool.common.exceptionHandler");
				if (pp != null)
				{
					parallelism = Convert.ToInt32(pp);
				}
				if (fp != null)
				{
					factory = ((ForkJoinWorkerThreadFactory)ClassLoader.SystemClassLoader.LoadClass(fp).NewInstance());
				}
				if (hp != null)
				{
					handler = ((UncaughtExceptionHandler)ClassLoader.SystemClassLoader.LoadClass(hp).NewInstance());
				}
			}
			catch (Exception)
			{
			}
			if (factory == null)
			{
				if (System.SecurityManager == null)
				{
					factory = DefaultForkJoinWorkerThreadFactory;
				}
				else // use security-managed default
				{
					factory = new InnocuousForkJoinWorkerThreadFactory();
				}
			}
			if (parallelism < 0 && (parallelism = Runtime.Runtime.availableProcessors() - 1) <= 0) // default 1 less than #cores
			{
				parallelism = 1;
			}
			if (parallelism > MAX_CAP)
			{
				parallelism = MAX_CAP;
			}
			return new ForkJoinPool(parallelism, factory, handler, LIFO_QUEUE, "ForkJoinPool.commonPool-worker-");
		}

		/// <summary>
		/// Factory for innocuous worker threads
		/// </summary>
		internal sealed class InnocuousForkJoinWorkerThreadFactory : ForkJoinWorkerThreadFactory
		{

			/// <summary>
			/// An ACC to restrict permissions for the factory itself.
			/// The constructed workers have no permissions set.
			/// </summary>
			internal static readonly AccessControlContext InnocuousAcc;
		}

			public ForkJoinWorkerThread NewThread(ForkJoinPool pool)
			{
				return (ForkJoinWorkerThread.InnocuousForkJoinWorkerThread) java.security.AccessController.doPrivileged(new PrivilegedActionAnonymousInnerClassHelper(this, pool), innocuousAcc);
			}

			private class PrivilegedActionAnonymousInnerClassHelper : java.security.PrivilegedAction<ForkJoinWorkerThread>
			{
				private readonly ForkJoinPool OuterInstance;

				private java.util.concurrent.ForkJoinPool Pool;

				public PrivilegedActionAnonymousInnerClassHelper(ForkJoinPool outerInstance, java.util.concurrent.ForkJoinPool pool)
				{
					this.OuterInstance = outerInstance;
					this.Pool = pool;
				}

				public virtual ForkJoinWorkerThread Run()
				{
					return new ForkJoinWorkerThread.InnocuousForkJoinWorkerThread(Pool);
				}
			}
	}

}

}
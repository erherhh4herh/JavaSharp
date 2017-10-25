/*
 * Copyright (c) 2001, 2013, Oracle and/or its affiliates. All rights reserved.
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

namespace java.nio.channels
{

	/// <summary>
	/// A token representing a lock on a region of a file.
	/// 
	/// <para> A file-lock object is created each time a lock is acquired on a file via
	/// one of the <seealso cref="FileChannel#lock(long,long,boolean) lock"/> or {@link
	/// FileChannel#tryLock(long,long,boolean) tryLock} methods of the
	/// <seealso cref="FileChannel"/> class, or the {@link
	/// AsynchronousFileChannel#lock(long,long,boolean,Object,CompletionHandler) lock}
	/// or <seealso cref="AsynchronousFileChannel#tryLock(long,long,boolean) tryLock"/>
	/// methods of the <seealso cref="AsynchronousFileChannel"/> class.
	/// 
	/// </para>
	/// <para> A file-lock object is initially valid.  It remains valid until the lock
	/// is released by invoking the <seealso cref="#release release"/> method, by closing the
	/// channel that was used to acquire it, or by the termination of the Java
	/// virtual machine, whichever comes first.  The validity of a lock may be
	/// tested by invoking its <seealso cref="#isValid isValid"/> method.
	/// 
	/// </para>
	/// <para> A file lock is either <i>exclusive</i> or <i>shared</i>.  A shared lock
	/// prevents other concurrently-running programs from acquiring an overlapping
	/// exclusive lock, but does allow them to acquire overlapping shared locks.  An
	/// exclusive lock prevents other programs from acquiring an overlapping lock of
	/// either type.  Once it is released, a lock has no further effect on the locks
	/// that may be acquired by other programs.
	/// 
	/// </para>
	/// <para> Whether a lock is exclusive or shared may be determined by invoking its
	/// <seealso cref="#isShared isShared"/> method.  Some platforms do not support shared
	/// locks, in which case a request for a shared lock is automatically converted
	/// into a request for an exclusive lock.
	/// 
	/// </para>
	/// <para> The locks held on a particular file by a single Java virtual machine do
	/// not overlap.  The <seealso cref="#overlaps overlaps"/> method may be used to test
	/// whether a candidate lock range overlaps an existing lock.
	/// 
	/// </para>
	/// <para> A file-lock object records the file channel upon whose file the lock is
	/// held, the type and validity of the lock, and the position and size of the
	/// locked region.  Only the validity of a lock is subject to change over time;
	/// all other aspects of a lock's state are immutable.
	/// 
	/// </para>
	/// <para> File locks are held on behalf of the entire Java virtual machine.
	/// They are not suitable for controlling access to a file by multiple
	/// threads within the same virtual machine.
	/// 
	/// </para>
	/// <para> File-lock objects are safe for use by multiple concurrent threads.
	/// 
	/// 
	/// <a name="pdep"></a><h2> Platform dependencies </h2>
	/// 
	/// </para>
	/// <para> This file-locking API is intended to map directly to the native locking
	/// facility of the underlying operating system.  Thus the locks held on a file
	/// should be visible to all programs that have access to the file, regardless
	/// of the language in which those programs are written.
	/// 
	/// </para>
	/// <para> Whether or not a lock actually prevents another program from accessing
	/// the content of the locked region is system-dependent and therefore
	/// unspecified.  The native file-locking facilities of some systems are merely
	/// <i>advisory</i>, meaning that programs must cooperatively observe a known
	/// locking protocol in order to guarantee data integrity.  On other systems
	/// native file locks are <i>mandatory</i>, meaning that if one program locks a
	/// region of a file then other programs are actually prevented from accessing
	/// that region in a way that would violate the lock.  On yet other systems,
	/// whether native file locks are advisory or mandatory is configurable on a
	/// per-file basis.  To ensure consistent and correct behavior across platforms,
	/// it is strongly recommended that the locks provided by this API be used as if
	/// they were advisory locks.
	/// 
	/// </para>
	/// <para> On some systems, acquiring a mandatory lock on a region of a file
	/// prevents that region from being {@link java.nio.channels.FileChannel#map
	/// <i>mapped into memory</i>}, and vice versa.  Programs that combine
	/// locking and mapping should be prepared for this combination to fail.
	/// 
	/// </para>
	/// <para> On some systems, closing a channel releases all locks held by the Java
	/// virtual machine on the underlying file regardless of whether the locks were
	/// acquired via that channel or via another channel open on the same file.  It
	/// is strongly recommended that, within a program, a unique channel be used to
	/// acquire all locks on any given file.
	/// 
	/// </para>
	/// <para> Some network filesystems permit file locking to be used with
	/// memory-mapped files only when the locked regions are page-aligned and a
	/// whole multiple of the underlying hardware's page size.  Some network
	/// filesystems do not implement file locks on regions that extend past a
	/// certain position, often 2<sup>30</sup> or 2<sup>31</sup>.  In general, great
	/// care should be taken when locking files that reside on network filesystems.
	/// 
	/// 
	/// @author Mark Reinhold
	/// @author JSR-51 Expert Group
	/// @since 1.4
	/// </para>
	/// </summary>

	public abstract class FileLock : AutoCloseable
	{

		private readonly Channel Channel_Renamed;
		private readonly long Position_Renamed;
		private readonly long Size_Renamed;
		private readonly bool Shared_Renamed;

		/// <summary>
		/// Initializes a new instance of this class.
		/// </summary>
		/// <param name="channel">
		///         The file channel upon whose file this lock is held
		/// </param>
		/// <param name="position">
		///         The position within the file at which the locked region starts;
		///         must be non-negative
		/// </param>
		/// <param name="size">
		///         The size of the locked region; must be non-negative, and the sum
		///         <tt>position</tt>&nbsp;+&nbsp;<tt>size</tt> must be non-negative
		/// </param>
		/// <param name="shared">
		///         <tt>true</tt> if this lock is shared,
		///         <tt>false</tt> if it is exclusive
		/// </param>
		/// <exception cref="IllegalArgumentException">
		///         If the preconditions on the parameters do not hold </exception>
		protected internal FileLock(FileChannel channel, long position, long size, bool shared)
		{
			if (position < 0)
			{
				throw new IllegalArgumentException("Negative position");
			}
			if (size < 0)
			{
				throw new IllegalArgumentException("Negative size");
			}
			if (position + size < 0)
			{
				throw new IllegalArgumentException("Negative position + size");
			}
			this.Channel_Renamed = channel;
			this.Position_Renamed = position;
			this.Size_Renamed = size;
			this.Shared_Renamed = shared;
		}

		/// <summary>
		/// Initializes a new instance of this class.
		/// </summary>
		/// <param name="channel">
		///         The channel upon whose file this lock is held
		/// </param>
		/// <param name="position">
		///         The position within the file at which the locked region starts;
		///         must be non-negative
		/// </param>
		/// <param name="size">
		///         The size of the locked region; must be non-negative, and the sum
		///         <tt>position</tt>&nbsp;+&nbsp;<tt>size</tt> must be non-negative
		/// </param>
		/// <param name="shared">
		///         <tt>true</tt> if this lock is shared,
		///         <tt>false</tt> if it is exclusive
		/// </param>
		/// <exception cref="IllegalArgumentException">
		///         If the preconditions on the parameters do not hold
		/// 
		/// @since 1.7 </exception>
		protected internal FileLock(AsynchronousFileChannel channel, long position, long size, bool shared)
		{
			if (position < 0)
			{
				throw new IllegalArgumentException("Negative position");
			}
			if (size < 0)
			{
				throw new IllegalArgumentException("Negative size");
			}
			if (position + size < 0)
			{
				throw new IllegalArgumentException("Negative position + size");
			}
			this.Channel_Renamed = channel;
			this.Position_Renamed = position;
			this.Size_Renamed = size;
			this.Shared_Renamed = shared;
		}

		/// <summary>
		/// Returns the file channel upon whose file this lock was acquired.
		/// 
		/// <para> This method has been superseded by the <seealso cref="#acquiredBy acquiredBy"/>
		/// method.
		/// 
		/// </para>
		/// </summary>
		/// <returns>  The file channel, or {@code null} if the file lock was not
		///          acquired by a file channel. </returns>
		public FileChannel Channel()
		{
			return (Channel_Renamed is FileChannel) ? (FileChannel)Channel_Renamed : null;
		}

		/// <summary>
		/// Returns the channel upon whose file this lock was acquired.
		/// </summary>
		/// <returns>  The channel upon whose file this lock was acquired.
		/// 
		/// @since 1.7 </returns>
		public virtual Channel AcquiredBy()
		{
			return Channel_Renamed;
		}

		/// <summary>
		/// Returns the position within the file of the first byte of the locked
		/// region.
		/// 
		/// <para> A locked region need not be contained within, or even overlap, the
		/// actual underlying file, so the value returned by this method may exceed
		/// the file's current size.  </para>
		/// </summary>
		/// <returns>  The position </returns>
		public long Position()
		{
			return Position_Renamed;
		}

		/// <summary>
		/// Returns the size of the locked region in bytes.
		/// 
		/// <para> A locked region need not be contained within, or even overlap, the
		/// actual underlying file, so the value returned by this method may exceed
		/// the file's current size.  </para>
		/// </summary>
		/// <returns>  The size of the locked region </returns>
		public long Size()
		{
			return Size_Renamed;
		}

		/// <summary>
		/// Tells whether this lock is shared.
		/// </summary>
		/// <returns> <tt>true</tt> if lock is shared,
		///         <tt>false</tt> if it is exclusive </returns>
		public bool Shared
		{
			get
			{
				return Shared_Renamed;
			}
		}

		/// <summary>
		/// Tells whether or not this lock overlaps the given lock range.
		/// </summary>
		/// <param name="position">
		///          The starting position of the lock range </param>
		/// <param name="size">
		///          The size of the lock range
		/// </param>
		/// <returns>  <tt>true</tt> if, and only if, this lock and the given lock
		///          range overlap by at least one byte </returns>
		public bool Overlaps(long position, long size)
		{
			if (position + size <= this.Position_Renamed)
			{
				return false; // That is below this
			}
			if (this.Position_Renamed + this.Size_Renamed <= position)
			{
				return false; // This is below that
			}
			return true;
		}

		/// <summary>
		/// Tells whether or not this lock is valid.
		/// 
		/// <para> A lock object remains valid until it is released or the associated
		/// file channel is closed, whichever comes first.  </para>
		/// </summary>
		/// <returns>  <tt>true</tt> if, and only if, this lock is valid </returns>
		public abstract bool Valid {get;}

		/// <summary>
		/// Releases this lock.
		/// 
		/// <para> If this lock object is valid then invoking this method releases the
		/// lock and renders the object invalid.  If this lock object is invalid
		/// then invoking this method has no effect.  </para>
		/// </summary>
		/// <exception cref="ClosedChannelException">
		///          If the channel that was used to acquire this lock
		///          is no longer open
		/// </exception>
		/// <exception cref="IOException">
		///          If an I/O error occurs </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public abstract void release() throws java.io.IOException;
		public abstract void Release();

		/// <summary>
		/// This method invokes the <seealso cref="#release"/> method. It was added
		/// to the class so that it could be used in conjunction with the
		/// automatic resource management block construct.
		/// 
		/// @since 1.7
		/// </summary>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public final void close() throws java.io.IOException
		public void Close()
		{
			Release();
		}

		/// <summary>
		/// Returns a string describing the range, type, and validity of this lock.
		/// </summary>
		/// <returns>  A descriptive string </returns>
		public sealed override String ToString()
		{
//JAVA TO C# CONVERTER WARNING: The .NET Type.FullName property will not always yield results identical to the Java Class.getName method:
			return (this.GetType().FullName + "[" + Position_Renamed + ":" + Size_Renamed + " " + (Shared_Renamed ? "shared" : "exclusive") + " " + (Valid ? "valid" : "invalid") + "]");
		}

	}

}
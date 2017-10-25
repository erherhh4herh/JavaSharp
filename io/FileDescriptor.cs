using System.Collections.Generic;
using System.Runtime.InteropServices;

/*
 * Copyright (c) 2003, 2011, Oracle and/or its affiliates. All rights reserved.
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

namespace java.io
{


	/// <summary>
	/// Instances of the file descriptor class serve as an opaque handle
	/// to the underlying machine-specific structure representing an
	/// open file, an open socket, or another source or sink of bytes.
	/// The main practical use for a file descriptor is to create a
	/// <seealso cref="FileInputStream"/> or <seealso cref="FileOutputStream"/> to contain it.
	/// 
	/// <para>Applications should not create their own file descriptors.
	/// 
	/// @author  Pavani Diwanji
	/// @since   JDK1.0
	/// </para>
	/// </summary>
	public sealed class FileDescriptor
	{

		private int Fd;

		private long Handle;

		private Closeable Parent;
		private IList<Closeable> OtherParents;
		private bool Closed;

		/// <summary>
		/// Constructs an (invalid) FileDescriptor
		/// object.
		/// </summary>
		public FileDescriptor()
		{
			Fd = -1;
			Handle = -1;
		}

		static FileDescriptor()
		{
			initIDs();
			sun.misc.SharedSecrets.setJavaIOFileDescriptorAccess(new JavaIOFileDescriptorAccessAnonymousInnerClassHelper()
		   );
		}

		private class JavaIOFileDescriptorAccessAnonymousInnerClassHelper : sun.misc.JavaIOFileDescriptorAccess
		{
			public JavaIOFileDescriptorAccessAnonymousInnerClassHelper()
			{
			}

			public virtual void Set(FileDescriptor obj, int fd)
			{
				obj.Fd = fd;
			}

			public virtual int Get(FileDescriptor obj)
			{
				return obj.Fd;
			}

			public virtual void SetHandle(FileDescriptor obj, long handle)
			{
				obj.Handle = handle;
			}

			public virtual long GetHandle(FileDescriptor obj)
			{
				return obj.Handle;
			}
		}

		// Set up JavaIOFileDescriptorAccess in SharedSecrets

		/// <summary>
		/// A handle to the standard input stream. Usually, this file
		/// descriptor is not used directly, but rather via the input stream
		/// known as {@code System.in}.
		/// </summary>
		/// <seealso cref=     java.lang.System#in </seealso>
		public static readonly FileDescriptor @in = StandardStream(0);

		/// <summary>
		/// A handle to the standard output stream. Usually, this file
		/// descriptor is not used directly, but rather via the output stream
		/// known as {@code System.out}. </summary>
		/// <seealso cref=     java.lang.System#out </seealso>
		public static readonly FileDescriptor @out = StandardStream(1);

		/// <summary>
		/// A handle to the standard error stream. Usually, this file
		/// descriptor is not used directly, but rather via the output stream
		/// known as {@code System.err}.
		/// </summary>
		/// <seealso cref=     java.lang.System#err </seealso>
		public static readonly FileDescriptor Err = StandardStream(2);

		/// <summary>
		/// Tests if this file descriptor object is valid.
		/// </summary>
		/// <returns>  {@code true} if the file descriptor object represents a
		///          valid, open file, socket, or other active I/O connection;
		///          {@code false} otherwise. </returns>
		public bool Valid()
		{
			return ((Handle != -1) || (Fd != -1));
		}

		/// <summary>
		/// Force all system buffers to synchronize with the underlying
		/// device.  This method returns after all modified data and
		/// attributes of this FileDescriptor have been written to the
		/// relevant device(s).  In particular, if this FileDescriptor
		/// refers to a physical storage medium, such as a file in a file
		/// system, sync will not return until all in-memory modified copies
		/// of buffers associated with this FileDesecriptor have been
		/// written to the physical medium.
		/// 
		/// sync is meant to be used by code that requires physical
		/// storage (such as a file) to be in a known state  For
		/// example, a class that provided a simple transaction facility
		/// might use sync to ensure that all changes to a file caused
		/// by a given transaction were recorded on a storage medium.
		/// 
		/// sync only affects buffers downstream of this FileDescriptor.  If
		/// any in-memory buffering is being done by the application (for
		/// example, by a BufferedOutputStream object), those buffers must
		/// be flushed into the FileDescriptor (for example, by invoking
		/// OutputStream.flush) before that data will be affected by sync.
		/// </summary>
		/// <exception cref="SyncFailedException">
		///        Thrown when the buffers cannot be flushed,
		///        or because the system cannot guarantee that all the
		///        buffers have been synchronized with physical media.
		/// @since     JDK1.1 </exception>
//JAVA TO C# CONVERTER TODO TASK: Replace 'unknown' with the appropriate dll name:
		[DllImport("unknown")]
		public extern void sync();

		/* This routine initializes JNI field offsets for the class */
//JAVA TO C# CONVERTER TODO TASK: Replace 'unknown' with the appropriate dll name:
		[DllImport("unknown")]
		private static extern void initIDs();

//JAVA TO C# CONVERTER TODO TASK: Replace 'unknown' with the appropriate dll name:
		[DllImport("unknown")]
		private static extern long set(int d);

		private static FileDescriptor StandardStream(int fd)
		{
			FileDescriptor desc = new FileDescriptor();
			desc.Handle = set(fd);
			return desc;
		}

		/*
		 * Package private methods to track referents.
		 * If multiple streams point to the same FileDescriptor, we cycle
		 * through the list of all referents and call close()
		 */

		/// <summary>
		/// Attach a Closeable to this FD for tracking.
		/// parent reference is added to otherParents when
		/// needed to make closeAll simpler.
		/// </summary>
		internal void Attach(Closeable c)
		{
			lock (this)
			{
				if (Parent == null)
				{
					// first caller gets to do this
					Parent = c;
				}
				else if (OtherParents == null)
				{
					OtherParents = new List<>();
					OtherParents.Add(Parent);
					OtherParents.Add(c);
				}
				else
				{
					OtherParents.Add(c);
				}
			}
		}

		/// <summary>
		/// Cycle through all Closeables sharing this FD and call
		/// close() on each one.
		/// 
		/// The caller closeable gets to call close0().
		/// </summary>
//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @SuppressWarnings("try") synchronized void closeAll(Closeable releaser) throws IOException
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
		internal void CloseAll(Closeable releaser)
		{
			lock (this)
			{
				if (!Closed)
				{
					Closed = true;
					IOException ioe = null;
					try
					{
							using (Closeable c = releaser)
							{
							if (OtherParents != null)
							{
								foreach (Closeable referent in OtherParents)
								{
									try
									{
										referent.Close();
									}
									catch (IOException x)
									{
										if (ioe == null)
										{
											ioe = x;
										}
										else
										{
											ioe.AddSuppressed(x);
										}
									}
								}
							}
							}
					}
					catch (IOException ex)
					{
						/*
						 * If releaser close() throws IOException
						 * add other exceptions as suppressed.
						 */
						if (ioe != null)
						{
							ex.AddSuppressed(ioe);
						}
						ioe = ex;
					}
					finally
					{
						if (ioe != null)
						{
							throw ioe;
						}
					}
				}
			}
		}
	}

}
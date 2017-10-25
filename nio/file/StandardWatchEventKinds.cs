using System;

/*
 * Copyright (c) 2007, 2011, Oracle and/or its affiliates. All rights reserved.
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
	/// Defines the <em>standard</em> event kinds.
	/// 
	/// @since 1.7
	/// </summary>

	public sealed class StandardWatchEventKinds
	{
		private StandardWatchEventKinds()
		{
		}

		/// <summary>
		/// A special event to indicate that events may have been lost or
		/// discarded.
		/// 
		/// <para> The <seealso cref="WatchEvent#context context"/> for this event is
		/// implementation specific and may be {@code null}. The event {@link
		/// WatchEvent#count count} may be greater than {@code 1}.
		/// 
		/// </para>
		/// </summary>
		/// <seealso cref= WatchService </seealso>
		public static readonly WatchEvent_Kind<Object> OVERFLOW = new StdWatchEventKind<Object>("OVERFLOW", typeof(Object));

		/// <summary>
		/// Directory entry created.
		/// 
		/// <para> When a directory is registered for this event then the <seealso cref="WatchKey"/>
		/// is queued when it is observed that an entry is created in the directory
		/// or renamed into the directory. The event <seealso cref="WatchEvent#count count"/>
		/// for this event is always {@code 1}.
		/// </para>
		/// </summary>
		public static readonly WatchEvent_Kind<Path> ENTRY_CREATE = new StdWatchEventKind<Path>("ENTRY_CREATE", typeof(Path));

		/// <summary>
		/// Directory entry deleted.
		/// 
		/// <para> When a directory is registered for this event then the <seealso cref="WatchKey"/>
		/// is queued when it is observed that an entry is deleted or renamed out of
		/// the directory. The event <seealso cref="WatchEvent#count count"/> for this event
		/// is always {@code 1}.
		/// </para>
		/// </summary>
		public static readonly WatchEvent_Kind<Path> ENTRY_DELETE = new StdWatchEventKind<Path>("ENTRY_DELETE", typeof(Path));

		/// <summary>
		/// Directory entry modified.
		/// 
		/// <para> When a directory is registered for this event then the <seealso cref="WatchKey"/>
		/// is queued when it is observed that an entry in the directory has been
		/// modified. The event <seealso cref="WatchEvent#count count"/> for this event is
		/// {@code 1} or greater.
		/// </para>
		/// </summary>
		public static readonly WatchEvent_Kind<Path> ENTRY_MODIFY = new StdWatchEventKind<Path>("ENTRY_MODIFY", typeof(Path));

		private class StdWatchEventKind<T> : WatchEvent_Kind<T>
		{
			internal readonly String Name_Renamed;
			internal readonly Class Type_Renamed;
			internal StdWatchEventKind(String name, Class type)
			{
				this.Name_Renamed = name;
				this.Type_Renamed = type;
			}
			public override String Name()
			{
				return Name_Renamed;
			}
			public override Class Type()
			{
				return Type_Renamed;
			}
			public override String ToString()
			{
				return Name_Renamed;
			}
		}
	}

}
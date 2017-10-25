using System;
using System.Diagnostics;
using System.Collections;
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

	using BasicFileAttributesHolder = sun.nio.fs.BasicFileAttributesHolder;

	/// <summary>
	/// Walks a file tree, generating a sequence of events corresponding to the files
	/// in the tree.
	/// 
	/// <pre>{@code
	///     Path top = ...
	///     Set<FileVisitOption> options = ...
	///     int maxDepth = ...
	/// 
	///     try (FileTreeWalker walker = new FileTreeWalker(options, maxDepth)) {
	///         FileTreeWalker.Event ev = walker.walk(top);
	///         do {
	///             process(ev);
	///             ev = walker.next();
	///         } while (ev != null);
	///     }
	/// }</pre>
	/// </summary>
	/// <seealso cref= Files#walkFileTree </seealso>

	internal class FileTreeWalker : Closeable
	{
		private readonly bool FollowLinks;
		private readonly LinkOption[] LinkOptions;
		private readonly int MaxDepth;
		private readonly ArrayDeque<DirectoryNode> Stack = new ArrayDeque<DirectoryNode>();
		private bool Closed;

		/// <summary>
		/// The element on the walking stack corresponding to a directory node.
		/// </summary>
		private class DirectoryNode
		{
			internal readonly Path Dir;
			internal readonly Object Key_Renamed;
			internal readonly DirectoryStream<Path> Stream_Renamed;
			internal readonly IEnumerator<Path> Iterator_Renamed;
			internal bool Skipped_Renamed;

			internal DirectoryNode(Path dir, Object key, DirectoryStream<Path> stream)
			{
				this.Dir = dir;
				this.Key_Renamed = key;
				this.Stream_Renamed = stream;
				this.Iterator_Renamed = stream.Iterator();
			}

			internal virtual Path Directory()
			{
				return Dir;
			}

			internal virtual Object Key()
			{
				return Key_Renamed;
			}

			internal virtual DirectoryStream<Path> Stream()
			{
				return Stream_Renamed;
			}

			internal virtual IEnumerator<Path> Iterator()
			{
				return Iterator_Renamed;
			}

			internal virtual void Skip()
			{
				Skipped_Renamed = true;
			}

			internal virtual bool Skipped()
			{
				return Skipped_Renamed;
			}
		}

		/// <summary>
		/// The event types.
		/// </summary>
		internal sealed class EventType
		{
			/// <summary>
			/// Start of a directory
			/// </summary>
			START_DIRECTORY,
			public static readonly EventType START_DIRECTORY = new EventType("START_DIRECTORY", InnerEnum.START_DIRECTORY);
			/// <summary>
			/// End of a directory
			/// </summary>
			END_DIRECTORY,
			public static readonly EventType END_DIRECTORY = new EventType("END_DIRECTORY", InnerEnum.END_DIRECTORY);
			/// <summary>
			/// An entry in a directory
			/// </summary>
			ENTRY
			public static readonly EventType ENTRY = new EventType("ENTRY", InnerEnum.ENTRY);

			private static readonly IList<EventType> valueList = new List<EventType>();

			static EventType()
			{
				valueList.Add(START_DIRECTORY);
				valueList.Add(END_DIRECTORY);
				valueList.Add(ENTRY);
			}

			public enum InnerEnum
			{
				START_DIRECTORY,
				END_DIRECTORY,
				ENTRY
			}

			private readonly string nameValue;
			private readonly int ordinalValue;
			private readonly InnerEnum innerEnumValue;
			private static int nextOrdinal = 0;

			public static IList<EventType> values()
			{
				return valueList;
			}

			public InnerEnum InnerEnumValue()
			{
				return innerEnumValue;
			}

			public int ordinal()
			{
				return ordinalValue;
			}

			public override string ToString()
			{
				return nameValue;
			}

			public static EventType valueOf(string name)
			{
				foreach (EventType enumInstance in EventType.values())
				{
					if (enumInstance.nameValue == name)
					{
						return enumInstance;
					}
				}
				throw new System.ArgumentException(name);
			}
		}

		/// <summary>
		/// Events returned by the <seealso cref="#walk"/> and <seealso cref="#next"/> methods.
		/// </summary>
		internal class Event
		{
			internal readonly EventType Type_Renamed;
			internal readonly Path File_Renamed;
			internal readonly BasicFileAttributes Attrs;
			internal readonly IOException Ioe;

			internal Event(EventType type, Path file, BasicFileAttributes attrs, IOException ioe)
			{
				this.Type_Renamed = type;
				this.File_Renamed = file;
				this.Attrs = attrs;
				this.Ioe = ioe;
			}

			internal Event(EventType type, Path file, BasicFileAttributes attrs) : this(type, file, attrs, null)
			{
			}

			internal Event(EventType type, Path file, IOException ioe) : this(type, file, null, ioe)
			{
			}

			internal virtual EventType Type()
			{
				return Type_Renamed;
			}

			internal virtual Path File()
			{
				return File_Renamed;
			}

			internal virtual BasicFileAttributes Attributes()
			{
				return Attrs;
			}

			internal virtual IOException IoeException()
			{
				return Ioe;
			}
		}

		/// <summary>
		/// Creates a {@code FileTreeWalker}.
		/// </summary>
		/// <exception cref="IllegalArgumentException">
		///          if {@code maxDepth} is negative </exception>
		/// <exception cref="ClassCastException">
		///          if (@code options} contains an element that is not a
		///          {@code FileVisitOption} </exception>
		/// <exception cref="NullPointerException">
		///          if {@code options} is {@ocde null} or the options
		///          array contains a {@code null} element </exception>
		internal FileTreeWalker(ICollection<FileVisitOption> options, int maxDepth)
		{
			bool fl = false;
			foreach (FileVisitOption option in options)
			{
				// will throw NPE if options contains null
				switch (option.InnerEnumValue())
				{
					case java.nio.file.FileVisitOption.InnerEnum.FOLLOW_LINKS:
						fl = true;
						break;
					default:
						throw new AssertionError("Should not get here");
				}
			}
			if (maxDepth < 0)
			{
				throw new IllegalArgumentException("'maxDepth' is negative");
			}

			this.FollowLinks = fl;
			this.LinkOptions = (fl) ? new LinkOption[0] : new LinkOption[] {LinkOption.NOFOLLOW_LINKS};
			this.MaxDepth = maxDepth;
		}

		/// <summary>
		/// Returns the attributes of the given file, taking into account whether
		/// the walk is following sym links is not. The {@code canUseCached}
		/// argument determines whether this method can use cached attributes.
		/// </summary>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: private java.nio.file.attribute.BasicFileAttributes getAttributes(Path file, boolean canUseCached) throws java.io.IOException
		private BasicFileAttributes GetAttributes(Path file, bool canUseCached)
		{
			// if attributes are cached then use them if possible
			if (canUseCached && (file is BasicFileAttributesHolder) && (System.SecurityManager == null))
			{
				BasicFileAttributes cached = ((BasicFileAttributesHolder)file).get();
				if (cached != null && (!FollowLinks || !cached.SymbolicLink))
				{
					return cached;
				}
			}

			// attempt to get attributes of file. If fails and we are following
			// links then a link target might not exist so get attributes of link
			BasicFileAttributes attrs;
			try
			{
				attrs = Files.ReadAttributes(file, typeof(BasicFileAttributes), LinkOptions);
			}
			catch (IOException ioe)
			{
				if (!FollowLinks)
				{
					throw ioe;
				}

				// attempt to get attrmptes without following links
				attrs = Files.ReadAttributes(file, typeof(BasicFileAttributes), LinkOption.NOFOLLOW_LINKS);
			}
			return attrs;
		}

		/// <summary>
		/// Returns true if walking into the given directory would result in a
		/// file system loop/cycle.
		/// </summary>
		private bool WouldLoop(Path dir, Object key)
		{
			// if this directory and ancestor has a file key then we compare
			// them; otherwise we use less efficient isSameFile test.
			foreach (DirectoryNode ancestor in Stack)
			{
				Object ancestorKey = ancestor.Key();
				if (key != null && ancestorKey != null)
				{
					if (key.Equals(ancestorKey))
					{
						// cycle detected
						return true;
					}
				}
				else
				{
					try
					{
						if (Files.IsSameFile(dir, ancestor.Directory()))
						{
							// cycle detected
							return true;
						}
					}
//JAVA TO C# CONVERTER TODO TASK: There is no equivalent in C# to Java 'multi-catch' syntax:
					catch (IOException | SecurityException x)
					{
						// ignore
					}
				}
			}
			return false;
		}

		/// <summary>
		/// Visits the given file, returning the {@code Event} corresponding to that
		/// visit.
		/// 
		/// The {@code ignoreSecurityException} parameter determines whether
		/// any SecurityException should be ignored or not. If a SecurityException
		/// is thrown, and is ignored, then this method returns {@code null} to
		/// mean that there is no event corresponding to a visit to the file.
		/// 
		/// The {@code canUseCached} parameter determines whether cached attributes
		/// for the file can be used or not.
		/// </summary>
		private Event Visit(Path entry, bool ignoreSecurityException, bool canUseCached)
		{
			// need the file attributes
			BasicFileAttributes attrs;
			try
			{
				attrs = GetAttributes(entry, canUseCached);
			}
			catch (IOException ioe)
			{
				return new Event(EventType.ENTRY, entry, ioe);
			}
			catch (SecurityException se)
			{
				if (ignoreSecurityException)
				{
					return null;
				}
				throw se;
			}

			// at maximum depth or file is not a directory
			int depth = Stack.Size();
			if (depth >= MaxDepth || !attrs.Directory)
			{
				return new Event(EventType.ENTRY, entry, attrs);
			}

			// check for cycles when following links
			if (FollowLinks && WouldLoop(entry, attrs.FileKey()))
			{
				return new Event(EventType.ENTRY, entry, new FileSystemLoopException(entry.ToString()));
			}

			// file is a directory, attempt to open it
			DirectoryStream<Path> stream = null;
			try
			{
				stream = Files.NewDirectoryStream(entry);
			}
			catch (IOException ioe)
			{
				return new Event(EventType.ENTRY, entry, ioe);
			}
			catch (SecurityException se)
			{
				if (ignoreSecurityException)
				{
					return null;
				}
				throw se;
			}

			// push a directory node to the stack and return an event
			Stack.Push(new DirectoryNode(entry, attrs.FileKey(), stream));
			return new Event(EventType.START_DIRECTORY, entry, attrs);
		}


		/// <summary>
		/// Start walking from the given file.
		/// </summary>
		internal virtual Event Walk(Path file)
		{
			if (Closed)
			{
				throw new IllegalStateException("Closed");
			}

			Event ev = Visit(file, false, false); // canUseCached -  ignoreSecurityException
			Debug.Assert(ev != null);
			return ev;
		}

		/// <summary>
		/// Returns the next Event or {@code null} if there are no more events or
		/// the walker is closed.
		/// </summary>
		internal virtual Event Next()
		{
			DirectoryNode top = Stack.Peek();
			if (top == null)
			{
				return null; // stack is empty, we are done
			}

			// continue iteration of the directory at the top of the stack
			Event ev;
			do
			{
				Path entry = null;
				IOException ioe = null;

				// get next entry in the directory
				if (!top.Skipped())
				{
					IEnumerator<Path> iterator = top.Iterator();
					try
					{
//JAVA TO C# CONVERTER TODO TASK: Java iterators are only converted within the context of 'while' and 'for' loops:
						if (iterator.hasNext())
						{
//JAVA TO C# CONVERTER TODO TASK: Java iterators are only converted within the context of 'while' and 'for' loops:
							entry = iterator.next();
						}
					}
					catch (DirectoryIteratorException x)
					{
						ioe = x.InnerException;
					}
				}

				// no next entry so close and pop directory, creating corresponding event
				if (entry == null)
				{
					try
					{
						top.Stream().Close();
					}
					catch (IOException e)
					{
						if (ioe != null)
						{
							ioe = e;
						}
						else
						{
							ioe.AddSuppressed(e);
						}
					}
					Stack.Pop();
					return new Event(EventType.END_DIRECTORY, top.Directory(), ioe);
				}

				// visit the entry
				ev = Visit(entry, true, true); // canUseCached -  ignoreSecurityException

			} while (ev == null);

			return ev;
		}

		/// <summary>
		/// Pops the directory node that is the current top of the stack so that
		/// there are no more events for the directory (including no END_DIRECTORY)
		/// event. This method is a no-op if the stack is empty or the walker is
		/// closed.
		/// </summary>
		internal virtual void Pop()
		{
			if (!Stack.Empty)
			{
				DirectoryNode node = Stack.Pop();
				try
				{
					node.Stream().Close();
				}
				catch (IOException)
				{
				}
			}
		}

		/// <summary>
		/// Skips the remaining entries in the directory at the top of the stack.
		/// This method is a no-op if the stack is empty or the walker is closed.
		/// </summary>
		internal virtual void SkipRemainingSiblings()
		{
			if (!Stack.Empty)
			{
				Stack.Peek().Skip();
			}
		}

		/// <summary>
		/// Returns {@code true} if the walker is open.
		/// </summary>
		internal virtual bool Open
		{
			get
			{
				return !Closed;
			}
		}

		/// <summary>
		/// Closes/pops all directories on the stack.
		/// </summary>
		public virtual void Close()
		{
			if (!Closed)
			{
				while (!Stack.Empty)
				{
					Pop();
				}
				Closed = true;
			}
		}
	}

}
using System.Diagnostics;

/*
 * Copyright (c) 2013, Oracle and/or its affiliates. All rights reserved.
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
	/// An {@code Iterator to iterate over the nodes of a file tree.
	/// 
	/// <pre>{@code
	///     try (FileTreeIterator iterator = new FileTreeIterator(start, maxDepth, options)) {
	///         while (iterator.hasNext()) {
	///             Event ev = iterator.next();
	///             Path path = ev.file();
	///             BasicFileAttributes attrs = ev.attributes();
	///         }
	///     }
	/// }</pre>
	/// </summary>

	internal class FileTreeIterator : Iterator<Event>, Closeable
	{
		private readonly FileTreeWalker Walker;
		private Event Next_Renamed;

		/// <summary>
		/// Creates a new iterator to walk the file tree starting at the given file.
		/// </summary>
		/// <exception cref="IllegalArgumentException">
		///          if {@code maxDepth} is negative </exception>
		/// <exception cref="IOException">
		///          if an I/O errors occurs opening the starting file </exception>
		/// <exception cref="SecurityException">
		///          if the security manager denies access to the starting file </exception>
		/// <exception cref="NullPointerException">
		///          if {@code start} or {@code options} is {@ocde null} or
		///          the options array contains a {@code null} element </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: FileTreeIterator(Path start, int maxDepth, FileVisitOption... options) throws java.io.IOException
		internal FileTreeIterator(Path start, int maxDepth, params FileVisitOption[] options)
		{
			this.Walker = new FileTreeWalker(Arrays.AsList(options), maxDepth);
			this.Next_Renamed = Walker.Walk(start);
			Debug.Assert(Next_Renamed.Type() == FileTreeWalker.EventType.ENTRY || Next_Renamed.Type() == FileTreeWalker.EventType.START_DIRECTORY);

			// IOException if there a problem accessing the starting file
			IOException ioe = Next_Renamed.IoeException();
			if (ioe != null)
			{
				throw ioe;
			}
		}

		private void FetchNextIfNeeded()
		{
			if (Next_Renamed == null)
			{
				FileTreeWalker.Event ev = Walker.Next();
				while (ev != null)
				{
					IOException ioe = ev.IoeException();
					if (ioe != null)
					{
						throw new UncheckedIOException(ioe);
					}

					// END_DIRECTORY events are ignored
					if (ev.Type() != FileTreeWalker.EventType.END_DIRECTORY)
					{
						Next_Renamed = ev;
						return;
					}
					ev = Walker.Next();
				}
			}
		}

		public override bool HasNext()
		{
			if (!Walker.Open)
			{
				throw new IllegalStateException();
			}
			FetchNextIfNeeded();
			return Next_Renamed != null;
		}

		public override Event Next()
		{
			if (!Walker.Open)
			{
				throw new IllegalStateException();
			}
			FetchNextIfNeeded();
			if (Next_Renamed == null)
			{
				throw new NoSuchElementException();
			}
			Event result = Next_Renamed;
			Next_Renamed = null;
			return result;
		}

		public virtual void Close()
		{
			Walker.Close();
		}
	}

}
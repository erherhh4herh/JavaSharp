using System.Collections;

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

namespace java.beans
{


	/// <summary>
	/// Hash table based mapping, which uses weak references to store keys
	/// and reference-equality in place of object-equality to compare them.
	/// An entry will automatically be removed when its key is no longer
	/// in ordinary use.  Both null values and the null key are supported.
	/// This class does not require additional synchronization.
	/// A thread-safety is provided by a fragile combination
	/// of synchronized blocks and volatile fields.
	/// Be very careful during editing!
	/// </summary>
	/// <seealso cref= java.util.IdentityHashMap </seealso>
	/// <seealso cref= java.util.WeakHashMap </seealso>
	internal abstract class WeakIdentityMap<T>
	{
		private bool InstanceFieldsInitialized = false;

		public WeakIdentityMap()
		{
			if (!InstanceFieldsInitialized)
			{
				InitializeInstanceFields();
				InstanceFieldsInitialized = true;
			}
		}

		private void InitializeInstanceFields()
		{
			Table = NewTable(1 << 3);
		}


		private static readonly int MAXIMUM_CAPACITY = 1 << 30; // it MUST be a power of two
		private static readonly Object NULL = new Object(); // special object for null key

		private readonly ReferenceQueue<Object> Queue = new ReferenceQueue<Object>();

		private volatile Entry<T>[] Table; // table's length MUST be a power of two
		private int Threshold = 6; // the next size value at which to resize
		private int Size = 0; // the number of key-value mappings

		public virtual T Get(Object key)
		{
			RemoveStaleEntries();
			if (key == null)
			{
				key = NULL;
			}
			int hash = key.HashCode();
			Entry<T>[] table = this.Table;
			// unsynchronized search improves performance
			// the null value does not mean that there are no needed entry
			int index = GetIndex(table, hash);
			for (Entry<T> entry = table[index]; entry != null; entry = entry.Next)
			{
				if (entry.IsMatched(key, hash))
				{
					return entry.Value;
				}
			}
			lock (NULL)
			{
				// synchronized search improves stability
				// we must create and add new value if there are no needed entry
				index = GetIndex(this.Table, hash);
				for (Entry<T> entry = this.Table[index]; entry != null; entry = entry.Next)
				{
					if (entry.IsMatched(key, hash))
					{
						return entry.Value;
					}
				}
				T value = Create(key);
				this.Table[index] = new Entry<T>(key, hash, value, this.Queue, this.Table[index]);
				if (++this.Size >= this.Threshold)
				{
					if (this.Table.Length == MAXIMUM_CAPACITY)
					{
						this.Threshold = Integer.MaxValue;
					}
					else
					{
						RemoveStaleEntries();
						table = NewTable(this.Table.Length * 2);
						Transfer(this.Table, table);
						// If ignoring null elements and processing ref queue caused massive
						// shrinkage, then restore old table.  This should be rare, but avoids
						// unbounded expansion of garbage-filled tables.
						if (this.Size >= this.Threshold / 2)
						{
							this.Table = table;
							this.Threshold *= 2;
						}
						else
						{
							Transfer(table, this.Table);
						}
					}
				}
				return value;
			}
		}

		protected internal abstract T Create(Object key);

		private void RemoveStaleEntries()
		{
			Object @ref = this.Queue.poll();
			if (@ref != null)
			{
				lock (NULL)
				{
					do
					{
//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @SuppressWarnings("unchecked") Entry<T> entry = (Entry<T>) ref;
						Entry<T> entry = (Entry<T>) @ref;
						int index = GetIndex(this.Table, entry.Hash);

						Entry<T> prev = this.Table[index];
						Entry<T> current = prev;
						while (current != null)
						{
							Entry<T> next = current.Next;
							if (current == entry)
							{
								if (prev == entry)
								{
									this.Table[index] = next;
								}
								else
								{
									prev.Next = next;
								}
								entry.Value = null; // Help GC
								entry.Next = null; // Help GC
								this.Size--;
								break;
							}
							prev = current;
							current = next;
						}
						@ref = this.Queue.poll();
					} while (@ref != null);
				}
			}
		}

		private void Transfer(Entry<T>[] oldTable, Entry<T>[] newTable)
		{
			for (int i = 0; i < oldTable.Length; i++)
			{
				Entry<T> entry = oldTable[i];
				oldTable[i] = null;
				while (entry != null)
				{
					Entry<T> next = entry.Next;
					Object key = entry.get();
					if (key == null)
					{
						entry.Value = null; // Help GC
						entry.Next = null; // Help GC
						this.Size--;
					}
					else
					{
						int index = GetIndex(newTable, entry.Hash);
						entry.Next = newTable[index];
						newTable[index] = entry;
					}
					entry = next;
				}
			}
		}


//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @SuppressWarnings("unchecked") private Entry<T>[] newTable(int length)
		private Entry<T>[] NewTable(int length)
		{
//JAVA TO C# CONVERTER TODO TASK: Java wildcard generics are not converted to .NET:
//ORIGINAL LINE: return (Entry<T>[]) new Entry<?>[length];
			return (Entry<T>[]) new Entry<?>[length];
		}

		private static int getIndex<T1>(Entry<T1>[] table, int hash)
		{
			return hash & (table.Length - 1);
		}

		private class Entry<T> : WeakReference<Object>
		{
			internal readonly int Hash;
			internal volatile T Value;
			internal volatile Entry<T> Next;

			internal Entry(Object key, int hash, T value, ReferenceQueue<Object> queue, Entry<T> next) : base(key, queue)
			{
				this.Hash = hash;
				this.Value = value;
				this.Next = next;
			}

			internal virtual bool IsMatched(Object key, int hash)
			{
				return (this.Hash == hash) && (key == get());
			}
		}
	}

}
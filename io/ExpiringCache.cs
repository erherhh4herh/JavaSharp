using System.Collections.Generic;

/*
 * Copyright (c) 2002, 2011, Oracle and/or its affiliates. All rights reserved.
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
 */

namespace java.io
{


	internal class ExpiringCache
	{
		private long MillisUntilExpiration;
		private IDictionary<String, Entry> Map;
		// Clear out old entries every few queries
		private int QueryCount;
		private int QueryOverflow = 300;
		private int MAX_ENTRIES = 200;

		internal class Entry
		{
			internal long Timestamp_Renamed;
			internal String Val_Renamed;

			internal Entry(long timestamp, String val)
			{
				this.Timestamp_Renamed = timestamp;
				this.Val_Renamed = val;
			}

			internal virtual long Timestamp()
			{
				return Timestamp_Renamed;
			}
			internal virtual long Timestamp
			{
				set
				{
					this.Timestamp_Renamed = value;
				}
			}

			internal virtual String Val()
			{
				return Val_Renamed;
			}
			internal virtual String Val
			{
				set
				{
					this.Val_Renamed = value;
				}
			}
		}

		internal ExpiringCache() : this(30000)
		{
		}

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @SuppressWarnings("serial") ExpiringCache(long millisUntilExpiration)
		internal ExpiringCache(long millisUntilExpiration)
		{
			this.MillisUntilExpiration = millisUntilExpiration;
			Map = new LinkedHashMapAnonymousInnerClassHelper(this);
		}

		private class LinkedHashMapAnonymousInnerClassHelper : LinkedHashMap<String, Entry>
		{
			private readonly ExpiringCache OuterInstance;

			public LinkedHashMapAnonymousInnerClassHelper(ExpiringCache outerInstance)
			{
				this.OuterInstance = outerInstance;
			}

			protected internal virtual bool RemoveEldestEntry(java.util.Map_Entry<String, Entry> eldest)
			{
			  return size() > OuterInstance.MAX_ENTRIES;
			}
		}

		internal virtual String Get(String key)
		{
			lock (this)
			{
				if (++QueryCount >= QueryOverflow)
				{
					Cleanup();
				}
				Entry entry = EntryFor(key);
				if (entry != null)
				{
					return entry.Val();
				}
				return null;
			}
		}

		internal virtual void Put(String key, String val)
		{
			lock (this)
			{
				if (++QueryCount >= QueryOverflow)
				{
					Cleanup();
				}
				Entry entry = EntryFor(key);
				if (entry != null)
				{
					entry.Timestamp = DateTimeHelperClass.CurrentUnixTimeMillis();
					entry.Val = val;
				}
				else
				{
					Map[key] = new Entry(DateTimeHelperClass.CurrentUnixTimeMillis(), val);
				}
			}
		}

		internal virtual void Clear()
		{
			lock (this)
			{
				Map.Clear();
			}
		}

		private Entry EntryFor(String key)
		{
			Entry entry = Map[key];
			if (entry != null)
			{
				long delta = DateTimeHelperClass.CurrentUnixTimeMillis() - entry.Timestamp();
				if (delta < 0 || delta >= MillisUntilExpiration)
				{
					Map.Remove(key);
					entry = null;
				}
			}
			return entry;
		}

		private void Cleanup()
		{
			IDictionary<String, Entry>.KeyCollection keySet = Map.Keys;
			// Avoid ConcurrentModificationExceptions
			String[] keys = new String[keySet.size()];
			int i = 0;
			foreach (String key in keySet)
			{
				keys[i++] = key;
			}
			for (int j = 0; j < keys.Length; j++)
			{
				EntryFor(keys[j]);
			}
			QueryCount = 0;
		}
	}

}
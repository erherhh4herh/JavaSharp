using System;
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

/* We use APIs that access a so-called Windows "Environment Block",
 * which looks like an array of jchars like this:
 *
 * FOO=BAR\u0000 ... GORP=QUUX\u0000\u0000
 *
 * This data structure has a number of peculiarities we must contend with:
 * (see: http://windowssdk.msdn.microsoft.com/en-us/library/ms682009.aspx)
 * - The NUL jchar separators, and a double NUL jchar terminator.
 *   It appears that the Windows implementation requires double NUL
 *   termination even if the environment is empty.  We should always
 *   generate environments with double NUL termination, while accepting
 *   empty environments consisting of a single NUL.
 * - on Windows9x, this is actually an array of 8-bit chars, not jchars,
 *   encoded in the system default encoding.
 * - The block must be sorted by Unicode value, case-insensitively,
 *   as if folded to upper case.
 * - There are magic environment variables maintained by Windows
 *   that start with a `=' (!) character.  These are used for
 *   Windows drive current directory (e.g. "=C:=C:\WINNT") or the
 *   exit code of the last command (e.g. "=ExitCode=0000001").
 *
 * Since Java and non-9x Windows speak the same character set, and
 * even the same encoding, we don't have to deal with unreliable
 * conversion to byte streams.  Just add a few NUL terminators.
 *
 * System.getenv(String) is case-insensitive, while System.getenv()
 * returns a map that is case-sensitive, which is consistent with
 * native Windows APIs.
 *
 * The non-private methods in this class are not for general use even
 * within this package.  Instead, they are the system-dependent parts
 * of the system-independent method of the same name.  Don't even
 * think of using this class unless your method's name appears below.
 *
 * @author Martin Buchholz
 * @since 1.5
 */

namespace java.lang
{


	internal sealed class ProcessEnvironment : HashMap<String, String>
	{

		private const long SerialVersionUID = -8017839552603542824L;

		private static String ValidateName(String name)
		{
			// An initial `=' indicates a magic Windows variable name -- OK
			if (name.IndexOf('=', 1) != -1 || name.IndexOf('\u0000') != -1)
			{
				throw new IllegalArgumentException("Invalid environment variable name: \"" + name + "\"");
			}
			return name;
		}

		private static String ValidateValue(String value)
		{
			if (value.IndexOf('\u0000') != -1)
			{
				throw new IllegalArgumentException("Invalid environment variable value: \"" + value + "\"");
			}
			return value;
		}

		private static String NonNullString(Object o)
		{
			if (o == java.util.Map_Fields.Null)
			{
				throw new NullPointerException();
			}
			return (String) o;
		}

		public String Put(String key, String value)
		{
			return base.Put(ValidateName(key), ValidateValue(value));
		}

		public String Get(Object key)
		{
			return base.Get(NonNullString(key));
		}

		public bool ContainsKey(Object key)
		{
			return base.ContainsKey(NonNullString(key));
		}

		public bool ContainsValue(Object value)
		{
			return base.ContainsValue(NonNullString(value));
		}

		public String Remove(Object key)
		{
			return base.Remove(NonNullString(key));
		}

		private class CheckedEntry : Map_Entry<String, String>
		{
			internal readonly Map_Entry<String, String> e;
			public CheckedEntry(Map_Entry<String, String> e)
			{
				this.e = e;
			}
			public virtual String Key
			{
				get
				{
					return e.Key;
				}
			}
			public virtual String Value
			{
				get
				{
					return e.Value;
				}
			}
			public virtual String SetValue(String value)
			{
				return e.setValue(ValidateValue(value));
			}
			public override String ToString()
			{
				return Key + "=" + Value;
			}
			public override bool Equals(Object o)
			{
				return e.Equals(o);
			}
			public override int HashCode()
			{
				return e.HashCode();
			}
		}

		private class CheckedEntrySet : AbstractSet<Map_Entry<String, String>>
		{
			internal readonly Set<Map_Entry<String, String>> s;
			public CheckedEntrySet(Set<Map_Entry<String, String>> s)
			{
				this.s = s;
			}
			public virtual int Size()
			{
				return s.Count;
			}
			public virtual bool Empty
			{
				get
				{
					return s.Count == 0;
				}
			}
			public virtual void Clear()
			{
				s.Clear();
			}
			public virtual Iterator<Map_Entry<String, String>> Iterator()
			{
				return new IteratorAnonymousInnerClassHelper(this);
			}

			private class IteratorAnonymousInnerClassHelper : Iterator<Map_Entry<String, String>>
			{
				private readonly CheckedEntrySet OuterInstance;

				public IteratorAnonymousInnerClassHelper(CheckedEntrySet outerInstance)
				{
					this.OuterInstance = outerInstance;
					i = outerInstance.s.Iterator();
				}

				internal Iterator<Map_Entry<String, String>> i;
				public virtual bool HasNext()
				{
					return i.hasNext();
				}
				public virtual Map_Entry<String, String> Next()
				{
					return new CheckedEntry(i.next());
				}
				public virtual void Remove()
				{
					i.remove();
				}
			}
			internal static Map_Entry<String, String> CheckedEntry(Object o)
			{
//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @SuppressWarnings("unchecked") Map_Entry<String,String> e = (Map_Entry<String,String>) o;
				Map_Entry<String, String> e = (Map_Entry<String, String>) o;
				NonNullString(e.Key);
				NonNullString(e.Value);
				return e;
			}
			public virtual bool Contains(Object o)
			{
				return s.Contains(CheckedEntry(o));
			}
			public virtual bool Remove(Object o)
			{
				return s.Remove(CheckedEntry(o));
			}
		}

		private class CheckedValues : AbstractCollection<String>
		{
			internal readonly Collection<String> c;
			public CheckedValues(Collection<String> c)
			{
				this.c = c;
			}
			public virtual int Size()
			{
				return c.Size();
			}
			public virtual bool Empty
			{
				get
				{
					return c.Empty;
				}
			}
			public virtual void Clear()
			{
				c.Clear();
			}
			public virtual Iterator<String> Iterator()
			{
				return c.Iterator();
			}
			public virtual bool Contains(Object o)
			{
				return c.Contains(NonNullString(o));
			}
			public virtual bool Remove(Object o)
			{
				return c.Remove(NonNullString(o));
			}
		}

		private class CheckedKeySet : AbstractSet<String>
		{
			internal readonly Set<String> s;
			public CheckedKeySet(Set<String> s)
			{
				this.s = s;
			}
			public virtual int Size()
			{
				return s.Count;
			}
			public virtual bool Empty
			{
				get
				{
					return s.Count == 0;
				}
			}
			public virtual void Clear()
			{
				s.Clear();
			}
			public virtual Iterator<String> Iterator()
			{
				return s.Iterator();
			}
			public virtual bool Contains(Object o)
			{
				return s.Contains(NonNullString(o));
			}
			public virtual bool Remove(Object o)
			{
				return s.Remove(NonNullString(o));
			}
		}

		public Set<String> KeySet()
		{
			return new CheckedKeySet(base.KeySet());
		}

		public Collection<String> Values()
		{
			return new CheckedValues(base.Values());
		}

		public Set<Map_Entry<String, String>> EntrySet()
		{
			return new CheckedEntrySet(base.EntrySet());
		}


		private sealed class NameComparator : Comparator<String>
		{
			public int Compare(String s1, String s2)
			{
				// We can't use String.compareToIgnoreCase since it
				// canonicalizes to lower case, while Windows
				// canonicalizes to upper case!  For example, "_" should
				// sort *after* "Z", not before.
				int n1 = s1.Length();
				int n2 = s2.Length();
				int min = System.Math.Min(n1, n2);
				for (int i = 0; i < min; i++)
				{
					char c1 = s1.CharAt(i);
					char c2 = s2.CharAt(i);
					if (c1 != c2)
					{
						c1 = char.ToUpper(c1);
						c2 = char.ToUpper(c2);
						if (c1 != c2)
						{
							// No overflow because of numeric promotion
							return c1 - c2;
						}
					}
				}
				return n1 - n2;
			}
		}

		private sealed class EntryComparator : Comparator<Map_Entry<String, String>>
		{
			public int Compare(Map_Entry<String, String> e1, Map_Entry<String, String> e2)
			{
				return NameComparator.Compare(e1.Key, e2.Key);
			}
		}

		// Allow `=' as first char in name, e.g. =C:=C:\DIR
		internal const int MIN_NAME_LENGTH = 1;

		private static readonly NameComparator NameComparator;
		private static readonly EntryComparator EntryComparator;
		private static readonly ProcessEnvironment TheEnvironment;
		private static readonly Map<String, String> TheUnmodifiableEnvironment;
		private static readonly Map<String, String> TheCaseInsensitiveEnvironment;

		static ProcessEnvironment()
		{
			NameComparator = new NameComparator();
			EntryComparator = new EntryComparator();
			TheEnvironment = new ProcessEnvironment();
			TheUnmodifiableEnvironment = Collections.UnmodifiableMap(TheEnvironment);

			String envblock = environmentBlock();
			int beg, end, eql;
			for (beg = 0; ((end = envblock.IndexOf('\u0000', beg)) != -1 && (eql = envblock.IndexOf('=', beg + 1)) != -1); beg = end + 1)
			{
				  // An initial `=' indicates a magic Windows variable name -- OK
				// Ignore corrupted environment strings.
				if (eql < end)
				{
					TheEnvironment.Put(envblock.Substring(beg, eql - beg), StringHelperClass.SubstringSpecial(envblock, eql + 1,end));
				}
			}

			TheCaseInsensitiveEnvironment = new TreeMap<>(NameComparator);
			TheCaseInsensitiveEnvironment.PutAll(TheEnvironment);
		}

		private ProcessEnvironment() : base()
		{
		}

		private ProcessEnvironment(int capacity) : base(capacity)
		{
		}

		// Only for use by System.getenv(String)
		internal static String Getenv(String name)
		{
			// The original implementation used a native call to _wgetenv,
			// but it turns out that _wgetenv is only consistent with
			// GetEnvironmentStringsW (for non-ASCII) if `wmain' is used
			// instead of `main', even in a process created using
			// CREATE_UNICODE_ENVIRONMENT.  Instead we perform the
			// case-insensitive comparison ourselves.  At least this
			// guarantees that System.getenv().get(String) will be
			// consistent with System.getenv(String).
			return TheCaseInsensitiveEnvironment.Get(name);
		}

		// Only for use by System.getenv()
		internal static Map<String, String> Getenv()
		{
			return TheUnmodifiableEnvironment;
		}

		// Only for use by ProcessBuilder.environment()
//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @SuppressWarnings("unchecked") static Map<String,String> environment()
		internal static Map<String, String> Environment()
		{
			return (Map<String, String>) TheEnvironment.Clone();
		}

		// Only for use by ProcessBuilder.environment(String[] envp)
		internal static Map<String, String> EmptyEnvironment(int capacity)
		{
			return new ProcessEnvironment(capacity);
		}

//JAVA TO C# CONVERTER TODO TASK: Replace 'unknown' with the appropriate dll name:
		[DllImport("unknown")]
		private static extern String environmentBlock();

		// Only for use by ProcessImpl.start()
		internal String ToEnvironmentBlock()
		{
			// Sort Unicode-case-insensitively by name
			List<Map_Entry<String, String>> list = new List<Map_Entry<String, String>>(this.EntrySet());
			list.Sort(EntryComparator);

			StringBuilder sb = new StringBuilder(this.Size() * 30);
			int cmp = -1;

			// Some versions of MSVCRT.DLL require SystemRoot to be set.
			// So, we make sure that it is always set, even if not provided
			// by the caller.
			const String SYSTEMROOT = "SystemRoot";

			foreach (Map_Entry<String, String> e in list)
			{
				String key = e.Key;
				String value = e.Value;
				if (cmp < 0 && (cmp = NameComparator.Compare(key, SYSTEMROOT)) > 0)
				{
					// Not set, so add it here
					AddToEnvIfSet(sb, SYSTEMROOT);
				}
				AddToEnv(sb, key, value);
			}
			if (cmp < 0)
			{
				// Got to end of list and still not found
				AddToEnvIfSet(sb, SYSTEMROOT);
			}
			if (sb.Length() == 0)
			{
				// Environment was empty and SystemRoot not set in parent
				sb.Append('\u0000');
			}
			// Block is double NUL terminated
			sb.Append('\u0000');
			return sb.ToString();
		}

		// add the environment variable to the child, if it exists in parent
		private static void AddToEnvIfSet(StringBuilder sb, String name)
		{
			String s = Getenv(name);
			if (s != java.util.Map_Fields.Null)
			{
				AddToEnv(sb, name, s);
			}
		}

		private static void AddToEnv(StringBuilder sb, String name, String val)
		{
			sb.Append(name).Append('=').Append(val).Append('\u0000');
		}

		internal static String ToEnvironmentBlock(Map<String, String> map)
		{
			return map == java.util.Map_Fields.Null ? java.util.Map_Fields.Null : ((ProcessEnvironment)map).ToEnvironmentBlock();
		}
	}

}
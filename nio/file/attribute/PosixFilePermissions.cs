using System.Collections.Generic;

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

namespace java.nio.file.attribute
{


	/// <summary>
	/// This class consists exclusively of static methods that operate on sets of
	/// <seealso cref="PosixFilePermission"/> objects.
	/// 
	/// @since 1.7
	/// </summary>

	public sealed class PosixFilePermissions
	{
		private PosixFilePermissions()
		{
		}

		// Write string representation of permission bits to {@code sb}.
		private static void WriteBits(StringBuilder sb, bool r, bool w, bool x)
		{
			if (r)
			{
				sb.Append('r');
			}
			else
			{
				sb.Append('-');
			}
			if (w)
			{
				sb.Append('w');
			}
			else
			{
				sb.Append('-');
			}
			if (x)
			{
				sb.Append('x');
			}
			else
			{
				sb.Append('-');
			}
		}

		/// <summary>
		/// Returns the {@code String} representation of a set of permissions. It
		/// is guaranteed that the returned {@code String} can be parsed by the
		/// <seealso cref="#fromString"/> method.
		/// 
		/// <para> If the set contains {@code null} or elements that are not of type
		/// {@code PosixFilePermission} then these elements are ignored.
		/// 
		/// </para>
		/// </summary>
		/// <param name="perms">
		///          the set of permissions
		/// </param>
		/// <returns>  the string representation of the permission set </returns>
		public static String ToString(Set<PosixFilePermission> perms)
		{
			StringBuilder sb = new StringBuilder(9);
			WriteBits(sb, perms.Contains(OWNER_READ), perms.Contains(OWNER_WRITE), perms.Contains(OWNER_EXECUTE));
			WriteBits(sb, perms.Contains(GROUP_READ), perms.Contains(GROUP_WRITE), perms.Contains(GROUP_EXECUTE));
			WriteBits(sb, perms.Contains(OTHERS_READ), perms.Contains(OTHERS_WRITE), perms.Contains(OTHERS_EXECUTE));
			return sb.ToString();
		}

		private static bool IsSet(char c, char setValue)
		{
			if (c == setValue)
			{
				return true;
			}
			if (c == '-')
			{
				return false;
			}
			throw new IllegalArgumentException("Invalid mode");
		}
		private static bool IsR(char c)
		{
			return IsSet(c, 'r');
		}
		private static bool IsW(char c)
		{
			return IsSet(c, 'w');
		}
		private static bool IsX(char c)
		{
			return IsSet(c, 'x');
		}

		/// <summary>
		/// Returns the set of permissions corresponding to a given {@code String}
		/// representation.
		/// 
		/// <para> The {@code perms} parameter is a {@code String} representing the
		/// permissions. It has 9 characters that are interpreted as three sets of
		/// three. The first set refers to the owner's permissions; the next to the
		/// group permissions and the last to others. Within each set, the first
		/// character is {@code 'r'} to indicate permission to read, the second
		/// character is {@code 'w'} to indicate permission to write, and the third
		/// character is {@code 'x'} for execute permission. Where a permission is
		/// not set then the corresponding character is set to {@code '-'}.
		/// 
		/// </para>
		/// <para> <b>Usage Example:</b>
		/// Suppose we require the set of permissions that indicate the owner has read,
		/// write, and execute permissions, the group has read and execute permissions
		/// and others have none.
		/// <pre>
		///   Set&lt;PosixFilePermission&gt; perms = PosixFilePermissions.fromString("rwxr-x---");
		/// </pre>
		/// 
		/// </para>
		/// </summary>
		/// <param name="perms">
		///          string representing a set of permissions
		/// </param>
		/// <returns>  the resulting set of permissions
		/// </returns>
		/// <exception cref="IllegalArgumentException">
		///          if the string cannot be converted to a set of permissions
		/// </exception>
		/// <seealso cref= #toString(Set) </seealso>
		public static Set<PosixFilePermission> FromString(String perms)
		{
			if (perms.Length() != 9)
			{
				throw new IllegalArgumentException("Invalid mode");
			}
			Set<PosixFilePermission> result = EnumSet.NoneOf(typeof(PosixFilePermission));
			if (IsR(perms.CharAt(0)))
			{
				result.Add(OWNER_READ);
			}
			if (IsW(perms.CharAt(1)))
			{
				result.Add(OWNER_WRITE);
			}
			if (IsX(perms.CharAt(2)))
			{
				result.Add(OWNER_EXECUTE);
			}
			if (IsR(perms.CharAt(3)))
			{
				result.Add(GROUP_READ);
			}
			if (IsW(perms.CharAt(4)))
			{
				result.Add(GROUP_WRITE);
			}
			if (IsX(perms.CharAt(5)))
			{
				result.Add(GROUP_EXECUTE);
			}
			if (IsR(perms.CharAt(6)))
			{
				result.Add(OTHERS_READ);
			}
			if (IsW(perms.CharAt(7)))
			{
				result.Add(OTHERS_WRITE);
			}
			if (IsX(perms.CharAt(8)))
			{
				result.Add(OTHERS_EXECUTE);
			}
			return result;
		}

		/// <summary>
		/// Creates a <seealso cref="FileAttribute"/>, encapsulating a copy of the given file
		/// permissions, suitable for passing to the {@link java.nio.file.Files#createFile
		/// createFile} or <seealso cref="java.nio.file.Files#createDirectory createDirectory"/>
		/// methods.
		/// </summary>
		/// <param name="perms">
		///          the set of permissions
		/// </param>
		/// <returns>  an attribute encapsulating the given file permissions with
		///          <seealso cref="FileAttribute#name name"/> {@code "posix:permissions"}
		/// </returns>
		/// <exception cref="ClassCastException">
		///          if the set contains elements that are not of type {@code
		///          PosixFilePermission} </exception>
		public static FileAttribute<Set<PosixFilePermission>> AsFileAttribute(Set<PosixFilePermission> perms)
		{
			// copy set and check for nulls (CCE will be thrown if an element is not
			// a PosixFilePermission)
			perms = new HashSet<PosixFilePermission>(perms);
			foreach (PosixFilePermission p in perms)
			{
				if (p == null)
				{
					throw new NullPointerException();
				}
			}
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final Set<PosixFilePermission> value = perms;
			Set<PosixFilePermission> value = perms;
			return new FileAttributeAnonymousInnerClassHelper(value);
		}

		private class FileAttributeAnonymousInnerClassHelper : FileAttribute<Set<PosixFilePermission>>
		{
			private java.util.Set<PosixFilePermission> Value;

			public FileAttributeAnonymousInnerClassHelper(java.util.Set<PosixFilePermission> value)
			{
				this.Value = value;
			}

			public virtual String Name()
			{
				return "posix:permissions";
			}
			public virtual Set<PosixFilePermission> Value()
			{
				return Collections.UnmodifiableSet(Value);
			}
		}
	}

}
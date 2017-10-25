using System;

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
namespace java.lang.invoke
{

	using PlatformLogger = sun.util.logging.PlatformLogger;


	/// <summary>
	/// Helper class used by InnerClassLambdaMetafactory to log generated classes
	/// 
	/// @implNote
	/// <para> Because this class is called by LambdaMetafactory, make use
	/// of lambda lead to recursive calls cause stack overflow.
	/// </para>
	/// </summary>
	internal sealed class ProxyClassesDumper
	{
		private static readonly char[] HEX = new char[] {'0', '1', '2', '3', '4', '5', '6', '7', '8', '9', 'A', 'B', 'C', 'D', 'E', 'F'};
		private static readonly char[] BAD_CHARS = new char[] {'\\', ':', '*', '?', '"', '<', '>', '|'};
		private static readonly String[] REPLACEMENT = new String[] {"%5C", "%3A", "%2A", "%3F", "%22", "%3C", "%3E", "%7C"};

		private readonly Path DumpDir;

		public static ProxyClassesDumper GetInstance(String path)
		{
			if (null == path)
			{
				return null;
			}
			try
			{
				path = path.Trim();
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final java.nio.file.Path dir = java.nio.file.Paths.get(path.length() == 0 ? "." : path);
				Path dir = Paths.Get(path.Length() == 0 ? "." : path);
				AccessController.DoPrivileged(new PrivilegedActionAnonymousInnerClassHelper(dir), null, new FilePermission("<<ALL FILES>>", "read, write"));
				return new ProxyClassesDumper(dir);
			}
			catch (InvalidPathException ex)
			{
//JAVA TO C# CONVERTER WARNING: The .NET Type.FullName property will not always yield results identical to the Java Class.getName method:
				PlatformLogger.getLogger(typeof(ProxyClassesDumper).FullName).warning("Path " + path + " is not valid - dumping disabled", ex);
			}
			catch (IllegalArgumentException iae)
			{
//JAVA TO C# CONVERTER WARNING: The .NET Type.FullName property will not always yield results identical to the Java Class.getName method:
				PlatformLogger.getLogger(typeof(ProxyClassesDumper).FullName).warning(iae.Message + " - dumping disabled");
			}
			return null;
		}

		private class PrivilegedActionAnonymousInnerClassHelper : PrivilegedAction<Void>
		{
			private Path Dir;

			public PrivilegedActionAnonymousInnerClassHelper(Path dir)
			{
				this.Dir = dir;
			}

			public virtual Void Run()
			{
				ValidateDumpDir(Dir);
				return null;
			}
		}

		private ProxyClassesDumper(Path path)
		{
			DumpDir = Objects.RequireNonNull(path);
		}

		private static void ValidateDumpDir(Path path)
		{
			if (!Files.exists(path))
			{
				throw new IllegalArgumentException("Directory " + path + " does not exist");
			}
			else if (!Files.isDirectory(path))
			{
				throw new IllegalArgumentException("Path " + path + " is not a directory");
			}
			else if (!Files.IsWritable(path))
			{
				throw new IllegalArgumentException("Directory " + path + " is not writable");
			}
		}

		public static String EncodeForFilename(String className)
		{
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final int len = className.length();
			int len = className.Length();
			StringBuilder sb = new StringBuilder(len);

			for (int i = 0; i < len; i++)
			{
				char c = className.CharAt(i);
				// control characters
				if (c <= 31)
				{
					sb.Append('%');
					sb.Append(HEX[c >> 4 & 0x0F]);
					sb.Append(HEX[c & 0x0F]);
				}
				else
				{
					int j = 0;
					for (; j < BAD_CHARS.Length; j++)
					{
						if (c == BAD_CHARS[j])
						{
							sb.Append(REPLACEMENT[j]);
							break;
						}
					}
					if (j >= BAD_CHARS.Length)
					{
						sb.Append(c);
					}
				}
			}

			return sb.ToString();
		}

//JAVA TO C# CONVERTER WARNING: 'final' parameters are not allowed in .NET:
//ORIGINAL LINE: public void dumpClass(String className, final byte[] classBytes)
		public void DumpClass(String className, sbyte[] classBytes)
		{
			Path file;
			try
			{
				file = DumpDir.Resolve(EncodeForFilename(className) + ".class");
			}
			catch (InvalidPathException)
			{
//JAVA TO C# CONVERTER WARNING: The .NET Type.FullName property will not always yield results identical to the Java Class.getName method:
				PlatformLogger.getLogger(typeof(ProxyClassesDumper).FullName).warning("Invalid path for class " + className);
				return;
			}

			try
			{
				Path dir = file.Parent;
				Files.createDirectories(dir);
				Files.write(file, classBytes);
			}
			catch (Exception)
			{
//JAVA TO C# CONVERTER WARNING: The .NET Type.FullName property will not always yield results identical to the Java Class.getName method:
				PlatformLogger.getLogger(typeof(ProxyClassesDumper).FullName).warning("Exception writing to path at " + file.ToString());
				// simply don't care if this operation failed
			}
		}
	}

}
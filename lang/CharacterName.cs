using System;

/*
 * Copyright (c) 2010, 2011, Oracle and/or its affiliates. All rights reserved.
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

namespace java.lang
{


	internal class CharacterName
	{

		private static SoftReference<sbyte[]> RefStrPool;
		private static int[][] Lookup;

		private static sbyte[] InitNamePool()
		{
			lock (typeof(CharacterName))
			{
				sbyte[] strPool = null;
				if (RefStrPool != null && (strPool = RefStrPool.get()) != null)
				{
					return strPool;
				}
				DataInputStream dis = null;
				try
				{
					dis = new DataInputStream(new InflaterInputStream(AccessController.doPrivileged(new PrivilegedActionAnonymousInnerClassHelper())));
        
					Lookup = new int[(Character.MAX_CODE_POINT + 1) >> 8][];
					int total = dis.ReadInt();
					int cpEnd = dis.ReadInt();
					sbyte[] ba = new sbyte[cpEnd];
					dis.ReadFully(ba);
        
					int nameOff = 0;
					int cpOff = 0;
					int cp = 0;
					do
					{
						int len = ba[cpOff++] & 0xff;
						if (len == 0)
						{
							len = ba[cpOff++] & 0xff;
							// always big-endian
							cp = ((ba[cpOff++] & 0xff) << 16) | ((ba[cpOff++] & 0xff) << 8) | ((ba[cpOff++] & 0xff));
						}
						else
						{
							cp++;
						}
						int hi = cp >> 8;
						if (Lookup[hi] == null)
						{
							Lookup[hi] = new int[0x100];
						}
						Lookup[hi][cp & 0xff] = (nameOff << 8) | len;
						nameOff += len;
					} while (cpOff < cpEnd);
					strPool = new sbyte[total - cpEnd];
					dis.ReadFully(strPool);
					RefStrPool = new SoftReference<>(strPool);
				}
				catch (Exception x)
				{
					throw new InternalError(x.Message, x);
				}
				finally
				{
					try
					{
						if (dis != null)
						{
							dis.Close();
						}
					}
					catch (Exception)
					{
					}
				}
				return strPool;
			}
		}

		private class PrivilegedActionAnonymousInnerClassHelper : PrivilegedAction<InputStream>
		{
			public PrivilegedActionAnonymousInnerClassHelper()
			{
			}

			public virtual InputStream Run()
			{
				return this.GetType().getResourceAsStream("uniName.dat");
			}
		}

		public static String Get(int cp)
		{
			sbyte[] strPool = null;
			if (RefStrPool == null || (strPool = RefStrPool.get()) == null)
			{
				strPool = InitNamePool();
			}
			int off = 0;
			if (Lookup[cp >> 8] == null || (off = Lookup[cp >> 8][cp & 0xff]) == 0)
			{
				return null;
			}
//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @SuppressWarnings("deprecation") String result = new String(strPool, 0, off >>> 8, off & 0xff);
			String result = StringHelperClass.NewString(strPool, 0, (int)((uint)off >> 8), off & 0xff); // ASCII
			return result;
		}
	}

}
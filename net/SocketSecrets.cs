/*
 * Copyright (c) 2014, Oracle and/or its affiliates. All rights reserved.
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

namespace java.net
{

	internal class SocketSecrets
	{

		/* accessed by reflection from jdk.net.Sockets */

		/* obj must be a Socket or ServerSocket */

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: private static <T> void setOption(Object obj, SocketOption<T> name, T value) throws java.io.IOException
		private static void setOption<T>(Object obj, SocketOption<T> name, T value)
		{
			SocketImpl impl;

			if (obj is Socket)
			{
				impl = ((Socket)obj).Impl;
			}
			else if (obj is ServerSocket)
			{
				impl = ((ServerSocket)obj).Impl;
			}
			else
			{
				throw new IllegalArgumentException();
			}
			impl.SetOption(name, value);
		}

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: private static <T> T getOption(Object obj, SocketOption<T> name) throws java.io.IOException
		private static T getOption<T>(Object obj, SocketOption<T> name)
		{
			SocketImpl impl;

			if (obj is Socket)
			{
				impl = ((Socket)obj).Impl;
			}
			else if (obj is ServerSocket)
			{
				impl = ((ServerSocket)obj).Impl;
			}
			else
			{
				throw new IllegalArgumentException();
			}
			return impl.GetOption(name);
		}

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: private static <T> void setOption(DatagramSocket s, SocketOption<T> name, T value) throws java.io.IOException
		private static void setOption<T>(DatagramSocket s, SocketOption<T> name, T value)
		{
			s.Impl.SetOption(name, value);
		}

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: private static <T> T getOption(DatagramSocket s, SocketOption<T> name) throws java.io.IOException
		private static T getOption<T>(DatagramSocket s, SocketOption<T> name)
		{
			return s.Impl.GetOption(name);
		}

	}

}
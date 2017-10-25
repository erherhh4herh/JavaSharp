using System;
using System.Diagnostics;

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
namespace java.net
{


	/// <summary>
	/// This class defines a factory for creating DatagramSocketImpls. It defaults
	/// to creating plain DatagramSocketImpls, but may create other DatagramSocketImpls
	/// by setting the impl.prefix system property.
	/// 
	/// For Windows versions lower than Windows Vista a TwoStacksPlainDatagramSocketImpl
	/// is always created. This impl supports IPv6 on these platform where available.
	/// 
	/// On Windows platforms greater than Vista that support a dual layer TCP/IP stack
	/// a DualStackPlainDatagramSocketImpl is created for DatagramSockets. For MulticastSockets
	/// a TwoStacksPlainDatagramSocketImpl is always created. This is to overcome the lack
	/// of behavior defined for multicasting over a dual layer socket by the RFC.
	/// 
	/// @author Chris Hegarty
	/// </summary>

	internal class DefaultDatagramSocketImplFactory
	{
		internal static Class PrefixImplClass = null;

		/* the windows version. */
		private static float Version;

		/* java.net.preferIPv4Stack */
		private static bool PreferIPv4Stack = false;

		/* If the version supports a dual stack TCP implementation */
		private static bool UseDualStackImpl = false;

		/* sun.net.useExclusiveBind */
		private static String ExclBindProp;

		/* True if exclusive binding is on for Windows */
		private static bool ExclusiveBind = true;


		static DefaultDatagramSocketImplFactory()
		{
			// Determine Windows Version.
			AccessController.doPrivileged(new PrivilegedActionAnonymousInnerClassHelper());

			// (version >= 6.0) implies Vista or greater.
			if (Version >= 6.0 && !PreferIPv4Stack)
			{
					UseDualStackImpl = true;
			}
			if (ExclBindProp != null)
			{
				// sun.net.useExclusiveBind is true
				ExclusiveBind = ExclBindProp.Length() == 0 ? true : Convert.ToBoolean(ExclBindProp);
			}
			else if (Version < 6.0)
			{
				ExclusiveBind = false;
			}

			// impl.prefix
			String prefix = null;
			try
			{
				prefix = AccessController.doPrivileged(new sun.security.action.GetPropertyAction("impl.prefix", null));
				if (prefix != null)
				{
					PrefixImplClass = Class.ForName("java.net." + prefix + "DatagramSocketImpl");
				}
			}
			catch (Exception)
			{
				System.Console.Error.WriteLine("Can't find class: java.net." + prefix + "DatagramSocketImpl: check impl.prefix property");
			}
		}

		private class PrivilegedActionAnonymousInnerClassHelper : PrivilegedAction<Object>
		{
			public PrivilegedActionAnonymousInnerClassHelper()
			{
			}

			public virtual Object Run()
			{
				Version = 0;
				try
				{
					Version = Convert.ToSingle(System.Properties.getProperty("os.version"));
					PreferIPv4Stack = Convert.ToBoolean(System.Properties.getProperty("java.net.preferIPv4Stack"));
					ExclBindProp = System.getProperty("sun.net.useExclusiveBind");
				}
				catch (NumberFormatException e)
				{
					Debug.Assert(false, e);
				}
				return null; // nothing to return
			}
		}

		/// <summary>
		/// Creates a new <code>DatagramSocketImpl</code> instance.
		/// </summary>
		/// <param name="isMulticast"> true if this impl is to be used for a MutlicastSocket </param>
		/// <returns>  a new instance of <code>PlainDatagramSocketImpl</code>. </returns>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: static DatagramSocketImpl createDatagramSocketImpl(boolean isMulticast) throws SocketException
		internal static DatagramSocketImpl CreateDatagramSocketImpl(bool isMulticast)
		{
			if (PrefixImplClass != null)
			{
				try
				{
					return (DatagramSocketImpl) PrefixImplClass.NewInstance();
				}
				catch (Exception)
				{
					throw new SocketException("can't instantiate DatagramSocketImpl");
				}
			}
			else
			{
				if (isMulticast)
				{
					ExclusiveBind = false;
				}
				if (UseDualStackImpl && !isMulticast)
				{
					return new DualStackPlainDatagramSocketImpl(ExclusiveBind);
				}
				else
				{
					return new TwoStacksPlainDatagramSocketImpl(ExclusiveBind);
				}
			}
		}
	}

}
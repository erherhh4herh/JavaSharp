/*
 * Copyright (c) 2002, 2009, Oracle and/or its affiliates. All rights reserved.
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

namespace java.security.cert
{

	using CertPathHelper = sun.security.provider.certpath.CertPathHelper;

	using GeneralNameInterface = sun.security.x509.GeneralNameInterface;

	/// <summary>
	/// Helper class that allows the Sun CertPath provider to access
	/// implementation dependent APIs in CertPath framework.
	/// 
	/// @author Andreas Sterbenz
	/// </summary>
	internal class CertPathHelperImpl : CertPathHelper
	{

		private CertPathHelperImpl()
		{
			// empty
		}

		/// <summary>
		/// Initialize the helper framework. This method must be called from
		/// the static initializer of each class that is the target of one of
		/// the methods in this class. This ensures that the helper is initialized
		/// prior to a tunneled call from the Sun provider.
		/// </summary>
		internal static void Initialize()
		{
			lock (typeof(CertPathHelperImpl))
			{
				if (CertPathHelper.instance == null)
				{
					CertPathHelper.instance = new CertPathHelperImpl();
				}
			}
		}

		protected internal virtual void ImplSetPathToNames(X509CertSelector sel, Set<GeneralNameInterface> names)
		{
			sel.PathToNamesInternal = names;
		}

		protected internal virtual void ImplSetDateAndTime(X509CRLSelector sel, Date date, long skew)
		{
			sel.SetDateAndTime(date, skew);
		}
	}

}
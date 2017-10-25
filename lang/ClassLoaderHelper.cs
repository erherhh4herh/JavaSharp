/*
 * Copyright (c) 2012, Oracle and/or its affiliates. All rights reserved.
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

	internal class ClassLoaderHelper
	{

		private ClassLoaderHelper()
		{
		}

		/// <summary>
		/// Returns an alternate path name for the given file
		/// such that if the original pathname did not exist, then the
		/// file may be located at the alternate location.
		/// For most platforms, this behavior is not supported and returns null.
		/// </summary>
		internal static File MapAlternativeName(File lib)
		{
			return null;
		}
	}

}
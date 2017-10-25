using System.Collections.Generic;

/*
 * Copyright (c) 2002, 2013, Oracle and/or its affiliates. All rights reserved.
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

namespace java.util.jar
{

	using JavaUtilJarAccess = sun.misc.JavaUtilJarAccess;

	internal class JavaUtilJarAccessImpl : JavaUtilJarAccess
	{
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public boolean jarFileHasClassPathAttribute(JarFile jar) throws java.io.IOException
		public virtual bool JarFileHasClassPathAttribute(JarFile jar)
		{
			return jar.HasClassPathAttribute();
		}

		public virtual CodeSource[] GetCodeSources(JarFile jar, URL url)
		{
			return jar.GetCodeSources(url);
		}

		public virtual CodeSource GetCodeSource(JarFile jar, URL url, String name)
		{
			return jar.GetCodeSource(url, name);
		}

		public virtual IEnumerator<String> EntryNames(JarFile jar, CodeSource[] cs)
		{
			return jar.EntryNames(cs);
		}

		public virtual IEnumerator<JarEntry> Entries2(JarFile jar)
		{
			return jar.Entries2();
		}

		public virtual void SetEagerValidation(JarFile jar, bool eager)
		{
			jar.EagerValidation = eager;
		}

		public virtual IList<Object> GetManifestDigests(JarFile jar)
		{
			return jar.ManifestDigests;
		}
	}

}
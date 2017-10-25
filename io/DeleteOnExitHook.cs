using System.Collections.Generic;

/*
 * Copyright (c) 2005, 2010, Oracle and/or its affiliates. All rights reserved.
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
namespace java.io
{


	/// <summary>
	/// This class holds a set of filenames to be deleted on VM exit through a shutdown hook.
	/// A set is used both to prevent double-insertion of the same file as well as offer
	/// quick removal.
	/// </summary>

	internal class DeleteOnExitHook
	{
		private static LinkedHashSet<String> Files = new LinkedHashSet<String>();
		static DeleteOnExitHook()
		{
			// DeleteOnExitHook must be the last shutdown hook to be invoked.
			// Application shutdown hooks may add the first file to the
			// delete on exit list and cause the DeleteOnExitHook to be
			// registered during shutdown in progress. So set the
			// registerShutdownInProgress parameter to true.
			sun.misc.SharedSecrets.JavaLangAccess.registerShutdownHook(2, true, new RunnableAnonymousInnerClassHelper() // register even if shutdown in progress -  Shutdown hook invocation order
		   );
		}

		private class RunnableAnonymousInnerClassHelper : Runnable
		{
			public RunnableAnonymousInnerClassHelper()
			{
			}

			public virtual void Run()
			{
			   RunHooks();
			}
		}

		private DeleteOnExitHook()
		{
		}

		internal static void Add(String file)
		{
			lock (typeof(DeleteOnExitHook))
			{
				if (Files == null)
				{
					// DeleteOnExitHook is running. Too late to add a file
					throw new IllegalStateException("Shutdown in progress");
				}
        
				Files.Add(file);
			}
		}

		internal static void RunHooks()
		{
			LinkedHashSet<String> theFiles;

			lock (typeof(DeleteOnExitHook))
			{
				theFiles = Files;
				Files = null;
			}

			List<String> toBeDeleted = new List<String>(theFiles);

			// reverse the list to maintain previous jdk deletion order.
			// Last in first deleted.
			Collections.Reverse(toBeDeleted);
			foreach (String filename in toBeDeleted)
			{
				if (System.IO.Directory.Exists(filename)) System.IO.Directory.Delete(filename, true); else System.IO.File.Delete(filename);
			}
		}
	}

}
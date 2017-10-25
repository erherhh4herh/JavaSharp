using System;
using System.Collections.Generic;
using System.Threading;

/*
 * Copyright (c) 2011, 2013, Oracle and/or its affiliates. All rights reserved.
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

namespace java.beans
{

	using BeanInfoFinder = com.sun.beans.finder.BeanInfoFinder;
	using PropertyEditorFinder = com.sun.beans.finder.PropertyEditorFinder;


	/// <summary>
	/// The {@code ThreadGroupContext} is an application-dependent
	/// context referenced by the specific <seealso cref="ThreadGroup"/>.
	/// This is a replacement for the <seealso cref="sun.awt.AppContext"/>.
	/// 
	/// @author  Sergey Malenkov
	/// </summary>
	internal sealed class ThreadGroupContext
	{

		private static readonly WeakIdentityMap<ThreadGroupContext> contexts = new WeakIdentityMapAnonymousInnerClassHelper();

		private class WeakIdentityMapAnonymousInnerClassHelper : WeakIdentityMap<ThreadGroupContext>
		{
			public WeakIdentityMapAnonymousInnerClassHelper()
			{
			}

			protected internal virtual ThreadGroupContext Create(Object key)
			{
				return new ThreadGroupContext();
			}
		}

		/// <summary>
		/// Returns the appropriate {@code ThreadGroupContext} for the caller,
		/// as determined by its {@code ThreadGroup}.
		/// </summary>
		/// <returns>  the application-dependent context </returns>
		internal static ThreadGroupContext Context
		{
			get
			{
				return contexts.get(Thread.CurrentThread.ThreadGroup);
			}
		}

		private volatile bool IsDesignTime;
		private volatile Boolean IsGuiAvailable;

		private IDictionary<Class, BeanInfo> BeanInfoCache;
		private BeanInfoFinder BeanInfoFinder_Renamed;
		private PropertyEditorFinder PropertyEditorFinder_Renamed;

		private ThreadGroupContext()
		{
		}

		internal bool DesignTime
		{
			get
			{
				return this.IsDesignTime;
			}
			set
			{
				this.IsDesignTime = value;
			}
		}



		internal bool GuiAvailable
		{
			get
			{
				Boolean isGuiAvailable = this.IsGuiAvailable;
				return (isGuiAvailable != null) ? isGuiAvailable.BooleanValue() :!GraphicsEnvironment.Headless;
			}
			set
			{
				this.IsGuiAvailable = Convert.ToBoolean(value);
			}
		}



		internal BeanInfo GetBeanInfo(Class type)
		{
			return (this.BeanInfoCache != null) ? this.BeanInfoCache[type] : null;
		}

		internal BeanInfo PutBeanInfo(Class type, BeanInfo info)
		{
			if (this.BeanInfoCache == null)
			{
				this.BeanInfoCache = new WeakHashMap<>();
			}
			return this.BeanInfoCache[type] = info;
		}

		internal void RemoveBeanInfo(Class type)
		{
			if (this.BeanInfoCache != null)
			{
				this.BeanInfoCache.Remove(type);
			}
		}

		internal void ClearBeanInfoCache()
		{
			if (this.BeanInfoCache != null)
			{
				this.BeanInfoCache.Clear();
			}
		}


		internal BeanInfoFinder BeanInfoFinder
		{
			get
			{
				lock (this)
				{
					if (this.BeanInfoFinder_Renamed == null)
					{
						this.BeanInfoFinder_Renamed = new BeanInfoFinder();
					}
					return this.BeanInfoFinder_Renamed;
				}
			}
		}

		internal PropertyEditorFinder PropertyEditorFinder
		{
			get
			{
				lock (this)
				{
					if (this.PropertyEditorFinder_Renamed == null)
					{
						this.PropertyEditorFinder_Renamed = new PropertyEditorFinder();
					}
					return this.PropertyEditorFinder_Renamed;
				}
			}
		}
	}

}
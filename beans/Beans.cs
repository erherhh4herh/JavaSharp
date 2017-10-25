using System;
using System.Collections.Generic;

/*
 * Copyright (c) 1996, 2015, Oracle and/or its affiliates. All rights reserved.
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

	using ClassFinder = com.sun.beans.finder.ClassFinder;






	/// <summary>
	/// This class provides some general purpose beans control methods.
	/// </summary>

	public class Beans
	{

		/// <summary>
		/// <para>
		/// Instantiate a JavaBean.
		/// </para> </summary>
		/// <returns> a JavaBean </returns>
		/// <param name="cls">         the class-loader from which we should create
		///                        the bean.  If this is null, then the system
		///                        class-loader is used. </param>
		/// <param name="beanName">    the name of the bean within the class-loader.
		///                        For example "sun.beanbox.foobah"
		/// </param>
		/// <exception cref="ClassNotFoundException"> if the class of a serialized
		///              object could not be found. </exception>
		/// <exception cref="IOException"> if an I/O error occurs. </exception>

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public static Object instantiate(ClassLoader cls, String beanName) throws java.io.IOException, ClassNotFoundException
		public static Object Instantiate(ClassLoader cls, String beanName)
		{
			return Beans.Instantiate(cls, beanName, null, null);
		}

		/// <summary>
		/// <para>
		/// Instantiate a JavaBean.
		/// </para> </summary>
		/// <returns> a JavaBean
		/// </returns>
		/// <param name="cls">         the class-loader from which we should create
		///                        the bean.  If this is null, then the system
		///                        class-loader is used. </param>
		/// <param name="beanName">    the name of the bean within the class-loader.
		///                        For example "sun.beanbox.foobah" </param>
		/// <param name="beanContext"> The BeanContext in which to nest the new bean
		/// </param>
		/// <exception cref="ClassNotFoundException"> if the class of a serialized
		///              object could not be found. </exception>
		/// <exception cref="IOException"> if an I/O error occurs. </exception>

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public static Object instantiate(ClassLoader cls, String beanName, java.beans.beancontext.BeanContext beanContext) throws java.io.IOException, ClassNotFoundException
		public static Object Instantiate(ClassLoader cls, String beanName, BeanContext beanContext)
		{
			return Beans.Instantiate(cls, beanName, beanContext, null);
		}

		/// <summary>
		/// Instantiate a bean.
		/// <para>
		/// The bean is created based on a name relative to a class-loader.
		/// This name should be a dot-separated name such as "a.b.c".
		/// </para>
		/// <para>
		/// In Beans 1.0 the given name can indicate either a serialized object
		/// or a class.  Other mechanisms may be added in the future.  In
		/// beans 1.0 we first try to treat the beanName as a serialized object
		/// name then as a class name.
		/// </para>
		/// <para>
		/// When using the beanName as a serialized object name we convert the
		/// given beanName to a resource pathname and add a trailing ".ser" suffix.
		/// We then try to load a serialized object from that resource.
		/// </para>
		/// <para>
		/// For example, given a beanName of "x.y", Beans.instantiate would first
		/// try to read a serialized object from the resource "x/y.ser" and if
		/// that failed it would try to load the class "x.y" and create an
		/// instance of that class.
		/// </para>
		/// <para>
		/// If the bean is a subtype of java.applet.Applet, then it is given
		/// some special initialization.  First, it is supplied with a default
		/// AppletStub and AppletContext.  Second, if it was instantiated from
		/// a classname the applet's "init" method is called.  (If the bean was
		/// deserialized this step is skipped.)
		/// </para>
		/// <para>
		/// Note that for beans which are applets, it is the caller's responsiblity
		/// to call "start" on the applet.  For correct behaviour, this should be done
		/// after the applet has been added into a visible AWT container.
		/// </para>
		/// <para>
		/// Note that applets created via beans.instantiate run in a slightly
		/// different environment than applets running inside browsers.  In
		/// particular, bean applets have no access to "parameters", so they may
		/// wish to provide property get/set methods to set parameter values.  We
		/// advise bean-applet developers to test their bean-applets against both
		/// the JDK appletviewer (for a reference browser environment) and the
		/// BDK BeanBox (for a reference bean container).
		/// 
		/// </para>
		/// </summary>
		/// <returns> a JavaBean </returns>
		/// <param name="cls">         the class-loader from which we should create
		///                        the bean.  If this is null, then the system
		///                        class-loader is used. </param>
		/// <param name="beanName">    the name of the bean within the class-loader.
		///                        For example "sun.beanbox.foobah" </param>
		/// <param name="beanContext"> The BeanContext in which to nest the new bean </param>
		/// <param name="initializer"> The AppletInitializer for the new bean
		/// </param>
		/// <exception cref="ClassNotFoundException"> if the class of a serialized
		///              object could not be found. </exception>
		/// <exception cref="IOException"> if an I/O error occurs. </exception>

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public static Object instantiate(ClassLoader cls, String beanName, java.beans.beancontext.BeanContext beanContext, AppletInitializer initializer) throws java.io.IOException, ClassNotFoundException
		public static Object Instantiate(ClassLoader cls, String beanName, BeanContext beanContext, AppletInitializer initializer)
		{

			InputStream ins;
			ObjectInputStream oins = null;
			Object result = null;
			bool serialized = false;
			IOException serex = null;

			// If the given classloader is null, we check if an
			// system classloader is available and (if so)
			// use that instead.
			// Note that calls on the system class loader will
			// look in the bootstrap class loader first.
			if (cls == null)
			{
				try
				{
					cls = ClassLoader.SystemClassLoader;
				}
				catch (SecurityException)
				{
					// We're not allowed to access the system class loader.
					// Drop through.
				}
			}

			// Try to find a serialized object with this name
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final String serName = beanName.replace('.','/').concat(".ser");
			String serName = beanName.Replace('.','/') + ".ser";
			if (cls == null)
			{
				ins = ClassLoader.GetSystemResourceAsStream(serName);
			}
			else
			{
				ins = cls.GetResourceAsStream(serName);
			}
			if (ins != null)
			{
				try
				{
					if (cls == null)
					{
						oins = new ObjectInputStream(ins);
					}
					else
					{
						oins = new ObjectInputStreamWithLoader(ins, cls);
					}
					result = oins.ReadObject();
					serialized = true;
					oins.Close();
				}
				catch (IOException ex)
				{
					ins.Close();
					// Drop through and try opening the class.  But remember
					// the exception in case we can't find the class either.
					serex = ex;
				}
				catch (ClassNotFoundException ex)
				{
					ins.Close();
					throw ex;
				}
			}

			if (result == null)
			{
				// No serialized object, try just instantiating the class
				Class cl;

				try
				{
					cl = ClassFinder.findClass(beanName, cls);
				}
				catch (ClassNotFoundException ex)
				{
					// There is no appropriate class.  If we earlier tried to
					// deserialize an object and got an IO exception, throw that,
					// otherwise rethrow the ClassNotFoundException.
					if (serex != null)
					{
						throw serex;
					}
					throw ex;
				}

				if (!Modifier.isPublic(cl.Modifiers))
				{
					throw new ClassNotFoundException("" + cl + " : no public access");
				}

				/*
				 * Try to instantiate the class.
				 */

				try
				{
					result = cl.NewInstance();
				}
				catch (Exception ex)
				{
					// We have to remap the exception to one in our signature.
					// But we pass extra information in the detail message.
					throw new ClassNotFoundException("" + cl + " : " + ex, ex);
				}
			}

			if (result != null)
			{

				// Ok, if the result is an applet initialize it.

				AppletStub stub = null;

				if (result is Applet)
				{
					Applet applet = (Applet) result;
					bool needDummies = initializer == null;

					if (needDummies)
					{

						// Figure our the codebase and docbase URLs.  We do this
						// by locating the URL for a known resource, and then
						// massaging the URL.

						// First find the "resource name" corresponding to the bean
						// itself.  So a serialzied bean "a.b.c" would imply a
						// resource name of "a/b/c.ser" and a classname of "x.y"
						// would imply a resource name of "x/y.class".

//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final String resourceName;
						String resourceName;

						if (serialized)
						{
							// Serialized bean
							resourceName = beanName.Replace('.','/') + ".ser";
						}
						else
						{
							// Regular class
							resourceName = beanName.Replace('.','/') + ".class";
						}

						URL objectUrl = null;
						URL codeBase = null;
						URL docBase = null;

						// Now get the URL correponding to the resource name.
						if (cls == null)
						{
							objectUrl = ClassLoader.GetSystemResource(resourceName);
						}
						else
						{
							objectUrl = cls.GetResource(resourceName);
						}

						// If we found a URL, we try to locate the docbase by taking
						// of the final path name component, and the code base by taking
						// of the complete resourceName.
						// So if we had a resourceName of "a/b/c.class" and we got an
						// objectURL of "file://bert/classes/a/b/c.class" then we would
						// want to set the codebase to "file://bert/classes/" and the
						// docbase to "file://bert/classes/a/b/"

						if (objectUrl != null)
						{
							String s = objectUrl.ToExternalForm();

							if (s.EndsWith(resourceName))
							{
								int ix = s.Length() - resourceName.Length();
								codeBase = new URL(s.Substring(0,ix));
								docBase = codeBase;

								ix = s.LastIndexOf('/');

								if (ix >= 0)
								{
									docBase = new URL(s.Substring(0,ix + 1));
								}
							}
						}

						// Setup a default context and stub.
						BeansAppletContext context = new BeansAppletContext(applet);

						stub = (AppletStub)new BeansAppletStub(applet, context, codeBase, docBase);
						applet.Stub = stub;
					}
					else
					{
						initializer.Initialize(applet, beanContext);
					}

					// now, if there is a BeanContext, add the bean, if applicable.

					if (beanContext != null)
					{
						UnsafeBeanContextAdd(beanContext, result);
					}

					// If it was deserialized then it was already init-ed.
					// Otherwise we need to initialize it.

					if (!serialized)
					{
						// We need to set a reasonable initial size, as many
						// applets are unhappy if they are started without
						// having been explicitly sized.
						applet.SetSize(100,100);
						applet.Init();
					}

					if (needDummies)
					{
					  ((BeansAppletStub)stub).Active_Renamed = true;
					}
					else
					{
						initializer.Activate(applet);
					}

				}
				else if (beanContext != null)
				{
					UnsafeBeanContextAdd(beanContext, result);
				}
			}

			return result;
		}

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @SuppressWarnings("unchecked") private static void unsafeBeanContextAdd(java.beans.beancontext.BeanContext beanContext, Object res)
		private static void UnsafeBeanContextAdd(BeanContext beanContext, Object res)
		{
			beanContext.Add(res);
		}

		/// <summary>
		/// From a given bean, obtain an object representing a specified
		/// type view of that source object.
		/// <para>
		/// The result may be the same object or a different object.  If
		/// the requested target view isn't available then the given
		/// bean is returned.
		/// </para>
		/// <para>
		/// This method is provided in Beans 1.0 as a hook to allow the
		/// addition of more flexible bean behaviour in the future.
		/// 
		/// </para>
		/// </summary>
		/// <returns> an object representing a specified type view of the
		/// source object </returns>
		/// <param name="bean">        Object from which we want to obtain a view. </param>
		/// <param name="targetType">  The type of view we'd like to get.
		///  </param>
		public static Object GetInstanceOf(Object bean, Class targetType)
		{
			return bean;
		}

		/// <summary>
		/// Check if a bean can be viewed as a given target type.
		/// The result will be true if the Beans.getInstanceof method
		/// can be used on the given bean to obtain an object that
		/// represents the specified targetType type view.
		/// </summary>
		/// <param name="bean">  Bean from which we want to obtain a view. </param>
		/// <param name="targetType">  The type of view we'd like to get. </param>
		/// <returns> "true" if the given bean supports the given targetType.
		///  </returns>
		public static bool IsInstanceOf(Object bean, Class targetType)
		{
			return Introspector.IsSubclass(bean.GetType(), targetType);
		}

		/// <summary>
		/// Test if we are in design-mode.
		/// </summary>
		/// <returns>  True if we are running in an application construction
		///          environment.
		/// </returns>
		/// <seealso cref= DesignMode </seealso>
		public static bool DesignTime
		{
			get
			{
				return ThreadGroupContext.Context.DesignTime;
			}
			set
			{
				SecurityManager sm = System.SecurityManager;
				if (sm != null)
				{
					sm.CheckPropertiesAccess();
				}
				ThreadGroupContext.Context.DesignTime = value;
			}
		}

		/// <summary>
		/// Determines whether beans can assume a GUI is available.
		/// </summary>
		/// <returns>  True if we are running in an environment where beans
		///     can assume that an interactive GUI is available, so they
		///     can pop up dialog boxes, etc.  This will normally return
		///     true in a windowing environment, and will normally return
		///     false in a server environment or if an application is
		///     running as part of a batch job.
		/// </returns>
		/// <seealso cref= Visibility
		///  </seealso>
		public static bool GuiAvailable
		{
			get
			{
				return ThreadGroupContext.Context.GuiAvailable;
			}
			set
			{
				SecurityManager sm = System.SecurityManager;
				if (sm != null)
				{
					sm.CheckPropertiesAccess();
				}
				ThreadGroupContext.Context.GuiAvailable = value;
			}
		}

		/// <summary>
		/// Used to indicate whether of not we are running in an application
		/// builder environment.
		/// 
		/// <para>Note that this method is security checked
		/// and is not available to (for example) untrusted applets.
		/// More specifically, if there is a security manager,
		/// its <code>checkPropertiesAccess</code>
		/// method is called. This could result in a SecurityException.
		/// 
		/// </para>
		/// </summary>
		/// <param name="isDesignTime">  True if we're in an application builder tool. </param>
		/// <exception cref="SecurityException">  if a security manager exists and its
		///             <code>checkPropertiesAccess</code> method doesn't allow setting
		///              of system properties. </exception>
		/// <seealso cref= SecurityManager#checkPropertiesAccess </seealso>


		/// <summary>
		/// Used to indicate whether of not we are running in an environment
		/// where GUI interaction is available.
		/// 
		/// <para>Note that this method is security checked
		/// and is not available to (for example) untrusted applets.
		/// More specifically, if there is a security manager,
		/// its <code>checkPropertiesAccess</code>
		/// method is called. This could result in a SecurityException.
		/// 
		/// </para>
		/// </summary>
		/// <param name="isGuiAvailable">  True if GUI interaction is available. </param>
		/// <exception cref="SecurityException">  if a security manager exists and its
		///             <code>checkPropertiesAccess</code> method doesn't allow setting
		///              of system properties. </exception>
		/// <seealso cref= SecurityManager#checkPropertiesAccess </seealso>

	}

	/// <summary>
	/// This subclass of ObjectInputStream delegates loading of classes to
	/// an existing ClassLoader.
	/// </summary>

	internal class ObjectInputStreamWithLoader : ObjectInputStream
	{
		private ClassLoader Loader;

		/// <summary>
		/// Loader must be non-null;
		/// </summary>

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public ObjectInputStreamWithLoader(java.io.InputStream in, ClassLoader loader) throws java.io.IOException, java.io.StreamCorruptedException
		public ObjectInputStreamWithLoader(InputStream @in, ClassLoader loader) : base(@in)
		{

			if (loader == null)
			{
				throw new IllegalArgumentException("Illegal null argument to ObjectInputStreamWithLoader");
			}
			this.Loader = loader;
		}

		/// <summary>
		/// Use the given ClassLoader rather than using the system class
		/// </summary>
//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @SuppressWarnings("rawtypes") protected Class resolveClass(java.io.ObjectStreamClass classDesc) throws java.io.IOException, ClassNotFoundException
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
		protected internal override Class ResolveClass(ObjectStreamClass classDesc)
		{

			String cname = classDesc.Name;
			return ClassFinder.resolveClass(cname, this.Loader);
		}
	}

	/// <summary>
	/// Package private support class.  This provides a default AppletContext
	/// for beans which are applets.
	/// </summary>

	internal class BeansAppletContext : AppletContext
	{
		internal Applet Target;
		internal Dictionary<URL, Object> ImageCache = new Dictionary<URL, Object>();

		internal BeansAppletContext(Applet target)
		{
			this.Target = target;
		}

		public virtual AudioClip GetAudioClip(URL url)
		{
			// We don't currently support audio clips in the Beans.instantiate
			// applet context, unless by some luck there exists a URL content
			// class that can generate an AudioClip from the audio URL.
			try
			{
				return (AudioClip) url.Content;
			}
			catch (Exception)
			{
				return null;
			}
		}

		public virtual Image GetImage(URL url)
		{
			lock (this)
			{
				Object o = ImageCache[url];
				if (o != null)
				{
					return (Image)o;
				}
				try
				{
					o = url.Content;
					if (o == null)
					{
						return null;
					}
					if (o is Image)
					{
						ImageCache[url] = o;
						return (Image) o;
					}
					// Otherwise it must be an ImageProducer.
					Image img = Target.CreateImage((java.awt.image.ImageProducer)o);
					ImageCache[url] = img;
					return img;
        
				}
				catch (Exception)
				{
					return null;
				}
			}
		}

		public virtual Applet GetApplet(String name)
		{
			return null;
		}

		public virtual IEnumerator<Applet> Applets
		{
			get
			{
				List<Applet> applets = new List<Applet>();
				applets.Add(Target);
				return applets.elements();
			}
		}

		public virtual void ShowDocument(URL url)
		{
			// We do nothing.
		}

		public virtual void ShowDocument(URL url, String target)
		{
			// We do nothing.
		}

		public virtual void ShowStatus(String status)
		{
			// We do nothing.
		}

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public void setStream(String key, java.io.InputStream stream)throws java.io.IOException
		public virtual void SetStream(String key, InputStream stream)
		{
			// We do nothing.
		}

		public virtual InputStream GetStream(String key)
		{
			// We do nothing.
			return null;
		}

		public virtual IEnumerator<String> StreamKeys
		{
			get
			{
				// We do nothing.
				return null;
			}
		}
	}

	/// <summary>
	/// Package private support class.  This provides an AppletStub
	/// for beans which are applets.
	/// </summary>
	internal class BeansAppletStub : AppletStub
	{
		[NonSerialized]
		internal bool Active_Renamed;
		[NonSerialized]
		internal Applet Target;
		[NonSerialized]
		internal AppletContext Context;
		[NonSerialized]
		internal URL CodeBase_Renamed;
		[NonSerialized]
		internal URL DocBase;

		internal BeansAppletStub(Applet target, AppletContext context, URL codeBase, URL docBase)
		{
			this.Target = target;
			this.Context = context;
			this.CodeBase_Renamed = codeBase;
			this.DocBase = docBase;
		}

		public virtual bool Active
		{
			get
			{
				return Active_Renamed;
			}
		}

		public virtual URL DocumentBase
		{
			get
			{
				// use the root directory of the applet's class-loader
				return DocBase;
			}
		}

		public virtual URL CodeBase
		{
			get
			{
				// use the directory where we found the class or serialized object.
				return CodeBase_Renamed;
			}
		}

		public virtual String GetParameter(String name)
		{
			return null;
		}

		public virtual AppletContext AppletContext
		{
			get
			{
				return Context;
			}
		}

		public virtual void AppletResize(int width, int height)
		{
			// we do nothing.
		}
	}

}
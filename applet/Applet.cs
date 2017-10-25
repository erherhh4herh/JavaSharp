using System;

/*
 * Copyright (c) 1995, 2013, Oracle and/or its affiliates. All rights reserved.
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
namespace java.applet
{


	/// <summary>
	/// An applet is a small program that is intended not to be run on
	/// its own, but rather to be embedded inside another application.
	/// <para>
	/// The <code>Applet</code> class must be the superclass of any
	/// applet that is to be embedded in a Web page or viewed by the Java
	/// Applet Viewer. The <code>Applet</code> class provides a standard
	/// interface between applets and their environment.
	/// 
	/// @author      Arthur van Hoff
	/// @author      Chris Warth
	/// @since       JDK1.0
	/// </para>
	/// </summary>
	public class Applet : Panel
	{

		/// <summary>
		/// Constructs a new Applet.
		/// <para>
		/// Note: Many methods in <code>java.applet.Applet</code>
		/// may be invoked by the applet only after the applet is
		/// fully constructed; applet should avoid calling methods
		/// in <code>java.applet.Applet</code> in the constructor.
		/// 
		/// </para>
		/// </summary>
		/// <exception cref="HeadlessException"> if GraphicsEnvironment.isHeadless()
		/// returns true. </exception>
		/// <seealso cref= java.awt.GraphicsEnvironment#isHeadless
		/// @since 1.4 </seealso>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public Applet() throws HeadlessException
		public Applet()
		{
			if (GraphicsEnvironment.Headless)
			{
				throw new HeadlessException();
			}
		}

		/// <summary>
		/// Applets can be serialized but the following conventions MUST be followed:
		/// 
		/// Before Serialization:
		/// An applet must be in STOPPED state.
		/// 
		/// After Deserialization:
		/// The applet will be restored in STOPPED state (and most clients will
		/// likely move it into RUNNING state).
		/// The stub field will be restored by the reader.
		/// </summary>
		[NonSerialized]
		private AppletStub Stub_Renamed;

		/* version ID for serialized form. */
		private const long SerialVersionUID = -5836846270535785031L;

		/// <summary>
		/// Read an applet from an object input stream. </summary>
		/// <exception cref="HeadlessException"> if
		/// <code>GraphicsEnvironment.isHeadless()</code> returns
		/// <code>true</code>
		/// @serial </exception>
		/// <seealso cref= java.awt.GraphicsEnvironment#isHeadless
		/// @since 1.4 </seealso>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: private void readObject(java.io.ObjectInputStream s) throws ClassNotFoundException, java.io.IOException, HeadlessException
		private void ReadObject(ObjectInputStream s)
		{
			if (GraphicsEnvironment.Headless)
			{
				throw new HeadlessException();
			}
			s.DefaultReadObject();
		}

		/// <summary>
		/// Sets this applet's stub. This is done automatically by the system.
		/// <para>If there is a security manager, its <code> checkPermission </code>
		/// method is called with the
		/// <code>AWTPermission("setAppletStub")</code>
		/// permission if a stub has already been set.
		/// </para>
		/// </summary>
		/// <param name="stub">   the new stub. </param>
		/// <exception cref="SecurityException"> if the caller cannot set the stub </exception>
		public AppletStub Stub
		{
			set
			{
				if (this.Stub_Renamed != null)
				{
					SecurityManager s = System.SecurityManager;
					if (s != null)
					{
						s.CheckPermission(new AWTPermission("setAppletStub"));
					}
				}
				this.Stub_Renamed = value;
			}
		}

		/// <summary>
		/// Determines if this applet is active. An applet is marked active
		/// just before its <code>start</code> method is called. It becomes
		/// inactive just before its <code>stop</code> method is called.
		/// </summary>
		/// <returns>  <code>true</code> if the applet is active;
		///          <code>false</code> otherwise. </returns>
		/// <seealso cref=     java.applet.Applet#start() </seealso>
		/// <seealso cref=     java.applet.Applet#stop() </seealso>
		public virtual bool Active
		{
			get
			{
				if (Stub_Renamed != null)
				{
					return Stub_Renamed.Active;
				} // If stub field not filled in, applet never active
				else
				{
					return false;
				}
			}
		}

		/// <summary>
		/// Gets the URL of the document in which this applet is embedded.
		/// For example, suppose an applet is contained
		/// within the document:
		/// <blockquote><pre>
		///    http://www.oracle.com/technetwork/java/index.html
		/// </pre></blockquote>
		/// The document base is:
		/// <blockquote><pre>
		///    http://www.oracle.com/technetwork/java/index.html
		/// </pre></blockquote>
		/// </summary>
		/// <returns>  the <seealso cref="java.net.URL"/> of the document that contains this
		///          applet. </returns>
		/// <seealso cref=     java.applet.Applet#getCodeBase() </seealso>
		public virtual URL DocumentBase
		{
			get
			{
				return Stub_Renamed.DocumentBase;
			}
		}

		/// <summary>
		/// Gets the base URL. This is the URL of the directory which contains this applet.
		/// </summary>
		/// <returns>  the base <seealso cref="java.net.URL"/> of
		///          the directory which contains this applet. </returns>
		/// <seealso cref=     java.applet.Applet#getDocumentBase() </seealso>
		public virtual URL CodeBase
		{
			get
			{
				return Stub_Renamed.CodeBase;
			}
		}

		/// <summary>
		/// Returns the value of the named parameter in the HTML tag. For
		/// example, if this applet is specified as
		/// <blockquote><pre>
		/// &lt;applet code="Clock" width=50 height=50&gt;
		/// &lt;param name=Color value="blue"&gt;
		/// &lt;/applet&gt;
		/// </pre></blockquote>
		/// <para>
		/// then a call to <code>getParameter("Color")</code> returns the
		/// value <code>"blue"</code>.
		/// </para>
		/// <para>
		/// The <code>name</code> argument is case insensitive.
		/// 
		/// </para>
		/// </summary>
		/// <param name="name">   a parameter name. </param>
		/// <returns>  the value of the named parameter,
		///          or <code>null</code> if not set. </returns>
		 public virtual String GetParameter(String name)
		 {
			 return Stub_Renamed.GetParameter(name);
		 }

		/// <summary>
		/// Determines this applet's context, which allows the applet to
		/// query and affect the environment in which it runs.
		/// <para>
		/// This environment of an applet represents the document that
		/// contains the applet.
		/// 
		/// </para>
		/// </summary>
		/// <returns>  the applet's context. </returns>
		public virtual AppletContext AppletContext
		{
			get
			{
				return Stub_Renamed.AppletContext;
			}
		}

		/// <summary>
		/// Requests that this applet be resized.
		/// </summary>
		/// <param name="width">    the new requested width for the applet. </param>
		/// <param name="height">   the new requested height for the applet. </param>
//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @SuppressWarnings("deprecation") public void resize(int width, int height)
		public override void Resize(int width, int height)
		{
			Dimension d = Size();
			if ((d.Width_Renamed != width) || (d.Height_Renamed != height))
			{
				base.Resize(width, height);
				if (Stub_Renamed != null)
				{
					Stub_Renamed.AppletResize(width, height);
				}
			}
		}

		/// <summary>
		/// Requests that this applet be resized.
		/// </summary>
		/// <param name="d">   an object giving the new width and height. </param>
//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @SuppressWarnings("deprecation") public void resize(Dimension d)
		public override void Resize(Dimension d)
		{
			Resize(d.Width_Renamed, d.Height_Renamed);
		}

		/// <summary>
		/// Indicates if this container is a validate root.
		/// <para>
		/// {@code Applet} objects are the validate roots, and, therefore, they
		/// override this method to return {@code true}.
		/// 
		/// </para>
		/// </summary>
		/// <returns> {@code true}
		/// @since 1.7 </returns>
		/// <seealso cref= java.awt.Container#isValidateRoot </seealso>
		public override bool ValidateRoot
		{
			get
			{
				return true;
			}
		}

		/// <summary>
		/// Requests that the argument string be displayed in the
		/// "status window". Many browsers and applet viewers
		/// provide such a window, where the application can inform users of
		/// its current state.
		/// </summary>
		/// <param name="msg">   a string to display in the status window. </param>
		public virtual void ShowStatus(String msg)
		{
			AppletContext.ShowStatus(msg);
		}

		/// <summary>
		/// Returns an <code>Image</code> object that can then be painted on
		/// the screen. The <code>url</code> that is passed as an argument
		/// must specify an absolute URL.
		/// <para>
		/// This method always returns immediately, whether or not the image
		/// exists. When this applet attempts to draw the image on the screen,
		/// the data will be loaded. The graphics primitives that draw the
		/// image will incrementally paint on the screen.
		/// 
		/// </para>
		/// </summary>
		/// <param name="url">   an absolute URL giving the location of the image. </param>
		/// <returns>  the image at the specified URL. </returns>
		/// <seealso cref=     java.awt.Image </seealso>
		public virtual Image GetImage(URL url)
		{
			return AppletContext.GetImage(url);
		}

		/// <summary>
		/// Returns an <code>Image</code> object that can then be painted on
		/// the screen. The <code>url</code> argument must specify an absolute
		/// URL. The <code>name</code> argument is a specifier that is
		/// relative to the <code>url</code> argument.
		/// <para>
		/// This method always returns immediately, whether or not the image
		/// exists. When this applet attempts to draw the image on the screen,
		/// the data will be loaded. The graphics primitives that draw the
		/// image will incrementally paint on the screen.
		/// 
		/// </para>
		/// </summary>
		/// <param name="url">    an absolute URL giving the base location of the image. </param>
		/// <param name="name">   the location of the image, relative to the
		///                 <code>url</code> argument. </param>
		/// <returns>  the image at the specified URL. </returns>
		/// <seealso cref=     java.awt.Image </seealso>
		public virtual Image GetImage(URL url, String name)
		{
			try
			{
				return GetImage(new URL(url, name));
			}
			catch (MalformedURLException)
			{
				return null;
			}
		}

		/// <summary>
		/// Get an audio clip from the given URL.
		/// </summary>
		/// <param name="url"> points to the audio clip </param>
		/// <returns> the audio clip at the specified URL.
		/// 
		/// @since       1.2 </returns>
		public static AudioClip NewAudioClip(URL url)
		{
			return new sun.applet.AppletAudioClip(url);
		}

		/// <summary>
		/// Returns the <code>AudioClip</code> object specified by the
		/// <code>URL</code> argument.
		/// <para>
		/// This method always returns immediately, whether or not the audio
		/// clip exists. When this applet attempts to play the audio clip, the
		/// data will be loaded.
		/// 
		/// </para>
		/// </summary>
		/// <param name="url">  an absolute URL giving the location of the audio clip. </param>
		/// <returns>  the audio clip at the specified URL. </returns>
		/// <seealso cref=     java.applet.AudioClip </seealso>
		public virtual AudioClip GetAudioClip(URL url)
		{
			return AppletContext.GetAudioClip(url);
		}

		/// <summary>
		/// Returns the <code>AudioClip</code> object specified by the
		/// <code>URL</code> and <code>name</code> arguments.
		/// <para>
		/// This method always returns immediately, whether or not the audio
		/// clip exists. When this applet attempts to play the audio clip, the
		/// data will be loaded.
		/// 
		/// </para>
		/// </summary>
		/// <param name="url">    an absolute URL giving the base location of the
		///                 audio clip. </param>
		/// <param name="name">   the location of the audio clip, relative to the
		///                 <code>url</code> argument. </param>
		/// <returns>  the audio clip at the specified URL. </returns>
		/// <seealso cref=     java.applet.AudioClip </seealso>
		public virtual AudioClip GetAudioClip(URL url, String name)
		{
			try
			{
				return GetAudioClip(new URL(url, name));
			}
			catch (MalformedURLException)
			{
				return null;
			}
		}

		/// <summary>
		/// Returns information about this applet. An applet should override
		/// this method to return a <code>String</code> containing information
		/// about the author, version, and copyright of the applet.
		/// <para>
		/// The implementation of this method provided by the
		/// <code>Applet</code> class returns <code>null</code>.
		/// 
		/// </para>
		/// </summary>
		/// <returns>  a string containing information about the author, version, and
		///          copyright of the applet. </returns>
		public virtual String AppletInfo
		{
			get
			{
				return null;
			}
		}

		/// <summary>
		/// Gets the locale of the applet. It allows the applet
		/// to maintain its own locale separated from the locale
		/// of the browser or appletviewer.
		/// </summary>
		/// <returns>  the locale of the applet; if no locale has
		///          been set, the default locale is returned.
		/// @since   JDK1.1 </returns>
		public override Locale Locale
		{
			get
			{
			  Locale locale = base.Locale;
			  if (locale == null)
			  {
				return Locale.Default;
			  }
			  return locale;
			}
		}

		/// <summary>
		/// Returns information about the parameters that are understood by
		/// this applet. An applet should override this method to return an
		/// array of <code>Strings</code> describing these parameters.
		/// <para>
		/// Each element of the array should be a set of three
		/// <code>Strings</code> containing the name, the type, and a
		/// description. For example:
		/// <blockquote><pre>
		/// String pinfo[][] = {
		///   {"fps",    "1-10",    "frames per second"},
		///   {"repeat", "boolean", "repeat image loop"},
		///   {"imgs",   "url",     "images directory"}
		/// };
		/// </pre></blockquote>
		/// </para>
		/// <para>
		/// The implementation of this method provided by the
		/// <code>Applet</code> class returns <code>null</code>.
		/// 
		/// </para>
		/// </summary>
		/// <returns>  an array describing the parameters this applet looks for. </returns>
		public virtual String[][] ParameterInfo
		{
			get
			{
				return null;
			}
		}

		/// <summary>
		/// Plays the audio clip at the specified absolute URL. Nothing
		/// happens if the audio clip cannot be found.
		/// </summary>
		/// <param name="url">   an absolute URL giving the location of the audio clip. </param>
		public virtual void Play(URL url)
		{
			AudioClip clip = GetAudioClip(url);
			if (clip != null)
			{
				clip.Play();
			}
		}

		/// <summary>
		/// Plays the audio clip given the URL and a specifier that is
		/// relative to it. Nothing happens if the audio clip cannot be found.
		/// </summary>
		/// <param name="url">    an absolute URL giving the base location of the
		///                 audio clip. </param>
		/// <param name="name">   the location of the audio clip, relative to the
		///                 <code>url</code> argument. </param>
		public virtual void Play(URL url, String name)
		{
			AudioClip clip = GetAudioClip(url, name);
			if (clip != null)
			{
				clip.Play();
			}
		}

		/// <summary>
		/// Called by the browser or applet viewer to inform
		/// this applet that it has been loaded into the system. It is always
		/// called before the first time that the <code>start</code> method is
		/// called.
		/// <para>
		/// A subclass of <code>Applet</code> should override this method if
		/// it has initialization to perform. For example, an applet with
		/// threads would use the <code>init</code> method to create the
		/// threads and the <code>destroy</code> method to kill them.
		/// </para>
		/// <para>
		/// The implementation of this method provided by the
		/// <code>Applet</code> class does nothing.
		/// 
		/// </para>
		/// </summary>
		/// <seealso cref=     java.applet.Applet#destroy() </seealso>
		/// <seealso cref=     java.applet.Applet#start() </seealso>
		/// <seealso cref=     java.applet.Applet#stop() </seealso>
		public virtual void Init()
		{
		}

		/// <summary>
		/// Called by the browser or applet viewer to inform
		/// this applet that it should start its execution. It is called after
		/// the <code>init</code> method and each time the applet is revisited
		/// in a Web page.
		/// <para>
		/// A subclass of <code>Applet</code> should override this method if
		/// it has any operation that it wants to perform each time the Web
		/// page containing it is visited. For example, an applet with
		/// animation might want to use the <code>start</code> method to
		/// resume animation, and the <code>stop</code> method to suspend the
		/// animation.
		/// </para>
		/// <para>
		/// Note: some methods, such as <code>getLocationOnScreen</code>, can only
		/// provide meaningful results if the applet is showing.  Because
		/// <code>isShowing</code> returns <code>false</code> when the applet's
		/// <code>start</code> is first called, methods requiring
		/// <code>isShowing</code> to return <code>true</code> should be called from
		/// a <code>ComponentListener</code>.
		/// </para>
		/// <para>
		/// The implementation of this method provided by the
		/// <code>Applet</code> class does nothing.
		/// 
		/// </para>
		/// </summary>
		/// <seealso cref=     java.applet.Applet#destroy() </seealso>
		/// <seealso cref=     java.applet.Applet#init() </seealso>
		/// <seealso cref=     java.applet.Applet#stop() </seealso>
		/// <seealso cref=     java.awt.Component#isShowing() </seealso>
		/// <seealso cref=     java.awt.event.ComponentListener#componentShown(java.awt.event.ComponentEvent) </seealso>
		public virtual void Start()
		{
		}

		/// <summary>
		/// Called by the browser or applet viewer to inform
		/// this applet that it should stop its execution. It is called when
		/// the Web page that contains this applet has been replaced by
		/// another page, and also just before the applet is to be destroyed.
		/// <para>
		/// A subclass of <code>Applet</code> should override this method if
		/// it has any operation that it wants to perform each time the Web
		/// page containing it is no longer visible. For example, an applet
		/// with animation might want to use the <code>start</code> method to
		/// resume animation, and the <code>stop</code> method to suspend the
		/// animation.
		/// </para>
		/// <para>
		/// The implementation of this method provided by the
		/// <code>Applet</code> class does nothing.
		/// 
		/// </para>
		/// </summary>
		/// <seealso cref=     java.applet.Applet#destroy() </seealso>
		/// <seealso cref=     java.applet.Applet#init() </seealso>
		public virtual void Stop()
		{
		}

		/// <summary>
		/// Called by the browser or applet viewer to inform
		/// this applet that it is being reclaimed and that it should destroy
		/// any resources that it has allocated. The <code>stop</code> method
		/// will always be called before <code>destroy</code>.
		/// <para>
		/// A subclass of <code>Applet</code> should override this method if
		/// it has any operation that it wants to perform before it is
		/// destroyed. For example, an applet with threads would use the
		/// <code>init</code> method to create the threads and the
		/// <code>destroy</code> method to kill them.
		/// </para>
		/// <para>
		/// The implementation of this method provided by the
		/// <code>Applet</code> class does nothing.
		/// 
		/// </para>
		/// </summary>
		/// <seealso cref=     java.applet.Applet#init() </seealso>
		/// <seealso cref=     java.applet.Applet#start() </seealso>
		/// <seealso cref=     java.applet.Applet#stop() </seealso>
		public virtual void Destroy()
		{
		}

		//
		// Accessibility support
		//

		internal new AccessibleContext AccessibleContext_Renamed = null;

		/// <summary>
		/// Gets the AccessibleContext associated with this Applet.
		/// For applets, the AccessibleContext takes the form of an
		/// AccessibleApplet.
		/// A new AccessibleApplet instance is created if necessary.
		/// </summary>
		/// <returns> an AccessibleApplet that serves as the
		///         AccessibleContext of this Applet
		/// @since 1.3 </returns>
		public override AccessibleContext AccessibleContext
		{
			get
			{
				if (AccessibleContext_Renamed == null)
				{
					AccessibleContext_Renamed = new AccessibleApplet(this);
				}
				return AccessibleContext_Renamed;
			}
		}

		/// <summary>
		/// This class implements accessibility support for the
		/// <code>Applet</code> class.  It provides an implementation of the
		/// Java Accessibility API appropriate to applet user-interface elements.
		/// @since 1.3
		/// </summary>
		protected internal class AccessibleApplet : AccessibleAWTPanel
		{
			private readonly Applet OuterInstance;

			public AccessibleApplet(Applet outerInstance) : base(outerInstance)
			{
				this.OuterInstance = outerInstance;
			}


			internal const long SerialVersionUID = 8127374778187708896L;

			/// <summary>
			/// Get the role of this object.
			/// </summary>
			/// <returns> an instance of AccessibleRole describing the role of the
			/// object </returns>
			public override AccessibleRole AccessibleRole
			{
				get
				{
					return AccessibleRole.FRAME;
				}
			}

			/// <summary>
			/// Get the state of this object.
			/// </summary>
			/// <returns> an instance of AccessibleStateSet containing the current
			/// state set of the object </returns>
			/// <seealso cref= AccessibleState </seealso>
			public override AccessibleStateSet AccessibleStateSet
			{
				get
				{
					AccessibleStateSet states = base.AccessibleStateSet;
					states.add(AccessibleState.ACTIVE);
					return states;
				}
			}

		}
	}

}
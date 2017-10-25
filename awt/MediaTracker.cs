using System;
using System.Threading;

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

namespace java.awt
{

	using MultiResolutionToolkitImage = sun.awt.image.MultiResolutionToolkitImage;

	/// <summary>
	/// The <code>MediaTracker</code> class is a utility class to track
	/// the status of a number of media objects. Media objects could
	/// include audio clips as well as images, though currently only
	/// images are supported.
	/// <para>
	/// To use a media tracker, create an instance of
	/// <code>MediaTracker</code> and call its <code>addImage</code>
	/// method for each image to be tracked. In addition, each image can
	/// be assigned a unique identifier. This identifier controls the
	/// priority order in which the images are fetched. It can also be used
	/// to identify unique subsets of the images that can be waited on
	/// independently. Images with a lower ID are loaded in preference to
	/// those with a higher ID number.
	/// 
	/// </para>
	/// <para>
	/// 
	/// Tracking an animated image
	/// might not always be useful
	/// due to the multi-part nature of animated image
	/// loading and painting,
	/// but it is supported.
	/// <code>MediaTracker</code> treats an animated image
	/// as completely loaded
	/// when the first frame is completely loaded.
	/// At that point, the <code>MediaTracker</code>
	/// signals any waiters
	/// that the image is completely loaded.
	/// If no <code>ImageObserver</code>s are observing the image
	/// when the first frame has finished loading,
	/// the image might flush itself
	/// to conserve resources
	/// (see <seealso cref="Image#flush()"/>).
	/// 
	/// </para>
	/// <para>
	/// Here is an example of using <code>MediaTracker</code>:
	/// </para>
	/// <para>
	/// <hr><blockquote><pre>{@code
	/// import java.applet.Applet;
	/// import java.awt.Color;
	/// import java.awt.Image;
	/// import java.awt.Graphics;
	/// import java.awt.MediaTracker;
	/// 
	/// public class ImageBlaster extends Applet implements Runnable {
	///      MediaTracker tracker;
	///      Image bg;
	///      Image anim[] = new Image[5];
	///      int index;
	///      Thread animator;
	/// 
	///      // Get the images for the background (id == 0)
	///      // and the animation frames (id == 1)
	///      // and add them to the MediaTracker
	///      public void init() {
	///          tracker = new MediaTracker(this);
	///          bg = getImage(getDocumentBase(),
	///                  "images/background.gif");
	///          tracker.addImage(bg, 0);
	///          for (int i = 0; i < 5; i++) {
	///              anim[i] = getImage(getDocumentBase(),
	///                      "images/anim"+i+".gif");
	///              tracker.addImage(anim[i], 1);
	///          }
	///      }
	/// 
	///      // Start the animation thread.
	///      public void start() {
	///          animator = new Thread(this);
	///          animator.start();
	///      }
	/// 
	///      // Stop the animation thread.
	///      public void stop() {
	///          animator = null;
	///      }
	/// 
	///      // Run the animation thread.
	///      // First wait for the background image to fully load
	///      // and paint.  Then wait for all of the animation
	///      // frames to finish loading. Finally, loop and
	///      // increment the animation frame index.
	///      public void run() {
	///          try {
	///              tracker.waitForID(0);
	///              tracker.waitForID(1);
	///          } catch (InterruptedException e) {
	///              return;
	///          }
	///          Thread me = Thread.currentThread();
	///          while (animator == me) {
	///              try {
	///                  Thread.sleep(100);
	///              } catch (InterruptedException e) {
	///                  break;
	///              }
	///              synchronized (this) {
	///                  index++;
	///                  if (index >= anim.length) {
	///                      index = 0;
	///                  }
	///              }
	///              repaint();
	///          }
	///      }
	/// 
	///      // The background image fills the frame so we
	///      // don't need to clear the applet on repaints.
	///      // Just call the paint method.
	///      public void update(Graphics g) {
	///          paint(g);
	///      }
	/// 
	///      // Paint a large red rectangle if there are any errors
	///      // loading the images.  Otherwise always paint the
	///      // background so that it appears incrementally as it
	///      // is loading.  Finally, only paint the current animation
	///      // frame if all of the frames (id == 1) are done loading,
	///      // so that we don't get partial animations.
	///      public void paint(Graphics g) {
	///          if ((tracker.statusAll(false) & MediaTracker.ERRORED) != 0) {
	///              g.setColor(Color.red);
	///              g.fillRect(0, 0, size().width, size().height);
	///              return;
	///          }
	///          g.drawImage(bg, 0, 0, this);
	///          if (tracker.statusID(1, false) == MediaTracker.COMPLETE) {
	///              g.drawImage(anim[index], 10, 10, this);
	///          }
	///      }
	/// }
	/// } </pre></blockquote><hr>
	/// 
	/// @author      Jim Graham
	/// @since       JDK1.0
	/// </para>
	/// </summary>
	[Serializable]
	public class MediaTracker
	{

		/// <summary>
		/// A given <code>Component</code> that will be
		/// tracked by a media tracker where the image will
		/// eventually be drawn.
		/// 
		/// @serial </summary>
		/// <seealso cref= #MediaTracker(Component) </seealso>
		internal Component Target;
		/// <summary>
		/// The head of the list of <code>Images</code> that is being
		/// tracked by the <code>MediaTracker</code>.
		/// 
		/// @serial </summary>
		/// <seealso cref= #addImage(Image, int) </seealso>
		/// <seealso cref= #removeImage(Image) </seealso>
		internal MediaEntry Head;

		/*
		 * JDK 1.1 serialVersionUID
		 */
		private const long SerialVersionUID = -483174189758638095L;

		/// <summary>
		/// Creates a media tracker to track images for a given component. </summary>
		/// <param name="comp"> the component on which the images
		///                     will eventually be drawn </param>
		public MediaTracker(Component comp)
		{
			Target = comp;
		}

		/// <summary>
		/// Adds an image to the list of images being tracked by this media
		/// tracker. The image will eventually be rendered at its default
		/// (unscaled) size. </summary>
		/// <param name="image">   the image to be tracked </param>
		/// <param name="id">      an identifier used to track this image </param>
		public virtual void AddImage(Image image, int id)
		{
			AddImage(image, id, -1, -1);
		}

		/// <summary>
		/// Adds a scaled image to the list of images being tracked
		/// by this media tracker. The image will eventually be
		/// rendered at the indicated width and height.
		/// </summary>
		/// <param name="image">   the image to be tracked </param>
		/// <param name="id">   an identifier that can be used to track this image </param>
		/// <param name="w">    the width at which the image is rendered </param>
		/// <param name="h">    the height at which the image is rendered </param>
		public virtual void AddImage(Image image, int id, int w, int h)
		{
			lock (this)
			{
				AddImageImpl(image, id, w, h);
				Image rvImage = GetResolutionVariant(image);
				if (rvImage != null)
				{
					AddImageImpl(rvImage, id, w == -1 ? - 1 : 2 * w, h == -1 ? - 1 : 2 * h);
				}
			}
		}

		private void AddImageImpl(Image image, int id, int w, int h)
		{
			Head = MediaEntry.Insert(Head, new ImageMediaEntry(this, image, id, w, h));
		}
		/// <summary>
		/// Flag indicating that media is currently being loaded. </summary>
		/// <seealso cref=         java.awt.MediaTracker#statusAll </seealso>
		/// <seealso cref=         java.awt.MediaTracker#statusID </seealso>
		public const int LOADING = 1;

		/// <summary>
		/// Flag indicating that the downloading of media was aborted. </summary>
		/// <seealso cref=         java.awt.MediaTracker#statusAll </seealso>
		/// <seealso cref=         java.awt.MediaTracker#statusID </seealso>
		public const int ABORTED = 2;

		/// <summary>
		/// Flag indicating that the downloading of media encountered
		/// an error. </summary>
		/// <seealso cref=         java.awt.MediaTracker#statusAll </seealso>
		/// <seealso cref=         java.awt.MediaTracker#statusID </seealso>
		public const int ERRORED = 4;

		/// <summary>
		/// Flag indicating that the downloading of media was completed
		/// successfully. </summary>
		/// <seealso cref=         java.awt.MediaTracker#statusAll </seealso>
		/// <seealso cref=         java.awt.MediaTracker#statusID </seealso>
		public const int COMPLETE = 8;

		internal static readonly int DONE = (ABORTED | ERRORED | COMPLETE);

		/// <summary>
		/// Checks to see if all images being tracked by this media tracker
		/// have finished loading.
		/// <para>
		/// This method does not start loading the images if they are not
		/// already loading.
		/// </para>
		/// <para>
		/// If there is an error while loading or scaling an image, then that
		/// image is considered to have finished loading. Use the
		/// <code>isErrorAny</code> or <code>isErrorID</code> methods to
		/// check for errors.
		/// </para>
		/// </summary>
		/// <returns>      <code>true</code> if all images have finished loading,
		///                       have been aborted, or have encountered
		///                       an error; <code>false</code> otherwise </returns>
		/// <seealso cref=         java.awt.MediaTracker#checkAll(boolean) </seealso>
		/// <seealso cref=         java.awt.MediaTracker#checkID </seealso>
		/// <seealso cref=         java.awt.MediaTracker#isErrorAny </seealso>
		/// <seealso cref=         java.awt.MediaTracker#isErrorID </seealso>
		public virtual bool CheckAll()
		{
			return CheckAll(false, true);
		}

		/// <summary>
		/// Checks to see if all images being tracked by this media tracker
		/// have finished loading.
		/// <para>
		/// If the value of the <code>load</code> flag is <code>true</code>,
		/// then this method starts loading any images that are not yet
		/// being loaded.
		/// </para>
		/// <para>
		/// If there is an error while loading or scaling an image, that
		/// image is considered to have finished loading. Use the
		/// <code>isErrorAny</code> and <code>isErrorID</code> methods to
		/// check for errors.
		/// </para>
		/// </summary>
		/// <param name="load">   if <code>true</code>, start loading any
		///                       images that are not yet being loaded </param>
		/// <returns>      <code>true</code> if all images have finished loading,
		///                       have been aborted, or have encountered
		///                       an error; <code>false</code> otherwise </returns>
		/// <seealso cref=         java.awt.MediaTracker#checkID </seealso>
		/// <seealso cref=         java.awt.MediaTracker#checkAll() </seealso>
		/// <seealso cref=         java.awt.MediaTracker#isErrorAny() </seealso>
		/// <seealso cref=         java.awt.MediaTracker#isErrorID(int) </seealso>
		public virtual bool CheckAll(bool load)
		{
			return CheckAll(load, true);
		}

		private bool CheckAll(bool load, bool verify)
		{
			lock (this)
			{
				MediaEntry cur = Head;
				bool done = true;
				while (cur != null)
				{
					if ((cur.GetStatus(load, verify) & DONE) == 0)
					{
						done = false;
					}
					cur = cur.Next;
				}
				return done;
			}
		}

		/// <summary>
		/// Checks the error status of all of the images. </summary>
		/// <returns>   <code>true</code> if any of the images tracked
		///                  by this media tracker had an error during
		///                  loading; <code>false</code> otherwise </returns>
		/// <seealso cref=      java.awt.MediaTracker#isErrorID </seealso>
		/// <seealso cref=      java.awt.MediaTracker#getErrorsAny </seealso>
		public virtual bool ErrorAny
		{
			get
			{
				lock (this)
				{
					MediaEntry cur = Head;
					while (cur != null)
					{
						if ((cur.GetStatus(false, true) & ERRORED) != 0)
						{
							return true;
						}
						cur = cur.Next;
					}
					return false;
				}
			}
		}

		/// <summary>
		/// Returns a list of all media that have encountered an error. </summary>
		/// <returns>       an array of media objects tracked by this
		///                        media tracker that have encountered
		///                        an error, or <code>null</code> if
		///                        there are none with errors </returns>
		/// <seealso cref=          java.awt.MediaTracker#isErrorAny </seealso>
		/// <seealso cref=          java.awt.MediaTracker#getErrorsID </seealso>
		public virtual Object[] ErrorsAny
		{
			get
			{
				lock (this)
				{
					MediaEntry cur = Head;
					int numerrors = 0;
					while (cur != null)
					{
						if ((cur.GetStatus(false, true) & ERRORED) != 0)
						{
							numerrors++;
						}
						cur = cur.Next;
					}
					if (numerrors == 0)
					{
						return null;
					}
					Object[] errors = new Object[numerrors];
					cur = Head;
					numerrors = 0;
					while (cur != null)
					{
						if ((cur.GetStatus(false, false) & ERRORED) != 0)
						{
							errors[numerrors++] = cur.Media;
						}
						cur = cur.Next;
					}
					return errors;
				}
			}
		}

		/// <summary>
		/// Starts loading all images tracked by this media tracker. This
		/// method waits until all the images being tracked have finished
		/// loading.
		/// <para>
		/// If there is an error while loading or scaling an image, then that
		/// image is considered to have finished loading. Use the
		/// <code>isErrorAny</code> or <code>isErrorID</code> methods to
		/// check for errors.
		/// </para>
		/// </summary>
		/// <seealso cref=         java.awt.MediaTracker#waitForID(int) </seealso>
		/// <seealso cref=         java.awt.MediaTracker#waitForAll(long) </seealso>
		/// <seealso cref=         java.awt.MediaTracker#isErrorAny </seealso>
		/// <seealso cref=         java.awt.MediaTracker#isErrorID </seealso>
		/// <exception cref="InterruptedException">  if any thread has
		///                                     interrupted this thread </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public void waitForAll() throws InterruptedException
		public virtual void WaitForAll()
		{
			WaitForAll(0);
		}

		/// <summary>
		/// Starts loading all images tracked by this media tracker. This
		/// method waits until all the images being tracked have finished
		/// loading, or until the length of time specified in milliseconds
		/// by the <code>ms</code> argument has passed.
		/// <para>
		/// If there is an error while loading or scaling an image, then
		/// that image is considered to have finished loading. Use the
		/// <code>isErrorAny</code> or <code>isErrorID</code> methods to
		/// check for errors.
		/// </para>
		/// </summary>
		/// <param name="ms">       the number of milliseconds to wait
		///                       for the loading to complete </param>
		/// <returns>      <code>true</code> if all images were successfully
		///                       loaded; <code>false</code> otherwise </returns>
		/// <seealso cref=         java.awt.MediaTracker#waitForID(int) </seealso>
		/// <seealso cref=         java.awt.MediaTracker#waitForAll(long) </seealso>
		/// <seealso cref=         java.awt.MediaTracker#isErrorAny </seealso>
		/// <seealso cref=         java.awt.MediaTracker#isErrorID </seealso>
		/// <exception cref="InterruptedException">  if any thread has
		///                                     interrupted this thread. </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public synchronized boolean waitForAll(long ms) throws InterruptedException
		public virtual bool WaitForAll(long ms)
		{
			lock (this)
			{
				long end = DateTimeHelperClass.CurrentUnixTimeMillis() + ms;
				bool first = true;
				while (true)
				{
					int status = StatusAll(first, first);
					if ((status & LOADING) == 0)
					{
						return (status == COMPLETE);
					}
					first = false;
					long timeout;
					if (ms == 0)
					{
						timeout = 0;
					}
					else
					{
						timeout = end - DateTimeHelperClass.CurrentUnixTimeMillis();
						if (timeout <= 0)
						{
							return false;
						}
					}
					Monitor.Wait(this, TimeSpan.FromMilliseconds(timeout));
				}
			}
		}

		/// <summary>
		/// Calculates and returns the bitwise inclusive <b>OR</b> of the
		/// status of all media that are tracked by this media tracker.
		/// <para>
		/// Possible flags defined by the
		/// <code>MediaTracker</code> class are <code>LOADING</code>,
		/// <code>ABORTED</code>, <code>ERRORED</code>, and
		/// <code>COMPLETE</code>. An image that hasn't started
		/// loading has zero as its status.
		/// </para>
		/// <para>
		/// If the value of <code>load</code> is <code>true</code>, then
		/// this method starts loading any images that are not yet being loaded.
		/// 
		/// </para>
		/// </summary>
		/// <param name="load">   if <code>true</code>, start loading
		///                            any images that are not yet being loaded </param>
		/// <returns>       the bitwise inclusive <b>OR</b> of the status of
		///                            all of the media being tracked </returns>
		/// <seealso cref=          java.awt.MediaTracker#statusID(int, boolean) </seealso>
		/// <seealso cref=          java.awt.MediaTracker#LOADING </seealso>
		/// <seealso cref=          java.awt.MediaTracker#ABORTED </seealso>
		/// <seealso cref=          java.awt.MediaTracker#ERRORED </seealso>
		/// <seealso cref=          java.awt.MediaTracker#COMPLETE </seealso>
		public virtual int StatusAll(bool load)
		{
			return StatusAll(load, true);
		}

		private int StatusAll(bool load, bool verify)
		{
			lock (this)
			{
				MediaEntry cur = Head;
				int status = 0;
				while (cur != null)
				{
					status = status | cur.GetStatus(load, verify);
					cur = cur.Next;
				}
				return status;
			}
		}

		/// <summary>
		/// Checks to see if all images tracked by this media tracker that
		/// are tagged with the specified identifier have finished loading.
		/// <para>
		/// This method does not start loading the images if they are not
		/// already loading.
		/// </para>
		/// <para>
		/// If there is an error while loading or scaling an image, then that
		/// image is considered to have finished loading. Use the
		/// <code>isErrorAny</code> or <code>isErrorID</code> methods to
		/// check for errors.
		/// </para>
		/// </summary>
		/// <param name="id">   the identifier of the images to check </param>
		/// <returns>      <code>true</code> if all images have finished loading,
		///                       have been aborted, or have encountered
		///                       an error; <code>false</code> otherwise </returns>
		/// <seealso cref=         java.awt.MediaTracker#checkID(int, boolean) </seealso>
		/// <seealso cref=         java.awt.MediaTracker#checkAll() </seealso>
		/// <seealso cref=         java.awt.MediaTracker#isErrorAny() </seealso>
		/// <seealso cref=         java.awt.MediaTracker#isErrorID(int) </seealso>
		public virtual bool CheckID(int id)
		{
			return CheckID(id, false, true);
		}

		/// <summary>
		/// Checks to see if all images tracked by this media tracker that
		/// are tagged with the specified identifier have finished loading.
		/// <para>
		/// If the value of the <code>load</code> flag is <code>true</code>,
		/// then this method starts loading any images that are not yet
		/// being loaded.
		/// </para>
		/// <para>
		/// If there is an error while loading or scaling an image, then that
		/// image is considered to have finished loading. Use the
		/// <code>isErrorAny</code> or <code>isErrorID</code> methods to
		/// check for errors.
		/// </para>
		/// </summary>
		/// <param name="id">       the identifier of the images to check </param>
		/// <param name="load">     if <code>true</code>, start loading any
		///                       images that are not yet being loaded </param>
		/// <returns>      <code>true</code> if all images have finished loading,
		///                       have been aborted, or have encountered
		///                       an error; <code>false</code> otherwise </returns>
		/// <seealso cref=         java.awt.MediaTracker#checkID(int, boolean) </seealso>
		/// <seealso cref=         java.awt.MediaTracker#checkAll() </seealso>
		/// <seealso cref=         java.awt.MediaTracker#isErrorAny() </seealso>
		/// <seealso cref=         java.awt.MediaTracker#isErrorID(int) </seealso>
		public virtual bool CheckID(int id, bool load)
		{
			return CheckID(id, load, true);
		}

		private bool CheckID(int id, bool load, bool verify)
		{
			lock (this)
			{
				MediaEntry cur = Head;
				bool done = true;
				while (cur != null)
				{
					if (cur.ID == id && (cur.GetStatus(load, verify) & DONE) == 0)
					{
						done = false;
					}
					cur = cur.Next;
				}
				return done;
			}
		}

		/// <summary>
		/// Checks the error status of all of the images tracked by this
		/// media tracker with the specified identifier. </summary>
		/// <param name="id">   the identifier of the images to check </param>
		/// <returns>       <code>true</code> if any of the images with the
		///                          specified identifier had an error during
		///                          loading; <code>false</code> otherwise </returns>
		/// <seealso cref=          java.awt.MediaTracker#isErrorAny </seealso>
		/// <seealso cref=          java.awt.MediaTracker#getErrorsID </seealso>
		public virtual bool IsErrorID(int id)
		{
			lock (this)
			{
				MediaEntry cur = Head;
				while (cur != null)
				{
					if (cur.ID == id && (cur.GetStatus(false, true) & ERRORED) != 0)
					{
						return true;
					}
					cur = cur.Next;
				}
				return false;
			}
		}

		/// <summary>
		/// Returns a list of media with the specified ID that
		/// have encountered an error. </summary>
		/// <param name="id">   the identifier of the images to check </param>
		/// <returns>      an array of media objects tracked by this media
		///                       tracker with the specified identifier
		///                       that have encountered an error, or
		///                       <code>null</code> if there are none with errors </returns>
		/// <seealso cref=         java.awt.MediaTracker#isErrorID </seealso>
		/// <seealso cref=         java.awt.MediaTracker#isErrorAny </seealso>
		/// <seealso cref=         java.awt.MediaTracker#getErrorsAny </seealso>
		public virtual Object[] GetErrorsID(int id)
		{
			lock (this)
			{
				MediaEntry cur = Head;
				int numerrors = 0;
				while (cur != null)
				{
					if (cur.ID == id && (cur.GetStatus(false, true) & ERRORED) != 0)
					{
						numerrors++;
					}
					cur = cur.Next;
				}
				if (numerrors == 0)
				{
					return null;
				}
				Object[] errors = new Object[numerrors];
				cur = Head;
				numerrors = 0;
				while (cur != null)
				{
					if (cur.ID == id && (cur.GetStatus(false, false) & ERRORED) != 0)
					{
						errors[numerrors++] = cur.Media;
					}
					cur = cur.Next;
				}
				return errors;
			}
		}

		/// <summary>
		/// Starts loading all images tracked by this media tracker with the
		/// specified identifier. This method waits until all the images with
		/// the specified identifier have finished loading.
		/// <para>
		/// If there is an error while loading or scaling an image, then that
		/// image is considered to have finished loading. Use the
		/// <code>isErrorAny</code> and <code>isErrorID</code> methods to
		/// check for errors.
		/// </para>
		/// </summary>
		/// <param name="id">   the identifier of the images to check </param>
		/// <seealso cref=           java.awt.MediaTracker#waitForAll </seealso>
		/// <seealso cref=           java.awt.MediaTracker#isErrorAny() </seealso>
		/// <seealso cref=           java.awt.MediaTracker#isErrorID(int) </seealso>
		/// <exception cref="InterruptedException">  if any thread has
		///                          interrupted this thread. </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public void waitForID(int id) throws InterruptedException
		public virtual void WaitForID(int id)
		{
			WaitForID(id, 0);
		}

		/// <summary>
		/// Starts loading all images tracked by this media tracker with the
		/// specified identifier. This method waits until all the images with
		/// the specified identifier have finished loading, or until the
		/// length of time specified in milliseconds by the <code>ms</code>
		/// argument has passed.
		/// <para>
		/// If there is an error while loading or scaling an image, then that
		/// image is considered to have finished loading. Use the
		/// <code>statusID</code>, <code>isErrorID</code>, and
		/// <code>isErrorAny</code> methods to check for errors.
		/// </para>
		/// </summary>
		/// <param name="id">   the identifier of the images to check </param>
		/// <param name="ms">   the length of time, in milliseconds, to wait
		///                           for the loading to complete </param>
		/// <seealso cref=           java.awt.MediaTracker#waitForAll </seealso>
		/// <seealso cref=           java.awt.MediaTracker#waitForID(int) </seealso>
		/// <seealso cref=           java.awt.MediaTracker#statusID </seealso>
		/// <seealso cref=           java.awt.MediaTracker#isErrorAny() </seealso>
		/// <seealso cref=           java.awt.MediaTracker#isErrorID(int) </seealso>
		/// <exception cref="InterruptedException">  if any thread has
		///                          interrupted this thread. </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public synchronized boolean waitForID(int id, long ms) throws InterruptedException
		public virtual bool WaitForID(int id, long ms)
		{
			lock (this)
			{
				long end = DateTimeHelperClass.CurrentUnixTimeMillis() + ms;
				bool first = true;
				while (true)
				{
					int status = StatusID(id, first, first);
					if ((status & LOADING) == 0)
					{
						return (status == COMPLETE);
					}
					first = false;
					long timeout;
					if (ms == 0)
					{
						timeout = 0;
					}
					else
					{
						timeout = end - DateTimeHelperClass.CurrentUnixTimeMillis();
						if (timeout <= 0)
						{
							return false;
						}
					}
					Monitor.Wait(this, TimeSpan.FromMilliseconds(timeout));
				}
			}
		}

		/// <summary>
		/// Calculates and returns the bitwise inclusive <b>OR</b> of the
		/// status of all media with the specified identifier that are
		/// tracked by this media tracker.
		/// <para>
		/// Possible flags defined by the
		/// <code>MediaTracker</code> class are <code>LOADING</code>,
		/// <code>ABORTED</code>, <code>ERRORED</code>, and
		/// <code>COMPLETE</code>. An image that hasn't started
		/// loading has zero as its status.
		/// </para>
		/// <para>
		/// If the value of <code>load</code> is <code>true</code>, then
		/// this method starts loading any images that are not yet being loaded.
		/// </para>
		/// </summary>
		/// <param name="id">   the identifier of the images to check </param>
		/// <param name="load">   if <code>true</code>, start loading
		///                            any images that are not yet being loaded </param>
		/// <returns>       the bitwise inclusive <b>OR</b> of the status of
		///                            all of the media with the specified
		///                            identifier that are being tracked </returns>
		/// <seealso cref=          java.awt.MediaTracker#statusAll(boolean) </seealso>
		/// <seealso cref=          java.awt.MediaTracker#LOADING </seealso>
		/// <seealso cref=          java.awt.MediaTracker#ABORTED </seealso>
		/// <seealso cref=          java.awt.MediaTracker#ERRORED </seealso>
		/// <seealso cref=          java.awt.MediaTracker#COMPLETE </seealso>
		public virtual int StatusID(int id, bool load)
		{
			return StatusID(id, load, true);
		}

		private int StatusID(int id, bool load, bool verify)
		{
			lock (this)
			{
				MediaEntry cur = Head;
				int status = 0;
				while (cur != null)
				{
					if (cur.ID == id)
					{
						status = status | cur.GetStatus(load, verify);
					}
					cur = cur.Next;
				}
				return status;
			}
		}

		/// <summary>
		/// Removes the specified image from this media tracker.
		/// All instances of the specified image are removed,
		/// regardless of scale or ID. </summary>
		/// <param name="image">     the image to be removed </param>
		/// <seealso cref=     java.awt.MediaTracker#removeImage(java.awt.Image, int) </seealso>
		/// <seealso cref=     java.awt.MediaTracker#removeImage(java.awt.Image, int, int, int)
		/// @since   JDK1.1 </seealso>
		public virtual void RemoveImage(Image image)
		{
			lock (this)
			{
				RemoveImageImpl(image);
				Image rvImage = GetResolutionVariant(image);
				if (rvImage != null)
				{
					RemoveImageImpl(rvImage);
				}
				Monitor.PulseAll(this); // Notify in case remaining images are "done".
			}
		}

		private void RemoveImageImpl(Image image)
		{
			MediaEntry cur = Head;
			MediaEntry prev = null;
			while (cur != null)
			{
				MediaEntry next = cur.Next;
				if (cur.Media == image)
				{
					if (prev == null)
					{
						Head = next;
					}
					else
					{
						prev.Next = next;
					}
					cur.Cancel();
				}
				else
				{
					prev = cur;
				}
				cur = next;
			}
		}

		/// <summary>
		/// Removes the specified image from the specified tracking
		/// ID of this media tracker.
		/// All instances of <code>Image</code> being tracked
		/// under the specified ID are removed regardless of scale. </summary>
		/// <param name="image"> the image to be removed </param>
		/// <param name="id"> the tracking ID from which to remove the image </param>
		/// <seealso cref=        java.awt.MediaTracker#removeImage(java.awt.Image) </seealso>
		/// <seealso cref=        java.awt.MediaTracker#removeImage(java.awt.Image, int, int, int)
		/// @since      JDK1.1 </seealso>
		public virtual void RemoveImage(Image image, int id)
		{
			lock (this)
			{
				RemoveImageImpl(image, id);
				Image rvImage = GetResolutionVariant(image);
				if (rvImage != null)
				{
					RemoveImageImpl(rvImage, id);
				}
				Monitor.PulseAll(this); // Notify in case remaining images are "done".
			}
		}

		private void RemoveImageImpl(Image image, int id)
		{
			MediaEntry cur = Head;
			MediaEntry prev = null;
			while (cur != null)
			{
				MediaEntry next = cur.Next;
				if (cur.ID == id && cur.Media == image)
				{
					if (prev == null)
					{
						Head = next;
					}
					else
					{
						prev.Next = next;
					}
					cur.Cancel();
				}
				else
				{
					prev = cur;
				}
				cur = next;
			}
		}

		/// <summary>
		/// Removes the specified image with the specified
		/// width, height, and ID from this media tracker.
		/// Only the specified instance (with any duplicates) is removed. </summary>
		/// <param name="image"> the image to be removed </param>
		/// <param name="id"> the tracking ID from which to remove the image </param>
		/// <param name="width"> the width to remove (-1 for unscaled) </param>
		/// <param name="height"> the height to remove (-1 for unscaled) </param>
		/// <seealso cref=     java.awt.MediaTracker#removeImage(java.awt.Image) </seealso>
		/// <seealso cref=     java.awt.MediaTracker#removeImage(java.awt.Image, int)
		/// @since   JDK1.1 </seealso>
		public virtual void RemoveImage(Image image, int id, int width, int height)
		{
			lock (this)
			{
				RemoveImageImpl(image, id, width, height);
				Image rvImage = GetResolutionVariant(image);
				if (rvImage != null)
				{
					RemoveImageImpl(rvImage, id, width == -1 ? - 1 : 2 * width, height == -1 ? - 1 : 2 * height);
				}
				Monitor.PulseAll(this); // Notify in case remaining images are "done".
			}
		}

		private void RemoveImageImpl(Image image, int id, int width, int height)
		{
			MediaEntry cur = Head;
			MediaEntry prev = null;
			while (cur != null)
			{
				MediaEntry next = cur.Next;
				if (cur.ID == id && cur is ImageMediaEntry && ((ImageMediaEntry) cur).Matches(image, width, height))
				{
					if (prev == null)
					{
						Head = next;
					}
					else
					{
						prev.Next = next;
					}
					cur.Cancel();
				}
				else
				{
					prev = cur;
				}
				cur = next;
			}
		}

		internal virtual void SetDone()
		{
			lock (this)
			{
				Monitor.PulseAll(this);
			}
		}

		private static Image GetResolutionVariant(Image image)
		{
			if (image is MultiResolutionToolkitImage)
			{
				return ((MultiResolutionToolkitImage) image).ResolutionVariant;
			}
			return null;
		}
	}

	internal abstract class MediaEntry
	{
		internal MediaTracker Tracker;
		internal int ID_Renamed;
		internal MediaEntry Next;

		internal int Status_Renamed;
		internal bool Cancelled;

		internal MediaEntry(MediaTracker mt, int id)
		{
			Tracker = mt;
			ID_Renamed = id;
		}

		internal abstract Object Media {get;}

		internal static MediaEntry Insert(MediaEntry head, MediaEntry me)
		{
			MediaEntry cur = head;
			MediaEntry prev = null;
			while (cur != null)
			{
				if (cur.ID_Renamed > me.ID_Renamed)
				{
					break;
				}
				prev = cur;
				cur = cur.Next;
			}
			me.Next = cur;
			if (prev == null)
			{
				head = me;
			}
			else
			{
				prev.Next = me;
			}
			return head;
		}

		internal virtual int ID
		{
			get
			{
				return ID_Renamed;
			}
		}

		internal abstract void StartLoad();

		internal virtual void Cancel()
		{
			Cancelled = true;
		}

		internal const int LOADING = MediaTracker.LOADING;
		internal const int ABORTED = MediaTracker.ABORTED;
		internal const int ERRORED = MediaTracker.ERRORED;
		internal const int COMPLETE = MediaTracker.COMPLETE;

		internal static readonly int LOADSTARTED = (LOADING | ERRORED | COMPLETE);
		internal static readonly int DONE = (ABORTED | ERRORED | COMPLETE);

		internal virtual int GetStatus(bool doLoad, bool doVerify)
		{
			lock (this)
			{
				if (doLoad && ((Status_Renamed & LOADSTARTED) == 0))
				{
					Status_Renamed = (Status_Renamed & ~ABORTED) | LOADING;
					StartLoad();
				}
				return Status_Renamed;
			}
		}

		internal virtual int Status
		{
			set
			{
				lock (this)
				{
					Status_Renamed = value;
				}
				Tracker.SetDone();
			}
		}
	}

	[Serializable]
	internal class ImageMediaEntry : MediaEntry, ImageObserver
	{
		internal Image Image;
		internal int Width;
		internal int Height;

		/*
		 * JDK 1.1 serialVersionUID
		 */
		private const long SerialVersionUID = 4739377000350280650L;

		internal ImageMediaEntry(MediaTracker mt, Image img, int c, int w, int h) : base(mt, c)
		{
			Image = img;
			Width = w;
			Height = h;
		}

		internal virtual bool Matches(Image img, int w, int h)
		{
			return (Image == img && Width == w && Height == h);
		}

		internal override Object Media
		{
			get
			{
				return Image;
			}
		}

		internal override int GetStatus(bool doLoad, bool doVerify)
		{
			lock (this)
			{
				if (doVerify)
				{
					int flags = Tracker.Target.CheckImage(Image, Width, Height, null);
					int s = Parseflags(flags);
					if (s == 0)
					{
						if ((Status_Renamed & (ERRORED | COMPLETE)) != 0)
						{
							Status = ABORTED;
						}
					}
					else if (s != Status_Renamed)
					{
						Status = s;
					}
				}
				return base.GetStatus(doLoad, doVerify);
			}
		}

		internal override void StartLoad()
		{
			if (Tracker.Target.PrepareImage(Image, Width, Height, this))
			{
				Status = COMPLETE;
			}
		}

		internal virtual int Parseflags(int infoflags)
		{
			if ((infoflags & Image.ImageObserver_Fields.ERROR) != 0)
			{
				return ERRORED;
			}
			else if ((infoflags & Image.ImageObserver_Fields.ABORT) != 0)
			{
				return ABORTED;
			}
			else if ((infoflags & (Image.ImageObserver_Fields.ALLBITS | Image.ImageObserver_Fields.FRAMEBITS)) != 0)
			{
				return COMPLETE;
			}
			return 0;
		}

		public virtual bool ImageUpdate(Image img, int infoflags, int x, int y, int w, int h)
		{
			if (Cancelled)
			{
				return false;
			}
			int s = Parseflags(infoflags);
			if (s != 0 && s != Status_Renamed)
			{
				Status = s;
			}
			return ((Status_Renamed & LOADING) != 0);
		}
	}

}
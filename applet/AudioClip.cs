/*
 * Copyright (c) 1995, 1997, Oracle and/or its affiliates. All rights reserved.
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
	/// The <code>AudioClip</code> interface is a simple abstraction for
	/// playing a sound clip. Multiple <code>AudioClip</code> items can be
	/// playing at the same time, and the resulting sound is mixed
	/// together to produce a composite.
	/// 
	/// @author      Arthur van Hoff
	/// @since       JDK1.0
	/// </summary>
	public interface AudioClip
	{
		/// <summary>
		/// Starts playing this audio clip. Each time this method is called,
		/// the clip is restarted from the beginning.
		/// </summary>
		void Play();

		/// <summary>
		/// Starts playing this audio clip in a loop.
		/// </summary>
		void Loop();

		/// <summary>
		/// Stops playing this audio clip.
		/// </summary>
		void Stop();
	}

}
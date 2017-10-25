/*
 * Copyright (c) 1996, Oracle and/or its affiliates. All rights reserved.
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

	/// <summary>
	/// An abstract class which initiates and executes a print job.
	/// It provides access to a print graphics object which renders
	/// to an appropriate print device.
	/// </summary>
	/// <seealso cref= Toolkit#getPrintJob
	/// 
	/// @author      Amy Fowler </seealso>
	public abstract class PrintJob
	{

		/// <summary>
		/// Gets a Graphics object that will draw to the next page.
		/// The page is sent to the printer when the graphics
		/// object is disposed.  This graphics object will also implement
		/// the PrintGraphics interface. </summary>
		/// <seealso cref= PrintGraphics </seealso>
		public abstract Graphics Graphics {get;}

		/// <summary>
		/// Returns the dimensions of the page in pixels.
		/// The resolution of the page is chosen so that it
		/// is similar to the screen resolution.
		/// </summary>
		public abstract Dimension PageDimension {get;}

		/// <summary>
		/// Returns the resolution of the page in pixels per inch.
		/// Note that this doesn't have to correspond to the physical
		/// resolution of the printer.
		/// </summary>
		public abstract int PageResolution {get;}

		/// <summary>
		/// Returns true if the last page will be printed first.
		/// </summary>
		public abstract bool LastPageFirst();

		/// <summary>
		/// Ends the print job and does any necessary cleanup.
		/// </summary>
		public abstract void End();

		/// <summary>
		/// Ends this print job once it is no longer referenced. </summary>
		/// <seealso cref= #end </seealso>
		~PrintJob()
		{
			End();
		}

	}

}
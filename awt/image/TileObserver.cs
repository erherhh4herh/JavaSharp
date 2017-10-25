/*
 * Copyright (c) 1997, 1998, Oracle and/or its affiliates. All rights reserved.
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

namespace java.awt.image
{

	/// <summary>
	/// An interface for objects that wish to be informed when tiles
	/// of a WritableRenderedImage become modifiable by some writer via
	/// a call to getWritableTile, and when they become unmodifiable via
	/// the last call to releaseWritableTile.
	/// </summary>
	/// <seealso cref= WritableRenderedImage
	///  
	/// @author Thomas DeWeese
	/// @author Daniel Rice </seealso>
	public interface TileObserver
	{

	  /// <summary>
	  /// A tile is about to be updated (it is either about to be grabbed
	  /// for writing, or it is being released from writing).
	  /// </summary>
	  /// <param name="source"> the image that owns the tile. </param>
	  /// <param name="tileX"> the X index of the tile that is being updated. </param>
	  /// <param name="tileY"> the Y index of the tile that is being updated. </param>
	  /// <param name="willBeWritable">  If true, the tile will be grabbed for writing;
	  ///                        otherwise it is being released. </param>
		void TileUpdate(WritableRenderedImage source, int tileX, int tileY, bool willBeWritable);

	}

}
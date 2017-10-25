/*
 * Copyright (c) 1996, 1998, Oracle and/or its affiliates. All rights reserved.
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

/*
 * (C) Copyright Taligent, Inc. 1996 - All Rights Reserved
 * (C) Copyright IBM Corp. 1996 - All Rights Reserved
 *
 *   The original version of this source code and documentation is copyrighted
 * and owned by Taligent, Inc., a wholly-owned subsidiary of IBM. These
 * materials are provided under terms of a License Agreement between Taligent
 * and Sun. This technology is protected by multiple US and International
 * patents. This notice and attribution to Taligent may not be removed.
 *   Taligent is a registered trademark of Taligent, Inc.
 *
 */

namespace java.text
{

	/// <summary>
	/// This is used for building contracting character tables.  entryName
	/// is the contracting character name and value is its collation
	/// order.
	/// </summary>
	internal sealed class EntryPair
	{
		public String EntryName;
		public int Value;
		public bool Fwd;

		public EntryPair(String name, int value) : this(name, value, true)
		{
		}
		public EntryPair(String name, int value, bool fwd)
		{
			this.EntryName = name;
			this.Value = value;
			this.Fwd = fwd;
		}
	}

}
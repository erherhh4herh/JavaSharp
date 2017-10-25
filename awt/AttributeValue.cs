/*
 * Copyright (c) 1999, 2013, Oracle and/or its affiliates. All rights reserved.
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

	using PlatformLogger = sun.util.logging.PlatformLogger;

	internal abstract class AttributeValue
	{
		private static readonly PlatformLogger Log = PlatformLogger.getLogger("java.awt.AttributeValue");
		private readonly int Value;
		private readonly String[] Names;

		protected internal AttributeValue(int value, String[] names)
		{
			if (Log.isLoggable(PlatformLogger.Level.FINEST))
			{
				Log.finest("value = " + value + ", names = " + names);
			}

			if (Log.isLoggable(PlatformLogger.Level.FINER))
			{
				if ((value < 0) || (names == null) || (value >= names.Length))
				{
					Log.finer("Assertion failed");
				}
			}
			this.Value = value;
			this.Names = names;
		}
		// This hashCode is used by the sun.awt implementation as an array
		// index.
		public override int HashCode()
		{
			return Value;
		}
		public override String ToString()
		{
			return Names[Value];
		}
	}

}
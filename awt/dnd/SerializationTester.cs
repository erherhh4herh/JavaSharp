/*
 * Copyright (c) 2001, Oracle and/or its affiliates. All rights reserved.
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

namespace java.awt.dnd
{


	/// <summary>
	/// Tests if an object can truly be serialized by serializing it to a null
	/// OutputStream.
	/// 
	/// @since 1.4
	/// </summary>
	internal sealed class SerializationTester
	{
		private static ObjectOutputStream Stream;
		static SerializationTester()
		{
			try
			{
				Stream = new ObjectOutputStream(new OutputStreamAnonymousInnerClassHelper());
			}
			catch (IOException)
			{
			}
		}

		private class OutputStreamAnonymousInnerClassHelper : OutputStream
		{
			public OutputStreamAnonymousInnerClassHelper()
			{
			}

			public override void Write(int b)
			{
			}
		}

		internal static bool Test(Object obj)
		{
			if (!(obj is Serializable))
			{
				return false;
			}

			try
			{
				Stream.WriteObject(obj);
			}
			catch (IOException)
			{
				return false;
			}
			finally
			{
				// Fix for 4503661.
				// Reset the stream so that it doesn't keep a reference to the
				// written object.
				try
				{
					Stream.Reset();
				}
				catch (IOException)
				{
					// Ignore the exception.
				}
			}
			return true;
		}

		private SerializationTester()
		{
		}
	}

}
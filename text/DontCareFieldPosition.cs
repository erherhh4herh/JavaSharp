/*
 * Copyright (c) 2002, Oracle and/or its affiliates. All rights reserved.
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

namespace java.text
{

	/// <summary>
	/// DontCareFieldPosition defines no-op FieldDelegate. Its
	/// singleton is used for the format methods that don't take a
	/// FieldPosition.
	/// </summary>
	internal class DontCareFieldPosition : FieldPosition
	{
		// The singleton of DontCareFieldPosition.
		internal static readonly FieldPosition INSTANCE = new DontCareFieldPosition();

		private readonly Format.FieldDelegate noDelegate = new FieldDelegateAnonymousInnerClassHelper();

		private class FieldDelegateAnonymousInnerClassHelper : Format.FieldDelegate
		{
			public FieldDelegateAnonymousInnerClassHelper()
			{
			}

			public virtual void Formatted(Format.Field attr, Object value, int start, int end, StringBuffer buffer)
			{
			}
			public virtual void Formatted(int fieldID, Format.Field attr, Object value, int start, int end, StringBuffer buffer)
			{
			}
		}

		private DontCareFieldPosition() : base(0)
		{
		}

		internal override Format.FieldDelegate FieldDelegate
		{
			get
			{
				return noDelegate;
			}
		}
	}

}
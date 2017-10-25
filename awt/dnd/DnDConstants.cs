/*
 * Copyright (c) 1997, 2013, Oracle and/or its affiliates. All rights reserved.
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
	/// This class contains constant values representing
	/// the type of action(s) to be performed by a Drag and Drop operation.
	/// @since 1.2
	/// </summary>
	public sealed class DnDConstants
	{

		private DnDConstants() // define null private constructor.
		{
		}

		/// <summary>
		/// An <code>int</code> representing no action.
		/// </summary>
//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Native public static final int ACTION_NONE = 0x0;
		public const int ACTION_NONE = 0x0;

		/// <summary>
		/// An <code>int</code> representing a &quot;copy&quot; action.
		/// </summary>
//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Native public static final int ACTION_COPY = 0x1;
		public const int ACTION_COPY = 0x1;

		/// <summary>
		/// An <code>int</code> representing a &quot;move&quot; action.
		/// </summary>
//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Native public static final int ACTION_MOVE = 0x2;
		public const int ACTION_MOVE = 0x2;

		/// <summary>
		/// An <code>int</code> representing a &quot;copy&quot; or
		/// &quot;move&quot; action.
		/// </summary>
//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Native public static final int ACTION_COPY_OR_MOVE = ACTION_COPY | ACTION_MOVE;
		public static readonly int ACTION_COPY_OR_MOVE = ACTION_COPY | ACTION_MOVE;

		/// <summary>
		/// An <code>int</code> representing a &quot;link&quot; action.
		/// 
		/// The link verb is found in many, if not all native DnD platforms, and the
		/// actual interpretation of LINK semantics is both platform
		/// and application dependent. Broadly speaking, the
		/// semantic is "do not copy, or move the operand, but create a reference
		/// to it". Defining the meaning of "reference" is where ambiguity is
		/// introduced.
		/// 
		/// The verb is provided for completeness, but its use is not recommended
		/// for DnD operations between logically distinct applications where
		/// misinterpretation of the operations semantics could lead to confusing
		/// results for the user.
		/// </summary>

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Native public static final int ACTION_LINK = 0x40000000;
		public const int ACTION_LINK = 0x40000000;

		/// <summary>
		/// An <code>int</code> representing a &quot;reference&quot;
		/// action (synonym for ACTION_LINK).
		/// </summary>
//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Native public static final int ACTION_REFERENCE = ACTION_LINK;
		public const int ACTION_REFERENCE = ACTION_LINK;

	}

}
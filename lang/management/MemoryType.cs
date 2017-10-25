using System.Collections.Generic;

/*
 * Copyright (c) 2003, 2004, Oracle and/or its affiliates. All rights reserved.
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

namespace java.lang.management
{

	/// <summary>
	/// Types of <seealso cref="MemoryPoolMXBean memory pools"/>.
	/// 
	/// @author  Mandy Chung
	/// @since   1.5
	/// </summary>
	public sealed class MemoryType
	{

		/// <summary>
		/// Heap memory type.
		/// <para>
		/// The Java virtual machine has a <i>heap</i>
		/// that is the runtime data area from which
		/// memory for all class instances and arrays are allocated.
		/// </para>
		/// </summary>
		public static readonly MemoryType HEAP = new MemoryType("HEAP", InnerEnum.HEAP, "Heap memory");

		/// <summary>
		/// Non-heap memory type.
		/// <para>
		/// The Java virtual machine manages memory other than the heap
		/// (referred as <i>non-heap memory</i>).  The non-heap memory includes
		/// the <i>method area</i> and memory required for the internal
		/// processing or optimization for the Java virtual machine.
		/// It stores per-class structures such as a runtime
		/// constant pool, field and method data, and the code for
		/// methods and constructors.
		/// </para>
		/// </summary>
		public static readonly MemoryType NON_HEAP = new MemoryType("NON_HEAP", InnerEnum.NON_HEAP, "Non-heap memory");

		private static readonly IList<MemoryType> valueList = new List<MemoryType>();

		static MemoryType()
		{
			valueList.Add(HEAP);
			valueList.Add(NON_HEAP);
		}

		public enum InnerEnum
		{
			HEAP,
			NON_HEAP
		}

		private readonly string nameValue;
		private readonly int ordinalValue;
		private readonly InnerEnum innerEnumValue;
		private static int nextOrdinal = 0;

		private readonly String description;

		private MemoryType(string name, InnerEnum innerEnum, String s)
		{
			this.description = s;

			nameValue = name;
			ordinalValue = nextOrdinal++;
			innerEnumValue = innerEnum;
		}

		/// <summary>
		/// Returns the string representation of this <tt>MemoryType</tt>. </summary>
		/// <returns> the string representation of this <tt>MemoryType</tt>. </returns>
		public override String ToString()
		{
			return description;
		}

		private const long serialVersionUID = 6992337162326171013L;

		public static IList<MemoryType> values()
		{
			return valueList;
		}

		public InnerEnum InnerEnumValue()
		{
			return innerEnumValue;
		}

		public int ordinal()
		{
			return ordinalValue;
		}

		public override string ToString()
		{
			return nameValue;
		}

		public static MemoryType valueOf(string name)
		{
			foreach (MemoryType enumInstance in MemoryType.values())
			{
				if (enumInstance.nameValue == name)
				{
					return enumInstance;
				}
			}
			throw new System.ArgumentException(name);
		}
	}

}
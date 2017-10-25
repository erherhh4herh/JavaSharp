using System;

/*
 * Copyright (c) 2012, 2013, Oracle and/or its affiliates. All rights reserved.
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
namespace java.util
{


	/// <summary>
	/// Package private supporting class for <seealso cref="Comparator"/>.
	/// </summary>
	internal class Comparators
	{
		private Comparators()
		{
			throw new AssertionError("no instances");
		}

		/// <summary>
		/// Compares <seealso cref="Comparable"/> objects in natural order.
		/// </summary>
		/// <seealso cref= Comparable </seealso>
//JAVA TO C# CONVERTER TODO TASK: Enums cannot implement interfaces in .NET:
//ORIGINAL LINE: enum NaturalOrderComparator implements Comparator<Comparable<Object>>
		internal enum NaturalOrderComparator
		{
			INSTANCE

			public int compare(Comparable<Object> c1, Comparable<Object> c2)
			{
				return = c2
			}

			public Comparator<Comparable<Object>> reversed()
			{
				return = 
			}
		}

		/// <summary>
		/// Null-friendly comparators
		/// </summary>
		[Serializable]
		internal sealed class NullComparator<T> : Comparator<T>
		{
			internal const long SerialVersionUID = -7569533591570686392L;
			internal readonly bool NullFirst;
			// if null, non-null Ts are considered equal
			internal readonly Comparator<T> Real;

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @SuppressWarnings("unchecked") NullComparator(boolean nullFirst, Comparator<? base T> real)
//JAVA TO C# CONVERTER TODO TASK: There is no .NET equivalent to the Java 'super' constraint:
			internal NullComparator<T1>(bool nullFirst, Comparator<T1> real)
			{
				this.NullFirst = nullFirst;
				this.Real = (Comparator<T>) real;
			}

			public int Compare(T a, T b)
			{
				if (a == null)
				{
					return (b == null) ? 0 : (NullFirst ? - 1 : 1);
				}
				else if (b == null)
				{
					return NullFirst ? 1: -1;
				}
				else
				{
					return (Real == null) ? 0 : Real.Compare(a, b);
				}
			}

//JAVA TO C# CONVERTER TODO TASK: There is no .NET equivalent to the Java 'super' constraint:
//ORIGINAL LINE: @Override public Comparator<T> thenComparing(Comparator<? base T> other)
			public override Comparator<T> ThenComparing(Comparator<T1> other)
			{
				Objects.RequireNonNull(other);
				return new NullComparator<>(NullFirst, Real == null ? other : Real.thenComparing(other));
			}

			public override Comparator<T> Reversed()
			{
				return new NullComparator<>(!NullFirst, Real == null ? null : Real.reversed());
			}
		}
	}

}
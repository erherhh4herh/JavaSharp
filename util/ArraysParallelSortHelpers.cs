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
	/// Helper utilities for the parallel sort methods in Arrays.parallelSort.
	/// 
	/// For each primitive type, plus Object, we define a static class to
	/// contain the Sorter and Merger implementations for that type:
	/// 
	/// Sorter classes based mainly on CilkSort
	/// <A href="http://supertech.lcs.mit.edu/cilk/"> Cilk</A>:
	/// Basic algorithm:
	/// if array size is small, just use a sequential quicksort (via Arrays.sort)
	///         Otherwise:
	///         1. Break array in half.
	///         2. For each half,
	///             a. break the half in half (i.e., quarters),
	///             b. sort the quarters
	///             c. merge them together
	///         3. merge together the two halves.
	/// 
	/// One reason for splitting in quarters is that this guarantees that
	/// the final sort is in the main array, not the workspace array.
	/// (workspace and main swap roles on each subsort step.)  Leaf-level
	/// sorts use the associated sequential sort.
	/// 
	/// Merger classes perform merging for Sorter.  They are structured
	/// such that if the underlying sort is stable (as is true for
	/// TimSort), then so is the full sort.  If big enough, they split the
	/// largest of the two partitions in half, find the greatest point in
	/// smaller partition less than the beginning of the second half of
	/// larger via binary search; and then merge in parallel the two
	/// partitions.  In part to ensure tasks are triggered in
	/// stability-preserving order, the current CountedCompleter design
	/// requires some little tasks to serve as place holders for triggering
	/// completion tasks.  These classes (EmptyCompleter and Relay) don't
	/// need to keep track of the arrays, and are never themselves forked,
	/// so don't hold any task state.
	/// 
	/// The primitive class versions (FJByte... FJDouble) are
	/// identical to each other except for type declarations.
	/// 
	/// The base sequential sorts rely on non-public versions of TimSort,
	/// ComparableTimSort, and DualPivotQuicksort sort methods that accept
	/// temp workspace array slices that we will have already allocated, so
	/// avoids redundant allocation. (Except for DualPivotQuicksort byte[]
	/// sort, that does not ever use a workspace array.)
	/// </summary>
	/*package*/	 internal class ArraysParallelSortHelpers
	 {

		/*
		 * Style note: The task classes have a lot of parameters, that are
		 * stored as task fields and copied to local variables and used in
		 * compute() methods, We pack these into as few lines as possible,
		 * and hoist consistency checks among them before main loops, to
		 * reduce distraction.
		 */

		/// <summary>
		/// A placeholder task for Sorters, used for the lowest
		/// quartile task, that does not need to maintain array state.
		/// </summary>
		internal sealed class EmptyCompleter : CountedCompleter<Void>
		{
			internal const long SerialVersionUID = 2446542900576103244L;
			internal EmptyCompleter<T1>(CountedCompleter<T1> p) : base(p)
			{
			}
			public void Compute()
			{
			}
		}

		/// <summary>
		/// A trigger for secondary merge of two merges
		/// </summary>
		internal sealed class Relay : CountedCompleter<Void>
		{
			internal const long SerialVersionUID = 2446542900576103244L;
//JAVA TO C# CONVERTER TODO TASK: Java wildcard generics are not converted to .NET:
//ORIGINAL LINE: final java.util.concurrent.CountedCompleter<?> task;
			internal readonly CountedCompleter<?> Task;
			internal Relay<T1>(CountedCompleter<T1> task) : base(null, 1)
			{
				this.Task = task;
			}
			public void Compute()
			{
			}
			public void onCompletion<T1>(CountedCompleter<T1> t)
			{
				Task.Compute();
			}
		}

		/// <summary>
		/// Object + Comparator support class </summary>
		internal sealed class FJObject
		{
			internal sealed class Sorter<T> : CountedCompleter<Void>
			{
				internal const long SerialVersionUID = 2446542900576103244L;
				internal readonly T[] a, w;
				internal readonly int @base, Size, Wbase, Gran;
//JAVA TO C# CONVERTER TODO TASK: There is no .NET equivalent to the Java 'super' constraint:
//ORIGINAL LINE: Comparator<? base T> comparator;
//JAVA TO C# CONVERTER TODO TASK: Java wildcard generics are not converted to .NET:
				internal Comparator<?> Comparator;
//JAVA TO C# CONVERTER TODO TASK: There is no .NET equivalent to the Java 'super' constraint:
//ORIGINAL LINE: Sorter(java.util.concurrent.CountedCompleter<?> par, T[] a, T[] w, int base, int size, int wbase, int gran, Comparator<? base T> comparator)
				internal Sorter<T1, T2>(CountedCompleter<T1> par, T[] a, T[] w, int @base, int size, int wbase, int gran, Comparator<T2> comparator) : base(par)
				{
					this.a = a;
					this.w = w;
					this.@base = @base;
					this.Size = size;
					this.Wbase = wbase;
					this.Gran = gran;
					this.Comparator = comparator;
				}
				public void Compute()
				{
//JAVA TO C# CONVERTER TODO TASK: Java wildcard generics are not converted to .NET:
//ORIGINAL LINE: java.util.concurrent.CountedCompleter<?> s = this;
					CountedCompleter<?> s = this;
//JAVA TO C# CONVERTER TODO TASK: There is no .NET equivalent to the Java 'super' constraint:
//ORIGINAL LINE: Comparator<? base T> c = this.comparator;
//JAVA TO C# CONVERTER TODO TASK: Java wildcard generics are not converted to .NET:
					Comparator<?> c = this.Comparator;
					T[] a = this.a, w = this.w; // localize all params
					int b = this.@base, n = this.Size, wb = this.Wbase, g = this.Gran;
					while (n > g)
					{
						int h = (int)((uint)n >> 1), q = (int)((uint)h >> 1), u = h + q; // quartiles
						Relay fc = new Relay(new Merger<T>(s, w, a, wb, h, wb + h, n - h, b, g, c));
						Relay rc = new Relay(new Merger<T>(fc, a, w, b + h, q, b + u, n - u, wb + h, g, c));
						(new Sorter<T>(rc, a, w, b + u, n - u, wb + u, g, c)).Fork();
						(new Sorter<T>(rc, a, w, b + h, q, wb + h, g, c)).Fork();
						Relay bc = new Relay(new Merger<T>(fc, a, w, b, q, b + q, h - q, wb, g, c));
						(new Sorter<T>(bc, a, w, b + q, h - q, wb + q, g, c)).Fork();
						s = new EmptyCompleter(bc);
						n = q;
					}
					TimSort.Sort(a, b, b + n, c, w, wb, n);
					s.TryComplete();
				}
			}

			internal sealed class Merger<T> : CountedCompleter<Void>
			{
				internal const long SerialVersionUID = 2446542900576103244L;
				internal readonly T[] a, w; // main and workspace arrays
				internal readonly int Lbase, Lsize, Rbase, Rsize, Wbase, Gran;
//JAVA TO C# CONVERTER TODO TASK: There is no .NET equivalent to the Java 'super' constraint:
//ORIGINAL LINE: Comparator<? base T> comparator;
//JAVA TO C# CONVERTER TODO TASK: Java wildcard generics are not converted to .NET:
				internal Comparator<?> Comparator;
//JAVA TO C# CONVERTER TODO TASK: There is no .NET equivalent to the Java 'super' constraint:
//ORIGINAL LINE: Merger(java.util.concurrent.CountedCompleter<?> par, T[] a, T[] w, int lbase, int lsize, int rbase, int rsize, int wbase, int gran, Comparator<? base T> comparator)
				internal Merger<T1, T2>(CountedCompleter<T1> par, T[] a, T[] w, int lbase, int lsize, int rbase, int rsize, int wbase, int gran, Comparator<T2> comparator) : base(par)
				{
					this.a = a;
					this.w = w;
					this.Lbase = lbase;
					this.Lsize = lsize;
					this.Rbase = rbase;
					this.Rsize = rsize;
					this.Wbase = wbase;
					this.Gran = gran;
					this.Comparator = comparator;
				}

				public void Compute()
				{
//JAVA TO C# CONVERTER TODO TASK: There is no .NET equivalent to the Java 'super' constraint:
//ORIGINAL LINE: Comparator<? base T> c = this.comparator;
//JAVA TO C# CONVERTER TODO TASK: Java wildcard generics are not converted to .NET:
					Comparator<?> c = this.Comparator;
					T[] a = this.a, w = this.w; // localize all params
					int lb = this.Lbase, ln = this.Lsize, rb = this.Rbase, rn = this.Rsize, k = this.Wbase, g = this.Gran;
					if (a == null || w == null || lb < 0 || rb < 0 || k < 0 || c == null)
					{
						throw new IllegalStateException(); // hoist checks
					}
					for (int lh, rh;;) // split larger, find point in smaller
					{
						if (ln >= rn)
						{
							if (ln <= g)
							{
								break;
							}
							rh = rn;
							T split = a[(lh = (int)((uint)ln >> 1)) + lb];
							for (int lo = 0; lo < rh;)
							{
								int rm = (int)((uint)(lo + rh) >> 1);
								if (c.Compare(split, a[rm + rb]) <= 0)
								{
									rh = rm;
								}
								else
								{
									lo = rm + 1;
								}
							}
						}
						else
						{
							if (rn <= g)
							{
								break;
							}
							lh = ln;
							T split = a[(rh = (int)((uint)rn >> 1)) + rb];
							for (int lo = 0; lo < lh;)
							{
								int lm = (int)((uint)(lo + lh) >> 1);
								if (c.Compare(split, a[lm + lb]) <= 0)
								{
									lh = lm;
								}
								else
								{
									lo = lm + 1;
								}
							}
						}
						Merger<T> m = new Merger<T>(this, a, w, lb + lh, ln - lh, rb + rh, rn - rh, k + lh + rh, g, c);
						rn = rh;
						ln = lh;
						AddToPendingCount(1);
						m.Fork();
					}

					int lf = lb + ln, rf = rb + rn; // index bounds
					while (lb < lf && rb < rf)
					{
						T t, al, ar;
						if (c.Compare((al = a[lb]), (ar = a[rb])) <= 0)
						{
							lb++;
							t = al;
						}
						else
						{
							rb++;
							t = ar;
						}
						w[k++] = t;
					}
					if (rb < rf)
					{
						System.Array.Copy(a, rb, w, k, rf - rb);
					}
					else if (lb < lf)
					{
						System.Array.Copy(a, lb, w, k, lf - lb);
					}

					TryComplete();
				}

			}
		} // FJObject

		/// <summary>
		/// byte support class </summary>
		internal sealed class FJByte
		{
			internal sealed class Sorter : CountedCompleter<Void>
			{
				internal const long SerialVersionUID = 2446542900576103244L;
				internal readonly sbyte[] a, w;
				internal readonly int @base, Size, Wbase, Gran;
				internal Sorter<T1>(CountedCompleter<T1> par, sbyte[] a, sbyte[] w, int @base, int size, int wbase, int gran) : base(par)
				{
					this.a = a;
					this.w = w;
					this.@base = @base;
					this.Size = size;
					this.Wbase = wbase;
					this.Gran = gran;
				}
				public void Compute()
				{
//JAVA TO C# CONVERTER TODO TASK: Java wildcard generics are not converted to .NET:
//ORIGINAL LINE: java.util.concurrent.CountedCompleter<?> s = this;
					CountedCompleter<?> s = this;
					sbyte[] a = this.a, w = this.w; // localize all params
					int b = this.@base, n = this.Size, wb = this.Wbase, g = this.Gran;
					while (n > g)
					{
						int h = (int)((uint)n >> 1), q = (int)((uint)h >> 1), u = h + q; // quartiles
						Relay fc = new Relay(new Merger(s, w, a, wb, h, wb + h, n - h, b, g));
						Relay rc = new Relay(new Merger(fc, a, w, b + h, q, b + u, n - u, wb + h, g));
						(new Sorter(rc, a, w, b + u, n - u, wb + u, g)).Fork();
						(new Sorter(rc, a, w, b + h, q, wb + h, g)).Fork();
						Relay bc = new Relay(new Merger(fc, a, w, b, q, b + q, h - q, wb, g));
						(new Sorter(bc, a, w, b + q, h - q, wb + q, g)).Fork();
						s = new EmptyCompleter(bc);
						n = q;
					}
					DualPivotQuicksort.Sort(a, b, b + n - 1);
					s.TryComplete();
				}
			}

			internal sealed class Merger : CountedCompleter<Void>
			{
				internal const long SerialVersionUID = 2446542900576103244L;
				internal readonly sbyte[] a, w; // main and workspace arrays
				internal readonly int Lbase, Lsize, Rbase, Rsize, Wbase, Gran;
				internal Merger<T1>(CountedCompleter<T1> par, sbyte[] a, sbyte[] w, int lbase, int lsize, int rbase, int rsize, int wbase, int gran) : base(par)
				{
					this.a = a;
					this.w = w;
					this.Lbase = lbase;
					this.Lsize = lsize;
					this.Rbase = rbase;
					this.Rsize = rsize;
					this.Wbase = wbase;
					this.Gran = gran;
				}

				public void Compute()
				{
					sbyte[] a = this.a, w = this.w; // localize all params
					int lb = this.Lbase, ln = this.Lsize, rb = this.Rbase, rn = this.Rsize, k = this.Wbase, g = this.Gran;
					if (a == null || w == null || lb < 0 || rb < 0 || k < 0)
					{
						throw new IllegalStateException(); // hoist checks
					}
					for (int lh, rh;;) // split larger, find point in smaller
					{
						if (ln >= rn)
						{
							if (ln <= g)
							{
								break;
							}
							rh = rn;
							sbyte split = a[(lh = (int)((uint)ln >> 1)) + lb];
							for (int lo = 0; lo < rh;)
							{
								int rm = (int)((uint)(lo + rh) >> 1);
								if (split <= a[rm + rb])
								{
									rh = rm;
								}
								else
								{
									lo = rm + 1;
								}
							}
						}
						else
						{
							if (rn <= g)
							{
								break;
							}
							lh = ln;
							sbyte split = a[(rh = (int)((uint)rn >> 1)) + rb];
							for (int lo = 0; lo < lh;)
							{
								int lm = (int)((uint)(lo + lh) >> 1);
								if (split <= a[lm + lb])
								{
									lh = lm;
								}
								else
								{
									lo = lm + 1;
								}
							}
						}
						Merger m = new Merger(this, a, w, lb + lh, ln - lh, rb + rh, rn - rh, k + lh + rh, g);
						rn = rh;
						ln = lh;
						AddToPendingCount(1);
						m.Fork();
					}

					int lf = lb + ln, rf = rb + rn; // index bounds
					while (lb < lf && rb < rf)
					{
						sbyte t, al, ar;
						if ((al = a[lb]) <= (ar = a[rb]))
						{
							lb++;
							t = al;
						}
						else
						{
							rb++;
							t = ar;
						}
						w[k++] = t;
					}
					if (rb < rf)
					{
						System.Array.Copy(a, rb, w, k, rf - rb);
					}
					else if (lb < lf)
					{
						System.Array.Copy(a, lb, w, k, lf - lb);
					}
					TryComplete();
				}
			}
		} // FJByte

		/// <summary>
		/// char support class </summary>
		internal sealed class FJChar
		{
			internal sealed class Sorter : CountedCompleter<Void>
			{
				internal const long SerialVersionUID = 2446542900576103244L;
				internal readonly char[] a, w;
				internal readonly int @base, Size, Wbase, Gran;
				internal Sorter<T1>(CountedCompleter<T1> par, char[] a, char[] w, int @base, int size, int wbase, int gran) : base(par)
				{
					this.a = a;
					this.w = w;
					this.@base = @base;
					this.Size = size;
					this.Wbase = wbase;
					this.Gran = gran;
				}
				public void Compute()
				{
//JAVA TO C# CONVERTER TODO TASK: Java wildcard generics are not converted to .NET:
//ORIGINAL LINE: java.util.concurrent.CountedCompleter<?> s = this;
					CountedCompleter<?> s = this;
					char[] a = this.a, w = this.w; // localize all params
					int b = this.@base, n = this.Size, wb = this.Wbase, g = this.Gran;
					while (n > g)
					{
						int h = (int)((uint)n >> 1), q = (int)((uint)h >> 1), u = h + q; // quartiles
						Relay fc = new Relay(new Merger(s, w, a, wb, h, wb + h, n - h, b, g));
						Relay rc = new Relay(new Merger(fc, a, w, b + h, q, b + u, n - u, wb + h, g));
						(new Sorter(rc, a, w, b + u, n - u, wb + u, g)).Fork();
						(new Sorter(rc, a, w, b + h, q, wb + h, g)).Fork();
						Relay bc = new Relay(new Merger(fc, a, w, b, q, b + q, h - q, wb, g));
						(new Sorter(bc, a, w, b + q, h - q, wb + q, g)).Fork();
						s = new EmptyCompleter(bc);
						n = q;
					}
					DualPivotQuicksort.Sort(a, b, b + n - 1, w, wb, n);
					s.TryComplete();
				}
			}

			internal sealed class Merger : CountedCompleter<Void>
			{
				internal const long SerialVersionUID = 2446542900576103244L;
				internal readonly char[] a, w; // main and workspace arrays
				internal readonly int Lbase, Lsize, Rbase, Rsize, Wbase, Gran;
				internal Merger<T1>(CountedCompleter<T1> par, char[] a, char[] w, int lbase, int lsize, int rbase, int rsize, int wbase, int gran) : base(par)
				{
					this.a = a;
					this.w = w;
					this.Lbase = lbase;
					this.Lsize = lsize;
					this.Rbase = rbase;
					this.Rsize = rsize;
					this.Wbase = wbase;
					this.Gran = gran;
				}

				public void Compute()
				{
					char[] a = this.a, w = this.w; // localize all params
					int lb = this.Lbase, ln = this.Lsize, rb = this.Rbase, rn = this.Rsize, k = this.Wbase, g = this.Gran;
					if (a == null || w == null || lb < 0 || rb < 0 || k < 0)
					{
						throw new IllegalStateException(); // hoist checks
					}
					for (int lh, rh;;) // split larger, find point in smaller
					{
						if (ln >= rn)
						{
							if (ln <= g)
							{
								break;
							}
							rh = rn;
							char split = a[(lh = (int)((uint)ln >> 1)) + lb];
							for (int lo = 0; lo < rh;)
							{
								int rm = (int)((uint)(lo + rh) >> 1);
								if (split <= a[rm + rb])
								{
									rh = rm;
								}
								else
								{
									lo = rm + 1;
								}
							}
						}
						else
						{
							if (rn <= g)
							{
								break;
							}
							lh = ln;
							char split = a[(rh = (int)((uint)rn >> 1)) + rb];
							for (int lo = 0; lo < lh;)
							{
								int lm = (int)((uint)(lo + lh) >> 1);
								if (split <= a[lm + lb])
								{
									lh = lm;
								}
								else
								{
									lo = lm + 1;
								}
							}
						}
						Merger m = new Merger(this, a, w, lb + lh, ln - lh, rb + rh, rn - rh, k + lh + rh, g);
						rn = rh;
						ln = lh;
						AddToPendingCount(1);
						m.Fork();
					}

					int lf = lb + ln, rf = rb + rn; // index bounds
					while (lb < lf && rb < rf)
					{
						char t, al, ar;
						if ((al = a[lb]) <= (ar = a[rb]))
						{
							lb++;
							t = al;
						}
						else
						{
							rb++;
							t = ar;
						}
						w[k++] = t;
					}
					if (rb < rf)
					{
						System.Array.Copy(a, rb, w, k, rf - rb);
					}
					else if (lb < lf)
					{
						System.Array.Copy(a, lb, w, k, lf - lb);
					}
					TryComplete();
				}
			}
		} // FJChar

		/// <summary>
		/// short support class </summary>
		internal sealed class FJShort
		{
			internal sealed class Sorter : CountedCompleter<Void>
			{
				internal const long SerialVersionUID = 2446542900576103244L;
				internal readonly short[] a, w;
				internal readonly int @base, Size, Wbase, Gran;
				internal Sorter<T1>(CountedCompleter<T1> par, short[] a, short[] w, int @base, int size, int wbase, int gran) : base(par)
				{
					this.a = a;
					this.w = w;
					this.@base = @base;
					this.Size = size;
					this.Wbase = wbase;
					this.Gran = gran;
				}
				public void Compute()
				{
//JAVA TO C# CONVERTER TODO TASK: Java wildcard generics are not converted to .NET:
//ORIGINAL LINE: java.util.concurrent.CountedCompleter<?> s = this;
					CountedCompleter<?> s = this;
					short[] a = this.a, w = this.w; // localize all params
					int b = this.@base, n = this.Size, wb = this.Wbase, g = this.Gran;
					while (n > g)
					{
						int h = (int)((uint)n >> 1), q = (int)((uint)h >> 1), u = h + q; // quartiles
						Relay fc = new Relay(new Merger(s, w, a, wb, h, wb + h, n - h, b, g));
						Relay rc = new Relay(new Merger(fc, a, w, b + h, q, b + u, n - u, wb + h, g));
						(new Sorter(rc, a, w, b + u, n - u, wb + u, g)).Fork();
						(new Sorter(rc, a, w, b + h, q, wb + h, g)).Fork();
						Relay bc = new Relay(new Merger(fc, a, w, b, q, b + q, h - q, wb, g));
						(new Sorter(bc, a, w, b + q, h - q, wb + q, g)).Fork();
						s = new EmptyCompleter(bc);
						n = q;
					}
					DualPivotQuicksort.Sort(a, b, b + n - 1, w, wb, n);
					s.TryComplete();
				}
			}

			internal sealed class Merger : CountedCompleter<Void>
			{
				internal const long SerialVersionUID = 2446542900576103244L;
				internal readonly short[] a, w; // main and workspace arrays
				internal readonly int Lbase, Lsize, Rbase, Rsize, Wbase, Gran;
				internal Merger<T1>(CountedCompleter<T1> par, short[] a, short[] w, int lbase, int lsize, int rbase, int rsize, int wbase, int gran) : base(par)
				{
					this.a = a;
					this.w = w;
					this.Lbase = lbase;
					this.Lsize = lsize;
					this.Rbase = rbase;
					this.Rsize = rsize;
					this.Wbase = wbase;
					this.Gran = gran;
				}

				public void Compute()
				{
					short[] a = this.a, w = this.w; // localize all params
					int lb = this.Lbase, ln = this.Lsize, rb = this.Rbase, rn = this.Rsize, k = this.Wbase, g = this.Gran;
					if (a == null || w == null || lb < 0 || rb < 0 || k < 0)
					{
						throw new IllegalStateException(); // hoist checks
					}
					for (int lh, rh;;) // split larger, find point in smaller
					{
						if (ln >= rn)
						{
							if (ln <= g)
							{
								break;
							}
							rh = rn;
							short split = a[(lh = (int)((uint)ln >> 1)) + lb];
							for (int lo = 0; lo < rh;)
							{
								int rm = (int)((uint)(lo + rh) >> 1);
								if (split <= a[rm + rb])
								{
									rh = rm;
								}
								else
								{
									lo = rm + 1;
								}
							}
						}
						else
						{
							if (rn <= g)
							{
								break;
							}
							lh = ln;
							short split = a[(rh = (int)((uint)rn >> 1)) + rb];
							for (int lo = 0; lo < lh;)
							{
								int lm = (int)((uint)(lo + lh) >> 1);
								if (split <= a[lm + lb])
								{
									lh = lm;
								}
								else
								{
									lo = lm + 1;
								}
							}
						}
						Merger m = new Merger(this, a, w, lb + lh, ln - lh, rb + rh, rn - rh, k + lh + rh, g);
						rn = rh;
						ln = lh;
						AddToPendingCount(1);
						m.Fork();
					}

					int lf = lb + ln, rf = rb + rn; // index bounds
					while (lb < lf && rb < rf)
					{
						short t, al, ar;
						if ((al = a[lb]) <= (ar = a[rb]))
						{
							lb++;
							t = al;
						}
						else
						{
							rb++;
							t = ar;
						}
						w[k++] = t;
					}
					if (rb < rf)
					{
						System.Array.Copy(a, rb, w, k, rf - rb);
					}
					else if (lb < lf)
					{
						System.Array.Copy(a, lb, w, k, lf - lb);
					}
					TryComplete();
				}
			}
		} // FJShort

		/// <summary>
		/// int support class </summary>
		internal sealed class FJInt
		{
			internal sealed class Sorter : CountedCompleter<Void>
			{
				internal const long SerialVersionUID = 2446542900576103244L;
				internal readonly int[] a, w;
				internal readonly int @base, Size, Wbase, Gran;
				internal Sorter<T1>(CountedCompleter<T1> par, int[] a, int[] w, int @base, int size, int wbase, int gran) : base(par)
				{
					this.a = a;
					this.w = w;
					this.@base = @base;
					this.Size = size;
					this.Wbase = wbase;
					this.Gran = gran;
				}
				public void Compute()
				{
//JAVA TO C# CONVERTER TODO TASK: Java wildcard generics are not converted to .NET:
//ORIGINAL LINE: java.util.concurrent.CountedCompleter<?> s = this;
					CountedCompleter<?> s = this;
					int[] a = this.a, w = this.w; // localize all params
					int b = this.@base, n = this.Size, wb = this.Wbase, g = this.Gran;
					while (n > g)
					{
						int h = (int)((uint)n >> 1), q = (int)((uint)h >> 1), u = h + q; // quartiles
						Relay fc = new Relay(new Merger(s, w, a, wb, h, wb + h, n - h, b, g));
						Relay rc = new Relay(new Merger(fc, a, w, b + h, q, b + u, n - u, wb + h, g));
						(new Sorter(rc, a, w, b + u, n - u, wb + u, g)).Fork();
						(new Sorter(rc, a, w, b + h, q, wb + h, g)).Fork();
						Relay bc = new Relay(new Merger(fc, a, w, b, q, b + q, h - q, wb, g));
						(new Sorter(bc, a, w, b + q, h - q, wb + q, g)).Fork();
						s = new EmptyCompleter(bc);
						n = q;
					}
					DualPivotQuicksort.Sort(a, b, b + n - 1, w, wb, n);
					s.TryComplete();
				}
			}

			internal sealed class Merger : CountedCompleter<Void>
			{
				internal const long SerialVersionUID = 2446542900576103244L;
				internal readonly int[] a, w; // main and workspace arrays
				internal readonly int Lbase, Lsize, Rbase, Rsize, Wbase, Gran;
				internal Merger<T1>(CountedCompleter<T1> par, int[] a, int[] w, int lbase, int lsize, int rbase, int rsize, int wbase, int gran) : base(par)
				{
					this.a = a;
					this.w = w;
					this.Lbase = lbase;
					this.Lsize = lsize;
					this.Rbase = rbase;
					this.Rsize = rsize;
					this.Wbase = wbase;
					this.Gran = gran;
				}

				public void Compute()
				{
					int[] a = this.a, w = this.w; // localize all params
					int lb = this.Lbase, ln = this.Lsize, rb = this.Rbase, rn = this.Rsize, k = this.Wbase, g = this.Gran;
					if (a == null || w == null || lb < 0 || rb < 0 || k < 0)
					{
						throw new IllegalStateException(); // hoist checks
					}
					for (int lh, rh;;) // split larger, find point in smaller
					{
						if (ln >= rn)
						{
							if (ln <= g)
							{
								break;
							}
							rh = rn;
							int split = a[(lh = (int)((uint)ln >> 1)) + lb];
							for (int lo = 0; lo < rh;)
							{
								int rm = (int)((uint)(lo + rh) >> 1);
								if (split <= a[rm + rb])
								{
									rh = rm;
								}
								else
								{
									lo = rm + 1;
								}
							}
						}
						else
						{
							if (rn <= g)
							{
								break;
							}
							lh = ln;
							int split = a[(rh = (int)((uint)rn >> 1)) + rb];
							for (int lo = 0; lo < lh;)
							{
								int lm = (int)((uint)(lo + lh) >> 1);
								if (split <= a[lm + lb])
								{
									lh = lm;
								}
								else
								{
									lo = lm + 1;
								}
							}
						}
						Merger m = new Merger(this, a, w, lb + lh, ln - lh, rb + rh, rn - rh, k + lh + rh, g);
						rn = rh;
						ln = lh;
						AddToPendingCount(1);
						m.Fork();
					}

					int lf = lb + ln, rf = rb + rn; // index bounds
					while (lb < lf && rb < rf)
					{
						int t, al, ar;
						if ((al = a[lb]) <= (ar = a[rb]))
						{
							lb++;
							t = al;
						}
						else
						{
							rb++;
							t = ar;
						}
						w[k++] = t;
					}
					if (rb < rf)
					{
						System.Array.Copy(a, rb, w, k, rf - rb);
					}
					else if (lb < lf)
					{
						System.Array.Copy(a, lb, w, k, lf - lb);
					}
					TryComplete();
				}
			}
		} // FJInt

		/// <summary>
		/// long support class </summary>
		internal sealed class FJLong
		{
			internal sealed class Sorter : CountedCompleter<Void>
			{
				internal const long SerialVersionUID = 2446542900576103244L;
				internal readonly long[] a, w;
				internal readonly int @base, Size, Wbase, Gran;
				internal Sorter<T1>(CountedCompleter<T1> par, long[] a, long[] w, int @base, int size, int wbase, int gran) : base(par)
				{
					this.a = a;
					this.w = w;
					this.@base = @base;
					this.Size = size;
					this.Wbase = wbase;
					this.Gran = gran;
				}
				public void Compute()
				{
//JAVA TO C# CONVERTER TODO TASK: Java wildcard generics are not converted to .NET:
//ORIGINAL LINE: java.util.concurrent.CountedCompleter<?> s = this;
					CountedCompleter<?> s = this;
					long[] a = this.a, w = this.w; // localize all params
					int b = this.@base, n = this.Size, wb = this.Wbase, g = this.Gran;
					while (n > g)
					{
						int h = (int)((uint)n >> 1), q = (int)((uint)h >> 1), u = h + q; // quartiles
						Relay fc = new Relay(new Merger(s, w, a, wb, h, wb + h, n - h, b, g));
						Relay rc = new Relay(new Merger(fc, a, w, b + h, q, b + u, n - u, wb + h, g));
						(new Sorter(rc, a, w, b + u, n - u, wb + u, g)).Fork();
						(new Sorter(rc, a, w, b + h, q, wb + h, g)).Fork();
						Relay bc = new Relay(new Merger(fc, a, w, b, q, b + q, h - q, wb, g));
						(new Sorter(bc, a, w, b + q, h - q, wb + q, g)).Fork();
						s = new EmptyCompleter(bc);
						n = q;
					}
					DualPivotQuicksort.Sort(a, b, b + n - 1, w, wb, n);
					s.TryComplete();
				}
			}

			internal sealed class Merger : CountedCompleter<Void>
			{
				internal const long SerialVersionUID = 2446542900576103244L;
				internal readonly long[] a, w; // main and workspace arrays
				internal readonly int Lbase, Lsize, Rbase, Rsize, Wbase, Gran;
				internal Merger<T1>(CountedCompleter<T1> par, long[] a, long[] w, int lbase, int lsize, int rbase, int rsize, int wbase, int gran) : base(par)
				{
					this.a = a;
					this.w = w;
					this.Lbase = lbase;
					this.Lsize = lsize;
					this.Rbase = rbase;
					this.Rsize = rsize;
					this.Wbase = wbase;
					this.Gran = gran;
				}

				public void Compute()
				{
					long[] a = this.a, w = this.w; // localize all params
					int lb = this.Lbase, ln = this.Lsize, rb = this.Rbase, rn = this.Rsize, k = this.Wbase, g = this.Gran;
					if (a == null || w == null || lb < 0 || rb < 0 || k < 0)
					{
						throw new IllegalStateException(); // hoist checks
					}
					for (int lh, rh;;) // split larger, find point in smaller
					{
						if (ln >= rn)
						{
							if (ln <= g)
							{
								break;
							}
							rh = rn;
							long split = a[(lh = (int)((uint)ln >> 1)) + lb];
							for (int lo = 0; lo < rh;)
							{
								int rm = (int)((uint)(lo + rh) >> 1);
								if (split <= a[rm + rb])
								{
									rh = rm;
								}
								else
								{
									lo = rm + 1;
								}
							}
						}
						else
						{
							if (rn <= g)
							{
								break;
							}
							lh = ln;
							long split = a[(rh = (int)((uint)rn >> 1)) + rb];
							for (int lo = 0; lo < lh;)
							{
								int lm = (int)((uint)(lo + lh) >> 1);
								if (split <= a[lm + lb])
								{
									lh = lm;
								}
								else
								{
									lo = lm + 1;
								}
							}
						}
						Merger m = new Merger(this, a, w, lb + lh, ln - lh, rb + rh, rn - rh, k + lh + rh, g);
						rn = rh;
						ln = lh;
						AddToPendingCount(1);
						m.Fork();
					}

					int lf = lb + ln, rf = rb + rn; // index bounds
					while (lb < lf && rb < rf)
					{
						long t, al, ar;
						if ((al = a[lb]) <= (ar = a[rb]))
						{
							lb++;
							t = al;
						}
						else
						{
							rb++;
							t = ar;
						}
						w[k++] = t;
					}
					if (rb < rf)
					{
						System.Array.Copy(a, rb, w, k, rf - rb);
					}
					else if (lb < lf)
					{
						System.Array.Copy(a, lb, w, k, lf - lb);
					}
					TryComplete();
				}
			}
		} // FJLong

		/// <summary>
		/// float support class </summary>
		internal sealed class FJFloat
		{
			internal sealed class Sorter : CountedCompleter<Void>
			{
				internal const long SerialVersionUID = 2446542900576103244L;
				internal readonly float[] a, w;
				internal readonly int @base, Size, Wbase, Gran;
				internal Sorter<T1>(CountedCompleter<T1> par, float[] a, float[] w, int @base, int size, int wbase, int gran) : base(par)
				{
					this.a = a;
					this.w = w;
					this.@base = @base;
					this.Size = size;
					this.Wbase = wbase;
					this.Gran = gran;
				}
				public void Compute()
				{
//JAVA TO C# CONVERTER TODO TASK: Java wildcard generics are not converted to .NET:
//ORIGINAL LINE: java.util.concurrent.CountedCompleter<?> s = this;
					CountedCompleter<?> s = this;
					float[] a = this.a, w = this.w; // localize all params
					int b = this.@base, n = this.Size, wb = this.Wbase, g = this.Gran;
					while (n > g)
					{
						int h = (int)((uint)n >> 1), q = (int)((uint)h >> 1), u = h + q; // quartiles
						Relay fc = new Relay(new Merger(s, w, a, wb, h, wb + h, n - h, b, g));
						Relay rc = new Relay(new Merger(fc, a, w, b + h, q, b + u, n - u, wb + h, g));
						(new Sorter(rc, a, w, b + u, n - u, wb + u, g)).Fork();
						(new Sorter(rc, a, w, b + h, q, wb + h, g)).Fork();
						Relay bc = new Relay(new Merger(fc, a, w, b, q, b + q, h - q, wb, g));
						(new Sorter(bc, a, w, b + q, h - q, wb + q, g)).Fork();
						s = new EmptyCompleter(bc);
						n = q;
					}
					DualPivotQuicksort.Sort(a, b, b + n - 1, w, wb, n);
					s.TryComplete();
				}
			}

			internal sealed class Merger : CountedCompleter<Void>
			{
				internal const long SerialVersionUID = 2446542900576103244L;
				internal readonly float[] a, w; // main and workspace arrays
				internal readonly int Lbase, Lsize, Rbase, Rsize, Wbase, Gran;
				internal Merger<T1>(CountedCompleter<T1> par, float[] a, float[] w, int lbase, int lsize, int rbase, int rsize, int wbase, int gran) : base(par)
				{
					this.a = a;
					this.w = w;
					this.Lbase = lbase;
					this.Lsize = lsize;
					this.Rbase = rbase;
					this.Rsize = rsize;
					this.Wbase = wbase;
					this.Gran = gran;
				}

				public void Compute()
				{
					float[] a = this.a, w = this.w; // localize all params
					int lb = this.Lbase, ln = this.Lsize, rb = this.Rbase, rn = this.Rsize, k = this.Wbase, g = this.Gran;
					if (a == null || w == null || lb < 0 || rb < 0 || k < 0)
					{
						throw new IllegalStateException(); // hoist checks
					}
					for (int lh, rh;;) // split larger, find point in smaller
					{
						if (ln >= rn)
						{
							if (ln <= g)
							{
								break;
							}
							rh = rn;
							float split = a[(lh = (int)((uint)ln >> 1)) + lb];
							for (int lo = 0; lo < rh;)
							{
								int rm = (int)((uint)(lo + rh) >> 1);
								if (split <= a[rm + rb])
								{
									rh = rm;
								}
								else
								{
									lo = rm + 1;
								}
							}
						}
						else
						{
							if (rn <= g)
							{
								break;
							}
							lh = ln;
							float split = a[(rh = (int)((uint)rn >> 1)) + rb];
							for (int lo = 0; lo < lh;)
							{
								int lm = (int)((uint)(lo + lh) >> 1);
								if (split <= a[lm + lb])
								{
									lh = lm;
								}
								else
								{
									lo = lm + 1;
								}
							}
						}
						Merger m = new Merger(this, a, w, lb + lh, ln - lh, rb + rh, rn - rh, k + lh + rh, g);
						rn = rh;
						ln = lh;
						AddToPendingCount(1);
						m.Fork();
					}

					int lf = lb + ln, rf = rb + rn; // index bounds
					while (lb < lf && rb < rf)
					{
						float t, al, ar;
						if ((al = a[lb]) <= (ar = a[rb]))
						{
							lb++;
							t = al;
						}
						else
						{
							rb++;
							t = ar;
						}
						w[k++] = t;
					}
					if (rb < rf)
					{
						System.Array.Copy(a, rb, w, k, rf - rb);
					}
					else if (lb < lf)
					{
						System.Array.Copy(a, lb, w, k, lf - lb);
					}
					TryComplete();
				}
			}
		} // FJFloat

		/// <summary>
		/// double support class </summary>
		internal sealed class FJDouble
		{
			internal sealed class Sorter : CountedCompleter<Void>
			{
				internal const long SerialVersionUID = 2446542900576103244L;
				internal readonly double[] a, w;
				internal readonly int @base, Size, Wbase, Gran;
				internal Sorter<T1>(CountedCompleter<T1> par, double[] a, double[] w, int @base, int size, int wbase, int gran) : base(par)
				{
					this.a = a;
					this.w = w;
					this.@base = @base;
					this.Size = size;
					this.Wbase = wbase;
					this.Gran = gran;
				}
				public void Compute()
				{
//JAVA TO C# CONVERTER TODO TASK: Java wildcard generics are not converted to .NET:
//ORIGINAL LINE: java.util.concurrent.CountedCompleter<?> s = this;
					CountedCompleter<?> s = this;
					double[] a = this.a, w = this.w; // localize all params
					int b = this.@base, n = this.Size, wb = this.Wbase, g = this.Gran;
					while (n > g)
					{
						int h = (int)((uint)n >> 1), q = (int)((uint)h >> 1), u = h + q; // quartiles
						Relay fc = new Relay(new Merger(s, w, a, wb, h, wb + h, n - h, b, g));
						Relay rc = new Relay(new Merger(fc, a, w, b + h, q, b + u, n - u, wb + h, g));
						(new Sorter(rc, a, w, b + u, n - u, wb + u, g)).Fork();
						(new Sorter(rc, a, w, b + h, q, wb + h, g)).Fork();
						Relay bc = new Relay(new Merger(fc, a, w, b, q, b + q, h - q, wb, g));
						(new Sorter(bc, a, w, b + q, h - q, wb + q, g)).Fork();
						s = new EmptyCompleter(bc);
						n = q;
					}
					DualPivotQuicksort.Sort(a, b, b + n - 1, w, wb, n);
					s.TryComplete();
				}
			}

			internal sealed class Merger : CountedCompleter<Void>
			{
				internal const long SerialVersionUID = 2446542900576103244L;
				internal readonly double[] a, w; // main and workspace arrays
				internal readonly int Lbase, Lsize, Rbase, Rsize, Wbase, Gran;
				internal Merger<T1>(CountedCompleter<T1> par, double[] a, double[] w, int lbase, int lsize, int rbase, int rsize, int wbase, int gran) : base(par)
				{
					this.a = a;
					this.w = w;
					this.Lbase = lbase;
					this.Lsize = lsize;
					this.Rbase = rbase;
					this.Rsize = rsize;
					this.Wbase = wbase;
					this.Gran = gran;
				}

				public void Compute()
				{
					double[] a = this.a, w = this.w; // localize all params
					int lb = this.Lbase, ln = this.Lsize, rb = this.Rbase, rn = this.Rsize, k = this.Wbase, g = this.Gran;
					if (a == null || w == null || lb < 0 || rb < 0 || k < 0)
					{
						throw new IllegalStateException(); // hoist checks
					}
					for (int lh, rh;;) // split larger, find point in smaller
					{
						if (ln >= rn)
						{
							if (ln <= g)
							{
								break;
							}
							rh = rn;
							double split = a[(lh = (int)((uint)ln >> 1)) + lb];
							for (int lo = 0; lo < rh;)
							{
								int rm = (int)((uint)(lo + rh) >> 1);
								if (split <= a[rm + rb])
								{
									rh = rm;
								}
								else
								{
									lo = rm + 1;
								}
							}
						}
						else
						{
							if (rn <= g)
							{
								break;
							}
							lh = ln;
							double split = a[(rh = (int)((uint)rn >> 1)) + rb];
							for (int lo = 0; lo < lh;)
							{
								int lm = (int)((uint)(lo + lh) >> 1);
								if (split <= a[lm + lb])
								{
									lh = lm;
								}
								else
								{
									lo = lm + 1;
								}
							}
						}
						Merger m = new Merger(this, a, w, lb + lh, ln - lh, rb + rh, rn - rh, k + lh + rh, g);
						rn = rh;
						ln = lh;
						AddToPendingCount(1);
						m.Fork();
					}

					int lf = lb + ln, rf = rb + rn; // index bounds
					while (lb < lf && rb < rf)
					{
						double t, al, ar;
						if ((al = a[lb]) <= (ar = a[rb]))
						{
							lb++;
							t = al;
						}
						else
						{
							rb++;
							t = ar;
						}
						w[k++] = t;
					}
					if (rb < rf)
					{
						System.Array.Copy(a, rb, w, k, rf - rb);
					}
					else if (lb < lf)
					{
						System.Array.Copy(a, lb, w, k, lf - lb);
					}
					TryComplete();
				}
			}
		} // FJDouble

	 }

}
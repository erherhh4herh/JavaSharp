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

	/*
	 * Written by Doug Lea with assistance from members of JCP JSR-166
	 * Expert Group and released to the public domain, as explained at
	 * http://creativecommons.org/publicdomain/zero/1.0/
	 */


	/// <summary>
	/// ForkJoin tasks to perform Arrays.parallelPrefix operations.
	/// 
	/// @author Doug Lea
	/// @since 1.8
	/// </summary>
	internal class ArrayPrefixHelpers
	{
		private ArrayPrefixHelpers() // non-instantiable
		{
		};

		/*
		 * Parallel prefix (aka cumulate, scan) task classes
		 * are based loosely on Guy Blelloch's original
		 * algorithm (http://www.cs.cmu.edu/~scandal/alg/scan.html):
		 *  Keep dividing by two to threshold segment size, and then:
		 *   Pass 1: Create tree of partial sums for each segment
		 *   Pass 2: For each segment, cumulate with offset of left sibling
		 *
		 * This version improves performance within FJ framework mainly by
		 * allowing the second pass of ready left-hand sides to proceed
		 * even if some right-hand side first passes are still executing.
		 * It also combines first and second pass for leftmost segment,
		 * and skips the first pass for rightmost segment (whose result is
		 * not needed for second pass).  It similarly manages to avoid
		 * requiring that users supply an identity basis for accumulations
		 * by tracking those segments/subtasks for which the first
		 * existing element is used as base.
		 *
		 * Managing this relies on ORing some bits in the pendingCount for
		 * phases/states: CUMULATE, SUMMED, and FINISHED. CUMULATE is the
		 * main phase bit. When false, segments compute only their sum.
		 * When true, they cumulate array elements. CUMULATE is set at
		 * root at beginning of second pass and then propagated down. But
		 * it may also be set earlier for subtrees with lo==0 (the left
		 * spine of tree). SUMMED is a one bit join count. For leafs, it
		 * is set when summed. For internal nodes, it becomes true when
		 * one child is summed.  When the second child finishes summing,
		 * we then moves up tree to trigger the cumulate phase. FINISHED
		 * is also a one bit join count. For leafs, it is set when
		 * cumulated. For internal nodes, it becomes true when one child
		 * is cumulated.  When the second child finishes cumulating, it
		 * then moves up tree, completing at the root.
		 *
		 * To better exploit locality and reduce overhead, the compute
		 * method loops starting with the current task, moving if possible
		 * to one of its subtasks rather than forking.
		 *
		 * As usual for this sort of utility, there are 4 versions, that
		 * are simple copy/paste/adapt variants of each other.  (The
		 * double and int versions differ from long version soley by
		 * replacing "long" (with case-matching)).
		 */

		// see above
		internal const int CUMULATE = 1;
		internal const int SUMMED = 2;
		internal const int FINISHED = 4;

		/// <summary>
		/// The smallest subtask array partition size to use as threshold </summary>
		internal const int MIN_PARTITION = 16;

		internal sealed class CumulateTask<T> : CountedCompleter<Void>
		{
			internal readonly T[] Array;
			internal readonly BinaryOperator<T> Function;
			internal CumulateTask<T> Left, Right;
			internal T @in, @out;
			internal readonly int Lo, Hi, Origin, Fence, Threshold;

			/// <summary>
			/// Root task constructor </summary>
			public CumulateTask(CumulateTask<T> parent, BinaryOperator<T> function, T[] array, int lo, int hi) : base(parent)
			{
				this.Function = function;
				this.Array = array;
				this.Lo = this.Origin = lo;
				this.Hi = this.Fence = hi;
				int p;
				this.Threshold = (p = (hi - lo) / (ForkJoinPool.CommonPoolParallelism << 3)) <= MIN_PARTITION ? MIN_PARTITION : p;
			}

			/// <summary>
			/// Subtask constructor </summary>
			internal CumulateTask(CumulateTask<T> parent, BinaryOperator<T> function, T[] array, int origin, int fence, int threshold, int lo, int hi) : base(parent)
			{
				this.Function = function;
				this.Array = array;
				this.Origin = origin;
				this.Fence = fence;
				this.Threshold = threshold;
				this.Lo = lo;
				this.Hi = hi;
			}

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @SuppressWarnings("unchecked") public final void compute()
			public void Compute()
			{
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final java.util.function.BinaryOperator<T> fn;
				BinaryOperator<T> fn;
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final T[] a;
				T[] a;
				if ((fn = this.Function) == null || (a = this.Array) == null)
				{
					throw new NullPointerException(); // hoist checks
				}
				int th = Threshold, org = Origin, fnc = Fence, l , h ;
				CumulateTask<T> t = this;
				while ((l = t.Lo) >= 0 && (h = t.Hi) <= a.Length)
				{
					if (h - l > th)
					{
						CumulateTask<T> lt = t.Left, rt = t.Right, f ;
						if (lt == null) // first pass
						{
							int mid = (int)((uint)(l + h) >> 1);
							f = rt = t.Right = new CumulateTask<T>(t, fn, a, org, fnc, th, mid, h);
							t = lt = t.Left = new CumulateTask<T>(t, fn, a, org, fnc, th, l, mid);
						}
						else // possibly refork
						{
							T pin = t.@in;
							lt.@in = pin;
							f = t = null;
							if (rt != null)
							{
								T lout = lt.@out;
								rt.@in = (l == org ? lout : fn.Apply(pin, lout));
								for (int c;;)
								{
									if (((c = rt.PendingCount) & CUMULATE) != 0)
									{
										break;
									}
									if (rt.CompareAndSetPendingCount(c, c | CUMULATE))
									{
										t = rt;
										break;
									}
								}
							}
							for (int c;;)
							{
								if (((c = lt.PendingCount) & CUMULATE) != 0)
								{
									break;
								}
								if (lt.CompareAndSetPendingCount(c, c | CUMULATE))
								{
									if (t != null)
									{
										f = t;
									}
									t = lt;
									break;
								}
							}
							if (t == null)
							{
								break;
							}
						}
						if (f != null)
						{
							f.Fork();
						}
					}
					else
					{
						int state; // Transition to sum, cumulate, or both
						for (int b;;)
						{
							if (((b = t.PendingCount) & FINISHED) != 0)
							{
								goto outerBreak; // already done
							}
							state = ((b & CUMULATE) != 0? FINISHED : (l > org) ? SUMMED : (SUMMED | FINISHED));
							if (t.CompareAndSetPendingCount(b, b | state))
							{
								break;
							}
						}

						T sum;
						if (state != SUMMED)
						{
							int first;
							if (l == org) // leftmost; no in
							{
								sum = a[org];
								first = org + 1;
							}
							else
							{
								sum = t.@in;
								first = l;
							}
							for (int i = first; i < h; ++i) // cumulate
							{
								a[i] = sum = fn.Apply(sum, a[i]);
							}
						}
						else if (h < fnc) // skip rightmost
						{
							sum = a[l];
							for (int i = l + 1; i < h; ++i) // sum only
							{
								sum = fn.Apply(sum, a[i]);
							}
						}
						else
						{
							sum = t.@in;
						}
						t.@out = sum;
						for (CumulateTask<T> par;;) // propagate
						{
							if ((par = (CumulateTask<T>)t.Completer) == null)
							{
								if ((state & FINISHED) != 0) // enable join
								{
									t.QuietlyComplete();
								}
								goto outerBreak;
							}
							int b = par.PendingCount;
							if ((b & state & FINISHED) != 0)
							{
								t = par; // both done
							}
							else if ((b & state & SUMMED) != 0) // both summed
							{
								int nextState;
								CumulateTask<T> lt, rt;
								if ((lt = par.left) != null && (rt = par.right) != null)
								{
									T lout = lt.@out;
									par.@out = (rt.Hi == fnc ? lout : fn.Apply(lout, rt.@out));
								}
								int refork = (((b & CUMULATE) == 0 && par.lo == org) ? CUMULATE : 0);
								if ((nextState = b | state | refork) == b || par.compareAndSetPendingCount(b, nextState))
								{
									state = SUMMED; // drop finished
									t = par;
									if (refork != 0)
									{
										par.fork();
									}
								}
							}
							else if (par.compareAndSetPendingCount(b, b | state))
							{
								goto outerBreak; // sib not ready
							}
						}
					}
					outerContinue:;
				}
				outerBreak:;
			}
		}

		internal sealed class LongCumulateTask : CountedCompleter<Void>
		{
			internal readonly long[] Array;
			internal readonly LongBinaryOperator Function;
			internal LongCumulateTask Left, Right;
			internal long @in, @out;
			internal readonly int Lo, Hi, Origin, Fence, Threshold;

			/// <summary>
			/// Root task constructor </summary>
			public LongCumulateTask(LongCumulateTask parent, LongBinaryOperator function, long[] array, int lo, int hi) : base(parent)
			{
				this.Function = function;
				this.Array = array;
				this.Lo = this.Origin = lo;
				this.Hi = this.Fence = hi;
				int p;
				this.Threshold = (p = (hi - lo) / (ForkJoinPool.CommonPoolParallelism << 3)) <= MIN_PARTITION ? MIN_PARTITION : p;
			}

			/// <summary>
			/// Subtask constructor </summary>
			internal LongCumulateTask(LongCumulateTask parent, LongBinaryOperator function, long[] array, int origin, int fence, int threshold, int lo, int hi) : base(parent)
			{
				this.Function = function;
				this.Array = array;
				this.Origin = origin;
				this.Fence = fence;
				this.Threshold = threshold;
				this.Lo = lo;
				this.Hi = hi;
			}

			public void Compute()
			{
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final java.util.function.LongBinaryOperator fn;
				LongBinaryOperator fn;
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final long[] a;
				long[] a;
				if ((fn = this.Function) == null || (a = this.Array) == null)
				{
					throw new NullPointerException(); // hoist checks
				}
				int th = Threshold, org = Origin, fnc = Fence, l , h ;
				LongCumulateTask t = this;
				while ((l = t.Lo) >= 0 && (h = t.Hi) <= a.Length)
				{
					if (h - l > th)
					{
						LongCumulateTask lt = t.Left, rt = t.Right, f ;
						if (lt == null) // first pass
						{
							int mid = (int)((uint)(l + h) >> 1);
							f = rt = t.Right = new LongCumulateTask(t, fn, a, org, fnc, th, mid, h);
							t = lt = t.Left = new LongCumulateTask(t, fn, a, org, fnc, th, l, mid);
						}
						else // possibly refork
						{
							long pin = t.@in;
							lt.@in = pin;
							f = t = null;
							if (rt != null)
							{
								long lout = lt.@out;
								rt.@in = (l == org ? lout : fn.ApplyAsLong(pin, lout));
								for (int c;;)
								{
									if (((c = rt.PendingCount) & CUMULATE) != 0)
									{
										break;
									}
									if (rt.CompareAndSetPendingCount(c, c | CUMULATE))
									{
										t = rt;
										break;
									}
								}
							}
							for (int c;;)
							{
								if (((c = lt.PendingCount) & CUMULATE) != 0)
								{
									break;
								}
								if (lt.CompareAndSetPendingCount(c, c | CUMULATE))
								{
									if (t != null)
									{
										f = t;
									}
									t = lt;
									break;
								}
							}
							if (t == null)
							{
								break;
							}
						}
						if (f != null)
						{
							f.Fork();
						}
					}
					else
					{
						int state; // Transition to sum, cumulate, or both
						for (int b;;)
						{
							if (((b = t.PendingCount) & FINISHED) != 0)
							{
								goto outerBreak; // already done
							}
							state = ((b & CUMULATE) != 0? FINISHED : (l > org) ? SUMMED : (SUMMED | FINISHED));
							if (t.CompareAndSetPendingCount(b, b | state))
							{
								break;
							}
						}

						long sum;
						if (state != SUMMED)
						{
							int first;
							if (l == org) // leftmost; no in
							{
								sum = a[org];
								first = org + 1;
							}
							else
							{
								sum = t.@in;
								first = l;
							}
							for (int i = first; i < h; ++i) // cumulate
							{
								a[i] = sum = fn.ApplyAsLong(sum, a[i]);
							}
						}
						else if (h < fnc) // skip rightmost
						{
							sum = a[l];
							for (int i = l + 1; i < h; ++i) // sum only
							{
								sum = fn.ApplyAsLong(sum, a[i]);
							}
						}
						else
						{
							sum = t.@in;
						}
						t.@out = sum;
						for (LongCumulateTask par;;) // propagate
						{
							if ((par = (LongCumulateTask)t.Completer) == null)
							{
								if ((state & FINISHED) != 0) // enable join
								{
									t.QuietlyComplete();
								}
								goto outerBreak;
							}
							int b = par.PendingCount;
							if ((b & state & FINISHED) != 0)
							{
								t = par; // both done
							}
							else if ((b & state & SUMMED) != 0) // both summed
							{
								int nextState;
								LongCumulateTask lt, rt;
								if ((lt = par.left) != null && (rt = par.right) != null)
								{
									long lout = lt.@out;
									par.@out = (rt.Hi == fnc ? lout : fn.ApplyAsLong(lout, rt.@out));
								}
								int refork = (((b & CUMULATE) == 0 && par.lo == org) ? CUMULATE : 0);
								if ((nextState = b | state | refork) == b || par.compareAndSetPendingCount(b, nextState))
								{
									state = SUMMED; // drop finished
									t = par;
									if (refork != 0)
									{
										par.fork();
									}
								}
							}
							else if (par.compareAndSetPendingCount(b, b | state))
							{
								goto outerBreak; // sib not ready
							}
						}
					}
					outerContinue:;
				}
				outerBreak:;
			}
		}

		internal sealed class DoubleCumulateTask : CountedCompleter<Void>
		{
			internal readonly double[] Array;
			internal readonly DoubleBinaryOperator Function;
			internal DoubleCumulateTask Left, Right;
			internal double @in, @out;
			internal readonly int Lo, Hi, Origin, Fence, Threshold;

			/// <summary>
			/// Root task constructor </summary>
			public DoubleCumulateTask(DoubleCumulateTask parent, DoubleBinaryOperator function, double[] array, int lo, int hi) : base(parent)
			{
				this.Function = function;
				this.Array = array;
				this.Lo = this.Origin = lo;
				this.Hi = this.Fence = hi;
				int p;
				this.Threshold = (p = (hi - lo) / (ForkJoinPool.CommonPoolParallelism << 3)) <= MIN_PARTITION ? MIN_PARTITION : p;
			}

			/// <summary>
			/// Subtask constructor </summary>
			internal DoubleCumulateTask(DoubleCumulateTask parent, DoubleBinaryOperator function, double[] array, int origin, int fence, int threshold, int lo, int hi) : base(parent)
			{
				this.Function = function;
				this.Array = array;
				this.Origin = origin;
				this.Fence = fence;
				this.Threshold = threshold;
				this.Lo = lo;
				this.Hi = hi;
			}

			public void Compute()
			{
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final java.util.function.DoubleBinaryOperator fn;
				DoubleBinaryOperator fn;
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final double[] a;
				double[] a;
				if ((fn = this.Function) == null || (a = this.Array) == null)
				{
					throw new NullPointerException(); // hoist checks
				}
				int th = Threshold, org = Origin, fnc = Fence, l , h ;
				DoubleCumulateTask t = this;
				while ((l = t.Lo) >= 0 && (h = t.Hi) <= a.Length)
				{
					if (h - l > th)
					{
						DoubleCumulateTask lt = t.Left, rt = t.Right, f ;
						if (lt == null) // first pass
						{
							int mid = (int)((uint)(l + h) >> 1);
							f = rt = t.Right = new DoubleCumulateTask(t, fn, a, org, fnc, th, mid, h);
							t = lt = t.Left = new DoubleCumulateTask(t, fn, a, org, fnc, th, l, mid);
						}
						else // possibly refork
						{
							double pin = t.@in;
							lt.@in = pin;
							f = t = null;
							if (rt != null)
							{
								double lout = lt.@out;
								rt.@in = (l == org ? lout : fn.ApplyAsDouble(pin, lout));
								for (int c;;)
								{
									if (((c = rt.PendingCount) & CUMULATE) != 0)
									{
										break;
									}
									if (rt.CompareAndSetPendingCount(c, c | CUMULATE))
									{
										t = rt;
										break;
									}
								}
							}
							for (int c;;)
							{
								if (((c = lt.PendingCount) & CUMULATE) != 0)
								{
									break;
								}
								if (lt.CompareAndSetPendingCount(c, c | CUMULATE))
								{
									if (t != null)
									{
										f = t;
									}
									t = lt;
									break;
								}
							}
							if (t == null)
							{
								break;
							}
						}
						if (f != null)
						{
							f.Fork();
						}
					}
					else
					{
						int state; // Transition to sum, cumulate, or both
						for (int b;;)
						{
							if (((b = t.PendingCount) & FINISHED) != 0)
							{
								goto outerBreak; // already done
							}
							state = ((b & CUMULATE) != 0? FINISHED : (l > org) ? SUMMED : (SUMMED | FINISHED));
							if (t.CompareAndSetPendingCount(b, b | state))
							{
								break;
							}
						}

						double sum;
						if (state != SUMMED)
						{
							int first;
							if (l == org) // leftmost; no in
							{
								sum = a[org];
								first = org + 1;
							}
							else
							{
								sum = t.@in;
								first = l;
							}
							for (int i = first; i < h; ++i) // cumulate
							{
								a[i] = sum = fn.ApplyAsDouble(sum, a[i]);
							}
						}
						else if (h < fnc) // skip rightmost
						{
							sum = a[l];
							for (int i = l + 1; i < h; ++i) // sum only
							{
								sum = fn.ApplyAsDouble(sum, a[i]);
							}
						}
						else
						{
							sum = t.@in;
						}
						t.@out = sum;
						for (DoubleCumulateTask par;;) // propagate
						{
							if ((par = (DoubleCumulateTask)t.Completer) == null)
							{
								if ((state & FINISHED) != 0) // enable join
								{
									t.QuietlyComplete();
								}
								goto outerBreak;
							}
							int b = par.PendingCount;
							if ((b & state & FINISHED) != 0)
							{
								t = par; // both done
							}
							else if ((b & state & SUMMED) != 0) // both summed
							{
								int nextState;
								DoubleCumulateTask lt, rt;
								if ((lt = par.left) != null && (rt = par.right) != null)
								{
									double lout = lt.@out;
									par.@out = (rt.Hi == fnc ? lout : fn.ApplyAsDouble(lout, rt.@out));
								}
								int refork = (((b & CUMULATE) == 0 && par.lo == org) ? CUMULATE : 0);
								if ((nextState = b | state | refork) == b || par.compareAndSetPendingCount(b, nextState))
								{
									state = SUMMED; // drop finished
									t = par;
									if (refork != 0)
									{
										par.fork();
									}
								}
							}
							else if (par.compareAndSetPendingCount(b, b | state))
							{
								goto outerBreak; // sib not ready
							}
						}
					}
					outerContinue:;
				}
				outerBreak:;
			}
		}

		internal sealed class IntCumulateTask : CountedCompleter<Void>
		{
			internal readonly int[] Array;
			internal readonly IntBinaryOperator Function;
			internal IntCumulateTask Left, Right;
			internal int @in, @out;
			internal readonly int Lo, Hi, Origin, Fence, Threshold;

			/// <summary>
			/// Root task constructor </summary>
			public IntCumulateTask(IntCumulateTask parent, IntBinaryOperator function, int[] array, int lo, int hi) : base(parent)
			{
				this.Function = function;
				this.Array = array;
				this.Lo = this.Origin = lo;
				this.Hi = this.Fence = hi;
				int p;
				this.Threshold = (p = (hi - lo) / (ForkJoinPool.CommonPoolParallelism << 3)) <= MIN_PARTITION ? MIN_PARTITION : p;
			}

			/// <summary>
			/// Subtask constructor </summary>
			internal IntCumulateTask(IntCumulateTask parent, IntBinaryOperator function, int[] array, int origin, int fence, int threshold, int lo, int hi) : base(parent)
			{
				this.Function = function;
				this.Array = array;
				this.Origin = origin;
				this.Fence = fence;
				this.Threshold = threshold;
				this.Lo = lo;
				this.Hi = hi;
			}

			public void Compute()
			{
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final java.util.function.IntBinaryOperator fn;
				IntBinaryOperator fn;
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final int[] a;
				int[] a;
				if ((fn = this.Function) == null || (a = this.Array) == null)
				{
					throw new NullPointerException(); // hoist checks
				}
				int th = Threshold, org = Origin, fnc = Fence, l , h ;
				IntCumulateTask t = this;
				while ((l = t.Lo) >= 0 && (h = t.Hi) <= a.Length)
				{
					if (h - l > th)
					{
						IntCumulateTask lt = t.Left, rt = t.Right, f ;
						if (lt == null) // first pass
						{
							int mid = (int)((uint)(l + h) >> 1);
							f = rt = t.Right = new IntCumulateTask(t, fn, a, org, fnc, th, mid, h);
							t = lt = t.Left = new IntCumulateTask(t, fn, a, org, fnc, th, l, mid);
						}
						else // possibly refork
						{
							int pin = t.@in;
							lt.@in = pin;
							f = t = null;
							if (rt != null)
							{
								int lout = lt.@out;
								rt.@in = (l == org ? lout : fn.ApplyAsInt(pin, lout));
								for (int c;;)
								{
									if (((c = rt.PendingCount) & CUMULATE) != 0)
									{
										break;
									}
									if (rt.CompareAndSetPendingCount(c, c | CUMULATE))
									{
										t = rt;
										break;
									}
								}
							}
							for (int c;;)
							{
								if (((c = lt.PendingCount) & CUMULATE) != 0)
								{
									break;
								}
								if (lt.CompareAndSetPendingCount(c, c | CUMULATE))
								{
									if (t != null)
									{
										f = t;
									}
									t = lt;
									break;
								}
							}
							if (t == null)
							{
								break;
							}
						}
						if (f != null)
						{
							f.Fork();
						}
					}
					else
					{
						int state; // Transition to sum, cumulate, or both
						for (int b;;)
						{
							if (((b = t.PendingCount) & FINISHED) != 0)
							{
								goto outerBreak; // already done
							}
							state = ((b & CUMULATE) != 0? FINISHED : (l > org) ? SUMMED : (SUMMED | FINISHED));
							if (t.CompareAndSetPendingCount(b, b | state))
							{
								break;
							}
						}

						int sum;
						if (state != SUMMED)
						{
							int first;
							if (l == org) // leftmost; no in
							{
								sum = a[org];
								first = org + 1;
							}
							else
							{
								sum = t.@in;
								first = l;
							}
							for (int i = first; i < h; ++i) // cumulate
							{
								a[i] = sum = fn.ApplyAsInt(sum, a[i]);
							}
						}
						else if (h < fnc) // skip rightmost
						{
							sum = a[l];
							for (int i = l + 1; i < h; ++i) // sum only
							{
								sum = fn.ApplyAsInt(sum, a[i]);
							}
						}
						else
						{
							sum = t.@in;
						}
						t.@out = sum;
						for (IntCumulateTask par;;) // propagate
						{
							if ((par = (IntCumulateTask)t.Completer) == null)
							{
								if ((state & FINISHED) != 0) // enable join
								{
									t.QuietlyComplete();
								}
								goto outerBreak;
							}
							int b = par.PendingCount;
							if ((b & state & FINISHED) != 0)
							{
								t = par; // both done
							}
							else if ((b & state & SUMMED) != 0) // both summed
							{
								int nextState;
								IntCumulateTask lt, rt;
								if ((lt = par.left) != null && (rt = par.right) != null)
								{
									int lout = lt.@out;
									par.@out = (rt.Hi == fnc ? lout : fn.ApplyAsInt(lout, rt.@out));
								}
								int refork = (((b & CUMULATE) == 0 && par.lo == org) ? CUMULATE : 0);
								if ((nextState = b | state | refork) == b || par.compareAndSetPendingCount(b, nextState))
								{
									state = SUMMED; // drop finished
									t = par;
									if (refork != 0)
									{
										par.fork();
									}
								}
							}
							else if (par.compareAndSetPendingCount(b, b | state))
							{
								goto outerBreak; // sib not ready
							}
						}
					}
					outerContinue:;
				}
				outerBreak:;
			}
		}
	}

}
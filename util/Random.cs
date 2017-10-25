using System;

/*
 * Copyright (c) 1995, 2013, Oracle and/or its affiliates. All rights reserved.
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

	using Unsafe = sun.misc.Unsafe;

	/// <summary>
	/// An instance of this class is used to generate a stream of
	/// pseudorandom numbers. The class uses a 48-bit seed, which is
	/// modified using a linear congruential formula. (See Donald Knuth,
	/// <i>The Art of Computer Programming, Volume 2</i>, Section 3.2.1.)
	/// <para>
	/// If two instances of {@code Random} are created with the same
	/// seed, and the same sequence of method calls is made for each, they
	/// will generate and return identical sequences of numbers. In order to
	/// guarantee this property, particular algorithms are specified for the
	/// class {@code Random}. Java implementations must use all the algorithms
	/// shown here for the class {@code Random}, for the sake of absolute
	/// portability of Java code. However, subclasses of class {@code Random}
	/// are permitted to use other algorithms, so long as they adhere to the
	/// general contracts for all the methods.
	/// </para>
	/// <para>
	/// The algorithms implemented by class {@code Random} use a
	/// {@code protected} utility method that on each invocation can supply
	/// up to 32 pseudorandomly generated bits.
	/// </para>
	/// <para>
	/// Many applications will find the method <seealso cref="Math#random"/> simpler to use.
	/// 
	/// </para>
	/// <para>Instances of {@code java.util.Random} are threadsafe.
	/// However, the concurrent use of the same {@code java.util.Random}
	/// instance across threads may encounter contention and consequent
	/// poor performance. Consider instead using
	/// <seealso cref="java.util.concurrent.ThreadLocalRandom"/> in multithreaded
	/// designs.
	/// 
	/// </para>
	/// <para>Instances of {@code java.util.Random} are not cryptographically
	/// secure.  Consider instead using <seealso cref="java.security.SecureRandom"/> to
	/// get a cryptographically secure pseudo-random number generator for use
	/// by security-sensitive applications.
	/// 
	/// @author  Frank Yellin
	/// @since   1.0
	/// </para>
	/// </summary>
	[Serializable]
	public class Random
	{
		/// <summary>
		/// use serialVersionUID from JDK 1.1 for interoperability </summary>
		internal const long SerialVersionUID = 3905348978240129619L;

		/// <summary>
		/// The internal state associated with this pseudorandom number generator.
		/// (The specs for the methods in this class describe the ongoing
		/// computation of this value.)
		/// </summary>
		private readonly AtomicLong Seed_Renamed;

		private const long Multiplier = 0x5DEECE66DL;
		private const long Addend = 0xBL;
		private static readonly long Mask = (1L << 48) - 1;

		private static readonly double DOUBLE_UNIT = 0x1.0p - 53; // 1.0 / (1L << 53)

		// IllegalArgumentException messages
		internal const String BadBound = "bound must be positive";
		internal const String BadRange = "bound must be greater than origin";
		internal const String BadSize = "size must be non-negative";

		/// <summary>
		/// Creates a new random number generator. This constructor sets
		/// the seed of the random number generator to a value very likely
		/// to be distinct from any other invocation of this constructor.
		/// </summary>
		public Random() : this(SeedUniquifier() ^ System.nanoTime())
		{
		}

		private static long SeedUniquifier()
		{
			// L'Ecuyer, "Tables of Linear Congruential Generators of
			// Different Sizes and Good Lattice Structure", 1999
			for (;;)
			{
				long current = SeedUniquifier_Renamed.Get();
				long next = current * 181783497276652981L;
				if (SeedUniquifier_Renamed.CompareAndSet(current, next))
				{
					return next;
				}
			}
		}

		private static readonly AtomicLong SeedUniquifier_Renamed = new AtomicLong(8682522807148012L);

		/// <summary>
		/// Creates a new random number generator using a single {@code long} seed.
		/// The seed is the initial value of the internal state of the pseudorandom
		/// number generator which is maintained by method <seealso cref="#next"/>.
		/// 
		/// <para>The invocation {@code new Random(seed)} is equivalent to:
		///  <pre> {@code
		/// Random rnd = new Random();
		/// rnd.setSeed(seed);}</pre>
		/// 
		/// </para>
		/// </summary>
		/// <param name="seed"> the initial seed </param>
		/// <seealso cref=   #setSeed(long) </seealso>
		public Random(long seed)
		{
			if (this.GetType() == typeof(Random))
			{
				this.Seed_Renamed = new AtomicLong(InitialScramble(seed));
			}
			else
			{
				// subclass might have overriden setSeed
				this.Seed_Renamed = new AtomicLong();
				Seed = seed;
			}
		}

		private static long InitialScramble(long seed)
		{
			return (seed ^ Multiplier) & Mask;
		}

		/// <summary>
		/// Sets the seed of this random number generator using a single
		/// {@code long} seed. The general contract of {@code setSeed} is
		/// that it alters the state of this random number generator object
		/// so as to be in exactly the same state as if it had just been
		/// created with the argument {@code seed} as a seed. The method
		/// {@code setSeed} is implemented by class {@code Random} by
		/// atomically updating the seed to
		///  <pre>{@code (seed ^ 0x5DEECE66DL) & ((1L << 48) - 1)}</pre>
		/// and clearing the {@code haveNextNextGaussian} flag used by {@link
		/// #nextGaussian}.
		/// 
		/// <para>The implementation of {@code setSeed} by class {@code Random}
		/// happens to use only 48 bits of the given seed. In general, however,
		/// an overriding method may use all 64 bits of the {@code long}
		/// argument as a seed value.
		/// 
		/// </para>
		/// </summary>
		/// <param name="seed"> the initial seed </param>
		public virtual long Seed
		{
			set
			{
				lock (this)
				{
					this.Seed_Renamed.Set(InitialScramble(value));
					HaveNextNextGaussian = false;
				}
			}
		}

		/// <summary>
		/// Generates the next pseudorandom number. Subclasses should
		/// override this, as this is used by all other methods.
		/// 
		/// <para>The general contract of {@code next} is that it returns an
		/// {@code int} value and if the argument {@code bits} is between
		/// {@code 1} and {@code 32} (inclusive), then that many low-order
		/// bits of the returned value will be (approximately) independently
		/// chosen bit values, each of which is (approximately) equally
		/// likely to be {@code 0} or {@code 1}. The method {@code next} is
		/// implemented by class {@code Random} by atomically updating the seed to
		///  <pre>{@code (seed * 0x5DEECE66DL + 0xBL) & ((1L << 48) - 1)}</pre>
		/// and returning
		///  <pre>{@code (int)(seed >>> (48 - bits))}.</pre>
		/// 
		/// This is a linear congruential pseudorandom number generator, as
		/// defined by D. H. Lehmer and described by Donald E. Knuth in
		/// <i>The Art of Computer Programming,</i> Volume 3:
		/// <i>Seminumerical Algorithms</i>, section 3.2.1.
		/// 
		/// </para>
		/// </summary>
		/// <param name="bits"> random bits </param>
		/// <returns> the next pseudorandom value from this random number
		///         generator's sequence
		/// @since  1.1 </returns>
		protected internal virtual int Next(int bits)
		{
			long oldseed, nextseed;
			AtomicLong seed = this.Seed_Renamed;
			do
			{
				oldseed = seed.Get();
				nextseed = (oldseed * Multiplier + Addend) & Mask;
			} while (!seed.CompareAndSet(oldseed, nextseed));
			return (int)((long)((ulong)nextseed >> (48 - bits)));
		}

		/// <summary>
		/// Generates random bytes and places them into a user-supplied
		/// byte array.  The number of random bytes produced is equal to
		/// the length of the byte array.
		/// 
		/// <para>The method {@code nextBytes} is implemented by class {@code Random}
		/// as if by:
		///  <pre> {@code
		/// public void nextBytes(byte[] bytes) {
		///   for (int i = 0; i < bytes.length; )
		///     for (int rnd = nextInt(), n = Math.min(bytes.length - i, 4);
		///          n-- > 0; rnd >>= 8)
		///       bytes[i++] = (byte)rnd;
		/// }}</pre>
		/// 
		/// </para>
		/// </summary>
		/// <param name="bytes"> the byte array to fill with random bytes </param>
		/// <exception cref="NullPointerException"> if the byte array is null
		/// @since  1.1 </exception>
		public virtual void NextBytes(sbyte[] bytes)
		{
			for (int i = 0, len = bytes.Length; i < len;)
			{
				for (int rnd = NextInt(), n = System.Math.Min(len - i, sizeof(int) / sizeof(sbyte)); n-- > 0; rnd >>= sizeof(sbyte))
				{
					bytes[i++] = (sbyte)rnd;
				}
			}
		}

		/// <summary>
		/// The form of nextLong used by LongStream Spliterators.  If
		/// origin is greater than bound, acts as unbounded form of
		/// nextLong, else as bounded form.
		/// </summary>
		/// <param name="origin"> the least value, unless greater than bound </param>
		/// <param name="bound"> the upper bound (exclusive), must not equal origin </param>
		/// <returns> a pseudorandom value </returns>
		internal long InternalNextLong(long origin, long bound)
		{
			long r = NextLong();
			if (origin < bound)
			{
				long n = bound - origin, m = n - 1;
				if ((n & m) == 0L) // power of two
				{
					r = (r & m) + origin;
				}
				else if (n > 0L) // reject over-represented candidates
				{
					for (long u = (long)((ulong)r >> 1); u + m - (r = u % n) < 0L; u = (int)((uint)NextLong() >> 1)) // retry -  rejection check -  ensure nonnegative
					{
						;
					}
					r += origin;
				}
				else // range not representable as long
				{
					while (r < origin || r >= bound)
					{
						r = NextLong();
					}
				}
			}
			return r;
		}

		/// <summary>
		/// The form of nextInt used by IntStream Spliterators.
		/// For the unbounded case: uses nextInt().
		/// For the bounded case with representable range: uses nextInt(int bound)
		/// For the bounded case with unrepresentable range: uses nextInt()
		/// </summary>
		/// <param name="origin"> the least value, unless greater than bound </param>
		/// <param name="bound"> the upper bound (exclusive), must not equal origin </param>
		/// <returns> a pseudorandom value </returns>
		internal int InternalNextInt(int origin, int bound)
		{
			if (origin < bound)
			{
				int n = bound - origin;
				if (n > 0)
				{
					return NextInt(n) + origin;
				}
				else // range not representable as int
				{
					int r;
					do
					{
						r = NextInt();
					} while (r < origin || r >= bound);
					return r;
				}
			}
			else
			{
				return NextInt();
			}
		}

		/// <summary>
		/// The form of nextDouble used by DoubleStream Spliterators.
		/// </summary>
		/// <param name="origin"> the least value, unless greater than bound </param>
		/// <param name="bound"> the upper bound (exclusive), must not equal origin </param>
		/// <returns> a pseudorandom value </returns>
		internal double InternalNextDouble(double origin, double bound)
		{
			double r = NextDouble();
			if (origin < bound)
			{
				r = r * (bound - origin) + origin;
				if (r >= bound) // correct for rounding
				{
					r = Double.longBitsToDouble(Double.DoubleToLongBits(bound) - 1);
				}
			}
			return r;
		}

		/// <summary>
		/// Returns the next pseudorandom, uniformly distributed {@code int}
		/// value from this random number generator's sequence. The general
		/// contract of {@code nextInt} is that one {@code int} value is
		/// pseudorandomly generated and returned. All 2<sup>32</sup> possible
		/// {@code int} values are produced with (approximately) equal probability.
		/// 
		/// <para>The method {@code nextInt} is implemented by class {@code Random}
		/// as if by:
		///  <pre> {@code
		/// public int nextInt() {
		///   return next(32);
		/// }}</pre>
		/// 
		/// </para>
		/// </summary>
		/// <returns> the next pseudorandom, uniformly distributed {@code int}
		///         value from this random number generator's sequence </returns>
		public virtual int NextInt()
		{
			return Next(32);
		}

		/// <summary>
		/// Returns a pseudorandom, uniformly distributed {@code int} value
		/// between 0 (inclusive) and the specified value (exclusive), drawn from
		/// this random number generator's sequence.  The general contract of
		/// {@code nextInt} is that one {@code int} value in the specified range
		/// is pseudorandomly generated and returned.  All {@code bound} possible
		/// {@code int} values are produced with (approximately) equal
		/// probability.  The method {@code nextInt(int bound)} is implemented by
		/// class {@code Random} as if by:
		///  <pre> {@code
		/// public int nextInt(int bound) {
		///   if (bound <= 0)
		///     throw new IllegalArgumentException("bound must be positive");
		/// 
		///   if ((bound & -bound) == bound)  // i.e., bound is a power of 2
		///     return (int)((bound * (long)next(31)) >> 31);
		/// 
		///   int bits, val;
		///   do {
		///       bits = next(31);
		///       val = bits % bound;
		///   } while (bits - val + (bound-1) < 0);
		///   return val;
		/// }}</pre>
		/// 
		/// <para>The hedge "approximately" is used in the foregoing description only
		/// because the next method is only approximately an unbiased source of
		/// independently chosen bits.  If it were a perfect source of randomly
		/// chosen bits, then the algorithm shown would choose {@code int}
		/// values from the stated range with perfect uniformity.
		/// </para>
		/// <para>
		/// The algorithm is slightly tricky.  It rejects values that would result
		/// in an uneven distribution (due to the fact that 2^31 is not divisible
		/// by n). The probability of a value being rejected depends on n.  The
		/// worst case is n=2^30+1, for which the probability of a reject is 1/2,
		/// and the expected number of iterations before the loop terminates is 2.
		/// </para>
		/// <para>
		/// The algorithm treats the case where n is a power of two specially: it
		/// returns the correct number of high-order bits from the underlying
		/// pseudo-random number generator.  In the absence of special treatment,
		/// the correct number of <i>low-order</i> bits would be returned.  Linear
		/// congruential pseudo-random number generators such as the one
		/// implemented by this class are known to have short periods in the
		/// sequence of values of their low-order bits.  Thus, this special case
		/// greatly increases the length of the sequence of values returned by
		/// successive calls to this method if n is a small power of two.
		/// 
		/// </para>
		/// </summary>
		/// <param name="bound"> the upper bound (exclusive).  Must be positive. </param>
		/// <returns> the next pseudorandom, uniformly distributed {@code int}
		///         value between zero (inclusive) and {@code bound} (exclusive)
		///         from this random number generator's sequence </returns>
		/// <exception cref="IllegalArgumentException"> if bound is not positive
		/// @since 1.2 </exception>
		public virtual int NextInt(int bound)
		{
			if (bound <= 0)
			{
				throw new IllegalArgumentException(BadBound);
			}

			int r = Next(31);
			int m = bound - 1;
			if ((bound & m) == 0) // i.e., bound is a power of 2
			{
				r = (int)((bound * (long)r) >> 31);
			}
			else
			{
				for (int u = r; u - (r = u % bound) + m < 0; u = Next(31))
				{
					;
				}
			}
			return r;
		}

		/// <summary>
		/// Returns the next pseudorandom, uniformly distributed {@code long}
		/// value from this random number generator's sequence. The general
		/// contract of {@code nextLong} is that one {@code long} value is
		/// pseudorandomly generated and returned.
		/// 
		/// <para>The method {@code nextLong} is implemented by class {@code Random}
		/// as if by:
		///  <pre> {@code
		/// public long nextLong() {
		///   return ((long)next(32) << 32) + next(32);
		/// }}</pre>
		/// 
		/// Because class {@code Random} uses a seed with only 48 bits,
		/// this algorithm will not return all possible {@code long} values.
		/// 
		/// </para>
		/// </summary>
		/// <returns> the next pseudorandom, uniformly distributed {@code long}
		///         value from this random number generator's sequence </returns>
		public virtual long NextLong()
		{
			// it's okay that the bottom word remains signed.
			return ((long)(Next(32)) << 32) + Next(32);
		}

		/// <summary>
		/// Returns the next pseudorandom, uniformly distributed
		/// {@code boolean} value from this random number generator's
		/// sequence. The general contract of {@code nextBoolean} is that one
		/// {@code boolean} value is pseudorandomly generated and returned.  The
		/// values {@code true} and {@code false} are produced with
		/// (approximately) equal probability.
		/// 
		/// <para>The method {@code nextBoolean} is implemented by class {@code Random}
		/// as if by:
		///  <pre> {@code
		/// public boolean nextBoolean() {
		///   return next(1) != 0;
		/// }}</pre>
		/// 
		/// </para>
		/// </summary>
		/// <returns> the next pseudorandom, uniformly distributed
		///         {@code boolean} value from this random number generator's
		///         sequence
		/// @since 1.2 </returns>
		public virtual bool NextBoolean()
		{
			return Next(1) != 0;
		}

		/// <summary>
		/// Returns the next pseudorandom, uniformly distributed {@code float}
		/// value between {@code 0.0} and {@code 1.0} from this random
		/// number generator's sequence.
		/// 
		/// <para>The general contract of {@code nextFloat} is that one
		/// {@code float} value, chosen (approximately) uniformly from the
		/// range {@code 0.0f} (inclusive) to {@code 1.0f} (exclusive), is
		/// pseudorandomly generated and returned. All 2<sup>24</sup> possible
		/// {@code float} values of the form <i>m&nbsp;x&nbsp;</i>2<sup>-24</sup>,
		/// where <i>m</i> is a positive integer less than 2<sup>24</sup>, are
		/// produced with (approximately) equal probability.
		/// 
		/// </para>
		/// <para>The method {@code nextFloat} is implemented by class {@code Random}
		/// as if by:
		///  <pre> {@code
		/// public float nextFloat() {
		///   return next(24) / ((float)(1 << 24));
		/// }}</pre>
		/// 
		/// </para>
		/// <para>The hedge "approximately" is used in the foregoing description only
		/// because the next method is only approximately an unbiased source of
		/// independently chosen bits. If it were a perfect source of randomly
		/// chosen bits, then the algorithm shown would choose {@code float}
		/// </para>
		/// values from the stated range with perfect uniformity.<para>
		/// [In early versions of Java, the result was incorrectly calculated as:
		///  <pre> {@code
		///   return next(30) / ((float)(1 << 30));}</pre>
		/// This might seem to be equivalent, if not better, but in fact it
		/// introduced a slight nonuniformity because of the bias in the rounding
		/// of floating-point numbers: it was slightly more likely that the
		/// low-order bit of the significand would be 0 than that it would be 1.]
		/// 
		/// </para>
		/// </summary>
		/// <returns> the next pseudorandom, uniformly distributed {@code float}
		///         value between {@code 0.0} and {@code 1.0} from this
		///         random number generator's sequence </returns>
		public virtual float NextFloat()
		{
			return Next(24) / ((float)(1 << 24));
		}

		/// <summary>
		/// Returns the next pseudorandom, uniformly distributed
		/// {@code double} value between {@code 0.0} and
		/// {@code 1.0} from this random number generator's sequence.
		/// 
		/// <para>The general contract of {@code nextDouble} is that one
		/// {@code double} value, chosen (approximately) uniformly from the
		/// range {@code 0.0d} (inclusive) to {@code 1.0d} (exclusive), is
		/// pseudorandomly generated and returned.
		/// 
		/// </para>
		/// <para>The method {@code nextDouble} is implemented by class {@code Random}
		/// as if by:
		///  <pre> {@code
		/// public double nextDouble() {
		///   return (((long)next(26) << 27) + next(27))
		///     / (double)(1L << 53);
		/// }}</pre>
		/// 
		/// </para>
		/// <para>The hedge "approximately" is used in the foregoing description only
		/// because the {@code next} method is only approximately an unbiased
		/// source of independently chosen bits. If it were a perfect source of
		/// randomly chosen bits, then the algorithm shown would choose
		/// {@code double} values from the stated range with perfect uniformity.
		/// </para>
		/// <para>[In early versions of Java, the result was incorrectly calculated as:
		///  <pre> {@code
		///   return (((long)next(27) << 27) + next(27))
		///     / (double)(1L << 54);}</pre>
		/// This might seem to be equivalent, if not better, but in fact it
		/// introduced a large nonuniformity because of the bias in the rounding
		/// of floating-point numbers: it was three times as likely that the
		/// low-order bit of the significand would be 0 than that it would be 1!
		/// This nonuniformity probably doesn't matter much in practice, but we
		/// strive for perfection.]
		/// 
		/// </para>
		/// </summary>
		/// <returns> the next pseudorandom, uniformly distributed {@code double}
		///         value between {@code 0.0} and {@code 1.0} from this
		///         random number generator's sequence </returns>
		/// <seealso cref= Math#random </seealso>
		public virtual double NextDouble()
		{
			return (((long)(Next(26)) << 27) + Next(27)) * DOUBLE_UNIT;
		}

		private double NextNextGaussian;
		private bool HaveNextNextGaussian = false;

		/// <summary>
		/// Returns the next pseudorandom, Gaussian ("normally") distributed
		/// {@code double} value with mean {@code 0.0} and standard
		/// deviation {@code 1.0} from this random number generator's sequence.
		/// <para>
		/// The general contract of {@code nextGaussian} is that one
		/// {@code double} value, chosen from (approximately) the usual
		/// normal distribution with mean {@code 0.0} and standard deviation
		/// {@code 1.0}, is pseudorandomly generated and returned.
		/// 
		/// </para>
		/// <para>The method {@code nextGaussian} is implemented by class
		/// {@code Random} as if by a threadsafe version of the following:
		///  <pre> {@code
		/// private double nextNextGaussian;
		/// private boolean haveNextNextGaussian = false;
		/// 
		/// public double nextGaussian() {
		///   if (haveNextNextGaussian) {
		///     haveNextNextGaussian = false;
		///     return nextNextGaussian;
		///   } else {
		///     double v1, v2, s;
		///     do {
		///       v1 = 2 * nextDouble() - 1;   // between -1.0 and 1.0
		///       v2 = 2 * nextDouble() - 1;   // between -1.0 and 1.0
		///       s = v1 * v1 + v2 * v2;
		///     } while (s >= 1 || s == 0);
		///     double multiplier = StrictMath.sqrt(-2 * StrictMath.log(s)/s);
		///     nextNextGaussian = v2 * multiplier;
		///     haveNextNextGaussian = true;
		///     return v1 * multiplier;
		///   }
		/// }}</pre>
		/// This uses the <i>polar method</i> of G. E. P. Box, M. E. Muller, and
		/// G. Marsaglia, as described by Donald E. Knuth in <i>The Art of
		/// Computer Programming</i>, Volume 3: <i>Seminumerical Algorithms</i>,
		/// section 3.4.1, subsection C, algorithm P. Note that it generates two
		/// independent values at the cost of only one call to {@code StrictMath.log}
		/// and one call to {@code StrictMath.sqrt}.
		/// 
		/// </para>
		/// </summary>
		/// <returns> the next pseudorandom, Gaussian ("normally") distributed
		///         {@code double} value with mean {@code 0.0} and
		///         standard deviation {@code 1.0} from this random number
		///         generator's sequence </returns>
		public virtual double NextGaussian()
		{
			lock (this)
			{
				// See Knuth, ACP, Section 3.4.1 Algorithm C.
				if (HaveNextNextGaussian)
				{
					HaveNextNextGaussian = false;
					return NextNextGaussian;
				}
				else
				{
					double v1, v2, s;
					do
					{
						v1 = 2 * NextDouble() - 1; // between -1 and 1
						v2 = 2 * NextDouble() - 1; // between -1 and 1
						s = v1 * v1 + v2 * v2;
					} while (s >= 1 || s == 0);
					double multiplier = System.Math.Sqrt(-2 * System.Math.Log(s) / s);
					NextNextGaussian = v2 * multiplier;
					HaveNextNextGaussian = true;
					return v1 * multiplier;
				}
			}
		}

		// stream methods, coded in a way intended to better isolate for
		// maintenance purposes the small differences across forms.

		/// <summary>
		/// Returns a stream producing the given {@code streamSize} number of
		/// pseudorandom {@code int} values.
		/// 
		/// <para>A pseudorandom {@code int} value is generated as if it's the result of
		/// calling the method <seealso cref="#nextInt()"/>.
		/// 
		/// </para>
		/// </summary>
		/// <param name="streamSize"> the number of values to generate </param>
		/// <returns> a stream of pseudorandom {@code int} values </returns>
		/// <exception cref="IllegalArgumentException"> if {@code streamSize} is
		///         less than zero
		/// @since 1.8 </exception>
		public virtual IntStream Ints(long streamSize)
		{
			if (streamSize < 0L)
			{
				throw new IllegalArgumentException(BadSize);
			}
			return StreamSupport.IntStream(new RandomIntsSpliterator(this, 0L, streamSize, Integer.MaxValue, 0), false);
		}

		/// <summary>
		/// Returns an effectively unlimited stream of pseudorandom {@code int}
		/// values.
		/// 
		/// <para>A pseudorandom {@code int} value is generated as if it's the result of
		/// calling the method <seealso cref="#nextInt()"/>.
		/// 
		/// @implNote This method is implemented to be equivalent to {@code
		/// ints(Long.MAX_VALUE)}.
		/// 
		/// </para>
		/// </summary>
		/// <returns> a stream of pseudorandom {@code int} values
		/// @since 1.8 </returns>
		public virtual IntStream Ints()
		{
			return StreamSupport.IntStream(new RandomIntsSpliterator(this, 0L, Long.MaxValue, Integer.MaxValue, 0), false);
		}

		/// <summary>
		/// Returns a stream producing the given {@code streamSize} number
		/// of pseudorandom {@code int} values, each conforming to the given
		/// origin (inclusive) and bound (exclusive).
		/// 
		/// <para>A pseudorandom {@code int} value is generated as if it's the result of
		/// calling the following method with the origin and bound:
		/// <pre> {@code
		/// int nextInt(int origin, int bound) {
		///   int n = bound - origin;
		///   if (n > 0) {
		///     return nextInt(n) + origin;
		///   }
		///   else {  // range not representable as int
		///     int r;
		///     do {
		///       r = nextInt();
		///     } while (r < origin || r >= bound);
		///     return r;
		///   }
		/// }}</pre>
		/// 
		/// </para>
		/// </summary>
		/// <param name="streamSize"> the number of values to generate </param>
		/// <param name="randomNumberOrigin"> the origin (inclusive) of each random value </param>
		/// <param name="randomNumberBound"> the bound (exclusive) of each random value </param>
		/// <returns> a stream of pseudorandom {@code int} values,
		///         each with the given origin (inclusive) and bound (exclusive) </returns>
		/// <exception cref="IllegalArgumentException"> if {@code streamSize} is
		///         less than zero, or {@code randomNumberOrigin}
		///         is greater than or equal to {@code randomNumberBound}
		/// @since 1.8 </exception>
		public virtual IntStream Ints(long streamSize, int randomNumberOrigin, int randomNumberBound)
		{
			if (streamSize < 0L)
			{
				throw new IllegalArgumentException(BadSize);
			}
			if (randomNumberOrigin >= randomNumberBound)
			{
				throw new IllegalArgumentException(BadRange);
			}
			return StreamSupport.IntStream(new RandomIntsSpliterator(this, 0L, streamSize, randomNumberOrigin, randomNumberBound), false);
		}

		/// <summary>
		/// Returns an effectively unlimited stream of pseudorandom {@code
		/// int} values, each conforming to the given origin (inclusive) and bound
		/// (exclusive).
		/// 
		/// <para>A pseudorandom {@code int} value is generated as if it's the result of
		/// calling the following method with the origin and bound:
		/// <pre> {@code
		/// int nextInt(int origin, int bound) {
		///   int n = bound - origin;
		///   if (n > 0) {
		///     return nextInt(n) + origin;
		///   }
		///   else {  // range not representable as int
		///     int r;
		///     do {
		///       r = nextInt();
		///     } while (r < origin || r >= bound);
		///     return r;
		///   }
		/// }}</pre>
		/// 
		/// @implNote This method is implemented to be equivalent to {@code
		/// ints(Long.MAX_VALUE, randomNumberOrigin, randomNumberBound)}.
		/// 
		/// </para>
		/// </summary>
		/// <param name="randomNumberOrigin"> the origin (inclusive) of each random value </param>
		/// <param name="randomNumberBound"> the bound (exclusive) of each random value </param>
		/// <returns> a stream of pseudorandom {@code int} values,
		///         each with the given origin (inclusive) and bound (exclusive) </returns>
		/// <exception cref="IllegalArgumentException"> if {@code randomNumberOrigin}
		///         is greater than or equal to {@code randomNumberBound}
		/// @since 1.8 </exception>
		public virtual IntStream Ints(int randomNumberOrigin, int randomNumberBound)
		{
			if (randomNumberOrigin >= randomNumberBound)
			{
				throw new IllegalArgumentException(BadRange);
			}
			return StreamSupport.IntStream(new RandomIntsSpliterator(this, 0L, Long.MaxValue, randomNumberOrigin, randomNumberBound), false);
		}

		/// <summary>
		/// Returns a stream producing the given {@code streamSize} number of
		/// pseudorandom {@code long} values.
		/// 
		/// <para>A pseudorandom {@code long} value is generated as if it's the result
		/// of calling the method <seealso cref="#nextLong()"/>.
		/// 
		/// </para>
		/// </summary>
		/// <param name="streamSize"> the number of values to generate </param>
		/// <returns> a stream of pseudorandom {@code long} values </returns>
		/// <exception cref="IllegalArgumentException"> if {@code streamSize} is
		///         less than zero
		/// @since 1.8 </exception>
		public virtual LongStream Longs(long streamSize)
		{
			if (streamSize < 0L)
			{
				throw new IllegalArgumentException(BadSize);
			}
			return StreamSupport.LongStream(new RandomLongsSpliterator(this, 0L, streamSize, Long.MaxValue, 0L), false);
		}

		/// <summary>
		/// Returns an effectively unlimited stream of pseudorandom {@code long}
		/// values.
		/// 
		/// <para>A pseudorandom {@code long} value is generated as if it's the result
		/// of calling the method <seealso cref="#nextLong()"/>.
		/// 
		/// @implNote This method is implemented to be equivalent to {@code
		/// longs(Long.MAX_VALUE)}.
		/// 
		/// </para>
		/// </summary>
		/// <returns> a stream of pseudorandom {@code long} values
		/// @since 1.8 </returns>
		public virtual LongStream Longs()
		{
			return StreamSupport.LongStream(new RandomLongsSpliterator(this, 0L, Long.MaxValue, Long.MaxValue, 0L), false);
		}

		/// <summary>
		/// Returns a stream producing the given {@code streamSize} number of
		/// pseudorandom {@code long}, each conforming to the given origin
		/// (inclusive) and bound (exclusive).
		/// 
		/// <para>A pseudorandom {@code long} value is generated as if it's the result
		/// of calling the following method with the origin and bound:
		/// <pre> {@code
		/// long nextLong(long origin, long bound) {
		///   long r = nextLong();
		///   long n = bound - origin, m = n - 1;
		///   if ((n & m) == 0L)  // power of two
		///     r = (r & m) + origin;
		///   else if (n > 0L) {  // reject over-represented candidates
		///     for (long u = r >>> 1;            // ensure nonnegative
		///          u + m - (r = u % n) < 0L;    // rejection check
		///          u = nextLong() >>> 1) // retry
		///         ;
		///     r += origin;
		///   }
		///   else {              // range not representable as long
		///     while (r < origin || r >= bound)
		///       r = nextLong();
		///   }
		///   return r;
		/// }}</pre>
		/// 
		/// </para>
		/// </summary>
		/// <param name="streamSize"> the number of values to generate </param>
		/// <param name="randomNumberOrigin"> the origin (inclusive) of each random value </param>
		/// <param name="randomNumberBound"> the bound (exclusive) of each random value </param>
		/// <returns> a stream of pseudorandom {@code long} values,
		///         each with the given origin (inclusive) and bound (exclusive) </returns>
		/// <exception cref="IllegalArgumentException"> if {@code streamSize} is
		///         less than zero, or {@code randomNumberOrigin}
		///         is greater than or equal to {@code randomNumberBound}
		/// @since 1.8 </exception>
		public virtual LongStream Longs(long streamSize, long randomNumberOrigin, long randomNumberBound)
		{
			if (streamSize < 0L)
			{
				throw new IllegalArgumentException(BadSize);
			}
			if (randomNumberOrigin >= randomNumberBound)
			{
				throw new IllegalArgumentException(BadRange);
			}
			return StreamSupport.LongStream(new RandomLongsSpliterator(this, 0L, streamSize, randomNumberOrigin, randomNumberBound), false);
		}

		/// <summary>
		/// Returns an effectively unlimited stream of pseudorandom {@code
		/// long} values, each conforming to the given origin (inclusive) and bound
		/// (exclusive).
		/// 
		/// <para>A pseudorandom {@code long} value is generated as if it's the result
		/// of calling the following method with the origin and bound:
		/// <pre> {@code
		/// long nextLong(long origin, long bound) {
		///   long r = nextLong();
		///   long n = bound - origin, m = n - 1;
		///   if ((n & m) == 0L)  // power of two
		///     r = (r & m) + origin;
		///   else if (n > 0L) {  // reject over-represented candidates
		///     for (long u = r >>> 1;            // ensure nonnegative
		///          u + m - (r = u % n) < 0L;    // rejection check
		///          u = nextLong() >>> 1) // retry
		///         ;
		///     r += origin;
		///   }
		///   else {              // range not representable as long
		///     while (r < origin || r >= bound)
		///       r = nextLong();
		///   }
		///   return r;
		/// }}</pre>
		/// 
		/// @implNote This method is implemented to be equivalent to {@code
		/// longs(Long.MAX_VALUE, randomNumberOrigin, randomNumberBound)}.
		/// 
		/// </para>
		/// </summary>
		/// <param name="randomNumberOrigin"> the origin (inclusive) of each random value </param>
		/// <param name="randomNumberBound"> the bound (exclusive) of each random value </param>
		/// <returns> a stream of pseudorandom {@code long} values,
		///         each with the given origin (inclusive) and bound (exclusive) </returns>
		/// <exception cref="IllegalArgumentException"> if {@code randomNumberOrigin}
		///         is greater than or equal to {@code randomNumberBound}
		/// @since 1.8 </exception>
		public virtual LongStream Longs(long randomNumberOrigin, long randomNumberBound)
		{
			if (randomNumberOrigin >= randomNumberBound)
			{
				throw new IllegalArgumentException(BadRange);
			}
			return StreamSupport.LongStream(new RandomLongsSpliterator(this, 0L, Long.MaxValue, randomNumberOrigin, randomNumberBound), false);
		}

		/// <summary>
		/// Returns a stream producing the given {@code streamSize} number of
		/// pseudorandom {@code double} values, each between zero
		/// (inclusive) and one (exclusive).
		/// 
		/// <para>A pseudorandom {@code double} value is generated as if it's the result
		/// of calling the method <seealso cref="#nextDouble()"/>.
		/// 
		/// </para>
		/// </summary>
		/// <param name="streamSize"> the number of values to generate </param>
		/// <returns> a stream of {@code double} values </returns>
		/// <exception cref="IllegalArgumentException"> if {@code streamSize} is
		///         less than zero
		/// @since 1.8 </exception>
		public virtual DoubleStream Doubles(long streamSize)
		{
			if (streamSize < 0L)
			{
				throw new IllegalArgumentException(BadSize);
			}
			return StreamSupport.DoubleStream(new RandomDoublesSpliterator(this, 0L, streamSize, Double.MaxValue, 0.0), false);
		}

		/// <summary>
		/// Returns an effectively unlimited stream of pseudorandom {@code
		/// double} values, each between zero (inclusive) and one
		/// (exclusive).
		/// 
		/// <para>A pseudorandom {@code double} value is generated as if it's the result
		/// of calling the method <seealso cref="#nextDouble()"/>.
		/// 
		/// @implNote This method is implemented to be equivalent to {@code
		/// doubles(Long.MAX_VALUE)}.
		/// 
		/// </para>
		/// </summary>
		/// <returns> a stream of pseudorandom {@code double} values
		/// @since 1.8 </returns>
		public virtual DoubleStream Doubles()
		{
			return StreamSupport.DoubleStream(new RandomDoublesSpliterator(this, 0L, Long.MaxValue, Double.MaxValue, 0.0), false);
		}

		/// <summary>
		/// Returns a stream producing the given {@code streamSize} number of
		/// pseudorandom {@code double} values, each conforming to the given origin
		/// (inclusive) and bound (exclusive).
		/// 
		/// <para>A pseudorandom {@code double} value is generated as if it's the result
		/// of calling the following method with the origin and bound:
		/// <pre> {@code
		/// double nextDouble(double origin, double bound) {
		///   double r = nextDouble();
		///   r = r * (bound - origin) + origin;
		///   if (r >= bound) // correct for rounding
		///     r = Math.nextDown(bound);
		///   return r;
		/// }}</pre>
		/// 
		/// </para>
		/// </summary>
		/// <param name="streamSize"> the number of values to generate </param>
		/// <param name="randomNumberOrigin"> the origin (inclusive) of each random value </param>
		/// <param name="randomNumberBound"> the bound (exclusive) of each random value </param>
		/// <returns> a stream of pseudorandom {@code double} values,
		///         each with the given origin (inclusive) and bound (exclusive) </returns>
		/// <exception cref="IllegalArgumentException"> if {@code streamSize} is
		///         less than zero </exception>
		/// <exception cref="IllegalArgumentException"> if {@code randomNumberOrigin}
		///         is greater than or equal to {@code randomNumberBound}
		/// @since 1.8 </exception>
		public virtual DoubleStream Doubles(long streamSize, double randomNumberOrigin, double randomNumberBound)
		{
			if (streamSize < 0L)
			{
				throw new IllegalArgumentException(BadSize);
			}
			if (!(randomNumberOrigin < randomNumberBound))
			{
				throw new IllegalArgumentException(BadRange);
			}
			return StreamSupport.DoubleStream(new RandomDoublesSpliterator(this, 0L, streamSize, randomNumberOrigin, randomNumberBound), false);
		}

		/// <summary>
		/// Returns an effectively unlimited stream of pseudorandom {@code
		/// double} values, each conforming to the given origin (inclusive) and bound
		/// (exclusive).
		/// 
		/// <para>A pseudorandom {@code double} value is generated as if it's the result
		/// of calling the following method with the origin and bound:
		/// <pre> {@code
		/// double nextDouble(double origin, double bound) {
		///   double r = nextDouble();
		///   r = r * (bound - origin) + origin;
		///   if (r >= bound) // correct for rounding
		///     r = Math.nextDown(bound);
		///   return r;
		/// }}</pre>
		/// 
		/// @implNote This method is implemented to be equivalent to {@code
		/// doubles(Long.MAX_VALUE, randomNumberOrigin, randomNumberBound)}.
		/// 
		/// </para>
		/// </summary>
		/// <param name="randomNumberOrigin"> the origin (inclusive) of each random value </param>
		/// <param name="randomNumberBound"> the bound (exclusive) of each random value </param>
		/// <returns> a stream of pseudorandom {@code double} values,
		///         each with the given origin (inclusive) and bound (exclusive) </returns>
		/// <exception cref="IllegalArgumentException"> if {@code randomNumberOrigin}
		///         is greater than or equal to {@code randomNumberBound}
		/// @since 1.8 </exception>
		public virtual DoubleStream Doubles(double randomNumberOrigin, double randomNumberBound)
		{
			if (!(randomNumberOrigin < randomNumberBound))
			{
				throw new IllegalArgumentException(BadRange);
			}
			return StreamSupport.DoubleStream(new RandomDoublesSpliterator(this, 0L, Long.MaxValue, randomNumberOrigin, randomNumberBound), false);
		}

		/// <summary>
		/// Spliterator for int streams.  We multiplex the four int
		/// versions into one class by treating a bound less than origin as
		/// unbounded, and also by treating "infinite" as equivalent to
		/// Long.MAX_VALUE. For splits, it uses the standard divide-by-two
		/// approach. The long and double versions of this class are
		/// identical except for types.
		/// </summary>
		internal sealed class RandomIntsSpliterator : Spliterator_OfInt
		{
			internal readonly Random Rng;
			internal long Index;
			internal readonly long Fence;
			internal readonly int Origin;
			internal readonly int Bound;
			internal RandomIntsSpliterator(Random rng, long index, long fence, int origin, int bound)
			{
				this.Rng = rng;
				this.Index = index;
				this.Fence = fence;
				this.Origin = origin;
				this.Bound = bound;
			}

			public RandomIntsSpliterator TrySplit()
			{
				long i = Index, m = (int)((uint)(i + Fence) >> 1);
				return (m <= i) ? null : new RandomIntsSpliterator(Rng, i, Index = m, Origin, Bound);
			}

			public long EstimateSize()
			{
				return Fence - Index;
			}

			public int Characteristics()
			{
				return (Spliterator_Fields.SIZED | Spliterator_Fields.SUBSIZED | Spliterator_Fields.NONNULL | Spliterator_Fields.IMMUTABLE);
			}

			public bool TryAdvance(IntConsumer consumer)
			{
				if (consumer == null)
				{
					throw new NullPointerException();
				}
				long i = Index, f = Fence;
				if (i < f)
				{
					consumer.Accept(Rng.InternalNextInt(Origin, Bound));
					Index = i + 1;
					return true;
				}
				return false;
			}

			public void ForEachRemaining(IntConsumer consumer)
			{
				if (consumer == null)
				{
					throw new NullPointerException();
				}
				long i = Index, f = Fence;
				if (i < f)
				{
					Index = f;
					Random r = Rng;
					int o = Origin, b = Bound;
					do
					{
						consumer.Accept(r.InternalNextInt(o, b));
					} while (++i < f);
				}
			}
		}

		/// <summary>
		/// Spliterator for long streams.
		/// </summary>
		internal sealed class RandomLongsSpliterator : Spliterator_OfLong
		{
			internal readonly Random Rng;
			internal long Index;
			internal readonly long Fence;
			internal readonly long Origin;
			internal readonly long Bound;
			internal RandomLongsSpliterator(Random rng, long index, long fence, long origin, long bound)
			{
				this.Rng = rng;
				this.Index = index;
				this.Fence = fence;
				this.Origin = origin;
				this.Bound = bound;
			}

			public RandomLongsSpliterator TrySplit()
			{
				long i = Index, m = (int)((uint)(i + Fence) >> 1);
				return (m <= i) ? null : new RandomLongsSpliterator(Rng, i, Index = m, Origin, Bound);
			}

			public long EstimateSize()
			{
				return Fence - Index;
			}

			public int Characteristics()
			{
				return (Spliterator_Fields.SIZED | Spliterator_Fields.SUBSIZED | Spliterator_Fields.NONNULL | Spliterator_Fields.IMMUTABLE);
			}

			public bool TryAdvance(LongConsumer consumer)
			{
				if (consumer == null)
				{
					throw new NullPointerException();
				}
				long i = Index, f = Fence;
				if (i < f)
				{
					consumer.Accept(Rng.InternalNextLong(Origin, Bound));
					Index = i + 1;
					return true;
				}
				return false;
			}

			public void ForEachRemaining(LongConsumer consumer)
			{
				if (consumer == null)
				{
					throw new NullPointerException();
				}
				long i = Index, f = Fence;
				if (i < f)
				{
					Index = f;
					Random r = Rng;
					long o = Origin, b = Bound;
					do
					{
						consumer.Accept(r.InternalNextLong(o, b));
					} while (++i < f);
				}
			}

		}

		/// <summary>
		/// Spliterator for double streams.
		/// </summary>
		internal sealed class RandomDoublesSpliterator : Spliterator_OfDouble
		{
			internal readonly Random Rng;
			internal long Index;
			internal readonly long Fence;
			internal readonly double Origin;
			internal readonly double Bound;
			internal RandomDoublesSpliterator(Random rng, long index, long fence, double origin, double bound)
			{
				this.Rng = rng;
				this.Index = index;
				this.Fence = fence;
				this.Origin = origin;
				this.Bound = bound;
			}

			public RandomDoublesSpliterator TrySplit()
			{
				long i = Index, m = (int)((uint)(i + Fence) >> 1);
				return (m <= i) ? null : new RandomDoublesSpliterator(Rng, i, Index = m, Origin, Bound);
			}

			public long EstimateSize()
			{
				return Fence - Index;
			}

			public int Characteristics()
			{
				return (Spliterator_Fields.SIZED | Spliterator_Fields.SUBSIZED | Spliterator_Fields.NONNULL | Spliterator_Fields.IMMUTABLE);
			}

			public bool TryAdvance(DoubleConsumer consumer)
			{
				if (consumer == null)
				{
					throw new NullPointerException();
				}
				long i = Index, f = Fence;
				if (i < f)
				{
					consumer.Accept(Rng.InternalNextDouble(Origin, Bound));
					Index = i + 1;
					return true;
				}
				return false;
			}

			public void ForEachRemaining(DoubleConsumer consumer)
			{
				if (consumer == null)
				{
					throw new NullPointerException();
				}
				long i = Index, f = Fence;
				if (i < f)
				{
					Index = f;
					Random r = Rng;
					double o = Origin, b = Bound;
					do
					{
						consumer.Accept(r.InternalNextDouble(o, b));
					} while (++i < f);
				}
			}
		}

		/// <summary>
		/// Serializable fields for Random.
		/// 
		/// @serialField    seed long
		///              seed for random computations
		/// @serialField    nextNextGaussian double
		///              next Gaussian to be returned
		/// @serialField      haveNextNextGaussian boolean
		///              nextNextGaussian is valid
		/// </summary>
		private static readonly ObjectStreamField[] SerialPersistentFields = new ObjectStreamField[] {new ObjectStreamField("seed", Long.TYPE), new ObjectStreamField("nextNextGaussian", Double.TYPE), new ObjectStreamField("haveNextNextGaussian", Boolean.TYPE)};

		/// <summary>
		/// Reconstitute the {@code Random} instance from a stream (that is,
		/// deserialize it).
		/// </summary>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: private void readObject(java.io.ObjectInputStream s) throws java.io.IOException, ClassNotFoundException
		private void ReadObject(java.io.ObjectInputStream s)
		{

			ObjectInputStream.GetField fields = s.ReadFields();

			// The seed is read in as {@code long} for
			// historical reasons, but it is converted to an AtomicLong.
			long seedVal = fields.Get("seed", -1L);
			if (seedVal < 0)
			{
			  throw new java.io.StreamCorruptedException("Random: invalid seed");
			}
			ResetSeed(seedVal);
			NextNextGaussian = fields.Get("nextNextGaussian", 0.0);
			HaveNextNextGaussian = fields.Get("haveNextNextGaussian", false);
		}

		/// <summary>
		/// Save the {@code Random} instance to a stream.
		/// </summary>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: synchronized private void writeObject(ObjectOutputStream s) throws IOException
		private void WriteObject(ObjectOutputStream s)
		{
			lock (this)
			{
        
				// set the values of the Serializable fields
				ObjectOutputStream.PutField fields = s.PutFields();
        
				// The seed is serialized as a long for historical reasons.
				fields.Put("seed", Seed_Renamed.Get());
				fields.Put("nextNextGaussian", NextNextGaussian);
				fields.Put("haveNextNextGaussian", HaveNextNextGaussian);
        
				// save them
				s.WriteFields();
			}
		}

		// Support for resetting seed while deserializing
		private static readonly Unsafe @unsafe = Unsafe.Unsafe;
		private static readonly long SeedOffset;
		static Random()
		{
			try
			{
				SeedOffset = @unsafe.objectFieldOffset(typeof(Random).getDeclaredField("seed"));
			}
			catch (Exception ex)
			{
				throw new Error(ex);
			}
		}
		private void ResetSeed(long seedVal)
		{
			@unsafe.putObjectVolatile(this, SeedOffset, new AtomicLong(seedVal));
		}
	}

}
using System;
using System.Threading;

/*
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
 *
 *
 *
 *
 *
 * Written by Doug Lea with assistance from members of JCP JSR-166
 * Expert Group and released to the public domain, as explained at
 * http://creativecommons.org/publicdomain/zero/1.0/
 */

namespace java.util.concurrent
{


	/// <summary>
	/// A random number generator isolated to the current thread.  Like the
	/// global <seealso cref="java.util.Random"/> generator used by the {@link
	/// java.lang.Math} class, a {@code ThreadLocalRandom} is initialized
	/// with an internally generated seed that may not otherwise be
	/// modified. When applicable, use of {@code ThreadLocalRandom} rather
	/// than shared {@code Random} objects in concurrent programs will
	/// typically encounter much less overhead and contention.  Use of
	/// {@code ThreadLocalRandom} is particularly appropriate when multiple
	/// tasks (for example, each a <seealso cref="ForkJoinTask"/>) use random numbers
	/// in parallel in thread pools.
	/// 
	/// <para>Usages of this class should typically be of the form:
	/// {@code ThreadLocalRandom.current().nextX(...)} (where
	/// {@code X} is {@code Int}, {@code Long}, etc).
	/// When all usages are of this form, it is never possible to
	/// accidently share a {@code ThreadLocalRandom} across multiple threads.
	/// 
	/// </para>
	/// <para>This class also provides additional commonly used bounded random
	/// generation methods.
	/// 
	/// </para>
	/// <para>Instances of {@code ThreadLocalRandom} are not cryptographically
	/// secure.  Consider instead using <seealso cref="java.security.SecureRandom"/>
	/// in security-sensitive applications. Additionally,
	/// default-constructed instances do not use a cryptographically random
	/// seed unless the <seealso cref="System#getProperty system property"/>
	/// {@code java.util.secureRandomSeed} is set to {@code true}.
	/// 
	/// @since 1.7
	/// @author Doug Lea
	/// </para>
	/// </summary>
	public class ThreadLocalRandom : Random
	{
		/*
		 * This class implements the java.util.Random API (and subclasses
		 * Random) using a single static instance that accesses random
		 * number state held in class Thread (primarily, field
		 * threadLocalRandomSeed). In doing so, it also provides a home
		 * for managing package-private utilities that rely on exactly the
		 * same state as needed to maintain the ThreadLocalRandom
		 * instances. We leverage the need for an initialization flag
		 * field to also use it as a "probe" -- a self-adjusting thread
		 * hash used for contention avoidance, as well as a secondary
		 * simpler (xorShift) random seed that is conservatively used to
		 * avoid otherwise surprising users by hijacking the
		 * ThreadLocalRandom sequence.  The dual use is a marriage of
		 * convenience, but is a simple and efficient way of reducing
		 * application-level overhead and footprint of most concurrent
		 * programs.
		 *
		 * Even though this class subclasses java.util.Random, it uses the
		 * same basic algorithm as java.util.SplittableRandom.  (See its
		 * internal documentation for explanations, which are not repeated
		 * here.)  Because ThreadLocalRandoms are not splittable
		 * though, we use only a single 64bit gamma.
		 *
		 * Because this class is in a different package than class Thread,
		 * field access methods use Unsafe to bypass access control rules.
		 * To conform to the requirements of the Random superclass
		 * constructor, the common static ThreadLocalRandom maintains an
		 * "initialized" field for the sake of rejecting user calls to
		 * setSeed while still allowing a call from constructor.  Note
		 * that serialization is completely unnecessary because there is
		 * only a static singleton.  But we generate a serial form
		 * containing "rnd" and "initialized" fields to ensure
		 * compatibility across versions.
		 *
		 * Implementations of non-core methods are mostly the same as in
		 * SplittableRandom, that were in part derived from a previous
		 * version of this class.
		 *
		 * The nextLocalGaussian ThreadLocal supports the very rarely used
		 * nextGaussian method by providing a holder for the second of a
		 * pair of them. As is true for the base class version of this
		 * method, this time/space tradeoff is probably never worthwhile,
		 * but we provide identical statistical properties.
		 */

		/// <summary>
		/// Generates per-thread initialization/probe field </summary>
		private static readonly AtomicInteger ProbeGenerator = new AtomicInteger();

		/// <summary>
		/// The next seed for default constructors.
		/// </summary>
		private static readonly AtomicLong Seeder = new AtomicLong(InitialSeed());

		private static long InitialSeed()
		{
			String pp = java.security.AccessController.doPrivileged(new sun.security.action.GetPropertyAction("java.util.secureRandomSeed"));
			if (pp != null && pp.EqualsIgnoreCase("true"))
			{
				sbyte[] seedBytes = java.security.SecureRandom.GetSeed(8);
				long s = (long)(seedBytes[0]) & 0xffL;
				for (int i = 1; i < 8; ++i)
				{
					s = (s << 8) | ((long)(seedBytes[i]) & 0xffL);
				}
				return s;
			}
			return (Mix64(DateTimeHelperClass.CurrentUnixTimeMillis()) ^ Mix64(System.nanoTime()));
		}

		/// <summary>
		/// The seed increment
		/// </summary>
		private const long GAMMA = unchecked((long)0x9e3779b97f4a7c15L);

		/// <summary>
		/// The increment for generating probe values
		/// </summary>
		private const int PROBE_INCREMENT = unchecked((int)0x9e3779b9);

		/// <summary>
		/// The increment of seeder per new instance
		/// </summary>
		private const long SEEDER_INCREMENT = unchecked((long)0xbb67ae8584caa73bL);

		// Constants from SplittableRandom
		private static readonly double DOUBLE_UNIT = 0x1.0p - 53; // 1.0  / (1L << 53)
		private static readonly float FLOAT_UNIT = 0x1.0p - 24f; // 1.0f / (1 << 24)

		/// <summary>
		/// Rarely-used holder for the second of a pair of Gaussians </summary>
		private static readonly ThreadLocal<Double> NextLocalGaussian = new ThreadLocal<Double>();

		private static long Mix64(long z)
		{
			z = (z ^ ((long)((ulong)z >> 33))) * unchecked((long)0xff51afd7ed558ccdL);
			z = (z ^ ((long)((ulong)z >> 33))) * unchecked((long)0xc4ceb9fe1a85ec53L);
			return z ^ ((long)((ulong)z >> 33));
		}

		private static int Mix32(long z)
		{
			z = (z ^ ((long)((ulong)z >> 33))) * unchecked((long)0xff51afd7ed558ccdL);
			return (int)((int)((uint)((z ^ ((long)((ulong)z >> 33))) * 0xc4ceb9fe1a85ec53L) >> 32));
		}

		/// <summary>
		/// Field used only during singleton initialization.
		/// True when constructor completes.
		/// </summary>
		internal bool Initialized;

		/// <summary>
		/// Constructor used only for static singleton </summary>
		private ThreadLocalRandom()
		{
			Initialized = true; // false during super() call
		}

		/// <summary>
		/// The common ThreadLocalRandom </summary>
		internal static readonly ThreadLocalRandom Instance = new ThreadLocalRandom();

		/// <summary>
		/// Initialize Thread fields for the current thread.  Called only
		/// when Thread.threadLocalRandomProbe is zero, indicating that a
		/// thread local seed value needs to be generated. Note that even
		/// though the initialization is purely thread-local, we need to
		/// rely on (static) atomic generators to initialize the values.
		/// </summary>
		internal static void LocalInit()
		{
			int p = ProbeGenerator.AddAndGet(PROBE_INCREMENT);
			int probe = (p == 0) ? 1 : p; // skip 0
			long seed = Mix64(Seeder.GetAndAdd(SEEDER_INCREMENT));
			Thread t = Thread.CurrentThread;
			UNSAFE.putLong(t, SEED, seed);
			UNSAFE.putInt(t, PROBE, probe);
		}

		/// <summary>
		/// Returns the current thread's {@code ThreadLocalRandom}.
		/// </summary>
		/// <returns> the current thread's {@code ThreadLocalRandom} </returns>
		public static ThreadLocalRandom Current()
		{
			if (UNSAFE.getInt(Thread.CurrentThread, PROBE) == 0)
			{
				LocalInit();
			}
			return Instance;
		}

		/// <summary>
		/// Throws {@code UnsupportedOperationException}.  Setting seeds in
		/// this generator is not supported.
		/// </summary>
		/// <exception cref="UnsupportedOperationException"> always </exception>
		public override long Seed
		{
			set
			{
				// only allow call from super() constructor
				if (Initialized)
				{
					throw new UnsupportedOperationException();
				}
			}
		}

		internal long NextSeed()
		{
			Thread t; // read and update per-thread seed
			long r;
			UNSAFE.putLong(t = Thread.CurrentThread, SEED, r = UNSAFE.getLong(t, SEED) + GAMMA);
			return r;
		}

		// We must define this, but never use it.
		protected internal override int Next(int bits)
		{
			return (int)((int)((uint)Mix64(NextSeed()) >> (64 - bits)));
		}

		// IllegalArgumentException messages
		internal new const String BadBound = "bound must be positive";
		internal new const String BadRange = "bound must be greater than origin";
		internal new const String BadSize = "size must be non-negative";

		/// <summary>
		/// The form of nextLong used by LongStream Spliterators.  If
		/// origin is greater than bound, acts as unbounded form of
		/// nextLong, else as bounded form.
		/// </summary>
		/// <param name="origin"> the least value, unless greater than bound </param>
		/// <param name="bound"> the upper bound (exclusive), must not equal origin </param>
		/// <returns> a pseudorandom value </returns>
		internal sealed override long InternalNextLong(long origin, long bound)
		{
			long r = Mix64(NextSeed());
			if (origin < bound)
			{
				long n = bound - origin, m = n - 1;
				if ((n & m) == 0L) // power of two
				{
					r = (r & m) + origin;
				}
				else if (n > 0L) // reject over-represented candidates
				{
					for (long u = (long)((ulong)r >> 1); u + m - (r = u % n) < 0L; u = (int)((uint)Mix64(NextSeed()) >> 1)) // retry -  rejection check -  ensure nonnegative
					{
						;
					}
					r += origin;
				}
				else // range not representable as long
				{
					while (r < origin || r >= bound)
					{
						r = Mix64(NextSeed());
					}
				}
			}
			return r;
		}

		/// <summary>
		/// The form of nextInt used by IntStream Spliterators.
		/// Exactly the same as long version, except for types.
		/// </summary>
		/// <param name="origin"> the least value, unless greater than bound </param>
		/// <param name="bound"> the upper bound (exclusive), must not equal origin </param>
		/// <returns> a pseudorandom value </returns>
		internal sealed override int InternalNextInt(int origin, int bound)
		{
			int r = Mix32(NextSeed());
			if (origin < bound)
			{
				int n = bound - origin, m = n - 1;
				if ((n & m) == 0)
				{
					r = (r & m) + origin;
				}
				else if (n > 0)
				{
					for (int u = (int)((uint)r >> 1); u + m - (r = u % n) < 0; u = (int)((uint)Mix32(NextSeed()) >> 1))
					{
						;
					}
					r += origin;
				}
				else
				{
					while (r < origin || r >= bound)
					{
						r = Mix32(NextSeed());
					}
				}
			}
			return r;
		}

		/// <summary>
		/// The form of nextDouble used by DoubleStream Spliterators.
		/// </summary>
		/// <param name="origin"> the least value, unless greater than bound </param>
		/// <param name="bound"> the upper bound (exclusive), must not equal origin </param>
		/// <returns> a pseudorandom value </returns>
		internal sealed override double InternalNextDouble(double origin, double bound)
		{
			double r = ((int)((uint)NextLong() >> 11)) * DOUBLE_UNIT;
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
		/// Returns a pseudorandom {@code int} value.
		/// </summary>
		/// <returns> a pseudorandom {@code int} value </returns>
		public override int NextInt()
		{
			return Mix32(NextSeed());
		}

		/// <summary>
		/// Returns a pseudorandom {@code int} value between zero (inclusive)
		/// and the specified bound (exclusive).
		/// </summary>
		/// <param name="bound"> the upper bound (exclusive).  Must be positive. </param>
		/// <returns> a pseudorandom {@code int} value between zero
		///         (inclusive) and the bound (exclusive) </returns>
		/// <exception cref="IllegalArgumentException"> if {@code bound} is not positive </exception>
		public override int NextInt(int bound)
		{
			if (bound <= 0)
			{
				throw new IllegalArgumentException(BadBound);
			}
			int r = Mix32(NextSeed());
			int m = bound - 1;
			if ((bound & m) == 0) // power of two
			{
				r &= m;
			}
			else // reject over-represented candidates
			{
				for (int u = (int)((uint)r >> 1); u + m - (r = u % bound) < 0; u = (int)((uint)Mix32(NextSeed()) >> 1))
				{
					;
				}
			}
			return r;
		}

		/// <summary>
		/// Returns a pseudorandom {@code int} value between the specified
		/// origin (inclusive) and the specified bound (exclusive).
		/// </summary>
		/// <param name="origin"> the least value returned </param>
		/// <param name="bound"> the upper bound (exclusive) </param>
		/// <returns> a pseudorandom {@code int} value between the origin
		///         (inclusive) and the bound (exclusive) </returns>
		/// <exception cref="IllegalArgumentException"> if {@code origin} is greater than
		///         or equal to {@code bound} </exception>
		public virtual int NextInt(int origin, int bound)
		{
			if (origin >= bound)
			{
				throw new IllegalArgumentException(BadRange);
			}
			return InternalNextInt(origin, bound);
		}

		/// <summary>
		/// Returns a pseudorandom {@code long} value.
		/// </summary>
		/// <returns> a pseudorandom {@code long} value </returns>
		public override long NextLong()
		{
			return Mix64(NextSeed());
		}

		/// <summary>
		/// Returns a pseudorandom {@code long} value between zero (inclusive)
		/// and the specified bound (exclusive).
		/// </summary>
		/// <param name="bound"> the upper bound (exclusive).  Must be positive. </param>
		/// <returns> a pseudorandom {@code long} value between zero
		///         (inclusive) and the bound (exclusive) </returns>
		/// <exception cref="IllegalArgumentException"> if {@code bound} is not positive </exception>
		public virtual long NextLong(long bound)
		{
			if (bound <= 0)
			{
				throw new IllegalArgumentException(BadBound);
			}
			long r = Mix64(NextSeed());
			long m = bound - 1;
			if ((bound & m) == 0L) // power of two
			{
				r &= m;
			}
			else // reject over-represented candidates
			{
				for (long u = (long)((ulong)r >> 1); u + m - (r = u % bound) < 0L; u = (int)((uint)Mix64(NextSeed()) >> 1))
				{
					;
				}
			}
			return r;
		}

		/// <summary>
		/// Returns a pseudorandom {@code long} value between the specified
		/// origin (inclusive) and the specified bound (exclusive).
		/// </summary>
		/// <param name="origin"> the least value returned </param>
		/// <param name="bound"> the upper bound (exclusive) </param>
		/// <returns> a pseudorandom {@code long} value between the origin
		///         (inclusive) and the bound (exclusive) </returns>
		/// <exception cref="IllegalArgumentException"> if {@code origin} is greater than
		///         or equal to {@code bound} </exception>
		public virtual long NextLong(long origin, long bound)
		{
			if (origin >= bound)
			{
				throw new IllegalArgumentException(BadRange);
			}
			return InternalNextLong(origin, bound);
		}

		/// <summary>
		/// Returns a pseudorandom {@code double} value between zero
		/// (inclusive) and one (exclusive).
		/// </summary>
		/// <returns> a pseudorandom {@code double} value between zero
		///         (inclusive) and one (exclusive) </returns>
		public override double NextDouble()
		{
			return ((int)((uint)Mix64(NextSeed()) >> 11)) * DOUBLE_UNIT;
		}

		/// <summary>
		/// Returns a pseudorandom {@code double} value between 0.0
		/// (inclusive) and the specified bound (exclusive).
		/// </summary>
		/// <param name="bound"> the upper bound (exclusive).  Must be positive. </param>
		/// <returns> a pseudorandom {@code double} value between zero
		///         (inclusive) and the bound (exclusive) </returns>
		/// <exception cref="IllegalArgumentException"> if {@code bound} is not positive </exception>
		public virtual double NextDouble(double bound)
		{
			if (!(bound > 0.0))
			{
				throw new IllegalArgumentException(BadBound);
			}
			double result = ((int)((uint)Mix64(NextSeed()) >> 11)) * DOUBLE_UNIT * bound;
			return (result < bound) ? result : Double.longBitsToDouble(Double.DoubleToLongBits(bound) - 1); // correct for rounding
		}

		/// <summary>
		/// Returns a pseudorandom {@code double} value between the specified
		/// origin (inclusive) and bound (exclusive).
		/// </summary>
		/// <param name="origin"> the least value returned </param>
		/// <param name="bound"> the upper bound (exclusive) </param>
		/// <returns> a pseudorandom {@code double} value between the origin
		///         (inclusive) and the bound (exclusive) </returns>
		/// <exception cref="IllegalArgumentException"> if {@code origin} is greater than
		///         or equal to {@code bound} </exception>
		public virtual double NextDouble(double origin, double bound)
		{
			if (!(origin < bound))
			{
				throw new IllegalArgumentException(BadRange);
			}
			return InternalNextDouble(origin, bound);
		}

		/// <summary>
		/// Returns a pseudorandom {@code boolean} value.
		/// </summary>
		/// <returns> a pseudorandom {@code boolean} value </returns>
		public override bool NextBoolean()
		{
			return Mix32(NextSeed()) < 0;
		}

		/// <summary>
		/// Returns a pseudorandom {@code float} value between zero
		/// (inclusive) and one (exclusive).
		/// </summary>
		/// <returns> a pseudorandom {@code float} value between zero
		///         (inclusive) and one (exclusive) </returns>
		public override float NextFloat()
		{
			return ((int)((uint)Mix32(NextSeed()) >> 8)) * FLOAT_UNIT;
		}

		public override double NextGaussian()
		{
			// Use nextLocalGaussian instead of nextGaussian field
			Double d = NextLocalGaussian.Get();
			if (d != null)
			{
				NextLocalGaussian.Set(null);
				return d.DoubleValue();
			}
			double v1, v2, s;
			do
			{
				v1 = 2 * NextDouble() - 1; // between -1 and 1
				v2 = 2 * NextDouble() - 1; // between -1 and 1
				s = v1 * v1 + v2 * v2;
			} while (s >= 1 || s == 0);
			double multiplier = System.Math.Sqrt(-2 * System.Math.Log(s) / s);
			NextLocalGaussian.Set(new Double(v2 * multiplier));
			return v1 * multiplier;
		}

		// stream methods, coded in a way intended to better isolate for
		// maintenance purposes the small differences across forms.

		/// <summary>
		/// Returns a stream producing the given {@code streamSize} number of
		/// pseudorandom {@code int} values.
		/// </summary>
		/// <param name="streamSize"> the number of values to generate </param>
		/// <returns> a stream of pseudorandom {@code int} values </returns>
		/// <exception cref="IllegalArgumentException"> if {@code streamSize} is
		///         less than zero
		/// @since 1.8 </exception>
		public override IntStream Ints(long streamSize)
		{
			if (streamSize < 0L)
			{
				throw new IllegalArgumentException(BadSize);
			}
			return StreamSupport.IntStream(new RandomIntsSpliterator(0L, streamSize, Integer.MaxValue, 0), false);
		}

		/// <summary>
		/// Returns an effectively unlimited stream of pseudorandom {@code int}
		/// values.
		/// 
		/// @implNote This method is implemented to be equivalent to {@code
		/// ints(Long.MAX_VALUE)}.
		/// </summary>
		/// <returns> a stream of pseudorandom {@code int} values
		/// @since 1.8 </returns>
		public override IntStream Ints()
		{
			return StreamSupport.IntStream(new RandomIntsSpliterator(0L, Long.MaxValue, Integer.MaxValue, 0), false);
		}

		/// <summary>
		/// Returns a stream producing the given {@code streamSize} number
		/// of pseudorandom {@code int} values, each conforming to the given
		/// origin (inclusive) and bound (exclusive).
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
		public override IntStream Ints(long streamSize, int randomNumberOrigin, int randomNumberBound)
		{
			if (streamSize < 0L)
			{
				throw new IllegalArgumentException(BadSize);
			}
			if (randomNumberOrigin >= randomNumberBound)
			{
				throw new IllegalArgumentException(BadRange);
			}
			return StreamSupport.IntStream(new RandomIntsSpliterator(0L, streamSize, randomNumberOrigin, randomNumberBound), false);
		}

		/// <summary>
		/// Returns an effectively unlimited stream of pseudorandom {@code
		/// int} values, each conforming to the given origin (inclusive) and bound
		/// (exclusive).
		/// 
		/// @implNote This method is implemented to be equivalent to {@code
		/// ints(Long.MAX_VALUE, randomNumberOrigin, randomNumberBound)}.
		/// </summary>
		/// <param name="randomNumberOrigin"> the origin (inclusive) of each random value </param>
		/// <param name="randomNumberBound"> the bound (exclusive) of each random value </param>
		/// <returns> a stream of pseudorandom {@code int} values,
		///         each with the given origin (inclusive) and bound (exclusive) </returns>
		/// <exception cref="IllegalArgumentException"> if {@code randomNumberOrigin}
		///         is greater than or equal to {@code randomNumberBound}
		/// @since 1.8 </exception>
		public override IntStream Ints(int randomNumberOrigin, int randomNumberBound)
		{
			if (randomNumberOrigin >= randomNumberBound)
			{
				throw new IllegalArgumentException(BadRange);
			}
			return StreamSupport.IntStream(new RandomIntsSpliterator(0L, Long.MaxValue, randomNumberOrigin, randomNumberBound), false);
		}

		/// <summary>
		/// Returns a stream producing the given {@code streamSize} number of
		/// pseudorandom {@code long} values.
		/// </summary>
		/// <param name="streamSize"> the number of values to generate </param>
		/// <returns> a stream of pseudorandom {@code long} values </returns>
		/// <exception cref="IllegalArgumentException"> if {@code streamSize} is
		///         less than zero
		/// @since 1.8 </exception>
		public override LongStream Longs(long streamSize)
		{
			if (streamSize < 0L)
			{
				throw new IllegalArgumentException(BadSize);
			}
			return StreamSupport.LongStream(new RandomLongsSpliterator(0L, streamSize, Long.MaxValue, 0L), false);
		}

		/// <summary>
		/// Returns an effectively unlimited stream of pseudorandom {@code long}
		/// values.
		/// 
		/// @implNote This method is implemented to be equivalent to {@code
		/// longs(Long.MAX_VALUE)}.
		/// </summary>
		/// <returns> a stream of pseudorandom {@code long} values
		/// @since 1.8 </returns>
		public override LongStream Longs()
		{
			return StreamSupport.LongStream(new RandomLongsSpliterator(0L, Long.MaxValue, Long.MaxValue, 0L), false);
		}

		/// <summary>
		/// Returns a stream producing the given {@code streamSize} number of
		/// pseudorandom {@code long}, each conforming to the given origin
		/// (inclusive) and bound (exclusive).
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
		public override LongStream Longs(long streamSize, long randomNumberOrigin, long randomNumberBound)
		{
			if (streamSize < 0L)
			{
				throw new IllegalArgumentException(BadSize);
			}
			if (randomNumberOrigin >= randomNumberBound)
			{
				throw new IllegalArgumentException(BadRange);
			}
			return StreamSupport.LongStream(new RandomLongsSpliterator(0L, streamSize, randomNumberOrigin, randomNumberBound), false);
		}

		/// <summary>
		/// Returns an effectively unlimited stream of pseudorandom {@code
		/// long} values, each conforming to the given origin (inclusive) and bound
		/// (exclusive).
		/// 
		/// @implNote This method is implemented to be equivalent to {@code
		/// longs(Long.MAX_VALUE, randomNumberOrigin, randomNumberBound)}.
		/// </summary>
		/// <param name="randomNumberOrigin"> the origin (inclusive) of each random value </param>
		/// <param name="randomNumberBound"> the bound (exclusive) of each random value </param>
		/// <returns> a stream of pseudorandom {@code long} values,
		///         each with the given origin (inclusive) and bound (exclusive) </returns>
		/// <exception cref="IllegalArgumentException"> if {@code randomNumberOrigin}
		///         is greater than or equal to {@code randomNumberBound}
		/// @since 1.8 </exception>
		public override LongStream Longs(long randomNumberOrigin, long randomNumberBound)
		{
			if (randomNumberOrigin >= randomNumberBound)
			{
				throw new IllegalArgumentException(BadRange);
			}
			return StreamSupport.LongStream(new RandomLongsSpliterator(0L, Long.MaxValue, randomNumberOrigin, randomNumberBound), false);
		}

		/// <summary>
		/// Returns a stream producing the given {@code streamSize} number of
		/// pseudorandom {@code double} values, each between zero
		/// (inclusive) and one (exclusive).
		/// </summary>
		/// <param name="streamSize"> the number of values to generate </param>
		/// <returns> a stream of {@code double} values </returns>
		/// <exception cref="IllegalArgumentException"> if {@code streamSize} is
		///         less than zero
		/// @since 1.8 </exception>
		public override DoubleStream Doubles(long streamSize)
		{
			if (streamSize < 0L)
			{
				throw new IllegalArgumentException(BadSize);
			}
			return StreamSupport.DoubleStream(new RandomDoublesSpliterator(0L, streamSize, Double.MaxValue, 0.0), false);
		}

		/// <summary>
		/// Returns an effectively unlimited stream of pseudorandom {@code
		/// double} values, each between zero (inclusive) and one
		/// (exclusive).
		/// 
		/// @implNote This method is implemented to be equivalent to {@code
		/// doubles(Long.MAX_VALUE)}.
		/// </summary>
		/// <returns> a stream of pseudorandom {@code double} values
		/// @since 1.8 </returns>
		public override DoubleStream Doubles()
		{
			return StreamSupport.DoubleStream(new RandomDoublesSpliterator(0L, Long.MaxValue, Double.MaxValue, 0.0), false);
		}

		/// <summary>
		/// Returns a stream producing the given {@code streamSize} number of
		/// pseudorandom {@code double} values, each conforming to the given origin
		/// (inclusive) and bound (exclusive).
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
		public override DoubleStream Doubles(long streamSize, double randomNumberOrigin, double randomNumberBound)
		{
			if (streamSize < 0L)
			{
				throw new IllegalArgumentException(BadSize);
			}
			if (!(randomNumberOrigin < randomNumberBound))
			{
				throw new IllegalArgumentException(BadRange);
			}
			return StreamSupport.DoubleStream(new RandomDoublesSpliterator(0L, streamSize, randomNumberOrigin, randomNumberBound), false);
		}

		/// <summary>
		/// Returns an effectively unlimited stream of pseudorandom {@code
		/// double} values, each conforming to the given origin (inclusive) and bound
		/// (exclusive).
		/// 
		/// @implNote This method is implemented to be equivalent to {@code
		/// doubles(Long.MAX_VALUE, randomNumberOrigin, randomNumberBound)}.
		/// </summary>
		/// <param name="randomNumberOrigin"> the origin (inclusive) of each random value </param>
		/// <param name="randomNumberBound"> the bound (exclusive) of each random value </param>
		/// <returns> a stream of pseudorandom {@code double} values,
		///         each with the given origin (inclusive) and bound (exclusive) </returns>
		/// <exception cref="IllegalArgumentException"> if {@code randomNumberOrigin}
		///         is greater than or equal to {@code randomNumberBound}
		/// @since 1.8 </exception>
		public override DoubleStream Doubles(double randomNumberOrigin, double randomNumberBound)
		{
			if (!(randomNumberOrigin < randomNumberBound))
			{
				throw new IllegalArgumentException(BadRange);
			}
			return StreamSupport.DoubleStream(new RandomDoublesSpliterator(0L, Long.MaxValue, randomNumberOrigin, randomNumberBound), false);
		}

		/// <summary>
		/// Spliterator for int streams.  We multiplex the four int
		/// versions into one class by treating a bound less than origin as
		/// unbounded, and also by treating "infinite" as equivalent to
		/// Long.MAX_VALUE. For splits, it uses the standard divide-by-two
		/// approach. The long and double versions of this class are
		/// identical except for types.
		/// </summary>
		internal sealed class RandomIntsSpliterator : java.util.Spliterator_OfInt
		{
			internal long Index;
			internal readonly long Fence;
			internal readonly int Origin;
			internal readonly int Bound;
			internal RandomIntsSpliterator(long index, long fence, int origin, int bound)
			{
				this.Index = index;
				this.Fence = fence;
				this.Origin = origin;
				this.Bound = bound;
			}

			public RandomIntsSpliterator TrySplit()
			{
				long i = Index, m = (int)((uint)(i + Fence) >> 1);
				return (m <= i) ? null : new RandomIntsSpliterator(i, Index = m, Origin, Bound);
			}

			public long EstimateSize()
			{
				return Fence - Index;
			}

			public int Characteristics()
			{
				return (java.util.Spliterator_Fields.SIZED | java.util.Spliterator_Fields.SUBSIZED | java.util.Spliterator_Fields.NONNULL | java.util.Spliterator_Fields.IMMUTABLE);
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
					consumer.Accept(ThreadLocalRandom.Current().InternalNextInt(Origin, Bound));
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
					int o = Origin, b = Bound;
					ThreadLocalRandom rng = ThreadLocalRandom.Current();
					do
					{
						consumer.Accept(rng.InternalNextInt(o, b));
					} while (++i < f);
				}
			}
		}

		/// <summary>
		/// Spliterator for long streams.
		/// </summary>
		internal sealed class RandomLongsSpliterator : java.util.Spliterator_OfLong
		{
			internal long Index;
			internal readonly long Fence;
			internal readonly long Origin;
			internal readonly long Bound;
			internal RandomLongsSpliterator(long index, long fence, long origin, long bound)
			{
				this.Index = index;
				this.Fence = fence;
				this.Origin = origin;
				this.Bound = bound;
			}

			public RandomLongsSpliterator TrySplit()
			{
				long i = Index, m = (int)((uint)(i + Fence) >> 1);
				return (m <= i) ? null : new RandomLongsSpliterator(i, Index = m, Origin, Bound);
			}

			public long EstimateSize()
			{
				return Fence - Index;
			}

			public int Characteristics()
			{
				return (java.util.Spliterator_Fields.SIZED | java.util.Spliterator_Fields.SUBSIZED | java.util.Spliterator_Fields.NONNULL | java.util.Spliterator_Fields.IMMUTABLE);
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
					consumer.Accept(ThreadLocalRandom.Current().InternalNextLong(Origin, Bound));
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
					long o = Origin, b = Bound;
					ThreadLocalRandom rng = ThreadLocalRandom.Current();
					do
					{
						consumer.Accept(rng.InternalNextLong(o, b));
					} while (++i < f);
				}
			}

		}

		/// <summary>
		/// Spliterator for double streams.
		/// </summary>
		internal sealed class RandomDoublesSpliterator : java.util.Spliterator_OfDouble
		{
			internal long Index;
			internal readonly long Fence;
			internal readonly double Origin;
			internal readonly double Bound;
			internal RandomDoublesSpliterator(long index, long fence, double origin, double bound)
			{
				this.Index = index;
				this.Fence = fence;
				this.Origin = origin;
				this.Bound = bound;
			}

			public RandomDoublesSpliterator TrySplit()
			{
				long i = Index, m = (int)((uint)(i + Fence) >> 1);
				return (m <= i) ? null : new RandomDoublesSpliterator(i, Index = m, Origin, Bound);
			}

			public long EstimateSize()
			{
				return Fence - Index;
			}

			public int Characteristics()
			{
				return (java.util.Spliterator_Fields.SIZED | java.util.Spliterator_Fields.SUBSIZED | java.util.Spliterator_Fields.NONNULL | java.util.Spliterator_Fields.IMMUTABLE);
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
					consumer.Accept(ThreadLocalRandom.Current().InternalNextDouble(Origin, Bound));
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
					double o = Origin, b = Bound;
					ThreadLocalRandom rng = ThreadLocalRandom.Current();
					do
					{
						consumer.Accept(rng.InternalNextDouble(o, b));
					} while (++i < f);
				}
			}
		}


		// Within-package utilities

		/*
		 * Descriptions of the usages of the methods below can be found in
		 * the classes that use them. Briefly, a thread's "probe" value is
		 * a non-zero hash code that (probably) does not collide with
		 * other existing threads with respect to any power of two
		 * collision space. When it does collide, it is pseudo-randomly
		 * adjusted (using a Marsaglia XorShift). The nextSecondarySeed
		 * method is used in the same contexts as ThreadLocalRandom, but
		 * only for transient usages such as random adaptive spin/block
		 * sequences for which a cheap RNG suffices and for which it could
		 * in principle disrupt user-visible statistical properties of the
		 * main ThreadLocalRandom if we were to use it.
		 *
		 * Note: Because of package-protection issues, versions of some
		 * these methods also appear in some subpackage classes.
		 */

		/// <summary>
		/// Returns the probe value for the current thread without forcing
		/// initialization. Note that invoking ThreadLocalRandom.current()
		/// can be used to force initialization on zero return.
		/// </summary>
		internal static int Probe
		{
			get
			{
				return UNSAFE.getInt(Thread.CurrentThread, PROBE);
			}
		}

		/// <summary>
		/// Pseudo-randomly advances and records the given probe value for the
		/// given thread.
		/// </summary>
		internal static int AdvanceProbe(int probe)
		{
			probe ^= probe << 13; // xorshift
			probe ^= (int)((uint)probe >> 17);
			probe ^= probe << 5;
			UNSAFE.putInt(Thread.CurrentThread, PROBE, probe);
			return probe;
		}

		/// <summary>
		/// Returns the pseudo-randomly initialized or updated secondary seed.
		/// </summary>
		internal static int NextSecondarySeed()
		{
			int r;
			Thread t = Thread.CurrentThread;
			if ((r = UNSAFE.getInt(t, SECONDARY)) != 0)
			{
				r ^= r << 13; // xorshift
				r ^= (int)((uint)r >> 17);
				r ^= r << 5;
			}
			else
			{
				LocalInit();
				if ((r = (int)UNSAFE.getLong(t, SEED)) == 0)
				{
					r = 1; // avoid zero
				}
			}
			UNSAFE.putInt(t, SECONDARY, r);
			return r;
		}

		// Serialization support

		private new const long SerialVersionUID = -5851777807851030925L;

		/// <summary>
		/// @serialField rnd long
		///              seed for random computations
		/// @serialField initialized boolean
		///              always true
		/// </summary>
		private static readonly ObjectStreamField[] SerialPersistentFields = new ObjectStreamField[] {new ObjectStreamField("rnd", typeof(long)), new ObjectStreamField("initialized", typeof(bool))};

		/// <summary>
		/// Saves the {@code ThreadLocalRandom} to a stream (that is, serializes it). </summary>
		/// <param name="s"> the stream </param>
		/// <exception cref="java.io.IOException"> if an I/O error occurs </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: private void writeObject(java.io.ObjectOutputStream s) throws java.io.IOException
		private override void WriteObject(java.io.ObjectOutputStream s)
		{

			java.io.ObjectOutputStream.PutField fields = s.PutFields();
			fields.Put("rnd", UNSAFE.getLong(Thread.CurrentThread, SEED));
			fields.Put("initialized", true);
			s.WriteFields();
		}

		/// <summary>
		/// Returns the <seealso cref="#current() current"/> thread's {@code ThreadLocalRandom}. </summary>
		/// <returns> the <seealso cref="#current() current"/> thread's {@code ThreadLocalRandom} </returns>
		private Object ReadResolve()
		{
			return Current();
		}

		// Unsafe mechanics
		private static readonly sun.misc.Unsafe UNSAFE;
		private static readonly long SEED;
		private static readonly long PROBE;
		private static readonly long SECONDARY;
		static ThreadLocalRandom()
		{
			try
			{
				UNSAFE = sun.misc.Unsafe.Unsafe;
				Class tk = typeof(Thread);
				SEED = UNSAFE.objectFieldOffset(tk.GetDeclaredField("threadLocalRandomSeed"));
				PROBE = UNSAFE.objectFieldOffset(tk.GetDeclaredField("threadLocalRandomProbe"));
				SECONDARY = UNSAFE.objectFieldOffset(tk.GetDeclaredField("threadLocalRandomSecondarySeed"));
			}
			catch (Exception e)
			{
				throw new Error(e);
			}
		}
	}

}
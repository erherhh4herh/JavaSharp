using System.Collections;
using System.Collections.Generic;
using System.Collections.Concurrent;

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
namespace java.util.stream
{


	/// <summary>
	/// Implementations of <seealso cref="Collector"/> that implement various useful reduction
	/// operations, such as accumulating elements into collections, summarizing
	/// elements according to various criteria, etc.
	/// 
	/// <para>The following are examples of using the predefined collectors to perform
	/// common mutable reduction tasks:
	/// 
	/// <pre>{@code
	///     // Accumulate names into a List
	///     List<String> list = people.stream().map(Person::getName).collect(Collectors.toList());
	/// 
	///     // Accumulate names into a TreeSet
	///     Set<String> set = people.stream().map(Person::getName).collect(Collectors.toCollection(TreeSet::new));
	/// 
	///     // Convert elements to strings and concatenate them, separated by commas
	///     String joined = things.stream()
	///                           .map(Object::toString)
	///                           .collect(Collectors.joining(", "));
	/// 
	///     // Compute sum of salaries of employee
	///     int total = employees.stream()
	///                          .collect(Collectors.summingInt(Employee::getSalary)));
	/// 
	///     // Group employees by department
	///     Map<Department, List<Employee>> byDept
	///         = employees.stream()
	///                    .collect(Collectors.groupingBy(Employee::getDepartment));
	/// 
	///     // Compute sum of salaries by department
	///     Map<Department, Integer> totalByDept
	///         = employees.stream()
	///                    .collect(Collectors.groupingBy(Employee::getDepartment,
	///                                                   Collectors.summingInt(Employee::getSalary)));
	/// 
	///     // Partition students into passing and failing
	///     Map<Boolean, List<Student>> passingFailing =
	///         students.stream()
	///                 .collect(Collectors.partitioningBy(s -> s.getGrade() >= PASS_THRESHOLD));
	/// 
	/// }</pre>
	/// 
	/// @since 1.8
	/// </para>
	/// </summary>
	public sealed class Collectors
	{

		internal static readonly Set<Collector_Characteristics> CH_CONCURRENT_ID = Collections.UnmodifiableSet(EnumSet.Of(Collector_Characteristics.CONCURRENT, Collector_Characteristics.UNORDERED, Collector_Characteristics.IDENTITY_FINISH));
		internal static readonly Set<Collector_Characteristics> CH_CONCURRENT_NOID = Collections.UnmodifiableSet(EnumSet.Of(Collector_Characteristics.CONCURRENT, Collector_Characteristics.UNORDERED));
		internal static readonly Set<Collector_Characteristics> CH_ID = Collections.UnmodifiableSet(EnumSet.Of(Collector_Characteristics.IDENTITY_FINISH));
		internal static readonly Set<Collector_Characteristics> CH_UNORDERED_ID = Collections.UnmodifiableSet(EnumSet.Of(Collector_Characteristics.UNORDERED, Collector_Characteristics.IDENTITY_FINISH));
		internal static readonly Set<Collector_Characteristics> CH_NOID = Collections.EmptySet();

		private Collectors()
		{
		}

		/// <summary>
		/// Returns a merge function, suitable for use in
		/// <seealso cref="Map#merge(Object, Object, BiFunction) Map.merge()"/> or
		/// <seealso cref="#toMap(Function, Function, BinaryOperator) toMap()"/>, which always
		/// throws {@code IllegalStateException}.  This can be used to enforce the
		/// assumption that the elements being collected are distinct.
		/// </summary>
		/// @param <T> the type of input arguments to the merge function </param>
		/// <returns> a merge function which always throw {@code IllegalStateException} </returns>
		private static BinaryOperator<T> throwingMerger<T>()
		{
//JAVA TO C# CONVERTER TODO TASK: Java lambdas satisfy functional interfaces, while .NET lambdas satisfy delegates - change the appropriate interface to a delegate:
			return (u,v) =>
			{
				throw new IllegalStateException(string.Format("Duplicate key {0}", u));
			};
		}

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @SuppressWarnings("unchecked") private static <I, R> java.util.function.Function<I, R> castingIdentity()
		private static Function<I, R> castingIdentity<I, R>()
		{
//JAVA TO C# CONVERTER TODO TASK: Java lambdas satisfy functional interfaces, while .NET lambdas satisfy delegates - change the appropriate interface to a delegate:
			return i => (R) i;
		}

		/// <summary>
		/// Simple implementation class for {@code Collector}.
		/// </summary>
		/// @param <T> the type of elements to be collected </param>
		/// @param <R> the type of the result </param>
		internal class CollectorImpl<T, A, R> : Collector<T, A, R>
		{
			internal readonly Supplier<A> Supplier_Renamed;
			internal readonly BiConsumer<A, T> Accumulator_Renamed;
			internal readonly BinaryOperator<A> Combiner_Renamed;
			internal readonly Function<A, R> Finisher_Renamed;
			internal readonly Set<Collector_Characteristics> Characteristics_Renamed;

			internal CollectorImpl(Supplier<A> supplier, BiConsumer<A, T> accumulator, BinaryOperator<A> combiner, Function<A, R> finisher, Set<Collector_Characteristics> characteristics)
			{
				this.Supplier_Renamed = supplier;
				this.Accumulator_Renamed = accumulator;
				this.Combiner_Renamed = combiner;
				this.Finisher_Renamed = finisher;
				this.Characteristics_Renamed = characteristics;
			}

			internal CollectorImpl(Supplier<A> supplier, BiConsumer<A, T> accumulator, BinaryOperator<A> combiner, Set<Collector_Characteristics> characteristics) : this(supplier, accumulator, combiner, CastingIdentity(), characteristics)
			{
			}

			public override BiConsumer<A, T> Accumulator()
			{
				return Accumulator_Renamed;
			}

			public override Supplier<A> Supplier()
			{
				return Supplier_Renamed;
			}

			public override BinaryOperator<A> Combiner()
			{
				return Combiner_Renamed;
			}

			public override Function<A, R> Finisher()
			{
				return Finisher_Renamed;
			}

			public override Set<Collector_Characteristics> Characteristics()
			{
				return Characteristics_Renamed;
			}
		}

		/// <summary>
		/// Returns a {@code Collector} that accumulates the input elements into a
		/// new {@code Collection}, in encounter order.  The {@code Collection} is
		/// created by the provided factory.
		/// </summary>
		/// @param <T> the type of the input elements </param>
		/// @param <C> the type of the resulting {@code Collection} </param>
		/// <param name="collectionFactory"> a {@code Supplier} which returns a new, empty
		/// {@code Collection} of the appropriate type </param>
		/// <returns> a {@code Collector} which collects all the input elements into a
		/// {@code Collection}, in encounter order </returns>
//JAVA TO C# CONVERTER TODO TASK: Java wildcard generics are not converted to .NET:
//ORIGINAL LINE: public static <T, C extends java.util.Collection<T>> Collector<T, ?, C> toCollection(java.util.function.Supplier<C> collectionFactory)
		public static Collector<T, ?, C> toCollection<T, C>(Supplier<C> collectionFactory) where C : java.util.Collection<T>
		{
//JAVA TO C# CONVERTER TODO TASK: Java lambdas satisfy functional interfaces, while .NET lambdas satisfy delegates - change the appropriate interface to a delegate:
			return new CollectorImpl<>(collectionFactory, ICollection<T>::add, (r1, r2) =>
			{
				r1.addAll(r2);
				return r1;
			}, CH_ID);
		}

		/// <summary>
		/// Returns a {@code Collector} that accumulates the input elements into a
		/// new {@code List}. There are no guarantees on the type, mutability,
		/// serializability, or thread-safety of the {@code List} returned; if more
		/// control over the returned {@code List} is required, use <seealso cref="#toCollection(Supplier)"/>.
		/// </summary>
		/// @param <T> the type of the input elements </param>
		/// <returns> a {@code Collector} which collects all the input elements into a
		/// {@code List}, in encounter order </returns>
//JAVA TO C# CONVERTER TODO TASK: Java wildcard generics are not converted to .NET:
//ORIGINAL LINE: public static <T> Collector<T, ?, java.util.List<T>> toList()
		public static Collector<T, ?, IList<T>> toList<T>()
		{
//JAVA TO C# CONVERTER TODO TASK: Java lambdas satisfy functional interfaces, while .NET lambdas satisfy delegates - change the appropriate interface to a delegate:
			return new CollectorImpl<>((Supplier<IList<T>>) ArrayList::new, IList::add, (left, right) =>
			{
				left.addAll(right);
				return left;
			}, CH_ID);
		}

		/// <summary>
		/// Returns a {@code Collector} that accumulates the input elements into a
		/// new {@code Set}. There are no guarantees on the type, mutability,
		/// serializability, or thread-safety of the {@code Set} returned; if more
		/// control over the returned {@code Set} is required, use
		/// <seealso cref="#toCollection(Supplier)"/>.
		/// 
		/// <para>This is an <seealso cref="Collector.Characteristics#UNORDERED unordered"/>
		/// Collector.
		/// 
		/// </para>
		/// </summary>
		/// @param <T> the type of the input elements </param>
		/// <returns> a {@code Collector} which collects all the input elements into a
		/// {@code Set} </returns>
//JAVA TO C# CONVERTER TODO TASK: Java wildcard generics are not converted to .NET:
//ORIGINAL LINE: public static <T> Collector<T, ?, java.util.Set<T>> toSet()
		public static Collector<T, ?, Set<T>> toSet<T>()
		{
//JAVA TO C# CONVERTER TODO TASK: Java lambdas satisfy functional interfaces, while .NET lambdas satisfy delegates - change the appropriate interface to a delegate:
			return new CollectorImpl<>((Supplier<Set<T>>) HashSet::new, Set::add, (left, right) =>
			{
				left.addAll(right);
				return left;
			}, CH_UNORDERED_ID);
		}

		/// <summary>
		/// Returns a {@code Collector} that concatenates the input elements into a
		/// {@code String}, in encounter order.
		/// </summary>
		/// <returns> a {@code Collector} that concatenates the input elements into a
		/// {@code String}, in encounter order </returns>
//JAVA TO C# CONVERTER TODO TASK: Java wildcard generics are not converted to .NET:
//ORIGINAL LINE: public static Collector<CharSequence, ?, String> joining()
		public static Collector<CharSequence, ?, String> Joining()
		{
//JAVA TO C# CONVERTER TODO TASK: Java lambdas satisfy functional interfaces, while .NET lambdas satisfy delegates - change the appropriate interface to a delegate:
			return new CollectorImpl<CharSequence, StringBuilder, String>(StringBuilder::new, StringBuilder::append, (r1, r2) =>
			{
				r1.append(r2);
				return r1;
			}, StringBuilder::toString, CH_NOID);
		}

		/// <summary>
		/// Returns a {@code Collector} that concatenates the input elements,
		/// separated by the specified delimiter, in encounter order.
		/// </summary>
		/// <param name="delimiter"> the delimiter to be used between each element </param>
		/// <returns> A {@code Collector} which concatenates CharSequence elements,
		/// separated by the specified delimiter, in encounter order </returns>
//JAVA TO C# CONVERTER TODO TASK: Java wildcard generics are not converted to .NET:
//ORIGINAL LINE: public static Collector<CharSequence, ?, String> joining(CharSequence delimiter)
		public static Collector<CharSequence, ?, String> Joining(CharSequence delimiter)
		{
			return Joining(delimiter, "", "");
		}

		/// <summary>
		/// Returns a {@code Collector} that concatenates the input elements,
		/// separated by the specified delimiter, with the specified prefix and
		/// suffix, in encounter order.
		/// </summary>
		/// <param name="delimiter"> the delimiter to be used between each element </param>
		/// <param name="prefix"> the sequence of characters to be used at the beginning
		///                of the joined result </param>
		/// <param name="suffix"> the sequence of characters to be used at the end
		///                of the joined result </param>
		/// <returns> A {@code Collector} which concatenates CharSequence elements,
		/// separated by the specified delimiter, in encounter order </returns>
//JAVA TO C# CONVERTER TODO TASK: Java wildcard generics are not converted to .NET:
//ORIGINAL LINE: public static Collector<CharSequence, ?, String> joining(CharSequence delimiter, CharSequence prefix, CharSequence suffix)
		public static Collector<CharSequence, ?, String> Joining(CharSequence delimiter, CharSequence prefix, CharSequence suffix)
		{
//JAVA TO C# CONVERTER TODO TASK: Java lambdas satisfy functional interfaces, while .NET lambdas satisfy delegates - change the appropriate interface to a delegate:
			return new CollectorImpl<>(() => new StringJoiner(delimiter, prefix, suffix), StringJoiner::add, StringJoiner::merge, StringJoiner::toString, CH_NOID);
		}

		/// <summary>
		/// {@code BinaryOperator<Map>} that merges the contents of its right
		/// argument into its left argument, using the provided merge function to
		/// handle duplicate keys.
		/// </summary>
		/// @param <K> type of the map keys </param>
		/// @param <V> type of the map values </param>
		/// @param <M> type of the map </param>
		/// <param name="mergeFunction"> A merge function suitable for
		/// <seealso cref="Map#merge(Object, Object, BiFunction) Map.merge()"/> </param>
		/// <returns> a merge function for two maps </returns>
		private static BinaryOperator<M> mapMerger<K, V, M>(BinaryOperator<V> mergeFunction) where M : java.util.Map<K,V>
		{
//JAVA TO C# CONVERTER TODO TASK: Java lambdas satisfy functional interfaces, while .NET lambdas satisfy delegates - change the appropriate interface to a delegate:
			return (m1, m2) =>
			{
				foreach (java.util.Map_Entry<K, V> e in m2.entrySet())
				{
					m1.merge(e.Key, e.Value, mergeFunction);
				}
				return m1;
			};
		}

		/// <summary>
		/// Adapts a {@code Collector} accepting elements of type {@code U} to one
		/// accepting elements of type {@code T} by applying a mapping function to
		/// each input element before accumulation.
		/// 
		/// @apiNote
		/// The {@code mapping()} collectors are most useful when used in a
		/// multi-level reduction, such as downstream of a {@code groupingBy} or
		/// {@code partitioningBy}.  For example, given a stream of
		/// {@code Person}, to accumulate the set of last names in each city:
		/// <pre>{@code
		///     Map<City, Set<String>> lastNamesByCity
		///         = people.stream().collect(groupingBy(Person::getCity,
		///                                              mapping(Person::getLastName, toSet())));
		/// }</pre>
		/// </summary>
		/// @param <T> the type of the input elements </param>
		/// @param <U> type of elements accepted by downstream collector </param>
		/// @param <A> intermediate accumulation type of the downstream collector </param>
		/// @param <R> result type of collector </param>
		/// <param name="mapper"> a function to be applied to the input elements </param>
		/// <param name="downstream"> a collector which will accept mapped values </param>
		/// <returns> a collector which applies the mapping function to the input
		/// elements and provides the mapped results to the downstream collector </returns>
//JAVA TO C# CONVERTER TODO TASK: There is no .NET equivalent to the Java 'super' constraint:
//ORIGINAL LINE: public static <T, U, A, R> Collector<T, ?, R> mapping(java.util.function.Function<? base T, ? extends U> mapper, Collector<? base U, A, R> downstream)
//JAVA TO C# CONVERTER TODO TASK: Java wildcard generics are not converted to .NET:
		public static Collector<T, ?, R> mapping<T, U, A, R, T1, T2>(Function<T1> mapper, Collector<T2> downstream) where T1 : U
		{
//JAVA TO C# CONVERTER TODO TASK: There is no .NET equivalent to the Java 'super' constraint:
//ORIGINAL LINE: java.util.function.BiConsumer<A, ? base U> downstreamAccumulator = downstream.accumulator();
//JAVA TO C# CONVERTER TODO TASK: Java wildcard generics are not converted to .NET:
			BiConsumer<A, ?> downstreamAccumulator = downstream.Accumulator();
//JAVA TO C# CONVERTER TODO TASK: Java lambdas satisfy functional interfaces, while .NET lambdas satisfy delegates - change the appropriate interface to a delegate:
			return new CollectorImpl<>(downstream.Supplier(), (r, t) => downstreamAccumulator.accept(r, mapper.Apply(t)), downstream.Combiner(), downstream.Finisher(), downstream.Characteristics());
		}

		/// <summary>
		/// Adapts a {@code Collector} to perform an additional finishing
		/// transformation.  For example, one could adapt the <seealso cref="#toList()"/>
		/// collector to always produce an immutable list with:
		/// <pre>{@code
		///     List<String> people
		///         = people.stream().collect(collectingAndThen(toList(), Collections::unmodifiableList));
		/// }</pre>
		/// </summary>
		/// @param <T> the type of the input elements </param>
		/// @param <A> intermediate accumulation type of the downstream collector </param>
		/// @param <R> result type of the downstream collector </param>
		/// @param <RR> result type of the resulting collector </param>
		/// <param name="downstream"> a collector </param>
		/// <param name="finisher"> a function to be applied to the final result of the downstream collector </param>
		/// <returns> a collector which performs the action of the downstream collector,
		/// followed by an additional finishing step </returns>
		public static Collector<T, A, RR> collectingAndThen<T, A, R, RR>(Collector<T, A, R> downstream, Function<R, RR> finisher)
		{
			Set<Collector_Characteristics> characteristics = downstream.Characteristics();
			if (characteristics.Contains(Collector_Characteristics.IDENTITY_FINISH))
			{
				if (characteristics.Count == 1)
				{
					characteristics = Collectors.CH_NOID;
				}
				else
				{
					characteristics = EnumSet.CopyOf(characteristics);
					characteristics.Remove(Collector_Characteristics.IDENTITY_FINISH);
					characteristics = Collections.UnmodifiableSet(characteristics);
				}
			}
			return new CollectorImpl<>(downstream.Supplier(), downstream.Accumulator(), downstream.Combiner(), downstream.Finisher().andThen(finisher), characteristics);
		}

		/// <summary>
		/// Returns a {@code Collector} accepting elements of type {@code T} that
		/// counts the number of input elements.  If no elements are present, the
		/// result is 0.
		/// 
		/// @implSpec
		/// This produces a result equivalent to:
		/// <pre>{@code
		///     reducing(0L, e -> 1L, Long::sum)
		/// }</pre>
		/// </summary>
		/// @param <T> the type of the input elements </param>
		/// <returns> a {@code Collector} that counts the input elements </returns>
//JAVA TO C# CONVERTER TODO TASK: Java wildcard generics are not converted to .NET:
//ORIGINAL LINE: public static <T> Collector<T, ?, Long> counting()
		public static Collector<T, ?, Long> counting<T>()
		{
//JAVA TO C# CONVERTER TODO TASK: Java lambdas satisfy functional interfaces, while .NET lambdas satisfy delegates - change the appropriate interface to a delegate:
			return Reducing(0L, e => 1L, Long::sum);
		}

		/// <summary>
		/// Returns a {@code Collector} that produces the minimal element according
		/// to a given {@code Comparator}, described as an {@code Optional<T>}.
		/// 
		/// @implSpec
		/// This produces a result equivalent to:
		/// <pre>{@code
		///     reducing(BinaryOperator.minBy(comparator))
		/// }</pre>
		/// </summary>
		/// @param <T> the type of the input elements </param>
		/// <param name="comparator"> a {@code Comparator} for comparing elements </param>
		/// <returns> a {@code Collector} that produces the minimal value </returns>
//JAVA TO C# CONVERTER TODO TASK: There is no .NET equivalent to the Java 'super' constraint:
//ORIGINAL LINE: public static <T> Collector<T, ?, java.util.Optional<T>> minBy(java.util.Comparator<? base T> comparator)
//JAVA TO C# CONVERTER TODO TASK: Java wildcard generics are not converted to .NET:
		public static Collector<T, ?, Optional<T>> minBy<T, T1>(IComparer<T1> comparator)
		{
			return Reducing(BinaryOperator.minBy(comparator));
		}

		/// <summary>
		/// Returns a {@code Collector} that produces the maximal element according
		/// to a given {@code Comparator}, described as an {@code Optional<T>}.
		/// 
		/// @implSpec
		/// This produces a result equivalent to:
		/// <pre>{@code
		///     reducing(BinaryOperator.maxBy(comparator))
		/// }</pre>
		/// </summary>
		/// @param <T> the type of the input elements </param>
		/// <param name="comparator"> a {@code Comparator} for comparing elements </param>
		/// <returns> a {@code Collector} that produces the maximal value </returns>
//JAVA TO C# CONVERTER TODO TASK: There is no .NET equivalent to the Java 'super' constraint:
//ORIGINAL LINE: public static <T> Collector<T, ?, java.util.Optional<T>> maxBy(java.util.Comparator<? base T> comparator)
//JAVA TO C# CONVERTER TODO TASK: Java wildcard generics are not converted to .NET:
		public static Collector<T, ?, Optional<T>> maxBy<T, T1>(IComparer<T1> comparator)
		{
			return Reducing(BinaryOperator.maxBy(comparator));
		}

		/// <summary>
		/// Returns a {@code Collector} that produces the sum of a integer-valued
		/// function applied to the input elements.  If no elements are present,
		/// the result is 0.
		/// </summary>
		/// @param <T> the type of the input elements </param>
		/// <param name="mapper"> a function extracting the property to be summed </param>
		/// <returns> a {@code Collector} that produces the sum of a derived property </returns>
//JAVA TO C# CONVERTER TODO TASK: There is no .NET equivalent to the Java 'super' constraint:
//ORIGINAL LINE: public static <T> Collector<T, ?, Integer> summingInt(java.util.function.ToIntFunction<? base T> mapper)
//JAVA TO C# CONVERTER TODO TASK: Java wildcard generics are not converted to .NET:
		public static Collector<T, ?, Integer> summingInt<T, T1>(ToIntFunction<T1> mapper)
		{
//JAVA TO C# CONVERTER TODO TASK: Java lambdas satisfy functional interfaces, while .NET lambdas satisfy delegates - change the appropriate interface to a delegate:
			return new CollectorImpl<>(() => new int[1], (a, t) => {a[0] += mapper.ApplyAsInt(t);}, (a, b) => {a[0] += b[0]; return a;}, a => a[0], CH_NOID);
		}

		/// <summary>
		/// Returns a {@code Collector} that produces the sum of a long-valued
		/// function applied to the input elements.  If no elements are present,
		/// the result is 0.
		/// </summary>
		/// @param <T> the type of the input elements </param>
		/// <param name="mapper"> a function extracting the property to be summed </param>
		/// <returns> a {@code Collector} that produces the sum of a derived property </returns>
//JAVA TO C# CONVERTER TODO TASK: There is no .NET equivalent to the Java 'super' constraint:
//ORIGINAL LINE: public static <T> Collector<T, ?, Long> summingLong(java.util.function.ToLongFunction<? base T> mapper)
//JAVA TO C# CONVERTER TODO TASK: Java wildcard generics are not converted to .NET:
		public static Collector<T, ?, Long> summingLong<T, T1>(ToLongFunction<T1> mapper)
		{
//JAVA TO C# CONVERTER TODO TASK: Java lambdas satisfy functional interfaces, while .NET lambdas satisfy delegates - change the appropriate interface to a delegate:
			return new CollectorImpl<>(() => new long[1], (a, t) => {a[0] += mapper.ApplyAsLong(t);}, (a, b) => {a[0] += b[0]; return a;}, a => a[0], CH_NOID);
		}

		/// <summary>
		/// Returns a {@code Collector} that produces the sum of a double-valued
		/// function applied to the input elements.  If no elements are present,
		/// the result is 0.
		/// 
		/// <para>The sum returned can vary depending upon the order in which
		/// values are recorded, due to accumulated rounding error in
		/// addition of values of differing magnitudes. Values sorted by increasing
		/// absolute magnitude tend to yield more accurate results.  If any recorded
		/// value is a {@code NaN} or the sum is at any point a {@code NaN} then the
		/// sum will be {@code NaN}.
		/// 
		/// </para>
		/// </summary>
		/// @param <T> the type of the input elements </param>
		/// <param name="mapper"> a function extracting the property to be summed </param>
		/// <returns> a {@code Collector} that produces the sum of a derived property </returns>
//JAVA TO C# CONVERTER TODO TASK: There is no .NET equivalent to the Java 'super' constraint:
//ORIGINAL LINE: public static <T> Collector<T, ?, Double> summingDouble(java.util.function.ToDoubleFunction<? base T> mapper)
//JAVA TO C# CONVERTER TODO TASK: Java wildcard generics are not converted to .NET:
		public static Collector<T, ?, Double> summingDouble<T, T1>(ToDoubleFunction<T1> mapper)
		{
			/*
			 * In the arrays allocated for the collect operation, index 0
			 * holds the high-order bits of the running sum, index 1 holds
			 * the low-order bits of the sum computed via compensated
			 * summation, and index 2 holds the simple sum used to compute
			 * the proper result if the stream contains infinite values of
			 * the same sign.
			 */
//JAVA TO C# CONVERTER TODO TASK: Java lambdas satisfy functional interfaces, while .NET lambdas satisfy delegates - change the appropriate interface to a delegate:
			return new CollectorImpl<>(() => new double[3], (a, t) => {SumWithCompensation(a, mapper.ApplyAsDouble(t)); a[2] += mapper.ApplyAsDouble(t);}, (a, b) => {SumWithCompensation(a, b[0]); a[2] += b[2]; return SumWithCompensation(a, b[1]);}, a => ComputeFinalSum(a), CH_NOID);
		}

		/// <summary>
		/// Incorporate a new double value using Kahan summation /
		/// compensation summation.
		/// 
		/// High-order bits of the sum are in intermediateSum[0], low-order
		/// bits of the sum are in intermediateSum[1], any additional
		/// elements are application-specific.
		/// </summary>
		/// <param name="intermediateSum"> the high-order and low-order words of the intermediate sum </param>
		/// <param name="value"> the name value to be included in the running sum </param>
		internal static double[] SumWithCompensation(double[] intermediateSum, double value)
		{
			double tmp = value - intermediateSum[1];
			double sum = intermediateSum[0];
			double velvel = sum + tmp; // Little wolf of rounding error
			intermediateSum[1] = (velvel - sum) - tmp;
			intermediateSum[0] = velvel;
			return intermediateSum;
		}

		/// <summary>
		/// If the compensated sum is spuriously NaN from accumulating one
		/// or more same-signed infinite values, return the
		/// correctly-signed infinity stored in the simple sum.
		/// </summary>
		internal static double ComputeFinalSum(double[] summands)
		{
			// Better error bounds to add both terms as the final sum
			double tmp = summands[0] + summands[1];
			double simpleSum = summands[summands.Length - 1];
			if (Double.IsNaN(tmp) && Double.IsInfinity(simpleSum))
			{
				return simpleSum;
			}
			else
			{
				return tmp;
			}
		}

		/// <summary>
		/// Returns a {@code Collector} that produces the arithmetic mean of an integer-valued
		/// function applied to the input elements.  If no elements are present,
		/// the result is 0.
		/// </summary>
		/// @param <T> the type of the input elements </param>
		/// <param name="mapper"> a function extracting the property to be summed </param>
		/// <returns> a {@code Collector} that produces the sum of a derived property </returns>
//JAVA TO C# CONVERTER TODO TASK: There is no .NET equivalent to the Java 'super' constraint:
//ORIGINAL LINE: public static <T> Collector<T, ?, Double> averagingInt(java.util.function.ToIntFunction<? base T> mapper)
//JAVA TO C# CONVERTER TODO TASK: Java wildcard generics are not converted to .NET:
		public static Collector<T, ?, Double> averagingInt<T, T1>(ToIntFunction<T1> mapper)
		{
//JAVA TO C# CONVERTER TODO TASK: Java lambdas satisfy functional interfaces, while .NET lambdas satisfy delegates - change the appropriate interface to a delegate:
			return new CollectorImpl<>(() => new long[2], (a, t) => {a[0] += mapper.ApplyAsInt(t); a[1]++;}, (a, b) => {a[0] += b[0]; a[1] += b[1]; return a;}, a => (a[1] == 0) ? 0.0d : (double) a[0] / a[1], CH_NOID);
		}

		/// <summary>
		/// Returns a {@code Collector} that produces the arithmetic mean of a long-valued
		/// function applied to the input elements.  If no elements are present,
		/// the result is 0.
		/// </summary>
		/// @param <T> the type of the input elements </param>
		/// <param name="mapper"> a function extracting the property to be summed </param>
		/// <returns> a {@code Collector} that produces the sum of a derived property </returns>
//JAVA TO C# CONVERTER TODO TASK: There is no .NET equivalent to the Java 'super' constraint:
//ORIGINAL LINE: public static <T> Collector<T, ?, Double> averagingLong(java.util.function.ToLongFunction<? base T> mapper)
//JAVA TO C# CONVERTER TODO TASK: Java wildcard generics are not converted to .NET:
		public static Collector<T, ?, Double> averagingLong<T, T1>(ToLongFunction<T1> mapper)
		{
//JAVA TO C# CONVERTER TODO TASK: Java lambdas satisfy functional interfaces, while .NET lambdas satisfy delegates - change the appropriate interface to a delegate:
			return new CollectorImpl<>(() => new long[2], (a, t) => {a[0] += mapper.ApplyAsLong(t); a[1]++;}, (a, b) => {a[0] += b[0]; a[1] += b[1]; return a;}, a => (a[1] == 0) ? 0.0d : (double) a[0] / a[1], CH_NOID);
		}

		/// <summary>
		/// Returns a {@code Collector} that produces the arithmetic mean of a double-valued
		/// function applied to the input elements.  If no elements are present,
		/// the result is 0.
		/// 
		/// <para>The average returned can vary depending upon the order in which
		/// values are recorded, due to accumulated rounding error in
		/// addition of values of differing magnitudes. Values sorted by increasing
		/// absolute magnitude tend to yield more accurate results.  If any recorded
		/// value is a {@code NaN} or the sum is at any point a {@code NaN} then the
		/// average will be {@code NaN}.
		/// 
		/// @implNote The {@code double} format can represent all
		/// consecutive integers in the range -2<sup>53</sup> to
		/// 2<sup>53</sup>. If the pipeline has more than 2<sup>53</sup>
		/// values, the divisor in the average computation will saturate at
		/// 2<sup>53</sup>, leading to additional numerical errors.
		/// 
		/// </para>
		/// </summary>
		/// @param <T> the type of the input elements </param>
		/// <param name="mapper"> a function extracting the property to be summed </param>
		/// <returns> a {@code Collector} that produces the sum of a derived property </returns>
//JAVA TO C# CONVERTER TODO TASK: There is no .NET equivalent to the Java 'super' constraint:
//ORIGINAL LINE: public static <T> Collector<T, ?, Double> averagingDouble(java.util.function.ToDoubleFunction<? base T> mapper)
//JAVA TO C# CONVERTER TODO TASK: Java wildcard generics are not converted to .NET:
		public static Collector<T, ?, Double> averagingDouble<T, T1>(ToDoubleFunction<T1> mapper)
		{
			/*
			 * In the arrays allocated for the collect operation, index 0
			 * holds the high-order bits of the running sum, index 1 holds
			 * the low-order bits of the sum computed via compensated
			 * summation, and index 2 holds the number of values seen.
			 */
//JAVA TO C# CONVERTER TODO TASK: Java lambdas satisfy functional interfaces, while .NET lambdas satisfy delegates - change the appropriate interface to a delegate:
			return new CollectorImpl<>(() => new double[4], (a, t) => {SumWithCompensation(a, mapper.ApplyAsDouble(t)); a[2]++; a[3] += mapper.ApplyAsDouble(t);}, (a, b) => {SumWithCompensation(a, b[0]); SumWithCompensation(a, b[1]); a[2] += b[2]; a[3] += b[3]; return a;}, a => (a[2] == 0) ? 0.0d : (ComputeFinalSum(a) / a[2]), CH_NOID);
		}

		/// <summary>
		/// Returns a {@code Collector} which performs a reduction of its
		/// input elements under a specified {@code BinaryOperator} using the
		/// provided identity.
		/// 
		/// @apiNote
		/// The {@code reducing()} collectors are most useful when used in a
		/// multi-level reduction, downstream of {@code groupingBy} or
		/// {@code partitioningBy}.  To perform a simple reduction on a stream,
		/// use <seealso cref="Stream#reduce(Object, BinaryOperator)"/>} instead.
		/// </summary>
		/// @param <T> element type for the input and output of the reduction </param>
		/// <param name="identity"> the identity value for the reduction (also, the value
		///                 that is returned when there are no input elements) </param>
		/// <param name="op"> a {@code BinaryOperator<T>} used to reduce the input elements </param>
		/// <returns> a {@code Collector} which implements the reduction operation
		/// </returns>
		/// <seealso cref= #reducing(BinaryOperator) </seealso>
		/// <seealso cref= #reducing(Object, Function, BinaryOperator) </seealso>
//JAVA TO C# CONVERTER TODO TASK: Java wildcard generics are not converted to .NET:
//ORIGINAL LINE: public static <T> Collector<T, ?, T> reducing(T identity, java.util.function.BinaryOperator<T> op)
		public static Collector<T, ?, T> reducing<T>(T identity, BinaryOperator<T> op)
		{
//JAVA TO C# CONVERTER TODO TASK: Java lambdas satisfy functional interfaces, while .NET lambdas satisfy delegates - change the appropriate interface to a delegate:
			return new CollectorImpl<>(BoxSupplier(identity), (a, t) =>
			{
				a[0] = op.Apply(a[0], t);
//JAVA TO C# CONVERTER TODO TASK: Java lambdas satisfy functional interfaces, while .NET lambdas satisfy delegates - change the appropriate interface to a delegate:
			}, (a, b) => {a[0] = op.apply(a[0], b[0]); return a;}, a => a[0], CH_NOID);
		}

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @SuppressWarnings("unchecked") private static <T> java.util.function.Supplier<T[]> boxSupplier(T identity)
		private static Supplier<T[]> boxSupplier<T>(T identity)
		{
//JAVA TO C# CONVERTER TODO TASK: Java lambdas satisfy functional interfaces, while .NET lambdas satisfy delegates - change the appropriate interface to a delegate:
			return () => (T[]) new Object[] {identity};
		}

		/// <summary>
		/// Returns a {@code Collector} which performs a reduction of its
		/// input elements under a specified {@code BinaryOperator}.  The result
		/// is described as an {@code Optional<T>}.
		/// 
		/// @apiNote
		/// The {@code reducing()} collectors are most useful when used in a
		/// multi-level reduction, downstream of {@code groupingBy} or
		/// {@code partitioningBy}.  To perform a simple reduction on a stream,
		/// use <seealso cref="Stream#reduce(BinaryOperator)"/> instead.
		/// 
		/// <para>For example, given a stream of {@code Person}, to calculate tallest
		/// person in each city:
		/// <pre>{@code
		///     Comparator<Person> byHeight = Comparator.comparing(Person::getHeight);
		///     Map<City, Person> tallestByCity
		///         = people.stream().collect(groupingBy(Person::getCity, reducing(BinaryOperator.maxBy(byHeight))));
		/// }</pre>
		/// 
		/// </para>
		/// </summary>
		/// @param <T> element type for the input and output of the reduction </param>
		/// <param name="op"> a {@code BinaryOperator<T>} used to reduce the input elements </param>
		/// <returns> a {@code Collector} which implements the reduction operation
		/// </returns>
		/// <seealso cref= #reducing(Object, BinaryOperator) </seealso>
		/// <seealso cref= #reducing(Object, Function, BinaryOperator) </seealso>
//JAVA TO C# CONVERTER TODO TASK: Java wildcard generics are not converted to .NET:
//ORIGINAL LINE: public static <T> Collector<T, ?, java.util.Optional<T>> reducing(java.util.function.BinaryOperator<T> op)
		public static Collector<T, ?, Optional<T>> reducing<T>(BinaryOperator<T> op)
		{
//JAVA TO C# CONVERTER TODO TASK: Local classes are not converted by Java to C# Converter:
//			class OptionalBox implements java.util.function.Consumer<T>
	//		{
	//			T value = null;
	//			boolean present = false;
	//
	//			@@Override public void accept(T t)
	//			{
	//				if (present)
	//				{
	//					value = op.apply(value, t);
	//				}
	//				else
	//				{
	//					value = t;
	//					present = true;
	//				}
	//			}
	//		}

//JAVA TO C# CONVERTER TODO TASK: Java lambdas satisfy functional interfaces, while .NET lambdas satisfy delegates - change the appropriate interface to a delegate:
			return new CollectorImpl<T, OptionalBox, Optional<T>>(OptionalBox::new, OptionalBox::accept, (a, b) =>
			{
				if (b.present)
				{
					a.accept(b.value);
				}
				return a;
//JAVA TO C# CONVERTER TODO TASK: Java lambdas satisfy functional interfaces, while .NET lambdas satisfy delegates - change the appropriate interface to a delegate:
			}, a => java.util.Optional.ofNullable(a.value), CH_NOID);
		}

		/// <summary>
		/// Returns a {@code Collector} which performs a reduction of its
		/// input elements under a specified mapping function and
		/// {@code BinaryOperator}. This is a generalization of
		/// <seealso cref="#reducing(Object, BinaryOperator)"/> which allows a transformation
		/// of the elements before reduction.
		/// 
		/// @apiNote
		/// The {@code reducing()} collectors are most useful when used in a
		/// multi-level reduction, downstream of {@code groupingBy} or
		/// {@code partitioningBy}.  To perform a simple map-reduce on a stream,
		/// use <seealso cref="Stream#map(Function)"/> and <seealso cref="Stream#reduce(Object, BinaryOperator)"/>
		/// instead.
		/// 
		/// <para>For example, given a stream of {@code Person}, to calculate the longest
		/// last name of residents in each city:
		/// <pre>{@code
		///     Comparator<String> byLength = Comparator.comparing(String::length);
		///     Map<City, String> longestLastNameByCity
		///         = people.stream().collect(groupingBy(Person::getCity,
		///                                              reducing(Person::getLastName, BinaryOperator.maxBy(byLength))));
		/// }</pre>
		/// 
		/// </para>
		/// </summary>
		/// @param <T> the type of the input elements </param>
		/// @param <U> the type of the mapped values </param>
		/// <param name="identity"> the identity value for the reduction (also, the value
		///                 that is returned when there are no input elements) </param>
		/// <param name="mapper"> a mapping function to apply to each input value </param>
		/// <param name="op"> a {@code BinaryOperator<U>} used to reduce the mapped values </param>
		/// <returns> a {@code Collector} implementing the map-reduce operation
		/// </returns>
		/// <seealso cref= #reducing(Object, BinaryOperator) </seealso>
		/// <seealso cref= #reducing(BinaryOperator) </seealso>
//JAVA TO C# CONVERTER TODO TASK: There is no .NET equivalent to the Java 'super' constraint:
//ORIGINAL LINE: public static <T, U> Collector<T, ?, U> reducing(U identity, java.util.function.Function<? base T, ? extends U> mapper, java.util.function.BinaryOperator<U> op)
//JAVA TO C# CONVERTER TODO TASK: Java wildcard generics are not converted to .NET:
		public static Collector<T, ?, U> reducing<T, U, T1>(U identity, Function<T1> mapper, BinaryOperator<U> op) where T1 : U
		{
//JAVA TO C# CONVERTER TODO TASK: Java lambdas satisfy functional interfaces, while .NET lambdas satisfy delegates - change the appropriate interface to a delegate:
			return new CollectorImpl<>(BoxSupplier(identity), (a, t) =>
			{
				a[0] = op.Apply(a[0], mapper.Apply(t));
//JAVA TO C# CONVERTER TODO TASK: Java lambdas satisfy functional interfaces, while .NET lambdas satisfy delegates - change the appropriate interface to a delegate:
			}, (a, b) => {a[0] = op.apply(a[0], b[0]); return a;}, a => a[0], CH_NOID);
		}

		/// <summary>
		/// Returns a {@code Collector} implementing a "group by" operation on
		/// input elements of type {@code T}, grouping elements according to a
		/// classification function, and returning the results in a {@code Map}.
		/// 
		/// <para>The classification function maps elements to some key type {@code K}.
		/// The collector produces a {@code Map<K, List<T>>} whose keys are the
		/// values resulting from applying the classification function to the input
		/// elements, and whose corresponding values are {@code List}s containing the
		/// input elements which map to the associated key under the classification
		/// function.
		/// 
		/// </para>
		/// <para>There are no guarantees on the type, mutability, serializability, or
		/// thread-safety of the {@code Map} or {@code List} objects returned.
		/// @implSpec
		/// This produces a result similar to:
		/// <pre>{@code
		///     groupingBy(classifier, toList());
		/// }</pre>
		/// 
		/// @implNote
		/// The returned {@code Collector} is not concurrent.  For parallel stream
		/// pipelines, the {@code combiner} function operates by merging the keys
		/// from one map into another, which can be an expensive operation.  If
		/// preservation of the order in which elements appear in the resulting {@code Map}
		/// collector is not required, using <seealso cref="#groupingByConcurrent(Function)"/>
		/// may offer better parallel performance.
		/// 
		/// </para>
		/// </summary>
		/// @param <T> the type of the input elements </param>
		/// @param <K> the type of the keys </param>
		/// <param name="classifier"> the classifier function mapping input elements to keys </param>
		/// <returns> a {@code Collector} implementing the group-by operation
		/// </returns>
		/// <seealso cref= #groupingBy(Function, Collector) </seealso>
		/// <seealso cref= #groupingBy(Function, Supplier, Collector) </seealso>
		/// <seealso cref= #groupingByConcurrent(Function) </seealso>
//JAVA TO C# CONVERTER TODO TASK: There is no .NET equivalent to the Java 'super' constraint:
//ORIGINAL LINE: public static <T, K> Collector<T, ?, java.util.Map<K, java.util.List<T>>> groupingBy(java.util.function.Function<? base T, ? extends K> classifier)
//JAVA TO C# CONVERTER TODO TASK: Java wildcard generics are not converted to .NET:
		public static Collector<T, ?, IDictionary<K, IList<T>>> groupingBy<T, K, T1>(Function<T1> classifier) where T1 : K
		{
			return GroupingBy(classifier, ToList());
		}

		/// <summary>
		/// Returns a {@code Collector} implementing a cascaded "group by" operation
		/// on input elements of type {@code T}, grouping elements according to a
		/// classification function, and then performing a reduction operation on
		/// the values associated with a given key using the specified downstream
		/// {@code Collector}.
		/// 
		/// <para>The classification function maps elements to some key type {@code K}.
		/// The downstream collector operates on elements of type {@code T} and
		/// produces a result of type {@code D}. The resulting collector produces a
		/// {@code Map<K, D>}.
		/// 
		/// </para>
		/// <para>There are no guarantees on the type, mutability,
		/// serializability, or thread-safety of the {@code Map} returned.
		/// 
		/// </para>
		/// <para>For example, to compute the set of last names of people in each city:
		/// <pre>{@code
		///     Map<City, Set<String>> namesByCity
		///         = people.stream().collect(groupingBy(Person::getCity,
		///                                              mapping(Person::getLastName, toSet())));
		/// }</pre>
		/// 
		/// @implNote
		/// The returned {@code Collector} is not concurrent.  For parallel stream
		/// pipelines, the {@code combiner} function operates by merging the keys
		/// from one map into another, which can be an expensive operation.  If
		/// preservation of the order in which elements are presented to the downstream
		/// collector is not required, using <seealso cref="#groupingByConcurrent(Function, Collector)"/>
		/// may offer better parallel performance.
		/// 
		/// </para>
		/// </summary>
		/// @param <T> the type of the input elements </param>
		/// @param <K> the type of the keys </param>
		/// @param <A> the intermediate accumulation type of the downstream collector </param>
		/// @param <D> the result type of the downstream reduction </param>
		/// <param name="classifier"> a classifier function mapping input elements to keys </param>
		/// <param name="downstream"> a {@code Collector} implementing the downstream reduction </param>
		/// <returns> a {@code Collector} implementing the cascaded group-by operation </returns>
		/// <seealso cref= #groupingBy(Function)
		/// </seealso>
		/// <seealso cref= #groupingBy(Function, Supplier, Collector) </seealso>
		/// <seealso cref= #groupingByConcurrent(Function, Collector) </seealso>
//JAVA TO C# CONVERTER TODO TASK: There is no .NET equivalent to the Java 'super' constraint:
//ORIGINAL LINE: public static <T, K, A, D> Collector<T, ?, java.util.Map<K, D>> groupingBy(java.util.function.Function<? base T, ? extends K> classifier, Collector<? base T, A, D> downstream)
//JAVA TO C# CONVERTER TODO TASK: Java wildcard generics are not converted to .NET:
		public static Collector<T, ?, IDictionary<K, D>> groupingBy<T, K, A, D, T1, T2>(Function<T1> classifier, Collector<T2> downstream) where T1 : K
		{
			return GroupingBy(classifier, Hashtable::new, downstream);
		}

		/// <summary>
		/// Returns a {@code Collector} implementing a cascaded "group by" operation
		/// on input elements of type {@code T}, grouping elements according to a
		/// classification function, and then performing a reduction operation on
		/// the values associated with a given key using the specified downstream
		/// {@code Collector}.  The {@code Map} produced by the Collector is created
		/// with the supplied factory function.
		/// 
		/// <para>The classification function maps elements to some key type {@code K}.
		/// The downstream collector operates on elements of type {@code T} and
		/// produces a result of type {@code D}. The resulting collector produces a
		/// {@code Map<K, D>}.
		/// 
		/// </para>
		/// <para>For example, to compute the set of last names of people in each city,
		/// where the city names are sorted:
		/// <pre>{@code
		///     Map<City, Set<String>> namesByCity
		///         = people.stream().collect(groupingBy(Person::getCity, TreeMap::new,
		///                                              mapping(Person::getLastName, toSet())));
		/// }</pre>
		/// 
		/// @implNote
		/// The returned {@code Collector} is not concurrent.  For parallel stream
		/// pipelines, the {@code combiner} function operates by merging the keys
		/// from one map into another, which can be an expensive operation.  If
		/// preservation of the order in which elements are presented to the downstream
		/// collector is not required, using <seealso cref="#groupingByConcurrent(Function, Supplier, Collector)"/>
		/// may offer better parallel performance.
		/// 
		/// </para>
		/// </summary>
		/// @param <T> the type of the input elements </param>
		/// @param <K> the type of the keys </param>
		/// @param <A> the intermediate accumulation type of the downstream collector </param>
		/// @param <D> the result type of the downstream reduction </param>
		/// @param <M> the type of the resulting {@code Map} </param>
		/// <param name="classifier"> a classifier function mapping input elements to keys </param>
		/// <param name="downstream"> a {@code Collector} implementing the downstream reduction </param>
		/// <param name="mapFactory"> a function which, when called, produces a new empty
		///                   {@code Map} of the desired type </param>
		/// <returns> a {@code Collector} implementing the cascaded group-by operation
		/// </returns>
		/// <seealso cref= #groupingBy(Function, Collector) </seealso>
		/// <seealso cref= #groupingBy(Function) </seealso>
		/// <seealso cref= #groupingByConcurrent(Function, Supplier, Collector) </seealso>
//JAVA TO C# CONVERTER TODO TASK: There is no .NET equivalent to the Java 'super' constraint:
//ORIGINAL LINE: public static <T, K, D, A, M extends java.util.Map<K, D>> Collector<T, ?, M> groupingBy(java.util.function.Function<? base T, ? extends K> classifier, java.util.function.Supplier<M> mapFactory, Collector<? base T, A, D> downstream)
//JAVA TO C# CONVERTER TODO TASK: Java wildcard generics are not converted to .NET:
		public static Collector<T, ?, M> groupingBy<T, K, D, A, M, T1, T2>(Function<T1> classifier, Supplier<M> mapFactory, Collector<T2> downstream) where M : java.util.Map<K, D> where T1 : K
		{
			Supplier<A> downstreamSupplier = downstream.Supplier();
//JAVA TO C# CONVERTER TODO TASK: There is no .NET equivalent to the Java 'super' constraint:
//ORIGINAL LINE: java.util.function.BiConsumer<A, ? base T> downstreamAccumulator = downstream.accumulator();
//JAVA TO C# CONVERTER TODO TASK: Java wildcard generics are not converted to .NET:
			BiConsumer<A, ?> downstreamAccumulator = downstream.Accumulator();
//JAVA TO C# CONVERTER TODO TASK: Java lambdas satisfy functional interfaces, while .NET lambdas satisfy delegates - change the appropriate interface to a delegate:
			BiConsumer<IDictionary<K, A>, T> accumulator = (m, t) =>
			{
				K key = Objects.RequireNonNull(classifier.Apply(t), "element cannot be mapped to a null key");
//JAVA TO C# CONVERTER TODO TASK: Java lambdas satisfy functional interfaces, while .NET lambdas satisfy delegates - change the appropriate interface to a delegate:
				A container = m.computeIfAbsent(key, k => downstreamSupplier.Get());
				downstreamAccumulator.Accept(container, t);
			};
			BinaryOperator<IDictionary<K, A>> merger = Collectors.MapMerger<K, A, IDictionary<K, A>>(downstream.Combiner());
//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @SuppressWarnings("unchecked") java.util.function.Supplier<java.util.Map<K, A>> mangledFactory = (java.util.function.Supplier<java.util.Map<K, A>>) mapFactory;
			Supplier<IDictionary<K, A>> mangledFactory = (Supplier<IDictionary<K, A>>) mapFactory;

			if (downstream.Characteristics().Contains(Collector_Characteristics.IDENTITY_FINISH))
			{
				return new CollectorImpl<>(mangledFactory, accumulator, merger, CH_ID);
			}
			else
			{
//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @SuppressWarnings("unchecked") java.util.function.Function<A, A> downstreamFinisher = (java.util.function.Function<A, A>) downstream.finisher();
				Function<A, A> downstreamFinisher = (Function<A, A>) downstream.Finisher();
//JAVA TO C# CONVERTER TODO TASK: Java lambdas satisfy functional interfaces, while .NET lambdas satisfy delegates - change the appropriate interface to a delegate:
				Function<IDictionary<K, A>, M> finisher = intermediate =>
				{
//JAVA TO C# CONVERTER TODO TASK: Java lambdas satisfy functional interfaces, while .NET lambdas satisfy delegates - change the appropriate interface to a delegate:
					intermediate.replaceAll((k, v) => downstreamFinisher.Apply(v));
//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @SuppressWarnings("unchecked") M castResult = (M) intermediate;
					M castResult = (M) intermediate;
					return castResult;
				};
				return new CollectorImpl<>(mangledFactory, accumulator, merger, finisher, CH_NOID);
			}
		}

		/// <summary>
		/// Returns a concurrent {@code Collector} implementing a "group by"
		/// operation on input elements of type {@code T}, grouping elements
		/// according to a classification function.
		/// 
		/// <para>This is a <seealso cref="Collector.Characteristics#CONCURRENT concurrent"/> and
		/// <seealso cref="Collector.Characteristics#UNORDERED unordered"/> Collector.
		/// 
		/// </para>
		/// <para>The classification function maps elements to some key type {@code K}.
		/// The collector produces a {@code ConcurrentMap<K, List<T>>} whose keys are the
		/// values resulting from applying the classification function to the input
		/// elements, and whose corresponding values are {@code List}s containing the
		/// input elements which map to the associated key under the classification
		/// function.
		/// 
		/// </para>
		/// <para>There are no guarantees on the type, mutability, or serializability
		/// of the {@code Map} or {@code List} objects returned, or of the
		/// thread-safety of the {@code List} objects returned.
		/// @implSpec
		/// This produces a result similar to:
		/// <pre>{@code
		///     groupingByConcurrent(classifier, toList());
		/// }</pre>
		/// 
		/// </para>
		/// </summary>
		/// @param <T> the type of the input elements </param>
		/// @param <K> the type of the keys </param>
		/// <param name="classifier"> a classifier function mapping input elements to keys </param>
		/// <returns> a concurrent, unordered {@code Collector} implementing the group-by operation
		/// </returns>
		/// <seealso cref= #groupingBy(Function) </seealso>
		/// <seealso cref= #groupingByConcurrent(Function, Collector) </seealso>
		/// <seealso cref= #groupingByConcurrent(Function, Supplier, Collector) </seealso>
//JAVA TO C# CONVERTER TODO TASK: There is no .NET equivalent to the Java 'super' constraint:
//ORIGINAL LINE: public static <T, K> Collector<T, ?, java.util.concurrent.ConcurrentMap<K, java.util.List<T>>> groupingByConcurrent(java.util.function.Function<? base T, ? extends K> classifier)
//JAVA TO C# CONVERTER TODO TASK: Java wildcard generics are not converted to .NET:
		public static Collector<T, ?, ConcurrentMap<K, IList<T>>> groupingByConcurrent<T, K, T1>(Function<T1> classifier) where T1 : K
		{
			return GroupingByConcurrent(classifier, ConcurrentDictionary::new, ToList());
		}

		/// <summary>
		/// Returns a concurrent {@code Collector} implementing a cascaded "group by"
		/// operation on input elements of type {@code T}, grouping elements
		/// according to a classification function, and then performing a reduction
		/// operation on the values associated with a given key using the specified
		/// downstream {@code Collector}.
		/// 
		/// <para>This is a <seealso cref="Collector.Characteristics#CONCURRENT concurrent"/> and
		/// <seealso cref="Collector.Characteristics#UNORDERED unordered"/> Collector.
		/// 
		/// </para>
		/// <para>The classification function maps elements to some key type {@code K}.
		/// The downstream collector operates on elements of type {@code T} and
		/// produces a result of type {@code D}. The resulting collector produces a
		/// {@code Map<K, D>}.
		/// 
		/// </para>
		/// <para>For example, to compute the set of last names of people in each city,
		/// where the city names are sorted:
		/// <pre>{@code
		///     ConcurrentMap<City, Set<String>> namesByCity
		///         = people.stream().collect(groupingByConcurrent(Person::getCity,
		///                                                        mapping(Person::getLastName, toSet())));
		/// }</pre>
		/// 
		/// </para>
		/// </summary>
		/// @param <T> the type of the input elements </param>
		/// @param <K> the type of the keys </param>
		/// @param <A> the intermediate accumulation type of the downstream collector </param>
		/// @param <D> the result type of the downstream reduction </param>
		/// <param name="classifier"> a classifier function mapping input elements to keys </param>
		/// <param name="downstream"> a {@code Collector} implementing the downstream reduction </param>
		/// <returns> a concurrent, unordered {@code Collector} implementing the cascaded group-by operation
		/// </returns>
		/// <seealso cref= #groupingBy(Function, Collector) </seealso>
		/// <seealso cref= #groupingByConcurrent(Function) </seealso>
		/// <seealso cref= #groupingByConcurrent(Function, Supplier, Collector) </seealso>
//JAVA TO C# CONVERTER TODO TASK: There is no .NET equivalent to the Java 'super' constraint:
//ORIGINAL LINE: public static <T, K, A, D> Collector<T, ?, java.util.concurrent.ConcurrentMap<K, D>> groupingByConcurrent(java.util.function.Function<? base T, ? extends K> classifier, Collector<? base T, A, D> downstream)
//JAVA TO C# CONVERTER TODO TASK: Java wildcard generics are not converted to .NET:
		public static Collector<T, ?, ConcurrentMap<K, D>> groupingByConcurrent<T, K, A, D, T1, T2>(Function<T1> classifier, Collector<T2> downstream) where T1 : K
		{
			return GroupingByConcurrent(classifier, ConcurrentDictionary::new, downstream);
		}

		/// <summary>
		/// Returns a concurrent {@code Collector} implementing a cascaded "group by"
		/// operation on input elements of type {@code T}, grouping elements
		/// according to a classification function, and then performing a reduction
		/// operation on the values associated with a given key using the specified
		/// downstream {@code Collector}.  The {@code ConcurrentMap} produced by the
		/// Collector is created with the supplied factory function.
		/// 
		/// <para>This is a <seealso cref="Collector.Characteristics#CONCURRENT concurrent"/> and
		/// <seealso cref="Collector.Characteristics#UNORDERED unordered"/> Collector.
		/// 
		/// </para>
		/// <para>The classification function maps elements to some key type {@code K}.
		/// The downstream collector operates on elements of type {@code T} and
		/// produces a result of type {@code D}. The resulting collector produces a
		/// {@code Map<K, D>}.
		/// 
		/// </para>
		/// <para>For example, to compute the set of last names of people in each city,
		/// where the city names are sorted:
		/// <pre>{@code
		///     ConcurrentMap<City, Set<String>> namesByCity
		///         = people.stream().collect(groupingBy(Person::getCity, ConcurrentSkipListMap::new,
		///                                              mapping(Person::getLastName, toSet())));
		/// }</pre>
		/// 
		/// 
		/// </para>
		/// </summary>
		/// @param <T> the type of the input elements </param>
		/// @param <K> the type of the keys </param>
		/// @param <A> the intermediate accumulation type of the downstream collector </param>
		/// @param <D> the result type of the downstream reduction </param>
		/// @param <M> the type of the resulting {@code ConcurrentMap} </param>
		/// <param name="classifier"> a classifier function mapping input elements to keys </param>
		/// <param name="downstream"> a {@code Collector} implementing the downstream reduction </param>
		/// <param name="mapFactory"> a function which, when called, produces a new empty
		///                   {@code ConcurrentMap} of the desired type </param>
		/// <returns> a concurrent, unordered {@code Collector} implementing the cascaded group-by operation
		/// </returns>
		/// <seealso cref= #groupingByConcurrent(Function) </seealso>
		/// <seealso cref= #groupingByConcurrent(Function, Collector) </seealso>
		/// <seealso cref= #groupingBy(Function, Supplier, Collector) </seealso>
//JAVA TO C# CONVERTER TODO TASK: There is no .NET equivalent to the Java 'super' constraint:
//ORIGINAL LINE: public static <T, K, A, D, M extends java.util.concurrent.ConcurrentMap<K, D>> Collector<T, ?, M> groupingByConcurrent(java.util.function.Function<? base T, ? extends K> classifier, java.util.function.Supplier<M> mapFactory, Collector<? base T, A, D> downstream)
//JAVA TO C# CONVERTER TODO TASK: Java wildcard generics are not converted to .NET:
		public static Collector<T, ?, M> groupingByConcurrent<T, K, A, D, M, T1, T2>(Function<T1> classifier, Supplier<M> mapFactory, Collector<T2> downstream) where M : java.util.concurrent.ConcurrentMap<K, D> where T1 : K
		{
			Supplier<A> downstreamSupplier = downstream.Supplier();
//JAVA TO C# CONVERTER TODO TASK: There is no .NET equivalent to the Java 'super' constraint:
//ORIGINAL LINE: java.util.function.BiConsumer<A, ? base T> downstreamAccumulator = downstream.accumulator();
//JAVA TO C# CONVERTER TODO TASK: Java wildcard generics are not converted to .NET:
			BiConsumer<A, ?> downstreamAccumulator = downstream.Accumulator();
			BinaryOperator<ConcurrentMap<K, A>> merger = Collectors.MapMerger<K, A, ConcurrentMap<K, A>>(downstream.Combiner());
//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @SuppressWarnings("unchecked") java.util.function.Supplier<java.util.concurrent.ConcurrentMap<K, A>> mangledFactory = (java.util.function.Supplier<java.util.concurrent.ConcurrentMap<K, A>>) mapFactory;
			Supplier<ConcurrentMap<K, A>> mangledFactory = (Supplier<ConcurrentMap<K, A>>) mapFactory;
			BiConsumer<ConcurrentMap<K, A>, T> accumulator;
			if (downstream.Characteristics().Contains(Collector_Characteristics.CONCURRENT))
			{
//JAVA TO C# CONVERTER TODO TASK: Java lambdas satisfy functional interfaces, while .NET lambdas satisfy delegates - change the appropriate interface to a delegate:
				accumulator = (m, t) =>
				{
					K key = Objects.RequireNonNull(classifier.Apply(t), "element cannot be mapped to a null key");
//JAVA TO C# CONVERTER TODO TASK: Java lambdas satisfy functional interfaces, while .NET lambdas satisfy delegates - change the appropriate interface to a delegate:
					A resultContainer = m.computeIfAbsent(key, k => downstreamSupplier.Get());
					downstreamAccumulator.Accept(resultContainer, t);
				};
			}
			else
			{
//JAVA TO C# CONVERTER TODO TASK: Java lambdas satisfy functional interfaces, while .NET lambdas satisfy delegates - change the appropriate interface to a delegate:
				accumulator = (m, t) =>
				{
					K key = Objects.RequireNonNull(classifier.Apply(t), "element cannot be mapped to a null key");
//JAVA TO C# CONVERTER TODO TASK: Java lambdas satisfy functional interfaces, while .NET lambdas satisfy delegates - change the appropriate interface to a delegate:
					A resultContainer = m.computeIfAbsent(key, k => downstreamSupplier.Get());
					lock (resultContainer)
					{
						downstreamAccumulator.Accept(resultContainer, t);
					}
				};
			}

			if (downstream.Characteristics().Contains(Collector_Characteristics.IDENTITY_FINISH))
			{
				return new CollectorImpl<>(mangledFactory, accumulator, merger, CH_CONCURRENT_ID);
			}
			else
			{
//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @SuppressWarnings("unchecked") java.util.function.Function<A, A> downstreamFinisher = (java.util.function.Function<A, A>) downstream.finisher();
				Function<A, A> downstreamFinisher = (Function<A, A>) downstream.Finisher();
//JAVA TO C# CONVERTER TODO TASK: Java lambdas satisfy functional interfaces, while .NET lambdas satisfy delegates - change the appropriate interface to a delegate:
				Function<ConcurrentMap<K, A>, M> finisher = intermediate =>
				{
//JAVA TO C# CONVERTER TODO TASK: Java lambdas satisfy functional interfaces, while .NET lambdas satisfy delegates - change the appropriate interface to a delegate:
					intermediate.replaceAll((k, v) => downstreamFinisher.Apply(v));
//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @SuppressWarnings("unchecked") M castResult = (M) intermediate;
					M castResult = (M) intermediate;
					return castResult;
				};
				return new CollectorImpl<>(mangledFactory, accumulator, merger, finisher, CH_CONCURRENT_NOID);
			}
		}

		/// <summary>
		/// Returns a {@code Collector} which partitions the input elements according
		/// to a {@code Predicate}, and organizes them into a
		/// {@code Map<Boolean, List<T>>}.
		/// 
		/// There are no guarantees on the type, mutability,
		/// serializability, or thread-safety of the {@code Map} returned.
		/// </summary>
		/// @param <T> the type of the input elements </param>
		/// <param name="predicate"> a predicate used for classifying input elements </param>
		/// <returns> a {@code Collector} implementing the partitioning operation
		/// </returns>
		/// <seealso cref= #partitioningBy(Predicate, Collector) </seealso>
//JAVA TO C# CONVERTER TODO TASK: There is no .NET equivalent to the Java 'super' constraint:
//ORIGINAL LINE: public static <T> Collector<T, ?, java.util.Map<Boolean, java.util.List<T>>> partitioningBy(java.util.function.Predicate<? base T> predicate)
//JAVA TO C# CONVERTER TODO TASK: Java wildcard generics are not converted to .NET:
		public static Collector<T, ?, IDictionary<Boolean, IList<T>>> partitioningBy<T, T1>(Predicate<T1> predicate)
		{
			return PartitioningBy(predicate, ToList());
		}

		/// <summary>
		/// Returns a {@code Collector} which partitions the input elements according
		/// to a {@code Predicate}, reduces the values in each partition according to
		/// another {@code Collector}, and organizes them into a
		/// {@code Map<Boolean, D>} whose values are the result of the downstream
		/// reduction.
		/// 
		/// <para>There are no guarantees on the type, mutability,
		/// serializability, or thread-safety of the {@code Map} returned.
		/// 
		/// </para>
		/// </summary>
		/// @param <T> the type of the input elements </param>
		/// @param <A> the intermediate accumulation type of the downstream collector </param>
		/// @param <D> the result type of the downstream reduction </param>
		/// <param name="predicate"> a predicate used for classifying input elements </param>
		/// <param name="downstream"> a {@code Collector} implementing the downstream
		///                   reduction </param>
		/// <returns> a {@code Collector} implementing the cascaded partitioning
		///         operation
		/// </returns>
		/// <seealso cref= #partitioningBy(Predicate) </seealso>
//JAVA TO C# CONVERTER TODO TASK: There is no .NET equivalent to the Java 'super' constraint:
//ORIGINAL LINE: public static <T, D, A> Collector<T, ?, java.util.Map<Boolean, D>> partitioningBy(java.util.function.Predicate<? base T> predicate, Collector<? base T, A, D> downstream)
//JAVA TO C# CONVERTER TODO TASK: Java wildcard generics are not converted to .NET:
		public static Collector<T, ?, IDictionary<Boolean, D>> partitioningBy<T, D, A, T1, T2>(Predicate<T1> predicate, Collector<T2> downstream)
		{
//JAVA TO C# CONVERTER TODO TASK: There is no .NET equivalent to the Java 'super' constraint:
//ORIGINAL LINE: java.util.function.BiConsumer<A, ? base T> downstreamAccumulator = downstream.accumulator();
//JAVA TO C# CONVERTER TODO TASK: Java wildcard generics are not converted to .NET:
			BiConsumer<A, ?> downstreamAccumulator = downstream.Accumulator();
//JAVA TO C# CONVERTER TODO TASK: Java lambdas satisfy functional interfaces, while .NET lambdas satisfy delegates - change the appropriate interface to a delegate:
			BiConsumer<Partition<A>, T> accumulator = (result, t) => downstreamAccumulator.accept(predicate.Test(t) ? result.forTrue : result.forFalse, t);
			BinaryOperator<A> op = downstream.Combiner();
//JAVA TO C# CONVERTER TODO TASK: Java lambdas satisfy functional interfaces, while .NET lambdas satisfy delegates - change the appropriate interface to a delegate:
			BinaryOperator<Partition<A>> merger = (left, right) => new Partition<Partition<A>>(op.Apply(left.forTrue, right.forTrue), op.Apply(left.forFalse, right.forFalse));
//JAVA TO C# CONVERTER TODO TASK: Java lambdas satisfy functional interfaces, while .NET lambdas satisfy delegates - change the appropriate interface to a delegate:
			Supplier<Partition<A>> supplier = () => new Partition<Partition<A>>(downstream.Supplier().Get(), downstream.Supplier().Get());
			if (downstream.Characteristics().Contains(Collector_Characteristics.IDENTITY_FINISH))
			{
				return new CollectorImpl<>(supplier, accumulator, merger, CH_ID);
			}
			else
			{
//JAVA TO C# CONVERTER TODO TASK: Java lambdas satisfy functional interfaces, while .NET lambdas satisfy delegates - change the appropriate interface to a delegate:
				Function<Partition<A>, IDictionary<Boolean, D>> finisher = par => new Partition<Partition<A>, IDictionary<Boolean, D>>(downstream.Finisher().Apply(par.forTrue), downstream.Finisher().Apply(par.forFalse));
				return new CollectorImpl<>(supplier, accumulator, merger, finisher, CH_NOID);
			}
		}

		/// <summary>
		/// Returns a {@code Collector} that accumulates elements into a
		/// {@code Map} whose keys and values are the result of applying the provided
		/// mapping functions to the input elements.
		/// 
		/// <para>If the mapped keys contains duplicates (according to
		/// <seealso cref="Object#equals(Object)"/>), an {@code IllegalStateException} is
		/// thrown when the collection operation is performed.  If the mapped keys
		/// may have duplicates, use <seealso cref="#toMap(Function, Function, BinaryOperator)"/>
		/// instead.
		/// 
		/// @apiNote
		/// It is common for either the key or the value to be the input elements.
		/// In this case, the utility method
		/// <seealso cref="java.util.function.Function#identity()"/> may be helpful.
		/// For example, the following produces a {@code Map} mapping
		/// students to their grade point average:
		/// <pre>{@code
		///     Map<Student, Double> studentToGPA
		///         students.stream().collect(toMap(Functions.identity(),
		///                                         student -> computeGPA(student)));
		/// }</pre>
		/// And the following produces a {@code Map} mapping a unique identifier to
		/// students:
		/// <pre>{@code
		///     Map<String, Student> studentIdToStudent
		///         students.stream().collect(toMap(Student::getId,
		///                                         Functions.identity());
		/// }</pre>
		/// 
		/// @implNote
		/// The returned {@code Collector} is not concurrent.  For parallel stream
		/// pipelines, the {@code combiner} function operates by merging the keys
		/// from one map into another, which can be an expensive operation.  If it is
		/// not required that results are inserted into the {@code Map} in encounter
		/// order, using <seealso cref="#toConcurrentMap(Function, Function)"/>
		/// may offer better parallel performance.
		/// 
		/// </para>
		/// </summary>
		/// @param <T> the type of the input elements </param>
		/// @param <K> the output type of the key mapping function </param>
		/// @param <U> the output type of the value mapping function </param>
		/// <param name="keyMapper"> a mapping function to produce keys </param>
		/// <param name="valueMapper"> a mapping function to produce values </param>
		/// <returns> a {@code Collector} which collects elements into a {@code Map}
		/// whose keys and values are the result of applying mapping functions to
		/// the input elements
		/// </returns>
		/// <seealso cref= #toMap(Function, Function, BinaryOperator) </seealso>
		/// <seealso cref= #toMap(Function, Function, BinaryOperator, Supplier) </seealso>
		/// <seealso cref= #toConcurrentMap(Function, Function) </seealso>
//JAVA TO C# CONVERTER TODO TASK: There is no .NET equivalent to the Java 'super' constraint:
//ORIGINAL LINE: public static <T, K, U> Collector<T, ?, java.util.Map<K,U>> toMap(java.util.function.Function<? base T, ? extends K> keyMapper, java.util.function.Function<? base T, ? extends U> valueMapper)
//JAVA TO C# CONVERTER TODO TASK: Java wildcard generics are not converted to .NET:
		public static Collector<T, ?, IDictionary<K, U>> toMap<T, K, U, T1, T2>(Function<T1> keyMapper, Function<T2> valueMapper) where T1 : K where T2 : U
		{
			return ToMap(keyMapper, valueMapper, ThrowingMerger(), Hashtable::new);
		}

		/// <summary>
		/// Returns a {@code Collector} that accumulates elements into a
		/// {@code Map} whose keys and values are the result of applying the provided
		/// mapping functions to the input elements.
		/// 
		/// <para>If the mapped
		/// keys contains duplicates (according to <seealso cref="Object#equals(Object)"/>),
		/// the value mapping function is applied to each equal element, and the
		/// results are merged using the provided merging function.
		/// 
		/// @apiNote
		/// There are multiple ways to deal with collisions between multiple elements
		/// mapping to the same key.  The other forms of {@code toMap} simply use
		/// a merge function that throws unconditionally, but you can easily write
		/// more flexible merge policies.  For example, if you have a stream
		/// of {@code Person}, and you want to produce a "phone book" mapping name to
		/// address, but it is possible that two persons have the same name, you can
		/// do as follows to gracefully deals with these collisions, and produce a
		/// {@code Map} mapping names to a concatenated list of addresses:
		/// <pre>{@code
		///     Map<String, String> phoneBook
		///         people.stream().collect(toMap(Person::getName,
		///                                       Person::getAddress,
		///                                       (s, a) -> s + ", " + a));
		/// }</pre>
		/// 
		/// @implNote
		/// The returned {@code Collector} is not concurrent.  For parallel stream
		/// pipelines, the {@code combiner} function operates by merging the keys
		/// from one map into another, which can be an expensive operation.  If it is
		/// not required that results are merged into the {@code Map} in encounter
		/// order, using <seealso cref="#toConcurrentMap(Function, Function, BinaryOperator)"/>
		/// may offer better parallel performance.
		/// 
		/// </para>
		/// </summary>
		/// @param <T> the type of the input elements </param>
		/// @param <K> the output type of the key mapping function </param>
		/// @param <U> the output type of the value mapping function </param>
		/// <param name="keyMapper"> a mapping function to produce keys </param>
		/// <param name="valueMapper"> a mapping function to produce values </param>
		/// <param name="mergeFunction"> a merge function, used to resolve collisions between
		///                      values associated with the same key, as supplied
		///                      to <seealso cref="Map#merge(Object, Object, BiFunction)"/> </param>
		/// <returns> a {@code Collector} which collects elements into a {@code Map}
		/// whose keys are the result of applying a key mapping function to the input
		/// elements, and whose values are the result of applying a value mapping
		/// function to all input elements equal to the key and combining them
		/// using the merge function
		/// </returns>
		/// <seealso cref= #toMap(Function, Function) </seealso>
		/// <seealso cref= #toMap(Function, Function, BinaryOperator, Supplier) </seealso>
		/// <seealso cref= #toConcurrentMap(Function, Function, BinaryOperator) </seealso>
//JAVA TO C# CONVERTER TODO TASK: There is no .NET equivalent to the Java 'super' constraint:
//ORIGINAL LINE: public static <T, K, U> Collector<T, ?, java.util.Map<K,U>> toMap(java.util.function.Function<? base T, ? extends K> keyMapper, java.util.function.Function<? base T, ? extends U> valueMapper, java.util.function.BinaryOperator<U> mergeFunction)
//JAVA TO C# CONVERTER TODO TASK: Java wildcard generics are not converted to .NET:
		public static Collector<T, ?, IDictionary<K, U>> toMap<T, K, U, T1, T2>(Function<T1> keyMapper, Function<T2> valueMapper, BinaryOperator<U> mergeFunction) where T1 : K where T2 : U
		{
			return ToMap(keyMapper, valueMapper, mergeFunction, Hashtable::new);
		}

		/// <summary>
		/// Returns a {@code Collector} that accumulates elements into a
		/// {@code Map} whose keys and values are the result of applying the provided
		/// mapping functions to the input elements.
		/// 
		/// <para>If the mapped
		/// keys contains duplicates (according to <seealso cref="Object#equals(Object)"/>),
		/// the value mapping function is applied to each equal element, and the
		/// results are merged using the provided merging function.  The {@code Map}
		/// is created by a provided supplier function.
		/// 
		/// @implNote
		/// The returned {@code Collector} is not concurrent.  For parallel stream
		/// pipelines, the {@code combiner} function operates by merging the keys
		/// from one map into another, which can be an expensive operation.  If it is
		/// not required that results are merged into the {@code Map} in encounter
		/// order, using <seealso cref="#toConcurrentMap(Function, Function, BinaryOperator, Supplier)"/>
		/// may offer better parallel performance.
		/// 
		/// </para>
		/// </summary>
		/// @param <T> the type of the input elements </param>
		/// @param <K> the output type of the key mapping function </param>
		/// @param <U> the output type of the value mapping function </param>
		/// @param <M> the type of the resulting {@code Map} </param>
		/// <param name="keyMapper"> a mapping function to produce keys </param>
		/// <param name="valueMapper"> a mapping function to produce values </param>
		/// <param name="mergeFunction"> a merge function, used to resolve collisions between
		///                      values associated with the same key, as supplied
		///                      to <seealso cref="Map#merge(Object, Object, BiFunction)"/> </param>
		/// <param name="mapSupplier"> a function which returns a new, empty {@code Map} into
		///                    which the results will be inserted </param>
		/// <returns> a {@code Collector} which collects elements into a {@code Map}
		/// whose keys are the result of applying a key mapping function to the input
		/// elements, and whose values are the result of applying a value mapping
		/// function to all input elements equal to the key and combining them
		/// using the merge function
		/// </returns>
		/// <seealso cref= #toMap(Function, Function) </seealso>
		/// <seealso cref= #toMap(Function, Function, BinaryOperator) </seealso>
		/// <seealso cref= #toConcurrentMap(Function, Function, BinaryOperator, Supplier) </seealso>
//JAVA TO C# CONVERTER TODO TASK: There is no .NET equivalent to the Java 'super' constraint:
//ORIGINAL LINE: public static <T, K, U, M extends java.util.Map<K, U>> Collector<T, ?, M> toMap(java.util.function.Function<? base T, ? extends K> keyMapper, java.util.function.Function<? base T, ? extends U> valueMapper, java.util.function.BinaryOperator<U> mergeFunction, java.util.function.Supplier<M> mapSupplier)
//JAVA TO C# CONVERTER TODO TASK: Java wildcard generics are not converted to .NET:
		public static Collector<T, ?, M> toMap<T, K, U, M, T1, T2>(Function<T1> keyMapper, Function<T2> valueMapper, BinaryOperator<U> mergeFunction, Supplier<M> mapSupplier) where M : java.util.Map<K, U> where T1 : K where T2 : U
		{
//JAVA TO C# CONVERTER TODO TASK: Java lambdas satisfy functional interfaces, while .NET lambdas satisfy delegates - change the appropriate interface to a delegate:
			BiConsumer<M, T> accumulator = (map, element) => map.merge(keyMapper.Apply(element), valueMapper.Apply(element), mergeFunction);
			return new CollectorImpl<>(mapSupplier, accumulator, MapMerger(mergeFunction), CH_ID);
		}

		/// <summary>
		/// Returns a concurrent {@code Collector} that accumulates elements into a
		/// {@code ConcurrentMap} whose keys and values are the result of applying
		/// the provided mapping functions to the input elements.
		/// 
		/// <para>If the mapped keys contains duplicates (according to
		/// <seealso cref="Object#equals(Object)"/>), an {@code IllegalStateException} is
		/// thrown when the collection operation is performed.  If the mapped keys
		/// may have duplicates, use
		/// <seealso cref="#toConcurrentMap(Function, Function, BinaryOperator)"/> instead.
		/// 
		/// @apiNote
		/// It is common for either the key or the value to be the input elements.
		/// In this case, the utility method
		/// <seealso cref="java.util.function.Function#identity()"/> may be helpful.
		/// For example, the following produces a {@code Map} mapping
		/// students to their grade point average:
		/// <pre>{@code
		///     Map<Student, Double> studentToGPA
		///         students.stream().collect(toMap(Functions.identity(),
		///                                         student -> computeGPA(student)));
		/// }</pre>
		/// And the following produces a {@code Map} mapping a unique identifier to
		/// students:
		/// <pre>{@code
		///     Map<String, Student> studentIdToStudent
		///         students.stream().collect(toConcurrentMap(Student::getId,
		///                                                   Functions.identity());
		/// }</pre>
		/// 
		/// </para>
		/// <para>This is a <seealso cref="Collector.Characteristics#CONCURRENT concurrent"/> and
		/// <seealso cref="Collector.Characteristics#UNORDERED unordered"/> Collector.
		/// 
		/// </para>
		/// </summary>
		/// @param <T> the type of the input elements </param>
		/// @param <K> the output type of the key mapping function </param>
		/// @param <U> the output type of the value mapping function </param>
		/// <param name="keyMapper"> the mapping function to produce keys </param>
		/// <param name="valueMapper"> the mapping function to produce values </param>
		/// <returns> a concurrent, unordered {@code Collector} which collects elements into a
		/// {@code ConcurrentMap} whose keys are the result of applying a key mapping
		/// function to the input elements, and whose values are the result of
		/// applying a value mapping function to the input elements
		/// </returns>
		/// <seealso cref= #toMap(Function, Function) </seealso>
		/// <seealso cref= #toConcurrentMap(Function, Function, BinaryOperator) </seealso>
		/// <seealso cref= #toConcurrentMap(Function, Function, BinaryOperator, Supplier) </seealso>
//JAVA TO C# CONVERTER TODO TASK: There is no .NET equivalent to the Java 'super' constraint:
//ORIGINAL LINE: public static <T, K, U> Collector<T, ?, java.util.concurrent.ConcurrentMap<K,U>> toConcurrentMap(java.util.function.Function<? base T, ? extends K> keyMapper, java.util.function.Function<? base T, ? extends U> valueMapper)
//JAVA TO C# CONVERTER TODO TASK: Java wildcard generics are not converted to .NET:
		public static Collector<T, ?, ConcurrentMap<K, U>> toConcurrentMap<T, K, U, T1, T2>(Function<T1> keyMapper, Function<T2> valueMapper) where T1 : K where T2 : U
		{
			return ToConcurrentMap(keyMapper, valueMapper, ThrowingMerger(), ConcurrentDictionary::new);
		}

		/// <summary>
		/// Returns a concurrent {@code Collector} that accumulates elements into a
		/// {@code ConcurrentMap} whose keys and values are the result of applying
		/// the provided mapping functions to the input elements.
		/// 
		/// <para>If the mapped keys contains duplicates (according to <seealso cref="Object#equals(Object)"/>),
		/// the value mapping function is applied to each equal element, and the
		/// results are merged using the provided merging function.
		/// 
		/// @apiNote
		/// There are multiple ways to deal with collisions between multiple elements
		/// mapping to the same key.  The other forms of {@code toConcurrentMap} simply use
		/// a merge function that throws unconditionally, but you can easily write
		/// more flexible merge policies.  For example, if you have a stream
		/// of {@code Person}, and you want to produce a "phone book" mapping name to
		/// address, but it is possible that two persons have the same name, you can
		/// do as follows to gracefully deals with these collisions, and produce a
		/// {@code Map} mapping names to a concatenated list of addresses:
		/// <pre>{@code
		///     Map<String, String> phoneBook
		///         people.stream().collect(toConcurrentMap(Person::getName,
		///                                                 Person::getAddress,
		///                                                 (s, a) -> s + ", " + a));
		/// }</pre>
		/// 
		/// </para>
		/// <para>This is a <seealso cref="Collector.Characteristics#CONCURRENT concurrent"/> and
		/// <seealso cref="Collector.Characteristics#UNORDERED unordered"/> Collector.
		/// 
		/// </para>
		/// </summary>
		/// @param <T> the type of the input elements </param>
		/// @param <K> the output type of the key mapping function </param>
		/// @param <U> the output type of the value mapping function </param>
		/// <param name="keyMapper"> a mapping function to produce keys </param>
		/// <param name="valueMapper"> a mapping function to produce values </param>
		/// <param name="mergeFunction"> a merge function, used to resolve collisions between
		///                      values associated with the same key, as supplied
		///                      to <seealso cref="Map#merge(Object, Object, BiFunction)"/> </param>
		/// <returns> a concurrent, unordered {@code Collector} which collects elements into a
		/// {@code ConcurrentMap} whose keys are the result of applying a key mapping
		/// function to the input elements, and whose values are the result of
		/// applying a value mapping function to all input elements equal to the key
		/// and combining them using the merge function
		/// </returns>
		/// <seealso cref= #toConcurrentMap(Function, Function) </seealso>
		/// <seealso cref= #toConcurrentMap(Function, Function, BinaryOperator, Supplier) </seealso>
		/// <seealso cref= #toMap(Function, Function, BinaryOperator) </seealso>
//JAVA TO C# CONVERTER TODO TASK: There is no .NET equivalent to the Java 'super' constraint:
//ORIGINAL LINE: public static <T, K, U> Collector<T, ?, java.util.concurrent.ConcurrentMap<K,U>> toConcurrentMap(java.util.function.Function<? base T, ? extends K> keyMapper, java.util.function.Function<? base T, ? extends U> valueMapper, java.util.function.BinaryOperator<U> mergeFunction)
//JAVA TO C# CONVERTER TODO TASK: Java wildcard generics are not converted to .NET:
		public static Collector<T, ?, ConcurrentMap<K, U>> toConcurrentMap<T, K, U, T1, T2>(Function<T1> keyMapper, Function<T2> valueMapper, BinaryOperator<U> mergeFunction) where T1 : K where T2 : U
		{
			return ToConcurrentMap(keyMapper, valueMapper, mergeFunction, ConcurrentDictionary::new);
		}

		/// <summary>
		/// Returns a concurrent {@code Collector} that accumulates elements into a
		/// {@code ConcurrentMap} whose keys and values are the result of applying
		/// the provided mapping functions to the input elements.
		/// 
		/// <para>If the mapped keys contains duplicates (according to <seealso cref="Object#equals(Object)"/>),
		/// the value mapping function is applied to each equal element, and the
		/// results are merged using the provided merging function.  The
		/// {@code ConcurrentMap} is created by a provided supplier function.
		/// 
		/// </para>
		/// <para>This is a <seealso cref="Collector.Characteristics#CONCURRENT concurrent"/> and
		/// <seealso cref="Collector.Characteristics#UNORDERED unordered"/> Collector.
		/// 
		/// </para>
		/// </summary>
		/// @param <T> the type of the input elements </param>
		/// @param <K> the output type of the key mapping function </param>
		/// @param <U> the output type of the value mapping function </param>
		/// @param <M> the type of the resulting {@code ConcurrentMap} </param>
		/// <param name="keyMapper"> a mapping function to produce keys </param>
		/// <param name="valueMapper"> a mapping function to produce values </param>
		/// <param name="mergeFunction"> a merge function, used to resolve collisions between
		///                      values associated with the same key, as supplied
		///                      to <seealso cref="Map#merge(Object, Object, BiFunction)"/> </param>
		/// <param name="mapSupplier"> a function which returns a new, empty {@code Map} into
		///                    which the results will be inserted </param>
		/// <returns> a concurrent, unordered {@code Collector} which collects elements into a
		/// {@code ConcurrentMap} whose keys are the result of applying a key mapping
		/// function to the input elements, and whose values are the result of
		/// applying a value mapping function to all input elements equal to the key
		/// and combining them using the merge function
		/// </returns>
		/// <seealso cref= #toConcurrentMap(Function, Function) </seealso>
		/// <seealso cref= #toConcurrentMap(Function, Function, BinaryOperator) </seealso>
		/// <seealso cref= #toMap(Function, Function, BinaryOperator, Supplier) </seealso>
//JAVA TO C# CONVERTER TODO TASK: There is no .NET equivalent to the Java 'super' constraint:
//ORIGINAL LINE: public static <T, K, U, M extends java.util.concurrent.ConcurrentMap<K, U>> Collector<T, ?, M> toConcurrentMap(java.util.function.Function<? base T, ? extends K> keyMapper, java.util.function.Function<? base T, ? extends U> valueMapper, java.util.function.BinaryOperator<U> mergeFunction, java.util.function.Supplier<M> mapSupplier)
//JAVA TO C# CONVERTER TODO TASK: Java wildcard generics are not converted to .NET:
		public static Collector<T, ?, M> toConcurrentMap<T, K, U, M, T1, T2>(Function<T1> keyMapper, Function<T2> valueMapper, BinaryOperator<U> mergeFunction, Supplier<M> mapSupplier) where M : java.util.concurrent.ConcurrentMap<K, U> where T1 : K where T2 : U
		{
//JAVA TO C# CONVERTER TODO TASK: Java lambdas satisfy functional interfaces, while .NET lambdas satisfy delegates - change the appropriate interface to a delegate:
			BiConsumer<M, T> accumulator = (map, element) => map.merge(keyMapper.Apply(element), valueMapper.Apply(element), mergeFunction);
			return new CollectorImpl<>(mapSupplier, accumulator, MapMerger(mergeFunction), CH_CONCURRENT_ID);
		}

		/// <summary>
		/// Returns a {@code Collector} which applies an {@code int}-producing
		/// mapping function to each input element, and returns summary statistics
		/// for the resulting values.
		/// </summary>
		/// @param <T> the type of the input elements </param>
		/// <param name="mapper"> a mapping function to apply to each element </param>
		/// <returns> a {@code Collector} implementing the summary-statistics reduction
		/// </returns>
		/// <seealso cref= #summarizingDouble(ToDoubleFunction) </seealso>
		/// <seealso cref= #summarizingLong(ToLongFunction) </seealso>
//JAVA TO C# CONVERTER TODO TASK: There is no .NET equivalent to the Java 'super' constraint:
//ORIGINAL LINE: public static <T> Collector<T, ?, java.util.IntSummaryStatistics> summarizingInt(java.util.function.ToIntFunction<? base T> mapper)
//JAVA TO C# CONVERTER TODO TASK: Java wildcard generics are not converted to .NET:
		public static Collector<T, ?, IntSummaryStatistics> summarizingInt<T, T1>(ToIntFunction<T1> mapper)
		{
//JAVA TO C# CONVERTER TODO TASK: Java lambdas satisfy functional interfaces, while .NET lambdas satisfy delegates - change the appropriate interface to a delegate:
			return new CollectorImpl<T, IntSummaryStatistics, IntSummaryStatistics>(IntSummaryStatistics::new, (r, t) => r.accept(mapper.ApplyAsInt(t)), (l, r) => {l.combine(r); return l;}, CH_ID);
		}

		/// <summary>
		/// Returns a {@code Collector} which applies an {@code long}-producing
		/// mapping function to each input element, and returns summary statistics
		/// for the resulting values.
		/// </summary>
		/// @param <T> the type of the input elements </param>
		/// <param name="mapper"> the mapping function to apply to each element </param>
		/// <returns> a {@code Collector} implementing the summary-statistics reduction
		/// </returns>
		/// <seealso cref= #summarizingDouble(ToDoubleFunction) </seealso>
		/// <seealso cref= #summarizingInt(ToIntFunction) </seealso>
//JAVA TO C# CONVERTER TODO TASK: There is no .NET equivalent to the Java 'super' constraint:
//ORIGINAL LINE: public static <T> Collector<T, ?, java.util.LongSummaryStatistics> summarizingLong(java.util.function.ToLongFunction<? base T> mapper)
//JAVA TO C# CONVERTER TODO TASK: Java wildcard generics are not converted to .NET:
		public static Collector<T, ?, LongSummaryStatistics> summarizingLong<T, T1>(ToLongFunction<T1> mapper)
		{
//JAVA TO C# CONVERTER TODO TASK: Java lambdas satisfy functional interfaces, while .NET lambdas satisfy delegates - change the appropriate interface to a delegate:
			return new CollectorImpl<T, LongSummaryStatistics, LongSummaryStatistics>(LongSummaryStatistics::new, (r, t) => r.accept(mapper.ApplyAsLong(t)), (l, r) => {l.combine(r); return l;}, CH_ID);
		}

		/// <summary>
		/// Returns a {@code Collector} which applies an {@code double}-producing
		/// mapping function to each input element, and returns summary statistics
		/// for the resulting values.
		/// </summary>
		/// @param <T> the type of the input elements </param>
		/// <param name="mapper"> a mapping function to apply to each element </param>
		/// <returns> a {@code Collector} implementing the summary-statistics reduction
		/// </returns>
		/// <seealso cref= #summarizingLong(ToLongFunction) </seealso>
		/// <seealso cref= #summarizingInt(ToIntFunction) </seealso>
//JAVA TO C# CONVERTER TODO TASK: There is no .NET equivalent to the Java 'super' constraint:
//ORIGINAL LINE: public static <T> Collector<T, ?, java.util.DoubleSummaryStatistics> summarizingDouble(java.util.function.ToDoubleFunction<? base T> mapper)
//JAVA TO C# CONVERTER TODO TASK: Java wildcard generics are not converted to .NET:
		public static Collector<T, ?, DoubleSummaryStatistics> summarizingDouble<T, T1>(ToDoubleFunction<T1> mapper)
		{
//JAVA TO C# CONVERTER TODO TASK: Java lambdas satisfy functional interfaces, while .NET lambdas satisfy delegates - change the appropriate interface to a delegate:
			return new CollectorImpl<T, DoubleSummaryStatistics, DoubleSummaryStatistics>(DoubleSummaryStatistics::new, (r, t) => r.accept(mapper.ApplyAsDouble(t)), (l, r) => {l.combine(r); return l;}, CH_ID);
		}

		/// <summary>
		/// Implementation class used by partitioningBy.
		/// </summary>
		private sealed class Partition<T> : AbstractMap<Boolean, T>, Map<Boolean, T>
		{
			internal readonly T ForTrue;
			internal readonly T ForFalse;

			internal Partition(T forTrue, T forFalse)
			{
				this.ForTrue = forTrue;
				this.ForFalse = forFalse;
			}

			public override Set<java.util.Map_Entry<Boolean, T>> EntrySet()
			{
				return new AbstractSetAnonymousInnerClassHelper(this);
			}

			private class AbstractSetAnonymousInnerClassHelper : AbstractSet<java.util.Map_Entry<Boolean, T>>
			{
				private readonly Partition<T> OuterInstance;

				public AbstractSetAnonymousInnerClassHelper(Partition<T> outerInstance)
				{
					this.outerInstance = outerInstance;
				}

				public override IEnumerator<java.util.Map_Entry<Boolean, T>> Iterator()
				{
					java.util.Map_Entry<Boolean, T> falseEntry = new SimpleImmutableEntry<Boolean, T>(java.util.Map_Fields.False, OuterInstance.ForFalse);
					java.util.Map_Entry<Boolean, T> trueEntry = new SimpleImmutableEntry<Boolean, T>(java.util.Map_Fields.True, OuterInstance.ForTrue);
					return Arrays.asList(falseEntry, trueEntry).GetEnumerator();
				}

				public override int Size()
				{
					return 2;
				}
			}
		}
	}

}
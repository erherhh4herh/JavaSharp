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
	/// An extension of <seealso cref="Consumer"/> used to conduct values through the stages of
	/// a stream pipeline, with additional methods to manage size information,
	/// control flow, etc.  Before calling the {@code accept()} method on a
	/// {@code Sink} for the first time, you must first call the {@code begin()}
	/// method to inform it that data is coming (optionally informing the sink how
	/// much data is coming), and after all data has been sent, you must call the
	/// {@code end()} method.  After calling {@code end()}, you should not call
	/// {@code accept()} without again calling {@code begin()}.  {@code Sink} also
	/// offers a mechanism by which the sink can cooperatively signal that it does
	/// not wish to receive any more data (the {@code cancellationRequested()}
	/// method), which a source can poll before sending more data to the
	/// {@code Sink}.
	/// 
	/// <para>A sink may be in one of two states: an initial state and an active state.
	/// It starts out in the initial state; the {@code begin()} method transitions
	/// it to the active state, and the {@code end()} method transitions it back into
	/// the initial state, where it can be re-used.  Data-accepting methods (such as
	/// {@code accept()} are only valid in the active state.
	/// 
	/// @apiNote
	/// A stream pipeline consists of a source, zero or more intermediate stages
	/// (such as filtering or mapping), and a terminal stage, such as reduction or
	/// for-each.  For concreteness, consider the pipeline:
	/// 
	/// <pre>{@code
	///     int longestStringLengthStartingWithA
	///         = strings.stream()
	///                  .filter(s -> s.startsWith("A"))
	///                  .mapToInt(String::length)
	///                  .max();
	/// }</pre>
	/// 
	/// </para>
	/// <para>Here, we have three stages, filtering, mapping, and reducing.  The
	/// filtering stage consumes strings and emits a subset of those strings; the
	/// mapping stage consumes strings and emits ints; the reduction stage consumes
	/// those ints and computes the maximal value.
	/// 
	/// </para>
	/// <para>A {@code Sink} instance is used to represent each stage of this pipeline,
	/// whether the stage accepts objects, ints, longs, or doubles.  Sink has entry
	/// points for {@code accept(Object)}, {@code accept(int)}, etc, so that we do
	/// not need a specialized interface for each primitive specialization.  (It
	/// might be called a "kitchen sink" for this omnivorous tendency.)  The entry
	/// point to the pipeline is the {@code Sink} for the filtering stage, which
	/// sends some elements "downstream" -- into the {@code Sink} for the mapping
	/// stage, which in turn sends integral values downstream into the {@code Sink}
	/// for the reduction stage. The {@code Sink} implementations associated with a
	/// given stage is expected to know the data type for the next stage, and call
	/// the correct {@code accept} method on its downstream {@code Sink}.  Similarly,
	/// each stage must implement the correct {@code accept} method corresponding to
	/// the data type it accepts.
	/// 
	/// </para>
	/// <para>The specialized subtypes such as <seealso cref="Sink.OfInt"/> override
	/// {@code accept(Object)} to call the appropriate primitive specialization of
	/// {@code accept}, implement the appropriate primitive specialization of
	/// {@code Consumer}, and re-abstract the appropriate primitive specialization of
	/// {@code accept}.
	/// 
	/// </para>
	/// <para>The chaining subtypes such as <seealso cref="ChainedInt"/> not only implement
	/// {@code Sink.OfInt}, but also maintain a {@code downstream} field which
	/// represents the downstream {@code Sink}, and implement the methods
	/// {@code begin()}, {@code end()}, and {@code cancellationRequested()} to
	/// delegate to the downstream {@code Sink}.  Most implementations of
	/// intermediate operations will use these chaining wrappers.  For example, the
	/// mapping stage in the above example would look like:
	/// 
	/// <pre>{@code
	///     IntSink is = new Sink.ChainedReference<U>(sink) {
	///         public void accept(U u) {
	///             downstream.accept(mapper.applyAsInt(u));
	///         }
	///     };
	/// }</pre>
	/// 
	/// </para>
	/// <para>Here, we implement {@code Sink.ChainedReference<U>}, meaning that we expect
	/// to receive elements of type {@code U} as input, and pass the downstream sink
	/// to the constructor.  Because the next stage expects to receive integers, we
	/// must call the {@code accept(int)} method when emitting values to the downstream.
	/// The {@code accept()} method applies the mapping function from {@code U} to
	/// {@code int} and passes the resulting value to the downstream {@code Sink}.
	/// 
	/// </para>
	/// </summary>
	/// @param <T> type of elements for value streams
	/// @since 1.8 </param>
	internal interface Sink<T> : Consumer<T>
	{
		/// <summary>
		/// Resets the sink state to receive a fresh data set.  This must be called
		/// before sending any data to the sink.  After calling <seealso cref="#end()"/>,
		/// you may call this method to reset the sink for another calculation. </summary>
		/// <param name="size"> The exact size of the data to be pushed downstream, if
		/// known or {@code -1} if unknown or infinite.
		/// 
		/// <para>Prior to this call, the sink must be in the initial state, and after
		/// this call it is in the active state. </param>
//JAVA TO C# CONVERTER TODO TASK: There is no equivalent in C# to Java default interface methods:
//		default void begin(long size)
	//	{
	//	}

		/// <summary>
		/// Indicates that all elements have been pushed.  If the {@code Sink} is
		/// stateful, it should send any stored state downstream at this time, and
		/// should clear any accumulated state (and associated resources).
		/// 
		/// <para>Prior to this call, the sink must be in the active state, and after
		/// this call it is returned to the initial state.
		/// </para>
		/// </summary>
//JAVA TO C# CONVERTER TODO TASK: There is no equivalent in C# to Java default interface methods:
//		default void end()
	//	{
	//	}

		/// <summary>
		/// Indicates that this {@code Sink} does not wish to receive any more data.
		/// 
		/// @implSpec The default implementation always returns false.
		/// </summary>
		/// <returns> true if cancellation is requested </returns>
//JAVA TO C# CONVERTER TODO TASK: There is no equivalent in C# to Java default interface methods:
//		default boolean cancellationRequested()
	//	{
	//	}

		/// <summary>
		/// Accepts an int value.
		/// 
		/// @implSpec The default implementation throws IllegalStateException.
		/// </summary>
		/// <exception cref="IllegalStateException"> if this sink does not accept int values </exception>
//JAVA TO C# CONVERTER TODO TASK: There is no equivalent in C# to Java default interface methods:
//		default void accept(int value)
	//	{
	//		throw new IllegalStateException("called wrong accept method");
	//	}

		/// <summary>
		/// Accepts a long value.
		/// 
		/// @implSpec The default implementation throws IllegalStateException.
		/// </summary>
		/// <exception cref="IllegalStateException"> if this sink does not accept long values </exception>
//JAVA TO C# CONVERTER TODO TASK: There is no equivalent in C# to Java default interface methods:
//		default void accept(long value)
	//	{
	//		throw new IllegalStateException("called wrong accept method");
	//	}

		/// <summary>
		/// Accepts a double value.
		/// 
		/// @implSpec The default implementation throws IllegalStateException.
		/// </summary>
		/// <exception cref="IllegalStateException"> if this sink does not accept double values </exception>
//JAVA TO C# CONVERTER TODO TASK: There is no equivalent in C# to Java default interface methods:
//		default void accept(double value)
	//	{
	//		throw new IllegalStateException("called wrong accept method");
	//	}

		/// <summary>
		/// {@code Sink} that implements {@code Sink<Integer>}, re-abstracts
		/// {@code accept(int)}, and wires {@code accept(Integer)} to bridge to
		/// {@code accept(int)}.
		/// </summary>

		/// <summary>
		/// {@code Sink} that implements {@code Sink<Long>}, re-abstracts
		/// {@code accept(long)}, and wires {@code accept(Long)} to bridge to
		/// {@code accept(long)}.
		/// </summary>

		/// <summary>
		/// {@code Sink} that implements {@code Sink<Double>}, re-abstracts
		/// {@code accept(double)}, and wires {@code accept(Double)} to bridge to
		/// {@code accept(double)}.
		/// </summary>

		/// <summary>
		/// Abstract {@code Sink} implementation for creating chains of
		/// sinks.  The {@code begin}, {@code end}, and
		/// {@code cancellationRequested} methods are wired to chain to the
		/// downstream {@code Sink}.  This implementation takes a downstream
		/// {@code Sink} of unknown input shape and produces a {@code Sink<T>}.  The
		/// implementation of the {@code accept()} method must call the correct
		/// {@code accept()} method on the downstream {@code Sink}.
		/// </summary>

		/// <summary>
		/// Abstract {@code Sink} implementation designed for creating chains of
		/// sinks.  The {@code begin}, {@code end}, and
		/// {@code cancellationRequested} methods are wired to chain to the
		/// downstream {@code Sink}.  This implementation takes a downstream
		/// {@code Sink} of unknown input shape and produces a {@code Sink.OfInt}.
		/// The implementation of the {@code accept()} method must call the correct
		/// {@code accept()} method on the downstream {@code Sink}.
		/// </summary>

		/// <summary>
		/// Abstract {@code Sink} implementation designed for creating chains of
		/// sinks.  The {@code begin}, {@code end}, and
		/// {@code cancellationRequested} methods are wired to chain to the
		/// downstream {@code Sink}.  This implementation takes a downstream
		/// {@code Sink} of unknown input shape and produces a {@code Sink.OfLong}.
		/// The implementation of the {@code accept()} method must call the correct
		/// {@code accept()} method on the downstream {@code Sink}.
		/// </summary>

		/// <summary>
		/// Abstract {@code Sink} implementation designed for creating chains of
		/// sinks.  The {@code begin}, {@code end}, and
		/// {@code cancellationRequested} methods are wired to chain to the
		/// downstream {@code Sink}.  This implementation takes a downstream
		/// {@code Sink} of unknown input shape and produces a {@code Sink.OfDouble}.
		/// The implementation of the {@code accept()} method must call the correct
		/// {@code accept()} method on the downstream {@code Sink}.
		/// </summary>
	}

	public static class Sink_Fields
	{
			public static readonly return False;
	}

	internal interface Sink_OfInt : Sink<Integer>, IntConsumer
	{
		void Accept(int value);

//JAVA TO C# CONVERTER TODO TASK: There is no equivalent in C# to Java default interface methods:
//		default void accept(Integer i)
	//	{
	//		if (Tripwire.ENABLED)
	//			Tripwire.trip(getClass(), "{0} calling Sink.OfInt.accept(Integer)");
	//		accept(i.intValue());
	//	}
	}

	internal interface Sink_OfLong : Sink<Long>, LongConsumer
	{
		void Accept(long value);

//JAVA TO C# CONVERTER TODO TASK: There is no equivalent in C# to Java default interface methods:
//		default void accept(Long i)
	//	{
	//		if (Tripwire.ENABLED)
	//			Tripwire.trip(getClass(), "{0} calling Sink.OfLong.accept(Long)");
	//		accept(i.longValue());
	//	}
	}

	internal interface Sink_OfDouble : Sink<Double>, DoubleConsumer
	{
		void Accept(double value);

//JAVA TO C# CONVERTER TODO TASK: There is no equivalent in C# to Java default interface methods:
//		default void accept(Double i)
	//	{
	//		if (Tripwire.ENABLED)
	//			Tripwire.trip(getClass(), "{0} calling Sink.OfDouble.accept(Double)");
	//		accept(i.doubleValue());
	//	}
	}

	internal abstract class Sink_ChainedReference<T, E_OUT> : Sink<T>
	{
//JAVA TO C# CONVERTER TODO TASK: There is no .NET equivalent to the Java 'super' constraint:
//ORIGINAL LINE: protected final Sink<? base E_OUT> downstream;
//JAVA TO C# CONVERTER TODO TASK: Java wildcard generics are not converted to .NET:
		protected internal readonly Sink<?> Downstream;

//JAVA TO C# CONVERTER TODO TASK: There is no .NET equivalent to the Java 'super' constraint:
//ORIGINAL LINE: public Sink_ChainedReference(Sink<? base E_OUT> downstream)
		public override Sink_ChainedReference<T1>(Sink<T1> downstream)
		{
			this.Downstream = Objects.RequireNonNull(downstream);
		}

		public override void Begin(long size)
		{
			Downstream.begin(size);
		}

		public override void End()
		{
			Downstream.end();
		}

		public override bool CancellationRequested()
		{
			return Downstream.cancellationRequested();
		}
	}

	internal abstract class Sink_ChainedInt<E_OUT> : Sink_OfInt
	{
		public abstract void Accept(int value);
//JAVA TO C# CONVERTER TODO TASK: There is no .NET equivalent to the Java 'super' constraint:
//ORIGINAL LINE: protected final Sink<? base E_OUT> downstream;
//JAVA TO C# CONVERTER TODO TASK: Java wildcard generics are not converted to .NET:
		protected internal readonly Sink<?> Downstream;

//JAVA TO C# CONVERTER TODO TASK: There is no .NET equivalent to the Java 'super' constraint:
//ORIGINAL LINE: public Sink_ChainedInt(Sink<? base E_OUT> downstream)
		public Sink_ChainedInt<T1>(Sink<T1> downstream)
		{
			this.Downstream = Objects.RequireNonNull(downstream);
		}

		public override void Begin(long size)
		{
			Downstream.begin(size);
		}

		public override void End()
		{
			Downstream.end();
		}

		public override bool CancellationRequested()
		{
			return Downstream.cancellationRequested();
		}
	}

	internal abstract class Sink_ChainedLong<E_OUT> : Sink_OfLong
	{
		public abstract void Accept(long value);
//JAVA TO C# CONVERTER TODO TASK: There is no .NET equivalent to the Java 'super' constraint:
//ORIGINAL LINE: protected final Sink<? base E_OUT> downstream;
//JAVA TO C# CONVERTER TODO TASK: Java wildcard generics are not converted to .NET:
		protected internal readonly Sink<?> Downstream;

//JAVA TO C# CONVERTER TODO TASK: There is no .NET equivalent to the Java 'super' constraint:
//ORIGINAL LINE: public Sink_ChainedLong(Sink<? base E_OUT> downstream)
		public Sink_ChainedLong<T1>(Sink<T1> downstream)
		{
			this.Downstream = Objects.RequireNonNull(downstream);
		}

		public override void Begin(long size)
		{
			Downstream.begin(size);
		}

		public override void End()
		{
			Downstream.end();
		}

		public override bool CancellationRequested()
		{
			return Downstream.cancellationRequested();
		}
	}

	internal abstract class Sink_ChainedDouble<E_OUT> : Sink_OfDouble
	{
		public abstract void Accept(double value);
//JAVA TO C# CONVERTER TODO TASK: There is no .NET equivalent to the Java 'super' constraint:
//ORIGINAL LINE: protected final Sink<? base E_OUT> downstream;
//JAVA TO C# CONVERTER TODO TASK: Java wildcard generics are not converted to .NET:
		protected internal readonly Sink<?> Downstream;

//JAVA TO C# CONVERTER TODO TASK: There is no .NET equivalent to the Java 'super' constraint:
//ORIGINAL LINE: public Sink_ChainedDouble(Sink<? base E_OUT> downstream)
		public Sink_ChainedDouble<T1>(Sink<T1> downstream)
		{
			this.Downstream = Objects.RequireNonNull(downstream);
		}

		public override void Begin(long size)
		{
			Downstream.begin(size);
		}

		public override void End()
		{
			Downstream.end();
		}

		public override bool CancellationRequested()
		{
			return Downstream.cancellationRequested();
		}
	}

}
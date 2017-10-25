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
	/// A container object which may or may not contain a {@code long} value.
	/// If a value is present, {@code isPresent()} will return {@code true} and
	/// {@code getAsLong()} will return the value.
	/// 
	/// <para>Additional methods that depend on the presence or absence of a contained
	/// value are provided, such as <seealso cref="#orElse(long) orElse()"/>
	/// (return a default value if value not present) and
	/// <seealso cref="#ifPresent(java.util.function.LongConsumer) ifPresent()"/> (execute a block
	/// of code if the value is present).
	/// 
	/// </para>
	/// <para>This is a <a href="../lang/doc-files/ValueBased.html">value-based</a>
	/// class; use of identity-sensitive operations (including reference equality
	/// ({@code ==}), identity hash code, or synchronization) on instances of
	/// {@code OptionalLong} may have unpredictable results and should be avoided.
	/// 
	/// @since 1.8
	/// </para>
	/// </summary>
	public sealed class OptionalLong
	{
		/// <summary>
		/// Common instance for {@code empty()}.
		/// </summary>
		private static readonly OptionalLong EMPTY = new OptionalLong();

		/// <summary>
		/// If true then the value is present, otherwise indicates no value is present
		/// </summary>
		private readonly bool IsPresent;
		private readonly long Value;

		/// <summary>
		/// Construct an empty instance.
		/// 
		/// @implNote generally only one empty instance, <seealso cref="OptionalLong#EMPTY"/>,
		/// should exist per VM.
		/// </summary>
		private OptionalLong()
		{
			this.IsPresent = false;
			this.Value = 0;
		}

		/// <summary>
		/// Returns an empty {@code OptionalLong} instance.  No value is present for this
		/// OptionalLong.
		/// 
		/// @apiNote Though it may be tempting to do so, avoid testing if an object
		/// is empty by comparing with {@code ==} against instances returned by
		/// {@code Option.empty()}. There is no guarantee that it is a singleton.
		/// Instead, use <seealso cref="#isPresent()"/>.
		/// </summary>
		///  <returns> an empty {@code OptionalLong}. </returns>
		public static OptionalLong Empty()
		{
			return EMPTY;
		}

		/// <summary>
		/// Construct an instance with the value present.
		/// </summary>
		/// <param name="value"> the long value to be present </param>
		private OptionalLong(long value)
		{
			this.IsPresent = true;
			this.Value = value;
		}

		/// <summary>
		/// Return an {@code OptionalLong} with the specified value present.
		/// </summary>
		/// <param name="value"> the value to be present </param>
		/// <returns> an {@code OptionalLong} with the value present </returns>
		public static OptionalLong Of(long value)
		{
			return new OptionalLong(value);
		}

		/// <summary>
		/// If a value is present in this {@code OptionalLong}, returns the value,
		/// otherwise throws {@code NoSuchElementException}.
		/// </summary>
		/// <returns> the value held by this {@code OptionalLong} </returns>
		/// <exception cref="NoSuchElementException"> if there is no value present
		/// </exception>
		/// <seealso cref= OptionalLong#isPresent() </seealso>
		public long AsLong
		{
			get
			{
				if (!IsPresent)
				{
					throw new NoSuchElementException("No value present");
				}
				return Value;
			}
		}

		/// <summary>
		/// Return {@code true} if there is a value present, otherwise {@code false}.
		/// </summary>
		/// <returns> {@code true} if there is a value present, otherwise {@code false} </returns>
		public bool Present
		{
			get
			{
				return IsPresent;
			}
		}

		/// <summary>
		/// Have the specified consumer accept the value if a value is present,
		/// otherwise do nothing.
		/// </summary>
		/// <param name="consumer"> block to be executed if a value is present </param>
		/// <exception cref="NullPointerException"> if value is present and {@code consumer} is
		/// null </exception>
		public void IfPresent(LongConsumer consumer)
		{
			if (IsPresent)
			{
				consumer.Accept(Value);
			}
		}

		/// <summary>
		/// Return the value if present, otherwise return {@code other}.
		/// </summary>
		/// <param name="other"> the value to be returned if there is no value present </param>
		/// <returns> the value, if present, otherwise {@code other} </returns>
		public long OrElse(long other)
		{
			return IsPresent ? Value : other;
		}

		/// <summary>
		/// Return the value if present, otherwise invoke {@code other} and return
		/// the result of that invocation.
		/// </summary>
		/// <param name="other"> a {@code LongSupplier} whose result is returned if no value
		/// is present </param>
		/// <returns> the value if present otherwise the result of {@code other.getAsLong()} </returns>
		/// <exception cref="NullPointerException"> if value is not present and {@code other} is
		/// null </exception>
		public long OrElseGet(LongSupplier other)
		{
			return IsPresent ? Value : other.AsLong;
		}

		/// <summary>
		/// Return the contained value, if present, otherwise throw an exception
		/// to be created by the provided supplier.
		/// 
		/// @apiNote A method reference to the exception constructor with an empty
		/// argument list can be used as the supplier. For example,
		/// {@code IllegalStateException::new}
		/// </summary>
		/// @param <X> Type of the exception to be thrown </param>
		/// <param name="exceptionSupplier"> The supplier which will return the exception to
		/// be thrown </param>
		/// <returns> the present value </returns>
		/// <exception cref="X"> if there is no value present </exception>
		/// <exception cref="NullPointerException"> if no value is present and
		/// {@code exceptionSupplier} is null </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public<X extends Throwable> long orElseThrow(java.util.function.Supplier<X> exceptionSupplier) throws X
		public long orElseThrow<X>(Supplier<X> exceptionSupplier) where X : Throwable
		{
			if (IsPresent)
			{
				return Value;
			}
			else
			{
				throw exceptionSupplier.Get();
			}
		}

		/// <summary>
		/// Indicates whether some other object is "equal to" this OptionalLong. The
		/// other object is considered equal if:
		/// <ul>
		/// <li>it is also an {@code OptionalLong} and;
		/// <li>both instances have no value present or;
		/// <li>the present values are "equal to" each other via {@code ==}.
		/// </ul>
		/// </summary>
		/// <param name="obj"> an object to be tested for equality </param>
		/// <returns> {code true} if the other object is "equal to" this object
		/// otherwise {@code false} </returns>
		public override bool Equals(Object obj)
		{
			if (this == obj)
			{
				return true;
			}

			if (!(obj is OptionalLong))
			{
				return false;
			}

			OptionalLong other = (OptionalLong) obj;
			return (IsPresent && other.IsPresent) ? Value == other.Value : IsPresent == other.IsPresent;
		}

		/// <summary>
		/// Returns the hash code value of the present value, if any, or 0 (zero) if
		/// no value is present.
		/// </summary>
		/// <returns> hash code value of the present value or 0 if no value is present </returns>
		public override int HashCode()
		{
			return IsPresent ? Long.HashCode(Value) : 0;
		}

		/// <summary>
		/// {@inheritDoc}
		/// 
		/// Returns a non-empty string representation of this object suitable for
		/// debugging. The exact presentation format is unspecified and may vary
		/// between implementations and versions.
		/// 
		/// @implSpec If a value is present the result must include its string
		/// representation in the result. Empty and present instances must be
		/// unambiguously differentiable.
		/// </summary>
		/// <returns> the string representation of this instance </returns>
		public override String ToString()
		{
			return IsPresent ? string.Format("OptionalLong[{0}]", Value) : "OptionalLong.empty";
		}
	}

}
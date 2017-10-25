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
	/// A container object which may or may not contain a {@code int} value.
	/// If a value is present, {@code isPresent()} will return {@code true} and
	/// {@code getAsInt()} will return the value.
	/// 
	/// <para>Additional methods that depend on the presence or absence of a contained
	/// value are provided, such as <seealso cref="#orElse(int) orElse()"/>
	/// (return a default value if value not present) and
	/// <seealso cref="#ifPresent(java.util.function.IntConsumer) ifPresent()"/> (execute a block
	/// of code if the value is present).
	/// 
	/// </para>
	/// <para>This is a <a href="../lang/doc-files/ValueBased.html">value-based</a>
	/// class; use of identity-sensitive operations (including reference equality
	/// ({@code ==}), identity hash code, or synchronization) on instances of
	/// {@code OptionalInt} may have unpredictable results and should be avoided.
	/// 
	/// @since 1.8
	/// </para>
	/// </summary>
	public sealed class OptionalInt
	{
		/// <summary>
		/// Common instance for {@code empty()}.
		/// </summary>
		private static readonly OptionalInt EMPTY = new OptionalInt();

		/// <summary>
		/// If true then the value is present, otherwise indicates no value is present
		/// </summary>
		private readonly bool IsPresent;
		private readonly int Value;

		/// <summary>
		/// Construct an empty instance.
		/// 
		/// @implNote Generally only one empty instance, <seealso cref="OptionalInt#EMPTY"/>,
		/// should exist per VM.
		/// </summary>
		private OptionalInt()
		{
			this.IsPresent = false;
			this.Value = 0;
		}

		/// <summary>
		/// Returns an empty {@code OptionalInt} instance.  No value is present for this
		/// OptionalInt.
		/// 
		/// @apiNote Though it may be tempting to do so, avoid testing if an object
		/// is empty by comparing with {@code ==} against instances returned by
		/// {@code Option.empty()}. There is no guarantee that it is a singleton.
		/// Instead, use <seealso cref="#isPresent()"/>.
		/// </summary>
		///  <returns> an empty {@code OptionalInt} </returns>
		public static OptionalInt Empty()
		{
			return EMPTY;
		}

		/// <summary>
		/// Construct an instance with the value present.
		/// </summary>
		/// <param name="value"> the int value to be present </param>
		private OptionalInt(int value)
		{
			this.IsPresent = true;
			this.Value = value;
		}

		/// <summary>
		/// Return an {@code OptionalInt} with the specified value present.
		/// </summary>
		/// <param name="value"> the value to be present </param>
		/// <returns> an {@code OptionalInt} with the value present </returns>
		public static OptionalInt Of(int value)
		{
			return new OptionalInt(value);
		}

		/// <summary>
		/// If a value is present in this {@code OptionalInt}, returns the value,
		/// otherwise throws {@code NoSuchElementException}.
		/// </summary>
		/// <returns> the value held by this {@code OptionalInt} </returns>
		/// <exception cref="NoSuchElementException"> if there is no value present
		/// </exception>
		/// <seealso cref= OptionalInt#isPresent() </seealso>
		public int AsInt
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
		public void IfPresent(IntConsumer consumer)
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
		public int OrElse(int other)
		{
			return IsPresent ? Value : other;
		}

		/// <summary>
		/// Return the value if present, otherwise invoke {@code other} and return
		/// the result of that invocation.
		/// </summary>
		/// <param name="other"> a {@code IntSupplier} whose result is returned if no value
		/// is present </param>
		/// <returns> the value if present otherwise the result of {@code other.getAsInt()} </returns>
		/// <exception cref="NullPointerException"> if value is not present and {@code other} is
		/// null </exception>
		public int OrElseGet(IntSupplier other)
		{
			return IsPresent ? Value : other.AsInt;
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
//ORIGINAL LINE: public<X extends Throwable> int orElseThrow(java.util.function.Supplier<X> exceptionSupplier) throws X
		public int orElseThrow<X>(Supplier<X> exceptionSupplier) where X : Throwable
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
		/// Indicates whether some other object is "equal to" this OptionalInt. The
		/// other object is considered equal if:
		/// <ul>
		/// <li>it is also an {@code OptionalInt} and;
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

			if (!(obj is OptionalInt))
			{
				return false;
			}

			OptionalInt other = (OptionalInt) obj;
			return (IsPresent && other.IsPresent) ? Value == other.Value : IsPresent == other.IsPresent;
		}

		/// <summary>
		/// Returns the hash code value of the present value, if any, or 0 (zero) if
		/// no value is present.
		/// </summary>
		/// <returns> hash code value of the present value or 0 if no value is present </returns>
		public override int HashCode()
		{
			return IsPresent ? Integer.HashCode(Value) : 0;
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
			return IsPresent ? string.Format("OptionalInt[{0}]", Value) : "OptionalInt.empty";
		}
	}

}
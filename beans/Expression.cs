/*
 * Copyright (c) 2000, 2013, Oracle and/or its affiliates. All rights reserved.
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

namespace java.beans
{

	/// <summary>
	/// An <code>Expression</code> object represents a primitive expression
	/// in which a single method is applied to a target and a set of
	/// arguments to return a result - as in <code>"a.getFoo()"</code>.
	/// <para>
	/// In addition to the properties of the super class, the
	/// <code>Expression</code> object provides a <em>value</em> which
	/// is the object returned when this expression is evaluated.
	/// The return value is typically not provided by the caller and
	/// is instead computed by dynamically finding the method and invoking
	/// it when the first call to <code>getValue</code> is made.
	/// 
	/// </para>
	/// </summary>
	/// <seealso cref= #getValue </seealso>
	/// <seealso cref= #setValue
	/// 
	/// @since 1.4
	/// 
	/// @author Philip Milne </seealso>
	public class Expression : Statement
	{

		private static Object Unbound = new Object();

		private Object Value_Renamed = Unbound;

		/// <summary>
		/// Creates a new <seealso cref="Expression"/> object
		/// for the specified target object to invoke the method
		/// specified by the name and by the array of arguments.
		/// <para>
		/// The {@code target} and the {@code methodName} values should not be {@code null}.
		/// Otherwise an attempt to execute this {@code Expression}
		/// will result in a {@code NullPointerException}.
		/// If the {@code arguments} value is {@code null},
		/// an empty array is used as the value of the {@code arguments} property.
		/// 
		/// </para>
		/// </summary>
		/// <param name="target">  the target object of this expression </param>
		/// <param name="methodName">  the name of the method to invoke on the specified target </param>
		/// <param name="arguments">  the array of arguments to invoke the specified method
		/// </param>
		/// <seealso cref= #getValue </seealso>
//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @ConstructorProperties({"target", "methodName", "arguments"}) public Expression(Object target, String methodName, Object[] arguments)
		public Expression(Object target, String methodName, Object[] arguments) : base(target, methodName, arguments)
		{
		}

		/// <summary>
		/// Creates a new <seealso cref="Expression"/> object with the specified value
		/// for the specified target object to invoke the  method
		/// specified by the name and by the array of arguments.
		/// The {@code value} value is used as the value of the {@code value} property,
		/// so the <seealso cref="#getValue"/> method will return it
		/// without executing this {@code Expression}.
		/// <para>
		/// The {@code target} and the {@code methodName} values should not be {@code null}.
		/// Otherwise an attempt to execute this {@code Expression}
		/// will result in a {@code NullPointerException}.
		/// If the {@code arguments} value is {@code null},
		/// an empty array is used as the value of the {@code arguments} property.
		/// 
		/// </para>
		/// </summary>
		/// <param name="value">  the value of this expression </param>
		/// <param name="target">  the target object of this expression </param>
		/// <param name="methodName">  the name of the method to invoke on the specified target </param>
		/// <param name="arguments">  the array of arguments to invoke the specified method
		/// </param>
		/// <seealso cref= #setValue </seealso>
		public Expression(Object value, Object target, String methodName, Object[] arguments) : this(target, methodName, arguments)
		{
			Value = value;
		}

		/// <summary>
		/// {@inheritDoc}
		/// <para>
		/// If the invoked method completes normally,
		/// the value it returns is copied in the {@code value} property.
		/// Note that the {@code value} property is set to {@code null},
		/// if the return type of the underlying method is {@code void}.
		/// 
		/// </para>
		/// </summary>
		/// <exception cref="NullPointerException"> if the value of the {@code target} or
		///                              {@code methodName} property is {@code null} </exception>
		/// <exception cref="NoSuchMethodException"> if a matching method is not found </exception>
		/// <exception cref="SecurityException"> if a security manager exists and
		///                           it denies the method invocation </exception>
		/// <exception cref="Exception"> that is thrown by the invoked method
		/// </exception>
		/// <seealso cref= java.lang.reflect.Method
		/// @since 1.7 </seealso>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: @Override public void execute() throws Exception
		public override void Execute()
		{
			Value = Invoke();
		}

		/// <summary>
		/// If the value property of this instance is not already set,
		/// this method dynamically finds the method with the specified
		/// methodName on this target with these arguments and calls it.
		/// The result of the method invocation is first copied
		/// into the value property of this expression and then returned
		/// as the result of <code>getValue</code>. If the value property
		/// was already set, either by a call to <code>setValue</code>
		/// or a previous call to <code>getValue</code> then the value
		/// property is returned without either looking up or calling the method.
		/// <para>
		/// The value property of an <code>Expression</code> is set to
		/// a unique private (non-<code>null</code>) value by default and
		/// this value is used as an internal indication that the method
		/// has not yet been called. A return value of <code>null</code>
		/// replaces this default value in the same way that any other value
		/// would, ensuring that expressions are never evaluated more than once.
		/// </para>
		/// <para>
		/// See the <code>execute</code> method for details on how
		/// methods are chosen using the dynamic types of the target
		/// and arguments.
		/// 
		/// </para>
		/// </summary>
		/// <seealso cref= Statement#execute </seealso>
		/// <seealso cref= #setValue
		/// </seealso>
		/// <returns> The result of applying this method to these arguments. </returns>
		/// <exception cref="Exception"> if the method with the specified methodName
		/// throws an exception </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public Object getValue() throws Exception
		public virtual Object Value
		{
			get
			{
				if (Value_Renamed == Unbound)
				{
					Value = Invoke();
				}
				return Value_Renamed;
			}
			set
			{
				this.Value_Renamed = value;
			}
		}


		/*pp*/	 internal override String InstanceName(Object instance)
	 {
			return instance == Unbound ? "<unbound>" : base.InstanceName(instance);
	 }

		/// <summary>
		/// Prints the value of this expression using a Java-style syntax.
		/// </summary>
		public override String ToString()
		{
			return InstanceName(Value_Renamed) + "=" + base.ToString();
		}
	}

}
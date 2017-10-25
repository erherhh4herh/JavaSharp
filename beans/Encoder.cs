using System;
using System.Collections.Generic;

/*
 * Copyright (c) 2000, 2011, Oracle and/or its affiliates. All rights reserved.
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

	using PersistenceDelegateFinder = com.sun.beans.finder.PersistenceDelegateFinder;


	/// <summary>
	/// An <code>Encoder</code> is a class which can be used to create
	/// files or streams that encode the state of a collection of
	/// JavaBeans in terms of their public APIs. The <code>Encoder</code>,
	/// in conjunction with its persistence delegates, is responsible for
	/// breaking the object graph down into a series of <code>Statements</code>s
	/// and <code>Expression</code>s which can be used to create it.
	/// A subclass typically provides a syntax for these expressions
	/// using some human readable form - like Java source code or XML.
	/// 
	/// @since 1.4
	/// 
	/// @author Philip Milne
	/// </summary>

	public class Encoder
	{
		private readonly PersistenceDelegateFinder Finder = new PersistenceDelegateFinder();
		private IDictionary<Object, Expression> Bindings = new IdentityHashMap<Object, Expression>();
		private ExceptionListener ExceptionListener_Renamed;
		internal bool ExecuteStatements = true;
		private IDictionary<Object, Object> Attributes;

		/// <summary>
		/// Write the specified object to the output stream.
		/// The serialized form will denote a series of
		/// expressions, the combined effect of which will create
		/// an equivalent object when the input stream is read.
		/// By default, the object is assumed to be a <em>JavaBean</em>
		/// with a nullary constructor, whose state is defined by
		/// the matching pairs of "setter" and "getter" methods
		/// returned by the Introspector.
		/// </summary>
		/// <param name="o"> The object to be written to the stream.
		/// </param>
		/// <seealso cref= XMLDecoder#readObject </seealso>
		protected internal virtual void WriteObject(Object o)
		{
			if (o == this)
			{
				return;
			}
			PersistenceDelegate info = GetPersistenceDelegate(o == null ? null : o.GetType());
			info.WriteObject(o, this);
		}

		/// <summary>
		/// Sets the exception handler for this stream to <code>exceptionListener</code>.
		/// The exception handler is notified when this stream catches recoverable
		/// exceptions.
		/// </summary>
		/// <param name="exceptionListener"> The exception handler for this stream;
		///       if <code>null</code> the default exception listener will be used.
		/// </param>
		/// <seealso cref= #getExceptionListener </seealso>
		public virtual ExceptionListener ExceptionListener
		{
			set
			{
				this.ExceptionListener_Renamed = value;
			}
			get
			{
				return (ExceptionListener_Renamed != null) ? ExceptionListener_Renamed : Statement.defaultExceptionListener;
			}
		}


		internal virtual Object GetValue(Expression exp)
		{
			try
			{
				return (exp == null) ? null : exp.Value;
			}
			catch (Exception e)
			{
				ExceptionListener.ExceptionThrown(e);
				throw new RuntimeException("failed to evaluate: " + exp.ToString());
			}
		}

		/// <summary>
		/// Returns the persistence delegate for the given type.
		/// The persistence delegate is calculated by applying
		/// the following rules in order:
		/// <ol>
		/// <li>
		/// If a persistence delegate is associated with the given type
		/// by using the <seealso cref="#setPersistenceDelegate"/> method
		/// it is returned.
		/// <li>
		/// A persistence delegate is then looked up by the name
		/// composed of the the fully qualified name of the given type
		/// and the "PersistenceDelegate" postfix.
		/// For example, a persistence delegate for the {@code Bean} class
		/// should be named {@code BeanPersistenceDelegate}
		/// and located in the same package.
		/// <pre>
		/// public class Bean { ... }
		/// public class BeanPersistenceDelegate { ... }</pre>
		/// The instance of the {@code BeanPersistenceDelegate} class
		/// is returned for the {@code Bean} class.
		/// <li>
		/// If the type is {@code null},
		/// a shared internal persistence delegate is returned
		/// that encodes {@code null} value.
		/// <li>
		/// If the type is a {@code enum} declaration,
		/// a shared internal persistence delegate is returned
		/// that encodes constants of this enumeration
		/// by their names.
		/// <li>
		/// If the type is a primitive type or the corresponding wrapper,
		/// a shared internal persistence delegate is returned
		/// that encodes values of the given type.
		/// <li>
		/// If the type is an array,
		/// a shared internal persistence delegate is returned
		/// that encodes an array of the appropriate type and length,
		/// and each of its elements as if they are properties.
		/// <li>
		/// If the type is a proxy,
		/// a shared internal persistence delegate is returned
		/// that encodes a proxy instance by using
		/// the <seealso cref="java.lang.reflect.Proxy#newProxyInstance"/> method.
		/// <li>
		/// If the <seealso cref="BeanInfo"/> for this type has a <seealso cref="BeanDescriptor"/>
		/// which defined a "persistenceDelegate" attribute,
		/// the value of this named attribute is returned.
		/// <li>
		/// In all other cases the default persistence delegate is returned.
		/// The default persistence delegate assumes the type is a <em>JavaBean</em>,
		/// implying that it has a default constructor and that its state
		/// may be characterized by the matching pairs of "setter" and "getter"
		/// methods returned by the <seealso cref="Introspector"/> class.
		/// The default constructor is the constructor with the greatest number
		/// of parameters that has the <seealso cref="ConstructorProperties"/> annotation.
		/// If none of the constructors has the {@code ConstructorProperties} annotation,
		/// then the nullary constructor (constructor with no parameters) will be used.
		/// For example, in the following code fragment, the nullary constructor
		/// for the {@code Foo} class will be used,
		/// while the two-parameter constructor
		/// for the {@code Bar} class will be used.
		/// <pre>
		/// public class Foo {
		///     public Foo() { ... }
		///     public Foo(int x) { ... }
		/// }
		/// public class Bar {
		///     public Bar() { ... }
		///     &#64;ConstructorProperties({"x"})
		///     public Bar(int x) { ... }
		///     &#64;ConstructorProperties({"x", "y"})
		///     public Bar(int x, int y) { ... }
		/// }</pre>
		/// </ol>
		/// </summary>
		/// <param name="type">  the class of the objects </param>
		/// <returns> the persistence delegate for the given type
		/// </returns>
		/// <seealso cref= #setPersistenceDelegate </seealso>
		/// <seealso cref= java.beans.Introspector#getBeanInfo </seealso>
		/// <seealso cref= java.beans.BeanInfo#getBeanDescriptor </seealso>
		public virtual PersistenceDelegate GetPersistenceDelegate(Class type)
		{
			PersistenceDelegate pd = this.Finder.find(type);
			if (pd == null)
			{
				pd = MetaData.GetPersistenceDelegate(type);
				if (pd != null)
				{
					this.Finder.register(type, pd);
				}
			}
			return pd;
		}

		/// <summary>
		/// Associates the specified persistence delegate with the given type.
		/// </summary>
		/// <param name="type">  the class of objects that the specified persistence delegate applies to </param>
		/// <param name="delegate">  the persistence delegate for instances of the given type
		/// </param>
		/// <seealso cref= #getPersistenceDelegate </seealso>
		/// <seealso cref= java.beans.Introspector#getBeanInfo </seealso>
		/// <seealso cref= java.beans.BeanInfo#getBeanDescriptor </seealso>
		public virtual void SetPersistenceDelegate(Class type, PersistenceDelegate @delegate)
		{
			this.Finder.register(type, @delegate);
		}

		/// <summary>
		/// Removes the entry for this instance, returning the old entry.
		/// </summary>
		/// <param name="oldInstance"> The entry that should be removed. </param>
		/// <returns> The entry that was removed.
		/// </returns>
		/// <seealso cref= #get </seealso>
		public virtual Object Remove(Object oldInstance)
		{
			Expression exp = Bindings.Remove(oldInstance);
			return GetValue(exp);
		}

		/// <summary>
		/// Returns a tentative value for <code>oldInstance</code> in
		/// the environment created by this stream. A persistence
		/// delegate can use its <code>mutatesTo</code> method to
		/// determine whether this value may be initialized to
		/// form the equivalent object at the output or whether
		/// a new object must be instantiated afresh. If the
		/// stream has not yet seen this value, null is returned.
		/// </summary>
		/// <param name="oldInstance"> The instance to be looked up. </param>
		/// <returns> The object, null if the object has not been seen before. </returns>
		public virtual Object Get(Object oldInstance)
		{
			if (oldInstance == null || oldInstance == this || oldInstance.GetType() == typeof(String))
			{
				return oldInstance;
			}
			Expression exp = Bindings[oldInstance];
			return GetValue(exp);
		}

		private Object WriteObject1(Object oldInstance)
		{
			Object o = Get(oldInstance);
			if (o == null)
			{
				WriteObject(oldInstance);
				o = Get(oldInstance);
			}
			return o;
		}

		private Statement CloneStatement(Statement oldExp)
		{
			Object oldTarget = oldExp.Target;
			Object newTarget = WriteObject1(oldTarget);

			Object[] oldArgs = oldExp.Arguments;
			Object[] newArgs = new Object[oldArgs.Length];
			for (int i = 0; i < oldArgs.Length; i++)
			{
				newArgs[i] = WriteObject1(oldArgs[i]);
			}
			Statement newExp = typeof(Statement).Equals(oldExp.GetType()) ? new Statement(newTarget, oldExp.MethodName, newArgs) : new Expression(newTarget, oldExp.MethodName, newArgs);
			newExp.Loader = oldExp.Loader;
			return newExp;
		}

		/// <summary>
		/// Writes statement <code>oldStm</code> to the stream.
		/// The <code>oldStm</code> should be written entirely
		/// in terms of the callers environment, i.e. the
		/// target and all arguments should be part of the
		/// object graph being written. These expressions
		/// represent a series of "what happened" expressions
		/// which tell the output stream how to produce an
		/// object graph like the original.
		/// <para>
		/// The implementation of this method will produce
		/// a second expression to represent the same expression in
		/// an environment that will exist when the stream is read.
		/// This is achieved simply by calling <code>writeObject</code>
		/// on the target and all the arguments and building a new
		/// expression with the results.
		/// 
		/// </para>
		/// </summary>
		/// <param name="oldStm"> The expression to be written to the stream. </param>
		public virtual void WriteStatement(Statement oldStm)
		{
			// System.out.println("writeStatement: " + oldExp);
			Statement newStm = CloneStatement(oldStm);
			if (oldStm.Target != this && ExecuteStatements)
			{
				try
				{
					newStm.Execute();
				}
				catch (Exception e)
				{
					ExceptionListener.ExceptionThrown(new Exception("Encoder: discarding statement " + newStm, e));
				}
			}
		}

		/// <summary>
		/// The implementation first checks to see if an
		/// expression with this value has already been written.
		/// If not, the expression is cloned, using
		/// the same procedure as <code>writeStatement</code>,
		/// and the value of this expression is reconciled
		/// with the value of the cloned expression
		/// by calling <code>writeObject</code>.
		/// </summary>
		/// <param name="oldExp"> The expression to be written to the stream. </param>
		public virtual void WriteExpression(Expression oldExp)
		{
			// System.out.println("Encoder::writeExpression: " + oldExp);
			Object oldValue = GetValue(oldExp);
			if (Get(oldValue) != null)
			{
				return;
			}
			Bindings[oldValue] = (Expression)CloneStatement(oldExp);
			WriteObject(oldValue);
		}

		internal virtual void Clear()
		{
			Bindings.Clear();
		}

		// Package private method for setting an attributes table for the encoder
		internal virtual void SetAttribute(Object key, Object value)
		{
			if (Attributes == null)
			{
				Attributes = new Dictionary<>();
			}
			Attributes[key] = value;
		}

		internal virtual Object GetAttribute(Object key)
		{
			if (Attributes == null)
			{
				return null;
			}
			return Attributes[key];
		}
	}

}
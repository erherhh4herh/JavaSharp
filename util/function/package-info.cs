/*
 * Copyright (c) 2011, 2013, Oracle and/or its affiliates. All rights reserved.
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

/// <summary>
/// <em>Functional interfaces</em> provide target types for lambda expressions
/// and method references.  Each functional interface has a single abstract
/// method, called the <em>functional method</em> for that functional interface,
/// to which the lambda expression's parameter and return types are matched or
/// adapted.  Functional interfaces can provide a target type in multiple
/// contexts, such as assignment context, method invocation, or cast context:
/// 
/// <pre>{@code
///     // Assignment context
///     Predicate<String> p = String::isEmpty;
/// 
///     // Method invocation context
///     stream.filter(e -> e.getSize() > 10)...
/// 
///     // Cast context
///     stream.map((ToIntFunction) e -> e.getSize())...
/// }</pre>
/// 
/// <para>The interfaces in this package are general purpose functional interfaces
/// used by the JDK, and are available to be used by user code as well.  While
/// they do not identify a complete set of function shapes to which lambda
/// expressions might be adapted, they provide enough to cover common
/// requirements. Other functional interfaces provided for specific purposes,
/// such as <seealso cref="java.io.FileFilter"/>, are defined in the packages where they
/// are used.
/// 
/// </para>
/// <para>The interfaces in this package are annotated with
/// <seealso cref="java.lang.FunctionalInterface"/>. This annotation is not a requirement
/// for the compiler to recognize an interface as a functional interface, but
/// merely an aid to capture design intent and enlist the help of the compiler in
/// identifying accidental violations of design intent.
/// 
/// </para>
/// <para>Functional interfaces often represent abstract concepts like functions,
/// actions, or predicates.  In documenting functional interfaces, or referring
/// to variables typed as functional interfaces, it is common to refer directly
/// to those abstract concepts, for example using "this function" instead of
/// "the function represented by this object".  When an API method is said to
/// accept or return a functional interface in this manner, such as "applies the
/// provided function to...", this is understood to mean a <i>non-null</i>
/// reference to an object implementing the appropriate functional interface,
/// unless potential nullity is explicitly specified.
/// 
/// </para>
/// <para>The functional interfaces in this package follow an extensible naming
/// convention, as follows:
/// 
/// <ul>
///     <li>There are several basic function shapes, including
///     <seealso cref="java.util.function.Function"/> (unary function from {@code T} to {@code R}),
///     <seealso cref="java.util.function.Consumer"/> (unary function from {@code T} to {@code void}),
///     <seealso cref="java.util.function.Predicate"/> (unary function from {@code T} to {@code boolean}),
///     and <seealso cref="java.util.function.Supplier"/> (nilary function to {@code R}).
///     </li>
/// 
///     <li>Function shapes have a natural arity based on how they are most
///     commonly used.  The basic shapes can be modified by an arity prefix to
///     indicate a different arity, such as
///     <seealso cref="java.util.function.BiFunction"/> (binary function from {@code T} and
///     {@code U} to {@code R}).
///     </li>
/// 
///     <li>There are additional derived function shapes which extend the basic
///     function shapes, including <seealso cref="java.util.function.UnaryOperator"/>
///     (extends {@code Function}) and <seealso cref="java.util.function.BinaryOperator"/>
///     (extends {@code BiFunction}).
///     </li>
/// 
///     <li>Type parameters of functional interfaces can be specialized to
///     primitives with additional type prefixes.  To specialize the return type
///     for a type that has both generic return type and generic arguments, we
///     prefix {@code ToXxx}, as in <seealso cref="java.util.function.ToIntFunction"/>.
///     Otherwise, type arguments are specialized left-to-right, as in
///     <seealso cref="java.util.function.DoubleConsumer"/>
///     or <seealso cref="java.util.function.ObjIntConsumer"/>.
///     (The type prefix {@code Obj} is used to indicate that we don't want to
///     specialize this parameter, but want to move on to the next parameter,
///     as in <seealso cref="java.util.function.ObjIntConsumer"/>.)
///     These schemes can be combined, as in {@code IntToDoubleFunction}.
///     </li>
/// 
///     <li>If there are specialization prefixes for all arguments, the arity
///     prefix may be left out (as in <seealso cref="java.util.function.ObjIntConsumer"/>).
///     </li>
/// </ul>
/// 
/// </para>
/// </summary>
/// <seealso cref= java.lang.FunctionalInterface
/// @since 1.8 </seealso>
namespace java.util.function
{

}
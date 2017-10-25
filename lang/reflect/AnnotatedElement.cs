/*
 * Copyright (c) 2003, 2013, Oracle and/or its affiliates. All rights reserved.
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

namespace java.lang.reflect
{

	using AnnotationSupport = sun.reflect.annotation.AnnotationSupport;
	using AnnotationType = sun.reflect.annotation.AnnotationType;

	/// <summary>
	/// Represents an annotated element of the program currently running in this
	/// VM.  This interface allows annotations to be read reflectively.  All
	/// annotations returned by methods in this interface are immutable and
	/// serializable. The arrays returned by methods of this interface may be modified
	/// by callers without affecting the arrays returned to other callers.
	/// 
	/// <para>The <seealso cref="#getAnnotationsByType(Class)"/> and {@link
	/// #getDeclaredAnnotationsByType(Class)} methods support multiple
	/// annotations of the same type on an element. If the argument to
	/// either method is a repeatable annotation type (JLS 9.6), then the
	/// method will "look through" a container annotation (JLS 9.7), if
	/// present, and return any annotations inside the container. Container
	/// annotations may be generated at compile-time to wrap multiple
	/// annotations of the argument type.
	/// 
	/// </para>
	/// <para>The terms <em>directly present</em>, <em>indirectly present</em>,
	/// <em>present</em>, and <em>associated</em> are used throughout this
	/// interface to describe precisely which annotations are returned by
	/// methods:
	/// 
	/// <ul>
	/// 
	/// <li> An annotation <i>A</i> is <em>directly present</em> on an
	/// element <i>E</i> if <i>E</i> has a {@code
	/// RuntimeVisibleAnnotations} or {@code
	/// RuntimeVisibleParameterAnnotations} or {@code
	/// RuntimeVisibleTypeAnnotations} attribute, and the attribute
	/// contains <i>A</i>.
	/// 
	/// <li>An annotation <i>A</i> is <em>indirectly present</em> on an
	/// element <i>E</i> if <i>E</i> has a {@code RuntimeVisibleAnnotations} or
	/// {@code RuntimeVisibleParameterAnnotations} or {@code RuntimeVisibleTypeAnnotations}
	/// attribute, and <i>A</i> 's type is repeatable, and the attribute contains
	/// exactly one annotation whose value element contains <i>A</i> and whose
	/// type is the containing annotation type of <i>A</i> 's type.
	/// 
	/// <li>An annotation <i>A</i> is present on an element <i>E</i> if either:
	/// 
	/// <ul>
	/// 
	/// <li><i>A</i> is directly present on <i>E</i>; or
	/// 
	/// <li>No annotation of <i>A</i> 's type is directly present on
	/// <i>E</i>, and <i>E</i> is a class, and <i>A</i> 's type is
	/// inheritable, and <i>A</i> is present on the superclass of <i>E</i>.
	/// 
	/// </ul>
	/// 
	/// <li>An annotation <i>A</i> is <em>associated</em> with an element <i>E</i>
	/// if either:
	/// 
	/// <ul>
	/// 
	/// <li><i>A</i> is directly or indirectly present on <i>E</i>; or
	/// 
	/// <li>No annotation of <i>A</i> 's type is directly or indirectly
	/// present on <i>E</i>, and <i>E</i> is a class, and <i>A</i>'s type
	/// is inheritable, and <i>A</i> is associated with the superclass of
	/// <i>E</i>.
	/// 
	/// </ul>
	/// 
	/// </ul>
	/// 
	/// </para>
	/// <para>The table below summarizes which kind of annotation presence
	/// different methods in this interface examine.
	/// 
	/// <table border>
	/// <caption>Overview of kind of presence detected by different AnnotatedElement methods</caption>
	/// <tr><th colspan=2></th><th colspan=4>Kind of Presence</th>
	/// <tr><th colspan=2>Method</th><th>Directly Present</th><th>Indirectly Present</th><th>Present</th><th>Associated</th>
	/// <tr><td align=right>{@code T}</td><td><seealso cref="#getAnnotation(Class) getAnnotation(Class&lt;T&gt;)"/>
	/// <td></td><td></td><td>X</td><td></td>
	/// </tr>
	/// <tr><td align=right>{@code Annotation[]}</td><td><seealso cref="#getAnnotations getAnnotations()"/>
	/// <td></td><td></td><td>X</td><td></td>
	/// </tr>
	/// <tr><td align=right>{@code T[]}</td><td><seealso cref="#getAnnotationsByType(Class) getAnnotationsByType(Class&lt;T&gt;)"/>
	/// <td></td><td></td><td></td><td>X</td>
	/// </tr>
	/// <tr><td align=right>{@code T}</td><td><seealso cref="#getDeclaredAnnotation(Class) getDeclaredAnnotation(Class&lt;T&gt;)"/>
	/// <td>X</td><td></td><td></td><td></td>
	/// </tr>
	/// <tr><td align=right>{@code Annotation[]}</td><td><seealso cref="#getDeclaredAnnotations getDeclaredAnnotations()"/>
	/// <td>X</td><td></td><td></td><td></td>
	/// </tr>
	/// <tr><td align=right>{@code T[]}</td><td><seealso cref="#getDeclaredAnnotationsByType(Class) getDeclaredAnnotationsByType(Class&lt;T&gt;)"/>
	/// <td>X</td><td>X</td><td></td><td></td>
	/// </tr>
	/// </table>
	/// 
	/// </para>
	/// <para>For an invocation of {@code get[Declared]AnnotationsByType( Class <
	/// T >)}, the order of annotations which are directly or indirectly
	/// present on an element <i>E</i> is computed as if indirectly present
	/// annotations on <i>E</i> are directly present on <i>E</i> in place
	/// of their container annotation, in the order in which they appear in
	/// the value element of the container annotation.
	/// 
	/// </para>
	/// <para>There are several compatibility concerns to keep in mind if an
	/// annotation type <i>T</i> is originally <em>not</em> repeatable and
	/// later modified to be repeatable.
	/// 
	/// The containing annotation type for <i>T</i> is <i>TC</i>.
	/// 
	/// <ul>
	/// 
	/// <li>Modifying <i>T</i> to be repeatable is source and binary
	/// compatible with existing uses of <i>T</i> and with existing uses
	/// of <i>TC</i>.
	/// 
	/// That is, for source compatibility, source code with annotations of
	/// type <i>T</i> or of type <i>TC</i> will still compile. For binary
	/// compatibility, class files with annotations of type <i>T</i> or of
	/// type <i>TC</i> (or with other kinds of uses of type <i>T</i> or of
	/// type <i>TC</i>) will link against the modified version of <i>T</i>
	/// if they linked against the earlier version.
	/// 
	/// (An annotation type <i>TC</i> may informally serve as an acting
	/// containing annotation type before <i>T</i> is modified to be
	/// formally repeatable. Alternatively, when <i>T</i> is made
	/// repeatable, <i>TC</i> can be introduced as a new type.)
	/// 
	/// <li>If an annotation type <i>TC</i> is present on an element, and
	/// <i>T</i> is modified to be repeatable with <i>TC</i> as its
	/// containing annotation type then:
	/// 
	/// <ul>
	/// 
	/// <li>The change to <i>T</i> is behaviorally compatible with respect
	/// to the {@code get[Declared]Annotation(Class<T>)} (called with an
	/// argument of <i>T</i> or <i>TC</i>) and {@code
	/// get[Declared]Annotations()} methods because the results of the
	/// methods will not change due to <i>TC</i> becoming the containing
	/// annotation type for <i>T</i>.
	/// 
	/// <li>The change to <i>T</i> changes the results of the {@code
	/// get[Declared]AnnotationsByType(Class<T>)} methods called with an
	/// argument of <i>T</i>, because those methods will now recognize an
	/// annotation of type <i>TC</i> as a container annotation for <i>T</i>
	/// and will "look through" it to expose annotations of type <i>T</i>.
	/// 
	/// </ul>
	/// 
	/// <li>If an annotation of type <i>T</i> is present on an
	/// element and <i>T</i> is made repeatable and more annotations of
	/// type <i>T</i> are added to the element:
	/// 
	/// <ul>
	/// 
	/// <li> The addition of the annotations of type <i>T</i> is both
	/// source compatible and binary compatible.
	/// 
	/// <li>The addition of the annotations of type <i>T</i> changes the results
	/// of the {@code get[Declared]Annotation(Class<T>)} methods and {@code
	/// get[Declared]Annotations()} methods, because those methods will now
	/// only see a container annotation on the element and not see an
	/// annotation of type <i>T</i>.
	/// 
	/// <li>The addition of the annotations of type <i>T</i> changes the
	/// results of the {@code get[Declared]AnnotationsByType(Class<T>)}
	/// methods, because their results will expose the additional
	/// annotations of type <i>T</i> whereas previously they exposed only a
	/// single annotation of type <i>T</i>.
	/// 
	/// </ul>
	/// 
	/// </ul>
	/// 
	/// </para>
	/// <para>If an annotation returned by a method in this interface contains
	/// (directly or indirectly) a <seealso cref="Class"/>-valued member referring to
	/// a class that is not accessible in this VM, attempting to read the class
	/// by calling the relevant Class-returning method on the returned annotation
	/// will result in a <seealso cref="TypeNotPresentException"/>.
	/// 
	/// </para>
	/// <para>Similarly, attempting to read an enum-valued member will result in
	/// a <seealso cref="EnumConstantNotPresentException"/> if the enum constant in the
	/// annotation is no longer present in the enum type.
	/// 
	/// </para>
	/// <para>If an annotation type <i>T</i> is (meta-)annotated with an
	/// {@code @Repeatable} annotation whose value element indicates a type
	/// <i>TC</i>, but <i>TC</i> does not declare a {@code value()} method
	/// with a return type of <i>T</i>{@code []}, then an exception of type
	/// <seealso cref="java.lang.annotation.AnnotationFormatError"/> is thrown.
	/// 
	/// </para>
	/// <para>Finally, attempting to read a member whose definition has evolved
	/// incompatibly will result in a {@link
	/// java.lang.annotation.AnnotationTypeMismatchException} or an
	/// <seealso cref="java.lang.annotation.IncompleteAnnotationException"/>.
	/// 
	/// </para>
	/// </summary>
	/// <seealso cref= java.lang.EnumConstantNotPresentException </seealso>
	/// <seealso cref= java.lang.TypeNotPresentException </seealso>
	/// <seealso cref= AnnotationFormatError </seealso>
	/// <seealso cref= java.lang.annotation.AnnotationTypeMismatchException </seealso>
	/// <seealso cref= java.lang.annotation.IncompleteAnnotationException
	/// @since 1.5
	/// @author Josh Bloch </seealso>
	public interface AnnotatedElement
	{
		/// <summary>
		/// Returns true if an annotation for the specified type
		/// is <em>present</em> on this element, else false.  This method
		/// is designed primarily for convenient access to marker annotations.
		/// 
		/// <para>The truth value returned by this method is equivalent to:
		/// {@code getAnnotation(annotationClass) != null}
		/// 
		/// </para>
		/// <para>The body of the default method is specified to be the code
		/// above.
		/// 
		/// </para>
		/// </summary>
		/// <param name="annotationClass"> the Class object corresponding to the
		///        annotation type </param>
		/// <returns> true if an annotation for the specified annotation
		///     type is present on this element, else false </returns>
		/// <exception cref="NullPointerException"> if the given annotation class is null
		/// @since 1.5 </exception>
//JAVA TO C# CONVERTER TODO TASK: There is no equivalent in C# to Java default interface methods:
//		default boolean isAnnotationPresent(Class annotationClass)
	//	{
	//		return getAnnotation(annotationClass) != null;
	//	}

	   /// <summary>
	   /// Returns this element's annotation for the specified type if
	   /// such an annotation is <em>present</em>, else null.
	   /// </summary>
	   /// @param <T> the type of the annotation to query for and return if present </param>
	   /// <param name="annotationClass"> the Class object corresponding to the
	   ///        annotation type </param>
	   /// <returns> this element's annotation for the specified annotation type if
	   ///     present on this element, else null </returns>
	   /// <exception cref="NullPointerException"> if the given annotation class is null
	   /// @since 1.5 </exception>
		T getAnnotation<T>(Class annotationClass) where T : Annotation;

		/// <summary>
		/// Returns annotations that are <em>present</em> on this element.
		/// 
		/// If there are no annotations <em>present</em> on this element, the return
		/// value is an array of length 0.
		/// 
		/// The caller of this method is free to modify the returned array; it will
		/// have no effect on the arrays returned to other callers.
		/// </summary>
		/// <returns> annotations present on this element
		/// @since 1.5 </returns>
		Annotation[] Annotations {get;}

		/// <summary>
		/// Returns annotations that are <em>associated</em> with this element.
		/// 
		/// If there are no annotations <em>associated</em> with this element, the return
		/// value is an array of length 0.
		/// 
		/// The difference between this method and <seealso cref="#getAnnotation(Class)"/>
		/// is that this method detects if its argument is a <em>repeatable
		/// annotation type</em> (JLS 9.6), and if so, attempts to find one or
		/// more annotations of that type by "looking through" a container
		/// annotation.
		/// 
		/// The caller of this method is free to modify the returned array; it will
		/// have no effect on the arrays returned to other callers.
		/// 
		/// @implSpec The default implementation first calls {@link
		/// #getDeclaredAnnotationsByType(Class)} passing {@code
		/// annotationClass} as the argument. If the returned array has
		/// length greater than zero, the array is returned. If the returned
		/// array is zero-length and this {@code AnnotatedElement} is a
		/// class and the argument type is an inheritable annotation type,
		/// and the superclass of this {@code AnnotatedElement} is non-null,
		/// then the returned result is the result of calling {@link
		/// #getAnnotationsByType(Class)} on the superclass with {@code
		/// annotationClass} as the argument. Otherwise, a zero-length
		/// array is returned.
		/// </summary>
		/// @param <T> the type of the annotation to query for and return if present </param>
		/// <param name="annotationClass"> the Class object corresponding to the
		///        annotation type </param>
		/// <returns> all this element's annotations for the specified annotation type if
		///     associated with this element, else an array of length zero </returns>
		/// <exception cref="NullPointerException"> if the given annotation class is null
		/// @since 1.8 </exception>
//JAVA TO C# CONVERTER TODO TASK: There is no equivalent in C# to Java default interface methods:
//		default <T> T[] getAnnotationsByType(Class annotationClass) where T : Annotation
	//	{
	// /*
	//  * Definition of associated: directly or indirectly present OR
	//  * neither directly nor indirectly present AND the element is
	//  * a Class, the annotation type is inheritable, and the
	//  * annotation type is associated with the superclass of the
	//  * element.
	//  */
	//
	//		 if (result.length == 0 && this instanceof Class && AnnotationType.getInstance(annotationClass).isInherited()) // Inheritable -  the element is a class -  Neither directly nor indirectly present
	//		 {
	//			 if (superClass != null)
	//			 {
	//				 // Determine if the annotation is associated with the
	//				 // superclass
	//				 result = superClass.getAnnotationsByType(annotationClass);
	//			 }
	//		 }
	//	 }

		/// <summary>
		/// Returns this element's annotation for the specified type if
		/// such an annotation is <em>directly present</em>, else null.
		/// 
		/// This method ignores inherited annotations. (Returns null if no
		/// annotations are directly present on this element.)
		/// 
		/// @implSpec The default implementation first performs a null check
		/// and then loops over the results of {@link
		/// #getDeclaredAnnotations} returning the first annotation whose
		/// annotation type matches the argument type.
		/// </summary>
		/// @param <T> the type of the annotation to query for and return if directly present </param>
		/// <param name="annotationClass"> the Class object corresponding to the
		///        annotation type </param>
		/// <returns> this element's annotation for the specified annotation type if
		///     directly present on this element, else null </returns>
		/// <exception cref="NullPointerException"> if the given annotation class is null
		/// @since 1.8 </exception>
//JAVA TO C# CONVERTER TODO TASK: There is no equivalent in C# to Java default interface methods:
//		default <T> T getDeclaredAnnotation(Class annotationClass) where T : Annotation
	//	{
	//		 Objects.requireNonNull(annotationClass);
	//		 // Loop over all directly-present annotations looking for a matching one
	//		 for (Annotation annotation : getDeclaredAnnotations())
	//		 {
	//			 if (annotationClass.equals(annotation.annotationType()))
	//			 {
	//				 // More robust to do a dynamic cast at runtime instead
	//				 // of compile-time only.
	//				 return annotationClass.cast(annotation);
	//			 }
	//		 }
	//	 }

		/// <summary>
		/// Returns this element's annotation(s) for the specified type if
		/// such annotations are either <em>directly present</em> or
		/// <em>indirectly present</em>. This method ignores inherited
		/// annotations.
		/// 
		/// If there are no specified annotations directly or indirectly
		/// present on this element, the return value is an array of length
		/// 0.
		/// 
		/// The difference between this method and {@link
		/// #getDeclaredAnnotation(Class)} is that this method detects if its
		/// argument is a <em>repeatable annotation type</em> (JLS 9.6), and if so,
		/// attempts to find one or more annotations of that type by "looking
		/// through" a container annotation if one is present.
		/// 
		/// The caller of this method is free to modify the returned array; it will
		/// have no effect on the arrays returned to other callers.
		/// 
		/// @implSpec The default implementation may call {@link
		/// #getDeclaredAnnotation(Class)} one or more times to find a
		/// directly present annotation and, if the annotation type is
		/// repeatable, to find a container annotation. If annotations of
		/// the annotation type {@code annotationClass} are found to be both
		/// directly and indirectly present, then {@link
		/// #getDeclaredAnnotations()} will get called to determine the
		/// order of the elements in the returned array.
		/// 
		/// <para>Alternatively, the default implementation may call {@link
		/// #getDeclaredAnnotations()} a single time and the returned array
		/// examined for both directly and indirectly present
		/// annotations. The results of calling {@link
		/// #getDeclaredAnnotations()} are assumed to be consistent with the
		/// results of calling <seealso cref="#getDeclaredAnnotation(Class)"/>.
		/// 
		/// </para>
		/// </summary>
		/// @param <T> the type of the annotation to query for and return
		/// if directly or indirectly present </param>
		/// <param name="annotationClass"> the Class object corresponding to the
		///        annotation type </param>
		/// <returns> all this element's annotations for the specified annotation type if
		///     directly or indirectly present on this element, else an array of length zero </returns>
		/// <exception cref="NullPointerException"> if the given annotation class is null
		/// @since 1.8 </exception>
//JAVA TO C# CONVERTER TODO TASK: There is no equivalent in C# to Java default interface methods:
//		default <T> T[] getDeclaredAnnotationsByType(Class annotationClass) where T : Annotation
	//	{
	//		Objects.requireNonNull(annotationClass);
	//		return AnnotationSupport.getDirectlyAndIndirectlyPresent(Arrays.stream(getDeclaredAnnotations()).collect(Collectors.toMap(Annotation::annotationType, Function.identity(), ((first,second) -> first), LinkedHashMap::new)), annotationClass);
	//	}

		/// <summary>
		/// Returns annotations that are <em>directly present</em> on this element.
		/// This method ignores inherited annotations.
		/// 
		/// If there are no annotations <em>directly present</em> on this element,
		/// the return value is an array of length 0.
		/// 
		/// The caller of this method is free to modify the returned array; it will
		/// have no effect on the arrays returned to other callers.
		/// </summary>
		/// <returns> annotations directly present on this element
		/// @since 1.5 </returns>
		Annotation[] DeclaredAnnotations {get;}
	}

	public static class AnnotatedElement_Fields
	{
			 public static readonly T[] Result = getDeclaredAnnotationsByType(annotationClass);
				 public static readonly Class SuperClass = ((Class) this).BaseType;
			 public static readonly return Result;
			 public static readonly return Null;
	}

}
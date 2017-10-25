using System.Collections;
using System.Collections.Generic;
using System.Collections.Concurrent;

/*
 * Copyright (c) 2008, 2013, Oracle and/or its affiliates. All rights reserved.
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

namespace java.lang.invoke
{


	using ValueConversions = sun.invoke.util.ValueConversions;
	using VerifyAccess = sun.invoke.util.VerifyAccess;
	using Wrapper = sun.invoke.util.Wrapper;
	using CallerSensitive = sun.reflect.CallerSensitive;
	using Reflection = sun.reflect.Reflection;
	using ReflectUtil = sun.reflect.misc.ReflectUtil;
	using SecurityConstants = sun.security.util.SecurityConstants;

	/// <summary>
	/// This class consists exclusively of static methods that operate on or return
	/// method handles. They fall into several categories:
	/// <ul>
	/// <li>Lookup methods which help create method handles for methods and fields.
	/// <li>Combinator methods, which combine or transform pre-existing method handles into new ones.
	/// <li>Other factory methods to create method handles that emulate other common JVM operations or control flow patterns.
	/// </ul>
	/// <para>
	/// @author John Rose, JSR 292 EG
	/// @since 1.7
	/// </para>
	/// </summary>
	public class MethodHandles
	{

		private MethodHandles() // do not instantiate
		{
		}

		private static readonly MemberName.Factory IMPL_NAMES = MemberName.Factory;
		static MethodHandles()
		{
			MethodHandleImpl.InitStatics();
				IMPL_NAMES.GetType();
		}
		// See IMPL_LOOKUP below.

		//// Method handle creation from ordinary methods.

		/// <summary>
		/// Returns a <seealso cref="Lookup lookup object"/> with
		/// full capabilities to emulate all supported bytecode behaviors of the caller.
		/// These capabilities include <a href="MethodHandles.Lookup.html#privacc">private access</a> to the caller.
		/// Factory methods on the lookup object can create
		/// <a href="MethodHandleInfo.html#directmh">direct method handles</a>
		/// for any member that the caller has access to via bytecodes,
		/// including protected and private fields and methods.
		/// This lookup object is a <em>capability</em> which may be delegated to trusted agents.
		/// Do not store it in place where untrusted code can access it.
		/// <para>
		/// This method is caller sensitive, which means that it may return different
		/// values to different callers.
		/// </para>
		/// <para>
		/// For any given caller class {@code C}, the lookup object returned by this call
		/// has equivalent capabilities to any lookup object
		/// supplied by the JVM to the bootstrap method of an
		/// <a href="package-summary.html#indyinsn">invokedynamic instruction</a>
		/// executing in the same caller class {@code C}.
		/// </para>
		/// </summary>
		/// <returns> a lookup object for the caller of this method, with private access </returns>
//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @CallerSensitive public static Lookup lookup()
		public static Lookup Lookup()
		{
			return new Lookup(Reflection.CallerClass);
		}

		/// <summary>
		/// Returns a <seealso cref="Lookup lookup object"/> which is trusted minimally.
		/// It can only be used to create method handles to
		/// publicly accessible fields and methods.
		/// <para>
		/// As a matter of pure convention, the <seealso cref="Lookup#lookupClass lookup class"/>
		/// of this lookup object will be <seealso cref="java.lang.Object"/>.
		/// 
		/// <p style="font-size:smaller;">
		/// <em>Discussion:</em>
		/// The lookup class can be changed to any other class {@code C} using an expression of the form
		/// <seealso cref="Lookup#in publicLookup().in(C.class)"/>.
		/// Since all classes have equal access to public names,
		/// such a change would confer no new access rights.
		/// A public lookup object is always subject to
		/// <a href="MethodHandles.Lookup.html#secmgr">security manager checks</a>.
		/// Also, it cannot access
		/// <a href="MethodHandles.Lookup.html#callsens">caller sensitive methods</a>.
		/// </para>
		/// </summary>
		/// <returns> a lookup object which is trusted minimally </returns>
		public static Lookup PublicLookup()
		{
			return Lookup.PUBLIC_LOOKUP;
		}

		/// <summary>
		/// Performs an unchecked "crack" of a
		/// <a href="MethodHandleInfo.html#directmh">direct method handle</a>.
		/// The result is as if the user had obtained a lookup object capable enough
		/// to crack the target method handle, called
		/// <seealso cref="java.lang.invoke.MethodHandles.Lookup#revealDirect Lookup.revealDirect"/>
		/// on the target to obtain its symbolic reference, and then called
		/// <seealso cref="java.lang.invoke.MethodHandleInfo#reflectAs MethodHandleInfo.reflectAs"/>
		/// to resolve the symbolic reference to a member.
		/// <para>
		/// If there is a security manager, its {@code checkPermission} method
		/// is called with a {@code ReflectPermission("suppressAccessChecks")} permission.
		/// </para>
		/// </summary>
		/// @param <T> the desired type of the result, either <seealso cref="Member"/> or a subtype </param>
		/// <param name="target"> a direct method handle to crack into symbolic reference components </param>
		/// <param name="expected"> a class object representing the desired result type {@code T} </param>
		/// <returns> a reference to the method, constructor, or field object </returns>
		/// <exception cref="SecurityException"> if the caller is not privileged to call {@code setAccessible} </exception>
		/// <exception cref="NullPointerException"> if either argument is {@code null} </exception>
		/// <exception cref="IllegalArgumentException"> if the target is not a direct method handle </exception>
		/// <exception cref="ClassCastException"> if the member is not of the expected type
		/// @since 1.8 </exception>
		public static T reflectAs<T>(Class expected, MethodHandle target) where T : Member
		{
			SecurityManager smgr = System.SecurityManager;
			if (smgr != null)
			{
				smgr.CheckPermission(ACCESS_PERMISSION);
			}
			Lookup lookup = Lookup.IMPL_LOOKUP; // use maximally privileged lookup
			return lookup.RevealDirect(target).ReflectAs(expected, lookup);
		}
		// Copied from AccessibleObject, as used by Method.setAccessible, etc.:
		private static readonly java.security.Permission ACCESS_PERMISSION = new ReflectPermission("suppressAccessChecks");

		/// <summary>
		/// A <em>lookup object</em> is a factory for creating method handles,
		/// when the creation requires access checking.
		/// Method handles do not perform
		/// access checks when they are called, but rather when they are created.
		/// Therefore, method handle access
		/// restrictions must be enforced when a method handle is created.
		/// The caller class against which those restrictions are enforced
		/// is known as the <seealso cref="#lookupClass lookup class"/>.
		/// <para>
		/// A lookup class which needs to create method handles will call
		/// <seealso cref="MethodHandles#lookup MethodHandles.lookup"/> to create a factory for itself.
		/// When the {@code Lookup} factory object is created, the identity of the lookup class is
		/// determined, and securely stored in the {@code Lookup} object.
		/// The lookup class (or its delegates) may then use factory methods
		/// on the {@code Lookup} object to create method handles for access-checked members.
		/// This includes all methods, constructors, and fields which are allowed to the lookup class,
		/// even private ones.
		/// 
		/// <h1><a name="lookups"></a>Lookup Factory Methods</h1>
		/// The factory methods on a {@code Lookup} object correspond to all major
		/// use cases for methods, constructors, and fields.
		/// Each method handle created by a factory method is the functional
		/// equivalent of a particular <em>bytecode behavior</em>.
		/// (Bytecode behaviors are described in section 5.4.3.5 of the Java Virtual Machine Specification.)
		/// Here is a summary of the correspondence between these factory methods and
		/// the behavior the resulting method handles:
		/// <table border=1 cellpadding=5 summary="lookup method behaviors">
		/// <tr>
		///     <th><a name="equiv"></a>lookup expression</th>
		///     <th>member</th>
		///     <th>bytecode behavior</th>
		/// </tr>
		/// <tr>
		///     <td><seealso cref="java.lang.invoke.MethodHandles.Lookup#findGetter lookup.findGetter(C.class,"f",FT.class)"/></td>
		///     <td>{@code FT f;}</td><td>{@code (T) this.f;}</td>
		/// </tr>
		/// <tr>
		///     <td><seealso cref="java.lang.invoke.MethodHandles.Lookup#findStaticGetter lookup.findStaticGetter(C.class,"f",FT.class)"/></td>
		///     <td>{@code static}<br>{@code FT f;}</td><td>{@code (T) C.f;}</td>
		/// </tr>
		/// <tr>
		///     <td><seealso cref="java.lang.invoke.MethodHandles.Lookup#findSetter lookup.findSetter(C.class,"f",FT.class)"/></td>
		///     <td>{@code FT f;}</td><td>{@code this.f = x;}</td>
		/// </tr>
		/// <tr>
		///     <td><seealso cref="java.lang.invoke.MethodHandles.Lookup#findStaticSetter lookup.findStaticSetter(C.class,"f",FT.class)"/></td>
		///     <td>{@code static}<br>{@code FT f;}</td><td>{@code C.f = arg;}</td>
		/// </tr>
		/// <tr>
		///     <td><seealso cref="java.lang.invoke.MethodHandles.Lookup#findVirtual lookup.findVirtual(C.class,"m",MT)"/></td>
		///     <td>{@code T m(A*);}</td><td>{@code (T) this.m(arg*);}</td>
		/// </tr>
		/// <tr>
		///     <td><seealso cref="java.lang.invoke.MethodHandles.Lookup#findStatic lookup.findStatic(C.class,"m",MT)"/></td>
		///     <td>{@code static}<br>{@code T m(A*);}</td><td>{@code (T) C.m(arg*);}</td>
		/// </tr>
		/// <tr>
		///     <td><seealso cref="java.lang.invoke.MethodHandles.Lookup#findSpecial lookup.findSpecial(C.class,"m",MT,this.class)"/></td>
		///     <td>{@code T m(A*);}</td><td>{@code (T) super.m(arg*);}</td>
		/// </tr>
		/// <tr>
		///     <td><seealso cref="java.lang.invoke.MethodHandles.Lookup#findConstructor lookup.findConstructor(C.class,MT)"/></td>
		///     <td>{@code C(A*);}</td><td>{@code new C(arg*);}</td>
		/// </tr>
		/// <tr>
		///     <td><seealso cref="java.lang.invoke.MethodHandles.Lookup#unreflectGetter lookup.unreflectGetter(aField)"/></td>
		///     <td>({@code static})?<br>{@code FT f;}</td><td>{@code (FT) aField.get(thisOrNull);}</td>
		/// </tr>
		/// <tr>
		///     <td><seealso cref="java.lang.invoke.MethodHandles.Lookup#unreflectSetter lookup.unreflectSetter(aField)"/></td>
		///     <td>({@code static})?<br>{@code FT f;}</td><td>{@code aField.set(thisOrNull, arg);}</td>
		/// </tr>
		/// <tr>
		///     <td><seealso cref="java.lang.invoke.MethodHandles.Lookup#unreflect lookup.unreflect(aMethod)"/></td>
		///     <td>({@code static})?<br>{@code T m(A*);}</td><td>{@code (T) aMethod.invoke(thisOrNull, arg*);}</td>
		/// </tr>
		/// <tr>
		///     <td><seealso cref="java.lang.invoke.MethodHandles.Lookup#unreflectConstructor lookup.unreflectConstructor(aConstructor)"/></td>
		///     <td>{@code C(A*);}</td><td>{@code (C) aConstructor.newInstance(arg*);}</td>
		/// </tr>
		/// <tr>
		///     <td><seealso cref="java.lang.invoke.MethodHandles.Lookup#unreflect lookup.unreflect(aMethod)"/></td>
		///     <td>({@code static})?<br>{@code T m(A*);}</td><td>{@code (T) aMethod.invoke(thisOrNull, arg*);}</td>
		/// </tr>
		/// </table>
		/// 
		/// Here, the type {@code C} is the class or interface being searched for a member,
		/// documented as a parameter named {@code refc} in the lookup methods.
		/// The method type {@code MT} is composed from the return type {@code T}
		/// and the sequence of argument types {@code A*}.
		/// The constructor also has a sequence of argument types {@code A*} and
		/// is deemed to return the newly-created object of type {@code C}.
		/// Both {@code MT} and the field type {@code FT} are documented as a parameter named {@code type}.
		/// The formal parameter {@code this} stands for the self-reference of type {@code C};
		/// if it is present, it is always the leading argument to the method handle invocation.
		/// (In the case of some {@code protected} members, {@code this} may be
		/// restricted in type to the lookup class; see below.)
		/// The name {@code arg} stands for all the other method handle arguments.
		/// In the code examples for the Core Reflection API, the name {@code thisOrNull}
		/// stands for a null reference if the accessed method or field is static,
		/// and {@code this} otherwise.
		/// The names {@code aMethod}, {@code aField}, and {@code aConstructor} stand
		/// for reflective objects corresponding to the given members.
		/// </para>
		/// <para>
		/// In cases where the given member is of variable arity (i.e., a method or constructor)
		/// the returned method handle will also be of <seealso cref="MethodHandle#asVarargsCollector variable arity"/>.
		/// In all other cases, the returned method handle will be of fixed arity.
		/// <p style="font-size:smaller;">
		/// <em>Discussion:</em>
		/// The equivalence between looked-up method handles and underlying
		/// class members and bytecode behaviors
		/// can break down in a few ways:
		/// <ul style="font-size:smaller;">
		/// <li>If {@code C} is not symbolically accessible from the lookup class's loader,
		/// the lookup can still succeed, even when there is no equivalent
		/// Java expression or bytecoded constant.
		/// <li>Likewise, if {@code T} or {@code MT}
		/// is not symbolically accessible from the lookup class's loader,
		/// the lookup can still succeed.
		/// For example, lookups for {@code MethodHandle.invokeExact} and
		/// {@code MethodHandle.invoke} will always succeed, regardless of requested type.
		/// <li>If there is a security manager installed, it can forbid the lookup
		/// on various grounds (<a href="MethodHandles.Lookup.html#secmgr">see below</a>).
		/// By contrast, the {@code ldc} instruction on a {@code CONSTANT_MethodHandle}
		/// constant is not subject to security manager checks.
		/// <li>If the looked-up method has a
		/// <a href="MethodHandle.html#maxarity">very large arity</a>,
		/// the method handle creation may fail, due to the method handle
		/// type having too many parameters.
		/// </ul>
		/// 
		/// <h1><a name="access"></a>Access checking</h1>
		/// Access checks are applied in the factory methods of {@code Lookup},
		/// when a method handle is created.
		/// This is a key difference from the Core Reflection API, since
		/// <seealso cref="java.lang.reflect.Method#invoke java.lang.reflect.Method.invoke"/>
		/// performs access checking against every caller, on every call.
		/// </para>
		/// <para>
		/// All access checks start from a {@code Lookup} object, which
		/// compares its recorded lookup class against all requests to
		/// create method handles.
		/// A single {@code Lookup} object can be used to create any number
		/// of access-checked method handles, all checked against a single
		/// lookup class.
		/// </para>
		/// <para>
		/// A {@code Lookup} object can be shared with other trusted code,
		/// such as a metaobject protocol.
		/// A shared {@code Lookup} object delegates the capability
		/// to create method handles on private members of the lookup class.
		/// Even if privileged code uses the {@code Lookup} object,
		/// the access checking is confined to the privileges of the
		/// original lookup class.
		/// </para>
		/// <para>
		/// A lookup can fail, because
		/// the containing class is not accessible to the lookup class, or
		/// because the desired class member is missing, or because the
		/// desired class member is not accessible to the lookup class, or
		/// because the lookup object is not trusted enough to access the member.
		/// In any of these cases, a {@code ReflectiveOperationException} will be
		/// thrown from the attempted lookup.  The exact class will be one of
		/// the following:
		/// <ul>
		/// <li>NoSuchMethodException &mdash; if a method is requested but does not exist
		/// <li>NoSuchFieldException &mdash; if a field is requested but does not exist
		/// <li>IllegalAccessException &mdash; if the member exists but an access check fails
		/// </ul>
		/// </para>
		/// <para>
		/// In general, the conditions under which a method handle may be
		/// looked up for a method {@code M} are no more restrictive than the conditions
		/// under which the lookup class could have compiled, verified, and resolved a call to {@code M}.
		/// Where the JVM would raise exceptions like {@code NoSuchMethodError},
		/// a method handle lookup will generally raise a corresponding
		/// checked exception, such as {@code NoSuchMethodException}.
		/// And the effect of invoking the method handle resulting from the lookup
		/// is <a href="MethodHandles.Lookup.html#equiv">exactly equivalent</a>
		/// to executing the compiled, verified, and resolved call to {@code M}.
		/// The same point is true of fields and constructors.
		/// <p style="font-size:smaller;">
		/// <em>Discussion:</em>
		/// Access checks only apply to named and reflected methods,
		/// constructors, and fields.
		/// Other method handle creation methods, such as
		/// <seealso cref="MethodHandle#asType MethodHandle.asType"/>,
		/// do not require any access checks, and are used
		/// independently of any {@code Lookup} object.
		/// </para>
		/// <para>
		/// If the desired member is {@code protected}, the usual JVM rules apply,
		/// including the requirement that the lookup class must be either be in the
		/// same package as the desired member, or must inherit that member.
		/// (See the Java Virtual Machine Specification, sections 4.9.2, 5.4.3.5, and 6.4.)
		/// In addition, if the desired member is a non-static field or method
		/// in a different package, the resulting method handle may only be applied
		/// to objects of the lookup class or one of its subclasses.
		/// This requirement is enforced by narrowing the type of the leading
		/// {@code this} parameter from {@code C}
		/// (which will necessarily be a superclass of the lookup class)
		/// to the lookup class itself.
		/// </para>
		/// <para>
		/// The JVM imposes a similar requirement on {@code invokespecial} instruction,
		/// that the receiver argument must match both the resolved method <em>and</em>
		/// the current class.  Again, this requirement is enforced by narrowing the
		/// type of the leading parameter to the resulting method handle.
		/// (See the Java Virtual Machine Specification, section 4.10.1.9.)
		/// </para>
		/// <para>
		/// The JVM represents constructors and static initializer blocks as internal methods
		/// with special names ({@code "<init>"} and {@code "<clinit>"}).
		/// The internal syntax of invocation instructions allows them to refer to such internal
		/// methods as if they were normal methods, but the JVM bytecode verifier rejects them.
		/// A lookup of such an internal method will produce a {@code NoSuchMethodException}.
		/// </para>
		/// <para>
		/// In some cases, access between nested classes is obtained by the Java compiler by creating
		/// an wrapper method to access a private method of another class
		/// in the same top-level declaration.
		/// For example, a nested class {@code C.D}
		/// can access private members within other related classes such as
		/// {@code C}, {@code C.D.E}, or {@code C.B},
		/// but the Java compiler may need to generate wrapper methods in
		/// those related classes.  In such cases, a {@code Lookup} object on
		/// {@code C.E} would be unable to those private members.
		/// A workaround for this limitation is the <seealso cref="Lookup#in Lookup.in"/> method,
		/// which can transform a lookup on {@code C.E} into one on any of those other
		/// classes, without special elevation of privilege.
		/// </para>
		/// <para>
		/// The accesses permitted to a given lookup object may be limited,
		/// according to its set of <seealso cref="#lookupModes lookupModes"/>,
		/// to a subset of members normally accessible to the lookup class.
		/// For example, the <seealso cref="MethodHandles#publicLookup publicLookup"/>
		/// method produces a lookup object which is only allowed to access
		/// public members in public classes.
		/// The caller sensitive method <seealso cref="MethodHandles#lookup lookup"/>
		/// produces a lookup object with full capabilities relative to
		/// its caller class, to emulate all supported bytecode behaviors.
		/// Also, the <seealso cref="Lookup#in Lookup.in"/> method may produce a lookup object
		/// with fewer access modes than the original lookup object.
		/// 
		/// <p style="font-size:smaller;">
		/// <a name="privacc"></a>
		/// <em>Discussion of private access:</em>
		/// We say that a lookup has <em>private access</em>
		/// if its <seealso cref="#lookupModes lookup modes"/>
		/// include the possibility of accessing {@code private} members.
		/// As documented in the relevant methods elsewhere,
		/// only lookups with private access possess the following capabilities:
		/// <ul style="font-size:smaller;">
		/// <li>access private fields, methods, and constructors of the lookup class
		/// <li>create method handles which invoke <a href="MethodHandles.Lookup.html#callsens">caller sensitive</a> methods,
		///     such as {@code Class.forName}
		/// <li>create method handles which <seealso cref="Lookup#findSpecial emulate invokespecial"/> instructions
		/// <li>avoid <a href="MethodHandles.Lookup.html#secmgr">package access checks</a>
		///     for classes accessible to the lookup class
		/// <li>create <seealso cref="Lookup#in delegated lookup objects"/> which have private access to other classes
		///     within the same package member
		/// </ul>
		/// <p style="font-size:smaller;">
		/// Each of these permissions is a consequence of the fact that a lookup object
		/// with private access can be securely traced back to an originating class,
		/// whose <a href="MethodHandles.Lookup.html#equiv">bytecode behaviors</a> and Java language access permissions
		/// can be reliably determined and emulated by method handles.
		/// 
		/// <h1><a name="secmgr"></a>Security manager interactions</h1>
		/// Although bytecode instructions can only refer to classes in
		/// a related class loader, this API can search for methods in any
		/// class, as long as a reference to its {@code Class} object is
		/// available.  Such cross-loader references are also possible with the
		/// Core Reflection API, and are impossible to bytecode instructions
		/// such as {@code invokestatic} or {@code getfield}.
		/// There is a <seealso cref="java.lang.SecurityManager security manager API"/>
		/// to allow applications to check such cross-loader references.
		/// These checks apply to both the {@code MethodHandles.Lookup} API
		/// and the Core Reflection API
		/// (as found on <seealso cref="java.lang.Class Class"/>).
		/// </para>
		/// <para>
		/// If a security manager is present, member lookups are subject to
		/// additional checks.
		/// From one to three calls are made to the security manager.
		/// Any of these calls can refuse access by throwing a
		/// <seealso cref="java.lang.SecurityException SecurityException"/>.
		/// Define {@code smgr} as the security manager,
		/// {@code lookc} as the lookup class of the current lookup object,
		/// {@code refc} as the containing class in which the member
		/// is being sought, and {@code defc} as the class in which the
		/// member is actually defined.
		/// The value {@code lookc} is defined as <em>not present</em>
		/// if the current lookup object does not have
		/// <a href="MethodHandles.Lookup.html#privacc">private access</a>.
		/// The calls are made according to the following rules:
		/// <ul>
		/// <li><b>Step 1:</b>
		///     If {@code lookc} is not present, or if its class loader is not
		///     the same as or an ancestor of the class loader of {@code refc},
		///     then {@link SecurityManager#checkPackageAccess
		///     smgr.checkPackageAccess(refcPkg)} is called,
		///     where {@code refcPkg} is the package of {@code refc}.
		/// <li><b>Step 2:</b>
		///     If the retrieved member is not public and
		///     {@code lookc} is not present, then
		///     <seealso cref="SecurityManager#checkPermission smgr.checkPermission"/>
		///     with {@code RuntimePermission("accessDeclaredMembers")} is called.
		/// <li><b>Step 3:</b>
		///     If the retrieved member is not public,
		///     and if {@code lookc} is not present,
		///     and if {@code defc} and {@code refc} are different,
		///     then {@link SecurityManager#checkPackageAccess
		///     smgr.checkPackageAccess(defcPkg)} is called,
		///     where {@code defcPkg} is the package of {@code defc}.
		/// </ul>
		/// Security checks are performed after other access checks have passed.
		/// Therefore, the above rules presuppose a member that is public,
		/// or else that is being accessed from a lookup class that has
		/// rights to access the member.
		/// 
		/// <h1><a name="callsens"></a>Caller sensitive methods</h1>
		/// A small number of Java methods have a special property called caller sensitivity.
		/// A <em>caller-sensitive</em> method can behave differently depending on the
		/// identity of its immediate caller.
		/// </para>
		/// <para>
		/// If a method handle for a caller-sensitive method is requested,
		/// the general rules for <a href="MethodHandles.Lookup.html#equiv">bytecode behaviors</a> apply,
		/// but they take account of the lookup class in a special way.
		/// The resulting method handle behaves as if it were called
		/// from an instruction contained in the lookup class,
		/// so that the caller-sensitive method detects the lookup class.
		/// (By contrast, the invoker of the method handle is disregarded.)
		/// Thus, in the case of caller-sensitive methods,
		/// different lookup classes may give rise to
		/// differently behaving method handles.
		/// </para>
		/// <para>
		/// In cases where the lookup object is
		/// <seealso cref="MethodHandles#publicLookup() publicLookup()"/>,
		/// or some other lookup object without
		/// <a href="MethodHandles.Lookup.html#privacc">private access</a>,
		/// the lookup class is disregarded.
		/// In such cases, no caller-sensitive method handle can be created,
		/// access is forbidden, and the lookup fails with an
		/// {@code IllegalAccessException}.
		/// <p style="font-size:smaller;">
		/// <em>Discussion:</em>
		/// For example, the caller-sensitive method
		/// <seealso cref="java.lang.Class#forName(String) Class.forName(x)"/>
		/// can return varying classes or throw varying exceptions,
		/// depending on the class loader of the class that calls it.
		/// A public lookup of {@code Class.forName} will fail, because
		/// there is no reasonable way to determine its bytecode behavior.
		/// <p style="font-size:smaller;">
		/// If an application caches method handles for broad sharing,
		/// it should use {@code publicLookup()} to create them.
		/// If there is a lookup of {@code Class.forName}, it will fail,
		/// and the application must take appropriate action in that case.
		/// It may be that a later lookup, perhaps during the invocation of a
		/// bootstrap method, can incorporate the specific identity
		/// of the caller, making the method accessible.
		/// <p style="font-size:smaller;">
		/// The function {@code MethodHandles.lookup} is caller sensitive
		/// so that there can be a secure foundation for lookups.
		/// Nearly all other methods in the JSR 292 API rely on lookup
		/// objects to check access requests.
		/// </para>
		/// </summary>
		public sealed class Lookup
		{
			/// <summary>
			/// The class on behalf of whom the lookup is being performed. </summary>
			internal readonly Class LookupClass_Renamed;

			/// <summary>
			/// The allowed sorts of members which may be looked up (PUBLIC, etc.). </summary>
			internal readonly int AllowedModes;

			/// <summary>
			/// A single-bit mask representing {@code public} access,
			///  which may contribute to the result of <seealso cref="#lookupModes lookupModes"/>.
			///  The value, {@code 0x01}, happens to be the same as the value of the
			///  {@code public} <seealso cref="java.lang.reflect.Modifier#PUBLIC modifier bit"/>.
			/// </summary>
			public const int PUBLIC = Modifier.PUBLIC;

			/// <summary>
			/// A single-bit mask representing {@code private} access,
			///  which may contribute to the result of <seealso cref="#lookupModes lookupModes"/>.
			///  The value, {@code 0x02}, happens to be the same as the value of the
			///  {@code private} <seealso cref="java.lang.reflect.Modifier#PRIVATE modifier bit"/>.
			/// </summary>
			public const int PRIVATE = Modifier.PRIVATE;

			/// <summary>
			/// A single-bit mask representing {@code protected} access,
			///  which may contribute to the result of <seealso cref="#lookupModes lookupModes"/>.
			///  The value, {@code 0x04}, happens to be the same as the value of the
			///  {@code protected} <seealso cref="java.lang.reflect.Modifier#PROTECTED modifier bit"/>.
			/// </summary>
			public const int PROTECTED = Modifier.PROTECTED;

			/// <summary>
			/// A single-bit mask representing {@code package} access (default access),
			///  which may contribute to the result of <seealso cref="#lookupModes lookupModes"/>.
			///  The value is {@code 0x08}, which does not correspond meaningfully to
			///  any particular <seealso cref="java.lang.reflect.Modifier modifier bit"/>.
			/// </summary>
			public const int PACKAGE = Modifier.STATIC;

			internal static readonly int ALL_MODES = (PUBLIC | PRIVATE | PROTECTED | PACKAGE);
			internal const int TRUSTED = -1;

			internal static int Fixmods(int mods)
			{
				mods &= (ALL_MODES - PACKAGE);
				return (mods != 0) ? mods : PACKAGE;
			}

			/// <summary>
			/// Tells which class is performing the lookup.  It is this class against
			///  which checks are performed for visibility and access permissions.
			///  <para>
			///  The class implies a maximum level of access permission,
			///  but the permissions may be additionally limited by the bitmask
			///  <seealso cref="#lookupModes lookupModes"/>, which controls whether non-public members
			///  can be accessed.
			/// </para>
			/// </summary>
			///  <returns> the lookup class, on behalf of which this lookup object finds members </returns>
			public Class LookupClass()
			{
				return LookupClass_Renamed;
			}

			// This is just for calling out to MethodHandleImpl.
			internal Class LookupClassOrNull()
			{
				return (AllowedModes == TRUSTED) ? null : LookupClass_Renamed;
			}

			/// <summary>
			/// Tells which access-protection classes of members this lookup object can produce.
			///  The result is a bit-mask of the bits
			///  <seealso cref="#PUBLIC PUBLIC (0x01)"/>,
			///  <seealso cref="#PRIVATE PRIVATE (0x02)"/>,
			///  <seealso cref="#PROTECTED PROTECTED (0x04)"/>,
			///  and <seealso cref="#PACKAGE PACKAGE (0x08)"/>.
			///  <para>
			///  A freshly-created lookup object
			///  on the {@link java.lang.invoke.MethodHandles#lookup() caller's class}
			///  has all possible bits set, since the caller class can access all its own members.
			///  A lookup object on a new lookup class
			///  <seealso cref="java.lang.invoke.MethodHandles.Lookup#in created from a previous lookup object"/>
			///  may have some mode bits set to zero.
			///  The purpose of this is to restrict access via the new lookup object,
			///  so that it can access only names which can be reached by the original
			///  lookup object, and also by the new lookup class.
			/// </para>
			/// </summary>
			///  <returns> the lookup modes, which limit the kinds of access performed by this lookup object </returns>
			public int LookupModes()
			{
				return AllowedModes & ALL_MODES;
			}

			/// <summary>
			/// Embody the current class (the lookupClass) as a lookup class
			/// for method handle creation.
			/// Must be called by from a method in this package,
			/// which in turn is called by a method not in this package.
			/// </summary>
			internal Lookup(Class lookupClass) : this(lookupClass, ALL_MODES)
			{
				// make sure we haven't accidentally picked up a privileged class:
				CheckUnprivilegedlookupClass(lookupClass, ALL_MODES);
			}

			internal Lookup(Class lookupClass, int allowedModes)
			{
				this.LookupClass_Renamed = lookupClass;
				this.AllowedModes = allowedModes;
			}

			/// <summary>
			/// Creates a lookup on the specified new lookup class.
			/// The resulting object will report the specified
			/// class as its own <seealso cref="#lookupClass lookupClass"/>.
			/// <para>
			/// However, the resulting {@code Lookup} object is guaranteed
			/// to have no more access capabilities than the original.
			/// In particular, access capabilities can be lost as follows:<ul>
			/// <li>If the new lookup class differs from the old one,
			/// protected members will not be accessible by virtue of inheritance.
			/// (Protected members may continue to be accessible because of package sharing.)
			/// <li>If the new lookup class is in a different package
			/// than the old one, protected and default (package) members will not be accessible.
			/// <li>If the new lookup class is not within the same package member
			/// as the old one, private members will not be accessible.
			/// <li>If the new lookup class is not accessible to the old lookup class,
			/// then no members, not even public members, will be accessible.
			/// (In all other cases, public members will continue to be accessible.)
			/// </ul>
			/// 
			/// </para>
			/// </summary>
			/// <param name="requestedLookupClass"> the desired lookup class for the new lookup object </param>
			/// <returns> a lookup object which reports the desired lookup class </returns>
			/// <exception cref="NullPointerException"> if the argument is null </exception>
			public Lookup @in(Class requestedLookupClass)
			{
				requestedLookupClass.GetType(); // null check
				if (AllowedModes == TRUSTED) // IMPL_LOOKUP can make any lookup at all
				{
					return new Lookup(requestedLookupClass, ALL_MODES);
				}
				if (requestedLookupClass == this.LookupClass_Renamed)
				{
					return this; // keep same capabilities
				}
				int newModes = (AllowedModes & (ALL_MODES & ~PROTECTED));
				if ((newModes & PACKAGE) != 0 && !VerifyAccess.isSamePackage(this.LookupClass_Renamed, requestedLookupClass))
				{
					newModes &= ~(PACKAGE | PRIVATE);
				}
				// Allow nestmate lookups to be created without special privilege:
				if ((newModes & PRIVATE) != 0 && !VerifyAccess.isSamePackageMember(this.LookupClass_Renamed, requestedLookupClass))
				{
					newModes &= ~PRIVATE;
				}
				if ((newModes & PUBLIC) != 0 && !VerifyAccess.isClassAccessible(requestedLookupClass, this.LookupClass_Renamed, AllowedModes))
				{
					// The requested class it not accessible from the lookup class.
					// No permissions.
					newModes = 0;
				}
				CheckUnprivilegedlookupClass(requestedLookupClass, newModes);
				return new Lookup(requestedLookupClass, newModes);
			}

			// Make sure outer class is initialized first.

			/// <summary>
			/// Version of lookup which is trusted minimally.
			///  It can only be used to create method handles to
			///  publicly accessible members.
			/// </summary>
			internal static readonly Lookup PUBLIC_LOOKUP = new Lookup(typeof(Object), PUBLIC);

			/// <summary>
			/// Package-private version of lookup which is trusted. </summary>
			internal static readonly Lookup IMPL_LOOKUP = new Lookup(typeof(Object), TRUSTED);

			internal static void CheckUnprivilegedlookupClass(Class lookupClass, int allowedModes)
			{
				String name = lookupClass.Name;
				if (name.StartsWith("java.lang.invoke."))
				{
					throw newIllegalArgumentException("illegal lookupClass: " + lookupClass);
				}

				// For caller-sensitive MethodHandles.lookup()
				// disallow lookup more restricted packages
				if (allowedModes == ALL_MODES && lookupClass.ClassLoader == null)
				{
					if (name.StartsWith("java.") || (name.StartsWith("sun.") && !name.StartsWith("sun.invoke.")))
					{
						throw newIllegalArgumentException("illegal lookupClass: " + lookupClass);
					}
				}
			}

			/// <summary>
			/// Displays the name of the class from which lookups are to be made.
			/// (The name is the one reported by <seealso cref="java.lang.Class#getName() Class.getName"/>.)
			/// If there are restrictions on the access permitted to this lookup,
			/// this is indicated by adding a suffix to the class name, consisting
			/// of a slash and a keyword.  The keyword represents the strongest
			/// allowed access, and is chosen as follows:
			/// <ul>
			/// <li>If no access is allowed, the suffix is "/noaccess".
			/// <li>If only public access is allowed, the suffix is "/public".
			/// <li>If only public and package access are allowed, the suffix is "/package".
			/// <li>If only public, package, and private access are allowed, the suffix is "/private".
			/// </ul>
			/// If none of the above cases apply, it is the case that full
			/// access (public, package, private, and protected) is allowed.
			/// In this case, no suffix is added.
			/// This is true only of an object obtained originally from
			/// <seealso cref="java.lang.invoke.MethodHandles#lookup MethodHandles.lookup"/>.
			/// Objects created by <seealso cref="java.lang.invoke.MethodHandles.Lookup#in Lookup.in"/>
			/// always have restricted access, and will display a suffix.
			/// <para>
			/// (It may seem strange that protected access should be
			/// stronger than private access.  Viewed independently from
			/// package access, protected access is the first to be lost,
			/// because it requires a direct subclass relationship between
			/// caller and callee.)
			/// </para>
			/// </summary>
			/// <seealso cref= #in </seealso>
			public override String ToString()
			{
				String cname = LookupClass_Renamed.Name;
				switch (AllowedModes)
				{
				case 0: // no privileges
					return cname + "/noaccess";
				case PUBLIC:
					return cname + "/public";
				case PUBLIC | PACKAGE:
					return cname + "/package";
				case ALL_MODES & ~PROTECTED:
					return cname + "/private";
				case ALL_MODES:
					return cname;
				case TRUSTED:
					return "/trusted"; // internal only; not exported
				default: // Should not happen, but it's a bitfield...
					cname = cname + "/" + AllowedModes.ToString("x");
					assert(false) : cname;
					return cname;
				}
			}

			/// <summary>
			/// Produces a method handle for a static method.
			/// The type of the method handle will be that of the method.
			/// (Since static methods do not take receivers, there is no
			/// additional receiver argument inserted into the method handle type,
			/// as there would be with <seealso cref="#findVirtual findVirtual"/> or <seealso cref="#findSpecial findSpecial"/>.)
			/// The method and all its argument types must be accessible to the lookup object.
			/// <para>
			/// The returned method handle will have
			/// <seealso cref="MethodHandle#asVarargsCollector variable arity"/> if and only if
			/// the method's variable arity modifier bit ({@code 0x0080}) is set.
			/// </para>
			/// <para>
			/// If the returned method handle is invoked, the method's class will
			/// be initialized, if it has not already been initialized.
			/// </para>
			/// <para><b>Example:</b>
			/// <blockquote><pre>{@code
			/// import static java.lang.invoke.MethodHandles.*;
			/// import static java.lang.invoke.MethodType.*;
			/// ...
			/// MethodHandle MH_asList = publicLookup().findStatic(Arrays.class,
			/// "asList", methodType(List.class, Object[].class));
			/// assertEquals("[x, y]", MH_asList.invoke("x", "y").toString());
			/// }</pre></blockquote>
			/// </para>
			/// </summary>
			/// <param name="refc"> the class from which the method is accessed </param>
			/// <param name="name"> the name of the method </param>
			/// <param name="type"> the type of the method </param>
			/// <returns> the desired method handle </returns>
			/// <exception cref="NoSuchMethodException"> if the method does not exist </exception>
			/// <exception cref="IllegalAccessException"> if access checking fails,
			///                                or if the method is not {@code static},
			///                                or if the method's variable arity modifier bit
			///                                is set and {@code asVarargsCollector} fails </exception>
			/// <exception cref="SecurityException"> if a security manager is present and it
			///                              <a href="MethodHandles.Lookup.html#secmgr">refuses access</a> </exception>
			/// <exception cref="NullPointerException"> if any argument is null </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public MethodHandle findStatic(Class refc, String name, MethodType type) throws NoSuchMethodException, IllegalAccessException
			public MethodHandle FindStatic(Class refc, String name, MethodType type)
			{
				MemberName method = ResolveOrFail(REF_invokeStatic, refc, name, type);
				return GetDirectMethod(REF_invokeStatic, refc, method, FindBoundCallerClass(method));
			}

			/// <summary>
			/// Produces a method handle for a virtual method.
			/// The type of the method handle will be that of the method,
			/// with the receiver type (usually {@code refc}) prepended.
			/// The method and all its argument types must be accessible to the lookup object.
			/// <para>
			/// When called, the handle will treat the first argument as a receiver
			/// and dispatch on the receiver's type to determine which method
			/// implementation to enter.
			/// (The dispatching action is identical with that performed by an
			/// {@code invokevirtual} or {@code invokeinterface} instruction.)
			/// </para>
			/// <para>
			/// The first argument will be of type {@code refc} if the lookup
			/// class has full privileges to access the member.  Otherwise
			/// the member must be {@code protected} and the first argument
			/// will be restricted in type to the lookup class.
			/// </para>
			/// <para>
			/// The returned method handle will have
			/// <seealso cref="MethodHandle#asVarargsCollector variable arity"/> if and only if
			/// the method's variable arity modifier bit ({@code 0x0080}) is set.
			/// </para>
			/// <para>
			/// Because of the general <a href="MethodHandles.Lookup.html#equiv">equivalence</a> between {@code invokevirtual}
			/// instructions and method handles produced by {@code findVirtual},
			/// if the class is {@code MethodHandle} and the name string is
			/// {@code invokeExact} or {@code invoke}, the resulting
			/// method handle is equivalent to one produced by
			/// <seealso cref="java.lang.invoke.MethodHandles#exactInvoker MethodHandles.exactInvoker"/> or
			/// <seealso cref="java.lang.invoke.MethodHandles#invoker MethodHandles.invoker"/>
			/// with the same {@code type} argument.
			/// 
			/// <b>Example:</b>
			/// <blockquote><pre>{@code
			/// import static java.lang.invoke.MethodHandles.*;
			/// import static java.lang.invoke.MethodType.*;
			/// ...
			/// MethodHandle MH_concat = publicLookup().findVirtual(String.class,
			/// "concat", methodType(String.class, String.class));
			/// MethodHandle MH_hashCode = publicLookup().findVirtual(Object.class,
			/// "hashCode", methodType(int.class));
			/// MethodHandle MH_hashCode_String = publicLookup().findVirtual(String.class,
			/// "hashCode", methodType(int.class));
			/// assertEquals("xy", (String) MH_concat.invokeExact("x", "y"));
			/// assertEquals("xy".hashCode(), (int) MH_hashCode.invokeExact((Object)"xy"));
			/// assertEquals("xy".hashCode(), (int) MH_hashCode_String.invokeExact("xy"));
			/// // interface method:
			/// MethodHandle MH_subSequence = publicLookup().findVirtual(CharSequence.class,
			/// "subSequence", methodType(CharSequence.class, int.class, int.class));
			/// assertEquals("def", MH_subSequence.invoke("abcdefghi", 3, 6).toString());
			/// // constructor "internal method" must be accessed differently:
			/// MethodType MT_newString = methodType(void.class); //()V for new String()
			/// try { assertEquals("impossible", lookup()
			/// .findVirtual(String.class, "<init>", MT_newString));
			/// } catch (NoSuchMethodException ex) { } // OK
			/// MethodHandle MH_newString = publicLookup()
			/// .findConstructor(String.class, MT_newString);
			/// assertEquals("", (String) MH_newString.invokeExact());
			/// }</pre></blockquote>
			/// 
			/// </para>
			/// </summary>
			/// <param name="refc"> the class or interface from which the method is accessed </param>
			/// <param name="name"> the name of the method </param>
			/// <param name="type"> the type of the method, with the receiver argument omitted </param>
			/// <returns> the desired method handle </returns>
			/// <exception cref="NoSuchMethodException"> if the method does not exist </exception>
			/// <exception cref="IllegalAccessException"> if access checking fails,
			///                                or if the method is {@code static}
			///                                or if the method's variable arity modifier bit
			///                                is set and {@code asVarargsCollector} fails </exception>
			/// <exception cref="SecurityException"> if a security manager is present and it
			///                              <a href="MethodHandles.Lookup.html#secmgr">refuses access</a> </exception>
			/// <exception cref="NullPointerException"> if any argument is null </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public MethodHandle findVirtual(Class refc, String name, MethodType type) throws NoSuchMethodException, IllegalAccessException
			public MethodHandle FindVirtual(Class refc, String name, MethodType type)
			{
				if (refc == typeof(MethodHandle))
				{
					MethodHandle mh = FindVirtualForMH(name, type);
					if (mh != null)
					{
						return mh;
					}
				}
				sbyte refKind = (refc.Interface ? REF_invokeInterface : REF_invokeVirtual);
				MemberName method = ResolveOrFail(refKind, refc, name, type);
				return GetDirectMethod(refKind, refc, method, FindBoundCallerClass(method));
			}
			internal MethodHandle FindVirtualForMH(String name, MethodType type)
			{
				// these names require special lookups because of the implicit MethodType argument
				if ("invoke".Equals(name))
				{
					return Invoker(type);
				}
				if ("invokeExact".Equals(name))
				{
					return ExactInvoker(type);
				}
				if ("invokeBasic".Equals(name))
				{
					return BasicInvoker(type);
				}
				assert(!MemberName.IsMethodHandleInvokeName(name));
				return null;
			}

			/// <summary>
			/// Produces a method handle which creates an object and initializes it, using
			/// the constructor of the specified type.
			/// The parameter types of the method handle will be those of the constructor,
			/// while the return type will be a reference to the constructor's class.
			/// The constructor and all its argument types must be accessible to the lookup object.
			/// <para>
			/// The requested type must have a return type of {@code void}.
			/// (This is consistent with the JVM's treatment of constructor type descriptors.)
			/// </para>
			/// <para>
			/// The returned method handle will have
			/// <seealso cref="MethodHandle#asVarargsCollector variable arity"/> if and only if
			/// the constructor's variable arity modifier bit ({@code 0x0080}) is set.
			/// </para>
			/// <para>
			/// If the returned method handle is invoked, the constructor's class will
			/// be initialized, if it has not already been initialized.
			/// </para>
			/// <para><b>Example:</b>
			/// <blockquote><pre>{@code
			/// import static java.lang.invoke.MethodHandles.*;
			/// import static java.lang.invoke.MethodType.*;
			/// ...
			/// MethodHandle MH_newArrayList = publicLookup().findConstructor(
			/// ArrayList.class, methodType(void.class, Collection.class));
			/// Collection orig = Arrays.asList("x", "y");
			/// Collection copy = (ArrayList) MH_newArrayList.invokeExact(orig);
			/// assert(orig != copy);
			/// assertEquals(orig, copy);
			/// // a variable-arity constructor:
			/// MethodHandle MH_newProcessBuilder = publicLookup().findConstructor(
			/// ProcessBuilder.class, methodType(void.class, String[].class));
			/// ProcessBuilder pb = (ProcessBuilder)
			/// MH_newProcessBuilder.invoke("x", "y", "z");
			/// assertEquals("[x, y, z]", pb.command().toString());
			/// }</pre></blockquote>
			/// </para>
			/// </summary>
			/// <param name="refc"> the class or interface from which the method is accessed </param>
			/// <param name="type"> the type of the method, with the receiver argument omitted, and a void return type </param>
			/// <returns> the desired method handle </returns>
			/// <exception cref="NoSuchMethodException"> if the constructor does not exist </exception>
			/// <exception cref="IllegalAccessException"> if access checking fails
			///                                or if the method's variable arity modifier bit
			///                                is set and {@code asVarargsCollector} fails </exception>
			/// <exception cref="SecurityException"> if a security manager is present and it
			///                              <a href="MethodHandles.Lookup.html#secmgr">refuses access</a> </exception>
			/// <exception cref="NullPointerException"> if any argument is null </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public MethodHandle findConstructor(Class refc, MethodType type) throws NoSuchMethodException, IllegalAccessException
			public MethodHandle FindConstructor(Class refc, MethodType type)
			{
				String name = "<init>";
				MemberName ctor = ResolveOrFail(REF_newInvokeSpecial, refc, name, type);
				return GetDirectConstructor(refc, ctor);
			}

			/// <summary>
			/// Produces an early-bound method handle for a virtual method.
			/// It will bypass checks for overriding methods on the receiver,
			/// <a href="MethodHandles.Lookup.html#equiv">as if called</a> from an {@code invokespecial}
			/// instruction from within the explicitly specified {@code specialCaller}.
			/// The type of the method handle will be that of the method,
			/// with a suitably restricted receiver type prepended.
			/// (The receiver type will be {@code specialCaller} or a subtype.)
			/// The method and all its argument types must be accessible
			/// to the lookup object.
			/// <para>
			/// Before method resolution,
			/// if the explicitly specified caller class is not identical with the
			/// lookup class, or if this lookup object does not have
			/// <a href="MethodHandles.Lookup.html#privacc">private access</a>
			/// privileges, the access fails.
			/// </para>
			/// <para>
			/// The returned method handle will have
			/// <seealso cref="MethodHandle#asVarargsCollector variable arity"/> if and only if
			/// the method's variable arity modifier bit ({@code 0x0080}) is set.
			/// <p style="font-size:smaller;">
			/// <em>(Note:  JVM internal methods named {@code "<init>"} are not visible to this API,
			/// even though the {@code invokespecial} instruction can refer to them
			/// in special circumstances.  Use <seealso cref="#findConstructor findConstructor"/>
			/// to access instance initialization methods in a safe manner.)</em>
			/// </para>
			/// <para><b>Example:</b>
			/// <blockquote><pre>{@code
			/// import static java.lang.invoke.MethodHandles.*;
			/// import static java.lang.invoke.MethodType.*;
			/// ...
			/// static class Listie extends ArrayList {
			/// public String toString() { return "[wee Listie]"; }
			/// static Lookup lookup() { return MethodHandles.lookup(); }
			/// }
			/// ...
			/// // no access to constructor via invokeSpecial:
			/// MethodHandle MH_newListie = Listie.lookup()
			/// .findConstructor(Listie.class, methodType(void.class));
			/// Listie l = (Listie) MH_newListie.invokeExact();
			/// try { assertEquals("impossible", Listie.lookup().findSpecial(
			/// Listie.class, "<init>", methodType(void.class), Listie.class));
			/// } catch (NoSuchMethodException ex) { } // OK
			/// // access to super and self methods via invokeSpecial:
			/// MethodHandle MH_super = Listie.lookup().findSpecial(
			/// ArrayList.class, "toString" , methodType(String.class), Listie.class);
			/// MethodHandle MH_this = Listie.lookup().findSpecial(
			/// Listie.class, "toString" , methodType(String.class), Listie.class);
			/// MethodHandle MH_duper = Listie.lookup().findSpecial(
			/// Object.class, "toString" , methodType(String.class), Listie.class);
			/// assertEquals("[]", (String) MH_super.invokeExact(l));
			/// assertEquals(""+l, (String) MH_this.invokeExact(l));
			/// assertEquals("[]", (String) MH_duper.invokeExact(l)); // ArrayList method
			/// try { assertEquals("inaccessible", Listie.lookup().findSpecial(
			/// String.class, "toString", methodType(String.class), Listie.class));
			/// } catch (IllegalAccessException ex) { } // OK
			/// Listie subl = new Listie() { public String toString() { return "[subclass]"; } };
			/// assertEquals(""+l, (String) MH_this.invokeExact(subl)); // Listie method
			/// }</pre></blockquote>
			/// 
			/// </para>
			/// </summary>
			/// <param name="refc"> the class or interface from which the method is accessed </param>
			/// <param name="name"> the name of the method (which must not be "&lt;init&gt;") </param>
			/// <param name="type"> the type of the method, with the receiver argument omitted </param>
			/// <param name="specialCaller"> the proposed calling class to perform the {@code invokespecial} </param>
			/// <returns> the desired method handle </returns>
			/// <exception cref="NoSuchMethodException"> if the method does not exist </exception>
			/// <exception cref="IllegalAccessException"> if access checking fails
			///                                or if the method's variable arity modifier bit
			///                                is set and {@code asVarargsCollector} fails </exception>
			/// <exception cref="SecurityException"> if a security manager is present and it
			///                              <a href="MethodHandles.Lookup.html#secmgr">refuses access</a> </exception>
			/// <exception cref="NullPointerException"> if any argument is null </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public MethodHandle findSpecial(Class refc, String name, MethodType type, Class specialCaller) throws NoSuchMethodException, IllegalAccessException
			public MethodHandle FindSpecial(Class refc, String name, MethodType type, Class specialCaller)
			{
				CheckSpecialCaller(specialCaller);
				Lookup specialLookup = this.@in(specialCaller);
				MemberName method = specialLookup.ResolveOrFail(REF_invokeSpecial, refc, name, type);
				return specialLookup.GetDirectMethod(REF_invokeSpecial, refc, method, FindBoundCallerClass(method));
			}

			/// <summary>
			/// Produces a method handle giving read access to a non-static field.
			/// The type of the method handle will have a return type of the field's
			/// value type.
			/// The method handle's single argument will be the instance containing
			/// the field.
			/// Access checking is performed immediately on behalf of the lookup class. </summary>
			/// <param name="refc"> the class or interface from which the method is accessed </param>
			/// <param name="name"> the field's name </param>
			/// <param name="type"> the field's type </param>
			/// <returns> a method handle which can load values from the field </returns>
			/// <exception cref="NoSuchFieldException"> if the field does not exist </exception>
			/// <exception cref="IllegalAccessException"> if access checking fails, or if the field is {@code static} </exception>
			/// <exception cref="SecurityException"> if a security manager is present and it
			///                              <a href="MethodHandles.Lookup.html#secmgr">refuses access</a> </exception>
			/// <exception cref="NullPointerException"> if any argument is null </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public MethodHandle findGetter(Class refc, String name, Class type) throws NoSuchFieldException, IllegalAccessException
			public MethodHandle FindGetter(Class refc, String name, Class type)
			{
				MemberName field = ResolveOrFail(REF_getField, refc, name, type);
				return GetDirectField(REF_getField, refc, field);
			}

			/// <summary>
			/// Produces a method handle giving write access to a non-static field.
			/// The type of the method handle will have a void return type.
			/// The method handle will take two arguments, the instance containing
			/// the field, and the value to be stored.
			/// The second argument will be of the field's value type.
			/// Access checking is performed immediately on behalf of the lookup class. </summary>
			/// <param name="refc"> the class or interface from which the method is accessed </param>
			/// <param name="name"> the field's name </param>
			/// <param name="type"> the field's type </param>
			/// <returns> a method handle which can store values into the field </returns>
			/// <exception cref="NoSuchFieldException"> if the field does not exist </exception>
			/// <exception cref="IllegalAccessException"> if access checking fails, or if the field is {@code static} </exception>
			/// <exception cref="SecurityException"> if a security manager is present and it
			///                              <a href="MethodHandles.Lookup.html#secmgr">refuses access</a> </exception>
			/// <exception cref="NullPointerException"> if any argument is null </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public MethodHandle findSetter(Class refc, String name, Class type) throws NoSuchFieldException, IllegalAccessException
			public MethodHandle FindSetter(Class refc, String name, Class type)
			{
				MemberName field = ResolveOrFail(REF_putField, refc, name, type);
				return GetDirectField(REF_putField, refc, field);
			}

			/// <summary>
			/// Produces a method handle giving read access to a static field.
			/// The type of the method handle will have a return type of the field's
			/// value type.
			/// The method handle will take no arguments.
			/// Access checking is performed immediately on behalf of the lookup class.
			/// <para>
			/// If the returned method handle is invoked, the field's class will
			/// be initialized, if it has not already been initialized.
			/// </para>
			/// </summary>
			/// <param name="refc"> the class or interface from which the method is accessed </param>
			/// <param name="name"> the field's name </param>
			/// <param name="type"> the field's type </param>
			/// <returns> a method handle which can load values from the field </returns>
			/// <exception cref="NoSuchFieldException"> if the field does not exist </exception>
			/// <exception cref="IllegalAccessException"> if access checking fails, or if the field is not {@code static} </exception>
			/// <exception cref="SecurityException"> if a security manager is present and it
			///                              <a href="MethodHandles.Lookup.html#secmgr">refuses access</a> </exception>
			/// <exception cref="NullPointerException"> if any argument is null </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public MethodHandle findStaticGetter(Class refc, String name, Class type) throws NoSuchFieldException, IllegalAccessException
			public MethodHandle FindStaticGetter(Class refc, String name, Class type)
			{
				MemberName field = ResolveOrFail(REF_getStatic, refc, name, type);
				return GetDirectField(REF_getStatic, refc, field);
			}

			/// <summary>
			/// Produces a method handle giving write access to a static field.
			/// The type of the method handle will have a void return type.
			/// The method handle will take a single
			/// argument, of the field's value type, the value to be stored.
			/// Access checking is performed immediately on behalf of the lookup class.
			/// <para>
			/// If the returned method handle is invoked, the field's class will
			/// be initialized, if it has not already been initialized.
			/// </para>
			/// </summary>
			/// <param name="refc"> the class or interface from which the method is accessed </param>
			/// <param name="name"> the field's name </param>
			/// <param name="type"> the field's type </param>
			/// <returns> a method handle which can store values into the field </returns>
			/// <exception cref="NoSuchFieldException"> if the field does not exist </exception>
			/// <exception cref="IllegalAccessException"> if access checking fails, or if the field is not {@code static} </exception>
			/// <exception cref="SecurityException"> if a security manager is present and it
			///                              <a href="MethodHandles.Lookup.html#secmgr">refuses access</a> </exception>
			/// <exception cref="NullPointerException"> if any argument is null </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public MethodHandle findStaticSetter(Class refc, String name, Class type) throws NoSuchFieldException, IllegalAccessException
			public MethodHandle FindStaticSetter(Class refc, String name, Class type)
			{
				MemberName field = ResolveOrFail(REF_putStatic, refc, name, type);
				return GetDirectField(REF_putStatic, refc, field);
			}

			/// <summary>
			/// Produces an early-bound method handle for a non-static method.
			/// The receiver must have a supertype {@code defc} in which a method
			/// of the given name and type is accessible to the lookup class.
			/// The method and all its argument types must be accessible to the lookup object.
			/// The type of the method handle will be that of the method,
			/// without any insertion of an additional receiver parameter.
			/// The given receiver will be bound into the method handle,
			/// so that every call to the method handle will invoke the
			/// requested method on the given receiver.
			/// <para>
			/// The returned method handle will have
			/// <seealso cref="MethodHandle#asVarargsCollector variable arity"/> if and only if
			/// the method's variable arity modifier bit ({@code 0x0080}) is set
			/// <em>and</em> the trailing array argument is not the only argument.
			/// (If the trailing array argument is the only argument,
			/// the given receiver value will be bound to it.)
			/// </para>
			/// <para>
			/// This is equivalent to the following code:
			/// <blockquote><pre>{@code
			/// import static java.lang.invoke.MethodHandles.*;
			/// import static java.lang.invoke.MethodType.*;
			/// ...
			/// MethodHandle mh0 = lookup().findVirtual(defc, name, type);
			/// MethodHandle mh1 = mh0.bindTo(receiver);
			/// MethodType mt1 = mh1.type();
			/// if (mh0.isVarargsCollector())
			/// mh1 = mh1.asVarargsCollector(mt1.parameterType(mt1.parameterCount()-1));
			/// return mh1;
			/// }</pre></blockquote>
			/// where {@code defc} is either {@code receiver.getClass()} or a super
			/// type of that class, in which the requested method is accessible
			/// to the lookup class.
			/// (Note that {@code bindTo} does not preserve variable arity.)
			/// </para>
			/// </summary>
			/// <param name="receiver"> the object from which the method is accessed </param>
			/// <param name="name"> the name of the method </param>
			/// <param name="type"> the type of the method, with the receiver argument omitted </param>
			/// <returns> the desired method handle </returns>
			/// <exception cref="NoSuchMethodException"> if the method does not exist </exception>
			/// <exception cref="IllegalAccessException"> if access checking fails
			///                                or if the method's variable arity modifier bit
			///                                is set and {@code asVarargsCollector} fails </exception>
			/// <exception cref="SecurityException"> if a security manager is present and it
			///                              <a href="MethodHandles.Lookup.html#secmgr">refuses access</a> </exception>
			/// <exception cref="NullPointerException"> if any argument is null </exception>
			/// <seealso cref= MethodHandle#bindTo </seealso>
			/// <seealso cref= #findVirtual </seealso>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public MethodHandle bind(Object receiver, String name, MethodType type) throws NoSuchMethodException, IllegalAccessException
			public MethodHandle Bind(Object receiver, String name, MethodType type)
			{
				Class refc = receiver.GetType(); // may get NPE
				MemberName method = ResolveOrFail(REF_invokeSpecial, refc, name, type);
				MethodHandle mh = GetDirectMethodNoRestrict(REF_invokeSpecial, refc, method, FindBoundCallerClass(method));
				return mh.BindArgumentL(0, receiver).setVarargs(method);
			}

			/// <summary>
			/// Makes a <a href="MethodHandleInfo.html#directmh">direct method handle</a>
			/// to <i>m</i>, if the lookup class has permission.
			/// If <i>m</i> is non-static, the receiver argument is treated as an initial argument.
			/// If <i>m</i> is virtual, overriding is respected on every call.
			/// Unlike the Core Reflection API, exceptions are <em>not</em> wrapped.
			/// The type of the method handle will be that of the method,
			/// with the receiver type prepended (but only if it is non-static).
			/// If the method's {@code accessible} flag is not set,
			/// access checking is performed immediately on behalf of the lookup class.
			/// If <i>m</i> is not public, do not share the resulting handle with untrusted parties.
			/// <para>
			/// The returned method handle will have
			/// <seealso cref="MethodHandle#asVarargsCollector variable arity"/> if and only if
			/// the method's variable arity modifier bit ({@code 0x0080}) is set.
			/// </para>
			/// <para>
			/// If <i>m</i> is static, and
			/// if the returned method handle is invoked, the method's class will
			/// be initialized, if it has not already been initialized.
			/// </para>
			/// </summary>
			/// <param name="m"> the reflected method </param>
			/// <returns> a method handle which can invoke the reflected method </returns>
			/// <exception cref="IllegalAccessException"> if access checking fails
			///                                or if the method's variable arity modifier bit
			///                                is set and {@code asVarargsCollector} fails </exception>
			/// <exception cref="NullPointerException"> if the argument is null </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public MethodHandle unreflect(Method m) throws IllegalAccessException
			public MethodHandle Unreflect(Method m)
			{
				if (m.DeclaringClass == typeof(MethodHandle))
				{
					MethodHandle mh = UnreflectForMH(m);
					if (mh != null)
					{
						return mh;
					}
				}
				MemberName method = new MemberName(m);
				sbyte refKind = method.ReferenceKind;
				if (refKind == REF_invokeSpecial)
				{
					refKind = REF_invokeVirtual;
				}
				assert(method.Method);
				Lookup lookup = m.Accessible ? IMPL_LOOKUP : this;
				return lookup.GetDirectMethodNoSecurityManager(refKind, method.DeclaringClass, method, FindBoundCallerClass(method));
			}
			internal MethodHandle UnreflectForMH(Method m)
			{
				// these names require special lookups because they throw UnsupportedOperationException
				if (MemberName.IsMethodHandleInvokeName(m.Name))
				{
					return MethodHandleImpl.FakeMethodHandleInvoke(new MemberName(m));
				}
				return null;
			}

			/// <summary>
			/// Produces a method handle for a reflected method.
			/// It will bypass checks for overriding methods on the receiver,
			/// <a href="MethodHandles.Lookup.html#equiv">as if called</a> from an {@code invokespecial}
			/// instruction from within the explicitly specified {@code specialCaller}.
			/// The type of the method handle will be that of the method,
			/// with a suitably restricted receiver type prepended.
			/// (The receiver type will be {@code specialCaller} or a subtype.)
			/// If the method's {@code accessible} flag is not set,
			/// access checking is performed immediately on behalf of the lookup class,
			/// as if {@code invokespecial} instruction were being linked.
			/// <para>
			/// Before method resolution,
			/// if the explicitly specified caller class is not identical with the
			/// lookup class, or if this lookup object does not have
			/// <a href="MethodHandles.Lookup.html#privacc">private access</a>
			/// privileges, the access fails.
			/// </para>
			/// <para>
			/// The returned method handle will have
			/// <seealso cref="MethodHandle#asVarargsCollector variable arity"/> if and only if
			/// the method's variable arity modifier bit ({@code 0x0080}) is set.
			/// </para>
			/// </summary>
			/// <param name="m"> the reflected method </param>
			/// <param name="specialCaller"> the class nominally calling the method </param>
			/// <returns> a method handle which can invoke the reflected method </returns>
			/// <exception cref="IllegalAccessException"> if access checking fails
			///                                or if the method's variable arity modifier bit
			///                                is set and {@code asVarargsCollector} fails </exception>
			/// <exception cref="NullPointerException"> if any argument is null </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public MethodHandle unreflectSpecial(Method m, Class specialCaller) throws IllegalAccessException
			public MethodHandle UnreflectSpecial(Method m, Class specialCaller)
			{
				CheckSpecialCaller(specialCaller);
				Lookup specialLookup = this.@in(specialCaller);
				MemberName method = new MemberName(m, true);
				assert(method.Method);
				// ignore m.isAccessible:  this is a new kind of access
				return specialLookup.GetDirectMethodNoSecurityManager(REF_invokeSpecial, method.DeclaringClass, method, FindBoundCallerClass(method));
			}

			/// <summary>
			/// Produces a method handle for a reflected constructor.
			/// The type of the method handle will be that of the constructor,
			/// with the return type changed to the declaring class.
			/// The method handle will perform a {@code newInstance} operation,
			/// creating a new instance of the constructor's class on the
			/// arguments passed to the method handle.
			/// <para>
			/// If the constructor's {@code accessible} flag is not set,
			/// access checking is performed immediately on behalf of the lookup class.
			/// </para>
			/// <para>
			/// The returned method handle will have
			/// <seealso cref="MethodHandle#asVarargsCollector variable arity"/> if and only if
			/// the constructor's variable arity modifier bit ({@code 0x0080}) is set.
			/// </para>
			/// <para>
			/// If the returned method handle is invoked, the constructor's class will
			/// be initialized, if it has not already been initialized.
			/// </para>
			/// </summary>
			/// <param name="c"> the reflected constructor </param>
			/// <returns> a method handle which can invoke the reflected constructor </returns>
			/// <exception cref="IllegalAccessException"> if access checking fails
			///                                or if the method's variable arity modifier bit
			///                                is set and {@code asVarargsCollector} fails </exception>
			/// <exception cref="NullPointerException"> if the argument is null </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public MethodHandle unreflectConstructor(Constructor<?> c) throws IllegalAccessException
			public MethodHandle unreflectConstructor<T1>(Constructor<T1> c)
			{
				MemberName ctor = new MemberName(c);
				assert(ctor.Constructor);
				Lookup lookup = c.Accessible ? IMPL_LOOKUP : this;
				return lookup.GetDirectConstructorNoSecurityManager(ctor.DeclaringClass, ctor);
			}

			/// <summary>
			/// Produces a method handle giving read access to a reflected field.
			/// The type of the method handle will have a return type of the field's
			/// value type.
			/// If the field is static, the method handle will take no arguments.
			/// Otherwise, its single argument will be the instance containing
			/// the field.
			/// If the field's {@code accessible} flag is not set,
			/// access checking is performed immediately on behalf of the lookup class.
			/// <para>
			/// If the field is static, and
			/// if the returned method handle is invoked, the field's class will
			/// be initialized, if it has not already been initialized.
			/// </para>
			/// </summary>
			/// <param name="f"> the reflected field </param>
			/// <returns> a method handle which can load values from the reflected field </returns>
			/// <exception cref="IllegalAccessException"> if access checking fails </exception>
			/// <exception cref="NullPointerException"> if the argument is null </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public MethodHandle unreflectGetter(Field f) throws IllegalAccessException
			public MethodHandle UnreflectGetter(Field f)
			{
				return UnreflectField(f, false);
			}
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: private MethodHandle unreflectField(Field f, boolean isSetter) throws IllegalAccessException
			internal MethodHandle UnreflectField(Field f, bool isSetter)
			{
				MemberName field = new MemberName(f, isSetter);
				assert(isSetter ? MethodHandleNatives.RefKindIsSetter(field.ReferenceKind) : MethodHandleNatives.RefKindIsGetter(field.ReferenceKind));
				Lookup lookup = f.Accessible ? IMPL_LOOKUP : this;
				return lookup.GetDirectFieldNoSecurityManager(field.ReferenceKind, f.DeclaringClass, field);
			}

			/// <summary>
			/// Produces a method handle giving write access to a reflected field.
			/// The type of the method handle will have a void return type.
			/// If the field is static, the method handle will take a single
			/// argument, of the field's value type, the value to be stored.
			/// Otherwise, the two arguments will be the instance containing
			/// the field, and the value to be stored.
			/// If the field's {@code accessible} flag is not set,
			/// access checking is performed immediately on behalf of the lookup class.
			/// <para>
			/// If the field is static, and
			/// if the returned method handle is invoked, the field's class will
			/// be initialized, if it has not already been initialized.
			/// </para>
			/// </summary>
			/// <param name="f"> the reflected field </param>
			/// <returns> a method handle which can store values into the reflected field </returns>
			/// <exception cref="IllegalAccessException"> if access checking fails </exception>
			/// <exception cref="NullPointerException"> if the argument is null </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public MethodHandle unreflectSetter(Field f) throws IllegalAccessException
			public MethodHandle UnreflectSetter(Field f)
			{
				return UnreflectField(f, true);
			}

			/// <summary>
			/// Cracks a <a href="MethodHandleInfo.html#directmh">direct method handle</a>
			/// created by this lookup object or a similar one.
			/// Security and access checks are performed to ensure that this lookup object
			/// is capable of reproducing the target method handle.
			/// This means that the cracking may fail if target is a direct method handle
			/// but was created by an unrelated lookup object.
			/// This can happen if the method handle is <a href="MethodHandles.Lookup.html#callsens">caller sensitive</a>
			/// and was created by a lookup object for a different class. </summary>
			/// <param name="target"> a direct method handle to crack into symbolic reference components </param>
			/// <returns> a symbolic reference which can be used to reconstruct this method handle from this lookup object </returns>
			/// <exception cref="SecurityException"> if a security manager is present and it
			///                              <a href="MethodHandles.Lookup.html#secmgr">refuses access</a> </exception>
			/// <exception cref="IllegalArgumentException"> if the target is not a direct method handle or if access checking fails </exception>
			/// <exception cref="NullPointerException"> if the target is {@code null} </exception>
			/// <seealso cref= MethodHandleInfo
			/// @since 1.8 </seealso>
			public MethodHandleInfo RevealDirect(MethodHandle target)
			{
				MemberName member = target.InternalMemberName();
				if (member == null || (!member.Resolved && !member.MethodHandleInvoke))
				{
					throw newIllegalArgumentException("not a direct method handle");
				}
				Class defc = member.DeclaringClass;
				sbyte refKind = member.ReferenceKind;
				assert(MethodHandleNatives.RefKindIsValid(refKind));
				if (refKind == REF_invokeSpecial && !target.InvokeSpecial)
					// Devirtualized method invocation is usually formally virtual.
					// To avoid creating extra MemberName objects for this common case,
					// we encode this extra degree of freedom using MH.isInvokeSpecial.
				{
					refKind = REF_invokeVirtual;
				}
				if (refKind == REF_invokeVirtual && defc.Interface)
				{
					// Symbolic reference is through interface but resolves to Object method (toString, etc.)
					refKind = REF_invokeInterface;
				}
				// Check SM permissions and member access before cracking.
				try
				{
					CheckAccess(refKind, defc, member);
					CheckSecurityManager(defc, member);
				}
				catch (IllegalAccessException ex)
				{
					throw new IllegalArgumentException(ex);
				}
				if (AllowedModes != TRUSTED && member.CallerSensitive)
				{
					Class callerClass = target.InternalCallerClass();
					if (!HasPrivateAccess() || callerClass != LookupClass())
					{
						throw new IllegalArgumentException("method handle is caller sensitive: " + callerClass);
					}
				}
				// Produce the handle to the results.
				return new InfoFromMemberName(this, member, refKind);
			}

			/// Helper methods, all package-private.

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: MemberName resolveOrFail(byte refKind, Class refc, String name, Class type) throws NoSuchFieldException, IllegalAccessException
			internal MemberName ResolveOrFail(sbyte refKind, Class refc, String name, Class type)
			{
				CheckSymbolicClass(refc); // do this before attempting to resolve
				name.GetType(); // NPE
				type.GetType(); // NPE
				return IMPL_NAMES.ResolveOrFail(refKind, new MemberName(refc, name, type, refKind), LookupClassOrNull(), typeof(NoSuchFieldException));
			}

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: MemberName resolveOrFail(byte refKind, Class refc, String name, MethodType type) throws NoSuchMethodException, IllegalAccessException
			internal MemberName ResolveOrFail(sbyte refKind, Class refc, String name, MethodType type)
			{
				CheckSymbolicClass(refc); // do this before attempting to resolve
				name.GetType(); // NPE
				type.GetType(); // NPE
				CheckMethodName(refKind, name); // NPE check on name
				return IMPL_NAMES.ResolveOrFail(refKind, new MemberName(refc, name, type, refKind), LookupClassOrNull(), typeof(NoSuchMethodException));
			}

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: MemberName resolveOrFail(byte refKind, MemberName member) throws ReflectiveOperationException
			internal MemberName ResolveOrFail(sbyte refKind, MemberName member)
			{
				CheckSymbolicClass(member.DeclaringClass); // do this before attempting to resolve
				member.Name.GetType(); // NPE
				member.Type.GetType(); // NPE
				return IMPL_NAMES.ResolveOrFail(refKind, member, LookupClassOrNull(), typeof(ReflectiveOperationException));
			}

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: void checkSymbolicClass(Class refc) throws IllegalAccessException
			internal void CheckSymbolicClass(Class refc)
			{
				refc.GetType(); // NPE
				Class caller = LookupClassOrNull();
				if (caller != null && !VerifyAccess.isClassAccessible(refc, caller, AllowedModes))
				{
					throw (new MemberName(refc)).MakeAccessException("symbolic reference class is not public", this);
				}
			}

			/// <summary>
			/// Check name for an illegal leading "&lt;" character. </summary>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: void checkMethodName(byte refKind, String name) throws NoSuchMethodException
			internal void CheckMethodName(sbyte refKind, String name)
			{
				if (name.StartsWith("<") && refKind != REF_newInvokeSpecial)
				{
					throw new NoSuchMethodException("illegal method name: " + name);
				}
			}


			/// <summary>
			/// Find my trustable caller class if m is a caller sensitive method.
			/// If this lookup object has private access, then the caller class is the lookupClass.
			/// Otherwise, if m is caller-sensitive, throw IllegalAccessException.
			/// </summary>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: Class findBoundCallerClass(MemberName m) throws IllegalAccessException
			internal Class FindBoundCallerClass(MemberName m)
			{
				Class callerClass = null;
				if (MethodHandleNatives.IsCallerSensitive(m))
				{
					// Only lookups with private access are allowed to resolve caller-sensitive methods
					if (HasPrivateAccess())
					{
						callerClass = LookupClass_Renamed;
					}
					else
					{
						throw new IllegalAccessException("Attempt to lookup caller-sensitive method using restricted lookup object");
					}
				}
				return callerClass;
			}

			internal bool HasPrivateAccess()
			{
				return (AllowedModes & PRIVATE) != 0;
			}

			/// <summary>
			/// Perform necessary <a href="MethodHandles.Lookup.html#secmgr">access checks</a>.
			/// Determines a trustable caller class to compare with refc, the symbolic reference class.
			/// If this lookup object has private access, then the caller class is the lookupClass.
			/// </summary>
			internal void CheckSecurityManager(Class refc, MemberName m)
			{
				SecurityManager smgr = System.SecurityManager;
				if (smgr == null)
				{
					return;
				}
				if (AllowedModes == TRUSTED)
				{
					return;
				}

				// Step 1:
				bool fullPowerLookup = HasPrivateAccess();
				if (!fullPowerLookup || !VerifyAccess.classLoaderIsAncestor(LookupClass_Renamed, refc))
				{
					ReflectUtil.checkPackageAccess(refc);
				}

				// Step 2:
				if (m.Public)
				{
					return;
				}
				if (!fullPowerLookup)
				{
					smgr.CheckPermission(SecurityConstants.CHECK_MEMBER_ACCESS_PERMISSION);
				}

				// Step 3:
				Class defc = m.DeclaringClass;
				if (!fullPowerLookup && defc != refc)
				{
					ReflectUtil.checkPackageAccess(defc);
				}
			}

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: void checkMethod(byte refKind, Class refc, MemberName m) throws IllegalAccessException
			internal void CheckMethod(sbyte refKind, Class refc, MemberName m)
			{
				bool wantStatic = (refKind == REF_invokeStatic);
				String message;
				if (m.Constructor)
				{
					message = "expected a method, not a constructor";
				}
				else if (!m.Method)
				{
					message = "expected a method";
				}
				else if (wantStatic != m.Static)
				{
					message = wantStatic ? "expected a static method" : "expected a non-static method";
				}
				else
				{
						CheckAccess(refKind, refc, m);
						return;
				}
				throw m.MakeAccessException(message, this);
			}

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: void checkField(byte refKind, Class refc, MemberName m) throws IllegalAccessException
			internal void CheckField(sbyte refKind, Class refc, MemberName m)
			{
				bool wantStatic = !MethodHandleNatives.RefKindHasReceiver(refKind);
				String message;
				if (wantStatic != m.Static)
				{
					message = wantStatic ? "expected a static field" : "expected a non-static field";
				}
				else
				{
						CheckAccess(refKind, refc, m);
						return;
				}
				throw m.MakeAccessException(message, this);
			}

			/// <summary>
			/// Check public/protected/private bits on the symbolic reference class and its member. </summary>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: void checkAccess(byte refKind, Class refc, MemberName m) throws IllegalAccessException
			internal void CheckAccess(sbyte refKind, Class refc, MemberName m)
			{
				assert(m.ReferenceKindIsConsistentWith(refKind) && MethodHandleNatives.RefKindIsValid(refKind) && (MethodHandleNatives.RefKindIsField(refKind) == m.Field));
				int allowedModes = this.AllowedModes;
				if (allowedModes == TRUSTED)
				{
					return;
				}
				int mods = m.Modifiers;
				if (Modifier.IsProtected(mods) && refKind == REF_invokeVirtual && m.DeclaringClass == typeof(Object) && m.Name.Equals("clone") && refc.Array)
				{
					// The JVM does this hack also.
					// (See ClassVerifier::verify_invoke_instructions
					// and LinkResolver::check_method_accessability.)
					// Because the JVM does not allow separate methods on array types,
					// there is no separate method for int[].clone.
					// All arrays simply inherit Object.clone.
					// But for access checking logic, we make Object.clone
					// (normally protected) appear to be public.
					// Later on, when the DirectMethodHandle is created,
					// its leading argument will be restricted to the
					// requested array type.
					// N.B. The return type is not adjusted, because
					// that is *not* the bytecode behavior.
					mods ^= Modifier.PROTECTED | Modifier.PUBLIC;
				}
				if (Modifier.IsProtected(mods) && refKind == REF_newInvokeSpecial)
				{
					// cannot "new" a protected ctor in a different package
					mods ^= Modifier.PROTECTED;
				}
				if (Modifier.IsFinal(mods) && MethodHandleNatives.RefKindIsSetter(refKind))
				{
					throw m.MakeAccessException("unexpected set of a final field", this);
				}
				if (Modifier.IsPublic(mods) && Modifier.IsPublic(refc.Modifiers) && allowedModes != 0)
				{
					return; // common case
				}
				int requestedModes = Fixmods(mods); // adjust 0 => PACKAGE
				if ((requestedModes & allowedModes) != 0)
				{
					if (VerifyAccess.isMemberAccessible(refc, m.DeclaringClass, mods, LookupClass(), allowedModes))
					{
						return;
					}
				}
				else
				{
					// Protected members can also be checked as if they were package-private.
					if ((requestedModes & PROTECTED) != 0 && (allowedModes & PACKAGE) != 0 && VerifyAccess.isSamePackage(m.DeclaringClass, LookupClass()))
					{
						return;
					}
				}
				throw m.MakeAccessException(AccessFailedMessage(refc, m), this);
			}

			internal String AccessFailedMessage(Class refc, MemberName m)
			{
				Class defc = m.DeclaringClass;
				int mods = m.Modifiers;
				// check the class first:
				bool classOK = (Modifier.IsPublic(defc.Modifiers) && (defc == refc || Modifier.IsPublic(refc.Modifiers)));
				if (!classOK && (AllowedModes & PACKAGE) != 0)
				{
					classOK = (VerifyAccess.isClassAccessible(defc, LookupClass(), ALL_MODES) && (defc == refc || VerifyAccess.isClassAccessible(refc, LookupClass(), ALL_MODES)));
				}
				if (!classOK)
				{
					return "class is not public";
				}
				if (Modifier.IsPublic(mods))
				{
					return "access to public member failed"; // (how?)
				}
				if (Modifier.IsPrivate(mods))
				{
					return "member is private";
				}
				if (Modifier.IsProtected(mods))
				{
					return "member is protected";
				}
				return "member is private to package";
			}

			internal const bool ALLOW_NESTMATE_ACCESS = false;

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: private void checkSpecialCaller(Class specialCaller) throws IllegalAccessException
			internal void CheckSpecialCaller(Class specialCaller)
			{
				int allowedModes = this.AllowedModes;
				if (allowedModes == TRUSTED)
				{
					return;
				}
				if (!HasPrivateAccess() || (specialCaller != LookupClass() && !(ALLOW_NESTMATE_ACCESS && VerifyAccess.isSamePackageMember(specialCaller, LookupClass()))))
				{
					throw (new MemberName(specialCaller)).MakeAccessException("no private access for invokespecial", this);
				}
			}

			internal bool RestrictProtectedReceiver(MemberName method)
			{
				// The accessing class only has the right to use a protected member
				// on itself or a subclass.  Enforce that restriction, from JVMS 5.4.4, etc.
				if (!method.Protected || method.Static || AllowedModes == TRUSTED || method.DeclaringClass == LookupClass() || VerifyAccess.isSamePackage(method.DeclaringClass, LookupClass()) || (ALLOW_NESTMATE_ACCESS && VerifyAccess.isSamePackageMember(method.DeclaringClass, LookupClass())))
				{
					return false;
				}
				return true;
			}
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: private MethodHandle restrictReceiver(MemberName method, DirectMethodHandle mh, Class caller) throws IllegalAccessException
			internal MethodHandle RestrictReceiver(MemberName method, DirectMethodHandle mh, Class caller)
			{
				assert(!method.Static);
				// receiver type of mh is too wide; narrow to caller
				if (!caller.IsSubclassOf(method.DeclaringClass))
				{
					throw method.MakeAccessException("caller class must be a subclass below the method", caller);
				}
				MethodType rawType = mh.Type();
				if (rawType.ParameterType(0) == caller)
				{
					return mh;
				}
				MethodType narrowType = rawType.ChangeParameterType(0, caller);
				assert(!mh.VarargsCollector); // viewAsType will lose varargs-ness
				assert(mh.ViewAsTypeChecks(narrowType, true));
				return mh.CopyWith(narrowType, mh.Form);
			}

			/// <summary>
			/// Check access and get the requested method. </summary>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: private MethodHandle getDirectMethod(byte refKind, Class refc, MemberName method, Class callerClass) throws IllegalAccessException
			internal MethodHandle GetDirectMethod(sbyte refKind, Class refc, MemberName method, Class callerClass)
			{
				const bool doRestrict = true;
				const bool checkSecurity = true;
				return GetDirectMethodCommon(refKind, refc, method, checkSecurity, doRestrict, callerClass);
			}
			/// <summary>
			/// Check access and get the requested method, eliding receiver narrowing rules. </summary>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: private MethodHandle getDirectMethodNoRestrict(byte refKind, Class refc, MemberName method, Class callerClass) throws IllegalAccessException
			internal MethodHandle GetDirectMethodNoRestrict(sbyte refKind, Class refc, MemberName method, Class callerClass)
			{
				const bool doRestrict = false;
				const bool checkSecurity = true;
				return GetDirectMethodCommon(refKind, refc, method, checkSecurity, doRestrict, callerClass);
			}
			/// <summary>
			/// Check access and get the requested method, eliding security manager checks. </summary>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: private MethodHandle getDirectMethodNoSecurityManager(byte refKind, Class refc, MemberName method, Class callerClass) throws IllegalAccessException
			internal MethodHandle GetDirectMethodNoSecurityManager(sbyte refKind, Class refc, MemberName method, Class callerClass)
			{
				const bool doRestrict = true;
				const bool checkSecurity = false; // not needed for reflection or for linking CONSTANT_MH constants
				return GetDirectMethodCommon(refKind, refc, method, checkSecurity, doRestrict, callerClass);
			}
			/// <summary>
			/// Common code for all methods; do not call directly except from immediately above. </summary>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: private MethodHandle getDirectMethodCommon(byte refKind, Class refc, MemberName method, boolean checkSecurity, boolean doRestrict, Class callerClass) throws IllegalAccessException
			internal MethodHandle GetDirectMethodCommon(sbyte refKind, Class refc, MemberName method, bool checkSecurity, bool doRestrict, Class callerClass)
			{
				CheckMethod(refKind, refc, method);
				// Optionally check with the security manager; this isn't needed for unreflect* calls.
				if (checkSecurity)
				{
					CheckSecurityManager(refc, method);
				}
				assert(!method.MethodHandleInvoke);

				if (refKind == REF_invokeSpecial && refc != LookupClass() && !refc.Interface && refc != LookupClass().BaseType && LookupClass().IsSubclassOf(refc))
				{
					assert(!method.Name.Equals("<init>")); // not this code path
					// Per JVMS 6.5, desc. of invokespecial instruction:
					// If the method is in a superclass of the LC,
					// and if our original search was above LC.super,
					// repeat the search (symbolic lookup) from LC.super
					// and continue with the direct superclass of that class,
					// and so forth, until a match is found or no further superclasses exist.
					// FIXME: MemberName.resolve should handle this instead.
					Class refcAsSuper = LookupClass();
					MemberName m2;
					do
					{
						refcAsSuper = refcAsSuper.BaseType;
						m2 = new MemberName(refcAsSuper, method.Name, method.MethodType, REF_invokeSpecial);
						m2 = IMPL_NAMES.ResolveOrNull(refKind, m2, LookupClassOrNull());
					} while (m2 == null && refc != refcAsSuper); // search up to refc -  no method is found yet
					if (m2 == null)
					{
						throw new InternalError(method.ToString());
					}
					method = m2;
					refc = refcAsSuper;
					// redo basic checks
					CheckMethod(refKind, refc, method);
				}

				DirectMethodHandle dmh = DirectMethodHandle.Make(refKind, refc, method);
				MethodHandle mh = dmh;
				// Optionally narrow the receiver argument to refc using restrictReceiver.
				if (doRestrict && (refKind == REF_invokeSpecial || (MethodHandleNatives.RefKindHasReceiver(refKind) && RestrictProtectedReceiver(method))))
				{
					mh = RestrictReceiver(method, dmh, LookupClass());
				}
				mh = MaybeBindCaller(method, mh, callerClass);
				mh = mh.setVarargs(method);
				return mh;
			}
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: private MethodHandle maybeBindCaller(MemberName method, MethodHandle mh, Class callerClass) throws IllegalAccessException
			internal MethodHandle MaybeBindCaller(MemberName method, MethodHandle mh, Class callerClass)
			{
				if (AllowedModes == TRUSTED || !MethodHandleNatives.IsCallerSensitive(method))
				{
					return mh;
				}
				Class hostClass = LookupClass_Renamed;
				if (!HasPrivateAccess()) // caller must have private access
				{
					hostClass = callerClass; // callerClass came from a security manager style stack walk
				}
				MethodHandle cbmh = MethodHandleImpl.BindCaller(mh, hostClass);
				// Note: caller will apply varargs after this step happens.
				return cbmh;
			}
			/// <summary>
			/// Check access and get the requested field. </summary>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: private MethodHandle getDirectField(byte refKind, Class refc, MemberName field) throws IllegalAccessException
			internal MethodHandle GetDirectField(sbyte refKind, Class refc, MemberName field)
			{
				const bool checkSecurity = true;
				return GetDirectFieldCommon(refKind, refc, field, checkSecurity);
			}
			/// <summary>
			/// Check access and get the requested field, eliding security manager checks. </summary>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: private MethodHandle getDirectFieldNoSecurityManager(byte refKind, Class refc, MemberName field) throws IllegalAccessException
			internal MethodHandle GetDirectFieldNoSecurityManager(sbyte refKind, Class refc, MemberName field)
			{
				const bool checkSecurity = false; // not needed for reflection or for linking CONSTANT_MH constants
				return GetDirectFieldCommon(refKind, refc, field, checkSecurity);
			}
			/// <summary>
			/// Common code for all fields; do not call directly except from immediately above. </summary>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: private MethodHandle getDirectFieldCommon(byte refKind, Class refc, MemberName field, boolean checkSecurity) throws IllegalAccessException
			internal MethodHandle GetDirectFieldCommon(sbyte refKind, Class refc, MemberName field, bool checkSecurity)
			{
				CheckField(refKind, refc, field);
				// Optionally check with the security manager; this isn't needed for unreflect* calls.
				if (checkSecurity)
				{
					CheckSecurityManager(refc, field);
				}
				DirectMethodHandle dmh = DirectMethodHandle.Make(refc, field);
				bool doRestrict = (MethodHandleNatives.RefKindHasReceiver(refKind) && RestrictProtectedReceiver(field));
				if (doRestrict)
				{
					return RestrictReceiver(field, dmh, LookupClass());
				}
				return dmh;
			}
			/// <summary>
			/// Check access and get the requested constructor. </summary>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: private MethodHandle getDirectConstructor(Class refc, MemberName ctor) throws IllegalAccessException
			internal MethodHandle GetDirectConstructor(Class refc, MemberName ctor)
			{
				const bool checkSecurity = true;
				return GetDirectConstructorCommon(refc, ctor, checkSecurity);
			}
			/// <summary>
			/// Check access and get the requested constructor, eliding security manager checks. </summary>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: private MethodHandle getDirectConstructorNoSecurityManager(Class refc, MemberName ctor) throws IllegalAccessException
			internal MethodHandle GetDirectConstructorNoSecurityManager(Class refc, MemberName ctor)
			{
				const bool checkSecurity = false; // not needed for reflection or for linking CONSTANT_MH constants
				return GetDirectConstructorCommon(refc, ctor, checkSecurity);
			}
			/// <summary>
			/// Common code for all constructors; do not call directly except from immediately above. </summary>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: private MethodHandle getDirectConstructorCommon(Class refc, MemberName ctor, boolean checkSecurity) throws IllegalAccessException
			internal MethodHandle GetDirectConstructorCommon(Class refc, MemberName ctor, bool checkSecurity)
			{
				assert(ctor.Constructor);
				CheckAccess(REF_newInvokeSpecial, refc, ctor);
				// Optionally check with the security manager; this isn't needed for unreflect* calls.
				if (checkSecurity)
				{
					CheckSecurityManager(refc, ctor);
				}
				assert(!MethodHandleNatives.IsCallerSensitive(ctor)); // maybeBindCaller not relevant here
				return DirectMethodHandle.Make(ctor).setVarargs(ctor);
			}

			/// <summary>
			/// Hook called from the JVM (via MethodHandleNatives) to link MH constants:
			/// </summary>
			/*non-public*/
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: MethodHandle linkMethodHandleConstant(byte refKind, Class defc, String name, Object type) throws ReflectiveOperationException
			internal MethodHandle LinkMethodHandleConstant(sbyte refKind, Class defc, String name, Object type)
			{
				if (!(type is Class || type is MethodType))
				{
					throw new InternalError("unresolved MemberName");
				}
				MemberName member = new MemberName(refKind, defc, name, type);
				MethodHandle mh = LOOKASIDE_TABLE[member];
				if (mh != null)
				{
					CheckSymbolicClass(defc);
					return mh;
				}
				// Treat MethodHandle.invoke and invokeExact specially.
				if (defc == typeof(MethodHandle) && refKind == REF_invokeVirtual)
				{
					mh = FindVirtualForMH(member.Name, member.MethodType);
					if (mh != null)
					{
						return mh;
					}
				}
				MemberName resolved = ResolveOrFail(refKind, member);
				mh = GetDirectMethodForConstant(refKind, defc, resolved);
				if (mh is DirectMethodHandle && CanBeCached(refKind, defc, resolved))
				{
					MemberName key = mh.InternalMemberName();
					if (key != null)
					{
						key = key.AsNormalOriginal();
					}
					if (member.Equals(key)) // better safe than sorry
					{
						LOOKASIDE_TABLE[key] = (DirectMethodHandle) mh;
					}
				}
				return mh;
			}
			internal bool CanBeCached(sbyte refKind, Class defc, MemberName member)
			{
				if (refKind == REF_invokeSpecial)
				{
					return false;
				}
				if (!Modifier.IsPublic(defc.Modifiers) || !Modifier.IsPublic(member.DeclaringClass.Modifiers) || !member.Public || member.CallerSensitive)
				{
					return false;
				}
				ClassLoader loader = defc.ClassLoader;
				if (!sun.misc.VM.isSystemDomainLoader(loader))
				{
					ClassLoader sysl = ClassLoader.SystemClassLoader;
					bool found = false;
					while (sysl != null)
					{
						if (loader == sysl)
						{
							found = true;
							break;
						}
						sysl = sysl.Parent;
					}
					if (!found)
					{
						return false;
					}
				}
				try
				{
					MemberName resolved2 = PublicLookup().ResolveOrFail(refKind, new MemberName(refKind, defc, member.Name, member.Type));
					CheckSecurityManager(defc, resolved2);
				}
//JAVA TO C# CONVERTER TODO TASK: There is no equivalent in C# to Java 'multi-catch' syntax:
				catch (ReflectiveOperationException | SecurityException ex)
				{
					return false;
				}
				return true;
			}
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: private MethodHandle getDirectMethodForConstant(byte refKind, Class defc, MemberName member) throws ReflectiveOperationException
			internal MethodHandle GetDirectMethodForConstant(sbyte refKind, Class defc, MemberName member)
			{
				if (MethodHandleNatives.RefKindIsField(refKind))
				{
					return GetDirectFieldNoSecurityManager(refKind, defc, member);
				}
				else if (MethodHandleNatives.RefKindIsMethod(refKind))
				{
					return GetDirectMethodNoSecurityManager(refKind, defc, member, LookupClass_Renamed);
				}
				else if (refKind == REF_newInvokeSpecial)
				{
					return GetDirectConstructorNoSecurityManager(defc, member);
				}
				// oops
				throw newIllegalArgumentException("bad MethodHandle constant #" + member);
			}

			internal static ConcurrentDictionary<MemberName, DirectMethodHandle> LOOKASIDE_TABLE = new ConcurrentDictionary<MemberName, DirectMethodHandle>();
		}

		/// <summary>
		/// Produces a method handle giving read access to elements of an array.
		/// The type of the method handle will have a return type of the array's
		/// element type.  Its first argument will be the array type,
		/// and the second will be {@code int}. </summary>
		/// <param name="arrayClass"> an array type </param>
		/// <returns> a method handle which can load values from the given array type </returns>
		/// <exception cref="NullPointerException"> if the argument is null </exception>
		/// <exception cref="IllegalArgumentException"> if arrayClass is not an array type </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public static MethodHandle arrayElementGetter(Class arrayClass) throws IllegalArgumentException
		public static MethodHandle ArrayElementGetter(Class arrayClass)
		{
			return MethodHandleImpl.MakeArrayElementAccessor(arrayClass, false);
		}

		/// <summary>
		/// Produces a method handle giving write access to elements of an array.
		/// The type of the method handle will have a void return type.
		/// Its last argument will be the array's element type.
		/// The first and second arguments will be the array type and int. </summary>
		/// <param name="arrayClass"> the class of an array </param>
		/// <returns> a method handle which can store values into the array type </returns>
		/// <exception cref="NullPointerException"> if the argument is null </exception>
		/// <exception cref="IllegalArgumentException"> if arrayClass is not an array type </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public static MethodHandle arrayElementSetter(Class arrayClass) throws IllegalArgumentException
		public static MethodHandle ArrayElementSetter(Class arrayClass)
		{
			return MethodHandleImpl.MakeArrayElementAccessor(arrayClass, true);
		}

		/// method handle invocation (reflective style)

		/// <summary>
		/// Produces a method handle which will invoke any method handle of the
		/// given {@code type}, with a given number of trailing arguments replaced by
		/// a single trailing {@code Object[]} array.
		/// The resulting invoker will be a method handle with the following
		/// arguments:
		/// <ul>
		/// <li>a single {@code MethodHandle} target
		/// <li>zero or more leading values (counted by {@code leadingArgCount})
		/// <li>an {@code Object[]} array containing trailing arguments
		/// </ul>
		/// <para>
		/// The invoker will invoke its target like a call to <seealso cref="MethodHandle#invoke invoke"/> with
		/// the indicated {@code type}.
		/// That is, if the target is exactly of the given {@code type}, it will behave
		/// like {@code invokeExact}; otherwise it behave as if <seealso cref="MethodHandle#asType asType"/>
		/// is used to convert the target to the required {@code type}.
		/// </para>
		/// <para>
		/// The type of the returned invoker will not be the given {@code type}, but rather
		/// will have all parameters except the first {@code leadingArgCount}
		/// replaced by a single array of type {@code Object[]}, which will be
		/// the final parameter.
		/// </para>
		/// <para>
		/// Before invoking its target, the invoker will spread the final array, apply
		/// reference casts as necessary, and unbox and widen primitive arguments.
		/// If, when the invoker is called, the supplied array argument does
		/// not have the correct number of elements, the invoker will throw
		/// an <seealso cref="IllegalArgumentException"/> instead of invoking the target.
		/// </para>
		/// <para>
		/// This method is equivalent to the following code (though it may be more efficient):
		/// <blockquote><pre>{@code
		/// MethodHandle invoker = MethodHandles.invoker(type);
		/// int spreadArgCount = type.parameterCount() - leadingArgCount;
		/// invoker = invoker.asSpreader(Object[].class, spreadArgCount);
		/// return invoker;
		/// }</pre></blockquote>
		/// This method throws no reflective or security exceptions.
		/// </para>
		/// </summary>
		/// <param name="type"> the desired target type </param>
		/// <param name="leadingArgCount"> number of fixed arguments, to be passed unchanged to the target </param>
		/// <returns> a method handle suitable for invoking any method handle of the given type </returns>
		/// <exception cref="NullPointerException"> if {@code type} is null </exception>
		/// <exception cref="IllegalArgumentException"> if {@code leadingArgCount} is not in
		///                  the range from 0 to {@code type.parameterCount()} inclusive,
		///                  or if the resulting method handle's type would have
		///          <a href="MethodHandle.html#maxarity">too many parameters</a> </exception>
		public static MethodHandle SpreadInvoker(MethodType type, int leadingArgCount)
		{
			if (leadingArgCount < 0 || leadingArgCount > type.ParameterCount())
			{
				throw newIllegalArgumentException("bad argument count", leadingArgCount);
			}
			type = type.AsSpreaderType(typeof(Object[]), type.ParameterCount() - leadingArgCount);
			return type.Invokers().SpreadInvoker(leadingArgCount);
		}

		/// <summary>
		/// Produces a special <em>invoker method handle</em> which can be used to
		/// invoke any method handle of the given type, as if by <seealso cref="MethodHandle#invokeExact invokeExact"/>.
		/// The resulting invoker will have a type which is
		/// exactly equal to the desired type, except that it will accept
		/// an additional leading argument of type {@code MethodHandle}.
		/// <para>
		/// This method is equivalent to the following code (though it may be more efficient):
		/// {@code publicLookup().findVirtual(MethodHandle.class, "invokeExact", type)}
		/// 
		/// <p style="font-size:smaller;">
		/// <em>Discussion:</em>
		/// Invoker method handles can be useful when working with variable method handles
		/// of unknown types.
		/// For example, to emulate an {@code invokeExact} call to a variable method
		/// handle {@code M}, extract its type {@code T},
		/// look up the invoker method {@code X} for {@code T},
		/// and call the invoker method, as {@code X.invoke(T, A...)}.
		/// (It would not work to call {@code X.invokeExact}, since the type {@code T}
		/// is unknown.)
		/// If spreading, collecting, or other argument transformations are required,
		/// they can be applied once to the invoker {@code X} and reused on many {@code M}
		/// method handle values, as long as they are compatible with the type of {@code X}.
		/// <p style="font-size:smaller;">
		/// <em>(Note:  The invoker method is not available via the Core Reflection API.
		/// An attempt to call <seealso cref="java.lang.reflect.Method#invoke java.lang.reflect.Method.invoke"/>
		/// on the declared {@code invokeExact} or {@code invoke} method will raise an
		/// <seealso cref="java.lang.UnsupportedOperationException UnsupportedOperationException"/>.)</em>
		/// </para>
		/// <para>
		/// This method throws no reflective or security exceptions.
		/// </para>
		/// </summary>
		/// <param name="type"> the desired target type </param>
		/// <returns> a method handle suitable for invoking any method handle of the given type </returns>
		/// <exception cref="IllegalArgumentException"> if the resulting method handle's type would have
		///          <a href="MethodHandle.html#maxarity">too many parameters</a> </exception>
		public static MethodHandle ExactInvoker(MethodType type)
		{
			return type.Invokers().ExactInvoker();
		}

		/// <summary>
		/// Produces a special <em>invoker method handle</em> which can be used to
		/// invoke any method handle compatible with the given type, as if by <seealso cref="MethodHandle#invoke invoke"/>.
		/// The resulting invoker will have a type which is
		/// exactly equal to the desired type, except that it will accept
		/// an additional leading argument of type {@code MethodHandle}.
		/// <para>
		/// Before invoking its target, if the target differs from the expected type,
		/// the invoker will apply reference casts as
		/// necessary and box, unbox, or widen primitive values, as if by <seealso cref="MethodHandle#asType asType"/>.
		/// Similarly, the return value will be converted as necessary.
		/// If the target is a <seealso cref="MethodHandle#asVarargsCollector variable arity method handle"/>,
		/// the required arity conversion will be made, again as if by <seealso cref="MethodHandle#asType asType"/>.
		/// </para>
		/// <para>
		/// This method is equivalent to the following code (though it may be more efficient):
		/// {@code publicLookup().findVirtual(MethodHandle.class, "invoke", type)}
		/// <p style="font-size:smaller;">
		/// <em>Discussion:</em>
		/// A <seealso cref="MethodType#genericMethodType general method type"/> is one which
		/// mentions only {@code Object} arguments and return values.
		/// An invoker for such a type is capable of calling any method handle
		/// of the same arity as the general type.
		/// <p style="font-size:smaller;">
		/// <em>(Note:  The invoker method is not available via the Core Reflection API.
		/// An attempt to call <seealso cref="java.lang.reflect.Method#invoke java.lang.reflect.Method.invoke"/>
		/// on the declared {@code invokeExact} or {@code invoke} method will raise an
		/// <seealso cref="java.lang.UnsupportedOperationException UnsupportedOperationException"/>.)</em>
		/// </para>
		/// <para>
		/// This method throws no reflective or security exceptions.
		/// </para>
		/// </summary>
		/// <param name="type"> the desired target type </param>
		/// <returns> a method handle suitable for invoking any method handle convertible to the given type </returns>
		/// <exception cref="IllegalArgumentException"> if the resulting method handle's type would have
		///          <a href="MethodHandle.html#maxarity">too many parameters</a> </exception>
		public static MethodHandle Invoker(MethodType type)
		{
			return type.Invokers().GenericInvoker();
		}

		internal static MethodHandle BasicInvoker(MethodType type) //non-public
		{
			return type.Invokers().BasicInvoker();
		}

		 /// method handle modification (creation from other method handles)

		/// <summary>
		/// Produces a method handle which adapts the type of the
		/// given method handle to a new type by pairwise argument and return type conversion.
		/// The original type and new type must have the same number of arguments.
		/// The resulting method handle is guaranteed to report a type
		/// which is equal to the desired new type.
		/// <para>
		/// If the original type and new type are equal, returns target.
		/// </para>
		/// <para>
		/// The same conversions are allowed as for <seealso cref="MethodHandle#asType MethodHandle.asType"/>,
		/// and some additional conversions are also applied if those conversions fail.
		/// Given types <em>T0</em>, <em>T1</em>, one of the following conversions is applied
		/// if possible, before or instead of any conversions done by {@code asType}:
		/// <ul>
		/// <li>If <em>T0</em> and <em>T1</em> are references, and <em>T1</em> is an interface type,
		///     then the value of type <em>T0</em> is passed as a <em>T1</em> without a cast.
		///     (This treatment of interfaces follows the usage of the bytecode verifier.)
		/// <li>If <em>T0</em> is boolean and <em>T1</em> is another primitive,
		///     the boolean is converted to a byte value, 1 for true, 0 for false.
		///     (This treatment follows the usage of the bytecode verifier.)
		/// <li>If <em>T1</em> is boolean and <em>T0</em> is another primitive,
		///     <em>T0</em> is converted to byte via Java casting conversion (JLS 5.5),
		///     and the low order bit of the result is tested, as if by {@code (x & 1) != 0}.
		/// <li>If <em>T0</em> and <em>T1</em> are primitives other than boolean,
		///     then a Java casting conversion (JLS 5.5) is applied.
		///     (Specifically, <em>T0</em> will convert to <em>T1</em> by
		///     widening and/or narrowing.)
		/// <li>If <em>T0</em> is a reference and <em>T1</em> a primitive, an unboxing
		///     conversion will be applied at runtime, possibly followed
		///     by a Java casting conversion (JLS 5.5) on the primitive value,
		///     possibly followed by a conversion from byte to boolean by testing
		///     the low-order bit.
		/// <li>If <em>T0</em> is a reference and <em>T1</em> a primitive,
		///     and if the reference is null at runtime, a zero value is introduced.
		/// </ul>
		/// </para>
		/// </summary>
		/// <param name="target"> the method handle to invoke after arguments are retyped </param>
		/// <param name="newType"> the expected type of the new method handle </param>
		/// <returns> a method handle which delegates to the target after performing
		///           any necessary argument conversions, and arranges for any
		///           necessary return value conversions </returns>
		/// <exception cref="NullPointerException"> if either argument is null </exception>
		/// <exception cref="WrongMethodTypeException"> if the conversion cannot be made </exception>
		/// <seealso cref= MethodHandle#asType </seealso>
		public static MethodHandle ExplicitCastArguments(MethodHandle target, MethodType newType)
		{
			ExplicitCastArgumentsChecks(target, newType);
			// use the asTypeCache when possible:
			MethodType oldType = target.Type();
			if (oldType == newType)
			{
				return target;
			}
			if (oldType.ExplicitCastEquivalentToAsType(newType))
			{
				return target.AsFixedArity().AsType(newType);
			}
			return MethodHandleImpl.MakePairwiseConvert(target, newType, false);
		}

		private static void ExplicitCastArgumentsChecks(MethodHandle target, MethodType newType)
		{
			if (target.Type().ParameterCount() != newType.ParameterCount())
			{
				throw new WrongMethodTypeException("cannot explicitly cast " + target + " to " + newType);
			}
		}

		/// <summary>
		/// Produces a method handle which adapts the calling sequence of the
		/// given method handle to a new type, by reordering the arguments.
		/// The resulting method handle is guaranteed to report a type
		/// which is equal to the desired new type.
		/// <para>
		/// The given array controls the reordering.
		/// Call {@code #I} the number of incoming parameters (the value
		/// {@code newType.parameterCount()}, and call {@code #O} the number
		/// of outgoing parameters (the value {@code target.type().parameterCount()}).
		/// Then the length of the reordering array must be {@code #O},
		/// and each element must be a non-negative number less than {@code #I}.
		/// For every {@code N} less than {@code #O}, the {@code N}-th
		/// outgoing argument will be taken from the {@code I}-th incoming
		/// argument, where {@code I} is {@code reorder[N]}.
		/// </para>
		/// <para>
		/// No argument or return value conversions are applied.
		/// The type of each incoming argument, as determined by {@code newType},
		/// must be identical to the type of the corresponding outgoing parameter
		/// or parameters in the target method handle.
		/// The return type of {@code newType} must be identical to the return
		/// type of the original target.
		/// </para>
		/// <para>
		/// The reordering array need not specify an actual permutation.
		/// An incoming argument will be duplicated if its index appears
		/// more than once in the array, and an incoming argument will be dropped
		/// if its index does not appear in the array.
		/// As in the case of <seealso cref="#dropArguments(MethodHandle,int,List) dropArguments"/>,
		/// incoming arguments which are not mentioned in the reordering array
		/// are may be any type, as determined only by {@code newType}.
		/// <blockquote><pre>{@code
		/// import static java.lang.invoke.MethodHandles.*;
		/// import static java.lang.invoke.MethodType.*;
		/// ...
		/// MethodType intfn1 = methodType(int.class, int.class);
		/// MethodType intfn2 = methodType(int.class, int.class, int.class);
		/// MethodHandle sub = ... (int x, int y) -> (x-y) ...;
		/// assert(sub.type().equals(intfn2));
		/// MethodHandle sub1 = permuteArguments(sub, intfn2, 0, 1);
		/// MethodHandle rsub = permuteArguments(sub, intfn2, 1, 0);
		/// assert((int)rsub.invokeExact(1, 100) == 99);
		/// MethodHandle add = ... (int x, int y) -> (x+y) ...;
		/// assert(add.type().equals(intfn2));
		/// MethodHandle twice = permuteArguments(add, intfn1, 0, 0);
		/// assert(twice.type().equals(intfn1));
		/// assert((int)twice.invokeExact(21) == 42);
		/// }</pre></blockquote>
		/// </para>
		/// </summary>
		/// <param name="target"> the method handle to invoke after arguments are reordered </param>
		/// <param name="newType"> the expected type of the new method handle </param>
		/// <param name="reorder"> an index array which controls the reordering </param>
		/// <returns> a method handle which delegates to the target after it
		///           drops unused arguments and moves and/or duplicates the other arguments </returns>
		/// <exception cref="NullPointerException"> if any argument is null </exception>
		/// <exception cref="IllegalArgumentException"> if the index array length is not equal to
		///                  the arity of the target, or if any index array element
		///                  not a valid index for a parameter of {@code newType},
		///                  or if two corresponding parameter types in
		///                  {@code target.type()} and {@code newType} are not identical, </exception>
		public static MethodHandle PermuteArguments(MethodHandle target, MethodType newType, params int[] reorder)
		{
			reorder = reorder.clone(); // get a private copy
			MethodType oldType = target.Type();
			PermuteArgumentChecks(reorder, newType, oldType);
			// first detect dropped arguments and handle them separately
			int[] originalReorder = reorder;
			BoundMethodHandle result = target.Rebind();
			LambdaForm form = result.Form;
			int newArity = newType.ParameterCount();
			// Normalize the reordering into a real permutation,
			// by removing duplicates and adding dropped elements.
			// This somewhat improves lambda form caching, as well
			// as simplifying the transform by breaking it up into steps.
			for (int ddIdx; (ddIdx = FindFirstDupOrDrop(reorder, newArity)) != 0;)
			{
				if (ddIdx > 0)
				{
					// We found a duplicated entry at reorder[ddIdx].
					// Example:  (x,y,z)->asList(x,y,z)
					// permuted by [1*,0,1] => (a0,a1)=>asList(a1,a0,a1)
					// permuted by [0,1,0*] => (a0,a1)=>asList(a0,a1,a0)
					// The starred element corresponds to the argument
					// deleted by the dupArgumentForm transform.
					int srcPos = ddIdx, dstPos = srcPos, dupVal = reorder[srcPos];
					bool killFirst = false;
					for (int val; (val = reorder[--dstPos]) != dupVal;)
					{
						// Set killFirst if the dup is larger than an intervening position.
						// This will remove at least one inversion from the permutation.
						if (dupVal > val)
						{
							killFirst = true;
						}
					}
					if (!killFirst)
					{
						srcPos = dstPos;
						dstPos = ddIdx;
					}
					form = form.Editor().DupArgumentForm(1 + srcPos, 1 + dstPos);
					assert(reorder[srcPos] == reorder[dstPos]);
					oldType = oldType.DropParameterTypes(dstPos, dstPos + 1);
					// contract the reordering by removing the element at dstPos
					int tailPos = dstPos + 1;
					System.Array.Copy(reorder, tailPos, reorder, dstPos, reorder.Length - tailPos);
					reorder = Arrays.CopyOf(reorder, reorder.Length - 1);
				}
				else
				{
					int dropVal = ~ddIdx, insPos = 0;
					while (insPos < reorder.Length && reorder[insPos] < dropVal)
					{
						// Find first element of reorder larger than dropVal.
						// This is where we will insert the dropVal.
						insPos += 1;
					}
					Class ptype = newType.ParameterType(dropVal);
					form = form.Editor().AddArgumentForm(1 + insPos, BasicType.basicType(ptype));
					oldType = oldType.InsertParameterTypes(insPos, ptype);
					// expand the reordering by inserting an element at insPos
					int tailPos = insPos + 1;
					reorder = Arrays.CopyOf(reorder, reorder.Length + 1);
					System.Array.Copy(reorder, insPos, reorder, tailPos, reorder.Length - tailPos);
					reorder[insPos] = dropVal;
				}
				assert(PermuteArgumentChecks(reorder, newType, oldType));
			}
			assert(reorder.Length == newArity); // a perfect permutation
			// Note:  This may cache too many distinct LFs. Consider backing off to varargs code.
			form = form.Editor().PermuteArgumentsForm(1, reorder);
			if (newType == result.Type() && form == result.InternalForm())
			{
				return result;
			}
			return result.CopyWith(newType, form);
		}

		/// <summary>
		/// Return an indication of any duplicate or omission in reorder.
		/// If the reorder contains a duplicate entry, return the index of the second occurrence.
		/// Otherwise, return ~(n), for the first n in [0..newArity-1] that is not present in reorder.
		/// Otherwise, return zero.
		/// If an element not in [0..newArity-1] is encountered, return reorder.length.
		/// </summary>
		private static int FindFirstDupOrDrop(int[] reorder, int newArity)
		{
			const int BIT_LIMIT = 63; // max number of bits in bit mask
			if (newArity < BIT_LIMIT)
			{
				long mask = 0;
				for (int i = 0; i < reorder.Length; i++)
				{
					int arg = reorder[i];
					if (arg >= newArity)
					{
						return reorder.Length;
					}
					long bit = 1L << arg;
					if ((mask & bit) != 0)
					{
						return i; // >0 indicates a dup
					}
					mask |= bit;
				}
				if (mask == (1L << newArity) - 1)
				{
					assert(Long.NumberOfTrailingZeros(Long.LowestOneBit(~mask)) == newArity);
					return 0;
				}
				// find first zero
				long zeroBit = Long.LowestOneBit(~mask);
				int zeroPos = Long.NumberOfTrailingZeros(zeroBit);
				assert(zeroPos <= newArity);
				if (zeroPos == newArity)
				{
					return 0;
				}
				return ~zeroPos;
			}
			else
			{
				// same algorithm, different bit set
				BitArray mask = new BitArray(newArity);
				for (int i = 0; i < reorder.Length; i++)
				{
					int arg = reorder[i];
					if (arg >= newArity)
					{
						return reorder.Length;
					}
					if (mask.Get(arg))
					{
						return i; // >0 indicates a dup
					}
					mask.Set(arg, true);
				}
				int zeroPos = mask.nextClearBit(0);
				assert(zeroPos <= newArity);
				if (zeroPos == newArity)
				{
					return 0;
				}
				return ~zeroPos;
			}
		}

		private static bool PermuteArgumentChecks(int[] reorder, MethodType newType, MethodType oldType)
		{
			if (newType.ReturnType() != oldType.ReturnType())
			{
				throw newIllegalArgumentException("return types do not match", oldType, newType);
			}
			if (reorder.Length == oldType.ParameterCount())
			{
				int limit = newType.ParameterCount();
				bool bad = false;
				for (int j = 0; j < reorder.Length; j++)
				{
					int i = reorder[j];
					if (i < 0 || i >= limit)
					{
						bad = true;
						break;
					}
					Class src = newType.ParameterType(i);
					Class dst = oldType.ParameterType(j);
					if (src != dst)
					{
						throw newIllegalArgumentException("parameter types do not match after reorder", oldType, newType);
					}
				}
				if (!bad)
				{
					return true;
				}
			}
			throw newIllegalArgumentException("bad reorder array: " + Arrays.ToString(reorder));
		}

		/// <summary>
		/// Produces a method handle of the requested return type which returns the given
		/// constant value every time it is invoked.
		/// <para>
		/// Before the method handle is returned, the passed-in value is converted to the requested type.
		/// If the requested type is primitive, widening primitive conversions are attempted,
		/// else reference conversions are attempted.
		/// </para>
		/// <para>The returned method handle is equivalent to {@code identity(type).bindTo(value)}.
		/// </para>
		/// </summary>
		/// <param name="type"> the return type of the desired method handle </param>
		/// <param name="value"> the value to return </param>
		/// <returns> a method handle of the given return type and no arguments, which always returns the given value </returns>
		/// <exception cref="NullPointerException"> if the {@code type} argument is null </exception>
		/// <exception cref="ClassCastException"> if the value cannot be converted to the required return type </exception>
		/// <exception cref="IllegalArgumentException"> if the given type is {@code void.class} </exception>
		public static MethodHandle Constant(Class type, Object value)
		{
			if (type.Primitive)
			{
				if (type == typeof(void))
				{
					throw newIllegalArgumentException("void type");
				}
				Wrapper w = Wrapper.forPrimitiveType(type);
				value = w.Convert(value, type);
				if (w.zero().Equals(value))
				{
					return Zero(w, type);
				}
				return InsertArguments(Identity(type), 0, value);
			}
			else
			{
				if (value == null)
				{
					return Zero(Wrapper.OBJECT, type);
				}
				return Identity(type).BindTo(value);
			}
		}

		/// <summary>
		/// Produces a method handle which returns its sole argument when invoked. </summary>
		/// <param name="type"> the type of the sole parameter and return value of the desired method handle </param>
		/// <returns> a unary method handle which accepts and returns the given type </returns>
		/// <exception cref="NullPointerException"> if the argument is null </exception>
		/// <exception cref="IllegalArgumentException"> if the given type is {@code void.class} </exception>
		public static MethodHandle Identity(Class type)
		{
			Wrapper btw = (type.Primitive ? Wrapper.forPrimitiveType(type) : Wrapper.OBJECT);
			int pos = btw.ordinal();
			MethodHandle ident = IDENTITY_MHS[pos];
			if (ident == null)
			{
				ident = SetCachedMethodHandle(IDENTITY_MHS, pos, MakeIdentity(btw.primitiveType()));
			}
			if (ident.Type().ReturnType() == type)
			{
				return ident;
			}
			// something like identity(Foo.class); do not bother to intern these
			assert(btw == Wrapper.OBJECT);
			return MakeIdentity(type);
		}
		private static readonly MethodHandle[] IDENTITY_MHS = new MethodHandle[Wrapper.values().length];
		private static MethodHandle MakeIdentity(Class ptype)
		{
			MethodType mtype = MethodType.MethodType(ptype, ptype);
			LambdaForm lform = LambdaForm.IdentityForm(BasicType.basicType(ptype));
			return MethodHandleImpl.MakeIntrinsic(mtype, lform, Intrinsic.IDENTITY);
		}

		private static MethodHandle Zero(Wrapper btw, Class rtype)
		{
			int pos = btw.ordinal();
			MethodHandle zero = ZERO_MHS[pos];
			if (zero == null)
			{
				zero = SetCachedMethodHandle(ZERO_MHS, pos, MakeZero(btw.primitiveType()));
			}
			if (zero.Type().ReturnType() == rtype)
			{
				return zero;
			}
			assert(btw == Wrapper.OBJECT);
			return MakeZero(rtype);
		}
		private static readonly MethodHandle[] ZERO_MHS = new MethodHandle[Wrapper.values().length];
		private static MethodHandle MakeZero(Class rtype)
		{
			MethodType mtype = MethodType.MethodType(rtype);
			LambdaForm lform = LambdaForm.ZeroForm(BasicType.basicType(rtype));
			return MethodHandleImpl.MakeIntrinsic(mtype, lform, Intrinsic.ZERO);
		}

		private static MethodHandle SetCachedMethodHandle(MethodHandle[] cache, int pos, MethodHandle value)
		{
			lock (typeof(MethodHandles))
			{
				// Simulate a CAS, to avoid racy duplication of results.
				MethodHandle prev = cache[pos];
				if (prev != null)
				{
					return prev;
				}
				return cache[pos] = value;
			}
		}

		/// <summary>
		/// Provides a target method handle with one or more <em>bound arguments</em>
		/// in advance of the method handle's invocation.
		/// The formal parameters to the target corresponding to the bound
		/// arguments are called <em>bound parameters</em>.
		/// Returns a new method handle which saves away the bound arguments.
		/// When it is invoked, it receives arguments for any non-bound parameters,
		/// binds the saved arguments to their corresponding parameters,
		/// and calls the original target.
		/// <para>
		/// The type of the new method handle will drop the types for the bound
		/// parameters from the original target type, since the new method handle
		/// will no longer require those arguments to be supplied by its callers.
		/// </para>
		/// <para>
		/// Each given argument object must match the corresponding bound parameter type.
		/// If a bound parameter type is a primitive, the argument object
		/// must be a wrapper, and will be unboxed to produce the primitive value.
		/// </para>
		/// <para>
		/// The {@code pos} argument selects which parameters are to be bound.
		/// It may range between zero and <i>N-L</i> (inclusively),
		/// where <i>N</i> is the arity of the target method handle
		/// and <i>L</i> is the length of the values array.
		/// </para>
		/// </summary>
		/// <param name="target"> the method handle to invoke after the argument is inserted </param>
		/// <param name="pos"> where to insert the argument (zero for the first) </param>
		/// <param name="values"> the series of arguments to insert </param>
		/// <returns> a method handle which inserts an additional argument,
		///         before calling the original method handle </returns>
		/// <exception cref="NullPointerException"> if the target or the {@code values} array is null </exception>
		/// <seealso cref= MethodHandle#bindTo </seealso>
		public static MethodHandle InsertArguments(MethodHandle target, int pos, params Object[] values)
		{
			int insCount = values.Length;
			Class[] ptypes = InsertArgumentsChecks(target, insCount, pos);
			if (insCount == 0)
			{
				return target;
			}
			BoundMethodHandle result = target.Rebind();
			for (int i = 0; i < insCount; i++)
			{
				Object value = values[i];
				Class ptype = ptypes[pos + i];
				if (ptype.Primitive)
				{
					result = InsertArgumentPrimitive(result, pos, ptype, value);
				}
				else
				{
					value = ptype.Cast(value); // throw CCE if needed
					result = result.BindArgumentL(pos, value);
				}
			}
			return result;
		}

		private static BoundMethodHandle InsertArgumentPrimitive(BoundMethodHandle result, int pos, Class ptype, Object value)
		{
			Wrapper w = Wrapper.forPrimitiveType(ptype);
			// perform unboxing and/or primitive conversion
			value = w.Convert(value, ptype);
			switch (w)
			{
			case INT:
				return result.BindArgumentI(pos, (int)value);
			case LONG:
				return result.BindArgumentJ(pos, (long)value);
			case FLOAT:
				return result.BindArgumentF(pos, (float)value);
			case DOUBLE:
				return result.BindArgumentD(pos, (double)value);
			default:
				return result.BindArgumentI(pos, ValueConversions.widenSubword(value));
			}
		}

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: private static Class[] insertArgumentsChecks(MethodHandle target, int insCount, int pos) throws RuntimeException
		private static Class[] InsertArgumentsChecks(MethodHandle target, int insCount, int pos)
		{
			MethodType oldType = target.Type();
			int outargs = oldType.ParameterCount();
			int inargs = outargs - insCount;
			if (inargs < 0)
			{
				throw newIllegalArgumentException("too many values to insert");
			}
			if (pos < 0 || pos > inargs)
			{
				throw newIllegalArgumentException("no argument type to append");
			}
			return oldType.Ptypes();
		}

		/// <summary>
		/// Produces a method handle which will discard some dummy arguments
		/// before calling some other specified <i>target</i> method handle.
		/// The type of the new method handle will be the same as the target's type,
		/// except it will also include the dummy argument types,
		/// at some given position.
		/// <para>
		/// The {@code pos} argument may range between zero and <i>N</i>,
		/// where <i>N</i> is the arity of the target.
		/// If {@code pos} is zero, the dummy arguments will precede
		/// the target's real arguments; if {@code pos} is <i>N</i>
		/// they will come after.
		/// </para>
		/// <para>
		/// <b>Example:</b>
		/// <blockquote><pre>{@code
		/// import static java.lang.invoke.MethodHandles.*;
		/// import static java.lang.invoke.MethodType.*;
		/// ...
		/// MethodHandle cat = lookup().findVirtual(String.class,
		/// "concat", methodType(String.class, String.class));
		/// assertEquals("xy", (String) cat.invokeExact("x", "y"));
		/// MethodType bigType = cat.type().insertParameterTypes(0, int.class, String.class);
		/// MethodHandle d0 = dropArguments(cat, 0, bigType.parameterList().subList(0,2));
		/// assertEquals(bigType, d0.type());
		/// assertEquals("yz", (String) d0.invokeExact(123, "x", "y", "z"));
		/// }</pre></blockquote>
		/// </para>
		/// <para>
		/// This method is also equivalent to the following code:
		/// <blockquote><pre>
		/// <seealso cref="#dropArguments(MethodHandle,int,Class...) dropArguments"/>{@code (target, pos, valueTypes.toArray(new Class[0]))}
		/// </pre></blockquote>
		/// </para>
		/// </summary>
		/// <param name="target"> the method handle to invoke after the arguments are dropped </param>
		/// <param name="valueTypes"> the type(s) of the argument(s) to drop </param>
		/// <param name="pos"> position of first argument to drop (zero for the leftmost) </param>
		/// <returns> a method handle which drops arguments of the given types,
		///         before calling the original method handle </returns>
		/// <exception cref="NullPointerException"> if the target is null,
		///                              or if the {@code valueTypes} list or any of its elements is null </exception>
		/// <exception cref="IllegalArgumentException"> if any element of {@code valueTypes} is {@code void.class},
		///                  or if {@code pos} is negative or greater than the arity of the target,
		///                  or if the new method handle's type would have too many parameters </exception>
		public static MethodHandle DropArguments(MethodHandle target, int pos, IList<Class> valueTypes)
		{
			MethodType oldType = target.Type(); // get NPE
			int dropped = DropArgumentChecks(oldType, pos, valueTypes);
			MethodType newType = oldType.InsertParameterTypes(pos, valueTypes);
			if (dropped == 0)
			{
				return target;
			}
			BoundMethodHandle result = target.Rebind();
			LambdaForm lform = result.Form;
			int insertFormArg = 1 + pos;
			foreach (Class ptype in valueTypes)
			{
				lform = lform.Editor().AddArgumentForm(insertFormArg++, BasicType.basicType(ptype));
			}
			result = result.CopyWith(newType, lform);
			return result;
		}

		private static int DropArgumentChecks(MethodType oldType, int pos, IList<Class> valueTypes)
		{
			int dropped = valueTypes.Count;
			MethodType.CheckSlotCount(dropped);
			int outargs = oldType.ParameterCount();
			int inargs = outargs + dropped;
			if (pos < 0 || pos > outargs)
			{
				throw newIllegalArgumentException("no argument type to remove" + Arrays.asList(oldType, pos, valueTypes, inargs, outargs));
			}
			return dropped;
		}

		/// <summary>
		/// Produces a method handle which will discard some dummy arguments
		/// before calling some other specified <i>target</i> method handle.
		/// The type of the new method handle will be the same as the target's type,
		/// except it will also include the dummy argument types,
		/// at some given position.
		/// <para>
		/// The {@code pos} argument may range between zero and <i>N</i>,
		/// where <i>N</i> is the arity of the target.
		/// If {@code pos} is zero, the dummy arguments will precede
		/// the target's real arguments; if {@code pos} is <i>N</i>
		/// they will come after.
		/// </para>
		/// <para>
		/// <b>Example:</b>
		/// <blockquote><pre>{@code
		/// import static java.lang.invoke.MethodHandles.*;
		/// import static java.lang.invoke.MethodType.*;
		/// ...
		/// MethodHandle cat = lookup().findVirtual(String.class,
		/// "concat", methodType(String.class, String.class));
		/// assertEquals("xy", (String) cat.invokeExact("x", "y"));
		/// MethodHandle d0 = dropArguments(cat, 0, String.class);
		/// assertEquals("yz", (String) d0.invokeExact("x", "y", "z"));
		/// MethodHandle d1 = dropArguments(cat, 1, String.class);
		/// assertEquals("xz", (String) d1.invokeExact("x", "y", "z"));
		/// MethodHandle d2 = dropArguments(cat, 2, String.class);
		/// assertEquals("xy", (String) d2.invokeExact("x", "y", "z"));
		/// MethodHandle d12 = dropArguments(cat, 1, int.class, boolean.class);
		/// assertEquals("xz", (String) d12.invokeExact("x", 12, true, "z"));
		/// }</pre></blockquote>
		/// </para>
		/// <para>
		/// This method is also equivalent to the following code:
		/// <blockquote><pre>
		/// <seealso cref="#dropArguments(MethodHandle,int,List) dropArguments"/>{@code (target, pos, Arrays.asList(valueTypes))}
		/// </pre></blockquote>
		/// </para>
		/// </summary>
		/// <param name="target"> the method handle to invoke after the arguments are dropped </param>
		/// <param name="valueTypes"> the type(s) of the argument(s) to drop </param>
		/// <param name="pos"> position of first argument to drop (zero for the leftmost) </param>
		/// <returns> a method handle which drops arguments of the given types,
		///         before calling the original method handle </returns>
		/// <exception cref="NullPointerException"> if the target is null,
		///                              or if the {@code valueTypes} array or any of its elements is null </exception>
		/// <exception cref="IllegalArgumentException"> if any element of {@code valueTypes} is {@code void.class},
		///                  or if {@code pos} is negative or greater than the arity of the target,
		///                  or if the new method handle's type would have
		///                  <a href="MethodHandle.html#maxarity">too many parameters</a> </exception>
		public static MethodHandle DropArguments(MethodHandle target, int pos, params Class[] valueTypes)
		{
			return DropArguments(target, pos, Arrays.AsList(valueTypes));
		}

		/// <summary>
		/// Adapts a target method handle by pre-processing
		/// one or more of its arguments, each with its own unary filter function,
		/// and then calling the target with each pre-processed argument
		/// replaced by the result of its corresponding filter function.
		/// <para>
		/// The pre-processing is performed by one or more method handles,
		/// specified in the elements of the {@code filters} array.
		/// The first element of the filter array corresponds to the {@code pos}
		/// argument of the target, and so on in sequence.
		/// </para>
		/// <para>
		/// Null arguments in the array are treated as identity functions,
		/// and the corresponding arguments left unchanged.
		/// (If there are no non-null elements in the array, the original target is returned.)
		/// Each filter is applied to the corresponding argument of the adapter.
		/// </para>
		/// <para>
		/// If a filter {@code F} applies to the {@code N}th argument of
		/// the target, then {@code F} must be a method handle which
		/// takes exactly one argument.  The type of {@code F}'s sole argument
		/// replaces the corresponding argument type of the target
		/// in the resulting adapted method handle.
		/// The return type of {@code F} must be identical to the corresponding
		/// parameter type of the target.
		/// </para>
		/// <para>
		/// It is an error if there are elements of {@code filters}
		/// (null or not)
		/// which do not correspond to argument positions in the target.
		/// </para>
		/// <para><b>Example:</b>
		/// <blockquote><pre>{@code
		/// import static java.lang.invoke.MethodHandles.*;
		/// import static java.lang.invoke.MethodType.*;
		/// ...
		/// MethodHandle cat = lookup().findVirtual(String.class,
		/// "concat", methodType(String.class, String.class));
		/// MethodHandle upcase = lookup().findVirtual(String.class,
		/// "toUpperCase", methodType(String.class));
		/// assertEquals("xy", (String) cat.invokeExact("x", "y"));
		/// MethodHandle f0 = filterArguments(cat, 0, upcase);
		/// assertEquals("Xy", (String) f0.invokeExact("x", "y")); // Xy
		/// MethodHandle f1 = filterArguments(cat, 1, upcase);
		/// assertEquals("xY", (String) f1.invokeExact("x", "y")); // xY
		/// MethodHandle f2 = filterArguments(cat, 0, upcase, upcase);
		/// assertEquals("XY", (String) f2.invokeExact("x", "y")); // XY
		/// }</pre></blockquote>
		/// </para>
		/// <para> Here is pseudocode for the resulting adapter:
		/// <blockquote><pre>{@code
		/// V target(P... p, A[i]... a[i], B... b);
		/// A[i] filter[i](V[i]);
		/// T adapter(P... p, V[i]... v[i], B... b) {
		///   return target(p..., f[i](v[i])..., b...);
		/// }
		/// }</pre></blockquote>
		/// 
		/// </para>
		/// </summary>
		/// <param name="target"> the method handle to invoke after arguments are filtered </param>
		/// <param name="pos"> the position of the first argument to filter </param>
		/// <param name="filters"> method handles to call initially on filtered arguments </param>
		/// <returns> method handle which incorporates the specified argument filtering logic </returns>
		/// <exception cref="NullPointerException"> if the target is null
		///                              or if the {@code filters} array is null </exception>
		/// <exception cref="IllegalArgumentException"> if a non-null element of {@code filters}
		///          does not match a corresponding argument type of target as described above,
		///          or if the {@code pos+filters.length} is greater than {@code target.type().parameterCount()},
		///          or if the resulting method handle's type would have
		///          <a href="MethodHandle.html#maxarity">too many parameters</a> </exception>
		public static MethodHandle FilterArguments(MethodHandle target, int pos, params MethodHandle[] filters)
		{
			FilterArgumentsCheckArity(target, pos, filters);
			MethodHandle adapter = target;
			int curPos = pos - 1; // pre-incremented
			foreach (MethodHandle filter in filters)
			{
				curPos += 1;
				if (filter == null) // ignore null elements of filters
				{
					continue;
				}
				adapter = FilterArgument(adapter, curPos, filter);
			}
			return adapter;
		}

		/*non-public*/	 internal static MethodHandle FilterArgument(MethodHandle target, int pos, MethodHandle filter)
	 {
			FilterArgumentChecks(target, pos, filter);
			MethodType targetType = target.Type();
			MethodType filterType = filter.Type();
			BoundMethodHandle result = target.Rebind();
			Class newParamType = filterType.ParameterType(0);
			LambdaForm lform = result.Editor().FilterArgumentForm(1 + pos, BasicType.basicType(newParamType));
			MethodType newType = targetType.ChangeParameterType(pos, newParamType);
			result = result.CopyWithExtendL(newType, lform, filter);
			return result;
	 }

		private static void FilterArgumentsCheckArity(MethodHandle target, int pos, MethodHandle[] filters)
		{
			MethodType targetType = target.Type();
			int maxPos = targetType.ParameterCount();
			if (pos + filters.Length > maxPos)
			{
				throw newIllegalArgumentException("too many filters");
			}
		}

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: private static void filterArgumentChecks(MethodHandle target, int pos, MethodHandle filter) throws RuntimeException
		private static void FilterArgumentChecks(MethodHandle target, int pos, MethodHandle filter)
		{
			MethodType targetType = target.Type();
			MethodType filterType = filter.Type();
			if (filterType.ParameterCount() != 1 || filterType.ReturnType() != targetType.ParameterType(pos))
			{
				throw newIllegalArgumentException("target and filter types do not match", targetType, filterType);
			}
		}

		/// <summary>
		/// Adapts a target method handle by pre-processing
		/// a sub-sequence of its arguments with a filter (another method handle).
		/// The pre-processed arguments are replaced by the result (if any) of the
		/// filter function.
		/// The target is then called on the modified (usually shortened) argument list.
		/// <para>
		/// If the filter returns a value, the target must accept that value as
		/// its argument in position {@code pos}, preceded and/or followed by
		/// any arguments not passed to the filter.
		/// If the filter returns void, the target must accept all arguments
		/// not passed to the filter.
		/// No arguments are reordered, and a result returned from the filter
		/// replaces (in order) the whole subsequence of arguments originally
		/// passed to the adapter.
		/// </para>
		/// <para>
		/// The argument types (if any) of the filter
		/// replace zero or one argument types of the target, at position {@code pos},
		/// in the resulting adapted method handle.
		/// The return type of the filter (if any) must be identical to the
		/// argument type of the target at position {@code pos}, and that target argument
		/// is supplied by the return value of the filter.
		/// </para>
		/// <para>
		/// In all cases, {@code pos} must be greater than or equal to zero, and
		/// {@code pos} must also be less than or equal to the target's arity.
		/// </para>
		/// <para><b>Example:</b>
		/// <blockquote><pre>{@code
		/// import static java.lang.invoke.MethodHandles.*;
		/// import static java.lang.invoke.MethodType.*;
		/// ...
		/// MethodHandle deepToString = publicLookup()
		/// .findStatic(Arrays.class, "deepToString", methodType(String.class, Object[].class));
		/// 
		/// MethodHandle ts1 = deepToString.asCollector(String[].class, 1);
		/// assertEquals("[strange]", (String) ts1.invokeExact("strange"));
		/// 
		/// MethodHandle ts2 = deepToString.asCollector(String[].class, 2);
		/// assertEquals("[up, down]", (String) ts2.invokeExact("up", "down"));
		/// 
		/// MethodHandle ts3 = deepToString.asCollector(String[].class, 3);
		/// MethodHandle ts3_ts2 = collectArguments(ts3, 1, ts2);
		/// assertEquals("[top, [up, down], strange]",
		///         (String) ts3_ts2.invokeExact("top", "up", "down", "strange"));
		/// 
		/// MethodHandle ts3_ts2_ts1 = collectArguments(ts3_ts2, 3, ts1);
		/// assertEquals("[top, [up, down], [strange]]",
		///         (String) ts3_ts2_ts1.invokeExact("top", "up", "down", "strange"));
		/// 
		/// MethodHandle ts3_ts2_ts3 = collectArguments(ts3_ts2, 1, ts3);
		/// assertEquals("[top, [[up, down, strange], charm], bottom]",
		///         (String) ts3_ts2_ts3.invokeExact("top", "up", "down", "strange", "charm", "bottom"));
		/// }</pre></blockquote>
		/// </para>
		/// <para> Here is pseudocode for the resulting adapter:
		/// <blockquote><pre>{@code
		/// T target(A...,V,C...);
		/// V filter(B...);
		/// T adapter(A... a,B... b,C... c) {
		///   V v = filter(b...);
		///   return target(a...,v,c...);
		/// }
		/// // and if the filter has no arguments:
		/// T target2(A...,V,C...);
		/// V filter2();
		/// T adapter2(A... a,C... c) {
		///   V v = filter2();
		///   return target2(a...,v,c...);
		/// }
		/// // and if the filter has a void return:
		/// T target3(A...,C...);
		/// void filter3(B...);
		/// void adapter3(A... a,B... b,C... c) {
		///   filter3(b...);
		///   return target3(a...,c...);
		/// }
		/// }</pre></blockquote>
		/// </para>
		/// <para>
		/// A collection adapter {@code collectArguments(mh, 0, coll)} is equivalent to
		/// one which first "folds" the affected arguments, and then drops them, in separate
		/// steps as follows:
		/// <blockquote><pre>{@code
		/// mh = MethodHandles.dropArguments(mh, 1, coll.type().parameterList()); //step 2
		/// mh = MethodHandles.foldArguments(mh, coll); //step 1
		/// }</pre></blockquote>
		/// If the target method handle consumes no arguments besides than the result
		/// (if any) of the filter {@code coll}, then {@code collectArguments(mh, 0, coll)}
		/// is equivalent to {@code filterReturnValue(coll, mh)}.
		/// If the filter method handle {@code coll} consumes one argument and produces
		/// a non-void result, then {@code collectArguments(mh, N, coll)}
		/// is equivalent to {@code filterArguments(mh, N, coll)}.
		/// Other equivalences are possible but would require argument permutation.
		/// 
		/// </para>
		/// </summary>
		/// <param name="target"> the method handle to invoke after filtering the subsequence of arguments </param>
		/// <param name="pos"> the position of the first adapter argument to pass to the filter,
		///            and/or the target argument which receives the result of the filter </param>
		/// <param name="filter"> method handle to call on the subsequence of arguments </param>
		/// <returns> method handle which incorporates the specified argument subsequence filtering logic </returns>
		/// <exception cref="NullPointerException"> if either argument is null </exception>
		/// <exception cref="IllegalArgumentException"> if the return type of {@code filter}
		///          is non-void and is not the same as the {@code pos} argument of the target,
		///          or if {@code pos} is not between 0 and the target's arity, inclusive,
		///          or if the resulting method handle's type would have
		///          <a href="MethodHandle.html#maxarity">too many parameters</a> </exception>
		/// <seealso cref= MethodHandles#foldArguments </seealso>
		/// <seealso cref= MethodHandles#filterArguments </seealso>
		/// <seealso cref= MethodHandles#filterReturnValue </seealso>
		public static MethodHandle CollectArguments(MethodHandle target, int pos, MethodHandle filter)
		{
			MethodType newType = CollectArgumentsChecks(target, pos, filter);
			MethodType collectorType = filter.Type();
			BoundMethodHandle result = target.Rebind();
			LambdaForm lform;
			if (collectorType.ReturnType().Array && filter.IntrinsicName() == Intrinsic.NEW_ARRAY)
			{
				lform = result.Editor().CollectArgumentArrayForm(1 + pos, filter);
				if (lform != null)
				{
					return result.CopyWith(newType, lform);
				}
			}
			lform = result.Editor().CollectArgumentsForm(1 + pos, collectorType.BasicType());
			return result.CopyWithExtendL(newType, lform, filter);
		}

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: private static MethodType collectArgumentsChecks(MethodHandle target, int pos, MethodHandle filter) throws RuntimeException
		private static MethodType CollectArgumentsChecks(MethodHandle target, int pos, MethodHandle filter)
		{
			MethodType targetType = target.Type();
			MethodType filterType = filter.Type();
			Class rtype = filterType.ReturnType();
			IList<Class> filterArgs = filterType.ParameterList();
			if (rtype == typeof(void))
			{
				return targetType.InsertParameterTypes(pos, filterArgs);
			}
			if (rtype != targetType.ParameterType(pos))
			{
				throw newIllegalArgumentException("target and filter types do not match", targetType, filterType);
			}
			return targetType.DropParameterTypes(pos, pos + 1).InsertParameterTypes(pos, filterArgs);
		}

		/// <summary>
		/// Adapts a target method handle by post-processing
		/// its return value (if any) with a filter (another method handle).
		/// The result of the filter is returned from the adapter.
		/// <para>
		/// If the target returns a value, the filter must accept that value as
		/// its only argument.
		/// If the target returns void, the filter must accept no arguments.
		/// </para>
		/// <para>
		/// The return type of the filter
		/// replaces the return type of the target
		/// in the resulting adapted method handle.
		/// The argument type of the filter (if any) must be identical to the
		/// return type of the target.
		/// </para>
		/// <para><b>Example:</b>
		/// <blockquote><pre>{@code
		/// import static java.lang.invoke.MethodHandles.*;
		/// import static java.lang.invoke.MethodType.*;
		/// ...
		/// MethodHandle cat = lookup().findVirtual(String.class,
		/// "concat", methodType(String.class, String.class));
		/// MethodHandle length = lookup().findVirtual(String.class,
		/// "length", methodType(int.class));
		/// System.out.println((String) cat.invokeExact("x", "y")); // xy
		/// MethodHandle f0 = filterReturnValue(cat, length);
		/// System.out.println((int) f0.invokeExact("x", "y")); // 2
		/// }</pre></blockquote>
		/// </para>
		/// <para> Here is pseudocode for the resulting adapter:
		/// <blockquote><pre>{@code
		/// V target(A...);
		/// T filter(V);
		/// T adapter(A... a) {
		///   V v = target(a...);
		///   return filter(v);
		/// }
		/// // and if the target has a void return:
		/// void target2(A...);
		/// T filter2();
		/// T adapter2(A... a) {
		///   target2(a...);
		///   return filter2();
		/// }
		/// // and if the filter has a void return:
		/// V target3(A...);
		/// void filter3(V);
		/// void adapter3(A... a) {
		///   V v = target3(a...);
		///   filter3(v);
		/// }
		/// }</pre></blockquote>
		/// </para>
		/// </summary>
		/// <param name="target"> the method handle to invoke before filtering the return value </param>
		/// <param name="filter"> method handle to call on the return value </param>
		/// <returns> method handle which incorporates the specified return value filtering logic </returns>
		/// <exception cref="NullPointerException"> if either argument is null </exception>
		/// <exception cref="IllegalArgumentException"> if the argument list of {@code filter}
		///          does not match the return type of target as described above </exception>
		public static MethodHandle FilterReturnValue(MethodHandle target, MethodHandle filter)
		{
			MethodType targetType = target.Type();
			MethodType filterType = filter.Type();
			FilterReturnValueChecks(targetType, filterType);
			BoundMethodHandle result = target.Rebind();
			BasicType rtype = BasicType.basicType(filterType.ReturnType());
			LambdaForm lform = result.Editor().FilterReturnForm(rtype, false);
			MethodType newType = targetType.ChangeReturnType(filterType.ReturnType());
			result = result.CopyWithExtendL(newType, lform, filter);
			return result;
		}

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: private static void filterReturnValueChecks(MethodType targetType, MethodType filterType) throws RuntimeException
		private static void FilterReturnValueChecks(MethodType targetType, MethodType filterType)
		{
			Class rtype = targetType.ReturnType();
			int filterValues = filterType.ParameterCount();
			if (filterValues == 0 ? (rtype != typeof(void)) : (rtype != filterType.ParameterType(0)))
			{
				throw newIllegalArgumentException("target and filter types do not match", targetType, filterType);
			}
		}

		/// <summary>
		/// Adapts a target method handle by pre-processing
		/// some of its arguments, and then calling the target with
		/// the result of the pre-processing, inserted into the original
		/// sequence of arguments.
		/// <para>
		/// The pre-processing is performed by {@code combiner}, a second method handle.
		/// Of the arguments passed to the adapter, the first {@code N} arguments
		/// are copied to the combiner, which is then called.
		/// (Here, {@code N} is defined as the parameter count of the combiner.)
		/// After this, control passes to the target, with any result
		/// from the combiner inserted before the original {@code N} incoming
		/// arguments.
		/// </para>
		/// <para>
		/// If the combiner returns a value, the first parameter type of the target
		/// must be identical with the return type of the combiner, and the next
		/// {@code N} parameter types of the target must exactly match the parameters
		/// of the combiner.
		/// </para>
		/// <para>
		/// If the combiner has a void return, no result will be inserted,
		/// and the first {@code N} parameter types of the target
		/// must exactly match the parameters of the combiner.
		/// </para>
		/// <para>
		/// The resulting adapter is the same type as the target, except that the
		/// first parameter type is dropped,
		/// if it corresponds to the result of the combiner.
		/// </para>
		/// <para>
		/// (Note that <seealso cref="#dropArguments(MethodHandle,int,List) dropArguments"/> can be used to remove any arguments
		/// that either the combiner or the target does not wish to receive.
		/// If some of the incoming arguments are destined only for the combiner,
		/// consider using <seealso cref="MethodHandle#asCollector asCollector"/> instead, since those
		/// arguments will not need to be live on the stack on entry to the
		/// target.)
		/// </para>
		/// <para><b>Example:</b>
		/// <blockquote><pre>{@code
		/// import static java.lang.invoke.MethodHandles.*;
		/// import static java.lang.invoke.MethodType.*;
		/// ...
		/// MethodHandle trace = publicLookup().findVirtual(java.io.PrintStream.class,
		/// "println", methodType(void.class, String.class))
		/// .bindTo(System.out);
		/// MethodHandle cat = lookup().findVirtual(String.class,
		/// "concat", methodType(String.class, String.class));
		/// assertEquals("boojum", (String) cat.invokeExact("boo", "jum"));
		/// MethodHandle catTrace = foldArguments(cat, trace);
		/// // also prints "boo":
		/// assertEquals("boojum", (String) catTrace.invokeExact("boo", "jum"));
		/// }</pre></blockquote>
		/// </para>
		/// <para> Here is pseudocode for the resulting adapter:
		/// <blockquote><pre>{@code
		/// // there are N arguments in A...
		/// T target(V, A[N]..., B...);
		/// V combiner(A...);
		/// T adapter(A... a, B... b) {
		///   V v = combiner(a...);
		///   return target(v, a..., b...);
		/// }
		/// // and if the combiner has a void return:
		/// T target2(A[N]..., B...);
		/// void combiner2(A...);
		/// T adapter2(A... a, B... b) {
		///   combiner2(a...);
		///   return target2(a..., b...);
		/// }
		/// }</pre></blockquote>
		/// </para>
		/// </summary>
		/// <param name="target"> the method handle to invoke after arguments are combined </param>
		/// <param name="combiner"> method handle to call initially on the incoming arguments </param>
		/// <returns> method handle which incorporates the specified argument folding logic </returns>
		/// <exception cref="NullPointerException"> if either argument is null </exception>
		/// <exception cref="IllegalArgumentException"> if {@code combiner}'s return type
		///          is non-void and not the same as the first argument type of
		///          the target, or if the initial {@code N} argument types
		///          of the target
		///          (skipping one matching the {@code combiner}'s return type)
		///          are not identical with the argument types of {@code combiner} </exception>
		public static MethodHandle FoldArguments(MethodHandle target, MethodHandle combiner)
		{
			int foldPos = 0;
			MethodType targetType = target.Type();
			MethodType combinerType = combiner.Type();
			Class rtype = FoldArgumentChecks(foldPos, targetType, combinerType);
			BoundMethodHandle result = target.Rebind();
			bool dropResult = (rtype == typeof(void));
			// Note:  This may cache too many distinct LFs. Consider backing off to varargs code.
			LambdaForm lform = result.Editor().FoldArgumentsForm(1 + foldPos, dropResult, combinerType.BasicType());
			MethodType newType = targetType;
			if (!dropResult)
			{
				newType = newType.DropParameterTypes(foldPos, foldPos + 1);
			}
			result = result.CopyWithExtendL(newType, lform, combiner);
			return result;
		}

		private static Class FoldArgumentChecks(int foldPos, MethodType targetType, MethodType combinerType)
		{
			int foldArgs = combinerType.ParameterCount();
			Class rtype = combinerType.ReturnType();
			int foldVals = rtype == typeof(void) ? 0 : 1;
			int afterInsertPos = foldPos + foldVals;
			bool ok = (targetType.ParameterCount() >= afterInsertPos + foldArgs);
			if (ok && !(combinerType.ParameterList().Equals(targetType.ParameterList().subList(afterInsertPos, afterInsertPos + foldArgs))))
			{
				ok = false;
			}
			if (ok && foldVals != 0 && combinerType.ReturnType() != targetType.ParameterType(0))
			{
				ok = false;
			}
			if (!ok)
			{
				throw MisMatchedTypes("target and combiner types", targetType, combinerType);
			}
			return rtype;
		}

		/// <summary>
		/// Makes a method handle which adapts a target method handle,
		/// by guarding it with a test, a boolean-valued method handle.
		/// If the guard fails, a fallback handle is called instead.
		/// All three method handles must have the same corresponding
		/// argument and return types, except that the return type
		/// of the test must be boolean, and the test is allowed
		/// to have fewer arguments than the other two method handles.
		/// <para> Here is pseudocode for the resulting adapter:
		/// <blockquote><pre>{@code
		/// boolean test(A...);
		/// T target(A...,B...);
		/// T fallback(A...,B...);
		/// T adapter(A... a,B... b) {
		///   if (test(a...))
		///     return target(a..., b...);
		///   else
		///     return fallback(a..., b...);
		/// }
		/// }</pre></blockquote>
		/// Note that the test arguments ({@code a...} in the pseudocode) cannot
		/// be modified by execution of the test, and so are passed unchanged
		/// from the caller to the target or fallback as appropriate.
		/// </para>
		/// </summary>
		/// <param name="test"> method handle used for test, must return boolean </param>
		/// <param name="target"> method handle to call if test passes </param>
		/// <param name="fallback"> method handle to call if test fails </param>
		/// <returns> method handle which incorporates the specified if/then/else logic </returns>
		/// <exception cref="NullPointerException"> if any argument is null </exception>
		/// <exception cref="IllegalArgumentException"> if {@code test} does not return boolean,
		///          or if all three method types do not match (with the return
		///          type of {@code test} changed to match that of the target). </exception>
		public static MethodHandle GuardWithTest(MethodHandle test, MethodHandle target, MethodHandle fallback)
		{
			MethodType gtype = test.Type();
			MethodType ttype = target.Type();
			MethodType ftype = fallback.Type();
			if (!ttype.Equals(ftype))
			{
				throw MisMatchedTypes("target and fallback types", ttype, ftype);
			}
			if (gtype.ReturnType() != typeof(bool))
			{
				throw newIllegalArgumentException("guard type is not a predicate " + gtype);
			}
			IList<Class> targs = ttype.ParameterList();
			IList<Class> gargs = gtype.ParameterList();
			if (!targs.Equals(gargs))
			{
				int gpc = gargs.Count, tpc = targs.Count;
				if (gpc >= tpc || !targs.subList(0, gpc).Equals(gargs))
				{
					throw MisMatchedTypes("target and test types", ttype, gtype);
				}
				test = DropArguments(test, gpc, targs.subList(gpc, tpc));
				gtype = test.Type();
			}
			return MethodHandleImpl.MakeGuardWithTest(test, target, fallback);
		}

		internal static RuntimeException MisMatchedTypes(String what, MethodType t1, MethodType t2)
		{
			return newIllegalArgumentException(what + " must match: " + t1 + " != " + t2);
		}

		/// <summary>
		/// Makes a method handle which adapts a target method handle,
		/// by running it inside an exception handler.
		/// If the target returns normally, the adapter returns that value.
		/// If an exception matching the specified type is thrown, the fallback
		/// handle is called instead on the exception, plus the original arguments.
		/// <para>
		/// The target and handler must have the same corresponding
		/// argument and return types, except that handler may omit trailing arguments
		/// (similarly to the predicate in <seealso cref="#guardWithTest guardWithTest"/>).
		/// Also, the handler must have an extra leading parameter of {@code exType} or a supertype.
		/// </para>
		/// <para> Here is pseudocode for the resulting adapter:
		/// <blockquote><pre>{@code
		/// T target(A..., B...);
		/// T handler(ExType, A...);
		/// T adapter(A... a, B... b) {
		///   try {
		///     return target(a..., b...);
		///   } catch (ExType ex) {
		///     return handler(ex, a...);
		///   }
		/// }
		/// }</pre></blockquote>
		/// Note that the saved arguments ({@code a...} in the pseudocode) cannot
		/// be modified by execution of the target, and so are passed unchanged
		/// from the caller to the handler, if the handler is invoked.
		/// </para>
		/// <para>
		/// The target and handler must return the same type, even if the handler
		/// always throws.  (This might happen, for instance, because the handler
		/// is simulating a {@code finally} clause).
		/// To create such a throwing handler, compose the handler creation logic
		/// with <seealso cref="#throwException throwException"/>,
		/// in order to create a method handle of the correct return type.
		/// </para>
		/// </summary>
		/// <param name="target"> method handle to call </param>
		/// <param name="exType"> the type of exception which the handler will catch </param>
		/// <param name="handler"> method handle to call if a matching exception is thrown </param>
		/// <returns> method handle which incorporates the specified try/catch logic </returns>
		/// <exception cref="NullPointerException"> if any argument is null </exception>
		/// <exception cref="IllegalArgumentException"> if {@code handler} does not accept
		///          the given exception type, or if the method handle types do
		///          not match in their return types and their
		///          corresponding parameters </exception>
		public static MethodHandle CatchException(MethodHandle target, Class exType, MethodHandle handler)
		{
			MethodType ttype = target.Type();
			MethodType htype = handler.Type();
			if (htype.ParameterCount() < 1 || !exType.IsSubclassOf(htype.ParameterType(0)))
			{
				throw newIllegalArgumentException("handler does not accept exception type " + exType);
			}
			if (htype.ReturnType() != ttype.ReturnType())
			{
				throw MisMatchedTypes("target and handler return types", ttype, htype);
			}
			IList<Class> targs = ttype.ParameterList();
			IList<Class> hargs = htype.ParameterList();
			hargs = hargs.subList(1, hargs.Count); // omit leading parameter from handler
			if (!targs.Equals(hargs))
			{
				int hpc = hargs.Count, tpc = targs.Count;
				if (hpc >= tpc || !targs.subList(0, hpc).Equals(hargs))
				{
					throw MisMatchedTypes("target and handler types", ttype, htype);
				}
				handler = DropArguments(handler, 1 + hpc, targs.subList(hpc, tpc));
				htype = handler.Type();
			}
			return MethodHandleImpl.MakeGuardWithCatch(target, exType, handler);
		}

		/// <summary>
		/// Produces a method handle which will throw exceptions of the given {@code exType}.
		/// The method handle will accept a single argument of {@code exType},
		/// and immediately throw it as an exception.
		/// The method type will nominally specify a return of {@code returnType}.
		/// The return type may be anything convenient:  It doesn't matter to the
		/// method handle's behavior, since it will never return normally. </summary>
		/// <param name="returnType"> the return type of the desired method handle </param>
		/// <param name="exType"> the parameter type of the desired method handle </param>
		/// <returns> method handle which can throw the given exceptions </returns>
		/// <exception cref="NullPointerException"> if either argument is null </exception>
		public static MethodHandle ThrowException(Class returnType, Class exType)
		{
			if (!exType.IsSubclassOf(typeof(Throwable)))
			{
				throw new ClassCastException(exType.Name);
			}
			return MethodHandleImpl.ThrowException(MethodType.MethodType(returnType, exType));
		}
	}

}
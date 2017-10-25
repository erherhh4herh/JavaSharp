using System;
using System.Runtime.InteropServices;

/*
 * Copyright (c) 1997, 2013, Oracle and/or its affiliates. All rights reserved.
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

namespace java.security
{

	using Debug = sun.security.util.Debug;
	using CallerSensitive = sun.reflect.CallerSensitive;
	using Reflection = sun.reflect.Reflection;

	/// <summary>
	/// <para> The AccessController class is used for access control operations
	/// and decisions.
	/// 
	/// </para>
	/// <para> More specifically, the AccessController class is used for
	/// three purposes:
	/// 
	/// <ul>
	/// <li> to decide whether an access to a critical system
	/// resource is to be allowed or denied, based on the security policy
	/// currently in effect,
	/// <li>to mark code as being "privileged", thus affecting subsequent
	/// access determinations, and
	/// <li>to obtain a "snapshot" of the current calling context so
	/// access-control decisions from a different context can be made with
	/// respect to the saved context. </ul>
	/// 
	/// </para>
	/// <para> The <seealso cref="#checkPermission(Permission) checkPermission"/> method
	/// determines whether the access request indicated by a specified
	/// permission should be granted or denied. A sample call appears
	/// below. In this example, {@code checkPermission} will determine
	/// whether or not to grant "read" access to the file named "testFile" in
	/// the "/temp" directory.
	/// 
	/// <pre>
	/// 
	/// FilePermission perm = new FilePermission("/temp/testFile", "read");
	/// AccessController.checkPermission(perm);
	/// 
	/// </pre>
	/// 
	/// </para>
	/// <para> If a requested access is allowed,
	/// {@code checkPermission} returns quietly. If denied, an
	/// AccessControlException is
	/// thrown. AccessControlException can also be thrown if the requested
	/// permission is of an incorrect type or contains an invalid value.
	/// Such information is given whenever possible.
	/// 
	/// Suppose the current thread traversed m callers, in the order of caller 1
	/// to caller 2 to caller m. Then caller m invoked the
	/// {@code checkPermission} method.
	/// The {@code checkPermission} method determines whether access
	/// is granted or denied based on the following algorithm:
	/// 
	///  <pre> {@code
	/// for (int i = m; i > 0; i--) {
	/// 
	///     if (caller i's domain does not have the permission)
	///         throw AccessControlException
	/// 
	///     else if (caller i is marked as privileged) {
	///         if (a context was specified in the call to doPrivileged)
	///             context.checkPermission(permission)
	///         if (limited permissions were specified in the call to doPrivileged) {
	///             for (each limited permission) {
	///                 if (the limited permission implies the requested permission)
	///                     return;
	///             }
	///         } else
	///             return;
	///     }
	/// }
	/// 
	/// // Next, check the context inherited when the thread was created.
	/// // Whenever a new thread is created, the AccessControlContext at
	/// // that time is stored and associated with the new thread, as the
	/// // "inherited" context.
	/// 
	/// inheritedContext.checkPermission(permission);
	/// }</pre>
	/// 
	/// </para>
	/// <para> A caller can be marked as being "privileged"
	/// (see <seealso cref="#doPrivileged(PrivilegedAction) doPrivileged"/> and below).
	/// When making access control decisions, the {@code checkPermission}
	/// method stops checking if it reaches a caller that
	/// was marked as "privileged" via a {@code doPrivileged}
	/// call without a context argument (see below for information about a
	/// context argument). If that caller's domain has the
	/// specified permission and at least one limiting permission argument (if any)
	/// implies the requested permission, no further checking is done and
	/// {@code checkPermission}
	/// returns quietly, indicating that the requested access is allowed.
	/// If that domain does not have the specified permission, an exception
	/// is thrown, as usual. If the caller's domain had the specified permission
	/// but it was not implied by any limiting permission arguments given in the call
	/// to {@code doPrivileged} then the permission checking continues
	/// until there are no more callers or another {@code doPrivileged}
	/// call matches the requested permission and returns normally.
	/// 
	/// </para>
	/// <para> The normal use of the "privileged" feature is as follows. If you
	/// don't need to return a value from within the "privileged" block, do
	/// the following:
	/// 
	///  <pre> {@code
	/// somemethod() {
	///     ...normal code here...
	///     AccessController.doPrivileged(new PrivilegedAction<Void>() {
	///         public Void run() {
	///             // privileged code goes here, for example:
	///             System.loadLibrary("awt");
	///             return null; // nothing to return
	///         }
	///     });
	///     ...normal code here...
	/// }}</pre>
	/// 
	/// </para>
	/// <para>
	/// PrivilegedAction is an interface with a single method, named
	/// {@code run}.
	/// The above example shows creation of an implementation
	/// of that interface; a concrete implementation of the
	/// {@code run} method is supplied.
	/// When the call to {@code doPrivileged} is made, an
	/// instance of the PrivilegedAction implementation is passed
	/// to it. The {@code doPrivileged} method calls the
	/// {@code run} method from the PrivilegedAction
	/// implementation after enabling privileges, and returns the
	/// {@code run} method's return value as the
	/// {@code doPrivileged} return value (which is
	/// ignored in this example).
	/// 
	/// </para>
	/// <para> If you need to return a value, you can do something like the following:
	/// 
	///  <pre> {@code
	/// somemethod() {
	///     ...normal code here...
	///     String user = AccessController.doPrivileged(
	///         new PrivilegedAction<String>() {
	///         public String run() {
	///             return System.getProperty("user.name");
	///             }
	///         });
	///     ...normal code here...
	/// }}</pre>
	/// 
	/// </para>
	/// <para>If the action performed in your {@code run} method could
	/// throw a "checked" exception (those listed in the {@code throws} clause
	/// of a method), then you need to use the
	/// {@code PrivilegedExceptionAction} interface instead of the
	/// {@code PrivilegedAction} interface:
	/// 
	///  <pre> {@code
	/// somemethod() throws FileNotFoundException {
	///     ...normal code here...
	///     try {
	///         FileInputStream fis = AccessController.doPrivileged(
	///         new PrivilegedExceptionAction<FileInputStream>() {
	///             public FileInputStream run() throws FileNotFoundException {
	///                 return new FileInputStream("someFile");
	///             }
	///         });
	///     } catch (PrivilegedActionException e) {
	///         // e.getException() should be an instance of FileNotFoundException,
	///         // as only "checked" exceptions will be "wrapped" in a
	///         // PrivilegedActionException.
	///         throw (FileNotFoundException) e.getException();
	///     }
	///     ...normal code here...
	///  }}</pre>
	/// 
	/// </para>
	/// <para> Be *very* careful in your use of the "privileged" construct, and
	/// always remember to make the privileged code section as small as possible.
	/// You can pass {@code Permission} arguments to further limit the
	/// scope of the "privilege" (see below).
	/// 
	/// 
	/// </para>
	/// <para> Note that {@code checkPermission} always performs security checks
	/// within the context of the currently executing thread.
	/// Sometimes a security check that should be made within a given context
	/// will actually need to be done from within a
	/// <i>different</i> context (for example, from within a worker thread).
	/// The <seealso cref="#getContext() getContext"/> method and
	/// AccessControlContext class are provided
	/// for this situation. The {@code getContext} method takes a "snapshot"
	/// of the current calling context, and places
	/// it in an AccessControlContext object, which it returns. A sample call is
	/// the following:
	/// 
	/// <pre>
	/// 
	/// AccessControlContext acc = AccessController.getContext()
	/// 
	/// </pre>
	/// 
	/// </para>
	/// <para>
	/// AccessControlContext itself has a {@code checkPermission} method
	/// that makes access decisions based on the context <i>it</i> encapsulates,
	/// rather than that of the current execution thread.
	/// Code within a different context can thus call that method on the
	/// previously-saved AccessControlContext object. A sample call is the
	/// following:
	/// 
	/// <pre>
	/// 
	/// acc.checkPermission(permission)
	/// 
	/// </pre>
	/// 
	/// </para>
	/// <para> There are also times where you don't know a priori which permissions
	/// to check the context against. In these cases you can use the
	/// doPrivileged method that takes a context. You can also limit the scope
	/// of the privileged code by passing additional {@code Permission}
	/// parameters.
	/// 
	///  <pre> {@code
	/// somemethod() {
	///     AccessController.doPrivileged(new PrivilegedAction<Object>() {
	///         public Object run() {
	///             // Code goes here. Any permission checks within this
	///             // run method will require that the intersection of the
	///             // caller's protection domain and the snapshot's
	///             // context have the desired permission. If a requested
	///             // permission is not implied by the limiting FilePermission
	///             // argument then checking of the thread continues beyond the
	///             // caller of doPrivileged.
	///         }
	///     }, acc, new FilePermission("/temp/*", read));
	///     ...normal code here...
	/// }}</pre>
	/// </para>
	/// <para> Passing a limiting {@code Permission} argument of an instance of
	/// {@code AllPermission} is equivalent to calling the equivalent
	/// {@code doPrivileged} method without limiting {@code Permission}
	/// arguments. Passing a zero length array of {@code Permission} disables
	/// the code privileges so that checking always continues beyond the caller of
	/// that {@code doPrivileged} method.
	/// 
	/// </para>
	/// </summary>
	/// <seealso cref= AccessControlContext
	/// 
	/// @author Li Gong
	/// @author Roland Schemers </seealso>

	public sealed class AccessController
	{

		/// <summary>
		/// Don't allow anyone to instantiate an AccessController
		/// </summary>
		private AccessController()
		{
		}

		/// <summary>
		/// Performs the specified {@code PrivilegedAction} with privileges
		/// enabled. The action is performed with <i>all</i> of the permissions
		/// possessed by the caller's protection domain.
		/// 
		/// <para> If the action's {@code run} method throws an (unchecked)
		/// exception, it will propagate through this method.
		/// 
		/// </para>
		/// <para> Note that any DomainCombiner associated with the current
		/// AccessControlContext will be ignored while the action is performed.
		/// 
		/// </para>
		/// </summary>
		/// @param <T> the type of the value returned by the PrivilegedAction's
		///                  {@code run} method.
		/// </param>
		/// <param name="action"> the action to be performed.
		/// </param>
		/// <returns> the value returned by the action's {@code run} method.
		/// </returns>
		/// <exception cref="NullPointerException"> if the action is {@code null}
		/// </exception>
		/// <seealso cref= #doPrivileged(PrivilegedAction,AccessControlContext) </seealso>
		/// <seealso cref= #doPrivileged(PrivilegedExceptionAction) </seealso>
		/// <seealso cref= #doPrivilegedWithCombiner(PrivilegedAction) </seealso>
		/// <seealso cref= java.security.DomainCombiner </seealso>

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @CallerSensitive public static native <T> T doPrivileged(PrivilegedAction<T> action);
//JAVA TO C# CONVERTER TODO TASK: Replace 'unknown' with the appropriate dll name:
		[DllImport("unknown")]
		public static extern <T> T doPrivileged(PrivilegedAction<T> action);

		/// <summary>
		/// Performs the specified {@code PrivilegedAction} with privileges
		/// enabled. The action is performed with <i>all</i> of the permissions
		/// possessed by the caller's protection domain.
		/// 
		/// <para> If the action's {@code run} method throws an (unchecked)
		/// exception, it will propagate through this method.
		/// 
		/// </para>
		/// <para> This method preserves the current AccessControlContext's
		/// DomainCombiner (which may be null) while the action is performed.
		/// 
		/// </para>
		/// </summary>
		/// @param <T> the type of the value returned by the PrivilegedAction's
		///                  {@code run} method.
		/// </param>
		/// <param name="action"> the action to be performed.
		/// </param>
		/// <returns> the value returned by the action's {@code run} method.
		/// </returns>
		/// <exception cref="NullPointerException"> if the action is {@code null}
		/// </exception>
		/// <seealso cref= #doPrivileged(PrivilegedAction) </seealso>
		/// <seealso cref= java.security.DomainCombiner
		/// 
		/// @since 1.6 </seealso>
//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @CallerSensitive public static <T> T doPrivilegedWithCombiner(PrivilegedAction<T> action)
		public static T doPrivilegedWithCombiner<T>(PrivilegedAction<T> action)
		{
			AccessControlContext acc = StackAccessControlContext;
			if (acc == null)
			{
				return AccessController.doPrivileged(action);
			}
			DomainCombiner dc = acc.AssignedCombiner;
			return AccessController.doPrivileged(action, PreserveCombiner(dc, Reflection.CallerClass));
		}


		/// <summary>
		/// Performs the specified {@code PrivilegedAction} with privileges
		/// enabled and restricted by the specified {@code AccessControlContext}.
		/// The action is performed with the intersection of the permissions
		/// possessed by the caller's protection domain, and those possessed
		/// by the domains represented by the specified {@code AccessControlContext}.
		/// <para>
		/// If the action's {@code run} method throws an (unchecked) exception,
		/// it will propagate through this method.
		/// </para>
		/// <para>
		/// If a security manager is installed and the specified
		/// {@code AccessControlContext} was not created by system code and the
		/// caller's {@code ProtectionDomain} has not been granted the
		/// {@literal "createAccessControlContext"}
		/// <seealso cref="java.security.SecurityPermission"/>, then the action is performed
		/// with no permissions.
		/// 
		/// </para>
		/// </summary>
		/// @param <T> the type of the value returned by the PrivilegedAction's
		///                  {@code run} method. </param>
		/// <param name="action"> the action to be performed. </param>
		/// <param name="context"> an <i>access control context</i>
		///                representing the restriction to be applied to the
		///                caller's domain's privileges before performing
		///                the specified action.  If the context is
		///                {@code null}, then no additional restriction is applied.
		/// </param>
		/// <returns> the value returned by the action's {@code run} method.
		/// </returns>
		/// <exception cref="NullPointerException"> if the action is {@code null}
		/// </exception>
		/// <seealso cref= #doPrivileged(PrivilegedAction) </seealso>
		/// <seealso cref= #doPrivileged(PrivilegedExceptionAction,AccessControlContext) </seealso>
//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @CallerSensitive public static native <T> T doPrivileged(PrivilegedAction<T> action, AccessControlContext context);
//JAVA TO C# CONVERTER TODO TASK: Replace 'unknown' with the appropriate dll name:
		[DllImport("unknown")]
		public static extern <T> T doPrivileged(PrivilegedAction<T> action, AccessControlContext context);


		/// <summary>
		/// Performs the specified {@code PrivilegedAction} with privileges
		/// enabled and restricted by the specified
		/// {@code AccessControlContext} and with a privilege scope limited
		/// by specified {@code Permission} arguments.
		/// 
		/// The action is performed with the intersection of the permissions
		/// possessed by the caller's protection domain, and those possessed
		/// by the domains represented by the specified
		/// {@code AccessControlContext}.
		/// <para>
		/// If the action's {@code run} method throws an (unchecked) exception,
		/// it will propagate through this method.
		/// </para>
		/// <para>
		/// If a security manager is installed and the specified
		/// {@code AccessControlContext} was not created by system code and the
		/// caller's {@code ProtectionDomain} has not been granted the
		/// {@literal "createAccessControlContext"}
		/// <seealso cref="java.security.SecurityPermission"/>, then the action is performed
		/// with no permissions.
		/// 
		/// </para>
		/// </summary>
		/// @param <T> the type of the value returned by the PrivilegedAction's
		///                  {@code run} method. </param>
		/// <param name="action"> the action to be performed. </param>
		/// <param name="context"> an <i>access control context</i>
		///                representing the restriction to be applied to the
		///                caller's domain's privileges before performing
		///                the specified action.  If the context is
		///                {@code null},
		///                then no additional restriction is applied. </param>
		/// <param name="perms"> the {@code Permission} arguments which limit the
		///              scope of the caller's privileges. The number of arguments
		///              is variable.
		/// </param>
		/// <returns> the value returned by the action's {@code run} method.
		/// </returns>
		/// <exception cref="NullPointerException"> if action or perms or any element of
		///         perms is {@code null}
		/// </exception>
		/// <seealso cref= #doPrivileged(PrivilegedAction) </seealso>
		/// <seealso cref= #doPrivileged(PrivilegedExceptionAction,AccessControlContext)
		/// 
		/// @since 1.8 </seealso>
//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @CallerSensitive public static <T> T doPrivileged(PrivilegedAction<T> action, AccessControlContext context, Permission... perms)
		public static T doPrivileged<T>(PrivilegedAction<T> action, AccessControlContext context, params Permission[] perms)
		{

			AccessControlContext parent = Context;
			if (perms == null)
			{
				throw new NullPointerException("null permissions parameter");
			}
			Class  caller = Reflection.CallerClass;
			return AccessController.doPrivileged(action, CreateWrapper(null, caller, parent, context, perms));
		}


		/// <summary>
		/// Performs the specified {@code PrivilegedAction} with privileges
		/// enabled and restricted by the specified
		/// {@code AccessControlContext} and with a privilege scope limited
		/// by specified {@code Permission} arguments.
		/// 
		/// The action is performed with the intersection of the permissions
		/// possessed by the caller's protection domain, and those possessed
		/// by the domains represented by the specified
		/// {@code AccessControlContext}.
		/// <para>
		/// If the action's {@code run} method throws an (unchecked) exception,
		/// it will propagate through this method.
		/// 
		/// </para>
		/// <para> This method preserves the current AccessControlContext's
		/// DomainCombiner (which may be null) while the action is performed.
		/// </para>
		/// <para>
		/// If a security manager is installed and the specified
		/// {@code AccessControlContext} was not created by system code and the
		/// caller's {@code ProtectionDomain} has not been granted the
		/// {@literal "createAccessControlContext"}
		/// <seealso cref="java.security.SecurityPermission"/>, then the action is performed
		/// with no permissions.
		/// 
		/// </para>
		/// </summary>
		/// @param <T> the type of the value returned by the PrivilegedAction's
		///                  {@code run} method. </param>
		/// <param name="action"> the action to be performed. </param>
		/// <param name="context"> an <i>access control context</i>
		///                representing the restriction to be applied to the
		///                caller's domain's privileges before performing
		///                the specified action.  If the context is
		///                {@code null},
		///                then no additional restriction is applied. </param>
		/// <param name="perms"> the {@code Permission} arguments which limit the
		///              scope of the caller's privileges. The number of arguments
		///              is variable.
		/// </param>
		/// <returns> the value returned by the action's {@code run} method.
		/// </returns>
		/// <exception cref="NullPointerException"> if action or perms or any element of
		///         perms is {@code null}
		/// </exception>
		/// <seealso cref= #doPrivileged(PrivilegedAction) </seealso>
		/// <seealso cref= #doPrivileged(PrivilegedExceptionAction,AccessControlContext) </seealso>
		/// <seealso cref= java.security.DomainCombiner
		/// 
		/// @since 1.8 </seealso>
//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @CallerSensitive public static <T> T doPrivilegedWithCombiner(PrivilegedAction<T> action, AccessControlContext context, Permission... perms)
		public static T doPrivilegedWithCombiner<T>(PrivilegedAction<T> action, AccessControlContext context, params Permission[] perms)
		{

			AccessControlContext parent = Context;
			DomainCombiner dc = parent.Combiner;
			if (dc == null && context != null)
			{
				dc = context.Combiner;
			}
			if (perms == null)
			{
				throw new NullPointerException("null permissions parameter");
			}
			Class  caller = Reflection.CallerClass;
			return AccessController.doPrivileged(action, CreateWrapper(dc, caller, parent, context, perms));
		}

		/// <summary>
		/// Performs the specified {@code PrivilegedExceptionAction} with
		/// privileges enabled.  The action is performed with <i>all</i> of the
		/// permissions possessed by the caller's protection domain.
		/// 
		/// <para> If the action's {@code run} method throws an <i>unchecked</i>
		/// exception, it will propagate through this method.
		/// 
		/// </para>
		/// <para> Note that any DomainCombiner associated with the current
		/// AccessControlContext will be ignored while the action is performed.
		/// 
		/// </para>
		/// </summary>
		/// @param <T> the type of the value returned by the
		///                  PrivilegedExceptionAction's {@code run} method.
		/// </param>
		/// <param name="action"> the action to be performed
		/// </param>
		/// <returns> the value returned by the action's {@code run} method
		/// </returns>
		/// <exception cref="PrivilegedActionException"> if the specified action's
		///         {@code run} method threw a <i>checked</i> exception </exception>
		/// <exception cref="NullPointerException"> if the action is {@code null}
		/// </exception>
		/// <seealso cref= #doPrivileged(PrivilegedAction) </seealso>
		/// <seealso cref= #doPrivileged(PrivilegedExceptionAction,AccessControlContext) </seealso>
		/// <seealso cref= #doPrivilegedWithCombiner(PrivilegedExceptionAction) </seealso>
		/// <seealso cref= java.security.DomainCombiner </seealso>
//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @CallerSensitive public static native <T> T doPrivileged(PrivilegedExceptionAction<T> action) throws PrivilegedActionException;
//JAVA TO C# CONVERTER TODO TASK: Replace 'unknown' with the appropriate dll name:
		[DllImport("unknown")]
		public static extern <T> T doPrivileged(PrivilegedExceptionAction<T> action);


		/// <summary>
		/// Performs the specified {@code PrivilegedExceptionAction} with
		/// privileges enabled.  The action is performed with <i>all</i> of the
		/// permissions possessed by the caller's protection domain.
		/// 
		/// <para> If the action's {@code run} method throws an <i>unchecked</i>
		/// exception, it will propagate through this method.
		/// 
		/// </para>
		/// <para> This method preserves the current AccessControlContext's
		/// DomainCombiner (which may be null) while the action is performed.
		/// 
		/// </para>
		/// </summary>
		/// @param <T> the type of the value returned by the
		///                  PrivilegedExceptionAction's {@code run} method.
		/// </param>
		/// <param name="action"> the action to be performed.
		/// </param>
		/// <returns> the value returned by the action's {@code run} method
		/// </returns>
		/// <exception cref="PrivilegedActionException"> if the specified action's
		///         {@code run} method threw a <i>checked</i> exception </exception>
		/// <exception cref="NullPointerException"> if the action is {@code null}
		/// </exception>
		/// <seealso cref= #doPrivileged(PrivilegedAction) </seealso>
		/// <seealso cref= #doPrivileged(PrivilegedExceptionAction,AccessControlContext) </seealso>
		/// <seealso cref= java.security.DomainCombiner
		/// 
		/// @since 1.6 </seealso>
//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @CallerSensitive public static <T> T doPrivilegedWithCombiner(PrivilegedExceptionAction<T> action) throws PrivilegedActionException
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
		public static T doPrivilegedWithCombiner<T>(PrivilegedExceptionAction<T> action)
		{
			AccessControlContext acc = StackAccessControlContext;
			if (acc == null)
			{
				return AccessController.doPrivileged(action);
			}
			DomainCombiner dc = acc.AssignedCombiner;
			return AccessController.doPrivileged(action, PreserveCombiner(dc, Reflection.CallerClass));
		}

		/// <summary>
		/// preserve the combiner across the doPrivileged call
		/// </summary>
		private static AccessControlContext PreserveCombiner(DomainCombiner combiner, Class caller)
		{
			return CreateWrapper(combiner, caller, null, null, null);
		}

		/// <summary>
		/// Create a wrapper to contain the limited privilege scope data.
		/// </summary>
		private static AccessControlContext CreateWrapper(DomainCombiner combiner, Class caller, AccessControlContext parent, AccessControlContext context, Permission[] perms)
		{
			ProtectionDomain callerPD = GetCallerPD(caller);
			// check if caller is authorized to create context
			if (context != null && !context.Authorized && System.SecurityManager != null && !callerPD.ImpliesCreateAccessControlContext())
			{
				ProtectionDomain nullPD = new ProtectionDomain(null, null);
				return new AccessControlContext(new ProtectionDomain[] {nullPD});
			}
			else
			{
				return new AccessControlContext(callerPD, combiner, parent, context, perms);
			}
		}

//JAVA TO C# CONVERTER WARNING: 'final' parameters are not allowed in .NET:
//ORIGINAL LINE: private static ProtectionDomain getCallerPD(final Class  caller)
		private static ProtectionDomain GetCallerPD(Class caller)
		{
			ProtectionDomain callerPd = doPrivileged(new PrivilegedActionAnonymousInnerClassHelper(caller));

			return callerPd;
		}

		private class PrivilegedActionAnonymousInnerClassHelper : PrivilegedAction<ProtectionDomain>
		{
			private Type Caller;

			public PrivilegedActionAnonymousInnerClassHelper(Type caller)
			{
				this.Caller = caller;
			}

			public virtual ProtectionDomain Run()
			{
				return Caller.ProtectionDomain;
			}
		}

		/// <summary>
		/// Performs the specified {@code PrivilegedExceptionAction} with
		/// privileges enabled and restricted by the specified
		/// {@code AccessControlContext}.  The action is performed with the
		/// intersection of the permissions possessed by the caller's
		/// protection domain, and those possessed by the domains represented by the
		/// specified {@code AccessControlContext}.
		/// <para>
		/// If the action's {@code run} method throws an <i>unchecked</i>
		/// exception, it will propagate through this method.
		/// </para>
		/// <para>
		/// If a security manager is installed and the specified
		/// {@code AccessControlContext} was not created by system code and the
		/// caller's {@code ProtectionDomain} has not been granted the
		/// {@literal "createAccessControlContext"}
		/// <seealso cref="java.security.SecurityPermission"/>, then the action is performed
		/// with no permissions.
		/// 
		/// </para>
		/// </summary>
		/// @param <T> the type of the value returned by the
		///                  PrivilegedExceptionAction's {@code run} method. </param>
		/// <param name="action"> the action to be performed </param>
		/// <param name="context"> an <i>access control context</i>
		///                representing the restriction to be applied to the
		///                caller's domain's privileges before performing
		///                the specified action.  If the context is
		///                {@code null}, then no additional restriction is applied.
		/// </param>
		/// <returns> the value returned by the action's {@code run} method
		/// </returns>
		/// <exception cref="PrivilegedActionException"> if the specified action's
		///         {@code run} method threw a <i>checked</i> exception </exception>
		/// <exception cref="NullPointerException"> if the action is {@code null}
		/// </exception>
		/// <seealso cref= #doPrivileged(PrivilegedAction) </seealso>
		/// <seealso cref= #doPrivileged(PrivilegedAction,AccessControlContext) </seealso>
//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @CallerSensitive public static native <T> T doPrivileged(PrivilegedExceptionAction<T> action, AccessControlContext context) throws PrivilegedActionException;
//JAVA TO C# CONVERTER TODO TASK: Replace 'unknown' with the appropriate dll name:
		[DllImport("unknown")]
		public static extern <T> T doPrivileged(PrivilegedExceptionAction<T> action, AccessControlContext context);


		/// <summary>
		/// Performs the specified {@code PrivilegedExceptionAction} with
		/// privileges enabled and restricted by the specified
		/// {@code AccessControlContext} and with a privilege scope limited by
		/// specified {@code Permission} arguments.
		/// 
		/// The action is performed with the intersection of the permissions
		/// possessed by the caller's protection domain, and those possessed
		/// by the domains represented by the specified
		/// {@code AccessControlContext}.
		/// <para>
		/// If the action's {@code run} method throws an (unchecked) exception,
		/// it will propagate through this method.
		/// </para>
		/// <para>
		/// If a security manager is installed and the specified
		/// {@code AccessControlContext} was not created by system code and the
		/// caller's {@code ProtectionDomain} has not been granted the
		/// {@literal "createAccessControlContext"}
		/// <seealso cref="java.security.SecurityPermission"/>, then the action is performed
		/// with no permissions.
		/// 
		/// </para>
		/// </summary>
		/// @param <T> the type of the value returned by the
		///                  PrivilegedExceptionAction's {@code run} method. </param>
		/// <param name="action"> the action to be performed. </param>
		/// <param name="context"> an <i>access control context</i>
		///                representing the restriction to be applied to the
		///                caller's domain's privileges before performing
		///                the specified action.  If the context is
		///                {@code null},
		///                then no additional restriction is applied. </param>
		/// <param name="perms"> the {@code Permission} arguments which limit the
		///              scope of the caller's privileges. The number of arguments
		///              is variable.
		/// </param>
		/// <returns> the value returned by the action's {@code run} method.
		/// </returns>
		/// <exception cref="PrivilegedActionException"> if the specified action's
		///         {@code run} method threw a <i>checked</i> exception </exception>
		/// <exception cref="NullPointerException"> if action or perms or any element of
		///         perms is {@code null}
		/// </exception>
		/// <seealso cref= #doPrivileged(PrivilegedAction) </seealso>
		/// <seealso cref= #doPrivileged(PrivilegedAction,AccessControlContext)
		/// 
		/// @since 1.8 </seealso>
//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @CallerSensitive public static <T> T doPrivileged(PrivilegedExceptionAction<T> action, AccessControlContext context, Permission... perms) throws PrivilegedActionException
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
		public static T doPrivileged<T>(PrivilegedExceptionAction<T> action, AccessControlContext context, params Permission[] perms)
		{
			AccessControlContext parent = Context;
			if (perms == null)
			{
				throw new NullPointerException("null permissions parameter");
			}
			Class  caller = Reflection.CallerClass;
			return AccessController.doPrivileged(action, CreateWrapper(null, caller, parent, context, perms));
		}


		/// <summary>
		/// Performs the specified {@code PrivilegedExceptionAction} with
		/// privileges enabled and restricted by the specified
		/// {@code AccessControlContext} and with a privilege scope limited by
		/// specified {@code Permission} arguments.
		/// 
		/// The action is performed with the intersection of the permissions
		/// possessed by the caller's protection domain, and those possessed
		/// by the domains represented by the specified
		/// {@code AccessControlContext}.
		/// <para>
		/// If the action's {@code run} method throws an (unchecked) exception,
		/// it will propagate through this method.
		/// 
		/// </para>
		/// <para> This method preserves the current AccessControlContext's
		/// DomainCombiner (which may be null) while the action is performed.
		/// </para>
		/// <para>
		/// If a security manager is installed and the specified
		/// {@code AccessControlContext} was not created by system code and the
		/// caller's {@code ProtectionDomain} has not been granted the
		/// {@literal "createAccessControlContext"}
		/// <seealso cref="java.security.SecurityPermission"/>, then the action is performed
		/// with no permissions.
		/// 
		/// </para>
		/// </summary>
		/// @param <T> the type of the value returned by the
		///                  PrivilegedExceptionAction's {@code run} method. </param>
		/// <param name="action"> the action to be performed. </param>
		/// <param name="context"> an <i>access control context</i>
		///                representing the restriction to be applied to the
		///                caller's domain's privileges before performing
		///                the specified action.  If the context is
		///                {@code null},
		///                then no additional restriction is applied. </param>
		/// <param name="perms"> the {@code Permission} arguments which limit the
		///              scope of the caller's privileges. The number of arguments
		///              is variable.
		/// </param>
		/// <returns> the value returned by the action's {@code run} method.
		/// </returns>
		/// <exception cref="PrivilegedActionException"> if the specified action's
		///         {@code run} method threw a <i>checked</i> exception </exception>
		/// <exception cref="NullPointerException"> if action or perms or any element of
		///         perms is {@code null}
		/// </exception>
		/// <seealso cref= #doPrivileged(PrivilegedAction) </seealso>
		/// <seealso cref= #doPrivileged(PrivilegedAction,AccessControlContext) </seealso>
		/// <seealso cref= java.security.DomainCombiner
		/// 
		/// @since 1.8 </seealso>
//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @CallerSensitive public static <T> T doPrivilegedWithCombiner(PrivilegedExceptionAction<T> action, AccessControlContext context, Permission... perms) throws PrivilegedActionException
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
		public static T doPrivilegedWithCombiner<T>(PrivilegedExceptionAction<T> action, AccessControlContext context, params Permission[] perms)
		{
			AccessControlContext parent = Context;
			DomainCombiner dc = parent.Combiner;
			if (dc == null && context != null)
			{
				dc = context.Combiner;
			}
			if (perms == null)
			{
				throw new NullPointerException("null permissions parameter");
			}
			Class  caller = Reflection.CallerClass;
			return AccessController.doPrivileged(action, CreateWrapper(dc, caller, parent, context, perms));
		}

		/// <summary>
		/// Returns the AccessControl context. i.e., it gets
		/// the protection domains of all the callers on the stack,
		/// starting at the first class with a non-null
		/// ProtectionDomain.
		/// </summary>
		/// <returns> the access control context based on the current stack or
		///         null if there was only privileged system code. </returns>

//JAVA TO C# CONVERTER TODO TASK: Replace 'unknown' with the appropriate dll name:
		[DllImport("unknown")]
		private static extern AccessControlContext getStackAccessControlContext();


		/// <summary>
		/// Returns the "inherited" AccessControl context. This is the context
		/// that existed when the thread was created. Package private so
		/// AccessControlContext can use it.
		/// </summary>

//JAVA TO C# CONVERTER TODO TASK: Replace 'unknown' with the appropriate dll name:
		[DllImport("unknown")]
		internal static extern AccessControlContext getInheritedAccessControlContext();

		/// <summary>
		/// This method takes a "snapshot" of the current calling context, which
		/// includes the current Thread's inherited AccessControlContext and any
		/// limited privilege scope, and places it in an AccessControlContext object.
		/// This context may then be checked at a later point, possibly in another thread.
		/// </summary>
		/// <seealso cref= AccessControlContext
		/// </seealso>
		/// <returns> the AccessControlContext based on the current context. </returns>

		public static AccessControlContext Context
		{
			get
			{
				AccessControlContext acc = StackAccessControlContext;
				if (acc == null)
				{
					// all we had was privileged system code. We don't want
					// to return null though, so we construct a real ACC.
					return new AccessControlContext(null, true);
				}
				else
				{
					return acc.Optimize();
				}
			}
		}

		/// <summary>
		/// Determines whether the access request indicated by the
		/// specified permission should be allowed or denied, based on
		/// the current AccessControlContext and security policy.
		/// This method quietly returns if the access request
		/// is permitted, or throws an AccessControlException otherwise. The
		/// getPermission method of the AccessControlException returns the
		/// {@code perm} Permission object instance.
		/// </summary>
		/// <param name="perm"> the requested permission.
		/// </param>
		/// <exception cref="AccessControlException"> if the specified permission
		///            is not permitted, based on the current security policy. </exception>
		/// <exception cref="NullPointerException"> if the specified permission
		///            is {@code null} and is checked based on the
		///            security policy currently in effect. </exception>

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public static void checkPermission(Permission perm) throws AccessControlException
		public static void CheckPermission(Permission perm)
		{
			//System.err.println("checkPermission "+perm);
			//Thread.currentThread().dumpStack();

			if (perm == null)
			{
				throw new NullPointerException("permission can't be null");
			}

			AccessControlContext stack = StackAccessControlContext;
			// if context is null, we had privileged system code on the stack.
			if (stack == null)
			{
				Debug debug = AccessControlContext.Debug;
				bool dumpDebug = false;
				if (debug != null)
				{
					dumpDebug = !Debug.isOn("codebase=");
//JAVA TO C# CONVERTER WARNING: The .NET Type.FullName property will not always yield results identical to the Java Class.getCanonicalName method:
					dumpDebug &= !Debug.isOn("permission=") || Debug.isOn("permission=" + perm.GetType().FullName);
				}

				if (dumpDebug && Debug.isOn("stack"))
				{
					Thread.DumpStack();
				}

				if (dumpDebug && Debug.isOn("domain"))
				{
					debug.println("domain (context is null)");
				}

				if (dumpDebug)
				{
					debug.println("access allowed " + perm);
				}
				return;
			}

			AccessControlContext acc = stack.Optimize();
			acc.CheckPermission(perm);
		}
	}

}
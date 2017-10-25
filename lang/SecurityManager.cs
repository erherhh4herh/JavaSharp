using System;
using System.Threading;
using System.Runtime.InteropServices;

/*
 * Copyright (c) 1995, 2013, Oracle and/or its affiliates. All rights reserved.
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

namespace java.lang
{


	using CallerSensitive = sun.reflect.CallerSensitive;
	using SecurityConstants = sun.security.util.SecurityConstants;

	/// <summary>
	/// The security manager is a class that allows
	/// applications to implement a security policy. It allows an
	/// application to determine, before performing a possibly unsafe or
	/// sensitive operation, what the operation is and whether
	/// it is being attempted in a security context that allows the
	/// operation to be performed. The
	/// application can allow or disallow the operation.
	/// <para>
	/// The <code>SecurityManager</code> class contains many methods with
	/// names that begin with the word <code>check</code>. These methods
	/// are called by various methods in the Java libraries before those
	/// methods perform certain potentially sensitive operations. The
	/// invocation of such a <code>check</code> method typically looks like this:
	/// <blockquote><pre>
	///     SecurityManager security = System.getSecurityManager();
	///     if (security != null) {
	///         security.check<i>XXX</i>(argument, &nbsp;.&nbsp;.&nbsp;.&nbsp;);
	///     }
	/// </pre></blockquote>
	/// </para>
	/// <para>
	/// The security manager is thereby given an opportunity to prevent
	/// completion of the operation by throwing an exception. A security
	/// manager routine simply returns if the operation is permitted, but
	/// throws a <code>SecurityException</code> if the operation is not
	/// permitted. The only exception to this convention is
	/// <code>checkTopLevelWindow</code>, which returns a
	/// <code>boolean</code> value.
	/// </para>
	/// <para>
	/// The current security manager is set by the
	/// <code>setSecurityManager</code> method in class
	/// <code>System</code>. The current security manager is obtained
	/// by the <code>getSecurityManager</code> method.
	/// </para>
	/// <para>
	/// The special method
	/// <seealso cref="SecurityManager#checkPermission(java.security.Permission)"/>
	/// determines whether an access request indicated by a specified
	/// permission should be granted or denied. The
	/// default implementation calls
	/// 
	/// <pre>
	///   AccessController.checkPermission(perm);
	/// </pre>
	/// 
	/// </para>
	/// <para>
	/// If a requested access is allowed,
	/// <code>checkPermission</code> returns quietly. If denied, a
	/// <code>SecurityException</code> is thrown.
	/// </para>
	/// <para>
	/// As of Java 2 SDK v1.2, the default implementation of each of the other
	/// <code>check</code> methods in <code>SecurityManager</code> is to
	/// call the <code>SecurityManager checkPermission</code> method
	/// to determine if the calling thread has permission to perform the requested
	/// operation.
	/// </para>
	/// <para>
	/// Note that the <code>checkPermission</code> method with
	/// just a single permission argument always performs security checks
	/// within the context of the currently executing thread.
	/// Sometimes a security check that should be made within a given context
	/// will actually need to be done from within a
	/// <i>different</i> context (for example, from within a worker thread).
	/// The <seealso cref="SecurityManager#getSecurityContext getSecurityContext"/> method
	/// and the {@link SecurityManager#checkPermission(java.security.Permission,
	/// java.lang.Object) checkPermission}
	/// method that includes a context argument are provided
	/// for this situation. The
	/// <code>getSecurityContext</code> method returns a "snapshot"
	/// of the current calling context. (The default implementation
	/// returns an AccessControlContext object.) A sample call is
	/// the following:
	/// 
	/// <pre>
	///   Object context = null;
	///   SecurityManager sm = System.getSecurityManager();
	///   if (sm != null) context = sm.getSecurityContext();
	/// </pre>
	/// 
	/// </para>
	/// <para>
	/// The <code>checkPermission</code> method
	/// that takes a context object in addition to a permission
	/// makes access decisions based on that context,
	/// rather than on that of the current execution thread.
	/// Code within a different context can thus call that method,
	/// passing the permission and the
	/// previously-saved context object. A sample call, using the
	/// SecurityManager <code>sm</code> obtained as in the previous example,
	/// is the following:
	/// 
	/// <pre>
	///   if (sm != null) sm.checkPermission(permission, context);
	/// </pre>
	/// 
	/// </para>
	/// <para>Permissions fall into these categories: File, Socket, Net,
	/// Security, Runtime, Property, AWT, Reflect, and Serializable.
	/// The classes managing these various
	/// permission categories are <code>java.io.FilePermission</code>,
	/// <code>java.net.SocketPermission</code>,
	/// <code>java.net.NetPermission</code>,
	/// <code>java.security.SecurityPermission</code>,
	/// <code>java.lang.RuntimePermission</code>,
	/// <code>java.util.PropertyPermission</code>,
	/// <code>java.awt.AWTPermission</code>,
	/// <code>java.lang.reflect.ReflectPermission</code>, and
	/// <code>java.io.SerializablePermission</code>.
	/// 
	/// </para>
	/// <para>All but the first two (FilePermission and SocketPermission) are
	/// subclasses of <code>java.security.BasicPermission</code>, which itself
	/// is an abstract subclass of the
	/// top-level class for permissions, which is
	/// <code>java.security.Permission</code>. BasicPermission defines the
	/// functionality needed for all permissions that contain a name
	/// that follows the hierarchical property naming convention
	/// (for example, "exitVM", "setFactory", "queuePrintJob", etc).
	/// An asterisk
	/// may appear at the end of the name, following a ".", or by itself, to
	/// signify a wildcard match. For example: "a.*" or "*" is valid,
	/// "*a" or "a*b" is not valid.
	/// 
	/// </para>
	/// <para>FilePermission and SocketPermission are subclasses of the
	/// top-level class for permissions
	/// (<code>java.security.Permission</code>). Classes like these
	/// that have a more complicated name syntax than that used by
	/// BasicPermission subclass directly from Permission rather than from
	/// BasicPermission. For example,
	/// for a <code>java.io.FilePermission</code> object, the permission name is
	/// the path name of a file (or directory).
	/// 
	/// </para>
	/// <para>Some of the permission classes have an "actions" list that tells
	/// the actions that are permitted for the object.  For example,
	/// for a <code>java.io.FilePermission</code> object, the actions list
	/// (such as "read, write") specifies which actions are granted for the
	/// specified file (or for files in the specified directory).
	/// 
	/// </para>
	/// <para>Other permission classes are for "named" permissions -
	/// ones that contain a name but no actions list; you either have the
	/// named permission or you don't.
	/// 
	/// </para>
	/// <para>Note: There is also a <code>java.security.AllPermission</code>
	/// permission that implies all permissions. It exists to simplify the work
	/// of system administrators who might need to perform multiple
	/// tasks that require all (or numerous) permissions.
	/// </para>
	/// <para>
	/// See <a href ="../../../technotes/guides/security/permissions.html">
	/// Permissions in the JDK</a> for permission-related information.
	/// This document includes, for example, a table listing the various SecurityManager
	/// <code>check</code> methods and the permission(s) the default
	/// implementation of each such method requires.
	/// It also contains a table of all the version 1.2 methods
	/// that require permissions, and for each such method tells
	/// which permission it requires.
	/// </para>
	/// <para>
	/// For more information about <code>SecurityManager</code> changes made in
	/// the JDK and advice regarding porting of 1.1-style security managers,
	/// see the <a href="../../../technotes/guides/security/index.html">security documentation</a>.
	/// 
	/// @author  Arthur van Hoff
	/// @author  Roland Schemers
	/// 
	/// </para>
	/// </summary>
	/// <seealso cref=     java.lang.ClassLoader </seealso>
	/// <seealso cref=     java.lang.SecurityException </seealso>
	/// <seealso cref=     java.lang.SecurityManager#checkTopLevelWindow(java.lang.Object)
	///  checkTopLevelWindow </seealso>
	/// <seealso cref=     java.lang.System#getSecurityManager() getSecurityManager </seealso>
	/// <seealso cref=     java.lang.System#setSecurityManager(java.lang.SecurityManager)
	///  setSecurityManager </seealso>
	/// <seealso cref=     java.security.AccessController AccessController </seealso>
	/// <seealso cref=     java.security.AccessControlContext AccessControlContext </seealso>
	/// <seealso cref=     java.security.AccessControlException AccessControlException </seealso>
	/// <seealso cref=     java.security.Permission </seealso>
	/// <seealso cref=     java.security.BasicPermission </seealso>
	/// <seealso cref=     java.io.FilePermission </seealso>
	/// <seealso cref=     java.net.SocketPermission </seealso>
	/// <seealso cref=     java.util.PropertyPermission </seealso>
	/// <seealso cref=     java.lang.RuntimePermission </seealso>
	/// <seealso cref=     java.awt.AWTPermission </seealso>
	/// <seealso cref=     java.security.Policy Policy </seealso>
	/// <seealso cref=     java.security.SecurityPermission SecurityPermission </seealso>
	/// <seealso cref=     java.security.ProtectionDomain
	/// 
	/// @since   JDK1.0 </seealso>
	public class SecurityManager
	{

		/// <summary>
		/// This field is <code>true</code> if there is a security check in
		/// progress; <code>false</code> otherwise.
		/// </summary>
		/// @deprecated This type of security checking is not recommended.
		///  It is recommended that the <code>checkPermission</code>
		///  call be used instead. 
		[Obsolete("This type of security checking is not recommended.")]
		protected internal bool InCheck_Renamed;

		/*
		 * Have we been initialized. Effective against finalizer attacks.
		 */
		private bool Initialized = false;


		/// <summary>
		/// returns true if the current context has been granted AllPermission
		/// </summary>
		private bool HasAllPermission()
		{
			try
			{
				CheckPermission(SecurityConstants.ALL_PERMISSION);
				return true;
			}
			catch (SecurityException)
			{
				return false;
			}
		}

		/// <summary>
		/// Tests if there is a security check in progress.
		/// </summary>
		/// <returns> the value of the <code>inCheck</code> field. This field
		///          should contain <code>true</code> if a security check is
		///          in progress,
		///          <code>false</code> otherwise. </returns>
		/// <seealso cref=     java.lang.SecurityManager#inCheck </seealso>
		/// @deprecated This type of security checking is not recommended.
		///  It is recommended that the <code>checkPermission</code>
		///  call be used instead. 
		[Obsolete("This type of security checking is not recommended.")]
		public virtual bool InCheck
		{
			get
			{
				return InCheck_Renamed;
			}
		}

		/// <summary>
		/// Constructs a new <code>SecurityManager</code>.
		/// 
		/// <para> If there is a security manager already installed, this method first
		/// calls the security manager's <code>checkPermission</code> method
		/// with the <code>RuntimePermission("createSecurityManager")</code>
		/// permission to ensure the calling thread has permission to create a new
		/// security manager.
		/// This may result in throwing a <code>SecurityException</code>.
		/// 
		/// </para>
		/// </summary>
		/// <exception cref="java.lang.SecurityException"> if a security manager already
		///             exists and its <code>checkPermission</code> method
		///             doesn't allow creation of a new security manager. </exception>
		/// <seealso cref=        java.lang.System#getSecurityManager() </seealso>
		/// <seealso cref=        #checkPermission(java.security.Permission) checkPermission </seealso>
		/// <seealso cref= java.lang.RuntimePermission </seealso>
		public SecurityManager()
		{
			lock (typeof(SecurityManager))
			{
				SecurityManager sm = System.SecurityManager;
				if (sm != null)
				{
					// ask the currently installed security manager if we
					// can create a new one.
					sm.CheckPermission(new RuntimePermission("createSecurityManager"));
				}
				Initialized = true;
			}
		}

		/// <summary>
		/// Returns the current execution stack as an array of classes.
		/// <para>
		/// The length of the array is the number of methods on the execution
		/// stack. The element at index <code>0</code> is the class of the
		/// currently executing method, the element at index <code>1</code> is
		/// the class of that method's caller, and so on.
		/// 
		/// </para>
		/// </summary>
		/// <returns>  the execution stack. </returns>
//JAVA TO C# CONVERTER TODO TASK: Replace 'unknown' with the appropriate dll name:
		[DllImport("unknown")]
		protected internal extern Class[] getClassContext();

		/// <summary>
		/// Returns the class loader of the most recently executing method from
		/// a class defined using a non-system class loader. A non-system
		/// class loader is defined as being a class loader that is not equal to
		/// the system class loader (as returned
		/// by <seealso cref="ClassLoader#getSystemClassLoader"/>) or one of its ancestors.
		/// <para>
		/// This method will return
		/// <code>null</code> in the following three cases:
		/// <ol>
		///   <li>All methods on the execution stack are from classes
		///   defined using the system class loader or one of its ancestors.
		/// 
		///   <li>All methods on the execution stack up to the first
		///   "privileged" caller
		///   (see <seealso cref="java.security.AccessController#doPrivileged"/>)
		///   are from classes
		///   defined using the system class loader or one of its ancestors.
		/// 
		///   <li> A call to <code>checkPermission</code> with
		///   <code>java.security.AllPermission</code> does not
		///   result in a SecurityException.
		/// 
		/// </ol>
		/// 
		/// </para>
		/// </summary>
		/// <returns>  the class loader of the most recent occurrence on the stack
		///          of a method from a class defined using a non-system class
		///          loader.
		/// </returns>
		/// @deprecated This type of security checking is not recommended.
		///  It is recommended that the <code>checkPermission</code>
		///  call be used instead.
		/// 
		/// <seealso cref=  java.lang.ClassLoader#getSystemClassLoader() getSystemClassLoader </seealso>
		/// <seealso cref=  #checkPermission(java.security.Permission) checkPermission </seealso>
		[Obsolete("This type of security checking is not recommended.")]
		protected internal virtual ClassLoader CurrentClassLoader()
		{
			ClassLoader cl = currentClassLoader0();
			if ((cl != null) && HasAllPermission())
			{
				cl = null;
			}
			return cl;
		}

//JAVA TO C# CONVERTER TODO TASK: Replace 'unknown' with the appropriate dll name:
		[DllImport("unknown")]
		private extern ClassLoader currentClassLoader0();

		/// <summary>
		/// Returns the class of the most recently executing method from
		/// a class defined using a non-system class loader. A non-system
		/// class loader is defined as being a class loader that is not equal to
		/// the system class loader (as returned
		/// by <seealso cref="ClassLoader#getSystemClassLoader"/>) or one of its ancestors.
		/// <para>
		/// This method will return
		/// <code>null</code> in the following three cases:
		/// <ol>
		///   <li>All methods on the execution stack are from classes
		///   defined using the system class loader or one of its ancestors.
		/// 
		///   <li>All methods on the execution stack up to the first
		///   "privileged" caller
		///   (see <seealso cref="java.security.AccessController#doPrivileged"/>)
		///   are from classes
		///   defined using the system class loader or one of its ancestors.
		/// 
		///   <li> A call to <code>checkPermission</code> with
		///   <code>java.security.AllPermission</code> does not
		///   result in a SecurityException.
		/// 
		/// </ol>
		/// 
		/// </para>
		/// </summary>
		/// <returns>  the class  of the most recent occurrence on the stack
		///          of a method from a class defined using a non-system class
		///          loader.
		/// </returns>
		/// @deprecated This type of security checking is not recommended.
		///  It is recommended that the <code>checkPermission</code>
		///  call be used instead.
		/// 
		/// <seealso cref=  java.lang.ClassLoader#getSystemClassLoader() getSystemClassLoader </seealso>
		/// <seealso cref=  #checkPermission(java.security.Permission) checkPermission </seealso>
		[Obsolete("This type of security checking is not recommended.")]
		protected internal virtual Class CurrentLoadedClass()
		{
			Class c = currentLoadedClass0();
			if ((c != null) && HasAllPermission())
			{
				c = null;
			}
			return c;
		}

		/// <summary>
		/// Returns the stack depth of the specified class.
		/// </summary>
		/// <param name="name">   the fully qualified name of the class to search for. </param>
		/// <returns>  the depth on the stack frame of the first occurrence of a
		///          method from a class with the specified name;
		///          <code>-1</code> if such a frame cannot be found. </returns>
		/// @deprecated This type of security checking is not recommended.
		///  It is recommended that the <code>checkPermission</code>
		///  call be used instead.
		///  
//JAVA TO C# CONVERTER TODO TASK: Replace 'unknown' with the appropriate dll name:
		[DllImport("unknown")]
		protected internal extern int classDepth(String name);

		/// <summary>
		/// Returns the stack depth of the most recently executing method
		/// from a class defined using a non-system class loader.  A non-system
		/// class loader is defined as being a class loader that is not equal to
		/// the system class loader (as returned
		/// by <seealso cref="ClassLoader#getSystemClassLoader"/>) or one of its ancestors.
		/// <para>
		/// This method will return
		/// -1 in the following three cases:
		/// <ol>
		///   <li>All methods on the execution stack are from classes
		///   defined using the system class loader or one of its ancestors.
		/// 
		///   <li>All methods on the execution stack up to the first
		///   "privileged" caller
		///   (see <seealso cref="java.security.AccessController#doPrivileged"/>)
		///   are from classes
		///   defined using the system class loader or one of its ancestors.
		/// 
		///   <li> A call to <code>checkPermission</code> with
		///   <code>java.security.AllPermission</code> does not
		///   result in a SecurityException.
		/// 
		/// </ol>
		/// 
		/// </para>
		/// </summary>
		/// <returns> the depth on the stack frame of the most recent occurrence of
		///          a method from a class defined using a non-system class loader.
		/// </returns>
		/// @deprecated This type of security checking is not recommended.
		///  It is recommended that the <code>checkPermission</code>
		///  call be used instead.
		/// 
		/// <seealso cref=   java.lang.ClassLoader#getSystemClassLoader() getSystemClassLoader </seealso>
		/// <seealso cref=   #checkPermission(java.security.Permission) checkPermission </seealso>
		[Obsolete("This type of security checking is not recommended.")]
		protected internal virtual int ClassLoaderDepth()
		{
			int depth = classLoaderDepth0();
			if (depth != -1)
			{
				if (HasAllPermission())
				{
					depth = -1;
				}
				else
				{
					depth--; // make sure we don't include ourself
				}
			}
			return depth;
		}

//JAVA TO C# CONVERTER TODO TASK: Replace 'unknown' with the appropriate dll name:
		[DllImport("unknown")]
		private extern int classLoaderDepth0();

		/// <summary>
		/// Tests if a method from a class with the specified
		///         name is on the execution stack.
		/// </summary>
		/// <param name="name">   the fully qualified name of the class. </param>
		/// <returns> <code>true</code> if a method from a class with the specified
		///         name is on the execution stack; <code>false</code> otherwise. </returns>
		/// @deprecated This type of security checking is not recommended.
		///  It is recommended that the <code>checkPermission</code>
		///  call be used instead. 
		[Obsolete("This type of security checking is not recommended.")]
		protected internal virtual bool InClass(String name)
		{
			return classDepth(name) >= 0;
		}

		/// <summary>
		/// Basically, tests if a method from a class defined using a
		///          class loader is on the execution stack.
		/// </summary>
		/// <returns>  <code>true</code> if a call to <code>currentClassLoader</code>
		///          has a non-null return value.
		/// </returns>
		/// @deprecated This type of security checking is not recommended.
		///  It is recommended that the <code>checkPermission</code>
		///  call be used instead. 
		/// <seealso cref=        #currentClassLoader() currentClassLoader </seealso>
		[Obsolete("This type of security checking is not recommended.")]
		protected internal virtual bool InClassLoader()
		{
			return CurrentClassLoader() != null;
		}

		/// <summary>
		/// Creates an object that encapsulates the current execution
		/// environment. The result of this method is used, for example, by the
		/// three-argument <code>checkConnect</code> method and by the
		/// two-argument <code>checkRead</code> method.
		/// These methods are needed because a trusted method may be called
		/// on to read a file or open a socket on behalf of another method.
		/// The trusted method needs to determine if the other (possibly
		/// untrusted) method would be allowed to perform the operation on its
		/// own.
		/// <para> The default implementation of this method is to return
		/// an <code>AccessControlContext</code> object.
		/// 
		/// </para>
		/// </summary>
		/// <returns>  an implementation-dependent object that encapsulates
		///          sufficient information about the current execution environment
		///          to perform some security checks later. </returns>
		/// <seealso cref=     java.lang.SecurityManager#checkConnect(java.lang.String, int,
		///   java.lang.Object) checkConnect </seealso>
		/// <seealso cref=     java.lang.SecurityManager#checkRead(java.lang.String,
		///   java.lang.Object) checkRead </seealso>
		/// <seealso cref=     java.security.AccessControlContext AccessControlContext </seealso>
		public virtual Object SecurityContext
		{
			get
			{
				return AccessController.Context;
			}
		}

		/// <summary>
		/// Throws a <code>SecurityException</code> if the requested
		/// access, specified by the given permission, is not permitted based
		/// on the security policy currently in effect.
		/// <para>
		/// This method calls <code>AccessController.checkPermission</code>
		/// with the given permission.
		/// 
		/// </para>
		/// </summary>
		/// <param name="perm">   the requested permission. </param>
		/// <exception cref="SecurityException"> if access is not permitted based on
		///            the current security policy. </exception>
		/// <exception cref="NullPointerException"> if the permission argument is
		///            <code>null</code>.
		/// @since     1.2 </exception>
		public virtual void CheckPermission(Permission perm)
		{
			java.security.AccessController.CheckPermission(perm);
		}

		/// <summary>
		/// Throws a <code>SecurityException</code> if the
		/// specified security context is denied access to the resource
		/// specified by the given permission.
		/// The context must be a security
		/// context returned by a previous call to
		/// <code>getSecurityContext</code> and the access control
		/// decision is based upon the configured security policy for
		/// that security context.
		/// <para>
		/// If <code>context</code> is an instance of
		/// <code>AccessControlContext</code> then the
		/// <code>AccessControlContext.checkPermission</code> method is
		/// invoked with the specified permission.
		/// </para>
		/// <para>
		/// If <code>context</code> is not an instance of
		/// <code>AccessControlContext</code> then a
		/// <code>SecurityException</code> is thrown.
		/// 
		/// </para>
		/// </summary>
		/// <param name="perm">      the specified permission </param>
		/// <param name="context">   a system-dependent security context. </param>
		/// <exception cref="SecurityException">  if the specified security context
		///             is not an instance of <code>AccessControlContext</code>
		///             (e.g., is <code>null</code>), or is denied access to the
		///             resource specified by the given permission. </exception>
		/// <exception cref="NullPointerException"> if the permission argument is
		///             <code>null</code>. </exception>
		/// <seealso cref=        java.lang.SecurityManager#getSecurityContext() </seealso>
		/// <seealso cref= java.security.AccessControlContext#checkPermission(java.security.Permission)
		/// @since      1.2 </seealso>
		public virtual void CheckPermission(Permission perm, Object context)
		{
			if (context is AccessControlContext)
			{
				((AccessControlContext)context).CheckPermission(perm);
			}
			else
			{
				throw new SecurityException();
			}
		}

		/// <summary>
		/// Throws a <code>SecurityException</code> if the
		/// calling thread is not allowed to create a new class loader.
		/// <para>
		/// This method calls <code>checkPermission</code> with the
		/// <code>RuntimePermission("createClassLoader")</code>
		/// permission.
		/// </para>
		/// <para>
		/// If you override this method, then you should make a call to
		/// <code>super.checkCreateClassLoader</code>
		/// at the point the overridden method would normally throw an
		/// exception.
		/// 
		/// </para>
		/// </summary>
		/// <exception cref="SecurityException"> if the calling thread does not
		///             have permission
		///             to create a new class loader. </exception>
		/// <seealso cref=        java.lang.ClassLoader#ClassLoader() </seealso>
		/// <seealso cref=        #checkPermission(java.security.Permission) checkPermission </seealso>
		public virtual void CheckCreateClassLoader()
		{
			CheckPermission(SecurityConstants.CREATE_CLASSLOADER_PERMISSION);
		}

		/// <summary>
		/// reference to the root thread group, used for the checkAccess
		/// methods.
		/// </summary>

		private static ThreadGroup RootGroup_Renamed = RootGroup;

		private static ThreadGroup RootGroup
		{
			get
			{
				ThreadGroup root = Thread.CurrentThread.ThreadGroup;
				while (root.Parent != null)
				{
					root = root.Parent;
				}
				return root;
			}
		}

		/// <summary>
		/// Throws a <code>SecurityException</code> if the
		/// calling thread is not allowed to modify the thread argument.
		/// <para>
		/// This method is invoked for the current security manager by the
		/// <code>stop</code>, <code>suspend</code>, <code>resume</code>,
		/// <code>setPriority</code>, <code>setName</code>, and
		/// <code>setDaemon</code> methods of class <code>Thread</code>.
		/// </para>
		/// <para>
		/// If the thread argument is a system thread (belongs to
		/// the thread group with a <code>null</code> parent) then
		/// this method calls <code>checkPermission</code> with the
		/// <code>RuntimePermission("modifyThread")</code> permission.
		/// If the thread argument is <i>not</i> a system thread,
		/// this method just returns silently.
		/// </para>
		/// <para>
		/// Applications that want a stricter policy should override this
		/// method. If this method is overridden, the method that overrides
		/// it should additionally check to see if the calling thread has the
		/// <code>RuntimePermission("modifyThread")</code> permission, and
		/// if so, return silently. This is to ensure that code granted
		/// that permission (such as the JDK itself) is allowed to
		/// manipulate any thread.
		/// </para>
		/// <para>
		/// If this method is overridden, then
		/// <code>super.checkAccess</code> should
		/// be called by the first statement in the overridden method, or the
		/// equivalent security check should be placed in the overridden method.
		/// 
		/// </para>
		/// </summary>
		/// <param name="t">   the thread to be checked. </param>
		/// <exception cref="SecurityException">  if the calling thread does not have
		///             permission to modify the thread. </exception>
		/// <exception cref="NullPointerException"> if the thread argument is
		///             <code>null</code>. </exception>
		/// <seealso cref=        java.lang.Thread#resume() resume </seealso>
		/// <seealso cref=        java.lang.Thread#setDaemon(boolean) setDaemon </seealso>
		/// <seealso cref=        java.lang.Thread#setName(java.lang.String) setName </seealso>
		/// <seealso cref=        java.lang.Thread#setPriority(int) setPriority </seealso>
		/// <seealso cref=        java.lang.Thread#stop() stop </seealso>
		/// <seealso cref=        java.lang.Thread#suspend() suspend </seealso>
		/// <seealso cref=        #checkPermission(java.security.Permission) checkPermission </seealso>
		public virtual void CheckAccess(Thread t)
		{
			if (t == null)
			{
				throw new NullPointerException("thread can't be null");
			}
			if (t.ThreadGroup == RootGroup_Renamed)
			{
				CheckPermission(SecurityConstants.MODIFY_THREAD_PERMISSION);
			}
			else
			{
				// just return
			}
		}
		/// <summary>
		/// Throws a <code>SecurityException</code> if the
		/// calling thread is not allowed to modify the thread group argument.
		/// <para>
		/// This method is invoked for the current security manager when a
		/// new child thread or child thread group is created, and by the
		/// <code>setDaemon</code>, <code>setMaxPriority</code>,
		/// <code>stop</code>, <code>suspend</code>, <code>resume</code>, and
		/// <code>destroy</code> methods of class <code>ThreadGroup</code>.
		/// </para>
		/// <para>
		/// If the thread group argument is the system thread group (
		/// has a <code>null</code> parent) then
		/// this method calls <code>checkPermission</code> with the
		/// <code>RuntimePermission("modifyThreadGroup")</code> permission.
		/// If the thread group argument is <i>not</i> the system thread group,
		/// this method just returns silently.
		/// </para>
		/// <para>
		/// Applications that want a stricter policy should override this
		/// method. If this method is overridden, the method that overrides
		/// it should additionally check to see if the calling thread has the
		/// <code>RuntimePermission("modifyThreadGroup")</code> permission, and
		/// if so, return silently. This is to ensure that code granted
		/// that permission (such as the JDK itself) is allowed to
		/// manipulate any thread.
		/// </para>
		/// <para>
		/// If this method is overridden, then
		/// <code>super.checkAccess</code> should
		/// be called by the first statement in the overridden method, or the
		/// equivalent security check should be placed in the overridden method.
		/// 
		/// </para>
		/// </summary>
		/// <param name="g">   the thread group to be checked. </param>
		/// <exception cref="SecurityException">  if the calling thread does not have
		///             permission to modify the thread group. </exception>
		/// <exception cref="NullPointerException"> if the thread group argument is
		///             <code>null</code>. </exception>
		/// <seealso cref=        java.lang.ThreadGroup#destroy() destroy </seealso>
		/// <seealso cref=        java.lang.ThreadGroup#resume() resume </seealso>
		/// <seealso cref=        java.lang.ThreadGroup#setDaemon(boolean) setDaemon </seealso>
		/// <seealso cref=        java.lang.ThreadGroup#setMaxPriority(int) setMaxPriority </seealso>
		/// <seealso cref=        java.lang.ThreadGroup#stop() stop </seealso>
		/// <seealso cref=        java.lang.ThreadGroup#suspend() suspend </seealso>
		/// <seealso cref=        #checkPermission(java.security.Permission) checkPermission </seealso>
		public virtual void CheckAccess(ThreadGroup g)
		{
			if (g == null)
			{
				throw new NullPointerException("thread group can't be null");
			}
			if (g == RootGroup_Renamed)
			{
				CheckPermission(SecurityConstants.MODIFY_THREADGROUP_PERMISSION);
			}
			else
			{
				// just return
			}
		}

		/// <summary>
		/// Throws a <code>SecurityException</code> if the
		/// calling thread is not allowed to cause the Java Virtual Machine to
		/// halt with the specified status code.
		/// <para>
		/// This method is invoked for the current security manager by the
		/// <code>exit</code> method of class <code>Runtime</code>. A status
		/// of <code>0</code> indicates success; other values indicate various
		/// errors.
		/// </para>
		/// <para>
		/// This method calls <code>checkPermission</code> with the
		/// <code>RuntimePermission("exitVM."+status)</code> permission.
		/// </para>
		/// <para>
		/// If you override this method, then you should make a call to
		/// <code>super.checkExit</code>
		/// at the point the overridden method would normally throw an
		/// exception.
		/// 
		/// </para>
		/// </summary>
		/// <param name="status">   the exit status. </param>
		/// <exception cref="SecurityException"> if the calling thread does not have
		///              permission to halt the Java Virtual Machine with
		///              the specified status. </exception>
		/// <seealso cref=        java.lang.Runtime#exit(int) exit </seealso>
		/// <seealso cref=        #checkPermission(java.security.Permission) checkPermission </seealso>
		public virtual void CheckExit(int status)
		{
			CheckPermission(new RuntimePermission("exitVM." + status));
		}

		/// <summary>
		/// Throws a <code>SecurityException</code> if the
		/// calling thread is not allowed to create a subprocess.
		/// <para>
		/// This method is invoked for the current security manager by the
		/// <code>exec</code> methods of class <code>Runtime</code>.
		/// </para>
		/// <para>
		/// This method calls <code>checkPermission</code> with the
		/// <code>FilePermission(cmd,"execute")</code> permission
		/// if cmd is an absolute path, otherwise it calls
		/// <code>checkPermission</code> with
		/// <code>FilePermission("&lt;&lt;ALL FILES&gt;&gt;","execute")</code>.
		/// </para>
		/// <para>
		/// If you override this method, then you should make a call to
		/// <code>super.checkExec</code>
		/// at the point the overridden method would normally throw an
		/// exception.
		/// 
		/// </para>
		/// </summary>
		/// <param name="cmd">   the specified system command. </param>
		/// <exception cref="SecurityException"> if the calling thread does not have
		///             permission to create a subprocess. </exception>
		/// <exception cref="NullPointerException"> if the <code>cmd</code> argument is
		///             <code>null</code>. </exception>
		/// <seealso cref=     java.lang.Runtime#exec(java.lang.String) </seealso>
		/// <seealso cref=     java.lang.Runtime#exec(java.lang.String, java.lang.String[]) </seealso>
		/// <seealso cref=     java.lang.Runtime#exec(java.lang.String[]) </seealso>
		/// <seealso cref=     java.lang.Runtime#exec(java.lang.String[], java.lang.String[]) </seealso>
		/// <seealso cref=     #checkPermission(java.security.Permission) checkPermission </seealso>
		public virtual void CheckExec(String cmd)
		{
			File f = new File(cmd);
			if (f.Absolute)
			{
				CheckPermission(new FilePermission(cmd, SecurityConstants.FILE_EXECUTE_ACTION));
			}
			else
			{
				CheckPermission(new FilePermission("<<ALL FILES>>", SecurityConstants.FILE_EXECUTE_ACTION));
			}
		}

		/// <summary>
		/// Throws a <code>SecurityException</code> if the
		/// calling thread is not allowed to dynamic link the library code
		/// specified by the string argument file. The argument is either a
		/// simple library name or a complete filename.
		/// <para>
		/// This method is invoked for the current security manager by
		/// methods <code>load</code> and <code>loadLibrary</code> of class
		/// <code>Runtime</code>.
		/// </para>
		/// <para>
		/// This method calls <code>checkPermission</code> with the
		/// <code>RuntimePermission("loadLibrary."+lib)</code> permission.
		/// </para>
		/// <para>
		/// If you override this method, then you should make a call to
		/// <code>super.checkLink</code>
		/// at the point the overridden method would normally throw an
		/// exception.
		/// 
		/// </para>
		/// </summary>
		/// <param name="lib">   the name of the library. </param>
		/// <exception cref="SecurityException"> if the calling thread does not have
		///             permission to dynamically link the library. </exception>
		/// <exception cref="NullPointerException"> if the <code>lib</code> argument is
		///             <code>null</code>. </exception>
		/// <seealso cref=        java.lang.Runtime#load(java.lang.String) </seealso>
		/// <seealso cref=        java.lang.Runtime#loadLibrary(java.lang.String) </seealso>
		/// <seealso cref=        #checkPermission(java.security.Permission) checkPermission </seealso>
		public virtual void CheckLink(String lib)
		{
			if (lib == null)
			{
				throw new NullPointerException("library can't be null");
			}
			CheckPermission(new RuntimePermission("loadLibrary." + lib));
		}

		/// <summary>
		/// Throws a <code>SecurityException</code> if the
		/// calling thread is not allowed to read from the specified file
		/// descriptor.
		/// <para>
		/// This method calls <code>checkPermission</code> with the
		/// <code>RuntimePermission("readFileDescriptor")</code>
		/// permission.
		/// </para>
		/// <para>
		/// If you override this method, then you should make a call to
		/// <code>super.checkRead</code>
		/// at the point the overridden method would normally throw an
		/// exception.
		/// 
		/// </para>
		/// </summary>
		/// <param name="fd">   the system-dependent file descriptor. </param>
		/// <exception cref="SecurityException">  if the calling thread does not have
		///             permission to access the specified file descriptor. </exception>
		/// <exception cref="NullPointerException"> if the file descriptor argument is
		///             <code>null</code>. </exception>
		/// <seealso cref=        java.io.FileDescriptor </seealso>
		/// <seealso cref=        #checkPermission(java.security.Permission) checkPermission </seealso>
		public virtual void CheckRead(FileDescriptor fd)
		{
			if (fd == null)
			{
				throw new NullPointerException("file descriptor can't be null");
			}
			CheckPermission(new RuntimePermission("readFileDescriptor"));
		}

		/// <summary>
		/// Throws a <code>SecurityException</code> if the
		/// calling thread is not allowed to read the file specified by the
		/// string argument.
		/// <para>
		/// This method calls <code>checkPermission</code> with the
		/// <code>FilePermission(file,"read")</code> permission.
		/// </para>
		/// <para>
		/// If you override this method, then you should make a call to
		/// <code>super.checkRead</code>
		/// at the point the overridden method would normally throw an
		/// exception.
		/// 
		/// </para>
		/// </summary>
		/// <param name="file">   the system-dependent file name. </param>
		/// <exception cref="SecurityException"> if the calling thread does not have
		///             permission to access the specified file. </exception>
		/// <exception cref="NullPointerException"> if the <code>file</code> argument is
		///             <code>null</code>. </exception>
		/// <seealso cref=        #checkPermission(java.security.Permission) checkPermission </seealso>
		public virtual void CheckRead(String file)
		{
			CheckPermission(new FilePermission(file, SecurityConstants.FILE_READ_ACTION));
		}

		/// <summary>
		/// Throws a <code>SecurityException</code> if the
		/// specified security context is not allowed to read the file
		/// specified by the string argument. The context must be a security
		/// context returned by a previous call to
		/// <code>getSecurityContext</code>.
		/// <para> If <code>context</code> is an instance of
		/// <code>AccessControlContext</code> then the
		/// <code>AccessControlContext.checkPermission</code> method will
		/// be invoked with the <code>FilePermission(file,"read")</code> permission.
		/// </para>
		/// <para> If <code>context</code> is not an instance of
		/// <code>AccessControlContext</code> then a
		/// <code>SecurityException</code> is thrown.
		/// </para>
		/// <para>
		/// If you override this method, then you should make a call to
		/// <code>super.checkRead</code>
		/// at the point the overridden method would normally throw an
		/// exception.
		/// 
		/// </para>
		/// </summary>
		/// <param name="file">      the system-dependent filename. </param>
		/// <param name="context">   a system-dependent security context. </param>
		/// <exception cref="SecurityException">  if the specified security context
		///             is not an instance of <code>AccessControlContext</code>
		///             (e.g., is <code>null</code>), or does not have permission
		///             to read the specified file. </exception>
		/// <exception cref="NullPointerException"> if the <code>file</code> argument is
		///             <code>null</code>. </exception>
		/// <seealso cref=        java.lang.SecurityManager#getSecurityContext() </seealso>
		/// <seealso cref=        java.security.AccessControlContext#checkPermission(java.security.Permission) </seealso>
		public virtual void CheckRead(String file, Object context)
		{
			CheckPermission(new FilePermission(file, SecurityConstants.FILE_READ_ACTION), context);
		}

		/// <summary>
		/// Throws a <code>SecurityException</code> if the
		/// calling thread is not allowed to write to the specified file
		/// descriptor.
		/// <para>
		/// This method calls <code>checkPermission</code> with the
		/// <code>RuntimePermission("writeFileDescriptor")</code>
		/// permission.
		/// </para>
		/// <para>
		/// If you override this method, then you should make a call to
		/// <code>super.checkWrite</code>
		/// at the point the overridden method would normally throw an
		/// exception.
		/// 
		/// </para>
		/// </summary>
		/// <param name="fd">   the system-dependent file descriptor. </param>
		/// <exception cref="SecurityException">  if the calling thread does not have
		///             permission to access the specified file descriptor. </exception>
		/// <exception cref="NullPointerException"> if the file descriptor argument is
		///             <code>null</code>. </exception>
		/// <seealso cref=        java.io.FileDescriptor </seealso>
		/// <seealso cref=        #checkPermission(java.security.Permission) checkPermission </seealso>
		public virtual void CheckWrite(FileDescriptor fd)
		{
			if (fd == null)
			{
				throw new NullPointerException("file descriptor can't be null");
			}
			CheckPermission(new RuntimePermission("writeFileDescriptor"));

		}

		/// <summary>
		/// Throws a <code>SecurityException</code> if the
		/// calling thread is not allowed to write to the file specified by
		/// the string argument.
		/// <para>
		/// This method calls <code>checkPermission</code> with the
		/// <code>FilePermission(file,"write")</code> permission.
		/// </para>
		/// <para>
		/// If you override this method, then you should make a call to
		/// <code>super.checkWrite</code>
		/// at the point the overridden method would normally throw an
		/// exception.
		/// 
		/// </para>
		/// </summary>
		/// <param name="file">   the system-dependent filename. </param>
		/// <exception cref="SecurityException">  if the calling thread does not
		///             have permission to access the specified file. </exception>
		/// <exception cref="NullPointerException"> if the <code>file</code> argument is
		///             <code>null</code>. </exception>
		/// <seealso cref=        #checkPermission(java.security.Permission) checkPermission </seealso>
		public virtual void CheckWrite(String file)
		{
			CheckPermission(new FilePermission(file, SecurityConstants.FILE_WRITE_ACTION));
		}

		/// <summary>
		/// Throws a <code>SecurityException</code> if the
		/// calling thread is not allowed to delete the specified file.
		/// <para>
		/// This method is invoked for the current security manager by the
		/// <code>delete</code> method of class <code>File</code>.
		/// </para>
		/// <para>
		/// This method calls <code>checkPermission</code> with the
		/// <code>FilePermission(file,"delete")</code> permission.
		/// </para>
		/// <para>
		/// If you override this method, then you should make a call to
		/// <code>super.checkDelete</code>
		/// at the point the overridden method would normally throw an
		/// exception.
		/// 
		/// </para>
		/// </summary>
		/// <param name="file">   the system-dependent filename. </param>
		/// <exception cref="SecurityException"> if the calling thread does not
		///             have permission to delete the file. </exception>
		/// <exception cref="NullPointerException"> if the <code>file</code> argument is
		///             <code>null</code>. </exception>
		/// <seealso cref=        java.io.File#delete() </seealso>
		/// <seealso cref=        #checkPermission(java.security.Permission) checkPermission </seealso>
		public virtual void CheckDelete(String file)
		{
			CheckPermission(new FilePermission(file, SecurityConstants.FILE_DELETE_ACTION));
		}

		/// <summary>
		/// Throws a <code>SecurityException</code> if the
		/// calling thread is not allowed to open a socket connection to the
		/// specified host and port number.
		/// <para>
		/// A port number of <code>-1</code> indicates that the calling
		/// method is attempting to determine the IP address of the specified
		/// host name.
		/// </para>
		/// <para>
		/// This method calls <code>checkPermission</code> with the
		/// <code>SocketPermission(host+":"+port,"connect")</code> permission if
		/// the port is not equal to -1. If the port is equal to -1, then
		/// it calls <code>checkPermission</code> with the
		/// <code>SocketPermission(host,"resolve")</code> permission.
		/// </para>
		/// <para>
		/// If you override this method, then you should make a call to
		/// <code>super.checkConnect</code>
		/// at the point the overridden method would normally throw an
		/// exception.
		/// 
		/// </para>
		/// </summary>
		/// <param name="host">   the host name port to connect to. </param>
		/// <param name="port">   the protocol port to connect to. </param>
		/// <exception cref="SecurityException">  if the calling thread does not have
		///             permission to open a socket connection to the specified
		///               <code>host</code> and <code>port</code>. </exception>
		/// <exception cref="NullPointerException"> if the <code>host</code> argument is
		///             <code>null</code>. </exception>
		/// <seealso cref=        #checkPermission(java.security.Permission) checkPermission </seealso>
		public virtual void CheckConnect(String host, int port)
		{
			if (host == null)
			{
				throw new NullPointerException("host can't be null");
			}
			if (!host.StartsWith("[") && host.IndexOf(':') != -1)
			{
				host = "[" + host + "]";
			}
			if (port == -1)
			{
				CheckPermission(new SocketPermission(host, SecurityConstants.SOCKET_RESOLVE_ACTION));
			}
			else
			{
				CheckPermission(new SocketPermission(host + ":" + port, SecurityConstants.SOCKET_CONNECT_ACTION));
			}
		}

		/// <summary>
		/// Throws a <code>SecurityException</code> if the
		/// specified security context is not allowed to open a socket
		/// connection to the specified host and port number.
		/// <para>
		/// A port number of <code>-1</code> indicates that the calling
		/// method is attempting to determine the IP address of the specified
		/// host name.
		/// </para>
		/// <para> If <code>context</code> is not an instance of
		/// <code>AccessControlContext</code> then a
		/// <code>SecurityException</code> is thrown.
		/// </para>
		/// <para>
		/// Otherwise, the port number is checked. If it is not equal
		/// to -1, the <code>context</code>'s <code>checkPermission</code>
		/// method is called with a
		/// <code>SocketPermission(host+":"+port,"connect")</code> permission.
		/// If the port is equal to -1, then
		/// the <code>context</code>'s <code>checkPermission</code> method
		/// is called with a
		/// <code>SocketPermission(host,"resolve")</code> permission.
		/// </para>
		/// <para>
		/// If you override this method, then you should make a call to
		/// <code>super.checkConnect</code>
		/// at the point the overridden method would normally throw an
		/// exception.
		/// 
		/// </para>
		/// </summary>
		/// <param name="host">      the host name port to connect to. </param>
		/// <param name="port">      the protocol port to connect to. </param>
		/// <param name="context">   a system-dependent security context. </param>
		/// <exception cref="SecurityException"> if the specified security context
		///             is not an instance of <code>AccessControlContext</code>
		///             (e.g., is <code>null</code>), or does not have permission
		///             to open a socket connection to the specified
		///             <code>host</code> and <code>port</code>. </exception>
		/// <exception cref="NullPointerException"> if the <code>host</code> argument is
		///             <code>null</code>. </exception>
		/// <seealso cref=        java.lang.SecurityManager#getSecurityContext() </seealso>
		/// <seealso cref=        java.security.AccessControlContext#checkPermission(java.security.Permission) </seealso>
		public virtual void CheckConnect(String host, int port, Object context)
		{
			if (host == null)
			{
				throw new NullPointerException("host can't be null");
			}
			if (!host.StartsWith("[") && host.IndexOf(':') != -1)
			{
				host = "[" + host + "]";
			}
			if (port == -1)
			{
				CheckPermission(new SocketPermission(host, SecurityConstants.SOCKET_RESOLVE_ACTION), context);
			}
			else
			{
				CheckPermission(new SocketPermission(host + ":" + port, SecurityConstants.SOCKET_CONNECT_ACTION), context);
			}
		}

		/// <summary>
		/// Throws a <code>SecurityException</code> if the
		/// calling thread is not allowed to wait for a connection request on
		/// the specified local port number.
		/// <para>
		/// This method calls <code>checkPermission</code> with the
		/// <code>SocketPermission("localhost:"+port,"listen")</code>.
		/// </para>
		/// <para>
		/// If you override this method, then you should make a call to
		/// <code>super.checkListen</code>
		/// at the point the overridden method would normally throw an
		/// exception.
		/// 
		/// </para>
		/// </summary>
		/// <param name="port">   the local port. </param>
		/// <exception cref="SecurityException">  if the calling thread does not have
		///             permission to listen on the specified port. </exception>
		/// <seealso cref=        #checkPermission(java.security.Permission) checkPermission </seealso>
		public virtual void CheckListen(int port)
		{
			CheckPermission(new SocketPermission("localhost:" + port, SecurityConstants.SOCKET_LISTEN_ACTION));
		}

		/// <summary>
		/// Throws a <code>SecurityException</code> if the
		/// calling thread is not permitted to accept a socket connection from
		/// the specified host and port number.
		/// <para>
		/// This method is invoked for the current security manager by the
		/// <code>accept</code> method of class <code>ServerSocket</code>.
		/// </para>
		/// <para>
		/// This method calls <code>checkPermission</code> with the
		/// <code>SocketPermission(host+":"+port,"accept")</code> permission.
		/// </para>
		/// <para>
		/// If you override this method, then you should make a call to
		/// <code>super.checkAccept</code>
		/// at the point the overridden method would normally throw an
		/// exception.
		/// 
		/// </para>
		/// </summary>
		/// <param name="host">   the host name of the socket connection. </param>
		/// <param name="port">   the port number of the socket connection. </param>
		/// <exception cref="SecurityException">  if the calling thread does not have
		///             permission to accept the connection. </exception>
		/// <exception cref="NullPointerException"> if the <code>host</code> argument is
		///             <code>null</code>. </exception>
		/// <seealso cref=        java.net.ServerSocket#accept() </seealso>
		/// <seealso cref=        #checkPermission(java.security.Permission) checkPermission </seealso>
		public virtual void CheckAccept(String host, int port)
		{
			if (host == null)
			{
				throw new NullPointerException("host can't be null");
			}
			if (!host.StartsWith("[") && host.IndexOf(':') != -1)
			{
				host = "[" + host + "]";
			}
			CheckPermission(new SocketPermission(host + ":" + port, SecurityConstants.SOCKET_ACCEPT_ACTION));
		}

		/// <summary>
		/// Throws a <code>SecurityException</code> if the
		/// calling thread is not allowed to use
		/// (join/leave/send/receive) IP multicast.
		/// <para>
		/// This method calls <code>checkPermission</code> with the
		/// <code>java.net.SocketPermission(maddr.getHostAddress(),
		/// "accept,connect")</code> permission.
		/// </para>
		/// <para>
		/// If you override this method, then you should make a call to
		/// <code>super.checkMulticast</code>
		/// at the point the overridden method would normally throw an
		/// exception.
		/// 
		/// </para>
		/// </summary>
		/// <param name="maddr">  Internet group address to be used. </param>
		/// <exception cref="SecurityException">  if the calling thread is not allowed to
		///  use (join/leave/send/receive) IP multicast. </exception>
		/// <exception cref="NullPointerException"> if the address argument is
		///             <code>null</code>.
		/// @since      JDK1.1 </exception>
		/// <seealso cref=        #checkPermission(java.security.Permission) checkPermission </seealso>
		public virtual void CheckMulticast(InetAddress maddr)
		{
			String host = maddr.HostAddress;
			if (!host.StartsWith("[") && host.IndexOf(':') != -1)
			{
				host = "[" + host + "]";
			}
			CheckPermission(new SocketPermission(host, SecurityConstants.SOCKET_CONNECT_ACCEPT_ACTION));
		}

		/// <summary>
		/// Throws a <code>SecurityException</code> if the
		/// calling thread is not allowed to use
		/// (join/leave/send/receive) IP multicast.
		/// <para>
		/// This method calls <code>checkPermission</code> with the
		/// <code>java.net.SocketPermission(maddr.getHostAddress(),
		/// "accept,connect")</code> permission.
		/// </para>
		/// <para>
		/// If you override this method, then you should make a call to
		/// <code>super.checkMulticast</code>
		/// at the point the overridden method would normally throw an
		/// exception.
		/// 
		/// </para>
		/// </summary>
		/// <param name="maddr">  Internet group address to be used. </param>
		/// <param name="ttl">        value in use, if it is multicast send.
		/// Note: this particular implementation does not use the ttl
		/// parameter. </param>
		/// <exception cref="SecurityException">  if the calling thread is not allowed to
		///  use (join/leave/send/receive) IP multicast. </exception>
		/// <exception cref="NullPointerException"> if the address argument is
		///             <code>null</code>.
		/// @since      JDK1.1 </exception>
		/// @deprecated Use #checkPermission(java.security.Permission) instead 
		/// <seealso cref=        #checkPermission(java.security.Permission) checkPermission </seealso>
		[Obsolete("Use #checkPermission(java.security.Permission) instead")]
		public virtual void CheckMulticast(InetAddress maddr, sbyte ttl)
		{
			String host = maddr.HostAddress;
			if (!host.StartsWith("[") && host.IndexOf(':') != -1)
			{
				host = "[" + host + "]";
			}
			CheckPermission(new SocketPermission(host, SecurityConstants.SOCKET_CONNECT_ACCEPT_ACTION));
		}

		/// <summary>
		/// Throws a <code>SecurityException</code> if the
		/// calling thread is not allowed to access or modify the system
		/// properties.
		/// <para>
		/// This method is used by the <code>getProperties</code> and
		/// <code>setProperties</code> methods of class <code>System</code>.
		/// </para>
		/// <para>
		/// This method calls <code>checkPermission</code> with the
		/// <code>PropertyPermission("*", "read,write")</code> permission.
		/// </para>
		/// <para>
		/// If you override this method, then you should make a call to
		/// <code>super.checkPropertiesAccess</code>
		/// at the point the overridden method would normally throw an
		/// exception.
		/// </para>
		/// <para>
		/// 
		/// </para>
		/// </summary>
		/// <exception cref="SecurityException">  if the calling thread does not have
		///             permission to access or modify the system properties. </exception>
		/// <seealso cref=        java.lang.System#getProperties() </seealso>
		/// <seealso cref=        java.lang.System#setProperties(java.util.Properties) </seealso>
		/// <seealso cref=        #checkPermission(java.security.Permission) checkPermission </seealso>
		public virtual void CheckPropertiesAccess()
		{
			CheckPermission(new PropertyPermission("*", SecurityConstants.PROPERTY_RW_ACTION));
		}

		/// <summary>
		/// Throws a <code>SecurityException</code> if the
		/// calling thread is not allowed to access the system property with
		/// the specified <code>key</code> name.
		/// <para>
		/// This method is used by the <code>getProperty</code> method of
		/// class <code>System</code>.
		/// </para>
		/// <para>
		/// This method calls <code>checkPermission</code> with the
		/// <code>PropertyPermission(key, "read")</code> permission.
		/// </para>
		/// <para>
		/// If you override this method, then you should make a call to
		/// <code>super.checkPropertyAccess</code>
		/// at the point the overridden method would normally throw an
		/// exception.
		/// 
		/// </para>
		/// </summary>
		/// <param name="key">   a system property key.
		/// </param>
		/// <exception cref="SecurityException">  if the calling thread does not have
		///             permission to access the specified system property. </exception>
		/// <exception cref="NullPointerException"> if the <code>key</code> argument is
		///             <code>null</code>. </exception>
		/// <exception cref="IllegalArgumentException"> if <code>key</code> is empty.
		/// </exception>
		/// <seealso cref=        java.lang.System#getProperty(java.lang.String) </seealso>
		/// <seealso cref=        #checkPermission(java.security.Permission) checkPermission </seealso>
		public virtual void CheckPropertyAccess(String key)
		{
			CheckPermission(new PropertyPermission(key, SecurityConstants.PROPERTY_READ_ACTION));
		}

		/// <summary>
		/// Returns <code>false</code> if the calling
		/// thread is not trusted to bring up the top-level window indicated
		/// by the <code>window</code> argument. In this case, the caller can
		/// still decide to show the window, but the window should include
		/// some sort of visual warning. If the method returns
		/// <code>true</code>, then the window can be shown without any
		/// special restrictions.
		/// <para>
		/// See class <code>Window</code> for more information on trusted and
		/// untrusted windows.
		/// </para>
		/// <para>
		/// This method calls
		/// <code>checkPermission</code> with the
		/// <code>AWTPermission("showWindowWithoutWarningBanner")</code> permission,
		/// and returns <code>true</code> if a SecurityException is not thrown,
		/// otherwise it returns <code>false</code>.
		/// In the case of subset Profiles of Java SE that do not include the
		/// {@code java.awt} package, {@code checkPermission} is instead called
		/// to check the permission {@code java.security.AllPermission}.
		/// </para>
		/// <para>
		/// If you override this method, then you should make a call to
		/// <code>super.checkTopLevelWindow</code>
		/// at the point the overridden method would normally return
		/// <code>false</code>, and the value of
		/// <code>super.checkTopLevelWindow</code> should
		/// be returned.
		/// 
		/// </para>
		/// </summary>
		/// <param name="window">   the new window that is being created. </param>
		/// <returns>     <code>true</code> if the calling thread is trusted to put up
		///             top-level windows; <code>false</code> otherwise. </returns>
		/// <exception cref="NullPointerException"> if the <code>window</code> argument is
		///             <code>null</code>. </exception>
		/// @deprecated The dependency on {@code AWTPermission} creates an
		///             impediment to future modularization of the Java platform.
		///             Users of this method should instead invoke
		///             <seealso cref="#checkPermission"/> directly.
		///             This method will be changed in a future release to check
		///             the permission {@code java.security.AllPermission}. 
		/// <seealso cref=        java.awt.Window </seealso>
		/// <seealso cref=        #checkPermission(java.security.Permission) checkPermission </seealso>
		[Obsolete("The dependency on {@code AWTPermission} creates an")]
		public virtual bool CheckTopLevelWindow(Object window)
		{
			if (window == null)
			{
				throw new NullPointerException("window can't be null");
			}
			Permission perm = SecurityConstants.AWT.TOPLEVEL_WINDOW_PERMISSION;
			if (perm == null)
			{
				perm = SecurityConstants.ALL_PERMISSION;
			}
			try
			{
				CheckPermission(perm);
				return true;
			}
			catch (SecurityException)
			{
				// just return false
			}
			return false;
		}

		/// <summary>
		/// Throws a <code>SecurityException</code> if the
		/// calling thread is not allowed to initiate a print job request.
		/// <para>
		/// This method calls
		/// <code>checkPermission</code> with the
		/// <code>RuntimePermission("queuePrintJob")</code> permission.
		/// </para>
		/// <para>
		/// If you override this method, then you should make a call to
		/// <code>super.checkPrintJobAccess</code>
		/// at the point the overridden method would normally throw an
		/// exception.
		/// </para>
		/// <para>
		/// 
		/// </para>
		/// </summary>
		/// <exception cref="SecurityException">  if the calling thread does not have
		///             permission to initiate a print job request.
		/// @since   JDK1.1 </exception>
		/// <seealso cref=        #checkPermission(java.security.Permission) checkPermission </seealso>
		public virtual void CheckPrintJobAccess()
		{
			CheckPermission(new RuntimePermission("queuePrintJob"));
		}

		/// <summary>
		/// Throws a <code>SecurityException</code> if the
		/// calling thread is not allowed to access the system clipboard.
		/// <para>
		/// This method calls <code>checkPermission</code> with the
		/// <code>AWTPermission("accessClipboard")</code>
		/// permission.
		/// In the case of subset Profiles of Java SE that do not include the
		/// {@code java.awt} package, {@code checkPermission} is instead called
		/// to check the permission {@code java.security.AllPermission}.
		/// </para>
		/// <para>
		/// If you override this method, then you should make a call to
		/// <code>super.checkSystemClipboardAccess</code>
		/// at the point the overridden method would normally throw an
		/// exception.
		/// 
		/// @since   JDK1.1
		/// </para>
		/// </summary>
		/// <exception cref="SecurityException">  if the calling thread does not have
		///             permission to access the system clipboard. </exception>
		/// @deprecated The dependency on {@code AWTPermission} creates an
		///             impediment to future modularization of the Java platform.
		///             Users of this method should instead invoke
		///             <seealso cref="#checkPermission"/> directly.
		///             This method will be changed in a future release to check
		///             the permission {@code java.security.AllPermission}. 
		/// <seealso cref=        #checkPermission(java.security.Permission) checkPermission </seealso>
		[Obsolete("The dependency on {@code AWTPermission} creates an")]
		public virtual void CheckSystemClipboardAccess()
		{
			Permission perm = SecurityConstants.AWT.ACCESS_CLIPBOARD_PERMISSION;
			if (perm == null)
			{
				perm = SecurityConstants.ALL_PERMISSION;
			}
			CheckPermission(perm);
		}

		/// <summary>
		/// Throws a <code>SecurityException</code> if the
		/// calling thread is not allowed to access the AWT event queue.
		/// <para>
		/// This method calls <code>checkPermission</code> with the
		/// <code>AWTPermission("accessEventQueue")</code> permission.
		/// In the case of subset Profiles of Java SE that do not include the
		/// {@code java.awt} package, {@code checkPermission} is instead called
		/// to check the permission {@code java.security.AllPermission}.
		/// 
		/// </para>
		/// <para>
		/// If you override this method, then you should make a call to
		/// <code>super.checkAwtEventQueueAccess</code>
		/// at the point the overridden method would normally throw an
		/// exception.
		/// 
		/// @since   JDK1.1
		/// </para>
		/// </summary>
		/// <exception cref="SecurityException">  if the calling thread does not have
		///             permission to access the AWT event queue. </exception>
		/// @deprecated The dependency on {@code AWTPermission} creates an
		///             impediment to future modularization of the Java platform.
		///             Users of this method should instead invoke
		///             <seealso cref="#checkPermission"/> directly.
		///             This method will be changed in a future release to check
		///             the permission {@code java.security.AllPermission}. 
		/// <seealso cref=        #checkPermission(java.security.Permission) checkPermission </seealso>
		[Obsolete("The dependency on {@code AWTPermission} creates an")]
		public virtual void CheckAwtEventQueueAccess()
		{
			Permission perm = SecurityConstants.AWT.CHECK_AWT_EVENTQUEUE_PERMISSION;
			if (perm == null)
			{
				perm = SecurityConstants.ALL_PERMISSION;
			}
			CheckPermission(perm);
		}

		/*
		 * We have an initial invalid bit (initially false) for the class
		 * variables which tell if the cache is valid.  If the underlying
		 * java.security.Security property changes via setProperty(), the
		 * Security class uses reflection to change the variable and thus
		 * invalidate the cache.
		 *
		 * Locking is handled by synchronization to the
		 * packageAccessLock/packageDefinitionLock objects.  They are only
		 * used in this class.
		 *
		 * Note that cache invalidation as a result of the property change
		 * happens without using these locks, so there may be a delay between
		 * when a thread updates the property and when other threads updates
		 * the cache.
		 */
		private static bool PackageAccessValid = false;
		private static String[] PackageAccess;
		private static readonly Object PackageAccessLock = new Object();

		private static bool PackageDefinitionValid = false;
		private static String[] PackageDefinition;
		private static readonly Object PackageDefinitionLock = new Object();

		private static String[] GetPackages(String p)
		{
			String[] packages = null;
			if (p != null && !p.Equals(""))
			{
				java.util.StringTokenizer tok = new java.util.StringTokenizer(p, ",");
				int n = tok.CountTokens();
				if (n > 0)
				{
				packages = new String[n];
					int i = 0;
					while (tok.HasMoreElements())
					{
						String s = tok.NextToken().Trim();
					packages[i++] = s;
					}
				}
			}

			if (packages == null)
			{
			packages = new String[0];
			}
			return packages;
		}

		/// <summary>
		/// Throws a <code>SecurityException</code> if the
		/// calling thread is not allowed to access the package specified by
		/// the argument.
		/// <para>
		/// This method is used by the <code>loadClass</code> method of class
		/// loaders.
		/// </para>
		/// <para>
		/// This method first gets a list of
		/// restricted packages by obtaining a comma-separated list from
		/// a call to
		/// <code>java.security.Security.getProperty("package.access")</code>,
		/// and checks to see if <code>pkg</code> starts with or equals
		/// any of the restricted packages. If it does, then
		/// <code>checkPermission</code> gets called with the
		/// <code>RuntimePermission("accessClassInPackage."+pkg)</code>
		/// permission.
		/// </para>
		/// <para>
		/// If this method is overridden, then
		/// <code>super.checkPackageAccess</code> should be called
		/// as the first line in the overridden method.
		/// 
		/// </para>
		/// </summary>
		/// <param name="pkg">   the package name. </param>
		/// <exception cref="SecurityException">  if the calling thread does not have
		///             permission to access the specified package. </exception>
		/// <exception cref="NullPointerException"> if the package name argument is
		///             <code>null</code>. </exception>
		/// <seealso cref=        java.lang.ClassLoader#loadClass(java.lang.String, boolean)
		///  loadClass </seealso>
		/// <seealso cref=        java.security.Security#getProperty getProperty </seealso>
		/// <seealso cref=        #checkPermission(java.security.Permission) checkPermission </seealso>
		public virtual void CheckPackageAccess(String pkg)
		{
			if (pkg == null)
			{
				throw new NullPointerException("package name can't be null");
			}

			String[] pkgs;
			lock (PackageAccessLock)
			{
				/*
				 * Do we need to update our property array?
				 */
				if (!PackageAccessValid)
				{
					String tmpPropertyStr = AccessController.doPrivileged(new PrivilegedActionAnonymousInnerClassHelper(this)
					   );
				PackageAccess = GetPackages(tmpPropertyStr);
				PackageAccessValid = true;
				}

				// Using a snapshot of packageAccess -- don't care if static field
				// changes afterwards; array contents won't change.
				pkgs = PackageAccess;
			}

			/*
			 * Traverse the list of packages, check for any matches.
			 */
			for (int i = 0; i < pkgs.Length; i++)
			{
				if (pkg.StartsWith(pkgs[i]) || pkgs[i].Equals(pkg + "."))
				{
					CheckPermission(new RuntimePermission("accessClassInPackage." + pkg));
					break; // No need to continue; only need to check this once
				}
			}
		}

		private class PrivilegedActionAnonymousInnerClassHelper : PrivilegedAction<String>
		{
			private readonly SecurityManager OuterInstance;

			public PrivilegedActionAnonymousInnerClassHelper(SecurityManager outerInstance)
			{
				this.OuterInstance = outerInstance;
			}

			public virtual String Run()
			{
				return java.security.Security.GetProperty("package.access");
			}
		}

		/// <summary>
		/// Throws a <code>SecurityException</code> if the
		/// calling thread is not allowed to define classes in the package
		/// specified by the argument.
		/// <para>
		/// This method is used by the <code>loadClass</code> method of some
		/// class loaders.
		/// </para>
		/// <para>
		/// This method first gets a list of restricted packages by
		/// obtaining a comma-separated list from a call to
		/// <code>java.security.Security.getProperty("package.definition")</code>,
		/// and checks to see if <code>pkg</code> starts with or equals
		/// any of the restricted packages. If it does, then
		/// <code>checkPermission</code> gets called with the
		/// <code>RuntimePermission("defineClassInPackage."+pkg)</code>
		/// permission.
		/// </para>
		/// <para>
		/// If this method is overridden, then
		/// <code>super.checkPackageDefinition</code> should be called
		/// as the first line in the overridden method.
		/// 
		/// </para>
		/// </summary>
		/// <param name="pkg">   the package name. </param>
		/// <exception cref="SecurityException">  if the calling thread does not have
		///             permission to define classes in the specified package. </exception>
		/// <seealso cref=        java.lang.ClassLoader#loadClass(java.lang.String, boolean) </seealso>
		/// <seealso cref=        java.security.Security#getProperty getProperty </seealso>
		/// <seealso cref=        #checkPermission(java.security.Permission) checkPermission </seealso>
		public virtual void CheckPackageDefinition(String pkg)
		{
			if (pkg == null)
			{
				throw new NullPointerException("package name can't be null");
			}

			String[] pkgs;
			lock (PackageDefinitionLock)
			{
				/*
				 * Do we need to update our property array?
				 */
				if (!PackageDefinitionValid)
				{
					String tmpPropertyStr = AccessController.doPrivileged(new PrivilegedActionAnonymousInnerClassHelper2(this)
					   );
				PackageDefinition = GetPackages(tmpPropertyStr);
				PackageDefinitionValid = true;
				}
				// Using a snapshot of packageDefinition -- don't care if static
				// field changes afterwards; array contents won't change.
				pkgs = PackageDefinition;
			}

			/*
			 * Traverse the list of packages, check for any matches.
			 */
			for (int i = 0; i < pkgs.Length; i++)
			{
				if (pkg.StartsWith(pkgs[i]) || pkgs[i].Equals(pkg + "."))
				{
					CheckPermission(new RuntimePermission("defineClassInPackage." + pkg));
					break; // No need to continue; only need to check this once
				}
			}
		}

		private class PrivilegedActionAnonymousInnerClassHelper2 : PrivilegedAction<String>
		{
			private readonly SecurityManager OuterInstance;

			public PrivilegedActionAnonymousInnerClassHelper2(SecurityManager outerInstance)
			{
				this.OuterInstance = outerInstance;
			}

			public virtual String Run()
			{
				return java.security.Security.GetProperty("package.definition");
			}
		}

		/// <summary>
		/// Throws a <code>SecurityException</code> if the
		/// calling thread is not allowed to set the socket factory used by
		/// <code>ServerSocket</code> or <code>Socket</code>, or the stream
		/// handler factory used by <code>URL</code>.
		/// <para>
		/// This method calls <code>checkPermission</code> with the
		/// <code>RuntimePermission("setFactory")</code> permission.
		/// </para>
		/// <para>
		/// If you override this method, then you should make a call to
		/// <code>super.checkSetFactory</code>
		/// at the point the overridden method would normally throw an
		/// exception.
		/// </para>
		/// <para>
		/// 
		/// </para>
		/// </summary>
		/// <exception cref="SecurityException">  if the calling thread does not have
		///             permission to specify a socket factory or a stream
		///             handler factory.
		/// </exception>
		/// <seealso cref=        java.net.ServerSocket#setSocketFactory(java.net.SocketImplFactory) setSocketFactory </seealso>
		/// <seealso cref=        java.net.Socket#setSocketImplFactory(java.net.SocketImplFactory) setSocketImplFactory </seealso>
		/// <seealso cref=        java.net.URL#setURLStreamHandlerFactory(java.net.URLStreamHandlerFactory) setURLStreamHandlerFactory </seealso>
		/// <seealso cref=        #checkPermission(java.security.Permission) checkPermission </seealso>
		public virtual void CheckSetFactory()
		{
			CheckPermission(new RuntimePermission("setFactory"));
		}

		/// <summary>
		/// Throws a <code>SecurityException</code> if the
		/// calling thread is not allowed to access members.
		/// <para>
		/// The default policy is to allow access to PUBLIC members, as well
		/// as access to classes that have the same class loader as the caller.
		/// In all other cases, this method calls <code>checkPermission</code>
		/// with the <code>RuntimePermission("accessDeclaredMembers")
		/// </code> permission.
		/// </para>
		/// <para>
		/// If this method is overridden, then a call to
		/// <code>super.checkMemberAccess</code> cannot be made,
		/// as the default implementation of <code>checkMemberAccess</code>
		/// relies on the code being checked being at a stack depth of
		/// 4.
		/// 
		/// </para>
		/// </summary>
		/// <param name="clazz"> the class that reflection is to be performed on.
		/// </param>
		/// <param name="which"> type of access, PUBLIC or DECLARED.
		/// </param>
		/// <exception cref="SecurityException"> if the caller does not have
		///             permission to access members. </exception>
		/// <exception cref="NullPointerException"> if the <code>clazz</code> argument is
		///             <code>null</code>.
		/// </exception>
		/// @deprecated This method relies on the caller being at a stack depth
		///             of 4 which is error-prone and cannot be enforced by the runtime.
		///             Users of this method should instead invoke <seealso cref="#checkPermission"/>
		///             directly.  This method will be changed in a future release
		///             to check the permission {@code java.security.AllPermission}.
		/// 
		/// <seealso cref= java.lang.reflect.Member
		/// @since JDK1.1 </seealso>
		/// <seealso cref=        #checkPermission(java.security.Permission) checkPermission </seealso>
//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Deprecated("This method relies on the caller being at a stack depth") @CallerSensitive public void checkMemberAccess(Class clazz, int which)
		[Obsolete("This method relies on the caller being at a stack depth")]
		public virtual void CheckMemberAccess(Class clazz, int which)
		{
			if (clazz == null)
			{
				throw new NullPointerException("class can't be null");
			}
			if (which != Member_Fields.PUBLIC)
			{
				Class[] stack = ClassContext;
				/*
				 * stack depth of 4 should be the caller of one of the
				 * methods in java.lang.Class that invoke checkMember
				 * access. The stack should look like:
				 *
				 * someCaller                        [3]
				 * java.lang.Class.someReflectionAPI [2]
				 * java.lang.Class.checkMemberAccess [1]
				 * SecurityManager.checkMemberAccess [0]
				 *
				 */
				if ((stack.Length < 4) || (stack[3].ClassLoader != clazz.ClassLoader))
				{
					CheckPermission(SecurityConstants.CHECK_MEMBER_ACCESS_PERMISSION);
				}
			}
		}

		/// <summary>
		/// Determines whether the permission with the specified permission target
		/// name should be granted or denied.
		/// 
		/// <para> If the requested permission is allowed, this method returns
		/// quietly. If denied, a SecurityException is raised.
		/// 
		/// </para>
		/// <para> This method creates a <code>SecurityPermission</code> object for
		/// the given permission target name and calls <code>checkPermission</code>
		/// with it.
		/// 
		/// </para>
		/// <para> See the documentation for
		/// <code><seealso cref="java.security.SecurityPermission"/></code> for
		/// a list of possible permission target names.
		/// 
		/// </para>
		/// <para> If you override this method, then you should make a call to
		/// <code>super.checkSecurityAccess</code>
		/// at the point the overridden method would normally throw an
		/// exception.
		/// 
		/// </para>
		/// </summary>
		/// <param name="target"> the target name of the <code>SecurityPermission</code>.
		/// </param>
		/// <exception cref="SecurityException"> if the calling thread does not have
		/// permission for the requested access. </exception>
		/// <exception cref="NullPointerException"> if <code>target</code> is null. </exception>
		/// <exception cref="IllegalArgumentException"> if <code>target</code> is empty.
		/// 
		/// @since   JDK1.1 </exception>
		/// <seealso cref=        #checkPermission(java.security.Permission) checkPermission </seealso>
		public virtual void CheckSecurityAccess(String target)
		{
			CheckPermission(new SecurityPermission(target));
		}

//JAVA TO C# CONVERTER TODO TASK: Replace 'unknown' with the appropriate dll name:
		[DllImport("unknown")]
		private extern Class currentLoadedClass0();

		/// <summary>
		/// Returns the thread group into which to instantiate any new
		/// thread being created at the time this is being called.
		/// By default, it returns the thread group of the current
		/// thread. This should be overridden by a specific security
		/// manager to return the appropriate thread group.
		/// </summary>
		/// <returns>  ThreadGroup that new threads are instantiated into
		/// @since   JDK1.1 </returns>
		/// <seealso cref=     java.lang.ThreadGroup </seealso>
		public virtual ThreadGroup ThreadGroup
		{
			get
			{
				return Thread.CurrentThread.ThreadGroup;
			}
		}

	}

}
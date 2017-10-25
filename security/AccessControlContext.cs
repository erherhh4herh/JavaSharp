using System.Collections.Generic;

/*
 * Copyright (c) 1997, 2015, Oracle and/or its affiliates. All rights reserved.
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
	using SecurityConstants = sun.security.util.SecurityConstants;


	/// <summary>
	/// An AccessControlContext is used to make system resource access decisions
	/// based on the context it encapsulates.
	/// 
	/// <para>More specifically, it encapsulates a context and
	/// has a single method, {@code checkPermission},
	/// that is equivalent to the {@code checkPermission} method
	/// in the AccessController class, with one difference: The AccessControlContext
	/// {@code checkPermission} method makes access decisions based on the
	/// context it encapsulates,
	/// rather than that of the current execution thread.
	/// 
	/// </para>
	/// <para>Thus, the purpose of AccessControlContext is for those situations where
	/// a security check that should be made within a given context
	/// actually needs to be done from within a
	/// <i>different</i> context (for example, from within a worker thread).
	/// 
	/// </para>
	/// <para> An AccessControlContext is created by calling the
	/// {@code AccessController.getContext} method.
	/// The {@code getContext} method takes a "snapshot"
	/// of the current calling context, and places
	/// it in an AccessControlContext object, which it returns. A sample call is
	/// the following:
	/// 
	/// <pre>
	///   AccessControlContext acc = AccessController.getContext()
	/// </pre>
	/// 
	/// </para>
	/// <para>
	/// Code within a different context can subsequently call the
	/// {@code checkPermission} method on the
	/// previously-saved AccessControlContext object. A sample call is the
	/// following:
	/// 
	/// <pre>
	///   acc.checkPermission(permission)
	/// </pre>
	/// 
	/// </para>
	/// </summary>
	/// <seealso cref= AccessController
	/// 
	/// @author Roland Schemers </seealso>

	public sealed class AccessControlContext
	{

		private ProtectionDomain[] Context_Renamed;
		// isPrivileged and isAuthorized are referenced by the VM - do not remove
		// or change their names
		private bool IsPrivileged;
		private bool IsAuthorized = false;

		// Note: This field is directly used by the virtual machine
		// native codes. Don't touch it.
		private AccessControlContext PrivilegedContext;

		private DomainCombiner Combiner_Renamed = null;

		// limited privilege scope
		private Permission[] Permissions;
		private AccessControlContext Parent;
		private bool IsWrapped;

		// is constrained by limited privilege scope?
		private bool IsLimited;
		private ProtectionDomain[] LimitedContext;

		private static bool DebugInit = false;
		private static Debug Debug_Renamed = null;

		internal static Debug Debug
		{
			get
			{
				if (DebugInit)
				{
					return Debug_Renamed;
				}
				else
				{
					if (Policy.Set)
					{
						Debug_Renamed = Debug.getInstance("access");
						DebugInit = true;
					}
					return Debug_Renamed;
				}
			}
		}

		/// <summary>
		/// Create an AccessControlContext with the given array of ProtectionDomains.
		/// Context must not be null. Duplicate domains will be removed from the
		/// context.
		/// </summary>
		/// <param name="context"> the ProtectionDomains associated with this context.
		/// The non-duplicate domains are copied from the array. Subsequent
		/// changes to the array will not affect this AccessControlContext. </param>
		/// <exception cref="NullPointerException"> if {@code context} is {@code null} </exception>
		public AccessControlContext(ProtectionDomain[] context)
		{
			if (context.Length == 0)
			{
				this.Context_Renamed = null;
			}
			else if (context.Length == 1)
			{
				if (context[0] != null)
				{
					this.Context_Renamed = context.clone();
				}
				else
				{
					this.Context_Renamed = null;
				}
			}
			else
			{
				IList<ProtectionDomain> v = new List<ProtectionDomain>(context.Length);
				for (int i = 0; i < context.Length; i++)
				{
					if ((context[i] != null) && (!v.Contains(context[i])))
					{
						v.Add(context[i]);
					}
				}
				if (v.Count > 0)
				{
					this.Context_Renamed = new ProtectionDomain[v.Count];
					this.Context_Renamed = v.toArray(this.Context_Renamed);
				}
			}
		}

		/// <summary>
		/// Create a new {@code AccessControlContext} with the given
		/// {@code AccessControlContext} and {@code DomainCombiner}.
		/// This constructor associates the provided
		/// {@code DomainCombiner} with the provided
		/// {@code AccessControlContext}.
		/// 
		/// <para>
		/// 
		/// </para>
		/// </summary>
		/// <param name="acc"> the {@code AccessControlContext} associated
		///          with the provided {@code DomainCombiner}.
		/// </param>
		/// <param name="combiner"> the {@code DomainCombiner} to be associated
		///          with the provided {@code AccessControlContext}.
		/// </param>
		/// <exception cref="NullPointerException"> if the provided
		///          {@code context} is {@code null}.
		/// </exception>
		/// <exception cref="SecurityException"> if a security manager is installed and the
		///          caller does not have the "createAccessControlContext"
		///          <seealso cref="SecurityPermission"/>
		/// @since 1.3 </exception>
		public AccessControlContext(AccessControlContext acc, DomainCombiner combiner) : this(acc, combiner, false)
		{

		}

		/// <summary>
		/// package private to allow calls from ProtectionDomain without performing
		/// the security check for <seealso cref="SecurityConstants.CREATE_ACC_PERMISSION"/>
		/// permission
		/// </summary>
		internal AccessControlContext(AccessControlContext acc, DomainCombiner combiner, bool preauthorized)
		{
			if (!preauthorized)
			{
				SecurityManager sm = System.SecurityManager;
				if (sm != null)
				{
					sm.CheckPermission(SecurityConstants.CREATE_ACC_PERMISSION);
					this.IsAuthorized = true;
				}
			}
			else
			{
				this.IsAuthorized = true;
			}

			this.Context_Renamed = acc.Context_Renamed;

			// we do not need to run the combine method on the
			// provided ACC.  it was already "combined" when the
			// context was originally retrieved.
			//
			// at this point in time, we simply throw away the old
			// combiner and use the newly provided one.
			this.Combiner_Renamed = combiner;
		}

		/// <summary>
		/// package private for AccessController
		/// 
		/// This "argument wrapper" context will be passed as the actual context
		/// parameter on an internal doPrivileged() call used in the implementation.
		/// </summary>
		internal AccessControlContext(ProtectionDomain caller, DomainCombiner combiner, AccessControlContext parent, AccessControlContext context, Permission[] perms)
		{
			/*
			 * Combine the domains from the doPrivileged() context into our
			 * wrapper context, if necessary.
			 */
			ProtectionDomain[] callerPDs = null;
			if (caller != null)
			{
				 callerPDs = new ProtectionDomain[] {caller};
			}
			if (context != null)
			{
				if (combiner != null)
				{
					this.Context_Renamed = combiner.Combine(callerPDs, context.Context_Renamed);
				}
				else
				{
					this.Context_Renamed = Combine(callerPDs, context.Context_Renamed);
				}
			}
			else
			{
				/*
				 * Call combiner even if there is seemingly nothing to combine.
				 */
				if (combiner != null)
				{
					this.Context_Renamed = combiner.Combine(callerPDs, null);
				}
				else
				{
					this.Context_Renamed = Combine(callerPDs, null);
				}
			}
			this.Combiner_Renamed = combiner;

			Permission[] tmp = null;
			if (perms != null)
			{
				tmp = new Permission[perms.Length];
				for (int i = 0; i < perms.Length; i++)
				{
					if (perms[i] == null)
					{
						throw new NullPointerException("permission can't be null");
					}

					/*
					 * An AllPermission argument is equivalent to calling
					 * doPrivileged() without any limit permissions.
					 */
					if (perms[i].GetType() == typeof(AllPermission))
					{
						parent = null;
					}
					tmp[i] = perms[i];
				}
			}

			/*
			 * For a doPrivileged() with limited privilege scope, initialize
			 * the relevant fields.
			 *
			 * The limitedContext field contains the union of all domains which
			 * are enclosed by this limited privilege scope. In other words,
			 * it contains all of the domains which could potentially be checked
			 * if none of the limiting permissions implied a requested permission.
			 */
			if (parent != null)
			{
				this.LimitedContext = Combine(parent.Context_Renamed, parent.LimitedContext);
				this.IsLimited = true;
				this.IsWrapped = true;
				this.Permissions = tmp;
				this.Parent = parent;
				this.PrivilegedContext = context; // used in checkPermission2()
			}
			this.IsAuthorized = true;
		}


		/// <summary>
		/// package private constructor for AccessController.getContext()
		/// </summary>

		internal AccessControlContext(ProtectionDomain[] context, bool isPrivileged)
		{
			this.Context_Renamed = context;
			this.IsPrivileged = isPrivileged;
			this.IsAuthorized = true;
		}

		/// <summary>
		/// Constructor for JavaSecurityAccess.doIntersectionPrivilege()
		/// </summary>
		internal AccessControlContext(ProtectionDomain[] context, AccessControlContext privilegedContext)
		{
			this.Context_Renamed = context;
			this.PrivilegedContext = privilegedContext;
			this.IsPrivileged = true;
		}

		/// <summary>
		/// Returns this context's context.
		/// </summary>
		internal ProtectionDomain[] Context
		{
			get
			{
				return Context_Renamed;
			}
		}

		/// <summary>
		/// Returns true if this context is privileged.
		/// </summary>
		internal bool Privileged
		{
			get
			{
				return IsPrivileged;
			}
		}

		/// <summary>
		/// get the assigned combiner from the privileged or inherited context
		/// </summary>
		internal DomainCombiner AssignedCombiner
		{
			get
			{
				AccessControlContext acc;
				if (IsPrivileged)
				{
					acc = PrivilegedContext;
				}
				else
				{
					acc = AccessController.InheritedAccessControlContext;
				}
				if (acc != null)
				{
					return acc.Combiner_Renamed;
				}
				return null;
			}
		}

		/// <summary>
		/// Get the {@code DomainCombiner} associated with this
		/// {@code AccessControlContext}.
		/// 
		/// <para>
		/// 
		/// </para>
		/// </summary>
		/// <returns> the {@code DomainCombiner} associated with this
		///          {@code AccessControlContext}, or {@code null}
		///          if there is none.
		/// </returns>
		/// <exception cref="SecurityException"> if a security manager is installed and
		///          the caller does not have the "getDomainCombiner"
		///          <seealso cref="SecurityPermission"/>
		/// @since 1.3 </exception>
		public DomainCombiner DomainCombiner
		{
			get
			{
    
				SecurityManager sm = System.SecurityManager;
				if (sm != null)
				{
					sm.CheckPermission(SecurityConstants.GET_COMBINER_PERMISSION);
				}
				return Combiner;
			}
		}

		/// <summary>
		/// package private for AccessController
		/// </summary>
		internal DomainCombiner Combiner
		{
			get
			{
				return Combiner_Renamed;
			}
		}

		internal bool Authorized
		{
			get
			{
				return IsAuthorized;
			}
		}

		/// <summary>
		/// Determines whether the access request indicated by the
		/// specified permission should be allowed or denied, based on
		/// the security policy currently in effect, and the context in
		/// this object. The request is allowed only if every ProtectionDomain
		/// in the context implies the permission. Otherwise the request is
		/// denied.
		/// 
		/// <para>
		/// This method quietly returns if the access request
		/// is permitted, or throws a suitable AccessControlException otherwise.
		/// 
		/// </para>
		/// </summary>
		/// <param name="perm"> the requested permission.
		/// </param>
		/// <exception cref="AccessControlException"> if the specified permission
		/// is not permitted, based on the current security policy and the
		/// context encapsulated by this object. </exception>
		/// <exception cref="NullPointerException"> if the permission to check for is null. </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public void checkPermission(Permission perm) throws AccessControlException
		public void CheckPermission(Permission perm)
		{
			bool dumpDebug = false;

			if (perm == null)
			{
				throw new NullPointerException("permission can't be null");
			}
			if (Debug != null)
			{
				// If "codebase" is not specified, we dump the info by default.
				dumpDebug = !Debug.isOn("codebase=");
				if (!dumpDebug)
				{
					// If "codebase" is specified, only dump if the specified code
					// value is in the stack.
					for (int i = 0; Context_Renamed != null && i < Context_Renamed.Length; i++)
					{
						if (Context_Renamed[i].CodeSource != null && Context_Renamed[i].CodeSource.Location != null && Debug.isOn("codebase=" + Context_Renamed[i].CodeSource.Location.ToString()))
						{
							dumpDebug = true;
							break;
						}
					}
				}

//JAVA TO C# CONVERTER WARNING: The .NET Type.FullName property will not always yield results identical to the Java Class.getCanonicalName method:
				dumpDebug &= !Debug.isOn("permission=") || Debug.isOn("permission=" + perm.GetType().FullName);

				if (dumpDebug && Debug.isOn("stack"))
				{
					Thread.DumpStack();
				}

				if (dumpDebug && Debug.isOn("domain"))
				{
					if (Context_Renamed == null)
					{
						Debug_Renamed.println("domain (context is null)");
					}
					else
					{
						for (int i = 0; i < Context_Renamed.Length; i++)
						{
							Debug_Renamed.println("domain " + i + " " + Context_Renamed[i]);
						}
					}
				}
			}

			/*
			 * iterate through the ProtectionDomains in the context.
			 * Stop at the first one that doesn't allow the
			 * requested permission (throwing an exception).
			 *
			 */

			/* if ctxt is null, all we had on the stack were system domains,
			   or the first domain was a Privileged system domain. This
			   is to make the common case for system code very fast */

			if (Context_Renamed == null)
			{
				CheckPermission2(perm);
				return;
			}

			for (int i = 0; i < Context_Renamed.Length; i++)
			{
				if (Context_Renamed[i] != null && !Context_Renamed[i].Implies(perm))
				{
					if (dumpDebug)
					{
						Debug_Renamed.println("access denied " + perm);
					}

					if (Debug.isOn("failure") && Debug_Renamed != null)
					{
						// Want to make sure this is always displayed for failure,
						// but do not want to display again if already displayed
						// above.
						if (!dumpDebug)
						{
							Debug_Renamed.println("access denied " + perm);
						}
						Thread.DumpStack();
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final ProtectionDomain pd = context[i];
						ProtectionDomain pd = Context_Renamed[i];
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final sun.security.util.Debug db = debug;
						Debug db = Debug_Renamed;
						AccessController.doPrivileged(new PrivilegedActionAnonymousInnerClassHelper(this, pd, db));
					}
					throw new AccessControlException("access denied " + perm, perm);
				}
			}

			// allow if all of them allowed access
			if (dumpDebug)
			{
				Debug_Renamed.println("access allowed " + perm);
			}

			CheckPermission2(perm);
		}

		private class PrivilegedActionAnonymousInnerClassHelper : PrivilegedAction<Void>
		{
			private readonly AccessControlContext OuterInstance;

			private java.security.ProtectionDomain Pd;
			private Debug Db;

			public PrivilegedActionAnonymousInnerClassHelper(AccessControlContext outerInstance, java.security.ProtectionDomain pd, Debug db)
			{
				this.OuterInstance = outerInstance;
				this.Pd = pd;
				this.Db = db;
			}

			public virtual Void Run()
			{
				Db.println("domain that failed " + Pd);
				return null;
			}
		}

		/*
		 * Check the domains associated with the limited privilege scope.
		 */
		private void CheckPermission2(Permission perm)
		{
			if (!IsLimited)
			{
				return;
			}

			/*
			 * Check the doPrivileged() context parameter, if present.
			 */
			if (PrivilegedContext != null)
			{
				PrivilegedContext.CheckPermission2(perm);
			}

			/*
			 * Ignore the limited permissions and parent fields of a wrapper
			 * context since they were already carried down into the unwrapped
			 * context.
			 */
			if (IsWrapped)
			{
				return;
			}

			/*
			 * Try to match any limited privilege scope.
			 */
			if (Permissions != null)
			{
				Class permClass = perm.GetType();
				for (int i = 0; i < Permissions.Length; i++)
				{
					Permission limit = Permissions[i];
					if (limit.GetType().Equals(permClass) && limit.Implies(perm))
					{
						return;
					}
				}
			}

			/*
			 * Check the limited privilege scope up the call stack or the inherited
			 * parent thread call stack of this ACC.
			 */
			if (Parent != null)
			{
				/*
				 * As an optimization, if the parent context is the inherited call
				 * stack context from a parent thread then checking the protection
				 * domains of the parent context is redundant since they have
				 * already been merged into the child thread's context by
				 * optimize(). When parent is set to an inherited context this
				 * context was not directly created by a limited scope
				 * doPrivileged() and it does not have its own limited permissions.
				 */
				if (Permissions == null)
				{
					Parent.CheckPermission2(perm);
				}
				else
				{
					Parent.CheckPermission(perm);
				}
			}
		}

		/// <summary>
		/// Take the stack-based context (this) and combine it with the
		/// privileged or inherited context, if need be. Any limited
		/// privilege scope is flagged regardless of whether the assigned
		/// context comes from an immediately enclosing limited doPrivileged().
		/// The limited privilege scope can indirectly flow from the inherited
		/// parent thread or an assigned context previously captured by getContext().
		/// </summary>
		internal AccessControlContext Optimize()
		{
			// the assigned (privileged or inherited) context
			AccessControlContext acc;
			DomainCombiner combiner = null;
			AccessControlContext parent = null;
			Permission[] permissions = null;

			if (IsPrivileged)
			{
				acc = PrivilegedContext;
				if (acc != null)
				{
					/*
					 * If the context is from a limited scope doPrivileged() then
					 * copy the permissions and parent fields out of the wrapper
					 * context that was created to hold them.
					 */
					if (acc.IsWrapped)
					{
						permissions = acc.Permissions;
						parent = acc.Parent;
					}
				}
			}
			else
			{
				acc = AccessController.InheritedAccessControlContext;
				if (acc != null)
				{
					/*
					 * If the inherited context is constrained by a limited scope
					 * doPrivileged() then set it as our parent so we will process
					 * the non-domain-related state.
					 */
					if (acc.IsLimited)
					{
						parent = acc;
					}
				}
			}

			// this.context could be null if only system code is on the stack;
			// in that case, ignore the stack context
			bool skipStack = (Context_Renamed == null);

			// acc.context could be null if only system code was involved;
			// in that case, ignore the assigned context
			bool skipAssigned = (acc == null || acc.Context_Renamed == null);
			ProtectionDomain[] assigned = (skipAssigned) ? null : acc.Context_Renamed;
			ProtectionDomain[] pd;

			// if there is no enclosing limited privilege scope on the stack or
			// inherited from a parent thread
			bool skipLimited = ((acc == null || !acc.IsWrapped) && parent == null);

			if (acc != null && acc.Combiner_Renamed != null)
			{
				// let the assigned acc's combiner do its thing
				if (Debug != null)
				{
					Debug_Renamed.println("AccessControlContext invoking the Combiner");
				}

				// No need to clone current and assigned.context
				// combine() will not update them
				combiner = acc.Combiner_Renamed;
				pd = combiner.Combine(Context_Renamed, assigned);
			}
			else
			{
				if (skipStack)
				{
					if (skipAssigned)
					{
						CalculateFields(acc, parent, permissions);
						return this;
					}
					else if (skipLimited)
					{
						return acc;
					}
				}
				else if (assigned != null)
				{
					if (skipLimited)
					{
						// optimization: if there is a single stack domain and
						// that domain is already in the assigned context; no
						// need to combine
						if (Context_Renamed.Length == 1 && Context_Renamed[0] == assigned[0])
						{
							return acc;
						}
					}
				}

				pd = Combine(Context_Renamed, assigned);
				if (skipLimited && !skipAssigned && pd == assigned)
				{
					return acc;
				}
				else if (skipAssigned && pd == Context_Renamed)
				{
					CalculateFields(acc, parent, permissions);
					return this;
				}
			}

			// Reuse existing ACC
			this.Context_Renamed = pd;
			this.Combiner_Renamed = combiner;
			this.IsPrivileged = false;

			CalculateFields(acc, parent, permissions);
			return this;
		}


		/*
		 * Combine the current (stack) and assigned domains.
		 */
		private static ProtectionDomain[] Combine(ProtectionDomain[] current, ProtectionDomain[] assigned)
		{

			// current could be null if only system code is on the stack;
			// in that case, ignore the stack context
			bool skipStack = (current == null);

			// assigned could be null if only system code was involved;
			// in that case, ignore the assigned context
			bool skipAssigned = (assigned == null);

			int slen = (skipStack) ? 0 : current.Length;

			// optimization: if there is no assigned context and the stack length
			// is less then or equal to two; there is no reason to compress the
			// stack context, it already is
			if (skipAssigned && slen <= 2)
			{
				return current;
			}

			int n = (skipAssigned) ? 0 : assigned.Length;

			// now we combine both of them, and create a new context
			ProtectionDomain[] pd = new ProtectionDomain[slen + n];

			// first copy in the assigned context domains, no need to compress
			if (!skipAssigned)
			{
				System.Array.Copy(assigned, 0, pd, 0, n);
			}

			// now add the stack context domains, discarding nulls and duplicates
			for (int i = 0; i < slen; i++)
			{
				ProtectionDomain sd = current[i];
				if (sd != null)
				{
					for (int j = 0; j < n; j++)
					{
						if (sd == pd[j])
						{
							goto outerContinue;
						}
					}
					pd[n++] = sd;
				}
			outerContinue:;
			}
		outerBreak:

			// if length isn't equal, we need to shorten the array
			if (n != pd.Length)
			{
				// optimization: if we didn't really combine anything
				if (!skipAssigned && n == assigned.Length)
				{
					return assigned;
				}
				else if (skipAssigned && n == slen)
				{
					return current;
				}
				ProtectionDomain[] tmp = new ProtectionDomain[n];
				System.Array.Copy(pd, 0, tmp, 0, n);
				pd = tmp;
			}

			return pd;
		}


		/*
		 * Calculate the additional domains that could potentially be reached via
		 * limited privilege scope. Mark the context as being subject to limited
		 * privilege scope unless the reachable domains (if any) are already
		 * contained in this domain context (in which case any limited
		 * privilege scope checking would be redundant).
		 */
		private void CalculateFields(AccessControlContext assigned, AccessControlContext parent, Permission[] permissions)
		{
			ProtectionDomain[] parentLimit = null;
			ProtectionDomain[] assignedLimit = null;
			ProtectionDomain[] newLimit;

			parentLimit = (parent != null)? parent.LimitedContext: null;
			assignedLimit = (assigned != null)? assigned.LimitedContext: null;
			newLimit = Combine(parentLimit, assignedLimit);
			if (newLimit != null)
			{
				if (Context_Renamed == null || !ContainsAllPDs(newLimit, Context_Renamed))
				{
					this.LimitedContext = newLimit;
					this.Permissions = permissions;
					this.Parent = parent;
					this.IsLimited = true;
				}
			}
		}


		/// <summary>
		/// Checks two AccessControlContext objects for equality.
		/// Checks that <i>obj</i> is
		/// an AccessControlContext and has the same set of ProtectionDomains
		/// as this context.
		/// <P> </summary>
		/// <param name="obj"> the object we are testing for equality with this object. </param>
		/// <returns> true if <i>obj</i> is an AccessControlContext, and has the
		/// same set of ProtectionDomains as this context, false otherwise. </returns>
		public override bool Equals(Object obj)
		{
			if (obj == this)
			{
				return true;
			}

			if (!(obj is AccessControlContext))
			{
				return false;
			}

			AccessControlContext that = (AccessControlContext) obj;

			if (!EqualContext(that))
			{
				return false;
			}

			if (!EqualLimitedContext(that))
			{
				return false;
			}

			return true;
		}

		/*
		 * Compare for equality based on state that is free of limited
		 * privilege complications.
		 */
		private bool EqualContext(AccessControlContext that)
		{
			if (!EqualPDs(this.Context_Renamed, that.Context_Renamed))
			{
				return false;
			}

			if (this.Combiner_Renamed == null && that.Combiner_Renamed != null)
			{
				return false;
			}

			if (this.Combiner_Renamed != null && !this.Combiner_Renamed.Equals(that.Combiner_Renamed))
			{
				return false;
			}

			return true;
		}

		private bool EqualPDs(ProtectionDomain[] a, ProtectionDomain[] b)
		{
			if (a == null)
			{
				return (b == null);
			}

			if (b == null)
			{
				return false;
			}

			if (!(ContainsAllPDs(a, b) && ContainsAllPDs(b, a)))
			{
				return false;
			}

			return true;
		}

		/*
		 * Compare for equality based on state that is captured during a
		 * call to AccessController.getContext() when a limited privilege
		 * scope is in effect.
		 */
		private bool EqualLimitedContext(AccessControlContext that)
		{
			if (that == null)
			{
				return false;
			}

			/*
			 * If neither instance has limited privilege scope then we're done.
			 */
			if (!this.IsLimited && !that.IsLimited)
			{
				return true;
			}

			/*
			 * If only one instance has limited privilege scope then we're done.
			 */
			 if (!(this.IsLimited && that.IsLimited))
			 {
				 return false;
			 }

			/*
			 * Wrapped instances should never escape outside the implementation
			 * this class and AccessController so this will probably never happen
			 * but it only makes any sense to compare if they both have the same
			 * isWrapped state.
			 */
			if ((this.IsWrapped && !that.IsWrapped) || (!this.IsWrapped && that.IsWrapped))
			{
				return false;
			}

			if (this.Permissions == null && that.Permissions != null)
			{
				return false;
			}

			if (this.Permissions != null && that.Permissions == null)
			{
				return false;
			}

			if (!(this.ContainsAllLimits(that) && that.ContainsAllLimits(this)))
			{
				return false;
			}

			/*
			 * Skip through any wrapped contexts.
			 */
			AccessControlContext thisNextPC = GetNextPC(this);
			AccessControlContext thatNextPC = GetNextPC(that);

			/*
			 * The protection domains and combiner of a privilegedContext are
			 * not relevant because they have already been included in the context
			 * of this instance by optimize() so we only care about any limited
			 * privilege state they may have.
			 */
			if (thisNextPC == null && thatNextPC != null && thatNextPC.IsLimited)
			{
				return false;
			}

			if (thisNextPC != null && !thisNextPC.EqualLimitedContext(thatNextPC))
			{
				return false;
			}

			if (this.Parent == null && that.Parent != null)
			{
				return false;
			}

			if (this.Parent != null && !this.Parent.Equals(that.Parent))
			{
				return false;
			}

			return true;
		}

		/*
		 * Follow the privilegedContext link making our best effort to skip
		 * through any wrapper contexts.
		 */
		private static AccessControlContext GetNextPC(AccessControlContext acc)
		{
			while (acc != null && acc.PrivilegedContext != null)
			{
				acc = acc.PrivilegedContext;
				if (!acc.IsWrapped)
				{
					return acc;
				}
			}
			return null;
		}

		private static bool ContainsAllPDs(ProtectionDomain[] thisContext, ProtectionDomain[] thatContext)
		{
			bool match = false;

			//
			// ProtectionDomains within an ACC currently cannot be null
			// and this is enforced by the constructor and the various
			// optimize methods. However, historically this logic made attempts
			// to support the notion of a null PD and therefore this logic continues
			// to support that notion.
			ProtectionDomain thisPd;
			for (int i = 0; i < thisContext.Length; i++)
			{
				match = false;
				if ((thisPd = thisContext[i]) == null)
				{
					for (int j = 0; (j < thatContext.Length) && !match; j++)
					{
						match = (thatContext[j] == null);
					}
				}
				else
				{
					Class thisPdClass = thisPd.GetType();
					ProtectionDomain thatPd;
					for (int j = 0; (j < thatContext.Length) && !match; j++)
					{
						thatPd = thatContext[j];

						// Class check required to avoid PD exposure (4285406)
						match = (thatPd != null && thisPdClass == thatPd.GetType() && thisPd.Equals(thatPd));
					}
				}
				if (!match)
				{
					return false;
				}
			}
			return match;
		}

		private bool ContainsAllLimits(AccessControlContext that)
		{
			bool match = false;
			Permission thisPerm;

			if (this.Permissions == null && that.Permissions == null)
			{
				return true;
			}

			for (int i = 0; i < this.Permissions.Length; i++)
			{
				Permission limit = this.Permissions[i];
				Class  limitClass = limit.GetType();
				match = false;
				for (int j = 0; (j < that.Permissions.Length) && !match; j++)
				{
					Permission perm = that.Permissions[j];
					match = (limitClass.Equals(perm.GetType()) && limit.Equals(perm));
				}
				if (!match)
				{
					return false;
				}
			}
			return match;
		}


		/// <summary>
		/// Returns the hash code value for this context. The hash code
		/// is computed by exclusive or-ing the hash code of all the protection
		/// domains in the context together.
		/// </summary>
		/// <returns> a hash code value for this context. </returns>

		public override int HashCode()
		{
			int hashCode = 0;

			if (Context_Renamed == null)
			{
				return hashCode;
			}

			for (int i = 0; i < Context_Renamed.Length; i++)
			{
				if (Context_Renamed[i] != null)
				{
					hashCode ^= Context_Renamed[i].HashCode();
				}
			}

			return hashCode;
		}
	}

}
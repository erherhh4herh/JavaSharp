using System;
using System.Collections.Generic;

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
namespace java.awt
{

	using PlatformLogger = sun.util.logging.PlatformLogger;

	/// <summary>
	/// A FocusTraversalPolicy that determines traversal order based on the order
	/// of child Components in a Container. From a particular focus cycle root, the
	/// policy makes a pre-order traversal of the Component hierarchy, and traverses
	/// a Container's children according to the ordering of the array returned by
	/// <code>Container.getComponents()</code>. Portions of the hierarchy that are
	/// not visible and displayable will not be searched.
	/// <para>
	/// By default, ContainerOrderFocusTraversalPolicy implicitly transfers focus
	/// down-cycle. That is, during normal forward focus traversal, the Component
	/// traversed after a focus cycle root will be the focus-cycle-root's default
	/// Component to focus. This behavior can be disabled using the
	/// <code>setImplicitDownCycleTraversal</code> method.
	/// </para>
	/// <para>
	/// By default, methods of this class will return a Component only if it is
	/// visible, displayable, enabled, and focusable. Subclasses can modify this
	/// behavior by overriding the <code>accept</code> method.
	/// </para>
	/// <para>
	/// This policy takes into account <a
	/// href="doc-files/FocusSpec.html#FocusTraversalPolicyProviders">focus traversal
	/// policy providers</a>.  When searching for first/last/next/previous Component,
	/// if a focus traversal policy provider is encountered, its focus traversal
	/// policy is used to perform the search operation.
	/// 
	/// @author David Mendenhall
	/// 
	/// </para>
	/// </summary>
	/// <seealso cref= Container#getComponents
	/// @since 1.4 </seealso>
	[Serializable]
	public class ContainerOrderFocusTraversalPolicy : FocusTraversalPolicy
	{
		private static readonly PlatformLogger Log = PlatformLogger.getLogger("java.awt.ContainerOrderFocusTraversalPolicy");

		private readonly int FORWARD_TRAVERSAL = 0;
		private readonly int BACKWARD_TRAVERSAL = 1;

		/*
		 * JDK 1.4 serialVersionUID
		 */
		private const long SerialVersionUID = 486933713763926351L;

		private bool ImplicitDownCycleTraversal_Renamed = true;

		/// <summary>
		/// Used by getComponentAfter and getComponentBefore for efficiency. In
		/// order to maintain compliance with the specification of
		/// FocusTraversalPolicy, if traversal wraps, we should invoke
		/// getFirstComponent or getLastComponent. These methods may be overriden in
		/// subclasses to behave in a non-generic way. However, in the generic case,
		/// these methods will simply return the first or last Components of the
		/// sorted list, respectively. Since getComponentAfter and
		/// getComponentBefore have already built the list before determining
		/// that they need to invoke getFirstComponent or getLastComponent, the
		/// list should be reused if possible.
		/// </summary>
		[NonSerialized]
		private Container CachedRoot;
		[NonSerialized]
		private IList<Component> CachedCycle;

		/*
		 * We suppose to use getFocusTraversalCycle & getComponentIndex methods in order
		 * to divide the policy into two parts:
		 * 1) Making the focus traversal cycle.
		 * 2) Traversing the cycle.
		 * The 1st point assumes producing a list of components representing the focus
		 * traversal cycle. The two methods mentioned above should implement this logic.
		 * The 2nd point assumes implementing the common concepts of operating on the
		 * cycle: traversing back and forth, retrieving the initial/default/first/last
		 * component. These concepts are described in the AWT Focus Spec and they are
		 * applied to the FocusTraversalPolicy in general.
		 * Thus, a descendant of this policy may wish to not reimplement the logic of
		 * the 2nd point but just override the implementation of the 1st one.
		 * A striking example of such a descendant is the javax.swing.SortingFocusTraversalPolicy.
		 */
		/*protected*/	 private IList<Component> GetFocusTraversalCycle(Container aContainer)
	 {
			IList<Component> cycle = new List<Component>();
			EnumerateCycle(aContainer, cycle);
			return cycle;
	 }
		/*protected*/	 private int GetComponentIndex(IList<Component> cycle, Component aComponent)
	 {
			return cycle.IndexOf(aComponent);
	 }

		private void EnumerateCycle(Container container, IList<Component> cycle)
		{
			if (!(container.Visible && container.Displayable))
			{
				return;
			}

			cycle.Add(container);

			Component[] components = container.Components;
			for (int i = 0; i < components.Length; i++)
			{
				Component comp = components[i];
				if (comp is Container)
				{
					Container cont = (Container)comp;

					if (!cont.FocusCycleRoot && !cont.FocusTraversalPolicyProvider)
					{
						EnumerateCycle(cont, cycle);
						continue;
					}
				}
				cycle.Add(comp);
			}
		}

		private Container GetTopmostProvider(Container focusCycleRoot, Component aComponent)
		{
			Container aCont = aComponent.Parent;
			Container ftp = null;
			while (aCont != focusCycleRoot && aCont != null)
			{
				if (aCont.FocusTraversalPolicyProvider)
				{
					ftp = aCont;
				}
				aCont = aCont.Parent;
			}
			if (aCont == null)
			{
				return null;
			}
			return ftp;
		}

		/*
		 * Checks if a new focus cycle takes place and returns a Component to traverse focus to.
		 * @param comp a possible focus cycle root or policy provider
		 * @param traversalDirection the direction of the traversal
		 * @return a Component to traverse focus to if {@code comp} is a root or provider
		 *         and implicit down-cycle is set, otherwise {@code null}
		 */
		private Component GetComponentDownCycle(Component comp, int traversalDirection)
		{
			Component retComp = null;

			if (comp is Container)
			{
				Container cont = (Container)comp;

				if (cont.FocusCycleRoot)
				{
					if (ImplicitDownCycleTraversal)
					{
						retComp = cont.FocusTraversalPolicy.GetDefaultComponent(cont);

						if (retComp != null && Log.isLoggable(PlatformLogger.Level.FINE))
						{
							Log.fine("### Transfered focus down-cycle to " + retComp + " in the focus cycle root " + cont);
						}
					}
					else
					{
						return null;
					}
				}
				else if (cont.FocusTraversalPolicyProvider)
				{
					retComp = (traversalDirection == FORWARD_TRAVERSAL ? cont.FocusTraversalPolicy.GetDefaultComponent(cont) : cont.FocusTraversalPolicy.GetLastComponent(cont));

					if (retComp != null && Log.isLoggable(PlatformLogger.Level.FINE))
					{
						Log.fine("### Transfered focus to " + retComp + " in the FTP provider " + cont);
					}
				}
			}
			return retComp;
		}

		/// <summary>
		/// Returns the Component that should receive the focus after aComponent.
		/// aContainer must be a focus cycle root of aComponent or a focus traversal policy provider.
		/// <para>
		/// By default, ContainerOrderFocusTraversalPolicy implicitly transfers
		/// focus down-cycle. That is, during normal forward focus traversal, the
		/// Component traversed after a focus cycle root will be the focus-cycle-
		/// root's default Component to focus. This behavior can be disabled using
		/// the <code>setImplicitDownCycleTraversal</code> method.
		/// </para>
		/// <para>
		/// If aContainer is <a href="doc-files/FocusSpec.html#FocusTraversalPolicyProviders">focus
		/// traversal policy provider</a>, the focus is always transferred down-cycle.
		/// 
		/// </para>
		/// </summary>
		/// <param name="aContainer"> a focus cycle root of aComponent or a focus traversal policy provider </param>
		/// <param name="aComponent"> a (possibly indirect) child of aContainer, or
		///        aContainer itself </param>
		/// <returns> the Component that should receive the focus after aComponent, or
		///         null if no suitable Component can be found </returns>
		/// <exception cref="IllegalArgumentException"> if aContainer is not a focus cycle
		///         root of aComponent or focus traversal policy provider, or if either aContainer or
		///         aComponent is null </exception>
		public override Component GetComponentAfter(Container aContainer, Component aComponent)
		{
			if (Log.isLoggable(PlatformLogger.Level.FINE))
			{
				Log.fine("### Searching in " + aContainer + " for component after " + aComponent);
			}

			if (aContainer == null || aComponent == null)
			{
				throw new IllegalArgumentException("aContainer and aComponent cannot be null");
			}
			if (!aContainer.FocusTraversalPolicyProvider && !aContainer.FocusCycleRoot)
			{
				throw new IllegalArgumentException("aContainer should be focus cycle root or focus traversal policy provider");

			}
			else if (aContainer.FocusCycleRoot && !aComponent.IsFocusCycleRoot(aContainer))
			{
				throw new IllegalArgumentException("aContainer is not a focus cycle root of aComponent");
			}

			lock (aContainer.TreeLock)
			{

				if (!(aContainer.Visible && aContainer.Displayable))
				{
					return null;
				}

				// Before all the ckecks below we first see if it's an FTP provider or a focus cycle root.
				// If it's the case just go down cycle (if it's set to "implicit").
				Component comp = GetComponentDownCycle(aComponent, FORWARD_TRAVERSAL);
				if (comp != null)
				{
					return comp;
				}

				// See if the component is inside of policy provider.
				Container provider = GetTopmostProvider(aContainer, aComponent);
				if (provider != null)
				{
					if (Log.isLoggable(PlatformLogger.Level.FINE))
					{
						Log.fine("### Asking FTP " + provider + " for component after " + aComponent);
					}

					// FTP knows how to find component after the given. We don't.
					FocusTraversalPolicy policy = provider.FocusTraversalPolicy;
					Component afterComp = policy.GetComponentAfter(provider, aComponent);

					// Null result means that we overstepped the limit of the FTP's cycle.
					// In that case we must quit the cycle, otherwise return the component found.
					if (afterComp != null)
					{
						if (Log.isLoggable(PlatformLogger.Level.FINE))
						{
							Log.fine("### FTP returned " + afterComp);
						}
						return afterComp;
					}
					aComponent = provider;
				}

				IList<Component> cycle = GetFocusTraversalCycle(aContainer);

				if (Log.isLoggable(PlatformLogger.Level.FINE))
				{
					Log.fine("### Cycle is " + cycle + ", component is " + aComponent);
				}

				int index = GetComponentIndex(cycle, aComponent);

				if (index < 0)
				{
					if (Log.isLoggable(PlatformLogger.Level.FINE))
					{
						Log.fine("### Didn't find component " + aComponent + " in a cycle " + aContainer);
					}
					return GetFirstComponent(aContainer);
				}

				for (index++; index < cycle.Count; index++)
				{
					comp = cycle[index];
					if (Accept(comp))
					{
						return comp;
					}
					else if ((comp = GetComponentDownCycle(comp, FORWARD_TRAVERSAL)) != null)
					{
						return comp;
					}
				}

				if (aContainer.FocusCycleRoot)
				{
					this.CachedRoot = aContainer;
					this.CachedCycle = cycle;

					comp = GetFirstComponent(aContainer);

					this.CachedRoot = null;
					this.CachedCycle = null;

					return comp;
				}
			}
			return null;
		}

		/// <summary>
		/// Returns the Component that should receive the focus before aComponent.
		/// aContainer must be a focus cycle root of aComponent or a <a
		/// href="doc-files/FocusSpec.html#FocusTraversalPolicyProviders">focus traversal policy
		/// provider</a>.
		/// </summary>
		/// <param name="aContainer"> a focus cycle root of aComponent or focus traversal policy provider </param>
		/// <param name="aComponent"> a (possibly indirect) child of aContainer, or
		///        aContainer itself </param>
		/// <returns> the Component that should receive the focus before aComponent,
		///         or null if no suitable Component can be found </returns>
		/// <exception cref="IllegalArgumentException"> if aContainer is not a focus cycle
		///         root of aComponent or focus traversal policy provider, or if either aContainer or
		///         aComponent is null </exception>
		public override Component GetComponentBefore(Container aContainer, Component aComponent)
		{
			if (aContainer == null || aComponent == null)
			{
				throw new IllegalArgumentException("aContainer and aComponent cannot be null");
			}
			if (!aContainer.FocusTraversalPolicyProvider && !aContainer.FocusCycleRoot)
			{
				throw new IllegalArgumentException("aContainer should be focus cycle root or focus traversal policy provider");

			}
			else if (aContainer.FocusCycleRoot && !aComponent.IsFocusCycleRoot(aContainer))
			{
				throw new IllegalArgumentException("aContainer is not a focus cycle root of aComponent");
			}

			lock (aContainer.TreeLock)
			{

				if (!(aContainer.Visible && aContainer.Displayable))
				{
					return null;
				}

				// See if the component is inside of policy provider.
				Container provider = GetTopmostProvider(aContainer, aComponent);
				if (provider != null)
				{
					if (Log.isLoggable(PlatformLogger.Level.FINE))
					{
						Log.fine("### Asking FTP " + provider + " for component after " + aComponent);
					}

					// FTP knows how to find component after the given. We don't.
					FocusTraversalPolicy policy = provider.FocusTraversalPolicy;
					Component beforeComp = policy.GetComponentBefore(provider, aComponent);

					// Null result means that we overstepped the limit of the FTP's cycle.
					// In that case we must quit the cycle, otherwise return the component found.
					if (beforeComp != null)
					{
						if (Log.isLoggable(PlatformLogger.Level.FINE))
						{
							Log.fine("### FTP returned " + beforeComp);
						}
						return beforeComp;
					}
					aComponent = provider;

					// If the provider is traversable it's returned.
					if (Accept(aComponent))
					{
						return aComponent;
					}
				}

				IList<Component> cycle = GetFocusTraversalCycle(aContainer);

				if (Log.isLoggable(PlatformLogger.Level.FINE))
				{
					Log.fine("### Cycle is " + cycle + ", component is " + aComponent);
				}

				int index = GetComponentIndex(cycle, aComponent);

				if (index < 0)
				{
					if (Log.isLoggable(PlatformLogger.Level.FINE))
					{
						Log.fine("### Didn't find component " + aComponent + " in a cycle " + aContainer);
					}
					return GetLastComponent(aContainer);
				}

				Component comp = null;
				Component tryComp = null;

				for (index--; index >= 0; index--)
				{
					comp = cycle[index];
					if (comp != aContainer && (tryComp = GetComponentDownCycle(comp, BACKWARD_TRAVERSAL)) != null)
					{
						return tryComp;
					}
					else if (Accept(comp))
					{
						return comp;
					}
				}

				if (aContainer.FocusCycleRoot)
				{
					this.CachedRoot = aContainer;
					this.CachedCycle = cycle;

					comp = GetLastComponent(aContainer);

					this.CachedRoot = null;
					this.CachedCycle = null;

					return comp;
				}
			}
			return null;
		}

		/// <summary>
		/// Returns the first Component in the traversal cycle. This method is used
		/// to determine the next Component to focus when traversal wraps in the
		/// forward direction.
		/// </summary>
		/// <param name="aContainer"> the focus cycle root or focus traversal policy provider whose first
		///        Component is to be returned </param>
		/// <returns> the first Component in the traversal cycle of aContainer,
		///         or null if no suitable Component can be found </returns>
		/// <exception cref="IllegalArgumentException"> if aContainer is null </exception>
		public override Component GetFirstComponent(Container aContainer)
		{
			IList<Component> cycle;

			if (Log.isLoggable(PlatformLogger.Level.FINE))
			{
				Log.fine("### Getting first component in " + aContainer);
			}
			if (aContainer == null)
			{
				throw new IllegalArgumentException("aContainer cannot be null");

			}

			lock (aContainer.TreeLock)
			{

				if (!(aContainer.Visible && aContainer.Displayable))
				{
					return null;
				}

				if (this.CachedRoot == aContainer)
				{
					cycle = this.CachedCycle;
				}
				else
				{
					cycle = GetFocusTraversalCycle(aContainer);
				}

				if (cycle.Count == 0)
				{
					if (Log.isLoggable(PlatformLogger.Level.FINE))
					{
						Log.fine("### Cycle is empty");
					}
					return null;
				}
				if (Log.isLoggable(PlatformLogger.Level.FINE))
				{
					Log.fine("### Cycle is " + cycle);
				}

				foreach (Component comp in cycle)
				{
					if (Accept(comp))
					{
						return comp;
					}
					else if (comp != aContainer && (comp = GetComponentDownCycle(comp, FORWARD_TRAVERSAL)) != null)
					{
						return comp;
					}
				}
			}
			return null;
		}

		/// <summary>
		/// Returns the last Component in the traversal cycle. This method is used
		/// to determine the next Component to focus when traversal wraps in the
		/// reverse direction.
		/// </summary>
		/// <param name="aContainer"> the focus cycle root or focus traversal policy provider whose last
		///        Component is to be returned </param>
		/// <returns> the last Component in the traversal cycle of aContainer,
		///         or null if no suitable Component can be found </returns>
		/// <exception cref="IllegalArgumentException"> if aContainer is null </exception>
		public override Component GetLastComponent(Container aContainer)
		{
			IList<Component> cycle;
			if (Log.isLoggable(PlatformLogger.Level.FINE))
			{
				Log.fine("### Getting last component in " + aContainer);
			}

			if (aContainer == null)
			{
				throw new IllegalArgumentException("aContainer cannot be null");
			}

			lock (aContainer.TreeLock)
			{

				if (!(aContainer.Visible && aContainer.Displayable))
				{
					return null;
				}

				if (this.CachedRoot == aContainer)
				{
					cycle = this.CachedCycle;
				}
				else
				{
					cycle = GetFocusTraversalCycle(aContainer);
				}

				if (cycle.Count == 0)
				{
					if (Log.isLoggable(PlatformLogger.Level.FINE))
					{
						Log.fine("### Cycle is empty");
					}
					return null;
				}
				if (Log.isLoggable(PlatformLogger.Level.FINE))
				{
					Log.fine("### Cycle is " + cycle);
				}

				for (int i = cycle.Count - 1; i >= 0; i--)
				{
					Component comp = cycle[i];
					if (Accept(comp))
					{
						return comp;
					}
					else if (comp is Container && comp != aContainer)
					{
						Container cont = (Container)comp;
						if (cont.FocusTraversalPolicyProvider)
						{
							Component retComp = cont.FocusTraversalPolicy.GetLastComponent(cont);
							if (retComp != null)
							{
								return retComp;
							}
						}
					}
				}
			}
			return null;
		}

		/// <summary>
		/// Returns the default Component to focus. This Component will be the first
		/// to receive focus when traversing down into a new focus traversal cycle
		/// rooted at aContainer. The default implementation of this method
		/// returns the same Component as <code>getFirstComponent</code>.
		/// </summary>
		/// <param name="aContainer"> the focus cycle root or focus traversal policy provider whose default
		///        Component is to be returned </param>
		/// <returns> the default Component in the traversal cycle of aContainer,
		///         or null if no suitable Component can be found </returns>
		/// <seealso cref= #getFirstComponent </seealso>
		/// <exception cref="IllegalArgumentException"> if aContainer is null </exception>
		public override Component GetDefaultComponent(Container aContainer)
		{
			return GetFirstComponent(aContainer);
		}

		/// <summary>
		/// Sets whether this ContainerOrderFocusTraversalPolicy transfers focus
		/// down-cycle implicitly. If <code>true</code>, during normal forward focus
		/// traversal, the Component traversed after a focus cycle root will be the
		/// focus-cycle-root's default Component to focus. If <code>false</code>,
		/// the next Component in the focus traversal cycle rooted at the specified
		/// focus cycle root will be traversed instead. The default value for this
		/// property is <code>true</code>.
		/// </summary>
		/// <param name="implicitDownCycleTraversal"> whether this
		///        ContainerOrderFocusTraversalPolicy transfers focus down-cycle
		///        implicitly </param>
		/// <seealso cref= #getImplicitDownCycleTraversal </seealso>
		/// <seealso cref= #getFirstComponent </seealso>
		public virtual bool ImplicitDownCycleTraversal
		{
			set
			{
				this.ImplicitDownCycleTraversal_Renamed = value;
			}
			get
			{
				return ImplicitDownCycleTraversal_Renamed;
			}
		}


		/// <summary>
		/// Determines whether a Component is an acceptable choice as the new
		/// focus owner. By default, this method will accept a Component if and
		/// only if it is visible, displayable, enabled, and focusable.
		/// </summary>
		/// <param name="aComponent"> the Component whose fitness as a focus owner is to
		///        be tested </param>
		/// <returns> <code>true</code> if aComponent is visible, displayable,
		///         enabled, and focusable; <code>false</code> otherwise </returns>
		protected internal virtual bool Accept(Component aComponent)
		{
			if (!aComponent.CanBeFocusOwner())
			{
				return false;
			}

			// Verify that the Component is recursively enabled. Disabling a
			// heavyweight Container disables its children, whereas disabling
			// a lightweight Container does not.
			if (!(aComponent is Window))
			{
				for (Container enableTest = aComponent.Parent; enableTest != null; enableTest = enableTest.Parent)
				{
					if (!(enableTest.Enabled || enableTest.Lightweight))
					{
						return false;
					}
					if (enableTest is Window)
					{
						break;
					}
				}
			}

			return true;
		}
	}

}
using System;

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

	using Empty = sun.invoke.empty.Empty;

	/// <summary>
	/// A {@code CallSite} is a holder for a variable <seealso cref="MethodHandle"/>,
	/// which is called its {@code target}.
	/// An {@code invokedynamic} instruction linked to a {@code CallSite} delegates
	/// all calls to the site's current target.
	/// A {@code CallSite} may be associated with several {@code invokedynamic}
	/// instructions, or it may be "free floating", associated with none.
	/// In any case, it may be invoked through an associated method handle
	/// called its <seealso cref="#dynamicInvoker dynamic invoker"/>.
	/// <para>
	/// {@code CallSite} is an abstract class which does not allow
	/// direct subclassing by users.  It has three immediate,
	/// concrete subclasses that may be either instantiated or subclassed.
	/// <ul>
	/// <li>If a mutable target is not required, an {@code invokedynamic} instruction
	/// may be permanently bound by means of a <seealso cref="ConstantCallSite constant call site"/>.
	/// <li>If a mutable target is required which has volatile variable semantics,
	/// because updates to the target must be immediately and reliably witnessed by other threads,
	/// a <seealso cref="VolatileCallSite volatile call site"/> may be used.
	/// <li>Otherwise, if a mutable target is required,
	/// a <seealso cref="MutableCallSite mutable call site"/> may be used.
	/// </ul>
	/// </para>
	/// <para>
	/// A non-constant call site may be <em>relinked</em> by changing its target.
	/// The new target must have the same <seealso cref="MethodHandle#type() type"/>
	/// as the previous target.
	/// Thus, though a call site can be relinked to a series of
	/// successive targets, it cannot change its type.
	/// </para>
	/// <para>
	/// Here is a sample use of call sites and bootstrap methods which links every
	/// dynamic call site to print its arguments:
	/// <blockquote><pre>{@code
	/// static void test() throws Throwable {
	///    // THE FOLLOWING LINE IS PSEUDOCODE FOR A JVM INSTRUCTION
	///    InvokeDynamic[#bootstrapDynamic].baz("baz arg", 2, 3.14);
	/// }
	/// private static void printArgs(Object... args) {
	///  System.out.println(java.util.Arrays.deepToString(args));
	/// }
	/// private static final MethodHandle printArgs;
	/// static {
	///  MethodHandles.Lookup lookup = MethodHandles.lookup();
	///  Class thisClass = lookup.lookupClass();  // (who am I?)
	///  printArgs = lookup.findStatic(thisClass,
	///      "printArgs", MethodType.methodType(void.class, Object[].class));
	/// }
	/// private static CallSite bootstrapDynamic(MethodHandles.Lookup caller, String name, MethodType type) {
	///  // ignore caller and name, but match the type:
	///  return new ConstantCallSite(printArgs.asType(type));
	/// }
	/// }</pre></blockquote>
	/// @author John Rose, JSR 292 EG
	/// </para>
	/// </summary>
	public abstract class CallSite
	{
		static CallSite()
		{
			MethodHandleImpl.InitStatics();
			try
			{
				GET_TARGET = IMPL_LOOKUP.findVirtual(typeof(CallSite), "getTarget", MethodType.MethodType(typeof(MethodHandle)));
				THROW_UCS = IMPL_LOOKUP.findStatic(typeof(CallSite), "uninitializedCallSite", MethodType.MethodType(typeof(Object), typeof(Object[])));
			}
			catch (ReflectiveOperationException e)
			{
				throw newInternalError(e);
			}
			try
			{
				TARGET_OFFSET = UNSAFE.objectFieldOffset(typeof(CallSite).getDeclaredField("target"));
			}
			catch (Exception ex)
			{
				throw new Error(ex);
			}
		}

		// The actual payload of this call site:
		/*package-private*/
		internal MethodHandle Target_Renamed; // Note: This field is known to the JVM.  Do not change.

		/// <summary>
		/// Make a blank call site object with the given method type.
		/// An initial target method is supplied which will throw
		/// an <seealso cref="IllegalStateException"/> if called.
		/// <para>
		/// Before this {@code CallSite} object is returned from a bootstrap method,
		/// it is usually provided with a more useful target method,
		/// via a call to <seealso cref="CallSite#setTarget(MethodHandle) setTarget"/>.
		/// </para>
		/// </summary>
		/// <exception cref="NullPointerException"> if the proposed type is null </exception>
		/*package-private*/
		internal CallSite(MethodType type)
		{
			Target_Renamed = MakeUninitializedCallSite(type);
		}

		/// <summary>
		/// Make a call site object equipped with an initial target method handle. </summary>
		/// <param name="target"> the method handle which will be the initial target of the call site </param>
		/// <exception cref="NullPointerException"> if the proposed target is null </exception>
		/*package-private*/
		internal CallSite(MethodHandle target)
		{
			target.Type(); // null check
			this.Target_Renamed = target;
		}

		/// <summary>
		/// Make a call site object equipped with an initial target method handle. </summary>
		/// <param name="targetType"> the desired type of the call site </param>
		/// <param name="createTargetHook"> a hook which will bind the call site to the target method handle </param>
		/// <exception cref="WrongMethodTypeException"> if the hook cannot be invoked on the required arguments,
		///         or if the target returned by the hook is not of the given {@code targetType} </exception>
		/// <exception cref="NullPointerException"> if the hook returns a null value </exception>
		/// <exception cref="ClassCastException"> if the hook returns something other than a {@code MethodHandle} </exception>
		/// <exception cref="Throwable"> anything else thrown by the hook function </exception>
		/*package-private*/
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: CallSite(MethodType targetType, MethodHandle createTargetHook) throws Throwable
		internal CallSite(MethodType targetType, MethodHandle createTargetHook) : this(targetType)
		{
			ConstantCallSite selfCCS = (ConstantCallSite) this;
			MethodHandle boundTarget = (MethodHandle) createTargetHook.InvokeWithArguments(selfCCS);
			CheckTargetChange(this.Target_Renamed, boundTarget);
			this.Target_Renamed = boundTarget;
		}

		/// <summary>
		/// Returns the type of this call site's target.
		/// Although targets may change, any call site's type is permanent, and can never change to an unequal type.
		/// The {@code setTarget} method enforces this invariant by refusing any new target that does
		/// not have the previous target's type. </summary>
		/// <returns> the type of the current target, which is also the type of any future target </returns>
		public virtual MethodType Type()
		{
			// warning:  do not call getTarget here, because CCS.getTarget can throw IllegalStateException
			return Target_Renamed.Type();
		}

		/// <summary>
		/// Returns the target method of the call site, according to the
		/// behavior defined by this call site's specific class.
		/// The immediate subclasses of {@code CallSite} document the
		/// class-specific behaviors of this method.
		/// </summary>
		/// <returns> the current linkage state of the call site, its target method handle </returns>
		/// <seealso cref= ConstantCallSite </seealso>
		/// <seealso cref= VolatileCallSite </seealso>
		/// <seealso cref= #setTarget </seealso>
		/// <seealso cref= ConstantCallSite#getTarget </seealso>
		/// <seealso cref= MutableCallSite#getTarget </seealso>
		/// <seealso cref= VolatileCallSite#getTarget </seealso>
		public abstract MethodHandle Target {get;set;}


		internal virtual void CheckTargetChange(MethodHandle oldTarget, MethodHandle newTarget)
		{
			MethodType oldType = oldTarget.Type();
			MethodType newType = newTarget.Type(); // null check!
			if (!newType.Equals(oldType))
			{
				throw WrongTargetType(newTarget, oldType);
			}
		}

		private static WrongMethodTypeException WrongTargetType(MethodHandle target, MethodType type)
		{
			return new WrongMethodTypeException(Convert.ToString(target) + " should be of type " + type);
		}

		/// <summary>
		/// Produces a method handle equivalent to an invokedynamic instruction
		/// which has been linked to this call site.
		/// <para>
		/// This method is equivalent to the following code:
		/// <blockquote><pre>{@code
		/// MethodHandle getTarget, invoker, result;
		/// getTarget = MethodHandles.publicLookup().bind(this, "getTarget", MethodType.methodType(MethodHandle.class));
		/// invoker = MethodHandles.exactInvoker(this.type());
		/// result = MethodHandles.foldArguments(invoker, getTarget)
		/// }</pre></blockquote>
		/// 
		/// </para>
		/// </summary>
		/// <returns> a method handle which always invokes this call site's current target </returns>
		public abstract MethodHandle DynamicInvoker();

		/*non-public*/	 internal virtual MethodHandle MakeDynamicInvoker()
	 {
			MethodHandle getTarget = GET_TARGET.BindArgumentL(0, this);
			MethodHandle invoker = MethodHandles.ExactInvoker(this.Type());
			return MethodHandles.FoldArguments(invoker, getTarget);
	 }

		private static readonly MethodHandle GET_TARGET;
		private static readonly MethodHandle THROW_UCS;

		/// <summary>
		/// This guy is rolled into the default target if a MethodType is supplied to the constructor. </summary>
		private static Object UninitializedCallSite(params Object[] ignore)
		{
			throw new IllegalStateException("uninitialized call site");
		}

		private MethodHandle MakeUninitializedCallSite(MethodType targetType)
		{
			MethodType basicType = targetType.BasicType();
			MethodHandle invoker = basicType.Form().CachedMethodHandle(MethodTypeForm.MH_UNINIT_CS);
			if (invoker == null)
			{
				invoker = THROW_UCS.AsType(basicType);
				invoker = basicType.Form().SetCachedMethodHandle(MethodTypeForm.MH_UNINIT_CS, invoker);
			}
			// unchecked view is OK since no values will be received or returned
			return invoker.ViewAsType(targetType, false);
		}

		// unsafe stuff:
		private static readonly long TARGET_OFFSET;

		/*package-private*/
		internal virtual MethodHandle TargetNormal
		{
			set
			{
				MethodHandleNatives.setCallSiteTargetNormal(this, value);
			}
		}
		/*package-private*/
		internal virtual MethodHandle TargetVolatile
		{
			get
			{
				return (MethodHandle) UNSAFE.getObjectVolatile(this, TARGET_OFFSET);
			}
			set
			{
				MethodHandleNatives.setCallSiteTargetVolatile(this, value);
			}
		}

		// this implements the upcall from the JVM, MethodHandleNatives.makeDynamicCallSite:
		internal static CallSite MakeSite(MethodHandle bootstrapMethod, String name, MethodType type, Object info, Class callerClass)
								 // Callee information:
								 // Extra arguments for BSM, if any:
								 // Caller information:
		{
			MethodHandles.Lookup caller = IMPL_LOOKUP.@in(callerClass);
			CallSite site;
			try
			{
				Object binding;
				info = MaybeReBox(info);
				if (info == null)
				{
					binding = bootstrapMethod.invoke(caller, name, type);
				}
				else if (!info.GetType().IsArray)
				{
					binding = bootstrapMethod.invoke(caller, name, type, info);
				}
				else
				{
					Object[] argv = (Object[]) info;
					MaybeReBoxElements(argv);
					switch (argv.Length)
					{
					case 0:
						binding = bootstrapMethod.invoke(caller, name, type);
						break;
					case 1:
						binding = bootstrapMethod.invoke(caller, name, type, argv[0]);
						break;
					case 2:
						binding = bootstrapMethod.invoke(caller, name, type, argv[0], argv[1]);
						break;
					case 3:
						binding = bootstrapMethod.invoke(caller, name, type, argv[0], argv[1], argv[2]);
						break;
					case 4:
						binding = bootstrapMethod.invoke(caller, name, type, argv[0], argv[1], argv[2], argv[3]);
						break;
					case 5:
						binding = bootstrapMethod.invoke(caller, name, type, argv[0], argv[1], argv[2], argv[3], argv[4]);
						break;
					case 6:
						binding = bootstrapMethod.invoke(caller, name, type, argv[0], argv[1], argv[2], argv[3], argv[4], argv[5]);
						break;
					default:
						const int NON_SPREAD_ARG_COUNT = 3; // (caller, name, type)
						if (NON_SPREAD_ARG_COUNT + argv.Length > MethodType.MAX_MH_ARITY)
						{
							throw new BootstrapMethodError("too many bootstrap method arguments");
						}
						MethodType bsmType = bootstrapMethod.Type();
						MethodType invocationType = MethodType.GenericMethodType(NON_SPREAD_ARG_COUNT + argv.Length);
						MethodHandle typedBSM = bootstrapMethod.AsType(invocationType);
						MethodHandle spreader = invocationType.Invokers().SpreadInvoker(NON_SPREAD_ARG_COUNT);
						binding = spreader.invokeExact(typedBSM, (Object)caller, (Object)name, (Object)type, argv);
					break;
					}
				}
				//System.out.println("BSM for "+name+type+" => "+binding);
				if (binding is CallSite)
				{
					site = (CallSite) binding;
				}
				else
				{
					throw new ClassCastException("bootstrap method failed to produce a CallSite");
				}
				if (!site.Target.Type().Equals(type))
				{
					throw WrongTargetType(site.Target, type);
				}
			}
			catch (Throwable ex)
			{
				BootstrapMethodError bex;
				if (ex is BootstrapMethodError)
				{
					bex = (BootstrapMethodError) ex;
				}
				else
				{
					bex = new BootstrapMethodError("call site initialization exception", ex);
				}
				throw bex;
			}
			return site;
		}

		private static Object MaybeReBox(Object x)
		{
			if (x is Integer)
			{
				int xi = (int) x;
				if (xi == (sbyte) xi)
				{
					x = xi; // must rebox; see JLS 5.1.7
				}
			}
			return x;
		}
		private static void MaybeReBoxElements(Object[] xa)
		{
			for (int i = 0; i < xa.Length; i++)
			{
				xa[i] = MaybeReBox(xa[i]);
			}
		}
	}

}
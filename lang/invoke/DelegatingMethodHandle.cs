/*
 * Copyright (c) 2014, Oracle and/or its affiliates. All rights reserved.
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


	/// <summary>
	/// A method handle whose invocation behavior is determined by a target.
	/// The delegating MH itself can hold extra "intentions" beyond the simple behavior.
	/// @author jrose
	/// </summary>
	/*non-public*/
	internal abstract class DelegatingMethodHandle : MethodHandle
	{
		protected internal DelegatingMethodHandle(MethodHandle target) : this(target.Type(), target)
		{
		}

		protected internal DelegatingMethodHandle(MethodType type, MethodHandle target) : base(type, ChooseDelegatingForm(target))
		{
		}

		protected internal DelegatingMethodHandle(MethodType type, LambdaForm form) : base(type, form)
		{
		}

		/// <summary>
		/// Define this to extract the delegated target which supplies the invocation behavior. </summary>
		protected internal abstract MethodHandle Target {get;}

		internal override abstract MethodHandle AsTypeUncached(MethodType newType);

		internal override MemberName InternalMemberName()
		{
			return Target.InternalMemberName();
		}

		internal override bool InvokeSpecial
		{
			get
			{
				return Target.InvokeSpecial;
			}
		}

		internal override Class InternalCallerClass()
		{
			return Target.InternalCallerClass();
		}

		internal override MethodHandle CopyWith(MethodType mt, LambdaForm lf)
		{
			// FIXME: rethink 'copyWith' protocol; it is too low-level for use on all MHs
			throw newIllegalArgumentException("do not use this");
		}

		internal override String InternalProperties()
		{
			return "\n& Class=" + this.GetType().Name + "\n& Target=" + Target.DebugString();
		}

		internal override BoundMethodHandle Rebind()
		{
			return Target.Rebind();
		}

		private static LambdaForm ChooseDelegatingForm(MethodHandle target)
		{
			if (target is SimpleMethodHandle)
			{
				return target.InternalForm(); // no need for an indirection
			}
			return MakeReinvokerForm(target, MethodTypeForm.LF_DELEGATE, typeof(DelegatingMethodHandle), NF_getTarget);
		}

		internal static LambdaForm MakeReinvokerForm(MethodHandle target, int whichCache, Object constraint, NamedFunction getTargetFn)
		{
			String debugString;
			switch (whichCache)
			{
				case MethodTypeForm.LF_REBIND:
					debugString = "BMH.reinvoke";
					break;
				case MethodTypeForm.LF_DELEGATE:
					debugString = "MH.delegate";
					break;
				default:
					debugString = "MH.reinvoke";
					break;
			}
			// No pre-action needed.
			return MakeReinvokerForm(target, whichCache, constraint, debugString, true, getTargetFn, null);
		}
		/// <summary>
		/// Create a LF which simply reinvokes a target of the given basic type. </summary>
		internal static LambdaForm MakeReinvokerForm(MethodHandle target, int whichCache, Object constraint, String debugString, bool forceInline, NamedFunction getTargetFn, NamedFunction preActionFn)
		{
			MethodType mtype = target.Type().BasicType();
			bool customized = (whichCache < 0 || mtype.ParameterSlotCount() > MethodType.MAX_MH_INVOKER_ARITY);
			bool hasPreAction = (preActionFn != null);
			LambdaForm form;
			if (!customized)
			{
				form = mtype.Form().CachedLambdaForm(whichCache);
				if (form != null)
				{
					return form;
				}
			}
			const int THIS_DMH = 0;
			const int ARG_BASE = 1;
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final int ARG_LIMIT = ARG_BASE + mtype.parameterCount();
			int ARG_LIMIT = ARG_BASE + mtype.ParameterCount();
			int nameCursor = ARG_LIMIT;
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final int PRE_ACTION = hasPreAction ? nameCursor++ : -1;
			int PRE_ACTION = hasPreAction ? nameCursor++: -1;
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final int NEXT_MH = customized ? -1 : nameCursor++;
			int NEXT_MH = customized ? - 1 : nameCursor++;
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final int REINVOKE = nameCursor++;
			int REINVOKE = nameCursor++;
			LambdaForm.Name[] names = LambdaForm.Arguments(nameCursor - ARG_LIMIT, mtype.InvokerType());
			assert(names.Length == nameCursor);
			names[THIS_DMH] = names[THIS_DMH].WithConstraint(constraint);
			Object[] targetArgs;
			if (hasPreAction)
			{
				names[PRE_ACTION] = new LambdaForm.Name(preActionFn, names[THIS_DMH]);
			}
			if (customized)
			{
				targetArgs = Arrays.CopyOfRange(names, ARG_BASE, ARG_LIMIT, typeof(Object[]));
				names[REINVOKE] = new LambdaForm.Name(target, targetArgs); // the invoker is the target itself
			}
			else
			{
				names[NEXT_MH] = new LambdaForm.Name(getTargetFn, names[THIS_DMH]);
				targetArgs = Arrays.CopyOfRange(names, THIS_DMH, ARG_LIMIT, typeof(Object[]));
				targetArgs[0] = names[NEXT_MH]; // overwrite this MH with next MH
				names[REINVOKE] = new LambdaForm.Name(mtype, targetArgs);
			}
			form = new LambdaForm(debugString, ARG_LIMIT, names, forceInline);
			if (!customized)
			{
				form = mtype.Form().SetCachedLambdaForm(whichCache, form);
			}
			return form;
		}

		internal static readonly NamedFunction NF_getTarget;
		static DelegatingMethodHandle()
		{
			try
			{
				NF_getTarget = new NamedFunction(typeof(DelegatingMethodHandle).getDeclaredMethod("getTarget"));
			}
			catch (ReflectiveOperationException ex)
			{
				throw newInternalError(ex);
			}
		}
	}

}
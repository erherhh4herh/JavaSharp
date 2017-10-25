/*
 * Copyright (c) 2012, 2013, Oracle and/or its affiliates. All rights reserved.
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

	using MethodVisitor = jdk.@internal.org.objectweb.asm.MethodVisitor;
	using Opcodes = jdk.@internal.org.objectweb.asm.Opcodes;
	using Type = jdk.@internal.org.objectweb.asm.Type;
	using BytecodeDescriptor = sun.invoke.util.BytecodeDescriptor;
	using Wrapper = sun.invoke.util.Wrapper;
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to .NET:
	import static sun.invoke.util.Wrapper.*;

	internal class TypeConvertingMethodAdapter : MethodVisitor
	{

		internal TypeConvertingMethodAdapter(MethodVisitor mv) : base(Opcodes.ASM5, mv)
		{
		}

		private static readonly int NUM_WRAPPERS = Wrapper.values().length;

		private const String NAME_OBJECT = "java/lang/Object";
		private const String WRAPPER_PREFIX = "Ljava/lang/";

		// Same for all primitives; name of the boxing method
		private const String NAME_BOX_METHOD = "valueOf";

		// Table of opcodes for widening primitive conversions; NOP = no conversion
//JAVA TO C# CONVERTER NOTE: The following call to the 'RectangularArrays' helper class reproduces the rectangular array initialization that is automatic in Java:
//ORIGINAL LINE: private static readonly int[][] WideningOpcodes = new int[NUM_WRAPPERS][NUM_WRAPPERS];
		private static readonly int[][] WideningOpcodes = RectangularArrays.ReturnRectangularIntArray(NUM_WRAPPERS, NUM_WRAPPERS);

		private static readonly Wrapper[] FROM_WRAPPER_NAME = new Wrapper[16];

		// Table of wrappers for primitives, indexed by ASM type sorts
		private static readonly Wrapper[] FROM_TYPE_SORT = new Wrapper[16];

		static TypeConvertingMethodAdapter()
		{
			foreach (Wrapper w in Wrapper.values())
			{
				if (w.basicTypeChar() != 'L')
				{
					int wi = HashWrapperName(w.wrapperSimpleName());
					assert(FROM_WRAPPER_NAME[wi] == null);
					FROM_WRAPPER_NAME[wi] = w;
				}
			}

			for (int i = 0; i < NUM_WRAPPERS; i++)
			{
				for (int j = 0; j < NUM_WRAPPERS; j++)
				{
					WideningOpcodes[i][j] = Opcodes.NOP;
				}
			}

			initWidening(LONG, Opcodes.I2L, BYTE, SHORT, INT, CHAR);
			InitWidening(LONG, Opcodes.F2L, FLOAT);
			initWidening(FLOAT, Opcodes.I2F, BYTE, SHORT, INT, CHAR);
			InitWidening(FLOAT, Opcodes.L2F, LONG);
			initWidening(DOUBLE, Opcodes.I2D, BYTE, SHORT, INT, CHAR);
			InitWidening(DOUBLE, Opcodes.F2D, FLOAT);
			InitWidening(DOUBLE, Opcodes.L2D, LONG);

			FROM_TYPE_SORT[Type.BYTE] = Wrapper.BYTE;
			FROM_TYPE_SORT[Type.SHORT] = Wrapper.SHORT;
			FROM_TYPE_SORT[Type.INT] = Wrapper.INT;
			FROM_TYPE_SORT[Type.LONG] = Wrapper.LONG;
			FROM_TYPE_SORT[Type.CHAR] = Wrapper.CHAR;
			FROM_TYPE_SORT[Type.FLOAT] = Wrapper.FLOAT;
			FROM_TYPE_SORT[Type.DOUBLE] = Wrapper.DOUBLE;
			FROM_TYPE_SORT[Type.BOOLEAN] = Wrapper.BOOLEAN;
		}

		private static void InitWidening(Wrapper to, int opcode, params Wrapper[] from)
		{
			foreach (Wrapper f in from)
			{
				WideningOpcodes[f.ordinal()][to.ordinal()] = opcode;
			}
		}

		/// <summary>
		/// Class name to Wrapper hash, derived from Wrapper.hashWrap() </summary>
		/// <param name="xn"> </param>
		/// <returns> The hash code 0-15 </returns>
		private static int HashWrapperName(String xn)
		{
			if (xn.Length() < 3)
			{
				return 0;
			}
			return (3 * xn.CharAt(1) + xn.CharAt(2)) % 16;
		}

		private Wrapper WrapperOrNullFromDescriptor(String desc)
		{
			if (!desc.StartsWith(WRAPPER_PREFIX))
			{
				// Not a class type (array or method), so not a boxed type
				// or not in the right package
				return null;
			}
			// Pare it down to the simple class name
			String cname = StringHelperClass.SubstringSpecial(desc, WRAPPER_PREFIX.Length(), desc.Length() - 1);
			// Hash to a Wrapper
			Wrapper w = FROM_WRAPPER_NAME[HashWrapperName(cname)];
			if (w == null || w.wrapperSimpleName().Equals(cname))
			{
				return w;
			}
			else
			{
				return null;
			}
		}

		private static String WrapperName(Wrapper w)
		{
			return "java/lang/" + w.wrapperSimpleName();
		}

		private static String UnboxMethod(Wrapper w)
		{
			return w.primitiveSimpleName() + "Value";
		}

		private static String BoxingDescriptor(Wrapper w)
		{
			return string.Format("({0})L{1};", w.basicTypeChar(), WrapperName(w));
		}

		private static String UnboxingDescriptor(Wrapper w)
		{
			return "()" + w.basicTypeChar();
		}

		internal virtual void BoxIfTypePrimitive(Type t)
		{
			Wrapper w = FROM_TYPE_SORT[t.Sort];
			if (w != null)
			{
				Box(w);
			}
		}

		internal virtual void Widen(Wrapper ws, Wrapper wt)
		{
			if (ws != wt)
			{
				int opcode = WideningOpcodes[ws.ordinal()][wt.ordinal()];
				if (opcode != Opcodes.NOP)
				{
					visitInsn(opcode);
				}
			}
		}

		internal virtual void Box(Wrapper w)
		{
			visitMethodInsn(Opcodes.INVOKESTATIC, WrapperName(w), NAME_BOX_METHOD, BoxingDescriptor(w), false);
		}

		/// <summary>
		/// Convert types by unboxing. The source type is known to be a primitive wrapper. </summary>
		/// <param name="sname"> A primitive wrapper corresponding to wrapped reference source type </param>
		/// <param name="wt"> A primitive wrapper being converted to </param>
		internal virtual void Unbox(String sname, Wrapper wt)
		{
			visitMethodInsn(Opcodes.INVOKEVIRTUAL, sname, UnboxMethod(wt), UnboxingDescriptor(wt), false);
		}

		private String DescriptorToName(String desc)
		{
			int last = desc.Length() - 1;
			if (desc.CharAt(0) == 'L' && desc.CharAt(last) == ';')
			{
				// In descriptor form
				return desc.Substring(1, last - 1);
			}
			else
			{
				// Already in internal name form
				return desc;
			}
		}

		internal virtual void Cast(String ds, String dt)
		{
			String ns = DescriptorToName(ds);
			String nt = DescriptorToName(dt);
			if (!nt.Equals(ns) && !nt.Equals(NAME_OBJECT))
			{
				visitTypeInsn(Opcodes.CHECKCAST, nt);
			}
		}

		private bool IsPrimitive(Wrapper w)
		{
			return w != OBJECT;
		}

		private Wrapper ToWrapper(String desc)
		{
			char first = desc.CharAt(0);
			if (first == '[' || first == '(')
			{
				first = 'L';
			}
			return Wrapper.forBasicType(first);
		}

		/// <summary>
		/// Convert an argument of type 'arg' to be passed to 'target' assuring that it is 'functional'.
		/// Insert the needed conversion instructions in the method code. </summary>
		/// <param name="arg"> </param>
		/// <param name="target"> </param>
		/// <param name="functional"> </param>
		internal virtual void ConvertType(Class arg, Class target, Class functional)
		{
			if (arg.Equals(target) && arg.Equals(functional))
			{
				return;
			}
			if (arg == Void.TYPE || target == Void.TYPE)
			{
				return;
			}
			if (arg.Primitive)
			{
				Wrapper wArg = Wrapper.forPrimitiveType(arg);
				if (target.Primitive)
				{
					// Both primitives: widening
					Widen(wArg, Wrapper.forPrimitiveType(target));
				}
				else
				{
					// Primitive argument to reference target
					String dTarget = BytecodeDescriptor.unparse(target);
					Wrapper wPrimTarget = WrapperOrNullFromDescriptor(dTarget);
					if (wPrimTarget != null)
					{
						// The target is a boxed primitive type, widen to get there before boxing
						Widen(wArg, wPrimTarget);
						Box(wPrimTarget);
					}
					else
					{
						// Otherwise, box and cast
						Box(wArg);
						Cast(WrapperName(wArg), dTarget);
					}
				}
			}
			else
			{
				String dArg = BytecodeDescriptor.unparse(arg);
				String dSrc;
				if (functional.Primitive)
				{
					dSrc = dArg;
				}
				else
				{
					// Cast to convert to possibly more specific type, and generate CCE for invalid arg
					dSrc = BytecodeDescriptor.unparse(functional);
					Cast(dArg, dSrc);
				}
				String dTarget = BytecodeDescriptor.unparse(target);
				if (target.Primitive)
				{
					Wrapper wTarget = ToWrapper(dTarget);
					// Reference argument to primitive target
					Wrapper wps = WrapperOrNullFromDescriptor(dSrc);
					if (wps != null)
					{
						if (wps.Signed || wps.Floating)
						{
							// Boxed number to primitive
							Unbox(WrapperName(wps), wTarget);
						}
						else
						{
							// Character or Boolean
							Unbox(WrapperName(wps), wps);
							Widen(wps, wTarget);
						}
					}
					else
					{
						// Source type is reference type, but not boxed type,
						// assume it is super type of target type
						String intermediate;
						if (wTarget.Signed || wTarget.Floating)
						{
							// Boxed number to primitive
							intermediate = "java/lang/Number";
						}
						else
						{
							// Character or Boolean
							intermediate = WrapperName(wTarget);
						}
						Cast(dSrc, intermediate);
						Unbox(intermediate, wTarget);
					}
				}
				else
				{
					// Both reference types: just case to target type
					Cast(dSrc, dTarget);
				}
			}
		}

		/// <summary>
		/// The following method is copied from
		/// org.objectweb.asm.commons.InstructionAdapter. Part of ASM: a very small
		/// and fast Java bytecode manipulation framework.
		/// Copyright (c) 2000-2005 INRIA, France Telecom All rights reserved.
		/// </summary>
//JAVA TO C# CONVERTER WARNING: 'final' parameters are not allowed in .NET:
//ORIGINAL LINE: void iconst(final int cst)
		internal virtual void Iconst(int cst)
		{
			if (cst >= -1 && cst <= 5)
			{
				mv.visitInsn(Opcodes.ICONST_0 + cst);
			}
			else if (cst >= Byte.MinValue && cst <= Byte.MaxValue)
			{
				mv.visitIntInsn(Opcodes.BIPUSH, cst);
			}
			else if (cst >= Short.MinValue && cst <= Short.MaxValue)
			{
				mv.visitIntInsn(Opcodes.SIPUSH, cst);
			}
			else
			{
				mv.visitLdcInsn(cst);
			}
		}
	}

}
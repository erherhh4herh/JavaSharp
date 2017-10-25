/*
 * Copyright (c) 2008, 2012, Oracle and/or its affiliates. All rights reserved.
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
	/// A method handle whose behavior is determined only by its LambdaForm.
	/// @author jrose
	/// </summary>
	internal sealed class SimpleMethodHandle : BoundMethodHandle
	{
		private SimpleMethodHandle(MethodType type, LambdaForm form) : base(type, form)
		{
		}

		/*non-public*/	 internal static BoundMethodHandle Make(MethodType type, LambdaForm form)
	 {
			return new SimpleMethodHandle(type, form);
	 }

		/*non-public*/	 internal new static readonly SpeciesData SPECIES_DATA = SpeciesData.EMPTY;

		/*non-public*/	 public override SpeciesData SpeciesData()
	 {
				return SPECIES_DATA;
	 }

		internal override BoundMethodHandle CopyWith(MethodType mt, LambdaForm lf)
		/*non-public*/
		{
			return Make(mt, lf);
		}

		internal override String InternalProperties()
		{
			return "\n& Class=" + this.GetType().Name;
		}

		public override int FieldCount()
		/*non-public*/
		{
			return 0;
		}

		internal override BoundMethodHandle CopyWithExtendL(MethodType mt, LambdaForm lf, Object narg)
		/*non-public*/
		{
			return BoundMethodHandle.BindSingle(mt, lf, narg); // Use known fast path.
		}
		internal override BoundMethodHandle CopyWithExtendI(MethodType mt, LambdaForm lf, int narg)
		/*non-public*/
		{
			try
			{
				return (BoundMethodHandle) SPECIES_DATA.ExtendWith(I_TYPE).Constructor().invokeBasic(mt, lf, narg);
			}
			catch (Throwable ex)
			{
				throw uncaughtException(ex);
			}
		}
		internal override BoundMethodHandle CopyWithExtendJ(MethodType mt, LambdaForm lf, long narg)
		/*non-public*/
		{
			try
			{
				return (BoundMethodHandle) SPECIES_DATA.ExtendWith(J_TYPE).Constructor().invokeBasic(mt, lf, narg);
			}
			catch (Throwable ex)
			{
				throw uncaughtException(ex);
			}
		}
		internal override BoundMethodHandle CopyWithExtendF(MethodType mt, LambdaForm lf, float narg)
		/*non-public*/
		{
			try
			{
				return (BoundMethodHandle) SPECIES_DATA.ExtendWith(F_TYPE).Constructor().invokeBasic(mt, lf, narg);
			}
			catch (Throwable ex)
			{
				throw uncaughtException(ex);
			}
		}
		internal override BoundMethodHandle CopyWithExtendD(MethodType mt, LambdaForm lf, double narg)
		/*non-public*/
		{
			try
			{
				return (BoundMethodHandle) SPECIES_DATA.ExtendWith(D_TYPE).Constructor().invokeBasic(mt, lf, narg);
			}
			catch (Throwable ex)
			{
				throw uncaughtException(ex);
			}
		}
	}

}
using System;
using System.Collections.Generic;

/*
 * Copyright (c) 2013, 2014, Oracle and/or its affiliates. All rights reserved.
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
	/// Working storage for an LF that is being transformed.
	///  Similarly to a StringBuffer, the editing can take place in multiple steps.
	/// </summary>
	internal sealed class LambdaFormBuffer
	{
		private int Arity, Length;
		private Name[] Names_Renamed;
		private Name[] OriginalNames; // snapshot of pre-transaction names
		private sbyte Flags;
		private int FirstChange;
		private Name ResultName;
		private String DebugName;
		private List<Name> Dups;

		private const int F_TRANS = 0x10, F_OWNED = 0x03;

		internal LambdaFormBuffer(LambdaForm lf)
		{
			this.Arity = lf.Arity_Renamed;
			Names = lf.Names;
			int result = lf.Result;
			if (result == LAST_RESULT)
			{
				result = Length - 1;
			}
			if (result >= 0 && lf.Names[result].Type_Renamed != V_TYPE)
			{
				ResultName = lf.Names[result];
			}
			DebugName = lf.DebugName;
			assert(lf.NameRefsAreLegal());
		}

		private LambdaForm LambdaForm()
		{
			assert(!InTrans()); // need endEdit call to tidy things up
			return new LambdaForm(DebugName, Arity, NameArray(), ResultIndex());
		}

		internal Name Name(int i)
		{
			assert(i < Length);
			return Names_Renamed[i];
		}

		internal Name[] NameArray()
		{
			return Arrays.CopyOf(Names_Renamed, Length);
		}

		internal int ResultIndex()
		{
			if (ResultName == null)
			{
				return VOID_RESULT;
			}
			int index = IndexOf(ResultName, Names_Renamed);
			assert(index >= 0);
			return index;
		}

		internal Name[] Names
		{
			set
			{
				Names_Renamed = OriginalNames = value; // keep a record of where everything was to start with
				Length = value.Length;
				Flags = 0;
			}
		}

		private bool VerifyArity()
		{
			for (int i = 0; i < Arity && i < FirstChange; i++)
			{
				assert(Names_Renamed[i].Param) : "#" + i + "=" + Names_Renamed[i];
			}
			for (int i = Arity; i < Length; i++)
			{
				assert(!Names_Renamed[i].Param) : "#" + i + "=" + Names_Renamed[i];
			}
			for (int i = Length; i < Names_Renamed.Length; i++)
			{
				assert(Names_Renamed[i] == null) : "#" + i + "=" + Names_Renamed[i];
			}
			// check resultName also
			if (ResultName != null)
			{
				int resultIndex = IndexOf(ResultName, Names_Renamed);
				assert(resultIndex >= 0) : "not found: " + ResultName.exprString() + Arrays.AsList(Names_Renamed);
				assert(Names_Renamed[resultIndex] == ResultName);
			}
			return true;
		}

		private bool VerifyFirstChange()
		{
			assert(InTrans());
			for (int i = 0; i < Length; i++)
			{
				if (Names_Renamed[i] != OriginalNames[i])
				{
					assert(FirstChange == i) : Arrays.asList(FirstChange, i, OriginalNames[i].exprString(), Arrays.AsList(Names_Renamed));
					return true;
				}
			}
			assert(FirstChange == Length) : Arrays.asList(FirstChange, Arrays.AsList(Names_Renamed));
			return true;
		}

		private static int IndexOf(NamedFunction fn, NamedFunction[] fns)
		{
			for (int i = 0; i < fns.Length; i++)
			{
				if (fns[i] == fn)
				{
					return i;
				}
			}
			return -1;
		}

		private static int IndexOf(Name n, Name[] ns)
		{
			for (int i = 0; i < ns.Length; i++)
			{
				if (ns[i] == n)
				{
					return i;
				}
			}
			return -1;
		}

		internal bool InTrans()
		{
			return (Flags & F_TRANS) != 0;
		}

		internal int OwnedCount()
		{
			return Flags & F_OWNED;
		}

		internal void GrowNames(int insertPos, int growLength)
		{
			int oldLength = Length;
			int newLength = oldLength + growLength;
			int oc = OwnedCount();
			if (oc == 0 || newLength > Names_Renamed.Length)
			{
				Names_Renamed = Arrays.CopyOf(Names_Renamed, (Names_Renamed.Length + growLength) * 5 / 4);
				if (oc == 0)
				{
					Flags++;
					oc++;
					assert(OwnedCount() == oc);
				}
			}
			if (OriginalNames != null && OriginalNames.Length < Names_Renamed.Length)
			{
				OriginalNames = Arrays.CopyOf(OriginalNames, Names_Renamed.Length);
				if (oc == 1)
				{
					Flags++;
					oc++;
					assert(OwnedCount() == oc);
				}
			}
			if (growLength == 0)
			{
				return;
			}
			int insertEnd = insertPos + growLength;
			int tailLength = oldLength - insertPos;
			System.Array.Copy(Names_Renamed, insertPos, Names_Renamed, insertEnd, tailLength);
			Arrays.Fill(Names_Renamed, insertPos, insertEnd, null);
			if (OriginalNames != null)
			{
				System.Array.Copy(OriginalNames, insertPos, OriginalNames, insertEnd, tailLength);
				Arrays.Fill(OriginalNames, insertPos, insertEnd, null);
			}
			Length = newLength;
			if (FirstChange >= insertPos)
			{
				FirstChange += growLength;
			}
		}

		internal int LastIndexOf(Name n)
		{
			int result = -1;
			for (int i = 0; i < Length; i++)
			{
				if (Names_Renamed[i] == n)
				{
					result = i;
				}
			}
			return result;
		}

		/// <summary>
		/// We have just overwritten the name at pos1 with the name at pos2.
		///  This means that there are two copies of the name, which we will have to fix later.
		/// </summary>
		private void NoteDuplicate(int pos1, int pos2)
		{
			Name n = Names_Renamed[pos1];
			assert(n == Names_Renamed[pos2]);
			assert(OriginalNames[pos1] != null); // something was replaced at pos1
			assert(OriginalNames[pos2] == null || OriginalNames[pos2] == n);
			if (Dups == null)
			{
				Dups = new List<>();
			}
			Dups.Add(n);
		}

		/// <summary>
		/// Replace duplicate names by nulls, and remove all nulls. </summary>
		private void ClearDuplicatesAndNulls()
		{
			if (Dups != null)
			{
				// Remove duplicates.
				assert(OwnedCount() >= 1);
				foreach (Name dup in Dups)
				{
					for (int i = FirstChange; i < Length; i++)
					{
						if (Names_Renamed[i] == dup && OriginalNames[i] != dup)
						{
							Names_Renamed[i] = null;
							assert(Arrays.AsList(Names_Renamed).Contains(dup));
							break; // kill only one dup
						}
					}
				}
				Dups.Clear();
			}
			// Now that we are done with originalNames, remove "killed" names.
			int oldLength = Length;
			for (int i = FirstChange; i < Length; i++)
			{
				if (Names_Renamed[i] == null)
				{
					System.Array.Copy(Names_Renamed, i + 1, Names_Renamed, i, (--Length - i));
					--i; // restart loop at this position
				}
			}
			if (Length < oldLength)
			{
				Arrays.Fill(Names_Renamed, Length, oldLength, null);
			}
			assert(!Arrays.AsList(Names_Renamed).SubList(0, Length).Contains(null));
		}

		/// <summary>
		/// Create a private, writable copy of names.
		///  Preserve the original copy, for reference.
		/// </summary>
		internal void StartEdit()
		{
			assert(VerifyArity());
			int oc = OwnedCount();
			assert(!InTrans()); // no nested transactions
			Flags |= (sbyte)F_TRANS;
			Name[] oldNames = Names_Renamed;
			Name[] ownBuffer = (oc == 2 ? OriginalNames : null);
			assert(ownBuffer != oldNames);
			if (ownBuffer != null && ownBuffer.Length >= Length)
			{
				Names_Renamed = CopyNamesInto(ownBuffer);
			}
			else
			{
				// make a new buffer to hold the names
				const int SLOP = 2;
				Names_Renamed = Arrays.CopyOf(oldNames, System.Math.Max(Length + SLOP, oldNames.Length));
				if (oc < 2)
				{
					++Flags;
				}
				assert(OwnedCount() == oc + 1);
			}
			OriginalNames = oldNames;
			assert(OriginalNames != Names_Renamed);
			FirstChange = Length;
			assert(InTrans());
		}

		private void ChangeName(int i, Name name)
		{
			assert(InTrans());
			assert(i < Length);
			Name oldName = Names_Renamed[i];
			assert(oldName == OriginalNames[i]); // no multiple changes
			assert(VerifyFirstChange());
			if (OwnedCount() == 0)
			{
				GrowNames(0, 0);
			}
			Names_Renamed[i] = name;
			if (FirstChange > i)
			{
				FirstChange = i;
			}
			if (ResultName != null && ResultName == oldName)
			{
				ResultName = name;
			}
		}

		/// <summary>
		/// Change the result name.  Null means a void result. </summary>
		internal Name Result
		{
			set
			{
				assert(value == null || LastIndexOf(value) >= 0);
				ResultName = value;
			}
		}

		/// <summary>
		/// Finish a transaction. </summary>
		internal LambdaForm EndEdit()
		{
			assert(VerifyFirstChange());
			// Assuming names have been changed pairwise from originalNames[i] to names[i],
			// update arguments to ensure referential integrity.
			for (int i = System.Math.Max(FirstChange, Arity); i < Length; i++)
			{
				Name name = Names_Renamed[i];
				if (name == null) // space for removed duplicate
				{
					continue;
				}
				Name newName = name.replaceNames(OriginalNames, Names_Renamed, FirstChange, i);
				if (newName != name)
				{
					Names_Renamed[i] = newName;
					if (ResultName == name)
					{
						ResultName = newName;
					}
				}
			}
			assert(InTrans());
			Flags &= (sbyte)(~F_TRANS);
			ClearDuplicatesAndNulls();
			OriginalNames = null;
			// If any parameters have been changed, then reorder them as needed.
			// This is a "sheep-and-goats" stable sort, pushing all non-parameters
			// to the right of all parameters.
			if (FirstChange < Arity)
			{
				Name[] exprs = new Name[Arity - FirstChange];
				int argp = FirstChange, exprp = 0;
				for (int i = FirstChange; i < Arity; i++)
				{
					Name name = Names_Renamed[i];
					if (name.Param)
					{
						Names_Renamed[argp++] = name;
					}
					else
					{
						exprs[exprp++] = name;
					}
				}
				assert(exprp == (Arity - argp));
				// copy the exprs just after the last remaining param
				System.Array.Copy(exprs, 0, Names_Renamed, argp, exprp);
				// adjust arity
				Arity -= exprp;
			}
			assert(VerifyArity());
			return LambdaForm();
		}

		private Name[] CopyNamesInto(Name[] buffer)
		{
			System.Array.Copy(Names_Renamed, 0, buffer, 0, Length);
			Arrays.Fill(buffer, Length, buffer.Length, null);
			return buffer;
		}

		/// <summary>
		/// Replace any Name whose function is in oldFns with a copy
		///  whose function is in the corresponding position in newFns.
		///  Only do this if the arguments are exactly equal to the given.
		/// </summary>
		internal LambdaFormBuffer ReplaceFunctions(NamedFunction[] oldFns, NamedFunction[] newFns, params Object[] forArguments)
		{
			assert(InTrans());
			if (oldFns.Length == 0)
			{
				return this;
			}
			for (int i = Arity; i < Length; i++)
			{
				Name n = Names_Renamed[i];
				int nfi = IndexOf(n.function, oldFns);
				if (nfi >= 0 && Arrays.Equals(n.arguments, forArguments))
				{
					ChangeName(i, new Name(newFns[nfi], n.arguments));
				}
			}
			return this;
		}

		private void ReplaceName(int pos, Name binding)
		{
			assert(InTrans());
			assert(VerifyArity());
			assert(pos < Arity);
			Name param = Names_Renamed[pos];
			assert(param.Param);
			assert(param.type == binding.type);
			ChangeName(pos, binding);
		}

		/// <summary>
		/// Replace a parameter by a fresh parameter. </summary>
		internal LambdaFormBuffer RenameParameter(int pos, Name newParam)
		{
			assert(newParam.Param);
			ReplaceName(pos, newParam);
			return this;
		}

		/// <summary>
		/// Replace a parameter by a fresh expression. </summary>
		internal LambdaFormBuffer ReplaceParameterByNewExpression(int pos, Name binding)
		{
			assert(!binding.Param);
			assert(LastIndexOf(binding) < 0); // else use replaceParameterByCopy
			ReplaceName(pos, binding);
			return this;
		}

		/// <summary>
		/// Replace a parameter by another parameter or expression already in the form. </summary>
		internal LambdaFormBuffer ReplaceParameterByCopy(int pos, int valuePos)
		{
			assert(pos != valuePos);
			ReplaceName(pos, Names_Renamed[valuePos]);
			NoteDuplicate(pos, valuePos); // temporarily, will occur twice in the names array
			return this;
		}

		private void InsertName(int pos, Name expr, bool isParameter)
		{
			assert(InTrans());
			assert(VerifyArity());
			assert(isParameter ? pos <= Arity : pos >= Arity);
			GrowNames(pos, 1);
			if (isParameter)
			{
				Arity += 1;
			}
			ChangeName(pos, expr);
		}

		/// <summary>
		/// Insert a fresh expression. </summary>
		internal LambdaFormBuffer InsertExpression(int pos, Name expr)
		{
			assert(!expr.Param);
			InsertName(pos, expr, false);
			return this;
		}

		/// <summary>
		/// Insert a fresh parameter. </summary>
		internal LambdaFormBuffer InsertParameter(int pos, Name param)
		{
			assert(param.Param);
			InsertName(pos, param, true);
			return this;
		}
	}

}
/*
 * Copyright (c) 2003, Oracle and/or its affiliates. All rights reserved.
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

namespace java.util
{

	/// <summary>
	/// Unchecked exception thrown when an illegal combination flags is given.
	/// 
	/// <para> Unless otherwise specified, passing a <tt>null</tt> argument to any
	/// method or constructor in this class will cause a {@link
	/// NullPointerException} to be thrown.
	/// 
	/// @since 1.5
	/// </para>
	/// </summary>
	public class IllegalFormatFlagsException : IllegalFormatException
	{

		private new const long SerialVersionUID = 790824L;

		private String Flags_Renamed;

		/// <summary>
		/// Constructs an instance of this class with the specified flags.
		/// </summary>
		/// <param name="f">
		///         The set of format flags which contain an illegal combination </param>
		public IllegalFormatFlagsException(String f)
		{
			if (f == null)
			{
				throw new NullPointerException();
			}
			this.Flags_Renamed = f;
		}

		/// <summary>
		/// Returns the set of flags which contains an illegal combination.
		/// </summary>
		/// <returns>  The flags </returns>
		public virtual String Flags
		{
			get
			{
				return Flags_Renamed;
			}
		}

		public override String Message
		{
			get
			{
				return "Flags = '" + Flags_Renamed + "'";
			}
		}
	}

}
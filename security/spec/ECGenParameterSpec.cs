/*
 * Copyright (c) 2003, 2013, Oracle and/or its affiliates. All rights reserved.
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
namespace java.security.spec
{

	/// <summary>
	/// This immutable class specifies the set of parameters used for
	/// generating elliptic curve (EC) domain parameters.
	/// </summary>
	/// <seealso cref= AlgorithmParameterSpec
	/// 
	/// @author Valerie Peng
	/// 
	/// @since 1.5 </seealso>
	public class ECGenParameterSpec : AlgorithmParameterSpec
	{

		private String Name_Renamed;

		/// <summary>
		/// Creates a parameter specification for EC parameter
		/// generation using a standard (or predefined) name
		/// {@code stdName} in order to generate the corresponding
		/// (precomputed) elliptic curve domain parameters. For the
		/// list of supported names, please consult the documentation
		/// of provider whose implementation will be used. </summary>
		/// <param name="stdName"> the standard name of the to-be-generated EC
		/// domain parameters. </param>
		/// <exception cref="NullPointerException"> if {@code stdName}
		/// is null. </exception>
		public ECGenParameterSpec(String stdName)
		{
			if (stdName == null)
			{
				throw new NullPointerException("stdName is null");
			}
			this.Name_Renamed = stdName;
		}

		/// <summary>
		/// Returns the standard or predefined name of the
		/// to-be-generated EC domain parameters. </summary>
		/// <returns> the standard or predefined name. </returns>
		public virtual String Name
		{
			get
			{
				return Name_Renamed;
			}
		}
	}

}
using System;

/*
 * Copyright (c) 1998, 2013, Oracle and/or its affiliates. All rights reserved.
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

	/// <summary>
	/// This class defines the <i>Service Provider Interface</i> (<b>SPI</b>)
	/// for the {@code SecureRandom} class.
	/// All the abstract methods in this class must be implemented by each
	/// service provider who wishes to supply the implementation
	/// of a cryptographically strong pseudo-random number generator.
	/// 
	/// </summary>
	/// <seealso cref= SecureRandom
	/// @since 1.2 </seealso>

	[Serializable]
	public abstract class SecureRandomSpi
	{

		private const long SerialVersionUID = -2991854161009191830L;

		/// <summary>
		/// Reseeds this random object. The given seed supplements, rather than
		/// replaces, the existing seed. Thus, repeated calls are guaranteed
		/// never to reduce randomness.
		/// </summary>
		/// <param name="seed"> the seed. </param>
		protected internal abstract void EngineSetSeed(sbyte[] seed);

		/// <summary>
		/// Generates a user-specified number of random bytes.
		/// 
		/// <para> If a call to {@code engineSetSeed} had not occurred previously,
		/// the first call to this method forces this SecureRandom implementation
		/// to seed itself.  This self-seeding will not occur if
		/// {@code engineSetSeed} was previously called.
		/// 
		/// </para>
		/// </summary>
		/// <param name="bytes"> the array to be filled in with random bytes. </param>
		protected internal abstract void EngineNextBytes(sbyte[] bytes);

		/// <summary>
		/// Returns the given number of seed bytes.  This call may be used to
		/// seed other random number generators.
		/// </summary>
		/// <param name="numBytes"> the number of seed bytes to generate.
		/// </param>
		/// <returns> the seed bytes. </returns>
		 protected internal abstract sbyte[] EngineGenerateSeed(int numBytes);
	}

}
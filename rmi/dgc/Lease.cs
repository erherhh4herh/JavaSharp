using System;

/*
 * Copyright (c) 1996, 1998, Oracle and/or its affiliates. All rights reserved.
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
namespace java.rmi.dgc
{

	/// <summary>
	/// A lease contains a unique VM identifier and a lease duration. A
	/// Lease object is used to request and grant leases to remote object
	/// references.
	/// </summary>
	[Serializable]
	public sealed class Lease
	{

		/// <summary>
		/// @serial Virtual Machine ID with which this Lease is associated. </summary>
		/// <seealso cref= #getVMID </seealso>
		private VMID Vmid;

		/// <summary>
		/// @serial Duration of this lease. </summary>
		/// <seealso cref= #getValue </seealso>
		private long Value_Renamed;
		/// <summary>
		/// indicate compatibility with JDK 1.1.x version of class </summary>
		private const long SerialVersionUID = -5713411624328831948L;

		/// <summary>
		/// Constructs a lease with a specific VMID and lease duration. The
		/// vmid may be null. </summary>
		/// <param name="id"> VMID associated with this lease </param>
		/// <param name="duration"> lease duration </param>
		public Lease(VMID id, long duration)
		{
			Vmid = id;
			Value_Renamed = duration;
		}

		/// <summary>
		/// Returns the client VMID associated with the lease. </summary>
		/// <returns> client VMID </returns>
		public VMID VMID
		{
			get
			{
				return Vmid;
			}
		}

		/// <summary>
		/// Returns the lease duration. </summary>
		/// <returns> lease duration </returns>
		public long Value
		{
			get
			{
				return Value_Renamed;
			}
		}
	}

}
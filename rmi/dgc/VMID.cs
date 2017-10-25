using System;

/*
 * Copyright (c) 1996, 2013, Oracle and/or its affiliates. All rights reserved.
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
	/// A VMID is a identifier that is unique across all Java virtual
	/// machines.  VMIDs are used by the distributed garbage collector
	/// to identify client VMs.
	/// 
	/// @author      Ann Wollrath
	/// @author      Peter Jones
	/// </summary>
	[Serializable]
	public sealed class VMID
	{
		/// <summary>
		/// Array of bytes uniquely identifying this host </summary>
		private static readonly sbyte[] RandomBytes;

		/// <summary>
		/// @serial array of bytes uniquely identifying host created on
		/// </summary>
		private sbyte[] Addr;

		/// <summary>
		/// @serial unique identifier with respect to host created on
		/// </summary>
		private UID Uid;

		/// <summary>
		/// indicate compatibility with JDK 1.1.x version of class </summary>
		private const long SerialVersionUID = -538642295484486218L;

		static VMID()
		{
			// Generate 8 bytes of random data.
			SecureRandom secureRandom = new SecureRandom();
			sbyte[] bytes = new sbyte[8];
			secureRandom.NextBytes(bytes);
			RandomBytes = bytes;
		}

		/// <summary>
		/// Create a new VMID.  Each new VMID returned from this constructor
		/// is unique for all Java virtual machines under the following
		/// conditions: a) the conditions for uniqueness for objects of
		/// the class <code>java.rmi.server.UID</code> are satisfied, and b) an
		/// address can be obtained for this host that is unique and constant
		/// for the lifetime of this object.
		/// </summary>
		public VMID()
		{
			Addr = RandomBytes;
			Uid = new UID();
		}

		/// <summary>
		/// Return true if an accurate address can be determined for this
		/// host.  If false, reliable VMID cannot be generated from this host </summary>
		/// <returns> true if host address can be determined, false otherwise
		/// @deprecated </returns>
		[Obsolete]
		public static bool Unique
		{
			get
			{
				return true;
			}
		}

		/// <summary>
		/// Compute hash code for this VMID.
		/// </summary>
		public override int HashCode()
		{
			return Uid.HashCode();
		}

		/// <summary>
		/// Compare this VMID to another, and return true if they are the
		/// same identifier.
		/// </summary>
		public override bool Equals(Object obj)
		{
			if (obj is VMID)
			{
				VMID vmid = (VMID) obj;
				if (!Uid.Equals(vmid.Uid))
				{
					return false;
				}
				if ((Addr == null) ^ (vmid.Addr == null))
				{
					return false;
				}
				if (Addr != null)
				{
					if (Addr.Length != vmid.Addr.Length)
					{
						return false;
					}
					for (int i = 0; i < Addr.Length; ++i)
					{
						if (Addr[i] != vmid.Addr[i])
						{
							return false;
						}
					}
				}
				return true;
			}
			else
			{
				return false;
			}
		}

		/// <summary>
		/// Return string representation of this VMID.
		/// </summary>
		public override String ToString()
		{
			StringBuffer result = new StringBuffer();
			if (Addr != null)
			{
				for (int i = 0; i < Addr.Length; ++i)
				{
					int x = Addr[i] & 0xFF;
					result.Append((x < 0x10 ? "0" : "") + Convert.ToString(x, 16));
				}
			}
			result.Append(':');
			result.Append(Uid.ToString());
			return result.ToString();
		}
	}

}
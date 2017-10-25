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

/*
 * (C) Copyright Taligent, Inc. 1996 - 1997, All Rights Reserved
 * (C) Copyright IBM Corp. 1996 - 1998, All Rights Reserved
 *
 * The original version of this source code and documentation is
 * copyrighted and owned by Taligent, Inc., a wholly-owned subsidiary
 * of IBM. These materials are provided under terms of a License
 * Agreement between Taligent and Sun. This technology is protected
 * by multiple US and International patents.
 *
 * This notice and attribution to Taligent may not be removed.
 * Taligent is a registered trademark of Taligent, Inc.
 *
 */

namespace java.awt.font
{


	/// <summary>
	/// The <code>TransformAttribute</code> class provides an immutable
	/// wrapper for a transform so that it is safe to use as an attribute.
	/// </summary>
	[Serializable]
	public sealed class TransformAttribute
	{

		/// <summary>
		/// The <code>AffineTransform</code> for this
		/// <code>TransformAttribute</code>, or <code>null</code>
		/// if <code>AffineTransform</code> is the identity transform.
		/// </summary>
		private AffineTransform Transform_Renamed;

		/// <summary>
		/// Wraps the specified transform.  The transform is cloned and a
		/// reference to the clone is kept.  The original transform is unchanged.
		/// If null is passed as the argument, this constructor behaves as though
		/// it were the identity transform.  (Note that it is preferable to use
		/// <seealso cref="#IDENTITY"/> in this case.) </summary>
		/// <param name="transform"> the specified <seealso cref="AffineTransform"/> to be wrapped,
		/// or null. </param>
		public TransformAttribute(AffineTransform transform)
		{
			if (transform != null && !transform.Identity)
			{
				this.Transform_Renamed = new AffineTransform(transform);
			}
		}

		/// <summary>
		/// Returns a copy of the wrapped transform. </summary>
		/// <returns> a <code>AffineTransform</code> that is a copy of the wrapped
		/// transform of this <code>TransformAttribute</code>. </returns>
		public AffineTransform Transform
		{
			get
			{
				AffineTransform at = Transform_Renamed;
				return (at == null) ? new AffineTransform() : new AffineTransform(at);
			}
		}

		/// <summary>
		/// Returns <code>true</code> if the wrapped transform is
		/// an identity transform. </summary>
		/// <returns> <code>true</code> if the wrapped transform is
		/// an identity transform; <code>false</code> otherwise.
		/// @since 1.4 </returns>
		public bool Identity
		{
			get
			{
				return Transform_Renamed == null;
			}
		}

		/// <summary>
		/// A <code>TransformAttribute</code> representing the identity transform.
		/// @since 1.6
		/// </summary>
		public static readonly TransformAttribute IDENTITY = new TransformAttribute(null);

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: private void writeObject(java.io.ObjectOutputStream s) throws java.lang.ClassNotFoundException, java.io.IOException
		private void WriteObject(java.io.ObjectOutputStream s)
		{
			// sigh -- 1.3 expects transform is never null, so we need to always write one out
			if (this.Transform_Renamed == null)
			{
				this.Transform_Renamed = new AffineTransform();
			}
			s.DefaultWriteObject();
		}

		/*
		 * @since 1.6
		 */
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: private Object readResolve() throws java.io.ObjectStreamException
		private Object ReadResolve()
		{
			if (Transform_Renamed == null || Transform_Renamed.Identity)
			{
				return IDENTITY;
			}
			return this;
		}

		// Added for serial backwards compatibility (4348425)
		internal const long SerialVersionUID = 3356247357827709530L;

		/// <summary>
		/// @since 1.6
		/// </summary>
		public override int HashCode()
		{
			return Transform_Renamed == null ? 0 : Transform_Renamed.HashCode();
		}

		/// <summary>
		/// Returns <code>true</code> if rhs is a <code>TransformAttribute</code>
		/// whose transform is equal to this <code>TransformAttribute</code>'s
		/// transform. </summary>
		/// <param name="rhs"> the object to compare to </param>
		/// <returns> <code>true</code> if the argument is a <code>TransformAttribute</code>
		/// whose transform is equal to this <code>TransformAttribute</code>'s
		/// transform.
		/// @since 1.6 </returns>
		public override bool Equals(Object rhs)
		{
			if (rhs != null)
			{
				try
				{
					TransformAttribute that = (TransformAttribute)rhs;
					if (Transform_Renamed == null)
					{
						return that.Transform_Renamed == null;
					}
					return Transform_Renamed.Equals(that.Transform_Renamed);
				}
				catch (ClassCastException)
				{
				}
			}
			return false;
		}
	}

}
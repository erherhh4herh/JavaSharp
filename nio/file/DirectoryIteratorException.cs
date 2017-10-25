/*
 * Copyright (c) 2010, 2011, Oracle and/or its affiliates. All rights reserved.
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

namespace java.nio.file
{


	/// <summary>
	/// Runtime exception thrown if an I/O error is encountered when iterating over
	/// the entries in a directory. The I/O error is retrieved as an {@link
	/// IOException} using the <seealso cref="#getCause() getCause()"/> method.
	/// 
	/// @since 1.7 </summary>
	/// <seealso cref= DirectoryStream </seealso>

	public sealed class DirectoryIteratorException : ConcurrentModificationException
	{
		private new const long SerialVersionUID = -6012699886086212874L;

		/// <summary>
		/// Constructs an instance of this class.
		/// </summary>
		/// <param name="cause">
		///          the {@code IOException} that caused the directory iteration
		///          to fail
		/// </param>
		/// <exception cref="NullPointerException">
		///          if the cause is {@code null} </exception>
		public DirectoryIteratorException(IOException cause) : base(Objects.RequireNonNull(cause))
		{
		}

		/// <summary>
		/// Returns the cause of this exception.
		/// </summary>
		/// <returns>  the cause </returns>
		public override IOException Cause
		{
			get
			{
				return (IOException)base.InnerException;
			}
		}

		/// <summary>
		/// Called to read the object from a stream.
		/// </summary>
		/// <exception cref="InvalidObjectException">
		///          if the object is invalid or has a cause that is not
		///          an {@code IOException} </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: private void readObject(java.io.ObjectInputStream s) throws java.io.IOException, ClassNotFoundException
		private void ReadObject(ObjectInputStream s)
		{
			s.DefaultReadObject();
			Throwable cause = base.InnerException;
			if (!(cause is IOException))
			{
				throw new InvalidObjectException("Cause must be an IOException");
			}
		}
	}

}
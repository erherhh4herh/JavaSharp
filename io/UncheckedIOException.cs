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
namespace java.io
{

	/// <summary>
	/// Wraps an <seealso cref="IOException"/> with an unchecked exception.
	/// 
	/// @since   1.8
	/// </summary>
	public class UncheckedIOException : RuntimeException
	{
		private new const long SerialVersionUID = -8134305061645241065L;

		/// <summary>
		/// Constructs an instance of this class.
		/// </summary>
		/// <param name="message">
		///          the detail message, can be null </param>
		/// <param name="cause">
		///          the {@code IOException}
		/// </param>
		/// <exception cref="NullPointerException">
		///          if the cause is {@code null} </exception>
		public UncheckedIOException(String message, IOException cause) : base(message, Objects.RequireNonNull(cause))
		{
		}

		/// <summary>
		/// Constructs an instance of this class.
		/// </summary>
		/// <param name="cause">
		///          the {@code IOException}
		/// </param>
		/// <exception cref="NullPointerException">
		///          if the cause is {@code null} </exception>
		public UncheckedIOException(IOException cause) : base(Objects.RequireNonNull(cause))
		{
		}

		/// <summary>
		/// Returns the cause of this exception.
		/// </summary>
		/// <returns>  the {@code IOException} which is the cause of this exception. </returns>
		public override IOException Cause
		{
			get
			{
				return (IOException) base.InnerException;
			}
		}

		/// <summary>
		/// Called to read the object from a stream.
		/// </summary>
		/// <exception cref="InvalidObjectException">
		///          if the object is invalid or has a cause that is not
		///          an {@code IOException} </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: private void readObject(ObjectInputStream s) throws IOException, ClassNotFoundException
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
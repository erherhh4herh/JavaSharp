/*
 * Copyright (c) 2007, 2009, Oracle and/or its affiliates. All rights reserved.
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
	/// Thrown when a file system operation fails on one or two files. This class is
	/// the general class for file system exceptions.
	/// 
	/// @since 1.7
	/// </summary>

	public class FileSystemException : IOException
	{
		internal new const long SerialVersionUID = -3055425747967319812L;

		private readonly String File_Renamed;
		private readonly String Other;

		/// <summary>
		/// Constructs an instance of this class. This constructor should be used
		/// when an operation involving one file fails and there isn't any additional
		/// information to explain the reason.
		/// </summary>
		/// <param name="file">
		///          a string identifying the file or {@code null} if not known. </param>
		public FileSystemException(String file) : base((String)null)
		{
			this.File_Renamed = file;
			this.Other = null;
		}

		/// <summary>
		/// Constructs an instance of this class. This constructor should be used
		/// when an operation involving two files fails, or there is additional
		/// information to explain the reason.
		/// </summary>
		/// <param name="file">
		///          a string identifying the file or {@code null} if not known. </param>
		/// <param name="other">
		///          a string identifying the other file or {@code null} if there
		///          isn't another file or if not known </param>
		/// <param name="reason">
		///          a reason message with additional information or {@code null} </param>
		public FileSystemException(String file, String other, String reason) : base(reason)
		{
			this.File_Renamed = file;
			this.Other = other;
		}

		/// <summary>
		/// Returns the file used to create this exception.
		/// </summary>
		/// <returns>  the file (can be {@code null}) </returns>
		public virtual String File
		{
			get
			{
				return File_Renamed;
			}
		}

		/// <summary>
		/// Returns the other file used to create this exception.
		/// </summary>
		/// <returns>  the other file (can be {@code null}) </returns>
		public virtual String OtherFile
		{
			get
			{
				return Other;
			}
		}

		/// <summary>
		/// Returns the string explaining why the file system operation failed.
		/// </summary>
		/// <returns>  the string explaining why the file system operation failed </returns>
		public virtual String Reason
		{
			get
			{
				return base.Message;
			}
		}

		/// <summary>
		/// Returns the detail message string.
		/// </summary>
		public override String Message
		{
			get
			{
				if (File_Renamed == null && Other == null)
				{
					return Reason;
				}
				StringBuilder sb = new StringBuilder();
				if (File_Renamed != null)
				{
					sb.Append(File_Renamed);
				}
				if (Other != null)
				{
					sb.Append(" -> ");
					sb.Append(Other);
				}
				if (Reason != null)
				{
					sb.Append(": ");
					sb.Append(Reason);
				}
				return sb.ToString();
			}
		}
	}

}
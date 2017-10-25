/*
 * Copyright (c) 2001, 2013, Oracle and/or its affiliates. All rights reserved.
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

namespace java.nio.charset
{


	/// <summary>
	/// A typesafe enumeration for coding-error actions.
	/// 
	/// <para> Instances of this class are used to specify how malformed-input and
	/// unmappable-character errors are to be handled by charset <a
	/// href="CharsetDecoder.html#cae">decoders</a> and <a
	/// href="CharsetEncoder.html#cae">encoders</a>.  </para>
	/// 
	/// 
	/// @author Mark Reinhold
	/// @author JSR-51 Expert Group
	/// @since 1.4
	/// </summary>

	public class CodingErrorAction
	{

		private String Name;

		private CodingErrorAction(String name)
		{
			this.Name = name;
		}

		/// <summary>
		/// Action indicating that a coding error is to be handled by dropping the
		/// erroneous input and resuming the coding operation.
		/// </summary>
		public static readonly CodingErrorAction IGNORE = new CodingErrorAction("IGNORE");

		/// <summary>
		/// Action indicating that a coding error is to be handled by dropping the
		/// erroneous input, appending the coder's replacement value to the output
		/// buffer, and resuming the coding operation.
		/// </summary>
		public static readonly CodingErrorAction REPLACE = new CodingErrorAction("REPLACE");

		/// <summary>
		/// Action indicating that a coding error is to be reported, either by
		/// returning a <seealso cref="CoderResult"/> object or by throwing a {@link
		/// CharacterCodingException}, whichever is appropriate for the method
		/// implementing the coding process.
		/// </summary>
		public static readonly CodingErrorAction REPORT = new CodingErrorAction("REPORT");

		/// <summary>
		/// Returns a string describing this action.
		/// </summary>
		/// <returns>  A descriptive string </returns>
		public override String ToString()
		{
			return Name;
		}

	}

}
/*
 * Copyright (c) 2004, 2010, Oracle and/or its affiliates. All rights reserved.
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
	/// FomattableFlags are passed to the {@link Formattable#formatTo
	/// Formattable.formatTo()} method and modify the output format for {@linkplain
	/// Formattable Formattables}.  Implementations of <seealso cref="Formattable"/> are
	/// responsible for interpreting and validating any flags.
	/// 
	/// @since  1.5
	/// </summary>
	public class FormattableFlags
	{

		// Explicit instantiation of this class is prohibited.
		private FormattableFlags()
		{
		}

		/// <summary>
		/// Left-justifies the output.  Spaces (<tt>'&#92;u0020'</tt>) will be added
		/// at the end of the converted value as required to fill the minimum width
		/// of the field.  If this flag is not set then the output will be
		/// right-justified.
		/// 
		/// <para> This flag corresponds to <tt>'-'</tt> (<tt>'&#92;u002d'</tt>) in
		/// the format specifier.
		/// </para>
		/// </summary>
		public static readonly int LEFT_JUSTIFY = 1 << 0; // '-'

		/// <summary>
		/// Converts the output to upper case according to the rules of the
		/// <seealso cref="java.util.Locale locale"/> given during creation of the
		/// <tt>formatter</tt> argument of the {@link Formattable#formatTo
		/// formatTo()} method.  The output should be equivalent the following
		/// invocation of <seealso cref="String#toUpperCase(java.util.Locale)"/>
		/// 
		/// <pre>
		///     out.toUpperCase() </pre>
		/// 
		/// <para> This flag corresponds to <tt>'S'</tt> (<tt>'&#92;u0053'</tt>) in
		/// the format specifier.
		/// </para>
		/// </summary>
		public static readonly int UPPERCASE = 1 << 1; // 'S'

		/// <summary>
		/// Requires the output to use an alternate form.  The definition of the
		/// form is specified by the <tt>Formattable</tt>.
		/// 
		/// <para> This flag corresponds to <tt>'#'</tt> (<tt>'&#92;u0023'</tt>) in
		/// the format specifier.
		/// </para>
		/// </summary>
		public static readonly int ALTERNATE = 1 << 2; // '#'
	}

}
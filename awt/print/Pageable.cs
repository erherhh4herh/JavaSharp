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

namespace java.awt.print
{

	/// <summary>
	/// The <code>Pageable</code> implementation represents a set of
	/// pages to be printed. The <code>Pageable</code> object returns
	/// the total number of pages in the set as well as the
	/// <seealso cref="PageFormat"/> and <seealso cref="Printable"/> for a specified page. </summary>
	/// <seealso cref= java.awt.print.PageFormat </seealso>
	/// <seealso cref= java.awt.print.Printable </seealso>
	public interface Pageable
	{

		/// <summary>
		/// This constant is returned from the
		/// <seealso cref="#getNumberOfPages() getNumberOfPages"/>
		/// method if a <code>Pageable</code> implementation does not know
		/// the number of pages in its set.
		/// </summary>

		/// <summary>
		/// Returns the number of pages in the set.
		/// To enable advanced printing features,
		/// it is recommended that <code>Pageable</code>
		/// implementations return the true number of pages
		/// rather than the
		/// UNKNOWN_NUMBER_OF_PAGES constant. </summary>
		/// <returns> the number of pages in this <code>Pageable</code>. </returns>
		int NumberOfPages {get;}

		/// <summary>
		/// Returns the <code>PageFormat</code> of the page specified by
		/// <code>pageIndex</code>. </summary>
		/// <param name="pageIndex"> the zero based index of the page whose
		///            <code>PageFormat</code> is being requested </param>
		/// <returns> the <code>PageFormat</code> describing the size and
		///          orientation. </returns>
		/// <exception cref="IndexOutOfBoundsException"> if
		///          the <code>Pageable</code> does not contain the requested
		///          page. </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: PageFormat getPageFormat(int pageIndex) throws IndexOutOfBoundsException;
		PageFormat GetPageFormat(int pageIndex);

		/// <summary>
		/// Returns the <code>Printable</code> instance responsible for
		/// rendering the page specified by <code>pageIndex</code>. </summary>
		/// <param name="pageIndex"> the zero based index of the page whose
		///            <code>Printable</code> is being requested </param>
		/// <returns> the <code>Printable</code> that renders the page. </returns>
		/// <exception cref="IndexOutOfBoundsException"> if
		///            the <code>Pageable</code> does not contain the requested
		///            page. </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: Printable getPrintable(int pageIndex) throws IndexOutOfBoundsException;
		Printable GetPrintable(int pageIndex);
	}

	public static class Pageable_Fields
	{
		public const  Native;
	}

}
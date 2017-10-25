using System.Collections;

/*
 * Copyright (c) 1997, 2013, Oracle and/or its affiliates. All rights reserved.
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
	/// The <code>Book</code> class provides a representation of a document in
	/// which pages may have different page formats and page painters. This
	/// class uses the <seealso cref="Pageable"/> interface to interact with a
	/// <seealso cref="PrinterJob"/>. </summary>
	/// <seealso cref= Pageable </seealso>
	/// <seealso cref= PrinterJob </seealso>

	public class Book : Pageable
	{

	 /* Class Constants */

	 /* Class Variables */

	 /* Instance Variables */

		/// <summary>
		/// The set of pages that make up the Book.
		/// </summary>
		private ArrayList MPages;

	 /* Instance Methods */

		/// <summary>
		///  Creates a new, empty <code>Book</code>.
		/// </summary>
		public Book()
		{
			MPages = new ArrayList();
		}

		/// <summary>
		/// Returns the number of pages in this <code>Book</code>. </summary>
		/// <returns> the number of pages this <code>Book</code> contains. </returns>
		public virtual int NumberOfPages
		{
			get
			{
				return MPages.Count;
			}
		}

		/// <summary>
		/// Returns the <seealso cref="PageFormat"/> of the page specified by
		/// <code>pageIndex</code>. </summary>
		/// <param name="pageIndex"> the zero based index of the page whose
		///            <code>PageFormat</code> is being requested </param>
		/// <returns> the <code>PageFormat</code> describing the size and
		///          orientation of the page. </returns>
		/// <exception cref="IndexOutOfBoundsException"> if the <code>Pageable</code>
		///          does not contain the requested page </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public PageFormat getPageFormat(int pageIndex) throws IndexOutOfBoundsException
		public virtual PageFormat GetPageFormat(int pageIndex)
		{
			return GetPage(pageIndex).PageFormat;
		}

		/// <summary>
		/// Returns the <seealso cref="Printable"/> instance responsible for rendering
		/// the page specified by <code>pageIndex</code>. </summary>
		/// <param name="pageIndex"> the zero based index of the page whose
		///                  <code>Printable</code> is being requested </param>
		/// <returns> the <code>Printable</code> that renders the page. </returns>
		/// <exception cref="IndexOutOfBoundsException"> if the <code>Pageable</code>
		///            does not contain the requested page </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public Printable getPrintable(int pageIndex) throws IndexOutOfBoundsException
		public virtual Printable GetPrintable(int pageIndex)
		{
			return GetPage(pageIndex).Printable;
		}

		/// <summary>
		/// Sets the <code>PageFormat</code> and the <code>Painter</code> for a
		/// specified page number. </summary>
		/// <param name="pageIndex"> the zero based index of the page whose
		///                  painter and format is altered </param>
		/// <param name="painter">   the <code>Printable</code> instance that
		///                  renders the page </param>
		/// <param name="page">      the size and orientation of the page </param>
		/// <exception cref="IndexOutOfBoundsException"> if the specified
		///          page is not already in this <code>Book</code> </exception>
		/// <exception cref="NullPointerException"> if the <code>painter</code> or
		///          <code>page</code> argument is <code>null</code> </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public void setPage(int pageIndex, Printable painter, PageFormat page) throws IndexOutOfBoundsException
		public virtual void SetPage(int pageIndex, Printable painter, PageFormat page)
		{
			if (painter == null)
			{
				throw new NullPointerException("painter is null");
			}

			if (page == null)
			{
				throw new NullPointerException("page is null");
			}

			MPages[pageIndex] = new BookPage(this, painter, page);
		}

		/// <summary>
		/// Appends a single page to the end of this <code>Book</code>. </summary>
		/// <param name="painter">   the <code>Printable</code> instance that
		///                  renders the page </param>
		/// <param name="page">      the size and orientation of the page </param>
		/// <exception cref="NullPointerException">
		///          If the <code>painter</code> or <code>page</code>
		///          argument is <code>null</code> </exception>
		public virtual void Append(Printable painter, PageFormat page)
		{
			MPages.Add(new BookPage(this, painter, page));
		}

		/// <summary>
		/// Appends <code>numPages</code> pages to the end of this
		/// <code>Book</code>.  Each of the pages is associated with
		/// <code>page</code>. </summary>
		/// <param name="painter">   the <code>Printable</code> instance that renders
		///                  the page </param>
		/// <param name="page">      the size and orientation of the page </param>
		/// <param name="numPages">  the number of pages to be added to the
		///                  this <code>Book</code>. </param>
		/// <exception cref="NullPointerException">
		///          If the <code>painter</code> or <code>page</code>
		///          argument is <code>null</code> </exception>
		public virtual void Append(Printable painter, PageFormat page, int numPages)
		{
			BookPage bookPage = new BookPage(this, painter, page);
			int pageIndex = MPages.Count;
			int newSize = pageIndex + numPages;

			MPages.Capacity = newSize;
			for (int i = pageIndex; i < newSize; i++)
			{
				MPages[i] = bookPage;
			}
		}

		/// <summary>
		/// Return the BookPage for the page specified by 'pageIndex'.
		/// </summary>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: private BookPage getPage(int pageIndex) throws ArrayIndexOutOfBoundsException
		private BookPage GetPage(int pageIndex)
		{
			return (BookPage) MPages[pageIndex];
		}

		/// <summary>
		/// The BookPage inner class describes an individual
		/// page in a Book through a PageFormat-Printable pair.
		/// </summary>
		private class BookPage
		{
			private readonly Book OuterInstance;

			/// <summary>
			///  The size and orientation of the page.
			/// </summary>
			internal PageFormat MFormat;

			/// <summary>
			/// The instance that will draw the page.
			/// </summary>
			internal Printable MPainter;

			/// <summary>
			/// A new instance where 'format' describes the page's
			/// size and orientation and 'painter' is the instance
			/// that will draw the page's graphics. </summary>
			/// <exception cref="NullPointerException">
			///          If the <code>painter</code> or <code>format</code>
			///          argument is <code>null</code> </exception>
			internal BookPage(Book outerInstance, Printable painter, PageFormat format)
			{
				this.OuterInstance = outerInstance;

				if (painter == null || format == null)
				{
					throw new NullPointerException();
				}

				MFormat = format;
				MPainter = painter;
			}

			/// <summary>
			/// Return the instance that paints the
			/// page.
			/// </summary>
			internal virtual Printable Printable
			{
				get
				{
					return MPainter;
				}
			}

			/// <summary>
			/// Return the format of the page.
			/// </summary>
			internal virtual PageFormat PageFormat
			{
				get
				{
					return MFormat;
				}
			}
		}
	}

}
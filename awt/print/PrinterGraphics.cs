/*
 * Copyright (c) 1998, 1999, Oracle and/or its affiliates. All rights reserved.
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
	/// The <code>PrinterGraphics</code> interface is implemented by
	/// <seealso cref="java.awt.Graphics"/> objects that are passed to
	/// <seealso cref="Printable"/> objects to render a page. It allows an
	/// application to find the <seealso cref="PrinterJob"/> object that is
	/// controlling the printing.
	/// </summary>

	public interface PrinterGraphics
	{

		/// <summary>
		/// Returns the <code>PrinterJob</code> that is controlling the
		/// current rendering request. </summary>
		/// <returns> the <code>PrinterJob</code> controlling the current
		/// rendering request. </returns>
		/// <seealso cref= java.awt.print.Printable </seealso>
		PrinterJob PrinterJob {get;}

	}

}
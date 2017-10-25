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



	using GetPropertyAction = sun.security.action.GetPropertyAction;

	/// <summary>
	/// The <code>PrinterJob</code> class is the principal class that controls
	/// printing. An application calls methods in this class to set up a job,
	/// optionally to invoke a print dialog with the user, and then to print
	/// the pages of the job.
	/// </summary>
	public abstract class PrinterJob
	{

	 /* Public Class Methods */

		/// <summary>
		/// Creates and returns a <code>PrinterJob</code> which is initially
		/// associated with the default printer.
		/// If no printers are available on the system, a PrinterJob will still
		/// be returned from this method, but <code>getPrintService()</code>
		/// will return <code>null</code>, and calling
		/// <seealso cref="#print() print"/> with this <code>PrinterJob</code> might
		/// generate an exception.  Applications that need to determine if
		/// there are suitable printers before creating a <code>PrinterJob</code>
		/// should ensure that the array returned from
		/// <seealso cref="#lookupPrintServices() lookupPrintServices"/> is not empty. </summary>
		/// <returns> a new <code>PrinterJob</code>.
		/// </returns>
		/// <exception cref="SecurityException"> if a security manager exists and its
		///          <seealso cref="java.lang.SecurityManager#checkPrintJobAccess"/>
		///          method disallows this thread from creating a print job request </exception>
		public static PrinterJob PrinterJob
		{
			get
			{
				SecurityManager security = System.SecurityManager;
				if (security != null)
				{
					security.CheckPrintJobAccess();
				}
				return (PrinterJob) java.security.AccessController.doPrivileged(new PrivilegedActionAnonymousInnerClassHelper());
			}
		}

		private class PrivilegedActionAnonymousInnerClassHelper : java.security.PrivilegedAction
		{
			public PrivilegedActionAnonymousInnerClassHelper()
			{
			}

			public virtual Object Run()
			{
				String nm = System.getProperty("java.awt.printerjob", null);
				try
				{
					return (PrinterJob)Class.ForName(nm).NewInstance();
				}
				catch (ClassNotFoundException)
				{
					throw new AWTError("PrinterJob not found: " + nm);
				}
				catch (InstantiationException)
				{
				 throw new AWTError("Could not instantiate PrinterJob: " + nm);
				}
				catch (IllegalAccessException)
				{
					throw new AWTError("Could not access PrinterJob: " + nm);
				}
			}
		}

		/// <summary>
		/// A convenience method which looks up 2D print services.
		/// Services returned from this method may be installed on
		/// <code>PrinterJob</code>s which support print services.
		/// Calling this method is equivalent to calling
		/// {@link javax.print.PrintServiceLookup#lookupPrintServices(
		/// DocFlavor, AttributeSet)
		/// PrintServiceLookup.lookupPrintServices()}
		/// and specifying a Pageable DocFlavor. </summary>
		/// <returns> a possibly empty array of 2D print services.
		/// @since     1.4 </returns>
		public static PrintService[] LookupPrintServices()
		{
			return PrintServiceLookup.lookupPrintServices(DocFlavor.SERVICE_FORMATTED.PAGEABLE, null);
		}


		/// <summary>
		/// A convenience method which locates factories for stream print
		/// services which can image 2D graphics.
		/// Sample usage :
		/// <pre>{@code
		/// FileOutputStream outstream;
		/// StreamPrintService psPrinter;
		/// String psMimeType = "application/postscript";
		/// PrinterJob pj = PrinterJob.getPrinterJob();
		/// 
		/// StreamPrintServiceFactory[] factories =
		///     PrinterJob.lookupStreamPrintServices(psMimeType);
		/// if (factories.length > 0) {
		///     try {
		///         outstream = new File("out.ps");
		///         psPrinter =  factories[0].getPrintService(outstream);
		///         // psPrinter can now be set as the service on a PrinterJob
		///         pj.setPrintService(psPrinter)
		///     } catch (Exception e) {
		///         e.printStackTrace();
		///     }
		/// }
		/// }</pre>
		/// Services returned from this method may be installed on
		/// <code>PrinterJob</code> instances which support print services.
		/// Calling this method is equivalent to calling
		/// {@link javax.print.StreamPrintServiceFactory#lookupStreamPrintServiceFactories(DocFlavor, String)
		/// StreamPrintServiceFactory.lookupStreamPrintServiceFactories()
		/// } and specifying a Pageable DocFlavor.
		/// </summary>
		/// <param name="mimeType"> the required output format, or null to mean any format. </param>
		/// <returns> a possibly empty array of 2D stream print service factories.
		/// @since     1.4 </returns>
		public static StreamPrintServiceFactory[] LookupStreamPrintServices(String mimeType)
		{
			return StreamPrintServiceFactory.lookupStreamPrintServiceFactories(DocFlavor.SERVICE_FORMATTED.PAGEABLE, mimeType);
		}


	 /* Public Methods */

		/// <summary>
		/// A <code>PrinterJob</code> object should be created using the
		/// static <seealso cref="#getPrinterJob() getPrinterJob"/> method.
		/// </summary>
		public PrinterJob()
		{
		}

		/// <summary>
		/// Returns the service (printer) for this printer job.
		/// Implementations of this class which do not support print services
		/// may return null.  null will also be returned if no printers are
		/// available. </summary>
		/// <returns> the service for this printer job. </returns>
		/// <seealso cref= #setPrintService(PrintService) </seealso>
		/// <seealso cref= #getPrinterJob()
		/// @since     1.4 </seealso>
		public virtual PrintService PrintService
		{
			get
			{
				return null;
			}
			set
			{
					throw new PrinterException("Setting a service is not supported on this class");
			}
		}


		/// <summary>
		/// Calls <code>painter</code> to render the pages.  The pages in the
		/// document to be printed by this
		/// <code>PrinterJob</code> are rendered by the <seealso cref="Printable"/>
		/// object, <code>painter</code>.  The <seealso cref="PageFormat"/> for each page
		/// is the default page format. </summary>
		/// <param name="painter"> the <code>Printable</code> that renders each page of
		/// the document. </param>
		public abstract Printable Printable {set;}

		/// <summary>
		/// Calls <code>painter</code> to render the pages in the specified
		/// <code>format</code>.  The pages in the document to be printed by
		/// this <code>PrinterJob</code> are rendered by the
		/// <code>Printable</code> object, <code>painter</code>. The
		/// <code>PageFormat</code> of each page is <code>format</code>. </summary>
		/// <param name="painter"> the <code>Printable</code> called to render
		///          each page of the document </param>
		/// <param name="format"> the size and orientation of each page to
		///                   be printed </param>
		public abstract void SetPrintable(Printable painter, PageFormat format);

		/// <summary>
		/// Queries <code>document</code> for the number of pages and
		/// the <code>PageFormat</code> and <code>Printable</code> for each
		/// page held in the <code>Pageable</code> instance,
		/// <code>document</code>. </summary>
		/// <param name="document"> the pages to be printed. It can not be
		/// <code>null</code>. </param>
		/// <exception cref="NullPointerException"> the <code>Pageable</code> passed in
		/// was <code>null</code>. </exception>
		/// <seealso cref= PageFormat </seealso>
		/// <seealso cref= Printable </seealso>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public abstract void setPageable(Pageable document) throws NullPointerException;
		public abstract Pageable Pageable {set;}

		/// <summary>
		/// Presents a dialog to the user for changing the properties of
		/// the print job.
		/// This method will display a native dialog if a native print
		/// service is selected, and user choice of printers will be restricted
		/// to these native print services.
		/// To present the cross platform print dialog for all services,
		/// including native ones instead use
		/// <code>printDialog(PrintRequestAttributeSet)</code>.
		/// <para>
		/// PrinterJob implementations which can use PrintService's will update
		/// the PrintService for this PrinterJob to reflect the new service
		/// selected by the user.
		/// </para>
		/// </summary>
		/// <returns> <code>true</code> if the user does not cancel the dialog;
		/// <code>false</code> otherwise. </returns>
		/// <exception cref="HeadlessException"> if GraphicsEnvironment.isHeadless()
		/// returns true. </exception>
		/// <seealso cref= java.awt.GraphicsEnvironment#isHeadless </seealso>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public abstract boolean printDialog() throws java.awt.HeadlessException;
		public abstract bool PrintDialog();

		/// <summary>
		/// A convenience method which displays a cross-platform print dialog
		/// for all services which are capable of printing 2D graphics using the
		/// <code>Pageable</code> interface. The selected printer when the
		/// dialog is initially displayed will reflect the print service currently
		/// attached to this print job.
		/// If the user changes the print service, the PrinterJob will be
		/// updated to reflect this, unless the user cancels the dialog.
		/// As well as allowing the user to select the destination printer,
		/// the user can also select values of various print request attributes.
		/// <para>
		/// The attributes parameter on input will reflect the applications
		/// required initial selections in the user dialog. Attributes not
		/// specified display using the default for the service. On return it
		/// will reflect the user's choices. Selections may be updated by
		/// the implementation to be consistent with the supported values
		/// for the currently selected print service.
		/// </para>
		/// <para>
		/// As the user scrolls to a new print service selection, the values
		/// copied are based on the settings for the previous service, together
		/// with any user changes. The values are not based on the original
		/// settings supplied by the client.
		/// </para>
		/// <para>
		/// With the exception of selected printer, the PrinterJob state is
		/// not updated to reflect the user's changes.
		/// For the selections to affect a printer job, the attributes must
		/// be specified in the call to the
		/// <code>print(PrintRequestAttributeSet)</code> method. If using
		/// the Pageable interface, clients which intend to use media selected
		/// by the user must create a PageFormat derived from the user's
		/// selections.
		/// If the user cancels the dialog, the attributes will not reflect
		/// any changes made by the user.
		/// </para>
		/// </summary>
		/// <param name="attributes"> on input is application supplied attributes,
		/// on output the contents are updated to reflect user choices.
		/// This parameter may not be null. </param>
		/// <returns> <code>true</code> if the user does not cancel the dialog;
		/// <code>false</code> otherwise. </returns>
		/// <exception cref="HeadlessException"> if GraphicsEnvironment.isHeadless()
		/// returns true. </exception>
		/// <exception cref="NullPointerException"> if <code>attributes</code> parameter
		/// is null. </exception>
		/// <seealso cref= java.awt.GraphicsEnvironment#isHeadless
		/// @since     1.4
		///  </seealso>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public boolean printDialog(javax.print.attribute.PrintRequestAttributeSet attributes) throws java.awt.HeadlessException
		public virtual bool PrintDialog(PrintRequestAttributeSet attributes)
		{

			if (attributes == null)
			{
				throw new NullPointerException("attributes");
			}
			return PrintDialog();
		}

		/// <summary>
		/// Displays a dialog that allows modification of a
		/// <code>PageFormat</code> instance.
		/// The <code>page</code> argument is used to initialize controls
		/// in the page setup dialog.
		/// If the user cancels the dialog then this method returns the
		/// original <code>page</code> object unmodified.
		/// If the user okays the dialog then this method returns a new
		/// <code>PageFormat</code> object with the indicated changes.
		/// In either case, the original <code>page</code> object is
		/// not modified. </summary>
		/// <param name="page"> the default <code>PageFormat</code> presented to the
		///                  user for modification </param>
		/// <returns>    the original <code>page</code> object if the dialog
		///            is cancelled; a new <code>PageFormat</code> object
		///            containing the format indicated by the user if the
		///            dialog is acknowledged. </returns>
		/// <exception cref="HeadlessException"> if GraphicsEnvironment.isHeadless()
		/// returns true. </exception>
		/// <seealso cref= java.awt.GraphicsEnvironment#isHeadless
		/// @since     1.2 </seealso>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public abstract PageFormat pageDialog(PageFormat page) throws java.awt.HeadlessException;
		public abstract PageFormat PageDialog(PageFormat page);

		/// <summary>
		/// A convenience method which displays a cross-platform page setup dialog.
		/// The choices available will reflect the print service currently
		/// set on this PrinterJob.
		/// <para>
		/// The attributes parameter on input will reflect the client's
		/// required initial selections in the user dialog. Attributes which are
		/// not specified display using the default for the service. On return it
		/// will reflect the user's choices. Selections may be updated by
		/// the implementation to be consistent with the supported values
		/// for the currently selected print service.
		/// </para>
		/// <para>
		/// The return value will be a PageFormat equivalent to the
		/// selections in the PrintRequestAttributeSet.
		/// If the user cancels the dialog, the attributes will not reflect
		/// any changes made by the user, and the return value will be null.
		/// </para>
		/// </summary>
		/// <param name="attributes"> on input is application supplied attributes,
		/// on output the contents are updated to reflect user choices.
		/// This parameter may not be null. </param>
		/// <returns> a page format if the user does not cancel the dialog;
		/// <code>null</code> otherwise. </returns>
		/// <exception cref="HeadlessException"> if GraphicsEnvironment.isHeadless()
		/// returns true. </exception>
		/// <exception cref="NullPointerException"> if <code>attributes</code> parameter
		/// is null. </exception>
		/// <seealso cref= java.awt.GraphicsEnvironment#isHeadless
		/// @since     1.4
		///  </seealso>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public PageFormat pageDialog(javax.print.attribute.PrintRequestAttributeSet attributes) throws java.awt.HeadlessException
		public virtual PageFormat PageDialog(PrintRequestAttributeSet attributes)
		{

			if (attributes == null)
			{
				throw new NullPointerException("attributes");
			}
			return PageDialog(DefaultPage());
		}

		/// <summary>
		/// Clones the <code>PageFormat</code> argument and alters the
		/// clone to describe a default page size and orientation. </summary>
		/// <param name="page"> the <code>PageFormat</code> to be cloned and altered </param>
		/// <returns> clone of <code>page</code>, altered to describe a default
		///                      <code>PageFormat</code>. </returns>
		public abstract PageFormat DefaultPage(PageFormat page);

		/// <summary>
		/// Creates a new <code>PageFormat</code> instance and
		/// sets it to a default size and orientation. </summary>
		/// <returns> a <code>PageFormat</code> set to a default size and
		///          orientation. </returns>
		public virtual PageFormat DefaultPage()
		{
			return DefaultPage(new PageFormat());
		}

		/// <summary>
		/// Calculates a <code>PageFormat</code> with values consistent with those
		/// supported by the current <code>PrintService</code> for this job
		/// (ie the value returned by <code>getPrintService()</code>) and media,
		/// printable area and orientation contained in <code>attributes</code>.
		/// <para>
		/// Calling this method does not update the job.
		/// It is useful for clients that have a set of attributes obtained from
		/// <code>printDialog(PrintRequestAttributeSet attributes)</code>
		/// and need a PageFormat to print a Pageable object.
		/// </para>
		/// </summary>
		/// <param name="attributes"> a set of printing attributes, for example obtained
		/// from calling printDialog. If <code>attributes</code> is null a default
		/// PageFormat is returned. </param>
		/// <returns> a <code>PageFormat</code> whose settings conform with
		/// those of the current service and the specified attributes.
		/// @since 1.6 </returns>
		public virtual PageFormat GetPageFormat(PrintRequestAttributeSet attributes)
		{

			PrintService service = PrintService;
			PageFormat pf = DefaultPage();

			if (service == null || attributes == null)
			{
				return pf;
			}

			Media media = (Media)attributes.get(typeof(Media));
			MediaPrintableArea mpa = (MediaPrintableArea)attributes.get(typeof(MediaPrintableArea));
			OrientationRequested orientReq = (OrientationRequested)attributes.get(typeof(OrientationRequested));

			if (media == null && mpa == null && orientReq == null)
			{
			   return pf;
			}
			Paper paper = pf.Paper;

			/* If there's a media but no media printable area, we can try
			 * to retrieve the default value for mpa and use that.
			 */
			if (mpa == null && media != null && service.isAttributeCategorySupported(typeof(MediaPrintableArea)))
			{
				Object mpaVals = service.getSupportedAttributeValues(typeof(MediaPrintableArea), null, attributes);
				if (mpaVals is MediaPrintableArea[] && ((MediaPrintableArea[])mpaVals).Length > 0)
				{
					mpa = ((MediaPrintableArea[])mpaVals)[0];
				}
			}

			if (media != null && service.isAttributeValueSupported(media, null, attributes))
			{
				if (media is MediaSizeName)
				{
					MediaSizeName msn = (MediaSizeName)media;
					MediaSize msz = MediaSize.getMediaSizeForName(msn);
					if (msz != null)
					{
						double inch = 72.0;
						double paperWid = msz.getX(MediaSize.INCH) * inch;
						double paperHgt = msz.getY(MediaSize.INCH) * inch;
						paper.SetSize(paperWid, paperHgt);
						if (mpa == null)
						{
							paper.SetImageableArea(inch, inch, paperWid - 2 * inch, paperHgt - 2 * inch);
						}
					}
				}
			}

			if (mpa != null && service.isAttributeValueSupported(mpa, null, attributes))
			{
				float[] printableArea = mpa.getPrintableArea(MediaPrintableArea.INCH);
				for (int i = 0; i < printableArea.Length; i++)
				{
					printableArea[i] = printableArea[i] * 72.0f;
				}
				paper.SetImageableArea(printableArea[0], printableArea[1], printableArea[2], printableArea[3]);
			}

			if (orientReq != null && service.isAttributeValueSupported(orientReq, null, attributes))
			{
				int orient;
				if (orientReq.Equals(OrientationRequested.REVERSE_LANDSCAPE))
				{
					orient = PageFormat.REVERSE_LANDSCAPE;
				}
				else if (orientReq.Equals(OrientationRequested.LANDSCAPE))
				{
					orient = PageFormat.LANDSCAPE;
				}
				else
				{
					orient = PageFormat.PORTRAIT;
				}
				pf.Orientation = orient;
			}

			pf.Paper = paper;
			pf = ValidatePage(pf);
			return pf;
		}

		/// <summary>
		/// Returns the clone of <code>page</code> with its settings
		/// adjusted to be compatible with the current printer of this
		/// <code>PrinterJob</code>.  For example, the returned
		/// <code>PageFormat</code> could have its imageable area
		/// adjusted to fit within the physical area of the paper that
		/// is used by the current printer. </summary>
		/// <param name="page"> the <code>PageFormat</code> that is cloned and
		///          whose settings are changed to be compatible with
		///          the current printer </param>
		/// <returns> a <code>PageFormat</code> that is cloned from
		///          <code>page</code> and whose settings are changed
		///          to conform with this <code>PrinterJob</code>. </returns>
		public abstract PageFormat ValidatePage(PageFormat page);

		/// <summary>
		/// Prints a set of pages. </summary>
		/// <exception cref="PrinterException"> an error in the print system
		///            caused the job to be aborted. </exception>
		/// <seealso cref= Book </seealso>
		/// <seealso cref= Pageable </seealso>
		/// <seealso cref= Printable </seealso>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public abstract void print() throws PrinterException;
		public abstract void Print();

	   /// <summary>
	   /// Prints a set of pages using the settings in the attribute
	   /// set. The default implementation ignores the attribute set.
	   /// <para>
	   /// Note that some attributes may be set directly on the PrinterJob
	   /// by equivalent method calls, (for example), copies:
	   /// <code>setcopies(int)</code>, job name: <code>setJobName(String)</code>
	   /// and specifying media size and orientation though the
	   /// <code>PageFormat</code> object.
	   /// </para>
	   /// <para>
	   /// If a supported attribute-value is specified in this attribute set,
	   /// it will take precedence over the API settings for this print()
	   /// operation only.
	   /// The following behaviour is specified for PageFormat:
	   /// If a client uses the Printable interface, then the
	   /// <code>attributes</code> parameter to this method is examined
	   /// for attributes which specify media (by size), orientation, and
	   /// imageable area, and those are used to construct a new PageFormat
	   /// which is passed to the Printable object's print() method.
	   /// See <seealso cref="Printable"/> for an explanation of the required
	   /// behaviour of a Printable to ensure optimal printing via PrinterJob.
	   /// For clients of the Pageable interface, the PageFormat will always
	   /// be as supplied by that interface, on a per page basis.
	   /// </para>
	   /// <para>
	   /// These behaviours allow an application to directly pass the
	   /// user settings returned from
	   /// <code>printDialog(PrintRequestAttributeSet attributes</code> to
	   /// this print() method.
	   /// </para>
	   /// <para>
	   ///  
	   /// </para>
	   /// </summary>
	   /// <param name="attributes"> a set of attributes for the job </param>
	   /// <exception cref="PrinterException"> an error in the print system
	   ///            caused the job to be aborted. </exception>
	   /// <seealso cref= Book </seealso>
	   /// <seealso cref= Pageable </seealso>
	   /// <seealso cref= Printable
	   /// @since 1.4 </seealso>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public void print(javax.print.attribute.PrintRequestAttributeSet attributes) throws PrinterException
		public virtual void Print(PrintRequestAttributeSet attributes)
		{
			Print();
		}

		/// <summary>
		/// Sets the number of copies to be printed. </summary>
		/// <param name="copies"> the number of copies to be printed </param>
		/// <seealso cref= #getCopies </seealso>
		public abstract int Copies {set;get;}


		/// <summary>
		/// Gets the name of the printing user. </summary>
		/// <returns> the name of the printing user </returns>
		public abstract String UserName {get;}

		/// <summary>
		/// Sets the name of the document to be printed.
		/// The document name can not be <code>null</code>. </summary>
		/// <param name="jobName"> the name of the document to be printed </param>
		/// <seealso cref= #getJobName </seealso>
		public abstract String JobName {set;get;}


		/// <summary>
		/// Cancels a print job that is in progress.  If
		/// <seealso cref="#print() print"/> has been called but has not
		/// returned then this method signals
		/// that the job should be cancelled at the next
		/// chance. If there is no print job in progress then
		/// this call does nothing.
		/// </summary>
		public abstract void Cancel();

		/// <summary>
		/// Returns <code>true</code> if a print job is
		/// in progress, but is going to be cancelled
		/// at the next opportunity; otherwise returns
		/// <code>false</code>. </summary>
		/// <returns> <code>true</code> if the job in progress
		/// is going to be cancelled; <code>false</code> otherwise. </returns>
		public abstract bool Cancelled {get;}

	}

}
/*
 * Copyright (c) 1999, 2011, Oracle and/or its affiliates. All rights reserved.
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

namespace java.awt
{

	/// <summary>
	/// A set of attributes which control a print job.
	/// <para>
	/// Instances of this class control the number of copies, default selection,
	/// destination, print dialog, file and printer names, page ranges, multiple
	/// document handling (including collation), and multi-page imposition (such
	/// as duplex) of every print job which uses the instance. Attribute names are
	/// compliant with the Internet Printing Protocol (IPP) 1.1 where possible.
	/// Attribute values are partially compliant where possible.
	/// </para>
	/// <para>
	/// To use a method which takes an inner class type, pass a reference to
	/// one of the constant fields of the inner class. Client code cannot create
	/// new instances of the inner class types because none of those classes
	/// has a public constructor. For example, to set the print dialog type to
	/// the cross-platform, pure Java print dialog, use the following code:
	/// <pre>
	/// import java.awt.JobAttributes;
	/// 
	/// public class PureJavaPrintDialogExample {
	///     public void setPureJavaPrintDialog(JobAttributes jobAttributes) {
	///         jobAttributes.setDialog(JobAttributes.DialogType.COMMON);
	///     }
	/// }
	/// </pre>
	/// </para>
	/// <para>
	/// Every IPP attribute which supports an <i>attributeName</i>-default value
	/// has a corresponding <code>set<i>attributeName</i>ToDefault</code> method.
	/// Default value fields are not provided.
	/// 
	/// @author      David Mendenhall
	/// @since 1.3
	/// </para>
	/// </summary>
	public sealed class JobAttributes : Cloneable
	{
		/// <summary>
		/// A type-safe enumeration of possible default selection states.
		/// @since 1.3
		/// </summary>
		public sealed class DefaultSelectionType : AttributeValue
		{
			internal const int I_ALL = 0;
			internal const int I_RANGE = 1;
			internal const int I_SELECTION = 2;

			internal static readonly String[] NAMES = new String[] {"all", "range", "selection"};

			/// <summary>
			/// The <code>DefaultSelectionType</code> instance to use for
			/// specifying that all pages of the job should be printed.
			/// </summary>
			public static readonly DefaultSelectionType ALL = new DefaultSelectionType(I_ALL);
			/// <summary>
			/// The <code>DefaultSelectionType</code> instance to use for
			/// specifying that a range of pages of the job should be printed.
			/// </summary>
			public static readonly DefaultSelectionType RANGE = new DefaultSelectionType(I_RANGE);
			/// <summary>
			/// The <code>DefaultSelectionType</code> instance to use for
			/// specifying that the current selection should be printed.
			/// </summary>
			public static readonly DefaultSelectionType SELECTION = new DefaultSelectionType(I_SELECTION);

			internal DefaultSelectionType(int type) : base(type, NAMES)
			{
			}
		}

		/// <summary>
		/// A type-safe enumeration of possible job destinations.
		/// @since 1.3
		/// </summary>
		public sealed class DestinationType : AttributeValue
		{
			internal const int I_FILE = 0;
			internal const int I_PRINTER = 1;

			internal static readonly String[] NAMES = new String[] {"file", "printer"};

			/// <summary>
			/// The <code>DestinationType</code> instance to use for
			/// specifying print to file.
			/// </summary>
			public static readonly DestinationType FILE = new DestinationType(I_FILE);
			/// <summary>
			/// The <code>DestinationType</code> instance to use for
			/// specifying print to printer.
			/// </summary>
			public static readonly DestinationType PRINTER = new DestinationType(I_PRINTER);

			internal DestinationType(int type) : base(type, NAMES)
			{
			}
		}

		/// <summary>
		/// A type-safe enumeration of possible dialogs to display to the user.
		/// @since 1.3
		/// </summary>
		public sealed class DialogType : AttributeValue
		{
			internal const int I_COMMON = 0;
			internal const int I_NATIVE = 1;
			internal const int I_NONE = 2;

			internal static readonly String[] NAMES = new String[] {"common", "native", "none"};

			/// <summary>
			/// The <code>DialogType</code> instance to use for
			/// specifying the cross-platform, pure Java print dialog.
			/// </summary>
			public static readonly DialogType COMMON = new DialogType(I_COMMON);
			/// <summary>
			/// The <code>DialogType</code> instance to use for
			/// specifying the platform's native print dialog.
			/// </summary>
			public static readonly DialogType NATIVE = new DialogType(I_NATIVE);
			/// <summary>
			/// The <code>DialogType</code> instance to use for
			/// specifying no print dialog.
			/// </summary>
			public static readonly DialogType NONE = new DialogType(I_NONE);

			internal DialogType(int type) : base(type, NAMES)
			{
			}
		}

		/// <summary>
		/// A type-safe enumeration of possible multiple copy handling states.
		/// It is used to control how the sheets of multiple copies of a single
		/// document are collated.
		/// @since 1.3
		/// </summary>
		public sealed class MultipleDocumentHandlingType : AttributeValue
		{
			internal const int I_SEPARATE_DOCUMENTS_COLLATED_COPIES = 0;
			internal const int I_SEPARATE_DOCUMENTS_UNCOLLATED_COPIES = 1;

			internal static readonly String[] NAMES = new String[] {"separate-documents-collated-copies", "separate-documents-uncollated-copies"};

			/// <summary>
			/// The <code>MultipleDocumentHandlingType</code> instance to use for specifying
			/// that the job should be divided into separate, collated copies.
			/// </summary>
			public static readonly MultipleDocumentHandlingType SEPARATE_DOCUMENTS_COLLATED_COPIES = new MultipleDocumentHandlingType(I_SEPARATE_DOCUMENTS_COLLATED_COPIES);
			/// <summary>
			/// The <code>MultipleDocumentHandlingType</code> instance to use for specifying
			/// that the job should be divided into separate, uncollated copies.
			/// </summary>
			public static readonly MultipleDocumentHandlingType SEPARATE_DOCUMENTS_UNCOLLATED_COPIES = new MultipleDocumentHandlingType(I_SEPARATE_DOCUMENTS_UNCOLLATED_COPIES);

			internal MultipleDocumentHandlingType(int type) : base(type, NAMES)
			{
			}
		}

		/// <summary>
		/// A type-safe enumeration of possible multi-page impositions. These
		/// impositions are in compliance with IPP 1.1.
		/// @since 1.3
		/// </summary>
		public sealed class SidesType : AttributeValue
		{
			internal const int I_ONE_SIDED = 0;
			internal const int I_TWO_SIDED_LONG_EDGE = 1;
			internal const int I_TWO_SIDED_SHORT_EDGE = 2;

			internal static readonly String[] NAMES = new String[] {"one-sided", "two-sided-long-edge", "two-sided-short-edge"};

			/// <summary>
			/// The <code>SidesType</code> instance to use for specifying that
			/// consecutive job pages should be printed upon the same side of
			/// consecutive media sheets.
			/// </summary>
			public static readonly SidesType ONE_SIDED = new SidesType(I_ONE_SIDED);
			/// <summary>
			/// The <code>SidesType</code> instance to use for specifying that
			/// consecutive job pages should be printed upon front and back sides
			/// of consecutive media sheets, such that the orientation of each pair
			/// of pages on the medium would be correct for the reader as if for
			/// binding on the long edge.
			/// </summary>
			public static readonly SidesType TWO_SIDED_LONG_EDGE = new SidesType(I_TWO_SIDED_LONG_EDGE);
			/// <summary>
			/// The <code>SidesType</code> instance to use for specifying that
			/// consecutive job pages should be printed upon front and back sides
			/// of consecutive media sheets, such that the orientation of each pair
			/// of pages on the medium would be correct for the reader as if for
			/// binding on the short edge.
			/// </summary>
			public static readonly SidesType TWO_SIDED_SHORT_EDGE = new SidesType(I_TWO_SIDED_SHORT_EDGE);

			internal SidesType(int type) : base(type, NAMES)
			{
			}
		}

		private int Copies_Renamed;
		private DefaultSelectionType DefaultSelection_Renamed;
		private DestinationType Destination_Renamed;
		private DialogType Dialog_Renamed;
		private String FileName_Renamed;
		private int FromPage_Renamed;
		private int MaxPage_Renamed;
		private int MinPage_Renamed;
		private MultipleDocumentHandlingType MultipleDocumentHandling_Renamed;
		private int[][] PageRanges_Renamed;
		private int PrFirst;
		private int PrLast;
		private String Printer_Renamed;
		private SidesType Sides_Renamed;
		private int ToPage_Renamed;

		/// <summary>
		/// Constructs a <code>JobAttributes</code> instance with default
		/// values for every attribute.  The dialog defaults to
		/// <code>DialogType.NATIVE</code>.  Min page defaults to
		/// <code>1</code>.  Max page defaults to <code>Integer.MAX_VALUE</code>.
		/// Destination defaults to <code>DestinationType.PRINTER</code>.
		/// Selection defaults to <code>DefaultSelectionType.ALL</code>.
		/// Number of copies defaults to <code>1</code>. Multiple document handling defaults
		/// to <code>MultipleDocumentHandlingType.SEPARATE_DOCUMENTS_UNCOLLATED_COPIES</code>.
		/// Sides defaults to <code>SidesType.ONE_SIDED</code>. File name defaults
		/// to <code>null</code>.
		/// </summary>
		public JobAttributes()
		{
			SetCopiesToDefault();
			DefaultSelection = DefaultSelectionType.ALL;
			Destination = DestinationType.PRINTER;
			Dialog = DialogType.NATIVE;
			MaxPage = Integer.MaxValue;
			MinPage = 1;
			SetMultipleDocumentHandlingToDefault();
			SetSidesToDefault();
		}

		/// <summary>
		/// Constructs a <code>JobAttributes</code> instance which is a copy
		/// of the supplied <code>JobAttributes</code>.
		/// </summary>
		/// <param name="obj"> the <code>JobAttributes</code> to copy </param>
		public JobAttributes(JobAttributes obj)
		{
			Set(obj);
		}

		/// <summary>
		/// Constructs a <code>JobAttributes</code> instance with the
		/// specified values for every attribute.
		/// </summary>
		/// <param name="copies"> an integer greater than 0 </param>
		/// <param name="defaultSelection"> <code>DefaultSelectionType.ALL</code>,
		///          <code>DefaultSelectionType.RANGE</code>, or
		///          <code>DefaultSelectionType.SELECTION</code> </param>
		/// <param name="destination"> <code>DesintationType.FILE</code> or
		///          <code>DesintationType.PRINTER</code> </param>
		/// <param name="dialog"> <code>DialogType.COMMON</code>,
		///          <code>DialogType.NATIVE</code>, or
		///          <code>DialogType.NONE</code> </param>
		/// <param name="fileName"> the possibly <code>null</code> file name </param>
		/// <param name="maxPage"> an integer greater than zero and greater than or equal
		///          to <i>minPage</i> </param>
		/// <param name="minPage"> an integer greater than zero and less than or equal
		///          to <i>maxPage</i> </param>
		/// <param name="multipleDocumentHandling">
		///     <code>MultipleDocumentHandlingType.SEPARATE_DOCUMENTS_COLLATED_COPIES</code> or
		///     <code>MultipleDocumentHandlingType.SEPARATE_DOCUMENTS_UNCOLLATED_COPIES</code> </param>
		/// <param name="pageRanges"> an array of integer arrays of two elements; an array
		///          is interpreted as a range spanning all pages including and
		///          between the specified pages; ranges must be in ascending
		///          order and must not overlap; specified page numbers cannot be
		///          less than <i>minPage</i> nor greater than <i>maxPage</i>;
		///          for example:
		///          <pre>
		///          (new int[][] { new int[] { 1, 3 }, new int[] { 5, 5 },
		///                         new int[] { 15, 19 } }),
		///          </pre>
		///          specifies pages 1, 2, 3, 5, 15, 16, 17, 18, and 19. Note that
		///          (<code>new int[][] { new int[] { 1, 1 }, new int[] { 1, 2 } }</code>),
		///          is an invalid set of page ranges because the two ranges
		///          overlap </param>
		/// <param name="printer"> the possibly <code>null</code> printer name </param>
		/// <param name="sides"> <code>SidesType.ONE_SIDED</code>,
		///          <code>SidesType.TWO_SIDED_LONG_EDGE</code>, or
		///          <code>SidesType.TWO_SIDED_SHORT_EDGE</code> </param>
		/// <exception cref="IllegalArgumentException"> if one or more of the above
		///          conditions is violated </exception>
		public JobAttributes(int copies, DefaultSelectionType defaultSelection, DestinationType destination, DialogType dialog, String fileName, int maxPage, int minPage, MultipleDocumentHandlingType multipleDocumentHandling, int[][] pageRanges, String printer, SidesType sides)
		{
			Copies = copies;
			DefaultSelection = defaultSelection;
			Destination = destination;
			Dialog = dialog;
			FileName = fileName;
			MaxPage = maxPage;
			MinPage = minPage;
			MultipleDocumentHandling = multipleDocumentHandling;
			PageRanges = pageRanges;
			Printer = printer;
			Sides = sides;
		}

		/// <summary>
		/// Creates and returns a copy of this <code>JobAttributes</code>.
		/// </summary>
		/// <returns>  the newly created copy; it is safe to cast this Object into
		///          a <code>JobAttributes</code> </returns>
		public Object Clone()
		{
			try
			{
				return base.Clone();
			}
			catch (CloneNotSupportedException e)
			{
				// Since we implement Cloneable, this should never happen
				throw new InternalError(e);
			}
		}

		/// <summary>
		/// Sets all of the attributes of this <code>JobAttributes</code> to
		/// the same values as the attributes of obj.
		/// </summary>
		/// <param name="obj"> the <code>JobAttributes</code> to copy </param>
		public void Set(JobAttributes obj)
		{
			Copies_Renamed = obj.Copies_Renamed;
			DefaultSelection_Renamed = obj.DefaultSelection_Renamed;
			Destination_Renamed = obj.Destination_Renamed;
			Dialog_Renamed = obj.Dialog_Renamed;
			FileName_Renamed = obj.FileName_Renamed;
			FromPage_Renamed = obj.FromPage_Renamed;
			MaxPage_Renamed = obj.MaxPage_Renamed;
			MinPage_Renamed = obj.MinPage_Renamed;
			MultipleDocumentHandling_Renamed = obj.MultipleDocumentHandling_Renamed;
			// okay because we never modify the contents of pageRanges
			PageRanges_Renamed = obj.PageRanges_Renamed;
			PrFirst = obj.PrFirst;
			PrLast = obj.PrLast;
			Printer_Renamed = obj.Printer_Renamed;
			Sides_Renamed = obj.Sides_Renamed;
			ToPage_Renamed = obj.ToPage_Renamed;
		}

		/// <summary>
		/// Returns the number of copies the application should render for jobs
		/// using these attributes. This attribute is updated to the value chosen
		/// by the user.
		/// </summary>
		/// <returns>  an integer greater than 0. </returns>
		public int Copies
		{
			get
			{
				return Copies_Renamed;
			}
			set
			{
				if (value <= 0)
				{
					throw new IllegalArgumentException("Invalid value for attribute " + "copies");
				}
				this.Copies_Renamed = value;
			}
		}


		/// <summary>
		/// Sets the number of copies the application should render for jobs using
		/// these attributes to the default. The default number of copies is 1.
		/// </summary>
		public void SetCopiesToDefault()
		{
			Copies = 1;
		}

		/// <summary>
		/// Specifies whether, for jobs using these attributes, the application
		/// should print all pages, the range specified by the return value of
		/// <code>getPageRanges</code>, or the current selection. This attribute
		/// is updated to the value chosen by the user.
		/// </summary>
		/// <returns>  DefaultSelectionType.ALL, DefaultSelectionType.RANGE, or
		///          DefaultSelectionType.SELECTION </returns>
		public DefaultSelectionType DefaultSelection
		{
			get
			{
				return DefaultSelection_Renamed;
			}
			set
			{
				if (value == null)
				{
					throw new IllegalArgumentException("Invalid value for attribute " + "defaultSelection");
				}
				this.DefaultSelection_Renamed = value;
			}
		}


		/// <summary>
		/// Specifies whether output will be to a printer or a file for jobs using
		/// these attributes. This attribute is updated to the value chosen by the
		/// user.
		/// </summary>
		/// <returns>  DesintationType.FILE or DesintationType.PRINTER </returns>
		public DestinationType Destination
		{
			get
			{
				return Destination_Renamed;
			}
			set
			{
				if (value == null)
				{
					throw new IllegalArgumentException("Invalid value for attribute " + "destination");
				}
				this.Destination_Renamed = value;
			}
		}


		/// <summary>
		/// Returns whether, for jobs using these attributes, the user should see
		/// a print dialog in which to modify the print settings, and which type of
		/// print dialog should be displayed. DialogType.COMMON denotes a cross-
		/// platform, pure Java print dialog. DialogType.NATIVE denotes the
		/// platform's native print dialog. If a platform does not support a native
		/// print dialog, the pure Java print dialog is displayed instead.
		/// DialogType.NONE specifies no print dialog (i.e., background printing).
		/// This attribute cannot be modified by, and is not subject to any
		/// limitations of, the implementation or the target printer.
		/// </summary>
		/// <returns>  <code>DialogType.COMMON</code>, <code>DialogType.NATIVE</code>, or
		///          <code>DialogType.NONE</code> </returns>
		public DialogType Dialog
		{
			get
			{
				return Dialog_Renamed;
			}
			set
			{
				if (value == null)
				{
					throw new IllegalArgumentException("Invalid value for attribute " + "dialog");
				}
				this.Dialog_Renamed = value;
			}
		}


		/// <summary>
		/// Specifies the file name for the output file for jobs using these
		/// attributes. This attribute is updated to the value chosen by the user.
		/// </summary>
		/// <returns>  the possibly <code>null</code> file name </returns>
		public String FileName
		{
			get
			{
				return FileName_Renamed;
			}
			set
			{
				this.FileName_Renamed = value;
			}
		}


		/// <summary>
		/// Returns, for jobs using these attributes, the first page to be
		/// printed, if a range of pages is to be printed. This attribute is
		/// updated to the value chosen by the user. An application should ignore
		/// this attribute on output, unless the return value of the <code>
		/// getDefaultSelection</code> method is DefaultSelectionType.RANGE. An
		/// application should honor the return value of <code>getPageRanges</code>
		/// over the return value of this method, if possible.
		/// </summary>
		/// <returns>  an integer greater than zero and less than or equal to
		///          <i>toPage</i> and greater than or equal to <i>minPage</i> and
		///          less than or equal to <i>maxPage</i>. </returns>
		public int FromPage
		{
			get
			{
				if (FromPage_Renamed != 0)
				{
					return FromPage_Renamed;
				}
				else if (ToPage_Renamed != 0)
				{
					return MinPage;
				}
				else if (PageRanges_Renamed != null)
				{
					return PrFirst;
				}
				else
				{
					return MinPage;
				}
			}
			set
			{
				if (value <= 0 || (ToPage_Renamed != 0 && value > ToPage_Renamed) || value < MinPage_Renamed || value > MaxPage_Renamed)
				{
					throw new IllegalArgumentException("Invalid value for attribute " + "fromPage");
				}
				this.FromPage_Renamed = value;
			}
		}


		/// <summary>
		/// Specifies the maximum value the user can specify as the last page to
		/// be printed for jobs using these attributes. This attribute cannot be
		/// modified by, and is not subject to any limitations of, the
		/// implementation or the target printer.
		/// </summary>
		/// <returns>  an integer greater than zero and greater than or equal
		///          to <i>minPage</i>. </returns>
		public int MaxPage
		{
			get
			{
				return MaxPage_Renamed;
			}
			set
			{
				if (value <= 0 || value < MinPage_Renamed)
				{
					throw new IllegalArgumentException("Invalid value for attribute " + "maxPage");
				}
				this.MaxPage_Renamed = value;
			}
		}


		/// <summary>
		/// Specifies the minimum value the user can specify as the first page to
		/// be printed for jobs using these attributes. This attribute cannot be
		/// modified by, and is not subject to any limitations of, the
		/// implementation or the target printer.
		/// </summary>
		/// <returns>  an integer greater than zero and less than or equal
		///          to <i>maxPage</i>. </returns>
		public int MinPage
		{
			get
			{
				return MinPage_Renamed;
			}
			set
			{
				if (value <= 0 || value > MaxPage_Renamed)
				{
					throw new IllegalArgumentException("Invalid value for attribute " + "minPage");
				}
				this.MinPage_Renamed = value;
			}
		}


		/// <summary>
		/// Specifies the handling of multiple copies, including collation, for
		/// jobs using these attributes. This attribute is updated to the value
		/// chosen by the user.
		/// 
		/// @return
		///     MultipleDocumentHandlingType.SEPARATE_DOCUMENTS_COLLATED_COPIES or
		///     MultipleDocumentHandlingType.SEPARATE_DOCUMENTS_UNCOLLATED_COPIES.
		/// </summary>
		public MultipleDocumentHandlingType MultipleDocumentHandling
		{
			get
			{
				return MultipleDocumentHandling_Renamed;
			}
			set
			{
				if (value == null)
				{
					throw new IllegalArgumentException("Invalid value for attribute " + "multipleDocumentHandling");
				}
				this.MultipleDocumentHandling_Renamed = value;
			}
		}


		/// <summary>
		/// Sets the handling of multiple copies, including collation, for jobs
		/// using these attributes to the default. The default handling is
		/// MultipleDocumentHandlingType.SEPARATE_DOCUMENTS_UNCOLLATED_COPIES.
		/// </summary>
		public void SetMultipleDocumentHandlingToDefault()
		{
			MultipleDocumentHandling = MultipleDocumentHandlingType.SEPARATE_DOCUMENTS_UNCOLLATED_COPIES;
		}

		/// <summary>
		/// Specifies, for jobs using these attributes, the ranges of pages to be
		/// printed, if a range of pages is to be printed. All range numbers are
		/// inclusive. This attribute is updated to the value chosen by the user.
		/// An application should ignore this attribute on output, unless the
		/// return value of the <code>getDefaultSelection</code> method is
		/// DefaultSelectionType.RANGE.
		/// </summary>
		/// <returns>  an array of integer arrays of 2 elements. An array
		///          is interpreted as a range spanning all pages including and
		///          between the specified pages. Ranges must be in ascending
		///          order and must not overlap. Specified page numbers cannot be
		///          less than <i>minPage</i> nor greater than <i>maxPage</i>.
		///          For example:
		///          (new int[][] { new int[] { 1, 3 }, new int[] { 5, 5 },
		///                         new int[] { 15, 19 } }),
		///          specifies pages 1, 2, 3, 5, 15, 16, 17, 18, and 19. </returns>
		public int[][] PageRanges
		{
			get
			{
				if (PageRanges_Renamed != null)
				{
					// Return a copy because otherwise client code could circumvent the
					// the checks made in setPageRanges by modifying the returned
					// array.
//JAVA TO C# CONVERTER NOTE: The following call to the 'RectangularArrays' helper class reproduces the rectangular array initialization that is automatic in Java:
//ORIGINAL LINE: int[][] copy = new int[PageRanges_Renamed.Length][2];
					int[][] copy = RectangularArrays.ReturnRectangularIntArray(PageRanges_Renamed.Length, 2);
					for (int i = 0; i < PageRanges_Renamed.Length; i++)
					{
						copy[i][0] = PageRanges_Renamed[i][0];
						copy[i][1] = PageRanges_Renamed[i][1];
					}
					return copy;
				}
				else if (FromPage_Renamed != 0 || ToPage_Renamed != 0)
				{
					int fromPage = FromPage;
					int toPage = ToPage;
					return new int[][] {new int[] {fromPage, toPage}};
				}
				else
				{
					int minPage = MinPage;
					return new int[][] {new int[] {minPage, minPage}};
				}
			}
			set
			{
				String xcp = "Invalid value for attribute pageRanges";
				int first = 0;
				int last = 0;
    
				if (value == null)
				{
					throw new IllegalArgumentException(xcp);
				}
    
				for (int i = 0; i < value.Length; i++)
				{
					if (value[i] == null || value[i].Length != 2 || value[i][0] <= last || value[i][1] < value[i][0])
					{
							throw new IllegalArgumentException(xcp);
					}
					last = value[i][1];
					if (first == 0)
					{
						first = value[i][0];
					}
				}
    
				if (first < MinPage_Renamed || last > MaxPage_Renamed)
				{
					throw new IllegalArgumentException(xcp);
				}
    
				// Store a copy because otherwise client code could circumvent the
				// the checks made above by holding a reference to the array and
				// modifying it after calling setPageRanges.
//JAVA TO C# CONVERTER NOTE: The following call to the 'RectangularArrays' helper class reproduces the rectangular array initialization that is automatic in Java:
//ORIGINAL LINE: int[][] copy = new int[value.Length][2];
				int[][] copy = RectangularArrays.ReturnRectangularIntArray(value.Length, 2);
				for (int i = 0; i < value.Length; i++)
				{
					copy[i][0] = value[i][0];
					copy[i][1] = value[i][1];
				}
				this.PageRanges_Renamed = copy;
				this.PrFirst = first;
				this.PrLast = last;
			}
		}


		/// <summary>
		/// Returns the destination printer for jobs using these attributes. This
		/// attribute is updated to the value chosen by the user.
		/// </summary>
		/// <returns>  the possibly null printer name. </returns>
		public String Printer
		{
			get
			{
				return Printer_Renamed;
			}
			set
			{
				this.Printer_Renamed = value;
			}
		}


		/// <summary>
		/// Returns how consecutive pages should be imposed upon the sides of the
		/// print medium for jobs using these attributes. SidesType.ONE_SIDED
		/// imposes each consecutive page upon the same side of consecutive media
		/// sheets. This imposition is sometimes called <i>simplex</i>.
		/// SidesType.TWO_SIDED_LONG_EDGE imposes each consecutive pair of pages
		/// upon front and back sides of consecutive media sheets, such that the
		/// orientation of each pair of pages on the medium would be correct for
		/// the reader as if for binding on the long edge. This imposition is
		/// sometimes called <i>duplex</i>. SidesType.TWO_SIDED_SHORT_EDGE imposes
		/// each consecutive pair of pages upon front and back sides of consecutive
		/// media sheets, such that the orientation of each pair of pages on the
		/// medium would be correct for the reader as if for binding on the short
		/// edge. This imposition is sometimes called <i>tumble</i>. This attribute
		/// is updated to the value chosen by the user.
		/// </summary>
		/// <returns>  SidesType.ONE_SIDED, SidesType.TWO_SIDED_LONG_EDGE, or
		///          SidesType.TWO_SIDED_SHORT_EDGE. </returns>
		public SidesType Sides
		{
			get
			{
				return Sides_Renamed;
			}
			set
			{
				if (value == null)
				{
					throw new IllegalArgumentException("Invalid value for attribute " + "sides");
				}
				this.Sides_Renamed = value;
			}
		}


		/// <summary>
		/// Sets how consecutive pages should be imposed upon the sides of the
		/// print medium for jobs using these attributes to the default. The
		/// default imposition is SidesType.ONE_SIDED.
		/// </summary>
		public void SetSidesToDefault()
		{
			Sides = SidesType.ONE_SIDED;
		}

		/// <summary>
		/// Returns, for jobs using these attributes, the last page (inclusive)
		/// to be printed, if a range of pages is to be printed. This attribute is
		/// updated to the value chosen by the user. An application should ignore
		/// this attribute on output, unless the return value of the <code>
		/// getDefaultSelection</code> method is DefaultSelectionType.RANGE. An
		/// application should honor the return value of <code>getPageRanges</code>
		/// over the return value of this method, if possible.
		/// </summary>
		/// <returns>  an integer greater than zero and greater than or equal
		///          to <i>toPage</i> and greater than or equal to <i>minPage</i>
		///          and less than or equal to <i>maxPage</i>. </returns>
		public int ToPage
		{
			get
			{
				if (ToPage_Renamed != 0)
				{
					return ToPage_Renamed;
				}
				else if (FromPage_Renamed != 0)
				{
					return FromPage_Renamed;
				}
				else if (PageRanges_Renamed != null)
				{
					return PrLast;
				}
				else
				{
					return MinPage;
				}
			}
			set
			{
				if (value <= 0 || (FromPage_Renamed != 0 && value < FromPage_Renamed) || value < MinPage_Renamed || value > MaxPage_Renamed)
				{
					throw new IllegalArgumentException("Invalid value for attribute " + "toPage");
				}
				this.ToPage_Renamed = value;
			}
		}


		/// <summary>
		/// Determines whether two JobAttributes are equal to each other.
		/// <para>
		/// Two JobAttributes are equal if and only if each of their attributes are
		/// equal. Attributes of enumeration type are equal if and only if the
		/// fields refer to the same unique enumeration object. A set of page
		/// ranges is equal if and only if the sets are of equal length, each range
		/// enumerates the same pages, and the ranges are in the same order.
		/// 
		/// </para>
		/// </summary>
		/// <param name="obj"> the object whose equality will be checked. </param>
		/// <returns>  whether obj is equal to this JobAttribute according to the
		///          above criteria. </returns>
		public override bool Equals(Object obj)
		{
			if (!(obj is JobAttributes))
			{
				return false;
			}
			JobAttributes rhs = (JobAttributes)obj;

			if (FileName_Renamed == null)
			{
				if (rhs.FileName_Renamed != null)
				{
					return false;
				}
			}
			else
			{
				if (!FileName_Renamed.Equals(rhs.FileName_Renamed))
				{
					return false;
				}
			}

			if (PageRanges_Renamed == null)
			{
				if (rhs.PageRanges_Renamed != null)
				{
					return false;
				}
			}
			else
			{
				if (rhs.PageRanges_Renamed == null || PageRanges_Renamed.Length != rhs.PageRanges_Renamed.Length)
				{
					return false;
				}
				for (int i = 0; i < PageRanges_Renamed.Length; i++)
				{
					if (PageRanges_Renamed[i][0] != rhs.PageRanges_Renamed[i][0] || PageRanges_Renamed[i][1] != rhs.PageRanges_Renamed[i][1])
					{
						return false;
					}
				}
			}

			if (Printer_Renamed == null)
			{
				if (rhs.Printer_Renamed != null)
				{
					return false;
				}
			}
			else
			{
				if (!Printer_Renamed.Equals(rhs.Printer_Renamed))
				{
					return false;
				}
			}

			return (Copies_Renamed == rhs.Copies_Renamed && DefaultSelection_Renamed == rhs.DefaultSelection_Renamed && Destination_Renamed == rhs.Destination_Renamed && Dialog_Renamed == rhs.Dialog_Renamed && FromPage_Renamed == rhs.FromPage_Renamed && MaxPage_Renamed == rhs.MaxPage_Renamed && MinPage_Renamed == rhs.MinPage_Renamed && MultipleDocumentHandling_Renamed == rhs.MultipleDocumentHandling_Renamed && PrFirst == rhs.PrFirst && PrLast == rhs.PrLast && Sides_Renamed == rhs.Sides_Renamed && ToPage_Renamed == rhs.ToPage_Renamed);
		}

		/// <summary>
		/// Returns a hash code value for this JobAttributes.
		/// </summary>
		/// <returns>  the hash code. </returns>
		public override int HashCode()
		{
			int rest = ((Copies_Renamed + FromPage_Renamed + MaxPage_Renamed + MinPage_Renamed + PrFirst + PrLast + ToPage_Renamed) * 31) << 21;
			if (PageRanges_Renamed != null)
			{
				int sum = 0;
				for (int i = 0; i < PageRanges_Renamed.Length; i++)
				{
					sum += PageRanges_Renamed[i][0] + PageRanges_Renamed[i][1];
				}
				rest ^= (sum * 31) << 11;
			}
			if (FileName_Renamed != null)
			{
				rest ^= FileName_Renamed.HashCode();
			}
			if (Printer_Renamed != null)
			{
				rest ^= Printer_Renamed.HashCode();
			}
			return (DefaultSelection_Renamed.HashCode() << 6 ^ Destination_Renamed.HashCode() << 5 ^ Dialog_Renamed.HashCode() << 3 ^ MultipleDocumentHandling_Renamed.HashCode() << 2 ^ Sides_Renamed.HashCode() ^ rest);
		}

		/// <summary>
		/// Returns a string representation of this JobAttributes.
		/// </summary>
		/// <returns>  the string representation. </returns>
		public override String ToString()
		{
			int[][] pageRanges = PageRanges;
			String prStr = "[";
			bool first = true;
			for (int i = 0; i < pageRanges.Length; i++)
			{
				if (first)
				{
					first = false;
				}
				else
				{
					prStr += ",";
				}
				prStr += pageRanges[i][0] + ":" + pageRanges[i][1];
			}
			prStr += "]";

			return "copies=" + Copies + ",defaultSelection=" + DefaultSelection + ",destination=" + Destination + ",dialog=" + Dialog + ",fileName=" + FileName + ",fromPage=" + FromPage + ",maxPage=" + MaxPage + ",minPage=" + MinPage + ",multiple-document-handling=" + MultipleDocumentHandling + ",page-ranges=" + prStr + ",printer=" + Printer + ",sides=" + Sides + ",toPage=" + ToPage;
		}
	}

}
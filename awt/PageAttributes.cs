/*
 * Copyright (c) 1999, 2013, Oracle and/or its affiliates. All rights reserved.
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
	/// A set of attributes which control the output of a printed page.
	/// <para>
	/// Instances of this class control the color state, paper size (media type),
	/// orientation, logical origin, print quality, and resolution of every
	/// page which uses the instance. Attribute names are compliant with the
	/// Internet Printing Protocol (IPP) 1.1 where possible. Attribute values
	/// are partially compliant where possible.
	/// </para>
	/// <para>
	/// To use a method which takes an inner class type, pass a reference to
	/// one of the constant fields of the inner class. Client code cannot create
	/// new instances of the inner class types because none of those classes
	/// has a public constructor. For example, to set the color state to
	/// monochrome, use the following code:
	/// <pre>
	/// import java.awt.PageAttributes;
	/// 
	/// public class MonochromeExample {
	///     public void setMonochrome(PageAttributes pageAttributes) {
	///         pageAttributes.setColor(PageAttributes.ColorType.MONOCHROME);
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
	public sealed class PageAttributes : Cloneable
	{
		/// <summary>
		/// A type-safe enumeration of possible color states.
		/// @since 1.3
		/// </summary>
		public sealed class ColorType : AttributeValue
		{
			internal const int I_COLOR = 0;
			internal const int I_MONOCHROME = 1;

			internal static readonly String[] NAMES = new String[] {"color", "monochrome"};

			/// <summary>
			/// The ColorType instance to use for specifying color printing.
			/// </summary>
			public static readonly ColorType COLOR = new ColorType(I_COLOR);
			/// <summary>
			/// The ColorType instance to use for specifying monochrome printing.
			/// </summary>
			public static readonly ColorType MONOCHROME = new ColorType(I_MONOCHROME);

			internal ColorType(int type) : base(type, NAMES)
			{
			}
		}

		/// <summary>
		/// A type-safe enumeration of possible paper sizes. These sizes are in
		/// compliance with IPP 1.1.
		/// @since 1.3
		/// </summary>
		public sealed class MediaType : AttributeValue
		{
			internal const int I_ISO_4A0 = 0;
			internal const int I_ISO_2A0 = 1;
			internal const int I_ISO_A0 = 2;
			internal const int I_ISO_A1 = 3;
			internal const int I_ISO_A2 = 4;
			internal const int I_ISO_A3 = 5;
			internal const int I_ISO_A4 = 6;
			internal const int I_ISO_A5 = 7;
			internal const int I_ISO_A6 = 8;
			internal const int I_ISO_A7 = 9;
			internal const int I_ISO_A8 = 10;
			internal const int I_ISO_A9 = 11;
			internal const int I_ISO_A10 = 12;
			internal const int I_ISO_B0 = 13;
			internal const int I_ISO_B1 = 14;
			internal const int I_ISO_B2 = 15;
			internal const int I_ISO_B3 = 16;
			internal const int I_ISO_B4 = 17;
			internal const int I_ISO_B5 = 18;
			internal const int I_ISO_B6 = 19;
			internal const int I_ISO_B7 = 20;
			internal const int I_ISO_B8 = 21;
			internal const int I_ISO_B9 = 22;
			internal const int I_ISO_B10 = 23;
			internal const int I_JIS_B0 = 24;
			internal const int I_JIS_B1 = 25;
			internal const int I_JIS_B2 = 26;
			internal const int I_JIS_B3 = 27;
			internal const int I_JIS_B4 = 28;
			internal const int I_JIS_B5 = 29;
			internal const int I_JIS_B6 = 30;
			internal const int I_JIS_B7 = 31;
			internal const int I_JIS_B8 = 32;
			internal const int I_JIS_B9 = 33;
			internal const int I_JIS_B10 = 34;
			internal const int I_ISO_C0 = 35;
			internal const int I_ISO_C1 = 36;
			internal const int I_ISO_C2 = 37;
			internal const int I_ISO_C3 = 38;
			internal const int I_ISO_C4 = 39;
			internal const int I_ISO_C5 = 40;
			internal const int I_ISO_C6 = 41;
			internal const int I_ISO_C7 = 42;
			internal const int I_ISO_C8 = 43;
			internal const int I_ISO_C9 = 44;
			internal const int I_ISO_C10 = 45;
			internal const int I_ISO_DESIGNATED_LONG = 46;
			internal const int I_EXECUTIVE = 47;
			internal const int I_FOLIO = 48;
			internal const int I_INVOICE = 49;
			internal const int I_LEDGER = 50;
			internal const int I_NA_LETTER = 51;
			internal const int I_NA_LEGAL = 52;
			internal const int I_QUARTO = 53;
			internal const int I_A = 54;
			internal const int I_B = 55;
			internal const int I_C = 56;
			internal const int I_D = 57;
			internal const int I_E = 58;
			internal const int I_NA_10X15_ENVELOPE = 59;
			internal const int I_NA_10X14_ENVELOPE = 60;
			internal const int I_NA_10X13_ENVELOPE = 61;
			internal const int I_NA_9X12_ENVELOPE = 62;
			internal const int I_NA_9X11_ENVELOPE = 63;
			internal const int I_NA_7X9_ENVELOPE = 64;
			internal const int I_NA_6X9_ENVELOPE = 65;
			internal const int I_NA_NUMBER_9_ENVELOPE = 66;
			internal const int I_NA_NUMBER_10_ENVELOPE = 67;
			internal const int I_NA_NUMBER_11_ENVELOPE = 68;
			internal const int I_NA_NUMBER_12_ENVELOPE = 69;
			internal const int I_NA_NUMBER_14_ENVELOPE = 70;
			internal const int I_INVITE_ENVELOPE = 71;
			internal const int I_ITALY_ENVELOPE = 72;
			internal const int I_MONARCH_ENVELOPE = 73;
			internal const int I_PERSONAL_ENVELOPE = 74;

			internal static readonly String[] NAMES = new String[] {"iso-4a0", "iso-2a0", "iso-a0", "iso-a1", "iso-a2", "iso-a3", "iso-a4", "iso-a5", "iso-a6", "iso-a7", "iso-a8", "iso-a9", "iso-a10", "iso-b0", "iso-b1", "iso-b2", "iso-b3", "iso-b4", "iso-b5", "iso-b6", "iso-b7", "iso-b8", "iso-b9", "iso-b10", "jis-b0", "jis-b1", "jis-b2", "jis-b3", "jis-b4", "jis-b5", "jis-b6", "jis-b7", "jis-b8", "jis-b9", "jis-b10", "iso-c0", "iso-c1", "iso-c2", "iso-c3", "iso-c4", "iso-c5", "iso-c6", "iso-c7", "iso-c8", "iso-c9", "iso-c10", "iso-designated-long", "executive", "folio", "invoice", "ledger", "na-letter", "na-legal", "quarto", "a", "b", "c", "d", "e", "na-10x15-envelope", "na-10x14-envelope", "na-10x13-envelope", "na-9x12-envelope", "na-9x11-envelope", "na-7x9-envelope", "na-6x9-envelope", "na-number-9-envelope", "na-number-10-envelope", "na-number-11-envelope", "na-number-12-envelope", "na-number-14-envelope", "invite-envelope", "italy-envelope", "monarch-envelope", "personal-envelope"};

			/// <summary>
			/// The MediaType instance for ISO/DIN and JIS 4A0, 1682 x 2378 mm.
			/// </summary>
			public static readonly MediaType ISO_4A0 = new MediaType(I_ISO_4A0);
			/// <summary>
			/// The MediaType instance for ISO/DIN and JIS 2A0, 1189 x 1682 mm.
			/// </summary>
			public static readonly MediaType ISO_2A0 = new MediaType(I_ISO_2A0);
			/// <summary>
			/// The MediaType instance for ISO/DIN and JIS A0, 841 x 1189 mm.
			/// </summary>
			public static readonly MediaType ISO_A0 = new MediaType(I_ISO_A0);
			/// <summary>
			/// The MediaType instance for ISO/DIN and JIS A1, 594 x 841 mm.
			/// </summary>
			public static readonly MediaType ISO_A1 = new MediaType(I_ISO_A1);
			/// <summary>
			/// The MediaType instance for ISO/DIN and JIS A2, 420 x 594 mm.
			/// </summary>
			public static readonly MediaType ISO_A2 = new MediaType(I_ISO_A2);
			/// <summary>
			/// The MediaType instance for ISO/DIN and JIS A3, 297 x 420 mm.
			/// </summary>
			public static readonly MediaType ISO_A3 = new MediaType(I_ISO_A3);
			/// <summary>
			/// The MediaType instance for ISO/DIN and JIS A4, 210 x 297 mm.
			/// </summary>
			public static readonly MediaType ISO_A4 = new MediaType(I_ISO_A4);
			/// <summary>
			/// The MediaType instance for ISO/DIN and JIS A5, 148 x 210 mm.
			/// </summary>
			public static readonly MediaType ISO_A5 = new MediaType(I_ISO_A5);
			/// <summary>
			/// The MediaType instance for ISO/DIN and JIS A6, 105 x 148 mm.
			/// </summary>
			public static readonly MediaType ISO_A6 = new MediaType(I_ISO_A6);
			/// <summary>
			/// The MediaType instance for ISO/DIN and JIS A7, 74 x 105 mm.
			/// </summary>
			public static readonly MediaType ISO_A7 = new MediaType(I_ISO_A7);
			/// <summary>
			/// The MediaType instance for ISO/DIN and JIS A8, 52 x 74 mm.
			/// </summary>
			public static readonly MediaType ISO_A8 = new MediaType(I_ISO_A8);
			/// <summary>
			/// The MediaType instance for ISO/DIN and JIS A9, 37 x 52 mm.
			/// </summary>
			public static readonly MediaType ISO_A9 = new MediaType(I_ISO_A9);
			/// <summary>
			/// The MediaType instance for ISO/DIN and JIS A10, 26 x 37 mm.
			/// </summary>
			public static readonly MediaType ISO_A10 = new MediaType(I_ISO_A10);
			/// <summary>
			/// The MediaType instance for ISO/DIN B0, 1000 x 1414 mm.
			/// </summary>
			public static readonly MediaType ISO_B0 = new MediaType(I_ISO_B0);
			/// <summary>
			/// The MediaType instance for ISO/DIN B1, 707 x 1000 mm.
			/// </summary>
			public static readonly MediaType ISO_B1 = new MediaType(I_ISO_B1);
			/// <summary>
			/// The MediaType instance for ISO/DIN B2, 500 x 707 mm.
			/// </summary>
			public static readonly MediaType ISO_B2 = new MediaType(I_ISO_B2);
			/// <summary>
			/// The MediaType instance for ISO/DIN B3, 353 x 500 mm.
			/// </summary>
			public static readonly MediaType ISO_B3 = new MediaType(I_ISO_B3);
			/// <summary>
			/// The MediaType instance for ISO/DIN B4, 250 x 353 mm.
			/// </summary>
			public static readonly MediaType ISO_B4 = new MediaType(I_ISO_B4);
			/// <summary>
			/// The MediaType instance for ISO/DIN B5, 176 x 250 mm.
			/// </summary>
			public static readonly MediaType ISO_B5 = new MediaType(I_ISO_B5);
			/// <summary>
			/// The MediaType instance for ISO/DIN B6, 125 x 176 mm.
			/// </summary>
			public static readonly MediaType ISO_B6 = new MediaType(I_ISO_B6);
			/// <summary>
			/// The MediaType instance for ISO/DIN B7, 88 x 125 mm.
			/// </summary>
			public static readonly MediaType ISO_B7 = new MediaType(I_ISO_B7);
			/// <summary>
			/// The MediaType instance for ISO/DIN B8, 62 x 88 mm.
			/// </summary>
			public static readonly MediaType ISO_B8 = new MediaType(I_ISO_B8);
			/// <summary>
			/// The MediaType instance for ISO/DIN B9, 44 x 62 mm.
			/// </summary>
			public static readonly MediaType ISO_B9 = new MediaType(I_ISO_B9);
			/// <summary>
			/// The MediaType instance for ISO/DIN B10, 31 x 44 mm.
			/// </summary>
			public static readonly MediaType ISO_B10 = new MediaType(I_ISO_B10);
			/// <summary>
			/// The MediaType instance for JIS B0, 1030 x 1456 mm.
			/// </summary>
			public static readonly MediaType JIS_B0 = new MediaType(I_JIS_B0);
			/// <summary>
			/// The MediaType instance for JIS B1, 728 x 1030 mm.
			/// </summary>
			public static readonly MediaType JIS_B1 = new MediaType(I_JIS_B1);
			/// <summary>
			/// The MediaType instance for JIS B2, 515 x 728 mm.
			/// </summary>
			public static readonly MediaType JIS_B2 = new MediaType(I_JIS_B2);
			/// <summary>
			/// The MediaType instance for JIS B3, 364 x 515 mm.
			/// </summary>
			public static readonly MediaType JIS_B3 = new MediaType(I_JIS_B3);
			/// <summary>
			/// The MediaType instance for JIS B4, 257 x 364 mm.
			/// </summary>
			public static readonly MediaType JIS_B4 = new MediaType(I_JIS_B4);
			/// <summary>
			/// The MediaType instance for JIS B5, 182 x 257 mm.
			/// </summary>
			public static readonly MediaType JIS_B5 = new MediaType(I_JIS_B5);
			/// <summary>
			/// The MediaType instance for JIS B6, 128 x 182 mm.
			/// </summary>
			public static readonly MediaType JIS_B6 = new MediaType(I_JIS_B6);
			/// <summary>
			/// The MediaType instance for JIS B7, 91 x 128 mm.
			/// </summary>
			public static readonly MediaType JIS_B7 = new MediaType(I_JIS_B7);
			/// <summary>
			/// The MediaType instance for JIS B8, 64 x 91 mm.
			/// </summary>
			public static readonly MediaType JIS_B8 = new MediaType(I_JIS_B8);
			/// <summary>
			/// The MediaType instance for JIS B9, 45 x 64 mm.
			/// </summary>
			public static readonly MediaType JIS_B9 = new MediaType(I_JIS_B9);
			/// <summary>
			/// The MediaType instance for JIS B10, 32 x 45 mm.
			/// </summary>
			public static readonly MediaType JIS_B10 = new MediaType(I_JIS_B10);
			/// <summary>
			/// The MediaType instance for ISO/DIN C0, 917 x 1297 mm.
			/// </summary>
			public static readonly MediaType ISO_C0 = new MediaType(I_ISO_C0);
			/// <summary>
			/// The MediaType instance for ISO/DIN C1, 648 x 917 mm.
			/// </summary>
			public static readonly MediaType ISO_C1 = new MediaType(I_ISO_C1);
			/// <summary>
			/// The MediaType instance for ISO/DIN C2, 458 x 648 mm.
			/// </summary>
			public static readonly MediaType ISO_C2 = new MediaType(I_ISO_C2);
			/// <summary>
			/// The MediaType instance for ISO/DIN C3, 324 x 458 mm.
			/// </summary>
			public static readonly MediaType ISO_C3 = new MediaType(I_ISO_C3);
			/// <summary>
			/// The MediaType instance for ISO/DIN C4, 229 x 324 mm.
			/// </summary>
			public static readonly MediaType ISO_C4 = new MediaType(I_ISO_C4);
			/// <summary>
			/// The MediaType instance for ISO/DIN C5, 162 x 229 mm.
			/// </summary>
			public static readonly MediaType ISO_C5 = new MediaType(I_ISO_C5);
			/// <summary>
			/// The MediaType instance for ISO/DIN C6, 114 x 162 mm.
			/// </summary>
			public static readonly MediaType ISO_C6 = new MediaType(I_ISO_C6);
			/// <summary>
			/// The MediaType instance for ISO/DIN C7, 81 x 114 mm.
			/// </summary>
			public static readonly MediaType ISO_C7 = new MediaType(I_ISO_C7);
			/// <summary>
			/// The MediaType instance for ISO/DIN C8, 57 x 81 mm.
			/// </summary>
			public static readonly MediaType ISO_C8 = new MediaType(I_ISO_C8);
			/// <summary>
			/// The MediaType instance for ISO/DIN C9, 40 x 57 mm.
			/// </summary>
			public static readonly MediaType ISO_C9 = new MediaType(I_ISO_C9);
			/// <summary>
			/// The MediaType instance for ISO/DIN C10, 28 x 40 mm.
			/// </summary>
			public static readonly MediaType ISO_C10 = new MediaType(I_ISO_C10);
			/// <summary>
			/// The MediaType instance for ISO Designated Long, 110 x 220 mm.
			/// </summary>
			public static readonly MediaType ISO_DESIGNATED_LONG = new MediaType(I_ISO_DESIGNATED_LONG);
			/// <summary>
			/// The MediaType instance for Executive, 7 1/4 x 10 1/2 in.
			/// </summary>
			public static readonly MediaType EXECUTIVE = new MediaType(I_EXECUTIVE);
			/// <summary>
			/// The MediaType instance for Folio, 8 1/2 x 13 in.
			/// </summary>
			public static readonly MediaType FOLIO = new MediaType(I_FOLIO);
			/// <summary>
			/// The MediaType instance for Invoice, 5 1/2 x 8 1/2 in.
			/// </summary>
			public static readonly MediaType INVOICE = new MediaType(I_INVOICE);
			/// <summary>
			/// The MediaType instance for Ledger, 11 x 17 in.
			/// </summary>
			public static readonly MediaType LEDGER = new MediaType(I_LEDGER);
			/// <summary>
			/// The MediaType instance for North American Letter, 8 1/2 x 11 in.
			/// </summary>
			public static readonly MediaType NA_LETTER = new MediaType(I_NA_LETTER);
			/// <summary>
			/// The MediaType instance for North American Legal, 8 1/2 x 14 in.
			/// </summary>
			public static readonly MediaType NA_LEGAL = new MediaType(I_NA_LEGAL);
			/// <summary>
			/// The MediaType instance for Quarto, 215 x 275 mm.
			/// </summary>
			public static readonly MediaType QUARTO = new MediaType(I_QUARTO);
			/// <summary>
			/// The MediaType instance for Engineering A, 8 1/2 x 11 in.
			/// </summary>
			public static readonly MediaType A = new MediaType(I_A);
			/// <summary>
			/// The MediaType instance for Engineering B, 11 x 17 in.
			/// </summary>
			public static readonly MediaType B = new MediaType(I_B);
			/// <summary>
			/// The MediaType instance for Engineering C, 17 x 22 in.
			/// </summary>
			public static readonly MediaType C = new MediaType(I_C);
			/// <summary>
			/// The MediaType instance for Engineering D, 22 x 34 in.
			/// </summary>
			public static readonly MediaType D = new MediaType(I_D);
			/// <summary>
			/// The MediaType instance for Engineering E, 34 x 44 in.
			/// </summary>
			public static readonly MediaType E = new MediaType(I_E);
			/// <summary>
			/// The MediaType instance for North American 10 x 15 in.
			/// </summary>
			public static readonly MediaType NA_10X15_ENVELOPE = new MediaType(I_NA_10X15_ENVELOPE);
			/// <summary>
			/// The MediaType instance for North American 10 x 14 in.
			/// </summary>
			public static readonly MediaType NA_10X14_ENVELOPE = new MediaType(I_NA_10X14_ENVELOPE);
			/// <summary>
			/// The MediaType instance for North American 10 x 13 in.
			/// </summary>
			public static readonly MediaType NA_10X13_ENVELOPE = new MediaType(I_NA_10X13_ENVELOPE);
			/// <summary>
			/// The MediaType instance for North American 9 x 12 in.
			/// </summary>
			public static readonly MediaType NA_9X12_ENVELOPE = new MediaType(I_NA_9X12_ENVELOPE);
			/// <summary>
			/// The MediaType instance for North American 9 x 11 in.
			/// </summary>
			public static readonly MediaType NA_9X11_ENVELOPE = new MediaType(I_NA_9X11_ENVELOPE);
			/// <summary>
			/// The MediaType instance for North American 7 x 9 in.
			/// </summary>
			public static readonly MediaType NA_7X9_ENVELOPE = new MediaType(I_NA_7X9_ENVELOPE);
			/// <summary>
			/// The MediaType instance for North American 6 x 9 in.
			/// </summary>
			public static readonly MediaType NA_6X9_ENVELOPE = new MediaType(I_NA_6X9_ENVELOPE);
			/// <summary>
			/// The MediaType instance for North American #9 Business Envelope,
			/// 3 7/8 x 8 7/8 in.
			/// </summary>
			public static readonly MediaType NA_NUMBER_9_ENVELOPE = new MediaType(I_NA_NUMBER_9_ENVELOPE);
			/// <summary>
			/// The MediaType instance for North American #10 Business Envelope,
			/// 4 1/8 x 9 1/2 in.
			/// </summary>
			public static readonly MediaType NA_NUMBER_10_ENVELOPE = new MediaType(I_NA_NUMBER_10_ENVELOPE);
			/// <summary>
			/// The MediaType instance for North American #11 Business Envelope,
			/// 4 1/2 x 10 3/8 in.
			/// </summary>
			public static readonly MediaType NA_NUMBER_11_ENVELOPE = new MediaType(I_NA_NUMBER_11_ENVELOPE);
			/// <summary>
			/// The MediaType instance for North American #12 Business Envelope,
			/// 4 3/4 x 11 in.
			/// </summary>
			public static readonly MediaType NA_NUMBER_12_ENVELOPE = new MediaType(I_NA_NUMBER_12_ENVELOPE);
			/// <summary>
			/// The MediaType instance for North American #14 Business Envelope,
			/// 5 x 11 1/2 in.
			/// </summary>
			public static readonly MediaType NA_NUMBER_14_ENVELOPE = new MediaType(I_NA_NUMBER_14_ENVELOPE);
			/// <summary>
			/// The MediaType instance for Invitation Envelope, 220 x 220 mm.
			/// </summary>
			public static readonly MediaType INVITE_ENVELOPE = new MediaType(I_INVITE_ENVELOPE);
			/// <summary>
			/// The MediaType instance for Italy Envelope, 110 x 230 mm.
			/// </summary>
			public static readonly MediaType ITALY_ENVELOPE = new MediaType(I_ITALY_ENVELOPE);
			/// <summary>
			/// The MediaType instance for Monarch Envelope, 3 7/8 x 7 1/2 in.
			/// </summary>
			public static readonly MediaType MONARCH_ENVELOPE = new MediaType(I_MONARCH_ENVELOPE);
			/// <summary>
			/// The MediaType instance for 6 3/4 envelope, 3 5/8 x 6 1/2 in.
			/// </summary>
			public static readonly MediaType PERSONAL_ENVELOPE = new MediaType(I_PERSONAL_ENVELOPE);
			/// <summary>
			/// An alias for ISO_A0.
			/// </summary>
			public static readonly MediaType A0 = ISO_A0;
			/// <summary>
			/// An alias for ISO_A1.
			/// </summary>
			public static readonly MediaType A1 = ISO_A1;
			/// <summary>
			/// An alias for ISO_A2.
			/// </summary>
			public static readonly MediaType A2 = ISO_A2;
			/// <summary>
			/// An alias for ISO_A3.
			/// </summary>
			public static readonly MediaType A3 = ISO_A3;
			/// <summary>
			/// An alias for ISO_A4.
			/// </summary>
			public static readonly MediaType A4 = ISO_A4;
			/// <summary>
			/// An alias for ISO_A5.
			/// </summary>
			public static readonly MediaType A5 = ISO_A5;
			/// <summary>
			/// An alias for ISO_A6.
			/// </summary>
			public static readonly MediaType A6 = ISO_A6;
			/// <summary>
			/// An alias for ISO_A7.
			/// </summary>
			public static readonly MediaType A7 = ISO_A7;
			/// <summary>
			/// An alias for ISO_A8.
			/// </summary>
			public static readonly MediaType A8 = ISO_A8;
			/// <summary>
			/// An alias for ISO_A9.
			/// </summary>
			public static readonly MediaType A9 = ISO_A9;
			/// <summary>
			/// An alias for ISO_A10.
			/// </summary>
			public static readonly MediaType A10 = ISO_A10;
			/// <summary>
			/// An alias for ISO_B0.
			/// </summary>
			public static readonly MediaType B0 = ISO_B0;
			/// <summary>
			/// An alias for ISO_B1.
			/// </summary>
			public static readonly MediaType B1 = ISO_B1;
			/// <summary>
			/// An alias for ISO_B2.
			/// </summary>
			public static readonly MediaType B2 = ISO_B2;
			/// <summary>
			/// An alias for ISO_B3.
			/// </summary>
			public static readonly MediaType B3 = ISO_B3;
			/// <summary>
			/// An alias for ISO_B4.
			/// </summary>
			public static readonly MediaType B4 = ISO_B4;
			/// <summary>
			/// An alias for ISO_B4.
			/// </summary>
			public static readonly MediaType ISO_B4_ENVELOPE = ISO_B4;
			/// <summary>
			/// An alias for ISO_B5.
			/// </summary>
			public static readonly MediaType B5 = ISO_B5;
			/// <summary>
			/// An alias for ISO_B5.
			/// </summary>
			public static readonly MediaType ISO_B5_ENVELOPE = ISO_B5;
			/// <summary>
			/// An alias for ISO_B6.
			/// </summary>
			public static readonly MediaType B6 = ISO_B6;
			/// <summary>
			/// An alias for ISO_B7.
			/// </summary>
			public static readonly MediaType B7 = ISO_B7;
			/// <summary>
			/// An alias for ISO_B8.
			/// </summary>
			public static readonly MediaType B8 = ISO_B8;
			/// <summary>
			/// An alias for ISO_B9.
			/// </summary>
			public static readonly MediaType B9 = ISO_B9;
			/// <summary>
			/// An alias for ISO_B10.
			/// </summary>
			public static readonly MediaType B10 = ISO_B10;
			/// <summary>
			/// An alias for ISO_C0.
			/// </summary>
			public static readonly MediaType C0 = ISO_C0;
			/// <summary>
			/// An alias for ISO_C0.
			/// </summary>
			public static readonly MediaType ISO_C0_ENVELOPE = ISO_C0;
			/// <summary>
			/// An alias for ISO_C1.
			/// </summary>
			public static readonly MediaType C1 = ISO_C1;
			/// <summary>
			/// An alias for ISO_C1.
			/// </summary>
			public static readonly MediaType ISO_C1_ENVELOPE = ISO_C1;
			/// <summary>
			/// An alias for ISO_C2.
			/// </summary>
			public static readonly MediaType C2 = ISO_C2;
			/// <summary>
			/// An alias for ISO_C2.
			/// </summary>
			public static readonly MediaType ISO_C2_ENVELOPE = ISO_C2;
			/// <summary>
			/// An alias for ISO_C3.
			/// </summary>
			public static readonly MediaType C3 = ISO_C3;
			/// <summary>
			/// An alias for ISO_C3.
			/// </summary>
			public static readonly MediaType ISO_C3_ENVELOPE = ISO_C3;
			/// <summary>
			/// An alias for ISO_C4.
			/// </summary>
			public static readonly MediaType C4 = ISO_C4;
			/// <summary>
			/// An alias for ISO_C4.
			/// </summary>
			public static readonly MediaType ISO_C4_ENVELOPE = ISO_C4;
			/// <summary>
			/// An alias for ISO_C5.
			/// </summary>
			public static readonly MediaType C5 = ISO_C5;
			/// <summary>
			/// An alias for ISO_C5.
			/// </summary>
			public static readonly MediaType ISO_C5_ENVELOPE = ISO_C5;
			/// <summary>
			/// An alias for ISO_C6.
			/// </summary>
			public static readonly MediaType C6 = ISO_C6;
			/// <summary>
			/// An alias for ISO_C6.
			/// </summary>
			public static readonly MediaType ISO_C6_ENVELOPE = ISO_C6;
			/// <summary>
			/// An alias for ISO_C7.
			/// </summary>
			public static readonly MediaType C7 = ISO_C7;
			/// <summary>
			/// An alias for ISO_C7.
			/// </summary>
			public static readonly MediaType ISO_C7_ENVELOPE = ISO_C7;
			/// <summary>
			/// An alias for ISO_C8.
			/// </summary>
			public static readonly MediaType C8 = ISO_C8;
			/// <summary>
			/// An alias for ISO_C8.
			/// </summary>
			public static readonly MediaType ISO_C8_ENVELOPE = ISO_C8;
			/// <summary>
			/// An alias for ISO_C9.
			/// </summary>
			public static readonly MediaType C9 = ISO_C9;
			/// <summary>
			/// An alias for ISO_C9.
			/// </summary>
			public static readonly MediaType ISO_C9_ENVELOPE = ISO_C9;
			/// <summary>
			/// An alias for ISO_C10.
			/// </summary>
			public static readonly MediaType C10 = ISO_C10;
			/// <summary>
			/// An alias for ISO_C10.
			/// </summary>
			public static readonly MediaType ISO_C10_ENVELOPE = ISO_C10;
			/// <summary>
			/// An alias for ISO_DESIGNATED_LONG.
			/// </summary>
			public static readonly MediaType ISO_DESIGNATED_LONG_ENVELOPE = ISO_DESIGNATED_LONG;
			/// <summary>
			/// An alias for INVOICE.
			/// </summary>
			public static readonly MediaType STATEMENT = INVOICE;
			/// <summary>
			/// An alias for LEDGER.
			/// </summary>
			public static readonly MediaType TABLOID = LEDGER;
			/// <summary>
			/// An alias for NA_LETTER.
			/// </summary>
			public static readonly MediaType LETTER = NA_LETTER;
			/// <summary>
			/// An alias for NA_LETTER.
			/// </summary>
			public static readonly MediaType NOTE = NA_LETTER;
			/// <summary>
			/// An alias for NA_LEGAL.
			/// </summary>
			public static readonly MediaType LEGAL = NA_LEGAL;
			/// <summary>
			/// An alias for NA_10X15_ENVELOPE.
			/// </summary>
			public static readonly MediaType ENV_10X15 = NA_10X15_ENVELOPE;
			/// <summary>
			/// An alias for NA_10X14_ENVELOPE.
			/// </summary>
			public static readonly MediaType ENV_10X14 = NA_10X14_ENVELOPE;
			/// <summary>
			/// An alias for NA_10X13_ENVELOPE.
			/// </summary>
			public static readonly MediaType ENV_10X13 = NA_10X13_ENVELOPE;
			/// <summary>
			/// An alias for NA_9X12_ENVELOPE.
			/// </summary>
			public static readonly MediaType ENV_9X12 = NA_9X12_ENVELOPE;
			/// <summary>
			/// An alias for NA_9X11_ENVELOPE.
			/// </summary>
			public static readonly MediaType ENV_9X11 = NA_9X11_ENVELOPE;
			/// <summary>
			/// An alias for NA_7X9_ENVELOPE.
			/// </summary>
			public static readonly MediaType ENV_7X9 = NA_7X9_ENVELOPE;
			/// <summary>
			/// An alias for NA_6X9_ENVELOPE.
			/// </summary>
			public static readonly MediaType ENV_6X9 = NA_6X9_ENVELOPE;
			/// <summary>
			/// An alias for NA_NUMBER_9_ENVELOPE.
			/// </summary>
			public static readonly MediaType ENV_9 = NA_NUMBER_9_ENVELOPE;
			/// <summary>
			/// An alias for NA_NUMBER_10_ENVELOPE.
			/// </summary>
			public static readonly MediaType ENV_10 = NA_NUMBER_10_ENVELOPE;
			/// <summary>
			/// An alias for NA_NUMBER_11_ENVELOPE.
			/// </summary>
			public static readonly MediaType ENV_11 = NA_NUMBER_11_ENVELOPE;
			/// <summary>
			/// An alias for NA_NUMBER_12_ENVELOPE.
			/// </summary>
			public static readonly MediaType ENV_12 = NA_NUMBER_12_ENVELOPE;
			/// <summary>
			/// An alias for NA_NUMBER_14_ENVELOPE.
			/// </summary>
			public static readonly MediaType ENV_14 = NA_NUMBER_14_ENVELOPE;
			/// <summary>
			/// An alias for INVITE_ENVELOPE.
			/// </summary>
			public static readonly MediaType ENV_INVITE = INVITE_ENVELOPE;
			/// <summary>
			/// An alias for ITALY_ENVELOPE.
			/// </summary>
			public static readonly MediaType ENV_ITALY = ITALY_ENVELOPE;
			/// <summary>
			/// An alias for MONARCH_ENVELOPE.
			/// </summary>
			public static readonly MediaType ENV_MONARCH = MONARCH_ENVELOPE;
			/// <summary>
			/// An alias for PERSONAL_ENVELOPE.
			/// </summary>
			public static readonly MediaType ENV_PERSONAL = PERSONAL_ENVELOPE;
			/// <summary>
			/// An alias for INVITE_ENVELOPE.
			/// </summary>
			public static readonly MediaType INVITE = INVITE_ENVELOPE;
			/// <summary>
			/// An alias for ITALY_ENVELOPE.
			/// </summary>
			public static readonly MediaType ITALY = ITALY_ENVELOPE;
			/// <summary>
			/// An alias for MONARCH_ENVELOPE.
			/// </summary>
			public static readonly MediaType MONARCH = MONARCH_ENVELOPE;
			/// <summary>
			/// An alias for PERSONAL_ENVELOPE.
			/// </summary>
			public static readonly MediaType PERSONAL = PERSONAL_ENVELOPE;

			internal MediaType(int type) : base(type, NAMES)
			{
			}
		}

		/// <summary>
		/// A type-safe enumeration of possible orientations. These orientations
		/// are in partial compliance with IPP 1.1.
		/// @since 1.3
		/// </summary>
		public sealed class OrientationRequestedType : AttributeValue
		{
			internal const int I_PORTRAIT = 0;
			internal const int I_LANDSCAPE = 1;

			internal static readonly String[] NAMES = new String[] {"portrait", "landscape"};

			/// <summary>
			/// The OrientationRequestedType instance to use for specifying a
			/// portrait orientation.
			/// </summary>
			public static readonly OrientationRequestedType PORTRAIT = new OrientationRequestedType(I_PORTRAIT);
			/// <summary>
			/// The OrientationRequestedType instance to use for specifying a
			/// landscape orientation.
			/// </summary>
			public static readonly OrientationRequestedType LANDSCAPE = new OrientationRequestedType(I_LANDSCAPE);

			internal OrientationRequestedType(int type) : base(type, NAMES)
			{
			}
		}

		/// <summary>
		/// A type-safe enumeration of possible origins.
		/// @since 1.3
		/// </summary>
		public sealed class OriginType : AttributeValue
		{
			internal const int I_PHYSICAL = 0;
			internal const int I_PRINTABLE = 1;

			internal static readonly String[] NAMES = new String[] {"physical", "printable"};

			/// <summary>
			/// The OriginType instance to use for specifying a physical origin.
			/// </summary>
			public static readonly OriginType PHYSICAL = new OriginType(I_PHYSICAL);
			/// <summary>
			/// The OriginType instance to use for specifying a printable origin.
			/// </summary>
			public static readonly OriginType PRINTABLE = new OriginType(I_PRINTABLE);

			internal OriginType(int type) : base(type, NAMES)
			{
			}
		}

		/// <summary>
		/// A type-safe enumeration of possible print qualities. These print
		/// qualities are in compliance with IPP 1.1.
		/// @since 1.3
		/// </summary>
		public sealed class PrintQualityType : AttributeValue
		{
			internal const int I_HIGH = 0;
			internal const int I_NORMAL = 1;
			internal const int I_DRAFT = 2;

			internal static readonly String[] NAMES = new String[] {"high", "normal", "draft"};

			/// <summary>
			/// The PrintQualityType instance to use for specifying a high print
			/// quality.
			/// </summary>
			public static readonly PrintQualityType HIGH = new PrintQualityType(I_HIGH);
			/// <summary>
			/// The PrintQualityType instance to use for specifying a normal print
			/// quality.
			/// </summary>
			public static readonly PrintQualityType NORMAL = new PrintQualityType(I_NORMAL);
			/// <summary>
			/// The PrintQualityType instance to use for specifying a draft print
			/// quality.
			/// </summary>
			public static readonly PrintQualityType DRAFT = new PrintQualityType(I_DRAFT);

			internal PrintQualityType(int type) : base(type, NAMES)
			{
			}
		}

		private ColorType Color_Renamed;
		private MediaType Media_Renamed;
		private OrientationRequestedType OrientationRequested_Renamed;
		private OriginType Origin_Renamed;
		private PrintQualityType PrintQuality_Renamed;
		private int[] PrinterResolution_Renamed;

		/// <summary>
		/// Constructs a PageAttributes instance with default values for every
		/// attribute.
		/// </summary>
		public PageAttributes()
		{
			Color = ColorType.MONOCHROME;
			SetMediaToDefault();
			SetOrientationRequestedToDefault();
			Origin = OriginType.PHYSICAL;
			SetPrintQualityToDefault();
			SetPrinterResolutionToDefault();
		}

		/// <summary>
		/// Constructs a PageAttributes instance which is a copy of the supplied
		/// PageAttributes.
		/// </summary>
		/// <param name="obj"> the PageAttributes to copy. </param>
		public PageAttributes(PageAttributes obj)
		{
			Set(obj);
		}

		/// <summary>
		/// Constructs a PageAttributes instance with the specified values for
		/// every attribute.
		/// </summary>
		/// <param name="color"> ColorType.COLOR or ColorType.MONOCHROME. </param>
		/// <param name="media"> one of the constant fields of the MediaType class. </param>
		/// <param name="orientationRequested"> OrientationRequestedType.PORTRAIT or
		///          OrientationRequestedType.LANDSCAPE. </param>
		/// <param name="origin"> OriginType.PHYSICAL or OriginType.PRINTABLE </param>
		/// <param name="printQuality"> PrintQualityType.DRAFT, PrintQualityType.NORMAL,
		///          or PrintQualityType.HIGH </param>
		/// <param name="printerResolution"> an integer array of 3 elements. The first
		///          element must be greater than 0. The second element must be
		///          must be greater than 0. The third element must be either
		///          <code>3</code> or <code>4</code>. </param>
		/// <exception cref="IllegalArgumentException"> if one or more of the above
		///          conditions is violated. </exception>
		public PageAttributes(ColorType color, MediaType media, OrientationRequestedType orientationRequested, OriginType origin, PrintQualityType printQuality, int[] printerResolution)
		{
			Color = color;
			Media = media;
			SetOrientationRequested(orientationRequested);
			Origin = origin;
			SetPrintQuality(printQuality);
			PrinterResolution = printerResolution;
		}

		/// <summary>
		/// Creates and returns a copy of this PageAttributes.
		/// </summary>
		/// <returns>  the newly created copy. It is safe to cast this Object into
		///          a PageAttributes. </returns>
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
		/// Sets all of the attributes of this PageAttributes to the same values as
		/// the attributes of obj.
		/// </summary>
		/// <param name="obj"> the PageAttributes to copy. </param>
		public void Set(PageAttributes obj)
		{
			Color_Renamed = obj.Color_Renamed;
			Media_Renamed = obj.Media_Renamed;
			OrientationRequested_Renamed = obj.OrientationRequested_Renamed;
			Origin_Renamed = obj.Origin_Renamed;
			PrintQuality_Renamed = obj.PrintQuality_Renamed;
			// okay because we never modify the contents of printerResolution
			PrinterResolution_Renamed = obj.PrinterResolution_Renamed;
		}

		/// <summary>
		/// Returns whether pages using these attributes will be rendered in
		/// color or monochrome. This attribute is updated to the value chosen
		/// by the user.
		/// </summary>
		/// <returns>  ColorType.COLOR or ColorType.MONOCHROME. </returns>
		public ColorType Color
		{
			get
			{
				return Color_Renamed;
			}
			set
			{
				if (value == null)
				{
					throw new IllegalArgumentException("Invalid value for attribute " + "color");
				}
				this.Color_Renamed = value;
			}
		}


		/// <summary>
		/// Returns the paper size for pages using these attributes. This
		/// attribute is updated to the value chosen by the user.
		/// </summary>
		/// <returns>  one of the constant fields of the MediaType class. </returns>
		public MediaType Media
		{
			get
			{
				return Media_Renamed;
			}
			set
			{
				if (value == null)
				{
					throw new IllegalArgumentException("Invalid value for attribute " + "media");
				}
				this.Media_Renamed = value;
			}
		}


		/// <summary>
		/// Sets the paper size for pages using these attributes to the default
		/// size for the default locale. The default size for locales in the
		/// United States and Canada is MediaType.NA_LETTER. The default size for
		/// all other locales is MediaType.ISO_A4.
		/// </summary>
		public void SetMediaToDefault()
		{
			String defaultCountry = Locale.Default.Country;
			if (defaultCountry != null && (defaultCountry.Equals(Locale.US.Country) || defaultCountry.Equals(Locale.CANADA.Country)))
			{
				Media = MediaType.NA_LETTER;
			}
			else
			{
				Media = MediaType.ISO_A4;
			}
		}

		/// <summary>
		/// Returns the print orientation for pages using these attributes. This
		/// attribute is updated to the value chosen by the user.
		/// </summary>
		/// <returns>  OrientationRequestedType.PORTRAIT or
		///          OrientationRequestedType.LANDSCAPE. </returns>
		public OrientationRequestedType GetOrientationRequested()
		{
			return OrientationRequested_Renamed;
		}

		/// <summary>
		/// Specifies the print orientation for pages using these attributes. Not
		/// specifying the property is equivalent to specifying
		/// OrientationRequestedType.PORTRAIT.
		/// </summary>
		/// <param name="orientationRequested"> OrientationRequestedType.PORTRAIT or
		///          OrientationRequestedType.LANDSCAPE. </param>
		/// <exception cref="IllegalArgumentException"> if orientationRequested is null. </exception>
		public void SetOrientationRequested(OrientationRequestedType orientationRequested)
		{
			if (orientationRequested == null)
			{
				throw new IllegalArgumentException("Invalid value for attribute " + "orientationRequested");
			}
			this.OrientationRequested_Renamed = orientationRequested;
		}

		/// <summary>
		/// Specifies the print orientation for pages using these attributes.
		/// Specifying <code>3</code> denotes portrait. Specifying <code>4</code>
		/// denotes landscape. Specifying any other value will generate an
		/// IllegalArgumentException. Not specifying the property is equivalent
		/// to calling setOrientationRequested(OrientationRequestedType.PORTRAIT).
		/// </summary>
		/// <param name="orientationRequested"> <code>3</code> or <code>4</code> </param>
		/// <exception cref="IllegalArgumentException"> if orientationRequested is not
		///          <code>3</code> or <code>4</code> </exception>
		public void SetOrientationRequested(int orientationRequested)
		{
			switch (orientationRequested)
			{
			  case 3:
				SetOrientationRequested(OrientationRequestedType.PORTRAIT);
				break;
			  case 4:
				SetOrientationRequested(OrientationRequestedType.LANDSCAPE);
				break;
			  default:
				// This will throw an IllegalArgumentException
				SetOrientationRequested(null);
				break;
			}
		}

		/// <summary>
		/// Sets the print orientation for pages using these attributes to the
		/// default. The default orientation is portrait.
		/// </summary>
		public void SetOrientationRequestedToDefault()
		{
			SetOrientationRequested(OrientationRequestedType.PORTRAIT);
		}

		/// <summary>
		/// Returns whether drawing at (0, 0) to pages using these attributes
		/// draws at the upper-left corner of the physical page, or at the
		/// upper-left corner of the printable area. (Note that these locations
		/// could be equivalent.) This attribute cannot be modified by,
		/// and is not subject to any limitations of, the implementation or the
		/// target printer.
		/// </summary>
		/// <returns>  OriginType.PHYSICAL or OriginType.PRINTABLE </returns>
		public OriginType Origin
		{
			get
			{
				return Origin_Renamed;
			}
			set
			{
				if (value == null)
				{
					throw new IllegalArgumentException("Invalid value for attribute " + "origin");
				}
				this.Origin_Renamed = value;
			}
		}


		/// <summary>
		/// Returns the print quality for pages using these attributes. This
		/// attribute is updated to the value chosen by the user.
		/// </summary>
		/// <returns>  PrintQualityType.DRAFT, PrintQualityType.NORMAL, or
		///          PrintQualityType.HIGH </returns>
		public PrintQualityType GetPrintQuality()
		{
			return PrintQuality_Renamed;
		}

		/// <summary>
		/// Specifies the print quality for pages using these attributes. Not
		/// specifying the property is equivalent to specifying
		/// PrintQualityType.NORMAL.
		/// </summary>
		/// <param name="printQuality"> PrintQualityType.DRAFT, PrintQualityType.NORMAL,
		///          or PrintQualityType.HIGH </param>
		/// <exception cref="IllegalArgumentException"> if printQuality is null. </exception>
		public void SetPrintQuality(PrintQualityType printQuality)
		{
			if (printQuality == null)
			{
				throw new IllegalArgumentException("Invalid value for attribute " + "printQuality");
			}
			this.PrintQuality_Renamed = printQuality;
		}

		/// <summary>
		/// Specifies the print quality for pages using these attributes.
		/// Specifying <code>3</code> denotes draft. Specifying <code>4</code>
		/// denotes normal. Specifying <code>5</code> denotes high. Specifying
		/// any other value will generate an IllegalArgumentException. Not
		/// specifying the property is equivalent to calling
		/// setPrintQuality(PrintQualityType.NORMAL).
		/// </summary>
		/// <param name="printQuality"> <code>3</code>, <code>4</code>, or <code>5</code> </param>
		/// <exception cref="IllegalArgumentException"> if printQuality is not <code>3
		///          </code>, <code>4</code>, or <code>5</code> </exception>
		public void SetPrintQuality(int printQuality)
		{
			switch (printQuality)
			{
			  case 3:
				SetPrintQuality(PrintQualityType.DRAFT);
				break;
			  case 4:
				SetPrintQuality(PrintQualityType.NORMAL);
				break;
			  case 5:
				SetPrintQuality(PrintQualityType.HIGH);
				break;
			  default:
				// This will throw an IllegalArgumentException
				SetPrintQuality(null);
				break;
			}
		}

		/// <summary>
		/// Sets the print quality for pages using these attributes to the default.
		/// The default print quality is normal.
		/// </summary>
		public void SetPrintQualityToDefault()
		{
			SetPrintQuality(PrintQualityType.NORMAL);
		}

		/// <summary>
		/// Returns the print resolution for pages using these attributes.
		/// Index 0 of the array specifies the cross feed direction resolution
		/// (typically the horizontal resolution). Index 1 of the array specifies
		/// the feed direction resolution (typically the vertical resolution).
		/// Index 2 of the array specifies whether the resolutions are in dots per
		/// inch or dots per centimeter. <code>3</code> denotes dots per inch.
		/// <code>4</code> denotes dots per centimeter.
		/// </summary>
		/// <returns>  an integer array of 3 elements. The first
		///          element must be greater than 0. The second element must be
		///          must be greater than 0. The third element must be either
		///          <code>3</code> or <code>4</code>. </returns>
		public int[] PrinterResolution
		{
			get
			{
				// Return a copy because otherwise client code could circumvent the
				// the checks made in setPrinterResolution by modifying the
				// returned array.
				int[] copy = new int[3];
				copy[0] = PrinterResolution_Renamed[0];
				copy[1] = PrinterResolution_Renamed[1];
				copy[2] = PrinterResolution_Renamed[2];
				return copy;
			}
			set
			{
				if (value == null || value.Length != 3 || value[0] <= 0 || value[1] <= 0 || (value[2] != 3 && value[2] != 4))
				{
					throw new IllegalArgumentException("Invalid value for attribute " + "printerResolution");
				}
				// Store a copy because otherwise client code could circumvent the
				// the checks made above by holding a reference to the array and
				// modifying it after calling setPrinterResolution.
				int[] copy = new int[3];
				copy[0] = value[0];
				copy[1] = value[1];
				copy[2] = value[2];
				this.PrinterResolution_Renamed = copy;
			}
		}


		/// <summary>
		/// Specifies the desired cross feed and feed print resolutions in dots per
		/// inch for pages using these attributes. The same value is used for both
		/// resolutions. The actual resolutions will be determined by the
		/// limitations of the implementation and the target printer. Not
		/// specifying the property is equivalent to specifying <code>72</code>.
		/// </summary>
		/// <param name="printerResolution"> an integer greater than 0. </param>
		/// <exception cref="IllegalArgumentException"> if printerResolution is less than or
		///          equal to 0. </exception>
		public int PrinterResolution
		{
			set
			{
				PrinterResolution = new int[] {value, value, 3};
			}
		}

		/// <summary>
		/// Sets the printer resolution for pages using these attributes to the
		/// default. The default is 72 dpi for both the feed and cross feed
		/// resolutions.
		/// </summary>
		public void SetPrinterResolutionToDefault()
		{
			PrinterResolution = 72;
		}

		/// <summary>
		/// Determines whether two PageAttributes are equal to each other.
		/// <para>
		/// Two PageAttributes are equal if and only if each of their attributes are
		/// equal. Attributes of enumeration type are equal if and only if the
		/// fields refer to the same unique enumeration object. This means that
		/// an aliased media is equal to its underlying unique media. Printer
		/// resolutions are equal if and only if the feed resolution, cross feed
		/// resolution, and units are equal.
		/// 
		/// </para>
		/// </summary>
		/// <param name="obj"> the object whose equality will be checked. </param>
		/// <returns>  whether obj is equal to this PageAttribute according to the
		///          above criteria. </returns>
		public override bool Equals(Object obj)
		{
			if (!(obj is PageAttributes))
			{
				return false;
			}

			PageAttributes rhs = (PageAttributes)obj;

			return (Color_Renamed == rhs.Color_Renamed && Media_Renamed == rhs.Media_Renamed && OrientationRequested_Renamed == rhs.OrientationRequested_Renamed && Origin_Renamed == rhs.Origin_Renamed && PrintQuality_Renamed == rhs.PrintQuality_Renamed && PrinterResolution_Renamed[0] == rhs.PrinterResolution_Renamed[0] && PrinterResolution_Renamed[1] == rhs.PrinterResolution_Renamed[1] && PrinterResolution_Renamed[2] == rhs.PrinterResolution_Renamed[2]);
		}

		/// <summary>
		/// Returns a hash code value for this PageAttributes.
		/// </summary>
		/// <returns>  the hash code. </returns>
		public override int HashCode()
		{
			return (Color_Renamed.HashCode() << 31 ^ Media_Renamed.HashCode() << 24 ^ OrientationRequested_Renamed.HashCode() << 23 ^ Origin_Renamed.HashCode() << 22 ^ PrintQuality_Renamed.HashCode() << 20 ^ PrinterResolution_Renamed[2] >> 2 << 19 ^ PrinterResolution_Renamed[1] << 10 ^ PrinterResolution_Renamed[0]);
		}

		/// <summary>
		/// Returns a string representation of this PageAttributes.
		/// </summary>
		/// <returns>  the string representation. </returns>
		public override String ToString()
		{
			// int[] printerResolution = getPrinterResolution();
			return "color=" + Color + ",media=" + Media + ",orientation-requested=" + GetOrientationRequested() + ",origin=" + Origin + ",print-quality=" + GetPrintQuality() + ",printer-resolution=[" + PrinterResolution_Renamed[0] + "," + PrinterResolution_Renamed[1] + "," + PrinterResolution_Renamed[2] + "]";
		}
	}

}
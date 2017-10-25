using System;

/*
 * Copyright (c) 1996, 2014, Oracle and/or its affiliates. All rights reserved.
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

	using AWTAccessor = sun.awt.AWTAccessor;


	/// <summary>
	/// A class to encapsulate symbolic colors representing the color of
	/// native GUI objects on a system.  For systems which support the dynamic
	/// update of the system colors (when the user changes the colors)
	/// the actual RGB values of these symbolic colors will also change
	/// dynamically.  In order to compare the "current" RGB value of a
	/// <code>SystemColor</code> object with a non-symbolic Color object,
	/// <code>getRGB</code> should be used rather than <code>equals</code>.
	/// <para>
	/// Note that the way in which these system colors are applied to GUI objects
	/// may vary slightly from platform to platform since GUI objects may be
	/// rendered differently on each platform.
	/// </para>
	/// <para>
	/// System color values may also be available through the <code>getDesktopProperty</code>
	/// method on <code>java.awt.Toolkit</code>.
	/// 
	/// </para>
	/// </summary>
	/// <seealso cref= Toolkit#getDesktopProperty
	/// 
	/// @author      Carl Quinn
	/// @author      Amy Fowler </seealso>
	[Serializable]
	public sealed class SystemColor : Color
	{

	   /// <summary>
	   /// The array index for the
	   /// <seealso cref="#desktop"/> system color. </summary>
	   /// <seealso cref= SystemColor#desktop </seealso>
//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Native public final static int DESKTOP = 0;
		public const int DESKTOP = 0;

		/// <summary>
		/// The array index for the
		/// <seealso cref="#activeCaption"/> system color. </summary>
		/// <seealso cref= SystemColor#activeCaption </seealso>
//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Native public final static int ACTIVE_CAPTION = 1;
		public const int ACTIVE_CAPTION = 1;

		/// <summary>
		/// The array index for the
		/// <seealso cref="#activeCaptionText"/> system color. </summary>
		/// <seealso cref= SystemColor#activeCaptionText </seealso>
//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Native public final static int ACTIVE_CAPTION_TEXT = 2;
		public const int ACTIVE_CAPTION_TEXT = 2;

		/// <summary>
		/// The array index for the
		/// <seealso cref="#activeCaptionBorder"/> system color. </summary>
		/// <seealso cref= SystemColor#activeCaptionBorder </seealso>
//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Native public final static int ACTIVE_CAPTION_BORDER = 3;
		public const int ACTIVE_CAPTION_BORDER = 3;

		/// <summary>
		/// The array index for the
		/// <seealso cref="#inactiveCaption"/> system color. </summary>
		/// <seealso cref= SystemColor#inactiveCaption </seealso>
//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Native public final static int INACTIVE_CAPTION = 4;
		public const int INACTIVE_CAPTION = 4;

		/// <summary>
		/// The array index for the
		/// <seealso cref="#inactiveCaptionText"/> system color. </summary>
		/// <seealso cref= SystemColor#inactiveCaptionText </seealso>
//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Native public final static int INACTIVE_CAPTION_TEXT = 5;
		public const int INACTIVE_CAPTION_TEXT = 5;

		/// <summary>
		/// The array index for the
		/// <seealso cref="#inactiveCaptionBorder"/> system color. </summary>
		/// <seealso cref= SystemColor#inactiveCaptionBorder </seealso>
//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Native public final static int INACTIVE_CAPTION_BORDER = 6;
		public const int INACTIVE_CAPTION_BORDER = 6;

		/// <summary>
		/// The array index for the
		/// <seealso cref="#window"/> system color. </summary>
		/// <seealso cref= SystemColor#window </seealso>
//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Native public final static int WINDOW = 7;
		public const int WINDOW = 7;

		/// <summary>
		/// The array index for the
		/// <seealso cref="#windowBorder"/> system color. </summary>
		/// <seealso cref= SystemColor#windowBorder </seealso>
//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Native public final static int WINDOW_BORDER = 8;
		public const int WINDOW_BORDER = 8;

		/// <summary>
		/// The array index for the
		/// <seealso cref="#windowText"/> system color. </summary>
		/// <seealso cref= SystemColor#windowText </seealso>
//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Native public final static int WINDOW_TEXT = 9;
		public const int WINDOW_TEXT = 9;

		/// <summary>
		/// The array index for the
		/// <seealso cref="#menu"/> system color. </summary>
		/// <seealso cref= SystemColor#menu </seealso>
//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Native public final static int MENU = 10;
		public const int MENU = 10;

		/// <summary>
		/// The array index for the
		/// <seealso cref="#menuText"/> system color. </summary>
		/// <seealso cref= SystemColor#menuText </seealso>
//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Native public final static int MENU_TEXT = 11;
		public const int MENU_TEXT = 11;

		/// <summary>
		/// The array index for the
		/// <seealso cref="#text"/> system color. </summary>
		/// <seealso cref= SystemColor#text </seealso>
//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Native public final static int TEXT = 12;
		public const int TEXT = 12;

		/// <summary>
		/// The array index for the
		/// <seealso cref="#textText"/> system color. </summary>
		/// <seealso cref= SystemColor#textText </seealso>
//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Native public final static int TEXT_TEXT = 13;
		public const int TEXT_TEXT = 13;

		/// <summary>
		/// The array index for the
		/// <seealso cref="#textHighlight"/> system color. </summary>
		/// <seealso cref= SystemColor#textHighlight </seealso>
//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Native public final static int TEXT_HIGHLIGHT = 14;
		public const int TEXT_HIGHLIGHT = 14;

		/// <summary>
		/// The array index for the
		/// <seealso cref="#textHighlightText"/> system color. </summary>
		/// <seealso cref= SystemColor#textHighlightText </seealso>
//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Native public final static int TEXT_HIGHLIGHT_TEXT = 15;
		public const int TEXT_HIGHLIGHT_TEXT = 15;

		/// <summary>
		/// The array index for the
		/// <seealso cref="#textInactiveText"/> system color. </summary>
		/// <seealso cref= SystemColor#textInactiveText </seealso>
//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Native public final static int TEXT_INACTIVE_TEXT = 16;
		public const int TEXT_INACTIVE_TEXT = 16;

		/// <summary>
		/// The array index for the
		/// <seealso cref="#control"/> system color. </summary>
		/// <seealso cref= SystemColor#control </seealso>
//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Native public final static int CONTROL = 17;
		public const int CONTROL = 17;

		/// <summary>
		/// The array index for the
		/// <seealso cref="#controlText"/> system color. </summary>
		/// <seealso cref= SystemColor#controlText </seealso>
//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Native public final static int CONTROL_TEXT = 18;
		public const int CONTROL_TEXT = 18;

		/// <summary>
		/// The array index for the
		/// <seealso cref="#controlHighlight"/> system color. </summary>
		/// <seealso cref= SystemColor#controlHighlight </seealso>
//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Native public final static int CONTROL_HIGHLIGHT = 19;
		public const int CONTROL_HIGHLIGHT = 19;

		/// <summary>
		/// The array index for the
		/// <seealso cref="#controlLtHighlight"/> system color. </summary>
		/// <seealso cref= SystemColor#controlLtHighlight </seealso>
//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Native public final static int CONTROL_LT_HIGHLIGHT = 20;
		public const int CONTROL_LT_HIGHLIGHT = 20;

		/// <summary>
		/// The array index for the
		/// <seealso cref="#controlShadow"/> system color. </summary>
		/// <seealso cref= SystemColor#controlShadow </seealso>
//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Native public final static int CONTROL_SHADOW = 21;
		public const int CONTROL_SHADOW = 21;

		/// <summary>
		/// The array index for the
		/// <seealso cref="#controlDkShadow"/> system color. </summary>
		/// <seealso cref= SystemColor#controlDkShadow </seealso>
//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Native public final static int CONTROL_DK_SHADOW = 22;
		public const int CONTROL_DK_SHADOW = 22;

		/// <summary>
		/// The array index for the
		/// <seealso cref="#scrollbar"/> system color. </summary>
		/// <seealso cref= SystemColor#scrollbar </seealso>
//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Native public final static int SCROLLBAR = 23;
		public const int SCROLLBAR = 23;

		/// <summary>
		/// The array index for the
		/// <seealso cref="#info"/> system color. </summary>
		/// <seealso cref= SystemColor#info </seealso>
//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Native public final static int INFO = 24;
		public const int INFO = 24;

		/// <summary>
		/// The array index for the
		/// <seealso cref="#infoText"/> system color. </summary>
		/// <seealso cref= SystemColor#infoText </seealso>
//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Native public final static int INFO_TEXT = 25;
		public const int INFO_TEXT = 25;

		/// <summary>
		/// The number of system colors in the array.
		/// </summary>
//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Native public final static int NUM_COLORS = 26;
		public const int NUM_COLORS = 26;

		/// <summary>
		///*************************************************************************************** </summary>

		/*
		 * System colors with default initial values, overwritten by toolkit if
		 * system values differ and are available.
		 * Should put array initialization above first field that is using
		 * SystemColor constructor to initialize.
		 */
		private static int[] SystemColors = new int[] {unchecked((int)0xFF005C5C), unchecked((int)0xFF000080), unchecked((int)0xFFFFFFFF), unchecked((int)0xFFC0C0C0), unchecked((int)0xFF808080), unchecked((int)0xFFC0C0C0), unchecked((int)0xFFC0C0C0), unchecked((int)0xFFFFFFFF), unchecked((int)0xFF000000), unchecked((int)0xFF000000), unchecked((int)0xFFC0C0C0), unchecked((int)0xFF000000), unchecked((int)0xFFC0C0C0), unchecked((int)0xFF000000), unchecked((int)0xFF000080), unchecked((int)0xFFFFFFFF), unchecked((int)0xFF808080), unchecked((int)0xFFC0C0C0), unchecked((int)0xFF000000), unchecked((int)0xFFFFFFFF), unchecked((int)0xFFE0E0E0), unchecked((int)0xFF808080), unchecked((int)0xFF000000), unchecked((int)0xFFE0E0E0), unchecked((int)0xFFE0E000), unchecked((int)0xFF000000)};

	   /// <summary>
	   /// The color rendered for the background of the desktop.
	   /// </summary>
		public static readonly SystemColor Desktop = new SystemColor((sbyte)DESKTOP);

		/// <summary>
		/// The color rendered for the window-title background of the currently active window.
		/// </summary>
		public static readonly SystemColor ActiveCaption = new SystemColor((sbyte)ACTIVE_CAPTION);

		/// <summary>
		/// The color rendered for the window-title text of the currently active window.
		/// </summary>
		public static readonly SystemColor ActiveCaptionText = new SystemColor((sbyte)ACTIVE_CAPTION_TEXT);

		/// <summary>
		/// The color rendered for the border around the currently active window.
		/// </summary>
		public static readonly SystemColor ActiveCaptionBorder = new SystemColor((sbyte)ACTIVE_CAPTION_BORDER);

		/// <summary>
		/// The color rendered for the window-title background of inactive windows.
		/// </summary>
		public static readonly SystemColor InactiveCaption = new SystemColor((sbyte)INACTIVE_CAPTION);

		/// <summary>
		/// The color rendered for the window-title text of inactive windows.
		/// </summary>
		public static readonly SystemColor InactiveCaptionText = new SystemColor((sbyte)INACTIVE_CAPTION_TEXT);

		/// <summary>
		/// The color rendered for the border around inactive windows.
		/// </summary>
		public static readonly SystemColor InactiveCaptionBorder = new SystemColor((sbyte)INACTIVE_CAPTION_BORDER);

		/// <summary>
		/// The color rendered for the background of interior regions inside windows.
		/// </summary>
		public static readonly SystemColor Window = new SystemColor((sbyte)WINDOW);

		/// <summary>
		/// The color rendered for the border around interior regions inside windows.
		/// </summary>
		public static readonly SystemColor WindowBorder = new SystemColor((sbyte)WINDOW_BORDER);

		/// <summary>
		/// The color rendered for text of interior regions inside windows.
		/// </summary>
		public static readonly SystemColor WindowText = new SystemColor((sbyte)WINDOW_TEXT);

		/// <summary>
		/// The color rendered for the background of menus.
		/// </summary>
		public static readonly SystemColor Menu = new SystemColor((sbyte)MENU);

		/// <summary>
		/// The color rendered for the text of menus.
		/// </summary>
		public static readonly SystemColor MenuText = new SystemColor((sbyte)MENU_TEXT);

		/// <summary>
		/// The color rendered for the background of text control objects, such as
		/// textfields and comboboxes.
		/// </summary>
		public static readonly SystemColor Text = new SystemColor((sbyte)TEXT);

		/// <summary>
		/// The color rendered for the text of text control objects, such as textfields
		/// and comboboxes.
		/// </summary>
		public static readonly SystemColor TextText = new SystemColor((sbyte)TEXT_TEXT);

		/// <summary>
		/// The color rendered for the background of selected items, such as in menus,
		/// comboboxes, and text.
		/// </summary>
		public static readonly SystemColor TextHighlight = new SystemColor((sbyte)TEXT_HIGHLIGHT);

		/// <summary>
		/// The color rendered for the text of selected items, such as in menus, comboboxes,
		/// and text.
		/// </summary>
		public static readonly SystemColor TextHighlightText = new SystemColor((sbyte)TEXT_HIGHLIGHT_TEXT);

		/// <summary>
		/// The color rendered for the text of inactive items, such as in menus.
		/// </summary>
		public static readonly SystemColor TextInactiveText = new SystemColor((sbyte)TEXT_INACTIVE_TEXT);

		/// <summary>
		/// The color rendered for the background of control panels and control objects,
		/// such as pushbuttons.
		/// </summary>
		public static readonly SystemColor Control = new SystemColor((sbyte)CONTROL);

		/// <summary>
		/// The color rendered for the text of control panels and control objects,
		/// such as pushbuttons.
		/// </summary>
		public static readonly SystemColor ControlText = new SystemColor((sbyte)CONTROL_TEXT);

		/// <summary>
		/// The color rendered for light areas of 3D control objects, such as pushbuttons.
		/// This color is typically derived from the <code>control</code> background color
		/// to provide a 3D effect.
		/// </summary>
		public static readonly SystemColor ControlHighlight = new SystemColor((sbyte)CONTROL_HIGHLIGHT);

		/// <summary>
		/// The color rendered for highlight areas of 3D control objects, such as pushbuttons.
		/// This color is typically derived from the <code>control</code> background color
		/// to provide a 3D effect.
		/// </summary>
		public static readonly SystemColor ControlLtHighlight = new SystemColor((sbyte)CONTROL_LT_HIGHLIGHT);

		/// <summary>
		/// The color rendered for shadow areas of 3D control objects, such as pushbuttons.
		/// This color is typically derived from the <code>control</code> background color
		/// to provide a 3D effect.
		/// </summary>
		public static readonly SystemColor ControlShadow = new SystemColor((sbyte)CONTROL_SHADOW);

		/// <summary>
		/// The color rendered for dark shadow areas on 3D control objects, such as pushbuttons.
		/// This color is typically derived from the <code>control</code> background color
		/// to provide a 3D effect.
		/// </summary>
		public static readonly SystemColor ControlDkShadow = new SystemColor((sbyte)CONTROL_DK_SHADOW);

		/// <summary>
		/// The color rendered for the background of scrollbars.
		/// </summary>
		public static readonly SystemColor Scrollbar = new SystemColor((sbyte)SCROLLBAR);

		/// <summary>
		/// The color rendered for the background of tooltips or spot help.
		/// </summary>
		public static readonly SystemColor Info = new SystemColor((sbyte)INFO);

		/// <summary>
		/// The color rendered for the text of tooltips or spot help.
		/// </summary>
		public static readonly SystemColor InfoText = new SystemColor((sbyte)INFO_TEXT);

		/*
		 * JDK 1.1 serialVersionUID.
		 */
		private const long SerialVersionUID = 4503142729533789064L;

		/*
		 * An index into either array of SystemColor objects or values.
		 */
		[NonSerialized]
		private int Index;

		private static SystemColor[] SystemColorObjects = new SystemColor[] {SystemColor.Desktop, SystemColor.ActiveCaption, SystemColor.ActiveCaptionText, SystemColor.ActiveCaptionBorder, SystemColor.InactiveCaption, SystemColor.InactiveCaptionText, SystemColor.InactiveCaptionBorder, SystemColor.Window, SystemColor.WindowBorder, SystemColor.WindowText, SystemColor.Menu, SystemColor.MenuText, SystemColor.Text, SystemColor.TextText, SystemColor.TextHighlight, SystemColor.TextHighlightText, SystemColor.TextInactiveText, SystemColor.Control, SystemColor.ControlText, SystemColor.ControlHighlight, SystemColor.ControlLtHighlight, SystemColor.ControlShadow, SystemColor.ControlDkShadow, SystemColor.Scrollbar, SystemColor.Info, SystemColor.InfoText};

		static SystemColor()
		{
			AWTAccessor.SystemColorAccessor = SystemColor::updateSystemColors;
			UpdateSystemColors();
		}

		/// <summary>
		/// Called from {@code <init>} and toolkit to update the above systemColors cache.
		/// </summary>
		private static void UpdateSystemColors()
		{
			if (!GraphicsEnvironment.Headless)
			{
				Toolkit.DefaultToolkit.LoadSystemColors(SystemColors);
			}
			for (int i = 0; i < SystemColors.Length; i++)
			{
				SystemColorObjects[i].Value = SystemColors[i];
			}
		}

		/// <summary>
		/// Creates a symbolic color that represents an indexed entry into system
		/// color cache. Used by above static system colors.
		/// </summary>
		private SystemColor(sbyte index) : base(SystemColors[index])
		{
			this.Index = index;
		}

		/// <summary>
		/// Returns a string representation of this <code>Color</code>'s values.
		/// This method is intended to be used only for debugging purposes,
		/// and the content and format of the returned string may vary between
		/// implementations.
		/// The returned string may be empty but may not be <code>null</code>.
		/// </summary>
		/// <returns>  a string representation of this <code>Color</code> </returns>
		public override String ToString()
		{
//JAVA TO C# CONVERTER WARNING: The .NET Type.FullName property will not always yield results identical to the Java Class.getName method:
			return this.GetType().FullName + "[i=" + (Index) + "]";
		}

		/// <summary>
		/// The design of the {@code SystemColor} class assumes that
		/// the {@code SystemColor} object instances stored in the
		/// static final fields above are the only instances that can
		/// be used by developers.
		/// This method helps maintain those limits on instantiation
		/// by using the index stored in the value field of the
		/// serialized form of the object to replace the serialized
		/// object with the equivalent static object constant field
		/// of {@code SystemColor}.
		/// See the <seealso cref="#writeReplace"/> method for more information
		/// on the serialized form of these objects. </summary>
		/// <returns> one of the {@code SystemColor} static object
		///         fields that refers to the same system color. </returns>
		private Object ReadResolve()
		{
			// The instances of SystemColor are tightly controlled and
			// only the canonical instances appearing above as static
			// constants are allowed.  The serial form of SystemColor
			// objects stores the color index as the value.  Here we
			// map that index back into the canonical instance.
			return SystemColorObjects[Value];
		}

		/// <summary>
		/// Returns a specialized version of the {@code SystemColor}
		/// object for writing to the serialized stream.
		/// @serialData
		/// The value field of a serialized {@code SystemColor} object
		/// contains the array index of the system color instead of the
		/// rgb data for the system color.
		/// This index is used by the <seealso cref="#readResolve"/> method to
		/// resolve the deserialized objects back to the original
		/// static constant versions to ensure unique instances of
		/// each {@code SystemColor} object. </summary>
		/// <returns> a proxy {@code SystemColor} object with its value
		///         replaced by the corresponding system color index. </returns>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: private Object writeReplace() throws java.io.ObjectStreamException
		private Object WriteReplace()
		{
			// we put an array index in the SystemColor.value while serialize
			// to keep compatibility.
			SystemColor color = new SystemColor((sbyte)Index);
			color.Value = Index;
			return color;
		}
	}

}
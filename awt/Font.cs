using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;

/*
 * Copyright (c) 1995, 2013, Oracle and/or its affiliates. All rights reserved.
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

	using StandardGlyphVector = sun.font.StandardGlyphVector;

	using AttributeMap = sun.font.AttributeMap;
	using AttributeValues = sun.font.AttributeValues;
	using CompositeFont = sun.font.CompositeFont;
	using CreatedFontTracker = sun.font.CreatedFontTracker;
	using Font2D = sun.font.Font2D;
	using Font2DHandle = sun.font.Font2DHandle;
	using FontAccess = sun.font.FontAccess;
	using FontManager = sun.font.FontManager;
	using FontManagerFactory = sun.font.FontManagerFactory;
	using FontUtilities = sun.font.FontUtilities;
	using GlyphLayout = sun.font.GlyphLayout;
	using FontLineMetrics = sun.font.FontLineMetrics;
	using CoreMetrics = sun.font.CoreMetrics;

//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to .NET:
	import static sun.font.EAttribute.*;

	/// <summary>
	/// The <code>Font</code> class represents fonts, which are used to
	/// render text in a visible way.
	/// A font provides the information needed to map sequences of
	/// <em>characters</em> to sequences of <em>glyphs</em>
	/// and to render sequences of glyphs on <code>Graphics</code> and
	/// <code>Component</code> objects.
	/// 
	/// <h3>Characters and Glyphs</h3>
	/// 
	/// A <em>character</em> is a symbol that represents an item such as a letter,
	/// a digit, or punctuation in an abstract way. For example, <code>'g'</code>,
	/// LATIN SMALL LETTER G, is a character.
	/// <para>
	/// A <em>glyph</em> is a shape used to render a character or a sequence of
	/// characters. In simple writing systems, such as Latin, typically one glyph
	/// represents one character. In general, however, characters and glyphs do not
	/// have one-to-one correspondence. For example, the character '&aacute;'
	/// LATIN SMALL LETTER A WITH ACUTE, can be represented by
	/// two glyphs: one for 'a' and one for '&acute;'. On the other hand, the
	/// two-character string "fi" can be represented by a single glyph, an
	/// "fi" ligature. In complex writing systems, such as Arabic or the South
	/// and South-East Asian writing systems, the relationship between characters
	/// and glyphs can be more complicated and involve context-dependent selection
	/// of glyphs as well as glyph reordering.
	/// 
	/// A font encapsulates the collection of glyphs needed to render a selected set
	/// of characters as well as the tables needed to map sequences of characters to
	/// corresponding sequences of glyphs.
	/// 
	/// <h3>Physical and Logical Fonts</h3>
	/// 
	/// The Java Platform distinguishes between two kinds of fonts:
	/// <em>physical</em> fonts and <em>logical</em> fonts.
	/// </para>
	/// <para>
	/// <em>Physical</em> fonts are the actual font libraries containing glyph data
	/// and tables to map from character sequences to glyph sequences, using a font
	/// technology such as TrueType or PostScript Type 1.
	/// All implementations of the Java Platform must support TrueType fonts;
	/// support for other font technologies is implementation dependent.
	/// Physical fonts may use names such as Helvetica, Palatino, HonMincho, or
	/// any number of other font names.
	/// Typically, each physical font supports only a limited set of writing
	/// systems, for example, only Latin characters or only Japanese and Basic
	/// Latin.
	/// The set of available physical fonts varies between configurations.
	/// Applications that require specific fonts can bundle them and instantiate
	/// them using the <seealso cref="#createFont createFont"/> method.
	/// </para>
	/// <para>
	/// <em>Logical</em> fonts are the five font families defined by the Java
	/// platform which must be supported by any Java runtime environment:
	/// Serif, SansSerif, Monospaced, Dialog, and DialogInput.
	/// These logical fonts are not actual font libraries. Instead, the logical
	/// font names are mapped to physical fonts by the Java runtime environment.
	/// The mapping is implementation and usually locale dependent, so the look
	/// and the metrics provided by them vary.
	/// Typically, each logical font name maps to several physical fonts in order to
	/// cover a large range of characters.
	/// </para>
	/// <para>
	/// Peered AWT components, such as <seealso cref="Label Label"/> and
	/// <seealso cref="TextField TextField"/>, can only use logical fonts.
	/// </para>
	/// <para>
	/// For a discussion of the relative advantages and disadvantages of using
	/// physical or logical fonts, see the
	/// <a href="http://www.oracle.com/technetwork/java/javase/tech/faq-jsp-138165.html">Internationalization FAQ</a>
	/// document.
	/// 
	/// <h3>Font Faces and Names</h3>
	/// 
	/// A <code>Font</code>
	/// can have many faces, such as heavy, medium, oblique, gothic and
	/// regular. All of these faces have similar typographic design.
	/// </para>
	/// <para>
	/// There are three different names that you can get from a
	/// <code>Font</code> object.  The <em>logical font name</em> is simply the
	/// name that was used to construct the font.
	/// The <em>font face name</em>, or just <em>font name</em> for
	/// short, is the name of a particular font face, like Helvetica Bold. The
	/// <em>family name</em> is the name of the font family that determines the
	/// typographic design across several faces, like Helvetica.
	/// </para>
	/// <para>
	/// The <code>Font</code> class represents an instance of a font face from
	/// a collection of  font faces that are present in the system resources
	/// of the host system.  As examples, Arial Bold and Courier Bold Italic
	/// are font faces.  There can be several <code>Font</code> objects
	/// associated with a font face, each differing in size, style, transform
	/// and font features.
	/// </para>
	/// <para>
	/// The <seealso cref="GraphicsEnvironment#getAllFonts() getAllFonts"/> method
	/// of the <code>GraphicsEnvironment</code> class returns an
	/// array of all font faces available in the system. These font faces are
	/// returned as <code>Font</code> objects with a size of 1, identity
	/// transform and default font features. These
	/// base fonts can then be used to derive new <code>Font</code> objects
	/// with varying sizes, styles, transforms and font features via the
	/// <code>deriveFont</code> methods in this class.
	/// 
	/// <h3>Font and TextAttribute</h3>
	/// 
	/// </para>
	/// <para><code>Font</code> supports most
	/// <code>TextAttribute</code>s.  This makes some operations, such as
	/// rendering underlined text, convenient since it is not
	/// necessary to explicitly construct a <code>TextLayout</code> object.
	/// Attributes can be set on a Font by constructing or deriving it
	/// using a <code>Map</code> of <code>TextAttribute</code> values.
	/// 
	/// </para>
	/// <para>The values of some <code>TextAttributes</code> are not
	/// serializable, and therefore attempting to serialize an instance of
	/// <code>Font</code> that has such values will not serialize them.
	/// This means a Font deserialized from such a stream will not compare
	/// equal to the original Font that contained the non-serializable
	/// attributes.  This should very rarely pose a problem
	/// since these attributes are typically used only in special
	/// circumstances and are unlikely to be serialized.
	/// 
	/// <ul>
	/// <li><code>FOREGROUND</code> and <code>BACKGROUND</code> use
	/// <code>Paint</code> values. The subclass <code>Color</code> is
	/// serializable, while <code>GradientPaint</code> and
	/// <code>TexturePaint</code> are not.</li>
	/// <li><code>CHAR_REPLACEMENT</code> uses
	/// <code>GraphicAttribute</code> values.  The subclasses
	/// <code>ShapeGraphicAttribute</code> and
	/// <code>ImageGraphicAttribute</code> are not serializable.</li>
	/// <li><code>INPUT_METHOD_HIGHLIGHT</code> uses
	/// <code>InputMethodHighlight</code> values, which are
	/// not serializable.  See <seealso cref="java.awt.im.InputMethodHighlight"/>.</li>
	/// </ul>
	/// 
	/// </para>
	/// <para>Clients who create custom subclasses of <code>Paint</code> and
	/// <code>GraphicAttribute</code> can make them serializable and
	/// avoid this problem.  Clients who use input method highlights can
	/// convert these to the platform-specific attributes for that
	/// highlight on the current platform and set them on the Font as
	/// a workaround.
	/// 
	/// </para>
	/// <para>The <code>Map</code>-based constructor and
	/// <code>deriveFont</code> APIs ignore the FONT attribute, and it is
	/// not retained by the Font; the static <seealso cref="#getFont"/> method should
	/// be used if the FONT attribute might be present.  See {@link
	/// java.awt.font.TextAttribute#FONT} for more information.</para>
	/// 
	/// <para>Several attributes will cause additional rendering overhead
	/// and potentially invoke layout.  If a <code>Font</code> has such
	/// attributes, the <code><seealso cref="#hasLayoutAttributes()"/></code> method
	/// will return true.</para>
	/// 
	/// <para>Note: Font rotations can cause text baselines to be rotated.  In
	/// order to account for this (rare) possibility, font APIs are
	/// specified to return metrics and take parameters 'in
	/// baseline-relative coordinates'.  This maps the 'x' coordinate to
	/// the advance along the baseline, (positive x is forward along the
	/// baseline), and the 'y' coordinate to a distance along the
	/// perpendicular to the baseline at 'x' (positive y is 90 degrees
	/// clockwise from the baseline vector).  APIs for which this is
	/// especially important are called out as having 'baseline-relative
	/// coordinates.'
	/// </para>
	/// </summary>
	[Serializable]
	public class Font
	{
		private class FontAccessImpl : FontAccess
		{
			public virtual Font2D GetFont2D(Font font)
			{
				return font.Font2D;
			}

			public virtual void SetFont2D(Font font, Font2DHandle handle)
			{
				font.Font2DHandle = handle;
			}

			public virtual Font CreatedFont
			{
				set
				{
					value.CreatedFont = true;
				}
			}

			public virtual bool IsCreatedFont(Font font)
			{
				return font.CreatedFont;
			}
		}

		static Font()
		{
			/* ensure that the necessary native libraries are loaded */
			Toolkit.LoadLibraries();
			initIDs();
			FontAccess.FontAccess = new FontAccessImpl();
		}

		/// <summary>
		/// This is now only used during serialization.  Typically
		/// it is null.
		/// 
		/// @serial </summary>
		/// <seealso cref= #getAttributes() </seealso>
		private Dictionary<Object, Object> FRequestedAttributes;

		/*
		 * Constants to be used for logical font family names.
		 */

		/// <summary>
		/// A String constant for the canonical family name of the
		/// logical font "Dialog". It is useful in Font construction
		/// to provide compile-time verification of the name.
		/// @since 1.6
		/// </summary>
		public const String DIALOG = "Dialog";

		/// <summary>
		/// A String constant for the canonical family name of the
		/// logical font "DialogInput". It is useful in Font construction
		/// to provide compile-time verification of the name.
		/// @since 1.6
		/// </summary>
		public const String DIALOG_INPUT = "DialogInput";

		/// <summary>
		/// A String constant for the canonical family name of the
		/// logical font "SansSerif". It is useful in Font construction
		/// to provide compile-time verification of the name.
		/// @since 1.6
		/// </summary>
		public const String SANS_SERIF = "SansSerif";

		/// <summary>
		/// A String constant for the canonical family name of the
		/// logical font "Serif". It is useful in Font construction
		/// to provide compile-time verification of the name.
		/// @since 1.6
		/// </summary>
		public const String SERIF = "Serif";

		/// <summary>
		/// A String constant for the canonical family name of the
		/// logical font "Monospaced". It is useful in Font construction
		/// to provide compile-time verification of the name.
		/// @since 1.6
		/// </summary>
		public const String MONOSPACED = "Monospaced";

		/*
		 * Constants to be used for styles. Can be combined to mix
		 * styles.
		 */

		/// <summary>
		/// The plain style constant.
		/// </summary>
		public const int PLAIN = 0;

		/// <summary>
		/// The bold style constant.  This can be combined with the other style
		/// constants (except PLAIN) for mixed styles.
		/// </summary>
		public const int BOLD = 1;

		/// <summary>
		/// The italicized style constant.  This can be combined with the other
		/// style constants (except PLAIN) for mixed styles.
		/// </summary>
		public const int ITALIC = 2;

		/// <summary>
		/// The baseline used in most Roman scripts when laying out text.
		/// </summary>
		public const int ROMAN_BASELINE = 0;

		/// <summary>
		/// The baseline used in ideographic scripts like Chinese, Japanese,
		/// and Korean when laying out text.
		/// </summary>
		public const int CENTER_BASELINE = 1;

		/// <summary>
		/// The baseline used in Devanigiri and similar scripts when laying
		/// out text.
		/// </summary>
		public const int HANGING_BASELINE = 2;

		/// <summary>
		/// Identify a font resource of type TRUETYPE.
		/// Used to specify a TrueType font resource to the
		/// <seealso cref="#createFont"/> method.
		/// The TrueType format was extended to become the OpenType
		/// format, which adds support for fonts with Postscript outlines,
		/// this tag therefore references these fonts, as well as those
		/// with TrueType outlines.
		/// @since 1.3
		/// </summary>

		public const int TRUETYPE_FONT = 0;

		/// <summary>
		/// Identify a font resource of type TYPE1.
		/// Used to specify a Type1 font resource to the
		/// <seealso cref="#createFont"/> method.
		/// @since 1.5
		/// </summary>
		public const int TYPE1_FONT = 1;

		/// <summary>
		/// The logical name of this <code>Font</code>, as passed to the
		/// constructor.
		/// @since JDK1.0
		/// 
		/// @serial </summary>
		/// <seealso cref= #getName </seealso>
		protected internal String Name_Renamed;

		/// <summary>
		/// The style of this <code>Font</code>, as passed to the constructor.
		/// This style can be PLAIN, BOLD, ITALIC, or BOLD+ITALIC.
		/// @since JDK1.0
		/// 
		/// @serial </summary>
		/// <seealso cref= #getStyle() </seealso>
		protected internal int Style_Renamed;

		/// <summary>
		/// The point size of this <code>Font</code>, rounded to integer.
		/// @since JDK1.0
		/// 
		/// @serial </summary>
		/// <seealso cref= #getSize() </seealso>
		protected internal int Size_Renamed;

		/// <summary>
		/// The point size of this <code>Font</code> in <code>float</code>.
		/// 
		/// @serial </summary>
		/// <seealso cref= #getSize() </seealso>
		/// <seealso cref= #getSize2D() </seealso>
		protected internal float PointSize;

		/// <summary>
		/// The platform specific font information.
		/// </summary>
		[NonSerialized]
		private FontPeer Peer_Renamed;
		[NonSerialized]
		private long PData; // native JDK1.1 font pointer
		[NonSerialized]
		private Font2DHandle Font2DHandle;

		[NonSerialized]
		private AttributeValues Values;
		[NonSerialized]
		private bool HasLayoutAttributes_Renamed;

		/*
		 * If the origin of a Font is a created font then this attribute
		 * must be set on all derived fonts too.
		 */
		[NonSerialized]
		private bool CreatedFont = false;

		/*
		 * This is true if the font transform is not identity.  It
		 * is used to avoid unnecessary instantiation of an AffineTransform.
		 */
		[NonSerialized]
		private bool NonIdentityTx;

		/*
		 * A cached value used when a transform is required for internal
		 * use.  This must not be exposed to callers since AffineTransform
		 * is mutable.
		 */
		private static readonly AffineTransform IdentityTx = new AffineTransform();

		/*
		 * JDK 1.1 serialVersionUID
		 */
		private const long SerialVersionUID = -4206021311591459213L;

		/// <summary>
		/// Gets the peer of this <code>Font</code>. </summary>
		/// <returns>  the peer of the <code>Font</code>.
		/// @since JDK1.1 </returns>
		/// @deprecated Font rendering is now platform independent. 
		[Obsolete("Font rendering is now platform independent.")]
		public virtual FontPeer Peer
		{
			get
			{
				return Peer_NoClientCode;
			}
		}
		// NOTE: This method is called by privileged threads.
		//       We implement this functionality in a package-private method
		//       to insure that it cannot be overridden by client subclasses.
		//       DO NOT INVOKE CLIENT CODE ON THIS THREAD!
//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @SuppressWarnings("deprecation") final java.awt.peer.FontPeer getPeer_NoClientCode()
		internal FontPeer Peer_NoClientCode
		{
			get
			{
				if (Peer_Renamed == null)
				{
					Toolkit tk = Toolkit.DefaultToolkit;
					this.Peer_Renamed = tk.GetFontPeer(Name_Renamed, Style_Renamed);
				}
				return Peer_Renamed;
			}
		}

		/// <summary>
		/// Return the AttributeValues object associated with this
		/// font.  Most of the time, the internal object is null.
		/// If required, it will be created from the 'standard'
		/// state on the font.  Only non-default values will be
		/// set in the AttributeValues object.
		/// 
		/// <para>Since the AttributeValues object is mutable, and it
		/// is cached in the font, care must be taken to ensure that
		/// it is not mutated.
		/// </para>
		/// </summary>
		private AttributeValues AttributeValues
		{
			get
			{
				if (Values == null)
				{
					AttributeValues valuesTmp = new AttributeValues();
					valuesTmp.Family = Name_Renamed;
					valuesTmp.Size = PointSize; // expects the float value.
    
					if ((Style_Renamed & BOLD) != 0)
					{
						valuesTmp.Weight = 2; // WEIGHT_BOLD
					}
    
					if ((Style_Renamed & ITALIC) != 0)
					{
						valuesTmp.Posture = .2f; // POSTURE_OBLIQUE
					}
					valuesTmp.defineAll(PRIMARY_MASK); // for streaming compatibility
					Values = valuesTmp;
				}
    
				return Values;
			}
		}

		private Font2D Font2D
		{
			get
			{
				FontManager fm = FontManagerFactory.Instance;
				if (fm.usingPerAppContextComposites() && Font2DHandle != null && Font2DHandle.font2D is CompositeFont && ((CompositeFont)(Font2DHandle.font2D)).StdComposite)
				{
					return fm.findFont2D(Name_Renamed, Style_Renamed, FontManager.LOGICAL_FALLBACK);
				}
				else if (Font2DHandle == null)
				{
					Font2DHandle = fm.findFont2D(Name_Renamed, Style_Renamed, FontManager.LOGICAL_FALLBACK).handle;
				}
				/* Do not cache the de-referenced font2D. It must be explicitly
				 * de-referenced to pick up a valid font in the event that the
				 * original one is marked invalid
				 */
				return Font2DHandle.font2D;
			}
		}

		/// <summary>
		/// Creates a new <code>Font</code> from the specified name, style and
		/// point size.
		/// <para>
		/// The font name can be a font face name or a font family name.
		/// It is used together with the style to find an appropriate font face.
		/// When a font family name is specified, the style argument is used to
		/// select the most appropriate face from the family. When a font face
		/// name is specified, the face's style and the style argument are
		/// merged to locate the best matching font from the same family.
		/// For example if face name "Arial Bold" is specified with style
		/// <code>Font.ITALIC</code>, the font system looks for a face in the
		/// "Arial" family that is bold and italic, and may associate the font
		/// instance with the physical font face "Arial Bold Italic".
		/// The style argument is merged with the specified face's style, not
		/// added or subtracted.
		/// This means, specifying a bold face and a bold style does not
		/// double-embolden the font, and specifying a bold face and a plain
		/// style does not lighten the font.
		/// </para>
		/// <para>
		/// If no face for the requested style can be found, the font system
		/// may apply algorithmic styling to achieve the desired style.
		/// For example, if <code>ITALIC</code> is requested, but no italic
		/// face is available, glyphs from the plain face may be algorithmically
		/// obliqued (slanted).
		/// </para>
		/// <para>
		/// Font name lookup is case insensitive, using the case folding
		/// rules of the US locale.
		/// </para>
		/// <para>
		/// If the <code>name</code> parameter represents something other than a
		/// logical font, i.e. is interpreted as a physical font face or family, and
		/// this cannot be mapped by the implementation to a physical font or a
		/// compatible alternative, then the font system will map the Font
		/// instance to "Dialog", such that for example, the family as reported
		/// by <seealso cref="#getFamily() getFamily"/> will be "Dialog".
		/// </para>
		/// <para>
		/// 
		/// </para>
		/// </summary>
		/// <param name="name"> the font name.  This can be a font face name or a font
		/// family name, and may represent either a logical font or a physical
		/// font found in this {@code GraphicsEnvironment}.
		/// The family names for logical fonts are: Dialog, DialogInput,
		/// Monospaced, Serif, or SansSerif. Pre-defined String constants exist
		/// for all of these names, for example, {@code DIALOG}. If {@code name} is
		/// {@code null}, the <em>logical font name</em> of the new
		/// {@code Font} as returned by {@code getName()} is set to
		/// the name "Default". </param>
		/// <param name="style"> the style constant for the {@code Font}
		/// The style argument is an integer bitmask that may
		/// be {@code PLAIN}, or a bitwise union of {@code BOLD} and/or
		/// {@code ITALIC} (for example, {@code ITALIC} or {@code BOLD|ITALIC}).
		/// If the style argument does not conform to one of the expected
		/// integer bitmasks then the style is set to {@code PLAIN}. </param>
		/// <param name="size"> the point size of the {@code Font} </param>
		/// <seealso cref= GraphicsEnvironment#getAllFonts </seealso>
		/// <seealso cref= GraphicsEnvironment#getAvailableFontFamilyNames
		/// @since JDK1.0 </seealso>
		public Font(String name, int style, int size)
		{
			this.Name_Renamed = (name != null) ? name : "Default";
			this.Style_Renamed = (style & ~0x03) == 0 ? style : 0;
			this.Size_Renamed = size;
			this.PointSize = size;
		}

		private Font(String name, int style, float sizePts)
		{
			this.Name_Renamed = (name != null) ? name : "Default";
			this.Style_Renamed = (style & ~0x03) == 0 ? style : 0;
			this.Size_Renamed = (int)(sizePts + 0.5);
			this.PointSize = sizePts;
		}

		/* This constructor is used by deriveFont when attributes is null */
		private Font(String name, int style, float sizePts, bool created, Font2DHandle handle) : this(name, style, sizePts)
		{
			this.CreatedFont = created;
			/* Fonts created from a stream will use the same font2D instance
			 * as the parent.
			 * One exception is that if the derived font is requested to be
			 * in a different style, then also check if its a CompositeFont
			 * and if so build a new CompositeFont from components of that style.
			 * CompositeFonts can only be marked as "created" if they are used
			 * to add fall backs to a physical font. And non-composites are
			 * always from "Font.createFont()" and shouldn't get this treatment.
			 */
			if (created)
			{
				if (handle.font2D is CompositeFont && handle.font2D.Style != style)
				{
					FontManager fm = FontManagerFactory.Instance;
					this.Font2DHandle = fm.getNewComposite(null, style, handle);
				}
				else
				{
					this.Font2DHandle = handle;
				}
			}
		}

		/* used to implement Font.createFont */
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: private Font(File fontFile, int fontFormat, boolean isCopy, sun.font.CreatedFontTracker tracker) throws FontFormatException
		private Font(File fontFile, int fontFormat, bool isCopy, CreatedFontTracker tracker)
		{
			this.CreatedFont = true;
			/* Font2D instances created by this method track their font file
			 * so that when the Font2D is GC'd it can also remove the file.
			 */
			FontManager fm = FontManagerFactory.Instance;
			this.Font2DHandle = fm.createFont2D(fontFile, fontFormat, isCopy, tracker).handle;
			this.Name_Renamed = this.Font2DHandle.font2D.getFontName(Locale.Default);
			this.Style_Renamed = Font.PLAIN;
			this.Size_Renamed = 1;
			this.PointSize = 1f;
		}

		/* This constructor is used when one font is derived from another.
		 * Fonts created from a stream will use the same font2D instance as the
		 * parent. They can be distinguished because the "created" argument
		 * will be "true". Since there is no way to recreate these fonts they
		 * need to have the handle to the underlying font2D passed in.
		 * "created" is also true when a special composite is referenced by the
		 * handle for essentially the same reasons.
		 * But when deriving a font in these cases two particular attributes
		 * need special attention: family/face and style.
		 * The "composites" in these cases need to be recreated with optimal
		 * fonts for the new values of family and style.
		 * For fonts created with createFont() these are treated differently.
		 * JDK can often synthesise a different style (bold from plain
		 * for example). For fonts created with "createFont" this is a reasonable
		 * solution but its also possible (although rare) to derive a font with a
		 * different family attribute. In this case JDK needs
		 * to break the tie with the original Font2D and find a new Font.
		 * The oldName and oldStyle are supplied so they can be compared with
		 * what the Font2D and the values. To speed things along :
		 * oldName == null will be interpreted as the name is unchanged.
		 * oldStyle = -1 will be interpreted as the style is unchanged.
		 * In these cases there is no need to interrogate "values".
		 */
		private Font(AttributeValues values, String oldName, int oldStyle, bool created, Font2DHandle handle)
		{

			this.CreatedFont = created;
			if (created)
			{
				this.Font2DHandle = handle;

				String newName = null;
				if (oldName != null)
				{
					newName = values.Family;
					if (oldName.Equals(newName))
					{
						newName = null;
					}
				}
				int newStyle = 0;
				if (oldStyle == -1)
				{
					newStyle = -1;
				}
				else
				{
					if (values.Weight >= 2f)
					{
						newStyle = BOLD;
					}
					if (values.Posture >= .2f)
					{
						newStyle |= ITALIC;
					}
					if (oldStyle == newStyle)
					{
						newStyle = -1;
					}
				}
				if (handle.font2D is CompositeFont)
				{
					if (newStyle != -1 || newName != null)
					{
						FontManager fm = FontManagerFactory.Instance;
						this.Font2DHandle = fm.getNewComposite(newName, newStyle, handle);
					}
				}
				else if (newName != null)
				{
					this.CreatedFont = false;
					this.Font2DHandle = null;
				}
			}
			InitFromValues(values);
		}

		/// <summary>
		/// Creates a new <code>Font</code> with the specified attributes.
		/// Only keys defined in <seealso cref="java.awt.font.TextAttribute TextAttribute"/>
		/// are recognized.  In addition the FONT attribute is
		///  not recognized by this constructor
		/// (see <seealso cref="#getAvailableAttributes"/>). Only attributes that have
		/// values of valid types will affect the new <code>Font</code>.
		/// <para>
		/// If <code>attributes</code> is <code>null</code>, a new
		/// <code>Font</code> is initialized with default values.
		/// </para>
		/// </summary>
		/// <seealso cref= java.awt.font.TextAttribute </seealso>
		/// <param name="attributes"> the attributes to assign to the new
		///          <code>Font</code>, or <code>null</code> </param>
		public Font<T1>(IDictionary<T1> attributes) where T1 : java.text.AttributedCharacterIterator_Attribute
		{
			InitFromValues(AttributeValues.fromMap(attributes, RECOGNIZED_MASK));
		}

		/// <summary>
		/// Creates a new <code>Font</code> from the specified <code>font</code>.
		/// This constructor is intended for use by subclasses. </summary>
		/// <param name="font"> from which to create this <code>Font</code>. </param>
		/// <exception cref="NullPointerException"> if <code>font</code> is null
		/// @since 1.6 </exception>
		protected internal Font(Font font)
		{
			if (font.Values != null)
			{
				InitFromValues(font.AttributeValues.clone());
			}
			else
			{
				this.Name_Renamed = font.Name_Renamed;
				this.Style_Renamed = font.Style_Renamed;
				this.Size_Renamed = font.Size_Renamed;
				this.PointSize = font.PointSize;
			}
			this.Font2DHandle = font.Font2DHandle;
			this.CreatedFont = font.CreatedFont;
		}

		/// <summary>
		/// Font recognizes all attributes except FONT.
		/// </summary>
		private static readonly int RECOGNIZED_MASK = AttributeValues.MASK_ALL & ~AttributeValues.getMask(EFONT);

		/// <summary>
		/// These attributes are considered primary by the FONT attribute.
		/// </summary>
		private static readonly int PRIMARY_MASK = AttributeValues.getMask(EFAMILY, EWEIGHT, EWIDTH, EPOSTURE, ESIZE, ETRANSFORM, ESUPERSCRIPT, ETRACKING);

		/// <summary>
		/// These attributes are considered secondary by the FONT attribute.
		/// </summary>
		private static readonly int SECONDARY_MASK = RECOGNIZED_MASK & ~PRIMARY_MASK;

		/// <summary>
		/// These attributes are handled by layout.
		/// </summary>
		private static readonly int LAYOUT_MASK = AttributeValues.getMask(ECHAR_REPLACEMENT, EFOREGROUND, EBACKGROUND, EUNDERLINE, ESTRIKETHROUGH, ERUN_DIRECTION, EBIDI_EMBEDDING, EJUSTIFICATION, EINPUT_METHOD_HIGHLIGHT, EINPUT_METHOD_UNDERLINE, ESWAP_COLORS, ENUMERIC_SHAPING, EKERNING, ELIGATURES, ETRACKING, ESUPERSCRIPT);

		private static readonly int EXTRA_MASK = AttributeValues.getMask(ETRANSFORM, ESUPERSCRIPT, EWIDTH);

		/// <summary>
		/// Initialize the standard Font fields from the values object.
		/// </summary>
		private void InitFromValues(AttributeValues values)
		{
			this.Values = values;
			values.defineAll(PRIMARY_MASK); // for 1.5 streaming compatibility

			this.Name_Renamed = values.Family;
			this.PointSize = values.Size;
			this.Size_Renamed = (int)(values.Size + 0.5);
			if (values.Weight >= 2f) // not == 2f
			{
				this.Style_Renamed |= BOLD;
			}
			if (values.Posture >= .2f) // not  == .2f
			{
				this.Style_Renamed |= ITALIC;
			}

			this.NonIdentityTx = values.anyNonDefault(EXTRA_MASK);
			this.HasLayoutAttributes_Renamed = values.anyNonDefault(LAYOUT_MASK);
		}

		/// <summary>
		/// Returns a <code>Font</code> appropriate to the attributes.
		/// If <code>attributes</code>contains a <code>FONT</code> attribute
		/// with a valid <code>Font</code> as its value, it will be
		/// merged with any remaining attributes.  See
		/// <seealso cref="java.awt.font.TextAttribute#FONT"/> for more
		/// information.
		/// </summary>
		/// <param name="attributes"> the attributes to assign to the new
		///          <code>Font</code> </param>
		/// <returns> a new <code>Font</code> created with the specified
		///          attributes </returns>
		/// <exception cref="NullPointerException"> if <code>attributes</code> is null.
		/// @since 1.2 </exception>
		/// <seealso cref= java.awt.font.TextAttribute </seealso>
		public static Font getFont<T1>(IDictionary<T1> attributes) where T1 : java.text.AttributedCharacterIterator_Attribute
		{
			// optimize for two cases:
			// 1) FONT attribute, and nothing else
			// 2) attributes, but no FONT

			// avoid turning the attributemap into a regular map for no reason
			if (attributes is AttributeMap && ((AttributeMap)attributes).Values != null)
			{
				AttributeValues values = ((AttributeMap)attributes).Values;
				if (values.isNonDefault(EFONT))
				{
					Font font = values.Font;
					if (!values.anyDefined(SECONDARY_MASK))
					{
						return font;
					}
					// merge
					values = font.AttributeValues.clone();
					values.merge(attributes, SECONDARY_MASK);
					return new Font(values, font.Name_Renamed, font.Style_Renamed, font.CreatedFont, font.Font2DHandle);
				}
				return new Font(attributes);
			}

			Font font = (Font)attributes[TextAttribute.FONT];
			if (font != null)
			{
				if (attributes.Count > 1) // oh well, check for anything else
				{
					AttributeValues values = font.AttributeValues.clone();
					values.merge(attributes, SECONDARY_MASK);
					return new Font(values, font.Name_Renamed, font.Style_Renamed, font.CreatedFont, font.Font2DHandle);
				}

				return font;
			}

			return new Font(attributes);
		}

		/// <summary>
		/// Used with the byte count tracker for fonts created from streams.
		/// If a thread can create temp files anyway, no point in counting
		/// font bytes.
		/// </summary>
		private static bool HasTempPermission()
		{

			if (System.SecurityManager == null)
			{
				return true;
			}
			File f = null;
			bool hasPerm = false;
			try
			{
				f = Files.createTempFile("+~JT", ".tmp").toFile();
				f.Delete();
				f = null;
				hasPerm = true;
			}
			catch (Throwable)
			{
				/* inc. any kind of SecurityException */
			}
			return hasPerm;
		}

		/// <summary>
		/// Returns a new <code>Font</code> using the specified font type
		/// and input data.  The new <code>Font</code> is
		/// created with a point size of 1 and style <seealso cref="#PLAIN PLAIN"/>.
		/// This base font can then be used with the <code>deriveFont</code>
		/// methods in this class to derive new <code>Font</code> objects with
		/// varying sizes, styles, transforms and font features.  This
		/// method does not close the <seealso cref="InputStream"/>.
		/// <para>
		/// To make the <code>Font</code> available to Font constructors the
		/// returned <code>Font</code> must be registered in the
		/// <code>GraphicsEnviroment</code> by calling
		/// <seealso cref="GraphicsEnvironment#registerFont(Font) registerFont(Font)"/>.
		/// </para>
		/// </summary>
		/// <param name="fontFormat"> the type of the <code>Font</code>, which is
		/// <seealso cref="#TRUETYPE_FONT TRUETYPE_FONT"/> if a TrueType resource is specified.
		/// or <seealso cref="#TYPE1_FONT TYPE1_FONT"/> if a Type 1 resource is specified. </param>
		/// <param name="fontStream"> an <code>InputStream</code> object representing the
		/// input data for the font. </param>
		/// <returns> a new <code>Font</code> created with the specified font type. </returns>
		/// <exception cref="IllegalArgumentException"> if <code>fontFormat</code> is not
		///     <code>TRUETYPE_FONT</code>or<code>TYPE1_FONT</code>. </exception>
		/// <exception cref="FontFormatException"> if the <code>fontStream</code> data does
		///     not contain the required font tables for the specified format. </exception>
		/// <exception cref="IOException"> if the <code>fontStream</code>
		///     cannot be completely read. </exception>
		/// <seealso cref= GraphicsEnvironment#registerFont(Font)
		/// @since 1.3 </seealso>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public static Font createFont(int fontFormat, InputStream fontStream) throws java.awt.FontFormatException, java.io.IOException
		public static Font CreateFont(int fontFormat, InputStream fontStream)
		{

			if (HasTempPermission())
			{
				return CreateFont0(fontFormat, fontStream, null);
			}

			// Otherwise, be extra conscious of pending temp file creation and
			// resourcefully handle the temp file resources, among other things.
			CreatedFontTracker tracker = CreatedFontTracker.Tracker;
			bool acquired = false;
			try
			{
				acquired = tracker.acquirePermit();
				if (!acquired)
				{
					throw new IOException("Timed out waiting for resources.");
				}
				return CreateFont0(fontFormat, fontStream, tracker);
			}
			catch (InterruptedException)
			{
				throw new IOException("Problem reading font data.");
			}
			finally
			{
				if (acquired)
				{
					tracker.releasePermit();
				}
			}
		}

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: private static Font createFont0(int fontFormat, InputStream fontStream, sun.font.CreatedFontTracker tracker) throws java.awt.FontFormatException, java.io.IOException
		private static Font CreateFont0(int fontFormat, InputStream fontStream, CreatedFontTracker tracker)
		{

			if (fontFormat != Font.TRUETYPE_FONT && fontFormat != Font.TYPE1_FONT)
			{
				throw new IllegalArgumentException("font format not recognized");
			}
			bool copiedFontData = false;
			try
			{
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final File tFile = java.security.AccessController.doPrivileged(new java.security.PrivilegedExceptionAction<File>()
				File tFile = AccessController.doPrivileged(new PrivilegedExceptionActionAnonymousInnerClassHelper()
			   );
				if (tracker != null)
				{
					tracker.add(tFile);
				}

				int totalSize = 0;
				try
				{
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final OutputStream outStream = java.security.AccessController.doPrivileged(new java.security.PrivilegedExceptionAction<OutputStream>()
					OutputStream outStream = AccessController.doPrivileged(new PrivilegedExceptionActionAnonymousInnerClassHelper2(tFile)
					   );
					if (tracker != null)
					{
						tracker.set(tFile, outStream);
					}
					try
					{
						sbyte[] buf = new sbyte[8192];
						for (;;)
						{
							int bytesRead = fontStream.Read(buf);
							if (bytesRead < 0)
							{
								break;
							}
							if (tracker != null)
							{
								if (totalSize + bytesRead > CreatedFontTracker.MAX_FILE_SIZE)
								{
									throw new IOException("File too big.");
								}
								if (totalSize + tracker.NumBytes > CreatedFontTracker.MAX_TOTAL_BYTES)
								{
									throw new IOException("Total files too big.");
								}
								totalSize += bytesRead;
								tracker.addBytes(bytesRead);
							}
							outStream.Write(buf, 0, bytesRead);
						}
						/* don't close the input stream */
					}
					finally
					{
						outStream.Close();
					}
					/* After all references to a Font2D are dropped, the file
					 * will be removed. To support long-lived AppContexts,
					 * we need to then decrement the byte count by the size
					 * of the file.
					 * If the data isn't a valid font, the implementation will
					 * delete the tmp file and decrement the byte count
					 * in the tracker object before returning from the
					 * constructor, so we can set 'copiedFontData' to true here
					 * without waiting for the results of that constructor.
					 */
					copiedFontData = true;
					Font font = new Font(tFile, fontFormat, true, tracker);
					return font;
				}
				finally
				{
					if (tracker != null)
					{
						tracker.remove(tFile);
					}
					if (!copiedFontData)
					{
						if (tracker != null)
						{
							tracker.subBytes(totalSize);
						}
						AccessController.doPrivileged(new PrivilegedExceptionActionAnonymousInnerClassHelper3(tFile)
					   );
					}
				}
			}
			catch (Throwable t)
			{
				if (t is FontFormatException)
				{
					throw (FontFormatException)t;
				}
				if (t is IOException)
				{
					throw (IOException)t;
				}
				Throwable cause = t.Cause;
				if (cause is FontFormatException)
				{
					throw (FontFormatException)cause;
				}
				throw new IOException("Problem reading font data.");
			}
		}

		private class PrivilegedExceptionActionAnonymousInnerClassHelper : PrivilegedExceptionAction<File>
		{
			public PrivilegedExceptionActionAnonymousInnerClassHelper()
			{
			}

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public File run() throws IOException
			public virtual File Run()
			{
				return Files.createTempFile("+~JF", ".tmp").toFile();
			}
		}

		private class PrivilegedExceptionActionAnonymousInnerClassHelper2 : PrivilegedExceptionAction<OutputStream>
		{
			private File TFile;

			public PrivilegedExceptionActionAnonymousInnerClassHelper2(java.io.File tFile)
			{
				this.TFile = tFile;
			}

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public OutputStream run() throws IOException
			public virtual OutputStream Run()
			{
				return new FileOutputStream(TFile);
			}
		}

		private class PrivilegedExceptionActionAnonymousInnerClassHelper3 : PrivilegedExceptionAction<Void>
		{
			private File TFile;

			public PrivilegedExceptionActionAnonymousInnerClassHelper3(java.io.File tFile)
			{
				this.TFile = tFile;
			}

			public virtual Void Run()
			{
				TFile.Delete();
				return null;
			}
		}

		/// <summary>
		/// Returns a new <code>Font</code> using the specified font type
		/// and the specified font file.  The new <code>Font</code> is
		/// created with a point size of 1 and style <seealso cref="#PLAIN PLAIN"/>.
		/// This base font can then be used with the <code>deriveFont</code>
		/// methods in this class to derive new <code>Font</code> objects with
		/// varying sizes, styles, transforms and font features. </summary>
		/// <param name="fontFormat"> the type of the <code>Font</code>, which is
		/// <seealso cref="#TRUETYPE_FONT TRUETYPE_FONT"/> if a TrueType resource is
		/// specified or <seealso cref="#TYPE1_FONT TYPE1_FONT"/> if a Type 1 resource is
		/// specified.
		/// So long as the returned font, or its derived fonts are referenced
		/// the implementation may continue to access <code>fontFile</code>
		/// to retrieve font data. Thus the results are undefined if the file
		/// is changed, or becomes inaccessible.
		/// <para>
		/// To make the <code>Font</code> available to Font constructors the
		/// returned <code>Font</code> must be registered in the
		/// <code>GraphicsEnviroment</code> by calling
		/// <seealso cref="GraphicsEnvironment#registerFont(Font) registerFont(Font)"/>.
		/// </para>
		/// </param>
		/// <param name="fontFile"> a <code>File</code> object representing the
		/// input data for the font. </param>
		/// <returns> a new <code>Font</code> created with the specified font type. </returns>
		/// <exception cref="IllegalArgumentException"> if <code>fontFormat</code> is not
		///     <code>TRUETYPE_FONT</code>or<code>TYPE1_FONT</code>. </exception>
		/// <exception cref="NullPointerException"> if <code>fontFile</code> is null. </exception>
		/// <exception cref="IOException"> if the <code>fontFile</code> cannot be read. </exception>
		/// <exception cref="FontFormatException"> if <code>fontFile</code> does
		///     not contain the required font tables for the specified format. </exception>
		/// <exception cref="SecurityException"> if the executing code does not have
		/// permission to read from the file. </exception>
		/// <seealso cref= GraphicsEnvironment#registerFont(Font)
		/// @since 1.5 </seealso>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public static Font createFont(int fontFormat, File fontFile) throws java.awt.FontFormatException, java.io.IOException
		public static Font CreateFont(int fontFormat, File fontFile)
		{

			fontFile = new File(fontFile.Path);

			if (fontFormat != Font.TRUETYPE_FONT && fontFormat != Font.TYPE1_FONT)
			{
				throw new IllegalArgumentException("font format not recognized");
			}
			SecurityManager sm = System.SecurityManager;
			if (sm != null)
			{
				FilePermission filePermission = new FilePermission(fontFile.Path, "read");
				sm.CheckPermission(filePermission);
			}
			if (!fontFile.CanRead())
			{
				throw new IOException("Can't read " + fontFile);
			}
			return new Font(fontFile, fontFormat, false, null);
		}

		/// <summary>
		/// Returns a copy of the transform associated with this
		/// <code>Font</code>.  This transform is not necessarily the one
		/// used to construct the font.  If the font has algorithmic
		/// superscripting or width adjustment, this will be incorporated
		/// into the returned <code>AffineTransform</code>.
		/// <para>
		/// Typically, fonts will not be transformed.  Clients generally
		/// should call <seealso cref="#isTransformed"/> first, and only call this
		/// method if <code>isTransformed</code> returns true.
		/// 
		/// </para>
		/// </summary>
		/// <returns> an <seealso cref="AffineTransform"/> object representing the
		///          transform attribute of this <code>Font</code> object. </returns>
		public virtual AffineTransform Transform
		{
			get
			{
				/* The most common case is the identity transform.  Most callers
				 * should call isTransformed() first, to decide if they need to
				 * get the transform, but some may not.  Here we check to see
				 * if we have a nonidentity transform, and only do the work to
				 * fetch and/or compute it if so, otherwise we return a new
				 * identity transform.
				 *
				 * Note that the transform is _not_ necessarily the same as
				 * the transform passed in as an Attribute in a Map, as the
				 * transform returned will also reflect the effects of WIDTH and
				 * SUPERSCRIPT attributes.  Clients who want the actual transform
				 * need to call getRequestedAttributes.
				 */
				if (NonIdentityTx)
				{
					AttributeValues values = AttributeValues;
    
					AffineTransform at = values.isNonDefault(ETRANSFORM) ? new AffineTransform(values.Transform) : new AffineTransform();
    
					if (values.Superscript != 0)
					{
						// can't get ascent and descent here, recursive call to this fn,
						// so use pointsize
						// let users combine super- and sub-scripting
    
						int superscript = values.Superscript;
    
						double trans = 0;
						int n = 0;
						bool up = superscript > 0;
						int sign = up ? - 1 : 1;
						int ss = up ? superscript : -superscript;
    
						while ((ss & 7) > n)
						{
							int newn = ss & 7;
							trans += sign * (Ssinfo[newn] - Ssinfo[n]);
							ss >>= 3;
							sign = -sign;
							n = newn;
						}
						trans *= PointSize;
						double scale = System.Math.Pow(2.0 / 3.0, n);
    
						at.PreConcatenate(AffineTransform.GetTranslateInstance(0, trans));
						at.Scale(scale, scale);
    
						// note on placement and italics
						// We preconcatenate the transform because we don't want to translate along
						// the italic angle, but purely perpendicular to the baseline.  While this
						// looks ok for superscripts, it can lead subscripts to stack on each other
						// and bring the following text too close.  The way we deal with potential
						// collisions that can occur in the case of italics is by adjusting the
						// horizontal spacing of the adjacent glyphvectors.  Examine the italic
						// angle of both vectors, if one is non-zero, compute the minimum ascent
						// and descent, and then the x position at each for each vector along its
						// italic angle starting from its (offset) baseline.  Compute the difference
						// between the x positions and use the maximum difference to adjust the
						// position of the right gv.
					}
    
					if (values.isNonDefault(EWIDTH))
					{
						at.Scale(values.Width, 1f);
					}
    
					return at;
				}
    
				return new AffineTransform();
			}
		}

		// x = r^0 + r^1 + r^2... r^n
		// rx = r^1 + r^2 + r^3... r^(n+1)
		// x - rx = r^0 - r^(n+1)
		// x (1 - r) = r^0 - r^(n+1)
		// x = (r^0 - r^(n+1)) / (1 - r)
		// x = (1 - r^(n+1)) / (1 - r)

		// scale ratio is 2/3
		// trans = 1/2 of ascent * x
		// assume ascent is 3/4 of point size

		private static readonly float[] Ssinfo = new float[] {0.0f, 0.375f, 0.625f, 0.7916667f, 0.9027778f, 0.9768519f, 1.0262346f, 1.0591564f};

		/// <summary>
		/// Returns the family name of this <code>Font</code>.
		/// 
		/// <para>The family name of a font is font specific. Two fonts such as
		/// Helvetica Italic and Helvetica Bold have the same family name,
		/// <i>Helvetica</i>, whereas their font face names are
		/// <i>Helvetica Bold</i> and <i>Helvetica Italic</i>. The list of
		/// available family names may be obtained by using the
		/// <seealso cref="GraphicsEnvironment#getAvailableFontFamilyNames()"/> method.
		/// 
		/// </para>
		/// <para>Use <code>getName</code> to get the logical name of the font.
		/// Use <code>getFontName</code> to get the font face name of the font.
		/// </para>
		/// </summary>
		/// <returns> a <code>String</code> that is the family name of this
		///          <code>Font</code>.
		/// </returns>
		/// <seealso cref= #getName </seealso>
		/// <seealso cref= #getFontName
		/// @since JDK1.1 </seealso>
		public virtual String Family
		{
			get
			{
				return Family_NoClientCode;
			}
		}
		// NOTE: This method is called by privileged threads.
		//       We implement this functionality in a package-private
		//       method to insure that it cannot be overridden by client
		//       subclasses.
		//       DO NOT INVOKE CLIENT CODE ON THIS THREAD!
		internal String Family_NoClientCode
		{
			get
			{
				return GetFamily(Locale.Default);
			}
		}

		/// <summary>
		/// Returns the family name of this <code>Font</code>, localized for
		/// the specified locale.
		/// 
		/// <para>The family name of a font is font specific. Two fonts such as
		/// Helvetica Italic and Helvetica Bold have the same family name,
		/// <i>Helvetica</i>, whereas their font face names are
		/// <i>Helvetica Bold</i> and <i>Helvetica Italic</i>. The list of
		/// available family names may be obtained by using the
		/// <seealso cref="GraphicsEnvironment#getAvailableFontFamilyNames()"/> method.
		/// 
		/// </para>
		/// <para>Use <code>getFontName</code> to get the font face name of the font.
		/// </para>
		/// </summary>
		/// <param name="l"> locale for which to get the family name </param>
		/// <returns> a <code>String</code> representing the family name of the
		///          font, localized for the specified locale. </returns>
		/// <seealso cref= #getFontName </seealso>
		/// <seealso cref= java.util.Locale
		/// @since 1.2 </seealso>
		public virtual String GetFamily(Locale l)
		{
			if (l == null)
			{
				throw new NullPointerException("null locale doesn't mean default");
			}
			return Font2D.getFamilyName(l);
		}

		/// <summary>
		/// Returns the postscript name of this <code>Font</code>.
		/// Use <code>getFamily</code> to get the family name of the font.
		/// Use <code>getFontName</code> to get the font face name of the font. </summary>
		/// <returns> a <code>String</code> representing the postscript name of
		///          this <code>Font</code>.
		/// @since 1.2 </returns>
		public virtual String PSName
		{
			get
			{
				return Font2D.PostscriptName;
			}
		}

		/// <summary>
		/// Returns the logical name of this <code>Font</code>.
		/// Use <code>getFamily</code> to get the family name of the font.
		/// Use <code>getFontName</code> to get the font face name of the font. </summary>
		/// <returns> a <code>String</code> representing the logical name of
		///          this <code>Font</code>. </returns>
		/// <seealso cref= #getFamily </seealso>
		/// <seealso cref= #getFontName
		/// @since JDK1.0 </seealso>
		public virtual String Name
		{
			get
			{
				return Name_Renamed;
			}
		}

		/// <summary>
		/// Returns the font face name of this <code>Font</code>.  For example,
		/// Helvetica Bold could be returned as a font face name.
		/// Use <code>getFamily</code> to get the family name of the font.
		/// Use <code>getName</code> to get the logical name of the font. </summary>
		/// <returns> a <code>String</code> representing the font face name of
		///          this <code>Font</code>. </returns>
		/// <seealso cref= #getFamily </seealso>
		/// <seealso cref= #getName
		/// @since 1.2 </seealso>
		public virtual String FontName
		{
			get
			{
			  return GetFontName(Locale.Default);
			}
		}

		/// <summary>
		/// Returns the font face name of the <code>Font</code>, localized
		/// for the specified locale. For example, Helvetica Fett could be
		/// returned as the font face name.
		/// Use <code>getFamily</code> to get the family name of the font. </summary>
		/// <param name="l"> a locale for which to get the font face name </param>
		/// <returns> a <code>String</code> representing the font face name,
		///          localized for the specified locale. </returns>
		/// <seealso cref= #getFamily </seealso>
		/// <seealso cref= java.util.Locale </seealso>
		public virtual String GetFontName(Locale l)
		{
			if (l == null)
			{
				throw new NullPointerException("null locale doesn't mean default");
			}
			return Font2D.getFontName(l);
		}

		/// <summary>
		/// Returns the style of this <code>Font</code>.  The style can be
		/// PLAIN, BOLD, ITALIC, or BOLD+ITALIC. </summary>
		/// <returns> the style of this <code>Font</code> </returns>
		/// <seealso cref= #isPlain </seealso>
		/// <seealso cref= #isBold </seealso>
		/// <seealso cref= #isItalic
		/// @since JDK1.0 </seealso>
		public virtual int Style
		{
			get
			{
				return Style_Renamed;
			}
		}

		/// <summary>
		/// Returns the point size of this <code>Font</code>, rounded to
		/// an integer.
		/// Most users are familiar with the idea of using <i>point size</i> to
		/// specify the size of glyphs in a font. This point size defines a
		/// measurement between the baseline of one line to the baseline of the
		/// following line in a single spaced text document. The point size is
		/// based on <i>typographic points</i>, approximately 1/72 of an inch.
		/// <para>
		/// The Java(tm)2D API adopts the convention that one point is
		/// equivalent to one unit in user coordinates.  When using a
		/// normalized transform for converting user space coordinates to
		/// device space coordinates 72 user
		/// space units equal 1 inch in device space.  In this case one point
		/// is 1/72 of an inch.
		/// </para>
		/// </summary>
		/// <returns> the point size of this <code>Font</code> in 1/72 of an
		///          inch units. </returns>
		/// <seealso cref= #getSize2D </seealso>
		/// <seealso cref= GraphicsConfiguration#getDefaultTransform </seealso>
		/// <seealso cref= GraphicsConfiguration#getNormalizingTransform
		/// @since JDK1.0 </seealso>
		public virtual int Size
		{
			get
			{
				return Size_Renamed;
			}
		}

		/// <summary>
		/// Returns the point size of this <code>Font</code> in
		/// <code>float</code> value. </summary>
		/// <returns> the point size of this <code>Font</code> as a
		/// <code>float</code> value. </returns>
		/// <seealso cref= #getSize
		/// @since 1.2 </seealso>
		public virtual float Size2D
		{
			get
			{
				return PointSize;
			}
		}

		/// <summary>
		/// Indicates whether or not this <code>Font</code> object's style is
		/// PLAIN. </summary>
		/// <returns>    <code>true</code> if this <code>Font</code> has a
		///            PLAIN style;
		///            <code>false</code> otherwise. </returns>
		/// <seealso cref=       java.awt.Font#getStyle
		/// @since     JDK1.0 </seealso>
		public virtual bool Plain
		{
			get
			{
				return Style_Renamed == 0;
			}
		}

		/// <summary>
		/// Indicates whether or not this <code>Font</code> object's style is
		/// BOLD. </summary>
		/// <returns>    <code>true</code> if this <code>Font</code> object's
		///            style is BOLD;
		///            <code>false</code> otherwise. </returns>
		/// <seealso cref=       java.awt.Font#getStyle
		/// @since     JDK1.0 </seealso>
		public virtual bool Bold
		{
			get
			{
				return (Style_Renamed & BOLD) != 0;
			}
		}

		/// <summary>
		/// Indicates whether or not this <code>Font</code> object's style is
		/// ITALIC. </summary>
		/// <returns>    <code>true</code> if this <code>Font</code> object's
		///            style is ITALIC;
		///            <code>false</code> otherwise. </returns>
		/// <seealso cref=       java.awt.Font#getStyle
		/// @since     JDK1.0 </seealso>
		public virtual bool Italic
		{
			get
			{
				return (Style_Renamed & ITALIC) != 0;
			}
		}

		/// <summary>
		/// Indicates whether or not this <code>Font</code> object has a
		/// transform that affects its size in addition to the Size
		/// attribute. </summary>
		/// <returns>  <code>true</code> if this <code>Font</code> object
		///          has a non-identity AffineTransform attribute.
		///          <code>false</code> otherwise. </returns>
		/// <seealso cref=     java.awt.Font#getTransform
		/// @since   1.4 </seealso>
		public virtual bool Transformed
		{
			get
			{
				return NonIdentityTx;
			}
		}

		/// <summary>
		/// Return true if this Font contains attributes that require extra
		/// layout processing. </summary>
		/// <returns> true if the font has layout attributes
		/// @since 1.6 </returns>
		public virtual bool HasLayoutAttributes()
		{
			return HasLayoutAttributes_Renamed;
		}

		/// <summary>
		/// Returns a <code>Font</code> object from the system properties list.
		/// <code>nm</code> is treated as the name of a system property to be
		/// obtained.  The <code>String</code> value of this property is then
		/// interpreted as a <code>Font</code> object according to the
		/// specification of <code>Font.decode(String)</code>
		/// If the specified property is not found, or the executing code does
		/// not have permission to read the property, null is returned instead.
		/// </summary>
		/// <param name="nm"> the property name </param>
		/// <returns> a <code>Font</code> object that the property name
		///          describes, or null if no such property exists. </returns>
		/// <exception cref="NullPointerException"> if nm is null.
		/// @since 1.2 </exception>
		/// <seealso cref= #decode(String) </seealso>
		public static Font GetFont(String nm)
		{
			return GetFont(nm, null);
		}

		/// <summary>
		/// Returns the <code>Font</code> that the <code>str</code>
		/// argument describes.
		/// To ensure that this method returns the desired Font,
		/// format the <code>str</code> parameter in
		/// one of these ways
		/// 
		/// <ul>
		/// <li><em>fontname-style-pointsize</em>
		/// <li><em>fontname-pointsize</em>
		/// <li><em>fontname-style</em>
		/// <li><em>fontname</em>
		/// <li><em>fontname style pointsize</em>
		/// <li><em>fontname pointsize</em>
		/// <li><em>fontname style</em>
		/// <li><em>fontname</em>
		/// </ul>
		/// in which <i>style</i> is one of the four
		/// case-insensitive strings:
		/// <code>"PLAIN"</code>, <code>"BOLD"</code>, <code>"BOLDITALIC"</code>, or
		/// <code>"ITALIC"</code>, and pointsize is a positive decimal integer
		/// representation of the point size.
		/// For example, if you want a font that is Arial, bold, with
		/// a point size of 18, you would call this method with:
		/// "Arial-BOLD-18".
		/// This is equivalent to calling the Font constructor :
		/// <code>new Font("Arial", Font.BOLD, 18);</code>
		/// and the values are interpreted as specified by that constructor.
		/// <para>
		/// A valid trailing decimal field is always interpreted as the pointsize.
		/// Therefore a fontname containing a trailing decimal value should not
		/// be used in the fontname only form.
		/// </para>
		/// <para>
		/// If a style name field is not one of the valid style strings, it is
		/// interpreted as part of the font name, and the default style is used.
		/// </para>
		/// <para>
		/// Only one of ' ' or '-' may be used to separate fields in the input.
		/// The identified separator is the one closest to the end of the string
		/// which separates a valid pointsize, or a valid style name from
		/// the rest of the string.
		/// Null (empty) pointsize and style fields are treated
		/// as valid fields with the default value for that field.
		/// </para>
		/// <para>
		/// Some font names may include the separator characters ' ' or '-'.
		/// If <code>str</code> is not formed with 3 components, e.g. such that
		/// <code>style</code> or <code>pointsize</code> fields are not present in
		/// <code>str</code>, and <code>fontname</code> also contains a
		/// character determined to be the separator character
		/// then these characters where they appear as intended to be part of
		/// <code>fontname</code> may instead be interpreted as separators
		/// so the font name may not be properly recognised.
		/// 
		/// </para>
		/// <para>
		/// The default size is 12 and the default style is PLAIN.
		/// If <code>str</code> does not specify a valid size, the returned
		/// <code>Font</code> has a size of 12.  If <code>str</code> does not
		/// specify a valid style, the returned Font has a style of PLAIN.
		/// If you do not specify a valid font name in
		/// the <code>str</code> argument, this method will return
		/// a font with the family name "Dialog".
		/// To determine what font family names are available on
		/// your system, use the
		/// <seealso cref="GraphicsEnvironment#getAvailableFontFamilyNames()"/> method.
		/// If <code>str</code> is <code>null</code>, a new <code>Font</code>
		/// is returned with the family name "Dialog", a size of 12 and a
		/// PLAIN style.
		/// </para>
		/// </summary>
		/// <param name="str"> the name of the font, or <code>null</code> </param>
		/// <returns> the <code>Font</code> object that <code>str</code>
		///          describes, or a new default <code>Font</code> if
		///          <code>str</code> is <code>null</code>. </returns>
		/// <seealso cref= #getFamily
		/// @since JDK1.1 </seealso>
		public static Font Decode(String str)
		{
			String fontName = str;
			String styleName = "";
			int fontSize = 12;
			int fontStyle = Font.PLAIN;

			if (str == null)
			{
				return new Font(DIALOG, fontStyle, fontSize);
			}

			int lastHyphen = str.LastIndexOf('-');
			int lastSpace = str.LastIndexOf(' ');
			char sepChar = (lastHyphen > lastSpace) ? '-' : ' ';
			int sizeIndex = str.LastIndexOf(sepChar);
			int styleIndex = str.LastIndexOf(sepChar, sizeIndex - 1);
			int strlen = str.Length();

			if (sizeIndex > 0 && sizeIndex + 1 < strlen)
			{
				try
				{
					fontSize = Convert.ToInt32(str.Substring(sizeIndex + 1)).IntValue();
					if (fontSize <= 0)
					{
						fontSize = 12;
					}
				}
				catch (NumberFormatException)
				{
					/* It wasn't a valid size, if we didn't also find the
					 * start of the style string perhaps this is the style */
					styleIndex = sizeIndex;
					sizeIndex = strlen;
					if (str.CharAt(sizeIndex - 1) == sepChar)
					{
						sizeIndex--;
					}
				}
			}

			if (styleIndex >= 0 && styleIndex + 1 < strlen)
			{
				styleName = StringHelperClass.SubstringSpecial(str, styleIndex + 1, sizeIndex);
				styleName = styleName.ToLowerCase(Locale.ENGLISH);
				if (styleName.Equals("bolditalic"))
				{
					fontStyle = Font.BOLD | Font.ITALIC;
				}
				else if (styleName.Equals("italic"))
				{
					fontStyle = Font.ITALIC;
				}
				else if (styleName.Equals("bold"))
				{
					fontStyle = Font.BOLD;
				}
				else if (styleName.Equals("plain"))
				{
					fontStyle = Font.PLAIN;
				}
				else
				{
					/* this string isn't any of the expected styles, so
					 * assume its part of the font name
					 */
					styleIndex = sizeIndex;
					if (str.CharAt(styleIndex - 1) == sepChar)
					{
						styleIndex--;
					}
				}
				fontName = str.Substring(0, styleIndex);

			}
			else
			{
				int fontEnd = strlen;
				if (styleIndex > 0)
				{
					fontEnd = styleIndex;
				}
				else if (sizeIndex > 0)
				{
					fontEnd = sizeIndex;
				}
				if (fontEnd > 0 && str.CharAt(fontEnd - 1) == sepChar)
				{
					fontEnd--;
				}
				fontName = str.Substring(0, fontEnd);
			}

			return new Font(fontName, fontStyle, fontSize);
		}

		/// <summary>
		/// Gets the specified <code>Font</code> from the system properties
		/// list.  As in the <code>getProperty</code> method of
		/// <code>System</code>, the first
		/// argument is treated as the name of a system property to be
		/// obtained.  The <code>String</code> value of this property is then
		/// interpreted as a <code>Font</code> object.
		/// <para>
		/// The property value should be one of the forms accepted by
		/// <code>Font.decode(String)</code>
		/// If the specified property is not found, or the executing code does not
		/// have permission to read the property, the <code>font</code>
		/// argument is returned instead.
		/// </para>
		/// </summary>
		/// <param name="nm"> the case-insensitive property name </param>
		/// <param name="font"> a default <code>Font</code> to return if property
		///          <code>nm</code> is not defined </param>
		/// <returns>    the <code>Font</code> value of the property. </returns>
		/// <exception cref="NullPointerException"> if nm is null. </exception>
		/// <seealso cref= #decode(String) </seealso>
		public static Font GetFont(String nm, Font font)
		{
			String str = null;
			try
			{
				str = System.getProperty(nm);
			}
			catch (SecurityException)
			{
			}
			if (str == null)
			{
				return font;
			}
			return Decode(str);
		}

		[NonSerialized]
		internal int Hash;
		/// <summary>
		/// Returns a hashcode for this <code>Font</code>. </summary>
		/// <returns>     a hashcode value for this <code>Font</code>.
		/// @since      JDK1.0 </returns>
		public override int HashCode()
		{
			if (Hash == 0)
			{
				Hash = Name_Renamed.HashCode() ^ Style_Renamed ^ Size_Renamed;
				/* It is possible many fonts differ only in transform.
				 * So include the transform in the hash calculation.
				 * nonIdentityTx is set whenever there is a transform in
				 * 'values'. The tests for null are required because it can
				 * also be set for other reasons.
				 */
				if (NonIdentityTx && Values != null && Values.Transform != null)
				{
					Hash ^= Values.Transform.HashCode();
				}
			}
			return Hash;
		}

		/// <summary>
		/// Compares this <code>Font</code> object to the specified
		/// <code>Object</code>. </summary>
		/// <param name="obj"> the <code>Object</code> to compare </param>
		/// <returns> <code>true</code> if the objects are the same
		///          or if the argument is a <code>Font</code> object
		///          describing the same font as this object;
		///          <code>false</code> otherwise.
		/// @since JDK1.0 </returns>
		public override bool Equals(Object obj)
		{
			if (obj == this)
			{
				return true;
			}

			if (obj != null)
			{
				try
				{
					Font font = (Font)obj;
					if (Size_Renamed == font.Size_Renamed && Style_Renamed == font.Style_Renamed && NonIdentityTx == font.NonIdentityTx && HasLayoutAttributes_Renamed == font.HasLayoutAttributes_Renamed && PointSize == font.PointSize && Name_Renamed.Equals(font.Name_Renamed))
					{

						/* 'values' is usually initialized lazily, except when
						 * the font is constructed from a Map, or derived using
						 * a Map or other values. So if only one font has
						 * the field initialized we need to initialize it in
						 * the other instance and compare.
						 */
						if (Values == null)
						{
							if (font.Values == null)
							{
								return true;
							}
							else
							{
								return AttributeValues.Equals(font.Values);
							}
						}
						else
						{
							return Values.Equals(font.AttributeValues);
						}
					}
				}
				catch (ClassCastException)
				{
				}
			}
			return false;
		}

		/// <summary>
		/// Converts this <code>Font</code> object to a <code>String</code>
		/// representation. </summary>
		/// <returns>     a <code>String</code> representation of this
		///          <code>Font</code> object.
		/// @since      JDK1.0 </returns>
		// NOTE: This method may be called by privileged threads.
		//       DO NOT INVOKE CLIENT CODE ON THIS THREAD!
		public override String ToString()
		{
			String strStyle;

			if (Bold)
			{
				strStyle = Italic ? "bolditalic" : "bold";
			}
			else
			{
				strStyle = Italic ? "italic" : "plain";
			}

//JAVA TO C# CONVERTER WARNING: The .NET Type.FullName property will not always yield results identical to the Java Class.getName method:
			return this.GetType().FullName + "[family=" + Family + ",name=" + Name_Renamed + ",style=" + strStyle + ",size=" + Size_Renamed + "]";
		} // toString()


		/// <summary>
		/// Serialization support.  A <code>readObject</code>
		///  method is neccessary because the constructor creates
		///  the font's peer, and we can't serialize the peer.
		///  Similarly the computed font "family" may be different
		///  at <code>readObject</code> time than at
		///  <code>writeObject</code> time.  An integer version is
		///  written so that future versions of this class will be
		///  able to recognize serialized output from this one.
		/// </summary>
		/// <summary>
		/// The <code>Font</code> Serializable Data Form.
		/// 
		/// @serial
		/// </summary>
		private int FontSerializedDataVersion = 1;

		/// <summary>
		/// Writes default serializable fields to a stream.
		/// </summary>
		/// <param name="s"> the <code>ObjectOutputStream</code> to write </param>
		/// <seealso cref= AWTEventMulticaster#save(ObjectOutputStream, String, EventListener) </seealso>
		/// <seealso cref= #readObject(java.io.ObjectInputStream) </seealso>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: private void writeObject(java.io.ObjectOutputStream s) throws java.lang.ClassNotFoundException, java.io.IOException
		private void WriteObject(java.io.ObjectOutputStream s)
		{
			if (Values != null)
			{
			  lock (Values)
			  {
				// transient
				FRequestedAttributes = Values.toSerializableHashtable();
				s.DefaultWriteObject();
				FRequestedAttributes = null;
			  }
			}
			else
			{
			  s.DefaultWriteObject();
			}
		}

		/// <summary>
		/// Reads the <code>ObjectInputStream</code>.
		/// Unrecognized keys or values will be ignored.
		/// </summary>
		/// <param name="s"> the <code>ObjectInputStream</code> to read
		/// @serial </param>
		/// <seealso cref= #writeObject(java.io.ObjectOutputStream) </seealso>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: private void readObject(java.io.ObjectInputStream s) throws java.lang.ClassNotFoundException, java.io.IOException
		private void ReadObject(java.io.ObjectInputStream s)
		{
			s.DefaultReadObject();
			if (PointSize == 0)
			{
				PointSize = (float)Size_Renamed;
			}

			// Handle fRequestedAttributes.
			// in 1.5, we always streamed out the font values plus
			// TRANSFORM, SUPERSCRIPT, and WIDTH, regardless of whether the
			// values were default or not.  In 1.6 we only stream out
			// defined values.  So, 1.6 streams in from a 1.5 stream,
			// it check each of these values and 'undefines' it if the
			// value is the default.

			if (FRequestedAttributes != null)
			{
				Values = AttributeValues; // init
				AttributeValues extras = AttributeValues.fromSerializableHashtable(FRequestedAttributes);
				if (!AttributeValues.is16Hashtable(FRequestedAttributes))
				{
					extras.unsetDefault(); // if legacy stream, undefine these
				}
				Values = AttributeValues.merge(extras);
				this.NonIdentityTx = Values.anyNonDefault(EXTRA_MASK);
				this.HasLayoutAttributes_Renamed = Values.anyNonDefault(LAYOUT_MASK);

				FRequestedAttributes = null; // don't need it any more
			}
		}

		/// <summary>
		/// Returns the number of glyphs in this <code>Font</code>. Glyph codes
		/// for this <code>Font</code> range from 0 to
		/// <code>getNumGlyphs()</code> - 1. </summary>
		/// <returns> the number of glyphs in this <code>Font</code>.
		/// @since 1.2 </returns>
		public virtual int NumGlyphs
		{
			get
			{
				return Font2D.NumGlyphs;
			}
		}

		/// <summary>
		/// Returns the glyphCode which is used when this <code>Font</code>
		/// does not have a glyph for a specified unicode code point. </summary>
		/// <returns> the glyphCode of this <code>Font</code>.
		/// @since 1.2 </returns>
		public virtual int MissingGlyphCode
		{
			get
			{
				return Font2D.MissingGlyphCode;
			}
		}

		/// <summary>
		/// Returns the baseline appropriate for displaying this character.
		/// <para>
		/// Large fonts can support different writing systems, and each system can
		/// use a different baseline.
		/// The character argument determines the writing system to use. Clients
		/// should not assume all characters use the same baseline.
		/// 
		/// </para>
		/// </summary>
		/// <param name="c"> a character used to identify the writing system </param>
		/// <returns> the baseline appropriate for the specified character. </returns>
		/// <seealso cref= LineMetrics#getBaselineOffsets </seealso>
		/// <seealso cref= #ROMAN_BASELINE </seealso>
		/// <seealso cref= #CENTER_BASELINE </seealso>
		/// <seealso cref= #HANGING_BASELINE
		/// @since 1.2 </seealso>
		public virtual sbyte GetBaselineFor(char c)
		{
			return Font2D.getBaselineFor(c);
		}

		/// <summary>
		/// Returns a map of font attributes available in this
		/// <code>Font</code>.  Attributes include things like ligatures and
		/// glyph substitution. </summary>
		/// <returns> the attributes map of this <code>Font</code>. </returns>
//JAVA TO C# CONVERTER TODO TASK: Java wildcard generics are not converted to .NET:
//ORIGINAL LINE: public java.util.Map<java.awt.font.TextAttribute,?> getAttributes()
		public virtual IDictionary<TextAttribute, ?> Attributes
		{
			get
			{
				return new AttributeMap(AttributeValues);
			}
		}

		/// <summary>
		/// Returns the keys of all the attributes supported by this
		/// <code>Font</code>.  These attributes can be used to derive other
		/// fonts. </summary>
		/// <returns> an array containing the keys of all the attributes
		///          supported by this <code>Font</code>.
		/// @since 1.2 </returns>
		public virtual AttributedCharacterIterator_Attribute[] AvailableAttributes
		{
			get
			{
				// FONT is not supported by Font
    
				AttributedCharacterIterator_Attribute[] attributes = new AttributedCharacterIterator_Attribute[] {TextAttribute.FAMILY, TextAttribute.WEIGHT, TextAttribute.WIDTH, TextAttribute.POSTURE, TextAttribute.SIZE, TextAttribute.TRANSFORM, TextAttribute.SUPERSCRIPT, TextAttribute.CHAR_REPLACEMENT, TextAttribute.FOREGROUND, TextAttribute.BACKGROUND, TextAttribute.UNDERLINE, TextAttribute.STRIKETHROUGH, TextAttribute.RUN_DIRECTION, TextAttribute.BIDI_EMBEDDING, TextAttribute.JUSTIFICATION, TextAttribute.INPUT_METHOD_HIGHLIGHT, TextAttribute.INPUT_METHOD_UNDERLINE, TextAttribute.SWAP_COLORS, TextAttribute.NUMERIC_SHAPING, TextAttribute.KERNING, TextAttribute.LIGATURES, TextAttribute.TRACKING};
    
				return attributes;
			}
		}

		/// <summary>
		/// Creates a new <code>Font</code> object by replicating this
		/// <code>Font</code> object and applying a new style and size. </summary>
		/// <param name="style"> the style for the new <code>Font</code> </param>
		/// <param name="size"> the size for the new <code>Font</code> </param>
		/// <returns> a new <code>Font</code> object.
		/// @since 1.2 </returns>
		public virtual Font DeriveFont(int style, float size)
		{
			if (Values == null)
			{
				return new Font(Name_Renamed, style, size, CreatedFont, Font2DHandle);
			}
			AttributeValues newValues = AttributeValues.clone();
			int oldStyle = (this.Style_Renamed != style) ? this.Style_Renamed : -1;
			ApplyStyle(style, newValues);
			newValues.Size = size;
			return new Font(newValues, null, oldStyle, CreatedFont, Font2DHandle);
		}

		/// <summary>
		/// Creates a new <code>Font</code> object by replicating this
		/// <code>Font</code> object and applying a new style and transform. </summary>
		/// <param name="style"> the style for the new <code>Font</code> </param>
		/// <param name="trans"> the <code>AffineTransform</code> associated with the
		/// new <code>Font</code> </param>
		/// <returns> a new <code>Font</code> object. </returns>
		/// <exception cref="IllegalArgumentException"> if <code>trans</code> is
		///         <code>null</code>
		/// @since 1.2 </exception>
		public virtual Font DeriveFont(int style, AffineTransform trans)
		{
			AttributeValues newValues = AttributeValues.clone();
			int oldStyle = (this.Style_Renamed != style) ? this.Style_Renamed : -1;
			ApplyStyle(style, newValues);
			ApplyTransform(trans, newValues);
			return new Font(newValues, null, oldStyle, CreatedFont, Font2DHandle);
		}

		/// <summary>
		/// Creates a new <code>Font</code> object by replicating the current
		/// <code>Font</code> object and applying a new size to it. </summary>
		/// <param name="size"> the size for the new <code>Font</code>. </param>
		/// <returns> a new <code>Font</code> object.
		/// @since 1.2 </returns>
		public virtual Font DeriveFont(float size)
		{
			if (Values == null)
			{
				return new Font(Name_Renamed, Style_Renamed, size, CreatedFont, Font2DHandle);
			}
			AttributeValues newValues = AttributeValues.clone();
			newValues.Size = size;
			return new Font(newValues, null, -1, CreatedFont, Font2DHandle);
		}

		/// <summary>
		/// Creates a new <code>Font</code> object by replicating the current
		/// <code>Font</code> object and applying a new transform to it. </summary>
		/// <param name="trans"> the <code>AffineTransform</code> associated with the
		/// new <code>Font</code> </param>
		/// <returns> a new <code>Font</code> object. </returns>
		/// <exception cref="IllegalArgumentException"> if <code>trans</code> is
		///         <code>null</code>
		/// @since 1.2 </exception>
		public virtual Font DeriveFont(AffineTransform trans)
		{
			AttributeValues newValues = AttributeValues.clone();
			ApplyTransform(trans, newValues);
			return new Font(newValues, null, -1, CreatedFont, Font2DHandle);
		}

		/// <summary>
		/// Creates a new <code>Font</code> object by replicating the current
		/// <code>Font</code> object and applying a new style to it. </summary>
		/// <param name="style"> the style for the new <code>Font</code> </param>
		/// <returns> a new <code>Font</code> object.
		/// @since 1.2 </returns>
		public virtual Font DeriveFont(int style)
		{
			if (Values == null)
			{
			   return new Font(Name_Renamed, style, Size_Renamed, CreatedFont, Font2DHandle);
			}
			AttributeValues newValues = AttributeValues.clone();
			int oldStyle = (this.Style_Renamed != style) ? this.Style_Renamed : -1;
			ApplyStyle(style, newValues);
			return new Font(newValues, null, oldStyle, CreatedFont, Font2DHandle);
		}

		/// <summary>
		/// Creates a new <code>Font</code> object by replicating the current
		/// <code>Font</code> object and applying a new set of font attributes
		/// to it.
		/// </summary>
		/// <param name="attributes"> a map of attributes enabled for the new
		/// <code>Font</code> </param>
		/// <returns> a new <code>Font</code> object.
		/// @since 1.2 </returns>
		public virtual Font deriveFont<T1>(IDictionary<T1> attributes) where T1 : java.text.AttributedCharacterIterator_Attribute
		{
			if (attributes == null)
			{
				return this;
			}
			AttributeValues newValues = AttributeValues.clone();
			newValues.merge(attributes, RECOGNIZED_MASK);

			return new Font(newValues, Name_Renamed, Style_Renamed, CreatedFont, Font2DHandle);
		}

		/// <summary>
		/// Checks if this <code>Font</code> has a glyph for the specified
		/// character.
		/// 
		/// <para> <b>Note:</b> This method cannot handle <a
		/// href="../../java/lang/Character.html#supplementary"> supplementary
		/// characters</a>. To support all Unicode characters, including
		/// supplementary characters, use the <seealso cref="#canDisplay(int)"/>
		/// method or <code>canDisplayUpTo</code> methods.
		/// 
		/// </para>
		/// </summary>
		/// <param name="c"> the character for which a glyph is needed </param>
		/// <returns> <code>true</code> if this <code>Font</code> has a glyph for this
		///          character; <code>false</code> otherwise.
		/// @since 1.2 </returns>
		public virtual bool CanDisplay(char c)
		{
			return Font2D.canDisplay(c);
		}

		/// <summary>
		/// Checks if this <code>Font</code> has a glyph for the specified
		/// character.
		/// </summary>
		/// <param name="codePoint"> the character (Unicode code point) for which a glyph
		///        is needed. </param>
		/// <returns> <code>true</code> if this <code>Font</code> has a glyph for the
		///          character; <code>false</code> otherwise. </returns>
		/// <exception cref="IllegalArgumentException"> if the code point is not a valid Unicode
		///          code point. </exception>
		/// <seealso cref= Character#isValidCodePoint(int)
		/// @since 1.5 </seealso>
		public virtual bool CanDisplay(int codePoint)
		{
			if (!Character.IsValidCodePoint(codePoint))
			{
				throw new IllegalArgumentException("invalid code point: " + codePoint.ToString("x"));
			}
			return Font2D.canDisplay(codePoint);
		}

		/// <summary>
		/// Indicates whether or not this <code>Font</code> can display a
		/// specified <code>String</code>.  For strings with Unicode encoding,
		/// it is important to know if a particular font can display the
		/// string. This method returns an offset into the <code>String</code>
		/// <code>str</code> which is the first character this
		/// <code>Font</code> cannot display without using the missing glyph
		/// code. If the <code>Font</code> can display all characters, -1 is
		/// returned. </summary>
		/// <param name="str"> a <code>String</code> object </param>
		/// <returns> an offset into <code>str</code> that points
		///          to the first character in <code>str</code> that this
		///          <code>Font</code> cannot display; or <code>-1</code> if
		///          this <code>Font</code> can display all characters in
		///          <code>str</code>.
		/// @since 1.2 </returns>
		public virtual int CanDisplayUpTo(String str)
		{
			Font2D font2d = Font2D;
			int len = str.Length();
			for (int i = 0; i < len; i++)
			{
				char c = str.CharAt(i);
				if (font2d.canDisplay(c))
				{
					continue;
				}
				if (!char.IsHighSurrogate(c))
				{
					return i;
				}
				if (!font2d.canDisplay(str.CodePointAt(i)))
				{
					return i;
				}
				i++;
			}
			return -1;
		}

		/// <summary>
		/// Indicates whether or not this <code>Font</code> can display
		/// the characters in the specified <code>text</code>
		/// starting at <code>start</code> and ending at
		/// <code>limit</code>.  This method is a convenience overload. </summary>
		/// <param name="text"> the specified array of <code>char</code> values </param>
		/// <param name="start"> the specified starting offset (in
		///              <code>char</code>s) into the specified array of
		///              <code>char</code> values </param>
		/// <param name="limit"> the specified ending offset (in
		///              <code>char</code>s) into the specified array of
		///              <code>char</code> values </param>
		/// <returns> an offset into <code>text</code> that points
		///          to the first character in <code>text</code> that this
		///          <code>Font</code> cannot display; or <code>-1</code> if
		///          this <code>Font</code> can display all characters in
		///          <code>text</code>.
		/// @since 1.2 </returns>
		public virtual int CanDisplayUpTo(char[] text, int start, int limit)
		{
			Font2D font2d = Font2D;
			for (int i = start; i < limit; i++)
			{
				char c = text[i];
				if (font2d.canDisplay(c))
				{
					continue;
				}
				if (!char.IsHighSurrogate(c))
				{
					return i;
				}
				if (!font2d.canDisplay(Character.CodePointAt(text, i, limit)))
				{
					return i;
				}
				i++;
			}
			return -1;
		}

		/// <summary>
		/// Indicates whether or not this <code>Font</code> can display the
		/// text specified by the <code>iter</code> starting at
		/// <code>start</code> and ending at <code>limit</code>.
		/// </summary>
		/// <param name="iter">  a <seealso cref="CharacterIterator"/> object </param>
		/// <param name="start"> the specified starting offset into the specified
		///              <code>CharacterIterator</code>. </param>
		/// <param name="limit"> the specified ending offset into the specified
		///              <code>CharacterIterator</code>. </param>
		/// <returns> an offset into <code>iter</code> that points
		///          to the first character in <code>iter</code> that this
		///          <code>Font</code> cannot display; or <code>-1</code> if
		///          this <code>Font</code> can display all characters in
		///          <code>iter</code>.
		/// @since 1.2 </returns>
		public virtual int CanDisplayUpTo(CharacterIterator iter, int start, int limit)
		{
			Font2D font2d = Font2D;
			char c = iter.setIndex(start);
			for (int i = start; i < limit; i++, c = iter.Next())
			{
				if (font2d.canDisplay(c))
				{
					continue;
				}
				if (!char.IsHighSurrogate(c))
				{
					return i;
				}
				char c2 = iter.Next();
				// c2 could be CharacterIterator.DONE which is not a low surrogate.
				if (!char.IsLowSurrogate(c2))
				{
					return i;
				}
				if (!font2d.canDisplay(Character.ToCodePoint(c, c2)))
				{
					return i;
				}
				i++;
			}
			return -1;
		}

		/// <summary>
		/// Returns the italic angle of this <code>Font</code>.  The italic angle
		/// is the inverse slope of the caret which best matches the posture of this
		/// <code>Font</code>. </summary>
		/// <seealso cref= TextAttribute#POSTURE </seealso>
		/// <returns> the angle of the ITALIC style of this <code>Font</code>. </returns>
		public virtual float ItalicAngle
		{
			get
			{
				return GetItalicAngle(null);
			}
		}

		/* The FRC hints don't affect the value of the italic angle but
		 * we need to pass them in to look up a strike.
		 * If we can pass in ones already being used it can prevent an extra
		 * strike from being allocated. Note that since italic angle is
		 * a property of the font, the font transform is needed not the
		 * device transform. Finally, this is private but the only caller of this
		 * in the JDK - and the only likely caller - is in this same class.
		 */
		private float GetItalicAngle(FontRenderContext frc)
		{
			Object aa, fm;
			if (frc == null)
			{
				aa = RenderingHints.VALUE_TEXT_ANTIALIAS_OFF;
				fm = RenderingHints.VALUE_FRACTIONALMETRICS_OFF;
			}
			else
			{
				aa = frc.AntiAliasingHint;
				fm = frc.FractionalMetricsHint;
			}
			return Font2D.getItalicAngle(this, IdentityTx, aa, fm);
		}

		/// <summary>
		/// Checks whether or not this <code>Font</code> has uniform
		/// line metrics.  A logical <code>Font</code> might be a
		/// composite font, which means that it is composed of different
		/// physical fonts to cover different code ranges.  Each of these
		/// fonts might have different <code>LineMetrics</code>.  If the
		/// logical <code>Font</code> is a single
		/// font then the metrics would be uniform. </summary>
		/// <returns> <code>true</code> if this <code>Font</code> has
		/// uniform line metrics; <code>false</code> otherwise. </returns>
		public virtual bool HasUniformLineMetrics()
		{
			return false; // REMIND always safe, but prevents caller optimize
		}

		[NonSerialized]
		private SoftReference<FontLineMetrics> Flmref;
		private FontLineMetrics DefaultLineMetrics(FontRenderContext frc)
		{
			FontLineMetrics flm = null;
			if (Flmref == null || (flm = Flmref.get()) == null || !flm.frc.Equals(frc))
			{

				/* The device transform in the frc is not used in obtaining line
				 * metrics, although it probably should be: REMIND find why not?
				 * The font transform is used but its applied in getFontMetrics, so
				 * just pass identity here
				 */
				float[] metrics = new float[8];
				Font2D.getFontMetrics(this, IdentityTx, frc.AntiAliasingHint, frc.FractionalMetricsHint, metrics);
				float ascent = metrics[0];
				float descent = metrics[1];
				float leading = metrics[2];
				float ssOffset = 0;
				if (Values != null && Values.Superscript != 0)
				{
					ssOffset = (float)Transform.TranslateY;
					ascent -= ssOffset;
					descent += ssOffset;
				}
				float height = ascent + descent + leading;

				int baselineIndex = 0; // need real index, assumes roman for everything
				// need real baselines eventually
				float[] baselineOffsets = new float[] {0, (descent / 2f - ascent) / 2f, -ascent};

				float strikethroughOffset = metrics[4];
				float strikethroughThickness = metrics[5];

				float underlineOffset = metrics[6];
				float underlineThickness = metrics[7];

				float italicAngle = GetItalicAngle(frc);

				if (Transformed)
				{
					AffineTransform ctx = Values.CharTransform; // extract rotation
					if (ctx != null)
					{
						Point2D.Float pt = new Point2D.Float();
						pt.SetLocation(0, strikethroughOffset);
						ctx.DeltaTransform(pt, pt);
						strikethroughOffset = pt.y;
						pt.SetLocation(0, strikethroughThickness);
						ctx.DeltaTransform(pt, pt);
						strikethroughThickness = pt.y;
						pt.SetLocation(0, underlineOffset);
						ctx.DeltaTransform(pt, pt);
						underlineOffset = pt.y;
						pt.SetLocation(0, underlineThickness);
						ctx.DeltaTransform(pt, pt);
						underlineThickness = pt.y;
					}
				}
				strikethroughOffset += ssOffset;
				underlineOffset += ssOffset;

				CoreMetrics cm = new CoreMetrics(ascent, descent, leading, height, baselineIndex, baselineOffsets, strikethroughOffset, strikethroughThickness, underlineOffset, underlineThickness, ssOffset, italicAngle);

				flm = new FontLineMetrics(0, cm, frc);
				Flmref = new SoftReference<FontLineMetrics>(flm);
			}

			return (FontLineMetrics)flm.clone();
		}

		/// <summary>
		/// Returns a <seealso cref="LineMetrics"/> object created with the specified
		/// <code>String</code> and <seealso cref="FontRenderContext"/>. </summary>
		/// <param name="str"> the specified <code>String</code> </param>
		/// <param name="frc"> the specified <code>FontRenderContext</code> </param>
		/// <returns> a <code>LineMetrics</code> object created with the
		/// specified <code>String</code> and <seealso cref="FontRenderContext"/>. </returns>
		public virtual LineMetrics GetLineMetrics(String str, FontRenderContext frc)
		{
			FontLineMetrics flm = DefaultLineMetrics(frc);
			flm.numchars = str.Length();
			return flm;
		}

		/// <summary>
		/// Returns a <code>LineMetrics</code> object created with the
		/// specified arguments. </summary>
		/// <param name="str"> the specified <code>String</code> </param>
		/// <param name="beginIndex"> the initial offset of <code>str</code> </param>
		/// <param name="limit"> the end offset of <code>str</code> </param>
		/// <param name="frc"> the specified <code>FontRenderContext</code> </param>
		/// <returns> a <code>LineMetrics</code> object created with the
		/// specified arguments. </returns>
		public virtual LineMetrics GetLineMetrics(String str, int beginIndex, int limit, FontRenderContext frc)
		{
			FontLineMetrics flm = DefaultLineMetrics(frc);
			int numChars = limit - beginIndex;
			flm.numchars = (numChars < 0)? 0: numChars;
			return flm;
		}

		/// <summary>
		/// Returns a <code>LineMetrics</code> object created with the
		/// specified arguments. </summary>
		/// <param name="chars"> an array of characters </param>
		/// <param name="beginIndex"> the initial offset of <code>chars</code> </param>
		/// <param name="limit"> the end offset of <code>chars</code> </param>
		/// <param name="frc"> the specified <code>FontRenderContext</code> </param>
		/// <returns> a <code>LineMetrics</code> object created with the
		/// specified arguments. </returns>
		public virtual LineMetrics GetLineMetrics(char[] chars, int beginIndex, int limit, FontRenderContext frc)
		{
			FontLineMetrics flm = DefaultLineMetrics(frc);
			int numChars = limit - beginIndex;
			flm.numchars = (numChars < 0)? 0: numChars;
			return flm;
		}

		/// <summary>
		/// Returns a <code>LineMetrics</code> object created with the
		/// specified arguments. </summary>
		/// <param name="ci"> the specified <code>CharacterIterator</code> </param>
		/// <param name="beginIndex"> the initial offset in <code>ci</code> </param>
		/// <param name="limit"> the end offset of <code>ci</code> </param>
		/// <param name="frc"> the specified <code>FontRenderContext</code> </param>
		/// <returns> a <code>LineMetrics</code> object created with the
		/// specified arguments. </returns>
		public virtual LineMetrics GetLineMetrics(CharacterIterator ci, int beginIndex, int limit, FontRenderContext frc)
		{
			FontLineMetrics flm = DefaultLineMetrics(frc);
			int numChars = limit - beginIndex;
			flm.numchars = (numChars < 0)? 0: numChars;
			return flm;
		}

		/// <summary>
		/// Returns the logical bounds of the specified <code>String</code> in
		/// the specified <code>FontRenderContext</code>.  The logical bounds
		/// contains the origin, ascent, advance, and height, which includes
		/// the leading.  The logical bounds does not always enclose all the
		/// text.  For example, in some languages and in some fonts, accent
		/// marks can be positioned above the ascent or below the descent.
		/// To obtain a visual bounding box, which encloses all the text,
		/// use the <seealso cref="TextLayout#getBounds() getBounds"/> method of
		/// <code>TextLayout</code>.
		/// <para>Note: The returned bounds is in baseline-relative coordinates
		/// (see <seealso cref="java.awt.Font class notes"/>).
		/// </para>
		/// </summary>
		/// <param name="str"> the specified <code>String</code> </param>
		/// <param name="frc"> the specified <code>FontRenderContext</code> </param>
		/// <returns> a <seealso cref="Rectangle2D"/> that is the bounding box of the
		/// specified <code>String</code> in the specified
		/// <code>FontRenderContext</code>. </returns>
		/// <seealso cref= FontRenderContext </seealso>
		/// <seealso cref= Font#createGlyphVector
		/// @since 1.2 </seealso>
		public virtual Rectangle2D GetStringBounds(String str, FontRenderContext frc)
		{
			char[] array = str.ToCharArray();
			return GetStringBounds(array, 0, array.Length, frc);
		}

	   /// <summary>
	   /// Returns the logical bounds of the specified <code>String</code> in
	   /// the specified <code>FontRenderContext</code>.  The logical bounds
	   /// contains the origin, ascent, advance, and height, which includes
	   /// the leading.  The logical bounds does not always enclose all the
	   /// text.  For example, in some languages and in some fonts, accent
	   /// marks can be positioned above the ascent or below the descent.
	   /// To obtain a visual bounding box, which encloses all the text,
	   /// use the <seealso cref="TextLayout#getBounds() getBounds"/> method of
	   /// <code>TextLayout</code>.
	   /// <para>Note: The returned bounds is in baseline-relative coordinates
	   /// (see <seealso cref="java.awt.Font class notes"/>).
	   /// </para>
	   /// </summary>
	   /// <param name="str"> the specified <code>String</code> </param>
	   /// <param name="beginIndex"> the initial offset of <code>str</code> </param>
	   /// <param name="limit"> the end offset of <code>str</code> </param>
	   /// <param name="frc"> the specified <code>FontRenderContext</code> </param>
	   /// <returns> a <code>Rectangle2D</code> that is the bounding box of the
	   /// specified <code>String</code> in the specified
	   /// <code>FontRenderContext</code>. </returns>
	   /// <exception cref="IndexOutOfBoundsException"> if <code>beginIndex</code> is
	   ///         less than zero, or <code>limit</code> is greater than the
	   ///         length of <code>str</code>, or <code>beginIndex</code>
	   ///         is greater than <code>limit</code>. </exception>
	   /// <seealso cref= FontRenderContext </seealso>
	   /// <seealso cref= Font#createGlyphVector
	   /// @since 1.2 </seealso>
		public virtual Rectangle2D GetStringBounds(String str, int beginIndex, int limit, FontRenderContext frc)
		{
			String substr = str.Substring(beginIndex, limit - beginIndex);
			return GetStringBounds(substr, frc);
		}

	   /// <summary>
	   /// Returns the logical bounds of the specified array of characters
	   /// in the specified <code>FontRenderContext</code>.  The logical
	   /// bounds contains the origin, ascent, advance, and height, which
	   /// includes the leading.  The logical bounds does not always enclose
	   /// all the text.  For example, in some languages and in some fonts,
	   /// accent marks can be positioned above the ascent or below the
	   /// descent.  To obtain a visual bounding box, which encloses all the
	   /// text, use the <seealso cref="TextLayout#getBounds() getBounds"/> method of
	   /// <code>TextLayout</code>.
	   /// <para>Note: The returned bounds is in baseline-relative coordinates
	   /// (see <seealso cref="java.awt.Font class notes"/>).
	   /// </para>
	   /// </summary>
	   /// <param name="chars"> an array of characters </param>
	   /// <param name="beginIndex"> the initial offset in the array of
	   /// characters </param>
	   /// <param name="limit"> the end offset in the array of characters </param>
	   /// <param name="frc"> the specified <code>FontRenderContext</code> </param>
	   /// <returns> a <code>Rectangle2D</code> that is the bounding box of the
	   /// specified array of characters in the specified
	   /// <code>FontRenderContext</code>. </returns>
	   /// <exception cref="IndexOutOfBoundsException"> if <code>beginIndex</code> is
	   ///         less than zero, or <code>limit</code> is greater than the
	   ///         length of <code>chars</code>, or <code>beginIndex</code>
	   ///         is greater than <code>limit</code>. </exception>
	   /// <seealso cref= FontRenderContext </seealso>
	   /// <seealso cref= Font#createGlyphVector
	   /// @since 1.2 </seealso>
		public virtual Rectangle2D GetStringBounds(char[] chars, int beginIndex, int limit, FontRenderContext frc)
		{
			if (beginIndex < 0)
			{
				throw new IndexOutOfBoundsException("beginIndex: " + beginIndex);
			}
			if (limit > chars.Length)
			{
				throw new IndexOutOfBoundsException("limit: " + limit);
			}
			if (beginIndex > limit)
			{
				throw new IndexOutOfBoundsException("range length: " + (limit - beginIndex));
			}

			// this code should be in textlayout
			// quick check for simple text, assume GV ok to use if simple

			bool simple = Values == null || (Values.Kerning == 0 && Values.Ligatures == 0 && Values.BaselineTransform == null);
			if (simple)
			{
				simple = !FontUtilities.isComplexText(chars, beginIndex, limit);
			}

			if (simple)
			{
				GlyphVector gv = new StandardGlyphVector(this, chars, beginIndex, limit - beginIndex, frc);
				return gv.LogicalBounds;
			}
			else
			{
				// need char array constructor on textlayout
				String str = new String(chars, beginIndex, limit - beginIndex);
				TextLayout tl = new TextLayout(str, this, frc);
				return new Rectangle2D.Float(0, -tl.Ascent, tl.Advance, tl.Ascent + tl.Descent + tl.Leading);
			}
		}

	   /// <summary>
	   /// Returns the logical bounds of the characters indexed in the
	   /// specified <seealso cref="CharacterIterator"/> in the
	   /// specified <code>FontRenderContext</code>.  The logical bounds
	   /// contains the origin, ascent, advance, and height, which includes
	   /// the leading.  The logical bounds does not always enclose all the
	   /// text.  For example, in some languages and in some fonts, accent
	   /// marks can be positioned above the ascent or below the descent.
	   /// To obtain a visual bounding box, which encloses all the text,
	   /// use the <seealso cref="TextLayout#getBounds() getBounds"/> method of
	   /// <code>TextLayout</code>.
	   /// <para>Note: The returned bounds is in baseline-relative coordinates
	   /// (see <seealso cref="java.awt.Font class notes"/>).
	   /// </para>
	   /// </summary>
	   /// <param name="ci"> the specified <code>CharacterIterator</code> </param>
	   /// <param name="beginIndex"> the initial offset in <code>ci</code> </param>
	   /// <param name="limit"> the end offset in <code>ci</code> </param>
	   /// <param name="frc"> the specified <code>FontRenderContext</code> </param>
	   /// <returns> a <code>Rectangle2D</code> that is the bounding box of the
	   /// characters indexed in the specified <code>CharacterIterator</code>
	   /// in the specified <code>FontRenderContext</code>. </returns>
	   /// <seealso cref= FontRenderContext </seealso>
	   /// <seealso cref= Font#createGlyphVector
	   /// @since 1.2 </seealso>
	   /// <exception cref="IndexOutOfBoundsException"> if <code>beginIndex</code> is
	   ///         less than the start index of <code>ci</code>, or
	   ///         <code>limit</code> is greater than the end index of
	   ///         <code>ci</code>, or <code>beginIndex</code> is greater
	   ///         than <code>limit</code> </exception>
		public virtual Rectangle2D GetStringBounds(CharacterIterator ci, int beginIndex, int limit, FontRenderContext frc)
		{
			int start = ci.BeginIndex;
			int end = ci.EndIndex;

			if (beginIndex < start)
			{
				throw new IndexOutOfBoundsException("beginIndex: " + beginIndex);
			}
			if (limit > end)
			{
				throw new IndexOutOfBoundsException("limit: " + limit);
			}
			if (beginIndex > limit)
			{
				throw new IndexOutOfBoundsException("range length: " + (limit - beginIndex));
			}

			char[] arr = new char[limit - beginIndex];

			ci.Index = beginIndex;
			for (int idx = 0; idx < arr.Length; idx++)
			{
				arr[idx] = ci.Current();
				ci.Next();
			}

			return GetStringBounds(arr,0,arr.Length,frc);
		}

		/// <summary>
		/// Returns the bounds for the character with the maximum
		/// bounds as defined in the specified <code>FontRenderContext</code>.
		/// <para>Note: The returned bounds is in baseline-relative coordinates
		/// (see <seealso cref="java.awt.Font class notes"/>).
		/// </para>
		/// </summary>
		/// <param name="frc"> the specified <code>FontRenderContext</code> </param>
		/// <returns> a <code>Rectangle2D</code> that is the bounding box
		/// for the character with the maximum bounds. </returns>
		public virtual Rectangle2D GetMaxCharBounds(FontRenderContext frc)
		{
			float[] metrics = new float[4];

			Font2D.getFontMetrics(this, frc, metrics);

			return new Rectangle2D.Float(0, -metrics[0], metrics[3], metrics[0] + metrics[1] + metrics[2]);
		}

		/// <summary>
		/// Creates a <seealso cref="java.awt.font.GlyphVector GlyphVector"/> by
		/// mapping characters to glyphs one-to-one based on the
		/// Unicode cmap in this <code>Font</code>.  This method does no other
		/// processing besides the mapping of glyphs to characters.  This
		/// means that this method is not useful for some scripts, such
		/// as Arabic, Hebrew, Thai, and Indic, that require reordering,
		/// shaping, or ligature substitution. </summary>
		/// <param name="frc"> the specified <code>FontRenderContext</code> </param>
		/// <param name="str"> the specified <code>String</code> </param>
		/// <returns> a new <code>GlyphVector</code> created with the
		/// specified <code>String</code> and the specified
		/// <code>FontRenderContext</code>. </returns>
		public virtual GlyphVector CreateGlyphVector(FontRenderContext frc, String str)
		{
			return (GlyphVector)new StandardGlyphVector(this, str, frc);
		}

		/// <summary>
		/// Creates a <seealso cref="java.awt.font.GlyphVector GlyphVector"/> by
		/// mapping characters to glyphs one-to-one based on the
		/// Unicode cmap in this <code>Font</code>.  This method does no other
		/// processing besides the mapping of glyphs to characters.  This
		/// means that this method is not useful for some scripts, such
		/// as Arabic, Hebrew, Thai, and Indic, that require reordering,
		/// shaping, or ligature substitution. </summary>
		/// <param name="frc"> the specified <code>FontRenderContext</code> </param>
		/// <param name="chars"> the specified array of characters </param>
		/// <returns> a new <code>GlyphVector</code> created with the
		/// specified array of characters and the specified
		/// <code>FontRenderContext</code>. </returns>
		public virtual GlyphVector CreateGlyphVector(FontRenderContext frc, char[] chars)
		{
			return (GlyphVector)new StandardGlyphVector(this, chars, frc);
		}

		/// <summary>
		/// Creates a <seealso cref="java.awt.font.GlyphVector GlyphVector"/> by
		/// mapping the specified characters to glyphs one-to-one based on the
		/// Unicode cmap in this <code>Font</code>.  This method does no other
		/// processing besides the mapping of glyphs to characters.  This
		/// means that this method is not useful for some scripts, such
		/// as Arabic, Hebrew, Thai, and Indic, that require reordering,
		/// shaping, or ligature substitution. </summary>
		/// <param name="frc"> the specified <code>FontRenderContext</code> </param>
		/// <param name="ci"> the specified <code>CharacterIterator</code> </param>
		/// <returns> a new <code>GlyphVector</code> created with the
		/// specified <code>CharacterIterator</code> and the specified
		/// <code>FontRenderContext</code>. </returns>
		public virtual GlyphVector CreateGlyphVector(FontRenderContext frc, CharacterIterator ci)
		{
			return (GlyphVector)new StandardGlyphVector(this, ci, frc);
		}

		/// <summary>
		/// Creates a <seealso cref="java.awt.font.GlyphVector GlyphVector"/> by
		/// mapping characters to glyphs one-to-one based on the
		/// Unicode cmap in this <code>Font</code>.  This method does no other
		/// processing besides the mapping of glyphs to characters.  This
		/// means that this method is not useful for some scripts, such
		/// as Arabic, Hebrew, Thai, and Indic, that require reordering,
		/// shaping, or ligature substitution. </summary>
		/// <param name="frc"> the specified <code>FontRenderContext</code> </param>
		/// <param name="glyphCodes"> the specified integer array </param>
		/// <returns> a new <code>GlyphVector</code> created with the
		/// specified integer array and the specified
		/// <code>FontRenderContext</code>. </returns>
		public virtual GlyphVector CreateGlyphVector(FontRenderContext frc, int[] glyphCodes)
		{
			return (GlyphVector)new StandardGlyphVector(this, glyphCodes, frc);
		}

		/// <summary>
		/// Returns a new <code>GlyphVector</code> object, performing full
		/// layout of the text if possible.  Full layout is required for
		/// complex text, such as Arabic or Hindi.  Support for different
		/// scripts depends on the font and implementation.
		/// <para>
		/// Layout requires bidi analysis, as performed by
		/// <code>Bidi</code>, and should only be performed on text that
		/// has a uniform direction.  The direction is indicated in the
		/// flags parameter,by using LAYOUT_RIGHT_TO_LEFT to indicate a
		/// right-to-left (Arabic and Hebrew) run direction, or
		/// LAYOUT_LEFT_TO_RIGHT to indicate a left-to-right (English)
		/// run direction.
		/// </para>
		/// <para>
		/// In addition, some operations, such as Arabic shaping, require
		/// context, so that the characters at the start and limit can have
		/// the proper shapes.  Sometimes the data in the buffer outside
		/// the provided range does not have valid data.  The values
		/// LAYOUT_NO_START_CONTEXT and LAYOUT_NO_LIMIT_CONTEXT can be
		/// added to the flags parameter to indicate that the text before
		/// start, or after limit, respectively, should not be examined
		/// for context.
		/// </para>
		/// <para>
		/// All other values for the flags parameter are reserved.
		/// 
		/// </para>
		/// </summary>
		/// <param name="frc"> the specified <code>FontRenderContext</code> </param>
		/// <param name="text"> the text to layout </param>
		/// <param name="start"> the start of the text to use for the <code>GlyphVector</code> </param>
		/// <param name="limit"> the limit of the text to use for the <code>GlyphVector</code> </param>
		/// <param name="flags"> control flags as described above </param>
		/// <returns> a new <code>GlyphVector</code> representing the text between
		/// start and limit, with glyphs chosen and positioned so as to best represent
		/// the text </returns>
		/// <exception cref="ArrayIndexOutOfBoundsException"> if start or limit is
		/// out of bounds </exception>
		/// <seealso cref= java.text.Bidi </seealso>
		/// <seealso cref= #LAYOUT_LEFT_TO_RIGHT </seealso>
		/// <seealso cref= #LAYOUT_RIGHT_TO_LEFT </seealso>
		/// <seealso cref= #LAYOUT_NO_START_CONTEXT </seealso>
		/// <seealso cref= #LAYOUT_NO_LIMIT_CONTEXT
		/// @since 1.4 </seealso>
		public virtual GlyphVector LayoutGlyphVector(FontRenderContext frc, char[] text, int start, int limit, int flags)
		{

			GlyphLayout gl = GlyphLayout.get(null); // !!! no custom layout engines
			StandardGlyphVector gv = gl.layout(this, frc, text, start, limit - start, flags, null);
			GlyphLayout.done(gl);
			return gv;
		}

		/// <summary>
		/// A flag to layoutGlyphVector indicating that text is left-to-right as
		/// determined by Bidi analysis.
		/// </summary>
		public const int LAYOUT_LEFT_TO_RIGHT = 0;

		/// <summary>
		/// A flag to layoutGlyphVector indicating that text is right-to-left as
		/// determined by Bidi analysis.
		/// </summary>
		public const int LAYOUT_RIGHT_TO_LEFT = 1;

		/// <summary>
		/// A flag to layoutGlyphVector indicating that text in the char array
		/// before the indicated start should not be examined.
		/// </summary>
		public const int LAYOUT_NO_START_CONTEXT = 2;

		/// <summary>
		/// A flag to layoutGlyphVector indicating that text in the char array
		/// after the indicated limit should not be examined.
		/// </summary>
		public const int LAYOUT_NO_LIMIT_CONTEXT = 4;


		private static void ApplyTransform(AffineTransform trans, AttributeValues values)
		{
			if (trans == null)
			{
				throw new IllegalArgumentException("transform must not be null");
			}
			values.Transform = trans;
		}

		private static void ApplyStyle(int style, AttributeValues values)
		{
			// WEIGHT_BOLD, WEIGHT_REGULAR
			values.Weight = (style & BOLD) != 0 ? 2f : 1f;
			// POSTURE_OBLIQUE, POSTURE_REGULAR
			values.Posture = (style & ITALIC) != 0 ?.2f : 0f;
		}

		/*
		 * Initialize JNI field and method IDs
		 */
//JAVA TO C# CONVERTER TODO TASK: Replace 'unknown' with the appropriate dll name:
		[DllImport("unknown")]
		private static extern void initIDs();
	}

}
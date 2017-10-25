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

namespace java.awt.image
{

	using CMSManager = sun.java2d.cmm.CMSManager;
	using ColorTransform = sun.java2d.cmm.ColorTransform;
	using PCMM = sun.java2d.cmm.PCMM;

	/// <summary>
	/// The <code>ColorModel</code> abstract class encapsulates the
	/// methods for translating a pixel value to color components
	/// (for example, red, green, and blue) and an alpha component.
	/// In order to render an image to the screen, a printer, or another
	/// image, pixel values must be converted to color and alpha components.
	/// As arguments to or return values from methods of this class,
	/// pixels are represented as 32-bit ints or as arrays of primitive types.
	/// The number, order, and interpretation of color components for a
	/// <code>ColorModel</code> is specified by its <code>ColorSpace</code>.
	/// A <code>ColorModel</code> used with pixel data that does not include
	/// alpha information treats all pixels as opaque, which is an alpha
	/// value of 1.0.
	/// <para>
	/// This <code>ColorModel</code> class supports two representations of
	/// pixel values.  A pixel value can be a single 32-bit int or an
	/// array of primitive types.  The Java(tm) Platform 1.0 and 1.1 APIs
	/// represented pixels as single <code>byte</code> or single
	/// <code>int</code> values.  For purposes of the <code>ColorModel</code>
	/// class, pixel value arguments were passed as ints.  The Java(tm) 2
	/// Platform API introduced additional classes for representing images.
	/// With <seealso cref="BufferedImage"/> or <seealso cref="RenderedImage"/>
	/// objects, based on <seealso cref="Raster"/> and <seealso cref="SampleModel"/> classes, pixel
	/// values might not be conveniently representable as a single int.
	/// Consequently, <code>ColorModel</code> now has methods that accept
	/// pixel values represented as arrays of primitive types.  The primitive
	/// type used by a particular <code>ColorModel</code> object is called its
	/// transfer type.
	/// </para>
	/// <para>
	/// <code>ColorModel</code> objects used with images for which pixel values
	/// are not conveniently representable as a single int throw an
	/// <seealso cref="IllegalArgumentException"/> when methods taking a single int pixel
	/// argument are called.  Subclasses of <code>ColorModel</code> must
	/// specify the conditions under which this occurs.  This does not
	/// occur with <seealso cref="DirectColorModel"/> or <seealso cref="IndexColorModel"/> objects.
	/// </para>
	/// <para>
	/// Currently, the transfer types supported by the Java 2D(tm) API are
	/// DataBuffer.TYPE_BYTE, DataBuffer.TYPE_USHORT, DataBuffer.TYPE_INT,
	/// DataBuffer.TYPE_SHORT, DataBuffer.TYPE_FLOAT, and DataBuffer.TYPE_DOUBLE.
	/// Most rendering operations will perform much faster when using ColorModels
	/// and images based on the first three of these types.  In addition, some
	/// image filtering operations are not supported for ColorModels and
	/// images based on the latter three types.
	/// The transfer type for a particular <code>ColorModel</code> object is
	/// specified when the object is created, either explicitly or by default.
	/// All subclasses of <code>ColorModel</code> must specify what the
	/// possible transfer types are and how the number of elements in the
	/// primitive arrays representing pixels is determined.
	/// </para>
	/// <para>
	/// For <code>BufferedImages</code>, the transfer type of its
	/// <code>Raster</code> and of the <code>Raster</code> object's
	/// <code>SampleModel</code> (available from the
	/// <code>getTransferType</code> methods of these classes) must match that
	/// of the <code>ColorModel</code>.  The number of elements in an array
	/// representing a pixel for the <code>Raster</code> and
	/// <code>SampleModel</code> (available from the
	/// <code>getNumDataElements</code> methods of these classes) must match
	/// that of the <code>ColorModel</code>.
	/// </para>
	/// <para>
	/// The algorithm used to convert from pixel values to color and alpha
	/// components varies by subclass.  For example, there is not necessarily
	/// a one-to-one correspondence between samples obtained from the
	/// <code>SampleModel</code> of a <code>BufferedImage</code> object's
	/// <code>Raster</code> and color/alpha components.  Even when
	/// there is such a correspondence, the number of bits in a sample is not
	/// necessarily the same as the number of bits in the corresponding color/alpha
	/// component.  Each subclass must specify how the translation from
	/// pixel values to color/alpha components is done.
	/// </para>
	/// <para>
	/// Methods in the <code>ColorModel</code> class use two different
	/// representations of color and alpha components - a normalized form
	/// and an unnormalized form.  In the normalized form, each component is a
	/// <code>float</code> value between some minimum and maximum values.  For
	/// the alpha component, the minimum is 0.0 and the maximum is 1.0.  For
	/// color components the minimum and maximum values for each component can
	/// be obtained from the <code>ColorSpace</code> object.  These values
	/// will often be 0.0 and 1.0 (e.g. normalized component values for the
	/// default sRGB color space range from 0.0 to 1.0), but some color spaces
	/// have component values with different upper and lower limits.  These
	/// limits can be obtained using the <code>getMinValue</code> and
	/// <code>getMaxValue</code> methods of the <code>ColorSpace</code>
	/// class.  Normalized color component values are not premultiplied.
	/// All <code>ColorModels</code> must support the normalized form.
	/// </para>
	/// <para>
	/// In the unnormalized
	/// form, each component is an unsigned integral value between 0 and
	/// 2<sup>n</sup> - 1, where n is the number of significant bits for a
	/// particular component.  If pixel values for a particular
	/// <code>ColorModel</code> represent color samples premultiplied by
	/// the alpha sample, unnormalized color component values are
	/// also premultiplied.  The unnormalized form is used only with instances
	/// of <code>ColorModel</code> whose <code>ColorSpace</code> has minimum
	/// component values of 0.0 for all components and maximum values of
	/// 1.0 for all components.
	/// The unnormalized form for color and alpha components can be a convenient
	/// representation for <code>ColorModels</code> whose normalized component
	/// values all lie
	/// between 0.0 and 1.0.  In such cases the integral value 0 maps to 0.0 and
	/// the value 2<sup>n</sup> - 1 maps to 1.0.  In other cases, such as
	/// when the normalized component values can be either negative or positive,
	/// the unnormalized form is not convenient.  Such <code>ColorModel</code>
	/// objects throw an <seealso cref="IllegalArgumentException"/> when methods involving
	/// an unnormalized argument are called.  Subclasses of <code>ColorModel</code>
	/// must specify the conditions under which this occurs.
	/// 
	/// </para>
	/// </summary>
	/// <seealso cref= IndexColorModel </seealso>
	/// <seealso cref= ComponentColorModel </seealso>
	/// <seealso cref= PackedColorModel </seealso>
	/// <seealso cref= DirectColorModel </seealso>
	/// <seealso cref= java.awt.Image </seealso>
	/// <seealso cref= BufferedImage </seealso>
	/// <seealso cref= RenderedImage </seealso>
	/// <seealso cref= java.awt.color.ColorSpace </seealso>
	/// <seealso cref= SampleModel </seealso>
	/// <seealso cref= Raster </seealso>
	/// <seealso cref= DataBuffer </seealso>
	public abstract class ColorModel : Transparency
	{
		private long PData; // Placeholder for data for native functions

		/// <summary>
		/// The total number of bits in the pixel.
		/// </summary>
		protected internal int Pixel_bits;
		internal int[] NBits;
		internal int Transparency_Renamed = java.awt.Transparency_Fields.TRANSLUCENT;
		internal bool SupportsAlpha = true;
		internal bool IsAlphaPremultiplied = false;
		internal int NumComponents_Renamed = -1;
		internal int NumColorComponents_Renamed = -1;
		internal ColorSpace ColorSpace_Renamed = ColorSpace.GetInstance(ColorSpace.CS_sRGB_Renamed);
		internal int ColorSpaceType = ColorSpace.TYPE_RGB;
		internal int MaxBits;
		internal bool Is_sRGB = true;

		/// <summary>
		/// Data type of the array used to represent pixel values.
		/// </summary>
		protected internal int TransferType_Renamed;

		/// <summary>
		/// This is copied from java.awt.Toolkit since we need the library
		/// loaded in java.awt.image also:
		/// 
		/// WARNING: This is a temporary workaround for a problem in the
		/// way the AWT loads native libraries. A number of classes in the
		/// AWT package have a native method, initIDs(), which initializes
		/// the JNI field and method ids used in the native portion of
		/// their implementation.
		/// 
		/// Since the use and storage of these ids is done by the
		/// implementation libraries, the implementation of these method is
		/// provided by the particular AWT implementations (for example,
		/// "Toolkit"s/Peer), such as Motif, Microsoft Windows, or Tiny. The
		/// problem is that this means that the native libraries must be
		/// loaded by the java.* classes, which do not necessarily know the
		/// names of the libraries to load. A better way of doing this
		/// would be to provide a separate library which defines java.awt.*
		/// initIDs, and exports the relevant symbols out to the
		/// implementation libraries.
		/// 
		/// For now, we know it's done by the implementation, and we assume
		/// that the name of the library is "awt".  -br.
		/// </summary>
		private static bool Loaded = false;
		internal static void LoadLibraries()
		{
			if (!Loaded)
			{
				java.security.AccessController.doPrivileged(new PrivilegedActionAnonymousInnerClassHelper());
				Loaded = true;
			}
		}

		private class PrivilegedActionAnonymousInnerClassHelper : java.security.PrivilegedAction<Void>
		{
			public PrivilegedActionAnonymousInnerClassHelper()
			{
			}

			public virtual Void Run()
			{
//JAVA TO C# CONVERTER TODO TASK: The library is specified in the 'DllImport' attribute for .NET:
//				System.loadLibrary("awt");
				return null;
			}
		}
//JAVA TO C# CONVERTER TODO TASK: Replace 'unknown' with the appropriate dll name:
		[DllImport("unknown")]
		private static extern void initIDs();
		static ColorModel()
		{
			/* ensure that the proper libraries are loaded */
			LoadLibraries();
			initIDs();
		}
		private static ColorModel RGBdefault_Renamed;

		/// <summary>
		/// Returns a <code>DirectColorModel</code> that describes the default
		/// format for integer RGB values used in many of the methods in the
		/// AWT image interfaces for the convenience of the programmer.
		/// The color space is the default <seealso cref="ColorSpace"/>, sRGB.
		/// The format for the RGB values is an integer with 8 bits
		/// each of alpha, red, green, and blue color components ordered
		/// correspondingly from the most significant byte to the least
		/// significant byte, as in:  0xAARRGGBB.  Color components are
		/// not premultiplied by the alpha component.  This format does not
		/// necessarily represent the native or the most efficient
		/// <code>ColorModel</code> for a particular device or for all images.
		/// It is merely used as a common color model format. </summary>
		/// <returns> a <code>DirectColorModel</code>object describing default
		///          RGB values. </returns>
		public static ColorModel RGBdefault
		{
			get
			{
				if (RGBdefault_Renamed == null)
				{
					RGBdefault_Renamed = new DirectColorModel(32, 0x00ff0000, 0x0000ff00, 0x000000ff, unchecked((int)0xff000000)); // Alpha -  Blue -  Green -  Red
				}
				return RGBdefault_Renamed;
			}
		}

		/// <summary>
		/// Constructs a <code>ColorModel</code> that translates pixels of the
		/// specified number of bits to color/alpha components.  The color
		/// space is the default RGB <code>ColorSpace</code>, which is sRGB.
		/// Pixel values are assumed to include alpha information.  If color
		/// and alpha information are represented in the pixel value as
		/// separate spatial bands, the color bands are assumed not to be
		/// premultiplied with the alpha value. The transparency type is
		/// java.awt.Transparency.TRANSLUCENT.  The transfer type will be the
		/// smallest of DataBuffer.TYPE_BYTE, DataBuffer.TYPE_USHORT,
		/// or DataBuffer.TYPE_INT that can hold a single pixel
		/// (or DataBuffer.TYPE_UNDEFINED if bits is greater
		/// than 32).  Since this constructor has no information about the
		/// number of bits per color and alpha component, any subclass calling
		/// this constructor should override any method that requires this
		/// information. </summary>
		/// <param name="bits"> the number of bits of a pixel </param>
		/// <exception cref="IllegalArgumentException"> if the number
		///          of bits in <code>bits</code> is less than 1 </exception>
		public ColorModel(int bits)
		{
			Pixel_bits = bits;
			if (bits < 1)
			{
				throw new IllegalArgumentException("Number of bits must be > 0");
			}
			NumComponents_Renamed = 4;
			NumColorComponents_Renamed = 3;
			MaxBits = bits;
			// REMIND: make sure transferType is set correctly
			TransferType_Renamed = ColorModel.GetDefaultTransferType(bits);
		}

		/// <summary>
		/// Constructs a <code>ColorModel</code> that translates pixel values
		/// to color/alpha components.  Color components will be in the
		/// specified <code>ColorSpace</code>. <code>pixel_bits</code> is the
		/// number of bits in the pixel values.  The bits array
		/// specifies the number of significant bits per color and alpha component.
		/// Its length should be the number of components in the
		/// <code>ColorSpace</code> if there is no alpha information in the
		/// pixel values, or one more than this number if there is alpha
		/// information.  <code>hasAlpha</code> indicates whether or not alpha
		/// information is present.  The <code>boolean</code>
		/// <code>isAlphaPremultiplied</code> specifies how to interpret pixel
		/// values in which color and alpha information are represented as
		/// separate spatial bands.  If the <code>boolean</code>
		/// is <code>true</code>, color samples are assumed to have been
		/// multiplied by the alpha sample.  The <code>transparency</code>
		/// specifies what alpha values can be represented by this color model.
		/// The transfer type is the type of primitive array used to represent
		/// pixel values.  Note that the bits array contains the number of
		/// significant bits per color/alpha component after the translation
		/// from pixel values.  For example, for an
		/// <code>IndexColorModel</code> with <code>pixel_bits</code> equal to
		/// 16, the bits array might have four elements with each element set
		/// to 8. </summary>
		/// <param name="pixel_bits"> the number of bits in the pixel values </param>
		/// <param name="bits"> array that specifies the number of significant bits
		///          per color and alpha component </param>
		/// <param name="cspace"> the specified <code>ColorSpace</code> </param>
		/// <param name="hasAlpha"> <code>true</code> if alpha information is present;
		///          <code>false</code> otherwise </param>
		/// <param name="isAlphaPremultiplied"> <code>true</code> if color samples are
		///          assumed to be premultiplied by the alpha samples;
		///          <code>false</code> otherwise </param>
		/// <param name="transparency"> what alpha values can be represented by this
		///          color model </param>
		/// <param name="transferType"> the type of the array used to represent pixel
		///          values </param>
		/// <exception cref="IllegalArgumentException"> if the length of
		///          the bit array is less than the number of color or alpha
		///          components in this <code>ColorModel</code>, or if the
		///          transparency is not a valid value. </exception>
		/// <exception cref="IllegalArgumentException"> if the sum of the number
		///          of bits in <code>bits</code> is less than 1 or if
		///          any of the elements in <code>bits</code> is less than 0. </exception>
		/// <seealso cref= java.awt.Transparency </seealso>
		protected internal ColorModel(int pixel_bits, int[] bits, ColorSpace cspace, bool hasAlpha, bool isAlphaPremultiplied, int transparency, int transferType)
		{
			ColorSpace_Renamed = cspace;
			ColorSpaceType = cspace.Type;
			NumColorComponents_Renamed = cspace.NumComponents;
			NumComponents_Renamed = NumColorComponents_Renamed + (hasAlpha ? 1 : 0);
			SupportsAlpha = hasAlpha;
			if (bits.Length < NumComponents_Renamed)
			{
				throw new IllegalArgumentException("Number of color/alpha " + "components should be " + NumComponents_Renamed + " but length of bits array is " + bits.Length);
			}

			// 4186669
			if (transparency < java.awt.Transparency_Fields.OPAQUE || transparency > java.awt.Transparency_Fields.TRANSLUCENT)
			{
				throw new IllegalArgumentException("Unknown transparency: " + transparency);
			}

			if (SupportsAlpha == false)
			{
				this.IsAlphaPremultiplied = false;
				this.Transparency_Renamed = java.awt.Transparency_Fields.OPAQUE;
			}
			else
			{
				this.IsAlphaPremultiplied = isAlphaPremultiplied;
				this.Transparency_Renamed = transparency;
			}

			NBits = bits.clone();
			this.Pixel_bits = pixel_bits;
			if (pixel_bits <= 0)
			{
				throw new IllegalArgumentException("Number of pixel bits must " + "be > 0");
			}
			// Check for bits < 0
			MaxBits = 0;
			for (int i = 0; i < bits.Length; i++)
			{
				// bug 4304697
				if (bits[i] < 0)
				{
					throw new IllegalArgumentException("Number of bits must be >= 0");
				}
				if (MaxBits < bits[i])
				{
					MaxBits = bits[i];
				}
			}

			// Make sure that we don't have all 0-bit components
			if (MaxBits == 0)
			{
				throw new IllegalArgumentException("There must be at least " + "one component with > 0 " + "pixel bits.");
			}

			// Save this since we always need to check if it is the default CS
			if (cspace != ColorSpace.GetInstance(ColorSpace.CS_sRGB_Renamed))
			{
				Is_sRGB = false;
			}

			// Save the transfer type
			this.TransferType_Renamed = transferType;
		}

		/// <summary>
		/// Returns whether or not alpha is supported in this
		/// <code>ColorModel</code>. </summary>
		/// <returns> <code>true</code> if alpha is supported in this
		/// <code>ColorModel</code>; <code>false</code> otherwise. </returns>
		public bool HasAlpha()
		{
			return SupportsAlpha;
		}

		/// <summary>
		/// Returns whether or not the alpha has been premultiplied in the
		/// pixel values to be translated by this <code>ColorModel</code>.
		/// If the boolean is <code>true</code>, this <code>ColorModel</code>
		/// is to be used to interpret pixel values in which color and alpha
		/// information are represented as separate spatial bands, and color
		/// samples are assumed to have been multiplied by the
		/// alpha sample. </summary>
		/// <returns> <code>true</code> if the alpha values are premultiplied
		///          in the pixel values to be translated by this
		///          <code>ColorModel</code>; <code>false</code> otherwise. </returns>
		public bool AlphaPremultiplied
		{
			get
			{
				return IsAlphaPremultiplied;
			}
		}

		/// <summary>
		/// Returns the transfer type of this <code>ColorModel</code>.
		/// The transfer type is the type of primitive array used to represent
		/// pixel values as arrays. </summary>
		/// <returns> the transfer type.
		/// @since 1.3 </returns>
		public int TransferType
		{
			get
			{
				return TransferType_Renamed;
			}
		}

		/// <summary>
		/// Returns the number of bits per pixel described by this
		/// <code>ColorModel</code>. </summary>
		/// <returns> the number of bits per pixel. </returns>
		public virtual int PixelSize
		{
			get
			{
				return Pixel_bits;
			}
		}

		/// <summary>
		/// Returns the number of bits for the specified color/alpha component.
		/// Color components are indexed in the order specified by the
		/// <code>ColorSpace</code>.  Typically, this order reflects the name
		/// of the color space type. For example, for TYPE_RGB, index 0
		/// corresponds to red, index 1 to green, and index 2
		/// to blue.  If this <code>ColorModel</code> supports alpha, the alpha
		/// component corresponds to the index following the last color
		/// component. </summary>
		/// <param name="componentIdx"> the index of the color/alpha component </param>
		/// <returns> the number of bits for the color/alpha component at the
		///          specified index. </returns>
		/// <exception cref="ArrayIndexOutOfBoundsException"> if <code>componentIdx</code>
		///         is greater than the number of components or
		///         less than zero </exception>
		/// <exception cref="NullPointerException"> if the number of bits array is
		///         <code>null</code> </exception>
		public virtual int GetComponentSize(int componentIdx)
		{
			// REMIND:
			if (NBits == null)
			{
				throw new NullPointerException("Number of bits array is null.");
			}

			return NBits[componentIdx];
		}

		/// <summary>
		/// Returns an array of the number of bits per color/alpha component.
		/// The array contains the color components in the order specified by the
		/// <code>ColorSpace</code>, followed by the alpha component, if
		/// present. </summary>
		/// <returns> an array of the number of bits per color/alpha component </returns>
		public virtual int[] ComponentSize
		{
			get
			{
				if (NBits != null)
				{
					return NBits.clone();
				}
    
				return null;
			}
		}

		/// <summary>
		/// Returns the transparency.  Returns either OPAQUE, BITMASK,
		/// or TRANSLUCENT. </summary>
		/// <returns> the transparency of this <code>ColorModel</code>. </returns>
		/// <seealso cref= Transparency#OPAQUE </seealso>
		/// <seealso cref= Transparency#BITMASK </seealso>
		/// <seealso cref= Transparency#TRANSLUCENT </seealso>
		public virtual int Transparency
		{
			get
			{
				return Transparency_Renamed;
			}
		}

		/// <summary>
		/// Returns the number of components, including alpha, in this
		/// <code>ColorModel</code>.  This is equal to the number of color
		/// components, optionally plus one, if there is an alpha component. </summary>
		/// <returns> the number of components in this <code>ColorModel</code> </returns>
		public virtual int NumComponents
		{
			get
			{
				return NumComponents_Renamed;
			}
		}

		/// <summary>
		/// Returns the number of color components in this
		/// <code>ColorModel</code>.
		/// This is the number of components returned by
		/// <seealso cref="ColorSpace#getNumComponents"/>. </summary>
		/// <returns> the number of color components in this
		/// <code>ColorModel</code>. </returns>
		/// <seealso cref= ColorSpace#getNumComponents </seealso>
		public virtual int NumColorComponents
		{
			get
			{
				return NumColorComponents_Renamed;
			}
		}

		/// <summary>
		/// Returns the red color component for the specified pixel, scaled
		/// from 0 to 255 in the default RGB ColorSpace, sRGB.  A color conversion
		/// is done if necessary.  The pixel value is specified as an int.
		/// An <code>IllegalArgumentException</code> is thrown if pixel
		/// values for this <code>ColorModel</code> are not conveniently
		/// representable as a single int.  The returned value is not a
		/// pre-multiplied value.  For example, if the
		/// alpha is premultiplied, this method divides it out before returning
		/// the value.  If the alpha value is 0, the red value is 0. </summary>
		/// <param name="pixel"> a specified pixel </param>
		/// <returns> the value of the red component of the specified pixel. </returns>
		public abstract int GetRed(int pixel);

		/// <summary>
		/// Returns the green color component for the specified pixel, scaled
		/// from 0 to 255 in the default RGB ColorSpace, sRGB.  A color conversion
		/// is done if necessary.  The pixel value is specified as an int.
		/// An <code>IllegalArgumentException</code> is thrown if pixel
		/// values for this <code>ColorModel</code> are not conveniently
		/// representable as a single int.  The returned value is a non
		/// pre-multiplied value.  For example, if the alpha is premultiplied,
		/// this method divides it out before returning
		/// the value.  If the alpha value is 0, the green value is 0. </summary>
		/// <param name="pixel"> the specified pixel </param>
		/// <returns> the value of the green component of the specified pixel. </returns>
		public abstract int GetGreen(int pixel);

		/// <summary>
		/// Returns the blue color component for the specified pixel, scaled
		/// from 0 to 255 in the default RGB ColorSpace, sRGB.  A color conversion
		/// is done if necessary.  The pixel value is specified as an int.
		/// An <code>IllegalArgumentException</code> is thrown if pixel values
		/// for this <code>ColorModel</code> are not conveniently representable
		/// as a single int.  The returned value is a non pre-multiplied
		/// value, for example, if the alpha is premultiplied, this method
		/// divides it out before returning the value.  If the alpha value is
		/// 0, the blue value is 0. </summary>
		/// <param name="pixel"> the specified pixel </param>
		/// <returns> the value of the blue component of the specified pixel. </returns>
		public abstract int GetBlue(int pixel);

		/// <summary>
		/// Returns the alpha component for the specified pixel, scaled
		/// from 0 to 255.  The pixel value is specified as an int.
		/// An <code>IllegalArgumentException</code> is thrown if pixel
		/// values for this <code>ColorModel</code> are not conveniently
		/// representable as a single int. </summary>
		/// <param name="pixel"> the specified pixel </param>
		/// <returns> the value of alpha component of the specified pixel. </returns>
		public abstract int GetAlpha(int pixel);

		/// <summary>
		/// Returns the color/alpha components of the pixel in the default
		/// RGB color model format.  A color conversion is done if necessary.
		/// The pixel value is specified as an int.
		/// An <code>IllegalArgumentException</code> thrown if pixel values
		/// for this <code>ColorModel</code> are not conveniently representable
		/// as a single int.  The returned value is in a non
		/// pre-multiplied format. For example, if the alpha is premultiplied,
		/// this method divides it out of the color components.  If the alpha
		/// value is 0, the color values are 0. </summary>
		/// <param name="pixel"> the specified pixel </param>
		/// <returns> the RGB value of the color/alpha components of the
		///          specified pixel. </returns>
		/// <seealso cref= ColorModel#getRGBdefault </seealso>
		public virtual int GetRGB(int pixel)
		{
			return (GetAlpha(pixel) << 24) | (GetRed(pixel) << 16) | (GetGreen(pixel) << 8) | (GetBlue(pixel) << 0);
		}

		/// <summary>
		/// Returns the red color component for the specified pixel, scaled
		/// from 0 to 255 in the default RGB <code>ColorSpace</code>, sRGB.  A
		/// color conversion is done if necessary.  The pixel value is
		/// specified by an array of data elements of type transferType passed
		/// in as an object reference.  The returned value is a non
		/// pre-multiplied value.  For example, if alpha is premultiplied,
		/// this method divides it out before returning
		/// the value.  If the alpha value is 0, the red value is 0.
		/// If <code>inData</code> is not a primitive array of type
		/// transferType, a <code>ClassCastException</code> is thrown.  An
		/// <code>ArrayIndexOutOfBoundsException</code> is thrown if
		/// <code>inData</code> is not large enough to hold a pixel value for
		/// this <code>ColorModel</code>.
		/// If this <code>transferType</code> is not supported, a
		/// <code>UnsupportedOperationException</code> will be
		/// thrown.  Since
		/// <code>ColorModel</code> is an abstract class, any instance
		/// must be an instance of a subclass.  Subclasses inherit the
		/// implementation of this method and if they don't override it, this
		/// method throws an exception if the subclass uses a
		/// <code>transferType</code> other than
		/// <code>DataBuffer.TYPE_BYTE</code>,
		/// <code>DataBuffer.TYPE_USHORT</code>, or
		/// <code>DataBuffer.TYPE_INT</code>. </summary>
		/// <param name="inData"> an array of pixel values </param>
		/// <returns> the value of the red component of the specified pixel. </returns>
		/// <exception cref="ClassCastException"> if <code>inData</code>
		///  is not a primitive array of type <code>transferType</code> </exception>
		/// <exception cref="ArrayIndexOutOfBoundsException"> if
		///  <code>inData</code> is not large enough to hold a pixel value
		///  for this <code>ColorModel</code> </exception>
		/// <exception cref="UnsupportedOperationException"> if this
		///  <code>tranferType</code> is not supported by this
		///  <code>ColorModel</code> </exception>
		public virtual int GetRed(Object inData)
		{
			int pixel = 0, length = 0;
			switch (TransferType_Renamed)
			{
				case DataBuffer.TYPE_BYTE:
				   sbyte[] bdata = (sbyte[])inData;
				   pixel = bdata[0] & 0xff;
				   length = bdata.Length;
				break;
				case DataBuffer.TYPE_USHORT:
				   short[] sdata = (short[])inData;
				   pixel = sdata[0] & 0xffff;
				   length = sdata.Length;
				break;
				case DataBuffer.TYPE_INT:
				   int[] idata = (int[])inData;
				   pixel = idata[0];
				   length = idata.Length;
				break;
				default:
				   throw new UnsupportedOperationException("This method has not been " + "implemented for transferType " + TransferType_Renamed);
			}
			if (length == 1)
			{
				return GetRed(pixel);
			}
			else
			{
				throw new UnsupportedOperationException("This method is not supported by this color model");
			}
		}

		/// <summary>
		/// Returns the green color component for the specified pixel, scaled
		/// from 0 to 255 in the default RGB <code>ColorSpace</code>, sRGB.  A
		/// color conversion is done if necessary.  The pixel value is
		/// specified by an array of data elements of type transferType passed
		/// in as an object reference.  The returned value will be a non
		/// pre-multiplied value.  For example, if the alpha is premultiplied,
		/// this method divides it out before returning the value.  If the
		/// alpha value is 0, the green value is 0.  If <code>inData</code> is
		/// not a primitive array of type transferType, a
		/// <code>ClassCastException</code> is thrown.  An
		/// <code>ArrayIndexOutOfBoundsException</code> is thrown if
		/// <code>inData</code> is not large enough to hold a pixel value for
		/// this <code>ColorModel</code>.
		/// If this <code>transferType</code> is not supported, a
		/// <code>UnsupportedOperationException</code> will be
		/// thrown.  Since
		/// <code>ColorModel</code> is an abstract class, any instance
		/// must be an instance of a subclass.  Subclasses inherit the
		/// implementation of this method and if they don't override it, this
		/// method throws an exception if the subclass uses a
		/// <code>transferType</code> other than
		/// <code>DataBuffer.TYPE_BYTE</code>,
		/// <code>DataBuffer.TYPE_USHORT</code>, or
		/// <code>DataBuffer.TYPE_INT</code>. </summary>
		/// <param name="inData"> an array of pixel values </param>
		/// <returns> the value of the green component of the specified pixel. </returns>
		/// <exception cref="ClassCastException"> if <code>inData</code>
		///  is not a primitive array of type <code>transferType</code> </exception>
		/// <exception cref="ArrayIndexOutOfBoundsException"> if
		///  <code>inData</code> is not large enough to hold a pixel value
		///  for this <code>ColorModel</code> </exception>
		/// <exception cref="UnsupportedOperationException"> if this
		///  <code>tranferType</code> is not supported by this
		///  <code>ColorModel</code> </exception>
		public virtual int GetGreen(Object inData)
		{
			int pixel = 0, length = 0;
			switch (TransferType_Renamed)
			{
				case DataBuffer.TYPE_BYTE:
				   sbyte[] bdata = (sbyte[])inData;
				   pixel = bdata[0] & 0xff;
				   length = bdata.Length;
				break;
				case DataBuffer.TYPE_USHORT:
				   short[] sdata = (short[])inData;
				   pixel = sdata[0] & 0xffff;
				   length = sdata.Length;
				break;
				case DataBuffer.TYPE_INT:
				   int[] idata = (int[])inData;
				   pixel = idata[0];
				   length = idata.Length;
				break;
				default:
				   throw new UnsupportedOperationException("This method has not been " + "implemented for transferType " + TransferType_Renamed);
			}
			if (length == 1)
			{
				return GetGreen(pixel);
			}
			else
			{
				throw new UnsupportedOperationException("This method is not supported by this color model");
			}
		}

		/// <summary>
		/// Returns the blue color component for the specified pixel, scaled
		/// from 0 to 255 in the default RGB <code>ColorSpace</code>, sRGB.  A
		/// color conversion is done if necessary.  The pixel value is
		/// specified by an array of data elements of type transferType passed
		/// in as an object reference.  The returned value is a non
		/// pre-multiplied value.  For example, if the alpha is premultiplied,
		/// this method divides it out before returning the value.  If the
		/// alpha value is 0, the blue value will be 0.  If
		/// <code>inData</code> is not a primitive array of type transferType,
		/// a <code>ClassCastException</code> is thrown.  An
		/// <code>ArrayIndexOutOfBoundsException</code> is
		/// thrown if <code>inData</code> is not large enough to hold a pixel
		/// value for this <code>ColorModel</code>.
		/// If this <code>transferType</code> is not supported, a
		/// <code>UnsupportedOperationException</code> will be
		/// thrown.  Since
		/// <code>ColorModel</code> is an abstract class, any instance
		/// must be an instance of a subclass.  Subclasses inherit the
		/// implementation of this method and if they don't override it, this
		/// method throws an exception if the subclass uses a
		/// <code>transferType</code> other than
		/// <code>DataBuffer.TYPE_BYTE</code>,
		/// <code>DataBuffer.TYPE_USHORT</code>, or
		/// <code>DataBuffer.TYPE_INT</code>. </summary>
		/// <param name="inData"> an array of pixel values </param>
		/// <returns> the value of the blue component of the specified pixel. </returns>
		/// <exception cref="ClassCastException"> if <code>inData</code>
		///  is not a primitive array of type <code>transferType</code> </exception>
		/// <exception cref="ArrayIndexOutOfBoundsException"> if
		///  <code>inData</code> is not large enough to hold a pixel value
		///  for this <code>ColorModel</code> </exception>
		/// <exception cref="UnsupportedOperationException"> if this
		///  <code>tranferType</code> is not supported by this
		///  <code>ColorModel</code> </exception>
		public virtual int GetBlue(Object inData)
		{
			int pixel = 0, length = 0;
			switch (TransferType_Renamed)
			{
				case DataBuffer.TYPE_BYTE:
				   sbyte[] bdata = (sbyte[])inData;
				   pixel = bdata[0] & 0xff;
				   length = bdata.Length;
				break;
				case DataBuffer.TYPE_USHORT:
				   short[] sdata = (short[])inData;
				   pixel = sdata[0] & 0xffff;
				   length = sdata.Length;
				break;
				case DataBuffer.TYPE_INT:
				   int[] idata = (int[])inData;
				   pixel = idata[0];
				   length = idata.Length;
				break;
				default:
				   throw new UnsupportedOperationException("This method has not been " + "implemented for transferType " + TransferType_Renamed);
			}
			if (length == 1)
			{
				return GetBlue(pixel);
			}
			else
			{
				throw new UnsupportedOperationException("This method is not supported by this color model");
			}
		}

		/// <summary>
		/// Returns the alpha component for the specified pixel, scaled
		/// from 0 to 255.  The pixel value is specified by an array of data
		/// elements of type transferType passed in as an object reference.
		/// If inData is not a primitive array of type transferType, a
		/// <code>ClassCastException</code> is thrown.  An
		/// <code>ArrayIndexOutOfBoundsException</code> is thrown if
		/// <code>inData</code> is not large enough to hold a pixel value for
		/// this <code>ColorModel</code>.
		/// If this <code>transferType</code> is not supported, a
		/// <code>UnsupportedOperationException</code> will be
		/// thrown.  Since
		/// <code>ColorModel</code> is an abstract class, any instance
		/// must be an instance of a subclass.  Subclasses inherit the
		/// implementation of this method and if they don't override it, this
		/// method throws an exception if the subclass uses a
		/// <code>transferType</code> other than
		/// <code>DataBuffer.TYPE_BYTE</code>,
		/// <code>DataBuffer.TYPE_USHORT</code>, or
		/// <code>DataBuffer.TYPE_INT</code>. </summary>
		/// <param name="inData"> the specified pixel </param>
		/// <returns> the alpha component of the specified pixel, scaled from
		/// 0 to 255. </returns>
		/// <exception cref="ClassCastException"> if <code>inData</code>
		///  is not a primitive array of type <code>transferType</code> </exception>
		/// <exception cref="ArrayIndexOutOfBoundsException"> if
		///  <code>inData</code> is not large enough to hold a pixel value
		///  for this <code>ColorModel</code> </exception>
		/// <exception cref="UnsupportedOperationException"> if this
		///  <code>tranferType</code> is not supported by this
		///  <code>ColorModel</code> </exception>
		public virtual int GetAlpha(Object inData)
		{
			int pixel = 0, length = 0;
			switch (TransferType_Renamed)
			{
				case DataBuffer.TYPE_BYTE:
				   sbyte[] bdata = (sbyte[])inData;
				   pixel = bdata[0] & 0xff;
				   length = bdata.Length;
				break;
				case DataBuffer.TYPE_USHORT:
				   short[] sdata = (short[])inData;
				   pixel = sdata[0] & 0xffff;
				   length = sdata.Length;
				break;
				case DataBuffer.TYPE_INT:
				   int[] idata = (int[])inData;
				   pixel = idata[0];
				   length = idata.Length;
				break;
				default:
				   throw new UnsupportedOperationException("This method has not been " + "implemented for transferType " + TransferType_Renamed);
			}
			if (length == 1)
			{
				return GetAlpha(pixel);
			}
			else
			{
				throw new UnsupportedOperationException("This method is not supported by this color model");
			}
		}

		/// <summary>
		/// Returns the color/alpha components for the specified pixel in the
		/// default RGB color model format.  A color conversion is done if
		/// necessary.  The pixel value is specified by an array of data
		/// elements of type transferType passed in as an object reference.
		/// If inData is not a primitive array of type transferType, a
		/// <code>ClassCastException</code> is thrown.  An
		/// <code>ArrayIndexOutOfBoundsException</code> is
		/// thrown if <code>inData</code> is not large enough to hold a pixel
		/// value for this <code>ColorModel</code>.
		/// The returned value will be in a non pre-multiplied format, i.e. if
		/// the alpha is premultiplied, this method will divide it out of the
		/// color components (if the alpha value is 0, the color values will be 0). </summary>
		/// <param name="inData"> the specified pixel </param>
		/// <returns> the color and alpha components of the specified pixel. </returns>
		/// <seealso cref= ColorModel#getRGBdefault </seealso>
		public virtual int GetRGB(Object inData)
		{
			return (GetAlpha(inData) << 24) | (GetRed(inData) << 16) | (GetGreen(inData) << 8) | (GetBlue(inData) << 0);
		}

		/// <summary>
		/// Returns a data element array representation of a pixel in this
		/// <code>ColorModel</code>, given an integer pixel representation in
		/// the default RGB color model.
		/// This array can then be passed to the
		/// <seealso cref="WritableRaster#setDataElements"/> method of
		/// a <seealso cref="WritableRaster"/> object.  If the pixel variable is
		/// <code>null</code>, a new array will be allocated.  If
		/// <code>pixel</code> is not
		/// <code>null</code>, it must be a primitive array of type
		/// <code>transferType</code>; otherwise, a
		/// <code>ClassCastException</code> is thrown.  An
		/// <code>ArrayIndexOutOfBoundsException</code> is thrown if
		/// <code>pixel</code> is
		/// not large enough to hold a pixel value for this
		/// <code>ColorModel</code>. The pixel array is returned.
		/// If this <code>transferType</code> is not supported, a
		/// <code>UnsupportedOperationException</code> will be
		/// thrown.  Since <code>ColorModel</code> is an abstract class,
		/// any instance is an instance of a subclass.  Subclasses must
		/// override this method since the implementation in this abstract
		/// class throws an <code>UnsupportedOperationException</code>. </summary>
		/// <param name="rgb"> the integer pixel representation in the default RGB
		/// color model </param>
		/// <param name="pixel"> the specified pixel </param>
		/// <returns> an array representation of the specified pixel in this
		///  <code>ColorModel</code>. </returns>
		/// <exception cref="ClassCastException"> if <code>pixel</code>
		///  is not a primitive array of type <code>transferType</code> </exception>
		/// <exception cref="ArrayIndexOutOfBoundsException"> if
		///  <code>pixel</code> is not large enough to hold a pixel value
		///  for this <code>ColorModel</code> </exception>
		/// <exception cref="UnsupportedOperationException"> if this
		///  method is not supported by this <code>ColorModel</code> </exception>
		/// <seealso cref= WritableRaster#setDataElements </seealso>
		/// <seealso cref= SampleModel#setDataElements </seealso>
		public virtual Object GetDataElements(int rgb, Object pixel)
		{
			throw new UnsupportedOperationException("This method is not supported by this color model.");
		}

		/// <summary>
		/// Returns an array of unnormalized color/alpha components given a pixel
		/// in this <code>ColorModel</code>.  The pixel value is specified as
		/// an <code>int</code>.  An <code>IllegalArgumentException</code>
		/// will be thrown if pixel values for this <code>ColorModel</code> are
		/// not conveniently representable as a single <code>int</code> or if
		/// color component values for this <code>ColorModel</code> are not
		/// conveniently representable in the unnormalized form.
		/// For example, this method can be used to retrieve the
		/// components for a specific pixel value in a
		/// <code>DirectColorModel</code>.  If the components array is
		/// <code>null</code>, a new array will be allocated.  The
		/// components array will be returned.  Color/alpha components are
		/// stored in the components array starting at <code>offset</code>
		/// (even if the array is allocated by this method).  An
		/// <code>ArrayIndexOutOfBoundsException</code> is thrown if  the
		/// components array is not <code>null</code> and is not large
		/// enough to hold all the color and alpha components (starting at offset).
		/// Since <code>ColorModel</code> is an abstract class,
		/// any instance is an instance of a subclass.  Subclasses must
		/// override this method since the implementation in this abstract
		/// class throws an <code>UnsupportedOperationException</code>. </summary>
		/// <param name="pixel"> the specified pixel </param>
		/// <param name="components"> the array to receive the color and alpha
		/// components of the specified pixel </param>
		/// <param name="offset"> the offset into the <code>components</code> array at
		/// which to start storing the color and alpha components </param>
		/// <returns> an array containing the color and alpha components of the
		/// specified pixel starting at the specified offset. </returns>
		/// <exception cref="UnsupportedOperationException"> if this
		///          method is not supported by this <code>ColorModel</code> </exception>
		public virtual int[] GetComponents(int pixel, int[] components, int offset)
		{
			throw new UnsupportedOperationException("This method is not supported by this color model.");
		}

		/// <summary>
		/// Returns an array of unnormalized color/alpha components given a pixel
		/// in this <code>ColorModel</code>.  The pixel value is specified by
		/// an array of data elements of type transferType passed in as an
		/// object reference.  If <code>pixel</code> is not a primitive array
		/// of type transferType, a <code>ClassCastException</code> is thrown.
		/// An <code>IllegalArgumentException</code> will be thrown if color
		/// component values for this <code>ColorModel</code> are not
		/// conveniently representable in the unnormalized form.
		/// An <code>ArrayIndexOutOfBoundsException</code> is
		/// thrown if <code>pixel</code> is not large enough to hold a pixel
		/// value for this <code>ColorModel</code>.
		/// This method can be used to retrieve the components for a specific
		/// pixel value in any <code>ColorModel</code>.  If the components
		/// array is <code>null</code>, a new array will be allocated.  The
		/// components array will be returned.  Color/alpha components are
		/// stored in the <code>components</code> array starting at
		/// <code>offset</code> (even if the array is allocated by this
		/// method).  An <code>ArrayIndexOutOfBoundsException</code>
		/// is thrown if  the components array is not <code>null</code> and is
		/// not large enough to hold all the color and alpha components
		/// (starting at <code>offset</code>).
		/// Since <code>ColorModel</code> is an abstract class,
		/// any instance is an instance of a subclass.  Subclasses must
		/// override this method since the implementation in this abstract
		/// class throws an <code>UnsupportedOperationException</code>. </summary>
		/// <param name="pixel"> the specified pixel </param>
		/// <param name="components"> an array that receives the color and alpha
		/// components of the specified pixel </param>
		/// <param name="offset"> the index into the <code>components</code> array at
		/// which to begin storing the color and alpha components of the
		/// specified pixel </param>
		/// <returns> an array containing the color and alpha components of the
		/// specified pixel starting at the specified offset. </returns>
		/// <exception cref="UnsupportedOperationException"> if this
		///          method is not supported by this <code>ColorModel</code> </exception>
		public virtual int[] GetComponents(Object pixel, int[] components, int offset)
		{
			throw new UnsupportedOperationException("This method is not supported by this color model.");
		}

		/// <summary>
		/// Returns an array of all of the color/alpha components in unnormalized
		/// form, given a normalized component array.  Unnormalized components
		/// are unsigned integral values between 0 and 2<sup>n</sup> - 1, where
		/// n is the number of bits for a particular component.  Normalized
		/// components are float values between a per component minimum and
		/// maximum specified by the <code>ColorSpace</code> object for this
		/// <code>ColorModel</code>.  An <code>IllegalArgumentException</code>
		/// will be thrown if color component values for this
		/// <code>ColorModel</code> are not conveniently representable in the
		/// unnormalized form.  If the
		/// <code>components</code> array is <code>null</code>, a new array
		/// will be allocated.  The <code>components</code> array will
		/// be returned.  Color/alpha components are stored in the
		/// <code>components</code> array starting at <code>offset</code> (even
		/// if the array is allocated by this method). An
		/// <code>ArrayIndexOutOfBoundsException</code> is thrown if the
		/// <code>components</code> array is not <code>null</code> and is not
		/// large enough to hold all the color and alpha
		/// components (starting at <code>offset</code>).  An
		/// <code>IllegalArgumentException</code> is thrown if the
		/// <code>normComponents</code> array is not large enough to hold
		/// all the color and alpha components starting at
		/// <code>normOffset</code>. </summary>
		/// <param name="normComponents"> an array containing normalized components </param>
		/// <param name="normOffset"> the offset into the <code>normComponents</code>
		/// array at which to start retrieving normalized components </param>
		/// <param name="components"> an array that receives the components from
		/// <code>normComponents</code> </param>
		/// <param name="offset"> the index into <code>components</code> at which to
		/// begin storing normalized components from
		/// <code>normComponents</code> </param>
		/// <returns> an array containing unnormalized color and alpha
		/// components. </returns>
		/// <exception cref="IllegalArgumentException"> If the component values for this
		/// <CODE>ColorModel</CODE> are not conveniently representable in the
		/// unnormalized form. </exception>
		/// <exception cref="IllegalArgumentException"> if the length of
		///          <code>normComponents</code> minus <code>normOffset</code>
		///          is less than <code>numComponents</code> </exception>
		/// <exception cref="UnsupportedOperationException"> if the
		///          constructor of this <code>ColorModel</code> called the
		///          <code>super(bits)</code> constructor, but did not
		///          override this method.  See the constructor,
		///          <seealso cref="#ColorModel(int)"/>. </exception>
		public virtual int[] GetUnnormalizedComponents(float[] normComponents, int normOffset, int[] components, int offset)
		{
			// Make sure that someone isn't using a custom color model
			// that called the super(bits) constructor.
			if (ColorSpace_Renamed == null)
			{
				throw new UnsupportedOperationException("This method is not supported " + "by this color model.");
			}

			if (NBits == null)
			{
				throw new UnsupportedOperationException("This method is not supported.  " + "Unable to determine #bits per " + "component.");
			}
			if ((normComponents.Length - normOffset) < NumComponents_Renamed)
			{
				throw new IllegalArgumentException("Incorrect number of components.  Expecting " + NumComponents_Renamed);
			}

			if (components == null)
			{
				components = new int[offset + NumComponents_Renamed];
			}

			if (SupportsAlpha && IsAlphaPremultiplied)
			{
				float normAlpha = normComponents[normOffset + NumColorComponents_Renamed];
				for (int i = 0; i < NumColorComponents_Renamed; i++)
				{
					components[offset + i] = (int)(normComponents[normOffset + i] * ((1 << NBits[i]) - 1) * normAlpha + 0.5f);
				}
				components[offset + NumColorComponents_Renamed] = (int)(normAlpha * ((1 << NBits[NumColorComponents_Renamed]) - 1) + 0.5f);
			}
			else
			{
				for (int i = 0; i < NumComponents_Renamed; i++)
				{
					components[offset + i] = (int)(normComponents[normOffset + i] * ((1 << NBits[i]) - 1) + 0.5f);
				}
			}

			return components;
		}

		/// <summary>
		/// Returns an array of all of the color/alpha components in normalized
		/// form, given an unnormalized component array.  Unnormalized components
		/// are unsigned integral values between 0 and 2<sup>n</sup> - 1, where
		/// n is the number of bits for a particular component.  Normalized
		/// components are float values between a per component minimum and
		/// maximum specified by the <code>ColorSpace</code> object for this
		/// <code>ColorModel</code>.  An <code>IllegalArgumentException</code>
		/// will be thrown if color component values for this
		/// <code>ColorModel</code> are not conveniently representable in the
		/// unnormalized form.  If the
		/// <code>normComponents</code> array is <code>null</code>, a new array
		/// will be allocated.  The <code>normComponents</code> array
		/// will be returned.  Color/alpha components are stored in the
		/// <code>normComponents</code> array starting at
		/// <code>normOffset</code> (even if the array is allocated by this
		/// method).  An <code>ArrayIndexOutOfBoundsException</code> is thrown
		/// if the <code>normComponents</code> array is not <code>null</code>
		/// and is not large enough to hold all the color and alpha components
		/// (starting at <code>normOffset</code>).  An
		/// <code>IllegalArgumentException</code> is thrown if the
		/// <code>components</code> array is not large enough to hold all the
		/// color and alpha components starting at <code>offset</code>.
		/// <para>
		/// Since <code>ColorModel</code> is an abstract class,
		/// any instance is an instance of a subclass.  The default implementation
		/// of this method in this abstract class assumes that component values
		/// for this class are conveniently representable in the unnormalized
		/// form.  Therefore, subclasses which may
		/// have instances which do not support the unnormalized form must
		/// override this method.
		/// </para>
		/// </summary>
		/// <param name="components"> an array containing unnormalized components </param>
		/// <param name="offset"> the offset into the <code>components</code> array at
		/// which to start retrieving unnormalized components </param>
		/// <param name="normComponents"> an array that receives the normalized components </param>
		/// <param name="normOffset"> the index into <code>normComponents</code> at
		/// which to begin storing normalized components </param>
		/// <returns> an array containing normalized color and alpha
		/// components. </returns>
		/// <exception cref="IllegalArgumentException"> If the component values for this
		/// <CODE>ColorModel</CODE> are not conveniently representable in the
		/// unnormalized form. </exception>
		/// <exception cref="UnsupportedOperationException"> if the
		///          constructor of this <code>ColorModel</code> called the
		///          <code>super(bits)</code> constructor, but did not
		///          override this method.  See the constructor,
		///          <seealso cref="#ColorModel(int)"/>. </exception>
		/// <exception cref="UnsupportedOperationException"> if this method is unable
		///          to determine the number of bits per component </exception>
		public virtual float[] GetNormalizedComponents(int[] components, int offset, float[] normComponents, int normOffset)
		{
			// Make sure that someone isn't using a custom color model
			// that called the super(bits) constructor.
			if (ColorSpace_Renamed == null)
			{
				throw new UnsupportedOperationException("This method is not supported by " + "this color model.");
			}
			if (NBits == null)
			{
				throw new UnsupportedOperationException("This method is not supported.  " + "Unable to determine #bits per " + "component.");
			}

			if ((components.Length - offset) < NumComponents_Renamed)
			{
				throw new IllegalArgumentException("Incorrect number of components.  Expecting " + NumComponents_Renamed);
			}

			if (normComponents == null)
			{
				normComponents = new float[NumComponents_Renamed + normOffset];
			}

			if (SupportsAlpha && IsAlphaPremultiplied)
			{
				// Normalized coordinates are non premultiplied
				float normAlpha = (float)components[offset + NumColorComponents_Renamed];
				normAlpha /= (float)((1 << NBits[NumColorComponents_Renamed]) - 1);
				if (normAlpha != 0.0f)
				{
					for (int i = 0; i < NumColorComponents_Renamed; i++)
					{
						normComponents[normOffset + i] = ((float) components[offset + i]) / (normAlpha * ((float)((1 << NBits[i]) - 1)));
					}
				}
				else
				{
					for (int i = 0; i < NumColorComponents_Renamed; i++)
					{
						normComponents[normOffset + i] = 0.0f;
					}
				}
				normComponents[normOffset + NumColorComponents_Renamed] = normAlpha;
			}
			else
			{
				for (int i = 0; i < NumComponents_Renamed; i++)
				{
					normComponents[normOffset + i] = ((float) components[offset + i]) / ((float)((1 << NBits[i]) - 1));
				}
			}

			return normComponents;
		}

		/// <summary>
		/// Returns a pixel value represented as an <code>int</code> in this
		/// <code>ColorModel</code>, given an array of unnormalized color/alpha
		/// components.  This method will throw an
		/// <code>IllegalArgumentException</code> if component values for this
		/// <code>ColorModel</code> are not conveniently representable as a
		/// single <code>int</code> or if color component values for this
		/// <code>ColorModel</code> are not conveniently representable in the
		/// unnormalized form.  An
		/// <code>ArrayIndexOutOfBoundsException</code> is thrown if  the
		/// <code>components</code> array is not large enough to hold all the
		/// color and alpha components (starting at <code>offset</code>).
		/// Since <code>ColorModel</code> is an abstract class,
		/// any instance is an instance of a subclass.  Subclasses must
		/// override this method since the implementation in this abstract
		/// class throws an <code>UnsupportedOperationException</code>. </summary>
		/// <param name="components"> an array of unnormalized color and alpha
		/// components </param>
		/// <param name="offset"> the index into <code>components</code> at which to
		/// begin retrieving the color and alpha components </param>
		/// <returns> an <code>int</code> pixel value in this
		/// <code>ColorModel</code> corresponding to the specified components. </returns>
		/// <exception cref="IllegalArgumentException"> if
		///  pixel values for this <code>ColorModel</code> are not
		///  conveniently representable as a single <code>int</code> </exception>
		/// <exception cref="IllegalArgumentException"> if
		///  component values for this <code>ColorModel</code> are not
		///  conveniently representable in the unnormalized form </exception>
		/// <exception cref="ArrayIndexOutOfBoundsException"> if
		///  the <code>components</code> array is not large enough to
		///  hold all of the color and alpha components starting at
		///  <code>offset</code> </exception>
		/// <exception cref="UnsupportedOperationException"> if this
		///  method is not supported by this <code>ColorModel</code> </exception>
		public virtual int GetDataElement(int[] components, int offset)
		{
			throw new UnsupportedOperationException("This method is not supported " + "by this color model.");
		}

		/// <summary>
		/// Returns a data element array representation of a pixel in this
		/// <code>ColorModel</code>, given an array of unnormalized color/alpha
		/// components.  This array can then be passed to the
		/// <code>setDataElements</code> method of a <code>WritableRaster</code>
		/// object.  This method will throw an <code>IllegalArgumentException</code>
		/// if color component values for this <code>ColorModel</code> are not
		/// conveniently representable in the unnormalized form.
		/// An <code>ArrayIndexOutOfBoundsException</code> is thrown
		/// if the <code>components</code> array is not large enough to hold
		/// all the color and alpha components (starting at
		/// <code>offset</code>).  If the <code>obj</code> variable is
		/// <code>null</code>, a new array will be allocated.  If
		/// <code>obj</code> is not <code>null</code>, it must be a primitive
		/// array of type transferType; otherwise, a
		/// <code>ClassCastException</code> is thrown.  An
		/// <code>ArrayIndexOutOfBoundsException</code> is thrown if
		/// <code>obj</code> is not large enough to hold a pixel value for this
		/// <code>ColorModel</code>.
		/// Since <code>ColorModel</code> is an abstract class,
		/// any instance is an instance of a subclass.  Subclasses must
		/// override this method since the implementation in this abstract
		/// class throws an <code>UnsupportedOperationException</code>. </summary>
		/// <param name="components"> an array of unnormalized color and alpha
		/// components </param>
		/// <param name="offset"> the index into <code>components</code> at which to
		/// begin retrieving color and alpha components </param>
		/// <param name="obj"> the <code>Object</code> representing an array of color
		/// and alpha components </param>
		/// <returns> an <code>Object</code> representing an array of color and
		/// alpha components. </returns>
		/// <exception cref="ClassCastException"> if <code>obj</code>
		///  is not a primitive array of type <code>transferType</code> </exception>
		/// <exception cref="ArrayIndexOutOfBoundsException"> if
		///  <code>obj</code> is not large enough to hold a pixel value
		///  for this <code>ColorModel</code> or the <code>components</code>
		///  array is not large enough to hold all of the color and alpha
		///  components starting at <code>offset</code> </exception>
		/// <exception cref="IllegalArgumentException"> if
		///  component values for this <code>ColorModel</code> are not
		///  conveniently representable in the unnormalized form </exception>
		/// <exception cref="UnsupportedOperationException"> if this
		///  method is not supported by this <code>ColorModel</code> </exception>
		/// <seealso cref= WritableRaster#setDataElements </seealso>
		/// <seealso cref= SampleModel#setDataElements </seealso>
		public virtual Object GetDataElements(int[] components, int offset, Object obj)
		{
			throw new UnsupportedOperationException("This method has not been implemented " + "for this color model.");
		}

		/// <summary>
		/// Returns a pixel value represented as an <code>int</code> in this
		/// <code>ColorModel</code>, given an array of normalized color/alpha
		/// components.  This method will throw an
		/// <code>IllegalArgumentException</code> if pixel values for this
		/// <code>ColorModel</code> are not conveniently representable as a
		/// single <code>int</code>.  An
		/// <code>ArrayIndexOutOfBoundsException</code> is thrown if  the
		/// <code>normComponents</code> array is not large enough to hold all the
		/// color and alpha components (starting at <code>normOffset</code>).
		/// Since <code>ColorModel</code> is an abstract class,
		/// any instance is an instance of a subclass.  The default implementation
		/// of this method in this abstract class first converts from the
		/// normalized form to the unnormalized form and then calls
		/// <code>getDataElement(int[], int)</code>.  Subclasses which may
		/// have instances which do not support the unnormalized form must
		/// override this method. </summary>
		/// <param name="normComponents"> an array of normalized color and alpha
		/// components </param>
		/// <param name="normOffset"> the index into <code>normComponents</code> at which to
		/// begin retrieving the color and alpha components </param>
		/// <returns> an <code>int</code> pixel value in this
		/// <code>ColorModel</code> corresponding to the specified components. </returns>
		/// <exception cref="IllegalArgumentException"> if
		///  pixel values for this <code>ColorModel</code> are not
		///  conveniently representable as a single <code>int</code> </exception>
		/// <exception cref="ArrayIndexOutOfBoundsException"> if
		///  the <code>normComponents</code> array is not large enough to
		///  hold all of the color and alpha components starting at
		///  <code>normOffset</code>
		/// @since 1.4 </exception>
		public virtual int GetDataElement(float[] normComponents, int normOffset)
		{
			int[] components = GetUnnormalizedComponents(normComponents, normOffset, null, 0);
			return GetDataElement(components, 0);
		}

		/// <summary>
		/// Returns a data element array representation of a pixel in this
		/// <code>ColorModel</code>, given an array of normalized color/alpha
		/// components.  This array can then be passed to the
		/// <code>setDataElements</code> method of a <code>WritableRaster</code>
		/// object.  An <code>ArrayIndexOutOfBoundsException</code> is thrown
		/// if the <code>normComponents</code> array is not large enough to hold
		/// all the color and alpha components (starting at
		/// <code>normOffset</code>).  If the <code>obj</code> variable is
		/// <code>null</code>, a new array will be allocated.  If
		/// <code>obj</code> is not <code>null</code>, it must be a primitive
		/// array of type transferType; otherwise, a
		/// <code>ClassCastException</code> is thrown.  An
		/// <code>ArrayIndexOutOfBoundsException</code> is thrown if
		/// <code>obj</code> is not large enough to hold a pixel value for this
		/// <code>ColorModel</code>.
		/// Since <code>ColorModel</code> is an abstract class,
		/// any instance is an instance of a subclass.  The default implementation
		/// of this method in this abstract class first converts from the
		/// normalized form to the unnormalized form and then calls
		/// <code>getDataElement(int[], int, Object)</code>.  Subclasses which may
		/// have instances which do not support the unnormalized form must
		/// override this method. </summary>
		/// <param name="normComponents"> an array of normalized color and alpha
		/// components </param>
		/// <param name="normOffset"> the index into <code>normComponents</code> at which to
		/// begin retrieving color and alpha components </param>
		/// <param name="obj"> a primitive data array to hold the returned pixel </param>
		/// <returns> an <code>Object</code> which is a primitive data array
		/// representation of a pixel </returns>
		/// <exception cref="ClassCastException"> if <code>obj</code>
		///  is not a primitive array of type <code>transferType</code> </exception>
		/// <exception cref="ArrayIndexOutOfBoundsException"> if
		///  <code>obj</code> is not large enough to hold a pixel value
		///  for this <code>ColorModel</code> or the <code>normComponents</code>
		///  array is not large enough to hold all of the color and alpha
		///  components starting at <code>normOffset</code> </exception>
		/// <seealso cref= WritableRaster#setDataElements </seealso>
		/// <seealso cref= SampleModel#setDataElements
		/// @since 1.4 </seealso>
		public virtual Object GetDataElements(float[] normComponents, int normOffset, Object obj)
		{
			int[] components = GetUnnormalizedComponents(normComponents, normOffset, null, 0);
			return GetDataElements(components, 0, obj);
		}

		/// <summary>
		/// Returns an array of all of the color/alpha components in normalized
		/// form, given a pixel in this <code>ColorModel</code>.  The pixel
		/// value is specified by an array of data elements of type transferType
		/// passed in as an object reference.  If pixel is not a primitive array
		/// of type transferType, a <code>ClassCastException</code> is thrown.
		/// An <code>ArrayIndexOutOfBoundsException</code> is thrown if
		/// <code>pixel</code> is not large enough to hold a pixel value for this
		/// <code>ColorModel</code>.
		/// Normalized components are float values between a per component minimum
		/// and maximum specified by the <code>ColorSpace</code> object for this
		/// <code>ColorModel</code>.  If the
		/// <code>normComponents</code> array is <code>null</code>, a new array
		/// will be allocated.  The <code>normComponents</code> array
		/// will be returned.  Color/alpha components are stored in the
		/// <code>normComponents</code> array starting at
		/// <code>normOffset</code> (even if the array is allocated by this
		/// method).  An <code>ArrayIndexOutOfBoundsException</code> is thrown
		/// if the <code>normComponents</code> array is not <code>null</code>
		/// and is not large enough to hold all the color and alpha components
		/// (starting at <code>normOffset</code>).
		/// Since <code>ColorModel</code> is an abstract class,
		/// any instance is an instance of a subclass.  The default implementation
		/// of this method in this abstract class first retrieves color and alpha
		/// components in the unnormalized form using
		/// <code>getComponents(Object, int[], int)</code> and then calls
		/// <code>getNormalizedComponents(int[], int, float[], int)</code>.
		/// Subclasses which may
		/// have instances which do not support the unnormalized form must
		/// override this method. </summary>
		/// <param name="pixel"> the specified pixel </param>
		/// <param name="normComponents"> an array to receive the normalized components </param>
		/// <param name="normOffset"> the offset into the <code>normComponents</code>
		/// array at which to start storing normalized components </param>
		/// <returns> an array containing normalized color and alpha
		/// components. </returns>
		/// <exception cref="ClassCastException"> if <code>pixel</code> is not a primitive
		///          array of type transferType </exception>
		/// <exception cref="ArrayIndexOutOfBoundsException"> if
		///          <code>normComponents</code> is not large enough to hold all
		///          color and alpha components starting at <code>normOffset</code> </exception>
		/// <exception cref="ArrayIndexOutOfBoundsException"> if
		///          <code>pixel</code> is not large enough to hold a pixel
		///          value for this <code>ColorModel</code>. </exception>
		/// <exception cref="UnsupportedOperationException"> if the
		///          constructor of this <code>ColorModel</code> called the
		///          <code>super(bits)</code> constructor, but did not
		///          override this method.  See the constructor,
		///          <seealso cref="#ColorModel(int)"/>. </exception>
		/// <exception cref="UnsupportedOperationException"> if this method is unable
		///          to determine the number of bits per component
		/// @since 1.4 </exception>
		public virtual float[] GetNormalizedComponents(Object pixel, float[] normComponents, int normOffset)
		{
			int[] components = GetComponents(pixel, null, 0);
			return GetNormalizedComponents(components, 0, normComponents, normOffset);
		}

		/// <summary>
		/// Tests if the specified <code>Object</code> is an instance of
		/// <code>ColorModel</code> and if it equals this
		/// <code>ColorModel</code>. </summary>
		/// <param name="obj"> the <code>Object</code> to test for equality </param>
		/// <returns> <code>true</code> if the specified <code>Object</code>
		/// is an instance of <code>ColorModel</code> and equals this
		/// <code>ColorModel</code>; <code>false</code> otherwise. </returns>
		public override bool Equals(Object obj)
		{
			if (!(obj is ColorModel))
			{
				return false;
			}
			ColorModel cm = (ColorModel) obj;

			if (this == cm)
			{
				return true;
			}
			if (SupportsAlpha != cm.HasAlpha() || IsAlphaPremultiplied != cm.AlphaPremultiplied || Pixel_bits != cm.PixelSize || Transparency_Renamed != cm.Transparency || NumComponents_Renamed != cm.NumComponents)
			{
				return false;
			}

			int[] nb = cm.ComponentSize;

			if ((NBits != null) && (nb != null))
			{
				for (int i = 0; i < NumComponents_Renamed; i++)
				{
					if (NBits[i] != nb[i])
					{
						return false;
					}
				}
			}
			else
			{
				return ((NBits == null) && (nb == null));
			}

			return true;
		}

		/// <summary>
		/// Returns the hash code for this ColorModel.
		/// </summary>
		/// <returns>    a hash code for this ColorModel. </returns>
		public override int HashCode()
		{

			int result = 0;

			result = (SupportsAlpha ? 2 : 3) + (IsAlphaPremultiplied ? 4 : 5) + Pixel_bits * 6 + Transparency_Renamed * 7 + NumComponents_Renamed * 8;

			if (NBits != null)
			{
				for (int i = 0; i < NumComponents_Renamed; i++)
				{
					result = result + NBits[i] * (i + 9);
				}
			}

			return result;
		}

		/// <summary>
		/// Returns the <code>ColorSpace</code> associated with this
		/// <code>ColorModel</code>. </summary>
		/// <returns> the <code>ColorSpace</code> of this
		/// <code>ColorModel</code>. </returns>
		public ColorSpace ColorSpace
		{
			get
			{
				return ColorSpace_Renamed;
			}
		}

		/// <summary>
		/// Forces the raster data to match the state specified in the
		/// <code>isAlphaPremultiplied</code> variable, assuming the data is
		/// currently correctly described by this <code>ColorModel</code>.  It
		/// may multiply or divide the color raster data by alpha, or do
		/// nothing if the data is in the correct state.  If the data needs to
		/// be coerced, this method will also return an instance of this
		/// <code>ColorModel</code> with the <code>isAlphaPremultiplied</code>
		/// flag set appropriately.  This method will throw a
		/// <code>UnsupportedOperationException</code> if it is not supported
		/// by this <code>ColorModel</code>.
		/// Since <code>ColorModel</code> is an abstract class,
		/// any instance is an instance of a subclass.  Subclasses must
		/// override this method since the implementation in this abstract
		/// class throws an <code>UnsupportedOperationException</code>. </summary>
		/// <param name="raster"> the <code>WritableRaster</code> data </param>
		/// <param name="isAlphaPremultiplied"> <code>true</code> if the alpha is
		/// premultiplied; <code>false</code> otherwise </param>
		/// <returns> a <code>ColorModel</code> object that represents the
		/// coerced data. </returns>
		public virtual ColorModel CoerceData(WritableRaster raster, bool isAlphaPremultiplied)
		{
			throw new UnsupportedOperationException("This method is not supported by this color model");
		}

		/// <summary>
		/// Returns <code>true</code> if <code>raster</code> is compatible
		/// with this <code>ColorModel</code> and <code>false</code> if it is
		/// not.
		/// Since <code>ColorModel</code> is an abstract class,
		/// any instance is an instance of a subclass.  Subclasses must
		/// override this method since the implementation in this abstract
		/// class throws an <code>UnsupportedOperationException</code>. </summary>
		/// <param name="raster"> the <seealso cref="Raster"/> object to test for compatibility </param>
		/// <returns> <code>true</code> if <code>raster</code> is compatible
		/// with this <code>ColorModel</code>. </returns>
		/// <exception cref="UnsupportedOperationException"> if this
		///         method has not been implemented for this
		///         <code>ColorModel</code> </exception>
		public virtual bool IsCompatibleRaster(Raster raster)
		{
			throw new UnsupportedOperationException("This method has not been implemented for this ColorModel.");
		}

		/// <summary>
		/// Creates a <code>WritableRaster</code> with the specified width and
		/// height that has a data layout (<code>SampleModel</code>) compatible
		/// with this <code>ColorModel</code>.
		/// Since <code>ColorModel</code> is an abstract class,
		/// any instance is an instance of a subclass.  Subclasses must
		/// override this method since the implementation in this abstract
		/// class throws an <code>UnsupportedOperationException</code>. </summary>
		/// <param name="w"> the width to apply to the new <code>WritableRaster</code> </param>
		/// <param name="h"> the height to apply to the new <code>WritableRaster</code> </param>
		/// <returns> a <code>WritableRaster</code> object with the specified
		/// width and height. </returns>
		/// <exception cref="UnsupportedOperationException"> if this
		///          method is not supported by this <code>ColorModel</code> </exception>
		/// <seealso cref= WritableRaster </seealso>
		/// <seealso cref= SampleModel </seealso>
		public virtual WritableRaster CreateCompatibleWritableRaster(int w, int h)
		{
			throw new UnsupportedOperationException("This method is not supported by this color model");
		}

		/// <summary>
		/// Creates a <code>SampleModel</code> with the specified width and
		/// height that has a data layout compatible with this
		/// <code>ColorModel</code>.
		/// Since <code>ColorModel</code> is an abstract class,
		/// any instance is an instance of a subclass.  Subclasses must
		/// override this method since the implementation in this abstract
		/// class throws an <code>UnsupportedOperationException</code>. </summary>
		/// <param name="w"> the width to apply to the new <code>SampleModel</code> </param>
		/// <param name="h"> the height to apply to the new <code>SampleModel</code> </param>
		/// <returns> a <code>SampleModel</code> object with the specified
		/// width and height. </returns>
		/// <exception cref="UnsupportedOperationException"> if this
		///          method is not supported by this <code>ColorModel</code> </exception>
		/// <seealso cref= SampleModel </seealso>
		public virtual SampleModel CreateCompatibleSampleModel(int w, int h)
		{
			throw new UnsupportedOperationException("This method is not supported by this color model");
		}

		/// <summary>
		/// Checks if the <code>SampleModel</code> is compatible with this
		/// <code>ColorModel</code>.
		/// Since <code>ColorModel</code> is an abstract class,
		/// any instance is an instance of a subclass.  Subclasses must
		/// override this method since the implementation in this abstract
		/// class throws an <code>UnsupportedOperationException</code>. </summary>
		/// <param name="sm"> the specified <code>SampleModel</code> </param>
		/// <returns> <code>true</code> if the specified <code>SampleModel</code>
		/// is compatible with this <code>ColorModel</code>; <code>false</code>
		/// otherwise. </returns>
		/// <exception cref="UnsupportedOperationException"> if this
		///          method is not supported by this <code>ColorModel</code> </exception>
		/// <seealso cref= SampleModel </seealso>
		public virtual bool IsCompatibleSampleModel(SampleModel sm)
		{
			throw new UnsupportedOperationException("This method is not supported by this color model");
		}

		/// <summary>
		/// Disposes of system resources associated with this
		/// <code>ColorModel</code> once this <code>ColorModel</code> is no
		/// longer referenced.
		/// </summary>
		~ColorModel()
		{
		}


		/// <summary>
		/// Returns a <code>Raster</code> representing the alpha channel of an
		/// image, extracted from the input <code>Raster</code>, provided that
		/// pixel values of this <code>ColorModel</code> represent color and
		/// alpha information as separate spatial bands (e.g.
		/// <seealso cref="ComponentColorModel"/> and <code>DirectColorModel</code>).
		/// This method assumes that <code>Raster</code> objects associated
		/// with such a <code>ColorModel</code> store the alpha band, if
		/// present, as the last band of image data.  Returns <code>null</code>
		/// if there is no separate spatial alpha channel associated with this
		/// <code>ColorModel</code>.  If this is an
		/// <code>IndexColorModel</code> which has alpha in the lookup table,
		/// this method will return <code>null</code> since
		/// there is no spatially discrete alpha channel.
		/// This method will create a new <code>Raster</code> (but will share
		/// the data array).
		/// Since <code>ColorModel</code> is an abstract class, any instance
		/// is an instance of a subclass.  Subclasses must override this
		/// method to get any behavior other than returning <code>null</code>
		/// because the implementation in this abstract class returns
		/// <code>null</code>. </summary>
		/// <param name="raster"> the specified <code>Raster</code> </param>
		/// <returns> a <code>Raster</code> representing the alpha channel of
		/// an image, obtained from the specified <code>Raster</code>. </returns>
		public virtual WritableRaster GetAlphaRaster(WritableRaster raster)
		{
			return null;
		}

		/// <summary>
		/// Returns the <code>String</code> representation of the contents of
		/// this <code>ColorModel</code>object. </summary>
		/// <returns> a <code>String</code> representing the contents of this
		/// <code>ColorModel</code> object. </returns>
		public override String ToString()
		{
		   return "ColorModel: #pixelBits = " + Pixel_bits + " numComponents = " + NumComponents_Renamed + " color space = " + ColorSpace_Renamed + " transparency = " + Transparency_Renamed + " has alpha = " + SupportsAlpha + " isAlphaPre = " + IsAlphaPremultiplied;
		}

		internal static int GetDefaultTransferType(int pixel_bits)
		{
			if (pixel_bits <= 8)
			{
				return DataBuffer.TYPE_BYTE;
			}
			else if (pixel_bits <= 16)
			{
				return DataBuffer.TYPE_USHORT;
			}
			else if (pixel_bits <= 32)
			{
				return DataBuffer.TYPE_INT;
			}
			else
			{
				return DataBuffer.TYPE_UNDEFINED;
			}
		}

		internal static sbyte[] L8Tos8 = null; // 8-bit linear to 8-bit non-linear sRGB LUT
		internal static sbyte[] S8Tol8 = null; // 8-bit non-linear sRGB to 8-bit linear LUT
		internal static sbyte[] L16Tos8 = null; // 16-bit linear to 8-bit non-linear sRGB LUT
		internal static short[] S8Tol16 = null; // 8-bit non-linear sRGB to 16-bit linear LUT

									// Maps to hold LUTs for grayscale conversions
		internal static IDictionary<ICC_ColorSpace, sbyte[]> G8Tos8Map = null; // 8-bit gray values to 8-bit sRGB values
		internal static IDictionary<ICC_ColorSpace, sbyte[]> Lg16Toog8Map = null; // 16-bit linear to 8-bit "other" gray
		internal static IDictionary<ICC_ColorSpace, sbyte[]> G16Tos8Map = null; // 16-bit gray values to 8-bit sRGB values
		internal static IDictionary<ICC_ColorSpace, short[]> Lg16Toog16Map = null; // 16-bit linear to 16-bit "other" gray

		internal static bool IsLinearRGBspace(ColorSpace cs)
		{
			// Note: CMM.LINEAR_RGBspace will be null if the linear
			// RGB space has not been created yet.
			return (cs == CMSManager.LINEAR_RGBspace);
		}

		internal static bool IsLinearGRAYspace(ColorSpace cs)
		{
			// Note: CMM.GRAYspace will be null if the linear
			// gray space has not been created yet.
			return (cs == CMSManager.GRAYspace);
		}

		internal static sbyte[] LinearRGB8TosRGB8LUT
		{
			get
			{
				if (L8Tos8 == null)
				{
					L8Tos8 = new sbyte[256];
					float input, output;
					// algorithm for linear RGB to nonlinear sRGB conversion
					// is from the IEC 61966-2-1 International Standard,
					// Colour Management - Default RGB colour space - sRGB,
					// First Edition, 1999-10,
					// avaiable for order at http://www.iec.ch
					for (int i = 0; i <= 255; i++)
					{
						input = ((float) i) / 255.0f;
						if (input <= 0.0031308f)
						{
							output = input * 12.92f;
						}
						else
						{
							output = 1.055f * ((float) System.Math.Pow(input, (1.0 / 2.4))) - 0.055f;
						}
						L8Tos8[i] = (sbyte) System.Math.Round(output * 255.0f);
					}
				}
				return L8Tos8;
			}
		}

		internal static sbyte[] GetsRGB8ToLinearRGB8LUT()
		{
			if (S8Tol8 == null)
			{
				S8Tol8 = new sbyte[256];
				float input, output;
				// algorithm from IEC 61966-2-1 International Standard
				for (int i = 0; i <= 255; i++)
				{
					input = ((float) i) / 255.0f;
					if (input <= 0.04045f)
					{
						output = input / 12.92f;
					}
					else
					{
						output = (float) System.Math.Pow((input + 0.055f) / 1.055f, 2.4);
					}
					S8Tol8[i] = (sbyte) System.Math.Round(output * 255.0f);
				}
			}
			return S8Tol8;
		}

		internal static sbyte[] LinearRGB16TosRGB8LUT
		{
			get
			{
				if (L16Tos8 == null)
				{
					L16Tos8 = new sbyte[65536];
					float input, output;
					// algorithm from IEC 61966-2-1 International Standard
					for (int i = 0; i <= 65535; i++)
					{
						input = ((float) i) / 65535.0f;
						if (input <= 0.0031308f)
						{
							output = input * 12.92f;
						}
						else
						{
							output = 1.055f * ((float) System.Math.Pow(input, (1.0 / 2.4))) - 0.055f;
						}
						L16Tos8[i] = (sbyte) System.Math.Round(output * 255.0f);
					}
				}
				return L16Tos8;
			}
		}

		internal static short[] GetsRGB8ToLinearRGB16LUT()
		{
			if (S8Tol16 == null)
			{
				S8Tol16 = new short[256];
				float input, output;
				// algorithm from IEC 61966-2-1 International Standard
				for (int i = 0; i <= 255; i++)
				{
					input = ((float) i) / 255.0f;
					if (input <= 0.04045f)
					{
						output = input / 12.92f;
					}
					else
					{
						output = (float) System.Math.Pow((input + 0.055f) / 1.055f, 2.4);
					}
					S8Tol16[i] = (short) System.Math.Round(output * 65535.0f);
				}
			}
			return S8Tol16;
		}

		/*
		 * Return a byte LUT that converts 8-bit gray values in the grayCS
		 * ColorSpace to the appropriate 8-bit sRGB value.  I.e., if lut
		 * is the byte array returned by this method and sval = lut[gval],
		 * then the sRGB triple (sval,sval,sval) is the best match to gval.
		 * Cache references to any computed LUT in a Map.
		 */
		internal static sbyte[] GetGray8TosRGB8LUT(ICC_ColorSpace grayCS)
		{
			if (IsLinearGRAYspace(grayCS))
			{
				return LinearRGB8TosRGB8LUT;
			}
			if (G8Tos8Map != null)
			{
				sbyte[] g8Tos8LUT = G8Tos8Map[grayCS];
				if (g8Tos8LUT != null)
				{
					return g8Tos8LUT;
				}
			}
			sbyte[] g8Tos8LUT = new sbyte[256];
			for (int i = 0; i <= 255; i++)
			{
				g8Tos8LUT[i] = (sbyte) i;
			}
			ColorTransform[] transformList = new ColorTransform[2];
			PCMM mdl = CMSManager.Module;
			ICC_ColorSpace srgbCS = (ICC_ColorSpace) ColorSpace.GetInstance(ColorSpace.CS_sRGB_Renamed);
			transformList[0] = mdl.createTransform(grayCS.Profile, ColorTransform.Any, ColorTransform.In);
			transformList[1] = mdl.createTransform(srgbCS.Profile, ColorTransform.Any, ColorTransform.Out);
			ColorTransform t = mdl.createTransform(transformList);
			sbyte[] tmp = t.colorConvert(g8Tos8LUT, null);
			for (int i = 0, j = 2; i <= 255; i++, j += 3)
			{
				// All three components of tmp should be equal, since
				// the input color space to colorConvert is a gray scale
				// space.  However, there are slight anomalies in the results.
				// Copy tmp starting at index 2, since colorConvert seems
				// to be slightly more accurate for the third component!
				g8Tos8LUT[i] = tmp[j];
			}
			if (G8Tos8Map == null)
			{
				G8Tos8Map = Collections.SynchronizedMap(new WeakHashMap<ICC_ColorSpace, sbyte[]>(2));
			}
			G8Tos8Map[grayCS] = g8Tos8LUT;
			return g8Tos8LUT;
		}

		/*
		 * Return a byte LUT that converts 16-bit gray values in the CS_GRAY
		 * linear gray ColorSpace to the appropriate 8-bit value in the
		 * grayCS ColorSpace.  Cache references to any computed LUT in a Map.
		 */
		internal static sbyte[] GetLinearGray16ToOtherGray8LUT(ICC_ColorSpace grayCS)
		{
			if (Lg16Toog8Map != null)
			{
				sbyte[] lg16Toog8LUT = Lg16Toog8Map[grayCS];
				if (lg16Toog8LUT != null)
				{
					return lg16Toog8LUT;
				}
			}
			short[] tmp = new short[65536];
			for (int i = 0; i <= 65535; i++)
			{
				tmp[i] = (short) i;
			}
			ColorTransform[] transformList = new ColorTransform[2];
			PCMM mdl = CMSManager.Module;
			ICC_ColorSpace lgCS = (ICC_ColorSpace) ColorSpace.GetInstance(ColorSpace.CS_GRAY);
			transformList[0] = mdl.createTransform(lgCS.Profile, ColorTransform.Any, ColorTransform.In);
			transformList[1] = mdl.createTransform(grayCS.Profile, ColorTransform.Any, ColorTransform.Out);
			ColorTransform t = mdl.createTransform(transformList);
			tmp = t.colorConvert(tmp, null);
			sbyte[] lg16Toog8LUT = new sbyte[65536];
			for (int i = 0; i <= 65535; i++)
			{
				// scale unsigned short (0 - 65535) to unsigned byte (0 - 255)
				lg16Toog8LUT[i] = (sbyte)(((float)(tmp[i] & 0xffff)) * (1.0f / 257.0f) + 0.5f);
			}
			if (Lg16Toog8Map == null)
			{
				Lg16Toog8Map = Collections.SynchronizedMap(new WeakHashMap<ICC_ColorSpace, sbyte[]>(2));
			}
			Lg16Toog8Map[grayCS] = lg16Toog8LUT;
			return lg16Toog8LUT;
		}

		/*
		 * Return a byte LUT that converts 16-bit gray values in the grayCS
		 * ColorSpace to the appropriate 8-bit sRGB value.  I.e., if lut
		 * is the byte array returned by this method and sval = lut[gval],
		 * then the sRGB triple (sval,sval,sval) is the best match to gval.
		 * Cache references to any computed LUT in a Map.
		 */
		internal static sbyte[] GetGray16TosRGB8LUT(ICC_ColorSpace grayCS)
		{
			if (IsLinearGRAYspace(grayCS))
			{
				return LinearRGB16TosRGB8LUT;
			}
			if (G16Tos8Map != null)
			{
				sbyte[] g16Tos8LUT = G16Tos8Map[grayCS];
				if (g16Tos8LUT != null)
				{
					return g16Tos8LUT;
				}
			}
			short[] tmp = new short[65536];
			for (int i = 0; i <= 65535; i++)
			{
				tmp[i] = (short) i;
			}
			ColorTransform[] transformList = new ColorTransform[2];
			PCMM mdl = CMSManager.Module;
			ICC_ColorSpace srgbCS = (ICC_ColorSpace) ColorSpace.GetInstance(ColorSpace.CS_sRGB_Renamed);
			transformList[0] = mdl.createTransform(grayCS.Profile, ColorTransform.Any, ColorTransform.In);
			transformList[1] = mdl.createTransform(srgbCS.Profile, ColorTransform.Any, ColorTransform.Out);
			ColorTransform t = mdl.createTransform(transformList);
			tmp = t.colorConvert(tmp, null);
			sbyte[] g16Tos8LUT = new sbyte[65536];
			for (int i = 0, j = 2; i <= 65535; i++, j += 3)
			{
				// All three components of tmp should be equal, since
				// the input color space to colorConvert is a gray scale
				// space.  However, there are slight anomalies in the results.
				// Copy tmp starting at index 2, since colorConvert seems
				// to be slightly more accurate for the third component!

				// scale unsigned short (0 - 65535) to unsigned byte (0 - 255)
				g16Tos8LUT[i] = (sbyte)(((float)(tmp[j] & 0xffff)) * (1.0f / 257.0f) + 0.5f);
			}
			if (G16Tos8Map == null)
			{
				G16Tos8Map = Collections.SynchronizedMap(new WeakHashMap<ICC_ColorSpace, sbyte[]>(2));
			}
			G16Tos8Map[grayCS] = g16Tos8LUT;
			return g16Tos8LUT;
		}

		/*
		 * Return a short LUT that converts 16-bit gray values in the CS_GRAY
		 * linear gray ColorSpace to the appropriate 16-bit value in the
		 * grayCS ColorSpace.  Cache references to any computed LUT in a Map.
		 */
		internal static short[] GetLinearGray16ToOtherGray16LUT(ICC_ColorSpace grayCS)
		{
			if (Lg16Toog16Map != null)
			{
				short[] lg16Toog16LUT = Lg16Toog16Map[grayCS];
				if (lg16Toog16LUT != null)
				{
					return lg16Toog16LUT;
				}
			}
			short[] tmp = new short[65536];
			for (int i = 0; i <= 65535; i++)
			{
				tmp[i] = (short) i;
			}
			ColorTransform[] transformList = new ColorTransform[2];
			PCMM mdl = CMSManager.Module;
			ICC_ColorSpace lgCS = (ICC_ColorSpace) ColorSpace.GetInstance(ColorSpace.CS_GRAY);
			transformList[0] = mdl.createTransform(lgCS.Profile, ColorTransform.Any, ColorTransform.In);
			transformList[1] = mdl.createTransform(grayCS.Profile, ColorTransform.Any, ColorTransform.Out);
			ColorTransform t = mdl.createTransform(transformList);
			short[] lg16Toog16LUT = t.colorConvert(tmp, null);
			if (Lg16Toog16Map == null)
			{
				Lg16Toog16Map = Collections.SynchronizedMap(new WeakHashMap<ICC_ColorSpace, short[]>(2));
			}
			Lg16Toog16Map[grayCS] = lg16Toog16LUT;
			return lg16Toog16LUT;
		}

	}

}
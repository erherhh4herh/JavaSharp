using System;

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

/*
 **********************************************************************
 **********************************************************************
 **********************************************************************
 *** COPYRIGHT (c) Eastman Kodak Company, 1997                      ***
 *** As  an unpublished  work pursuant to Title 17 of the United    ***
 *** States Code.  All rights reserved.                             ***
 **********************************************************************
 **********************************************************************
 **********************************************************************/

namespace java.awt.color
{

	using ColorTransform = sun.java2d.cmm.ColorTransform;
	using CMSManager = sun.java2d.cmm.CMSManager;
	using PCMM = sun.java2d.cmm.PCMM;


	/// 
	/// <summary>
	/// The ICC_ColorSpace class is an implementation of the abstract
	/// ColorSpace class.  This representation of
	/// device independent and device dependent color spaces is based on the
	/// International Color Consortium Specification ICC.1:2001-12, File Format for
	/// Color Profiles (see <A href="http://www.color.org">http://www.color.org</A>).
	/// <para>
	/// Typically, a Color or ColorModel would be associated with an ICC
	/// Profile which is either an input, display, or output profile (see
	/// the ICC specification).  There are other types of ICC Profiles, e.g.
	/// abstract profiles, device link profiles, and named color profiles,
	/// which do not contain information appropriate for representing the color
	/// space of a color, image, or device (see ICC_Profile).
	/// Attempting to create an ICC_ColorSpace object from an inappropriate ICC
	/// Profile is an error.
	/// </para>
	/// <para>
	/// ICC Profiles represent transformations from the color space of
	/// the profile (e.g. a monitor) to a Profile Connection Space (PCS).
	/// Profiles of interest for tagging images or colors have a
	/// PCS which is one of the device independent
	/// spaces (one CIEXYZ space and two CIELab spaces) defined in the
	/// ICC Profile Format Specification.  Most profiles of interest
	/// either have invertible transformations or explicitly specify
	/// transformations going both directions.  Should an ICC_ColorSpace
	/// object be used in a way requiring a conversion from PCS to
	/// the profile's native space and there is inadequate data to
	/// correctly perform the conversion, the ICC_ColorSpace object will
	/// produce output in the specified type of color space (e.g. TYPE_RGB,
	/// TYPE_CMYK, etc.), but the specific color values of the output data
	/// will be undefined.
	/// </para>
	/// <para>
	/// The details of this class are not important for simple applets,
	/// which draw in a default color space or manipulate and display
	/// imported images with a known color space.  At most, such applets
	/// would need to get one of the default color spaces via
	/// ColorSpace.getInstance().
	/// </para>
	/// </summary>
	/// <seealso cref= ColorSpace </seealso>
	/// <seealso cref= ICC_Profile </seealso>



	public class ICC_ColorSpace : ColorSpace
	{

		internal new const long SerialVersionUID = 3455889114070431483L;

		private ICC_Profile ThisProfile;
		private float[] MinVal;
		private float[] MaxVal;
		private float[] DiffMinMax;
		private float[] InvDiffMinMax;
		private bool NeedScaleInit = true;

		// {to,from}{RGB,CIEXYZ} methods create and cache these when needed
		[NonSerialized]
		private ColorTransform This2srgb;
		[NonSerialized]
		private ColorTransform Srgb2this;
		[NonSerialized]
		private ColorTransform This2xyz;
		[NonSerialized]
		private ColorTransform Xyz2this;


		/// <summary>
		/// Constructs a new ICC_ColorSpace from an ICC_Profile object. </summary>
		/// <param name="profile"> the specified ICC_Profile object </param>
		/// <exception cref="IllegalArgumentException"> if profile is inappropriate for
		///            representing a ColorSpace. </exception>
		public ICC_ColorSpace(ICC_Profile profile) : base(profile.ColorSpaceType, profile.NumComponents)
		{

			int profileClass = profile.ProfileClass;

			/* REMIND - is NAMEDCOLOR OK? */
			if ((profileClass != ICC_Profile.CLASS_INPUT) && (profileClass != ICC_Profile.CLASS_DISPLAY) && (profileClass != ICC_Profile.CLASS_OUTPUT) && (profileClass != ICC_Profile.CLASS_COLORSPACECONVERSION) && (profileClass != ICC_Profile.CLASS_NAMEDCOLOR) && (profileClass != ICC_Profile.CLASS_ABSTRACT))
			{
				throw new IllegalArgumentException("Invalid profile type");
			}

			ThisProfile = profile;
			SetMinMax();
		}

		/// <summary>
		/// Returns the ICC_Profile for this ICC_ColorSpace. </summary>
		/// <returns> the ICC_Profile for this ICC_ColorSpace. </returns>
		public virtual ICC_Profile Profile
		{
			get
			{
				return ThisProfile;
			}
		}

		/// <summary>
		/// Transforms a color value assumed to be in this ColorSpace
		/// into a value in the default CS_sRGB color space.
		/// <para>
		/// This method transforms color values using algorithms designed
		/// to produce the best perceptual match between input and output
		/// colors.  In order to do colorimetric conversion of color values,
		/// you should use the <code>toCIEXYZ</code>
		/// method of this color space to first convert from the input
		/// color space to the CS_CIEXYZ color space, and then use the
		/// <code>fromCIEXYZ</code> method of the CS_sRGB color space to
		/// convert from CS_CIEXYZ to the output color space.
		/// See <seealso cref="#toCIEXYZ(float[]) toCIEXYZ"/> and
		/// <seealso cref="#fromCIEXYZ(float[]) fromCIEXYZ"/> for further information.
		/// </para>
		/// <para>
		/// </para>
		/// </summary>
		/// <param name="colorvalue"> a float array with length of at least the number
		///      of components in this ColorSpace. </param>
		/// <returns> a float array of length 3. </returns>
		/// <exception cref="ArrayIndexOutOfBoundsException"> if array length is not
		/// at least the number of components in this ColorSpace. </exception>
		public override float[] ToRGB(float[] colorvalue)
		{

			if (This2srgb == null)
			{
				ColorTransform[] transformList = new ColorTransform [2];
				ICC_ColorSpace srgbCS = (ICC_ColorSpace) ColorSpace.GetInstance(CS_sRGB_Renamed);
				PCMM mdl = CMSManager.Module;
				transformList[0] = mdl.createTransform(ThisProfile, ColorTransform.Any, ColorTransform.In);
				transformList[1] = mdl.createTransform(srgbCS.Profile, ColorTransform.Any, ColorTransform.Out);
				This2srgb = mdl.createTransform(transformList);
				if (NeedScaleInit)
				{
					SetComponentScaling();
				}
			}

			int nc = this.NumComponents;
			short[] tmp = new short[nc];
			for (int i = 0; i < nc; i++)
			{
				tmp[i] = (short)((colorvalue[i] - MinVal[i]) * InvDiffMinMax[i] + 0.5f);
			}
			tmp = This2srgb.colorConvert(tmp, null);
			float[] result = new float [3];
			for (int i = 0; i < 3; i++)
			{
				result[i] = ((float)(tmp[i] & 0xffff)) / 65535.0f;
			}
			return result;
		}

		/// <summary>
		/// Transforms a color value assumed to be in the default CS_sRGB
		/// color space into this ColorSpace.
		/// <para>
		/// This method transforms color values using algorithms designed
		/// to produce the best perceptual match between input and output
		/// colors.  In order to do colorimetric conversion of color values,
		/// you should use the <code>toCIEXYZ</code>
		/// method of the CS_sRGB color space to first convert from the input
		/// color space to the CS_CIEXYZ color space, and then use the
		/// <code>fromCIEXYZ</code> method of this color space to
		/// convert from CS_CIEXYZ to the output color space.
		/// See <seealso cref="#toCIEXYZ(float[]) toCIEXYZ"/> and
		/// <seealso cref="#fromCIEXYZ(float[]) fromCIEXYZ"/> for further information.
		/// </para>
		/// <para>
		/// </para>
		/// </summary>
		/// <param name="rgbvalue"> a float array with length of at least 3. </param>
		/// <returns> a float array with length equal to the number of
		///       components in this ColorSpace. </returns>
		/// <exception cref="ArrayIndexOutOfBoundsException"> if array length is not
		/// at least 3. </exception>
		public override float[] FromRGB(float[] rgbvalue)
		{

			if (Srgb2this == null)
			{
				ColorTransform[] transformList = new ColorTransform [2];
				ICC_ColorSpace srgbCS = (ICC_ColorSpace) ColorSpace.GetInstance(CS_sRGB_Renamed);
				PCMM mdl = CMSManager.Module;
				transformList[0] = mdl.createTransform(srgbCS.Profile, ColorTransform.Any, ColorTransform.In);
				transformList[1] = mdl.createTransform(ThisProfile, ColorTransform.Any, ColorTransform.Out);
				Srgb2this = mdl.createTransform(transformList);
				if (NeedScaleInit)
				{
					SetComponentScaling();
				}
			}

			short[] tmp = new short[3];
			for (int i = 0; i < 3; i++)
			{
				tmp[i] = (short)((rgbvalue[i] * 65535.0f) + 0.5f);
			}
			tmp = Srgb2this.colorConvert(tmp, null);
			int nc = this.NumComponents;
			float[] result = new float [nc];
			for (int i = 0; i < nc; i++)
			{
				result[i] = (((float)(tmp[i] & 0xffff)) / 65535.0f) * DiffMinMax[i] + MinVal[i];
			}
			return result;
		}


		/// <summary>
		/// Transforms a color value assumed to be in this ColorSpace
		/// into the CS_CIEXYZ conversion color space.
		/// <para>
		/// This method transforms color values using relative colorimetry,
		/// as defined by the ICC Specification.  This
		/// means that the XYZ values returned by this method are represented
		/// relative to the D50 white point of the CS_CIEXYZ color space.
		/// This representation is useful in a two-step color conversion
		/// process in which colors are transformed from an input color
		/// space to CS_CIEXYZ and then to an output color space.  This
		/// representation is not the same as the XYZ values that would
		/// be measured from the given color value by a colorimeter.
		/// A further transformation is necessary to compute the XYZ values
		/// that would be measured using current CIE recommended practices.
		/// The paragraphs below explain this in more detail.
		/// </para>
		/// <para>
		/// The ICC standard uses a device independent color space (DICS) as the
		/// mechanism for converting color from one device to another device.  In
		/// this architecture, colors are converted from the source device's color
		/// space to the ICC DICS and then from the ICC DICS to the destination
		/// device's color space.  The ICC standard defines device profiles which
		/// contain transforms which will convert between a device's color space
		/// and the ICC DICS.  The overall conversion of colors from a source
		/// device to colors of a destination device is done by connecting the
		/// device-to-DICS transform of the profile for the source device to the
		/// DICS-to-device transform of the profile for the destination device.
		/// For this reason, the ICC DICS is commonly referred to as the profile
		/// connection space (PCS).  The color space used in the methods
		/// toCIEXYZ and fromCIEXYZ is the CIEXYZ PCS defined by the ICC
		/// Specification.  This is also the color space represented by
		/// ColorSpace.CS_CIEXYZ.
		/// </para>
		/// <para>
		/// The XYZ values of a color are often represented as relative to some
		/// white point, so the actual meaning of the XYZ values cannot be known
		/// without knowing the white point of those values.  This is known as
		/// relative colorimetry.  The PCS uses a white point of D50, so the XYZ
		/// values of the PCS are relative to D50.  For example, white in the PCS
		/// will have the XYZ values of D50, which is defined to be X=.9642,
		/// Y=1.000, and Z=0.8249.  This white point is commonly used for graphic
		/// arts applications, but others are often used in other applications.
		/// </para>
		/// <para>
		/// To quantify the color characteristics of a device such as a printer
		/// or monitor, measurements of XYZ values for particular device colors
		/// are typically made.  For purposes of this discussion, the term
		/// device XYZ values is used to mean the XYZ values that would be
		/// measured from device colors using current CIE recommended practices.
		/// </para>
		/// <para>
		/// Converting between device XYZ values and the PCS XYZ values returned
		/// by this method corresponds to converting between the device's color
		/// space, as represented by CIE colorimetric values, and the PCS.  There
		/// are many factors involved in this process, some of which are quite
		/// subtle.  The most important, however, is the adjustment made to account
		/// for differences between the device's white point and the white point of
		/// the PCS.  There are many techniques for doing this and it is the
		/// subject of much current research and controversy.  Some commonly used
		/// methods are XYZ scaling, the von Kries transform, and the Bradford
		/// transform.  The proper method to use depends upon each particular
		/// application.
		/// </para>
		/// <para>
		/// The simplest method is XYZ scaling.  In this method each device XYZ
		/// value is  converted to a PCS XYZ value by multiplying it by the ratio
		/// of the PCS white point (D50) to the device white point.
		/// <pre>
		/// 
		/// Xd, Yd, Zd are the device XYZ values
		/// Xdw, Ydw, Zdw are the device XYZ white point values
		/// Xp, Yp, Zp are the PCS XYZ values
		/// Xd50, Yd50, Zd50 are the PCS XYZ white point values
		/// 
		/// Xp = Xd * (Xd50 / Xdw)
		/// Yp = Yd * (Yd50 / Ydw)
		/// Zp = Zd * (Zd50 / Zdw)
		/// 
		/// </pre>
		/// </para>
		/// <para>
		/// Conversion from the PCS to the device would be done by inverting these
		/// equations:
		/// <pre>
		/// 
		/// Xd = Xp * (Xdw / Xd50)
		/// Yd = Yp * (Ydw / Yd50)
		/// Zd = Zp * (Zdw / Zd50)
		/// 
		/// </pre>
		/// </para>
		/// <para>
		/// Note that the media white point tag in an ICC profile is not the same
		/// as the device white point.  The media white point tag is expressed in
		/// PCS values and is used to represent the difference between the XYZ of
		/// device illuminant and the XYZ of the device media when measured under
		/// that illuminant.  The device white point is expressed as the device
		/// XYZ values corresponding to white displayed on the device.  For
		/// example, displaying the RGB color (1.0, 1.0, 1.0) on an sRGB device
		/// will result in a measured device XYZ value of D65.  This will not
		/// be the same as the media white point tag XYZ value in the ICC
		/// profile for an sRGB device.
		/// </para>
		/// <para>
		/// </para>
		/// </summary>
		/// <param name="colorvalue"> a float array with length of at least the number
		///        of components in this ColorSpace. </param>
		/// <returns> a float array of length 3. </returns>
		/// <exception cref="ArrayIndexOutOfBoundsException"> if array length is not
		/// at least the number of components in this ColorSpace. </exception>
		public override float[] ToCIEXYZ(float[] colorvalue)
		{

			if (This2xyz == null)
			{
				ColorTransform[] transformList = new ColorTransform [2];
				ICC_ColorSpace xyzCS = (ICC_ColorSpace) ColorSpace.GetInstance(CS_CIEXYZ);
				PCMM mdl = CMSManager.Module;
				try
				{
					transformList[0] = mdl.createTransform(ThisProfile, ICC_Profile.IcRelativeColorimetric, ColorTransform.In);
				}
				catch (CMMException)
				{
					transformList[0] = mdl.createTransform(ThisProfile, ColorTransform.Any, ColorTransform.In);
				}
				transformList[1] = mdl.createTransform(xyzCS.Profile, ColorTransform.Any, ColorTransform.Out);
				This2xyz = mdl.createTransform(transformList);
				if (NeedScaleInit)
				{
					SetComponentScaling();
				}
			}

			int nc = this.NumComponents;
			short[] tmp = new short[nc];
			for (int i = 0; i < nc; i++)
			{
				tmp[i] = (short)((colorvalue[i] - MinVal[i]) * InvDiffMinMax[i] + 0.5f);
			}
			tmp = This2xyz.colorConvert(tmp, null);
			float ALMOST_TWO = 1.0f + (32767.0f / 32768.0f);
			// For CIEXYZ, min = 0.0, max = ALMOST_TWO for all components
			float[] result = new float [3];
			for (int i = 0; i < 3; i++)
			{
				result[i] = (((float)(tmp[i] & 0xffff)) / 65535.0f) * ALMOST_TWO;
			}
			return result;
		}


		/// <summary>
		/// Transforms a color value assumed to be in the CS_CIEXYZ conversion
		/// color space into this ColorSpace.
		/// <para>
		/// This method transforms color values using relative colorimetry,
		/// as defined by the ICC Specification.  This
		/// means that the XYZ argument values taken by this method are represented
		/// relative to the D50 white point of the CS_CIEXYZ color space.
		/// This representation is useful in a two-step color conversion
		/// process in which colors are transformed from an input color
		/// space to CS_CIEXYZ and then to an output color space.  The color
		/// values returned by this method are not those that would produce
		/// the XYZ value passed to the method when measured by a colorimeter.
		/// If you have XYZ values corresponding to measurements made using
		/// current CIE recommended practices, they must be converted to D50
		/// relative values before being passed to this method.
		/// The paragraphs below explain this in more detail.
		/// </para>
		/// <para>
		/// The ICC standard uses a device independent color space (DICS) as the
		/// mechanism for converting color from one device to another device.  In
		/// this architecture, colors are converted from the source device's color
		/// space to the ICC DICS and then from the ICC DICS to the destination
		/// device's color space.  The ICC standard defines device profiles which
		/// contain transforms which will convert between a device's color space
		/// and the ICC DICS.  The overall conversion of colors from a source
		/// device to colors of a destination device is done by connecting the
		/// device-to-DICS transform of the profile for the source device to the
		/// DICS-to-device transform of the profile for the destination device.
		/// For this reason, the ICC DICS is commonly referred to as the profile
		/// connection space (PCS).  The color space used in the methods
		/// toCIEXYZ and fromCIEXYZ is the CIEXYZ PCS defined by the ICC
		/// Specification.  This is also the color space represented by
		/// ColorSpace.CS_CIEXYZ.
		/// </para>
		/// <para>
		/// The XYZ values of a color are often represented as relative to some
		/// white point, so the actual meaning of the XYZ values cannot be known
		/// without knowing the white point of those values.  This is known as
		/// relative colorimetry.  The PCS uses a white point of D50, so the XYZ
		/// values of the PCS are relative to D50.  For example, white in the PCS
		/// will have the XYZ values of D50, which is defined to be X=.9642,
		/// Y=1.000, and Z=0.8249.  This white point is commonly used for graphic
		/// arts applications, but others are often used in other applications.
		/// </para>
		/// <para>
		/// To quantify the color characteristics of a device such as a printer
		/// or monitor, measurements of XYZ values for particular device colors
		/// are typically made.  For purposes of this discussion, the term
		/// device XYZ values is used to mean the XYZ values that would be
		/// measured from device colors using current CIE recommended practices.
		/// </para>
		/// <para>
		/// Converting between device XYZ values and the PCS XYZ values taken as
		/// arguments by this method corresponds to converting between the device's
		/// color space, as represented by CIE colorimetric values, and the PCS.
		/// There are many factors involved in this process, some of which are quite
		/// subtle.  The most important, however, is the adjustment made to account
		/// for differences between the device's white point and the white point of
		/// the PCS.  There are many techniques for doing this and it is the
		/// subject of much current research and controversy.  Some commonly used
		/// methods are XYZ scaling, the von Kries transform, and the Bradford
		/// transform.  The proper method to use depends upon each particular
		/// application.
		/// </para>
		/// <para>
		/// The simplest method is XYZ scaling.  In this method each device XYZ
		/// value is  converted to a PCS XYZ value by multiplying it by the ratio
		/// of the PCS white point (D50) to the device white point.
		/// <pre>
		/// 
		/// Xd, Yd, Zd are the device XYZ values
		/// Xdw, Ydw, Zdw are the device XYZ white point values
		/// Xp, Yp, Zp are the PCS XYZ values
		/// Xd50, Yd50, Zd50 are the PCS XYZ white point values
		/// 
		/// Xp = Xd * (Xd50 / Xdw)
		/// Yp = Yd * (Yd50 / Ydw)
		/// Zp = Zd * (Zd50 / Zdw)
		/// 
		/// </pre>
		/// </para>
		/// <para>
		/// Conversion from the PCS to the device would be done by inverting these
		/// equations:
		/// <pre>
		/// 
		/// Xd = Xp * (Xdw / Xd50)
		/// Yd = Yp * (Ydw / Yd50)
		/// Zd = Zp * (Zdw / Zd50)
		/// 
		/// </pre>
		/// </para>
		/// <para>
		/// Note that the media white point tag in an ICC profile is not the same
		/// as the device white point.  The media white point tag is expressed in
		/// PCS values and is used to represent the difference between the XYZ of
		/// device illuminant and the XYZ of the device media when measured under
		/// that illuminant.  The device white point is expressed as the device
		/// XYZ values corresponding to white displayed on the device.  For
		/// example, displaying the RGB color (1.0, 1.0, 1.0) on an sRGB device
		/// will result in a measured device XYZ value of D65.  This will not
		/// be the same as the media white point tag XYZ value in the ICC
		/// profile for an sRGB device.
		/// </para>
		/// <para>
		/// </para>
		/// </summary>
		/// <param name="colorvalue"> a float array with length of at least 3. </param>
		/// <returns> a float array with length equal to the number of
		///         components in this ColorSpace. </returns>
		/// <exception cref="ArrayIndexOutOfBoundsException"> if array length is not
		/// at least 3. </exception>
		public override float[] FromCIEXYZ(float[] colorvalue)
		{

			if (Xyz2this == null)
			{
				ColorTransform[] transformList = new ColorTransform [2];
				ICC_ColorSpace xyzCS = (ICC_ColorSpace) ColorSpace.GetInstance(CS_CIEXYZ);
				PCMM mdl = CMSManager.Module;
				transformList[0] = mdl.createTransform(xyzCS.Profile, ColorTransform.Any, ColorTransform.In);
				try
				{
					transformList[1] = mdl.createTransform(ThisProfile, ICC_Profile.IcRelativeColorimetric, ColorTransform.Out);
				}
				catch (CMMException)
				{
					transformList[1] = CMSManager.Module.createTransform(ThisProfile, ColorTransform.Any, ColorTransform.Out);
				}
				Xyz2this = mdl.createTransform(transformList);
				if (NeedScaleInit)
				{
					SetComponentScaling();
				}
			}

			short[] tmp = new short[3];
			float ALMOST_TWO = 1.0f + (32767.0f / 32768.0f);
			float factor = 65535.0f / ALMOST_TWO;
			// For CIEXYZ, min = 0.0, max = ALMOST_TWO for all components
			for (int i = 0; i < 3; i++)
			{
				tmp[i] = (short)((colorvalue[i] * factor) + 0.5f);
			}
			tmp = Xyz2this.colorConvert(tmp, null);
			int nc = this.NumComponents;
			float[] result = new float [nc];
			for (int i = 0; i < nc; i++)
			{
				result[i] = (((float)(tmp[i] & 0xffff)) / 65535.0f) * DiffMinMax[i] + MinVal[i];
			}
			return result;
		}

		/// <summary>
		/// Returns the minimum normalized color component value for the
		/// specified component.  For TYPE_XYZ spaces, this method returns
		/// minimum values of 0.0 for all components.  For TYPE_Lab spaces,
		/// this method returns 0.0 for L and -128.0 for a and b components.
		/// This is consistent with the encoding of the XYZ and Lab Profile
		/// Connection Spaces in the ICC specification.  For all other types, this
		/// method returns 0.0 for all components.  When using an ICC_ColorSpace
		/// with a profile that requires different minimum component values,
		/// it is necessary to subclass this class and override this method. </summary>
		/// <param name="component"> The component index. </param>
		/// <returns> The minimum normalized component value. </returns>
		/// <exception cref="IllegalArgumentException"> if component is less than 0 or
		///         greater than numComponents - 1.
		/// @since 1.4 </exception>
		public override float GetMinValue(int component)
		{
			if ((component < 0) || (component > this.NumComponents - 1))
			{
				throw new IllegalArgumentException("Component index out of range: + component");
			}
			return MinVal[component];
		}

		/// <summary>
		/// Returns the maximum normalized color component value for the
		/// specified component.  For TYPE_XYZ spaces, this method returns
		/// maximum values of 1.0 + (32767.0 / 32768.0) for all components.
		/// For TYPE_Lab spaces,
		/// this method returns 100.0 for L and 127.0 for a and b components.
		/// This is consistent with the encoding of the XYZ and Lab Profile
		/// Connection Spaces in the ICC specification.  For all other types, this
		/// method returns 1.0 for all components.  When using an ICC_ColorSpace
		/// with a profile that requires different maximum component values,
		/// it is necessary to subclass this class and override this method. </summary>
		/// <param name="component"> The component index. </param>
		/// <returns> The maximum normalized component value. </returns>
		/// <exception cref="IllegalArgumentException"> if component is less than 0 or
		///         greater than numComponents - 1.
		/// @since 1.4 </exception>
		public override float GetMaxValue(int component)
		{
			if ((component < 0) || (component > this.NumComponents - 1))
			{
				throw new IllegalArgumentException("Component index out of range: + component");
			}
			return MaxVal[component];
		}

		private void SetMinMax()
		{
			int nc = this.NumComponents;
			int type = this.Type;
			MinVal = new float[nc];
			MaxVal = new float[nc];
			if (type == ColorSpace.TYPE_Lab)
			{
				MinVal[0] = 0.0f; // L
				MaxVal[0] = 100.0f;
				MinVal[1] = -128.0f; // a
				MaxVal[1] = 127.0f;
				MinVal[2] = -128.0f; // b
				MaxVal[2] = 127.0f;
			}
			else if (type == ColorSpace.TYPE_XYZ)
			{
				MinVal[0] = MinVal[1] = MinVal[2] = 0.0f; // X, Y, Z
				MaxVal[0] = MaxVal[1] = MaxVal[2] = 1.0f + (32767.0f / 32768.0f);
			}
			else
			{
				for (int i = 0; i < nc; i++)
				{
					MinVal[i] = 0.0f;
					MaxVal[i] = 1.0f;
				}
			}
		}

		private void SetComponentScaling()
		{
			int nc = this.NumComponents;
			DiffMinMax = new float[nc];
			InvDiffMinMax = new float[nc];
			for (int i = 0; i < nc; i++)
			{
				MinVal[i] = this.GetMinValue(i); // in case getMinVal is overridden
				MaxVal[i] = this.GetMaxValue(i); // in case getMaxVal is overridden
				DiffMinMax[i] = MaxVal[i] - MinVal[i];
				InvDiffMinMax[i] = 65535.0f / DiffMinMax[i];
			}
			NeedScaleInit = false;
		}

	}

}
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

	using PCMM = sun.java2d.cmm.PCMM;
	using CMSManager = sun.java2d.cmm.CMSManager;
	using Profile = sun.java2d.cmm.Profile;
	using ProfileDataVerifier = sun.java2d.cmm.ProfileDataVerifier;
	using ProfileDeferralMgr = sun.java2d.cmm.ProfileDeferralMgr;
	using ProfileDeferralInfo = sun.java2d.cmm.ProfileDeferralInfo;
	using ProfileActivator = sun.java2d.cmm.ProfileActivator;




	/// <summary>
	/// A representation of color profile data for device independent and
	/// device dependent color spaces based on the International Color
	/// Consortium Specification ICC.1:2001-12, File Format for Color Profiles,
	/// (see <A href="http://www.color.org"> http://www.color.org</A>).
	/// <para>
	/// An ICC_ColorSpace object can be constructed from an appropriate
	/// ICC_Profile.
	/// Typically, an ICC_ColorSpace would be associated with an ICC
	/// Profile which is either an input, display, or output profile (see
	/// the ICC specification).  There are also device link, abstract,
	/// color space conversion, and named color profiles.  These are less
	/// useful for tagging a color or image, but are useful for other
	/// purposes (in particular device link profiles can provide improved
	/// performance for converting from one device's color space to
	/// another's).
	/// </para>
	/// <para>
	/// ICC Profiles represent transformations from the color space of
	/// the profile (e.g. a monitor) to a Profile Connection Space (PCS).
	/// Profiles of interest for tagging images or colors have a PCS
	/// which is one of the two specific device independent
	/// spaces (one CIEXYZ space and one CIELab space) defined in the
	/// ICC Profile Format Specification.  Most profiles of interest
	/// either have invertible transformations or explicitly specify
	/// transformations going both directions.
	/// </para>
	/// </summary>
	/// <seealso cref= ICC_ColorSpace </seealso>


	[Serializable]
	public class ICC_Profile
	{

		private const long SerialVersionUID = -3938515861990936766L;

		[NonSerialized]
		private Profile CmmProfile;

		[NonSerialized]
		private ProfileDeferralInfo DeferralInfo;
		[NonSerialized]
		private ProfileActivator ProfileActivator;

		// Registry of singleton profile objects for specific color spaces
		// defined in the ColorSpace class (e.g. CS_sRGB), see
		// getInstance(int cspace) factory method.
		private static ICC_Profile SRGBprofile;
		private static ICC_Profile XYZprofile;
		private static ICC_Profile PYCCprofile;
		private static ICC_Profile GRAYprofile;
		private static ICC_Profile LINEAR_RGBprofile;


		/// <summary>
		/// Profile class is input.
		/// </summary>
		public const int CLASS_INPUT = 0;

		/// <summary>
		/// Profile class is display.
		/// </summary>
		public const int CLASS_DISPLAY = 1;

		/// <summary>
		/// Profile class is output.
		/// </summary>
		public const int CLASS_OUTPUT = 2;

		/// <summary>
		/// Profile class is device link.
		/// </summary>
		public const int CLASS_DEVICELINK = 3;

		/// <summary>
		/// Profile class is color space conversion.
		/// </summary>
		public const int CLASS_COLORSPACECONVERSION = 4;

		/// <summary>
		/// Profile class is abstract.
		/// </summary>
		public const int CLASS_ABSTRACT = 5;

		/// <summary>
		/// Profile class is named color.
		/// </summary>
		public const int CLASS_NAMEDCOLOR = 6;


		/// <summary>
		/// ICC Profile Color Space Type Signature: 'XYZ '.
		/// </summary>
		public const int IcSigXYZData = 0x58595A20; // 'XYZ '

		/// <summary>
		/// ICC Profile Color Space Type Signature: 'Lab '.
		/// </summary>
		public const int IcSigLabData = 0x4C616220; // 'Lab '

		/// <summary>
		/// ICC Profile Color Space Type Signature: 'Luv '.
		/// </summary>
		public const int IcSigLuvData = 0x4C757620; // 'Luv '

		/// <summary>
		/// ICC Profile Color Space Type Signature: 'YCbr'.
		/// </summary>
		public const int IcSigYCbCrData = 0x59436272; // 'YCbr'

		/// <summary>
		/// ICC Profile Color Space Type Signature: 'Yxy '.
		/// </summary>
		public const int IcSigYxyData = 0x59787920; // 'Yxy '

		/// <summary>
		/// ICC Profile Color Space Type Signature: 'RGB '.
		/// </summary>
		public const int IcSigRgbData = 0x52474220; // 'RGB '

		/// <summary>
		/// ICC Profile Color Space Type Signature: 'GRAY'.
		/// </summary>
		public const int IcSigGrayData = 0x47524159; // 'GRAY'

		/// <summary>
		/// ICC Profile Color Space Type Signature: 'HSV'.
		/// </summary>
		public const int IcSigHsvData = 0x48535620; // 'HSV '

		/// <summary>
		/// ICC Profile Color Space Type Signature: 'HLS'.
		/// </summary>
		public const int IcSigHlsData = 0x484C5320; // 'HLS '

		/// <summary>
		/// ICC Profile Color Space Type Signature: 'CMYK'.
		/// </summary>
		public const int IcSigCmykData = 0x434D594B; // 'CMYK'

		/// <summary>
		/// ICC Profile Color Space Type Signature: 'CMY '.
		/// </summary>
		public const int IcSigCmyData = 0x434D5920; // 'CMY '

		/// <summary>
		/// ICC Profile Color Space Type Signature: '2CLR'.
		/// </summary>
		public const int IcSigSpace2CLR = 0x32434C52; // '2CLR'

		/// <summary>
		/// ICC Profile Color Space Type Signature: '3CLR'.
		/// </summary>
		public const int IcSigSpace3CLR = 0x33434C52; // '3CLR'

		/// <summary>
		/// ICC Profile Color Space Type Signature: '4CLR'.
		/// </summary>
		public const int IcSigSpace4CLR = 0x34434C52; // '4CLR'

		/// <summary>
		/// ICC Profile Color Space Type Signature: '5CLR'.
		/// </summary>
		public const int IcSigSpace5CLR = 0x35434C52; // '5CLR'

		/// <summary>
		/// ICC Profile Color Space Type Signature: '6CLR'.
		/// </summary>
		public const int IcSigSpace6CLR = 0x36434C52; // '6CLR'

		/// <summary>
		/// ICC Profile Color Space Type Signature: '7CLR'.
		/// </summary>
		public const int IcSigSpace7CLR = 0x37434C52; // '7CLR'

		/// <summary>
		/// ICC Profile Color Space Type Signature: '8CLR'.
		/// </summary>
		public const int IcSigSpace8CLR = 0x38434C52; // '8CLR'

		/// <summary>
		/// ICC Profile Color Space Type Signature: '9CLR'.
		/// </summary>
		public const int IcSigSpace9CLR = 0x39434C52; // '9CLR'

		/// <summary>
		/// ICC Profile Color Space Type Signature: 'ACLR'.
		/// </summary>
		public const int IcSigSpaceACLR = 0x41434C52; // 'ACLR'

		/// <summary>
		/// ICC Profile Color Space Type Signature: 'BCLR'.
		/// </summary>
		public const int IcSigSpaceBCLR = 0x42434C52; // 'BCLR'

		/// <summary>
		/// ICC Profile Color Space Type Signature: 'CCLR'.
		/// </summary>
		public const int IcSigSpaceCCLR = 0x43434C52; // 'CCLR'

		/// <summary>
		/// ICC Profile Color Space Type Signature: 'DCLR'.
		/// </summary>
		public const int IcSigSpaceDCLR = 0x44434C52; // 'DCLR'

		/// <summary>
		/// ICC Profile Color Space Type Signature: 'ECLR'.
		/// </summary>
		public const int IcSigSpaceECLR = 0x45434C52; // 'ECLR'

		/// <summary>
		/// ICC Profile Color Space Type Signature: 'FCLR'.
		/// </summary>
		public const int IcSigSpaceFCLR = 0x46434C52; // 'FCLR'


		/// <summary>
		/// ICC Profile Class Signature: 'scnr'.
		/// </summary>
		public const int IcSigInputClass = 0x73636E72; // 'scnr'

		/// <summary>
		/// ICC Profile Class Signature: 'mntr'.
		/// </summary>
		public const int IcSigDisplayClass = 0x6D6E7472; // 'mntr'

		/// <summary>
		/// ICC Profile Class Signature: 'prtr'.
		/// </summary>
		public const int IcSigOutputClass = 0x70727472; // 'prtr'

		/// <summary>
		/// ICC Profile Class Signature: 'link'.
		/// </summary>
		public const int IcSigLinkClass = 0x6C696E6B; // 'link'

		/// <summary>
		/// ICC Profile Class Signature: 'abst'.
		/// </summary>
		public const int IcSigAbstractClass = 0x61627374; // 'abst'

		/// <summary>
		/// ICC Profile Class Signature: 'spac'.
		/// </summary>
		public const int IcSigColorSpaceClass = 0x73706163; // 'spac'

		/// <summary>
		/// ICC Profile Class Signature: 'nmcl'.
		/// </summary>
		public const int IcSigNamedColorClass = 0x6e6d636c; // 'nmcl'


		/// <summary>
		/// ICC Profile Rendering Intent: Perceptual.
		/// </summary>
		public const int IcPerceptual = 0;

		/// <summary>
		/// ICC Profile Rendering Intent: RelativeColorimetric.
		/// </summary>
		public const int IcRelativeColorimetric = 1;

		/// <summary>
		/// ICC Profile Rendering Intent: Media-RelativeColorimetric.
		/// @since 1.5
		/// </summary>
		public const int IcMediaRelativeColorimetric = 1;

		/// <summary>
		/// ICC Profile Rendering Intent: Saturation.
		/// </summary>
		public const int IcSaturation = 2;

		/// <summary>
		/// ICC Profile Rendering Intent: AbsoluteColorimetric.
		/// </summary>
		public const int IcAbsoluteColorimetric = 3;

		/// <summary>
		/// ICC Profile Rendering Intent: ICC-AbsoluteColorimetric.
		/// @since 1.5
		/// </summary>
		public const int IcICCAbsoluteColorimetric = 3;


		/// <summary>
		/// ICC Profile Tag Signature: 'head' - special.
		/// </summary>
		public const int IcSigHead = 0x68656164; // 'head' - special

		/// <summary>
		/// ICC Profile Tag Signature: 'A2B0'.
		/// </summary>
		public const int IcSigAToB0Tag = 0x41324230; // 'A2B0'

		/// <summary>
		/// ICC Profile Tag Signature: 'A2B1'.
		/// </summary>
		public const int IcSigAToB1Tag = 0x41324231; // 'A2B1'

		/// <summary>
		/// ICC Profile Tag Signature: 'A2B2'.
		/// </summary>
		public const int IcSigAToB2Tag = 0x41324232; // 'A2B2'

		/// <summary>
		/// ICC Profile Tag Signature: 'bXYZ'.
		/// </summary>
		public const int IcSigBlueColorantTag = 0x6258595A; // 'bXYZ'

		/// <summary>
		/// ICC Profile Tag Signature: 'bXYZ'.
		/// @since 1.5
		/// </summary>
		public const int IcSigBlueMatrixColumnTag = 0x6258595A; // 'bXYZ'

		/// <summary>
		/// ICC Profile Tag Signature: 'bTRC'.
		/// </summary>
		public const int IcSigBlueTRCTag = 0x62545243; // 'bTRC'

		/// <summary>
		/// ICC Profile Tag Signature: 'B2A0'.
		/// </summary>
		public const int IcSigBToA0Tag = 0x42324130; // 'B2A0'

		/// <summary>
		/// ICC Profile Tag Signature: 'B2A1'.
		/// </summary>
		public const int IcSigBToA1Tag = 0x42324131; // 'B2A1'

		/// <summary>
		/// ICC Profile Tag Signature: 'B2A2'.
		/// </summary>
		public const int IcSigBToA2Tag = 0x42324132; // 'B2A2'

		/// <summary>
		/// ICC Profile Tag Signature: 'calt'.
		/// </summary>
		public const int IcSigCalibrationDateTimeTag = 0x63616C74;
																	   /* 'calt' */

		/// <summary>
		/// ICC Profile Tag Signature: 'targ'.
		/// </summary>
		public const int IcSigCharTargetTag = 0x74617267; // 'targ'

		/// <summary>
		/// ICC Profile Tag Signature: 'cprt'.
		/// </summary>
		public const int IcSigCopyrightTag = 0x63707274; // 'cprt'

		/// <summary>
		/// ICC Profile Tag Signature: 'crdi'.
		/// </summary>
		public const int IcSigCrdInfoTag = 0x63726469; // 'crdi'

		/// <summary>
		/// ICC Profile Tag Signature: 'dmnd'.
		/// </summary>
		public const int IcSigDeviceMfgDescTag = 0x646D6E64; // 'dmnd'

		/// <summary>
		/// ICC Profile Tag Signature: 'dmdd'.
		/// </summary>
		public const int IcSigDeviceModelDescTag = 0x646D6464; // 'dmdd'

		/// <summary>
		/// ICC Profile Tag Signature: 'devs'.
		/// </summary>
		public const int IcSigDeviceSettingsTag = 0x64657673; // 'devs'

		/// <summary>
		/// ICC Profile Tag Signature: 'gamt'.
		/// </summary>
		public const int IcSigGamutTag = 0x67616D74; // 'gamt'

		/// <summary>
		/// ICC Profile Tag Signature: 'kTRC'.
		/// </summary>
		public const int IcSigGrayTRCTag = 0x6b545243; // 'kTRC'

		/// <summary>
		/// ICC Profile Tag Signature: 'gXYZ'.
		/// </summary>
		public const int IcSigGreenColorantTag = 0x6758595A; // 'gXYZ'

		/// <summary>
		/// ICC Profile Tag Signature: 'gXYZ'.
		/// @since 1.5
		/// </summary>
		public const int IcSigGreenMatrixColumnTag = 0x6758595A; // 'gXYZ'

		/// <summary>
		/// ICC Profile Tag Signature: 'gTRC'.
		/// </summary>
		public const int IcSigGreenTRCTag = 0x67545243; // 'gTRC'

		/// <summary>
		/// ICC Profile Tag Signature: 'lumi'.
		/// </summary>
		public const int IcSigLuminanceTag = 0x6C756d69; // 'lumi'

		/// <summary>
		/// ICC Profile Tag Signature: 'meas'.
		/// </summary>
		public const int IcSigMeasurementTag = 0x6D656173; // 'meas'

		/// <summary>
		/// ICC Profile Tag Signature: 'bkpt'.
		/// </summary>
		public const int IcSigMediaBlackPointTag = 0x626B7074; // 'bkpt'

		/// <summary>
		/// ICC Profile Tag Signature: 'wtpt'.
		/// </summary>
		public const int IcSigMediaWhitePointTag = 0x77747074; // 'wtpt'

		/// <summary>
		/// ICC Profile Tag Signature: 'ncl2'.
		/// </summary>
		public const int IcSigNamedColor2Tag = 0x6E636C32; // 'ncl2'

		/// <summary>
		/// ICC Profile Tag Signature: 'resp'.
		/// </summary>
		public const int IcSigOutputResponseTag = 0x72657370; // 'resp'

		/// <summary>
		/// ICC Profile Tag Signature: 'pre0'.
		/// </summary>
		public const int IcSigPreview0Tag = 0x70726530; // 'pre0'

		/// <summary>
		/// ICC Profile Tag Signature: 'pre1'.
		/// </summary>
		public const int IcSigPreview1Tag = 0x70726531; // 'pre1'

		/// <summary>
		/// ICC Profile Tag Signature: 'pre2'.
		/// </summary>
		public const int IcSigPreview2Tag = 0x70726532; // 'pre2'

		/// <summary>
		/// ICC Profile Tag Signature: 'desc'.
		/// </summary>
		public const int IcSigProfileDescriptionTag = 0x64657363;
																	   /* 'desc' */

		/// <summary>
		/// ICC Profile Tag Signature: 'pseq'.
		/// </summary>
		public const int IcSigProfileSequenceDescTag = 0x70736571;
																	   /* 'pseq' */

		/// <summary>
		/// ICC Profile Tag Signature: 'psd0'.
		/// </summary>
		public const int IcSigPs2CRD0Tag = 0x70736430; // 'psd0'

		/// <summary>
		/// ICC Profile Tag Signature: 'psd1'.
		/// </summary>
		public const int IcSigPs2CRD1Tag = 0x70736431; // 'psd1'

		/// <summary>
		/// ICC Profile Tag Signature: 'psd2'.
		/// </summary>
		public const int IcSigPs2CRD2Tag = 0x70736432; // 'psd2'

		/// <summary>
		/// ICC Profile Tag Signature: 'psd3'.
		/// </summary>
		public const int IcSigPs2CRD3Tag = 0x70736433; // 'psd3'

		/// <summary>
		/// ICC Profile Tag Signature: 'ps2s'.
		/// </summary>
		public const int IcSigPs2CSATag = 0x70733273; // 'ps2s'

		/// <summary>
		/// ICC Profile Tag Signature: 'ps2i'.
		/// </summary>
		public const int IcSigPs2RenderingIntentTag = 0x70733269;
																	   /* 'ps2i' */

		/// <summary>
		/// ICC Profile Tag Signature: 'rXYZ'.
		/// </summary>
		public const int IcSigRedColorantTag = 0x7258595A; // 'rXYZ'

		/// <summary>
		/// ICC Profile Tag Signature: 'rXYZ'.
		/// @since 1.5
		/// </summary>
		public const int IcSigRedMatrixColumnTag = 0x7258595A; // 'rXYZ'

		/// <summary>
		/// ICC Profile Tag Signature: 'rTRC'.
		/// </summary>
		public const int IcSigRedTRCTag = 0x72545243; // 'rTRC'

		/// <summary>
		/// ICC Profile Tag Signature: 'scrd'.
		/// </summary>
		public const int IcSigScreeningDescTag = 0x73637264; // 'scrd'

		/// <summary>
		/// ICC Profile Tag Signature: 'scrn'.
		/// </summary>
		public const int IcSigScreeningTag = 0x7363726E; // 'scrn'

		/// <summary>
		/// ICC Profile Tag Signature: 'tech'.
		/// </summary>
		public const int IcSigTechnologyTag = 0x74656368; // 'tech'

		/// <summary>
		/// ICC Profile Tag Signature: 'bfd '.
		/// </summary>
		public const int IcSigUcrBgTag = 0x62666420; // 'bfd '

		/// <summary>
		/// ICC Profile Tag Signature: 'vued'.
		/// </summary>
		public const int IcSigViewingCondDescTag = 0x76756564; // 'vued'

		/// <summary>
		/// ICC Profile Tag Signature: 'view'.
		/// </summary>
		public const int IcSigViewingConditionsTag = 0x76696577; // 'view'

		/// <summary>
		/// ICC Profile Tag Signature: 'chrm'.
		/// </summary>
		public const int IcSigChromaticityTag = 0x6368726d; // 'chrm'

		/// <summary>
		/// ICC Profile Tag Signature: 'chad'.
		/// @since 1.5
		/// </summary>
		public const int IcSigChromaticAdaptationTag = 0x63686164; // 'chad'

		/// <summary>
		/// ICC Profile Tag Signature: 'clro'.
		/// @since 1.5
		/// </summary>
		public const int IcSigColorantOrderTag = 0x636C726F; // 'clro'

		/// <summary>
		/// ICC Profile Tag Signature: 'clrt'.
		/// @since 1.5
		/// </summary>
		public const int IcSigColorantTableTag = 0x636C7274; // 'clrt'


		/// <summary>
		/// ICC Profile Header Location: profile size in bytes.
		/// </summary>
		public const int IcHdrSize = 0; // Profile size in bytes

		/// <summary>
		/// ICC Profile Header Location: CMM for this profile.
		/// </summary>
		public const int IcHdrCmmId = 4; // CMM for this profile

		/// <summary>
		/// ICC Profile Header Location: format version number.
		/// </summary>
		public const int IcHdrVersion = 8; // Format version number

		/// <summary>
		/// ICC Profile Header Location: type of profile.
		/// </summary>
		public const int IcHdrDeviceClass = 12; // Type of profile

		/// <summary>
		/// ICC Profile Header Location: color space of data.
		/// </summary>
		public const int IcHdrColorSpace = 16; // Color space of data

		/// <summary>
		/// ICC Profile Header Location: PCS - XYZ or Lab only.
		/// </summary>
		public const int IcHdrPcs = 20; // PCS - XYZ or Lab only

		/// <summary>
		/// ICC Profile Header Location: date profile was created.
		/// </summary>
		public const int IcHdrDate = 24; // Date profile was created

		/// <summary>
		/// ICC Profile Header Location: icMagicNumber.
		/// </summary>
		public const int IcHdrMagic = 36; // icMagicNumber

		/// <summary>
		/// ICC Profile Header Location: primary platform.
		/// </summary>
		public const int IcHdrPlatform = 40; // Primary Platform

		/// <summary>
		/// ICC Profile Header Location: various bit settings.
		/// </summary>
		public const int IcHdrFlags = 44; // Various bit settings

		/// <summary>
		/// ICC Profile Header Location: device manufacturer.
		/// </summary>
		public const int IcHdrManufacturer = 48; // Device manufacturer

		/// <summary>
		/// ICC Profile Header Location: device model number.
		/// </summary>
		public const int IcHdrModel = 52; // Device model number

		/// <summary>
		/// ICC Profile Header Location: device attributes.
		/// </summary>
		public const int IcHdrAttributes = 56; // Device attributes

		/// <summary>
		/// ICC Profile Header Location: rendering intent.
		/// </summary>
		public const int IcHdrRenderingIntent = 64; // Rendering intent

		/// <summary>
		/// ICC Profile Header Location: profile illuminant.
		/// </summary>
		public const int IcHdrIlluminant = 68; // Profile illuminant

		/// <summary>
		/// ICC Profile Header Location: profile creator.
		/// </summary>
		public const int IcHdrCreator = 80; // Profile creator

		/// <summary>
		/// ICC Profile Header Location: profile's ID.
		/// @since 1.5
		/// </summary>
		public const int IcHdrProfileID = 84; // Profile's ID


		/// <summary>
		/// ICC Profile Constant: tag type signaturE.
		/// </summary>
		public const int IcTagType = 0; // tag type signature

		/// <summary>
		/// ICC Profile Constant: reserved.
		/// </summary>
		public const int IcTagReserved = 4; // reserved

		/// <summary>
		/// ICC Profile Constant: curveType count.
		/// </summary>
		public const int IcCurveCount = 8; // curveType count

		/// <summary>
		/// ICC Profile Constant: curveType data.
		/// </summary>
		public const int IcCurveData = 12; // curveType data

		/// <summary>
		/// ICC Profile Constant: XYZNumber X.
		/// </summary>
		public const int IcXYZNumberX = 8; // XYZNumber X


		/// <summary>
		/// Constructs an ICC_Profile object with a given ID.
		/// </summary>
		internal ICC_Profile(Profile p)
		{
			this.CmmProfile = p;
		}


		/// <summary>
		/// Constructs an ICC_Profile object whose loading will be deferred.
		/// The ID will be 0 until the profile is loaded.
		/// </summary>
		internal ICC_Profile(ProfileDeferralInfo pdi)
		{
			this.DeferralInfo = pdi;
			this.ProfileActivator = new ProfileActivatorAnonymousInnerClassHelper(this);
			ProfileDeferralMgr.registerDeferral(this.ProfileActivator);
		}

		private class ProfileActivatorAnonymousInnerClassHelper : ProfileActivator
		{
			private readonly ICC_Profile OuterInstance;

			public ProfileActivatorAnonymousInnerClassHelper(ICC_Profile outerInstance)
			{
				this.OuterInstance = outerInstance;
			}

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public void activate() throws ProfileDataException
			public virtual void Activate()
			{
				outerInstance.ActivateDeferredProfile();
			}
		}


		/// <summary>
		/// Frees the resources associated with an ICC_Profile object.
		/// </summary>
		~ICC_Profile()
		{
			if (CmmProfile != null)
			{
				CMSManager.Module.freeProfile(CmmProfile);
			}
			else if (ProfileActivator != null)
			{
				ProfileDeferralMgr.unregisterDeferral(ProfileActivator);
			}
		}


		/// <summary>
		/// Constructs an ICC_Profile object corresponding to the data in
		/// a byte array.  Throws an IllegalArgumentException if the data
		/// does not correspond to a valid ICC Profile. </summary>
		/// <param name="data"> the specified ICC Profile data </param>
		/// <returns> an <code>ICC_Profile</code> object corresponding to
		///          the data in the specified <code>data</code> array. </returns>
		public static ICC_Profile GetInstance(sbyte[] data)
		{
		ICC_Profile thisProfile;

			Profile p = null;

			if (ProfileDeferralMgr.deferring)
			{
				ProfileDeferralMgr.activateProfiles();
			}

			ProfileDataVerifier.verify(data);

			try
			{
				p = CMSManager.Module.loadProfile(data);
			}
			catch (CMMException)
			{
				throw new IllegalArgumentException("Invalid ICC Profile Data");
			}

			try
			{
				if ((GetColorSpaceType(p) == ColorSpace.TYPE_GRAY) && (GetData(p, IcSigMediaWhitePointTag) != null) && (GetData(p, IcSigGrayTRCTag) != null))
				{
					thisProfile = new ICC_ProfileGray(p);
				}
				else if ((GetColorSpaceType(p) == ColorSpace.TYPE_RGB) && (GetData(p, IcSigMediaWhitePointTag) != null) && (GetData(p, IcSigRedColorantTag) != null) && (GetData(p, IcSigGreenColorantTag) != null) && (GetData(p, IcSigBlueColorantTag) != null) && (GetData(p, IcSigRedTRCTag) != null) && (GetData(p, IcSigGreenTRCTag) != null) && (GetData(p, IcSigBlueTRCTag) != null))
				{
					thisProfile = new ICC_ProfileRGB(p);
				}
				else
				{
					thisProfile = new ICC_Profile(p);
				}
			}
			catch (CMMException)
			{
				thisProfile = new ICC_Profile(p);
			}
			return thisProfile;
		}



		/// <summary>
		/// Constructs an ICC_Profile corresponding to one of the specific color
		/// spaces defined by the ColorSpace class (for example CS_sRGB).
		/// Throws an IllegalArgumentException if cspace is not one of the
		/// defined color spaces.
		/// </summary>
		/// <param name="cspace"> the type of color space to create a profile for.
		/// The specified type is one of the color
		/// space constants defined in the  <CODE>ColorSpace</CODE> class.
		/// </param>
		/// <returns> an <code>ICC_Profile</code> object corresponding to
		///          the specified <code>ColorSpace</code> type. </returns>
		/// <exception cref="IllegalArgumentException"> If <CODE>cspace</CODE> is not
		/// one of the predefined color space types. </exception>
		public static ICC_Profile GetInstance(int cspace)
		{
			ICC_Profile thisProfile = null;
			String fileName;

			switch (cspace)
			{
			case ColorSpace.CS_sRGB_Renamed:
				lock (typeof(ICC_Profile))
				{
					if (SRGBprofile == null)
					{
						/*
						 * Deferral is only used for standard profiles.
						 * Enabling the appropriate access privileges is handled
						 * at a lower level.
						 */
						ProfileDeferralInfo pInfo = new ProfileDeferralInfo("sRGB.pf", ColorSpace.TYPE_RGB, 3, CLASS_DISPLAY);
						SRGBprofile = GetDeferredInstance(pInfo);
					}
					thisProfile = SRGBprofile;
				}

				break;

			case ColorSpace.CS_CIEXYZ:
				lock (typeof(ICC_Profile))
				{
					if (XYZprofile == null)
					{
						ProfileDeferralInfo pInfo = new ProfileDeferralInfo("CIEXYZ.pf", ColorSpace.TYPE_XYZ, 3, CLASS_DISPLAY);
						XYZprofile = GetDeferredInstance(pInfo);
					}
					thisProfile = XYZprofile;
				}

				break;

			case ColorSpace.CS_PYCC:
				lock (typeof(ICC_Profile))
				{
					if (PYCCprofile == null)
					{
						if (StandardProfileExists("PYCC.pf"))
						{
							ProfileDeferralInfo pInfo = new ProfileDeferralInfo("PYCC.pf", ColorSpace.TYPE_3CLR, 3, CLASS_DISPLAY);
							PYCCprofile = GetDeferredInstance(pInfo);
						}
						else
						{
							throw new IllegalArgumentException("Can't load standard profile: PYCC.pf");
						}
					}
					thisProfile = PYCCprofile;
				}

				break;

			case ColorSpace.CS_GRAY:
				lock (typeof(ICC_Profile))
				{
					if (GRAYprofile == null)
					{
						ProfileDeferralInfo pInfo = new ProfileDeferralInfo("GRAY.pf", ColorSpace.TYPE_GRAY, 1, CLASS_DISPLAY);
						GRAYprofile = GetDeferredInstance(pInfo);
					}
					thisProfile = GRAYprofile;
				}

				break;

			case ColorSpace.CS_LINEAR_RGB:
				lock (typeof(ICC_Profile))
				{
					if (LINEAR_RGBprofile == null)
					{
						ProfileDeferralInfo pInfo = new ProfileDeferralInfo("LINEAR_RGB.pf", ColorSpace.TYPE_RGB, 3, CLASS_DISPLAY);
						LINEAR_RGBprofile = GetDeferredInstance(pInfo);
					}
					thisProfile = LINEAR_RGBprofile;
				}

				break;

			default:
				throw new IllegalArgumentException("Unknown color space");
			}

			return thisProfile;
		}

		/* This asserts system privileges, so is used only for the
		 * standard profiles.
		 */
//JAVA TO C# CONVERTER WARNING: 'final' parameters are not allowed in .NET:
//ORIGINAL LINE: private static ICC_Profile getStandardProfile(final String name)
		private static ICC_Profile GetStandardProfile(String name)
		{

			return AccessController.doPrivileged(new PrivilegedActionAnonymousInnerClassHelper(name));
		}

		private class PrivilegedActionAnonymousInnerClassHelper : PrivilegedAction<ICC_Profile>
		{
			private string Name;

			public PrivilegedActionAnonymousInnerClassHelper(string name)
			{
				this.Name = name;
			}

			public virtual ICC_Profile Run()
			{
				ICC_Profile p = null;
				try
				{
					p = GetInstance(Name);
				}
				catch (IOException)
				{
					throw new IllegalArgumentException("Can't load standard profile: " + Name);
				}
				return p;
			}
		}

		/// <summary>
		/// Constructs an ICC_Profile corresponding to the data in a file.
		/// fileName may be an absolute or a relative file specification.
		/// Relative file names are looked for in several places: first, relative
		/// to any directories specified by the java.iccprofile.path property;
		/// second, relative to any directories specified by the java.class.path
		/// property; finally, in a directory used to store profiles always
		/// available, such as the profile for sRGB.  Built-in profiles use .pf as
		/// the file name extension for profiles, e.g. sRGB.pf.
		/// This method throws an IOException if the specified file cannot be
		/// opened or if an I/O error occurs while reading the file.  It throws
		/// an IllegalArgumentException if the file does not contain valid ICC
		/// Profile data. </summary>
		/// <param name="fileName"> The file that contains the data for the profile.
		/// </param>
		/// <returns> an <code>ICC_Profile</code> object corresponding to
		///          the data in the specified file. </returns>
		/// <exception cref="IOException"> If the specified file cannot be opened or
		/// an I/O error occurs while reading the file.
		/// </exception>
		/// <exception cref="IllegalArgumentException"> If the file does not
		/// contain valid ICC Profile data.
		/// </exception>
		/// <exception cref="SecurityException"> If a security manager is installed
		/// and it does not permit read access to the given file. </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public static ICC_Profile getInstance(String fileName) throws java.io.IOException
		public static ICC_Profile GetInstance(String fileName)
		{
			ICC_Profile thisProfile;
			FileInputStream fis = null;


			File f = GetProfileFile(fileName);
			if (f != null)
			{
				fis = new FileInputStream(f);
			}
			if (fis == null)
			{
				throw new IOException("Cannot open file " + fileName);
			}

			thisProfile = GetInstance(fis);

			fis.Close(); // close the file

			return thisProfile;
		}


		/// <summary>
		/// Constructs an ICC_Profile corresponding to the data in an InputStream.
		/// This method throws an IllegalArgumentException if the stream does not
		/// contain valid ICC Profile data.  It throws an IOException if an I/O
		/// error occurs while reading the stream. </summary>
		/// <param name="s"> The input stream from which to read the profile data.
		/// </param>
		/// <returns> an <CODE>ICC_Profile</CODE> object corresponding to the
		///     data in the specified <code>InputStream</code>.
		/// </returns>
		/// <exception cref="IOException"> If an I/O error occurs while reading the stream.
		/// </exception>
		/// <exception cref="IllegalArgumentException"> If the stream does not
		/// contain valid ICC Profile data. </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public static ICC_Profile getInstance(java.io.InputStream s) throws java.io.IOException
		public static ICC_Profile GetInstance(InputStream s)
		{
		sbyte[] profileData;

			if (s is ProfileDeferralInfo)
			{
				/* hack to detect profiles whose loading can be deferred */
				return GetDeferredInstance((ProfileDeferralInfo) s);
			}

			if ((profileData = GetProfileDataFromStream(s)) == null)
			{
				throw new IllegalArgumentException("Invalid ICC Profile Data");
			}

			return GetInstance(profileData);
		}


//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: static byte[] getProfileDataFromStream(java.io.InputStream s) throws java.io.IOException
		internal static sbyte[] GetProfileDataFromStream(InputStream s)
		{
		sbyte[] profileData;
		int profileSize;

			sbyte[] header = new sbyte[128];
			int bytestoread = 128;
			int bytesread = 0;
			int n;

			while (bytestoread != 0)
			{
				if ((n = s.Read(header, bytesread, bytestoread)) < 0)
				{
					return null;
				}
				bytesread += n;
				bytestoread -= n;
			}
			if (header[36] != 0x61 || header[37] != 0x63 || header[38] != 0x73 || header[39] != 0x70)
			{
				return null; // not a valid profile
			}
			profileSize = ((header[0] & 0xff) << 24) | ((header[1] & 0xff) << 16) | ((header[2] & 0xff) << 8) | (header[3] & 0xff);
			profileData = new sbyte[profileSize];
			System.Array.Copy(header, 0, profileData, 0, 128);
			bytestoread = profileSize - 128;
			bytesread = 128;
			while (bytestoread != 0)
			{
				if ((n = s.Read(profileData, bytesread, bytestoread)) < 0)
				{
					return null;
				}
				bytesread += n;
				bytestoread -= n;
			}

			return profileData;
		}


		/// <summary>
		/// Constructs an ICC_Profile for which the actual loading of the
		/// profile data from a file and the initialization of the CMM should
		/// be deferred as long as possible.
		/// Deferral is only used for standard profiles.
		/// If deferring is disabled, then getStandardProfile() ensures
		/// that all of the appropriate access privileges are granted
		/// when loading this profile.
		/// If deferring is enabled, then the deferred activation
		/// code will take care of access privileges. </summary>
		/// <seealso cref= activateDeferredProfile() </seealso>
		internal static ICC_Profile GetDeferredInstance(ProfileDeferralInfo pdi)
		{
			if (!ProfileDeferralMgr.deferring)
			{
				return GetStandardProfile(pdi.filename);
			}
			if (pdi.colorSpaceType == ColorSpace.TYPE_RGB)
			{
				return new ICC_ProfileRGB(pdi);
			}
			else if (pdi.colorSpaceType == ColorSpace.TYPE_GRAY)
			{
				return new ICC_ProfileGray(pdi);
			}
			else
			{
				return new ICC_Profile(pdi);
			}
		}


//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: void activateDeferredProfile() throws ProfileDataException
		internal virtual void ActivateDeferredProfile()
		{
			sbyte[] profileData;
			FileInputStream fis;
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final String fileName = deferralInfo.filename;
			String fileName = DeferralInfo.filename;

			ProfileActivator = null;
			DeferralInfo = null;
			PrivilegedAction<FileInputStream> pa = new PrivilegedActionAnonymousInnerClassHelper2(this, fileName);
			if ((fis = AccessController.doPrivileged(pa)) == null)
			{
				throw new ProfileDataException("Cannot open file " + fileName);
			}
			try
			{
				profileData = GetProfileDataFromStream(fis);
				fis.Close(); // close the file
			}
			catch (IOException e)
			{
				ProfileDataException pde = new ProfileDataException("Invalid ICC Profile Data" + fileName);
				pde.InitCause(e);
				throw pde;
			}
			if (profileData == null)
			{
				throw new ProfileDataException("Invalid ICC Profile Data" + fileName);
			}
			try
			{
				CmmProfile = CMSManager.Module.loadProfile(profileData);
			}
			catch (CMMException c)
			{
				ProfileDataException pde = new ProfileDataException("Invalid ICC Profile Data" + fileName);
				pde.InitCause(c);
				throw pde;
			}
		}

		private class PrivilegedActionAnonymousInnerClassHelper2 : PrivilegedAction<FileInputStream>
		{
			private readonly ICC_Profile OuterInstance;

			private string FileName;

			public PrivilegedActionAnonymousInnerClassHelper2(ICC_Profile outerInstance, string fileName)
			{
				this.OuterInstance = outerInstance;
				this.FileName = fileName;
			}

			public virtual FileInputStream Run()
			{
				File f = GetStandardProfileFile(FileName);
				if (f != null)
				{
					try
					{
						return new FileInputStream(f);
					}
					catch (FileNotFoundException)
					{
					}
				}
				return null;
			}
		}


		/// <summary>
		/// Returns profile major version. </summary>
		/// <returns>  The major version of the profile. </returns>
		public virtual int MajorVersion
		{
			get
			{
			sbyte[] theHeader;
    
				theHeader = GetData(IcSigHead); /* getData will activate deferred
		                                           profiles if necessary */
    
				return (int) theHeader[8];
			}
		}

		/// <summary>
		/// Returns profile minor version. </summary>
		/// <returns> The minor version of the profile. </returns>
		public virtual int MinorVersion
		{
			get
			{
			sbyte[] theHeader;
    
				theHeader = GetData(IcSigHead); /* getData will activate deferred
		                                           profiles if necessary */
    
				return (int) theHeader[9];
			}
		}

		/// <summary>
		/// Returns the profile class. </summary>
		/// <returns> One of the predefined profile class constants. </returns>
		public virtual int ProfileClass
		{
			get
			{
			sbyte[] theHeader;
			int theClassSig, theClass;
    
				if (DeferralInfo != null)
				{
					return DeferralInfo.profileClass; /* Need to have this info for
		                                                 ICC_ColorSpace without
		                                                 causing a deferred profile
		                                                 to be loaded */
				}
    
				theHeader = GetData(IcSigHead);
    
				theClassSig = IntFromBigEndian(theHeader, IcHdrDeviceClass);
    
				switch (theClassSig)
				{
				case IcSigInputClass:
					theClass = CLASS_INPUT;
					break;
    
				case IcSigDisplayClass:
					theClass = CLASS_DISPLAY;
					break;
    
				case IcSigOutputClass:
					theClass = CLASS_OUTPUT;
					break;
    
				case IcSigLinkClass:
					theClass = CLASS_DEVICELINK;
					break;
    
				case IcSigColorSpaceClass:
					theClass = CLASS_COLORSPACECONVERSION;
					break;
    
				case IcSigAbstractClass:
					theClass = CLASS_ABSTRACT;
					break;
    
				case IcSigNamedColorClass:
					theClass = CLASS_NAMEDCOLOR;
					break;
    
				default:
					throw new IllegalArgumentException("Unknown profile class");
				}
    
				return theClass;
			}
		}

		/// <summary>
		/// Returns the color space type.  Returns one of the color space type
		/// constants defined by the ColorSpace class.  This is the
		/// "input" color space of the profile.  The type defines the
		/// number of components of the color space and the interpretation,
		/// e.g. TYPE_RGB identifies a color space with three components - red,
		/// green, and blue.  It does not define the particular color
		/// characteristics of the space, e.g. the chromaticities of the
		/// primaries. </summary>
		/// <returns> One of the color space type constants defined in the
		/// <CODE>ColorSpace</CODE> class. </returns>
		public virtual int ColorSpaceType
		{
			get
			{
				if (DeferralInfo != null)
				{
					return DeferralInfo.colorSpaceType; /* Need to have this info for
		                                                   ICC_ColorSpace without
		                                                   causing a deferred profile
		                                                   to be loaded */
				}
				return GetColorSpaceType(CmmProfile);
			}
		}

		internal static int GetColorSpaceType(Profile p)
		{
		sbyte[] theHeader;
		int theColorSpaceSig, theColorSpace;

			theHeader = GetData(p, IcSigHead);
			theColorSpaceSig = IntFromBigEndian(theHeader, IcHdrColorSpace);
			theColorSpace = IccCStoJCS(theColorSpaceSig);
			return theColorSpace;
		}

		/// <summary>
		/// Returns the color space type of the Profile Connection Space (PCS).
		/// Returns one of the color space type constants defined by the
		/// ColorSpace class.  This is the "output" color space of the
		/// profile.  For an input, display, or output profile useful
		/// for tagging colors or images, this will be either TYPE_XYZ or
		/// TYPE_Lab and should be interpreted as the corresponding specific
		/// color space defined in the ICC specification.  For a device
		/// link profile, this could be any of the color space type constants. </summary>
		/// <returns> One of the color space type constants defined in the
		/// <CODE>ColorSpace</CODE> class. </returns>
		public virtual int PCSType
		{
			get
			{
				if (ProfileDeferralMgr.deferring)
				{
					ProfileDeferralMgr.activateProfiles();
				}
				return GetPCSType(CmmProfile);
			}
		}


		internal static int GetPCSType(Profile p)
		{
		sbyte[] theHeader;
		int thePCSSig, thePCS;

			theHeader = GetData(p, IcSigHead);
			thePCSSig = IntFromBigEndian(theHeader, IcHdrPcs);
			thePCS = IccCStoJCS(thePCSSig);
			return thePCS;
		}


		/// <summary>
		/// Write this ICC_Profile to a file.
		/// </summary>
		/// <param name="fileName"> The file to write the profile data to.
		/// </param>
		/// <exception cref="IOException"> If the file cannot be opened for writing
		/// or an I/O error occurs while writing to the file. </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public void write(String fileName) throws java.io.IOException
		public virtual void Write(String fileName)
		{
		FileOutputStream outputFile;
		sbyte[] profileData;

			profileData = Data; /* this will activate deferred
	                                    profiles if necessary */
			outputFile = new FileOutputStream(fileName);
			outputFile.Write(profileData);
			outputFile.Close();
		}


		/// <summary>
		/// Write this ICC_Profile to an OutputStream.
		/// </summary>
		/// <param name="s"> The stream to write the profile data to.
		/// </param>
		/// <exception cref="IOException"> If an I/O error occurs while writing to the
		/// stream. </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public void write(java.io.OutputStream s) throws java.io.IOException
		public virtual void Write(OutputStream s)
		{
		sbyte[] profileData;

			profileData = Data; /* this will activate deferred
	                                    profiles if necessary */
			s.Write(profileData);
		}


		/// <summary>
		/// Returns a byte array corresponding to the data of this ICC_Profile. </summary>
		/// <returns> A byte array that contains the profile data. </returns>
		/// <seealso cref= #setData(int, byte[]) </seealso>
		public virtual sbyte[] Data
		{
			get
			{
			int profileSize;
			sbyte[] profileData;
    
				if (ProfileDeferralMgr.deferring)
				{
					ProfileDeferralMgr.activateProfiles();
				}
    
				PCMM mdl = CMSManager.Module;
    
				/* get the number of bytes needed for this profile */
				profileSize = mdl.getProfileSize(CmmProfile);
    
				profileData = new sbyte [profileSize];
    
				/* get the data for the profile */
				mdl.getProfileData(CmmProfile, profileData);
    
				return profileData;
			}
		}


		/// <summary>
		/// Returns a particular tagged data element from the profile as
		/// a byte array.  Elements are identified by signatures
		/// as defined in the ICC specification.  The signature
		/// icSigHead can be used to get the header.  This method is useful
		/// for advanced applets or applications which need to access
		/// profile data directly.
		/// </summary>
		/// <param name="tagSignature"> The ICC tag signature for the data element you
		/// want to get.
		/// </param>
		/// <returns> A byte array that contains the tagged data element. Returns
		/// <code>null</code> if the specified tag doesn't exist. </returns>
		/// <seealso cref= #setData(int, byte[]) </seealso>
		public virtual sbyte[] GetData(int tagSignature)
		{

			if (ProfileDeferralMgr.deferring)
			{
				ProfileDeferralMgr.activateProfiles();
			}

			return GetData(CmmProfile, tagSignature);
		}


		internal static sbyte[] GetData(Profile p, int tagSignature)
		{
		int tagSize;
		sbyte[] tagData;

			try
			{
				PCMM mdl = CMSManager.Module;

				/* get the number of bytes needed for this tag */
				tagSize = mdl.getTagSize(p, tagSignature);

				tagData = new sbyte[tagSize]; // get an array for the tag

				/* get the tag's data */
				mdl.getTagData(p, tagSignature, tagData);
			}
			catch (CMMException)
			{
				tagData = null;
			}

			return tagData;
		}

		/// <summary>
		/// Sets a particular tagged data element in the profile from
		/// a byte array. The array should contain data in a format, corresponded
		/// to the {@code tagSignature} as defined in the ICC specification, section 10.
		/// This method is useful for advanced applets or applications which need to
		/// access profile data directly.
		/// </summary>
		/// <param name="tagSignature"> The ICC tag signature for the data element
		/// you want to set. </param>
		/// <param name="tagData"> the data to set for the specified tag signature </param>
		/// <exception cref="IllegalArgumentException"> if {@code tagSignature} is not a signature
		///         as defined in the ICC specification. </exception>
		/// <exception cref="IllegalArgumentException"> if a content of the {@code tagData}
		///         array can not be interpreted as valid tag data, corresponding
		///         to the {@code tagSignature}. </exception>
		/// <seealso cref= #getData </seealso>
		public virtual void SetData(int tagSignature, sbyte[] tagData)
		{

			if (ProfileDeferralMgr.deferring)
			{
				ProfileDeferralMgr.activateProfiles();
			}

			CMSManager.Module.setTagData(CmmProfile, tagSignature, tagData);
		}

		/// <summary>
		/// Sets the rendering intent of the profile.
		/// This is used to select the proper transform from a profile that
		/// has multiple transforms.
		/// </summary>
		internal virtual int RenderingIntent
		{
			set
			{
				sbyte[] theHeader = GetData(IcSigHead); /* getData will activate deferred
		                                                 profiles if necessary */
				IntToBigEndian(value, theHeader, IcHdrRenderingIntent);
														 /* set the rendering intent */
				SetData(IcSigHead, theHeader);
			}
			get
			{
				sbyte[] theHeader = GetData(IcSigHead); /* getData will activate deferred
		                                                 profiles if necessary */
    
				int renderingIntent = IntFromBigEndian(theHeader, IcHdrRenderingIntent);
														 /* set the rendering intent */
    
				/* According to ICC spec, only the least-significant 16 bits shall be
				 * used to encode the rendering intent. The most significant 16 bits
				 * shall be set to zero. Thus, we are ignoring two most significant
				 * bytes here.
				 *
				 *  See http://www.color.org/ICC1v42_2006-05.pdf, section 7.2.15.
				 */
				return (0xffff & renderingIntent);
			}
		}




		/// <summary>
		/// Returns the number of color components in the "input" color
		/// space of this profile.  For example if the color space type
		/// of this profile is TYPE_RGB, then this method will return 3.
		/// </summary>
		/// <returns> The number of color components in the profile's input
		/// color space.
		/// </returns>
		/// <exception cref="ProfileDataException"> if color space is in the profile
		///         is invalid </exception>
		public virtual int NumComponents
		{
			get
			{
			sbyte[] theHeader;
			int theColorSpaceSig, theNumComponents;
    
				if (DeferralInfo != null)
				{
					return DeferralInfo.numComponents; /* Need to have this info for
		                                                  ICC_ColorSpace without
		                                                  causing a deferred profile
		                                                  to be loaded */
				}
				theHeader = GetData(IcSigHead);
    
				theColorSpaceSig = IntFromBigEndian(theHeader, IcHdrColorSpace);
    
				switch (theColorSpaceSig)
				{
				case IcSigGrayData:
					theNumComponents = 1;
					break;
    
				case IcSigSpace2CLR:
					theNumComponents = 2;
					break;
    
				case IcSigXYZData:
				case IcSigLabData:
				case IcSigLuvData:
				case IcSigYCbCrData:
				case IcSigYxyData:
				case IcSigRgbData:
				case IcSigHsvData:
				case IcSigHlsData:
				case IcSigCmyData:
				case IcSigSpace3CLR:
					theNumComponents = 3;
					break;
    
				case IcSigCmykData:
				case IcSigSpace4CLR:
					theNumComponents = 4;
					break;
    
				case IcSigSpace5CLR:
					theNumComponents = 5;
					break;
    
				case IcSigSpace6CLR:
					theNumComponents = 6;
					break;
    
				case IcSigSpace7CLR:
					theNumComponents = 7;
					break;
    
				case IcSigSpace8CLR:
					theNumComponents = 8;
					break;
    
				case IcSigSpace9CLR:
					theNumComponents = 9;
					break;
    
				case IcSigSpaceACLR:
					theNumComponents = 10;
					break;
    
				case IcSigSpaceBCLR:
					theNumComponents = 11;
					break;
    
				case IcSigSpaceCCLR:
					theNumComponents = 12;
					break;
    
				case IcSigSpaceDCLR:
					theNumComponents = 13;
					break;
    
				case IcSigSpaceECLR:
					theNumComponents = 14;
					break;
    
				case IcSigSpaceFCLR:
					theNumComponents = 15;
					break;
    
				default:
					throw new ProfileDataException("invalid ICC color space");
				}
    
				return theNumComponents;
			}
		}


		/// <summary>
		/// Returns a float array of length 3 containing the X, Y, and Z
		/// components of the mediaWhitePointTag in the ICC profile.
		/// </summary>
		internal virtual float[] MediaWhitePoint
		{
			get
			{
				return GetXYZTag(IcSigMediaWhitePointTag);
												   /* get the media white point tag */
			}
		}


		/// <summary>
		/// Returns a float array of length 3 containing the X, Y, and Z
		/// components encoded in an XYZType tag.
		/// </summary>
		internal virtual float[] GetXYZTag(int theTagSignature)
		{
		sbyte[] theData;
		float[] theXYZNumber;
		int i1, i2, theS15Fixed16;

			theData = GetData(theTagSignature); // get the tag data
												/* getData will activate deferred
												   profiles if necessary */

			theXYZNumber = new float [3]; // array to return

			/* convert s15Fixed16Number to float */
			for (i1 = 0, i2 = IcXYZNumberX; i1 < 3; i1++, i2 += 4)
			{
				theS15Fixed16 = IntFromBigEndian(theData, i2);
				theXYZNumber [i1] = ((float) theS15Fixed16) / 65536.0f;
			}
			return theXYZNumber;
		}


		/// <summary>
		/// Returns a gamma value representing a tone reproduction
		/// curve (TRC).  If the profile represents the TRC as a table rather
		/// than a single gamma value, then an exception is thrown.  In this
		/// case the actual table can be obtained via getTRC().
		/// theTagSignature should be one of icSigGrayTRCTag, icSigRedTRCTag,
		/// icSigGreenTRCTag, or icSigBlueTRCTag. </summary>
		/// <returns> the gamma value as a float. </returns>
		/// <exception cref="ProfileDataException"> if the profile does not specify
		///            the TRC as a single gamma value. </exception>
		internal virtual float GetGamma(int theTagSignature)
		{
		sbyte[] theTRCData;
		float theGamma;
		int theU8Fixed8;

			theTRCData = GetData(theTagSignature); // get the TRC
												   /* getData will activate deferred
												      profiles if necessary */

			if (IntFromBigEndian(theTRCData, IcCurveCount) != 1)
			{
				throw new ProfileDataException("TRC is not a gamma");
			}

			/* convert u8Fixed8 to float */
			theU8Fixed8 = (ShortFromBigEndian(theTRCData, IcCurveData)) & 0xffff;

			theGamma = ((float) theU8Fixed8) / 256.0f;

			return theGamma;
		}


		/// <summary>
		/// Returns the TRC as an array of shorts.  If the profile has
		/// specified the TRC as linear (gamma = 1.0) or as a simple gamma
		/// value, this method throws an exception, and the getGamma() method
		/// should be used to get the gamma value.  Otherwise the short array
		/// returned here represents a lookup table where the input Gray value
		/// is conceptually in the range [0.0, 1.0].  Value 0.0 maps
		/// to array index 0 and value 1.0 maps to array index length-1.
		/// Interpolation may be used to generate output values for
		/// input values which do not map exactly to an index in the
		/// array.  Output values also map linearly to the range [0.0, 1.0].
		/// Value 0.0 is represented by an array value of 0x0000 and
		/// value 1.0 by 0xFFFF, i.e. the values are really unsigned
		/// short values, although they are returned in a short array.
		/// theTagSignature should be one of icSigGrayTRCTag, icSigRedTRCTag,
		/// icSigGreenTRCTag, or icSigBlueTRCTag. </summary>
		/// <returns> a short array representing the TRC. </returns>
		/// <exception cref="ProfileDataException"> if the profile does not specify
		///            the TRC as a table. </exception>
		internal virtual short[] GetTRC(int theTagSignature)
		{
		sbyte[] theTRCData;
		short[] theTRC;
		int i1, i2, nElements, theU8Fixed8;

			theTRCData = GetData(theTagSignature); // get the TRC
												   /* getData will activate deferred
												      profiles if necessary */

			nElements = IntFromBigEndian(theTRCData, IcCurveCount);

			if (nElements == 1)
			{
				throw new ProfileDataException("TRC is not a table");
			}

			/* make the short array */
			theTRC = new short [nElements];

			for (i1 = 0, i2 = IcCurveData; i1 < nElements; i1++, i2 += 2)
			{
				theTRC[i1] = ShortFromBigEndian(theTRCData, i2);
			}

			return theTRC;
		}


		/* convert an ICC color space signature into a Java color space type */
		internal static int IccCStoJCS(int theColorSpaceSig)
		{
		int theColorSpace;

			switch (theColorSpaceSig)
			{
			case IcSigXYZData:
				theColorSpace = ColorSpace.TYPE_XYZ;
				break;

			case IcSigLabData:
				theColorSpace = ColorSpace.TYPE_Lab;
				break;

			case IcSigLuvData:
				theColorSpace = ColorSpace.TYPE_Luv;
				break;

			case IcSigYCbCrData:
				theColorSpace = ColorSpace.TYPE_YCbCr;
				break;

			case IcSigYxyData:
				theColorSpace = ColorSpace.TYPE_Yxy;
				break;

			case IcSigRgbData:
				theColorSpace = ColorSpace.TYPE_RGB;
				break;

			case IcSigGrayData:
				theColorSpace = ColorSpace.TYPE_GRAY;
				break;

			case IcSigHsvData:
				theColorSpace = ColorSpace.TYPE_HSV;
				break;

			case IcSigHlsData:
				theColorSpace = ColorSpace.TYPE_HLS;
				break;

			case IcSigCmykData:
				theColorSpace = ColorSpace.TYPE_CMYK;
				break;

			case IcSigCmyData:
				theColorSpace = ColorSpace.TYPE_CMY;
				break;

			case IcSigSpace2CLR:
				theColorSpace = ColorSpace.TYPE_2CLR;
				break;

			case IcSigSpace3CLR:
				theColorSpace = ColorSpace.TYPE_3CLR;
				break;

			case IcSigSpace4CLR:
				theColorSpace = ColorSpace.TYPE_4CLR;
				break;

			case IcSigSpace5CLR:
				theColorSpace = ColorSpace.TYPE_5CLR;
				break;

			case IcSigSpace6CLR:
				theColorSpace = ColorSpace.TYPE_6CLR;
				break;

			case IcSigSpace7CLR:
				theColorSpace = ColorSpace.TYPE_7CLR;
				break;

			case IcSigSpace8CLR:
				theColorSpace = ColorSpace.TYPE_8CLR;
				break;

			case IcSigSpace9CLR:
				theColorSpace = ColorSpace.TYPE_9CLR;
				break;

			case IcSigSpaceACLR:
				theColorSpace = ColorSpace.TYPE_ACLR;
				break;

			case IcSigSpaceBCLR:
				theColorSpace = ColorSpace.TYPE_BCLR;
				break;

			case IcSigSpaceCCLR:
				theColorSpace = ColorSpace.TYPE_CCLR;
				break;

			case IcSigSpaceDCLR:
				theColorSpace = ColorSpace.TYPE_DCLR;
				break;

			case IcSigSpaceECLR:
				theColorSpace = ColorSpace.TYPE_ECLR;
				break;

			case IcSigSpaceFCLR:
				theColorSpace = ColorSpace.TYPE_FCLR;
				break;

			default:
				throw new IllegalArgumentException("Unknown color space");
			}

			return theColorSpace;
		}


		internal static int IntFromBigEndian(sbyte[] array, int index)
		{
			return (((array[index] & 0xff) << 24) | ((array[index + 1] & 0xff) << 16) | ((array[index + 2] & 0xff) << 8) | (array[index + 3] & 0xff));
		}


		internal static void IntToBigEndian(int value, sbyte[] array, int index)
		{
				array[index] = (sbyte)(value >> 24);
				array[index + 1] = (sbyte)(value >> 16);
				array[index + 2] = (sbyte)(value >> 8);
				array[index + 3] = (sbyte)(value);
		}


		internal static short ShortFromBigEndian(sbyte[] array, int index)
		{
			return (short)(((array[index] & 0xff) << 8) | (array[index + 1] & 0xff));
		}


		internal static void ShortToBigEndian(short value, sbyte[] array, int index)
		{
				array[index] = (sbyte)(value >> 8);
				array[index + 1] = (sbyte)(value);
		}


		/*
		 * fileName may be an absolute or a relative file specification.
		 * Relative file names are looked for in several places: first, relative
		 * to any directories specified by the java.iccprofile.path property;
		 * second, relative to any directories specified by the java.class.path
		 * property; finally, in a directory used to store profiles always
		 * available, such as a profile for sRGB.  Built-in profiles use .pf as
		 * the file name extension for profiles, e.g. sRGB.pf.
		 */
		private static File GetProfileFile(String fileName)
		{
			String path, dir, fullPath;

			File f = new File(fileName); // try absolute file name
			if (f.Absolute)
			{
				/* Rest of code has little sense for an absolute pathname,
				   so return here. */
				return f.File ? f : null;
			}
			if ((!f.File) && ((path = System.getProperty("java.iccprofile.path")) != null))
			{
										/* try relative to java.iccprofile.path */
					StringTokenizer st = new StringTokenizer(path, File.pathSeparator);
					while (st.HasMoreTokens() && ((f == null) || (!f.File)))
					{
						dir = st.NextToken();
							fullPath = dir + System.IO.Path.DirectorySeparatorChar + fileName;
						f = new File(fullPath);
						if (!IsChildOf(f, dir))
						{
							f = null;
						}
					}
			}

			if (((f == null) || (!f.File)) && ((path = System.getProperty("java.class.path")) != null))
			{
										/* try relative to java.class.path */
					StringTokenizer st = new StringTokenizer(path, File.pathSeparator);
					while (st.HasMoreTokens() && ((f == null) || (!f.File)))
					{
						dir = st.NextToken();
							fullPath = dir + System.IO.Path.DirectorySeparatorChar + fileName;
						f = new File(fullPath);
					}
			}

			if ((f == null) || (!f.File))
			{
				/* try the directory of built-in profiles */
				f = GetStandardProfileFile(fileName);
			}
			if (f != null && f.File)
			{
				return f;
			}
			return null;
		}

		/// <summary>
		/// Returns a file object corresponding to a built-in profile
		/// specified by fileName.
		/// If there is no built-in profile with such name, then the method
		/// returns null.
		/// </summary>
		private static File GetStandardProfileFile(String fileName)
		{
			String dir = System.getProperty("java.home") + System.IO.Path.DirectorySeparatorChar + "lib" + System.IO.Path.DirectorySeparatorChar + "cmm";
			String fullPath = dir + System.IO.Path.DirectorySeparatorChar + fileName;
			File f = new File(fullPath);
			return (f.File && IsChildOf(f, dir)) ? f : null;
		}

		/// <summary>
		/// Checks whether given file resides inside give directory.
		/// </summary>
		private static bool IsChildOf(File f, String dirName)
		{
			try
			{
				File dir = new File(dirName);
				String canonicalDirName = dir.CanonicalPath;
				if (!canonicalDirName.EndsWith(File.separator))
				{
					canonicalDirName += File.separator;
				}
				String canonicalFileName = f.CanonicalPath;
				return canonicalFileName.StartsWith(canonicalDirName);
			}
			catch (IOException)
			{
				/* we do not expect the IOException here, because invocation
				 * of this function is always preceeded by isFile() call.
				 */
				return false;
			}
		}

		/// <summary>
		/// Checks whether built-in profile specified by fileName exists.
		/// </summary>
//JAVA TO C# CONVERTER WARNING: 'final' parameters are not allowed in .NET:
//ORIGINAL LINE: private static boolean standardProfileExists(final String fileName)
		private static bool StandardProfileExists(String fileName)
		{
			return AccessController.doPrivileged(new PrivilegedActionAnonymousInnerClassHelper3(fileName));
		}

		private class PrivilegedActionAnonymousInnerClassHelper3 : PrivilegedAction<Boolean>
		{
			private string FileName;

			public PrivilegedActionAnonymousInnerClassHelper3(string fileName)
			{
				this.FileName = fileName;
			}

			public virtual Boolean Run()
			{
				return GetStandardProfileFile(FileName) != null;
			}
		}


		/*
		 * Serialization support.
		 *
		 * Directly deserialized profiles are useless since they are not
		 * registered with CMM.  We don't allow constructor to be called
		 * directly and instead have clients to call one of getInstance
		 * factory methods that will register the profile with CMM.  For
		 * deserialization we implement readResolve method that will
		 * resolve the bogus deserialized profile object with one obtained
		 * with getInstance as well.
		 *
		 * There're two primary factory methods for construction of ICC
		 * profiles: getInstance(int cspace) and getInstance(byte[] data).
		 * This implementation of ICC_Profile uses the former to return a
		 * cached singleton profile object, other implementations will
		 * likely use this technique too.  To preserve the singleton
		 * pattern across serialization we serialize cached singleton
		 * profiles in such a way that deserializing VM could call
		 * getInstance(int cspace) method that will resolve deserialized
		 * object into the corresponding singleton as well.
		 *
		 * Since the singletons are private to ICC_Profile the readResolve
		 * method have to be `protected' instead of `private' so that
		 * singletons that are instances of subclasses of ICC_Profile
		 * could be correctly deserialized.
		 */


		/// <summary>
		/// Version of the format of additional serialized data in the
		/// stream.  Version&nbsp;<code>1</code> corresponds to Java&nbsp;2
		/// Platform,&nbsp;v1.3.
		/// @since 1.3
		/// @serial
		/// </summary>
		private int IccProfileSerializedDataVersion = 1;


		/// <summary>
		/// Writes default serializable fields to the stream.  Writes a
		/// string and an array of bytes to the stream as additional data.
		/// </summary>
		/// <param name="s"> stream used for serialization. </param>
		/// <exception cref="IOException">
		///     thrown by <code>ObjectInputStream</code>.
		/// @serialData
		///     The <code>String</code> is the name of one of
		///     <code>CS_<var>*</var></code> constants defined in the
		///     <seealso cref="ColorSpace"/> class if the profile object is a profile
		///     for a predefined color space (for example
		///     <code>"CS_sRGB"</code>).  The string is <code>null</code>
		///     otherwise.
		///     <para>
		///     The <code>byte[]</code> array is the profile data for the
		///     profile.  For predefined color spaces <code>null</code> is
		///     written instead of the profile data.  If in the future
		///     versions of Java API new predefined color spaces will be
		///     added, future versions of this class may choose to write
		///     for new predefined color spaces not only the color space
		///     name, but the profile data as well so that older versions
		///     could still deserialize the object. </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: private void writeObject(java.io.ObjectOutputStream s) throws java.io.IOException
		private void WriteObject(ObjectOutputStream s)
		{
			s.DefaultWriteObject();

			String csName = null;
			if (this == SRGBprofile)
			{
				csName = "CS_sRGB";
			}
			else if (this == XYZprofile)
			{
				csName = "CS_CIEXYZ";
			}
			else if (this == PYCCprofile)
			{
				csName = "CS_PYCC";
			}
			else if (this == GRAYprofile)
			{
				csName = "CS_GRAY";
			}
			else if (this == LINEAR_RGBprofile)
			{
				csName = "CS_LINEAR_RGB";
			}

			// Future versions may choose to write profile data for new
			// predefined color spaces as well, if any will be introduced,
			// so that old versions that don't recognize the new CS name
			// may fall back to constructing profile from the data.
			sbyte[] data = null;
			if (csName == null)
			{
				// getData will activate deferred profile if necessary
				data = Data;
			}

			s.WriteObject(csName);
			s.WriteObject(data);
		}

		// Temporary storage used by readObject to store resolved profile
		// (obtained with getInstance) for readResolve to return.
		[NonSerialized]
		private ICC_Profile ResolvedDeserializedProfile;

		/// <summary>
		/// Reads default serializable fields from the stream.  Reads from
		/// the stream a string and an array of bytes as additional data.
		/// </summary>
		/// <param name="s"> stream used for deserialization. </param>
		/// <exception cref="IOException">
		///     thrown by <code>ObjectInputStream</code>. </exception>
		/// <exception cref="ClassNotFoundException">
		///     thrown by <code>ObjectInputStream</code>.
		/// @serialData
		///     The <code>String</code> is the name of one of
		///     <code>CS_<var>*</var></code> constants defined in the
		///     <seealso cref="ColorSpace"/> class if the profile object is a profile
		///     for a predefined color space (for example
		///     <code>"CS_sRGB"</code>).  The string is <code>null</code>
		///     otherwise.
		///     <para>
		///     The <code>byte[]</code> array is the profile data for the
		///     profile.  It will usually be <code>null</code> for the
		///     predefined profiles.
		/// </para>
		///     <para>
		///     If the string is recognized as a constant name for
		///     predefined color space the object will be resolved into
		///     profile obtained with
		///     <code>getInstance(int&nbsp;cspace)</code> and the profile
		///     data are ignored.  Otherwise the object will be resolved
		///     into profile obtained with
		///     <code>getInstance(byte[]&nbsp;data)</code>.
		/// </para>
		/// </exception>
		/// <seealso cref= #readResolve() </seealso>
		/// <seealso cref= #getInstance(int) </seealso>
		/// <seealso cref= #getInstance(byte[]) </seealso>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: private void readObject(java.io.ObjectInputStream s) throws java.io.IOException, ClassNotFoundException
		private void ReadObject(ObjectInputStream s)
		{
			s.DefaultReadObject();

			String csName = (String)s.ReadObject();
			sbyte[] data = (sbyte[])s.ReadObject();

			int cspace = 0; // ColorSpace.CS_* constant if known
			bool isKnownPredefinedCS = false;
			if (csName != null)
			{
				isKnownPredefinedCS = true;
				if (csName.Equals("CS_sRGB"))
				{
					cspace = ColorSpace.CS_sRGB_Renamed;
				}
				else if (csName.Equals("CS_CIEXYZ"))
				{
					cspace = ColorSpace.CS_CIEXYZ;
				}
				else if (csName.Equals("CS_PYCC"))
				{
					cspace = ColorSpace.CS_PYCC;
				}
				else if (csName.Equals("CS_GRAY"))
				{
					cspace = ColorSpace.CS_GRAY;
				}
				else if (csName.Equals("CS_LINEAR_RGB"))
				{
					cspace = ColorSpace.CS_LINEAR_RGB;
				}
				else
				{
					isKnownPredefinedCS = false;
				}
			}

			if (isKnownPredefinedCS)
			{
				ResolvedDeserializedProfile = GetInstance(cspace);
			}
			else
			{
				ResolvedDeserializedProfile = GetInstance(data);
			}
		}

		/// <summary>
		/// Resolves instances being deserialized into instances registered
		/// with CMM. </summary>
		/// <returns> ICC_Profile object for profile registered with CMM. </returns>
		/// <exception cref="ObjectStreamException">
		///     never thrown, but mandated by the serialization spec.
		/// @since 1.3 </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: protected Object readResolve() throws java.io.ObjectStreamException
		protected internal virtual Object ReadResolve()
		{
			return ResolvedDeserializedProfile;
		}
	}

}
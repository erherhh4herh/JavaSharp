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

	using Profile = sun.java2d.cmm.Profile;
	using ProfileDeferralInfo = sun.java2d.cmm.ProfileDeferralInfo;

	/// 
	/// <summary>
	/// The ICC_ProfileRGB class is a subclass of the ICC_Profile class
	/// that represents profiles which meet the following criteria:
	/// <ul>
	/// <li>The profile's color space type is RGB.</li>
	/// <li>The profile includes the <code>redColorantTag</code>,
	/// <code>greenColorantTag</code>, <code>blueColorantTag</code>,
	/// <code>redTRCTag</code>, <code>greenTRCTag</code>,
	/// <code>blueTRCTag</code>, and <code>mediaWhitePointTag</code> tags.</li>
	/// </ul>
	/// The <code>ICC_Profile</code> <code>getInstance</code> method will
	/// return an <code>ICC_ProfileRGB</code> object when these conditions are met.
	/// Three-component, matrix-based input profiles and RGB display profiles are
	/// examples of this type of profile.
	/// <para>
	/// This profile class provides color transform matrices and lookup tables
	/// that Java or native methods can use directly to
	/// optimize color conversion in some cases.
	/// </para>
	/// <para>
	/// To transform from a device profile color space to the CIEXYZ Profile
	/// Connection Space, each device color component is first linearized by
	/// a lookup through the corresponding tone reproduction curve (TRC).
	/// The resulting linear RGB components are converted to the CIEXYZ PCS
	/// using a a 3x3 matrix constructed from the RGB colorants.
	/// <pre>
	/// 
	/// &nbsp;               linearR = redTRC[deviceR]
	/// 
	/// &nbsp;               linearG = greenTRC[deviceG]
	/// 
	/// &nbsp;               linearB = blueTRC[deviceB]
	/// 
	/// &nbsp; _      _       _                                             _   _         _
	/// &nbsp;[  PCSX  ]     [  redColorantX  greenColorantX  blueColorantX  ] [  linearR  ]
	/// &nbsp;[        ]     [                                               ] [           ]
	/// &nbsp;[  PCSY  ]  =  [  redColorantY  greenColorantY  blueColorantY  ] [  linearG  ]
	/// &nbsp;[        ]     [                                               ] [           ]
	/// &nbsp;[_ PCSZ _]     [_ redColorantZ  greenColorantZ  blueColorantZ _] [_ linearB _]
	/// 
	/// </pre>
	/// The inverse transform is performed by converting PCS XYZ components to linear
	/// RGB components through the inverse of the above 3x3 matrix, and then converting
	/// linear RGB to device RGB through inverses of the TRCs.
	/// </para>
	/// </summary>



	public class ICC_ProfileRGB : ICC_Profile
	{

		internal const long SerialVersionUID = 8505067385152579334L;

		/// <summary>
		/// Used to get a gamma value or TRC for the red component.
		/// </summary>
		public const int REDCOMPONENT = 0;

		/// <summary>
		/// Used to get a gamma value or TRC for the green component.
		/// </summary>
		public const int GREENCOMPONENT = 1;

		/// <summary>
		/// Used to get a gamma value or TRC for the blue component.
		/// </summary>
		public const int BLUECOMPONENT = 2;


		/// <summary>
		/// Constructs an new <code>ICC_ProfileRGB</code> from a CMM ID.
		/// </summary>
		/// <param name="p"> The CMM ID for the profile.
		///  </param>
		internal ICC_ProfileRGB(Profile p) : base(p)
		{
		}

		/// <summary>
		/// Constructs a new <code>ICC_ProfileRGB</code> from a
		/// ProfileDeferralInfo object.
		/// </summary>
		/// <param name="pdi"> </param>
		internal ICC_ProfileRGB(ProfileDeferralInfo pdi) : base(pdi)
		{
		}


		/// <summary>
		/// Returns an array that contains the components of the profile's
		/// <CODE>mediaWhitePointTag</CODE>.
		/// </summary>
		/// <returns> A 3-element <CODE>float</CODE> array containing the x, y,
		/// and z components of the profile's <CODE>mediaWhitePointTag</CODE>. </returns>
		public override float[] MediaWhitePoint
		{
			get
			{
				return base.MediaWhitePoint;
			}
		}


		/// <summary>
		/// Returns a 3x3 <CODE>float</CODE> matrix constructed from the
		/// X, Y, and Z components of the profile's <CODE>redColorantTag</CODE>,
		/// <CODE>greenColorantTag</CODE>, and <CODE>blueColorantTag</CODE>.
		/// <para>
		/// This matrix can be used for color transforms in the forward
		/// direction of the profile--from the profile color space
		/// to the CIEXYZ PCS.
		/// 
		/// </para>
		/// </summary>
		/// <returns> A 3x3 <CODE>float</CODE> array that contains the x, y, and z
		/// components of the profile's <CODE>redColorantTag</CODE>,
		/// <CODE>greenColorantTag</CODE>, and <CODE>blueColorantTag</CODE>. </returns>
		public virtual float[][] Matrix
		{
			get
			{
//JAVA TO C# CONVERTER NOTE: The following call to the 'RectangularArrays' helper class reproduces the rectangular array initialization that is automatic in Java:
//ORIGINAL LINE: float[][] theMatrix = new float[3][3];
				float[][] theMatrix = RectangularArrays.ReturnRectangularFloatArray(3, 3);
				float[] tmpMatrix;
    
				tmpMatrix = GetXYZTag(ICC_Profile.IcSigRedColorantTag);
				theMatrix[0][0] = tmpMatrix[0];
				theMatrix[1][0] = tmpMatrix[1];
				theMatrix[2][0] = tmpMatrix[2];
				tmpMatrix = GetXYZTag(ICC_Profile.IcSigGreenColorantTag);
				theMatrix[0][1] = tmpMatrix[0];
				theMatrix[1][1] = tmpMatrix[1];
				theMatrix[2][1] = tmpMatrix[2];
				tmpMatrix = GetXYZTag(ICC_Profile.IcSigBlueColorantTag);
				theMatrix[0][2] = tmpMatrix[0];
				theMatrix[1][2] = tmpMatrix[1];
				theMatrix[2][2] = tmpMatrix[2];
				return theMatrix;
			}
		}

		/// <summary>
		/// Returns a gamma value representing the tone reproduction curve
		/// (TRC) for a particular component.  The component parameter
		/// must be one of REDCOMPONENT, GREENCOMPONENT, or BLUECOMPONENT.
		/// <para>
		/// If the profile
		/// represents the TRC for the corresponding component
		/// as a table rather than a single gamma value, an
		/// exception is thrown.  In this case the actual table
		/// can be obtained through the <seealso cref="#getTRC(int)"/> method.
		/// When using a gamma value,
		/// the linear component (R, G, or B) is computed as follows:
		/// <pre>
		/// 
		/// &nbsp;                                         gamma
		/// &nbsp;        linearComponent = deviceComponent
		/// 
		/// </pre>
		/// </para>
		/// </summary>
		/// <param name="component"> The <CODE>ICC_ProfileRGB</CODE> constant that
		/// represents the component whose TRC you want to retrieve </param>
		/// <returns> the gamma value as a float. </returns>
		/// <exception cref="ProfileDataException"> if the profile does not specify
		///            the corresponding TRC as a single gamma value. </exception>
		public override float GetGamma(int component)
		{
		float theGamma;
		int theSignature;

			switch (component)
			{
			case REDCOMPONENT:
				theSignature = ICC_Profile.IcSigRedTRCTag;
				break;

			case GREENCOMPONENT:
				theSignature = ICC_Profile.IcSigGreenTRCTag;
				break;

			case BLUECOMPONENT:
				theSignature = ICC_Profile.IcSigBlueTRCTag;
				break;

			default:
				throw new IllegalArgumentException("Must be Red, Green, or Blue");
			}

			theGamma = base.GetGamma(theSignature);

			return theGamma;
		}

		/// <summary>
		/// Returns the TRC for a particular component as an array.
		/// Component must be <code>REDCOMPONENT</code>,
		/// <code>GREENCOMPONENT</code>, or <code>BLUECOMPONENT</code>.
		/// Otherwise the returned array
		/// represents a lookup table where the input component value
		/// is conceptually in the range [0.0, 1.0].  Value 0.0 maps
		/// to array index 0 and value 1.0 maps to array index length-1.
		/// Interpolation might be used to generate output values for
		/// input values that do not map exactly to an index in the
		/// array.  Output values also map linearly to the range [0.0, 1.0].
		/// Value 0.0 is represented by an array value of 0x0000 and
		/// value 1.0 by 0xFFFF.  In other words, the values are really unsigned
		/// <code>short</code> values even though they are returned in a
		/// <code>short</code> array.
		/// 
		/// If the profile has specified the corresponding TRC
		/// as linear (gamma = 1.0) or as a simple gamma value, this method
		/// throws an exception.  In this case, the <seealso cref="#getGamma(int)"/>
		/// method should be used to get the gamma value.
		/// </summary>
		/// <param name="component"> The <CODE>ICC_ProfileRGB</CODE> constant that
		/// represents the component whose TRC you want to retrieve:
		/// <CODE>REDCOMPONENT</CODE>, <CODE>GREENCOMPONENT</CODE>, or
		/// <CODE>BLUECOMPONENT</CODE>.
		/// </param>
		/// <returns> a short array representing the TRC. </returns>
		/// <exception cref="ProfileDataException"> if the profile does not specify
		///            the corresponding TRC as a table. </exception>
		public override short[] GetTRC(int component)
		{
		short[] theTRC;
		int theSignature;

			switch (component)
			{
			case REDCOMPONENT:
				theSignature = ICC_Profile.IcSigRedTRCTag;
				break;

			case GREENCOMPONENT:
				theSignature = ICC_Profile.IcSigGreenTRCTag;
				break;

			case BLUECOMPONENT:
				theSignature = ICC_Profile.IcSigBlueTRCTag;
				break;

			default:
				throw new IllegalArgumentException("Must be Red, Green, or Blue");
			}

			theTRC = base.GetTRC(theSignature);

			return theTRC;
		}

	}

}
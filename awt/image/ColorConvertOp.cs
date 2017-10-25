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

namespace java.awt.image
{

	using ColorTransform = sun.java2d.cmm.ColorTransform;
	using CMSManager = sun.java2d.cmm.CMSManager;
	using ProfileDeferralMgr = sun.java2d.cmm.ProfileDeferralMgr;
	using PCMM = sun.java2d.cmm.PCMM;

	/// <summary>
	/// This class performs a pixel-by-pixel color conversion of the data in
	/// the source image.  The resulting color values are scaled to the precision
	/// of the destination image.  Color conversion can be specified
	/// via an array of ColorSpace objects or an array of ICC_Profile objects.
	/// <para>
	/// If the source is a BufferedImage with premultiplied alpha, the
	/// color components are divided by the alpha component before color conversion.
	/// If the destination is a BufferedImage with premultiplied alpha, the
	/// color components are multiplied by the alpha component after conversion.
	/// Rasters are treated as having no alpha channel, i.e. all bands are
	/// color bands.
	/// </para>
	/// <para>
	/// If a RenderingHints object is specified in the constructor, the
	/// color rendering hint and the dithering hint may be used to control
	/// color conversion.
	/// </para>
	/// <para>
	/// Note that Source and Destination may be the same object.
	/// </para>
	/// </summary>
	/// <seealso cref= java.awt.RenderingHints#KEY_COLOR_RENDERING </seealso>
	/// <seealso cref= java.awt.RenderingHints#KEY_DITHERING </seealso>
	public class ColorConvertOp : BufferedImageOp, RasterOp
	{
		internal ICC_Profile[] ProfileList;
		internal ColorSpace[] CSList;
		internal ColorTransform ThisTransform, ThisRasterTransform;
		internal ICC_Profile ThisSrcProfile, ThisDestProfile;
		internal RenderingHints Hints;
		internal bool GotProfiles;
		internal float[] SrcMinVals, SrcMaxVals, DstMinVals, DstMaxVals;

		/* the class initializer */
		static ColorConvertOp()
		{
			if (ProfileDeferralMgr.deferring)
			{
				ProfileDeferralMgr.activateProfiles();
			}
		}

		/// <summary>
		/// Constructs a new ColorConvertOp which will convert
		/// from a source color space to a destination color space.
		/// The RenderingHints argument may be null.
		/// This Op can be used only with BufferedImages, and will convert
		/// directly from the ColorSpace of the source image to that of the
		/// destination.  The destination argument of the filter method
		/// cannot be specified as null. </summary>
		/// <param name="hints"> the <code>RenderingHints</code> object used to control
		///        the color conversion, or <code>null</code> </param>
		public ColorConvertOp(RenderingHints hints)
		{
			ProfileList = new ICC_Profile [0]; // 0 length list
			this.Hints = hints;
		}

		/// <summary>
		/// Constructs a new ColorConvertOp from a ColorSpace object.
		/// The RenderingHints argument may be null.  This
		/// Op can be used only with BufferedImages, and is primarily useful
		/// when the <seealso cref="#filter(BufferedImage, BufferedImage) filter"/>
		/// method is invoked with a destination argument of null.
		/// In that case, the ColorSpace defines the destination color space
		/// for the destination created by the filter method.  Otherwise, the
		/// ColorSpace defines an intermediate space to which the source is
		/// converted before being converted to the destination space. </summary>
		/// <param name="cspace"> defines the destination <code>ColorSpace</code> or an
		///        intermediate <code>ColorSpace</code> </param>
		/// <param name="hints"> the <code>RenderingHints</code> object used to control
		///        the color conversion, or <code>null</code> </param>
		/// <exception cref="NullPointerException"> if cspace is null </exception>
		public ColorConvertOp(ColorSpace cspace, RenderingHints hints)
		{
			if (cspace == null)
			{
				throw new NullPointerException("ColorSpace cannot be null");
			}
			if (cspace is ICC_ColorSpace)
			{
				ProfileList = new ICC_Profile [1]; // 1 profile in the list

				ProfileList [0] = ((ICC_ColorSpace) cspace).Profile;
			}
			else
			{
				CSList = new ColorSpace[1]; // non-ICC case: 1 ColorSpace in list
				CSList[0] = cspace;
			}
			this.Hints = hints;
		}


		/// <summary>
		/// Constructs a new ColorConvertOp from two ColorSpace objects.
		/// The RenderingHints argument may be null.
		/// This Op is primarily useful for calling the filter method on
		/// Rasters, in which case the two ColorSpaces define the operation
		/// to be performed on the Rasters.  In that case, the number of bands
		/// in the source Raster must match the number of components in
		/// srcCspace, and the number of bands in the destination Raster
		/// must match the number of components in dstCspace.  For BufferedImages,
		/// the two ColorSpaces define intermediate spaces through which the
		/// source is converted before being converted to the destination space. </summary>
		/// <param name="srcCspace"> the source <code>ColorSpace</code> </param>
		/// <param name="dstCspace"> the destination <code>ColorSpace</code> </param>
		/// <param name="hints"> the <code>RenderingHints</code> object used to control
		///        the color conversion, or <code>null</code> </param>
		/// <exception cref="NullPointerException"> if either srcCspace or dstCspace is null </exception>
		public ColorConvertOp(ColorSpace srcCspace, ColorSpace dstCspace, RenderingHints hints)
		{
			if ((srcCspace == null) || (dstCspace == null))
			{
				throw new NullPointerException("ColorSpaces cannot be null");
			}
			if ((srcCspace is ICC_ColorSpace) && (dstCspace is ICC_ColorSpace))
			{
				ProfileList = new ICC_Profile [2]; // 2 profiles in the list

				ProfileList [0] = ((ICC_ColorSpace) srcCspace).Profile;
				ProfileList [1] = ((ICC_ColorSpace) dstCspace).Profile;

				GetMinMaxValsFromColorSpaces(srcCspace, dstCspace);
			}
			else
			{
				/* non-ICC case: 2 ColorSpaces in list */
				CSList = new ColorSpace[2];
				CSList[0] = srcCspace;
				CSList[1] = dstCspace;
			}
			this.Hints = hints;
		}


		 /// <summary>
		 /// Constructs a new ColorConvertOp from an array of ICC_Profiles.
		 /// The RenderingHints argument may be null.
		 /// The sequence of profiles may include profiles that represent color
		 /// spaces, profiles that represent effects, etc.  If the whole sequence
		 /// does not represent a well-defined color conversion, an exception is
		 /// thrown.
		 /// <para>For BufferedImages, if the ColorSpace
		 /// of the source BufferedImage does not match the requirements of the
		 /// first profile in the array,
		 /// the first conversion is to an appropriate ColorSpace.
		 /// If the requirements of the last profile in the array are not met
		 /// by the ColorSpace of the destination BufferedImage,
		 /// the last conversion is to the destination's ColorSpace.
		 /// </para>
		 /// <para>For Rasters, the number of bands in the source Raster must match
		 /// the requirements of the first profile in the array, and the
		 /// number of bands in the destination Raster must match the requirements
		 /// of the last profile in the array.  The array must have at least two
		 /// elements or calling the filter method for Rasters will throw an
		 /// IllegalArgumentException.
		 /// </para>
		 /// </summary>
		 /// <param name="profiles"> the array of <code>ICC_Profile</code> objects </param>
		 /// <param name="hints"> the <code>RenderingHints</code> object used to control
		 ///        the color conversion, or <code>null</code> </param>
		 /// <exception cref="IllegalArgumentException"> when the profile sequence does not
		 ///             specify a well-defined color conversion </exception>
		 /// <exception cref="NullPointerException"> if profiles is null </exception>
		public ColorConvertOp(ICC_Profile[] profiles, RenderingHints hints)
		{
			if (profiles == null)
			{
				throw new NullPointerException("Profiles cannot be null");
			}
			GotProfiles = true;
			ProfileList = new ICC_Profile[profiles.Length];
			for (int i1 = 0; i1 < profiles.Length; i1++)
			{
				ProfileList[i1] = profiles[i1];
			}
			this.Hints = hints;
		}


		/// <summary>
		/// Returns the array of ICC_Profiles used to construct this ColorConvertOp.
		/// Returns null if the ColorConvertOp was not constructed from such an
		/// array. </summary>
		/// <returns> the array of <code>ICC_Profile</code> objects of this
		///         <code>ColorConvertOp</code>, or <code>null</code> if this
		///         <code>ColorConvertOp</code> was not constructed with an
		///         array of <code>ICC_Profile</code> objects. </returns>
		public ICC_Profile[] ICC_Profiles
		{
			get
			{
				if (GotProfiles)
				{
					ICC_Profile[] profiles = new ICC_Profile[ProfileList.Length];
					for (int i1 = 0; i1 < ProfileList.Length; i1++)
					{
						profiles[i1] = ProfileList[i1];
					}
					return profiles;
				}
				return null;
			}
		}

		/// <summary>
		/// ColorConverts the source BufferedImage.
		/// If the destination image is null,
		/// a BufferedImage will be created with an appropriate ColorModel. </summary>
		/// <param name="src"> the source <code>BufferedImage</code> to be converted </param>
		/// <param name="dest"> the destination <code>BufferedImage</code>,
		///        or <code>null</code> </param>
		/// <returns> <code>dest</code> color converted from <code>src</code>
		///         or a new, converted <code>BufferedImage</code>
		///         if <code>dest</code> is <code>null</code> </returns>
		/// <exception cref="IllegalArgumentException"> if dest is null and this op was
		///             constructed using the constructor which takes only a
		///             RenderingHints argument, since the operation is ill defined. </exception>
		public BufferedImage Filter(BufferedImage src, BufferedImage dest)
		{
			ColorSpace srcColorSpace, destColorSpace;
			BufferedImage savdest = null;

			if (src.ColorModel is IndexColorModel)
			{
				IndexColorModel icm = (IndexColorModel) src.ColorModel;
				src = icm.ConvertToIntDiscrete(src.Raster, true);
			}
			srcColorSpace = src.ColorModel.ColorSpace;
			if (dest != null)
			{
				if (dest.ColorModel is IndexColorModel)
				{
					savdest = dest;
					dest = null;
					destColorSpace = null;
				}
				else
				{
					destColorSpace = dest.ColorModel.ColorSpace;
				}
			}
			else
			{
				destColorSpace = null;
			}

			if ((CSList != null) || (!(srcColorSpace is ICC_ColorSpace)) || ((dest != null) && (!(destColorSpace is ICC_ColorSpace))))
			{
				/* non-ICC case */
				dest = NonICCBIFilter(src, srcColorSpace, dest, destColorSpace);
			}
			else
			{
				dest = ICCBIFilter(src, srcColorSpace, dest, destColorSpace);
			}

			if (savdest != null)
			{
				Graphics2D big = savdest.CreateGraphics();
				try
				{
					big.DrawImage(dest, 0, 0, null);
				}
				finally
				{
					big.Dispose();
				}
				return savdest;
			}
			else
			{
				return dest;
			}
		}

		private BufferedImage ICCBIFilter(BufferedImage src, ColorSpace srcColorSpace, BufferedImage dest, ColorSpace destColorSpace)
		{
		int nProfiles = ProfileList.Length;
		ICC_Profile srcProfile = null, destProfile = null;

			srcProfile = ((ICC_ColorSpace) srcColorSpace).Profile;

			if (dest == null)
			{
			/* last profile in the list defines
			                              the output color space */
				if (nProfiles == 0)
				{
					throw new IllegalArgumentException("Destination ColorSpace is undefined");
				}
				destProfile = ProfileList [nProfiles - 1];
				dest = CreateCompatibleDestImage(src, null);
			}
			else
			{
				if (src.Height != dest.Height || src.Width != dest.Width)
				{
					throw new IllegalArgumentException("Width or height of BufferedImages do not match");
				}
				destProfile = ((ICC_ColorSpace) destColorSpace).Profile;
			}

			/* Checking if all profiles in the transform sequence are the same.
			 * If so, performing just copying the data.
			 */
			if (srcProfile == destProfile)
			{
				bool noTrans = true;
				for (int i = 0; i < nProfiles; i++)
				{
					if (srcProfile != ProfileList[i])
					{
						noTrans = false;
						break;
					}
				}
				if (noTrans)
				{
					Graphics2D g = dest.CreateGraphics();
					try
					{
						g.DrawImage(src, 0, 0, null);
					}
					finally
					{
						g.Dispose();
					}

					return dest;
				}
			}

			/* make a new transform if needed */
			if ((ThisTransform == null) || (ThisSrcProfile != srcProfile) || (ThisDestProfile != destProfile))
			{
				UpdateBITransform(srcProfile, destProfile);
			}

			/* color convert the image */
			ThisTransform.colorConvert(src, dest);

			return dest;
		}

		private void UpdateBITransform(ICC_Profile srcProfile, ICC_Profile destProfile)
		{
			ICC_Profile[] theProfiles;
			int i1, nProfiles, nTransforms, whichTrans, renderState;
			ColorTransform[] theTransforms;
			bool useSrc = false, useDest = false;

			nProfiles = ProfileList.Length;
			nTransforms = nProfiles;
			if ((nProfiles == 0) || (srcProfile != ProfileList[0]))
			{
				nTransforms += 1;
				useSrc = true;
			}
			if ((nProfiles == 0) || (destProfile != ProfileList[nProfiles - 1]) || (nTransforms < 2))
			{
				nTransforms += 1;
				useDest = true;
			}

			/* make the profile list */
			theProfiles = new ICC_Profile[nTransforms]; /* the list of profiles
	                                                       for this Op */

			int idx = 0;
			if (useSrc)
			{
				/* insert source as first profile */
				theProfiles[idx++] = srcProfile;
			}

			for (i1 = 0; i1 < nProfiles; i1++)
			{
									   /* insert profiles defined in this Op */
				theProfiles[idx++] = ProfileList [i1];
			}

			if (useDest)
			{
				/* insert dest as last profile */
				theProfiles[idx] = destProfile;
			}

			/* make the transform list */
			theTransforms = new ColorTransform [nTransforms];

			/* initialize transform get loop */
			if (theProfiles[0].ProfileClass == ICC_Profile.CLASS_OUTPUT)
			{
											/* if first profile is a printer
											   render as colorimetric */
				renderState = ICC_Profile.IcRelativeColorimetric;
			}
			else
			{
				renderState = ICC_Profile.IcPerceptual; /* render any other
	                                                       class perceptually */
			}

			whichTrans = ColorTransform.In;

			PCMM mdl = CMSManager.Module;

			/* get the transforms from each profile */
			for (i1 = 0; i1 < nTransforms; i1++)
			{
				if (i1 == nTransforms - 1) // last profile?
				{
					whichTrans = ColorTransform.Out; // get output transform
				}
				else // check for abstract profile
				{
					if ((whichTrans == ColorTransform.Simulation) && (theProfiles[i1].ProfileClass == ICC_Profile.CLASS_ABSTRACT))
					{
					renderState = ICC_Profile.IcPerceptual;
						whichTrans = ColorTransform.In;
					}
				}

				theTransforms[i1] = mdl.createTransform(theProfiles[i1], renderState, whichTrans);

				/* get this profile's rendering intent to select transform
				   from next profile */
				renderState = GetRenderingIntent(theProfiles[i1]);

				/* "middle" profiles use simulation transform */
				whichTrans = ColorTransform.Simulation;
			}

			/* make the net transform */
			ThisTransform = mdl.createTransform(theTransforms);

			/* update corresponding source and dest profiles */
			ThisSrcProfile = srcProfile;
			ThisDestProfile = destProfile;
		}

		/// <summary>
		/// ColorConverts the image data in the source Raster.
		/// If the destination Raster is null, a new Raster will be created.
		/// The number of bands in the source and destination Rasters must
		/// meet the requirements explained above.  The constructor used to
		/// create this ColorConvertOp must have provided enough information
		/// to define both source and destination color spaces.  See above.
		/// Otherwise, an exception is thrown. </summary>
		/// <param name="src"> the source <code>Raster</code> to be converted </param>
		/// <param name="dest"> the destination <code>WritableRaster</code>,
		///        or <code>null</code> </param>
		/// <returns> <code>dest</code> color converted from <code>src</code>
		///         or a new, converted <code>WritableRaster</code>
		///         if <code>dest</code> is <code>null</code> </returns>
		/// <exception cref="IllegalArgumentException"> if the number of source or
		///             destination bands is incorrect, the source or destination
		///             color spaces are undefined, or this op was constructed
		///             with one of the constructors that applies only to
		///             operations on BufferedImages. </exception>
		public WritableRaster Filter(Raster src, WritableRaster dest)
		{

			if (CSList != null)
			{
				/* non-ICC case */
				return NonICCRasterFilter(src, dest);
			}
			int nProfiles = ProfileList.Length;
			if (nProfiles < 2)
			{
				throw new IllegalArgumentException("Source or Destination ColorSpace is undefined");
			}
			if (src.NumBands != ProfileList[0].NumComponents)
			{
				throw new IllegalArgumentException("Numbers of source Raster bands and source color space " + "components do not match");
			}
			if (dest == null)
			{
				dest = CreateCompatibleDestRaster(src);
			}
			else
			{
				if (src.Height != dest.Height || src.Width != dest.Width)
				{
					throw new IllegalArgumentException("Width or height of Rasters do not match");
				}
				if (dest.NumBands != ProfileList[nProfiles - 1].NumComponents)
				{
					throw new IllegalArgumentException("Numbers of destination Raster bands and destination " + "color space components do not match");
				}
			}

			/* make a new transform if needed */
			if (ThisRasterTransform == null)
			{
				int i1, whichTrans, renderState;
				ColorTransform[] theTransforms;

				/* make the transform list */
				theTransforms = new ColorTransform [nProfiles];

				/* initialize transform get loop */
				if (ProfileList[0].ProfileClass == ICC_Profile.CLASS_OUTPUT)
				{
												/* if first profile is a printer
												   render as colorimetric */
					renderState = ICC_Profile.IcRelativeColorimetric;
				}
				else
				{
					renderState = ICC_Profile.IcPerceptual; /* render any other
	                                                           class perceptually */
				}

				whichTrans = ColorTransform.In;

				PCMM mdl = CMSManager.Module;

				/* get the transforms from each profile */
				for (i1 = 0; i1 < nProfiles; i1++)
				{
					if (i1 == nProfiles - 1) // last profile?
					{
						whichTrans = ColorTransform.Out; // get output transform
					}
					else // check for abstract profile
					{
						if ((whichTrans == ColorTransform.Simulation) && (ProfileList[i1].ProfileClass == ICC_Profile.CLASS_ABSTRACT))
						{
							renderState = ICC_Profile.IcPerceptual;
							whichTrans = ColorTransform.In;
						}
					}

					theTransforms[i1] = mdl.createTransform(ProfileList[i1], renderState, whichTrans);

					/* get this profile's rendering intent to select transform
					   from next profile */
					renderState = GetRenderingIntent(ProfileList[i1]);

					/* "middle" profiles use simulation transform */
					whichTrans = ColorTransform.Simulation;
				}

				/* make the net transform */
				ThisRasterTransform = mdl.createTransform(theTransforms);
			}

			int srcTransferType = src.TransferType;
			int dstTransferType = dest.TransferType;
			if ((srcTransferType == DataBuffer.TYPE_FLOAT) || (srcTransferType == DataBuffer.TYPE_DOUBLE) || (dstTransferType == DataBuffer.TYPE_FLOAT) || (dstTransferType == DataBuffer.TYPE_DOUBLE))
			{
				if (SrcMinVals == null)
				{
					GetMinMaxValsFromProfiles(ProfileList[0], ProfileList[nProfiles - 1]);
				}
				/* color convert the raster */
				ThisRasterTransform.colorConvert(src, dest, SrcMinVals, SrcMaxVals, DstMinVals, DstMaxVals);
			}
			else
			{
				/* color convert the raster */
				ThisRasterTransform.colorConvert(src, dest);
			}


			return dest;
		}

		/// <summary>
		/// Returns the bounding box of the destination, given this source.
		/// Note that this will be the same as the the bounding box of the
		/// source. </summary>
		/// <param name="src"> the source <code>BufferedImage</code> </param>
		/// <returns> a <code>Rectangle2D</code> that is the bounding box
		///         of the destination, given the specified <code>src</code> </returns>
		public Rectangle2D GetBounds2D(BufferedImage src)
		{
			return GetBounds2D(src.Raster);
		}

		/// <summary>
		/// Returns the bounding box of the destination, given this source.
		/// Note that this will be the same as the the bounding box of the
		/// source. </summary>
		/// <param name="src"> the source <code>Raster</code> </param>
		/// <returns> a <code>Rectangle2D</code> that is the bounding box
		///         of the destination, given the specified <code>src</code> </returns>
		public Rectangle2D GetBounds2D(Raster src)
		{
			/*        return new Rectangle (src.getXOffset(),
			                      src.getYOffset(),
			                      src.getWidth(), src.getHeight()); */
			return src.Bounds;
		}

		/// <summary>
		/// Creates a zeroed destination image with the correct size and number of
		/// bands, given this source. </summary>
		/// <param name="src">       Source image for the filter operation. </param>
		/// <param name="destCM">    ColorModel of the destination.  If null, an
		///                  appropriate ColorModel will be used. </param>
		/// <returns> a <code>BufferedImage</code> with the correct size and
		/// number of bands from the specified <code>src</code>. </returns>
		/// <exception cref="IllegalArgumentException"> if <code>destCM</code> is
		///         <code>null</code> and this <code>ColorConvertOp</code> was
		///         created without any <code>ICC_Profile</code> or
		///         <code>ColorSpace</code> defined for the destination </exception>
		public virtual BufferedImage CreateCompatibleDestImage(BufferedImage src, ColorModel destCM)
		{
			ColorSpace cs = null;
			if (destCM == null)
			{
				if (CSList == null)
				{
					/* ICC case */
					int nProfiles = ProfileList.Length;
					if (nProfiles == 0)
					{
						throw new IllegalArgumentException("Destination ColorSpace is undefined");
					}
					ICC_Profile destProfile = ProfileList[nProfiles - 1];
					cs = new ICC_ColorSpace(destProfile);
				}
				else
				{
					/* non-ICC case */
					int nSpaces = CSList.Length;
					cs = CSList[nSpaces - 1];
				}
			}
			return CreateCompatibleDestImage(src, destCM, cs);
		}

		private BufferedImage CreateCompatibleDestImage(BufferedImage src, ColorModel destCM, ColorSpace destCS)
		{
			BufferedImage image;
			if (destCM == null)
			{
				ColorModel srcCM = src.ColorModel;
				int nbands = destCS.NumComponents;
				bool hasAlpha = srcCM.HasAlpha();
				if (hasAlpha)
				{
				   nbands += 1;
				}
				int[] nbits = new int[nbands];
				for (int i = 0; i < nbands; i++)
				{
					nbits[i] = 8;
				}
				destCM = new ComponentColorModel(destCS, nbits, hasAlpha, srcCM.AlphaPremultiplied, srcCM.Transparency, DataBuffer.TYPE_BYTE);
			}
			int w = src.Width;
			int h = src.Height;
			image = new BufferedImage(destCM, destCM.CreateCompatibleWritableRaster(w, h), destCM.AlphaPremultiplied, null);
			return image;
		}


		/// <summary>
		/// Creates a zeroed destination Raster with the correct size and number of
		/// bands, given this source. </summary>
		/// <param name="src"> the specified <code>Raster</code> </param>
		/// <returns> a <code>WritableRaster</code> with the correct size and number
		///         of bands from the specified <code>src</code> </returns>
		/// <exception cref="IllegalArgumentException"> if this <code>ColorConvertOp</code>
		///         was created without sufficient information to define the
		///         <code>dst</code> and <code>src</code> color spaces </exception>
		public virtual WritableRaster CreateCompatibleDestRaster(Raster src)
		{
			int ncomponents;

			if (CSList != null)
			{
				/* non-ICC case */
				if (CSList.Length != 2)
				{
					throw new IllegalArgumentException("Destination ColorSpace is undefined");
				}
				ncomponents = CSList[1].NumComponents;
			}
			else
			{
				/* ICC case */
				int nProfiles = ProfileList.Length;
				if (nProfiles < 2)
				{
					throw new IllegalArgumentException("Destination ColorSpace is undefined");
				}
				ncomponents = ProfileList[nProfiles - 1].NumComponents;
			}

			WritableRaster dest = Raster.CreateInterleavedRaster(DataBuffer.TYPE_BYTE, src.Width, src.Height, ncomponents, new Point(src.MinX, src.MinY));
			return dest;
		}

		/// <summary>
		/// Returns the location of the destination point given a
		/// point in the source.  If <code>dstPt</code> is non-null,
		/// it will be used to hold the return value.  Note that
		/// for this class, the destination point will be the same
		/// as the source point. </summary>
		/// <param name="srcPt"> the specified source <code>Point2D</code> </param>
		/// <param name="dstPt"> the destination <code>Point2D</code> </param>
		/// <returns> <code>dstPt</code> after setting its location to be
		///         the same as <code>srcPt</code> </returns>
		public Point2D GetPoint2D(Point2D srcPt, Point2D dstPt)
		{
			if (dstPt == null)
			{
				dstPt = new Point2D.Float();
			}
			dstPt.SetLocation(srcPt.X, srcPt.Y);

			return dstPt;
		}


		/// <summary>
		/// Returns the RenderingIntent from the specified ICC Profile.
		/// </summary>
		private int GetRenderingIntent(ICC_Profile profile)
		{
			sbyte[] header = profile.GetData(ICC_Profile.IcSigHead);
			int index = ICC_Profile.IcHdrRenderingIntent;

			/* According to ICC spec, only the least-significant 16 bits shall be
			 * used to encode the rendering intent. The most significant 16 bits
			 * shall be set to zero. Thus, we are ignoring two most significant
			 * bytes here.
			 *
			 *  See http://www.color.org/ICC1v42_2006-05.pdf, section 7.2.15.
			 */
			return ((header[index + 2] & 0xff) << 8) | (header[index + 3] & 0xff);
		}

		/// <summary>
		/// Returns the rendering hints used by this op. </summary>
		/// <returns> the <code>RenderingHints</code> object of this
		///         <code>ColorConvertOp</code> </returns>
		public RenderingHints RenderingHints
		{
			get
			{
				return Hints;
			}
		}

		private BufferedImage NonICCBIFilter(BufferedImage src, ColorSpace srcColorSpace, BufferedImage dst, ColorSpace dstColorSpace)
		{

			int w = src.Width;
			int h = src.Height;
			ICC_ColorSpace ciespace = (ICC_ColorSpace) ColorSpace.GetInstance(ColorSpace.CS_CIEXYZ);
			if (dst == null)
			{
				dst = CreateCompatibleDestImage(src, null);
				dstColorSpace = dst.ColorModel.ColorSpace;
			}
			else
			{
				if ((h != dst.Height) || (w != dst.Width))
				{
					throw new IllegalArgumentException("Width or height of BufferedImages do not match");
				}
			}
			Raster srcRas = src.Raster;
			WritableRaster dstRas = dst.Raster;
			ColorModel srcCM = src.ColorModel;
			ColorModel dstCM = dst.ColorModel;
			int srcNumComp = srcCM.NumColorComponents;
			int dstNumComp = dstCM.NumColorComponents;
			bool dstHasAlpha = dstCM.HasAlpha();
			bool needSrcAlpha = srcCM.HasAlpha() && dstHasAlpha;
			ColorSpace[] list;
			if ((CSList == null) && (ProfileList.Length != 0))
			{
				/* possible non-ICC src, some profiles, possible non-ICC dst */
				bool nonICCSrc, nonICCDst;
				ICC_Profile srcProfile, dstProfile;
				if (!(srcColorSpace is ICC_ColorSpace))
				{
					nonICCSrc = true;
					srcProfile = ciespace.Profile;
				}
				else
				{
					nonICCSrc = false;
					srcProfile = ((ICC_ColorSpace) srcColorSpace).Profile;
				}
				if (!(dstColorSpace is ICC_ColorSpace))
				{
					nonICCDst = true;
					dstProfile = ciespace.Profile;
				}
				else
				{
					nonICCDst = false;
					dstProfile = ((ICC_ColorSpace) dstColorSpace).Profile;
				}
				/* make a new transform if needed */
				if ((ThisTransform == null) || (ThisSrcProfile != srcProfile) || (ThisDestProfile != dstProfile))
				{
					UpdateBITransform(srcProfile, dstProfile);
				}
				// process per scanline
				float maxNum = 65535.0f; // use 16-bit precision in CMM
				ColorSpace cs;
				int iccSrcNumComp;
				if (nonICCSrc)
				{
					cs = ciespace;
					iccSrcNumComp = 3;
				}
				else
				{
					cs = srcColorSpace;
					iccSrcNumComp = srcNumComp;
				}
				float[] srcMinVal = new float[iccSrcNumComp];
				float[] srcInvDiffMinMax = new float[iccSrcNumComp];
				for (int i = 0; i < srcNumComp; i++)
				{
					srcMinVal[i] = cs.GetMinValue(i);
					srcInvDiffMinMax[i] = maxNum / (cs.GetMaxValue(i) - srcMinVal[i]);
				}
				int iccDstNumComp;
				if (nonICCDst)
				{
					cs = ciespace;
					iccDstNumComp = 3;
				}
				else
				{
					cs = dstColorSpace;
					iccDstNumComp = dstNumComp;
				}
				float[] dstMinVal = new float[iccDstNumComp];
				float[] dstDiffMinMax = new float[iccDstNumComp];
				for (int i = 0; i < dstNumComp; i++)
				{
					dstMinVal[i] = cs.GetMinValue(i);
					dstDiffMinMax[i] = (cs.GetMaxValue(i) - dstMinVal[i]) / maxNum;
				}
				float[] dstColor;
				if (dstHasAlpha)
				{
					int size = ((dstNumComp + 1) > 3) ? (dstNumComp + 1) : 3;
					dstColor = new float[size];
				}
				else
				{
					int size = (dstNumComp > 3) ? dstNumComp : 3;
					dstColor = new float[size];
				}
				short[] srcLine = new short[w * iccSrcNumComp];
				short[] dstLine = new short[w * iccDstNumComp];
				Object pixel;
				float[] color;
				float[] alpha = null;
				if (needSrcAlpha)
				{
					alpha = new float[w];
				}
				int idx;
				// process each scanline
				for (int y = 0; y < h; y++)
				{
					// convert src scanline
					pixel = null;
					color = null;
					idx = 0;
					for (int x = 0; x < w; x++)
					{
						pixel = srcRas.GetDataElements(x, y, pixel);
						color = srcCM.GetNormalizedComponents(pixel, color, 0);
						if (needSrcAlpha)
						{
							alpha[x] = color[srcNumComp];
						}
						if (nonICCSrc)
						{
							color = srcColorSpace.ToCIEXYZ(color);
						}
						for (int i = 0; i < iccSrcNumComp; i++)
						{
							srcLine[idx++] = (short)((color[i] - srcMinVal[i]) * srcInvDiffMinMax[i] + 0.5f);
						}
					}
					// color convert srcLine to dstLine
					ThisTransform.colorConvert(srcLine, dstLine);
					// convert dst scanline
					pixel = null;
					idx = 0;
					for (int x = 0; x < w; x++)
					{
						for (int i = 0; i < iccDstNumComp; i++)
						{
							dstColor[i] = ((float)(dstLine[idx++] & 0xffff)) * dstDiffMinMax[i] + dstMinVal[i];
						}
						if (nonICCDst)
						{
							color = srcColorSpace.FromCIEXYZ(dstColor);
							for (int i = 0; i < dstNumComp; i++)
							{
								dstColor[i] = color[i];
							}
						}
						if (needSrcAlpha)
						{
							dstColor[dstNumComp] = alpha[x];
						}
						else if (dstHasAlpha)
						{
							dstColor[dstNumComp] = 1.0f;
						}
						pixel = dstCM.GetDataElements(dstColor, 0, pixel);
						dstRas.SetDataElements(x, y, pixel);
					}
				}
			}
			else
			{
				/* possible non-ICC src, possible CSList, possible non-ICC dst */
				// process per pixel
				int numCS;
				if (CSList == null)
				{
					numCS = 0;
				}
				else
				{
					numCS = CSList.Length;
				}
				float[] dstColor;
				if (dstHasAlpha)
				{
					dstColor = new float[dstNumComp + 1];
				}
				else
				{
					dstColor = new float[dstNumComp];
				}
				Object spixel = null;
				Object dpixel = null;
				float[] color = null;
				float[] tmpColor;
				// process each pixel
				for (int y = 0; y < h; y++)
				{
					for (int x = 0; x < w; x++)
					{
						spixel = srcRas.GetDataElements(x, y, spixel);
						color = srcCM.GetNormalizedComponents(spixel, color, 0);
						tmpColor = srcColorSpace.ToCIEXYZ(color);
						for (int i = 0; i < numCS; i++)
						{
							tmpColor = CSList[i].FromCIEXYZ(tmpColor);
							tmpColor = CSList[i].ToCIEXYZ(tmpColor);
						}
						tmpColor = dstColorSpace.FromCIEXYZ(tmpColor);
						for (int i = 0; i < dstNumComp; i++)
						{
							dstColor[i] = tmpColor[i];
						}
						if (needSrcAlpha)
						{
							dstColor[dstNumComp] = color[srcNumComp];
						}
						else if (dstHasAlpha)
						{
							dstColor[dstNumComp] = 1.0f;
						}
						dpixel = dstCM.GetDataElements(dstColor, 0, dpixel);
						dstRas.SetDataElements(x, y, dpixel);

					}
				}
			}

			return dst;
		}

		/* color convert a Raster - handles byte, ushort, int, short, float,
		   or double transferTypes */
		private WritableRaster NonICCRasterFilter(Raster src, WritableRaster dst)
		{

			if (CSList.Length != 2)
			{
				throw new IllegalArgumentException("Destination ColorSpace is undefined");
			}
			if (src.NumBands != CSList[0].NumComponents)
			{
				throw new IllegalArgumentException("Numbers of source Raster bands and source color space " + "components do not match");
			}
			if (dst == null)
			{
				dst = CreateCompatibleDestRaster(src);
			}
			else
			{
				if (src.Height != dst.Height || src.Width != dst.Width)
				{
					throw new IllegalArgumentException("Width or height of Rasters do not match");
				}
				if (dst.NumBands != CSList[1].NumComponents)
				{
					throw new IllegalArgumentException("Numbers of destination Raster bands and destination " + "color space components do not match");
				}
			}

			if (SrcMinVals == null)
			{
				GetMinMaxValsFromColorSpaces(CSList[0], CSList[1]);
			}

			SampleModel srcSM = src.SampleModel;
			SampleModel dstSM = dst.SampleModel;
			bool srcIsFloat, dstIsFloat;
			int srcTransferType = src.TransferType;
			int dstTransferType = dst.TransferType;
			if ((srcTransferType == DataBuffer.TYPE_FLOAT) || (srcTransferType == DataBuffer.TYPE_DOUBLE))
			{
				srcIsFloat = true;
			}
			else
			{
				srcIsFloat = false;
			}
			if ((dstTransferType == DataBuffer.TYPE_FLOAT) || (dstTransferType == DataBuffer.TYPE_DOUBLE))
			{
				dstIsFloat = true;
			}
			else
			{
				dstIsFloat = false;
			}
			int w = src.Width;
			int h = src.Height;
			int srcNumBands = src.NumBands;
			int dstNumBands = dst.NumBands;
			float[] srcScaleFactor = null;
			float[] dstScaleFactor = null;
			if (!srcIsFloat)
			{
				srcScaleFactor = new float[srcNumBands];
				for (int i = 0; i < srcNumBands; i++)
				{
					if (srcTransferType == DataBuffer.TYPE_SHORT)
					{
						srcScaleFactor[i] = (SrcMaxVals[i] - SrcMinVals[i]) / 32767.0f;
					}
					else
					{
						srcScaleFactor[i] = (SrcMaxVals[i] - SrcMinVals[i]) / ((float)((1 << srcSM.GetSampleSize(i)) - 1));
					}
				}
			}
			if (!dstIsFloat)
			{
				dstScaleFactor = new float[dstNumBands];
				for (int i = 0; i < dstNumBands; i++)
				{
					if (dstTransferType == DataBuffer.TYPE_SHORT)
					{
						dstScaleFactor[i] = 32767.0f / (DstMaxVals[i] - DstMinVals[i]);
					}
					else
					{
						dstScaleFactor[i] = ((float)((1 << dstSM.GetSampleSize(i)) - 1)) / (DstMaxVals[i] - DstMinVals[i]);
					}
				}
			}
			int ys = src.MinY;
			int yd = dst.MinY;
			int xs, xd;
			float sample;
			float[] color = new float[srcNumBands];
			float[] tmpColor;
			ColorSpace srcColorSpace = CSList[0];
			ColorSpace dstColorSpace = CSList[1];
			// process each pixel
			for (int y = 0; y < h; y++, ys++, yd++)
			{
				// get src scanline
				xs = src.MinX;
				xd = dst.MinX;
				for (int x = 0; x < w; x++, xs++, xd++)
				{
					for (int i = 0; i < srcNumBands; i++)
					{
						sample = src.GetSampleFloat(xs, ys, i);
						if (!srcIsFloat)
						{
							sample = sample * srcScaleFactor[i] + SrcMinVals[i];
						}
						color[i] = sample;
					}
					tmpColor = srcColorSpace.ToCIEXYZ(color);
					tmpColor = dstColorSpace.FromCIEXYZ(tmpColor);
					for (int i = 0; i < dstNumBands; i++)
					{
						sample = tmpColor[i];
						if (!dstIsFloat)
						{
							sample = (sample - DstMinVals[i]) * dstScaleFactor[i];
						}
						dst.SetSample(xd, yd, i, sample);
					}
				}
			}
			return dst;
		}

		private void GetMinMaxValsFromProfiles(ICC_Profile srcProfile, ICC_Profile dstProfile)
		{
			int type = srcProfile.ColorSpaceType;
			int nc = srcProfile.NumComponents;
			SrcMinVals = new float[nc];
			SrcMaxVals = new float[nc];
			SetMinMax(type, nc, SrcMinVals, SrcMaxVals);
			type = dstProfile.ColorSpaceType;
			nc = dstProfile.NumComponents;
			DstMinVals = new float[nc];
			DstMaxVals = new float[nc];
			SetMinMax(type, nc, DstMinVals, DstMaxVals);
		}

		private void SetMinMax(int type, int nc, float[] minVals, float[] maxVals)
		{
			if (type == ColorSpace.TYPE_Lab)
			{
				minVals[0] = 0.0f; // L
				maxVals[0] = 100.0f;
				minVals[1] = -128.0f; // a
				maxVals[1] = 127.0f;
				minVals[2] = -128.0f; // b
				maxVals[2] = 127.0f;
			}
			else if (type == ColorSpace.TYPE_XYZ)
			{
				minVals[0] = minVals[1] = minVals[2] = 0.0f; // X, Y, Z
				maxVals[0] = maxVals[1] = maxVals[2] = 1.0f + (32767.0f / 32768.0f);
			}
			else
			{
				for (int i = 0; i < nc; i++)
				{
					minVals[i] = 0.0f;
					maxVals[i] = 1.0f;
				}
			}
		}

		private void GetMinMaxValsFromColorSpaces(ColorSpace srcCspace, ColorSpace dstCspace)
		{
			int nc = srcCspace.NumComponents;
			SrcMinVals = new float[nc];
			SrcMaxVals = new float[nc];
			for (int i = 0; i < nc; i++)
			{
				SrcMinVals[i] = srcCspace.GetMinValue(i);
				SrcMaxVals[i] = srcCspace.GetMaxValue(i);
			}
			nc = dstCspace.NumComponents;
			DstMinVals = new float[nc];
			DstMaxVals = new float[nc];
			for (int i = 0; i < nc; i++)
			{
				DstMinVals[i] = dstCspace.GetMinValue(i);
				DstMaxVals[i] = dstCspace.GetMaxValue(i);
			}
		}

	}

}
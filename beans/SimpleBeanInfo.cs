/*
 * Copyright (c) 1996, 2015, Oracle and/or its affiliates. All rights reserved.
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

namespace java.beans
{


	/// <summary>
	/// This is a support class to make it easier for people to provide
	/// BeanInfo classes.
	/// <para>
	/// It defaults to providing "noop" information, and can be selectively
	/// overriden to provide more explicit information on chosen topics.
	/// When the introspector sees the "noop" values, it will apply low
	/// level introspection and design patterns to automatically analyze
	/// the target bean.
	/// </para>
	/// </summary>

	public class SimpleBeanInfo : BeanInfo
	{

		/// <summary>
		/// Deny knowledge about the class and customizer of the bean.
		/// You can override this if you wish to provide explicit info.
		/// </summary>
		public virtual BeanDescriptor BeanDescriptor
		{
			get
			{
				return null;
			}
		}

		/// <summary>
		/// Deny knowledge of properties. You can override this
		/// if you wish to provide explicit property info.
		/// </summary>
		public virtual PropertyDescriptor[] PropertyDescriptors
		{
			get
			{
				return null;
			}
		}

		/// <summary>
		/// Deny knowledge of a default property. You can override this
		/// if you wish to define a default property for the bean.
		/// </summary>
		public virtual int DefaultPropertyIndex
		{
			get
			{
				return -1;
			}
		}

		/// <summary>
		/// Deny knowledge of event sets. You can override this
		/// if you wish to provide explicit event set info.
		/// </summary>
		public virtual EventSetDescriptor[] EventSetDescriptors
		{
			get
			{
				return null;
			}
		}

		/// <summary>
		/// Deny knowledge of a default event. You can override this
		/// if you wish to define a default event for the bean.
		/// </summary>
		public virtual int DefaultEventIndex
		{
			get
			{
				return -1;
			}
		}

		/// <summary>
		/// Deny knowledge of methods. You can override this
		/// if you wish to provide explicit method info.
		/// </summary>
		public virtual MethodDescriptor[] MethodDescriptors
		{
			get
			{
				return null;
			}
		}

		/// <summary>
		/// Claim there are no other relevant BeanInfo objects.  You
		/// may override this if you want to (for example) return a
		/// BeanInfo for a base class.
		/// </summary>
		public virtual BeanInfo[] AdditionalBeanInfo
		{
			get
			{
				return null;
			}
		}

		/// <summary>
		/// Claim there are no icons available.  You can override
		/// this if you want to provide icons for your bean.
		/// </summary>
		public virtual Image GetIcon(int iconKind)
		{
			return null;
		}

		/// <summary>
		/// This is a utility method to help in loading icon images.
		/// It takes the name of a resource file associated with the
		/// current object's class file and loads an image object
		/// from that file.  Typically images will be GIFs.
		/// <para>
		/// </para>
		/// </summary>
		/// <param name="resourceName">  A pathname relative to the directory
		///          holding the class file of the current class.  For example,
		///          "wombat.gif". </param>
		/// <returns>  an image object.  May be null if the load failed. </returns>
//JAVA TO C# CONVERTER WARNING: 'final' parameters are not allowed in .NET:
//ORIGINAL LINE: public java.awt.Image loadImage(final String resourceName)
		public virtual Image LoadImage(String resourceName)
		{
			try
			{
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final java.net.URL url = getClass().getResource(resourceName);
				URL url = this.GetType().getResource(resourceName);
				if (url != null)
				{
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final java.awt.image.ImageProducer ip = (java.awt.image.ImageProducer) url.getContent();
					ImageProducer ip = (ImageProducer) url.Content;
					if (ip != null)
					{
						return Toolkit.DefaultToolkit.CreateImage(ip);
					}
				}
			}
//JAVA TO C# CONVERTER WARNING: 'final' catch parameters are not allowed in C#:
//ORIGINAL LINE: catch (final Exception ignored)
			catch (ception)
			{
			}
			return null;
		}
	}

}
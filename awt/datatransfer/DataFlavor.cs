using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading;

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

namespace java.awt.datatransfer
{

	using DataTransferer = sun.awt.datatransfer.DataTransferer;
	using ReflectUtil = sun.reflect.misc.ReflectUtil;


//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to .NET:
	import static sun.security.util.SecurityConstants.GET_CLASSLOADER_PERMISSION;

	/// <summary>
	/// A {@code DataFlavor} provides meta information about data. {@code DataFlavor}
	/// is typically used to access data on the clipboard, or during
	/// a drag and drop operation.
	/// <para>
	/// An instance of {@code DataFlavor} encapsulates a content type as
	/// defined in <a href="http://www.ietf.org/rfc/rfc2045.txt">RFC 2045</a>
	/// and <a href="http://www.ietf.org/rfc/rfc2046.txt">RFC 2046</a>.
	/// A content type is typically referred to as a MIME type.
	/// </para>
	/// <para>
	/// A content type consists of a media type (referred
	/// to as the primary type), a subtype, and optional parameters. See
	/// <a href="http://www.ietf.org/rfc/rfc2045.txt">RFC 2045</a>
	/// for details on the syntax of a MIME type.
	/// </para>
	/// <para>
	/// The JRE data transfer implementation interprets the parameter &quot;class&quot;
	/// of a MIME type as <B>a representation class</b>.
	/// The representation class reflects the class of the object being
	/// transferred. In other words, the representation class is the type of
	/// object returned by <seealso cref="Transferable#getTransferData"/>.
	/// For example, the MIME type of <seealso cref="#imageFlavor"/> is
	/// {@code "image/x-java-image;class=java.awt.Image"},
	/// the primary type is {@code image}, the subtype is
	/// {@code x-java-image}, and the representation class is
	/// {@code java.awt.Image}. When {@code getTransferData} is invoked
	/// with a {@code DataFlavor} of {@code imageFlavor}, an instance of
	/// {@code java.awt.Image} is returned.
	/// It's important to note that {@code DataFlavor} does no error checking
	/// against the representation class. It is up to consumers of
	/// {@code DataFlavor}, such as {@code Transferable}, to honor the representation
	/// class.
	/// <br>
	/// Note, if you do not specify a representation class when
	/// creating a {@code DataFlavor}, the default
	/// representation class is used. See appropriate documentation for
	/// {@code DataFlavor}'s constructors.
	/// </para>
	/// <para>
	/// Also, {@code DataFlavor} instances with the &quot;text&quot; primary
	/// MIME type may have a &quot;charset&quot; parameter. Refer to
	/// <a href="http://www.ietf.org/rfc/rfc2046.txt">RFC 2046</a> and
	/// <seealso cref="#selectBestTextFlavor"/> for details on &quot;text&quot; MIME types
	/// and the &quot;charset&quot; parameter.
	/// </para>
	/// <para>
	/// Equality of {@code DataFlavors} is determined by the primary type,
	/// subtype, and representation class. Refer to <seealso cref="#equals(DataFlavor)"/> for
	/// details. When determining equality, any optional parameters are ignored.
	/// For example, the following produces two {@code DataFlavors} that
	/// are considered identical:
	/// <pre>
	///   DataFlavor flavor1 = new DataFlavor(Object.class, &quot;X-test/test; class=&lt;java.lang.Object&gt;; foo=bar&quot;);
	///   DataFlavor flavor2 = new DataFlavor(Object.class, &quot;X-test/test; class=&lt;java.lang.Object&gt;; x=y&quot;);
	///   // The following returns true.
	///   flavor1.equals(flavor2);
	/// </pre>
	/// As mentioned, {@code flavor1} and {@code flavor2} are considered identical.
	/// As such, asking a {@code Transferable} for either {@code DataFlavor} returns
	/// the same results.
	/// </para>
	/// <para>
	/// For more information on the using data transfer with Swing see
	/// the <a href="https://docs.oracle.com/javase/tutorial/uiswing/dnd/index.html">
	/// How to Use Drag and Drop and Data Transfer</a>,
	/// section in <em>Java Tutorial</em>.
	/// 
	/// @author      Blake Sullivan
	/// @author      Laurence P. G. Cable
	/// @author      Jeff Dunn
	/// </para>
	/// </summary>
	[Serializable]
	public class DataFlavor : Externalizable, Cloneable
	{

		private const long SerialVersionUID = 8367026044764648243L;
		private static readonly Class IoInputStreamClass = typeof(InputStream);

		/// <summary>
		/// Tries to load a class from: the bootstrap loader, the system loader,
		/// the context loader (if one is present) and finally the loader specified.
		/// </summary>
		/// <param name="className"> the name of the class to be loaded </param>
		/// <param name="fallback"> the fallback loader </param>
		/// <returns> the class loaded </returns>
		/// <exception cref="ClassNotFoundException"> if class is not found </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: protected final static Class tryToLoadClass(String className, ClassLoader fallback) throws ClassNotFoundException
		protected internal static Class TryToLoadClass(String className, ClassLoader fallback)
		{
			ReflectUtil.checkPackageAccess(className);
			try
			{
				SecurityManager sm = System.SecurityManager;
				if (sm != null)
				{
					sm.CheckPermission(GET_CLASSLOADER_PERMISSION);
				}
				ClassLoader loader = ClassLoader.SystemClassLoader;
				try
				{
					// bootstrap class loader and system class loader if present
					return Class.ForName(className, true, loader);
				}
				catch (ClassNotFoundException)
				{
					// thread context class loader if and only if present
					loader = Thread.CurrentThread.ContextClassLoader;
					if (loader != null)
					{
						try
						{
							return Class.ForName(className, true, loader);
						}
						catch (ClassNotFoundException)
						{
							// fallback to user's class loader
						}
					}
				}
			}
			catch (SecurityException)
			{
				// ignore secured class loaders
			}
			return Class.ForName(className, true, fallback);
		}

		/*
		 * private initializer
		 */
		private static DataFlavor CreateConstant(Class rc, String prn)
		{
			try
			{
				return new DataFlavor(rc, prn);
			}
			catch (Exception)
			{
				return null;
			}
		}

		/*
		 * private initializer
		 */
		private static DataFlavor CreateConstant(String mt, String prn)
		{
			try
			{
				return new DataFlavor(mt, prn);
			}
			catch (Exception)
			{
				return null;
			}
		}

		/*
		 * private initializer
		 */
		private static DataFlavor InitHtmlDataFlavor(String htmlFlavorType)
		{
			try
			{
				return new DataFlavor("text/html; class=java.lang.String;document=" + htmlFlavorType + ";charset=Unicode");
			}
			catch (Exception)
			{
				return null;
			}
		}

		/// <summary>
		/// The <code>DataFlavor</code> representing a Java Unicode String class,
		/// where:
		/// <pre>
		///     representationClass = java.lang.String
		///     mimeType           = "application/x-java-serialized-object"
		/// </pre>
		/// </summary>
		public static readonly DataFlavor StringFlavor = CreateConstant(typeof(string), "Unicode String");

		/// <summary>
		/// The <code>DataFlavor</code> representing a Java Image class,
		/// where:
		/// <pre>
		///     representationClass = java.awt.Image
		///     mimeType            = "image/x-java-image"
		/// </pre>
		/// </summary>
		public static readonly DataFlavor ImageFlavor = CreateConstant("image/x-java-image; class=java.awt.Image", "Image");

		/// <summary>
		/// The <code>DataFlavor</code> representing plain text with Unicode
		/// encoding, where:
		/// <pre>
		///     representationClass = InputStream
		///     mimeType            = "text/plain; charset=unicode"
		/// </pre>
		/// This <code>DataFlavor</code> has been <b>deprecated</b> because
		/// (1) Its representation is an InputStream, an 8-bit based representation,
		/// while Unicode is a 16-bit character set; and (2) The charset "unicode"
		/// is not well-defined. "unicode" implies a particular platform's
		/// implementation of Unicode, not a cross-platform implementation.
		/// </summary>
		/// @deprecated as of 1.3. Use <code>DataFlavor.getReaderForText(Transferable)</code>
		///             instead of <code>Transferable.getTransferData(DataFlavor.plainTextFlavor)</code>. 
		[Obsolete("as of 1.3. Use <code>DataFlavor.getReaderForText(Transferable)</code>")]
		public static readonly DataFlavor PlainTextFlavor = CreateConstant("text/plain; charset=unicode; class=java.io.InputStream", "Plain Text");

		/// <summary>
		/// A MIME Content-Type of application/x-java-serialized-object represents
		/// a graph of Java object(s) that have been made persistent.
		/// 
		/// The representation class associated with this <code>DataFlavor</code>
		/// identifies the Java type of an object returned as a reference
		/// from an invocation <code>java.awt.datatransfer.getTransferData</code>.
		/// </summary>
		public const String JavaSerializedObjectMimeType = "application/x-java-serialized-object";

		/// <summary>
		/// To transfer a list of files to/from Java (and the underlying
		/// platform) a <code>DataFlavor</code> of this type/subtype and
		/// representation class of <code>java.util.List</code> is used.
		/// Each element of the list is required/guaranteed to be of type
		/// <code>java.io.File</code>.
		/// </summary>
		public static readonly DataFlavor JavaFileListFlavor = CreateConstant("application/x-java-file-list;class=java.util.List", null);

		/// <summary>
		/// To transfer a reference to an arbitrary Java object reference that
		/// has no associated MIME Content-type, across a <code>Transferable</code>
		/// interface WITHIN THE SAME JVM, a <code>DataFlavor</code>
		/// with this type/subtype is used, with a <code>representationClass</code>
		/// equal to the type of the class/interface being passed across the
		/// <code>Transferable</code>.
		/// <para>
		/// The object reference returned from
		/// <code>Transferable.getTransferData</code> for a <code>DataFlavor</code>
		/// with this MIME Content-Type is required to be
		/// an instance of the representation Class of the <code>DataFlavor</code>.
		/// </para>
		/// </summary>
		public const String JavaJVMLocalObjectMimeType = "application/x-java-jvm-local-objectref";

		/// <summary>
		/// In order to pass a live link to a Remote object via a Drag and Drop
		/// <code>ACTION_LINK</code> operation a Mime Content Type of
		/// application/x-java-remote-object should be used,
		/// where the representation class of the <code>DataFlavor</code>
		/// represents the type of the <code>Remote</code> interface to be
		/// transferred.
		/// </summary>
		public const String JavaRemoteObjectMimeType = "application/x-java-remote-object";

		/// <summary>
		/// Represents a piece of an HTML markup. The markup consists of the part
		/// selected on the source side. Therefore some tags in the markup may be
		/// unpaired. If the flavor is used to represent the data in
		/// a <seealso cref="Transferable"/> instance, no additional changes will be made.
		/// This DataFlavor instance represents the same HTML markup as DataFlavor
		/// instances which content MIME type does not contain document parameter
		/// and representation class is the String class.
		/// <pre>
		///     representationClass = String
		///     mimeType           = "text/html"
		/// </pre>
		/// </summary>
		public static DataFlavor SelectionHtmlFlavor = InitHtmlDataFlavor("selection");

		/// <summary>
		/// Represents a piece of an HTML markup. If possible, the markup received
		/// from a native system is supplemented with pair tags to be
		/// a well-formed HTML markup. If the flavor is used to represent the data in
		/// a <seealso cref="Transferable"/> instance, no additional changes will be made.
		/// <pre>
		///     representationClass = String
		///     mimeType           = "text/html"
		/// </pre>
		/// </summary>
		public static DataFlavor FragmentHtmlFlavor = InitHtmlDataFlavor("fragment");

		/// <summary>
		/// Represents a piece of an HTML markup. If possible, the markup
		/// received from a native system is supplemented with additional
		/// tags to make up a well-formed HTML document. If the flavor is used to
		/// represent the data in a <seealso cref="Transferable"/> instance,
		/// no additional changes will be made.
		/// <pre>
		///     representationClass = String
		///     mimeType           = "text/html"
		/// </pre>
		/// </summary>
		public static DataFlavor AllHtmlFlavor = InitHtmlDataFlavor("all");

		/// <summary>
		/// Constructs a new <code>DataFlavor</code>.  This constructor is
		/// provided only for the purpose of supporting the
		/// <code>Externalizable</code> interface.  It is not
		/// intended for public (client) use.
		/// 
		/// @since 1.2
		/// </summary>
		public DataFlavor() : base()
		{
		}

		/// <summary>
		/// Constructs a fully specified <code>DataFlavor</code>.
		/// </summary>
		/// <exception cref="NullPointerException"> if either <code>primaryType</code>,
		///            <code>subType</code> or <code>representationClass</code> is null </exception>
		private DataFlavor(String primaryType, String subType, MimeTypeParameterList @params, Class representationClass, String humanPresentableName) : base()
		{
			if (primaryType == null)
			{
				throw new NullPointerException("primaryType");
			}
			if (subType == null)
			{
				throw new NullPointerException("subType");
			}
			if (representationClass == null)
			{
				throw new NullPointerException("representationClass");
			}

			if (@params == null)
			{
				@params = new MimeTypeParameterList();
			}

			@params.Set("class", representationClass.Name);

			if (humanPresentableName == null)
			{
				humanPresentableName = @params.Get("humanPresentableName");

				if (humanPresentableName == null)
				{
					humanPresentableName = primaryType + "/" + subType;
				}
			}

			try
			{
				MimeType_Renamed = new MimeType(primaryType, subType, @params);
			}
			catch (MimeTypeParseException mtpe)
			{
				throw new IllegalArgumentException("MimeType Parse Exception: " + mtpe.Message);
			}

			this.RepresentationClass_Renamed = representationClass;
			this.HumanPresentableName_Renamed = humanPresentableName;

			MimeType_Renamed.RemoveParameter("humanPresentableName");
		}

		/// <summary>
		/// Constructs a <code>DataFlavor</code> that represents a Java class.
		/// <para>
		/// The returned <code>DataFlavor</code> will have the following
		/// characteristics:
		/// <pre>
		///    representationClass = representationClass
		///    mimeType            = application/x-java-serialized-object
		/// </pre>
		/// </para>
		/// </summary>
		/// <param name="representationClass"> the class used to transfer data in this flavor </param>
		/// <param name="humanPresentableName"> the human-readable string used to identify
		///                 this flavor; if this parameter is <code>null</code>
		///                 then the value of the the MIME Content Type is used </param>
		/// <exception cref="NullPointerException"> if <code>representationClass</code> is null </exception>
		public DataFlavor(Class representationClass, String humanPresentableName) : this("application", "x-java-serialized-object", null, representationClass, humanPresentableName)
		{
			if (representationClass == null)
			{
				throw new NullPointerException("representationClass");
			}
		}

		/// <summary>
		/// Constructs a <code>DataFlavor</code> that represents a
		/// <code>MimeType</code>.
		/// <para>
		/// The returned <code>DataFlavor</code> will have the following
		/// characteristics:
		/// </para>
		/// <para>
		/// If the <code>mimeType</code> is
		/// "application/x-java-serialized-object; class=&lt;representation class&gt;",
		/// the result is the same as calling
		/// <code>new DataFlavor(Class:forName(&lt;representation class&gt;)</code>.
		/// </para>
		/// <para>
		/// Otherwise:
		/// <pre>
		///     representationClass = InputStream
		///     mimeType            = mimeType
		/// </pre>
		/// </para>
		/// </summary>
		/// <param name="mimeType"> the string used to identify the MIME type for this flavor;
		///                 if the the <code>mimeType</code> does not specify a
		///                 "class=" parameter, or if the class is not successfully
		///                 loaded, then an <code>IllegalArgumentException</code>
		///                 is thrown </param>
		/// <param name="humanPresentableName"> the human-readable string used to identify
		///                 this flavor; if this parameter is <code>null</code>
		///                 then the value of the the MIME Content Type is used </param>
		/// <exception cref="IllegalArgumentException"> if <code>mimeType</code> is
		///                 invalid or if the class is not successfully loaded </exception>
		/// <exception cref="NullPointerException"> if <code>mimeType</code> is null </exception>
		public DataFlavor(String mimeType, String humanPresentableName) : base()
		{
			if (mimeType == null)
			{
				throw new NullPointerException("mimeType");
			}
			try
			{
				Initialize(mimeType, humanPresentableName, this.GetType().ClassLoader);
			}
			catch (MimeTypeParseException)
			{
				throw new IllegalArgumentException("failed to parse:" + mimeType);
			}
			catch (ClassNotFoundException cnfe)
			{
				throw new IllegalArgumentException("can't find specified class: " + cnfe.Message);
			}
		}

		/// <summary>
		/// Constructs a <code>DataFlavor</code> that represents a
		/// <code>MimeType</code>.
		/// <para>
		/// The returned <code>DataFlavor</code> will have the following
		/// characteristics:
		/// </para>
		/// <para>
		/// If the mimeType is
		/// "application/x-java-serialized-object; class=&lt;representation class&gt;",
		/// the result is the same as calling
		/// <code>new DataFlavor(Class:forName(&lt;representation class&gt;)</code>.
		/// </para>
		/// <para>
		/// Otherwise:
		/// <pre>
		///     representationClass = InputStream
		///     mimeType            = mimeType
		/// </pre>
		/// </para>
		/// </summary>
		/// <param name="mimeType"> the string used to identify the MIME type for this flavor </param>
		/// <param name="humanPresentableName"> the human-readable string used to
		///          identify this flavor </param>
		/// <param name="classLoader"> the class loader to use </param>
		/// <exception cref="ClassNotFoundException"> if the class is not loaded </exception>
		/// <exception cref="IllegalArgumentException"> if <code>mimeType</code> is
		///                 invalid </exception>
		/// <exception cref="NullPointerException"> if <code>mimeType</code> is null </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public DataFlavor(String mimeType, String humanPresentableName, ClassLoader classLoader) throws ClassNotFoundException
		public DataFlavor(String mimeType, String humanPresentableName, ClassLoader classLoader) : base()
		{
			if (mimeType == null)
			{
				throw new NullPointerException("mimeType");
			}
			try
			{
				Initialize(mimeType, humanPresentableName, classLoader);
			}
			catch (MimeTypeParseException)
			{
				throw new IllegalArgumentException("failed to parse:" + mimeType);
			}
		}

		/// <summary>
		/// Constructs a <code>DataFlavor</code> from a <code>mimeType</code> string.
		/// The string can specify a "class=&lt;fully specified Java class name&gt;"
		/// parameter to create a <code>DataFlavor</code> with the desired
		/// representation class. If the string does not contain "class=" parameter,
		/// <code>java.io.InputStream</code> is used as default.
		/// </summary>
		/// <param name="mimeType"> the string used to identify the MIME type for this flavor;
		///                 if the class specified by "class=" parameter is not
		///                 successfully loaded, then an
		///                 <code>ClassNotFoundException</code> is thrown </param>
		/// <exception cref="ClassNotFoundException"> if the class is not loaded </exception>
		/// <exception cref="IllegalArgumentException"> if <code>mimeType</code> is
		///                 invalid </exception>
		/// <exception cref="NullPointerException"> if <code>mimeType</code> is null </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public DataFlavor(String mimeType) throws ClassNotFoundException
		public DataFlavor(String mimeType) : base()
		{
			if (mimeType == null)
			{
				throw new NullPointerException("mimeType");
			}
			try
			{
				Initialize(mimeType, null, this.GetType().ClassLoader);
			}
			catch (MimeTypeParseException)
			{
				throw new IllegalArgumentException("failed to parse:" + mimeType);
			}
		}

	   /// <summary>
	   /// Common initialization code called from various constructors.
	   /// </summary>
	   /// <param name="mimeType"> the MIME Content Type (must have a class= param) </param>
	   /// <param name="humanPresentableName"> the human Presentable Name or
	   ///                 <code>null</code> </param>
	   /// <param name="classLoader"> the fallback class loader to resolve against
	   /// </param>
	   /// <exception cref="MimeTypeParseException"> </exception>
	   /// <exception cref="ClassNotFoundException"> </exception>
	   /// <exception cref="NullPointerException"> if <code>mimeType</code> is null
	   /// </exception>
	   /// <seealso cref= #tryToLoadClass </seealso>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: private void initialize(String mimeType, String humanPresentableName, ClassLoader classLoader) throws MimeTypeParseException, ClassNotFoundException
		private void Initialize(String mimeType, String humanPresentableName, ClassLoader classLoader)
		{
			if (mimeType == null)
			{
				throw new NullPointerException("mimeType");
			}

			this.MimeType_Renamed = new MimeType(mimeType); // throws

			String rcn = GetParameter("class");

			if (rcn == null)
			{
				if ("application/x-java-serialized-object".Equals(this.MimeType_Renamed.BaseType))

				{
					throw new IllegalArgumentException("no representation class specified for:" + mimeType);
				}
				else
				{
					RepresentationClass_Renamed = typeof(InputStream); // default
				}
			} // got a class name
			else
			{
				RepresentationClass_Renamed = DataFlavor.TryToLoadClass(rcn, classLoader);
			}

			this.MimeType_Renamed.SetParameter("class", RepresentationClass_Renamed.Name);

			if (humanPresentableName == null)
			{
				humanPresentableName = this.MimeType_Renamed.GetParameter("humanPresentableName");
				if (humanPresentableName == null)
				{
					humanPresentableName = this.MimeType_Renamed.PrimaryType + "/" + this.MimeType_Renamed.SubType;
				}
			}

			this.HumanPresentableName_Renamed = humanPresentableName; // set it.

			this.MimeType_Renamed.RemoveParameter("humanPresentableName"); // just in case
		}

		/// <summary>
		/// String representation of this <code>DataFlavor</code> and its
		/// parameters. The resulting <code>String</code> contains the name of
		/// the <code>DataFlavor</code> class, this flavor's MIME type, and its
		/// representation class. If this flavor has a primary MIME type of "text",
		/// supports the charset parameter, and has an encoded representation, the
		/// flavor's charset is also included. See <code>selectBestTextFlavor</code>
		/// for a list of text flavors which support the charset parameter.
		/// </summary>
		/// <returns>  string representation of this <code>DataFlavor</code> </returns>
		/// <seealso cref= #selectBestTextFlavor </seealso>
		public override String ToString()
		{
//JAVA TO C# CONVERTER WARNING: The .NET Type.FullName property will not always yield results identical to the Java Class.getName method:
			String @string = this.GetType().FullName;
			@string += "[" + ParamString() + "]";
			return @string;
		}

		private String ParamString()
		{
			String @params = "";
			@params += "mimetype=";
			if (MimeType_Renamed == null)
			{
				@params += "null";
			}
			else
			{
				@params += MimeType_Renamed.BaseType;
			}
			@params += ";representationclass=";
			if (RepresentationClass_Renamed == null)
			{
			   @params += "null";
			}
			else
			{
			   @params += RepresentationClass_Renamed.Name;
			}
			if (DataTransferer.isFlavorCharsetTextType(this) && (RepresentationClassInputStream || RepresentationClassByteBuffer || typeof(sbyte[]).Equals(RepresentationClass_Renamed)))
			{
				@params += ";charset=" + DataTransferer.getTextCharset(this);
			}
			return @params;
		}

		/// <summary>
		/// Returns a <code>DataFlavor</code> representing plain text with Unicode
		/// encoding, where:
		/// <pre>
		///     representationClass = java.io.InputStream
		///     mimeType            = "text/plain;
		///                            charset=&lt;platform default Unicode encoding&gt;"
		/// </pre>
		/// Sun's implementation for Microsoft Windows uses the encoding <code>utf-16le</code>.
		/// Sun's implementation for Solaris and Linux uses the encoding
		/// <code>iso-10646-ucs-2</code>.
		/// </summary>
		/// <returns> a <code>DataFlavor</code> representing plain text
		///    with Unicode encoding
		/// @since 1.3 </returns>
		public static DataFlavor TextPlainUnicodeFlavor
		{
			get
			{
				String encoding = null;
				DataTransferer transferer = DataTransferer.Instance;
				if (transferer != null)
				{
					encoding = transferer.DefaultUnicodeEncoding;
				}
				return new DataFlavor("text/plain;charset=" + encoding + ";class=java.io.InputStream", "Plain Text");
			}
		}

		/// <summary>
		/// Selects the best text <code>DataFlavor</code> from an array of <code>
		/// DataFlavor</code>s. Only <code>DataFlavor.stringFlavor</code>, and
		/// equivalent flavors, and flavors that have a primary MIME type of "text",
		/// are considered for selection.
		/// <para>
		/// Flavors are first sorted by their MIME types in the following order:
		/// <ul>
		/// <li>"text/sgml"
		/// <li>"text/xml"
		/// <li>"text/html"
		/// <li>"text/rtf"
		/// <li>"text/enriched"
		/// <li>"text/richtext"
		/// <li>"text/uri-list"
		/// <li>"text/tab-separated-values"
		/// <li>"text/t140"
		/// <li>"text/rfc822-headers"
		/// <li>"text/parityfec"
		/// <li>"text/directory"
		/// <li>"text/css"
		/// <li>"text/calendar"
		/// <li>"application/x-java-serialized-object"
		/// <li>"text/plain"
		/// <li>"text/&lt;other&gt;"
		/// </ul>
		/// </para>
		/// <para>For example, "text/sgml" will be selected over
		/// "text/html", and <code>DataFlavor.stringFlavor</code> will be chosen
		/// over <code>DataFlavor.plainTextFlavor</code>.
		/// </para>
		/// <para>
		/// If two or more flavors share the best MIME type in the array, then that
		/// MIME type will be checked to see if it supports the charset parameter.
		/// </para>
		/// <para>
		/// The following MIME types support, or are treated as though they support,
		/// the charset parameter:
		/// <ul>
		/// <li>"text/sgml"
		/// <li>"text/xml"
		/// <li>"text/html"
		/// <li>"text/enriched"
		/// <li>"text/richtext"
		/// <li>"text/uri-list"
		/// <li>"text/directory"
		/// <li>"text/css"
		/// <li>"text/calendar"
		/// <li>"application/x-java-serialized-object"
		/// <li>"text/plain"
		/// </ul>
		/// The following MIME types do not support, or are treated as though they
		/// do not support, the charset parameter:
		/// <ul>
		/// <li>"text/rtf"
		/// <li>"text/tab-separated-values"
		/// <li>"text/t140"
		/// <li>"text/rfc822-headers"
		/// <li>"text/parityfec"
		/// </ul>
		/// For "text/&lt;other&gt;" MIME types, the first time the JRE needs to
		/// determine whether the MIME type supports the charset parameter, it will
		/// check whether the parameter is explicitly listed in an arbitrarily
		/// chosen <code>DataFlavor</code> which uses that MIME type. If so, the JRE
		/// will assume from that point on that the MIME type supports the charset
		/// parameter and will not check again. If the parameter is not explicitly
		/// listed, the JRE will assume from that point on that the MIME type does
		/// not support the charset parameter and will not check again. Because
		/// this check is performed on an arbitrarily chosen
		/// <code>DataFlavor</code>, developers must ensure that all
		/// <code>DataFlavor</code>s with a "text/&lt;other&gt;" MIME type specify
		/// the charset parameter if it is supported by that MIME type. Developers
		/// should never rely on the JRE to substitute the platform's default
		/// charset for a "text/&lt;other&gt;" DataFlavor. Failure to adhere to this
		/// restriction will lead to undefined behavior.
		/// </para>
		/// <para>
		/// If the best MIME type in the array does not support the charset
		/// parameter, the flavors which share that MIME type will then be sorted by
		/// their representation classes in the following order:
		/// <code>java.io.InputStream</code>, <code>java.nio.ByteBuffer</code>,
		/// <code>[B</code>, &lt;all others&gt;.
		/// </para>
		/// <para>
		/// If two or more flavors share the best representation class, or if no
		/// flavor has one of the three specified representations, then one of those
		/// flavors will be chosen non-deterministically.
		/// </para>
		/// <para>
		/// If the best MIME type in the array does support the charset parameter,
		/// the flavors which share that MIME type will then be sorted by their
		/// representation classes in the following order:
		/// <code>java.io.Reader</code>, <code>java.lang.String</code>,
		/// <code>java.nio.CharBuffer</code>, <code>[C</code>, &lt;all others&gt;.
		/// </para>
		/// <para>
		/// If two or more flavors share the best representation class, and that
		/// representation is one of the four explicitly listed, then one of those
		/// flavors will be chosen non-deterministically. If, however, no flavor has
		/// one of the four specified representations, the flavors will then be
		/// sorted by their charsets. Unicode charsets, such as "UTF-16", "UTF-8",
		/// "UTF-16BE", "UTF-16LE", and their aliases, are considered best. After
		/// them, the platform default charset and its aliases are selected.
		/// "US-ASCII" and its aliases are worst. All other charsets are chosen in
		/// alphabetical order, but only charsets supported by this implementation
		/// of the Java platform will be considered.
		/// </para>
		/// <para>
		/// If two or more flavors share the best charset, the flavors will then
		/// again be sorted by their representation classes in the following order:
		/// <code>java.io.InputStream</code>, <code>java.nio.ByteBuffer</code>,
		/// <code>[B</code>, &lt;all others&gt;.
		/// </para>
		/// <para>
		/// If two or more flavors share the best representation class, or if no
		/// flavor has one of the three specified representations, then one of those
		/// flavors will be chosen non-deterministically.
		/// 
		/// </para>
		/// </summary>
		/// <param name="availableFlavors"> an array of available <code>DataFlavor</code>s </param>
		/// <returns> the best (highest fidelity) flavor according to the rules
		///         specified above, or <code>null</code>,
		///         if <code>availableFlavors</code> is <code>null</code>,
		///         has zero length, or contains no text flavors
		/// @since 1.3 </returns>
		public static DataFlavor SelectBestTextFlavor(DataFlavor[] availableFlavors)
		{
			if (availableFlavors == null || availableFlavors.Length == 0)
			{
				return null;
			}

			if (TextFlavorComparator == null)
			{
				TextFlavorComparator = new TextFlavorComparator();
			}

			DataFlavor bestFlavor = (DataFlavor)Collections.Max(Arrays.AsList(availableFlavors), TextFlavorComparator);

			if (!bestFlavor.FlavorTextType)
			{
				return null;
			}

			return bestFlavor;
		}

		private static IComparer<DataFlavor> TextFlavorComparator;

		internal class TextFlavorComparator : DataTransferer.DataFlavorComparator
		{

			/// <summary>
			/// Compares two <code>DataFlavor</code> objects. Returns a negative
			/// integer, zero, or a positive integer as the first
			/// <code>DataFlavor</code> is worse than, equal to, or better than the
			/// second.
			/// <para>
			/// <code>DataFlavor</code>s are ordered according to the rules outlined
			/// for <code>selectBestTextFlavor</code>.
			/// 
			/// </para>
			/// </summary>
			/// <param name="obj1"> the first <code>DataFlavor</code> to be compared </param>
			/// <param name="obj2"> the second <code>DataFlavor</code> to be compared </param>
			/// <returns> a negative integer, zero, or a positive integer as the first
			///         argument is worse, equal to, or better than the second </returns>
			/// <exception cref="ClassCastException"> if either of the arguments is not an
			///         instance of <code>DataFlavor</code> </exception>
			/// <exception cref="NullPointerException"> if either of the arguments is
			///         <code>null</code>
			/// </exception>
			/// <seealso cref= #selectBestTextFlavor </seealso>
			public virtual int Compare(Object obj1, Object obj2)
			{
				DataFlavor flavor1 = (DataFlavor)obj1;
				DataFlavor flavor2 = (DataFlavor)obj2;

				if (flavor1.FlavorTextType)
				{
					if (flavor2.FlavorTextType)
					{
						return base.Compare(obj1, obj2);
					}
					else
					{
						return 1;
					}
				}
				else if (flavor2.FlavorTextType)
				{
					return -1;
				}
				else
				{
					return 0;
				}
			}
		}

		/// <summary>
		/// Gets a Reader for a text flavor, decoded, if necessary, for the expected
		/// charset (encoding). The supported representation classes are
		/// <code>java.io.Reader</code>, <code>java.lang.String</code>,
		/// <code>java.nio.CharBuffer</code>, <code>[C</code>,
		/// <code>java.io.InputStream</code>, <code>java.nio.ByteBuffer</code>,
		/// and <code>[B</code>.
		/// <para>
		/// Because text flavors which do not support the charset parameter are
		/// encoded in a non-standard format, this method should not be called for
		/// such flavors. However, in order to maintain backward-compatibility,
		/// if this method is called for such a flavor, this method will treat the
		/// flavor as though it supports the charset parameter and attempt to
		/// decode it accordingly. See <code>selectBestTextFlavor</code> for a list
		/// of text flavors which do not support the charset parameter.
		/// 
		/// </para>
		/// </summary>
		/// <param name="transferable"> the <code>Transferable</code> whose data will be
		///        requested in this flavor
		/// </param>
		/// <returns> a <code>Reader</code> to read the <code>Transferable</code>'s
		///         data
		/// </returns>
		/// <exception cref="IllegalArgumentException"> if the representation class
		///            is not one of the seven listed above </exception>
		/// <exception cref="IllegalArgumentException"> if the <code>Transferable</code>
		///            has <code>null</code> data </exception>
		/// <exception cref="NullPointerException"> if the <code>Transferable</code> is
		///            <code>null</code> </exception>
		/// <exception cref="UnsupportedEncodingException"> if this flavor's representation
		///            is <code>java.io.InputStream</code>,
		///            <code>java.nio.ByteBuffer</code>, or <code>[B</code> and
		///            this flavor's encoding is not supported by this
		///            implementation of the Java platform </exception>
		/// <exception cref="UnsupportedFlavorException"> if the <code>Transferable</code>
		///            does not support this flavor </exception>
		/// <exception cref="IOException"> if the data cannot be read because of an
		///            I/O error </exception>
		/// <seealso cref= #selectBestTextFlavor
		/// @since 1.3 </seealso>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public java.io.Reader getReaderForText(Transferable transferable) throws UnsupportedFlavorException, java.io.IOException
		public virtual Reader GetReaderForText(Transferable transferable)
		{
			Object transferObject = transferable.GetTransferData(this);
			if (transferObject == null)
			{
				throw new IllegalArgumentException("getTransferData() returned null");
			}

			if (transferObject is Reader)
			{
				return (Reader)transferObject;
			}
			else if (transferObject is String)
			{
				return new StringReader((String)transferObject);
			}
			else if (transferObject is CharBuffer)
			{
				CharBuffer buffer = (CharBuffer)transferObject;
				int size = buffer.Remaining();
				char[] chars = new char[size];
				buffer.Get(chars, 0, size);
				return new CharArrayReader(chars);
			}
			else if (transferObject is char[])
			{
				return new CharArrayReader((char[])transferObject);
			}

			InputStream stream = null;

			if (transferObject is InputStream)
			{
				stream = (InputStream)transferObject;
			}
			else if (transferObject is ByteBuffer)
			{
				ByteBuffer buffer = (ByteBuffer)transferObject;
				int size = buffer.Remaining();
				sbyte[] bytes = new sbyte[size];
				buffer.Get(bytes, 0, size);
				stream = new ByteArrayInputStream(bytes);
			}
			else if (transferObject is sbyte[])
			{
				stream = new ByteArrayInputStream((sbyte[])transferObject);
			}

			if (stream == null)
			{
				throw new IllegalArgumentException("transfer data is not Reader, String, CharBuffer, char array, InputStream, ByteBuffer, or byte array");
			}

			String encoding = GetParameter("charset");
			return (encoding == null) ? new InputStreamReader(stream) : new InputStreamReader(stream, encoding);
		}

		/// <summary>
		/// Returns the MIME type string for this <code>DataFlavor</code>. </summary>
		/// <returns> the MIME type string for this flavor </returns>
		public virtual String MimeType
		{
			get
			{
				return (MimeType_Renamed != null) ? MimeType_Renamed.ToString() : null;
			}
		}

		/// <summary>
		/// Returns the <code>Class</code> which objects supporting this
		/// <code>DataFlavor</code> will return when this <code>DataFlavor</code>
		/// is requested. </summary>
		/// <returns> the <code>Class</code> which objects supporting this
		/// <code>DataFlavor</code> will return when this <code>DataFlavor</code>
		/// is requested </returns>
		public virtual Class RepresentationClass
		{
			get
			{
				return RepresentationClass_Renamed;
			}
		}

		/// <summary>
		/// Returns the human presentable name for the data format that this
		/// <code>DataFlavor</code> represents.  This name would be localized
		/// for different countries. </summary>
		/// <returns> the human presentable name for the data format that this
		///    <code>DataFlavor</code> represents </returns>
		public virtual String HumanPresentableName
		{
			get
			{
				return HumanPresentableName_Renamed;
			}
			set
			{
				this.HumanPresentableName_Renamed = value;
			}
		}

		/// <summary>
		/// Returns the primary MIME type for this <code>DataFlavor</code>. </summary>
		/// <returns> the primary MIME type of this <code>DataFlavor</code> </returns>
		public virtual String PrimaryType
		{
			get
			{
				return (MimeType_Renamed != null) ? MimeType_Renamed.PrimaryType : null;
			}
		}

		/// <summary>
		/// Returns the sub MIME type of this <code>DataFlavor</code>. </summary>
		/// <returns> the Sub MIME type of this <code>DataFlavor</code> </returns>
		public virtual String SubType
		{
			get
			{
				return (MimeType_Renamed != null) ? MimeType_Renamed.SubType : null;
			}
		}

		/// <summary>
		/// Returns the human presentable name for this <code>DataFlavor</code>
		/// if <code>paramName</code> equals "humanPresentableName".  Otherwise
		/// returns the MIME type value associated with <code>paramName</code>.
		/// </summary>
		/// <param name="paramName"> the parameter name requested </param>
		/// <returns> the value of the name parameter, or <code>null</code>
		///  if there is no associated value </returns>
		public virtual String GetParameter(String paramName)
		{
			if (paramName.Equals("humanPresentableName"))
			{
				return HumanPresentableName_Renamed;
			}
			else
			{
				return (MimeType_Renamed != null) ? MimeType_Renamed.GetParameter(paramName) : null;
			}
		}


		/// <summary>
		/// {@inheritDoc}
		/// <para>
		/// The equals comparison for the {@code DataFlavor} class is implemented
		/// as follows: Two <code>DataFlavor</code>s are considered equal if and
		/// only if their MIME primary type and subtype and representation class are
		/// equal. Additionally, if the primary type is "text", the subtype denotes
		/// a text flavor which supports the charset parameter, and the
		/// representation class is not <code>java.io.Reader</code>,
		/// <code>java.lang.String</code>, <code>java.nio.CharBuffer</code>, or
		/// <code>[C</code>, the <code>charset</code> parameter must also be equal.
		/// If a charset is not explicitly specified for one or both
		/// <code>DataFlavor</code>s, the platform default encoding is assumed. See
		/// <code>selectBestTextFlavor</code> for a list of text flavors which
		/// support the charset parameter.
		/// 
		/// </para>
		/// </summary>
		/// <param name="o"> the <code>Object</code> to compare with <code>this</code> </param>
		/// <returns> <code>true</code> if <code>that</code> is equivalent to this
		///         <code>DataFlavor</code>; <code>false</code> otherwise </returns>
		/// <seealso cref= #selectBestTextFlavor </seealso>
		public override bool Equals(Object o)
		{
			return ((o is DataFlavor) && Equals((DataFlavor)o));
		}

		/// <summary>
		/// This method has the same behavior as <seealso cref="#equals(Object)"/>.
		/// The only difference being that it takes a {@code DataFlavor} instance
		/// as a parameter.
		/// </summary>
		/// <param name="that"> the <code>DataFlavor</code> to compare with
		///        <code>this</code> </param>
		/// <returns> <code>true</code> if <code>that</code> is equivalent to this
		///         <code>DataFlavor</code>; <code>false</code> otherwise </returns>
		/// <seealso cref= #selectBestTextFlavor </seealso>
		public virtual bool Equals(DataFlavor that)
		{
			if (that == null)
			{
				return false;
			}
			if (this == that)
			{
				return true;
			}

			if (!Objects.Equals(this.RepresentationClass, that.RepresentationClass))
			{
				return false;
			}

			if (MimeType_Renamed == null)
			{
				if (that.MimeType_Renamed != null)
				{
					return false;
				}
			}
			else
			{
				if (!MimeType_Renamed.Match(that.MimeType_Renamed))
				{
					return false;
				}

				if ("text".Equals(PrimaryType))
				{
					if (DataTransferer.doesSubtypeSupportCharset(this) && RepresentationClass_Renamed != null && !StandardTextRepresentationClass)
					{
						String thisCharset = DataTransferer.canonicalName(this.GetParameter("charset"));
						String thatCharset = DataTransferer.canonicalName(that.GetParameter("charset"));
						if (!Objects.Equals(thisCharset, thatCharset))
						{
							return false;
						}
					}

					if ("html".Equals(SubType))
					{
						String thisDocument = this.GetParameter("document");
						String thatDocument = that.GetParameter("document");
						if (!Objects.Equals(thisDocument, thatDocument))
						{
							return false;
						}
					}
				}
			}

			return true;
		}

		/// <summary>
		/// Compares only the <code>mimeType</code> against the passed in
		/// <code>String</code> and <code>representationClass</code> is
		/// not considered in the comparison.
		/// 
		/// If <code>representationClass</code> needs to be compared, then
		/// <code>equals(new DataFlavor(s))</code> may be used. </summary>
		/// @deprecated As inconsistent with <code>hashCode()</code> contract,
		///             use <code>isMimeTypeEqual(String)</code> instead. 
		/// <param name="s"> the {@code mimeType} to compare. </param>
		/// <returns> true if the String (MimeType) is equal; false otherwise or if
		///         {@code s} is {@code null} </returns>
		[Obsolete("As inconsistent with <code>hashCode()</code> contract,")]
		public virtual bool Equals(String s)
		{
			if (s == null || MimeType_Renamed == null)
			{
				return false;
			}
			return IsMimeTypeEqual(s);
		}

		/// <summary>
		/// Returns hash code for this <code>DataFlavor</code>.
		/// For two equal <code>DataFlavor</code>s, hash codes are equal.
		/// For the <code>String</code>
		/// that matches <code>DataFlavor.equals(String)</code>, it is not
		/// guaranteed that <code>DataFlavor</code>'s hash code is equal
		/// to the hash code of the <code>String</code>.
		/// </summary>
		/// <returns> a hash code for this <code>DataFlavor</code> </returns>
		public override int HashCode()
		{
			int total = 0;

			if (RepresentationClass_Renamed != null)
			{
				total += RepresentationClass_Renamed.HashCode();
			}

			if (MimeType_Renamed != null)
			{
				String primaryType = MimeType_Renamed.PrimaryType;
				if (primaryType != null)
				{
					total += primaryType.HashCode();
				}

				// Do not add subType.hashCode() to the total. equals uses
				// MimeType.match which reports a match if one or both of the
				// subTypes is '*', regardless of the other subType.

				if ("text".Equals(primaryType))
				{
					if (DataTransferer.doesSubtypeSupportCharset(this) && RepresentationClass_Renamed != null && !StandardTextRepresentationClass)
					{
						String charset = DataTransferer.canonicalName(GetParameter("charset"));
						if (charset != null)
						{
							total += charset.HashCode();
						}
					}

					if ("html".Equals(SubType))
					{
						String document = this.GetParameter("document");
						if (document != null)
						{
							total += document.HashCode();
						}
					}
				}
			}

			return total;
		}

		/// <summary>
		/// Identical to <seealso cref="#equals(DataFlavor)"/>.
		/// </summary>
		/// <param name="that"> the <code>DataFlavor</code> to compare with
		///        <code>this</code> </param>
		/// <returns> <code>true</code> if <code>that</code> is equivalent to this
		///         <code>DataFlavor</code>; <code>false</code> otherwise </returns>
		/// <seealso cref= #selectBestTextFlavor
		/// @since 1.3 </seealso>
		public virtual bool Match(DataFlavor that)
		{
			return Equals(that);
		}

		/// <summary>
		/// Returns whether the string representation of the MIME type passed in
		/// is equivalent to the MIME type of this <code>DataFlavor</code>.
		/// Parameters are not included in the comparison.
		/// </summary>
		/// <param name="mimeType"> the string representation of the MIME type </param>
		/// <returns> true if the string representation of the MIME type passed in is
		///         equivalent to the MIME type of this <code>DataFlavor</code>;
		///         false otherwise </returns>
		/// <exception cref="NullPointerException"> if mimeType is <code>null</code> </exception>
		public virtual bool IsMimeTypeEqual(String mimeType)
		{
			// JCK Test DataFlavor0117: if 'mimeType' is null, throw NPE
			if (mimeType == null)
			{
				throw new NullPointerException("mimeType");
			}
			if (this.MimeType_Renamed == null)
			{
				return false;
			}
			try
			{
				return this.MimeType_Renamed.Match(new MimeType(mimeType));
			}
			catch (MimeTypeParseException)
			{
				return false;
			}
		}

		/// <summary>
		/// Compares the <code>mimeType</code> of two <code>DataFlavor</code>
		/// objects. No parameters are considered.
		/// </summary>
		/// <param name="dataFlavor"> the <code>DataFlavor</code> to be compared </param>
		/// <returns> true if the <code>MimeType</code>s are equal,
		///  otherwise false </returns>

		public bool IsMimeTypeEqual(DataFlavor dataFlavor)
		{
			return IsMimeTypeEqual(dataFlavor.MimeType_Renamed);
		}

		/// <summary>
		/// Compares the <code>mimeType</code> of two <code>DataFlavor</code>
		/// objects.  No parameters are considered.
		/// </summary>
		/// <returns> true if the <code>MimeType</code>s are equal,
		///  otherwise false </returns>

		private bool IsMimeTypeEqual(MimeType mtype)
		{
			if (this.MimeType_Renamed == null)
			{
				return (mtype == null);
			}
			return MimeType_Renamed.Match(mtype);
		}

		/// <summary>
		/// Checks if the representation class is one of the standard text
		/// representation classes.
		/// </summary>
		/// <returns> true if the representation class is one of the standard text
		///              representation classes, otherwise false </returns>
		private bool StandardTextRepresentationClass
		{
			get
			{
				return RepresentationClassReader || typeof(String).Equals(RepresentationClass_Renamed) || RepresentationClassCharBuffer || typeof(char[]).Equals(RepresentationClass_Renamed);
			}
		}

	   /// <summary>
	   /// Does the <code>DataFlavor</code> represent a serialized object?
	   /// </summary>

		public virtual bool MimeTypeSerializedObject
		{
			get
			{
				return IsMimeTypeEqual(JavaSerializedObjectMimeType);
			}
		}

		public Class DefaultRepresentationClass
		{
			get
			{
				return IoInputStreamClass;
			}
		}

		public String DefaultRepresentationClassAsString
		{
			get
			{
				return DefaultRepresentationClass.Name;
			}
		}

	   /// <summary>
	   /// Does the <code>DataFlavor</code> represent a
	   /// <code>java.io.InputStream</code>?
	   /// </summary>

		public virtual bool RepresentationClassInputStream
		{
			get
			{
				return RepresentationClass_Renamed.IsSubclassOf(IoInputStreamClass);
			}
		}

		/// <summary>
		/// Returns whether the representation class for this
		/// <code>DataFlavor</code> is <code>java.io.Reader</code> or a subclass
		/// thereof.
		/// 
		/// @since 1.4
		/// </summary>
		public virtual bool RepresentationClassReader
		{
			get
			{
				return RepresentationClass_Renamed.IsSubclassOf(typeof(Reader));
			}
		}

		/// <summary>
		/// Returns whether the representation class for this
		/// <code>DataFlavor</code> is <code>java.nio.CharBuffer</code> or a
		/// subclass thereof.
		/// 
		/// @since 1.4
		/// </summary>
		public virtual bool RepresentationClassCharBuffer
		{
			get
			{
				return RepresentationClass_Renamed.IsSubclassOf(typeof(CharBuffer));
			}
		}

		/// <summary>
		/// Returns whether the representation class for this
		/// <code>DataFlavor</code> is <code>java.nio.ByteBuffer</code> or a
		/// subclass thereof.
		/// 
		/// @since 1.4
		/// </summary>
		public virtual bool RepresentationClassByteBuffer
		{
			get
			{
				return RepresentationClass_Renamed.IsSubclassOf(typeof(ByteBuffer));
			}
		}

	   /// <summary>
	   /// Returns true if the representation class can be serialized. </summary>
	   /// <returns> true if the representation class can be serialized </returns>

		public virtual bool RepresentationClassSerializable
		{
			get
			{
				return RepresentationClass_Renamed.IsSubclassOf(typeof(java.io.Serializable));
			}
		}

	   /// <summary>
	   /// Returns true if the representation class is <code>Remote</code>. </summary>
	   /// <returns> true if the representation class is <code>Remote</code> </returns>

		public virtual bool RepresentationClassRemote
		{
			get
			{
				return DataTransferer.isRemote(RepresentationClass_Renamed);
			}
		}

	   /// <summary>
	   /// Returns true if the <code>DataFlavor</code> specified represents
	   /// a serialized object. </summary>
	   /// <returns> true if the <code>DataFlavor</code> specified represents
	   ///   a Serialized Object </returns>

		public virtual bool FlavorSerializedObjectType
		{
			get
			{
				return RepresentationClassSerializable && IsMimeTypeEqual(JavaSerializedObjectMimeType);
			}
		}

		/// <summary>
		/// Returns true if the <code>DataFlavor</code> specified represents
		/// a remote object. </summary>
		/// <returns> true if the <code>DataFlavor</code> specified represents
		///  a Remote Object </returns>

		public virtual bool FlavorRemoteObjectType
		{
			get
			{
				return RepresentationClassRemote && RepresentationClassSerializable && IsMimeTypeEqual(JavaRemoteObjectMimeType);
			}
		}


	   /// <summary>
	   /// Returns true if the <code>DataFlavor</code> specified represents
	   /// a list of file objects. </summary>
	   /// <returns> true if the <code>DataFlavor</code> specified represents
	   ///   a List of File objects </returns>

	   public virtual bool FlavorJavaFileListType
	   {
		   get
		   {
				if (MimeType_Renamed == null || RepresentationClass_Renamed == null)
				{
					return false;
				}
				return RepresentationClass_Renamed.IsSubclassOf(typeof(IList)) && MimeType_Renamed.Match(JavaFileListFlavor.MimeType_Renamed);
    
		   }
	   }

		/// <summary>
		/// Returns whether this <code>DataFlavor</code> is a valid text flavor for
		/// this implementation of the Java platform. Only flavors equivalent to
		/// <code>DataFlavor.stringFlavor</code> and <code>DataFlavor</code>s with
		/// a primary MIME type of "text" can be valid text flavors.
		/// <para>
		/// If this flavor supports the charset parameter, it must be equivalent to
		/// <code>DataFlavor.stringFlavor</code>, or its representation must be
		/// <code>java.io.Reader</code>, <code>java.lang.String</code>,
		/// <code>java.nio.CharBuffer</code>, <code>[C</code>,
		/// <code>java.io.InputStream</code>, <code>java.nio.ByteBuffer</code>, or
		/// <code>[B</code>. If the representation is
		/// <code>java.io.InputStream</code>, <code>java.nio.ByteBuffer</code>, or
		/// <code>[B</code>, then this flavor's <code>charset</code> parameter must
		/// be supported by this implementation of the Java platform. If a charset
		/// is not specified, then the platform default charset, which is always
		/// supported, is assumed.
		/// </para>
		/// <para>
		/// If this flavor does not support the charset parameter, its
		/// representation must be <code>java.io.InputStream</code>,
		/// <code>java.nio.ByteBuffer</code>, or <code>[B</code>.
		/// </para>
		/// <para>
		/// See <code>selectBestTextFlavor</code> for a list of text flavors which
		/// support the charset parameter.
		/// 
		/// </para>
		/// </summary>
		/// <returns> <code>true</code> if this <code>DataFlavor</code> is a valid
		///         text flavor as described above; <code>false</code> otherwise </returns>
		/// <seealso cref= #selectBestTextFlavor
		/// @since 1.4 </seealso>
		public virtual bool FlavorTextType
		{
			get
			{
				return (DataTransferer.isFlavorCharsetTextType(this) || DataTransferer.isFlavorNoncharsetTextType(this));
			}
		}

	   /// <summary>
	   /// Serializes this <code>DataFlavor</code>.
	   /// </summary>

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public synchronized void writeExternal(java.io.ObjectOutput os) throws java.io.IOException
	   public virtual void WriteExternal(ObjectOutput os)
	   {
		   lock (this)
		   {
			   if (MimeType_Renamed != null)
			   {
				   MimeType_Renamed.SetParameter("humanPresentableName", HumanPresentableName_Renamed);
				   os.WriteObject(MimeType_Renamed);
				   MimeType_Renamed.RemoveParameter("humanPresentableName");
			   }
			   else
			   {
				   os.WriteObject(null);
			   }
        
			   os.WriteObject(RepresentationClass_Renamed);
		   }
	   }

	   /// <summary>
	   /// Restores this <code>DataFlavor</code> from a Serialized state.
	   /// </summary>

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public synchronized void readExternal(java.io.ObjectInput is) throws java.io.IOException, ClassNotFoundException
	   public virtual void ReadExternal(ObjectInput @is)
	   {
		   lock (this)
		   {
			   String rcn = null;
				MimeType_Renamed = (MimeType)@is.ReadObject();
        
				if (MimeType_Renamed != null)
				{
					HumanPresentableName_Renamed = MimeType_Renamed.GetParameter("humanPresentableName");
					MimeType_Renamed.RemoveParameter("humanPresentableName");
					rcn = MimeType_Renamed.GetParameter("class");
					if (rcn == null)
					{
						throw new IOException("no class parameter specified in: " + MimeType_Renamed);
					}
				}
        
				try
				{
					RepresentationClass_Renamed = (Class)@is.ReadObject();
				}
				catch (OptionalDataException ode)
				{
					if (!ode.Eof || ode.Length != 0)
					{
						throw ode;
					}
					// Ensure backward compatibility.
					// Old versions didn't write the representation class to the stream.
					if (rcn != null)
					{
						RepresentationClass_Renamed = DataFlavor.TryToLoadClass(rcn, this.GetType().ClassLoader);
					}
				}
		   }
	   }

	   /// <summary>
	   /// Returns a clone of this <code>DataFlavor</code>. </summary>
	   /// <returns> a clone of this <code>DataFlavor</code> </returns>

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public Object clone() throws CloneNotSupportedException
		public virtual Object Clone()
		{
			Object newObj = base.Clone();
			if (MimeType_Renamed != null)
			{
				((DataFlavor)newObj).MimeType_Renamed = (MimeType)MimeType_Renamed.Clone();
			}
			return newObj;
		} // clone()

	   /// <summary>
	   /// Called on <code>DataFlavor</code> for every MIME Type parameter
	   /// to allow <code>DataFlavor</code> subclasses to handle special
	   /// parameters like the text/plain <code>charset</code>
	   /// parameters, whose values are case insensitive.  (MIME type parameter
	   /// values are supposed to be case sensitive.
	   /// <para>
	   /// This method is called for each parameter name/value pair and should
	   /// return the normalized representation of the <code>parameterValue</code>.
	   /// 
	   /// This method is never invoked by this implementation from 1.1 onwards.
	   /// 
	   /// @deprecated
	   /// </para>
	   /// </summary>
		[Obsolete]
		protected internal virtual String NormalizeMimeTypeParameter(String parameterName, String parameterValue)
		{
			return parameterValue;
		}

	   /// <summary>
	   /// Called for each MIME type string to give <code>DataFlavor</code> subtypes
	   /// the opportunity to change how the normalization of MIME types is
	   /// accomplished.  One possible use would be to add default
	   /// parameter/value pairs in cases where none are present in the MIME
	   /// type string passed in.
	   /// 
	   /// This method is never invoked by this implementation from 1.1 onwards.
	   /// 
	   /// @deprecated
	   /// </summary>
		[Obsolete]
		protected internal virtual String NormalizeMimeType(String mimeType)
		{
			return mimeType;
		}

		/*
		 * fields
		 */

		/* placeholder for caching any platform-specific data for flavor */

		[NonSerialized]
		internal int Atom;

		/* Mime Type of DataFlavor */

		internal MimeType MimeType_Renamed;

		private String HumanPresentableName_Renamed;

		/// <summary>
		/// Java class of objects this DataFlavor represents * </summary>

		private Class RepresentationClass_Renamed;

	} // class DataFlavor

}
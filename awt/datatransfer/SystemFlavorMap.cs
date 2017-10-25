using System;
using System.Collections.Generic;

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

namespace java.awt.datatransfer
{





	using AppContext = sun.awt.AppContext;
	using DataTransferer = sun.awt.datatransfer.DataTransferer;

	/// <summary>
	/// The SystemFlavorMap is a configurable map between "natives" (Strings), which
	/// correspond to platform-specific data formats, and "flavors" (DataFlavors),
	/// which correspond to platform-independent MIME types. This mapping is used
	/// by the data transfer subsystem to transfer data between Java and native
	/// applications, and between Java applications in separate VMs.
	/// <para>
	/// 
	/// @since 1.2
	/// </para>
	/// </summary>
	public sealed class SystemFlavorMap : FlavorMap, FlavorTable
	{

		/// <summary>
		/// Constant prefix used to tag Java types converted to native platform
		/// type.
		/// </summary>
		private static String JavaMIME = "JAVA_DATAFLAVOR:";

		private static readonly Object FLAVOR_MAP_KEY = new Object();

		/// <summary>
		/// Copied from java.util.Properties.
		/// </summary>
		private const String KeyValueSeparators = "=: \t\r\n\f";
		private const String StrictKeyValueSeparators = "=:";
		private const String WhiteSpaceChars = " \t\r\n\f";

		/// <summary>
		/// The list of valid, decoded text flavor representation classes, in order
		/// from best to worst.
		/// </summary>
		private static readonly String[] UNICODE_TEXT_CLASSES = new String[] {"java.io.Reader", "java.lang.String", "java.nio.CharBuffer", "\"[C\""};

		/// <summary>
		/// The list of valid, encoded text flavor representation classes, in order
		/// from best to worst.
		/// </summary>
		private static readonly String[] ENCODED_TEXT_CLASSES = new String[] {"java.io.InputStream", "java.nio.ByteBuffer", "\"[B\""};

		/// <summary>
		/// A String representing text/plain MIME type.
		/// </summary>
		private const String TEXT_PLAIN_BASE_TYPE = "text/plain";

		/// <summary>
		/// A String representing text/html MIME type.
		/// </summary>
		private const String HTML_TEXT_BASE_TYPE = "text/html";

		/// <summary>
		/// Maps native Strings to Lists of DataFlavors (or base type Strings for
		/// text DataFlavors).
		/// Do not use the field directly, use getNativeToFlavor() instead.
		/// </summary>
		private readonly IDictionary<String, LinkedHashSet<DataFlavor>> NativeToFlavor_Renamed = new Dictionary<String, LinkedHashSet<DataFlavor>>();

		/// <summary>
		/// Accessor to nativeToFlavor map.  Since we use lazy initialization we must
		/// use this accessor instead of direct access to the field which may not be
		/// initialized yet.  This method will initialize the field if needed.
		/// </summary>
		/// <returns> nativeToFlavor </returns>
		private IDictionary<String, LinkedHashSet<DataFlavor>> NativeToFlavor
		{
			get
			{
				if (!IsMapInitialized)
				{
					InitSystemFlavorMap();
				}
				return NativeToFlavor_Renamed;
			}
		}

		/// <summary>
		/// Maps DataFlavors (or base type Strings for text DataFlavors) to Lists of
		/// native Strings.
		/// Do not use the field directly, use getFlavorToNative() instead.
		/// </summary>
		private readonly IDictionary<DataFlavor, LinkedHashSet<String>> FlavorToNative_Renamed = new Dictionary<DataFlavor, LinkedHashSet<String>>();

		/// <summary>
		/// Accessor to flavorToNative map.  Since we use lazy initialization we must
		/// use this accessor instead of direct access to the field which may not be
		/// initialized yet.  This method will initialize the field if needed.
		/// </summary>
		/// <returns> flavorToNative </returns>
		private IDictionary<DataFlavor, LinkedHashSet<String>> FlavorToNative
		{
			get
			{
				lock (this)
				{
					if (!IsMapInitialized)
					{
						InitSystemFlavorMap();
					}
					return FlavorToNative_Renamed;
				}
			}
		}

		/// <summary>
		/// Maps a text DataFlavor primary mime-type to the native. Used only to store
		/// standard mappings registered in the flavormap.properties
		/// Do not use this field directly, use getTextTypeToNative() instead.
		/// </summary>
		private IDictionary<String, LinkedHashSet<String>> TextTypeToNative_Renamed = new Dictionary<String, LinkedHashSet<String>>();

		/// <summary>
		/// Shows if the object has been initialized.
		/// </summary>
		private bool IsMapInitialized = false;

		/// <summary>
		/// An accessor to textTypeToNative map.  Since we use lazy initialization we
		/// must use this accessor instead of direct access to the field which may not
		/// be initialized yet. This method will initialize the field if needed.
		/// </summary>
		/// <returns> textTypeToNative </returns>
		private IDictionary<String, LinkedHashSet<String>> TextTypeToNative
		{
			get
			{
				lock (this)
				{
					if (!IsMapInitialized)
					{
						InitSystemFlavorMap();
						// From this point the map should not be modified
						TextTypeToNative_Renamed = Collections.UnmodifiableMap(TextTypeToNative_Renamed);
					}
					return TextTypeToNative_Renamed;
				}
			}
		}

		/// <summary>
		/// Caches the result of getNativesForFlavor(). Maps DataFlavors to
		/// SoftReferences which reference LinkedHashSet of String natives.
		/// </summary>
		private readonly SoftCache<DataFlavor, String> NativesForFlavorCache = new SoftCache<DataFlavor, String>();

		/// <summary>
		/// Caches the result getFlavorsForNative(). Maps String natives to
		/// SoftReferences which reference LinkedHashSet of DataFlavors.
		/// </summary>
		private readonly SoftCache<String, DataFlavor> FlavorsForNativeCache = new SoftCache<String, DataFlavor>();

		/// <summary>
		/// Dynamic mapping generation used for text mappings should not be applied
		/// to the DataFlavors and String natives for which the mappings have been
		/// explicitly specified with setFlavorsForNative() or
		/// setNativesForFlavor(). This keeps all such keys.
		/// </summary>
		private Set<Object> DisabledMappingGenerationKeys = new HashSet<Object>();

		/// <summary>
		/// Returns the default FlavorMap for this thread's ClassLoader.
		/// </summary>
		public static FlavorMap DefaultFlavorMap
		{
			get
			{
				AppContext context = AppContext.AppContext;
				FlavorMap fm = (FlavorMap) context.get(FLAVOR_MAP_KEY);
				if (fm == null)
				{
					fm = new SystemFlavorMap();
					context.put(FLAVOR_MAP_KEY, fm);
				}
				return fm;
			}
		}

		private SystemFlavorMap()
		{
		}

		/// <summary>
		/// Initializes a SystemFlavorMap by reading flavormap.properties and
		/// AWT.DnD.flavorMapFileURL.
		/// For thread-safety must be called under lock on this.
		/// </summary>
		private void InitSystemFlavorMap()
		{
			if (IsMapInitialized)
			{
				return;
			}

			IsMapInitialized = true;
			BufferedReader flavormapDotProperties = java.security.AccessController.doPrivileged(new PrivilegedActionAnonymousInnerClassHelper(this));

			String url = java.security.AccessController.doPrivileged(new PrivilegedActionAnonymousInnerClassHelper2(this));

			if (flavormapDotProperties != null)
			{
				try
				{
					ParseAndStoreReader(flavormapDotProperties);
				}
				catch (IOException e)
				{
					System.Console.Error.WriteLine("IOException:" + e + " while parsing default flavormap.properties file");
				}
			}

			BufferedReader flavormapURL = null;
			if (url != null)
			{
				try
				{
					flavormapURL = new BufferedReader(new InputStreamReader((new URL(url)).OpenStream(), "ISO-8859-1"));
				}
				catch (MalformedURLException e)
				{
					System.Console.Error.WriteLine("MalformedURLException:" + e + " while reading AWT.DnD.flavorMapFileURL:" + url);
				}
				catch (IOException e)
				{
					System.Console.Error.WriteLine("IOException:" + e + " while reading AWT.DnD.flavorMapFileURL:" + url);
				}
				catch (SecurityException)
				{
					// ignored
				}
			}

			if (flavormapURL != null)
			{
				try
				{
					ParseAndStoreReader(flavormapURL);
				}
				catch (IOException e)
				{
					System.Console.Error.WriteLine("IOException:" + e + " while parsing AWT.DnD.flavorMapFileURL");
				}
			}
		}

		private class PrivilegedActionAnonymousInnerClassHelper : java.security.PrivilegedAction<BufferedReader>
		{
			private readonly SystemFlavorMap OuterInstance;

			public PrivilegedActionAnonymousInnerClassHelper(SystemFlavorMap outerInstance)
			{
				this.OuterInstance = outerInstance;
			}

			public virtual BufferedReader Run()
			{
				String fileName = System.getProperty("java.home") + File.separator + "lib" + File.separator + "flavormap.properties";
				try
				{
					return new BufferedReader(new InputStreamReader((new File(fileName)).toURI().toURL().openStream(), "ISO-8859-1"));
				}
				catch (MalformedURLException e)
				{
					System.Console.Error.WriteLine("MalformedURLException:" + e + " while loading default flavormap.properties file:" + fileName);
				}
				catch (IOException e)
				{
					System.Console.Error.WriteLine("IOException:" + e + " while loading default flavormap.properties file:" + fileName);
				}
				return null;
			}
		}

		private class PrivilegedActionAnonymousInnerClassHelper2 : java.security.PrivilegedAction<String>
		{
			private readonly SystemFlavorMap OuterInstance;

			public PrivilegedActionAnonymousInnerClassHelper2(SystemFlavorMap outerInstance)
			{
				this.OuterInstance = outerInstance;
			}

			public virtual String Run()
			{
				return Toolkit.GetProperty("AWT.DnD.flavorMapFileURL", null);
			}
		}
		/// <summary>
		/// Copied code from java.util.Properties. Parsing the data ourselves is the
		/// only way to handle duplicate keys and values.
		/// </summary>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: private void parseAndStoreReader(java.io.BufferedReader in) throws java.io.IOException
		private void ParseAndStoreReader(BufferedReader @in)
		{
			while (true)
			{
				// Get next line
				String line = @in.ReadLine();
				if (line == null)
				{
					return;
				}

				if (line.Length() > 0)
				{
					// Continue lines that end in slashes if they are not comments
					char firstChar = line.CharAt(0);
					if (firstChar != '#' && firstChar != '!')
					{
						while (ContinueLine(line))
						{
							String nextLine = @in.ReadLine();
							if (nextLine == null)
							{
								nextLine = "";
							}
							String loppedLine = line.Substring(0, line.Length() - 1);
							// Advance beyond whitespace on new line
							int startIndex = 0;
							for (; startIndex < nextLine.Length(); startIndex++)
							{
								if (WhiteSpaceChars.IndexOf(nextLine.CharAt(startIndex)) == -1)
								{
									break;
								}
							}
							nextLine = nextLine.Substring(startIndex, nextLine.Length() - startIndex);
							line = loppedLine + nextLine;
						}

						// Find start of key
						int len = line.Length();
						int keyStart = 0;
						for (; keyStart < len; keyStart++)
						{
							if (WhiteSpaceChars.IndexOf(line.CharAt(keyStart)) == -1)
							{
								break;
							}
						}

						// Blank lines are ignored
						if (keyStart == len)
						{
							continue;
						}

						// Find separation between key and value
						int separatorIndex = keyStart;
						for (; separatorIndex < len; separatorIndex++)
						{
							char currentChar = line.CharAt(separatorIndex);
							if (currentChar == '\\')
							{
								separatorIndex++;
							}
							else if (KeyValueSeparators.IndexOf(currentChar) != -1)
							{
								break;
							}
						}

						// Skip over whitespace after key if any
						int valueIndex = separatorIndex;
						for (; valueIndex < len; valueIndex++)
						{
							if (WhiteSpaceChars.IndexOf(line.CharAt(valueIndex)) == -1)
							{
								break;
							}
						}

						// Skip over one non whitespace key value separators if any
						if (valueIndex < len)
						{
							if (StrictKeyValueSeparators.IndexOf(line.CharAt(valueIndex)) != -1)
							{
								valueIndex++;
							}
						}

						// Skip over white space after other separators if any
						while (valueIndex < len)
						{
							if (WhiteSpaceChars.IndexOf(line.CharAt(valueIndex)) == -1)
							{
								break;
							}
							valueIndex++;
						}

						String key = line.Substring(keyStart, separatorIndex - keyStart);
						String value = (separatorIndex < len) ? line.Substring(valueIndex, len - valueIndex) : "";

						// Convert then store key and value
						key = LoadConvert(key);
						value = LoadConvert(value);

						try
						{
							MimeType mime = new MimeType(value);
							if ("text".Equals(mime.PrimaryType))
							{
								String charset = mime.GetParameter("charset");
								if (DataTransferer.doesSubtypeSupportCharset(mime.SubType, charset))
								{
									// We need to store the charset and eoln
									// parameters, if any, so that the
									// DataTransferer will have this information
									// for conversion into the native format.
									DataTransferer transferer = DataTransferer.Instance;
									if (transferer != null)
									{
										transferer.registerTextFlavorProperties(key, charset, mime.GetParameter("eoln"), mime.GetParameter("terminators"));
									}
								}

								// But don't store any of these parameters in the
								// DataFlavor itself for any text natives (even
								// non-charset ones). The SystemFlavorMap will
								// synthesize the appropriate mappings later.
								mime.RemoveParameter("charset");
								mime.RemoveParameter("class");
								mime.RemoveParameter("eoln");
								mime.RemoveParameter("terminators");
								value = mime.ToString();
							}
						}
						catch (MimeTypeParseException e)
						{
							Console.WriteLine(e.ToString());
							Console.Write(e.StackTrace);
							continue;
						}

						DataFlavor flavor;
						try
						{
							flavor = new DataFlavor(value);
						}
						catch (Exception)
						{
							try
							{
								flavor = new DataFlavor(value, null);
							}
							catch (Exception ee)
							{
								ee.PrintStackTrace();
								continue;
							}
						}

//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final java.util.LinkedHashSet<DataFlavor> dfs = new java.util.LinkedHashSet<>();
						LinkedHashSet<DataFlavor> dfs = new LinkedHashSet<DataFlavor>();
						dfs.Add(flavor);

						if ("text".Equals(flavor.PrimaryType))
						{
							dfs.AddAll(ConvertMimeTypeToDataFlavors(value));
							Store(flavor.MimeType_Renamed.BaseType, key, TextTypeToNative);
						}

						foreach (DataFlavor df in dfs)
						{
							Store(df, key, FlavorToNative);
							Store(key, df, NativeToFlavor);
						}
					}
				}
			}
		}

		/// <summary>
		/// Copied from java.util.Properties.
		/// </summary>
		private bool ContinueLine(String line)
		{
			int slashCount = 0;
			int index = line.Length() - 1;
			while ((index >= 0) && (line.CharAt(index--) == '\\'))
			{
				slashCount++;
			}
			return (slashCount % 2 == 1);
		}

		/// <summary>
		/// Copied from java.util.Properties.
		/// </summary>
		private String LoadConvert(String theString)
		{
			char aChar;
			int len = theString.Length();
			StringBuilder outBuffer = new StringBuilder(len);

			for (int x = 0; x < len;)
			{
				aChar = theString.CharAt(x++);
				if (aChar == '\\')
				{
					aChar = theString.CharAt(x++);
					if (aChar == 'u')
					{
						// Read the xxxx
						int value = 0;
						for (int i = 0; i < 4; i++)
						{
							aChar = theString.CharAt(x++);
							switch (aChar)
							{
							  case '0':
						  case '1':
					  case '2':
				  case '3':
			  case '4':
							  case '5':
						  case '6':
					  case '7':
				  case '8':
			  case '9':
			  {
								 value = (value << 4) + aChar - '0';
								 break;
			  }
							  case 'a':
						  case 'b':
					  case 'c':
							  case 'd':
						  case 'e':
					  case 'f':
					  {
								 value = (value << 4) + 10 + aChar - 'a';
								 break;
					  }
							  case 'A':
						  case 'B':
					  case 'C':
							  case 'D':
						  case 'E':
					  case 'F':
					  {
								 value = (value << 4) + 10 + aChar - 'A';
								 break;
					  }
							  default:
							  {
								  throw new IllegalArgumentException("Malformed \\uxxxx encoding.");
							  }
							}
						}
						outBuffer.Append((char)value);
					}
					else
					{
						if (aChar == 't')
						{
							aChar = '\t';
						}
						else if (aChar == 'r')
						{
							aChar = '\r';
						}
						else if (aChar == 'n')
						{
							aChar = '\n';
						}
						else if (aChar == 'f')
						{
							aChar = '\f';
						}
						outBuffer.Append(aChar);
					}
				}
				else
				{
					outBuffer.Append(aChar);
				}
			}
			return outBuffer.ToString();
		}

		/// <summary>
		/// Stores the listed object under the specified hash key in map. Unlike a
		/// standard map, the listed object will not replace any object already at
		/// the appropriate Map location, but rather will be appended to a List
		/// stored in that location.
		/// </summary>
		private void store<H, L>(H hashed, L listed, IDictionary<H, LinkedHashSet<L>> map)
		{
			LinkedHashSet<L> list = map[hashed];
			if (list == null)
			{
				list = new LinkedHashSet<>(1);
				map[hashed] = list;
			}
			if (!list.Contains(listed))
			{
				list.Add(listed);
			}
		}

		/// <summary>
		/// Semantically equivalent to 'nativeToFlavor.get(nat)'. This method
		/// handles the case where 'nat' is not found in 'nativeToFlavor'. In that
		/// case, a new DataFlavor is synthesized, stored, and returned, if and
		/// only if the specified native is encoded as a Java MIME type.
		/// </summary>
		private LinkedHashSet<DataFlavor> NativeToFlavorLookup(String nat)
		{
			LinkedHashSet<DataFlavor> flavors = NativeToFlavor[nat];


			if (nat != null && !DisabledMappingGenerationKeys.Contains(nat))
			{
				DataTransferer transferer = DataTransferer.Instance;
				if (transferer != null)
				{
					LinkedHashSet<DataFlavor> platformFlavors = transferer.getPlatformMappingsForNative(nat);
					if (platformFlavors.Count > 0)
					{
						if (flavors != null)
						{
							// Prepending the platform-specific mappings ensures
							// that the flavors added with
							// addFlavorForUnencodedNative() are at the end of
							// list.
							platformFlavors.AddAll(flavors);
						}
						flavors = platformFlavors;
					}
				}
			}

			if (flavors == null && IsJavaMIMEType(nat))
			{
				String decoded = DecodeJavaMIMEType(nat);
				DataFlavor flavor = null;

				try
				{
					flavor = new DataFlavor(decoded);
				}
				catch (Exception e)
				{
//JAVA TO C# CONVERTER WARNING: The .NET Type.FullName property will not always yield results identical to the Java Class.getName method:
					System.Console.Error.WriteLine("Exception \"" + e.GetType().FullName + ": " + e.Message + "\"while constructing DataFlavor for: " + decoded);
				}

				if (flavor != null)
				{
					flavors = new LinkedHashSet<>(1);
					NativeToFlavor[nat] = flavors;
					flavors.Add(flavor);
					FlavorsForNativeCache.Remove(nat);

					LinkedHashSet<String> natives = FlavorToNative[flavor];
					if (natives == null)
					{
						natives = new LinkedHashSet<>(1);
						FlavorToNative[flavor] = natives;
					}
					natives.Add(nat);
					NativesForFlavorCache.Remove(flavor);
				}
			}

			return (flavors != null) ? flavors : new LinkedHashSet<>(0);
		}

		/// <summary>
		/// Semantically equivalent to 'flavorToNative.get(flav)'. This method
		/// handles the case where 'flav' is not found in 'flavorToNative' depending
		/// on the value of passes 'synthesize' parameter. If 'synthesize' is
		/// SYNTHESIZE_IF_NOT_FOUND a native is synthesized, stored, and returned by
		/// encoding the DataFlavor's MIME type. Otherwise an empty List is returned
		/// and 'flavorToNative' remains unaffected.
		/// </summary>
//JAVA TO C# CONVERTER WARNING: 'final' parameters are not allowed in .NET:
//ORIGINAL LINE: private java.util.LinkedHashSet<String> flavorToNativeLookup(final DataFlavor flav, final boolean synthesize)
		private LinkedHashSet<String> FlavorToNativeLookup(DataFlavor flav, bool synthesize)
		{

			LinkedHashSet<String> natives = FlavorToNative[flav];

			if (flav != null && !DisabledMappingGenerationKeys.Contains(flav))
			{
				DataTransferer transferer = DataTransferer.Instance;
				if (transferer != null)
				{
					LinkedHashSet<String> platformNatives = transferer.getPlatformMappingsForFlavor(flav);
					if (platformNatives.Count > 0)
					{
						if (natives != null)
						{
							// Prepend the platform-specific mappings to ensure
							// that the natives added with
							// addUnencodedNativeForFlavor() are at the end of
							// list.
							platformNatives.AddAll(natives);
						}
						natives = platformNatives;
					}
				}
			}

			if (natives == null)
			{
				if (synthesize)
				{
					String encoded = EncodeDataFlavor(flav);
					natives = new LinkedHashSet<>(1);
					FlavorToNative[flav] = natives;
					natives.Add(encoded);

					LinkedHashSet<DataFlavor> flavors = NativeToFlavor[encoded];
					if (flavors == null)
					{
						flavors = new LinkedHashSet<>(1);
						NativeToFlavor[encoded] = flavors;
					}
					flavors.Add(flav);

					NativesForFlavorCache.Remove(flav);
					FlavorsForNativeCache.Remove(encoded);
				}
				else
				{
					natives = new LinkedHashSet<>(0);
				}
			}

			return new LinkedHashSet<>(natives);
		}

		/// <summary>
		/// Returns a <code>List</code> of <code>String</code> natives to which the
		/// specified <code>DataFlavor</code> can be translated by the data transfer
		/// subsystem. The <code>List</code> will be sorted from best native to
		/// worst. That is, the first native will best reflect data in the specified
		/// flavor to the underlying native platform.
		/// <para>
		/// If the specified <code>DataFlavor</code> is previously unknown to the
		/// data transfer subsystem and the data transfer subsystem is unable to
		/// translate this <code>DataFlavor</code> to any existing native, then
		/// invoking this method will establish a
		/// mapping in both directions between the specified <code>DataFlavor</code>
		/// and an encoded version of its MIME type as its native.
		/// 
		/// </para>
		/// </summary>
		/// <param name="flav"> the <code>DataFlavor</code> whose corresponding natives
		///        should be returned. If <code>null</code> is specified, all
		///        natives currently known to the data transfer subsystem are
		///        returned in a non-deterministic order. </param>
		/// <returns> a <code>java.util.List</code> of <code>java.lang.String</code>
		///         objects which are platform-specific representations of platform-
		///         specific data formats
		/// </returns>
		/// <seealso cref= #encodeDataFlavor
		/// @since 1.4 </seealso>
		public IList<String> GetNativesForFlavor(DataFlavor flav)
		{
			lock (this)
			{
				LinkedHashSet<String> retval = NativesForFlavorCache.Check(flav);
				if (retval != null)
				{
					return new List<>(retval);
				}
        
				if (flav == null)
				{
					retval = new LinkedHashSet<>(NativeToFlavor.Keys);
				}
				else if (DisabledMappingGenerationKeys.Contains(flav))
				{
					// In this case we shouldn't synthesize a native for this flavor,
					// since its mappings were explicitly specified.
					retval = FlavorToNativeLookup(flav, false);
				}
				else if (DataTransferer.isFlavorCharsetTextType(flav))
				{
					retval = new LinkedHashSet<>(0);
        
					// For text/* flavors, flavor-to-native mappings specified in
					// flavormap.properties are stored per flavor's base type.
					if ("text".Equals(flav.PrimaryType))
					{
						LinkedHashSet<String> textTypeNatives = TextTypeToNative[flav.MimeType_Renamed.BaseType];
						if (textTypeNatives != null)
						{
							retval.AddAll(textTypeNatives);
						}
					}
        
					// Also include text/plain natives, but don't duplicate Strings
					LinkedHashSet<String> textTypeNatives = TextTypeToNative[TEXT_PLAIN_BASE_TYPE];
					if (textTypeNatives != null)
					{
						retval.AddAll(textTypeNatives);
					}
        
					if (retval.Count == 0)
					{
						retval = FlavorToNativeLookup(flav, true);
					}
					else
					{
						// In this branch it is guaranteed that natives explicitly
						// listed for flav's MIME type were added with
						// addUnencodedNativeForFlavor(), so they have lower priority.
						retval.AddAll(FlavorToNativeLookup(flav, false));
					}
				}
				else if (DataTransferer.isFlavorNoncharsetTextType(flav))
				{
					retval = TextTypeToNative[flav.MimeType_Renamed.BaseType];
        
					if (retval == null || retval.Count == 0)
					{
						retval = FlavorToNativeLookup(flav, true);
					}
					else
					{
						// In this branch it is guaranteed that natives explicitly
						// listed for flav's MIME type were added with
						// addUnencodedNativeForFlavor(), so they have lower priority.
						retval.AddAll(FlavorToNativeLookup(flav, false));
					}
				}
				else
				{
					retval = FlavorToNativeLookup(flav, true);
				}
        
				NativesForFlavorCache.Put(flav, retval);
				// Create a copy, because client code can modify the returned list.
				return new List<>(retval);
			}
		}

		/// <summary>
		/// Returns a <code>List</code> of <code>DataFlavor</code>s to which the
		/// specified <code>String</code> native can be translated by the data
		/// transfer subsystem. The <code>List</code> will be sorted from best
		/// <code>DataFlavor</code> to worst. That is, the first
		/// <code>DataFlavor</code> will best reflect data in the specified
		/// native to a Java application.
		/// <para>
		/// If the specified native is previously unknown to the data transfer
		/// subsystem, and that native has been properly encoded, then invoking this
		/// method will establish a mapping in both directions between the specified
		/// native and a <code>DataFlavor</code> whose MIME type is a decoded
		/// version of the native.
		/// </para>
		/// <para>
		/// If the specified native is not a properly encoded native and the
		/// mappings for this native have not been altered with
		/// <code>setFlavorsForNative</code>, then the contents of the
		/// <code>List</code> is platform dependent, but <code>null</code>
		/// cannot be returned.
		/// 
		/// </para>
		/// </summary>
		/// <param name="nat"> the native whose corresponding <code>DataFlavor</code>s
		///        should be returned. If <code>null</code> is specified, all
		///        <code>DataFlavor</code>s currently known to the data transfer
		///        subsystem are returned in a non-deterministic order. </param>
		/// <returns> a <code>java.util.List</code> of <code>DataFlavor</code>
		///         objects into which platform-specific data in the specified,
		///         platform-specific native can be translated
		/// </returns>
		/// <seealso cref= #encodeJavaMIMEType
		/// @since 1.4 </seealso>
		public IList<DataFlavor> GetFlavorsForNative(String nat)
		{
			lock (this)
			{
				LinkedHashSet<DataFlavor> returnValue = FlavorsForNativeCache.Check(nat);
				if (returnValue != null)
				{
					return new List<>(returnValue);
				}
				else
				{
					returnValue = new LinkedHashSet<>();
				}
        
				if (nat == null)
				{
					foreach (String n in GetNativesForFlavor(null))
					{
						returnValue.AddAll(GetFlavorsForNative(n));
					}
				}
				else
				{
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final java.util.LinkedHashSet<DataFlavor> flavors = nativeToFlavorLookup(nat);
					LinkedHashSet<DataFlavor> flavors = NativeToFlavorLookup(nat);
					if (DisabledMappingGenerationKeys.Contains(nat))
					{
						return new List<>(flavors);
					}
        
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final java.util.LinkedHashSet<DataFlavor> flavorsWithSynthesized = nativeToFlavorLookup(nat);
					LinkedHashSet<DataFlavor> flavorsWithSynthesized = NativeToFlavorLookup(nat);
        
					foreach (DataFlavor df in flavorsWithSynthesized)
					{
						returnValue.Add(df);
						if ("text".Equals(df.PrimaryType))
						{
							String baseType = df.MimeType_Renamed.BaseType;
							returnValue.AddAll(ConvertMimeTypeToDataFlavors(baseType));
						}
					}
				}
				FlavorsForNativeCache.Put(nat, returnValue);
				return new List<>(returnValue);
			}
		}

//JAVA TO C# CONVERTER WARNING: 'final' parameters are not allowed in .NET:
//ORIGINAL LINE: private static java.util.Set<DataFlavor> convertMimeTypeToDataFlavors(final String baseType)
		private static Set<DataFlavor> ConvertMimeTypeToDataFlavors(String baseType)
		{

//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final java.util.Set<DataFlavor> returnValue = new java.util.LinkedHashSet<>();
			Set<DataFlavor> returnValue = new LinkedHashSet<DataFlavor>();

			String subType = null;

			try
			{
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final MimeType mimeType = new MimeType(baseType);
				MimeType mimeType = new MimeType(baseType);
				subType = mimeType.SubType;
			}
			catch (MimeTypeParseException)
			{
				// Cannot happen, since we checked all mappings
				// on load from flavormap.properties.
			}

			if (DataTransferer.doesSubtypeSupportCharset(subType, null))
			{
				if (TEXT_PLAIN_BASE_TYPE.Equals(baseType))
				{
					returnValue.Add(DataFlavor.StringFlavor);
				}

				foreach (String unicodeClassName in UNICODE_TEXT_CLASSES)
				{
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final String mimeType = baseType + ";charset=Unicode;class=" + unicodeClassName;
					String mimeType = baseType + ";charset=Unicode;class=" + unicodeClassName;

//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final java.util.LinkedHashSet<String> mimeTypes = handleHtmlMimeTypes(baseType, mimeType);
					LinkedHashSet<String> mimeTypes = HandleHtmlMimeTypes(baseType, mimeType);
					foreach (String mt in mimeTypes)
					{
						DataFlavor toAdd = null;
						try
						{
							toAdd = new DataFlavor(mt);
						}
						catch (ClassNotFoundException)
						{
						}
						returnValue.Add(toAdd);
					}
				}

				foreach (String charset in DataTransferer.standardEncodings())
				{

					foreach (String encodedTextClass in ENCODED_TEXT_CLASSES)
					{
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final String mimeType = baseType + ";charset=" + charset + ";class=" + encodedTextClass;
						String mimeType = baseType + ";charset=" + charset + ";class=" + encodedTextClass;

//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final java.util.LinkedHashSet<String> mimeTypes = handleHtmlMimeTypes(baseType, mimeType);
						LinkedHashSet<String> mimeTypes = HandleHtmlMimeTypes(baseType, mimeType);

						foreach (String mt in mimeTypes)
						{

							DataFlavor df = null;

							try
							{
								df = new DataFlavor(mt);
								// Check for equality to plainTextFlavor so
								// that we can ensure that the exact charset of
								// plainTextFlavor, not the canonical charset
								// or another equivalent charset with a
								// different name, is used.
								if (df.Equals(DataFlavor.PlainTextFlavor))
								{
									df = DataFlavor.PlainTextFlavor;
								}
							}
							catch (ClassNotFoundException)
							{
							}

							returnValue.Add(df);
						}
					}
				}

				if (TEXT_PLAIN_BASE_TYPE.Equals(baseType))
				{
					returnValue.Add(DataFlavor.PlainTextFlavor);
				}
			}
			else
			{
				// Non-charset text natives should be treated as
				// opaque, 8-bit data in any of its various
				// representations.
				foreach (String encodedTextClassName in ENCODED_TEXT_CLASSES)
				{
					DataFlavor toAdd = null;
					try
					{
						toAdd = new DataFlavor(baseType + ";class=" + encodedTextClassName);
					}
					catch (ClassNotFoundException)
					{
					}
					returnValue.Add(toAdd);
				}
			}
			return returnValue;
		}

		private static readonly String[] HtmlDocumntTypes = new String [] {"all", "selection", "fragment"};

		private static LinkedHashSet<String> HandleHtmlMimeTypes(String baseType, String mimeType)
		{

			LinkedHashSet<String> returnValues = new LinkedHashSet<String>();

			if (HTML_TEXT_BASE_TYPE.Equals(baseType))
			{
				foreach (String documentType in HtmlDocumntTypes)
				{
					returnValues.Add(mimeType + ";document=" + documentType);
				}
			}
			else
			{
				returnValues.Add(mimeType);
			}

			return returnValues;
		}

		/// <summary>
		/// Returns a <code>Map</code> of the specified <code>DataFlavor</code>s to
		/// their most preferred <code>String</code> native. Each native value will
		/// be the same as the first native in the List returned by
		/// <code>getNativesForFlavor</code> for the specified flavor.
		/// <para>
		/// If a specified <code>DataFlavor</code> is previously unknown to the
		/// data transfer subsystem, then invoking this method will establish a
		/// mapping in both directions between the specified <code>DataFlavor</code>
		/// and an encoded version of its MIME type as its native.
		/// 
		/// </para>
		/// </summary>
		/// <param name="flavors"> an array of <code>DataFlavor</code>s which will be the
		///        key set of the returned <code>Map</code>. If <code>null</code> is
		///        specified, a mapping of all <code>DataFlavor</code>s known to the
		///        data transfer subsystem to their most preferred
		///        <code>String</code> natives will be returned. </param>
		/// <returns> a <code>java.util.Map</code> of <code>DataFlavor</code>s to
		///         <code>String</code> natives
		/// </returns>
		/// <seealso cref= #getNativesForFlavor </seealso>
		/// <seealso cref= #encodeDataFlavor </seealso>
		public IDictionary<DataFlavor, String> GetNativesForFlavors(DataFlavor[] flavors)
		{
			lock (this)
			{
				// Use getNativesForFlavor to generate extra natives for text flavors
				// and stringFlavor
        
				if (flavors == null)
				{
					IList<DataFlavor> flavor_list = GetFlavorsForNative(null);
					flavors = new DataFlavor[flavor_list.Count];
					flavor_list.toArray(flavors);
				}
        
				IDictionary<DataFlavor, String> retval = new Dictionary<DataFlavor, String>(flavors.Length, 1.0f);
				foreach (DataFlavor flavor in flavors)
				{
					IList<String> natives = GetNativesForFlavor(flavor);
					String nat = (natives.Count == 0) ? null : natives[0];
					retval[flavor] = nat;
				}
        
				return retval;
			}
		}

		/// <summary>
		/// Returns a <code>Map</code> of the specified <code>String</code> natives
		/// to their most preferred <code>DataFlavor</code>. Each
		/// <code>DataFlavor</code> value will be the same as the first
		/// <code>DataFlavor</code> in the List returned by
		/// <code>getFlavorsForNative</code> for the specified native.
		/// <para>
		/// If a specified native is previously unknown to the data transfer
		/// subsystem, and that native has been properly encoded, then invoking this
		/// method will establish a mapping in both directions between the specified
		/// native and a <code>DataFlavor</code> whose MIME type is a decoded
		/// version of the native.
		/// 
		/// </para>
		/// </summary>
		/// <param name="natives"> an array of <code>String</code>s which will be the
		///        key set of the returned <code>Map</code>. If <code>null</code> is
		///        specified, a mapping of all supported <code>String</code> natives
		///        to their most preferred <code>DataFlavor</code>s will be
		///        returned. </param>
		/// <returns> a <code>java.util.Map</code> of <code>String</code> natives to
		///         <code>DataFlavor</code>s
		/// </returns>
		/// <seealso cref= #getFlavorsForNative </seealso>
		/// <seealso cref= #encodeJavaMIMEType </seealso>
		public IDictionary<String, DataFlavor> GetFlavorsForNatives(String[] natives)
		{
			lock (this)
			{
				// Use getFlavorsForNative to generate extra flavors for text natives
				if (natives == null)
				{
					IList<String> nativesList = GetNativesForFlavor(null);
					natives = new String[nativesList.Count];
					nativesList.toArray(natives);
				}
        
				IDictionary<String, DataFlavor> retval = new Dictionary<String, DataFlavor>(natives.Length, 1.0f);
				foreach (String aNative in natives)
				{
					IList<DataFlavor> flavors = GetFlavorsForNative(aNative);
					DataFlavor flav = (flavors.Count == 0)? null : flavors[0];
					retval[aNative] = flav;
				}
				return retval;
			}
		}

		/// <summary>
		/// Adds a mapping from the specified <code>DataFlavor</code> (and all
		/// <code>DataFlavor</code>s equal to the specified <code>DataFlavor</code>)
		/// to the specified <code>String</code> native.
		/// Unlike <code>getNativesForFlavor</code>, the mapping will only be
		/// established in one direction, and the native will not be encoded. To
		/// establish a two-way mapping, call
		/// <code>addFlavorForUnencodedNative</code> as well. The new mapping will
		/// be of lower priority than any existing mapping.
		/// This method has no effect if a mapping from the specified or equal
		/// <code>DataFlavor</code> to the specified <code>String</code> native
		/// already exists.
		/// </summary>
		/// <param name="flav"> the <code>DataFlavor</code> key for the mapping </param>
		/// <param name="nat"> the <code>String</code> native value for the mapping </param>
		/// <exception cref="NullPointerException"> if flav or nat is <code>null</code>
		/// </exception>
		/// <seealso cref= #addFlavorForUnencodedNative
		/// @since 1.4 </seealso>
		public void AddUnencodedNativeForFlavor(DataFlavor flav, String nat)
		{
			lock (this)
			{
				Objects.RequireNonNull(nat, "Null native not permitted");
				Objects.RequireNonNull(flav, "Null flavor not permitted");
        
				LinkedHashSet<String> natives = FlavorToNative[flav];
				if (natives == null)
				{
					natives = new LinkedHashSet<>(1);
					FlavorToNative[flav] = natives;
				}
				natives.Add(nat);
				NativesForFlavorCache.Remove(flav);
			}
		}

		/// <summary>
		/// Discards the current mappings for the specified <code>DataFlavor</code>
		/// and all <code>DataFlavor</code>s equal to the specified
		/// <code>DataFlavor</code>, and creates new mappings to the
		/// specified <code>String</code> natives.
		/// Unlike <code>getNativesForFlavor</code>, the mappings will only be
		/// established in one direction, and the natives will not be encoded. To
		/// establish two-way mappings, call <code>setFlavorsForNative</code>
		/// as well. The first native in the array will represent the highest
		/// priority mapping. Subsequent natives will represent mappings of
		/// decreasing priority.
		/// <para>
		/// If the array contains several elements that reference equal
		/// <code>String</code> natives, this method will establish new mappings
		/// for the first of those elements and ignore the rest of them.
		/// </para>
		/// <para>
		/// It is recommended that client code not reset mappings established by the
		/// data transfer subsystem. This method should only be used for
		/// application-level mappings.
		/// 
		/// </para>
		/// </summary>
		/// <param name="flav"> the <code>DataFlavor</code> key for the mappings </param>
		/// <param name="natives"> the <code>String</code> native values for the mappings </param>
		/// <exception cref="NullPointerException"> if flav or natives is <code>null</code>
		///         or if natives contains <code>null</code> elements
		/// </exception>
		/// <seealso cref= #setFlavorsForNative
		/// @since 1.4 </seealso>
		public void SetNativesForFlavor(DataFlavor flav, String[] natives)
		{
			lock (this)
			{
				Objects.RequireNonNull(natives, "Null natives not permitted");
				Objects.RequireNonNull(flav, "Null flavors not permitted");
        
				FlavorToNative.Remove(flav);
				foreach (String aNative in natives)
				{
					AddUnencodedNativeForFlavor(flav, aNative);
				}
				DisabledMappingGenerationKeys.Add(flav);
				NativesForFlavorCache.Remove(flav);
			}
		}

		/// <summary>
		/// Adds a mapping from a single <code>String</code> native to a single
		/// <code>DataFlavor</code>. Unlike <code>getFlavorsForNative</code>, the
		/// mapping will only be established in one direction, and the native will
		/// not be encoded. To establish a two-way mapping, call
		/// <code>addUnencodedNativeForFlavor</code> as well. The new mapping will
		/// be of lower priority than any existing mapping.
		/// This method has no effect if a mapping from the specified
		/// <code>String</code> native to the specified or equal
		/// <code>DataFlavor</code> already exists.
		/// </summary>
		/// <param name="nat"> the <code>String</code> native key for the mapping </param>
		/// <param name="flav"> the <code>DataFlavor</code> value for the mapping </param>
		/// <exception cref="NullPointerException"> if nat or flav is <code>null</code>
		/// </exception>
		/// <seealso cref= #addUnencodedNativeForFlavor
		/// @since 1.4 </seealso>
		public void AddFlavorForUnencodedNative(String nat, DataFlavor flav)
		{
			lock (this)
			{
				Objects.RequireNonNull(nat, "Null native not permitted");
				Objects.RequireNonNull(flav, "Null flavor not permitted");
        
				LinkedHashSet<DataFlavor> flavors = NativeToFlavor[nat];
				if (flavors == null)
				{
					flavors = new LinkedHashSet<>(1);
					NativeToFlavor[nat] = flavors;
				}
				flavors.Add(flav);
				FlavorsForNativeCache.Remove(nat);
			}
		}

		/// <summary>
		/// Discards the current mappings for the specified <code>String</code>
		/// native, and creates new mappings to the specified
		/// <code>DataFlavor</code>s. Unlike <code>getFlavorsForNative</code>, the
		/// mappings will only be established in one direction, and the natives need
		/// not be encoded. To establish two-way mappings, call
		/// <code>setNativesForFlavor</code> as well. The first
		/// <code>DataFlavor</code> in the array will represent the highest priority
		/// mapping. Subsequent <code>DataFlavor</code>s will represent mappings of
		/// decreasing priority.
		/// <para>
		/// If the array contains several elements that reference equal
		/// <code>DataFlavor</code>s, this method will establish new mappings
		/// for the first of those elements and ignore the rest of them.
		/// </para>
		/// <para>
		/// It is recommended that client code not reset mappings established by the
		/// data transfer subsystem. This method should only be used for
		/// application-level mappings.
		/// 
		/// </para>
		/// </summary>
		/// <param name="nat"> the <code>String</code> native key for the mappings </param>
		/// <param name="flavors"> the <code>DataFlavor</code> values for the mappings </param>
		/// <exception cref="NullPointerException"> if nat or flavors is <code>null</code>
		///         or if flavors contains <code>null</code> elements
		/// </exception>
		/// <seealso cref= #setNativesForFlavor
		/// @since 1.4 </seealso>
		public void SetFlavorsForNative(String nat, DataFlavor[] flavors)
		{
			lock (this)
			{
				Objects.RequireNonNull(nat, "Null native not permitted");
				Objects.RequireNonNull(flavors, "Null flavors not permitted");
        
				NativeToFlavor.Remove(nat);
				foreach (DataFlavor flavor in flavors)
				{
					AddFlavorForUnencodedNative(nat, flavor);
				}
				DisabledMappingGenerationKeys.Add(nat);
				FlavorsForNativeCache.Remove(nat);
			}
		}

		/// <summary>
		/// Encodes a MIME type for use as a <code>String</code> native. The format
		/// of an encoded representation of a MIME type is implementation-dependent.
		/// The only restrictions are:
		/// <ul>
		/// <li>The encoded representation is <code>null</code> if and only if the
		/// MIME type <code>String</code> is <code>null</code>.</li>
		/// <li>The encoded representations for two non-<code>null</code> MIME type
		/// <code>String</code>s are equal if and only if these <code>String</code>s
		/// are equal according to <code>String.equals(Object)</code>.</li>
		/// </ul>
		/// <para>
		/// The reference implementation of this method returns the specified MIME
		/// type <code>String</code> prefixed with <code>JAVA_DATAFLAVOR:</code>.
		/// 
		/// </para>
		/// </summary>
		/// <param name="mimeType"> the MIME type to encode </param>
		/// <returns> the encoded <code>String</code>, or <code>null</code> if
		///         mimeType is <code>null</code> </returns>
		public static String EncodeJavaMIMEType(String mimeType)
		{
			return (mimeType != null) ? JavaMIME + mimeType : null;
		}

		/// <summary>
		/// Encodes a <code>DataFlavor</code> for use as a <code>String</code>
		/// native. The format of an encoded <code>DataFlavor</code> is
		/// implementation-dependent. The only restrictions are:
		/// <ul>
		/// <li>The encoded representation is <code>null</code> if and only if the
		/// specified <code>DataFlavor</code> is <code>null</code> or its MIME type
		/// <code>String</code> is <code>null</code>.</li>
		/// <li>The encoded representations for two non-<code>null</code>
		/// <code>DataFlavor</code>s with non-<code>null</code> MIME type
		/// <code>String</code>s are equal if and only if the MIME type
		/// <code>String</code>s of these <code>DataFlavor</code>s are equal
		/// according to <code>String.equals(Object)</code>.</li>
		/// </ul>
		/// <para>
		/// The reference implementation of this method returns the MIME type
		/// <code>String</code> of the specified <code>DataFlavor</code> prefixed
		/// with <code>JAVA_DATAFLAVOR:</code>.
		/// 
		/// </para>
		/// </summary>
		/// <param name="flav"> the <code>DataFlavor</code> to encode </param>
		/// <returns> the encoded <code>String</code>, or <code>null</code> if
		///         flav is <code>null</code> or has a <code>null</code> MIME type </returns>
		public static String EncodeDataFlavor(DataFlavor flav)
		{
			return (flav != null) ? SystemFlavorMap.EncodeJavaMIMEType(flav.MimeType) : null;
		}

		/// <summary>
		/// Returns whether the specified <code>String</code> is an encoded Java
		/// MIME type.
		/// </summary>
		/// <param name="str"> the <code>String</code> to test </param>
		/// <returns> <code>true</code> if the <code>String</code> is encoded;
		///         <code>false</code> otherwise </returns>
		public static bool IsJavaMIMEType(String str)
		{
			return (str != null && str.StartsWith(JavaMIME, 0));
		}

		/// <summary>
		/// Decodes a <code>String</code> native for use as a Java MIME type.
		/// </summary>
		/// <param name="nat"> the <code>String</code> to decode </param>
		/// <returns> the decoded Java MIME type, or <code>null</code> if nat is not
		///         an encoded <code>String</code> native </returns>
		public static String DecodeJavaMIMEType(String nat)
		{
			return (IsJavaMIMEType(nat)) ? StringHelperClass.SubstringSpecial(nat, JavaMIME.Length(), nat.Length()).Trim() : null;
		}

		/// <summary>
		/// Decodes a <code>String</code> native for use as a
		/// <code>DataFlavor</code>.
		/// </summary>
		/// <param name="nat"> the <code>String</code> to decode </param>
		/// <returns> the decoded <code>DataFlavor</code>, or <code>null</code> if
		///         nat is not an encoded <code>String</code> native </returns>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public static DataFlavor decodeDataFlavor(String nat) throws ClassNotFoundException
		public static DataFlavor DecodeDataFlavor(String nat)
		{
			String retval_str = SystemFlavorMap.DecodeJavaMIMEType(nat);
			return (retval_str != null) ? new DataFlavor(retval_str) : null;
		}

		private sealed class SoftCache<K, V>
		{
			internal IDictionary<K, SoftReference<LinkedHashSet<V>>> Cache;

			public void Put(K key, LinkedHashSet<V> value)
			{
				if (Cache == null)
				{
					Cache = new Dictionary<>(1);
				}
				Cache[key] = new SoftReference<>(value);
			}

			public void Remove(K key)
			{
				if (Cache == null)
				{
					return;
				}
				Cache.Remove(null);
				Cache.Remove(key);
			}

			public LinkedHashSet<V> Check(K key)
			{
				if (Cache == null)
				{
					return null;
				}
				SoftReference<LinkedHashSet<V>> @ref = Cache[key];
				if (@ref != null)
				{
					return @ref.get();
				}
				return null;
			}
		}
	}

}
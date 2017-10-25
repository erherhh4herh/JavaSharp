using System.Collections.Generic;

/*
 * Copyright (c) 2002, 2012, Oracle and/or its affiliates. All rights reserved.
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

namespace java.util.prefs
{

	using org.xml.sax;
	using org.w3c.dom;

	/// <summary>
	/// XML Support for java.util.prefs. Methods to import and export preference
	/// nodes and subtrees.
	/// 
	/// @author  Josh Bloch and Mark Reinhold </summary>
	/// <seealso cref=     Preferences
	/// @since   1.4 </seealso>
	internal class XmlSupport
	{
		// The required DTD URI for exported preferences
		private const String PREFS_DTD_URI = "http://java.sun.com/dtd/preferences.dtd";

		// The actual DTD corresponding to the URI
		private static readonly String PREFS_DTD = "<?xml version=\"1.0\" encoding=\"UTF-8\"?>" + "<!-- DTD for preferences -->" + "<!ELEMENT preferences (root) >" + "<!ATTLIST preferences" + " EXTERNAL_XML_VERSION CDATA \"0.0\"  >" + "<!ELEMENT root (map, node*) >" + "<!ATTLIST root" + "          type (system|user) #REQUIRED >" + "<!ELEMENT node (map, node*) >" + "<!ATTLIST node" + "          name CDATA #REQUIRED >" + "<!ELEMENT map (entry*) >" + "<!ATTLIST map" + "  MAP_XML_VERSION CDATA \"0.0\"  >" + "<!ELEMENT entry EMPTY >" + "<!ATTLIST entry" + "          key CDATA #REQUIRED" + "          value CDATA #REQUIRED >";
		/// <summary>
		/// Version number for the format exported preferences files.
		/// </summary>
		private const String EXTERNAL_XML_VERSION = "1.0";

		/*
		 * Version number for the internal map files.
		 */
		private const String MAP_XML_VERSION = "1.0";

		/// <summary>
		/// Export the specified preferences node and, if subTree is true, all
		/// subnodes, to the specified output stream.  Preferences are exported as
		/// an XML document conforming to the definition in the Preferences spec.
		/// </summary>
		/// <exception cref="IOException"> if writing to the specified output stream
		///         results in an <tt>IOException</tt>. </exception>
		/// <exception cref="BackingStoreException"> if preference data cannot be read from
		///         backing store. </exception>
		/// <exception cref="IllegalStateException"> if this node (or an ancestor) has been
		///         removed with the <seealso cref="Preferences#removeNode()"/> method. </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: static void export(OutputStream os, final Preferences p, boolean subTree) throws IOException, BackingStoreException
//JAVA TO C# CONVERTER WARNING: 'final' parameters are not allowed in .NET:
		internal static void Export(OutputStream os, Preferences p, bool subTree)
		{
			if (((AbstractPreferences)p).Removed)
			{
				throw new IllegalStateException("Node has been removed");
			}
			Document doc = CreatePrefsDoc("preferences");
			Element preferences = doc.DocumentElement;
			preferences.setAttribute("EXTERNAL_XML_VERSION", EXTERNAL_XML_VERSION);
			Element xmlRoot = (Element) preferences.appendChild(doc.createElement("root"));
			xmlRoot.setAttribute("type", (p.UserNode ? "user" : "system"));

			// Get bottom-up list of nodes from p to root, excluding root
			List<Preferences> ancestors = new List<Preferences>();

			for (Preferences kid = p, dad = kid.Parent(); dad != null; kid = dad, dad = kid.Parent())
			{
				ancestors.Add(kid);
			}
			Element e = xmlRoot;
			for (int i = ancestors.Count - 1; i >= 0; i--)
			{
				e.appendChild(doc.createElement("map"));
				e = (Element) e.appendChild(doc.createElement("node"));
				e.setAttribute("name", ancestors.Get(i).Name());
			}
			PutPreferencesInXml(e, doc, p, subTree);

			WriteDoc(doc, os);
		}

		/// <summary>
		/// Put the preferences in the specified Preferences node into the
		/// specified XML element which is assumed to represent a node
		/// in the specified XML document which is assumed to conform to
		/// PREFS_DTD.  If subTree is true, create children of the specified
		/// XML node conforming to all of the children of the specified
		/// Preferences node and recurse.
		/// </summary>
		/// <exception cref="BackingStoreException"> if it is not possible to read
		///         the preferences or children out of the specified
		///         preferences node. </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: private static void putPreferencesInXml(Element elt, Document doc, Preferences prefs, boolean subTree) throws BackingStoreException
		private static void PutPreferencesInXml(Element elt, Document doc, Preferences prefs, bool subTree)
		{
			Preferences[] kidsCopy = null;
			String[] kidNames = null;

			// Node is locked to export its contents and get a
			// copy of children, then lock is released,
			// and, if subTree = true, recursive calls are made on children
			lock (((AbstractPreferences)prefs).@lock)
			{
				// Check if this node was concurrently removed. If yes
				// remove it from XML Document and return.
				if (((AbstractPreferences)prefs).Removed)
				{
					elt.ParentNode.removeChild(elt);
					return;
				}
				// Put map in xml element
				String[] keys = prefs.Keys();
				Element map = (Element) elt.appendChild(doc.createElement("map"));
				for (int i = 0; i < keys.Length; i++)
				{
					Element entry = (Element) map.appendChild(doc.createElement("entry"));
					entry.setAttribute("key", keys[i]);
					// NEXT STATEMENT THROWS NULL PTR EXC INSTEAD OF ASSERT FAIL
					entry.setAttribute("value", prefs.Get(keys[i], null));
				}
				// Recurse if appropriate
				if (subTree)
				{
					/* Get a copy of kids while lock is held */
					kidNames = prefs.ChildrenNames();
					kidsCopy = new Preferences[kidNames.Length];
					for (int i = 0; i < kidNames.Length; i++)
					{
						kidsCopy[i] = prefs.Node(kidNames[i]);
					}
				}
				// release lock
			}

			if (subTree)
			{
				for (int i = 0; i < kidNames.Length; i++)
				{
					Element xmlKid = (Element) elt.appendChild(doc.createElement("node"));
					xmlKid.setAttribute("name", kidNames[i]);
					PutPreferencesInXml(xmlKid, doc, kidsCopy[i], subTree);
				}
			}
		}

		/// <summary>
		/// Import preferences from the specified input stream, which is assumed
		/// to contain an XML document in the format described in the Preferences
		/// spec.
		/// </summary>
		/// <exception cref="IOException"> if reading from the specified output stream
		///         results in an <tt>IOException</tt>. </exception>
		/// <exception cref="InvalidPreferencesFormatException"> Data on input stream does not
		///         constitute a valid XML document with the mandated document type. </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: static void importPreferences(InputStream is) throws IOException, InvalidPreferencesFormatException
		internal static void ImportPreferences(InputStream @is)
		{
			try
			{
				Document doc = LoadPrefsDoc(@is);
				String xmlVersion = doc.DocumentElement.getAttribute("EXTERNAL_XML_VERSION");
				if (xmlVersion.CompareTo(EXTERNAL_XML_VERSION) > 0)
				{
					throw new InvalidPreferencesFormatException("Exported preferences file format version " + xmlVersion + " is not supported. This java installation can read" + " versions " + EXTERNAL_XML_VERSION + " or older. You may need" + " to install a newer version of JDK.");
				}

				Element xmlRoot = (Element) doc.DocumentElement.ChildNodes.item(0);
				Preferences prefsRoot = (xmlRoot.getAttribute("type").Equals("user") ? Preferences.UserRoot() : Preferences.SystemRoot());
				ImportSubtree(prefsRoot, xmlRoot);
			}
			catch (SAXException e)
			{
				throw new InvalidPreferencesFormatException(e);
			}
		}

		/// <summary>
		/// Create a new prefs XML document.
		/// </summary>
		private static Document CreatePrefsDoc(String qname)
		{
			try
			{
				DOMImplementation di = DocumentBuilderFactory.newInstance().newDocumentBuilder().DOMImplementation;
				DocumentType dt = di.createDocumentType(qname, null, PREFS_DTD_URI);
				return di.createDocument(null, qname, dt);
			}
			catch (ParserConfigurationException e)
			{
				throw new AssertionError(e);
			}
		}

		/// <summary>
		/// Load an XML document from specified input stream, which must
		/// have the requisite DTD URI.
		/// </summary>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: private static Document loadPrefsDoc(InputStream in) throws SAXException, IOException
		private static Document LoadPrefsDoc(InputStream @in)
		{
			DocumentBuilderFactory dbf = DocumentBuilderFactory.newInstance();
			dbf.IgnoringElementContentWhitespace = true;
			dbf.Validating = true;
			dbf.Coalescing = true;
			dbf.IgnoringComments = true;
			try
			{
				DocumentBuilder db = dbf.newDocumentBuilder();
				db.EntityResolver = new Resolver();
				db.ErrorHandler = new EH();
				return db.parse(new InputSource(@in));
			}
			catch (ParserConfigurationException e)
			{
				throw new AssertionError(e);
			}
		}

		/// <summary>
		/// Write XML document to the specified output stream.
		/// </summary>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: private static final void writeDoc(Document doc, OutputStream out) throws IOException
		private static void WriteDoc(Document doc, OutputStream @out)
		{
			try
			{
				TransformerFactory tf = TransformerFactory.newInstance();
				try
				{
					tf.setAttribute("indent-number", new Integer(2));
				}
				catch (IllegalArgumentException)
				{
					//Ignore the IAE. Should not fail the writeout even the
					//transformer provider does not support "indent-number".
				}
				Transformer t = tf.newTransformer();
				t.setOutputProperty(OutputKeys.DOCTYPE_SYSTEM, doc.Doctype.SystemId);
				t.setOutputProperty(OutputKeys.INDENT, "yes");
				//Transformer resets the "indent" info if the "result" is a StreamResult with
				//an OutputStream object embedded, creating a Writer object on top of that
				//OutputStream object however works.
				t.transform(new DOMSource(doc), new StreamResult(new BufferedWriter(new OutputStreamWriter(@out, "UTF-8"))));
			}
			catch (TransformerException e)
			{
				throw new AssertionError(e);
			}
		}

		/// <summary>
		/// Recursively traverse the specified preferences node and store
		/// the described preferences into the system or current user
		/// preferences tree, as appropriate.
		/// </summary>
		private static void ImportSubtree(Preferences prefsNode, Element xmlNode)
		{
			NodeList xmlKids = xmlNode.ChildNodes;
			int numXmlKids = xmlKids.Length;
			/*
			 * We first lock the node, import its contents and get
			 * child nodes. Then we unlock the node and go to children
			 * Since some of the children might have been concurrently
			 * deleted we check for this.
			 */
			Preferences[] prefsKids;
			/* Lock the node */
			lock (((AbstractPreferences)prefsNode).@lock)
			{
				//If removed, return silently
				if (((AbstractPreferences)prefsNode).Removed)
				{
					return;
				}

				// Import any preferences at this node
				Element firstXmlKid = (Element) xmlKids.item(0);
				ImportPrefs(prefsNode, firstXmlKid);
				prefsKids = new Preferences[numXmlKids - 1];

				// Get involved children
				for (int i = 1; i < numXmlKids; i++)
				{
					Element xmlKid = (Element) xmlKids.item(i);
					prefsKids[i - 1] = prefsNode.Node(xmlKid.getAttribute("name"));
				}
			} // unlocked the node
			// import children
			for (int i = 1; i < numXmlKids; i++)
			{
				ImportSubtree(prefsKids[i - 1], (Element)xmlKids.item(i));
			}
		}

		/// <summary>
		/// Import the preferences described by the specified XML element
		/// (a map from a preferences document) into the specified
		/// preferences node.
		/// </summary>
		private static void ImportPrefs(Preferences prefsNode, Element map)
		{
			NodeList entries = map.ChildNodes;
			for (int i = 0, numEntries = entries.Length; i < numEntries; i++)
			{
				Element entry = (Element) entries.item(i);
				prefsNode.Put(entry.getAttribute("key"), entry.getAttribute("value"));
			}
		}

		/// <summary>
		/// Export the specified Map<String,String> to a map document on
		/// the specified OutputStream as per the prefs DTD.  This is used
		/// as the internal (undocumented) format for FileSystemPrefs.
		/// </summary>
		/// <exception cref="IOException"> if writing to the specified output stream
		///         results in an <tt>IOException</tt>. </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: static void exportMap(OutputStream os, Map<String, String> map) throws IOException
		internal static void ExportMap(OutputStream os, Map<String, String> map)
		{
			Document doc = CreatePrefsDoc("map");
			Element xmlMap = doc.DocumentElement;
			xmlMap.setAttribute("MAP_XML_VERSION", MAP_XML_VERSION);

			for (Iterator<Map_Entry<String, String>> i = map.EntrySet().Iterator(); i.HasNext();)
			{
				Map_Entry<String, String> e = i.Next();
				Element xe = (Element) xmlMap.appendChild(doc.createElement("entry"));
				xe.setAttribute("key", e.Key);
				xe.setAttribute("value", e.Value);
			}

			WriteDoc(doc, os);
		}

		/// <summary>
		/// Import Map from the specified input stream, which is assumed
		/// to contain a map document as per the prefs DTD.  This is used
		/// as the internal (undocumented) format for FileSystemPrefs.  The
		/// key-value pairs specified in the XML document will be put into
		/// the specified Map.  (If this Map is empty, it will contain exactly
		/// the key-value pairs int the XML-document when this method returns.)
		/// </summary>
		/// <exception cref="IOException"> if reading from the specified output stream
		///         results in an <tt>IOException</tt>. </exception>
		/// <exception cref="InvalidPreferencesFormatException"> Data on input stream does not
		///         constitute a valid XML document with the mandated document type. </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: static void importMap(InputStream is, Map<String, String> m) throws IOException, InvalidPreferencesFormatException
		internal static void ImportMap(InputStream @is, Map<String, String> m)
		{
			try
			{
				Document doc = LoadPrefsDoc(@is);
				Element xmlMap = doc.DocumentElement;
				// check version
				String mapVersion = xmlMap.getAttribute("MAP_XML_VERSION");
				if (mapVersion.CompareTo(MAP_XML_VERSION) > 0)
				{
					throw new InvalidPreferencesFormatException("Preferences map file format version " + mapVersion + " is not supported. This java installation can read" + " versions " + MAP_XML_VERSION + " or older. You may need" + " to install a newer version of JDK.");
				}

				NodeList entries = xmlMap.ChildNodes;
				for (int i = 0, numEntries = entries.Length; i < numEntries; i++)
				{
					Element entry = (Element) entries.item(i);
					m.Put(entry.getAttribute("key"), entry.getAttribute("value"));
				}
			}
			catch (SAXException e)
			{
				throw new InvalidPreferencesFormatException(e);
			}
		}

		private class Resolver : EntityResolver
		{
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public InputSource resolveEntity(String pid, String sid) throws SAXException
			public virtual InputSource ResolveEntity(String pid, String sid)
			{
				if (sid.Equals(PREFS_DTD_URI))
				{
					InputSource @is;
					@is = new InputSource(new StringReader(PREFS_DTD));
					@is.SystemId = PREFS_DTD_URI;
					return @is;
				}
				throw new SAXException("Invalid system identifier: " + sid);
			}
		}

		private class EH : ErrorHandler
		{
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public void error(SAXParseException x) throws SAXException
			public virtual void Error(SAXParseException x)
			{
				throw x;
			}
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public void fatalError(SAXParseException x) throws SAXException
			public virtual void FatalError(SAXParseException x)
			{
				throw x;
			}
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public void warning(SAXParseException x) throws SAXException
			public virtual void Warning(SAXParseException x)
			{
				throw x;
			}
		}
	}

}
/*
 * Copyright (c) 2005, 2013, Oracle and/or its affiliates. All rights reserved.
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

namespace java.sql
{



	/// <summary>
	/// The mapping in the JavaTM programming language for the SQL XML type.
	/// XML is a built-in type that stores an XML value
	/// as a column value in a row of a database table.
	/// By default drivers implement an SQLXML object as
	/// a logical pointer to the XML data
	/// rather than the data itself.
	/// An SQLXML object is valid for the duration of the transaction in which it was created.
	/// <para>
	/// The SQLXML interface provides methods for accessing the XML value
	/// as a String, a Reader or Writer, or as a Stream.  The XML value
	/// may also be accessed through a Source or set as a Result, which
	/// are used with XML Parser APIs such as DOM, SAX, and StAX, as
	/// well as with XSLT transforms and XPath evaluations.
	/// </para>
	/// <para>
	/// Methods in the interfaces ResultSet, CallableStatement, and PreparedStatement,
	/// such as getSQLXML allow a programmer to access an XML value.
	/// In addition, this interface has methods for updating an XML value.
	/// </para>
	/// <para>
	/// The XML value of the SQLXML instance may be obtained as a BinaryStream using
	/// <pre>
	///   SQLXML sqlxml = resultSet.getSQLXML(column);
	///   InputStream binaryStream = sqlxml.getBinaryStream();
	/// </pre>
	/// For example, to parse an XML value with a DOM parser:
	/// <pre>
	///   DocumentBuilder parser = DocumentBuilderFactory.newInstance().newDocumentBuilder();
	///   Document result = parser.parse(binaryStream);
	/// </pre>
	/// or to parse an XML value with a SAX parser to your handler:
	/// <pre>
	///   SAXParser parser = SAXParserFactory.newInstance().newSAXParser();
	///   parser.parse(binaryStream, myHandler);
	/// </pre>
	/// or to parse an XML value with a StAX parser:
	/// <pre>
	///   XMLInputFactory factory = XMLInputFactory.newInstance();
	///   XMLStreamReader streamReader = factory.createXMLStreamReader(binaryStream);
	/// </pre>
	/// </para>
	/// <para>
	/// Because databases may use an optimized representation for the XML,
	/// accessing the value through getSource() and
	/// setResult() can lead to improved processing performance
	/// without serializing to a stream representation and parsing the XML.
	/// </para>
	/// <para>
	/// For example, to obtain a DOM Document Node:
	/// <pre>
	///   DOMSource domSource = sqlxml.getSource(DOMSource.class);
	///   Document document = (Document) domSource.getNode();
	/// </pre>
	/// or to set the value to a DOM Document Node to myNode:
	/// <pre>
	///   DOMResult domResult = sqlxml.setResult(DOMResult.class);
	///   domResult.setNode(myNode);
	/// </pre>
	/// or, to send SAX events to your handler:
	/// <pre>
	///   SAXSource saxSource = sqlxml.getSource(SAXSource.class);
	///   XMLReader xmlReader = saxSource.getXMLReader();
	///   xmlReader.setContentHandler(myHandler);
	///   xmlReader.parse(saxSource.getInputSource());
	/// </pre>
	/// or, to set the result value from SAX events:
	/// <pre>
	///   SAXResult saxResult = sqlxml.setResult(SAXResult.class);
	///   ContentHandler contentHandler = saxResult.getHandler();
	///   contentHandler.startDocument();
	///   // set the XML elements and attributes into the result
	///   contentHandler.endDocument();
	/// </pre>
	/// or, to obtain StAX events:
	/// <pre>
	///   StAXSource staxSource = sqlxml.getSource(StAXSource.class);
	///   XMLStreamReader streamReader = staxSource.getXMLStreamReader();
	/// </pre>
	/// or, to set the result value from StAX events:
	/// <pre>
	///   StAXResult staxResult = sqlxml.setResult(StAXResult.class);
	///   XMLStreamWriter streamWriter = staxResult.getXMLStreamWriter();
	/// </pre>
	/// or, to perform XSLT transformations on the XML value using the XSLT in xsltFile
	/// output to file resultFile:
	/// <pre>
	///   File xsltFile = new File("a.xslt");
	///   File myFile = new File("result.xml");
	///   Transformer xslt = TransformerFactory.newInstance().newTransformer(new StreamSource(xsltFile));
	///   Source source = sqlxml.getSource(null);
	///   Result result = new StreamResult(myFile);
	///   xslt.transform(source, result);
	/// </pre>
	/// or, to evaluate an XPath expression on the XML value:
	/// <pre>
	///   XPath xpath = XPathFactory.newInstance().newXPath();
	///   DOMSource domSource = sqlxml.getSource(DOMSource.class);
	///   Document document = (Document) domSource.getNode();
	///   String expression = "/foo/@bar";
	///   String barValue = xpath.evaluate(expression, document);
	/// </pre>
	/// To set the XML value to be the result of an XSLT transform:
	/// <pre>
	///   File sourceFile = new File("source.xml");
	///   Transformer xslt = TransformerFactory.newInstance().newTransformer(new StreamSource(xsltFile));
	///   Source streamSource = new StreamSource(sourceFile);
	///   Result result = sqlxml.setResult(null);
	///   xslt.transform(streamSource, result);
	/// </pre>
	/// Any Source can be transformed to a Result using the identity transform
	/// specified by calling newTransformer():
	/// <pre>
	///   Transformer identity = TransformerFactory.newInstance().newTransformer();
	///   Source source = sqlxml.getSource(null);
	///   File myFile = new File("result.xml");
	///   Result result = new StreamResult(myFile);
	///   identity.transform(source, result);
	/// </pre>
	/// To write the contents of a Source to standard output:
	/// <pre>
	///   Transformer identity = TransformerFactory.newInstance().newTransformer();
	///   Source source = sqlxml.getSource(null);
	///   Result result = new StreamResult(System.out);
	///   identity.transform(source, result);
	/// </pre>
	/// To create a DOMSource from a DOMResult:
	/// <pre>
	///    DOMSource domSource = new DOMSource(domResult.getNode());
	/// </pre>
	/// </para>
	/// <para>
	/// Incomplete or invalid XML values may cause an SQLException when
	/// set or the exception may occur when execute() occurs.  All streams
	/// must be closed before execute() occurs or an SQLException will be thrown.
	/// </para>
	/// <para>
	/// Reading and writing XML values to or from an SQLXML object can happen at most once.
	/// The conceptual states of readable and not readable determine if one
	/// of the reading APIs will return a value or throw an exception.
	/// The conceptual states of writable and not writable determine if one
	/// of the writing APIs will set a value or throw an exception.
	/// </para>
	/// <para>
	/// The state moves from readable to not readable once free() or any of the
	/// reading APIs are called: getBinaryStream(), getCharacterStream(), getSource(), and getString().
	/// Implementations may also change the state to not writable when this occurs.
	/// </para>
	/// <para>
	/// The state moves from writable to not writeable once free() or any of the
	/// writing APIs are called: setBinaryStream(), setCharacterStream(), setResult(), and setString().
	/// Implementations may also change the state to not readable when this occurs.
	/// 
	/// </para>
	/// <para>
	/// All methods on the <code>SQLXML</code> interface must be fully implemented if the
	/// JDBC driver supports the data type.
	/// 
	/// </para>
	/// </summary>
	/// <seealso cref= javax.xml.parsers </seealso>
	/// <seealso cref= javax.xml.stream </seealso>
	/// <seealso cref= javax.xml.transform </seealso>
	/// <seealso cref= javax.xml.xpath
	/// @since 1.6 </seealso>
	public interface SQLXML
	{
	  /// <summary>
	  /// This method closes this object and releases the resources that it held.
	  /// The SQL XML object becomes invalid and neither readable or writeable
	  /// when this method is called.
	  /// 
	  /// After <code>free</code> has been called, any attempt to invoke a
	  /// method other than <code>free</code> will result in a <code>SQLException</code>
	  /// being thrown.  If <code>free</code> is called multiple times, the subsequent
	  /// calls to <code>free</code> are treated as a no-op. </summary>
	  /// <exception cref="SQLException"> if there is an error freeing the XML value. </exception>
	  /// <exception cref="SQLFeatureNotSupportedException"> if the JDBC driver does not support
	  /// this method
	  /// @since 1.6 </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: void free() throws SQLException;
	  void Free();

	  /// <summary>
	  /// Retrieves the XML value designated by this SQLXML instance as a stream.
	  /// The bytes of the input stream are interpreted according to appendix F of the XML 1.0 specification.
	  /// The behavior of this method is the same as ResultSet.getBinaryStream()
	  /// when the designated column of the ResultSet has a type java.sql.Types of SQLXML.
	  /// <para>
	  /// The SQL XML object becomes not readable when this method is called and
	  /// may also become not writable depending on implementation.
	  /// 
	  /// </para>
	  /// </summary>
	  /// <returns> a stream containing the XML data. </returns>
	  /// <exception cref="SQLException"> if there is an error processing the XML value.
	  ///   An exception is thrown if the state is not readable. </exception>
	  /// <exception cref="SQLFeatureNotSupportedException"> if the JDBC driver does not support
	  /// this method
	  /// @since 1.6 </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: java.io.InputStream getBinaryStream() throws SQLException;
	  InputStream BinaryStream {get;}

	  /// <summary>
	  /// Retrieves a stream that can be used to write the XML value that this SQLXML instance represents.
	  /// The stream begins at position 0.
	  /// The bytes of the stream are interpreted according to appendix F of the XML 1.0 specification
	  /// The behavior of this method is the same as ResultSet.updateBinaryStream()
	  /// when the designated column of the ResultSet has a type java.sql.Types of SQLXML.
	  /// <para>
	  /// The SQL XML object becomes not writeable when this method is called and
	  /// may also become not readable depending on implementation.
	  /// 
	  /// </para>
	  /// </summary>
	  /// <returns> a stream to which data can be written. </returns>
	  /// <exception cref="SQLException"> if there is an error processing the XML value.
	  ///   An exception is thrown if the state is not writable. </exception>
	  /// <exception cref="SQLFeatureNotSupportedException"> if the JDBC driver does not support
	  /// this method
	  /// @since 1.6 </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: java.io.OutputStream setBinaryStream() throws SQLException;
	  OutputStream SetBinaryStream();

	  /// <summary>
	  /// Retrieves the XML value designated by this SQLXML instance as a java.io.Reader object.
	  /// The format of this stream is defined by org.xml.sax.InputSource,
	  /// where the characters in the stream represent the unicode code points for
	  /// XML according to section 2 and appendix B of the XML 1.0 specification.
	  /// Although an encoding declaration other than unicode may be present,
	  /// the encoding of the stream is unicode.
	  /// The behavior of this method is the same as ResultSet.getCharacterStream()
	  /// when the designated column of the ResultSet has a type java.sql.Types of SQLXML.
	  /// <para>
	  /// The SQL XML object becomes not readable when this method is called and
	  /// may also become not writable depending on implementation.
	  /// 
	  /// </para>
	  /// </summary>
	  /// <returns> a stream containing the XML data. </returns>
	  /// <exception cref="SQLException"> if there is an error processing the XML value.
	  ///   The getCause() method of the exception may provide a more detailed exception, for example,
	  ///   if the stream does not contain valid characters.
	  ///   An exception is thrown if the state is not readable. </exception>
	  /// <exception cref="SQLFeatureNotSupportedException"> if the JDBC driver does not support
	  /// this method
	  /// @since 1.6 </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: java.io.Reader getCharacterStream() throws SQLException;
	  Reader CharacterStream {get;}

	  /// <summary>
	  /// Retrieves a stream to be used to write the XML value that this SQLXML instance represents.
	  /// The format of this stream is defined by org.xml.sax.InputSource,
	  /// where the characters in the stream represent the unicode code points for
	  /// XML according to section 2 and appendix B of the XML 1.0 specification.
	  /// Although an encoding declaration other than unicode may be present,
	  /// the encoding of the stream is unicode.
	  /// The behavior of this method is the same as ResultSet.updateCharacterStream()
	  /// when the designated column of the ResultSet has a type java.sql.Types of SQLXML.
	  /// <para>
	  /// The SQL XML object becomes not writeable when this method is called and
	  /// may also become not readable depending on implementation.
	  /// 
	  /// </para>
	  /// </summary>
	  /// <returns> a stream to which data can be written. </returns>
	  /// <exception cref="SQLException"> if there is an error processing the XML value.
	  ///   The getCause() method of the exception may provide a more detailed exception, for example,
	  ///   if the stream does not contain valid characters.
	  ///   An exception is thrown if the state is not writable. </exception>
	  /// <exception cref="SQLFeatureNotSupportedException"> if the JDBC driver does not support
	  /// this method
	  /// @since 1.6 </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: java.io.Writer setCharacterStream() throws SQLException;
	  Writer SetCharacterStream();

	  /// <summary>
	  /// Returns a string representation of the XML value designated by this SQLXML instance.
	  /// The format of this String is defined by org.xml.sax.InputSource,
	  /// where the characters in the stream represent the unicode code points for
	  /// XML according to section 2 and appendix B of the XML 1.0 specification.
	  /// Although an encoding declaration other than unicode may be present,
	  /// the encoding of the String is unicode.
	  /// The behavior of this method is the same as ResultSet.getString()
	  /// when the designated column of the ResultSet has a type java.sql.Types of SQLXML.
	  /// <para>
	  /// The SQL XML object becomes not readable when this method is called and
	  /// may also become not writable depending on implementation.
	  /// 
	  /// </para>
	  /// </summary>
	  /// <returns> a string representation of the XML value designated by this SQLXML instance. </returns>
	  /// <exception cref="SQLException"> if there is an error processing the XML value.
	  ///   The getCause() method of the exception may provide a more detailed exception, for example,
	  ///   if the stream does not contain valid characters.
	  ///   An exception is thrown if the state is not readable. </exception>
	  /// <exception cref="SQLFeatureNotSupportedException"> if the JDBC driver does not support
	  /// this method
	  /// @since 1.6 </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: String getString() throws SQLException;
	  String String {get;set;}

	  /// <summary>
	  /// Sets the XML value designated by this SQLXML instance to the given String representation.
	  /// The format of this String is defined by org.xml.sax.InputSource,
	  /// where the characters in the stream represent the unicode code points for
	  /// XML according to section 2 and appendix B of the XML 1.0 specification.
	  /// Although an encoding declaration other than unicode may be present,
	  /// the encoding of the String is unicode.
	  /// The behavior of this method is the same as ResultSet.updateString()
	  /// when the designated column of the ResultSet has a type java.sql.Types of SQLXML.
	  /// <para>
	  /// The SQL XML object becomes not writeable when this method is called and
	  /// may also become not readable depending on implementation.
	  /// 
	  /// </para>
	  /// </summary>
	  /// <param name="value"> the XML value </param>
	  /// <exception cref="SQLException"> if there is an error processing the XML value.
	  ///   The getCause() method of the exception may provide a more detailed exception, for example,
	  ///   if the stream does not contain valid characters.
	  ///   An exception is thrown if the state is not writable. </exception>
	  /// <exception cref="SQLFeatureNotSupportedException"> if the JDBC driver does not support
	  /// this method
	  /// @since 1.6 </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: void setString(String value) throws SQLException;

	  /// <summary>
	  /// Returns a Source for reading the XML value designated by this SQLXML instance.
	  /// Sources are used as inputs to XML parsers and XSLT transformers.
	  /// <para>
	  /// Sources for XML parsers will have namespace processing on by default.
	  /// The systemID of the Source is implementation dependent.
	  /// </para>
	  /// <para>
	  /// The SQL XML object becomes not readable when this method is called and
	  /// may also become not writable depending on implementation.
	  /// </para>
	  /// <para>
	  /// Note that SAX is a callback architecture, so a returned
	  /// SAXSource should then be set with a content handler that will
	  /// receive the SAX events from parsing.  The content handler
	  /// will receive callbacks based on the contents of the XML.
	  /// <pre>
	  ///   SAXSource saxSource = sqlxml.getSource(SAXSource.class);
	  ///   XMLReader xmlReader = saxSource.getXMLReader();
	  ///   xmlReader.setContentHandler(myHandler);
	  ///   xmlReader.parse(saxSource.getInputSource());
	  /// </pre>
	  /// 
	  /// </para>
	  /// </summary>
	  /// @param <T> the type of the class modeled by this Class object </param>
	  /// <param name="sourceClass"> The class of the source, or null.
	  /// If the class is null, a vendor specific Source implementation will be returned.
	  /// The following classes are supported at a minimum:
	  /// <pre>
	  ///   javax.xml.transform.dom.DOMSource - returns a DOMSource
	  ///   javax.xml.transform.sax.SAXSource - returns a SAXSource
	  ///   javax.xml.transform.stax.StAXSource - returns a StAXSource
	  ///   javax.xml.transform.stream.StreamSource - returns a StreamSource
	  /// </pre> </param>
	  /// <returns> a Source for reading the XML value. </returns>
	  /// <exception cref="SQLException"> if there is an error processing the XML value
	  ///   or if this feature is not supported.
	  ///   The getCause() method of the exception may provide a more detailed exception, for example,
	  ///   if an XML parser exception occurs.
	  ///   An exception is thrown if the state is not readable. </exception>
	  /// <exception cref="SQLFeatureNotSupportedException"> if the JDBC driver does not support
	  /// this method
	  /// @since 1.6 </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: <T extends javax.xml.transform.Source> T getSource(Class sourceClass) throws SQLException;
	  T getSource<T>(Class sourceClass) where T : javax.xml.transform.Source;

	  /// <summary>
	  /// Returns a Result for setting the XML value designated by this SQLXML instance.
	  /// <para>
	  /// The systemID of the Result is implementation dependent.
	  /// </para>
	  /// <para>
	  /// The SQL XML object becomes not writeable when this method is called and
	  /// may also become not readable depending on implementation.
	  /// </para>
	  /// <para>
	  /// Note that SAX is a callback architecture and the returned
	  /// SAXResult has a content handler assigned that will receive the
	  /// SAX events based on the contents of the XML.  Call the content
	  /// handler with the contents of the XML document to assign the values.
	  /// <pre>
	  ///   SAXResult saxResult = sqlxml.setResult(SAXResult.class);
	  ///   ContentHandler contentHandler = saxResult.getXMLReader().getContentHandler();
	  ///   contentHandler.startDocument();
	  ///   // set the XML elements and attributes into the result
	  ///   contentHandler.endDocument();
	  /// </pre>
	  /// 
	  /// </para>
	  /// </summary>
	  /// @param <T> the type of the class modeled by this Class object </param>
	  /// <param name="resultClass"> The class of the result, or null.
	  /// If resultClass is null, a vendor specific Result implementation will be returned.
	  /// The following classes are supported at a minimum:
	  /// <pre>
	  ///   javax.xml.transform.dom.DOMResult - returns a DOMResult
	  ///   javax.xml.transform.sax.SAXResult - returns a SAXResult
	  ///   javax.xml.transform.stax.StAXResult - returns a StAXResult
	  ///   javax.xml.transform.stream.StreamResult - returns a StreamResult
	  /// </pre> </param>
	  /// <returns> Returns a Result for setting the XML value. </returns>
	  /// <exception cref="SQLException"> if there is an error processing the XML value
	  ///   or if this feature is not supported.
	  ///   The getCause() method of the exception may provide a more detailed exception, for example,
	  ///   if an XML parser exception occurs.
	  ///   An exception is thrown if the state is not writable. </exception>
	  /// <exception cref="SQLFeatureNotSupportedException"> if the JDBC driver does not support
	  /// this method
	  /// @since 1.6 </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: <T extends javax.xml.transform.Result> T setResult(Class resultClass) throws SQLException;
	  T setResult<T>(Class resultClass) where T : javax.xml.transform.Result;

	}

}
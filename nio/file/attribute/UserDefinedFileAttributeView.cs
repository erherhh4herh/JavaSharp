using System.Collections.Generic;

/*
 * Copyright (c) 2007, 2013, Oracle and/or its affiliates. All rights reserved.
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

namespace java.nio.file.attribute
{


	/// <summary>
	/// A file attribute view that provides a view of a file's user-defined
	/// attributes, sometimes known as <em>extended attributes</em>. User-defined
	/// file attributes are used to store metadata with a file that is not meaningful
	/// to the file system. It is primarily intended for file system implementations
	/// that support such a capability directly but may be emulated. The details of
	/// such emulation are highly implementation specific and therefore not specified.
	/// 
	/// <para> This {@code FileAttributeView} provides a view of a file's user-defined
	/// attributes as a set of name/value pairs, where the attribute name is
	/// represented by a {@code String}. An implementation may require to encode and
	/// decode from the platform or file system representation when accessing the
	/// attribute. The value has opaque content. This attribute view defines the
	/// <seealso cref="#read read"/> and <seealso cref="#write write"/> methods to read the value into
	/// or write from a <seealso cref="ByteBuffer"/>. This {@code FileAttributeView} is not
	/// intended for use where the size of an attribute value is larger than {@link
	/// Integer#MAX_VALUE}.
	/// 
	/// </para>
	/// <para> User-defined attributes may be used in some implementations to store
	/// security related attributes so consequently, in the case of the default
	/// provider at least, all methods that access user-defined attributes require the
	/// {@code RuntimePermission("accessUserDefinedAttributes")} permission when a
	/// security manager is installed.
	/// 
	/// </para>
	/// <para> The {@link java.nio.file.FileStore#supportsFileAttributeView
	/// supportsFileAttributeView} method may be used to test if a specific {@link
	/// java.nio.file.FileStore FileStore} supports the storage of user-defined
	/// attributes.
	/// 
	/// </para>
	/// <para> Where dynamic access to file attributes is required, the {@link
	/// java.nio.file.Files#getAttribute getAttribute} method may be used to read
	/// the attribute value. The attribute value is returned as a byte array (byte[]).
	/// The <seealso cref="java.nio.file.Files#setAttribute setAttribute"/> method may be used
	/// to write the value of a user-defined attribute from a buffer (as if by
	/// invoking the <seealso cref="#write write"/> method), or byte array (byte[]).
	/// 
	/// @since 1.7
	/// </para>
	/// </summary>

	public interface UserDefinedFileAttributeView : FileAttributeView
	{
		/// <summary>
		/// Returns the name of this attribute view. Attribute views of this type
		/// have the name {@code "user"}.
		/// </summary>
		String Name();

		/// <summary>
		/// Returns a list containing the names of the user-defined attributes.
		/// </summary>
		/// <returns>  An unmodifiable list containing the names of the file's
		///          user-defined
		/// </returns>
		/// <exception cref="IOException">
		///          If an I/O error occurs </exception>
		/// <exception cref="SecurityException">
		///          In the case of the default provider, a security manager is
		///          installed, and it denies {@link
		///          RuntimePermission}<tt>("accessUserDefinedAttributes")</tt>
		///          or its <seealso cref="SecurityManager#checkRead(String) checkRead"/> method
		///          denies read access to the file. </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: java.util.List<String> list() throws java.io.IOException;
		IList<String> List();

		/// <summary>
		/// Returns the size of the value of a user-defined attribute.
		/// </summary>
		/// <param name="name">
		///          The attribute name
		/// </param>
		/// <returns>  The size of the attribute value, in bytes.
		/// </returns>
		/// <exception cref="ArithmeticException">
		///          If the size of the attribute is larger than <seealso cref="Integer#MAX_VALUE"/> </exception>
		/// <exception cref="IOException">
		///          If an I/O error occurs </exception>
		/// <exception cref="SecurityException">
		///          In the case of the default provider, a security manager is
		///          installed, and it denies {@link
		///          RuntimePermission}<tt>("accessUserDefinedAttributes")</tt>
		///          or its <seealso cref="SecurityManager#checkRead(String) checkRead"/> method
		///          denies read access to the file. </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: int size(String name) throws java.io.IOException;
		int Size(String name);

		/// <summary>
		/// Read the value of a user-defined attribute into a buffer.
		/// 
		/// <para> This method reads the value of the attribute into the given buffer
		/// as a sequence of bytes, failing if the number of bytes remaining in
		/// the buffer is insufficient to read the complete attribute value. The
		/// number of bytes transferred into the buffer is {@code n}, where {@code n}
		/// is the size of the attribute value. The first byte in the sequence is at
		/// index {@code p} and the last byte is at index {@code p + n - 1}, where
		/// {@code p} is the buffer's position. Upon return the buffer's position
		/// will be equal to {@code p + n}; its limit will not have changed.
		/// 
		/// </para>
		/// <para> <b>Usage Example:</b>
		/// Suppose we want to read a file's MIME type that is stored as a user-defined
		/// attribute with the name "{@code user.mimetype}".
		/// <pre>
		///    UserDefinedFileAttributeView view =
		///        Files.getFileAttributeView(path, UserDefinedFileAttributeView.class);
		///    String name = "user.mimetype";
		///    ByteBuffer buf = ByteBuffer.allocate(view.size(name));
		///    view.read(name, buf);
		///    buf.flip();
		///    String value = Charset.defaultCharset().decode(buf).toString();
		/// </pre>
		/// 
		/// </para>
		/// </summary>
		/// <param name="name">
		///          The attribute name </param>
		/// <param name="dst">
		///          The destination buffer
		/// </param>
		/// <returns>  The number of bytes read, possibly zero
		/// </returns>
		/// <exception cref="IllegalArgumentException">
		///          If the destination buffer is read-only </exception>
		/// <exception cref="IOException">
		///          If an I/O error occurs or there is insufficient space in the
		///          destination buffer for the attribute value </exception>
		/// <exception cref="SecurityException">
		///          In the case of the default provider, a security manager is
		///          installed, and it denies {@link
		///          RuntimePermission}<tt>("accessUserDefinedAttributes")</tt>
		///          or its <seealso cref="SecurityManager#checkRead(String) checkRead"/> method
		///          denies read access to the file.
		/// </exception>
		/// <seealso cref= #size </seealso>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: int read(String name, java.nio.ByteBuffer dst) throws java.io.IOException;
		int Read(String name, ByteBuffer dst);

		/// <summary>
		/// Writes the value of a user-defined attribute from a buffer.
		/// 
		/// <para> This method writes the value of the attribute from a given buffer as
		/// a sequence of bytes. The size of the value to transfer is {@code r},
		/// where {@code r} is the number of bytes remaining in the buffer, that is
		/// {@code src.remaining()}. The sequence of bytes is transferred from the
		/// buffer starting at index {@code p}, where {@code p} is the buffer's
		/// position. Upon return, the buffer's position will be equal to {@code
		/// p + n}, where {@code n} is the number of bytes transferred; its limit
		/// will not have changed.
		/// 
		/// </para>
		/// <para> If an attribute of the given name already exists then its value is
		/// replaced. If the attribute does not exist then it is created. If it
		/// implementation specific if a test to check for the existence of the
		/// attribute and the creation of attribute are atomic with respect to other
		/// file system activities.
		/// 
		/// </para>
		/// <para> Where there is insufficient space to store the attribute, or the
		/// attribute name or value exceed an implementation specific maximum size
		/// then an {@code IOException} is thrown.
		/// 
		/// </para>
		/// <para> <b>Usage Example:</b>
		/// Suppose we want to write a file's MIME type as a user-defined attribute:
		/// <pre>
		///    UserDefinedFileAttributeView view =
		///        FIles.getFileAttributeView(path, UserDefinedFileAttributeView.class);
		///    view.write("user.mimetype", Charset.defaultCharset().encode("text/html"));
		/// </pre>
		/// 
		/// </para>
		/// </summary>
		/// <param name="name">
		///          The attribute name </param>
		/// <param name="src">
		///          The buffer containing the attribute value
		/// </param>
		/// <returns>  The number of bytes written, possibly zero
		/// </returns>
		/// <exception cref="IOException">
		///          If an I/O error occurs </exception>
		/// <exception cref="SecurityException">
		///          In the case of the default provider, a security manager is
		///          installed, and it denies {@link
		///          RuntimePermission}<tt>("accessUserDefinedAttributes")</tt>
		///          or its <seealso cref="SecurityManager#checkWrite(String) checkWrite"/>
		///          method denies write access to the file. </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: int write(String name, java.nio.ByteBuffer src) throws java.io.IOException;
		int Write(String name, ByteBuffer src);

		/// <summary>
		/// Deletes a user-defined attribute.
		/// </summary>
		/// <param name="name">
		///          The attribute name
		/// </param>
		/// <exception cref="IOException">
		///          If an I/O error occurs or the attribute does not exist </exception>
		/// <exception cref="SecurityException">
		///          In the case of the default provider, a security manager is
		///          installed, and it denies {@link
		///          RuntimePermission}<tt>("accessUserDefinedAttributes")</tt>
		///          or its <seealso cref="SecurityManager#checkWrite(String) checkWrite"/>
		///          method denies write access to the file. </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: void delete(String name) throws java.io.IOException;
		void Delete(String name);
	}

}
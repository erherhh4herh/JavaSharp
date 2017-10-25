using System;

/*
 * Copyright (c) 2000, 2012, Oracle and/or its affiliates. All rights reserved.
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

	using DocumentHandler = com.sun.beans.decoder.DocumentHandler;


	using InputSource = org.xml.sax.InputSource;
	using DefaultHandler = org.xml.sax.helpers.DefaultHandler;

	/// <summary>
	/// The <code>XMLDecoder</code> class is used to read XML documents
	/// created using the <code>XMLEncoder</code> and is used just like
	/// the <code>ObjectInputStream</code>. For example, one can use
	/// the following fragment to read the first object defined
	/// in an XML document written by the <code>XMLEncoder</code>
	/// class:
	/// <pre>
	///       XMLDecoder d = new XMLDecoder(
	///                          new BufferedInputStream(
	///                              new FileInputStream("Test.xml")));
	///       Object result = d.readObject();
	///       d.close();
	/// </pre>
	/// 
	/// <para>
	/// For more information you might also want to check out
	/// <a
	/// href="http://java.sun.com/products/jfc/tsc/articles/persistence3">Long Term Persistence of JavaBeans Components: XML Schema</a>,
	/// an article in <em>The Swing Connection.</em>
	/// </para>
	/// </summary>
	/// <seealso cref= XMLEncoder </seealso>
	/// <seealso cref= java.io.ObjectInputStream
	/// 
	/// @since 1.4
	/// 
	/// @author Philip Milne </seealso>
	public class XMLDecoder : AutoCloseable
	{
		private readonly AccessControlContext Acc = AccessController.Context;
		private readonly DocumentHandler Handler = new DocumentHandler();
		private readonly InputSource Input;
		private Object Owner_Renamed;
		private Object[] Array;
		private int Index;

		/// <summary>
		/// Creates a new input stream for reading archives
		/// created by the <code>XMLEncoder</code> class.
		/// </summary>
		/// <param name="in"> The underlying stream.
		/// </param>
		/// <seealso cref= XMLEncoder#XMLEncoder(java.io.OutputStream) </seealso>
		public XMLDecoder(InputStream @in) : this(@in, null)
		{
		}

		/// <summary>
		/// Creates a new input stream for reading archives
		/// created by the <code>XMLEncoder</code> class.
		/// </summary>
		/// <param name="in"> The underlying stream. </param>
		/// <param name="owner"> The owner of this stream.
		///  </param>
		public XMLDecoder(InputStream @in, Object owner) : this(@in, owner, null)
		{
		}

		/// <summary>
		/// Creates a new input stream for reading archives
		/// created by the <code>XMLEncoder</code> class.
		/// </summary>
		/// <param name="in"> the underlying stream. </param>
		/// <param name="owner"> the owner of this stream. </param>
		/// <param name="exceptionListener"> the exception handler for the stream;
		///        if <code>null</code> the default exception listener will be used. </param>
		public XMLDecoder(InputStream @in, Object owner, ExceptionListener exceptionListener) : this(@in, owner, exceptionListener, null)
		{
		}

		/// <summary>
		/// Creates a new input stream for reading archives
		/// created by the <code>XMLEncoder</code> class.
		/// </summary>
		/// <param name="in"> the underlying stream.  <code>null</code> may be passed without
		///        error, though the resulting XMLDecoder will be useless </param>
		/// <param name="owner"> the owner of this stream.  <code>null</code> is a legal
		///        value </param>
		/// <param name="exceptionListener"> the exception handler for the stream, or
		///        <code>null</code> to use the default </param>
		/// <param name="cl"> the class loader used for instantiating objects.
		///        <code>null</code> indicates that the default class loader should
		///        be used
		/// @since 1.5 </param>
		public XMLDecoder(InputStream @in, Object owner, ExceptionListener exceptionListener, ClassLoader cl) : this(new InputSource(@in), owner, exceptionListener, cl)
		{
		}


		/// <summary>
		/// Creates a new decoder to parse XML archives
		/// created by the {@code XMLEncoder} class.
		/// If the input source {@code is} is {@code null},
		/// no exception is thrown and no parsing is performed.
		/// This behavior is similar to behavior of other constructors
		/// that use {@code InputStream} as a parameter.
		/// </summary>
		/// <param name="is">  the input source to parse
		/// 
		/// @since 1.7 </param>
		public XMLDecoder(InputSource @is) : this(@is, null, null, null)
		{
		}

		/// <summary>
		/// Creates a new decoder to parse XML archives
		/// created by the {@code XMLEncoder} class.
		/// </summary>
		/// <param name="is">     the input source to parse </param>
		/// <param name="owner">  the owner of this decoder </param>
		/// <param name="el">     the exception handler for the parser,
		///               or {@code null} to use the default exception handler </param>
		/// <param name="cl">     the class loader used for instantiating objects,
		///               or {@code null} to use the default class loader
		/// 
		/// @since 1.7 </param>
		private XMLDecoder(InputSource @is, Object owner, ExceptionListener el, ClassLoader cl)
		{
			this.Input = @is;
			this.Owner_Renamed = owner;
			ExceptionListener = el;
			this.Handler.ClassLoader = cl;
			this.Handler.Owner = this;
		}

		/// <summary>
		/// This method closes the input stream associated
		/// with this stream.
		/// </summary>
		public virtual void Close()
		{
			if (ParsingComplete())
			{
				Close(this.Input.CharacterStream);
				Close(this.Input.ByteStream);
			}
		}

		private void Close(Closeable @in)
		{
			if (@in != null)
			{
				try
				{
					@in.Close();
				}
				catch (IOException e)
				{
					ExceptionListener.ExceptionThrown(e);
				}
			}
		}

		private bool ParsingComplete()
		{
			if (this.Input == null)
			{
				return false;
			}
			if (this.Array == null)
			{
				if ((this.Acc == null) && (null != System.SecurityManager))
				{
					throw new SecurityException("AccessControlContext is not set");
				}
				AccessController.doPrivileged(new PrivilegedActionAnonymousInnerClassHelper(this), this.Acc);
				this.Array = this.Handler.Objects;
			}
			return true;
		}

		private class PrivilegedActionAnonymousInnerClassHelper : PrivilegedAction<Void>
		{
			private readonly XMLDecoder OuterInstance;

			public PrivilegedActionAnonymousInnerClassHelper(XMLDecoder outerInstance)
			{
				this.OuterInstance = outerInstance;
			}

			public virtual Void Run()
			{
				OuterInstance.Handler.parse(OuterInstance.Input);
				return null;
			}
		}

		/// <summary>
		/// Sets the exception handler for this stream to <code>exceptionListener</code>.
		/// The exception handler is notified when this stream catches recoverable
		/// exceptions.
		/// </summary>
		/// <param name="exceptionListener"> The exception handler for this stream;
		/// if <code>null</code> the default exception listener will be used.
		/// </param>
		/// <seealso cref= #getExceptionListener </seealso>
		public virtual ExceptionListener ExceptionListener
		{
			set
			{
				if (value == null)
				{
					value = Statement.defaultExceptionListener;
				}
				this.Handler.ExceptionListener = value;
			}
			get
			{
				return this.Handler.ExceptionListener;
			}
		}


		/// <summary>
		/// Reads the next object from the underlying input stream.
		/// </summary>
		/// <returns> the next object read
		/// </returns>
		/// <exception cref="ArrayIndexOutOfBoundsException"> if the stream contains no objects
		///         (or no more objects)
		/// </exception>
		/// <seealso cref= XMLEncoder#writeObject </seealso>
		public virtual Object ReadObject()
		{
			return (ParsingComplete()) ? this.Array[this.Index++] : null;
		}

		/// <summary>
		/// Sets the owner of this decoder to <code>owner</code>.
		/// </summary>
		/// <param name="owner"> The owner of this decoder.
		/// </param>
		/// <seealso cref= #getOwner </seealso>
		public virtual Object Owner
		{
			set
			{
				this.Owner_Renamed = value;
			}
			get
			{
				return Owner_Renamed;
			}
		}


		/// <summary>
		/// Creates a new handler for SAX parser
		/// that can be used to parse embedded XML archives
		/// created by the {@code XMLEncoder} class.
		/// 
		/// The {@code owner} should be used if parsed XML document contains
		/// the method call within context of the &lt;java&gt; element.
		/// The {@code null} value may cause illegal parsing in such case.
		/// The same problem may occur, if the {@code owner} class
		/// does not contain expected method to call. See details <a
		/// href="http://java.sun.com/products/jfc/tsc/articles/persistence3/">here</a>.
		/// </summary>
		/// <param name="owner">  the owner of the default handler
		///               that can be used as a value of &lt;java&gt; element </param>
		/// <param name="el">     the exception handler for the parser,
		///               or {@code null} to use the default exception handler </param>
		/// <param name="cl">     the class loader used for instantiating objects,
		///               or {@code null} to use the default class loader </param>
		/// <returns> an instance of {@code DefaultHandler} for SAX parser
		/// 
		/// @since 1.7 </returns>
		public static DefaultHandler CreateHandler(Object owner, ExceptionListener el, ClassLoader cl)
		{
			DocumentHandler handler = new DocumentHandler();
			handler.Owner = owner;
			handler.ExceptionListener = el;
			handler.ClassLoader = cl;
			return handler;
		}
	}

}
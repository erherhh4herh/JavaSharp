using System;
using System.Collections.Generic;

/*
 * Copyright (c) 2000, 2013, Oracle and/or its affiliates. All rights reserved.
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
	/// The <code>XMLEncoder</code> class is a complementary alternative to
	/// the <code>ObjectOutputStream</code> and can used to generate
	/// a textual representation of a <em>JavaBean</em> in the same
	/// way that the <code>ObjectOutputStream</code> can
	/// be used to create binary representation of <code>Serializable</code>
	/// objects. For example, the following fragment can be used to create
	/// a textual representation the supplied <em>JavaBean</em>
	/// and all its properties:
	/// <pre>
	///       XMLEncoder e = new XMLEncoder(
	///                          new BufferedOutputStream(
	///                              new FileOutputStream("Test.xml")));
	///       e.writeObject(new JButton("Hello, world"));
	///       e.close();
	/// </pre>
	/// Despite the similarity of their APIs, the <code>XMLEncoder</code>
	/// class is exclusively designed for the purpose of archiving graphs
	/// of <em>JavaBean</em>s as textual representations of their public
	/// properties. Like Java source files, documents written this way
	/// have a natural immunity to changes in the implementations of the classes
	/// involved. The <code>ObjectOutputStream</code> continues to be recommended
	/// for interprocess communication and general purpose serialization.
	/// <para>
	/// The <code>XMLEncoder</code> class provides a default denotation for
	/// <em>JavaBean</em>s in which they are represented as XML documents
	/// complying with version 1.0 of the XML specification and the
	/// UTF-8 character encoding of the Unicode/ISO 10646 character set.
	/// The XML documents produced by the <code>XMLEncoder</code> class are:
	/// <ul>
	/// <li>
	/// <em>Portable and version resilient</em>: they have no dependencies
	/// on the private implementation of any class and so, like Java source
	/// files, they may be exchanged between environments which may have
	/// different versions of some of the classes and between VMs from
	/// different vendors.
	/// <li>
	/// <em>Structurally compact</em>: The <code>XMLEncoder</code> class
	/// uses a <em>redundancy elimination</em> algorithm internally so that the
	/// default values of a Bean's properties are not written to the stream.
	/// <li>
	/// <em>Fault tolerant</em>: Non-structural errors in the file,
	/// caused either by damage to the file or by API changes
	/// made to classes in an archive remain localized
	/// so that a reader can report the error and continue to load the parts
	/// of the document which were not affected by the error.
	/// </ul>
	/// </para>
	/// <para>
	/// Below is an example of an XML archive containing
	/// some user interface components from the <em>swing</em> toolkit:
	/// <pre>
	/// &lt;?xml version="1.0" encoding="UTF-8"?&gt;
	/// &lt;java version="1.0" class="java.beans.XMLDecoder"&gt;
	/// &lt;object class="javax.swing.JFrame"&gt;
	///   &lt;void property="name"&gt;
	///     &lt;string&gt;frame1&lt;/string&gt;
	///   &lt;/void&gt;
	///   &lt;void property="bounds"&gt;
	///     &lt;object class="java.awt.Rectangle"&gt;
	///       &lt;int&gt;0&lt;/int&gt;
	///       &lt;int&gt;0&lt;/int&gt;
	///       &lt;int&gt;200&lt;/int&gt;
	///       &lt;int&gt;200&lt;/int&gt;
	///     &lt;/object&gt;
	///   &lt;/void&gt;
	///   &lt;void property="contentPane"&gt;
	///     &lt;void method="add"&gt;
	///       &lt;object class="javax.swing.JButton"&gt;
	///         &lt;void property="label"&gt;
	///           &lt;string&gt;Hello&lt;/string&gt;
	///         &lt;/void&gt;
	///       &lt;/object&gt;
	///     &lt;/void&gt;
	///   &lt;/void&gt;
	///   &lt;void property="visible"&gt;
	///     &lt;boolean&gt;true&lt;/boolean&gt;
	///   &lt;/void&gt;
	/// &lt;/object&gt;
	/// &lt;/java&gt;
	/// </pre>
	/// The XML syntax uses the following conventions:
	/// <ul>
	/// <li>
	/// Each element represents a method call.
	/// <li>
	/// The "object" tag denotes an <em>expression</em> whose value is
	/// to be used as the argument to the enclosing element.
	/// <li>
	/// The "void" tag denotes a <em>statement</em> which will
	/// be executed, but whose result will not be used as an
	/// argument to the enclosing method.
	/// <li>
	/// Elements which contain elements use those elements as arguments,
	/// unless they have the tag: "void".
	/// <li>
	/// The name of the method is denoted by the "method" attribute.
	/// <li>
	/// XML's standard "id" and "idref" attributes are used to make
	/// references to previous expressions - so as to deal with
	/// circularities in the object graph.
	/// <li>
	/// The "class" attribute is used to specify the target of a static
	/// method or constructor explicitly; its value being the fully
	/// qualified name of the class.
	/// <li>
	/// Elements with the "void" tag are executed using
	/// the outer context as the target if no target is defined
	/// by a "class" attribute.
	/// <li>
	/// Java's String class is treated specially and is
	/// written &lt;string&gt;Hello, world&lt;/string&gt; where
	/// the characters of the string are converted to bytes
	/// using the UTF-8 character encoding.
	/// </ul>
	/// </para>
	/// <para>
	/// Although all object graphs may be written using just these three
	/// tags, the following definitions are included so that common
	/// data structures can be expressed more concisely:
	/// </para>
	/// <para>
	/// <ul>
	/// <li>
	/// The default method name is "new".
	/// <li>
	/// A reference to a java class is written in the form
	///  &lt;class&gt;javax.swing.JButton&lt;/class&gt;.
	/// <li>
	/// Instances of the wrapper classes for Java's primitive types are written
	/// using the name of the primitive type as the tag. For example, an
	/// instance of the <code>Integer</code> class could be written:
	/// &lt;int&gt;123&lt;/int&gt;. Note that the <code>XMLEncoder</code> class
	/// uses Java's reflection package in which the conversion between
	/// Java's primitive types and their associated "wrapper classes"
	/// is handled internally. The API for the <code>XMLEncoder</code> class
	/// itself deals only with <code>Object</code>s.
	/// <li>
	/// In an element representing a nullary method whose name
	/// starts with "get", the "method" attribute is replaced
	/// with a "property" attribute whose value is given by removing
	/// the "get" prefix and decapitalizing the result.
	/// <li>
	/// In an element representing a monadic method whose name
	/// starts with "set", the "method" attribute is replaced
	/// with a "property" attribute whose value is given by removing
	/// the "set" prefix and decapitalizing the result.
	/// <li>
	/// In an element representing a method named "get" taking one
	/// integer argument, the "method" attribute is replaced
	/// with an "index" attribute whose value the value of the
	/// first argument.
	/// <li>
	/// In an element representing a method named "set" taking two arguments,
	/// the first of which is an integer, the "method" attribute is replaced
	/// with an "index" attribute whose value the value of the
	/// first argument.
	/// <li>
	/// A reference to an array is written using the "array"
	/// tag. The "class" and "length" attributes specify the
	/// sub-type of the array and its length respectively.
	/// </ul>
	/// 
	/// </para>
	/// <para>
	/// For more information you might also want to check out
	/// <a
	/// href="http://java.sun.com/products/jfc/tsc/articles/persistence4">Using XMLEncoder</a>,
	/// an article in <em>The Swing Connection.</em>
	/// </para>
	/// </summary>
	/// <seealso cref= XMLDecoder </seealso>
	/// <seealso cref= java.io.ObjectOutputStream
	/// 
	/// @since 1.4
	/// 
	/// @author Philip Milne </seealso>
	public class XMLEncoder : Encoder, AutoCloseable
	{

		private readonly CharsetEncoder Encoder;
		private readonly String Charset;
		private readonly bool Declaration;

		private OutputStreamWriter @out;
		private Object Owner_Renamed;
		private int Indentation = 0;
		private bool @internal = false;
		private Map<Object, ValueData> ValueToExpression;
		private Map<Object, List<Statement>> TargetToStatementList;
		private bool PreambleWritten = false;
		private NameGenerator NameGenerator;

		private class ValueData
		{
			private readonly XMLEncoder OuterInstance;

			public ValueData(XMLEncoder outerInstance)
			{
				this.OuterInstance = outerInstance;
			}

			public int Refs = 0;
			public bool Marked = false; // Marked -> refs > 0 unless ref was a target.
			public String Name = null;
			public Expression Exp = null;
		}

		/// <summary>
		/// Creates a new XML encoder to write out <em>JavaBeans</em>
		/// to the stream <code>out</code> using an XML encoding.
		/// </summary>
		/// <param name="out">  the stream to which the XML representation of
		///             the objects will be written
		/// </param>
		/// <exception cref="IllegalArgumentException">
		///          if <code>out</code> is <code>null</code>
		/// </exception>
		/// <seealso cref= XMLDecoder#XMLDecoder(InputStream) </seealso>
		public XMLEncoder(OutputStream @out) : this(@out, "UTF-8", true, 0)
		{
		}

		/// <summary>
		/// Creates a new XML encoder to write out <em>JavaBeans</em>
		/// to the stream <code>out</code> using the given <code>charset</code>
		/// starting from the given <code>indentation</code>.
		/// </summary>
		/// <param name="out">          the stream to which the XML representation of
		///                     the objects will be written </param>
		/// <param name="charset">      the name of the requested charset;
		///                     may be either a canonical name or an alias </param>
		/// <param name="declaration">  whether the XML declaration should be generated;
		///                     set this to <code>false</code>
		///                     when embedding the contents in another XML document </param>
		/// <param name="indentation">  the number of space characters to indent the entire XML document by
		/// </param>
		/// <exception cref="IllegalArgumentException">
		///          if <code>out</code> or <code>charset</code> is <code>null</code>,
		///          or if <code>indentation</code> is less than 0
		/// </exception>
		/// <exception cref="IllegalCharsetNameException">
		///          if <code>charset</code> name is illegal
		/// </exception>
		/// <exception cref="UnsupportedCharsetException">
		///          if no support for the named charset is available
		///          in this instance of the Java virtual machine
		/// </exception>
		/// <exception cref="UnsupportedOperationException">
		///          if loaded charset does not support encoding
		/// </exception>
		/// <seealso cref= Charset#forName(String)
		/// 
		/// @since 1.7 </seealso>
		public XMLEncoder(OutputStream @out, String charset, bool declaration, int indentation)
		{
			if (@out == null)
			{
				throw new IllegalArgumentException("the output stream cannot be null");
			}
			if (indentation < 0)
			{
				throw new IllegalArgumentException("the indentation must be >= 0");
			}
			Charset cs = Charset.ForName(charset);
			this.Encoder = cs.NewEncoder();
			this.Charset = charset;
			this.Declaration = declaration;
			this.Indentation = indentation;
			this.@out = new OutputStreamWriter(@out, cs.NewEncoder());
			ValueToExpression = new IdentityHashMap<>();
			TargetToStatementList = new IdentityHashMap<>();
			NameGenerator = new NameGenerator();
		}

		/// <summary>
		/// Sets the owner of this encoder to <code>owner</code>.
		/// </summary>
		/// <param name="owner"> The owner of this encoder.
		/// </param>
		/// <seealso cref= #getOwner </seealso>
		public virtual Object Owner
		{
			set
			{
				this.Owner_Renamed = value;
				WriteExpression(new Expression(this, "getOwner", new Object[0]));
			}
			get
			{
				return Owner_Renamed;
			}
		}


		/// <summary>
		/// Write an XML representation of the specified object to the output.
		/// </summary>
		/// <param name="o"> The object to be written to the stream.
		/// </param>
		/// <seealso cref= XMLDecoder#readObject </seealso>
		public override void WriteObject(Object o)
		{
			if (@internal)
			{
				base.WriteObject(o);
			}
			else
			{
				WriteStatement(new Statement(this, "writeObject", new Object[]{o}));
			}
		}

		private List<Statement> StatementList(Object target)
		{
			List<Statement> list = TargetToStatementList.Get(target);
			if (list == null)
			{
				list = new List<>();
				TargetToStatementList.Put(target, list);
			}
			return list;
		}


		private void Mark(Object o, bool isArgument)
		{
			if (o == null || o == this)
			{
				return;
			}
			ValueData d = GetValueData(o);
			Expression exp = d.Exp;
			// Do not mark liternal strings. Other strings, which might,
			// for example, come from resource bundles should still be marked.
			if (o.GetType() == typeof(String) && exp == null)
			{
				return;
			}

			// Bump the reference counts of all arguments
			if (isArgument)
			{
				d.Refs++;
			}
			if (d.Marked)
			{
				return;
			}
			d.Marked = true;
			Object target = exp.Target;
			Mark(exp);
			if (!(target is Class))
			{
				StatementList(target).Add(exp);
				// Pending: Why does the reference count need to
				// be incremented here?
				d.Refs++;
			}
		}

		private void Mark(Statement stm)
		{
			Object[] args = stm.Arguments;
			for (int i = 0; i < args.Length; i++)
			{
				Object arg = args[i];
				Mark(arg, true);
			}
			Mark(stm.Target, stm is Expression);
		}


		/// <summary>
		/// Records the Statement so that the Encoder will
		/// produce the actual output when the stream is flushed.
		/// <P>
		/// This method should only be invoked within the context
		/// of initializing a persistence delegate.
		/// </summary>
		/// <param name="oldStm"> The statement that will be written
		///               to the stream. </param>
		/// <seealso cref= java.beans.PersistenceDelegate#initialize </seealso>
		public override void WriteStatement(Statement oldStm)
		{
			// System.out.println("XMLEncoder::writeStatement: " + oldStm);
			bool @internal = this.@internal;
			this.@internal = true;
			try
			{
				base.WriteStatement(oldStm);
				/*
				   Note we must do the mark first as we may
				   require the results of previous values in
				   this context for this statement.
				   Test case is:
				       os.setOwner(this);
				       os.writeObject(this);
				*/
				Mark(oldStm);
				Object target = oldStm.Target;
				if (target is Field)
				{
					String method = oldStm.MethodName;
					Object[] args = oldStm.Arguments;
					if ((method == null) || (args == null))
					{
					}
					else if (method.Equals("get") && (args.Length == 1))
					{
						target = args[0];
					}
					else if (method.Equals("set") && (args.Length == 2))
					{
						target = args[0];
					}
				}
				StatementList(target).Add(oldStm);
			}
			catch (Exception e)
			{
				ExceptionListener.ExceptionThrown(new Exception("XMLEncoder: discarding statement " + oldStm, e));
			}
			this.@internal = @internal;
		}


		/// <summary>
		/// Records the Expression so that the Encoder will
		/// produce the actual output when the stream is flushed.
		/// <P>
		/// This method should only be invoked within the context of
		/// initializing a persistence delegate or setting up an encoder to
		/// read from a resource bundle.
		/// <P>
		/// For more information about using resource bundles with the
		/// XMLEncoder, see
		/// http://java.sun.com/products/jfc/tsc/articles/persistence4/#i18n
		/// </summary>
		/// <param name="oldExp"> The expression that will be written
		///               to the stream. </param>
		/// <seealso cref= java.beans.PersistenceDelegate#initialize </seealso>
		public override void WriteExpression(Expression oldExp)
		{
			bool @internal = this.@internal;
			this.@internal = true;
			Object oldValue = GetValue(oldExp);
			if (Get(oldValue) == null || (oldValue is String && !@internal))
			{
				GetValueData(oldValue).Exp = oldExp;
				base.WriteExpression(oldExp);
			}
			this.@internal = @internal;
		}

		/// <summary>
		/// This method writes out the preamble associated with the
		/// XML encoding if it has not been written already and
		/// then writes out all of the values that been
		/// written to the stream since the last time <code>flush</code>
		/// was called. After flushing, all internal references to the
		/// values that were written to this stream are cleared.
		/// </summary>
		public virtual void Flush()
		{
			if (!PreambleWritten) // Don't do this in constructor - it throws ... pending.
			{
				if (this.Declaration)
				{
					Writeln("<?xml version=" + Quote("1.0") + " encoding=" + Quote(this.Charset) + "?>");
				}
				Writeln("<java version=" + Quote(System.getProperty("java.version")) + " class=" + Quote(typeof(XMLDecoder).Name) + ">");
				PreambleWritten = true;
			}
			Indentation++;
			List<Statement> statements = StatementList(this);
			while (statements.Count > 0)
			{
				Statement s = statements.Remove(0);
				if ("writeObject".Equals(s.MethodName))
				{
					OutputValue(s.Arguments[0], this, true);
				}
				else
				{
					OutputStatement(s, this, false);
				}
			}
			Indentation--;

			Statement statement = MissedStatement;
			while (statement != null)
			{
				OutputStatement(statement, this, false);
				statement = MissedStatement;
			}

			try
			{
				@out.Flush();
			}
			catch (IOException e)
			{
				ExceptionListener.ExceptionThrown(e);
			}
			Clear();
		}

		internal override void Clear()
		{
			base.Clear();
			NameGenerator.Clear();
			ValueToExpression.Clear();
			TargetToStatementList.Clear();
		}

		internal virtual Statement MissedStatement
		{
			get
			{
				foreach (List<Statement> statements in this.TargetToStatementList.Values())
				{
					for (int i = 0; i < statements.Count; i++)
					{
						if (typeof(Statement) == statements.Get(i).GetType())
						{
							return statements.Remove(i);
						}
					}
				}
				return null;
			}
		}


		/// <summary>
		/// This method calls <code>flush</code>, writes the closing
		/// postamble and then closes the output stream associated
		/// with this stream.
		/// </summary>
		public virtual void Close()
		{
			Flush();
			Writeln("</java>");
			try
			{
				@out.Close();
			}
			catch (IOException e)
			{
				ExceptionListener.ExceptionThrown(e);
			}
		}

		private String Quote(String s)
		{
			return "\"" + s + "\"";
		}

		private ValueData GetValueData(Object o)
		{
			ValueData d = ValueToExpression.Get(o);
			if (d == null)
			{
				d = new ValueData(this);
				ValueToExpression.Put(o, d);
			}
			return d;
		}

		/// <summary>
		/// Returns <code>true</code> if the argument,
		/// a Unicode code point, is valid in XML documents.
		/// Unicode characters fit into the low sixteen bits of a Unicode code point,
		/// and pairs of Unicode <em>surrogate characters</em> can be combined
		/// to encode Unicode code point in documents containing only Unicode.
		/// (The <code>char</code> datatype in the Java Programming Language
		/// represents Unicode characters, including unpaired surrogates.)
		/// <par>
		/// [2] Char ::= #x0009 | #x000A | #x000D
		///            | [#x0020-#xD7FF]
		///            | [#xE000-#xFFFD]
		///            | [#x10000-#x10ffff]
		/// </par>
		/// </summary>
		/// <param name="code">  the 32-bit Unicode code point being tested </param>
		/// <returns>  <code>true</code> if the Unicode code point is valid,
		///          <code>false</code> otherwise </returns>
		private static bool IsValidCharCode(int code)
		{
			return (0x0020 <= code && code <= 0xD7FF) || (0x000A == code) || (0x0009 == code) || (0x000D == code) || (0xE000 <= code && code <= 0xFFFD) || (0x10000 <= code && code <= 0x10ffff);
		}

		private void Writeln(String exp)
		{
			try
			{
				StringBuilder sb = new StringBuilder();
				for (int i = 0; i < Indentation; i++)
				{
					sb.Append(' ');
				}
				sb.Append(exp);
				sb.Append('\n');
				this.@out.Write(sb.ToString());
			}
			catch (IOException e)
			{
				ExceptionListener.ExceptionThrown(e);
			}
		}

		private void OutputValue(Object value, Object outer, bool isArgument)
		{
			if (value == null)
			{
				Writeln("<null/>");
				return;
			}

			if (value is Class)
			{
				Writeln("<class>" + ((Class)value).Name + "</class>");
				return;
			}

			ValueData d = GetValueData(value);
			if (d.Exp != null)
			{
				Object target = d.Exp.Target;
				String methodName = d.Exp.MethodName;

				if (target == null || methodName == null)
				{
					throw new NullPointerException((target == null ? "target" : "methodName") + " should not be null");
				}

				if (isArgument && target is Field && methodName.Equals("get"))
				{
					Field f = (Field)target;
					Writeln("<object class=" + Quote(f.DeclaringClass.Name) + " field=" + Quote(f.Name) + "/>");
					return;
				}

				Class primitiveType = PrimitiveTypeFor(value.GetType());
				if (primitiveType != null && target == value.GetType() && methodName.Equals("new"))
				{
					String primitiveTypeName = primitiveType.Name;
					// Make sure that character types are quoted correctly.
					if (primitiveType == Character.TYPE)
					{
						char code = ((Character) value).CharValue();
						if (!IsValidCharCode(code))
						{
							Writeln(CreateString(code));
							return;
						}
						value = QuoteCharCode(code);
						if (value == null)
						{
							value = Convert.ToChar(code);
						}
					}
					Writeln("<" + primitiveTypeName + ">" + value + "</" + primitiveTypeName + ">");
					return;
				}

			}
			else if (value is String)
			{
				Writeln(CreateString((String) value));
				return;
			}

			if (d.Name != null)
			{
				if (isArgument)
				{
					Writeln("<object idref=" + Quote(d.Name) + "/>");
				}
				else
				{
					outputXML("void", " idref=" + Quote(d.Name), value);
				}
			}
			else if (d.Exp != null)
			{
				OutputStatement(d.Exp, outer, isArgument);
			}
		}

		private static String QuoteCharCode(int code)
		{
			switch (code)
			{
			  case '&':
				  return "&amp;";
			  case '<':
				  return "&lt;";
			  case '>':
				  return "&gt;";
			  case '"':
				  return "&quot;";
			  case '\'':
				  return "&apos;";
			  case '\r':
				  return "&#13;";
			  default:
				  return null;
			}
		}

		private static String CreateString(int code)
		{
			return "<char code=\"#" + Convert.ToString(code, 16) + "\"/>";
		}

		private String CreateString(String @string)
		{
			StringBuilder sb = new StringBuilder();
			sb.Append("<string>");
			int index = 0;
			while (index < @string.Length())
			{
				int point = @string.CodePointAt(index);
				int count = Character.CharCount(point);

				if (IsValidCharCode(point) && this.Encoder.CanEncode(@string.Substring(index, count)))
				{
					String value = QuoteCharCode(point);
					if (value != null)
					{
						sb.Append(value);
					}
					else
					{
						sb.AppendCodePoint(point);
					}
					index += count;
				}
				else
				{
					sb.Append(CreateString(@string.CharAt(index)));
					index++;
				}
			}
			sb.Append("</string>");
			return sb.ToString();
		}

		private void OutputStatement(Statement exp, Object outer, bool isArgument)
		{
			Object target = exp.Target;
			String methodName = exp.MethodName;

			if (target == null || methodName == null)
			{
				throw new NullPointerException((target == null ? "target" : "methodName") + " should not be null");
			}

			Object[] args = exp.Arguments;
			bool expression = exp.GetType() == typeof(Expression);
			Object value = (expression) ? GetValue((Expression)exp) : null;

			String tag = (expression && isArgument) ? "object" : "void";
			String attributes = "";
			ValueData d = GetValueData(value);

			// Special cases for targets.
			if (target == outer)
			{
			}
			else if (target == typeof(Array) && methodName.Equals("newInstance"))
			{
				tag = "array";
				attributes = attributes + " class=" + Quote(((Class)args[0]).Name);
				attributes = attributes + " length=" + Quote(args[1].ToString());
				args = new Object[]{};
			}
			else if (target.GetType() == typeof(Class))
			{
				attributes = attributes + " class=" + Quote(((Class)target).Name);
			}
			else
			{
				d.Refs = 2;
				if (d.Name == null)
				{
					GetValueData(target).Refs++;
					List<Statement> statements = StatementList(target);
					if (!statements.Contains(exp))
					{
						statements.Add(exp);
					}
					OutputValue(target, outer, false);
				}
				if (expression)
				{
					OutputValue(value, outer, isArgument);
				}
				return;
			}
			if (expression && (d.Refs > 1))
			{
				String instanceName = NameGenerator.InstanceName(value);
				d.Name = instanceName;
				attributes = attributes + " id=" + Quote(instanceName);
			}

			// Special cases for methods.
			if ((!expression && methodName.Equals("set") && args.Length == 2 && args[0] is Integer) || (expression && methodName.Equals("get") && args.Length == 1 && args[0] is Integer))
			{
				attributes = attributes + " index=" + Quote(args[0].ToString());
				args = (args.Length == 1) ? new Object[]{} : new Object[]{args[1]};
			}
			else if ((!expression && methodName.StartsWith("set") && args.Length == 1) || (expression && methodName.StartsWith("get") && args.Length == 0))
			{
				if (3 < methodName.Length())
				{
					attributes = attributes + " property=" + Quote(Introspector.Decapitalize(methodName.Substring(3)));
				}
			}
			else if (!methodName.Equals("new") && !methodName.Equals("newInstance"))
			{
				attributes = attributes + " method=" + Quote(methodName);
			}
			OutputXML(tag, attributes, value, args);
		}

		private void OutputXML(String tag, String attributes, Object value, params Object[] args)
		{
			List<Statement> statements = StatementList(value);
			// Use XML's short form when there is no body.
			if (args.Length == 0 && statements.Count == 0)
			{
				Writeln("<" + tag + attributes + "/>");
				return;
			}

			Writeln("<" + tag + attributes + ">");
			Indentation++;

			for (int i = 0; i < args.Length; i++)
			{
				OutputValue(args[i], null, true);
			}

			while (statements.Count > 0)
			{
				Statement s = statements.Remove(0);
				OutputStatement(s, value, false);
			}

			Indentation--;
			Writeln("</" + tag + ">");
		}

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @SuppressWarnings("rawtypes") static Class primitiveTypeFor(Class wrapper)
		internal static Class PrimitiveTypeFor(Class wrapper)
		{
			if (wrapper == typeof(Boolean))
			{
				return Boolean.TYPE;
			}
			if (wrapper == typeof(Byte))
			{
				return Byte.TYPE;
			}
			if (wrapper == typeof(Character))
			{
				return Character.TYPE;
			}
			if (wrapper == typeof(Short))
			{
				return Short.TYPE;
			}
			if (wrapper == typeof(Integer))
			{
				return Integer.TYPE;
			}
			if (wrapper == typeof(Long))
			{
				return Long.TYPE;
			}
			if (wrapper == typeof(Float))
			{
				return Float.TYPE;
			}
			if (wrapper == typeof(Double))
			{
				return Double.TYPE;
			}
			if (wrapper == typeof(Void))
			{
				return Void.TYPE;
			}
			return null;
		}
	}

}
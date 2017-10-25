using System;
using System.Collections;
using System.Collections.Generic;

/*
 * Copyright (c) 1998, 2004, Oracle and/or its affiliates. All rights reserved.
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

namespace java.awt.image.renderable
{

	/// <summary>
	/// A <code>ParameterBlock</code> encapsulates all the information about sources and
	/// parameters (Objects) required by a RenderableImageOp, or other
	/// classes that process images.
	/// 
	/// <para> Although it is possible to place arbitrary objects in the
	/// source Vector, users of this class may impose semantic constraints
	/// such as requiring all sources to be RenderedImages or
	/// RenderableImage.  <code>ParameterBlock</code> itself is merely a container and
	/// performs no checking on source or parameter types.
	/// 
	/// </para>
	/// <para> All parameters in a <code>ParameterBlock</code> are objects; convenience
	/// add and set methods are available that take arguments of base type and
	/// construct the appropriate subclass of Number (such as
	/// Integer or Float).  Corresponding get methods perform a
	/// downward cast and have return values of base type; an exception
	/// will be thrown if the stored values do not have the correct type.
	/// There is no way to distinguish between the results of
	/// "short s; add(s)" and "add(new Short(s))".
	/// 
	/// </para>
	/// <para> Note that the get and set methods operate on references.
	/// Therefore, one must be careful not to share references between
	/// <code>ParameterBlock</code>s when this is inappropriate.  For example, to create
	/// a new <code>ParameterBlock</code> that is equal to an old one except for an
	/// added source, one might be tempted to write:
	/// 
	/// <pre>
	/// ParameterBlock addSource(ParameterBlock pb, RenderableImage im) {
	///     ParameterBlock pb1 = new ParameterBlock(pb.getSources());
	///     pb1.addSource(im);
	///     return pb1;
	/// }
	/// </pre>
	/// 
	/// </para>
	/// <para> This code will have the side effect of altering the original
	/// <code>ParameterBlock</code>, since the getSources operation returned a reference
	/// to its source Vector.  Both pb and pb1 share their source Vector,
	/// and a change in either is visible to both.
	/// 
	/// </para>
	/// <para> A correct way to write the addSource function is to clone
	/// the source Vector:
	/// 
	/// <pre>
	/// ParameterBlock addSource (ParameterBlock pb, RenderableImage im) {
	///     ParameterBlock pb1 = new ParameterBlock(pb.getSources().clone());
	///     pb1.addSource(im);
	///     return pb1;
	/// }
	/// </pre>
	/// 
	/// </para>
	/// <para> The clone method of <code>ParameterBlock</code> has been defined to
	/// perform a clone of both the source and parameter Vectors for
	/// this reason.  A standard, shallow clone is available as
	/// shallowClone.
	/// 
	/// </para>
	/// <para> The addSource, setSource, add, and set methods are
	/// defined to return 'this' after adding their argument.  This allows
	/// use of syntax like:
	/// 
	/// <pre>
	/// ParameterBlock pb = new ParameterBlock();
	/// op = new RenderableImageOp("operation", pb.add(arg1).add(arg2));
	/// </pre>
	/// 
	/// </para>
	/// </summary>
	[Serializable]
	public class ParameterBlock : Cloneable
	{
		/// <summary>
		/// A Vector of sources, stored as arbitrary Objects. </summary>
		protected internal List<Object> Sources_Renamed = new List<Object>();

		/// <summary>
		/// A Vector of non-source parameters, stored as arbitrary Objects. </summary>
		protected internal List<Object> Parameters_Renamed = new List<Object>();

		/// <summary>
		/// A dummy constructor. </summary>
		public ParameterBlock()
		{
		}

		/// <summary>
		/// Constructs a <code>ParameterBlock</code> with a given Vector
		/// of sources. </summary>
		/// <param name="sources"> a <code>Vector</code> of source images </param>
		public ParameterBlock(List<Object> sources)
		{
			Sources = sources;
		}

		/// <summary>
		/// Constructs a <code>ParameterBlock</code> with a given Vector of sources and
		/// Vector of parameters. </summary>
		/// <param name="sources"> a <code>Vector</code> of source images </param>
		/// <param name="parameters"> a <code>Vector</code> of parameters to be used in the
		///        rendering operation </param>
		public ParameterBlock(List<Object> sources, List<Object> parameters)
		{
			Sources = sources;
			Parameters = parameters;
		}

		/// <summary>
		/// Creates a shallow copy of a <code>ParameterBlock</code>.  The source and
		/// parameter Vectors are copied by reference -- additions or
		/// changes will be visible to both versions.
		/// </summary>
		/// <returns> an Object clone of the <code>ParameterBlock</code>. </returns>
		public virtual Object ShallowClone()
		{
			try
			{
				return base.Clone();
			}
			catch (Exception)
			{
				// We can't be here since we implement Cloneable.
				return null;
			}
		}

		/// <summary>
		/// Creates a copy of a <code>ParameterBlock</code>.  The source and parameter
		/// Vectors are cloned, but the actual sources and parameters are
		/// copied by reference.  This allows modifications to the order
		/// and number of sources and parameters in the clone to be invisible
		/// to the original <code>ParameterBlock</code>.  Changes to the shared sources or
		/// parameters themselves will still be visible.
		/// </summary>
		/// <returns> an Object clone of the <code>ParameterBlock</code>. </returns>
		public virtual Object Clone()
		{
			ParameterBlock theClone;

			try
			{
				theClone = (ParameterBlock) base.Clone();
			}
			catch (Exception)
			{
				// We can't be here since we implement Cloneable.
				return null;
			}

			if (Sources_Renamed != null)
			{
				theClone.Sources = (ArrayList)Sources_Renamed.clone();
			}
			if (Parameters_Renamed != null)
			{
				theClone.Parameters = (ArrayList)Parameters_Renamed.clone();
			}
			return (Object) theClone;
		}

		/// <summary>
		/// Adds an image to end of the list of sources.  The image is
		/// stored as an object in order to allow new node types in the
		/// future.
		/// </summary>
		/// <param name="source"> an image object to be stored in the source list. </param>
		/// <returns> a new <code>ParameterBlock</code> containing the specified
		///         <code>source</code>. </returns>
		public virtual ParameterBlock AddSource(Object source)
		{
			Sources_Renamed.Add(source);
			return this;
		}

		/// <summary>
		/// Returns a source as a general Object.  The caller must cast it into
		/// an appropriate type.
		/// </summary>
		/// <param name="index"> the index of the source to be returned. </param>
		/// <returns> an <code>Object</code> that represents the source located
		///         at the specified index in the <code>sources</code>
		///         <code>Vector</code>. </returns>
		/// <seealso cref= #setSource(Object, int) </seealso>
		public virtual Object GetSource(int index)
		{
			return Sources_Renamed[index];
		}

		/// <summary>
		/// Replaces an entry in the list of source with a new source.
		/// If the index lies beyond the current source list,
		/// the list is extended with nulls as needed. </summary>
		/// <param name="source"> the specified source image </param>
		/// <param name="index"> the index into the <code>sources</code>
		///              <code>Vector</code> at which to
		///              insert the specified <code>source</code> </param>
		/// <returns> a new <code>ParameterBlock</code> that contains the
		///         specified <code>source</code> at the specified
		///         <code>index</code>. </returns>
		/// <seealso cref= #getSource(int) </seealso>
		public virtual ParameterBlock SetSource(Object source, int index)
		{
			int oldSize = Sources_Renamed.Count;
			int newSize = index + 1;
			if (oldSize < newSize)
			{
				Sources_Renamed.Capacity = newSize;
			}
			Sources_Renamed[index] = source;
			return this;
		}

		/// <summary>
		/// Returns a source as a <code>RenderedImage</code>.  This method is
		/// a convenience method.
		/// An exception will be thrown if the source is not a RenderedImage.
		/// </summary>
		/// <param name="index"> the index of the source to be returned </param>
		/// <returns> a <code>RenderedImage</code> that represents the source
		///         image that is at the specified index in the
		///         <code>sources</code> <code>Vector</code>. </returns>
		public virtual RenderedImage GetRenderedSource(int index)
		{
			return (RenderedImage) Sources_Renamed[index];
		}

		/// <summary>
		/// Returns a source as a RenderableImage.  This method is a
		/// convenience method.
		/// An exception will be thrown if the sources is not a RenderableImage.
		/// </summary>
		/// <param name="index"> the index of the source to be returned </param>
		/// <returns> a <code>RenderableImage</code> that represents the source
		///         image that is at the specified index in the
		///         <code>sources</code> <code>Vector</code>. </returns>
		public virtual RenderableImage GetRenderableSource(int index)
		{
			return (RenderableImage) Sources_Renamed[index];
		}

		/// <summary>
		/// Returns the number of source images. </summary>
		/// <returns> the number of source images in the <code>sources</code>
		///         <code>Vector</code>. </returns>
		public virtual int NumSources
		{
			get
			{
				return Sources_Renamed.Count;
			}
		}

		/// <summary>
		/// Returns the entire Vector of sources. </summary>
		/// <returns> the <code>sources</code> <code>Vector</code>. </returns>
		/// <seealso cref= #setSources(Vector) </seealso>
		public virtual List<Object> Sources
		{
			get
			{
				return Sources_Renamed;
			}
			set
			{
				this.Sources_Renamed = value;
			}
		}


		/// <summary>
		/// Clears the list of source images. </summary>
		public virtual void RemoveSources()
		{
			Sources_Renamed = new ArrayList();
		}

		/// <summary>
		/// Returns the number of parameters (not including source images). </summary>
		/// <returns> the number of parameters in the <code>parameters</code>
		///         <code>Vector</code>. </returns>
		public virtual int NumParameters
		{
			get
			{
				return Parameters_Renamed.Count;
			}
		}

		/// <summary>
		/// Returns the entire Vector of parameters. </summary>
		/// <returns> the <code>parameters</code> <code>Vector</code>. </returns>
		/// <seealso cref= #setParameters(Vector) </seealso>
		public virtual List<Object> Parameters
		{
			get
			{
				return Parameters_Renamed;
			}
			set
			{
				this.Parameters_Renamed = value;
			}
		}


		/// <summary>
		/// Clears the list of parameters. </summary>
		public virtual void RemoveParameters()
		{
			Parameters_Renamed = new ArrayList();
		}

		/// <summary>
		/// Adds an object to the list of parameters. </summary>
		/// <param name="obj"> the <code>Object</code> to add to the
		///            <code>parameters</code> <code>Vector</code> </param>
		/// <returns> a new <code>ParameterBlock</code> containing
		///         the specified parameter. </returns>
		public virtual ParameterBlock Add(Object obj)
		{
			Parameters_Renamed.Add(obj);
			return this;
		}

		/// <summary>
		/// Adds a Byte to the list of parameters. </summary>
		/// <param name="b"> the byte to add to the
		///            <code>parameters</code> <code>Vector</code> </param>
		/// <returns> a new <code>ParameterBlock</code> containing
		///         the specified parameter. </returns>
		public virtual ParameterBlock Add(sbyte b)
		{
			return Add(new Byte(b));
		}

		/// <summary>
		/// Adds a Character to the list of parameters. </summary>
		/// <param name="c"> the char to add to the
		///            <code>parameters</code> <code>Vector</code> </param>
		/// <returns> a new <code>ParameterBlock</code> containing
		///         the specified parameter. </returns>
		public virtual ParameterBlock Add(char c)
		{
			return Add(new Character(c));
		}

		/// <summary>
		/// Adds a Short to the list of parameters. </summary>
		/// <param name="s"> the short to add to the
		///            <code>parameters</code> <code>Vector</code> </param>
		/// <returns> a new <code>ParameterBlock</code> containing
		///         the specified parameter. </returns>
		public virtual ParameterBlock Add(short s)
		{
			return Add(new Short(s));
		}

		/// <summary>
		/// Adds a Integer to the list of parameters. </summary>
		/// <param name="i"> the int to add to the
		///            <code>parameters</code> <code>Vector</code> </param>
		/// <returns> a new <code>ParameterBlock</code> containing
		///         the specified parameter. </returns>
		public virtual ParameterBlock Add(int i)
		{
			return Add(new Integer(i));
		}

		/// <summary>
		/// Adds a Long to the list of parameters. </summary>
		/// <param name="l"> the long to add to the
		///            <code>parameters</code> <code>Vector</code> </param>
		/// <returns> a new <code>ParameterBlock</code> containing
		///         the specified parameter. </returns>
		public virtual ParameterBlock Add(long l)
		{
			return Add(new Long(l));
		}

		/// <summary>
		/// Adds a Float to the list of parameters. </summary>
		/// <param name="f"> the float to add to the
		///            <code>parameters</code> <code>Vector</code> </param>
		/// <returns> a new <code>ParameterBlock</code> containing
		///         the specified parameter. </returns>
		public virtual ParameterBlock Add(float f)
		{
			return Add(new Float(f));
		}

		/// <summary>
		/// Adds a Double to the list of parameters. </summary>
		/// <param name="d"> the double to add to the
		///            <code>parameters</code> <code>Vector</code> </param>
		/// <returns> a new <code>ParameterBlock</code> containing
		///         the specified parameter. </returns>
		public virtual ParameterBlock Add(double d)
		{
			return Add(new Double(d));
		}

		/// <summary>
		/// Replaces an Object in the list of parameters.
		/// If the index lies beyond the current source list,
		/// the list is extended with nulls as needed. </summary>
		/// <param name="obj"> the parameter that replaces the
		///        parameter at the specified index in the
		///        <code>parameters</code> <code>Vector</code> </param>
		/// <param name="index"> the index of the parameter to be
		///        replaced with the specified parameter </param>
		/// <returns> a new <code>ParameterBlock</code> containing
		///        the specified parameter. </returns>
		public virtual ParameterBlock Set(Object obj, int index)
		{
			int oldSize = Parameters_Renamed.Count;
			int newSize = index + 1;
			if (oldSize < newSize)
			{
				Parameters_Renamed.Capacity = newSize;
			}
			Parameters_Renamed[index] = obj;
			return this;
		}

		/// <summary>
		/// Replaces an Object in the list of parameters with a Byte.
		/// If the index lies beyond the current source list,
		/// the list is extended with nulls as needed. </summary>
		/// <param name="b"> the parameter that replaces the
		///        parameter at the specified index in the
		///        <code>parameters</code> <code>Vector</code> </param>
		/// <param name="index"> the index of the parameter to be
		///        replaced with the specified parameter </param>
		/// <returns> a new <code>ParameterBlock</code> containing
		///        the specified parameter. </returns>
		public virtual ParameterBlock Set(sbyte b, int index)
		{
			return Set(new Byte(b), index);
		}

		/// <summary>
		/// Replaces an Object in the list of parameters with a Character.
		/// If the index lies beyond the current source list,
		/// the list is extended with nulls as needed. </summary>
		/// <param name="c"> the parameter that replaces the
		///        parameter at the specified index in the
		///        <code>parameters</code> <code>Vector</code> </param>
		/// <param name="index"> the index of the parameter to be
		///        replaced with the specified parameter </param>
		/// <returns> a new <code>ParameterBlock</code> containing
		///        the specified parameter. </returns>
		public virtual ParameterBlock Set(char c, int index)
		{
			return Set(new Character(c), index);
		}

		/// <summary>
		/// Replaces an Object in the list of parameters with a Short.
		/// If the index lies beyond the current source list,
		/// the list is extended with nulls as needed. </summary>
		/// <param name="s"> the parameter that replaces the
		///        parameter at the specified index in the
		///        <code>parameters</code> <code>Vector</code> </param>
		/// <param name="index"> the index of the parameter to be
		///        replaced with the specified parameter </param>
		/// <returns> a new <code>ParameterBlock</code> containing
		///        the specified parameter. </returns>
		public virtual ParameterBlock Set(short s, int index)
		{
			return Set(new Short(s), index);
		}

		/// <summary>
		/// Replaces an Object in the list of parameters with an Integer.
		/// If the index lies beyond the current source list,
		/// the list is extended with nulls as needed. </summary>
		/// <param name="i"> the parameter that replaces the
		///        parameter at the specified index in the
		///        <code>parameters</code> <code>Vector</code> </param>
		/// <param name="index"> the index of the parameter to be
		///        replaced with the specified parameter </param>
		/// <returns> a new <code>ParameterBlock</code> containing
		///        the specified parameter. </returns>
		public virtual ParameterBlock Set(int i, int index)
		{
			return Set(new Integer(i), index);
		}

		/// <summary>
		/// Replaces an Object in the list of parameters with a Long.
		/// If the index lies beyond the current source list,
		/// the list is extended with nulls as needed. </summary>
		/// <param name="l"> the parameter that replaces the
		///        parameter at the specified index in the
		///        <code>parameters</code> <code>Vector</code> </param>
		/// <param name="index"> the index of the parameter to be
		///        replaced with the specified parameter </param>
		/// <returns> a new <code>ParameterBlock</code> containing
		///        the specified parameter. </returns>
		public virtual ParameterBlock Set(long l, int index)
		{
			return Set(new Long(l), index);
		}

		/// <summary>
		/// Replaces an Object in the list of parameters with a Float.
		/// If the index lies beyond the current source list,
		/// the list is extended with nulls as needed. </summary>
		/// <param name="f"> the parameter that replaces the
		///        parameter at the specified index in the
		///        <code>parameters</code> <code>Vector</code> </param>
		/// <param name="index"> the index of the parameter to be
		///        replaced with the specified parameter </param>
		/// <returns> a new <code>ParameterBlock</code> containing
		///        the specified parameter. </returns>
		public virtual ParameterBlock Set(float f, int index)
		{
			return Set(new Float(f), index);
		}

		/// <summary>
		/// Replaces an Object in the list of parameters with a Double.
		/// If the index lies beyond the current source list,
		/// the list is extended with nulls as needed. </summary>
		/// <param name="d"> the parameter that replaces the
		///        parameter at the specified index in the
		///        <code>parameters</code> <code>Vector</code> </param>
		/// <param name="index"> the index of the parameter to be
		///        replaced with the specified parameter </param>
		/// <returns> a new <code>ParameterBlock</code> containing
		///        the specified parameter. </returns>
		public virtual ParameterBlock Set(double d, int index)
		{
			return Set(new Double(d), index);
		}

		/// <summary>
		/// Gets a parameter as an object. </summary>
		/// <param name="index"> the index of the parameter to get </param>
		/// <returns> an <code>Object</code> representing the
		///         the parameter at the specified index
		///         into the <code>parameters</code>
		///         <code>Vector</code>. </returns>
		public virtual Object GetObjectParameter(int index)
		{
			return Parameters_Renamed[index];
		}

		/// <summary>
		/// A convenience method to return a parameter as a byte.  An
		/// exception is thrown if the parameter is
		/// <code>null</code> or not a <code>Byte</code>.
		/// </summary>
		/// <param name="index"> the index of the parameter to be returned. </param>
		/// <returns> the parameter at the specified index
		///         as a <code>byte</code> value. </returns>
		/// <exception cref="ClassCastException"> if the parameter at the
		///         specified index is not a <code>Byte</code> </exception>
		/// <exception cref="NullPointerException"> if the parameter at the specified
		///         index is <code>null</code> </exception>
		/// <exception cref="ArrayIndexOutOfBoundsException"> if <code>index</code>
		///         is negative or not less than the current size of this
		///         <code>ParameterBlock</code> object </exception>
		public virtual sbyte GetByteParameter(int index)
		{
			return ((Byte)Parameters_Renamed[index]).ByteValue();
		}

		/// <summary>
		/// A convenience method to return a parameter as a char.  An
		/// exception is thrown if the parameter is
		/// <code>null</code> or not a <code>Character</code>.
		/// </summary>
		/// <param name="index"> the index of the parameter to be returned. </param>
		/// <returns> the parameter at the specified index
		///         as a <code>char</code> value. </returns>
		/// <exception cref="ClassCastException"> if the parameter at the
		///         specified index is not a <code>Character</code> </exception>
		/// <exception cref="NullPointerException"> if the parameter at the specified
		///         index is <code>null</code> </exception>
		/// <exception cref="ArrayIndexOutOfBoundsException"> if <code>index</code>
		///         is negative or not less than the current size of this
		///         <code>ParameterBlock</code> object </exception>
		public virtual char GetCharParameter(int index)
		{
			return ((Character)Parameters_Renamed[index]).CharValue();
		}

		/// <summary>
		/// A convenience method to return a parameter as a short.  An
		/// exception is thrown if the parameter is
		/// <code>null</code> or not a <code>Short</code>.
		/// </summary>
		/// <param name="index"> the index of the parameter to be returned. </param>
		/// <returns> the parameter at the specified index
		///         as a <code>short</code> value. </returns>
		/// <exception cref="ClassCastException"> if the parameter at the
		///         specified index is not a <code>Short</code> </exception>
		/// <exception cref="NullPointerException"> if the parameter at the specified
		///         index is <code>null</code> </exception>
		/// <exception cref="ArrayIndexOutOfBoundsException"> if <code>index</code>
		///         is negative or not less than the current size of this
		///         <code>ParameterBlock</code> object </exception>
		public virtual short GetShortParameter(int index)
		{
			return ((Short)Parameters_Renamed[index]).ShortValue();
		}

		/// <summary>
		/// A convenience method to return a parameter as an int.  An
		/// exception is thrown if the parameter is
		/// <code>null</code> or not an <code>Integer</code>.
		/// </summary>
		/// <param name="index"> the index of the parameter to be returned. </param>
		/// <returns> the parameter at the specified index
		///         as a <code>int</code> value. </returns>
		/// <exception cref="ClassCastException"> if the parameter at the
		///         specified index is not a <code>Integer</code> </exception>
		/// <exception cref="NullPointerException"> if the parameter at the specified
		///         index is <code>null</code> </exception>
		/// <exception cref="ArrayIndexOutOfBoundsException"> if <code>index</code>
		///         is negative or not less than the current size of this
		///         <code>ParameterBlock</code> object </exception>
		public virtual int GetIntParameter(int index)
		{
			return ((Integer)Parameters_Renamed[index]).IntValue();
		}

		/// <summary>
		/// A convenience method to return a parameter as a long.  An
		/// exception is thrown if the parameter is
		/// <code>null</code> or not a <code>Long</code>.
		/// </summary>
		/// <param name="index"> the index of the parameter to be returned. </param>
		/// <returns> the parameter at the specified index
		///         as a <code>long</code> value. </returns>
		/// <exception cref="ClassCastException"> if the parameter at the
		///         specified index is not a <code>Long</code> </exception>
		/// <exception cref="NullPointerException"> if the parameter at the specified
		///         index is <code>null</code> </exception>
		/// <exception cref="ArrayIndexOutOfBoundsException"> if <code>index</code>
		///         is negative or not less than the current size of this
		///         <code>ParameterBlock</code> object </exception>
		public virtual long GetLongParameter(int index)
		{
			return ((Long)Parameters_Renamed[index]).LongValue();
		}

		/// <summary>
		/// A convenience method to return a parameter as a float.  An
		/// exception is thrown if the parameter is
		/// <code>null</code> or not a <code>Float</code>.
		/// </summary>
		/// <param name="index"> the index of the parameter to be returned. </param>
		/// <returns> the parameter at the specified index
		///         as a <code>float</code> value. </returns>
		/// <exception cref="ClassCastException"> if the parameter at the
		///         specified index is not a <code>Float</code> </exception>
		/// <exception cref="NullPointerException"> if the parameter at the specified
		///         index is <code>null</code> </exception>
		/// <exception cref="ArrayIndexOutOfBoundsException"> if <code>index</code>
		///         is negative or not less than the current size of this
		///         <code>ParameterBlock</code> object </exception>
		public virtual float GetFloatParameter(int index)
		{
			return ((Float)Parameters_Renamed[index]).FloatValue();
		}

		/// <summary>
		/// A convenience method to return a parameter as a double.  An
		/// exception is thrown if the parameter is
		/// <code>null</code> or not a <code>Double</code>.
		/// </summary>
		/// <param name="index"> the index of the parameter to be returned. </param>
		/// <returns> the parameter at the specified index
		///         as a <code>double</code> value. </returns>
		/// <exception cref="ClassCastException"> if the parameter at the
		///         specified index is not a <code>Double</code> </exception>
		/// <exception cref="NullPointerException"> if the parameter at the specified
		///         index is <code>null</code> </exception>
		/// <exception cref="ArrayIndexOutOfBoundsException"> if <code>index</code>
		///         is negative or not less than the current size of this
		///         <code>ParameterBlock</code> object </exception>
		public virtual double GetDoubleParameter(int index)
		{
			return ((Double)Parameters_Renamed[index]).DoubleValue();
		}

		/// <summary>
		/// Returns an array of Class objects describing the types
		/// of the parameters. </summary>
		/// <returns> an array of <code>Class</code> objects. </returns>
		public virtual Class [] ParamClasses
		{
			get
			{
				int numParams = NumParameters;
				Class[] classes = new Class[numParams];
				int i;
    
				for (i = 0; i < numParams; i++)
				{
					Object obj = GetObjectParameter(i);
					if (obj is Byte)
					{
					  classes[i] = typeof(sbyte);
					}
					else if (obj is Character)
					{
					  classes[i] = typeof(char);
					}
					else if (obj is Short)
					{
					  classes[i] = typeof(short);
					}
					else if (obj is Integer)
					{
					  classes[i] = typeof(int);
					}
					else if (obj is Long)
					{
					  classes[i] = typeof(long);
					}
					else if (obj is Float)
					{
					  classes[i] = typeof(float);
					}
					else if (obj is Double)
					{
					  classes[i] = typeof(double);
					}
					else
					{
					  classes[i] = obj.GetType();
					}
				}
    
				return classes;
			}
		}
	}

}
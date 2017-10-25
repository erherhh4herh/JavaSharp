using System;

/*
 * Copyright (c) 2012, 2013, Oracle and/or its affiliates. All rights reserved.
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
 *
 *
 *
 *
 *
 * Copyright (c) 2011-2012, Stephen Colebourne & Michael Nascimento Santos
 *
 * All rights reserved.
 *
 * Redistribution and use in source and binary forms, with or without
 * modification, are permitted provided that the following conditions are met:
 *
 *  * Redistributions of source code must retain the above copyright notice,
 *    this list of conditions and the following disclaimer.
 *
 *  * Redistributions in binary form must reproduce the above copyright notice,
 *    this list of conditions and the following disclaimer in the documentation
 *    and/or other materials provided with the distribution.
 *
 *  * Neither the name of JSR-310 nor the names of its contributors
 *    may be used to endorse or promote products derived from this software
 *    without specific prior written permission.
 *
 * THIS SOFTWARE IS PROVIDED BY THE COPYRIGHT HOLDERS AND CONTRIBUTORS
 * "AS IS" AND ANY EXPRESS OR IMPLIED WARRANTIES, INCLUDING, BUT NOT
 * LIMITED TO, THE IMPLIED WARRANTIES OF MERCHANTABILITY AND FITNESS FOR
 * A PARTICULAR PURPOSE ARE DISCLAIMED. IN NO EVENT SHALL THE COPYRIGHT OWNER OR
 * CONTRIBUTORS BE LIABLE FOR ANY DIRECT, INDIRECT, INCIDENTAL, SPECIAL,
 * EXEMPLARY, OR CONSEQUENTIAL DAMAGES (INCLUDING, BUT NOT LIMITED TO,
 * PROCUREMENT OF SUBSTITUTE GOODS OR SERVICES; LOSS OF USE, DATA, OR
 * PROFITS; OR BUSINESS INTERRUPTION) HOWEVER CAUSED AND ON ANY THEORY OF
 * LIABILITY, WHETHER IN CONTRACT, STRICT LIABILITY, OR TORT (INCLUDING
 * NEGLIGENCE OR OTHERWISE) ARISING IN ANY WAY OUT OF THE USE OF THIS
 * SOFTWARE, EVEN IF ADVISED OF THE POSSIBILITY OF SUCH DAMAGE.
 */
namespace java.time.zone
{


	/// <summary>
	/// The shared serialization delegate for this package.
	/// 
	/// @implNote
	/// This class is mutable and should be created once per serialization.
	/// 
	/// @serial include
	/// @since 1.8
	/// </summary>
	[Serializable]
	internal sealed class Ser : Externalizable
	{

		/// <summary>
		/// Serialization version.
		/// </summary>
		private const long SerialVersionUID = -8885321777449118786L;

		/// <summary>
		/// Type for ZoneRules. </summary>
		internal const sbyte ZRULES = 1;
		/// <summary>
		/// Type for ZoneOffsetTransition. </summary>
		internal const sbyte ZOT = 2;
		/// <summary>
		/// Type for ZoneOffsetTransition. </summary>
		internal const sbyte ZOTRULE = 3;

		/// <summary>
		/// The type being serialized. </summary>
		private sbyte Type;
		/// <summary>
		/// The object being serialized. </summary>
		private Object @object;

		/// <summary>
		/// Constructor for deserialization.
		/// </summary>
		public Ser()
		{
		}

		/// <summary>
		/// Creates an instance for serialization.
		/// </summary>
		/// <param name="type">  the type </param>
		/// <param name="object">  the object </param>
		internal Ser(sbyte type, Object @object)
		{
			this.Type = type;
			this.@object = @object;
		}

		//-----------------------------------------------------------------------
		/// <summary>
		/// Implements the {@code Externalizable} interface to write the object.
		/// @serialData
		/// Each serializable class is mapped to a type that is the first byte
		/// in the stream.  Refer to each class {@code writeReplace}
		/// serialized form for the value of the type and sequence of values for the type.
		/// 
		/// <ul>
		/// <li><a href="../../../serialized-form.html#java.time.zone.ZoneRules">ZoneRules.writeReplace</a>
		/// <li><a href="../../../serialized-form.html#java.time.zone.ZoneOffsetTransition">ZoneOffsetTransition.writeReplace</a>
		/// <li><a href="../../../serialized-form.html#java.time.zone.ZoneOffsetTransitionRule">ZoneOffsetTransitionRule.writeReplace</a>
		/// </ul>
		/// </summary>
		/// <param name="out">  the data stream to write to, not null </param>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: @Override public void writeExternal(java.io.ObjectOutput out) throws java.io.IOException
		public void WriteExternal(ObjectOutput @out)
		{
			WriteInternal(Type, @object, @out);
		}

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: static void write(Object object, java.io.DataOutput out) throws java.io.IOException
		internal static void Write(Object @object, DataOutput @out)
		{
			WriteInternal(ZRULES, @object, @out);
		}

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: private static void writeInternal(byte type, Object object, java.io.DataOutput out) throws java.io.IOException
		private static void WriteInternal(sbyte type, Object @object, DataOutput @out)
		{
			@out.WriteByte(type);
			switch (type)
			{
				case ZRULES:
					((ZoneRules) @object).WriteExternal(@out);
					break;
				case ZOT:
					((ZoneOffsetTransition) @object).WriteExternal(@out);
					break;
				case ZOTRULE:
					((ZoneOffsetTransitionRule) @object).WriteExternal(@out);
					break;
				default:
					throw new InvalidClassException("Unknown serialized type");
			}
		}

		//-----------------------------------------------------------------------
		/// <summary>
		/// Implements the {@code Externalizable} interface to read the object.
		/// @serialData
		/// The streamed type and parameters defined by the type's {@code writeReplace}
		/// method are read and passed to the corresponding static factory for the type
		/// to create a new instance.  That instance is returned as the de-serialized
		/// {@code Ser} object.
		/// 
		/// <ul>
		/// <li><a href="../../../serialized-form.html#java.time.zone.ZoneRules">ZoneRules</a>
		/// - {@code ZoneRules.of(standardTransitions, standardOffsets, savingsInstantTransitions, wallOffsets, lastRules);}
		/// <li><a href="../../../serialized-form.html#java.time.zone.ZoneOffsetTransition">ZoneOffsetTransition</a>
		/// - {@code ZoneOffsetTransition of(LocalDateTime.ofEpochSecond(epochSecond), offsetBefore, offsetAfter);}
		/// <li><a href="../../../serialized-form.html#java.time.zone.ZoneOffsetTransitionRule">ZoneOffsetTransitionRule</a>
		/// - {@code ZoneOffsetTransitionRule.of(month, dom, dow, time, timeEndOfDay, timeDefinition, standardOffset, offsetBefore, offsetAfter);}
		/// </ul> </summary>
		/// <param name="in">  the data to read, not null </param>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: @Override public void readExternal(java.io.ObjectInput in) throws java.io.IOException, ClassNotFoundException
		public void ReadExternal(ObjectInput @in)
		{
			Type = @in.ReadByte();
			@object = ReadInternal(Type, @in);
		}

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: static Object read(java.io.DataInput in) throws java.io.IOException, ClassNotFoundException
		internal static Object Read(DataInput @in)
		{
			sbyte type = @in.ReadByte();
			return ReadInternal(type, @in);
		}

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: private static Object readInternal(byte type, java.io.DataInput in) throws java.io.IOException, ClassNotFoundException
		private static Object ReadInternal(sbyte type, DataInput @in)
		{
			switch (type)
			{
				case ZRULES:
					return ZoneRules.ReadExternal(@in);
				case ZOT:
					return ZoneOffsetTransition.ReadExternal(@in);
				case ZOTRULE:
					return ZoneOffsetTransitionRule.ReadExternal(@in);
				default:
					throw new StreamCorruptedException("Unknown serialized type");
			}
		}

		/// <summary>
		/// Returns the object that will replace this one.
		/// </summary>
		/// <returns> the read object, should never be null </returns>
		private Object ReadResolve()
		{
			 return @object;
		}

		//-----------------------------------------------------------------------
		/// <summary>
		/// Writes the state to the stream.
		/// </summary>
		/// <param name="offset">  the offset, not null </param>
		/// <param name="out">  the output stream, not null </param>
		/// <exception cref="IOException"> if an error occurs </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: static void writeOffset(java.time.ZoneOffset offset, java.io.DataOutput out) throws java.io.IOException
		internal static void WriteOffset(ZoneOffset offset, DataOutput @out)
		{
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final int offsetSecs = offset.getTotalSeconds();
			int offsetSecs = offset.TotalSeconds;
			int offsetByte = offsetSecs % 900 == 0 ? offsetSecs / 900 : 127; // compress to -72 to +72
			@out.WriteByte(offsetByte);
			if (offsetByte == 127)
			{
				@out.WriteInt(offsetSecs);
			}
		}

		/// <summary>
		/// Reads the state from the stream.
		/// </summary>
		/// <param name="in">  the input stream, not null </param>
		/// <returns> the created object, not null </returns>
		/// <exception cref="IOException"> if an error occurs </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: static java.time.ZoneOffset readOffset(java.io.DataInput in) throws java.io.IOException
		internal static ZoneOffset ReadOffset(DataInput @in)
		{
			int offsetByte = @in.ReadByte();
			return (offsetByte == 127 ? ZoneOffset.OfTotalSeconds(@in.ReadInt()) : ZoneOffset.OfTotalSeconds(offsetByte * 900));
		}

		//-----------------------------------------------------------------------
		/// <summary>
		/// Writes the state to the stream.
		/// </summary>
		/// <param name="epochSec">  the epoch seconds, not null </param>
		/// <param name="out">  the output stream, not null </param>
		/// <exception cref="IOException"> if an error occurs </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: static void writeEpochSec(long epochSec, java.io.DataOutput out) throws java.io.IOException
		internal static void WriteEpochSec(long epochSec, DataOutput @out)
		{
			if (epochSec >= -4575744000L && epochSec < 10413792000L && epochSec % 900 == 0) // quarter hours between 1825 and 2300
			{
				int store = unchecked((int)((epochSec + 4575744000L) / 900));
				@out.WriteByte(((int)((uint)store >> 16)) & 255);
				@out.WriteByte(((int)((uint)store >> 8)) & 255);
				@out.WriteByte(store & 255);
			}
			else
			{
				@out.WriteByte(255);
				@out.WriteLong(epochSec);
			}
		}

		/// <summary>
		/// Reads the state from the stream.
		/// </summary>
		/// <param name="in">  the input stream, not null </param>
		/// <returns> the epoch seconds, not null </returns>
		/// <exception cref="IOException"> if an error occurs </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: static long readEpochSec(java.io.DataInput in) throws java.io.IOException
		internal static long ReadEpochSec(DataInput @in)
		{
			int hiByte = @in.ReadByte() & 255;
			if (hiByte == 255)
			{
				return @in.ReadLong();
			}
			else
			{
				int midByte = @in.ReadByte() & 255;
				int loByte = @in.ReadByte() & 255;
				long tot = ((hiByte << 16) + (midByte << 8) + loByte);
				return (tot * 900) - 4575744000L;
			}
		}

	}

}
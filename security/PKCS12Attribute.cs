using System;

/*
 * Copyright (c) 2013, Oracle and/or its affiliates. All rights reserved.
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

namespace java.security
{

	using sun.security.util;

	/// <summary>
	/// An attribute associated with a PKCS12 keystore entry.
	/// The attribute name is an ASN.1 Object Identifier and the attribute
	/// value is a set of ASN.1 types.
	/// 
	/// @since 1.8
	/// </summary>
	public sealed class PKCS12Attribute : KeyStore.Entry_Attribute
	{

		private static readonly Pattern COLON_SEPARATED_HEX_PAIRS = Pattern.Compile("^[0-9a-fA-F]{2}(:[0-9a-fA-F]{2})+$");
		private String Name_Renamed;
		private String Value_Renamed;
		private sbyte[] Encoded_Renamed;
		private int HashValue = -1;

		/// <summary>
		/// Constructs a PKCS12 attribute from its name and value.
		/// The name is an ASN.1 Object Identifier represented as a list of
		/// dot-separated integers.
		/// A string value is represented as the string itself.
		/// A binary value is represented as a string of colon-separated
		/// pairs of hexadecimal digits.
		/// Multi-valued attributes are represented as a comma-separated
		/// list of values, enclosed in square brackets. See
		/// <seealso cref="Arrays#toString(java.lang.Object[])"/>.
		/// <para>
		/// A string value will be DER-encoded as an ASN.1 UTF8String and a
		/// binary value will be DER-encoded as an ASN.1 Octet String.
		/// 
		/// </para>
		/// </summary>
		/// <param name="name"> the attribute's identifier </param>
		/// <param name="value"> the attribute's value
		/// </param>
		/// <exception cref="NullPointerException"> if {@code name} or {@code value}
		///     is {@code null} </exception>
		/// <exception cref="IllegalArgumentException"> if {@code name} or
		///     {@code value} is incorrectly formatted </exception>
		public PKCS12Attribute(String name, String value)
		{
			if (name == null || value == null)
			{
				throw new NullPointerException();
			}
			// Validate name
			ObjectIdentifier type;
			try
			{
				type = new ObjectIdentifier(name);
			}
			catch (IOException e)
			{
				throw new IllegalArgumentException("Incorrect format: name", e);
			}
			this.Name_Renamed = name;

			// Validate value
			int length = value.Length();
			String[] values;
			if (value.CharAt(0) == '[' && value.CharAt(length - 1) == ']')
			{
				values = value.Substring(1, length - 1 - 1).Split(", ", true);
			}
			else
			{
				values = new String[]{value};
			}
			this.Value_Renamed = value;

			try
			{
				this.Encoded_Renamed = Encode(type, values);
			}
			catch (IOException e)
			{
				throw new IllegalArgumentException("Incorrect format: value", e);
			}
		}

		/// <summary>
		/// Constructs a PKCS12 attribute from its ASN.1 DER encoding.
		/// The DER encoding is specified by the following ASN.1 definition:
		/// <pre>
		/// 
		/// Attribute ::= SEQUENCE {
		///     type   AttributeType,
		///     values SET OF AttributeValue
		/// }
		/// AttributeType ::= OBJECT IDENTIFIER
		/// AttributeValue ::= ANY defined by type
		/// 
		/// </pre>
		/// </summary>
		/// <param name="encoded"> the attribute's ASN.1 DER encoding. It is cloned
		///     to prevent subsequent modificaion.
		/// </param>
		/// <exception cref="NullPointerException"> if {@code encoded} is
		///     {@code null} </exception>
		/// <exception cref="IllegalArgumentException"> if {@code encoded} is
		///     incorrectly formatted </exception>
		public PKCS12Attribute(sbyte[] encoded)
		{
			if (encoded == null)
			{
				throw new NullPointerException();
			}
			this.Encoded_Renamed = encoded.clone();

			try
			{
				Parse(encoded);
			}
			catch (IOException e)
			{
				throw new IllegalArgumentException("Incorrect format: encoded", e);
			}
		}

		/// <summary>
		/// Returns the attribute's ASN.1 Object Identifier represented as a
		/// list of dot-separated integers.
		/// </summary>
		/// <returns> the attribute's identifier </returns>
		public String Name
		{
			get
			{
				return Name_Renamed;
			}
		}

		/// <summary>
		/// Returns the attribute's ASN.1 DER-encoded value as a string.
		/// An ASN.1 DER-encoded value is returned in one of the following
		/// {@code String} formats:
		/// <ul>
		/// <li> the DER encoding of a basic ASN.1 type that has a natural
		///      string representation is returned as the string itself.
		///      Such types are currently limited to BOOLEAN, INTEGER,
		///      OBJECT IDENTIFIER, UTCTime, GeneralizedTime and the
		///      following six ASN.1 string types: UTF8String,
		///      PrintableString, T61String, IA5String, BMPString and
		///      GeneralString.
		/// <li> the DER encoding of any other ASN.1 type is not decoded but
		///      returned as a binary string of colon-separated pairs of
		///      hexadecimal digits.
		/// </ul>
		/// Multi-valued attributes are represented as a comma-separated
		/// list of values, enclosed in square brackets. See
		/// <seealso cref="Arrays#toString(java.lang.Object[])"/>.
		/// </summary>
		/// <returns> the attribute value's string encoding </returns>
		public String Value
		{
			get
			{
				return Value_Renamed;
			}
		}

		/// <summary>
		/// Returns the attribute's ASN.1 DER encoding.
		/// </summary>
		/// <returns> a clone of the attribute's DER encoding </returns>
		public sbyte[] Encoded
		{
			get
			{
				return Encoded_Renamed.clone();
			}
		}

		/// <summary>
		/// Compares this {@code PKCS12Attribute} and a specified object for
		/// equality.
		/// </summary>
		/// <param name="obj"> the comparison object
		/// </param>
		/// <returns> true if {@code obj} is a {@code PKCS12Attribute} and
		/// their DER encodings are equal. </returns>
		public override bool Equals(Object obj)
		{
			if (this == obj)
			{
				return true;
			}
			if (!(obj is PKCS12Attribute))
			{
				return false;
			}
			return Arrays.Equals(Encoded_Renamed, ((PKCS12Attribute) obj).Encoded);
		}

		/// <summary>
		/// Returns the hashcode for this {@code PKCS12Attribute}.
		/// The hash code is computed from its DER encoding.
		/// </summary>
		/// <returns> the hash code </returns>
		public override int HashCode()
		{
			if (HashValue == -1)
			{
				Arrays.HashCode(Encoded_Renamed);
			}
			return HashValue;
		}

		/// <summary>
		/// Returns a string representation of this {@code PKCS12Attribute}.
		/// </summary>
		/// <returns> a name/value pair separated by an 'equals' symbol </returns>
		public override String ToString()
		{
			return (Name_Renamed + "=" + Value_Renamed);
		}

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: private byte[] encode(ObjectIdentifier type, String[] values) throws java.io.IOException
		private sbyte[] Encode(ObjectIdentifier type, String[] values)
		{
			DerOutputStream attribute = new DerOutputStream();
			attribute.putOID(type);
			DerOutputStream attrContent = new DerOutputStream();
			foreach (String value in values)
			{
				if (COLON_SEPARATED_HEX_PAIRS.Matcher(value).Matches())
				{
					sbyte[] bytes = (new System.Numerics.BigInteger(value.Replace(":", ""), 16)).ToByteArray();
					if (bytes[0] == 0)
					{
						bytes = Arrays.CopyOfRange(bytes, 1, bytes.Length);
					}
					attrContent.putOctetString(bytes);
				}
				else
				{
					attrContent.putUTF8String(value);
				}
			}
			attribute.write(DerValue.tag_Set, attrContent);
			DerOutputStream attributeValue = new DerOutputStream();
			attributeValue.write(DerValue.tag_Sequence, attribute);

			return attributeValue.toByteArray();
		}

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: private void parse(byte[] encoded) throws java.io.IOException
		private void Parse(sbyte[] encoded)
		{
			DerInputStream attributeValue = new DerInputStream(encoded);
			DerValue[] attrSeq = attributeValue.getSequence(2);
			ObjectIdentifier type = attrSeq[0].OID;
			DerInputStream attrContent = new DerInputStream(attrSeq[1].toByteArray());
			DerValue[] attrValueSet = attrContent.getSet(1);
			String[] values = new String[attrValueSet.Length];
			String printableString;
			for (int i = 0; i < attrValueSet.Length; i++)
			{
				if (attrValueSet[i].tag == DerValue.tag_OctetString)
				{
					values[i] = Debug.ToString(attrValueSet[i].OctetString);
				}
				else if ((printableString = attrValueSet[i].AsString) != null)
				{
					values[i] = printableString;
				}
				else if (attrValueSet[i].tag == DerValue.tag_ObjectId)
				{
					values[i] = attrValueSet[i].OID.ToString();
				}
				else if (attrValueSet[i].tag == DerValue.tag_GeneralizedTime)
				{
					values[i] = attrValueSet[i].GeneralizedTime.ToString();
				}
				else if (attrValueSet[i].tag == DerValue.tag_UtcTime)
				{
					values[i] = attrValueSet[i].UTCTime.ToString();
				}
				else if (attrValueSet[i].tag == DerValue.tag_Integer)
				{
					values[i] = attrValueSet[i].BigInteger.ToString();
				}
				else if (attrValueSet[i].tag == DerValue.tag_Boolean)
				{
					values[i] = Convert.ToString(attrValueSet[i].Boolean);
				}
				else
				{
					values[i] = Debug.ToString(attrValueSet[i].DataBytes);
				}
			}

			this.Name_Renamed = type.ToString();
			this.Value_Renamed = values.Length == 1 ? values[0] : Arrays.ToString(values);
		}
	}

}
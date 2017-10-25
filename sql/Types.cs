/*
 * Copyright (c) 1996, 2013, Oracle and/or its affiliates. All rights reserved.
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
	/// <P>The class that defines the constants that are used to identify generic
	/// SQL types, called JDBC types.
	/// <para>
	/// This class is never instantiated.
	/// </para>
	/// </summary>
	public class Types
	{

	/// <summary>
	/// <P>The constant in the Java programming language, sometimes referred
	/// to as a type code, that identifies the generic SQL type
	/// <code>BIT</code>.
	/// </summary>
			public const int BIT = -7;

	/// <summary>
	/// <P>The constant in the Java programming language, sometimes referred
	/// to as a type code, that identifies the generic SQL type
	/// <code>TINYINT</code>.
	/// </summary>
			public const int TINYINT = -6;

	/// <summary>
	/// <P>The constant in the Java programming language, sometimes referred
	/// to as a type code, that identifies the generic SQL type
	/// <code>SMALLINT</code>.
	/// </summary>
			public const int SMALLINT = 5;

	/// <summary>
	/// <P>The constant in the Java programming language, sometimes referred
	/// to as a type code, that identifies the generic SQL type
	/// <code>INTEGER</code>.
	/// </summary>
			public const int INTEGER = 4;

	/// <summary>
	/// <P>The constant in the Java programming language, sometimes referred
	/// to as a type code, that identifies the generic SQL type
	/// <code>BIGINT</code>.
	/// </summary>
			public const int BIGINT = -5;

	/// <summary>
	/// <P>The constant in the Java programming language, sometimes referred
	/// to as a type code, that identifies the generic SQL type
	/// <code>FLOAT</code>.
	/// </summary>
			public const int FLOAT = 6;

	/// <summary>
	/// <P>The constant in the Java programming language, sometimes referred
	/// to as a type code, that identifies the generic SQL type
	/// <code>REAL</code>.
	/// </summary>
			public const int REAL = 7;


	/// <summary>
	/// <P>The constant in the Java programming language, sometimes referred
	/// to as a type code, that identifies the generic SQL type
	/// <code>DOUBLE</code>.
	/// </summary>
			public const int DOUBLE = 8;

	/// <summary>
	/// <P>The constant in the Java programming language, sometimes referred
	/// to as a type code, that identifies the generic SQL type
	/// <code>NUMERIC</code>.
	/// </summary>
			public const int NUMERIC = 2;

	/// <summary>
	/// <P>The constant in the Java programming language, sometimes referred
	/// to as a type code, that identifies the generic SQL type
	/// <code>DECIMAL</code>.
	/// </summary>
			public const int DECIMAL = 3;

	/// <summary>
	/// <P>The constant in the Java programming language, sometimes referred
	/// to as a type code, that identifies the generic SQL type
	/// <code>CHAR</code>.
	/// </summary>
			public const int CHAR = 1;

	/// <summary>
	/// <P>The constant in the Java programming language, sometimes referred
	/// to as a type code, that identifies the generic SQL type
	/// <code>VARCHAR</code>.
	/// </summary>
			public const int VARCHAR = 12;

	/// <summary>
	/// <P>The constant in the Java programming language, sometimes referred
	/// to as a type code, that identifies the generic SQL type
	/// <code>LONGVARCHAR</code>.
	/// </summary>
			public const int LONGVARCHAR = -1;


	/// <summary>
	/// <P>The constant in the Java programming language, sometimes referred
	/// to as a type code, that identifies the generic SQL type
	/// <code>DATE</code>.
	/// </summary>
			public const int DATE = 91;

	/// <summary>
	/// <P>The constant in the Java programming language, sometimes referred
	/// to as a type code, that identifies the generic SQL type
	/// <code>TIME</code>.
	/// </summary>
			public const int TIME = 92;

	/// <summary>
	/// <P>The constant in the Java programming language, sometimes referred
	/// to as a type code, that identifies the generic SQL type
	/// <code>TIMESTAMP</code>.
	/// </summary>
			public const int TIMESTAMP = 93;


	/// <summary>
	/// <P>The constant in the Java programming language, sometimes referred
	/// to as a type code, that identifies the generic SQL type
	/// <code>BINARY</code>.
	/// </summary>
			public const int BINARY = -2;

	/// <summary>
	/// <P>The constant in the Java programming language, sometimes referred
	/// to as a type code, that identifies the generic SQL type
	/// <code>VARBINARY</code>.
	/// </summary>
			public const int VARBINARY = -3;

	/// <summary>
	/// <P>The constant in the Java programming language, sometimes referred
	/// to as a type code, that identifies the generic SQL type
	/// <code>LONGVARBINARY</code>.
	/// </summary>
			public const int LONGVARBINARY = -4;

	/// <summary>
	/// <P>The constant in the Java programming language
	/// that identifies the generic SQL value
	/// <code>NULL</code>.
	/// </summary>
			public const int NULL = 0;

		/// <summary>
		/// The constant in the Java programming language that indicates
		/// that the SQL type is database-specific and
		/// gets mapped to a Java object that can be accessed via
		/// the methods <code>getObject</code> and <code>setObject</code>.
		/// </summary>
			public const int OTHER = 1111;



		/// <summary>
		/// The constant in the Java programming language, sometimes referred to
		/// as a type code, that identifies the generic SQL type
		/// <code>JAVA_OBJECT</code>.
		/// @since 1.2
		/// </summary>
			public const int JAVA_OBJECT = 2000;

		/// <summary>
		/// The constant in the Java programming language, sometimes referred to
		/// as a type code, that identifies the generic SQL type
		/// <code>DISTINCT</code>.
		/// @since 1.2
		/// </summary>
			public const int DISTINCT = 2001;

		/// <summary>
		/// The constant in the Java programming language, sometimes referred to
		/// as a type code, that identifies the generic SQL type
		/// <code>STRUCT</code>.
		/// @since 1.2
		/// </summary>
			public const int STRUCT = 2002;

		/// <summary>
		/// The constant in the Java programming language, sometimes referred to
		/// as a type code, that identifies the generic SQL type
		/// <code>ARRAY</code>.
		/// @since 1.2
		/// </summary>
			public const int ARRAY = 2003;

		/// <summary>
		/// The constant in the Java programming language, sometimes referred to
		/// as a type code, that identifies the generic SQL type
		/// <code>BLOB</code>.
		/// @since 1.2
		/// </summary>
			public const int BLOB = 2004;

		/// <summary>
		/// The constant in the Java programming language, sometimes referred to
		/// as a type code, that identifies the generic SQL type
		/// <code>CLOB</code>.
		/// @since 1.2
		/// </summary>
			public const int CLOB = 2005;

		/// <summary>
		/// The constant in the Java programming language, sometimes referred to
		/// as a type code, that identifies the generic SQL type
		/// <code>REF</code>.
		/// @since 1.2
		/// </summary>
			public const int REF = 2006;

		/// <summary>
		/// The constant in the Java programming language, somtimes referred to
		/// as a type code, that identifies the generic SQL type <code>DATALINK</code>.
		/// 
		/// @since 1.4
		/// </summary>
		public const int DATALINK = 70;

		/// <summary>
		/// The constant in the Java programming language, somtimes referred to
		/// as a type code, that identifies the generic SQL type <code>BOOLEAN</code>.
		/// 
		/// @since 1.4
		/// </summary>
		public const int BOOLEAN = 16;

		//------------------------- JDBC 4.0 -----------------------------------

		/// <summary>
		/// The constant in the Java programming language, sometimes referred to
		/// as a type code, that identifies the generic SQL type <code>ROWID</code>
		/// 
		/// @since 1.6
		/// 
		/// </summary>
		public const int ROWID = -8;

		/// <summary>
		/// The constant in the Java programming language, sometimes referred to
		/// as a type code, that identifies the generic SQL type <code>NCHAR</code>
		/// 
		/// @since 1.6
		/// </summary>
		public const int NCHAR = -15;

		/// <summary>
		/// The constant in the Java programming language, sometimes referred to
		/// as a type code, that identifies the generic SQL type <code>NVARCHAR</code>.
		/// 
		/// @since 1.6
		/// </summary>
		public const int NVARCHAR = -9;

		/// <summary>
		/// The constant in the Java programming language, sometimes referred to
		/// as a type code, that identifies the generic SQL type <code>LONGNVARCHAR</code>.
		/// 
		/// @since 1.6
		/// </summary>
		public const int LONGNVARCHAR = -16;

		/// <summary>
		/// The constant in the Java programming language, sometimes referred to
		/// as a type code, that identifies the generic SQL type <code>NCLOB</code>.
		/// 
		/// @since 1.6
		/// </summary>
		public const int NCLOB = 2011;

		/// <summary>
		/// The constant in the Java programming language, sometimes referred to
		/// as a type code, that identifies the generic SQL type <code>XML</code>.
		/// 
		/// @since 1.6
		/// </summary>
		public const int SQLXML = 2009;

		//--------------------------JDBC 4.2 -----------------------------

		/// <summary>
		/// The constant in the Java programming language, sometimes referred to
		/// as a type code, that identifies the generic SQL type {@code REF CURSOR}.
		/// 
		/// @since 1.8
		/// </summary>
		public const int REF_CURSOR = 2012;

		/// <summary>
		/// The constant in the Java programming language, sometimes referred to
		/// as a type code, that identifies the generic SQL type
		/// {@code TIME WITH TIMEZONE}.
		/// 
		/// @since 1.8
		/// </summary>
		public const int TIME_WITH_TIMEZONE = 2013;

		/// <summary>
		/// The constant in the Java programming language, sometimes referred to
		/// as a type code, that identifies the generic SQL type
		/// {@code TIMESTAMP WITH TIMEZONE}.
		/// 
		/// @since 1.8
		/// </summary>
		public const int TIMESTAMP_WITH_TIMEZONE = 2014;

		// Prevent instantiation
		private Types()
		{
		}
	}

}
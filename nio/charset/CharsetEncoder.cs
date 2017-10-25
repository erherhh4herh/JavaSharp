using System.Diagnostics;

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

// -- This file was mechanically generated: Do not edit! -- //

namespace java.nio.charset
{



	/// <summary>
	/// An engine that can transform a sequence of sixteen-bit Unicode characters into a sequence of
	/// bytes in a specific charset.
	/// 
	/// <a name="steps"></a>
	/// 
	/// <para> The input character sequence is provided in a character buffer or a series
	/// of such buffers.  The output byte sequence is written to a byte buffer
	/// or a series of such buffers.  An encoder should always be used by making
	/// the following sequence of method invocations, hereinafter referred to as an
	/// <i>encoding operation</i>:
	/// 
	/// <ol>
	/// 
	/// </para>
	///   <li><para> Reset the encoder via the <seealso cref="#reset reset"/> method, unless it
	///   has not been used before; </para></li>
	/// 
	///   <li><para> Invoke the <seealso cref="#encode encode"/> method zero or more times, as
	///   long as additional input may be available, passing <tt>false</tt> for the
	///   <tt>endOfInput</tt> argument and filling the input buffer and flushing the
	///   output buffer between invocations; </para></li>
	/// 
	///   <li><para> Invoke the <seealso cref="#encode encode"/> method one final time, passing
	///   <tt>true</tt> for the <tt>endOfInput</tt> argument; and then </para></li>
	/// 
	///   <li><para> Invoke the <seealso cref="#flush flush"/> method so that the encoder can
	///   flush any internal state to the output buffer. </para></li>
	/// 
	/// </ol>
	/// 
	/// Each invocation of the <seealso cref="#encode encode"/> method will encode as many
	/// characters as possible from the input buffer, writing the resulting bytes
	/// to the output buffer.  The <seealso cref="#encode encode"/> method returns when more
	/// input is required, when there is not enough room in the output buffer, or
	/// when an encoding error has occurred.  In each case a <seealso cref="CoderResult"/>
	/// object is returned to describe the reason for termination.  An invoker can
	/// examine this object and fill the input buffer, flush the output buffer, or
	/// attempt to recover from an encoding error, as appropriate, and try again.
	/// 
	/// <a name="ce"></a>
	/// 
	/// <para> There are two general types of encoding errors.  If the input character
	/// sequence is not a legal sixteen-bit Unicode sequence then the input is considered <i>malformed</i>.  If
	/// the input character sequence is legal but cannot be mapped to a valid
	/// byte sequence in the given charset then an <i>unmappable character</i> has been encountered.
	/// 
	/// <a name="cae"></a>
	/// 
	/// </para>
	/// <para> How an encoding error is handled depends upon the action requested for
	/// that type of error, which is described by an instance of the {@link
	/// CodingErrorAction} class.  The possible error actions are to {@linkplain
	/// CodingErrorAction#IGNORE ignore} the erroneous input, {@linkplain
	/// CodingErrorAction#REPORT report} the error to the invoker via
	/// the returned <seealso cref="CoderResult"/> object, or {@link CodingErrorAction#REPLACE
	/// replace} the erroneous input with the current value of the
	/// replacement byte array.  The replacement
	/// 
	/// 
	/// is initially set to the encoder's default replacement, which often
	/// (but not always) has the initial value&nbsp;<tt>{</tt>&nbsp;<tt>(byte)'?'</tt>&nbsp;<tt>}</tt>;
	/// 
	/// 
	/// 
	/// 
	/// 
	/// its value may be changed via the {@link #replaceWith(byte[])
	/// replaceWith} method.
	/// 
	/// </para>
	/// <para> The default action for malformed-input and unmappable-character errors
	/// is to <seealso cref="CodingErrorAction#REPORT report"/> them.  The
	/// malformed-input error action may be changed via the {@link
	/// #onMalformedInput(CodingErrorAction) onMalformedInput} method; the
	/// unmappable-character action may be changed via the {@link
	/// #onUnmappableCharacter(CodingErrorAction) onUnmappableCharacter} method.
	/// 
	/// </para>
	/// <para> This class is designed to handle many of the details of the encoding
	/// process, including the implementation of error actions.  An encoder for a
	/// specific charset, which is a concrete subclass of this class, need only
	/// implement the abstract <seealso cref="#encodeLoop encodeLoop"/> method, which
	/// encapsulates the basic encoding loop.  A subclass that maintains internal
	/// state should, additionally, override the <seealso cref="#implFlush implFlush"/> and
	/// <seealso cref="#implReset implReset"/> methods.
	/// 
	/// </para>
	/// <para> Instances of this class are not safe for use by multiple concurrent
	/// threads.  </para>
	/// 
	/// 
	/// @author Mark Reinhold
	/// @author JSR-51 Expert Group
	/// @since 1.4
	/// </summary>
	/// <seealso cref= ByteBuffer </seealso>
	/// <seealso cref= CharBuffer </seealso>
	/// <seealso cref= Charset </seealso>
	/// <seealso cref= CharsetDecoder </seealso>

	public abstract class CharsetEncoder
	{

		private readonly Charset Charset_Renamed;
		private readonly float AverageBytesPerChar_Renamed;
		private readonly float MaxBytesPerChar_Renamed;

		private sbyte[] Replacement_Renamed;
		private CodingErrorAction MalformedInputAction_Renamed = CodingErrorAction.REPORT;
		private CodingErrorAction UnmappableCharacterAction_Renamed = CodingErrorAction.REPORT;

		// Internal states
		//
		private const int ST_RESET = 0;
		private const int ST_CODING = 1;
		private const int ST_END = 2;
		private const int ST_FLUSHED = 3;

		private int State = ST_RESET;

		private static String[] StateNames = new String[] {"RESET", "CODING", "CODING_END", "FLUSHED"};


		/// <summary>
		/// Initializes a new encoder.  The new encoder will have the given
		/// bytes-per-char and replacement values.
		/// </summary>
		/// <param name="cs">
		///         The charset that created this encoder
		/// </param>
		/// <param name="averageBytesPerChar">
		///         A positive float value indicating the expected number of
		///         bytes that will be produced for each input character
		/// </param>
		/// <param name="maxBytesPerChar">
		///         A positive float value indicating the maximum number of
		///         bytes that will be produced for each input character
		/// </param>
		/// <param name="replacement">
		///         The initial replacement; must not be <tt>null</tt>, must have
		///         non-zero length, must not be longer than maxBytesPerChar,
		///         and must be <seealso cref="#isLegalReplacement legal"/>
		/// </param>
		/// <exception cref="IllegalArgumentException">
		///          If the preconditions on the parameters do not hold </exception>
		protected internal CharsetEncoder(Charset cs, float averageBytesPerChar, float maxBytesPerChar, sbyte[] replacement)
		{
			this.Charset_Renamed = cs;
			if (averageBytesPerChar <= 0.0f)
			{
				throw new IllegalArgumentException("Non-positive " + "averageBytesPerChar");
			}
			if (maxBytesPerChar <= 0.0f)
			{
				throw new IllegalArgumentException("Non-positive " + "maxBytesPerChar");
			}
			if (!Charset.AtBugLevel("1.4"))
			{
				if (averageBytesPerChar > maxBytesPerChar)
				{
					throw new IllegalArgumentException("averageBytesPerChar" + " exceeds " + "maxBytesPerChar");
				}
			}
			this.Replacement_Renamed = replacement;
			this.AverageBytesPerChar_Renamed = averageBytesPerChar;
			this.MaxBytesPerChar_Renamed = maxBytesPerChar;
			ReplaceWith(replacement);
		}

		/// <summary>
		/// Initializes a new encoder.  The new encoder will have the given
		/// bytes-per-char values and its replacement will be the
		/// byte array <tt>{</tt>&nbsp;<tt>(byte)'?'</tt>&nbsp;<tt>}</tt>.
		/// </summary>
		/// <param name="cs">
		///         The charset that created this encoder
		/// </param>
		/// <param name="averageBytesPerChar">
		///         A positive float value indicating the expected number of
		///         bytes that will be produced for each input character
		/// </param>
		/// <param name="maxBytesPerChar">
		///         A positive float value indicating the maximum number of
		///         bytes that will be produced for each input character
		/// </param>
		/// <exception cref="IllegalArgumentException">
		///          If the preconditions on the parameters do not hold </exception>
		protected internal CharsetEncoder(Charset cs, float averageBytesPerChar, float maxBytesPerChar) : this(cs, averageBytesPerChar, maxBytesPerChar, new sbyte[] {(sbyte)'?'})
		{
		}

		/// <summary>
		/// Returns the charset that created this encoder.
		/// </summary>
		/// <returns>  This encoder's charset </returns>
		public Charset Charset()
		{
			return Charset_Renamed;
		}

		/// <summary>
		/// Returns this encoder's replacement value.
		/// </summary>
		/// <returns>  This encoder's current replacement,
		///          which is never <tt>null</tt> and is never empty </returns>
		public sbyte[] Replacement()
		{




			return Arrays.CopyOf(Replacement_Renamed, Replacement_Renamed.Length);

		}

		/// <summary>
		/// Changes this encoder's replacement value.
		/// 
		/// <para> This method invokes the <seealso cref="#implReplaceWith implReplaceWith"/>
		/// method, passing the new replacement, after checking that the new
		/// replacement is acceptable.  </para>
		/// </summary>
		/// <param name="newReplacement">  The replacement value
		/// 
		/// 
		/// 
		/// 
		/// 
		/// 
		///         The new replacement; must not be <tt>null</tt>, must have
		///         non-zero length, must not be longer than the value returned by
		///         the <seealso cref="#maxBytesPerChar() maxBytesPerChar"/> method, and
		///         must be <seealso cref="#isLegalReplacement legal"/>
		/// 
		/// </param>
		/// <returns>  This encoder
		/// </returns>
		/// <exception cref="IllegalArgumentException">
		///          If the preconditions on the parameter do not hold </exception>
		public CharsetEncoder ReplaceWith(sbyte[] newReplacement)
		{
			if (newReplacement == null)
			{
				throw new IllegalArgumentException("Null replacement");
			}
			int len = newReplacement.Length;
			if (len == 0)
			{
				throw new IllegalArgumentException("Empty replacement");
			}
			if (len > MaxBytesPerChar_Renamed)
			{
				throw new IllegalArgumentException("Replacement too long");
			}




			if (!IsLegalReplacement(newReplacement))
			{
				throw new IllegalArgumentException("Illegal replacement");
			}
			this.Replacement_Renamed = Arrays.CopyOf(newReplacement, newReplacement.Length);

			ImplReplaceWith(this.Replacement_Renamed);
			return this;
		}

		/// <summary>
		/// Reports a change to this encoder's replacement value.
		/// 
		/// <para> The default implementation of this method does nothing.  This method
		/// should be overridden by encoders that require notification of changes to
		/// the replacement.  </para>
		/// </summary>
		/// <param name="newReplacement">    The replacement value </param>
		protected internal virtual void ImplReplaceWith(sbyte[] newReplacement)
		{
		}



		private WeakReference<CharsetDecoder> CachedDecoder = null;

		/// <summary>
		/// Tells whether or not the given byte array is a legal replacement value
		/// for this encoder.
		/// 
		/// <para> A replacement is legal if, and only if, it is a legal sequence of
		/// bytes in this encoder's charset; that is, it must be possible to decode
		/// the replacement into one or more sixteen-bit Unicode characters.
		/// 
		/// </para>
		/// <para> The default implementation of this method is not very efficient; it
		/// should generally be overridden to improve performance.  </para>
		/// </summary>
		/// <param name="repl">  The byte array to be tested
		/// </param>
		/// <returns>  <tt>true</tt> if, and only if, the given byte array
		///          is a legal replacement value for this encoder </returns>
		public virtual bool IsLegalReplacement(sbyte[] repl)
		{
			WeakReference<CharsetDecoder> wr = CachedDecoder;
			CharsetDecoder dec = null;
			if ((wr == null) || ((dec = wr.get()) == null))
			{
				dec = Charset().NewDecoder();
				dec.OnMalformedInput(CodingErrorAction.REPORT);
				dec.OnUnmappableCharacter(CodingErrorAction.REPORT);
				CachedDecoder = new WeakReference<CharsetDecoder>(dec);
			}
			else
			{
				dec.Reset();
			}
			ByteBuffer bb = ByteBuffer.Wrap(repl);
			CharBuffer cb = CharBuffer.Allocate((int)(bb.Remaining() * dec.MaxCharsPerByte()));
			CoderResult cr = dec.Decode(bb, cb, true);
			return !cr.Error;
		}



		/// <summary>
		/// Returns this encoder's current action for malformed-input errors.
		/// </summary>
		/// <returns> The current malformed-input action, which is never <tt>null</tt> </returns>
		public virtual CodingErrorAction MalformedInputAction()
		{
			return MalformedInputAction_Renamed;
		}

		/// <summary>
		/// Changes this encoder's action for malformed-input errors.
		/// 
		/// <para> This method invokes the {@link #implOnMalformedInput
		/// implOnMalformedInput} method, passing the new action.  </para>
		/// </summary>
		/// <param name="newAction">  The new action; must not be <tt>null</tt>
		/// </param>
		/// <returns>  This encoder
		/// </returns>
		/// <exception cref="IllegalArgumentException">
		///         If the precondition on the parameter does not hold </exception>
		public CharsetEncoder OnMalformedInput(CodingErrorAction newAction)
		{
			if (newAction == null)
			{
				throw new IllegalArgumentException("Null action");
			}
			MalformedInputAction_Renamed = newAction;
			ImplOnMalformedInput(newAction);
			return this;
		}

		/// <summary>
		/// Reports a change to this encoder's malformed-input action.
		/// 
		/// <para> The default implementation of this method does nothing.  This method
		/// should be overridden by encoders that require notification of changes to
		/// the malformed-input action.  </para>
		/// </summary>
		/// <param name="newAction">  The new action </param>
		protected internal virtual void ImplOnMalformedInput(CodingErrorAction newAction)
		{
		}

		/// <summary>
		/// Returns this encoder's current action for unmappable-character errors.
		/// </summary>
		/// <returns> The current unmappable-character action, which is never
		///         <tt>null</tt> </returns>
		public virtual CodingErrorAction UnmappableCharacterAction()
		{
			return UnmappableCharacterAction_Renamed;
		}

		/// <summary>
		/// Changes this encoder's action for unmappable-character errors.
		/// 
		/// <para> This method invokes the {@link #implOnUnmappableCharacter
		/// implOnUnmappableCharacter} method, passing the new action.  </para>
		/// </summary>
		/// <param name="newAction">  The new action; must not be <tt>null</tt>
		/// </param>
		/// <returns>  This encoder
		/// </returns>
		/// <exception cref="IllegalArgumentException">
		///         If the precondition on the parameter does not hold </exception>
		public CharsetEncoder OnUnmappableCharacter(CodingErrorAction newAction)
		{
			if (newAction == null)
			{
				throw new IllegalArgumentException("Null action");
			}
			UnmappableCharacterAction_Renamed = newAction;
			ImplOnUnmappableCharacter(newAction);
			return this;
		}

		/// <summary>
		/// Reports a change to this encoder's unmappable-character action.
		/// 
		/// <para> The default implementation of this method does nothing.  This method
		/// should be overridden by encoders that require notification of changes to
		/// the unmappable-character action.  </para>
		/// </summary>
		/// <param name="newAction">  The new action </param>
		protected internal virtual void ImplOnUnmappableCharacter(CodingErrorAction newAction)
		{
		}

		/// <summary>
		/// Returns the average number of bytes that will be produced for each
		/// character of input.  This heuristic value may be used to estimate the size
		/// of the output buffer required for a given input sequence.
		/// </summary>
		/// <returns>  The average number of bytes produced
		///          per character of input </returns>
		public float AverageBytesPerChar()
		{
			return AverageBytesPerChar_Renamed;
		}

		/// <summary>
		/// Returns the maximum number of bytes that will be produced for each
		/// character of input.  This value may be used to compute the worst-case size
		/// of the output buffer required for a given input sequence.
		/// </summary>
		/// <returns>  The maximum number of bytes that will be produced per
		///          character of input </returns>
		public float MaxBytesPerChar()
		{
			return MaxBytesPerChar_Renamed;
		}

		/// <summary>
		/// Encodes as many characters as possible from the given input buffer,
		/// writing the results to the given output buffer.
		/// 
		/// <para> The buffers are read from, and written to, starting at their current
		/// positions.  At most <seealso cref="Buffer#remaining in.remaining()"/> characters
		/// will be read and at most <seealso cref="Buffer#remaining out.remaining()"/>
		/// bytes will be written.  The buffers' positions will be advanced to
		/// reflect the characters read and the bytes written, but their marks and
		/// limits will not be modified.
		/// 
		/// </para>
		/// <para> In addition to reading characters from the input buffer and writing
		/// bytes to the output buffer, this method returns a <seealso cref="CoderResult"/>
		/// object to describe its reason for termination:
		/// 
		/// <ul>
		/// 
		/// </para>
		///   <li><para> <seealso cref="CoderResult#UNDERFLOW"/> indicates that as much of the
		///   input buffer as possible has been encoded.  If there is no further
		///   input then the invoker can proceed to the next step of the
		///   <a href="#steps">encoding operation</a>.  Otherwise this method
		///   should be invoked again with further input.  </para></li>
		/// 
		///   <li><para> <seealso cref="CoderResult#OVERFLOW"/> indicates that there is
		///   insufficient space in the output buffer to encode any more characters.
		///   This method should be invoked again with an output buffer that has
		///   more <seealso cref="Buffer#remaining remaining"/> bytes. This is
		///   typically done by draining any encoded bytes from the output
		///   buffer.  </para></li>
		/// 
		///   <li><para> A {@link CoderResult#malformedForLength
		///   malformed-input} result indicates that a malformed-input
		///   error has been detected.  The malformed characters begin at the input
		///   buffer's (possibly incremented) position; the number of malformed
		///   characters may be determined by invoking the result object's {@link
		///   CoderResult#length() length} method.  This case applies only if the
		///   <seealso cref="#onMalformedInput malformed action"/> of this encoder
		///   is <seealso cref="CodingErrorAction#REPORT"/>; otherwise the malformed input
		///   will be ignored or replaced, as requested.  </para></li>
		/// 
		///   <li><para> An {@link CoderResult#unmappableForLength
		///   unmappable-character} result indicates that an
		///   unmappable-character error has been detected.  The characters that
		///   encode the unmappable character begin at the input buffer's (possibly
		///   incremented) position; the number of such characters may be determined
		///   by invoking the result object's <seealso cref="CoderResult#length() length"/>
		///   method.  This case applies only if the {@link #onUnmappableCharacter
		///   unmappable action} of this encoder is {@link
		///   CodingErrorAction#REPORT}; otherwise the unmappable character will be
		///   ignored or replaced, as requested.  </para></li>
		/// 
		/// </ul>
		/// 
		/// In any case, if this method is to be reinvoked in the same encoding
		/// operation then care should be taken to preserve any characters remaining
		/// in the input buffer so that they are available to the next invocation.
		/// 
		/// <para> The <tt>endOfInput</tt> parameter advises this method as to whether
		/// the invoker can provide further input beyond that contained in the given
		/// input buffer.  If there is a possibility of providing additional input
		/// then the invoker should pass <tt>false</tt> for this parameter; if there
		/// is no possibility of providing further input then the invoker should
		/// pass <tt>true</tt>.  It is not erroneous, and in fact it is quite
		/// common, to pass <tt>false</tt> in one invocation and later discover that
		/// no further input was actually available.  It is critical, however, that
		/// the final invocation of this method in a sequence of invocations always
		/// pass <tt>true</tt> so that any remaining unencoded input will be treated
		/// as being malformed.
		/// 
		/// </para>
		/// <para> This method works by invoking the <seealso cref="#encodeLoop encodeLoop"/>
		/// method, interpreting its results, handling error conditions, and
		/// reinvoking it as necessary.  </para>
		/// 
		/// </summary>
		/// <param name="in">
		///         The input character buffer
		/// </param>
		/// <param name="out">
		///         The output byte buffer
		/// </param>
		/// <param name="endOfInput">
		///         <tt>true</tt> if, and only if, the invoker can provide no
		///         additional input characters beyond those in the given buffer
		/// </param>
		/// <returns>  A coder-result object describing the reason for termination
		/// </returns>
		/// <exception cref="IllegalStateException">
		///          If an encoding operation is already in progress and the previous
		///          step was an invocation neither of the <seealso cref="#reset reset"/>
		///          method, nor of this method with a value of <tt>false</tt> for
		///          the <tt>endOfInput</tt> parameter, nor of this method with a
		///          value of <tt>true</tt> for the <tt>endOfInput</tt> parameter
		///          but a return value indicating an incomplete encoding operation
		/// </exception>
		/// <exception cref="CoderMalfunctionError">
		///          If an invocation of the encodeLoop method threw
		///          an unexpected exception </exception>
		public CoderResult Encode(CharBuffer @in, ByteBuffer @out, bool endOfInput)
		{
			int newState = endOfInput ? ST_END : ST_CODING;
			if ((State != ST_RESET) && (State != ST_CODING) && !(endOfInput && (State == ST_END)))
			{
				ThrowIllegalStateException(State, newState);
			}
			State = newState;

			for (;;)
			{

				CoderResult cr;
				try
				{
					cr = EncodeLoop(@in, @out);
				}
				catch (BufferUnderflowException x)
				{
					throw new CoderMalfunctionError(x);
				}
				catch (BufferOverflowException x)
				{
					throw new CoderMalfunctionError(x);
				}

				if (cr.Overflow)
				{
					return cr;
				}

				if (cr.Underflow)
				{
					if (endOfInput && @in.HasRemaining())
					{
						cr = CoderResult.MalformedForLength(@in.Remaining());
						// Fall through to malformed-input case
					}
					else
					{
						return cr;
					}
				}

				CodingErrorAction action = null;
				if (cr.Malformed)
				{
					action = MalformedInputAction_Renamed;
				}
				else if (cr.Unmappable)
				{
					action = UnmappableCharacterAction_Renamed;
				}
				else
				{
					Debug.Assert(false, cr.ToString());
				}

				if (action == CodingErrorAction.REPORT)
				{
					return cr;
				}

				if (action == CodingErrorAction.REPLACE)
				{
					if (@out.Remaining() < Replacement_Renamed.Length)
					{
						return CoderResult.OVERFLOW;
					}
					@out.Put(Replacement_Renamed);
				}

				if ((action == CodingErrorAction.IGNORE) || (action == CodingErrorAction.REPLACE))
				{
					// Skip erroneous input either way
					@in.Position(@in.Position() + cr.Length());
					continue;
				}

				Debug.Assert(false);
			}

		}

		/// <summary>
		/// Flushes this encoder.
		/// 
		/// <para> Some encoders maintain internal state and may need to write some
		/// final bytes to the output buffer once the overall input sequence has
		/// been read.
		/// 
		/// </para>
		/// <para> Any additional output is written to the output buffer beginning at
		/// its current position.  At most <seealso cref="Buffer#remaining out.remaining()"/>
		/// bytes will be written.  The buffer's position will be advanced
		/// appropriately, but its mark and limit will not be modified.
		/// 
		/// </para>
		/// <para> If this method completes successfully then it returns {@link
		/// CoderResult#UNDERFLOW}.  If there is insufficient room in the output
		/// buffer then it returns <seealso cref="CoderResult#OVERFLOW"/>.  If this happens
		/// then this method must be invoked again, with an output buffer that has
		/// more room, in order to complete the current <a href="#steps">encoding
		/// operation</a>.
		/// 
		/// </para>
		/// <para> If this encoder has already been flushed then invoking this method
		/// has no effect.
		/// 
		/// </para>
		/// <para> This method invokes the <seealso cref="#implFlush implFlush"/> method to
		/// perform the actual flushing operation.  </para>
		/// </summary>
		/// <param name="out">
		///         The output byte buffer
		/// </param>
		/// <returns>  A coder-result object, either <seealso cref="CoderResult#UNDERFLOW"/> or
		///          <seealso cref="CoderResult#OVERFLOW"/>
		/// </returns>
		/// <exception cref="IllegalStateException">
		///          If the previous step of the current encoding operation was an
		///          invocation neither of the <seealso cref="#flush flush"/> method nor of
		///          the three-argument {@link
		///          #encode(CharBuffer,ByteBuffer,boolean) encode} method
		///          with a value of <tt>true</tt> for the <tt>endOfInput</tt>
		///          parameter </exception>
		public CoderResult Flush(ByteBuffer @out)
		{
			if (State == ST_END)
			{
				CoderResult cr = ImplFlush(@out);
				if (cr.Underflow)
				{
					State = ST_FLUSHED;
				}
				return cr;
			}

			if (State != ST_FLUSHED)
			{
				ThrowIllegalStateException(State, ST_FLUSHED);
			}

			return CoderResult.UNDERFLOW; // Already flushed
		}

		/// <summary>
		/// Flushes this encoder.
		/// 
		/// <para> The default implementation of this method does nothing, and always
		/// returns <seealso cref="CoderResult#UNDERFLOW"/>.  This method should be overridden
		/// by encoders that may need to write final bytes to the output buffer
		/// once the entire input sequence has been read. </para>
		/// </summary>
		/// <param name="out">
		///         The output byte buffer
		/// </param>
		/// <returns>  A coder-result object, either <seealso cref="CoderResult#UNDERFLOW"/> or
		///          <seealso cref="CoderResult#OVERFLOW"/> </returns>
		protected internal virtual CoderResult ImplFlush(ByteBuffer @out)
		{
			return CoderResult.UNDERFLOW;
		}

		/// <summary>
		/// Resets this encoder, clearing any internal state.
		/// 
		/// <para> This method resets charset-independent state and also invokes the
		/// <seealso cref="#implReset() implReset"/> method in order to perform any
		/// charset-specific reset actions.  </para>
		/// </summary>
		/// <returns>  This encoder
		///  </returns>
		public CharsetEncoder Reset()
		{
			ImplReset();
			State = ST_RESET;
			return this;
		}

		/// <summary>
		/// Resets this encoder, clearing any charset-specific internal state.
		/// 
		/// <para> The default implementation of this method does nothing.  This method
		/// should be overridden by encoders that maintain internal state.  </para>
		/// </summary>
		protected internal virtual void ImplReset()
		{
		}

		/// <summary>
		/// Encodes one or more characters into one or more bytes.
		/// 
		/// <para> This method encapsulates the basic encoding loop, encoding as many
		/// characters as possible until it either runs out of input, runs out of room
		/// in the output buffer, or encounters an encoding error.  This method is
		/// invoked by the <seealso cref="#encode encode"/> method, which handles result
		/// interpretation and error recovery.
		/// 
		/// </para>
		/// <para> The buffers are read from, and written to, starting at their current
		/// positions.  At most <seealso cref="Buffer#remaining in.remaining()"/> characters
		/// will be read, and at most <seealso cref="Buffer#remaining out.remaining()"/>
		/// bytes will be written.  The buffers' positions will be advanced to
		/// reflect the characters read and the bytes written, but their marks and
		/// limits will not be modified.
		/// 
		/// </para>
		/// <para> This method returns a <seealso cref="CoderResult"/> object to describe its
		/// reason for termination, in the same manner as the <seealso cref="#encode encode"/>
		/// method.  Most implementations of this method will handle encoding errors
		/// by returning an appropriate result object for interpretation by the
		/// <seealso cref="#encode encode"/> method.  An optimized implementation may instead
		/// examine the relevant error action and implement that action itself.
		/// 
		/// </para>
		/// <para> An implementation of this method may perform arbitrary lookahead by
		/// returning <seealso cref="CoderResult#UNDERFLOW"/> until it receives sufficient
		/// input.  </para>
		/// </summary>
		/// <param name="in">
		///         The input character buffer
		/// </param>
		/// <param name="out">
		///         The output byte buffer
		/// </param>
		/// <returns>  A coder-result object describing the reason for termination </returns>
		protected internal abstract CoderResult EncodeLoop(CharBuffer @in, ByteBuffer @out);

		/// <summary>
		/// Convenience method that encodes the remaining content of a single input
		/// character buffer into a newly-allocated byte buffer.
		/// 
		/// <para> This method implements an entire <a href="#steps">encoding
		/// operation</a>; that is, it resets this encoder, then it encodes the
		/// characters in the given character buffer, and finally it flushes this
		/// encoder.  This method should therefore not be invoked if an encoding
		/// operation is already in progress.  </para>
		/// </summary>
		/// <param name="in">
		///         The input character buffer
		/// </param>
		/// <returns> A newly-allocated byte buffer containing the result of the
		///         encoding operation.  The buffer's position will be zero and its
		///         limit will follow the last byte written.
		/// </returns>
		/// <exception cref="IllegalStateException">
		///          If an encoding operation is already in progress
		/// </exception>
		/// <exception cref="MalformedInputException">
		///          If the character sequence starting at the input buffer's current
		///          position is not a legal sixteen-bit Unicode sequence and the current malformed-input action
		///          is <seealso cref="CodingErrorAction#REPORT"/>
		/// </exception>
		/// <exception cref="UnmappableCharacterException">
		///          If the character sequence starting at the input buffer's current
		///          position cannot be mapped to an equivalent byte sequence and
		///          the current unmappable-character action is {@link
		///          CodingErrorAction#REPORT} </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public final java.nio.ByteBuffer encode(java.nio.CharBuffer in) throws CharacterCodingException
		public ByteBuffer Encode(CharBuffer @in)
		{
			int n = (int)(@in.Remaining() * AverageBytesPerChar());
			ByteBuffer @out = ByteBuffer.Allocate(n);

			if ((n == 0) && (@in.Remaining() == 0))
			{
				return @out;
			}
			Reset();
			for (;;)
			{
				CoderResult cr = @in.HasRemaining() ? Encode(@in, @out, true) : CoderResult.UNDERFLOW;
				if (cr.Underflow)
				{
					cr = Flush(@out);
				}

				if (cr.Underflow)
				{
					break;
				}
				if (cr.Overflow)
				{
					n = 2 * n + 1; // Ensure progress; n might be 0!
					ByteBuffer o = ByteBuffer.Allocate(n);
					@out.Flip();
					o.Put(@out);
					@out = o;
					continue;
				}
				cr.ThrowException();
			}
			@out.Flip();
			return @out;
		}















































































		private bool CanEncode(CharBuffer cb)
		{
			if (State == ST_FLUSHED)
			{
				Reset();
			}
			else if (State != ST_RESET)
			{
				ThrowIllegalStateException(State, ST_CODING);
			}
			CodingErrorAction ma = MalformedInputAction();
			CodingErrorAction ua = UnmappableCharacterAction();
			try
			{
				OnMalformedInput(CodingErrorAction.REPORT);
				OnUnmappableCharacter(CodingErrorAction.REPORT);
				Encode(cb);
			}
			catch (CharacterCodingException)
			{
				return false;
			}
			finally
			{
				OnMalformedInput(ma);
				OnUnmappableCharacter(ua);
				Reset();
			}
			return true;
		}

		/// <summary>
		/// Tells whether or not this encoder can encode the given character.
		/// 
		/// <para> This method returns <tt>false</tt> if the given character is a
		/// surrogate character; such characters can be interpreted only when they
		/// are members of a pair consisting of a high surrogate followed by a low
		/// surrogate.  The {@link #canEncode(java.lang.CharSequence)
		/// canEncode(CharSequence)} method may be used to test whether or not a
		/// character sequence can be encoded.
		/// 
		/// </para>
		/// <para> This method may modify this encoder's state; it should therefore not
		/// be invoked if an <a href="#steps">encoding operation</a> is already in
		/// progress.
		/// 
		/// </para>
		/// <para> The default implementation of this method is not very efficient; it
		/// should generally be overridden to improve performance.  </para>
		/// </summary>
		/// <param name="c">
		///          The given character
		/// </param>
		/// <returns>  <tt>true</tt> if, and only if, this encoder can encode
		///          the given character
		/// </returns>
		/// <exception cref="IllegalStateException">
		///          If an encoding operation is already in progress </exception>
		public virtual bool CanEncode(char c)
		{
			CharBuffer cb = CharBuffer.Allocate(1);
			cb.Put(c);
			cb.Flip();
			return CanEncode(cb);
		}

		/// <summary>
		/// Tells whether or not this encoder can encode the given character
		/// sequence.
		/// 
		/// <para> If this method returns <tt>false</tt> for a particular character
		/// sequence then more information about why the sequence cannot be encoded
		/// may be obtained by performing a full <a href="#steps">encoding
		/// operation</a>.
		/// 
		/// </para>
		/// <para> This method may modify this encoder's state; it should therefore not
		/// be invoked if an encoding operation is already in progress.
		/// 
		/// </para>
		/// <para> The default implementation of this method is not very efficient; it
		/// should generally be overridden to improve performance.  </para>
		/// </summary>
		/// <param name="cs">
		///          The given character sequence
		/// </param>
		/// <returns>  <tt>true</tt> if, and only if, this encoder can encode
		///          the given character without throwing any exceptions and without
		///          performing any replacements
		/// </returns>
		/// <exception cref="IllegalStateException">
		///          If an encoding operation is already in progress </exception>
		public virtual bool CanEncode(CharSequence cs)
		{
			CharBuffer cb;
			if (cs is CharBuffer)
			{
				cb = ((CharBuffer)cs).Duplicate();
			}
			else
			{
				cb = CharBuffer.Wrap(cs.ToString());
			}
			return CanEncode(cb);
		}




		private void ThrowIllegalStateException(int from, int to)
		{
			throw new IllegalStateException("Current state = " + StateNames[from] + ", new state = " + StateNames[to]);
		}

	}

}
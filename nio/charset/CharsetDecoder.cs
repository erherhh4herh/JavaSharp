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
	/// An engine that can transform a sequence of bytes in a specific charset into a sequence of
	/// sixteen-bit Unicode characters.
	/// 
	/// <a name="steps"></a>
	/// 
	/// <para> The input byte sequence is provided in a byte buffer or a series
	/// of such buffers.  The output character sequence is written to a character buffer
	/// or a series of such buffers.  A decoder should always be used by making
	/// the following sequence of method invocations, hereinafter referred to as a
	/// <i>decoding operation</i>:
	/// 
	/// <ol>
	/// 
	/// </para>
	///   <li><para> Reset the decoder via the <seealso cref="#reset reset"/> method, unless it
	///   has not been used before; </para></li>
	/// 
	///   <li><para> Invoke the <seealso cref="#decode decode"/> method zero or more times, as
	///   long as additional input may be available, passing <tt>false</tt> for the
	///   <tt>endOfInput</tt> argument and filling the input buffer and flushing the
	///   output buffer between invocations; </para></li>
	/// 
	///   <li><para> Invoke the <seealso cref="#decode decode"/> method one final time, passing
	///   <tt>true</tt> for the <tt>endOfInput</tt> argument; and then </para></li>
	/// 
	///   <li><para> Invoke the <seealso cref="#flush flush"/> method so that the decoder can
	///   flush any internal state to the output buffer. </para></li>
	/// 
	/// </ol>
	/// 
	/// Each invocation of the <seealso cref="#decode decode"/> method will decode as many
	/// bytes as possible from the input buffer, writing the resulting characters
	/// to the output buffer.  The <seealso cref="#decode decode"/> method returns when more
	/// input is required, when there is not enough room in the output buffer, or
	/// when a decoding error has occurred.  In each case a <seealso cref="CoderResult"/>
	/// object is returned to describe the reason for termination.  An invoker can
	/// examine this object and fill the input buffer, flush the output buffer, or
	/// attempt to recover from a decoding error, as appropriate, and try again.
	/// 
	/// <a name="ce"></a>
	/// 
	/// <para> There are two general types of decoding errors.  If the input byte
	/// sequence is not legal for this charset then the input is considered <i>malformed</i>.  If
	/// the input byte sequence is legal but cannot be mapped to a valid
	/// Unicode character then an <i>unmappable character</i> has been encountered.
	/// 
	/// <a name="cae"></a>
	/// 
	/// </para>
	/// <para> How a decoding error is handled depends upon the action requested for
	/// that type of error, which is described by an instance of the {@link
	/// CodingErrorAction} class.  The possible error actions are to {@linkplain
	/// CodingErrorAction#IGNORE ignore} the erroneous input, {@linkplain
	/// CodingErrorAction#REPORT report} the error to the invoker via
	/// the returned <seealso cref="CoderResult"/> object, or {@link CodingErrorAction#REPLACE
	/// replace} the erroneous input with the current value of the
	/// replacement string.  The replacement
	/// 
	/// 
	/// 
	/// 
	/// 
	/// 
	/// has the initial value <tt>"&#92;uFFFD"</tt>;
	/// 
	/// 
	/// its value may be changed via the {@link #replaceWith(java.lang.String)
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
	/// <para> This class is designed to handle many of the details of the decoding
	/// process, including the implementation of error actions.  A decoder for a
	/// specific charset, which is a concrete subclass of this class, need only
	/// implement the abstract <seealso cref="#decodeLoop decodeLoop"/> method, which
	/// encapsulates the basic decoding loop.  A subclass that maintains internal
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
	/// <seealso cref= CharsetEncoder </seealso>

	public abstract class CharsetDecoder
	{

		private readonly Charset Charset_Renamed;
		private readonly float AverageCharsPerByte_Renamed;
		private readonly float MaxCharsPerByte_Renamed;

		private String Replacement_Renamed;
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
		/// Initializes a new decoder.  The new decoder will have the given
		/// chars-per-byte and replacement values.
		/// </summary>
		/// <param name="cs">
		///         The charset that created this decoder
		/// </param>
		/// <param name="averageCharsPerByte">
		///         A positive float value indicating the expected number of
		///         characters that will be produced for each input byte
		/// </param>
		/// <param name="maxCharsPerByte">
		///         A positive float value indicating the maximum number of
		///         characters that will be produced for each input byte
		/// </param>
		/// <param name="replacement">
		///         The initial replacement; must not be <tt>null</tt>, must have
		///         non-zero length, must not be longer than maxCharsPerByte,
		///         and must be <seealso cref="#isLegalReplacement legal"/>
		/// </param>
		/// <exception cref="IllegalArgumentException">
		///          If the preconditions on the parameters do not hold </exception>
		private CharsetDecoder(Charset cs, float averageCharsPerByte, float maxCharsPerByte, String replacement)
		{
			this.Charset_Renamed = cs;
			if (averageCharsPerByte <= 0.0f)
			{
				throw new IllegalArgumentException("Non-positive " + "averageCharsPerByte");
			}
			if (maxCharsPerByte <= 0.0f)
			{
				throw new IllegalArgumentException("Non-positive " + "maxCharsPerByte");
			}
			if (!Charset.AtBugLevel("1.4"))
			{
				if (averageCharsPerByte > maxCharsPerByte)
				{
					throw new IllegalArgumentException("averageCharsPerByte" + " exceeds " + "maxCharsPerByte");
				}
			}
			this.Replacement_Renamed = replacement;
			this.AverageCharsPerByte_Renamed = averageCharsPerByte;
			this.MaxCharsPerByte_Renamed = maxCharsPerByte;
			ReplaceWith(replacement);
		}

		/// <summary>
		/// Initializes a new decoder.  The new decoder will have the given
		/// chars-per-byte values and its replacement will be the
		/// string <tt>"&#92;uFFFD"</tt>.
		/// </summary>
		/// <param name="cs">
		///         The charset that created this decoder
		/// </param>
		/// <param name="averageCharsPerByte">
		///         A positive float value indicating the expected number of
		///         characters that will be produced for each input byte
		/// </param>
		/// <param name="maxCharsPerByte">
		///         A positive float value indicating the maximum number of
		///         characters that will be produced for each input byte
		/// </param>
		/// <exception cref="IllegalArgumentException">
		///          If the preconditions on the parameters do not hold </exception>
		protected internal CharsetDecoder(Charset cs, float averageCharsPerByte, float maxCharsPerByte) : this(cs, averageCharsPerByte, maxCharsPerByte, "\uFFFD")
		{
		}

		/// <summary>
		/// Returns the charset that created this decoder.
		/// </summary>
		/// <returns>  This decoder's charset </returns>
		public Charset Charset()
		{
			return Charset_Renamed;
		}

		/// <summary>
		/// Returns this decoder's replacement value.
		/// </summary>
		/// <returns>  This decoder's current replacement,
		///          which is never <tt>null</tt> and is never empty </returns>
		public String Replacement()
		{

			return Replacement_Renamed;




		}

		/// <summary>
		/// Changes this decoder's replacement value.
		/// 
		/// <para> This method invokes the <seealso cref="#implReplaceWith implReplaceWith"/>
		/// method, passing the new replacement, after checking that the new
		/// replacement is acceptable.  </para>
		/// </summary>
		/// <param name="newReplacement">  The replacement value
		/// 
		/// 
		///         The new replacement; must not be <tt>null</tt>
		///         and must have non-zero length
		/// 
		/// 
		/// 
		/// 
		/// 
		/// 
		/// 
		/// </param>
		/// <returns>  This decoder
		/// </returns>
		/// <exception cref="IllegalArgumentException">
		///          If the preconditions on the parameter do not hold </exception>
		public CharsetDecoder ReplaceWith(String newReplacement)
		{
			if (newReplacement == null)
			{
				throw new IllegalArgumentException("Null replacement");
			}
			int len = newReplacement.Length();
			if (len == 0)
			{
				throw new IllegalArgumentException("Empty replacement");
			}
			if (len > MaxCharsPerByte_Renamed)
			{
				throw new IllegalArgumentException("Replacement too long");
			}

			this.Replacement_Renamed = newReplacement;






			ImplReplaceWith(this.Replacement_Renamed);
			return this;
		}

		/// <summary>
		/// Reports a change to this decoder's replacement value.
		/// 
		/// <para> The default implementation of this method does nothing.  This method
		/// should be overridden by decoders that require notification of changes to
		/// the replacement.  </para>
		/// </summary>
		/// <param name="newReplacement">    The replacement value </param>
		protected internal virtual void ImplReplaceWith(String newReplacement)
		{
		}









































		/// <summary>
		/// Returns this decoder's current action for malformed-input errors.
		/// </summary>
		/// <returns> The current malformed-input action, which is never <tt>null</tt> </returns>
		public virtual CodingErrorAction MalformedInputAction()
		{
			return MalformedInputAction_Renamed;
		}

		/// <summary>
		/// Changes this decoder's action for malformed-input errors.
		/// 
		/// <para> This method invokes the {@link #implOnMalformedInput
		/// implOnMalformedInput} method, passing the new action.  </para>
		/// </summary>
		/// <param name="newAction">  The new action; must not be <tt>null</tt>
		/// </param>
		/// <returns>  This decoder
		/// </returns>
		/// <exception cref="IllegalArgumentException">
		///         If the precondition on the parameter does not hold </exception>
		public CharsetDecoder OnMalformedInput(CodingErrorAction newAction)
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
		/// Reports a change to this decoder's malformed-input action.
		/// 
		/// <para> The default implementation of this method does nothing.  This method
		/// should be overridden by decoders that require notification of changes to
		/// the malformed-input action.  </para>
		/// </summary>
		/// <param name="newAction">  The new action </param>
		protected internal virtual void ImplOnMalformedInput(CodingErrorAction newAction)
		{
		}

		/// <summary>
		/// Returns this decoder's current action for unmappable-character errors.
		/// </summary>
		/// <returns> The current unmappable-character action, which is never
		///         <tt>null</tt> </returns>
		public virtual CodingErrorAction UnmappableCharacterAction()
		{
			return UnmappableCharacterAction_Renamed;
		}

		/// <summary>
		/// Changes this decoder's action for unmappable-character errors.
		/// 
		/// <para> This method invokes the {@link #implOnUnmappableCharacter
		/// implOnUnmappableCharacter} method, passing the new action.  </para>
		/// </summary>
		/// <param name="newAction">  The new action; must not be <tt>null</tt>
		/// </param>
		/// <returns>  This decoder
		/// </returns>
		/// <exception cref="IllegalArgumentException">
		///         If the precondition on the parameter does not hold </exception>
		public CharsetDecoder OnUnmappableCharacter(CodingErrorAction newAction)
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
		/// Reports a change to this decoder's unmappable-character action.
		/// 
		/// <para> The default implementation of this method does nothing.  This method
		/// should be overridden by decoders that require notification of changes to
		/// the unmappable-character action.  </para>
		/// </summary>
		/// <param name="newAction">  The new action </param>
		protected internal virtual void ImplOnUnmappableCharacter(CodingErrorAction newAction)
		{
		}

		/// <summary>
		/// Returns the average number of characters that will be produced for each
		/// byte of input.  This heuristic value may be used to estimate the size
		/// of the output buffer required for a given input sequence.
		/// </summary>
		/// <returns>  The average number of characters produced
		///          per byte of input </returns>
		public float AverageCharsPerByte()
		{
			return AverageCharsPerByte_Renamed;
		}

		/// <summary>
		/// Returns the maximum number of characters that will be produced for each
		/// byte of input.  This value may be used to compute the worst-case size
		/// of the output buffer required for a given input sequence.
		/// </summary>
		/// <returns>  The maximum number of characters that will be produced per
		///          byte of input </returns>
		public float MaxCharsPerByte()
		{
			return MaxCharsPerByte_Renamed;
		}

		/// <summary>
		/// Decodes as many bytes as possible from the given input buffer,
		/// writing the results to the given output buffer.
		/// 
		/// <para> The buffers are read from, and written to, starting at their current
		/// positions.  At most <seealso cref="Buffer#remaining in.remaining()"/> bytes
		/// will be read and at most <seealso cref="Buffer#remaining out.remaining()"/>
		/// characters will be written.  The buffers' positions will be advanced to
		/// reflect the bytes read and the characters written, but their marks and
		/// limits will not be modified.
		/// 
		/// </para>
		/// <para> In addition to reading bytes from the input buffer and writing
		/// characters to the output buffer, this method returns a <seealso cref="CoderResult"/>
		/// object to describe its reason for termination:
		/// 
		/// <ul>
		/// 
		/// </para>
		///   <li><para> <seealso cref="CoderResult#UNDERFLOW"/> indicates that as much of the
		///   input buffer as possible has been decoded.  If there is no further
		///   input then the invoker can proceed to the next step of the
		///   <a href="#steps">decoding operation</a>.  Otherwise this method
		///   should be invoked again with further input.  </para></li>
		/// 
		///   <li><para> <seealso cref="CoderResult#OVERFLOW"/> indicates that there is
		///   insufficient space in the output buffer to decode any more bytes.
		///   This method should be invoked again with an output buffer that has
		///   more <seealso cref="Buffer#remaining remaining"/> characters. This is
		///   typically done by draining any decoded characters from the output
		///   buffer.  </para></li>
		/// 
		///   <li><para> A {@link CoderResult#malformedForLength
		///   malformed-input} result indicates that a malformed-input
		///   error has been detected.  The malformed bytes begin at the input
		///   buffer's (possibly incremented) position; the number of malformed
		///   bytes may be determined by invoking the result object's {@link
		///   CoderResult#length() length} method.  This case applies only if the
		///   <seealso cref="#onMalformedInput malformed action"/> of this decoder
		///   is <seealso cref="CodingErrorAction#REPORT"/>; otherwise the malformed input
		///   will be ignored or replaced, as requested.  </para></li>
		/// 
		///   <li><para> An {@link CoderResult#unmappableForLength
		///   unmappable-character} result indicates that an
		///   unmappable-character error has been detected.  The bytes that
		///   decode the unmappable character begin at the input buffer's (possibly
		///   incremented) position; the number of such bytes may be determined
		///   by invoking the result object's <seealso cref="CoderResult#length() length"/>
		///   method.  This case applies only if the {@link #onUnmappableCharacter
		///   unmappable action} of this decoder is {@link
		///   CodingErrorAction#REPORT}; otherwise the unmappable character will be
		///   ignored or replaced, as requested.  </para></li>
		/// 
		/// </ul>
		/// 
		/// In any case, if this method is to be reinvoked in the same decoding
		/// operation then care should be taken to preserve any bytes remaining
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
		/// pass <tt>true</tt> so that any remaining undecoded input will be treated
		/// as being malformed.
		/// 
		/// </para>
		/// <para> This method works by invoking the <seealso cref="#decodeLoop decodeLoop"/>
		/// method, interpreting its results, handling error conditions, and
		/// reinvoking it as necessary.  </para>
		/// 
		/// </summary>
		/// <param name="in">
		///         The input byte buffer
		/// </param>
		/// <param name="out">
		///         The output character buffer
		/// </param>
		/// <param name="endOfInput">
		///         <tt>true</tt> if, and only if, the invoker can provide no
		///         additional input bytes beyond those in the given buffer
		/// </param>
		/// <returns>  A coder-result object describing the reason for termination
		/// </returns>
		/// <exception cref="IllegalStateException">
		///          If a decoding operation is already in progress and the previous
		///          step was an invocation neither of the <seealso cref="#reset reset"/>
		///          method, nor of this method with a value of <tt>false</tt> for
		///          the <tt>endOfInput</tt> parameter, nor of this method with a
		///          value of <tt>true</tt> for the <tt>endOfInput</tt> parameter
		///          but a return value indicating an incomplete decoding operation
		/// </exception>
		/// <exception cref="CoderMalfunctionError">
		///          If an invocation of the decodeLoop method threw
		///          an unexpected exception </exception>
		public CoderResult Decode(ByteBuffer @in, CharBuffer @out, bool endOfInput)
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
					cr = DecodeLoop(@in, @out);
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
					if (@out.Remaining() < Replacement_Renamed.Length())
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
		/// Flushes this decoder.
		/// 
		/// <para> Some decoders maintain internal state and may need to write some
		/// final characters to the output buffer once the overall input sequence has
		/// been read.
		/// 
		/// </para>
		/// <para> Any additional output is written to the output buffer beginning at
		/// its current position.  At most <seealso cref="Buffer#remaining out.remaining()"/>
		/// characters will be written.  The buffer's position will be advanced
		/// appropriately, but its mark and limit will not be modified.
		/// 
		/// </para>
		/// <para> If this method completes successfully then it returns {@link
		/// CoderResult#UNDERFLOW}.  If there is insufficient room in the output
		/// buffer then it returns <seealso cref="CoderResult#OVERFLOW"/>.  If this happens
		/// then this method must be invoked again, with an output buffer that has
		/// more room, in order to complete the current <a href="#steps">decoding
		/// operation</a>.
		/// 
		/// </para>
		/// <para> If this decoder has already been flushed then invoking this method
		/// has no effect.
		/// 
		/// </para>
		/// <para> This method invokes the <seealso cref="#implFlush implFlush"/> method to
		/// perform the actual flushing operation.  </para>
		/// </summary>
		/// <param name="out">
		///         The output character buffer
		/// </param>
		/// <returns>  A coder-result object, either <seealso cref="CoderResult#UNDERFLOW"/> or
		///          <seealso cref="CoderResult#OVERFLOW"/>
		/// </returns>
		/// <exception cref="IllegalStateException">
		///          If the previous step of the current decoding operation was an
		///          invocation neither of the <seealso cref="#flush flush"/> method nor of
		///          the three-argument {@link
		///          #decode(ByteBuffer,CharBuffer,boolean) decode} method
		///          with a value of <tt>true</tt> for the <tt>endOfInput</tt>
		///          parameter </exception>
		public CoderResult Flush(CharBuffer @out)
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
		/// Flushes this decoder.
		/// 
		/// <para> The default implementation of this method does nothing, and always
		/// returns <seealso cref="CoderResult#UNDERFLOW"/>.  This method should be overridden
		/// by decoders that may need to write final characters to the output buffer
		/// once the entire input sequence has been read. </para>
		/// </summary>
		/// <param name="out">
		///         The output character buffer
		/// </param>
		/// <returns>  A coder-result object, either <seealso cref="CoderResult#UNDERFLOW"/> or
		///          <seealso cref="CoderResult#OVERFLOW"/> </returns>
		protected internal virtual CoderResult ImplFlush(CharBuffer @out)
		{
			return CoderResult.UNDERFLOW;
		}

		/// <summary>
		/// Resets this decoder, clearing any internal state.
		/// 
		/// <para> This method resets charset-independent state and also invokes the
		/// <seealso cref="#implReset() implReset"/> method in order to perform any
		/// charset-specific reset actions.  </para>
		/// </summary>
		/// <returns>  This decoder
		///  </returns>
		public CharsetDecoder Reset()
		{
			ImplReset();
			State = ST_RESET;
			return this;
		}

		/// <summary>
		/// Resets this decoder, clearing any charset-specific internal state.
		/// 
		/// <para> The default implementation of this method does nothing.  This method
		/// should be overridden by decoders that maintain internal state.  </para>
		/// </summary>
		protected internal virtual void ImplReset()
		{
		}

		/// <summary>
		/// Decodes one or more bytes into one or more characters.
		/// 
		/// <para> This method encapsulates the basic decoding loop, decoding as many
		/// bytes as possible until it either runs out of input, runs out of room
		/// in the output buffer, or encounters a decoding error.  This method is
		/// invoked by the <seealso cref="#decode decode"/> method, which handles result
		/// interpretation and error recovery.
		/// 
		/// </para>
		/// <para> The buffers are read from, and written to, starting at their current
		/// positions.  At most <seealso cref="Buffer#remaining in.remaining()"/> bytes
		/// will be read, and at most <seealso cref="Buffer#remaining out.remaining()"/>
		/// characters will be written.  The buffers' positions will be advanced to
		/// reflect the bytes read and the characters written, but their marks and
		/// limits will not be modified.
		/// 
		/// </para>
		/// <para> This method returns a <seealso cref="CoderResult"/> object to describe its
		/// reason for termination, in the same manner as the <seealso cref="#decode decode"/>
		/// method.  Most implementations of this method will handle decoding errors
		/// by returning an appropriate result object for interpretation by the
		/// <seealso cref="#decode decode"/> method.  An optimized implementation may instead
		/// examine the relevant error action and implement that action itself.
		/// 
		/// </para>
		/// <para> An implementation of this method may perform arbitrary lookahead by
		/// returning <seealso cref="CoderResult#UNDERFLOW"/> until it receives sufficient
		/// input.  </para>
		/// </summary>
		/// <param name="in">
		///         The input byte buffer
		/// </param>
		/// <param name="out">
		///         The output character buffer
		/// </param>
		/// <returns>  A coder-result object describing the reason for termination </returns>
		protected internal abstract CoderResult DecodeLoop(ByteBuffer @in, CharBuffer @out);

		/// <summary>
		/// Convenience method that decodes the remaining content of a single input
		/// byte buffer into a newly-allocated character buffer.
		/// 
		/// <para> This method implements an entire <a href="#steps">decoding
		/// operation</a>; that is, it resets this decoder, then it decodes the
		/// bytes in the given byte buffer, and finally it flushes this
		/// decoder.  This method should therefore not be invoked if a decoding
		/// operation is already in progress.  </para>
		/// </summary>
		/// <param name="in">
		///         The input byte buffer
		/// </param>
		/// <returns> A newly-allocated character buffer containing the result of the
		///         decoding operation.  The buffer's position will be zero and its
		///         limit will follow the last character written.
		/// </returns>
		/// <exception cref="IllegalStateException">
		///          If a decoding operation is already in progress
		/// </exception>
		/// <exception cref="MalformedInputException">
		///          If the byte sequence starting at the input buffer's current
		///          position is not legal for this charset and the current malformed-input action
		///          is <seealso cref="CodingErrorAction#REPORT"/>
		/// </exception>
		/// <exception cref="UnmappableCharacterException">
		///          If the byte sequence starting at the input buffer's current
		///          position cannot be mapped to an equivalent character sequence and
		///          the current unmappable-character action is {@link
		///          CodingErrorAction#REPORT} </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public final java.nio.CharBuffer decode(java.nio.ByteBuffer in) throws CharacterCodingException
		public CharBuffer Decode(ByteBuffer @in)
		{
			int n = (int)(@in.Remaining() * AverageCharsPerByte());
			CharBuffer @out = CharBuffer.Allocate(n);

			if ((n == 0) && (@in.Remaining() == 0))
			{
				return @out;
			}
			Reset();
			for (;;)
			{
				CoderResult cr = @in.HasRemaining() ? Decode(@in, @out, true) : CoderResult.UNDERFLOW;
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
					CharBuffer o = CharBuffer.Allocate(n);
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



		/// <summary>
		/// Tells whether or not this decoder implements an auto-detecting charset.
		/// 
		/// <para> The default implementation of this method always returns
		/// <tt>false</tt>; it should be overridden by auto-detecting decoders to
		/// return <tt>true</tt>.  </para>
		/// </summary>
		/// <returns>  <tt>true</tt> if, and only if, this decoder implements an
		///          auto-detecting charset </returns>
		public virtual bool AutoDetecting
		{
			get
			{
				return false;
			}
		}

		/// <summary>
		/// Tells whether or not this decoder has yet detected a
		/// charset&nbsp;&nbsp;<i>(optional operation)</i>.
		/// 
		/// <para> If this decoder implements an auto-detecting charset then at a
		/// single point during a decoding operation this method may start returning
		/// <tt>true</tt> to indicate that a specific charset has been detected in
		/// the input byte sequence.  Once this occurs, the {@link #detectedCharset
		/// detectedCharset} method may be invoked to retrieve the detected charset.
		/// 
		/// </para>
		/// <para> That this method returns <tt>false</tt> does not imply that no bytes
		/// have yet been decoded.  Some auto-detecting decoders are capable of
		/// decoding some, or even all, of an input byte sequence without fixing on
		/// a particular charset.
		/// 
		/// </para>
		/// <para> The default implementation of this method always throws an {@link
		/// UnsupportedOperationException}; it should be overridden by
		/// auto-detecting decoders to return <tt>true</tt> once the input charset
		/// has been determined.  </para>
		/// </summary>
		/// <returns>  <tt>true</tt> if, and only if, this decoder has detected a
		///          specific charset
		/// </returns>
		/// <exception cref="UnsupportedOperationException">
		///          If this decoder does not implement an auto-detecting charset </exception>
		public virtual bool CharsetDetected
		{
			get
			{
				throw new UnsupportedOperationException();
			}
		}

		/// <summary>
		/// Retrieves the charset that was detected by this
		/// decoder&nbsp;&nbsp;<i>(optional operation)</i>.
		/// 
		/// <para> If this decoder implements an auto-detecting charset then this
		/// method returns the actual charset once it has been detected.  After that
		/// point, this method returns the same value for the duration of the
		/// current decoding operation.  If not enough input bytes have yet been
		/// read to determine the actual charset then this method throws an {@link
		/// IllegalStateException}.
		/// 
		/// </para>
		/// <para> The default implementation of this method always throws an {@link
		/// UnsupportedOperationException}; it should be overridden by
		/// auto-detecting decoders to return the appropriate value.  </para>
		/// </summary>
		/// <returns>  The charset detected by this auto-detecting decoder,
		///          or <tt>null</tt> if the charset has not yet been determined
		/// </returns>
		/// <exception cref="IllegalStateException">
		///          If insufficient bytes have been read to determine a charset
		/// </exception>
		/// <exception cref="UnsupportedOperationException">
		///          If this decoder does not implement an auto-detecting charset </exception>
		public virtual Charset DetectedCharset()
		{
			throw new UnsupportedOperationException();
		}
































































































		private void ThrowIllegalStateException(int from, int to)
		{
			throw new IllegalStateException("Current state = " + StateNames[from] + ", new state = " + StateNames[to]);
		}

	}

}
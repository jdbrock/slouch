using System;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Runtime.InteropServices;

namespace Paperboy
{
    internal class UsenetStreamReader : TextReader
    {
        // ===========================================================================
        // = Private Constants
        // ===========================================================================
        
        private const Int32 BUFFER_SIZE = 1024;

        // ===========================================================================
        // = Private Fields
        // ===========================================================================
        
        private Byte[] _inputBuffer;
        private Char[] _decodedBuffer;
        private Int32 _decodedCount;
        private Int32 _currentDecodePosition;
        private Int32 _bufferSize;

        private Encoding _encoding;
        private Decoder _decoder;

        private Stream _baseStream;
        private Boolean _mayBlock;
        private StringBuilder _lineBuilder;

        private Boolean _foundCarriageReturn;

        // ===========================================================================
        // = Construction
        // ===========================================================================
        
        public UsenetStreamReader(Stream inStream, Encoding inEncoding)
        {
            _baseStream            = inStream;
            _bufferSize            = BUFFER_SIZE;
            _encoding              = inEncoding;
            _decoder               = inEncoding.GetDecoder();

            _inputBuffer           = new Byte[BUFFER_SIZE];
            _decodedBuffer         = new Char[inEncoding.GetMaxCharCount(BUFFER_SIZE)];

            _decodedCount          = 0;
            _currentDecodePosition = 0;
        }

        // ===========================================================================
        // = Overrides
        // ===========================================================================


        public override String ReadLine()
        {
            if (_currentDecodePosition >= _decodedCount && ReadBuffer() == 0)
                return null;

            var begin = _currentDecodePosition;
            var end = FindNextEndOfLineIndex();

            if (end < _decodedCount && end >= begin)
                return new String(_decodedBuffer, begin, end - begin);

            if (_lineBuilder == null)
                _lineBuilder = new StringBuilder();
            else
                _lineBuilder.Length = 0;

            while (true)
            {
                if (_foundCarriageReturn)
                    _decodedCount--;

                _lineBuilder.Append(_decodedBuffer, begin, _decodedCount - begin);

                if (ReadBuffer() == 0)
                {
                    if (_lineBuilder.Capacity > 32768)
                    {
                        var stringBuilder = _lineBuilder;
                        _lineBuilder = null;

                        return stringBuilder.ToString(0, stringBuilder.Length);
                    }

                    return _lineBuilder.ToString(0, _lineBuilder.Length);
                }

                begin = _currentDecodePosition;
                end = FindNextEndOfLineIndex();

                if (end < _decodedCount && end >= begin)
                {
                    _lineBuilder.Append(_decodedBuffer, begin, end - begin);

                    if (_lineBuilder.Capacity > 32768)
                    {
                        var stringBuilder = _lineBuilder;
                        _lineBuilder = null;

                        return stringBuilder.ToString(0, stringBuilder.Length);
                    }

                    return _lineBuilder.ToString(0, _lineBuilder.Length);
                }
            }
        }

        public override String ReadToEnd()
        {
            var stringBuilder = new StringBuilder();

            var size = _decodedBuffer.Length;
            var buffer = new Char[size];
            int bytesRead;

            while ((bytesRead = Read(buffer, 0, size)) > 0)
                stringBuilder.Append(buffer, 0, bytesRead);

            return stringBuilder.ToString();
        }

        public override Int32 Peek()
        {
            if (_currentDecodePosition >= _decodedCount && (_mayBlock || ReadBuffer() == 0))
                return -1;

            return _decodedBuffer[_currentDecodePosition];
        }

        public override Int32 Read()
        {
            if (_currentDecodePosition >= _decodedCount && ReadBuffer() == 0)
                return -1;

            return _decodedBuffer[_currentDecodePosition++];
        }

        public override Int32 Read([In, Out] Char[] destinationBuffer, Int32 index, Int32 count)
        {
            int charsRead = 0;

            while (count > 0)
            {
                if (_currentDecodePosition >= _decodedCount && ReadBuffer() == 0)
                    return charsRead > 0 ? charsRead : 0;

                var cch = Math.Min(_decodedCount - _currentDecodePosition, count);

                Array.Copy(_decodedBuffer, _currentDecodePosition, destinationBuffer, index, cch);

                _currentDecodePosition += cch;

                index += cch;
                count -= cch;
                charsRead += cch;
            }

            return charsRead;
        }

        public override void Close()
        {
            Dispose(true);
        }

        protected override void Dispose(Boolean disposing)
        {
            try
            {
                if (disposing && _baseStream != null)
                {
                    _baseStream.Close();
                }

                _inputBuffer = null;
                _decodedBuffer = null;
                _encoding = null;
                _decoder = null;
                _baseStream = null;
            }
            finally
            {
                base.Dispose(disposing);
            }
        }

        // ===========================================================================
        // = Private Methods
        // ===========================================================================
        
        private Int32 ReadBuffer()
        {
            _currentDecodePosition = 0;
            _decodedCount = 0;

            Int32 currentByteEncoded = 0;

            do
            {
                currentByteEncoded = _baseStream.Read(_inputBuffer, 0, _bufferSize);

                if (currentByteEncoded <= 0)
                    return 0;

                _mayBlock = (currentByteEncoded < _bufferSize);
                _decodedCount += _decoder.GetChars(_inputBuffer, 0, currentByteEncoded, _decodedBuffer, 0);

            } while (_decodedCount == 0);

            return _decodedCount;
        }

        private Int32 FindNextEndOfLineIndex()
        {
            var curChar = '\0';

            while (_currentDecodePosition < _decodedCount)
            {
                curChar = _decodedBuffer[_currentDecodePosition];

                if (curChar == '\n' && _foundCarriageReturn)
                {
                    _currentDecodePosition++;

                    var res = _currentDecodePosition - 2;

                    if (res < 0)
                        res = 0; // If a new array starts with a \n and there was a \r at the end of the previous one, we get here.

                    _foundCarriageReturn = false;
                    return res;
                }

                _foundCarriageReturn = curChar == '\r';
                _currentDecodePosition++;
            }

            return -1;
        }
    }
}


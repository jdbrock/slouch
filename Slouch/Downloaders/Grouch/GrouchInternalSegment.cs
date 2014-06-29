using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Linq;

namespace Slouch
{
    public class GrouchInternalSegment
    {
        public Int32 Number { get; private set; }
        public Int64 Length { get; private set; }
        public String MessageId { get; private set; }

        public GrouchInternalFile File { get; set; }

        public GrouchInternalSegment(Int32 inNumber, Int64 inLength, String inMessageId)
        {
            Number = inNumber;
            Length = inLength;
            MessageId = inMessageId;
        }

        public static GrouchInternalSegment FromXml(XElement inElement)
        {
            var length    = inElement.Attribute("bytes").Value;
            var number    = inElement.Attribute("number").Value;
            var messageId = inElement.Value;

            Int64 lengthAsInt64;

            if (!Int64.TryParse(length, out lengthAsInt64))
                throw new ApplicationException("NZB segment has an invalid length: " + length);

            Int32 numberAsInt32;

            if (!Int32.TryParse(number, out numberAsInt32))
                throw new ApplicationException("NZB segment has an invalid number: " + number);

            return new GrouchInternalSegment(numberAsInt32, lengthAsInt64, messageId);
        }
    }
}

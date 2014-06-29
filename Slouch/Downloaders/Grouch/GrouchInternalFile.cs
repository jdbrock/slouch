using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Linq;

namespace Slouch
{
    public class GrouchInternalFile
    {
        public String Poster { get; private set; }
        public Int64 Timestamp { get; private set; }
        public String Subject { get; private set; }

        public IList<GrouchInternalGroup> Groups { get; private set; }
        public IList<GrouchInternalSegment> Segments { get; private set; }

        public GrouchInternalFile(String inPoster, Int64 inTimestamp, String inSubject,
            IList<GrouchInternalGroup> inGroups, IList<GrouchInternalSegment> inSegments)
        {
            Poster = inPoster;
            Timestamp = inTimestamp;
            Subject = inSubject;

            Groups = inGroups;
            Segments = inSegments;

            foreach (var segment in inSegments)
                segment.File = this;
        }

        public static GrouchInternalFile FromXml(XElement inElement)
        {
            var poster    = inElement.Attribute("poster").Value;
            var timestamp = inElement.Attribute("date").Value;
            var subject   = inElement.Attribute("subject").Value;

            Int64 timestampAsInt64;

            if (!Int64.TryParse(timestamp, out timestampAsInt64))
                throw new ApplicationException("NZB file has invalid timestamp: " + timestamp);

            var groups = inElement.Element("groups").Elements("group")
                .Select(X => GrouchInternalGroup.FromXml(X))
                .ToList();

            var segments  = inElement.Element("segments").Elements("segment")
                .Select(X => GrouchInternalSegment.FromXml(X))
                .ToList();

            return new GrouchInternalFile(poster, timestampAsInt64, subject, groups, segments);
        }
    }
}

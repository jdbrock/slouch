using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Linq;

namespace Slouch
{
    public class GrouchInternalGroup
    {
        public String Name { get; private set; }

        public GrouchInternalGroup(String inName)
        {
            Name = inName;
        }

        public static GrouchInternalGroup FromXml(XElement inElement)
        {
            var name = inElement.Value;

            return new GrouchInternalGroup(name);
        }
    }
}

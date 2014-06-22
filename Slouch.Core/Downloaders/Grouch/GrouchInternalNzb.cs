using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;
using System.Xml.Linq;
using System.Xml.XPath;

namespace Slouch.Core
{
    public class GrouchInternalNzb
    {
        public Dictionary<String, String> MetaData { get; private set; }
        public IList<GrouchInternalFile> Files { get; private set; }

        public GrouchInternalNzb(Dictionary<String, String> inMetaData, IList<GrouchInternalFile> inFiles)
        {
            MetaData = inMetaData;
            Files = inFiles;
        }

        public static GrouchInternalNzb FromXml(XDocument inDocument)
        {
            var doc = StripNamespaces(inDocument.Root);

            var meta = doc.XPathSelectElements("head/meta")
                .Select(X => new { Key = X.Attribute("type").Value, Value = X.Value })
                .ToDictionary(X => X.Key, X => X.Value);

            var files = doc.XPathSelectElements("file")
                .Select(X => GrouchInternalFile.FromXml(X))
                .ToList();

            return new GrouchInternalNzb(meta, files);
        }

        private static XElement StripNamespaces(XElement inRoot)
        {
            var res = new XElement(
                inRoot.Name.LocalName,
                inRoot.HasElements
                    ? inRoot.Elements().Select(X => StripNamespaces(X))
                    : (Object)inRoot.Value
            );

            res.ReplaceAttributes(inRoot.Attributes().Where(X => (!X.IsNamespaceDeclaration)));

            return res;
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Slouch
{
    public class CabinetKeyAttribute : Attribute
    {
        // ===========================================================================
        // = Public Properties
        // ===========================================================================
        
        public String[] KeyFormat { get; set; }

        // ===========================================================================
        // = Private Fields
        // ===========================================================================

        private static Dictionary<Type, Object[]> _cache;

        // ===========================================================================
        // = Construction
        // ===========================================================================

        static CabinetKeyAttribute()
        {
            _cache = new Dictionary<Type, Object[]>();
        }

        public CabinetKeyAttribute(params String[] inKeyFormat)
        {
            KeyFormat = inKeyFormat;
        }

        // ===========================================================================
        // = Public Methods
        // ===========================================================================

        public static Object[] TryGetKey(Object inObject)
        {
            var type = inObject.GetType();

            if (_cache.ContainsKey(type))
                return _cache[type];

            var attribute = type.GetCustomAttributes(typeof(CabinetKeyAttribute), false).FirstOrDefault() as CabinetKeyAttribute;

            if (attribute == null)
                return _cache[type] = null;

            var keys = new List<Object>();

            foreach (var keyFormatPiece in attribute.KeyFormat)
                keys.Add(type.GetProperty(keyFormatPiece).GetValue(inObject));

            return keys.ToArray();
        }
    }
}

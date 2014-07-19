using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Slouch
{
    public class Cabinet
    {
        // ===========================================================================
        // = Private Fields
        // ===========================================================================
        
        private static readonly Boolean _typeAsKey = true;
        private static readonly Object _lock = new Object();

        private static String _basePath;

        // ===========================================================================
        // = Construction
        // ===========================================================================

        public static void Initialise(String inBasePath)
        {
            _basePath = inBasePath;
        }

        // ===========================================================================
        // = Public Methods
        // ===========================================================================

        public static IEnumerable<Object[]> GetSubkeys<T>(Boolean inRecurse, params Object[] inKeys)
        {
            var fullKeys = GetFullKeys<T>(inKeys, _typeAsKey);

            // Skip the first part of each key as it's the type.
            // TODO: Clean this up.
            if (_typeAsKey)
                return GetSubkeysInternal(inRecurse, fullKeys).Select(X => X.Skip(1).ToArray());
            else
                return GetSubkeysInternal(inRecurse, fullKeys);
        }

        public static async Task<T> GetAsync<T>(params Object[] inKeys)
        {
            var path = GetFilePath<T>(inKeys, _typeAsKey);

            return await GetAsyncInternal<T>(path);
        }

        public static async Task SetAsync<T>(T inData, params Object[] inKeys)
        {
            await Task.Run(() =>
            {
                var keys = inKeys;

                if (keys == null || keys.Length == 0)
                    keys = CabinetKeyAttribute.TryGetKey(inData);

                var path = GetFilePath<T>(keys, _typeAsKey);
                var dir = Path.GetDirectoryName(path);

                Directory.CreateDirectory(dir);

                lock (_lock) // TODO: Beef up locking.
                    using (var file = new FileStream(path, FileMode.Create, FileAccess.Write, FileShare.None, 4096, false))
                    {
                        var writer = new StreamWriter(file);
                        var data = fastJSON.JSON.ToJSON(inData);

                        writer.Write(data);
                        writer.Flush();
                        writer.Close();
                    }
            });
        }

        public static Boolean Exists<T>(params Object[] inKeys)
        {
            var path = GetFilePath<T>(inKeys, _typeAsKey);
            return File.Exists(path) || Directory.Exists(path);
        }

        // ===========================================================================
        // = Private Methods
        // ===========================================================================
        
        private static IEnumerable<Object[]> GetSubkeysInternal(Boolean inRecurse, Object[] inKeys)
        {
            var path = GetFilePathWithoutType(inKeys);

            // Have we been given a file?
            if (File.Exists(path))
            {
                yield return inKeys;
                yield break;
            }

            foreach (var file in Directory.GetFiles(path).Select(X => Path.GetFileName(X)))
                yield return inKeys.Concat(new[] { file }).ToArray();

            foreach (var dir in Directory.GetDirectories(path).Select(X => Path.GetFileName(X)))
                if (inRecurse)
                    foreach (var keys in GetSubkeysInternal(inRecurse, inKeys.Concat(new[] { dir }).ToArray()))
                        yield return keys;
                else
                    yield return inKeys.Concat(new[] { dir }).ToArray();
        }

        private static async Task<T> GetAsyncInternal<T>(String inPath)
        {
            return await Task.Run(() =>
            {
                lock (_lock) // TODO: Beef up locking.
                    using (var file = new FileStream(inPath, FileMode.Open, FileAccess.Read, FileShare.Read, 4096, false))
                    {
                        var reader = new StreamReader(file);
                        var data = reader.ReadToEnd();

                        return fastJSON.JSON.ToObject<T>(data);
                    }
            });
        }

        private static String GetFilePath<T>(IEnumerable<Object> inKeys, Boolean inIncludeType)
        {
            var keys = inKeys;

            if (inIncludeType)
                keys = new[] { typeof(T).Name }.Concat(inKeys);

            return Path.Combine(_basePath, String.Join(Path.DirectorySeparatorChar.ToString(), keys));
        }

        private static String GetFilePathWithoutType(IEnumerable<Object> inKeys)
        {
            return Path.Combine(_basePath, String.Join(Path.DirectorySeparatorChar.ToString(), inKeys));
        }

        private static Object[] GetFullKeys<T>(IEnumerable<Object> inKeys, Boolean inIncludeType)
        {
            var keys = inKeys;

            if (_typeAsKey)
                keys = new[] { typeof(T).Name }.Concat(inKeys);

            return keys.ToArray();
        }
    }
}

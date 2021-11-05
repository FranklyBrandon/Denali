using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

using System.Text.Json;
using System.Threading;

namespace Denali.Shared.Utility
{
    public class FileHelper
    {
        public static async Task WriteJSONToFile(object content, string filename)
        {
            var fullPath = AppDomain.CurrentDomain.BaseDirectory;
            var directory = Path.Combine(fullPath, filename + ".txt");
            var json = JsonSerializer.Serialize(content);

            await File.WriteAllTextAsync(directory, json);
        }

        public static async Task<T> DeserializeJSONFromFile<T>(string filename, CancellationToken cancellationToken = default)
        {
            var fullPath = AppDomain.CurrentDomain.BaseDirectory;
            var directory = Path.Combine(fullPath, filename);
            var json = File.OpenRead(directory);
            return await JsonSerializer.DeserializeAsync<T>(json);
        }
    }
}

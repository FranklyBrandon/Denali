using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

using System.Text.Json;

namespace Denali.Shared.Utility
{
    public static class FileHelper
    {
        public static async Task WriteJSONToFile(object content, string filename)
        {
            var fullPath = AppDomain.CurrentDomain.BaseDirectory;
            var directory = Path.Combine(fullPath, filename + ".txt");
            var json = JsonSerializer.Serialize(content);

            await File.WriteAllTextAsync(directory, json);
        }
    }
}

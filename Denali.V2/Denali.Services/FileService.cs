using Alpaca.Markets;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace Denali.Services
{
    public class FileService
    {
        public async Task<T> LoadDataFromFile<T>(string filename)
        {
            var directory = AppDomain.CurrentDomain.BaseDirectory;
            var fullpath = Path.Combine(directory, filename);
            var json = File.OpenRead(fullpath);
            return await JsonSerializer.DeserializeAsync<T>(json);
        }
    }
}

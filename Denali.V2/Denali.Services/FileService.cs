using System.Text.Json;

namespace Denali.Services
{
    public class FileService
    {
        public async Task<T> LoadResourceFromFile<T>(string filename)
        {
            var directory = AppDomain.CurrentDomain.BaseDirectory;
            var fullpath = Path.Combine(directory, filename);
            var json = File.OpenRead(fullpath);
            return await JsonSerializer.DeserializeAsync<T>(json);
        }
    }
}

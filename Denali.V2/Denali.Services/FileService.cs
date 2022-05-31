using System.Text.Json;

namespace Denali.Services
{
    public class FileService
    {
        public async Task<T> LoadJSONResourceFromFile<T>(string filename)
        {
            var json = File.OpenRead(DirectoryPath(filename));
            return await JsonSerializer.DeserializeAsync<T>(json);
        }

        public async Task WriteJSONResourceToFile(string filename, object data)
        {
            var json = JsonSerializer.Serialize(data);
            await File.AppendAllTextAsync(DirectoryPath(filename), json);
        }

        private static string DirectoryPath(string filename) => 
            Path.Combine(AppDomain.CurrentDomain.BaseDirectory, filename);
    }
}

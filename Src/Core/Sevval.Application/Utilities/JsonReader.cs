using System.Text.Json;

namespace Sevval.Application.Utilities
{

    public static class JsonReaderHelper
    {
        public static async Task<JsonDocument> ReadJsonFileAsync(string filePath)
        {
            try
            {
                using (FileStream fileStream = File.OpenRead(filePath))
                {
                    return await JsonDocument.ParseAsync(fileStream);
                }
            }
            catch (FileNotFoundException ex)
            {
                Console.WriteLine($"File not found: {ex.Message}");
                return null;
            }
            catch (JsonException ex)
            {
                Console.WriteLine($"Error parsing JSON: {ex.Message}");
                return null;
            }
        }
    }
}

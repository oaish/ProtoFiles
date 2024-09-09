using System.Text;
using System.Text.Json;
using ProtoFiles.Lib.Dto;
using BCryptNet = BCrypt.Net.BCrypt;

namespace ProtoFiles.API;

public static class Helper
{
    public static string ToHexString(this Guid guid)
    {
        return guid.ToString().Substring(0, 16);
    }

    public static string Hash(this string? value)
    {
        return value != null ? BCryptNet.HashPassword(value) : string.Empty;
    }

    public static bool VerifyHash(this string? value, string? hash)
    {
        return value != null && BCryptNet.Verify(value, hash);
    }

    public static async Task<string?> GenerateToken(this LoginUserDto dto, HttpClient client, string baseUrl)
    {
        var payload = new
        {
            dto.Username,
            UserId = Guid.NewGuid()
        };

        var jsonPayload = JsonSerializer.Serialize(payload);
        var content = new StringContent(jsonPayload, Encoding.UTF8, "application/json");

        try
        {
            var response = await client.PostAsync($"{baseUrl}/api/Token/Generate", content);

            if (!response.IsSuccessStatusCode)
                return null;

            var token = await response.Content.ReadAsStringAsync();
            return token;
        }
        catch (HttpRequestException ex)
        {
            Console.WriteLine($"Request error: {ex.Message}");
            return null;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Unexpected error: {ex.Message}");
            return null;
        }
    }
}

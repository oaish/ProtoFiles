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

    public static bool VerifyHash(this string? value, string? salt)
    {
        return value != null && BCryptNet.Verify(value, salt);
    }
}
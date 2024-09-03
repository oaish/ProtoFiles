namespace ProtoFiles.API;

public static class Helper
{
    public static string ToHexString(this Guid guid)
    {
        return guid.ToString().Substring(0, 16);
    }
}
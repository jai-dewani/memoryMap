class RedisConfig
{
    public static string Role;

    public static List<string> GetConfig()
    {
        var response = new List<string>();
        response.Add($"role:{Role}");
        return response;
    }
}
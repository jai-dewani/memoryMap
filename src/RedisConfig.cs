class RedisConfig
{
    public static string Role;

    public static List<string> GetConfig()
    {
        var response = new List<string>();
        response.Append($"# Replication");
        response.Append($"role:{Role}");
        return response;
    }
}
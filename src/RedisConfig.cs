class RedisConfig
{
    public static string Role;

    public static string[] GetConfig()
    {
        var response = new string[2];
        response.Append($"# Replication");
        response.Append($"role:{Role}");
        return response;
    }
}
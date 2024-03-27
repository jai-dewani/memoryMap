class RedisConfig
{
    public static string Role;
    public static int Port;

    public static string ReplicationID;

    public static int Offset;

    public static List<string> GetConfig()
    {
        var response = new List<string>();
        response.Add($"role:{Role}");
        response.Add($"master_replid:{ReplicationID}");
        response.Add($"master_repl_offset:{Offset}");
        return response;
    }
}
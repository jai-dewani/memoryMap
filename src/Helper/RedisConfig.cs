class RedisConfig
{
    public static string MasterHostUrl;
    public static int MasterHostPort;
    public static int Port;
    public static string? Role;
    public static string? ReplicationID;
    public static int Offset;

    public static string GetConfig()
    {
        var response = new List<string>
        {
            $"role:{Role}",
            $"master_replid:{ReplicationID}",
            $"master_repl_offset:{Offset}"
        };
        return string.Join("\r\n", response);
    }
}
public class RedisKeyVault
{
    private Dictionary<string, RedisValueModel> store = new Dictionary<string, RedisValueModel>();

    public RedisValueModel Get(string key)
    {
        return store[key];
    }

    public void Set(string key, RedisValueModel value)
    {
        store[key] = value;
    }
}

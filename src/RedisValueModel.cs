public class RedisValueModel
{
    public string value { get; protected set; }
    public DateTime? expiry { get; protected set; }

    public RedisValueModel(string value, DateTime? expiry)
    {
        this.value = value;
        this.expiry = expiry;
    }
}


class ParseCommandLineArguments
{
    public Dictionary<string, List<string>> args = new Dictionary<string, List<string>>();

    public ParseCommandLineArguments Parse(string[] argument)
    {
        var argValue = new List<string>();
        string key = "";
        for (int i = 0; i < argument.Length; i += 1)
        {
            if (argument[i].Contains("--"))
            {
                this.args.Add(key, argValue.Clone());
                argValue = new List<string>();
                key = argument[i].TrimStart('-');
            }
            else
            {
                argValue.Add(argument[i]);
            }
        }
        this.args.Add(key,argValue.Clone());
        return this;
    }
}
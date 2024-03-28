
public static class ExtensionMethods
{
    public static List<string> Clone(this List<string> str){
        var copy = new string[str.Count];
        str.CopyTo(copy);
        return copy.ToList();
    }
}

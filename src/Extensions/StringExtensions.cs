namespace Extensions;

public static class StringExtensions
{
    public static string Subtract(this string input, string substring)
    {
        return input.Contains(substring) ? input.Replace(substring, string.Empty) : input;
    }
}

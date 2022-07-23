
public static class ColouredConsole
{
    public static void PrintInRed(string s)
    {   
        Console.ForegroundColor = ConsoleColor.Red;
        Console.WriteLine(s);
        Console.ResetColor();
    }

    public static void PrintInGreen(string s)
    {
        Console.ForegroundColor = ConsoleColor.Green;
        Console.WriteLine(s);
        Console.ResetColor();
    }

    public static void PrintInBlue(string s)
    {
        Console.ForegroundColor = ConsoleColor.Blue;
        Console.WriteLine(s);
        Console.ResetColor();
    }

    public static void PrintInYellow(string s)
    {
        Console.ForegroundColor = ConsoleColor.Yellow;
        Console.WriteLine(s);
        Console.ResetColor();
    }

    public static void PrintInOrange(string s)
    {
        Console.ForegroundColor = ConsoleColor.DarkYellow;
        Console.WriteLine(s);
        Console.ResetColor();
    }

    public static void PrintInPurple(string s)
    {
        Console.ForegroundColor = ConsoleColor.DarkMagenta;
        Console.WriteLine(s);
        Console.ResetColor();
    }
}
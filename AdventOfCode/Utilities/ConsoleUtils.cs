namespace AdventOfCode.Utilities;

public static class ConsoleUtils
{
    private static void ConsoleColorMethod(ConsoleColor color, string text, Action<string> method)
    {
        Console.ForegroundColor = color;
        method(text);
        Console.ResetColor();
    }

    public static void Write(ConsoleColor color, string text)
    {
        ConsoleColorMethod(color, text, Console.Write);
    }

    public static void WriteLine(ConsoleColor color, string text)
    {
        ConsoleColorMethod(color, text, Console.WriteLine);
    }
}

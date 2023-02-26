namespace ResourceKFIV;

public class Logger
{
    public static void LogInfo(string msg)
    {
        var dateTime = DateTime.Now;

        Console.Write("<[INFO][");
        Console.Write(dateTime.ToShortTimeString());
        Console.Write("]> ");
        Console.WriteLine(msg);
    }

    public static void LogWarn(string msg)
    {
        var dateTime = DateTime.Now;

        Console.Write("<[WARN][");
        Console.Write(dateTime.ToShortTimeString());
        Console.Write("]> ");
        Console.WriteLine(msg);
    }

    public static void LogError(string msg)
    {
        var dateTime = DateTime.Now;

        Console.Write("<[SHIT][");
        Console.Write(dateTime.ToShortTimeString());
        Console.Write("]> ");
        Console.WriteLine(msg);
    }
}
namespace Autoregistrar.Settings;

public static class StateManager
{
    public static Status Status { get; set; } = Status.Stopped;
    public static int StartedForUserId { get; set; }
    public static Settings? Settings { get; set; }
}
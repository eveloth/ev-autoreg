namespace EvAutoreg.Autoregistrar.State;

public static class StateManager
{
    private static Status _status = Status.Stopped;
    private static int _operatorId;
    public static bool IsOperator(int userId)
    {
        return _operatorId == userId;
    }

    public static int GetOperator()
    {
        return _operatorId;
    }

    public static void SetOperator(int userId)
    {
        _operatorId = userId;
    }

    public static Status GetStatus()
    {
        return _status;
    }

    public static void SetStatus(Status status)
    {
        _status = status;
    }

    public static bool IsStarted()
    {
        return _status == Status.Started;
    }

    public static bool IsStopped()
    {
        return _status == Status.Stopped;
    }
}
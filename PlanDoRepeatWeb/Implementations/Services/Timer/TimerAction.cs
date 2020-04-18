namespace PlanDoRepeatWeb.Implementations.Services.Timer
{
    public enum TimerAction : byte
    {
        Start = 0,
        Stop = 1,
        Pause = 2,
        Repeat = 3,
        Expire = 4,
        Delete = 5
    }
}
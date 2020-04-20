namespace PlanDoRepeatWeb.Models
{
    public enum TimerState : byte
    {
        Stopped = 0,
        Active = 1,
        Paused = 2,
        Expired = 3
    }
}
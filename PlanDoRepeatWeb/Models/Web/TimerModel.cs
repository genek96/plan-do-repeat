namespace PlanDoRepeatWeb.Models.Web
{
    public class TimerModel
    {
        public string Id { get; set; }

        public string Name { get; set; }

        public string Description { get; set; }

        public int PeriodInSeconds { get; set; }

        public int ElapsedSeconds { get; set; }

        public long LastUpdate { get; set; }    //UTC time in seconds

        public TimerState State { get; set; }
    }
}
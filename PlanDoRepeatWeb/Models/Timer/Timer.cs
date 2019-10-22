using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace PlanDoRepeatWeb.Models.Timer
{
    public enum TimerState : byte
    {
        Stoped = 0,
        Active = 1,
        Paused = 2
    }

    public class Timer
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string Id { get; set; }

        [BsonRepresentation(BsonType.ObjectId)]
        public string UserId { get; set; }

        public string Name { get; set; }

        public string Description { get; set; }

        [BsonElement("Period")]
        public int PeriodInSeconds { get; set; }

        [BsonElement("Remaining")]
        public int PassedSeconds { get; set; }

        [BsonRepresentation(BsonType.Timestamp)]
        public long LastUpdate { get; set; }    //UTC time in ticks

        [BsonRepresentation(BsonType.Int32)]
        public TimerState State { get; set; }
    }
}

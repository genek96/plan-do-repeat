using System;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace PlanDoRepeatWeb.Models.Database
{
    public enum TimerState : byte
    {
        Stopped = 0,
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

        public int PeriodInSeconds { get; set; }

        public int PassedSeconds { get; set; }

        public long LastUpdate { get; set; }    //UTC time in ticks

        [BsonRepresentation(BsonType.Int32)]
        public TimerState State { get; set; }
    }
}

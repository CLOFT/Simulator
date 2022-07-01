using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace CLOFT.SerenUp.Simulator.Entities
{
    [JsonObject(MemberSerialization.OptIn)]
    public class Message
    {
        [JsonProperty]
        public string Time { get; set; }
        [JsonProperty]
        public Guid SerialNumber { get; set; }
        [JsonProperty]
        public string Position { get; set; }
        [JsonProperty]
        public int Steps { get; set; }
        [JsonProperty]
        public int Heartbeat { get; set; }
        [JsonProperty]
        public BloodPressure BloodPressure { get; set; }
        [JsonProperty]
        public int OxygenSaturation { get; set; }
        [JsonProperty]
        public int Battery { get; set; }
        [JsonProperty]
        public string Alarm { get; set; }
        public Accelerometer? Accelerometer { get; set; }

        public Status Status { get; set; }





    }
}
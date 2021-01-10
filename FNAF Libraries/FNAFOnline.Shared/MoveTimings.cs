using Stx.Serialization;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System;

namespace FNAFOnline.Shared
{
    public class MoveTimings : IByteDefined<MoveTimings>
    {
        public float SecondsPerMove { get; set; } = 15f;
        public float MaxRandomness { get; set; } = 7.5f;
        public float DecreaseOverNight { get; set; } = 5f;
        public float StartWait { get; set; } = 8f;

        public static MoveTimings RandomTimings 
        {
            get {
                Random random = new Random();
                return new MoveTimings
                {
                    StartWait = (float)random.NextDouble() * 13f + 2f,
                    MaxRandomness = (float)random.NextDouble() * 6f + 2f,
                    DecreaseOverNight = (float)random.NextDouble() * 3f + 4f,
                    SecondsPerMove = (float)random.NextDouble() * 7f + 12f,
                };
            }
        }

        public override string ToString()
        {
            return $"SecondsPerMove={SecondsPerMove} MaxRandomness={MaxRandomness} DecreaseOverNight={DecreaseOverNight} StartWait={StartWait}";
        }
    }
}
